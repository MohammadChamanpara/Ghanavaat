using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using APMTools;
using APMComponents;
using System.Windows.Input;
using Microsoft.Windows.Controls;


namespace UserInterfaceLayer
{
    public class WindowSelectGrid<RT> : WindowSelect<RT>
    {
        #region Constructor
        public WindowSelectGrid() : this(SelectType.WindowSelectGrid) { }
        protected WindowSelectGrid(SelectType selectType)
        {
            Initial_WindowSelect(selectType,null);// new WindowSearch<RT>());
        }
        #endregion

        #region override
        public override void LoadDataFromDB()
        {
            ShowSomeRecords(RecordParameter);
            selectedListBeforeChange.Clear();
            selectedListAfterChange.Clear();
            if (allRecords != null && allRecords.Count > 0)
            {
                if (GlobalFunctions.PropertyExist(selectedRecord, FieldNames<RT>.Selected))
                    foreach (RT item in allRecords)
                        if (GlobalFunctions.GetValueFromProperty<RT, Boolean>(item, FieldNames<RT>.Selected))
                        {
                            var newItem = Activator.CreateInstance<RT>();
                            GlobalFunctions.CopyRecord(newItem, item);
                            selectedListBeforeChange.Add(newItem);
                            selectedListAfterChange.Add(newItem);
                        }
            }
        }
        public override bool ValidationForSelect()
        {
            if (txtName != null)
                txtName.Focus();
            else if (txtCode != null)
                txtCode.Focus();
            if (selectedRecord == null)
            {
                Messages.NoSelectedRow();
                return false;
            }
            if (!GlobalFunctions.PropertyExist(selectedRecord, FieldNames<RT>.Selected))
                return true;
            if (dataGrid.XMultiSelect == true)
            {
                selectedListAfterChange.Clear();
                foreach (RT item in allRecords)
                    if (GlobalFunctions.GetValueFromProperty<RT, Boolean>(item, FieldNames<RT>.Selected))
                        selectedListAfterChange.Add(item);
            }
            return true;
        }
        public override void CallFilter()
        {
            bindingList.Clear();
            foreach (RT record in allRecords)
                if (FilterRecord(record))
                    bindingList.Add(record);
        }
        #endregion
    }
}
