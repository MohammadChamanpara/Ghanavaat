using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using APMComponents;
using APMTools;
using CrystalDecisions.CrystalReports.Engine;
using DataAccessLayer;
using Microsoft.Windows.Controls;
using System.ComponentModel;
using BusinessLogicLayer;

namespace UserInterfaceLayer
{
    public class WindowReport<RT> : WindowBase<RT>
    {
        #region Variables
        private APMDocumentHeader documentHeader;
        private RT lastParameter;
        private Boolean fiscalYearChanged = true;
        private Boolean firstCall = true;
        private ReportClass reportFile;

        #endregion

        #region SumRecord
        protected class SumRecord
        {
            private static Boolean SumIsShowing = false;
            public static RT sumRecord;
            public static void Calculate(List<RT> allRecords)
            {
                double sum;
                allRecords.Add(Activator.CreateInstance<RT>());
                foreach (PropertyInfo property in typeof(RT).GetProperties())
                {
                    if
                    (
                        property.PropertyType != typeof(double) &&
                        property.PropertyType != typeof(double?) &&
                        property.PropertyType != typeof(int) &&
                        property.PropertyType != typeof(int?) &&
                        property.PropertyType != typeof(long) &&
                        property.PropertyType != typeof(long?)
                    )
                        continue;
                    sum = 0;
                    foreach (RT item in allRecords)
                        sum += System.Convert.ToDouble(property.GetValue(item, null));
                    if
                    (
                        property.PropertyType == typeof(double) ||
                        property.PropertyType == typeof(double?)
                    )
                        property.SetValue(allRecords.Last(), sum, null);
                    else if
                    (
                        property.PropertyType == typeof(long) ||
                        property.PropertyType == typeof(long?)
                    )
                        property.SetValue(allRecords.Last(), (long)Math.Round(sum), null);
                    else
                        property.SetValue(allRecords.Last(), (int)Math.Round(sum), null);

                }
                PropertyInfo sumProperty = null;
                foreach (PropertyInfo property in typeof(RT).GetProperties())
                    if (property.PropertyType == typeof(string))
                    {
                        sumProperty = property;
                        break;
                    }
                if (sumProperty != null)
                    sumProperty.SetValue(allRecords.Last(), "مجموع", null);
                sumRecord = Activator.CreateInstance<RT>();
                sumRecord = allRecords.Last();
                SumIsShowing = true;

            }
            public static void RemoveFromList(List<RT> allRecords)
            {
                if (!SumIsShowing)
                    return;
                sumRecord = allRecords.Last();
                allRecords.RemoveAt(allRecords.Count - 1);
                SumIsShowing = false;
            }
            public static void AddToList(List<RT> allRecords)
            {
                if (SumIsShowing || sumRecord == null)
                    return;
                allRecords.Add(Activator.CreateInstance<RT>());
                SumIsShowing = true;
            }
        }
        #endregion

        #region Initial

        public void Initial_WindowReport(APMDocumentHeader userDocumentHeader, APMDataGridExtended userDataGrid, APMToolBar userToolbar, string Xoperation, ReportClass reportFile)
        {
            userToolbar.XType = XWindowType.ReportWindow;
            this.documentHeader = userDocumentHeader;
            this.reportFile = reportFile;
            userDataGrid.datagrid.Sorting+=dataGrid_Sorting;
            Initial_WindowBase(userDataGrid, userToolbar, null, Xoperation, false, null);
            lastParameter = Activator.CreateInstance<RT>();
            if (documentHeader != null)
            {
                documentHeader.XTextBoxKeyDown_MainStore += new KeyEventHandler(documentHeader_XTextBoxKeyDown_MainStore);
                documentHeader.XTextBoxKeyDown_GoodsRequesterCostCenter += new KeyEventHandler(documentHeader_XTextBoxKeyDown_GoodsRequesterCostCenter);
                documentHeader.XTextBoxKeyDown_GoodsRequesterPersonel += new KeyEventHandler(documentHeader_XTextBoxKeyDown_GoodsRequesterPersonel);
                documentHeader.XTextBoxKeyDown_RequestConfirmerPersonel += new KeyEventHandler(documentHeader_XTextBoxKeyDown_RequestConfirmerPersonel);
                documentHeader.XTextBoxKeyDown_DestinationDetail += new KeyEventHandler(documentHeader_XTextBoxKeyDown_DestinationDetail);
                //new BLL<stp_glb_fiscal_year_selResult>().FillComboBoxForShow(documentHeader.cmbFiscalYear);
            }
        }

        #endregion

        #region Overrides
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            RT saveSelectedRecord = Activator.CreateInstance<RT>();
            GlobalFunctions.CopyRecord(saveSelectedRecord, selectedRecord);
            bindingList.AddNew();
            collectionView.MoveCurrentToLast();
            GlobalFunctions.CopyRecord(selectedRecord, saveSelectedRecord);
            MoveCollectionView();
            WindowState = WindowState.Maximized;
        }
        public override void PrintClick()
        {
            SearchClick();
            if (reportFile == null)
                return;
            var printForm = new WindowPrint<RT, RT>(reportFile);
            var sumRecord = allRecords.Last();
            allRecords.RemoveAt(allRecords.Count - 1);
            var listForPrint = new List<RT>();
            foreach (RT record in dataGrid.Items)
                listForPrint.Add(record);
            printForm.articleList = listForPrint;
            printForm.selectedRecord = this.selectedRecord;
            printForm.ShowDialog();
            allRecords.Add(sumRecord);
        }
        public override void SearchClick()
        {
            if (documentHeader != null)
                documentHeader.CopyDataFromControlsToARecord(selectedRecord);
            if (SameFilterObjects())
                return;
            firstCall = false;
            fiscalYearChanged = false;
            GlobalFunctions.CopyRecord(lastParameter, selectedRecord);
            LoadData();
            SumRecord.Calculate(allRecords);
            dataGrid.ItemsSource = allRecords;
            (dataGrid.Parent as APMDataGridExtended).XSetBinding(bindingList);
            MoveCollectionView();
        }

        public virtual bool SameFilterObjects()
        {
            return GlobalFunctions.ObjectsAreEqual(selectedRecord, lastParameter) && !firstCall && !fiscalYearChanged;
        }

        public virtual void LoadData()
        {
            allRecords = BLL.GetSomeRecords_DB(selectedRecord);
        }
        public override void SetEnables(bool enable)
        {
        }
        public override void SetBinding()
        {
            base.SetBinding();
            if (documentHeader != null)
                SetBinding_Rec(documentHeader, bindingList);
        }
        public override void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SumRecord.AddToList(allRecords);
        }
        #endregion

        #region Tools

        public void CustomReport(RT record)
        {
            GlobalFunctions.CopyRecord(selectedRecord, record);
            MoveCollectionView();
            SearchClick();
            ShowDialogForm(this);
        }
        #endregion

        #region Events
        public void dataGrid_Sorting(object sender, Microsoft.Windows.Controls.DataGridSortingEventArgs e)
        {
            SumRecord.RemoveFromList(allRecords);
        }
        void documentHeader_XTextBoxKeyDown_MainStore(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown_Report<stp_inv_store_selResult, RT>(sender, FieldNames<RT>.StoreId, e, selectedRecord);
        }
        void documentHeader_XTextBoxKeyDown_GoodsRequesterPersonel(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown_Report<stp_glb_personel_selResult, RT>(sender, FieldNames<RT>.RequesterPersonelId, e, selectedRecord);
        }
        void documentHeader_XTextBoxKeyDown_GoodsRequesterCostCenter(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Tab)
                return;
            CodeTextBox_KeyDown_Report<stp_glb_cost_center_selResult, RT>(sender, FieldNames<RT>.CostCenterId, e, selectedRecord);
        }
        void documentHeader_XTextBoxKeyDown_RequestConfirmerPersonel(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown_Report<stp_glb_personel_selResult, RT>(sender, FieldNames<RT>.ConfirmerPersonelId, e, selectedRecord);
        }
        void documentHeader_XTextBoxKeyDown_DestinationDetail(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown_Report<stp_acc_detail_selResult, RT>(sender, FieldNames<RT>.DestinationDetailId, e, selectedRecord);
        }
        void cmbFiscalYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            firstCall = true;
        }
        #endregion
    }
}
