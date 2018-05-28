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
    public partial class frm_inv_physical_counting : WindowTwoTabs<stp_inv_physical_counting_selResult, stp_inv_physical_counting_article_selResult>
    {
        #region Variable
        stp_inv_goods_store_selResult SelectedItem;

        #endregion

        #region Constructor
        public frm_inv_physical_counting()
        {
            InitializeComponent();
            Initial_WindowTwoTab(documentHeader, null, null, null, null, cmb_inv_physical_counting_article_glb_measure_id, dbg_physical_master, dbg_inv_physical_count_sum, tbr_physical_count, documentHeader, grp_article_current_row, "inv_physical_counting",
           "inv_physical_counting_article", tab_main, txt_inv_physical_counting_article_description, null, null, new APM_SubSystems.APM_Inventory.inv_physical_counting.rpt_inv_physical_counting(), null);

        }
        #endregion

        #region Events

        private void APMToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            string DescriptionSend, DescriptionReceive;
            MessageBoxResult result = new MessageBoxResult();
            ////در صورتی که این سند انبار گردانی تعدیل شده باشد پیغام می دهد
            if (selectedRecord.inv_physical_counting_have_adjustment == true)
                result = Messages.QuestionMessage_YesNo(" این سند انبار گردانی دارای سند تعدیل می باشد آیا  می خواهید مجدداً سند تعدیل صادر شود");
            if (result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
                return;
            /////
            List<stp_inv_goods_send_article_selResult> ArticleListForSendForm = new List<stp_inv_goods_send_article_selResult>();
            List<stp_inv_goods_receive_article_selResult> ArticleListForReceiveForm = new List<stp_inv_goods_receive_article_selResult>();
            /////
            stp_inv_goods_send_selResult SendRecord = new stp_inv_goods_send_selResult();
            stp_inv_goods_receive_selResult ReceiveRecord = new stp_inv_goods_receive_selResult();
            /////جهت به دست آوردن طرف سند انبار 
            stp_inv_store_selResult store = new stp_inv_store_selResult();
            store.inv_store_id = selectedRecord.inv_physical_counting_inv_store_id;
            store = new BLL<stp_inv_store_selResult>().GetOneRecord(store);
            DescriptionSend = "سند تعدیل کاهشی مربوط به سند انبار گردانی" + " " + selectedRecord.inv_physical_counting_code;
            SendRecord.inv_goods_send_inv_store_id = selectedRecord.inv_physical_counting_inv_store_id;
            SendRecord.inv_goods_send_type_glb_coding_name = SendType.Adjustment.ToString();
            SendRecord.inv_goods_send_send_type_glb_coding_id = (long)SendType.Adjustment;
            SendRecord.inv_goods_send_date = APMDateTime.Today;
            SendRecord.inv_goods_send_inv_store_id = selectedRecord.inv_physical_counting_inv_store_id;
            SendRecord.inv_goods_send_inv_store_name = selectedRecord.inv_physical_counting_inv_store_name;
            SendRecord.inv_goods_send_inv_store_code = selectedRecord.inv_physical_counting_inv_store_code;
            SendRecord.inv_goods_send_description = DescriptionSend;
            SendRecord.inv_goods_send_destination_acc_detail_id = (long)store.inv_store_glb_personel_acc_detail_id;
            SendRecord.inv_goods_send_destination_acc_detail_code = store.inv_store_glb_personel_code;
            SendRecord.inv_goods_send_destination_acc_detail_name = store.inv_store_glb_personel_name;
            ////
            ////
            DescriptionReceive = "سند تعدیل افزایشی مربوط به سند انبار گردانی" + " " + selectedRecord.inv_physical_counting_code;
            ReceiveRecord.inv_goods_receive_inv_store_id = selectedRecord.inv_physical_counting_inv_store_id;
            ReceiveRecord.inv_goods_receive_type_glb_coding_name = ReceiveType.Adjustment.ToString();
            ReceiveRecord.inv_goods_receive_receive_type_glb_coding_id = (long)ReceiveType.Adjustment;
            ReceiveRecord.inv_goods_receive_date = APMDateTime.Today;
            ReceiveRecord.inv_goods_receive_inv_store_id = selectedRecord.inv_physical_counting_inv_store_id;
            ReceiveRecord.inv_goods_receive_inv_store_name = selectedRecord.inv_physical_counting_inv_store_name;
            ReceiveRecord.inv_goods_receive_inv_store_code = selectedRecord.inv_physical_counting_inv_store_code;
            ReceiveRecord.inv_goods_receive_description = DescriptionReceive;
            ReceiveRecord.inv_goods_receive_destination_acc_detail_id = (long)store.inv_store_glb_personel_acc_detail_id; ;
            ReceiveRecord.inv_goods_receive_destination_acc_detail_code = store.inv_store_glb_personel_code;
            ReceiveRecord.inv_goods_receive_destination_acc_detail_name = store.inv_store_glb_personel_name;
            ////
            foreach (stp_inv_physical_counting_article_selResult article in bindingListArticle)
            {
                if (article.inv_physical_counting_article_shortage != 0 && article.inv_physical_counting_article_shortage != null)
                {
                    stp_inv_goods_send_article_selResult Senditem = new stp_inv_goods_send_article_selResult();
                    Senditem.inv_goods_send_article_inv_group_goods_id = article.inv_physical_counting_article_inv_group_goods_id;
                    Senditem.inv_goods_send_article_inv_group_goods_name = article.inv_physical_counting_article_inv_group_goods_name;
                    Senditem.inv_goods_send_article_inv_group_goods_code = article.inv_physical_counting_article_inv_group_goods_code;
                    Senditem.inv_goods_send_article_count = System.Convert.ToDouble(article.inv_physical_counting_article_shortage);
                    Senditem.inv_goods_send_article_glb_measure_id = article.inv_physical_counting_article_glb_measure_id;
                    Senditem.inv_goods_send_article_count_measure = Senditem.inv_goods_send_article_count + article.inv_physical_counting_article_glb_measure_name;
                    Senditem.inv_goods_send_article_description = DescriptionSend;
                    ArticleListForSendForm.Add(Senditem);
                }

                else
                    if (article.inv_physical_counting_article_surplus != 0 && article.inv_physical_counting_article_surplus != null)
                    {
                        stp_inv_goods_receive_article_selResult Receiveitem = new stp_inv_goods_receive_article_selResult();
                        Receiveitem.inv_goods_receive_article_inv_group_goods_id = article.inv_physical_counting_article_inv_group_goods_id;
                        Receiveitem.inv_goods_receive_article_inv_group_goods_name = article.inv_physical_counting_article_inv_group_goods_name;
                        Receiveitem.inv_goods_receive_article_inv_group_goods_code = article.inv_physical_counting_article_inv_group_goods_code;
                        Receiveitem.inv_goods_receive_article_glb_measure_id = article.inv_physical_counting_article_glb_measure_id;
                        Receiveitem.inv_goods_receive_article_count = System.Convert.ToDouble(article.inv_physical_counting_article_surplus);
                        Receiveitem.inv_goods_receive_article_count_measure = Receiveitem.inv_goods_receive_article_count + article.inv_physical_counting_article_glb_measure_name;
                        Receiveitem.inv_goods_receive_article_description = DescriptionReceive;
                        ArticleListForReceiveForm.Add(Receiveitem);
                    }

            }
            if (ArticleListForSendForm.Count != 0)
            {
                frm_inv_goods_send AdjustmentSendForm = new frm_inv_goods_send();
                AdjustmentSendForm.CreateDocument(SendRecord, ArticleListForSendForm);
            }

            if (ArticleListForReceiveForm.Count != 0)
            {
                frm_inv_goods_receive AdjustmentReceiveForm = new frm_inv_goods_receive();
                AdjustmentReceiveForm.CreateDocument(ReceiveRecord, ArticleListForReceiveForm);
            }
            RefreshClick();
        }

        private void documentHeader_XBrowseClick_MainStore(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", typeof(frm_inv_store), sender);

        }

        private void brw_inv_physical_counting_article_inv_group_goods_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            if (selectedRecord.inv_physical_counting_inv_store_id == 0)
            {
                Messages.ErrorMessage("لطفاّ انبار مورد نظر را انتخاب کنید");
                return;
            }
            BrowseClick_Parameter(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_Entity, "کالا و گروه کالا"),
                selectedArticle, new stp_inv_group_goods_for_select_selResult() { inv_group_goods_for_select_inv_store_id = selectedRecord.inv_physical_counting_inv_store_id },
                "کالا", typeof(frm_group_goods), sender);
            MoveCollectionViewArticle();
            SelectedItem = new stp_inv_goods_store_selResult();
            selectedArticle.inv_physical_counting_article_inv_goods_store_stock = SelectedItem.inv_goods_store_stock;
            selectedArticle.inv_physical_counting_article_main_glb_measure_name = SelectedItem.inv_goods_store_glb_measure_name;
            selectedArticle.inv_physical_counting_article_inv_store_name = SelectedItem.inv_goods_store_store_name;
            selectedArticle.inv_physical_counting_article_shortage = 0;
            selectedArticle.inv_physical_counting_article_surplus = 0;
            MoveCollectionView(collectionViewArticle);
        }

        private void documentHeader_XBrowseClick_AccountingPersonel(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_glb_personel_selResult>(), "حسابدار", typeof(frm_Personel), sender);
        }

        private void btn_selected_good_info_Click(object sender, RoutedEventArgs e)
        {
            if (selectedArticle == null)
                return;
            stp_inv_goods_store_selResult inputRecord = new stp_inv_goods_store_selResult();
            inputRecord.inv_goods_store_inv_store_id = selectedRecord.inv_physical_counting_inv_store_id;
            inputRecord.inv_goods_store_inv_group_goods_id = selectedArticle.inv_physical_counting_article_inv_group_goods_id;
            if (inputRecord.inv_goods_store_inv_store_id == 0 || inputRecord.inv_goods_store_inv_group_goods_id == 0)
                return;
            var SelectedItemList = ((new BLL<stp_inv_goods_store_selResult>()).GetSomeRecords_DB(inputRecord));
            if (SelectedItemList == null || SelectedItemList.Count == 0)
                return;
            stp_inv_goods_store_selResult SelectedItem = SelectedItemList[0];
            selectedArticle.inv_physical_counting_article_inv_goods_store_stock = SelectedItem.inv_goods_store_stock;
            selectedArticle.inv_physical_counting_article_main_glb_measure_name = SelectedItem.inv_goods_store_glb_measure_name;
            selectedArticle.inv_physical_counting_article_inv_store_name = SelectedItem.inv_goods_store_store_name;
            MoveCollectionView(collectionViewArticle);
            calculationSupply();
        }

        private void txt_inv_physical_counting_article_third_count_LostFocus(object sender, RoutedEventArgs e)
        {
            btn_selected_good_info_Click(null, null);
        }

        private void mnuCardex_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_cardex().CustomReport(
                new stp_inv_rpt_goods_cardex_selResult()
                {
                    inv_rpt_goods_cardex_inv_group_goods_id = selectedArticle.inv_physical_counting_article_inv_group_goods_id,
                    inv_rpt_goods_cardex_inv_group_goods_code = selectedArticle.inv_physical_counting_article_inv_group_goods_code,
                    inv_rpt_goods_cardex_inv_group_goods_name = selectedArticle.inv_physical_counting_article_inv_group_goods_name
                });
        }
        #endregion

        #region Methods
        private void calculationSupply()
        {
            Double third_count, store_stock;
            third_count = txt_inv_physical_counting_article_third_count.Text == string.Empty ? 0 : Convert.ToDouble(txt_inv_physical_counting_article_third_count.Text);
            object contentOfLabel = lbl_inv_physical_counting_article_inv_goods_store_stock.Content;
            store_stock = (contentOfLabel == null || contentOfLabel.ToString() == string.Empty) ? 0 : Convert.ToDouble(contentOfLabel);
            var result = Math.Abs(third_count - store_stock);
            //در صورتی که شمارش سوم بزرگتر از موجودی انبار برنامه باشد در نتیجه مازاد داریم در برنامه بر اساس موجودی برنامه
            if (third_count > store_stock)
            {
                selectedArticle.inv_physical_counting_article_surplus = result;
                selectedArticle.inv_physical_counting_article_shortage = 0;
            }
            // در صورتی که شمارش سوم کوچکتر از موجودی انبار برنامه باشد در نتیجه در برنامه کسری داریم بر اساس موجودی برنامه 
            else if (third_count < store_stock)
            {
                selectedArticle.inv_physical_counting_article_shortage = result;
                selectedArticle.inv_physical_counting_article_surplus = 0;
            }
            else
            {
                selectedArticle.inv_physical_counting_article_shortage = 0;
                selectedArticle.inv_physical_counting_article_surplus = 0;
            }
            MoveCollectionView(collectionViewArticle);
        }
        #endregion


    }
}
