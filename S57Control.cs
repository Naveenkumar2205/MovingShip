using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ENCX;

namespace Encx.Wpf
{

    /// <summary>
    /// This control draws an S57 chart within its bounds.
    /// It supports drag-scrolling and mousewheel zooming.
    /// An event <c>OnBitmapUpdated</c> fires when the bitmap data is updated (to allow overlaid elements to be repositioned).
    /// </summary>
    /// <remarks>
    /// Usage:
    /// Create the element (in XAML or otherwise), then ensure the properties <c>S57Manager</c> and <c>ENCX</c> are set
    /// to valid instances of ENCX.S57Manager and ENCX.Library respectively.
    /// 
    /// You can now control the chart display programmatically with the DisplayScale and GeographicCenter properties.
    /// The ENCX S57Draw object is also exposed as a property to allow configuration of chart display settings etc.
    /// </remarks>
    /// 
    public class S57Control : FrameworkElement, INotifyPropertyChanged
    {
        
        #region ENCX API
        private S57Draw m_S57Draw = new S57Draw();
        public S57Draw S57Draw
        {
            get
            {
                return m_S57Draw;
            }
            private set
            {
                m_S57Draw = value;
            }
        }
        private S57Manager m_S57Manager;
        private S57CatalogueObserver m_catObserver = new S57CatalogueObserver();
        public S57Manager S57Manager
        {
            get
            {
                return m_S57Manager;
            }

            set
            {
                if (m_S57Manager != value)
                {
                    if (m_S57Manager != null)
                    {
                        m_catObserver.OnChanged -= new _IS57CatalogueObserverEvents_OnChangedEventHandler(OnCatChanged);
                    }

                    m_S57Manager = value;

                    m_S57Draw.SetManager(m_S57Manager);

                    // draw background colour so that anti-aliasing works
                    m_S57Draw.BackgroundColour = (uint)System.Drawing.Color.White.ToArgb();

                    m_catObserver.OnChanged += new _IS57CatalogueObserverEvents_OnChangedEventHandler(OnCatChanged);
                    m_catObserver.Register(m_S57Manager);

                    NotifyChanged("S57Manager");

                    UpdateBitmap();
                }
            }
        }

        // Any ENCX app needs one (and only one) ENCX.Library object. Give this to this class and it
        // will redraw if the dongle status changes. Dongles are sometimes not fully ready until a few
        // seconds in the run of an app and what gets drawn often depends on the dongle!

        private static ENCX.Library m_encxSdk;
        private ENCX.DongleObserver m_dongleObserver = new DongleObserver();
        public ENCX.Library ENCX
        {
            get { return m_encxSdk; }
            set
            {
                if ((m_encxSdk != null) && (!m_encxSdk.Equals(value)))
                {
                    throw new Exception("You should only have one instance of ENCX.Library");
                }

                m_encxSdk = value;
                m_dongleObserver.OnDongleChangeEvt += new _IDongleObserverEvents_OnDongleChangeEvtEventHandler(() =>
                {
                    UpdateBitmap();
                    NotifyChanged("ENCX");
					NotifyChanged("DongleStateDescription");
                    this.InvalidateVisual();
                });

                m_dongleObserver.Register(m_encxSdk.Dongle);
            }
        }

        void OnCatChanged()
        {
            UpdateBitmap();
        }
        
        #endregion

        #region Movement & zooming

        private int m_displayScale;
        public int DisplayScale
        {
            get { return m_displayScale; }
            set
            {
                if (m_displayScale != value)
                {
                    m_displayScale = value;
                    m_S57Draw.SetDisplayScale(m_displayScale, new PixelPoint { X = -1, Y = -1 });
                    NotifyChanged("DisplayScale");
                }
            }
        }

        public void SetDisplayScale(int displayScale, ENCX.PixelPoint pp)
        {
            m_displayScale = displayScale;
            m_S57Draw.SetDisplayScale(m_displayScale, pp);
            NotifyChanged("DisplayScale");
        }
        
        private GeoPoint m_geographicCenter = new GeoPoint { Lat = 53.0, Lon = 1.0 };
        public GeoPoint GeographicCenter
        {
			get { return m_geographicCenter; }
			set
			{
				if (!m_geographicCenter.Equals(value))
				{
					m_geographicCenter = value;
					m_S57Draw.SetPosition(m_geographicCenter, new PixelPoint { X = -1, Y = -1 });
					NotifyChanged("GeographicCenter");
					NotifyChanged("GeographicCenterLat");
					NotifyChanged("GeographicCenterLon");
				}
			}
        }
		//public double GeographicCenterLat { get => m_geographicCenter.Lat; }
		//public double GeographicCenterLon { get => m_geographicCenter.Lon; }

		public void RotateMidpointTo(double angle)
        {
            S57Draw.SetDirectionOfUp(angle, new PixelPoint() { X = -1, Y = -1 });
            this.UpdateBitmap();
        }


        public void SetPositionAndScale(GeoRect gr)
        {
            S57Draw.SetPositionAndScale(gr);
            GeographicCenter = S57Draw.PositionOfCentre;
            DisplayScale = S57Draw.DisplayScale;
            UpdateBitmap();
        }


        public void Zoom(int direction, ENCX.PixelPoint cursorPoint)
        {
            bool allowed = false;
            int scale;
            S57Draw.CalculateNewDisplayScale(out allowed, out scale, DisplayScale, direction, 1);
            if (allowed)
            {
                SetDisplayScale(scale, cursorPoint);
                this.UpdateBitmap();
            }
        }

        public void UpdatePosition(GeoPoint gp)
        {
            GeographicCenter = gp;
            UpdateBitmap();
        }
        
		public void Refresh() 
		{
			UpdateBitmap();
			InvalidateVisual();
		}

		public string DongleStateDescription
		{
			get
			{
				if (ENCX == null)
					return string.Empty;

				switch (ENCX.Dongle.State)
				{
					case DongleState.DS_Active: return "Dongle Active";
					case DongleState.DS_Exists: return "Dongle Exists";
					case DongleState.DS_NotFound: return "Dongle not found";
					default: return string.Empty;
				}
			}
			set { }
		}

        #endregion

        #region Default mouse handling
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_dragOrigin != null)
            {
                var mousePos = Mouse.GetPosition(this);
                var pp = new PixelPoint() { X = (float)mousePos.X, Y = (float)mousePos.Y };
                var gp = S57Draw.GeoPix.GeoPointFromPixelPoint(pp);
                S57Draw.SetPosition(m_dragOrigin, pp);
                GeographicCenter = S57Draw.PositionOfCentre;
                this.UpdateBitmap();
                this.InvalidateVisual();
            }
        }

        protected GeoPoint m_dragOrigin = null;

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                m_dragOrigin = null;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
            {
                var mousePos = Mouse.GetPosition(this);
                var pp = new PixelPoint() { X = (float) mousePos.X, Y = (float) mousePos.Y };
                m_dragOrigin = S57Draw.GeoPix.GeoPointFromPixelPoint(pp);
            }
            e.Handled = true;
        }

		private double _mouseLat = 0.0;
		public double MouseLat
		{
			get { return _mouseLat; }
			set
			{
				if(value != _mouseLat)
				{
					_mouseLat = value;
				}
			}
		}
		private double _mouseLon = 0.0;
		public double MouseLon
		{
			get { return _mouseLon; }
			set
			{
				if (value != _mouseLon)
				{
					_mouseLon = value;
				}
			}
		}
        #endregion

        #region Rendering
        private BitmapSource m_Chart;

        public BitmapSource Chart
        {
            get
            {
                return m_Chart;
            }
            private set
            {
                m_Chart = value;
                NotifyChanged("Chart");
            }
        }
        private void UpdateBitmap()
        {
            if (m_S57Manager == null || (this.RenderSize.Height == 0 || this.RenderSize.Width == 0) || this.Visibility != System.Windows.Visibility.Visible)
            {
                // nothing to do
                Chart = null;
                return;
            }

            var pxSize = new ENCX.PixelSize { Width = (int)this.RenderSize.Width, Height = (int)this.RenderSize.Height };
            m_S57Draw.SetSize(pxSize);

            using (var gdiImage = new System.Drawing.Bitmap(pxSize.Width, pxSize.Height))
            {
				using (var g = System.Drawing.Graphics.FromImage(gdiImage))
				{
					float fdpix = g.DpiX;
					double dOnePixelMetres = 25.4e-3 / fdpix;
					m_S57Draw.PixelSizeMetres = dOnePixelMetres;

					System.IntPtr iphDC = g.GetHdc();
					m_S57Draw.Draw(iphDC.ToInt32());
					g.ReleaseHdc(iphDC);

					Chart = gdiImage.ToBitmapSource();
				}
            }
            // reduce memory usage when resizing the window
            GC.Collect();

            // notify listeners that the bitmap was updated
            if (this.OnBitmapUpdated != null)
                this.OnBitmapUpdated();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateBitmap();
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(Chart, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            base.OnRender(drawingContext);
        }

        public delegate void BitmapUpdated();

        public event BitmapUpdated OnBitmapUpdated;

        #endregion

        static S57Control()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(S57Control), new FrameworkPropertyMetadata(typeof(S57Control)));
        }

        private void NotifyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

	}
}
