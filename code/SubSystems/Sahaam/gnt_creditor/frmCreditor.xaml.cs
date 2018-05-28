using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DataAccessLayer;
using APMTools;
using BusinessLogicLayer;
using UserInterfaceLayer;
using System.Linq;
using APM_SubSystems.Sahaam.gnt_creditor;
using System.Collections.Generic;

namespace APM_SubSystems
{
  public partial class frm_gnt_creditor : WindowEntity<stp_gnt_creditor_selResult>
  {
    #region Variables
    Boolean windowLoaded = false;
    long? filterGroupId = null;
    ArticlePackage<stp_gnt_creditor_group_selResult, stp_gnt_creditor_in_group_selResult> gnt_creditor_group = new WindowBase<stp_gnt_creditor_selResult>.ArticlePackage<stp_gnt_creditor_group_selResult, stp_gnt_creditor_in_group_selResult>();
    #endregion

    #region Constructor
    public frm_gnt_creditor()
    {
      InitializeComponent();
      Initial_WindowEntity(dbgCreditor, windowToolbar, grpInfo, "gnt_creditor", true, null, (long)EntityType.gnt_creditor, txt_gnt_creditor_child_code);
    }
    #endregion

    #region override
    public override SaveResult SaveClick(bool askForConfirm)
    {
      return base.SaveClick(askForConfirm);
    }
    public override void SetEnables(bool enable)
    {
      base.SetEnables(enable);
      txt_gnt_creditor_child_code.IsEnabled = (operationType == OperationType.Insert);
      grpAdvancedSearch.IsEnabled = (operationType == OperationType.Nothing);
      stkEmptyCodes.IsEnabled = true;
      btnRefreshEmptyCodes.IsEnabled = true;
      lst_empty_codes.IsEnabled = true;
      lblEmptyCodesCount.IsEnabled = true;
      lblEmptyCodesCountTitle.IsEnabled = true;
      btnShowAllAccounts.IsEnabled = (operationType == OperationType.Nothing);
      lblShowAllAccounts.IsEnabled = (operationType == OperationType.Nothing);
      btnShowAllEarthCounts.IsEnabled = (operationType == OperationType.Nothing);
      lblShowAllEarthCounts.IsEnabled = (operationType == OperationType.Nothing);
    }

    public override bool ValidationForSave()
    {
      if (cmb_gnt_creditor_title_glb_coding_id.SelectedIndex == -1)
      {
        Messages.ErrorMessage("لطفا عنوان را انتخاب نمایید");
        return false;
      }
      return base.ValidationForSave();
    }
    public override bool ValidationForInsert()
    {
      CreateNewCode();
      return true;
    }
    public override void InitializationBeforeSave()
    {
      base.InitializationBeforeSave();
      selectedRecord.gnt_creditor_name = cmb_gnt_creditor_title_glb_coding_id.Text + " " + txt_gnt_creditor_first_name.Text.Trim() + " " + txt_gnt_creditor_family.Text.Trim();
    }
    public override void RefreshClick()
    {
      base.RefreshClick();
      CalculateAccountForAllCreditors();
      CalculateEarthCountForAllCreditors();
    }
    public override void Window_Loaded(object sender, RoutedEventArgs e)
    {
      base.Window_Loaded(sender, e);
      CalculateAccountForAllCreditors();
      CalculateEarthCountForAllCreditors();
      grpAdvancedSearch.XCanClear = true;
      grpAdvancedSearch.XCanCollapse = true;
      grpInfo.XCanCollapse = true;
      windowLoaded = true;
      FillTitleComboBox();
    }
    public override void PrintClick()
    {
      btnPrintCreditorsList_Click(null, null);
    }
    public override void OperationsAfterSaved()
    {
      base.OperationsAfterSaved();
      gnt_creditor_group.Save(selectedRecord);
    }

    #endregion

    #region Methods
    private void CalculateAccountForAllCreditors(Boolean caculatefinancial = true)
    {
      try
      {
        if (caculatefinancial && bindingList.Count > 100)
          return;
        foreach (var record in bindingList)
          CalculateAccountForCreditor(record);
        MoveCollectionView();
      }
      catch (Exception exception)
      {
        Messages.ErrorMessage(exception.CompleteMessages());
      }
    }
    private void CalculateEarthCountForAllCreditors(Boolean caculatefinancial = true)
    {
      try
      {
        if (caculatefinancial && bindingList.Count > 100)
          return;
        foreach (var record in bindingList)
          CalculateEarthCountForCreditor(record);
        MoveCollectionView();
      }
      catch (Exception exception)
      {
        Messages.ErrorMessage(exception.CompleteMessages());
      }
    }
    private void CalculateAccountForCreditor(stp_gnt_creditor_selResult oldCreditor, Boolean caculatefinancial = true)
    {
      var newCreditor = oldCreditor.ToEntity();

      oldCreditor.gnt_creditor_sum_credit = newCreditor.gnt_creditor_sum_credit;

      if (!caculatefinancial)
        return;

      oldCreditor.gnt_creditor_sum_earth = newCreditor.gnt_creditor_sum_earth;
      oldCreditor.gnt_creditor_account_remaining = Math.Abs(newCreditor.gnt_creditor_account_remaining);
      oldCreditor.gnt_creditor_account_remaining_c = oldCreditor.gnt_creditor_account_remaining.DigitGrouping();

      oldCreditor.gnt_creditor_account_status = newCreditor.gnt_creditor_account_status;
    }
    private void CalculateEarthCountForCreditor(stp_gnt_creditor_selResult oldCreditor, Boolean caculatefinancial = true)
    {
      var newCreditor = oldCreditor.ToEntity();
      oldCreditor.gnt_creditor_earth_count = newCreditor.gnt_creditor_earth_count;
    }
    private int GetShowCount()
    {
      int showCount = 20;
      int.TryParse(txtSearchCount.Text, out showCount);
      if (showCount == 0)
      {
        txtSearchCount.Text = "20";
        showCount = 20;
      }
      return showCount;
    }
    private void CreateNewCode()
    {

      SahaamEntities db = DDB.NewContext();
      long maxCode;
      if (db.tbl_gnt_creditor.Count() == 0)
        maxCode = 1;
      else
      {
        maxCode = long.Parse(db.tbl_gnt_creditor.Select(x => x.gnt_creditor_child_code).Max());
      }

      var digitCount = db.tbl_glb_entity_type_option
          .Where(x => x.glb_entity_type_option_glb_entity_type_id == 12)
          .First()
          .glb_entity_type_option_digit_count;

      newCode = (maxCode + 1).ToString();

      GlobalFunctions.PutZeroBeforeCode(newCode, digitCount);
    }
    private void SimpleSearch()
    {
      ShowSomeRecords(new stp_gnt_creditor_selResult() { gnt_creditor_simple_search = txtSearch.Text, show_count = GetShowCount() });
      CalculateAccountForAllCreditors();
      CalculateEarthCountForAllCreditors();
    }
    private void AdvancedSearch()
    {
      if (!windowLoaded)
        return;
      if (brwFilterGroup.XLabel.IsVisible == false)
        filterGroupId = null;
      int titleCoding = 0;
      if (cmbSearchTitle.SelectedValue != null)
        int.TryParse(cmbSearchTitle.SelectedValue.ToString(), out titleCoding);

      int s;
      int? street = null;
      if (int.TryParse(txtSearchEarthStreet.Text, out s))
        street = s;

      int? line = null;
      if (int.TryParse(txtSearchEarthline.Text, out s))
        line = s;

      int? block = null;
      if (int.TryParse(txtSearchEarthBlock.Text, out s))
        block = s;

      int? plaque = null;
      if (int.TryParse(txtSearchEarthPlaque.Text, out s))
        plaque = s;

      ShowSomeRecords(new stp_gnt_creditor_selResult()
      {
        gnt_creditor_title_glb_coding_id = titleCoding,
        gnt_creditor_address = txtSearchAddress.Text,
        gnt_creditor_code = txtSearchCode.Text,
        gnt_creditor_family = txtSearchFamily.Text,
        gnt_creditor_father_name = txtSearchFatherName.Text,
        gnt_creditor_first_name = txtSearchName.Text,
        gnt_creditor_mobile = txtSearchPhone.Text,
        gnt_creditor_national_code = txtSearchNationalCode.Text,
        gnt_creditor_tel = txtSearchPhone.Text,
        gnt_creditor_num = txtSearchOldCode.Text,
        gnt_creditor_parent = txtSearchOldParentCode.Text,
        show_count = GetShowCount(),
        gnt_creditor_gnt_creditor_group_id = filterGroupId,
        gnt_creditor_filter = true,
        gnt_creditor_gnt_earth_street = street,
        gnt_creditor_gnt_earth_line = line,
        gnt_creditor_gnt_earth_block = block,
        gnt_creditor_gnt_earth_plaque = plaque
      });
      CalculateAccountForAllCreditors();
      CalculateEarthCountForAllCreditors();
    }
    public void FillTitleComboBox()
    {
      var sourceRecords = new BLL<stp_glb_coding_selResult>().GetSomeRecords_DB(new stp_glb_coding_selResult() { glb_coding_category = (int)CodingCategory.Glb_Title });
      cmbSearchTitle.ItemsSource = sourceRecords;
      cmbSearchTitle.DisplayMemberPath = FieldNames<stp_glb_coding_selResult>.Name;
      cmbSearchTitle.SelectedValuePath = FieldNames<stp_glb_coding_selResult>.ID;

      var list = new List<stp_glb_coding_selResult>();
      list.Add(new stp_glb_coding_selResult() { glb_coding_id = 0, glb_coding_name = "" });
      foreach (stp_glb_coding_selResult item in sourceRecords)
        list.Add(item);
      cmbSearchTitle.ItemsSource = list;
    }
    #endregion

    #region Events
    private void gnt_creditor_group_XBrowseClick(object sender, RoutedEventArgs e)
    {
      BrowseClick_MultiSelect(new WindowSelectGrid<stp_gnt_creditor_group_selResult>(), ref gnt_creditor_group, "گروه سهامداران", typeof(frm_gnt_creditor_group));
    }
    private void btnShowAccount_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        new frm_gnt_creditor_account(selectedRecord).ShowDialog();
        CalculateAccountForCreditor(selectedRecord);
        dbgCreditor.datagrid.Items.Refresh();
      }
      catch (Exception exception)
      {
        Messages.ErrorMessage(exception.CompleteMessages());
      }
    }
    private void btnShowOwnership_Click(object sender, RoutedEventArgs e)
    {
      new frm_gnt_ownership(selectedRecord).ShowDialog();
    }
    private void btnShowEarths_Click(object sender, RoutedEventArgs e)
    {
      BrowseClick_Parameter(new WindowSelectGrid<stp_gnt_earth_selResult>(), selectedRecord, new stp_gnt_earth_selResult() { gnt_earth_gnt_creditor_id = selectedRecord.gnt_creditor_id }, "", typeof(frm_gnt_earth), null);
    }
    private void btnPrintCreditorsList_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        CalculateAccountForAllCreditors(false);
        CalculateEarthCountForAllCreditors(false);
        var printForm = new WindowPrint<tbl_gnt_creditor, stp_gnt_creditor_selResult>(new gnt_rpt_creditor_list());
        printForm.articleList = allRecords;
        printForm.selectedRecord = null;
        printForm.ShowDialog();
      }
      catch (Exception exception)
      {
        Messages.ErrorMessage(exception.CompleteMessages());
      }
    }
    private void btnPrintHelps_Click(object sender, RoutedEventArgs e)
    {
      var db = DDB.NewContext();
      var list = db.tbl_gnt_creditor.Where(x => x.gnt_creditor_account_number.Trim() != "").ToList();
      ShowHelpsReport(list);
    }
    private void btnPrintDebtors_Click(object sender, RoutedEventArgs e)
    {
      var db = DDB.NewContext();
      var list = db.tbl_gnt_creditor.OrderBy(x => x.gnt_creditor_child_code).ToList().Where(x => x.gnt_creditor_account_remaining < 0).ToList();
      ShowDebtorsReport(list);
    }
    private void btnPrintAllCreditors_Click(object sender, RoutedEventArgs e)
    {
      var db = DDB.NewContext();
      var list = db.tbl_gnt_creditor.OrderBy(x => x.gnt_creditor_child_code).ToList();
      ShowAllCreditorsReport(list);
    }
    private static void ShowHelpsReport(List<tbl_gnt_creditor> creditorsList)
    {
      try
      {
        var db = DDB.NewContext();
        var printForm = new WindowPrint<tbl_gnt_settings, tbl_gnt_creditor>(new gnt_rpt_creditor_helps());
        printForm.articleList = creditorsList;

        creditorsList.ForEach
        (
            x =>
            {
              if (x.gnt_creditor_mobile == null || x.gnt_creditor_mobile.Trim() == "")
                x.gnt_creditor_mobile = x.gnt_creditor_tel;
            }
        );

        var setting = db.tbl_gnt_settings.First();
        printForm.selectedRecord = setting;
        int helpPrice = 0;
        if (setting.gnt_settings_help_price.HasValue)
          helpPrice = setting.gnt_settings_help_price.Value;

        printForm.AddCustomParameter("price", helpPrice.DigitGrouping());
        printForm.AddCustomParameter("Title1", "سازمان جهاد کشاورزی استان اصفهان");
        printForm.AddCustomParameter("Title2", "مدیریت جهاد کشاورزی شهرستان نجف آباد");
        printForm.AddCustomParameter("Title3", "لیست پرداخت کمک های بلاعوض به خسارت دیدگان ناشی از خشکسالی سال");
        printForm.ShowDialog();
      }
      catch (Exception exception)
      {
        Messages.ErrorMessage(exception.CompleteMessages());
      }
    }
    private static void ShowDebtorsReport(List<tbl_gnt_creditor> creditorsList)
    {
      try
      {
        var db = DDB.NewContext();
        var printForm = new WindowPrint<tbl_gnt_settings, tbl_gnt_creditor>(new gnt_rpt_creditor_debtors());
        printForm.articleList = creditorsList;
        printForm.selectedRecord = db.tbl_gnt_settings.First();
        printForm.AddCustomParameter("Title2", "لیست مانده بدهی سهامداران شرکت در سال " + GlobalVariables.current_fiscal_year_name);
        printForm.ShowDialog();
      }
      catch (Exception exception)
      {
        Messages.ErrorMessage(exception.CompleteMessages());
      }
    }
    private static void ShowAllCreditorsReport(List<tbl_gnt_creditor> creditorsList)
    {
      try
      {
        var db = DDB.NewContext();
        var printForm = new WindowPrint<tbl_gnt_settings, tbl_gnt_creditor>(new gnt_rpt_all_creditors());
        printForm.articleList = creditorsList;
        printForm.selectedRecord = db.tbl_gnt_settings.First();
        printForm.AddCustomParameter("Title2", "لیست مشخصات کل سهامداران شرکت ");
        printForm.ShowDialog();
      }
      catch (Exception exception)
      {
        Messages.ErrorMessage(exception.CompleteMessages());
      }
    }
    private void btnAdvancedSearch_Click(object sender, RoutedEventArgs e)
    {
      AdvancedSearch();
    }
    private void txtAdvancedSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key != System.Windows.Input.Key.Enter)
        return;
      AdvancedSearch();
    }
    private void txtSimpleSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key != System.Windows.Input.Key.Enter)
        return;
      SimpleSearch();
    }
    private void tlt_gnt_creditor_identity_no_TextChanged(object sender, TextChangedEventArgs e)
    {
      lbl_identity_no_length.Content = tlt_gnt_creditor_identity_no.Text.Length.ToString();
    }
    private void txt_gnt_creditor_national_code_TextChanged(object sender, TextChangedEventArgs e)
    {
      if ((sender as TextBox).Text.Length == 10)
        img_national_code.XImage = APMComponents.ImageType.Correct;
      else
        img_national_code.XImage = APMComponents.ImageType.Wrong;

    }
    private void txt_gnt_creditor_postalcode_TextChanged(object sender, TextChangedEventArgs e)
    {
      if ((sender as TextBox).Text.Length == 10)
        img_postal_code.XImage = APMComponents.ImageType.Correct;
      else
        img_postal_code.XImage = APMComponents.ImageType.Wrong;

    }
    private void txt_gnt_creditor_tel_TextChanged(object sender, TextChangedEventArgs e)
    {
      if ((sender as TextBox).Text.Length == 8)
        img_phone.XImage = APMComponents.ImageType.Correct;
      else
        img_phone.XImage = APMComponents.ImageType.Wrong;

    }
    private void txt_gnt_creditor_mobile_TextChanged(object sender, TextChangedEventArgs e)
    {
      if ((sender as TextBox).Text.Length == 11)
        img_mobile.XImage = APMComponents.ImageType.Correct;
      else
        img_mobile.XImage = APMComponents.ImageType.Wrong;

    }
    private void txt_gnt_creditor_account_number_TextChanged(object sender, TextChangedEventArgs e)
    {
      if ((sender as TextBox).Text.Length == 9)
        img_account.XImage = APMComponents.ImageType.Correct;
      else
        img_account.XImage = APMComponents.ImageType.Wrong;

    }
    private void txt_GotFocus(object sender, RoutedEventArgs e)
    {
      if (!(sender is TextBox))
        return;
      (sender as TextBox).SelectAll();
    }
    private void txt_gnt_creditor_birth_date_GotFocus(object sender, RoutedEventArgs e)
    {
      txt_gnt_creditor_birth_date.XDateTextBox.SelectAll();
    }
    private void btnShowServices_Click(object sender, RoutedEventArgs e)
    {
      new frm_gnt_service(selectedRecord).ShowDialog();
    }
    private void btnRefreshEmptyCodes_Click(object sender, RoutedEventArgs e)
    {
      var db = DDB.NewContext();
      var codes = db.tbl_gnt_creditor.Select(x => x.gnt_creditor_child_code).Cast<int>().ToList();
      codes.Sort();
      int lastCode = 0;
      lst_empty_codes.Items.Clear();
      foreach (int code in codes)
      {
        if (code > lastCode + 1)
          for (int j = lastCode + 1; j < code; j++)
            lst_empty_codes.Items.Add(j.ToString());
        lastCode = code;
      }
      lblEmptyCodesCount.Content = lst_empty_codes.Items.Count.ToString();
    }
    private void btnShowAllAccounts_Click(object sender, RoutedEventArgs e)
    {
      CalculateAccountForAllCreditors(false);
      dataGrid.Items.Refresh();
    }
    private void btnShowAllEarthCounts_Click(object sender, RoutedEventArgs e)
    {
      CalculateEarthCountForAllCreditors(false);
      dataGrid.Items.Refresh();
    }
    private void brwFilterGroup_XBrowseClick(object sender, RoutedEventArgs e)
    {
      var group = BrowseClick(new WindowSelectGrid<stp_gnt_creditor_group_selResult>(), "گروه سهامداران", typeof(frm_gnt_creditor_group), sender);
      if (group != null)
      {
        filterGroupId = group.gnt_creditor_group_id;
        brwFilterGroup.XLabel.Content = group.gnt_creditor_group_name;
        brwFilterGroup.XLabel.Visibility = System.Windows.Visibility.Visible;
        AdvancedSearch();
      }
    }
    private void btnShowService_masters_Click(object sender, RoutedEventArgs e)
    {
      new frm_gnt_service_masters().ShowDialog();
    }
    private void cmbSearchTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      AdvancedSearch();
    }
    private void btnPrintStockSheet_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var db = DDB.NewContext();
        var printForm = new WindowPrint<tbl_gnt_creditor, tbl_gnt_creditor>(new gnt_rpt_creditor_stock());
        var settings = db.tbl_gnt_settings.FirstOrDefault();
        var creditor = db.tbl_gnt_creditor.Where(x => x.gnt_creditor_id == selectedRecord.gnt_creditor_id).FirstOrDefault();
        creditor.gnt_creditor_birth_date = APMDateTime.dateWithSlash(creditor.gnt_creditor_birth_date);
        printForm.selectedRecord = creditor;
        printForm.AddCustomParameter("chairman", settings.gnt_settings_chairman_name);
        printForm.AddCustomParameter("executive_manager", settings.gnt_settings_executive_manager_name);
        printForm.ShowDialog();
      }
      catch (Exception exception)
      {
        Messages.ErrorMessage(exception.Message);
      }
    }
    #endregion


  }
}


