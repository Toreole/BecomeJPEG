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

namespace BecomeJPEG
{
    internal class Program
    {
        private const string templateFile = "templates.txt";

        //settings and and a Random instance that should just be accessible.
        static Random rng = new Random();
        static float frameDropChance = 0.85f;
        static int compressionQuality = 0;
        static int frameLagTime = 100;
        static int frameLagRandom = 400;
        //readonly windowName.
        static readonly string windowName = "BecomeJPEG Preview";

        static readonly Encoding encoder = Encoding.UTF8;

        //list of templates.
        static List<QualityTemplate> templates = null;

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
            //Initialize the template list.
            LoadTemplatesFromDrive();

            DsDevice[] devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            Console.WriteLine("Video Input Devices found:");
            foreach(var device in devices)
            {
                Console.WriteLine(device.Name + " - " +  device.GetPropBagValue("DevicePath"));
            }
            Console.WriteLine("---------");

            Console.ReadLine();
            return; //TEMPORARY RETURN; DONT WANT ALL THAT OTHER STUFF RIGHT NOW

            //create the window.
            CvInvoke.NamedWindow(windowName);

            //create the videocapture. this will default to the first installed device.
            VideoCapture capture = new VideoCapture();
            
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
                if (nextFrameMillis > currentFrameMillis && (frameLagRandom | frameLagTime) != 0) 
                    return;
                //chance for frame to be dropped is checked before anything else.
                if (rng.NextDouble() <= frameDropChance)
                {
                    //lag is started by skipping frames.
                    nextFrameMillis = currentFrameMillis + frameLagTime + rng.Next(frameLagRandom);
                    return;
                }
                //retrieve the grabbed frame data.
                capture.Retrieve(frame);

                //if quality is 100 (aka no compression, just show the frame directly.)
                if (CompressionQuality == 100)
                {
                    CvInvoke.Imshow(windowName, frame);
                    return;
                }

                //jpeg compress the hell out of it.
                //this is the only source of garbage in this event, and I dont think there is a way to avoid it.
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

        private static void LoadTemplatesFromDrive()
        {
            if (File.Exists(templateFile) == false)
            {
                //default templates.
                templates = new List<QualityTemplate>()
                {
                    new QualityTemplate("high", 0, 100, 0, 0),
                    new QualityTemplate("worst", 0.5f, 0, 100, 100),
                    new QualityTemplate("medium", 0.2f, 4, 10, 20)
                };
            }
            else
            {
                FileStream stream = File.OpenRead(templateFile);
                byte[] buffer = new byte[stream.Length];
                //read the entire file.
                stream.Read(buffer, 0, (int)stream.Length);
                //immediately dispose the stream.
                stream.Flush();
                stream.Dispose();
                string data = encoder.GetString(buffer);
                string[] entries = data.Split('\n');
                //initialize templates to an appropriate length.
                templates = new List<QualityTemplate>(entries.Length);
                //add all entries as QualityTemplates.
                foreach (var s in entries)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    templates.Add(QualityTemplate.FromString(s));
                }
            }
        }

        //store templates in a file.
        private static void SaveTemplatesToDrive()
        {
            //always create a new file or replace the contents of the previous one.
            FileStream stream = File.Open(templateFile, FileMode.Create);
            for(int i = 0; i < templates.Count; i++)
            {
                byte[] buffer = encoder.GetBytes(templates[i].ToString() + '\n');
                stream.Write(buffer, 0, buffer.Length);
            }
            stream.Flush();
            stream.Dispose();
        }

        //this is a seperate thread.
        private static void DoConsoleCommands()
        {
            //receive input into these two local variables.
            string input = null;
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

                //convert all arguments to lower.
                for (int i = 0; i < inputArgs.Length; ++i)
                    inputArgs[i] = inputArgs[i].ToLower();

                //shouldnt do anything if input is empty or first argument is blank.
                if (inputArgs.Length <= 0 || string.IsNullOrWhiteSpace(inputArgs[0]))
                    continue;

                //quit command is checked first.
                if (inputArgs[0] == "quit")
                {
                    return;
                }

                //stoplag command - just to break out of long lag times when you set them before.
                if (inputArgs[0] == "stoplag")
                {
                    frameLagRandom = 0;
                    frameLagTime = 0;
                    continue;
                }

                //set command is next - it requires 3 params.
                if (inputArgs[0] == "set" && inputArgs.Length == 3)
                {
                    if (inputArgs[1] == "droprate")
                        frameDropChance = float.Parse(inputArgs[2]) / 100f; //divide by 100, so 100 means 100%, 10 means 10%, etc.
                    else if (inputArgs[1] == "quality")
                        CompressionQuality = int.Parse(inputArgs[2]); //can be set directly to Property because it handles Clamping.
                    else if (inputArgs[1] == "lagtime")
                        frameLagTime = int.Parse(inputArgs[2]);
                    else if (inputArgs[1] == "lagrandom")
                        frameLagRandom = int.Parse(inputArgs[2]);
                    continue;
                }

                //template command - requires 3 params.
                if (inputArgs[0] == "template" && inputArgs.Length >= 2)
                {
                    if (inputArgs.Length == 3)
                    {
                        if (inputArgs[1] == "apply")
                            ApplyTemplate(inputArgs[2]);
                        else if (inputArgs[1] == "save")
                            SaveTemplate(inputArgs[2]);
                        else if (inputArgs[1] == "delete")
                            DeleteTemplate(inputArgs[2]);
                    }
                    else if (inputArgs.Length == 2)
                    {
                        if (inputArgs[1] == "list")
                            ListTemplates();
                    }
                    continue;
                }
            }
        }

        private static void ListTemplates()
        {
            if(templates.Count == 0)
            {
                Console.WriteLine("- There are no templates.");
            }
            else
            {
                Console.WriteLine("Available templates:");
                foreach (var template in templates)
                    Console.WriteLine($"- {template.templateName}");
            }
        }

        //saves a template with the provided name. if a template with that name already exists, override it.
        private static void SaveTemplate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            QualityTemplate template = templates.Find(x => x.templateName == name);
            if (template == null)
            {
                template = new QualityTemplate(name);
                templates.Add(template);
            }
            template.frameDropChance = frameDropChance;
            template.frameLagRandom = frameLagRandom;
            template.frameLagTime = frameLagTime;
            template.compressionQuality = CompressionQuality;
            //write templates to a file.
            SaveTemplatesToDrive();
        }

        //applies a template by name.
        private static void ApplyTemplate(string name)
        {
            QualityTemplate foundTemplate = templates.Find(x => x.templateName == name);
            if(foundTemplate != null)
            {
                CompressionQuality = foundTemplate.compressionQuality;
                frameLagRandom = foundTemplate.frameLagRandom;
                frameLagTime = foundTemplate.frameLagTime;
                frameDropChance = foundTemplate.frameDropChance;
            }
        }

        //Removes a named template if it exists.
        private static void DeleteTemplate(string name)
        {
            QualityTemplate foundTemplate = templates.Find(x => x.templateName == name);
            if (foundTemplate != null)
            {
                templates.Remove(foundTemplate);
                //write templates to a file.
                SaveTemplatesToDrive();
            }
        }
    }
}
