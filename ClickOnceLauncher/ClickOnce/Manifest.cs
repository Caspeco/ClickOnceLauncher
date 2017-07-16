using System;
using System.Collections.Generic;
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
    public class Manifest
    {
        static Manifest()
        {
            // setup sha256 support for SignedXml
            CryptoConfig.AddAlgorithm(typeof(System.Deployment.Internal.CodeSigning.RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
            CryptoConfig.AddAlgorithm(typeof(System.Deployment.Internal.CodeSigning.RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2000/09/xmldsig#rsa-sha256");
        }

        private readonly XDocument Xml;
        private readonly Asmv1.assembly assembly;

        /// <summary>
        /// Product
        /// </summary>
        public readonly string Product;

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
        /// Required. Specifies whether this application defines a presence on the Windows Start menu and in the Control Panel Add or Remove Programs application
        /// If false ClickOnce will always run the latest version of this application from the network, and will not recognize the subscription element.
        /// </summary>
        public readonly bool? Install;

        /// <summary>
        /// Optional. Specifies the minimum version of this application that can run on the client. If the version number of the application is less than the version number supplied in the deployment manifest, the application will not run
        /// If install is false then this must be set
        /// </summary>
        public readonly Version MinimumRequiredVersion;

        /// <summary>
        /// Optional. Defaults to false. If true, all files in the deployment must have a .deploy extension. ClickOnce will strip this extension off these files as soon as it downloads them from the Web server. If you publish your application by using Visual Studio, it automatically adds this extension to all files. This parameter allows all the files within a ClickOnce deployment to be downloaded from a Web server that blocks transmission of files ending in "unsafe" extensions such as .exe.
        /// </summary>
        public bool MapFileExtensions;

        /// <summary>
        /// Optional. When set, the application will be blocked when ClickOnce checks for updates, if the client is online. If this element does not exist, ClickOnce will first scan for updates based on the values specified for the expiration element.
        /// </summary>
        public readonly bool SubscriptionUpdateBeforeApplicationStartup;

        internal readonly string DeploymentProviderCodebase;

        internal readonly string Location;

        private EntryPoint entry = new EntryPoint();
        /// <remarks/>
        public EntryPoint Entry { get { return entry; } }

        /// <param name="location">Uri to manifest, will be used as both location and path</param>
        public Manifest(string location) :
            this(location, location)
        {
        }

        /// <param name="location">Uri to manifest, original download path</param>
        /// <param name="path">Path to manifest, can be local filename</param>
        /// <remarks>https://docs.microsoft.com/sv-se/visualstudio/deployment/deployment-element-clickonce-deployment</remarks>
        public Manifest(string location, string path) :
            this(location, XDocument.Load(path, LoadOptions.PreserveWhitespace))
        {
        }

        /// <remarks>https://docs.microsoft.com/sv-se/visualstudio/deployment/deployment-element-clickonce-deployment</remarks>
        public Manifest(string location, XDocument xml)
        {
            Location = location;
            Xml = xml;
            assembly = Asmv1.Tools.Deserialize(Xml);
            var ident = assembly.assemblyIdentity;
            Name = ident.name;
            Version = new Version(ident.version);
            PublicKeyToken = ident.publicKeyToken;
            Language = ident.language;
            Architecture = ident.processorArchitecture;

            var entryp = assembly.entryPoint;
            if (entryp != null)
            {
                entry.Name = entryp.assemblyIdentity.name;
                entry.Version = new Version(entryp.assemblyIdentity.version);
                entry.PublicKeyToken = entryp.assemblyIdentity.publicKeyToken;
                entry.Language = entryp.assemblyIdentity.language;
                entry.Architecture = entryp.assemblyIdentity.processorArchitecture;
                entry.Executable = entryp.commandLine.file;
                entry.Parameters = entryp.commandLine.parameters;
            }
            if (assembly.description != null)
            {
                entry.Icon = assembly.description.iconFile;
                entry.Publisher = assembly.description.publisher;
                entry.Product = assembly.description.product;
                entry.SupportUrl = assembly.description.supportUrl;
            }

            if (assembly.deployment != null)
            {
                Install = assembly.deployment.install;
                MapFileExtensions = assembly.deployment.mapFileExtensions;
                if (assembly.deployment.minimumRequiredVersion != null)
                    MinimumRequiredVersion = new Version(assembly.deployment.minimumRequiredVersion);

                // if update is defined
                if (assembly.deployment.subscription != null &&
                    assembly.deployment.subscription.update != null)
                    SubscriptionUpdateBeforeApplicationStartup =
                        assembly.deployment.subscription.update.beforeApplicationStartup != null;

                if (assembly.deployment.deploymentProvider != null)
                    DeploymentProviderCodebase = assembly.deployment.deploymentProvider.codebase;
            }
        }

        /// <summary>
        /// Get raw bytes of what makes up the Manifest
        /// </summary>
        public byte[] GetBytesOfSelf()
        {
            using (var ms = new System.IO.MemoryStream())
            {
                Xml.Save(ms, SaveOptions.DisableFormatting);
                ms.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Gives new manifest if there is a deployment available, otherwise we return ourself
        /// </summary>
        public Manifest GetDeployment()
        {
            // TODO if !SubscriptionUpdateBeforeApplicationStartup then this should be delayed
            if (string.IsNullOrEmpty(DeploymentProviderCodebase))
                return this;

            // already loaded that version, don't load again
            if (DeploymentProviderCodebase == Location)
                return this;

            var newDeployment = new Manifest(DeploymentProviderCodebase);
            // TODO validate: newDeployment.VerifySignature();

            // TODO Do better checks for version
            if (Version == newDeployment.Version)
                return this;

            if (newDeployment.MinimumRequiredVersion != null &&
                Version < newDeployment.MinimumRequiredVersion)
                return newDeployment;

            return newDeployment;
        }

        /// <summary>
        /// Get name used for library directory
        /// </summary>
        public string LocalLibName()
        {
            return Tools.GetLocalLibName(Name, PublicKeyToken, Version, Language, null);
        }

        /// <remarks/>
        public IEnumerable<Dependency> GetDependencys()
        {
            if (Install.HasValue)
            {
                yield return new Dependency(entry.Product + ".appref-ms", GenerateApprefMsBytes());
                yield return new Dependency(this);
            }

            if (assembly.dependency != null)
                foreach (var dep in assembly.dependency)
                {
                    // TODO handle dependentOS information
                    if (dep.dependentAssembly == null &&
                        dep.dependentOS != null)
                        continue;

                    yield return new Dependency(dep, this);
                }

            if (assembly.file != null)
                foreach (var file in assembly.file)
                {
                    yield return new Dependency(file, this);
                }
        }

        /// <summary>
        /// Get a string used in .appref-ms file, save with Littele Endian Unicode
        /// </summary>
        public string GenerateApprefMsString()
        {
            return Location + "#" + Name +
                ", Culture=" + Language +
                ", PublicKeyToken=" + PublicKeyToken +
                ", processorArchitecture=" + Architecture;
        }

        /// <summary>
        /// Get a byte array for .appref-ms file
        /// </summary>
        public byte[] GenerateApprefMsBytes()
        {
            var enc = new UnicodeEncoding(false, true, true);
            return enc.GetPreamble().
                Concat(enc.GetBytes(GenerateApprefMsString())).
                ToArray();
        }

        /// <remarks/>
        public void VerifySignature()
        {
            var xdoc = new XmlDocument() { PreserveWhitespace = true };
            using (var reader = Xml.Root.CreateReader())
                xdoc.Load(reader);

            // TODO move these to separate reusable class
            var publisherIdentity = assembly.publisherIdentity;
            // if null throw ?
            string pubName = publisherIdentity.name;
            string pubHash = publisherIdentity.issuerKeyHash;

            if (assembly.Signature == null || assembly.Signature.Length == 0)
                throw new CryptographicException("No Signature found");
            foreach (var signode in assembly.Signature)
            {
                Console.WriteLine("Sig Node " + signode.Id);
            }

            var nodeList = xdoc.GetElementsByTagName("Signature", Asmv1.Constants.NS_DSIG);
            foreach (XmlElement signElm in nodeList)
            {
                // Possible BUG in .NET that makes us need to load signode twice instead of xdoc first
                var signed = new SignedXml(signElm);
                signed.LoadXml(signElm);

                Console.WriteLine("   ==  SignatureMethod " + signed.SignatureMethod);
                Console.WriteLine("   ==  SignedInfo.Id: " + signed.SignedInfo.Id);
                Console.WriteLine("   ==  KeyInfo " + signed.Signature.KeyInfo.GetType());

                Console.WriteLine("   ==  signElm " + signElm.OuterXml);

                var keys = GetKeyInfoKeys(signed.Signature.KeyInfo);

                foreach (var key in keys)
                {
                    Console.WriteLine("   ==  key " + key);
                    bool validSignature = signed.CheckSignature(key);
                    //var validSignature = signed.CheckSignatureReturningKey(out AsymmetricAlgorithm key);
                    //if (!validSignature)
                    //    throw new CryptographicException("Signature not valid");
                }
            }


            // TODO move these to separate reusable class
            string updatedLocation = assembly.deployment.deploymentProvider.codebase;
        }

        /// <remarks/>
        public static IEnumerable<AsymmetricAlgorithm> GetKeyInfoKeys(KeyInfo keyInfo)
        {
            foreach (KeyInfoClause key in keyInfo)
            {
                foreach (System.Security.Cryptography.X509Certificates.
                    X509Certificate2 cert in (key as KeyInfoX509Data).Certificates)
                {
                    yield return cert.PublicKey.Key;
                }
                if (key is KeyInfoX509Data)
                {
                    var current = (KeyInfoX509Data)key;
                    foreach (System.Security.Cryptography.X509Certificates.
                        X509Certificate2 cert in current.Certificates)
                        yield return cert.PublicKey.Key;
                }
                else if (key is RSAKeyValue)
                {
                    var current = (RSAKeyValue)key;
                        yield return current.Key;
                }
                else if (key is DSAKeyValue)
                {
                    var current = (DSAKeyValue)key;
                    yield return current.Key;
                }
                else if (key is KeyInfoNode)
                {
                    var current = (KeyInfoNode)key;
                    throw new CryptographicException(current.GetXml().OuterXml);
                }
                else
                    throw new CryptographicException("Unknown KeyInfo type: " + key.GetType());
            }
        }
    }
}
