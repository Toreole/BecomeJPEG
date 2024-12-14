using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace BecomeJPEG
{
    internal class JpegCamWindow
    {
        internal bool IsActive { get; set; } = true; //starts active when created.

        private readonly Random rng = new Random();
        private readonly VideoCapture capture;

        private Mat frameMatrix;
        private Image<Bgr, byte> frame;

        private int bufferSize;
        private ArrayQueue<Image<Bgr, byte>> frameBuffer;
        private int currentBufferFrame = 0;
        private bool isRepeating = false;
        private int currentLoop = 0;
        private int loopCount = 0;
        private long repeatCooldownUntil = 0;

        private long nextFrameMillis;

        private Form form;
        private ImageBox imageBox;

        internal event Action OnBeforeExit;

        internal JpegCamWindow(VideoCapture cap, Resolution res)
        {
            capture = cap;
            //apply resolution to capture
            Logger.LogLine("Trying to set capture size to: " + res.ToString());
            capture.SetCaptureProperty(CapProp.FrameWidth, res.width);
            capture.SetCaptureProperty(CapProp.FrameHeight, res.height);
            bufferSize = Settings.repeatFrameCount;
			frameBuffer = new ArrayQueue<Image<Bgr, byte>>(bufferSize);
        }

        internal async Task Run()
        {
            Logger.LogLine("Starting JpegCamWindow");
            Resolution captureRes = new Resolution(capture.Width, capture.Height);
            Logger.LogLine("Resolution: " + captureRes.ToString());
            form = new Form
            {
                Text = "BecomeJPEG"
            };
            imageBox = new ImageBox
            {
                Size = new System.Drawing.Size(captureRes.width, captureRes.height)
            };
            form.Controls.Add(imageBox);
            form.AutoSize = true;

            form.FormClosing += OnFormClosed;

            form.Show();

            //create the window.
            //CvInvoke.NamedWindow(Settings.windowName);

            //defining both of these up here avoids a LOT of garbage.
            //frame is required for compressing the data into Jpeg format.
            frame = new Image<Bgr, byte>(captureRes.width, captureRes.height);
            //the frameMatrix is needed to read the jpeg data into a displayable image again.
            frameMatrix = new Mat(captureRes.width, captureRes.height, DepthType.Cv8U, 3);

            // first frame should always show immediately.
            nextFrameMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            //setup for the loop timer.
            float targetFrameRate = 30f;
            nextFrameMillis += (long)(1000 / targetFrameRate);

            while (IsActive)
            {
                long currentMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (nextFrameMillis > currentMillis && (Settings.frameLagRandom | Settings.frameLagTime) != 0)
                    goto loop_end; //goto loop_end skips past the part where the frame is grabbed, which works out nicely.

                //chance for frame to be dropped is checked before anything else.
                if (rng.Next(100) <= Settings.frameDropChance)
                {
                    //lag is started by skipping frames.
                    nextFrameMillis = currentMillis + (long)(1000 / targetFrameRate) + Settings.frameLagTime + rng.Next(Settings.frameLagRandom);
                    goto loop_end;
                }
                //next up are repeats
                if (rng.Next(100) <= Settings.repeatChance && !isRepeating && repeatCooldownUntil < currentMillis)
                {
                    isRepeating = true;
                    loopCount = rng.Next(Settings.repeatChain);
                    currentLoop = 0;
                }
                nextFrameMillis = currentMillis + (long)(1000 / targetFrameRate);
                capture.Grab();
                capture.Retrieve(frame);
                UpdateShownFrame(frame, currentMillis);
                EnqueueFrame(frame);
			loop_end:
                await Task.Delay((int)(1000 / targetFrameRate));
            }

            //clean up anything left to clean up.
            //capture.ImageGrabbed -= OnImageGrabbed;

            Logger.LogLine("Shutting down JpegCamWindow");
            form.Dispose();
            frameMatrix.Dispose();
			frame.Dispose();
            capture.Stop();
            //CvInvoke.DestroyWindow(Settings.windowName);
            //i dont know what im doing
            new CancellationTokenSource().Cancel();
        }

        private void UpdateShownFrame(Image<Bgr, byte> frame, long currentMillis)
		{
            //repeats override the frame being used in here.
			if (isRepeating)
			{
                if (currentBufferFrame == bufferSize)
                {
                    currentLoop++;
                    if (currentLoop >= loopCount)
                    {
						isRepeating = false;
                        currentLoop = 0;
						currentBufferFrame = 0;
                        repeatCooldownUntil = currentMillis + (long)Settings.repeatCooldown;
					}
                }
                else if (frameBuffer.Count == Settings.repeatFrameCount) //only do repeat once buffer is full
                {
                    frame.Data = frameBuffer[currentBufferFrame].Data;
                    currentBufferFrame++;
                }
			}

			if (Settings.CompressionQuality == 100)
			{
				imageBox.Image = frame;
			}
			else
			{
				byte[] data = frame.ToJpegData(Settings.CompressionQuality);
				CvInvoke.Imdecode(data, ImreadModes.Color, frameMatrix);
				imageBox.Image = frameMatrix;
			}

		}

        private void OnFormClosed(object sender, FormClosingEventArgs e)
        {
            OnBeforeExit?.Invoke();
            this.IsActive = false;
        }

        private void EnqueueFrame(Image<Bgr, byte> frame)
        {
			if (bufferSize <= 0)
			{
                return;
			}
            // uses buffer
			if (frameBuffer.Count == bufferSize)
			{
				var oldestFrame = frameBuffer.Dequeue();
                oldestFrame.Dispose();
			}
            // this makes copies of previous frames, which needs to be disposed when dequeueing.
			frameBuffer.Enqueue(new Image<Bgr, byte>(frame.Data));
		}
    }
}
