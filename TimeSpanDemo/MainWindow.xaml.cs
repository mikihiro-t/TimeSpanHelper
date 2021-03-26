using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace TimeSpanDemo
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private TimeSpan? time1;
        public TimeSpan? Time1
        {
            get => time1;
            set => SetField(ref time1, value);
        }

        private TimeSpan? _time2;
        public TimeSpan? Time2
        {
            get => _time2;
            set => SetField(ref _time2, value);
        }

        private TimeSpan? _time3;
        public TimeSpan? Time3
        {
            get => _time3;
            set => SetField(ref _time3, value);
        }

        private TimeSpan? _time4;
        public TimeSpan? Time4
        {
            get => _time4;
            set => SetField(ref _time4, value);
        }

        public MainWindow()
        {
            InitializeComponent();
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
