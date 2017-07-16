using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnceLauncher
{
    /// <summary>
    /// Implementation that logs to Console and Debug channels
    /// </summary>
    public class LogConsole : ClickOnce.ILog
    {
        /// <summary>
        /// Singleton of instance
        /// </summary>
        public static readonly LogConsole Shared = new LogConsole();
        /// <summary>
        /// Internal log
        /// </summary>
        public readonly StringBuilder Log = new StringBuilder();

        /// <summary>
        /// Obtains the next character or function key pressed by the user. The pressed key is displayed in the console window.
        /// </summary>
        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        public void WriteLine(string format)
        {
            if (Log.Length > 1024 * 1024)
            {
                ClearLog();
                WriteLine("Logg cleared due to size.");
            }
            string ts = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss ");
            Log.AppendLine(ts + format);
            Console.WriteLine(format);
            System.Diagnostics.Debug.WriteLine(ts + format);
        }

        /// <summary>
        /// Writes the text representation of the specified object, followed by the current
        ///     line terminator, to the standard output stream using the specified format information.
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            WriteLine(String.Format(format, args));
        }
        internal void WriteLine(int value)
        {
            WriteLine(value.ToString());
        }
        internal void WriteLine(Exception value)
        {
            WriteLine(value.ToString());
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        public void Write(string value)
        {
            if (value == null) throw new ArgumentNullException("value"); // TODO Use Code Contracts

            if (value != ".") Log.Append(value);
            Console.Write(value);
        }

        /// <summary>
        /// Clears internal log
        /// </summary>
        public void ClearLog()
        {
            Log.Length = 0;
        }
    }
}
