using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DataAccessLayer;
using APMTools;
using BusinessLogicLayer;
using UserInterfaceLayer;
using System.Linq;
using APM_SubSystems.Sahaam.gnt_creditor;
using APM_SubSystems.Sahaam.gnt_water;

namespace APM_SubSystems
{
    public partial class frm_gnt_water_ownerships : WindowBase<stp_gnt_ownership_selResult>
    {
        #region Properties
        stp_gnt_water_selResult CurrentWater { get; set; }
        #endregion

        #region Constructor
        public frm_gnt_water_ownerships(stp_gnt_water_selResult currentWater)
        {
            InitializeComponent();
            Initial_WindowBase(dbgCreditor, windowToolbar, null, "gnt_water_ownerships", true, null);
            this.CurrentWater = currentWater;
        }

        #endregion

        #region override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.RecordParameter.gnt_ownership_gnt_water_id = this.CurrentWater.gnt_water_id;
            base.Window_Loaded(sender, e);
            lbl_water_name.Content = this.CurrentWater.gnt_water_name;
            var dataBase = DDB.NewContext();
            if (dataBase.tbl_gnt_settings.Count() == 0)
            {
                Messages.ErrorMessage("تنظیمات سیستم سهام وارد نشده اند");
                return;
            }
        }
        public override void PrintClick()
        {
            btnPrint_Click(null, null);
        }

        #endregion

        #region Events
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printForm = new WindowPrint<stp_gnt_water_selResult, stp_gnt_ownership_selResult>(new gnt_rpt_water_ownerships());
                printForm.articleList = allRecords;
                printForm.selectedRecord = CurrentWater;
                printForm.ShowDialog();
            }
            catch (Exception exception)
            {
                Messages.ErrorMessage(exception.CompleteMessages());
            }
        }
        #endregion

    }
}


