using System.Linq;
using System.Windows;
using System.Windows.Controls;
using APMComponents;
using BusinessLogicLayer;
using DataAccessLayer;
using UserInterfaceLayer;

namespace APM_Accounting
{
    public partial class frm_acc_rpt_chart_balance : WindowReport<stp_acc_rpt_chart_balance_selResult>
    {
        #region Constuctor
        public frm_acc_rpt_chart_balance()
        {
            InitializeComponent();
            Initial_WindowReport(null, dbg_acc_rpt_chart_balance, tbr_acc_rpt_chart_balance, "acc_rpt_chart_balance", new APM_SubSystems.APM_Accounting.acc_Reports.chart_balance.rpt_acc_chart_balance());
        }
        #endregion

        #region Tools
        private void CreateLevelNo(int level_no)
        {
            APMRadioButton radioButton;
            StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical,Margin=new Thickness(5) };
            grp_acc_rpt_chart_balance.Content = stackPanel;
            for (int i = 1; i <= level_no; i++)
            {
                radioButton = new APMRadioButton() { Tag = i};
                stackPanel.Children.Add(radioButton);
                radioButton.Checked += new RoutedEventHandler(radioButton_Checked);
                if (i == 1)
                    radioButton.Content = "گروه";
                else if (i == 2)
                    radioButton.Content = "کل";
                else if (i == 3)
                    radioButton.Content = "معین";
                else
                    radioButton.Content = " تفصیل" + (i-3).ToString();
            }
        }
        #endregion

        #region Event
        void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedRecord.acc_rpt_chart_balance_acc_chart_account_level_no = (int)(sender as APMRadioButton).Tag;
        }
        //private void APMToolbarButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ShowSomeRecords(RecordParameter);
        //}

        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            BLL<stp_acc_options_selResult> bllAccOptions = new BLL<stp_acc_options_selResult>();
            var level_no = bllAccOptions.GetAllRecords_DB().Max().acc_options_detail_level_count;

            CreateLevelNo(level_no + 3);
        }
        #endregion
    }
}
