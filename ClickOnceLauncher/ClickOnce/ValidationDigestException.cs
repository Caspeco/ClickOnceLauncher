

namespace ClickOnce
{
    /// <summary>
    /// Excpetion for size missmatch
    /// </summary>
    public class ValidationDigestException : ValidationException
    {
        /// <summary>The expected size</summary>
        public readonly byte[] Expected;
        /// <summary>The size we got</summary>
        public readonly byte[] Was;

        /// <summary></summary>
        public ValidationDigestException(string codebase, byte[] expected, byte[] was) :
            base(codebase, $"Checksum differs, expected {expected.ToHexLower()} but was {was.ToHexLower()}")
        {
        }
    }
}
