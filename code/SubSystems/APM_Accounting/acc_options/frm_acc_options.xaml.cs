using UserInterfaceLayer;
using DataAccessLayer;

namespace APM_Accounting
{
    public partial class frm_acc_options : WindowOptions<stp_acc_options_selResult>
    {
        public frm_acc_options()
        {
            InitializeComponent();
            Initial_WindowOptions(tbrMain, grp_acc_options);
        }
    }
}
