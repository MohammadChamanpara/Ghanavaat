using System.Windows;
using DataAccessLayer;
using APMTools;
using UserInterfaceLayer;
using APM_SubSystems;
using APM_Accounting;

namespace APM_SubSystems
{
    public partial class frm_inv_rpt_goods_receive_all : WindowReport<stp_inv_rpt_goods_receive_all_selResult>
    {

        #region Initialize
        public frm_inv_rpt_goods_receive_all()
        {
            InitializeComponent();
            Initial_WindowReport(dh_receive, grd_filter, tbr_filter, "inv_rpt_goods_receive_all", new APM_SubSystems.APM_Inventory.inv_reports.goods_receive.rpt_inv_goods_receive_all());
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
            var currentRecord = dataGrid.CurrentItem as stp_inv_rpt_goods_receive_all_selResult;
            new frm_inv_rpt_goods_cardex().CustomReport(
                  new stp_inv_rpt_goods_cardex_selResult()
                  {
                      inv_rpt_goods_cardex_inv_group_goods_id = currentRecord.inv_rpt_goods_receive_all_inv_goods_id,
                      inv_rpt_goods_cardex_inv_group_goods_name = currentRecord.inv_rpt_goods_receive_all_inv_group_goods_code,
                      inv_rpt_goods_cardex_inv_group_goods_code = currentRecord.inv_rpt_goods_receive_all_inv_group_goods_name
                  });
        }
        #endregion
    }
}
