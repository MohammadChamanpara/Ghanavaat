using System.Windows.Controls;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;

namespace APM_SubSystems
{
    /// <summary>
    /// Interaction logic for frm_personel_group.xaml
    /// </summary>
    public partial class frm_personel_group : WindowBase<stp_glb_personel_group_selResult>
    {
        #region Variables
        ArticlePackage<stp_glb_personel_selResult, stp_glb_personel_in_group_selResult> glb_personel = new ArticlePackage<stp_glb_personel_selResult, stp_glb_personel_in_group_selResult>();
        #endregion

        #region Constructor
        public frm_personel_group()
        {
            InitializeComponent();
            Initial_WindowBase(dbg_Personel_group, tbr_glb_personel_group, grp_glb_personel_group, "glb_personel_group", true, null);
        }
        #endregion

        #region XBrowseClick

        private void APMBrowser_XBrowseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_glb_personel_selResult>(), ref glb_personel, "پرسنل",typeof(frm_Personel));
        }
        #endregion

        #region Override
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            glb_personel.Save(selectedRecord);
        }
        #endregion

    }
}
