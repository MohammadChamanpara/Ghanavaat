using System.Windows;
using System.Windows.Controls;
using UserInterfaceLayer;
using DataAccessLayer;
using APMTools;
using BusinessLogicLayer;
using System.Linq;
using System;
using CrystalDecisions.CrystalReports.Engine;
using APM_SubSystems.Sahaam.gnt_creditor;

namespace APM_SubSystems
{
    public partial class frm_gnt_ownership : WindowBase<stp_gnt_ownership_selResult>
    {
        #region Variables
        stp_gnt_creditor_selResult CurrentCreditor { get; set; }
        public ReportClass printReportFile;
        #endregion
        
        #region Constructor
        public frm_gnt_ownership(stp_gnt_creditor_selResult currentCreditor)
        {
            InitializeComponent();
            this.CurrentCreditor = currentCreditor;
            this.printReportFile = new gnt_rpt_creditor_ownership();
            Initial_WindowBase(dbgOwnership, windowToolbar, grpInfo, "gnt_ownership", true, null);
        }
        #endregion

        #region Events
        private void txt_gnt_ownership_second_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            long creditorId = selectedRecord.gnt_ownership_gnt_creditor_id;
            if (e.Key != System.Windows.Input.Key.Enter)
                return;
            if (SaveClick(true) == SaveResult.Saved)
                InsertClick();
            selectedRecord.gnt_ownership_gnt_creditor_id = creditorId;
            MoveCollectionView();
            brw_gnt_ownership_gnt_water_id.Focusable = true;
            brw_gnt_ownership_gnt_water_id.Focus();
        }
        private void brw_gnt_ownership_earth_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_gnt_earth_selResult>(), "زمین", typeof(frm_gnt_earth), sender);
        }
        private void brw_gnt_ownership_gnt_water_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_gnt_water_selResult>(), "آب", typeof(frm_gnt_water), sender);
            txt_gnt_ownership_jeribMinuteSecond_LostFocus(null, null);
        }
        private void brw_gnt_ownership_gnt_creditor_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), "سهامدار", typeof(frm_gnt_creditor), sender);
        }

        private void txt_gnt_ownership_jeribMinuteSecond_LostFocus(object sender, RoutedEventArgs e)
        {
            var ownership = new tbl_gnt_ownership();
            ownership.InitialFromRecord(selectedRecord);
            selectedRecord.gnt_ownership_jerib = ownership.gnt_ownership_correct_jerib;
            selectedRecord.gnt_ownership_minute = ownership.gnt_ownership_correct_minute;
            selectedRecord.gnt_ownership_second = ownership.gnt_ownership_correct_second;
            selectedRecord.gnt_ownership_earth = ownership.gnt_ownership_correct_earth;
            selectedRecord.gnt_ownership_credit = ownership.gnt_ownership_correct_credit;
            MoveCollectionView();
        }
        #endregion

        #region Override
        public override void PrintClick()
        {
            try
            {
                if (printReportFile == null)
                    return;
                var printForm = new WindowPrint<tbl_gnt_creditor, stp_gnt_ownership_selResult>(printReportFile);
                printForm.articleList = allRecords;
                printForm.selectedRecord = this.CurrentCreditor.ToEntity();
                printForm.ShowDialog();
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.RecordParameter.gnt_ownership_gnt_creditor_id = this.CurrentCreditor.gnt_creditor_id;
            base.Window_Loaded(sender, e);
            lbl_creditor_name.Content = this.CurrentCreditor.gnt_creditor_name;
            var dataBase = DDB.NewContext();
            if (dataBase.tbl_gnt_settings.Count() == 0)
            {
                Messages.ErrorMessage("تنظیمات سیستم سهام وارد نشده اند");
                return;
            }
        }
        public override bool ValidationForSave()
        {
            MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
            selectedRecord.gnt_ownership_gnt_creditor_id = this.CurrentCreditor.gnt_creditor_id;
            return base.ValidationForSave();
        }
        #endregion
    }
}
