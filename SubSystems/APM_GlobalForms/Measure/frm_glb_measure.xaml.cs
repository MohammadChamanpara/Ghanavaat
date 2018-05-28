using System;
using System.Windows;
using System.Windows.Controls;
using UserInterfaceLayer;
using DataAccessLayer;
using APMTools;
using APMComponents;


namespace APM_SubSystems
{
    public partial class frm_glb_Measure : WindowTreeGrid<stp_glb_measure_selResult>
    {
        #region Constructor
        public frm_glb_Measure()
        {
            InitializeComponent();
            Initial_WindowTreeGrid(dbg_glb_measure, tbr_glb_measure, grp_glb_measure, "glb_secondary_measure", null, tre_glb_measure, "واحد های شمارش", 0, 0);
          
        }
        #endregion

        #region Events
        private void txt_glb_measure_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (parentNode == null)
                return;
            lbl_glb_measure_name.Content = (GlobalFunctions.GetValueFromProperty<stp_glb_measure_selResult, long>(selectedRecord, FieldNames<stp_glb_measure_selResult>.ParentID) != 0) ? ((APMTreeViewItem)parentNode).XCaption : "";
            lbl_glb_measure_name_main.Content = (GlobalFunctions.GetValueFromProperty<stp_glb_measure_selResult, long>(selectedRecord, FieldNames<stp_glb_measure_selResult>.ParentID) != 0) ? txt_glb_measure_name.Text : "";
        }
        #endregion

        #region Overrided Methods
        public override Boolean ChangeSelectedRecord(stp_glb_measure_selResult newSelectedRecord, object caller)
        {
           
            SetVisibilityFor_SkpNoMain();
            
            if (!base.ChangeSelectedRecord(newSelectedRecord, caller))
                return false;
                
            return true;
        }
        void SetVisibilityFor_SkpNoMain()
        {
            skp_No_main.Visibility = GlobalFunctions.BooleanToVisibility((selectedRecord.glb_measure_parent_id) != 0 && (selectedRecord.glb_measure_parent_id)!=null);
            txt_glb_measure_name_TextChanged(null, null);
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            if (skp_No_main.Visibility == Visibility.Visible)
                selectedRecord.glb_measure_full_formula = "هر" + lbl_glb_measure_name.Content + " = " + txt_glb_measure_formula.Text + lbl_glb_measure_name_main.Content;
        }
        public override void OperationsAfterInsert()
        {
            base.OperationsAfterInsert();
            SetVisibilityFor_SkpNoMain();
        }

        #endregion
    }
}