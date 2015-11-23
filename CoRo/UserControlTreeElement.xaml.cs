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
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class UserControlTreeElement : UserControl
    {
        public event RoutedEventHandler CustomClick;

        public object Image
        {
            get { return (object)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(object), typeof(UserControlTreeElement), new PropertyMetadata(null));

        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
    
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(UserControlTreeElement), new PropertyMetadata(null));
        

        public UserControlTreeElement()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomClick != null)
            {
                CustomClick(this, new RoutedEventArgs());
            }
        }
    }
}
