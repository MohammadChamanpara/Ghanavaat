using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataAccessLayer;
using UserInterfaceLayer;
using APMComponents;

namespace APM_Accounting
{
    public partial class frm_glb_user_entity : WindowEntity<stp_glb_user_entity_selResult>
    {
        string header;
        public frm_glb_user_entity(stp_glb_entity_type_selResult Recordentity)
        {
            entity_type = Recordentity.glb_entity_type_id;
            this.header = Recordentity.glb_entity_type_name;
            InitializeComponent();
            Initial_WindowEntity(dbg_glb_user_entity, tbr_glb_user_entity, grp_glb_user_entity, "glb_user_entity", false, null, entity_type, tpc_glb_user_entity_child_code);
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RecordParameter.glb_user_entity_glb_entity_type_id = entity_type;
            ShowSomeRecords(RecordParameter);
            grp2_glb_user_entity.Header = " لیست " + header;
            this.Title = header;
            base.Window_Loaded(sender, e);
        }
        public override void OperationsAfterSaved()
        {
            base.OperationsAfterSaved();
            selectedRecord.glb_user_entity_name = selectedRecord.glb_user_entity_real_name;
        }
    }
}
