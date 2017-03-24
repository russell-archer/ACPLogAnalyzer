using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ACPLogAnalyzer
{
    /// <summary>
    /// Interaction logic for ConfigPlot.xaml
    /// </summary>
    public partial class ConfigPlot : Window
    {
        public bool RestartConfig;
        public int RestartTab;

        private bool _init;

        private List<KeyValuePair<object, object>> _grDataSeries1;
        private List<KeyValuePair<object, object>> _grDataSeries2;
        private List<KeyValuePair<object, object>> _grDataSeries3;

        private SolidColorBrush _fgBrush;
        private SolidColorBrush _fgBrushTitle;
        private SolidColorBrush _bgBrushGraphWnd;
        private SolidColorBrush _bgBrushKey;
        private SolidColorBrush _brushMainLine;
        private SolidColorBrush _brushSecondaryLine;
        private SolidColorBrush _brushTertiaryLine;

        private FontFamily _fontFamily;
        private FontFamily _fontFamilyTitle;

        private string _fontName;
        private string _fontNameTitle;

        public double PlotFontSize { get; set; }
        public double PlotFontSizeTitle { get; set; }

        public FontWeight PlotFontWeight { get; set; }
        public FontWeight PlotFontWeightTitle { get; set; }
        public FontStyle PlotFontStyle { get; set; }
        public FontStyle PlotFontStyleTitle { get; set; }

        public ConfigPlot()
        {
            _init = false;
            InitializeComponent();
        }

        private void InitText()
        {
            var fwc = new FontWeightConverter();
            var fsc = new FontStyleConverter();

            FontSelector.ItemsSource = Fonts.SystemFontFamilies;
            var fontIndex = -1;

            // Font name...
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plot_Font_Name))
            {
                var g = new Graph(null, null);
                _fontName = g.FontFamily.Source;
                Properties.Settings.Default.Plot_Font_Name = _fontName;
            }
            else
                _fontName = Properties.Settings.Default.Plot_Font_Name;

            // Font size...
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plot_Font_Size))
                PlotFontSize = 12;
            else
            {
                try
                {
                    PlotFontSize = double.Parse(Properties.Settings.Default.Plot_Font_Size);
                }
                catch
                {
                }
            }

            // Font weight...
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plot_Font_Weight))
                PlotFontWeight = (FontWeight)fwc.ConvertFromString("Normal");
            else
                PlotFontWeight = (FontWeight)fwc.ConvertFromString(Properties.Settings.Default.Plot_Font_Weight);

            // Font style...
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plot_Font_Style))
                PlotFontStyle = (FontStyle)fsc.ConvertFromString("Normal");
            else
                PlotFontStyle = (FontStyle)fsc.ConvertFromString(Properties.Settings.Default.Plot_Font_Style);

            foreach (System.Windows.Media.FontFamily ff in FontSelector.Items)
            {
                fontIndex++;
                if (System.String.Compare(ff.Source, _fontName, System.StringComparison.Ordinal) == 0)
                {
                    _fontFamily = ff;
                    break;
                }
            }

            // Font FG color
            if (Properties.Settings.Default.Plot_FG_Color != null)
            {
                _fgBrush = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_FG_Color.R,
                        Properties.Settings.Default.Plot_FG_Color.G,
                        Properties.Settings.Default.Plot_FG_Color.B));
            }

            // Init the controls
            FontSelector.SelectedIndex = fontIndex;
            ComboBoxStyle.SelectedValue = fsc.ConvertToString(PlotFontStyle);
            ComboBoxWeight.SelectedValue = fwc.ConvertToString(PlotFontWeight);
            SliderFontSize.DataContext = this;
        }

        private void InitTextTitle()
        {
            var fwc = new FontWeightConverter();
            var fsc = new FontStyleConverter();

            FontSelectorTitle.ItemsSource = Fonts.SystemFontFamilies;
            var fontIndex = -1;

            // Font name...
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plot_Font_Name_Title))
            {
                var g = new Graph(null, null);
                _fontNameTitle = g.FontFamily.Source;
                Properties.Settings.Default.Plot_Font_Name_Title = _fontNameTitle;
            }
            else
                _fontNameTitle = Properties.Settings.Default.Plot_Font_Name_Title;

            // Font size...
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plot_Font_Size_Title))
                PlotFontSizeTitle = 12;
            else
            {
                try
                {
                    PlotFontSizeTitle = double.Parse(Properties.Settings.Default.Plot_Font_Size_Title);
                }
                catch
                {
                }
            }

            // Font weight...
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plot_Font_Weight_Title))
                PlotFontWeightTitle = (FontWeight)fwc.ConvertFromString("Normal");
            else
                PlotFontWeightTitle = (FontWeight)fwc.ConvertFromString(Properties.Settings.Default.Plot_Font_Weight_Title);

            // Font style...
            if (string.IsNullOrEmpty(Properties.Settings.Default.Plot_Font_Style_Title))
                PlotFontStyleTitle = (FontStyle)fsc.ConvertFromString("Normal");
            else
                PlotFontStyleTitle = (FontStyle)fsc.ConvertFromString(Properties.Settings.Default.Plot_Font_Style_Title);

            foreach (System.Windows.Media.FontFamily ff in FontSelectorTitle.Items)
            {
                fontIndex++;
                if (System.String.Compare(ff.Source, _fontNameTitle, System.StringComparison.Ordinal) == 0)
                {
                    _fontFamilyTitle = ff;
                    break;
                }
            }

            // Font FG color
            if (Properties.Settings.Default.Plot_FG_Color_Title != null)
            {
                _fgBrushTitle = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_FG_Color_Title.R,
                        Properties.Settings.Default.Plot_FG_Color_Title.G,
                        Properties.Settings.Default.Plot_FG_Color_Title.B));
            }

            // Init the controls
            FontSelectorTitle.SelectedIndex = fontIndex;
            ComboBoxStyleTitle.SelectedValue = fsc.ConvertToString(PlotFontStyleTitle);
            ComboBoxWeightTitle.SelectedValue = fwc.ConvertToString(PlotFontWeightTitle);
            SliderFontSizeTitle.DataContext = this;
        }

        private void InitGraphColors()
        {
            // Graph BG color
            if (Properties.Settings.Default.Plot_BG_Graph != null)
            {
                _bgBrushGraphWnd = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_BG_Graph.R,
                        Properties.Settings.Default.Plot_BG_Graph.G,
                        Properties.Settings.Default.Plot_BG_Graph.B));
            }

            // Graph Key BG color
            if (Properties.Settings.Default.Plot_BG_Key != null)
            {
                _bgBrushKey = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_BG_Key.R,
                        Properties.Settings.Default.Plot_BG_Key.G,
                        Properties.Settings.Default.Plot_BG_Key.B));
            }

            // Graph Key transparency
            CheckBoxKeyTransparent.IsChecked = Properties.Settings.Default.Plot_BG_Key_Transparent;
        }

        private void InitSeriesStyles()
        {
            TextBoxPrimaryThickness.Text = Properties.Settings.Default.Plot_Main_Thickness.ToString(CultureInfo.InvariantCulture);
            TextBoxSecondaryThickness.Text = Properties.Settings.Default.Plot_Secondary_Thickness.ToString(CultureInfo.InvariantCulture);
            TextBoxTertiaryThickness.Text = Properties.Settings.Default.Plot_Tertiary_Thickness.ToString(CultureInfo.InvariantCulture);

            CheckBoxShowPointsMain.IsChecked = Properties.Settings.Default.Plot_Main_Show_Data_Points;
            CheckBoxSecondaryShowDataPoints.IsChecked = Properties.Settings.Default.Plot_Secondary_Show_Data_Points;
            CheckBoxTertiaryShowDataPoints.IsChecked = Properties.Settings.Default.Plot_Tertiary_Show_Data_Points;

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
                _brushTertiaryLine = new SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        Properties.Settings.Default.Plot_Tertiary_Line_Color.R,
                        Properties.Settings.Default.Plot_Tertiary_Line_Color.G,
                        Properties.Settings.Default.Plot_Tertiary_Line_Color.B));
            }
        }

        private void InitUiControls()
        {
            // Graph text controls...
            TextBlockExample.FontFamily = _fontFamily;
            TextBlockExample.FontSize = PlotFontSize;
            TextBlockExample.FontWeight = PlotFontWeight;
            TextBlockExample.FontStyle = PlotFontStyle;
            TextBlockExample.Foreground = _fgBrush;
            LabelFgColor.Background = _fgBrush;

            // Graph title controls...
            TextBlockExampleTitle.FontFamily = _fontFamilyTitle;
            TextBlockExampleTitle.FontSize = PlotFontSizeTitle;
            TextBlockExampleTitle.FontWeight = PlotFontWeightTitle;
            TextBlockExampleTitle.FontStyle = PlotFontStyleTitle;
            TextBlockExampleTitle.Foreground = _fgBrushTitle;
            LabelFgColorTitle.Background = _fgBrushTitle;

            // Graph bg color control...
            LabelGraphWndBg.Background = _bgBrushGraphWnd;

            // Graph key bg color control...
            LabelKeyBg.Background = _bgBrushKey;

            // Data series line colors
            LabelPrimaryColor.Background = _brushMainLine;
            LabelSecondaryColor.Background = _brushSecondaryLine;
            LabelTertiaryColor.Background = _brushTertiaryLine;
        }

        private void InitGraphData()
        {
            _grDataSeries1 = new List<KeyValuePair<object, object>>();
            _grDataSeries2 = new List<KeyValuePair<object, object>>();
            _grDataSeries3 = new List<KeyValuePair<object, object>>();

            GraphUc.DataSourceSeries1 = _grDataSeries1;
            GraphUc.DataSourceSeries2 = _grDataSeries2;
            GraphUc.DataSourceSeries3 = _grDataSeries3;

            _grDataSeries1.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 10, 0).TimeOfDay, 5.1));
            _grDataSeries1.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 20, 0).TimeOfDay, 3.2));
            _grDataSeries1.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 30, 0).TimeOfDay, 8.4));
            _grDataSeries1.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 40, 0).TimeOfDay, 9.5));
            _grDataSeries1.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 50, 0).TimeOfDay, 2.2));

            _grDataSeries2.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 10, 0).TimeOfDay, 5.1));
            _grDataSeries2.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 20, 0).TimeOfDay, 5.1));
            _grDataSeries2.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 30, 0).TimeOfDay, 5.1));
            _grDataSeries2.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 40, 0).TimeOfDay, 5.1));
            _grDataSeries2.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 50, 0).TimeOfDay, 5.1));

            _grDataSeries3.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 05, 0).TimeOfDay, 3.2));
            _grDataSeries3.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 15, 0).TimeOfDay, 3.7));
            _grDataSeries3.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 25, 0).TimeOfDay, 4.4));
            _grDataSeries3.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 35, 0).TimeOfDay, 6.5));
            _grDataSeries3.Add(new KeyValuePair<object, object>(new DateTime(2011, 11, 22, 1, 45, 0).TimeOfDay, 1.6));

            GraphUc.ShowSecondarySeries(true);
            GraphUc.RefreshData();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (RestartConfig)
                TabControl1.SelectedIndex = RestartTab;

            RestartConfig = false;
            RestartTab = 0;

            InitText();
            InitTextTitle();
            InitGraphColors();
            InitSeriesStyles();
            InitGraphData();
            GraphUc.UpdateStyles();
            InitUiControls();

            _init = true;
        }

        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void FontSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _fontFamily = (FontFamily)FontSelector.SelectedValue;
            TextBlockExample.FontFamily = _fontFamily;
            Properties.Settings.Default.Plot_Font_Name = _fontFamily.Source;
            GraphUc.UpdateStyles();
        }

        private void FontSelectorTitleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _fontFamilyTitle = (FontFamily)FontSelectorTitle.SelectedValue;
            TextBlockExampleTitle.FontFamily = _fontFamilyTitle;
            Properties.Settings.Default.Plot_Font_Name_Title = _fontFamilyTitle.Source;
            GraphUc.UpdateStyles();
        }

        private void SliderFontSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_init || e.NewValue < 5)  
                return;

            TextBlockExample.FontSize = e.NewValue;
            Properties.Settings.Default.Plot_Font_Size = e.NewValue.ToString(CultureInfo.InvariantCulture);
            GraphUc.UpdateStyles();
        }

        private void SliderFontSizeTitleValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_init || e.NewValue < 5)
                return;

            TextBlockExampleTitle.FontSize = e.NewValue;
            Properties.Settings.Default.Plot_Font_Size_Title = e.NewValue.ToString(CultureInfo.InvariantCulture);
            GraphUc.UpdateStyles();
        }

        private void ComboBoxWeightSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxWeight.SelectedValue == null)
                return;

            var fontWeight = (string)ComboBoxWeight.SelectedValue;
            var fwc = new FontWeightConverter();
            PlotFontWeight = (FontWeight)fwc.ConvertFromString(fontWeight);
            TextBlockExample.FontWeight = PlotFontWeight;
            Properties.Settings.Default.Plot_Font_Weight = fontWeight;
            GraphUc.UpdateStyles();
        }

        private void ComboBoxWeightTitleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxWeightTitle.SelectedValue == null)
                return;

            var fontWeight = (string)ComboBoxWeightTitle.SelectedValue;
            var fwc = new FontWeightConverter();
            PlotFontWeightTitle = (FontWeight)fwc.ConvertFromString(fontWeight);
            TextBlockExampleTitle.FontWeight = PlotFontWeightTitle;
            Properties.Settings.Default.Plot_Font_Weight_Title = fontWeight;
            GraphUc.UpdateStyles();
        }

        private void ComboBoxStyleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxStyle.SelectedValue == null)
                return;

            var fontStyle = (string)ComboBoxStyle.SelectedValue;
            var fsc = new FontStyleConverter();
            PlotFontStyle = (FontStyle)fsc.ConvertFromString(fontStyle);
            TextBlockExample.FontStyle = PlotFontStyle;
            Properties.Settings.Default.Plot_Font_Style = fontStyle;
            GraphUc.UpdateStyles();
        }

        private void ComboBoxStyleTitleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxStyleTitle.SelectedValue == null)
                return;

            var fontStyle = (string)ComboBoxStyleTitle.SelectedValue;
            var fsc = new FontStyleConverter();
            PlotFontStyleTitle = (FontStyle)fsc.ConvertFromString(fontStyle);
            TextBlockExampleTitle.FontStyle = PlotFontStyleTitle;
            Properties.Settings.Default.Plot_Font_Style_Title = fontStyle;
            GraphUc.UpdateStyles();
        }

        private void LabelFgColorMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (var cd = new System.Windows.Forms.ColorDialog())
            { 
                cd.AllowFullOpen = true;
                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _fgBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
                    Properties.Settings.Default.Plot_FG_Color = System.Drawing.Color.FromArgb(cd.Color.R, cd.Color.G, cd.Color.B);
                    LabelFgColor.Background = _fgBrush;
                    TextBlockExample.Foreground = _fgBrush;
                    GraphUc.UpdateStyles();
                }
            }
        }

        private void LabelFgColorTitleMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (var cd = new System.Windows.Forms.ColorDialog())
            {
                cd.AllowFullOpen = true;
                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _fgBrushTitle = new SolidColorBrush(System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
                    Properties.Settings.Default.Plot_FG_Color_Title = System.Drawing.Color.FromArgb(cd.Color.R, cd.Color.G, cd.Color.B);
                    LabelFgColorTitle.Background = _fgBrushTitle;
                    TextBlockExampleTitle.Foreground = _fgBrushTitle;
                    GraphUc.UpdateStyles();
                }
            }
        }

        private void LabelGraphWndBgMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (var cd = new System.Windows.Forms.ColorDialog())
            {
                cd.AllowFullOpen = true;
                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _bgBrushGraphWnd = new SolidColorBrush(System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
                    Properties.Settings.Default.Plot_BG_Graph = System.Drawing.Color.FromArgb(cd.Color.R, cd.Color.G, cd.Color.B);
                    LabelGraphWndBg.Background = _bgBrushGraphWnd;
                    GraphUc.UpdateStyles();
                }
            }
        }

        private void LabelKeyBgMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (var cd = new System.Windows.Forms.ColorDialog())
            {
                cd.AllowFullOpen = true;
                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _bgBrushKey = new SolidColorBrush(System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
                    Properties.Settings.Default.Plot_BG_Key = System.Drawing.Color.FromArgb(cd.Color.R, cd.Color.G, cd.Color.B);
                    LabelKeyBg.Background = _bgBrushKey;
                    GraphUc.UpdateStyles();
                }
            }
        }

        private void CheckBoxKeyTransparentChecked(object sender, RoutedEventArgs e)
        {
            if (!_init)
                return;

            Properties.Settings.Default.Plot_BG_Key_Transparent = true;
            GraphUc.UpdateStyles();
        }

        private void CheckBoxKeyTransparentUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_init)
                return;

            Properties.Settings.Default.Plot_BG_Key_Transparent = false;
            GraphUc.UpdateStyles();
        }

        private void LabelPrimaryColorMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (var cd = new System.Windows.Forms.ColorDialog())
            {
                cd.AllowFullOpen = true;
                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _brushMainLine = new SolidColorBrush(System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
                    Properties.Settings.Default.Plot_Main_Line_Color = System.Drawing.Color.FromArgb(cd.Color.R, cd.Color.G, cd.Color.B);
                    LabelPrimaryColor.Background = _brushMainLine;
                    RestartConfig = true;
                    RestartTab = 3;  // Hack: Can't get the example graph line colors to update unless we destroy the wnd and re-open
                    this.Close();
                }
            }
        }

        private void LabelSecondaryColorMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (var cd = new System.Windows.Forms.ColorDialog())
            {
                cd.AllowFullOpen = true;
                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _brushSecondaryLine = new SolidColorBrush(System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
                    Properties.Settings.Default.Plot_Secondary_Line_Color = System.Drawing.Color.FromArgb(cd.Color.R, cd.Color.G, cd.Color.B);
                    LabelSecondaryColor.Background = _brushSecondaryLine;
                    RestartConfig = true;
                    RestartTab = 4;  // Hack: Can't get the example graph line colors to update unless we destroy the wnd and re-open
                    this.Close();
                }
            }
        }

        private void LabelTertiaryColorMouseUp(object sender, MouseButtonEventArgs e)
        {
            using (var cd = new System.Windows.Forms.ColorDialog())
            {
                cd.AllowFullOpen = true;
                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _brushTertiaryLine = new SolidColorBrush(System.Windows.Media.Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B));
                    Properties.Settings.Default.Plot_Tertiary_Line_Color = System.Drawing.Color.FromArgb(cd.Color.R, cd.Color.G, cd.Color.B);
                    LabelTertiaryColor.Background = _brushTertiaryLine;
                    RestartConfig = true;
                    RestartTab = 5;  // Hack: Can't get the example graph line colors to update unless we destroy the wnd and re-open
                    this.Close();
                }
            }
        }

        private void TextBoxPrimaryThicknessTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_init)
                return;

            try
            {
                var d = double.Parse(TextBoxPrimaryThickness.Text);
                Properties.Settings.Default.Plot_Main_Thickness = d;
                GraphUc.UpdateStyles();
            }
            catch
            {
            }
        }

        private void TextBoxSecondaryThicknessTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_init)
                return;

            try
            {
                var d = double.Parse(TextBoxSecondaryThickness.Text);
                Properties.Settings.Default.Plot_Secondary_Thickness = d;
                GraphUc.UpdateStyles();
            }
            catch
            {
            }
        }

        private void TextBoxTertiaryThicknessTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_init)
                return;

            try
            {
                var d = double.Parse(TextBoxTertiaryThickness.Text);
                Properties.Settings.Default.Plot_Tertiary_Thickness = d;
                GraphUc.UpdateStyles();
            }
            catch
            {
            }
        }

        private void CheckBoxShowPointsMainChecked(object sender, RoutedEventArgs e)
        {
            if (!_init)
                return;

            Properties.Settings.Default.Plot_Main_Show_Data_Points = true;
            RestartConfig = true;
            RestartTab = 3;  // Hack: Can't get the example graph line datapoints to update unless we destroy the wnd and re-open
            this.Close();
        }

        private void CheckBoxShowPointsMainUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_init)
                return;

            Properties.Settings.Default.Plot_Main_Show_Data_Points = false;
            RestartConfig = true;
            RestartTab = 3;  // Hack: Can't get the example graph line datapoints to update unless we destroy the wnd and re-open
            this.Close();
        }

        private void CheckBoxSecondaryShowDataPointsChecked(object sender, RoutedEventArgs e)
        {
            if (!_init)
                return;

            Properties.Settings.Default.Plot_Secondary_Show_Data_Points = true;
            RestartConfig = true;
            RestartTab = 4;  // Hack: Can't get the example graph line datapoints to update unless we destroy the wnd and re-open
            this.Close();
        }

        private void CheckBoxSecondaryShowDataPointsUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_init)
                return;

            Properties.Settings.Default.Plot_Secondary_Show_Data_Points = false;
            RestartConfig = true;
            RestartTab = 4;  // Hack: Can't get the example graph line datapoints to update unless we destroy the wnd and re-open
            this.Close();
        }

        private void CheckBoxTertiaryShowDataPointsChecked(object sender, RoutedEventArgs e)
        {
            if (!_init)
                return;

            Properties.Settings.Default.Plot_Tertiary_Show_Data_Points = true;
            RestartConfig = true;
            RestartTab = 5;  // Hack: Can't get the example graph line datapoints to update unless we destroy the wnd and re-open
            this.Close();
        }

        private void CheckBoxTertiaryShowDataPointsUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_init)
                return;

            Properties.Settings.Default.Plot_Tertiary_Show_Data_Points = false;
            RestartConfig = true;
            RestartTab = 5;  // Hack: Can't get the example graph line datapoints to update unless we destroy the wnd and re-open
            this.Close();
        }

    }
}
