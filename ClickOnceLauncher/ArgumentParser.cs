using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnceLauncher
{
    /// <summary>
    /// Exception for parsing issues
    /// </summary>
    public class ArgumentParserException : Exception
    {
        /// <remarks/>
        public ArgumentParserException(string message) :
            base(message)
        {
        }
        /// <remarks/>
        public ArgumentParserException(string message, Exception exception) :
            base(message, exception)
        {
        }
    }

    /// <summary>
    /// Contains Commandline parser and handling
    /// </summary>
    public class ArgumentParser
    {
        /// <summary>
        /// First file argument
        /// </summary>
        public string AppRef;

        /// <summary>
        /// Show Help selected
        /// </summary>
        public bool Help;

        /// <summary>
        /// If given prevents launching of app
        /// </summary>
        public bool NoLaunch;

        /// <summary>
        /// BasePath that app should be downloaded to, libname will be added
        /// </summary>
        public string Path;

        /// <summary>
        /// Extra params to be passed on
        /// </summary>
        public readonly List<string> ExtraParams = new List<string>();

        /// <summary>
        /// Helptext for arguments
        /// </summary>
        public string HelpText
        {
            get
            {
                string appName = System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
                return
                    "Launches ClickOnce .appref-ms files\n" +
                    "Usage:\n" +
                    $"  {appName} --help\n" +
                    $"  {appName} application.appref-ms [--NoLaunch] [--Path alternative data path]\n" +
                    $"  {appName} application.appref-ms [--Path alternative data path] [-- arguments to pass on]\n" +
                    "Options:\n" +
                    "  --help\t Shows this help\n" +
                    "  --NoLaunch\t Only downloads program without launching\n" +
                    "  --Path path\t Set alternative path to store files, default is in your local AppData path\n" +
                    "  -- arguments\t terminates commandline and passes the rest on to the actuall ClickOnce Application\n";
            }
        }

        /// <summary>
        /// Parses the commandline
        /// </summary>
        public void Parse(string[] args)
        {
            // This should be more dynamic, but for now theree is to little time
            string state = null;
            foreach (string arg in args)
            {
                if (string.IsNullOrEmpty(arg))
                    continue;

                switch (arg.ToLower())
                {
                    case "--help":
                        Help = true;
                        break;
                    case "--nolaunch":
                        NoLaunch = true;
                        break;
                    case "--path":
                    case "--":
                        state = arg.ToLower();
                        break;
                    default:
                        // we assume this is a file
                        try
                        {
                            var fi = new System.IO.FileInfo(arg);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentParserException($"Argument invalid as app file '{arg}'", ex);
                        }

                        switch (state)
                        {
                            case "--path":
                                if (Path != null)
                                    throw new ArgumentParserException($"Multiple Path arguments '{arg}'");
                                Path = arg;
                                state = null;
                                break;

                            case "--":
                                if (!string.IsNullOrEmpty(arg))
                                    ExtraParams.Add(arg);
                                break;

                            case null:
                                // only allow for one file
                                if (AppRef != null)
                                    throw new ArgumentParserException($"Multiple app arguments '{arg}'");

                                if (!arg.ToLower().EndsWith(".appref-ms") &&
                                    !arg.ToLower().EndsWith(".application"))
                                    throw new ArgumentParserException($"File not .appref-ms or .applciation '{arg}'");

                                AppRef = arg;
                                break;
                            default:
                                throw new NotImplementedException($"State '{state}' not implemented");
                        }
                        break;
                }
            }
            if (state != null && 
                state != "--")
                throw new ArgumentParserException($"Non handled argument '{state}'");

            if (Help)
                return;

            if (string.IsNullOrEmpty(AppRef))
                throw new ArgumentParserException("Missing .appref-ms file argument");
        }
    }
}
