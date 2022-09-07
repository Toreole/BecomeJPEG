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

            //defining both of these up here avoids a LOT of garbage.
            //frame is required for compressing the data into Jpeg format.
            Image<Bgr, byte> frame = new Image<Bgr, byte>(capture.Width, capture.Height);
            //the frameMatrix is needed to read the jpeg data into a displayable image again.
            Mat frameMatrix = new Mat(capture.Width, capture.Height, DepthType.Cv8U, 3);

            //subscribe to the ImageGrabbed event. This will follow the device's framerate.
            capture.ImageGrabbed += new EventHandler(delegate (object sender, EventArgs e)
            {
                //retrieve the grabbed frame data.
                capture.Retrieve(frame);
                
                //jpeg compress the hell out of it.
                byte[] data = frame.ToJpegData(0);
                
                //read the JPEG back into frameMatrix.
                CvInvoke.Imdecode(data, ImreadModes.Color, frameMatrix);

                //show the image.
                CvInvoke.Imshow(windowName, frameMatrix);
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
