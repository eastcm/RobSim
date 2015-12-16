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
    /// Interaktionslogik für UserControlButtonModifierSelector.xaml
    /// </summary>
    public partial class UserControlButtonModifierSelector : UserControl
    {
        public event RoutedEventHandler Click;

        public bool Selected = false;

        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(UserControlButtonModifierSelector), new PropertyMetadata(null));

        public object ButtonText
        {
            get { return (object)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(object), typeof(UserControlButtonModifierSelector), new PropertyMetadata(null));

        public UserControlButtonModifierSelector()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (Selected)
            {
                button.Background = Brushes.Transparent;
                Selected = false;
            }
            else
            {
                button.Background = Brushes.LightGreen;
                Selected = true;
            }

            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }
        }
    }
}
