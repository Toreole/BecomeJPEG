using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace BecomeJPEG
{
    internal class Program
    {
        static Random rng = new Random();
        static volatile float frameDropChance = 0.85f;
        static volatile int compressionQuality = 0;
        static int CompressionQuality 
        { 
            get => compressionQuality; 
            set { 
                compressionQuality = 
                    (value < 0) ? 0 : 
                    (value > 100) ? 100 
                    : value; 
            } }
        static bool quit = false;

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
                if (rng.NextDouble() <= frameDropChance)
                    return;
                //retrieve the grabbed frame data.
                capture.Retrieve(frame);
                
                //jpeg compress the hell out of it.
                byte[] data = frame.ToJpegData(CompressionQuality);
                
                //read the JPEG back into frameMatrix.
                CvInvoke.Imdecode(data, ImreadModes.Color, frameMatrix);

                //show the image.
                CvInvoke.Imshow(windowName, frameMatrix);
            });

            //start the image grabbing thread.
            capture.Start();

            Thread commandThread = new Thread(DoConsoleCommands);
            commandThread.Start();

            while (commandThread.IsAlive)
            {
                CvInvoke.WaitKey(1);
            }

            //stop the image grabbing thread.
            capture.Stop();
            CvInvoke.DestroyWindow(windowName);
        }

        //this is a seperate thread.
        private static void DoConsoleCommands()
        {
            string input = "";
            string[] inputArgs;
            while(true)
            {
                input = Console.ReadLine();
                if (input == null)
                    continue;
                inputArgs = input.Split(' ');

                if (inputArgs.Length <= 0)
                    continue;

                if (string.IsNullOrWhiteSpace(inputArgs[0]))
                    continue;

                if (inputArgs[0].ToLower() == "quit")
                {
                    quit = true;
                    return;
                }

                if (inputArgs[0].ToLower() == "set" && inputArgs.Length == 3)
                {
                    if (inputArgs[1] == "droprate")
                        frameDropChance = float.Parse(inputArgs[2]) / 100f; //divide by 100, so 100 means 100%, 10 means 10%, etc.
                    if (inputArgs[1] == "quality")
                        CompressionQuality = int.Parse(inputArgs[2]);
                }
            }
        }
    }
}
