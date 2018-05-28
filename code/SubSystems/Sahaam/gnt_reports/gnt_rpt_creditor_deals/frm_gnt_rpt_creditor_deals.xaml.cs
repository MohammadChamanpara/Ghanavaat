using System.Windows;
using APMTools;
using DataAccessLayer;
using UserInterfaceLayer;
using APM_SubSystems.Sahaam.gnt_reports.gnt_rpt_creditor_deals;

namespace APM_SubSystems
{
    public partial class frm_gnt_rpt_creditor_deals : WindowReport<stp_gnt_rpt_creditor_deals_selResult>
    {

        #region Initialize
        public frm_gnt_rpt_creditor_deals()
        {
            InitializeComponent();
            Initial_WindowReport(null, dbg_filter_results, tbr_filter, "gnt_rpt_creditor_deals",new gnt_rpt_creditor_deals());
            grpHeader.XCanClear = true;

        }
        #endregion

        #region Events

        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var currentRecord = dataGrid.CurrentItem as stp_gnt_rpt_creditor_deals_selResult;
            new frm_gnt_deal().ShowOneDocument(currentRecord.gnt_rpt_creditor_deals_gnt_deal_id, currentRecord.gnt_rpt_creditor_deals_id);
        }

        #endregion

        private void brw_gnt_rpt_creditor_deals_gnt_creditor_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), "سهامدار", typeof(frm_gnt_creditor), sender);
            brw_gnt_rpt_creditor_deals_gnt_creditor_id.XLabel.Visibility = Visibility.Visible;
        }
        public override void SearchClick()
        {
            base.SearchClick();
            selectedRecord = selectedRecord;
        }
        public override void SetBinding()
        {
            base.SetBinding();
            SetBinding_Rec(grpHeader, bindingList);
        }
    }
}
