using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using IRob;
using Kitware.VTK;
using Microsoft.Research.DynamicDataDisplay.DataSources;


namespace CoRo
{

    public partial class MainWindow : Window
    {
        Color colorBackground = Color.FromArgb(255, 50, 50, 50);
        Color colorBackground2 = Color.FromArgb(255, 100, 100, 100);
        bool showCellBase = true, showCellFloor = true;

        public Robot robotWorkpiece = new Robot();
        List<IRob.Geometry.Position> transformations = new List<IRob.Geometry.Position>();


        private readonly BackgroundWorker worker = new BackgroundWorker();
        bool update = true;

        Scanner sensor = new Scanner();
        EnumerableDataSource<IRob.Geometry.Point> dataSource;

        public enum device { Robot, Sensor, None };
        private device Device = device.None;
        public device selectedDevice
        {
            get { return Device; }
            set
            {
                Device = value;
                switch (Device)
                {
                    case device.None:
                        deviceColumn.Width = new GridLength(10, GridUnitType.Pixel);
                        break;
                    case device.Robot:
                        tabControl.SelectedIndex = 0;
                        deviceColumn.Width = new GridLength(deviceColumn.MaxWidth, GridUnitType.Pixel);
                        break;
                    case device.Sensor:
                        tabControl.SelectedIndex = 1;
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

            Rect domainRect = new Rect(-75, 120, 150, 150);
            plotter.Viewport.Domain = domainRect;
            plotter.LegendVisible = false;
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (update)
            {
                System.Windows.Threading.Dispatcher dispatcher = buttonSensorConnect.Dispatcher;

                switch (sensor.Status)
                {
                    case Scanner.ConnectionStatus.Disconnected:
                        dispatcher.BeginInvoke(new Action(() => buttonSensorConnect.SetStatus(UserControlConnectionButton.Status.Disconnected)));
                        break;
                    case Scanner.ConnectionStatus.Connecting:
                        dispatcher.BeginInvoke(new Action(() => buttonSensorConnect.SetStatus(UserControlConnectionButton.Status.Connecting)));
                        break;
                    case Scanner.ConnectionStatus.Connected:
                        dispatcher.BeginInvoke(new Action(() => buttonSensorConnect.SetStatus(UserControlConnectionButton.Status.Connected)));
                        dataSource = new EnumerableDataSource<IRob.Geometry.Point>(sensor.Profile.Select(x => x.Point));
                        dataSource.SetXMapping(x => x.X);
                        dataSource.SetYMapping(z => z.Z);
                        dispatcher.BeginInvoke(new Action(() => linegraph.DataSource = dataSource));
                        break;
                    case Scanner.ConnectionStatus.Error:
                        dispatcher.BeginInvoke(new Action(() => buttonSensorConnect.SetStatus(UserControlConnectionButton.Status.Error)));
                        break;
                }

                Thread.Sleep(100);
            }
        }

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

        private void buttonRobot_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDevice == device.Robot)
                selectedDevice = device.None;
            else
                selectedDevice = device.Robot;
        }

        private void buttonSensor_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDevice == device.Sensor)
                selectedDevice = device.None;
            else
                selectedDevice = device.Sensor;
        }


        Components components;
        Robot robot = new Robot();
        Robot robot2 = new Robot();
        vtkRenderer renderer;
        myRobot myRobot;
        myRobot myRobot2;
        vtkAssembly myRobotAssembly;
        vtkAssembly myRobotAssembly2;

        private void WFHost_Loaded(object sender, RoutedEventArgs e)
        {


            renderer = renderControl.RenderWindow.GetRenderers().GetFirstRenderer();
            renderer.GradientBackgroundOn();
            renderer.SetBackground((double)colorBackground.R / 255, (double)colorBackground.G / 255, (double)colorBackground.B / 255);
            renderer.SetBackground2((double)colorBackground2.R / 255, (double)colorBackground2.G / 255, (double)colorBackground2.B / 255);
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);

            components = new Components();

            renderer.AddActor(components.drawFloor());
            renderer.AddActor(components.drawCoordinates());


            robot.Pos.X = 0;
            robot.Pos.Y = 0;
            robot.Pos.Z = 0;
            robot.Pos.A = 0;
            robot.Pos.B = 0;
            robot.Pos.C = 0;

            robot.Axs.X = 0;
            robot.Axs.Y = -90;
            robot.Axs.Z = 90;
            robot.Axs.A = 0;
            robot.Axs.B = 0;
            robot.Axs.C = 0;

            myRobot = new myRobot(robot.Pos, robot.Axs);
            myRobotAssembly = myRobot.getRobotModel;
            renderer.AddActor(myRobotAssembly);

            /*
            myRobot = new myRobot(robot.Pos, robot.Axs);
            myRobotAssembly = myRobot.drawRob();
            renderer.AddActor(myRobotAssembly);

            /*
            robot2.Pos.X = 300;
            robot2.Pos.Y = 300;
            robot2.Pos.Z = 0;
            robot2.Pos.A = 0;
            robot2.Pos.B = 0;
            robot2.Pos.C = 0;

            robot2.Axs.X = 70; //A1
            robot2.Axs.Y = -90; //A2
            robot2.Axs.Z = 90; //A3
            robot2.Axs.A = 0; //A4
            robot2.Axs.B = 0; //A5
            robot2.Axs.C = 0;  //A6
            
            myRobot2 = new myRobot(robot2.Pos, robot2.Axs);
            myRobotAssembly2 = myRobot2.drawRob();
            renderer.AddActor(myRobotAssembly2);
            */

            renderer.ResetCamera();
        }


        private void OnShowCellBaseClick(object sender, RoutedEventArgs e)
        {
            showCellBase = !showCellBase;

            if (showCellBase)
                components.axes.SetVisibility(1);
            else
                components.axes.SetVisibility(0);

            renderControl.RenderWindow.GetInteractor().Render();
        }

        private void OnShowCellFloorClick(object sender, RoutedEventArgs e)
        {
            showCellFloor = !showCellFloor;

            if (showCellFloor)
                components.floor.SetVisibility(1);

            else
                components.floor.SetVisibility(0);

            renderControl.RenderWindow.GetInteractor().Render();
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

        private void controlShuttertime_ValueChanged(object sender, RoutedEventArgs e)
        {
            sensor.SetShutterTime(Convert.ToUInt16(controlShuttertime.Value));
        }

        private void buttonRobotConnect_Click(object sender, RoutedEventArgs e)
        {

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            renderer.RemoveActor(myRobotAssembly);

            
            myRobot.rotateA1(-10);
            renderer.AddActor(myRobot.getRobotModel);
            renderControl.Refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //for (double i = 0; i < 10; i = i + 1)
            //{
                renderer.RemoveActor(myRobotAssembly);

                myRobot.rotateA1(10);
                renderer.AddActor(myRobot.getRobotModel);

                renderControl.Refresh();
            //}
            
        }

    }

}
