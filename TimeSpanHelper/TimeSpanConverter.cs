using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace TimeSpanHelper;

public class TimeSpanToStringConverter : MarkupExtension, IValueConverter
{
    /// <summary>
    /// Convert TimeSpan to string "Hours:Minutes:Seconds". Over 24 Hours Ok. 
    /// </summary>
    /// <param name="value">TimeSpan</param>
    /// <param name="targetType"></param>
    /// <param name="parameter">ConverterParameter Optional.Å@TimeSpan format strings. Replace {SumHours}, {Sign}. {SumHours} show negative hours.</param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value is null) return "";

            var ts = (TimeSpan)value; // ex : negative TimeSpanString "-1:2:3"
            var sumHours = (int)ts.TotalHours;  // -1.0341666666666667 to -1
            var minutes = Math.Abs(ts.Minutes);    // -2 to 2
            var seconds = Math.Abs(ts.Seconds);  // -3 to 3 

            if (parameter is null)
            {
                return $@"{sumHours}:{minutes}:{seconds}";
            }
            else
            {
                var format = (string)parameter;  //'{SumHours}'\:mm\:ss,  '{Sign}'hh\:mm\:ss
                var sign = ts < TimeSpan.Zero ? "-" : "";
                return ts.ToString(format).Replace("{SumHours}", sumHours.ToString()).Replace("{Sign}", sign);
           }
        }
        catch (Exception)
        {
            return DependencyProperty.UnsetValue;  //Format is bad. But TimeProperty will be setted.
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty((string)value)) return null; //If TextBox.Text is null or empty, set Binding TimeSpanProperty to null

        TimeSpan ts = new();
        var rt = TimeSpanUtils.GetTimeSpan((string)value, ref ts);
        return rt ? ts : DependencyProperty.UnsetValue;  // Better than "Binding.DoNothing" ?
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}

public class TimeSpanTextBoxRule : ValidationRule
{
    /// <summary>
    /// Optional.Input Range Max
    /// </summary>
    public TimeSpan? Max { get; set; }

    /// <summary>
    /// Optional.Input Range Min
    /// </summary>
    public TimeSpan? Min { get; set; }

    /// <summary>
    /// Optional.Seconds should be 0
    /// </summary>
    public bool IsShortTime { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is null || string.IsNullOrEmpty((string)value)) return ValidationResult.ValidResult;

        TimeSpan ts = new();
        var rt = TimeSpanUtils.GetTimeSpan((string)value, ref ts);
        if (!rt) return new ValidationResult(false, "convert exception");

        if (Min is not null && ts < Min) return new ValidationResult(false, $@"Min: {Min} out of range");
        if (Max is not null && ts > Max) return new ValidationResult(false, $@"Max: {Max} out of range");
        if (IsShortTime && ts.Seconds != 0) return new ValidationResult(false, $@"Seconds should be 0.");

        return ValidationResult.ValidResult;
    }
}

public static class TimeSpanUtils
{
    /// <summary>
    /// 1 number : hours    "0" to "0:0:0" ,    "-1" to "-01:00:00"
    /// 2 numbers : hours, minutes    "1:2" to "01:02:00"
    /// 3 numbers : hours, minutes, seconds    "1:2:3" to "01:02:03"
    /// 4 numbers : days, hours, minutes, seconds    "1:2:3:4" to "1.02:03:04"
    /// Any char can be used as separator.    "1,2 3aaaa4" to "1.02:03:04"
    /// </summary>
    /// <param name="timeSpanString"></param>
    /// <param name="ts"></param>
    /// <returns>true : conversion succeeded</returns>
    public static bool GetTimeSpan(string timeSpanString, ref TimeSpan ts)
    {
        try
        {
            bool isNegative = timeSpanString.StartsWith("-"); // "-1:2:3" is true
            var digitsString = Regex.Replace(timeSpanString, "[^0-9]", " "); // "-1:2:3" to " 1 2 3" 
            var s = digitsString.Split(' ', StringSplitOptions.RemoveEmptyEntries); // "1","2","3"

            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            switch (s.Length)
            {
                case 1:
                    hours = int.Parse(s[0]);
                    break;

                case 2:
                    hours = int.Parse(s[0]);
                    minutes = int.Parse(s[1]);
                    break;

                case 3:
                    hours = int.Parse(s[0]);
                    minutes = int.Parse(s[1]);
                    seconds = int.Parse(s[2]);
                    break;

                case 4:
                    days = int.Parse(s[0]);
                    hours = int.Parse(s[1]);
                    minutes = int.Parse(s[2]);
                    seconds = int.Parse(s[3]);
                    break;

                default:
                    return false; //no digits or length > 4
            }

            if (isNegative)
            {
                ts = new TimeSpan(-days, -hours, -minutes, -seconds);
            }
            else
            {
                ts = new TimeSpan(days, hours, minutes, seconds);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
