using UserInterfaceLayer;
using DataAccessLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_gnt_service_report_parameters : WindowBase<stp_gnt_settings_selResult>
    {
        public frm_gnt_service_report_parameters(string creditor,string address)
        {
            this.Creditor = creditor;
            this.Address= address;
            InitializeComponent();
            Initial_WindowBase(null, null, null, null, false, null);
        }
        public string Creditor { get; set; }
        public string Address { get; set; }
        public string Sentence
        {
            get
            {
                if (rad_store.IsChecked == true)
                    return txt_store1.Text.Trim() + " " + Creditor + " " + txt_store2.Text.Trim();
                else if (rad_truck.IsChecked == true)
                    return txt_truck1.Text.Trim() + " " + Creditor + " " + txt_truck2.Text.Trim() + " " + txt_address.Text.Trim() + " " + txt_truck3.Text.Trim();
                else
                    return "";
            }
        }
        public override void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            rad_store.IsChecked = true;
            GlobalFunctions.BindEnableToIsCheched(stk_store, rad_store);
            GlobalFunctions.BindEnableToIsCheched(stk_truck, rad_truck);
            lbl_Creditor1.Content = this.Creditor;
            lbl_Creditor2.Content = this.Creditor;
            txt_address.Text = this.Address;
        }

        private void APMToolbarButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void APMToolbarButton_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;

        }
    }
}
