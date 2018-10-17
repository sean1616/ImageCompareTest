using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Diagnostics;
using AForge.Imaging;
using AForge.Imaging.Formats;
using AForge.Imaging.Filters;

namespace ImageCompareTest
{
    public partial class Form1 : Form
    {
        Bitmap sourceImage, templateImage;
        Stopwatch stopWatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "所有檔案(*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sourceImage = ImageDecoder.DecodeFromFile(openFileDialog1.FileName);
                pictureBox1.Image = sourceImage;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Crop filter = new Crop(new Rectangle(100, 150, 128, 128));
            templateImage = filter.Apply(sourceImage);
            pictureBox3.Image = templateImage;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            stopWatch.Reset();
            stopWatch.Start();
            // create template matching algorithm's instance
            ExhaustiveTemplateMatching tm = new
            ExhaustiveTemplateMatching(0.99f);
            // find all matchings with specified above similarity


            TemplateMatch[] matchings = tm.ProcessImage(sourceImage, templateImage);
            // highlight found matchings
            BitmapData data = sourceImage.LockBits(
            new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
            ImageLockMode.ReadWrite, sourceImage.PixelFormat);
            foreach (TemplateMatch m in matchings)
            {
                Drawing.Rectangle(data, m.Rectangle, Color.White);

                textBox2.Text = m.Rectangle.Location.ToString();
                // do something else with matching
            }
            sourceImage.UnlockBits(data);
            pictureBox2.Image = sourceImage;
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            textBox1.Text = ts.ToString();
        }

        public static bool ImageCompareString(Bitmap firstImage, Bitmap secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;

            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());



            //MessageBox.Show(firstBitmap.Length.ToString()+"   "+secondBitmap.Length.ToString());

            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(ImageCompareString(templateImage, templateImage).ToString());

            ImageCompareString(sourceImage, templateImage);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
