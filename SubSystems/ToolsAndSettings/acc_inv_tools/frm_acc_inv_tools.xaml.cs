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
using System.Data.Linq;
using DataAccessLayer;
using UserInterfaceLayer;
using BusinessLogicLayer;
using APMTools;
using APMComponents;
using APM_Accounting;
namespace APM_SubSystems
{
    public partial class frm_acc_inv_tools : WindowBase<stp_acc_document_selResult>
    {
        #region Variables
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        #endregion

        #region Constructor
        public frm_acc_inv_tools()
        {
            InitializeComponent();
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pgb_calculate_debt_credit.Value = 0;
            pgb_calculate_price_of_send_and_receive.Value = 0;
            pgb_document_sort_code.Value = 0;
            pgb_document_sort_nos.Value = 0;
        }
        #endregion

        #region tools
        private void progressbar(ProgressBar ProgressBar)
        {
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = 100;
            ProgressBar.Value = 0;
            double value = 0;
            UpdateProgressBarDelegate updatePbDelegate =
                new UpdateProgressBarDelegate(ProgressBar.SetValue);

            do
            {
                value += 10;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
            }
            while (ProgressBar.Value != ProgressBar.Maximum);
        }
        #endregion

        #region Event
        private void btn_calculate_debt_credit_Click(object sender, RoutedEventArgs e)
        {
            if (pgb_calculate_debt_credit.Value == 100)
                return;
            var BLL_Calculate_Debt_Credit = new BLL<stp_acc_chart_account_calculate_debt_creditResult>();
            if (!BLL_Calculate_Debt_Credit.DoDataBaseOperation())
                return;
            progressbar(pgb_calculate_debt_credit);
        }

        private void btn_calculate_price_of_send_and_receive_Click(object sender, RoutedEventArgs e)
        {
            if (pgb_calculate_price_of_send_and_receive.Value == 100)
                return;
            var BLL_calculate_price_of_send_and_receive = new BLL<stp_acc_inv_calculate_price_of_sends_and_receivesResult>();
            if (!BLL_calculate_price_of_send_and_receive.DoDataBaseOperation())
                return;
            progressbar(pgb_calculate_price_of_send_and_receive);
        }

        private void btn_document_sort_nos_Click(object sender, RoutedEventArgs e)
        {
            if (pgb_document_sort_nos.Value == 100)
                return;
            var BLL_SortNos = new BLL<stp_inv_document_sort_nosResult>();
            if (!BLL_SortNos.DoDataBaseOperation())
                return;
            progressbar(pgb_document_sort_nos);
        }

        private void btn_document_sort_code_Click(object sender, RoutedEventArgs e)
        {
            if (pgb_document_sort_code.Value == 100)
            return;
            var BLL_Codes = new BLL<stp_inv_document_sort_codesResult>();
            if (!BLL_Codes.DoDataBaseOperation())
                return;
            progressbar(pgb_document_sort_code);
        }
        private void btn_calculate_have_acc_document_Click(object sender, RoutedEventArgs e)
        {
            if (pgb_calculate_have_acc_document.Value == 100)
                return;
            var BLL_calculate_have_acc_document = new BLL<stp_acc_inv_calculate_have_acc_documentResult>();
            if (!BLL_calculate_have_acc_document.DoDataBaseOperation())
                return;
            progressbar(pgb_calculate_have_acc_document);

        }

        #endregion

    }
}
