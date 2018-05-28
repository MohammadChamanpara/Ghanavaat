using System.Windows;
using System.Windows.Input;
using DataAccessLayer;
using BusinessLogicLayer;
using UserInterfaceLayer;
using APMTools;
using APM_SubSystems;
using APMComponents;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace APM_SubSystems
{
    public partial class frm_group_goods : WindowTreeGridTwoTables<stp_inv_group_goods_selResult>
    {
        #region Variabel
        bool GroupChanged = false;
        BLL<stp_inv_options_selResult> BLLOption = new BLL<stp_inv_options_selResult>();
        ArticlePackage<stp_glb_measure_grdResult, stp_inv_goods_glb_measure_selResult> measure = new WindowBase<stp_inv_group_goods_selResult>.ArticlePackage<stp_glb_measure_grdResult, stp_inv_goods_glb_measure_selResult>();
        ArticlePackage<stp_inv_group_goods_for_select_selResult, stp_inv_goods_similar_selResult> similar_goods = new ArticlePackage<stp_inv_group_goods_for_select_selResult, stp_inv_goods_similar_selResult>();
        ArticlePackage<stp_inv_group_goods_for_select_selResult, stp_inv_goods_parts_selResult> parts_goods = new ArticlePackage<stp_inv_group_goods_for_select_selResult, stp_inv_goods_parts_selResult>();
        ArticlePackage<stp_inv_attribute_selResult, stp_inv_goods_attribute_selResult> attribute = new ArticlePackage<stp_inv_attribute_selResult, stp_inv_goods_attribute_selResult>();
        ArticlePackage<stp_inv_attribute_selResult, stp_inv_group_attribute_selResult> attributeGroup = new ArticlePackage<stp_inv_attribute_selResult, stp_inv_group_attribute_selResult>();
        ArticlePackage<stp_inv_store_selResult, stp_inv_goods_store_selResult> store = new ArticlePackage<stp_inv_store_selResult, stp_inv_goods_store_selResult>();
        #endregion

        #region Constructor
        public frm_group_goods()
        {
            InitializeComponent();
            var groupLevelCount = BLLOption.GetOneRecord().inv_options_group_level_count;
            var groupDigitCount = BLLOption.GetOneRecord().inv_options_group_digit_count;
            tpcGroup.XMaxLength = groupDigitCount;
            Initial_WindowTreeGridTwoTable(dbg_group, tbrMain, null, grpGroupGoods, trv_inv_group, "گروههای کالا", groupLevelCount, groupDigitCount, grd_info_group, grd_info_good, "inv_groups", "inv_goods", "گروه ", "کالا");
            tpc_Goods.XEditable = glbEntityTypeOption_For_Goods.glb_entity_type_option_code_edit_by_user;
            tpc_Goods.XMaxLength = glbEntityTypeOption_For_Goods.glb_entity_type_option_digit_count;
        }
        #endregion

        #region Override
        public override void OperationsAfterSaved()
        {
            selectedRecord.inv_group_goods_name = selectedRecord.inv_group_goods_real_name;
            base.OperationsAfterSaved();
            measure.Save(selectedRecord);
            similar_goods.Save(selectedRecord);
            parts_goods.Save(selectedRecord);
            attribute.Save(selectedRecord);
            attributeGroup.Save(selectedRecord);
            store.Save(selectedRecord);

            if (GroupChanged)
            {
                MakeTree();
                if (tree.Items != null && tree.Items.Count > 0)
                    SelectTreeNode(tree, tree.Items[0] as APMTreeViewItem);
                GroupChanged = false;
            }
        }
        public override void InitializationBeforeSave()
        {
            if (selectedRecord.inv_group_goods_is_group == true)
            {
                base.InitializationBeforeSave();
                return;
            }
            int digitCount = (status == Status.Entity) ? glbEntityTypeOption_For_Goods.glb_entity_type_option_digit_count : Code_DigitCount;
            GlobalFunctions.PutZeroBeforeCode(selectedRecord, digitCount);
            if (GroupChanged)
            {
                List<stp_inv_group_goods_selResult> list = new List<stp_inv_group_goods_selResult>();
                list = (from records in allRecords
                        where records.inv_group_goods_parent_id == selectedRecord.inv_group_goods_parent_id
                        select records).ToList();

                selectedRecord.inv_group_goods_child_code = GlobalFunctions.CreateNewCode(list, BLLOption.GetOneRecord().inv_options_group_digit_count);
            }
        }
        public override void OperationsAfterInsert()
        {
            if (bindingList.Count > 1)
            {
                int i = bindingList.Count - 2;
                if ((bool)(!bindingList.ElementAt(i).inv_group_goods_is_group))
                {
                    selectedRecord.inv_group_goods_glb_measure_id = bindingList.ElementAt(i).inv_group_goods_glb_measure_id;
                    selectedRecord.inv_group_goods_glb_measure_name = bindingList.ElementAt(i).inv_group_goods_glb_measure_name;
                }
            }
            base.OperationsAfterInsert();
        }
        public override void SetBinding()
        {
            base.SetBinding();
            GlobalFunctions.BindVisibilityToIsChecked(stkShowParts, chk_inv_group_goods_is_product);
        }
        #endregion

        #region Browse_Click
        private void btn_Browse_main_glb_measure_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick(new WindowSelectTree<stp_glb_measure_selResult>(TreeType.SingleSelect_All, "واحدهای شمارش"), "واحد شمارش اصلی", typeof(frm_glb_Measure), sender);
        }
        private void btn_Browse_store_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_Parameter(new WindowSelectGrid<stp_inv_goods_store_selResult>(), selectedRecord,
                new stp_inv_goods_store_selResult() { inv_goods_store_inv_group_goods_id = selectedRecord.inv_group_goods_id },
                "انبار", typeof(frm_inv_store), sender);
        }
        private void btn_Browse_group_attribute_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_inv_attribute_selResult>(), ref attributeGroup, "خصوصیت", typeof(frm_inv_attribute), FieldNames<stp_inv_attribute_selResult>.IsGroup, true);
        }
        private void btn_browse_parts_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.MultiSelect_LastNode, "کالا و گروه کالا"), ref parts_goods, "اجزای تشکیل دهنده", typeof(frm_group_goods), "inv_group_goods_for_select_call_for_parts", true);
        }
        private void btn_browse_goods_attribute_Click(object sender, RoutedEventArgs e)
        {
            if (lbl_inv_group_goods_parent_name.Content == null || lbl_inv_group_goods_parent_name.Content.ToString() == "")
                Messages.ErrorMessage("برای کالا گروهی اختصاص داده نشده است");
            else
                BrowseClick_MultiSelect(new WindowSelectGrid<stp_inv_attribute_selResult>(), ref attribute, "خصوصیت", typeof(frm_inv_attribute), FieldNames<stp_inv_attribute_selResult>.IsGroup, false);
        }
        private void btn_browse_similar_Click(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectTree<stp_inv_group_goods_for_select_selResult>(TreeType.MultiSelect_LastNode, "کالا و گروه کالا"), ref similar_goods, "کالاهای مشابه", typeof(frm_group_goods), "inv_group_goods_for_select_call_for_similarity", true);
        }
        private void btn_browse_change_group_Click(object sender, RoutedEventArgs e)
        {
            GroupChanged = true;
            BrowseClick(new WindowSelectTree<stp_inv_group_goods_treResult>(TreeType.SingleSelect_LastChild, "گروه کالا"), "انتخاب گروه کالا", typeof(frm_group_goods), sender);
        }
        #endregion

        #region Events
        private void btnAddGroup_click(object sender, RoutedEventArgs e)
        {
            InsertClick();
        }
        private void txt_inv_group_code_KeyDown(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_inv_group_goods_treResult>(tpc_Goods, "inv_group_goods_parent_id", e);
        }
        private void btnShowParts_Click(object sender, RoutedEventArgs e)
        {
            var frmProduct = new frm_inv_product();
            var product = new stp_inv_product_part_selResult();
            product.inv_product_part_product_inv_group_goods_id = selectedRecord.inv_group_goods_id;
            product.inv_product_part_product_inv_group_goods_code = selectedRecord.inv_group_goods_code;
            product.inv_product_part_product_inv_group_goods_name = selectedRecord.inv_group_goods_name;
            frmProduct.ShowProductParts(product);
            frmProduct.ShowDialog();
        }
        private void APMMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecord.inv_group_goods_is_group == false)
            {
                new frm_inv_rpt_goods_cardex().CustomReport(
                    new stp_inv_rpt_goods_cardex_selResult()
                    {
                        inv_rpt_goods_cardex_inv_group_goods_id = selectedRecord.inv_group_goods_id,
                        inv_rpt_goods_cardex_inv_group_goods_name = selectedRecord.inv_group_goods_name,
                        inv_rpt_goods_cardex_inv_group_goods_code = selectedRecord.inv_group_goods_code
                    });
            }
            else
                Messages.InformationMessage("کاردکس برای کالا ها قابل استفاده می باشد.");
        }
        private void btn_browse_glb_measureClick(object sender, RoutedEventArgs e)
        {
            var frmGoodsMeasure = new frm_inv_goods_glb_measure();
            var GoodsMeasure = new stp_inv_goods_glb_measure_selResult();
            GoodsMeasure.inv_goods_glb_measure_inv_group_goods_id = selectedRecord.inv_group_goods_id;
            GoodsMeasure.inv_goods_glb_measure_inv_group_goods_code = selectedRecord.inv_group_goods_code;
            GoodsMeasure.inv_goods_glb_measure_inv_group_goods_name = selectedRecord.inv_group_goods_name;
            GoodsMeasure.inv_goods_glb_measure_basic_glb_measure_id = selectedRecord.inv_group_goods_glb_measure_id;
            GoodsMeasure.inv_goods_glb_measure_basic_glb_measure_name = selectedRecord.inv_group_goods_glb_measure_name;
            frmGoodsMeasure.ShowGoodsMeasure(GoodsMeasure);
            frmGoodsMeasure.ShowDialog();
        }

        #endregion

    }
}
