using System.Windows;
using APM_SubSystems;
using DataAccessLayer;
using APMComponents;
using APMTools;
using APM_Accounting;
using BusinessLogicLayer;
using System.Windows.Controls;
using System.Linq;
using System;
using Microsoft.Win32;

namespace APM_SubSystems
{
    public partial class frm_main : Window
    {
        #region Variables
        SahaamEntities context = DDB.NewContext();
        #endregion

        #region Tools
        public void CreateUserAccessItemsFromButtons()
        {
            try
            {

                foreach (var record in context.tbl_glb_user_access_items.ToList())
                    if (record.tbl_glb_user_access.Count == 0)
                        context.tbl_glb_user_access_items.DeleteObject(record);

                context.SaveChanges();

                CreateUserAccessItemsFromButtons_Rec(SubSystemsPanel, "سیستم ها");
                CreateUserAccessItemsFromButtons_Rec(grpGlobal, "اطلاعات پایه");
                CreateUserAccessItemsFromButtons_Rec(grpAccounting, "حسابداری");
                CreateUserAccessItemsFromButtons_Rec(grpInventory, "انبارداری");
                CreateUserAccessItemsFromButtons_Rec(grpInventoryAccounting, "حسابداری انبار");
                CreateUserAccessItemsFromButtons_Rec(grpSahaam, "سهام");
                CreateUserAccessItemsFromButtons_Rec(grpTools, "ابزار ها و تنظیمات");

                context.SaveChanges();
                Messages.SuccessMessage("عملیات به روز رسانی کلیه آیتم ها");
            }
            catch (Exception exception)
            {
                Messages.ExceptionMessage(exception);
            }
        }
        private void CreateUserAccessItemsFromButtons_Rec(UIElement element, string subSystem)
        {
            if (element is APMSubSystemButton)
            {
                var button = (element as APMSubSystemButton);
                SaveUserAccessItem(subSystem, button.Name, button.Xcaption);
            }
            else if (element is APMButton)
            {
                var button = element as APMButton;
                SaveUserAccessItem(subSystem, button.Name, (button.Content != null) ? button.Content.ToString() : "");
            }

            else if (element is DockPanel)
            {
                foreach (UIElement child in (element as DockPanel).Children)
                    CreateUserAccessItemsFromButtons_Rec(child, subSystem);
            }
            else if (element is GroupBox)
            {
                if ((element as GroupBox).Content is UIElement)
                    CreateUserAccessItemsFromButtons_Rec((element as GroupBox).Content as UIElement, subSystem);
            }
            else if (element is ScrollViewer)
            {
                if ((element as ScrollViewer).Content is UIElement)
                    CreateUserAccessItemsFromButtons_Rec((element as ScrollViewer).Content as UIElement, subSystem);
            }
            else if (element is StackPanel)
            {
                foreach (UIElement child in (element as StackPanel).Children)
                    CreateUserAccessItemsFromButtons_Rec(child, subSystem);
            }
            else if (element is APMExpander)
            {
                var expander = (element as APMExpander);
                SaveUserAccessItem(subSystem, expander.Name, expander.XCaption);
                if (expander.Content is UIElement)
                    CreateUserAccessItemsFromButtons_Rec(expander.Content as UIElement, subSystem);
            }
            else if (element is Border)
            {
                if ((element as Border).Child is UIElement)
                    CreateUserAccessItemsFromButtons_Rec((element as Border).Child as UIElement, subSystem);
            }
        }
        private void SaveUserAccessItem(string subSystem, string controlName, string caption)
        {
            if (controlName == null || controlName == "")
                return;
            tbl_glb_user_access_items record;

            var existingRecords = context.tbl_glb_user_access_items
                .Where(x => x.glb_user_access_items_control_name == controlName);

            if (existingRecords.Count() > 0)
                record = existingRecords.First();
            else
                record = new tbl_glb_user_access_items();

            record.glb_user_access_items_name = caption;
            record.glb_user_access_items_subsystem = subSystem;
            record.glb_user_access_items_control_name = controlName;
            record.glb_user_access_items_glb_branch_id = GlobalVariables.current_branch_id;

            if (existingRecords.Count() > 0)
                context.ApplyCurrentValues(context.tbl_glb_user_access_items.EntitySet.Name, record);
            else
                context.tbl_glb_user_access_items.AddObject(record);
        }
        private void HideInaccessibleButtons()
        {

            HideInaccessibleButtons_Rec(main_DockPanel);

            if (!btn_global_subsystem.IsVisible)
                grpGlobal.Visibility = Visibility.Collapsed;

            if (!btn_sahaam_subsystem.IsVisible)
                grpSahaam.Visibility = Visibility.Collapsed;

            if (!btn_tools_subsystem.IsVisible)
                grpTools.Visibility = Visibility.Collapsed;

            if (!btn_inventory_accounting_subsystem.IsVisible)
                grpInventoryAccounting.Visibility = Visibility.Collapsed;

            if (!btn_inventory_subsystem.IsVisible)
                grpInventory.Visibility = Visibility.Collapsed;

            if (!btn_accounting_subsystem.IsVisible)
                grpAccounting.Visibility = Visibility.Collapsed;
        }
        private void HideInaccessibleButtons_Rec(UIElement element)
        {
            if (element is APMSubSystemButton)
            {
                var button = (element as APMSubSystemButton);
                if (!UITools.CurrentUserHasAccessTo(button.Name))
                    button.Visibility = Visibility.Collapsed;
                else
                    button.Visibility = Visibility.Visible;

            }
            else if (element is APMButton)
            {
                var button = element as APMButton;
                if (!UITools.CurrentUserHasAccessTo(button.Name))
                    button.Visibility = Visibility.Collapsed;
                else
                    button.Visibility = Visibility.Visible;
            }

            else if (element is DockPanel)
            {
                foreach (UIElement child in (element as DockPanel).Children)
                    HideInaccessibleButtons_Rec(child);
            }
            else if (element is GroupBox)
            {
                if ((element as GroupBox).Content is UIElement)
                    HideInaccessibleButtons_Rec((element as GroupBox).Content as UIElement);
            }
            else if (element is ScrollViewer)
            {
                if ((element as ScrollViewer).Content is UIElement)
                    HideInaccessibleButtons_Rec((element as ScrollViewer).Content as UIElement);
            }
            else if (element is StackPanel)
            {
                foreach (UIElement child in (element as StackPanel).Children)
                    HideInaccessibleButtons_Rec(child);
            }
            else if (element is APMExpander)
            {
                var expander = (element as APMExpander);

                if (!UITools.CurrentUserHasAccessTo(expander.Name))
                    expander.Visibility = Visibility.Collapsed;
                else
                    expander.Visibility = Visibility.Visible;

                if (expander.Content is UIElement)
                    HideInaccessibleButtons_Rec(expander.Content as UIElement);
            }
            else if (element is Border)
            {
                if ((element as Border).Child is UIElement)
                    HideInaccessibleButtons_Rec((element as Border).Child as UIElement);
            }
        }
        private void ChangeBranchOperation()
        {
            UITools.Create_UserEntityButtons();
        }
        #endregion

        #region Window Events
        public frm_main()
        {
            InitializeComponent();
            grpGlobal.Visibility = Visibility.Collapsed;
            grpAccounting.Visibility = Visibility.Collapsed;
            grpInventory.Visibility = Visibility.Collapsed;
            grpInventoryAccounting.Visibility = Visibility.Collapsed;
            grpSahaam.Visibility = Visibility.Collapsed;
            grpTools.Visibility = Visibility.Collapsed;
            grpGlobal.Visibility = Visibility.Collapsed;

        }
        private void Window_Activated(object sender, System.EventArgs e)
        {
            lbl_glb_barnch_name.Content = GlobalVariables.current_branch_name;
            lbl_glb_user_name.Content = GlobalVariables.current_user_name;
            lbl_glb_fiscal_year_name.Content = GlobalVariables.current_fiscal_year_name;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DDB.Connect();
            this.WindowState = WindowState.Maximized;
            ChangeBranchOperation();
            HideInaccessibleButtons();
            //CreateUserAccessItemsFromButtons();
        }
        private void Window_Closed(object sender, System.EventArgs e)
        {
            App.Current.Shutdown();
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F11)
                new frm_gnt_creditor().Show();
        }
        #endregion

        #region HeaderButtons
        private void btnGlobal_Click(object sender, RoutedEventArgs e)
        {
            grpGlobal.Visibility = (grpGlobal.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }
        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            grpInventory.Visibility = (grpInventory.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }
        private void btnInventoryAccounting_Click(object sender, RoutedEventArgs e)
        {
            grpInventoryAccounting.Visibility = (grpInventoryAccounting.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }
        private void btnAccounting_Click(object sender, RoutedEventArgs e)
        {
            grpAccounting.Visibility = (grpAccounting.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }
        private void btnSahaam_Click(object sender, RoutedEventArgs e)
        {
            grpSahaam.Visibility = (grpSahaam.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }
        private void btnTools_Click(object sender, RoutedEventArgs e)
        {
            grpTools.Visibility = (grpTools.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

        }
        private void btn_inv_login_Click(object sender, RoutedEventArgs e)
        {
            (new frm_Login()).ShowDialog();
            ChangeBranchOperation();
            HideInaccessibleButtons();
        }

        #endregion

        #region Global Subsystem
        private void btn_glb_cost_center_click(object sender, RoutedEventArgs e)
        {
            new frm_CostCenter().Show();
        }
        private void btn_glb_measure_Click(object sender, RoutedEventArgs e)
        {
            new frm_glb_Measure().Show();
        }
        private void btn_glb_user_access_Click(object sender, RoutedEventArgs e)
        {
            new frm_UserAccess().Show();
        }
        private void btn_glb_user_access_items_Click(object sender, RoutedEventArgs e)
        {
            new frm_UserAccessItems().Show();
        }
        private void btn_glb_user_Click(object sender, RoutedEventArgs e)
        {
            new frm_User().Show();
        }
        private void btn_glb_FiscalYear_Click(object sender, RoutedEventArgs e)
        {
            (new frm_glb_fiscal_year()).ShowDialog();
        }
        private void btn_glb_Project_Click(object sender, RoutedEventArgs e)
        {
            (new frm_Project()).ShowDialog();
        }
        private void btn_glb_Personel_Click(object sender, RoutedEventArgs e)
        {
            (new frm_Personel()).ShowDialog();
        }
        private void btn_Personel_group_Click(object sender, RoutedEventArgs e)
        {
            (new frm_personel_group()).ShowDialog();
        }
        private void btn_glb_Person_Click(object sender, RoutedEventArgs e)
        {
            (new frm_Person()).ShowDialog();
        }
        private void btn_glb_PersonGroup_Click(object sender, RoutedEventArgs e)
        {
            (new frm_Person_Group()).ShowDialog();
        }
        private void btn_glb_Company_Click(object sender, RoutedEventArgs e)
        {
            new frm_Company().Show();
        }
        private void btn_glb_Company_Group_Click(object sender, RoutedEventArgs e)
        {
            new frm_CompanyGroup().Show();
        }
        private void btn_glb_cost_cntr_Click(object sender, RoutedEventArgs e)
        {
            new frm_CostCenter().Show();
        }
        private void btn_glb_entity_Click(object sender, RoutedEventArgs e)
        {
            new frm_entity().Show();
        }
        private void btn_glb_user_group_Click(object sender, RoutedEventArgs e)
        {
            new frm_UserGroup().Show();
        }
        private void btn_cash_Click(object sender, RoutedEventArgs e)
        {
            new frm_glb_cash().Show();
        }
        private void btn_bank_Click(object sender, RoutedEventArgs e)
        {
            (new frm_glb_bank()).ShowDialog();
        }
        private void btn_bank_branch_Click(object sender, RoutedEventArgs e)
        {
            (new frm_glb_bank_branch()).ShowDialog();
        }
        private void btn_bank_account_Click(object sender, RoutedEventArgs e)
        {
            (new frm_glb_bank_account()).ShowDialog();
        }
        private void btn_bank_account_type_Click(object sender, RoutedEventArgs e)
        {
            (new frm_glb_bank_account_type()).ShowDialog();
        }
        private void btn_inv_create_closing_document_Click(object sender, RoutedEventArgs e)
        {
            new frm_glb_fiscal_year().Show();
        }

        #endregion

        #region Inventory Subsystem
        private void btn_inv_goods_send_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_send(false, false).ShowDialog();
        }
        private void btn_goods_group_options_Click(object sender, RoutedEventArgs e)//تنظیمات پایه انبار
        {
            new frm_inv_options().Show();
        }
        private void btn_inv_attribute_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_attribute().Show();
        }
        private void btn_inv_group_goods_Click(object sender, RoutedEventArgs e)
        {
            new frm_group_goods().Show();
        }
        private void btn_inv_store_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_store().Show();
        }
        private void btn_inv_goods_request_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_request().Show();
        }
        private void btn_inv_buy_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_buy_request().Show();
        }
        private void btn_inv_goods_receipt_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_receive(false, false).ShowDialog();
        }
        private void btn_inv_opening_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_receive(false, true).ShowDialog();
        }
        private void btn_inv_closing_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_send(false, true).ShowDialog();
        }
        private void btn_inv_physical_counting_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_physical_counting().Show();
        }
        private void btn_inv_product_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_product().Show();
        }
        private void btn_inv_goods_glb_measure_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_glb_measure().Show();
        }

        #region Reports
        private void btn_inv_rpt_goods_opening_all_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_opening_all().Show();
        }
        private void btn_inv_rpt_goods_receive_all_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_receive_all().Show();
        }
        private void btn_inv_rpt_buy_request_all_Click(object sender, RoutedEventArgs e)
        {
            new frm_rpt_inv_buy_request_all().Show();
        }
        private void btn_inv_rpt_goods_send_all_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_send_all().Show();
        }
        private void btn_inv_rpt_goods_request_all_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_request_all().Show();
        }
        private void btn_inv_rpt_goods_cardex_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_cardex().Show();
        }
        private void btn_inv_rpt_goods_stock_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_stock().Show();
        }
        private void btn_inv_rpt_goods_stock_all_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_stock_all().Show();
        }
        private void btn_inv_rpt_goods_send_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_send().Show();
        }

        private void btn_inv_rpt_goods_receive_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_rpt_goods_receive().Show();
        }

        #endregion

        #endregion

        #region Accounting Subsystem
        private void btn_accounting_options_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_options().Show();
        }
        private void btn_acc_document_type_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_document_type().Show();
        }
        private void btn_acc_chart_account_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_chart_account().Show();
        }
        private void btn_glb_entity_type_Click(object sender, RoutedEventArgs e)
        {
            new frm_entity(true).ShowDialog();
        }
        private void btn_acc_detail_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_detail().Show();
        }
        private void btn_acc_document_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_document().Show();
        }
        private void btn_acc_rpt_account_balance_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_account_balance().Show();
        }
        private void btn_acc_rpt_chart_balance_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_chart_balance().Show();
        }
        private void btn_acc_rpt_chart_balance_all_levels_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_chart_balance_all_levels().Show();
        }
        private void btn_acc_rpt_cover_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_cover().Show();
        }

        private void btn_acc_rpt_notebooks_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_notebooks().Show();
        }
        private void btn_acc_rpt_daily_notebooks_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_daily_notebooks().Show();
        }
        private void btn_acc_rpt_kol_notebook_rule_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_kol_notebook_rule().Show();
        }
        private void btn_acc_rpt_balance_4columns_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_balance_4columns().Show();
        }
        #endregion

        #region Accounting_Inventory Subsystem
        private void btn_inv_create_accounting_document_multi_select_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_create_accounting_document_multi_select().Show();
        }
        private void btn_inv_receive_financial_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_receive(true, false).ShowDialog();
        }
        private void btn_inv_send_financial_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_send(true, false).ShowDialog();
        }
        private void btn_inv_opening_financial_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_receive(true, true).ShowDialog();
        }
        private void btn_inv_closing_financial_Click(object sender, RoutedEventArgs e)
        {
            new frm_inv_goods_send(true, true).ShowDialog();
        }
        #endregion

        #region Sahaam
        private void btn_gnt_creditor_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_creditor().Show();
        }
        private void btn_gnt_creditor_group_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_creditor_group().Show();
        }
        private void btn_gnt_water_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_water().Show();
        }
        private void btn_gnt_water2_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_water2().Show();
        }
        private void btn_gnt_water1_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_water1().Show();
        }
        private void btn_gnt_deal_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_deal().Show();
        }
        private void btn_gnt_earth_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_earth().Show();
        }
        private void btn_gnt_settings_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_settings().Show();
        }
        private void btn_gnt_cost_type_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_cost_type().Show();
        }
        private void btn_gnt_rpt_creditor_deals_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_rpt_creditor_deals().Show();
        }
        private void btn_service_masters_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_service_masters().Show();
        }
        #endregion

        #region ToolsAndSettings
        private void btn_acc_inv_tools_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_inv_tools().Show();
        }
        private void btn_backup_Click(object sender, RoutedEventArgs e)
        {
            new frm_Backup().Show();
        }
        private void btn_gnt_tools_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_tools().Show();
        }
        private void btn_acc_tools_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_tools().Show();
        }
        #endregion
    }
}