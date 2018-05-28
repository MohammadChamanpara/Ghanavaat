using System.Windows;
using UserInterfaceLayer;
using DataAccessLayer;
using APMTools;
using BusinessLogicLayer;

namespace APM_SubSystems
{
    public partial class frm_gnt_water2 : WindowBase<stp_gnt_water2_selResult>
    {
        #region Initial
        public frm_gnt_water2()
        {
            InitializeComponent();
            Initial_WindowBase(grd_bank_branch, windowToolbar, grpInfo, "gnt_water2", true, null);
        }
        #endregion

        #region Events
        private void APMBrowser_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_gnt_water1_selResult>(), "آب اصلی", typeof(frm_gnt_water1), sender);
        }
        private void cmb_gnt_water2_gnt_water1_id_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmb_gnt_water2_gnt_water1_id.SelectedIndex != -1)
            {
                stp_gnt_water2_selResult record = new stp_gnt_water2_selResult();
                GlobalFunctions.Copy_PK_To_FK(record, (stp_gnt_water1_selResult)cmb_gnt_water2_gnt_water1_id.SelectedItem);
                GlobalFunctions.ListToBindingList(BLL.GetSomeRecords_DB(record), bindingList, collectionView);
            }
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<stp_gnt_water1_selResult>().FillComboBoxForShow(cmb_gnt_water2_gnt_water1_id, new stp_gnt_water1_selResult(), "نمایش همه", 0);
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            if (cmb_gnt_water2_gnt_water1_id.SelectedIndex > 0)
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, (stp_gnt_water1_selResult)cmb_gnt_water2_gnt_water1_id.SelectedItem);
        }
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            cmb_gnt_water2_gnt_water1_id.IsEnabled = enable;
        }
        #endregion
    }
}
