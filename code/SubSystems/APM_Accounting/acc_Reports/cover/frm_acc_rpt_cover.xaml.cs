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
    public partial class frm_acc_rpt_cover : WindowReport<stp_acc_rpt_cover_selResult>
    {
        #region Variables

        private ArticlePackage<stp_acc_document_selResult, stp_acc_document_selResult> ref_acc_document =
            new WindowBase<stp_acc_rpt_cover_selResult>.ArticlePackage<stp_acc_document_selResult, stp_acc_document_selResult>();

        #endregion

        #region Constructor
        public frm_acc_rpt_cover()
        {
            InitializeComponent();
            Initial_WindowReport(null, dbg_acc_rpt_cover, tbr_acc_rpt_cover, "acc_rpt_cover", new APM_SubSystems.APM_Accounting.acc_Reports.cover.rpt_acc_cover());
        }
        #endregion

        #region Override
        public override bool SameFilterObjects()
        {
            return false;
        }
        public override void LoadData()
        {
            try
            {
                var levels = FindSelectedLevels();
                var context = DDB.NewContext();
                List<tbl_acc_document_article> articles = new List<tbl_acc_document_article>();
                foreach (var document in ref_acc_document.ListAfterChange)
                    articles.AddRange(context.tbl_acc_document_article.Where(x => x.acc_document_article_acc_document_id == document.acc_document_id).ToList());

                allRecords = new List<stp_acc_rpt_cover_selResult>();

                foreach (int levelNo in levels)
                {
                    foreach (var article in articles)
                    {
                        var parentCA = GetParentCA(article, levelNo);
                        if (parentCA == null)
                            continue;
                        var findedCA = allRecords.Find(x => x.acc_rpt_cover_acc_chart_account_id == parentCA.acc_rpt_cover_acc_chart_account_id);
                        if (findedCA == null)
                            allRecords.Add(parentCA);
                        else
                        {
                            findedCA.acc_rpt_cover_acc_chart_account_credit += parentCA.acc_rpt_cover_acc_chart_account_credit;
                            findedCA.acc_rpt_cover_acc_chart_account_debt += parentCA.acc_rpt_cover_acc_chart_account_debt;
                        }
                    }
                }
                foreach (var record in allRecords)
                {
                    record.acc_rpt_cover_remaining = Math.Abs(record.acc_rpt_cover_acc_chart_account_debt.Value - record.acc_rpt_cover_acc_chart_account_credit.Value);
                    if (record.acc_rpt_cover_acc_chart_account_debt > record.acc_rpt_cover_acc_chart_account_credit)
                        record.acc_rpt_cover_specification = "بدهکار";
                    else if (record.acc_rpt_cover_acc_chart_account_debt < record.acc_rpt_cover_acc_chart_account_credit)
                        record.acc_rpt_cover_specification = "بستانکار";
                    else
                        record.acc_rpt_cover_specification = "تراز";
                }
                allRecords = allRecords.OrderBy(x => x.acc_rpt_cover_acc_chart_account_code).ToList();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            BLL<stp_acc_options_selResult> bllAccOptions = new BLL<stp_acc_options_selResult>();
            var level_no = bllAccOptions.GetAllRecords_DB().Max().acc_options_detail_level_count;

            CreateLevelNoCheckBoxes(level_no + 3);
        }
        public override void SearchClick()
        {
            if (ref_acc_document.ListAfterChange.Count == 0)
                Messages.ErrorMessage("ابتدا سند یا اسناد حسابداری را انتخاب نمایید.");
            base.SearchClick();

        }
        #endregion

        #region Tools

        private void CreateLevelNoCheckBoxes(int level_no)
        {
            APMCheckBox CheckBox;
            StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical, Margin = new Thickness(5) };
            grp_acc_rpt_cover.Content = stackPanel;
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

        private List<int> FindSelectedLevels()
        {
            List<int> levels = new List<int>();

            var parentgroup = grp_acc_rpt_cover.Content;
            if (parentgroup is StackPanel)
            {
                var stackPanel = parentgroup as StackPanel;
                foreach (object child in stackPanel.Children)
                {
                    if (child is APMCheckBox)
                    {
                        var CheckBox = child as APMCheckBox;
                        if (CheckBox.IsChecked == true)
                            levels.Add((int)CheckBox.Tag);
                    }
                }
            }
            return levels;
        }

        private stp_acc_rpt_cover_selResult GetParentCA(tbl_acc_document_article article, int levelNo)
        {
            var parent = article.tbl_acc_chart_account;
            while (parent != null && parent.acc_chart_account_level_no != levelNo)
            {
                parent = parent.tbl_acc_chart_account2;
            }
            string code = "";

            var parentForCode = parent;
            while (parentForCode != null)
            {
                code = parentForCode.acc_chart_account_child_code + code;
                parentForCode = parentForCode.tbl_acc_chart_account2;
            }

            if (parent == null)
                return null;
            return new stp_acc_rpt_cover_selResult()
            {
                acc_rpt_cover_acc_chart_account_code = code,
                acc_rpt_cover_acc_chart_account_credit = article.acc_document_article_credit,
                acc_rpt_cover_acc_chart_account_debt = article.acc_document_article_debt,
                acc_rpt_cover_acc_chart_account_id = parent.acc_chart_account_id,
                acc_rpt_cover_acc_chart_account_name = parent.acc_chart_account_name,
                acc_chart_account_level_no = levelNo
            };
        }

        #endregion

        #region Events
        private void brw_select_acc_document_XBrowseClick(object sender, RoutedEventArgs e)
        {
            BrowseClick_MultiSelect(new WindowSelectGrid<stp_acc_document_selResult>(), ref ref_acc_document, "انتخاب سند حسابداری", typeof(frm_acc_document));
            string selectedCodes = "";
            string selectedDescriptions = "";
            int maxDocuments = 5;

            ref_acc_document.ListAfterChange
                .Take(maxDocuments)
                .ToList()
                .ForEach(x =>
                {
                    selectedCodes = x.acc_document_code + "," + selectedCodes;
                    selectedDescriptions += x.acc_document_description + ",";
                });

            selectedCodes = selectedCodes.TrimEnd(',');
            selectedDescriptions = selectedDescriptions.TrimEnd(',');

            if (ref_acc_document.ListAfterChange.Count > maxDocuments)
            {
                selectedCodes += ",...";
                selectedDescriptions += ",...";
            }
            brw_select_acc_document.XLabel.Content = selectedCodes;
            brw_select_acc_document.XShowName = true;

            selectedRecord.acc_rpt_cover_acc_document_codes = selectedCodes;
            selectedRecord.acc_rpt_cover_acc_document_descriptions = selectedDescriptions;
        }
        #endregion

    }
}
