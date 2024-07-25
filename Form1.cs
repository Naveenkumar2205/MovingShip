using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using EncViewWithCSharp.Properties;
using ENCX;
using EncXControls;
using LatLongRead;
using ScreenRecorderLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace EncViewWithCSharp
{
    public class FormEnc : XtraForm
    {
        private readonly S57CatalogueObserver m_catNotify = new S57CatalogueObserver();
        private readonly DongleObserver m_dongleNotify = new DongleObserverClass();
        private readonly S57Draw m_draw = new S57DrawClass();
        private readonly Library m_lib = new LibraryClass();
        private readonly S57Manager m_senc = new S57ManagerClass();
        private static GeoPoint _dragStartGeoPoint;
        private System.ComponentModel.IContainer components;
        private ChartMode _currentChartMode;
        private Image _mark, _anchorImage, _homeImage, _waypointImage, _routeIcon, _conformationIcon;
        private Bitmap _measureIcon, _aisImage, _orangeaisImage, _greenaisImage, _redaisImage;
        private static Bitmap _usvImage, _goalPointimage;
        private GeoPoint _markGeoPoint, _clickGeoPt;
        private static GeoCoordinate clickGeoCordinate, _anchorGeoCoordinate, _cursorGeocoordinate, lastPosition, lastaisPosition;
        private GeoCoordinate HomeLocation;
        public Timer UpdateTimer;
        public static List<Tuple<GeoCoordinate, double>> usvTrail = new List<Tuple<GeoCoordinate, double>>();
        public static List<Tuple<GeoCoordinate, double>> aisTrail = new List<Tuple<GeoCoordinate, double>>();
        private static Rectangle trailRectangle, trailaisRectangle, usvRectangle, aisRectangle;
        private static double lastPositionAngle, lastaisPositionAngle, trueTotalDistance;
        private static PixelPoint _cursorPixelPoint, _defaultPixelPoint, clickPixelPoint;
        internal S57Control S57Control1;
        private PanelControl panelControl1;
        private LayoutControl layoutControl3, layoutControl1, layoutControl6, layoutControl2;
        private static string _wayPointPath, _recordingPath;
        private Recorder _videoRecorder, _snapRecorder;
        private static bool _rotatechartbuttonChecked;
        private Bar bar1,bar2, bar3;
        private BarManager barManager1;
        private BarDockControl barDockControlTop, barDockControlBottom, barDockControlLeft, barDockControlRight;
        private Timer _refreshTimer;
        private SimpleLabelItem simpleLabelItem8,simpleLabelItem1, simpleLabelItem3, simpleLabelItem4, simpleLabelItem5, simpleLabelItem6, simpleLabelItem7, simpleLabelLatitude, simpleLabelItem10, simpleLabelItemSpeed, simpleLabelItemDepth;
        private LayoutControlGroup layoutControlGroup2,layoutControlGroup4, Root, layoutControlGroup6, layoutControlGroup5;
        private SimpleLabelItem simpleLabelItem19, simpleLabelItem20, simpleLabelItem21, simpleLabelItem22, simpleLabelItem23,simpleLabelItem14, simpleLabelCursorLatitude, simpleLabelCursorAngle, simpleLabelLongitude, simpleLabelItemHeading, simpleLabelcursorLongitude, simpleLabelCursorRange, messagelabel1, messagelabel2;
        private SimpleSeparator simpleSeparator1;
        private static string wayPointName, routeName, routeDescription, routeColour, routeSpeed, routeTotalDistance, routeTotalTime, _routePointsPath, _geofencePath;
        private DataTable _routePointsData, _geofenceData, _aisData, _wayPointData;
        private GroupControl groupControl4,groupControl1, groupControl3, groupControl5, groupControl7;
        private BarButtonItem barButtonItem4, barButtonItem5;
        private PopupMenu popupMenu2, popupMenu1;
        private BarStaticItem barStaticItem14, barStaticItem15,barStaticItem2, barStaticItem8,barStaticItem3, barStaticItem4, barStaticItem5, barStaticItem6, barStaticItem7, barStaticItem10, barStaticItem1, barStaticItem9, barStaticItem11, barStaticItem12, barStaticItem13;
        private List<GeoCoordinate> _routeWayPointsList;
        private SkinBarSubItem skinBarSubItem1, skinBarSubItem2;
        private static int routeIndex, startpointfoundline;
        private static double secTohr = 0.000277778;
        private static double mTonm = 0.000539957;
        private BarHeaderItem barHeaderItem2, barHeaderItem3, barHeaderItem1;
        private BarSubItem barSubItem2, barSubItem3, barSubItem1;
        private static double knotsTomps = 0.5144444444444444;
        private LabelControl labelControl1;
        private bool footfall, _routeDragStarted;
        private static bool _routeedit;
        private static Tuple<pointType, DataRow, int> pointTodragTuple;
        private BarStaticItem barStaticItem20, barStaticItem21,barStaticItem16, barStaticItem17, barStaticItem19, barStaticItem18;
        private static int _editRouteIndex = -1;
        private DevExpress.XtraBars.Navigation.TabPane tabPane3;
        private DevExpress.XtraBars.Navigation.TabNavigationPage tabNavigationPage5, tabNavigationPage6, tabNavigationPage7;
        private DevExpress.XtraGrid.GridControl gridControl1, gridControl2;
        private DevExpress.XtraGrid.Views.Card.CardView cardView1, cardView3, cardView2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12,gridColumn1, gridColumn2, gridColumn3, gridColumn4, gridColumn5, gridColumn6, gridColumn7, gridColumn8, gridColumn9, gridColumn10, gridColumn11, Latitude, Longitude;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1, repositoryItemCheckEdit2;
        private ColorPickEdit colorPickedit_routeColour;
        private MemoEdit memoedit_routeDescription, memoEdit1;
        private SpinEdit spintxtedit_usvspeed;
        private TextEdit wayptxt_Name, wayptxt_Lat, wayptxt_Lon,txtedit_routeName, textEdit1,txtedit_Totaldistance, txtedit_Timetake;
        private SimpleButton routecreate_button, routrcancel_button, simpleButton4, waypointcreatebutton, waypointcancelbutton, simpleButton3;
        private LayoutControlItem layoutControlItem3, layoutControlItem4, layoutControlItem5, layoutControlItem8, layoutControlItem6, layoutControlItem7,layoutControlItem1, layoutControlItem2, layoutControlItem9, layoutControlItem10, layoutControlItem11, layoutControlItem12, layoutControlItem13, layoutControlItem14, layoutControlItem15, layoutControlItem16, layoutControlItem17;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1, gridView2;
        private List<GeoCoordinate> _measuredList;
        private string _remoteIpAddress;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarItem navBarItem1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup1, navBarGroup2, navBarGroup3, navBarGroup4, navBarGroup5, navBarGroup6, navBarGroup7;
        private DXErrorProvider dxErrorProvider1;
        private LayoutControlGroup layoutControlGroup1,layoutControlGroup7, layoutControlGroup8, layoutControlGroup10;
        private LayoutControlItem layoutControlItem18, layoutControlItem19, layoutControlItem20, layoutControlItem21, layoutControlItem22;
        private DevExpress.XtraGrid.GridControl gridControl3, gridControl5;
        private SimpleButton simpleButton1, simpleButton2;
        private static GeoPoint usvGeoPoint;
        private LayoutControl layoutControl7,layoutControl8, layoutControl10, layoutControl5;
        private DevExpress.XtraGrid.Views.Layout.LayoutView layoutView2;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn layoutViewColumn1, layoutViewColumn2, layoutViewColumn3, layoutViewColumn4, layoutViewColumn5, layoutViewColumn6, layoutViewColumn7, layoutViewColumn8, layoutViewColumn9, layoutViewColumn10, layoutViewColumn11, layoutViewColumn12;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField1, layoutViewField2, layoutViewField3, layoutViewField4, layoutViewField5, layoutViewField6, layoutViewField7, layoutViewField8, layoutViewField9, layoutViewField10, layoutViewField11, layoutViewField12;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit2;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewCard layoutViewCard2;
        private static PixelPoint aisPixelPoint, usvPixelPoint;
        private readonly object _targetImageLocker;
        private TextEdit textEdit21, textEdit22, textEdit23, textEdit24, textEdit25, textEdit26, textEdit27, textEdit28, textEdit29, textEdit30, textEdit31, textEdit32, textEdit33, textEdit34, textEdit35, textEdit36, textEdit37, textEdit38, textEdit39;
        private LayoutControlItem layoutControlItem23,layoutControlItem43, layoutControlItem44, layoutControlItem45, layoutControlItem46, layoutControlItem47, layoutControlItem48, layoutControlItem49, layoutControlItem50, layoutControlItem51, layoutControlItem52, layoutControlItem53, layoutControlItem54, layoutControlItem55, layoutControlItem56, layoutControlItem57, layoutControlItem58, layoutControlItem59, layoutControlItem60, layoutControlItem61;
        private EmptySpaceItem emptySpaceItem2, emptySpaceItem3, emptySpaceItem1;
        private bool _aisShowtrailbuttonClicked, _aisShowrangeBarbuttonClicked;
        private WindowsUIButtonPanel windowsUIButtonPanel8, windowsUIButtonPanel7,windowsUIButtonPanel5, windowsUIButtonPanel9, windowsUIButtonPanel10, windowsUIButtonPanel3, windowsUIButtonPanel4, windowsUIButtonPanel2, windowsUIButtonPanel1;

        private enum ChartMode
        {
            Drag,
            None,
            Route,
            Measure
        }

        private enum pointType
        {
            None,
            waypoint,
            RouteWayPoint,
            RouteLine
        }

        public FormEnc()
        {
            InitializeComponent();
            _targetImageLocker = new object();

            InitializeENCXControl();

            _remoteIpAddress = ConfigurationManager.AppSettings["RemoteIpAddress"];
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions1 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions10 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions11 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions12 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions13 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions14 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions15 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions9 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions8 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions7 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions6 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEnc));
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions5 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions2 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions3 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions windowsUIButtonImageOptions4 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonImageOptions();
            this.S57Control1 = new EncXControls.S57Control();
            this.windowsUIButtonPanel1 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem3 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem4 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem5 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem6 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem7 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelLatitude = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItemHeading = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItemSpeed = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItemDepth = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem14 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelCursorLatitude = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelCursorAngle = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelLongitude = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelcursorLongitude = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelCursorRange = new DevExpress.XtraLayout.SimpleLabelItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barStaticItem20 = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
            this.barHeaderItem1 = new DevExpress.XtraBars.BarHeaderItem();
            this.barStaticItem3 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem4 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem5 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem6 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem7 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem10 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
            this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem8 = new DevExpress.XtraBars.BarStaticItem();
            this.skinBarSubItem1 = new DevExpress.XtraBars.SkinBarSubItem();
            this.skinBarSubItem2 = new DevExpress.XtraBars.SkinBarSubItem();
            this.barHeaderItem2 = new DevExpress.XtraBars.BarHeaderItem();
            this.barStaticItem9 = new DevExpress.XtraBars.BarStaticItem();
            this.barSubItem2 = new DevExpress.XtraBars.BarSubItem();
            this.barHeaderItem3 = new DevExpress.XtraBars.BarHeaderItem();
            this.barStaticItem11 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem12 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem13 = new DevExpress.XtraBars.BarStaticItem();
            this.barSubItem3 = new DevExpress.XtraBars.BarSubItem();
            this.barStaticItem14 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem15 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem16 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem17 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem18 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem19 = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem21 = new DevExpress.XtraBars.BarStaticItem();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.windowsUIButtonPanel3 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            this.windowsUIButtonPanel4 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            this.windowsUIButtonPanel2 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            this.simpleLabelItem10 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.navBarGroup1 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
            this.navBarGroup7 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroup3 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroup4 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroup5 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroup6 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroup2 = new DevExpress.XtraNavBar.NavBarGroup();
            this.groupControl4 = new DevExpress.XtraEditors.GroupControl();
            this.layoutControl6 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.simpleLabelItem8 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.windowsUIButtonPanel5 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            this.popupMenu2 = new DevExpress.XtraBars.PopupMenu(this.components);
            this.windowsUIButtonPanel7 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            this.windowsUIButtonPanel8 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.messagelabel1 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.messagelabel2 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.tabNavigationPage7 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.layoutControl7 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControl3 = new DevExpress.XtraGrid.GridControl();
            this.cardView3 = new DevExpress.XtraGrid.Views.Card.CardView();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup7 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
            this.tabNavigationPage6 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControl2 = new DevExpress.XtraGrid.GridControl();
            this.cardView2 = new DevExpress.XtraGrid.Views.Card.CardView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colorPickedit_routeColour = new DevExpress.XtraEditors.ColorPickEdit();
            this.txtedit_routeName = new DevExpress.XtraEditors.TextEdit();
            this.memoedit_routeDescription = new DevExpress.XtraEditors.MemoEdit();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.spintxtedit_usvspeed = new DevExpress.XtraEditors.SpinEdit();
            this.txtedit_Totaldistance = new DevExpress.XtraEditors.TextEdit();
            this.txtedit_Timetake = new DevExpress.XtraEditors.TextEdit();
            this.routecreate_button = new DevExpress.XtraEditors.SimpleButton();
            this.routrcancel_button = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton4 = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleLabelItem19 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.simpleLabelItem20 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleLabelItem21 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleLabelItem22 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleLabelItem23 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.tabNavigationPage5 = new DevExpress.XtraBars.Navigation.TabNavigationPage();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.cardView1 = new DevExpress.XtraGrid.Views.Card.CardView();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Latitude = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Longitude = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.wayptxt_Name = new DevExpress.XtraEditors.TextEdit();
            this.wayptxt_Lat = new DevExpress.XtraEditors.TextEdit();
            this.wayptxt_Lon = new DevExpress.XtraEditors.TextEdit();
            this.waypointcreatebutton = new DevExpress.XtraEditors.SimpleButton();
            this.waypointcancelbutton = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.tabPane3 = new DevExpress.XtraBars.Navigation.TabPane();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.groupControl5 = new DevExpress.XtraEditors.GroupControl();
            this.layoutControl8 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControl5 = new DevExpress.XtraGrid.GridControl();
            this.layoutView2 = new DevExpress.XtraGrid.Views.Layout.LayoutView();
            this.layoutViewColumn1 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn2 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField2 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn3 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField3 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn4 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField4 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn5 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField5 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn6 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField6 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn7 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.repositoryItemPictureEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.layoutViewField7 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn8 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField8 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn9 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField9 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn10 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField10 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn11 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField11 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewColumn12 = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.layoutViewField12 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewCard2 = new DevExpress.XtraGrid.Views.Layout.LayoutViewCard();
            this.layoutControlGroup8 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
            this.groupControl7 = new DevExpress.XtraEditors.GroupControl();
            this.layoutControl10 = new DevExpress.XtraLayout.LayoutControl();
            this.textEdit21 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit22 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit23 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit24 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit25 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit26 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit27 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit28 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit29 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit30 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit31 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit32 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit33 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit34 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit35 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit36 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit37 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit38 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit39 = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup10 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem43 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem44 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem45 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem46 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem47 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem48 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem49 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem50 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem51 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem52 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem53 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem54 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem55 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem56 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem57 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem58 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem59 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem60 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem61 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.windowsUIButtonPanel9 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            this.windowsUIButtonPanel10 = new DevExpress.XtraBars.Docking2010.WindowsUIButtonPanel();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelLatitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItemHeading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItemSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItemDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelCursorLatitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelCursorAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelLongitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelcursorLongitude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelCursorRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl4)).BeginInit();
            this.groupControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
            this.groupControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.messagelabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.messagelabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
            this.tabNavigationPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).BeginInit();
            this.layoutControl7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
            this.tabNavigationPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).BeginInit();
            this.layoutControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPickedit_routeColour.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtedit_routeName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoedit_routeDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spintxtedit_usvspeed.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtedit_Totaldistance.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtedit_Timetake.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.tabNavigationPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wayptxt_Name.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wayptxt_Lat.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wayptxt_Lon.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabPane3)).BeginInit();
            this.tabPane3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl5)).BeginInit();
            this.groupControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl8)).BeginInit();
            this.layoutControl8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl7)).BeginInit();
            this.groupControl7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl10)).BeginInit();
            this.layoutControl10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit21.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit22.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit23.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit24.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit25.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit26.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit27.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit28.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit29.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit30.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit31.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit32.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit33.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit34.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit35.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit36.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit37.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit38.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit39.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem43)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem44)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem45)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem46)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem47)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem48)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem49)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem50)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem51)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem52)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem53)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem54)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem55)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem56)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem57)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem58)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem59)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem60)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem61)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // S57Control1
            // 
            this.S57Control1.AutoSize = true;
            this.S57Control1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.S57Control1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.S57Control1.ForeColor = System.Drawing.Color.Transparent;
            this.S57Control1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.S57Control1.Location = new System.Drawing.Point(0, 0);
            this.S57Control1.Name = "S57Control1";
            this.S57Control1.ShowScrollBars = false;
            this.S57Control1.Size = new System.Drawing.Size(1819, 789);
            this.S57Control1.TabIndex = 3;
            this.S57Control1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.S57Control1_KeyDown);
            this.S57Control1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.S57Control1_MouseClick);
            this.S57Control1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.S57Control1_MouseDown);
            this.S57Control1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.S57Control1_MouseMove);
            this.S57Control1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.S57Control1_MouseUp);
            // 
            // windowsUIButtonPanel1
            // 
            this.windowsUIButtonPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsUIButtonPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            windowsUIButtonImageOptions1.EnableTransparency = true;
            windowsUIButtonImageOptions1.Image = global::EncViewWithCSharp.Properties.Resources.refresh_16x16;
            serializableAppearanceObject1.BackColor = System.Drawing.Color.White;
            serializableAppearanceObject1.BorderColor = System.Drawing.Color.White;
            serializableAppearanceObject1.ForeColor = System.Drawing.Color.White;
            serializableAppearanceObject1.Options.UseBackColor = true;
            serializableAppearanceObject1.Options.UseBorderColor = true;
            serializableAppearanceObject1.Options.UseForeColor = true;
            this.windowsUIButtonPanel1.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("ClearCursor", true, windowsUIButtonImageOptions1, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUISeparator(null, false, -1, false, serializableAppearanceObject1)});
            this.windowsUIButtonPanel1.Font = new System.Drawing.Font("Tahoma", 6F);
            this.windowsUIButtonPanel1.ForeColor = System.Drawing.Color.Transparent;
            this.windowsUIButtonPanel1.Location = new System.Drawing.Point(1452, 22);
            this.windowsUIButtonPanel1.Name = "windowsUIButtonPanel1";
            this.windowsUIButtonPanel1.Size = new System.Drawing.Size(69, 64);
            this.windowsUIButtonPanel1.TabIndex = 10;
            this.windowsUIButtonPanel1.Text = "windowsUIButtonPanel1";
            this.windowsUIButtonPanel1.ButtonClick += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.windowsUIButtonPanel1_ButtonClick);
            // 
            // panelControl1
            // 
            this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panelControl1.Appearance.BorderColor = System.Drawing.Color.White;
            this.panelControl1.Appearance.ForeColor = System.Drawing.Color.White;
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.Appearance.Options.UseBorderColor = true;
            this.panelControl1.Appearance.Options.UseForeColor = true;
            this.panelControl1.AutoSize = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.panelControl1.Controls.Add(this.layoutControl3);
            this.panelControl1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.panelControl1.Location = new System.Drawing.Point(485, 11);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(796, 94);
            this.panelControl1.TabIndex = 11;
            // 
            // layoutControl3
            // 
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(2, 2);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(934, 0, 650, 400);
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(792, 90);
            this.layoutControl3.TabIndex = 12;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.AllowHide = false;
            this.layoutControlGroup2.AppearanceGroup.BorderColor = System.Drawing.Color.White;
            this.layoutControlGroup2.AppearanceGroup.Options.UseBorderColor = true;
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.simpleLabelItem1,
            this.simpleLabelItem3,
            this.simpleLabelItem4,
            this.simpleLabelItem5,
            this.simpleLabelItem6,
            this.simpleLabelItem7,
            this.simpleLabelLatitude,
            this.simpleLabelItemHeading,
            this.simpleLabelItemSpeed,
            this.simpleLabelItemDepth,
            this.simpleLabelItem14,
            this.simpleLabelCursorLatitude,
            this.simpleLabelCursorAngle,
            this.simpleLabelLongitude,
            this.simpleLabelcursorLongitude,
            this.simpleLabelCursorRange});
            this.layoutControlGroup2.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
            this.layoutControlGroup2.Name = "Root";
            this.layoutControlGroup2.OptionsPrint.AppearanceItem.BorderColor = System.Drawing.Color.Transparent;
            this.layoutControlGroup2.OptionsPrint.AppearanceItem.Options.UseBorderColor = true;
            columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition1.Width = 14.28D;
            columnDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition2.Width = 14.28D;
            columnDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition3.Width = 14.28D;
            columnDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition4.Width = 14.28D;
            columnDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition5.Width = 14.28D;
            columnDefinition6.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition6.Width = 14.28D;
            columnDefinition7.SizeType = System.Windows.Forms.SizeType.Percent;
            columnDefinition7.Width = 14.28D;
            this.layoutControlGroup2.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
            columnDefinition1,
            columnDefinition2,
            columnDefinition3,
            columnDefinition4,
            columnDefinition5,
            columnDefinition6,
            columnDefinition7});
            rowDefinition1.Height = 33.33D;
            rowDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition2.Height = 33.33D;
            rowDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
            rowDefinition3.Height = 33.33D;
            rowDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
            this.layoutControlGroup2.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
            rowDefinition1,
            rowDefinition2,
            rowDefinition3});
            this.layoutControlGroup2.Size = new System.Drawing.Size(792, 90);
            // 
            // simpleLabelItem1
            // 
            this.simpleLabelItem1.AllowHotTrack = false;
            this.simpleLabelItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItem1.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem1.Location = new System.Drawing.Point(0, 0);
            this.simpleLabelItem1.Name = "simpleLabelItem1";
            this.simpleLabelItem1.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelItem1.Text = "Position";
            this.simpleLabelItem1.TextSize = new System.Drawing.Size(67, 19);
            // 
            // simpleLabelItem3
            // 
            this.simpleLabelItem3.AllowHotTrack = false;
            this.simpleLabelItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItem3.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItem3.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem3.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem3.Location = new System.Drawing.Point(113, 0);
            this.simpleLabelItem3.Name = "simpleLabelItem3";
            this.simpleLabelItem3.OptionsTableLayoutItem.ColumnIndex = 1;
            this.simpleLabelItem3.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelItem3.Text = "Heading";
            this.simpleLabelItem3.TextSize = new System.Drawing.Size(67, 19);
            // 
            // simpleLabelItem4
            // 
            this.simpleLabelItem4.AllowHotTrack = false;
            this.simpleLabelItem4.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItem4.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem4.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem4.AppearanceItemCaptionDisabled.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItem4.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            this.simpleLabelItem4.Location = new System.Drawing.Point(226, 0);
            this.simpleLabelItem4.Name = "simpleLabelItem4";
            this.simpleLabelItem4.OptionsTableLayoutItem.ColumnIndex = 2;
            this.simpleLabelItem4.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelItem4.Text = "Speed";
            this.simpleLabelItem4.TextSize = new System.Drawing.Size(67, 19);
            // 
            // simpleLabelItem5
            // 
            this.simpleLabelItem5.AllowHotTrack = false;
            this.simpleLabelItem5.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItem5.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItem5.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem5.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem5.Location = new System.Drawing.Point(339, 0);
            this.simpleLabelItem5.Name = "simpleLabelItem5";
            this.simpleLabelItem5.OptionsTableLayoutItem.ColumnIndex = 3;
            this.simpleLabelItem5.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelItem5.Text = "Depth";
            this.simpleLabelItem5.TextSize = new System.Drawing.Size(67, 19);
            // 
            // simpleLabelItem6
            // 
            this.simpleLabelItem6.AllowHotTrack = false;
            this.simpleLabelItem6.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItem6.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItem6.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem6.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem6.Location = new System.Drawing.Point(452, 0);
            this.simpleLabelItem6.Name = "simpleLabelItem6";
            this.simpleLabelItem6.OptionsTableLayoutItem.ColumnIndex = 4;
            this.simpleLabelItem6.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelItem6.Text = "Mode";
            this.simpleLabelItem6.TextSize = new System.Drawing.Size(67, 19);
            // 
            // simpleLabelItem7
            // 
            this.simpleLabelItem7.AllowHotTrack = false;
            this.simpleLabelItem7.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItem7.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItem7.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem7.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem7.Location = new System.Drawing.Point(565, 0);
            this.simpleLabelItem7.Name = "simpleLabelItem7";
            this.simpleLabelItem7.OptionsTableLayoutItem.ColumnIndex = 5;
            this.simpleLabelItem7.OptionsTableLayoutItem.ColumnSpan = 2;
            this.simpleLabelItem7.Size = new System.Drawing.Size(227, 30);
            this.simpleLabelItem7.Text = "Cursor";
            this.simpleLabelItem7.TextSize = new System.Drawing.Size(67, 19);
            // 
            // simpleLabelLatitude
            // 
            this.simpleLabelLatitude.AllowHotTrack = false;
            this.simpleLabelLatitude.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelLatitude.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelLatitude.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelLatitude.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelLatitude.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelLatitude.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelLatitude.Location = new System.Drawing.Point(0, 30);
            this.simpleLabelLatitude.Name = "simpleLabelLatitude";
            this.simpleLabelLatitude.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleLabelLatitude.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelLatitude.Text = "- - -";
            this.simpleLabelLatitude.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelItemHeading
            // 
            this.simpleLabelItemHeading.AllowHotTrack = false;
            this.simpleLabelItemHeading.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItemHeading.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItemHeading.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItemHeading.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItemHeading.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItemHeading.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItemHeading.Location = new System.Drawing.Point(113, 30);
            this.simpleLabelItemHeading.Name = "simpleLabelItemHeading";
            this.simpleLabelItemHeading.OptionsTableLayoutItem.ColumnIndex = 1;
            this.simpleLabelItemHeading.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleLabelItemHeading.OptionsTableLayoutItem.RowSpan = 2;
            this.simpleLabelItemHeading.Size = new System.Drawing.Size(113, 60);
            this.simpleLabelItemHeading.Text = "- - -";
            this.simpleLabelItemHeading.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelItemSpeed
            // 
            this.simpleLabelItemSpeed.AllowHotTrack = false;
            this.simpleLabelItemSpeed.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItemSpeed.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItemSpeed.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItemSpeed.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItemSpeed.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItemSpeed.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItemSpeed.Location = new System.Drawing.Point(226, 30);
            this.simpleLabelItemSpeed.Name = "simpleLabelItemSpeed";
            this.simpleLabelItemSpeed.OptionsTableLayoutItem.ColumnIndex = 2;
            this.simpleLabelItemSpeed.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleLabelItemSpeed.OptionsTableLayoutItem.RowSpan = 2;
            this.simpleLabelItemSpeed.Size = new System.Drawing.Size(113, 60);
            this.simpleLabelItemSpeed.Text = "- - -";
            this.simpleLabelItemSpeed.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelItemDepth
            // 
            this.simpleLabelItemDepth.AllowHotTrack = false;
            this.simpleLabelItemDepth.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItemDepth.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItemDepth.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItemDepth.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItemDepth.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItemDepth.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItemDepth.Location = new System.Drawing.Point(339, 30);
            this.simpleLabelItemDepth.Name = "simpleLabelItemDepth";
            this.simpleLabelItemDepth.OptionsTableLayoutItem.ColumnIndex = 3;
            this.simpleLabelItemDepth.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleLabelItemDepth.OptionsTableLayoutItem.RowSpan = 2;
            this.simpleLabelItemDepth.Size = new System.Drawing.Size(113, 60);
            this.simpleLabelItemDepth.Text = "- - -";
            this.simpleLabelItemDepth.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelItem14
            // 
            this.simpleLabelItem14.AllowHotTrack = false;
            this.simpleLabelItem14.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelItem14.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelItem14.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem14.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelItem14.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem14.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem14.Location = new System.Drawing.Point(452, 30);
            this.simpleLabelItem14.Name = "simpleLabelItem14";
            this.simpleLabelItem14.OptionsTableLayoutItem.ColumnIndex = 4;
            this.simpleLabelItem14.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleLabelItem14.OptionsTableLayoutItem.RowSpan = 2;
            this.simpleLabelItem14.Size = new System.Drawing.Size(113, 60);
            this.simpleLabelItem14.Text = "- - -";
            this.simpleLabelItem14.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelCursorLatitude
            // 
            this.simpleLabelCursorLatitude.AllowHotTrack = false;
            this.simpleLabelCursorLatitude.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelCursorLatitude.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelCursorLatitude.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelCursorLatitude.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelCursorLatitude.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelCursorLatitude.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelCursorLatitude.Location = new System.Drawing.Point(565, 30);
            this.simpleLabelCursorLatitude.Name = "simpleLabelCursorLatitude";
            this.simpleLabelCursorLatitude.OptionsTableLayoutItem.ColumnIndex = 5;
            this.simpleLabelCursorLatitude.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleLabelCursorLatitude.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelCursorLatitude.Text = "- - -";
            this.simpleLabelCursorLatitude.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelCursorAngle
            // 
            this.simpleLabelCursorAngle.AllowHotTrack = false;
            this.simpleLabelCursorAngle.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelCursorAngle.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelCursorAngle.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelCursorAngle.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelCursorAngle.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelCursorAngle.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelCursorAngle.Location = new System.Drawing.Point(678, 30);
            this.simpleLabelCursorAngle.Name = "simpleLabelCursorAngle";
            this.simpleLabelCursorAngle.OptionsTableLayoutItem.ColumnIndex = 6;
            this.simpleLabelCursorAngle.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleLabelCursorAngle.Size = new System.Drawing.Size(114, 30);
            this.simpleLabelCursorAngle.Text = "- - -";
            this.simpleLabelCursorAngle.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelLongitude
            // 
            this.simpleLabelLongitude.AllowHotTrack = false;
            this.simpleLabelLongitude.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelLongitude.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelLongitude.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelLongitude.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelLongitude.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelLongitude.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelLongitude.Location = new System.Drawing.Point(0, 60);
            this.simpleLabelLongitude.Name = "simpleLabelLongitude";
            this.simpleLabelLongitude.OptionsTableLayoutItem.RowIndex = 2;
            this.simpleLabelLongitude.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelLongitude.Text = "- - -";
            this.simpleLabelLongitude.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelcursorLongitude
            // 
            this.simpleLabelcursorLongitude.AllowHotTrack = false;
            this.simpleLabelcursorLongitude.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelcursorLongitude.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelcursorLongitude.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelcursorLongitude.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelcursorLongitude.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelcursorLongitude.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelcursorLongitude.Location = new System.Drawing.Point(565, 60);
            this.simpleLabelcursorLongitude.Name = "simpleLabelcursorLongitude";
            this.simpleLabelcursorLongitude.OptionsTableLayoutItem.ColumnIndex = 5;
            this.simpleLabelcursorLongitude.OptionsTableLayoutItem.RowIndex = 2;
            this.simpleLabelcursorLongitude.Size = new System.Drawing.Size(113, 30);
            this.simpleLabelcursorLongitude.Text = "- - -";
            this.simpleLabelcursorLongitude.TextSize = new System.Drawing.Size(67, 14);
            // 
            // simpleLabelCursorRange
            // 
            this.simpleLabelCursorRange.AllowHotTrack = false;
            this.simpleLabelCursorRange.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.simpleLabelCursorRange.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.simpleLabelCursorRange.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelCursorRange.AppearanceItemCaption.Options.UseForeColor = true;
            this.simpleLabelCursorRange.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelCursorRange.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelCursorRange.Location = new System.Drawing.Point(678, 60);
            this.simpleLabelCursorRange.Name = "simpleLabelCursorRange";
            this.simpleLabelCursorRange.OptionsTableLayoutItem.ColumnIndex = 6;
            this.simpleLabelCursorRange.OptionsTableLayoutItem.RowIndex = 2;
            this.simpleLabelCursorRange.Size = new System.Drawing.Size(114, 30);
            this.simpleLabelCursorRange.Text = "- - -";
            this.simpleLabelCursorRange.TextSize = new System.Drawing.Size(67, 14);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem4,
            this.barButtonItem5,
            this.barHeaderItem1,
            this.barStaticItem3,
            this.barStaticItem4,
            this.barStaticItem5,
            this.barStaticItem6,
            this.barStaticItem7,
            this.barStaticItem10,
            this.barStaticItem1,
            this.barSubItem1,
            this.barStaticItem2,
            this.barStaticItem8,
            this.skinBarSubItem1,
            this.skinBarSubItem2,
            this.barHeaderItem2,
            this.barStaticItem9,
            this.barSubItem2,
            this.barHeaderItem3,
            this.barStaticItem11,
            this.barStaticItem12,
            this.barStaticItem13,
            this.barSubItem3,
            this.barStaticItem14,
            this.barStaticItem15,
            this.barStaticItem16,
            this.barStaticItem17,
            this.barStaticItem18,
            this.barStaticItem19,
            this.barStaticItem20,
            this.barStaticItem21});
            this.barManager1.MaxItemId = 41;
            this.barManager1.StatusBar = this.bar3;
            // 
            // bar3
            // 
            this.bar3.BarName = "Custom 2";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem20)});
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Custom 2";
            // 
            // barStaticItem20
            // 
            this.barStaticItem20.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.barStaticItem20.Caption = "barStaticItem20";
            this.barStaticItem20.Id = 39;
            this.barStaticItem20.Name = "barStaticItem20";
            this.barStaticItem20.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(1819, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 789);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(1819, 22);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 789);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1819, 0);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 789);
            // 
            // barButtonItem4
            // 
            this.barButtonItem4.AccessibleName = "pointsToolStripMenuItem";
            this.barButtonItem4.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.barButtonItem4.Caption = "Points";
            this.barButtonItem4.Hint = null;
            this.barButtonItem4.Id = 4;
            this.barButtonItem4.Name = "barButtonItem4";
            this.barButtonItem4.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barButtonItem5
            // 
            this.barButtonItem5.AccessibleName = "rectangleToolStripMenuItem";
            this.barButtonItem5.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.barButtonItem5.Caption = "Rectangle";
            this.barButtonItem5.Hint = null;
            this.barButtonItem5.Id = 5;
            this.barButtonItem5.Name = "barButtonItem5";
            this.barButtonItem5.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barHeaderItem1
            // 
            this.barHeaderItem1.Caption = "Create";
            this.barHeaderItem1.Id = 10;
            this.barHeaderItem1.Name = "barHeaderItem1";
            // 
            // barStaticItem3
            // 
            this.barStaticItem3.Caption = "WayPoint";
            this.barStaticItem3.Id = 11;
            this.barStaticItem3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem3.ImageOptions.Image")));
            this.barStaticItem3.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem3.ImageOptions.LargeImage")));
            this.barStaticItem3.Name = "barStaticItem3";
            this.barStaticItem3.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barStaticItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStaticItem3_ItemClick);
            // 
            // barStaticItem4
            // 
            this.barStaticItem4.Caption = "Route";
            this.barStaticItem4.Id = 12;
            this.barStaticItem4.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem4.ImageOptions.Image")));
            this.barStaticItem4.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem4.ImageOptions.LargeImage")));
            this.barStaticItem4.Name = "barStaticItem4";
            this.barStaticItem4.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barStaticItem4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStaticItem4_ItemClick);
            // 
            // barStaticItem5
            // 
            this.barStaticItem5.Caption = "Geofence";
            this.barStaticItem5.Id = 13;
            this.barStaticItem5.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem5.ImageOptions.Image")));
            this.barStaticItem5.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem5.ImageOptions.LargeImage")));
            this.barStaticItem5.Name = "barStaticItem5";
            this.barStaticItem5.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barStaticItem6
            // 
            this.barStaticItem6.Caption = "Query Chart";
            this.barStaticItem6.Id = 14;
            this.barStaticItem6.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem6.ImageOptions.Image")));
            this.barStaticItem6.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem6.ImageOptions.LargeImage")));
            this.barStaticItem6.Name = "barStaticItem6";
            this.barStaticItem6.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barStaticItem7
            // 
            this.barStaticItem7.Caption = "Set Anchor Position";
            this.barStaticItem7.Id = 15;
            this.barStaticItem7.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem7.ImageOptions.Image")));
            this.barStaticItem7.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem7.ImageOptions.LargeImage")));
            this.barStaticItem7.Name = "barStaticItem7";
            this.barStaticItem7.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barStaticItem7.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStaticItem7_ItemClick);
            // 
            // barStaticItem10
            // 
            this.barStaticItem10.Caption = "Set Home Position";
            this.barStaticItem10.Id = 18;
            this.barStaticItem10.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem10.ImageOptions.Image")));
            this.barStaticItem10.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem10.ImageOptions.LargeImage")));
            this.barStaticItem10.Name = "barStaticItem10";
            this.barStaticItem10.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barStaticItem10.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStaticItem10_ItemClick);
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "Geofence";
            this.barStaticItem1.Id = 19;
            this.barStaticItem1.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.barStaticItem1.ImageOptions.AllowStubGlyph = DevExpress.Utils.DefaultBoolean.True;
            this.barStaticItem1.ItemAppearance.Hovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.barStaticItem1.ItemAppearance.Hovered.Options.UseBackColor = true;
            this.barStaticItem1.ItemAppearance.Normal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.barStaticItem1.ItemAppearance.Normal.Options.UseBackColor = true;
            this.barStaticItem1.ItemInMenuAppearance.Hovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.barStaticItem1.ItemInMenuAppearance.Hovered.Options.UseBackColor = true;
            this.barStaticItem1.ItemInMenuAppearance.Normal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.barStaticItem1.ItemInMenuAppearance.Normal.Options.UseBackColor = true;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // barSubItem1
            // 
            this.barSubItem1.Caption = "Geofence";
            this.barSubItem1.Id = 20;
            this.barSubItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barSubItem1.ImageOptions.Image")));
            this.barSubItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barSubItem1.ImageOptions.LargeImage")));
            this.barSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem8)});
            this.barSubItem1.Name = "barSubItem1";
            // 
            // barStaticItem2
            // 
            this.barStaticItem2.Caption = "Points";
            this.barStaticItem2.Id = 21;
            this.barStaticItem2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem2.ImageOptions.Image")));
            this.barStaticItem2.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem2.ImageOptions.LargeImage")));
            this.barStaticItem2.Name = "barStaticItem2";
            this.barStaticItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barStaticItem8
            // 
            this.barStaticItem8.Caption = "Rectangle";
            this.barStaticItem8.Id = 22;
            this.barStaticItem8.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem8.ImageOptions.Image")));
            this.barStaticItem8.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem8.ImageOptions.LargeImage")));
            this.barStaticItem8.Name = "barStaticItem8";
            this.barStaticItem8.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // skinBarSubItem1
            // 
            this.skinBarSubItem1.Caption = "Change S";
            this.skinBarSubItem1.Id = 23;
            this.skinBarSubItem1.Name = "skinBarSubItem1";
            // 
            // skinBarSubItem2
            // 
            this.skinBarSubItem2.Caption = "Change Theme";
            this.skinBarSubItem2.Id = 24;
            this.skinBarSubItem2.Name = "skinBarSubItem2";
            // 
            // barHeaderItem2
            // 
            this.barHeaderItem2.Caption = "Route(s)";
            this.barHeaderItem2.Id = 25;
            this.barHeaderItem2.Name = "barHeaderItem2";
            // 
            // barStaticItem9
            // 
            this.barStaticItem9.Caption = "RouteName";
            this.barStaticItem9.Id = 26;
            this.barStaticItem9.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barStaticItem9.ImageOptions.Image")));
            this.barStaticItem9.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barStaticItem9.ImageOptions.LargeImage")));
            this.barStaticItem9.Name = "barStaticItem9";
            this.barStaticItem9.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barSubItem2
            // 
            this.barSubItem2.Caption = "barSubItem2";
            this.barSubItem2.Id = 27;
            this.barSubItem2.Name = "barSubItem2";
            // 
            // barHeaderItem3
            // 
            this.barHeaderItem3.Caption = "Create";
            this.barHeaderItem3.Id = 28;
            this.barHeaderItem3.Name = "barHeaderItem3";
            // 
            // barStaticItem11
            // 
            this.barStaticItem11.Caption = "WayPoint";
            this.barStaticItem11.Id = 29;
            this.barStaticItem11.Name = "barStaticItem11";
            // 
            // barStaticItem12
            // 
            this.barStaticItem12.Caption = "Route";
            this.barStaticItem12.Id = 30;
            this.barStaticItem12.Name = "barStaticItem12";
            // 
            // barStaticItem13
            // 
            this.barStaticItem13.Caption = "barStaticItem13";
            this.barStaticItem13.Id = 31;
            this.barStaticItem13.Name = "barStaticItem13";
            // 
            // barSubItem3
            // 
            this.barSubItem3.Caption = "Geofence";
            this.barSubItem3.Id = 32;
            this.barSubItem3.Name = "barSubItem3";
            // 
            // barStaticItem14
            // 
            this.barStaticItem14.Caption = "QueryChart";
            this.barStaticItem14.Id = 33;
            this.barStaticItem14.Name = "barStaticItem14";
            // 
            // barStaticItem15
            // 
            this.barStaticItem15.Caption = "Set Anchor Position";
            this.barStaticItem15.Id = 34;
            this.barStaticItem15.Name = "barStaticItem15";
            // 
            // barStaticItem16
            // 
            this.barStaticItem16.Caption = "barStaticItem16";
            this.barStaticItem16.Id = 35;
            this.barStaticItem16.Name = "barStaticItem16";
            // 
            // barStaticItem17
            // 
            this.barStaticItem17.Caption = "barStaticItem17";
            this.barStaticItem17.Id = 36;
            this.barStaticItem17.Name = "barStaticItem17";
            // 
            // barStaticItem18
            // 
            this.barStaticItem18.Caption = "barStaticItem18";
            this.barStaticItem18.Id = 37;
            this.barStaticItem18.Name = "barStaticItem18";
            // 
            // barStaticItem19
            // 
            this.barStaticItem19.Caption = "barStaticItem19";
            this.barStaticItem19.Id = 38;
            this.barStaticItem19.Name = "barStaticItem19";
            // 
            // barStaticItem21
            // 
            this.barStaticItem21.Caption = "Go To Anchor Position";
            this.barStaticItem21.Id = 40;
            this.barStaticItem21.ImageOptions.SvgImage = global::EncViewWithCSharp.Properties.Resources.employeequickaward;
            this.barStaticItem21.Name = "barStaticItem21";
            this.barStaticItem21.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barStaticItem21.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStaticItem21_ItemClick);
            // 
            // bar1
            // 
            this.bar1.BarName = "Custom 3";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.Text = "Custom 3";
            // 
            // bar2
            // 
            this.bar2.BarName = "Custom 3";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.Text = "Custom 3";
            // 
            // windowsUIButtonPanel3
            // 
            this.windowsUIButtonPanel3.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton(),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton()});
            this.windowsUIButtonPanel3.Location = new System.Drawing.Point(340, 305);
            this.windowsUIButtonPanel3.Name = "windowsUIButtonPanel3";
            this.windowsUIButtonPanel3.Size = new System.Drawing.Size(75, 23);
            this.windowsUIButtonPanel3.TabIndex = 19;
            this.windowsUIButtonPanel3.Text = "windowsUIButtonPanel3";
            // 
            // windowsUIButtonPanel4
            // 
            this.windowsUIButtonPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsUIButtonPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            windowsUIButtonImageOptions10.SvgImage = global::EncViewWithCSharp.Properties.Resources.moveup;
            windowsUIButtonImageOptions11.SvgImage = global::EncViewWithCSharp.Properties.Resources.bo_state;
            windowsUIButtonImageOptions12.SvgImage = global::EncViewWithCSharp.Properties.Resources.charttype_polarline;
            windowsUIButtonImageOptions13.SvgImage = global::EncViewWithCSharp.Properties.Resources.weather_partlycloudyday;
            windowsUIButtonImageOptions14.Image = global::EncViewWithCSharp.Properties.Resources.video_16x161;
            windowsUIButtonImageOptions15.SvgImage = global::EncViewWithCSharp.Properties.Resources.setselectedimagesstretchmode_uniformtofill;
            this.windowsUIButtonPanel4.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("N", true, windowsUIButtonImageOptions10, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("AIS", true, windowsUIButtonImageOptions11, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Radar", true, windowsUIButtonImageOptions12, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("D/N", true, windowsUIButtonImageOptions13, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Record", true, windowsUIButtonImageOptions14, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Snap", true, windowsUIButtonImageOptions15, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, null, -1, false)});
            this.windowsUIButtonPanel4.ForeColor = System.Drawing.Color.White;
            this.windowsUIButtonPanel4.Location = new System.Drawing.Point(1215, 708);
            this.windowsUIButtonPanel4.Name = "windowsUIButtonPanel4";
            this.windowsUIButtonPanel4.Size = new System.Drawing.Size(306, 64);
            this.windowsUIButtonPanel4.TabIndex = 19;
            this.windowsUIButtonPanel4.Text = "windowsUIButtonPanel4";
            this.windowsUIButtonPanel4.ButtonClick += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.windowsUIButtonPanel4_ButtonClick);
            this.windowsUIButtonPanel4.ButtonUnchecked += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.windowsUIButtonPanel4_ButtonUnchecked);
            this.windowsUIButtonPanel4.ButtonChecked += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.windowsUIButtonPanel4_ButtonChecked);
            // 
            // windowsUIButtonPanel2
            // 
            this.windowsUIButtonPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsUIButtonPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            windowsUIButtonImageOptions9.Image = global::EncViewWithCSharp.Properties.Resources.ide_16x16;
            this.windowsUIButtonPanel2.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Tools", true, windowsUIButtonImageOptions9, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton()});
            this.windowsUIButtonPanel2.ForeColor = System.Drawing.Color.Transparent;
            this.windowsUIButtonPanel2.Location = new System.Drawing.Point(1527, 707);
            this.windowsUIButtonPanel2.Name = "windowsUIButtonPanel2";
            this.windowsUIButtonPanel2.Size = new System.Drawing.Size(75, 65);
            this.windowsUIButtonPanel2.TabIndex = 20;
            this.windowsUIButtonPanel2.Text = "windowsUIButtonPanel2";
            // 
            // simpleLabelItem10
            // 
            this.simpleLabelItem10.AllowHotTrack = false;
            this.simpleLabelItem10.Location = new System.Drawing.Point(120, 25);
            this.simpleLabelItem10.Name = "simpleLabelItem10";
            this.simpleLabelItem10.OptionsTableLayoutItem.ColumnIndex = 1;
            this.simpleLabelItem10.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleLabelItem10.Size = new System.Drawing.Size(120, 26);
            this.simpleLabelItem10.TextSize = new System.Drawing.Size(39, 13);
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.Location = new System.Drawing.Point(798, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.OptionsTableLayoutItem.ColumnIndex = 6;
            this.layoutControlGroup4.Size = new System.Drawing.Size(133, 70);
            // 
            // simpleSeparator1
            // 
            this.simpleSeparator1.AllowHotTrack = false;
            this.simpleSeparator1.Location = new System.Drawing.Point(120, 25);
            this.simpleSeparator1.Name = "simpleSeparator1";
            this.simpleSeparator1.OptionsTableLayoutItem.ColumnIndex = 1;
            this.simpleSeparator1.OptionsTableLayoutItem.RowIndex = 1;
            this.simpleSeparator1.Size = new System.Drawing.Size(120, 26);
            // 
            // groupControl1
            // 
            this.groupControl1.Appearance.BackColor = System.Drawing.Color.Gray;
            this.groupControl1.Appearance.Options.UseBackColor = true;
            this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupControl1.AppearanceCaption.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.ControlText;
            this.groupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.groupControl1.AppearanceCaption.Options.UseFont = true;
            this.groupControl1.AppearanceCaption.Options.UseForeColor = true;
            this.groupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.groupControl1.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.groupControl1.Controls.Add(this.navBarControl1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupControl1.GroupStyle = DevExpress.Utils.GroupStyle.Card;
            this.groupControl1.Location = new System.Drawing.Point(1619, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(200, 789);
            this.groupControl1.TabIndex = 25;
            this.groupControl1.Text = "VESSEL INFORMATION";
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.navBarGroup1;
            this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navBarControl1.ForeColor = System.Drawing.Color.Transparent;
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navBarGroup1,
            this.navBarGroup7,
            this.navBarGroup3,
            this.navBarGroup4,
            this.navBarGroup5,
            this.navBarGroup6,
            this.navBarGroup2});
            this.navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.navBarItem1});
            this.navBarControl1.Location = new System.Drawing.Point(2, 23);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 196;
            this.navBarControl1.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.NavigationPane;
            this.navBarControl1.Size = new System.Drawing.Size(196, 764);
            this.navBarControl1.TabIndex = 4;
            this.navBarControl1.Text = "navBarControl1";
            this.navBarControl1.View = new DevExpress.XtraNavBar.ViewInfo.SkinNavigationPaneViewInfoRegistrator();
            // 
            // navBarGroup1
            // 
            this.navBarGroup1.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navBarGroup1.Appearance.Options.UseFont = true;
            this.navBarGroup1.Appearance.Options.UseTextOptions = true;
            this.navBarGroup1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.navBarGroup1.Caption = "DashBoard";
            this.navBarGroup1.Expanded = true;
            this.navBarGroup1.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] {
            new DevExpress.XtraNavBar.NavBarItemLink(this.navBarItem1)});
            this.navBarGroup1.Name = "navBarGroup1";
            // 
            // navBarItem1
            // 
            this.navBarItem1.Caption = "Connect";
            this.navBarItem1.ImageOptions.SmallImage = global::EncViewWithCSharp.Properties.Resources.On_64;
            this.navBarItem1.Name = "navBarItem1";
            // 
            // navBarGroup7
            // 
            this.navBarGroup7.Caption = "Navigation";
            this.navBarGroup7.Name = "navBarGroup7";
            // 
            // navBarGroup3
            // 
            this.navBarGroup3.Caption = "Helmsman";
            this.navBarGroup3.Name = "navBarGroup3";
            // 
            // navBarGroup4
            // 
            this.navBarGroup4.Caption = "Switch Control";
            this.navBarGroup4.Name = "navBarGroup4";
            // 
            // navBarGroup5
            // 
            this.navBarGroup5.Caption = "Source Selection";
            this.navBarGroup5.Name = "navBarGroup5";
            // 
            // navBarGroup6
            // 
            this.navBarGroup6.Caption = "About";
            this.navBarGroup6.Name = "navBarGroup6";
            // 
            // navBarGroup2
            // 
            this.navBarGroup2.Caption = "navBarGroup2";
            this.navBarGroup2.Name = "navBarGroup2";
            // 
            // groupControl4
            // 
            this.groupControl4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl4.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupControl4.AppearanceCaption.Options.UseFont = true;
            this.groupControl4.AppearanceCaption.Options.UseTextOptions = true;
            this.groupControl4.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.groupControl4.Controls.Add(this.layoutControl6);
            this.groupControl4.Location = new System.Drawing.Point(1624, 256);
            this.groupControl4.Name = "groupControl4";
            this.groupControl4.Size = new System.Drawing.Size(190, 181);
            this.groupControl4.TabIndex = 2;
            this.groupControl4.Text = "WayPoints Of Routes";
            // 
            // layoutControl6
            // 
            this.layoutControl6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl6.Location = new System.Drawing.Point(2, 23);
            this.layoutControl6.Name = "layoutControl6";
            this.layoutControl6.Root = this.layoutControlGroup6;
            this.layoutControl6.Size = new System.Drawing.Size(186, 156);
            this.layoutControl6.TabIndex = 0;
            this.layoutControl6.Text = "layoutControl6";
            // 
            // layoutControlGroup6
            // 
            this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup6.GroupBordersVisible = false;
            this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.simpleLabelItem8});
            this.layoutControlGroup6.Name = "layoutControlGroup6";
            this.layoutControlGroup6.Size = new System.Drawing.Size(186, 156);
            this.layoutControlGroup6.TextVisible = false;
            // 
            // simpleLabelItem8
            // 
            this.simpleLabelItem8.AllowHotTrack = false;
            this.simpleLabelItem8.Location = new System.Drawing.Point(0, 0);
            this.simpleLabelItem8.Name = "simpleLabelItem8";
            this.simpleLabelItem8.Size = new System.Drawing.Size(166, 136);
            this.simpleLabelItem8.Text = "...";
            this.simpleLabelItem8.TextSize = new System.Drawing.Size(9, 13);
            // 
            // windowsUIButtonPanel5
            // 
            this.windowsUIButtonPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsUIButtonPanel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            windowsUIButtonImageOptions8.Image = global::EncViewWithCSharp.Properties.Resources.boorder_16x16;
            this.windowsUIButtonPanel5.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Menu", true, windowsUIButtonImageOptions8, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton()});
            this.windowsUIButtonPanel5.ForeColor = System.Drawing.Color.White;
            this.windowsUIButtonPanel5.Location = new System.Drawing.Point(1527, 22);
            this.windowsUIButtonPanel5.Name = "windowsUIButtonPanel5";
            this.windowsUIButtonPanel5.Size = new System.Drawing.Size(75, 64);
            this.windowsUIButtonPanel5.TabIndex = 26;
            this.windowsUIButtonPanel5.Text = "windowsUIButtonPanel5";
            // 
            // popupMenu2
            // 
            this.popupMenu2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barHeaderItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem3),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem4),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem6),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem7),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem10),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem21),
            new DevExpress.XtraBars.LinkPersistInfo(this.skinBarSubItem2)});
            this.popupMenu2.Manager = this.barManager1;
            this.popupMenu2.Name = "popupMenu2";
            // 
            // windowsUIButtonPanel7
            // 
            this.windowsUIButtonPanel7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsUIButtonPanel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            windowsUIButtonImageOptions7.Image = global::EncViewWithCSharp.Properties.Resources.zoomin_16x16;
            this.windowsUIButtonPanel7.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Zoom In", true, windowsUIButtonImageOptions7, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton()});
            this.windowsUIButtonPanel7.ForeColor = System.Drawing.Color.Transparent;
            this.windowsUIButtonPanel7.Location = new System.Drawing.Point(1527, 636);
            this.windowsUIButtonPanel7.Name = "windowsUIButtonPanel7";
            this.windowsUIButtonPanel7.Size = new System.Drawing.Size(75, 66);
            this.windowsUIButtonPanel7.TabIndex = 41;
            this.windowsUIButtonPanel7.Text = "windowsUIButtonPanel7";
            this.windowsUIButtonPanel7.ButtonClick += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.windowsUIButtonPanel7_ButtonClick);
            // 
            // windowsUIButtonPanel8
            // 
            this.windowsUIButtonPanel8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsUIButtonPanel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            windowsUIButtonImageOptions6.Image = global::EncViewWithCSharp.Properties.Resources.zoomout_16x161;
            this.windowsUIButtonPanel8.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Zoom out", true, windowsUIButtonImageOptions6, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton()});
            this.windowsUIButtonPanel8.ForeColor = System.Drawing.Color.Transparent;
            this.windowsUIButtonPanel8.Location = new System.Drawing.Point(1527, 566);
            this.windowsUIButtonPanel8.Name = "windowsUIButtonPanel8";
            this.windowsUIButtonPanel8.Size = new System.Drawing.Size(75, 65);
            this.windowsUIButtonPanel8.TabIndex = 42;
            this.windowsUIButtonPanel8.Text = "windowsUIButtonPanel8";
            this.windowsUIButtonPanel8.ButtonClick += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.windowsUIButtonPanel8_ButtonClick);
            // 
            // groupControl3
            // 
            this.groupControl3.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupControl3.Appearance.Options.UseBackColor = true;
            this.groupControl3.AppearanceCaption.ForeColor = System.Drawing.Color.White;
            this.groupControl3.AppearanceCaption.Options.UseFont = true;
            this.groupControl3.AppearanceCaption.Options.UseForeColor = true;
            this.groupControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.groupControl3.Controls.Add(this.layoutControl1);
            this.groupControl3.Location = new System.Drawing.Point(12, 11);
            this.groupControl3.Name = "groupControl3";
            this.groupControl3.ScrollBarSmallChange = 1;
            this.groupControl3.Size = new System.Drawing.Size(253, 131);
            this.groupControl3.TabIndex = 47;
            this.groupControl3.Text = "MessageCenter";
            this.groupControl3.Visible = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(253, 131);
            this.layoutControl1.TabIndex = 48;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.messagelabel1,
            this.messagelabel2});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(253, 131);
            this.Root.TextVisible = false;
            // 
            // messagelabel1
            // 
            this.messagelabel1.AllowHotTrack = false;
            this.messagelabel1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.messagelabel1.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.messagelabel1.AppearanceItemCaption.Options.UseFont = true;
            this.messagelabel1.AppearanceItemCaption.Options.UseForeColor = true;
            this.messagelabel1.Location = new System.Drawing.Point(0, 0);
            this.messagelabel1.MaxSize = new System.Drawing.Size(233, 43);
            this.messagelabel1.MinSize = new System.Drawing.Size(233, 43);
            this.messagelabel1.Name = "messagelabel1";
            this.messagelabel1.Size = new System.Drawing.Size(233, 43);
            this.messagelabel1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.messagelabel1.TextSize = new System.Drawing.Size(115, 13);
            // 
            // messagelabel2
            // 
            this.messagelabel2.AllowHotTrack = false;
            this.messagelabel2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.messagelabel2.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.messagelabel2.AppearanceItemCaption.Options.UseFont = true;
            this.messagelabel2.AppearanceItemCaption.Options.UseForeColor = true;
            this.messagelabel2.Location = new System.Drawing.Point(0, 43);
            this.messagelabel2.MaxSize = new System.Drawing.Size(233, 47);
            this.messagelabel2.MinSize = new System.Drawing.Size(233, 47);
            this.messagelabel2.Name = "messagelabel2";
            this.messagelabel2.Size = new System.Drawing.Size(233, 68);
            this.messagelabel2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.messagelabel2.TextSize = new System.Drawing.Size(115, 13);
            // 
            // popupMenu1
            // 
            this.popupMenu1.Manager = this.barManager1;
            this.popupMenu1.Name = "popupMenu1";
            // 
            // labelControl1
            // 
            this.labelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(1294, 11);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(15, 14);
            this.labelControl1.TabIndex = 52;
            this.labelControl1.Text = "---";
            // 
            // tabNavigationPage7
            // 
            this.tabNavigationPage7.Caption = "Geofences";
            this.tabNavigationPage7.Controls.Add(this.layoutControl7);
            this.tabNavigationPage7.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("tabNavigationPage7.ImageOptions.Image")));
            this.tabNavigationPage7.Name = "tabNavigationPage7";
            this.tabNavigationPage7.Size = new System.Drawing.Size(310, 470);
            // 
            // layoutControl7
            // 
            this.layoutControl7.Controls.Add(this.gridControl3);
            this.layoutControl7.Controls.Add(this.textEdit1);
            this.layoutControl7.Controls.Add(this.memoEdit1);
            this.layoutControl7.Controls.Add(this.simpleButton1);
            this.layoutControl7.Controls.Add(this.simpleButton2);
            this.layoutControl7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl7.Location = new System.Drawing.Point(0, 0);
            this.layoutControl7.Name = "layoutControl7";
            this.layoutControl7.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(370, 503, 650, 400);
            this.layoutControl7.Root = this.layoutControlGroup7;
            this.layoutControl7.Size = new System.Drawing.Size(310, 470);
            this.layoutControl7.TabIndex = 0;
            this.layoutControl7.Text = "layoutControl7";
            // 
            // gridControl3
            // 
            this.gridControl3.Location = new System.Drawing.Point(12, 173);
            this.gridControl3.MainView = this.cardView3;
            this.gridControl3.MenuManager = this.barManager1;
            this.gridControl3.Name = "gridControl3";
            this.gridControl3.Size = new System.Drawing.Size(286, 285);
            this.gridControl3.TabIndex = 8;
            this.gridControl3.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.cardView3});
            // 
            // cardView3
            // 
            this.cardView3.CardCaptionFormat = "GeoFence {1}";
            this.cardView3.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11});
            this.cardView3.GridControl = this.gridControl3;
            this.cardView3.Name = "cardView3";
            this.cardView3.OptionsFind.AlwaysVisible = true;
            this.cardView3.OptionsSelection.MultiSelect = true;
            this.cardView3.OptionsView.ShowQuickCustomizeButton = false;
            this.cardView3.OptionsView.ShowViewCaption = true;
            this.cardView3.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Auto;
            this.cardView3.ViewCaption = "GeoFence Manager";
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "GeoFence Name";
            this.gridColumn9.FieldName = "GeoFenceName";
            this.gridColumn9.Name = "gridColumn9";
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Notes";
            this.gridColumn10.FieldName = "Notes";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 0;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "GeoFence Points";
            this.gridColumn11.FieldName = "GeoFencePoints";
            this.gridColumn11.Name = "gridColumn11";
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(77, 12);
            this.textEdit1.MenuManager = this.barManager1;
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(221, 20);
            this.textEdit1.StyleController = this.layoutControl7;
            this.textEdit1.TabIndex = 4;
            // 
            // memoEdit1
            // 
            this.memoEdit1.Location = new System.Drawing.Point(77, 36);
            this.memoEdit1.MenuManager = this.barManager1;
            this.memoEdit1.Name = "memoEdit1";
            this.memoEdit1.Size = new System.Drawing.Size(221, 105);
            this.memoEdit1.StyleController = this.layoutControl7;
            this.memoEdit1.TabIndex = 5;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(12, 145);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(137, 24);
            this.simpleButton1.StyleController = this.layoutControl7;
            this.simpleButton1.TabIndex = 6;
            this.simpleButton1.Text = "Create";
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(153, 145);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(145, 24);
            this.simpleButton2.StyleController = this.layoutControl7;
            this.simpleButton2.TabIndex = 7;
            this.simpleButton2.Text = "Cancel";
            // 
            // layoutControlGroup7
            // 
            this.layoutControlGroup7.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup7.GroupBordersVisible = false;
            this.layoutControlGroup7.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem18,
            this.layoutControlItem19,
            this.layoutControlItem20,
            this.layoutControlItem21,
            this.layoutControlItem22});
            this.layoutControlGroup7.Name = "Root";
            this.layoutControlGroup7.Size = new System.Drawing.Size(310, 470);
            this.layoutControlGroup7.TextVisible = false;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.textEdit1;
            this.layoutControlItem18.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem18.MaxSize = new System.Drawing.Size(290, 24);
            this.layoutControlItem18.MinSize = new System.Drawing.Size(290, 24);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(290, 24);
            this.layoutControlItem18.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem18.Text = "Name";
            this.layoutControlItem18.TextSize = new System.Drawing.Size(53, 13);
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.memoEdit1;
            this.layoutControlItem19.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem19.MaxSize = new System.Drawing.Size(290, 109);
            this.layoutControlItem19.MinSize = new System.Drawing.Size(290, 109);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Size = new System.Drawing.Size(290, 109);
            this.layoutControlItem19.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem19.Text = "Description";
            this.layoutControlItem19.TextSize = new System.Drawing.Size(53, 13);
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.Control = this.simpleButton1;
            this.layoutControlItem20.Location = new System.Drawing.Point(0, 133);
            this.layoutControlItem20.MaxSize = new System.Drawing.Size(141, 28);
            this.layoutControlItem20.MinSize = new System.Drawing.Size(141, 28);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Size = new System.Drawing.Size(141, 28);
            this.layoutControlItem20.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem20.TextVisible = false;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.Control = this.simpleButton2;
            this.layoutControlItem21.Location = new System.Drawing.Point(141, 133);
            this.layoutControlItem21.MaxSize = new System.Drawing.Size(149, 28);
            this.layoutControlItem21.MinSize = new System.Drawing.Size(149, 28);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.Size = new System.Drawing.Size(149, 28);
            this.layoutControlItem21.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem21.TextVisible = false;
            // 
            // layoutControlItem22
            // 
            this.layoutControlItem22.Control = this.gridControl3;
            this.layoutControlItem22.Location = new System.Drawing.Point(0, 161);
            this.layoutControlItem22.Name = "layoutControlItem22";
            this.layoutControlItem22.Size = new System.Drawing.Size(290, 289);
            this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem22.TextVisible = false;
            // 
            // tabNavigationPage6
            // 
            this.tabNavigationPage6.Caption = "Routes";
            this.tabNavigationPage6.Controls.Add(this.layoutControl5);
            this.tabNavigationPage6.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("tabNavigationPage6.ImageOptions.Image")));
            this.tabNavigationPage6.Name = "tabNavigationPage6";
            this.tabNavigationPage6.Size = new System.Drawing.Size(310, 470);
            // 
            // layoutControl5
            // 
            this.layoutControl5.Controls.Add(this.gridControl2);
            this.layoutControl5.Controls.Add(this.colorPickedit_routeColour);
            this.layoutControl5.Controls.Add(this.txtedit_routeName);
            this.layoutControl5.Controls.Add(this.memoedit_routeDescription);
            this.layoutControl5.Controls.Add(this.simpleButton3);
            this.layoutControl5.Controls.Add(this.spintxtedit_usvspeed);
            this.layoutControl5.Controls.Add(this.txtedit_Totaldistance);
            this.layoutControl5.Controls.Add(this.txtedit_Timetake);
            this.layoutControl5.Controls.Add(this.routecreate_button);
            this.layoutControl5.Controls.Add(this.routrcancel_button);
            this.layoutControl5.Controls.Add(this.simpleButton4);
            this.layoutControl5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl5.Location = new System.Drawing.Point(0, 0);
            this.layoutControl5.Name = "layoutControl5";
            this.layoutControl5.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1008, 236, 650, 400);
            this.layoutControl5.Root = this.layoutControlGroup5;
            this.layoutControl5.Size = new System.Drawing.Size(310, 470);
            this.layoutControl5.TabIndex = 0;
            this.layoutControl5.Text = "layoutControl5";
            // 
            // gridControl2
            // 
            this.gridControl2.Location = new System.Drawing.Point(12, 343);
            this.gridControl2.MainView = this.cardView2;
            this.gridControl2.MenuManager = this.barManager1;
            this.gridControl2.Name = "gridControl2";
            this.gridControl2.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1,
            this.repositoryItemCheckEdit2});
            this.gridControl2.Size = new System.Drawing.Size(289, 115);
            this.gridControl2.TabIndex = 13;
            this.gridControl2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.cardView2,
            this.gridView1});
            // 
            // cardView2
            // 
            this.cardView2.CardCaptionFormat = "Route {1}";
            this.cardView2.CardWidth = 250;
            this.cardView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8});
            this.cardView2.GridControl = this.gridControl2;
            this.cardView2.Name = "cardView2";
            this.cardView2.OptionsBehavior.Editable = false;
            this.cardView2.OptionsBehavior.ReadOnly = true;
            this.cardView2.OptionsFind.AlwaysVisible = true;
            this.cardView2.OptionsPrint.UsePrintStyles = false;
            this.cardView2.OptionsSelection.MultiSelect = true;
            this.cardView2.OptionsView.ShowQuickCustomizeButton = false;
            this.cardView2.OptionsView.ShowViewCaption = true;
            this.cardView2.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Auto;
            this.cardView2.ViewCaption = "Route Manager";
            this.cardView2.CustomCardCaptionImage += new DevExpress.XtraGrid.Views.Card.CardCaptionImageEventHandler(this.cardView2_CustomCardCaptionImage);
            this.cardView2.CustomDrawCardField += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.cardView2_CustomDrawCardField);
            this.cardView2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cardView2_KeyUp);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "RouteName";
            this.gridColumn1.FieldName = "RouteName";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsColumn.ReadOnly = true;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Description";
            this.gridColumn2.FieldName = "Description";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "USV Speed";
            this.gridColumn3.FieldName = "USV Speed";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.OptionsColumn.ReadOnly = true;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Total Distance";
            this.gridColumn4.FieldName = "Total Distance";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.OptionsColumn.ReadOnly = true;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Cover Time";
            this.gridColumn5.FieldName = "Cover Time";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.OptionsColumn.ReadOnly = true;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Show Route";
            this.gridColumn6.ColumnEdit = this.repositoryItemCheckEdit2;
            this.gridColumn6.FieldName = "Show Route";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            this.repositoryItemCheckEdit2.CheckedChanged += new System.EventHandler(this.repositoryItemCheckEdit2_CheckedChanged);
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "waypoins of Route";
            this.gridColumn7.FieldName = "waypoins of Route";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.OptionsColumn.ReadOnly = true;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Colour";
            this.gridColumn8.FieldName = "Colour";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.OptionsColumn.ReadOnly = true;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 5;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.CheckBox;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.gridControl2;
            this.gridView1.Name = "gridView1";
            // 
            // colorPickedit_routeColour
            // 
            this.colorPickedit_routeColour.EditValue = System.Drawing.Color.Red;
            this.colorPickedit_routeColour.Location = new System.Drawing.Point(93, 57);
            this.colorPickedit_routeColour.MenuManager = this.barManager1;
            this.colorPickedit_routeColour.Name = "colorPickedit_routeColour";
            this.colorPickedit_routeColour.Properties.AutomaticColor = System.Drawing.Color.Black;
            this.colorPickedit_routeColour.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.colorPickedit_routeColour.Properties.NullColor = System.Drawing.Color.Empty;
            this.colorPickedit_routeColour.Size = new System.Drawing.Size(208, 20);
            this.colorPickedit_routeColour.StyleController = this.layoutControl5;
            this.colorPickedit_routeColour.TabIndex = 6;
            // 
            // txtedit_routeName
            // 
            this.txtedit_routeName.Location = new System.Drawing.Point(93, 12);
            this.txtedit_routeName.MenuManager = this.barManager1;
            this.txtedit_routeName.Name = "txtedit_routeName";
            this.txtedit_routeName.Size = new System.Drawing.Size(208, 20);
            this.txtedit_routeName.StyleController = this.layoutControl5;
            this.txtedit_routeName.TabIndex = 4;
            // 
            // memoedit_routeDescription
            // 
            this.memoedit_routeDescription.Location = new System.Drawing.Point(93, 36);
            this.memoedit_routeDescription.MenuManager = this.barManager1;
            this.memoedit_routeDescription.Name = "memoedit_routeDescription";
            this.memoedit_routeDescription.Size = new System.Drawing.Size(208, 17);
            this.memoedit_routeDescription.StyleController = this.layoutControl5;
            this.memoedit_routeDescription.TabIndex = 5;
            // 
            // simpleButton3
            // 
            this.simpleButton3.Location = new System.Drawing.Point(12, 317);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Size = new System.Drawing.Size(289, 22);
            this.simpleButton3.StyleController = this.layoutControl5;
            this.simpleButton3.TabIndex = 12;
            this.simpleButton3.Text = "Import RTZ file";
            // 
            // spintxtedit_usvspeed
            // 
            this.spintxtedit_usvspeed.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spintxtedit_usvspeed.Location = new System.Drawing.Point(93, 152);
            this.spintxtedit_usvspeed.MenuManager = this.barManager1;
            this.spintxtedit_usvspeed.Name = "spintxtedit_usvspeed";
            this.spintxtedit_usvspeed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spintxtedit_usvspeed.Size = new System.Drawing.Size(135, 20);
            this.spintxtedit_usvspeed.StyleController = this.layoutControl5;
            this.spintxtedit_usvspeed.TabIndex = 14;
            // 
            // txtedit_Totaldistance
            // 
            this.txtedit_Totaldistance.Location = new System.Drawing.Point(93, 176);
            this.txtedit_Totaldistance.MenuManager = this.barManager1;
            this.txtedit_Totaldistance.Name = "txtedit_Totaldistance";
            this.txtedit_Totaldistance.Size = new System.Drawing.Size(135, 20);
            this.txtedit_Totaldistance.StyleController = this.layoutControl5;
            this.txtedit_Totaldistance.TabIndex = 15;
            // 
            // txtedit_Timetake
            // 
            this.txtedit_Timetake.Location = new System.Drawing.Point(93, 200);
            this.txtedit_Timetake.MenuManager = this.barManager1;
            this.txtedit_Timetake.Name = "txtedit_Timetake";
            this.txtedit_Timetake.Size = new System.Drawing.Size(135, 20);
            this.txtedit_Timetake.StyleController = this.layoutControl5;
            this.txtedit_Timetake.TabIndex = 16;
            // 
            // routecreate_button
            // 
            this.routecreate_button.Location = new System.Drawing.Point(12, 224);
            this.routecreate_button.Name = "routecreate_button";
            this.routecreate_button.Size = new System.Drawing.Size(149, 22);
            this.routecreate_button.StyleController = this.layoutControl5;
            this.routecreate_button.TabIndex = 17;
            this.routecreate_button.Text = "Save";
            this.routecreate_button.Click += new System.EventHandler(this.routecreate_button_Click);
            // 
            // routrcancel_button
            // 
            this.routrcancel_button.Location = new System.Drawing.Point(165, 224);
            this.routrcancel_button.Name = "routrcancel_button";
            this.routrcancel_button.Size = new System.Drawing.Size(136, 22);
            this.routrcancel_button.StyleController = this.layoutControl5;
            this.routrcancel_button.TabIndex = 18;
            this.routrcancel_button.Text = "Cancel";
            this.routrcancel_button.Click += new System.EventHandler(this.routrcancel_button_Click);
            // 
            // simpleButton4
            // 
            this.simpleButton4.Location = new System.Drawing.Point(12, 250);
            this.simpleButton4.Name = "simpleButton4";
            this.simpleButton4.Size = new System.Drawing.Size(289, 22);
            this.simpleButton4.StyleController = this.layoutControl5;
            this.simpleButton4.TabIndex = 19;
            this.simpleButton4.Text = "Clear All Routes";
            this.simpleButton4.Click += new System.EventHandler(this.simpleButton4_Click);
            // 
            // layoutControlGroup5
            // 
            this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup5.GroupBordersVisible = false;
            this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.layoutControlItem11,
            this.simpleLabelItem19,
            this.simpleLabelItem20,
            this.layoutControlItem17,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.simpleLabelItem21,
            this.layoutControlItem12,
            this.simpleLabelItem22,
            this.layoutControlItem13,
            this.simpleLabelItem23,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.layoutControlItem16,
            this.emptySpaceItem1});
            this.layoutControlGroup5.Name = "Root";
            this.layoutControlGroup5.Size = new System.Drawing.Size(313, 470);
            this.layoutControlGroup5.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.txtedit_routeName;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem9.MaxSize = new System.Drawing.Size(293, 24);
            this.layoutControlItem9.MinSize = new System.Drawing.Size(293, 24);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(293, 24);
            this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem9.Text = "Name";
            this.layoutControlItem9.TextSize = new System.Drawing.Size(69, 13);
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.memoedit_routeDescription;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem10.MaxSize = new System.Drawing.Size(293, 21);
            this.layoutControlItem10.MinSize = new System.Drawing.Size(293, 21);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(293, 21);
            this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem10.Text = "Description";
            this.layoutControlItem10.TextSize = new System.Drawing.Size(69, 13);
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.colorPickedit_routeColour;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 45);
            this.layoutControlItem11.MaxSize = new System.Drawing.Size(293, 24);
            this.layoutControlItem11.MinSize = new System.Drawing.Size(293, 24);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(293, 24);
            this.layoutControlItem11.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem11.Text = "Colour";
            this.layoutControlItem11.TextSize = new System.Drawing.Size(69, 13);
            // 
            // simpleLabelItem19
            // 
            this.simpleLabelItem19.AllowHotTrack = false;
            this.simpleLabelItem19.AppearanceItemCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.simpleLabelItem19.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem19.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem19.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.simpleLabelItem19.Location = new System.Drawing.Point(0, 69);
            this.simpleLabelItem19.MaxSize = new System.Drawing.Size(280, 71);
            this.simpleLabelItem19.MinSize = new System.Drawing.Size(280, 71);
            this.simpleLabelItem19.Name = "simpleLabelItem19";
            this.simpleLabelItem19.Size = new System.Drawing.Size(293, 71);
            this.simpleLabelItem19.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.simpleLabelItem19.Text = "Note : Plan Speed is constant here just because we can calculate approax time tak" +
    "en to execute route.You can adjust the USV speed anytime";
            this.simpleLabelItem19.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.simpleLabelItem19.TextSize = new System.Drawing.Size(99, 13);
            // 
            // simpleLabelItem20
            // 
            this.simpleLabelItem20.AllowHotTrack = false;
            this.simpleLabelItem20.AppearanceItemCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.simpleLabelItem20.AppearanceItemCaption.Options.UseFont = true;
            this.simpleLabelItem20.AppearanceItemCaption.Options.UseTextOptions = true;
            this.simpleLabelItem20.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem20.AppearanceItemCaptionDisabled.Options.UseTextOptions = true;
            this.simpleLabelItem20.AppearanceItemCaptionDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.simpleLabelItem20.AppearanceItemCaptionDisabled.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.simpleLabelItem20.Location = new System.Drawing.Point(0, 288);
            this.simpleLabelItem20.Name = "simpleLabelItem20";
            this.simpleLabelItem20.Size = new System.Drawing.Size(293, 17);
            this.simpleLabelItem20.Text = "RTZ Format";
            this.simpleLabelItem20.TextSize = new System.Drawing.Size(69, 13);
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.simpleButton3;
            this.layoutControlItem17.Location = new System.Drawing.Point(0, 305);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(293, 26);
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl2;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 331);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(293, 119);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.spintxtedit_usvspeed;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 140);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem2.Text = "USV Speed";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(69, 13);
            // 
            // simpleLabelItem21
            // 
            this.simpleLabelItem21.AllowHotTrack = false;
            this.simpleLabelItem21.Location = new System.Drawing.Point(220, 140);
            this.simpleLabelItem21.Name = "simpleLabelItem21";
            this.simpleLabelItem21.Size = new System.Drawing.Size(73, 24);
            this.simpleLabelItem21.Text = "Kn";
            this.simpleLabelItem21.TextSize = new System.Drawing.Size(69, 13);
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.txtedit_Totaldistance;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 164);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem12.Text = "Total Distance";
            this.layoutControlItem12.TextSize = new System.Drawing.Size(69, 13);
            // 
            // simpleLabelItem22
            // 
            this.simpleLabelItem22.AllowHotTrack = false;
            this.simpleLabelItem22.Location = new System.Drawing.Point(220, 164);
            this.simpleLabelItem22.Name = "simpleLabelItem22";
            this.simpleLabelItem22.Size = new System.Drawing.Size(73, 24);
            this.simpleLabelItem22.Text = "nm";
            this.simpleLabelItem22.TextSize = new System.Drawing.Size(69, 13);
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.txtedit_Timetake;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 188);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem13.Text = "Cover Time";
            this.layoutControlItem13.TextSize = new System.Drawing.Size(69, 13);
            // 
            // simpleLabelItem23
            // 
            this.simpleLabelItem23.AllowHotTrack = false;
            this.simpleLabelItem23.Location = new System.Drawing.Point(220, 188);
            this.simpleLabelItem23.Name = "simpleLabelItem23";
            this.simpleLabelItem23.Size = new System.Drawing.Size(73, 24);
            this.simpleLabelItem23.Text = "H";
            this.simpleLabelItem23.TextSize = new System.Drawing.Size(69, 13);
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.routecreate_button;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 212);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(153, 26);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.routrcancel_button;
            this.layoutControlItem15.Location = new System.Drawing.Point(153, 212);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(140, 26);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.simpleButton4;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 238);
            this.layoutControlItem16.MaxSize = new System.Drawing.Size(293, 26);
            this.layoutControlItem16.MinSize = new System.Drawing.Size(293, 26);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(293, 26);
            this.layoutControlItem16.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem16.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 264);
            this.emptySpaceItem1.MaxSize = new System.Drawing.Size(293, 24);
            this.emptySpaceItem1.MinSize = new System.Drawing.Size(293, 24);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(293, 24);
            this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // tabNavigationPage5
            // 
            this.tabNavigationPage5.Caption = "Waypoints";
            this.tabNavigationPage5.Controls.Add(this.layoutControl2);
            this.tabNavigationPage5.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("tabNavigationPage5.ImageOptions.Image")));
            this.tabNavigationPage5.Name = "tabNavigationPage5";
            this.tabNavigationPage5.Size = new System.Drawing.Size(310, 470);
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.gridControl1);
            this.layoutControl2.Controls.Add(this.wayptxt_Name);
            this.layoutControl2.Controls.Add(this.wayptxt_Lat);
            this.layoutControl2.Controls.Add(this.wayptxt_Lon);
            this.layoutControl2.Controls.Add(this.waypointcreatebutton);
            this.layoutControl2.Controls.Add(this.waypointcancelbutton);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1015, 174, 650, 400);
            this.layoutControl2.Root = this.layoutControlGroup1;
            this.layoutControl2.Size = new System.Drawing.Size(310, 470);
            this.layoutControl2.TabIndex = 15;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(12, 115);
            this.gridControl1.MainView = this.cardView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(286, 343);
            this.gridControl1.TabIndex = 15;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.cardView1,
            this.gridView2});
            // 
            // cardView1
            // 
            this.cardView1.AppearancePrint.CardCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.cardView1.AppearancePrint.CardCaption.Options.UseFont = true;
            this.cardView1.CardCaptionFormat = "WayPoint {1}";
            this.cardView1.CardWidth = 250;
            this.cardView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn12,
            this.Latitude,
            this.Longitude});
            this.cardView1.GridControl = this.gridControl1;
            this.cardView1.Name = "cardView1";
            this.cardView1.OptionsBehavior.Editable = false;
            this.cardView1.OptionsBehavior.ReadOnly = true;
            this.cardView1.OptionsFind.AlwaysVisible = true;
            this.cardView1.OptionsSelection.MultiSelect = true;
            this.cardView1.OptionsView.ShowQuickCustomizeButton = false;
            this.cardView1.OptionsView.ShowViewCaption = true;
            this.cardView1.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Auto;
            this.cardView1.ViewCaption = "Waypoint Manager";
            this.cardView1.CustomCardCaptionImage += new DevExpress.XtraGrid.Views.Card.CardCaptionImageEventHandler(this.cardView1_CustomCardCaptionImage);
            this.cardView1.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.cardView1_SelectionChanged);
            this.cardView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cardView1_KeyUp);
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "WayPoint Name";
            this.gridColumn12.FieldName = "WayPointName";
            this.gridColumn12.Name = "gridColumn12";
            // 
            // Latitude
            // 
            this.Latitude.Caption = "Latitude";
            this.Latitude.FieldName = "Latitude";
            this.Latitude.Name = "Latitude";
            this.Latitude.Visible = true;
            this.Latitude.VisibleIndex = 0;
            // 
            // Longitude
            // 
            this.Longitude.Caption = "Longitude";
            this.Longitude.FieldName = "Longitude";
            this.Longitude.Name = "Longitude";
            this.Longitude.Visible = true;
            this.Longitude.VisibleIndex = 1;
            // 
            // gridView2
            // 
            this.gridView2.GridControl = this.gridControl1;
            this.gridView2.Name = "gridView2";
            // 
            // wayptxt_Name
            // 
            this.wayptxt_Name.Location = new System.Drawing.Point(71, 12);
            this.wayptxt_Name.Name = "wayptxt_Name";
            this.wayptxt_Name.Size = new System.Drawing.Size(227, 20);
            this.wayptxt_Name.StyleController = this.layoutControl2;
            this.wayptxt_Name.TabIndex = 4;
            // 
            // wayptxt_Lat
            // 
            this.wayptxt_Lat.Location = new System.Drawing.Point(71, 36);
            this.wayptxt_Lat.Name = "wayptxt_Lat";
            this.wayptxt_Lat.Size = new System.Drawing.Size(227, 20);
            this.wayptxt_Lat.StyleController = this.layoutControl2;
            this.wayptxt_Lat.TabIndex = 5;
            // 
            // wayptxt_Lon
            // 
            this.wayptxt_Lon.Location = new System.Drawing.Point(71, 60);
            this.wayptxt_Lon.Name = "wayptxt_Lon";
            this.wayptxt_Lon.Size = new System.Drawing.Size(227, 20);
            this.wayptxt_Lon.StyleController = this.layoutControl2;
            this.wayptxt_Lon.TabIndex = 6;
            // 
            // waypointcreatebutton
            // 
            this.waypointcreatebutton.Location = new System.Drawing.Point(12, 84);
            this.waypointcreatebutton.Name = "waypointcreatebutton";
            this.waypointcreatebutton.Size = new System.Drawing.Size(147, 27);
            this.waypointcreatebutton.StyleController = this.layoutControl2;
            this.waypointcreatebutton.TabIndex = 16;
            this.waypointcreatebutton.Text = "Create";
            this.waypointcreatebutton.Click += new System.EventHandler(this.waypointcreatebutton_Click);
            // 
            // waypointcancelbutton
            // 
            this.waypointcancelbutton.Location = new System.Drawing.Point(163, 84);
            this.waypointcancelbutton.Name = "waypointcancelbutton";
            this.waypointcancelbutton.Size = new System.Drawing.Size(135, 27);
            this.waypointcancelbutton.StyleController = this.layoutControl2;
            this.waypointcancelbutton.TabIndex = 17;
            this.waypointcancelbutton.Text = "Cancel";
            this.waypointcancelbutton.Click += new System.EventHandler(this.waypointcancelbutton_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem8,
            this.layoutControlItem6,
            this.layoutControlItem7});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(310, 470);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.wayptxt_Name;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(290, 24);
            this.layoutControlItem3.Text = "Name";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(47, 13);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.wayptxt_Lat;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(290, 24);
            this.layoutControlItem4.Text = "Latitude";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(47, 13);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.wayptxt_Lon;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(290, 24);
            this.layoutControlItem5.Text = "Longitude";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(47, 13);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.gridControl1;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 103);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(290, 347);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.waypointcreatebutton;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(151, 31);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(151, 31);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(151, 31);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.waypointcancelbutton;
            this.layoutControlItem7.Location = new System.Drawing.Point(151, 72);
            this.layoutControlItem7.MaxSize = new System.Drawing.Size(139, 31);
            this.layoutControlItem7.MinSize = new System.Drawing.Size(139, 31);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(139, 31);
            this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // tabPane3
            // 
            this.tabPane3.Controls.Add(this.tabNavigationPage5);
            this.tabPane3.Controls.Add(this.tabNavigationPage6);
            this.tabPane3.Controls.Add(this.tabNavigationPage7);
            this.tabPane3.Location = new System.Drawing.Point(12, 269);
            this.tabPane3.Name = "tabPane3";
            this.tabPane3.PageProperties.ShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
            this.tabPane3.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.tabNavigationPage5,
            this.tabNavigationPage6,
            this.tabNavigationPage7});
            this.tabPane3.RegularSize = new System.Drawing.Size(310, 503);
            this.tabPane3.SelectedPage = this.tabNavigationPage5;
            this.tabPane3.Size = new System.Drawing.Size(310, 503);
            this.tabPane3.TabIndex = 14;
            this.tabPane3.Text = "tabPane3";
            this.tabPane3.Visible = false;
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // groupControl5
            // 
            this.groupControl5.Controls.Add(this.layoutControl8);
            this.groupControl5.Location = new System.Drawing.Point(755, 271);
            this.groupControl5.Name = "groupControl5";
            this.groupControl5.Size = new System.Drawing.Size(271, 540);
            this.groupControl5.TabIndex = 62;
            this.groupControl5.Visible = false;
            // 
            // layoutControl8
            // 
            this.layoutControl8.Controls.Add(this.gridControl5);
            this.layoutControl8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl8.Location = new System.Drawing.Point(2, 23);
            this.layoutControl8.Name = "layoutControl8";
            this.layoutControl8.OptionsPrint.AppearanceGroupCaption.BackColor = System.Drawing.Color.LightGray;
            this.layoutControl8.OptionsPrint.AppearanceGroupCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.layoutControl8.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
            this.layoutControl8.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
            this.layoutControl8.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
            this.layoutControl8.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutControl8.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.layoutControl8.Root = this.layoutControlGroup8;
            this.layoutControl8.Size = new System.Drawing.Size(267, 515);
            this.layoutControl8.TabIndex = 0;
            this.layoutControl8.Text = "layoutControl8";
            // 
            // gridControl5
            // 
            this.gridControl5.Location = new System.Drawing.Point(12, 12);
            this.gridControl5.MainView = this.layoutView2;
            this.gridControl5.MenuManager = this.barManager1;
            this.gridControl5.Name = "gridControl5";
            this.gridControl5.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPictureEdit2});
            this.gridControl5.Size = new System.Drawing.Size(243, 491);
            this.gridControl5.TabIndex = 4;
            this.gridControl5.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.layoutView2});
            // 
            // layoutView2
            // 
            this.layoutView2.CardCaptionFormat = "Priority {3}";
            this.layoutView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.LayoutViewColumn[] {
            this.layoutViewColumn1,
            this.layoutViewColumn2,
            this.layoutViewColumn3,
            this.layoutViewColumn4,
            this.layoutViewColumn5,
            this.layoutViewColumn6,
            this.layoutViewColumn7,
            this.layoutViewColumn8,
            this.layoutViewColumn9,
            this.layoutViewColumn10,
            this.layoutViewColumn11,
            this.layoutViewColumn12});
            this.layoutView2.GridControl = this.gridControl5;
            this.layoutView2.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutViewField2,
            this.layoutViewField8});
            this.layoutView2.Name = "layoutView2";
            this.layoutView2.OptionsBehavior.Editable = false;
            this.layoutView2.OptionsBehavior.ReadOnly = true;
            this.layoutView2.OptionsBehavior.ScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Auto;
            this.layoutView2.OptionsCustomization.AllowFilter = false;
            this.layoutView2.OptionsCustomization.AllowSort = false;
            this.layoutView2.OptionsView.AllowBorderColorBlending = true;
            this.layoutView2.OptionsView.ContentAlignment = System.Drawing.ContentAlignment.TopCenter;
            this.layoutView2.OptionsView.DefaultColumnCount = 1;
            this.layoutView2.OptionsView.ShowHeaderPanel = false;
            this.layoutView2.OptionsView.ShowViewCaption = true;
            this.layoutView2.OptionsView.ViewMode = DevExpress.XtraGrid.Views.Layout.LayoutViewMode.Column;
            this.layoutView2.TemplateCard = this.layoutViewCard2;
            this.layoutView2.ViewCaption = "Priority";
            // 
            // layoutViewColumn1
            // 
            this.layoutViewColumn1.Caption = "Id";
            this.layoutViewColumn1.FieldName = "TargetNo";
            this.layoutViewColumn1.LayoutViewField = this.layoutViewField1;
            this.layoutViewColumn1.Name = "layoutViewColumn1";
            // 
            // layoutViewField1
            // 
            this.layoutViewField1.EditorPreferredWidth = 10;
            this.layoutViewField1.Location = new System.Drawing.Point(0, 0);
            this.layoutViewField1.Name = "layoutViewField1";
            this.layoutViewField1.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField1.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn2
            // 
            this.layoutViewColumn2.Caption = "Name";
            this.layoutViewColumn2.FieldName = "TargetName";
            this.layoutViewColumn2.LayoutViewField = this.layoutViewField2;
            this.layoutViewColumn2.Name = "layoutViewColumn2";
            // 
            // layoutViewField2
            // 
            this.layoutViewField2.EditorPreferredWidth = 10;
            this.layoutViewField2.Location = new System.Drawing.Point(0, 20);
            this.layoutViewField2.Name = "layoutViewField2";
            this.layoutViewField2.Size = new System.Drawing.Size(127, 20);
            this.layoutViewField2.TextSize = new System.Drawing.Size(95, 20);
            // 
            // layoutViewColumn3
            // 
            this.layoutViewColumn3.Caption = "Course";
            this.layoutViewColumn3.FieldName = "TargetCourse";
            this.layoutViewColumn3.LayoutViewField = this.layoutViewField3;
            this.layoutViewColumn3.Name = "layoutViewColumn3";
            // 
            // layoutViewField3
            // 
            this.layoutViewField3.EditorPreferredWidth = 10;
            this.layoutViewField3.Location = new System.Drawing.Point(0, 20);
            this.layoutViewField3.Name = "layoutViewField3";
            this.layoutViewField3.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField3.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn4
            // 
            this.layoutViewColumn4.Caption = "Speed";
            this.layoutViewColumn4.FieldName = "TargetSpeed";
            this.layoutViewColumn4.LayoutViewField = this.layoutViewField4;
            this.layoutViewColumn4.Name = "layoutViewColumn4";
            // 
            // layoutViewField4
            // 
            this.layoutViewField4.EditorPreferredWidth = 10;
            this.layoutViewField4.Location = new System.Drawing.Point(0, 40);
            this.layoutViewField4.Name = "layoutViewField4";
            this.layoutViewField4.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField4.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn5
            // 
            this.layoutViewColumn5.Caption = "TCPA";
            this.layoutViewColumn5.FieldName = "TargetTcpa";
            this.layoutViewColumn5.LayoutViewField = this.layoutViewField5;
            this.layoutViewColumn5.Name = "layoutViewColumn5";
            // 
            // layoutViewField5
            // 
            this.layoutViewField5.EditorPreferredWidth = 10;
            this.layoutViewField5.Location = new System.Drawing.Point(0, 60);
            this.layoutViewField5.Name = "layoutViewField5";
            this.layoutViewField5.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField5.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn6
            // 
            this.layoutViewColumn6.Caption = "CPA";
            this.layoutViewColumn6.FieldName = "TargetCpa";
            this.layoutViewColumn6.LayoutViewField = this.layoutViewField6;
            this.layoutViewColumn6.Name = "layoutViewColumn6";
            // 
            // layoutViewField6
            // 
            this.layoutViewField6.EditorPreferredWidth = 10;
            this.layoutViewField6.Location = new System.Drawing.Point(0, 80);
            this.layoutViewField6.Name = "layoutViewField6";
            this.layoutViewField6.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField6.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn7
            // 
            this.layoutViewColumn7.Caption = "Symbol";
            this.layoutViewColumn7.ColumnEdit = this.repositoryItemPictureEdit2;
            this.layoutViewColumn7.FieldName = "TargetImage";
            this.layoutViewColumn7.LayoutViewField = this.layoutViewField7;
            this.layoutViewColumn7.Name = "layoutViewColumn7";
            // 
            // repositoryItemPictureEdit2
            // 
            this.repositoryItemPictureEdit2.Name = "repositoryItemPictureEdit2";
            // 
            // layoutViewField7
            // 
            this.layoutViewField7.EditorPreferredWidth = 10;
            this.layoutViewField7.Location = new System.Drawing.Point(0, 100);
            this.layoutViewField7.Name = "layoutViewField7";
            this.layoutViewField7.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField7.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn8
            // 
            this.layoutViewColumn8.Caption = "Sort Column";
            this.layoutViewColumn8.FieldName = "SortColumn";
            this.layoutViewColumn8.LayoutViewField = this.layoutViewField8;
            this.layoutViewColumn8.Name = "layoutViewColumn8";
            // 
            // layoutViewField8
            // 
            this.layoutViewField8.EditorPreferredWidth = 10;
            this.layoutViewField8.Location = new System.Drawing.Point(0, 120);
            this.layoutViewField8.Name = "layoutViewField8";
            this.layoutViewField8.Size = new System.Drawing.Size(127, 20);
            this.layoutViewField8.TextSize = new System.Drawing.Size(95, 20);
            // 
            // layoutViewColumn9
            // 
            this.layoutViewColumn9.Caption = "Track TCPA";
            this.layoutViewColumn9.FieldName = "TrackTcpa";
            this.layoutViewColumn9.LayoutViewField = this.layoutViewField9;
            this.layoutViewColumn9.Name = "layoutViewColumn9";
            // 
            // layoutViewField9
            // 
            this.layoutViewField9.EditorPreferredWidth = 10;
            this.layoutViewField9.Location = new System.Drawing.Point(0, 120);
            this.layoutViewField9.Name = "layoutViewField9";
            this.layoutViewField9.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField9.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn10
            // 
            this.layoutViewColumn10.Caption = "Track Cpa";
            this.layoutViewColumn10.FieldName = "TrackCpa";
            this.layoutViewColumn10.LayoutViewField = this.layoutViewField10;
            this.layoutViewColumn10.Name = "layoutViewColumn10";
            // 
            // layoutViewField10
            // 
            this.layoutViewField10.EditorPreferredWidth = 10;
            this.layoutViewField10.Location = new System.Drawing.Point(0, 140);
            this.layoutViewField10.Name = "layoutViewField10";
            this.layoutViewField10.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField10.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn11
            // 
            this.layoutViewColumn11.Caption = "Bearing";
            this.layoutViewColumn11.FieldName = "TargetBearing";
            this.layoutViewColumn11.LayoutViewField = this.layoutViewField11;
            this.layoutViewColumn11.Name = "layoutViewColumn11";
            // 
            // layoutViewField11
            // 
            this.layoutViewField11.EditorPreferredWidth = 10;
            this.layoutViewField11.Location = new System.Drawing.Point(0, 160);
            this.layoutViewField11.Name = "layoutViewField11";
            this.layoutViewField11.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField11.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewColumn12
            // 
            this.layoutViewColumn12.Caption = "Distance";
            this.layoutViewColumn12.FieldName = "TargetDistance";
            this.layoutViewColumn12.LayoutViewField = this.layoutViewField12;
            this.layoutViewColumn12.Name = "layoutViewColumn12";
            // 
            // layoutViewField12
            // 
            this.layoutViewField12.EditorPreferredWidth = 10;
            this.layoutViewField12.Location = new System.Drawing.Point(0, 180);
            this.layoutViewField12.Name = "layoutViewField12";
            this.layoutViewField12.Size = new System.Drawing.Size(88, 20);
            this.layoutViewField12.TextSize = new System.Drawing.Size(62, 13);
            // 
            // layoutViewCard2
            // 
            this.layoutViewCard2.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.layoutViewCard2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutViewField1,
            this.layoutViewField3,
            this.layoutViewField4,
            this.layoutViewField5,
            this.layoutViewField6,
            this.layoutViewField7,
            this.layoutViewField9,
            this.layoutViewField10,
            this.layoutViewField11,
            this.layoutViewField12});
            this.layoutViewCard2.Name = "layoutViewTemplateCard";
            // 
            // layoutControlGroup8
            // 
            this.layoutControlGroup8.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup8.GroupBordersVisible = false;
            this.layoutControlGroup8.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem23});
            this.layoutControlGroup8.Name = "Root";
            this.layoutControlGroup8.Size = new System.Drawing.Size(267, 515);
            this.layoutControlGroup8.TextVisible = false;
            // 
            // layoutControlItem23
            // 
            this.layoutControlItem23.Control = this.gridControl5;
            this.layoutControlItem23.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem23.Name = "layoutControlItem23";
            this.layoutControlItem23.Size = new System.Drawing.Size(247, 495);
            this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem23.TextVisible = false;
            // 
            // groupControl7
            // 
            this.groupControl7.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupControl7.AppearanceCaption.Options.UseFont = true;
            this.groupControl7.AppearanceCaption.Options.UseTextOptions = true;
            this.groupControl7.AppearanceCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.groupControl7.Controls.Add(this.layoutControl10);
            this.groupControl7.Location = new System.Drawing.Point(328, 194);
            this.groupControl7.Name = "groupControl7";
            this.groupControl7.Size = new System.Drawing.Size(311, 578);
            this.groupControl7.TabIndex = 72;
            this.groupControl7.Text = "AIS Information";
            this.groupControl7.Visible = false;
            // 
            // layoutControl10
            // 
            this.layoutControl10.Controls.Add(this.textEdit21);
            this.layoutControl10.Controls.Add(this.textEdit22);
            this.layoutControl10.Controls.Add(this.textEdit23);
            this.layoutControl10.Controls.Add(this.textEdit24);
            this.layoutControl10.Controls.Add(this.textEdit25);
            this.layoutControl10.Controls.Add(this.textEdit26);
            this.layoutControl10.Controls.Add(this.textEdit27);
            this.layoutControl10.Controls.Add(this.textEdit28);
            this.layoutControl10.Controls.Add(this.textEdit29);
            this.layoutControl10.Controls.Add(this.textEdit30);
            this.layoutControl10.Controls.Add(this.textEdit31);
            this.layoutControl10.Controls.Add(this.textEdit32);
            this.layoutControl10.Controls.Add(this.textEdit33);
            this.layoutControl10.Controls.Add(this.textEdit34);
            this.layoutControl10.Controls.Add(this.textEdit35);
            this.layoutControl10.Controls.Add(this.textEdit36);
            this.layoutControl10.Controls.Add(this.textEdit37);
            this.layoutControl10.Controls.Add(this.textEdit38);
            this.layoutControl10.Controls.Add(this.textEdit39);
            this.layoutControl10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl10.Location = new System.Drawing.Point(2, 24);
            this.layoutControl10.Name = "layoutControl10";
            this.layoutControl10.Root = this.layoutControlGroup10;
            this.layoutControl10.Size = new System.Drawing.Size(307, 552);
            this.layoutControl10.TabIndex = 0;
            this.layoutControl10.Text = "layoutControl10";
            // 
            // textEdit21
            // 
            this.textEdit21.Location = new System.Drawing.Point(94, 12);
            this.textEdit21.MenuManager = this.barManager1;
            this.textEdit21.Name = "textEdit21";
            this.textEdit21.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textEdit21.Properties.Appearance.Options.UseFont = true;
            this.textEdit21.Size = new System.Drawing.Size(201, 26);
            this.textEdit21.StyleController = this.layoutControl10;
            this.textEdit21.TabIndex = 4;
            // 
            // textEdit22
            // 
            this.textEdit22.Location = new System.Drawing.Point(94, 39);
            this.textEdit22.MenuManager = this.barManager1;
            this.textEdit22.Name = "textEdit22";
            this.textEdit22.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textEdit22.Properties.Appearance.Options.UseFont = true;
            this.textEdit22.Size = new System.Drawing.Size(201, 26);
            this.textEdit22.StyleController = this.layoutControl10;
            this.textEdit22.TabIndex = 5;
            // 
            // textEdit23
            // 
            this.textEdit23.Location = new System.Drawing.Point(94, 102);
            this.textEdit23.MenuManager = this.barManager1;
            this.textEdit23.Name = "textEdit23";
            this.textEdit23.Size = new System.Drawing.Size(201, 20);
            this.textEdit23.StyleController = this.layoutControl10;
            this.textEdit23.TabIndex = 6;
            // 
            // textEdit24
            // 
            this.textEdit24.Location = new System.Drawing.Point(94, 126);
            this.textEdit24.MenuManager = this.barManager1;
            this.textEdit24.Name = "textEdit24";
            this.textEdit24.Size = new System.Drawing.Size(201, 20);
            this.textEdit24.StyleController = this.layoutControl10;
            this.textEdit24.TabIndex = 7;
            // 
            // textEdit25
            // 
            this.textEdit25.Location = new System.Drawing.Point(94, 150);
            this.textEdit25.MenuManager = this.barManager1;
            this.textEdit25.Name = "textEdit25";
            this.textEdit25.Size = new System.Drawing.Size(201, 20);
            this.textEdit25.StyleController = this.layoutControl10;
            this.textEdit25.TabIndex = 8;
            // 
            // textEdit26
            // 
            this.textEdit26.Location = new System.Drawing.Point(94, 174);
            this.textEdit26.MenuManager = this.barManager1;
            this.textEdit26.Name = "textEdit26";
            this.textEdit26.Size = new System.Drawing.Size(201, 20);
            this.textEdit26.StyleController = this.layoutControl10;
            this.textEdit26.TabIndex = 9;
            // 
            // textEdit27
            // 
            this.textEdit27.Location = new System.Drawing.Point(94, 198);
            this.textEdit27.MenuManager = this.barManager1;
            this.textEdit27.Name = "textEdit27";
            this.textEdit27.Size = new System.Drawing.Size(201, 20);
            this.textEdit27.StyleController = this.layoutControl10;
            this.textEdit27.TabIndex = 10;
            // 
            // textEdit28
            // 
            this.textEdit28.Location = new System.Drawing.Point(94, 222);
            this.textEdit28.MenuManager = this.barManager1;
            this.textEdit28.Name = "textEdit28";
            this.textEdit28.Size = new System.Drawing.Size(201, 20);
            this.textEdit28.StyleController = this.layoutControl10;
            this.textEdit28.TabIndex = 11;
            // 
            // textEdit29
            // 
            this.textEdit29.Location = new System.Drawing.Point(94, 246);
            this.textEdit29.MenuManager = this.barManager1;
            this.textEdit29.Name = "textEdit29";
            this.textEdit29.Size = new System.Drawing.Size(201, 20);
            this.textEdit29.StyleController = this.layoutControl10;
            this.textEdit29.TabIndex = 12;
            // 
            // textEdit30
            // 
            this.textEdit30.Location = new System.Drawing.Point(94, 303);
            this.textEdit30.MenuManager = this.barManager1;
            this.textEdit30.Name = "textEdit30";
            this.textEdit30.Size = new System.Drawing.Size(201, 20);
            this.textEdit30.StyleController = this.layoutControl10;
            this.textEdit30.TabIndex = 13;
            // 
            // textEdit31
            // 
            this.textEdit31.Location = new System.Drawing.Point(94, 327);
            this.textEdit31.MenuManager = this.barManager1;
            this.textEdit31.Name = "textEdit31";
            this.textEdit31.Size = new System.Drawing.Size(201, 20);
            this.textEdit31.StyleController = this.layoutControl10;
            this.textEdit31.TabIndex = 14;
            // 
            // textEdit32
            // 
            this.textEdit32.Location = new System.Drawing.Point(94, 351);
            this.textEdit32.MenuManager = this.barManager1;
            this.textEdit32.Name = "textEdit32";
            this.textEdit32.Size = new System.Drawing.Size(201, 20);
            this.textEdit32.StyleController = this.layoutControl10;
            this.textEdit32.TabIndex = 15;
            // 
            // textEdit33
            // 
            this.textEdit33.Location = new System.Drawing.Point(94, 375);
            this.textEdit33.MenuManager = this.barManager1;
            this.textEdit33.Name = "textEdit33";
            this.textEdit33.Size = new System.Drawing.Size(201, 20);
            this.textEdit33.StyleController = this.layoutControl10;
            this.textEdit33.TabIndex = 16;
            // 
            // textEdit34
            // 
            this.textEdit34.Location = new System.Drawing.Point(94, 399);
            this.textEdit34.MenuManager = this.barManager1;
            this.textEdit34.Name = "textEdit34";
            this.textEdit34.Size = new System.Drawing.Size(201, 20);
            this.textEdit34.StyleController = this.layoutControl10;
            this.textEdit34.TabIndex = 17;
            // 
            // textEdit35
            // 
            this.textEdit35.Location = new System.Drawing.Point(94, 423);
            this.textEdit35.MenuManager = this.barManager1;
            this.textEdit35.Name = "textEdit35";
            this.textEdit35.Size = new System.Drawing.Size(201, 20);
            this.textEdit35.StyleController = this.layoutControl10;
            this.textEdit35.TabIndex = 18;
            // 
            // textEdit36
            // 
            this.textEdit36.Location = new System.Drawing.Point(94, 447);
            this.textEdit36.MenuManager = this.barManager1;
            this.textEdit36.Name = "textEdit36";
            this.textEdit36.Size = new System.Drawing.Size(201, 20);
            this.textEdit36.StyleController = this.layoutControl10;
            this.textEdit36.TabIndex = 19;
            // 
            // textEdit37
            // 
            this.textEdit37.Location = new System.Drawing.Point(94, 471);
            this.textEdit37.MenuManager = this.barManager1;
            this.textEdit37.Name = "textEdit37";
            this.textEdit37.Size = new System.Drawing.Size(201, 20);
            this.textEdit37.StyleController = this.layoutControl10;
            this.textEdit37.TabIndex = 20;
            // 
            // textEdit38
            // 
            this.textEdit38.Location = new System.Drawing.Point(94, 495);
            this.textEdit38.MenuManager = this.barManager1;
            this.textEdit38.Name = "textEdit38";
            this.textEdit38.Size = new System.Drawing.Size(201, 20);
            this.textEdit38.StyleController = this.layoutControl10;
            this.textEdit38.TabIndex = 21;
            // 
            // textEdit39
            // 
            this.textEdit39.Location = new System.Drawing.Point(94, 519);
            this.textEdit39.MenuManager = this.barManager1;
            this.textEdit39.Name = "textEdit39";
            this.textEdit39.Size = new System.Drawing.Size(201, 20);
            this.textEdit39.StyleController = this.layoutControl10;
            this.textEdit39.TabIndex = 22;
            // 
            // layoutControlGroup10
            // 
            this.layoutControlGroup10.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup10.GroupBordersVisible = false;
            this.layoutControlGroup10.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem43,
            this.layoutControlItem44,
            this.layoutControlItem45,
            this.layoutControlItem46,
            this.layoutControlItem47,
            this.layoutControlItem48,
            this.layoutControlItem49,
            this.layoutControlItem50,
            this.layoutControlItem51,
            this.layoutControlItem52,
            this.layoutControlItem53,
            this.layoutControlItem54,
            this.layoutControlItem55,
            this.layoutControlItem56,
            this.layoutControlItem57,
            this.layoutControlItem58,
            this.layoutControlItem59,
            this.layoutControlItem60,
            this.layoutControlItem61,
            this.emptySpaceItem2,
            this.emptySpaceItem3});
            this.layoutControlGroup10.Name = "layoutControlGroup9";
            this.layoutControlGroup10.Size = new System.Drawing.Size(307, 552);
            this.layoutControlGroup10.TextVisible = false;
            // 
            // layoutControlItem43
            // 
            this.layoutControlItem43.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem43.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem43.Control = this.textEdit21;
            this.layoutControlItem43.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem43.MaxSize = new System.Drawing.Size(287, 27);
            this.layoutControlItem43.MinSize = new System.Drawing.Size(287, 27);
            this.layoutControlItem43.Name = "layoutControlItem24";
            this.layoutControlItem43.Size = new System.Drawing.Size(287, 27);
            this.layoutControlItem43.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem43.Text = "Name";
            this.layoutControlItem43.TextSize = new System.Drawing.Size(70, 23);
            // 
            // layoutControlItem44
            // 
            this.layoutControlItem44.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem44.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem44.Control = this.textEdit22;
            this.layoutControlItem44.Location = new System.Drawing.Point(0, 27);
            this.layoutControlItem44.MaxSize = new System.Drawing.Size(287, 27);
            this.layoutControlItem44.MinSize = new System.Drawing.Size(287, 27);
            this.layoutControlItem44.Name = "layoutControlItem25";
            this.layoutControlItem44.Size = new System.Drawing.Size(287, 27);
            this.layoutControlItem44.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem44.Text = "MMSI";
            this.layoutControlItem44.TextSize = new System.Drawing.Size(70, 23);
            // 
            // layoutControlItem45
            // 
            this.layoutControlItem45.Control = this.textEdit23;
            this.layoutControlItem45.Location = new System.Drawing.Point(0, 90);
            this.layoutControlItem45.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem45.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem45.Name = "layoutControlItem26";
            this.layoutControlItem45.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem45.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem45.Text = "CallSign";
            this.layoutControlItem45.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem46
            // 
            this.layoutControlItem46.Control = this.textEdit24;
            this.layoutControlItem46.Location = new System.Drawing.Point(0, 114);
            this.layoutControlItem46.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem46.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem46.Name = "layoutControlItem27";
            this.layoutControlItem46.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem46.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem46.Text = "IMO";
            this.layoutControlItem46.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem47
            // 
            this.layoutControlItem47.Control = this.textEdit25;
            this.layoutControlItem47.Location = new System.Drawing.Point(0, 138);
            this.layoutControlItem47.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem47.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem47.Name = "layoutControlItem28";
            this.layoutControlItem47.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem47.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem47.Text = "AIS Class";
            this.layoutControlItem47.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem48
            // 
            this.layoutControlItem48.Control = this.textEdit26;
            this.layoutControlItem48.Location = new System.Drawing.Point(0, 162);
            this.layoutControlItem48.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem48.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem48.Name = "layoutControlItem29";
            this.layoutControlItem48.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem48.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem48.Text = "Length (m)";
            this.layoutControlItem48.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem49
            // 
            this.layoutControlItem49.Control = this.textEdit27;
            this.layoutControlItem49.Location = new System.Drawing.Point(0, 186);
            this.layoutControlItem49.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem49.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem49.Name = "layoutControlItem30";
            this.layoutControlItem49.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem49.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem49.Text = "Beam (m)";
            this.layoutControlItem49.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem50
            // 
            this.layoutControlItem50.Control = this.textEdit28;
            this.layoutControlItem50.Location = new System.Drawing.Point(0, 210);
            this.layoutControlItem50.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem50.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem50.Name = "layoutControlItem31";
            this.layoutControlItem50.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem50.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem50.Text = "Draft (m)";
            this.layoutControlItem50.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem51
            // 
            this.layoutControlItem51.Control = this.textEdit29;
            this.layoutControlItem51.Location = new System.Drawing.Point(0, 234);
            this.layoutControlItem51.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem51.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem51.Name = "layoutControlItem32";
            this.layoutControlItem51.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem51.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem51.Text = "Ship Type";
            this.layoutControlItem51.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem52
            // 
            this.layoutControlItem52.Control = this.textEdit30;
            this.layoutControlItem52.Location = new System.Drawing.Point(0, 291);
            this.layoutControlItem52.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem52.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem52.Name = "layoutControlItem33";
            this.layoutControlItem52.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem52.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem52.Text = "Nav Status";
            this.layoutControlItem52.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem53
            // 
            this.layoutControlItem53.Control = this.textEdit31;
            this.layoutControlItem53.Location = new System.Drawing.Point(0, 315);
            this.layoutControlItem53.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem53.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem53.Name = "layoutControlItem34";
            this.layoutControlItem53.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem53.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem53.Text = "Latitude";
            this.layoutControlItem53.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem54
            // 
            this.layoutControlItem54.Control = this.textEdit32;
            this.layoutControlItem54.Location = new System.Drawing.Point(0, 339);
            this.layoutControlItem54.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem54.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem54.Name = "layoutControlItem35";
            this.layoutControlItem54.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem54.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem54.Text = "Longitude";
            this.layoutControlItem54.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem55
            // 
            this.layoutControlItem55.Control = this.textEdit33;
            this.layoutControlItem55.Location = new System.Drawing.Point(0, 363);
            this.layoutControlItem55.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem55.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem55.Name = "layoutControlItem36";
            this.layoutControlItem55.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem55.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem55.Text = "GPS Accuracy";
            this.layoutControlItem55.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem56
            // 
            this.layoutControlItem56.Control = this.textEdit34;
            this.layoutControlItem56.Location = new System.Drawing.Point(0, 387);
            this.layoutControlItem56.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem56.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem56.Name = "layoutControlItem37";
            this.layoutControlItem56.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem56.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem56.Text = "ROT";
            this.layoutControlItem56.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem57
            // 
            this.layoutControlItem57.Control = this.textEdit35;
            this.layoutControlItem57.Location = new System.Drawing.Point(0, 411);
            this.layoutControlItem57.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem57.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem57.Name = "layoutControlItem38";
            this.layoutControlItem57.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem57.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem57.Text = "SOG";
            this.layoutControlItem57.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem58
            // 
            this.layoutControlItem58.Control = this.textEdit36;
            this.layoutControlItem58.Location = new System.Drawing.Point(0, 435);
            this.layoutControlItem58.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem58.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem58.Name = "layoutControlItem39";
            this.layoutControlItem58.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem58.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem58.Text = "COG";
            this.layoutControlItem58.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem59
            // 
            this.layoutControlItem59.Control = this.textEdit37;
            this.layoutControlItem59.Location = new System.Drawing.Point(0, 459);
            this.layoutControlItem59.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem59.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem59.Name = "layoutControlItem40";
            this.layoutControlItem59.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem59.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem59.Text = "Heading";
            this.layoutControlItem59.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem60
            // 
            this.layoutControlItem60.Control = this.textEdit38;
            this.layoutControlItem60.Location = new System.Drawing.Point(0, 483);
            this.layoutControlItem60.MaxSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem60.MinSize = new System.Drawing.Size(287, 24);
            this.layoutControlItem60.Name = "layoutControlItem41";
            this.layoutControlItem60.Size = new System.Drawing.Size(287, 24);
            this.layoutControlItem60.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem60.Text = "Destination";
            this.layoutControlItem60.TextSize = new System.Drawing.Size(70, 13);
            // 
            // layoutControlItem61
            // 
            this.layoutControlItem61.Control = this.textEdit39;
            this.layoutControlItem61.Location = new System.Drawing.Point(0, 507);
            this.layoutControlItem61.MaxSize = new System.Drawing.Size(287, 25);
            this.layoutControlItem61.MinSize = new System.Drawing.Size(287, 25);
            this.layoutControlItem61.Name = "layoutControlItem42";
            this.layoutControlItem61.Size = new System.Drawing.Size(287, 25);
            this.layoutControlItem61.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem61.Text = "ETA";
            this.layoutControlItem61.TextSize = new System.Drawing.Size(70, 13);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emptySpaceItem2.AppearanceItemCaption.Options.UseFont = true;
            this.emptySpaceItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.emptySpaceItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 54);
            this.emptySpaceItem2.MaxSize = new System.Drawing.Size(286, 36);
            this.emptySpaceItem2.MinSize = new System.Drawing.Size(286, 36);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(287, 36);
            this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem2.Text = "General Information";
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(70, 0);
            this.emptySpaceItem2.TextVisible = true;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emptySpaceItem3.AppearanceItemCaption.Options.UseFont = true;
            this.emptySpaceItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.emptySpaceItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.emptySpaceItem3.Location = new System.Drawing.Point(0, 258);
            this.emptySpaceItem3.MaxSize = new System.Drawing.Size(286, 33);
            this.emptySpaceItem3.MinSize = new System.Drawing.Size(286, 33);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(287, 33);
            this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem3.Text = "Navigation Information";
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(70, 0);
            this.emptySpaceItem3.TextVisible = true;
            // 
            // windowsUIButtonPanel9
            // 
            this.windowsUIButtonPanel9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsUIButtonPanel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            windowsUIButtonImageOptions5.Image = global::EncViewWithCSharp.Properties.Resources.radio_16x161;
            this.windowsUIButtonPanel9.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Priority", true, windowsUIButtonImageOptions5, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false)});
            this.windowsUIButtonPanel9.ForeColor = System.Drawing.Color.White;
            this.windowsUIButtonPanel9.Location = new System.Drawing.Point(1527, 493);
            this.windowsUIButtonPanel9.Name = "windowsUIButtonPanel9";
            this.windowsUIButtonPanel9.Size = new System.Drawing.Size(75, 67);
            this.windowsUIButtonPanel9.TabIndex = 77;
            this.windowsUIButtonPanel9.Text = "windowsUIButtonPanel9";
            // 
            // windowsUIButtonPanel10
            // 
            this.windowsUIButtonPanel10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.windowsUIButtonPanel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            windowsUIButtonImageOptions2.Image = global::EncViewWithCSharp.Properties.Resources.geopoint_16x165;
            windowsUIButtonImageOptions3.Image = global::EncViewWithCSharp.Properties.Resources.line2_16x163;
            windowsUIButtonImageOptions4.Image = global::EncViewWithCSharp.Properties.Resources.spline_32x321;
            this.windowsUIButtonPanel10.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("WayPoints", true, windowsUIButtonImageOptions2, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Routes", true, windowsUIButtonImageOptions3, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton(),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton("Measure", true, windowsUIButtonImageOptions4, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, "", -1, true, null, true, false, true, null, -1, false),
            new DevExpress.XtraBars.Docking2010.WindowsUIButton()});
            this.windowsUIButtonPanel10.ForeColor = System.Drawing.Color.White;
            this.windowsUIButtonPanel10.Location = new System.Drawing.Point(1527, 92);
            this.windowsUIButtonPanel10.Name = "windowsUIButtonPanel10";
            this.windowsUIButtonPanel10.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.windowsUIButtonPanel10.Size = new System.Drawing.Size(75, 321);
            this.windowsUIButtonPanel10.TabIndex = 82;
            this.windowsUIButtonPanel10.Text = "windowsUIButtonPanel10";
            this.windowsUIButtonPanel10.ButtonUnchecked += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.windowsUIButtonPanel10_ButtonUnchecked);
            this.windowsUIButtonPanel10.ButtonChecked += new DevExpress.XtraBars.Docking2010.ButtonEventHandler(this.windowsUIButtonPanel10_ButtonChecked);
            // 
            // FormEnc
            // 
            this.Appearance.ForeColor = System.Drawing.Color.Black;
            this.Appearance.Options.UseFont = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
            this.ClientSize = new System.Drawing.Size(1819, 811);
            this.Controls.Add(this.windowsUIButtonPanel10);
            this.Controls.Add(this.windowsUIButtonPanel9);
            this.Controls.Add(this.groupControl7);
            this.Controls.Add(this.groupControl5);
            this.Controls.Add(this.groupControl4);
            this.Controls.Add(this.tabPane3);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.groupControl3);
            this.Controls.Add(this.windowsUIButtonPanel8);
            this.Controls.Add(this.windowsUIButtonPanel7);
            this.Controls.Add(this.windowsUIButtonPanel5);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.windowsUIButtonPanel2);
            this.Controls.Add(this.windowsUIButtonPanel4);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.windowsUIButtonPanel1);
            this.Controls.Add(this.S57Control1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("FormEnc.IconOptions.Icon")));
            this.IconOptions.Image = ((System.Drawing.Image)(resources.GetObject("FormEnc.IconOptions.Image")));
            this.Name = "FormEnc";
            this.Text = "VOYAGER AI VESSEL";
            this.Load += new System.EventHandler(this.FormEnc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelLatitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItemHeading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItemSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItemDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelCursorLatitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelCursorAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelLongitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelcursorLongitude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelCursorRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl4)).EndInit();
            this.groupControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
            this.groupControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.messagelabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.messagelabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
            this.tabNavigationPage7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).EndInit();
            this.layoutControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
            this.tabNavigationPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).EndInit();
            this.layoutControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorPickedit_routeColour.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtedit_routeName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoedit_routeDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spintxtedit_usvspeed.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtedit_Totaldistance.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtedit_Timetake.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.tabNavigationPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wayptxt_Name.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wayptxt_Lat.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wayptxt_Lon.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabPane3)).EndInit();
            this.tabPane3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl5)).EndInit();
            this.groupControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl8)).EndInit();
            this.layoutControl8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl7)).EndInit();
            this.groupControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl10)).EndInit();
            this.layoutControl10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textEdit21.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit22.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit23.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit24.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit25.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit26.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit27.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit28.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit29.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit30.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit31.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit32.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit33.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit34.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit35.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit36.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit37.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit38.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit39.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem43)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem44)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem45)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem46)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem47)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem48)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem49)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem50)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem51)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem52)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem53)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem54)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem55)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem56)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem57)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem58)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem59)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem60)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem61)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        [STAThread]
        private static void Main()
        {
            Application.Run(new FormEnc());
        }

        private void InitializeENCXControl()
        {
            m_lib.Construct("", "");
            var init = new S57ManagerInitialisationData
            {
                SencPath = "C:\\SENCS\\USSENC",
                ReadOnly = true, // ReadOnly is simplest (more advanced ways are more functional)
                WVSPath = @"C:\Users\Robosys\Documents\Voyager Background\wvs.cvcf"
            };
            init.ReadOnly = true;

            m_catNotify.OnChanged += m_catNotify_OnChanged;
            m_catNotify.Register(m_senc);
            m_senc.OpenSenc2(m_lib, init);

            m_draw.SetManager(m_senc);
            m_draw.DisplaySettings.set_ViewingGroup(11050, false);
            m_draw.DrawGrid = true;

            _markGeoPoint = new GeoPoint();
            _clickGeoPt = new GeoPoint();
            _routeWayPointsList = new List<GeoCoordinate>();
            _measuredList = new List<GeoCoordinate>();

            _homeImage = Resources.home_32x321;
            _anchorImage = Resources.delete_32x32;
            _mark = Resources.add16x161;
            _usvImage = Resources.USV;
            _waypointImage = Resources.geopoint_16x16;
            _conformationIcon = Resources.index_32x32;
            _goalPointimage = Resources.goalpointImage;
            _routeIcon = Resources.line2_16x16;
            _measureIcon = Resources.MeasureIcon;
            _aisImage = Resources.Target.ToBitmap();
            _orangeaisImage = Resources.TargetOrange.ToBitmap();
            _greenaisImage = Resources.TargetGreen.ToBitmap();
            _redaisImage = Resources.TargetRed.ToBitmap();

            GeoRect rect = new GeoRectClass();
            rect.LatN = 90;
            rect.LonW = -180;
            rect.LatS = -90;
            rect.LonE = 180;

            m_draw.SetPositionAndScale(rect); // Point the control at the sample data

            S57Control1.Init(m_draw, m_lib.Dongle);
            UpdateDongleLabel();
            m_dongleNotify.Register(m_lib.Dongle);
            SetupDisplaySettings();

            _defaultPixelPoint = new PixelPoint { X = S57Control1.Width / 2f, Y = S57Control1.Height - 200 };


            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            string folderPath = Path.Combine(directoryPath, "NaveenClient");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            _wayPointPath = Path.Combine(folderPath, "WaypointsTemp.txt");
            if (!File.Exists(_wayPointPath))
            {
                var fs = File.Create(_wayPointPath);
                fs.Flush();
                fs.Close();
            }

            _routePointsPath = Path.Combine(folderPath, "RoutesTemp.txt");
            if (!File.Exists(_routePointsPath))
            {
                var fs = File.Create(_routePointsPath);
                fs.Flush();
                fs.Close();
            }

            _geofencePath = Path.Combine(folderPath, "GeoFenceTemp.txt");
            if (!File.Exists(_geofencePath))
            {
                var fs = File.Create(_geofencePath);
                fs.Flush();
                fs.Close();
            }

            _recordingPath = "C:\\Users\\Public\\Documents\\NaveenClient\\Recorded Videos";

            var videoRc = new RecorderOptions { RecorderMode = RecorderMode.Video };
            _videoRecorder = Recorder.CreateRecorder(videoRc);
            //_videoRecorder.OnRecordingFailed += VideoRecorder_OnRecordingFailed;
            _videoRecorder.OnRecordingComplete += VideoRecorder_OnRecordingComplete;

            var snapRc = new RecorderOptions { RecorderMode = RecorderMode.Snapshot };
            _snapRecorder = Recorder.CreateRecorder(snapRc);
            //_snapRecorder.OnRecordingFailed += SnapRecorder_OnRecordingFailed;
            _snapRecorder.OnRecordingComplete += SnapRecorder_OnRecordingComplete;

            UpdateTimer = new Timer(10);
            UpdateTimer.Elapsed += _updateTimer_Elapsed;

            _refreshTimer = new Timer(100);
            _refreshTimer.Elapsed += _refreshTimer_Elapsed;

            Task.Factory.StartNew(() =>
            {
                //Getting usv location
                Thread.Sleep(1);
                DataRead.DumpMessageLoop();
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                //Getting ais location
                Thread.Sleep(1);
                AIS_Read.AIS_Read.DumpaisMessageLoop();
            }, TaskCreationOptions.LongRunning);

            _wayPointData = new DataTable();
            _wayPointData.Columns.Add("WayPointName", typeof(string));
            _wayPointData.Columns.Add("Latitude", typeof(string));
            _wayPointData.Columns.Add("Longitude", typeof(string));

            ReadStoredWayPoints();
            gridControl1.DataSource = _wayPointData;

            _routePointsData = new DataTable();
            _routePointsData.Columns.Add("Index", typeof(int));
            _routePointsData.Columns.Add("RouteName", typeof(string));
            _routePointsData.Columns.Add("Description", typeof(string));
            _routePointsData.Columns.Add("Colour", typeof(string));
            _routePointsData.Columns.Add("USV Speed", typeof(string));
            _routePointsData.Columns.Add("Total Distance", typeof(string));
            _routePointsData.Columns.Add("Cover Time", typeof(string));
            _routePointsData.Columns.Add("waypoints of Route", typeof(string));
            _routePointsData.Columns.Add("Show Route", typeof(bool));
            _routePointsData.PrimaryKey = new[] { _routePointsData.Columns["Index"] };

            ReadStoredRoutes();
            gridControl2.DataSource = _routePointsData;

            _geofenceData = new DataTable();
            _geofenceData.Columns.Add("Index", typeof(int));
            _geofenceData.Columns.Add("GeoFenceName", typeof(string));
            _geofenceData.Columns.Add("Notes", typeof(string));
            _geofenceData.Columns.Add("GeoFencePoints", typeof(string));
            _geofenceData.PrimaryKey = new[] { _geofenceData.Columns["Index"] };

            //ReadStoredGeofences();
            gridControl3.DataSource = _geofenceData;

            _aisData = new DataTable();
            _aisData.Columns.Add("TargetNo", typeof(string));
            _aisData.Columns.Add("TargetName", typeof(string));
            _aisData.Columns.Add("TargetMmsi", typeof(string));
            _aisData.Columns.Add("TargetCourse", typeof(string));
            _aisData.Columns.Add("TargetSpeed", typeof(string));
            _aisData.Columns.Add("TargetCpa", typeof(string));
            _aisData.Columns.Add("TargetTcpa", typeof(string));
            _aisData.Columns.Add("TargetImage", typeof(Image));
            _aisData.Columns.Add("TargetCpaVal", typeof(double));
            _aisData.Columns.Add("TargetTcpaVal", typeof(double));
            _aisData.Columns.Add("TrackCpa", typeof(string));
            _aisData.Columns.Add("TrackTcpa", typeof(string));
            _aisData.Columns.Add("SortColumn", typeof(int));
            _aisData.Columns.Add("TargetBearing", typeof(string));
            _aisData.Columns.Add("TargetDistance", typeof(string));
            _aisData.Columns.Add("TargetImageRectangle", typeof(Rectangle));
            _aisData.Columns.Add("TargetLastUpdatedTime", typeof(DateTime));
            gridControl5.DataSource = _aisData;
        }

        private void ReadStoredWayPoints()
        {
            var wayPointsInfo = File.ReadAllLines(_wayPointPath);
            foreach (var waypoint in wayPointsInfo)
            {
                var rowInfo = waypoint.Split(';');
                if (rowInfo.Length != 3) continue;
                var row = _wayPointData.NewRow();
                row["WayPointName"] = rowInfo[0];

                var geoCr = new GeoCoordinate()
                {
                    Latitude = Convert.ToDouble(rowInfo[1]),
                    Longitude = Convert.ToDouble(rowInfo[2])
                };
                string[] cardinalSg = DataRead.GetCardinalCoordinates(geoCr);
                row["Latitude"] = cardinalSg[0];
                row["Longitude"] = cardinalSg[1];
                _wayPointData.Rows.Add(row);
            }
            gridControl1.RefreshDataSource();
        }

        private void ReadStoredRoutes()
        {
            _routePointsData.Rows.Clear();
            var routesInfo = File.ReadAllLines(_routePointsPath);
            foreach (var route in routesInfo)
            {
                var routeInfo = route.Split(';');
                if (routeInfo.Length != 8) continue;
                var row = _routePointsData.NewRow();
                row["Index"] = routeInfo[0];
                row["RouteName"] = routeInfo[1];
                row["Description"] = routeInfo[2];
                row["Colour"] = routeInfo[3];
                row["USV Speed"] = routeInfo[4];
                row["Total Distance"] = routeInfo[5];
                row["Cover Time"] = routeInfo[6];
                row["waypoints of Route"] = routeInfo[7];
                row["Show Route"] = true;
                _routePointsData.Rows.Add(row);
            }
            gridControl2.RefreshDataSource();
        }

        private void _refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            S57Control1.Invalidate();
        }

        private void _updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invoke((MethodInvoker)(() =>
            {
                barStaticItem20.Caption =
                              $@"{" Scale : 1:" + m_draw.DisplayScale}	{"Local Time : " + DateTime.Now.ToLocalTime()}";

                string[] cardinalString = DataRead.GetCardinalCoordinates(DataRead._USVGeoCoordinate);

                simpleLabelLatitude.Text = DataRead._USVGeoCoordinate != null ? cardinalString[0] : "--";
                simpleLabelLongitude.Text = DataRead._USVGeoCoordinate != null ? cardinalString[1] : "--";

                simpleLabelItemHeading.Text = DataRead.USV_Angle.ToString(CultureInfo.InvariantCulture) + '\u00B0';
                simpleLabelItemSpeed.Text = DataRead.USV_SOG + @"kn";
                simpleLabelItemDepth.Text = Math.Round(DataRead.depth, 2) + @"m";

                if (_cursorGeocoordinate != null && DataRead._USVGeoCoordinate != null)
                {
                    simpleLabelCursorAngle.Text = Convert
                        .ToInt32(Math.Round(DataRead.DegreeBearing(DataRead._USVGeoCoordinate.Latitude,
                            DataRead._USVGeoCoordinate.Longitude, _cursorGeocoordinate.Latitude,
                            _cursorGeocoordinate.Longitude))).ToString() + '\u00B0';
                    simpleLabelCursorRange.Text = Math
                        .Round(
                            DataRead.GetDistanceBetweenPoints(DataRead._USVGeoCoordinate.Latitude,
                                DataRead._USVGeoCoordinate.Longitude, _cursorGeocoordinate.Latitude,
                                _cursorGeocoordinate.Longitude), 3).ToString(CultureInfo.InvariantCulture) + @"m";

                    if (aisRectangle.Contains((int)_cursorPixelPoint.X, (int)_cursorPixelPoint.Y))
                    {
                        FillAisInfo();
                        groupControl7.Show();
                        groupControl7.Dock = DockStyle.Left;

                        S57Control1.Invalidate();
                    }
                    else
                    {
                        groupControl7.Hide();
                    }
                }
            }));
        }

        private void FillAisInfo()
        {
            textEdit21.Text = AIS_Read.AIS_Read._aisName;
            textEdit22.Text = AIS_Read.AIS_Read.ais_mmsi.ToString(CultureInfo.InvariantCulture);

            textEdit23.Text = AIS_Read.AIS_Read._aiscallSign;
            textEdit24.Text = AIS_Read.AIS_Read.ais_IMO.ToString(CultureInfo.InvariantCulture);
            textEdit25.Text = string.Empty;
            textEdit26.Text = AIS_Read.AIS_Read._shipLength.ToString(CultureInfo.InvariantCulture);
            textEdit27.Text = AIS_Read.AIS_Read._shipBeam.ToString(CultureInfo.InvariantCulture);
            textEdit28.Text = AIS_Read.AIS_Read._shipDraft.ToString(CultureInfo.InvariantCulture);
            textEdit29.Text = AIS_Read.AIS_Read.cargoShipType;
            textEdit30.Text = AIS_Read.AIS_Read._aisNavstatus;

            string[] cardinalString = DataRead.GetCardinalCoordinates(AIS_Read.AIS_Read._AISGeoCoordinate);
            textEdit31.Text = cardinalString[0];
            textEdit32.Text = cardinalString[1];

            textEdit33.Text = AIS_Read.AIS_Read.ais_positionAccuracy;
            textEdit34.Text = AIS_Read.AIS_Read._aisRateOfTurn.ToString(CultureInfo.InvariantCulture);
            textEdit35.Text = AIS_Read.AIS_Read.AIS_SOG.ToString(CultureInfo.InvariantCulture) + @" Kn";
            textEdit36.Text = AIS_Read.AIS_Read.AIS_Angle.ToString(CultureInfo.InvariantCulture);
            textEdit37.Text = AIS_Read.AIS_Read.AIS_Angle.ToString(CultureInfo.InvariantCulture);
            textEdit38.Text = string.Empty;
            textEdit39.Text = string.Empty;
        }

        private void SetupDisplaySettings()
        {
            m_draw.DisplaySettings.SetViewingGroupRange(0, m_draw.DisplaySettings.NumViewingGroups, true);
            m_draw.DisplaySettings.ViewingGroup[31010] = false;
            m_draw.DisplaySettings.ViewingGroup[21030] = false;
            m_draw.DisplaySettings.TextGroup[10] = true;
            m_draw.DisplaySettings.TextGroup[11] = true;
            m_draw.DisplaySettings.TextGroup[20] = true;
            m_draw.DisplaySettings.TextGroup[21] = true;
            m_draw.DisplaySettings.TextGroup[23] = true;
            m_draw.DisplaySettings.TextGroup[24] = true;
            m_draw.DisplaySettings.TextGroup[26] = true;
            m_draw.DisplaySettings.TextGroup[27] = true;
            m_draw.DisplaySettings.TextGroup[28] = true;
            m_draw.DisplaySettings.TextGroup[29] = true;
            m_draw.DisplaySettings.TextGroup[31] = true;
            m_draw.DisplaySettings.ViewingGroup[33010] = true;
        }

        private void m_catNotify_OnChanged()
        {
            S57Control1.Invalidate();
        }

        private void UpdateDongleLabel()
        {
            //Label2.Text = "Dongle " + m_lib.Dongle.DeviceName + "\r\n";
            //Label2.Text += "State " + m_lib.Dongle.State;
        }

        private void S57Control1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (pointTodragTuple != null && _routeDragStarted)
                {
                    gridControl2.RefreshDataSource();
                    _routeDragStarted = false;
                }
                _dragStartGeoPoint = null;
                pointTodragTuple = null;
            }
        }

        private void S57Control1_MouseDown(object sender, MouseEventArgs e)
        {
            var pixelPoint = new PixelPoint
            {
                X = e.X,
                Y = e.Y
            };
            var mouseDownGeoPoint = m_draw.GeoPix.GeoPointFromPixelPoint(pixelPoint);

            if (e.Button == MouseButtons.Left && (_currentChartMode == ChartMode.None || _currentChartMode == ChartMode.Drag))
            {
                if (_routeedit)
                {
                    var data = TestPoint(m_draw.GeoPix.Point(_markGeoPoint));
                    foreach (var ptdTuple in data.Where(ptdTuple =>
                                 ptdTuple.Item1 == pointType.RouteWayPoint))
                    {
                        pointTodragTuple = ptdTuple;
                        _currentChartMode = ChartMode.Drag;
                    }
                }
            }
            if (_currentChartMode == ChartMode.Drag && pointTodragTuple == null)
            {
                _dragStartGeoPoint = mouseDownGeoPoint;
            }
        }

        private void S57Control1_MouseMove(object sender, MouseEventArgs e)
        {
            _cursorPixelPoint = new PixelPoint();
            _cursorPixelPoint.X = e.X;
            _cursorPixelPoint.Y = e.Y;

            //for showing cursor position
            var cursorGeoPoint = m_draw.GeoPix.GeoPointFromPixelPoint(_cursorPixelPoint);

            _cursorGeocoordinate = new GeoCoordinate
            {
                Latitude = cursorGeoPoint.Lat,
                Longitude = cursorGeoPoint.Lon
            };

            string[] cardinalString = DataRead.GetCardinalCoordinates(cursorGeoPoint);

            simpleLabelCursorLatitude.Text = cardinalString[0];
            simpleLabelcursorLongitude.Text = cardinalString[1];

            if (_dragStartGeoPoint != null && e.Button == MouseButtons.Left)
            {
                m_draw.SetPosition(_dragStartGeoPoint, _cursorPixelPoint);
                S57Control1.Invalidate();
            }
            if (pointTodragTuple != null && e.Button == MouseButtons.Left)
            {
                var routePoints = GetPointsFromString(Convert.ToString(pointTodragTuple.Item2["waypoints of Route"]));
                routePoints[pointTodragTuple.Item3] = _cursorGeocoordinate;
                pointTodragTuple.Item2["waypoints of Route"] = ConvertPointsToString(routePoints);
                pointTodragTuple.Item2["Total Distance"] = Math.Round(DataRead.GetTotalDistance(routePoints) * mTonm, 3)
                    .ToString(CultureInfo.InvariantCulture) + " nm";

                _routeDragStarted = true;
                S57Control1.Invalidate();
            }
            S57Control1.Invalidate();
        }

        private void S57Control1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                clickPixelPoint = new PixelPoint
                {
                    X = e.X,
                    Y = e.Y
                };
                _clickGeoPt = m_draw.GeoPix.GeoPointFromPixelPoint(clickPixelPoint);

                if ((_currentChartMode == ChartMode.None || _currentChartMode == ChartMode.Drag) && !_routeDragStarted)
                {
                    _markGeoPoint = _clickGeoPt;
                }

                clickGeoCordinate = new GeoCoordinate
                {
                    Latitude = _clickGeoPt.Lat,
                    Longitude = _clickGeoPt.Lon
                };

                labelControl1.Text = _clickGeoPt.FormattedPosition.ToString(CultureInfo.InvariantCulture);

                if (aisRectangle.Contains((int)clickPixelPoint.X, (int)clickPixelPoint.Y))
                {
                    CreateAiSmenu();
                    popupMenu1.ShowPopup(PointToScreen(e.Location));
                    S57Control1.Invalidate();
                }
                else
                {
                    popupMenu1.HidePopup();
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    //for getting information at particular point
                    var isPointHeaderAdded = false;
                    var isRoutePointHeaderAdded = false;
                    var isRouteLineHeaderAdded = false;
                    var testData = TestPoint(clickPixelPoint);

                    foreach (var pointInfo in testData)
                    {
                        switch (pointInfo.Item1)
                        {
                            case pointType.waypoint:
                                if (!isPointHeaderAdded)
                                {
                                    popupMenu1.ClearLinks();
                                    AddHeaderItem(@"Waypoint(s)");
                                    CreateWaypointSubMenu(pointInfo.Item2);
                                    isPointHeaderAdded = true;
                                }
                                break;
                            case pointType.RouteLine:
                                if (!isRouteLineHeaderAdded)
                                {
                                    popupMenu1.ClearLinks();
                                    AddHeaderItem(@"Route(s)");
                                    CreateRouteLineSubmenu(pointInfo.Item2);
                                    isRouteLineHeaderAdded = true;
                                }
                                break;
                            case pointType.RouteWayPoint:
                                if (!isRoutePointHeaderAdded)
                                {
                                    popupMenu1.ClearLinks();
                                    AddHeaderItem(@"Route Point(s)");
                                    CreateRoutePointSubmenu(pointInfo);
                                    isRoutePointHeaderAdded = true;
                                }
                                break;
                        }
                    }
                    if (isPointHeaderAdded)
                    {
                        Point screenPoint = new Point((int)clickPixelPoint.X, (int)clickPixelPoint.Y);
                        popupMenu1.ShowPopup(PointToScreen(screenPoint));
                        isPointHeaderAdded = false;
                    }
                    else if (isRouteLineHeaderAdded)
                    {
                        Point screenPoint = new Point((int)clickPixelPoint.X, (int)clickPixelPoint.Y);
                        popupMenu1.ShowPopup(PointToScreen(screenPoint));
                        isRouteLineHeaderAdded = false;
                    }
                    else if (isRoutePointHeaderAdded)
                    {
                        Point screenPoint = new Point((int)clickPixelPoint.X, (int)clickPixelPoint.Y);
                        popupMenu1.ShowPopup(PointToScreen(screenPoint));
                        isRoutePointHeaderAdded = false;
                    }
                    else
                    {
                        popupMenu2.ShowPopup(PointToScreen(e.Location));
                    }
                    S57Control1.Invalidate();
                }
                catch { }
            }

            if (_currentChartMode == ChartMode.Route)
            {
                _routeWayPointsList.Add(new GeoCoordinate(_clickGeoPt.Lat, _clickGeoPt.Lon));

                spintxtedit_usvspeed.Text = @"6"; //
                trueTotalDistance = DataRead.GetTotalDistance(_routeWayPointsList);
                txtedit_Totaldistance.Text = Math.Round((trueTotalDistance * mTonm), 3).ToString(CultureInfo.InvariantCulture); //convert meters to nautical miles.
                txtedit_Timetake.Text = Math.Round((trueTotalDistance / (6 * knotsTomps) * secTohr), 3).ToString(CultureInfo.InvariantCulture);
            }

            if (_currentChartMode == ChartMode.Measure)
            {
                _measuredList.Add(new GeoCoordinate(_clickGeoPt.Lat, _clickGeoPt.Lon));
            }
            S57Control1.Invalidate();
        }

        private string ConvertPointsToString(IEnumerable<GeoCoordinate> pointList)
        {
            var points = new StringBuilder();
            foreach (var point in pointList) points.Append(point + ":");
            return points.ToString();
        }

        private void CreateAiSmenu()
        {
            popupMenu1.ClearLinks();
            var subItem = new BarSubItem(barManager1, AIS_Read.AIS_Read._aisName)
            {
                Glyph = Resources.next_16x16
            };

            var menuTitle1 = _aisShowtrailbuttonClicked ? "Hide Trail" : "Show Trail";
            var aisBarButtonItem1 = new BarButtonItem(barManager1, menuTitle1, -1)
            {
                Glyph = Resources.existlink_16x16,
                Tag = menuTitle1
            };
            aisBarButtonItem1.ItemClick += aisBarButtonItem1_ItemClick;

            var menuTitle2 = _aisShowrangeBarbuttonClicked ? "Hide RangeBar" : "Show RangeBar";
            var aisBarButtonItem2 = new BarButtonItem(barManager1, menuTitle2, -1)
            {
                Glyph = Resources.clear_16x16,
                Tag = menuTitle2
            };
            aisBarButtonItem2.ItemClick += aisBarButtonItem2_ItemClick;

            subItem.AddItem(aisBarButtonItem1);
            subItem.AddItem(aisBarButtonItem2);
            popupMenu1.AddItem(subItem);
        }

        private void aisBarButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            _aisShowrangeBarbuttonClicked = (string)e.Item.Tag == "Hide RangeBar" ? false : true;
        }

        private void aisBarButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            _aisShowtrailbuttonClicked = (string)e.Item.Tag == "Hide Trail" ? false : true;
        }

        private List<Tuple<pointType, DataRow, int>> TestPoint(PixelPoint testpixpt)
        {
            var testList = new List<Tuple<pointType, DataRow, int>>();

            //check for waypoints
            foreach (DataRow dataRow in _wayPointData.Rows)
            {
                var selectedRowPoint = DataRead.GetGeoPointFromDataRow(dataRow);

                var pointToTest = new GeoPoint
                {
                    Lat = Convert.ToDouble(selectedRowPoint.Lat),
                    Lon = Convert.ToDouble(selectedRowPoint.Lon)
                };

                var pixelToTest = m_draw.GeoPix.Point(pointToTest);
                if (m_draw.GeoPix.PointInView)
                {
                    if (Math.Sqrt((pixelToTest.X - testpixpt.X) * (pixelToTest.X - testpixpt.X) +
                                  (pixelToTest.Y - testpixpt.Y) * (pixelToTest.Y - testpixpt.Y)) < 12)
                    {
                        testList.Add(new Tuple<pointType, DataRow, int>(pointType.waypoint, dataRow, 0));
                        return testList;
                    }
                }
            }

            //check for route waypoints
            var displayedRoutesRow = (from x in _routePointsData.AsEnumerable()
                                      where x.Field<bool>("Show Route")
                                      select x).ToList();

            foreach (var testrouteInfo in displayedRoutesRow)
            {
                var routeList = GetPointsFromString(Convert.ToString(testrouteInfo["waypoints of Route"]));
                for (int i = 0; i < routeList.Count; i++)
                {
                    var pointToTest = new GeoPoint
                    {
                        Lat = Convert.ToDouble(routeList[i].Latitude),
                        Lon = Convert.ToDouble(routeList[i].Longitude)
                    };

                    var pixelToTest = m_draw.GeoPix.Point(pointToTest);
                    if (m_draw.GeoPix.PointInView)
                    {
                        if (Math.Sqrt((pixelToTest.X - testpixpt.X) * (pixelToTest.X - testpixpt.X) +
                                      (pixelToTest.Y - testpixpt.Y) * (pixelToTest.Y - testpixpt.Y)) < 9)
                        {
                            simpleLabelItem8.Text = string.Empty;
                            testList.Add(new Tuple<pointType, DataRow, int>(pointType.RouteWayPoint, testrouteInfo, i));
                            for (int j = i; j < routeList.Count; j++)
                            {
                                simpleLabelItem8.Text += (Environment.NewLine + (j + 1) + @")" + routeList[j] + @";" + Environment.NewLine);
                            }
                            return testList;
                        }
                    }
                }
            }

            //check for route lines
            foreach (var testrouteInfo in displayedRoutesRow)
            {
                var routeList = GetPointsFromString(Convert.ToString(testrouteInfo["waypoints of Route"]));
                for (int i = 1; i < routeList.Count; i++)
                {
                    var gpt1 = new GeoPoint
                    {
                        Lat = Convert.ToDouble(routeList[i - 1].Latitude),
                        Lon = Convert.ToDouble(routeList[i - 1].Longitude)
                    };
                    var pixelToTest1 = m_draw.GeoPix.Point(gpt1);
                    var gpt2 = new GeoPoint
                    {
                        Lat = Convert.ToDouble(routeList[i].Latitude),
                        Lon = Convert.ToDouble(routeList[i].Longitude)
                    };
                    var pixelToTest2 = m_draw.GeoPix.Point(gpt2);
                    //var line = new GeoLine
                    //{
                    //    Point1 = gpt1,
                    //    Point2 = gpt2
                    //};
                    var perpdist = CalculatePerpendicularDist(pixelToTest1, pixelToTest2, testpixpt);
                    footfall = false;
                    footfall = Checkforfootfall(pixelToTest1, pixelToTest2, testpixpt);
                    if (perpdist < 3 && footfall)
                    {
                        startpointfoundline = i;
                        simpleLabelItem8.Text = string.Empty;
                        testList.Add(new Tuple<pointType, DataRow, int>(pointType.RouteLine, testrouteInfo, i));
                        for (int j = 0; j < routeList.Count; j++)
                        {
                            simpleLabelItem8.Text += (Environment.NewLine + (j + 1) + @")" + routeList[j] + @";" + Environment.NewLine);
                        }
                        return testList;
                    }
                }
            }
            return testList;
        }

        private bool Checkforfootfall(PixelPoint pixp1, PixelPoint pixp2, PixelPoint testpixpt)
        {
            var a = testpixpt.X - pixp1.X;
            var b = pixp2.X - pixp1.X;
            var c = testpixpt.Y - pixp1.Y;
            var d = pixp2.Y - pixp1.Y;

            var t = ((a * b) + (c * d)) / ((b * b) + (d * d));
            if (t > 0 && t < 1)
            {
                return true;
            }
            return false;
        }

        private double CalculatePerpendicularDist(PixelPoint pixp1, PixelPoint pixp2, PixelPoint testpixpt)
        {
            var a = (pixp2.Y - pixp1.Y) / (pixp2.X - pixp1.X);
            var b = -1;
            var c = (pixp1.Y) - (pixp1.X * a);

            var perpdist = Math.Round(Math.Abs(a * testpixpt.X + b * testpixpt.Y + c) / Math.Sqrt(a * a + b * b), 2);

            return perpdist;
        }

        private void CreateWaypointSubMenu(DataRow wayPointRow)
        {
            var subItem = new BarSubItem(barManager1, Convert.ToString(wayPointRow["WayPointName"]))
            {
                Glyph = Resources.geopoint_16x16
            };

            var waypointBarButtonItem1 = new BarButtonItem(barManager1, "Show Information", -1)
            {
                Glyph = Resources.index_16x16,
                Tag = wayPointRow
            };
            waypointBarButtonItem1.ItemClick += waypointBarButtonItem1_ItemClick;

            var waypointBarButtonItem2 = new BarButtonItem(barManager1, "Delete", -1)
            {
                Glyph = Resources.delete_16x16,
                Tag = wayPointRow
            };
            waypointBarButtonItem2.ItemClick += waypointBarButtonItem2_ItemClick;

            subItem.AddItem(waypointBarButtonItem1);
            subItem.AddItem(waypointBarButtonItem2);
            popupMenu1.AddItem(subItem);
        }

        private void waypointBarButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            cardView1.ClearSelection();
            windowsUIButtonPanel10.Buttons["WayPoints"].Properties.Checked = true;
            windowsUIButtonPanel1.Buttons[0].Properties.Checked = true;

            var rowHandle = cardView1.GetRowHandle(_wayPointData.Rows.IndexOf((DataRow)e.Item.Tag));
            cardView1.SelectRow(rowHandle);
            cardView1.FocusedRowHandle = rowHandle;
        }

        private void waypointBarButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            cardView1.ClearSelection();
            cardView1.SelectRow(cardView1.GetRowHandle(_wayPointData.Rows.IndexOf((DataRow)e.Item.Tag)));
            DeleteWaypoint();
        }

        private void CreateRouteLineSubmenu(DataRow routeRow)
        {
            var subItem = new BarSubItem(barManager1, Convert.ToString(routeRow["RouteName"]))
            {
                Glyph = Resources.line2_16x16
            };

            var routeBarButtonItem1 = new BarButtonItem(barManager1, "Execute Route", -1)
            {
                Glyph = Resources.stepline_16x16,
                Tag = routeRow
            };

            var routeBarButtonItem2 = new BarButtonItem(barManager1, "Edit Route", -1)
            {
                Glyph = Resources.edittask_16x16,
                Tag = routeRow
            };
            routeBarButtonItem2.ItemClick += routeBarButtonItem2_ItemClick;

            var routeBarButtonItem3 = new BarButtonItem(barManager1, "Show Information", -1)
            {
                Glyph = Resources.index_16x16,
                Tag = routeRow
            };
            //routeBarButtonItem1.ItemClick += waypointBarButtonItem1_ItemClick;

            var routeBarButtonItem4 = new BarButtonItem(barManager1, "Delete", -1)
            {
                Glyph = Resources.delete_16x16,
                Tag = routeRow
            };
            routeBarButtonItem4.ItemClick += routeBarButtonItem4_ItemClick;

            var routeBarButtonItem5 = new BarButtonItem(barManager1, "Edit Route Completed", -1)
            {
                Glyph = Resources.apply_16x16,
                Tag = routeRow
            };
            routeBarButtonItem5.ItemClick += routeBarButtonItem5_ItemClick;

            var routeBarButtonItem6 = new BarButtonItem(barManager1, "Insert WayPoint", -1)
            {
                Glyph = Resources.piebubble_16x16,
                Tag = routeRow
            };
            routeBarButtonItem6.ItemClick += routeBarButtonItem6_ItemClick;

            var routeBarButtonItem7 = new BarButtonItem(barManager1, "Extend Route", -1)
            {
                Glyph = Resources.newfeed_16x16,
                Tag = routeRow
            };
            routeBarButtonItem7.ItemClick += routeBarButtonItem7_ItemClick;

            if (!_routeedit)
            {
                subItem.AddItem(routeBarButtonItem1);
                subItem.AddItem(routeBarButtonItem2);
                subItem.AddItem(routeBarButtonItem3);
                subItem.AddItem(routeBarButtonItem4);
            }
            else if (_routeedit)
            {
                subItem.AddItem(routeBarButtonItem1);
                subItem.AddItem(routeBarButtonItem5);
                subItem.AddItem(routeBarButtonItem6);
                subItem.AddItem(routeBarButtonItem7);
                subItem.AddItem(routeBarButtonItem3);
                subItem.AddItem(routeBarButtonItem4);
            }
            popupMenu1.AddItem(subItem);
        }

        private void routeBarButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            _routeWayPointsList.Clear();
            _routeedit = true;
            _dragStartGeoPoint = null;
        }

        private void routeBarButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            cardView2.ClearSelection();
            cardView2.SelectRow(cardView2.GetRowHandle(_routePointsData.Rows.IndexOf((DataRow)e.Item.Tag)));
            DeleteRoute();
        }

        private void routeBarButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            _routeWayPointsList.Clear();
            _routeedit = false;
            SaveRoutestextfile();
        }

        private void routeBarButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            _routeWayPointsList.Clear();

            if (e.Item.Tag is DataRow routeRow)
            {
                var routePoints = GetPointsFromString(Convert.ToString(routeRow["waypoints of Route"]));
                routePoints.Insert(startpointfoundline, new GeoCoordinate(_markGeoPoint.Lat, _markGeoPoint.Lon));
                routeRow["waypoints of Route"] = ConvertPointsToString(routePoints);
                routeRow["Total Distance"] = Math.Round(DataRead.GetTotalDistance(routePoints), 1).ToString(CultureInfo.InvariantCulture);
                gridControl2.RefreshDataSource();
                S57Control1.Invalidate();
                SaveRoutestextfile();
            }
        }

        private void routeBarButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            //DataRow routeRow = (DataRow)e.Item.Tag;
            if (e.Item.Tag is DataRow routeRow)
            {
                routeRow["Show Route"] = false;
                //groupControl3.Show();

                // Put it on route List
                _routeWayPointsList.Clear();
                _routeWayPointsList = GetPointsFromString(Convert.ToString(routeRow["waypoints of Route"]));
                _editRouteIndex = Convert.ToInt32(routeRow["Index"]);
                _currentChartMode = ChartMode.Route;

                txtedit_routeName.Text = routeRow["RouteName"].ToString();
                colorPickedit_routeColour.Text = routeRow["Colour"].ToString();
            }
        }

        private void CreateRoutePointSubmenu(Tuple<pointType, DataRow, int> routePointInfo)
        {
            var subItem = new BarSubItem(barManager1, Convert.ToString(routePointInfo.Item2["RouteName"]) + " :Point " + (routePointInfo.Item3 + 1))
            {
                Glyph = Resources.mapit_16x16
            };

            var routeBarButtonItem1 = new BarButtonItem(barManager1, "Execute from here", -1)
            {
                Glyph = Resources.drilldown_16x16,
                Tag = routePointInfo
            };
            //routeBarButtonItem2.ItemClick += waypointBarButtonItem2_ItemClick;

            var routeBarButtonItem2 = new BarButtonItem(barManager1, "Delete from Route", -1)
            {
                Glyph = Resources.delete_16x16,
                Tag = routePointInfo
            };
            routeBarButtonItem2.ItemClick += routepointBarButtonItem2_ItemClick;

            if (!_routeedit)
            {
                subItem.AddItem(routeBarButtonItem1);
            }
            else
            {
                subItem.AddItem(routeBarButtonItem2);
            }
            popupMenu1.AddItem(subItem);
        }

        private void routepointBarButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            _routeWayPointsList.Clear();
            var routeRow = ((Tuple<pointType, DataRow, int>)e.Item.Tag).Item2;
            var routeWaypointIndex = ((Tuple<pointType, DataRow, int>)e.Item.Tag).Item3;
            var routePoints = GetPointsFromString(Convert.ToString(routeRow["waypoints of Route"]));
            routePoints.RemoveAt(routeWaypointIndex);
            routeRow["waypoints of Route"] = ConvertPointsToString(routePoints);
            routeRow["Total Distance"] =
                Math.Round(DataRead.GetTotalDistance(routePoints), 1).ToString(CultureInfo.InvariantCulture);
            gridControl2.RefreshDataSource();
            S57Control1.Invalidate();
            SaveRoutestextfile();
        }

        private void AddHeaderItem(string caption)
        {
            var headerItem = new BarHeaderItem { Caption = caption };
            popupMenu1.AddItem(headerItem);
        }

        private void FormEnc_Load(object sender, EventArgs e)
        {
            S57Control.PaintOverlaysEvent += S57Control_PaintOverlaysEvent;
            UpdateTimer.Start();
        }

        private void DrawRectBoxes(Tuple<GeoCoordinate, double> boxData, Graphics g, Rectangle r)
        {
            if (boxData.Item2 >= 0 && boxData.Item2 < 90)
            {
                g.DrawRectangle(Pens.Black, r.X - 3, r.Y, 3, 10);
            }
            else if (boxData.Item2 >= 90 && boxData.Item2 < 180)
            {
                g.DrawRectangle(Pens.Black, r.X, r.Y + 7, 3, 10);
            }
            else if (boxData.Item2 >= 180 && boxData.Item2 <= 270)
            {
                g.DrawRectangle(Pens.Black, r.X + 7, r.Y, 3, 10);
            }
            else if (boxData.Item2 > 270 && boxData.Item2 < 360)
            {
                g.DrawRectangle(Pens.Black, r.X, r.Y, 3, 10);
            }
        }

        private void S57Control_PaintOverlaysEvent(object sender, Graphics gr)
        {
            switch (_currentChartMode)
            {
                case ChartMode.Drag:
                    if (_routeWayPointsList.Count > 0)
                        DrawAllPath(gr, _routeWayPointsList, _goalPointimage,
                            new SolidBrush(Color.Black));
                    break;

                case ChartMode.Route:
                    DrawRoutePath(gr, _routeWayPointsList, _goalPointimage);
                    break;

                case ChartMode.Measure:
                    DrawRoutePath(gr, _measuredList, _measureIcon);
                    break;
            }

            //Draw Mark
            if (_currentChartMode == ChartMode.None || _currentChartMode == ChartMode.Drag)
            {
                var curClickedPixelpt = m_draw.GeoPix.Point(_markGeoPoint);
                var markRectangle = new Rectangle(Convert.ToInt32(curClickedPixelpt.X - 8),
                    Convert.ToInt32(curClickedPixelpt.Y - 8), 16, 16);
                gr.DrawImage(_mark, markRectangle);
            }

            // Draw home position
            if (HomeLocation != null)
            {
                var homePixelPoint = m_draw.GeoPix.Point(new GeoPoint
                { Lat = HomeLocation.Latitude, Lon = HomeLocation.Longitude });
                if (m_draw.GeoPix.PointInView)
                {
                    var homeRectangle =
                        new Rectangle(Convert.ToInt32(homePixelPoint.X - 14),
                            Convert.ToInt32(homePixelPoint.Y - 14), 30, 30);
                    gr.DrawImage(_homeImage, homeRectangle);
                }
            }

            // Draw Anchor position
            if (_anchorGeoCoordinate != null)
            {
                var anchorPixelPoint = m_draw.GeoPix.Point(new GeoPoint
                { Lat = _anchorGeoCoordinate.Latitude, Lon = _anchorGeoCoordinate.Longitude });
                if (m_draw.GeoPix.PointInView)
                {
                    var anchorRectangle =
                        new Rectangle(Convert.ToInt32(anchorPixelPoint.X - 14),
                            Convert.ToInt32(anchorPixelPoint.Y - 14), 30, 30);
                    gr.DrawImage(_anchorImage, anchorRectangle);
                }
            }

            // Draw Waypoints
            foreach (DataRow dataRow in _wayPointData.Rows)
            {
                var selectedRowPoint = DataRead.GetGeoPointFromDataRow(dataRow);
                {
                    var wayPoint = new GeoPoint
                    {
                        Lat = selectedRowPoint.Lat,
                        Lon = selectedRowPoint.Lon
                    };

                    var waypointPixel = m_draw.GeoPix.Point(wayPoint);
                    if (m_draw.GeoPix.PointInView)
                    {
                        var geoRectangle = new Rectangle(Convert.ToInt32(waypointPixel.X - 8),
                            Convert.ToInt32(waypointPixel.Y - 8), 16, 16);
                        gr.DrawImage(_waypointImage, geoRectangle);
                        var distFont = new Font("Calibri", 11, FontStyle.Bold);
                        gr.DrawString(dataRow["WayPointName"].ToString(), distFont, Brushes.Black,
                            waypointPixel.X,
                            waypointPixel.Y + 5);
                    }
                }
            }

            //Draw Routes
            var displayedRoutesRow = (from x in _routePointsData.AsEnumerable()
                                      where x.Field<bool>("Show Route")
                                      select x).ToList();

            foreach (var routeInfo in displayedRoutesRow)
            {
                var routeList = GetPointsFromString(Convert.ToString(routeInfo["waypoints of Route"]));

                DrawAllPath(gr, routeList, _goalPointimage,
                    new SolidBrush(Color.FromArgb(Convert.ToInt32(routeInfo["Colour"]))));
            }

            // Initialize usvTrail if not already done
            if (usvTrail == null)
            {
                usvTrail = new List<Tuple<GeoCoordinate, double>>();
            }

            // DrawAIS
            if (windowsUIButtonPanel4.Buttons["AIS"].Properties.Checked) DrawAis(gr);

            // USV Position
            if (DataRead._USVGeoCoordinate != null)
            {
                usvGeoPoint = new GeoPoint
                {
                    Lat = DataRead._USVGeoCoordinate.Latitude,
                    Lon = DataRead._USVGeoCoordinate.Longitude
                };

                usvPixelPoint = m_draw.GeoPix.Point(usvGeoPoint);

                if (m_draw.GeoPix.PointInView || !m_draw.GeoPix.PointInView)
                {
                    var currentusvGeocordinate = new GeoCoordinate(DataRead._USVGeoCoordinate.Latitude, DataRead._USVGeoCoordinate.Longitude);

                    if (usvTrail.Count > 0)
                    {
                        lastPosition = usvTrail[usvTrail.Count - 1].Item1;
                        lastPositionAngle = usvTrail[usvTrail.Count - 1].Item2;
                        float distance = DataRead.DistanceBetweenPoints(lastPosition, currentusvGeocordinate);
                        if (distance >= 10)
                        {
                            usvTrail.Add(new Tuple<GeoCoordinate, double>(currentusvGeocordinate, DataRead.USV_Angle));
                        }
                    }
                    else
                    {
                        usvTrail.Add(new Tuple<GeoCoordinate, double>(currentusvGeocordinate, DataRead.USV_Angle));
                    }

                    if (DataRead._USVGeoCoordinate != null)
                    {
                        //Draw the USV at the current position
                        usvRectangle = new Rectangle(Convert.ToInt32(usvPixelPoint.X - 14),
                            Convert.ToInt32(usvPixelPoint.Y - 14), 30, 30);
                    }

                    //Draw Circle around USV
                    Color transparentOrange = Color.FromArgb(50, Color.DarkOrange);
                    var usvPixelPerMeters = m_draw.PixelSizeMetres * m_draw.GeoPix.DisplayScale;
                    int radius = (int)(600 / (2 * Math.PI) / usvPixelPerMeters);

                    using (Brush brush = new SolidBrush(transparentOrange))
                    {
                        gr.FillEllipse(brush, usvPixelPoint.X - radius, usvPixelPoint.Y - radius, radius * 2, radius * 2);
                    }

                    Font distFont = new Font("Calibri", 11, FontStyle.Bold);
                    Brush textBrush = Brushes.Black;

                    PointF topPoint = new PointF(usvPixelPoint.X - 30, usvPixelPoint.Y - radius - 15);
                    gr.DrawString("Safe Zone", distFont, textBrush, topPoint);

                    if (_rotatechartbuttonChecked)
                    {
                        m_draw.SetDirectionOfUp(DataRead.USV_Angle, _defaultPixelPoint);

                        //Draw USVImage
                        Bitmap rotatedImage = RotateImage(_usvImage, 0);
                        gr.DrawImage(rotatedImage, usvRectangle);

                        gr.SmoothingMode = SmoothingMode.AntiAlias;

                        // Draw trail
                        foreach (var point in usvTrail)
                        {
                            var trailPixelPoint = m_draw.GeoPix.Point(new GeoPoint
                            {
                                Lat = point.Item1.Latitude,
                                Lon = point.Item1.Longitude
                            });

                            trailRectangle = new Rectangle(Convert.ToInt32(trailPixelPoint.X), Convert.ToInt32(trailPixelPoint.Y), 10, 10);
                            RotateRectangle(gr, trailRectangle, point.Item2 - DataRead.USV_Angle);
                            DrawRectBoxes(point, gr, trailRectangle);
                        }
                        gr.SmoothingMode = SmoothingMode.Default;
                    }
                    else if (!_rotatechartbuttonChecked)
                    {
                        m_draw.SetDirectionOfUp(0, _defaultPixelPoint);

                        //Draw USVImage
                        Bitmap rotatedImage = RotateImage(_usvImage, DataRead.USV_Angle);
                        gr.DrawImage(rotatedImage, usvRectangle);

                        gr.SmoothingMode = SmoothingMode.AntiAlias;

                        // Draw trail
                        foreach (var point in usvTrail)
                        {
                            var trailPixelPoint = m_draw.GeoPix.Point(new GeoPoint
                            {
                                Lat = point.Item1.Latitude,
                                Lon = point.Item1.Longitude
                            });

                            trailRectangle = new Rectangle(Convert.ToInt32(trailPixelPoint.X), Convert.ToInt32(trailPixelPoint.Y), 10, 10);
                            RotateRectangle(gr, trailRectangle, point.Item2);
                            DrawRectBoxes(point, gr, trailRectangle);
                        }
                        gr.SmoothingMode = SmoothingMode.Default;
                    }

                    // Draw RangeBar
                    //var futureDistance = DataRead.USV_SOG * 1.852 * 1 * 0.01666667*5; // distance in nautical miles for 1 minute
                    //var futurePoint = DataRead.FindPointAtDistanceFrom(
                    //    DataRead._USVGeoCoordinate,
                    //    DataRead.ToRad(DataRead.USV_Angle),
                    //    futureDistance);

                    //var futureDistancePixelPoint = m_draw.GeoPix.Point(new GeoPoint { Lat = futurePoint.Latitude, Lon = futurePoint.Longitude });
                    //var futureGeopoint = m_draw.GeoPix.GeoPointFromPixelPoint(futureDistancePixelPoint);

                    //gr.SmoothingMode = SmoothingMode.AntiAlias;
                    //using (var rangeBarPen = new Pen(Color.Black, 3))
                    //{
                    //    gr.DrawLine(rangeBarPen, (int)usvGeoPoint.Lat, (int)usvGeoPoint.Lon, (int)futureGeopoint.Lat, (int)futureGeopoint.Lon);
                    //}
                    //gr.SmoothingMode = SmoothingMode.Default;
                }
            }
            S57Control1.Invalidate();
        }

        private void DrawAis(Graphics gr)
        {
            //AIS Position
            if (aisTrail == null)  //Initialize usvTrail if not already done
            {
                aisTrail = new List<Tuple<GeoCoordinate, double>>();
            }

            var vesselString = AIS_Read.AIS_Read._aisName;

            if (AIS_Read.AIS_Read._AISGeoCoordinate != null)
            {
                var aisGeoPoint = new GeoPoint
                {
                    Lat = AIS_Read.AIS_Read._AISGeoCoordinate.Latitude,
                    Lon = AIS_Read.AIS_Read._AISGeoCoordinate.Longitude
                };

                aisPixelPoint = m_draw.GeoPix.Point(aisGeoPoint);

                if (m_draw.GeoPix.PointInView || !m_draw.GeoPix.PointInView)
                {
                    var currentaisGeocordinate = new GeoCoordinate(AIS_Read.AIS_Read._AISGeoCoordinate.Latitude,
                        AIS_Read.AIS_Read._AISGeoCoordinate.Longitude);

                    if (aisTrail.Count > 0)
                    {
                        lastaisPosition = aisTrail[aisTrail.Count - 1].Item1;
                        lastaisPositionAngle = aisTrail[aisTrail.Count - 1].Item2;
                        float aisdistance = DataRead.DistanceBetweenPoints(lastaisPosition, currentaisGeocordinate);
                        if (aisdistance >= 10)
                        {
                            aisTrail.Add(new Tuple<GeoCoordinate, double>(currentaisGeocordinate, AIS_Read.AIS_Read.AIS_Angle));
                        }
                    }
                    else
                    {
                        aisTrail.Add(new Tuple<GeoCoordinate, double>(currentaisGeocordinate, AIS_Read.AIS_Read.AIS_Angle));
                    }

                    // put a lock to prevent the multiple threads access to rotate image
                    lock (_targetImageLocker)
                    {
                        if (AIS_Read.AIS_Read._AISGeoCoordinate != null)
                        {
                            //Draw the USV at the current position
                            aisRectangle = new Rectangle(Convert.ToInt32(aisPixelPoint.X - 14),
                                Convert.ToInt32(aisPixelPoint.Y - 14), 30, 30);
                        }

                        //Draw AIS Image
                        var ab = new PointF();
                        var a = m_draw.GeoPix.Point(aisGeoPoint);
                        ab.X = a.X;
                        ab.Y = a.Y - 25;

                        var aisTousvDist = DataRead.DistanceBetweenPoints(DataRead._USVGeoCoordinate,
                            AIS_Read.AIS_Read._AISGeoCoordinate);
                        if (aisTousvDist <= 200)
                        {
                            Bitmap rotatedredAisImage = RotateImage(_redaisImage, AIS_Read.AIS_Read.AIS_Angle);
                            if (DateTime.Now.Second % 2 == 0)
                            {
                                gr.DrawImage(rotatedredAisImage, aisRectangle);
                            }
                        }
                        else if (aisTousvDist > 200 && aisTousvDist <= 300)
                        {
                            Bitmap rotatedorangeAisImage = RotateImage(_orangeaisImage, AIS_Read.AIS_Read.AIS_Angle);
                            if (DateTime.Now.Second % 2 == 0)
                            {
                                gr.DrawImage(rotatedorangeAisImage, aisRectangle);
                            }
                        }
                        else if (aisTousvDist > 300 && aisTousvDist <= 500)
                        {
                            Bitmap rotatedgreenAisImage = RotateImage(_greenaisImage, AIS_Read.AIS_Read.AIS_Angle);
                            if (DateTime.Now.Second % 2 == 0)
                            {
                                gr.DrawImage(rotatedgreenAisImage, aisRectangle);
                            }
                        }
                        else
                        {
                            Bitmap rotatedAisImage = RotateImage(_aisImage, AIS_Read.AIS_Read.AIS_Angle);
                            gr.DrawImage(rotatedAisImage, aisRectangle);
                        }
                        gr.DrawString(vesselString, new Font("Calibri", 11, FontStyle.Bold), Brushes.Black,
                            ab);

                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                    }

                    if (_aisShowtrailbuttonClicked)
                    {
                        // Draw AIS trail
                        foreach (var aispoint in aisTrail)
                        {
                            var trailaisGeoPoint = new GeoPoint
                            {
                                Lat = aispoint.Item1.Latitude,
                                Lon = aispoint.Item1.Longitude
                            };

                            var trailaisPixelPoint = m_draw.GeoPix.Point(trailaisGeoPoint);

                            trailaisRectangle = new Rectangle(Convert.ToInt32(trailaisPixelPoint.X),
                                Convert.ToInt32(trailaisPixelPoint.Y), 10, 10);
                            RotateRectangle(gr, trailaisRectangle, aispoint.Item2);
                            DrawRectBoxes(aispoint, gr, trailaisRectangle);
                        }
                    }
                    gr.SmoothingMode = SmoothingMode.Default;
                }
            }
        }

        private List<GeoCoordinate> GetPointsFromString(string points)
        {
            var selectedRoutePoints = points
                .Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            var selectedRoutePointsList = selectedRoutePoints.Select(stringPoint => stringPoint.Split(','))
                .Select(pointCoordinates => new GeoCoordinate
                {
                    Latitude = Convert.ToDouble(pointCoordinates[0]),
                    Longitude = Convert.ToDouble(pointCoordinates[1])
                }).ToList();
            return selectedRoutePointsList;
        }

        private void DrawRoutePath(Graphics gr, List<GeoCoordinate> routeWayPointsList, Bitmap image)
        {
            if (routeWayPointsList.Count == 1)
            {
                DrawLine(gr, routeWayPointsList.First(), _cursorGeocoordinate, image);
                var firstPoint = new GeoPoint
                {
                    Lat = routeWayPointsList.First().Latitude,
                    Lon = routeWayPointsList.First().Longitude
                };
                var firstPointPixel = m_draw.GeoPix.Point(firstPoint);
                var rect = new Rectangle((int)firstPointPixel.X - 8, (int)firstPointPixel.Y - 8, 16, 16);
                gr.DrawImage(image, rect);
            }
            if (routeWayPointsList.Count > 1)
            {
                //var cirCenterPt = new GeoPoint
                //{
                //    Lat = routeWayPointsList.Last().Latitude,
                //    Lon = routeWayPointsList.Last().Longitude
                //};
                //var cirCenterPixel = m_draw.GeoPix.Point(cirCenterPt);
                //var lastdistance = routeWayPointsList.Last().GetDistanceTo(_cursorGeocoordinate);
                //var usvpixelpermeters = m_draw.PixelSizeMetres * m_draw.GeoPix.DisplayScale;
                //var d = lastdistance / usvpixelpermeters;

                //using (var p = new System.Drawing.Pen(System.Drawing.Color.Black))
                //{
                //    gr.DrawEllipse(p, cirCenterPixel.X - (float)d, cirCenterPixel.Y - (float)d,
                //        (float)d + (float)d, (float)d + (float)d);
                //}

                DrawAllPath(gr, routeWayPointsList, image, Brushes.Black);
                DrawLine(gr, routeWayPointsList[routeWayPointsList.Count - 1], _cursorGeocoordinate, image);
            }
        }

        //For continuosly drawing route
        private void DrawAllPath(Graphics gr, List<GeoCoordinate> routeWayPointsList, Bitmap image, Brush brush)
        {
            var _mTotalMeasuredDistance = 0.0;
            for (var i = 1; i < routeWayPointsList.Count; i++)
            {
                var measureGeoPoint1 = new GeoPoint
                {
                    Lat = routeWayPointsList[i - 1].Latitude,
                    Lon = routeWayPointsList[i - 1].Longitude
                };
                var measureGeoPoint2 = new GeoPoint
                {
                    Lat = routeWayPointsList[i].Latitude,
                    Lon = routeWayPointsList[i].Longitude
                };

                var line = new GeoLine
                {
                    Point1 = measureGeoPoint1,
                    Point2 = measureGeoPoint2
                };

                //converting line points to pixels
                var collection = m_draw.GeoPix.Line(line);

                foreach (PixelPoints pixelCollection in collection)
                {
                    //if (pixelCollection.Count < 2)
                    //    continue;
                    var ptf = new PointF[pixelCollection.Count];
                    var n = 0;
                    foreach (PixelPoint pixel in pixelCollection)
                    {
                        ptf[n++] = new PointF(pixel.X, pixel.Y);
                    }
                    var drawPen = new Pen(brush, 2);
                    gr.DrawLines(drawPen, ptf);
                }

                var goalpoints = m_draw.GeoPix.Point(measureGeoPoint1);
                var recta = new Rectangle((int)goalpoints.X - 8, (int)goalpoints.Y - 8, 16, 16);
                gr.DrawImage(image, recta);

                if (i == routeWayPointsList.Count - 1)
                {
                    goalpoints = m_draw.GeoPix.Point(measureGeoPoint2);
                    recta = new Rectangle((int)goalpoints.X - 8, (int)goalpoints.Y - 8, 16, 16);
                    gr.DrawImage(image, recta);
                }

                //drawing the string here
                var pt1 = new GeoCoordinate(measureGeoPoint1.Lat, measureGeoPoint1.Lon);
                var pt2 = new GeoCoordinate(measureGeoPoint2.Lat, measureGeoPoint2.Lon);

                var distance = Math.Round(pt1.GetDistanceTo(pt2), 3);

                var distFont = new Font("Calibri", 11, FontStyle.Bold);
                var ab = new PointF();
                var a = m_draw.GeoPix.Point(measureGeoPoint2);
                ab.X = a.X;
                ab.Y = a.Y - 25;

                var bearing =
                 DataRead.DegreeBearing(pt1.Latitude, pt1.Longitude, pt2.Latitude, pt2.Longitude);

                //_mTotalMeasuredDistance += distance;

                gr.DrawString(
                distance + "m"
                + " at " + Math.Round(bearing) + "°T", distFont,
                Brushes.Black, ab);

                if (i == routeWayPointsList.Count - 1) //continue;
                {
                    ab.X = a.X;
                    ab.Y = a.Y - 40;
                    gr.DrawString("Total Distance:" +
                     DataRead.GetTotalDistance(routeWayPointsList) + "m"
                     + " at " + Math.Round(bearing) + "°T", distFont,
                     Brushes.Black, ab);
                }

                // Reset the total distance
                _mTotalMeasuredDistance = 0.0;
            }
        }

        //for drawing line with image 
        private void DrawLine(Graphics gr, GeoCoordinate pt1, GeoCoordinate pt2, Bitmap image)
        {
            var lineEndPt = new GeoPoint { Lat = pt2.Latitude, Lon = pt2.Longitude };

            var line = new GeoLine
            {
                Point1 = lineEndPt,
                Point2 = new GeoPoint { Lat = pt1.Latitude, Lon = pt1.Longitude }
            };

            var iconpoints = m_draw.GeoPix.Point(lineEndPt);
            var rect = new Rectangle((int)iconpoints.X - 8, (int)iconpoints.Y - 8, 16, 16);
            gr.DrawImage(image, rect);

            //converting line points to pixels
            var collection = m_draw.GeoPix.Line(line);

            foreach (PixelPoints pixelCollection in collection)
            {
                //if (pixelCollection.Count < 1)
                //    continue;
                var ptf = new PointF[pixelCollection.Count];
                var n = 0;
                foreach (PixelPoint pixel in pixelCollection)
                {
                    ptf[n++] = new PointF(pixel.X, pixel.Y);
                }
                var drawPen = new Pen(Brushes.Black, 2);
                gr.DrawLines(drawPen, ptf);
            }

            var distance = Math.Round(pt1.GetDistanceTo(pt2), 3);

            var distFont = new Font("Calibri", 11, FontStyle.Bold);
            var ab = new PointF();
            var a = m_draw.GeoPix.Point(lineEndPt);
            ab.X = a.X;
            ab.Y = a.Y + 25;

            var bearing =
                DataRead.DegreeBearing(pt1.Latitude, pt1.Longitude, pt2.Latitude, pt2.Longitude);

            gr.DrawString(
                distance + "m"
                + " at " + Math.Round(bearing) + "°", distFont,
                Brushes.Black, ab);
        }

        //Rotate Rectangle
        public Rectangle RotateRectangle(Graphics g, Rectangle r, double angle)
        {
            using (Matrix m = new Matrix())
            {
                //Rotate around the center of the rectangle
                m.RotateAt((float)angle, new PointF(r.Left + (r.Width / 2f), r.Top + (r.Height / 2f)));
                g.Transform = m;
            }
            return r;
        }

        //Rotate image
        private Bitmap RotateImage(Bitmap bmp, double angle)
        {
            float height = bmp.Height;
            float width = bmp.Width;
            int hypotenuse = Convert.ToInt32(Math.Floor(Math.Sqrt(height * height + width * width)));
            Bitmap rotatedImage = new Bitmap(hypotenuse, hypotenuse);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform((float)rotatedImage.Width / 2, (float)rotatedImage.Height / 2); //set the rotation point as the center into the matrix
                g.RotateTransform((float)angle); //rotate
                g.TranslateTransform(-(float)rotatedImage.Width / 2, -(float)rotatedImage.Height / 2); //restore rotation point into the matrix
                g.DrawImage(bmp, (hypotenuse - width) / 2, (hypotenuse - height) / 2, width, height);
            }
            return rotatedImage;
        }

        //Focus USV
        private void windowsUIButtonPanel1_ButtonClick(object sender, ButtonEventArgs e)
        {
            GeoRect rect = new GeoRectClass();
            rect.LatN = DataRead._USVGeoCoordinate.Latitude;
            rect.LonW = DataRead._USVGeoCoordinate.Longitude;
            rect.LatS = DataRead._USVGeoCoordinate.Latitude;
            rect.LonE = DataRead._USVGeoCoordinate.Longitude;

            m_draw.SetPositionAndScale(rect); // Point the control at the sample data
        }

        private void Clearuserwaypoints()
        {
            wayptxt_Name.Text = string.Empty;
            wayptxt_Lat.Text = string.Empty;
            wayptxt_Lon.Text = string.Empty;
            dxErrorProvider1.ClearErrors();
        }

        private void barStaticItem21_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void windowsUIButtonPanel4_ButtonChecked(object sender, ButtonEventArgs e)
        {
            var buttonName = ((WindowsUIButton)e.Button).Caption;

            switch (buttonName)
            {
                case "D/N":
                    m_draw.DisplayPalette.PalType = PaletteType.PalNight;
                    break;
                case "Radar":

                    break;
                case "AIS":
                    break;
                case "N":
                    _rotatechartbuttonChecked = true;
                    break;
                case "Record":
                    var currentTime = DateTime.Now;
                    var filename = "VID-" + currentTime.ToLongDateString().Replace(' ', '-') + "-" +
                                   currentTime.ToLongTimeString().Replace(':', '-') + ".mp4";
                    _videoRecorder.Record(Path.Combine(_recordingPath, filename));
                    break;
            }
        }

        private void windowsUIButtonPanel4_ButtonUnchecked(object sender, ButtonEventArgs e)
        {
            var buttonName = ((WindowsUIButton)e.Button).Caption;

            switch (buttonName)
            {
                case "D/N":
                    m_draw.DisplayPalette.PalType = PaletteType.PalDay;
                    break;
                case "Radar":

                    break;
                case "AIS":
                    break;
                case "N":
                    _rotatechartbuttonChecked = false;
                    break;
                case "Record":
                    _videoRecorder.Stop();
                    break;
            }
        }

        private void cardView1_CustomCardCaptionImage(object sender, DevExpress.XtraGrid.Views.Card.CardCaptionImageEventArgs e)
        {
            e.Image = _waypointImage;
        }

        private void cardView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteWaypoint();
            }
        }

        private void VideoRecorder_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            barStaticItem20.Caption = @"Video recorded successfully";
        }

        private void SnapRecorder_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            barStaticItem20.Caption = @"Snap recorded successfully";
        }

        private void DeleteWaypoint()
        {
            var dialogResult = (XtraMessageBox.Show("Do you want to Delete the WayPoint(s)?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Hand));
            if (dialogResult == DialogResult.Yes)
            {
                cardView1.DeleteSelectedRows();
                gridControl1.RefreshDataSource();
                SaveWaypointstextfile();
            }
        }

        private void cardView2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteRoute();
            }
        }

        private void DeleteRoute()
        {
            var dialogResult = XtraMessageBox.Show("Do you want to Delete the Route(s)?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Hand);
            if (dialogResult == DialogResult.Yes)
            {
                cardView2.DeleteSelectedRows();
                gridControl2.RefreshDataSource();
                _routeWayPointsList.Clear();
                SaveRoutestextfile();
            }
        }

        private void SaveWaypointstextfile()
        {
            var sb = new StringBuilder();
            foreach (DataRow waypointRow in _wayPointData.Rows)
            {
                var selectedRowPoint = DataRead.GetGeoPointFromDataRow(waypointRow);

                sb.AppendLine(waypointRow[0] + ";" + selectedRowPoint.Lat + ";" + selectedRowPoint.Lon);
                File.WriteAllText(_wayPointPath, sb.ToString());
            }
            if (_wayPointData.Rows.Count == 0)
            {
                sb.AppendLine(string.Empty);
                File.WriteAllText(_wayPointPath, sb.ToString());
            }
        }

        private void SaveRoutestextfile()
        {
            var sb = new StringBuilder();
            var i = 1;
            foreach (DataRow routeRow in _routePointsData.Rows)
            {
                sb.AppendLine(i + ";" + routeRow[1] + ";" + routeRow[2] + ";" + routeRow[3] + ";" + routeRow[4] + ";" + routeRow[5] + ";" + routeRow[6] + ";" + routeRow[7]);
                File.WriteAllText(_routePointsPath, sb.ToString());
                i++;
            }
            if (_routePointsData.Rows.Count == 0)
            {
                sb.AppendLine(string.Empty);
                File.WriteAllText(_routePointsPath, sb.ToString());
            }
        }

        private void cardView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var selectedRows = cardView1.GetSelectedRows();
            if (selectedRows.Length == 1)
            {
                var selectedDataRow = cardView1.GetDataRow(selectedRows.First());
                var selectedRowPoint = DataRead.GetGeoPointFromDataRow(selectedDataRow);

                var selectedPoint = new GeoPoint
                {
                    Lat = selectedRowPoint.Lat,
                    Lon = selectedRowPoint.Lon
                };

                m_draw.SetPosition(selectedPoint, _defaultPixelPoint);
                S57Control1.Invalidate();
            }
        }

        private void waypointcreatebutton_Click(object sender, EventArgs e)
        {
            if (wayptxt_Name.Text != string.Empty && wayptxt_Lat.Text != string.Empty && wayptxt_Lon.Text != string.Empty)
            {
                wayPointName = wayptxt_Name.Text;
                var waypoint = new GeoCoordinate
                {
                    Latitude = Convert.ToDouble(wayptxt_Lat.Text),
                    Longitude = Convert.ToDouble(wayptxt_Lon.Text),
                };

                var row = _wayPointData.NewRow();
                row[0] = wayPointName;
                string[] cardinalString = DataRead.GetCardinalCoordinates(waypoint);

                row[1] = cardinalString[0];
                row[2] = cardinalString[1];

                _wayPointData.Rows.Add(row);
                AppendWaypointInfile(wayPointName, waypoint);
                gridControl1.RefreshDataSource();
                Clearuserwaypoints();
            }
            else
            {
                if (wayptxt_Name.Text == String.Empty)
                {
                    dxErrorProvider1.SetError(wayptxt_Name, "Enter Name");
                }
                if (wayptxt_Lat.Text == String.Empty)
                {
                    dxErrorProvider1.SetError(wayptxt_Lat, "Enter Latitude");
                }
                if (wayptxt_Lon.Text == String.Empty)
                {
                    dxErrorProvider1.SetError(wayptxt_Lon, "Enter Longitude");
                }
            }
        }

        private void AppendWaypointInfile(string wpName, GeoCoordinate waypoint)
        {
            var waypointString = wpName + ";" + waypoint.Latitude + ";" + waypoint.Longitude;
            var sw = File.AppendText(_wayPointPath);
            sw.WriteLine(waypointString);
            sw.Flush();
            sw.Close();
        }

        private void routecreate_button_Click(object sender, EventArgs e)
        {
            if (txtedit_routeName.Text != String.Empty && spintxtedit_usvspeed.Text != String.Empty &&
                txtedit_Totaldistance.Text != String.Empty && txtedit_Timetake.Text != String.Empty && double.TryParse(spintxtedit_usvspeed.Text, out _)
                && double.TryParse(txtedit_Totaldistance.Text, out _) && double.TryParse(txtedit_Timetake.Text, out _))
            {
                routeIndex = cardView2.DataRowCount + 1;
                routeName = txtedit_routeName.Text;
                routeDescription = memoedit_routeDescription.Text;

                // Get the selected color
                var selectedColor = colorPickedit_routeColour.Color.ToArgb();
                routeColour = selectedColor.ToString();

                routeSpeed = spintxtedit_usvspeed.Text + " kn";
                routeTotalDistance = txtedit_Totaldistance.Text + " nm";
                routeTotalTime = txtedit_Timetake.Text + " H";
                var routeWaypointList = string.Join(":", _routeWayPointsList);

                var index = 0;
                DataRow routeRow;
                if (_editRouteIndex != -1)
                {
                    routeRow = _routePointsData.Rows.Find(_editRouteIndex);
                    index = Convert.ToInt32(routeRow["Index"]);
                }
                else
                {
                    routeRow = _routePointsData.NewRow();
                    routeRow[0] = routeIndex;
                    routeRow[1] = routeName;
                    routeRow[2] = routeDescription;
                    routeRow[3] = routeColour;
                    routeRow[4] = routeSpeed;
                    routeRow[5] = routeTotalDistance;
                    routeRow[6] = routeTotalTime;
                    routeRow[7] = routeWaypointList;
                    routeRow[8] = true;
                }

                if (_editRouteIndex == -1)
                {
                    _routePointsData.Rows.Add(routeRow);
                    AppendRoutesInfile(routeIndex, routeName, routeDescription, routeColour, routeSpeed, routeTotalDistance, routeTotalTime, _routeWayPointsList);
                }
                else
                {
                    var rRow = _routePointsData.Rows.Find(_editRouteIndex);
                    index = Convert.ToInt32(rRow["Index"]);
                    rRow[0] = index;
                    rRow[1] = routeName;
                    rRow[2] = routeDescription;
                    rRow[3] = rRow["Colour"].ToString();
                    rRow[4] = routeSpeed;
                    rRow[5] = routeTotalDistance;
                    rRow[6] = routeTotalTime;
                    rRow[7] = routeWaypointList;
                    rRow[8] = true;
                }

                gridControl2.RefreshDataSource();
                _editRouteIndex = -1;
                ClearuserRoutesValues();
            }
            else
            {
                if (txtedit_routeName.Text == String.Empty)
                {
                    dxErrorProvider1.SetError(txtedit_routeName, "Invalid Value");
                }
                if (spintxtedit_usvspeed.Text == String.Empty || !double.TryParse(spintxtedit_usvspeed.Text, out _))
                {
                    dxErrorProvider1.SetError(spintxtedit_usvspeed, "Invalid Value");
                }
                if (txtedit_Totaldistance.Text == String.Empty || !double.TryParse(txtedit_Totaldistance.Text, out _))
                {
                    dxErrorProvider1.SetError(txtedit_Totaldistance, "Invalid Value");
                }
                if (txtedit_Timetake.Text == String.Empty || !double.TryParse(txtedit_Timetake.Text, out _))
                {
                    dxErrorProvider1.SetError(txtedit_Timetake, "Invalid Value");
                }
            }
        }

        private void AppendRoutesInfile(int index, string name, string description, string colour, string speed, string totalDistance, string totalTime, List<GeoCoordinate> waypointsList)
        {
            StringBuilder routewaypointsString = new StringBuilder();
            routewaypointsString.Append(index).Append(";").Append(name).Append(";")
                .Append(description).Append(";")
                .Append(colour).Append(";").Append(speed).Append(";")
                .Append(totalDistance).Append(";").Append(totalTime).Append(";");

            foreach (var routeWaypoint in waypointsList)
            {
                routewaypointsString.Append(routeWaypoint).Append(":");
            }

            using (var sw = File.AppendText(_routePointsPath))
            {
                sw.WriteLine(routewaypointsString.ToString());
                sw.Flush();
            }
        }

        private void ClearuserRoutesValues()
        {
            txtedit_routeName.Text = string.Empty;
            memoedit_routeDescription.Text = string.Empty;
            colorPickedit_routeColour.Color = Color.Red;
            spintxtedit_usvspeed.Text = string.Empty;
            txtedit_Totaldistance.Text = string.Empty;
            txtedit_Timetake.Text = string.Empty;
            dxErrorProvider1.ClearErrors();
        }

        private void waypointcancelbutton_Click(object sender, EventArgs e)
        {
            Clearuserwaypoints();
        }

        private void windowsUIButtonPanel10_ButtonChecked(object sender, ButtonEventArgs e)
        {
            var buttonName = ((WindowsUIButton)e.Button).Caption;

            switch (buttonName)
            {
                case "WayPoints":
                    tabPane3.SelectedPage = tabNavigationPage5;
                    tabPane3.Show();
                    tabPane3.Dock = DockStyle.Left;
                    break;
                case "Routes":
                    tabPane3.SelectedPage = tabNavigationPage6;
                    tabPane3.Show();
                    tabPane3.Dock = DockStyle.Left;
                    break;
                case "Measure":
                    _currentChartMode = ChartMode.Measure;
                    break;
                case "N":
                    break;
                case "Record":
                    break;
            }
        }

        private void windowsUIButtonPanel10_ButtonUnchecked(object sender, ButtonEventArgs e)
        {
            var buttonName = ((WindowsUIButton)e.Button).Caption;

            switch (buttonName)
            {
                case "WayPoints":
                    tabPane3.Hide();
                    break;
                case "Routes":
                    tabPane3.Hide();
                    break;
                case "Measure":
                    _measuredList.Clear();
                    _currentChartMode = ChartMode.Drag;
                    break;
                case "N":
                    break;
                case "Record":
                    break;
            }
        }

        private void barStaticItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            popupMenu2.HidePopup();
            windowsUIButtonPanel10.Buttons["WayPoints"].Properties.Checked = true;
            tabPane3.SelectedPage = tabNavigationPage5;
            tabPane3.Show();
            tabPane3.Dock = DockStyle.Left;
        }

        private void barStaticItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            _anchorGeoCoordinate = new GeoCoordinate(_markGeoPoint.Lat, _markGeoPoint.Lon);
            popupMenu2.HidePopup();
        }

        private void barStaticItem10_ItemClick(object sender, ItemClickEventArgs e)
        {
            HomeLocation = new GeoCoordinate(_markGeoPoint.Lat, _markGeoPoint.Lon);
            popupMenu2.HidePopup();
        }

        private void windowsUIButtonPanel8_ButtonClick(object sender, ButtonEventArgs e)
        {
            bool succeeded;
            int pnScaleout;
            var pnScaleIn = m_draw.DisplayScale;
            m_draw.CalculateNewDisplayScale(out succeeded, out pnScaleout, pnScaleIn, 1, 1);

            if (succeeded)
            {
                m_draw.SetDisplayScale(pnScaleout, _defaultPixelPoint);
            }
        }

        private void windowsUIButtonPanel7_ButtonClick(object sender, ButtonEventArgs e)
        {
            bool succeeded;
            int pnScaleout;
            var pnScaleIn = m_draw.DisplayScale;
            m_draw.CalculateNewDisplayScale(out succeeded, out pnScaleout, pnScaleIn, -1, 1);

            if (succeeded)
            {
                m_draw.SetDisplayScale(pnScaleout, _defaultPixelPoint);
            }
        }

        private void barStaticItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Clear the list when starting a new route
            _routeWayPointsList.Clear();

            _currentChartMode = ChartMode.Route;
            popupMenu2.HidePopup();
            groupControl3.Show();
            messagelabel1.Text = @"Press End Button to stop drawing Route";
            messagelabel2.Text = @"Press Delete Button to Delete the  Route";
        }

        private void S57Control1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.End)
            {
                _refreshTimer.Stop();
                if (_currentChartMode == ChartMode.Route)
                {
                    groupControl3.Hide();
                    windowsUIButtonPanel10.Buttons["Routes"].Properties.Checked = true;
                    windowsUIButtonPanel1.Buttons[0].Properties.Checked = true;
                    _currentChartMode = ChartMode.Drag;
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                if (_currentChartMode == ChartMode.Route)
                {
                    _routeWayPointsList.Clear();
                    _currentChartMode = ChartMode.Drag;
                }
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            foreach (DataRow dataRow in _routePointsData.Rows)
            {
                dataRow["Show Route"] = false;
            }

            gridControl2.RefreshDataSource();
            _routeWayPointsList.Clear();
            S57Control1.Invalidate();
        }

        private void routrcancel_button_Click(object sender, EventArgs e)
        {
            ClearuserRoutesValues();
        }

        private void cardView2_CustomCardCaptionImage(object sender, DevExpress.XtraGrid.Views.Card.CardCaptionImageEventArgs e)
        {
            e.Image = _routeIcon;
        }

        private void repositoryItemCheckEdit2_CheckedChanged(object sender, EventArgs e)
        {
            cardView2.PostEditor();

            if (((CheckEdit)sender).Checked) SelectRoute();
        }

        private void SelectRoute()
        {
            _currentChartMode = ChartMode.Drag;

            var lastClickedRow = cardView2.GetFocusedDataRow();
            var lastClickedGeoPoints = GetPointsFromString(Convert.ToString(lastClickedRow["waypoints of Route"]));

            var routeFirstPoint = new GeoPoint
            {
                Lat = lastClickedGeoPoints.First().Latitude,
                Lon = lastClickedGeoPoints.First().Longitude
            };
            m_draw.SetPosition(routeFirstPoint, _defaultPixelPoint);
            S57Control1.Invalidate();
        }

        private void cardView2_CustomDrawCardField(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName.Equals("Colour"))
            {
                var rowData = cardView2.GetDataRow(e.RowHandle);
                e.Cache.FillRectangle(e.Cache.GetSolidBrush(Color.FromArgb(Convert.ToInt32(rowData["Colour"]))),
                    e.Bounds);
                e.Handled = true;
            }
        }

        private void windowsUIButtonPanel4_ButtonClick(object sender, ButtonEventArgs e)
        {
            var currentTime = DateTime.Now;
            var filename = "PIC-" + currentTime.ToLongDateString().Replace(' ', '-') + "-" +
                           currentTime.ToLongTimeString().Replace(':', '-') + ".png";
            _snapRecorder.Record(Path.Combine(_recordingPath, filename));
        }
    }
}
