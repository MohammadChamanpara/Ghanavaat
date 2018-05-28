using System.Windows;
using APMTools;
using DataAccessLayer;
using UserInterfaceLayer;
using APM_SubSystems.Sahaam.gnt_creditor;

namespace APM_SubSystems
{
    public partial class frm_gnt_rpt_creditor_ownership : WindowReport<stp_gnt_rpt_creditor_credits_selResult>
    {
        #region Initialize
        public frm_gnt_rpt_creditor_ownership()
        {
            InitializeComponent();
            Initial_WindowReport(null, dbg_filter_results, tbr_filter, "gnt_rpt_creditor_credits",new gnt_rpt_creditor_ownership());
            grpHeader.XCanClear = true;
        }
        #endregion

        private void brw_gnt_rpt_creditor_credits_gnt_creditor_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), "سهامدار", typeof(frm_gnt_creditor),sender);
            brw_gnt_rpt_creditor_credits_gnt_creditor_id.XLabel.Visibility = Visibility.Visible;
        }
        public override void SetBinding()
        {
            base.SetBinding();
            SetBinding_Rec(grpHeader, bindingList);
        }
    }
}
