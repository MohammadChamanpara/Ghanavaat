using System.Windows.Controls;
using APMTools;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
namespace APM_SubSystems
{
    public partial class frm_UserGroup : WindowBase<stp_glb_user_group_selResult>
    {
        #region Variables
        ArticlePackage<stp_glb_user_selResult, stp_glb_user_in_group_selResult> glb_user_ref
            = new WindowBase<stp_glb_user_group_selResult>.ArticlePackage<stp_glb_user_selResult, stp_glb_user_in_group_selResult>();

        #endregion

        public frm_UserGroup()
        {
            InitializeComponent();
            Initial_WindowBase(dbg_glb_user, windowToolbar, grp_glb_user_group, "glb_user_group",true, null);
            
        }

        private void APMBrowser_XBrowseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_glb_user_selResult>(), ref glb_user_ref, "کاربر",typeof(frm_User));
        }

        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            glb_user_ref.Save(selectedRecord);
        }

    }
}