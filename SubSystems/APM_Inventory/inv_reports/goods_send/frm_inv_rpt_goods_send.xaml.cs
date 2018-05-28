using System.Windows;
using DataAccessLayer;
using APMTools;
using UserInterfaceLayer;
using APM_SubSystems;
using APM_Accounting;

namespace APM_SubSystems
{
   
    public partial class frm_inv_rpt_goods_send : WindowReport<stp_inv_rpt_goods_send_selResult>
    {
        #region Initial
        public frm_inv_rpt_goods_send()
        {
            InitializeComponent();
            Initial_WindowReport(dh_filter, grd_filter, tbr_filter, "inv_rpt_goods_send", new APM_SubSystems.APM_Inventory.inv_reports.goods_send.rpt_inv_goods_send());
        }
        #endregion

        #region BrowseClicks
        private void dh_filter_XBrowseClick_MainStore(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store));
        }

        private void dh_filter_XBrowseClick_SelectGoods(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.MultiSelect_LastNode, "کالا و گروه کالا"), "کالا", typeof(frm_group_goods));
        }

        private void dh_filter_XBrowseClick_DestinationDetail(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "طرف سند", typeof(frm_acc_detail));
        }

        private void dh_filter_XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), "ثبت کننده", typeof(frm_User));
        }
        #endregion

        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var currentRecord = dataGrid.CurrentItem as stp_inv_rpt_goods_send_selResult;
            new frm_inv_goods_send(true, false).ShowOneDocument(currentRecord.inv_rpt_goods_send_inv_document_id,currentRecord.inv_rpt_goods_send_inv_article_id);
        }

        private void APMMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var currentRecord = dataGrid.CurrentItem as stp_inv_rpt_goods_send_selResult;
            new frm_inv_rpt_goods_cardex().CustomReport(
                  new stp_inv_rpt_goods_cardex_selResult()
                  {
                      inv_rpt_goods_cardex_inv_group_goods_id = currentRecord.inv_rpt_goods_send_inv_goods_id,
                      inv_rpt_goods_cardex_inv_group_goods_name = currentRecord.inv_rpt_goods_send_inv_group_goods_code,
                      inv_rpt_goods_cardex_inv_group_goods_code = currentRecord.inv_rpt_goods_send_inv_group_goods_name
                  });
        }
    }
}
