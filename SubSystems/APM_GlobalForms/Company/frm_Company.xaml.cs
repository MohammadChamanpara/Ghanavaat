using DataAccessLayer;
using UserInterfaceLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_Company : WindowEntityGroup<stp_glb_company_selResult,stp_glb_company_in_group_selResult,stp_glb_company_group_selResult>
    {
        ArticlePackage<stp_glb_company_group_selResult, stp_glb_company_in_group_selResult> glb_company_group_ref = new WindowBase<stp_glb_company_selResult>.ArticlePackage<stp_glb_company_group_selResult, stp_glb_company_in_group_selResult>();
        public frm_Company()
        {
            InitializeComponent();
            Initial_WindowEntityGroup(dbgCompany, windowToolbar, grpInfo, "glb_company", true,null, (long)EntityType.glb_company, txt_glb_company_child_code, cmb_glb_company_glb_company_group_id);
        }
        private void APMBrowser_XBrowseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_glb_company_group_selResult>(), ref glb_company_group_ref, "گروه شرکت",typeof(frm_CompanyGroup));
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            glb_company_group_ref.Save(selectedRecord);
        }
    }
}