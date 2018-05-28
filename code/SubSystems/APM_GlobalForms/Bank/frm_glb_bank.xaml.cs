using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UserInterfaceLayer;
using DataAccessLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_glb_bank : WindowEntity<stp_glb_bank_selResult>
    {
        public frm_glb_bank()
        {
            InitializeComponent();
            Initial_WindowEntity(grd_bank, windowToolbar, grpInfo, "glb_bank", true, null, (long)EntityType.glb_bank, tpc_glb_bank_child_code);
        }
    }
}
