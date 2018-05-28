using System.Windows;
using System.Windows.Controls;
using UserInterfaceLayer;
using DataAccessLayer;
using APMTools;
using BusinessLogicLayer;
using System.Linq;
using System;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using APM_SubSystems.Sahaam.gnt_creditor;

namespace APM_SubSystems
{
    public partial class frm_gnt_creditor_account : WindowBase<stp_gnt_creditor_account_selResult>
    {
        #region Variables
        stp_gnt_creditor_selResult CurrentCreditor { get; set; }
        public ReportClass printReportFile;
        stp_gnt_cost_type_selResult costType = new stp_gnt_cost_type_selResult();
        #endregion

        #region Constructor
        public frm_gnt_creditor_account(stp_gnt_creditor_selResult currentCreditor)
        {
            InitializeComponent();
            this.CurrentCreditor = currentCreditor;
            this.printReportFile = new gnt_rpt_creditor_account();
            Initial_WindowBase(dbg_account, windowToolbar, grpInfo, "gnt_creditor_account", true, null);
        }
        #endregion

        #region Override
        public override void PrintClick()
        {
            try
            {
                if (printReportFile == null)
                    return;
                var printForm = new WindowPrint<tbl_gnt_creditor, stp_gnt_creditor_account_selResult>(printReportFile);
                printForm.articleList = allRecords;
                printForm.selectedRecord = this.CurrentCreditor.ToEntity();
                printForm.ShowDialog();
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.RecordParameter.gnt_creditor_account_gnt_creditor_id = this.CurrentCreditor.gnt_creditor_id;
            base.Window_Loaded(sender, e);
            lbl_creditor_name.Content = this.CurrentCreditor.gnt_creditor_name;
            CalculateTotal();
            var dataBase = DDB.NewContext();
            if (dataBase.tbl_gnt_settings.Count() == 0)
            {
                Messages.ErrorMessage("تنظیمات سیستم سهام وارد نشده اند");
                return;
            }
        }
        public override bool ValidationForSave()
        {
            selectedRecord.gnt_creditor_account_gnt_creditor_id = this.CurrentCreditor.gnt_creditor_id;
            return base.ValidationForSave();
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            CalculateTotal();
        }
        public override void RefreshClick()
        {
            base.RefreshClick();
            CalculateTotal();
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            pdp_gnt_creditor_account_date.Text = APMDateTime.Today;
        }
        #endregion

        #region Events
        private void showThisServiceDocument_Click(object sender, RoutedEventArgs e)
        {
            long? serviceId = selectedRecord.gnt_creditor_account_gnt_service_id;
            if (!serviceId.HasValue)
            {
                Messages.ErrorMessage("لطفاً سطر مربوط به سند خدمات را انتخاب کنید.");
                return;
            }
            new frm_gnt_service(CurrentCreditor).ShowOneDocument(serviceId.Value);
            RefreshClick();
        }
        private void btnShowServices_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_service(CurrentCreditor).ShowDialog();
            RefreshClick();
        }
        private void printMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrintClick();
        }
        private void PrintPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRecord.gnt_creditor_account_credit == 0)
                {
                    Messages.InformationMessage("لطفاً یک سطر پرداخت را انتخاب نمائید");
                    return;
                }
                var reportDocument = new gnt_rpt_creditor_payment();
                var printForm = new WindowPrint<tbl_gnt_creditor, stp_gnt_creditor_account_selResult>(reportDocument);
                string sentence = string.Format("گواهی می شود سهامدار فوق مبلغ {0} ریال طی فیش شماره {1} بابت بدهی های زیر پرداخت نموده است.", selectedRecord.gnt_creditor_account_credit.DigitGrouping(), selectedRecord.gnt_creditor_payment_receipt_no);
                printForm.AddCustomParameter("payment_sentence", sentence);
                printForm.AddCustomParameter("payment_date", selectedRecord.gnt_creditor_account_date);
                printForm.articleList = allRecords;
                printForm.selectedRecord = this.CurrentCreditor.ToEntity();
                printForm.ShowDialog();
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }
        private void txt_debt_credit_TextChange(object sender, TextChangedEventArgs e)
        {
            SetReceiptVisibility(selectedRecord);
        }
        private void txt_debt_credit_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (operationType == OperationType.Nothing)
                return;
            if (sender == txt_gnt_creditor_account_debt)
                txt_gnt_creditor_account_credit.Text = "0";
            else
                txt_gnt_creditor_account_debt.Text = "0";
        }
        private void UpdateAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Messages.QuestionMessage_YesNo("آیا مایلید بدهی سهامدار برای هزینه های عمومی حذف و مجدداً محاسبه شود؟") != MessageBoxResult.Yes)
                    return;
                CurrentCreditor.ToEntity().UpdateAccountForPublicCosts();
                RefreshClick();
                Messages.InformationMessage("عملیات با موفقیت انجام شد.");
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }
        #endregion

        #region Methods
        private void CalculateTotal()
        {
            var sumCredit = bindingList.Sum(x => x.gnt_creditor_account_credit);
            var sumDebt = bindingList.Sum(x => x.gnt_creditor_account_debt);
            dbg_account.XTotalCreditContent = sumCredit.DigitGrouping();
            dbg_account.XTotalDebtContent = sumDebt.DigitGrouping();
            dbg_account.XRemainingContent = (sumDebt - sumCredit).DigitGrouping();
        }
        public override bool ChangeSelectedRecord(stp_gnt_creditor_account_selResult newSelectedRecord, object caller)
        {
            SetReceiptVisibility(newSelectedRecord);
            return base.ChangeSelectedRecord(newSelectedRecord, caller);
        }
        private void SetReceiptVisibility(stp_gnt_creditor_account_selResult selectedRecord)
        {
            stkReceipt.Visibility = (selectedRecord.gnt_creditor_account_credit > 0) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }
}
