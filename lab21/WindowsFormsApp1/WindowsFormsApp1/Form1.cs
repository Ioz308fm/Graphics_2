using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        PointF f1, f2;
        List<PointF> vertices = new List<PointF>();
        List<PointF> bezier = new List<PointF>();
        int k = -1, mdown = 0, startfill = 0;
        const int resx = 1920, resy = 1080;
        Bitmap map = new Bitmap(resx, resy);
        Bitmap mapsave = new Bitmap(resx, resy);
        Bitmap mapsave2 = new Bitmap(resx, resy);


        void line(PointF A, PointF B)
        {
            int x1 = Convert.ToInt32(A.X);
            int y1 = Convert.ToInt32(A.Y);
            int x2 = Convert.ToInt32(B.X);
            int y2 = Convert.ToInt32(B.Y);
            int dx = (x2 - x1 >= 0 ? 1 : -1);
            int dy = (y2 - y1 >= 0 ? 1 : -1);

            int lengthX = Math.Abs(x2 - x1);
            int lengthY = Math.Abs(y2 - y1);

            int length = Math.Max(lengthX, lengthY);

            if (length == 0)
            {
                map.SetPixel(x1, y1, Color.Black);
            }

            if (lengthY <= lengthX)
            {
                int x = x1;
                int y = y1;
                int d = -lengthX;

                length++;
                while (length > 0)
                {
                    length--;
                    map.SetPixel(x, y, Color.Black);
                    x += dx;
                    d += 2 * lengthY;
                    if (d > 0)
                    {
                        d -= 2 * lengthX;
                        y += dy;
                    }
                }
            }
            else
            {
                int x = x1;
                int y = y1;
                int d = -lengthY;

                length++;
                while (length > 0)
                {
                    length--;
                    map.SetPixel(x, y, Color.Black);
                    y += dy;
                    d += 2 * lengthX;
                    if (d > 0)
                    {
                        d -= 2 * lengthY;
                        x += dx;
                    }
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (k == -1 && startfill == 0)
            {
                vertices.Add(e.Location);
                k++;
            }
            if (e.Button == MouseButtons.Right && startfill == 0)
            {
                startfill = 1;//РИСОВАТЬ БЕЗЬЕ
            }


            mdown = 1;
        }

        private static int Cominations(int allNumbers, int perGroup)//число сочетаний
        {
            if (allNumbers > 13)
            {
                Console.WriteLine("Too big number!");
                return -1;
            }

            return Factorial(allNumbers) / (Factorial(perGroup) * Factorial(allNumbers - perGroup));
        }

        private static int Factorial(int number)
        {
            int n = 1;

            for (int i = 1; i <= number; i++)
            {
                n *= i;
            }

            return n; 
        }//факториал =)

        private void bezierfunc(List<PointF> points)
        {
            int n = points.Count-1;
            for (double t =0;t<=1;t+=0.001)
            {
                float x=0, y=0;
                if(t==0)
                {
                    x = points[0].X;
                    y = points[0].Y;
                }
                else
                {
                    if (t == 1)
                    {
                        x = points[points.Count - 1].X;
                        y = points[points.Count - 1].Y;
                    }
                    else
                    {
                        for (int i = 0; i <= n; i++)
                        {
                            x += points[i].X * Cominations(n, i) * Convert.ToSingle(Math.Pow(t, i)) * Convert.ToSingle(Math.Pow(1 - t, n - i));
                            y += points[i].Y * Cominations(n, i) * Convert.ToSingle(Math.Pow(t, i)) * Convert.ToSingle(Math.Pow(1 - t, n - i));
                        }
                    }
                }
                map.SetPixel((int)x, (int)y, Color.Red);
            }
        }//алгоритм


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mdown == 1 && startfill == 0 && k != -1)
            {
                Bitmap map1 = new Bitmap(resx, resy);
                map1 = (Bitmap)mapsave.Clone();
                map = map1;
                f2 = e.Location;
                line(vertices[k], f2);
                pictureBox1.Image = map;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mdown == 1 && startfill == 0 && k != -1)
            {
                Bitmap map1 = new Bitmap(resx, resy);
                map1 = (Bitmap)mapsave.Clone();
                map = map1;
                vertices.Add(e.Location);
                line(vertices[k], vertices[k + 1]);
                pictureBox1.Image = map;
                mapsave = map;
                k++;
                mdown = 0;
            }
            if (mdown == 1 && startfill == 1)
            {
                Bitmap map1 = new Bitmap(resx, resy);
                map1 = (Bitmap)mapsave.Clone();
                map = map1;
                pictureBox1.Image = map;
                mapsave = map;
                mdown = 0;
                startfill = 0;
                bezierfunc(vertices);

            }
        }
    }
}
