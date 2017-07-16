using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnce
{
    /// <summary>
    /// Class that holds actuall application
    /// </summary>
    public class Application
    {
        private ILog _console;
        /// <summary>
        /// Console logger
        /// </summary>
        public ILog Console
        {
            get { return _console ?? Tools.Console; }
            set { _console = value; }
        }

        /// <summary>
        /// Path used as base for ClickOnce
        /// </summary>
        public string BasePath = Tools.LibraryLocation;
        /// <summary>
        /// Library named pointing to application if no name is defines for files
        /// </summary>
        public string BaseLibName;

        /// <remarks/>
        public string LastLoadedSource;
        /// <remarks/>
        public Manifest LastLoadedApp;
        /// <remarks/>
        public Manifest LastLoadedManifest;

        private EntryPoint _entry;
        /// <remarks/>
        public EntryPoint Entry { get { return _entry; } }

        /// <summary>
        /// Expected Total size in bytes of dependencys according to manifest
        /// </summary>
        public long TotalSizeByManifest { get { return _allDeps.Where(d => d.Codebase != null).Sum(d => d.Size); } }

        /// <summary>
        /// Load from .appref-ms or .application if it has a deployment path
        /// </summary>
        public void Load(string src)
        {
            LastLoadedSource = src;
            if (Path.GetExtension(src) == ".appref-ms")
            {
                string contents = File.ReadAllText(src);
                string newPath = contents.Substring(0, contents.IndexOf("#"));
                Load(newPath);
                return;
            }
            var asm = new Manifest(src);
            LastLoadedApp = asm;
            var depAsm = asm.GetDeployment();
            LastLoadedApp = depAsm;
            _entry = depAsm.Entry;
            Load(depAsm);
            _entry.LocalPath = Path.Combine(BasePath, _entry.LocalLibName());
        }

        /// <remarks/>
        public void CopyDependencysLocal()
        {
            WaitAllTasks();
            foreach (var dep in _allDeps)
            {
                string dstPath = Path.Combine(_entry.LocalPath, dep.Codebase);
                if (!File.Exists(dstPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(dstPath));
                    File.Copy(dep.LastSeenLocalPath, dstPath);
                }
                // TODO add option to validate files on start
                // dep.Validate(dstPath);
            }
        }

        private List<Dependency> _allDeps = new List<Dependency>();

        /// <remarks/>
        public List<Dependency> Dependencys { get { return _allDeps; } }

        private List<Task<string>> allDepTasks = new List<Task<string>>();

        private void WaitAllTasks()
        {
            try
            {
                Task.WaitAll(allDepTasks.ToArray());
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten();
            }
        }

        private void Load(Manifest depAsm)
        {
            if (BaseLibName == null)
                BaseLibName = depAsm.LocalLibName();
            //depAsm.VerifySignature();
            Dependency chainTo = null;
            foreach (var d in depAsm.GetDependencys())
            {
                // Ignore dependencies that should exist locally
                if (d.Codebase == null)
                    continue;
                if (d.Codebase.EndsWith(".manifest") && chainTo == null)
                    chainTo = d;
                string libName = d.LocalLibName() ?? BaseLibName;
                string remote = d.RemoteUri();
                Console.WriteLine($"  Name: {libName} Save to: {Path.GetFileName(d.Codebase)} Size: {d.Size}");
                if (!string.IsNullOrEmpty(remote))
                    Console.WriteLine($"  From remote: {remote}");

                _allDeps.Add(d);
                string p = Path.Combine(BasePath, libName);
                allDepTasks.Add(d.GetToLocalPathAsync(p).
                    ContinueWith(t => {
                        if (t.Exception != null)
                            throw t.Exception.Flatten();
                        Console.WriteLine($" - Download done {t.Result.Replace(BasePath, "...")}");
                        return t.Result;
                    }));
            }

            if (chainTo != null)
            {
                WaitAllTasks();
                string remote = chainTo.RemoteUri();
                Console.WriteLine($" Found next stage to load {chainTo} @ {remote}");

                // This is used in recursive Load below
                BaseLibName = chainTo.LocalLibName() ?? BaseLibName;
                string path = chainTo.LastSeenLocalPath;
                if (path == null)
                    throw new ArgumentNullException("LastSeenLocalPath", "Download might have failed");

                var app = new Manifest(remote, path);
                Console.WriteLine($" Loaded {app.Name}");
                LastLoadedManifest = app;
                app.MapFileExtensions = depAsm.MapFileExtensions;
                Load(app);
                _entry.Import(app.Entry);
            }
        }

    }
}
