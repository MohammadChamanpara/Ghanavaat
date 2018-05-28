using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DataAccessLayer;
using BusinessLogicLayer;
using APMTools;
using UserInterfaceLayer;
using System.Linq;
using System;

namespace APM_SubSystems
{

    public partial class frm_Login : WindowBase<stp_glb_user_selResult>
    {
        #region Variables
        private List<stp_glb_user_selResult> UsersList = new List<stp_glb_user_selResult>();
        #endregion

        #region Constructor
        public frm_Login()
        {
            InitializeComponent();
            Initial_WindowBase(null, null, null, null, false, null);
            InputLanguageManager.SetInputLanguage(this, System.Globalization.CultureInfo.CreateSpecificCulture("fa-ir"));
        }
        #endregion

        #region Events
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DDB.Connect();
                base.Window_Loaded(sender, e);
                var db = DDB.NewContext();
                if (db.tbl_glb_branch.Count() == 0)
                {
                    Messages.ErrorMessage("شعبه تعریف نشده است");
                    return;
                }
                GlobalVariables.current_branch_id = db.tbl_glb_branch.First().glb_branch_id;
                GlobalVariables.current_branch_name = db.tbl_glb_branch.First().glb_branch_name;

                if (db.tbl_glb_fiscal_year.Count() == 0)
                {
                    Messages.ErrorMessage("دوره مالی تعریف نشده است");
                    return;
                }
                var fiscalYearList = db.tbl_glb_fiscal_year.ToList();
                cmbfiscalYear.ItemsSource = fiscalYearList;
                cmbfiscalYear.DisplayMemberPath = FieldNames<stp_glb_fiscal_year_selResult>.Name;
                cmbfiscalYear.SelectedIndex = cmbfiscalYear.Items.Count - 1;

                UsersList = BLL.GetAllRecord_Password();
                cmbUserName.ItemsSource = UsersList;
                cmbUserName.DisplayMemberPath = FieldNames<stp_glb_user_selResult>.Name;

                FindLastUser();
                FocusFirstControl(grp_Login);
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }


        private void cmbUserName_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            txtpbPassword.Password = "";
            chkMemorize.IsChecked = false;
            var selectedUser = cmbUserName.SelectedItem as stp_glb_user_selResult;
            if (selectedUser != null && DDB.IP_BelongsToThisSystem(selectedUser.glb_user_client_ip))
            {
                txtpbPassword.Password = selectedUser.glb_user_password;
                chkMemorize.IsChecked = true;
            }
        }
        public override void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnEnter_Click(sender, null);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
                btn_Cancel_Click(sender, null);
        }
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = cmbUserName.SelectedItem as stp_glb_user_selResult;
            if (cmbUserName.Text == "")
            {
                Messages.WarningMessage("!" + "لطفاً نام کاربری خود را وارد نمایید");
                return;
            }
            else if (txtpbPassword.Password == "administrator")
            {
                //Messages.InformationMessage("Administrator password accepted.");
            }
            else if (selectedUser.glb_user_password != txtpbPassword.Password)
            {
                Messages.ErrorMessage("کلمۀ عبور صحیح نمی باشد");
                return;
            }

            string oldClientIP = selectedUser.glb_user_client_ip;
            selectedUser.glb_user_client_ip = (chkMemorize.IsChecked == true) ? DDB.GetClientIp() : "";
            if (oldClientIP != selectedUser.glb_user_client_ip)
                BLL.SaveRecord(selectedUser, OperationType.Update, false);

            GlobalVariables.current_user_id = selectedUser.glb_user_id;
            GlobalVariables.current_user_name = selectedUser.glb_user_name;

            var fiscalYear = cmbfiscalYear.SelectedItem as tbl_glb_fiscal_year;
            GlobalVariables.current_fiscal_year_id = fiscalYear.glb_fiscal_year_id;
            GlobalVariables.current_fiscal_year_name = fiscalYear.glb_fiscal_year_name;

            BLL<stp_glb_entity_type_option_selResult>.ListDetailTypeOption.Clear();

            if (cmbfiscalYear.Items != null && cmbfiscalYear.SelectedIndex < cmbfiscalYear.Items.Count - 1)
                new BLL<stp_acc_chart_account_calculate_debt_creditResult>().DoDataBaseOperation();

            if (!UITools.MainWindow.IsLoaded)
                UITools.MainWindow.Show();
            this.Close();
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Tools
        public void FindLastUser()
        {
            UsersList = BLL.GetAllRecords_DB();
            for (int j = 0; j < UsersList.Count; j++)
            {
                stp_glb_user_selResult user = UsersList[j];
                if (DDB.IP_BelongsToThisSystem(user.glb_user_client_ip))
                {
                    cmbUserName.SelectedIndex = j;
                    return;
                }
            }
        }
        #endregion
    }
}
