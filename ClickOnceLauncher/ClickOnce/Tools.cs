using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnce
{
    /// <summary>Helper tools</summary>
    public static class Tools
    {
        private static ILog _console;
        /// <summary>
        /// Console logger
        /// </summary>
        public static ILog Console
        {
            get { return _console ?? ClickOnceLauncher.LogConsole.Shared; }
            set { _console = value; }
        }

        /// <summary>Create lowecase hex string from byte array</summary>
        public static string ToHexLower(this byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>Version in hexadecimal ex: 0001.0faf</summary>
        public static string ToHex(this Version ver)
        {
            return ver.Major.ToString("x4") + "." +
                ver.Minor.ToString("x4");
        }

        /// <summary>Checks byte array equal</summary>
        public static bool AreEqual(this byte[] array1, byte[] array2)
        {
            if (array1 == null && array2 == null)
                return true;
            if (array1 == null || array2 == null)
                return false;
            if (array1.Length != array2.Length)
                return false;

            for (int i = 0; i < array1.Length; i++)
                if (array1[i] != array2[i])
                    return false;
            return true;
        }

        internal static HashAlgorithm GetHasher(string hashAlgo)
        {
            if (hashAlgo == "sha256")
                return new SHA256Managed();
            if (hashAlgo == "sha1")
                return new SHA1Managed();
            throw new NotImplementedException("Unknown algo " + hashAlgo);
        }

        private static string GetShortAsmName(string name)
        {
            if (name.Length <= 10)
                return name;
            return name.Substring(0, 4) + ".." +
                name.Substring(name.Length - 4);
        }

        /// <summary>
        /// Get name used for library directory
        /// </summary>
        public static string GetLocalLibName(string asmName, string publicKeyToken, Version version,
            string language, byte[] hash)
        {
            if (version == null)
                return null; //use parent instead

            string res = string.Join("_",
                GetShortAsmName(asmName),
                publicKeyToken,
                version.ToHex()).ToLower();
            if (!asmName.EndsWith(".installation"))
                res = string.Join("_", res, language == null || language == "neutral" ? "none" : language.ToLower());

            // TODO fix the hash, checksum is incorrect
            if (hash == null)
                return res + "_nohash";
            return string.Join("_", res, hash.ToHexLower().Substring(0, 16) , "dev");
        }

        private static string libraryLocation;
        /// <summary>
        /// Get Path for ClickOnce Files
        /// </summary>
        public static string LibraryLocation
        {
            get
            {
                // TODO add handling for config options
                if (string.IsNullOrEmpty(libraryLocation))
                {
                    libraryLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apps", "2.0", "ClickOnce");
                }
                return libraryLocation;
            }
            set
            {
                libraryLocation = value;
                //Directory.CreateDirectory(Path.Combine(libraryLocation, "Manifests"));
            }
        }
    }
}
