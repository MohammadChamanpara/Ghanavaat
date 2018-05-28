using System.Windows;
using APMTools;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;

namespace APM_Accounting
{
    public partial class frm_acc_rpt_daily_notebooks : WindowReport<stp_acc_rpt_daily_notebooks_selResult>
    {
        #region Constructor
        public frm_acc_rpt_daily_notebooks()
        {
            InitializeComponent();
            Initial_WindowReport(dh_balance, dbg_acc_rpt_account_balance, tbr_acc_rpt_account_balance, "acc_rpt_daily_notebooks", new APM_SubSystems.APM_Accounting.acc_Reports.daily_notebooks.rpt_acc_daily_notebooks());
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<stp_acc_document_type_selResult>().FillComboBoxForShow(dh_balance.cmbAccountingDocumentType);
            selectedRecord.acc_rpt_daily_notebooks_acc_document_type_id = null;
            MoveCollectionView();
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
            BrowseClick_Report(sender,new WindowSelectTree<stp_acc_chart_account_treResult>(TreeType.MultiSelect_LastNode, "حساب"), "حساب", typeof(frm_acc_chart_account));
        }
        private void XBrowseClick_SelectDetail(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender,new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "تفصیل", typeof(frm_acc_detail));
        }
        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(dataGrid.SelectedItem is stp_acc_rpt_daily_notebooks_selResult))
                return;
            var record = dataGrid.SelectedItem as stp_acc_rpt_daily_notebooks_selResult;
            new frm_acc_document().ShowOneDocument(record.acc_rpt_daily_notebooks_acc_document_id, record.acc_rpt_daily_notebooks_article_id);
        }
        #endregion 

    }
}
