using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using UserInterfaceLayer;
using APMComponents;
using APMTools;
using BusinessLogicLayer;
using System.Windows;

namespace UserInterfaceLayer
{
    public class WindowEntityGroup<EntityRT, RelationRT, GroupRT> : WindowEntity<EntityRT>
    {
        #region Variables
        private APMComboBox comboBox;
        private GroupRT record_GroupRT = Activator.CreateInstance<GroupRT>();
        #endregion

        #region Initial_WindowEntityGroup
        public virtual void Initial_WindowEntityGroup(APMDataGridExtended userDataGrid, APMToolBar userToolBar, GroupBox currentRowGroupBox
                                                 , string xOperation, Boolean GetAllRecordsAtFirst, WindowSearch<EntityRT> userSearchForm
                                                 , long userEntity, APMTwoPartCode userTwoPartCode, APMComboBox userComboBox)
        {
            this.comboBox = userComboBox;
            //new BLL<GroupRT>().FillComboBoxForShow(comboBox, record_GroupRT, "نمایش همه", 0);
            this.comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);
            Initial_WindowEntity(userDataGrid, userToolBar, currentRowGroupBox, xOperation,
                                 GetAllRecordsAtFirst, userSearchForm, userEntity, userTwoPartCode);


        }

        #endregion

        #region Tools
        void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboBox.SelectedIndex != -1)
            {
                GlobalFunctions.Copy_PK_To_FK(RecordParameter, (GroupRT)comboBox.SelectedItem);
                var FullRecord = BLL.GetSomeRecords_DB(RecordParameter);
                if (GlobalFunctions.PropertyExist(RecordParameter, FieldNames<EntityRT>.Selected) && comboBox.SelectedIndex != 0)
                    FullRecord = FullRecord.FindAll(recordForSelected => GlobalFunctions.GetValueFromProperty<EntityRT, bool>(recordForSelected, FieldNames<EntityRT>.Selected) == true);
                GlobalFunctions.ListToBindingList(FullRecord, bindingList, collectionView);
            }
        }
        #endregion

        #region override
        public override void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            
            new BLL<GroupRT>().FillComboBoxForShow(comboBox, record_GroupRT,"نمایش همه", 0);
        }

        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            if (comboBox.SelectedIndex == 0)
                return;
            if (typeof(RelationRT) == typeof(EntityRT))
                return;
            RelationRT relationRecord = Activator.CreateInstance<RelationRT>();
            GlobalFunctions.Copy_PK_To_FK(relationRecord, selectedRecord);
            GlobalFunctions.Copy_PK_To_FK(relationRecord, (GroupRT)comboBox.SelectedItem);
            new BLL<RelationRT>().SaveRecord(relationRecord, operationType, false);
        }

        public override void RefreshClick()
        {
            base.RefreshClick();
            new BLL<GroupRT>().FillComboBoxForShow(comboBox,record_GroupRT, "نمایش همه", 0);
        }

        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            comboBox.IsEnabled = enable;
        }


        #endregion
    }
}
