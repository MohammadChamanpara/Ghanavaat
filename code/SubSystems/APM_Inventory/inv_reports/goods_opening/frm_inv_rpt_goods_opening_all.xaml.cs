using System.Windows;
using DataAccessLayer;
using APMTools;
using UserInterfaceLayer;
using APM_SubSystems;
using APM_Accounting;

namespace APM_SubSystems
{
    public partial class frm_inv_rpt_goods_opening_all : WindowReport<stp_inv_rpt_goods_opening_all_selResult>
    {
        #region Variables
        stp_inv_store_selResult filterStore = new stp_inv_store_selResult();
        #endregion 

        #region Initialize
        public frm_inv_rpt_goods_opening_all()
        {
            InitializeComponent();
            Initial_WindowReport(dh_receive, grd_filter, tbr_filter, "inv_rpt_goods_opening_all", new APM_SubSystems.APM_Inventory.inv_reports.goods_opening.rpt_inv_goods_opening_all());
        }
        #endregion

        #region BrowseClicks
        private void store_Browser_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender,new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store));
        }

        private void goods_Browser_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender,new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.MultiSelect_LastNode, "کالا و گروه کالا"), "کالا", typeof(frm_group_goods));
        }

        private void APMDocumentHeader_XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender,new WindowSelectGrid<stp_glb_user_selResult>(), "ثبت کننده", typeof(frm_User));
        }
        private void APMDocumentHeader_XBrowseClick_DestinationDetail(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "طرف سند", typeof(frm_acc_detail));
        }
        #endregion

        #region Events
        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var currentRecord = dataGrid.CurrentItem as stp_inv_rpt_goods_opening_all_selResult;
            new frm_inv_goods_receive(true, true).ShowOneDocument(currentRecord.inv_rpt_goods_opening_all_inv_document_id,currentRecord.inv_rpt_goods_opening_all_inv_article_id);
        }
        #endregion

    }
}
