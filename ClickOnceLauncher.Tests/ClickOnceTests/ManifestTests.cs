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
using Xunit;

namespace ClickOnceLauncher.Tests.ClickOnceTests
{
    public class ManifestTests
    {
        public static ClickOnce.ILog Console = LogConsole.Shared;

        private static string _appData;
        public static string GetTempPath()
        {
            if (!string.IsNullOrEmpty(_appData))
                return _appData;

            string appData = Path.Combine(Path.GetTempPath(), "ClickOnceTest");
            Console.WriteLine($" Temp path: {appData}");
            _appData = appData;
            return appData;
        }

        [Theory]
        [InlineData(@"..\..\TestFiles\GitHub.appref-ms")]
        [InlineData(@"..\..\TestFiles\Azure Explorer.appref-ms")]
        public void ValidateDownloadIntegrationTest(string testfile)
        {
            var app = new ClickOnce.Application();
            app.BasePath = GetTempPath();
            app.Load(testfile);
            var depAsm = app.LastLoadedManifest;
            // always true in App
            Assert.True(app.LastLoadedApp.Install.HasValue);
            // but should still be false in Manifest
            Assert.False(app.LastLoadedManifest.Install.HasValue);

            // for now we don't have any tests where this is false
            Assert.True(app.LastLoadedApp.MapFileExtensions);
            // this should be propagated
            Assert.True(app.LastLoadedManifest.MapFileExtensions);

            var deps = app.Dependencys;
            Console.WriteLine($" TotalSize: {app.TotalSizeByManifest} for total {deps.Count} dependencys");
            var entry = app.Entry;

            string baseLibName = app.BaseLibName;
            foreach (var d in deps)
            {
                Assert.Contains(".", d.Codebase);
                string libName = d.LocalLibName() ?? baseLibName;
                Console.WriteLine($"  LibName: {libName} {d.Codebase}");

                Assert.NotNull(libName);
                Assert.NotEmpty(libName);
                string p = Path.Combine(app.BasePath, libName);

                string dstPath = Path.Combine(entry.LocalPath, d.Codebase);
                Console.WriteLine($"{dstPath}");
            }

            app.CopyDependencysLocal();

            foreach (var fi in entry.GetType().GetFields())
            {
                Console.WriteLine($" Entry: {fi.Name} val: {fi.GetValue(entry)}");
            }
            Console.WriteLine($" Entry {entry.LocalLibName()}");
            string entryPath = Path.Combine(app.BasePath, entry.LocalLibName());
            Console.WriteLine($" Path {entryPath}");

            Console.WriteLine(string.Join("\n", deps.Select(d => d.Codebase).Where(n => Path.GetExtension(n) == ".exe")));

            Assert.Contains(".manifest", deps.Select(d => Path.GetExtension(d.Codebase)));
            Assert.Contains(".application", deps.Select(d => Path.GetExtension(d.Codebase)));
            Assert.Contains(Path.GetFileName(testfile.Replace("%20", " ")), deps.Select(d => Path.GetFileName(d.Codebase)));
            Assert.Contains(".appref-ms", deps.Select(d => Path.GetExtension(d.Codebase)));
            Assert.Contains(entry.Product + ".appref-ms", deps.Select(d => Path.GetFileName(d.Codebase)));
            Assert.Contains(entry.Executable, deps.Select(d => Path.GetFileName(d.Codebase)));

            Assert.Equal(1, deps.Where(d => Path.GetExtension(d.Codebase) == ".manifest").Count());
            Assert.Equal(1, deps.Where(d => Path.GetExtension(d.Codebase) == ".application").Count());
            Assert.Equal(1, deps.Where(d => Path.GetFileName(d.Codebase) == entry.Executable).Count());
            Assert.Equal(1, deps.Where(d => Path.GetFileName(d.Codebase) == entry.Product + ".appref-ms").Count());
        }

        [Theory]
        //[MemberData(nameof(GetTestFiles))]
        [InlineData(@"..\..\TestFiles\GitHub.appref-ms")]
        [InlineData(@"..\..\TestFiles\Azure Explorer.appref-ms")]
        public void ValidateSignaturesTest(string testfile)
        {
            Console.WriteLine(testfile);
            string location = testfile;
            if (!testfile.StartsWith("http:") && !testfile.StartsWith("https:"))
            {
                string contents = File.ReadAllText(testfile);
                if (!contents.Contains("<?xml "))
                {
                    // TODO use separate function to identify filetype
                    Console.WriteLine(" not XML data - assuming appref-ms");
                    Console.WriteLine(contents);
                    Assert.EndsWith(".appref-ms", testfile);
                    string newPath = contents.Substring(0, contents.IndexOf("#"));
                    ValidateSignaturesTest(newPath);
                    return;
                }
            }
            bool isApplicationFile = testfile.EndsWith(".application");
            var asm = new ClickOnce.Manifest(location, testfile);
            var depAsm = asm.GetDeployment();
            Assert.Equal(isApplicationFile, depAsm.Install.HasValue);
            HandleManifest(depAsm);
        }

        private static void HandleManifest(ClickOnce.Manifest depAsm, string baseLibName = null)
        {
            // for now we don't have any tests where this is false
            Assert.True(depAsm.MapFileExtensions);
            if (baseLibName == null)
                baseLibName = depAsm.LocalLibName();
            Console.WriteLine($" BaseLibName: {baseLibName}");
            Assert.NotNull(baseLibName);
            Assert.NotEmpty(baseLibName);
            //depAsm.VerifySignature();
            ClickOnce.Dependency chainTo = null;
            foreach (var d in depAsm.GetDependencys())
            {
                Console.WriteLine($"+ {d} {d.Size} {d.DependencyType}");
                if (d.Codebase == null)
                {
                    Console.WriteLine($"!! No Codebase skipping");
                    continue;
                }
                if (d.Codebase.EndsWith(".manifest") && chainTo == null)
                    chainTo = d;
                Assert.Contains(".", d.Codebase);
                string remote = d.RemoteUri();
                string libName = d.LocalLibName() ?? baseLibName;
                Console.WriteLine($"  LibName: {libName} {d.Codebase}");
                Console.WriteLine($"  Remote: {remote}");
                if (remote != null && remote.StartsWith("http"))
                {
                    if (depAsm.Install.HasValue)
                        Assert.EndsWith(".manifest", remote);
                    else // this can only be tested when manifest is loaded from application file
                        Assert.EndsWith(".deploy", remote);
                }

                Assert.NotNull(libName);
                Assert.NotEmpty(libName);
            }

            if (chainTo != null)
            {
                string remote = chainTo.RemoteUri();
                string libName = chainTo.LocalLibName() ?? baseLibName;

                string p = Path.Combine(GetTempPath(), libName);
                chainTo.GetToLocalPathAsync(p).
                    ContinueWith(t => {
                        if (t.Exception != null)
                            throw t.Exception.Flatten();
                        Console.WriteLine($" - OK {t.Result.Replace(GetTempPath(), "...")}");
                        return t.Result;
                    }).Wait();
                string path = chainTo.LastSeenLocalPath;
                Assert.NotNull(path); // we should already have waited

                var app = new ClickOnce.Manifest(remote, path);
                app.MapFileExtensions = depAsm.MapFileExtensions;
                HandleManifest(app, libName);
            }
        }

        [Theory]
        [InlineData(@"..\..\TestFiles\GitHub.appref-ms")]
        [InlineData(@"..\..\TestFiles\Azure Explorer.appref-ms")]
        public void GenerateApprefMsStringTest(string orgappref)
        {
            Console.WriteLine(orgappref);
            string contents = File.ReadAllText(orgappref);
            string newPath = contents.Substring(0, contents.IndexOf("#"));
            var asm = new ClickOnce.Manifest(newPath);

            Assert.Equal(contents, asm.GenerateApprefMsString());
            byte[] rawBytes = File.ReadAllBytes(orgappref);
            Assert.Equal(rawBytes, asm.GenerateApprefMsBytes());
        }

        [Theory]
        [InlineData(@"rest..net2_f6fe32e50e0a4676_0066.0007_none_6221ff226514e44e_dev", "RestSharp.Net2", "102.7.0.0", "F6FE32E50E0A4676", "neutral", "YiH/ImUU5E5V7u4BcyG3KT1nKVw=")]
        //[InlineData(@"rest..net2_f6fe32e50e0a4676_0066.0007_none_5c5715a499681cf4", "RestSharp.Net2", "102.7.0.0", "F6FE32E50E0A4676", "neutral", "YiH/ImUU5E5V7u4BcyG3KT1nKVw=", Skip = "Not Implemented correctly yet")]
        [InlineData(@"pdfs..rces_f94615aa0424f9eb_0001.0032_de_afabf9e432ea9404_dev", "PdfSharp.resources", "1.50.3915.0", "F94615AA0424F9EB", "de", "r6v55DLqlARA+W6RNuhfXDdWfJE=")]
        //[InlineData(@"pdfs..rces_f94615aa0424f9eb_0001.0032_de_ee4d966e84a78747", "PdfSharp.resources", "1.50.3915.0", "F94615AA0424F9EB", "de", "r6v55DLqlARA+W6RNuhfXDdWfJE=", Skip = "Not Implemented correctly yet")]
        [InlineData(@"akavache__0004.0001_none_0fe3ab26a196fea6_dev", "Akavache", "4.1.2.0", null, "neutral", "D+OrJqGW/qZJ4mqJbN1/C4mbMj6TbkN7AV/bOf0/png=")]
        public void DependencyLocalLibNameTest(string expected, string asm, string ver, string pubkey, string lang, string digest)
        {
            var dep = new ClickOnce.Asmv1.dependency();
            dep.dependentAssembly = new ClickOnce.Asmv1.dependencyDependentAssembly();
            dep.dependentAssembly.dependencyType = "install";
            dep.dependentAssembly.assemblyIdentity = new ClickOnce.Asmv1.dependencyDependentAssemblyAssemblyIdentity();
            dep.dependentAssembly.assemblyIdentity.name = asm;
            dep.dependentAssembly.assemblyIdentity.version = ver;
            dep.dependentAssembly.assemblyIdentity.publicKeyToken = pubkey;
            dep.dependentAssembly.assemblyIdentity.language = lang;
            dep.dependentAssembly.assemblyIdentity.processorArchitecture = "msil";
            dep.dependentAssembly.hash = new ClickOnce.Asmv1.hash();
            dep.dependentAssembly.hash.DigestMethod = new ClickOnce.Asmv1.DigestMethod();
            dep.dependentAssembly.hash.DigestValue = digest;

            // this is found structure, separate each field with _ all lowercase
            // * assemblyIdentity Name (cut to 10 chars, if longer then take first 4, and last 4 add ".." inbetween)
            // ** there might be a special name for "finnished" download that ends with tion (installation?)
            // * PublicKeyToken (lowercase)
            // * Major.Minor version in hex
            // * language (none if neutral) this is missing if "installation"
            // * checksum len16 (of what? appname, pubkey, lang, arch ?)

            var libName = ClickOnce.Tools.GetLocalLibName(asm, pubkey, new Version(ver), lang, Convert.FromBase64String(digest));
            Assert.Equal(expected, libName);
        }

        // dotnet/corefx#19198.
        [Fact]
        void Verify_XdsNamespace()
        {
            const string inputXml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no"" ?><a><b>A</b><ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" xmlns:xds=""http://uri.etsi.org/01903/v1.1.1#""><ds:SignedInfo><ds:CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315""/><ds:SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/><ds:Reference URI=""""><ds:Transforms><ds:Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/><ds:Transform Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315""/></ds:Transforms><ds:DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/><ds:DigestValue>/l5xchvEjxgLdPNt9RiB3TTQTcc=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>cSUDo87XVNqbo+ljCZRAXvcuIXkak/10XyVWks9BNz5EdUmcVbiU84Y5qrJoAkMdyHPiIFkz8Dx5
v5psK5oZjGtRleQ67nm1BryHA8F7EW4Otxe/8hEHsqVFK0zz3P79XzKD2vl4lUaTTOLlCMD+Sbpb
ekyLkDIjXS/6IylPWbNNF3sH9MclGGhSSjREwOuJAyj8xqqibQEDvIytLN23+bZJtOGAU54ERXPn
h4rccBvzByno08DHVnkQrSQCgY4ECvVbocIFo7GGtq8v9oj6rK3KpWUQnL1V1Aqj5fXRNRC8VnxJ
yIkHAXOBWC3Wr+DQzEm4W0Xa+vZ8o0x/2Ct2cg==</ds:SignatureValue><ds:KeyInfo><ds:X509Data><ds:X509Certificate>MIIEejCCA+OgAwIBAgIIBg5jkS3DgTkwDQYJKoZIhvcNAQEFBQAwOjETMBEGA1UECxMKSW5m
cmF4STRDQTEWMBQGA1UEChMNSW5mcmF4IGQuby5vLjELMAkGA1UEBhMCU0kwHhcNMTEwNTAz
MDkxMzU2WhcNMTYwNTAxMDkxMzU2WjBfMRcwFQYDVQQDEw5QcnZpIFVwb3JhYm5pazEQMA4G
A1UECxMHaTQgdXNlcjEUMBIGA1UEChMLVGVzdCBkLm8uby4xDzANBgNVBAcTBlRvbG1pbjEL
MAkGA1UEBhMCU0kwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCkkkadn/Uapod4
N0f7GVgDNNN/vRoNUzqbdsERvUA7f/MoARF0LoXAoxpAEk2DDf24oIkGjL60uqwx4di5LQr8
3zsdA/7Wh7RHb73jCv0glfwK/YpfJcsxfLi/OajWNX5p63D9ISSDYtIgUb4k4JFElg+mbMsR
FlPQNyR6bADQTWputmTE+reWRiHLlnXLNSH3wGO008BfYuBnsuWupK8ar5UQ5I2IyafZJONO
4UQjsXsanYGqivWm147bihj7zfDjqEmVg0yYm9NS/jbhQSXRV9n8348engtu48NDg7Jsr363
Ou1nvThzsxIyRBbqSCNSJPAiEYl/jF4WRO2zGmZzAgMBAAGjggHeMIIB2jAMBgNVHRMBAf8E
AjAAMCAGA1UdJQEB/wQWMBQGCCsGAQUFBwMCBggrBgEFBQcDBDAfBgNVHSMEGDAWgBSjdHM8
IbieDPxF1rwNvMKR0T6WQDB3BgNVHR8EcDBuMGygaqBohmZodHRwOi8vY2EuaW5mcmF4LnNp
L2VqYmNhL3B1YmxpY3dlYi93ZWJkaXN0L2NlcnRkaXN0P2NtZD1jcmwmaXNzdWVyPU9VPUlu
ZnJheEk0Q0EsTz1JbmZyYXggZC5vLm8uLEM9U0kwDgYDVR0PAQH/BAQDAgSwMB0GA1UdDgQW
BBSKMErpxIQp6TQ8Jqr0ywYrYla4NzCB3gYIKwYBBQUHAQEEgdEwgc4wgYkGCCsGAQUFBzAC
hn1odHRwOi8vY2EuaW5mcmF4LnNpL2VqYmNhL3B1YmxpY3dlYi93ZWJkaXN0L2NlcnRkaXN0
P2NtZD1pZWNhY2VydCZpc3N1ZXI9T1UlM2RJbmZyYXhJNENBJTJjTyUzZEluZnJheCtkLm8u
by4lMmNDJTNkU0kmbGV2ZWw9MDBABggrBgEFBQcwAYY0aHR0cDovL2NhLmluZnJheC5zaTo4
MDgwL2VqYmNhL3B1YmxpY3dlYi9zdGF0dXMvb2NzcDANBgkqhkiG9w0BAQUFAAOBgQAms5UL
/f0AIavlVo9C5W5Ijdt12g/59PIZeAEDWzoEKi+WrE7ORaq527UHgcVGw7Gr2HhOEHmggUe6
k43edQ0fkNg5WXfJzf18hHA+foVqsxraDxBeot442A1zw9GjRnIDCl2r91tHgkneqg2EE8kf
7lkRtMRck1MUbHBnLnppKg==</ds:X509Certificate></ds:X509Data></ds:KeyInfo></ds:Signature></a>";

            var input = new XmlDocument { PreserveWhitespace = true };
            input.LoadXml(inputXml);

            var signatureElement = input.
                GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl).
                OfType<XmlElement>().
                First();

            var signedXml = new SignedXml(signatureElement);
            signedXml.LoadXml(signatureElement);

            foreach (var k in ClickOnce.Manifest.GetKeyInfoKeys(signedXml.KeyInfo))
            {
                Console.WriteLine(k.ToString());
            }

            var signatureCertificate = signedXml.KeyInfo.
                OfType<KeyInfoX509Data>().
                SelectMany(ki => ki.Certificates.OfType<System.Security.Cryptography.X509Certificates.X509Certificate2>()).
                First();

            bool validSignature = signedXml.CheckSignature(signatureCertificate.PublicKey.Key);
            Assert.True(validSignature);
        }
    }
}
