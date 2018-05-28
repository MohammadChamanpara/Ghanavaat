using BusinessLogicLayer;
using APMTools;
using DataAccessLayer;
using System.Windows;
using UserInterfaceLayer;
using System;

namespace APM_SubSystems
{
    public partial class frm_inv_product : WindowBase<stp_inv_product_part_selResult>
    {
        #region Variables
        stp_inv_product_part_selResult selectedProduct = new stp_inv_product_part_selResult();
        #endregion

        #region Constructor
        public frm_inv_product()
        {
            InitializeComponent();
            Initial_WindowBase(grd_product_parts, tbr_main, grp_current_row, "inv_product_part", false, null);
        }
        #endregion

        #region Events
        private void brw_part_goods_id_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_Entity, "کالا و گروه کالا"), "کالا", typeof(frm_group_goods), sender);
            new BLL<stp_glb_measure_selResult>().FillComboBox(cmb_inv_product_part_part_glb_measure_id, bindingList,
                new stp_glb_measure_selResult() { glb_measure_inv_group_goods_id = selectedRecord.inv_product_part_part_inv_group_goods_id });
        }
        private void brw_product_XBrowseClick(object sender, RoutedEventArgs e)
        {
            var productGoods = BrowseClick(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.SingleSelect_LastEntity, "کالا و گروه کالا"), "محصول", typeof(frm_group_goods), sender);
            selectedProduct.inv_product_part_product_inv_group_goods_id = productGoods.inv_group_goods_for_select_id;
            selectedProduct.inv_product_part_product_inv_group_goods_code = productGoods.inv_group_goods_for_select_code;
            selectedProduct.inv_product_part_product_inv_group_goods_name = productGoods.inv_group_goods_for_select_name;
            ShowProductParts(selectedProduct);
        }
        private void brw_part_XTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_inv_group_goods_selResult>(sender, "inv_product_part_part_inv_group_goods_id", e);
        }
        #endregion

        #region Override
        public override void SetEnables(bool enable)
        {
            base.SetEnables(enable);
            brw_product.IsEnabled = enable;
            cmb_inv_product_part_part_glb_measure_id.Visibility = GlobalFunctions.BooleanToVisibility(operationType != OperationType.Nothing);
            lbl_inv_product_part_part_glb_measure_name.Visibility = GlobalFunctions.BooleanToVisibility(operationType == OperationType.Nothing);
            if (operationType == OperationType.Update)
            {
                var saveValue = cmb_inv_product_part_part_glb_measure_id.SelectedValue;
                new BLL<stp_glb_measure_selResult>().FillComboBox(cmb_inv_product_part_part_glb_measure_id, bindingList,
                    new stp_glb_measure_selResult() { glb_measure_inv_group_goods_id = selectedRecord.inv_product_part_part_inv_group_goods_id }
                    );
                cmb_inv_product_part_part_glb_measure_id.SelectedValue = saveValue;
            }
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            GlobalFunctions.CopyRecord(selectedRecord, selectedProduct);
            if (allRecords.Count > 0)
            {
                selectedRecord.inv_product_part_product_glb_measure_id = allRecords[allRecords.Count - 1].inv_product_part_product_glb_measure_id;
                selectedRecord.inv_product_part_product_glb_measure_name = allRecords[allRecords.Count - 1].inv_product_part_product_glb_measure_name;
            }
            selectedRecord.har = "هر";
            selectedRecord.shamel = "شامل";
            selectedRecord.amount = "به مقدار";
            selectedRecord.ast = "است";
        }
        public override bool ValidationForInsert()
        {
            if (selectedProduct.inv_product_part_product_inv_group_goods_id == 0)
            {
                Messages.ErrorMessage("لطفاً محصول را انتخاب نمایید");
                return false;
            }
            return base.ValidationForInsert();
        }
        #endregion
        
        #region Tools
        public void ShowProductParts(stp_inv_product_part_selResult product)
        {
            selectedProduct = product;
            brw_product.XTextBox.Text = selectedProduct.inv_product_part_product_inv_group_goods_code;
            brw_product.XLabel.Content = selectedProduct.inv_product_part_product_inv_group_goods_name;
            ShowSomeRecords(new stp_inv_product_part_selResult() { inv_product_part_product_inv_group_goods_id = selectedProduct.inv_product_part_product_inv_group_goods_id });
            new BLL<stp_glb_measure_selResult>().FillComboBox
                (cmb_inv_product_part_product_glb_measure_id, bindingList,
                new stp_glb_measure_selResult() { glb_measure_inv_group_goods_id = selectedProduct.inv_product_part_product_inv_group_goods_id }/*,
                "inv_product_part_product_glb_measure_id"*/);
        } 
        #endregion
    }
}
