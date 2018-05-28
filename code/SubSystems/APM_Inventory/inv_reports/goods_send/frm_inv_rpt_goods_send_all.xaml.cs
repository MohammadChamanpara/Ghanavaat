using System.Windows;
using DataAccessLayer;
using APMTools;
using UserInterfaceLayer;
using APM_SubSystems;
using APM_Accounting;

namespace APM_SubSystems
{
    public partial class frm_inv_rpt_goods_send_all : WindowReport<stp_inv_rpt_goods_send_all_selResult>
    {
        #region Variable
        stp_inv_store_selResult filterStore = new stp_inv_store_selResult();
        #endregion

        #region Initialize
        public frm_inv_rpt_goods_send_all()
        {
            InitializeComponent();
            Initial_WindowReport(dh_send, grd_filter, tbr_filter, "inv_rpt_goods_send_all", new APM_SubSystems.APM_Inventory.inv_reports.goods_send.rpt_inv_goods_send_all());
        }
        #endregion

        #region BrowseClicks
        private void store_Browser_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store));
            
        }

        private void goods_Browser_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.MultiSelect_LastNode, "کالا و گروه کالا"), "کالا", typeof(frm_group_goods));
        }

        private void dh_send_XBrowseClick_DestinationCompany(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "طرف سند", typeof(frm_acc_detail));
        }
        private void dh_send_XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), "ثبت کننده", typeof(frm_User));
        }

        #endregion
       
    }
}
