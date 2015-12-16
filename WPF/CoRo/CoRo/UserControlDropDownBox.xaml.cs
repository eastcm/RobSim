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
    /// Interaktionslogik für UserControlDropDownBox.xaml
    /// </summary>
    public partial class UserControlDropDownBox : UserControl
    {
        public event RoutedEventHandler Changed;

        public bool Extended = false;

        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(UserControlDropDownBox), new PropertyMetadata(null));

        public UserControlDropDownBox()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, new RoutedEventArgs());
            }
            if (Extended)
            {
                Extended = false;
                buttonPlus.Content = "5";
            }
            else
            {
                Extended = true;
                buttonPlus.Content = "6";
            }
        }
    }
}
