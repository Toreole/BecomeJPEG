namespace BecomeJPEG
{
    //Note all this stuff is necessary because this project runs on an older version of C#
    //and therefore does not allow top-level-statements yet.
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
