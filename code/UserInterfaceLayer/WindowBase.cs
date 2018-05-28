using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using APMTools;
using APMComponents;
using BusinessLogicLayer;
using DataAccessLayer;
using System.Windows.Input;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Windows.Media;
using Microsoft.Windows.Controls;

namespace UserInterfaceLayer
{
    public class WindowBase<RT> : Window
    {
        #region Variables
        public static BLL<stp_glb_coding_selResult> BLLCoding = new BLL<stp_glb_coding_selResult>();
        public RT beforeEditing, searchRecord, RecordParameter;
        public APMTreeViewItem selectedNode, parentNode;
        public RT selectedRecord, parentRecord;
        public string selectHeader;
        public APMTree tree;
        public Boolean ChangeSelectedRecordIsEnable = true;
        public List<RT> allRecords = new List<RT>();
        private List<RT> ListForMakingTree = new List<RT>();
        public List<APMComboBox> listForComboBox = new List<APMComboBox>();
        public CollectionView collectionView, collectionViewArticle, CollectionViewFilter;
        public APMBindingList<RT> bindingList = new APMBindingList<RT>();
        public OperationType operationType = new OperationType();
        public APMDataGrid dataGrid;
        private APMToolBar toolBar;
        public BLL<RT> BLL;
        private GroupBox grpCurrentRow;
        protected Boolean getAllRecordsAtFirst = true;
        public WindowSearch<RT> searchForm;
        public long formID;
        #endregion

        #region Initial_WindowBase
        protected void Initial_WindowBase(APMDataGridExtended userDataGrid, APMToolBar userToolBar, GroupBox currentRowGroupBox, string xOperation, Boolean getAllRecordsAtFirst, WindowSearch<RT> userSearchForm)
        {
            RecordParameter = Activator.CreateInstance<RT>();
            this.getAllRecordsAtFirst = getAllRecordsAtFirst;
            BLL = Activator.CreateInstance<BLL<RT>>();

            if (userDataGrid != null)
            {
                dataGrid = userDataGrid.datagrid;
                dataGrid.CanUserSortColumns = true;
                dataGrid.SelectionChanged += dataGrid_SelectionChanged;
                dataGrid.MouseDoubleClick += new MouseButtonEventHandler(dataGrid_MouseDoubleClick);
                dataGrid.XOperation = xOperation;

            }
            toolBar = userToolBar;
            grpCurrentRow = currentRowGroupBox;
            if (toolBar != null)
                foreach (object ob in toolBar.Items)
                    if (ob is APMToolbarButton)
                        (ob as APMToolbarButton).Click += new RoutedEventHandler(toolBarButton_Click);

            if (bindingList != null)
            {
                collectionView = (CollectionView)CollectionViewSource.GetDefaultView(bindingList);
                collectionView.CurrentChanged += collectionView_CurrentChanged;
            }
            searchForm = userSearchForm;
            SetBinding();
        }
        #endregion

        #region Constructor
        public WindowBase()
        {
            if (FieldNames<RT>.FixPart.StartsWith("glb_"))
                GlobalVariables.currentSubSystem = SubSystems.Inventory;
            else if (FieldNames<RT>.FixPart.StartsWith("acc_"))
                GlobalVariables.currentSubSystem = SubSystems.Accounting;
            else if (FieldNames<RT>.FixPart.StartsWith("inv_"))
                GlobalVariables.currentSubSystem = SubSystems.Inventory;
            else if (FieldNames<RT>.FixPart.StartsWith("gnt_"))
                GlobalVariables.currentSubSystem = SubSystems.Accounting;
            selectedRecord = (RT)Activator.CreateInstance(typeof(RT));
            beforeEditing = (RT)Activator.CreateInstance(typeof(RT));
            searchRecord = (RT)Activator.CreateInstance(typeof(RT));
            Window window = (this as Window);
            window.KeyDown -= Window_KeyDown;
            window.KeyDown += Window_KeyDown;
            window.Loaded -= Window_Loaded;
            window.Loaded += Window_Loaded;
            window.Closing -= Window_Closing;
            window.Closing += Window_Closing;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.FontFamily = new System.Windows.Media.FontFamily("tahoma");
        }

        #endregion

        #region ArticlePackage
        public class ArticlePackage<ListT, RelationT>
        {
            #region Variables
            private BLL<RelationT> BLLRelation = new BLL<RelationT>();
            private BLL<ListT> BLLArticle = new BLL<ListT>();
            public List<ListT>
                ListBeforeChange = new List<ListT>(),
                ListAfterChange = new List<ListT>(),
                AddedList = new List<ListT>(),
                DeletedList = new List<ListT>(),
                EditedList = new List<ListT>();

            #endregion

            #region Save
            public SaveResult Save<T>(T selectedRecord)
            {
                if (ListBeforeChange.Count == 0 && ListAfterChange.Count == 0)
                    return SaveResult.Saved;
                FillLists();
                SaveResult result;
                if (typeof(RelationT) == typeof(ListT))
                    result = SaveArticles<T>(selectedRecord);
                else
                    result = SaveRelations<T>(selectedRecord);
                if (result == SaveResult.Saved)
                    Clear();
                return result;
            }
            #endregion

            #region SaveArticles
            private SaveResult SaveArticles<T>(T selectedRecord)
            {
                SaveResult result;
                RelationT relationRecord = Activator.CreateInstance<RelationT>();
                foreach (ListT article in AddedList)
                {
                    GlobalFunctions.Copy_PK_To_FK(article, selectedRecord);
                    result = BLLArticle.SaveRecord(article, OperationType.Insert, false);
                    if (result != SaveResult.Saved)
                        return result;
                }

                foreach (ListT article in EditedList)
                {
                    result = BLLArticle.SaveRecord(article, OperationType.Update, false);
                    if (result != SaveResult.Saved)
                        return result;
                }

                foreach (ListT article in DeletedList)
                {
                    result = BLLArticle.DeleteRecord(article, false) == true ? SaveResult.Saved : SaveResult.DontSave;
                    if (result != SaveResult.Saved)
                        return result;
                }
                return SaveResult.Saved;
            }
            #endregion

            #region SaveRelations
            private SaveResult SaveRelations<T>(T selectedRecord)
            {
                SaveResult result = SaveResult.Saved;
                RelationT relationRecord = Activator.CreateInstance<RelationT>();

                foreach (ListT article in AddedList)
                {
                    Copy_Values_ToRelationRecord<T>(selectedRecord, relationRecord, article);
                    result = BLLRelation.SaveRecord(relationRecord, OperationType.Insert, false);
                }

                foreach (ListT article in EditedList)
                {
                    Copy_Values_ToRelationRecord<T>(selectedRecord, relationRecord, article);
                    result = BLLRelation.SaveRecord(relationRecord, OperationType.Update, false);
                }

                foreach (ListT article in DeletedList)
                {
                    Copy_Values_ToRelationRecord<T>(selectedRecord, relationRecord, article);
                    result = BLLRelation.DeleteRecord(relationRecord, false) == true ? SaveResult.Saved : SaveResult.DontSave;
                }
                return result;
            }
            #endregion

            #region Copy Values To Relation Record
            private static void Copy_Values_ToRelationRecord<T>(T selectedRecord, RelationT relationRecord, ListT article)
            {
                GlobalFunctions.Copy_Same_Fields(relationRecord, article);
                GlobalFunctions.Copy_FK_To_PK(relationRecord, article);
                GlobalFunctions.Copy_FK_To_FK(relationRecord, article);
                GlobalFunctions.Copy_PK_To_FK(relationRecord, selectedRecord);
                GlobalFunctions.Copy_PK_To_FK(relationRecord, article);

            }
            #endregion

            #region Fill Lists
            public void FillLists()
            {
                long oldID, newID;
                AddedList.Clear();
                EditedList.Clear();
                DeletedList.Clear();
                Boolean added;
                foreach (var newRecord in ListAfterChange)
                {
                    added = true;
                    newID = GlobalFunctions.GetValueFromProperty<ListT, long>(newRecord, FieldNames<ListT>.ID);
                    foreach (var oldRecord in ListBeforeChange)
                    {
                        oldID = GlobalFunctions.GetValueFromProperty<ListT, long>(oldRecord, FieldNames<ListT>.ID);
                        if (oldID == newID && newID != 0)
                        {
                            added = false;
                            break;
                        }
                    }
                    if (added == true)
                        AddedList.Add(newRecord);

                }
                Boolean deleted;
                foreach (ListT oldRecord in ListBeforeChange)
                {
                    oldID = GlobalFunctions.GetValueFromProperty<ListT, long>(oldRecord, FieldNames<ListT>.ID);
                    deleted = true;
                    foreach (ListT newRecord in ListAfterChange)
                    {
                        newID = GlobalFunctions.GetValueFromProperty<ListT, long>(newRecord, FieldNames<ListT>.ID);
                        if (oldID == newID)
                        {
                            deleted = false;
                            break;
                        }
                    }
                    if (deleted == true)
                        DeletedList.Add(oldRecord);
                }

                foreach (var oldArticle in ListBeforeChange)
                {
                    oldID = GlobalFunctions.GetValueFromProperty<ListT, long>(oldArticle, FieldNames<ListT>.ID);
                    foreach (ListT newArticle in ListAfterChange)
                    {
                        newID = GlobalFunctions.GetValueFromProperty<ListT, long>(newArticle, FieldNames<ListT>.ID);
                        if (oldID == newID)
                            if (!GlobalFunctions.ObjectsAreEqual(oldArticle, newArticle))
                                EditedList.Add(newArticle);
                    }
                }
            }
            #endregion

            #region Clear
            public void Clear()
            {
                AddedList.Clear();
                DeletedList.Clear();
                EditedList.Clear();
                ListAfterChange.Clear();
                ListBeforeChange.Clear();
            }

            #endregion
        }
        #endregion

        #region Events

        #region CodeTextBox KeyDown
        public Boolean CodeTextBox_KeyDown<BrowseT>(Object textBox, string destinationID, KeyEventArgs PressedKey)
        {
            return CodeTextBox_KeyDown<BrowseT, RT>(textBox, destinationID, PressedKey, selectedRecord);
        }
        public Boolean CodeTextBox_KeyDown<BrowseT, MainT>(Object userTextBox, string destinationID, KeyEventArgs PressedKey, MainT selectedRecord)
        {
            BrowseT recordParametr = Activator.CreateInstance<BrowseT>();
            return CodeTextBox_KeyDown_Filter<BrowseT, MainT>(userTextBox, destinationID, PressedKey, selectedRecord, recordParametr);
        }
        public Boolean CodeTextBox_KeyDown_Filter<BrowseT, MainT>(Object userTextBox, string destinationID, KeyEventArgs PressedKey, MainT selectedRecord, BrowseT RecordParametr)
        {
            if (!(userTextBox is TextBox))
                return false;
            TextBox textBox = userTextBox as TextBox;
            var bll = Activator.CreateInstance<BLL<BrowseT>>();
            if (PressedKey.Key != Key.Enter)
                return false;
            textBox.Text = textBox.Text.Trim();
            List<BrowseT> list = new List<BrowseT>();
            list = bll.GetSomeRecords_DB(RecordParametr);
            var record = list.Find(rec => GlobalFunctions.GetValueFromProperty<BrowseT, string>(rec, FieldNames<BrowseT>.Code) == textBox.Text);

            if (record == null)
            {
                Messages.ErrorMessage("کد مورد نظر اشتباه می باشد");
                MoveCollectionViewArticle();
                MoveCollectionView();
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, Activator.CreateInstance<BrowseT>(), destinationID);
                PressedKey.Handled = true;
                textBox.Focus();
            }
            else
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, record, destinationID);
            OperationsAfterBrowse();
            MoveCollectionView();
            return true;
        }
        public Boolean CodeTextBox_KeyDown_Report<BrowseT, MainT>(object sender, string destinationID, KeyEventArgs PressedKey, MainT selectedRecord)
        {
            BrowseT recordParameter = Activator.CreateInstance<BrowseT>();
            APMBrowser browser;
            TextBox textBox = (sender as APMTextBox);
            browser = (sender as APMTextBox).Parent as APMBrowser;
            var bll = Activator.CreateInstance<BLL<BrowseT>>();
            if (PressedKey.Key != Key.Enter)
                return false;
            textBox.Text = textBox.Text.Trim();
            List<BrowseT> list = new List<BrowseT>();
            list = bll.GetSomeRecords_DB(recordParameter);
            var record = list.Find(rec => GlobalFunctions.GetValueFromProperty<BrowseT, string>(rec, FieldNames<BrowseT>.Code) == textBox.Text);
            string IdFieldName = GetPropertyNameFromControlName(browser);
            IdFieldName = IdFieldName + "s";
            string NameFieldName = IdFieldName.Replace("ids", "name");
            string CodeFieldName = IdFieldName.Replace("ids", "code");

            if (record == null)
            {
                Messages.ErrorMessage("کد مورد نظر اشتباه می باشد");
                MoveCollectionViewArticle();
                MoveCollectionView();
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, Activator.CreateInstance<BrowseT>(), destinationID);
                PressedKey.Handled = true;
                textBox.Focus();
            }
            else
            {
                string ID = (GlobalFunctions.GetValueFromProperty<BrowseT, long>(record, FieldNames<BrowseT>.ID)).ToString();
                ID = "," + ID + ",";
                string selectedName = GlobalFunctions.GetValueFromProperty<BrowseT, string>(record, FieldNames<BrowseT>.Name);
                GlobalFunctions.SetValueToProperty(selectedRecord, IdFieldName, ID);
                GlobalFunctions.SetValueToProperty(selectedRecord, NameFieldName, selectedName);
                GlobalFunctions.SetValueToProperty(selectedRecord, CodeFieldName, textBox.Text);
                MoveCollectionView();
                browser.XLabel.Content = selectedName;
                browser.XTextBox.DataContext = null;
                browser.XTextBox.Text = GlobalFunctions.GetValueFromProperty<BrowseT, string>(record, FieldNames<BrowseT>.Code);
                OperationsAfterBrowse();

            }

            return true;
        }
        #endregion

        #region BrowseClick
        /// <summary>
        ///.و انتخاب چندعنصر در آن استفاده می شود Select Form این تابع جهت باز کردن یک 
        /// </summary>
        /// <typeparam name="T">
        /// .نوع رکورد فرم باز کننده که در اینجا لازم نیست آن را وارد کنید
        /// </typeparam>
        /// <typeparam name="RelationT">
        /// .نوع رکورد فرم باز شونده که در اینجا لازم نیست آن را وارد کنید
        /// </typeparam>
        /// <param name="BrowseForm">
        /// نام فرم باز شونده
        /// </param>
        /// <param name="articlePackage">
        /// که در فرم خود تعریف کرده اید ArticlePackage نام شیء 
        /// </param>
        /// <param name="FieldName_ToSetTrue">
        ///  <para>Browse Form این پارامتر برای فرم هایی مانند کالا که می خواهند از طریق فرم خود یک</para>  
        ///  <para>را چند بار با عناوین مختلف باز کنند به کار می رود </para>
        /// <para>  به عنوان مثال فرم کالا برای انتخاب کالای مشابه و اجزای تشکیل دهنده </para>
        /// <para>.را باز میکند GroupGoodsSelect دو بار فرم </para>
        /// </param>
        public Boolean BrowseClick_MultiSelect<BrowseRT, RelationRT>(WindowSelect<BrowseRT> BrowseForm, ref ArticlePackage<BrowseRT, RelationRT> articlePackage,
            string selectHeader, Type typeOfMainForm, params object[] FieldName_and_Value)
        {
            BrowseForm.multiSelect = true;
            if (BrowseForm.dataGrid != null)
                BrowseForm.dataGrid.XMultiSelect = true;

            articlePackage.ListAfterChange.Clear();
            articlePackage.ListBeforeChange.Clear();
            GlobalFunctions.Copy_PK_To_FK(BrowseForm.RecordParameter, selectedRecord);
            GlobalFunctions.Copy_FK_To_PK(BrowseForm.RecordParameter, selectedRecord);
            string fieldName = null;
            if (FieldName_and_Value != null && FieldName_and_Value.Length == 2 && FieldName_and_Value[0] is string)
                fieldName = (FieldName_and_Value[0] as string);
            if (fieldName != null)
                GlobalFunctions.SetValueToProperty(BrowseForm.RecordParameter, fieldName, FieldName_and_Value[1]);
            var result = BrowseClick(BrowseForm, selectHeader, typeOfMainForm, null);

            foreach (BrowseRT record in BrowseForm.selectedListBeforeChange)
                articlePackage.ListBeforeChange.Add(record);
            foreach (BrowseRT record in BrowseForm.selectedListAfterChange)
                articlePackage.ListAfterChange.Add(record);
            if (fieldName != null)
                GlobalFunctions.SetValueToProperty(BrowseForm.RecordParameter, fieldName, FieldName_and_Value[1]);
            BrowseForm.multiSelect = false;
            if (GlobalFunctions.GetValueFromProperty<BrowseRT, long>(result, FieldNames<BrowseRT>.ID) == 0)
                return false;
            return true;
        }
        public void BrowseClick_Report<BrowseRT>(object sender, WindowSelect<BrowseRT> BrowseForm, string selectHeader, Type typeOfMainForm)
        {
            var articlePackage = new ArticlePackage<BrowseRT, BrowseRT>();
            BrowseClick_MultiSelect(BrowseForm, ref articlePackage, selectHeader, typeOfMainForm);
            string IDfieldName = FieldNames<BrowseRT>.ID;
            string selectedIds = "";
            string selectedCodes = "";
            string selectedNames = "";
            int maxNames = 3;
            foreach (var record in articlePackage.ListAfterChange)
            {
                selectedIds += "," + GlobalFunctions.GetValueFromProperty<BrowseRT, long>(record, IDfieldName).ToString();
                if (maxNames == 0)
                    continue;
                selectedCodes = GlobalFunctions.GetValueFromProperty<BrowseRT, string>(record, FieldNames<BrowseRT>.Code) + "," + selectedCodes;
                selectedNames += GlobalFunctions.GetValueFromProperty<BrowseRT, string>(record, FieldNames<BrowseRT>.Name) + ",";
                maxNames--;
            }
            selectedCodes = (selectedCodes == "") ? selectedCodes : selectedCodes.Substring(0, selectedCodes.Length - 1);
            selectedNames = (selectedNames == "") ? selectedNames : selectedNames.Substring(0, selectedNames.Length - 1);
            selectedIds = (selectedIds == "") ? selectedIds : selectedIds + ",";
            APMBrowser browser;
            if (sender is APMToolbarButton && (sender as APMToolbarButton).Parent is APMBrowser)
            {
                browser = (sender as APMToolbarButton).Parent as APMBrowser;
                string IdFieldName = GetPropertyNameFromControlName(browser);
                string IdsFieldName = IdFieldName.Replace("id", "ids");
                string NameFieldName = IdFieldName.Replace("id", "name");
                string CodeFieldName = IdFieldName.Replace("id", "code");

                GlobalFunctions.SetValueToProperty(selectedRecord, IdsFieldName, selectedIds);
                GlobalFunctions.SetValueToProperty(selectedRecord, NameFieldName, selectedNames);
                GlobalFunctions.SetValueToProperty(selectedRecord, CodeFieldName, selectedCodes);
                MoveCollectionView();
                browser.XTextBox.DataContext = null;
                browser.XTextBox.Text = selectedCodes;
                browser.XLabel.Content = selectedNames;
            }
        }
        public BrowseRT BrowseClick<BrowseRT>(WindowSelect<BrowseRT> BrowseForm, string selectHeader, Type typeOfMainForm, object sender)
        {
            return BrowseClick(BrowseForm, selectedRecord, selectHeader, typeOfMainForm, sender);
        }
        public BrowseRT BrowseClick_Parameter<BrowseRT, MainRT>(WindowSelect<BrowseRT> BrowseForm, MainRT selectedRecord, BrowseRT recordParameter, string selectHeader, Type typeOfMainForm, object sender)
        {
            BrowseForm.RecordParameter = recordParameter;
            return BrowseClick(BrowseForm, selectedRecord, selectHeader, typeOfMainForm, sender);
        }
        public BrowseRT BrowseClick<BrowseRT, MainRT>(WindowSelect<BrowseRT> BrowseForm, MainRT selectedRecord, string selectHeader, Type typeOfMainForm, object sender)
        {
            try
            {
                string destinationID;
                if (sender == null)
                    destinationID = null;
                else
                {
                    var browser = (sender as APMToolbarButton).Parent as APMBrowser;
                    if (browser.Name != null && browser.Name.Length > 4)
                        destinationID = browser.Name.Substring(("brw_").Length);
                    else
                        destinationID = null;
                }
                BrowseForm.mainForm = typeOfMainForm;
                if (BrowseForm.multiSelect == false && BrowseForm.dataGrid != null)
                    BrowseForm.dataGrid.XMultiSelect = false;
                GlobalFunctions.SetValueToProperty(BrowseForm.RecordParameter, FieldNames<BrowseRT>.Name, GlobalFunctions.GetValueFromProperty<MainRT, string>(selectedRecord, FieldNames<MainRT>.Name));
                BrowseForm.selectHeader = selectHeader;
                if (!ShowDialogForm(BrowseForm))
                    return Activator.CreateInstance<BrowseRT>();
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, BrowseForm.selectedRecord, destinationID);
                MoveCollectionView();
                OperationsAfterBrowse();
                return BrowseForm.selectedRecord;
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
            return Activator.CreateInstance<BrowseRT>();
        }
        #endregion

        #endregion

        #region Tools
        public void MoveCollectionView()
        {
            MoveCollectionView(collectionView);
        }
        public void MoveCollectionView(CollectionView userCollectionview)
        {
            if (userCollectionview == null)
                return;
            if (GlobalVariables.collectionViewIsMoving)
                return;
            GlobalVariables.collectionViewIsMoving = true;
            int savePosition = userCollectionview.CurrentPosition;
            userCollectionview.MoveCurrentToPosition(-1);
            userCollectionview.MoveCurrentToPosition(savePosition);
            GlobalVariables.collectionViewIsMoving = false;
        }
        public void ShowAllRecords()
        {
            GlobalFunctions.ListToBindingList(BLL.GetAllRecords_local(), bindingList, collectionView);
            allRecords = bindingList.ToList();
            if (bindingList.Count == 0)
                selectedRecord = Activator.CreateInstance<RT>();
        }
        public Boolean ShowDialogForm<T>(WindowBase<T> window)
        {
            window.ShowDialog();
            if (window.DialogResult.HasValue && window.DialogResult.Value)
                return true;
            return false;
        }
        public void FocusFirstControl()
        {
            FocusFirstControl(grpCurrentRow);
        }
        public Boolean FocusFirstControl(object control)
        {
            if (control == null)
                return false;
            if (control is UIElement)
                if ((control as UIElement).Visibility != Visibility.Visible || (control as UIElement).IsEnabled == false)
                    return false;

            if (control is APMBrowser)
            {
                (control as APMBrowser).Focus();
                return true;
            }
            else if (control is TextBox)
            {
                (control as TextBox).Focus();
                return true;
            }

            else if (control is ComboBox)
            {
                (control as ComboBox).Focus();
                return true;
            }

            else if (control is Panel)
            {
                foreach (object obj in (control as Panel).Children)
                    if (FocusFirstControl(obj) == true)
                        return true;
            }
            else if (control is ContentControl)
            {
                if (FocusFirstControl((control as ContentControl).Content) == true)
                    return true;
            }
            else if (control is TabControl)
            {
                if (FocusFirstControl((control as TabControl).SelectedItem) == true)
                    return true;
            }
            else if (control is ItemsControl)
                foreach (object obj in (control as ItemsControl).Items)
                    if (FocusFirstControl(obj) == true)
                        return true;
            return false;
        }
        private static string GetPropertyNameFromControlName(object control)
        {
            string fieldName = "";
            if ((control is FrameworkElement) && (control as FrameworkElement).Name != null && (control as FrameworkElement).Name.Length > 4)
                fieldName = (control as FrameworkElement).Name.Substring(4);
            return fieldName;
        }
        public void SetSelectedRecordFromComboBoxes()
        {
            if (listForComboBox == null)
                return;
            foreach (ComboBox comboBox in listForComboBox)
                GlobalFunctions.SetValueToProperty(selectedRecord, GetPropertyNameFromControlName(comboBox).Replace("id", "name"), comboBox.Text);
        }
        private void ConcatPreCodeAndChildCode()
        {
            if (GlobalFunctions.PropertyExist(selectedRecord, FieldNames<RT>.ChildCode) &&
                GlobalFunctions.PropertyExist(selectedRecord, FieldNames<RT>.PreCode))
                GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<RT>.Code,
                    (string)GlobalFunctions.GetValueFromProperty(selectedRecord, FieldNames<RT>.PreCode) +
                        (string)GlobalFunctions.GetValueFromProperty(selectedRecord, FieldNames<RT>.ChildCode));
        }
        public void ShowOneRecord(long RecordId)
        {
            ShowOneRecord(RecordId, RecordParameter);
        }
        public void ShowOneRecord(RT recordParameter)
        {
            ShowOneRecord(0, recordParameter);
        }
        private void ShowOneRecord(long RecordId, RT recordParameter)
        {
            if (RecordId == 0)
                GlobalFunctions.CopyRecord(RecordParameter, recordParameter);
            else
                GlobalFunctions.SetValueToProperty<RT, long>(RecordParameter, FieldNames<RT>.ID, RecordId);
            getAllRecordsAtFirst = false;
            ShowSomeRecords(RecordParameter);
            ShowDialogForm(this);
        }
        #endregion

        #region Virtual Methods

        #region SetEnables
        public virtual void SetEnables(Boolean enable)
        {
            SetEnables_Rec(grpCurrentRow, !enable);
            if (toolBar != null)
                foreach (object control in toolBar.Items)
                    if (control is APMToolbarButton)
                    {
                        var button = control as APMToolbarButton;
                        if (
                            button.XImage == ButtonImageType.Save ||
                            button.XImage == ButtonImageType.Cancel ||
                            button.XImage == ButtonImageType.SaveTemp ||
                            button.XImage == ButtonImageType.SaveNote
                            )
                            button.IsEnabled = !enable;
                        else if (button.XImage == ButtonImageType.Help)
                            button.IsEnabled = true;
                        else
                            button.IsEnabled = enable;
                    }

            if (dataGrid != null)
                dataGrid.IsEnabled = enable;
            if (tree != null)
                tree.IsEnabled = enable;
            if (selectedNode != null)
                selectedNode.Focus();
            SetVisibilityOrEnableOfControlsBasedOnSelectedRecord();
        }
        public void SetEnables_Rec(object control, bool enable)
        {
            if (control == null)
                return;
            else if (control is Panel)
                foreach (object c in (control as Panel).Children)
                    SetEnables_Rec(c, enable);
            else if (control is ScrollViewer)
                SetEnables_Rec((control as ScrollViewer).Content, enable);
            else if (control is HeaderedContentControl)
                SetEnables_Rec((control as HeaderedContentControl).Content, enable);
            else if (control is ScrollViewer)
                SetEnables_Rec((control as ScrollViewer).Content, enable);
            else if (control is ItemsControl && !(control is ComboBox))
                foreach (object c in (control as ItemsControl).Items)
                    SetEnables_Rec(c, enable);
            else if (control is Border)
                SetEnables_Rec((control as Border).Child, enable);
            else if (control is UIElement)
                (control as UIElement).IsEnabled = enable;


        }
        #endregion

        #region SetBinding
        public virtual void SetBinding()
        {
            if (dataGrid != null && dataGrid.XOperation != null && dataGrid.XOperation != "")
                dataGrid.Adjust(bindingList);
            SetBinding_Rec(grpCurrentRow, bindingList);
        }

        public void SetBinding_Rec<T>(object control, BindingList<T> userBindingList)
        {
            if (control == null)
                return;

            TextBox textBox;
            Label label;
            CheckBox checkBox;
            APMComboBox comboBox;
            APMComboBoxCoding comboBoxCoding;
            PersianDatePicker persianDatePicker;

            string fieldName = GetPropertyNameFromControlName(control);
            if (typeof(T).GetProperty(fieldName) != null)
            {
                if (control is TextBox)
                {
                    textBox = control as TextBox;
                    textBox.SetBinding(TextBox.TextProperty, new Binding() { NotifyOnSourceUpdated = true, BindsDirectlyToSource = true, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath(fieldName) });
                    textBox.DataContext = userBindingList;
                }

                else if (control is Label)
                {
                    label = control as Label;
                    label.SetBinding(Label.ContentProperty, fieldName);
                    label.DataContext = userBindingList;
                }

                else if (control is CheckBox)
                {
                    checkBox = control as CheckBox;
                    checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding() { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath(fieldName) });
                    checkBox.DataContext = userBindingList;
                }
                else if (control is APMComboBoxCoding)
                {
                    comboBoxCoding = control as APMComboBoxCoding;
                    BLLCoding.FillComboBox(comboBoxCoding, bindingList, new stp_glb_coding_selResult() { glb_coding_category = (int)comboBoxCoding.XCategory });
                    listForComboBox.Add(comboBoxCoding);
                }
                else if (control is APMComboBox)
                {
                    comboBox = control as APMComboBox;
                    comboBox.SetBinding(ComboBox.SelectedValueProperty, fieldName);
                    comboBox.DataContext = userBindingList;
                    listForComboBox.Add(comboBox);
                }
                else if (control is PersianDatePicker)
                {
                    persianDatePicker = control as PersianDatePicker;
                    persianDatePicker.XDateTextBox.SetBinding(TextBox.TextProperty, new Binding() { NotifyOnSourceUpdated = true, BindsDirectlyToSource = true, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath(fieldName) });
                    persianDatePicker.XDateTextBox.DataContext = userBindingList;
                }
            }
            if (control is APMDocumentHeader)
            {
                (control as APMDocumentHeader).XSetBinding(userBindingList);
                SetBinding_Rec((control as APMDocumentHeader).Content, userBindingList);
            }
            else if (control is APMBrowser)
                (control as APMBrowser).XSetBinding(userBindingList, fieldName);


            else if (control is APMTwoPartCode)
                (control as APMTwoPartCode).XSetBinding(userBindingList);

            else if (control is Panel)
                foreach (object c in (control as Panel).Children)
                    SetBinding_Rec(c, userBindingList);

            else if (control is ContentControl)
                SetBinding_Rec((control as ContentControl).Content, userBindingList);

            else if (control is ItemsControl)
                foreach (object c in (control as ItemsControl).Items)
                    SetBinding_Rec(c, userBindingList);

            else if (control is Border)
                SetBinding_Rec((control as Border).Child, userBindingList);

        }
        #endregion

        #region ToolBar Button Clicks

        #region Click Event For All Buttons
        void toolBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (
                    !(sender is APMToolbarButton) ||
                    (sender as APMToolbarButton).IsEnabled == false ||
                    (sender as APMToolbarButton).Visibility == Visibility.Collapsed ||
                    (sender as APMToolbarButton).Visibility == Visibility.Hidden
                )
                return;
            switch ((sender as APMToolbarButton).XImage)
            {
                case ButtonImageType.Insert:
                    InsertClick();
                    break;
                case ButtonImageType.Edit:
                    EditClick();
                    break;
                case ButtonImageType.Delete:
                    DeleteClick();
                    break;
                case ButtonImageType.Save:
                    SaveClick(true);
                    break;
                case ButtonImageType.Cancel:
                    CancelClick();
                    break;
                case ButtonImageType.Next:
                    NextClick();
                    break;
                case ButtonImageType.Previous:
                    PreviousClick();
                    break;
                case ButtonImageType.First:
                    FirstClick();
                    break;
                case ButtonImageType.Last:
                    LastClick();
                    break;
                case ButtonImageType.Help:
                    HelpClick();
                    break;
                case ButtonImageType.Refresh:
                    RefreshClick();
                    break;
                case ButtonImageType.Select:
                    SelectClick();
                    break;
                case ButtonImageType.Search:
                    SearchClick();
                    break;
                case ButtonImageType.Print:
                    PrintClick();
                    break;
                case ButtonImageType.SaveNote:
                    SaveNoteClick();
                    break;
                case ButtonImageType.SaveTemp:
                    SaveTempClick();
                    break;
                case ButtonImageType.Confirm:
                    ConfirmClick();
                    break;
                case ButtonImageType.UnConfirm:
                    UndoConfirmClick();
                    break;
                case ButtonImageType.UseLess:
                    UseLessClick();
                    break;
                case ButtonImageType.UndoUseLess:
                    UndoUseLessClick();
                    break;
            }
        }
        #endregion

        #region EditClick
        private void EditClick(object sender, RoutedEventArgs e)
        {
            EditClick();
        }
        public virtual void EditClick()
        {
            try
            {
                if (BLL == null)
                    return;
                if (operationType != OperationType.Nothing)
                    return;
                if (!ValidationForEdit())
                    return;
                if (bindingList.Count == 0)
                {
                    Messages.NotExistsRecordForEditOrDel(OperationType.Update);
                    return;
                }
                GlobalFunctions.CopyRecord(beforeEditing, selectedRecord);
                operationType = OperationType.Update;
                SetEnables(false);
                OperationsAfterEdit();
                if (grpCurrentRow != null)
                    grpCurrentRow.Focus();
                FocusFirstControl();
            }
            catch (System.Exception ex)
            {
                Messages.ErrorMessage(ex.Message);
            }
            return;
        }
        #endregion

        #region InsertClick
        private void InsertClick(object sender, RoutedEventArgs e)
        {
            InsertClick();
        }
        public virtual void InsertClick()
        {
            try
            {
                if (BLL == null) return;
                if (operationType != OperationType.Nothing)
                    return;
                if (!ValidationForInsert())
                    return;
                bindingList.AddNew();
                ClearBindingListArticle();
                operationType = OperationType.Insert;
                SetEnables(false);
                collectionView.MoveCurrentToLast();
                if (grpCurrentRow != null)
                    grpCurrentRow.Focus();
                OperationsAfterInsert();
                SetVisibilityOrEnableOfControlsBasedOnSelectedRecord();
                MoveCollectionView();
                FocusFirstControl();
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }

        #endregion

        #region InsertChildClick
        private void InsertChildClick(object sender, RoutedEventArgs e)
        {
            InsertChildClick();
        }
        public virtual void InsertChildClick()
        {
            parentNode = (tree.SelectedItem as APMTreeViewItem);
            if (parentNode == null)
            {
                Messages.InformationMessage("لطفا یک عنصر را انتخاب کنید");
                return;
            }
            parentRecord = (RT)parentNode.Tag;
            FillBindingListFromTree();
            if (collectionView.Count > 0)
                collectionView.MoveCurrentToFirst();
            InsertClick();
        }
        #endregion

        #region SaveClick
        public virtual SaveResult SaveClick(Boolean askForConfirm)
        {
            try
            {
                if (operationType == OperationType.Nothing)
                    return SaveResult.Cancelled;
                if (BLL == null)
                    return SaveResult.Cancelled;
                SetSelectedRecordFromComboBoxes();
                InitializationBeforeSave();
                ConcatPreCodeAndChildCode();
                MoveCollectionView();
                if (!ValidationForSave())
                    return SaveResult.Cancelled;
                SaveResult result = BLL.SaveRecord(selectedRecord, operationType, askForConfirm);
                switch (result)
                {

                    case SaveResult.Saved:
                        result = SaveArticles();
                        if (result != SaveResult.Saved)
                            break;
                        OperationsAfterSaved();
                        SetVisibilityOrEnableOfControlsBasedOnSelectedRecord();
                        if (dataGrid != null)
                            dataGrid.Items.Refresh();
                        if (operationType == OperationType.Insert)
                            allRecords.Add(selectedRecord);
                        operationType = OperationType.Nothing;
                        SetEnables(true);
                        MoveCollectionView();
                        break;

                    case SaveResult.DontSave:
                        CancelClick();
                        break;

                    case SaveResult.Cancelled:
                        break;
                }
                return result;
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
                return SaveResult.Cancelled;
            }

        }
        #endregion

        #region CancelClick
        public virtual void CancelClick()
        {
            try
            {
                if (BLL == null)
                {
                    this.Close();
                    return;
                }
                if (operationType == OperationType.Nothing)
                    return;
                if (operationType == OperationType.Insert)
                {
                    bindingList.CancelNew(collectionView.CurrentPosition);
                }
                else
                {
                    MoveCollectionView();
                    GlobalFunctions.CopyRecord(selectedRecord, beforeEditing);
                }
                OperationsAfterCanceled();
                if (dataGrid != null)
                    dataGrid.Items.Refresh();
                operationType = OperationType.Nothing;
                SetEnables(true);
                MoveCollectionView();
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }
        #endregion

        #region DeleteClick
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            DeleteClick();
        }
        public virtual void DeleteClick()
        {
            try
            {
                if (BLL == null) return;
                if (operationType != OperationType.Nothing)
                    return;
                if (bindingList.Count == 0)
                {
                    Messages.NotExistsRecordForEditOrDel(OperationType.Delete);
                    return;
                }
                if (!ValidationForDelete())
                    return;
                if (BLL.DeleteRecord(selectedRecord))
                {
                    var deletingRecord = selectedRecord;
                    OperationsAfterDelete();
                    bindingList.Remove(deletingRecord);
                    allRecords.Remove(deletingRecord);
                }
            }
            catch (System.Exception ex)
            {
                Messages.ErrorMessage(ex.Message);
            }
        }
        #endregion

        #region PreviousClick
        public virtual void PreviousClick()
        {
            if (operationType != OperationType.Nothing)
                return;
            if (collectionView != null && collectionView.CurrentPosition != 0)
                collectionView.MoveCurrentToPrevious();
        }
        #endregion

        #region NextClick
        public virtual void NextClick()
        {
            if (operationType != OperationType.Nothing)
                return;
            if (collectionView != null && collectionView.CurrentPosition != collectionView.Count - 1)
                collectionView.MoveCurrentToNext();
        }
        #endregion

        #region First
        public virtual void FirstClick()
        {
            if (operationType != OperationType.Nothing)
                return;
            if (collectionView != null)
                collectionView.MoveCurrentToFirst();
        }
        #endregion

        #region LastClick
        public virtual void LastClick()
        {
            if (operationType != OperationType.Nothing)
                return;
            if (collectionView != null)
                collectionView.MoveCurrentToLast();
        }
        #endregion

        #region HelpClick
        public virtual void HelpClick()
        {

        }
        #endregion

        #region RefreshClick
        public virtual void RefreshClick()
        {
            if (BLL == null) return;
            if (operationType != OperationType.Nothing)
                return;
            GlobalFunctions.ListToBindingList(BLL.RefreshData(), bindingList, collectionView);
            allRecords = bindingList.ToList();
            if (bindingList.Count == 0)
                selectedRecord = Activator.CreateInstance<RT>();

        }
        #endregion

        #region SelectClick
        public virtual void SelectClick() { }
        #endregion

        #region PrintClick
        public virtual void PrintClick()
        {

        }
        #endregion

        #region SearchClick
        public virtual void SearchClick()
        {
            if (
                    searchForm == null ||
                    searchForm.documentType == null ||
                    operationType != OperationType.Nothing
                )
                return;
            CreateSearchForm();
            if (!ShowDialogForm(searchForm))
                return;
            ShowSomeRecords(searchForm.selectedRecord);
        }
        #endregion

        #region SaveNoteClick
        public virtual void SaveNoteClick()
        { }
        #endregion

        #region SaveTempClick
        public virtual void SaveTempClick()
        { }
        #endregion

        #region ConfirmClick
        public virtual void ConfirmClick()
        { }
        #endregion

        #region UndoConfirmClick
        public virtual void UndoConfirmClick()
        { }
        #endregion

        #region UseLessClick
        public virtual void UseLessClick()
        { }
        #endregion

        #region UndoUseLessClick
        public virtual void UndoUseLessClick()
        { }
        #endregion

        #endregion

        #region Other
        public virtual Boolean ValidationForInsert() { return true; }
        public virtual Boolean ValidationForSelect() { return true; }
        public virtual Boolean ValidationForDelete() { return true; }
        public virtual Boolean ValidationForSave() { return true; }
        public virtual Boolean ValidationForEdit() { return true; }
        public virtual void OperationsAfterInsert() { }
        public virtual void OperationsAfterEdit() { }
        public virtual void OperationsAfterSaved() { }
        public virtual void OperationsAfterCanceled() { }
        public virtual void OperationsAfterDelete() { }
        public virtual void OperationsAfterBrowse() { }
        public virtual void InitializationBeforeSave() { }
        public virtual void CreateSearchForm()
        {
            searchForm = Activator.CreateInstance<WindowSearch<RT>>();
        }
        public virtual void ClearBindingListArticle() { }
        public virtual void MoveCollectionViewArticle() { }
        public virtual void SetVisibilityOrEnableOfControlsBasedOnSelectedRecord()
        {
            if (grpCurrentRow != null)
            {
                if (tree != null && operationType == OperationType.Insert)
                    grpCurrentRow.Header = "ایجاد زیر مجموعه برای " + GlobalFunctions.GetValueFromProperty<RT, string>(parentRecord, FieldNames<RT>.Name);
                else if (collectionView != null && !(grpCurrentRow is APMDocumentHeader))
                    grpCurrentRow.Header = collectionView.Count != 0 ? "مشخصات " + GlobalFunctions.GetValueFromProperty<RT, string>(selectedRecord, FieldNames<RT>.Name) : "مشخصات ";
            }
        }
        public virtual void ShowSomeRecords(RT inputRecord)
        {
            GlobalFunctions.ListToBindingList(BLL.GetSomeRecords_DB(inputRecord), bindingList, collectionView);
            allRecords = bindingList.ToList();
            if (bindingList.Count == 0)
                selectedRecord = Activator.CreateInstance<RT>();
        }
        public virtual void LoadDataFromDB()
        {
            if (BLL != null && getAllRecordsAtFirst)
                ShowSomeRecords(RecordParameter);
        }
        public virtual void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (toolBar == null)
                return;
            switch (e.Key)
            {
                case Key.Add: toolBarButton_Click(toolBar.XInsertButton, null);
                    e.Handled = true;
                    break;
                case Key.Subtract: toolBarButton_Click(toolBar.XDeleteButton, null);
                    break;
                case Key.F4: toolBarButton_Click(toolBar.XEditButton, null);
                    break;
                case Key.F5: toolBarButton_Click(toolBar.XRefreshButton, null);
                    break;
                case Key.Down: toolBarButton_Click(toolBar.XNextButton, null);
                    break;
                case Key.Up: toolBarButton_Click(toolBar.XPreviousButton, null);
                    break;
                case Key.F7: toolBarButton_Click(toolBar.XSearchButton, null);
                    break;
                case Key.F11: toolBarButton_Click(toolBar.XSaveButton, null);
                    toolBarButton_Click(toolBar.XSaveTempButton, null);
                    break;
                case Key.F12: toolBarButton_Click(toolBar.XSaveNoteButton, null);
                    break;
                case Key.Escape:
                    if (operationType == OperationType.Nothing)
                        this.Close();
                    else
                        toolBarButton_Click(toolBar.XCancelButton, null);

                    break;
                case Key.F1: toolBarButton_Click(toolBar.XHelpButton, null);
                    break;
            }
        }
        public virtual void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetEnables(true);
            LoadDataFromDB();

        }
        public virtual void collectionView_CurrentChanged(object sender, EventArgs e)
        {
            if ((collectionView.CurrentPosition != -1) && (!GlobalVariables.collectionViewIsMoving))
                ChangeSelectedRecord((RT)collectionView.CurrentItem, collectionView);
        }
        public virtual void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null && dataGrid.SelectedItem.GetType() == typeof(RT))
                ChangeSelectedRecord((RT)dataGrid.SelectedItem, dataGrid);
        }
        public virtual void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.XMultiSelect == false)
                SelectClick();
        }
        public virtual void Window_Closing(object sender, CancelEventArgs e)
        {
            if (operationType == OperationType.Nothing)
                // if operation is nothing then program doesn't
                return;                                     // show any messages and closes the form.

            MessageBoxResult result = Messages.QuestionMessage_YesNoCancel(MessagesForUser.ClosingMessage);
            // if operation is adding or editing program shows a message for 
            // user with this content : "Do you want to save your changes."

            if (result == MessageBoxResult.Cancel)          // if user pressed Cancel button
            {                                               // then do nothing (form is still open)
                e.Cancel = true;
                return;
            }

            if (result == MessageBoxResult.Yes)             // if user pressed yes button then
            {                                               // save the operation
                if (SaveClick(false) != SaveResult.Saved)   // if there is problem with saving 
                    e.Cancel = true;                        // then program doesn't close the form.
            }
            else if (result == MessageBoxResult.No)
            {
                CancelClick();
            }
            // if user pressed 'no' button then program cancel 
            // saving the operaton and closes the form.

        }
        public virtual SaveResult SaveArticles() { return SaveResult.Saved; }
        public virtual Boolean ChangeSelectedRecord(RT newSelectedRecord, object caller)
        {
            if (!ChangeSelectedRecordIsEnable)
                return false;
            ChangeSelectedRecordIsEnable = false;
            if (dataGrid == null)
                dataGrid = new APMDataGrid();
            selectedRecord = newSelectedRecord;

            if (caller is APMTree) /* Caller Is Tree */
            {
                foreach (RT record in bindingList)
                    if (GlobalFunctions.GetValueFromProperty<RT, long>(record, FieldNames<RT>.ID) ==
                        GlobalFunctions.GetValueFromProperty<RT, long>(selectedRecord, FieldNames<RT>.ID))
                    {
                        collectionView.MoveCurrentTo(record);
                        dataGrid.SelectedItem = record;
                        break;
                    }
            }
            else /* Caller is CollectionView Or DataGrid */
            {
                if (tree != null && parentNode != null)
                    foreach (APMTreeViewItem node in parentNode.Items)
                        if (GlobalFunctions.GetValueFromProperty<RT, long>((RT)node.Tag, FieldNames<RT>.ID) == GlobalFunctions.GetValueFromProperty<RT, long>(selectedRecord, FieldNames<RT>.ID))
                        {
                            SelectTreeNode(tree, node);
                            break;
                        }

                collectionView.MoveCurrentTo(selectedRecord);
                dataGrid.SelectedIndex = collectionView.CurrentPosition;
            }
            SetVisibilityOrEnableOfControlsBasedOnSelectedRecord();
            ChangeSelectedRecordIsEnable = true;
            return true;
        }

        #endregion

        #endregion

        #region Tree

        #region MakeTree
        public void MakeTree()
        {
            MakeTree<RT>(0);
        }
        public void MakeTree(int levelNo)
        {
            MakeTree<RT>(levelNo);
        }
        public void MakeTree<T>()
        {
            MakeTree<T>(0);
        }
        public void MakeTree<T>(int levelNo)
        {
            if (typeof(T) == typeof(RT))
                allRecords = BLL.GetSomeRecords_DB(RecordParameter);
            else
            {
                var list = new BLL<T>().GetAllRecords_DB();
                if (list == null)
                    return;
                allRecords.Clear();
                foreach (var treeRecord in list)
                {
                    RT gridRecord = Activator.CreateInstance<RT>();
                    GlobalFunctions.CopyRecord(gridRecord, treeRecord);
                    allRecords.Add(gridRecord);
                }
            }
            tree.Items.Clear();
            if (allRecords == null)
                return;
            ListForMakingTree.Clear();
            foreach (var rec in allRecords)
                ListForMakingTree.Add(rec);
            if (levelNo == 0)
            {
                RT RootRecord = Activator.CreateInstance<RT>();
                GlobalFunctions.SetValueToProperty(RootRecord, FieldNames<RT>.Name, tree.XCaption);
                GlobalFunctions.SetValueToProperty(RootRecord, FieldNames<RT>.IsGroup, true);
                GlobalFunctions.SetValueToProperty(RootRecord, FieldNames<RT>.LevelNo, (int)0);
                if (tree.XTreeType == TreeType.MultiSelect_All || tree.XTreeType == TreeType.MultiSelect_LastNode)
                    GlobalFunctions.SetValueToProperty(RootRecord, FieldNames<RT>.Selected, false);
                AddNodeToTree_rec(AddNodeToTree(null, RootRecord), GlobalFunctions.GetValueFromProperty<RT, long>(RootRecord, FieldNames<RT>.ID), GlobalFunctions.GetValueFromProperty<RT, int>(RootRecord, FieldNames<RT>.LevelNo));
                if (tree.Items.Count > 0)
                    (tree.Items[0] as APMTreeViewItem).IsExpanded = true;
            }
            else
                for (int i = 0; i < ListForMakingTree.Count(); i++)
                {
                    RT record = ListForMakingTree[i];
                    if (GlobalFunctions.GetValueFromProperty<RT, int>(record, FieldNames<RT>.LevelNo) == levelNo)
                        AddNodeToTree_rec(AddNodeToTree(null, record), GlobalFunctions.GetValueFromProperty<RT, long>(record, FieldNames<RT>.ID), levelNo);
                }
            tree.XInsertChildClick -= InsertChildClick;
            tree.XInsertChildClick += InsertChildClick;
            tree.XEditClick -= EditClick;
            tree.XEditClick += EditClick;
            tree.XDeleteClick -= DeleteClick;
            tree.XDeleteClick += DeleteClick;
            tree.SelectedItemChanged += tree_SelectedItemChanged;
            if (tree.Items.Count > 0)
                if ((tree.Items[0] as TreeViewItem).Items.Count > 0 && !(this is WindowSelect<RT>))
                    SelectTreeNode(tree, (tree.Items[0] as APMTreeViewItem).Items[0] as APMTreeViewItem);
                else
                    SelectTreeNode(tree, tree.Items[0] as APMTreeViewItem);
        }
        #endregion

        #region AddNodeToTree_rec
        public void AddNodeToTree_rec(APMTreeViewItem node, long parentID, int LevelNo)
        {
            for (int i = 0; i < ListForMakingTree.Count(); i++)
            {
                RT record = ListForMakingTree[i];
                if ((node != null && GlobalFunctions.GetValueFromProperty<RT, long>(record, FieldNames<RT>.ParentID) == parentID) && GlobalFunctions.GetValueFromProperty<RT, int>(record, FieldNames<RT>.LevelNo) == (int)(LevelNo + 1))
                {
                    int saveCount = ListForMakingTree.Count;
                    AddNodeToTree_rec(AddNodeToTree(node, record), GlobalFunctions.GetValueFromProperty<RT, long>(record, FieldNames<RT>.ID), GlobalFunctions.GetValueFromProperty<RT, int>(record, FieldNames<RT>.LevelNo));
                    i = Math.Max(i - (saveCount - ListForMakingTree.Count), -1);
                }
            }
        }
        #endregion

        #region AddNodeToTree
        public APMTreeViewItem AddNodeToTree(APMTreeViewItem parentNode, RT rec)
        {
            RT rec_copy = Activator.CreateInstance<RT>();
            GlobalFunctions.CopyRecord(rec_copy, rec);
            int levelNo = GlobalFunctions.GetValueFromProperty<RT, int>(rec, FieldNames<RT>.LevelNo);
            Brush x;
            APMTreeViewItem newNode = new APMTreeViewItem()
            {
                Tag = rec_copy,
                XCaption = GlobalFunctions.GetValueFromProperty<RT, string>(rec, FieldNames<RT>.Name),
                XHaveCheckBox = (tree.XTreeType == TreeType.MultiSelect_LastNode)
                    ? !GlobalFunctions.GetValueFromProperty<RT, Boolean>(rec, FieldNames<RT>.IsGroup)
                    : (GlobalFunctions.GetValueFromProperty<RT, long>(rec, FieldNames<RT>.ID) != 0 && tree.XTreeType == TreeType.MultiSelect_All),
                XIsChecked = GlobalFunctions.GetValueFromProperty<RT, Boolean>(rec, FieldNames<RT>.Selected),
                Foreground = (x = APMComponents.SubSystemColors.Items
                    [(int)GlobalVariables.currentSubSystem].TreeColors[levelNo]) != null
                    ? x : Brushes.Black,
                FontSize = (levelNo <= 3) ? FontSize + 1 : FontSize
            };

            if (parentNode != null)
                parentNode.Items.Add(newNode);
            else
                tree.Items.Add(newNode);

            if (GlobalFunctions.GetValueFromProperty<RT, Boolean>((RT)newNode.Tag, FieldNames<RT>.Selected) && (this is WindowSelect<RT>))
            {

                (this as WindowSelect<RT>).selectedListBeforeChange.Add((RT)newNode.Tag);
                (this as WindowSelect<RT>).selectedListAfterChange.Add((RT)newNode.Tag);
                ExpandSelected(newNode);
            }
            if (levelNo > tree.XLevelCount)
                tree.XLevelCount = levelNo;
            ListForMakingTree.Remove(rec);
            return newNode;
        }
        #endregion AddNodeToTree

        #region ExpandSelected
        public void ExpandSelected(APMTreeViewItem Node)
        {
            if (Node == null)
                return;
            while ((Node.Parent as APMTreeViewItem) != null)
            {
                Node = Node.Parent as APMTreeViewItem;
                Node.IsExpanded = true;
            }
        }
        #endregion

        #region selectTreeNode
        public void SelectTreeNode(APMTree tree, APMTreeViewItem node)
        {
            if (tree == null || node == null)
                return;
            tree.Focus();
            node.Focus();
            node.IsSelected = true;
            if (node.Parent != null && node.Parent is APMTreeViewItem)
                (node.Parent as APMTreeViewItem).IsExpanded = true;
        }
        #endregion

        #region tree_SelectedItemChanged
        public virtual void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e != null)
                e.Handled = true;
            if (!GlobalFunctions.PropertyExist(selectedRecord, FieldNames<RT>.ParentID))
                return;
            if (tree.SelectedItem == null)
                return;
            selectedNode = ((tree.SelectedItem) as APMTreeViewItem);
            var saveParentNode = parentNode;
            parentNode = (selectedNode.Parent) as APMTreeViewItem;
            if (parentNode != null && parentNode.Tag != null)
                parentRecord = (RT)parentNode.Tag;

            if (saveParentNode != parentNode)
            {
                ChangeSelectedRecordIsEnable = false;
                FillBindingListFromTree();
                ChangeSelectedRecordIsEnable = true;
            }
            ChangeSelectedRecord((RT)selectedNode.Tag, tree);
        }
        #endregion

        #region FillBindingListFromTree
        public virtual void FillBindingListFromTree()
        {
            bindingList.Clear();
            if (parentNode == null)
                bindingList.Add((RT)selectedNode.Tag);
            else
                foreach (APMTreeViewItem node in parentNode.Items)
                    bindingList.Add((RT)node.Tag);
            allRecords = bindingList.ToList();
        }
        #endregion

        #endregion
    }
}
