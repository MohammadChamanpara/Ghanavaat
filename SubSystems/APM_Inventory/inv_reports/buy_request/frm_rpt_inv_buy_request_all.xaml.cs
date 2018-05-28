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

namespace APM_SubSystems
{
    public partial class frm_rpt_inv_buy_request_all : WindowReport<stp_inv_rpt_buy_request_all_selResult>
    {

        #region Initialize
        public frm_rpt_inv_buy_request_all()
        {
            InitializeComponent();
            Initial_WindowReport(dh_buyRequest, dbg_filter_results, tbr_filter, "inv_rpt_buy_request_article", new APM_SubSystems.APM_Inventory.inv_reports.buy_request.rpt_inv_buy_request_all());   
        }
        #endregion

        #region Events

        #region Browse Clicks
        private void brw_inv_rpt_buy_request_all_inv_store_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender,new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store));
        }

        private void brw_inv_rpt_buy_request_all_inv_group_goods_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender,new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.MultiSelect_LastNode, "کالا و گروه کالا"), "کالا", typeof(frm_group_goods));
        }

        private void APMDocumentHeader_XBrowseClick_RequestConfirmerPersonel(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender,new WindowSelectGrid<stp_glb_personel_selResult>(), "شخص تائید کننده", typeof(frm_Personel));
        }

        private void APMDocumentHeader_XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), "ثبت کننده", typeof(frm_User));
        }
        #endregion

        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var currentRecord = dataGrid.CurrentItem as stp_inv_rpt_buy_request_all_selResult;
        new frm_inv_buy_request().ShowOneDocument(currentRecord.inv_rpt_buy_request_all_inv_document_id,currentRecord.inv_rpt_buy_request_all_inv_article_id);
        }

        #endregion
       
    }
}
