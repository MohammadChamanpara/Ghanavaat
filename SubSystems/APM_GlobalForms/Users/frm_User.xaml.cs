using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using APMTools;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
namespace APM_SubSystems
{
    public partial class frm_User : WindowBase<stp_glb_user_selResult>
    {
        #region Variables
        ArticlePackage<stp_glb_user_group_selResult, stp_glb_user_in_group_selResult> glb_user_group_ref =
                        new WindowBase<stp_glb_user_selResult>.ArticlePackage<stp_glb_user_group_selResult, stp_glb_user_in_group_selResult>();
        #endregion

        #region Constructor
        public frm_User()
        {
            InitializeComponent();
            Initial_WindowBase(dbgUsers, windowToolbar, grpInputInfo, "glb_user", false, null);
        }
        #endregion

        #region Overrided Methods

        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            GlobalFunctions.ListToBindingList(BLL.GetAllRecord_Password(), bindingList, collectionView);
        }

        public override bool ValidationForSave()
        {
            if (!Check_user_Password())
                return false;
            return true;
        }
        public override void collectionView_CurrentChanged(object sender, EventArgs e)
        {
            if (operationType != OperationType.Nothing)
                selectedRecord.glb_user_password = psw_glb_user_password.Password;
            base.collectionView_CurrentChanged(sender, e);
            psw_glb_user_password.Password = selectedRecord.glb_user_password;
            psw2_user_password.Password = selectedRecord.glb_user_password;
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            glb_user_group_ref.Save(selectedRecord);
        }
        public override void RefreshClick()
        {
            if (operationType != OperationType.Nothing)
                return;
            GlobalFunctions.ListToBindingList(BLL.GetAllRecord_Password(), bindingList, collectionView);
        }
        #endregion

        #region Event

        #region BrowseClick
        private void brw_user_glb_personel_BrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_glb_personel_selResult, stp_glb_personel_group_selResult>(), "پرسنل", typeof(frm_Personel), sender);
        }
        #endregion

        #region TextBoxKeyDown
        private void brw_user_glb_personel_TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_glb_person_selResult>(sender, FieldNames<stp_glb_user_selResult>.PersonelId, e);
        }
        #endregion

        private void psw2_user_password_LostFocus(object sender, RoutedEventArgs e)
        {
            Check_user_Password();
        }

        private void brw_glb_user_group_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_glb_user_group_selResult>(), ref glb_user_group_ref, "گروه کاربر", typeof(frm_UserGroup));
        }

        private Boolean Check_user_Password()
        {
            if (psw_glb_user_password.Password == psw2_user_password.Password)
            {
                selectedRecord.glb_user_password = psw_glb_user_password.Password;
                return true;
            }
            else
            {
                Messages.ErrorMessage("لطفا کلمه عبور را صحیح وارد نمایید");
                psw2_user_password.Password = string.Empty;
            }
            return false;

        }
        #endregion

    }
}