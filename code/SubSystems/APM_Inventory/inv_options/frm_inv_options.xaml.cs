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
using BusinessLogicLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_inv_options : WindowOptions<stp_inv_options_selResult>
    {
        public frm_inv_options()
        {
            InitializeComponent();
            Initial_WindowOptions(tbrMain, grp_inv_options);
        }
    }
}
