using System;

namespace ClickOnce
{
    /// <remarks/>
    public class EntryPoint
    {
        /// <remarks/>
        public string Publisher;

        /// <summary>
        /// Product
        /// </summary>
        public string Product;

        /// <summary>
        /// Identity Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Identity Version
        /// </summary>
        public Version Version;

        /// <summary>
        /// Specifies a 16-character hexadecimal string that represents the last 8 bytes of the SHA-1 hash of the public key under which the application or assembly is signed. The public key used to sign must be 2048 bits or greater
        /// </summary>
        public string PublicKeyToken;

        /// <summary>
        /// Identity Language
        /// </summary>
        public string Language;

        /// <summary>
        /// Identity ProcessorArchitecture
        /// </summary>
        public string Architecture;

        /// <remarks/>
        public string Executable;
        /// <remarks/>
        public string Parameters;
        /// <remarks/>
        public string Icon;
        /// <remarks/>
        public string SupportUrl;

        /// <remarks/>
        public string LocalPath;

        private static readonly System.Reflection.FieldInfo[] fields = typeof(EntryPoint).GetFields();

        /// <summary>
        /// Get name used for library directory
        /// </summary>
        public string LocalLibName()
        {
            return Tools.GetLocalLibName(Name + ".installation", PublicKeyToken, Version, Language, null);
        }

        /// <remarks/>
        public void Import(EntryPoint child)
        {
            // automatic propagation of public fields
            foreach (var fi in fields)
            {
                var v = fi.GetValue(this);
                if (v == null ||
                    (fi.FieldType == typeof(string) && string.IsNullOrWhiteSpace((string)v)))
                    fi.SetValue(this, fi.GetValue(child));
            }
        }
    }
}
