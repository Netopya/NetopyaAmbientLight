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

        Graphics screen;
        Bitmap screenSurface;
        Rectangle screenSample;

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

            screenSurface = new Bitmap(1680, 1050);
            screenSample = new Rectangle(0, 0, 1680, 1050);
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

            SerialPort port = new SerialPort("COM3", 9600);

            while (true)
            {
                screen.CopyFromScreen(screenSample.X, screenSample.Y, 0, 0, screenSample.Size);
                for (int i = 0; i < samples.Length; i++)
                {
                    r = 0;
                    g = 0;
                    b = 0;
                    n = 0;

                    for (int x = samples[i].X; x < samples[i].X + samples[i].Width; x += 2)
                    {
                        for (int y = samples[i].Y; y < samples[i].Y + samples[i].Height; y += 2)
                        {
                            Color pixel = screenSurface.GetPixel(x, y);
                            r += pixel.R;
                            g += pixel.G;
                            b += pixel.B;
                            n++;
                        }
                    }

                    r /= n;
                    g /= n;
                    b /= n;

                    colors[i] = Color.FromArgb(r, g, b);
                }

                panels[0].BackColor = colors[0];


                byte[] buffer = new byte[30];

                for (int i = 0; i < panels.Length; i++)
                {
                    panels[i].BackColor = colors[i];
                    buffer[i * 3] = colors[i].R;
                    buffer[i * 3 + 1] = colors[i].G;
                    buffer[i * 3 + 2] = colors[i].B;
                }

                backgroundWorker1.ReportProgress(0, buffer);

                port.Open();
                port.Write(buffer, 0, buffer.Length);
                port.Close();

                Thread.Sleep(1000);
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

            screen = Graphics.FromImage(screenSurface);

            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            byte[] buffer = (byte[])e.UserState;
            textBox1.Text = String.Join(",",buffer);
        }  
    }
}
