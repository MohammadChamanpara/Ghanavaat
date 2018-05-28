using System.Windows;
using UserInterfaceLayer;
using BusinessLogicLayer;
using DataAccessLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_glb_bank_account : WindowEntityGroup<stp_glb_bank_account_selResult,
                                                                  stp_glb_bank_account_selResult,
                                                                  stp_glb_bank_selResult>
    {  
        #region Initial
        public frm_glb_bank_account()
        {
            InitializeComponent();
            Initial_WindowEntityGroup(grd_bank_account, windowToolbar, grpInfo, "glb_bank_account", true, null, (long)EntityType.glb_bank_account, tpc_glb_bank_account_child_code, cmb_glb_bank_account_glb_bank_id);
        }
       #endregion

        #region Events

        #region XBrowseClick

        private void brw_glb_bank_account_glb_bank_branch_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_Parameter(new WindowSelectGrid<stp_glb_bank_branch_selResult>(), selectedRecord,
                new stp_glb_bank_branch_selResult() { glb_bank_branch_glb_bank_id = selectedRecord.glb_bank_account_glb_bank_id}, 
                "شعبه", typeof(frm_glb_bank_branch), sender);
        }

        private void brw_glb_bank_account_glb_bank_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectGrid<stp_glb_bank_selResult>(), "بانک", typeof(frm_glb_bank), sender);
            GlobalFunctions.Copy_PK_To_FK(selectedRecord, new stp_glb_bank_branch_selResult());
            MoveCollectionView();
        }

        #endregion

        #region XTextBoxKeyDown

        private void brw_glb_bank_account_glb_bank_id_XTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_glb_bank_selResult>(sender, "glb_bank_account_glb_bank_id", e);
        }

        private void brw_glb_bank_account_glb_bank_branch_id_XTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            stp_glb_bank_branch_selResult recordParameter = new stp_glb_bank_branch_selResult();
            recordParameter.glb_bank_branch_glb_bank_id = selectedRecord.glb_bank_account_glb_bank_id;
            CodeTextBox_KeyDown_Filter<stp_glb_bank_branch_selResult, stp_glb_bank_account_selResult>(sender, "glb_bank_account_glb_bank_branch_id", e, selectedRecord, recordParameter);

        }

        #endregion

        #region ComboBox

        private void cmb_glb_bank_account_glb_bank_branch_id_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmb_glb_bank_account_glb_bank_branch_id.SelectedIndex != -1)
            {
                stp_glb_bank_account_selResult record = new stp_glb_bank_account_selResult();
                GlobalFunctions.Copy_PK_To_FK(record, (stp_glb_bank_selResult)cmb_glb_bank_account_glb_bank_id.SelectedItem);
                GlobalFunctions.Copy_PK_To_FK(record, (stp_glb_bank_branch_selResult)cmb_glb_bank_account_glb_bank_branch_id.SelectedItem);
                GlobalFunctions.ListToBindingList(BLL.GetSomeRecords_DB(record), bindingList, collectionView);
            }

        }

        private void cmb_glb_bank_account_glb_bank_id_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            BLL<stp_glb_bank_branch_selResult> bll_bank_branch = new BLL<stp_glb_bank_branch_selResult>();
            stp_glb_bank_branch_selResult record_bank_branch = new stp_glb_bank_branch_selResult();
            GlobalFunctions.Copy_PK_To_FK(record_bank_branch, (stp_glb_bank_selResult)cmb_glb_bank_account_glb_bank_id.SelectedItem);
            bll_bank_branch.FillComboBoxForShow(cmb_glb_bank_account_glb_bank_branch_id, record_bank_branch, "نمایش همه", 0);

        }
        #endregion


        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            BLL<stp_glb_bank_account_type_selResult> bll = new BLL<stp_glb_bank_account_type_selResult>();
            bll.FillComboBox(cmb_glb_bank_account_glb_bank_account_type_id, bindingList);
           
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            if (cmb_glb_bank_account_glb_bank_id.SelectedIndex > 0)
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, (stp_glb_bank_selResult)cmb_glb_bank_account_glb_bank_id.SelectedItem);
            if (cmb_glb_bank_account_glb_bank_branch_id.SelectedIndex > 0)
                GlobalFunctions.Copy_PK_To_FK(selectedRecord, (stp_glb_bank_branch_selResult)cmb_glb_bank_account_glb_bank_branch_id.SelectedItem);
        }
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            cmb_glb_bank_account_glb_bank_branch_id.IsEnabled = enable;
        }
      
        #endregion
    }
}
