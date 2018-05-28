using System.Windows;
using System.Windows.Input;
using APM_SubSystems;
using DataAccessLayer;
using UserInterfaceLayer;
using BusinessLogicLayer;
using APMTools;

namespace APM_SubSystems
{
    public partial class frm_inv_store : WindowEntity<stp_inv_store_selResult>
    {
        #region frm_inv_store
        public frm_inv_store()
        {
            InitializeComponent();
            Initial_WindowEntity(dbg_inv_store, tbr_inv_store, grpStoreInfo, "inv_store", true, null, (long)EntityType.inv_store, tpc_inv_store_child_code);
        }
        #endregion

        #region Event
        private void brw_inv_store_glb_personal_browseclick(object sender, RoutedEventArgs e)
        {        
           BrowseClick(new WindowSelectGridGroup<stp_glb_personel_selResult, stp_glb_personel_group_selResult>(), "انبار دار",typeof(frm_Personel),sender);
        }
        private void brw_inv_store_glb_personal_KeyDown(object sender, KeyEventArgs e)
        {
            CodeTextBox_KeyDown<stp_glb_personel_selResult>(sender, FieldNames<stp_inv_store_selResult>.PersonelId, e);
        }
        #endregion

        private void brw_browse_goods_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_Parameter(new WindowSelectGrid<stp_inv_goods_store_selResult>(), selectedRecord, 
                new stp_inv_goods_store_selResult() { inv_goods_store_inv_store_id=selectedRecord.inv_store_id}, 
                "کالا", typeof(frm_group_goods), sender); 
        }

        

    }
}
