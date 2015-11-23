using System;
using Kitware.VTK;
using IRob;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoRo
{
    class Components : Simulation
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
            floor.GetProperty().SetColor(0.8, 0.8, 0.8);
            return floor;
        }

        public vtkAssembly drawRob(Geometry.Position robotBase, Geometry.Position angles)
        {
            List<Point> points = new List<Point>();
            Point prior = new Point(robotBase.X, robotBase.Y, robotBase.Z);
            Point tempPoint;
            Matrix4 tempMatrix = new Matrix4();

            Matrix4 A1 = GetLinkTransformation(new Link(-angles.X * Math.PI / 180, 400, 25, -Math.PI / 2));
            Matrix4 A2 = GetLinkTransformation(new Link(angles.Y * Math.PI / 180, 0, 455, 0));
            Matrix4 A3 = GetLinkTransformation(new Link(angles.Z * Math.PI / 180 - Math.PI / 2, 0, 35, -Math.PI / 2));
            Matrix4 A4 = GetLinkTransformation(new Link(-angles.A * Math.PI / 180, 420, 0, Math.PI / 2));
            Matrix4 A5 = GetLinkTransformation(new Link(angles.B * Math.PI / 180, 0, 0, -Math.PI / 2));
            Matrix4 A6 = GetLinkTransformation(new Link(angles.C * Math.PI / 180, 80, 0, 0));

            tempPoint = GetLinkTransformationPoint(A1);
            tempPoint.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPoint.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            points.Add(tempPoint);

            Matrix4 sumMatrix = new Matrix4();
            sumMatrix.Multiply(A1, A2);
            tempMatrix = sumMatrix;
            tempPoint = GetLinkTransformationPoint(tempMatrix);
            tempPoint.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPoint.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            points.Add(tempPoint);

            sumMatrix.Multiply(tempMatrix, A3);
            tempMatrix = sumMatrix;
            tempPoint = GetLinkTransformationPoint(tempMatrix);
            tempPoint.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPoint.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            points.Add(tempPoint);

            sumMatrix.Multiply(tempMatrix, A4);
            tempMatrix = sumMatrix;
            tempPoint = GetLinkTransformationPoint(tempMatrix);
            tempPoint.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPoint.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            points.Add(tempPoint);

            sumMatrix.Multiply(tempMatrix, A5);
            tempMatrix = sumMatrix;
            tempPoint = GetLinkTransformationPoint(tempMatrix);
            tempPoint.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPoint.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            points.Add(tempPoint);

            sumMatrix.Multiply(tempMatrix, A6);
            tempMatrix = sumMatrix;
            tempPoint = GetLinkTransformationPoint(tempMatrix);
            tempPoint.Rotate(robotBase.A, robotBase.B, robotBase.C);
            tempPoint.Translate(robotBase.X, robotBase.Y, robotBase.Z);
            points.Add(tempPoint);

            vtkAssembly asse = vtkAssembly.New();

            vtkActor rob00 = readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_00.stl"));
            vtkActor rob01 = readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_01.stl"));
            vtkActor rob02 = readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_02.stl"));
            vtkActor rob03 = readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_03.stl"));
            vtkActor rob04 = readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_04.stl"));
            vtkActor rob05 = readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_05.stl"));

            rob00.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            rob01.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            rob02.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            rob02.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            rob03.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            rob04.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            rob05.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);

            List<vtkActor> vtkPointList = new List<vtkActor>();
            vtkActor point00 = drawPoint(points[0].X, points[0].Y, points[0].Z);
            vtkActor point01 = drawPoint(points[1].X, points[1].Y, points[1].Z);
            vtkActor point02 = drawPoint(points[2].X, points[2].Y, points[2].Z);
            vtkActor point03 = drawPoint(points[3].X, points[3].Y, points[3].Z);
            vtkActor point04 = drawPoint(points[4].X, points[4].Y, points[4].Z);
            vtkActor point05 = drawPoint(points[5].X, points[5].Y, points[5].Z);
            vtkPointList.Add(point00);
            vtkPointList.Add(point01);
            vtkPointList.Add(point02);
            vtkPointList.Add(point03);
            vtkPointList.Add(point04);
            vtkPointList.Add(point05);
            foreach(vtkActor actor in vtkPointList){
                actor.SetPosition(robotBase.X - points[0].X, robotBase.Y - points[0].Y, robotBase.Z - points[0].Z);
                actor.SetOrigin(points[0].X, points[0].Y, points[0].Z);
                actor.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
                asse.AddPart(actor);
            }
            

            List<vtkActor> vtkLineList = new List<vtkActor>();
            vtkActor line00 = drawLines(points[0].X, points[0].Y, points[0].Z, points[1].X, points[1].Y, points[1].Z);
            vtkActor line01 = drawLines(points[1].X, points[1].Y, points[1].Z, points[2].X, points[2].Y, points[2].Z);
            vtkActor line02 = drawLines(points[2].X, points[2].Y, points[2].Z, points[3].X, points[3].Y, points[3].Z);
            vtkActor line03 = drawLines(points[3].X, points[3].Y, points[3].Z, points[4].X, points[4].Y, points[4].Z);
            vtkActor line04 = drawLines(points[4].X, points[4].Y, points[4].Z, points[5].X, points[5].Y, points[5].Z);
            vtkLineList.Add(line00);
            vtkLineList.Add(line01);
            vtkLineList.Add(line02);
            vtkLineList.Add(line03);
            vtkLineList.Add(line04);
            

            foreach (vtkActor actor in vtkLineList)
            {
                actor.SetPosition(robotBase.X - points[0].X, robotBase.Y - points[0].Y, robotBase.Z - points[0].Z);
                actor.SetOrigin(points[0].X, points[0].Y, points[0].Z);
                actor.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
                asse.AddPart(actor);
            }

            vtkActor robotBasePoint = drawPoint(robotBase.X, robotBase.Y, robotBase.Z);
            asse.AddPart(robotBasePoint);
           

            asse.AddPart(rob00);
            asse.AddPart(rob01);
            asse.AddPart(rob02);
            //asse.AddPart(rob03);
            //asse.AddPart(rob04);
            //asse.AddPart(rob05);


            return asse;
        }


        private vtkActor drawPoint(double x, double y, double z)
        {
            vtkPoints vtkPoints = vtkPoints.New();
            double[,] p = new double[,] { { x, y, z } };
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
            pointActor.SetMapper(mapper);
            pointActor.GetProperty().SetPointSize(10);
            return pointActor;
        }

        private vtkActor drawLines(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            vtkPolyData pointsAndLinesPoly = vtkPolyData.New();
            vtkPoints pts = new vtkPoints();
            pts.InsertNextPoint(x1, y1, z1);
            pts.InsertNextPoint(x2, y2, z2);
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
