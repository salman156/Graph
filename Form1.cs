using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button== MouseButtons.Left)
            {
                A.Paths = null;
                A.AddVersh(e.X, e.Y);
                Bitmap MyImage = new Bitmap(pictureBox1.Image);
                var g = Graphics.FromImage(MyImage);
                Rectangle el = new Rectangle(e.X - 20, e.Y - 20, 40, 40);
                g.DrawEllipse(new Pen(Color.Black), el);
                g.DrawString(A.size.ToString(), new Font("Microsoft Sans Serif", 12f, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), e.X-8, e.Y-10);
                pictureBox1.Image = MyImage;
            }
        }
        private Graph A = new GraphList();
        private int X;
        private int Y;
        public static int Rebro;
        private void Form1_Load(object sender, EventArgs e)
        {
            var bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            using (var g = Graphics.FromImage(bmp))
                g.Clear(Color.White);
            pictureBox1.Image = bmp;
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                int x = e.X;
                int y = e.Y;
                if (A.Lies(ref x, ref y))
                {
                    X = x;
                    Y = y;
                }
            }
        }
        private void Interpret(double[] x, int X0, int Y0)
        {
            if (X - X0 >= 0 && Y - Y0 >= 0)
            {
                x[0] = -1; x[1] = -1; x[2] = 1; x[3] = 1;
            }
            else
            if (X - X0 >= 0 && Y - Y0 <= 0)
            {
                x[0] = -1; x[1] = 1; x[2] = 1; x[3] = -1;
            }
            else
            if (X - X0 <= 0 && Y - Y0 <= 0)
            {
                x[0] = 1; x[1] = 1; x[2] = -1; x[3] = -1;
            }
            else
            if (X - X0 <= 0 && Y - Y0 >= 0)
            {
                x[0] = 1; x[1] = -1; x[2] = -1; x[3] = 1;
            }
        }
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int x = e.X;
                int y = e.Y;
                if (A.Lies(ref x, ref y) && X != x && Y != y)
                {
                    Form2 f2 = new Form2();
                    f2.ShowDialog();
                    if (Rebro > 0)
                    {
                        A.Paths = null;
                        A.AddRebro(A.Get(X, Y), A.Get(x, y), Rebro);
                        double a = 1.57079632679;
                        if (X != e.X)
                            a = Math.Atan(Math.Abs((double)(Y - e.Y)) / Math.Abs((double)(X - e.X)));
                        double[] koef = new double[4];
                        Interpret(koef, e.X, e.Y);
                        var bmp = new Bitmap(pictureBox1.Image);
                        var g = Graphics.FromImage(bmp);
                        g.DrawLine(new Pen(Color.Black), new Point((int)(X + koef[0] * 20 * Math.Cos(a)), (int)(Y + koef[1] * 20 * Math.Sin(a))), new Point((int)(x + koef[2] * 20 * Math.Cos(a)), (int)(y + koef[3] * 20 * Math.Sin(a))));
                        g.FillRectangle(Brushes.White, (X + x) / 2 - 10, (Y + y) / 2 - 10, 20, 20);
                        g.DrawRectangle(new Pen(Color.Black), (X + x) / 2 - 10, (Y + y) / 2 - 10, 20, 20);
                        g.DrawString(Rebro.ToString(), new Font("Microsoft Sans Serif", 8f, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), (X + x) / 2 - 8, (Y + y) / 2 - 9);
                        pictureBox1.Image = bmp;
                    }
                    else if (Rebro == 0) 
                    {
                        A.Paths = null;
                        A.AddRebro(A.Get(X, Y), A.Get(x, y), Rebro);
                        PaintGraph();
                    }
                    f2.Dispose();
                }
            }
        }
        private void PaintPath(PathsNode p)
        {
            PaintGraph();
            PathBox.Text = "";
            if (p == null)
            {
                PathBox.Text = "Такого цикла нет";
                return;
            }
            WeightBox.Text = p.weight.ToString();
            var bmp = new Bitmap(pictureBox1.Image);
            var g = Graphics.FromImage(bmp);
            for (int i = p.kpath-1; i >=0 ; i--)
            {
                if (i == 0)
                    PathBox.Text += (p.path[i]+1).ToString();
                else
                    PathBox.Text += (p.path[i]+1).ToString()+ "=>";
                A.GetCoord(p.path[i], out X,out  Y);
                g.FillEllipse(Brushes.White, X-20, Y-20, 40, 40);
                g.DrawEllipse(new Pen(Color.Red), X-20, Y-20, 40, 40);
                g.DrawString((p.path[i] + 1).ToString(), new Font("Microsoft Sans Serif", 12f, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Red), X - 8, Y - 10);
                if(i!=p.kpath-1)
                {
                    int x, y;
                    A.GetCoord(p.path[i+1], out x, out y);
                    double a = 1.57079632679;
                    if (X != x)
                        a = Math.Atan(Math.Abs((double)(Y - y)) / Math.Abs((double)(X - x)));
                    double[] koef=new double[4];
                    Interpret(koef, x, y);
                    g.DrawLine(new Pen(Color.Red), new Point((int)(X + koef[0] * 20 * Math.Cos(a)), (int)(Y + koef[1] * 20 * Math.Sin(a))), new Point((int)(x + koef[2] * 20 * Math.Cos(a)), (int)(y + koef[3] * 20 * Math.Sin(a))));
                    g.FillRectangle(Brushes.White,(X + x) / 2 - 10, (Y + y) / 2 - 10, 20, 20);
                    g.DrawRectangle(new Pen(Color.Red), (X + x) / 2 - 10, (Y + y) / 2 - 10, 20, 20);
                    g.DrawString(A.GetWeight(p.path[i],p.path[i+1]).ToString(), new Font("Microsoft Sans Serif", 8f, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Red), (X + x) / 2 - 8, (Y + y) / 2 - 9);
                }
            }
            pictureBox1.Image = bmp;
        }
        private void PaintGraph()
        {
            var bmp = new Bitmap(pictureBox1.Image);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            for (int i = 0; i < A.size; i++)
            {
                A.GetCoord(i, out X,out Y);
                g.FillEllipse(Brushes.White, X-20, Y-20, 40, 40);
                g.DrawEllipse(new Pen(Color.Black), X-20, Y-20, 40, 40);
                g.DrawString((i + 1).ToString(), new Font("Microsoft Sans Serif", 12f, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), X - 8, Y - 10); 
                for(int j = 0; j < A.size; j++)
                {
                    if (A.GetWeight(i, j) > 0)
                    {
                        int x, y;
                        A.GetCoord(j, out x, out y);
                        double a = 1.57079632679;
                        if (X != x)
                            a = Math.Atan(Math.Abs((double)(Y - y)) / Math.Abs((double)(X - x)));
                        double[] koef = new double[4];
                        Interpret(koef, x, y);
                        g.DrawLine(new Pen(Color.Black), new Point((int)(X + koef[0] * 20 * Math.Cos(a)), (int)(Y + koef[1] * 20 * Math.Sin(a))), new Point((int)(x + koef[2] * 20 * Math.Cos(a)), (int)(y + koef[3] * 20 * Math.Sin(a))));
                        g.FillRectangle(Brushes.White, (X + x) / 2 - 10, (Y + y) / 2 - 10, 20, 20);
                        g.DrawRectangle(new Pen(Color.Black), (X + x) / 2 - 10, (Y + y) / 2 - 10, 20, 20);
                        g.DrawString(A.GetWeight(i, j).ToString(), new Font("Microsoft Sans Serif", 8f, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), (X + x) / 2 - 8, (Y + y) / 2 - 9);
                    }
                }
            }
            pictureBox1.Image = bmp;
        }
        private void Find()
        {
            PathsBox.Text = "";
            A.Paths = null;
            A.DFS(int.Parse(VershVn.Text) - 1, int.Parse(VershVk.Text) - 1, int.Parse(VershA.Text) - 1, int.Parse(VershB.Text) - 1);
            if (A.IsPathFound())
            {
                var path = A.Paths;
                while (path != null)
                {
                    string str = "";
                    for (int i = path.kpath - 1; i > 0; i--)
                        str += (path.path[i]+1).ToString() + "=>";
                    str += (path.path[0]+1).ToString();
                    PathsBox.Text += str;
                    PathsBox.Text += Environment.NewLine;
                    path = path.next;
                }
            }
            else
            {
                PathsBox.Text = "Такого пути нет.";
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Find();
        }
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            PaintGraph();
        }

        private void СохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, false, Encoding.Default))
            {
                sw.WriteLine(A.size);
                for (int i = 0; i < A.size; i++) { 
                    A.GetCoord(i, out X, out Y);
                    sw.WriteLine(X);
                    sw.WriteLine(Y);
                }
                for (int i=0;i<A.size;i++)
                {
                    int k = 0;
                    string weights="";
                    for(int j=0;j<A.size;j++)
                    {
                        if(A.GetWeight(i,j)!=-1)
                        {
                            k++;
                            weights+=(j+1).ToString()+'\n'+A.GetWeight(i, j).ToString()+'\n';
                        }
                    }
                    sw.WriteLine(k);
                    sw.WriteLine(weights);
                }
                sw.Close();
            }
        }

        private void ЗагрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string input,input1;
            A.Clear();
            using (StreamReader sr = new StreamReader(openFileDialog1.FileName,Encoding.Default))
            {
                int size = int.Parse(sr.ReadLine());
                for(int i=0;i<size;i++)
                {
                    input = sr.ReadLine();
                    input1 = sr.ReadLine();
                    A.AddVersh(int.Parse(input), int.Parse(input1));
                }
                for(int i=0;i<A.size;i++)
                {
                    int k = int.Parse(sr.ReadLine());
                    for(int j=0;j<k;j++)
                    {
                        input = sr.ReadLine();
                        input1 = sr.ReadLine();
                        A.AddRebro(i, int.Parse(input)-1, int.Parse(input1));
                    }
                    sr.ReadLine();
                }
                sr.Close();
            }
            PaintGraph();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            A.Clear();
            PathsBox.Text = "";
            PaintGraph();
        }
    }
}
