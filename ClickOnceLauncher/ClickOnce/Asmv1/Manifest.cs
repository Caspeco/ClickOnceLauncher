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
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    [XmlRoot(Namespace = NS_ASMV2, IsNullable = false)]
    public partial class entryPoint
    {
        /// <remarks/>
        public entryPointAssemblyIdentity assemblyIdentity { get; set; }

        /// <remarks/>
        public entryPointCommandLine commandLine { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class entryPointAssemblyIdentity
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
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class entryPointCommandLine
    {
        /// <remarks/>
        [XmlAttribute]
        public string file { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string parameters { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    [XmlRoot(Namespace = NS_ASMV2, IsNullable = false)]
    public partial class trustInfo
    {
        /// <remarks/>
        public trustInfoSecurity security { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class trustInfoSecurity
    {
        /// <remarks/>
        public trustInfoSecurityApplicationRequestMinimum applicationRequestMinimum { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_ASMV3)]
        public requestedPrivileges requestedPrivileges { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class trustInfoSecurityApplicationRequestMinimum
    {
        /// <remarks/>
        public trustInfoSecurityApplicationRequestMinimumPermissionSet PermissionSet { get; set; }

        /// <remarks/>
        public trustInfoSecurityApplicationRequestMinimumDefaultAssemblyRequest defaultAssemblyRequest { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class trustInfoSecurityApplicationRequestMinimumPermissionSet
    {
        /// <remarks/>
        [XmlAttribute]
        public byte version { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string @class { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Description { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public bool Unrestricted { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string ID { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string SameSite { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class trustInfoSecurityApplicationRequestMinimumDefaultAssemblyRequest
    {
        /// <remarks/>
        [XmlAttribute]
        public string permissionSetReference { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV3)]
    [XmlRoot(Namespace = NS_ASMV3, IsNullable = false)]
    public partial class requestedPrivileges
    {
        /// <remarks/>
        public requestedPrivilegesRequestedExecutionLevel requestedExecutionLevel { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV3)]
    public partial class requestedPrivilegesRequestedExecutionLevel
    {
        /// <remarks/>
        [XmlAttribute]
        public string level { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public bool uiAccess { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class dependencyDependentOS
    {
        /// <remarks/>
        public dependencyDependentOSOsVersionInfo osVersionInfo { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class dependencyDependentOSOsVersionInfo
    {
        /// <remarks/>
        public dependencyDependentOSOsVersionInfoOS os { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    public partial class dependencyDependentOSOsVersionInfoOS
    {
        /// <remarks/>
        [XmlAttribute]
        public byte majorVersion { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public byte minorVersion { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public ushort buildNumber { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public byte servicePackMajor { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_ASMV2)]
    [XmlRoot(Namespace = NS_ASMV2, IsNullable = false)]
    public partial class file
    {
        /// <remarks/>
        public hash hash { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public uint size { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_COMPAT)]
    [XmlRoot(Namespace = NS_COMPAT, IsNullable = false)]
    public partial class compatibility
    {
        /// <remarks/>
        public compatibilityApplication application { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_COMPAT)]
    public partial class compatibilityApplication
    {
        /// <remarks/>
        [XmlElement("supportedOS")]
        public compatibilityApplicationSupportedOS[] supportedOS { get; set; }

        /// <remarks/>
        [XmlElement(Namespace = NS_WINDOWSSETTINGS)]
        public windowsSettings windowsSettings { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_COMPAT)]
    public partial class compatibilityApplicationSupportedOS
    {
        /// <remarks/>
        [XmlAttribute]
        public string Id { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory(CODE)]
    [XmlType(AnonymousType = true, Namespace = NS_WINDOWSSETTINGS)]
    [XmlRoot(Namespace = NS_WINDOWSSETTINGS, IsNullable = false)]
    public partial class windowsSettings
    {
        /// <remarks/>
        public string dpiAware { get; set; }

        /// <remarks/>
        public bool highResolutionScrollingAware { get; set; }

        /// <remarks/>
        public bool ultraHighResolutionScrollingAware { get; set; }
    }
}
