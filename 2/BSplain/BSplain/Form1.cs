using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSplain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static void thick_dot(int x, int y, string color)
        {
            map.SetPixel(x, y, Color.FromName(color));
            map.SetPixel(x + 1, y, Color.FromName(color));
            map.SetPixel(x, y + 1, Color.FromName(color));
            map.SetPixel(x + 1, y + 1, Color.FromName(color));
            map.SetPixel(x + 1, y - 1, Color.FromName(color));
            map.SetPixel(x, y - 1, Color.FromName(color));
            map.SetPixel(x - 1, y - 1, Color.FromName(color));
            map.SetPixel(x - 1, y, Color.FromName(color));
            map.SetPixel(x - 1, y + 1, Color.FromName(color));

        }
        const int resx = 1920, resy = 1080;
        public static Bitmap map = new Bitmap(resx,resy);

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image= map;
        }
        public List<Point> P = new List<Point>();
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            P.Add(new Point(x, y));
            thick_dot(x, y, "Black");
            pictureBox1.Image = map;
        }
        void bspl2()
        {
            int oldX, oldY, newX, newY;
            double u;
            int i = 0;
            oldX = (P[i].X + P[i + 1].X) / 2;
            oldY = (P[i].Y + P[i + 1].Y) / 2;
            for (i = 0; i < P.Count - 2; i++)
            {
                for (u = 0.0; u < 1.0; u += 0.01)
                {
                    newX = Convert.ToInt32(0.5 * (1 - u) * (1 - u) * P[i].X + (0.75 - (u - 0.5) * (u - 0.5)) * P[i+1].X + 0.5 * u * u * P[i+2].X);
                    newY = Convert.ToInt32(0.5 * (1 - u) * (1 - u) * P[i].Y + (0.75 - (u - 0.5) * (u - 0.5)) * P[i+1].Y + 0.5 * u * u * P[i+2].Y);
                    thick_dot(newX, newY, "Red");
                }
            }
        }
        void bspl3()
        {
            int oldX, oldY, newX, newY;
            double u;
            int i = 0;
            oldX = (P[i].X + P[i + 1].X) / 2;
            oldY = (P[i].Y + P[i + 1].Y) / 2;
            for (i = 0; i < P.Count - 3; i++)
            {
                for (u = 0.0; u < 1.0; u += 0.01)
                {
                    newX = Convert.ToInt32(((double)1 / 6) * u * u * u * (double)P[i + 3].X + (((double)2 / 3) - 0.5 * (u - 1) * (u - 1) * (u - 1) - (u - 1) * (u - 1)) * P[i+2].X+
                        (((double)2 / 3) + 0.5 * u * u * u - u * u) * P[i + 1].X + (((double)1 / 6) * (1 - u) * (1 - u) * (1 - u)) * P[i].X);
                    newY = Convert.ToInt32(((double)1 / 6) * u * u * u * P[i + 3].Y + (((double)2 / 3) - 0.5 * (u - 1) * (u - 1) * (u - 1) - (u - 1) * (u - 1)) * P[i + 2].Y +
                        (((double)2 / 3) + 0.5 * u * u * u - u * u) * P[i + 1].Y + (((double)1 / 6) * (1 - u) * (1 - u) * (1 - u)) * P[i].Y);
                    thick_dot(newX, newY, "Blue");
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            bspl2();
            pictureBox1.Image = map;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bspl3();
            pictureBox1.Image = map;
        }
    }

}
