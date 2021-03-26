using Ganges.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeSpanStudy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {




        private TimeSpan? time1;
        public TimeSpan? Time1
        {
            get => time1;
            set => SetField(ref time1, value);
        }
        private TimeSpan time2;
        public TimeSpan Time2
        {
            get => time2;
            set => SetField(ref time2, value);
        }




        private string _test;
        public string Test
        {
            get => _test;
            set => SetField(ref _test, value);
        }


        public MainWindow()
        {
            InitializeComponent();
            //Time1 = new TimeSpan(-150, 1, 1);
            Time2 = new TimeSpan(0, 1, 1);
            Test = "1";

            DataContext = this;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }


}
