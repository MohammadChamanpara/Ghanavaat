using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using APMTools;
using APMComponents;
using BusinessLogicLayer;
using System.Collections.Generic;
using System.Windows.Input;

namespace UserInterfaceLayer
{
    public class WindowSelectGridGroup<RT, GroupRT> : WindowSelectGrid<RT>
    {
        #region Constructor
        public WindowSelectGridGroup() : base(SelectType.WindowSelectGridGroup) { }
        #endregion

        #region Override
        public override void APMComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (aPMComboBox.SelectedIndex == -1)
                return;
            GlobalFunctions.Copy_PK_To_FK(RecordParameter, (GroupRT)aPMComboBox.SelectedItem);
            var x = GlobalFunctions.GetValueFromProperty<GroupRT, long>((GroupRT)aPMComboBox.SelectedItem, FieldNames<GroupRT>.ID);
            var someRecords = new List<RT>();
            if (aPMComboBox.SelectedIndex == 0)
                someRecords = allRecords;
            else
            {
                if
                (
                    GlobalFunctions.PropertyExist(RecordParameter, FieldNames<RT>.GroupId) &&
                    GlobalFunctions.PropertyExist(RecordParameter, FieldNames<RT>.Selected)
                )
                {
                    someRecords = BLL.GetSomeRecords_DB(RecordParameter);
                    someRecords = someRecords.FindAll(recordForSelected => GlobalFunctions.GetValueFromProperty<RT, bool>(recordForSelected, FieldNames<RT>.Selected) == true);
                }
                else
                    someRecords = allRecords.FindAll(recordForSelected => (x == 0) || GlobalFunctions.GetValueFromProperty<RT, long>(recordForSelected, "acc_detail_glb_entity_type_id") == x);
            }
            GlobalFunctions.ListToBindingList(someRecords, bindingList, collectionView);
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<GroupRT>().FillComboBoxForShow(aPMComboBox, "نمایش همه", 0);
        }
        #endregion
    }
}
