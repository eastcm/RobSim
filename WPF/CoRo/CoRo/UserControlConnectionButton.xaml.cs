using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.ComponentModel;

namespace CoRo
{
    /// <summary>
    /// Interaktionslogik für UserControlConnectionButton.xaml
    /// </summary>
    public partial class UserControlConnectionButton : UserControl
    {
        public enum Status { Disconnected, Connecting, Connected, Error };

        public Status ValueStatus
        {
            get { return (Status)GetValue(ValueStatusProperty); }
            set { SetValue(ValueStatusProperty, value); }
        }

        public static readonly DependencyProperty ValueStatusProperty = DependencyProperty.Register("ValueStatus", typeof(Status), typeof(UserControlConnectionButton), new PropertyMetadata(null));

        public UserControlConnectionButton()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public event RoutedEventHandler Click;

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null) Click(sender, e);
        }

        public void SetStatus(Status status)
        {
            switch (status)
            {
                case Status.Disconnected:
                    icon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connectionicon_black.png"));
                    break;
                case Status.Connecting:
                    icon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connectionicon_yellow.png"));
                    break;
                case Status.Connected:
                    icon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connectionicon_green.png"));
                    break;
                case Status.Error:
                    icon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/connectionicon_red.png"));
                    break;
            }
        }     

    }
}
