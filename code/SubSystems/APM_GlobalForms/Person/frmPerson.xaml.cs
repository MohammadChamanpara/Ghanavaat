using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DataAccessLayer;
using APMTools;
using BusinessLogicLayer;
using UserInterfaceLayer;

namespace APM_SubSystems
{
    public partial class frm_Person : WindowEntityGroup<stp_glb_person_selResult,stp_glb_person_in_group_selResult,stp_glb_person_group_selResult>
    {
        #region Variables
        ArticlePackage<stp_glb_person_group_selResult, stp_glb_person_in_group_selResult> glb_person_group = new WindowBase<stp_glb_person_selResult>.ArticlePackage<stp_glb_person_group_selResult, stp_glb_person_in_group_selResult>();
        #endregion

        #region Constructor
        public frm_Person()
        {
            InitializeComponent();
            Initial_WindowEntityGroup(dbgPerson, windowToolbar, grpInfo, "glb_person", true, null, (long)EntityType.glb_person, txt_glb_person_child_code, cmb_glb_person_glb_person_group_id);
        }
        #endregion

        #region override
        public override bool ValidationForSave()
        {
            if (cmb_glb_person_title_glb_coding_id.SelectedIndex == -1)
            {
                Messages.ErrorMessage("لطفا عنوان را انتخاب نمایید");
                return false;
            }
           return  base.ValidationForSave();
        }
        public override void InitializationBeforeSave()
        {
            base.InitializationBeforeSave();
            selectedRecord.glb_person_name = cmb_glb_person_title_glb_coding_id.Text + " " + txt_glb_person_first_name.Text.Trim() + " " + txt_glb_person_family.Text.Trim();
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            glb_person_group.Save(selectedRecord);
        }
        #endregion

        #region XBrowseClick
        private void glb_person_group_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_glb_person_group_selResult>(), ref glb_person_group, "گروهاشخاص",typeof(frm_Person_Group));
        }
        #endregion
    }
}


