using UserInterfaceLayer;
using APMTools;
using DataAccessLayer;
using APM_SubSystems;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Documents;
using System.Collections.Generic;

namespace APM_SubSystems
{
    public partial class frm_gnt_deal : WindowTwoTabs<stp_gnt_deal_selResult, stp_gnt_deal_article_selResult>
    {
        #region Variables
        long OneCreditPrice { get; set; }
        struct ProcessedDeal
        {
            public long sellerId, buyerId, earthId;
            public double sellerCredit,selCredit;
        }

        #endregion

        #region Constructor

        public frm_gnt_deal()
        {
            InitializeComponent();
            Initial_WindowTwoTab(documentHeader, null, null, null, null, null, dbg_gnt_deal, dbg_gnt_deal_article, tbr_buy_request, documentHeader, grp_article_current_row, "gnt_deal", "gnt_deal_article", tab_main, txt_gnt_deal_article_description, null, null, new APM_SubSystems.Sahaam.gnt_reports.gnt_rpt_creditor_deals.gnt_rpt_creditor_deals());
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            var dataBase = DDB.NewContext();
            if (dataBase.tbl_gnt_settings.Count() == 0)
            {
                Messages.ErrorMessage("تنظیمات سهام وارد نشده است");
                return;
            }
            this.OneCreditPrice = dataBase.tbl_gnt_settings.First().gnt_settings_one_credit_price;
        }
        public override void OperationsAfterInsertArticle()
        {
            base.OperationsAfterInsertArticle();
            CopyArticleData();
            selectedArticle.gnt_deal_article_sell_price = OneCreditPrice;
            MoveCollectionViewArticle();
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            if (
                Messages.QuestionMessage_YesNo("آیا مایلید بر اساس این سند میزان مالکیت سهامداران تغییر کند؟")
                == MessageBoxResult.Yes)
                SaveDealToOwnerships();
        }
        #endregion

        #region Events
        private void brw_gnt_deal_article_gnt_sell_creditor_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            long oldSeller = selectedArticle.gnt_deal_article_gnt_sell_creditor_id;
            BrowseClick(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), selectedArticle, "فروشنده", typeof(frm_gnt_creditor), sender);
            AfterSelectSeller(oldSeller);
        }
        private void brw_gnt_deal_article_gnt_buy_creditor_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), selectedArticle, "خریدار", typeof(frm_gnt_creditor), sender);
            AfterSelectBuyer();
        }
        private void brw_gnt_deal_article_gnt_ownership_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            if (selectedArticle.gnt_deal_article_gnt_sell_creditor_id == 0)
            {
                Messages.ErrorMessage("لطفاً فروشنده را انتخاب نمایید");
                return;
            }
            var recordParameter = GetOwnershipRecordParameter();
            BrowseClick_Parameter(new WindowSelectGrid<stp_gnt_ownership_selResult>(), selectedArticle, recordParameter, "مالکیت", typeof(frm_gnt_ownership), sender);
            AfterSelectOwnership();
        }
        private void brw_gnt_deal_article_gnt_earth_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            if (selectedArticle.gnt_deal_article_gnt_sell_creditor_id == 0)
            {
                Messages.ErrorMessage("لطفاً فروشنده را انتخاب نمایید");
                return;
            }
            var recordParameter = GetEarthRecordParameter();
            BrowseClick_Parameter(new WindowSelectGrid<stp_gnt_earth_selResult>(), selectedArticle, recordParameter, "زمین", typeof(frm_gnt_earth), sender);
        }


        private void brw_gnt_deal_article_gnt_sell_creditor_id_XTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            long oldSeller = selectedArticle.gnt_deal_article_gnt_sell_creditor_id;
            CodeTextBox_KeyDown<stp_gnt_creditor_selResult, stp_gnt_deal_article_selResult>(sender, "gnt_deal_article_gnt_sell_creditor_id", e, selectedArticle);
            AfterSelectSeller(oldSeller);
        }
        private void brw_gnt_deal_article_gnt_buy_creditor_id_XTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_gnt_creditor_selResult, stp_gnt_deal_article_selResult>(sender, "gnt_deal_article_gnt_buy_creditor_id", e, selectedArticle);
            AfterSelectBuyer();
        }
        private void txt_gnt_deal_article_sell_jeribMinuteSecond_LostFocus(object sender, RoutedEventArgs e)
        {
            var dataBase = DDB.NewContext();
            if (selectedArticle.gnt_deal_article_gnt_water_id == 0)
                return;
            var ownership = new tbl_gnt_ownership();
            ownership.tbl_gnt_water = dataBase.tbl_gnt_water.Where(w => w.gnt_water_id == selectedArticle.gnt_deal_article_gnt_water_id).First();
            ownership.gnt_ownership_jerib = selectedArticle.gnt_deal_article_sell_jerib;
            ownership.gnt_ownership_minute = selectedArticle.gnt_deal_article_sell_minute;
            ownership.gnt_ownership_second = selectedArticle.gnt_deal_article_sell_second;

            selectedArticle.gnt_deal_article_sell_jerib = ownership.gnt_ownership_correct_jerib;
            selectedArticle.gnt_deal_article_sell_minute = ownership.gnt_ownership_correct_minute;
            selectedArticle.gnt_deal_article_sell_second = ownership.gnt_ownership_correct_second;
            selectedArticle.gnt_deal_article_sell_credit = ownership.gnt_ownership_correct_credit;
            selectedArticle.gnt_deal_article_sell_earth = ownership.gnt_ownership_correct_earth;
            selectedArticle.gnt_deal_article_sell_all_price = (int)Math.Round(selectedArticle.gnt_deal_article_sell_credit * selectedArticle.gnt_deal_article_sell_price);
            selectedArticle.gnt_deal_article_sell_agreement_price = selectedArticle.gnt_deal_article_sell_all_price;

            MoveCollectionViewArticle();
        }
        #endregion

        #region Methods
        private void CopyArticleData()
        {
            try
            {
                if (bindingListArticle.Count > 1)
                {
                    var previousArticle = bindingListArticle[bindingListArticle.Count - 2];
                    var newArticle = bindingListArticle[bindingListArticle.Count - 1];
                    GlobalFunctions.CopyRecord(newArticle, previousArticle);
                    newArticle.gnt_deal_article_id = 0;
                    newArticle.gnt_deal_article_sell_jerib = 0;
                    newArticle.gnt_deal_article_sell_minute = 0;
                    newArticle.gnt_deal_article_sell_second = 0;
                    newArticle.gnt_deal_article_sell_earth = 0;
                    newArticle.gnt_deal_article_sell_credit = 0;
                    newArticle.gnt_deal_article_sell_all_price = 0;
                }
            }
            catch { }
        }
        private void SaveDealToOwnerships()
        {
            try
            {
                var dataBase = DDB.NewContext();
                var deal = dataBase.tbl_gnt_deal.Where(d => d.gnt_deal_id == selectedRecord.gnt_deal_id).First();
                foreach (var article in deal.tbl_gnt_deal_article)
                {
                    var ownership = tbl_gnt_ownership.FindRecord(dataBase, article.gnt_deal_article_gnt_ownership_id);
                    if (ownership == null)
                    {
                        Messages.ErrorMessage("مالکیت سهامدار یافت نشد");
                        return;
                    }
                    if (ownership.TotalSeconds > article.TotlaSeconds)
                    {
                        ownership.TotalSeconds -= article.TotlaSeconds; /* Decrease old Ownership */
                        dataBase.tbl_gnt_ownership.ApplyCurrentValues(ownership);
                    }
                    else if (ownership.TotalSeconds == article.TotlaSeconds)
                        dataBase.tbl_gnt_ownership.DeleteObject(ownership);
                    else
                    {
                        string message = "سهام انتقال یافته بیش از سهام مالک است" + "\n" +
                            "نام مالک :" + ownership.tbl_gnt_creditor.gnt_creditor_family + "\n" +
                            "نام آب :" + ownership.tbl_gnt_water.gnt_water_name;

                        Messages.ErrorMessage(message);
                        return;
                    }

                    tbl_gnt_ownership newOwnership = new tbl_gnt_ownership(); /* Create New Ownership with amount of selling*/
                    newOwnership.gnt_ownership_gnt_water_id = ownership.gnt_ownership_gnt_water_id;
                    newOwnership.gnt_ownership_gnt_creditor_id = article.gnt_deal_article_gnt_buy_creditor_id;
                    newOwnership.TotalSeconds = article.TotlaSeconds;
                    newOwnership.gnt_ownership_glb_branch_id = GlobalVariables.current_branch_id;
                    dataBase.tbl_gnt_ownership.AddObject(newOwnership);
                }
                dataBase.SaveChanges();
            }
            catch (Exception exception)
            {
                string message = "خطا در ذخیره سازی تغییرات" + "\n" + exception.Message;
                if (exception.InnerException != null)
                    message += "\n" + "خطای پایگاه داده " + "\n" + exception.InnerException.Message;
                Messages.ErrorMessage(message);

            }
        }

        private void AfterSelectSeller(long oldSeller)
        {
            CheckDiffrentCreditors();
            if (oldSeller != selectedArticle.gnt_deal_article_gnt_sell_creditor_id)
            {
                GlobalFunctions.Copy_PK_To_FK(selectedArticle, new stp_gnt_water_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedArticle, new stp_gnt_earth_selResult());
                selectedArticle.gnt_deal_article_seller_credit = 0;
                selectedArticle.gnt_deal_article_seller_earth = 0;
                selectedArticle.gnt_deal_article_seller_jerib = 0;
                selectedArticle.gnt_deal_article_seller_minute = 0;
                selectedArticle.gnt_deal_article_seller_second = 0;
                MoveCollectionViewArticle();
            }
        }
        private void AfterSelectOwnership()
        {
            var ownership = tbl_gnt_ownership.FindRecord(selectedArticle.gnt_deal_article_gnt_ownership_id);
            if (ownership == null)
            {
                Messages.ErrorMessage("مالکیت سهامدار برای این آب تعریف نشده است");
                return;
            }

            selectedArticle.gnt_deal_article_gnt_water_id = ownership.gnt_ownership_gnt_water_id;
            selectedArticle.gnt_deal_article_gnt_water_name = ownership.tbl_gnt_water.gnt_water_name;

            selectedArticle.gnt_deal_article_seller_jerib = ownership.gnt_ownership_jerib;
            selectedArticle.gnt_deal_article_seller_minute = ownership.gnt_ownership_minute;
            selectedArticle.gnt_deal_article_seller_second = ownership.gnt_ownership_second;
            selectedArticle.gnt_deal_article_seller_credit = ownership.gnt_ownership_credit;
            selectedArticle.gnt_deal_article_seller_earth = ownership.gnt_ownership_earth;

            MoveCollectionViewArticle();
        }
        private stp_gnt_ownership_selResult GetOwnershipRecordParameter()
        {
            return new stp_gnt_ownership_selResult()
            {
                gnt_ownership_gnt_creditor_id = selectedArticle.gnt_deal_article_gnt_sell_creditor_id
            };
        }
        private stp_gnt_earth_selResult GetEarthRecordParameter()
        {
            return new stp_gnt_earth_selResult()
            {
                gnt_earth_gnt_creditor_id = selectedArticle.gnt_deal_article_gnt_sell_creditor_id
            };
        }
        private void CheckDiffrentCreditors()
        {
            if (selectedArticle.gnt_deal_article_gnt_buy_creditor_id != selectedArticle.gnt_deal_article_gnt_sell_creditor_id)
                return;
            if (selectedArticle.gnt_deal_article_gnt_sell_creditor_id == 0)
                return;
            Messages.ErrorMessage("فروشنده و خریدار یکسان انتخاب شده اند");
            GlobalFunctions.Copy_PK_To_FK(selectedArticle, new stp_gnt_creditor_selResult(), "gnt_deal_article_gnt_buy_creditor_id");
            GlobalFunctions.Copy_PK_To_FK(selectedArticle, new stp_gnt_creditor_selResult(), "gnt_deal_article_gnt_sell_creditor_id");
            MoveCollectionViewArticle();
        }
        private void AfterSelectBuyer()
        {
            CheckDiffrentCreditors();
        }
        #endregion
    }
}
