using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;

namespace BecomeJPEG
{
    internal class JpegCamWindow
    {
        internal bool IsActive { get; set; } = true; //starts active when created.

        private readonly Random rng = new Random();
        private readonly int cameraIndex;

        private Mat frameMatrix;
        private Image<Bgr, byte> frame;
        private VideoCapture capture;

        private long nextFrameMillis;

        private Form form;
        private ImageBox imageBox;

        internal event Action OnBeforeExit;

        internal JpegCamWindow(int cameraIndex)
        {
            this.cameraIndex = cameraIndex;
        }

        internal async Task Run()
        {
            Logger.LogLine("Starting JpegCamWindow");

            form = new Form
            {
                Text = "BecomeJPEG"
            };
            imageBox = new ImageBox
            {
                Size = new System.Drawing.Size(640, 480)
            };
            form.Controls.Add(imageBox);
            form.AutoSize = true;

            form.FormClosing += OnFormClosed;

            form.Show();

            //create the window.
            //CvInvoke.NamedWindow(Settings.windowName);

            //create the videocapture. this will default to the first installed device.
            using (capture = new VideoCapture(cameraIndex))
            {

                //--TODO: capture resolution configurable by user.
                capture.SetCaptureProperty(CapProp.FrameWidth, 640);
                capture.SetCaptureProperty(CapProp.FrameHeight, 360);

                Logger.LogLine($"Capture size: {capture.Width}x{capture.Height}");

                //defining both of these up here avoids a LOT of garbage.
                //frame is required for compressing the data into Jpeg format.
                frame = new Image<Bgr, byte>(capture.Width, capture.Height);
                //the frameMatrix is needed to read the jpeg data into a displayable image again.
                frameMatrix = new Mat(capture.Width, capture.Height, DepthType.Cv8U, 3);

                // first frame should always show immediately.
                nextFrameMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                //subscribe to the ImageGrabbed event. This will follow the device's framerate.
                //capture.ImageGrabbed += OnImageGrabbed;

                //start the image grabbing thread.
                //capture.Start();

                float targetFrameRate = 30f;
                nextFrameMillis += (long) (1000 / targetFrameRate);
                
                while (IsActive)
                {
                    long currentMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    if (nextFrameMillis > currentMillis && (Settings.frameLagRandom | Settings.frameLagTime) != 0)
                        goto loop_end;
                    
                    //chance for frame to be dropped is checked before anything else.
                    if (rng.Next(100) <= Settings.frameDropChance)
                    {
                        //lag is started by skipping frames.
                        nextFrameMillis = currentMillis + (long)(1000 / targetFrameRate) + Settings.frameLagTime + rng.Next(Settings.frameLagRandom);
                        goto loop_end;
                    }
                    nextFrameMillis = currentMillis + (long)(1000 / targetFrameRate);
                    capture.Grab();
                    capture.Retrieve(frame);
                    if(Settings.CompressionQuality == 100)
                    {
                        imageBox.Image = frame;
                    }
                    else
                    {
                        byte[] data = frame.ToJpegData(Settings.CompressionQuality);
                        CvInvoke.Imdecode(data, ImreadModes.Color, frameMatrix);
                        imageBox.Image = frameMatrix;
                    }
                    loop_end:
                    await Task.Delay((int)(1000 / targetFrameRate));
                }

                //clean up anything left to clean up.
                capture.Stop();

                //capture.ImageGrabbed -= OnImageGrabbed;
            }
            Logger.LogLine("Shutting down JpegCamWindow");
            form.Dispose();
            frameMatrix.Dispose();
            frame.Dispose();
            //CvInvoke.DestroyWindow(Settings.windowName);
            //i dont know what im doing
            new CancellationTokenSource().Cancel();
        }

        private void OnFormClosed(object sender, FormClosingEventArgs e)
        {
            OnBeforeExit?.Invoke();
            this.IsActive = false;
        }

        private void OnImageGrabbed(object sender, EventArgs e)
        {
            if (imageBox == null || imageBox.IsDisposed)
                return;
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
                try
                {
                    if (imageBox.IsDisposed == false)
                        imageBox.Image = frame;
                }
                catch(ObjectDisposedException ex)
                {
                    //Logger cannot be used here because it is on a seperate thread.
                    //Logger.LogLine($"ImageBox has been disposed, IsDisposed property was: {imageBox.IsDisposed.ToString()}");
                }
                //CvInvoke.Imshow(Settings.windowName, frame);
                return;
            }

            //jpeg compress the hell out of it.
            //this is the only source of garbage in this event, and I dont think there is a way to avoid it.
            byte[] data = frame.ToJpegData(Settings.CompressionQuality);

            //read the JPEG back into frameMatrix.
            CvInvoke.Imdecode(data, ImreadModes.Color, frameMatrix);
            //show the image.
            //CvInvoke.Imshow(Settings.windowName, frameMatrix);
            if (imageBox.IsDisposed == false)
                 imageBox.Image = frameMatrix;
        }
    }
}
