using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace APMComponents
{
    public enum CalendarMode
    {
        Month,
        Year,
        Decade,
    }
    [System.ComponentModel.DefaultEvent("SelectedDateChanged")]
    [System.ComponentModel.DefaultProperty("DisplayDate")]
    public partial class PersianCalendar : UserControl
    {
        
        //Properties
        public static readonly DependencyProperty DisplayDateProperty;
        /// <summary>
        /// Gets or sets the date that is being displayed in the calendar.
        /// </summary>
        public PersianDate DisplayDate { 
            get{ 
                return (PersianDate) this.GetValue(DisplayDateProperty);
            }
            set {
                this.SetValue(DisplayDateProperty,value);
            }
        }

        public static readonly DependencyProperty DisplayModeProperty;
        public CalendarMode DisplayMode
        {
            get { return (CalendarMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }


        /// <summary>
        /// the minimum date that is displayed, and can be selected
        /// </summary>
        public PersianDate DisplayDateStart
        {
            get { return (PersianDate)GetValue(DisplayDateStartProperty); }
            set { SetValue(DisplayDateStartProperty, value); }
        }
        public static readonly DependencyProperty DisplayDateStartProperty;


        /// <summary>
        /// the minimum date that is displayed, and can be selected
        /// </summary>
        public PersianDate DisplayDateEnd
        {
            get { return (PersianDate)GetValue(DisplayDateEndProperty); }
            set { SetValue(DisplayDateEndProperty, value); }
        }
        public static readonly DependencyProperty DisplayDateEndProperty;




        public PersianDate SelectedDate
        {
            get { return (PersianDate)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }               
        public static readonly DependencyProperty SelectedDateProperty;



        public Brush SelectedDateBackGround
        {
            get { return (Brush)GetValue(SelectedDateBackGroundProperty); }
            set { SetValue(SelectedDateBackGroundProperty, value); }
        }
        public static readonly DependencyProperty SelectedDateBackGroundProperty =
            DependencyProperty.Register("SelectedDateBackGround", typeof(Brush), typeof(PersianCalendar), new UIPropertyMetadata(Brushes.Lavender));



        public Brush TodayBackGround
        {
            get { return (Brush)GetValue(TodayBackGroundProperty); }
            set { SetValue(TodayBackGroundProperty, value); }
        }
        public static readonly DependencyProperty TodayBackGroundProperty;


        //properties coercions and changed event handlers
        static void DisplayDateStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar pc = d as PersianCalendar;
            pc.CoerceValue(DisplayDateEndProperty);
            pc.CoerceValue(SelectedDateProperty);
            pc.CoerceValue(DisplayDateProperty);
            modeChanged(d, new DependencyPropertyChangedEventArgs());
        }
        static void DisplayDateEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar pc = d as PersianCalendar;
            pc.CoerceValue(SelectedDateProperty);
            pc.CoerceValue(DisplayDateProperty);
            modeChanged(d, new DependencyPropertyChangedEventArgs());
        }

        static object coerceDisplayDateStart(DependencyObject d,object o){
            PersianCalendar pc=d as PersianCalendar;
            PersianDate value = (PersianDate)o;
            return o;

        }
        static object coerceDisplayDateEnd(DependencyObject d,object o){
            PersianCalendar pc=d as PersianCalendar;
            PersianDate value = (PersianDate)o;
            if (value < pc.DisplayDateStart)
            {
                return pc.DisplayDateStart;
            }
            return o;
        }
        static object coerceDateToBeInRange(DependencyObject d, object o)
        {
            PersianCalendar pc = d as PersianCalendar;
            PersianDate value = (PersianDate)o;
            if (value < pc.DisplayDateStart)
            {
                return pc.DisplayDateStart;
            }
            if (value > pc.DisplayDateEnd)
            {
                return pc.DisplayDateEnd;
            }
            return o;
        }

        //events

        public static readonly RoutedEvent SelectedDateChangedEvent;
        
        public event RoutedEventHandler SelectedDateChanged
        {
            add { AddHandler(SelectedDateChangedEvent, value); }
            remove { RemoveHandler(SelectedDateChangedEvent, value); }
        }
        

        static void modeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar pc = d as PersianCalendar;
            pc.setCalendar();
        }

        static PersianCalendar()
        {

            PropertyMetadata displayModeMetaData = new PropertyMetadata(modeChanged);
            DisplayModeProperty =
                DependencyProperty.Register("DisplayMode", typeof(CalendarMode), typeof(PersianCalendar), displayModeMetaData);
            
            PropertyMetadata displayDateMetaData = new PropertyMetadata(PersianDate.Today, modeChanged);
            displayDateMetaData.CoerceValueCallback = coerceDateToBeInRange;
            DisplayDateProperty =
                DependencyProperty.Register("DisplayDate", typeof(PersianDate), typeof(PersianCalendar), displayDateMetaData);

            
            PropertyMetadata selectedDateMetaData = new PropertyMetadata(PersianDate.Today,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                PersianCalendar pc = d as PersianCalendar;
                pc.selectedDateCheck((PersianDate)e.OldValue);
                pc.RaiseEvent(new RoutedEventArgs(SelectedDateChangedEvent,pc));
            }
            );
            selectedDateMetaData.CoerceValueCallback = coerceDateToBeInRange;
            SelectedDateProperty=
                DependencyProperty.Register("SelectedDate", typeof(PersianDate), typeof(PersianCalendar), selectedDateMetaData);

            PropertyMetadata displayDateStartMetaData = new PropertyMetadata
            {
                DefaultValue=new PersianDate(),
                CoerceValueCallback = new CoerceValueCallback(coerceDisplayDateStart),
                PropertyChangedCallback=new PropertyChangedCallback(DisplayDateStartChanged),
            };
                
            DisplayDateStartProperty=                
                DependencyProperty.Register("DisplayDateStart", typeof(PersianDate), typeof(PersianCalendar), displayDateStartMetaData);

            PropertyMetadata displayDateEndMetaData = new PropertyMetadata
            {
                DefaultValue=new PersianDate(10000,1,1),
                CoerceValueCallback = new CoerceValueCallback(coerceDisplayDateEnd),
                PropertyChangedCallback=new PropertyChangedCallback(DisplayDateEndChanged),
            };

            DisplayDateEndProperty =
                DependencyProperty.Register("DisplayDateEnd", typeof(PersianDate), typeof(PersianCalendar), displayDateEndMetaData);

            
            PropertyMetadata todayBackgroundMetaData = new PropertyMetadata(Brushes.AliceBlue,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                PersianCalendar pc = d as PersianCalendar;
                pc.todayCheck();
            }
            );
            TodayBackGroundProperty =
                DependencyProperty.Register("TodayBackGround", typeof(Brush), typeof(PersianCalendar),todayBackgroundMetaData);

            SelectedDateChangedEvent= EventManager.RegisterRoutedEvent("SelectedDateChanged", 
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PersianCalendar));
            
        }
        
        public PersianCalendar()
        {
            InitializeComponent();
            InitializeMonth();
            initializeYear();
            initializeDecade();
            
            
            this.setCalendar();
        }

        Button newControl()
        {
            var element = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FlowDirection=FlowDirection.RightToLeft,
                Padding = new Thickness(0),
                Style = (Style)this.FindResource("InsideButtonsStyle"),
                Background=Brushes.Transparent,
            };
            return element;
        }
        Button[,] monthModeButtons = new Button[6, 7];
        void InitializeMonth()
        {
            string[] dayOfWeeks = new string[] { "ش", "١ش", "٢ش", "٣ش", "٤ش", "٥ش", "ج" };
            for (int j = 1; j <= 7; j++)
            {
                var element = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    Padding = new Thickness(0),
                    FontWeight=FontWeights.Medium,
                };
                element.Content = dayOfWeeks[j - 1];

                this.monthUniformGrid.Children.Add(element);
            }
            for (int i = 2; i <= 7; i++)
            {
                for (int j = 1; j <= 7; j++)
                {
                    var element = newControl();
                    element.Content = string.Format("{0},{1}", i, j);
                    //element.FontSize = 11d;
                    element.Click += new RoutedEventHandler(monthModeButton_Click);
                    this.monthUniformGrid.Children.Add(element);
                    this.monthModeButtons[i - 2, j - 1] = element;
                }
            }

        }

        void monthModeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (SelectedDate == (PersianDate)button.Tag)
                SelectedDate = ((PersianDate)button.Tag).AddDays(1);
            SelectedDate = (PersianDate)button.Tag;
        }


        Button[,] yearModeButtons = new Button[4, 3];
        void initializeYear()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var element = newControl();
                    element.Content = ((PersianMonth)j + i * 3 + 1).ToString();
                    //element.FontSize = 11d;
                    element.Click += new RoutedEventHandler(yearModeButton_Click);
                    element.Tag = j + i * 3 + 1;
                    this.yearModeButtons[i, j] = element;
                    this.yearUniformGrid.Children.Add(element);

                }
            }
        }
        void yearModeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int month = (int)button.Tag;
            this.DisplayDate = new PersianDate(this.DisplayDate.Year, month, 1);
            this.DisplayMode = CalendarMode.Month;
        }


        Button[] DecadeModeButtons = new Button[12];
        void initializeDecade()
        {
            for (int j = 0; j < 12; j++)
            {
                var element = newControl();
                //element.FontSize = 11d;
                element.Click += new RoutedEventHandler(decadeModeButton_Click);
                element.Tag = j-1;
                this.DecadeModeButtons[j] = element;
                this.decadeUniformGrid.Children.Add(element);

            }
        }

        void decadeModeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Content == null)
                return;
            this.DisplayDate = new PersianDate((int)button.Tag, 1, 1);
            this.DisplayMode = CalendarMode.Year;
        }

        private void selectedDateCheck(PersianDate? oldValue)
        {        
            if (this.DisplayMode == CalendarMode.Month)
            {
                int r, c;
                monthModeDateToRowColumn(this.SelectedDate, out r, out c);
                setMonthModeButtonAppearance(this.monthModeButtons[r, c]);

                if (oldValue != null)
                {
                    monthModeDateToRowColumn(oldValue.Value, out r, out c);
                    setMonthModeButtonAppearance(this.monthModeButtons[r, c]);
                }
            }

        }
        void setMonthModeButtonAppearance(Button button)
        {
            Brush bg = Brushes.Transparent;
            if (button.Tag != null)
            {
                var bdate = (PersianDate)button.Tag;
                if (bdate == PersianDate.Today)
                {
                    bg = this.TodayBackGround;
                }
                if (bdate == this.SelectedDate)
                {
                    bg = this.SelectedDateBackGround;
                }
            }
            button.Background = bg;
        }
        private void todayCheck()
        {
            if (this.DisplayMode == CalendarMode.Month)
            {
                int r, c;
                monthModeDateToRowColumn(PersianDate.Today, out r, out c);
                setMonthModeButtonAppearance(this.monthModeButtons[r, c]);
            }
        }
        private void monthModeDateToRowColumn(PersianDate date, out int r, out int c)
        {
            int year = date.Year;
            int month = date.Month;
            PersianDate firstDay = new PersianDate(year, month, 1);
            int fstCol = 2 + (int)firstDay.PersianDayOfWeek;
            int fstRow = fstCol == 1 ? 2 : 1;
            r = (date.Day + fstCol - 2) / 7 + fstRow;
            c = (date.Day + fstCol - 1) % 7;
            c = c == 0 ? 7 : c;
            c--; r--;
        }

        private void setCalendar()
        {
            switch (this.DisplayMode){
                case CalendarMode.Month:
                    setMonthMode();
                    break;
                case CalendarMode.Year:
                    setYearMode();
                    break;
                case CalendarMode.Decade:
                    setDecadeMode();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The DisplayMode value is not in acceptable range");
            }
        }
        private void setDecadeMode()
        {
            this.monthUniformGrid.Visibility = this.yearUniformGrid.Visibility = Visibility.Collapsed;
            this.decadeUniformGrid.Visibility = Visibility.Visible;
            
            int decade = DisplayDate.Year- DisplayDate.Year % 10;
            for (int i = 0; i < 10; i++)
            {
                int y = i + decade;
                if (y >= DisplayDateStart.Year && y <= DisplayDateEnd.Year)
                {
                    DecadeModeButtons[i + 1].Content = decade + i;
                    DecadeModeButtons[i + 1].Tag = decade + i;
                    DecadeModeButtons[i + 1].IsEnabled = true;

                }
                else
                {
                    DecadeModeButtons[i + 1].Content = "";
                    DecadeModeButtons[i + 1].Tag = null;
                    DecadeModeButtons[i + 1].IsEnabled = false;
                }
            }
            this.titleButton.Content = decade.ToString();
        }
        private void setMonthMode()
        {
            this.decadeUniformGrid.Visibility = this.yearUniformGrid.Visibility = Visibility.Collapsed;
            this.monthUniformGrid.Visibility = Visibility.Visible;
            
            int year = DisplayDate.Year;
            int month = DisplayDate.Month;
            int days = PersianDate.DaysInMonth(year, month);
            PersianDate firstDay = new PersianDate(year, month, 1);
            int fstCol = 2 + (int)firstDay.PersianDayOfWeek;
            int fstRow = fstCol == 1 ? 2 : 1;
            for (int i = 1; i <= 6; i++)
            {
                for (int j = 1; j <= 7; j++)
                {
                    monthModeButtons[i - 1, j - 1].Content = "";
                    monthModeButtons[i - 1, j - 1].IsEnabled = false;
                    monthModeButtons[i - 1, j - 1].Background = Brushes.Transparent;
                }

            }
            for (int d = 1; d <= days; d++)
            {
                PersianDate date=new PersianDate(year, month, d);
                if (date >= DisplayDateStart && date <= DisplayDateEnd)
                {
                    int c, r;
                    r = (d + fstCol - 2) / 7 + fstRow;
                    c = (d + fstCol - 1) % 7;
                    c = c == 0 ? 7 : c;
                    monthModeButtons[r - 1, c - 1].Content = d.ToString();
                    monthModeButtons[r - 1, c - 1].IsEnabled = true;
                    monthModeButtons[r - 1, c - 1].Tag = date;
                }

            }
            
            this.titleButton.Content = ((PersianMonth)month).ToString() + " " + year.ToString();
            this.todayCheck();
            this.selectedDateCheck(null);
        }


        private void setYearMode()
        {
            this.monthUniformGrid.Visibility = this.decadeUniformGrid.Visibility = Visibility.Collapsed;
            this.yearUniformGrid.Visibility = Visibility.Visible;
            
            this.titleButton.Content = this.DisplayDate.Year.ToString();
            
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int month=j + i * 3 + 1;
                    if (new PersianDate(DisplayDate.Year, month, PersianDate.DaysInMonth(DisplayDate.Year, month)) >= DisplayDateStart &&
                        new PersianDate(DisplayDate.Year, month, 1) <= DisplayDateEnd)
                    {
                        yearModeButtons[i, j].Content = ((PersianMonth)month).ToString();
                        yearModeButtons[i, j].IsEnabled = true;
                    }
                    else
                    {
                        yearModeButtons[i, j].Content = "";
                        yearModeButtons[i, j].IsEnabled = false;
                    }
                }
            }
        }



        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            int m = this.DisplayDate.Month;
            int y = this.DisplayDate.Year;
            try
            {
                PersianDate newDisplayDate = DisplayDate;
                if (this.DisplayMode == CalendarMode.Month)
                {
                    if (m == 12)
                        newDisplayDate = new PersianDate(y + 1, 1, 1);
                    else
                        newDisplayDate = new PersianDate(y, m + 1, 1);
                }
                else if (this.DisplayMode == CalendarMode.Year)
                {
                    newDisplayDate = new PersianDate(DisplayDate.Year + 1, 1, 1);
                }
                else if (this.DisplayMode == CalendarMode.Decade)
                {
                    newDisplayDate = new PersianDate(y - y % 10 + 10, 1, 1);
                }

                if (newDisplayDate >= DisplayDateStart && newDisplayDate <= DisplayDateEnd)
                    DisplayDate = newDisplayDate;
            }
            catch (ArgumentOutOfRangeException )
            {
                
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            int m = this.DisplayDate.Month;
            int y = this.DisplayDate.Year;
            try
            {
                PersianDate newDisplayDate = DisplayDate;

                if (this.DisplayMode == CalendarMode.Month)
                {
                    if (m == 1)
                        newDisplayDate = new PersianDate(y - 1, 12, PersianDate.DaysInMonth(y - 1, 12));
                    else
                        newDisplayDate = new PersianDate(y, m - 1, PersianDate.DaysInMonth(y, m - 1));
                }
                else if (this.DisplayMode == CalendarMode.Year)
                {
                    newDisplayDate = new PersianDate(y - 1, 12, PersianDate.DaysInMonth(y - 1, 12));
                }
                else if (this.DisplayMode == CalendarMode.Decade)
                {
                    newDisplayDate = new PersianDate(y - y % 10 - 1, 12, PersianDate.DaysInMonth(y - y % 10 - 1, 12));
                }

                if (newDisplayDate >= DisplayDateStart && newDisplayDate <= DisplayDateEnd)
                    DisplayDate = newDisplayDate;
            }
            catch (ArgumentOutOfRangeException )
            {
            }
        }

        private void titleButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DisplayMode == CalendarMode.Month) 
                this.DisplayMode = CalendarMode.Year;
            else if (this.DisplayMode == CalendarMode.Year) 
                this.DisplayMode = CalendarMode.Decade;
        }
    }
}
