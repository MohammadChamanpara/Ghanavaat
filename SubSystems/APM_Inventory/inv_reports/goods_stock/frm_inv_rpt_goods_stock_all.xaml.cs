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
using DataAccessLayer;
using System.Data;
using APMTools;
using System.ComponentModel;
using CrystalDecisions.CrystalReports.Engine;
using UserInterfaceLayer;
using APM_SubSystems;
using APM_Accounting;

namespace APM_SubSystems
{
    public partial class frm_inv_rpt_goods_stock_all : WindowReport<stp_inv_rpt_goods_stock_all_selResult>
    {
        #region Initialize
        public frm_inv_rpt_goods_stock_all()
        {
            InitializeComponent();
            Initial_WindowReport(dh_stock, grd_filter, tbr_filter, "inv_rpt_goods_stock_all_sel", new APM_SubSystems.APM_Inventory.inv_reports.goods_stock.rpt_inv_goods_stock_all());
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
        private void APMDocumentHeader_XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), " ثبت کننده", typeof(frm_User));
        }
        private void APMDocumentHeader_XBrowseClick_DestinationDetail(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "طرف سند", typeof(frm_acc_detail));
        }
        #endregion
    }
}
