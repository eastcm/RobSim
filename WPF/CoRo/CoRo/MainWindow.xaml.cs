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
using Microsoft.Win32;
using IRob;
using Kitware.VTK;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Runtime.InteropServices;


namespace CoRo
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        bool update = true;

        Simulation simulation;

        Roboteam roboteam = new Roboteam();
        PointCloud pointCloud = new PointCloud();

        int selecetedCommand = -1;
        bool play = false;

        Gamepad gamepad = new Gamepad();

        Scanner sensor = new Scanner();
        EnumerableDataSource<IRob.Geometry.Point> dataSource;

        public enum device { Robot, Sensor, Workpiece, Pump, None };
        private device Device = device.None;
        public device selectedDevice
        {
            get { return Device; }
            set
            {
                Device = value;
                buttonRobot.Background = Brushes.LightGray;
                buttonSensor.Background = Brushes.LightGray;
                buttonWorkpiece.Background = Brushes.LightGray;
                buttonPump.Background = Brushes.LightGray;
                buttonRobot.Margin = new Thickness(0, 0, 0, 0);
                buttonSensor.Margin = new Thickness(0, 0, 0, 0);
                buttonWorkpiece.Margin = new Thickness(0, 0, 0, 0);
                buttonPump.Margin = new Thickness(0, 0, 0, 0);
                buttonRobot.BorderThickness = new Thickness(1);
                buttonSensor.BorderThickness = new Thickness(1);
                buttonWorkpiece.BorderThickness = new Thickness(1);
                buttonPump.BorderThickness = new Thickness(1);
                switch (Device)
                {
                    case device.None:
                        deviceColumn.Width = new GridLength(10, GridUnitType.Pixel);
                        break;
                    case device.Robot:
                        tabControl.SelectedIndex = 0;
                        buttonRobot.Background = Brushes.White;
                        buttonRobot.Margin = new Thickness(0, 0, -2, 0);
                        buttonRobot.BorderThickness = new Thickness(0);
                        deviceColumn.Width = new GridLength(deviceColumn.MaxWidth, GridUnitType.Pixel);
                        break;
                    case device.Sensor:
                        tabControl.SelectedIndex = 1;
                        buttonSensor.Background = Brushes.White;
                        buttonSensor.Margin = new Thickness(0, 0, -2, 0);
                        buttonSensor.BorderThickness = new Thickness(0);
                        deviceColumn.Width = new GridLength(deviceColumn.MaxWidth, GridUnitType.Pixel);
                        break;
                    case device.Workpiece:
                        tabControl.SelectedIndex = 2;
                        buttonWorkpiece.Background = Brushes.White;
                        buttonWorkpiece.Margin = new Thickness(0, 0, -2, 0);
                        buttonWorkpiece.BorderThickness = new Thickness(0);
                        deviceColumn.Width = new GridLength(deviceColumn.MaxWidth, GridUnitType.Pixel);
                        break;
                    case device.Pump:
                        tabControl.SelectedIndex = 3;
                        buttonPump.Background = Brushes.White;
                        buttonPump.Margin = new Thickness(0, 0, -2, 0);
                        buttonPump.BorderThickness = new Thickness(0);
                        deviceColumn.Width = new GridLength(deviceColumn.MaxWidth, GridUnitType.Pixel);
                        break;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;

            controlIncrement.SetValue(1);
            controlShuttertime.SetValue(200);

            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();

            Rect domainRect = new Rect(-75, 120, 150, 275);
            plotter.Viewport.Domain = domainRect;
            plotter.LegendVisible = false;

            foreach (Routine.Task task in Enum.GetValues(typeof(Routine.Task)))
            {
                comboBoxRobotProgramTask.Items.Add(task);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:

                    break;
                case Key.T:
                    buttonTree_Click(sender, e);
                    break;
                case Key.D1:
                    buttonRobot_Click(sender, e);
                    break;
                case Key.D2:
                    buttonSensor_Click(sender, e);
                    break;
                case Key.D3:
                    buttonWorkpiece_Click(sender, e);
                    break;
                case Key.D4:
                    buttonPump_Click(sender, e);
                    break;
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (update)
            {
                //Simulation
                Dispatcher.BeginInvoke(new Action(() => renderControl.RenderWindow.Render()));

                //Robot
                if (controlRobot.Value == 1)
                {
                    Dispatcher.BeginInvoke(new Action(() => ViewRobotData(roboteam, 1)));
                }
                else if (controlRobot.Value == 2)
                {
                    Dispatcher.BeginInvoke(new Action(() => ViewRobotData(roboteam, 2)));
                }

                //Gamepad
                if (gamepad.Connected)
                {
                    Dispatcher.BeginInvoke(new Action(() => imageGamepad.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/gamepadicon_green.png"))));
                    if (controlRobot.Value == 1)
                        gamepad.GamepadRobot = roboteam.Robot1;
                    else if (controlRobot.Value == 2)
                        gamepad.GamepadRobot = roboteam.Robot2;
                }
                else
                    Dispatcher.BeginInvoke(new Action(() => imageGamepad.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/gamepadicon_black.png"))));

                //Sensor
                switch (sensor.Status)
                {
                    case Scanner.ConnectionStatus.Disconnected:
                        Dispatcher.BeginInvoke(new Action(() => buttonSensorConnect.SetStatus(UserControlConnectionButton.Status.Disconnected)));
                        break;
                    case Scanner.ConnectionStatus.Connecting:
                        Dispatcher.BeginInvoke(new Action(() => buttonSensorConnect.SetStatus(UserControlConnectionButton.Status.Connecting)));
                        break;
                    case Scanner.ConnectionStatus.Connected:
                        Dispatcher.BeginInvoke(new Action(() => buttonSensorConnect.SetStatus(UserControlConnectionButton.Status.Connected)));
                        dataSource = new EnumerableDataSource<IRob.Geometry.Point>(sensor.Profile.Select(x => x.Point));
                        dataSource.SetXMapping(x => x.X);
                        dataSource.SetYMapping(z => z.Z);
                        Dispatcher.BeginInvoke(new Action(() => labelSensorPoints.Content = "Points: " + sensor.Profile.Count()));
                        Dispatcher.BeginInvoke(new Action(() => labelSensorMinimum.Content = "Minimum: X " + sensor.Minimum.X.ToString("0.00") + ", Z " + sensor.Minimum.Z.ToString("0.00")));
                        Dispatcher.BeginInvoke(new Action(() => labelSensorMaximum.Content = "Maximum: X " + sensor.Maximum.X.ToString("0.00") + ", Z " + sensor.Maximum.Z.ToString("0.00")));
                        Dispatcher.BeginInvoke(new Action(() => linegraph.DataSource = dataSource));
                        break;
                    case Scanner.ConnectionStatus.Error:
                        Dispatcher.BeginInvoke(new Action(() => buttonSensorConnect.SetStatus(UserControlConnectionButton.Status.Error)));
                        break;
                }

                Thread.Sleep(200);
            }
        }

        #region Tree

        private void buttonTree_Click(object sender, RoutedEventArgs e)
        {
            if (treeColumn.Width.Value > 10)
            {
                treeColumn.Width = new GridLength(10, GridUnitType.Pixel);
                buttonTree.Content = 4;
            }
            else
            {
                treeColumn.Width = new GridLength(treeColumn.MaxWidth, GridUnitType.Pixel);
                buttonTree.Content = 3;
            }
        }

        private void OnShowCellBaseClick(object sender, RoutedEventArgs e)
        {
            if (simulation.BaseVisibility)
                simulation.HideBase();
            else
                simulation.ViewBase();
        }

        private void OnShowCellFloorClick(object sender, RoutedEventArgs e)
        {
            if (simulation.FloorVisibility)
                simulation.HideFloor();
            else
                simulation.ViewFloor();
        }

        private void OnShowRobot1Click(object sender, RoutedEventArgs e)
        {
            if (simulation.Robot1Visibility)
                simulation.HideRobot1();
            else
                simulation.ViewRobot1();
        }

        #endregion

        #region Simulation

        private void WFHost_Loaded(object sender, RoutedEventArgs e)
        {
            simulation = new Simulation(renderControl.RenderWindow);
            simulation.Robots = roboteam;
            simulation.PointCloud = pointCloud;
            
        }

        #endregion

        #region Robot

        private void buttonRobot_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDevice == device.Robot)
                selectedDevice = device.None;
            else
                selectedDevice = device.Robot;
        }

        private void controlDropDownRobotControl_Changed(object sender, RoutedEventArgs e)
        {
            if (controlDropDownRobotControl.Extended)
                groupBoxRobotControl.Height = Double.NaN;
            else
                groupBoxRobotControl.Height = 0;
        }

        private void controlDropDownRobotProgram_Changed(object sender, RoutedEventArgs e)
        {
            if (controlDropDownRobotProgram.Extended)
                groupBoxRobotProgram.Height = Double.NaN;
            else
                groupBoxRobotProgram.Height = 0;
        }

        private void buttonRobotConnect_Click(object sender, RoutedEventArgs e)
        {
            Robot robot = new Robot();
            if (controlRobot.Value == 1)
                robot = roboteam.Robot1;
            else if (controlRobot.Value == 2)
                robot = roboteam.Robot2;
            if (robot.Status == Robot.ConnectionStatus.Disconnected)
                robot.Connect(Properties.Settings.Default.Port, Properties.Settings.Default.CardIndex);
            else
                robot.Disconnect();
        }

        private void buttonRobotHome_Click(object sender, RoutedEventArgs e)
        {
            if (controlRobot.Value == 1)
                roboteam.Robot1.MoveToHome();
            else if (controlRobot.Value == 2)
                roboteam.Robot2.MoveToHome();
        }

        private void controlIncrement_ValueChanged(object sender, RoutedEventArgs e)
        {
            controlX.Increment = controlIncrement.Value;
            controlY.Increment = controlIncrement.Value;
            controlZ.Increment = controlIncrement.Value;
            controlA.Increment = controlIncrement.Value;
            controlB.Increment = controlIncrement.Value;
            controlC.Increment = controlIncrement.Value;
        }

        private void radioButtonRobotControl_Checked(object sender, RoutedEventArgs e)
        {
            if (controlX != null)
            {
                if (radioButtonRobotControlCartesian.IsChecked == true)
                {
                    controlX.Label = "X:";
                    controlY.Label = "Y:";
                    controlZ.Label = "Z:";
                    controlA.Label = "A:";
                    controlB.Label = "B:";
                    controlC.Label = "C:";
                    roboteam.Robot1.SwitchMode(Routine.Task.LIN);
                    roboteam.Robot2.SwitchMode(Routine.Task.LIN);
                }
                else if (radioButtonRobotControlAxis.IsChecked == true)
                {
                    controlX.Label = "A1:";
                    controlY.Label = "A2:";
                    controlZ.Label = "A3:";
                    controlA.Label = "A4:";
                    controlB.Label = "A5:";
                    controlC.Label = "A6:";
                    roboteam.Robot1.SwitchMode(Routine.Task.AXS);
                    roboteam.Robot2.SwitchMode(Routine.Task.AXS);
                }
            }
        }

        private void controlX_ValueChanged(object sender, RoutedEventArgs e)
        {

            Robot robot = new Robot();
            if (controlRobot.Value == 1)
                robot = roboteam.Robot1;
            else if (controlRobot.Value == 2)
                robot = roboteam.Robot2;
            if (robot.Action == Routine.Task.AXS)
                robot.TAxs.X = controlX.Value;
            else
                robot.TPos.X = controlX.Value;

        }

        private void controlY_ValueChanged(object sender, RoutedEventArgs e)
        {
            Robot robot = new Robot();
            if (controlRobot.Value == 1)
                robot = roboteam.Robot1;
            else if (controlRobot.Value == 2)
                robot = roboteam.Robot2;
            if (robot.Action == Routine.Task.AXS)
                robot.TAxs.Y = controlY.Value;
            else
                robot.TPos.Y = controlY.Value;
        }

        private void controlZ_ValueChanged(object sender, RoutedEventArgs e)
        {
            Robot robot = new Robot();
            if (controlRobot.Value == 1)
                robot = roboteam.Robot1;
            else if (controlRobot.Value == 2)
                robot = roboteam.Robot2;
            if (robot.Action == Routine.Task.AXS)
                robot.TAxs.Z = controlZ.Value;
            else
                robot.TPos.Z = controlZ.Value;
        }

        private void controlA_ValueChanged(object sender, RoutedEventArgs e)
        {
            Robot robot = new Robot();
            if (controlRobot.Value == 1)
                robot = roboteam.Robot1;
            else if (controlRobot.Value == 2)
                robot = roboteam.Robot2;
            if (robot.Action == Routine.Task.AXS)
                robot.TAxs.A = controlA.Value;
            else
                robot.TPos.A = controlA.Value;
        }

        private void controlB_ValueChanged(object sender, RoutedEventArgs e)
        {
            Robot robot = new Robot();
            if (controlRobot.Value == 1)
                robot = roboteam.Robot1;
            else if (controlRobot.Value == 2)
                robot = roboteam.Robot2;
            if (robot.Action == Routine.Task.AXS)
                robot.TAxs.B = controlB.Value;
            else
                robot.TPos.B = controlB.Value;
        }

        private void controlC_ValueChanged(object sender, RoutedEventArgs e)
        {
            Robot robot = new Robot();
            if (controlRobot.Value == 1)
                robot = roboteam.Robot1;
            else if (controlRobot.Value == 2)
                robot = roboteam.Robot2;
            if (robot.Action == Routine.Task.AXS)
                robot.TAxs.C = controlC.Value;
            else
                robot.TPos.C = controlC.Value;
        }

        private void ViewRobotData(Roboteam robotteam, uint index)
        {
            //Get selected robot
            Robot robot = new Robot();
            if (index == 1)
                robot = roboteam.Robot1;
            else if (index == 2)
                robot = roboteam.Robot2;

            //Connection and Position
            if (robot.Status == Robot.ConnectionStatus.Connected)
            {
                buttonRobotConnect.SetStatus(UserControlConnectionButton.Status.Connected);
                labelRobotVelocity.Content = "Velocity: " + robot.Vel;
            }
            else if (robot.Status == Robot.ConnectionStatus.Connecting)
            {
                buttonRobotConnect.SetStatus(UserControlConnectionButton.Status.Connecting);
            }
            else
            {
                buttonRobotConnect.SetStatus(UserControlConnectionButton.Status.Disconnected);
            }

            //Position
            if (radioButtonRobotControlCartesian.IsChecked == true)
            {
                controlX.SetValue(robot.TPos.X);
                controlY.SetValue(robot.TPos.Y);
                controlZ.SetValue(robot.TPos.Z);
                controlA.SetValue(robot.TPos.A);
                controlB.SetValue(robot.TPos.B);
                controlC.SetValue(robot.TPos.C);
            }
            else if (radioButtonRobotControlAxis.IsChecked == true)
            {
                controlX.SetValue(robot.TAxs.X);
                controlY.SetValue(robot.TAxs.Y);
                controlZ.SetValue(robot.TAxs.Z);
                controlA.SetValue(robot.TAxs.A);
                controlB.SetValue(robot.TAxs.B);
                controlC.SetValue(robot.TAxs.C);
            }

            //Program
            List<string> itemCollectionRobotProgram = new List<string>();
            foreach (Routine.Command command in robot.Program.Commands)
            {
                itemCollectionRobotProgram.Add(command.Task.ToString() + "   X " + command.Values.X.ToString("0.00") + ", Y " + command.Values.Y.ToString("0.00") + ", Z " + command.Values.Z.ToString("0.00") + ", A " + command.Values.A.ToString("0.00") + ", B " + command.Values.B.ToString("0.00") + ", C " + command.Values.C.ToString("0.00"));
            }
            listBoxRobotProgram.ItemsSource = itemCollectionRobotProgram;
            listBoxRobotProgram.SelectedIndex = selecetedCommand;
            if (robot.Program.Commands.Count() == 0)
            {
                buttonRobotProgramPlay.IsEnabled = false;
                play = false;
            }
            else
                buttonRobotProgramPlay.IsEnabled = true;
            if (play)
            {
                imageRobotProgramPlay.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/pauseicon.png"));
                buttonRobotProgramOpen.IsEnabled = false;
                buttonRobotProgramSave.IsEnabled = false;
                buttonRobotProgramClose.IsEnabled = false;
                buttonRobotProgramPositionAdd.IsEnabled = false;
                buttonRobotProgramPositionCopy.IsEnabled = false;
                buttonRobotProgramPositionUp.IsEnabled = false;
                buttonRobotProgramPositionDown.IsEnabled = false;
                buttonRobotProgramPositionDelete.IsEnabled = false;
                buttonRobotProgramPositionConfirm.Visibility = System.Windows.Visibility.Collapsed;
                lineRobotProgramPosition.Visibility = System.Windows.Visibility.Collapsed;
                groupBoxRobotProgramPosition.Height = 0;
            }
            else
            {
                imageRobotProgramPlay.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/playicon.png"));
                buttonRobotProgramOpen.IsEnabled = true;
                buttonRobotProgramSave.IsEnabled = true;
                buttonRobotProgramClose.IsEnabled = true;
                buttonRobotProgramPositionAdd.IsEnabled = true;
                buttonRobotProgramPositionCopy.IsEnabled = true;
                buttonRobotProgramPositionUp.IsEnabled = true;
                buttonRobotProgramPositionDown.IsEnabled = true;
                buttonRobotProgramPositionDelete.IsEnabled = true;
            }
        }

        private void buttonGamepad_Click(object sender, RoutedEventArgs e)
        {
            if (gamepad.Connected)
                gamepad.Disconnect();
            else
                gamepad.Connect(Properties.Settings.Default.GamepadIndex);
        }

        private void buttonGamepadSettings_Click(object sender, RoutedEventArgs e)
        {
            WindowGamepadSettings windowGamepadSettings = new WindowGamepadSettings();
            windowGamepadSettings.ShowDialog();
        }

        private void buttonRobotProgramOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.RestoreDirectory = true;
            dialog.Title = "Open program";
            dialog.Filter = "src files (*.src)|*.src|crp files (*.crp)|*.crp|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() == true)
            {
                if (controlRobot.Value == 1)
                    roboteam.Robot1.Program.Load(dialog.FileName);
                else if (controlRobot.Value == 2)
                    roboteam.Robot2.Program.Load(dialog.FileName);
                selecetedCommand = -1;
            }
        }

        private void buttonRobotProgramSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.RestoreDirectory = true;
            dialog.Title = "Save program";
            dialog.Filter = "src files (*.src)|*.src|crp files (*.crp)|*.crp|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() == true)
            {
                if (controlRobot.Value == 1)
                    roboteam.Robot1.Program.Save(dialog.FileName);
                else if (controlRobot.Value == 2)
                    roboteam.Robot2.Program.Save(dialog.FileName);
            }
        }

        private void buttonRobotProgramClose_Click(object sender, RoutedEventArgs e)
        {
            if (controlRobot.Value == 1)
                roboteam.Robot1.Program.Clear();
            else if (controlRobot.Value == 2)
                roboteam.Robot2.Program.Clear();
        }

        private void buttonRobotProgramPlayModus_Click(object sender, RoutedEventArgs e)
        {
            switch (roboteam.Mode)
            {
                case Roboteam.Playmode.Gradual:
                    roboteam.Mode = Roboteam.Playmode.Complete;
                    imageRobotProgramPlayMode.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/completeicon.png"));
                    break;
                case Roboteam.Playmode.Complete:
                    roboteam.Mode = Roboteam.Playmode.Endless;
                    imageRobotProgramPlayMode.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/endlessicon.png"));
                    break;
                case Roboteam.Playmode.Endless:
                    roboteam.Mode = Roboteam.Playmode.Gradual;
                    imageRobotProgramPlayMode.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/gradualicon.png"));
                    break;
            }
        }

        private void buttonRobotProgramPlay_Click(object sender, RoutedEventArgs e)
        {
            play = !play;
            if (play)
            {
                roboteam.ProgramIndex = selecetedCommand;
                roboteam.RunPrograms();
            }
            else
            {
                roboteam.StopPrograms();
            }
        }

        private void buttonRobotProgramPositionConfirm_Click(object sender, RoutedEventArgs e)
        {
            int taskIndex = comboBoxRobotProgramTask.SelectedIndex;
            Routine.Command command = new Routine.Command((Routine.Task)taskIndex, new IRob.Geometry.Position(controlProgramX.Value, controlProgramY.Value, controlProgramZ.Value, controlProgramA.Value, controlProgramB.Value, controlProgramC.Value));
            if (controlRobot.Value == 1)
            {
                roboteam.Robot1.Program.RemoveCommand(selecetedCommand);
                roboteam.Robot1.Program.InsertCommand(selecetedCommand, command);
            }
            else if (controlRobot.Value == 2)
            {
                roboteam.Robot2.Program.RemoveCommand(selecetedCommand);
                roboteam.Robot2.Program.InsertCommand(selecetedCommand, command);
            }
            selecetedCommand = -1;
        }

        private void buttonRobotProgramPositionAdd_Click(object sender, RoutedEventArgs e)
        {
            Robot robot = new Robot();
            if (controlRobot.Value == 1)
                robot = roboteam.Robot1;
            else if (controlRobot.Value == 2)
                robot = roboteam.Robot2;
            Routine.Command command = new Routine.Command(Routine.Task.PTP, new IRob.Geometry.Position(0, 0, 0, 0, 0, 0));
            if (selecetedCommand != -1)
            {
                robot.Program.InsertCommand(selecetedCommand, command);
                ViewRobotProgramPosition(robot);
            }
            else
            {
                robot.Program.AddCommand(command);
                selecetedCommand = robot.Program.Commands.Count() - 1;
                ViewRobotProgramPosition(robot);
            }
        }

        private void buttonRobotProgramPositionCopy_Click(object sender, RoutedEventArgs e)
        {
            if (selecetedCommand != -1)
            {
                if (controlRobot.Value == 1)
                    roboteam.Robot1.Program.CopyCommand(listBoxRobotProgram.SelectedIndex);
                else if (controlRobot.Value == 2)
                    roboteam.Robot2.Program.CopyCommand(listBoxRobotProgram.SelectedIndex);
                selecetedCommand = selecetedCommand + 1;
            }
        }

        private void buttonRobotProgramPositionUp_Click(object sender, RoutedEventArgs e)
        {
            if (selecetedCommand != -1)
            {
                if (controlRobot.Value == 1 && selecetedCommand > 0)
                {
                    roboteam.Robot1.Program.SwapCommand(selecetedCommand - 1, selecetedCommand);
                    selecetedCommand--;
                }
                else if (controlRobot.Value == 2 && selecetedCommand > 0)
                {
                    roboteam.Robot2.Program.SwapCommand(selecetedCommand - 1, selecetedCommand);
                    selecetedCommand--;
                }
            }
        }

        private void buttonRobotProgramPositionDown_Click(object sender, RoutedEventArgs e)
        {
            if (selecetedCommand != -1)
            {
                if (controlRobot.Value == 1 && selecetedCommand < roboteam.Robot1.Program.Commands.Count() - 1)
                {
                    roboteam.Robot1.Program.SwapCommand(selecetedCommand, selecetedCommand + 1);
                    selecetedCommand++;
                }
                else if (controlRobot.Value == 2 && selecetedCommand < roboteam.Robot2.Program.Commands.Count() - 1)
                {
                    roboteam.Robot2.Program.SwapCommand(selecetedCommand, selecetedCommand + 1);
                    selecetedCommand++;
                }
            }
        }

        private void buttonRobotProgramPositionDelete_Click(object sender, RoutedEventArgs e)
        {
            if (controlRobot.Value == 1)
                roboteam.Robot1.Program.RemoveCommand(selecetedCommand);
            else if (controlRobot.Value == 2)
                roboteam.Robot2.Program.RemoveCommand(selecetedCommand);
            selecetedCommand = -1;
        }

        private void buttonRobotPosition_Click(object sender, RoutedEventArgs e)
        {
            controlProgramX.SetValue(controlX.Value);
            controlProgramY.SetValue(controlY.Value);
            controlProgramZ.SetValue(controlZ.Value);
            controlProgramA.SetValue(controlA.Value);
            controlProgramB.SetValue(controlB.Value);
            controlProgramC.SetValue(controlC.Value);
        }

        private void listBoxRobotProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!play)
            {
                if (listBoxRobotProgram.SelectedIndex != -1)
                {
                    if (selecetedCommand != listBoxRobotProgram.SelectedIndex)
                    {
                        selecetedCommand = listBoxRobotProgram.SelectedIndex;
                        if (controlRobot.Value == 1)
                            ViewRobotProgramPosition(roboteam.Robot1);
                        else if (controlRobot.Value == 2)
                            ViewRobotProgramPosition(roboteam.Robot2);
                    }
                }
                else
                {
                    buttonRobotProgramPositionConfirm.Visibility = System.Windows.Visibility.Collapsed;
                    lineRobotProgramPosition.Visibility = System.Windows.Visibility.Collapsed;
                    groupBoxRobotProgramPosition.Height = 0;
                }
            }
            else
            {
                selecetedCommand = listBoxRobotProgram.SelectedIndex;
            }
        }

        private void ViewRobotProgramPosition(Robot robot)
        {
            comboBoxRobotProgramTask.SelectedIndex = (int)robot.Program.Commands[selecetedCommand].Task;
            controlProgramX.SetValue(robot.Program.Commands[selecetedCommand].Values.X);
            controlProgramY.SetValue(robot.Program.Commands[selecetedCommand].Values.Y);
            controlProgramZ.SetValue(robot.Program.Commands[selecetedCommand].Values.Z);
            controlProgramA.SetValue(robot.Program.Commands[selecetedCommand].Values.A);
            controlProgramB.SetValue(robot.Program.Commands[selecetedCommand].Values.B);
            controlProgramC.SetValue(robot.Program.Commands[selecetedCommand].Values.C);
            buttonRobotProgramPositionConfirm.Visibility = System.Windows.Visibility.Visible;
            lineRobotProgramPosition.Visibility = System.Windows.Visibility.Visible;
            groupBoxRobotProgramPosition.Height = Double.NaN;
        }

        #endregion

        #region Sensor

        private void buttonSensor_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDevice == device.Sensor)
                selectedDevice = device.None;
            else
                selectedDevice = device.Sensor;
        }

        private void controlDropDownSensorControl_Changed(object sender, RoutedEventArgs e)
        {
            if (controlDropDownSensorControl.Extended)
                groupBoxSensorControl.Height = Double.NaN;
            else
                groupBoxSensorControl.Height = 0;
        }

        private void controlDropDownSensorMeasurement_Changed(object sender, RoutedEventArgs e)
        {
            if (controlDropDownSensorMeasurement.Extended)
                groupBoxSensorMeasurement.Height = Double.NaN;
            else
                groupBoxSensorMeasurement.Height = 0;
        }

        private void buttonSensorConnect_Click(object sender, RoutedEventArgs e)
        {
            if (sensor.Status == Scanner.ConnectionStatus.Disconnected)
            {
                sensor.Connect();
            }
            else
                sensor.Disconnect();
        }

        private void buttonSensorProfileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Save Profile";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                sensor.SaveProfile(saveFileDialog.FileName);
        }

        private void controlShuttertime_ValueChanged(object sender, RoutedEventArgs e)
        {
            sensor.SetShutterTime(Convert.ToUInt16(controlShuttertime.Value));
        }



        #endregion

        #region Workpiece

        private void buttonWorkpiece_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDevice == device.Workpiece)
                selectedDevice = device.None;
            else
                selectedDevice = device.Workpiece;
        }

        #endregion

        #region Pump

        private void buttonPump_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDevice == device.Pump)
                selectedDevice = device.None;
            else
                selectedDevice = device.Pump;
        }

        private void buttonPumpConnect_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion



    }
}
