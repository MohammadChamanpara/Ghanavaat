using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using APMTools;
using APMComponents;
using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.Windows.Controls;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;

namespace UserInterfaceLayer
{
    public class WindowTwoTabs<MasterRT, ArticleRT> : WindowBase<MasterRT>
    {
        #region Global Variables
        public ArticleRT selectedArticle, beforeEditingArticle;
        private List<ArticleRT> ArticlesList = new List<ArticleRT>();
        private BLL<ArticleRT> BLLArticle;
        public APMBindingList<ArticleRT> bindingListArticle = new APMBindingList<ArticleRT>();
        private ArticlePackage<ArticleRT, ArticleRT> articlePackage = new ArticlePackage<ArticleRT, ArticleRT>();
        private TabControl tabControl;
        private APMDataGrid articleGrid;
        private APMDataGridExtended articleDataGrid;
        private APMGroupBoxExtended grpCurrentRowArticle;
        private string xOperationArticle;
        private APMDocumentHeader documentHeader;
        private TextBox txtCount, txtPrice;
        private TextBox txtDescription;
        private Label lblCount, lblPrice;
        private APMComboBox cmbMeasure;
        private List<APMToolbarButton> toolbarButtons = new List<APMToolbarButton>();
        private List<int> collapsingColumns = new List<int>();
        private int? pricePosition, all_pricePosition;
        public Boolean? isFinancial;
        public Boolean changeSelectedRecordArticleIsEnable = true;
        public ReportClass printReportFile;
        private Boolean creatingDocument;
        private Boolean createdDocumentSaved = false;

        #endregion

        #region Constructor
        public WindowTwoTabs(Boolean? isFinancial)
        {
            this.isFinancial = isFinancial;
        }
        public WindowTwoTabs()
        {
            this.isFinancial = null;
        }
        #endregion

        #region Initial_WindowTwoTabs
        public void Initial_WindowTwoTab
        (
            APMDocumentHeader documentHeader, TextBox txtCount, Label lblCount, TextBox txtPrice, Label lblPrice, APMComboBox cmbMeasure, APMDataGridExtended masterDataGrid, APMDataGridExtended articleDataGrid,
            APMToolBar masterToolbar, GroupBox masterCurrentRowGroupBox, APMGroupBoxExtended articleCurrentRowGroupBox,
            string masterXOperation, string articleXOperation, TabControl tabControl,
            TextBox txtDescription, int? pricePosition, int? all_pricePosition, ReportClass printReportFile, params int[] collapsingColumnsWhenDontHaveBaseDocument
        )
        {
            this.WindowState = WindowState.Maximized;
            BLLArticle = Activator.CreateInstance<BLL<ArticleRT>>();
            grpCurrentRowArticle = articleCurrentRowGroupBox;
            if (grpCurrentRowArticle != null)
                grpCurrentRowArticle.XCanCollapse = true;
            beforeEditingArticle = Activator.CreateInstance<ArticleRT>();
            this.txtDescription = txtDescription;
            this.txtCount = txtCount;
            this.lblCount = lblCount;
            this.txtPrice = txtPrice;
            this.lblPrice = lblPrice;
            this.pricePosition = pricePosition;
            this.all_pricePosition = all_pricePosition;
            this.cmbMeasure = cmbMeasure;
            this.documentHeader = documentHeader;
            this.articleGrid = articleDataGrid.datagrid;
            this.articleDataGrid = articleDataGrid;
            this.xOperationArticle = articleXOperation;
            this.tabControl = tabControl;
            this.KeyUp += Window_KeyUp;
            this.printReportFile = printReportFile;
            toolbarButtons.Clear();
            FindToolBarButtons(grpCurrentRowArticle);
            foreach (APMToolbarButton button in toolbarButtons)
                button.Click += toolbarArticle_Click;
            Initial_WindowBase(masterDataGrid, masterToolbar, masterCurrentRowGroupBox, masterXOperation, false, new WindowSearch<MasterRT>());

            if (bindingListArticle != null)
            {
                collectionViewArticle = (CollectionView)CollectionViewSource.GetDefaultView(bindingListArticle);
                collectionViewArticle.CurrentChanged += collectionViewArticle_CurrentChanged;
            }

            if (articleDataGrid != null)
                articleDataGrid.datagrid.SelectionChanged += articleGrid_SelectionChanged;

            documentHeader.XBaseDocument_Changed += new RoutedEventHandler(DocumentHeader_XBaseDocument_Changed);

            collapsingColumns.Clear();
            if (collapsingColumnsWhenDontHaveBaseDocument != null)
                foreach (int c in collapsingColumnsWhenDontHaveBaseDocument)
                    collapsingColumns.Add(c);
            if (cmbMeasure != null)
                cmbMeasure.LostFocus += new RoutedEventHandler(cmbMeasure_LostFocus);

            if (txtDescription != null)
                txtDescription.KeyDown += new KeyEventHandler(lastTextBox_KeyDown);

            documentHeader.XTextBoxKeyDown_MainStore += new KeyEventHandler(documentHeader_XTextBoxKeyDown_MainStore);
            documentHeader.XTextBoxKeyDown_BaseBuyRequest += new KeyEventHandler(documentHeader_XTextBoxKeyDown_BaseBuyRequest);
            documentHeader.XTextBoxKeyDown_BaseGoodsRequest += new KeyEventHandler(documentHeader_XTextBoxKeyDown_BaseGoodsRequest);
            documentHeader.XTextBoxKeyDown_GoodsRequesterCostCenter += new KeyEventHandler(documentHeader_XTextBoxKeyDown_GoodsRequesterCostCenter);
            documentHeader.XTextBoxKeyDown_GoodsRequesterPersonel += new KeyEventHandler(documentHeader_XTextBoxKeyDown_GoodsRequesterPersonel);
            documentHeader.XTextBoxKeyDown_RequestConfirmerPersonel += new KeyEventHandler(documentHeader_XTextBoxKeyDown_RequestConfirmerPersonel);
            documentHeader.XTextBoxKeyDown_DestinationDetail += new KeyEventHandler(documentHeader_XTextBoxKeyDown_DestinationDetail);

            if (tabControl != null)
                tabControl.SelectedIndex = 1;
            SetUpDownkeyFuntionForTextBoxes(articleCurrentRowGroupBox);
            if (isFinancial != null && isFinancial == true)
                articleDataGrid.XShowTotalRials = true;
            if (isFinancial != null && txtPrice != null && pricePosition != null && all_pricePosition != null)
            {
                articleDataGrid.datagrid.Columns[(int)pricePosition].Visibility = GlobalFunctions.BooleanToVisibility((bool)isFinancial);
                articleDataGrid.datagrid.Columns[(int)all_pricePosition].Visibility = GlobalFunctions.BooleanToVisibility((bool)isFinancial);
                if (isFinancial == false)
                {
                    var columnDefinitionsIndex = ((int)txtPrice.GetValue(Grid.ColumnProperty)) - 1;
                    (txtPrice.Parent as Grid).ColumnDefinitions[columnDefinitionsIndex].Width = new GridLength(0);
                }
                GlobalFunctions.SetVisibilityForControl(txtPrice, (bool)isFinancial);
                GlobalFunctions.SetVisibilityForControl(lblPrice, (bool)isFinancial);
            }
        }

        #endregion

        #region Overrided Methods
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            SetEnablesForArticleGroupBox();
        }
        public override void SetBinding()
        {
            base.SetBinding();
            if (articleGrid != null && xOperationArticle != null && xOperationArticle != "")
            {
                articleGrid.XOperation = xOperationArticle;
                articleGrid.Adjust(bindingListArticle);
            }
            SetBinding_Rec(grpCurrentRowArticle, bindingListArticle);
            if (articleDataGrid != null)
                articleDataGrid.XSetBinding(bindingList);
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            articlePackage.ListBeforeChange.Clear();
            tabControl.SelectedIndex = 1;
            bindingListArticle.Clear();
            documentHeader.XHaveBaseDocument = false;
            collectionViewArticle.MoveCurrentToLast();
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<MasterRT>.RegisterDate, APMDateTime.Today);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<MasterRT>.RegisterTime, APMDateTime.SystemTime);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<MasterRT>.RegistererUserId, GlobalVariables.current_user_id);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<MasterRT>.RegistererUserName, GlobalVariables.current_user_name);
            GlobalFunctions.SetValueToProperty(selectedRecord, FieldNames<MasterRT>.Date, APMDateTime.Today);
            if (bindingListArticle.Count == 0)
                InsertArticleClick();
        }
        public override void OperationsAfterEdit()
        {
            articlePackage.ListBeforeChange.Clear();
            foreach (ArticleRT article in bindingListArticle)
            {
                ArticleRT record = Activator.CreateInstance<ArticleRT>();
                GlobalFunctions.CopyRecord(record, article);
                articlePackage.ListBeforeChange.Add(record);
            }
            tabControl.SelectedIndex = 1;
            if (bindingListArticle.Count == 0 && documentHeader.XHaveBaseDocument == false)
            {
                bindingListArticle.AddNew();
                collectionViewArticle.MoveCurrentToFirst();
            }
        }
        public override void InitializationBeforeSave()
        {
            MoveCollectionView(collectionViewArticle);
            if (documentHeader.XType != DocumentTypes.AccountingDocument && !documentHeader.XHaveBaseDocument)
            {
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_request_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_buy_request_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_receive_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_send_selResult());
                foreach (ArticleRT article in bindingListArticle)
                {
                    GlobalFunctions.Copy_PK_To_FK(article, new stp_inv_goods_request_article_selResult());
                    GlobalFunctions.Copy_PK_To_FK(article, new stp_inv_buy_request_article_selResult());
                    GlobalFunctions.Copy_PK_To_FK(article, new stp_inv_goods_receive_article_selResult());
                    GlobalFunctions.Copy_PK_To_FK(article, new stp_inv_goods_send_article_selResult());
                }
                CalculateGridTotalFields(articleDataGrid.datagrid);
            }

            articlePackage.ListAfterChange.Clear();
            foreach (ArticleRT article in bindingListArticle)
                articlePackage.ListAfterChange.Add(article);
            MoveCollectionView(collectionViewArticle);
        }
        public override SaveResult SaveArticles()
        {
            return articlePackage.Save(selectedRecord);
        }
        public override void OperationsAfterSaved()
        {
            DDB.Transaction.CommitTransaction();
            createdDocumentSaved = true;
        }
        public override void MoveCollectionViewArticle()
        {
            MoveCollectionView(collectionViewArticle);
        }
        public override void OperationsAfterBrowse()
        {
            MoveCollectionView(collectionViewArticle);
            articleGrid.Items.Refresh();
        }
        public override bool ValidationForEdit()
        {
            if (!ValidationForChange())
                return false;
            return base.ValidationForEdit();
        }
        public override bool ValidationForDelete()
        {
            if (!ValidationForChange())
                return false;
            return base.ValidationForDelete();
        }
        public override bool ValidationForSave()
        {
            DDB.Transaction.BeginTransaction();
            return true;
        }
        public override SaveResult SaveClick(bool askForConfirm)
        {
            var result = base.SaveClick(askForConfirm);
            if (result != SaveResult.Saved)
                DDB.Transaction.RollBackTransaction();
            return result;

        }
        public override void DeleteClick()
        {
            base.DeleteClick();
            if (bindingList.Count == 0)
                bindingListArticle.Clear();
        }
        public override void ClearBindingListArticle()
        {
            bindingListArticle.Clear();
        }
        public override bool ChangeSelectedRecord(MasterRT newSelectedRecord, object caller)
        {
            if (!base.ChangeSelectedRecord(newSelectedRecord, caller))
                return false;
            ShowArticles();
            return true;
        }
        public override void CancelClick()
        {
            base.CancelClick();
            DDB.Transaction.RollBackTransaction();
            ChangeSelectedRecord(selectedRecord, collectionView);
        }
        public override void PrintClick()
        {
            if (printReportFile == null)
                return;
            var printForm = new WindowPrint<MasterRT, ArticleRT>(printReportFile);
            List<ArticleRT> listForPrint = new List<ArticleRT>();
            foreach (ArticleRT article in articleDataGrid.datagrid.Items)
                listForPrint.Add(article);
            printForm.articleList = listForPrint;
            printForm.selectedRecord = this.selectedRecord;
            printForm.ShowDialog();
        }
        public override void ShowSomeRecords(MasterRT inputRecord)
        {
            base.ShowSomeRecords(inputRecord);
            if (bindingList.Count == 0)
            {
                bindingListArticle.Clear();
                ArticlesList.Clear();
            }
        }
        public override void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Escape)
                CancelArticleClick();
            else
                base.Window_KeyDown(sender, e);
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (creatingDocument == false)
                base.Window_Loaded(sender, e);

        }

        #endregion

        #region Virtual Methods
        public virtual void OperationsAfterInsertArticle() { }
        public virtual void ChangeSelectedRecordArticle(ArticleRT newSelectedRecord)
        {
            if
            (
                GlobalVariables.collectionViewArticleIsMoving ||
                !changeSelectedRecordArticleIsEnable
            )
                return;
            changeSelectedRecordArticleIsEnable = false;
            SetAllPrice();
            SetCountMeasure();
            selectedArticle = newSelectedRecord;
            if
            (
                GlobalFunctions.GetValueFromProperty<ArticleRT, long>(beforeEditingArticle, FieldNames<ArticleRT>.ID) !=
                GlobalFunctions.GetValueFromProperty<ArticleRT, long>(selectedArticle, FieldNames<ArticleRT>.ID)
            )
                GlobalFunctions.CopyRecord(beforeEditingArticle, selectedArticle);
            collectionViewArticle.MoveCurrentTo(selectedArticle);
            articleGrid.SelectedIndex = collectionViewArticle.CurrentPosition;

            if (cmbMeasure != null)
            {
                var saveValue = GlobalFunctions.GetValueFromProperty<ArticleRT, long>(selectedArticle, FieldNames<ArticleRT>.MeasureId);
                new BLL<stp_glb_measure_selResult>().FillComboBox(cmbMeasure, bindingListArticle,
                    new stp_glb_measure_selResult()
                    {
                        glb_measure_inv_group_goods_id = GlobalFunctions.GetValueFromProperty<ArticleRT, long>(selectedArticle, FieldNames<ArticleRT>.GroupGoodsId)
                    });
                cmbMeasure.SelectedValue = saveValue;
                if (cmbMeasure.SelectedIndex == -1)
                {
                    cmbMeasure.SelectedValue = null;
                    GlobalFunctions.SetValueToProperty<ArticleRT, long?>(selectedArticle, FieldNames<ArticleRT>.MeasureId, null);
                }
            }
            SetCountMeasure();
            CalculateGridTotalFields(articleGrid);
            changeSelectedRecordArticleIsEnable = true;
        }
        public virtual void ShowArticles()
        {
            var parameters = Activator.CreateInstance<ArticleRT>();
            GlobalFunctions.Copy_PK_To_FK(parameters, selectedRecord);
            ArticlesList = BLLArticle.GetSomeRecords_DB(parameters);
            GlobalFunctions.ListToBindingList(ArticlesList, bindingListArticle, collectionViewArticle);
        }
        public virtual void collectionViewArticle_CurrentChanged(object sender, EventArgs e)
        {
            if (collectionViewArticle.CurrentPosition != -1)
                ChangeSelectedRecordArticle((ArticleRT)collectionViewArticle.CurrentItem);
        }
        public virtual void DocumentHeader_XBaseDocument_Changed(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.collectionViewIsMoving)
                return;
            if (documentHeader == null)
                return;
            SetEnablesForArticleGroupBox();
            if (!documentHeader.XHaveBaseDocument)
            {
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_request_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_buy_request_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_receive_selResult());
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_send_selResult());
            }
            for (int i = 0; i < articleGrid.Columns.Count; i++)
                if (collapsingColumns.Contains(i))
                {
                    articleGrid.Columns[i].Visibility = GlobalFunctions.BooleanToVisibility
                    (
                        documentHeader.XHaveBaseDocument &&
                        !documentHeader.XBasedocumentIsSendOrReceive
                    );

                }
            if (operationType != OperationType.Nothing && documentHeader.XHaveBaseDocument == false && bindingListArticle.Count == 0)
            {
                InsertArticleClick();
            }

        }
        public virtual void SetEnablesForArticleGroupBox()
        {
            Boolean enableAll = operationType != OperationType.Nothing && !documentHeader.XHaveBaseDocument;
            SetEnables_Rec(grpCurrentRowArticle, enableAll);
            Boolean enableForCountAndPrice = (operationType != OperationType.Nothing);
            if (txtCount != null)
            {
                lblCount.IsEnabled = enableForCountAndPrice;
                txtCount.IsEnabled = enableForCountAndPrice;
            }
            if (txtPrice != null)
            {
                lblPrice.IsEnabled = enableForCountAndPrice;
                txtPrice.IsEnabled = enableForCountAndPrice;
            }

            foreach (APMToolbarButton button in toolbarButtons)
            {
                if
                (
                    button.XImage == ButtonImageType.Previous ||
                    button.XImage == ButtonImageType.Next ||
                    button.XImage == ButtonImageType.Refresh
                )
                    button.IsEnabled = true;
                else if
                (
                    button.XImage == ButtonImageType.Insert ||
                    button.XImage == ButtonImageType.Delete
                )
                    button.IsEnabled = operationType != OperationType.Nothing;
                else
                    button.IsEnabled = enableAll;
            }
        }
        public virtual void articleGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (articleGrid.SelectedItem != null)
                ChangeSelectedRecordArticle((ArticleRT)articleGrid.SelectedItem);
        }
        public virtual void CalculateGridTotalFields(APMDataGrid DataGrid)
        {
            Boolean haveNullArticle = false;
            if
            (
                !(DataGrid.Parent is APMDataGridExtended) ||
                (operationType == OperationType.Nothing) ||
                (documentHeader.XType == DocumentTypes.AccountingDocument)
            )
                return;
            double sumAmount = 0;
            double? sumPrice = 0;

            foreach (ArticleRT item in DataGrid.Items)
            {
                sumAmount += GlobalFunctions.GetValueFromProperty<ArticleRT, double>(item, FieldNames<ArticleRT>.Count);
                var articlePrice = GlobalFunctions.GetValueFromProperty<ArticleRT, double>(item, FieldNames<ArticleRT>.AllPrice);
                if (articlePrice == 0)
                    haveNullArticle = true;
                sumPrice += articlePrice;
            }
            GlobalFunctions.SetValueToProperty<MasterRT, double>(selectedRecord, FieldNames<MasterRT>.SumCount, sumAmount);
            GlobalFunctions.SetValueToProperty<MasterRT, double?>(selectedRecord, FieldNames<MasterRT>.SumPrice, (haveNullArticle ? null : sumPrice));
        }
        #endregion

        #region Controls Events
        void cmbMeasure_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmbMeasure.IsDropDownOpen == true)
                return;
            SetCountMeasure();
        }
        public void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (operationType == OperationType.Nothing)
                return;
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.OemPlus))
                InsertArticleClick();
            else if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.OemMinus))
                DeleteArticleClick();
            else if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.N))
                NextArticleClick();
            else if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.P))
                PreviousArticleClick();
        }
        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                PreviousArticleClick();
            else if (e.Key == Key.Down)
                NextArticleClick();
        }
        private void lastTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Tab)
                return;
            e.Handled = true;
            if(collectionViewArticle.CurrentPosition<collectionViewArticle.Count-1)
                NextArticleClick();
            else 
                InsertArticleClick();
            FocusFirstControl(grpCurrentRowArticle);
        }
        #endregion

        #region ArticleButtonsClicks
        public virtual void InsertArticleClick()
        {
            if (operationType == OperationType.Nothing)
                return;
            if (documentHeader.XHaveBaseDocument)
            {
                Messages.ErrorMessage("برای سندی که سند مبنا دارد نمی توانید آرتیکل اضافه نمایید");
                return;
            }
            if (BLLArticle == null)
                return;
            if (operationType == OperationType.Nothing)
                return;
            MoveCollectionView(collectionViewArticle);
            bindingListArticle.AddNew();
            if (bindingListArticle.Count > 1)
            {
                GlobalFunctions.SetValueToProperty(bindingListArticle[(int)collectionViewArticle.Count - 1], FieldNames<ArticleRT>.ID, 0);
            }
            collectionViewArticle.MoveCurrentToLast();
            FocusFirstControl(grpCurrentRowArticle);
            OperationsAfterInsertArticle();
        }
        public void DeleteArticleClick()
        {
            try
            {
                if (operationType == OperationType.Nothing)
                    return;
                if (bindingListArticle == null)
                    return;
                if (bindingListArticle.Count == 0)
                {
                    Messages.NotExistsRecordForEditOrDel(OperationType.Delete);
                    return;
                }

                MessageBoxResult r = Messages.DeleteMessage("ردیف");
                if (r == MessageBoxResult.No)
                    return;

                var deletingRecord = selectedArticle;
                bindingListArticle.Remove(selectedArticle);
            }
            catch (Exception ex)
            {
                Messages.ErrorMessage(ex.Message);
            }
        }
        public void NextArticleClick()
        {
            if (collectionViewArticle != null && collectionViewArticle.CurrentPosition != collectionViewArticle.Count - 1)
                collectionViewArticle.MoveCurrentToNext();
        }
        public void PreviousArticleClick()
        {
            if (collectionViewArticle != null && collectionViewArticle.CurrentPosition != 0)
                collectionViewArticle.MoveCurrentToPrevious();
        }
        public void FirstArticleClick()
        {
            if (collectionViewArticle != null)
                collectionViewArticle.MoveCurrentToFirst();
        }
        public void LastArticleClick()
        {
            if (collectionViewArticle != null)
                collectionViewArticle.MoveCurrentToLast();
        }
        public void CancelArticleClick()
        {
            MoveCollectionView(collectionViewArticle);
            if (GlobalFunctions.GetValueFromProperty<ArticleRT, long>(selectedArticle, FieldNames<ArticleRT>.ID) == 0)
                bindingListArticle.CancelNew(collectionViewArticle.CurrentPosition);
            else
            {
                GlobalFunctions.CopyRecord(selectedArticle, beforeEditingArticle);
                MoveCollectionView(collectionViewArticle);
                if (articleGrid != null)
                    articleGrid.Items.Refresh();
            }
        }
        private void toolbarArticle_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is APMToolbarButton))
                return;
            switch ((sender as APMToolbarButton).XImage)
            {
                case ButtonImageType.Insert:
                    InsertArticleClick();
                    break;
                case ButtonImageType.Delete:
                    DeleteArticleClick();
                    break;
                case ButtonImageType.Next:
                    NextArticleClick();
                    break;
                case ButtonImageType.Previous:
                    PreviousArticleClick();
                    break;
                case ButtonImageType.First:
                    FirstArticleClick();
                    break;
                case ButtonImageType.Last:
                    LastArticleClick();
                    break;
                case ButtonImageType.Cancel:
                    CancelArticleClick();
                    break;
            }
        }
        #endregion

        #region Tools
        protected void CopyArticlesFromSourceDocument<SourceArticleT>()
        {
            /* Copy Some Master Fields From Source Document To This Document */

            /* Copy Articles From Source Document To This Document */
            var sourceArticleParameter = Activator.CreateInstance<SourceArticleT>();
            GlobalFunctions.Copy_FK_To_FK(sourceArticleParameter, selectedRecord); /* Copy for example inv_goods_send_request_id(SourceRequestID) To inv_goods_request_article_inv_goods_request_id(MasterID)*/
            var sourceList = new BLL<SourceArticleT>().GetSomeRecords_DB(sourceArticleParameter);
            var destinationList = new List<ArticleRT>();
            foreach (var sourceArticle in sourceList)
            {
                var destinationArticle = Activator.CreateInstance<ArticleRT>();
                GlobalFunctions.Copy_PK_To_FK(destinationArticle, sourceArticle);
                GlobalFunctions.Copy_Same_Fields(destinationArticle, sourceArticle);

                destinationList.Add(destinationArticle);
            }
            GlobalFunctions.ListToBindingList(destinationList, bindingListArticle, collectionViewArticle);
            SetEnablesForArticleGroupBox();
        }
        public void FindToolBarButtons(object control)
        {
            if (control == null)
                return;

            if (control is APMToolbarButton)
                toolbarButtons.Add(control as APMToolbarButton);

            else if (control is Panel)
                foreach (object c in (control as Panel).Children)
                    FindToolBarButtons(c);

            else if (control is ContentControl)
                FindToolBarButtons((control as ContentControl).Content);

            else if (control is ItemsControl && !(control is ComboBox))
                foreach (object c in (control as ItemsControl).Items)
                    FindToolBarButtons(c);

            else if (control is System.Windows.Controls.Border)
                FindToolBarButtons((control as System.Windows.Controls.Border).Child);
        }
        protected void SetCountMeasure()
        {
            if (selectedArticle == null || operationType == OperationType.Nothing || cmbMeasure == null)
                return;

            GlobalFunctions.SetValueToProperty(selectedArticle, FieldNames<ArticleRT>.CountMeasure,
                 (GlobalFunctions.GetValueFromProperty<ArticleRT, double>(selectedArticle, (FieldNames<ArticleRT>.Count)))
              + " " + cmbMeasure.Text);
            articleGrid.Items.Refresh();
        }
        protected void SetAllPrice()
        {
            if
            (
                selectedArticle == null ||
                operationType == OperationType.Nothing ||
                documentHeader.XType == DocumentTypes.AccountingDocument
            )
                return;
            GlobalFunctions.SetValueToProperty(selectedArticle, FieldNames<ArticleRT>.AllPrice,
                (double)((GlobalFunctions.GetValueFromProperty<ArticleRT, double>(selectedArticle, (FieldNames<ArticleRT>.Count))) *
                ((double)GlobalFunctions.GetValueFromProperty<ArticleRT, double>(selectedArticle, (FieldNames<ArticleRT>.Price)))));
            articleGrid.Items.Refresh();
        }
        public void SetUpDownkeyFuntionForTextBoxes(object control)
        {
            if (control == null)
                return;
            if (control is TextBox)
            {
                TextBox tbox = (control as TextBox);
                KeyGesture UpKeyGesture = new KeyGesture(Key.Up);
                KeyGesture DownKeyGesture = new KeyGesture(Key.Down);
                KeyBinding Up = new KeyBinding(ApplicationCommands.NotACommand, UpKeyGesture);
                KeyBinding Down = new KeyBinding(ApplicationCommands.NotACommand, DownKeyGesture);
                tbox.InputBindings.Add(Up);
                tbox.InputBindings.Add(Down);
                tbox.KeyDown += new KeyEventHandler(txt_KeyDown);
            }

            if (control is ItemsControl)
            {
                foreach (object c in (control as ItemsControl).Items)
                    SetUpDownkeyFuntionForTextBoxes(c);
            }

            if (control is ContentControl)
            {
                object c = (control as ContentControl).Content;
                SetUpDownkeyFuntionForTextBoxes(c);
            }

            if (control is Panel)
            {
                foreach (object c in (control as Panel).Children)
                    SetUpDownkeyFuntionForTextBoxes(c);
            }
        }
        private bool ValidationForChange()
        {
            if (GlobalFunctions.GetValueFromProperty<MasterRT, Boolean>(selectedRecord, FieldNames<MasterRT>.HaveAccDocument) == true)
            {
                Messages.ErrorMessage("برای این سند، سند حسابداری درج شده است. مجاز به تغییر آن نمی باشید");
                return false;
            }
            else if (GlobalFunctions.GetValueFromProperty<MasterRT, Boolean>(selectedRecord, FieldNames<MasterRT>.HasFollowingAccDocument) == true)
            {
                Messages.ErrorMessage("برای اسناد بعدی، سند حسابداری درج شده است. مجاز به تغییر آن نمی باشید");
                return false;
            }
            return true;
        }
        public Boolean CreateDocument(MasterRT masterRecord, List<ArticleRT> articleList)
        {
            creatingDocument = true;
            getAllRecordsAtFirst = false;
            InsertClick();
            APMTools.GlobalFunctions.CopyRecord(selectedRecord, masterRecord);
            OperationsAfterInsert();
            APMTools.GlobalFunctions.ListToBindingList(articleList, bindingListArticle);
            collectionViewArticle.MoveCurrentToFirst();
            MoveCollectionView();
            MoveCollectionViewArticle();
            dataGrid.Items.Refresh();
            SetVisibilityOrEnableOfControlsBasedOnSelectedRecord();
            this.ShowDialog();
            return createdDocumentSaved;
        }
        public void ShowOneDocument(long masterID)
        {
            ShowOneDocument(masterID, 0);
        }
        public void ShowOneDocument(long masterID, long articleID)
        {
            if (masterID == 0)
                return;
            ShowOneDocument(masterID, articleID, RecordParameter);

        }
        public void ShowOneDocument(MasterRT recordParameter)
        {
            ShowOneDocument(0, 0, recordParameter);
        }
        private void ShowOneDocument(long masterID, long articleID, MasterRT recordParameter)
        {
            if (masterID == 0)
                GlobalFunctions.CopyRecord(RecordParameter, recordParameter);
            else
                GlobalFunctions.SetValueToProperty<MasterRT, long>(RecordParameter, FieldNames<MasterRT>.ID, masterID);
            getAllRecordsAtFirst = false;
            ShowSomeRecords(RecordParameter);
            if (articleID != 0)
            {
                var articleToShow = bindingListArticle.SingleOrDefault(article => GlobalFunctions.GetValueFromProperty<ArticleRT, long>(article, FieldNames<ArticleRT>.ID) == articleID);
                if (articleToShow != null)
                    collectionViewArticle.MoveCurrentTo(articleToShow);
            }
            ShowDialogForm(this);
        }
        #endregion

        #region CodeTextBox KeyDowns
        void documentHeader_XTextBoxKeyDown_MainStore(object sender, KeyEventArgs e)
        {
            var saveStoreId = GlobalFunctions.GetValueFromProperty<MasterRT, long>(selectedRecord, FieldNames<MasterRT>.StoreId);
            if (CodeTextBox_KeyDown<stp_inv_store_selResult>(sender, FieldNames<MasterRT>.StoreId, e))
                if (GlobalFunctions.GetValueFromProperty<MasterRT, long>(selectedRecord, FieldNames<MasterRT>.StoreId) != saveStoreId)
                {
                    GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_buy_request_selResult());
                    GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_request_selResult());
                    GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_receive_selResult());
                    GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_inv_goods_send_selResult());
                    MoveCollectionView();
                }
        }
        void documentHeader_XTextBoxKeyDown_GoodsRequesterPersonel(object sender, KeyEventArgs e)
        {
            stp_glb_personel_selResult record = new stp_glb_personel_selResult();
            GlobalFunctions.Copy_FK_To_FK(record, selectedRecord);
            CodeTextBox_KeyDown_Filter<stp_glb_personel_selResult, MasterRT>(sender, FieldNames<MasterRT>.RequesterPersonelId, e, selectedRecord, record);
        }
        void documentHeader_XTextBoxKeyDown_GoodsRequesterCostCenter(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Tab)
                return;
            var saveCostCenterId = GlobalFunctions.GetValueFromProperty<MasterRT, long>(selectedRecord, FieldNames<MasterRT>.CostCenterId);
            if (CodeTextBox_KeyDown<stp_glb_cost_center_selResult>(sender, FieldNames<MasterRT>.CostCenterId, e))
            {
                if (GlobalFunctions.GetValueFromProperty<MasterRT, long>(selectedRecord, FieldNames<MasterRT>.CostCenterId) != saveCostCenterId)
                {
                    GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_glb_personel_selResult(), FieldNames<MasterRT>.RequesterPersonelId);
                    MoveCollectionView();
                }
            }
        }
        void documentHeader_XTextBoxKeyDown_RequestConfirmerPersonel(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_glb_personel_selResult>(sender, FieldNames<MasterRT>.ConfirmerPersonelId, e);
        }
        void documentHeader_XTextBoxKeyDown_BaseGoodsRequest(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Tab)
                return;

            stp_inv_goods_request_selResult record = new stp_inv_goods_request_selResult();
            GlobalFunctions.Copy_FK_To_FK(record, selectedRecord);
            CodeTextBox_KeyDown_Filter<stp_inv_goods_request_selResult, MasterRT>(sender, FieldNames<MasterRT>.GoodsRequestId, e, selectedRecord, record);
            CopyArticlesFromSourceDocument<stp_inv_goods_request_article_selResult>();
            MoveCollectionView();
        }
        void documentHeader_XTextBoxKeyDown_BaseBuyRequest(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Tab)
                return;
            stp_inv_buy_request_selResult record = new stp_inv_buy_request_selResult();
            GlobalFunctions.Copy_FK_To_FK(record, selectedRecord);
            CodeTextBox_KeyDown_Filter<stp_inv_buy_request_selResult, MasterRT>(sender, FieldNames<MasterRT>.BuyRequestId, e, selectedRecord, record);
            CopyArticlesFromSourceDocument<stp_inv_buy_request_article_selResult>();
            MoveCollectionView();
        }
        void documentHeader_XTextBoxKeyDown_DestinationDetail(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_acc_detail_selResult>(sender, FieldNames<MasterRT>.DestinationDetailId, e);
        }

        #endregion
    }
}
