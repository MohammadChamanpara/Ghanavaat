using System.Windows;
using System.Windows.Input;
using DataAccessLayer;
using UserInterfaceLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_glb_cash : WindowEntity<stp_glb_cash_selResult>
    {
        public frm_glb_cash()
        {
            InitializeComponent();
            Initial_WindowEntity(dbg_glb_cash, tbr_glb_cash, grp_glb_cash, "glb_cash", true, null, (long)EntityType.glb_cash, tpc_glb_cash_child_code);
        }

        private void APMBrowser_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGridGroup<stp_glb_personel_selResult, stp_glb_personel_group_selResult>(), "پرسنل", typeof(frm_Personel), sender);
        }

        private void APMBrowser_XTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_glb_personel_selResult>(sender, "glb_cash_cashier_glb_personel_id", e);
        }
    }
}
