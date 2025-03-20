using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGL;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;

namespace OpenGL_lesson_CSharp
{

    public partial class SharpGLForm : Form
    {

        //[DllImport("kernel32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool AllocConsole();

        float rotation = 0.0f;
        public SharpGLForm()
        {
            //AllocConsole();
            InitializeComponent();
        }

        float delta = 0.001f;
        List<Point3D> vertices = new List<Point3D>();
        List<Point3D> bezierl = new List<Point3D>();
        List<Point3D> b2 = new List<Point3D>();
        List<Point3D> b3 = new List<Point3D>();
        int k = 0,m=0;

        Point3D red = new Point3D(1f, 0.2f, 0.0f);
        Point3D blue = new Point3D(0.2f, 0.0f, 1.0f);
        Point3D green = new Point3D(0.0f, 1.0f, 0.0f);
        Point3D yellow = new Point3D(1.0f, 1.0f, 0.0f);
        Point3D purple = new Point3D(1.0f, 0.2f, 1.0f);
        void bazis()
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(0f, 0f, 0.0f);
            gl.Vertex(0f, 10, 0);
            gl.Vertex(0, 0, 0);
            gl.End();

            gl.Begin(OpenGL.GL_LINES);
            gl.Color(0f, 0f, 0.0f);
            gl.Vertex(0f, 0, 10);
            gl.Vertex(0, 0, 0);
            gl.End();

            gl.Begin(OpenGL.GL_LINES);
            gl.Color(0f, 0f, 0.0f);
            gl.Vertex(10f, 0, 0);
            gl.Vertex(0, 0, 0);
            gl.End();
        }

        void glline(Point3D a, Point3D b,Point3D c)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(c.X, c.Y, c.Z);
            gl.Vertex(a.X, a.Y, a.Z);
            gl.Vertex(b.X, b.Y, b.Z);
            gl.End();
        }

        void addlines(int k)
        {   
            if(k==0)
            {
                Point3D f1 = new Point3D(0.0f, 0.0f, 0.0f);
                Point3D f2 = new Point3D(0.0f, 1.0f, 2.0f);
                Point3D f3 = new Point3D(1.0f, 1.0f, 1.0f);
                Point3D f4 = new Point3D(1.0f, 0.0f, 0.0f);
                vertices.Add(f1);
                vertices.Add(f2);
                vertices.Add(f3);
                vertices.Add(f4);
            }
        }

        void drawfig(List<Point3D> array,Point3D c)
        {
            for (int i = 0; i < array.Count - 1 ; i++) 
            {
                glline(array[i], array[i + 1],c);
            }
        }


       

        private static int Cominations(int allNumbers, int perGroup)
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

            return n; // returns 1 when number is 0
        }

        private void bezierfunc(List<Point3D> points)
        {
            int n = points.Count - 1;
            for (double t = 0; t <= 1; t += 0.001)
            {
                float x = 0, y = 0,z = 0;
                if (t == 0)
                {
                    x = points[0].X;
                    y = points[0].Y;
                    z = points[0].Z;
                }
                else
                {
                    if (t == 1)
                    {
                        x = points[points.Count - 1].X;
                        y = points[points.Count - 1].Y;
                        z = points[points.Count - 1].Z;
                    }
                    else
                    {
                        for (int i = 0; i <= n; i++)
                        {
                            x += points[i].X * Cominations(n, i) * Convert.ToSingle(Math.Pow(t, i)) * Convert.ToSingle(Math.Pow(1 - t, n - i));
                            y += points[i].Y * Cominations(n, i) * Convert.ToSingle(Math.Pow(t, i)) * Convert.ToSingle(Math.Pow(1 - t, n - i));
                            z += points[i].Z * Cominations(n, i) * Convert.ToSingle(Math.Pow(t, i)) * Convert.ToSingle(Math.Pow(1 - t, n - i));
                        }
                    }
                }
                Point3D np = new Point3D(x, y, z);
                bezierl.Add(np);
            }
        }

        void bspl2(List<Point3D> P)
        {
            float oldX, oldY, oldZ, newX, newY, newZ;
            double u;
            int i = 0;
            oldX = (P[i].X + P[i + 1].X) / 2;
            oldY = (P[i].Y + P[i + 1].Y) / 2;
            oldZ = (P[i].Z + P[i + 1].Z) / 2;
            for (i = 0; i < P.Count - 2; i++)
            {
                for (u = 0.0; u < 1.0; u += 0.01)
                {
                    newX = Convert.ToSingle(0.5 * (1 - u) * (1 - u) * P[i].X + (0.75 - (u - 0.5) * (u - 0.5)) * P[i + 1].X + 0.5 * u * u * P[i + 2].X);
                    newY = Convert.ToSingle(0.5 * (1 - u) * (1 - u) * P[i].Y + (0.75 - (u - 0.5) * (u - 0.5)) * P[i + 1].Y + 0.5 * u * u * P[i + 2].Y);
                    newZ = Convert.ToSingle(0.5 * (1 - u) * (1 - u) * P[i].Z + (0.75 - (u - 0.5) * (u - 0.5)) * P[i + 1].Z + 0.5 * u * u * P[i + 2].Z);
                    Point3D tmp = new Point3D(newX, newY, newZ);
                    b2.Add(tmp);
                }
            }
        }

        void bspl3(List<Point3D> P)
        {
            float oldX, oldY, oldZ, newX, newY, newZ;
            double u;
            int i = 0;
            oldX = (P[i].X + P[i + 1].X) / 2;
            oldY = (P[i].Y + P[i + 1].Y) / 2;
            oldZ = (P[i].Z + P[i + 1].Z) / 2;
            for (i = 0; i < P.Count - 3; i++)
            {
                for (u = 0.0; u < 1.0; u += 0.01)
                {
                    newX = Convert.ToSingle(((double)1 / 6) * u * u * u * P[i + 3].X + (((double)2 / 3) - 0.5 * (u - 1) * (u - 1) * (u - 1) - (u - 1) * (u - 1)) * P[i + 2].X +
                        (((double)2 / 3) + 0.5 * u * u * u - u * u) * P[i + 1].X + (((double)1 / 6) * (1 - u) * (1 - u) * (1 - u)) * P[i].X);
                    newY = Convert.ToSingle(((double)1 / 6) * u * u * u * P[i + 3].Y + (((double)2 / 3) - 0.5 * (u - 1) * (u - 1) * (u - 1) - (u - 1) * (u - 1)) * P[i + 2].Y +
                        (((double)2 / 3) + 0.5 * u * u * u - u * u) * P[i + 1].Y + (((double)1 / 6) * (1 - u) * (1 - u) * (1 - u)) * P[i].Y);
                    newZ = Convert.ToSingle(((double)1 / 6) * u * u * u * P[i + 3].Z + (((double)2 / 3) - 0.5 * (u - 1) * (u - 1) * (u - 1) - (u - 1) * (u - 1)) * P[i + 2].Z +
                        (((double)2 / 3) + 0.5 * u * u * u - u * u) * P[i + 1].Z + (((double)1 / 6) * (1 - u) * (1 - u) * (1 - u)) * P[i].Z);
                    Point3D tmp = new Point3D(newX, newY, newZ);
                    b3.Add(tmp);
                }
            }
        }

        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
           //  Возьмём OpenGL объект
            OpenGL gl = openGLControl.OpenGL;

            //  Очищаем буфер цвета и глубины
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            
            //  Загружаем единичную матрицу
            gl.LoadIdentity();

            //  Указываем оси вращения (x, y, z)
            gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

            
            addlines(k);
            k += 1;
            drawfig(vertices,red);
            bazis();
            
            drawfig(bezierl,blue);
            drawfig(b2, purple);
            drawfig(b3, green);

            rotation += 1.5f;
        }

        // Эту функцию используем для задания некоторых значений по умолчанию
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
           //  Возьмём OpenGL объект
            OpenGL gl = openGLControl.OpenGL;

            //  Фоновый цвет по умолчанию (в данном случае цвет голубой)
            gl.ClearColor(1f, 1f, 1.0f, 0);
        }

        // Данная функция используется для преобразования изображения 
        // в объёмный вид с перспективой
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //  Возьмём OpenGL объект
            OpenGL gl = openGLControl.OpenGL;

            //  Зададим матрицу проекции
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Единичная матрица для последующих преобразований
            gl.LoadIdentity();

            //  Преобразование
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            //  Данная функция позволяет установить камеру и её положение
            gl.LookAt( 5, 6, -7,    // Позиция самой камеры
                       0, 1, 0,     // Направление, куда мы смотрим
                       0, 1, 0);    // Верх камеры

            //  Зададим модель отображения
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void openGLControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Left)
            {
                bezierfunc(vertices);
            }
            if (e.Button == MouseButtons.Right)
            {
                bspl2(vertices);
            }
            if (e.Button == MouseButtons.Middle)
            {
                bspl3(vertices);
            }
        }
    }
}