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

    public partial class frm_acc_tools : WindowBase<stp_acc_document_selResult>
    {
        #region Constructor
        public frm_acc_tools()
        {
            GlobalVariables.currentSubSystem = SubSystems.Accounting;
            InitializeComponent();
        }
        #endregion

        #region Events

        private void btn_TransferCreditorsAccountToNewFiscalYear_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                CreateClosingAndOpeningAccountingDocument();
                Messages.SuccessMessage("بستن سال مالی حسابداری و صدور سند اختتامیه و افتتاحیه");
            }
            catch (Exception exception)
            {
                Messages.ExceptionMessage(exception);
            }
        }

        #endregion

        #region Methods
        public void CreateClosingAndOpeningAccountingDocument()
        {


            var db = DDB.NewContext();

            var fiscalYears = db.tbl_glb_fiscal_year.OrderByDescending(x => x.glb_fiscal_year_id).ToList();
            var newFiscalYear = fiscalYears.First();
            if (GlobalVariables.current_fiscal_year_id != newFiscalYear.glb_fiscal_year_id)
                throw new Exception(string.Format("سال مالی جاری سال {0} و آخرین سال مالی سال {1} می باشد. لطفا با آخرین سال مالی وارد برنامه شوید.", GlobalVariables.current_fiscal_year_name, newFiscalYear.glb_fiscal_year_name));
            if (fiscalYears.Count == 1)
                throw new Exception("برنامه تنها یک سال مالی دارد");

            var oldFiscalYear = fiscalYears[1];

            GlobalVariables.current_fiscal_year_id = oldFiscalYear.glb_fiscal_year_id;
            new BLL<stp_acc_chart_account_calculate_debt_creditResult>().DoDataBaseOperation();
            GlobalVariables.current_fiscal_year_id = newFiscalYear.glb_fiscal_year_id;

            var documents = db.tbl_acc_document.ToList();

            Boolean closingDeleted = false;
            var closings = db.tbl_acc_document
                .Where
                (
                    x =>
                        x.acc_document_is_closing == true &&
                        x.acc_document_glb_fiscal_year_id == oldFiscalYear.glb_fiscal_year_id
                ).ToList();
            if (closings.Count > 0)
            {
                closingDeleted = true;
                db.tbl_acc_document.DeleteObject(closings.First());
            }

            var openings = db.tbl_acc_document
             .Where
             (
                 x =>
                     x.acc_document_is_opening == true &&
                     x.acc_document_glb_fiscal_year_id == newFiscalYear.glb_fiscal_year_id
             ).ToList();
            if (openings.Count > 0)
                db.tbl_acc_document.DeleteObject(openings.First());

            long newDocumentCode = CreateNewDocumentCode(db, oldFiscalYear.glb_fiscal_year_id);
            var newDocumentNo = CreateNewDocumentNo(db, oldFiscalYear.glb_fiscal_year_id);

            if (closingDeleted && newDocumentCode > 1)
                newDocumentCode--;

            if (closingDeleted && newDocumentNo > 1)
                newDocumentNo--;

            var closingDocumentType = FindAccDocumentType(db, "اخ");
            var openningDocumentType = FindAccDocumentType(db, "اف");

            string closingDescription = "سند اختتامیه سال " + oldFiscalYear.glb_fiscal_year_name;
            string openningDescription = "سند افتتاحیه سال " + newFiscalYear.glb_fiscal_year_name;

            var closingDocument = new tbl_acc_document()
            {
                acc_document_acc_document_type_id = closingDocumentType.acc_document_type_id,
                acc_document_date = APMDateTime.dateWithNoSlash(APMDateTime.Today),
                acc_document_description = closingDescription,
                acc_document_glb_branch_id = GlobalVariables.current_branch_id,
                acc_document_glb_fiscal_year_id = oldFiscalYear.glb_fiscal_year_id,
                acc_document_from_inventory = false,
                acc_document_is_closing = true,
                acc_document_is_opening = false,
                acc_document_register_date = APMDateTime.dateWithNoSlash(APMDateTime.Today),
                acc_document_register_time = APMDateTime.SystemTime,
                acc_document_registerer_glb_user_id = GlobalVariables.current_user_id,
                acc_document_status_glb_coding_id = (long)AccDocumentStatus.Temporary,
                acc_document_code = newDocumentCode.ToString(),
                acc_document_no = newDocumentNo
            };

            var openningDocument = new tbl_acc_document()
            {
                acc_document_acc_document_type_id = openningDocumentType.acc_document_type_id,
                acc_document_date = APMDateTime.dateWithNoSlash(APMDateTime.Today),
                acc_document_description = openningDescription,
                acc_document_glb_branch_id = GlobalVariables.current_branch_id,
                acc_document_glb_fiscal_year_id = newFiscalYear.glb_fiscal_year_id,
                acc_document_from_inventory = false,
                acc_document_is_closing = false,
                acc_document_is_opening = true,
                acc_document_register_date = APMDateTime.dateWithNoSlash(APMDateTime.Today),
                acc_document_register_time = APMDateTime.SystemTime,
                acc_document_registerer_glb_user_id = GlobalVariables.current_user_id,
                acc_document_status_glb_coding_id = (long)AccDocumentStatus.Temporary,
                acc_document_code = (newDocumentCode + 1).ToString(),
                acc_document_no = 1
            };

            var accounts = db.tbl_acc_chart_account.ToList();
            foreach (tbl_acc_chart_account account in accounts)
            {
                if (!account.acc_chart_account_acc_detail_id.HasValue)
                    continue;

                double debt = (account.acc_chart_account_debt.HasValue ? account.acc_chart_account_debt.Value : 0);
                double credit = (account.acc_chart_account_credit.HasValue ? account.acc_chart_account_credit.Value : 0);
                double remaining = credit - debt;

                credit = (remaining > 0 ? remaining : 0);
                debt = (remaining < 0 ? -remaining : 0);

                if (debt == credit)
                    continue;

                var closingArticle = new tbl_acc_document_article()
                {
                    acc_document_article_acc_chart_account_id = account.acc_chart_account_id,
                    acc_document_article_credit = debt,
                    acc_document_article_debt = credit,
                    acc_document_article_description = closingDescription,
                    acc_document_article_glb_branch_id = GlobalVariables.current_branch_id,
                };
                closingDocument.tbl_acc_document_article.Add(closingArticle);

                var openningArticle = new tbl_acc_document_article()
                {
                    acc_document_article_acc_chart_account_id = account.acc_chart_account_id,
                    acc_document_article_credit = credit,
                    acc_document_article_debt = debt,
                    acc_document_article_description = openningDescription,
                    acc_document_article_glb_branch_id = GlobalVariables.current_branch_id,
                };
                openningDocument.tbl_acc_document_article.Add(openningArticle);
            }
            db.tbl_acc_document.AddObject(closingDocument);
            db.tbl_acc_document.AddObject(openningDocument);

            db.SaveChanges();
        }

        private long CreateNewDocumentCode(SahaamEntities db, long fiscalYearId)
        {
            var list = db.tbl_acc_document
                  .Where
                  (x =>
                      x.acc_document_code != null &&
                      x.acc_document_code != "" &&
                      x.acc_document_glb_fiscal_year_id == fiscalYearId
                  )
                  .Select(x => x.acc_document_code)
                  .ToList()
                  .Select(x => long.Parse(x));

            if (list.Count() == 0)
                return 1;
            else
                return list.Max() + 1;
        }
        private int? CreateNewDocumentNo(SahaamEntities db, long fiscalYearId)
        {
            return db.tbl_acc_document
                  .Where
                  (x =>
                      x.acc_document_no != null &&
                      x.acc_document_glb_fiscal_year_id == fiscalYearId
                  )
                  .Max(x => x.acc_document_no) + 1;
        }

        private static tbl_acc_document_type FindAccDocumentType(SahaamEntities db, string key)
        {
            if (db.tbl_acc_document_type.Count() == 0)
                throw HelperMethods.CreateException("در جدول نوع سند حسابداری رکوردی وجود ندارد");
            var documentType = db.tbl_acc_document_type.First();
            var list = db.tbl_acc_document_type.Where(x => x.acc_document_type_name.Contains(key)).ToList();
            if (list.Count > 0)
                documentType = list.First();
            return documentType;
        }
        #endregion
    }
}
