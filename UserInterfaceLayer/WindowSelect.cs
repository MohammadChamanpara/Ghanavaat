using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using APMComponents;
using APMTools;

namespace UserInterfaceLayer
{
    public class WindowSelect<RT> : WindowBase<RT>
    {
        #region Variables
        public Type mainForm;
        private object displayerControl;
        public enum SelectType { WindowSelectGrid, WindowSelectTree, WindowSelectGridGroup };
        private SelectType selectType;
        public Boolean multiSelect=false;
        public List<RT> selectedListAfterChange = new List<RT>();
        public List<RT> selectedListBeforeChange = new List<RT>();
        private string userXoperation;

        #endregion

        #region Controls
        public APMComboBox aPMComboBox;
        public APMLabel lblName, lblCode;
        public APMTextBox txtName, txtCode;
        APMToolBar APMToolBar;
        APMGroupBox APMGroupBoxFilter;
        APMGroupBox APMGroupBoxDisplay;
        APMGroupBox APMGroupBoxGridGroup;
        APMDataGridExtended dataGridExtended;
        APMStatusBar APMStatusBar;
        #endregion

        #region Initial_WindowSelect
        public void Initial_WindowSelect(SelectType selectType, WindowSearch<RT> userSearchForm)
        {
            this.selectType = selectType;
            if (selectType == SelectType.WindowSelectTree)
            {
                displayerControl = tree;
                tree.ContextMenu = null;
            }
            else
            {
                displayerControl = dataGridExtended;
                dataGridExtended.ContextMenu = null;
            }

            APMGroupBoxDisplay.Content = displayerControl;
            var userDataGrid = (displayerControl is APMDataGridExtended) ? displayerControl as APMDataGridExtended : null;
            userXoperation = FieldNames<RT>.FixPart.Substring(0, FieldNames<RT>.FixPart.Length - 1);
            Initial_WindowBase(userDataGrid, APMToolBar, null, userXoperation, false, userSearchForm);

            if (txtName != null)
                txtName.TextChanged += txtTextChanged;
            if (txtCode != null)
                txtCode.TextChanged += txtTextChanged;
            GlobalFunctions.SetVisibilityForControl(txtCode, GlobalFunctions.PropertyExist(selectedRecord, FieldNames<RT>.Code));
            GlobalFunctions.SetVisibilityForControl(lblCode, GlobalFunctions.PropertyExist(selectedRecord, FieldNames<RT>.Code));
            GlobalFunctions.SetVisibilityForControl(APMGroupBoxGridGroup, selectType == SelectType.WindowSelectGridGroup);
        }


        #endregion

        #region Constructor
        public WindowSelect()
        {
            CreateControls();
            DesignTheForm();
            AssignEvents();
            this.Width = 450;
            this.Height = 450;
            KeyGesture UpKeyGesture = new KeyGesture(Key.Up);
            KeyGesture DownKeyGesture = new KeyGesture(Key.Down);
            KeyBinding Up = new KeyBinding(ApplicationCommands.NotACommand, UpKeyGesture);
            KeyBinding Down = new KeyBinding(ApplicationCommands.NotACommand, DownKeyGesture);
            txtName.InputBindings.Add(Up);
            txtName.InputBindings.Add(Down);
            txtCode.InputBindings.Add(Up);
            txtCode.InputBindings.Add(Down);
            txtCode.KeyDown += new KeyEventHandler(txt_KeyDown);
            txtName.KeyDown += new KeyEventHandler(txt_KeyDown);
        }
        #endregion

        #region Override
        public override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);
            CreateGroupBoxHeader();
            if (txtName != null)
                txtName.Focus();
            selectedRecord = selectedRecord;
        }
        public override void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Enter)
                SelectClick();
            else if (e.Key == Key.Up)
                PreviousClick();
            else if (e.Key == Key.Down)
                NextClick();
            base.Window_KeyDown(sender, e);
        }
        public override void SelectClick()
        {
            if (txtName != null)
                txtName.Focus();
            else if (txtCode != null)
                txtCode.Focus();
            if (BLL == null || !ValidationForSelect())
                return;
            if (collectionView != null && collectionView.Count > 0)
            {
                if (this.DialogResult != true)
                    this.DialogResult = true;
                Close();
            }
            else
                Messages.NotExistRowForSelect("ردیفی");
        }
        public override void RefreshClick()
        {
            LoadDataFromDB();
        }
        public override void InsertClick()
        {
            if (mainForm == null)
                return;
            var mainFormShow = Activator.CreateInstance(mainForm);
            if (mainFormShow is Window)
                (mainFormShow as Window).ShowDialog();
            RefreshClick();
        }
        public override void collectionView_CurrentChanged(object sender, EventArgs e)
        {
            if (collectionView.CurrentItem is RT)
                base.collectionView_CurrentChanged(sender, e);
            else if
            (
                (collectionView.CurrentItem is APMTreeViewItem) &&
                (collectionView.CurrentItem as APMTreeViewItem).Tag is RT
            )
                selectedRecord = (RT)(collectionView.CurrentItem as APMTreeViewItem).Tag;
        }
        #endregion

        #region Virtual Methods
        public virtual void CallFilter() { }
        public virtual void APMTree_MouseDoubleClick(object sender, MouseButtonEventArgs e) { }
        public virtual void APMComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        #endregion

        #region Tools
        public void CreateControls()
        {
            aPMComboBox = new APMComboBox();
            APMGroupBoxFilter = new APMGroupBox() { Header = "فیلتر کردن" };
            APMGroupBoxDisplay = new APMGroupBox();
            APMGroupBoxGridGroup = new APMGroupBox();
            dataGridExtended = new APMDataGridExtended();
            APMStatusBar = new APMStatusBar() { XType = APMStatusBar.XStatusBarType.SelectWindow };
        }
        private void DesignTheForm()
        {
            APMBorder APMBorder = new APMBorder();
            this.Content = APMBorder;
            APMDockPanel APMDockPanel = new APMDockPanel();
            APMBorder.Child = APMDockPanel;
            APMToolBar = new APMToolBar() { XType = XWindowType.SelectWindow };
            APMDockPanel.Children.Add(APMToolBar);
            APMDockPanel.Children.Add(APMStatusBar);
            APMDockPanel.Children.Add(APMGroupBoxFilter);
            APMGroupBoxFilter.SetValue(DockPanel.DockProperty, Dock.Bottom);
            StackPanel StackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            lblName = new APMLabel() { Content = "نام:" };
            txtName = new APMTextBox();
            lblCode = new APMLabel() { Content = "کد:" };
            txtCode = new APMTextBox();
            StackPanel.Children.Add(new APMLabel() { Width = 40 });
            StackPanel.Children.Add(lblName);
            StackPanel.Children.Add(txtName);
            StackPanel.Children.Add(new APMLabel() { Width = 100 });
            StackPanel.Children.Add(lblCode);
            StackPanel.Children.Add(txtCode);
            APMGroupBoxFilter.Content = StackPanel;
            APMGroupBoxGridGroup.SetValue(DockPanel.DockProperty, Dock.Top);
            WrapPanel WrapPanel = new WrapPanel() { Margin = new Thickness(8) };
            WrapPanel.Children.Add(aPMComboBox);
            APMGroupBoxGridGroup.Content = WrapPanel;
            APMDockPanel.Children.Add(APMGroupBoxGridGroup);
            tree = new APMTree() { Background = System.Windows.Media.Brushes.Transparent, Margin = new Thickness(7) };
            APMDockPanel.Children.Add(APMGroupBoxDisplay);
        }
        private void AssignEvents()
        {
            aPMComboBox.SelectionChanged += new SelectionChangedEventHandler(APMComboBox_SelectionChanged);
            tree.MouseDoubleClick += new MouseButtonEventHandler(APMTree_MouseDoubleClick);
        }
        public Boolean FilterRecord(object input)
        {
            RT record;
            if (input is APMTreeViewItem)
                record = (RT)(input as APMTreeViewItem).Tag;
            else
            {
                if (input == null || !(input is RT))
                    return false;
                record = (RT)input;
            }
            string name = GlobalFunctions.GetValueFromProperty<RT, string>(record, FieldNames<RT>.Name);
            string code = GlobalFunctions.GetValueFromProperty<RT, string>(record, FieldNames<RT>.Code);


            if 
            (
                (
                    txtName == null || 
                    txtName.Text.Trim() == "" || 
                    (
                        (txtName.Text.Contains('ی') || txtName.Text.Contains('ي')) && name.Replace('ي', 'ی').Contains(txtName.Text.Replace('ي', 'ی'))
                    ) ||
                    name.Contains(txtName.Text)
                )
                &&
                (
                    txtCode == null || 
                    txtCode.Text.Trim() == "" || 
                    code.StartsWith(txtCode.Text)
                )
            )
                return true;
            return false;
        }
        public void CreateGroupBoxHeader()
        {
            if (displayerControl == null)
                return;
            DependencyObject parent = null;
            GroupBox grpHeader;

            if (displayerControl is APMDataGridExtended)
                parent = (displayerControl as APMDataGridExtended).Parent;
            else if (displayerControl is APMTree)
                parent = (displayerControl as APMTree).Parent;
            if (parent == null || !(parent is GroupBox))
                return;
            grpHeader = parent as GroupBox;
            string name = GlobalFunctions.GetValueFromProperty<RT, string>(RecordParameter, FieldNames<RT>.Name);
            grpHeader.Header = "انتخاب " + selectHeader;
            if (name != "")
                grpHeader.Header = grpHeader.Header + " برای " + name;

        }
        #endregion

        #region Events
        void txt_KeyDown(object sender, KeyEventArgs e)
        {
            Window_KeyDown(sender, e);
        }

        private void txtTextChanged(object sender, TextChangedEventArgs e)
        {
            CallFilter();
            (sender as TextBox).Focus();
            if (collectionView != null)
                collectionView.MoveCurrentToFirst();
        }

        #endregion
    }
}
