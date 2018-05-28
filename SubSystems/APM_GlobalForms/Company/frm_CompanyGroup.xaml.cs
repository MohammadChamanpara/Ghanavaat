using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DataAccessLayer;
using APMTools;
using BusinessLogicLayer;
using UserInterfaceLayer;
using APMComponents;


namespace APM_SubSystems
{
    public partial class frm_CompanyGroup : WindowBase<stp_glb_company_group_selResult>
    {
        #region Variables
        ArticlePackage<stp_glb_company_selResult, stp_glb_company_in_group_selResult> glb_company_ref = new WindowBase<stp_glb_company_group_selResult>.ArticlePackage<stp_glb_company_selResult, stp_glb_company_in_group_selResult>();
        #endregion

        #region Constructor
        public frm_CompanyGroup()
        {
            InitializeComponent();
            Initial_WindowBase(dbg_glb_company_group, tbr_glb_company_group, grp_glb_company_group, "glb_company_group", true, null);
        }
        #endregion

        #region Override

        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            glb_company_ref.Save(selectedRecord);
        }
        #endregion

        #region XBrowseClick

        private void APMBrowser_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_glb_company_selResult>(), ref glb_company_ref, "شرکت",typeof(frm_Company));
        }
        #endregion
    }
}
