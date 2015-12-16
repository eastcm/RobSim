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
    /// Interaktionslogik für UserControlAxisControl.xaml
    /// </summary>
    public partial class UserControlAxisControl : UserControl
    {
        public event RoutedEventHandler ValueChanged;

        public double Value = 0;
        public double Increment = 1;

        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(UserControlAxisControl), new PropertyMetadata(null));

        public object Minimum
        {
            get { return (object)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(object), typeof(UserControlAxisControl), new PropertyMetadata(null));

        public object Maximum
        {
            get { return (object)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(object), typeof(UserControlAxisControl), new PropertyMetadata(null));

        public UserControlAxisControl()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            Value -= Increment;
            slider.Value = Value;
            textbox.Text = Value.ToString("0.0");
        }

        private void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            Value += Increment;
            slider.Value = Value;
            textbox.Text = Value.ToString("0.0");
        }

        private void slider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new RoutedEventArgs());
            }
        }

        public void SetValue(double value)
        {
            Value = value;
            slider.Value = Value;
            textbox.Text = Value.ToString("0.0");
        }
    }
}
