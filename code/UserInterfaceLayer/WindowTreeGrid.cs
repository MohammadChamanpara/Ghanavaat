using System.Linq;
using System.Windows.Controls;
using APMComponents;
using APMTools;

namespace UserInterfaceLayer
{
    public class WindowTreeGrid<RT> : WindowBase<RT>
    {
        #region Variables
        public int LevelCount, Code_DigitCount;
        #endregion

        #region Initial_WindowTreeGrid
        public virtual void Initial_WindowTreeGrid(APMDataGridExtended userDataGrid,
            APMToolBar userToolBar, GroupBox currentRowGroupBox, string xOperation,
            WindowSearch<RT> userSearchForm, APMTree userTreeView,
            string RootCaption, int LevelCount, int Code_DigitCount)
        {
            base.Initial_WindowBase(userDataGrid, userToolBar, currentRowGroupBox, xOperation, false, userSearchForm);
            tree = userTreeView;
            tree.XCaption = RootCaption;
            this.Code_DigitCount = Code_DigitCount;
            this.LevelCount = LevelCount;
        }
        #endregion

        #region Overrided Methods
        public override void HelpClick()
        {
            selectedNode.Focus();
        }
        public override void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            MakeTree();
        }
        public override void OperationsAfterDelete()
        {
            DeleteNodeFromTree();
        }
        public override bool ValidationForInsert()
        {
            if (parentNode == null)
            {
                Messages.InformationMessage("قادر به تعریف عنصر جدید در سطح ریشه نمی باشید. لطفا روی ریشه کلیک راست کنید و گزینۀ مورد نظر خود را انتخاب کنید");
                return false;
            }
            if (!GlobalFunctions.PropertyExist(parentRecord, FieldNames<RT>.LevelNo))
                return true;
            if (LevelCount > 0 && GlobalFunctions.GetValueFromProperty<RT, int>(parentRecord, FieldNames<RT>.LevelNo) >= LevelCount)
            {
                Messages.ErrorMessage(".مجاز به اضافه کردن عنصر جدید در سطحی بیشتر از تعداد سطوح تعریف شده نیستید");
                return false;
            }
            return base.ValidationForInsert();
        }
        public override bool ValidationForDelete()
        {
            if (parentNode == null)
            {
                Messages.ErrorMessage("شما قادر به حذف سر گروه نمی باشید");
                return false;
            }

            if (selectedNode.Items.Count > 0)
            {
                Messages.ErrorMessage("شما قادر به حذف سر گروه نمی باشید");
                return false;
            }
            return true;
        }
        public override void InitializationBeforeSave()
        {
            MoveCollectionView();
            base.InitializationBeforeSave();
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.ChildCode, GlobalFunctions.PutZeroBeforeCode(GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode), Code_DigitCount));
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.Code, GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.PreCode)
                + GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode));
        }
        public override void OperationsAfterInsert()
        {
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.LevelNo, GlobalFunctions.GetValueFromProperty<RT, int>(parentRecord, FieldNames<RT>.LevelNo) + 1);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.ParentID, GlobalFunctions.GetValueFromProperty<RT, long>(parentRecord, FieldNames<RT>.ID));
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.ChildCode, GlobalFunctions.CreateNewCode(bindingList.ToList(), Code_DigitCount));
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.PreCode, GlobalFunctions.GetValueFromProperty<RT, string>(parentRecord, FieldNames<RT>.Code));
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.ParentName, GlobalFunctions.GetValueFromProperty<RT, string>(parentRecord, FieldNames<RT>.Name));
        }
        public override void RefreshClick()
        {
            MakeTree();
        }
        public override bool ValidationForEdit()
        {
            if (GlobalFunctions.GetValueFromProperty<RT, long>(selectedRecord, FieldNames<RT>.ID) == 0)
            {
                Messages.ErrorMessage(".این عنصر قابل ویرایش نمی باشد");
                return false;
            }
            return base.ValidationForEdit();
        }
        public override void OperationsAfterCanceled()
        {
            base.OperationsAfterCanceled();
            tree_SelectedItemChanged(null, null);
            tree.Focus();
        }
        public override void OperationsAfterSaved()
        {
            if (operationType == OperationType.Insert)
            {
                AddNodeToTree(parentNode, selectedRecord);
                if (parentNode.Items.Count > 0)
                    SelectTreeNode(tree, parentNode.Items[0] as APMTreeViewItem);
            }
            if (operationType == OperationType.Update)
                UpdateTreeNode();
            else
                dataGrid_SelectionChanged(null, null);
            tree.Items.Refresh();
        }
        public override void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.dataGrid_MouseDoubleClick(sender, e);
            if (selectedNode == null || selectedNode.Items.Count == 0 || !((selectedNode.Items[0] as TreeViewItem).Tag is RT))
                return;
            SelectTreeNode(tree, selectedNode.Items[0] as APMTreeViewItem);
        }
        #endregion

        #region Tree
        public virtual void UpdateTreeNode()
        {
            selectedNode.XCaption = GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.Name);
        }
        public void DeleteNodeFromTree()
        {
            parentNode.Items.Remove(selectedNode);
        }
        #endregion
    }
}
