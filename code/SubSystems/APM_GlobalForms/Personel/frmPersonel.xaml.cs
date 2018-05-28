using System;
using System.Windows;
using DataAccessLayer;
using UserInterfaceLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_Personel : WindowEntityGroup<stp_glb_personel_selResult,stp_glb_personel_in_group_selResult,stp_glb_personel_group_selResult>
    {
        #region Variables
        ArticlePackage<stp_glb_personel_group_selResult, stp_glb_personel_in_group_selResult> glb_personel_group =
            new WindowBase<stp_glb_personel_selResult>.ArticlePackage<stp_glb_personel_group_selResult, stp_glb_personel_in_group_selResult>();
        #endregion

        #region Constructor
        public frm_Personel()
        {
            InitializeComponent();
            Initial_WindowEntityGroup(dbgPersonel, windowToolbar, grpInfo, "glb_personel", true, null, (long)EntityType.glb_personel, tpc_glb_personel_child_code, cmb_glb_personel_glb_personel_group_id);
        }
        #endregion

        #region XBrowseClick
        private void lblPersonelCostCntr_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_glb_cost_center_selResult>(), "مرکز هزینه", typeof(frm_CostCenter), sender);
        }
        private void brw_glb_personel_glb_personel_group_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_glb_personel_group_selResult>(), ref glb_personel_group, "گروه پرسنلی", typeof(frm_personel_group));
        }

        #endregion

        #region Override
        public override void InitializationBeforeSave()
        {
            base.InitializationBeforeSave();
            selectedRecord.glb_personel_name = cmb_glb_personel_title_glb_coding_id.Text + " " + txt_glb_personel_first_name.Text.Trim() + " " + txt_glb_personel_family.Text.Trim();
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            glb_personel_group.Save(selectedRecord);
        }
        #endregion

        #region TextBoxKeydown
        private void brw_glb_personel_glb_cost_center_id_XTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_glb_cost_center_selResult>(sender, "glb_personel_glb_cost_center_id", e);
        }
        #endregion
    }
}