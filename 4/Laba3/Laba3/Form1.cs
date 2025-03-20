﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Laba3.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Laba3
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            map = new Bitmap(resx,resy);
            XYZ();

        }
        public struct Matrix3x3
        {
            public float V00;
            public float V01;
            public float V02;
            public float V10;
            public float V11;
            public float V12;
            public float V20;
            public float V21;
            public float V22;
            public static Matrix3x3 Identity
            {
                get
                {
                    Matrix3x3 m = new Matrix3x3();
                    m.V00 = m.V11 = m.V22 = 1;
                    return m;
                }
            }
            public static Matrix3x3 CreateRotationX(float angle)
            {
                Matrix3x3 m = Matrix3x3.Identity;
                float radians = angle * (float)Math.PI / 180;
                float cos = (float)Math.Cos(radians);
                float sin = (float)Math.Sin(radians);

                m.V11 = m.V22 = cos;
                m.V12 = -sin;
                m.V21 = sin;
                m.V01 = 0f;
                m.V02 = 0f;
                m.V10 = 0f;
                m.V20 = 0f;
                m.V00 = 1f;

                return m;
            }
            public static Matrix3x3 CreateRotationY(float angle)
            {
                Matrix3x3 m = Matrix3x3.Identity;
                float radians = angle * (float)Math.PI / 180;
                float cos = (float)System.Math.Cos(radians);
                float sin = (float)System.Math.Sin(radians);

                m.V00 = m.V22 = cos;
                m.V02 = sin;
                m.V20 = -sin;
                m.V11 = 1;

                return m;
            }
            public static Matrix3x3 CreateRotationZ(float angle)
            {
                Matrix3x3 m = Matrix3x3.Identity;
                float radians = angle * (float)Math.PI / 180;
                float cos = (float)System.Math.Cos(radians);
                float sin = (float)System.Math.Sin(radians);

                m.V00 = m.V11 = cos;
                m.V01 = -sin;
                m.V10 = sin;
                m.V22 = 1;
                return m;
            }
            public static Matrix3x3 operator *(Matrix3x3 matrix1, Matrix3x3 matrix2)
            {
                Matrix3x3 m = new Matrix3x3();

                m.V00 = matrix1.V00 * matrix2.V00 + matrix1.V01 * matrix2.V10 + matrix1.V02 * matrix2.V20;
                m.V01 = matrix1.V00 * matrix2.V01 + matrix1.V01 * matrix2.V11 + matrix1.V02 * matrix2.V21;
                m.V02 = matrix1.V00 * matrix2.V02 + matrix1.V01 * matrix2.V12 + matrix1.V02 * matrix2.V22;

                m.V10 = matrix1.V10 * matrix2.V00 + matrix1.V11 * matrix2.V10 + matrix1.V12 * matrix2.V20;
                m.V11 = matrix1.V10 * matrix2.V01 + matrix1.V11 * matrix2.V11 + matrix1.V12 * matrix2.V21;
                m.V12 = matrix1.V10 * matrix2.V02 + matrix1.V11 * matrix2.V12 + matrix1.V12 * matrix2.V22;

                m.V20 = matrix1.V20 * matrix2.V00 + matrix1.V21 * matrix2.V10 + matrix1.V22 * matrix2.V20;
                m.V21 = matrix1.V20 * matrix2.V01 + matrix1.V21 * matrix2.V11 + matrix1.V22 * matrix2.V21;
                m.V22 = matrix1.V20 * matrix2.V02 + matrix1.V21 * matrix2.V12 + matrix1.V22 * matrix2.V22;

                return m;
            }

            public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector)
            {
                return new Vector3(
                    matrix.V00 * vector.X + matrix.V01 * vector.Y + matrix.V02 * vector.Z,
                    matrix.V10 * vector.X + matrix.V11 * vector.Y + matrix.V12 * vector.Z,
                    matrix.V20 * vector.X + matrix.V21 * vector.Y + matrix.V22 * vector.Z
                    );
            }
        }
        public struct Vector3
        {
            public float X;
            public float Y;
            public float Z;

            public Vector3(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
        public List<PointF> Axes_2D_ = new List<PointF>();
        public List<Vector3> Axes_3D_ = new List<Vector3>();
        public List<Vector3> P = new List<Vector3>();
        public List<Vector3> P_Bezie = new List<Vector3>();
        public List<Vector3> P_Bsplain2 = new List<Vector3>();
        public List<Vector3> P_Bsplain3 = new List<Vector3>();
        public List<List<Vector3>> P_splain = new List<List<Vector3>>();
        public int K1=0;
        public int K2=0;
        public int K3=0;

        int resx = 1920;
        int resy = 1080;
        int x0 = 810;
        int y0 = 475;
        static double delta = 0.1;
        double delta2 = delta / (1.0 - delta);
        int diff = Convert.ToInt32(1 / delta);
        Bitmap map;
        string color1 = "Black";
        string color2 = "Magenta";
        string color3 = "OrangeRed";
        string color4 = "Yellow";


        public void Scrolling(List<Vector3> Points, string color)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = Matrix3x3.CreateRotationZ(-K3) * Matrix3x3.CreateRotationY(-K2) * Matrix3x3.CreateRotationX(-K1) * Points[i];
                Points[i] = Matrix3x3.CreateRotationX(trackBar1.Value) * Matrix3x3.CreateRotationY(trackBar2.Value) * Matrix3x3.CreateRotationZ(trackBar3.Value) * Points[i];
            }
            drawarray(Points, color);
        }
         void thick_dot(int x, int y, string color)
        {
            map.SetPixel(x, y, Color.FromName(color));
            map.SetPixel(x + 1, y, Color.FromName(color));
            map.SetPixel(x, y + 1, Color.FromName(color));
            map.SetPixel(x + 1, y + 1, Color.FromName(color));
            

        }
        void line(PointF A, PointF B, string color)
        {
            int x = (int)A.X;//начальная координата x
            int y = (int)A.Y;//начальная координата y
            int dx;//приращение по x
            int dy;//приращение по y
            int sx;//направление приращения по x
            int sy;//направление приращения по y
            if ((int)B.X > (int)A.X)
            {
                dx = (int)B.X - (int)A.X;
            }
            else
            {
                dx = (int)A.X - (int)B.X;
            }

            if ((int)B.Y > (int)A.Y)
            {
                dy = (int)B.Y - (int)A.Y;
            }
            else
            {
                dy = (int)A.Y - (int)B.Y;
            }
            //узнаем четверть
            if ((int)B.X >= (int)A.X)
            {
                sx = 1;
            }
            else
            {
                sx = -1;
            }
            if ((int)B.Y >= (int)A.Y)
            {
                sy = 1;
            }
            else
            {
                sy = -1;
            }
            //узнаем 1\8

            if (dy < dx)//+-45 градусов
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;
                thick_dot((int)A.X, (int)A.Y, color);
                pictureBox1.Image = map;
                if ((int)A.X + sx <= (int)B.X)//вторая точка правее первой
                {
                    for (x = (int)A.X + sx; x <= (int)B.X; x++)
                    {
                        if (d > 0)
                        {
                            d += d2;
                            y += sy;
                        }
                        else
                        {
                            d += d1;
                        }

                        thick_dot(x, y, color);
                        pictureBox1.Image = map;
                    }

                }
                else//вторая точка левее первой
                {

                    for (x = (int)A.X + sx; x != (int)B.X; x--)
                    {
                        if (d > 0)
                        {
                            d += d2;
                            y += sy;
                        }
                        else
                        {
                            d += d1;
                        }

                        thick_dot(x, y, color);
                        pictureBox1.Image = map;
                    }
                }
            }
            //x меняем y
            else//угол больше 45 градусов
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;
                thick_dot((int)A.X, (int)A.Y, color);
                pictureBox1.Image = map;
                if ((int)A.Y + sy <= (int)B.Y)
                {
                    for (y = (int)A.Y + sy; y <= (int)B.Y; y++)
                    {
                        if (d > 0)
                        {
                            d += d2;
                            x += sx;
                        }
                        else
                        {
                            d += d1;
                        }

                        thick_dot(x, y, color);
                        pictureBox1.Image = map;
                    }
                }
                else
                {
                    for (y = (int)A.Y + sy; y != (int)B.Y; y--)
                    {
                        if (d > 0)
                        {
                            d += d2;
                            x += sx;
                        }
                        else
                        {
                            d += d1;
                        }

                        thick_dot(x, y, color);
                        pictureBox1.Image = map;
                    }
                }
            }
        }

        public void Swap(ref int x, ref int y)
        {
            var temp = x;
            x = y;
            y = temp;
        }
        void XYZ()
        {
            Vector3 O0 = new Vector3(0, 0, 0);
            Vector3 OZ = new Vector3(0, 0, -400);
            Vector3 OY = new Vector3(400, 0, 0);
            Vector3 OX = new Vector3(0, 400, 0);
            Axes_3D_.Add(O0);
            Axes_3D_.Add(OZ);
            Axes_3D_.Add(OY);
            Axes_3D_.Add(OX);
            List<PointF> b = new List<PointF>();
            b = T_2D(Axes_3D_);

            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[1].X + x0, b[1].Y + y0), "Red");
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[2].X + x0, b[2].Y + y0), "Blue");
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[3].X + x0, b[3].Y + y0), "Green");

        }
        public List<PointF> T_2D(List<Vector3> a)
        {
            float phi = 20 * (float)Math.PI / (float)180;
            float psi = 50 * (float)Math.PI / (float)180;
            var e1 = new Vector3((float)Math.Cos(phi), (float)Math.Sin(phi), 0);
            var e2 = new Vector3((float)Math.Sin(phi) * (float)Math.Sin(psi), (float)Math.Cos(phi) * (float)Math.Sin(psi), (float)Math.Cos(psi));
            var list = new List<PointF>();
            for (int i = 0; i < a.Count; i++)
            {
                list.Add(new PointF(scal(e1, a[i]), scal(e2, a[i])));
            }
            return list;

        }
        public float scal(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Bitmap a = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = a;
            map = a;
            for (int i = 0; i < Axes_3D_.Count; i++)
            {
                Axes_3D_[i] = Matrix3x3.CreateRotationZ(-K3) * Matrix3x3.CreateRotationY(-K2) * Matrix3x3.CreateRotationX(-K1) * Axes_3D_[i];
                Axes_3D_[i] = Matrix3x3.CreateRotationX(trackBar1.Value) * Matrix3x3.CreateRotationY(trackBar2.Value) * Matrix3x3.CreateRotationZ(trackBar3.Value) * Axes_3D_[i];
            }
            List<PointF> b = new List<PointF>();
            b = T_2D(Axes_3D_);
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[1].X + x0, b[1].Y + y0), "Red");
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[2].X + x0, b[2].Y + y0), "Blue");
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[3].X + x0, b[3].Y + y0), "Green");
            Scrolling(P, color1);
            Scrolling(P_Bezie, color2);
            for (int i = 0; i < P_splain.Count; i++)
            {
                Scrolling(P_splain[i], color3);
            }
            pictureBox1.Image = map;
            K1 = trackBar1.Value;
        }
        
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            Bitmap a = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = a;
            map = a;
            for (int i = 0; i < Axes_3D_.Count; i++)
            {
                Axes_3D_[i] = Matrix3x3.CreateRotationZ(-K3) * Matrix3x3.CreateRotationY(-K2) * Matrix3x3.CreateRotationX(-K1) * Axes_3D_[i];
                Axes_3D_[i] = Matrix3x3.CreateRotationX(trackBar1.Value) * Matrix3x3.CreateRotationY(trackBar2.Value) * Matrix3x3.CreateRotationZ(trackBar3.Value) * Axes_3D_[i];
            }
          
            List<PointF> b = new List<PointF>();
            b = T_2D(Axes_3D_);
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[1].X + x0, b[1].Y + y0), "Red");
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[2].X + x0, b[2].Y + y0), "Blue");
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[3].X + x0, b[3].Y + y0), "Green");
            Scrolling(P, color1);
            Scrolling(P_Bezie, color2);
            for (int i = 0; i < P_splain.Count; i++)
            {
                Scrolling(P_splain[i], color3);
            }
            pictureBox1.Image = map;
            K2 = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            Bitmap a = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = a;
            map = a;
            for (int i = 0; i < Axes_3D_.Count; i++)
            {
                Axes_3D_[i] = Matrix3x3.CreateRotationZ(-K3) * Matrix3x3.CreateRotationY(-K2) * Matrix3x3.CreateRotationX(-K1) * Axes_3D_[i];
                Axes_3D_[i] = Matrix3x3.CreateRotationX(trackBar1.Value) * Matrix3x3.CreateRotationY(trackBar2.Value) * Matrix3x3.CreateRotationZ(trackBar3.Value) * Axes_3D_[i];
            }
            List<PointF> b = new List<PointF>();
            b = T_2D(Axes_3D_);
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[1].X + x0, b[1].Y + y0), "Red");
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[2].X + x0, b[2].Y + y0), "Blue");
            line(new PointF(b[0].X + x0, b[0].Y + y0), new PointF(b[3].X + x0, b[3].Y + y0), "Green");
            Scrolling(P, color1);
            Scrolling(P_Bezie, color2);
            for (int i = 0; i < P_splain.Count; i++)
            {
                Scrolling(P_splain[i], color3);
            }

            pictureBox1.Image = map;
            K3 = trackBar3.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            spheredr();
            List<PointF> b = new List<PointF>();
            b = T_2D(P);
            drawarray(P, color1);
            pictureBox1.Image = map;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

        }
        private void drawarray(List<Vector3> Points, string color)
        {
            List<PointF> b = new List<PointF>();
            b = T_2D(Points);
            int dd =(int) Math.Sqrt(b.Count);
            for (int u = 0; u < dd; u++) 
            {
                
                for (int v = 0; v < dd - 1  ; v++)
                {
                    line(new PointF(b[u * dd + v].X + x0, b[u * dd + v].Y + y0), new PointF(b[u * dd + v + 1].X + x0, b[u * dd + v + 1].Y + y0), color);
                    //MessageBox.Show("aa");
                }
            }
            for (int u = 0; u < dd; u++)
            {
                
                for (int v = 0; v < dd - 1; v++)
                {
                    line(new PointF(b[v * dd + u].X + x0, b[v * dd + u].Y + y0), new PointF(b[(v+1) * dd + u ].X + x0, b[(v+1) * dd + u ].Y + y0), color);
                }
            }
            
        }

        private void spheredr()
        {
            int R = 150;
            int r = R / 2;
            for (double u = 0; Math.Round(u,5) <= 1.0; u += delta2)
            {
                for (double v = 0; Math.Round(v, 5) <= 1.0; v += delta2)
                {
                    //double x = Math.Cos(2 * Math.PI * u) * Math.Cos(Math.PI * v - Math.PI/2) * R;
                    //double y = Math.Sin(2 * Math.PI * u) * Math.Cos(Math.PI * v - Math.PI / 2) * R;
                    //double z = Math.Sin(Math.PI * v - Math.PI / 2) * R;

                    double x = (R + r * Math.Cos(2 * Math.PI * v - Math.PI)) * Math.Cos(2 * Math.PI * u);
                    double y = (R + r * Math.Cos(2 * Math.PI * v - Math.PI)) * Math.Sin(2 * Math.PI * u);
                    double z = r * Math.Sin(2 * Math.PI * v - Math.PI);


                    Vector3 point = new Vector3( Convert.ToSingle(x) , Convert.ToSingle(y), (-1) * Convert.ToSingle(z));
                    point = Matrix3x3.CreateRotationX(trackBar1.Value) * Matrix3x3.CreateRotationY(trackBar2.Value) * Matrix3x3.CreateRotationZ(trackBar3.Value) * point;
                    P.Add(point);
                    //MessageBox.Show("count=" +P.Count+ " v=" + v + " u=" + u + " x=" + x + " y=" + y +  " z=" + z);
                }
            }
        }
        public static int Fact(int num)
        {
            int n = 1;

            for (int i = 1; i <= num; i++)
            {
                n *= i;
            }

            return n; // returns 1 when number is 0
        }
        double Bu(double u,int nn)
        {
            return Math.Pow(u, nn);
        }
        double Bv(double v,int nn, int dim)
        {
            return Math.Pow(1 - v, dim - nn);
        }

        private static int Cnk(int k, int n)
        {
            return Fact(n) / (Fact(k) * Fact(n - k));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int dimU = diff-1;
            int dimV = diff-1;
            double s1=0, s2=0;
            int k1 = 0, k2 = 0;
            List <List<double>> tt  = new List<List<double>>();
            for (double u = 0; Math.Round(u, 5) <= 1.0; u += delta2)
            {
                for (double v = 0; Math.Round(v, 5) <= 1.0; v += delta2)
                {
                    float zero = 0.0f;
                    Vector3 Pb = new Vector3(zero, zero, zero);
                    for (int i=0;i<=dimU;i++)
                    {
                        for(int j=0;j<=dimV;j++)
                        {
                            k1 = 0;
                            if (u==0 && i==0)
                            {
                                s1 = 1;
                                k1 = 1;
                            }
                            if (u == 0 && i > 0)
                            {
                                s1 = 0;
                                k1 = 1;
                            }
                            
                            if(Math.Round(u, 5) == 1.0 && i <dimU)
                            {
                                s1 = 0;
                                k1 = 1;
                            }
                            if (Math.Round(u, 5) == 1.0 && i == dimU)
                            {
                                s1 = 1;
                                k1 = 1;
                            }

                            k2 = 0;
                            if (v == 0 && j == 0)
                            {
                                s2 = 1;
                                k2 = 1;
                            }
                            if (v == 0 && j > 0)
                            {
                                s2 = 0;
                                k2 = 1;
                            }

                            if (Math.Round(v, 5) == 1.0 && j < dimV)
                            {
                                s2 = 0;
                                k2 = 1;
                            }
                            if (Math.Round(v, 5) == 1.0 && j == dimV)
                            {
                                s2 = 1;
                                k2 = 1;
                            }
                            if (k1 == 0) 
                            {
                                s1 = Cnk(i, dimU) * Bu(u, i) * Bv(u, i, dimU);
                            }
                            if (k2 == 0)
                            {
                                s2 = Cnk(j, dimV) * Bu(v, j) * Bv(v, j, dimV);
                            }
                            Pb.X += (float)(s1 * s2 * P[i * diff + j].X);
                            Pb.Y += (float)(s1 * s2 * P[i * diff + j].Y);
                            Pb.Z += (float)(s1 * s2 * P[i * diff + j].Z);
                            
                        }
                    }
                    P_Bezie.Add(Pb);

                }
            }
            drawarray(P_Bezie, color2);
            pictureBox1.Image = map;
        }


        private double ni(int i,double t)
        {
            switch (i)
            {
                case 0:
                    return (1 - t) * (1 - t) * (1 - t) / 6.0;
                case 1:
                    return (3 * t * t * t - 6 * t * t + 4) / 6.0;
                case 2:
                    return (-3 * t*t*t +3*t*t+3*t+1 ) / 6.0;
                case 3:
                    return (t * t * t) / 6.0;
            }
            return 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < diff - 3; i++)
            {
                for (int j = 0; j < diff - 3; j++)
                {
                    for (double u = 0; Math.Round(u, 5) <= 1.0; u += delta2)
                    {
                        for (double v = 0; Math.Round(v, 5) <= 1.0; v += delta2)
                        {
                            float zero = 0.0f;
                            Vector3 Pb = new Vector3(zero, zero, zero);
                            for(int k = 0; k <= 3; k++)
                            {
                                for (int m = 0; m <= 3; m++)
                                {
                                    Pb.X += (float)(ni(k, u) * ni(m, v) * P[(i + k) * diff + j + m].X);
                                    Pb.Y += (float)(ni(k, u) * ni(m, v) * P[(i + k) * diff + j + m].Y);
                                    Pb.Z += (float)(ni(k, u) * ni(m, v) * P[(i + k) * diff + j + m].Z);
                                }
                            }
                            P_Bsplain2.Add(Pb);
                        }
                    }
                    List<Vector3> temp = P_Bsplain2.ToList();
                    P_splain.Add(temp);
                    P_Bsplain2.Clear();
                }
            }
            
            for(int i=0;i<P_splain.Count;i++)
            {
                drawarray(P_splain[i], color3);
            }
            

            
            
            pictureBox1.Image = map;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int x, y,z;
            double u;
            List<PointF> b1 = new List<PointF>();
            for (int i = 0; i < P.Count - 3; i++)
            {
                for (u = 0.0; u < 1.0; u += 0.01)
                {
                    x = Convert.ToInt32(((double)1 / 6) * u * u * u * P[i + 3].X + (((double)2 / 3) - 0.5 * (u - 1) * (u - 1) * (u - 1) - (u - 1) * (u - 1)) * P[i + 2].X +
                        (((double)2 / 3) + 0.5 * u * u * u - u * u) * P[i + 1].X + (((double)1 / 6) * (1 - u) * (1 - u) * (1 - u)) * P[i].X);
                    y = Convert.ToInt32(((double)1 / 6) * u * u * u * P[i + 3].Y + (((double)2 / 3) - 0.5 * (u - 1) * (u - 1) * (u - 1) - (u - 1) * (u - 1)) * P[i + 2].Y +
                        (((double)2 / 3) + 0.5 * u * u * u - u * u) * P[i + 1].Y + (((double)1 / 6) * (1 - u) * (1 - u) * (1 - u)) * P[i].Y);
                    z = Convert.ToInt32(((double)1 / 6) * u * u * u * P[i + 3].Z + (((double)2 / 3) - 0.5 * (u - 1) * (u - 1) * (u - 1) - (u - 1) * (u - 1)) * P[i + 2].Z +
                        (((double)2 / 3) + 0.5 * u * u * u - u * u) * P[i + 1].Z + (((double)1 / 6) * (1 - u) * (1 - u) * (1 - u)) * P[i].Z);
                    Vector3 b = new Vector3(x, y, z);
                    P_Bsplain3.Add(b);
                }
            }
            b1 = T_2D(P_Bsplain3);
            for (int i = 0; i < b1.Count - 1; i++)
            {
                line(new PointF(b1[i].X + x0, b1[i].Y + y0), new PointF(b1[i + 1].X + x0, b1[i + 1].Y + y0), color4);
            }
            pictureBox1.Image = map;
        }
    }
}
