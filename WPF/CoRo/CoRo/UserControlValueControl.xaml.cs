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

namespace CoRo
{
    /// <summary>
    /// Interaktionslogik für UserControlValueControl.xaml
    /// </summary>
    public partial class UserControlValueControl : UserControl
    {
        public double Value = 1;

        public object ValueName
        {
            get { return (object)GetValue(ValueNameProperty); }
            set { SetValue(ValueNameProperty, value); }
        }

        public static readonly DependencyProperty ValueNameProperty = DependencyProperty.Register("ValueName", typeof(object), typeof(UserControlAxisControl), new PropertyMetadata(null));

        public object ValueMinimum
        {
            get { return (object)GetValue(ValueMinimumProperty); }
            set { SetValue(ValueMinimumProperty, value); }
        }

        public static readonly DependencyProperty ValueMinimumProperty = DependencyProperty.Register("ValueMinimum", typeof(object), typeof(UserControlAxisControl), new PropertyMetadata(null));

        public object ValueMaximum
        {
            get { return (object)GetValue(ValueMaximumProperty); }
            set { SetValue(ValueMaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueMaximumProperty = DependencyProperty.Register("ValueMaximum", typeof(object), typeof(UserControlAxisControl), new PropertyMetadata(null));

        public object ValueTickFrequency
        {
            get { return (object)GetValue(ValueTickFrequencyProperty); }
            set { SetValue(ValueTickFrequencyProperty, value); }
        }

        public static readonly DependencyProperty ValueTickFrequencyProperty = DependencyProperty.Register("ValueTickFrequency", typeof(object), typeof(UserControlAxisControl), new PropertyMetadata(null));

        public object ValueUnit
        {
            get { return (object)GetValue(ValueUnitProperty); }
            set { SetValue(ValueUnitProperty, value); }
        }

        public static readonly DependencyProperty ValueUnitProperty = DependencyProperty.Register("ValueUnit", typeof(object), typeof(UserControlAxisControl), new PropertyMetadata(null));

        public UserControlValueControl()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public event RoutedEventHandler ValueChanged;

        private void slider_ValueChanged(object sender, RoutedEventArgs e)
        {
            Value = slider.Value;
            if((Convert.ToDouble(ValueTickFrequency) == Math.Floor(Convert.ToDouble(ValueTickFrequency))))
                label.Content = Value.ToString("0");
            else
                label.Content = Value.ToString("0.0");
            if (ValueUnit != null)
                label.Content = label.Content + " " + ValueUnit.ToString();
            if (ValueChanged != null) ValueChanged(sender, e);

        }

        public void SetValue(double value)
        {
            slider.Value = value;
        }
    }
}
