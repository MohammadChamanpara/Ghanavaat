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
    public partial class frm_gnt_water1 : WindowBase<stp_gnt_water1_selResult>
    {
        public frm_gnt_water1()
        {
            InitializeComponent();
            Initial_WindowBase(grd_bank, windowToolbar, grpInfo, "gnt_water1", true, null);
        }
    }
}
