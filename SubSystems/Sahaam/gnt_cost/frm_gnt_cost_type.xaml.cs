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
using APM_SubSystems;
using BusinessLogicLayer;

namespace APM_SubSystems
{
    public partial class frm_gnt_cost_type : WindowBase<stp_gnt_cost_type_selResult>
    {
        stp_glb_fiscal_year_selResult fromFiscalYear = new stp_glb_fiscal_year_selResult();
        stp_glb_fiscal_year_selResult toFiscalYear = new stp_glb_fiscal_year_selResult();
        public frm_gnt_cost_type()
        {
            InitializeComponent();
            Initial_WindowBase(grd_bank, windowToolbar, grpInfo, "gnt_cost_type", true, null);
        }

        private void brw_gnt_cost_type_glb_fiscal_year_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_glb_fiscal_year_selResult>(), "دوره مالی", typeof(frm_glb_fiscal_year), sender);
        }
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            cmb_gnt_cost_type_glb_fiscal_year.IsEnabled = enable;
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<stp_glb_fiscal_year_selResult>().FillComboBoxForShow(cmb_gnt_cost_type_glb_fiscal_year, new stp_glb_fiscal_year_selResult(), "نمایش همه", 0);
        }
        private void cmb_gnt_cost_type_glb_fiscal_year_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_gnt_cost_type_glb_fiscal_year.SelectedIndex != -1)
            {
                stp_gnt_cost_type_selResult recordParameter = new stp_gnt_cost_type_selResult();
                GlobalFunctions.Copy_PK_To_FK(recordParameter, (stp_glb_fiscal_year_selResult)cmb_gnt_cost_type_glb_fiscal_year.SelectedItem);
                ShowSomeRecords(recordParameter);
            }
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            if (cmb_gnt_cost_type_glb_fiscal_year.SelectedIndex > 0)
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, (stp_glb_fiscal_year_selResult)cmb_gnt_cost_type_glb_fiscal_year.SelectedItem);
        }
    }
}
