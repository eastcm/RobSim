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
    /// Interaktionslogik für UserControlNumberControl.xaml
    /// </summary>
    public partial class UserControlNumberControl : UserControl
    {
        public int Value = 1;
        public int Minimum = 1;
        public int Maximum = 2;

        public UserControlNumberControl()
        {
            InitializeComponent();
        }

        private void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            Value++;
            if (Value > Maximum)
                Value = Minimum;
            labelValue.Content = Value.ToString();
        }

        private void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            Value--;
            if (Value < Minimum)
                Value = Maximum;
            labelValue.Content = Value.ToString();
        }

        public void SetValue(int value)
        {
            Value = value;
            labelValue.Content = Value.ToString();
        }

        public int GetValue()
        {
            return Value;
        }
    }
}
