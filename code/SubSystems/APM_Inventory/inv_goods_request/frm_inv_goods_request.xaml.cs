using System.Windows;
using System.Windows.Input;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
using APMTools;
using APM_SubSystems;
using System;
using System.Linq;

namespace APM_SubSystems
{
    public partial class frm_inv_goods_request : WindowTwoTabs<stp_inv_goods_request_selResult, stp_inv_goods_request_article_selResult>
    {
        #region Constructor
        public frm_inv_goods_request()
        {
            InitializeComponent();
            Initial_WindowTwoTab(documentHeader, txt_inv_goods_request_article_count, lbl_countTitle, null, null, cmb_inv_goods_request_article_glb_measure_id, dbg_Request_list, dbg_request_article, tbr_goods_request, documentHeader, grp_atticle, "inv_goods_request", "inv_goods_request_article", tab_main, txt_inv_goods_request_article_description, null, null, new APM_SubSystems.APM_Inventory.inv_goods_request.rpt_inv_goods_request(),null);
        }
        #endregion

        #region Events
        private void MainStore_BrowseClick(object sender, RoutedEventArgs e)
        {
            {
                var saveStoreId = selectedRecord.inv_goods_request_inv_store_id;
                BrowseClick(new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store),sender);
                if (selectedRecord.inv_goods_request_inv_store_id != saveStoreId)
                {
                    GlobalFunctions.Copy_PK_To_FK(selectedArticle, new stp_inv_group_goods_selResult());
                    MoveCollectionView(collectionViewArticle);
                }

            }
            
        }
        private void SelectCostCenter_BrowseClick(object sender, RoutedEventArgs e)
        {
            var saveCostCenterId = selectedRecord.inv_goods_request_glb_cost_center_id;
            BrowseClick(new WindowSelectGrid<stp_glb_cost_center_selResult>(), "مرکز هزینه درخواست دهنده", typeof(frm_CostCenter),sender);
            if (selectedRecord.inv_goods_request_glb_cost_center_id != saveCostCenterId)
            {
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_glb_personel_selResult(), FieldNames<stp_inv_goods_request_selResult>.RequesterPersonelId);
                MoveCollectionView(collectionView);
            }
        }
        private void RequesterPersonel_BrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_Parameter(new WindowSelectGridGroup<stp_glb_personel_selResult, stp_glb_personel_group_selResult>(), selectedRecord, 
                new stp_glb_personel_selResult() {glb_personel_glb_cost_center_id = selectedRecord.inv_goods_request_glb_cost_center_id }, 
                "درخواست دهنده", typeof(frm_Personel), sender);
        }
        private void ConfirmerPersonel_BrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_glb_personel_selResult, stp_glb_personel_group_selResult>(), "تایید کننده", typeof(frm_Personel),sender);
        }
        private void SelectGood_BrowseClick(object sender, RoutedEventArgs e)
        {
            if (selectedRecord.inv_goods_request_inv_store_id == 0)
            {
                Messages.ErrorMessage("لطفاّ انبار مورد نظر را انتخاب کنید");
                return;
            }
            BrowseClick_Parameter(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_Entity, "کالا و گروه کالا"),selectedArticle, 
                new stp_inv_group_goods_for_select_selResult() { inv_group_goods_for_select_inv_store_id = selectedRecord.inv_goods_request_inv_store_id}, 
                "کالا", typeof(frm_group_goods), sender);
        }
        private void brw_inv_goods_request_article_inv_group_goods_TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown_Filter<stp_inv_group_goods_for_select_selResult, stp_inv_goods_request_article_selResult>(sender, FieldNames<stp_inv_goods_request_article_selResult>.GroupGoodsId, e, selectedArticle, new stp_inv_group_goods_for_select_selResult() { inv_group_goods_for_select_inv_store_id = selectedRecord.inv_goods_request_inv_store_id });
        }
        private void mnuCardex_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_cardex().CustomReport(
                new stp_inv_rpt_goods_cardex_selResult()
                {
                    inv_rpt_goods_cardex_inv_group_goods_id = selectedArticle.inv_goods_request_article_inv_group_goods_id,
                    inv_rpt_goods_cardex_inv_group_goods_code = selectedArticle.inv_goods_request_article_inv_group_goods_code,
                    inv_rpt_goods_cardex_inv_group_goods_name = selectedArticle.inv_goods_request_article_inv_group_goods_name
                });
        }
        #endregion
    }
}
