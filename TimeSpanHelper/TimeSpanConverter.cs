using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace TimeSpanHelper
{
    public class TimeSpanToStringConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// VMのプロパティのTimeSpanを時間+分+秒で表示する。
        /// ConverterParameter をfalseにしておくと、Convertを利用しない。その時は、TimeSpanのまま、TextBoxに渡す。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "";
            if (parameter is not null && bool.Parse((string)parameter) == false) return value;

            var ts = (TimeSpan)value;
            var hourOnly = (int)ts.TotalHours;  //マイナス表示もある
            var minutes = Math.Abs(ts.Minutes);    //マイナスの時は、hourOnlyで表示するので、数字のみにする。
            var secornds = Math.Abs(ts.Seconds);
            return $@"{hourOnly}:{minutes}:{secornds}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (string.IsNullOrEmpty((string)value)) return null; //""や空白の時は、バインディングされているTimeをnullにする。

            TimeSpan ts = new();
            var rt = TimeSpanUtils.GetTimeSpan((string)value, ref ts);
            return rt ? ts : DependencyProperty.UnsetValue;
            //return rt ? ts : Binding.DoNothing;  //TimeSpanに変更できない時は、バインディング先のTimeに何もしない。ただし、TimeSpanTextBoxRuleで、TimeSpanに変換できない時は、ConvertBackされないはず。

            //https://take4-blue.com/program/wpf%E3%82%92%E4%BD%BF%E3%81%86-%E3%83%A9%E3%82%B8%E3%82%AA%E3%83%9C%E3%82%BF%E3%83%B3%E3%81%A8enum/


        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }


    /// <summary>
    /// https://stackoverflow.com/questions/2728321/how-to-parse-string-with-hours-greater-than-24-to-timespan
    /// https://araramistudio.jimdo.com/2016/09/16/wpf%E3%81%AE%E5%85%A5%E5%8A%9B%E8%A6%8F%E5%88%B6%E3%82%92%E3%82%AB%E3%82%B9%E3%82%BF%E3%83%9E%E3%82%A4%E3%82%BA/
    /// https://github.com/microsoft/WPF-Samples/blob/master/Data%20Binding/BindValidation/MainWindow.xaml
    /// </summary>
    public class TimeSpanTextBoxRule : ValidationRule
    {
        /// <summary>
        /// 入力できる最小値。指定しないとnull。
        /// </summary>
        public TimeSpan? Min { get; set; }
        public TimeSpan? Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is null || string.IsNullOrEmpty((string)value)) return new ValidationResult(true, null);

            TimeSpan ts = new();
            var rt = TimeSpanUtils.GetTimeSpan((string)value, ref ts);
            if (!rt) return new ValidationResult(false, "TimeSpanに変換できません。");

            if (Min is not null && ts < Min) return new ValidationResult(false, $@"{Min}以上で入力して下さい。");
            if (Max is not null && ts > Max) return new ValidationResult(false, $@"{Max}以下で入力して下さい。");


            return new ValidationResult(true, null);

        }
    }

    public static class TimeSpanUtils
    {

        /// <summary>
        /// 1 number : hours    "0" -> "0:0:0" ,    "-1" -> "-01:00:00"
        /// 2 numbers : hours, minutes    "1:2" -> "01:02:00"
        /// 3 numbers : hours, minutes, seconds    "1:2:3" -> "01:02:03"
        /// 4 numbers : days, hours, minutes, seconds    "1:2:3:4" -> "1.02:03:04"
        /// Any char can be used as separator.    "1,2 3aaaa4" -> "1.02:03:04"
        /// </summary>
        /// <param name="timeSpanString"></param>
        /// <param name="ts"></param>
        /// <returns>true : conversion succeeded</returns>
        public static bool GetTimeSpan(string timeSpanString, ref TimeSpan ts)
        {

            bool isNegative = timeSpanString.StartsWith("-"); // "-1:2:3" -> true
            var digitsString = Regex.Replace(timeSpanString, "[^0-9]", " "); // "-1:2:3" -> " 1 2 3" 
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

    }
}
