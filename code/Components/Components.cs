using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using APMTools;
using Microsoft.Windows.Controls;

namespace APMComponents
{
    #region Tools

    #region Button ImageType
    public enum ButtonImageType { Copy, Print, Browse, UseLess, ReturnUseLess, Confirm, UnConfirm, SaveTemp, SaveNote, Search, Refresh, Insert, Delete, Edit, Save, Cancel, Next, Previous, First, Last, Help, Select, UndoUseLess, Backup, Restore, Action, Hesabdari, Adjustment };
    #endregion

    #region Button ImageType
    public enum ImageType { Wrong, Correct };
    #endregion

    #region WhiteSide
    public enum WhiteSideMode { Up, Down };
    #endregion

    #region SubSystemColors
    public static class SubSystemColors
    {
        public class ColorPackage
        {
            public Color BackgroundColor = new Color();
            public Brush HorizontalLineBrush;
            public Brush VerticalLineBrush;
            public Brush BordersBrush;
            public Brush AlternateRowColor;
            public Brush TabItemHeaderBackGround;
            public Brush[] TreeColors = new Brush[10];
        }
        public static ColorPackage[] Items = new ColorPackage[typeof(SubSystems).GetMembers().Count()];
        static SubSystemColors()
        {
            Items[(int)SubSystems.Accounting] = new ColorPackage();
            Items[(int)SubSystems.Accounting].BackgroundColor = Colors.LightBlue;
            Items[(int)SubSystems.Accounting].HorizontalLineBrush = Brushes.CadetBlue;
            Items[(int)SubSystems.Accounting].VerticalLineBrush = Brushes.CadetBlue;
            Items[(int)SubSystems.Accounting].BordersBrush = Brushes.CornflowerBlue;
            Items[(int)SubSystems.Accounting].AlternateRowColor = Brushes.AliceBlue;
            Items[(int)SubSystems.Accounting].TabItemHeaderBackGround = Brushes.AliceBlue;
            Items[(int)SubSystems.Accounting].TabItemHeaderBackGround = CreateColorForTabItem(Colors.CornflowerBlue);
            Items[(int)SubSystems.Accounting].TreeColors[0] = Brushes.MidnightBlue;
            Items[(int)SubSystems.Accounting].TreeColors[1] = Brushes.DarkSlateBlue;
            Items[(int)SubSystems.Accounting].TreeColors[2] = Brushes.SteelBlue;
            Items[(int)SubSystems.Accounting].TreeColors[3] = Brushes.DeepSkyBlue;
            Items[(int)SubSystems.Accounting].TreeColors[4] = Brushes.Blue;
            Items[(int)SubSystems.Accounting].TreeColors[5] = Brushes.RoyalBlue;
            Items[(int)SubSystems.Accounting].TreeColors[6] = Brushes.CornflowerBlue;

            Items[(int)SubSystems.Inventory] = new ColorPackage();
            Items[(int)SubSystems.Inventory].BackgroundColor = Color.FromRgb(236, 232, 243);
            Items[(int)SubSystems.Inventory].HorizontalLineBrush = Brushes.Thistle;
            Items[(int)SubSystems.Inventory].VerticalLineBrush = Brushes.MediumOrchid;
            Items[(int)SubSystems.Inventory].BordersBrush = Brushes.Thistle;
            Items[(int)SubSystems.Inventory].AlternateRowColor = Brushes.Thistle;
            Items[(int)SubSystems.Inventory].TabItemHeaderBackGround = CreateColorForTabItem(Colors.Thistle);
            Items[(int)SubSystems.Inventory].TreeColors[0] = Brushes.Purple; ;
            Items[(int)SubSystems.Inventory].TreeColors[1] = Brushes.BlueViolet;
            Items[(int)SubSystems.Inventory].TreeColors[2] = Brushes.MediumVioletRed;
            Items[(int)SubSystems.Inventory].TreeColors[3] = Brushes.Orchid;
            Items[(int)SubSystems.Inventory].TreeColors[4] = Brushes.DarkViolet;
            Items[(int)SubSystems.Inventory].TreeColors[5] = Brushes.MediumOrchid;

        }
        private static LinearGradientBrush CreateColorForTabItem(Color headerOfTabItem)
        {
            GradientStop[] collection = new GradientStop[] 
            { 
                new GradientStop(headerOfTabItem, 0),
                new GradientStop(Colors.Snow, 0.445), 
                new GradientStop(headerOfTabItem, 1), 
                new GradientStop(Colors.Snow, 0.53) 
            };
            return new LinearGradientBrush() { GradientStops = new GradientStopCollection(collection) };
        }
    }
    #endregion

    #region SetBackGround Class
    abstract class Tools
    {
        #region SetBackground
        public static void SetBackground(Control control, Boolean transparent, int whiteSide, Color backGroundColor)
        {
            if (transparent)
            {
                control.Background = Brushes.Transparent;
                return;
            }

            LinearGradientBrush backGround = new LinearGradientBrush();
            backGround.EndPoint = new Point(0, 1);
            backGround.StartPoint = new Point(0, 0.25);

            backGround.GradientStops.Add(new GradientStop(backGroundColor, 1 - whiteSide));
            backGround.GradientStops.Add(new GradientStop(Colors.White, whiteSide));
            control.Background = backGround;
        }
        #endregion

        #region SetBackground (For Borders)
        public static void SetBackground(Border control, Boolean transparent, int whiteSide, Color borderBackGroundColor)
        {
            if (transparent)
            {
                control.Background = Brushes.Transparent;
                return;
            }

            LinearGradientBrush backGround = new LinearGradientBrush();
            backGround.EndPoint = new Point(0, 1);
            backGround.StartPoint = new Point(0, 0.25);

            backGround.GradientStops.Add(new GradientStop(borderBackGroundColor, 1 - whiteSide));
            backGround.GradientStops.Add(new GradientStop(Colors.White, whiteSide));
            control.Background = backGround;
        }
        #endregion

    }
    #endregion

    #region XWindowType
    public enum XWindowType { NormalWindow, SelectWindow, InvDocumentWindow, AccDocumentWindow, OptionWindow, ReportWindow };
    #endregion

    #region Functions
    class Functions
    {
        #region Convert_Bitmap_To_Source
        public static BitmapSource Convert_Bitmap_To_Source(System.Drawing.Bitmap bm)
        {
            BitmapSource source = null;
            if (bm != null)
            {
                IntPtr h_bm = bm.GetHbitmap();
                source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap
                    (h_bm, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            return source;
        }
        #endregion
    }
    #endregion
    #endregion

    #region Components

    #region APM (DocumentHeader)
    public class APMDocumentHeader : APMGroupBoxExtended
    {
        #region Controls

        /*----Main----*/
        private APMScrollViewer scrollViewer = new APMScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
        private StackPanel stkMain = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkWizard = new StackPanel() { Orientation = Orientation.Horizontal };

        /*----Description----*/
        private StackPanel stkDescription = new StackPanel() { Orientation = Orientation.Horizontal };
        private APMTextBox txtDescription = new APMTextBox();
        private APMLabel lblDescriptionTitle = new APMLabel() { Content = "شرح سند : " };


        /*----Invisible Document Types----*/
        private CheckBox chkShowGoodsRequest = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = false };
        private CheckBox chkShowPhisycalCountingControls = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        private CheckBox chkShowDestinationEntity = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowMainStore = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowReceiveType = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowSendType = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowAccountingPrice = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowAccount = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowAccountingDocumentStatus = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowRequest = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowBaseDocument = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowBaseDocumentTypeSelect = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowConfirm = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        private CheckBox chkShowAccountingDocumentType = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        private CheckBox chkShowStkDocumentNo = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowlblDocumentNo = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowTitleOfDocumentNo = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowBaseGoodsRequest = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowBaseReceive = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowBaseSendAndBaseBuyRequest = new CheckBox() { Visibility = Visibility.Collapsed };
        private CheckBox chkShowCardexGoods = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        private CheckBox chkShowGoodsFilter = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        private CheckBox chkShowAccountingLevelRadios = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        private CheckBox chkShowAccountingLevelCheckBoxes = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        private CheckBox chkShowSendOrReceive = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = false };
        private CheckBox chkForReport = new CheckBox() { Visibility = Visibility.Collapsed, IsChecked = false };


        /*----Cardex Goods----*/
        private APMGroupBoxExtended grpCardexGoods = new APMGroupBoxExtended() { Header = "انتخاب کالا جهت مشاهدۀ کاردکس" };
        private StackPanel stkCardexGoods = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkCardexGoodsTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkCardexGoodsControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblCardexGoodsSelectGoodsTitle = new APMLabel() { Content = "انتخاب کالا :" };
        private APMLabel lblCardexGoodsSelectMeasureTitle = new APMLabel() { Content = "واحد اندازه گیری :" };
        public APMBrowser brwCardexGoodsSelectGoods = new APMBrowser();
        public APMComboBox cmbCardexGoodsSelectMeasure = new APMComboBox();

        /*----Accounting Level Radios----*/
        private APMGroupBoxExtended grpAccountingLevelRadios = new APMGroupBoxExtended() { Header = "سطح گزارش گیری" };
        private StackPanel stkAccountingLevelRadios = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkAccountingLevelRadiosControls = new StackPanel() { Orientation = Orientation.Vertical };
        private RadioButton radGroup = new RadioButton() { Content = "گروه" };
        private RadioButton radKol = new RadioButton() { Content = "کل" };
        private RadioButton radMoein = new RadioButton() { Content = "معین" };
        private RadioButton radDetail = new RadioButton() { Content = "تفصیل" };

        /*---- Accounting Level CheckBoxes ----*/
        private APMGroupBoxExtended grpAccountingLevelCheckBoxes = new APMGroupBoxExtended() { Header = "سطح گزارش گیری" };
        private StackPanel stkAccountingLevelCheckBoxes = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkAccountingLevelCheckBoxesControls = new StackPanel() { Orientation = Orientation.Vertical };
        private CheckBox chkGroup = new CheckBox() { Content = "گروه", IsChecked = true };
        private CheckBox chkKol = new CheckBox() { Content = "کل" };
        private CheckBox chkMoein = new CheckBox() { Content = "معین" };
        private CheckBox chkDetail = new CheckBox() { Content = "تفصیل" };

        /*----Account And Detail----*/
        private APMGroupBoxExtended grpAccount = new APMGroupBoxExtended() { Header = "حساب و تفصیل" };
        private StackPanel stkAccount = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkAccountTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkAccountControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblSelectAccountTitle = new APMLabel() { Content = "انتخاب حساب :" };
        private APMLabel lblSelectDetailTitle = new APMLabel() { Content = "انتخاب تفصیل :" };
        public APMBrowser brwSelectAccount = new APMBrowser();
        public APMBrowser brwSelectDetail = new APMBrowser();

        /*----Send Or Receive----*/
        private APMGroupBoxExtended grpSendOrReceive = new APMGroupBoxExtended() { Header = "رسید یا حواله" };
        private StackPanel stkSendOrReceive = new StackPanel() { Orientation = Orientation.Vertical };
        private CheckBox chkSendOrReceive_Send = new CheckBox() { Content = "حواله", IsChecked = true };
        private CheckBox chkSendOrReceive_Receive = new CheckBox() { Content = "رسید", IsChecked = true };

        /*----Main Information-----*/
        private APMGroupBoxExtended grpMainInformation = new APMGroupBoxExtended() { Header = "اطلاعات اصلی" };
        private StackPanel stkMainInformation = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkMainInformationTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkMainInformationControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblMainStoreTitle = new APMLabel() { Content = "انتخاب انبار :" };
        public APMBrowser brwMainStore = new APMBrowser();
        private APMLabel lblDateTitle = new APMLabel() { Content = "تاریخ سند :" };
        public PersianDatePicker pdpDate = new PersianDatePicker();
        public PersianDatePicker pdpFromDate = new PersianDatePicker();
        private APMLabel lblToDateTitle = new APMLabel() { Content = "تا :" };
        public PersianDatePicker pdpToDate = new PersianDatePicker();
        private APMLabel lblSendDocumentTypeTitle = new APMLabel() { Content = "نوع حواله :" };
        private APMComboBoxCoding cmbSendDocumentType = new APMComboBoxCoding() { XCategory = CodingCategory.Inv_SendType };
        private APMLabel lblReceiveDocumentTypeTitle = new APMLabel() { Content = "نوع رسید :" };
        private APMComboBoxCoding cmbReceiveDocumentType = new APMComboBoxCoding() { XCategory = CodingCategory.Inv_ReceiveType };
        private APMLabel lblAccountingDocumentTypeTitle = new APMLabel() { Content = "نوع سند :" };
        public APMComboBox cmbAccountingDocumentType = new APMComboBox();


        /*----WardenShip----*/
        private APMGroupBoxExtended grpWardenShip = new APMGroupBoxExtended() { Header = "نظارت" };
        private StackPanel stkWardenShip = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkWardenShipTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkWardenShipControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblWardenNameTitle = new APMLabel() { Content = "نام بازرس :" };
        public APMTextBox txtWardenName = new APMTextBox();
        private APMLabel lblWardenAccountingPersonelTitle = new APMLabel() { Content = "حسابدار ناظر :" };
        public APMBrowser brwAccountingPersonel = new APMBrowser();

        /*----PhysicalCountingDate----*/
        private APMGroupBoxExtended grpPhysicalCountingDate = new APMGroupBoxExtended() { Header = "تاریخ انبار گردانی" };
        private StackPanel stkPhysicalCountingDate = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkPhysicalCountingDateTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkPhysicalCountingDateControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblPhysicalStartDateTitle = new APMLabel() { Content = "تاریخ شروع :" };
        private PersianDatePicker pdpPhysicalStartDate = new PersianDatePicker();
        private APMLabel lblPhysicalEndDateTitle = new APMLabel() { Content = "تاریخ خاتمه :" };
        private PersianDatePicker pdpPhysicalEndDate = new PersianDatePicker();


        /*-----Accounting Document Status-----*/
        private APMGroupBoxExtended grpAccountingDocumentStatus = new APMGroupBoxExtended() { Header = "وضعیت سند" };
        private StackPanel stkAccountingDocumentStatus = new StackPanel() { Orientation = Orientation.Vertical };
        private APMCheckBox chkAccountingDocumentStatusConfirm = new APMCheckBox() { IsChecked = true, Content = "قطعی", Tag = AccDocumentStatus.Confirm };
        private APMCheckBox chkAccountingDocumentStatusTemp = new APMCheckBox() { IsChecked = true, Content = "موقت", Tag = AccDocumentStatus.Temporary };
        private APMCheckBox chkAccountingDocumentStatusUseLess = new APMCheckBox() { Content = "ابطالی", Tag = AccDocumentStatus.UseLess };

        /*-----Filter Goods-----*/
        private APMGroupBoxExtended grpFilterGoods = new APMGroupBoxExtended() { Header = "کالا" };
        private StackPanel stkFilterGoods = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkFilterGoodsControls = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkFilterGoodsTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblFilterGoodsSelectGoodsTitle = new APMLabel() { Content = "انتخاب کالا :", Margin = new Thickness(0, 6, 0, 0) };
        private APMLabel lblFilterGoodsSumAmountTitle = new APMLabel() { Content = " جمع مقدار :" };
        private APMLabel lblFilterGoodsSumAmountToTitle = new APMLabel() { Content = "تا :" };
        private APMLabel lblFilterGoodsSumPriceTitle = new APMLabel() { Content = "جمع مبلغ :" };
        private APMLabel lblFilterGoodsSumPriceToTitle = new APMLabel() { Content = "تا :" };
        public APMBrowser brwFilterGoodsSelectGoods = new APMBrowser();
        private APMIntTextBox txtFilterGoodsSumAmountFrom = new APMIntTextBox() { Margin = new Thickness(0, 2, 5, 2) };
        private APMIntTextBox txtFilterGoodsSumAmountTo = new APMIntTextBox() { Margin = new Thickness(0, 2, 5, 2) };
        private APMIntTextBox txtFilterGoodsSumPriceFrom = new APMIntTextBox() { Margin = new Thickness(0, 2, 5, 2) };
        private APMIntTextBox txtFilterGoodsSumPriceTo = new APMIntTextBox() { Margin = new Thickness(0, 2, 5, 2) };


        /*-----Have Base Document-----*/
        private APMGroupBoxExtended grpBaseDocument = new APMGroupBoxExtended() { Header = "سند مبنا" };
        private StackPanel stkHaveBaseDocument = new StackPanel() { Orientation = Orientation.Vertical };
        private RadioButton radNoBase = new RadioButton() { Content = "بدون سند مبنا", Cursor = Cursors.Hand };
        private RadioButton radHaveBase = new RadioButton() { Content = "بر اساس سند مبنا", Cursor = Cursors.Hand };

        /*-----Base Document Type-----*/
        private StackPanel stkBaseDocumentTypeAndSelect = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkBaseDocumentType = new StackPanel() { Orientation = Orientation.Vertical };
        private RadioButton radBaseGoodsRequest = new RadioButton() { Content = "درخواست کالا", Cursor = Cursors.Hand };
        private RadioButton radBaseReceive = new RadioButton() { Content = "رسید", Visibility = Visibility.Collapsed };
        private RadioButton radBaseBuyRequest = new RadioButton() { Content = "درخواست خرید" };
        private RadioButton radBaseSend = new RadioButton() { Content = "حواله" };
        private StackPanel stkBaseDocument = new StackPanel() { Orientation = Orientation.Horizontal };
        public APMBrowser brwBaseSend = new APMBrowser();
        public APMBrowser brwBaseReceive = new APMBrowser();
        public APMBrowser brwBaseGoodsRequest = new APMBrowser();
        public APMBrowser brwBaseBuyRequest = new APMBrowser();

        /*----Destination Detail----*/
        private APMGroupBoxExtended grpDestinationEntity = new APMGroupBoxExtended() { Header = "طرف سند" };
        public APMBrowser brwDestinationDetail = new APMBrowser() { XTitle = "طرف سند" };

        /*----GoodsRequest And BuyRequest----*/
        private APMGroupBoxExtended grpRequest = new APMGroupBoxExtended() { Header = "درخواست" };
        private StackPanel stkRequest = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkRequestTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkRequestControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblRequestDeadLineTitle = new APMLabel() { Content = "مهلت ارسال :", Margin = new Thickness(1) };
        public PersianDatePicker pdpRequestDeadLineDate = new PersianDatePicker();
        public PersianDatePicker pdpRequestDeadLineFromDate = new PersianDatePicker();
        private APMLabel lblRequestDeadLineToDateTitle = new APMLabel() { Content = "تا :" };
        public PersianDatePicker pdpRequestDeadLineToDate = new PersianDatePicker();
        private APMLabel lblRequestConfirmerTitle = new APMLabel() { Content = "تایید کننده :", Margin = new Thickness(1) };
        public APMBrowser brwRequestConfirmerPerson = new APMBrowser();

        /*----GoodsRequester----*/
        private APMGroupBoxExtended grpGoodsRequester = new APMGroupBoxExtended() { Header = "درخواست دهنده" };
        private StackPanel stkGoodsRequester = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkGoodsRequesterLabels = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkGoodsRequesterBrowsers = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblGoodsRequesterCostCenterTitle = new APMLabel() { Content = "مرکز هزینۀ درخواست دهنده :" };
        private APMLabel lblGoodsRequesterPersonelTitle = new APMLabel() { Content = "شخص درخواست دهنده :" };
        public APMBrowser brwGoodsRequesterCostCenter = new APMBrowser();
        public APMBrowser brwGoodsRequesterPersonel = new APMBrowser();

        /*----Confirm Information----*/
        private APMGroupBoxExtended grpConfirm = new APMGroupBoxExtended() { Header = "اطلاعات تایید" };
        private StackPanel stkConfirm = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkConfirmTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkConfirmControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblDocumentStatusTitle = new APMLabel() { Content = "وضعیت سند :" };
        private APMInfoLabel lblDocumentStatus = new APMInfoLabel();
        private APMLabel lblConfirmDateTimeTitle = new APMLabel() { Content = "زمان تایید :" };
        private APMLabel lblConfirmDateTitle = new APMLabel() { Content = "تاریخ تایید :" };
        private StackPanel stkConfirmDateTime = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left };
        private APMInfoLabel lblConfirmDate = new APMInfoLabel();
        private APMInfoLabel lblConfirmTime = new APMInfoLabel();
        private PersianDatePicker pdpConfirmFromDate = new PersianDatePicker();
        private PersianDatePicker pdpConfirmToDate = new PersianDatePicker();
        private APMLabel lblConfirmToDateTitle = new APMLabel() { Content = "تا :" };
        private APMLabel lblConfirmUserNameTitle = new APMLabel() { Content = "تایید کننده :" };
        private APMInfoLabel lblConfirmerUserName = new APMInfoLabel();
        private APMBrowser brwConfirmerUser = new APMBrowser();

        /*----Document Price----*/
        private APMGroupBoxExtended grpDocumentPrice = new APMGroupBoxExtended() { Header = "مبلغ سند" };
        private StackPanel stkDocumentPrice = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkDocumentPriceTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkDocumentPriceControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblDocumentPriceDebtTitle = new APMLabel() { Content = "مبلغ بدهکار :" };
        private APMIntTextBox txtDocumentPriceFromDebt = new APMIntTextBox();
        private APMLabel lblDocumentPriceToDebtTitle = new APMLabel() { Content = "تا :" };
        private APMIntTextBox txtDocumentPriceToDebt = new APMIntTextBox();
        private APMLabel lblDocumentPriceCreditTitle = new APMLabel() { Content = "مبلغ بستانکار :" };
        private APMIntTextBox txtDocumentPriceFromCredit = new APMIntTextBox();
        private APMLabel lblDocumentPriceToCreditTitle = new APMLabel() { Content = "تا :" };
        private APMIntTextBox txtDocumentPriceToCredit = new APMIntTextBox();

        /*----Article Price----*/
        private APMGroupBoxExtended grpArticlePrice = new APMGroupBoxExtended() { Header = "مبلغ آرتیکل" };
        private StackPanel stkArticlePrice = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkArticlePriceTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkArticlePriceControls = new StackPanel() { Orientation = Orientation.Vertical };
        private APMLabel lblArticlePriceDebtTitle = new APMLabel() { Content = "مبلغ بدهکار :" };
        private APMIntTextBox txtArticlePriceFromDebt = new APMIntTextBox();
        private APMLabel lblArticlePriceToDebtTitle = new APMLabel() { Content = "تا :" };
        private APMIntTextBox txtArticlePriceToDebt = new APMIntTextBox();
        private APMLabel lblArticlePriceCreditTitle = new APMLabel() { Content = "مبلغ بستانکار :" };
        private APMIntTextBox txtArticlePriceFromCredit = new APMIntTextBox();
        private APMLabel lblArticlePriceToCreditTitle = new APMLabel() { Content = "تا :" };
        private APMIntTextBox txtArticlePriceToCredit = new APMIntTextBox();

        /*----Register Information----*/
        private APMGroupBoxExtended grpRegister = new APMGroupBoxExtended() { Header = "اطلاعات ثبت" };
        private StackPanel stkRegister = new StackPanel() { Orientation = Orientation.Horizontal };
        private StackPanel stkRegisterTitles = new StackPanel() { Orientation = Orientation.Vertical };
        private StackPanel stkRegisterControls = new StackPanel() { Orientation = Orientation.Vertical };

        private APMLabel lblDocumentNoTitle = new APMLabel() { Content = "شمارۀ سریال :", BorderThickness = new Thickness(0), BorderBrush = Brushes.LightGray };
        private APMInfoLabel lblDocumentNo = new APMInfoLabel() { BorderThickness = new Thickness(0), BorderBrush = Brushes.LightGray };
        private StackPanel stkDocumentNo = new StackPanel() { Orientation = Orientation.Horizontal };
        private APMIntTextBox txtDocumentNoFrom = new APMIntTextBox();
        private APMLabel lblDocumentNoToTitle = new APMLabel() { Content = "تا :" };
        private APMIntTextBox txtDocumentNoTo = new APMIntTextBox();


        private APMLabel lblDocumentCodeTitle = new APMLabel() { BorderThickness = new Thickness(0), Content = "شمارۀ سند :" };
        private APMInfoLabel lblDocumentCode = new APMInfoLabel() { BorderThickness = new Thickness(0) };
        private StackPanel stkDocumentCode = new StackPanel() { Orientation = Orientation.Horizontal };
        private APMIntTextBox txtDocumentCodeFrom = new APMIntTextBox();
        private APMLabel lblDocumentCodeToTitle = new APMLabel() { Content = "تا :" };
        private APMIntTextBox txtDocumentCodeTo = new APMIntTextBox();

        private APMLabel lblRegisterDateTimeTitle = new APMLabel() { Content = "زمان ثبت :" };
        private StackPanel stkRegisterDateTime = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left };
        private APMInfoLabel lblRegisterDate = new APMInfoLabel();
        private APMInfoLabel lblRegisterTime = new APMInfoLabel();
        private APMLabel lblRegisterDateTitle = new APMLabel() { Content = "تاریخ ثبت :" };
        private PersianDatePicker pdpRegisterFromDate = new PersianDatePicker();
        private APMLabel lblRegisterToDateTitle = new APMLabel() { Content = "تا :" };
        private PersianDatePicker pdpRegisterToDate = new PersianDatePicker();

        private APMLabel lblRegistererUserNameTitle = new APMLabel() { Content = "ثبت کننده :" };
        private APMInfoLabel lblRegisterUserName = new APMInfoLabel();
        public APMBrowser brwRegistererUser = new APMBrowser();

        /*----FiscalYear----*/
        //private APMGroupBoxExtended grpFiscalYear = new APMGroupBoxExtended() { Header = "انتخاب دوره مالی" };
        //private StackPanel stkFiscalYear = new StackPanel();
        //public APMComboBox cmbFiscalYear = new APMComboBox();

        #endregion

        #region Constructor
        public APMDocumentHeader()
        {
            DockPanel.SetDock(this, Dock.Top);
            HorizontalAlignment = HorizontalAlignment.Left;
            CreateControls();
            SetControlsProperties(this);
            BindControlsToEachOther();
            ChangeControlsForReport();
            AssignInternalEvents();
        }
        #endregion

        #region Methods

        #region CreateControls
        private void CreateControls()
        {
            /*----Main StackPanel----*/
            this.Content = scrollViewer;
            scrollViewer.Content = stkMain;
            stkMain.Children.Clear();
            stkMain.Children.Add(stkWizard);
            stkMain.Children.Add(stkDescription);


            /*----Description----*/
            stkDescription.Children.Clear();
            stkDescription.Children.Add(lblDescriptionTitle);
            stkDescription.Children.Add(txtDescription);

            /*----Wizard----*/
            stkWizard.Children.Clear();
            stkWizard.Children.Add(grpCardexGoods);
            stkWizard.Children.Add(grpAccountingLevelRadios);
            stkWizard.Children.Add(grpAccountingLevelCheckBoxes);
            stkWizard.Children.Add(grpAccount);
            stkWizard.Children.Add(grpSendOrReceive);
            stkWizard.Children.Add(grpMainInformation);
            stkWizard.Children.Add(grpWardenShip);
            stkWizard.Children.Add(grpPhysicalCountingDate);
            stkWizard.Children.Add(grpAccountingDocumentStatus);
            stkWizard.Children.Add(grpFilterGoods);
            stkWizard.Children.Add(grpBaseDocument);
            stkWizard.Children.Add(grpDestinationEntity);
            stkWizard.Children.Add(grpGoodsRequester);
            stkWizard.Children.Add(grpRequest);
            stkWizard.Children.Add(grpConfirm);
            stkWizard.Children.Add(grpDocumentPrice);
            stkWizard.Children.Add(grpArticlePrice);
            stkWizard.Children.Add(grpRegister);
            //stkWizard.Children.Add(grpFiscalYear);

            /*----Cardex Goods----*/
            grpCardexGoods.Content = stkCardexGoods;
            stkCardexGoods.Children.Clear();
            stkCardexGoods.Children.Add(stkCardexGoodsTitles);
            stkCardexGoods.Children.Add(stkCardexGoodsControls);
            stkCardexGoodsTitles.Children.Clear();
            stkCardexGoodsTitles.Children.Add(lblCardexGoodsSelectGoodsTitle);
            stkCardexGoodsTitles.Children.Add(lblCardexGoodsSelectMeasureTitle);
            stkCardexGoodsControls.Children.Clear();
            stkCardexGoodsControls.Children.Add(brwCardexGoodsSelectGoods);
            stkCardexGoodsControls.Children.Add(cmbCardexGoodsSelectMeasure);


            /*----Accounting Level Radios----*/
            grpAccountingLevelRadios.Content = stkAccountingLevelRadios;
            stkAccountingLevelRadios.Children.Clear();
            stkAccountingLevelRadios.Children.Add(stkAccountingLevelRadiosControls);
            stkAccountingLevelRadiosControls.Children.Clear();
            stkAccountingLevelRadiosControls.Children.Add(radGroup);
            stkAccountingLevelRadiosControls.Children.Add(radKol);
            stkAccountingLevelRadiosControls.Children.Add(radMoein);
            stkAccountingLevelRadiosControls.Children.Add(radDetail);

            /*----Accounting Level CheckBoxes ----*/
            grpAccountingLevelCheckBoxes.Content = stkAccountingLevelCheckBoxes;
            stkAccountingLevelCheckBoxes.Children.Clear();
            stkAccountingLevelCheckBoxes.Children.Add(stkAccountingLevelCheckBoxesControls);
            stkAccountingLevelCheckBoxesControls.Children.Clear();
            stkAccountingLevelCheckBoxesControls.Children.Add(chkGroup);
            stkAccountingLevelCheckBoxesControls.Children.Add(chkKol);
            stkAccountingLevelCheckBoxesControls.Children.Add(chkMoein);
            stkAccountingLevelCheckBoxesControls.Children.Add(chkDetail);

            /*----Account And Detail----*/
            grpAccount.Content = stkAccount;
            stkAccount.Children.Clear();
            stkAccount.Children.Add(stkAccountTitles);
            stkAccount.Children.Add(stkAccountControls);
            stkAccountTitles.Children.Clear();
            stkAccountTitles.Children.Add(lblSelectAccountTitle);
            stkAccountTitles.Children.Add(lblSelectDetailTitle);
            stkAccountControls.Children.Clear();
            stkAccountControls.Children.Add(brwSelectAccount);
            stkAccountControls.Children.Add(brwSelectDetail);

            /*----Send Or Receive----*/
            grpSendOrReceive.Content = stkSendOrReceive;
            stkSendOrReceive.Children.Clear();
            stkSendOrReceive.Children.Add(chkSendOrReceive_Send);
            stkSendOrReceive.Children.Add(chkSendOrReceive_Receive);

            /*----Main Information----*/
            grpMainInformation.Content = stkMainInformation;
            stkMainInformation.Children.Clear();
            stkMainInformation.Children.Add(stkMainInformationTitles);
            stkMainInformation.Children.Add(stkMainInformationControls);

            stkMainInformationTitles.Children.Clear();
            stkMainInformationTitles.Children.Add(lblMainStoreTitle);
            stkMainInformationTitles.Children.Add(lblDateTitle);
            stkMainInformationTitles.Children.Add(lblToDateTitle);
            stkMainInformationTitles.Children.Add(lblSendDocumentTypeTitle);
            stkMainInformationTitles.Children.Add(lblReceiveDocumentTypeTitle);
            stkMainInformationTitles.Children.Add(lblAccountingDocumentTypeTitle);

            stkMainInformationControls.Children.Clear();
            stkMainInformationControls.Children.Add(brwMainStore);
            stkMainInformationControls.Children.Add(pdpDate);
            stkMainInformationControls.Children.Add(pdpFromDate);
            stkMainInformationControls.Children.Add(pdpToDate);
            stkMainInformationControls.Children.Add(cmbSendDocumentType);
            stkMainInformationControls.Children.Add(cmbReceiveDocumentType);
            stkMainInformationControls.Children.Add(cmbAccountingDocumentType);


            /*----WardenShip----*/
            grpWardenShip.Content = stkWardenShip;
            stkWardenShip.Children.Clear();
            stkWardenShip.Children.Add(stkWardenShipTitles);
            stkWardenShip.Children.Add(stkWardenShipControls);
            stkWardenShipTitles.Children.Clear();
            stkWardenShipTitles.Children.Add(lblWardenNameTitle);
            stkWardenShipTitles.Children.Add(lblWardenAccountingPersonelTitle);
            stkWardenShipControls.Children.Clear();
            stkWardenShipControls.Children.Add(txtWardenName);
            stkWardenShipControls.Children.Add(brwAccountingPersonel);

            /*----PhysicalCountingDate----*/
            grpPhysicalCountingDate.Content = stkPhysicalCountingDate;
            stkPhysicalCountingDate.Children.Clear();
            stkPhysicalCountingDate.Children.Add(stkPhysicalCountingDateTitles);
            stkPhysicalCountingDate.Children.Add(stkPhysicalCountingDateControls);
            stkPhysicalCountingDateTitles.Children.Clear();
            stkPhysicalCountingDateTitles.Children.Add(lblPhysicalStartDateTitle);
            stkPhysicalCountingDateTitles.Children.Add(lblPhysicalEndDateTitle);
            stkPhysicalCountingDateControls.Children.Clear();
            stkPhysicalCountingDateControls.Children.Add(pdpPhysicalStartDate);
            stkPhysicalCountingDateControls.Children.Add(pdpPhysicalEndDate);

            /*----Accounting Document Status----*/
            grpAccountingDocumentStatus.Content = stkAccountingDocumentStatus;
            stkAccountingDocumentStatus.Children.Clear();
            stkAccountingDocumentStatus.Children.Add(chkAccountingDocumentStatusConfirm);
            stkAccountingDocumentStatus.Children.Add(chkAccountingDocumentStatusTemp);
            stkAccountingDocumentStatus.Children.Add(chkAccountingDocumentStatusUseLess);

            /*----Filter Goods----*/
            grpFilterGoods.Content = stkFilterGoods;
            stkFilterGoods.Children.Clear();
            stkFilterGoods.Children.Add(stkFilterGoodsTitles);
            stkFilterGoods.Children.Add(stkFilterGoodsControls);
            stkFilterGoodsTitles.Children.Clear();
            stkFilterGoodsTitles.Children.Add(lblFilterGoodsSelectGoodsTitle);
            stkFilterGoodsControls.Children.Clear();
            stkFilterGoodsControls.Children.Add(brwFilterGoodsSelectGoods);

            /*-----Have Base Document-----*/
            grpBaseDocument.Content = stkBaseDocument;
            stkHaveBaseDocument.Children.Clear();
            stkHaveBaseDocument.Children.Add(radNoBase);
            stkHaveBaseDocument.Children.Add(radHaveBase);


            /*-----Base Document Type-----*/
            stkBaseDocument.Children.Clear();
            stkBaseDocument.Children.Add(stkHaveBaseDocument);
            stkBaseDocument.Children.Add(stkBaseDocumentTypeAndSelect);
            stkBaseDocumentTypeAndSelect.Children.Clear();
            stkBaseDocumentTypeAndSelect.Children.Add(new APMSeprator());
            stkBaseDocumentTypeAndSelect.Children.Add(stkBaseDocumentType);
            stkBaseDocumentTypeAndSelect.Children.Add(new APMSeprator());
            stkBaseDocumentTypeAndSelect.Children.Add(brwBaseReceive);
            stkBaseDocumentTypeAndSelect.Children.Add(brwBaseSend);
            stkBaseDocumentTypeAndSelect.Children.Add(brwBaseGoodsRequest);
            stkBaseDocumentTypeAndSelect.Children.Add(brwBaseBuyRequest);
            stkBaseDocumentType.Children.Clear();
            stkBaseDocumentType.Children.Add(radBaseGoodsRequest);
            stkBaseDocumentType.Children.Add(radBaseBuyRequest);
            stkBaseDocumentType.Children.Add(radBaseReceive);
            stkBaseDocumentType.Children.Add(radBaseSend);

            /*----Destination Entity----*/
            grpDestinationEntity.Content = brwDestinationDetail;


            /*----Request Information----*/
            grpRequest.Content = stkRequest;
            stkRequest.Children.Clear();
            stkRequest.Children.Add(stkRequestTitles);
            stkRequest.Children.Add(stkRequestControls);

            stkRequestTitles.Children.Clear();
            stkRequestTitles.Children.Add(lblRequestConfirmerTitle);
            stkRequestTitles.Children.Add(lblRequestDeadLineTitle);
            stkRequestTitles.Children.Add(lblRequestDeadLineToDateTitle);

            stkRequestControls.Children.Clear();
            stkRequestControls.Children.Add(brwRequestConfirmerPerson);
            stkRequestControls.Children.Add(pdpRequestDeadLineDate);
            stkRequestControls.Children.Add(pdpRequestDeadLineFromDate);
            stkRequestControls.Children.Add(pdpRequestDeadLineToDate);

            /*----GoodsRequester----*/
            grpGoodsRequester.Content = stkGoodsRequester;
            stkGoodsRequesterBrowsers.Children.Clear();
            stkGoodsRequesterBrowsers.Children.Add(brwGoodsRequesterCostCenter);
            stkGoodsRequesterBrowsers.Children.Add(brwGoodsRequesterPersonel);
            stkGoodsRequesterLabels.Children.Clear();
            stkGoodsRequesterLabels.Children.Add(lblGoodsRequesterCostCenterTitle);
            stkGoodsRequesterLabels.Children.Add(lblGoodsRequesterPersonelTitle);
            stkGoodsRequester.Children.Clear();
            stkGoodsRequester.Children.Add(stkGoodsRequesterLabels);
            stkGoodsRequester.Children.Add(stkGoodsRequesterBrowsers);

            /*----Confirm Information----*/
            grpConfirm.Content = stkConfirm;
            stkConfirm.Children.Clear();
            stkConfirm.Children.Add(stkConfirmTitles);
            stkConfirm.Children.Add(stkConfirmControls);

            stkConfirmDateTime.Children.Clear();
            stkConfirmDateTime.Children.Add(lblConfirmTime);
            stkConfirmDateTime.Children.Add(new APMLabel() { Content = "-", Margin = new Thickness(0) });
            stkConfirmDateTime.Children.Add(lblConfirmDate); stkConfirmTitles.Children.Clear();

            stkConfirmTitles.Children.Add(lblDocumentStatusTitle);
            stkConfirmTitles.Children.Add(lblConfirmDateTimeTitle);
            stkConfirmTitles.Children.Add(lblConfirmDateTitle);
            stkConfirmTitles.Children.Add(lblConfirmToDateTitle);
            stkConfirmTitles.Children.Add(lblConfirmUserNameTitle);

            stkConfirmControls.Children.Clear();
            stkConfirmControls.Children.Add(lblDocumentStatus);
            stkConfirmControls.Children.Add(stkConfirmDateTime);
            stkConfirmControls.Children.Add(pdpConfirmFromDate);
            stkConfirmControls.Children.Add(pdpConfirmToDate);
            stkConfirmControls.Children.Add(lblConfirmerUserName);
            stkConfirmControls.Children.Add(brwConfirmerUser);


            /*----DocumentPrice----*/
            grpDocumentPrice.Content = stkDocumentPrice;
            stkDocumentPrice.Children.Clear();
            stkDocumentPrice.Children.Add(stkDocumentPriceTitles);
            stkDocumentPrice.Children.Add(stkDocumentPriceControls);

            stkDocumentPriceTitles.Children.Add(lblDocumentPriceDebtTitle);
            stkDocumentPriceTitles.Children.Add(lblDocumentPriceToDebtTitle);
            stkDocumentPriceTitles.Children.Add(lblDocumentPriceCreditTitle);
            stkDocumentPriceTitles.Children.Add(lblDocumentPriceToCreditTitle);

            stkDocumentPriceControls.Children.Clear();
            stkDocumentPriceControls.Children.Add(txtDocumentPriceFromDebt);
            stkDocumentPriceControls.Children.Add(txtDocumentPriceToDebt);
            stkDocumentPriceControls.Children.Add(txtDocumentPriceFromCredit);
            stkDocumentPriceControls.Children.Add(txtDocumentPriceToCredit);

            /*----ArticlePrice----*/
            grpArticlePrice.Content = stkArticlePrice;
            stkArticlePrice.Children.Clear();
            stkArticlePrice.Children.Add(stkArticlePriceTitles);
            stkArticlePrice.Children.Add(stkArticlePriceControls);

            stkArticlePriceTitles.Children.Add(lblArticlePriceDebtTitle);
            stkArticlePriceTitles.Children.Add(lblArticlePriceToDebtTitle);
            stkArticlePriceTitles.Children.Add(lblArticlePriceCreditTitle);
            stkArticlePriceTitles.Children.Add(lblArticlePriceToCreditTitle);

            stkArticlePriceControls.Children.Clear();
            stkArticlePriceControls.Children.Add(txtArticlePriceFromDebt);
            stkArticlePriceControls.Children.Add(txtArticlePriceToDebt);
            stkArticlePriceControls.Children.Add(txtArticlePriceFromCredit);
            stkArticlePriceControls.Children.Add(txtArticlePriceToCredit);

            /*----Register Information----*/
            grpRegister.Content = stkRegister;
            stkRegister.Children.Clear();
            stkRegister.Children.Add(stkRegisterTitles);
            stkRegister.Children.Add(stkRegisterControls);

            stkRegisterDateTime.Children.Clear();
            stkRegisterDateTime.Children.Add(lblRegisterTime);
            stkRegisterDateTime.Children.Add(new APMLabel() { Content = "-", Margin = new Thickness(0) });
            stkRegisterDateTime.Children.Add(lblRegisterDate);

            stkDocumentNo.Children.Clear();
            stkDocumentNo.Children.Add(txtDocumentNoFrom);
            stkDocumentNo.Children.Add(lblDocumentNoToTitle);
            stkDocumentNo.Children.Add(txtDocumentNoTo);

            stkDocumentCode.Children.Clear();
            stkDocumentCode.Children.Add(txtDocumentCodeFrom);
            stkDocumentCode.Children.Add(lblDocumentCodeToTitle);
            stkDocumentCode.Children.Add(txtDocumentCodeTo);

            stkRegisterTitles.Children.Clear();
            stkRegisterTitles.Children.Add(lblDocumentNoTitle);
            stkRegisterTitles.Children.Add(lblDocumentCodeTitle);
            stkRegisterTitles.Children.Add(lblRegisterDateTimeTitle);
            stkRegisterTitles.Children.Add(lblRegisterDateTitle);
            stkRegisterTitles.Children.Add(lblRegisterToDateTitle);
            stkRegisterTitles.Children.Add(lblRegistererUserNameTitle);


            stkRegisterControls.Children.Clear();
            stkRegisterControls.Children.Add(lblDocumentNo);
            stkRegisterControls.Children.Add(stkDocumentNo);
            stkRegisterControls.Children.Add(lblDocumentCode);
            stkRegisterControls.Children.Add(stkDocumentCode);
            stkRegisterControls.Children.Add(stkRegisterDateTime);
            stkRegisterControls.Children.Add(pdpRegisterFromDate);
            stkRegisterControls.Children.Add(pdpRegisterToDate);
            stkRegisterControls.Children.Add(lblRegisterUserName);
            stkRegisterControls.Children.Add(brwRegistererUser);

            /*----FiscalYear----*/
            //grpFiscalYear.Content = stkFiscalYear;
            //stkFiscalYear.Children.Clear();
            //stkFiscalYear.Children.Add(cmbFiscalYear);

        }
        #endregion

        #region AssignInternalEvents
        private void AssignInternalEvents()
        {
            /*----report----*/
            chkForReport.Checked += chkForReport_Checked;
            chkForReport.Unchecked += chkForReport_Checked;

            /*----Have Base Document----*/
            radNoBase.IsEnabledChanged += APMDocumentHeader_IsEnabledChanged;
            radHaveBase.Checked += (radHaveBase_CheckedChanged);
            radHaveBase.Unchecked += (radHaveBase_CheckedChanged);

            /*----Base Document----*/
            brwBaseGoodsRequest.XTextBox.TextChanged += txtBaseDocument_TextChanged;
            brwBaseReceive.XTextBox.TextChanged += txtBaseDocument_TextChanged;
            brwBaseSend.XTextBox.TextChanged += txtBaseDocument_TextChanged;
            brwBaseBuyRequest.XTextBox.TextChanged += txtBaseDocument_TextChanged;

        }
        #endregion

        #region BindControlsToEachOther
        private void BindControlsToEachOther()
        {

            /*-----Base Document-----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpBaseDocument, chkShowBaseDocument);
            GlobalFunctions.BindVisibilityToIsChecked(stkBaseDocumentTypeAndSelect, chkShowBaseDocumentTypeSelect);
            GlobalFunctions.BindVisibilityToIsChecked(brwBaseReceive, radBaseReceive);
            GlobalFunctions.BindVisibilityToIsChecked(brwBaseSend, radBaseSend);
            GlobalFunctions.BindVisibilityToIsChecked(brwBaseGoodsRequest, radBaseGoodsRequest);
            GlobalFunctions.BindVisibilityToIsChecked(brwBaseBuyRequest, radBaseBuyRequest);
            GlobalFunctions.BindVisibilityToIsChecked(radBaseGoodsRequest, chkShowBaseGoodsRequest);
            GlobalFunctions.BindVisibilityToIsChecked(radBaseSend, chkShowBaseSendAndBaseBuyRequest);
            GlobalFunctions.BindVisibilityToIsChecked(radBaseReceive, chkShowBaseReceive);
            GlobalFunctions.BindVisibilityToIsChecked(radBaseBuyRequest, chkShowBaseSendAndBaseBuyRequest);


            /*---- Cardex Goods----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpCardexGoods, chkShowCardexGoods);

            /*----Accounting Level Radios----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpAccountingLevelRadios, chkShowAccountingLevelRadios);

            /*----Accounting Level Radios----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpAccountingLevelCheckBoxes, chkShowAccountingLevelCheckBoxes);

            /*----Account And Detail----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpAccount, chkShowAccount);

            /*----Send Or Receive----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpSendOrReceive, chkShowSendOrReceive);

            /*---- Main Information----*/
            GlobalFunctions.BindVisibilityToIsChecked(lblMainStoreTitle, chkShowMainStore);
            GlobalFunctions.BindVisibilityToIsChecked(brwMainStore, chkShowMainStore);
            GlobalFunctions.BindVisibilityToNotIsChecked(pdpDate, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(pdpFromDate, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(lblToDateTitle, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(pdpToDate, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(lblSendDocumentTypeTitle, chkShowSendType);
            GlobalFunctions.BindVisibilityToIsChecked(cmbSendDocumentType, chkShowSendType);
            GlobalFunctions.BindVisibilityToIsChecked(lblReceiveDocumentTypeTitle, chkShowReceiveType);
            GlobalFunctions.BindVisibilityToIsChecked(cmbReceiveDocumentType, chkShowReceiveType);
            GlobalFunctions.BindVisibilityToIsChecked(lblAccountingDocumentTypeTitle, chkShowAccountingDocumentType);
            GlobalFunctions.BindVisibilityToIsChecked(cmbAccountingDocumentType, chkShowAccountingDocumentType);

            /*----WardenShip----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpWardenShip, chkShowPhisycalCountingControls);

            /*----Physical Date----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpPhysicalCountingDate, chkShowPhisycalCountingControls);

            /*----Accounting Document status----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpAccountingDocumentStatus, chkShowAccountingDocumentStatus);


            /*----Filter Goods----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpFilterGoods, chkShowGoodsFilter);


            /*----Destination Entity----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpDestinationEntity, chkShowDestinationEntity);

            /*----GoodsRequest----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpGoodsRequester, chkShowGoodsRequest);


            /*----Request----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpRequest, chkShowRequest);
            GlobalFunctions.BindVisibilityToNotIsChecked(pdpRequestDeadLineDate, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(pdpRequestDeadLineFromDate, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(lblRequestDeadLineToDateTitle, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(pdpRequestDeadLineToDate, chkForReport);

            /*----Confirm----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpConfirm, chkShowConfirm);
            GlobalFunctions.BindVisibilityToNotIsChecked(lblDocumentStatusTitle, chkForReport);
            GlobalFunctions.BindVisibilityToNotIsChecked(lblDocumentStatus, chkForReport);
            GlobalFunctions.BindVisibilityToNotIsChecked(lblConfirmDateTimeTitle, chkForReport);
            GlobalFunctions.BindVisibilityToNotIsChecked(stkConfirmDateTime, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(lblConfirmDateTitle, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(lblConfirmToDateTitle, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(pdpConfirmFromDate, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(pdpConfirmToDate, chkForReport);
            GlobalFunctions.BindVisibilityToNotIsChecked(lblConfirmerUserName, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(brwConfirmerUser, chkForReport);

            /*----Document Price----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpDocumentPrice, chkShowAccountingPrice);

            /*----Article Price----*/
            GlobalFunctions.BindVisibilityToIsChecked(grpArticlePrice, chkShowAccountingPrice);

            /*----Register----*/
            GlobalFunctions.BindVisibilityToIsChecked(lblDocumentNoTitle, chkShowTitleOfDocumentNo);
            GlobalFunctions.BindVisibilityToIsChecked(lblDocumentNo, chkShowlblDocumentNo);
            GlobalFunctions.BindVisibilityToIsChecked(stkDocumentNo, chkShowStkDocumentNo);
            GlobalFunctions.BindVisibilityToNotIsChecked(lblDocumentCode, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(stkDocumentCode, chkForReport);
            GlobalFunctions.BindVisibilityToNotIsChecked(lblRegisterDateTimeTitle, chkForReport);
            GlobalFunctions.BindVisibilityToNotIsChecked(stkRegisterDateTime, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(lblRegisterDateTitle, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(pdpRegisterFromDate, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(lblRegisterToDateTitle, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(pdpRegisterToDate, chkForReport);
            GlobalFunctions.BindVisibilityToNotIsChecked(lblRegisterUserName, chkForReport);
            GlobalFunctions.BindVisibilityToIsChecked(brwRegistererUser, chkForReport);

            /*----FiscalYear----*/
            //GlobalFunctions.BindVisibilityToIsChecked(grpFiscalYear, chkForReport);

        }
        #endregion

        #region SetControlsProperties
        private void SetControlsProperties(object control)
        {
            if (control == null)
                return;

            else if (control is APMTextBox)
                (control as APMTextBox).Margin = new Thickness(0, 3, 0, 3);

            else if (control is APMInfoLabel)
                (control as APMInfoLabel).HorizontalAlignment = HorizontalAlignment.Left;

            else if (control is APMLabel)
            {
                (control as APMLabel).HorizontalAlignment = HorizontalAlignment.Right;
                (control as APMLabel).Margin = new Thickness(0, 2, 0, 2);
            }
            else if (control is ToggleButton)
            {
                (control as ToggleButton).Cursor = Cursors.Hand;
                (control as ToggleButton).Margin = new Thickness(5);
            }
            else if (control is ComboBox)
                (control as ComboBox).Margin = new Thickness(2);
            else if (control is PersianDatePicker)
            {
                (control as PersianDatePicker).HorizontalAlignment = HorizontalAlignment.Left;
                (control as PersianDatePicker).Margin = new Thickness(1);
                (control as PersianDatePicker).Text = "";
            }
            else if (control is APMBrowser)
            {
                (control as APMBrowser).VerticalAlignment = VerticalAlignment.Center;
                (control as APMBrowser).HorizontalAlignment = HorizontalAlignment.Left;
                (control as APMBrowser).Margin = new Thickness(1);
            }
            else if (control is StackPanel)
            {
                var stackPanel = control as StackPanel;
                if (stackPanel.Orientation == Orientation.Horizontal)
                    stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                else
                    stackPanel.VerticalAlignment = VerticalAlignment.Center;
                stackPanel.Margin = new Thickness(0);
                foreach (object c in stackPanel.Children)
                    SetControlsProperties(c);
            }
            else if (control is Panel)
            {
                Panel panel = control as Panel;
                panel.Margin = new Thickness(0);
                panel.VerticalAlignment = VerticalAlignment.Center;
                foreach (object c in (control as Panel).Children)
                    SetControlsProperties(c);
            }

            else if (control is ContentControl)
                SetControlsProperties((control as ContentControl).Content);

            else if (control is ItemsControl)
                foreach (object c in (control as ItemsControl).Items)
                    SetControlsProperties(c);

        }
        #endregion

        #region ChangeControlsForReport
        private void ChangeControlsForReport()
        {
            ChangeControlsForReport(this);
            brwSelectAccount.XMultiSelect = false;
            brwSelectDetail.XMultiSelect = false;
            brwCardexGoodsSelectGoods.XMultiSelect = false;
            this.XCanCollapse = true;
        }
        private void ChangeControlsForReport(object control)
        {
            if (control == null)
                return;

            if (control is APMGroupBoxExtended)
            {
                (control as APMGroupBoxExtended).XCanClear = (bool)chkForReport.IsChecked;
                (control as APMGroupBoxExtended).XCanCollapse = false;
            }
            if (control is Panel)
                foreach (object c in (control as Panel).Children)
                    ChangeControlsForReport(c);

            else if (control is ContentControl)
                ChangeControlsForReport((control as ContentControl).Content);

            else if (control is ItemsControl)
                foreach (object c in (control as ItemsControl).Items)
                    ChangeControlsForReport(c);
        }
        #endregion

        #region CopyDataFromControlsToARecord
        public void CopyDataFromControlsToARecord<T>(T record)
        {
            /*----Base Document----*/
            GlobalFunctions.SetValueToProperty(record, FieldNames<T>.HaveBaseDocument, null);
            if ((bool)radHaveBase.IsChecked || (bool)radNoBase.IsChecked)
                GlobalFunctions.SetValueToProperty(record, FieldNames<T>.HaveBaseDocument, ((bool)radHaveBase.IsChecked));

            /*----Send Or Receive----*/
            GlobalFunctions.SetValueToProperty(record, FieldNames<T>.IsSend, chkSendOrReceive_Send.IsChecked.Value);
            GlobalFunctions.SetValueToProperty(record, FieldNames<T>.IsReceive, chkSendOrReceive_Receive.IsChecked.Value);

            /*----Accounting Document Status----*/
            if (chkAccountingDocumentStatusConfirm.IsChecked == true)
                GlobalFunctions.SetValueToProperty(record, FieldNames<T>.DocumentStatusIsConfirm,
                    (long)((AccDocumentStatus)chkAccountingDocumentStatusConfirm.Tag));
            else
                GlobalFunctions.SetValueToProperty(record, FieldNames<T>.DocumentStatusIsConfirm, null);

            if (chkAccountingDocumentStatusTemp.IsChecked == true)
                GlobalFunctions.SetValueToProperty(record, FieldNames<T>.DocumentStatusIsTemp,
                    (long)((AccDocumentStatus)chkAccountingDocumentStatusTemp.Tag));
            else
                GlobalFunctions.SetValueToProperty(record, FieldNames<T>.DocumentStatusIsTemp, null);

            if (chkAccountingDocumentStatusUseLess.IsChecked == true)
                GlobalFunctions.SetValueToProperty(record, FieldNames<T>.DocumentStatusIsUseLess,
                    (long)((AccDocumentStatus)chkAccountingDocumentStatusUseLess.Tag));
            else
                GlobalFunctions.SetValueToProperty(record, FieldNames<T>.DocumentStatusIsUseLess, null);

            /*---- Accounting Level Radios ----*/
            if (chkShowAccountingLevelRadios.IsChecked == true)
                if (radKol.IsChecked == true)
                    GlobalFunctions.SetValueToProperty<T, int>(record, FieldNames<T>.LevelNo, 2);
                else if (radMoein.IsChecked == true)
                    GlobalFunctions.SetValueToProperty<T, int>(record, FieldNames<T>.LevelNo, 3);
                else if (radDetail.IsChecked == true)
                    GlobalFunctions.SetValueToProperty<T, int>(record, FieldNames<T>.LevelNo, 4);
                else
                    GlobalFunctions.SetValueToProperty<T, int>(record, FieldNames<T>.LevelNo, 1);

            /*---- Accounting Level CheckBoxes ----*/
            if (chkShowAccountingLevelCheckBoxes.IsChecked == true)
            {
                string s = "";
                if (chkGroup.IsChecked == true)
                    s += "1";
                if (chkKol.IsChecked == true)
                    s += "2";
                if (chkMoein.IsChecked == true)
                    s += "3";
                if (chkDetail.IsChecked == true)
                    s += "4";
                GlobalFunctions.SetValueToProperty<T, string>(record, FieldNames<T>.LevelNos, s);
            }

        }
        #endregion

        #endregion

        #region SetBinding
        public void XSetBinding<T>(BindingList<T> bindingList)
        {
            /*-----Cardex-----*/
            brwCardexGoodsSelectGoods.XSetBinding(bindingList, FieldNames<T>.GroupGoodsId);

            /*-----Account And Detail-----*/
            brwSelectAccount.XSetBinding(bindingList, FieldNames<T>.ChartAccountId);
            brwSelectDetail.XSetBinding(bindingList, FieldNames<T>.DetailId);

            /*----Cardex Goods----*/
            cmbCardexGoodsSelectMeasure.Name = "cmb_" + FieldNames<T>.MeasureId;

            /*-----Main Information-----*/
            brwMainStore.XSetBinding(bindingList, FieldNames<T>.StoreId);
            cmbReceiveDocumentType.Name = "cmb_" + FieldNames<T>.ReceiveType;
            cmbSendDocumentType.Name = "cmb_" + FieldNames<T>.SendType;
            cmbAccountingDocumentType.Name = "cmb_" + FieldNames<T>.DocumentType;
            pdpDate.Name = "pdp_" + FieldNames<T>.Date;
            pdpFromDate.Name = "pdp_" + FieldNames<T>.FromDate;
            pdpToDate.Name = "pdp_" + FieldNames<T>.ToDate;

            /*-----------WardenShip------------*/
            txtWardenName.Name = "txt_" + FieldNames<T>.WardenName;
            brwAccountingPersonel.XSetBinding(bindingList, FieldNames<T>.PersonelId);


            /*----Physical Counting Date----*/
            pdpPhysicalStartDate.Name = "pdp_" + FieldNames<T>.PhysicalStartDate;
            pdpPhysicalEndDate.Name = "pdp_" + FieldNames<T>.PhysicalEndDate;

            /*-----FilterGoods-----*/
            brwFilterGoodsSelectGoods.XSetBinding(bindingList, FieldNames<T>.GroupGoodsId);
            txtFilterGoodsSumAmountFrom.Name = "txt_" + FieldNames<T>.SumAmountFrom;
            txtFilterGoodsSumAmountTo.Name = "txt_" + FieldNames<T>.SumAmountTo;
            txtFilterGoodsSumPriceFrom.Name = "txt_" + FieldNames<T>.SumPriceFrom;
            txtFilterGoodsSumPriceTo.Name = "txt_" + FieldNames<T>.SumPriceTo;

            /*----AccDocument Price----*/
            txtDocumentPriceFromCredit.Name = "txt_" + FieldNames<T>.DocumentFromCredit;
            txtDocumentPriceToCredit.Name = "txt_" + FieldNames<T>.DocumentToCredit;
            txtDocumentPriceFromDebt.Name = "txt_" + FieldNames<T>.DocumentFromDebt;
            txtDocumentPriceToDebt.Name = "txt_" + FieldNames<T>.DocumentToDebt;
            txtArticlePriceFromCredit.Name = "txt_" + FieldNames<T>.ArticleFromCredit;
            txtArticlePriceToCredit.Name = "txt_" + FieldNames<T>.ArticleToCredit;
            txtArticlePriceFromDebt.Name = "txt_" + FieldNames<T>.ArticleFromDebt;
            txtArticlePriceToDebt.Name = "txt_" + FieldNames<T>.ArticleToDebt;

            /*-----Description-----*/
            txtDescription.Name = "txt_" + FieldNames<T>.Description;

            /*-----Base Document-----*/
            brwBaseReceive.XSetBinding(bindingList, FieldNames<T>.ReceiveId);
            brwBaseGoodsRequest.XSetBinding(bindingList, FieldNames<T>.GoodsRequestId);
            brwBaseSend.XSetBinding(bindingList, FieldNames<T>.SendId);
            brwBaseBuyRequest.XSetBinding(bindingList, FieldNames<T>.BuyRequestId);

            /*----Destination Entity----*/
            brwDestinationDetail.XSetBinding(bindingList, FieldNames<T>.DestinationDetailId);

            /*----GoodsRequester----*/
            brwGoodsRequesterCostCenter.XSetBinding(bindingList, FieldNames<T>.CostCenterId);
            brwGoodsRequesterPersonel.XSetBinding(bindingList, FieldNames<T>.RequesterPersonelId);

            /*----GoodsRequest and BuyRequest----*/
            brwRequestConfirmerPerson.XSetBinding(bindingList, FieldNames<T>.ConfirmerPersonelId);
            pdpRequestDeadLineDate.Name = "pdp_" + FieldNames<T>.SendDeadLineDate;
            pdpRequestDeadLineFromDate.Name = "pdp_" + FieldNames<T>.SendDeadLineFromDate;
            pdpRequestDeadLineToDate.Name = "pdp_" + FieldNames<T>.SendDeadLineToDate;

            /*----Confirm----*/
            lblDocumentStatus.DataContext = bindingList;
            lblDocumentStatus.SetBinding(Label.ContentProperty, FieldNames<T>.DocumentStatusName);
            lblConfirmDate.DataContext = bindingList;
            lblConfirmDate.SetBinding(Label.ContentProperty, FieldNames<T>.ConfirmDate);
            lblConfirmTime.DataContext = bindingList;
            lblConfirmTime.SetBinding(Label.ContentProperty, FieldNames<T>.ConfirmTime);
            lblConfirmerUserName.DataContext = bindingList;
            lblConfirmerUserName.SetBinding(Label.ContentProperty, FieldNames<T>.ConfirmerUserName);
            brwConfirmerUser.XSetBinding(bindingList, FieldNames<T>.ConfirmerUserId);
            pdpConfirmFromDate.Name = "pdp_" + FieldNames<T>.ConfirmFromDate;
            pdpConfirmToDate.Name = "pdp_" + FieldNames<T>.ConfirmToDate;


            /*----Register----*/
            lblDocumentNo.DataContext = bindingList;
            lblDocumentNo.SetBinding(Label.ContentProperty, FieldNames<T>.DocumentNo);
            lblDocumentCode.DataContext = bindingList;
            lblDocumentCode.SetBinding(Label.ContentProperty, FieldNames<T>.Code);
            lblRegisterDate.DataContext = bindingList;
            lblRegisterDate.SetBinding(Label.ContentProperty, FieldNames<T>.RegisterDate);
            lblRegisterTime.DataContext = bindingList;
            lblRegisterTime.SetBinding(Label.ContentProperty, FieldNames<T>.RegisterTime);
            lblRegisterUserName.DataContext = bindingList;
            lblRegisterUserName.SetBinding(Label.ContentProperty, FieldNames<T>.RegistererUserName);
            txtDocumentNoFrom.Name = "txt_" + FieldNames<T>.DocumentNoFrom;
            txtDocumentNoTo.Name = "txt_" + FieldNames<T>.DocumentNoTo;
            txtDocumentCodeFrom.Name = "txt_" + FieldNames<T>.CodeFrom;
            txtDocumentCodeTo.Name = "txt_" + FieldNames<T>.CodeTo;
            pdpRegisterFromDate.Name = "pdp_" + FieldNames<T>.RegisterFromDate;
            pdpRegisterToDate.Name = "pdp_" + FieldNames<T>.RegisterToDate;
            brwRegistererUser.XSetBinding(bindingList, FieldNames<T>.RegistererUserId);

            /*----Fiscal year----*/
            //cmbFiscalYear.Name = "cmb_" + FieldNames<T>.FiscalYear;

        }
        #endregion

        #region Properties

        #region Type
        private DocumentTypes _xtype;
        public DocumentTypes XType
        {
            get
            {
                return _xtype;
            }
            set
            {
                _xtype = value;
                SetCheckBoxes();
                SetHeaders();
            }
        }
        private void SetCheckBoxes()
        {
            chkForReport.IsChecked =
                XType == DocumentTypes.AccountBalanceReport ||
                XType == DocumentTypes.NoteBooksReport ||
                XType == DocumentTypes.Balance4Columns ||
                XType == DocumentTypes.KolNoteBookRule ||
                XType == DocumentTypes.StockReport ||
                XType == DocumentTypes.CardexReport ||
                XType == DocumentTypes.SendOrReceiveReport ||
                XType == DocumentTypes.SendReport ||
                XType == DocumentTypes.ReceiveReport ||
                XType == DocumentTypes.BuyRequestReport ||
                XType == DocumentTypes.GoodsRequestReport ||
                XType == DocumentTypes.OpeningReport;

            chkShowGoodsRequest.IsChecked =
                XType == DocumentTypes.GoodsRequest ||
                XType == DocumentTypes.GoodsRequestReport;

            chkShowPhisycalCountingControls.IsChecked =
                XType == DocumentTypes.PhysicalCounting;

            chkShowConfirm.IsChecked =
                XType == DocumentTypes.AccountBalanceReport ||
                XType == DocumentTypes.AccountingDocument ||
                XType == DocumentTypes.NoteBooksReport;

            chkShowAccountingDocumentType.IsChecked =
                XType == DocumentTypes.AccountBalanceReport ||
                XType == DocumentTypes.AccountingDocument ||
                XType == DocumentTypes.KolNoteBookRule ||
                XType == DocumentTypes.Balance4Columns ||
                XType == DocumentTypes.NoteBooksReport;

            chkShowRequest.IsChecked =
                XType == DocumentTypes.GoodsRequest ||
                XType == DocumentTypes.GoodsRequestReport ||
                XType == DocumentTypes.BuyRequest ||
                XType == DocumentTypes.BuyRequestReport;

            chkShowReceiveType.IsChecked =
                XType == DocumentTypes.Receive ||
                XType == DocumentTypes.ReceiveReport ||
                XType == DocumentTypes.StockReport ||
                XType == DocumentTypes.CardexReport ||
                XType == DocumentTypes.SendOrReceiveReport;

            chkShowSendType.IsChecked =
                XType == DocumentTypes.Send ||
                XType == DocumentTypes.SendReport ||
                XType == DocumentTypes.StockReport ||
                XType == DocumentTypes.CardexReport ||
                XType == DocumentTypes.SendOrReceiveReport;

            chkShowAccountingPrice.IsChecked =
                XType == DocumentTypes.AccountBalanceReport ||
                XType == DocumentTypes.NoteBooksReport;

            chkShowAccount.IsChecked =
                XType == DocumentTypes.AccountBalanceReport ||
                XType == DocumentTypes.Balance4Columns ||
                XType == DocumentTypes.NoteBooksReport;

            chkShowAccountingDocumentStatus.IsChecked =
                XType == DocumentTypes.AccountBalanceReport ||
                XType == DocumentTypes.KolNoteBookRule ||
                XType == DocumentTypes.Balance4Columns ||
                XType == DocumentTypes.NoteBooksReport;

            chkShowBaseDocument.IsChecked =
                XType == DocumentTypes.BuyRequest ||
                XType == DocumentTypes.BuyRequestReport ||
                XType == DocumentTypes.Send ||
                XType == DocumentTypes.SendReport ||
                XType == DocumentTypes.Receive ||
                XType == DocumentTypes.ReceiveReport;

            chkShowStkDocumentNo.IsChecked =
                chkForReport.IsChecked == true &&
                (
                    XType == DocumentTypes.Send ||
                    XType == DocumentTypes.Receive ||
                    XType == DocumentTypes.Opening ||
                    XType == DocumentTypes.Closing ||
                    XType == DocumentTypes.StockReport ||
                    XType == DocumentTypes.PhysicalCounting ||
                    XType == DocumentTypes.CardexReport ||
                    XType == DocumentTypes.SendOrReceiveReport ||
                    XType == DocumentTypes.SendReport ||
                    XType == DocumentTypes.ReceiveReport ||
                    XType == DocumentTypes.OpeningReport ||
                    XType == DocumentTypes.AccountingDocument ||
                    XType == DocumentTypes.AccountBalanceReport ||
                    XType == DocumentTypes.NoteBooksReport ||
                    XType == DocumentTypes.KolNoteBookRule ||
                    XType == DocumentTypes.Balance4Columns
                );

            chkShowlblDocumentNo.IsChecked =
                chkForReport.IsChecked == false &&
                (
                    XType == DocumentTypes.Send ||
                    XType == DocumentTypes.Receive ||
                    XType == DocumentTypes.Opening ||
                    XType == DocumentTypes.Closing ||
                    XType == DocumentTypes.StockReport ||
                    XType == DocumentTypes.CardexReport ||
                    XType == DocumentTypes.SendOrReceiveReport ||
                    XType == DocumentTypes.SendReport ||
                    XType == DocumentTypes.ReceiveReport ||
                    XType == DocumentTypes.OpeningReport ||
                    XType == DocumentTypes.AccountingDocument ||
                    XType == DocumentTypes.AccountBalanceReport ||
                    XType == DocumentTypes.NoteBooksReport ||
                    XType == DocumentTypes.KolNoteBookRule ||
                    XType == DocumentTypes.Balance4Columns
                );

            chkShowTitleOfDocumentNo.IsChecked =
                XType == DocumentTypes.Send ||
                XType == DocumentTypes.Receive ||
                XType == DocumentTypes.Opening ||
                XType == DocumentTypes.Closing ||
                XType == DocumentTypes.StockReport ||
                XType == DocumentTypes.CardexReport ||
                XType == DocumentTypes.SendOrReceiveReport ||
                XType == DocumentTypes.SendReport ||
                XType == DocumentTypes.ReceiveReport ||
                XType == DocumentTypes.OpeningReport ||
                XType == DocumentTypes.AccountingDocument ||
                XType == DocumentTypes.AccountBalanceReport ||
                XType == DocumentTypes.NoteBooksReport ||
                XType == DocumentTypes.KolNoteBookRule ||
                XType == DocumentTypes.Balance4Columns;

            chkShowMainStore.IsChecked =
                XType == DocumentTypes.BuyRequest ||
                XType == DocumentTypes.BuyRequestReport ||
                XType == DocumentTypes.GoodsRequest ||
                XType == DocumentTypes.GoodsRequestReport ||
                XType == DocumentTypes.CardexReport ||
                XType == DocumentTypes.Closing ||
                XType == DocumentTypes.Opening ||
                XType == DocumentTypes.OpeningReport ||
                XType == DocumentTypes.PhysicalCounting ||
                XType == DocumentTypes.Receive ||
                XType == DocumentTypes.ReceiveReport ||
                XType == DocumentTypes.Send ||
                XType == DocumentTypes.SendReport ||
                XType == DocumentTypes.StockReport;

            chkShowDestinationEntity.IsChecked =
                XType == DocumentTypes.Send ||
                XType == DocumentTypes.SendReport ||
                XType == DocumentTypes.Receive ||
                XType == DocumentTypes.ReceiveReport ||
                XType == DocumentTypes.StockReport ||
                XType == DocumentTypes.CardexReport ||
                XType == DocumentTypes.SendOrReceiveReport;


            chkShowCardexGoods.IsChecked =
                XType == DocumentTypes.CardexReport;

            chkShowGoodsFilter.IsChecked =
                XType == DocumentTypes.StockReport ||
                XType == DocumentTypes.BuyRequestReport ||
                XType == DocumentTypes.GoodsRequestReport ||
                XType == DocumentTypes.OpeningReport ||
                XType == DocumentTypes.ReceiveReport ||
                XType == DocumentTypes.SendReport;

            chkShowSendOrReceive.IsChecked =
                XType == DocumentTypes.SendOrReceiveReport;

            chkShowAccountingLevelRadios.IsChecked =
                XType == DocumentTypes.NoteBooksReport;

            chkShowAccountingLevelCheckBoxes.IsChecked =
                XType == DocumentTypes.Balance4Columns;

            /*----Base RadioButtons----*/

            chkShowBaseReceive.IsChecked =
                XType == DocumentTypes.Send;

            chkShowBaseSendAndBaseBuyRequest.IsChecked =
                XType == DocumentTypes.Receive;

            chkShowBaseGoodsRequest.IsChecked =
                XType == DocumentTypes.Send ||
                XType == DocumentTypes.BuyRequest;

            chkShowBaseDocumentTypeSelect.IsChecked =
                radHaveBase.IsChecked == true &&
                chkForReport.IsChecked == false;
        }
        private void SetHeaders()
        {
            Header = "تعیین مشخصات ";
            switch (_xtype)
            {

                /*---- Inv Documents ----*/
                case DocumentTypes.Receive:
                case DocumentTypes.ReceiveReport:
                    Header += "رسید";
                    break;

                case DocumentTypes.Send:
                case DocumentTypes.SendReport:
                    Header += "حواله";
                    break;

                case DocumentTypes.GoodsRequest:
                case DocumentTypes.GoodsRequestReport:
                    Header += "درخواست کالا";
                    break;

                case DocumentTypes.BuyRequest:
                case DocumentTypes.BuyRequestReport:
                    Header += "درخواست خرید";
                    break;

                case DocumentTypes.Opening:
                case DocumentTypes.OpeningReport:
                    Header += "سند افتتاحیه";
                    break;

                case DocumentTypes.Closing:
                    Header += "سند اختتامیه";
                    break;

                case DocumentTypes.PhysicalCounting:
                    Header += "سند انبارگردانی";
                    break;


                /*---- Inv Reports ----*/
                case DocumentTypes.StockReport:
                case DocumentTypes.CardexReport:
                case DocumentTypes.SendOrReceiveReport:
                    Header = "فیلتر کردن گزارش";
                    break;


                /*---- Accounting ----*/
                case DocumentTypes.KolNoteBookRule:
                case DocumentTypes.NoteBooksReport:
                case DocumentTypes.Balance4Columns:
                case DocumentTypes.AccountBalanceReport:
                case DocumentTypes.AccountingDocument:
                    Header += "سند حسابداری";
                    lblDocumentCodeTitle.Content = "شمارۀ عطف :";
                    lblDocumentNoTitle.Content = "شمارۀ سند :";
                    break;
            }
        }
        #endregion

        #region BaseDocumentType
        public DocumentTypes? XBaseDocumentType
        {
            get
            {
                if (radBaseSend.IsChecked == true)
                    return DocumentTypes.Send;
                else if (radBaseBuyRequest.IsChecked == true)
                    return DocumentTypes.BuyRequest;
                else if (radBaseGoodsRequest.IsChecked == true)
                    return DocumentTypes.GoodsRequest;
                else if (radBaseReceive.IsChecked == true)
                    return DocumentTypes.Receive;
                else return null;
            }
        }
        #endregion

        #region HaveBaseDocument
        private Boolean _haveBaseDocument;
        private Boolean ChangingIsEnable = true;
        public Boolean XHaveBaseDocument
        {
            get
            {
                return (_haveBaseDocument);
            }
            set
            {
                if (ChangingIsEnable == false)
                    return;
                ChangingIsEnable = false;
                _haveBaseDocument = value;
                if (value == false)
                {
                    foreach (object control in stkBaseDocumentType.Children)
                        if (control is RadioButton)
                            (control as RadioButton).IsChecked = false;
                    radNoBase.IsChecked = true;
                }
                else
                {
                    radHaveBase.IsChecked = true;

                    if (brwBaseBuyRequest.XTextBox.Text != "")
                        radBaseBuyRequest.IsChecked = true;

                    else if (brwBaseGoodsRequest.XTextBox.Text != "")
                        radBaseGoodsRequest.IsChecked = true;

                    else if (brwBaseSend.XTextBox.Text != "")
                        radBaseSend.IsChecked = true;

                    else if (brwBaseReceive.XTextBox.Text != "")
                        radBaseReceive.IsChecked = true;
                }
                ChangingIsEnable = true;
            }
        }
        #endregion

        #region BaseDocumentIsSendOrReceive
        public Boolean XBasedocumentIsSendOrReceive
        {
            get
            {
                return radBaseReceive.IsChecked == true || radBaseSend.IsChecked == true;
            }
        }
        #endregion

        #endregion

        #region Events

        #region ExternalEvents

        #region BrowseClick

        /*----Cardex Goods----*/
        public event RoutedEventHandler XBrowseClick_CardexGoods
        {
            add
            {
                brwCardexGoodsSelectGoods.XBrowseClick += value;
            }
            remove
            {
                brwCardexGoodsSelectGoods.XBrowseClick -= value;
            }
        }

        /*----Account----*/
        public event RoutedEventHandler XBrowseClick_SelectAccount
        {
            add
            {
                brwSelectAccount.XBrowseClick += value;
            }
            remove
            {
                brwSelectAccount.XBrowseClick -= value;
            }
        }
        public event RoutedEventHandler XBrowseClick_SelectDetail
        {
            add
            {
                brwSelectDetail.XBrowseClick += value;
            }
            remove
            {
                brwSelectDetail.XBrowseClick -= value;
            }
        }

        /*----Main Information----*/
        public event RoutedEventHandler XBrowseClick_MainStore
        {
            add
            {
                brwMainStore.XBrowseClick += value;
            }
            remove
            {
                brwMainStore.XBrowseClick -= value;
            }
        }

        /*----WardenShip----*/
        public event RoutedEventHandler XBrowseClick_AccountingPersonel
        {
            add
            {
                brwAccountingPersonel.XBrowseClick += value;
            }
            remove
            {
                brwAccountingPersonel.XBrowseClick -= value;
            }
        }

        /*----Filter Goods----*/
        public event RoutedEventHandler XBrowseClick_SelectGoods
        {
            add
            {
                brwFilterGoodsSelectGoods.XBrowseClick += value;
            }
            remove
            {
                brwFilterGoodsSelectGoods.XBrowseClick -= value;
            }
        }

        /*----Base Document----*/
        public event RoutedEventHandler XBrowseClick_BaseReceive
        {
            add
            {
                brwBaseReceive.XBrowseClick += value;
            }
            remove
            {
                brwBaseReceive.XBrowseClick -= value;
            }
        }
        public event RoutedEventHandler XBrowseClick_BaseGoodsRequest
        {
            add
            {
                brwBaseGoodsRequest.XBrowseClick += value;
            }
            remove
            {
                brwBaseGoodsRequest.XBrowseClick -= value;
            }
        }
        public event RoutedEventHandler XBrowseClick_BaseSend
        {
            add
            {
                brwBaseSend.XBrowseClick += value;
            }
            remove
            {
                brwBaseSend.XBrowseClick -= value;
            }
        }
        public event RoutedEventHandler XBrowseClick_BaseBuyRequest
        {
            add
            {
                brwBaseBuyRequest.XBrowseClick += value;
            }
            remove
            {
                brwBaseBuyRequest.XBrowseClick -= value;
            }
        }

        /*----Destination----*/
        public event RoutedEventHandler XBrowseClick_DestinationDetail
        {
            add
            {
                brwDestinationDetail.XBrowseClick += value;
            }
            remove
            {
                brwDestinationDetail.XBrowseClick -= value;
            }
        }

        /*----Goods Request----*/
        public event RoutedEventHandler XBrowseClick_GoodsRequesterPersonel
        {
            add
            {
                brwGoodsRequesterPersonel.XBrowseClick += value;
            }
            remove
            {
                brwGoodsRequesterPersonel.XBrowseClick -= value;
            }
        }
        public event RoutedEventHandler XBrowseClick_GoodsRequesterCostCenter
        {
            add
            {
                brwGoodsRequesterCostCenter.XBrowseClick += value;
            }
            remove
            {
                brwGoodsRequesterCostCenter.XBrowseClick -= value;
            }
        }

        /*----Request----*/
        public event RoutedEventHandler XBrowseClick_RequestConfirmerPersonel
        {
            add
            {
                brwRequestConfirmerPerson.XBrowseClick += value;
            }
            remove
            {
                brwRequestConfirmerPerson.XBrowseClick -= value;
            }
        }

        /*----Confirm---*/
        public event RoutedEventHandler XBrowseClick_ConfirmerUser
        {
            add
            {
                brwConfirmerUser.XBrowseClick += value;
            }
            remove
            {
                brwConfirmerUser.XBrowseClick -= value;
            }
        }

        /*----Register----*/
        public event RoutedEventHandler XBrowseClick_RegistererUser
        {
            add
            {
                brwRegistererUser.XBrowseClick += value;
            }
            remove
            {
                brwRegistererUser.XBrowseClick -= value;
            }
        }
        #endregion

        #region BaseDocument_Changed
        public event RoutedEventHandler XBaseDocument_Changed
        {
            add
            {
                radNoBase.Checked += value;
                radHaveBase.Checked += value;
                radBaseSend.Checked += value;
                radBaseReceive.Checked += value;
                radBaseBuyRequest.Checked += value;
                radBaseGoodsRequest.Checked += value;
            }
            remove
            {
                radHaveBase.Checked -= value;
                radHaveBase.Unchecked -= value;
                radBaseSend.Checked -= value;
                radBaseReceive.Checked -= value;
                radBaseBuyRequest.Checked -= value;
                radBaseGoodsRequest.Checked -= value;
            }

        }
        #endregion

        #region CodeTextBox_KeyDown
        public event KeyEventHandler XTextBoxKeyDown_MainStore
        {
            add
            {
                brwMainStore.XTextBox.KeyDown += value;
            }
            remove
            {
                brwMainStore.XTextBox.KeyDown -= value;
            }
        }

        public event KeyEventHandler XTextBoxKeyDown_SelectGoods
        {
            add
            {
                brwFilterGoodsSelectGoods.XTextBox.KeyDown += value;
            }
            remove
            {
                brwFilterGoodsSelectGoods.XTextBox.KeyDown -= value;
            }
        }

        public event KeyEventHandler XTextBoxKeyDown_BaseReceive
        {
            add
            {
                brwBaseReceive.XTextBox.KeyDown += value;
            }
            remove
            {
                brwBaseReceive.XTextBox.KeyDown -= value;
            }
        }
        public event KeyEventHandler XTextBoxKeyDown_BaseGoodsRequest
        {
            add
            {
                brwBaseGoodsRequest.XTextBox.KeyDown += value;
            }
            remove
            {
                brwBaseGoodsRequest.XTextBox.KeyDown -= value;
            }
        }
        public event KeyEventHandler XTextBoxKeyDown_BaseSend
        {
            add
            {
                brwBaseSend.XTextBox.KeyDown += value;
            }
            remove
            {
                brwBaseSend.XTextBox.KeyDown -= value;
            }
        }
        public event KeyEventHandler XTextBoxKeyDown_BaseBuyRequest
        {
            add
            {
                brwBaseBuyRequest.XTextBox.KeyDown += value;
            }
            remove
            {
                brwBaseBuyRequest.XTextBox.KeyDown -= value;
            }
        }

        public event KeyEventHandler XTextBoxKeyDown_DestinationDetail
        {
            add
            {
                brwDestinationDetail.XTextBox.KeyDown += value;
            }
            remove
            {
                brwDestinationDetail.XTextBox.KeyDown -= value;
            }
        }

        public event KeyEventHandler XTextBoxKeyDown_GoodsRequesterPersonel
        {
            add
            {
                brwGoodsRequesterPersonel.XTextBox.KeyDown += value;
            }
            remove
            {
                brwGoodsRequesterPersonel.XTextBox.KeyDown -= value;
            }
        }
        public event KeyEventHandler XTextBoxKeyDown_GoodsRequesterCostCenter
        {
            add
            {
                brwGoodsRequesterCostCenter.XTextBox.KeyDown += value;
            }
            remove
            {
                brwGoodsRequesterCostCenter.XTextBox.KeyDown -= value;
            }
        }
        public event KeyEventHandler XTextBoxKeyDown_RequestConfirmerPersonel
        {
            add
            {
                brwRequestConfirmerPerson.XTextBox.KeyDown += value;

            }
            remove
            {
                brwRequestConfirmerPerson.XTextBox.KeyDown -= value;
            }
        }
        public event KeyEventHandler XTextBoxKeyDown_Registerer_User
        {
            add
            {
                brwRegistererUser.XTextBox.KeyDown += value;
            }
            remove
            {
                brwRegistererUser.XTextBox.KeyDown -= value;
            }
        }
        #endregion

        #endregion

        #region Intrenal Events
        private void chkForReport_Checked(object sender, RoutedEventArgs e)
        {
            ChangeControlsForReport();
        }
        private void APMDocumentHeader_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((brwBaseBuyRequest.Visibility != Visibility.Visible || brwBaseBuyRequest.XTextBox.Text == "") &&
                (brwBaseReceive.Visibility != Visibility.Visible || brwBaseReceive.XTextBox.Text == "") &&
                (brwBaseGoodsRequest.Visibility != Visibility.Visible || brwBaseGoodsRequest.XTextBox.Text == "") &&
                (brwBaseSend.Visibility != Visibility.Visible || brwBaseSend.XTextBox.Text == ""))
                XHaveBaseDocument = false;
        }
        private void txtBaseDocument_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox))
                return;
            var textBox = sender as TextBox;
            if (textBox.Text == null || textBox.Text.Trim() == "")
            {
                if (textBox.IsEnabled == true)
                    return;
                XHaveBaseDocument = false;
            }
            else
                XHaveBaseDocument = true;
        }
        private void radHaveBase_CheckedChanged(object sender, RoutedEventArgs e)
        {
            XHaveBaseDocument = (bool)radHaveBase.IsChecked;
            chkShowBaseDocumentTypeSelect.IsChecked =
                    radHaveBase.IsChecked == true &&
                    chkForReport.IsChecked == false;
        }

        #endregion

        #endregion
    }
    #endregion

    #region APM (MoneyLabel)
    public class APMMoneyLabel : APMInfoLabel
    {
        public APMMoneyLabel()
        {
            XHaveBorder = true;
            ContentStringFormat = "N0";
        }
    }

    #endregion

    #region APM (ScrollViewer)
    public class APMScrollViewer : ScrollViewer
    {
        public APMScrollViewer()
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            FlowDirection = FlowDirection.RightToLeft;
        }
    }
    #endregion

    #region APM (Tree)
    public class APMTree : TreeView
    {
        #region Variables
        private ContextMenu contextMenu = new ContextMenu();
        APMMenuItem mnuInsert = new APMMenuItem(ButtonImageType.Insert, "ایجاد زیر مجموعه");
        APMMenuItem mnuInsertEntity = new APMMenuItem(ButtonImageType.Insert, "");
        APMMenuItem mnuInsertGroup = new APMMenuItem(ButtonImageType.Insert, "");
        APMMenuItem mnuEdit = new APMMenuItem(ButtonImageType.Edit, "ویرایش این عنصر");
        APMMenuItem mnuDelete = new APMMenuItem(ButtonImageType.Delete, "حذف این عنصر");
        #endregion

        #region Constructor
        public APMTree()
        {
            XHaveContextMenu = true;
            XIsTwoTable = false;
            XLevelCount = 0;
            IsTextSearchEnabled = true;
            Background = Brushes.Transparent;
        }

        #endregion

        #region External Events
        public event RoutedEventHandler XInsertChildClick
        {
            add
            {
                mnuInsert.Click += value;
                mnuInsertGroup.Click += value;
            }
            remove
            {
                mnuInsert.Click -= value;
                mnuInsertGroup.Click -= value;
            }
        }
        public event RoutedEventHandler XInsertChildClick_Entity
        {
            add
            {
                mnuInsertEntity.Click += value;
            }
            remove
            {
                mnuInsertEntity.Click -= value;
            }
        }
        public event RoutedEventHandler XEditClick
        {
            add
            {
                mnuEdit.Click += value;
            }
            remove
            {
                mnuEdit.Click -= value;
            }
        }
        public event RoutedEventHandler XDeleteClick
        {
            add
            {
                mnuDelete.Click += value;
            }
            remove
            {
                mnuDelete.Click -= value;
            }
        }
        #endregion

        #region Properties

        #region ContextMenu
        private void CreateContextMenu()
        {
            mnuInsertGroup.Header = "ایجاد " + XGroupTitle + " زیر مجموعه";
            mnuInsertEntity.Header = "ایجاد " + XEntityTitle + " زیر مجموعه";

            contextMenu.Items.Clear();
            if (XIsTwoTable)
            {
                contextMenu.Items.Add(mnuInsertGroup);
                contextMenu.Items.Add(mnuInsertEntity);
            }
            else
                contextMenu.Items.Add(mnuInsert);
            contextMenu.Items.Add(mnuEdit);
            contextMenu.Items.Add(mnuDelete);
        }
        private Boolean _haveContextMenu;
        public Boolean XHaveContextMenu
        {
            get
            {
                return _haveContextMenu;
            }
            set
            {
                _haveContextMenu = value;
                if (_haveContextMenu)
                {
                    CreateContextMenu();
                    ContextMenu = contextMenu;
                }
                else
                    ContextMenu = null;
            }
        }
        #endregion

        #region EntityTitle
        private string _entityTitle = "";
        public string XEntityTitle
        {
            get
            {
                return _entityTitle;
            }
            set
            {
                _entityTitle = value;
                CreateContextMenu();
            }
        }
        #endregion

        #region GroupTitle
        private string _groupTitle = "";
        public string XGroupTitle
        {
            get
            {
                return _groupTitle;
            }
            set
            {
                _groupTitle = value;
                CreateContextMenu();
            }
        }
        #endregion

        #region IsTwoTable
        private Boolean _isTwoTable = false;
        public Boolean XIsTwoTable
        {
            get
            {
                return _isTwoTable;
            }
            set
            {
                _isTwoTable = value;
                CreateContextMenu();
            }
        }
        #endregion

        #region TreeType
        TreeType _treeType;
        public TreeType XTreeType
        {
            get
            {
                return _treeType;
            }
            set
            {
                _treeType = value;
            }
        }
        #endregion

        #region Caption
        private string _caption = "";
        public string XCaption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                if (Items != null && Items.Count > 0)
                    (Items[0] as APMTreeViewItem).XCaption = value;
            }
        }
        #endregion

        #region LevelCount
        public int XLevelCount = 0;

        #endregion
        #endregion
    }
    #endregion

    #region APM (BindingList)
    public class APMBindingList<T> : BindingList<T>
    {
        ////This bindingList is sortable and also has a Find method
        //public APMBindingList()
        //{

        //}
        //public override void EndNew(int itemIndex)
        //{
        //    if (sortPropertyValue != null && itemIndex > 0
        //        && itemIndex == this.Count - 1)
        //        ApplySortCore(this.sortPropertyValue, this.sortDirectionValue);
        //    base.EndNew(itemIndex);
        //}

        //ListSortDirection sortDirectionValue;
        //PropertyDescriptor sortPropertyValue;

        //protected override PropertyDescriptor SortPropertyCore
        //{
        //    get { return sortPropertyValue; }
        //}

        //protected override ListSortDirection SortDirectionCore
        //{
        //    get { return sortDirectionValue; }
        //}

        //public ListSortDirection SortDirection
        //{
        //    get { return sortDirectionValue; }
        //}

        //protected override bool SupportsSortingCore
        //{
        //    get { return true; }
        //}

        //public void RemoveSort()
        //{
        //    RemoveSortCore();
        //}

        //private ArrayList sortedList;
        //ArrayList unsortedItems;

        //protected override void ApplySortCore(PropertyDescriptor prop,
        //    ListSortDirection direction)
        //{
        //    sortedList = new ArrayList();

        //    // Check to see if the property type we are sorting by implements
        //    // the IComparable interface.
        //    if (prop == null)
        //        return;
        //    Type interfaceType = prop.PropertyType.GetInterface("IComparable");

        //    //  if (interfaceType != null)
        //    // {
        //    // If so, set the SortPropertyValue and SortDirectionValue.
        //    sortPropertyValue = prop;
        //    sortDirectionValue = direction;

        //    unsortedItems = new ArrayList(this.Count);

        //    // Loop through each item, adding it the the sortedItems ArrayList.
        //    foreach (Object item in this.Items)
        //    {
        //        sortedList.Add(prop.GetValue(item));
        //        unsortedItems.Add(item);
        //    }

        //    // Call Sort on the ArrayList.
        //    sortedList.Sort();
        //    T temp;

        //    // Check the sort direction and then copy the sorted items
        //    // back into the list.
        //    if (direction == ListSortDirection.Descending)
        //        sortedList.Reverse();
        //    for (int i = 0; i < this.Count; i++)
        //    {
        //        if (sortedList[i] != null)
        //        {
        //            int position = Find(prop.Name, sortedList[i]);
        //            if (position != i)
        //            {
        //                temp = this[i];
        //                this[i] = this[position];
        //                this[position] = temp;
        //            }
        //        }
        //    }
        //    // Raise the ListChanged event so bound controls refresh their
        //    // values.
        //    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        //    //}
        //    //  else
        //    // If the property type does not implement IComparable, let the user
        //    // know.
        //    //  throw new NotSupportedException("Cannot sort by " + prop.Name + ". This" +
        //    //       prop.PropertyType.ToString() + " does not implement IComparable");
        //}

        //public void ApplySort(Microsoft.Windows.Controls.DataGridSortingEventArgs e)
        //{
        //    ListSortDirection dir = new ListSortDirection();
        //    if (e.Column.SortDirection == ListSortDirection.Ascending)
        //        dir = ListSortDirection.Ascending;
        //    else
        //        dir = ListSortDirection.Descending;
        //    ApplySortCore(TypeDescriptor.GetProperties(this.ElementAt(0))[e.Column.SortMemberPath], dir);
        //}

        //protected override void RemoveSortCore()
        //{
        //    int position;
        //    object temp;
        //    // Ensure the list has been sorted.
        //    if (unsortedItems != null)
        //    {
        //        // Loop through the unsorted items and reorder the
        //        // list per the unsorted list.
        //        for (int i = 0; i < unsortedItems.Count; )
        //        {
        //            position = this.Find(SortPropertyCore.Name,
        //                unsortedItems[i].GetType().
        //                GetProperty(SortPropertyCore.Name).
        //                GetValue(unsortedItems[i], null));
        //            if (position >= 0 && position != i)
        //            {
        //                temp = this[i];
        //                this[i] = this[position];
        //                this[position] = (T)temp;
        //                i++;
        //            }
        //            else if (position == i)
        //                i++;
        //            else
        //                // If an item in the unsorted list no longer exists, delete it.
        //                unsortedItems.RemoveAt(i);
        //        }
        //        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        //    }
        //}

        //protected override bool SupportsSearchingCore
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        //protected override int FindCore(PropertyDescriptor prop, object key)
        //{
        //    // Get the property info for the specified property.
        //    PropertyInfo propInfo = typeof(T).GetProperty(prop.Name);
        //    T item;

        //    if (key != null)
        //    {
        //        // Loop through the the items to see if the key
        //        // value matches the property value.

        //        for (int i = 0; i < Count; ++i)
        //        {
        //            item = (T)Items[i];
        //            var value = propInfo.GetValue(item, null);
        //            if (value != null && value.Equals(key))
        //                return i;
        //        }
        //    }
        //    return -1;
        //}

        //public int Find(string property, object key)
        //{
        //    // Check the properties for a property with the specified name.
        //    PropertyDescriptorCollection properties =
        //    TypeDescriptor.GetProperties(typeof(T));
        //    PropertyDescriptor prop = properties.Find(property, true);
        //    // If there is not a match, return -1 otherwise pass search to
        //    // FindCore method.
        //    if (prop == null)
        //        return -1;
        //    else
        //        return FindCore(prop, key);
        //}
    }
    #endregion

    #region APM (GroupBoxExtended)
    public class APMGroupBoxExtended : APMGroupBox
    {
        #region Controls
        Boolean collapsed = false;
        APMToolbarButton btnClear = new APMToolbarButton() { Visibility = Visibility.Collapsed, XImage = ButtonImageType.Cancel, Height = 23, ToolTip = "حذف فیلتر", XCanMagnify = false };
        APMToolbarButton btnCollapse = new APMToolbarButton() { Visibility = Visibility.Visible, XImage = ButtonImageType.First, Height = 23, ToolTip = "بستن کادر", XCanMagnify = false };
        StackPanel stkHeader = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(0) };
        APMLabel lblHeader = new APMLabel() { Margin = new Thickness(0) };
        #endregion

        #region Constructor
        public APMGroupBoxExtended()
        {
            base.Header = stkHeader;
            stkHeader.Children.Clear();
            stkHeader.Children.Add(lblHeader);
            stkHeader.Children.Add(btnClear);
            stkHeader.Children.Add(btnCollapse);
            btnClear.Click += btnClear_Click;
            btnCollapse.Click += CollapseOrExpand;
        }
        #endregion

        #region Tools


        #region CollapseOrExpand
        private void CollapseOrExpand(object sender, RoutedEventArgs e)
        {
            collapsed = !collapsed;
            GlobalFunctions.SetVisibilityForControl(Content as UIElement, !collapsed);
            btnCollapse.XImage = (collapsed) ? ButtonImageType.Last : ButtonImageType.First;
            btnCollapse.Height = 23;
        }

        #endregion

        #region Collapse
        public void Collapse()
        {
            collapsed = true;
            GlobalFunctions.SetVisibilityForControl(Content as UIElement, !collapsed);
            btnCollapse.XImage = (collapsed) ? ButtonImageType.Last : ButtonImageType.First;
            btnCollapse.Height = 23;
        }
        #endregion


        #region ClearControls
        void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearControls(this);
                if (OnClearfilter != null)
                    OnClearfilter();
            }
            catch (Exception exception)
            {
                Messages.ExceptionMessage(exception);
            }
        }
        private void ClearControls(object control)
        {
            if (control == null)
                return;
            else if (control is APMBrowser)
                (control as APMBrowser).XClear();

            else if (control is ToggleButton && (control as ToggleButton).Visibility == Visibility.Visible)
                (control as ToggleButton).IsChecked = false;

            else if (control is ComboBox)
                (control as ComboBox).SelectedIndex = -1;

            else if (control is PersianDatePicker)
                (control as PersianDatePicker).Text = "";

            else if (control is TextBox)
            {
                var BindingName = (control as TextBox).GetBindingExpression(TextBox.TextProperty);
                (control as TextBox).Text = null;

                if (BindingName != null)
                    BindingName.UpdateSource();
            }

            else if (control is Panel)
                foreach (object c in (control as Panel).Children)
                    ClearControls(c);

            else if (control is ContentControl)
                ClearControls((control as ContentControl).Content);

            else if (control is ItemsControl)
                foreach (object c in (control as ItemsControl).Items)
                    ClearControls(c);
        }
        #endregion

        #endregion

        #region Properties

        #region Header
        public new string Header
        {
            get
            {
                return (string)lblHeader.Content;
            }
            set
            {
                lblHeader.Content = value;
            }
        }
        #endregion

        #region XCanClear
        public Boolean XCanClear
        {
            get
            {
                return btnClear.Visibility == Visibility.Visible;
            }
            set
            {
                GlobalFunctions.SetVisibilityForControl(btnClear, value);
                if (base.Header is string)
                    lblHeader.Content = base.Header;
                base.Header = stkHeader;
            }
        }
        #endregion

        #region XCanCollapse
        public Boolean XCanCollapse
        {
            get
            {
                return btnCollapse.Visibility == Visibility.Visible;
            }
            set
            {
                GlobalFunctions.SetVisibilityForControl(btnCollapse, value);
                if (base.Header is string)
                    lblHeader.Content = base.Header;
                base.Header = stkHeader;
            }
        }
        #endregion

        public delegate void OnClearfilterDelegate();
        public event OnClearfilterDelegate OnClearfilter;

        #endregion
    }
    #endregion

    #region APM (Browser)
    public class APMBrowser : StackPanel
    {
        #region Controls
        APMToolbarButton btnBrowse = new APMToolbarButton() { XImage = ButtonImageType.Browse, VerticalAlignment = VerticalAlignment.Center };
        APMIntTextBox txtCode = new APMIntTextBox() { Margin = new Thickness(0), VerticalAlignment = VerticalAlignment.Center };
        APMIntTextBox txtId = new APMIntTextBox() { Visibility = Visibility.Collapsed };
        APMLabel lblTitle = new APMLabel() { VerticalAlignment = VerticalAlignment.Center, Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMInfoLabel lblName = new APMInfoLabel() { VerticalAlignment = VerticalAlignment.Center };
        APMTextBox txtName = new APMTextBox() { Visibility = Visibility.Collapsed };
        APMTextBox txtIds = new APMTextBox() { Visibility = Visibility.Collapsed };
        APMCheckBox chkShowCode = new APMCheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        APMCheckBox chkShowName = new APMCheckBox() { Visibility = Visibility.Collapsed, IsChecked = true };
        #endregion

        #region Constructor
        public APMBrowser()
        {
            this.Orientation = Orientation.Horizontal;
            txtCode.KeyDown += new KeyEventHandler(txtCode_KeyDown);
            txtCode.LostFocus += new RoutedEventHandler(txtCode_LostFocus);
            txtCode.TextChanged += new TextChangedEventHandler(txtCode_TextChanged);
            CreateChildren();
            BindingControls();
            Focusable = true;
        }
        #endregion

        #region Properties

        #region ShowCode
        public Boolean XShowCode
        {
            get
            {
                return (bool)chkShowCode.IsChecked;
            }
            set
            {
                chkShowCode.IsChecked = value;
            }
        }
        #endregion

        #region ShowName
        public Boolean XShowName
        {
            get
            {
                return (bool)chkShowName.IsChecked;
            }
            set
            {
                chkShowName.IsChecked = value;
            }
        }
        #endregion

        #region Title
        public string XTitle
        {
            get
            {
                return lblTitle.Content.ToString();
            }
            set
            {
                lblTitle.Content = (value.Trim().EndsWith(":")) ? value : value + " :";
                lblTitle.Visibility = (value.Trim() == "") ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

        #region TextBox
        public TextBox XTextBox
        {
            get
            {
                return txtCode;
            }
        }
        #endregion

        #region Label
        public APMInfoLabel XLabel
        {
            get
            {
                return lblName;
            }
        }
        #endregion

        #region MultiSelect
        private Boolean _multiSelect = false;
        public Boolean XMultiSelect
        {
            get
            {
                return _multiSelect;
            }
            set
            {
                _multiSelect = value;
                XShowCode = !value;
                XShowName = !value;
            }
        }
        #endregion

        #region SetBinding
        public void XSetBinding<T>(BindingList<T> bindingList, string fieldName)
        {
            if (fieldName == null || fieldName.Length < 2)
                return;
            this.Name = "brw_" + fieldName;
            fieldName = fieldName.Substring(0, fieldName.Length - 2);
            txtCode.DataContext = bindingList;
            txtCode.SetBinding(TextBox.TextProperty, fieldName + "code");
            lblName.DataContext = bindingList;
            lblName.SetBinding(Label.ContentProperty, fieldName + "name");
            txtName.DataContext = bindingList;
            txtName.SetBinding(TextBox.TextProperty, fieldName + "name");
            txtId.DataContext = bindingList;
            txtId.SetBinding(TextBox.TextProperty, fieldName + "id");
            txtIds.DataContext = bindingList;
            txtIds.SetBinding(TextBox.TextProperty, fieldName + "ids");
        }
        #endregion

        #region CanMagnify
        public bool XCanMagnify
        {
            get
            {
                return (btnBrowse.XCanMagnify == true);
            }
            set
            {
                btnBrowse.XCanMagnify = value;
            }
        }
        #endregion

        #endregion

        #region Tools

        #region CreateChildren
        private void CreateChildren()
        {
            this.Children.Clear();
            this.Children.Add(lblTitle);
            this.Children.Add(btnBrowse);
            this.Children.Add(txtId);
            this.Children.Add(txtName);
            this.Children.Add(txtCode);
            this.Children.Add(lblName);
        }
        #endregion

        #region Clear
        public void XClear()
        {
            txtName.Text = "";
            var BindingName = txtName.GetBindingExpression(TextBox.TextProperty);
            if (BindingName != null)
                BindingName.UpdateSource();
            lblName.Visibility = Visibility.Collapsed;

            txtCode.Text = "";
            var BindingCode = txtCode.GetBindingExpression(TextBox.TextProperty);
            if (BindingCode != null)
                BindingCode.UpdateSource();
            txtId.Text = "0";
            var BindingId = txtId.GetBindingExpression(TextBox.TextProperty);
            if (BindingId != null)
                BindingId.UpdateSource();
            txtIds.Text = "";
            var BindingIds = txtIds.GetBindingExpression(TextBox.TextProperty);
            if (BindingIds != null)
                BindingIds.UpdateSource();
        }
        #endregion

        #region BindControlsToEachOther
        private void BindingControls()
        {
            GlobalFunctions.BindVisibilityToIsChecked(txtCode, chkShowCode);
            GlobalFunctions.BindVisibilityToIsChecked(lblName, chkShowName);
        }

        #endregion

        #endregion

        #region Events

        #region ExternalEvents
        public event RoutedEventHandler XBrowseClick
        {
            add
            {
                btnBrowse.Click += value;
            }
            remove
            {
                btnBrowse.Click -= value;
            }
        }
        public event KeyEventHandler XTextBoxKeyDown
        {
            add
            {
                txtCode.KeyDown += value;
            }
            remove
            {
                txtCode.KeyDown -= value;
            }
        }
        #endregion

        #region InternalEvents
        void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtCode.Text.Trim() == "")
            {
                btnBrowse.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                e.Handled = true;
                return;
            }
        }
        void txtCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCode.Text != null && txtCode.Text.Trim() != "")
                lblName.Visibility = Visibility.Visible;
        }
        void txtCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCode.Text == null || txtCode.Text.Trim() == "")
                XClear();
        }
        #endregion

        #region Overrided Events
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            btnBrowse.Focus();
        }
        #endregion

        #endregion
    }
    #endregion

    #region APM (DataGrid)
    public class APMDataGrid : Microsoft.Windows.Controls.DataGrid
    {
        #region Variables
        APMCheckBox HeaderCheckBox = new APMCheckBox();
        #endregion

        #region Constructor
        public APMDataGrid()
        {
            FlowDirection = FlowDirection.RightToLeft;
            this.IsReadOnly = false;
            AutoGenerateColumns = false;
            SelectionMode = Microsoft.Windows.Controls.DataGridSelectionMode.Single;
            CanUserResizeRows = false;
            CanUserAddRows = false;
            CanUserDeleteRows = false;
            CanUserSortColumns = true;
            BorderThickness = new Thickness(0);
            Margin = new Thickness(0);
            _SubSystem = GlobalVariables.currentSubSystem;
            HorizontalGridLinesBrush = SubSystemColors.Items[(int)XSubSystem].HorizontalLineBrush;
            VerticalGridLinesBrush = SubSystemColors.Items[(int)XSubSystem].VerticalLineBrush;
            AlternatingRowBackground = SubSystemColors.Items[(int)XSubSystem].AlternateRowColor;
            HeaderCheckBox.Click += new RoutedEventHandler(HeaderCheckBox_Click);
            SetBackground();
            XWhiteSide = WhiteSideMode.Down;
            this.Sorting += new Microsoft.Windows.Controls.DataGridSortingEventHandler(datagrid_Sorting);


        }
        #endregion

        #region SetBackground
        private void SetBackground()
        {
            Tools.SetBackground(this, _transparent, (int)_whiteside, SubSystemColors.Items[(int)_SubSystem].BackgroundColor);
        }
        #endregion

        #region Remove Or Add Select Column
        private void RemoveORAddSelectColumn(Boolean Value)
        {
            if (Columns == null || Columns.Count == 0)
                return;
            if (Value == false)
            {
                if (this.Columns.Count > 0 && this.Columns[0].GetType() == typeof(Microsoft.Windows.Controls.DataGridCheckBoxColumn))
                    this.Columns[0].Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Columns[0].Visibility = Visibility.Visible;
                this.Columns[0].IsReadOnly = false;
                this.Columns[0].Header = HeaderCheckBox;
                HeaderCheckBox.Content = Columns[0].Header as string;

            }
        }

        #endregion

        #region SubSystem Property
        public SubSystems _SubSystem;
        public SubSystems XSubSystem
        {
            set
            {
                _SubSystem = value;
                SetBackground();
            }
            get
            {
                return _SubSystem;
            }
        }
        #endregion

        #region WhiteSide Property
        public WhiteSideMode _whiteside = WhiteSideMode.Up;
        public WhiteSideMode XWhiteSide
        {
            set
            {
                _whiteside = value;
                SetBackground();
            }
            get
            {
                return _whiteside;
            }
        }
        #endregion

        #region Transparent Property
        public Boolean _transparent = false;
        public Boolean XTransparent
        {
            set
            {
                _transparent = value;
                SetBackground();
            }
            get
            {
                return _transparent;
            }
        }
        #endregion

        #region Operation Propery
        private string _operation;
        public string XOperation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;
            }
        }
        #endregion

        #region MultiSelect
        private bool _multiSelect = false;
        public bool XMultiSelect
        {
            get
            {
                return _multiSelect;
            }
            set
            {
                _multiSelect = value;
                RemoveORAddSelectColumn(value);
            }
        }
        #endregion

        #region Adjust Method
        public void Adjust(object table)
        {
            if (XOperation == null || XOperation == "")
            {
                MessageBox.Show("Please Fill Operation property of the APMDataGrid.", "Message for Programmer");
                return;
            }
            XmlDocument xmlFile = new XmlDocument();
            Stream stream = this.GetType().Assembly.GetManifestResourceStream("APMComponents.GridColumns.GridColumns.xml");
            if (stream == null)
            {
                Messages.ErrorMessage(" نمی باشد " + " APMComponents\\GidColumns\\GridColumns.xml " + " برنامه قادر به خواندن فایل ");
                return;
            }
            StreamReader sr = new StreamReader(stream);
            xmlFile.LoadXml(sr.ReadToEnd());
            XmlElement root = xmlFile.DocumentElement;
            XmlNodeList Reports = root.ChildNodes;
            {
                Boolean ReportFinded = false;
                foreach (XmlNode Report in Reports)
                {
                    if (XOperation.ToLower() == Report.Name.ToLower())
                    {
                        ReportFinded = true;
                        XmlNodeList columns = Report.ChildNodes;
                        this.Columns.Clear();
                        this.SetBinding(ItemsSourceProperty, new Binding() { Source = table, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });


                        foreach (XmlNode column in columns)
                        {

                            Binding binding = new Binding(column.Name);
                            Microsoft.Windows.Controls.DataGridBoundColumn newColumn;
                            string ColumnType = column.ChildNodes[1].InnerText;
                            ColumnType = ColumnType.ToLower();
                            ColumnType = ColumnType.Trim();
                            if (ColumnType == "checkboxcolumn")
                                newColumn = new Microsoft.Windows.Controls.DataGridCheckBoxColumn();
                            else
                                newColumn = new Microsoft.Windows.Controls.DataGridTextColumn();
                            newColumn.Header = column.ChildNodes[0].InnerText;
                            newColumn.Width = Microsoft.Windows.Controls.DataGridLength.Auto;
                            newColumn.Binding = binding;

                            if (column.ChildNodes.Count == 4)
                                newColumn.IsReadOnly = false;
                            else
                                newColumn.IsReadOnly = true;

                            this.Columns.Add(newColumn);
                        }
                        RemoveORAddSelectColumn(XMultiSelect);
                        return;
                    }
                }
                if (!ReportFinded)
                    MessageBox.Show("Your Operation '" + XOperation + "' does not exist in XML file.", "Message To Programmer");
            }

        }
        #endregion

        #region Events
        void HeaderCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (XMultiSelect == false)
                return;
            if (Columns.Count == 0)
                return;
            var chkColumn = Columns[0] as Microsoft.Windows.Controls.DataGridColumn;
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (chkColumn.GetCellContent(this.Items[i]) is CheckBox)
                {
                    (chkColumn.GetCellContent(this.Items[i]) as CheckBox).IsChecked = (sender as CheckBox).IsChecked;
                    BindingExpression be = (chkColumn.GetCellContent(this.Items[i]) as CheckBox).GetBindingExpression(CheckBox.IsCheckedProperty);
                    be.UpdateSource();
                }
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RoutedEvent rout = e.RoutedEvent;
                e = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, Key.Tab);
                e.RoutedEvent = rout;
            }
            base.OnKeyDown(e);
        }
        private void datagrid_Sorting(object sender, Microsoft.Windows.Controls.DataGridSortingEventArgs e)
        {
            //string name = this.ItemsSource.GetType().Name;
            //if (name.StartsWith("APMBindingList"))
            //{
            //    Type[] typeForGetMethod = { typeof(APMBindingList<>) };
            //    MethodInfo mInfo = this.ItemsSource.GetType().GetMethod("ApplySort");
            //    object[] parameters = { e };
            //    object b = this.ItemsSource;
            //    mInfo.Invoke(b, parameters);
            //}
        }
        #endregion
    }
    #endregion

    #region APM (MenuItem)
    public class APMMenuItem : MenuItem
    {
        #region Constructor
        public APMMenuItem(string caption) : this(ButtonImageType.Search, caption) { }
        public APMMenuItem() : this(ButtonImageType.Search, "") { }
        public APMMenuItem(ButtonImageType imageType, string caption)
        {
            Height = 27;
            HorizontalAlignment = HorizontalAlignment.Left;
            FontFamily = new FontFamily("tahoma");
            Margin = new Thickness(1);
            Cursor = Cursors.Hand;
            this.Header = caption;
            SetImage(imageType);
        }
        #endregion

        #region Tools
        private void SetImage(ButtonImageType imageType)
        {
            APMToolbarButton b = new APMToolbarButton() { XImage = imageType };
            this.Icon = b.Content;
        }
        #endregion

        #region ImageType
        private ButtonImageType _xImageType = ButtonImageType.Search;
        public ButtonImageType XImageType
        {
            set
            {
                _xImageType = value;
                SetImage(value);
            }
            get
            {
                return _xImageType;
            }
        }
        #endregion

        #region Caption
        public string XCaption
        {
            set
            {
                Header = value;
            }
            get
            {
                return Header.ToString();
            }
        }
        #endregion
    }
    #endregion

    #region APM (ToolBar)
    public class APMToolBar : ToolBar
    {
        #region Controls
        APMToolbarButton btnRefresh = new APMToolbarButton() { XImage = ButtonImageType.Refresh };
        APMToolbarButton btnInsert = new APMToolbarButton() { XImage = ButtonImageType.Insert };
        APMToolbarButton btnEdit = new APMToolbarButton() { XImage = ButtonImageType.Edit };
        APMToolbarButton btnDelete = new APMToolbarButton() { XImage = ButtonImageType.Delete };
        APMToolbarButton btnSave = new APMToolbarButton() { XImage = ButtonImageType.Save };
        APMToolbarButton btnCancel = new APMToolbarButton() { XImage = ButtonImageType.Cancel };
        APMToolbarButton btnFirst = new APMToolbarButton() { XImage = ButtonImageType.First };
        APMToolbarButton btnPrevious = new APMToolbarButton() { XImage = ButtonImageType.Previous };
        APMToolbarButton btnNext = new APMToolbarButton() { XImage = ButtonImageType.Next };
        APMToolbarButton btnLast = new APMToolbarButton() { XImage = ButtonImageType.Last };
        APMToolbarButton btnPrint = new APMToolbarButton() { XImage = ButtonImageType.Print };
        APMToolbarButton btnHelp = new APMToolbarButton() { XImage = ButtonImageType.Help };
        APMToolbarButton btnSelect = new APMToolbarButton() { XImage = ButtonImageType.Select };
        APMToolbarButton btnSearch = new APMToolbarButton() { XImage = ButtonImageType.Search };

        APMToolbarButton btnConfirm = new APMToolbarButton() { XImage = ButtonImageType.Confirm };
        APMToolbarButton btnUndoConfirm = new APMToolbarButton() { XImage = ButtonImageType.UnConfirm };
        APMToolbarButton btnUseLess = new APMToolbarButton() { XImage = ButtonImageType.UseLess };
        APMToolbarButton btnUndoUseLess = new APMToolbarButton() { XImage = ButtonImageType.UndoUseLess };
        APMToolbarButton btnSaveTemp = new APMToolbarButton() { XImage = ButtonImageType.SaveTemp };
        APMToolbarButton btnSaveNote = new APMToolbarButton() { XImage = ButtonImageType.SaveNote };

        APMLabel label = new APMLabel();
        APMLabel seperator1 = new APMLabel();
        APMLabel seperator2 = new APMLabel();
        APMLabel seperator3 = new APMLabel();
        APMLabel seperator4 = new APMLabel();
        APMLabel seperator5 = new APMLabel();
        APMLabel seperator6 = new APMLabel();
        APMLabel seperator7 = new APMLabel();
        #endregion

        #region Constructor
        public APMToolBar()
        {
            this.AddChild(btnRefresh);

            this.AddChild(seperator1);
            this.AddChild(btnInsert);
            this.AddChild(btnEdit);
            this.AddChild(btnDelete);
            this.AddChild(seperator2);
            this.AddChild(btnSave);
            this.AddChild(btnConfirm);
            this.AddChild(btnUndoConfirm);
            this.AddChild(btnUseLess);
            this.AddChild(btnUndoUseLess);
            this.AddChild(seperator6);
            this.AddChild(btnSaveTemp);
            this.AddChild(btnSaveNote);
            this.AddChild(btnCancel);
            this.AddChild(seperator3);
            this.AddChild(btnFirst);
            this.AddChild(btnPrevious);
            this.AddChild(btnNext);
            this.AddChild(btnLast);
            this.AddChild(seperator4);
            this.AddChild(btnSearch);
            this.AddChild(seperator7);
            this.AddChild(btnPrint);
            this.AddChild(btnHelp);
            this.AddChild(seperator5);
            this.AddChild(btnSelect);
            DockPanel.SetDock(this, Dock.Top);
            Focusable = false;
            MinHeight = 52;
            xSubSystem = GlobalVariables.currentSubSystem;
            SetToolbarBackground();
            XType = XWindowType.NormalWindow;
        }
        #endregion

        #region SetBackground
        private void SetToolbarBackground()
        {
            Tools.SetBackground(this, transparent, (int)whiteside, SubSystemColors.Items[(int)xSubSystem].BackgroundColor);
        }
        #endregion

        #region SetButtonsVisibility
        private void SetButtonsVisibility()
        {
            btnRefresh.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.ReportWindow);
            btnInsert.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.OptionWindow && XType != XWindowType.ReportWindow);
            btnEdit.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.ReportWindow);
            btnDelete.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.OptionWindow && XType != XWindowType.ReportWindow);
            btnSave.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.AccDocumentWindow && XType != XWindowType.ReportWindow);
            btnCancel.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.ReportWindow);
            btnFirst.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.OptionWindow && XType != XWindowType.ReportWindow);
            btnPrevious.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.OptionWindow && XType != XWindowType.ReportWindow);
            btnNext.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.OptionWindow && XType != XWindowType.ReportWindow);
            btnLast.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.OptionWindow && XType != XWindowType.ReportWindow);
            btnSearch.Visibility = GlobalFunctions.BooleanToVisibility(true);
            btnPrint.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow);
            btnHelp.Visibility = GlobalFunctions.BooleanToVisibility(true);
            btnSelect.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.SelectWindow);
            btnConfirm.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.AccDocumentWindow);
            btnUndoConfirm.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.AccDocumentWindow);
            btnUseLess.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.AccDocumentWindow);
            btnUndoUseLess.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.AccDocumentWindow);
            btnSaveTemp.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.AccDocumentWindow);
            btnSaveNote.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.AccDocumentWindow);
            seperator1.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.ReportWindow);
            seperator2.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.ReportWindow);
            seperator3.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.ReportWindow);
            seperator4.Visibility = GlobalFunctions.BooleanToVisibility(XType != XWindowType.SelectWindow && XType != XWindowType.ReportWindow);
            seperator5.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.SelectWindow);
            seperator6.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.AccDocumentWindow);
            seperator7.Visibility = GlobalFunctions.BooleanToVisibility(XType == XWindowType.ReportWindow);

        }
        #endregion

        #region Property

        #region SubSystem Property
        public SubSystems xSubSystem;
        public SubSystems XSubSystem
        {
            set
            {
                xSubSystem = value;
                SetToolbarBackground();
            }
            get
            {
                return xSubSystem;
            }

        }
        #endregion

        #region WhiteSide Property
        public WhiteSideMode whiteside = WhiteSideMode.Up;
        public WhiteSideMode XWhiteSide
        {
            set
            {
                whiteside = value;
                SetToolbarBackground();
            }
            get
            {
                return whiteside;
            }
        }
        #endregion

        #region Transparent Property
        public Boolean transparent = false;
        public Boolean XTransparent
        {
            set
            {
                transparent = value;
                SetToolbarBackground();
            }
            get
            {
                return transparent;
            }
        }
        #endregion

        #region Type Property
        private XWindowType _type;
        public XWindowType XType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                SetButtonsVisibility();
            }

        }
        #endregion

        #region Buttons

        #region XInsertButton
        public APMToolbarButton XInsertButton
        {
            get { return btnInsert; }
        }
        #endregion

        #region XDeleteButton
        public APMToolbarButton XDeleteButton
        {
            get { return btnDelete; }
        }
        #endregion

        #region XEditButton
        public APMToolbarButton XEditButton
        {
            get { return btnEdit; }
        }
        #endregion

        #region XSaveButton
        public APMToolbarButton XSaveButton
        {
            get { return btnSave; }
        }
        #endregion

        #region XCancelButton
        public APMToolbarButton XCancelButton
        {
            get { return btnCancel; }
        }
        #endregion

        #region XNextButton
        public APMToolbarButton XNextButton
        {
            get { return btnNext; }
        }
        #endregion

        #region XPreviousButton
        public APMToolbarButton XPreviousButton
        {
            get { return btnPrevious; }
        }
        #endregion

        #region XFirstButton
        public APMToolbarButton XFirstButton
        {
            get { return btnFirst; }
        }
        #endregion

        #region XLastButton
        public APMToolbarButton XLastButton
        {
            get { return btnLast; }
        }
        #endregion

        #region XHelpButton
        public APMToolbarButton XHelpButton
        {
            get { return btnHelp; }
        }
        #endregion

        #region XRefreshButton
        public APMToolbarButton XRefreshButton
        {
            get { return btnRefresh; }
        }
        #endregion

        #region XSelectButton
        public APMToolbarButton XSelectButton
        {
            get { return btnSelect; }
        }
        #endregion

        #region XSearchButton
        public APMToolbarButton XSearchButton
        {
            get { return btnSearch; }
        }
        #endregion

        #region XPrintButton
        public APMToolbarButton XPrintButton
        {
            get { return btnPrint; }
        }
        #endregion

        #region XConfirmButton
        public APMToolbarButton XConfirmButton
        {
            get { return btnConfirm; }
        }
        #endregion

        #region XUndoConfirmButton
        public APMToolbarButton XUndoConfirmButton
        {
            get { return btnUndoConfirm; }
        }
        #endregion

        #region XUseLessButton
        public APMToolbarButton XUseLessButton
        {
            get { return btnUseLess; }
        }
        #endregion

        #region XUndoUseLessButton
        public APMToolbarButton XUndoUseLessButton
        {
            get { return btnUndoUseLess; }
        }
        #endregion

        #region XSaveNoteButton
        public APMToolbarButton XSaveNoteButton
        {
            get { return btnSaveNote; }
        }
        #endregion

        #region XSaveTempButton
        public APMToolbarButton XSaveTempButton
        {
            get { return btnSaveTemp; }
        }
        #endregion

        #endregion

        #endregion

    }
    #endregion

    #region APM (ToolbarButton)
    public class APMToolbarButton : Button
    {
        #region Global Variables
        private double saveHeight, BigHeight = 40, SmallHeight = 32, ExtraBigHeight = 45;

        #endregion

        #region Image Property
        public ButtonImageType _XImage = ButtonImageType.Insert;
        public ButtonImageType XImage
        {
            set
            {
                _XImage = value;
                if (value == ButtonImageType.Browse)
                    XSize = Size.Small;
                else if (value == ButtonImageType.Select)
                    XSize = Size.ExtraBig;
                else
                    XSize = Size.Big;
                set_image();
            }
            get
            {
                return _XImage;
            }
        }
        #endregion

        #region XSize
        public enum Size { Small, Big, ExtraBig };
        Size _size;
        public Size XSize
        {
            set
            {
                _size = value;
                switch (value)
                {
                    case Size.Small:
                        Height = SmallHeight;
                        break;
                    case Size.Big:
                        Height = BigHeight;
                        break;
                    case Size.ExtraBig:
                        Height = ExtraBigHeight;
                        break;
                }
            }
            get
            {
                return _size;
            }
        }
        #endregion

        #region SubSystem Property
        public SubSystems _xSubSystem = SubSystems.Accounting;
        public SubSystems XSubSystem
        {
            set
            {
                _xSubSystem = value;
                set_image();
            }
            get
            {
                return _xSubSystem;
            }

        }
        #endregion

        #region Constructor
        public APMToolbarButton()
        {
            BorderBrush = Brushes.Transparent;
            Background = Brushes.Transparent;
            Cursor = Cursors.Hand;
            _xSubSystem = GlobalVariables.currentSubSystem;
            IsEnabled = true;
            MouseEnter += MyMouseEnterEvent;
            MouseLeave += MyMouseLeaveEvent;
            IsTabStop = false;
            this.IsEnabledChanged += (APMToolbarButton_IsEnabledChanged);
            XSize = Size.Big;
        }

        #endregion

        #region Set Image
        public void set_image()
        {
            Image image = new Image();
            System.Drawing.Bitmap bitmap;
            switch (XSubSystem)
            {
                case SubSystems.Accounting:
                    switch (XImage)
                    {
                        case ButtonImageType.Search:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Search;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Search_d;
                            ToolTip = "جستجو";
                            break;

                        case ButtonImageType.Refresh:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Refresh;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Refresh_d;
                            ToolTip = "بازیابی مجدد";
                            break;

                        case ButtonImageType.Insert:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Insert;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Insert_d;
                            ToolTip = "درج";
                            break;
                        case ButtonImageType.Delete:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Delete;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Delete_d;
                            ToolTip = "حذف";
                            break;

                        case ButtonImageType.Edit:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Edit;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Edit_d;
                            ToolTip = "ویرایش";
                            break;

                        case ButtonImageType.Save:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Save;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Save_d;
                            ToolTip = "ذخیره";
                            break;

                        case ButtonImageType.Cancel:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Cancel;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Cancel_d;
                            ToolTip = "انصراف";
                            break;

                        case ButtonImageType.First:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.acc_First;
                            else
                                bitmap = global::APMComponents.Properties.Resources.First_d;
                            ToolTip = "اولین";
                            break;

                        case ButtonImageType.Last:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Last;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Last_d;
                            ToolTip = "اخرین";
                            break;

                        case ButtonImageType.Next:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Next;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Next_d;
                            ToolTip = "بعدی";
                            break;

                        case ButtonImageType.Previous:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Previous;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Previous_d;
                            ToolTip = "قبلی";
                            break;

                        case ButtonImageType.Help:
                            bitmap = global::APMComponents.Properties.Resources.Help;
                            ToolTip = "راهنما";
                            break;

                        case ButtonImageType.UseLess:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.UseLess;
                            else
                                bitmap = global::APMComponents.Properties.Resources.UseLess_d;
                            ToolTip = "ابطال";
                            break;

                        case ButtonImageType.ReturnUseLess:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.UseLessReturn;
                            else
                                bitmap = global::APMComponents.Properties.Resources.UseLessReturn_d;
                            ToolTip = "برگشت از ابطال";
                            break;
                        case ButtonImageType.Confirm:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Confirm;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Confirm_d;
                            ToolTip = "قطعی کردن";
                            break;
                        case ButtonImageType.UnConfirm:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.UndoConfirm;
                            else
                                bitmap = global::APMComponents.Properties.Resources.UndoConfirm_d;
                            ToolTip = "برگشت از قطعی کردن";
                            break;
                        case ButtonImageType.SaveTemp:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.SaveTemp;
                            else
                                bitmap = global::APMComponents.Properties.Resources.SaveTemp_d;
                            ToolTip = "ثبت موقت";
                            break;

                        case ButtonImageType.SaveNote:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.SaveNote;
                            else
                                bitmap = global::APMComponents.Properties.Resources.SaveNote_d;
                            ToolTip = "ثبت یادداشت";
                            break;

                        case ButtonImageType.Browse:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Browse;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Browse_d;
                            ToolTip = "انتخاب";
                            break;

                        case ButtonImageType.Print:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Print;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Print_d;
                            ToolTip = "چاپ";
                            break;

                        case ButtonImageType.Select:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Select;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Select_d;
                            ToolTip = "انتخاب";
                            break;

                        case ButtonImageType.Copy:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Copy;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Copy_d;
                            ToolTip = "کپی";
                            break;
                        case ButtonImageType.UndoUseLess:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.UndoUseLess;
                            else
                                bitmap = global::APMComponents.Properties.Resources.UndoUseLess_d;
                            ToolTip = "بازگشت";
                            break;
                        case ButtonImageType.Backup:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.backup;
                            else
                                bitmap = global::APMComponents.Properties.Resources.backup;
                            ToolTip = "گرفتن فایل پشتیبان";
                            break;
                        case ButtonImageType.Restore:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.restore;
                            else
                                bitmap = global::APMComponents.Properties.Resources.restore;
                            ToolTip = "بازیابی فایل پشتیبان";
                            break;
                        case ButtonImageType.Action:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Action;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Action_d;
                            break;

                        default: bitmap = null;
                            break;

                    }

                    break;
                case SubSystems.Inventory:
                    switch (XImage)
                    {
                        case ButtonImageType.Search:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Search;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Search_d;
                            ToolTip = "جستجو";
                            break;

                        case ButtonImageType.Refresh:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Refresh;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Refresh_d;
                            ToolTip = "بازیابی مجدد";
                            break;

                        case ButtonImageType.Insert:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Insert;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Insert_d;
                            ToolTip = "درج";
                            break;
                        case ButtonImageType.Delete:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Delete;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Delete_d;
                            ToolTip = "حذف";
                            break;

                        case ButtonImageType.Edit:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Edit;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Edit_d;
                            ToolTip = "ویرایش";
                            break;

                        case ButtonImageType.Save:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Save;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Save_d;
                            ToolTip = "ذخیره";
                            break;

                        case ButtonImageType.Cancel:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Cancel;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Cancel_d;
                            ToolTip = "انصراف";
                            break;

                        case ButtonImageType.First:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_First;
                            else
                                bitmap = global::APMComponents.Properties.Resources.First_d;
                            ToolTip = "اولین";
                            break;

                        case ButtonImageType.Last:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Last;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Last_d;
                            ToolTip = "اخرین";
                            break;

                        case ButtonImageType.Next:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Next;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Next_d;
                            ToolTip = "بعدی";
                            break;

                        case ButtonImageType.Previous:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Previous;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Previous_d;
                            ToolTip = "قبلی";
                            break;

                        case ButtonImageType.Help:
                            bitmap = global::APMComponents.Properties.Resources.Inv_Help;
                            ToolTip = "راهنما";
                            break;

                        case ButtonImageType.UseLess:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_UseLess;
                            else
                                bitmap = global::APMComponents.Properties.Resources.UseLess_d;
                            ToolTip = "ابطال";
                            break;

                        case ButtonImageType.ReturnUseLess:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_UseLessReturn;
                            else
                                bitmap = global::APMComponents.Properties.Resources.UseLessReturn_d;
                            ToolTip = "برگشت از ابطال";
                            break;
                        case ButtonImageType.Confirm:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Confirm;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Confirm_d;
                            ToolTip = "قطعی کردن";
                            break;
                        case ButtonImageType.UnConfirm:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_UndoConfirm;
                            else
                                bitmap = global::APMComponents.Properties.Resources.UndoConfirm_d;
                            ToolTip = "برگشت از قطعی کردن";
                            break;
                        case ButtonImageType.SaveTemp:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_SaveTemp;
                            else
                                bitmap = global::APMComponents.Properties.Resources.SaveTemp_d;
                            ToolTip = "ثبت موقت";
                            break;

                        case ButtonImageType.SaveNote:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_SaveNote;
                            else
                                bitmap = global::APMComponents.Properties.Resources.SaveNote_d;
                            ToolTip = "ثبت یادداشت";
                            break;

                        case ButtonImageType.Browse:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Browse;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Browse_d;
                            ToolTip = "انتخاب";
                            break;

                        case ButtonImageType.Print:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Print;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Print_d;
                            ToolTip = "چاپ";
                            break;

                        case ButtonImageType.Select:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Select;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Select_d;
                            ToolTip = "انتخاب";
                            break;

                        case ButtonImageType.Copy:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Copy;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Copy_d;
                            ToolTip = "کپی";
                            break;
                        case ButtonImageType.Action:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Inv_Action;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Inv_Action_d;
                            ToolTip = "اجرا";
                            break;
                        case ButtonImageType.Hesabdari:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Hesabdari;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Hesabdari_d;
                            ToolTip = "ریالی کردن سند";
                            break;
                        case ButtonImageType.Adjustment:
                            if (IsEnabled)
                                bitmap = global::APMComponents.Properties.Resources.Adjustment;
                            else
                                bitmap = global::APMComponents.Properties.Resources.Adjustment;
                            ToolTip = "سند تعدیل";
                            break;
                        default: bitmap = null;
                            break;
                    }
                    break;
                default: bitmap = global::APMComponents.Properties.Resources.Insert;
                    break;
            }
            image.Source = Functions.Convert_Bitmap_To_Source(bitmap);
            Content = image;
        }
        #endregion

        #region XCanMagnify
        public bool? _canMagnify = true;
        public bool? XCanMagnify
        {
            set
            {
                _canMagnify = value;

            }
            get
            {
                return _canMagnify;
            }
        }
        #endregion

        #region MyEvents
        private void MyMouseEnterEvent(object source, EventArgs e)
        {
            saveHeight = Height;
            if ((this.XImage == ButtonImageType.Browse && this.XCanMagnify != true) || this.XCanMagnify == false)
                return;
            Height = Height * 1.15;
        }
        private void MyMouseLeaveEvent(object source, EventArgs e)
        {
            Height = saveHeight;
        }
        private void APMToolbarButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            set_image();
        }
        #endregion
    }
    #endregion

    #region APM (TreeViewItem)
    public class APMTreeViewItem : TreeViewItem
    {
        #region Variables
        private Brush saveFontColor;
        #endregion

        #region Controls
        private CheckBox checkBox = new CheckBox();
        #endregion

        #region Constructor
        public APMTreeViewItem()
        {
            checkBox.VerticalAlignment = VerticalAlignment.Center;
            checkBox.Checked += new RoutedEventHandler(checkBox_Checked);
            checkBox.Unchecked += new RoutedEventHandler(checkBox_Checked);
            checkBox.Focusable = false;
            Background = Brushes.Transparent;
            Cursor = Cursors.Hand;
            Margin = new Thickness(2);
            this.Selected += APMTreeViewItem_Selected;
            this.Unselected += APMTreeViewItem_UnSelected;
            this.LostFocus += APMTreeViewItem_UnSelected;
            this.GotFocus += APMTreeViewItem_Selected;
            XHaveCheckBox = false;
            XCheckParentWithChildren = false;
        }

        #endregion

        #region Internal Events
        void APMTreeViewItem_UnSelected(object sender, RoutedEventArgs e)
        {
            this.Foreground = saveFontColor;
        }
        void APMTreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (Foreground != Brushes.White)
                saveFontColor = this.Foreground;
            if ((this as TreeViewItem).IsSelected)
            {
                this.Foreground = Brushes.White;
            }
        }
        #endregion

        #region Properties

        #region IsChecked
        public Boolean XIsChecked
        {
            get
            {
                return (XHaveCheckBox && checkBox.IsChecked == true);
            }
            set
            {
                checkBox.IsChecked = value;
            }
        }
        #endregion

        #region Caption
        public string XCaption
        {
            get
            {
                return (checkBox.Content as string);
            }
            set
            {
                checkBox.Content = value;
                if (!XHaveCheckBox)
                    Header = value;
            }
        }
        #endregion

        #region CheckParentWithChildren
        private Boolean _checkParentWithChildren = false;
        public Boolean XCheckParentWithChildren
        {
            get
            {
                return _checkParentWithChildren;
            }
            set
            {
                _checkParentWithChildren = value;
            }
        }
        #endregion

        #region HaveCheckBox
        private Boolean _haveCheckBox = false;
        public Boolean XHaveCheckBox
        {
            get
            {
                return _haveCheckBox;
            }
            set
            {
                _haveCheckBox = value;
                if (_haveCheckBox)
                    Header = checkBox;
                else
                    Header = (checkBox.Content as string);
            }
        }
        #endregion

        #region LoadedFromDataBase
        private Boolean _loadedFromDataBase = false;
        public Boolean XLoadedFromDataBase
        {
            get
            {
                return _loadedFromDataBase;
            }
            set
            {
                _loadedFromDataBase = value;
            }
        }
        #endregion

        #endregion

        #region CheckBox Checked

        #region  checkBox_Checked
        void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (XIsChecked == checkBox.IsChecked)
                return;
            XIsChecked = (Boolean)checkBox.IsChecked;
            Check_Changed(null, null);
        }

        #endregion

        #region Check_Changed
        private void Check_Changed(object obj, RoutedPropertyChangedEventArgs<object> e)
        {
            if (XCheckParentWithChildren == false)
                return;
            if (XIsChecked == true)
                foreach (APMTreeViewItem child in this.Items)
                    child.XIsChecked = XIsChecked;
            else if (Parent != null && Parent is APMTreeViewItem)
                (Parent as APMTreeViewItem).XIsChecked = false;
        }
        #endregion

        #endregion
    }
    #endregion

    #region APM (StatusBar)
    public class APMStatusBar : System.Windows.Controls.Primitives.StatusBar
    {
        #region Initial

        enum StatusBarElementType { Insert, Delete, Edit, Save, Cancel, Refresh, Help, Search, InsertArticle, DeleteArticle }


        APMLabel lblInsert = new APMLabel() { Content = "درج:  +", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblDelete = new APMLabel() { Content = "حذف:  -", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblEdit = new APMLabel() { Content = "ویرایش: F4", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSave = new APMLabel() { Content = "ثبت: F11", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblCancel = new APMLabel() { Content = "انصراف: Escape", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblRefresh = new APMLabel() { Content = "بازیابی مجدد: F5", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblHelp = new APMLabel() { Content = "راهنمایی: F1", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSearch = new APMLabel() { Content = "جستجو: F7", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblInsertArticle = new APMLabel() { Content = "اضافه کردن آرتیکل: + Ctrl", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblDeleteArticle = new APMLabel() { Content = "حذف آرتیکل : - Ctrl ", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblArticleNext = new APMLabel() { Content = " آرتیکل بعدی: Ctrl N", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblArticlePrevious = new APMLabel() { Content = " آرتیکل قبلی : Ctrl P", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSelect = new APMLabel() { Content = "انتخاب : Enter", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSaveTemp = new APMLabel() { Content = " ثبت موقت: F11", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSaveNote = new APMLabel() { Content = " ثبت یادداشت: F12", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };

        APMLabel lblSeperator1 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator2 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator3 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator4 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator5 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator6 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator7 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator8 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator9 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator10 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator11 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator12 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator13 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };
        APMLabel lblSeperator14 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed, Margin = new Thickness(0) };

        #endregion

        #region Properties

        #region StatusBarType
        public enum XStatusBarType { NormalWindow, SelectWindow, InvDocumentWindow, AccDocumentWindow, OptionWindow };
        private XStatusBarType _type;
        public XStatusBarType XType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                SetStausBarItems();
            }
        }
        #endregion

        #region StatusBarIems
        private void SetStausBarItems()
        {
            this.lblInsert.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.OptionWindow);
            this.lblSeperator1.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.OptionWindow);

            this.lblDelete.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.SelectWindow && XType != XStatusBarType.OptionWindow);
            this.lblSeperator2.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.SelectWindow && XType != XStatusBarType.OptionWindow);

            this.lblEdit.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.SelectWindow);
            this.lblSeperator3.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.SelectWindow);

            this.lblSave.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.SelectWindow && XType != XStatusBarType.AccDocumentWindow);
            this.lblSeperator4.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.SelectWindow && XType != XStatusBarType.AccDocumentWindow);

            this.lblSelect.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.SelectWindow);
            this.lblSeperator13.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.SelectWindow);

            this.lblCancel.Visibility = GlobalFunctions.BooleanToVisibility(true);
            this.lblSeperator7.Visibility = GlobalFunctions.BooleanToVisibility(true);

            this.lblRefresh.Visibility = GlobalFunctions.BooleanToVisibility(true);
            this.lblSeperator5.Visibility = GlobalFunctions.BooleanToVisibility(true);

            this.lblHelp.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.InvDocumentWindow);
            this.lblSeperator8.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.InvDocumentWindow);

            this.lblSearch.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.SelectWindow && XType != XStatusBarType.InvDocumentWindow);
            this.lblSeperator6.Visibility = GlobalFunctions.BooleanToVisibility(XType != XStatusBarType.SelectWindow && XType != XStatusBarType.InvDocumentWindow);

            this.lblInsertArticle.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.InvDocumentWindow);
            this.lblSeperator9.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.InvDocumentWindow);

            this.lblArticleNext.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.InvDocumentWindow);
            this.lblSeperator10.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.InvDocumentWindow);

            this.lblArticlePrevious.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.InvDocumentWindow);
            this.lblSeperator11.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.InvDocumentWindow);

            this.lblDeleteArticle.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.InvDocumentWindow);

            this.lblSaveTemp.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.AccDocumentWindow);
            this.lblSeperator14.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.AccDocumentWindow);

            this.lblSaveNote.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.AccDocumentWindow);
            this.lblSeperator12.Visibility = GlobalFunctions.BooleanToVisibility(XType == XStatusBarType.AccDocumentWindow);

        }
        #endregion

        #endregion

        #region Constructor
        public APMStatusBar()
        {
            DockPanel.SetDock(this, Dock.Bottom);
            FontFamily = new FontFamily("Tahoma");
            FontSize = 12;

            this.Items.Clear();
            this.Items.Add(lblInsert);
            this.Items.Add(lblSeperator1);
            this.Items.Add(lblDelete);
            this.Items.Add(lblSeperator2);
            this.Items.Add(lblEdit);
            this.Items.Add(lblSeperator3);
            this.Items.Add(lblSave);
            this.Items.Add(lblSeperator4);
            this.Items.Add(lblSaveTemp);
            this.Items.Add(lblSeperator14);
            this.Items.Add(lblSaveNote);
            this.Items.Add(lblSeperator12);
            this.Items.Add(lblRefresh);
            this.Items.Add(lblSeperator5);
            this.Items.Add(lblSearch);
            this.Items.Add(lblSeperator6);
            this.Items.Add(lblCancel);
            this.Items.Add(lblSeperator7);
            this.Items.Add(lblHelp);
            this.Items.Add(lblSeperator8);
            this.Items.Add(lblInsertArticle);
            this.Items.Add(lblSeperator9);
            this.Items.Add(lblDeleteArticle);
            this.Items.Add(lblSeperator10);
            this.Items.Add(lblArticlePrevious);
            this.Items.Add(lblSeperator11);
            this.Items.Add(lblArticleNext);
            _whiteside = WhiteSideMode.Down;
            _xSubSystem = GlobalVariables.currentSubSystem;
            SetBackground();
            XType = XStatusBarType.NormalWindow;
        }
        #endregion

        #region SetBackground
        private void SetBackground()
        {
            Tools.SetBackground(this, _transparent, (int)_whiteside, SubSystemColors.Items[(int)_xSubSystem].BackgroundColor);
        }
        #endregion

        #region SubSystem Property
        public SubSystems _xSubSystem;
        public SubSystems XSubSystem
        {
            set
            {
                _xSubSystem = value;
                SetBackground();
            }
            get
            {
                return _xSubSystem;
            }

        }
        #endregion

        #region WhiteSide Property
        public WhiteSideMode _whiteside = WhiteSideMode.Up;
        public WhiteSideMode XWhiteSide
        {
            set
            {
                _whiteside = value;
                SetBackground();
            }
            get
            {
                return _whiteside;
            }
        }
        #endregion

        #region Transparent Property
        public Boolean _transparent = false;
        public Boolean XTransparent
        {
            set
            {
                _transparent = value;
                SetBackground();
            }
            get
            {
                return _transparent;
            }
        }
        #endregion

    }
    #endregion

    #region APM (TwoPartCode)
    public class APMTwoPartCode : StackPanel
    {
        #region Controls
        APMInfoLabel lblPreCode = new APMInfoLabel() { Margin = new Thickness(0) };
        APMIntTextBox txtChildCode = new APMIntTextBox() { Margin = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Right };
        APMInfoLabel lblChildCode = new APMInfoLabel() { Margin = new Thickness(0) };
        #endregion

        #region Constuctor
        public APMTwoPartCode()
        {
            this.Orientation = Orientation.Horizontal;
            this.Children.Add(lblChildCode);
            this.Children.Add(txtChildCode);
            this.Children.Add(lblPreCode);
            XEditable = true;
        }
        #endregion

        #region SetBinding
        public void XSetBinding<T>(BindingList<T> bindingList)
        {
            txtChildCode.DataContext = bindingList;
            txtChildCode.SetBinding(TextBox.TextProperty, new Binding() { Path = new PropertyPath(FieldNames<T>.ChildCode), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            lblPreCode.DataContext = bindingList;
            lblPreCode.SetBinding(Label.ContentProperty, FieldNames<T>.PreCode);
            lblChildCode.DataContext = bindingList;
            lblChildCode.SetBinding(Label.ContentProperty, FieldNames<T>.ChildCode);

        }
        #endregion

        #region Properties
        public string XChildCode
        {
            get
            {
                return txtChildCode.Text;
            }
        }
        public string XPreCode
        {
            get
            {
                return lblPreCode.Content as string;
            }
        }
        public Boolean XEditable
        {
            get
            {
                return (txtChildCode.Visibility == Visibility.Visible);
            }
            set
            {
                lblChildCode.Visibility = GlobalFunctions.BooleanToVisibility(!value);
                txtChildCode.Visibility = GlobalFunctions.BooleanToVisibility(value);
            }
        }
        public int XMaxLength
        {
            set
            {
                txtChildCode.MaxLength = value;
            }
        }

        #endregion

    }
    #endregion

    #region APM (SubSystemButton)
    public class APMSubSystemButton : StackPanel
    {
        #region Global Variables
        private double saveHeight;
        APMInfoLabel captionLabel = new APMInfoLabel() { Margin = new Thickness(0) };
        Button imageButton = new Button() { Margin = new Thickness(0) };
        #endregion

        #region Image Property
        private SubSystems _image;
        public SubSystems XImage
        {
            set
            {
                _image = value;
                set_image();
            }
            get
            {
                return _image;
            }
        }
        #endregion

        #region  Properties
        public SubSystems _xSubSystem = SubSystems.Accounting;
        public SubSystems XSubSystem
        {
            set
            {
                _xSubSystem = value;
                set_image();
            }
            get
            {
                return _xSubSystem;
            }
        }

        public string Xcaption
        {
            set
            {
                this.captionLabel.Content = value;
            }
            get
            {
                return this.captionLabel.Content as string;
            }
        }
        #endregion

        #region Constructor
        public APMSubSystemButton()
        {
            this.Orientation = Orientation.Vertical;
            this.Children.Add(imageButton);
            this.Children.Add(captionLabel);
            captionLabel.HorizontalAlignment = HorizontalAlignment.Center;
            imageButton.BorderBrush = Brushes.Transparent;
            imageButton.Background = Brushes.Transparent;
            Cursor = Cursors.Hand;
            imageButton.Height = 80;
            imageButton.MouseEnter += MyMouseEnterEvent;
            imageButton.MouseLeave += MyMouseLeaveEvent;
        }

        #endregion

        #region Set Image
        public void set_image()
        {
            Image image = new Image();
            System.Drawing.Bitmap bitmap;
            switch (XImage)
            {
                case SubSystems.Inventory:
                    bitmap = global::APMComponents.Properties.Resources.Inventory;
                    break;

                case SubSystems.Accounting:
                    bitmap = global::APMComponents.Properties.Resources.Accounting;
                    break;

                case SubSystems.Global:
                    bitmap = global::APMComponents.Properties.Resources.Global;
                    break;

                case SubSystems.AfterSaleServices:
                    bitmap = global::APMComponents.Properties.Resources.khadamatpassazforoush;
                    break;

                case SubSystems.Exchange:
                    bitmap = global::APMComponents.Properties.Resources.saham;
                    break;

                case SubSystems.IndustryEngineering:
                    bitmap = global::APMComponents.Properties.Resources.industry;
                    break;

                case SubSystems.PayRoll:
                    bitmap = global::APMComponents.Properties.Resources.hoghooghdastmozd;
                    break;


                case SubSystems.InnerAuditing:
                    bitmap = global::APMComponents.Properties.Resources.hesabresiDakheli;
                    break;

                case SubSystems.ProductionManagement:
                    bitmap = global::APMComponents.Properties.Resources.GenerateManagement;
                    break;

                case SubSystems.Sell:
                    bitmap = global::APMComponents.Properties.Resources.Forosh;
                    break;

                case SubSystems.Automation:
                    bitmap = global::APMComponents.Properties.Resources.Otomasion;
                    break;

                case SubSystems.Tools:
                    bitmap = global::APMComponents.Properties.Resources.Tools;
                    break;

                default: bitmap = null;
                    break;

            }
            image.Source = Functions.Convert_Bitmap_To_Source(bitmap);
            imageButton.Content = image;
        }
        #endregion

        #region MyEvents
        private void MyMouseEnterEvent(object source, EventArgs e)
        {
            saveHeight = Height;
            Height = Height * 1.15;
        }

        private void MyMouseLeaveEvent(object source, EventArgs e)
        {
            Height = saveHeight;
        }

        #endregion

        #region External Events
        public event RoutedEventHandler XClick
        {
            add
            {
                imageButton.Click += value;
            }
            remove
            {
                imageButton.Click -= value;
            }
        }
        #endregion

    }
    #endregion

    #region APM (DataGridExtended)
    public class APMDataGridExtended : DockPanel
    {
        #region Controls
        public APMDataGrid datagrid = new APMDataGrid();
        StackPanel dataGridStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        APMLabel lblGridItemsCount = new APMLabel();
        APMLabel lblCurrentItem = new APMLabel();
        APMLabel lblTotalAmount = new APMLabel() { Visibility = Visibility.Collapsed };
        APMLabel lblTotalRials = new APMLabel() { Visibility = Visibility.Collapsed };
        APMLabel lblGridItemsCountTitle = new APMLabel() { Content = "تعداد عناصر:" };
        APMLabel lblCurrentItemTitle = new APMLabel() { Content = "عنصر جاری:" };
        APMLabel lblTotalAmountTitle = new APMLabel() { Visibility = Visibility.Collapsed, Content = "جمع مقدار:" };
        APMLabel lblTotalRialsTitle = new APMLabel() { Visibility = Visibility.Collapsed, Content = "جمع مبلغ:" };
        APMLabel lblTotalDept = new APMLabel();
        APMLabel lblTotalDeptTitle = new APMLabel() { Visibility = Visibility.Collapsed, Content = "جمع بدهکار:" };
        APMLabel lblTotalCredit = new APMLabel();
        APMLabel lblTotalCreditTitle = new APMLabel() { Visibility = Visibility.Collapsed, Content = "جمع بستانکار:" };
        APMLabel lblRemaining = new APMLabel();
        APMLabel lblRemainingTitle = new APMLabel() { Visibility = Visibility.Collapsed, Content = "مانده:" };
        APMLabel lblSeperator1 = new APMLabel() { Content = "|" };
        APMLabel lblSeperator2 = new APMLabel() { Content = "|" };
        APMLabel lblSeperator3 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed };
        APMLabel lblSeperator4 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed };
        APMLabel lblSeperator5 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed };
        APMLabel lblSeperator6 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed };
        APMLabel lblSeperator7 = new APMLabel() { Content = "|", Visibility = Visibility.Collapsed };
        #endregion

        #region Constructor
        public APMDataGridExtended()
        {
            AddChildren();
            datagrid.SelectionChanged += SetCountAndCurrentَ;

        }

        #endregion

        #region Tools
        private void SetCountAndCurrentَ(object source, EventArgs e)
        {
            lblGridItemsCount.Content = this.datagrid.Items.Count;
            lblCurrentItem.Content = this.datagrid.SelectedIndex + 1;
        }
        private void AddChildren()
        {
            this.dataGridStackPanel.Background = SystemColors.ControlLightLightBrush;
            this.dataGridStackPanel.Children.Add(lblGridItemsCountTitle);
            this.dataGridStackPanel.Children.Add(lblGridItemsCount);
            this.dataGridStackPanel.Children.Add(lblSeperator1);
            this.dataGridStackPanel.Children.Add(lblCurrentItemTitle);
            this.dataGridStackPanel.Children.Add(lblCurrentItem);
            this.dataGridStackPanel.Children.Add(lblSeperator2);
            this.dataGridStackPanel.Children.Add(lblTotalAmountTitle);
            this.dataGridStackPanel.Children.Add(lblTotalAmount);
            this.dataGridStackPanel.Children.Add(lblSeperator3);
            this.dataGridStackPanel.Children.Add(lblTotalRialsTitle);
            this.dataGridStackPanel.Children.Add(lblTotalRials);
            this.dataGridStackPanel.Children.Add(lblSeperator4);
            this.dataGridStackPanel.Children.Add(lblTotalDeptTitle);
            this.dataGridStackPanel.Children.Add(lblTotalDept);
            this.dataGridStackPanel.Children.Add(lblSeperator6);
            this.dataGridStackPanel.Children.Add(lblTotalCreditTitle);
            this.dataGridStackPanel.Children.Add(lblTotalCredit);
            this.dataGridStackPanel.Children.Add(lblSeperator5);
            this.dataGridStackPanel.Children.Add(lblRemainingTitle);
            this.dataGridStackPanel.Children.Add(lblRemaining);
            this.dataGridStackPanel.Children.Add(lblSeperator7);
            this.Children.Add(dataGridStackPanel);
            DockPanel.SetDock(dataGridStackPanel, Dock.Bottom);
            this.Children.Add(datagrid);

        }
        #endregion

        #region Properties
        public string XTotalAmountContent
        {
            set
            {
                this.lblTotalAmount.Content = value;
            }
            get
            {
                return this.lblTotalAmount.Content.ToString();
            }
        }
        public string XTotalRialsContent
        {
            set
            {
                this.lblTotalRials.Content = value;
            }
            get
            {
                return this.lblTotalRials.Content.ToString();
            }
        }
        public string XTotalCreditContent
        {
            set
            {
                this.lblTotalCredit.Content = value;
            }
            get
            {
                return this.lblTotalCredit.Content.ToString();
            }
        }
        public string XRemainingContent
        {
            set
            {
                this.lblRemaining.Content = value;
            }
            get
            {
                return this.lblRemaining.Content.ToString();
            }
        }
        public string XTotalDebtContent
        {
            set
            {
                this.lblTotalDept.Content = value;
            }
            get
            {
                return this.lblTotalDept.Content.ToString();
            }
        }
        public bool XShowTotalAmount
        {
            set
            {
                this.lblTotalAmountTitle.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblTotalAmount.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblSeperator3.Visibility = GlobalFunctions.BooleanToVisibility(value);
            }
            get
            {
                return System.Convert.ToBoolean(lblTotalAmountTitle.Visibility);
            }
        }
        public bool XShowTotalRials
        {
            set
            {
                this.lblTotalRialsTitle.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblTotalRials.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblSeperator4.Visibility = GlobalFunctions.BooleanToVisibility(value);
            }
            get
            {
                return
                System.Convert.ToBoolean(lblTotalRialsTitle.Visibility);
            }
        }
        public bool XShowTotalCredit
        {
            set
            {
                this.lblTotalCreditTitle.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblTotalCredit.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblSeperator5.Visibility = GlobalFunctions.BooleanToVisibility(value);
            }
            get
            {
                return System.Convert.ToBoolean(lblTotalCreditTitle.Visibility);
            }
        }
        public bool XShowTotalDept
        {
            set
            {
                this.lblTotalDeptTitle.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblTotalDept.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblSeperator6.Visibility = GlobalFunctions.BooleanToVisibility(value);
            }
            get
            {
                return System.Convert.ToBoolean(lblTotalDeptTitle.Visibility);
            }
        }
        public bool XShowRemaining
        {
            set
            {
                this.lblRemainingTitle.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblRemaining.Visibility = GlobalFunctions.BooleanToVisibility(value);
                this.lblSeperator7.Visibility = GlobalFunctions.BooleanToVisibility(value);
            }
            get
            {
                return System.Convert.ToBoolean(lblRemainingTitle.Visibility);
            }
        }
        #endregion

        #region SetBinding
        public void XSetBinding<T>(BindingList<T> bindingList)
        {
            lblTotalAmount.DataContext = bindingList;
            lblTotalAmount.SetBinding(Label.ContentProperty, FieldNames<T>.SumCount);

            lblTotalRials.DataContext = bindingList;
            lblTotalRials.SetBinding(Label.ContentProperty, FieldNames<T>.SumPrice);

            lblTotalDept.DataContext = bindingList;
            lblTotalDept.SetBinding(Label.ContentProperty, FieldNames<T>.SumDebt);

            lblTotalCredit.DataContext = bindingList;
            lblTotalCredit.SetBinding(Label.ContentProperty, FieldNames<T>.SumCredit);

            lblRemaining.DataContext = bindingList;
            lblRemaining.SetBinding(Label.ContentProperty, FieldNames<T>.AccRemaining);

        }
        #endregion
    }
    #endregion

    #region APM (Seprator)
    public class APMSeprator : APMGroupBox
    {
        public APMSeprator(int margin)
        {
            Margin = new Thickness(margin);
            Width = 3;
        }
        public APMSeprator()
        {
            Width = 3;
        }
    }
    #endregion

    #region APM (Logo)
    public class APMLogo : Image
    {
        #region Constructor
        public APMLogo()
        {
        }
        #endregion
    }
    #endregion

    #region APM (Image)
    public class APMCorrectImage : Image
    {
        #region Constructor
        public APMCorrectImage()
        {
            Stretch = System.Windows.Media.Stretch.None;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Margin = new Thickness(5, 1, 1, 1);
            XImage = ImageType.Correct;
            XImage = ImageType.Wrong;
        }
        #endregion
        private ImageType _XImage;
        public ImageType XImage
        {
            set
            {
                if (_XImage == value)
                    return;
                System.Drawing.Bitmap bitmap;
                _XImage = value;
                switch (value)
                {
                    case ImageType.Correct:
                        bitmap = global::APMComponents.Properties.Resources.Correct;
                        break;

                    case ImageType.Wrong:
                        bitmap = global::APMComponents.Properties.Resources.Wrong;
                        break;

                    default: bitmap = global::APMComponents.Properties.Resources.Correct;
                        break;
                }
                Source = Functions.Convert_Bitmap_To_Source(bitmap);
            }
            get
            {
                return _XImage;
            }
        }
    }
    #endregion

    #region APM (GroupBox)
    public class APMGroupBox : GroupBox
    {
        #region Constructor
        public APMGroupBox()
        {
            FlowDirection = FlowDirection.RightToLeft;
            BorderThickness = new Thickness(2);
            Margin = new Thickness(3, 2, 3, 5);
            _xSubSystem = GlobalVariables.currentSubSystem;
            SetBackground();
        }
        #endregion

        #region SetBackground
        private void SetBackground()
        {
            BorderBrush = SubSystemColors.Items[(int)XSubSystem].BordersBrush;
            Tools.SetBackground(this, _transparent, (int)_whiteside, SubSystemColors.Items[(int)_xSubSystem].BackgroundColor);
        }
        #endregion

        #region SubSystem Property
        public SubSystems _xSubSystem;
        public SubSystems XSubSystem
        {
            set
            {
                _xSubSystem = value;
                SetBackground();
            }
            get
            {
                return _xSubSystem;
            }

        }
        #endregion

        #region WhiteSide Property
        public WhiteSideMode _whiteside = WhiteSideMode.Up;
        public WhiteSideMode XWhiteSide
        {
            set
            {
                _whiteside = value;
                SetBackground();
            }
            get
            {
                return _whiteside;
            }
        }
        #endregion

        #region Transparent Property
        public Boolean _transparent = false;
        public Boolean XTransparent
        {
            set
            {
                _transparent = value;
                SetBackground();
            }
            get
            {
                return _transparent;
            }
        }
        #endregion
    }
    #endregion

    #region APM (Border)
    public class APMBorder : Border
    {
        #region Constructor
        public APMBorder()
        {
            CornerRadius = new CornerRadius(10);
            FlowDirection = FlowDirection.RightToLeft;
            BorderThickness = new Thickness(2);
            Margin = new Thickness(1);
            Padding = new Thickness(0);
            _xSubSystem = GlobalVariables.currentSubSystem;
            SetBackground();
        }
        #endregion

        #region SetBackground
        private void SetBackground()
        {
            BorderBrush = SubSystemColors.Items[(int)XSubSystem].BordersBrush;
            Tools.SetBackground(this, _transparent, (int)_whiteside, SubSystemColors.Items[(int)_xSubSystem].BackgroundColor);
        }
        #endregion

        #region SubSystem Property
        public SubSystems _xSubSystem;
        public SubSystems XSubSystem
        {

            set
            {
                _xSubSystem = value;
                SetBackground();
            }
            get
            {
                return _xSubSystem;
            }

        }
        #endregion

        #region WhiteSide Property
        public WhiteSideMode _whiteside = WhiteSideMode.Up;
        public WhiteSideMode XWhiteSide
        {
            set
            {
                _whiteside = value;
                SetBackground();
            }
            get
            {
                return _whiteside;
            }
        }
        #endregion

        #region Transparent Property
        public Boolean _transparent = false;
        public Boolean XTransparent
        {
            set
            {
                _transparent = value;
                SetBackground();
            }
            get
            {
                return _transparent;
            }
        }
        #endregion
    }
    #endregion

    #region APM (BorderComponent)
    public class APMBorderComponent : APMBorder
    {
        #region Constructor
        public APMBorderComponent()
        {
            CornerRadius = new CornerRadius(2);
            Margin = new Thickness(3, 2, 3, 1);
        }
        #endregion
    }

    #endregion

    #region   APM (Button)
    public class APMButton : Button
    {
        #region Constructor
        public APMButton()
        {
            Cursor = Cursors.Hand;
            BorderBrush = Brushes.Silver;
            _xSubSystem = GlobalVariables.currentSubSystem;
            SetBackground();
        }
        #endregion

        #region SetBackground
        private void SetBackground()
        {
            Tools.SetBackground(this, _transparent, (int)_whiteside, SubSystemColors.Items[(int)_xSubSystem].BackgroundColor);
        }
        #endregion

        #region SubSystem Property
        private SubSystems _xSubSystem;
        public SubSystems XSubSystem
        {
            set
            {
                _xSubSystem = value;
                SetBackground();
            }
            get
            {
                return _xSubSystem;
            }

        }
        #endregion

        #region WhiteSide Property
        public WhiteSideMode _whiteside = WhiteSideMode.Up;
        public WhiteSideMode XWhiteSide
        {
            set
            {
                _whiteside = value;
                SetBackground();
            }
            get
            {
                return _whiteside;
            }
        }
        #endregion

        #region Transparent Property
        public Boolean _transparent = false;
        public Boolean XTransparent
        {
            set
            {
                _transparent = value;
                SetBackground();
            }
            get
            {
                return _transparent;
            }
        }
        #endregion
    }
    #endregion

    #region APM (Expander)
    public class APMExpander : Expander
    {
        #region variables
        APMButton HeaderButton = new APMButton();
        #endregion

        #region constructor
        public APMExpander()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            BorderThickness = new Thickness(1);
            Background = Brushes.Transparent;
            Margin = new Thickness(3);
            this.Header = HeaderButton;
            HeaderButton.XWhiteSide = APMComponents.WhiteSideMode.Down;
            HeaderButton.Foreground = Brushes.Gray;
            HeaderButton.Width = 150;
            HeaderButton.Click += new RoutedEventHandler(HeaderButton_Click);
            BorderBrush = Brushes.Silver;
            BorderThickness = new Thickness(1.2);
        }
        #endregion

        #region ExpanderButtonClick
        void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsExpanded == false)
                this.IsExpanded = true;
            else
                this.IsExpanded = false;
        }
        #endregion

        #region Properties
        public SubSystems XSubsystem
        {
            get { return HeaderButton.XSubSystem; }
            set { HeaderButton.XSubSystem = value; }
        }

        public string XCaption
        {
            get { return HeaderButton.Content as string; }
            set { HeaderButton.Content = value; }
        }
        #endregion

    }
    #endregion

    #region APM (TextBox)
    public class APMTextBox : TextBox
    {
        public APMTextBox()
        {
            VerticalContentAlignment = VerticalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            base.FlowDirection = FlowDirection.RightToLeft;
            Margin = new Thickness(0, 2, 0, 2);
            MinWidth = 40;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            /*  راه دوم  */

            /*if (e.Key == Key.Return)
            {
                KeyEventArgs e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource,
                                  0, Key.Tab);
                e1.RoutedEvent = Keyboard.KeyDownEvent;
                InputManager.Current.ProcessInput(e1);
                e.Handled = true;
            }*/

        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            TextBox textBox = (e.OriginalSource) as TextBox;
            textBox.SelectionLength = textBox.Text.Length;

            Background = Brushes.LemonChiffon;
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Background = Brushes.White;
        }
    }
    #endregion

    #region APM (LatinTextBox)
    public class APMLatinTextBox : APMTextBox
    {
        public class user32
        {
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            public static extern int LoadKeyboardLayoutA(string pwszKLID, int Flags);
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            user32.LoadKeyboardLayoutA("00000417", 1);
            base.OnGotFocus(e);
            TextBox tt = new TextBox();
            tt.Focusable = true;
            tt.Focus();
            this.Focus();
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            user32.LoadKeyboardLayoutA("00000429", 1);
        }
    }
    #endregion

    #region APM (NumericTextBox)
    public class APMIntTextBox : APMTextBox
    {
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            short val;
            if (!Int16.TryParse(e.Text, out val))
            {
                e.Handled = true;
            }
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
    #endregion

    #region APM (FloatTextBox)

    public class APMFloatTextBox : APMTextBox
    {
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            double val;
            if (!Double.TryParse(e.Text, out val) && (e.Text != "."))
            {
                e.Handled = true;
            }
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }

    #endregion

    #region APM (MoneyTextBox)
    public class APMMoneyTextBox : APMIntTextBox
    {
        public char zero
        {
            get
            {
                return '.';
            }
        }
        public enum CountZero
        {
            _3zero, _6zero, _9zero
        }
        private CountZero _CountZero;
        public CountZero ModeZero
        {
            get
            {
                return _CountZero;
            }
            set
            {
                _CountZero = value;
            }
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            short val;
            if (!Int16.TryParse(e.Text, out val))
            {
                e.Handled = true;
            }


            if (e.Text == zero.ToString())
            {
                if (_CountZero == CountZero._3zero)
                {
                    this.Text += "000";
                }
                if (_CountZero == CountZero._6zero)
                {
                    this.Text += "000000";
                }
                if (_CountZero == CountZero._9zero)
                {
                    this.Text += "000000000";
                }
            }
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (this.Text == "0")
                SelectionStart = 1;
            base.OnGotFocus(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (this.Text == "0")
                SelectionStart = 1;
            base.OnMouseUp(e);
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Back)
            {
                if (SelectionStart > 0 && Text[SelectionStart - 1] == ',')
                    SelectionStart--;
            }
            else if (e.Key == Key.Delete)
            {
                if (SelectionStart < Text.Length && Text[SelectionStart] == ',')
                    SelectionStart++;
            }
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            long val;
            string number = "";
            foreach (char ch in this.Text)
                if (ch != ',')
                    number = number + ch.ToString();


            if (long.TryParse(number, out val))
            {
                int preSelectionStart = SelectionStart;
                int preLength = Text.Length;
                this.Text = long.Parse(number, NumberStyles.AllowThousands).ToString("N0");

                if (Text.Length > preLength)
                    SelectionStart = preSelectionStart + 1;
                else if (preSelectionStart > 0 && Text.Length < preLength)
                    SelectionStart = preSelectionStart - 1;
                else
                    SelectionStart = preSelectionStart;
            }
            else
            {
                this.Text = Convert.ToString(0);
                this.SelectAll();
            }
        }
    }

    #endregion

    #region APM (ComboBox)
    public class APMComboBox : ComboBox
    {
        #region constructor
        public APMComboBox()
        {
            Margin = new Thickness(0, 2, 0, 2);
            Background = Brushes.White;
            FlowDirection = FlowDirection.LeftToRight;
            MinWidth = 40;
            VerticalAlignment = VerticalAlignment.Center;
        }
        #endregion

        #region Property
        private Type _sourceType;
        public Type XSourceType
        {
            get { return _sourceType; }
            set { _sourceType = value; }
        }
        #endregion

        #region Events
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Return)
            {
                KeyEventArgs e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab);
                e1.RoutedEvent = Keyboard.KeyDownEvent;
                InputManager.Current.ProcessInput(e1);
            }
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            Background = Brushes.LemonChiffon;
            InputLanguageManager.SetInputLanguage(this, System.Globalization.CultureInfo.CreateSpecificCulture("fa-ir"));
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Background = Brushes.White;
        }
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            this.SelectedIndex = 0;
        }
        #endregion
    }
    #endregion

    #region APM (ComboBoxFiscalYear)
    //public class APMComboBoxFiscalYear : ComboBox
    //{
    //    #region Variables
    //    int index;
    //    #endregion

    //    #region Constructor
    //    public APMComboBoxFiscalYear()
    //    {
    //        Background = Brushes.Transparent;
    //        Cursor = Cursors.Hand;
    //        BorderBrush = Brushes.Transparent;
    //        BorderThickness = new Thickness(0);
    //        FlowDirection = FlowDirection.LeftToRight;
    //        SelectionChanged += new SelectionChangedEventHandler(ComboBoxFiscalYear_SelectionChanged);
    //        Refresh();
    //    }
    //    #endregion

    //    #region Tools
    //    public void Refresh()
    //    {
    //        BLL<stp_glb_fiscal_year_selResult> bll_glb_fiscal_year = new BLL<stp_glb_fiscal_year_selResult>();
    //        bll_glb_fiscal_year.ClearAllRecord();
    //        bll_glb_fiscal_year.FillComboBoxForShow(this, new stp_glb_fiscal_year_selResult() { glb_fiscal_year_glb_branch_id = GlobalVariables.current_branch_id });
    //        if (XGeneric)
    //        {
    //            index = this.Items.Count - 1;
    //            this.SelectedIndex = index;
    //        }
    //        else
    //            this.SelectedIndex = GlobalVariables.fiscal_year_index;
    //    }
    //    #endregion

    //    #region Event
    //    private void ComboBoxFiscalYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {
    //        if (this.SelectedItem != null)
    //            DDB.ConnectToDataBase(((stp_glb_fiscal_year_selResult)this.SelectedItem).glb_fiscal_year_database_name);
    //        if (XGeneric)
    //        {
    //            GlobalVariables.current_fiscal_year_database_name = this.SelectedIndex != -1 ? ((stp_glb_fiscal_year_selResult)this.SelectedItem).glb_fiscal_year_database_name : "";
    //            GlobalVariables.current_fiscal_year_name = this.SelectedIndex != -1 ? ((stp_glb_fiscal_year_selResult)this.SelectedItem).glb_fiscal_year_name : "";
    //            GlobalVariables.fiscal_year_index = this.SelectedIndex;
    //        }
    //    }
    //    #endregion

    //    #region Properties
    //    private bool _xGeneric;
    //    public bool XGeneric
    //    {
    //        get { return _xGeneric; }
    //        set { _xGeneric = value; }
    //    }
    //    #endregion
    //}
    #endregion

    #region APM (ComboBoxCoding)
    public class APMComboBoxCoding : APMComboBox
    {
        #region XCategory
        private CodingCategory _category;
        public CodingCategory XCategory
        {
            get { return _category; }
            set { _category = value; }
        }
        #endregion
    }
    #endregion

    #region APM (TabControl)

    public class APMTabControl : TabControl
    {
        #region Constructor
        public APMTabControl()
        {

            _xSubSystem = GlobalVariables.currentSubSystem;

            this.Margin = new Thickness(5);
            SetBackground();
        }
        #endregion

        #region SubSystem Property
        public SubSystems _xSubSystem;
        public SubSystems XSubSystem
        {
            set
            {
                _xSubSystem = value;
                SetBackground();
            }
            get
            {
                return _xSubSystem;
            }

        }
        #endregion

        #region SetBackground
        private void SetBackground()
        {
            Tools.SetBackground(this, _transparent, (int)_whiteside, SubSystemColors.Items[(int)_xSubSystem].BackgroundColor);
            foreach (object tabitem in this.Items)
            {
                if (tabitem is TabItem)
                    (tabitem as TabItem).Background = SubSystemColors.Items[(int)XSubSystem].TabItemHeaderBackGround;

            }
        }
        #endregion

        #region WhiteSide Property
        public WhiteSideMode _whiteside = WhiteSideMode.Up;
        public WhiteSideMode XWhiteSide
        {
            set
            {
                _whiteside = value;
                SetBackground();
            }
            get
            {
                return _whiteside;
            }
        }
        #endregion

        #region Transparent Property
        public Boolean _transparent = false;
        public Boolean XTransparent
        {
            set
            {
                _transparent = value;
                SetBackground();
            }
            get
            {
                return _transparent;
            }
        }
        #endregion


    }

    #endregion

    #region APM (TabItem)
    public class APMTabItem : TabItem
    {
        #region Constructor
        public APMTabItem()
        {

            _xSubSystem = GlobalVariables.currentSubSystem;

            SetBackground();


        }

        #endregion

        #region SubSystem Property
        public SubSystems _xSubSystem;
        public SubSystems XSubSystem
        {
            set
            {
                _xSubSystem = value;
                SetBackground();
            }
            get
            {
                return _xSubSystem;
            }

        }
        #endregion

        #region SetBackground
        private void SetBackground()
        {
            this.Background = SubSystemColors.Items[(int)XSubSystem].TabItemHeaderBackGround;

        }
        #endregion

    }
    #endregion

    #region APM (Label)
    public class APMLabel : Label
    {
        #region Constructor
        public APMLabel()
        {
            HorizontalAlignment = HorizontalAlignment.Right;
            HorizontalContentAlignment = HorizontalAlignment.Right;
            Margin = new Thickness(1);
            VerticalAlignment = VerticalAlignment.Center;
        }
        #endregion
    }

    #endregion

    #region APM (InfoLabel)
    public class APMInfoLabel : APMLabel
    {
        #region Constructor
        public APMInfoLabel()
        {
            HorizontalContentAlignment = HorizontalAlignment.Left;
            BorderBrush = Brushes.Gray;
            BorderThickness = new Thickness(0);
            MinHeight = 22;
            VerticalAlignment = VerticalAlignment.Center;
            this.XHaveBorder = false;
        }
        #endregion

        #region Properties
        private Boolean _haveBorder = false;
        public Boolean XHaveBorder
        {
            get { return _haveBorder; }
            set
            {
                _haveBorder = value;
                if (value == true)
                    this.BorderThickness = new Thickness(1);
                else
                    this.BorderThickness = new Thickness(0);
            }
        }

        #endregion
    }
    #endregion

    #region APM (DockPanel)
    public class APMDockPanel : DockPanel
    {
        public APMDockPanel()
        {
            LastChildFill = true;
            Margin = new Thickness(3);
            Background = Brushes.Transparent;

        }
    }
    #endregion

    #region APM (CheckBox)
    public class APMCheckBox : CheckBox
    {
        public APMCheckBox()
        {
            VerticalAlignment = VerticalAlignment.Center;
        }
    }
    #endregion APMCheckBox

    #region APM (RadioButton)
    public class APMRadioButton : RadioButton
    {
        public APMRadioButton()
        {
            VerticalAlignment = VerticalAlignment.Center;
        }
    }

    #endregion

    #region APM (DockPanelMain)
    public class APMDockPanelMain : DockPanel
    {
        #region Constructor
        public APMDockPanelMain()
        {
            SetResourceReference(APMDockPanelMain.BackgroundProperty, "SideExpanderBackground");
            FlowDirection = FlowDirection.RightToLeft;
            LastChildFill = false;
        }
        #endregion
    }
    #endregion

    #region APM (ProgressBar)
    public class APMProgressBar : ProgressBar
    {
        #region Variables
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        #endregion

        #region Constructor
        public APMProgressBar()
        {
            Minimum = 0;
            Maximum = 100;
        }
        #endregion

        #region Methode
        public void XProgress()
        {

            Value = 0;
            double value = 0;
            UpdateProgressBarDelegate updatePbDelegate =
               new UpdateProgressBarDelegate(this.SetValue);

            do
            {
                value += 1;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
            }
            while (this.Value != this.Maximum);
        }
        public void XRefresh()
        {
            this.Value = 0;
        }
        #endregion

        #region Property
        public double XMinimum
        {
            get { return this.Minimum; }
            set { this.Minimum = value; }
        }
        public double XMaximum
        {
            get { return this.Maximum; }
            set { this.Maximum = value; }
        }
        #endregion
    }
    #endregion

    #region MaskedTextBox
    public class MaskedTextBox : TextBox
    {
        public static readonly DependencyProperty InputMaskProperty;
        private List<InputMaskChar> _maskChars;
        private int _caretIndex;
        static MaskedTextBox()
        {
            TextProperty.OverrideMetadata(typeof(MaskedTextBox),
                new FrameworkPropertyMetadata(null, new CoerceValueCallback(Text_CoerceValue)));
            InputMaskProperty = DependencyProperty.Register("InputMask", typeof(string), typeof(MaskedTextBox),
                new PropertyMetadata(string.Empty, new PropertyChangedCallback(InputMask_Changed)));
        }
        public MaskedTextBox()
        {
            this._maskChars = new List<InputMaskChar>();
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(MaskedTextBox_Paste));
        }
        public string InputMask
        {
            get { return this.GetValue(InputMaskProperty) as string; }
            set { this.SetValue(InputMaskProperty, value); }
        }

        [Flags]
        protected enum InputMaskValidationFlags
        {
            None = 0,
            AllowInteger = 1,
            AllowDecimal = 2,
            AllowAlphabet = 4,
            AllowAlphanumeric = 8
        }

        public bool IsTextValid()
        {
            string value;
            return this.ValidateTextInternal(this.Text, out value);
        }

        private class InputMaskChar
        {
            private InputMaskValidationFlags _validationFlags;
            private char _literal;
            public InputMaskChar(InputMaskValidationFlags validationFlags)
            {
                this._validationFlags = validationFlags;
                this._literal = (char)0;
            }
            public InputMaskChar(char literal)
            {
                this._literal = literal;
            }
            public InputMaskValidationFlags ValidationFlags
            {
                get { return this._validationFlags; }
                set { this._validationFlags = value; }
            }
            public char Literal
            {
                get { return this._literal; }
                set { this._literal = value; }
            }
            public bool IsLiteral()
            {
                return (this._literal != (char)0);
            }
            public char GetDefaultChar()
            {
                return (this.IsLiteral()) ? this.Literal : '_';
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            this._caretIndex = this.CaretIndex;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            //no mask specified, just function as a normal textbox
            if (this._maskChars.Count == 0)
                return;

            if (e.Key == Key.Delete)
            {
                //delete key pressed: delete all text
                this.Text = this.GetDefaultText();
                this._caretIndex = this.CaretIndex = 0;
                e.Handled = true;
            }
            else
            {
                //backspace key pressed
                if (e.Key == Key.Back)
                {
                    if (this._caretIndex > 0 || this.SelectionLength > 0)
                    {
                        if (this.SelectionLength > 0)
                        {
                            //if one or more characters selected, delete them
                            this.DeleteSelectedText();
                        }
                        else
                        {
                            //if no characters selected, shift the caret back to the previous non-literal char and delete it
                            this.MoveBack();

                            char[] characters = this.Text.ToCharArray();
                            characters[this._caretIndex] = this._maskChars[this._caretIndex].GetDefaultChar();
                            this.Text = new string(characters);
                        }

                        //update the base class caret index, and swallow the event
                        this.CaretIndex = this._caretIndex;
                        e.Handled = true;
                    }
                }
                else if (e.Key == Key.Left)
                {
                    //move back to the previous non-literal character
                    this.MoveBack();
                    e.Handled = true;
                }
                else if (e.Key == Key.Right || e.Key == Key.Space)
                {
                    //move forwards to the next non-literal character
                    this.MoveForward();
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {

            base.OnPreviewTextInput(e);

            //no mask specified, just function as a normal textbox
            if (this._maskChars.Count == 0)
                return;

            this._caretIndex = this.CaretIndex = this.SelectionStart;

            if (this._caretIndex == this._maskChars.Count)
            {
                //at the end of the character count defined by the input mask- no more characters allowed
                e.Handled = true;
            }
            else
            {
                //validate the character against its validation scheme
                bool isValid = this.ValidateInputChar(char.Parse(e.Text),
                    this._maskChars[this._caretIndex].ValidationFlags);

                if (isValid)
                {
                    //delete any selected text
                    if (this.SelectionLength > 0)
                    {
                        this.DeleteSelectedText();
                    }

                    //insert the new character
                    char[] characters = this.Text.ToCharArray();
                    characters[this._caretIndex] = char.Parse(e.Text);
                    this.Text = new string(characters);

                    //move the caret on 
                    this.MoveForward();
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Validates the specified character against all selected validation schemes.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validationFlags"></param>
        /// <returns></returns>
        protected virtual bool ValidateInputChar(char input, InputMaskValidationFlags validationFlags)
        {
            bool valid = (validationFlags == InputMaskValidationFlags.None);

            if (!valid)
            {
                Array values = Enum.GetValues(typeof(InputMaskValidationFlags));

                //iterate through the validation schemes
                foreach (object o in values)
                {
                    InputMaskValidationFlags instance = (InputMaskValidationFlags)(int)o;
                    if ((instance & validationFlags) != 0)
                    {
                        if (this.ValidateCharInternal(input, instance))
                        {
                            valid = true;
                            break;
                        }
                    }
                }
            }

            return valid;
        }

        /// <summary>
        /// Returns a value indicating if the current text value is valid.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateTextInternal(string text, out string displayText)
        {
            if (this._maskChars.Count == 0)
            {
                displayText = text;
                return true;
            }

            StringBuilder displayTextBuilder = new StringBuilder(this.GetDefaultText());

            bool valid = (!string.IsNullOrEmpty(text) &&
                text.Length <= this._maskChars.Count);

            if (valid)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (!this._maskChars[i].IsLiteral())
                    {
                        if (this.ValidateInputChar(text[i], this._maskChars[i].ValidationFlags))
                        {
                            displayTextBuilder[i] = text[i];
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
            }

            displayText = displayTextBuilder.ToString();

            return valid;
        }

        /// <summary>
        /// Deletes the currently selected text.
        /// </summary>
        protected virtual void DeleteSelectedText()
        {
            StringBuilder text = new StringBuilder(this.Text);
            string defaultText = this.GetDefaultText();
            int selectionStart = this.SelectionStart;
            int selectionLength = this.SelectionLength;

            text.Remove(selectionStart, selectionLength);
            text.Insert(selectionStart, defaultText.Substring(selectionStart, selectionLength));
            this.Text = text.ToString();

            //reset the caret position
            this.CaretIndex = this._caretIndex = selectionStart;
        }

        /// <summary>
        /// Returns a value indicating if the specified input mask character is a placeholder.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="validationFlags">If the character is a placeholder, returns the relevant validation scheme.</param>
        /// <returns></returns>
        protected virtual bool IsPlaceholderChar(char character, out InputMaskValidationFlags validationFlags)
        {
            validationFlags = InputMaskValidationFlags.None;

            switch (character.ToString().ToUpper())
            {
                case "I":
                    validationFlags = InputMaskValidationFlags.AllowInteger;
                    break;
                case "D":
                    validationFlags = InputMaskValidationFlags.AllowDecimal;
                    break;
                case "A":
                    validationFlags = InputMaskValidationFlags.AllowAlphabet;
                    break;
                case "W":
                    validationFlags = (InputMaskValidationFlags.AllowAlphanumeric);
                    break;
            }

            return (validationFlags != InputMaskValidationFlags.None);
        }

        /// <summary>
        /// Invoked when the coerce value callback is invoked.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static object Text_CoerceValue(DependencyObject obj, object value)
        {
            MaskedTextBox mtb = (MaskedTextBox)obj;

            if (value == null || value.Equals(string.Empty))
                value = mtb.GetDefaultText();
            else if (value.ToString().Length > 0)
            {
                string displayText;
                mtb.ValidateTextInternal(value.ToString(), out displayText);
                value = displayText;
            }

            return value;
        }

        /// <summary>
        /// Invoked when the InputMask dependency property reports a change.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void InputMask_Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as MaskedTextBox).UpdateInputMask();
        }

        /// <summary>
        /// Invokes when a paste event is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaskedTextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            //TODO: play nicely here?
            //

            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string value = e.DataObject.GetData(typeof(string)).ToString();
                string displayText;

                if (this.ValidateTextInternal(value, out displayText))
                {
                    this.Text = displayText;
                }
            }

            e.CancelCommand();
        }

        /// <summary>
        /// Rebuilds the InputMaskChars collection when the input mask property is updated.
        /// </summary>
        private void UpdateInputMask()
        {

            string text = this.Text;
            this._maskChars.Clear();

            this.Text = string.Empty;

            string mask = this.InputMask;

            if (string.IsNullOrEmpty(mask))
                return;

            InputMaskValidationFlags validationFlags = InputMaskValidationFlags.None;

            for (int i = 0; i < mask.Length; i++)
            {
                bool isPlaceholder = this.IsPlaceholderChar(mask[i], out validationFlags);

                if (isPlaceholder)
                {
                    this._maskChars.Add(new InputMaskChar(validationFlags));
                }
                else
                {
                    this._maskChars.Add(new InputMaskChar(mask[i]));
                }
            }

            string displayText;
            if (text.Length > 0 && this.ValidateTextInternal(text, out displayText))
            {
                this.Text = displayText;
            }
            else
            {
                this.Text = this.GetDefaultText();
            }
        }

        /// <summary>
        /// Validates the specified character against its input mask validation scheme.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validationType"></param>
        /// <returns></returns>
        private bool ValidateCharInternal(char input, InputMaskValidationFlags validationType)
        {
            bool valid = false;

            switch (validationType)
            {
                case InputMaskValidationFlags.AllowInteger:
                case InputMaskValidationFlags.AllowDecimal:
                    int i;
                    if (validationType == InputMaskValidationFlags.AllowDecimal &&
                        input == '.' && !this.Text.Contains('.'))
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = int.TryParse(input.ToString(), out i);
                    }
                    break;
                case InputMaskValidationFlags.AllowAlphabet:
                    valid = char.IsLetter(input);
                    break;
                case InputMaskValidationFlags.AllowAlphanumeric:
                    valid = (char.IsLetter(input) || char.IsNumber(input));
                    break;
            }

            return valid;
        }

        /// <summary>
        /// Builds the default display text for the control.
        /// </summary>
        /// <returns></returns>
        private string GetDefaultText()
        {
            StringBuilder text = new StringBuilder();
            foreach (InputMaskChar maskChar in this._maskChars)
            {
                text.Append(maskChar.GetDefaultChar());
            }
            return text.ToString();
        }

        /// <summary>
        /// Moves the caret forward to the next non-literal position.
        /// </summary>
        private void MoveForward()
        {
            int pos = this._caretIndex;
            while (pos < this._maskChars.Count)
            {
                if (++pos == this._maskChars.Count || !this._maskChars[pos].IsLiteral())
                {
                    this._caretIndex = this.CaretIndex = pos;
                    break;
                }
            }
        }

        /// <summary>
        /// Moves the caret backward to the previous non-literal position.
        /// </summary>
        private void MoveBack()
        {
            int pos = this._caretIndex;
            while (pos > 0)
            {
                if (--pos == 0 || !this._maskChars[pos].IsLiteral())
                {
                    this._caretIndex = this.CaretIndex = pos;
                    break;
                }
            }
        }
    }
    #endregion

    #region APM (BarCode)

    #region Barcodes
    public class Barcodes
    {


        public enum YesNoEnum
        {
            Yes,
            No
        }

        public enum BarcodeEnum
        {
            Code39
        }

        public string Data
        {
            get { return data; }
            set { data = value; }
        }
        private string data;

        public BarcodeEnum BarcodeType
        {
            get { return barcodeType; }
            set { barcodeType = value; }
        }
        private BarcodeEnum barcodeType;

        public YesNoEnum CheckDigit
        {
            get { return checkDigit; }
            set { checkDigit = value; }
        }
        private YesNoEnum checkDigit;

        public string HumanText
        {
            get
            {
                return humanText;
            }
            set { humanText = value; }
        }

        private string humanText;

        public string EncodedData
        {
            get { return encodedData; }
            set { encodedData = value; }
        }
        private string encodedData;

        public void encode()
        {
            int check = 0;
            if (checkDigit == Barcodes.YesNoEnum.Yes)
                check = 1;

            if (barcodeType == BarcodeEnum.Code39)
            {
                Code39 barcode = new Code39();
                encodedData = barcode.encode(data, check);
                humanText = barcode.getHumanText();
            }
        }
    }
    #endregion

    #region Code39
    public class Code39
    {
        //w - wide
        //t - thin
        //Start the drawing with black, white, black, white......
        public string encode(string data, int chk)
        {
            string fontOutput = mcode(data, chk);
            string output = "";
            string pattern = "";
            for (int x = 0; x < fontOutput.Length; x++)
            {
                switch (fontOutput[x])
                {
                    case '1':
                        pattern = "wttwttttwt";
                        break;
                    case '2':
                        pattern = "ttwwttttwt";
                        break;
                    case '3':
                        pattern = "wtwwtttttt";
                        break;
                    case '4':
                        pattern = "tttwwtttwt";
                        break;
                    case '5':
                        pattern = "wttwwttttt";
                        break;
                    case '6':
                        pattern = "ttwwwttttt";
                        break;
                    case '7':
                        pattern = "tttwttwtwt";
                        break;
                    case '8':
                        pattern = "wttwttwttt";
                        break;
                    case '9':
                        pattern = "ttwwttwttt";
                        break;
                    case '0':
                        pattern = "tttwwtwttt";
                        break;
                    case 'A':
                        pattern = "wttttwttwt";
                        break;
                    case 'B':
                        pattern = "ttwttwttwt";
                        break;
                    case 'C':
                        pattern = "wtwttwtttt";
                        break;
                    case 'D':
                        pattern = "ttttwwttwt";
                        break;
                    case 'E':
                        pattern = "wtttwwtttt";
                        break;
                    case 'F':
                        pattern = "ttwtwwtttt";
                        break;
                    case 'G':
                        pattern = "tttttwwtwt";
                        break;
                    case 'H':
                        pattern = "wttttwwttt";
                        break;
                    case 'I':
                        pattern = "ttwttwwttt";
                        break;
                    case 'J':
                        pattern = "ttttwwwttt";
                        break;
                    case 'K':
                        pattern = "wttttttwwt";
                        break;
                    case 'L':
                        pattern = "ttwttttwwt";
                        break;
                    case 'M':
                        pattern = "wtwttttwtt";
                        break;
                    case 'N':
                        pattern = "ttttwttwwt";
                        break;
                    case 'O':
                        pattern = "wtttwttwtt";
                        break;
                    case 'P':
                        pattern = "ttwtwttwtt";
                        break;
                    case 'Q':
                        pattern = "ttttttwwwt";
                        break;
                    case 'R':
                        pattern = "wtttttwwtt";
                        break;
                    case 'S':
                        pattern = "ttwtttwwtt";
                        break;
                    case 'T':
                        pattern = "ttttwtwwtt";
                        break;
                    case 'U':
                        pattern = "wwttttttwt";
                        break;
                    case 'V':
                        pattern = "twwtttttwt";
                        break;
                    case 'W':
                        pattern = "wwwttttttt";
                        break;
                    case 'X':
                        pattern = "twttwtttwt";
                        break;
                    case 'Y':
                        pattern = "wwttwttttt";
                        break;
                    case 'Z':
                        pattern = "twwtwttttt";
                        break;
                    case '-':
                        pattern = "twttttwtwt";
                        break;
                    case '.':
                        pattern = "wwttttwttt";
                        break;
                    case ' ':
                        pattern = "twwtttwttt";
                        break;
                    case '*':
                        pattern = "twttwtwttt";
                        break;
                    case '$':
                        pattern = "twtwtwtttt";
                        break;
                    case '/':
                        pattern = "twtwtttwtt";
                        break;
                    case '+':
                        pattern = "twtttwtwtt";
                        break;
                    case '%':
                        pattern = "tttwtwtwtt";
                        break;
                }
                output = output.Insert(output.Length, pattern);
            }
            return output;
        }

        private string humanText;
        static char[] CODE39MAP = {'0','1','2','3','4','5','6','7','8','9',
							 'A','B','C','D','E','F','G','H','I','J',
							 'K','L','M','N','O','P','Q','R','S','T',
							 'U','V','W','X','Y','Z','-','.',' ','$',    
							 '/','+','%'};

        private string mcode(string data, int chk)
        {
            string cd = "", result = "";
            string filtereddata = filterInput(data);
            int filteredlength = filtereddata.Length;

            if (chk == 1)
            {
                if (filteredlength > 254)
                    filtereddata = filtereddata.Remove(254, filteredlength - 254);
                cd = generateCheckDigit(filtereddata);
            }
            else
            {
                if (filteredlength > 255)
                    filtereddata = filtereddata.Remove(255, filteredlength - 255);
            }

            result = "*" + filtereddata + "*";

            humanText = result;
            return result;
        }

        public string getHumanText()
        {
            return humanText;
        }

        string generateCheckDigit(string data)
        {

            int datalength = 0;
            int sum = 0;
            int result = -1;
            string strResult = "";
            char barcodechar;

            datalength = data.Length;
            for (int x = 0; x < datalength; x++)
            {
                barcodechar = data[x];
                sum = sum + getCode39Value(barcodechar);
            }
            result = sum % 43;
            strResult = getCode39Character(result).ToString();

            return strResult;

        }
        char getCode39Character(int inputdecimal)
        {
            return CODE39MAP[inputdecimal];
        }

        int getCode39Value(char inputchar)
        {
            for (int x = 0; x < 43; x++)
            {
                if (CODE39MAP[x] == inputchar)
                    return x;
            }
            return -1;
        }

        string filterInput(string data)
        {
            string result = "";
            int datalength = data.Length;

            for (int x = 0; x < datalength; x++)
            {
                char barcodechar = data[x];
                if (getCode39Value(barcodechar) != -1)
                    result = result.Insert(result.Length, barcodechar.ToString());
            }

            return result;
        }

    }
    #endregion

    #region APMBarCode
    public class APMBarCode : StackPanel
    {
        #region Define Variables
        private StackPanel spBarcode = new StackPanel();
        private TextBox txtBarcode = new TextBox();
        private Button btnBarcode = new Button();
        private Canvas cnvBarcode = new Canvas();

        #endregion
        #region Constructor
        public APMBarCode()
        {

            spBarcode.Orientation = Orientation.Horizontal;

            txtBarcode.HorizontalAlignment = HorizontalAlignment.Right;
            txtBarcode.Width = 120;


            btnBarcode.Content = "تولید بارکد";
            btnBarcode.Click += new RoutedEventHandler(btnBarcode_Click);


            cnvBarcode.Width = 120;
            //cnvBarcode.Height = 40;
            spBarcode.Children.Add(txtBarcode);
            spBarcode.Children.Add(btnBarcode);
            spBarcode.Children.Add(cnvBarcode);
            this.Children.Add(spBarcode);
        }
        #endregion

        #region Barcode_Click
        private void btnBarcode_Click(object sender, RoutedEventArgs e)
        {

            cnvBarcode.Children.Clear();
            /////////////////////////////////////
            // Encode The Data
            /////////////////////////////////////
            Barcodes bb = new Barcodes();
            bb.BarcodeType = Barcodes.BarcodeEnum.Code39;
            //bb.Data = "1234567";
            //bb.Data = "A";
            bb.Data = txtBarcode.Text.Trim();

            bb.CheckDigit = Barcodes.YesNoEnum.Yes;
            bb.encode();

            int thinWidth;
            int thickWidth;

            thinWidth = 3;
            thickWidth = 3 * thinWidth;

            string outputString = bb.EncodedData;
            string humanText = bb.HumanText;


            /////////////////////////////////////
            // Draw The Barcode
            /////////////////////////////////////
            int len = outputString.Length;
            int currentPos = 5;
            int currentTop = 0;
            int currentColor = 0;
            for (int i = 0; i < len; i++)
            {
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
                //EDIT// rect.Height = 200;
                rect.Height = 20;
                //rect.Width = 10;
                if (currentColor == 0)
                {
                    currentColor = 1;
                    rect.Fill = new SolidColorBrush(Colors.Black);

                }
                else
                {
                    currentColor = 0;
                    rect.Fill = new SolidColorBrush(Colors.White);

                }
                Canvas.SetLeft(rect, currentPos);
                Canvas.SetTop(rect, currentTop);

                if (outputString[i] == 't')
                {
                    rect.Width = thinWidth;
                    currentPos += thinWidth;

                }
                else if (outputString[i] == 'w')
                {
                    rect.Width = thickWidth;
                    currentPos += thickWidth;

                }
                cnvBarcode.Children.Add(rect);

            }


            /////////////////////////////////////
            // Add the Human Readable Text
            /////////////////////////////////////
            TextBlock tb = new TextBlock();
            //DELETE//tb.Text = humanText;
            //EDIT//tb.FontSize = 32;
            tb.FontSize = 10;
            tb.FontFamily = new FontFamily("Courier New");
            Rect rx = new Rect(0, 0, 0, 0);
            tb.Arrange(rx);
            Canvas.SetLeft(tb, (currentPos - tb.ActualWidth) / 2);
            Canvas.SetTop(tb, currentTop + 205);
            cnvBarcode.Children.Add(tb);



        }
        #endregion
    }
    #endregion

    #endregion

    #region PersianDate

    #region PersianDate
    //this attribute enables converting to/from this type in wpf and other designing environments
    [TypeConverter(typeof(PersianDateConverter))]

    public struct PersianDate : IComparable<PersianDate>
    {
        const int period33y = 365 * 33 + 8;

        const int p33p1 = 366;
        const int p33p2 = 365 * 20 + 4;
        const int p33p3 = 366;
        const int p33p4 = 365 * 11 + 2;

        const int offset = 365 * 22 + 7;
        const int offsety = 22;
        const int period4y = 365 * 4 + 1;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        static void yearMonthDay(uint x, out int y, out int m, out int d)
        {
            int a = (int)(x / period33y);
            y = a * 33;
            a = (int)(x % period33y);
            int b;
            if (a < p33p1)
            {
                b = 0;
                monthAndDay(a, out m, out d);
            }
            else if (a < p33p1 + p33p2)
            {
                int rem = (a - p33p1) % period4y;
                b = 1 + (a - p33p1) / period4y * 4 + yearInP4y(rem);
                monthAndDay(rem - 365 * yearInP4y(rem), out m, out d);
            }
            else if (a < p33p1 + p33p2 + p33p3)
            {
                b = 21;
                monthAndDay(a - p33p1 - p33p2, out m, out d);
            }
            else
            {
                int rem = (a - p33p1 - p33p2 - p33p3) % period4y;

                b = (a - p33p1 - p33p2 - p33p3) / period4y * 4 + 22 + yearInP4y(rem);
                monthAndDay(rem - 365 * yearInP4y(rem), out m, out d);
            }

            y += b + 1;

        }
        static void monthAndDay(int x, out int m, out int d)
        {
            if (x >= 366) throw new ArgumentException("X must be less than or equal to 365");
            if (x >= 31 * 6)
            {
                m = (x - 31 * 6) / 30 + 7;
                d = (x - 31 * 6) % 30 + 1;
            }
            else
            {
                m = x / 31 + 1;
                d = x % 31 + 1;
            }
        }
        static int yearInP4y(int x)
        {
            if (x > period4y) throw new ArgumentException("X must be less than or equal to period4y");
            int r = x / 365;
            return r < 4 ? r : 3;
        }




        //max date:"11759224/6/25"
        const int maxYear = 11759224, maxMonth = 6, maxDay = 25;
        //min date: "1/1/1"
        const int minYear = 1, minMonth = 1, minDay = 1;
        //3/21/0622 12:00:00 AM
        static DateTime firstDateDateTime = new DateTime(622, 3, 21);

        /// <summary>
        /// compares 2 dates with no validation
        /// </summary>
        static int comp(int y1, int m1, int d1, int y2, int m2, int d2)
        {
            if (y1 != y2) return y1 - y2;
            if (m1 != m2) return m1 - m2;
            return d1 - d2;
        }

        static void yearAndDaysInYear(uint x, out int y, out int ds)
        {
            int a = (int)(x / period33y);
            y = a * 33;
            a = (int)(x % period33y);
            int b;
            if (a < p33p1)
            {
                b = 0;
                ds = a;//monthAndDay(a, out m, out d);
            }
            else if (a < p33p1 + p33p2)
            {
                int rem = (a - p33p1) % period4y;
                b = 1 + (a - p33p1) / period4y * 4 + yearInP4y(rem);
                ds = rem - 365 * yearInP4y(rem);//monthAndDay(rem - 365 * yearInP4y(rem), out m, out d);
            }
            else if (a < p33p1 + p33p2 + p33p3)
            {
                b = 21;
                ds = a - p33p1 - p33p2;//monthAndDay(a - p33p1 - p33p2, out m, out d);
            }
            else
            {
                int rem = (a - p33p1 - p33p2 - p33p3) % period4y;

                b = (a - p33p1 - p33p2 - p33p3) / period4y * 4 + 22 + yearInP4y(rem);
                ds = rem - 365 * yearInP4y(rem);//monthAndDay(rem - 365 * yearInP4y(rem), out m, out d);
            }

            y += b + 1;

        }

        static int daysInYear(int m, int d)
        {
            if (m < 7) return (m - 1) * 31 + d - 1;
            return (31 * 6) + (m - 7) * 30 + d - 1;
        }
        static uint days(int y, int m, int d)
        {
            uint r;
            r = (uint)(y - 1) / 33 * period33y + (uint)daysInYear(m, d);
            int a = (y - 1) % 33;
            if (a == 0)
            {

            }
            else if (a <= 20)
            {
                r += (uint)p33p1 + (uint)((a - 1) / 4) * period4y + (uint)((a - 1) % 4) * 365;
            }
            else if (a <= 21)
            {
                r += p33p1 + p33p2;
            }
            else
            {
                r += (uint)(p33p1 + p33p2 + p33p3) + (uint)((a - 22) / 4) * period4y + (uint)((a - 22) % 4) * 365;
            }
            return r;
        }

        /// <summary>
        /// Checks whether the given date is a valid date
        /// </summary>
        public static bool IsValid(int year, int month, int day)
        {
            if (month < 1 || month > 12) return false;
            if (day < 1) return false;
            if (month < 7 && day > 31) return false;
            if (month >= 7 && day > 30) return false;
            if (month == 12 && day > 29 && !IsLeapYear(year)) return false;
            return true;
        }

        public static bool IsLeapYear(int year)
        {
            int r = year % 33;
            return (r == 1 || r == 5 || r == 9 || r == 13 || r == 17 || r == 22 || r == 26 || r == 30);
        }
        public static PersianDate Today
        {
            get
            {
                return new PersianDate(DateTime.Today);
            }
        }
        public static int DaysInMonth(int year, int month)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException("month", "Month must be between 1 and 12");
            if (month <= 6) return 31;
            if (month <= 11) return 30;
            if (IsLeapYear(year)) return 30;
            return 29;
        }
        /// <summary>
        /// Converts the specified string representation of a persian date to its equivalent PersianDate value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static PersianDate Parse(string str)
        {
            string[] parts = str.Split('/');
            if (parts.Length != 3) throw new ArgumentException("The date string must be in the form y/m/d");
            int y, m, d;
            var style = System.Globalization.NumberStyles.AllowLeadingWhite |
                System.Globalization.NumberStyles.AllowTrailingWhite;
            try
            {
                y = int.Parse(parts[0], style);
                m = int.Parse(parts[1], style);
                d = int.Parse(parts[2], style);
                return new PersianDate(y, m, d);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The date string must be in the form y/m/d", ex);
            }
        }
        /// <summary>
        /// Converts the specified string representation of a persian date to its equivalent PersianDate value.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result">If the conversion succeeds, this parameter will contain the PersianDate value 
        /// equivalent to the persian date specified in the first parameter, otherwise it will have the value of date 1/1/1 </param>
        /// <returns>true if the s parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string str, out PersianDate result)
        {
            try
            {
                result = Parse(str);
                return true;
            }
            catch
            {
                result = new PersianDate();
                return false;
            }
        }

        #region Comparison operators
        public static bool operator <(PersianDate x, PersianDate y)
        {
            return x.CompareTo(y) < 0;
        }
        public static bool operator <=(PersianDate x, PersianDate y)
        {
            return x.CompareTo(y) <= 0;
        }
        public static bool operator ==(PersianDate x, PersianDate y)
        {
            return x.n == y.n;
        }
        public static bool operator >=(PersianDate x, PersianDate y)
        {
            return x.CompareTo(y) >= 0;
        }
        public static bool operator >(PersianDate x, PersianDate y)
        {
            return x.CompareTo(y) > 0;
        }
        public static bool operator !=(PersianDate x, PersianDate y)
        {
            return x.n != y.n;
        }
        #endregion

        #region Arithmetic operators
        public static PersianDate operator +(PersianDate persianDate, int days)
        {
            long n = persianDate.n + days;
            try
            {
                return new PersianDate(checked((uint)n));
            }
            catch (OverflowException ex)
            {
                throw new OverflowException("The resulting date of the addition is outside of acceptable range.", ex);
            }
        }
        public static PersianDate operator -(PersianDate persianDate, int days)
        {
            return persianDate + (-days);
        }
        public static TimeSpan operator -(PersianDate x, PersianDate y)
        {
            long l = (long)x.n - y.n;
            return new TimeSpan(checked((int)l), 0, 0, 0);
        }

        #endregion
        uint n;//the only field, stores the number of days passed 1/1/1
        private PersianDate(uint n)
        {
            this.n = n;
        }
        /// <summary>
        /// Initializes a new instance of the PersianDate structure set to the year,month, and day parameters.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public PersianDate(int year, int month, int day)
        {
            if (!IsValid(year, month, day))
                throw new ArgumentException(string.Format("The date ({0}/{1}/{2}) is not a valid date.", year, month, day));
            if (comp(year, month, day, minYear, minMonth, minDay) < 0)
                throw new ArgumentOutOfRangeException(string.Format("The date ({0}/{1}/{2}) is less than the minimun acceptable date", year, month, day));
            if (comp(year, month, day, maxYear, maxMonth, maxDay) > 0)
                throw new ArgumentOutOfRangeException(string.Format("The date ({0}/{1}/{2}) is greater than the maximum acceptable date", year, month, day));
            n = days(year, month, day);
        }
        /// <summary>
        /// Initializes a new instance of the PersianDate structure set to the persian date equivalent to the date specified in the dateTime parameter.
        /// </summary>
        /// <param name="dateTime"></param>
        public PersianDate(DateTime dateTime)
        {
            n = (uint)(dateTime.Date - firstDateDateTime).Days;
        }
        /// <summary>
        /// Gets the day of the month represented by this PersianDate instance.
        /// </summary>
        public int Day
        {
            get
            {
                int y, m, d;
                yearMonthDay(n, out y, out m, out d);
                return d;
            }
        }
        /// <summary>
        /// Gets the month represented by this PersianDate instance.
        /// </summary>
        public int Month
        {
            get
            {
                int y, m, d;
                yearMonthDay(n, out y, out m, out d);
                return m;
            }
        }
        /// <summary>
        /// Gets year represented by this PersianDate instance.
        /// </summary>
        public int Year
        {
            get
            {
                int y, m, d;
                yearMonthDay(n, out y, out m, out d);
                return y;
            }
        }
        /// <summary>
        /// Gets the month as PersianMonth represented by this PersianDate instance.
        /// </summary>
        public PersianMonth MonthAsPersianMonth
        {
            get
            {
                return (PersianMonth)Month;
            }
        }
        /// <summary>
        /// Gets the day of the week represented by this PersianDate instance.
        /// </summary>
        public DayOfWeek DayOfWeek
        {
            get
            {
                return (DayOfWeek)((n - 3) % 7);
            }
        }
        /// <summary>
        /// Gets the day of the week as PersianDayOfWeek represented by this PersianDate instance.
        /// </summary>
        public PersianDayOfWeek PersianDayOfWeek
        {
            get
            {
                return (PersianDayOfWeek)((n - 3) % 7);
            }
        }
        /// <summary>
        /// Gets the day of the year represented by this PersianDate instance.
        /// </summary>
        public int DayOfYear
        {
            get
            {
                int y, ds;
                yearAndDaysInYear(n, out y, out ds);
                return ds + 1;

            }
        }

        /// <summary>
        /// returns a new PersianDate resulting from adding the days specified to the current PersianDate
        /// </summary>
        /// <param name="days">number of days to be added to the current PersianDate</param>
        public PersianDate AddDays(int days)
        {
            return (this + days);
        }

        /// <summary>
        /// Converts the PersianDate value to its DateTime equivalent.
        /// </summary>
        /// <exception cref="OverflowException">
        ///  Throws when the conversion results in an un-representable DateTime
        /// </exception>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            try
            {
                return firstDateDateTime.AddDays(n);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new OverflowException("The conversion results in an un-representable DateTime.", ex);
            }
        }
        /// <summary>
        /// Returns the String representation of the PersianDate value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", Year, Month, Day);
        }
        /// <summary>
        /// Returns the long date String representation of the PersianDate Value.
        /// </summary>
        /// <returns></returns>
        public string ToLongDateString()
        {
            return string.Format("‏" + "{3}، {2} {1} {0}", Year, MonthAsPersianMonth, Day, PersianDayOfWeek);
        }

        #region IComparable<PersianDate> Members

        public int CompareTo(PersianDate that)
        {
            return this.n.CompareTo(that.n);
        }

        #endregion


        //Test methods. 
        internal static string tst()
        {

            string r = "";
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            var z = pc.ToDateTime(1, 1, 1, 0, 0, 0, 0);
            for (uint i = 0; i <= (DateTime.MaxValue - z).Days; i++)
            {
                int y, m, d;
                yearMonthDay(i, out y, out m, out d);
                var td = pc.AddDays(z, (int)i);
                int yy = pc.GetYear(td), mm = pc.GetMonth(td), dd = pc.GetDayOfMonth(td);
                if (yy != y || dd != d || mm != m || days(yy, mm, dd) != i || !IsValid(y, m, d))
                    r += string.Format("\r\n{0}: {1}/{2}/{3},  {4}", i, y, m, d, days(y, m, d));
            }

            return r;
        }
        internal static string tst1()
        {

            StringBuilder r = new StringBuilder();
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            var z = pc.ToDateTime(1, 1, 1, 0, 0, 0, 0);
            for (uint i = 0; i <= (DateTime.MaxValue - z).Days; i++)
            {
                PersianDate pd = new PersianDate(i);
                if (pc.GetDayOfYear(pd.ToDateTime()) != pd.DayOfYear)
                    r.AppendFormat("\r\n{0}: {1}, {2}, {3}", i, pd.ToString(), pd.DayOfYear, pc.GetDayOfYear(pd.ToDateTime()));
            }

            return r.ToString();
        }
        internal static string tst2()
        {
            string r;
            int y, m, d;
            yearMonthDay(uint.MaxValue, out y, out m, out d);
            r = string.Format("{0}/{1}/{2}", y, m, d);
            PersianDate today = new PersianDate(1387, 12, 3);
            r = today.DayOfWeek.ToString() + " " + today.Day + "/" + today.Month + "/" + today.Year;
            return r;
        }
        internal static string tst3()
        {
            string r;
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            r = pc.ToDateTime(1, 1, 1, 0, 0, 0, 0).ToString();

            r = new PersianDate(1387, 12, 10).ToDateTime().ToString();
            r += "\n" + new PersianDate(1387, 12, 10).ToDateTime().ToShortDateString();
            r += "\n" + new PersianDate(1387, 12, 10).ToDateTime().ToLongDateString();
            r += "\n" + new PersianDate(new DateTime(2009, 2, 28)).ToString();
            r += "\n" + new PersianDate(new DateTime(2009, 2, 28)).ToLongDateString();
            r += "\n" + new PersianDate(new DateTime(2009, 2, 28)).MonthAsPersianMonth;
            r += "\n" + new PersianDate(1387, 12, 13).ToLongDateString();
            DateTime d = DateTime.Now;

            return r;
        }
        internal static string tst4()
        {
            string r = "";
            for (int i = 1300; i < 1400; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    r += string.Format("\r\n{0}, {1}: {2}", i, j, DaysInMonth(i, j));
                }
            }
            r = PersianDate.Today.ToLongDateString();
            PersianDate xxx = PersianDate.Today, yyy = xxx;
            var bbb = xxx = yyy;
            return r;
        }

    }
    #endregion

    #region PersianDateConverter
    public class PersianDateConverter : TypeConverter
    {
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            if (sourceType == typeof(DateTime))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value == null)
            {
                return PersianDate.Today;
            }
            if (value is string)
            {
                return PersianDate.Parse(value as string);
            }
            if (value is DateTime)
            {
                return new PersianDate((DateTime)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                PersianDate pd = (PersianDate)value;
                return value.ToString();
            }
            if (destinationType == typeof(DateTime))
            {
                PersianDate pd = (PersianDate)value;
                return pd.ToDateTime();
            }
            return base.ConvertTo(context, culture, value, destinationType);

        }
    }
    #endregion

    #region PersianDateStuff
    public enum PersianDayOfWeek
    {
        شنبه = 6,
        یکشنبه = 0,
        دوشنبه,
        ﺳﻪشنبه,
        چهارشنبه,
        پنجشنبه,
        جمعه
    }
    public enum PersianMonth
    {
        فروردین = 1,
        اردیبهشت,
        خرداد,
        تیر,
        مرداد,
        شهریور,
        مهر,
        آبان,
        آذر,
        دی,
        بهمن,
        اسفند
    }
    #endregion

    #endregion

    #region Flow Components

    #region Flow_Font_Style
    public static class Flow_Font_Style
    {
        public static FontFamily fontFamily = new FontFamily("B Nazanin");
        public static double fontSizeFlowDocument = 14;
        public static double fontSizeHeader = 19;
        public static double fontSizeFooter = 19;
        public static Brush foregroundFlowDocument = Brushes.Black;
        public static Brush foregroundHeader = Brushes.DarkBlue;
        public static FontWeight fontWeightHeader = FontWeights.Bold;
    }
    #endregion

    #region FlowDocument
    public class Flow_FlowDocumentBase : FlowDocument
    {
        protected PrintDialog printDialog = new PrintDialog();
        public Flow_FlowDocumentBase()
        {
            printDialog = new PrintDialog();
            FontFamily = Flow_Font_Style.fontFamily;
            FontSize = Flow_Font_Style.fontSizeFlowDocument;
            ColumnGap = 0;
        }
    }
    #endregion

    #region Flow_FlowDocument
    public class Flow_FlowDocument : Flow_FlowDocumentBase
    {
        public Flow_FlowDocument()
        {
            //A4 is 210 x 297mm.
            //1cm==(96/2.54) px
            //  ColumnWidth = 793.70;
            this.PageHeight = printDialog.PrintableAreaHeight;
            this.PageWidth = printDialog.PrintableAreaWidth;
            this.ColumnWidth = printDialog.PrintableAreaWidth;

        }
    }
    #endregion

    #region Flow_FlowDocumentLandscape
    public class Flow_FlowDocument_Landscape : Flow_FlowDocumentBase
    {
        public Flow_FlowDocument_Landscape()
        {
            this.PageHeight = printDialog.PrintableAreaWidth;
            this.PageWidth = printDialog.PrintableAreaHeight;
            this.ColumnWidth = printDialog.PrintableAreaHeight;
        }
    }
    #endregion

    #region Flow_Section
    public class Flow_Section : Section
    {
        public Flow_Section()
        {
            LineHeight = 1;
        }
    }
    #endregion

    #region Flow_Section_Header
    public class Flow_Section_Header : Flow_Section
    {
        public Flow_Section_Header()
        {
            Foreground = Flow_Font_Style.foregroundHeader;
            FontWeight = Flow_Font_Style.fontWeightHeader;
            FontSize = Flow_Font_Style.fontSizeHeader;
            TextAlignment = TextAlignment.Center;
            LineHeight = 2;
        }
    }
    #endregion

    #region Flow_Section_Footer
    public class Flow_Section_Footer : Flow_Section
    {
        public Flow_Section_Footer()
        {
            this.TextAlignment = TextAlignment.Center;
            this.FontWeight = FontWeights.Bold;
            this.FontSize = Flow_Font_Style.fontSizeFooter;
        }
    }
    #endregion

    #region  Flow_Paragraph_Base
    public class Flow_Paragraph_Base : Paragraph
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Flow_Paragraph_Base), new PropertyMetadata(String.Empty));
        public string Text
        {
            get { return this.GetValue(TextProperty) as string; }
            set
            {
                this.SetValue(TextProperty, value);
                run.Text = Text;
            }
        }
        protected Run run = new Run();
    }
    #endregion

    #region Flow_Paragraph
    public class Flow_Paragraph : Flow_Paragraph_Base
    {
        public Flow_Paragraph()
        {
            this.Inlines.Add(run);
        }

    }
    #endregion

    #region Flow_Paragraph_Underline
    public class Flow_Paragraph_Underline : Flow_Paragraph_Base
    {
        private Underline underLine;
        public Flow_Paragraph_Underline()
        {
            underLine = new Underline();
            underLine.Inlines.Add(run);
            this.Inlines.Add(underLine);
        }
    }


    #endregion

    #region Flow_Paragraph_Bold
    public class Flow_Paragraph_Bold : Flow_Paragraph_Base
    {
        private Bold bold;
        public Flow_Paragraph_Bold()
        {
            bold = new Bold();
            bold.Inlines.Add(run);
            this.Inlines.Add(bold);
        }
    }
    #endregion

    #region Table Cell
    public class Flow_Table_Cell_Base : TableCell
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Flow_Table_Cell_Base), new PropertyMetadata(String.Empty));
        public string Text
        {
            get { return this.GetValue(TextProperty) as string; }
            set
            {
                this.SetValue(TextProperty, value);
                run.Text = Text;
            }
        }
        protected Run run = new Run();
        protected Paragraph paragraph = new Paragraph();
        protected Thickness buttomThickness = new Thickness(0, 0, 0, 0.5);
        protected Thickness topBottomThickness = new Thickness(0, 0.5, 0, 0.5);
        protected Brush borderBrush;

        public Flow_Table_Cell_Base()
        {
            paragraph.Inlines.Add(run);
            this.Blocks.Add(paragraph);
            borderBrush = Brushes.Black;
        }
    }

    public class Flow_Table_Cell : Flow_Table_Cell_Base
    {
        public Flow_Table_Cell()
        {
        }
    }

    public class Flow_Table_Cell_Center : Flow_Table_Cell
    {
        public Flow_Table_Cell_Center()
        {
            this.TextAlignment = TextAlignment.Center;
        }
    }

    public class Flow_Table_Cell_Bold : Flow_Table_Cell
    {
        public Flow_Table_Cell_Bold()
        {
            paragraph.FontWeight = FontWeights.Bold;
        }
    }

    public class Flow_Table_Cell_Bold_Center : Flow_Table_Cell_Bold
    {
        public Flow_Table_Cell_Bold_Center()
        {
            this.TextAlignment = TextAlignment.Center;
        }
    }

    public class Flow_Table_Cell_With_Buttom_Border : Flow_Table_Cell
    {
        public Flow_Table_Cell_With_Buttom_Border()
        {
            this.BorderBrush = borderBrush;
            this.BorderThickness = buttomThickness;
        }
    }

    public class Flow_Table_Cell_With_Top_Bottom_Border : Flow_Table_Cell
    {
        public Flow_Table_Cell_With_Top_Bottom_Border()
        {
            this.BorderBrush = borderBrush;
            this.BorderThickness = topBottomThickness;
        }
    }

    public class Flow_Table_Cell_Center_With_Buttom_Border : Flow_Table_Cell_With_Buttom_Border
    {
        public Flow_Table_Cell_Center_With_Buttom_Border()
        {
            this.TextAlignment = TextAlignment.Center;
        }
    }

    public class Flow_Table_Cell_Center_With_Top_Bottom_Border : Flow_Table_Cell_With_Top_Bottom_Border
    {
        public Flow_Table_Cell_Center_With_Top_Bottom_Border()
        {
            this.TextAlignment = TextAlignment.Center;
        }
    }

    public class Flow_Table_Cell_Center_With_2_Buttom_Border : Flow_Table_Cell_With_Buttom_Border
    {
        public Flow_Table_Cell_Center_With_2_Buttom_Border()
        {
            this.TextAlignment = TextAlignment.Center;
            this.Blocks.Add(new BlockUIContainer((new Border() { BorderThickness = buttomThickness, BorderBrush = borderBrush, Margin = new Thickness(0, 0, 0, 2) })));
        }
    }

    public class Flow_Table_Cell_Header : Flow_Table_Cell_Center_With_Buttom_Border
    {
        public Flow_Table_Cell_Header()
        {
            paragraph.FontWeight = FontWeights.Bold;
        }
    }
    #endregion

    #region Flow_Table_Base
    public class Flow_Table_Base : Table
    {
        public Flow_Table_Base()
        {
            this.CellSpacing = 1;
        }
    }
    #endregion

    #region Flow_Table
    public class Flow_Table : Flow_Table_Base
    {
    }

    #endregion

    #region TableRowBase
    public class Flow_Table_Row_Base : TableRow
    {
        public Flow_Table_Row_Base()
        {
        }
    }
    #endregion

    #region Flow_Table_Row
    public class Flow_Table_Row : Flow_Table_Row_Base
    {
        public Flow_Table_Row()
        {
        }
    }
    #endregion

    #region Flow_Table_Row_Bold
    public class Flow_Table_Row_Bold : Flow_Table_Row
    {
        public Flow_Table_Row_Bold()
        {
            this.FontWeight = FontWeights.Black;
        }
    }
    #endregion

    #region Flow_Table_Row_Bold_1_Size_Biger
    public class Flow_Table_Row_Bold_1_Size_Biger : Flow_Table_Row_Bold
    {
        public Flow_Table_Row_Bold_1_Size_Biger()
        {
            this.FontSize = Flow_Font_Style.fontSizeFlowDocument + 1;
        }
    }
    #endregion

    #region Flow_AbstractLabel
    public abstract class Flow_BaseLabel : Label
    {
        public Flow_BaseLabel()
        {
            BorderThickness = new Thickness(0);
            Padding = new Thickness(0);
            VerticalContentAlignment = VerticalAlignment.Bottom;
            HorizontalContentAlignment = HorizontalAlignment.Center;
        }
    }
    #endregion

    #region Flow_HeaderLabel
    public class Flow_HeaderLabel : Flow_BaseLabel
    {
        public Flow_HeaderLabel()
        {
            Foreground = Brushes.DarkBlue;
            FontSize = 20;
            Height = 25;
            FontWeight = FontWeights.Bold;
        }
    }
    #endregion

    #region Flow_TitleLabel
    public class Flow_TitleLabel : Flow_BaseLabel
    {
        public Flow_TitleLabel()
        {
            FontSize = 18;
            FontWeight = FontWeights.Bold;
        }
    }
    #endregion

    #region Flow_AccountLabel
    public class Flow_AccountLabel : Flow_BaseLabel
    {
        public Flow_AccountLabel()
        {
            FontSize = 17;
        }
    }
    #endregion

    #region Flow_AccountLabel Bold
    public class Flow_AccountLabelBold : Flow_AccountLabel
    {
        public Flow_AccountLabelBold()
        {
            FontWeight = FontWeights.Bold;
        }
    }
    #endregion

    #region Flow_TextLabel
    public class Flow_TextLabel : Flow_BaseLabel
    {
        public Flow_TextLabel()
        {
            FontSize = 16;
        }
    }
    #endregion

    #region Flow_TextLabelBold
    public class Flow_TextLabelBold : Flow_TextLabel
    {
        public Flow_TextLabelBold()
        {
            FontWeight = FontWeights.Bold;
        }
    }
    #endregion

    #region Flow_SumLabel
    public class Flow_SumLabel : Flow_BaseLabel
    {
        public Flow_SumLabel()
        {
            Content = "======================";
            FontSize = 22;
            VerticalContentAlignment = VerticalAlignment.Top;
            Margin = new Thickness(0, -15, 0, 0);
        }
    }
    #endregion

    #region Flow_SectionHeader
    public class Flow_SectionCenter : Section
    {
        public Flow_SectionCenter()
        {
            TextAlignment = TextAlignment.Center;
            BreakPageBefore = true;
            LineHeight = 15;
            Foreground = Brushes.DarkBlue;
            FontWeight = FontWeights.Bold;
        }
    }
    #endregion

    #region Flow_ParagraphBase
    public abstract class Flow_ParagraphBase : Paragraph
    {
        public static int TabSize = 40;
        public Flow_ParagraphBase()
        {
        }
    }
    #endregion

    #region Flow_ParagraphCenter
    public class Flow_ParagraphCenter : Flow_ParagraphBase
    {
        public Flow_ParagraphCenter()
        {
            TextAlignment = TextAlignment.Center;
        }
    }
    #endregion

    #region Flow_ParagraphLeft
    public class Flow_ParagraphLeft : Flow_ParagraphBase
    {
        public Flow_ParagraphLeft()
        {
            TextAlignment = TextAlignment.Right;
        }
    }
    #endregion

    #region Flow_Paragraph1Tab
    public class Flow_Paragraph1Tab : Flow_ParagraphBase
    {
        public Flow_Paragraph1Tab()
        {
            Margin = new Thickness(Flow_ParagraphBase.TabSize, 0, 0, 0);
        }
    }
    #endregion

    #region Flow_Paragraph2Tab
    public class Flow_Paragraph2Tab : Flow_ParagraphBase
    {
        public Flow_Paragraph2Tab()
        {
            Margin = new Thickness(Flow_ParagraphBase.TabSize * 2, 0, 0, 0);
        }
    }
    #endregion

    #region Flow_Container
    public class Flow_Container : InlineUIContainer
    {
        public Flow_Container()
        {
            BaselineAlignment = BaselineAlignment.Center;
        }
    }
    #endregion

    #region Flow_TableCellHeader
    public class Flow_TableCellHeader : TableCell
    {
        public Flow_TableCellHeader()
        {
            BorderThickness = new Thickness(0, 0, 0, 1);
            BorderBrush = Brushes.Black;
        }
    }
    #endregion

    #region Flow_TableCellFooter
    public class Flow_TableCellFooter : TableCell
    {
        public Flow_TableCellFooter()
        {
            BorderThickness = new Thickness(0, 0.5, 0, 0);
            BorderBrush = Brushes.Black;
        }
    }
    #endregion

    #region Flow_TableCellBorder
    public class Flow_TableCellBorder : TableCell
    {
        public Flow_TableCellBorder()
        {
            BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5);
            BorderBrush = Brushes.Black;
        }
    }
    #endregion

    #region Flow_Table1Tab
    public class Flow_Table1Tab : Flow_Table
    {
        public Flow_Table1Tab()
        {
            Margin = new Thickness(Flow_ParagraphBase.TabSize, 0, 0, 0);
        }
    }
    #endregion

    #region Flow_Table2Tab
    public class Flow_Table2Tab : Flow_Table
    {
        public Flow_Table2Tab()
        {
            Margin = new Thickness(Flow_ParagraphBase.TabSize * 2, 0, 0, 0);
        }
    }
    #endregion

    #endregion

    #endregion
}