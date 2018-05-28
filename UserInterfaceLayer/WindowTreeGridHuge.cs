using System.Linq;
using System.Windows.Controls;
using APMComponents;
using APMTools;
using System;

namespace UserInterfaceLayer
{
    public class WindowTreeGridHuge<GridRT,TreeRT> : WindowTreeGrid<GridRT>
    {
        #region Initial_WindowTreeGridHuge
        public virtual void Initial_WindowTreeGridHuge(APMDataGridExtended userDataGrid,
            APMToolBar userToolBar,GroupBox currentRowGroupBox,string xOperation,
            WindowSearch<GridRT> userSearchForm,APMTree userTreeView, 
            string RootCaption,int LevelCount,int Code_DigitCount)
        {
            base.Initial_WindowTreeGrid( userDataGrid,userToolBar, currentRowGroupBox, xOperation,userSearchForm, userTreeView,RootCaption, LevelCount, Code_DigitCount);
        }
        #endregion
      
        #region Overrided Methods
        public override void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetEnables(true);
            MakeTree<TreeRT>();
        }
        public override void RefreshClick()
        {
            MakeTree<TreeRT>();
        }
        public override void FillBindingListFromTree()
        {
            if
            (
                parentNode == null ||
                parentNode.Items.Count == 0 ||
                parentNode.XLoadedFromDataBase
            )
                base.FillBindingListFromTree();
            else
            {
                bindingList.Clear();
                var parameter=Activator.CreateInstance<GridRT>();
                GlobalFunctions.Copy_Value(parameter, FieldNames<GridRT>.ID, parentRecord, FieldNames<GridRT>.ID);
                ShowSomeRecords(parameter);
                parentNode.XLoadedFromDataBase = true; 
                for (int i = 0; i < allRecords.Count; i++)
                    for (int j = 0; j < parentNode.Items.Count; j++)
                    {
                        var node=parentNode.Items[j] as TreeViewItem;
                        var nodeRecord=(GridRT)node.Tag;
                        var nodeId=GlobalFunctions.GetValueFromProperty<GridRT,long>(nodeRecord,FieldNames<TreeRT>.ID);
                        var recordId = GlobalFunctions.GetValueFromProperty<GridRT, long>(allRecords[i], FieldNames<TreeRT>.ID);
                        if (nodeId == recordId)
                        {
                            node.Tag = allRecords[i];
                            continue;
                        }
                    }
            }
        }
        public override bool ChangeSelectedRecord(GridRT newSelectedRecord, object caller)
        {
            return base.ChangeSelectedRecord(newSelectedRecord, caller);
        }
        #endregion
    }
}
