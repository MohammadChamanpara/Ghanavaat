using DataAccessLayer;
using UserInterfaceLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_Project : WindowEntity<stp_glb_project_selResult>
    { 
        public frm_Project()
        {
            InitializeComponent();
            Initial_WindowEntity(dbgProject, windowToolbar, grpInputInfo, "glb_project", true, null, (long)EntityType.glb_project, tpc_glb_project_code);
        }
       
    }
}