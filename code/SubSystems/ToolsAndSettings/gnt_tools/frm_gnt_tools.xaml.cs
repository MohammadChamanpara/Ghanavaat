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

    public partial class frm_gnt_tools : WindowBase<stp_acc_document_selResult>
    {
        #region Variable
        private ArticlePackage<stp_gnt_creditor_selResult, stp_gnt_creditor_selResult> creditorsArticlePackage =
                    new WindowBase<stp_acc_document_selResult>.ArticlePackage<stp_gnt_creditor_selResult, stp_gnt_creditor_selResult>();
        int selectedCreditorsCount = 0;
        #endregion

        #region Constructor
        public frm_gnt_tools()
        {
            GlobalVariables.currentSubSystem = SubSystems.Inventory;
            InitializeComponent();
        }
        #endregion

        #region Events
        private void brwCreditors_XBrowseClick(object sender, RoutedEventArgs e)
        {
            chkSelectAllCreditors.IsChecked = false;
            BrowseClick_MultiSelect(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), ref creditorsArticlePackage, "سهامدار", typeof(frm_gnt_creditor));
            selectedCreditorsCount = creditorsArticlePackage.ListAfterChange.Count;
            lbl_selected_creditors.Content = selectedCreditorsCount;
        }
        private void chkSelectAllCreditors_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkSelectAllCreditors.IsChecked == true)
                    selectedCreditorsCount = DDB.NewContext().tbl_gnt_creditor.Count();
                else
                    selectedCreditorsCount = 0;
                lbl_selected_creditors.Content = selectedCreditorsCount;
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }
        private void btnUpdateCreditorAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedCreditorsCount == 0)
                {
                    Messages.ErrorMessage("سهامداران انتخاب نشده اند");
                    return;
                }
                SahaamEntities db = DDB.NewContext();
                if (chkSelectAllCreditors.IsChecked == true)
                    foreach (var creditor in db.tbl_gnt_creditor.ToArray())
                        creditor.UpdateAccountForPublicCosts();
                else
                    foreach (var oldcreditor in creditorsArticlePackage.ListAfterChange)
                        oldcreditor.ToEntity().UpdateAccountForPublicCosts();

                Messages.SuccessMessage(string.Format("اعمال هزینه های عمومی در حساب {0} سهامدر", (selectedCreditorsCount == 1) ? "یک" : selectedCreditorsCount.ToString()));
                selectedCreditorsCount = 0;
                lbl_selected_creditors.Content = selectedCreditorsCount;
                chkSelectAllCreditors.IsChecked = false;
                creditorsArticlePackage.Clear();
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }

        }



        private void btnCreateAccountingDocument_Click(object sender, RoutedEventArgs e)
        {
            //if (servicesArticlePackage.ListAfterChange.Count == 0)
            //{
            //    Messages.ErrorMessage("اسناد حساب سهامداران انتخاب نشده اند");
            //    return;
            //}
            //SahaamEntities db = DDB.NewContext();

            //stp_acc_document_selResult accountingDocument = new stp_acc_document_selResult()
            //{
            //    acc_document_description = "سند حسابداری مربوط به اسناد حساب سهامدار تا تاریخ " + pdpDate.Text,
            //    acc_document_glb_fiscal_year_id = GlobalVariables.current_fiscal_year_id
            //};
            //if (db.tbl_acc_document_type.Count() > 0)
            //{
            //    var type = db.tbl_acc_document_type.First();
            //    GlobalFunctions.Copy_PK_To_FK(accountingDocument, type);
            //}

            //List<stp_acc_document_article_selResult> articles = new List<stp_acc_document_article_selResult>();

            //double credit = 0;
            //foreach (var oldSahaamDocument in servicesArticlePackage.ListAfterChange)
            //{
            //    var sahaamDocument = db.tbl_gnt_service.Where(x => x.gnt_service_id == oldSahaamDocument.gnt_service_id).Single();
            //    List<long> processedArticles = new List<long>();
            //    foreach (tbl_gnt_service_article sahaamArticle in sahaamDocument.tbl_gnt_service_article)
            //    {
            //        if (processedArticles.Contains(sahaamArticle.gnt_service_article_gnt_creditor_id))
            //            continue;
            //        processedArticles.Add(sahaamArticle.gnt_service_article_gnt_creditor_id);
            //        var article = new stp_acc_document_article_selResult();
            //        long price = sahaamDocument.tbl_gnt_service_article
            //            .Where(x => x.gnt_service_article_gnt_creditor_id == sahaamArticle.gnt_service_article_gnt_creditor_id)
            //            .Sum(x => x.gnt_service_article_total_price);

            //        article.acc_document_article_debt = price;

            //        var detail = db.tbl_acc_detail.Where(x => x.acc_detail_id == sahaamArticle.tbl_gnt_creditor.gnt_creditor_acc_detail_id).First();

            //        var creditorAccounts = new BLL<stp_acc_chart_account_treResult>().GetAllRecords_DB()
            //            .Where(x => x.acc_chart_account_acc_detail_id == sahaamArticle.tbl_gnt_creditor.gnt_creditor_acc_detail_id);

            //        if (creditorAccounts.Count() == 0)
            //        {
            //            Messages.ErrorMessage(string.Format("برای {0} در سیستم حسابداری حساب تعریف نشده است.",
            //                sahaamArticle.tbl_gnt_creditor.gnt_creditor_name));
            //            return;
            //        }
            //        var creditorAccount = creditorAccounts.First();

            //        GlobalFunctions.Copy_PK_To_FK(article, creditorAccount);
            //        article.acc_document_article_description = string.Format("خدمات انجام شده برای {0} .شماره سند خدمات : {1} ", sahaamArticle.tbl_gnt_creditor.gnt_creditor_name, sahaamDocument.gnt_service_code);
            //        article.acc_document_article_gnt_service_id = sahaamDocument.gnt_service_id;
            //        articles.Add(article);
            //    }
            //    credit += sahaamDocument.tbl_gnt_service_article.Sum(x => x.gnt_service_article_total_price);
            //}
            //var creditArticle = new stp_acc_document_article_selResult();
            //creditArticle.acc_document_article_credit = credit;
            //creditArticle.acc_document_article_description = "آرتیکل تراز";
            //articles.Add(creditArticle);

            //frm_acc_document documentForm = new frm_acc_document();
            //documentForm.CreateDocument(accountingDocument, articles);
            //lbl_selected_documents.Content = "0";
            //servicesArticlePackage.Clear();
        }
        private void btn_TransferCreditorsAccountToNewFiscalYear_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var db = DDB.NewContext();
                var fiscalYears = db.tbl_glb_fiscal_year.OrderByDescending(x => x.glb_fiscal_year_id).ToList();
                var lastFiscalYear = fiscalYears.First();
                if (GlobalVariables.current_fiscal_year_id != lastFiscalYear.glb_fiscal_year_id)
                    throw new Exception(string.Format("سال مالی جاری سال {0} و آخرین سال مالی سال {1} می باشد. لطفا با آخرین سال مالی وارد برنامه شوید.", GlobalVariables.current_fiscal_year_name, lastFiscalYear.glb_fiscal_year_name));
                if (fiscalYears.Count == 1)
                    throw new Exception("برنامه تنها یک سال مالی دارد");

                var oldFiscalYear = fiscalYears[1];
                int fine;
                if (!int.TryParse(txtFine.Text, out fine))
                {
                    Messages.ErrorMessage("میزان جریمه را وارد نمائید");
                    return;
                }

                tbl_gnt_creditor.TransferCreditorAccountsToNewfiscalYear(fine, oldFiscalYear.glb_fiscal_year_id, lastFiscalYear.glb_fiscal_year_id);
                Messages.SuccessMessage("انتقال حساب");
            }
            catch (Exception exception)
            {
                Messages.ExceptionMessage(exception);
            }
        }

        private void btnCopyCosts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = DDB.NewContext();

                var fiscalYears = db.tbl_glb_fiscal_year.OrderByDescending(x => x.glb_fiscal_year_id).ToList();
                var newFiscalYear = fiscalYears.First();
                if (GlobalVariables.current_fiscal_year_id != newFiscalYear.glb_fiscal_year_id)
                    throw new Exception(string.Format("سال مالی جاری سال {0} و آخرین سال مالی سال {1} می باشد. لطفا با آخرین سال مالی وارد برنامه شوید.", GlobalVariables.current_fiscal_year_name, newFiscalYear.glb_fiscal_year_name));
                if (fiscalYears.Count == 1)
                    throw new Exception("برنامه تنها یک سال مالی دارد");
                var oldFiscalYear = fiscalYears[1];

                if (txtIncrease.Text.Trim() == "")
                {
                    Messages.ErrorMessage("لطفا میزان افزایش تعرفه را وارد نمایید ");
                    return;
                }

                var result = Messages.QuestionMessage_YesNo(string.Format("آیا مایلید تعرفه های دوره مالی {0} به تعرفه های دوره مالی {1} اضافه شود؟", oldFiscalYear.glb_fiscal_year_name, newFiscalYear.glb_fiscal_year_name));
                if (result != MessageBoxResult.Yes)
                    return;

                var oldCosts = db.tbl_gnt_cost_type.Where
                (
                    x =>
                    x.gnt_cost_type_glb_fiscal_year_id == oldFiscalYear.glb_fiscal_year_id &&
                    x.gnt_cost_type_glb_branch_id == GlobalVariables.current_branch_id
                );
                int increase = int.Parse(txtIncrease.Text);
                foreach (var oldCost in oldCosts)
                {
                    var newCost = new tbl_gnt_cost_type()
                    {
                        gnt_cost_type_glb_branch_id = oldCost.gnt_cost_type_glb_branch_id,
                        gnt_cost_type_glb_fiscal_year_id = newFiscalYear.glb_fiscal_year_id,
                        gnt_cost_type_is_public = oldCost.gnt_cost_type_is_public,
                        gnt_cost_type_name = oldCost.gnt_cost_type_name,
                        gnt_cost_type_price = (int)Math.Round(oldCost.gnt_cost_type_price * (1 + (double)increase / 100)),
                    };
                    db.tbl_gnt_cost_type.AddObject(newCost);
                }
                db.SaveChanges();
                Messages.SuccessMessage("انتقال تعرفه ها");
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }
        #endregion


    }
}
