using System;

namespace ClickOnce
{
    /// <summary>
    /// Base Exception for Validation
    /// </summary>
    public abstract class ValidationException : Exception
    {
        /// <summary>Codebase that refers to assembly</summary>
        public readonly string Codebase;

        /// <summary>Default constructor</summary>
        public ValidationException(string codebase, string message) :
            base(message + " for " + codebase)
        {
            Codebase = codebase;
        }
    }
}
