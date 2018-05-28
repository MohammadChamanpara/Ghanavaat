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
    /// <summary>
    /// Interaction logic for frm_bank_account_type.xaml
    /// </summary>
    public partial class frm_glb_bank_account_type : WindowBase<stp_glb_bank_account_type_selResult>
    {
        public frm_glb_bank_account_type()
        {
            InitializeComponent();
            Initial_WindowBase(grd_bank_account_type, windowToolbar, grpInfo, "glb_bank_account_type", true, null);

            
        }
    }
}
