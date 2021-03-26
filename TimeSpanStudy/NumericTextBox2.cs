using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace TimeSpanStudy
{



    class NumericTextBox2 : Border
{
    //値変化のイベントハンドラ
    public delegate void ValueChangedHandler(object sender, int value);
    public event ValueChangedHandler ValueChanged;

    //最小値
    public int Min
    {
        get { return (int)GetValue(MinProperty); }
        set { SetValue(MinProperty, value); }
    }
    public static readonly DependencyProperty MinProperty = DependencyProperty.Register(nameof(Min), typeof(int), typeof(NumericTextBox2), new PropertyMetadata(0, CheckMinMax));
    //最大値
    public int Max
    {
        get { return (int)GetValue(MaxProperty); }
        set { SetValue(MaxProperty, value); }
    }
    public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(nameof(Max), typeof(int), typeof(NumericTextBox2), new PropertyMetadata(100, CheckMinMax));

    private static void CheckMinMax(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var con = d as NumericTextBox2;
        con.Value = con.RoundValue(con.Value);
    }
    //デフォルト値
    public int Def
    {
        get { return (int)GetValue(DefProperty); }
        set { SetValue(DefProperty, value); }
    }
    public static readonly DependencyProperty DefProperty = DependencyProperty.Register(nameof(Def), typeof(int), typeof(NumericTextBox2), new PropertyMetadata(0));
    //増加値
    public int Increment
    {
        get { return (int)GetValue(IncrementProperty); }
        set { SetValue(IncrementProperty, value); }
    }
    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(nameof(Increment), typeof(int), typeof(NumericTextBox2), new PropertyMetadata(1));
    //現在値
    public int Value
    {
        get { return (int)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericTextBox2), new PropertyMetadata(0, ValuePropertyChanged, RoundValue));
    private static object RoundValue(DependencyObject d, object baseValue)
    {
        var con = d as NumericTextBox2;
        return con.RoundValue((int)baseValue);
    }
    private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var con = d as NumericTextBox2;
        con.textBox.Text = con.Value.ToString();
        con.ValueChanged?.Invoke(con, con.Value);
    }

    private DateTime? pressedTime;
    private TextBox textBox;
    private DispatcherTimer timer;

    public NumericTextBox2()
    {

        var grid = new Grid();
        textBox = new TextBox();

        var btns = new Button[2];
        for (var i = 0; i < 2; i++)
        {
            var btn = new Button();
            btn.Tag = (i == 0) ? -1 : 1;
            btn.Content = (i == 0) ? "-" : "+";
            btn.BorderThickness = new Thickness(0);
            btn.PreviewMouseLeftButtonDown += Btn_PreviewMouseLeftButtonDown;
            btn.PreviewMouseLeftButtonUp += Btn_PreviewMouseLeftButtonUp;
            btns[i] = btn;
        }
        textBox.BorderThickness = new Thickness(0);
        textBox.HorizontalContentAlignment = HorizontalAlignment.Right;
        textBox.MouseWheel += TextBox_MouseWheel;
        textBox.PreviewTextInput += TextBox_PreviewTextInput;
        InputMethod.SetIsInputMethodEnabled(textBox, false);
        CommandManager.AddPreviewExecutedHandler(textBox, ExecuteCommandEvent);
        textBox.LostFocus += TextBox_LostFocus;
        textBox.KeyDown += TextBox_KeyDown;
        textBox.ContextMenu = null;

        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Pixel) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Pixel) });
        grid.Children.Add(btns[0]);
        grid.Children.Add(textBox);
        grid.Children.Add(btns[1]);
        Grid.SetColumn(btns[0], 0);
        Grid.SetColumn(textBox, 1);
        Grid.SetColumn(btns[1], 2);

        this.BorderThickness = new Thickness(1);
        this.BorderBrush = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        this.Child = grid;
        timer = new DispatcherTimer();
        timer.Interval = new TimeSpan(0, 0, 0, 0, 30);
        timer.Tick += Timer_Tick;
    }



    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CheckValue();
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        CheckValue();
    }

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = true;
        if (e.Text == "-" && textBox.CaretIndex == 0 && !Regex.IsMatch(textBox.Text, "^-"))
        {
            e.Handled = false;
        }
        else if (Regex.IsMatch(e.Text, @"[0-9]") && !(textBox.CaretIndex == 0 && textBox.Text.Contains("-")))
        {
            e.Handled = false;
        }
    }

    private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var val = e.Delta > 0 ? Increment : -Increment;
        AddValue(val);
    }

    private void Btn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var val = Increment * (int)(sender as Button).Tag;
        AddValue(val);
        pressedTime = DateTime.Now;
        timer.Tag = val;
        timer.Start();
    }

    private void Btn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        timer.Stop();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (pressedTime != null)
        {
            var ts = (TimeSpan)(DateTime.Now - pressedTime);
            if (ts.TotalMilliseconds < 500)
            {
                return;
            }
            pressedTime = null;
        }
        var val = (int)timer.Tag;
        AddValue(val);
    }

    private void ExecuteCommandEvent(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Paste)
        {
            e.Handled = true;
        }
    }

    private int RoundValue(int value)
    {
        return Math.Max(Math.Min(value, Max), Min);
    }

    private void AddValue(int val)
    {
        Value = Value + val;
    }

    private void CheckValue()
    {
        Value = int.TryParse(textBox.Text, out var v) ? v : Def;
        textBox.Text = Value.ToString();
    }


}

}

