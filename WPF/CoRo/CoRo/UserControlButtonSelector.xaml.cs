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
    /// Interaktionslogik für UserControlButtonSelector.xaml
    /// </summary>
    public partial class UserControlButtonSelector : UserControl
    {
        public event RoutedEventHandler Click;

        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(UserControlButtonSelector), new PropertyMetadata(null));

        public object ButtonText
        {
            get { return (object)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(object), typeof(UserControlButtonSelector), new PropertyMetadata(null));

        public UserControlButtonSelector()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }
        }
    }
}
