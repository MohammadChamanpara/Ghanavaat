using System;
using System.Linq;
using DataAccessLayer;
using UserInterfaceLayer;
using BusinessLogicLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_entity : WindowBase<stp_glb_entity_type_option_selResult>
    {
        #region Variables
        stp_glb_entity_type_selResult selectedRecordEntityType = new stp_glb_entity_type_selResult();
        BLL<stp_glb_entity_type_selResult> bllGlbEntityType = new BLL<stp_glb_entity_type_selResult>();
        #endregion

        #region Constractor
        public frm_entity(bool callFromDetailType)
        {
            if (callFromDetailType)
                GlobalVariables.currentSubSystem = SubSystems.Accounting;
            else
                GlobalVariables.currentSubSystem = SubSystems.Inventory;

            InitializeComponent();
            Initial_WindowBase(dbg_entity, tbr_entity, grp_entity, "acc_entity", true, null);
        }
        public frm_entity() : this(false) { }
        #endregion

        #region Override
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            if (operationType != OperationType.Nothing && operationType != OperationType.Insert)
                GlobalFunctions.SetEnabledForControl(txt_glb_entity_type_option_glb_entity_type_name, selectedRecord.glb_entity_type_option_glb_entity_type_user_define);
        }
        public override bool ValidationForSave()
        {
            selectedRecord.glb_entity_type_option_pre_code = GlobalFunctions.PutZeroBeforeCode(selectedRecord.glb_entity_type_option_pre_code, 2);
            txt_glb_entity_type_option_pre_code.Text = selectedRecord.glb_entity_type_option_pre_code;
            if (Convert.ToInt32(selectedRecord.glb_entity_type_option_pre_code) == 0)
            {
                Messages.ErrorMessage("لطفا پیش کد را وارد کنید");
                return false;
            }
            return base.ValidationForSave();
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            selectedRecordEntityType.glb_entity_type_name = selectedRecord.glb_entity_type_option_glb_entity_type_name;
            selectedRecordEntityType.glb_entity_type_id = selectedRecord.glb_entity_type_option_glb_entity_type_id;
            if (operationType == OperationType.Insert)
                APM_SubSystems.UITools.Create_UserEntityButton(selectedRecordEntityType);
            else if (operationType == OperationType.Update)
            {
                selectedRecordEntityType.glb_entity_type_id = selectedRecord.glb_entity_type_option_glb_entity_type_id;
                APM_SubSystems.UITools.Edit_UserEntityButton(selectedRecordEntityType);
            }
            BusinessLogicLayer.BLL<stp_glb_entity_type_option_selResult>.ListDetailTypeOption.Clear();
        }
        public override void OperationsAfterDelete()
        {
            base.OperationsAfterDelete();
            selectedRecordEntityType.glb_entity_type_id = selectedRecord.glb_entity_type_option_glb_entity_type_id;
            APM_SubSystems.UITools.Delete_UserEntityButton(selectedRecordEntityType);
            BusinessLogicLayer.BLL<stp_glb_entity_type_option_selResult>.ListDetailTypeOption.Clear();
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            selectedRecord.glb_entity_type_option_code_edit_by_user = true;
            selectedRecord.glb_entity_type_option_digit_count = 2;
            selectedRecord.glb_entity_type_option_glb_entity_type_user_define = true;
            selectedRecord.glb_entity_type_option_pre_code = GlobalFunctions.CreateNewCode(bindingList.ToList(), selectedRecord.glb_entity_type_option_digit_count,FieldNames<stp_glb_entity_type_option_selResult>.PreCode);
            MoveCollectionView();
        }
        #endregion
    }
}
