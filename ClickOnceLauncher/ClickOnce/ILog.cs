using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnce
{
    /// <summary>
    /// Interface for logging, should be able to replace Console
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        void WriteLine(string format);

        /// <summary>
        /// Writes the text representation of the specified object, followed by the current
        ///     line terminator, to the standard output stream using the specified format information.
        /// </summary>
        void WriteLine(string format, params object[] args);

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        void Write(string value);
    }
}
