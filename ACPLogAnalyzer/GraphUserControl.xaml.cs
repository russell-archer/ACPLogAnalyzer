using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.IO;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// Interaction logic for GraphUserControl.xaml
    /// </summary>
    public partial class GraphUserControl : UserControl
    {
        public enum SeriesName { Main, Secondary, Third };

        public delegate void DataPointSelected(object sender, DataPointSelectedEventArgs e);
        public event DataPointSelected DataPointSelectionChanged;

        private SolidColorBrush _fgBrush;
        private SolidColorBrush _fgBrushTitle;
        private SolidColorBrush _bgBrushGraph;
        private SolidColorBrush _bgKeyBrush;
        private SolidColorBrush _brushMainLine;
        private SolidColorBrush _brushSecondaryLine;
        private SolidColorBrush _brushThirdLine;

        private List<KeyValuePair<object, object>> _dataSourceSeries1;  // Set by caller to DataSource property 
        public List<KeyValuePair<object, object>> DataSourceSeries1  // string is the x-axis (normally time or date), object is the data to plot
        {
            get
            {
                return _dataSourceSeries1;
            }
            set
            {
                _dataSourceSeries1 = value;
                ((LineSeries)LineGraph.Series[0]).ItemsSource = _dataSourceSeries1;
            }
        }

        private List<KeyValuePair<object, object>> _dataSourceSeries2;  // Set by caller to DataSourceSeries2 property 
        public List<KeyValuePair<object, object>> DataSourceSeries2
        {
            get
            {
                return _dataSourceSeries2;
            }
            set
            {
                _dataSourceSeries2 = value;
                ((LineSeries)LineGraph.Series[1]).ItemsSource = _dataSourceSeries2;
            }
        }

        private List<KeyValuePair<object, object>> _dataSourceSeries3;  // Set by caller to DataSource3 property 
        public List<KeyValuePair<object, object>> DataSourceSeries3  // 
        {
            get
            {
                return _dataSourceSeries3;
            }
            set
            {
                _dataSourceSeries3 = value;
                ((LineSeries)LineGraph.Series[2]).ItemsSource = _dataSourceSeries3;
            }
        }

        public LogEventType LogEventTypeMainSeries { get; set; }
        public LogEventType LogEventTypeTertiarySeries { get; set; }

        private void InitText()
        {
            try
            {
                var fwc = new FontWeightConverter();
                var fsc = new FontStyleConverter();

                if (Properties.Settings.Default.Plot_FG_Color != null)
                {
                    _fgBrush = new SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(
                            Properties.Settings.Default.Plot_FG_Color.R,
                            Properties.Settings.Default.Plot_FG_Color.G,
                            Properties.Settings.Default.Plot_FG_Color.B));
                }

                // Overall graph text properties...
                LineGraph.FontFamily = new System.Windows.Media.FontFamily(Properties.Settings.Default.Plot_Font_Name);
                LineGraph.FontSize = double.Parse(Properties.Settings.Default.Plot_Font_Size);
                LineGraph.FontWeight = (FontWeight)fwc.ConvertFromString(Properties.Settings.Default.Plot_Font_Weight);
                LineGraph.FontStyle = (FontStyle)fsc.ConvertFromString(Properties.Settings.Default.Plot_Font_Style);
                LineGraph.Foreground = _fgBrush;
            }
            catch
            {
            }
        }

        private void InitTextTitle()
        {
            try
            {
                var fwc = new FontWeightConverter();
                var fsc = new FontStyleConverter();

                if (Properties.Settings.Default.Plot_FG_Color_Title != null)
                {
                    _fgBrushTitle = new SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(
                            Properties.Settings.Default.Plot_FG_Color_Title.R,
                            Properties.Settings.Default.Plot_FG_Color_Title.G,
                            Properties.Settings.Default.Plot_FG_Color_Title.B));
                }

                // Title...
                GraphTitle.FontFamily = new System.Windows.Media.FontFamily(Properties.Settings.Default.Plot_Font_Name_Title);
                GraphTitle.FontSize = double.Parse(Properties.Settings.Default.Plot_Font_Size_Title);
                GraphTitle.FontWeight = (FontWeight)fwc.ConvertFromString(Properties.Settings.Default.Plot_Font_Weight_Title);
                GraphTitle.FontStyle = (FontStyle)fsc.ConvertFromString(Properties.Settings.Default.Plot_Font_Style_Title);
                GraphTitle.Foreground = _fgBrushTitle;
            }
            catch
            {
            }
        }

        private void InitGraphStyles()
        {
            if (Properties.Settings.Default.Plot_BG_Graph != null)
            {
                _bgBrushGraph = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_BG_Graph.R,
                        Properties.Settings.Default.Plot_BG_Graph.G,
                        Properties.Settings.Default.Plot_BG_Graph.B));
            }

            LineGraph.Background = _bgBrushGraph;
            GraphTitle.Background = _bgBrushGraph;  // Make sure the bg of the title control is the same as the graph

            if (Properties.Settings.Default.Plot_BG_Key != null)
            {
                _bgKeyBrush = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_BG_Key.R,
                        Properties.Settings.Default.Plot_BG_Key.G,
                        Properties.Settings.Default.Plot_BG_Key.B));
            }

            LineGraph.LegendStyle = LegendStyle();

            if (Properties.Settings.Default.Plot_Main_Line_Color != null)
            {
                _brushMainLine = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_Main_Line_Color.R,
                        Properties.Settings.Default.Plot_Main_Line_Color.G,
                        Properties.Settings.Default.Plot_Main_Line_Color.B));
            }

            if (Properties.Settings.Default.Plot_Secondary_Line_Color != null)
            {
                _brushSecondaryLine = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_Secondary_Line_Color.R,
                        Properties.Settings.Default.Plot_Secondary_Line_Color.G,
                        Properties.Settings.Default.Plot_Secondary_Line_Color.B));
            }

            if (Properties.Settings.Default.Plot_Tertiary_Line_Color != null)
            {
                _brushThirdLine = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_Tertiary_Line_Color.R,
                        Properties.Settings.Default.Plot_Tertiary_Line_Color.G,
                        Properties.Settings.Default.Plot_Tertiary_Line_Color.B));
            }

            LineGraph.LegendStyle = LegendStyle();
            ((LineSeries)LineGraph.Series[0]).PolylineStyle = PolyLineStyle(SeriesName.Main);
            ((LineSeries)LineGraph.Series[1]).PolylineStyle = PolyLineStyle(SeriesName.Secondary);
            ((LineSeries)LineGraph.Series[2]).PolylineStyle = PolyLineStyle(SeriesName.Third);

            ((LineSeries)LineGraph.Series[0]).DataPointStyle = 
                DataPointStyle(SeriesName.Main, Properties.Settings.Default.Plot_Main_Show_Data_Points);
            
            ((LineSeries)LineGraph.Series[1]).DataPointStyle = 
                DataPointStyle(SeriesName.Secondary, Properties.Settings.Default.Plot_Secondary_Show_Data_Points);
            
            ((LineSeries)LineGraph.Series[2]).DataPointStyle = 
                DataPointStyle(SeriesName.Third, Properties.Settings.Default.Plot_Tertiary_Show_Data_Points);

            LineGraph.ChartAreaStyle = ChartAreaStyle();  // Sets the chart area to be transparent
            LineGraph.PlotAreaStyle = PlotAreaStyle();  // Sets the plot area to be transparent
        }

        private void InitSeries1()
        {
            var fwc = new FontWeightConverter();
            var fsc = new FontStyleConverter();

            ((LegendItem)LineGraph.LegendItems[0]).FontFamily = new System.Windows.Media.FontFamily(Properties.Settings.Default.Plot_Font_Name);
            ((LegendItem)LineGraph.LegendItems[0]).FontSize = double.Parse(Properties.Settings.Default.Plot_Font_Size);
            ((LegendItem)LineGraph.LegendItems[0]).FontWeight = (FontWeight)fwc.ConvertFromString(Properties.Settings.Default.Plot_Font_Weight);
            ((LegendItem)LineGraph.LegendItems[0]).FontStyle = (FontStyle)fsc.ConvertFromString(Properties.Settings.Default.Plot_Font_Style);
            ((LegendItem)LineGraph.LegendItems[0]).Foreground = _fgBrush;
        }

        private void InitSeries2()
        {
            var fwc = new FontWeightConverter();
            var fsc = new FontStyleConverter();

            ((LegendItem)LineGraph.LegendItems[1]).FontFamily = new System.Windows.Media.FontFamily(Properties.Settings.Default.Plot_Font_Name);
            ((LegendItem)LineGraph.LegendItems[1]).FontSize = double.Parse(Properties.Settings.Default.Plot_Font_Size);
            ((LegendItem)LineGraph.LegendItems[1]).FontWeight = (FontWeight)fwc.ConvertFromString(Properties.Settings.Default.Plot_Font_Weight);
            ((LegendItem)LineGraph.LegendItems[1]).FontStyle = (FontStyle)fsc.ConvertFromString(Properties.Settings.Default.Plot_Font_Style);
            ((LegendItem)LineGraph.LegendItems[1]).Foreground = _fgBrush;

            ((LegendItem)LineGraph.LegendItems[1]).Visibility = System.Windows.Visibility.Visible;
            ((LineSeries)LineGraph.Series[1]).Visibility = System.Windows.Visibility.Visible;
        }

        private void InitSeries3()
        {
            var fwc = new FontWeightConverter();
            var fsc = new FontStyleConverter();

            ((LegendItem)LineGraph.LegendItems[2]).FontFamily = new System.Windows.Media.FontFamily(Properties.Settings.Default.Plot_Font_Name);
            ((LegendItem)LineGraph.LegendItems[2]).FontSize = double.Parse(Properties.Settings.Default.Plot_Font_Size);
            ((LegendItem)LineGraph.LegendItems[2]).FontWeight = (FontWeight)fwc.ConvertFromString(Properties.Settings.Default.Plot_Font_Weight);
            ((LegendItem)LineGraph.LegendItems[2]).FontStyle = (FontStyle)fsc.ConvertFromString(Properties.Settings.Default.Plot_Font_Style);
            ((LegendItem)LineGraph.LegendItems[2]).Foreground = _fgBrush;

            ((LegendItem)LineGraph.LegendItems[2]).Visibility = System.Windows.Visibility.Visible;
            ((LineSeries)LineGraph.Series[2]).Visibility = System.Windows.Visibility.Visible;
        }

        private Style PolyLineStyle(SeriesName series)
        {
            /* XAML equiv:
             
                <Window.Resources>
                    <Style x:Key="MainPolylineStyle" TargetType="Polyline">
                        <Setter Property="StrokeThickness" Value="5"/>
                    </Style>
                </Window.Resources>
             
             */

            double thickness = 0;
            
            switch (series)
            {
                case SeriesName.Main:
                    thickness = Properties.Settings.Default.Plot_Main_Thickness;
                    break;
                case SeriesName.Secondary:
                    thickness = Properties.Settings.Default.Plot_Secondary_Thickness;
                    break;
                case SeriesName.Third:
                    thickness = Properties.Settings.Default.Plot_Tertiary_Thickness;
                    break;
            }

            var style = new Style(typeof(Polyline));
            style.Setters.Add(new Setter(Polyline.StrokeThicknessProperty, thickness));

            return style;
        }

        private Style DataPointStyle(SeriesName series, bool showDataPoints)
        {
            /* XAML equiv:
             
                <Window.Resources>
                    <Style x:Key="MainLineDataPointStyle" TargetType="DVC:LineDataPoint">
                    <Setter Property="Background" Value="Red" />
                    <Setter Property="BorderBrush" Value="White"/>
                    <Setter Property="BorderThickness" Value="1"/>
                    <Setter Property="IsTabStop" Value="False"/>
                    </Style>
                </Window.Resources>
             
             */

            var style = new Style(typeof(DataPoint));

            switch (series)
            {
                case SeriesName.Main:
                    style.Setters.Add(new Setter(DataPoint.BackgroundProperty, _brushMainLine));
                    style.Setters.Add(new Setter(DataPoint.BorderBrushProperty, _brushMainLine));
                    break;
                case SeriesName.Secondary:
                    style.Setters.Add(new Setter(DataPoint.BackgroundProperty, _brushSecondaryLine));
                    style.Setters.Add(new Setter(DataPoint.BorderBrushProperty, _brushSecondaryLine));
                    break;
                case SeriesName.Third:
                    style.Setters.Add(new Setter(DataPoint.BackgroundProperty, _brushThirdLine));
                    style.Setters.Add(new Setter(DataPoint.BorderBrushProperty, _brushThirdLine));
                    break;
            }

            style.Setters.Add(new Setter(DataPoint.BorderThicknessProperty, new Thickness(1)));

            if (!showDataPoints)
                style.Setters.Add(new Setter(DataPoint.TemplateProperty, null));  // Nulling DataPoint.TemplateProperty turns OFF the data points

            return style;
        }

        private Style LegendStyle()
        {
            /* XAML equiv:
             
                <DVC:Chart>
                    <DVC:Chart.LegendStyle>
                        <Style TargetType="VIZ:Legend">
                            <Setter Property="BorderBrush" Value="Transparent"/>
                            <Setter Property="Background" Value="Transparent"/>
                            <Setter Property="Height" Value="100" />
                        </Style>
                    </DVC:Chart.LegendStyle>
                    :
                    :
                </DVC:Chart>
             
             */

            Style style = null;
            try
            {
                style = new Style(typeof(Legend));
                style.Setters.Add(new Setter(Legend.BackgroundProperty, _bgKeyBrush));
                style.Setters.Add(new Setter(Legend.BorderBrushProperty, _bgKeyBrush));
                style.Setters.Add(new Setter(Legend.HeightProperty, 100.0));

                if (Properties.Settings.Default.Plot_BG_Key_Transparent)
                    _bgKeyBrush.Opacity = 0;
            }
            catch
            {
            }
            return style;
        }

        private Style ChartAreaStyle()
        {
            // Here we make the chart area TRANSPARENT, so the graph bg color setting will apply to the chart/plot area too

            // This style works for both the plot area (where the data is plotted) and chart area (the large border around the graph)
            /*
                The style can also be set in XAML:
                
                <DVC:Chart>
                    <DVC:Chart.PlotAreaStyle>
                        <Style TargetType="Panel" x:Name="plotAreaStyle">
                            <Setter Property="Background" Value="Transparent"/>
                        </Style>
                    </DVC:Chart.PlotAreaStyle>
            
                    <DVC:Chart.ChartAreaStyle>
                        <Style TargetType="Panel">
                            <Setter Property="Background" Value="Transparent" />
                        </Style>
                    </DVC:Chart.ChartAreaStyle>
                :
                :
                </DVC:Chart>
            */

            var background = Color.FromRgb(0, 0, 0);
            var brush = new SolidColorBrush(background) {Opacity = 0};

            var style = new Style(typeof(Panel));
            style.Setters.Add(new Setter(Panel.BackgroundProperty, brush));
            
            return style;
        }

        private Style PlotAreaStyle()
        {
            return ChartAreaStyle();
        }

        public GraphUserControl() : this(LogEventType.None, LogEventType.None)
        {
        }

        public GraphUserControl(LogEventType mainSeries, LogEventType tertiarySeries)
        {
            InitializeComponent();
            UpdateStyles();

            LogEventTypeMainSeries = mainSeries;
            LogEventTypeTertiarySeries = tertiarySeries;
        }

        public void RefreshData()
        {
            ((LineSeries)LineGraph.Series[0]).Refresh();
            ((LineSeries)LineGraph.Series[1]).Refresh();
            ((LineSeries)LineGraph.Series[2]).Refresh();
        }

        public void UpdateStyles()
        {
            InitText();
            InitTextTitle();
            InitGraphStyles();
            InitSeries1();
            InitSeries2();
            InitSeries3();
        }

        public void ShowSecondarySeries(bool show)
        {
            if (show)
            {
                ((LegendItem)LineGraph.LegendItems[1]).Visibility = System.Windows.Visibility.Visible;
                ((LineSeries)LineGraph.Series[1]).Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ((LegendItem)LineGraph.LegendItems[1]).Visibility = System.Windows.Visibility.Hidden;
                ((LineSeries)LineGraph.Series[1]).Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public void ShowThirdSeries(bool show)
        {
            if (show)
            {
                ((LegendItem)LineGraph.LegendItems[2]).Visibility = System.Windows.Visibility.Visible;
                ((LineSeries)LineGraph.Series[2]).Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ((LegendItem)LineGraph.LegendItems[2]).Visibility = System.Windows.Visibility.Hidden;
                ((LineSeries)LineGraph.Series[2]).Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void MenuItemExportExcelClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var excel = new Microsoft.Office.Interop.Excel.Application {Visible = true};  // Create an instance of Excel
                excel.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);  // Create a new, blank sheet

                excel.ActiveCell.set_Item(1, 1, XAxis.Title.ToString());  // Add data to R1C1
                excel.ActiveCell.set_Item(1, 2, LineSeries.Title.ToString());  // Add data to R1C2

                var rowIndex = 2;
                foreach (var kvp in _dataSourceSeries1)
                {
                    excel.ActiveCell.set_Item(rowIndex, 1, kvp.Key);
                    excel.ActiveCell.set_Item(rowIndex, 2, kvp.Value.ToString());
                    rowIndex++;
                }

                if (_dataSourceSeries3 == null || _dataSourceSeries3.Count <= 0) return;
                
                rowIndex++;
                excel.ActiveCell.set_Item(rowIndex, 1, XAxisTertiary.Title.ToString());
                excel.ActiveCell.set_Item(rowIndex, 2, LineSeries3.Title.ToString());
                rowIndex++;

                foreach (var kvp in _dataSourceSeries3)
                {
                    excel.ActiveCell.set_Item(rowIndex, 1, kvp.Key);
                    excel.ActiveCell.set_Item(rowIndex, 2, kvp.Value.ToString());
                    rowIndex++;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to export data to Excel:\n\n" + ex.Message, "Excel Interop Error");
            }
        }

        private void MenuItemExportCsvClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var timestamp = DateTime.Now.ToLongTimeString();
                timestamp = timestamp.Replace(":", "");
                
                var title = GraphTitle.Text.Trim().Replace(" ", "_");

                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                    @"\ACPLogAnalyzer-Data-" + title + "-" + timestamp + ".csv";

                using (var outfile = new StreamWriter(path))
                {
                    outfile.WriteLine(XAxis.Title.ToString() + "," + LineSeries.Title.ToString());
                    foreach (var kvp in _dataSourceSeries1)
                    {
                        outfile.WriteLine(kvp.Key + "," + kvp.Value.ToString());
                    }

                    if (_dataSourceSeries3 != null && _dataSourceSeries3.Count > 0)
                    {
                        outfile.WriteLine("");
                        outfile.WriteLine(XAxisTertiary.Title.ToString() + "," +  LineSeries3.Title.ToString());
                        foreach (var kvp in _dataSourceSeries3)
                        {
                            outfile.WriteLine(kvp.Key + "," + kvp.Value.ToString());
                        }
                    }
                }

                MessageBox.Show("Report saved on your Desktop as: \n\nACPLogAnalyzer-Export-" + title + "-" + timestamp + ".csv", "Data Exported");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving Report");
            }
        }

        private void MenuItemShowHideAverageClick(object sender, RoutedEventArgs e)
        {
            if (CtxMenuShowHideAvg.Header.ToString().StartsWith("Hide"))
            {
                CtxMenuShowHideAvg.Header = "Show " + LineSeries2.Title;
                ShowSecondarySeries(false);
            }
            else
            {
                CtxMenuShowHideAvg.Header = "Hide " + LineSeries2.Title;
                ShowSecondarySeries(true);
            }
        }

        private void MenuItemShowHideTertiaryClick(object sender, RoutedEventArgs e)
        {
            if (CtxMenuShowHideTertiary.Header.ToString().StartsWith("Hide"))
            {
                CtxMenuShowHideTertiary.Header = "Show " + LineSeries3.Title;
                ShowThirdSeries(false);
            }
            else
            {
                CtxMenuShowHideTertiary.Header = "Hide " + LineSeries3.Title;
                ShowThirdSeries(true);
            }
        }

        private void GridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Do we need to hide the Show/Hide 'Tertiary' command?
            if (DataSourceSeries3 == null || DataSourceSeries3.Count == 0)
                CtxMenuShowHideTertiary.Visibility = System.Windows.Visibility.Collapsed;
            else
                CtxMenuShowHideTertiary.Visibility = System.Windows.Visibility.Visible;

            // Customize the menu text...
            if (CtxMenuShowHideTertiary.Header.ToString().StartsWith("Hide"))
                CtxMenuShowHideTertiary.Header = "Hide " + LineSeries3.Title;
            else
                CtxMenuShowHideTertiary.Header = "Show " + LineSeries3.Title;

            if (CtxMenuShowHideAvg.Header.ToString().StartsWith("Hide"))
                CtxMenuShowHideAvg.Header = "Hide " + LineSeries2.Title;
            else
                CtxMenuShowHideAvg.Header = "Show " + LineSeries2.Title;
        }

        private void LineSeries1MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dp = GetDataPointAtMousePosition(
                LineSeries, e.GetPosition(LineSeries), _dataSourceSeries1);

            if(dp.Key != null && dp.Value != null)
                OnDataPointSelected(dp, LogEventTypeMainSeries);  // Raise the DataPointSelected event
        }

        private void LineSeries3MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dp = GetDataPointAtMousePosition(
                LineSeries3, e.GetPosition(LineSeries3), _dataSourceSeries3);

            if (dp.Key != null && dp.Value != null)
                OnDataPointSelected(dp, LogEventTypeTertiarySeries);  // Raise the DataPointSelected event
        }

        private KeyValuePair<object, object> GetDataPointAtMousePosition(
            LineSeries series, Point position, List<KeyValuePair<object, object>>dataPointList)
        {
            var dataPointIndex = FindNearestPointIndex(series.Points, position);
            
            return dataPointIndex == null ? new KeyValuePair<object, object>(null, null) : dataPointList[(int)dataPointIndex];
        }

        private int? FindNearestPointIndex(PointCollection points, Point newPoint)
        {
            if (points == null || !points.Any())
                return null;

            Func<Point, Point, double> getLength = (p1, p2) => Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            var items = points.Select((p, i) => new { Point = p, Length = getLength(p, newPoint), Index = i });
            var minLength = items.Min(item => item.Length);

            return items.First(item => item.Length == minLength).Index;
        }

        protected void OnDataPointSelected(KeyValuePair<object, object> data, LogEventType logEventType)
        {
            if (DataPointSelectionChanged != null)
                DataPointSelectionChanged(this, new DataPointSelectedEventArgs(data, logEventType));
        }

    }

    /// <summary>
    /// Custom event args class
    /// </summary>
    public class DataPointSelectedEventArgs : EventArgs
    {
        public KeyValuePair<object, object> Data { get; set; }
        public LogEventType EventType { get; set; }

        public DataPointSelectedEventArgs() : this(new KeyValuePair<object, object>(null, null), LogEventType.None)
        {
        }

        public DataPointSelectedEventArgs(KeyValuePair<object, object> data, LogEventType logEventType)
        {
            Data = data;
            EventType = logEventType;
        }
    }
}
