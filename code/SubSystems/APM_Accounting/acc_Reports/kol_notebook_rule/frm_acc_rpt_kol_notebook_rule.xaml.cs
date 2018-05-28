using System.Windows;
using APMTools;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
using System;

namespace APM_Accounting
{
    public partial class frm_acc_rpt_kol_notebook_rule : WindowReport<stp_acc_rpt_kol_notebook_rule_selResult>
    {
        #region Constructor
        public frm_acc_rpt_kol_notebook_rule()
        {
            InitializeComponent();
            Initial_WindowReport(dh_balance, dbg_acc_rpt_account_balance, tbr_acc_rpt_account_balance, "acc_rpt_kol_notebook_rule", new APM_SubSystems.APM_Accounting.acc_Reports.kol_notebook_rule.rpt_kol_notebook_rule());
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<stp_acc_document_type_selResult>().FillComboBoxForShow(dh_balance.cmbAccountingDocumentType);
            MoveCollectionView();
        }
        public override void SearchClick()
        {
            base.SearchClick();
            double remaining = 0;
            string accountCode="";
            foreach (var record in allRecords)
            {
                if(record.acc_rpt_kol_notebook_rule_chart_account_code!=accountCode)
                    remaining=0;
                remaining +=
                    record.acc_rpt_kol_notebook_rule_sum_credit -
                    record.acc_rpt_kol_notebook_rule_sum_debt;
                record.acc_rpt_kol_notebook_rule_remaining = Math.Abs(remaining);
                record.acc_rpt_kol_notebook_rule_specification = (remaining > 0) ? "بس" : "بد";
                accountCode=record.acc_rpt_kol_notebook_rule_chart_account_code;
            }
            SumRecord.sumRecord.acc_rpt_kol_notebook_rule_remaining =
                Math.Abs(SumRecord.sumRecord.acc_rpt_kol_notebook_rule_sum_credit -
                SumRecord.sumRecord.acc_rpt_kol_notebook_rule_sum_debt);

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
            if (!(dataGrid.SelectedItem is stp_acc_rpt_kol_notebook_rule_selResult))
                return;
            var record = dataGrid.SelectedItem as stp_acc_rpt_kol_notebook_rule_selResult;
            new frm_acc_document().ShowOneDocument(record.acc_rpt_kol_notebook_rule_acc_document_id);
        }
        #endregion

    }
}
