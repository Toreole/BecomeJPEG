using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Threading;
using System.IO;
using System.Collections.Generic;

using Encoding = System.Text.Encoding;
using Emgu.CV.Util;

//from the DirectShowLib-2005.dll
using DirectShowLib;
using BecomeJPEG.src;

namespace BecomeJPEG
{
    internal class Program
    {
        //entry point. you know how it is.
        static void Main(string[] args)
        {
            Settings.LoadTemplatesFromDrive();

            SettingsPanel mWindow = new SettingsPanel();
            mWindow.Enabled = true;
            mWindow.ShowDialog();

            //ensure that everything is closed properly.
            Logger.ShutDown();

            return; //VERY TEMPORARY

            DsDevice[] devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            //--TODO: Select device.
            Console.WriteLine("Video Input Devices found:");
            foreach(var device in devices)
            {
                Console.WriteLine(device.Name);
            }
            Console.WriteLine("---------");

            Console.ReadLine();
            return; //TEMPORARY RETURN; DONT WANT ALL THAT OTHER STUFF RIGHT NOW

            Random rng = new Random();

            //create the window.
            CvInvoke.NamedWindow(Settings.windowName);

            //create the videocapture. this will default to the first installed device.
            VideoCapture capture = new VideoCapture();
            
            //--TODO: capture resolution configurable by user.
            capture.SetCaptureProperty(CapProp.FrameWidth, 640);
            capture.SetCaptureProperty(CapProp.FrameHeight, 360);

            Console.WriteLine($"Capture size: {capture.Width}x{capture.Height}");

            //defining both of these up here avoids a LOT of garbage.
            //frame is required for compressing the data into Jpeg format.
            Image<Bgr, byte> frame = new Image<Bgr, byte>(capture.Width, capture.Height);
            //the frameMatrix is needed to read the jpeg data into a displayable image again.
            Mat frameMatrix = new Mat(capture.Width, capture.Height, DepthType.Cv8U, 3);

            // first frame should always show immediately.
            long nextFrameMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            //subscribe to the ImageGrabbed event. This will follow the device's framerate.
            capture.ImageGrabbed += new EventHandler(delegate (object sender, EventArgs e)
            {
                long currentFrameMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                
                //skip frame if currently in "lag" time.    -when the lagtimes have been set to 0, this should be skipped.
                if (nextFrameMillis > currentFrameMillis && (Settings.frameLagRandom | Settings.frameLagTime) != 0) 
                    return;
                //chance for frame to be dropped is checked before anything else.
                if (rng.NextDouble() <= Settings.frameDropChance)
                {
                    //lag is started by skipping frames.
                    nextFrameMillis = currentFrameMillis + Settings.frameLagTime + rng.Next(Settings.frameLagRandom);
                    return;
                }
                //retrieve the grabbed frame data.
                capture.Retrieve(frame);

                //if quality is 100 (aka no compression, just show the frame directly.)
                if (Settings.CompressionQuality == 100)
                {
                    CvInvoke.Imshow(Settings.windowName, frame);
                    return;
                }

                //jpeg compress the hell out of it.
                //this is the only source of garbage in this event, and I dont think there is a way to avoid it.
                byte[] data = frame.ToJpegData(Settings.CompressionQuality);

                //read the JPEG back into frameMatrix.
                CvInvoke.Imdecode(data, ImreadModes.Color, frameMatrix);
                //show the image.
                CvInvoke.Imshow(Settings.windowName, frameMatrix);

            });

            //start the image grabbing thread.
            capture.Start();


            //clean up anything left to clean up.
            capture.Stop();
            frame.Dispose();
            frameMatrix.Dispose();
            CvInvoke.DestroyWindow(Settings.windowName);
        }

    }
}
