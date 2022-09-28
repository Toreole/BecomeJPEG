using System;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace BecomeJPEG
{
    internal class JpegCamWindow
    {
        internal bool IsActive { get; set; } = true; //starts active when created.

        private readonly Random rng = new Random();
        private int cameraIndex;

        internal JpegCamWindow(int cameraIndex)
        {
            this.cameraIndex = cameraIndex;
        }

        internal async Task Run()
        {
            Logger.LogLine("Starting JpegWindow");

            //create the window.
            CvInvoke.NamedWindow(Settings.windowName);

            //create the videocapture. this will default to the first installed device.
            VideoCapture capture = new VideoCapture(cameraIndex);

            //--TODO: capture resolution configurable by user.
            capture.SetCaptureProperty(CapProp.FrameWidth, 640);
            capture.SetCaptureProperty(CapProp.FrameHeight, 360);

            Logger.LogLine($"Capture size: {capture.Width}x{capture.Height}");

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
                if (rng.Next(100) <= Settings.frameDropChance)
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

            while (IsActive)
                await Task.Delay(2);

            //clean up anything left to clean up.
            capture.Stop();
            capture.Dispose();
            frame.Dispose();
            frameMatrix.Dispose();
            CvInvoke.DestroyWindow(Settings.windowName);
            //i dont know what im doing
            new CancellationTokenSource().Cancel();
        }
    }
}
