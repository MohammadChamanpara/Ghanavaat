using System.Windows;
using System.Windows.Input;
using DataAccessLayer;
using UserInterfaceLayer;
using APM_SubSystems;
using System.Windows.Controls;
using APMTools;
using System.Collections.Generic;
using System.Linq;
using APM_Accounting;
using System;
using BusinessLogicLayer;

namespace APM_SubSystems
{
    public partial class frm_inv_goods_receive : WindowTwoTabs<stp_inv_goods_receive_selResult, stp_inv_goods_receive_article_selResult>
    {
        #region Variables
        bool isOpening;
        stp_inv_store_selResult inv_store = new stp_inv_store_selResult();
        #endregion

        #region Constructor
        public frm_inv_goods_receive() : this(false, false) { }
        public frm_inv_goods_receive(bool isFinancial, bool isOpening)
            : base(isFinancial)
        {
            this.isOpening = isOpening;
            InitializeComponent();
            Initial_WindowTwoTab(documentHeader, txt_inv_goods_receive_article_count,
                lbl_count, txt_inv_goods_receive_article_price,
                lbl_inv_goods_receive_article_price_lbl,
                cmb_inv_goods_receive_article_glb_measure_id,
                dbg_receive_master, dbg_inv_goods_receive_sum, tbr_receive,
                documentHeader, grp_article_current_row, "inv_goods_receive",
                "inv_goods_receive_article", tab_main,
                txt_inv_goods_receive_article_description, 5, 6,
                new APM_SubSystems.APM_Inventory.inv_goods_receive.rpt_inv_goods_receive(),2, 3);

            RecordParameter.inv_goods_receive_is_opening = isOpening;
            selectedRecord.inv_goods_receive_is_opening = isOpening;
            documentHeader.XType = isOpening ? DocumentTypes.Opening : DocumentTypes.Receive;
            GlobalFunctions.SetVisibilityForControl(btn_hesabdari, isFinancial);
        }
        #endregion

        #region BrowseClick
        private void documentHeader_XBrowseClick_MainStore(object sender, RoutedEventArgs e)
        {
            var saveStoreId = selectedRecord.inv_goods_receive_inv_store_id;
            inv_store = BrowseClick(new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store), sender);
            if (selectedRecord.inv_goods_receive_inv_store_id != saveStoreId)
            {
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_buy_request_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_send_selResult());
                MoveCollectionView();
            }
        }
        private void documentHeader_XBrowseClick_BaseBuyRequest(object sender, RoutedEventArgs e)
        {
            GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_send_selResult());
            BrowseClick_Parameter(new WindowSelectGrid<stp_inv_buy_request_selResult>(),
                selectedRecord, new stp_inv_buy_request_selResult() { inv_buy_request_inv_store_id = selectedRecord.inv_goods_receive_inv_store_id },
                "درخواست خرید", typeof(frm_inv_buy_request), sender);
            CopyArticlesFromSourceDocument<stp_inv_buy_request_article_selResult>();
            MoveCollectionView();
        }
        private void DestinationDetail_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "طرف سند", typeof(frm_acc_detail), sender);

        }
        private void documentHeader_XBrowseClick_BaseSend(object sender, RoutedEventArgs e)
        {
            if (selectedRecord.inv_goods_receive_receive_type_glb_coding_id != (long)ReceiveType.Transitive)
            {
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_buy_request_selResult());
                BrowseClick_Parameter(new WindowSelectGrid<stp_inv_goods_send_selResult>(), selectedRecord,
                    new stp_inv_goods_send_selResult() { inv_goods_send_inv_store_id = selectedRecord.inv_goods_receive_inv_store_id },
                    "حواله", typeof(frm_inv_goods_send), sender);
            }
            else
                BrowseClick(new WindowSelectGrid<stp_inv_goods_send_selResult>(), "حواله", typeof(frm_inv_goods_send), sender);
            CopyArticlesFromSourceDocument<stp_inv_goods_send_article_selResult>();
            MoveCollectionView();
        }
        private void brw_inv_goods_receive_article_inv_group_goods_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_Entity, "کالا و گروه کالا"), selectedArticle, "کالا", typeof(frm_group_goods), sender);
            selectedArticle.inv_goods_receive_article_price = null;
            selectedArticle.inv_goods_receive_article_all_price = null;
            MoveCollectionViewArticle();
        }
        #endregion

        #region TxtKeyDown
        private void brw_inv_goods_receive_article_inv_group_goods_id_XTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_inv_group_goods_selResult, stp_inv_goods_receive_article_selResult>(sender, FieldNames<stp_inv_goods_receive_article_selResult>.GroupGoodsId, e, selectedArticle);
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            if (isOpening)
            {
                this.Title = "افتتاحیه";
                tbi_inv_receive.Header = "لیست اسناد افتتاحیه";
                tbi_inv_receive_article.Header = "جزئیات سند افتتاحیه";
                dbg_receive_master.datagrid.Columns[2].Header = "شماره افتتاحیه";
                dbg_receive_master.datagrid.Columns[3].Visibility = Visibility.Collapsed;
                dbg_receive_master.datagrid.Columns[5].Visibility = Visibility.Collapsed;
                dbg_receive_master.datagrid.Columns[6].Visibility = Visibility.Collapsed;
                dbg_inv_goods_receive_sum.datagrid.Columns[4].Header = "مقدار افتتاحیه";
            }
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
        }
        public override void InitializationBeforeSave()
        {
            base.InitializationBeforeSave();
            if (isOpening && operationType == OperationType.Insert)
            {
                selectedRecord.inv_goods_receive_is_opening = true;
                selectedRecord.inv_goods_receive_receive_type_glb_coding_id = 267;
                selectedRecord.inv_goods_receive_destination_acc_detail_id = inv_store.inv_store_acc_detail_id;
            }
        }
        public override void SetEnablesForArticleGroupBox()
        {
            base.SetEnablesForArticleGroupBox();
            SetEnableForPrice();
        }
        public override void PrintClick()
        {
            if (isFinancial == true)
                if (isOpening == true)
                    printReportFile = new APM_SubSystems.APM_Inventory.inv_goods_receive.rpt_inv_goods_receive_opening_isfinincial();
                else
                    printReportFile = new APM_SubSystems.APM_Inventory.inv_goods_receive.rpt_inv_goods_receive_financal();
            else
                if (isOpening == true)
                    printReportFile = new APM_SubSystems.APM_Inventory.inv_goods_receive.rpt_inv_goods_opening();
                else
                    printReportFile = new APM_SubSystems.APM_Inventory.inv_goods_receive.rpt_inv_goods_receive();
            base.PrintClick();
        }
        public override void CreateSearchForm()
        {
            if (isOpening)
            {
                searchForm = new WindowSearch<stp_inv_goods_receive_selResult>(DocumentTypes.OpeningReport);
                searchForm.selectedRecord.inv_goods_receive_is_opening = true;
            }
            else
                base.CreateSearchForm();
        }
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            btn_product.ToolTip = "صدور حوالۀ مواد اولیه بر اساس رسید محصول";
        }
        #endregion

        #region Tools
        private void SetEnableForPrice()
        {
            bool enable = (isFinancial == true && operationType != OperationType.Nothing && !documentHeader.XBasedocumentIsSendOrReceive);
            txt_inv_goods_receive_article_price.IsEnabled = enable;
            lbl_inv_goods_receive_article_price_lbl.IsEnabled = enable;
        }
        #endregion

        #region Events

        private void btn_hesabdari_Click(object sender, RoutedEventArgs e)
        {

            var BLL_price = new BLL<stp_acc_inv_calculate_price_of_sends_and_receivesResult>();
            if (!BLL_price.DoDataBaseOperation())
                return;
            RefreshClick();
        }
        private void btn_product_Click(object sender, RoutedEventArgs e)
        {
            var sendMaster = new stp_inv_goods_send_selResult();
            var sendArticles = new List<stp_inv_goods_send_article_selResult>();
            var storesList = new BLL<stp_inv_store_selResult>().GetAllRecords_DB();
            var partsStore = storesList.FindLast(store => store.inv_store_name.Contains("اولیه"));
            if (partsStore == null && storesList.Count > 0)
                partsStore = storesList[0];
            sendMaster.inv_goods_send_inv_store_id = partsStore.inv_store_id;
            sendMaster.inv_goods_send_inv_store_name = partsStore.inv_store_name;
            sendMaster.inv_goods_send_inv_store_code = partsStore.inv_store_code;
            sendMaster.inv_goods_send_description = "حواله مواد اولیه بر اساس رسید محصول - شمارۀ سریال رسید : " + selectedRecord.inv_goods_receive_no;
            sendMaster.inv_goods_send_send_type_glb_coding_id = (long)SendType.DeliverToProduction;
            var detailsList = new BLL<stp_acc_detail_selResult>().GetSomeRecords_DB
            (
                new stp_acc_detail_selResult()
                {
                    acc_detail_id = selectedRecord.inv_goods_receive_destination_acc_detail_id
                }
            );
            if (detailsList.Count == 0)
            {
                Messages.ErrorMessage("طرف سند را مشخص کنید");
                return;
            }
            var destinationDetail = detailsList[0];
            sendMaster.inv_goods_send_destination_acc_detail_id = destinationDetail.acc_detail_id;
            sendMaster.inv_goods_send_destination_acc_detail_code = destinationDetail.acc_detail_code;
            sendMaster.inv_goods_send_destination_acc_detail_name = destinationDetail.acc_detail_name;
            foreach (stp_inv_goods_receive_article_selResult productRA in bindingListArticle)
            {
                var partsList = new BLL<stp_inv_product_part_selResult>().
                    GetSomeRecords_DB(new stp_inv_product_part_selResult()
                    {
                        inv_product_part_product_inv_group_goods_id = productRA.inv_goods_receive_article_inv_group_goods_id
                    });

                foreach (stp_inv_product_part_selResult partFormula in partsList)
                {
                    var sendArticle = new stp_inv_goods_send_article_selResult();
                    var convertResult = new stp_inv_convert_count_from_measure1_to_measure2Result
                    {
                        inv_goods_id = productRA.inv_goods_receive_article_inv_group_goods_id,
                        count1 = productRA.inv_goods_receive_article_count,
                        glb_measure_id1 = productRA.inv_goods_receive_article_glb_measure_id,
                        glb_measure_id2 = partFormula.inv_product_part_product_glb_measure_id
                    };

                    convertResult = new BLL<stp_inv_convert_count_from_measure1_to_measure2Result>().GetOneRecord(convertResult);
                    if (convertResult == null || convertResult.count2 == null)
                    {
                        Messages.ErrorMessage("بروز خطا هنگام محاسبۀ تعداد کالای " + partFormula.inv_product_part_part_inv_group_goods_name);
                        continue;
                    }
                    var resultCount = convertResult.count2;
                    sendArticle.inv_goods_send_article_count = resultCount.Value * partFormula.inv_product_part_formula;
                    sendArticle.inv_goods_send_article_glb_measure_id = partFormula.inv_product_part_part_glb_measure_id;
                    sendArticle.inv_goods_send_article_glb_measure_name = partFormula.inv_product_part_part_glb_measure_name;
                    sendArticle.inv_goods_send_article_inv_group_goods_id = partFormula.inv_product_part_part_inv_group_goods_id;
                    sendArticle.inv_goods_send_article_inv_group_goods_name = partFormula.inv_product_part_part_inv_group_goods_name;
                    sendArticle.inv_goods_send_article_inv_group_goods_code = partFormula.inv_product_part_part_inv_group_goods_code;
                    sendArticle.inv_goods_send_article_description = "کالای تشکیل دهندۀ محصول " + partFormula.inv_product_part_product_inv_group_goods_name;
                    sendArticle.inv_goods_send_article_count_measure = sendArticle.inv_goods_send_article_count + " " + sendArticle.inv_goods_send_article_glb_measure_name;
                    sendArticles.Add(sendArticle);
                }
            }
            if (sendArticles.Count > 0)
                new frm_inv_goods_send().CreateDocument(sendMaster, sendArticles);
            else
                Messages.InformationMessage("کالاهای این رسید محصول نیستند");
        }
        private void mnuCardex_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_cardex().CustomReport(
                new stp_inv_rpt_goods_cardex_selResult()
                {
                    inv_rpt_goods_cardex_inv_group_goods_id = selectedArticle.inv_goods_receive_article_inv_group_goods_id,
                    inv_rpt_goods_cardex_inv_group_goods_code = selectedArticle.inv_goods_receive_article_inv_group_goods_code,
                    inv_rpt_goods_cardex_inv_group_goods_name = selectedArticle.inv_goods_receive_article_inv_group_goods_name
                });
        }

        private void APMMenuItemShowFinancial_Click(object sender, RoutedEventArgs e)
        {
            var currentRecord = dataGrid.CurrentItem as stp_inv_goods_receive_selResult;
            new frm_inv_goods_receive(true, false).ShowOneDocument(currentRecord.inv_goods_receive_id);
        }

        private void APMMenuItemShowAccDoc_Click(object sender, RoutedEventArgs e)
        {
            var current = dataGrid.CurrentItem as stp_inv_goods_receive_selResult;
            if (current.inv_goods_receive_have_acc_document == false)
                Messages.InformationMessage("این رسید سند حسابداری ندارد");
            else
            {
                stp_acc_document_selResult parameter = new stp_acc_document_selResult();
                parameter.acc_document_inv_goods_receive_id = current.inv_goods_receive_id;
                new frm_acc_document().ShowOneDocument(parameter);
            }
        }
        #endregion
    }
}
