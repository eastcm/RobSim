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
using System.Text.RegularExpressions;

namespace CoRo
{
    /// <summary>
    /// Interaktionslogik für UserControlNumericControl.xaml
    /// </summary>
    public partial class UserControlNumericControl : UserControl
    {
        public event RoutedEventHandler ValueChanged;

        public double Value = 0;
        public double Increment = 1;

        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(UserControlNumericControl), new PropertyMetadata(null));

        public object Minimum
        {
            get { return (object)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(object), typeof(UserControlNumericControl), new PropertyMetadata(null));

        public object Maximum
        {
            get { return (object)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(object), typeof(UserControlNumericControl), new PropertyMetadata(null));

        public UserControlNumericControl()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            Value -= Increment;
            SetValue(Value);
        }

        private void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            Value += Increment;
            SetValue(Value);
        }

        public void SetValue(double value)
        {
            Value = value;
            textbox.Text = Value.ToString("0.00");
            if (ValueChanged != null)
            {
                ValueChanged(this, new RoutedEventArgs());
            }
        }

        private void textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Convert.ToDouble(textbox.Text);
        }
    }
}
