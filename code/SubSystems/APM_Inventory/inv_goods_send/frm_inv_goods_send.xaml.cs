using BusinessLogicLayer;
using System;
using UserInterfaceLayer;
using APMTools;
using DataAccessLayer;
using System.Collections.Generic;
using System.Windows;
using APM_SubSystems;
using APMComponents;
using System.Linq;
using System.IO;
using APM_Accounting;

namespace APM_SubSystems
{
    public partial class frm_inv_goods_send : WindowTwoTabs<stp_inv_goods_send_selResult, stp_inv_goods_send_article_selResult>
    {
        #region Variables
        bool isClosing;
        stp_inv_store_selResult inv_store = new stp_inv_store_selResult();
        #endregion

        #region Constructors
        public frm_inv_goods_send(bool isFinancial, bool isClosing)
            : base(isFinancial)
        {
            this.isClosing = isClosing;
            InitializeComponent();
            Initial_WindowTwoTab(documentHeader, txt_inv_goods_send_article_count, lbl_countTitle, txt_inv_goods_send_article_price, lbl_inv_goods_send_article_price_lbl, cmb_inv_goods_send_article_glb_measure_id, dbg_send_master, dbg_send_article, tbr_Send, documentHeader, grp_article_current_row, "inv_goods_send", "inv_goods_send_article", tab_main, txt_inv_goods_send_article_description, 7, 8, new APM_SubSystems.APM_Inventory.inv_goods_send.rpt_inv_goods_send(), 2, 3, 4, 6);
            RecordParameter.inv_goods_send_is_closing = isClosing;
            selectedRecord.inv_goods_send_is_closing = isClosing;
            documentHeader.XType = isClosing ? DocumentTypes.Opening : DocumentTypes.Send;
            GlobalFunctions.SetVisibilityForControl(tbrButton_hesabdari, isFinancial);
        }
        public frm_inv_goods_send() : this(false, false) { }
        #endregion

        #region ControlEvents

        #region BrowseClicks
        private void SelectStore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var saveStoreId = selectedRecord.inv_goods_send_inv_store_id;
            inv_store = BrowseClick(new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store),sender);
            if (selectedRecord.inv_goods_send_inv_store_id != saveStoreId)
            {

                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_request_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_receive_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedArticle, new stp_inv_group_goods_selResult());
                MoveCollectionView();
                MoveCollectionView(collectionViewArticle);
            }

        }
        private void SelectBaseRequest_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_receive_selResult());
            var result = BrowseClick_Parameter(new WindowSelectGrid<stp_inv_goods_request_selResult>(), selectedRecord,
                new stp_inv_goods_request_selResult() { inv_goods_request_inv_store_id = selectedRecord.inv_goods_send_inv_store_id }
                , "درخواست کالا", typeof(frm_inv_goods_request), sender);
            CopyArticlesFromSourceDocument<stp_inv_goods_request_article_selResult>();
        }
        private void SelectBaseReceive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (selectedRecord.inv_goods_send_send_type_glb_coding_id != (long)SendType.Transitive)
            {
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_request_selResult());
                BrowseClick_Parameter(new WindowSelectGrid<stp_inv_goods_receive_selResult>(), selectedRecord,
                    new stp_inv_goods_receive_selResult() { inv_goods_receive_inv_store_id = selectedRecord.inv_goods_send_inv_store_id }
                    , "رسید", typeof(frm_inv_goods_receive), sender);
            }
            else
                BrowseClick(new WindowSelectGrid<stp_inv_goods_receive_selResult>(), "رسید", typeof(frm_inv_goods_receive),sender);

            CopyArticlesFromSourceDocument<stp_inv_goods_receive_article_selResult>();
        }
        private void SelectGoods_BrowseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (selectedRecord.inv_goods_send_inv_store_id == 0)
            {
                Messages.ErrorMessage("لطفاّ انبار مورد نظر را انتخاب کنید");
                return;
            }
            BrowseClick_Parameter(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_Entity, "کالا و گروه کالا"), selectedArticle,
                new stp_inv_group_goods_for_select_selResult() { inv_group_goods_for_select_inv_store_id = selectedRecord.inv_goods_send_inv_store_id},
                "کالا", typeof(frm_group_goods), sender);
            SetEnableForPrice();
        }
        private void DestinationPerson_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "طرف سند", typeof(frm_acc_detail),sender);
        }

        #endregion

        #region TextBoxKeyDown
        private void brw_inv_goods_send_article_inv_group_goods_TextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CodeTextBox_KeyDown_Filter<stp_inv_group_goods_for_select_selResult, stp_inv_goods_send_article_selResult>(sender, FieldNames<stp_inv_goods_send_article_selResult>.GroupGoodsId, e, selectedArticle, new stp_inv_group_goods_for_select_selResult() { inv_group_goods_for_select_inv_store_id = selectedRecord.inv_goods_send_inv_store_id });
        }
        #endregion

        #region Other
        private void btn_selected_good_info_Click(object sender, RoutedEventArgs e)
        {
            if (selectedArticle.inv_goods_send_article_inv_group_goods_code != null)
            {
                GoodsPropertiesFromDatabase();
                CalculateStockAfterChanges();
            }
        }
        private void btnFinancial_Click(object sender, RoutedEventArgs e)
        {
            var BLL_calculate_price_of_send_and_receive = new BLL<stp_acc_inv_calculate_price_of_sends_and_receivesResult>();
            if (!BLL_calculate_price_of_send_and_receive.DoDataBaseOperation())
                return;
            RefreshClick();
        }
        private void mnuCardex_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_cardex().CustomReport(
                new stp_inv_rpt_goods_cardex_selResult()
                {
                    inv_rpt_goods_cardex_inv_group_goods_id = selectedArticle.inv_goods_send_article_inv_group_goods_id,
                    inv_rpt_goods_cardex_inv_group_goods_code = selectedArticle.inv_goods_send_article_inv_group_goods_code,
                    inv_rpt_goods_cardex_inv_group_goods_name = selectedArticle.inv_goods_send_article_inv_group_goods_name
                });
        }
        #endregion

        #endregion

        #region Override
        public override void ChangeSelectedRecordArticle(stp_inv_goods_send_article_selResult newSelectedRecord)
        {
            base.ChangeSelectedRecordArticle(newSelectedRecord);
            SetEnableForPrice();
        }
        public override void SetEnablesForArticleGroupBox()
        {
            base.SetEnablesForArticleGroupBox();
            SetEnableForPrice();
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            selectedRecord = selectedRecord;
            if (isClosing)
                RecordParameter.inv_goods_send_is_closing = true;
            base.Window_Loaded(sender, e);
            if (isClosing)
            {
                tbi_inv_send.Header = "لیست اسناد اختتاحیه";
                tbi_inv_send_article.Header = "جزئیات لیست سند اختتاحیه";
                dbg_send_master.datagrid.Columns[1].Header = "شماره اختتاحیه";
                dbg_send_master.datagrid.Columns[2].Visibility = Visibility.Collapsed;
                dbg_send_master.datagrid.Columns[4].Visibility = Visibility.Collapsed;
                dbg_send_master.datagrid.Columns[5].Visibility = Visibility.Collapsed;
                dbg_send_article.datagrid.Columns[5].Header = "مقدار اختتاحیه";
            }

        }
        public override void InitializationBeforeSave()
        {
            base.InitializationBeforeSave();
            if (isClosing && operationType == OperationType.Insert)
            {
                selectedRecord.inv_goods_send_is_closing = true;
                selectedRecord.inv_goods_send_send_type_glb_coding_id = 268;
                selectedRecord.inv_goods_send_destination_acc_detail_id = inv_store.inv_store_acc_detail_id;
            }
        }
        public override void PrintClick()
        {
            if (isFinancial==true)
                printReportFile = new APM_SubSystems.APM_Inventory.inv_goods_send.rpt_inv_goods_send_financial() ;
            else
                printReportFile = new APM_SubSystems.APM_Inventory.inv_goods_send.rpt_inv_goods_send();
            base.PrintClick();
        }
        public override void CreateSearchForm()
        {
            if (isClosing)
            {
                searchForm = new WindowSearch<stp_inv_goods_send_selResult>(DocumentTypes.OpeningReport);
                searchForm.selectedRecord.inv_goods_send_is_closing = true;
            }
            else
                base.CreateSearchForm();
        }
       
        #endregion

        #region Tools
        public void GoodsPropertiesFromDatabase()
        {

            stp_inv_goods_store_selResult inputRecord = new stp_inv_goods_store_selResult();
            inputRecord.inv_goods_store_inv_store_id = selectedRecord.inv_goods_send_inv_store_id;
            inputRecord.inv_goods_store_inv_group_goods_id = selectedArticle.inv_goods_send_article_inv_group_goods_id;
            if (inputRecord.inv_goods_store_inv_store_id == 0 || inputRecord.inv_goods_store_inv_group_goods_id == 0)
                return;
            var SelectedItemList = ((new BLL<stp_inv_goods_store_selResult>()).GetSomeRecords_DB(inputRecord));
            if (SelectedItemList == null || SelectedItemList.Count == 0)
                return;
            stp_inv_goods_store_selResult SelectedItem = SelectedItemList[0];
            selectedArticle.inv_goods_send_article_inv_goods_store_stock = SelectedItem.inv_goods_store_stock;
            selectedArticle.inv_goods_send_article_goods_financial_pricing = SelectedItem.inv_goods_store_financial_glb_coding_name;
            selectedArticle.inv_goods_send_article_inv_goods_order_point = SelectedItem.inv_goods_store_goods_order_point;
            selectedArticle.inv_goods_send_article_main_glb_measure_name = SelectedItem.inv_goods_store_glb_measure_name;
            selectedArticle.inv_goods_send_article_inv_goods_order_min = SelectedItem.inv_goods_store_order_min;
            selectedArticle.inv_goods_send_article_inv_store_name = SelectedItem.inv_goods_store_store_name;
            return;
        }
        public void CalculateStockAfterChanges()
        {
            double sum = 0;
            if (selectedArticle == null)
                return;
            if (selectedArticle.inv_goods_send_article_inv_goods_store_stock == null)
                selectedArticle.inv_goods_send_article_inv_goods_store_stock = 0;
            foreach (stp_inv_goods_send_article_selResult article in bindingListArticle)
                if
                (
                    article.inv_goods_send_article_inv_group_goods_id == selectedArticle.inv_goods_send_article_inv_group_goods_id &&
                    article.inv_goods_send_article_id == 0
                )
                    sum += (double)article.inv_goods_send_article_count;
            selectedArticle.inv_goods_send_article_goods_stock_after_change = selectedArticle.inv_goods_send_article_inv_goods_store_stock - sum;

            MoveCollectionView(collectionViewArticle);
        }
        private void SetEnableForPrice()
        {
            bool enable =
            (
                isFinancial == true &&
                operationType != OperationType.Nothing &&
                selectedArticle != null &&
                selectedArticle.inv_goods_send_article_inv_group_goods_financial_glb_coding_id == (long)GoodsFinancial.Inv_Specific_Price &&
                !documentHeader.XBasedocumentIsSendOrReceive
            );
            txt_inv_goods_send_article_price.IsEnabled = enable;
            lbl_inv_goods_send_article_price_lbl.IsEnabled = enable;
        }
        #endregion

        private void APMMenuItemShowFinancial_Click(object sender, RoutedEventArgs e)
        {
            var currentRecord = dataGrid.CurrentItem as stp_inv_goods_send_selResult;
            new frm_inv_goods_send(true, false).ShowOneDocument(currentRecord.inv_goods_send_id);
        }

        private void APMMenuItemShowAccDoc_Click(object sender, RoutedEventArgs e)
        {
            var current = dataGrid.CurrentItem as stp_inv_goods_send_selResult;
            if (current.inv_goods_send_have_acc_document == false)
                Messages.InformationMessage("این حواله سند حسابداری ندارد");
            else
            {
                stp_acc_document_selResult parameter = new stp_acc_document_selResult();
                parameter.acc_document_inv_goods_send_id = current.inv_goods_send_id;
                new frm_acc_document().ShowOneDocument(parameter);
            }
        }

        
    }
}