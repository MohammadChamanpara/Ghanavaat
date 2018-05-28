using System.Windows;
using UserInterfaceLayer;
using BusinessLogicLayer;
using DataAccessLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_gnt_water : WindowBase<stp_gnt_water_selResult>
    {
        #region Initial
        public frm_gnt_water()
        {
            InitializeComponent();
            Initial_WindowBase(grd_bank_account, windowToolbar, grpInfo, "gnt_water", true, null);
        }
        #endregion

        #region Events

        #region XBrowseClick

        private void brw_gnt_water_gnt_water2_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_Parameter(new WindowSelectGrid<stp_gnt_water2_selResult>(), selectedRecord,
                new stp_gnt_water2_selResult() { gnt_water2_gnt_water1_id = selectedRecord.gnt_water_gnt_water1_id },
                "جوی", typeof(frm_gnt_water2), sender);
        }

        private void brw_gnt_water_gnt_water1_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_gnt_water1_selResult>(), "آب اصلی", typeof(frm_gnt_water1), sender);
            GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_gnt_water2_selResult());
            MoveCollectionView();
        }

        #endregion

        #region ComboBox

        private void cmb_gnt_water_gnt_water2_id_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmb_gnt_water_gnt_water2_id.SelectedIndex != -1)
            {
                stp_gnt_water_selResult record = new stp_gnt_water_selResult();
                GlobalFunctions.Copy_PK_To_FK(record, (stp_gnt_water1_selResult)cmb_gnt_water_gnt_water1_id.SelectedItem);
                GlobalFunctions.Copy_PK_To_FK(record, (stp_gnt_water2_selResult)cmb_gnt_water_gnt_water2_id.SelectedItem);
                GlobalFunctions.ListToBindingList(BLL.GetSomeRecords_DB(record), bindingList, collectionView);
            }
        }

        private void cmb_gnt_water_gnt_water1_id_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            stp_gnt_water2_selResult water2 = new stp_gnt_water2_selResult();
            GlobalFunctions.Copy_PK_To_FK(water2, (stp_gnt_water1_selResult)cmb_gnt_water_gnt_water1_id.SelectedItem);
            new BLL<stp_gnt_water2_selResult>().FillComboBoxForShow(cmb_gnt_water_gnt_water2_id, water2, "نمایش همه", 0);

            stp_gnt_water_selResult water = new stp_gnt_water_selResult();
            GlobalFunctions.Copy_PK_To_FK(water, (stp_gnt_water1_selResult)cmb_gnt_water_gnt_water1_id.SelectedItem);
            GlobalFunctions.ListToBindingList(BLL.GetSomeRecords_DB(water), bindingList, collectionView);
        }
        #endregion

        #region Menu
        private void btnShowOwnership_Click(object sender, RoutedEventArgs e)
        {
            new frm_gnt_water_ownerships(selectedRecord).ShowDialog();
        }
        #endregion
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<stp_gnt_water1_selResult>().FillComboBoxForShow(cmb_gnt_water_gnt_water1_id, new stp_gnt_water1_selResult(), "نمایش همه", 0);
            new BLL<stp_gnt_water2_selResult>().FillComboBoxForShow(cmb_gnt_water_gnt_water2_id, new stp_gnt_water2_selResult(), "نمایش همه", 0);
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            if (cmb_gnt_water_gnt_water1_id.SelectedIndex > 0)
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, (stp_gnt_water1_selResult)cmb_gnt_water_gnt_water1_id.SelectedItem);
            if (cmb_gnt_water_gnt_water2_id.SelectedIndex > 0)
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, (stp_gnt_water2_selResult)cmb_gnt_water_gnt_water2_id.SelectedItem);
        }
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            cmb_gnt_water_gnt_water2_id.IsEnabled = enable;
            cmb_gnt_water_gnt_water1_id.IsEnabled = enable;
        }

        #endregion


    }
}
