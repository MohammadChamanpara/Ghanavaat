using DataAccessLayer;
using UserInterfaceLayer;
using APMTools;
namespace APM_SubSystems
{
    public partial class frm_CostCenter : WindowEntity<stp_glb_cost_center_selResult>
    {
        public frm_CostCenter()
        {
            InitializeComponent();
            Initial_WindowEntity(dbgCostCntr, windowToolbar, grpInputInfo, "glb_cost_center", true, null, (long)EntityType.glb_cost_center, tpc_glb_cost_center_child_code);
        }
       
    }
}
        
