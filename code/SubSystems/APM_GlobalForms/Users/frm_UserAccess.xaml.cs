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
    public partial class frm_UserAccess : WindowBase<stp_glb_user_access_selResult>
    {
        #region Constructor
        public frm_UserAccess()
        {
            InitializeComponent();
            Initial_WindowBase(dbgUsers, windowToolbar, grpInputInfo, "glb_user_access", false, null);
        }
        #endregion

        #region Overrided Methods

        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            GlobalFunctions.ListToBindingList(BLL.GetAllRecord_Password(), bindingList, collectionView);
        }
        #endregion

        #region Event

        private void brw_glb_user_access_glb_user_id_BrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_glb_user_selResult, stp_glb_user_group_selResult>(), "کاربران", typeof(frm_User), sender);
        }

        private void brw_glb_user_access_glb_user_access_items_id_BrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_glb_user_access_items_selResult>(), "دسترسی محدود شده", typeof(frm_UserAccessItems), sender);
        }
        #endregion
    }
}