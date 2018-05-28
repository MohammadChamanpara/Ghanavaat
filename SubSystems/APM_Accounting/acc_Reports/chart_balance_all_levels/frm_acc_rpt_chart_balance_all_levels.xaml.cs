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
using APMComponents;
using APMTools;
using BusinessLogicLayer;
using UserInterfaceLayer;
using DataAccessLayer;

namespace APM_Accounting
{
    public partial class frm_acc_rpt_chart_balance_all_levels: WindowReport<stp_acc_rpt_chart_balance_all_levels_selResult>
    {
        #region Variable
        #endregion

        #region Constructor
        public frm_acc_rpt_chart_balance_all_levels()
        {
            InitializeComponent();
            Initial_WindowReport(null, dbg_acc_rpt_chart_balance_all_levels, tbr_acc_rpt_chart_balance_all_levels, "acc_rpt_chart_balance_all_levels", new APM_SubSystems.APM_Accounting.acc_Reports.chart_balance_all_levels.rpt_acc_chart_balance_all_levels());
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            BLL<stp_acc_options_selResult> bllAccOptions = new BLL<stp_acc_options_selResult>();
            var level_no = bllAccOptions.GetAllRecords_DB().Max().acc_options_detail_level_count;

            CreateLevelNo(level_no + 3);
        }
        public override void SearchClick()
        {
            string AltogetherLevelNo = "";

            var parentgroup = grp_acc_rpt_chart_balance_all_levels.Content;
            if (parentgroup is StackPanel)
            {
                var stackPanel = parentgroup as StackPanel;
                foreach (object child in stackPanel.Children)
                {
                    if (child is APMCheckBox)
                    {
                        var CheckBox = child as APMCheckBox;
                        if (CheckBox.IsChecked == true)
                            AltogetherLevelNo = AltogetherLevelNo + CheckBox.Tag.ToString();
                    }
                }
            }
            selectedRecord.acc_rpt_chart_balance_all_levels_acc_chart_account_level_no = AltogetherLevelNo;
            base.SearchClick();

        }
        #endregion

        #region Tools
        private void CreateLevelNo(int level_no)
        {
            APMCheckBox CheckBox;
            StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(5) };
            grp_acc_rpt_chart_balance_all_levels.Content = stackPanel;
            for (int i = 1; i <= level_no; i++)
            {
                CheckBox = new APMCheckBox() { Tag = i };
                stackPanel.Children.Add(CheckBox);
                if (i == 1)
                    CheckBox.Content = "گروه";
                else if (i == 2)
                    CheckBox.Content = "کل";
                else if (i == 3)
                    CheckBox.Content = "معین";
                else
                    CheckBox.Content = " تفصیل" + (i - 3).ToString();
            }
        }

        
        #endregion

        #region Event
        #endregion 

    }
}
