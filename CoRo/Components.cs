using System;
using Kitware.VTK;
using IRob;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoRo
{
    class MySimulation : Simulation
    {
        public List<Position> kinematic(Geometry.Position robotBase, Geometry.Position angles)
        {
            //Get axis points
            List<Position> positions = new List<Position>();
            Point prior = new Point(robotBase.X, robotBase.Y, robotBase.Z);
            Position tempPosition;
            Matrix4 tempMatrix = new Matrix4();

            Matrix4 A1 = GetLinkTransformation(new Link(-angles.X * Math.PI / 180, 400, 25, -Math.PI / 2));
            Matrix4 A2 = GetLinkTransformation(new Link(angles.Y * Math.PI / 180, 0, 455, 0));
            Matrix4 A3 = GetLinkTransformation(new Link(angles.Z * Math.PI / 180 - Math.PI / 2, 0, 35, -Math.PI / 2));
            Matrix4 A4 = GetLinkTransformation(new Link(-angles.A * Math.PI / 180, 420, 0, Math.PI / 2));
            Matrix4 A5 = GetLinkTransformation(new Link(angles.B * Math.PI / 180, 0, 0, -Math.PI / 2));
            Matrix4 A6 = GetLinkTransformation(new Link(angles.C * Math.PI / 180, 80, 0, 0));

            tempPosition = GetLinkTransformationPosition(A1);
            tempPosition.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPosition.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            positions.Add(tempPosition);

            Matrix4 sumMatrix = new Matrix4();
            sumMatrix.Multiply(A1, A2);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPosition.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A3);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPosition.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A4);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPosition.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A5);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPosition.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A6);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPosition.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            positions.Add(tempPosition);

            return positions;
        }
    }

    class RobotModel : Simulation
    {
        vtkActor stl00;
        vtkActor stl01;
        vtkActor stl02;
        vtkActor stl03;
        vtkActor stl04;
        vtkActor stl05;

        vtkActor point00;
        vtkActor point01;
        vtkActor point02;
        vtkActor point03;
        vtkActor point04;
        vtkActor point05;

        vtkActor line00;
        vtkActor line01;
        vtkActor line02;
        vtkActor line03;
        vtkActor line04;

        vtkAxesActor axes00;
        vtkAxesActor axes01;
        vtkAxesActor axes02;
        vtkAxesActor axes03;
        vtkAxesActor axes04;
        vtkAxesActor axes05;

        MySimulation simulation = new MySimulation();
        VTKComponents myVtk = new VTKComponents();
        List<Position> positionList;

        IRob.Geometry.Position robotBase;
        IRob.Geometry.Position angles;

        vtkAssembly stlAssembly;
        public vtkAssembly getStlModel
        {
            get
            {
                return stlAssembly;
            }
        }

        vtkAssembly lineAssembly;
        public vtkAssembly getLineModel
        {
            get { return lineAssembly; }
        }

        vtkAssembly pointAssembly;
        public vtkAssembly getPointModel
        {
            get { return pointAssembly; }
        }

        vtkAssembly axesAssembly;
        public vtkAssembly getAxesModel
        {
            get { return axesAssembly; }
        }

        public void rotateA1(double value)
        {
            angles.X += value;
            //angles.Y += value;
            //angles.Z += value;
            //angles.A += value;
            //angles.B += value;
            //angles.C += value;

            positionList = simulation.kinematic(robotBase, angles);

            #region Axes
            axesAssembly.RemovePart(axes00);
            axes00 = myVtk.drawCoordinates(positionList[0]);
            axesAssembly.AddPart(axes00);
            
            axesAssembly.RemovePart(axes01);
            axes01 = myVtk.drawCoordinates(positionList[1]);
            axesAssembly.AddPart(axes01);

            axesAssembly.RemovePart(axes02);
            axes02 = myVtk.drawCoordinates(positionList[2]);
            axesAssembly.AddPart(axes02);

            axesAssembly.RemovePart(axes03);
            axes03 = myVtk.drawCoordinates(positionList[3]);
            axesAssembly.AddPart(axes03);

            axesAssembly.RemovePart(axes04);
            axes04 = myVtk.drawCoordinates(positionList[4]);
            axesAssembly.AddPart(axes04);

            axesAssembly.RemovePart(axes05);
            axes05 = myVtk.drawCoordinates(positionList[5]);
            axesAssembly.AddPart(axes05);
             
            #endregion

            #region Points
            point00.SetPosition(positionList[0].X, positionList[0].Y, positionList[0].Z);
            point01.SetPosition(positionList[1].X, positionList[1].Y, positionList[1].Z);
            point02.SetPosition(positionList[2].X, positionList[2].Y, positionList[2].Z);
            point03.SetPosition(positionList[3].X, positionList[3].Y, positionList[3].Z);
            point04.SetPosition(positionList[4].X, positionList[4].Y, positionList[4].Z);
            point05.SetPosition(positionList[5].X, positionList[5].Y, positionList[5].Z);
            #endregion

            #region Lines
            lineAssembly.RemovePart(line00);
            line00 = myVtk.drawLines(positionList[0].X, positionList[0].Y, positionList[0].Z, positionList[1].X, positionList[1].Y, positionList[1].Z);
            lineAssembly.AddPart(line00);

            lineAssembly.RemovePart(line01);
            line01 = myVtk.drawLines(positionList[1].X, positionList[1].Y, positionList[1].Z, positionList[2].X, positionList[2].Y, positionList[2].Z);
            lineAssembly.AddPart(line01);

            lineAssembly.RemovePart(line02);
            line02 = myVtk.drawLines(positionList[2].X, positionList[2].Y, positionList[2].Z, positionList[3].X, positionList[3].Y, positionList[3].Z);
            lineAssembly.AddPart(line02);

            lineAssembly.RemovePart(line03);
            line03 = myVtk.drawLines(positionList[3].X, positionList[3].Y, positionList[3].Z, positionList[4].X, positionList[4].Y, positionList[4].Z);
            lineAssembly.AddPart(line03);

            lineAssembly.RemovePart(line04);
            line04 = myVtk.drawLines(positionList[4].X, positionList[4].Y, positionList[4].Z, positionList[5].X, positionList[5].Y, positionList[5].Z);
            lineAssembly.AddPart(line04);
            #endregion

            /*
            stl01.SetPosition(-robotBase.X, -robotBase.Y, -robotBase.Z);
            stl02.SetPosition(-robotBase.X - 25, -robotBase.Y, -robotBase.Z - 400);
            stl03.SetPosition(-robotBase.X - 25, -robotBase.Y, -robotBase.Z - 855);
            stl04.SetPosition(-robotBase.X, -robotBase.Y, robotBase.Z - 890);
            stl05.SetPosition(-robotBase.X - 445, -robotBase.Y, -robotBase.Z - 890);


            stl01.SetOrigin(robotBase.X, robotBase.Y, robotBase.Z);
            stl02.SetOrigin(robotBase.X + 25, robotBase.Y, robotBase.Z + 400);
            stl03.SetOrigin(robotBase.X + 25, robotBase.Y, robotBase.Z + 855);
            stl04.SetOrigin(robotBase.X, robotBase.Y, robotBase.Z + 890);
            stl05.SetOrigin(robotBase.X + 445, robotBase.Y, robotBase.Z + 890);


            stl01.RotateZ(value);
            stl02.RotateY(value);
            stl03.RotateY(value);
            stl04.RotateX(value);
            stl05.RotateY(value);


            stl01.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);

            stl02.SetPosition(robotBase.X + positionList[0].X - 25, robotBase.Y + positionList[0].Y, robotBase.Z + positionList[0].Z - 400);
            stl02.RotateWXYZ(value, 0, 0, 1);


            stl03.SetPosition(robotBase.X + positionList[0].X - 25, robotBase.Y + positionList[0].Y, robotBase.Z + positionList[0].Z - 400);
            stl03.RotateWXYZ(value, 0, 0, 1);
            stl03.SetPosition(robotBase.X + positionList[1].X - 25, robotBase.Y + positionList[1].Y, robotBase.Z + positionList[1].Z - 855);
            stl03.RotateY(value);
             * */
        }

        public RobotModel(Geometry.Position robotBase, Geometry.Position angles)
        {
            this.robotBase = robotBase;
            this.angles = angles;

            stlAssembly = vtkAssembly.New();
            pointAssembly = vtkAssembly.New();
            lineAssembly = vtkAssembly.New();
            axesAssembly = vtkAssembly.New();

            positionList = new List<Position>();

            positionList = simulation.kinematic(robotBase, angles);
            
            #region STL
            stl00 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_00.stl"));
            stl01 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_01.stl"));
            stl02 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_02.stl"));
            stl03 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_03.stl"));
            stl04 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_04.stl"));
            stl05 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_05.stl"));

            stl00.GetProperty().SetColor(0.3, 0.3, 0.3);

            stl00.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl01.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl02.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl03.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl04.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl05.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);

            stlAssembly.AddPart(stl00);
            stlAssembly.AddPart(stl01);
            stlAssembly.AddPart(stl02);
            stlAssembly.AddPart(stl03);
            stlAssembly.AddPart(stl04);
            stlAssembly.AddPart(stl05);
            #endregion
            
            #region Points
            point00 = myVtk.drawPoint(positionList[0].X, positionList[0].Y, positionList[0].Z);
            point01 = myVtk.drawPoint(positionList[1].X, positionList[1].Y, positionList[1].Z);
            point02 = myVtk.drawPoint(positionList[2].X, positionList[2].Y, positionList[2].Z);
            point03 = myVtk.drawPoint(positionList[3].X, positionList[3].Y, positionList[3].Z);
            point04 = myVtk.drawPoint(positionList[4].X, positionList[4].Y, positionList[4].Z);
            point05 = myVtk.drawPoint(positionList[5].X, positionList[5].Y, positionList[5].Z);

            pointAssembly.AddPart(point00);
            pointAssembly.AddPart(point01);
            pointAssembly.AddPart(point02);
            pointAssembly.AddPart(point03);
            pointAssembly.AddPart(point04);
            pointAssembly.AddPart(point05);
            #endregion
            
            #region Axes
            axes00 = myVtk.drawCoordinates(positionList[0].X, positionList[0].Y, positionList[0].Z);
            axes01 = myVtk.drawCoordinates(positionList[1].X, positionList[1].Y, positionList[1].Z);
            axes02 = myVtk.drawCoordinates(positionList[2].X, positionList[2].Y, positionList[2].Z);
            axes03 = myVtk.drawCoordinates(positionList[3].X, positionList[3].Y, positionList[3].Z);
            axes04 = myVtk.drawCoordinates(positionList[4].X, positionList[4].Y, positionList[4].Z);
            axes05 = myVtk.drawCoordinates(positionList[5].X, positionList[5].Y, positionList[5].Z);

            axesAssembly.AddPart(axes00);
            axesAssembly.AddPart(axes01);
            axesAssembly.AddPart(axes02);
            axesAssembly.AddPart(axes03);
            axesAssembly.AddPart(axes04);
            axesAssembly.AddPart(axes05);
            #endregion
            
            #region Lines
            line00 = myVtk.drawLines(positionList[0].X, positionList[0].Y, positionList[0].Z, positionList[1].X, positionList[1].Y, positionList[1].Z);
            line01 = myVtk.drawLines(positionList[1].X, positionList[1].Y, positionList[1].Z, positionList[2].X, positionList[2].Y, positionList[2].Z);
            line02 = myVtk.drawLines(positionList[2].X, positionList[2].Y, positionList[2].Z, positionList[3].X, positionList[3].Y, positionList[3].Z);
            line03 = myVtk.drawLines(positionList[3].X, positionList[3].Y, positionList[3].Z, positionList[4].X, positionList[4].Y, positionList[4].Z);
            line04 = myVtk.drawLines(positionList[4].X, positionList[4].Y, positionList[4].Z, positionList[5].X, positionList[5].Y, positionList[5].Z);

            lineAssembly.AddPart(line00);
            lineAssembly.AddPart(line01);
            lineAssembly.AddPart(line02);
            lineAssembly.AddPart(line03);
            lineAssembly.AddPart(line04);
            #endregion

            //draw RobotBase point
            /*
            vtkActor robotBasePoint = myVtk.drawPoint(robotBase.X, robotBase.Y, robotBase.Z);
            robotBasePoint.GetProperty().SetColor(255, 0, 0);
            actorList.Add(robotBasePoint);
            */

            /*
            vtkSTLWriter stlWriter = vtkSTLWriter.New();
            stlWriter.SetFileName("U:\\RobSim\\CoRo\\bin\\Debug\\Robot\\test.stl");
            stlWriter.SetInput(stl01.GetMapper().GetInput());
            stlWriter.Write();
            */
        }
    }

    class VTKComponents
    {
        public vtkAxesActor axes;

        public vtkActor floor;

        public vtkAxesActor drawCoordinates()
        {
            //vtkRenderer Renderer = ((MainWindow)System.Windows.Application.Current.MainWindow).renderControl.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkTransform transform = new vtkTransform();
            transform.Translate(-500, 0.0, 0.0);
            axes = new vtkAxesActor();
            axes.SetUserTransform(transform);
            axes.SetTotalLength(200, 200, 200);
            axes.AxisLabelsOff();
            return axes;
        }

        public vtkAxesActor drawCoordinates(double x, double y, double z)
        {
            //vtkRenderer Renderer = ((MainWindow)System.Windows.Application.Current.MainWindow).renderControl.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkTransform transform = new vtkTransform();
            transform.Translate(x, y, z);
            vtkAxesActor axes = new vtkAxesActor();
            axes.SetUserTransform(transform);
            axes.SetTotalLength(100, 100, 100);
            axes.AxisLabelsOff();
            return axes;
        }

        public vtkAxesActor drawCoordinates(double x, double y, double z, double a, double b, double c)
        {
            //vtkRenderer Renderer = ((MainWindow)System.Windows.Application.Current.MainWindow).renderControl.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkTransform transform = new vtkTransform();
            transform.Translate(x, y, z);
            transform.RotateWXYZ(c, 1, 0, 0);
            transform.RotateWXYZ(b, 0, 1, 0);
            transform.RotateWXYZ(a, 0, 0, 1);

            vtkAxesActor axes = new vtkAxesActor();
            axes.SetUserTransform(transform);
            axes.SetTotalLength(100, 100, 100);
            axes.AxisLabelsOff();
            return axes;
        }

        public vtkAxesActor drawCoordinates(Geometry.Position position)
        {
            //vtkRenderer Renderer = ((MainWindow)System.Windows.Application.Current.MainWindow).renderControl.RenderWindow.GetRenderers().GetFirstRenderer();
            vtkAxesActor axes = drawCoordinates(position.X, position.Y, position.Z, position.A, position.B, position.C);
            return axes;
        }

        public vtkActor drawFloor()
        {
            vtkPlaneSource planeSource = vtkPlaneSource.New();
            planeSource.SetOrigin(-1000.0, -1000.0, -20.0);
            planeSource.SetPoint1(-1000.0, 1000.0, -20.0);
            planeSource.SetPoint2(1000.0, -1000.0, -20.0);
            planeSource.Update();
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            vtkPolyData plane = planeSource.GetOutput();
            mapper.SetInput(plane);
            floor = vtkActor.New();
            floor.SetMapper(mapper);
            floor.SetPosition(0, 0, 0);
            floor.GetProperty().SetColor(0.8, 0.8, 0.8);
            return floor;
        }

        public vtkActor drawPoint(double x, double y, double z)
        {
            vtkPoints vtkPoints = vtkPoints.New();
            double[,] p = new double[,] { { 0, 0, 0 } };
            vtkCellArray vertices = vtkCellArray.New();
            int nPts = 1;
            int[] ids = new int[nPts];
            for (int i = 0; i < nPts; i++)
            {
                ids[i] = vtkPoints.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);
            }

            int size = Marshal.SizeOf(typeof(int)) * nPts;
            IntPtr pIds = Marshal.AllocHGlobal(size);
            Marshal.Copy(ids, 0, pIds, nPts);
            vertices.InsertNextCell(nPts, pIds);
            Marshal.FreeHGlobal(pIds);
            vtkPolyData pointsAndLinesPoly = vtkPolyData.New();
            pointsAndLinesPoly.SetPoints(vtkPoints);
            pointsAndLinesPoly.SetVerts(vertices);
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(pointsAndLinesPoly);
            vtkActor pointActor = vtkActor.New();
            pointActor.SetPosition(x, y, z);
            pointActor.SetMapper(mapper);
            pointActor.GetProperty().SetPointSize(10);

            return pointActor;
        }

        public vtkActor drawLines(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            vtkPolyData pointsAndLinesPoly = vtkPolyData.New();
            vtkPoints pts = new vtkPoints();
            pts.InsertPoint(0, x1, y1, z1);
            pts.InsertPoint(1, x2, y2, z2);
            pointsAndLinesPoly.SetPoints(pts);
            vtkLine line0 = new vtkLine();
            line0.GetPointIds().SetId(0, 0);
            line0.GetPointIds().SetId(1, 1);
            vtkCellArray lines = new vtkCellArray();
            lines.InsertNextCell(line0);
            pointsAndLinesPoly.SetLines(lines);
            vtkPolyDataMapper mapper2 = vtkPolyDataMapper.New();
            mapper2.SetInput(pointsAndLinesPoly);
            vtkActor lineActor = new vtkActor();
            lineActor.SetMapper(mapper2);
            return lineActor;
        }

        public vtkActor readSTL(string filePath)
        {
            vtkSTLReader reader = vtkSTLReader.New();
            reader.SetFileName(filePath);
            reader.Update();
            vtkMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());
            vtkActor actorSTL = vtkActor.New();
            actorSTL.SetMapper(mapper);
            actorSTL.GetProperty().SetColor(2.55, 1.28, 0);


            return actorSTL;
        }
    }
}
