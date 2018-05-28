using System.Windows;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.Linq;
using DataAccessLayer;
using UserInterfaceLayer;
using BusinessLogicLayer;
using APMTools;
using APMComponents;
using APM_Accounting;

namespace APM_SubSystems
{

    public partial class frm_inv_create_accounting_document_multi_select : WindowBase<stp_acc_document_selResult>
    {
        #region Variable
        private ArticlePackage<stp_inv_goods_receive_inv_goods_send_selResult, stp_inv_goods_receive_inv_goods_send_selResult> ref_receive_send =
            new WindowBase<stp_acc_document_selResult>.ArticlePackage<stp_inv_goods_receive_inv_goods_send_selResult, stp_inv_goods_receive_inv_goods_send_selResult>();
        private long MaxSelectedNo;
        private Boolean CallDocumentSortNos = true;
        private frm_acc_document Form_Acc_Document;
        private frm_acc_chart_account Form_Acc_Ahart_Account;
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        #endregion

        #region Constructor
        public frm_inv_create_accounting_document_multi_select()
        {
            GlobalVariables.currentSubSystem = SubSystems.Inventory;
            InitializeComponent();
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            APMLabel label = new APMLabel() { Content = "عملیات مورد نیاز برای صدور سند حسابداری" };
            APMToolbarButton toolbarButton = new APMToolbarButton() { XImage = ButtonImageType.Action, XSize = APMToolbarButton.Size.Small, XCanMagnify = false };
            toolbarButton.Click += new RoutedEventHandler(All_Operations_Click);
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(toolbarButton);
            grp_nv_create_accounting_document.Header = stackPanel;
        }
        #endregion

        #region All_Operations_Click
        void All_Operations_Click(object sender, RoutedEventArgs e)
        {
            Sort_Nos_Account();
        }
        #endregion

        #region XBrowseClick
        private void brw_select_inv_document_XBrowseClick(object sender, RoutedEventArgs e)
        {
            if (pgb_sort_nos.Value != 100)
                Sort_Nos_Account();
            else
            {
                var x = new stp_inv_goods_receive_inv_goods_send_selResult();
                BrowseClick_MultiSelect(new WindowSelectGrid<stp_inv_goods_receive_inv_goods_send_selResult>(), ref ref_receive_send, "", typeof(frm_inv_goods_receive), null, FieldNames<stp_inv_goods_receive_inv_goods_send_selResult>.Date, pdp_inv_create_accountind_document_date.Text);
                if (ref_receive_send.ListAfterChange.Count == 0)
                    return;
                pgb_check_store.Value = 0;
                pgb_check_distination.Value = 0;
                pgb_calculate_price_of_receive_and_sen.Value = 0;
                ref_receive_send.ListBeforeChange = ref_receive_send.ListAfterChange;
                MaxSelectedNo = 0;
                foreach (stp_inv_goods_receive_inv_goods_send_selResult item in ref_receive_send.ListAfterChange)
                {
                    if (item.inv_goods_receive_inv_goods_send_no > MaxSelectedNo)
                        MaxSelectedNo = item.inv_goods_receive_inv_goods_send_no;
                }
                Check_Store_Account();
            }
        }
        #endregion

        #region Tools
        private void Sort_Nos_Account()
        {
            if (pgb_sort_nos.Value != 100)
            {
                if (!new BLL<stp_inv_document_sort_nosResult>().DoDataBaseOperation())
                    return;
                CallDocumentSortNos = false;
                pgb_sort_nos.XProgress();
            }
            Check_Show_Form_Accounting_Document();
        }
        private void Check_Store_Account()
        {
            if (pgb_check_store.Value != 100)
            {
                var Bll_CheckStore = new BLL<stp_acc_inv_check_account_of_storeResult>();
                if (!Bll_CheckStore.DoDataBaseOperation(new stp_acc_inv_check_account_of_storeResult() { inv_document_no = MaxSelectedNo }))
                {
                    Form_Acc_Ahart_Account = new frm_acc_chart_account();
                    Form_Acc_Ahart_Account.ShowDialog();
                    return;
                }
                //progressbar(pgb_check_store);
                pgb_check_store.XProgress();
            }
            Check_Distination_Account();
        }
        private void Check_Distination_Account()
        {
            if (pgb_check_distination.Value != 100)
            {
                var Bll_CheckDistination = new BLL<stp_acc_inv_check_account_of_destinationResult>();
                if (!Bll_CheckDistination.DoDataBaseOperation(new stp_acc_inv_check_account_of_destinationResult() { inv_document_no = MaxSelectedNo }))
                {
                    Form_Acc_Ahart_Account = new frm_acc_chart_account();
                    Form_Acc_Ahart_Account.ShowDialog();
                    return;
                }
                //progressbar(pgb_check_distination);
                pgb_check_distination.XProgress();
            }
            Calculate_Price_Of_Receive_And_Send();
        }
        private void Calculate_Price_Of_Receive_And_Send()
        {
            if (pgb_calculate_price_of_receive_and_sen.Value != 100)
            {
                var Bll_CalculatePriceOfReceiveAndSend = new BLL<stp_acc_inv_calculate_price_of_sends_and_receivesResult>();
                if (!Bll_CalculatePriceOfReceiveAndSend.DoDataBaseOperation(new stp_acc_inv_calculate_price_of_sends_and_receivesResult() { MaxNo = MaxSelectedNo, Call_document_sort_nos = CallDocumentSortNos }))
                    return;
                //progressbar(pgb_calculate_price_of_receive_and_sen);
                pgb_calculate_price_of_receive_and_sen.XProgress();
            }
            Acc_Document_Show();
        }
        private void Acc_Document_Show()
        {
            //جهت صدور سند حسابداری برای انبار
            stp_acc_document_selResult DocumentRecord = new stp_acc_document_selResult();
            stp_acc_document_article_selResult acc_document_article_inv_store = new stp_acc_document_article_selResult();
            stp_acc_document_article_selResult acc_document_article_inv_destination = new stp_acc_document_article_selResult();
            List<stp_acc_document_article_selResult> LstDocumentArticle = new List<stp_acc_document_article_selResult>();
            DocumentRecord.acc_document_description = " سند حسابداری اسناد انبار تا تاریخ " + selectedRecord.acc_document_date;
            DocumentRecord.acc_document_from_inventory = true;
            List<stp_inv_store_selResult> lstStore = new List<stp_inv_store_selResult>();
            stp_inv_store_selResult invStore = new stp_inv_store_selResult();
            lstStore = new BLL<stp_inv_store_selResult>().GetAllRecords_DB();
            stp_acc_chart_account_selResult accChartAccountInvStore = new stp_acc_chart_account_selResult();
            stp_acc_chart_account_treResult accChartAccount = new stp_acc_chart_account_treResult();
            stp_acc_chart_account_selResult accChartAccountInvDestination = new stp_acc_chart_account_selResult();
            List<stp_acc_chart_account_treResult> lstAccChartAccount = new List<stp_acc_chart_account_treResult>();
            lstAccChartAccount = new BLL<stp_acc_chart_account_treResult>().GetAllRecords_DB();
            long inv_destination_id;
            foreach (stp_inv_goods_receive_inv_goods_send_selResult record in ref_receive_send.ListBeforeChange)
            {
                invStore = lstStore.Find(inv_store => inv_store.inv_store_id == record.inv_goods_receive_inv_goods_send_inv_store_id);
                inv_destination_id = record.inv_goods_receive_inv_goods_send_destination_acc_detail_id;

                accChartAccountInvStore.acc_chart_account_acc_detail_id = invStore != null ? invStore.inv_store_acc_detail_id : 0;
                accChartAccount = lstAccChartAccount.FindLast(acc_detail_id => acc_detail_id.acc_chart_account_acc_detail_id == accChartAccountInvStore.acc_chart_account_acc_detail_id);
                acc_document_article_inv_store = new stp_acc_document_article_selResult();
                acc_document_article_inv_store.acc_document_article_acc_chart_account_id = accChartAccount != null ? accChartAccount.acc_chart_account_id : 0;
                acc_document_article_inv_store.acc_document_article_acc_chart_account_name = accChartAccount != null ? accChartAccount.acc_chart_account_name : "";
                acc_document_article_inv_store.acc_document_article_acc_chart_account_code = accChartAccount != null ? accChartAccount.acc_chart_account_code : "";
                acc_document_article_inv_store.acc_document_article_acc_chart_account_glb_entity_type_name = accChartAccount != null ? accChartAccount.acc_chart_account_glb_entity_type_name : "";
                acc_document_article_inv_store.acc_document_article_acc_detail_id = accChartAccount != null ? accChartAccount.acc_chart_account_acc_detail_id : null;
                acc_document_article_inv_store.acc_document_article_acc_chart_account_parent_names = accChartAccount.acc_chart_account_parent_names;
                //acc_document_article_inv_store.acc_document_article_acc_chart_account_group_name = accChartAccount.acc_chart_account_group_name;
                //acc_document_article_inv_store.acc_document_article_acc_chart_account_kol_name = accChartAccount.acc_chart_account_kol_name;
                //acc_document_article_inv_store.acc_document_article_acc_chart_account_moein_name = accChartAccount.acc_chart_account_moein_name;
                //acc_document_article_inv_store.acc_document_article_acc_chart_account_detail_name = accChartAccount.acc_chart_account_detail_name;
                acc_document_article_inv_store.acc_document_article_description = "جهت صدور سند مربوط به انبار";
                LstDocumentArticle.Add(acc_document_article_inv_store);

                accChartAccountInvDestination.acc_chart_account_acc_detail_id = inv_destination_id;
                accChartAccount = lstAccChartAccount.FindLast(acc_detail_id => acc_detail_id.acc_chart_account_acc_detail_id == accChartAccountInvDestination.acc_chart_account_acc_detail_id);
                acc_document_article_inv_destination = new stp_acc_document_article_selResult();
                acc_document_article_inv_destination.acc_document_article_acc_chart_account_id = accChartAccount != null ? accChartAccount.acc_chart_account_id : 0;
                acc_document_article_inv_destination.acc_document_article_acc_chart_account_name = accChartAccount != null ? accChartAccount.acc_chart_account_name : "";
                acc_document_article_inv_destination.acc_document_article_acc_chart_account_code = accChartAccount != null ? accChartAccount.acc_chart_account_code : "";
                acc_document_article_inv_destination.acc_document_article_acc_chart_account_glb_entity_type_name = accChartAccount != null ? accChartAccount.acc_chart_account_glb_entity_type_name : "";
                acc_document_article_inv_destination.acc_document_article_description = "جهت صدور سند مربوط به انبار";
                acc_document_article_inv_destination.acc_document_article_acc_detail_id = accChartAccount != null ? accChartAccount.acc_chart_account_acc_detail_id : null;
                acc_document_article_inv_destination.acc_document_article_acc_chart_account_parent_names = accChartAccount.acc_chart_account_parent_names;
                //acc_document_article_inv_destination.acc_document_article_acc_chart_account_group_name = accChartAccount.acc_chart_account_group_name;
                //acc_document_article_inv_destination.acc_document_article_acc_chart_account_kol_name = accChartAccount.acc_chart_account_kol_name;
                //acc_document_article_inv_destination.acc_document_article_acc_chart_account_moein_name = accChartAccount.acc_chart_account_moein_name;
                //acc_document_article_inv_destination.acc_document_article_acc_chart_account_detail_name = accChartAccount.acc_chart_account_detail_name;

                if (record.inv_goods_receive_inv_goods_send_is_receive == true)
                {
                    acc_document_article_inv_destination.acc_document_article_credit = (double)(Convert.ToInt64(record.inv_goods_receive_inv_goods_send_sum_price));
                    acc_document_article_inv_store.acc_document_article_debt = (double)(Convert.ToInt64(record.inv_goods_receive_inv_goods_send_sum_price));
                    acc_document_article_inv_store.acc_document_article_inv_goods_receive_id = record.inv_goods_receive_inv_goods_send_id;
                    acc_document_article_inv_destination.acc_document_article_inv_goods_receive_id = record.inv_goods_receive_inv_goods_send_id;
                    acc_document_article_inv_store.acc_document_article_inventory_information = "رسید شماره   " + record.inv_goods_receive_inv_goods_send_no + "  از  " +  acc_document_article_inv_store.acc_document_article_acc_chart_account_name;
                    acc_document_article_inv_destination.acc_document_article_inventory_information = "رسید شماره   " + record.inv_goods_receive_inv_goods_send_no + "  از  "+ acc_document_article_inv_store.acc_document_article_acc_chart_account_name;
                }
                else
                {
                    acc_document_article_inv_destination.acc_document_article_debt = (double)((long)record.inv_goods_receive_inv_goods_send_sum_price);
                    acc_document_article_inv_store.acc_document_article_credit = (double)((long)record.inv_goods_receive_inv_goods_send_sum_price);
                    acc_document_article_inv_store.acc_document_article_inv_goods_send_id = record.inv_goods_receive_inv_goods_send_id;
                    acc_document_article_inv_destination.acc_document_article_inv_goods_send_id = record.inv_goods_receive_inv_goods_send_id;
                    acc_document_article_inv_store.acc_document_article_inventory_information = "حواله شماره   " + record.inv_goods_receive_inv_goods_send_no + "  از  "  + acc_document_article_inv_store.acc_document_article_acc_chart_account_name;
                    acc_document_article_inv_destination.acc_document_article_inventory_information = "حواله شماره   " + record.inv_goods_receive_inv_goods_send_no + "  از  "  + acc_document_article_inv_store.acc_document_article_acc_chart_account_name;
                }

                LstDocumentArticle.Add(acc_document_article_inv_destination);
            }

            if (LstDocumentArticle.Count != 0)
            {
                Form_Acc_Document = new frm_acc_document();
                if (Form_Acc_Document.CreateDocument(DocumentRecord, LstDocumentArticle))
                    ref_receive_send.ListAfterChange.Clear();
            }
        }
       
        private void Check_Show_Form_Accounting_Document()
        {
            if (ref_receive_send.ListAfterChange.Count == 0)
                brw_select_inv_document_XBrowseClick(null, null);
            else
                Check_Store_Account();
        }
        #endregion
    }
}
