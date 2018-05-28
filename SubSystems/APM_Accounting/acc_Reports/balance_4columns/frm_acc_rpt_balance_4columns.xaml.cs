using System.Windows;
using APMTools;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
using System;

namespace APM_Accounting
{
    public partial class frm_acc_rpt_balance_4columns : WindowReport<stp_acc_rpt_balance_4columns_selResult>
    {
        #region Constructor
        public frm_acc_rpt_balance_4columns()
        {
            InitializeComponent();
            Initial_WindowReport(dh_balance, dbg_acc_rpt_account_balance, tbr_acc_rpt_account_balance, "acc_rpt_balance_4columns", new APM_SubSystems.APM_Accounting.acc_Reports.balance_4columns.rpt_balance_4columns());
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<stp_acc_document_type_selResult>().FillComboBoxForShow(dh_balance.cmbAccountingDocumentType);
            selectedRecord.acc_rpt_balance_4columns_acc_document_type_id = null;
            MoveCollectionView();
        }
        public override void SearchClick()
        {
            base.SearchClick();
            foreach (var record in allRecords)
            {
                double remaining = record.acc_rpt_balance_4columns_sum_credit - record.acc_rpt_balance_4columns_sum_debt;
                record.acc_rpt_balance_4columns_remaining_credit = Math.Max(remaining, 0);
                record.acc_rpt_balance_4columns_remaining_debt =Math.Abs(Math.Min(remaining, 0));
            }
        }
        #endregion

        #region Events
        private void XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), "ثبت کننده", typeof(APM_SubSystems.frm_User));
        }
        private void XBrowseClick_ConfirmerUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_personel_selResult>(), "شخص تائید کننده", typeof(APM_SubSystems.frm_Personel));
        }
        private void XBrowseClick_SelectAccount(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectTree<stp_acc_chart_account_treResult>(TreeType.MultiSelect_LastNode, "حساب"), "حساب", typeof(frm_acc_chart_account));
        }
        private void XBrowseClick_SelectDetail(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "تفصیل", typeof(frm_acc_detail));
        }
        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_chart_account().ShowDialog();
        }
        #endregion

    }
}
