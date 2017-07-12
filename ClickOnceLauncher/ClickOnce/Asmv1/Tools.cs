using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace ClickOnce.Asmv1
{
    /// <remarks/>
    public static class Tools
    {
        /// <remarks/>
        private static readonly XmlSerializer AsmSerialiser = new XmlSerializer(typeof(assembly));

        /// <remarks/>
        public static void Serialize(TextWriter writer, assembly asm)
        {
            AsmSerialiser.Serialize(writer, asm, Constants.Namespaces);
        }

        /// <remarks/>
        public static assembly Deserialize(string file)
        {
            using (var fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                return (assembly)AsmSerialiser.Deserialize(fstream);
        }

        /// <remarks/>
        public static assembly Deserialize(XDocument xmlDoc)
        {
            using (var reader = xmlDoc.Root.CreateReader())
                return (assembly)AsmSerialiser.Deserialize(reader);
        }
    }
}
