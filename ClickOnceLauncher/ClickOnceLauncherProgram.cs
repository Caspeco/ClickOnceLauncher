using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnceLauncher
{
    /// <summary>
    /// Main application
    /// </summary>
    public static class ClickOnceLauncherProgram
    {
        static int Main(string[] args)
        {
            var parser = new ArgumentParser();

            try
            {
                parser.Parse(args);
                if (parser.Help)
                {
                    Console.WriteLine(parser.HelpText);
                    return 0;
                }
            }
            catch (ArgumentParserException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(parser.HelpText);
                return -1;
            }
            // make sure file exists and is available
            var fi = new FileInfo(parser.AppRef);
            if (!fi.Exists)
            {
                Console.WriteLine($"File not found {fi.FullName}");
                return -2;
            }

            return HandleAppLaunch(parser);
        }

        private static int HandleAppLaunch(ArgumentParser parser)
        {
            var app = new ClickOnce.Application();
            app.BasePath = parser.Path ?? ClickOnce.Tools.LibraryLocation;
            Console.WriteLine($" Using basepath: {app.BasePath}");

            app.Load(parser.AppRef);
            var depAsm = app.LastLoadedManifest;

            var deps = app.Dependencys;
            Console.WriteLine($" TotalSize: {app.TotalSizeByManifest} for total {deps.Count} dependencys");
            var entry = app.Entry;

            string baseLibName = app.BaseLibName;

            app.CopyDependencysLocal();

            foreach (var fi in entry.GetType().GetFields())
            {
                Console.WriteLine($" Entry: {fi.Name} val: {fi.GetValue(entry)}");
            }
            string entryPath = Path.Combine(app.BasePath, entry.LocalLibName());
            string entryExe = Path.Combine(entryPath, entry.Executable);
            Console.WriteLine($" Run {entryExe} {entry.Parameters}");

            if (parser.NoLaunch)
                return 0;

            string arguments = entry.Parameters;
            if (parser.ExtraParams.Count > 0)
            {
                arguments = string.Join(" ", parser.ExtraParams.Select(a => a.Contains(" ") ? $"\"{a}\"" : a));
                Console.WriteLine($" Will use passthru arguments {arguments}");
            }

            var startInfo = new ProcessStartInfo(entryExe);
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = entryPath;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = arguments;

            using (var exeProcess = Process.Start(startInfo))
            {
                // we will only wait for a little bit
                // TODO add option for if we should wait or not
                exeProcess.WaitForExit(1000);
                if (exeProcess.HasExited)
                    return exeProcess.ExitCode;
            }
            return 0;
        }
    }
}
