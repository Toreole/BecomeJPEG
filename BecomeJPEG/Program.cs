using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Threading;

namespace BecomeJPEG
{
    internal class Program
    {
        //settings and and a Random instance that should just be accessible.
        static Random rng = new Random();
        static volatile float frameDropChance = 0.85f;
        static volatile int compressionQuality = 0;
        //readonly windowName.
        static readonly string windowName = "BecomeJPEG Preview";
        //Property that automatically clamps the quality between 0 and 100.
        static int CompressionQuality 
        { 
            get => compressionQuality; 
            set { 
                compressionQuality = 
                    (value < 0) ? 0 : 
                    (value > 100) ? 100 
                    : value; 
            } }

        //entry point. you know how it is.
        static void Main(string[] args)
        {
            //create the window.
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
                //chance for frame to be dropped is checked before anything else.
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

            //input commands are handled on a seperate thread for ease of use.
            Thread commandThread = new Thread(DoConsoleCommands);
            commandThread.Start();

            //when the commandThread stops, thats a sign that the "quit" command has been used.
            while (commandThread.IsAlive)
            {
                CvInvoke.WaitKey(1);
            }

            //clean up anything left to clean up.
            capture.Stop();
            frame.Dispose();
            frameMatrix.Dispose();
            CvInvoke.DestroyWindow(windowName);
        }

        //this is a seperate thread.
        private static void DoConsoleCommands()
        {
            //receive input into these two local variables.
            string input = "";
            string[] inputArgs;
            //only exit point is the "quit" command. 
            while(true)
            {
                //read a line from the console.
                input = Console.ReadLine();
                if (input == null)
                    continue;
                //split the line into individual arguments
                inputArgs = input.Split(' ');

                //shouldnt do anything if input is empty or first argument is blank.
                if (inputArgs.Length <= 0 || string.IsNullOrWhiteSpace(inputArgs[0]))
                    continue;

                //quit command is checked first.
                if (inputArgs[0].ToLower() == "quit")
                {
                    return;
                }

                //set command is next - it requires 3 params.
                if (inputArgs[0].ToLower() == "set" && inputArgs.Length == 3)
                {
                    if (inputArgs[1] == "droprate")
                        frameDropChance = float.Parse(inputArgs[2]) / 100f; //divide by 100, so 100 means 100%, 10 means 10%, etc.
                    if (inputArgs[1] == "quality")
                        CompressionQuality = int.Parse(inputArgs[2]); //can be set directly to Property because it handles Clamping.
                }
            }
        }
    }
}
