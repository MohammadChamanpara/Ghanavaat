using BusinessLogicLayer;
using APMTools;
using DataAccessLayer;
using System.Windows;
using UserInterfaceLayer;
using System;


namespace APM_SubSystems
{

    public partial class frm_inv_goods_glb_measure : WindowBase<stp_inv_goods_glb_measure_selResult>
    {
        #region Variables
        stp_inv_goods_glb_measure_selResult selectedMeasure = new stp_inv_goods_glb_measure_selResult();
        stp_inv_group_goods_for_select_selResult MeasureGoods;
        #endregion

        #region Constructor
        public frm_inv_goods_glb_measure()
        {
            InitializeComponent();
            Initial_WindowBase(grd_inv_goods_glb_measure, tbr_main, grp_current_row, "inv_goods_glb_measure", false, null);
        }
        #endregion

        #region Event
        private void brw_inv_goods_glb_measure_XBrowseClick(object sender, RoutedEventArgs e)
        {
            MeasureGoods = BrowseClick(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_LastEntity, "کالا و گروه کالا"), "کالا", typeof(frm_group_goods), sender);
            selectedMeasure.inv_goods_glb_measure_inv_group_goods_id = MeasureGoods.inv_group_goods_for_select_id;
            selectedMeasure.inv_goods_glb_measure_inv_group_goods_name = MeasureGoods.inv_group_goods_for_select_name;
            selectedMeasure.inv_goods_glb_measure_inv_group_goods_code = MeasureGoods.inv_group_goods_for_select_code;
            selectedMeasure.inv_goods_glb_measure_basic_glb_measure_id = MeasureGoods.inv_group_goods_for_select_glb_measure_id;
            selectedMeasure.inv_goods_glb_measure_basic_glb_measure_name = MeasureGoods.inv_group_goods_for_select_glb_measure_name;
            selectedMeasure.inv_goods_glb_measure_Second_glb_measure_name = MeasureGoods.inv_group_goods_for_select_glb_measure_name;
            ShowGoodsMeasure(selectedMeasure);
        }

        private void brw_inv_goods_glb_measure_glb_measure_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            var measure = BrowseClick(new WindowSelectTree<stp_glb_measure_selResult>(TreeType.SingleSelect_All, "واحد اندازه گیری"), "", typeof(APM_SubSystems.frm_glb_Measure), sender);
            selectedRecord.inv_goods_glb_measure_Main_glb_measure_name = measure.glb_measure_name;
            grd_inv_goods_glb_measure.datagrid.Items.Refresh();
        }

        private void brv_inv_goods_glb_measure_glb_measure_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            var measure = BrowseClick(new WindowSelectTree<stp_glb_measure_selResult>(TreeType.SingleSelect_All, "واحد اندازه گیری"), "", typeof(APM_SubSystems.frm_glb_Measure), sender);
            selectedRecord.inv_goods_glb_measure_Main_glb_measure_name = measure.glb_measure_name;
            grd_inv_goods_glb_measure.datagrid.Items.Refresh();
        }
        #endregion

        #region Method
        public void ShowGoodsMeasure(stp_inv_goods_glb_measure_selResult GoodsMeasure)
        {
            selectedMeasure = GoodsMeasure;
            brw_inv_goods_glb_measure.XTextBox.Text = selectedMeasure.inv_goods_glb_measure_inv_group_goods_code;
            brw_inv_goods_glb_measure.XLabel.Content = selectedMeasure.inv_goods_glb_measure_inv_group_goods_name;
            ShowSomeRecords(new stp_inv_goods_glb_measure_selResult() { inv_goods_glb_measure_inv_group_goods_id = selectedMeasure.inv_goods_glb_measure_inv_group_goods_id });
        }
        //جهت محاسبه فرمول هر کالا 
        private void AccountMeasureFormula(string strValue, bool IsFormula)
        {
            if (strValue == string.Empty || strValue == "0")
            {
                selectedRecord.inv_goods_glb_measure_formula = (double)1;
                selectedRecord.inv_goods_glb_measure_formula_d = (double)1;
                return;
            }
            var result = (1 / Convert.ToDouble(strValue));
            if (IsFormula)
                selectedRecord.inv_goods_glb_measure_formula = result;
            else
                selectedRecord.inv_goods_glb_measure_formula_d = result;
            if (selectedRecord.inv_goods_glb_measure_formula >= 1)
            {
                selectedRecord.inv_goods_glb_measure_formula_for_show = selectedRecord.inv_goods_glb_measure_formula;
                selectedRecord.inv_goods_glb_measure_Main_glb_measure_name = selectedRecord.inv_goods_glb_measure_glb_measure_name;
                selectedRecord.inv_goods_glb_measure_Second_glb_measure_name = selectedRecord.inv_goods_glb_measure_basic_glb_measure_name;
            }
            else
            {
                selectedRecord.inv_goods_glb_measure_formula_for_show = selectedRecord.inv_goods_glb_measure_formula_d;
                selectedRecord.inv_goods_glb_measure_Main_glb_measure_name = selectedRecord.inv_goods_glb_measure_basic_glb_measure_name;
                selectedRecord.inv_goods_glb_measure_Second_glb_measure_name = selectedRecord.inv_goods_glb_measure_glb_measure_name;
            }
            MoveCollectionView();
            grd_inv_goods_glb_measure.datagrid.Items.Refresh();
            
        }
        private void txt_inv_goods_glb_measure_formula_LostFocus(object sender, RoutedEventArgs e)
        {
            AccountMeasureFormula(txt_inv_goods_glb_measure_formula.Text, false);
        }
        private void txt_inv_goods_glb_measure_formula_d_LostFocus(object sender, RoutedEventArgs e)
        {
            AccountMeasureFormula(txt_inv_goods_glb_measure_formula_d.Text, true);
        }
        #endregion

        #region Override
        public override bool ValidationForInsert()
        {
            if (selectedMeasure.inv_goods_glb_measure_inv_group_goods_id == 0)
            {
                Messages.ErrorMessage("لطفاً کالا را انتخاب نمایید");
                return false;
            }
            return base.ValidationForInsert();
        }
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            brw_inv_goods_glb_measure.IsEnabled = enable;
            GlobalFunctions.BindEnableToIsCheched(border_inv_goods_glb_measure_one, rdb_inv_goods_glb_measure_one);
            GlobalFunctions.BindEnableToIsCheched(border_inv_goods_glb_measure_two, rdb_inv_goods_glb_measure_two);
            rdb_inv_goods_glb_measure_one.IsChecked = true;
            rdb_inv_goods_glb_measure_two.IsChecked = false;
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            selectedMeasure.inv_goods_glb_measure_Main_glb_measure_name = "";
            GlobalFunctions.CopyRecord(selectedRecord, selectedMeasure);
            selectedRecord.inv_goods_glb_measure_az = "از";
            selectedRecord.inv_goods_glb_measure_equals = "برابر است با";
            selectedRecord.inv_goods_glb_measure_har = "هر";
        }
        public override bool ValidationForSave()
        {
            if (rdb_inv_goods_glb_measure_one.IsChecked == true)
                AccountMeasureFormula(txt_inv_goods_glb_measure_formula.Text, false);
            else
                AccountMeasureFormula(txt_inv_goods_glb_measure_formula_d.Text, true);
            return base.ValidationForSave();
        }
        #endregion
    }
}
