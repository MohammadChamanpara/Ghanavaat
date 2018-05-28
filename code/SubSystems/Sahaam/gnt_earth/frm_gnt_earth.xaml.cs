using System.Windows;
using UserInterfaceLayer;
using DataAccessLayer;
using APM_SubSystems.Sahaam.gnt_earth;
using System.Collections.Generic;
using System;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_gnt_earth : WindowBase<stp_gnt_earth_selResult>
    {
        long filter_creditor_id;
        public frm_gnt_earth()
        {
            InitializeComponent();
            Initial_WindowBase(grd_bank, windowToolbar, grpInfo, "gnt_earth", true, null);
            grp_search.XCanCollapse = true;
        }

        private void brw_gnt_earth_gnt_creditor_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), "مالک", typeof(frm_gnt_creditor), sender);
        }

        //private void btnSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    SearchClick();
        //}
        public override void SearchClick()
        {
            var earth = new stp_gnt_earth_selResult();

            int value;

            if (int.TryParse(txt_gnt_earth_street_search.Text, out value))
                earth.gnt_earth_street = value;

            if (int.TryParse(txt_gnt_earth_line_search.Text, out value))
                earth.gnt_earth_line = value;

            if (int.TryParse(txt_gnt_earth_block_search.Text, out value))
                earth.gnt_earth_block = value;

            if (int.TryParse(txt_gnt_earth_plaque_search.Text, out value))
                earth.gnt_earth_plaque = value;
            earth.gnt_earth_gnt_creditor_id = filter_creditor_id;
            ShowSomeRecords(earth);
        }

        private void txt_search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //if (chkSearch.IsChecked == false)
            //    return;
            SearchClick();
        }
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            grp_search.IsEnabled = enable;
        }

        private void brw_filter_creditor_XBrowseClick(object sender, RoutedEventArgs e)
        {
            var creditor = BrowseClick(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), "مالک زمین", typeof(frm_gnt_creditor), sender);
            filter_creditor_id = creditor.gnt_creditor_id;
            brw_filter_creditor.XLabel.Content = creditor.gnt_creditor_name;
            //if (chkSearch.IsChecked == false)
            //    return;
            SearchClick();
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            MoveCollectionView();
        }
        private void btnPrintStockSheet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printForm = new WindowPrint<stp_gnt_earth_selResult, stp_gnt_earth_selResult>(new gnt_rpt_creditor_stock_back());
                printForm.articleList = new List<stp_gnt_earth_selResult>();
                printForm.articleList.Add(selectedRecord);
                printForm.ShowDialog();
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.Message);
            }
        }

        private void grp_search_OnClearfilter()
        {
            filter_creditor_id = 0;
            SearchClick();
        }
    }
}
