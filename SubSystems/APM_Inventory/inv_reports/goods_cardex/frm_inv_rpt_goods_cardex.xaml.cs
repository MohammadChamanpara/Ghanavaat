using System.Windows;
using DataAccessLayer;
using System.Data;
using APMTools;
using UserInterfaceLayer;
using BusinessLogicLayer;
using APM_SubSystems;
using APM_Accounting;

namespace APM_SubSystems
{
    public partial class frm_inv_rpt_goods_cardex : WindowReport<stp_inv_rpt_goods_cardex_selResult>
    {
        #region Initialize
        public frm_inv_rpt_goods_cardex()
        {
            InitializeComponent();
            Initial_WindowReport(dh_cardex, userDataGrid, tbr_filter, "inv_rpt_goods_cardex", new APM_SubSystems.APM_Inventory.inv_reports.goods_cardex.rpt_inv_goods_cardex());
        }
        #endregion

        #region override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            if (!dh_cardex.cmbCardexGoodsSelectMeasure.HasItems)
                FillMeasureComboBox();
        }
        #endregion

        #region BrowseClicks
        private void goods_Browser_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_Entity, "کالا و گروه کالا"), "کالا", typeof(frm_group_goods), sender);
            FillMeasureComboBox();
        }
        private void store_Browser_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender,new WindowSelectGrid<stp_inv_store_selResult>(), "انبار",typeof(frm_inv_store));
        }
        private void Destination_Detail_Browser_Click(object sender, RoutedEventArgs e)
        {
             BrowseClick_Report(sender,new WindowSelectGridGroup<stp_acc_detail_selResult,stp_glb_entity_type_selResult>(), "طرف سند", typeof(frm_acc_detail));
        }
        private void dh_cardex_XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), "ثبت کننده", typeof(frm_User));
        }
        #endregion

        #region Tools
        private void FillMeasureComboBox()
        {
            new BLL<stp_glb_measure_selResult>().FillComboBox(dh_cardex.cmbCardexGoodsSelectMeasure, bindingList,
                new stp_glb_measure_selResult()
                {
                    glb_measure_inv_group_goods_id = selectedRecord.inv_rpt_goods_cardex_inv_group_goods_id
                });
        }

        #endregion

        #region Events
        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(dataGrid.CurrentItem is stp_inv_rpt_goods_cardex_selResult))
                return;
            var currentRecord = dataGrid.CurrentItem as stp_inv_rpt_goods_cardex_selResult;
            switch (currentRecord.inv_rpt_goods_cardex_1opening_2receive_3send)
            {

                case 1:
                    new frm_inv_goods_receive(true, true).ShowOneDocument(currentRecord.inv_rpt_goods_cardex_inv_document_id,currentRecord.inv_rpt_goods_cardex_inv_article_id);
                    break;
                case 2:
                    new frm_inv_goods_receive(true, false).ShowOneDocument(currentRecord.inv_rpt_goods_cardex_inv_document_id, currentRecord.inv_rpt_goods_cardex_inv_article_id);
                    break;
                case 3:
                    new frm_inv_goods_send(true, false).ShowOneDocument(currentRecord.inv_rpt_goods_cardex_inv_document_id, currentRecord.inv_rpt_goods_cardex_inv_article_id);
                    break;
            }
        }
        #endregion

        
    }
}
