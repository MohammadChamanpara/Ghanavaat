using UserInterfaceLayer;
using APMTools;
using DataAccessLayer;
using APM_SubSystems;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Documents;
using System.Collections.Generic;
using APM_Accounting;
using BusinessLogicLayer;
using APM_SubSystems.Sahaam.gnt_service;

namespace APM_SubSystems
{
    public partial class frm_gnt_service : WindowTwoTabs<stp_gnt_service_selResult, stp_gnt_service_article_selResult>
    {
        #region Variables
        stp_gnt_creditor_selResult CurrentCreditor { get; set; }
        #endregion

        #region Constructor
        public frm_gnt_service(stp_gnt_creditor_selResult currentCreditor)
        {
            this.CurrentCreditor = currentCreditor;
            InitializeComponent();
            Initial_WindowTwoTab(documentHeader, null, null, null, null, null, dbg_gnt_service, dbg_gnt_service_article, tbr_buy_request, documentHeader, grp_article_current_row, "gnt_service", "gnt_service_article", tab_main, txt_gnt_service_article_description, null, null, null);
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.RecordParameter.gnt_service_gnt_creditor_id = this.CurrentCreditor.gnt_creditor_id;
            lbl_creditor.Content = this.CurrentCreditor.gnt_creditor_name;
            base.Window_Loaded(sender, e);
            if (allRecords.Count == 0)
                ShowSomeRecords(RecordParameter);
            var dataBase = DDB.NewContext();
            if (dataBase.tbl_gnt_settings.Count() == 0)
            {
                Messages.ErrorMessage("تنظیمات سهام وارد نشده است");
                return;
            }
            CalculateTotal();
        }
        public override void RefreshClick()
        {
            base.RefreshClick();
            CalculateTotal();
        }
        public override bool ValidationForSave()
        {
            selectedRecord.gnt_service_gnt_creditor_id = this.CurrentCreditor.gnt_creditor_id;
            CalculateTotalPrice(selectedArticle);
            return base.ValidationForSave();
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            CalculateTotal();
            UpdateCreditorAccount();
            foreach (var article in bindingListArticle)
                article.gnt_service_article_count_string = article.gnt_service_article_count.ToString();
        }
        public override void OperationsAfterDelete()
        {
            base.OperationsAfterDelete();
            DeleteFromCreditorAccount();
        }
        public override bool ValidationForDelete()
        {
            Messages.WarningMessage("با حذف این سند مبلغ سند از حساب سهامدار حذف خواهد شد");
            return base.ValidationForDelete();
        }
        public override void PrintClick()
        {
            string address = "...";
            try
            {
                var db = DDB.NewContext();
                var list = db.tbl_gnt_earth.Where(x => x.gnt_earth_gnt_creditor_id == CurrentCreditor.gnt_creditor_id);
                if (list.Count() > 0)
                {
                    var earth = list.First();
                    address = "خیابان {0} ، ردیف {1} ، بلوک {2} ، پلاک {3}".FormatWith(earth.gnt_earth_street, earth.gnt_earth_line, earth.gnt_earth_block, earth.gnt_earth_plaque);
                }
            }
            catch
            {
                address = "---";
            }
            var parameterForm = new frm_gnt_service_report_parameters(this.CurrentCreditor.gnt_creditor_name, address);
            if (parameterForm.ShowDialog() != true)
                return;
            var sentence = parameterForm.Sentence;

            var reportFile = new gnt_rpt_service();
            var printForm = new WindowPrint<tbl_gnt_creditor, stp_gnt_service_article_selResult>(reportFile);
            printForm.articleList = bindingListArticle.ToList();
            printForm.selectedRecord = this.CurrentCreditor.ToEntity();
            printForm.AddCustomParameter("gnt_service_code", selectedRecord.gnt_service_code);
            printForm.AddCustomParameter("gnt_service_date", selectedRecord.gnt_service_date);
            printForm.AddCustomParameter("below_sentence", sentence);
            printForm.ShowDialog();
        }
        public override bool ChangeSelectedRecord(stp_gnt_service_selResult newSelectedRecord, object caller)
        {
            var r = base.ChangeSelectedRecord(newSelectedRecord, caller);
            CalculateTotal();
            return r;
        }
        #endregion

        #region Events
        private void brw_gnt_service_article_gnt_cost_type_XBrowseClick(object sender, RoutedEventArgs e)
        {
            stp_gnt_cost_type_selResult recordParameter = new stp_gnt_cost_type_selResult() { gnt_cost_type_glb_fiscal_year_id = GlobalVariables.current_fiscal_year_id };
            var service_articleType = BrowseClick_Parameter(new WindowSelectGrid<stp_gnt_cost_type_selResult>(), selectedArticle, recordParameter, "نوع هزینه", typeof(frm_gnt_cost_type), sender);
            selectedArticle.gnt_service_article_unit_price = service_articleType.gnt_cost_type_price;
            MoveCollectionViewArticle();
        }
        private void brw_gnt_service_article_gnt_creditor_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            var creditor = BrowseClick(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), selectedArticle, "سهامدار", typeof(frm_gnt_creditor), sender);
        }
        private void txt_gnt_service_article_count_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateTotalPrice(selectedArticle);
            MoveCollectionViewArticle();
        }
        #endregion

        #region Methods
        private void UpdateCreditorAccount()
        {
            try
            {
                Boolean oldRecordDeleted = false;
                var db = DDB.NewContext();
                var accountRecords = db.tbl_gnt_creditor_account.Where(x => x.gnt_creditor_account_gnt_service_id == selectedRecord.gnt_service_id);
                if (accountRecords.Count() > 0)
                {
                    oldRecordDeleted = true;
                    foreach (var record in accountRecords)
                        db.tbl_gnt_creditor_account.DeleteObject(record);
                }
                db.tbl_gnt_creditor_account.AddObject(new tbl_gnt_creditor_account()
                {
                    gnt_creditor_account_gnt_service_id = selectedRecord.gnt_service_id,
                    gnt_creditor_account_date = APMDateTime.dateWithNoSlash(selectedRecord.gnt_service_date),
                    gnt_creditor_account_credit = 0,
                    gnt_creditor_account_debt = bindingListArticle.Sum(x => x.gnt_service_article_total_price),
                    gnt_creditor_account_gnt_creditor_id = CurrentCreditor.gnt_creditor_id,
                    gnt_creditor_account_title = string.Format("خدمات خصوصی به شماره سند {0}", selectedRecord.gnt_service_code),
                    gnt_creditor_account_description = selectedRecord.gnt_service_description,
                    gnt_creditor_account_is_public_cost = false,
                    gnt_creditor_account_glb_fiscal_year_id = GlobalVariables.current_fiscal_year_id,
                    gnt_creditor_account_is_opening = false
                });
                db.SaveChanges();
                string message = "";
                if (oldRecordDeleted)
                    message = "مبلغ سند قبلی از حساب سهامدار حذف و مبلغ سند جدید به حساب سهامدار افزوده شد";
                else
                    message = "مبلغ این سند به حساب سهامدار افزوده شد";
                Messages.InformationMessage(message);
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage("بروز خطا هنگام به روز رسانی حساب سهامدار. متن کامل خطا : \r\n" + exception.CompleteMessages());
            }
        }
        private void DeleteFromCreditorAccount()
        {
            try
            {
                var db = DDB.NewContext();
                var accountRecords = db.tbl_gnt_creditor_account.Where(x => x.gnt_creditor_account_gnt_service_id == selectedRecord.gnt_service_id);
                if (accountRecords.Count() > 0)
                {
                    foreach (var record in accountRecords)
                        db.tbl_gnt_creditor_account.DeleteObject(record);
                    db.SaveChanges();
                    Messages.InformationMessage("مبلغ این سند از حساب سهامدار کسر شد");
                }
                else
                    Messages.WarningMessage("این سند در حساب سهامدار یافت نشد");
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage("بروز خطا هنگام به روز رسانی حساب سهامدار. متن کامل خطا : \r\n" + exception.CompleteMessages());
            }
        }
        private void CalculateTotal()
        {
            var sum = bindingListArticle.Sum(x => x.gnt_service_article_total_price);
            dbg_gnt_service_article.XTotalRialsContent = sum.DigitGrouping();
        }
        private void CalculateTotalPrice(stp_gnt_service_article_selResult article)
        {
            try
            {
                //if (article.gnt_service_article_unit_price == null || article.gnt_service_article_total_price != 0)
                //    return;
                article.gnt_service_article_total_price = (int)Math.Round(article.gnt_service_article_unit_price.Value * article.gnt_service_article_count);
            }
            catch (Exception exception)
            {
                Messages.ExceptionMessage(exception);
            }

        }
        #endregion
    }
}
