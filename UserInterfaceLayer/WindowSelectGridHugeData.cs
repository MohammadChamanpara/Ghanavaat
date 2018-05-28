using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using APMTools;
using APMComponents;
using System.Windows.Input;
using Microsoft.Windows.Controls;


namespace UserInterfaceLayer
{
    public class WindowSelectGridHugeData<RT> : WindowSelectGrid<RT>
    {
        #region Constructor
        public WindowSelectGridHugeData()
            : base(SelectType.WindowSelectGrid)
        {
            txtCode.Visibility = Visibility.Collapsed;
            lblCode.Visibility = Visibility.Collapsed;
            lblName.Content = "جستجو روی همه فیلد ها :";
        }
        #endregion

        #region override
        public override void CallFilter()
        {
            GlobalFunctions.SetValueToProperty(RecordParameter, FieldNames<RT>.SimpleSearch, txtName.Text);
            ShowSomeRecords(RecordParameter);
        }
        #endregion
    }
}
