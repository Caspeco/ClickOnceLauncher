

namespace ClickOnce
{
    /// <summary>
    /// Excpetion for size missmatch
    /// </summary>
    public class ValidationSizeException : ValidationException
    {
        /// <summary>The expected size</summary>
        public readonly uint Expected;
        /// <summary>The size we got</summary>
        public readonly uint Was;

        /// <summary></summary>
        public ValidationSizeException(string codebase, uint expected, uint was) :
            base(codebase, $"Size differs, expected {expected} but was {was}")
        {
        }
    }
}
