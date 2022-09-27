using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BecomeJPEG.src
{
    internal static class Logger
    {
        private static TextBox logTextBox;
        private static FileStream logFileStream;
        private static StreamWriter logWriter;
        private const string logFileName = "log.txt";

        internal static void LogLine(string message)
        {
            if (logTextBox == null)
                return;
            message += '\n'; //append line break character.
            logTextBox.AppendText(message);
            logWriter.Write(message);
            logWriter.Flush();
        }

        internal static void Init(TextBox textBox)
        {
            logTextBox = textBox;
            logFileStream = File.Open(logFileName, FileMode.Create);
            logWriter = new StreamWriter(logFileStream, Encoding.UTF8);
        }

        internal static void ShutDown()
        {
            logTextBox = null;
            logWriter.Dispose();
            logFileStream.Dispose();
        }
    }
}
