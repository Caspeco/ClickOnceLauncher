
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace ClickOnceLauncher.Tests.ClickOnceTests.Asmv1
{
    public class AssemblyXmlSerializationTests
    {
        public static readonly string TestFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "TestFiles");

        public static IEnumerable<object[]> GetTestFiles()
        {
            foreach (string file in Directory.GetFiles(TestFilesPath))
                yield return new object[] { file };
        }

        [Fact]
        public void BasicSerializationAndDeserializationWithoutExceptionTest()
        {
            // this is a simple test to check that we don't get exceptions on Serialization
            // due to incorrect data structures
            // also checks for namespace prefix
            var asmObject = new ClickOnce.Asmv1.assembly();
            asmObject.description = new ClickOnce.Asmv1.assemblyDescription();
            using (var writer = new StringWriterUtf8())
            {
                ClickOnce.Asmv1.Tools.Serialize(writer, asmObject);
                writer.Close();
                System.Diagnostics.Debug.Print(writer.ToString());
                // Namespace prefix not found
                Assert.Contains("<asmv1:assembly", writer.ToString());
                // Another Namespace prefix not found
                Assert.Contains(" xsi:schemaLocation=\"urn:schemas-microsoft-com:asm.v1 assembly.adaptive.xsd\" ", writer.ToString());
            }
        }

        [Theory]
        [MemberData(nameof(GetTestFiles))]
        public void EnsureDeserializeAndSerializeCreatesSameDataTest(string testfile)
        {
            // Test to ensure that all data available in .application and .manifest is available on deserialize
            // Quite a lot of hacks to ignore XML namespaces

            System.Diagnostics.Debug.Print(testfile);
            string contents = File.ReadAllText(testfile);
            if (!contents.Contains("<?xml "))
            {
                // TODO use separate function to identify filetype
                System.Diagnostics.Debug.Print(" not XML data - assuming appref-ms");
                System.Diagnostics.Debug.Print(contents);
                Assert.EndsWith(".appref-ms", testfile);
                return;
            }

            var asmObject = ClickOnce.Asmv1.Tools.Deserialize(testfile);

            using (var writer = new StringWriterUtf8())
            {
                ClickOnce.Asmv1.Tools.Serialize(writer, asmObject);
                writer.Close();
                string outData = writer.ToString();

                foreach (string remove in new string[] {
                    " xmlns=\"urn:schemas-microsoft-com:asm.v2\"",
                    " xmlns=\"urn:schemas-microsoft-com:asm.v1\"",
                    " xmlns=\"urn:schemas-microsoft-com:clickonce.v2\"",
                    " xmlns=\"http://www.w3.org/2000/09/xmldsig#\"",
                    " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"",
                    " xsi:schemaLocation=\"urn:schemas-microsoft-com:asm.v1 assembly.adaptive.xsd\"",
                    " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"",
                    " xmlns:co.v1=\"urn:schemas-microsoft-com:clickonce.v1\"",
                    " xmlns:co.v2=\"urn:schemas-microsoft-com:clickonce.v2\"",
                    " xmlns:asmv1=\"urn:schemas-microsoft-com:asm.v1\"",
                    " xmlns:asmv2=\"urn:schemas-microsoft-com:asm.v2\"",
                    " xmlns:asmv3=\"urn:schemas-microsoft-com:asm.v3\"",
                    " xmlns:dsig=\"http://www.w3.org/2000/09/xmldsig#\"",
                    " xmlns:xrml=\"urn:mpeg:mpeg21:2003:01-REL-R-NS\"",
                    " xmlns:msrel=\"http://schemas.microsoft.com/windows/rel/2005/reldata\"",
                    " xmlns:r=\"urn:mpeg:mpeg21:2003:01-REL-R-NS\"",
                    " xmlns:as=\"http://schemas.microsoft.com/windows/pki/2005/Authenticode\"",
                    "  ",
                    "\r\n",
                    "\n",
                })
                {
                    contents = contents.Replace(remove, string.Empty);
                    outData = outData.Replace(remove, string.Empty);
                }

                foreach (string cleantag in new string[] {
                    "asmv1:",
                    "asmv2:",
                    "co.v1:",
                    "co.v2:",
                    "dsig:",
                    "r:",
                    "as:",
                })
                {
                    foreach (string pre in new string[] { "<", "</" })
                    {
                        contents = contents.Replace(pre + cleantag, pre);
                        outData = outData.Replace(pre + cleantag, pre);
                    } 
                }

                contents = Regex.Replace(contents, "<!--.*?-->", string.Empty);
                //contents = contents.Replace("</dependency><dependency><dependentAssembly", string.Empty);

                Assert.Equal(contents, outData);
                System.Diagnostics.Debug.Print(writer.ToString());
            }
        }
    }

    /// <summary>
    /// Class to give UTF-8 Encoded XML
    /// Based on https://stackoverflow.com/a/1564727/2716218
    /// </summary>
    public sealed class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
