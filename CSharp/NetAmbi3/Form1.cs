using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;

namespace NetAmbi3
{
    public partial class Form1 : Form
    {
        Panel[] panels = new Panel[10];

        Rectangle[] samples = new Rectangle[10];
        Bitmap[] surfaces = new Bitmap[10];
        Graphics[] graphics = new Graphics[10];
        Color[] toColors = new Color[10];
        Color[] currentColors = new Color[10];
        SerialPort port; // = new SerialPort("COM3", 9600);

        Graphics screen;


        public Form1()
        {
            InitializeComponent();

            samples[4] = new Rectangle(130, 100, 300, 120);
            samples[5] = new Rectangle(690, 100, 300, 120);
            samples[6] = new Rectangle(1250, 100, 300, 120);
            samples[8] = new Rectangle(1560, 112, 120, 300);
            samples[7] = new Rectangle(1560, 637, 120, 300);
            samples[9] = new Rectangle(1250, 830, 300, 120);
            samples[3] = new Rectangle(690, 830, 300, 120);
            samples[0] = new Rectangle(130, 830, 300, 120);
            samples[1] = new Rectangle(0, 637, 120, 300);
            samples[2] = new Rectangle(0, 112, 120, 300);

            surfaces[4] = new Bitmap(300, 120);
            surfaces[5] = new Bitmap(300, 120);
            surfaces[6] = new Bitmap(300, 120);
            surfaces[8] = new Bitmap(120, 300);
            surfaces[7] = new Bitmap(120, 300);
            surfaces[9] = new Bitmap(300, 120);
            surfaces[3] = new Bitmap(300, 120);
            surfaces[0] = new Bitmap(300, 120);
            surfaces[1] = new Bitmap(120, 300);
            surfaces[2] = new Bitmap(120, 300);

            for (int i = 0; i < currentColors.Length; i++)
            {
                currentColors[i] = Color.Black;
                toColors[i] = Color.Black;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Color[] colors = new Color[10];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color();
            }

            int r = 0;
            int g = 0;
            int b = 0;
            int n = 0;

            while (true)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    graphics[i].CopyFromScreen(samples[i].X, samples[i].Y, 0, 0, samples[i].Size);

                    r = 0;
                    g = 0;
                    b = 0;
                    n = 0;

                    for (int x = 0; x < samples[i].Width; x += 2)
                    {
                        for (int y = 0; y < samples[i].Height; y += 2)
                        {
                            Color pixel = surfaces[i].GetPixel(x, y);
                            r += pixel.R;
                            g += pixel.G;
                            b += pixel.B;
                            n++;
                        }
                    }

                    r /= n;
                    g /= n;
                    b /= n;
                    /*
                    currentColors[i] = Color.FromArgb(
                        incrementToVal(currentColors[i].R, (byte)r, 10),
                        incrementToVal(currentColors[i].G, (byte)g, 10),
                        incrementToVal(currentColors[i].B, (byte)b, 10)
                    );*/

                    colors[i] = Color.FromArgb(r, g, b);
                }

                //backgroundWorker1.ReportProgress(0, colors);


                // here
                panels[0].BackColor = colors[0];


                byte[] buffer = new byte[30];

                for (int i = 0; i < panels.Length; i++)
                {
                    panels[i].BackColor = colors[i];
                    buffer[i * 3] = colors[i].R;// myLerp(0, 200,colors[i].R, 255);
                    buffer[i * 3 + 1] = colors[i].G;
                    buffer[i * 3 + 2] = colors[i].B;
                }
                /*
                buffer = buffer.Select(t => (byte)0).ToArray();

                buffer[0] = 255;// colors[0].R;
                buffer[1] = 0; // colors[0].G;
                buffer[2] = 0;// colors[0].B;
                */
                //textBox1.Text = String.Join(",", buffer);

                SerialPort porta = new SerialPort("COM3", 9600);
                porta.Open();
                porta.Write(buffer, 0, buffer.Length);
                porta.Close();

                Thread.Sleep(100);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panels[4] = panel1;
            panels[5] = panel2;
            panels[6] = panel3;
            panels[8] = panel4;
            panels[7] = panel5;
            panels[9] = panel6;
            panels[3] = panel7;
            panels[0] = panel8;
            panels[1] = panel9;
            panels[2] = panel10;

            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i] = Graphics.FromImage(surfaces[i]);
            }

            backgroundWorker1.RunWorkerAsync();
            //backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //toColors = (Color[])e.UserState;

            //return;
            Color[] colors = (Color[])e.UserState;
            
            panels[0].BackColor = colors[0];


            byte[] buffer = new byte[30];

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].BackColor = colors[i];
                buffer[i * 3] = colors[i].R;// myLerp(0, 200,colors[i].R, 255);
                buffer[i * 3 + 1] = colors[i].G;
                buffer[i * 3 + 2] = colors[i].B;
            }
            /*
            buffer = buffer.Select(t => (byte)0).ToArray();

            buffer[0] = 255;// colors[0].R;
            buffer[1] = 0; // colors[0].G;
            buffer[2] = 0;// colors[0].B;
            */
            textBox1.Text = String.Join(",",buffer);
            
            port.Open();
            port.Write(buffer, 0, buffer.Length);
            port.Close();
        }

        private byte myLerp(float min, float max, float amount, float oldMax)
        {
            return (byte)(min + (max - min) * (amount / oldMax));
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] buffer = new byte[30];

            while (true)
            {
                for (int i = 0; i < currentColors.Length; i++)
                {
                    currentColors[i] = Color.FromArgb(
                        incrementToVal(currentColors[i].R, toColors[i].R, 10),
                        incrementToVal(currentColors[i].G, toColors[i].G, 10),
                        incrementToVal(currentColors[i].B, toColors[i].B, 10)
                    );

                }

                for (int i = 0; i < panels.Length; i++)
                {
                    //panels[i].BackColor = colors[i];
                    buffer[i * 3] = currentColors[i].R;// myLerp(0, 200,colors[i].R, 255);
                    buffer[i * 3 + 1] = currentColors[i].G;
                    buffer[i * 3 + 2] = currentColors[i].B;
                }

                port.Open();
                port.Write(buffer, 0, buffer.Length);
                port.Close();

                backgroundWorker2.ReportProgress(0);

                Thread.Sleep(5);
            }
        }

        private byte incrementToVal(byte currentVal, byte toVal, byte ratio)
        {
            //return (byte)(Math.Ceiling((double)(Math.Abs((currentVal - toVal) / ratio))) * Math.Sign(currentVal - toVal) + currentVal);
            if (Math.Abs(toVal - currentVal) <= ratio)
            {
                return toVal;
            }
            return (byte)(currentVal + Math.Sign(toVal - currentVal)*ratio);
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Color[] colors = (Color[])e.UserState;

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].BackColor = currentColors[i];
            }
        }
    }
}
