using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using DataAccessLayer;
using BusinessLogicLayer;
using APMComponents;
using APMTools;


namespace UserInterfaceLayer
{
    public class WindowEntity<RT> : WindowBase<RT>
    {
        #region Variables
        protected long entity_type;
        private stp_glb_entity_type_option_selResult current_entity_type;
        private stp_acc_detail_selResult detailRecord = new stp_acc_detail_selResult();
        private BLL<stp_acc_detail_selResult> bllDetail = new BLL<stp_acc_detail_selResult>();
        private APMTwoPartCode userTwoPartCode;
        protected string newCode;
        #endregion

        #region Initial_WindowEntity
        public virtual void Initial_WindowEntity(APMDataGridExtended userDataGrid, APMToolBar userToolBar, GroupBox currentRowGroupBox
                                                , string xOperation, Boolean GetAllRecordsAtFirst, WindowSearch<RT> userSearchForm
                                                , long userEntity, APMTwoPartCode userTwoPartCode)
        {
            this.entity_type = userEntity;
            this.userTwoPartCode = userTwoPartCode;
            Initial_WindowBase(userDataGrid, userToolBar, currentRowGroupBox, xOperation, GetAllRecordsAtFirst, userSearchForm);
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            current_entity_type = BLL<stp_glb_entity_type_option_selResult>.GetDetailsOption(entity_type);
            if (current_entity_type == null)
            {
                DialogResult = true;
                return;
            }
            userTwoPartCode.XEditable = current_entity_type.glb_entity_type_option_code_edit_by_user;
            userTwoPartCode.XMaxLength = current_entity_type.glb_entity_type_option_digit_count;
            detailRecord.acc_detail_glb_entity_type_id = entity_type;
            base.Window_Loaded(sender, e);
        }
        public override void InitializationBeforeSave()
        {
            if (newCode == null)
                newCode = GlobalFunctions.CreateNewCode(bindingList.ToList(), current_entity_type.glb_entity_type_option_digit_count);
            if (
                GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode) == string.Empty ||
                Convert.ToInt32(GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode)) == 0
                )
                GlobalFunctions.SetValueToProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode, newCode);
            GlobalFunctions.PutZeroBeforeCode(selectedRecord, current_entity_type.glb_entity_type_option_digit_count);
        }
        public override void OperationsAfterInsert()
        {
            GlobalFunctions.SetValueToProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode, newCode);
            GlobalFunctions.SetValueToProperty<RT, string>(selectedRecord, FieldNames<RT>.PreCode, current_entity_type.glb_entity_type_option_pre_code);
            detailRecord.acc_detail_id = 0;
        }
        public override bool ValidationForInsert()
        {
            newCode = GlobalFunctions.CreateNewCode(bindingList.ToList(), current_entity_type.glb_entity_type_option_digit_count);
            if (Convert.ToInt32(newCode.Trim()) == 0)
            {
                Messages.WarningMessage("به دلیل پر شدن محدوده کد شما قادر به اضافه کردن رکورد نمی باشید");
                return false;
            }
            return base.ValidationForInsert();
        }
        public override bool ValidationForSave()
        {
            if (Convert.ToInt32(GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode)) == 0)
            {
                Messages.WarningMessage("کد وارد شده صحیح نمی باشد");
                return false;
            }
            if (GlobalFunctions.GetValueFromProperty<RT, long>(selectedRecord, FieldNames<RT>.DetailId) == 0)
            {
                var result = bllDetail.SaveRecord(detailRecord, operationType, false);
                if (result != SaveResult.Saved)
                {
                    Messages.ErrorMessage("برنامه قادر به ایجاد تفصیل نمی باشد");
                    return false;
                }
                GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.DetailId,
                    GlobalFunctions.GetValueFromProperty<stp_acc_detail_selResult, long>(detailRecord, FieldNames<stp_acc_detail_selResult>.ID));
               
            }
            //string detailName = GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.Name);
            //if (detailName == "")
            //{
            //    string realName = GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.RealName);
            //    GlobalFunctions.SetValueToProperty<RT, string>(selectedRecord, FieldNames<RT>.Name, realName);
            //    detailName = realName;
            //}
            //if (detailName.Trim() != "")
            //{
            //    var detailsList = new BLL<stp_acc_detail_selResult>().GetSomeRecords_DB(new stp_acc_detail_selResult() { acc_detail_name = detailName });
            //    if (
            //        (operationType == OperationType.Insert && detailsList.Count > 0)
            //        ||
            //        (operationType == OperationType.Update && detailsList.Count > 1)
            //      )
            //    {
            //        string detailTypes = "";
            //        foreach (stp_acc_detail_selResult detail in detailsList)
            //        {
            //            if (detailTypes != "")
            //                detailTypes += " و ";
            //            detailTypes += detail.acc_detail_glb_entity_type_name;
            //        }
            //        Messages.InformationMessage("در بخش " + detailTypes + " موجودیتی با این نام تعریف شده است ");
            //    }
            //}
            return true;
        }
        public override void OperationsAfterCanceled()
        {
            if (operationType == OperationType.Insert && detailRecord.acc_detail_id != 0)
                bllDetail.DeleteRecord(detailRecord, false);

        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            GlobalFunctions.Copy_Value<RT, RT>(selectedRecord, FieldNames<RT>.Name, selectedRecord, FieldNames<RT>.RealName);
        }
        public override void OperationsAfterDelete()
        {
            base.OperationsAfterDelete();
            GlobalFunctions.Copy_FK_To_PK(detailRecord, selectedRecord);
            bllDetail.DeleteRecord(detailRecord, false);
        }
        #endregion
    }
}
