using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeSpanStudy
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TimeSpanStudy"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TimeSpanStudy;assembly=TimeSpanStudy"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class CustomControl1 : TextBox
    {
        static CustomControl1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl1), new FrameworkPropertyMetadata(typeof(CustomControl1)));

            //https://github.com/ray-sgdc/r10810test2/blob/e619e6b319988e4da6d6d9fe760f17a915a2c162/DEV/Client/Source/Common/Controls/SearchTextBox/SearchTextBox.cs
            TextProperty.OverrideMetadata(typeof(CustomControl1),
               new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));
        }

        //    public string SubText
        //    {
        //        get { return (string)GetValue(SubTextProperty); }
        //        set { SetValue(SubTextProperty, value); }
        //    }

        //    public static readonly DependencyProperty SubTextProperty =
        //DependencyProperty.Register(
        //    nameof(SubText),
        //    typeof(string),
        //    typeof(CustomControl1),
        //    new UIPropertyMetadata(null)
        //);


        //http://yujiro15.net/YKSoftware/tips_DependencyProperty.html

        //public TimeSpan? Text
        //{
        //    get { return (TimeSpan?)this.GetValue(TextProperty); }
        //    set { this.SetValue(TextProperty, value); }
        //}
        //public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        //    "Text",
        //    typeof(TimeSpan?),
        //    typeof(CustomControl1),
        //    new FrameworkPropertyMetadata(new TimeSpan?(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(TextPropertyChanged)));
        //private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((CustomControl1)d).Text = ((CustomControl1)d).FormatBack(e.NewValue.ToString());
        //}

        public TimeSpan? Value
        {
            get { return (TimeSpan?)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(TimeSpan?),
            typeof(CustomControl1),
            new FrameworkPropertyMetadata(new TimeSpan?(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ValuePropertyChanged)));


        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //((CustomControl1)d).Value = ((CustomControl1)d).FormatBack(e.NewValue.ToString());
            ((CustomControl1)d).Text = e.NewValue is null ? "" : e.NewValue.ToString();

        }


        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var text =  ((CustomControl1)d).Text;
            if (string.IsNullOrEmpty(text))
            {
                ((CustomControl1)d).Value = null;
                return;
            }

            TimeSpan timeSpan;
            var r = TimeSpan.TryParse(text, out timeSpan);
            if (!r) return;

            ((CustomControl1)d).Value = ((CustomControl1)d).FormatBack(((CustomControl1)d).Text);
        }

        private string Format(string text)
        {
            string unformatedString = text;

            return unformatedString;
        }
        //private TimeSpan FormatBack(string text)
        //{
        //    TimeSpan returnValue = TimeSpan.Parse(text);
        //    return returnValue;
        //}
        private TimeSpan? FormatBack(string text)
        {
            TimeSpan timeSpan;
            var r = TimeSpan.TryParse(text, out timeSpan);
            if (r == false) return null;
            return timeSpan;
        }
    }
}
