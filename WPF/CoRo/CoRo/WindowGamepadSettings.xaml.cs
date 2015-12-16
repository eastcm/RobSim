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
using System.Windows.Shapes;
using System.ComponentModel;
using IRob;

namespace CoRo
{
    /// <summary>
    /// Interaktionslogik für WindowGamepadSettings.xaml
    /// </summary>
    public partial class WindowGamepadSettings : Window
    {
        public Gamepad gamepad = new Gamepad();
        private readonly BackgroundWorker worker = new BackgroundWorker();
        bool update = true;
        int index = -1;

        public WindowGamepadSettings()
        {
            InitializeComponent();        

            //Load saved data
            string path = Properties.Settings.Default.GamepadIniPath;
            if (path != "")
            {
                IniListPath.Text = path;
                gamepad.LoadIni(path);
            }

            //View data
            worker.DoWork += ViewData;
            worker.RunWorkerAsync();

            //Select saved gamepad
            index = Properties.Settings.Default.GamepadIndex;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            update = false;
        }

        private void buttonLoadIniList_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                IniListPath.Text = dlg.FileName;
                Properties.Settings.Default.GamepadIniPath = dlg.FileName;
                gamepad.LoadIni(Properties.Settings.Default.GamepadIniPath);
            }
        }

        private void buttonSaveIniList_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".ini";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                IniListPath.Text = dlg.FileName;
                Properties.Settings.Default.GamepadIniPath = dlg.FileName;
                gamepad.SaveIni(Properties.Settings.Default.GamepadIniPath);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.GamepadIndex = GamepadList.SelectedIndex;
            Properties.Settings.Default.Save();
            gamepad.SaveIni(Properties.Settings.Default.GamepadIniPath);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void ViewData(object sender, EventArgs arg)
        {
            while (update)
            {
                //View settings
                foreach (Gamepad.Settings settings in gamepad.IniList)
                {
                    switch (settings.Type)
                    {
                        case "X":
                            Dispatcher.BeginInvoke(new Action(() => controlX.ButtonText = settings.Button));
                            break;
                        case "Y":
                            Dispatcher.BeginInvoke(new Action(() => controlY.ButtonText = settings.Button));
                            break;
                        case "Z":
                            Dispatcher.BeginInvoke(new Action(() => controlZ.ButtonText = settings.Button));
                            break;
                        case "A":
                            Dispatcher.BeginInvoke(new Action(() => controlA.ButtonText = settings.Button));
                            break;
                        case "B":
                            Dispatcher.BeginInvoke(new Action(() => controlB.ButtonText = settings.Button));
                            break;
                        case "C":
                            Dispatcher.BeginInvoke(new Action(() => controlC.ButtonText = settings.Button));
                            break;
                        case "A1":
                            Dispatcher.BeginInvoke(new Action(() => controlA1.ButtonText = settings.Button));
                            break;
                        case "A2":
                            Dispatcher.BeginInvoke(new Action(() => controlA2.ButtonText = settings.Button));
                            break;
                        case "A3":
                            Dispatcher.BeginInvoke(new Action(() => controlA3.ButtonText = settings.Button));
                            break;
                        case "A4":
                            Dispatcher.BeginInvoke(new Action(() => controlA4.ButtonText = settings.Button));
                            break;
                        case "A5":
                            Dispatcher.BeginInvoke(new Action(() => controlA5.ButtonText = settings.Button));
                            break;
                        case "A6":
                            Dispatcher.BeginInvoke(new Action(() => controlA6.ButtonText = settings.Button));
                            break;
                        case "Mode":
                            Dispatcher.BeginInvoke(new Action(() => controlMode.ButtonText = settings.Button));
                            break;
                        case "Gripper":
                            Dispatcher.BeginInvoke(new Action(() => controlGripper.ButtonText = settings.Button));
                            break;
                        case "Cooling":
                            Dispatcher.BeginInvoke(new Action(() => controlCooling.ButtonText = settings.Button));
                            break;
                        case "Home":
                            Dispatcher.BeginInvoke(new Action(() => controlHome.ButtonText = settings.Button));
                            break;
                        case "ConnectDisconnect":
                            Dispatcher.BeginInvoke(new Action(() => controlMode.ButtonText = settings.Button));
                            break;
                        case "SwitchRobot":
                            Dispatcher.BeginInvoke(new Action(() => controlSwitch.ButtonText = settings.Button));
                            break;
                    }
                }

                //View connected gamepads
                List<string> connectedGamepads = gamepad.GetConnectedGamepads();
                Dispatcher.BeginInvoke(new Action(() => GamepadList.Items.Clear()));
                foreach (string connectedGamepad in connectedGamepads)
                {
                    Dispatcher.BeginInvoke(new Action(() => GamepadList.Items.Add(connectedGamepad)));
                }
                Dispatcher.BeginInvoke(new Action(() => GamepadList.SelectedIndex = index));

                Thread.Sleep(200);
            }
        }

        private void controlX_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlX.Label.ToString());
        }

        private void controlY_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlY.Label.ToString());
        }

        private void controlZ_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlZ.Label.ToString());
        }

        private void controlA_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlA.Label.ToString());
        }

        private void controlB_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlB.Label.ToString());
        }

        private void controlC_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlC.Label.ToString());
        }

        private void controlA1_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlA1.Label.ToString());
        }

        private void controlA2_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlA2.Label.ToString());
        }

        private void controlA3_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlA3.Label.ToString());
        }

        private void controlA4_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlA4.Label.ToString());
        }

        private void controlA5_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlA5.Label.ToString());
        }

        private void controlA6_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlA6.Label.ToString());
        }

        private void controlHome_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlHome.Label.ToString());
        }

        private void controlSwitch_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlSwitch.Label.ToString());
        }

        private void controlGripper_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlGripper.Label.ToString());
        }

        private void controlCooling_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlCooling.Label.ToString());
        }

        private void controlMode_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlMode.Label.ToString());
        }

        private void controlConnect_Click(object sender, RoutedEventArgs e)
        {
            gamepad.SetButton(controlConnect.Label.ToString());
        }

        private void GamepadList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Connect to selected gamepad
            index = GamepadList.SelectedIndex;
            if (index != -1)
                gamepad.Connect(index);
        }

    }
}
