using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClickOnce.Asmv1
{
    /// <remarks/>
    public static class Constants
    {
        /// <remarks/>
        public const string NS_COMPAT = "urn:schemas-microsoft-com:compatibility.v1";
        /// <remarks/>
        public const string NS_ASMV1 = "urn:schemas-microsoft-com:asm.v1";
        /// <remarks/>
        public const string NS_ASMV2 = "urn:schemas-microsoft-com:asm.v2";
        /// <remarks/>
        public const string NS_ASMV3 = "urn:schemas-microsoft-com:asm.v3";
        /// <remarks/>
        public const string NS_COV1 = "urn:schemas-microsoft-com:clickonce.v1";
        /// <remarks/>
        public const string NS_COV2 = "urn:schemas-microsoft-com:clickonce.v2";
        /// <remarks/>
        public const string NS_R = "urn:mpeg:mpeg21:2003:01-REL-R-NS";
        /// <remarks/>
        public const string NS_DSIG = System.Security.Cryptography.Xml.SignedXml.XmlDsigNamespaceUrl;
        /// <remarks/>
        public const string NS_MSREL = "http://schemas.microsoft.com/windows/rel/2005/reldata";
        /// <remarks/>
        public const string NS_AUTHENTICODE = "http://schemas.microsoft.com/windows/pki/2005/Authenticode";
        /// <remarks/>
        public const string NS_WINDOWSSETTINGS = "http://schemas.microsoft.com/SMI/2011/WindowsSettings";
        /// <remarks/>
        public const string NS_XSI = "http://www.w3.org/2001/XMLSchema-instance";
        /// <remarks/>
        public const string SCHEMA_LOCATION = "urn:schemas-microsoft-com:asm.v1 assembly.adaptive.xsd";
        /// <remarks/>
        public const string CODE = "code";

        /// <remarks/>
        public static readonly XmlSerializerNamespaces Namespaces = namespaces();

        private static XmlSerializerNamespaces namespaces()
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("asmv1", NS_ASMV1);
            ns.Add("asmv2", NS_ASMV2);
            ns.Add("co.v1", NS_COV1);
            ns.Add("co.v2", NS_COV2);
            ns.Add("dsig", NS_DSIG);
            ns.Add("msrel", NS_MSREL);
            ns.Add("r", NS_R);
            ns.Add("as", NS_AUTHENTICODE);
            ns.Add("xsi", NS_XSI);
            return ns;
        }
    }
}