using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClickOnceLauncher.Tests
{
    public class ArgumentParserTests
    {
        [Fact]
        public void ShowHelp()
        {
            var parser = new ArgumentParser();
            Console.WriteLine(parser.HelpText);
        }

        [Theory]
        [InlineData("", "Missing .appref-ms file argument")]
        [InlineData("--help", null)]
        [InlineData(@"c:\data\test.appref-ms", @"c:\data\test.appref-ms")]
        [InlineData(@"c:\data\test.appref-ms file2.appref-ms", "Multiple app arguments 'file2.appref-ms'")]
        [InlineData(@" --NoLaunch --Path c:\app", "Missing .appref-ms file argument")]
        [InlineData(@":", "Argument invalid as app file ':'")]
        [InlineData(@"test.txt", "File not .appref-ms or .applciation 'test.txt'")]
        [InlineData(@"test.appref-ms --NoLaunch", "test.appref-ms")]
        [InlineData(@"test.application --NoLaunch --Path c:\app", "test.application")]
        [InlineData(@"test.appref-ms --NoLaunch --Path", "Non handled argument '--path'")]
        [InlineData(@"--NoLaunch --Path c:\app test.appref-ms", "test.appref-ms")]
        [InlineData(@"c:\data\test.appref-ms -- extra", @"c:\data\test.appref-ms")]
        public void GetAppRefFileFromArgsTest(string arg, string expectedFile)
        {
            // would like to send args directly, but Xunit and the compiler won't co-opperate
            string[] args = arg.Split(' ');
            var parser = new ArgumentParser();
            string result;
            try
            {
                parser.Parse(args);
                result = parser.AppRef;
            }
            catch (ArgumentParserException ex)
            {
                Console.WriteLine(ex.ToString());
                result = ex.Message;
            }
            Assert.Equal(expectedFile, result);
        }

        [Theory]
        [InlineData(@"c:\data\test.appref-ms", false)]
        [InlineData("test.appref-ms --NoLaunch", true)]
        [InlineData("test.appref-ms --nolaunch", true)]
        [InlineData("test.appref-ms --nolaunch -- e", true)]
        public void GetNoLaunchOptionFromArgsTest(string arg, bool expectNoLaunch)
        {
            // would like to send args directly, but Xunit and the compiler won't co-opperate
            string[] args = arg.Split(' ');
            var parser = new ArgumentParser();
            parser.Parse(args);
            Assert.Equal(expectNoLaunch, parser.NoLaunch);
        }

        [Theory]
        [InlineData(@"c:\data\test.appref-ms", null)]
        [InlineData(@"test.appref-ms --Path c:\app --NoLaunch", @"c:\app")]
        [InlineData(@"--Path c:\app test.appref-ms", @"c:\app")]
        [InlineData(@"--Path c:\app test.appref-ms -- extra", @"c:\app")]
        public void GePathOptionFromArgsTest(string arg, string expectedPath)
        {
            // would like to send args directly, but Xunit and the compiler won't co-opperate
            string[] args = arg.Split(' ');
            var parser = new ArgumentParser();
            parser.Parse(args);
            Assert.Equal(expectedPath, parser.Path);
        }

        [Theory]
        [InlineData(@"c:\data\test.appref-ms", "")]
        [InlineData(@"test.appref-ms --Path c:\app --NoLaunch", "")]
        [InlineData(@"--Path c:\app test.appref-ms --", @"")]
        [InlineData(@"--Path c:\app test.appref-ms -- ", @"")]
        [InlineData(@"--Path c:\app test.appref-ms -- extra extra", @"extra extra")]
        public void GeExtraParamsOptionFromArgsTest(string arg, string expectedPath)
        {
            // would like to send args directly, but Xunit and the compiler won't co-opperate
            string[] args = arg.Split(' ');
            var parser = new ArgumentParser();
            parser.Parse(args);
            Assert.Equal(expectedPath, string.Join(" ", parser.ExtraParams));
        }
    }
}
