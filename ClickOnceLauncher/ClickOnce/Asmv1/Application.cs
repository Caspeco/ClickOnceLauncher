using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using static ClickOnce.Asmv1.Constants;

namespace ClickOnce.Asmv1
{
    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV1)]
    [XmlRoot(Namespace = NS_ASMV1, IsNullable = false)]
    public partial class assembly
    {
        /// <remarks/>
        [XmlAttribute(Namespace = NS_XSI)]
        public string schemaLocation = SCHEMA_LOCATION;

        /// <remarks/>
        public assemblyAssemblyIdentity assemblyIdentity { get; set; }

        /// <remarks/>
        public assemblyDescription description { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_ASMV2)]
        public object application { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_ASMV2)]
        public entryPoint entryPoint { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_ASMV2)]
        public trustInfo trustInfo { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_ASMV2)]
        public deployment deployment { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_COV2)]
        public compatibleFrameworks compatibleFrameworks { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_ASMV2)]
        public dependency[] dependency { get; set; }

        /// <remarks/>
        [XmlElement("file", Namespace = NS_ASMV2)]
        public file[] file { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_COMPAT)]
        public compatibility compatibility { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_ASMV2)]
        public publisherIdentity publisherIdentity { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_DSIG)]
        public Signature[] Signature { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public decimal manifestVersion { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV1)]
    public partial class assemblyAssemblyIdentity
    {
        /// <remarks/>
        [XmlAttribute]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string version { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string publicKeyToken { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string language { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string processorArchitecture { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV1)]
    public partial class assemblyDescription
    {
        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = NS_ASMV2)]
        public string iconFile { get; set; }

        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = NS_ASMV2)]
        public string publisher { get; set; }

        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = NS_ASMV2)]
        public string product { get; set; }

        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = NS_ASMV2)]
        public string supportUrl { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    [XmlRoot(Namespace = NS_ASMV2, IsNullable = false)]
    public partial class deployment
    {
        private bool? trustURLParametersField;
        private bool? createDesktopShortcutField;

        /// <remarks/>
        public deploymentSubscription subscription { get; set; }

        /// <remarks/>
        public deploymentDeploymentProvider deploymentProvider { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public bool install { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public bool mapFileExtensions { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string minimumRequiredVersion { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public bool trustURLParameters
        {
            get { return trustURLParametersField.Value; }
            set { trustURLParametersField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool trustURLParametersSpecified
        {
            get { return trustURLParametersField.HasValue; }
        }

        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = NS_COV1)]
        public bool createDesktopShortcut {
            get { return createDesktopShortcutField.Value; }
            set { createDesktopShortcutField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createDesktopShortcutSpecified
        {
            get { return createDesktopShortcutField.HasValue; }
        }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class deploymentSubscription
    {
        /// <remarks/>
        public deploymentSubscriptionUpdate update { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class deploymentSubscriptionUpdate
    {
        /// <remarks/>
        public object beforeApplicationStartup { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class deploymentDeploymentProvider
    {
        /// <remarks/>
        [XmlAttribute]
        public string codebase { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_COV2)]
    [XmlRoot(Namespace = NS_COV2, IsNullable = false)]
    public partial class compatibleFrameworks
    {
        /// <remarks/>
        public compatibleFrameworksFramework framework { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_COV2)]
    public partial class compatibleFrameworksFramework
    {
        /// <remarks/>
        [XmlAttribute]
        public decimal targetVersion { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string profile { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string supportedRuntime { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    [XmlRoot(Namespace = NS_ASMV2, IsNullable = false)]
    public partial class dependency
    {
        /// <remarks/>
        public dependencyDependentAssembly dependentAssembly { get; set; }

        /// <remarks/>
        public dependencyDependentOS dependentOS { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class dependencyDependentAssembly
    {
        private bool? allowDelayedBindingField;
        private uint? sizeField;

        /// <remarks/>
        public dependencyDependentAssemblyAssemblyIdentity assemblyIdentity { get; set; }

        /// <remarks/>
        public hash hash { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string dependencyType { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public bool allowDelayedBinding
        {
            get { return allowDelayedBindingField.Value; }
            set { allowDelayedBindingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool allowDelayedBindingSpecified { get { return allowDelayedBindingField.HasValue; } }

        /// <remarks/>
        [XmlAttribute]
        public string codebase { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public uint size
        {
            get { return sizeField.Value; }
            set { sizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool sizeSpecified { get { return sizeField.HasValue; } }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class dependencyDependentAssemblyAssemblyIdentity
    {
        /// <remarks/>
        [XmlAttribute]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string version { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string publicKeyToken { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string language { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string processorArchitecture { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class hash
    {
        /// <remarks/>
        [XmlElement(Namespace = NS_DSIG)]
        public Transforms Transforms { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_DSIG)]
        public DigestMethod DigestMethod { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_DSIG)]
        public string DigestValue { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    [XmlRoot(Namespace = NS_DSIG, IsNullable = false)]
    public partial class Transforms
    {
        /// <remarks/>
        public DigestMethod Transform { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    public partial class DigestMethod
    {
        /// <remarks/>
        [XmlAttribute]
        public string Algorithm { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    [XmlRoot(Namespace = NS_ASMV2, IsNullable = false)]
    public partial class publisherIdentity
    {
        /// <remarks/>
        [XmlAttribute]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string issuerKeyHash { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    [XmlRoot(Namespace = NS_DSIG, IsNullable = false)]
    public partial class Signature
    {
        /// <remarks/>
        public SignatureSignedInfo SignedInfo { get; set; }

        /// <remarks/>
        public string SignatureValue { get; set; }

        /// <remarks/>
        public SignatureKeyInfo KeyInfo { get; set; }

        /// <remarks/>
        public SignatureObject Object { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Id { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    public partial class SignatureSignedInfo
    {
        /// <remarks/>
        public DigestMethod CanonicalizationMethod { get; set; }

        /// <remarks/>
        public DigestMethod SignatureMethod { get; set; }

        /// <remarks/>
        public SignatureSignedInfoReference Reference { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    public partial class SignatureSignedInfoReference
    {
        /// <remarks/>
        [XmlArrayItem("Transform", IsNullable = false)]
        public DigestMethod[] Transforms { get; set; }

        /// <remarks/>
        public DigestMethod DigestMethod { get; set; }

        /// <remarks/>
        public string DigestValue { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string URI { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    public partial class SignatureKeyInfo
    {
        /// <remarks/>
        public SignatureKeyInfoKeyValue KeyValue { get; set; }

        /// <remarks/>
        [XmlArrayItem("X509Certificate", IsNullable = false)]
        public string[] X509Data { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_MSREL)]
        public RelData RelData { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Id { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    public partial class SignatureKeyInfoKeyValue
    {
        /// <remarks/>
        public SignatureKeyInfoKeyValueRSAKeyValue RSAKeyValue { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    public partial class SignatureKeyInfoKeyValueRSAKeyValue
    {
        /// <remarks/>
        public string Modulus { get; set; }

        /// <remarks/>
        public string Exponent { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_MSREL)]
    [XmlRoot(Namespace = NS_MSREL, IsNullable = false)]
    public partial class RelData
    {
        /// <remarks/>
        [XmlElement(Namespace = NS_R)]
        public license license { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_R)]
    [XmlRoot(Namespace = NS_R, IsNullable = false)]
    public partial class license
    {
        /// <remarks/>
        public licenseGrant grant { get; set; }

        /// <remarks/>
        public licenseIssuer issuer { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_R)]
    public partial class licenseGrant
    {
        /// <remarks/>
        [XmlElement(Namespace = NS_AUTHENTICODE)]
        public ManifestInformation ManifestInformation { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_AUTHENTICODE)]
        public object SignedBy { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_AUTHENTICODE)]
        public AuthenticodePublisher AuthenticodePublisher { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_AUTHENTICODE)]
    [XmlRoot(Namespace = NS_AUTHENTICODE, IsNullable = false)]
    public partial class ManifestInformation
    {
        /// <remarks/>
        public ManifestInformationAssemblyIdentity assemblyIdentity { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Hash { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Description { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Url { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_AUTHENTICODE)]
    public partial class ManifestInformationAssemblyIdentity
    {
        /// <remarks/>
        [XmlAttribute]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string version { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string publicKeyToken { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string language { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string processorArchitecture { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_AUTHENTICODE)]
    [XmlRoot(Namespace = NS_AUTHENTICODE, IsNullable = false)]
    public partial class AuthenticodePublisher
    {
        /// <remarks/>
        public string X509SubjectName { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_R)]
    public partial class licenseIssuer
    {
        /// <remarks/>
        [XmlElement(Namespace = NS_DSIG)]
        public Signature Signature { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_DSIG)]
    public partial class SignatureObject
    {
        /// <remarks/>
        [XmlElement(Namespace = NS_AUTHENTICODE)]
        public string Timestamp { get; set; }
    }
}
