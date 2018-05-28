using System.Windows;
using System.Windows.Input;
using System.Linq;
using System;
using DataAccessLayer;
using UserInterfaceLayer;
using APM_SubSystems;
using System.Windows.Controls;
using APMTools;
using BusinessLogicLayer;
using APMComponents;
using System.Collections.Generic;

namespace APM_Accounting
{
    public partial class frm_acc_document : WindowTwoTabs<stp_acc_document_selResult, stp_acc_document_article_selResult>
    {
        #region Variables
        private stp_acc_document_article_selResult AccDocumentArticle = new stp_acc_document_article_selResult();
        private double creditCount = 0, debtCount = 0;
        #endregion

        #region Constructor
        public frm_acc_document()
        {
            InitializeComponent();
            Initial_WindowTwoTab(documentHeader, null, lbl_countTitle, null, null, null, dbg_acc_document, dbg_acc_document_article, tbr_acc_document, documentHeader, grp_atticle, "acc_document", "acc_document_article", tab_acc_document, txt_acc_document_article_description, null, null, new APM_SubSystems.APM_Accounting.acc_document.rpt_acc_document());
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new BLL<stp_acc_document_type_selResult>().FillComboBox(documentHeader.cmbAccountingDocumentType, bindingList);
            base.Window_Loaded(sender, e);

        }
        public override void SetVisibilityOrEnableOfControlsBasedOnSelectedRecord()
        {
            base.SetVisibilityOrEnableOfControlsBasedOnSelectedRecord();
            dbg_acc_document_article.datagrid.Columns[2].Visibility = GlobalFunctions.BooleanToVisibility(selectedRecord.acc_document_from_inventory);
            ctm_acc_document_article.Visibility = GlobalFunctions.BooleanToVisibility(selectedRecord.acc_document_from_inventory);
            Boolean enable = operationType != OperationType.Nothing && !selectedRecord.acc_document_from_inventory;
            txt_acc_document_article_credit.IsEnabled = enable;
            txt_acc_document_article_debt.IsEnabled = enable;
            SetEnableToolBarButton();
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            selectedRecord.acc_document_status_glb_coding_id = (long)AccDocumentStatus.Note;
            SetEnableToolBarButton();
            SetEnablesForArticleGroupBox();
        }
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            SetEnableToolBarButton();
        }
        public override void CalculateGridTotalFields(APMDataGrid DataGrid)
        {
            base.CalculateGridTotalFields(DataGrid);
            creditCount = 0;
            debtCount = 0;
            stp_acc_document_article_selResult documentArticle = new stp_acc_document_article_selResult();
            for (int i = 0; i < bindingListArticle.Count; i++)
            {
                documentArticle = bindingListArticle[i];
                debtCount = documentArticle.acc_document_article_debt + debtCount;
                creditCount = documentArticle.acc_document_article_credit + creditCount;
            }

            selectedRecord.acc_document_sum_credit = creditCount;
            selectedRecord.acc_document_sum_debt = debtCount;
            selectedRecord.acc_document_remaining = Math.Abs(creditCount - debtCount);
        }
        #endregion

        #region APMToolBarButtonClick
        public override void SaveNoteClick()
        {
            CalculateGridTotalFields(dataGrid);
            selectedRecord.acc_document_status_glb_coding_id = (long)AccDocumentStatus.Note;
            selectedRecord.acc_document_no = null;
            RemoveEmptyArticle();
            SaveClick(true);
        }
        public override void SaveTempClick()
        {
            if (selectedRecord.acc_document_sum_debt != selectedRecord.acc_document_sum_credit)
            {
                Messages.ErrorMessage("به دلیل تراز نبودن آرتیکل ها شما قادر به ثبت موقت نمی باشید");
                return;
            }
            selectedRecord.acc_document_status_glb_coding_id = (long)AccDocumentStatus.Temporary;
            RemoveEmptyArticle();
            SaveClick(true);
        }
        public override void UseLessClick()
        {
            selectedRecord.acc_document_status_glb_coding_id = (long)AccDocumentStatus.UseLess;
            selectedRecord.acc_document_no = null;
            if (BLL.SaveRecord(selectedRecord, OperationType.Update, false) == SaveResult.Saved)
            {
                Messages.SuccessMessage("باطل شدن سند");

                MoveCollectionView();
                dbg_acc_document.datagrid.Items.Refresh();
                SetEnableToolBarButton();
            }
        }
        public override void UndoUseLessClick()
        {
            selectedRecord.acc_document_status_glb_coding_id = (long)AccDocumentStatus.Temporary;
            if (BLL.SaveRecord(selectedRecord, OperationType.Update, false) == SaveResult.Saved)
            {
                Messages.SuccessMessage("برکشت از ابطالی سند");
                MoveCollectionView();
                dbg_acc_document.datagrid.Items.Refresh();
                SetEnableToolBarButton();
            }
        }
        public override void ConfirmClick()
        {
            selectedRecord.acc_document_status_glb_coding_id = (long)AccDocumentStatus.Confirm;
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<stp_acc_document_selResult>.ConfirmDate, APMDateTime.Today);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<stp_acc_document_selResult>.ConfirmTime, APMDateTime.SystemTime);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<stp_acc_document_selResult>.ConfirmerUserId, GlobalVariables.current_user_id);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<stp_acc_document_selResult>.ConfirmerUserName, GlobalVariables.current_user_name);
            if (BLL.SaveRecord(selectedRecord, OperationType.Update, false) == SaveResult.Saved)
            {
                Messages.SuccessMessage("قطعی شدن سند");
                MoveCollectionView();
                dbg_acc_document.datagrid.Items.Refresh();
                SetEnableToolBarButton();
            }
        }
        public override void UndoConfirmClick()
        {
            selectedRecord.acc_document_status_glb_coding_id = (long)AccDocumentStatus.Temporary;
            selectedRecord.acc_document_confirm_date = null;
            selectedRecord.acc_document_confirm_time = null;
            selectedRecord.acc_document_confirmer_glb_user_id = null;
            selectedRecord.acc_document_confirmer_glb_user_name = null;
            if (BLL.SaveRecord(selectedRecord, OperationType.Update, false) == SaveResult.Saved)
            {
                Messages.SuccessMessage("برگشت از قطعی سند");
                MoveCollectionView();
                dbg_acc_document.datagrid.Items.Refresh();
                SetEnableToolBarButton();
            }
        }
        public override void InsertArticleClick()
        {
            base.InsertArticleClick();
            if (bindingListArticle.Count > 1)
            {
                GlobalFunctions.CopyRecord(selectedArticle, bindingListArticle[bindingListArticle.Count - 2]);
                selectedArticle.acc_document_article_id = 0;
                selectedArticle.acc_document_article_credit = 0;
                selectedArticle.acc_document_article_debt = 0;
            }
            MinusDebtCredit(creditCount, debtCount);
            MoveCollectionViewArticle();
        }
        #endregion

        #region XBrowseClick
        private void brw_acc_document_article_acc_chart_account_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            if (selectedRecord.acc_document_from_inventory)
                BrowseClick_Parameter(new WindowSelectGrid<stp_acc_chart_account_selResult>(), selectedArticle,
                    new stp_acc_chart_account_selResult() { acc_chart_account_acc_detail_id = selectedArticle.acc_document_article_acc_detail_id },
                    "حساب برای صدور سند حسابداری", typeof(frm_acc_chart_account), sender);
            else if (radSelectFromGrid.IsChecked == true)
                BrowseClick(new WindowSelectGrid<stp_acc_chart_account_grdResult>(), selectedArticle, "حساب برای صدور سند حسابداری", typeof(frm_acc_chart_account), sender);
            else
                BrowseClick(new WindowSelectTree<stp_acc_chart_account_treResult>(TreeType.SingleSelect_LastEntity, "کدینگ حسابداری"), selectedArticle, "حساب برای صدور سند حسابداری", typeof(frm_acc_chart_account), sender);

        }
        #endregion

        #region XTextBoxKeyDown
        private void brw_acc_document_article_acc_chart_account_id_XTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_acc_chart_account_treResult, stp_acc_document_article_selResult>(sender, "acc_document_article_acc_chart_account_id", e, selectedArticle);
        }
        #endregion

        #region Events
        private void txt_acc_document_article_debt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_acc_document_article_debt.Text != ("0") && txt_acc_document_article_credit.Text != ("0"))
                txt_acc_document_article_credit.Text = "0";
        }
        private void txt_acc_document_article_credit_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_acc_document_article_credit.Text != ("0") && txt_acc_document_article_debt.Text != ("0"))
                txt_acc_document_article_debt.Text = "0";
        }
        private void ShowInvDocument_MenuClick(object sender, RoutedEventArgs e)
        {
            if (selectedArticle.acc_document_article_inv_goods_receive_id != null)
                new frm_inv_goods_receive(true, false).ShowOneDocument(selectedArticle.acc_document_article_inv_goods_receive_id.Value);
            else if (selectedArticle.acc_document_article_inv_goods_send_id != null)
                new frm_inv_goods_send(true, false).ShowOneDocument(selectedArticle.acc_document_article_inv_goods_send_id.Value);
        }
        private void ShowAccountReport_MenuClick(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_account_balance().CustomReport(
                new stp_acc_rpt_account_balance_selResult()
                {
                    acc_rpt_account_balance_acc_chart_account_id = selectedArticle.acc_document_article_acc_chart_account_id,
                    acc_rpt_account_balance_acc_chart_account_name = selectedArticle.acc_document_article_acc_chart_account_name,
                    acc_rpt_account_balance_acc_chart_account_code = selectedArticle.acc_document_article_acc_chart_account_code
                });
        }
        private void ShowDetailReport_MenuClick(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_account_balance().CustomReport(
                new stp_acc_rpt_account_balance_selResult()
                {
                    acc_rpt_account_balance_acc_detail_id = selectedArticle.acc_document_article_acc_detail_id,
                    acc_rpt_account_balance_acc_detail_name = selectedArticle.acc_document_article_acc_detail_name,
                    acc_rpt_account_balance_acc_detail_code = selectedArticle.acc_document_article_acc_detail_code,
                });
        }
        #endregion

        #region Tools
        private void SetEnableToolBarButton()
        {
            long accDocumentStatus = selectedRecord.acc_document_status_glb_coding_id;
            tbr_acc_document.XUndoConfirmButton.IsEnabled = operationType == OperationType.Nothing && (accDocumentStatus == (long)AccDocumentStatus.Confirm);
            tbr_acc_document.XUndoUseLessButton.IsEnabled = operationType == OperationType.Nothing && accDocumentStatus == (long)AccDocumentStatus.UseLess ? true : false;
            tbr_acc_document.XConfirmButton.IsEnabled = operationType == OperationType.Nothing && !(tbr_acc_document.XUndoConfirmButton.IsEnabled || accDocumentStatus == (long)AccDocumentStatus.Note || accDocumentStatus == (long)AccDocumentStatus.UseLess);
            tbr_acc_document.XUseLessButton.IsEnabled = operationType == OperationType.Nothing && !(tbr_acc_document.XUndoUseLessButton.IsEnabled || accDocumentStatus == (long)AccDocumentStatus.Note || accDocumentStatus == (long)AccDocumentStatus.Confirm);
            tbr_acc_document.XEditButton.IsEnabled = operationType == OperationType.Nothing && (accDocumentStatus == (long)AccDocumentStatus.Note || accDocumentStatus == (long)AccDocumentStatus.Temporary);
            tbr_acc_document.XDeleteButton.IsEnabled = operationType == OperationType.Nothing && (accDocumentStatus == (long)AccDocumentStatus.Note || accDocumentStatus == (long)AccDocumentStatus.UseLess);
        }
        public void MinusDebtCredit(double credit, double debt)
        {
            var x = Math.Abs(creditCount - debtCount);
            if (creditCount < debtCount)
            {
                txt_acc_document_article_debt.Text = "0";
                txt_acc_document_article_credit.Text = x.ToString();
            }
            else if (creditCount > debtCount)
            {
                txt_acc_document_article_credit.Text = "0";
                txt_acc_document_article_debt.Text = x.ToString();
            }
        }
        private void RemoveEmptyArticle()
        {
            if (bindingListArticle.Count > 1)
            {
                var lastArticle = bindingListArticle[bindingListArticle.Count - 1];
                if
                (
                    lastArticle.acc_document_article_acc_chart_account_id == 0
                )
                    bindingListArticle.RemoveAt(bindingListArticle.Count - 1);
            }
        }
        #endregion
    }
}
