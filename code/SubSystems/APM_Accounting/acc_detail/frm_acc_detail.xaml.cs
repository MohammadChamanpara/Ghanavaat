using System.Windows;
using System.Windows.Controls;
using DataAccessLayer;
using UserInterfaceLayer;
using BusinessLogicLayer;
using APMTools;

namespace APM_Accounting
{
    public partial class frm_acc_detail : WindowBase<stp_acc_detail_selResult>
    {
        #region Constructor
        public frm_acc_detail()
        {
            InitializeComponent();
            Initial_WindowBase(dbg_acc_detail, tbr_acc_detail, grp_acc_detail, "acc_detail", true, null);
            tbr_acc_detail.XDeleteButton.Visibility = Visibility.Collapsed;
            tbr_acc_detail.XEditButton.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            new BLL<stp_glb_entity_type_selResult>().FillComboBoxForShow(cmb_acc_detail_glb_entity_type_id,"نمایش همه",0);
        }
        public override void InsertClick()
        {
            long currentEntityType = (long)cmb_acc_detail_glb_entity_type_id.SelectedValue;
            if (currentEntityType == 0)
                if (selectedRecord != null)
                    currentEntityType = selectedRecord.acc_detail_glb_entity_type_id;
            if (currentEntityType == 0)
                return;

            if (currentEntityType == (long)EntityType.glb_personel)
                new APM_SubSystems.frm_Personel().ShowDialog();
            else if (currentEntityType == (long)EntityType.glb_person)
                new APM_SubSystems.frm_Person().ShowDialog();
            else if (currentEntityType == (long)EntityType.glb_company)
                new APM_SubSystems.frm_Company().ShowDialog();
            else if (currentEntityType == (long)EntityType.glb_cash)
                new APM_SubSystems.frm_glb_cash().ShowDialog();
            else if (currentEntityType == (long)EntityType.glb_bank)
                new APM_SubSystems.frm_glb_bank().ShowDialog();
            else if (currentEntityType == (long)EntityType.glb_bank_branch)
                new APM_SubSystems.frm_glb_bank_branch().ShowDialog();
            else if (currentEntityType == (long)EntityType.glb_bank_account)
                new APM_SubSystems.frm_glb_bank_account().ShowDialog();
            else if (currentEntityType == (long)EntityType.glb_cost_center)
                new APM_SubSystems.frm_CostCenter().ShowDialog();
            else if (currentEntityType == (long)EntityType.glb_project)
                new APM_SubSystems.frm_Project().ShowDialog();
            else if (currentEntityType == (long)EntityType.inv_store)
                new APM_SubSystems.frm_inv_store().ShowDialog();
            else if (currentEntityType == (long)EntityType.inv_goods)
                new APM_SubSystems.frm_group_goods().ShowDialog();
            else if (currentEntityType == (long)EntityType.gnt_creditor)
                new APM_SubSystems.frm_gnt_creditor().ShowDialog();
            else
                new frm_glb_user_entity((stp_glb_entity_type_selResult)cmb_acc_detail_glb_entity_type_id.SelectedItem).ShowDialog();
            RefreshClick();

        }
        #endregion

        #region Event
        private void cmb_acc_detail_glb_entity_type_id_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalFunctions.Copy_PK_To_FK(RecordParameter, (stp_glb_entity_type_selResult)cmb_acc_detail_glb_entity_type_id.SelectedItem);
            ShowSomeRecords(RecordParameter);
        }
        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new frm_acc_rpt_account_balance().CustomReport(
                new stp_acc_rpt_account_balance_selResult()
                {
                    acc_rpt_account_balance_acc_detail_id = selectedRecord.acc_detail_id,
                    acc_rpt_account_balance_acc_detail_name = selectedRecord.acc_detail_name,
                    acc_rpt_account_balance_acc_detail_code = selectedRecord.acc_detail_code
                });
        }
        #endregion
    }
}
