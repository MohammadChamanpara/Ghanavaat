using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APMTools;

namespace DataAccessLayer
{
  #region Shared Methods
  internal static class SharedMethods
  {
    public static int TotalSecondsToJerib(int totalSeconds)
    {
      return totalSeconds / 1800;
    }
    public static int TotalSecondsToMinute(int totalSeconds)
    {
      return (totalSeconds % 1800) / 60;
    }
    public static int TotalSecondsToSecond(int totalSeconds)
    {
      return (totalSeconds % 1800) % 60;
    }
    public static int GenerateTotalSeconds(int jerib, int minute, int second)
    {
      return
          jerib * 1800 +
          minute * 60 +
          second;
    }
  }
  #endregion

  #region Ownership
  public partial class tbl_gnt_ownership
  {
    private static SahaamEntities dataBase = DDB.NewContext();
    public int gnt_ownership_gnt_water_period
    {
      get
      {
        if (this.tbl_gnt_water == null)
          return 0;
        return this.tbl_gnt_water.tbl_gnt_water2.tbl_gnt_water1.gnt_water1_period;
      }
    }
    public int TotalSeconds
    {
      get
      {
        return SharedMethods.GenerateTotalSeconds
        (
            this.gnt_ownership_jerib,
            this.gnt_ownership_minute,
            this.gnt_ownership_second
        );
      }
      set
      {
        this.gnt_ownership_jerib = SharedMethods.TotalSecondsToJerib(value);
        this.gnt_ownership_minute = SharedMethods.TotalSecondsToMinute(value);
        this.gnt_ownership_second = SharedMethods.TotalSecondsToSecond(value);
      }
    }
    private double Coefftion
    {
      get
      {
        if (this.gnt_ownership_gnt_water_period == 0)
          return 0;
        return (double)10 / this.gnt_ownership_gnt_water_period;
      }
    }
    private double ExactMeters
    {
      get
      {
        return this.Coefftion * this.TotalSeconds / 1800 * 1000;
      }
    }
    public int gnt_ownership_correct_jerib
    {
      get
      {
        return SharedMethods.TotalSecondsToJerib(this.TotalSeconds);
      }
    }
    public int gnt_ownership_correct_minute
    {
      get
      {
        return SharedMethods.TotalSecondsToMinute(this.TotalSeconds);
      }
    }
    public int gnt_ownership_correct_second
    {
      get
      {
        return SharedMethods.TotalSecondsToSecond(this.TotalSeconds);
      }
    }
    public int gnt_ownership_correct_earth
    {
      get
      {
        return (int)Math.Round(this.ExactMeters);
      }
    }
    public double gnt_ownership_correct_credit
    {
      get
      {
        if (dataBase.tbl_gnt_settings.Count() == 0)
          return 0;
        int creditMeters = dataBase.tbl_gnt_settings.First().gnt_settings_credit_meters;
        return Math.Round(this.ExactMeters / creditMeters, 2);
      }
    }
    public void InitialFromRecord(stp_gnt_ownership_selResult record)
    {
      this.gnt_ownership_jerib = record.gnt_ownership_jerib;
      this.gnt_ownership_minute = record.gnt_ownership_minute;
      this.gnt_ownership_second = record.gnt_ownership_second;
      if (record.gnt_ownership_gnt_water_id != 0)
        this.tbl_gnt_water = dataBase.tbl_gnt_water.Where(w => w.gnt_water_id == record.gnt_ownership_gnt_water_id).First();
    }
    public static tbl_gnt_ownership FindRecord(long id)
    {
      return FindRecord(dataBase, id);
    }
    public static tbl_gnt_ownership FindRecord(SahaamEntities dataBase, long id)
    {
      var list = dataBase.tbl_gnt_ownership.Where(o => o.gnt_ownership_id == id);
      if (list.Count() == 0)
        return null;
      return list.First();
    }

  }
  #endregion

  #region Creditor
  public partial class tbl_gnt_creditor
  {
    private static SahaamEntities dataBase = DDB.NewContext();

    public double gnt_creditor_sum_credit_t
    {
      get
      {
        return Math.Truncate(this.gnt_creditor_sum_credit);
      }
    }
    public double gnt_creditor_sum_credit_s
    {
      get
      {
        return Math.Round((this.gnt_creditor_sum_credit - this.gnt_creditor_sum_credit_t) * 30, 0);
      }
    }
    public double gnt_creditor_sum_credit_m
    {
      get
      {
        return 30;
      }
    }
    public int gnt_creditor_no
    {
      get
      {
        int no = 0;
        int.TryParse(this.gnt_creditor_child_code, out no);
        return no;
      }
    }
    public int gnt_creditor_sum_earth
    {
      get
      {
        return this.tbl_gnt_ownership.Sum(o => o.gnt_ownership_earth);
      }
    }
    public double gnt_creditor_sum_credit
    {
      get
      {
        return Math.Round(this.tbl_gnt_ownership.Sum(o => o.gnt_ownership_credit), 2);
        //return this.tbl_gnt_ownership.Sum(o => o.gnt_ownership_credit);
      }
    }
    public int gnt_creditor_earth_count
    {
      get
      {
        return this.tbl_gnt_earth.Count(o => o.gnt_earth_gnt_creditor_id == this.gnt_creditor_id);
      }
    }
    public string gnt_creditor_name
    {
      get
      {
        return this.tbl_glb_coding.glb_coding_name + " " + this.gnt_creditor_first_name.Trim() + " " + this.gnt_creditor_family.Trim();
      }
    }

    private long? _remaining;
    public long gnt_creditor_account_remaining
    {
      get
      {
        if (_remaining != null)
          return _remaining.Value;

        var thisYearAccounts = this.tbl_gnt_creditor_account.Where(y => y.gnt_creditor_account_glb_fiscal_year_id == GlobalVariables.current_fiscal_year_id);

        if (_remaining == null)
          _remaining =
              thisYearAccounts.Sum(x => x.gnt_creditor_account_credit) -
              thisYearAccounts.Sum(x => x.gnt_creditor_account_debt);

        return _remaining.Value;
      }
    }
    public string gnt_creditor_account_remaining_c
    {
      get
      {
        return Math.Abs(this.gnt_creditor_account_remaining).DigitGrouping();
      }
    }

    public string gnt_creditor_account_status
    {
      get
      {
        if (this.gnt_creditor_account_remaining == 0)
          return "تراز";
        else if (this.gnt_creditor_account_remaining < 0)
          return "بدهکار";
        else
          return "بستانکار";
      }
    }
    public void UpdateAccountForPublicCosts()
    {
      try
      {
        SahaamEntities db = DDB.NewContext();
        var creditor = db.tbl_gnt_creditor.First(x => x.gnt_creditor_id == this.gnt_creditor_id);
        var fiscalYear = db.tbl_glb_fiscal_year.First(x => x.glb_fiscal_year_id == GlobalVariables.current_fiscal_year_id);

        var publicCosts = db.tbl_gnt_cost_type.Where
        (
            x =>
            x.gnt_cost_type_glb_fiscal_year_id == fiscalYear.glb_fiscal_year_id &&
            x.gnt_cost_type_is_public == true
        ).ToList();

        if (publicCosts.Count == 0)
        {
          throw new Exception("در دوره انتخاب شده هزینه عمومی تعریف نشده است");
        }

        var creditorAccounts =
            db
            .tbl_gnt_creditor_account
            .Where
            (
                x =>
                x.gnt_creditor_account_gnt_creditor_id == creditor.gnt_creditor_id &&
                x.gnt_creditor_account_is_public_cost == true &&
                x.gnt_creditor_account_glb_fiscal_year_id == fiscalYear.glb_fiscal_year_id
            )
            .ToList();

        foreach (var creditorAccount in creditorAccounts)
          db.tbl_gnt_creditor_account.DeleteObject(creditorAccount);

        foreach (tbl_gnt_cost_type cost in publicCosts)
          creditor.tbl_gnt_creditor_account.Add(new tbl_gnt_creditor_account()
          {
            gnt_creditor_account_title = cost.gnt_cost_type_name,
            gnt_creditor_account_date = fiscalYear.glb_fiscal_year_start_date.Replace("/", ""),
            gnt_creditor_account_debt = RoundPrice(cost.gnt_cost_type_price * creditor.gnt_creditor_sum_credit),
            gnt_creditor_account_credit = 0,
            gnt_creditor_account_description = "هزینه های عمومی سهامداران",
            gnt_creditor_account_glb_fiscal_year_id = fiscalYear.glb_fiscal_year_id,
            gnt_creditor_account_is_public_cost = true
          });

        db.SaveChanges();
      }
      catch (Exception exception)
      {
        while (exception.InnerException != null)
          exception = exception.InnerException;
        throw new Exception("خطا در اعمال هزینه های عمومی در حساب سهامدر. متن خطا : \n" + exception.Message);
      }
    }

    private static int RoundPrice(double price)
    {
      return ((int)Math.Round(price)).Round(10000);
    }
    public static void TransferCreditorAccountsToNewfiscalYear(int finePercent, long oldFiscalYearId, long newFiscalYearId)
    {
      try
      {
        var db = DDB.NewContext();
        var fiscalYearList = db.tbl_glb_fiscal_year.Where(x => x.glb_fiscal_year_id == oldFiscalYearId);
        if (fiscalYearList.Count() == 0)
        {
          Messages.ErrorMessage(string.Format("سال مبدا با شناسه {0} وجود ندارد", oldFiscalYearId));
          return;
        }
        var oldFiscalYear = fiscalYearList.First();
        fiscalYearList = db.tbl_glb_fiscal_year.Where(x => x.glb_fiscal_year_id == newFiscalYearId);
        if (fiscalYearList.Count() == 0)
        {
          Messages.ErrorMessage(string.Format("سال مقصد با شناسه {0} وجود ندارد", newFiscalYearId));
          return;
        }
        var newFiscalYear = fiscalYearList.First();

        var creditors = db.tbl_gnt_creditor.ToList();
        foreach (var creditor in creditors)
        {
          var oldOpening = db
            .tbl_gnt_creditor_account
            .Where
            (
                x =>
                x.gnt_creditor_account_gnt_creditor_id == creditor.gnt_creditor_id &&
                x.gnt_creditor_account_glb_fiscal_year_id == newFiscalYear.glb_fiscal_year_id &&
                x.gnt_creditor_account_is_opening == true
            )
            .ToList();

          foreach (var opening in oldOpening)
            db.tbl_gnt_creditor_account.DeleteObject(opening);

          var creditorAccounts = db
            .tbl_gnt_creditor_account
            .Where
            (
                x =>
                x.gnt_creditor_account_gnt_creditor_id == creditor.gnt_creditor_id &&
                x.gnt_creditor_account_glb_fiscal_year_id == oldFiscalYear.glb_fiscal_year_id
            )
            .ToList();


          var sumDebt = creditorAccounts.Sum(x => x.gnt_creditor_account_debt)
          - creditorAccounts.Sum(x => x.gnt_creditor_account_credit);


          var newAccount = new tbl_gnt_creditor_account()
          {
            gnt_creditor_account_date = newFiscalYear.glb_fiscal_year_start_date,
            gnt_creditor_account_credit = 0,
            gnt_creditor_account_glb_fiscal_year_id = newFiscalYearId,
            gnt_creditor_account_gnt_creditor_id = creditor.gnt_creditor_id,
            gnt_creditor_account_gnt_service_id = null,
            gnt_creditor_account_is_public_cost = false,
            gnt_creditor_account_is_opening = true
          };
          if (sumDebt >= 0)
          {
            newAccount.gnt_creditor_account_debt = RoundPrice(sumDebt * (1 + (double)finePercent / 100));
            newAccount.gnt_creditor_account_title = "مانده بدهی سال " + oldFiscalYear.glb_fiscal_year_name;
          }
          else
          {
            newAccount.gnt_creditor_account_credit = -sumDebt;
            newAccount.gnt_creditor_account_title = "مانده بستانکاری سال " + oldFiscalYear.glb_fiscal_year_name;
          }
          db.tbl_gnt_creditor_account.AddObject(newAccount);
        }
        db.SaveChanges();
      }
      catch (Exception exception)
      {
        while (exception.InnerException != null)
          exception = exception.InnerException;
        throw new Exception("خطا در انتقال حساب سهامدران به سال جدید. متن خطا : \n" + exception.Message);
      }

    }
  }
  public partial class stp_gnt_creditor_selResult
  {
    public tbl_gnt_creditor ToEntity()
    {
      var db = DDB.NewContext();
      var creditorsList = db.tbl_gnt_creditor.Where(x => x.gnt_creditor_id == this.gnt_creditor_id);
      if (creditorsList.Count() == 0)
        throw HelperMethods.CreateException("سهامدار یافت نشد.");
      if (creditorsList.Count() > 1)
        throw HelperMethods.CreateException("چند سهامدار یافت شد.");
      return creditorsList.First();
    }
  }
  #endregion

  #region Settings
  public partial class tbl_gnt_settings
  {
    public long gnt_settings_one_credit_price
    {
      get
      {
        if (this._gnt_settings_total_credit_count == 0)
          return 0;
        return this.gnt_settings_total_credit_price /
            this.gnt_settings_total_credit_count;
      }
    }
  }
  #endregion

  #region DealArticle
  public partial class tbl_gnt_deal_article
  {
    public int TotlaSeconds
    {
      get
      {
        return SharedMethods.GenerateTotalSeconds
        (
            this.gnt_deal_article_sell_jerib,
            this.gnt_deal_article_sell_minute,
            this.gnt_deal_article_sell_second
        );
      }

    }
  }

  #endregion

  #region Water
  public partial class tbl_gnt_water
  {
    public string gnt_water_name
    {
      get
      {
        return
            this.tbl_gnt_water2.tbl_gnt_water1.gnt_water1_name +
            '-' +
            this.tbl_gnt_water2.gnt_water2_name +
            '-' +
            this.gnt_water_taagh_name;
      }
    }
  }

  #endregion
}
