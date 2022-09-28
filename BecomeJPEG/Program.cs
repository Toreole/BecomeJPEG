using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace BecomeJPEG
{
    internal class Program
    {
        //entry point. you know how it is.
        static void Main()
        {
            Settings.LoadTemplatesFromDrive();

            SettingsPanel mWindow = new SettingsPanel();
            
            //ShowDialog essentially waits until after the form has been closed.
            mWindow.ShowDialog();

            //ensure that everything is closed properly.
            Logger.ShutDown();
        }

    }
}
