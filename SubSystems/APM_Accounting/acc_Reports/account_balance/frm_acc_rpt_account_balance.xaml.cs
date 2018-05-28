using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using APMTools;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;

namespace APM_Accounting
{
    public partial class frm_acc_rpt_account_balance : WindowReport<stp_acc_rpt_account_balance_selResult>
    {
        #region Constructor
        public frm_acc_rpt_account_balance()
        {
            InitializeComponent();
            Initial_WindowReport(dh_balance, dbg_acc_rpt_account_balance, tbr_acc_rpt_account_balance, "acc_rpt_account_balance", new APM_SubSystems.APM_Accounting.acc_Reports.account_balance.rpt_acc_account_balance());
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<stp_acc_document_type_selResult>().FillComboBoxForShow(dh_balance.cmbAccountingDocumentType);
            selectedRecord.acc_rpt_account_balance_acc_document_type_id = 0;
            MoveCollectionView();
        }
        #endregion

        #region events
        private void XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), "ثبت کننده", typeof(APM_SubSystems.frm_User));
        }
        private void XBrowseClick_ConfirmerUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), "شخص تائید کننده", typeof(APM_SubSystems.frm_User));
        }
        private void XBrowseClick_SelectAccount(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectTree<stp_acc_chart_account_treResult>(TreeType.SingleSelect_LastEntity, "حساب"), "حساب", typeof(frm_acc_chart_account), sender);
        }
        private void XBrowseClick_SelectDetail(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "تفصیل", typeof(frm_acc_detail), sender);
        }
        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(dataGrid.SelectedItem is stp_acc_rpt_account_balance_selResult))
                return;
            var record = dataGrid.SelectedItem as stp_acc_rpt_account_balance_selResult;
            new frm_acc_document().ShowOneDocument(record.acc_rpt_account_balance_acc_document_id, record.acc_rpt_account_balance_acc_document_article_id);
        }
        public override void SearchClick()
        {
            base.SearchClick();
            double totalRemaining=0;
            for (int i = 0; i < allRecords.Count - 1;i++ )
            {
                var record = allRecords[i];
                totalRemaining += record.acc_rpt_account_balance_article_debt - record.acc_rpt_account_balance_article_credit;
                record.acc_rpt_account_balance_remaining = Math.Abs(totalRemaining);
                record.acc_rpt_account_balance_specification = (totalRemaining > 0) ? "بدهکار" : ((totalRemaining==0)?"تراز": "بستانکار");
            }
        }
        #endregion
    }
}
