using System;
using System.Windows.Controls;
using APMComponents;
using APMTools;
using BusinessLogicLayer;
using APM_SubSystems;
using DataAccessLayer;
using UserInterfaceLayer;
using System.Windows;
using System.Linq;

#region Description
/// Modify By : Khanian 1388.08.30
/// Description  : همان کار را میکند listForTree را حذف کردم چون به نظر میرسد ChartAccountList
/// Modify By : ChamanPara 1389.05.14
/// Description  : کار خیلی خوبی کردید
#endregion

namespace APM_Accounting
{
    public partial class frm_acc_chart_account : WindowTreeGridHuge<stp_acc_chart_account_selResult, stp_acc_chart_account_treResult>
    {
        #region Variables
        BLL<stp_acc_options_selResult> BLLOption = new BLL<stp_acc_options_selResult>();
        long? saveType, saveNature;
        ArticlePackage<stp_acc_detail_selResult, stp_acc_chart_account_selResult> accountsArticlePackage = new WindowBase<stp_acc_chart_account_selResult>.ArticlePackage<stp_acc_detail_selResult, stp_acc_chart_account_selResult>();
        #endregion

        #region Constructor
        public frm_acc_chart_account()
        {
            InitializeComponent();
            LevelCount = BLLOption.GetOneRecord().acc_options_detail_level_count + 3;
            Code_DigitCount = BLLOption.GetOneRecord().acc_options_chart_digit_count;
            Initial_WindowTreeGrid(dbgChartAccount, windowToolbar, grpChartAccountCurrentRow, "acc_chart_account_for_form", null, trvChartAccount, "کدینگ حسابداری", LevelCount, Code_DigitCount);
        }
        #endregion

        #region Override
        public override void InsertChildClick()
        {
            if (selectedRecord.acc_chart_account_level_no <= 2)
                base.InsertChildClick();
            else
                Create_AccountsFromDetails();
        }
        public override void InitializationBeforeSave()
        {
            base.InitializationBeforeSave();
            if (selectedRecord.acc_chart_account_acc_detail_id != null && selectedRecord.acc_chart_account_acc_detail_id != 0)
                selectedRecord.acc_chart_account_child_code = null;
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            selectedRecord.acc_chart_account_type_glb_coding_name = parentRecord.acc_chart_account_type_glb_coding_name;
            selectedRecord.acc_chart_account_type_glb_coding_id = parentRecord.acc_chart_account_type_glb_coding_id;
            selectedRecord.acc_chart_account_nature_glb_coding_name = parentRecord.acc_chart_account_nature_glb_coding_name;
            selectedRecord.acc_chart_account_nature_glb_coding_id = parentRecord.acc_chart_account_nature_glb_coding_id;
        }
        public override void OperationsAfterEdit()
        {
            base.OperationsAfterEdit();
            saveNature = selectedRecord.acc_chart_account_nature_glb_coding_id;
            saveType = selectedRecord.acc_chart_account_type_glb_coding_id;
        }
        public override void InsertClick()
        {
            if (selectedRecord.acc_chart_account_level_no > 3)
            {
                var saveAllRecords = allRecords;
                SelectTreeNode(tree, parentNode);
                allRecords = saveAllRecords;
                Create_AccountsFromDetails();
            }
            else
                base.InsertClick();
        }
        public override void OperationsAfterSaved( )
        {
            base.OperationsAfterSaved();
            if
            (
                operationType == OperationType.Update &&
                selectedNode.Items.Count != 0 &&
                (
                    selectedRecord.acc_chart_account_type_glb_coding_id != saveType ||
                    selectedRecord.acc_chart_account_nature_glb_coding_id != saveNature
                )
            )
                RefreshClick();
        }
        public override void SetVisibilityOrEnableOfControlsBasedOnSelectedRecord()
        {
            base.SetVisibilityOrEnableOfControlsBasedOnSelectedRecord();
            var levelNo = selectedRecord.acc_chart_account_level_no;

            GlobalFunctions.SetVisibilityForControl(lbl_CodeTitle, levelNo >= 1);
            GlobalFunctions.SetVisibilityForControl(tpcCode, levelNo >= 1);

            GlobalFunctions.SetVisibilityForControl(lbl_TypeTitle, levelNo >= 1);
            GlobalFunctions.SetVisibilityForControl(cmb_acc_chart_account_type_glb_coding_id, levelNo == 1);
            GlobalFunctions.SetVisibilityForControl(lbl_acc_chart_account_type_glb_coding_name, levelNo >= 2);

            GlobalFunctions.SetVisibilityForControl(lbl_NatureTitle, levelNo >= 2);
            GlobalFunctions.SetVisibilityForControl(cmb_acc_chart_account_nature_glb_coding_id, levelNo == 2);
            GlobalFunctions.SetVisibilityForControl(lbl_acc_chart_account_nature_glb_coding_name, levelNo >= 3);

            GlobalFunctions.SetVisibilityForControl(chk_acc_chart_account_active, levelNo >= 4);

            GlobalFunctions.SetVisibilityForControl(lblSpace, levelNo >= 4);
            GlobalFunctions.SetVisibilityForControl(lblEntityTypeTitle, levelNo >= 4);
            GlobalFunctions.SetVisibilityForControl(lbl_acc_chart_account_glb_entity_type_name, levelNo >= 4);

            dbgChartAccount.datagrid.Columns[2].Visibility = (levelNo >= 4) ? Visibility.Visible : Visibility.Collapsed;
            Boolean formIsEnable = operationType != OperationType.Nothing;
            txt_acc_chart_account_name.IsEnabled = formIsEnable && selectedRecord.acc_chart_account_level_no <= 3;
            tpcCode.IsEnabled = formIsEnable && selectedRecord.acc_chart_account_level_no <= 3;
            string level_name="";
            switch(selectedRecord.acc_chart_account_level_no)
            {
                case(1):
                    level_name="گروه";
                    break;
                case(2):
                    level_name="کل";
                    break;
                case(3):
                    level_name="معین";
                    break;
                default:
                    level_name="تفصیل "+(selectedRecord.acc_chart_account_level_no-3).ToString();
                    break;
            }
            grpDataGrid.Header="سطح جاری : "+level_name;
        }
        #endregion

        #region Tools
        private void Create_AccountsFromDetails()
        {
            if (selectedRecord.acc_chart_account_level_no > 3 && selectedRecord.acc_chart_account_have_document == true)
            {
                Messages.ErrorMessage("برای این حساب سند درج شده است قادر به اضافه کردن حساب زیر مجموعه نمی باشید");
                return;
            }
            if (selectedRecord.acc_chart_account_level_no == LevelCount)
            {
                Messages.ErrorMessage("تعداد سطوح تفصیل حداکثر برابر با " + (LevelCount - 3).ToString() + " می باشد");
                return;
            }
            if (!BrowseClick_MultiSelect(
                    new WindowSelectGridGroup<stp_acc_detail_selResult, stp_glb_entity_type_selResult>(),
                    ref accountsArticlePackage, "حساب های زیر مجموعه", typeof(frm_acc_chart_account)))
                return;
            accountsArticlePackage.FillLists();

            foreach (var detail in accountsArticlePackage.AddedList)
            {
                stp_acc_chart_account_selResult newAccount = new stp_acc_chart_account_selResult();
                newAccount.acc_chart_account_acc_detail_id = detail.acc_detail_id;
                newAccount.acc_chart_account_child_code = detail.acc_detail_code;
                newAccount.acc_chart_account_pre_code = selectedRecord.acc_chart_account_code;
                newAccount.acc_chart_account_code = newAccount.acc_chart_account_pre_code + newAccount.acc_chart_account_child_code;
                newAccount.acc_chart_account_level_no = selectedRecord.acc_chart_account_level_no + 1;
                newAccount.acc_chart_account_name = detail.acc_detail_name;
                newAccount.acc_chart_account_nature_glb_coding_id = selectedRecord.acc_chart_account_nature_glb_coding_id;
                newAccount.acc_chart_account_nature_glb_coding_name = selectedRecord.acc_chart_account_nature_glb_coding_name;
                newAccount.acc_chart_account_type_glb_coding_id = selectedRecord.acc_chart_account_type_glb_coding_id;
                newAccount.acc_chart_account_type_glb_coding_name = selectedRecord.acc_chart_account_type_glb_coding_name;
                newAccount.acc_chart_account_parent_id = selectedRecord.acc_chart_account_id;
                newAccount.acc_chart_account_active = true;
                newAccount.acc_chart_account_glb_entity_type_name = detail.acc_detail_glb_entity_type_name;
                newAccount.acc_chart_account_credit = 0;
                newAccount.acc_chart_account_debt = 0;
                newAccount.acc_chart_account_remaining_credit = 0;
                newAccount.acc_chart_account_remaining_debt = 0;
                BLL.SaveRecord(newAccount, OperationType.Insert, false);
                allRecords.Add(newAccount);
            }
            foreach (var detail in accountsArticlePackage.DeletedList)
            {
                var account = FindDeletedAccountFromDetail(detail);
                BLL.DeleteRecord(account, false);
                allRecords.RemoveAll(x => x.acc_chart_account_id == account.acc_chart_account_id);
            }
            UpdateTree_AfterAccountChange();
            accountsArticlePackage.Clear();
        }
        private void UpdateTree_AfterAccountChange()
        {
            selectedNode.Items.Clear();
            foreach (var account in allRecords)
                if (account.acc_chart_account_parent_id == selectedRecord.acc_chart_account_id)
                    AddNodeToTree(selectedNode, account);
            if (selectedNode.Items.Count > 0)
                SelectTreeNode(tree, selectedNode.Items[0] as APMTreeViewItem);
        }
        private stp_acc_chart_account_selResult FindDeletedAccountFromDetail(stp_acc_detail_selResult detail)
        {
            foreach (var node in selectedNode.Items)
            {
                var account = (node as APMTreeViewItem).Tag as stp_acc_chart_account_selResult;
                if (account.acc_chart_account_acc_detail_id == detail.acc_detail_id)
                    return account;
            }
            return Activator.CreateInstance<stp_acc_chart_account_selResult>();
        }
        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecord.acc_chart_account_level_no > 3 && selectedNode.Items.Count == 0)
                new frm_acc_rpt_account_balance().CustomReport(new stp_acc_rpt_account_balance_selResult()
                {
                    acc_rpt_account_balance_acc_chart_account_id = selectedRecord.acc_chart_account_id,
                    acc_rpt_account_balance_acc_chart_account_code = selectedRecord.acc_chart_account_code,
                    acc_rpt_account_balance_acc_chart_account_name = selectedRecord.acc_chart_account_name
                });
            else
                Messages.InformationMessage("گردش حساب برای حساب های سطح آخر نمایش داده می شود");
        }
    }
}