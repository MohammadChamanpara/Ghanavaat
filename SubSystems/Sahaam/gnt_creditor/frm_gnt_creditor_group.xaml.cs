using System.Windows.Controls;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
namespace APM_SubSystems
{
    public partial class frm_gnt_creditor_group : WindowBase<stp_gnt_creditor_group_selResult>
    {
        #region Variables
        ArticlePackage<stp_gnt_creditor_selResult, stp_gnt_creditor_in_group_selResult> articlePackage = new WindowBase<stp_gnt_creditor_group_selResult>.ArticlePackage<stp_gnt_creditor_selResult, stp_gnt_creditor_in_group_selResult>();
        #endregion

        #region Constructor
        public frm_gnt_creditor_group()
        {
            InitializeComponent();
            Initial_WindowBase(dbgCreditorGroup, windowToolbar, grpInfo, "gnt_creditor_Group", true, null);
        }
        #endregion

        #region Browse_Click
        private void Browser_select_creditors_click(object sender, System.Windows.RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGridHugeData<stp_gnt_creditor_selResult>(), ref articlePackage, "اشخاص", typeof(frm_gnt_creditor));
        }
        #endregion

        #region Overrid
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            articlePackage.Save(selectedRecord);
        }
        #endregion
    }
}
