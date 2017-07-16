using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ClickOnce
{
    /// <remarks/>
    public enum DependencyType
    {
        /// <summary>dependency Component represents a separate installation from the current application</summary>
        install,
        /// <summary>dependency Component is required by the current application</summary>
        preRequisite,
        /// <summary>file possible resource</summary>
        file,
    }

    /// <remarks/>
    public partial class Dependency
    {
        /// <remarks/>
        public readonly DependencyType DependencyType;

        /// <summary>
        /// Can be installed in parallel, most likely <see cref="DependencyType.preRequisite"/>
        /// </summary>
        public readonly bool? AllowDelayedBinding;

        /// <summary>
        /// Base Filename
        /// </summary>
        public readonly string Codebase;

        /// <summary>
        /// Size of basefile (expected)
        /// </summary>
        public readonly uint Size;

        /// <summary>
        /// Identity Name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Identity Version
        /// </summary>
        public readonly Version Version;

        /// <summary>
        /// Specifies a 16-character hexadecimal string that represents the last 8 bytes of the SHA-1 hash of the public key under which the application or assembly is signed. The public key used to sign must be 2048 bits or greater
        /// </summary>
        public readonly string PublicKeyToken;

        /// <summary>
        /// Identity Language
        /// </summary>
        public readonly string Language;

        /// <summary>
        /// Identity ProcessorArchitecture
        /// </summary>
        public readonly string Architecture;

        /// <summary>
        /// Identity Type (win32)
        /// </summary>
        public readonly string ManifestType;

        private readonly string HashAlgo;
        private readonly byte[] HashValue;

        private readonly string BaseLocation;
        private readonly bool MapFileExtensions;

        private static string GetUrlFolder(string url)
        {
            if (String.IsNullOrEmpty(url))
                return url;
            url = new Uri(url).ToString();
            return url.Substring(0, url.LastIndexOf('/'));
        }

        private static string FixToPathSeperator(string path)
        {
            if (String.IsNullOrEmpty(path))
                return path;
            foreach (char sep in new char[] { '/', '\\' })
                if (sep != Path.DirectorySeparatorChar)
                    path = path.Replace(sep, Path.DirectorySeparatorChar);
            return path;
        }

        private Dependency(Manifest baseManifest = null)
        {
            if (baseManifest == null)
                return;

            BaseLocation =
                GetUrlFolder(
                    baseManifest.DeploymentProviderCodebase ??
                    baseManifest.Location);
            MapFileExtensions = !baseManifest.Install.HasValue &&
                baseManifest.MapFileExtensions;
        }

        /// <summary>
        /// Creates new instance from <see cref="Asmv1.dependency"/>
        /// </summary>
        /// <remarks>
        /// https://docs.microsoft.com/en-us/visualstudio/deployment/dependency-element-clickonce-application
        /// Older object with some more data: https://msdn.microsoft.com/en-us/library/k26e96zf.aspx
        /// </remarks>
        public Dependency(Asmv1.dependency dep, Manifest baseManifest = null) :
            this(baseManifest)
        {
            var depAsm = dep.dependentAssembly;
            DependencyType = (DependencyType)Enum.Parse(typeof(DependencyType), depAsm.dependencyType);
            AllowDelayedBinding = depAsm.allowDelayedBindingSpecified ?
                depAsm.allowDelayedBinding : (bool?)null;
            Codebase = depAsm.codebase;
            if (depAsm.sizeSpecified) Size = depAsm.size;

            var ident = depAsm.assemblyIdentity;
            Name = ident.name;
            Version = new Version(ident.version);
            PublicKeyToken = ident.publicKeyToken;
            Language = ident.language;
            Architecture = ident.processorArchitecture;
            ManifestType = ident.type;

            if (depAsm.hash != null)  // Optional
                ParseHash(depAsm.hash, ref HashAlgo, ref HashValue);
        }

        /// <summary>
        /// Creates new instance from <see cref="Asmv1.file"/>
        /// </summary>
        /// <remarks>https://docs.microsoft.com/en-us/visualstudio/deployment/file-element-clickonce-application</remarks>
        public Dependency(Asmv1.file file, Manifest baseManifest = null) :
            this(baseManifest)
        {
            DependencyType = DependencyType.file;
            Codebase = file.name;
            Name = file.name;
            Size = file.size;

            if (file.hash != null)  // Optional
                ParseHash(file.hash, ref HashAlgo, ref HashValue);
        }

        /// <summary>
        /// Creates new instance from <see cref="Manifest"/>
        /// </summary>
        public Dependency(Manifest manifest, Manifest baseManifest = null) :
            this(baseManifest)
        {
            DependencyType = DependencyType.file;
            Codebase = manifest.Name;
            // ensure we have a proper filename here
            if (!Codebase.Contains('.'))
                Codebase += ".application";
            Name = manifest.Name;
            Version = manifest.Version;
            PublicKeyToken = manifest.PublicKeyToken;
            Language = manifest.Language;
            Architecture = manifest.Architecture;
            internalBytes = manifest.GetBytesOfSelf();
        }

        /// <summary>
        /// Create dependency from name and known bytes
        /// </summary>
        public Dependency(string name, byte[] data)
        {
            DependencyType = DependencyType.file;
            Codebase = name;
            Name = name;
            internalBytes = data;
        }

        private static void ParseHash(Asmv1.hash hash,
            ref string hashAlgo, ref byte[] hashValue)
        {
            // Transforms?
            hashAlgo = hash.DigestMethod.Algorithm;
            if (!string.IsNullOrEmpty(hashAlgo) &&
                hashAlgo.StartsWith(SignedXml.XmlDsigNamespaceUrl))
                hashAlgo = hashAlgo.Substring(SignedXml.XmlDsigNamespaceUrl.Length);
            hashValue = Convert.FromBase64String(hash.DigestValue);
        }

        /// <summary>
        /// Get name used for library directory
        /// </summary>
        public string LocalLibName()
        {
            return Tools.GetLocalLibName(Name, PublicKeyToken, Version, Language, HashValue);
        }

        /// <summary>
        /// Uri that this should be loadable from
        /// </summary>
        public string RemoteUri()
        {
            if (Codebase == null || string.IsNullOrEmpty(BaseLocation))
                return null;
            return BaseLocation + "/" +
                FixToPathSeperator(Codebase) +
                (MapFileExtensions ? ".deploy" : string.Empty);
        }

        /// <summary>
        /// Validate size and checksum
        /// </summary>
        /// <exception cref="ValidationSizeException">When size differs</exception>
        /// <exception cref="NotImplementedException">If the checksum algoritm is not implemented</exception>
        /// <exception cref="ValidationDigestException">When checksum differs</exception>
        public void Validate(Stream s)
        {
            if (0 != Size && s.Length != Size)
                throw new ValidationSizeException(Codebase, Size, (uint)s.Length);
            var hasher = Tools.GetHasher(HashAlgo);
            s.Seek(0, SeekOrigin.Begin);
            var calcedHash = hasher.ComputeHash(s);
            s.Seek(0, SeekOrigin.Begin);
            if (!HashValue.AreEqual(calcedHash))
            {
                throw new ValidationDigestException(Codebase, HashValue, calcedHash);
            }
        }

        /// <summary>
        /// Validate size and checksum from path
        /// </summary>
        public void Validate(string path)
        {
            if (Size == 0 && HashAlgo == null)
                return;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                Validate(fs);
        }

        /// <summary>
        /// Predefined dataset
        /// </summary>
        private readonly byte[] internalBytes;

        // TOOO Implement progress callback
        /// <summary>
        /// Downloads The Dependency from <see cref="RemoteUri"/>
        /// </summary>
        /// <param name="wc">Possible <see cref="System.Net.WebClient"/> that can have callbacks for progress</param>
        public Task<byte[]> DownloadAsync(System.Net.WebClient wc = null)
        {
            // TODO fix this unecessary task properly
            if (internalBytes != null)
                return Task.Factory.StartNew(() => internalBytes);
            if (wc == null)
                wc = new System.Net.WebClient();
            return wc.DownloadDataTaskAsync(RemoteUri());
        }

        /// <summary>
        /// Lock to prevent multiple fetches of the same dependency
        /// </summary>
        private readonly object fetchResLock = new object();

        /// <summary>
        /// Will hold the value that GetToLocalPathAsync last returned
        /// </summary>
        public string LastSeenLocalPath;

        /// <summary>
        /// Path to localy stored version
        /// </summary>
        public Task<string> GetToLocalPathAsync(string storage)
        {
            string saveTo = Path.Combine(storage, Codebase);
            Directory.CreateDirectory(Path.GetDirectoryName(saveTo));

            lock (fetchResLock)
            {
                if (File.Exists(saveTo))
                {
                    // on success, return existing path
                    // TODO fix this Task.Run hack
                    try
                    {
                        Validate(saveTo);
                        LastSeenLocalPath = saveTo;
                        return Task.Factory.StartNew(() => saveTo);
                    }
                    catch (ValidationException)
                    {
                        // validation failed, try redownload before failing
                    }
                }

                return DownloadAsync().ContinueWith(t =>
                {
                    if (t.Exception != null)
                        throw t.Exception.Flatten();
                    File.WriteAllBytes(saveTo, t.Result);
                    Validate(saveTo);
                    LastSeenLocalPath = saveTo;
                    return saveTo;
                });
            }
        }

        /// <summary>
        /// Returns dependency Name
        /// </summary>
        /// <returns>dependency Name</returns>
        public override string ToString()
        {
            if (Version == null)
                return Name;
            return $"{Name} {Version}";
        }
    }
}
