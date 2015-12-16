using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Xml;
using System.ComponentModel;
using scanCONTROL;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using SlimDX;
using SlimDX.DirectInput;
using Kitware.VTK;

namespace IRob
{

    public class Geometry
    {
        public struct Point
        {
            public double X;
            public double Y;
            public double Z;

            public Point(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public void Translate(double x, double y, double z)
            {
                X = X + x;
                Y = Y + y;
                Z = Z + z;
            }

            public void Translate(Point t)
            {
                Translate(t.X, t.Y, t.Z);
            }

            public void Rotate(double a, double b, double c)
            {
                RotationMatrix R = new RotationMatrix(a, b, c);
                Rotate(R);
            }

            public void Rotate(RotationMatrix R)
            {
                double x = X * R.C1R1 + Y * R.C2R1 + Z * R.C3R1;
                double y = X * R.C1R2 + Y * R.C2R2 + Z * R.C3R2;
                double z = X * R.C1R3 + Y * R.C2R3 + Z * R.C3R3;
                X = x;
                Y = y;
                Z = z;
            }
        }

        public struct ColorPoint
        {
            public Point Point;
            public Color Color;

            public ColorPoint(Point p, Color c)
            {
                Point = p;
                Color = c;
            }
        }

        public struct Position
        {
            public double X, Y, Z, A, B, C;

            public Position(double x, double y, double z, double a, double b, double c)
            {
                X = x;
                Y = y;
                Z = z;
                A = a;
                B = b;
                C = c;
            }

            public Position(Position position)
            {
                X = position.X;
                Y = position.Y;
                Z = position.Z;
                A = position.A;
                B = position.B;
                C = position.C;
            }

            public void Translate(double x, double y, double z)
            {
                X = X + x;
                Y = Y + y;
                Z = Z + z;
            }

            public void Rotate(double a, double b, double c)
            {
                RotationMatrix R = new RotationMatrix(a, b, c);
                double x = X * R.C1R1 + Y * R.C2R1 + Z * R.C3R1;
                double y = X * R.C1R2 + Y * R.C2R2 + Z * R.C3R2;
                double z = X * R.C1R3 + Y * R.C2R3 + Z * R.C3R3;
                X = x;
                Y = y;
                Z = z;
                A = A + a;
                B = B + b;
                C = C + c;
            }

            public void Rotate(RotationMatrix R)
            {
                double x = X * R.C1R1 + Y * R.C2R1 + Z * R.C3R1;
                double y = X * R.C1R2 + Y * R.C2R2 + Z * R.C3R2;
                double z = X * R.C1R3 + Y * R.C2R3 + Z * R.C3R3;
                double a = Math.Atan2(R.C1R2, R.C1R1) * 180 / Math.PI;
                double b = Math.Atan2(-R.C1R3, Math.Sqrt(Math.Pow(R.C2R3, 2) + Math.Pow(R.C3R3, 2))) * 180 / Math.PI;
                double c = Math.Atan2(R.C2R3, R.C3R3) * 180 / Math.PI;
                X = x;
                Y = y;
                Z = z;
                A = A + a;
                B = B + b;
                C = C + c;
            }

            public void TranslateAxial(double x, double y, double z)
            {
                Position Pos = new Position(X, Y, Z, A, B, C);
                Pos.Translate(-X, -Y, -Z);
                Pos.Rotate(-A, -B, -C);
                Pos.Translate(x, y, z);
                Pos.Rotate(A, B, C);
                Pos.Translate(X, Y, Z);
                X = Pos.X;
                Y = Pos.Y;
                Z = Pos.Z;
            }

            public void Add(Position position)
            {
                X = X + position.X;
                Y = Y + position.Y;
                Z = Z + position.Z;
                A = A + position.A;
                B = B + position.B;
                C = C + position.C;
            }

            public void Subtract(Position position)
            {
                X = X - position.X;
                Y = Y - position.Y;
                Z = Z - position.Z;
                A = A - position.A;
                B = B - position.B;
                C = C - position.C;
            }

            public void Scale(double factor)
            {
                X = factor * X;
                Y = factor * Y;
                Z = factor * Z;
                A = factor * A;
                B = factor * B;
                C = factor * C;
            }
            /*
            public static bool operator ==(Position p1, Position p2)
            {
                return p1.Equals(p2);
            }
            
            public static bool operator !=(Position p1, Position p2)
            {
                return !p1.Equals(p2);
            }
            /*
            public override bool Equals(object o)
            {
               return true;
            }

            public override int GetHashCode()
            {
                return 0;
            }*/

            public Point ToPoint()
            {
                Point point = new Point(X, Y, Z);
                return point;
            }
        }

        public struct Vector
        {
            public Point Foot, Tip;
            public double X, Y, Z, Lenght;

            public Vector(Point foot, Point tip)
            {
                Foot = foot;
                Tip = tip;
                X = Tip.X - Foot.X;
                Y = Tip.Y - Foot.Y;
                Z = Tip.Z - Foot.Z;
                Lenght = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
            }

            public Vector(double x1, double y1, double z1, double x2, double y2, double z2)
            {
                Foot.X = x1;
                Foot.Y = y1;
                Foot.Z = z1;
                Tip.X = x2;
                Tip.Y = y2;
                Tip.Z = z2;
                X = Tip.X - Foot.X;
                Y = Tip.Y - Foot.Y;
                Z = Tip.Z - Foot.Z;
                Lenght = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
            }

            public void Translate(double x, double y, double z)
            {
                Foot.Translate(x, y, z);
                Tip.Translate(x, y, z);
                X = Tip.X - Foot.X;
                Y = Tip.Y - Foot.Y;
                Z = Tip.Z - Foot.Z;
                Lenght = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
            }

            public void Rotate(double a, double b, double c)
            {
                Rotate(new RotationMatrix(a, b, c));
            }

            public void Rotate(RotationMatrix R)
            {
                double x = Foot.X * R.C1R1 + Foot.Y * R.C2R1 + Foot.Z * R.C3R1;
                double y = Foot.X * R.C1R2 + Foot.Y * R.C2R2 + Foot.Z * R.C3R2;
                double z = Foot.X * R.C1R3 + Foot.Y * R.C2R3 + Foot.Z * R.C3R3;
                Foot = new Point(x, y, z);
                x = Tip.X * R.C1R1 + Tip.Y * R.C2R1 + Tip.Z * R.C3R1;
                y = Tip.X * R.C1R2 + Tip.Y * R.C2R2 + Tip.Z * R.C3R2;
                z = Tip.X * R.C1R3 + Tip.Y * R.C2R3 + Tip.Z * R.C3R3;
                Tip = new Point(x, y, z);
                X = Tip.X - Foot.X;
                Y = Tip.Y - Foot.Y;
                Z = Tip.Z - Foot.Z;
                Lenght = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
            }
        }

        public struct Matrix
        {
            public double C1R1, C1R2, C1R3, C2R1, C2R2, C2R3, C3R1, C3R2, C3R3;

            public Matrix(double c1r1, double c2r1, double c3r1, double c1r2, double c2r2, double c3r2, double c1r3, double c2r3, double c3r3)
            {
                C1R1 = c1r1;
                C1R2 = c1r2;
                C1R3 = c1r3;
                C2R1 = c2r1;
                C2R2 = c2r2;
                C2R3 = c2r3;
                C3R1 = c3r1;
                C3R2 = c3r2;
                C3R3 = c3r3;
            }
        }

        public struct Matrix4
        {
            public double C1R1, C1R2, C1R3, C1R4, C2R1, C2R2, C2R3, C2R4, C3R1, C3R2, C3R3, C3R4, C4R1, C4R2, C4R3, C4R4;

            public void Multiply(Matrix4 Matrix1, Matrix4 Matrix2)
            {
                C1R1 = Matrix1.C1R1 * Matrix2.C1R1 + Matrix1.C2R1 * Matrix2.C1R2 + Matrix1.C3R1 * Matrix2.C1R3 + Matrix1.C4R1 * Matrix2.C1R4;
                C1R2 = Matrix1.C1R2 * Matrix2.C1R1 + Matrix1.C2R2 * Matrix2.C1R2 + Matrix1.C3R2 * Matrix2.C1R3 + Matrix1.C4R2 * Matrix2.C1R4;
                C1R3 = Matrix1.C1R3 * Matrix2.C1R1 + Matrix1.C2R3 * Matrix2.C1R2 + Matrix1.C3R3 * Matrix2.C1R3 + Matrix1.C4R3 * Matrix2.C1R4;
                C1R4 = Matrix1.C1R4 * Matrix2.C1R1 + Matrix1.C2R4 * Matrix2.C1R2 + Matrix1.C3R4 * Matrix2.C1R3 + Matrix1.C4R4 * Matrix2.C1R4;
                C2R1 = Matrix1.C1R1 * Matrix2.C2R1 + Matrix1.C2R1 * Matrix2.C2R2 + Matrix1.C3R1 * Matrix2.C2R3 + Matrix1.C4R1 * Matrix2.C2R4;
                C2R2 = Matrix1.C1R2 * Matrix2.C2R1 + Matrix1.C2R2 * Matrix2.C2R2 + Matrix1.C3R2 * Matrix2.C2R3 + Matrix1.C4R2 * Matrix2.C2R4;
                C2R3 = Matrix1.C1R3 * Matrix2.C2R1 + Matrix1.C2R3 * Matrix2.C2R2 + Matrix1.C3R3 * Matrix2.C2R3 + Matrix1.C4R3 * Matrix2.C2R4;
                C2R4 = Matrix1.C1R4 * Matrix2.C2R1 + Matrix1.C2R4 * Matrix2.C2R2 + Matrix1.C3R4 * Matrix2.C2R3 + Matrix1.C4R4 * Matrix2.C2R4;
                C3R1 = Matrix1.C1R1 * Matrix2.C3R1 + Matrix1.C2R1 * Matrix2.C3R2 + Matrix1.C3R1 * Matrix2.C3R3 + Matrix1.C4R1 * Matrix2.C3R4;
                C3R2 = Matrix1.C1R2 * Matrix2.C3R1 + Matrix1.C2R2 * Matrix2.C3R2 + Matrix1.C3R2 * Matrix2.C3R3 + Matrix1.C4R2 * Matrix2.C3R4;
                C3R3 = Matrix1.C1R3 * Matrix2.C3R1 + Matrix1.C2R3 * Matrix2.C3R2 + Matrix1.C3R3 * Matrix2.C3R3 + Matrix1.C4R3 * Matrix2.C3R4;
                C3R4 = Matrix1.C1R4 * Matrix2.C3R1 + Matrix1.C2R4 * Matrix2.C3R2 + Matrix1.C3R4 * Matrix2.C3R3 + Matrix1.C4R4 * Matrix2.C3R4;
                C4R1 = Matrix1.C1R1 * Matrix2.C4R1 + Matrix1.C2R1 * Matrix2.C4R2 + Matrix1.C3R1 * Matrix2.C4R3 + Matrix1.C4R1 * Matrix2.C4R4;
                C4R2 = Matrix1.C1R2 * Matrix2.C4R1 + Matrix1.C2R2 * Matrix2.C4R2 + Matrix1.C3R2 * Matrix2.C4R3 + Matrix1.C4R2 * Matrix2.C4R4;
                C4R3 = Matrix1.C1R3 * Matrix2.C4R1 + Matrix1.C2R3 * Matrix2.C4R2 + Matrix1.C3R3 * Matrix2.C4R3 + Matrix1.C4R3 * Matrix2.C4R4;
                C4R4 = Matrix1.C1R4 * Matrix2.C4R1 + Matrix1.C2R4 * Matrix2.C4R2 + Matrix1.C3R4 * Matrix2.C4R3 + Matrix1.C4R4 * Matrix2.C4R4;
            }
        }

        public struct RotationMatrix
        {
            public double C1R1, C1R2, C1R3, C2R1, C2R2, C2R3, C3R1, C3R2, C3R3;

            public RotationMatrix(double a, double b, double c)
            {
                //Radian
                double RA = a * Math.PI / 180, RB = b * Math.PI / 180, RC = c * Math.PI / 180;
                //RzRyRx
                C1R1 = Math.Cos(RB) * Math.Cos(RA);
                C1R2 = Math.Cos(RB) * Math.Sin(RA);
                C1R3 = -Math.Sin(RB);
                C2R1 = Math.Sin(RC) * Math.Sin(RB) * Math.Cos(RA) - Math.Cos(RC) * Math.Sin(RA);
                C2R2 = Math.Sin(RC) * Math.Sin(RB) * Math.Sin(RA) + Math.Cos(RC) * Math.Cos(RA);
                C2R3 = Math.Sin(RC) * Math.Cos(RB);
                C3R1 = Math.Cos(RC) * Math.Sin(RB) * Math.Cos(RA) + Math.Sin(RC) * Math.Sin(RA);
                C3R2 = Math.Cos(RC) * Math.Sin(RB) * Math.Sin(RA) - Math.Sin(RC) * Math.Cos(RA);
                C3R3 = Math.Cos(RC) * Math.Cos(RB);
            }
        }

        public struct ReverseRotationMatrix
        {
            public double C1R1, C1R2, C1R3, C2R1, C2R2, C2R3, C3R1, C3R2, C3R3;

            public ReverseRotationMatrix(double a, double b, double c)
            {
                //Radian
                double RA = a * Math.PI / 180, RB = b * Math.PI / 180, RC = c * Math.PI / 180;
                //RzRyRx
                C1R1 = Math.Cos(RB) * Math.Cos(RC);
                C1R2 = Math.Sin(RA) * Math.Sin(RB) * Math.Cos(RC) + Math.Cos(RA) * Math.Sin(RC);
                C1R3 = -Math.Cos(RA) * Math.Sin(RB) * Math.Cos(RC) + Math.Sin(RA) * Math.Sin(RC);
                C2R1 = -Math.Cos(RB) * Math.Sin(RC);
                C2R2 = -Math.Sin(RA) * Math.Sin(RB) * Math.Sin(RC) + Math.Cos(RA) * Math.Cos(RC);
                C2R3 = Math.Cos(RA) * Math.Sin(RB) * Math.Sin(RC) + Math.Sin(RA) * Math.Cos(RC);
                C3R1 = Math.Sin(RB);
                C3R2 = -Math.Sin(RA) * Math.Cos(RB);
                C3R3 = Math.Cos(RA) * Math.Cos(RB);
            }

            public RotationMatrix ToRotationMatrix()
            {
                RotationMatrix R = new RotationMatrix();
                R.C1R1 = C1R1;
                R.C1R2 = C1R2;
                R.C1R3 = C1R3;
                R.C2R1 = C2R1;
                R.C2R2 = C2R2;
                R.C2R3 = C2R3;
                R.C3R1 = C3R1;
                R.C3R2 = C3R2;
                R.C3R3 = C3R3;
                return R;
            }
        }

        public struct Triangle
        {
            public Point Point1;
            public Point Point2;
            public Point Point3;

            public Triangle(Point p1, Point p2, Point p3)
            {
                Point1 = p1;
                Point2 = p2;
                Point3 = p3;
            }
        }

        public struct Link
        {
            public double theta, d, a, alpha;

            public Link(double Theta, double D, double A, double Alpha)
            {
                theta = Theta;
                d = D;
                a = A;
                alpha = Alpha;
            }
        }

        public Vector Normal(Triangle Rect)
        {
            //Vectors
            Point U = new Point();
            Point V = new Point();
            U = Subtract(Rect.Point2, Rect.Point1);
            V = Subtract(Rect.Point3, Rect.Point1);
            //Normal
            Vector N = new Vector(0, 0, 0, (U.Y * V.Z) - (U.Z * V.Y), (U.Z * V.X) - (U.X * V.Z), (U.X * V.Y) - (U.Y * V.X));
            return N;
        }

        public Point Center(Triangle Rect)
        {
            Point C = new Point();
            C.X = (Rect.Point1.X + Rect.Point2.X + Rect.Point3.X) / 3;
            C.Y = (Rect.Point1.Y + Rect.Point2.Y + Rect.Point3.Y) / 3;
            C.Z = (Rect.Point1.Z + Rect.Point2.Z + Rect.Point3.Z) / 3;
            return C;
        }

        public Point Subtract(Point Tip, Point Base)
        {
            Point Res = new Point();
            Res.X = Tip.X - Base.X;
            Res.Y = Tip.Y - Base.Y;
            Res.Z = Tip.Z - Base.Z;
            return Res;
        }

        public Matrix Subtract(Matrix M1, Matrix M2)
        {
            Matrix Res = new Matrix();
            Res.C1R1 = M1.C1R1 - M2.C1R1;
            Res.C1R2 = M1.C1R2 - M2.C1R2;
            Res.C1R3 = M1.C1R3 - M2.C1R3;
            Res.C2R1 = M1.C2R1 - M2.C2R1;
            Res.C2R2 = M1.C2R2 - M2.C2R2;
            Res.C2R3 = M1.C2R3 - M2.C2R3;
            Res.C3R1 = M1.C3R1 - M2.C3R1;
            Res.C3R2 = M1.C3R2 - M2.C3R2;
            Res.C3R3 = M1.C3R3 - M2.C3R3;
            return Res;
        }

        public Point Sum(Point P1, Point P2)
        {
            Point Res = new Point();
            Res.X = P1.X + P2.X;
            Res.Y = P1.Y + P2.Y;
            Res.Z = P1.Z + P2.Z;
            return Res;
        }

        public Matrix Sum(Matrix M1, Matrix M2)
        {
            Matrix Res = new Matrix();
            Res.C1R1 = M1.C1R1 + M2.C1R1;
            Res.C1R2 = M1.C1R2 + M2.C1R2;
            Res.C1R3 = M1.C1R3 + M2.C1R3;
            Res.C2R1 = M1.C2R1 + M2.C2R1;
            Res.C2R2 = M1.C2R2 + M2.C2R2;
            Res.C2R3 = M1.C2R3 + M2.C2R3;
            Res.C3R1 = M1.C3R1 + M2.C3R1;
            Res.C3R2 = M1.C3R2 + M2.C3R2;
            Res.C3R3 = M1.C3R3 + M2.C3R3;
            return Res;
        }

        public Point Multiply(Matrix Matrix, Point Point)
        {
            Point Res = new Point();
            Res.X = Matrix.C1R1 * Point.X + Matrix.C2R1 * Point.Y + Matrix.C3R1 * Point.Z;
            Res.Y = Matrix.C1R2 * Point.X + Matrix.C2R2 * Point.Y + Matrix.C3R2 * Point.Z;
            Res.Z = Matrix.C1R3 * Point.X + Matrix.C2R3 * Point.Y + Matrix.C3R3 * Point.Z;
            return Res;
        }

        public Point Multiply(RotationMatrix Matrix, Point Point)
        {
            Point Res = new Point();
            Res.X = Matrix.C1R1 * Point.X + Matrix.C2R1 * Point.Y + Matrix.C3R1 * Point.Z;
            Res.Y = Matrix.C1R2 * Point.X + Matrix.C2R2 * Point.Y + Matrix.C3R2 * Point.Z;
            Res.Z = Matrix.C1R3 * Point.X + Matrix.C2R3 * Point.Y + Matrix.C3R3 * Point.Z;
            return Res;
        }

        public Matrix Multiply(Matrix M1, Matrix M2)
        {
            Matrix Res = new Matrix();
            Res.C1R1 = M1.C1R1 * M2.C1R1 + M1.C2R1 * M2.C1R2 + M1.C3R1 * M2.C1R3;
            Res.C1R2 = M1.C1R2 * M2.C1R1 + M1.C2R2 * M2.C1R2 + M1.C3R2 * M2.C1R3;
            Res.C1R3 = M1.C1R3 * M2.C1R1 + M1.C2R3 * M2.C1R2 + M1.C3R3 * M2.C1R3;
            Res.C2R1 = M1.C1R1 * M2.C2R1 + M1.C2R1 * M2.C2R2 + M1.C3R1 * M2.C2R3;
            Res.C2R2 = M1.C1R2 * M2.C2R1 + M1.C2R2 * M2.C2R2 + M1.C3R2 * M2.C2R3;
            Res.C2R3 = M1.C1R3 * M2.C2R1 + M1.C2R3 * M2.C2R2 + M1.C3R3 * M2.C2R3;
            Res.C3R1 = M1.C1R1 * M2.C3R1 + M1.C2R1 * M2.C3R2 + M1.C3R1 * M2.C3R3;
            Res.C3R2 = M1.C1R2 * M2.C3R1 + M1.C2R2 * M2.C3R2 + M1.C3R2 * M2.C3R3;
            Res.C3R3 = M1.C1R3 * M2.C3R1 + M1.C2R3 * M2.C3R2 + M1.C3R3 * M2.C3R3;
            return Res;
        }

        public RotationMatrix Multiply(RotationMatrix M1, RotationMatrix M2)
        {
            RotationMatrix Res = new RotationMatrix();
            Res.C1R1 = M1.C1R1 * M2.C1R1 + M1.C2R1 * M2.C1R2 + M1.C3R1 * M2.C1R3;
            Res.C1R2 = M1.C1R2 * M2.C1R1 + M1.C2R2 * M2.C1R2 + M1.C3R2 * M2.C1R3;
            Res.C1R3 = M1.C1R3 * M2.C1R1 + M1.C2R3 * M2.C1R2 + M1.C3R3 * M2.C1R3;
            Res.C2R1 = M1.C1R1 * M2.C2R1 + M1.C2R1 * M2.C2R2 + M1.C3R1 * M2.C2R3;
            Res.C2R2 = M1.C1R2 * M2.C2R1 + M1.C2R2 * M2.C2R2 + M1.C3R2 * M2.C2R3;
            Res.C2R3 = M1.C1R3 * M2.C2R1 + M1.C2R3 * M2.C2R2 + M1.C3R3 * M2.C2R3;
            Res.C3R1 = M1.C1R1 * M2.C3R1 + M1.C2R1 * M2.C3R2 + M1.C3R1 * M2.C3R3;
            Res.C3R2 = M1.C1R2 * M2.C3R1 + M1.C2R2 * M2.C3R2 + M1.C3R2 * M2.C3R3;
            Res.C3R3 = M1.C1R3 * M2.C3R1 + M1.C2R3 * M2.C3R2 + M1.C3R3 * M2.C3R3;
            return Res;
        }

        public Matrix Multiply(double c, Matrix M)
        {
            Matrix Res = new Matrix();
            Res.C1R1 = c * M.C1R1;
            Res.C1R2 = c * M.C1R2;
            Res.C1R3 = c * M.C1R3;
            Res.C2R1 = c * M.C2R1;
            Res.C2R2 = c * M.C2R2;
            Res.C2R3 = c * M.C2R3;
            Res.C3R1 = c * M.C3R1;
            Res.C3R2 = c * M.C3R2;
            Res.C3R3 = c * M.C3R3;
            return Res;
        }

        public Matrix Invert(Matrix Matrix)
        {
            Matrix Res = new Matrix();
            double det = (Matrix.C1R1 * Matrix.C2R2 * Matrix.C3R3) +
                (Matrix.C2R1 * Matrix.C3R2 * Matrix.C1R3) + (Matrix.C3R1 * Matrix.C1R2 * Matrix.C2R3) -
                (Matrix.C3R1 * Matrix.C2R2 * Matrix.C1R3) - (Matrix.C2R1 * Matrix.C1R2 * Matrix.C3R3) -
                (Matrix.C1R1 * Matrix.C3R2 * Matrix.C2R3);
            Res.C1R1 = det * ((Matrix.C2R2 * Matrix.C3R3) - (Matrix.C3R2 * Matrix.C2R3));
            Res.C1R2 = det * ((Matrix.C3R2 * Matrix.C1R3) - (Matrix.C1R2 * Matrix.C3R3));
            Res.C1R3 = det * ((Matrix.C1R2 * Matrix.C2R3) - (Matrix.C2R2 * Matrix.C1R3));
            Res.C2R1 = det * ((Matrix.C3R1 * Matrix.C2R3) - (Matrix.C2R1 * Matrix.C3R3));
            Res.C2R2 = det * ((Matrix.C1R1 * Matrix.C3R3) - (Matrix.C3R1 * Matrix.C1R3));
            Res.C2R3 = det * ((Matrix.C2R1 * Matrix.C1R3) - (Matrix.C1R1 * Matrix.C2R3));
            Res.C3R1 = det * ((Matrix.C2R1 * Matrix.C3R2) - (Matrix.C3R1 * Matrix.C2R2));
            Res.C3R2 = det * ((Matrix.C3R1 * Matrix.C1R2) - (Matrix.C1R1 * Matrix.C3R2));
            Res.C3R3 = det * ((Matrix.C1R1 * Matrix.C2R2) - (Matrix.C2R1 * Matrix.C1R2));
            return Res;
        }

        public Point ToPoint(Position position)
        {
            Point point = new Point(position.X, position.Y, position.Z);
            return point;
        }

        public Point ToPoint(Vector vector)
        {
            Point point = new Point(vector.X, vector.Y, vector.Z);
            return point;
        }

        public Vector CrossProduct(Vector a, Vector b)
        {
            return new Vector(0, 0, 0, a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        public double VectorProduct(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public double Angle(Vector a, Vector b)
        {
            return Math.Acos(VectorProduct(a, b) / (a.Lenght * b.Lenght)) * 180 / Math.PI;
        }

        public Vector UnitVector(Vector v)
        {
            double l = v.Lenght;
            Vector Res = new Vector(v.Foot, new Point(v.Foot.X + v.X / l, v.Foot.Y + v.Y / l, v.Foot.Z + v.Z / l));
            return Res;
        }
    }


    public class PointCloud : Geometry
    {
        public List<ColorPoint> Points = new List<ColorPoint>();
        public List<Triangle> Mesh = new List<Triangle>();
        public Dictionary<Key, Point> Voxels = new Dictionary<Key, Point>();
        public Space SpaceBox = new Space();

        public PointCloud()
        {
            Points = new List<ColorPoint>();
            Mesh = new List<Triangle>();
        }

        public PointCloud(PointCloud pointCloud)
        {
            Points = pointCloud.Points;
            Mesh = pointCloud.Mesh;
            Voxels = pointCloud.Voxels;
            SpaceBox = pointCloud.SpaceBox;
        }

        public struct Voxel
        {
            public int X;
            public int Y;
            public int Z;

            public Voxel(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public void Translate(double x, double y, double z)
            {
                Point point = new Point(X, Y, Z);
                point.Translate(x, y, z);
                X = (int)point.X;
                Y = (int)point.Y;
                Z = (int)point.Z;
            }

            public void Translate(Point t)
            {
                Point point = new Point(X, Y, Z);
                point.Translate(t);
                X = (int)point.X;
                Y = (int)point.Y;
                Z = (int)point.Z;
            }

            public void Rotate(double a, double b, double c)
            {
                Point point = new Point(X, Y, Z);
                point.Rotate(a, b, c);
                X = (int)point.X;
                Y = (int)point.Y;
                Z = (int)point.Z;
            }

            public void Rotate(RotationMatrix R)
            {
                Point point = new Point(X, Y, Z);
                point.Rotate(R);
                X = (int)point.X;
                Y = (int)point.Y;
                Z = (int)point.Z;
            }
        }

        public struct Key
        {
            public int X;
            public int Y;
        }

        public struct Space
        {
            public Voxel Min;
            public Voxel Max;

            public Space(Voxel min, Voxel max)
            {
                Min = min;
                Max = max;
            }

            public Space(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
            {
                Min = new Voxel(minX, minY, minZ);
                Max = new Voxel(maxX, maxY, maxZ); ;
            }
        }

        Voxel Subtract(Voxel Tip, Voxel Base)
        {
            Voxel Res = new Voxel();
            Res.X = Tip.X - Base.X;
            Res.Y = Tip.Y - Base.Y;
            Res.Z = Tip.Z - Base.Z;
            return Res;
        }

        private double Distance(Point p1, Point p2)
        {
            double distance = 0;
            distance = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.Z - p1.Z, 2));
            return distance;
        }

        public void Translate(Point t)
        {
            List<ColorPoint> NewPoints = new List<ColorPoint>();
            foreach (ColorPoint Point in Points)
            {
                NewPoints.Add(new ColorPoint(new Point(Point.Point.X + t.X, Point.Point.Y + t.Y, Point.Point.Z + t.Z), Point.Color));
            }
            Points = NewPoints;

            List<Triangle> NewMesh = new List<Triangle>();
            foreach (Triangle Triangle in Mesh)
            {
                NewMesh.Add(new Triangle(new Point(Triangle.Point1.X + t.X, Triangle.Point1.Y + t.Y, Triangle.Point1.Z + t.Z), new Point(Triangle.Point2.X + t.X, Triangle.Point2.Y + t.Y, Triangle.Point2.Z + t.Z), new Point(Triangle.Point3.X + t.X, Triangle.Point3.Y + t.Y, Triangle.Point3.Z + t.Z)));
            }
            Mesh = NewMesh;

            Dictionary<Key, Point> NewVoxels = new Dictionary<Key, Point>();
            foreach (KeyValuePair<Key, Point> Voxel in Voxels)
            {
                NewVoxels.Add(Voxel.Key, new Point(Voxel.Value.X + t.X, Voxel.Value.Y + t.Y, Voxel.Value.Z + t.Z));
            }
            Voxels = NewVoxels;
        }

        public void Translate(double x, double y, double z)
        {
            Translate(new Point(x, y, z));
        }

        public void Rotate(RotationMatrix R)
        {
            List<ColorPoint> NewPoints = new List<ColorPoint>();
            foreach (ColorPoint Point in Points)
            {
                NewPoints.Add(new ColorPoint(new Point(Point.Point.X * R.C1R1 + Point.Point.Y * R.C2R1 + Point.Point.Z * R.C3R1,
                    Point.Point.X * R.C1R2 + Point.Point.Y * R.C2R2 + Point.Point.Z * R.C3R2,
                    Point.Point.X * R.C1R3 + Point.Point.Y * R.C2R3 + Point.Point.Z * R.C3R3), Point.Color));
            }
            Points = NewPoints;

            List<Triangle> NewMesh = new List<Triangle>();
            foreach (Triangle Triangle in Mesh)
            {
                NewMesh.Add(new Triangle(
                    new Point(Triangle.Point1.X * R.C1R1 + Triangle.Point1.Y * R.C2R1 + Triangle.Point1.Z * R.C3R1,
                    Triangle.Point1.X * R.C1R2 + Triangle.Point1.Y * R.C2R2 + Triangle.Point1.Z * R.C3R2,
                    Triangle.Point1.X * R.C1R3 + Triangle.Point1.Y * R.C2R3 + Triangle.Point1.Z * R.C3R3),
                    new Point(Triangle.Point2.X * R.C1R1 + Triangle.Point2.Y * R.C2R1 + Triangle.Point2.Z * R.C3R1,
                    Triangle.Point2.X * R.C1R2 + Triangle.Point2.Y * R.C2R2 + Triangle.Point2.Z * R.C3R2,
                    Triangle.Point2.X * R.C1R3 + Triangle.Point2.Y * R.C2R3 + Triangle.Point2.Z * R.C3R3),
                    new Point(Triangle.Point3.X * R.C1R1 + Triangle.Point3.Y * R.C2R1 + Triangle.Point3.Z * R.C3R1,
                    Triangle.Point3.X * R.C1R2 + Triangle.Point3.Y * R.C2R2 + Triangle.Point3.Z * R.C3R2,
                    Triangle.Point3.X * R.C1R3 + Triangle.Point3.Y * R.C2R3 + Triangle.Point3.Z * R.C3R3)));
            }
            Mesh = NewMesh;

            Dictionary<Key, Point> NewVoxels = new Dictionary<Key, Point>();
            foreach (KeyValuePair<Key, Point> Voxel in Voxels)
            {
                NewVoxels.Add(Voxel.Key, new Point(Voxel.Value.X * R.C1R1 + Voxel.Value.Y * R.C2R1 + Voxel.Value.Z * R.C3R1,
                    Voxel.Value.X * R.C1R2 + Voxel.Value.Y * R.C2R2 + Voxel.Value.Z * R.C3R2,
                    Voxel.Value.X * R.C1R3 + Voxel.Value.Y * R.C2R3 + Voxel.Value.Z * R.C3R3));
            }
            Voxels = NewVoxels;
        }

        public void Rotate(double a, double b, double c)
        {
            Rotate(new RotationMatrix(a, b, c));
        }

        public void Transform(Position reference, Position transformation)
        {
            Translate(-reference.X, -reference.Y, -reference.Z);
            Rotate(transformation.A, transformation.B, transformation.C);
            Translate(reference.X + transformation.X, reference.Y + transformation.Y, reference.Z + transformation.Z);
        }

        public void Add(PointCloud points)
        {
            Points.AddRange(points.Points);
        }

        public void Clear()
        {
            Points = new List<ColorPoint>();
            Voxels = new Dictionary<Key, Point>();
            Mesh = new List<Triangle>();
            SpaceBox = new Space();
        }

        public void GetSpace()
        {
            if (Points.Count() > 0)
            {
                SpaceBox.Min.X = Convert.ToInt32(Math.Floor(Points.Min(x => x.Point.X)));
                SpaceBox.Min.Y = Convert.ToInt32(Math.Floor(Points.Min(x => x.Point.Y)));
                SpaceBox.Min.Z = Convert.ToInt32(Math.Floor(Points.Min(x => x.Point.Z)));
                SpaceBox.Max.X = Convert.ToInt32(Math.Ceiling(Points.Max(x => x.Point.X)));
                SpaceBox.Max.Y = Convert.ToInt32(Math.Ceiling(Points.Max(x => x.Point.Y)));
                SpaceBox.Max.Z = Convert.ToInt32(Math.Ceiling(Points.Max(x => x.Point.Z)));
            }
        }

        public void GetMesh(int Raster)
        {
            //Get the voxels
            Voxels.Clear();

            foreach (ColorPoint Point in Points)
            {
                if (Point.Point.X > SpaceBox.Min.X && Point.Point.X < SpaceBox.Max.X && Point.Point.Y > SpaceBox.Min.Y && Point.Point.Y < SpaceBox.Max.Y && Point.Point.Z > SpaceBox.Min.Z && Point.Point.Z < SpaceBox.Max.Z)
                {
                    Key PointKey = new Key();
                    PointKey.X = Convert.ToInt32(Math.Floor(1000 * (Point.Point.X - SpaceBox.Min.X) / Raster));
                    PointKey.Y = Convert.ToInt32(Math.Floor(1000 * (Point.Point.Y - SpaceBox.Min.Y) / Raster));
                    Point Value = new Point();
                    if (Voxels.TryGetValue(PointKey, out Value))
                    {
                        Value.X = (Value.X + Point.Point.X) / 2;
                        Value.Y = (Value.Y + Point.Point.Y) / 2;
                        Value.Z = (Value.Z + Point.Point.Z) / 2;
                        Voxels[PointKey] = Value;
                    }
                    else
                    {
                        Voxels.Add(PointKey, Point.Point);
                    }
                }
            }

            //Mesh the voxel points
            if (Voxels.Count() != 0)
            {
                Mesh.Clear();

                foreach (KeyValuePair<Key, Point> Voxel in Voxels)
                {
                    Key Key = Voxel.Key, KeyR = Voxel.Key, KeyB = Voxel.Key, KeyRB = Voxel.Key;
                    KeyB.X++;
                    KeyR.Y++;
                    KeyRB.X++;
                    KeyRB.Y++;

                    if (Voxels.ContainsKey(KeyRB) && Voxels.ContainsKey(KeyB) && Voxels.ContainsKey(KeyR))
                    {
                        if (Distance(Voxels[Key], Voxels[KeyRB]) < Distance(Voxels[KeyB], Voxels[KeyR]))
                        {
                            Mesh.Add(new Triangle(Voxels[Key], Voxels[KeyRB], Voxels[KeyR]));
                            Mesh.Add(new Triangle(Voxels[Key], Voxels[KeyB], Voxels[KeyRB]));
                        }
                        else
                        {
                            Mesh.Add(new Triangle(Voxels[Key], Voxels[KeyB], Voxels[KeyR]));
                            Mesh.Add(new Triangle(Voxels[KeyRB], Voxels[KeyR], Voxels[KeyB]));
                        }
                    }
                    else if (Voxels.ContainsKey(KeyB) && Voxels.ContainsKey(KeyR))
                    {
                        Mesh.Add(new Triangle(Voxels[Key], Voxels[KeyB], Voxels[KeyR]));
                    }
                }
            }
        }

        /// <summary>
        /// Saves points or mesh to a file. The data type results from the path.
        /// Compatible: ASCII, PCD, STL
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <returns></returns>
        public bool SaveFile(string path)
        {
            bool status = true;
            StreamWriter writer = new StreamWriter(path);

            //Get file extention 
            string type = System.IO.Path.GetExtension(path);

            switch (type)
            {
                //ASCII
                case ".asc":
                    foreach (ColorPoint point in Points)
                    {
                        writer.WriteLine(point.Point.X.ToString("F3").Replace(",", ".") + "," + point.Point.Y.ToString("F3").Replace(",", ".") + "," + point.Point.Z.ToString("F3").Replace(",", "."));
                    }
                    break;
                //PCD
                case ".pcd":
                    writer.WriteLine("# .PCD v.7 - Point Cloud Data file format\r\nVERSION .7\r\nFIELDS x y z rgb\r\nSIZE 4 4 4 4\r\nTYPE F F F F\r\nCOUNT 1 1 1 1\r\nWIDTH " + Points.Count() + "\r\nHEIGHT 1\r\nVIEWPOINT 0 0 0 1 0 0 0\r\nPOINTS " + Points.Count() + "\r\nDATA ascii");
                    foreach (ColorPoint point in Points)
                    {
                        string hexColor = point.Color.R.ToString("X2") + point.Color.G.ToString("X2") + point.Color.B.ToString("X2");
                        uint num = uint.Parse(hexColor, System.Globalization.NumberStyles.AllowHexSpecifier);
                        byte[] floatVals = BitConverter.GetBytes(num);
                        float floatColor = BitConverter.ToSingle(floatVals, 0);
                        writer.WriteLine((point.Point.X.ToString("F3") + " " + point.Point.X.ToString("F3") + " " + point.Point.Y.ToString("F3") + " " + floatColor).Replace(",", "."));
                    }
                    break;
                //STL
                case ".stl":
                    string name = System.IO.Path.GetFileNameWithoutExtension(path);
                    writer.WriteLine("solid " + name);
                    foreach (Triangle triangle in Mesh)
                    {
                        writer.WriteLine(("  facet normal " + Normal(triangle).X.ToString("E6") + " " + Normal(triangle).Y.ToString("E6") + " " + Normal(triangle).Z.ToString("E6")).Replace(",", "."));
                        writer.WriteLine("    outer loop");
                        writer.WriteLine(("      vertex " + triangle.Point1.X.ToString("E6") + " " + triangle.Point1.Y.ToString("E6") + " " + triangle.Point1.Z.ToString("E6")).Replace(",", "."));
                        writer.WriteLine(("      vertex " + triangle.Point2.X.ToString("E6") + " " + triangle.Point2.Y.ToString("E6") + " " + triangle.Point2.Z.ToString("E6")).Replace(",", "."));
                        writer.WriteLine(("      vertex " + triangle.Point3.X.ToString("E6") + " " + triangle.Point3.Y.ToString("E6") + " " + triangle.Point3.Z.ToString("E6")).Replace(",", "."));
                        writer.WriteLine("    endloop");
                        writer.WriteLine("  endfacet");
                    }
                    writer.WriteLine("endsolid " + name);
                    break;
                default:
                    status = false;
                    break;
            }

            writer.Dispose();
            writer.Close();
            return status;
        }

        /// <summary>
        /// Loads points or mesh from a file.
        /// Compatible: ASCII, PCD, STL
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <returns></returns>
        public bool LoadFile(string path)
        {
            Clear();

            bool status = true;
            System.IO.StreamReader reader = new System.IO.StreamReader(path);

            //Get file extenstion
            string type = System.IO.Path.GetExtension(path);
            //Get file text
            string file = reader.ReadToEnd();
            List<string> lines = new List<string>(file.Split('\n'));

            try
            {
                switch (type)
                {
                    //ASCII
                    case ".asc":
                        foreach (string line in lines)
                        {
                            if (line != "")
                            {
                                string[] elements = line.Split(',');
                                //ColorPoint pointData = new ColorPoint(new Point { X = Convert.ToDouble(elements[0].Replace(".", ",")), Y = Convert.ToDouble(elements[1].Replace(".", ",")), Z = Convert.ToDouble(elements[2].Replace(".", ",")) }, Color.Orange);
                                ColorPoint pointData = new ColorPoint(new Point { X = Convert.ToDouble(elements[0]), Y = Convert.ToDouble(elements[1]), Z = Convert.ToDouble(elements[2]) }, Color.Orange);
                                Points.Add(pointData);
                            }
                        }
                        GetSpace();
                        break;
                    //PCD
                    case ".pcd":
                        lines.RemoveRange(0, 11);
                        foreach (string line in lines)
                        {
                            if (line != "")
                            {
                                string[] elements = line.Split(' ');
                                float floatColor = (float)Convert.ToDouble(elements[3]);
                                //float floatColor = (float)Convert.ToDouble(elements[3].Replace(".", ","));
                                byte[] bytes = BitConverter.GetBytes(floatColor);
                                string hexColor = "#";
                                for (int i = 3; i >= 0; i--)
                                {
                                    hexColor += bytes[i].ToString("X");
                                }
                                Color color = System.Drawing.ColorTranslator.FromHtml(hexColor);
                                ColorPoint pointData = new ColorPoint(new Point { X = Convert.ToDouble(elements[0]), Y = Convert.ToDouble(elements[1]), Z = Convert.ToDouble(elements[2]) }, color);
                                //ColorPoint pointData = new ColorPoint(new Point { X = Convert.ToDouble(elements[0].Replace(".", ",")), Y = Convert.ToDouble(elements[1].Replace(".", ",")), Z = Convert.ToDouble(elements[2].Replace(".", ",")) }, color);
                                Points.Add(pointData);
                            }
                        }
                        GetSpace();
                        break;
                    //STL
                    case ".stl":
                        //Ascii
                        if (lines[1].Substring(2, 5) == "facet")
                        {
                            int count = lines.Count() / 7;
                            for (int i = 0; i < count; i++)
                            {
                                //string[] line1 = lines[i * 7 + 3].Replace(".", ",").Split(' ');
                                //string[] line2 = lines[i * 7 + 4].Replace(".", ",").Split(' ');
                                //string[] line3 = lines[i * 7 + 5].Replace(".", ",").Split(' ');
                                string[] line1 = lines[i * 7 + 3].Split(' ');
                                string[] line2 = lines[i * 7 + 4].Split(' ');
                                string[] line3 = lines[i * 7 + 5].Split(' ');
                                Point point1 = new Point { X = Convert.ToDouble(line1[7]), Y = Convert.ToDouble(line1[8]), Z = Convert.ToDouble(line1[9]) };
                                Point point2 = new Point { X = Convert.ToDouble(line2[7]), Y = Convert.ToDouble(line2[8]), Z = Convert.ToDouble(line2[9]) };
                                Point point3 = new Point { X = Convert.ToDouble(line3[7]), Y = Convert.ToDouble(line3[8]), Z = Convert.ToDouble(line3[9]) };
                                ColorPoint colorPoint1 = new ColorPoint(point1, Color.LightGreen);
                                ColorPoint colorPoint2 = new ColorPoint(point2, Color.LightGreen);
                                ColorPoint colorPoint3 = new ColorPoint(point3, Color.LightGreen);
                                if (!Points.Contains(colorPoint1))
                                    Points.Add(colorPoint1);
                                if (!Points.Contains(colorPoint2))
                                    Points.Add(colorPoint2);
                                if (!Points.Contains(colorPoint3))
                                    Points.Add(colorPoint3);
                                Mesh.Add(new Triangle(point1, point2, point3));
                            }
                        }
                        //Binary
                        else
                        {
                            string header;
                            using (var br = new BinaryReader(File.OpenRead(path), Encoding.ASCII))      //Fehlerhaft!!!
                            {
                                header = Encoding.ASCII.GetString(br.ReadBytes(80));
                                var triCount = br.ReadUInt32();
                                char[] buffer = new char[100];
                                for (int i = 0; i < triCount; i++)
                                {
                                    double[] norm = new double[3];
                                    for (int j = 0; j < 3; j++)
                                    {
                                        norm[j] = br.Read(buffer, i, 1);
                                    }
                                    double[] vertex1 = new double[3];
                                    for (int j = 0; j < 3; j++)
                                    {
                                        vertex1[j] = br.Read(buffer, i, 1);
                                    }
                                    double[] vertex2 = new double[3];
                                    for (int j = 0; j < 3; j++)
                                    {
                                        vertex2[j] = br.Read(buffer, i, 1);
                                    }
                                    double[] vertex3 = new double[3];
                                    for (int j = 0; j < 3; j++)
                                    {
                                        vertex3[j] = br.Read(buffer, i, 1);
                                    }
                                    var attribute = br.ReadUInt16();

                                    Mesh.Add(new Triangle(new Point(vertex1[0], vertex1[1], vertex1[2]), new Point(vertex2[0], vertex2[1], vertex2[2]), new Point(vertex3[0], vertex3[1], vertex3[2])));
                                }

                            }
                        }
                        break;
                    default:
                        status = false;
                        break;
                }
            }
            catch
            {
                status = false;
            }
            return status;
        }
    }


    /*
    public class Path : Geometry
    {
        public List<Position> path = new List<Position>();

        public void Clear()
        {
            path = new List<Position>();
        }

        public void AddPosition(Position position)
        {
            path.Add(position);
        }

        public void InsertPosition(int index, Position position)
        {
            if (index >= 0)
                path.Insert(index, position);
        }

        public void RemovePosition(Position position)
        {
            path.Remove(position);
        }

        public void RemovePosition(int index)
        {
            if (index >= 0)
                path.RemoveAt(index);
        }

        public void ChangePosition(int index, Position position)
        {
            if (index >= 0)
                path[index] = position;
        }
    }
    */

    public class Robot : Routine
    {
        //public Geometry.Position Base = new Position(0, 0, 0, 0, 0, 0);
        public Geometry.Position Base { get; set; }


        public enum ConnectionStatus { Disconnected, Connecting, Connected };
        public ConnectionStatus Status = ConnectionStatus.Disconnected;
        Socket Handler, Listener;
        Thread ThreadConnect;
        public bool Move = false;
        public static Position HomePos = new Position(525, 0, 890, 0, 90, 0);
        public static Position HomeAxs = new Position(0, -90, 90, 0, 0, 0);
        public Position Pos = new Position(HomePos);
        public Position Axs = new Position(HomeAxs);
        public Position TPos = new Position(HomePos);
        public Position TAxs = new Position(HomeAxs);
        public bool[] Out = new bool[6];
        public Task Action = Task.LIN;
        public double Vel = new double();
        public int S = new int(), T = new int();
        public Routine Program = new Routine();
        //bool Play = false;

        public void Connect(uint port, uint addressListIdx)
        {
            ThreadConnect = new Thread(delegate()
            {
                Status = ConnectionStatus.Connecting;
                IPHostEntry ipHostInfo = Dns.GetHostEntry(System.Net.Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[addressListIdx];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, (int)port);

                //Create a TCP/IP socket
                Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Open Socket and listen on network
                Listener.Bind(localEndPoint);
                Listener.Listen(1);

                //Program is suspended while waiting for an incoming connection
                Handler = Listener.Accept();

                //No connections are income
                Listener.Close();

                //Receive data for initialization
                Receive();
                TPos = Pos;
                TAxs = Axs;
                Status = ConnectionStatus.Connected;

                //Transmit data while connected
                Transmit();
            });
            ThreadConnect.Start();
        }

        public void Disconnect()
        {
            Status = ConnectionStatus.Disconnected;
            Thread.Sleep(500);
            if (Handler != null)
                Handler.Close();
            if (Listener != null)
                Listener.Close();
            if (ThreadConnect != null && ThreadConnect.IsAlive)
                ThreadConnect.Abort();
        }

        private void Receive()
        {
            String strReceive = null;
            byte[] bytes = new Byte[1024];
            string XMLstring;

            //Wait for data
            int bytesRec = 0;
            bytesRec = Handler.Receive(bytes);
            if (bytesRec == 0)
            {
                Disconnect();
            }
            else
            {
                //Convert bytes to a string
                //strReceive = String.Concat(strReceive, System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec));
                string Receive = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec);
                strReceive = Receive;
                int start = 0;
                int length = strReceive.IndexOf("</Robot>") + 8;
                strReceive.Substring(start, length);
                //Replace point with comma
                strReceive = strReceive.Replace(".", ",");
                //Get position parameters from string              
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strReceive);
                ///X
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Pos/@X").Value;
                if (XMLstring != "")
                    Pos.X = Convert.ToDouble(XMLstring);
                ///Y
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Pos/@Y").Value;
                if (XMLstring != "")
                    Pos.Y = Convert.ToDouble(XMLstring);
                ///Z
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Pos/@Z").Value;
                if (XMLstring != "")
                    Pos.Z = Convert.ToDouble(XMLstring);
                ///A
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Pos/@A").Value;
                if (XMLstring != "")
                    Pos.A = Convert.ToDouble(XMLstring);
                ///B
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Pos/@B").Value;
                if (XMLstring != "")
                    Pos.B = Convert.ToDouble(XMLstring);
                ///C
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Pos/@C").Value;
                if (XMLstring != "")
                    Pos.C = Convert.ToDouble(XMLstring);
                ///S
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Pos/@S").Value;
                if (XMLstring != "")
                    S = Convert.ToInt16(XMLstring);
                ///T
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Pos/@T").Value;
                if (XMLstring != "")
                    T = Convert.ToInt16(XMLstring);
                ///A1
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Axis/@A1").Value;
                if (XMLstring != "")
                    Axs.X = Convert.ToDouble(XMLstring);
                ///A2
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Axis/@A2").Value;
                if (XMLstring != "")
                    Axs.Y = Convert.ToDouble(XMLstring);
                ///A3
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Axis/@A3").Value;
                if (XMLstring != "")
                    Axs.Z = Convert.ToDouble(XMLstring);
                ///A4
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Axis/@A4").Value;
                if (XMLstring != "")
                    Axs.A = Convert.ToDouble(XMLstring);
                ///A5
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Axis/@A5").Value;
                if (XMLstring != "")
                    Axs.B = Convert.ToDouble(XMLstring);
                ///A6
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Axis/@A6").Value;
                if (XMLstring != "")
                    Axs.C = Convert.ToDouble(XMLstring);
                ///Vel        
                XMLstring = xmlDoc.SelectSingleNode("Robot/Data/Vel").InnerText;
                if (XMLstring != "")
                    Vel = Convert.ToDouble(XMLstring);
            }
        }

        private void Send()
        {
            Position MPos = new Position();
            MPos = TPos;
            Position MAxs = new Position();
            MAxs = TAxs;
            //Write XML-Text
            String strSend =
                "<Control>" +
                "<Data>" +
                "<Pos X=\"" + MPos.X + "\" Y=\"" + MPos.Y + "\" Z=\"" + MPos.Z + "\" A=\"" + MPos.A + "\" B=\"" + MPos.B + "\" C=\"" + MPos.C + "\" S=\"0\" T=\"0\"></Pos>" +
                "<Axis A1=\"" + MAxs.X + "\" A2=\"" + MAxs.Y + "\" A3=\"" + MAxs.Z + "\" A4=\"" + MAxs.A + "\" A5=\"" + MAxs.B + "\" A6=\"" + MAxs.C + "\"></Axis>" +
                "<Out DO1=\"0\" DO2=\"0\" DO3=\"0\" DO4=\"0\" DO5=\"0\" DO6=\"0\"></Out>" +
                "</Data>" +
                "<Status><Action P=\"" + CheckMoveType(Action, Task.PTP) + "\" L=\"" + CheckMoveType(Action, Task.LIN) + "\" A=\"" + CheckMoveType(Action, Task.AXS) + "\" O=\"FALSE\"></Action><IsActive>1</IsActive></Status>" +
                "</Control>";
            //Replace comma
            strSend = strSend.Replace(",", ".");
            //Convert to bytes
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(strSend);
            //Send bytes via Ethernet
            Handler.Send(msg, 0, msg.Length, System.Net.Sockets.SocketFlags.None);

            Thread.Sleep(200);
        }

        private void Transmit()
        {
            //Send while connected
            Thread ThreadSend = new Thread(delegate()
            {
                while (Status == ConnectionStatus.Connected)
                {
                    try
                    {
                        Send();
                    }
                    catch
                    {
                        Thread.Sleep(200);
                    }
                }
            });
            ThreadSend.Start();

            //Receive while connected
            Thread ThreadReceive = new Thread(delegate()
            {
                while (Status == ConnectionStatus.Connected)
                {
                    try
                    {
                        Receive();
                    }
                    catch
                    {
                        Thread.Sleep(200);
                    }
                }
            });
            ThreadReceive.Start();

            //Compare while connected
            Thread ThreadCompare = new Thread(delegate()
            {
                while (Status == ConnectionStatus.Connected)
                {
                    try
                    {
                        //Wait for achievement
                        if (Action == Task.AXS)
                        {
                            while (!TAxs.Equals(Axs))
                            {
                                Move = true;
                                Thread.Sleep(10);
                            }
                        }
                        else
                        {
                            while (!Pos.Equals(TPos))
                            {
                                Move = true;
                                Thread.Sleep(10);
                            }
                        }
                        Move = false;
                    }
                    catch
                    {
                        Thread.Sleep(200);
                    }
                }
            });
            ThreadCompare.Start();
        }

        public void MoveToPosition(Position position)
        {
            if (Status == ConnectionStatus.Connected)
                TPos = position;
            else
                Pos = position;
        }

        public void MoveToAxis(Position axis)
        {
            if (Status == ConnectionStatus.Connected)
                TAxs = axis;
            else
                Axs = axis;
        }

        public void MoveToHome()
        {
            TPos = HomePos;
            TAxs = HomeAxs;
        }

        public void SwitchMode(Task mode)
        {
            Action = mode;
            if (mode == Task.LIN || mode == Task.PTP)
                TPos = Pos;
            else if (mode == Task.AXS)
                TAxs = Axs;
        }

        public void Execute(Command command)
        {
            Action = command.Task;
            if (Action == Task.LIN || Action == Task.PTP)
            {
                MoveToPosition(command.Values);
                Thread.Sleep(3000);
            }
            else if (Action == Task.AXS)
            {
                MoveToAxis(command.Values);
                Thread.Sleep(3000);
            }
            else if (Action == Task.ITE)
            {
                Action = Task.LIN;
                IterateTo(command.Values);
                if (Status != ConnectionStatus.Connected)
                    Thread.Sleep(1000);
            }
            else if (Action == Task.COOP)
            {
                Action = Task.LIN;
                MoveToPosition(command.Values);
                Thread.Sleep(3000);
            }
        }

        /*
        public void ExecuteProgram(int index)
        {
            Thread ThreadExecute = new Thread(delegate()
            {
                Play = true;
                for (int i = index; i < Program.Commands.Count; i++)
                {
                    //Move
                    if (Play)
                    {
                        index = i;
                        if (Program.Commands[i].Task != Task.COOP)
                            Execute(Program.Commands[i]);
                        //else { }
                    }
                }
                //Reset
                if (Play)
                {
                    index = 0;
                    Thread.Sleep(1000);
                }
                Play = false;
            });
        }

        public void ExecuteStep(int index)
        {
            Thread ThreadExecute = new Thread(delegate()
            {
                Play = true;
                //Move
                Execute(Program.Commands[index]);
                //Reset
                if (index < Program.Commands.Count - 1)
                    index++;
                else
                    index = 0;
                Play = false;
            });
        }

        public void StopExecution()
        {
            Play = false;
        }
        */

        public void IterateTo(Position position)
        {
            double ink = 1;
            Position newPos = Pos;
            //Get changes
            double[] delta = { position.X - newPos.X, position.Y - newPos.Y, position.Z - newPos.Z, position.A - newPos.A, position.B - newPos.B, position.C - newPos.C };
            //Check rotation
            for (int i = 3; i < 6; i++)
            {
                if (Math.Abs(delta[i]) > 180)
                {
                    double S = 0, F = 0;
                    switch (i)
                    {
                        case 3:
                            S = newPos.A;
                            F = position.A;
                            break;
                        case 4:
                            S = newPos.B;
                            F = position.B;
                            break;
                        case 5:
                            S = newPos.C;
                            F = position.C;
                            break;
                    }
                    if (S > F)
                        delta[i] = F - S + 360;
                    else
                        delta[i] = F - S - 360;
                }
            }
            //Get maximum changes
            double deltaMax = 0;
            if (Math.Abs(delta.Max()) > Math.Abs(delta.Min()))
                deltaMax = delta.Max();
            else
                deltaMax = Math.Abs(delta.Min());
            //Count steps
            int steps = Convert.ToInt16(deltaMax / ink);
            if (steps != 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    delta[i] = delta[i] / steps;
                }
                //Drive
                for (int i = 0; i < steps; i++)
                {
                    //Get steps
                    newPos.X = newPos.X + delta[0];
                    newPos.Y = newPos.Y + delta[1];
                    newPos.Z = newPos.Z + delta[2];
                    newPos.A = newPos.A + delta[3];
                    newPos.B = newPos.B + delta[4];
                    newPos.C = newPos.C + delta[5];

                    //Drive to position
                    MoveToPosition(newPos);
                    Thread.Sleep(2000);
                }
            }
        }

        string CheckMoveType(Task type, Task checktype)
        {
            string status = "FALSE";
            if (type == checktype)
                status = "TRUE";
            return status;
        }

        /*
        public enum Direction { X, Y, Z }
         * 
        public bool Calibration(Robot robot, Scanner scanner, out Position transformation, out double diviation)
        {
            transformation = new Position();
            int CalbDrift = 40;
            int CalbStep = 1;
            bool status = true;
            bool direction = true;
            diviation = 0;

            //Get robots to start positions
            robot.TPos = new Position(227.39, -447.70, 806.14, -178.81, 1.57, 179.96);
            TPos = new Position(285.68, 781.42, 305.30, -90, 0, 0);
            Thread.Sleep(8000);

            //Search first minimum
            if (SearchPin(scanner, Direction.X, !direction))
            {
                Thread.Sleep(1000);

                //Save first position
                Position Pos1 = Pos;
                double LaserMin1 = scanner.Minimum.Z;

                //Move in y-direction
                TPos.Y += CalbDrift;
                Thread.Sleep(4000);

                //Get further search direction
                double LaserMinCheck1 = scanner.Minimum.Z;
                TPos.X += CalbStep / 10;
                Thread.Sleep(2000);
                double LaserMinCheck2 = scanner.Minimum.Z;

                //Optional turn over of serach direction
                if (LaserMinCheck1 < LaserMinCheck2)
                    direction = false;

                //Search second minimum
                if (SearchPin(scanner, Direction.X, !direction))
                {
                    Thread.Sleep(1000);

                    //Save second position
                    Position Pos2 = Pos;
                    double LaserMin2 = scanner.Minimum.Z;

                    //Calculate chnages
                    double dx = Pos2.X - Pos1.X;
                    double dy = CalbDrift;
                    double dz = LaserMin2 - LaserMin1;

                    //Calculate angles
                    transformation.A = Math.Asin(dx / dy) * 180 / Math.PI;
                    transformation.C = Math.Asin(dz / dy) * 180 / Math.PI;

                    //Move in z-direction
                    TPos.Z += CalbDrift;
                    Thread.Sleep(2000);

                    //Get further search direction
                    LaserMinCheck1 = scanner.Minimum.Z;
                    TPos.X += CalbStep / 10;
                    Thread.Sleep(2000);
                    LaserMinCheck2 = scanner.Minimum.Z;

                    //Optional turn over of serach direction
                    if (LaserMinCheck1 < LaserMinCheck2)
                        direction = false;
                    else
                        direction = true;

                    //Search third minimum
                    if (SearchPin(scanner, Direction.X, !direction))
                    {
                        Thread.Sleep(1000);

                        //Save third position
                        Position Pos3 = Pos;

                        //Calculate chnages
                        dx = Pos3.X - Pos2.X;
                        dz = CalbDrift;

                        //Calculate angle
                        transformation.B = Math.Asin(dx / dz) * 180 / Math.PI;

                        //Calculate translation
                        Position RefPin = new Position(-14.248, 15.185, 118.637, 0, 0, 0);
                        RefPin.Rotate(Pos.A, Pos.B, Pos.C);
                        RefPin.Translate(Pos.X, Pos.Y, Pos.Z);

                        Position ProfileMinimum = new Position(0, - scanner.Minimum.X, scanner.Minimum.Z, 0, 0, 0);
                        ProfileMinimum.Rotate(1.312, 1.588, 0.013);
                        Position LaserSensor = new Position(-69.595, -44.055, 170.336, 0, 0, 0);
                        ProfileMinimum.Translate(LaserSensor.X, LaserSensor.Y, LaserSensor.Z);
                        ProfileMinimum.Rotate(robot.Pos.A, robot.Pos.B, robot.Pos.C);
                        ProfileMinimum.Translate(robot.Pos.X, robot.Pos.Y, robot.Pos.Z);

                        transformation.X = ProfileMinimum.X - RefPin.X;
                        transformation.Y = ProfileMinimum.Y - RefPin.Y;
                        transformation.Z = ProfileMinimum.Z - RefPin.Z;

                        //Get robots to control positions
                        //robot.TPos = new Position(50, -330, 730, 145, 45, 180);
                        //TPos = new Position(390, 710, 354, -125, 5, 45);
                        robot.TPos = new Position(-162.34, -453.70, 806.14, 91.19, 1.57, 179.96);
                        TPos = new Position(-214.32, 781.42, 305.30, -90, 0, 0);
                        Thread.Sleep(10000);

                        //Search tip
                        //if (SearchTipZ(scanner))
                        if (SearchPin(scanner, Direction.Y, false))
                        {
                            Thread.Sleep(1000);

                            //Caculate accuracy
                            RefPin = new Position(-14.248, 15.185, 118.637, 0, 0, 0); 
                            RefPin.Rotate(Pos.A, Pos.B, Pos.C);
                            RefPin.Translate(Pos.X, Pos.Y, Pos.Z);

                            ProfileMinimum = new Position(0, -scanner.Minimum.X, scanner.Minimum.Z, 0, 0, 0);
                            ProfileMinimum.Rotate(1.312, 1.588, 0.013);
                            LaserSensor = new Position(-69.595, -44.055, 170.336, 0, 0, 0);
                            ProfileMinimum.Translate(LaserSensor.X, LaserSensor.Y, LaserSensor.Z);
                            ProfileMinimum.Rotate(robot.Pos.A, robot.Pos.B, robot.Pos.C);
                            ProfileMinimum.Translate(robot.Pos.X, robot.Pos.Y, robot.Pos.Z);

                            double deltaX = transformation.X - (ProfileMinimum.X - RefPin.X);
                            double deltaY = transformation.Y - (ProfileMinimum.Y - RefPin.Y);
                            double deltaZ = transformation.Z - (ProfileMinimum.Z - RefPin.Z);

                            diviation = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2) + Math.Pow(deltaZ, 2));

                            //Save results to a file
                            string path = string.Concat(Environment.CurrentDirectory, @"\robotCalibrations.txt");
                            using (StreamWriter file = new StreamWriter(path, true))
                            {
                                file.WriteLine(DateTime.Now.Date.Day + "." + DateTime.Now.Date.Month + "." + DateTime.Now.Date.Year + "; " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "; " + transformation.X.ToString("F4") + "; " + transformation.Y.ToString("F4") + "; " + transformation.Z.ToString("F4") + "; " + transformation.A.ToString("F4") + "; " + transformation.B.ToString("F4") + "; " + transformation.C.ToString("F4") + "; " + diviation.ToString("F4"));
                            }
                        }
                        else
                            status = false;
                    }
                    else
                        status = false;
                }
                else
                    status = false;
            }
            else
                status = false;

            return status;
        }

        public bool Calibration(Scanner scanner, List<Position> startpositions, List<bool> directions, out Position transformation, out double diviation)
        {
            transformation = new Position();
            List<Position> positions = new List<Position>();
            Point delta = new Point(); ;
            bool located = false;
            int step = 1;
            Point L0 = new Point(), L1 = new Point(), L2 = new Point();
            diviation = 0;

            do
            {
                located = false;
                switch (step)
                {
                    case 1:
                        //Search first position
                        TPos = startpositions[0];
                        Thread.Sleep(5000);
                        if (SearchPin(scanner, Direction.X, directions[0]))
                        {
                            delta.X = Pos.X;
                            delta.Y = Pos.Y;
                            if (SearchMiddle(scanner, Direction.Y, directions[0]))
                            {
                                if (SearchPin(scanner, Direction.X, directions[0]))
                                {
                                    positions.Add(Pos);
                                    L0 = scanner.Minimum;
                                    located = true;
                                }
                            }
                        }
                        break;
                    case 2:
                        //Search second position
                        TPos = startpositions[1];
                        Thread.Sleep(5000);
                        if (SearchPin(scanner, Direction.Y, directions[1]))
                        {
                            if (SearchMiddle(scanner, Direction.X, directions[1]))
                            {
                                if (SearchPin(scanner, Direction.Y, directions[1]))
                                {
                                    positions.Add(Pos);
                                    located = true;
                                }
                            }
                        }
                        break;
                    case 3:
                        //Search third position
                        TPos = startpositions[2];
                        Thread.Sleep(5000);
                        if (SearchPin(scanner, Direction.X, directions[2]))
                        {
                            if (SearchMiddle(scanner, Direction.Y, directions[2]))
                            {
                                if (SearchPin(scanner, Direction.X, directions[2]))
                                {
                                    positions.Add(Pos);
                                    L1 = scanner.Minimum;
                                    located = true;
                                }
                            }
                        }
                        break;
                    case 4:
                        //Search fourth position
                        TPos = startpositions[3];
                        Thread.Sleep(5000);
                        if (SearchPin(scanner, Direction.Y, directions[3]))
                        {
                            if (SearchMiddle(scanner, Direction.X, directions[3]))
                            {
                                if (SearchPin(scanner, Direction.Y, directions[3]))
                                {
                                    positions.Add(Pos);
                                    located = true;
                                }
                            }
                        }
                        break;
                    case 5:
                        //Search fifth position
                        //TPos = startpositions[4]; // Falsch !!! Position 4 mit C - 5° !!!
                        Position Pos5 = positions[2];
                        Pos5.C = 175;
                        TPos = Pos5;
                        Thread.Sleep(5000);
                        if (SearchPin(scanner, Direction.X, directions[4]))
                        {
                            positions.Add(Pos);
                            L2 = scanner.Minimum;
                            located = true;
                        }
                        break;
                }
                step++;
            } while (located && step <= 5);

            if (located)
            {
                //Calculated transformation
                //delta.X = -(positions[0].X - delta.X);
                delta.X = positions[0].X - delta.X;
                delta.Y = positions[0].Y - delta.Y;
                transformation.A = Math.Atan(delta.X / delta.Y) * (180 / Math.PI);

                Point p = Subtract(ToPoint(positions[1]), ToPoint(positions[0]));
                Point v1 = new Point();
                v1.X = -(p.Y - p.X) / 2;
                v1.Y = -(p.X + p.Y) / 2;
                p = Subtract(ToPoint(positions[3]), ToPoint(positions[2]));
                Point v2 = new Point();
                v2.X = -(p.Y - p.X) / 2;
                v2.Y = -(p.X + p.Y) / 2;

                delta.X = Subtract(v2, v1).X;
                delta.Y = Subtract(v2, v1).Y;
                delta.Z = Subtract(ToPoint(positions[2]), ToPoint(positions[0])).Z;

                Point deltaO = new Point();
                deltaO.Z = -L0.Z;
                deltaO.X = delta.X * deltaO.Z / delta.Z;
                deltaO.Y = delta.Y * deltaO.Z / delta.Z;

                transformation.X = v1.X + deltaO.X;
                transformation.Y = v1.Y + deltaO.Y;
                transformation.B = Math.Atan(delta.X / delta.Z) * (180 / Math.PI);
                transformation.C = Math.Atan(delta.Y / delta.Z) * (180 / Math.PI);

                double C = -5 * Math.PI / 180;
                RotationMatrix RL = new RotationMatrix(transformation.A, transformation.B, transformation.C);

                L1 = new Point(L1.Y, -L1.X, L1.Z);
                L1 = Multiply(RL, L1);
                L2 = new Point(L2.Y, -L2.X, L2.Z);
                L2 = Multiply(RL, L2);
                transformation.Z = (-L2.Y * Math.Cos(C) + L2.Z * Math.Sin(C) - transformation.Y * Math.Cos(C) + transformation.Y + L1.Y) / (-1 * Math.Sin(C));

                //Control transformation
                TPos = startpositions[5];
                Thread.Sleep(5000);
                if (SearchPin(scanner, Direction.X, directions[5]))
                {
                    positions.Add(Pos);

                    Point L3 = scanner.Minimum;
                    L3 = new Point(L3.Y, -L3.X, L3.Z);
                    L3 = Multiply(RL, L3);

                    RotationMatrix R1 = new RotationMatrix(positions[2].A, positions[2].B, positions[2].C);
                    Point d = new Point(transformation.X, transformation.Y, transformation.Z);
                    Point S1 = Sum(ToPoint(positions[2]), Multiply(R1, (Sum(d, L1))));
                    RotationMatrix R2 = new RotationMatrix(positions[5].A, positions[5].B, positions[5].C);
                    Point S2 = Sum(ToPoint(positions[5]), Multiply(R2, (Sum(d, L3))));

                    Vector S = new Vector(S2, S1);
                    diviation = S.Lenght;
                }

                //Save results to a file
                string path = string.Concat(Environment.CurrentDirectory, @"\calibrations.txt");
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(DateTime.Now.Date.Day + "." + DateTime.Now.Date.Month + "." + DateTime.Now.Date.Year + "; " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "; " + transformation.X.ToString("F4") + "; " + transformation.Y.ToString("F4") + "; " + transformation.Z.ToString("F4") + "; " + transformation.A.ToString("F4") + "; " + transformation.B.ToString("F4") + "; " + transformation.C.ToString("F4") + "; " + diviation.ToString("F4"));
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SearchPin(Scanner scanner, Direction direction, bool sign)
        {
            int limit = 20;
            double step;
            if (sign)
                step = -1;
            else
                step = 1;
            double lastZ, currentZ;

            //Get laser line to tip
            do
            {
                lastZ = scanner.Minimum.Z;
                if (direction == Direction.X)
                    TPos.X += step;
                else if (direction == Direction.Y)
                    TPos.Y += step;
                Thread.Sleep(1000);
                limit--;
                currentZ = scanner.Minimum.Z;
            } while (lastZ >= currentZ && limit > 0);
            if (limit > 0)
            {
                limit = 20;
                do
                {
                    lastZ = scanner.Minimum.Z;
                    if (direction == Direction.X)
                        TPos.X -= step / 10;
                    else if (direction == Direction.Y)
                        TPos.Y -= step / 10;
                    Thread.Sleep(500);
                    limit--;
                    currentZ = scanner.Minimum.Z;
                }
                while (lastZ >= currentZ && limit > 0);
                if (limit > 0)
                {
                    limit = 20;
                    do
                    {
                        lastZ = scanner.Minimum.Z;
                        if (direction == Direction.X)
                            TPos.X += step / 50;
                        else if (direction == Direction.Y)
                            TPos.Y += step / 50;
                        Thread.Sleep(500);
                        limit--;
                        currentZ = scanner.Minimum.Z;
                    }
                    while (lastZ >= currentZ && limit > 0);
                }
            }

            Thread.Sleep(2000);

            if (limit <= 0)
                return false;
            else
                return true;
        }

        private bool SearchMiddle(Scanner scanner, Direction direction, bool sign)
        {
            try
            {
                double move = 0;
                int step = 0;
                do
                {
                    move = scanner.Minimum.X;
                    if (direction == Direction.X)
                        TPos.X -= move;
                    else if (direction == Direction.Y)
                        TPos.Y += move;
                    Thread.Sleep(1000);
                    step++;
                } while (Math.Abs(move) > 0.01 && step < 5);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool SearchTipZ(Scanner scanner)
        {
            int limit = 20;
            int step = -1;
            double lastZ, currentZ;

            do
            {
                lastZ = scanner.Minimum.Z;
                TPos.Z += step;
                Thread.Sleep(1000);
                limit--;
                currentZ = scanner.Minimum.Z;
            } while (lastZ >= currentZ && limit > 0);
            if (limit > 0)
            {
                limit = 20;
                do
                {
                    lastZ = scanner.Minimum.Z;
                    TPos.Z -= step / 10;                   
                    Thread.Sleep(500);
                    limit--;
                    currentZ = scanner.Minimum.Z;
                }
                while (lastZ >= currentZ && limit > 0);
                if (limit > 0)
                {
                    limit = 20;
                    do
                    {
                        lastZ = scanner.Minimum.Z;
                        TPos.Z += step / 100;
                        Thread.Sleep(200);
                        limit--;
                        currentZ = scanner.Minimum.Z;
                    }
                    while (lastZ >= currentZ && limit > 0);
                }
            }

            Thread.Sleep(2000);

            if (limit <= 0)
                return false;
            else
                return true;
        }
        */

        public List<Position> GetLinkPositions()
        {
            List<Position> positions = new List<Position>();
            Point prior = new Point(Base.X, Base.Y, Base.Z);
            Position tempPosition;
            Matrix4 tempMatrix = new Matrix4();

            Matrix4 A1 = GetLinkTransformation(new Link(-TAxs.X * Math.PI / 180, 400, 25, -Math.PI / 2));
            Matrix4 A2 = GetLinkTransformation(new Link(TAxs.Y * Math.PI / 180, 0, 455, 0));
            Matrix4 A3 = GetLinkTransformation(new Link(TAxs.Z * Math.PI / 180 - Math.PI / 2, 0, 35, -Math.PI / 2));
            Matrix4 A4 = GetLinkTransformation(new Link(-TAxs.A * Math.PI / 180, 420, 0, Math.PI / 2));
            Matrix4 A5 = GetLinkTransformation(new Link(TAxs.B * Math.PI / 180 + Math.PI, 0, 0, Math.PI / 2));
            Matrix4 A6 = GetLinkTransformation(new Link(-TAxs.C * Math.PI / 180, 80, 0, 0));

            tempPosition = GetLinkTransformationPosition(A1);
            tempPosition.Rotate(Base.A, Base.B, Base.C);
            tempPosition.Translate(Base.X, Base.Y, Base.Z);
            positions.Add(tempPosition);

            Matrix4 sumMatrix = new Matrix4();
            sumMatrix.Multiply(A1, A2);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(Base.A, Base.B, Base.C);
            tempPosition.Translate(Base.X, Base.Y, Base.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A3);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(Base.A, Base.B, Base.C);
            tempPosition.Translate(Base.X, Base.Y, Base.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A4);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(Base.A, Base.B, Base.C);
            tempPosition.Translate(Base.X, Base.Y, Base.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A5);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(Base.A, Base.B, Base.C);
            tempPosition.Translate(Base.X, Base.Y, Base.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A6);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(Base.A, Base.B, Base.C);
            tempPosition.Translate(Base.X, Base.Y, Base.Z);
            positions.Add(tempPosition);

            return positions;
        }

        public List<Position> GetLinkPositions(Position newBase)
        {
            List<Position> positions = new List<Position>();
            Point prior = new Point(newBase.X, newBase.Y, newBase.Z);
            Position tempPosition;
            Matrix4 tempMatrix = new Matrix4();

            Matrix4 A1 = GetLinkTransformation(new Link(-TAxs.X * Math.PI / 180, 400, 25, -Math.PI / 2));
            Matrix4 A2 = GetLinkTransformation(new Link(TAxs.Y * Math.PI / 180, 0, 455, 0));
            Matrix4 A3 = GetLinkTransformation(new Link(TAxs.Z * Math.PI / 180 - Math.PI / 2, 0, 35, -Math.PI / 2));
            Matrix4 A4 = GetLinkTransformation(new Link(-TAxs.A * Math.PI / 180, 420, 0, Math.PI / 2));
            Matrix4 A5 = GetLinkTransformation(new Link(TAxs.B * Math.PI / 180 + Math.PI, 0, 0, Math.PI / 2));
            Matrix4 A6 = GetLinkTransformation(new Link(-TAxs.C * Math.PI / 180, 80, 0, 0));

            tempPosition = GetLinkTransformationPosition(A1);
            tempPosition.Rotate(newBase.A, newBase.B, newBase.C);
            tempPosition.Translate(newBase.X, newBase.Y, newBase.Z);
            positions.Add(tempPosition);

            Matrix4 sumMatrix = new Matrix4();
            sumMatrix.Multiply(A1, A2);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(newBase.A, newBase.B, newBase.C);
            tempPosition.Translate(newBase.X, newBase.Y, newBase.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A3);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(newBase.A, newBase.B, newBase.C);
            tempPosition.Translate(newBase.X, newBase.Y, newBase.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A4);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(newBase.A, newBase.B, newBase.C);
            tempPosition.Translate(newBase.X, newBase.Y, newBase.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A5);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(newBase.A, newBase.B, newBase.C);
            tempPosition.Translate(newBase.X, newBase.Y, newBase.Z);
            positions.Add(tempPosition);

            sumMatrix.Multiply(tempMatrix, A6);
            tempMatrix = sumMatrix;
            tempPosition = GetLinkTransformationPosition(tempMatrix);
            tempPosition.Rotate(newBase.A, newBase.B, newBase.C);
            tempPosition.Translate(newBase.X, newBase.Y, newBase.Z);
            positions.Add(tempPosition);

            return positions;
        }


        private Matrix4 GetLinkTransformation(Link link)
        {
            Matrix4 matrix = new Matrix4();
            matrix.C1R1 = Math.Cos(link.theta);
            matrix.C1R2 = Math.Sin(link.theta);
            matrix.C1R3 = 0;
            matrix.C1R4 = 0;
            matrix.C2R1 = -Math.Sin(link.theta) * Math.Cos(link.alpha);
            matrix.C2R2 = Math.Cos(link.theta) * Math.Cos(link.alpha);
            matrix.C2R3 = Math.Sin(link.alpha);
            matrix.C2R4 = 0;
            matrix.C3R1 = Math.Sin(link.theta) * Math.Sin(link.alpha);
            matrix.C3R2 = -Math.Cos(link.theta) * Math.Sin(link.alpha);
            matrix.C3R3 = Math.Cos(link.alpha);
            matrix.C3R4 = 0;
            matrix.C4R1 = link.a * Math.Cos(link.theta);
            matrix.C4R2 = link.a * Math.Sin(link.theta);
            matrix.C4R3 = link.d;
            matrix.C4R4 = 1;
            return matrix;
        }

        private Position GetLinkTransformationPosition(Matrix4 transformation)
        {
            double a, b, c;
            a = Math.Atan2(transformation.C1R2, transformation.C1R1);
            b = Math.Atan2(-transformation.C1R3, Math.Sqrt(Math.Pow(transformation.C2R3, 2) + Math.Pow(transformation.C3R3, 2)));
            c = Math.Atan2(transformation.C2R3, transformation.C3R3);
            if (b > -Math.PI / 2 && b < Math.PI / 2)
            {
                a = Math.Atan2(-transformation.C1R2, -transformation.C1R1);
                b = Math.Atan2(-transformation.C1R3, -Math.Sqrt(Math.Pow(transformation.C2R3, 2) + Math.Pow(transformation.C3R3, 2)));
                c = Math.Atan2(-transformation.C2R3, -transformation.C3R3);
            }
            a = a * 180 / Math.PI;
            b = b * 180 / Math.PI;
            c = c * 180 / Math.PI;
            Position position = new Position(transformation.C4R1, transformation.C4R2, transformation.C4R3, a, b, c);
            return position;
        }
    }

    public class Routine : Geometry
    {
        public List<Command> Commands = new List<Command>();
        public enum Task { PTP = 0, LIN = 1, CIRC = 2, AXS = 3, ITE = 4, COOP = 5 }

        public struct Command
        {
            public Task Task;
            public Position Values;

            public Command(Task task, Position values)
            {
                Task = task;
                Values = values;
            }
        }

        public void Clear()
        {
            Commands = new List<Command>();
        }

        public void AddCommand(Command command)
        {
            Commands.Add(command);
        }

        public void AddCommand(Task task, Position values)
        {
            Commands.Add(new Command(task, values));
        }

        public void InsertCommand(int index, Command command)
        {
            if (index >= 0)
                Commands.Insert(index, command);
        }

        public void RemoveCommand(int index)
        {
            if (index >= 0 && index < Commands.Count())
            {
                Commands.RemoveAt(index);
            }
        }

        public void CopyCommand(int index)
        {
            if (index >= 0)
                InsertCommand(index + 1, Commands[index]);
        }

        public void SwapCommand(int indexA, int indexB)
        {
            if (indexA >= 0 && indexB >= 0)
            {
                Command temp = Commands[indexA];
                Commands[indexA] = Commands[indexB];
                Commands[indexB] = temp;
            }
        }

        public bool Save(string path)
        {
            bool status = true;

            //Get file extention 
            string type = System.IO.Path.GetExtension(path);

            switch (type)
            {
                //crp
                case ".crp":
                    StreamWriter writer = new StreamWriter(path);
                    foreach (Command command in Commands)
                    {
                        writer.WriteLine(command.Task.ToString() + "," + command.Values.X.ToString("F3").Replace(",", ".") + "," + command.Values.Y.ToString("F3").Replace(",", ".") + "," + command.Values.Z.ToString("F3").Replace(",", ".") + "," + command.Values.A.ToString("F3").Replace(",", ".") + "," + command.Values.B.ToString("F3").Replace(",", ".") + "," + command.Values.C.ToString("F3").Replace(",", "."));
                    }
                    writer.Dispose();
                    writer.Close();
                    break;
                //src
                case ".src":
                    string subpath = System.IO.Path.GetDirectoryName(path) + "\\" + System.IO.Path.GetFileNameWithoutExtension(path);
                    writeSRC(subpath + "_master.src", 1, 0);
                    writeSRC(subpath + "_slave.src", 2, 3);
                    writer = new StreamWriter(path);
                    writer.WriteLine("DEF " + System.IO.Path.GetFileNameWithoutExtension(path) + "()\r\n");
                    writer.WriteLine(";1. Dub the master and the slave program to the respective control");
                    writer.WriteLine(";2. Connect master controller and slave controller");
                    writer.WriteLine(";3. Run the program on the master controller\r\n");
                    writer.WriteLine("END");
                    writer.Dispose();
                    writer.Close();
                    break;
                default:
                    status = false;
                    break;
            }
            return status;
        }

        private void writeSRC(string path, int robotIndex, int toolIndex)
        {
            StreamWriter writer = new StreamWriter(path);
            int n = 1;
            bool coop = false;
            //Header
            writer.WriteLine("DEF " + System.IO.Path.GetFileNameWithoutExtension(path) + "()\r\n");
            writer.WriteLine("$BWDSTART = FALSE\r\nPDAT_ACT=PDEFAULT\r\nLDAT_ACT=LDEFAULT\r\nFDAT_ACT=FHOME\r\nBAS(#PTP_PARAMS,100)\r\nBAS(#CP_PARAMS,2)\r\n$H_POS=XHOME\r\n$ORI_TYPE=#CONSTANT\r\n$CIRC_TYPE=#BASE\r\n");
            writer.WriteLine("GLOBAL INTERRUPT DECL 3 WHEN $STOPMESS==TRUE DO IR_STOPM()\r\nINTERRUPT ON 3\r\nBAS(#INITMOV,0)\r\n");
            //Tool
            writer.WriteLine("$ACT_TOOL=" + toolIndex);
            if (toolIndex > 0)
                writer.WriteLine("$TOOL=TOOL_DATA[" + toolIndex + "]");
            else
                writer.WriteLine("$TOOL=$NULLFRAME");
            //Commands
            writer.WriteLine("\r\nPTP XHOME\r\n");
            foreach (Command command in Commands)
            {
                switch (command.Task)
                {
                    case Task.PTP:
                        if (robotIndex == 1)
                            writer.WriteLine("PTP {X " + command.Values.X.ToString("F3").Replace(",", ".") + ", Y " + command.Values.Y.ToString("F3").Replace(",", ".") + ", Z " + command.Values.Z.ToString("F3").Replace(",", ".") + ", A " + command.Values.A.ToString("F3").Replace(",", ".") + ", B " + command.Values.B.ToString("F3").Replace(",", ".") + ", C " + command.Values.C.ToString("F3").Replace(",", ".") + ", S 'B110', T 'B110011'}");
                        else if (robotIndex == 2)
                            writer.WriteLine("PTP {X " + command.Values.X.ToString("F3").Replace(",", ".") + ", Y " + command.Values.Y.ToString("F3").Replace(",", ".") + ", Z " + command.Values.Z.ToString("F3").Replace(",", ".") + ", A " + command.Values.A.ToString("F3").Replace(",", ".") + ", B " + command.Values.B.ToString("F3").Replace(",", ".") + ", C " + command.Values.C.ToString("F3").Replace(",", ".") + ", S 'B110', T 'B111010'}");
                        break;
                    case Task.LIN:
                        writer.WriteLine("LIN {X " + command.Values.X.ToString("F3").Replace(",", ".") + ", Y " + command.Values.Y.ToString("F3").Replace(",", ".") + ", Z " + command.Values.Z.ToString("F3").Replace(",", ".") + ", A " + command.Values.A.ToString("F3").Replace(",", ".") + ", B " + command.Values.B.ToString("F3").Replace(",", ".") + ", C " + command.Values.C.ToString("F3").Replace(",", ".") + "}");
                        break;
                    case Task.COOP:
                        writer.WriteLine("SYNCCMD(#ProgSync,\"A" + n + "\",3)");
                        writer.WriteLine("LIN {X " + command.Values.X.ToString("F3").Replace(",", ".") + ", Y " + command.Values.Y.ToString("F3").Replace(",", ".") + ", Z " + command.Values.Z.ToString("F3").Replace(",", ".") + ", A " + command.Values.A.ToString("F3").Replace(",", ".") + ", B " + command.Values.B.ToString("F3").Replace(",", ".") + ", C " + command.Values.C.ToString("F3").Replace(",", ".") + "}");
                        n++;
                        coop = true;
                        break;
                }
            }
            writer.WriteLine("\r\nWAIT SEC 2.0\r\n");
            if (coop)
                writer.WriteLine("SYNCCMD(#ProgSync,\"A" + n + "\",3)");
            writer.WriteLine("PTP XHOME\r\n");
            writer.WriteLine("END");
            writer.Dispose();
            writer.Close();
        }

        public bool Load(string path)
        {
            Clear();

            string file;
            List<string> lines;
            bool status = true;
            int toolIndex = 0;
            Task task = new Task();

            //Get file extenstion
            string type = System.IO.Path.GetExtension(path);

            try
            {
                switch (type)
                {
                    //crp
                    case ".crp":
                        using (StreamReader reader = new StreamReader(path))
                        {
                            //Get file text
                            file = reader.ReadToEnd();
                            lines = new List<string>(file.Split('\r', '\n'));
                            //Get commands
                            foreach (string line in lines)
                            {
                                if (line != "")
                                {
                                    string[] elements = line.Split(',');
                                    task = GetTask(elements[0]);
                                    Position position = new Position(Convert.ToDouble(elements[1].Replace(".", ",")), Convert.ToDouble(elements[2].Replace(".", ",")), Convert.ToDouble(elements[3].Replace(".", ",")), Convert.ToDouble(elements[4].Replace(".", ",")), Convert.ToDouble(elements[5].Replace(".", ",")), Convert.ToDouble(elements[6].Replace(".", ",")));
                                    AddCommand(task, position);
                                }
                            }
                        }
                        break;
                    //src
                    case ".src":

                        bool coop = false;
                        bool move = false;
                        //Get file text
                        string subpath = System.IO.Path.GetDirectoryName(path) + "\\" + System.IO.Path.GetFileNameWithoutExtension(path);
                        //Coop
                        if (File.Exists(subpath + "_master.src") && File.Exists(subpath + "_slave.src"))
                        {
                            using (StreamReader reader = new StreamReader(subpath + "_master.src"))
                                file = reader.ReadToEnd();
                            List<string> lines_master = new List<string>(file.Split('\r', '\n'));
                            using (StreamReader reader = new StreamReader(subpath + "_slave.src"))
                                file = reader.ReadToEnd();
                            List<string> lines_slave = new List<string>(file.Split('\r', '\n'));
                            int lineCounter = 0;
                            foreach (string line in lines_master)
                            {
                                if (line != "")
                                {
                                    //Get tool
                                    if (line.Length > 10 && line.Substring(0, 10) == "$ACT_TOOL=")
                                        toolIndex = Convert.ToInt16(line.Substring(10));
                                    //Get command
                                    if (!coop)
                                    {
                                        if (line.Length > 6 && line.Substring(0, 7) == "SYNCCMD")
                                        {
                                            task = Task.COOP;
                                            coop = true;
                                        }
                                        else if (line.Length > 3 && line.Substring(0, 3) == "PTP" && line.Substring(0, 9) != "PTP XHOME")
                                        {
                                            task = Task.PTP;
                                            move = true;
                                        }
                                        else if (line.Length > 3 && line.Substring(0, 3) == "LIN")
                                        {
                                            task = Task.LIN;
                                            move = true;
                                        }
                                    }
                                    else
                                    {
                                        coop = false;
                                        move = true;
                                    }
                                    if (task == Task.PTP || task == Task.LIN || task == Task.COOP)
                                    {
                                        if (move && !coop)
                                        {
                                            Position position = new Position();
                                            position.X = getPositionParameter(line, " {X ", ", Y ");
                                            position.Y = getPositionParameter(line, ", Y ", ", Z ");
                                            position.Z = getPositionParameter(line, ", Z ", ", A ");
                                            position.A = getPositionParameter(line, ", A ", ", B ");
                                            position.B = getPositionParameter(line, ", B ", ", C ");
                                            position.C = getPositionParameter(line, ", C ", "}");
                                            AddCommand(task, position);
                                            //Get second robot
                                            if (task != Task.COOP)
                                            {
                                                if (lines_slave[lineCounter].Length > 3 && lines_slave[lineCounter].Substring(0, 3) == "PTP" && lines_slave[lineCounter].Substring(0, 9) != "PTP XHOME")
                                                    task = Task.PTP;
                                                else if (lines_slave[lineCounter].Length > 3 && lines_slave[lineCounter].Substring(0, 3) == "LIN")
                                                    task = Task.LIN;
                                            }
                                            position = new Position();
                                            position.X = getPositionParameter(lines_slave[lineCounter], " {X ", ", Y ");
                                            position.Y = getPositionParameter(lines_slave[lineCounter], ", Y ", ", Z ");
                                            position.Z = getPositionParameter(lines_slave[lineCounter], ", Z ", ", A ");
                                            position.A = getPositionParameter(lines_slave[lineCounter], ", A ", ", B ");
                                            position.B = getPositionParameter(lines_slave[lineCounter], ", B ", ", C ");
                                            position.C = getPositionParameter(lines_slave[lineCounter], ", C ", "}");
                                            AddCommand(task, position);

                                            move = false;
                                        }
                                    }
                                }
                                lineCounter++;
                            }
                        }
                        //Single
                        else
                        {
                            //Get robot
                            using (StreamReader reader = new StreamReader(path))
                                file = reader.ReadToEnd();
                            lines = new List<string>(file.Split('\r', '\n'));
                            foreach (string line in lines)
                            {
                                if (line != "")
                                {
                                    //Get command
                                    move = false;
                                    if (line.Length > 3 && line.Substring(0, 3) == "PTP" && line.Substring(0, 9) != "PTP XHOME")
                                    {
                                        task = Task.PTP;
                                        move = true;
                                    }
                                    else if (line.Length > 3 && line.Substring(0, 3) == "LIN")
                                    {
                                        task = Task.LIN;
                                        move = true;
                                    }
                                    if (move)
                                    {
                                        Position position = new Position();
                                        position.X = getPositionParameter(line, " {X ", ", Y ");
                                        position.Y = getPositionParameter(line, ", Y ", ", Z ");
                                        position.Z = getPositionParameter(line, ", Z ", ", A ");
                                        position.A = getPositionParameter(line, ", A ", ", B ");
                                        position.B = getPositionParameter(line, ", B ", ", C ");
                                        position.C = getPositionParameter(line, ", C ", "}");
                                        AddCommand(task, position);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        status = false;
                        break;
                }
            }
            catch
            {
                status = false;
            }
            return status;
        }

        private double getPositionParameter(string line, string previous, string afterwards)
        {
            List<int> from = FindPositionsOfText(previous, line);
            List<int> to = FindPositionsOfText(afterwards, line);
            string text = line.Substring(from[0] + previous.Length, to[0] - (from[0] + previous.Length));
            return double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
        }

        private List<int> FindPositionsOfText(string textToFind, string textToSearch)
        {
            List<int> result = new List<int>();

            int lastIndex = 0;
            while (textToSearch.IndexOf(textToFind, lastIndex) > -1)
            {
                result.Add(textToSearch.IndexOf(textToFind, lastIndex));
                lastIndex = result[result.Count - 1] + textToFind.Length;
            }
            return result;
        }

        private Task GetTask(string text)
        {
            Task task = new Task();
            switch (text)
            {
                case "PTP":
                    task = Task.PTP;
                    break;
                case "LIN":
                    task = Task.LIN;
                    break;
                case "CIRC":
                    task = Task.CIRC;
                    break;
                case "AXS":
                    task = Task.AXS;
                    break;
                case "ITE":
                    task = Task.ITE;
                    break;
                case "COOP":
                    task = Task.COOP;
                    break;
                default:
                    task = Task.PTP;
                    break;
            }
            return task;
        }

        /*
        public void AddPath(Path path)
        {
            bool start = false;
            foreach (Position position in path.path)
            {
                if (start)
                    AddCommand(Task.LIN, position);
                else
                {
                    AddCommand(Task.PTP, position);
                    start = true;
                }
            }
        }
        */

        /*
        public void AddCoopPath(Path path, Position startPosWorkpiece, Position startPosTool, Position transformation)
        {
            bool start = false;
            Position targetPosWorkpiece = new Position();
            Position targetPosTool = new Position();
            Position lastPos = new Position();

            foreach (Position position in path.path)
            {
                if (start)
                {
                    Position deltaPos = position;
                    deltaPos.Subtract(lastPos);
                    deltaPos.Scale(0.5);
                    //targetPosWorkpiece.Subtract(deltaPos);

                    //Transformation
                    targetPosWorkpiece.Rotate(transformation.A, transformation.B, transformation.C);
                    targetPosWorkpiece.Translate(transformation.X, transformation.Y, transformation.Z);

                    Position b = new Position();
                    b = targetPosWorkpiece;
                    b.Scale(-1);
                    Position p = new Position();
                    p = position;
                    p.Subtract(targetPosWorkpiece);

                    targetPosWorkpiece.Translate(-deltaPos.X, -deltaPos.Y, -deltaPos.Z);
                    //targetPosWorkpiece.Translate(b.X, b.Y, b.Z);
                    //targetPosWorkpiece.Translate(-p.X, -p.Y, -p.Z);
                    //targetPosWorkpiece.Rotate(-deltaPos.A, -deltaPos.B, -deltaPos.C);
                    //targetPosWorkpiece.Translate(p.X, p.Y, p.Z);
                    //targetPosWorkpiece.Translate(-b.X, -b.Y, -b.Z);

                    //Reverse transformation
                    targetPosWorkpiece.Translate(-transformation.X, -transformation.Y, -transformation.Z);
                    targetPosWorkpiece.Rotate(-transformation.A, -transformation.B, -transformation.C);

                    AddCommand(1, Task.COOP, targetPosWorkpiece);
                    targetPosTool.Add(deltaPos);
                    AddCommand(2, Task.COOP, targetPosTool);
                }
                else
                {
                    AddCommand(1, Task.PTP, startPosWorkpiece);
                    AddCommand(2, Task.PTP, startPosTool);
                    targetPosWorkpiece = startPosWorkpiece;
                    targetPosTool = startPosTool;
                    start = true;
                }
                lastPos = position;
            }
        }*/

        public void AddCooperation(List<Geometry.Position> pathTool, List<Geometry.Position> pathWorkpiece)
        {
            int i = 1;
            AddCommand(Task.PTP, pathWorkpiece[0]);
            AddCommand(Task.PTP, pathTool[0]);
            foreach (Position positionTool in pathTool.Skip(1))
            {
                AddCommand(Task.COOP, pathWorkpiece[i]);
                AddCommand(Task.COOP, positionTool);
                i++;
            }
        }
    }

    public class Roboteam
    {
        public Robot Robot1, Robot2;
        public int ProgramIndex = 0;
        public enum Playmode { Gradual, Complete, Endless };
        public Playmode Mode = Playmode.Gradual;
        public bool Run = false;

        public Roboteam()
        {
            Robot1 = new Robot();
            Robot2 = new Robot();
        }

        public Roboteam(Robot robot1, Robot robot2)
        {
            Robot1 = robot1;
            Robot2 = robot2;
        }

        public void RunPrograms()
        {
            Run = true;
            switch (Mode)
            {
                case Playmode.Gradual:
                    RunStep();
                    break;
                case Playmode.Complete:
                    RunComplete();
                    break;
                case Playmode.Endless:
                    while (Run)
                        RunComplete();
                    break;
            }
        }

        private void RunComplete()
        {
            bool play1 = false;
            bool play2 = false;
            bool coop1 = false;
            bool coop2 = false;
            Thread ThreadExecuteProgram1 = new Thread(delegate()
            {
                play1 = true;
                for (int i = ProgramIndex; i < Robot1.Program.Commands.Count; i++)
                {
                    //Move
                    if (Run)
                    {
                        ProgramIndex = i;
                        if (Robot1.Program.Commands[i].Task != Routine.Task.COOP)
                        {
                            Robot1.Execute(Robot1.Program.Commands[i]);
                        }
                        else
                        {
                            coop1 = true;
                            while (Run && !coop2)
                            {
                                Thread.Sleep(200);
                            }
                            if (coop1 && coop2)
                                Robot1.Execute(Robot1.Program.Commands[i]);
                            i++;
                        }
                    }
                }
                //Reset
                if (Run)
                {
                    ProgramIndex = 0;
                    Thread.Sleep(1000);
                }
                play1 = false;
                if (!play1 && !play2)
                    Run = false;
            });
            Thread ThreadExecuteProgram2 = new Thread(delegate()
            {
                play2 = true;
                for (int i = ProgramIndex; i < Robot2.Program.Commands.Count; i++)
                {
                    //Move
                    if (Run)
                    {
                        ProgramIndex = i;
                        if (Robot2.Program.Commands[i].Task != Routine.Task.COOP)
                        {
                            Robot2.Execute(Robot2.Program.Commands[i]);
                        }
                        else
                        {
                            coop2 = true;
                            while (Run && !coop2)
                            {
                                Thread.Sleep(200);
                            }
                            if (coop1 && coop2)
                                Robot2.Execute(Robot2.Program.Commands[i]);
                            i++;
                        }
                    }
                }
                //Reset
                if (Run)
                {
                    ProgramIndex = 0;
                    Thread.Sleep(1000);
                }
                play2 = false;
                if (!play1 && !play2)
                    Run = false;
            });
        }

        private void RunStep()
        {
            Thread ThreadExecute = new Thread(delegate()
            {
                //Move
                Run = true;
                Robot1.Execute(Robot1.Program.Commands[ProgramIndex]);
                Robot2.Execute(Robot2.Program.Commands[ProgramIndex]);
                //Reset
                if (ProgramIndex < Robot1.Program.Commands.Count - 1)
                    ProgramIndex++;
                else
                    ProgramIndex = 0;
                Run = false;
            });
        }

        public void StopPrograms()
        {
            Run = false;
        }
    }

    //public class Cooperation
    //{
    //    public Geometry.Position StartingPositionTool = new Geometry.Position();
    //    public Geometry.Position StartingPositionWorkpiece = new Geometry.Position();
    //    public List<Geometry.Position> Path = new List<Geometry.Position>();
    //    private List<Geometry.Position> toolPath = new List<Geometry.Position>();
    //    private List<Geometry.Position> workpiecePath = new List<Geometry.Position>();

    //    public Cooperation(Geometry.Position startingPositionTool, Geometry.Position startingPositionWorkpiece, List<Geometry.Position> path)
    //    {
    //        StartingPositionTool = startingPositionTool;
    //        StartingPositionWorkpiece = startingPositionWorkpiece;
    //        Path = path;
    //    }

    //    public List<Geometry.Position> ToolPath()
    //    {
    //        GetPaths();
    //        return toolPath;
    //    }

    //    public List<Geometry.Position> WorkpiecePath()
    //    {
    //        GetPaths();
    //        return workpiecePath;
    //    }

    //    private void GetPaths()
    //    {
    //        toolPath = new List<Geometry.Position>();
    //        Geometry.Position iniPos = new Geometry.Position();
    //        iniPos = Path[0];
    //        iniPos.Rotate(StartingPositionTool.A, StartingPositionTool.B, StartingPositionTool.C);
    //        iniPos.Translate(StartingPositionTool.X, StartingPositionTool.Y, StartingPositionTool.Z);
    //        Geometry.Point iniVector = new Geometry.Point(0, 0, -20);
    //        iniVector.Rotate(StartingPositionTool.A, StartingPositionTool.B, StartingPositionTool.C);
    //        iniPos = StartingPositionTool;
    //        iniPos.Translate(iniVector.X, iniVector.Y, iniVector.Z);
    //        toolPath.Add(iniPos);
    //        workpiecePath = new List<Geometry.Position>();
    //        iniPos = new Geometry.Position();
    //        iniVector = new Geometry.Point(0, 0, -20);
    //        iniVector.Rotate(StartingPositionWorkpiece.A, StartingPositionWorkpiece.B, StartingPositionWorkpiece.C);
    //        iniPos = StartingPositionWorkpiece;
    //        iniPos.Translate(iniVector.X, iniVector.Y, iniVector.Z);
    //        workpiecePath.Add(iniPos);
    //        Geometry.Point pt = new Geometry.Point(StartingPositionTool.X, StartingPositionTool.Y, StartingPositionTool.Z);
    //        Geometry.Point rt = new Geometry.Point(StartingPositionTool.A, StartingPositionTool.B, StartingPositionTool.C);
    //        Geometry.Point pw = new Geometry.Point(StartingPositionWorkpiece.X, StartingPositionWorkpiece.Y, StartingPositionWorkpiece.Z);
    //        Geometry.Point rw = new Geometry.Point(StartingPositionWorkpiece.A, StartingPositionWorkpiece.B, StartingPositionWorkpiece.C);
    //        Geometry.Position previousPosition = new Geometry.Position(Path[1].X, Path[1].Y, Path[1].Z, Path[1].A, Path[1].B, Path[1].C);

    //        Geometry g = new Geometry();

    //        /*
    //        for (int i = 1; i < Path.Count() - 1; i++)
    //        {
    //            Geometry.RotationMatrix Rw = new Geometry.RotationMatrix(rw.X, rw.Y, rw.Z);
    //            Geometry.Point Dr = new Geometry.Point(0.5 * (Path[i].A - Path[i - 1].A), 0.5 * (Path[i].B - Path[i - 1].B), 0.5 * (Path[i].C - Path[i - 1].C));
    //            if (Dr.X > 90)
    //                Dr.X = 180 - Dr.X;
    //            if (Dr.Y > 90)
    //                Dr.Y = 180 - Dr.Y;
    //            if (Dr.Z > 90)
    //                Dr.Z = 180 - Dr.Z;
    //            Geometry.Point Dt = new Geometry.Point(0.5 * (Path[i].X - Path[i - 1].X), 0.5 * (Path[i].Y - Path[i - 1].Y), 0.5 * (Path[i].Z - Path[i - 1].Z));
    //            Geometry.Point dt = g.Multiply(Rw, Dt);
    //            //Geometry.Point dt = Dt;
    //            Geometry.Point s = new Geometry.Point();
    //            Geometry.Point Dp = new Geometry.Point((Path[i].X - Path[i - 1].X), (Path[i].Y - Path[i - 1].Y), (Path[i].Z - Path[i - 1].Z));
    //            Geometry.RotationMatrix RDr = new Geometry.RotationMatrix(-Dr.X, -Dr.Y, -Dr.Z);
    //            s = g.Multiply(RDr, g.Sum(g.Multiply(Rw, Dp), g.Subtract(pt, pw)));
    //            //s = g.Multiply(RDr, g.Sum(Dp, g.Subtract(pt, pw)));

    //            pt = g.Sum(pt, dt);
    //            rt = new Geometry.Point(rt.X + Dr.X, rt.Y + Dr.Y, rt.Z + Dr.Z);
    //            toolPath.Add(new Geometry.Position(pt.X, pt.Y, pt.Z, rt.X, rt.Y, rt.Z));

    //            pw = g.Subtract(pt, s);
    //            rw = new Geometry.Point(rw.X - Dr.X, rw.Y - Dr.Y, rw.Z - Dr.Z);
    //            workpiecePath.Add(new Geometry.Position(pw.X, pw.Y, pw.Z, rw.X, rw.Y, rw.Z));
    //        }*/
    //        foreach (Geometry.Position position in Path.Skip(1))
    //        {
    //            Geometry.RotationMatrix Rw = new Geometry.RotationMatrix(rw.X, rw.Y, rw.Z);
    //            Geometry.Point Dr = new Geometry.Point(0.5 * (position.A - previousPosition.A), 0.5 * (position.B - previousPosition.B), 0.5 * (position.C - previousPosition.C));
    //            if (Dr.X > 90)
    //                Dr.X = 180 - Dr.X;
    //            if (Dr.Y > 90)
    //                Dr.Y = 180 - Dr.Y;
    //            if (Dr.Z > 90)
    //                Dr.Z = 180 - Dr.Z;
    //            Geometry.Point Dt = new Geometry.Point(0.5 * (position.X - previousPosition.X), 0.5 * (position.Y - previousPosition.Y), 0.5 * (position.Z - previousPosition.Z));
    //            Geometry.Point dt = g.Multiply(Rw, Dt);
    //            //Geometry.Point dt = Dt;
    //            Geometry.Point s = new Geometry.Point();
    //            Geometry.Point Dp = new Geometry.Point((position.X - previousPosition.X), (position.Y - previousPosition.Y), (position.Z - previousPosition.Z));
    //            Geometry.RotationMatrix RDr = new Geometry.RotationMatrix(-Dr.X, -Dr.Y, -Dr.Z);
    //            s = g.Multiply(RDr, g.Sum(g.Multiply(Rw, Dp), g.Subtract(pt, pw)));
    //            //s = g.Multiply(RDr, g.Sum(Dp, g.Subtract(pt, pw)));

    //            pt = g.Sum(pt, dt);
    //            rt = new Geometry.Point(rt.X + Dr.X, rt.Y + Dr.Y, rt.Z + Dr.Z);
    //            toolPath.Add(new Geometry.Position(pt.X, pt.Y, pt.Z, rt.X, rt.Y, rt.Z));

    //            pw = g.Subtract(pt, s);
    //            rw = new Geometry.Point(rw.X - Dr.X, rw.Y - Dr.Y, rw.Z - Dr.Z);
    //            workpiecePath.Add(new Geometry.Position(pw.X, pw.Y, pw.Z, rw.X, rw.Y, rw.Z));

    //            previousPosition = new Geometry.Position(position.X, position.Y, position.Z, position.A, position.B, position.C);
    //        }
    //    }

    //}

    /*
    public class Sketch : Geometry
    {
        public List<Element> Linkage = new List<Element>();

        public Sketch()
        {
            Linkage = new List<Element>();
        }

        public Sketch(Sketch sketch)
        {
            Linkage.Clear();
            foreach (Element element in sketch.Linkage)
            {
                Linkage.Add(element);
            }
        }

        public enum DrawType { Point, Line, Circle, Interpolation };

        public struct Element
        {
            public DrawType Type;
            public Point Point;

            public Element(DrawType type, Point point)
            {
                Type = type;
                Point = point;
            }
        }

        public struct Arc
        {
            public Point Center;
            public double Radius, StartAngle, Angle;

            public Arc(Point center, double radius, double start, double angle)
            {
                Center = center;
                Radius = radius;
                StartAngle = start;
                Angle = angle;
            }

            public Arc(Point p1, Point p2, Point p3)
            {
                Center.Y = (((p3.X * p3.X) - (p1.X * p1.X) + (p3.Y * p3.Y) - (p1.Y * p1.Y)) * (p2.X - p1.X) - ((p2.X * p2.X) - (p1.X * p1.X) + (p2.Y * p2.Y) - (p1.Y * p1.Y)) * (p3.X - p1.X)) / (2 * ((p3.Y - p1.Y) * (p2.X - p1.X) - (p2.Y - p1.Y) * (p3.X - p1.X)));
                Center.X = (((p2.X * p2.X) - (p1.X * p1.X)) + ((p2.Y * p2.Y) - (p1.Y * p1.Y)) - 2 * Center.Y * (p2.Y - p1.Y)) / (2 * (p2.X - p1.X));
                Center.Z = 0;
                Radius = Math.Sqrt(Math.Pow((Center.X - p1.X), 2) + Math.Pow((Center.Y - p1.Y), 2));

                double alpha = 180 + Math.Acos((Center.X - p1.X) / Radius) * 180 / Math.PI;
                double beta = 180 + Math.Acos((Center.X - p2.X) / Radius) * 180 / Math.PI;
                double gamma = 180 + Math.Acos((Center.X - p3.X) / Radius) * 180 / Math.PI;

                if (p1.Y < Center.Y)
                    alpha = 360 - alpha;
                if (p2.Y < Center.Y)
                    beta = 360 - beta;
                if (p3.Y < Center.Y)
                    gamma = 360 - gamma;

                if (alpha > gamma && (beta > alpha || beta < gamma))
                    gamma = gamma + 360 - alpha;
                else if (alpha < gamma && (beta < alpha || beta > gamma))
                    gamma = -(360 - (gamma - alpha));
                else
                    gamma = gamma - alpha;

                StartAngle = alpha;
                Angle = gamma;
            }
        }

        public void AddPoint(int x, int y)
        {
            Linkage.Add(new Element(DrawType.Point, new Point(x, y, 0)));
        }

        public void AddLine(int x, int y)
        {
            Linkage.Add(new Element(DrawType.Line, new Point(x, y, 0)));
        }

        public void AddCircle(int x, int y)
        {
            Linkage.Add(new Element(DrawType.Circle, new Point(x, y, 0)));
        }

        public void AddInterpolation(int x, int y)
        {
            Linkage.Add(new Element(DrawType.Interpolation, new Point(x, y, 0)));
        }

        public void Remove(Element element)
        {
            if (Linkage.Count > 0)
            {
                //Only one element
                if (Linkage.Count <= 1)
                    Linkage.Clear();
                //More elements
                else
                {
                    //Get element index
                    int ind = Linkage.FindLastIndex(e => e.Equals(element));
                    if (ind != -1)
                    {
                        //Change next element to point
                        if (Linkage.Count > ind + 1)
                        {
                            Sketch.Element next = Linkage.ElementAt(ind + 1);
                            if (next.Type != Sketch.DrawType.Point)
                            {
                                next.Type = Sketch.DrawType.Point;
                                Linkage.RemoveAt(ind + 1);
                                Linkage.Insert(ind + 1, next);
                            }
                        }
                        //Remove element
                        Linkage.RemoveAt(ind);
                    }
                }
            }
        }

        public Element Select(int x, int y)
        {
            Element element = new Element();
            int range = 5;
            if (Linkage.Count > 0)
            {
                //Get points around selected position
                List<Element> selection = new List<Element>();
                for (int i = -range; i <= range; i++)
                {
                    for (int j = -range; j <= range; j++)
                    {
                        selection.Add(new Element(DrawType.Point, new Point(x + i, y + j, 0)));
                    }
                }
                //Get selected element
                Point point = Linkage.Select(e => e.Point).Intersect(selection.Select(e => e.Point)).First();
                element = Linkage.LastOrDefault(e => e.Point.Equals(point));
            }
            return element;
        }

        public void Change(Element oldElement, Element newElement)
        {
            if (Linkage.Count > 0)
            {
                //Get selected element
                int ind = Linkage.FindLastIndex(e => e.Equals(oldElement));
                if (ind != -1)
                {
                    //Remove old element
                    Linkage.RemoveAt(ind);
                    //Add new element
                    Linkage.Insert(ind, newElement);
                }
            }
        }

        public void InsertLine(int x, int y, int px, int py)
        {
            int index = Linkage.IndexOf(new Element(DrawType.Line, new Point(px, py, 0)));
            Linkage.Insert(index, new Element(DrawType.Line, new Point(x, y, 0)));
        }

        public PointCloud.Space GetSpace()
        {
            PointCloud.Space space = new PointCloud.Space();
            if (Linkage.Count > 0)
            {
                space.Min.X = (int)Linkage.Min(tile => tile.Point.X);
                space.Min.Y = (int)Linkage.Min(tile => tile.Point.Y);
                space.Min.Z = (int)Linkage.Min(tile => tile.Point.Z);
                space.Max.X = (int)Linkage.Max(tile => tile.Point.X);
                space.Max.Y = (int)Linkage.Max(tile => tile.Point.Y);
                space.Max.Z = (int)Linkage.Max(tile => tile.Point.Z);
            }
            return space;
        }

        public void Interpolate(double stepsize)
        {
            Element lastElement = new Element();
            Element lastLastElement = new Element();
            Element stepElement = new Element();
            List<Element> newElements = new List<Element>();
            Point newPoint = new Point();
            int steps;
            bool iniCirc = false;
            foreach (Element element in Linkage)
            {
                switch (element.Type)
                {
                    case DrawType.Point:
                        newElements.Add(element);
                        break;
                    case DrawType.Line:
                    case DrawType.Interpolation:
                        Vector distance = new Vector(lastElement.Point, element.Point);
                        steps = (int)(distance.Lenght / stepsize);
                        Vector step = new Vector(0, 0, 0, distance.X / steps, distance.Y / steps, distance.Z / steps);
                        newPoint = lastElement.Point;
                        for (int n = 0; n < steps - 1; n++)
                        {
                            newPoint.Translate(new Point(step.X, step.Y, step.Z));
                            stepElement = new Element(DrawType.Line, newPoint);
                            newElements.Add(stepElement);
                        }
                        newElements.Add(element);
                        break;
                    case DrawType.Circle:
                        if (iniCirc)
                        {
                            Arc arc = new Arc(lastLastElement.Point, lastElement.Point, element.Point);
                            double d = Math.PI * arc.Radius * arc.Angle / 180;
                            steps = (int)Math.Abs(d / stepsize);
                            double a = arc.Angle / steps;
                            newPoint = lastLastElement.Point;
                            for (int n = 0; n < steps - 1; n++)
                            {
                                newPoint.Translate(-arc.Center.X, -arc.Center.Y, -arc.Center.Z);
                                newPoint.Rotate(-a, 0, 0);
                                newPoint.Translate(arc.Center.X, arc.Center.Y, arc.Center.Z);
                                stepElement = new Element(DrawType.Line, newPoint);
                                newElements.Add(stepElement);
                            }
                            stepElement = new Element(DrawType.Line, element.Point);
                            newElements.Add(stepElement);
                            iniCirc = false;
                        }
                        else
                        {
                            lastLastElement = lastElement;
                            iniCirc = true;
                        }
                        break;
                }
                lastElement = element;
            }
            Linkage = newElements;
        }

        public Path Projection(List<Triangle> mesh, Position vector)
        {
            Path path = new Path();
            Position lastPos = new Position();
            Position newPosition = new Position();
            bool ini = false;
            int offset = 50;
            if (Linkage.Count > 0 && mesh.Count > 0)
            {                  
                Position position = new Position();
                foreach (Element element in Linkage)
                {
                    //Search intersection points
                    List<Position> positions = new List<Position>();
                    foreach (Triangle triangle in mesh)
                    {
                        Vector p = new Vector(0, 0, 0, element.Point.X, element.Point.Y, element.Point.Z);
                        p.Rotate(vector.A, vector.B, vector.C);
                        p = new Vector(0, 0, 0, p.X + vector.X, p.Y + vector.Y, p.Z + vector.Z);
                        Vector u = new Vector(0, 0, 0, triangle.Point2.X - triangle.Point1.X, triangle.Point2.Y - triangle.Point1.Y, triangle.Point2.Z - triangle.Point1.Z);
                        Vector v = new Vector(0, 0, 0, triangle.Point3.X - triangle.Point1.X, triangle.Point3.Y - triangle.Point1.Y, triangle.Point3.Z - triangle.Point1.Z);
                        Vector w = new Vector(0, 0, 0, p.X - triangle.Point1.X, p.Y - triangle.Point1.Y, p.Z - triangle.Point1.Z);
                        Vector d = new Vector(0, 0, 0, 0, 0, 1);
                        d.Rotate(vector.A, vector.B, vector.C);
                        Vector c_wu = CrossProduct(w, u);
                        Vector c_dv = CrossProduct(d, v);
                        double c = VectorProduct(c_dv, u);
                        double t = VectorProduct(c_wu, v) / c;
                        double r = VectorProduct(c_dv, w) / c;
                        double s = VectorProduct(c_wu, d) / c;
                        if (!(r < 0 | r > 1 | s < 0 | s > 1 | r + s < 0 | r + s > 1))
                        {
                            Vector normal = Normal(triangle);
                            position = new Position(t * d.X + p.X, t * d.Y + p.Y, t * d.Z + p.Z, normal.X, normal.Y, normal.Z);
                            positions.Add(position);
                        }
                    }
                    //Get the closest intersection point
                    if (positions.Count > 1)
                    {
                        double distance = 9999999;
                        foreach (Position p in positions)
                        {
                            double d = Math.Sqrt(Math.Pow(p.X - element.Point.X, 2) + Math.Pow(p.Y - element.Point.Y, 2) + Math.Pow(p.Z - element.Point.Z, 2));
                            if (d < distance)
                            {
                                position = p;
                                distance = d;
                            }
                        }
                    }
                    //Save intersection point
                    if (positions.Count > 0)
                    {
                        //Get orientation
                        if (ini)
                        {
                            //Normal direction
                            newPosition = GetNormalOrientation(new Vector(0, 0, 0, -lastPos.A, -lastPos.B, -lastPos.C), new Vector(0, 0, 0, 0, 0, 1));
                            lastPos.A = newPosition.A;
                            lastPos.B = newPosition.B;
                            lastPos.C = newPosition.C;
                            //Move direction
                            if (element.Type != DrawType.Point)
                            {
                                newPosition = GetMovementOrientation(new Vector(0, 0, 0, position.X - lastPos.X, position.Y - lastPos.Y, position.Z - lastPos.Z), new Vector(0, 0, 0, 1, 0, 0), new Vector(0, 0, 0, 0, 0, 1), lastPos);
                                lastPos.A = newPosition.A;
                                lastPos.B = newPosition.B;
                                lastPos.C = newPosition.C;
                            }
                            else
                            {
                                Position lastLastPosition = path.path[path.path.Count - 1];
                                Vector move = new Vector(0, 0, 0, 1, 0, 0);
                                move.Rotate(lastLastPosition.A, lastLastPosition.B, lastLastPosition.C);
                                newPosition = GetMovementOrientation(move, new Vector(0, 0, 0, 1, 0, 0), new Vector(0, 0, 0, 0, 0, 1), lastPos);
                                //Get angles between -180 and 180
                                lastPos.A = GetAngleInRange(newPosition.A);
                                lastPos.B = GetAngleInRange(newPosition.B);
                                lastPos.C = GetAngleInRange(newPosition.C);
                                path.AddPosition(lastPos);
                                //Get to next segment
                                Position savePos = new Position(lastPos);
                                lastPos.TranslateAxial(0, 0, -offset);
                                path.AddPosition(lastPos);
                                lastPos.Translate(position.X - savePos.X, position.Y - savePos.Y, position.Z - savePos.Z);
                            }
                            path.AddPosition(lastPos);
                        }
                        else
                            ini = true;
                        lastPos = position;
                    }
                }
                //Add start position
                Position iniPos = new Position(path.path[0]);
                iniPos.TranslateAxial(0, 0, -offset);
                path.InsertPosition(0, iniPos);
                //Add last position
                Vector m = new Vector(0, 0, 0, 1, 0, 0);
                m.Rotate(newPosition.A, newPosition.B, newPosition.C);
                newPosition = GetNormalOrientation(new Vector(0, 0, 0, -position.A, -position.B, -position.C), new Vector(0, 0, 0, 0, 0, 1));
                newPosition = GetMovementOrientation(m, new Vector(0, 0, 0, 1, 0, 0), new Vector(0, 0, 0, 0, 0, 1), newPosition);
                path.AddPosition(new Position(position.X, position.Y, position.Z, newPosition.A, newPosition.B, newPosition.C));
                //Add end position
                iniPos = new Position(path.path[path.path.Count - 1]);
                iniPos.TranslateAxial(0, 0, -offset);
                path.AddPosition(iniPos);
            }
            return path;
        }

        private double GetAngleInRange(double angle)
        {
            if (angle > 180)
                angle = angle - 360;
            else if (angle < -180)
                angle = angle + 360;
            return angle;
        }

        private Position GetNormalOrientation(Vector normal, Vector orientation)
        {
            Position newPosition = new Position();
            double A = 0, B = 0, C = 0;
            Vector a = UnitVector(orientation);
            Vector b = UnitVector(normal);
            Vector v = CrossProduct(a, b);
            double s = v.Lenght;
            double c = VectorProduct(a, b);
            Matrix I = new Matrix(1, 0, 0, 0, 1, 0, 0, 0, 1);
            Matrix V = new Matrix(0, -v.Z, v.Y, v.Z, 0, -v.X, -v.Y, v.X, 0);
            Matrix V2 = Multiply(V, V);
            double k = (1 - c) / Math.Pow(s, 2);
            V2 = Multiply(k, V2);
            Matrix R = new Matrix();
            R = Sum(I, V);
            R = Sum(R, V2);
            if (Math.Abs(R.C1R3) != 1)
            {
                B = Math.PI + Math.Sin(R.C1R3);
                C = Math.Atan2(R.C2R3 / Math.Cos(B), R.C3R3 / Math.Cos(B));
                A = Math.Atan2(R.C1R2 / Math.Cos(B), R.C1R1 / Math.Cos(B));
            }
            else
            {
                A = 0;
                if (R.C1R3 == -1)
                {
                    B = Math.PI / 2;
                    C = A + Math.Atan2(R.C2R1, R.C3R1);
                }
                else
                {
                    B = -Math.PI / 2;
                    C = -A + Math.Atan2(-R.C2R1, -R.C3R1);
                }
            }
            if (A > Math.PI)
                A = A - 2 * Math.PI;
            else if (A < -Math.PI)
                A = A + 2 * Math.PI;
            if (B > Math.PI)
                B = B - 2 * Math.PI;
            else if (B < -Math.PI)
                B = B + 2 * Math.PI;
            if (C > Math.PI)
                C = C - 2 * Math.PI;
            else if (C < -Math.PI)
                C = C + 2 * Math.PI;
            newPosition.A = A * 180 / Math.PI;
            newPosition.B = B * 180 / Math.PI;
            newPosition.C = C * 180 / Math.PI;
            return newPosition;
        }

        private Position GetMovementOrientation(Vector movement, Vector orientation, Vector axis, Position rotation)
        {
            Position newPosition = new Position();
            double A = 0, B = 0, C = 0;
            //Get rotation axis
            RotationMatrix R1 = new RotationMatrix(rotation.A, rotation.B, rotation.C);
            Vector r1 = UnitVector(axis);
            r1.Rotate(rotation.A, rotation.B, rotation.C);
            //Get missing rotation angle
            Vector x = UnitVector(orientation);
            x.Rotate(rotation.A, rotation.B, rotation.C);
            Vector m = UnitVector(movement);
            double angle = Math.Acos(VectorProduct(m, x) / (m.Lenght * x.Lenght));
            int sign = Math.Sign(VectorProduct(CrossProduct(x, m), r1));
            angle = sign * angle;
            //Get second room rotation
            RotationMatrix R2 = new RotationMatrix();
            R2.C1R1 = Math.Cos(angle) + Math.Pow(r1.X, 2) * (1 - Math.Cos(angle));
            R2.C1R2 = r1.Y * r1.X * (1 - Math.Cos(angle)) + r1.Z * Math.Sin(angle);
            R2.C1R3 = r1.Z * r1.X * (1 - Math.Cos(angle)) - r1.Y * Math.Sin(angle);
            R2.C2R1 = r1.X * r1.Y * (1 - Math.Cos(angle)) - r1.Z * Math.Sin(angle);
            R2.C2R2 = Math.Cos(angle) + Math.Pow(r1.Y, 2) * (1 - Math.Cos(angle));
            R2.C2R3 = r1.Z * r1.Y * (1 - Math.Cos(angle)) + r1.X * Math.Sin(angle);
            R2.C3R1 = r1.X * r1.Z * (1 - Math.Cos(angle)) + r1.Y * Math.Sin(angle);
            R2.C3R2 = r1.Y * r1.Z * (1 - Math.Cos(angle)) - r1.X * Math.Sin(angle);
            R2.C3R3 = Math.Cos(angle) + Math.Pow(r1.Z, 2) * (1 - Math.Cos(angle));
            RotationMatrix RR = Multiply(R2, R1);
            A = 0;
            B = 0;
            C = 0;
            if (Math.Abs(RR.C1R3) != 1)
            {
                B = Math.PI + Math.Sin(RR.C1R3);
                C = Math.Atan2(RR.C2R3 / Math.Cos(B), RR.C3R3 / Math.Cos(B));
                A = Math.Atan2(RR.C1R2 / Math.Cos(B), RR.C1R1 / Math.Cos(B));
            }
            else
            {
                A = 0;
                if (RR.C1R3 == -1)
                {
                    B = Math.PI / 2;
                    C = A + Math.Atan2(RR.C2R1, RR.C3R1);
                }
                else
                {
                    B = -Math.PI / 2;
                    C = -A + Math.Atan2(-RR.C2R1, -RR.C3R1);
                }
            }
            if (A > Math.PI)
                A = A - 2 * Math.PI;
            else if (A < -Math.PI)
                A = A + 2 * Math.PI;
            if (B > Math.PI)
                B = B - 2 * Math.PI;
            else if (B < -Math.PI)
                B = B + 2 * Math.PI;
            if (C > Math.PI)
                C = C - 2 * Math.PI;
            else if (C < -Math.PI)
                C = C + 2 * Math.PI;
            newPosition.A = A * 180 / Math.PI;
            newPosition.B = B * 180 / Math.PI;
            newPosition.C = C * 180 / Math.PI;
            return newPosition;
        }

    }
    */

    public class Scanner : Geometry
    {
        public enum ConnectionStatus { Disconnected, Connecting, Connected, Error };
        public ConnectionStatus Status = ConnectionStatus.Disconnected;
        public List<ColorPoint> Profile = new List<ColorPoint>();
        public Point Minimum = new Point();
        public Point Maximum = new Point();
        Thread ThreadTransmitScanner;

        //bool colorExtension = false;
        //int colorExtensionCameraChannel = 0;
        //float[] colorExtensionNormal = new float[2];
        //public Camera camera = new Camera();

        private const int MAX_INTERFACE_COUNT = 5;
        private const int MAX_RESOULUTIONS = 6;
        static public uint m_uiResolution = 0;
        static public uint m_hLLT = 0;
        static public CLLTI.TScannerType m_tscanCONTROLType;
        private uint ShutterTime = 200;
        private uint IdleTime = 800;
        static uint Frequence = 1000;

        public void Connect()
        {
            Thread ThreadConnect = new Thread(delegate()
            {
                Status = ConnectionStatus.Connecting;

                uint[] auiFirewireInterfaces = new uint[MAX_INTERFACE_COUNT];
                uint[] auiResolutions = new uint[MAX_RESOULUTIONS];
                int iFirewireInterfaceCount = 0;

                //Create a Firewire Device -> returns handle to LLT device
                m_hLLT = CLLTI.CreateLLTDevice(CLLTI.TInterfaceType.INTF_TYPE_ETHERNET);

                //Gets the available interfaces from the scanCONTROL-device
                iFirewireInterfaceCount = CLLTI.GetDeviceInterfaces(m_hLLT, auiFirewireInterfaces, auiFirewireInterfaces.GetLength(0));

                if (iFirewireInterfaceCount == 0)
                {
                    Disconnect();
                }
                if (iFirewireInterfaceCount >= 1)
                {
                    if ((CLLTI.SetDeviceInterface(m_hLLT, auiFirewireInterfaces[0], 0)) < CLLTI.GENERAL_FUNCTION_OK)
                    {
                        Status = ConnectionStatus.Error;
                    }
                    if (Status != ConnectionStatus.Error && (CLLTI.Connect(m_hLLT)) < CLLTI.GENERAL_FUNCTION_OK)
                    {
                        Status = ConnectionStatus.Error;
                    }
                    if (Status != ConnectionStatus.Error)
                    {
                        if ((CLLTI.GetLLTType(m_hLLT, ref m_tscanCONTROLType)) < CLLTI.GENERAL_FUNCTION_OK)
                        {
                            Status = ConnectionStatus.Error;
                        }

                        if ((CLLTI.GetResolutions(m_hLLT, auiResolutions, auiResolutions.GetLength(0))) < CLLTI.GENERAL_FUNCTION_OK)
                        {
                            Status = ConnectionStatus.Error;
                        }
                        m_uiResolution = auiResolutions[0];
                    }
                    if (Status != ConnectionStatus.Error)
                    {
                        if ((CLLTI.SetResolution(m_hLLT, m_uiResolution)) < CLLTI.GENERAL_FUNCTION_OK)
                        {
                            Status = ConnectionStatus.Error;
                        }
                    }
                    if (Status != ConnectionStatus.Error)
                    {
                        if ((CLLTI.SetFeature(m_hLLT, CLLTI.FEATURE_FUNCTION_TRIGGER, 0x00000000)) < CLLTI.GENERAL_FUNCTION_OK)
                        {
                            Status = ConnectionStatus.Error;
                        }
                    }
                    if (Status != ConnectionStatus.Error)
                    {
                        if ((CLLTI.SetProfileConfig(m_hLLT, CLLTI.TProfileConfig.PROFILE)) < CLLTI.GENERAL_FUNCTION_OK)
                        {
                            Status = ConnectionStatus.Error;
                        }
                    }
                    if (Status != ConnectionStatus.Error)
                    {
                        if ((CLLTI.SetFeature(m_hLLT, CLLTI.FEATURE_FUNCTION_SHUTTERTIME, ShutterTime)) < CLLTI.GENERAL_FUNCTION_OK)
                        {
                            Status = ConnectionStatus.Error;
                        }
                    }
                    if (Status != ConnectionStatus.Error)
                    {
                        if ((CLLTI.SetFeature(m_hLLT, CLLTI.FEATURE_FUNCTION_IDLETIME, IdleTime)) < CLLTI.GENERAL_FUNCTION_OK)
                        {
                            Status = ConnectionStatus.Error;
                        }
                    }
                }

                //Start data transmission
                if (Status == ConnectionStatus.Connecting)
                {
                    ThreadTransmitScanner = new Thread(Transmit);
                    ThreadTransmitScanner.Start();
                    Status = ConnectionStatus.Connected;
                }
                else
                {
                    Thread.Sleep(1000);
                    Disconnect();
                }
            });
            ThreadConnect.Start();
        }

        public void Disconnect()
        {
            if ((CLLTI.Disconnect(m_hLLT)) < CLLTI.GENERAL_FUNCTION_OK)
            {
                Status = ConnectionStatus.Error;
                Thread.Sleep(500);
            }

            //Clear profile
            Profile.Clear();
            //Abort the threads
            if (ThreadTransmitScanner != null && ThreadTransmitScanner.IsAlive)
            {
                ThreadTransmitScanner.Abort();
            }
            //Set connection status
            Status = ConnectionStatus.Disconnected;
        }

        public void Transmit()
        {
            while (Status != ConnectionStatus.Error)
            {
                uint uiLostProfiles = 0;
                double[] ValueX = new double[m_uiResolution];
                double[] ValueZ = new double[m_uiResolution];

                //Resize the profile buffer to the maximal profile size
                byte[] abyProfileBuffer = new byte[m_uiResolution * 4 + 16];

                if ((CLLTI.TransferProfiles(m_hLLT, CLLTI.TTransferProfileType.NORMAL_TRANSFER, 1)) < CLLTI.GENERAL_FUNCTION_OK)
                {
                    Status = ConnectionStatus.Error;
                    break;
                }

                //Sleep for a while to warm up the transfer
                Thread.Sleep(100);

                //Gets 1 profile in "polling-mode" and PURE_PROFILE configuration
                if ((CLLTI.GetActualProfile(m_hLLT, abyProfileBuffer, abyProfileBuffer.GetLength(0), CLLTI.TProfileConfig.PURE_PROFILE, ref uiLostProfiles)) != abyProfileBuffer.GetLength(0))
                {
                    Status = ConnectionStatus.Error;
                    break;
                }

                //Converting of profile data from the first reflection
                int iRetValue = CLLTI.ConvertProfile2Values(m_hLLT, abyProfileBuffer, m_uiResolution, CLLTI.TProfileConfig.PURE_PROFILE, m_tscanCONTROLType, 0, 1, null, null, null, ValueX, ValueZ, null, null);
                if (((iRetValue & CLLTI.CONVERT_X) == 0) || ((iRetValue & CLLTI.CONVERT_Z) == 0))
                {
                    Status = ConnectionStatus.Error;
                    break;
                }

                if ((CLLTI.TransferProfiles(m_hLLT, CLLTI.TTransferProfileType.NORMAL_TRANSFER, 0)) < CLLTI.GENERAL_FUNCTION_OK)
                {
                    Status = ConnectionStatus.Error;
                    break;
                }

                //Save new profile
                List<ColorPoint> tempProfile = new List<ColorPoint>();
                for (int i = 0; i < ValueX.Count(); i++)
                {
                    if (ValueZ[i] > 100)
                    {
                        Point point = new Point((float)ValueX[i], 0, (float)ValueZ[i]);
                        Color color = Color.Orange;
                        //if (colorExtension)
                        //    GetPointColor(point, camera, colorExtensionNormal, out color);
                        tempProfile.Add(new ColorPoint(point, color));
                    }
                }
                Profile = tempProfile;

                //Save minimum and maximum
                if (Profile.Count > 0)
                {
                    double minValue = Profile.Min(x => x.Point.Z);
                    Minimum = Profile.Find(x => x.Point.Z == minValue).Point;
                    double maxValue = Profile.Max(x => x.Point.Z);
                    Maximum = Profile.Find(x => x.Point.Z == maxValue).Point;
                }

                if (Status != ConnectionStatus.Error)
                    Status = ConnectionStatus.Connected;
            }
            Disconnect();
        }

        public void SetShutterTime(uint time)
        {
            ShutterTime = time;
            IdleTime = Frequence - ShutterTime;
            if (Status == ConnectionStatus.Connected)
            {
                if ((CLLTI.SetFeature(m_hLLT, CLLTI.FEATURE_FUNCTION_SHUTTERTIME, ShutterTime)) < CLLTI.GENERAL_FUNCTION_OK)
                {
                    Status = ConnectionStatus.Error;
                }
                if ((CLLTI.SetFeature(m_hLLT, CLLTI.FEATURE_FUNCTION_IDLETIME, IdleTime)) < CLLTI.GENERAL_FUNCTION_OK)
                {
                    Status = ConnectionStatus.Error;
                }
                Thread.Sleep(50);
            }
        }

        public void SaveProfile(string path)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                foreach (Geometry.ColorPoint point in Profile)
                    file.WriteLine(point.Point.X.ToString("0.000") + " " + point.Point.Y.ToString("0.000") + " " + point.Point.Z.ToString("0.000"));
            }
        }

        /*
        public bool GetColorProfile(List<Point> profile, Image<Bgr, Byte> image, IntrinsicCameraParameters IC, ExtrinsicCameraParameters EX, float[] colorExtensionNormal, out List<ColorPoint> colorProfile)
        {
            Matrix<float> objectPoints = new Matrix<float>(3, 1);
            Matrix<float> imagePoints = new Matrix<float>(2, 1);
            colorProfile = new List<ColorPoint>();
            int r = 50, g = 200, b = 150;
            int offset = 8;

            try
            {
                for (int i = 0; i < profile.Count(); i++)
                {
                    //Get pixel
                    objectPoints[0, 0] = (float)profile[i].X;
                    objectPoints[1, 0] = (float)profile[i].Y;
                    objectPoints[2, 0] = (float)profile[i].Z;

                    CvInvoke.cvProjectPoints2(objectPoints.Ptr, EX.RotationVector.Ptr, EX.TranslationVector.Ptr, IC.IntrinsicMatrix.Ptr, IC.DistortionCoeffs.Ptr, imagePoints.Ptr, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0);
                    int pixelX = (int)imagePoints.Data[0, 0];
                    int pixelY = (int)imagePoints.Data[1, 0];

                    //Get color    
                    if (pixelX > 0 && pixelX < image.Width && pixelY > 0 && pixelY < image.Height)
                    {
                        //Clone image
                        Byte[, ,] buffer = image.Clone().Data;

                        //Get offset
                        int pixelX1 = pixelX + (int)(colorExtensionNormal[0] * offset);
                        int pixelY1 = pixelY + (int)(colorExtensionNormal[1] * offset);
                        int pixelX2 = pixelX - (int)(colorExtensionNormal[0] * offset);
                        int pixelY2 = pixelY - (int)(colorExtensionNormal[1] * offset);

                        //Grab color
                        b = (buffer[pixelY1, pixelX1, 0] + buffer[pixelY2, pixelX2, 0]) / 2;
                        g = (buffer[pixelY1, pixelX1, 1] + buffer[pixelY2, pixelX2, 1]) / 2;
                        r = (buffer[pixelY1, pixelX1, 2] + buffer[pixelY2, pixelX2, 2]) / 2;

                        //Save pixel
                        colorProfile.Add(new ColorPoint(new Point(objectPoints[0, 0], objectPoints[1, 0], objectPoints[2, 0]), Color.FromArgb(r, g, b)));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GetPointColor(Point point, Camera camera, float[] colorExtensionNormal, out Color color)
        {
            Matrix<float> objectPoints = new Matrix<float>(3, 1);
            Matrix<float> imagePoints = new Matrix<float>(2, 1);
            color = Color.FromArgb(50, 200, 150);
            int offset = 8;

            try
            {
            if (camera.imgOriginal != null)
            {
                //Get pixel
                objectPoints[0, 0] = (float)point.X;
                objectPoints[1, 0] = (float)point.Y;
                objectPoints[2, 0] = (float)point.Z;

                CvInvoke.cvProjectPoints2(objectPoints.Ptr, camera.EX[0].RotationVector.Ptr, camera.EX[0].TranslationVector.Ptr, camera.IC.IntrinsicMatrix.Ptr, camera.IC.DistortionCoeffs.Ptr, imagePoints.Ptr, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0);
                int pixelX = (int)imagePoints.Data[0, 0];
                int pixelY = (int)imagePoints.Data[1, 0];

                //Get color    
                if (pixelX > 0 && pixelX < camera.imgOriginal.Width && pixelY > 0 && pixelY < camera.imgOriginal.Height)
                {
                    //Clone image
                    Byte[, ,] buffer = camera.imgOriginal.Clone().Data;

                    //Get offset
                    int pixelX1 = pixelX + (int)(colorExtensionNormal[0] * offset);
                    int pixelY1 = pixelY + (int)(colorExtensionNormal[1] * offset);
                    int pixelX2 = pixelX - (int)(colorExtensionNormal[0] * offset);
                    int pixelY2 = pixelY - (int)(colorExtensionNormal[1] * offset);

                    //Grab color
                    int r = (buffer[pixelY1, pixelX1, 2] + buffer[pixelY2, pixelX2, 2]) / 2;
                    int g = (buffer[pixelY1, pixelX1, 1] + buffer[pixelY2, pixelX2, 1]) / 2;
                    int b = (buffer[pixelY1, pixelX1, 0] + buffer[pixelY2, pixelX2, 0]) / 2;
                    color = Color.FromArgb(r, g, b);
                }
                return true;
            }
            else
                return false;
            }
            catch
            {
                return false;
            }
        }
        */

        /*
        public void StartColorExtension()
        {
            if(!camera.Connected)
                camera.Connect(colorExtensionCameraChannel);
            colorExtension = true;
        }

        public void StopColorExtension()
        {
            colorExtension = false;
        }

        public bool SetColorExtensionCameraChannel(int channel)
        {
            if (channel > 0 && channel < 5)
            {
                colorExtensionCameraChannel = channel;
                return true;
            }
            else
                return false;
        }

        public bool SetColorExtensionNormal(float[] normal)
        {
            if (normal.Count() == 2)
            {
                colorExtensionNormal = normal;
                return true;
            }
            else
                return false;
        }

        public class ColorExtensionCalibration
        {
            List<float[]> objectPointList = new List<float[]>();
            List<float[]> imagePointList = new List<float[]>();
            public Color colorMin = new Color();
            public Color colorMax = new Color();
            public Matrix<float> translation = new Matrix<float>(3, 1);
            public RotationVector3D rotation = new RotationVector3D();
            public double diviation;
            public float[] normal = new float[2];

            public void Calculate(IntrinsicCameraParameters IC)
            {
                //Calibrate
                int count = objectPointList.Count;
                Matrix<float> objectPointMatrix = new Matrix<float>(3, count);
                Matrix<float> imagePointMatrix = new Matrix<float>(2, count);
                //int n = 0;
                for (int i = 0; i < count; i++)
                {
                    objectPointMatrix[0, i] = objectPointList[i][0];
                    objectPointMatrix[1, i] = objectPointList[i][1];
                    objectPointMatrix[2, i] = objectPointList[i][2];
                    imagePointMatrix[0, i] = imagePointList[i][0];
                    imagePointMatrix[1, i] = imagePointList[i][1];
                    if (i % 2 == 0)
                    {
                        float length = (float)Math.Sqrt(Math.Pow(imagePointMatrix[0, i + 1] - imagePointMatrix[0, i], 2) + Math.Pow(imagePointMatrix[1, i + 1] - imagePointMatrix[1, i], 2));
                        normal[0] = (normal[0] + (imagePointMatrix[1, i + 1] - imagePointMatrix[1, i]) / length) / 2;
                        normal[1] = (normal[1] + (imagePointMatrix[0, i + 1] - imagePointMatrix[0, i]) / length) / 2;
                    }
                }
                CvInvoke.cvFindExtrinsicCameraParams2(objectPointMatrix.Ptr, imagePointMatrix.Ptr, IC.IntrinsicMatrix, IC.DistortionCoeffs, rotation.Ptr, translation.Ptr, 0);

                //Calculate Accuracy
                Matrix<float> objectPoints = new Matrix<float>(3, 1);
                Matrix<float> imagePoints = new Matrix<float>(2, 1);
                count = 0;
                double[] diviationArray = new double[objectPointList.Count()];

                foreach (float[] point in objectPointList)
                {
                    //Get pixel
                    objectPoints[0, 0] = point[0];
                    objectPoints[1, 0] = point[1];
                    objectPoints[2, 0] = point[2];

                    CvInvoke.cvProjectPoints2(objectPoints.Ptr, rotation.Ptr, translation.Ptr, IC.IntrinsicMatrix.Ptr, IC.DistortionCoeffs.Ptr, imagePoints.Ptr, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0);
                    int pixelX = (int)imagePoints.Data[0, 0];
                    int pixelY = (int)imagePoints.Data[1, 0];

                    //Get diviation
                    diviationArray[count] = Math.Sqrt(Math.Pow((imagePointList[count][0] - pixelX), 2) + Math.Pow((imagePointList[count][1] - pixelY), 2));

                    count++;
                }
                diviation = diviationArray.Sum() / diviationArray.Count();
            }

            public void Clear()
            {
                objectPointList = new List<float[]>();
                imagePointList = new List<float[]>();
            }

            public bool Record(List<ColorPoint> profile, Image<Bgr, Byte> image)
            {
                bool status = true;
                float[] mainLine;

                //Get profile points               
                double minX = 1000, maxX = -1000, minZ = 1000, maxZ = 0;
                foreach (ColorPoint point in profile)
                {
                    if (point.Point.X > maxX)
                    {
                        maxX = point.Point.X;
                        maxZ = point.Point.Z;

                    }
                    if (point.Point.X < minX)
                    {
                        minX = point.Point.X;
                        minZ = point.Point.Z;
                    }
                }

                //Check image points
                if (maxX != -1000 && minX != 1000 && GetMainLine(image, colorMin, colorMax, out mainLine))
                {
                    int imgRange = 50;
                    if (mainLine[0] > imgRange && mainLine[0] < (image.Size.Width - imgRange) && mainLine[1] > imgRange && mainLine[1] < (image.Size.Height - imgRange) && mainLine[2] > imgRange && mainLine[2] < (image.Size.Width - imgRange) && mainLine[3] > imgRange && mainLine[3] < (image.Size.Height - imgRange))
                    {
                        //Check profile points
                        double scannerRange = 1;
                        double gnx = 42.5, gny = 265, gpx = 42.5, gpy = -265, snx = -71.75 + scannerRange, sny = -390, spx = 29.25 - scannerRange, spy = -125;
                        if ((gny * (-minX - snx) - gnx * (-minZ - sny)) > 0 && (gpy * (-minX - spx) - gpx * (-minZ - spy)) > 0 && (gny * (-maxX - snx) - gnx * (-maxZ - sny)) > 0 && (gpy * (-maxX - spx) - gpx * (-maxZ - spy)) > 0)
                        {

                            //Save profil points
                            objectPointList.Add(new float[] { (float)maxX, 0, (float)maxZ });
                            objectPointList.Add(new float[] { (float)minX, 0, (float)minZ });

                            //Save line points
                            imagePointList.Add(new float[] { mainLine[0], mainLine[1] });
                            imagePointList.Add(new float[] { mainLine[2], mainLine[3] });
                        }
                        else
                        {
                            status = false;
                        }
                    }
                    else
                    {
                        status = false;
                    }
                }
                else
                {
                    status = false;
                }
                return status;
            }

            private bool GetMainLine(Image<Bgr, Byte> image, Color filterColorMin, Color filterColorMax, out float[] mainLine)
            {
                mainLine = new float[4];

                //Get in range image
                Bgr colorMin = new Bgr(filterColorMin.B, filterColorMin.G, filterColorMin.R);
                Bgr colorMax = new Bgr(filterColorMax.B, filterColorMax.G, filterColorMax.R);
                Image<Gray, Byte> imgProcessed = image.InRange(colorMin, colorMax);
                CvInvoke.cvDilate(imgProcessed, imgProcessed, IntPtr.Zero, 5);
                CvInvoke.cvErode(imgProcessed, imgProcessed, IntPtr.Zero, 4);

                //Get largest contour
                Contour<System.Drawing.Point> contour = null;
                double largestArea = 0;
                for (Contour<System.Drawing.Point> contours = imgProcessed.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, RETR_TYPE.CV_RETR_TREE, new MemStorage()); contours != null; contours = contours.HNext)
                {
                    if (contours.Area > largestArea)
                    {
                        largestArea = contours.Area;
                        contour = contours;
                    }
                }

                //Get main line
                float[] contourLine = new float[4];
                if (contour != null)
                {
                    CvInvoke.cvFitLine(contour, DIST_TYPE.CV_DIST_L2, 0, 0.01, 0.01, contourLine);
                    float m = contourLine[1] / contourLine[0];
                    float t = contourLine[3] - m * contourLine[2];
                    if (contour.BoundingRectangle.Height < contour.BoundingRectangle.Width)
                    {
                        mainLine[0] = contour.BoundingRectangle.Right;
                        mainLine[1] = m * contour.BoundingRectangle.Right + t;
                        mainLine[2] = contour.BoundingRectangle.Left;
                        mainLine[3] = m * contour.BoundingRectangle.Left + t;
                    }
                    else
                    {
                        mainLine[0] = (contour.BoundingRectangle.Top - t) / m;
                        mainLine[1] = contour.BoundingRectangle.Top;
                        mainLine[2] = (contour.BoundingRectangle.Bottom - t) / m;
                        mainLine[3] = contour.BoundingRectangle.Bottom;
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }

            public void ToFile(string path)
            {
                //Write calibration data to file
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine("*** Extrinsic Calibration ***");
                    file.WriteLine("Date:               " + DateTime.Now.Date);
                    file.WriteLine("Number of Points:   " + objectPointList.Count());
                    file.WriteLine("Diviation:          " + diviation);
                    file.WriteLine("");
                    file.WriteLine("Profile Points:");
                    foreach (float[] data in objectPointList)
                    {
                        file.WriteLine(data[0].ToString("F4") + "   " + data[1].ToString("F4") + "   " + data[2].ToString("F4"));
                    }
                    file.WriteLine("");
                    file.WriteLine("Image Points:");
                    foreach (float[] data in imagePointList)
                    {
                        file.WriteLine(data[0].ToString("F4") + "   " + data[1].ToString("F4"));
                    }
                    file.WriteLine("");
                    file.WriteLine("Transformation:");
                    file.WriteLine("tx = " + translation.Data[0, 0].ToString("F4") + "  ty = " + translation.Data[1, 0].ToString("F4") + "  tz = " + translation.Data[2, 0].ToString("F4"));
                    file.WriteLine("rx = " + rotation.Data[0, 0].ToString("F4") + "  ry = " + rotation.Data[1, 0].ToString("F4") + "  rz = " + rotation.Data[2, 0].ToString("F4"));
                }
            }
        }
        */
    }

    /*
    public class Camera
    {

        Thread ThreadConnect;
        public Image<Bgr, Byte> imgOriginal;
        public Image<Bgr, Byte> imgIntrinsic;
        public Image<Bgr, Byte> imgUndistort;
        Capture capwebcam = null;
        public bool Connected = false;
        public IntrinsicCameraParameters IC = new IntrinsicCameraParameters();
        public ExtrinsicCameraParameters[] EX = new ExtrinsicCameraParameters[1];
        public bool IntrinsicCalibration = false;
        List<Image<Gray, Byte>> Frames = new List<Image<Gray, Byte>>();

        public bool Connect(int Channel)
        {
            try
            {
                capwebcam = new Capture(Channel);
                capwebcam.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_CONTRAST, 160);
                capwebcam.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_BRIGHTNESS, 160);
                capwebcam.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_SHARPNESS, 160);
                capwebcam.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 1920);
                capwebcam.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 1080);
                capwebcam.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FOCUS, 15);

                ThreadConnect = new Thread(delegate()
                {
                    Connected = true;
                    while (Connected)
                    {
                        imgOriginal = capwebcam.QueryFrame();
                        if (imgOriginal == null)
                        {
                            Connected = false;
                        }
                        else
                        {
                            GetUndistortedImage();
                        }
                    }
                });
                ThreadConnect.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Disconnect()
        {
            Connected = false;
            Thread.Sleep(200);
            if (ThreadConnect != null)
                ThreadConnect.Abort();
            if (capwebcam != null)
                capwebcam.Dispose();
            imgOriginal = null;
        }

        public bool SwitchChannel(int Channel)
        {
            if (Channel >= 0 && Channel < 4)
            {
                Disconnect();
                Connect(Channel);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GetUndistortedImage()
        {
            try
            {
                Matrix<float> Map1, Map2;
                IC.InitUndistortMap(imgOriginal.Width, imgOriginal.Height, out Map1, out Map2);
                Image<Bgr, Byte> temp = imgOriginal.CopyBlank();
                CvInvoke.cvRemap(imgOriginal, temp, Map1, Map2, 0, new MCvScalar(0));
                imgUndistort = temp.Copy();
            }
            catch
            {
            }
        }

        public Image<Gray, Byte> GetInRangeImage(Color ColorMin, Color ColorMax)
        {
            Bgr colorMin = new Bgr(ColorMin.B, ColorMin.G, ColorMin.R);
            Bgr colorMax = new Bgr(ColorMax.B, ColorMax.G, ColorMax.R);
            Image<Bgr, Byte> imgTemp = imgOriginal.Clone();
            Image<Gray, Byte> imgInRange = imgTemp.InRange(colorMin, colorMax);
            CvInvoke.cvDilate(imgInRange, imgInRange, IntPtr.Zero, 5);
            CvInvoke.cvErode(imgInRange, imgInRange, IntPtr.Zero, 4);
            return imgInRange;
        }

        public void StartIntrinsicCalibration()
        {
            Thread ThreadCalibration = new Thread(delegate()
            {
                Size patternSize = new Size(9, 6);
                int size = patternSize.Width * patternSize.Height;
                Hsv[] line_colour_array = new Hsv[size];
                for (int i = 0; i < size; i++)
                {
                    line_colour_array[i] = new Hsv(180 / size * i, 255, 255);
                }
                IntrinsicCalibration = true;

                while (IntrinsicCalibration)
                {
                    Image<Hsv, Byte> imgTemp = imgOriginal.Convert<Hsv, Byte>().Copy();
                    Image<Gray, Byte> Gray_Frame = imgTemp.Convert<Gray, Byte>();
                    PointF[] corners = CameraCalibration.FindChessboardCorners(Gray_Frame, patternSize, Emgu.CV.CvEnum.CALIB_CB_TYPE.ADAPTIVE_THRESH);
                    if (corners != null)
                    {
                        Gray_Frame.FindCornerSubPix(new PointF[1][] { corners }, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                        imgTemp.Draw(new CircleF(corners[0], 3), line_colour_array[0], 3);
                        for (int i = 1; i < corners.Length; i++)
                        {
                            imgTemp.Draw(new LineSegment2DF(corners[i - 1], corners[i]), line_colour_array[i], 2);
                            imgTemp.Draw(new CircleF(corners[i], 3), line_colour_array[i], 3);
                        }
                        imgIntrinsic = imgTemp.Convert<Bgr, Byte>();
                    }
                    else
                    {
                        imgIntrinsic = imgOriginal;
                    }
                    corners = null;
                }
            });
            ThreadCalibration.Start();
        }

        public void StopIntrinsicCalibration()
        {
            IntrinsicCalibration = false;
            Frames.Clear();
        }

        public void AddIntrinsicCalibrationImage(Image image)
        {
            Bitmap bmpImage = (Bitmap)image;
            Image<Bgr, Byte> convImage = new Image<Bgr, Byte>(bmpImage);
            if (IntrinsicCalibration)
                Frames.Add(convImage.Convert<Gray, Byte>().Copy());
        }

        public void AddIntrinsicCalibrationImage(Bitmap image)
        {
            Image<Bgr, Byte> convImage = new Image<Bgr, Byte>(image);
            if (IntrinsicCalibration)
                Frames.Add(convImage.Convert<Gray, Byte>().Copy());
        }

        public double CalculateIntrinsicCalibration()
        {
            MCvPoint3D32f[][] corners_object_list = new MCvPoint3D32f[Frames.Count][];
            PointF[][] corners_points_list = new PointF[Frames.Count][];
            Image<Gray, Byte> Gray_Frame = imgOriginal.Convert<Gray, Byte>();

            if (IntrinsicCalibration)
            {
                //we can do this in the loop above to increase speed
                for (int k = 0; k < Frames.Count; k++)
                {
                    corners_points_list[k] = CameraCalibration.FindChessboardCorners(Frames[k], new Size(9, 6), Emgu.CV.CvEnum.CALIB_CB_TYPE.ADAPTIVE_THRESH);
                    //for accuracy
                    Gray_Frame.FindCornerSubPix(corners_points_list, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                    //Fill our objects list with the real world mesurments for the intrinsic calculations
                    List<MCvPoint3D32f> object_list = new List<MCvPoint3D32f>();
                    for (int i = 0; i < 6; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            object_list.Add(new MCvPoint3D32f(j * 20.0F, i * 20.0F, 0.0F));
                        }
                    }
                    corners_object_list[k] = object_list.ToArray();
                }

                //our error should be as close to 0 as possible
                return CameraCalibration.CalibrateCamera(corners_object_list, corners_points_list, Gray_Frame.Size, IC, Emgu.CV.CvEnum.CALIB_TYPE.CV_CALIB_RATIONAL_MODEL, new MCvTermCriteria(30, 0.1), out EX);
            }
            else
            {
                return 999.999;
            }

        }

    }
    */

    public class Simulation : Geometry
    {
        private vtkRenderWindow RenderWindow;
        private vtkRenderer Renderer;
        private vtkAxesActor baseAxes = new vtkAxesActor();
        private vtkActor floor = new vtkActor();
        private vtkAxesActor robot1axes1 = new vtkAxesActor(), robot1axes2 = new vtkAxesActor(), robot1axes3 = new vtkAxesActor(), robot1axes4 = new vtkAxesActor(), robot1axes5 = new vtkAxesActor(), robot1axes6 = new vtkAxesActor();
        private vtkActor robot1part1 = new vtkActor(), robot1part2 = new vtkActor(), robot1part3 = new vtkActor(), robot1part4 = new vtkActor(), robot1part5 = new vtkActor(), robot1part6 = new vtkActor(), robot1part7 = new vtkActor();

        public Position robot1Pos;
        public Position robot2Pos;

        public PointCloud PointCloud = new PointCloud();

        private vtkAxesActor robot2axes1 = new vtkAxesActor(), robot2axes2 = new vtkAxesActor(), robot2axes3 = new vtkAxesActor(), robot2axes4 = new vtkAxesActor(), robot2axes5 = new vtkAxesActor(), robot2axes6 = new vtkAxesActor();
        private vtkActor robot2part1 = new vtkActor(), robot2part2 = new vtkActor(), robot2part3 = new vtkActor(), robot2part4 = new vtkActor(), robot2part5 = new vtkActor(), robot2part6 = new vtkActor(), robot2part7 = new vtkActor();

        public Roboteam Robots = new Roboteam();


        Color colorBackground = Color.FromArgb(255, 50, 50, 50);
        Color colorBackground2 = Color.FromArgb(255, 100, 100, 100);

        public bool BaseVisibility = true, FloorVisibility = true, Robot1Visibility = true, Robot2Visibility = true;

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public Simulation(vtkRenderWindow renderWindow)
        {
            RenderWindow = renderWindow;
            Renderer = renderWindow.GetRenderers().GetFirstRenderer();

            //Background
            Renderer.GradientBackgroundOn();
            Renderer.SetBackground((double)colorBackground.R / 255, (double)colorBackground.G / 255, (double)colorBackground.B / 255);
            Renderer.SetBackground2((double)colorBackground2.R / 255, (double)colorBackground2.G / 255, (double)colorBackground2.B / 255);

            //Base
            baseAxes.SetTotalLength(200, 200, 200);
            baseAxes.AxisLabelsOff();
            Renderer.AddActor(baseAxes);

            //Floor
            vtkPlaneSource planeSource = vtkPlaneSource.New();
            planeSource.SetOrigin(-1000.0, -1000.0, 0);
            planeSource.SetPoint1(-1000.0, 1000.0, 0);
            planeSource.SetPoint2(1000.0, -1000.0, 0);
            planeSource.Update();
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            vtkPolyData plane = planeSource.GetOutput();
            mapper.SetInput(plane);
            floor = vtkActor.New();
            floor.SetMapper(mapper);
            floor.GetProperty().SetColor(0.8, 0.8, 0.8);
            Renderer.AddActor(floor);

            //Robot1
            robot1Pos = new Position(-500, 0, 0, 0, 0, 0);
            //Robots.Robot1.Base = new Position(-500, 0, 0, 0, 0, 0);

            robot1part1 = LoadSTL(robot1part1, @"\Robot\agilus_00.stl", Color.FromArgb(25, 25, 25));
            robot1part2 = LoadSTL(robot1part2, @"\Robot\agilus_01.stl", Color.FromArgb(255, 110, 0));
            robot1part3 = LoadSTL(robot1part3, @"\Robot\agilus_02.stl", Color.FromArgb(255, 110, 0));
            robot1part4 = LoadSTL(robot1part4, @"\Robot\agilus_03.stl", Color.FromArgb(255, 110, 0));
            robot1part5 = LoadSTL(robot1part5, @"\Robot\agilus_04.stl", Color.FromArgb(255, 110, 0));
            robot1part6 = LoadSTL(robot1part6, @"\Robot\agilus_05.stl", Color.FromArgb(255, 110, 0));
            robot1part7 = LoadSTL(robot1part6, @"\Robot\agilus_06.stl", Color.DarkGray);
            robot1part1.SetPosition(robot1Pos.X, robot1Pos.Y, robot1Pos.Z);
            Renderer.AddActor(robot1part1);
            Renderer.AddActor(robot1part2);
            Renderer.AddActor(robot1part3);
            Renderer.AddActor(robot1part4);
            Renderer.AddActor(robot1part5);
            Renderer.AddActor(robot1part6);
            Renderer.AddActor(robot1part7);
            robot1axes1.SetTotalLength(200, 200, 200);
            robot1axes1.AxisLabelsOff();
            Renderer.AddActor(robot1axes1);
            robot1axes2.SetTotalLength(200, 200, 200);
            robot1axes2.AxisLabelsOff();
            Renderer.AddActor(robot1axes2);
            robot1axes3.SetTotalLength(200, 200, 200);
            robot1axes3.AxisLabelsOff();
            Renderer.AddActor(robot1axes3);
            robot1axes4.SetTotalLength(200, 200, 200);
            robot1axes4.AxisLabelsOff();
            Renderer.AddActor(robot1axes4);
            robot1axes5.SetTotalLength(200, 200, 200);
            robot1axes5.AxisLabelsOff();
            Renderer.AddActor(robot1axes5);
            robot1axes6.SetTotalLength(200, 200, 200);
            robot1axes6.AxisLabelsOff();
            Renderer.AddActor(robot1axes6);

            //Robot2
            robot2Pos = new Position(1000, 0, 0, 0, 0, 0);
            Robots.Robot2.Base = robot2Pos;
            robot2part1 = LoadSTL(robot2part1, @"\Robot\agilus_00.stl", Color.FromArgb(25, 25, 25));
            robot2part2 = LoadSTL(robot2part2, @"\Robot\agilus_01.stl", Color.FromArgb(255, 110, 0));
            robot2part3 = LoadSTL(robot2part3, @"\Robot\agilus_02.stl", Color.FromArgb(255, 110, 0));
            robot2part4 = LoadSTL(robot2part4, @"\Robot\agilus_03.stl", Color.FromArgb(255, 110, 0));
            robot2part5 = LoadSTL(robot2part5, @"\Robot\agilus_04.stl", Color.FromArgb(255, 110, 0));
            robot2part6 = LoadSTL(robot2part6, @"\Robot\agilus_05.stl", Color.FromArgb(255, 110, 0));
            robot2part7 = LoadSTL(robot2part6, @"\Robot\agilus_06.stl", Color.DarkGray);
            robot2part1.SetPosition(Robots.Robot2.Base.X, Robots.Robot2.Base.Y, Robots.Robot2.Base.Z);
            Renderer.AddActor(robot2part1);
            Renderer.AddActor(robot2part2);
            Renderer.AddActor(robot2part3);
            Renderer.AddActor(robot2part4);
            Renderer.AddActor(robot2part5);
            Renderer.AddActor(robot2part6);
            Renderer.AddActor(robot2part7);
            robot2axes1.SetTotalLength(200, 200, 200);
            robot2axes1.AxisLabelsOff();
            Renderer.AddActor(robot2axes1);
            robot2axes2.SetTotalLength(200, 200, 200);
            robot2axes2.AxisLabelsOff();
            Renderer.AddActor(robot2axes2);
            robot2axes3.SetTotalLength(200, 200, 200);
            robot2axes3.AxisLabelsOff();
            Renderer.AddActor(robot2axes3);
            robot2axes4.SetTotalLength(200, 200, 200);
            robot2axes4.AxisLabelsOff();
            Renderer.AddActor(robot2axes4);
            robot2axes5.SetTotalLength(200, 200, 200);
            robot2axes5.AxisLabelsOff();
            Renderer.AddActor(robot2axes5);
            robot2axes6.SetTotalLength(200, 200, 200);
            robot2axes6.AxisLabelsOff();
            Renderer.AddActor(robot2axes6);

            Renderer.AddActor(drawPointCloud());
            Renderer.Render();

            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (RenderWindow != null)
            {
                SetRobot1();
                SetRobot2();
                Thread.Sleep(200);
            }
        }

        private vtkActor LoadSTL(vtkActor actor, string path, Color color)
        {
            vtkSTLReader reader = vtkSTLReader.New();
            string filePath = string.Concat(Environment.CurrentDirectory, path);
            reader.SetFileName(filePath);
            reader.Update();
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());
            actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetColor((double)color.R / 255, (double)color.G / 255, (double)color.B / 255);
            return actor;
        }

        private vtkActor drawPointCloud()
        {
            vtkPoints points = vtkPoints.New();
            PointCloud.LoadFile(@"U:\RobSim\curve2.asc");


            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();
            colors.SetNumberOfComponents(3);
            colors.SetName("Colors");

            foreach (ColorPoint p in PointCloud.Points)
            {
                points.InsertNextPoint(p.Point.X, p.Point.Y, p.Point.Z);
                byte[] color = new byte[] { p.Color.R, p.Color.G, p.Color.B };
                colors.InsertNextValue(color[0]);
                colors.InsertNextValue(color[1]);
                colors.InsertNextValue(color[2]);
            }

            vtkPolyData pointsPolydata = vtkPolyData.New();
            pointsPolydata.SetPoints(points);

            vtkVertexGlyphFilter vertexFilter = vtkVertexGlyphFilter.New();
            vertexFilter.SetInputConnection(pointsPolydata.GetProducerPort());
            vertexFilter.Update();

            vtkPolyData polydata = vtkPolyData.New();
            polydata.ShallowCopy(vertexFilter.GetOutput());

            polydata.GetCellData().SetScalars(colors);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(polydata.GetProducerPort());


            vtkActor actor = vtkActor.New();

            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(3);
            return actor;
        }

        public void ViewBase()
        {
            BaseVisibility = true;
            baseAxes.SetVisibility(1);
            RenderWindow.Render();
        }

        public void HideBase()
        {
            BaseVisibility = false;
            baseAxes.SetVisibility(0);
            RenderWindow.Render();
        }

        public void ViewFloor()
        {
            FloorVisibility = true;
            floor.SetVisibility(1);
            RenderWindow.Render();
        }

        public void HideFloor()
        {
            FloorVisibility = false;
            floor.SetVisibility(0);
            RenderWindow.Render();
        }

        public void ViewRobot1()
        {
            Robot1Visibility = true;
            robot1part1.SetVisibility(1);
            robot1part2.SetVisibility(1);
            robot1part3.SetVisibility(1);
            robot1part4.SetVisibility(1);
            robot1part5.SetVisibility(1);
            robot1part6.SetVisibility(1);
            robot1part7.SetVisibility(1);
            RenderWindow.Render();
        }

        public void ViewRobot2()
        {
            Robot2Visibility = true;
            robot2part1.SetVisibility(1);
            robot2part2.SetVisibility(1);
            robot2part3.SetVisibility(1);
            robot2part4.SetVisibility(1);
            robot2part5.SetVisibility(1);
            robot2part6.SetVisibility(1);
            robot2part7.SetVisibility(1);
            RenderWindow.Render();
        }

        public void HideRobot1()
        {
            Robot1Visibility = false;
            robot1part1.SetVisibility(0);
            robot1part2.SetVisibility(0);
            robot1part3.SetVisibility(0);
            robot1part4.SetVisibility(0);
            robot1part5.SetVisibility(0);
            robot1part6.SetVisibility(0);
            robot1part7.SetVisibility(0);
            RenderWindow.Render();
        }

        public void HideRobot2()
        {
            Robot2Visibility = false;
            robot2part1.SetVisibility(0);
            robot2part2.SetVisibility(0);
            robot2part3.SetVisibility(0);
            robot2part4.SetVisibility(0);
            robot2part5.SetVisibility(0);
            robot2part6.SetVisibility(0);
            robot2part7.SetVisibility(0);
            RenderWindow.Render();
        }

        private void SetRobot2()
        {
            List<Position> linkPositions = Robots.Robot2.GetLinkPositions(robot2Pos);
            TransformAxes(robot2axes1, linkPositions[0]);
            TransformAxes(robot2axes2, linkPositions[1]);
            TransformAxes(robot2axes3, linkPositions[2]);
            TransformAxes(robot2axes4, linkPositions[3]);
            TransformAxes(robot2axes5, linkPositions[4]);
            TransformAxes(robot2axes6, linkPositions[5]);
            TransformRobotPart(robot2part2, new Position(-25, 0, -400, 0, 0, 90), linkPositions[0]);
            TransformRobotPart(robot2part3, new Position(-25, 0, -855, 0, 90, 90), linkPositions[1]);
            TransformRobotPart(robot2part4, new Position(-25, 0, -890, 0, 90, 180), linkPositions[2]);
            TransformRobotPart(robot2part5, new Position(-445, 0, -890, 0, 90, 90), linkPositions[3]);
            TransformRobotPart(robot2part6, new Position(-445, 0, -890, 0, -90, 0), linkPositions[4]);
            TransformRobotPart(robot2part7, new Position(-525, 0, -890, 0, -90, 0), linkPositions[5]);
        }

        private void SetRobot1()
        {

            List<Position> linkPositions = Robots.Robot1.GetLinkPositions(robot1Pos);
            TransformAxes(robot1axes1, linkPositions[0]);
            TransformAxes(robot1axes2, linkPositions[1]);
            TransformAxes(robot1axes3, linkPositions[2]);
            TransformAxes(robot1axes4, linkPositions[3]);
            TransformAxes(robot1axes5, linkPositions[4]);
            TransformAxes(robot1axes6, linkPositions[5]);
            TransformRobotPart(robot1part2, new Position(-25, 0, -400, 0, 0, 90), linkPositions[0]);
            TransformRobotPart(robot1part3, new Position(-25, 0, -855, 0, 90, 90), linkPositions[1]);
            TransformRobotPart(robot1part4, new Position(-25, 0, -890, 0, 90, 180), linkPositions[2]);
            TransformRobotPart(robot1part5, new Position(-445, 0, -890, 0, 90, 90), linkPositions[3]);
            TransformRobotPart(robot1part6, new Position(-445, 0, -890, 0, -90, 0), linkPositions[4]);
            TransformRobotPart(robot1part7, new Position(-525, 0, -890, 0, -90, 0), linkPositions[5]);
        }

        private void TransformAxes(vtkAxesActor axes, Position position)
        {
            vtkTransform transform = new vtkTransform();
            transform.Translate(position.X, position.Y, position.Z);
            transform.RotateWXYZ(position.A, 0, 0, 1);
            transform.RotateWXYZ(position.B, 0, 1, 0);
            transform.RotateWXYZ(position.C, 1, 0, 0);
            axes.SetUserTransform(transform);
        }

        private void TransformRobotPart(vtkActor part, Position shift, Position position)
        {
            vtkTransform transform = new vtkTransform();
            transform.Translate(position.X, position.Y, position.Z);
            transform.RotateWXYZ(position.A, 0, 0, 1);
            transform.RotateWXYZ(position.B, 0, 1, 0);
            transform.RotateWXYZ(position.C, 1, 0, 0);
            if (part == robot1part7)
                transform.RotateWXYZ(11.5, 0, 0, 1);
            transform.RotateWXYZ(shift.C, 1, 0, 0);
            transform.RotateWXYZ(shift.B, 0, 1, 0);
            transform.RotateWXYZ(shift.A, 0, 0, 1);
            transform.Translate(shift.X, shift.Y, shift.Z);
            part.SetUserTransform(transform);
        }

    }

    /*
    public class Pump
    {
        SerialPort serialPort = new SerialPort();
        public bool Connection = false;
        public bool Cleaning = false;
        public enum Mode { Timer, Remote, Continuous };
        public enum Direction { Extrude, Inhale };

        public void Connect()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.PortName = "COM1";
                serialPort.BaudRate = 57600;
                serialPort.DataBits = 8;
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One");
                serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None");
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), "None"); ;
                serialPort.Open();
            }
            Connection = true;
        }

        public void Disconnect()
        {
            if (serialPort.IsOpen)
            {
                Stop();
                serialPort.Close();
            }
            Connection = false;
        }

        public void SetMode(Mode mode)
        {
            if (serialPort.IsOpen)
            {
                switch (mode)
                {
                    case Mode.Timer:
                        serialPort.Write("1");
                        break;
                    case Mode.Continuous:
                        serialPort.Write("2");
                        break;
                    case Mode.Remote:
                        serialPort.Write("3");
                        break;
                }
            }
            else
                Disconnect();
        }

        public void Rotate(int rpm)
        {
            if (serialPort.IsOpen)
            {
                string text;
                serialPort.Write("e");
                Thread.Sleep(100);
                text = "0000,";
                serialPort.Write(text);
                Thread.Sleep(20);
                text = "0000,";
                serialPort.Write(text);
                Thread.Sleep(20);
                text = "0000,";
                serialPort.Write(text);
                Thread.Sleep(20);
                text = rpm.ToString("000");
                serialPort.Write(text);
                Thread.Sleep(20);
                serialPort.Write("\r");
                Thread.Sleep(20);
                if (rpm == 0)
                {
                    serialPort.Write("s");
                }
                else if (rpm > 0)
                {
                    serialPort.Write("r");
                }

                else if (rpm < 0)
                {
                    serialPort.Write("l");
                }
                Thread.Sleep(15);
                serialPort.Write("3");
                Thread.Sleep(1000);
                serialPort.Write("S");
            }
            else
                Disconnect();
        }

        public void Start()
        {
            if (serialPort.IsOpen)
                serialPort.Write("S");
            else
                Disconnect();
        }

        public void Stop()
        {
            if (serialPort.IsOpen)
                serialPort.Write("s");
            else
                Disconnect();
        }

        public void SetDirection(Direction direction)
        {
            if (serialPort.IsOpen)
            {
                switch (direction)
                {
                    case Direction.Extrude:
                        serialPort.Write("r");
                        break;
                    case Direction.Inhale:
                        serialPort.Write("l");
                        break;
                }
            }
            else
                Disconnect();
        }

        public void Clean()
        {
            if (serialPort.IsOpen)
            {
                Cleaning = true;

                //Switch valve to acetone
                Relais relais = new Relais();
                relais.SetPort(0, true);

                //Pump
                SetDirection(Direction.Extrude);
                Rotate(200);
                new Thread(() =>
                {
                    //Wait
                    Thread.Sleep(1000 * 60);
                    Stop();
                    Cleaning = false;

                    //Switch valve to glue
                    relais.SetPort(0, false);
                }).Start();
            }
            else
                Disconnect();
        }
    }

    public class Relais
    {
        bool[] ports = new bool[4];
        public bool Connection = true;
        IntPtr m_hParstat; //This is the variable for the event handle. It is being created in the CreateFile() function and used to Read or Write 
        byte[] latch_state_recieved = new byte[1];

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess,
        uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition,
        uint dwFlagsAndAttributes, uint hTemplateFile);

        //Parameter for CreateFile()
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint OPEN_EXISTING = 3;
        public const Int32 INVALID_HANDLE_VALUE = -1;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern void CloseHandle(IntPtr handle);

        [DllImport("CP210xRuntime.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr CP210xRT_WriteLatch(IntPtr m_hParstat, Byte ipbMask, Byte ipbLatch);

        [DllImport("CP210xRuntime.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CP210xRT_ReadLatch(IntPtr m_hParstat, byte[] latch_state_recieved);

        void SetStatus()
        {
            byte StatusSetzen = 255; //255 all ports are set to 0
            if (ports[0])
                StatusSetzen--;
            if (ports[1])
                StatusSetzen -= 2;
            if (ports[2])
                StatusSetzen -= 4;
            if (ports[3])
                StatusSetzen -= 8;

            //Creates a handle, which is used in WriteLatch
            m_hParstat = CreateFile("\\\\.\\COM3", GENERIC_READ | GENERIC_WRITE, 0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, 0);
            if (m_hParstat.ToInt32() == INVALID_HANDLE_VALUE)
                Connection = false;

            //writes the latch state, StatusSetzen is between 239 and 255. 255= all states OFF 239=all state ON
            CP210xRT_WriteLatch(m_hParstat, 15, StatusSetzen);
            CloseHandle(m_hParstat);
        }

        void GetStatus()
        {
            int[] bitRechnung = new int[] { 1, 2, 4, 8 };
            int i;

            //Creates a handle, which is used in ReadLatch
            m_hParstat = CreateFile("\\\\.\\COM3", GENERIC_READ | GENERIC_WRITE, 0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, 0);
            if (m_hParstat.ToInt32() == INVALID_HANDLE_VALUE)
                Connection = false;

            //reads the state of the pins and sets latch_state_recieved between 0 and 15
            CP210xRT_ReadLatch(m_hParstat, latch_state_recieved);
            CloseHandle(m_hParstat);

            for (i = 0; i < 4; i++)
                ports[i] = true;
            if ((latch_state_recieved[0] / 8) == 1)
                ports[3] = false;
            if ((latch_state_recieved[0] % 8) / 4 == 1)
                ports[2] = false;
            if (((latch_state_recieved[0] % 8)) % 4 / 2 == 1)
                ports[1] = false;
            if ((((latch_state_recieved[0] % 8)) % 4) % 2 == 1)
                ports[0] = false;
        }

        public void SetPort(int port, bool state)
        {
            GetStatus();
            ports[port] = state;
            SetStatus();
        }

        public bool GetPort(int port)
        {
            GetStatus();
            return ports[port];
        }
    }
    */

    public class Gamepad
    {
        DirectInput dinput = new DirectInput();
        SlimDX.DirectInput.Joystick joystick;
        SlimDX.DirectInput.JoystickState state = new SlimDX.DirectInput.JoystickState();
        public bool Connected = false;
        Thread ThreadTransmit;

        public bool[] Buttons = new bool[13];
        public int[] Joystick = new int[7];
        public int[] Sliders = new int[2];

        public Robot GamepadRobot = new Robot();
        public double Increment = 1;

        public struct Settings
        {
            public string Button;
            public string Type;
            public double Modifier;
        }

        public Settings[] IniList = new Settings[20];

        private void Declare()
        {
            IniList[0].Type = "X";
            IniList[1].Type = "Y";
            IniList[2].Type = "Z";
            IniList[3].Type = "A";
            IniList[4].Type = "B";
            IniList[5].Type = "C";
            IniList[6].Type = "A1";
            IniList[7].Type = "A2";
            IniList[8].Type = "A3";
            IniList[9].Type = "A4";
            IniList[10].Type = "A5";
            IniList[11].Type = "A6";
            IniList[12].Type = "Mode";
            IniList[13].Type = "Gripper";
            IniList[14].Type = "Cooling";
            IniList[15].Type = "Home";
            IniList[16].Type = "ConnectDisconnect";
            IniList[17].Type = "SwitchRobot";
            IniList[18].Type = "Increment";
            IniList[19].Type = "Safety";
        }

        public List<string> GetConnectedGamepads()
        {
            List<string> DevicesNames = new List<string>();

            foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                DevicesNames.Add(device.InstanceName);
            }

            return DevicesNames;
        }

        public void Connect(int index)
        {
            List<DeviceInstance> devices = new List<DeviceInstance>();

            Declare();
            dinput = new DirectInput();

            //Get connected devices
            foreach (DeviceInstance device in dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                devices.Add(device);
            }

            if (devices.Count() > 0)
            {
                try
                {
                    //Connect to selected device
                    joystick = new SlimDX.DirectInput.Joystick(dinput, devices[index].InstanceGuid);
                    foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
                    {
                        if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                            joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);
                    }
                    joystick.Acquire();
                    Connected = true;
                }
                catch (DirectInputException)
                {
                    Connected = false;
                }
                //Get data from selected gamepad
                ThreadTransmit = new Thread(delegate()
                {
                    while (Connected)
                    {
                        Thread.Sleep(50);
                        GetInput();
                    }
                });
                ThreadTransmit.Start();
            }
        }

        public void Disconnect()
        {
            if (ThreadTransmit != null)
                ThreadTransmit.Abort();
            joystick.Dispose();
            Connected = false;
        }

        private void GetInput()
        {
            state = joystick.GetCurrentState();
            Buttons = state.GetButtons();
            Joystick[0] = state.X;
            Joystick[1] = state.Y;
            Joystick[2] = state.Z;
            Joystick[3] = state.RotationX;
            Joystick[4] = state.RotationY;
            Joystick[5] = state.RotationZ;
            Sliders = state.GetPointOfViewControllers();
            for (int i = 0; i < 7; i++)
            {
                if ((Joystick[i] > 200) || (Joystick[i] < -200))
                {
                    Move(i, Joystick[i]);
                }
            }
            for (int i = 0; i < 10; i++)
            {
                if (Buttons[i])
                {
                    Move(i, 500);
                }
            }
        }

        private void Move(int index, int step)
        {
            //Safty query
            if (Joystick[2] == 996)
            {
                switch (IniList[index].Button)
                {
                    case "X":
                        GamepadRobot.TPos.X = GamepadRobot.Pos.X + step * IniList[index].Modifier * Increment;
                        break;
                    case "Y":
                        GamepadRobot.TPos.Y = GamepadRobot.Pos.Y + step * IniList[index].Modifier * Increment;
                        break;
                    case "Z":
                        GamepadRobot.TPos.Z = GamepadRobot.Pos.Z + step * IniList[index].Modifier * Increment;
                        break;
                    case "A":
                        GamepadRobot.TPos.A = GamepadRobot.Pos.A + step * IniList[index].Modifier * Increment;
                        break;
                    case "B":
                        GamepadRobot.TPos.B = GamepadRobot.Pos.B + step * IniList[index].Modifier * Increment;
                        break;
                    case "C":
                        GamepadRobot.TPos.C = GamepadRobot.Pos.C + step * IniList[index].Modifier * Increment;
                        break;
                    case "A1":
                        GamepadRobot.TAxs.X = GamepadRobot.Axs.X + step * IniList[index].Modifier * Increment;
                        break;
                    case "A2":
                        GamepadRobot.TAxs.Y = GamepadRobot.Axs.Y + step * IniList[index].Modifier * Increment;
                        break;
                    case "A3":
                        GamepadRobot.TAxs.Z = GamepadRobot.Axs.Z + step * IniList[index].Modifier * Increment;
                        break;
                    case "A4":
                        GamepadRobot.TAxs.A = GamepadRobot.Axs.A + step * IniList[index].Modifier * Increment;
                        break;
                    case "A5":
                        GamepadRobot.TAxs.B = GamepadRobot.Axs.B + step * IniList[index].Modifier * Increment;
                        break;
                    case "A6":
                        GamepadRobot.TAxs.C = GamepadRobot.Axs.C + step * IniList[index].Modifier * Increment;
                        break;
                    case "Mode":
                        break;
                    case "Gripper":
                        break;
                    case "Cooling":
                        break;
                    case "Home":
                        break;
                    case "ConnectDisconnect":
                        break;
                    case "SwitchRobot":
                        break;
                    case "Increment":
                        break;
                }
            }
        }

        public void LoadIni(string path)
        {
            int count = 0;
            string line;
            StreamReader LoadIniStream = new StreamReader(File.OpenRead(path));
            while ((line = LoadIniStream.ReadLine()) != null)
            {
                IniList[count].Button = line.Substring(0, line.LastIndexOf('='));
                IniList[count].Type = line.Substring(line.LastIndexOf('=') + 1, line.LastIndexOf(' ') - line.LastIndexOf('=') - 1);
                IniList[count].Modifier = Convert.ToSingle(line.Substring(line.LastIndexOf(' '), line.Length - line.LastIndexOf(' ')));
                count++;
            }
            LoadIniStream.Dispose();
        }

        public void SaveIni(string path)
        {
            int count = 0;
            for (int i = 0; i < IniList.Count(); i++)
            {
                if (IniList[i].Button != null)
                    count++;
            }
            StreamWriter SaveIniStream = new StreamWriter(File.Create(path));
            for (int i = 0; i < count; i++)
                SaveIniStream.WriteLine(IniList[i].Button + '=' + IniList[i].Type + ' ' + IniList[i].Modifier.ToString());
            SaveIniStream.Dispose();
        }

        public void SetButton(string Type)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(GetPressedButton));
            thread.Start(Type);
        }

        private void GetPressedButton(object Type)
        {
            bool[] test = new bool[20];
            bool pressed = false;
            int[] JoystickOld = new int[7];
            bool[] OldButtons = new bool[13];
            int[] OldSliders = new int[2];
            string[] ButtonStringChanged = { "X", "Y", "T", "4th", "5th" };

            for (int i = 0; i < 5; i++)
                JoystickOld[i] = Joystick[i];

            for (int k = 0; k < 13; k++)
                OldButtons[k] = Buttons[k];

            for (int l = 0; l < 2; l++)
                OldSliders[l] = Sliders[l];

            while (!pressed)
            {
                for (int i = 0; i < 7; i++)
                {
                    if ((Joystick[i] != JoystickOld[i]) && ((Joystick[i] > 400) || (Joystick[i] < -400)))
                    {
                        for (int k = 0; k < 20; k++)
                        {
                            if (IniList[k].Type == Type.ToString())
                            {
                                if (i == 2)
                                {
                                    if (Joystick[i] < 0)
                                        IniList[k].Button = "R" + ButtonStringChanged[i];
                                    else
                                        IniList[k].Button = "L" + ButtonStringChanged[i];
                                }
                                else
                                {
                                    if (Joystick[i] < 0)
                                        IniList[k].Button = ButtonStringChanged[i];
                                    else
                                        IniList[k].Button = ButtonStringChanged[i] + "-";
                                }
                            }
                        }
                        pressed = true;
                    }
                }

                if (!pressed)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (Buttons[i] != OldButtons[i])
                        {
                            for (int k = 0; k < 20; k++)
                            {
                                if (IniList[k].Type == Type.ToString())
                                    IniList[k].Button = i.ToString();
                            }
                            pressed = true;
                        }
                    }
                }

                if (!pressed)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (Sliders[i] != OldSliders[i])
                        {
                            for (int k = 0; k < 20; k++)
                            {
                                if (IniList[k].Type == Type.ToString())
                                {
                                    if (Sliders[i] == 0)
                                        IniList[k].Button = "up";

                                    if (Sliders[i] == 18000)
                                        IniList[k].Button = "down";

                                    if (Sliders[i] == 27000)
                                        IniList[k].Button = "left";

                                    if (Sliders[i] == 9000)
                                        IniList[k].Button = "right";
                                }
                            }
                            pressed = true;
                        }
                    }
                }
            }
        }

        /*Optimierungen:
         * - Safty-Button variabel machen >> XInput
         * - Entfernen des Gamepads erkennen
         * - Anpassung der Geschwindigkeitsfestlegung
         * - Arraylänge veriabel machen
         */
    }

}
