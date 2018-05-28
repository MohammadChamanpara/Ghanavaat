using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Forms.Integration;
using APMTools;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;


namespace UserInterfaceLayer
{
    public class WindowPrint<MasterRT, ArticleRT> : WindowBase<MasterRT>
    {
        #region Variables
        private List<KeyValuePair<string, string>> CustomParameters = new List<KeyValuePair<string, string>>();
        public ReportDocument reportDocument;
        public CrystalReportViewer crystalReportViewer;
        public DataTable dt = new DataTable();
        public List<ArticleRT> articleList = new List<ArticleRT>();
        #endregion

        #region Constructor
        public WindowPrint(ReportDocument report)
        {
            Initial_WindowBase(null, null, null, null, false, null);
            this.crystalReportViewer = new CrystalReportViewer();
            this.Content = new APMComponents.APMBorder() { Child = new WindowsFormsHost() { Child = crystalReportViewer } };
            this.reportDocument = report;
            this.crystalReportViewer.Navigate += new NavigateEventHandler(crystalReportViewer_Navigate);
            this.crystalReportViewer.ShowGroupTreeButton = false;
            this.crystalReportViewer.ShowRefreshButton = false;
        }
        #endregion

        #region VirtualMethods
        public virtual void SetParameterValues()
        {
            foreach (ParameterField parameterfield in reportDocument.ParameterFields)
            {
                object fieldValue;
                string fieldName = parameterfield.Name;

                if (fieldName == "current_branch_name")
                    fieldValue = GlobalVariables.current_branch_name;
                else if (fieldName == "current_company_name")
                    fieldValue = GlobalVariables.current_company_name;
                else if (fieldName == "current_user_name")
                    fieldValue = GlobalVariables.current_user_name;
                else if (fieldName == "current_date")
                    fieldValue = APMTools.APMDateTime.Today;
                else if (CustomParameters.Exists(x => x.Key == fieldName))
                    fieldValue = CustomParameters.Find(x => x.Key == fieldName).Value;
                else if (GlobalFunctions.PropertyExist(selectedRecord, fieldName) || GlobalFunctions.PropertyExist(selectedRecord, FieldNames<MasterRT>.FixPart + fieldName))
                {
                    fieldValue = GlobalFunctions.GetValueFromProperty(selectedRecord, fieldName);
                    if (fieldValue == null)
                        fieldValue = GlobalFunctions.GetValueFromProperty(selectedRecord, FieldNames<MasterRT>.FixPart + fieldName);
                }
                else
                    fieldValue = null;

                //reportDocument.SetParameterValue(fieldName, (fieldValue != null && ((object)"") != fieldValue) ? fieldValue : "مشخص نشده");
                reportDocument.SetParameterValue(fieldName, (fieldValue != null && ((object)"") != fieldValue) ? fieldValue : "-");

            }
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            if (articleList != null)
                dt = GlobalFunctions.ListToDataTable(articleList);
            if (crystalReportViewer == null || reportDocument == null)
                return;
            crystalReportViewer.Refresh();
            reportDocument.Refresh();
            reportDocument.SetDataSource(dt);

            SetParameterValues();
            crystalReportViewer.ReportSource = reportDocument;
            crystalReportViewer.Refresh();
            reportDocument.Refresh();
        }
        void crystalReportViewer_Navigate(object source, NavigateEventArgs e)
        {
            SetParameterValues();
        }
        #endregion

        #region Methods
        public void AddCustomParameter(string parameterName, string parameterValue)
        {
            this.CustomParameters.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
        }
        #endregion
    }
}
