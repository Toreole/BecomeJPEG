using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace BecomeJPEG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string windowName = "first window";
            CvInvoke.NamedWindow(windowName);
          
            //create the videocapture. this will default to the first installed device.
            VideoCapture capture = new VideoCapture();

            //subscribe to the ImageGrabbed event.
            capture.ImageGrabbed += new EventHandler(delegate (object sender, EventArgs e)
            {
                Mat frame = new Mat(capture.Width, capture.Height, DepthType.Default, 3);
                capture.Retrieve(frame);
                

                Image<Bgr, byte> img = frame.ToImage<Bgr, byte>();

                //jpeg compress the hell out of it.
                byte[] data = img.ToJpegData(0);
                ////dispose the old image data.
                //img.Dispose();

                CvInvoke.Imdecode(data, ImreadModes.Color, frame);
                //show the image.
                CvInvoke.Imshow(windowName, frame);

                img.Dispose();
                frame.Dispose();
            });

            //start the image grabbing thread.
            capture.Start();

            CvInvoke.WaitKey(0);
            //stop the image grabbing thread.
            capture.Stop();
            CvInvoke.DestroyWindow(windowName);
        }
    }
}
