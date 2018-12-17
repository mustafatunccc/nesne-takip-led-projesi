using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.IO.Ports;

namespace first_project_in_image_processing
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection VideoCapTureDevices;   // form1 load içindeki tanımsızlık için yazıldı. Kameradan görüntüalmayla ilgili...
        VideoCaptureDevice videoSource;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.DataSource = SerialPort.GetPortNames();
            VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)

            {

                comboBox1.Items.Add(VideoCaptureDevice.Name);

            }

            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // create video source
            VideoCaptureDevice videoSource = new VideoCaptureDevice(VideoCapTureDevices[0].MonikerString);
            // set NewFrame event handler
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            // start the video source
            videoSource.Start();

        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // video kameradan alınan görüntüler bitmap formatında alınıyor.
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();



            // aynalama yöntemini görüntümüze uyguladık.
            Mirror mirrorfilter = new Mirror(false, true);
            // apply the filter
            mirrorfilter.ApplyInPlace(image);

            // bicubik yöntem ile görüntümüzün boyutlarını belirledik.                                                
            ResizeBicubic filter = new ResizeBicubic(450, 360);
            // apply the filter
            image = filter.Apply(image);

            // aynalama yöntemini görüntümüze uyguladık.
            Mirror mirrorfilterr = new Mirror(false, true);
            // apply the filter
            mirrorfilterr.ApplyInPlace(image1);

            // bicubik yöntem ile görüntümüzün boyutlarını belirledik.                                                
            ResizeBicubic filterr = new ResizeBicubic(450, 360);
            // apply the filter
            image1 = filterr.Apply(image1);


            pictureBox1.Image = image;

            if (radioButton1.Checked)
            {

                // create filter
                EuclideanColorFiltering oklidfilter = new EuclideanColorFiltering();
                // set center colol and radius
                oklidfilter.CenterColor = new RGB(215, 0, 0);
                oklidfilter.Radius = 100;
                // apply the filter
                oklidfilter.ApplyInPlace(image1);
                nesnebul(image1);

            }

            if (radioButton2.Checked)
            {

                // create filter
                EuclideanColorFiltering oklidfilter = new EuclideanColorFiltering();
                // set center colol and radius
                oklidfilter.CenterColor = new RGB(0, 215, 0);
                oklidfilter.Radius = 100;
                // apply the filter
                oklidfilter.ApplyInPlace(image1);
                nesnebul(image1);

            }

            if (radioButton3.Checked)
            {
                // create filter
                EuclideanColorFiltering oklidfilter = new EuclideanColorFiltering();
                // set center color and radius
                oklidfilter.CenterColor = new RGB(Color.FromArgb(30, 144, 255));
                oklidfilter.Radius = 100;
                // apply the filter
                oklidfilter.ApplyInPlace(image1);
                nesnebul(image1);

            }


        }
        public void nesnebul( Bitmap deneme)
        { 
            // Blob counter yaptırılması
            BlobCounter bc = new BlobCounter();
            bc.FilterBlobs = true;
            bc.MinHeight = 5;
            bc.MinWidth = 5;
            bc.ProcessImage(deneme);

            Rectangle[] rects = bc.GetObjectsRectangles();
            


            if (rects.Length > 0)
            {

                Rectangle objectRect = rects[0];
                //Graphics g = Graphics.FromImage(image);
                Graphics g = pictureBox1.CreateGraphics();
                using (Pen pen = new Pen(Color.FromArgb(0, 0, 235), 3))
                {
                    g.DrawRectangle(pen, objectRect);
                }
                //Cizdirilen Dikdörtgenin Koordinatlari aliniyor.
                int objectX = objectRect.X + (objectRect.Width / 2);
                int objectY = objectRect.Y + (objectRect.Height / 2);


                g.Dispose();

                if (objectX <= 150 && objectY <= 120)
                {
                    serialPort1.Write("1");
                }
                else if (objectX > 150 && objectX < 300 && objectY <= 120)
                {
                    serialPort1.Write("2");
                }
                else if (objectX >= 300 && objectY <= 120)
                {
                    serialPort1.Write("3");
                }
                else if (objectX < 150 && objectY > 120 && objectY < 240)
                {
                    serialPort1.Write("4");
                }
                else if (objectX > 150 && objectX < 300 && objectY > 120 && objectY < 240)
                {
                    serialPort1.Write("5");
                }
                else if (objectX > 300 && objectY > 120 && objectY < 240)
                {
                    serialPort1.Write("6");
                }
                else if (objectX < 150 && objectY > 240)
                {
                    serialPort1.Write("7");
                }
                else if (objectX > 150 && objectX < 300 && objectY > 240)
                {
                    serialPort1.Write("8");
                }
                else if (objectX > 300 && objectY > 240)
                {
                    serialPort1.Write("9");
                }

            }
           
            else 
            {
                serialPort1.Write("a");
            }
            
            pictureBox2.Image = deneme;
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            // signal to stop
            videoSource.SignalToStop();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox2.SelectedItem.ToString();
            serialPort1.BaudRate = 9600;
            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
                MessageBox.Show("Port Bağlantısı Yapıldı.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            MessageBox.Show("Port Bağlantısı Kesildi.");
        }
    }
}

