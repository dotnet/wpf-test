// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MS.Internal
{
    class WpfSetup
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Install WPF bits for test environments.");

            try
            {
                System.Environment.GetEnvironmentVariable("Homepath");
            }
            catch (System.Security.SecurityException)
            {
                Console.WriteLine("Error: WpfSetup has insufficient permissions.\nTo install from the network, run WpfRemoteSetup.cmd.");
                return 2;
            }

            WpfSetup app = new WpfSetup();

            int rc = app.ParseCmdLine(args);
            if (rc != 0)
                return rc;

            try
            {
                rc = app.Run();
                if (rc != 0)
                    app.SaveLogFiles();
                return rc;

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}\n{1}", e.GetType().FullName, e.ToString());
                app.SaveLogFiles();
                return 1;
            }
        }

        //---------------------------------------------------------------------
        public WpfSetup()
        {
            _components = new List<Component>();
            _components.Add(new MSXML6("msxml6"));
            _components.Add(new Rgb9Rast("Rgb9Rast"));
            _components.Add(new TestCert("TestCert"));
            _components.Add(new WIC("WIC"));
            _components.Add(new XPSEPSC("XPS-EPSC"));
            _components.Add(new WPF("WPF"));
        }

        //---------------------------------------------------------------------
        private int  Usage()
        {
            Console.WriteLine("Usage:\n");
            Console.WriteLine("wpfSetup <option>");
            Console.WriteLine("  -i        Install WPF and any system prerequisite components");
            Console.WriteLine("  -r path   remote path to WPF install components; used with -i");
            Console.WriteLine("  -f        force install; will reinstall components even if already present");
            Console.WriteLine("  -u        Uninstall WPF only, leave system prerequisite components installed");
            Console.WriteLine("  -c        Complete uninstall of WPF and system prereq components");
            Console.WriteLine("  -s        Status and versions of WPF and system prereq components (default)");
            Console.WriteLine("  -l        List expected installer files");
            Console.WriteLine("  -h        this help");
            Console.WriteLine("  -PrivateOnVista  Sets up DLL redirection using these bits (only on Vista)");
            Console.WriteLine("");
            Console.WriteLine("wpfSetup.exe cannot be launched from a remote share, copy this exe locally");
            Console.WriteLine(" and launch local copy:");
            Console.WriteLine("wpfsetup -i -r \\\\server\\shareWithWPFbits");

            return 10;
        }

        //---------------------------------------------------------------------
        private int ParseCmdLine(string[] args)
        {
            _mode = Mode.Status;
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg[0] == '-' || arg[0] == '/')
                {
                    // must be an option switch (with opt. parameter)
                    string option = arg.Substring(1);
                    switch (option.ToUpper())
                    {
                        case "H":
                        case "?":
                            return Usage();
                        case "I":
                            _mode = Mode.Install;
                            break;
                        case "F":
                            _forceInstall = true;
                            break;
                        case "U":
                            _mode = Mode.Uninstall;
                            break;
                        case "C":
                            _mode = Mode.Cleanup;
                            break;
                        case "S":
                            _mode = Mode.Status;
                            break;
                        case "L":
                            _mode = Mode.ListInstallers;
                            break;
                        case "R":
                            if (i < (args.Length - 1))
                            {
                                string path = args[++i];
                                if (!Directory.Exists(path))
                                {
                                    Console.WriteLine("ERROR: path to install components does not exist:\n{0}", path);
                                    return 1;
                                }
                                RemoteBasePath = path;
                            }
                            else
                                return Usage();
                            break;
                        case "N":
                            NoExecute = true;
                            break;
                        case "PRIVATEONVISTA":
                            _components.Add(new VistaTestSetup("DLL Redirect"));
                            break;
                        default:
                            Console.WriteLine("Unknown command line parameter: {0}", arg);
                            return Usage();
                    }
                }
            }
            return 0;
        }

        //---------------------------------------------------------------------
        internal int Run()
        {
            if (WpfSetup.NoExecute)
                Console.WriteLine("NoExecute=true, will skip actual setup");

            int exitCode = 0;

            switch (_mode)
            {
                // install system prereq and WPF components
                case Mode.Install:
                    exitCode = InstallAll();
                    break;

                // uninstall in reverse install order; skip system prereq components
                case Mode.Uninstall:
                    exitCode = UninstallAll(false);
                    break;

                // uninstall in reverse install order; uninstall WPF and all system prereq components
                case Mode.Cleanup:
                    exitCode = UninstallAll(true);
                    break;

                case Mode.Status:
                    exitCode = ShowStatus();
                    break;

                case Mode.ListInstallers:
                    exitCode = ListComponents();
                    break;
            }
            return exitCode;
        }

        //---------------------------------------------------------------------
        private int InstallAll()
        {
            HostOS hostOS = Component.HostEnvironment.hostOS;
            foreach (Component comp in _components)
            {
                Console.WriteLine("Installing {0} ...", comp.Name);
                if (!comp.InstallOnThisOS(hostOS))
                {
                    Console.WriteLine("component '{0}' is not necessary for this OS: {1}, skipping install", comp.Name, hostOS.ToString());
                    continue;
                }

                if (comp.IsInstalled() && !_forceInstall && !comp.ForceOperation)
                {
                    Console.WriteLine("component '{0}' already installed; found version = {1}", comp.Name, comp.GetVersion());
                    continue;
                }
                int exitCode = comp.Install();
                if (exitCode != 0)
                {
                    Console.WriteLine("Component {0} failed to install, aborting!", comp.Name);
                    return exitCode;
                }
            }
            return 0;
        }

        //---------------------------------------------------------------------
        private int UninstallAll(bool removeSysPrereq)
        {
            HostOS hostOS = Component.HostEnvironment.hostOS;
            List<Component> components = _components;
            components.Reverse();
            foreach (Component comp in components)
            {
                if (comp.IsSystemPrerequisite && !removeSysPrereq)
                    continue;   // skip this component if this is not a cleanup run

                Console.WriteLine("Uninstalling {0} ...", comp.Name);
                if (!comp.InstallOnThisOS(hostOS))
                {
                    Console.WriteLine("component '{0}' is not necessary for this OS: {1}, uninstall skipped", comp.Name, hostOS.ToString());
                    continue;
                }

                if (!comp.IsInstalled() && !comp.ForceOperation)
                {
                    Console.WriteLine("component '{0}' already uninstalled", comp.Name);
                    continue;
                }
                // ignore failure exit codes when uninstalling
                comp.Uninstall();
            }
            return 0;
        }

        //---------------------------------------------------------------------
        private int ShowStatus()
        {
            HostOS hostOS = Component.HostEnvironment.hostOS;
            foreach (Component comp in _components)
            {
                Console.Write("{0, -15}: ", comp.Name);
                if (comp.InstallOnThisOS(hostOS))
                {
                    bool isInstalled = comp.IsInstalled();
                    Console.WriteLine("Installed: {0}  Version: {1}",
                        isInstalled ? "Y" : "N", isInstalled ? comp.GetVersion() : "-");
                }
                else
                {
                    Console.WriteLine("Skipped, not required on this OS: {0}", hostOS.ToString());
                    continue;
                }

            }
            return 0;
        }

        //---------------------------------------------------------------------
        private int ListComponents()
        {
            HostOS hostOS = Component.HostEnvironment.hostOS;
            foreach (Component comp in _components)
            {
                Console.Write("{0, -15}: ", comp.Name);
                if (comp.InstallOnThisOS(hostOS))
                {
                    Console.Write("File: ");
                    foreach (string file in comp.InstallerFiles)
                        Console.Write("{0} ", file);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Skipped, not required on this OS: {0}", hostOS.ToString());
                    continue;
                }
            }
            return 0;
        }

        //---------------------------------------------------------------------
        // save any log files to cinch's DRT_LOGS folder so they get transferred
        // back to \\longcut\store
        private void SaveLogFiles()
        {
            string drtLogs = Environment.GetEnvironmentVariable("DRT_LOGS");
            if (!String.IsNullOrEmpty(drtLogs) && Directory.Exists(drtLogs))
            {
                try
                {
                    string targetDir = Path.Combine(drtLogs, "wpfSetup-logs");
                    Console.WriteLine("'DRT_LOGS' is set, saving log files to: " + targetDir);
                    Directory.CreateDirectory(targetDir);
                    foreach (Component comp in _components)
                    {
                        if (!String.IsNullOrEmpty(comp.LastLogFile))
                        {
                            string logFile = comp.LastLogFile;
                            Console.WriteLine("'copying " + logFile);
                            string targetFile = Path.Combine(targetDir, Path.GetFileName(logFile));
                            File.Copy(comp.LastLogFile, targetFile, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR while copying log files:\n{0}", e.ToString());
                }
            }

        }

        //---------------------------------------------------------------------
        enum Mode
        {
            Install,
            Uninstall,
            Cleanup,
            Status,
            ListInstallers,
        }

        /// <summary>
        /// if true, only show command lines as they would be executed
        /// </summary>
        static internal bool NoExecute = false;

        /// <summary>
        /// if non-empty, alternate BasePath used to find component binaries
        /// </summary>
        static internal string RemoteBasePath = String.Empty;

        private Mode _mode;
        private List<Component> _components;
        private bool _forceInstall;
    }
}

