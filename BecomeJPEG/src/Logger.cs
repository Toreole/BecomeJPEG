using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BecomeJPEG
{
    internal static class Logger
    {
        private static TextBox logTextBox;
        private static FileStream logFileStream;
        private static StreamWriter logWriter;
        private const string logFileName = "log.txt";

        /// <summary>
        /// Appends the message to the GUI log box and adds it to the log.txt file.
        /// </summary>
        /// <param name="message"></param>
        internal static void LogLine(string message)
        {
            if (logTextBox == null)
                return;
            message += '\n'; //append line break character.
            logTextBox.AppendText(message);
            //not sure if theres a need to lock the logWriter, because there are two threads potentially accessing it...
            //lock (logWriter)
            //{
                logWriter.Write(message);
                logWriter.Flush();
            //}
        }

        /// <summary>
        /// Initialize the Logger.
        /// </summary>
        /// <param name="textBox">The GUI log textBox</param>
        internal static void Init(TextBox textBox)
        {
            logTextBox = textBox;
            logFileStream = File.Open(logFileName, FileMode.Create);
            logWriter = new StreamWriter(logFileStream, Encoding.UTF8);
        }

        /// <summary>
        /// Shutdown the Logger, disposing of the streams.
        /// </summary>
        internal static void ShutDown()
        {
            logTextBox = null;
            logWriter.Dispose();
            logFileStream.Dispose();
        }
    }
}
