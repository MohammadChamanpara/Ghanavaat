using UserInterfaceLayer;
using DataAccessLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_gnt_settings : WindowOptions<stp_gnt_settings_selResult>
    {
        public frm_gnt_settings()
        {
            InitializeComponent();
            Initial_WindowOptions(tbrMain, grp_gnt_settings);
        }
        public override void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SahaamEntities dataBase = DDB.NewContext();
            base.Window_Loaded(sender, e);
            if (allRecords.Count == 0)
            {
                dataBase.tbl_gnt_settings.AddObject(
                    new tbl_gnt_settings()
                    {
                        gnt_settings_credit_meters = 1000,
                        gnt_settings_total_credit_count = 1,
                        gnt_settings_total_credit_price = 1,
                        gnt_settings_accountant_name = "-",
                        gnt_settings_chairman_name = "-",
                        gnt_settings_executive_manager_name = "-"
                    });
                dataBase.SaveChanges();
                APMTools.Messages.InformationMessage("تنظیمات به طور خودکار اضافه شد");
                RefreshClick();

            }
            CalculateOneCreditPrice();
        }

        public override void RefreshClick()
        {
            base.RefreshClick();
            CalculateOneCreditPrice();
        }
        private void CalculateOneCreditPrice()
        {
            if (selectedRecord.gnt_settings_total_credit_count == 0)
                return;

            selectedRecord.gnt_settings_one_credit_price =
                selectedRecord.gnt_settings_total_credit_price /
                selectedRecord.gnt_settings_total_credit_count;
            MoveCollectionView();
        }

        private void txt_gnt_settings_price_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            CalculateOneCreditPrice();
        }
    }
}
