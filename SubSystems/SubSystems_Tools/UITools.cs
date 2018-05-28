using System.Windows;
using System.Windows.Controls;
using APMComponents;
using DataAccessLayer;
using BusinessLogicLayer;
using APMTools;
using APM_Accounting;
using System;
using System.Linq;

namespace APM_SubSystems
{
    static class UITools
    {
        #region Variables
        public static frm_main MainWindow = new frm_main();
        #endregion

        #region Methods
        public static void Create_UserEntityButton(stp_glb_entity_type_selResult newEntityType)
        {
            APMButton btn_glb_user_entity = new APMButton()
            {
                XSubSystem = APMTools.SubSystems.Accounting,
                Content = newEntityType.glb_entity_type_name,
                Margin = new Thickness(3),
                Tag = newEntityType
            };
            btn_glb_user_entity.Click += new RoutedEventHandler(btn_glb_user_entity_Click);
            MainWindow.skp_glb_user_entity.Children.Add(btn_glb_user_entity);
            GlobalFunctions.SetVisibilityForControl(MainWindow.expGlbUserEntities, MainWindow.skp_glb_user_entity.Children.Count != 0);
        }
        public static void Edit_UserEntityButton(stp_glb_entity_type_selResult EdiEntityType)
        {
            for (int i = 0; i < MainWindow.skp_glb_user_entity.Children.Count; i++)
            {
                var button = MainWindow.skp_glb_user_entity.Children[i];
                if (button is Button && ((button as Button).Tag as stp_glb_entity_type_selResult).glb_entity_type_id == EdiEntityType.glb_entity_type_id)
                {
                    (button as APMButton).Content = EdiEntityType.glb_entity_type_name;
                    GlobalFunctions.SetVisibilityForControl(MainWindow.expGlbUserEntities, MainWindow.skp_glb_user_entity.Children.Count != 0);
                    return;
                }
            }
        }
        private static void btn_glb_user_entity_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button).Tag is stp_glb_entity_type_selResult))
                return;
            new frm_glb_user_entity((sender as Button).Tag as stp_glb_entity_type_selResult).ShowDialog();
        }
        public static void Create_UserEntityButtons()
        {
            MainWindow.skp_glb_user_entity.Children.Clear();
            var listForEntity_Type = new BLL<stp_glb_entity_type_selResult>().GetAllRecords_DB();
            listForEntity_Type = listForEntity_Type.FindAll(record => record.glb_entity_type_id > GlobalVariables.FixEntitysCount);
            if (listForEntity_Type.Count == 0)
                GlobalFunctions.SetVisibilityForControl(MainWindow.expGlbUserEntities, false);
            else
                foreach (stp_glb_entity_type_selResult record in listForEntity_Type)
                    Create_UserEntityButton(record);
        }
        public static void Delete_UserEntityButton(stp_glb_entity_type_selResult deleteEntityType)
        {
            for (int i = 0; i <= MainWindow.skp_glb_user_entity.Children.Count; i++)
            {
                var button = MainWindow.skp_glb_user_entity.Children[i];
                if (button is Button && ((button as Button).Tag as stp_glb_entity_type_selResult).glb_entity_type_id == deleteEntityType.glb_entity_type_id)
                {
                    MainWindow.skp_glb_user_entity.Children.Remove((button as Button));
                    GlobalFunctions.SetVisibilityForControl(MainWindow.expGlbUserEntities, MainWindow.skp_glb_user_entity.Children.Count != 0);
                    return;
                }
            }

        }
        public static bool CurrentUserHasAccessTo(string controlName)
        {
            var AccessDeniedList= DDB.NewContext().tbl_glb_user_access
                .Where
                (
                    x => 
                    x.glb_user_access_glb_user_id == GlobalVariables.current_user_id && 
                    x.tbl_glb_user_access_items.glb_user_access_items_control_name.ToLower() == controlName.ToLower()
                )
                .ToList();

            if (AccessDeniedList.Count > 0)
                return false;

            return true;
        }
        #endregion
    }
}