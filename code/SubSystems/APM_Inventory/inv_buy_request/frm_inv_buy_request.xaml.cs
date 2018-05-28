using UserInterfaceLayer;
using APMTools;
using DataAccessLayer;
using APM_SubSystems;
using System.Linq;
using System.Windows;

namespace APM_SubSystems
{
    public partial class frm_inv_buy_request : WindowTwoTabs<stp_inv_buy_request_selResult, stp_inv_buy_request_article_selResult>
    {
        #region Initialize
        public frm_inv_buy_request()
        {
            InitializeComponent();
            Initial_WindowTwoTab(documentHeader, txt_inv_buy_request_article_count, lbl_count, null, null, cmb_inv_buy_request_article_glb_measure_id, dbg_buy_request_master, dbg_buy_request_article, tbr_buy_request, documentHeader, grp_article_current_row, "inv_buy_request", "inv_buy_request_article", tab_main, txt_inv_buy_request_article_description, 2, 3, new APM_SubSystems.APM_Inventory.inv_buy_request.rpt_inv_buy_request(), 2, 4, 5);
        }
        #endregion

        #region Events
        private void documentHeader_XBrowseClick_MainStore(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store), sender);
            GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_buy_request_selResult());
            MoveCollectionView();
        }
        private void documentHeader_XBrowseClick_BaseGoodsRequest(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick_Parameter(new WindowSelectGrid<stp_inv_goods_request_selResult>(), selectedRecord, 
                new stp_inv_goods_request_selResult() { inv_goods_request_inv_store_id = selectedRecord.inv_buy_request_inv_store_id }
                , "درخواست کالا", typeof(frm_inv_goods_request), sender);
            CopyArticlesFromSourceDocument<stp_inv_goods_request_article_selResult>();
        }
        private void documentHeader_XBrowseClick_RequestConfirmerPersonel(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_glb_personel_selResult, stp_glb_personel_group_selResult>(), "پرسنل", typeof(frm_Personel), sender);
        }
        private void XBrowseClick_GoodsSelect(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_Entity, "کالا و گروه کالا"), selectedArticle, "کالا",typeof(frm_group_goods),sender);
        }
        private void XTextBoxKeyDown_GoodsSelect(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_inv_group_goods_selResult, stp_inv_buy_request_article_selResult>(sender, FieldNames<stp_inv_buy_request_article_selResult>.GroupGoodsId, e, selectedArticle);
        }
        private void mnuCardex_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_cardex().CustomReport(
                new stp_inv_rpt_goods_cardex_selResult()
                {
                    inv_rpt_goods_cardex_inv_group_goods_id = selectedArticle.inv_buy_request_article_inv_group_goods_id,
                    inv_rpt_goods_cardex_inv_group_goods_code = selectedArticle.inv_buy_request_article_inv_group_goods_code,
                    inv_rpt_goods_cardex_inv_group_goods_name = selectedArticle.inv_buy_request_article_inv_group_goods_name
                });
        }
        #endregion

    }
}
