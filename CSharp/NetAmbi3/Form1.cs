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

/*
 * Netopya Ambient Light Client
 * 
 * This application finds the average RGB colour of 10 locations around the
 * edges of the screen and transmits this information over Serial to an Arduino
 * 
 * more information at: www.netopyaplanet.com
 * */

namespace NetAmbi3
{
    public partial class Form1 : Form
    {
        Panel[] panels = new Panel[10]; // Panels are used to display the colour on the form
        Rectangle[] samples = new Rectangle[10]; // Rectangles are used to easily define the various sample location and sizes

        Graphics screen; // The graphics stream the screenshots will be draw to
        Bitmap screenSurface; // A bitmap of the screen shot
        Rectangle screenSample; // The dimensions of the screen (the screen resolution)

        public Form1()
        {
            InitializeComponent();

            // Define the location of the sample areas around the screen
            // The indexes match those of the RGB LEDs on the back of the monitor
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

            // Define the surface of the entire screen
            screenSurface = new Bitmap(1680, 1050);
            screenSample = new Rectangle(0, 0, 1680, 1050);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // The panels don't match the LEDs since the panels are counted in
            // a clockwise manner from the upper left while the LEDs are placed
            // wherever it was most convient electrically
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

            // Setup the bitmap to draw to
            screen = Graphics.FromImage(screenSurface);

            // Start sampling in a seperate thread to keep the program responsive
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Color[] colors = new Color[10];

            // Initialize the colours
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
                // Capture a screen shot
                screen.CopyFromScreen(screenSample.X, screenSample.Y, 0, 0, screenSample.Size);
                for (int i = 0; i < samples.Length; i++)
                {
                    // For each location, find the average RGB value

                    r = 0;
                    g = 0;
                    b = 0;
                    n = 0;

                    // Skip every second pixel to save processing time
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

                // Display the colour on the form
                panels[0].BackColor = colors[0];

                byte[] buffer = new byte[30];

                // Create the buffer string
                for (int i = 0; i < panels.Length; i++)
                {
                    panels[i].BackColor = colors[i];
                    buffer[i * 3] = colors[i].R;
                    buffer[i * 3 + 1] = colors[i].G;
                    buffer[i * 3 + 2] = colors[i].B;
                }

                // Report the buffer so that it can be displayed
                backgroundWorker1.ReportProgress(0, buffer);

                // Write the buffer to the Arduino
                port.Open();
                port.Write(buffer, 0, buffer.Length);
                port.Close();

                // Pause to save CPU cycles and to allow the Arduino to process the buffer
                Thread.Sleep(1000);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Display the buffer in the textbox
            byte[] buffer = (byte[])e.UserState;
            textBox1.Text = String.Join(",",buffer);
        }  
    }
}
