using System;
using System.Windows;
using System.Windows.Controls;
using APMTools;
using APMComponents;
using System.Collections.Generic;
using BusinessLogicLayer;
using DataAccessLayer;
using System.Linq;

namespace UserInterfaceLayer
{
    public class WindowTreeGridTwoTables<RT> : WindowTreeGrid<RT>
    {
        #region Variables

        public enum Status { Group, Entity };
        public Status status = new Status();
        UIElement grpGroup, grpEntity;
        string XOperationGroup, XOperationEntity;
        public stp_glb_entity_type_option_selResult glbEntityTypeOption_For_Goods;
        private string newCode;
        #endregion

        #region Initial_WindowTreeGridTwoTable
        public virtual void Initial_WindowTreeGridTwoTable(APMDataGridExtended userDataGrid, APMToolBar userToolBar,
            WindowSearch<RT> userSearchForm, GroupBox groupBoxCurrentRow, APMTree userTreeView, string Header,
            int LevelCount, int Code_DigitCount, UIElement grpGroup, UIElement grpEntity,
            string XOperationGroup, string XOperationEntity, string groupTitle, string entityTitle)
        {
            base.Initial_WindowTreeGrid(userDataGrid, userToolBar, groupBoxCurrentRow, null, userSearchForm, userTreeView, Header, LevelCount, Code_DigitCount);
            this.grpGroup = grpGroup;
            this.grpEntity = grpEntity;
            this.XOperationGroup = XOperationGroup;
            this.XOperationEntity = XOperationEntity;
            status = Status.Entity;
            ChangeStatus(Status.Group);
            tree.XIsTwoTable = true;
            tree.XEntityTitle = entityTitle;
            tree.XGroupTitle = groupTitle;
            tree.XHaveContextMenu = true;
            tree.XInsertChildClick_Entity += InsertChildClick_Entity;
            glbEntityTypeOption_For_Goods = BLL<stp_glb_entity_type_option_selResult>.GetDetailsOption((long)EntityType.inv_goods);

        }
        #endregion

        #region ChangeStatus
        public virtual void ChangeStatus(Status newStatus)
        {
            if (status == newStatus)
                return;
            status = newStatus;
            GlobalFunctions.SetVisibilityForControl(grpGroup, (status == Status.Group));
            GlobalFunctions.SetVisibilityForControl(grpEntity, (status == Status.Entity));
            if (dataGrid != null)
            {
                dataGrid.XOperation = (status == Status.Group) ? XOperationGroup : XOperationEntity;
                if (dataGrid.XOperation != null)
                    dataGrid.Adjust(bindingList);
                dataGrid.SelectionChanged -= dataGrid_SelectionChanged;
                dataGrid.SelectedIndex = -1;
                dataGrid.SelectedIndex = collectionView.CurrentPosition;
                dataGrid.SelectionChanged += dataGrid_SelectionChanged;
            }
        }
        #endregion

        #region Override
        public override Boolean ChangeSelectedRecord(RT newSelectedRecord, object caller)
        {
            if (base.ChangeSelectedRecord(newSelectedRecord, caller))
            {
                if (operationType != OperationType.Nothing)
                    return true;
                if (GlobalFunctions.GetValueFromProperty<RT, Boolean>(selectedRecord, FieldNames<RT>.IsGroup) == false ||
                    GlobalFunctions.GetValueFromProperty<RT, int>(parentRecord, FieldNames<RT>.LevelNo) == base.LevelCount)
                    ChangeStatus(Status.Entity);
                else
                    ChangeStatus(Status.Group);
                return true;
            }
            return false;
        }
        public override Boolean ValidationForInsert()
        {
            if (!GlobalFunctions.GetValueFromProperty<RT, Boolean>(parentRecord, FieldNames<RT>.IsGroup))
            {
                Messages.ErrorMessage("امکان تعریف عنصر جدید در این سطح وجود ندارد");
                tree_SelectedItemChanged(null, null);
                return false;
            }
            int digitCount = (status == Status.Entity) ? glbEntityTypeOption_For_Goods.glb_entity_type_option_digit_count : Code_DigitCount;
            newCode = GlobalFunctions.CreateNewCode(bindingList.ToList(), digitCount);
            if (Convert.ToInt32(newCode.Trim()) == 0)
            {
                Messages.WarningMessage("به دلیل پر شدن محدوده کد شما قادر به اضافه کردن رکورد نمی باشید");
                return false;
            }
            if (status == Status.Entity && GlobalFunctions.GetValueFromProperty<RT, int>(parentRecord, FieldNames<RT>.LevelNo) == LevelCount)
                return true;

            return base.ValidationForInsert();
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.ChildCode, newCode);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.IsGroup, (status == Status.Group));
        }
        public override bool ValidationForSave()
        {
            string childCode = GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode);
            if (childCode == "" || Convert.ToInt32(GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.ChildCode)) == 0)
            {
                Messages.WarningMessage("کد وارد شده صحیح نمی باشد");
                return false;
            }
            return base.ValidationForSave();
        }
        #endregion

        #region InsertEntity
        private void InsertChildClick_Entity(object sender, RoutedEventArgs e)
        {
            if (selectedNode.Items.Count > 0)
                InsertChildClick();
            else
            {
                parentNode = (tree.SelectedItem as APMTreeViewItem);
                parentRecord = (RT)parentNode.Tag;
                FillBindingListFromTree();
                ChangeStatus(Status.Entity);
                InsertClick();
            }
        }
        #endregion
    }
}
