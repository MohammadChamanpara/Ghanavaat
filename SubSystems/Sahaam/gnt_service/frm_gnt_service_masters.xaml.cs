using UserInterfaceLayer;
using APMTools;
using DataAccessLayer;
using APM_SubSystems;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Documents;
using System.Collections.Generic;
using APM_Accounting;
using BusinessLogicLayer;
using APM_SubSystems.Sahaam.gnt_service;

namespace APM_SubSystems
{
    public partial class frm_gnt_service_masters : WindowBase<stp_gnt_service_masters_selResult>
    {
        #region Constructor
        public frm_gnt_service_masters()
        {
            InitializeComponent();
            Initial_WindowBase(dbg_gnt_service, toolbar, null, "gnt_service_masters", true, null);
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
        }
        #endregion

        private void btnShowServiceDetails_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecord == null)
                return;
            var creditor = new stp_gnt_creditor_selResult();
            creditor.gnt_creditor_id = selectedRecord.gnt_service_gnt_creditor_id;
            creditor.gnt_creditor_name = selectedRecord.gnt_service_gnt_creditor_name;
            new frm_gnt_service(creditor).ShowDialog();
        }

        #region Events
        #endregion

        #region Methods
        #endregion
    }
}
