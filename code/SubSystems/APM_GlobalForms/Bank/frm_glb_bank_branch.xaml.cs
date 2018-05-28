using System.Windows;
using System.Windows.Input;
using UserInterfaceLayer;
using DataAccessLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_glb_bank_branch : WindowEntityGroup<stp_glb_bank_branch_selResult,
                                                                 stp_glb_bank_branch_selResult,
                                                                 stp_glb_bank_selResult>
    {
        #region Initial
        public frm_glb_bank_branch()
        {
            InitializeComponent();
            Initial_WindowEntityGroup(grd_bank_branch, windowToolbar, grpInfo, "glb_bank_branch", true, null, (long)EntityType.glb_bank_branch, tpc_glb_bank_branch_child_code, cmb_glb_bank_branch_glb_bank_id);
        }
        #endregion

        #region Events
        private void APMBrowser_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_glb_bank_selResult>(), "بانک", typeof(frm_glb_bank), sender);
        }

        private void brw_glb_bank_branch_glb_bank_id_XTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_glb_bank_selResult>(sender, "glb_bank_branch_glb_bank_id", e);
        }
        #endregion

        #region Override
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            if (cmb_glb_bank_branch_glb_bank_id.SelectedIndex > 0)
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, (stp_glb_bank_selResult)cmb_glb_bank_branch_glb_bank_id.SelectedItem);
        }
        #endregion
    }
}
