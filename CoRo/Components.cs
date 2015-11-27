﻿using System;
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

        public List<Point> forwardKinematic(Geometry.Position robotBase, Geometry.Position angles)
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

            return points;
        }
    }

    class myRobot : Simulation
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


        List<Point> pointList;
        public List<Point> getPointList
        {
            get { return pointList; }
        }
        List<vtkActor> pointAndLineActors;
        List<vtkActor> stlActors;
        List<vtkActor> stlActorsZ;
        Components components = new Components();
        myVtk myVtk = new myVtk();

        vtkAssembly linesAndPointsAssembly;
        public vtkAssembly getLinesAndPointsAssembly
        {
            get { return linesAndPointsAssembly; }
        }


        IRob.Geometry.Position robotBase;
        IRob.Geometry.Position angles;
        public Geometry.Position getRobotBase
        {
            get { return robotBase; }
        }
        public Geometry.Position getAngles
        {
            get { return angles; }
        }

        public vtkAssembly drawRob()
        {
            pointList = components.forwardKinematic(robotBase, angles);

            stl00 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_00.stl"));
            stl01 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_01.stl"));
            stl02 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_02.stl"));
            stl03 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_03.stl"));
            stl04 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_04.stl"));
            stl05 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_05.stl"));


            stl00.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl01.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl02.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl03.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl04.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl05.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);

            stl01.RotateZ(-angles.X);
            stl02.RotateZ(-angles.X);
            stl03.RotateZ(-angles.X);
            stl04.RotateZ(-angles.X);
            stl05.RotateZ(-angles.X);
            
            stlActors.Add(stl00);
            stlActors.Add(stl01);
            stlActors.Add(stl02);
            stlActors.Add(stl03);
            stlActors.Add(stl04);
            stlActors.Add(stl05);

            stlActorsZ.Add(stl01);
            stlActorsZ.Add(stl02);
            stlActorsZ.Add(stl03);
            stlActorsZ.Add(stl04);
            stlActorsZ.Add(stl05);



            point00 = myVtk.drawPoint(pointList[0].X, pointList[0].Y, pointList[0].Z);
            point01 = myVtk.drawPoint(pointList[1].X, pointList[1].Y, pointList[1].Z);
            point02 = myVtk.drawPoint(pointList[2].X, pointList[2].Y, pointList[2].Z);
            point03 = myVtk.drawPoint(pointList[3].X, pointList[3].Y, pointList[3].Z);
            point04 = myVtk.drawPoint(pointList[4].X, pointList[4].Y, pointList[4].Z);
            point05 = myVtk.drawPoint(pointList[5].X, pointList[5].Y, pointList[5].Z);

            pointAndLineActors.Add(point00);
            pointAndLineActors.Add(point01);
            pointAndLineActors.Add(point02);
            pointAndLineActors.Add(point03);
            pointAndLineActors.Add(point04);
            pointAndLineActors.Add(point05);


            line00 = myVtk.drawLines(pointList[0].X, pointList[0].Y, pointList[0].Z, pointList[1].X, pointList[1].Y, pointList[1].Z);
            line01 = myVtk.drawLines(pointList[1].X, pointList[1].Y, pointList[1].Z, pointList[2].X, pointList[2].Y, pointList[2].Z);
            line02 = myVtk.drawLines(pointList[2].X, pointList[2].Y, pointList[2].Z, pointList[3].X, pointList[3].Y, pointList[3].Z);
            line03 = myVtk.drawLines(pointList[3].X, pointList[3].Y, pointList[3].Z, pointList[4].X, pointList[4].Y, pointList[4].Z);
            line04 = myVtk.drawLines(pointList[4].X, pointList[4].Y, pointList[4].Z, pointList[5].X, pointList[5].Y, pointList[5].Z);
            pointAndLineActors.Add(line00);
            pointAndLineActors.Add(line01);
            pointAndLineActors.Add(line02);
            pointAndLineActors.Add(line03);
            pointAndLineActors.Add(line04);

            foreach (vtkActor actor in pointAndLineActors)
            {
                linesAndPointsAssembly.AddPart(actor);
            }
            foreach (vtkActor actor in stlActors)
            {
                linesAndPointsAssembly.AddPart(actor);
            }

            return linesAndPointsAssembly;
        }

        public vtkAssembly rotateAxsA1(double value)
        {

            pointList.Clear();
            pointAndLineActors.Clear();
            stlActors.Clear();
            stlActorsZ.Clear();
            linesAndPointsAssembly = vtkAssembly.New();

            angles.X += value;
            pointList = components.forwardKinematic(robotBase, angles);

            stl00 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_00.stl"));
            stl01 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_01.stl"));
            stl02 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_02.stl"));
            stl03 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_03.stl"));
            stl04 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_04.stl"));
            stl05 = myVtk.readSTL(string.Concat(Environment.CurrentDirectory, @"\Robot\agilus_05.stl"));


            stl00.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl01.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl02.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl03.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl04.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);
            stl05.SetPosition(robotBase.X, robotBase.Y, robotBase.Z);

            stl01.RotateZ(-angles.X);
            stl02.RotateZ(-angles.X);
            stl03.RotateZ(-angles.X);
            stl04.RotateZ(-angles.X);
            stl05.RotateZ(-angles.X);
            
            stlActors.Add(stl00);
            stlActors.Add(stl01);
            stlActors.Add(stl02);
            stlActors.Add(stl03);
            stlActors.Add(stl04);
            stlActors.Add(stl05);

            stlActorsZ.Add(stl01);
            stlActorsZ.Add(stl02);
            stlActorsZ.Add(stl03);
            stlActorsZ.Add(stl04);
            stlActorsZ.Add(stl05);



            point00 = myVtk.drawPoint(pointList[0].X, pointList[0].Y, pointList[0].Z);
            point01 = myVtk.drawPoint(pointList[1].X, pointList[1].Y, pointList[1].Z);
            point02 = myVtk.drawPoint(pointList[2].X, pointList[2].Y, pointList[2].Z);
            point03 = myVtk.drawPoint(pointList[3].X, pointList[3].Y, pointList[3].Z);
            point04 = myVtk.drawPoint(pointList[4].X, pointList[4].Y, pointList[4].Z);
            point05 = myVtk.drawPoint(pointList[5].X, pointList[5].Y, pointList[5].Z);



            pointAndLineActors.Add(point00);
            pointAndLineActors.Add(point01);
            pointAndLineActors.Add(point02);
            pointAndLineActors.Add(point03);
            pointAndLineActors.Add(point04);
            pointAndLineActors.Add(point05);


            line00 = myVtk.drawLines(pointList[0].X, pointList[0].Y, pointList[0].Z, pointList[1].X, pointList[1].Y, pointList[1].Z);
            line01 = myVtk.drawLines(pointList[1].X, pointList[1].Y, pointList[1].Z, pointList[2].X, pointList[2].Y, pointList[2].Z);
            line02 = myVtk.drawLines(pointList[2].X, pointList[2].Y, pointList[2].Z, pointList[3].X, pointList[3].Y, pointList[3].Z);
            line03 = myVtk.drawLines(pointList[3].X, pointList[3].Y, pointList[3].Z, pointList[4].X, pointList[4].Y, pointList[4].Z);
            line04 = myVtk.drawLines(pointList[4].X, pointList[4].Y, pointList[4].Z, pointList[5].X, pointList[5].Y, pointList[5].Z);
            pointAndLineActors.Add(line00);
            pointAndLineActors.Add(line01);
            pointAndLineActors.Add(line02);
            pointAndLineActors.Add(line03);
            pointAndLineActors.Add(line04);

            foreach (vtkActor actor in pointAndLineActors)
            {
                linesAndPointsAssembly.AddPart(actor);

            }
            foreach (vtkActor actor in stlActors)
            {
                linesAndPointsAssembly.AddPart(actor);

            }
            return linesAndPointsAssembly;

        }

        public myRobot(Geometry.Position robotBase, Geometry.Position angles)
        {
            this.robotBase = robotBase;
            this.angles = angles;

            linesAndPointsAssembly = vtkAssembly.New();
            pointAndLineActors = new List<vtkActor>();
            stlActors = new List<vtkActor>();
            stlActorsZ = new List<vtkActor>();

        }
    }

    public class myVtk
    {
        public vtkActor drawPoint(double x, double y, double z)
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

        public vtkActor drawLines(double x1, double y1, double z1, double x2, double y2, double z2)
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
