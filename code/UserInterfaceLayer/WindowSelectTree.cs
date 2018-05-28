using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using APMTools;
using APMComponents;
using System.Windows.Input;

namespace UserInterfaceLayer
{
    public class WindowSelectTree<RT> : WindowSelect<RT>
    {
        #region Variables
        #endregion

        #region Constructor
        public WindowSelectTree(TreeType treeType, string rootCaption)
        {
            base.Initial_WindowSelect(SelectType.WindowSelectTree,null);
            tree.XCaption = rootCaption;
            tree.XTreeType = treeType;
            collectionView = (CollectionView)CollectionViewSource.GetDefaultView(tree.Items);
        }
        #endregion

        #region override
        public override void LoadDataFromDB()
        {
            selectedListBeforeChange.Clear();
            selectedListAfterChange.Clear();
            MakeTree();
        }
        public override void CallFilter()
        {
            if (collectionView == null)
                return;
            collectionView.Filter += new Predicate<object>(FilterRecord);
            for (int i = 0; i <= tree.XLevelCount; i++)
            {
                MakeTree(i);
                collectionView.Filter += new Predicate<object>(FilterRecord);
                if (collectionView.Count > 0)
                    break;
            }
        }
        public override bool ValidationForSelect()
        {
            if (tree.XTreeType != TreeType.MultiSelect_All && tree.XTreeType != TreeType.MultiSelect_LastNode)
            {
                if (selectedRecord == null)
                {
                    Messages.NoSelectedRow();
                    return false;
                }
                if(GlobalFunctions.GetValueFromProperty<RT,long>(selectedRecord,FieldNames<RT>.ID)==0)
                {
                    Messages.ErrorMessage("مجاز به انتخاب این عنصر نمی باشید ");
                    return false;
                }
                if(tree.XTreeType==TreeType.SingleSelect_LastChild)
                    if(selectedNode.Items !=null && selectedNode.Items.Count>0)
                    {
                        Messages.ErrorMessage("مجاز به انتخاب عنصری که زیر مجموعه دارد نمی باشید");
                        return false;
                    }
                if (tree.XTreeType == TreeType.SingleSelect_Entity)
                    if (GlobalFunctions.GetValueFromProperty<RT, Boolean>(selectedRecord, FieldNames<RT>.IsGroup) == true)
                    {
                        Messages.ErrorMessage("شما مجاز به انتخاب گروه نمی باشید");
                        return false;
                    }
                if (tree.XTreeType == TreeType.SingleSelect_LastEntity)
                    if ((selectedNode.Items != null && selectedNode.Items.Count > 0 )|| (GlobalFunctions.GetValueFromProperty<RT, Boolean>(selectedRecord, FieldNames<RT>.IsGroup) == true))
                    {
                        Messages.ErrorMessage("فقط مجاز به انتخاب آخرین سطح می باشید");
                        return false;
                    }
            }
            else
            {
                selectedListAfterChange.Clear();
                FindCheckInTree(tree.Items[0] as APMTreeViewItem);
            }
            return true;
        }
        #endregion

        #region Tools
        private void FindCheckInTree(APMTreeViewItem Root)
        {
            foreach (APMTreeViewItem node in Root.Items)
            {
                if (node.XIsChecked == true)
                {
                    GlobalFunctions.SetValueToProperty((RT)node.Tag, FieldNames<RT>.Selected, true);
                    selectedListAfterChange.Add((RT)node.Tag);
                }
                FindCheckInTree(node);
            }
        }
        #endregion
               
        #region Events
        public override void APMTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            base.APMTree_MouseDoubleClick(sender, e);
            if (multiSelect==false)
                SelectClick();
        }
        #endregion
    }
}
