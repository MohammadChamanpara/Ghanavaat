using DataAccessLayer;
using UserInterfaceLayer;

namespace APM_Accounting
{
    public partial class frm_acc_document_type : WindowBase<stp_acc_document_type_selResult>
    {
        public frm_acc_document_type()
        {
            InitializeComponent();
            Initial_WindowBase(dbg_acc_document_type, tbr_acc_document_type, grp_acc_document_type, "acc_document_type", true, null);
        }
    }
}
