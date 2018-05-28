using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using APMComponents;
using APMTools;
using DataAccessLayer;
using System.Collections;

namespace UserInterfaceLayer
{
    public class WindowSearch<RT> : WindowBase<RT>
    {
        #region Variables
        private APMToolBar APMToolBar = new APMToolBar() { XType = XWindowType.ReportWindow };
        APMDocumentHeader APMDocumentHeader = new APMDocumentHeader();
        public DocumentTypes? documentType;
       
        #endregion

        #region Constructor
        public WindowSearch() : this(null) { }
        public WindowSearch(DocumentTypes? reportName)
        {
            if(reportName!=null)
                this.documentType = reportName.Value;
            else if (typeof(RT) == typeof(stp_inv_goods_receive_selResult))
                this.documentType = DocumentTypes.ReceiveReport;
            else if (typeof(RT) == typeof(stp_inv_goods_send_selResult))
                this.documentType = DocumentTypes.SendReport;
            else if (typeof(RT) == typeof(stp_acc_document_selResult))
                this.documentType = DocumentTypes.AccountBalanceReport;
            else if (typeof(RT) == typeof(stp_inv_buy_request_selResult))
                this.documentType = DocumentTypes.BuyRequestReport;
            else if (typeof(RT) == typeof(stp_inv_goods_request_selResult))
                this.documentType = DocumentTypes.GoodsRequestReport;
            else if (typeof(RT) == typeof(stp_inv_goods_receive_inv_goods_send_selResult))
                this.documentType = DocumentTypes.SendOrReceiveReport;
            else
            {
                this.documentType = null;
                return;
            }

            APMDocumentHeader.XType = documentType.Value;
            APMDocumentHeader.XBrowseClick_MainStore += new RoutedEventHandler(APMDocumentHeader_XBrowseClick_MainStore);
            APMDocumentHeader.XBrowseClick_DestinationDetail += new RoutedEventHandler(APMDocumentHeader_XBrowseClick_DestinationDetail);
            APMDocumentHeader.XBrowseClick_RegistererUser += new RoutedEventHandler(APMDocumentHeader_XBrowseClick_RegistererUser);
            APMDocumentHeader.XBrowseClick_SelectGoods += new RoutedEventHandler(APMDocumentHeader_XBrowseClick_SelectGoods);
            APMDocumentHeader.XBrowseClick_RequestConfirmerPersonel += new RoutedEventHandler(APMDocumentHeader_XBrowseClick_ConfirmerPersonel);
            APMDocumentHeader.XBrowseClick_GoodsRequesterCostCenter += new RoutedEventHandler(APMDocumentHeader_XBrowseClick_RequesterCostCenter);
            APMDocumentHeader.XBrowseClick_GoodsRequesterPersonel += new RoutedEventHandler(APMDocumentHeader_XBrowseClick_RequesterPersonel);
            DesignTheForm();
            Initial_WindowBase(null, APMToolBar, APMDocumentHeader, null, false, null);
        }
        #endregion

        #region XBrowseClick
        void APMDocumentHeader_XBrowseClick_SelectGoods(object sender, RoutedEventArgs e)
        {

            BrowseClick_Report(sender, new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.MultiSelect_LastNode, "کالا و گروه کالا"), "کالا", null);
        }
        void APMDocumentHeader_XBrowseClick_RegistererUser(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_user_selResult>(), "ثبت کننده", null);
        }
        void APMDocumentHeader_XBrowseClick_DestinationDetail(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(), "طرف سند", null);
        }
        void APMDocumentHeader_XBrowseClick_MainStore(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_inv_store_selResult>(), "انبار", null);
        }
        void APMDocumentHeader_XBrowseClick_ConfirmerPersonel(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_personel_selResult>(), "شخص تائید کننده", null);
        }

        void APMDocumentHeader_XBrowseClick_RequesterCostCenter(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_cost_center_selResult>(), "مرکز هزینه", null);
        }

        void APMDocumentHeader_XBrowseClick_RequesterPersonel(object sender, RoutedEventArgs e)
        {
            BrowseClick_Report(sender, new WindowSelectGrid<stp_glb_personel_selResult>(), "درخواست دهنده", null);
        }
        #endregion

        #region Tools
        public void Initial_WindowSearch()
        {

        }
        private void DesignTheForm()
        {
            APMBorder APMBorder = new APMBorder();
            this.Content = APMBorder;
            APMDockPanel APMDockPanel = new APMDockPanel();
            APMBorder.Child = APMDockPanel;
            APMDockPanel.Children.Add(APMToolBar);
            var str = typeof(RT).Name.Substring(4);
            str = str.Substring(0, (str.Length) - 10);
            APMDocumentHeader.Name = typeof(RT).Name;
            APMDockPanel.Children.Add(APMDocumentHeader);
            this.Content = APMBorder;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }
        
        #endregion

        #region Override
        public override void SearchClick()
        {
            if (APMDocumentHeader != null)
                APMDocumentHeader.CopyDataFromControlsToARecord(selectedRecord);
            this.DialogResult = true;

        }
        public override void SetEnables(bool enable)
        { }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            RT saveSelectedRecord = Activator.CreateInstance<RT>();
            GlobalFunctions.CopyRecord(saveSelectedRecord, selectedRecord);
            bindingList.AddNew();
            try
            {
                collectionView.MoveCurrentToLast();
            }
            catch (NullReferenceException)
            {
                return;
            }
            GlobalFunctions.CopyRecord(selectedRecord, saveSelectedRecord);
            MoveCollectionView();
        }
        #endregion
    }
}
