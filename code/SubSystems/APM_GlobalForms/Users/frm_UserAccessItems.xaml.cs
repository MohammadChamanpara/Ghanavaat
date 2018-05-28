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
    public partial class frm_UserAccessItems : WindowBase<stp_glb_user_access_items_selResult>
    {
        #region Constructor
        public frm_UserAccessItems()
        {
            InitializeComponent();
            Initial_WindowBase(dbgUsers, windowToolbar, grpInputInfo, "glb_user_access_items", true, null);
        }
        #endregion
        
        #region Event
        private void brw_user_access_items_glb_user_BrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_glb_user_selResult, stp_glb_user_group_selResult>(), "کاربران", typeof(frm_User), sender);
        }
        private void btnUpdateItems_Click(object sender, RoutedEventArgs e)
        {
            new frm_main().CreateUserAccessItemsFromButtons();
        }
        #endregion
    }
}