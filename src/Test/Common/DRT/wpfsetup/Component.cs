// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace MS.Internal
{
    //=========================================================================
    /// <summary>
    /// WPF.msi
    /// </summary>
    internal class WPF : Component
    {
        //---------------------------------------------------------------------
        public WPF(string name) : base(name)
        {
            HostCPU cpu = HostEnvironment.hostCPU;
            switch (cpu)
            {
                case HostCPU.x86:
                    _msiFile = "wpf.msi";
                    break;
                case HostCPU.x64:
                    _msiFile = "wpf_x64.msi";
                    break;
                default:
                    throw new NotSupportedException("WPF.msi not supported for this CPU: " + cpu.ToString());
            }
        }

        //---------------------------------------------------------------------
        public override bool IsSystemPrerequisite
        {
            get { return false; }
        }

        //---------------------------------------------------------------------
        public override int Install()
        {
            Console.WriteLine("Install.....");

            string msi = Path.Combine(BasePath, InstallerFiles[0]);
            _lastLogFile = Path.Combine(LogFilePath, "WPF.install.log");

            if (SkipVerification(msi))
            {
                // the MSI is not signed, we need to set that special SN key
                // before doing the real installation.
                SNKey.Create(HostEnvironment.hostCPU);
            }

            StringBuilder additionalCmdLineArgs = new StringBuilder("STANDALONE=yes");
            
            if (HostEnvironment.hostOS == HostOS.Vista)
            {
                additionalCmdLineArgs.Append(" OSOVERRIDE=yes");
            }
            
            return InstallMSI(msi, _lastLogFile, additionalCmdLineArgs.ToString());
        }

        //---------------------------------------------------------------------
        public override int Uninstall()
        {
            // try finding file under the install directory
            string msi = Path.Combine(InstallRoot, InstallerFiles[0]);
            _lastLogFile = Path.Combine(LogFilePath, "WPF.uninstall.log");

            if (SkipVerification(msi))
            {
                // The MSI is not signed, we need to remove the special SN key
                // which was set when the msi was installed.

                SNKey.Remove(HostEnvironment.hostCPU);
            }

            string additionalCmdLineArgs = String.Empty;
            
            if (HostEnvironment.hostOS == HostOS.Vista)
            {
                additionalCmdLineArgs = "OSOVERRIDE=yes";
            }

            return UninstallMSI(msi, _lastLogFile, additionalCmdLineArgs);
        }

        //---------------------------------------------------------------------
        public override string GetVersion()
        {
            RegistryKey wpfKey = WpfRegKey;
            if (wpfKey != null)
            {
                object value = wpfKey.GetValue("ProductVersion");
                return (value != null) ? value.ToString() : String.Empty;
            }
            return String.Empty;
        }

        //---------------------------------------------------------------------
        public override string[] InstallerFiles
        {
            get { return new string[] { _msiFile }; }
        }

        //---------------------------------------------------------------------
        public override bool IsInstalled()
        {
            return !String.IsNullOrEmpty(InstallRoot);
        }

        //---------------------------------------------------------------------
        private string InstallRoot
        {
            get
            {
                if (String.IsNullOrEmpty(_installRoot))
                {
                    RegistryKey wpfKey = WpfRegKey;
                    if (wpfKey != null)
                    {
                        object value = wpfKey.GetValue("InstallRoot");
                        _installRoot = (value != null) ? value.ToString() : String.Empty;
                    }
                    else
                        _installRoot = String.Empty;
                }
                return _installRoot;
            }
        }

        //---------------------------------------------------------------------
        private RegistryKey WpfRegKey
        {
            get
            {
                return Registry.LocalMachine.OpenSubKey(s_WpfRegPath);
            }
        }

        //
        // Read the msi and check if this MSI is not signed and then requires SkipVerification
        // for the strong name validation.
        //
        private bool SkipVerification(string msiPath)
        {

            bool bSkip = false;   // By default, it should not skip the strong name validation.

            // Since we always sign the assemblies with test key, and that test key is 
            // always put in the SN validation skip registry on Vista, there is no need 
            // to specially handle the SN key on Vista.

            if (HostEnvironment.hostOS == HostOS.Vista)
                return false;

            IntPtr hDatabase;

            //
            // Open the Database for Read-Only.
            //
            // According to MsiQuery.h,
            //    MSIDBOPEN_READONLY     0
            //    MSIDBOPEN_TRANSACT     1
            //    MSIDBOPEN_DIRECT       2
            //    MSIDBOPEN_CREATE       3
            //    MSIDBOPEN_CREATEDIRECT 4
            //

            uint uRet = MsiOpenDatabase(msiPath, (IntPtr)0, out hDatabase);

            if (uRet != 0)
            {
                // Error occurs. Report error and stop the setup.
                throw new Exception("Cannot open database from " + msiPath + ", return value is " + uRet.ToString());
            }

            //
            // Query the value for "SkipVerification" from Property table.
            //

            IntPtr hView;
            uRet = MsiDatabaseOpenView(hDatabase, "SELECT * FROM Property WHERE Property='SkipVerification'", out hView);

            if (uRet != 0)
            {
                // Error occurs. Report error and stop the setup.
                throw new Exception("Cannot open Property table from " + msiPath + ", return value is " + uRet.ToString());
            }

            IntPtr hRecord;
            MsiViewExecute(hView, IntPtr.Zero);
            uRet = MsiViewFetch(hView, out hRecord);

            if (uRet != 0)
            {
                // Error occurs. Report error and stop the setup.
                throw new Exception("Cannot get record for 'SkipVerification' from the Property table in " + msiPath + ", return value is " + uRet.ToString());
            }

            uint cchValueBuf = 10;  // The value is either "yes" or "no", so just put a hard code here.
            StringBuilder szValueBuf = new StringBuilder((int)cchValueBuf + 1);

            MsiRecordGetString(hRecord, 2, szValueBuf, ref cchValueBuf);
            MsiCloseHandle(hRecord);

            if (szValueBuf != null && szValueBuf.ToString() == "yes")
            {
                bSkip = true;
            }

            MsiCloseHandle(hView);
            MsiCloseHandle(hDatabase);

            return bSkip;

        }

        private static string s_WpfRegPath = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup\Windows Presentation Foundation";
        private string _msiFile;
        private string _installRoot;
    }


    //=========================================================================
    /// <summary>
    /// WIC
    /// </summary>
    internal class WIC : Component
    {
        //---------------------------------------------------------------------
        public WIC(string name) : base(name)
        {
            string arch;
            switch (HostEnvironment.hostCPU)
            {
                case HostCPU.x86:
                    arch = "X86";
                    break;
                case HostCPU.x64:
                    arch = "X64";
                    break;
                default:
                    throw new NotSupportedException("WIC_update not supported for this CPU: " + HostEnvironment.hostCPU.ToString());
            }
            // file naming schema: WIC_X86_ENU.exe
            _updateExe = String.Format("WIC_{0}_ENU.exe", arch);
        }

        //---------------------------------------------------------------------
        public override bool IsSystemPrerequisite
        {
            get { return false; }
        }

        //---------------------------------------------------------------------
        // do not install WIC on Vista
        public override bool InstallOnThisOS(HostOS hostOS)
        {
            return (hostOS != HostOS.Vista);
        }

        //---------------------------------------------------------------------
        public override int Install()
        {
            _lastLogFile = Path.Combine(LogFilePath, "WIC.install.log");
            string wicExe = Path.Combine(BasePath, InstallerFiles[0]);
            return RunUpdateExe(wicExe, _lastLogFile);
        }

        //---------------------------------------------------------------------
        public override int Uninstall()
        {
            if (String.IsNullOrEmpty(UninstallPath))
                return 0;
            // try finding file under the install directory
            _lastLogFile = Path.Combine(LogFilePath, "WIC.uninstall.log");
            return RunUpdateExe(UninstallPath, _lastLogFile);
        }

        //---------------------------------------------------------------------
        // returns the file version of WindowsCodecs.dll
        public override string GetVersion()
        {
            if (!IsInstalled())
                return String.Empty;

            string codecDLL = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                "WindowsCodecs.dll");

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(codecDLL);
            return info.FileVersion;
        }

        //---------------------------------------------------------------------
        public override string[] InstallerFiles
        {
            get { return new string[] { _updateExe } ; }
        }

        //---------------------------------------------------------------------
        public override bool IsInstalled()
        {
            return !String.IsNullOrEmpty(UninstallPath);
        }

        //---------------------------------------------------------------------
        private string UninstallPath
        {
            get
            {
                if (String.IsNullOrEmpty(_uninstallPath))
                {
                    RegistryKey wicKey = Registry.LocalMachine.OpenSubKey(s_WicUninstallRegPath);
                    if (wicKey != null)
                    {
                        object value = wicKey.GetValue("UninstallString");
                        _uninstallPath = (value != null) ? value.ToString() : String.Empty;
                        // trim off any leading/trailing double quotes that are found in reg value
                        _uninstallPath = _uninstallPath.Trim('\"');
                    }
                    else
                        _uninstallPath = String.Empty;
                }
                return _uninstallPath;
            }
        }

        private static string s_WicUninstallRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\WIC";
        private string _updateExe;
        private string _uninstallPath;
    }


    //=========================================================================
    /// <summary>
    /// XPS Essential Pack Shared Components
    /// </summary>
    internal class XPSEPSC : Component
    {
        //---------------------------------------------------------------------
        public XPSEPSC(string name) : base(name)
        {
            string arch;
            switch (HostEnvironment.hostCPU)
            {
                case HostCPU.x86:
                    arch = "x86";
                    break;
                case HostCPU.x64:
                    arch = "amd64";
                    break;
                default:
                    throw new NotSupportedException("XPSEPSC_update not supported for this CPU: " + HostEnvironment.hostCPU.ToString());
            }
            // file naming schema: XPSEPSC-x86-en-US.exe
            _updateExe = String.Format("XPSEPSC-{0}-en-US.exe", arch);
        }

        //---------------------------------------------------------------------
        public override bool IsSystemPrerequisite
        {
            get { return true; }
        }

        //---------------------------------------------------------------------
        // do not install XPSEPSC on Vista
        public override bool InstallOnThisOS(HostOS hostOS)
        {
            return (hostOS != HostOS.Vista);
        }

        //---------------------------------------------------------------------
        public override int Install()
        {
            _lastLogFile = Path.Combine(LogFilePath, "XPSEPSC.install.log");
            string xpsExe = Path.Combine(BasePath, InstallerFiles[0]);
            return RunUpdateExe(xpsExe, _lastLogFile);
        }

        //---------------------------------------------------------------------
        public override int Uninstall()
        {
            if (String.IsNullOrEmpty(UninstallPath))
                return 0;
            // try finding file under the install directory
            _lastLogFile = Path.Combine(LogFilePath, "XPSEPSC.uninstall.log");
            return RunUpdateExe(UninstallPath, _lastLogFile);
        }

        //---------------------------------------------------------------------
        // returns the file version of XpsSvcs.dll
        public override string GetVersion()
        {
            if (!IsInstalled())
                return String.Empty;

            string codecDLL = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                "XpsSvcs.dll");

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(codecDLL);
            return info.FileVersion;
        }

        //---------------------------------------------------------------------
        public override string[] InstallerFiles
        {
            get { return new string[] { _updateExe } ; }
        }

        //---------------------------------------------------------------------
        public override bool IsInstalled()
        {
            return !String.IsNullOrEmpty(UninstallPath);
        }

        //---------------------------------------------------------------------
        private string UninstallPath
        {
            get
            {
                if (String.IsNullOrEmpty(_uninstallPath))
                {
                    RegistryKey xpsKey = Registry.LocalMachine.OpenSubKey(s_XpsUninstallRegPath);
                    if (xpsKey != null)
                    {
                        object value = xpsKey.GetValue("UninstallString");
                        _uninstallPath = (value != null) ? value.ToString() : String.Empty;
                        // trim off any leading/trailing double quotes that are found in reg value
                        _uninstallPath = _uninstallPath.Trim('\"');
                    }
                    else
                        _uninstallPath = String.Empty;
                }
                return _uninstallPath;
            }
        }

        private static string s_XpsUninstallRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\XpsEPSC";
        private string _updateExe;
        private string _uninstallPath;
    }

    //=========================================================================
    /// <summary>
    /// MSXML6 needed for MXDW/XPS printing
    /// </summary>
    internal class MSXML6 : Component
    {
        //---------------------------------------------------------------------
        public MSXML6(string name) : base(name)
        {
            HostCPU cpu = HostEnvironment.hostCPU;
            switch (cpu)
            {
                case HostCPU.x86:
                    _msiFile = "msxml6.msi";
                    break;
                case HostCPU.x64:
                    _msiFile = "msxml6_x64.msi";
                    break;
                default:
                    throw new NotSupportedException("MSXML6.msi not supported for this CPU: " + cpu.ToString());
            }
        }

        //---------------------------------------------------------------------
        public override bool IsSystemPrerequisite
        {
            get { return true; }
        }

        //---------------------------------------------------------------------
        // do not install msxml6 on Vista, already there
        public override bool InstallOnThisOS(HostOS hostOS)
        {
            return (hostOS != HostOS.Vista);
        }

        //---------------------------------------------------------------------
        public override int Install()
        {
            string msi = Path.Combine(BasePath, InstallerFiles[0]);
            _lastLogFile = Path.Combine(LogFilePath, "MSXML6.install.log");
            return InstallMSI(msi, _lastLogFile, String.Empty);
        }

        //---------------------------------------------------------------------
        public override int Uninstall()
        {
            HostCPU cpu = HostEnvironment.hostCPU;
            string package = "";
            switch (cpu)
            {
                case HostCPU.x86:
                    package = s_x86XmlUninstallPackage;
                    break;
                case HostCPU.x64:
                    package = s_x64XmlUninstallPackage;
                    break;
            }

            _lastLogFile = Path.Combine(LogFilePath, "MSXML6.uninstall.log");
            int errorCode = UninstallMSI(package, _lastLogFile, String.Empty);
            // some machines didn't install using the MSI but instead directly registered XML COM objects
            if (errorCode == 1605)
            {
                errorCode = UninstallRegSvr(InstallRoot);
            }
            return errorCode;
        }

        //---------------------------------------------------------------------
        public override string GetVersion()
        {
            if (!IsInstalled())
                return String.Empty;
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(InstallRoot);
            return info.FileVersion;
        }

        //---------------------------------------------------------------------
        public override string[] InstallerFiles
        {
            get { return new string[] { _msiFile }; }
        }

        //---------------------------------------------------------------------
        public override bool IsInstalled()
        {
            return !String.IsNullOrEmpty(InstallRoot) && File.Exists(InstallRoot);
        }

        //---------------------------------------------------------------------
        private string InstallRoot
        {
            get
            {
                if (String.IsNullOrEmpty(_installRoot))
                {
                    RegistryKey xmlDocServer = Registry.ClassesRoot.OpenSubKey(s_XmlDOMDoc6Server);
                    if (xmlDocServer != null)
                    {
                        object val = xmlDocServer.GetValue("");
                         _installRoot = (val != null) ? val.ToString() : String.Empty;
                    }
                }
                return _installRoot;
            }
        }

        private static string s_XmlDOMDoc6Server = @"CLSID\{88d96a05-f192-11d4-a65f-0040963251e5}\InProcServer32";

        private static string s_x86XmlUninstallPackage = @"{AEB9948B-4FF2-47C9-990E-47014492A0FE}";
        private static string s_x64XmlUninstallPackage = @"{FF59CB23-1800-4047-B40C-E20AE7051491}";

        private string _msiFile;
        private string _installRoot;
    }

    //=========================================================================
    /// <summary>
    /// rgb9rast - software renderer
    /// </summary>
    internal class Rgb9Rast : Component
    {
        //---------------------------------------------------------------------
        public Rgb9Rast(string name)
            : base(name)
        {
            HostCPU cpu = HostEnvironment.hostCPU;
            switch (cpu)
            {
                case HostCPU.x86:
                    _msiFile = "Rgb9Rast_x86.msi";
                    break;
                case HostCPU.x64:
                    _msiFile = "Rgb9Rast_x64.msi";
                    break;
                default:
                    throw new NotSupportedException("Rgb9Rast.msi not supported for this CPU: " + cpu.ToString());
            }
        }

        //---------------------------------------------------------------------
        public override bool IsSystemPrerequisite
        {
            get { return true; }
        }

        //---------------------------------------------------------------------
        // do not install msxml6 on Vista, already there
        public override bool InstallOnThisOS(HostOS hostOS)
        {
            return (hostOS != HostOS.Vista);
        }

        //---------------------------------------------------------------------
        public override int Install()
        {
            string msi = Path.Combine(BasePath, InstallerFiles[0]);
            _lastLogFile = Path.Combine(LogFilePath, "Rgb9Rast.install.log");
            return InstallMSI(msi, _lastLogFile, String.Empty);
        }

        //---------------------------------------------------------------------
        public override int Uninstall()
        {
            // NOTE: rgb9rast.msi doesn't keep a local copy of itself
            // nor does it leave any uninstall entry in the registry;
            // for the time being, Uninstall() just succeeds as a NOP
            Console.WriteLine("rgb9rast: no uninstall possible, skipped.");
            return 0;   // always succeed

            //// try finding file under the install directory
            //string msi = Path.Combine(InstallRoot, InstallerFiles[0]);
            //_lastLogFile = Path.Combine(LogFilePath, "Rgb9Rast.uninstall.log");
            //return UninstallMSI(msi, _lastLogFile);
        }

        //---------------------------------------------------------------------
        public override string GetVersion()
        {
            if (!IsInstalled())
                return String.Empty;
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(InstallRoot);
            return info.FileVersion;
        }

        //---------------------------------------------------------------------
        public override string[] InstallerFiles
        {
            get { return new string[] { _msiFile }; }
        }

        //---------------------------------------------------------------------
        public override bool IsInstalled()
        {
            return !String.IsNullOrEmpty(InstallRoot) && File.Exists(InstallRoot);
        }

        //---------------------------------------------------------------------
        private string InstallRoot
        {
            get
            {
                if (String.IsNullOrEmpty(_installRoot))
                {
                    HostOS hostOS = HostEnvironment.hostOS;

                    string rastDLL = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),
                        (hostOS != HostOS.Vista) ? "RGB9Rast_2.dll" : "RGB9Rast.dll");
                    _installRoot = File.Exists(rastDLL) ? rastDLL : String.Empty;
                }
                return _installRoot;
            }
        }

        private string _msiFile;
        private string _installRoot;
    }

    //=========================================================================
    /// <summary>
    /// Test Root Authority certificate
    /// </summary>
    internal class TestCert : Component
    {
        //---------------------------------------------------------------------
        public TestCert(string name) : base(name)
        {
        }

        //---------------------------------------------------------------------
        public override int Install()
        {
            string certName = InstallerFiles[0];
            string certFile = Path.Combine(BasePath, certName);
            if (!File.Exists(certFile))
            {
                throw new FileNotFoundException("Cannot find certificate file to install ", certFile);
            }
            X509Certificate cert = X509Certificate.CreateFromCertFile(certFile);
            X509Certificate2 _certificate = new X509Certificate2(cert);
            if (_certificate != null)
            {
                Console.WriteLine("> {0}", certName);
                if (WpfSetup.NoExecute)
                    return 0;

                CertStore.Add(_certificate);
                Console.WriteLine("< exitCode={0}", 0);
                return 0;
            }
            return 1;
        }

        //---------------------------------------------------------------------
        public override int Uninstall()
        {
            if (InstalledCertificate == null)
                return 1;
            Console.WriteLine("> {0}", InstallerFiles[0]);
            HostOS hostOS = HostEnvironment.hostOS;
            if (hostOS == HostOS.Vista)
            {
                Console.WriteLine("Internal builds of Vista require test cert, skipping uninstall");
                return 0;
            }
            if (WpfSetup.NoExecute)
                return 0;

            CertStore.Remove(InstalledCertificate);
            Console.WriteLine("< exitCode={0}", 0);
            _certificate = null;
            return 0;
        }

        //---------------------------------------------------------------------
        public override bool IsInstalled()
        {
            return (InstalledCertificate != null);
        }

        //---------------------------------------------------------------------
        public override string GetVersion()
        {
            return (InstalledCertificate != null) ? InstalledCertificate.IssuerName.Name : String.Empty;
        }

        //---------------------------------------------------------------------
        public override string[] InstallerFiles
        {
            get { return new string[] { s_testRootCert }; }
        }

        //---------------------------------------------------------------------
        private X509Certificate2 InstalledCertificate
        {
            get
            {
                if (_certificate == null)
                {
                    X509Certificate2Collection certs = CertStore.Certificates;
                    X509Certificate2Collection matching = certs.Find(X509FindType.FindBySerialNumber, s_testRootSerial, true);
                    if (matching.Count >= 1)
                        _certificate = matching[0];
                }
                return _certificate;
            }
        }

        //---------------------------------------------------------------------
        private X509Store CertStore
        {
            get
            {
                if (_store == null)
                {
                    _store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                    _store.Open(OpenFlags.ReadWrite);
                }
                return _store;
            }
        }

        //---------------------------------------------------------------------
        public override bool IsSystemPrerequisite { get { return true; } }

        private X509Store _store;
        private X509Certificate2 _certificate;
        private static string s_testRootCert = "testroot.cer";
        private static string s_testRootSerial = "5FEA4FD2F21D4310B6E8543ED8952618";
    }

    //=========================================================================
    /// Post-WPF-install step for Vista machines.  Sets up PresentationHost.exe to
    /// use the native DLLs from this install instead of those provided by Vista.
    /// Does this by setting the DevOverrideEnable registry key and creating a
    /// .local directory with the native DLLs for PresentationHost.exe.  This diretory
    /// can be reused via symbolic links to get other exes to use the set of native
    /// DLLs from this install.
    /// This step is unncessary for down-level machines.
    internal class VistaTestSetup : Component
    {
        //---------------------------------------------------------------------
        public VistaTestSetup(string name)
            : base(name)
        {
            _targetDirectory = Path.Combine(Environment.SystemDirectory, RedirectDirectory);
            _wowDirectory = Path.Combine(Environment.GetEnvironmentVariable("SYSTEMROOT"), "SysWOW64");
            _targetWOWDirectory = Path.Combine(_wowDirectory, RedirectDirectory);

            HostCPU cpu = HostEnvironment.hostCPU;
            switch (cpu)
            {
                case HostCPU.x86:
                    _wpfInstaller = "wpf.msi";
                    _wicInstaller = "wic_x86_enu.exe";

                    _dlls.Add("milcore_x86.dll");
                    _dlls.Add("uiautomationcore_x86.dll");

                    _sourceFiles.Add("milcore_x86.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "milcore.dll"));
                    _sourceFiles.Add("uiautomationcore_x86.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "uiautomationcore.dll"));
                    _sourceFiles.Add("windowscodecs.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "windowscodecs.dll"));
                    _sourceFiles.Add("windowscodec----t.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "windowscodec----t.dll"));
                    _sourceFiles.Add("wmphoto.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "wmphoto.dll"));
                    _sourceFiles.Add("photometadatahandler.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "photometadatahandler.dll"));
                    break;
                case HostCPU.x64:
                    _wpfInstaller = "wpf_x64.msi";
                    _wicInstaller = "wic_x64_enu.exe";
                                        
                    _dlls.Add("milcore_a64.dll");
                    _dlls.Add("uiautomationcore_a64.dll");
                    
                    // These are 64-bit versions of the DLLs - going into the default PrivateOnVista directory
                    _sourceFiles.Add("milcore_a64.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "milcore.dll"));
                    _sourceFiles.Add("uiautomationcore_a64.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "uiautomationcore.dll"));
                    _sourceFiles.Add("windowscodecs.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "windowscodecs.dll"));
                    _sourceFiles.Add("windowscodec----t.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "windowscodec----t.dll"));
                    _sourceFiles.Add("wmphoto.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "wmphoto.dll"));
                    _sourceFiles.Add("photometadatahandler.dll"); _targetFiles.Add(Path.Combine(_targetDirectory, "photometadatahandler.dll"));

                    // WOW version of the DLLs
                    _dlls.Add("milcore_x86.dll");
                    _dlls.Add("uiautomationcore_x86.dll");

                    // These are WOW versions of the DLLs - going into the WOW PrivateOnVista directory
                    _sourceFiles.Add("milcore_x86.dll"); _targetFiles.Add(Path.Combine(_targetWOWDirectory, "milcore.dll"));
                    _sourceFiles.Add("uiautomationcore_x86.dll"); _targetFiles.Add(Path.Combine(_targetWOWDirectory, "uiautomationcore.dll"));
                    _sourceFiles.Add("wwindowscodecs.dll"); _targetFiles.Add(Path.Combine(_targetWOWDirectory, "windowscodecs.dll"));
                    _sourceFiles.Add("wwindowscodec----t.dll"); _targetFiles.Add(Path.Combine(_targetWOWDirectory, "windowscodec----t.dll"));
                    _sourceFiles.Add("wwmphoto.dll"); _targetFiles.Add(Path.Combine(_targetWOWDirectory, "wmphoto.dll"));
                    _sourceFiles.Add("wphotometadatahandler.dll"); _targetFiles.Add(Path.Combine(_targetWOWDirectory, "photometadatahandler.dll"));
                    break;
                default:
                    throw new NotSupportedException("PrivateOnVista option not supported for this CPU: " + cpu.ToString());
            }
        }

        //---------------------------------------------------------------------
        public override int Install()
        {
            HostCPU cpu = HostEnvironment.hostCPU;
            try 
            {                
                string targetFile = null, sourceFile = null;

                ExtractDLLs();

                // Create PrivateOnVista directory 
                DirectoryInfo privateOnVistaDirectory = CreateDirectory(_targetDirectory);

                // On amd64, need to create PrivateOnVista for WOW too
                if (cpu == HostCPU.x64)
                {
                    CreateDirectory(_targetWOWDirectory);
                }

                // Copy all files to the PrivateOnVista directories
                for (int i = 0; i < _sourceFiles.Count; i++)
                {
                    sourceFile = Path.Combine(_tempDirectory, _sourceFiles[i]);
                    targetFile = _targetFiles[i];

                    Console.WriteLine("> Copying native DLL to private directory: " + _sourceFiles[i]);
                    if (!WpfSetup.NoExecute)
                    {
                        CopyFile(sourceFile, targetFile, false);
                    }
                }

                // Move PresentationHost.exe to PrivateOnVista directory
                sourceFile = Path.Combine(Environment.SystemDirectory, ExeToRedirect); 
                targetFile = Path.Combine(_targetDirectory, ExeToRedirect);

                Console.WriteLine("> Moving installed EXEs to private directory: " + ExeToRedirect);
                if (!WpfSetup.NoExecute)
                {
                    // If we are on 64-bit - we need two copies of the EXE
                    if (cpu == HostCPU.x64)
                    {
                        CopyFile(sourceFile, Path.Combine(_targetWOWDirectory, ExeToRedirect), false);
                    }
                    CopyFile(sourceFile, targetFile, true);
                }
                
                // Setup any installed EXEs to use the PrivateOnVista directory
                Console.WriteLine("> Setting up XPSViewer.exe to use private directory.");
                if (!WpfSetup.NoExecute)
                {
                    string targetDirectory, sourceDirectory;                    
                    switch (cpu)
                    {
                        case HostCPU.x86:
                            targetDirectory = Environment.SystemDirectory ;
                            sourceDirectory = _targetDirectory;
                            break;
                        case HostCPU.x64:
                            targetDirectory = _wowDirectory;
                            sourceDirectory = _targetWOWDirectory;
                            break;
                        default:
                            throw new Exception("Unsupported architecture.");
                    }
                    
                    MakeLink(Path.Combine(targetDirectory, @"XPSViewer\XPSViewer.exe.local"), sourceDirectory);
                }

                // Create the PresentationHost.exe.local file in PrivateOnVista directory
                if (!WpfSetup.NoExecute)
                {                
                    FileInfo exeLocalFile = new FileInfo(Path.Combine(_targetDirectory, ExeToRedirect + ".local"));
                    if (!exeLocalFile.Exists)
                    {
                        exeLocalFile.Create();
                        exeLocalFile.Refresh();
                    }

                    // Mark file read-only to prevent inadvertent deleting via symbolic link
                    exeLocalFile.Attributes = exeLocalFile.Attributes | FileAttributes.ReadOnly;
                }

                // Now set the directory to be read-only to prevent accidental deletion
                if (!WpfSetup.NoExecute)
                {
                    // Make the directory read-only to prevent inadvertent deleting via symbolic links
                    privateOnVistaDirectory.Attributes = privateOnVistaDirectory.Attributes | FileAttributes.ReadOnly;
                }

                InitRegistryKeys(true);

                // Set DevOverride registry key
                Console.WriteLine("> Setting " + DevOverride.Name + "!DevOverrideEnable to 1 (DWORD)");
                if (!WpfSetup.NoExecute)
                {
                    DevOverride.SetValue("DevOverrideEnable", 1, RegistryValueKind.DWord);

                    ModifyValue(XbapCLSID_HKLM, "LocalServer32", "", true);
                    ModifyValue(XbapCLSID_HKLM, "ServerExecutable", "", true);
                    ModifyValue(XbapPROGID, null, "FriendlyTypeName", true);
                    ModifyValue(XbapPROGID, null, "InfoTip", true);
                    ModifyValue(XbapPROGID, "DefaultIcon", "", true);
                    ModifyValue(XbapPROGID, @"shell\open\command", "", true);

                    ModifyValue(XamlCLSID_HKLM, "LocalServer32", "", true);
                    ModifyValue(XamlCLSID_HKLM, "ServerExecutable", "", true);
                    ModifyValue(XamlPROGID, null, "FriendlyTypeName", true);
                    ModifyValue(XamlPROGID, null, "InfoTip", true);
                    ModifyValue(XamlPROGID, "DefaultIcon", "", true);
                    ModifyValue(XamlPROGID, @"shell\open\command", "", true);

                    ModifyValue(CompositeFontPROGID, null, "FriendlyTypeName", true);
                    ModifyValue(CompositeFontPROGID, "DefaultIcon", "", true);

                    if (cpu == HostCPU.x64)
                    {
                        ModifyValue(XbapCLSID_HKLM_64, "LocalServer32", "", true);
                        ModifyValue(XbapCLSID_HKLM_64, "ServerExecutable", "", true);
                        ModifyValue(XbapPROGID_64, null, "FriendlyTypeName", true);
                        ModifyValue(XbapPROGID_64, null, "InfoTip", true);
                        ModifyValue(XbapPROGID_64, "DefaultIcon", "", true);
                        ModifyValue(XbapPROGID_64, @"shell\open\command", "", true);

                        ModifyValue(XamlCLSID_HKLM_64, "LocalServer32", "", true);
                        ModifyValue(XamlCLSID_HKLM_64, "ServerExecutable", "", true);
                        ModifyValue(XamlPROGID_64, null, "FriendlyTypeName", true);
                        ModifyValue(XamlPROGID_64, null, "InfoTip", true);
                        ModifyValue(XamlPROGID_64, "DefaultIcon", "", true);
                        ModifyValue(XamlPROGID_64, @"shell\open\command", "", true);

                        ModifyValue(CompositeFontPROGID_64, null, "FriendlyTypeName", true);
                        ModifyValue(CompositeFontPROGID_64, "DefaultIcon", "", true);
                    }
                }

                if (!WpfSetup.NoExecute)
                {
                    DeleteDirectory(_tempDirectory);
                }

                return 0;   // All steps completed

            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error: "+ e.Message);
            }

            return 1;
        }

        //---------------------------------------------------------------------
        public override int Uninstall()
        {
            HostCPU cpu = HostEnvironment.hostCPU;
            try
            {
                InitRegistryKeys(false);

                // Clear DevOverride registry key
                Console.WriteLine("> Clearing " + DevOverride.Name + "!DevOverrideEnable.");
                if (!WpfSetup.NoExecute)
                {
                    DevOverride.DeleteValue("DevOverrideEnable", false);

                    // This undoes any changes to the registry we made previously
                    // but only if the keys actual exists. If this method is called
                    // as part of a normal uninstall (not a rollback of a failed install)
                    // then these keys have probably been removed by the WPF uninstaller.
                    if (XbapCLSID_HKLM != null)
                    {
                        ModifyValue(XbapCLSID_HKLM, "LocalServer32", "", false);
                        ModifyValue(XbapCLSID_HKLM, "ServerExecutable", "", false);
                    }
                    if (XbapPROGID != null)
                    {
                        ModifyValue(XbapPROGID, null, "FriendlyTypeName", false);
                        ModifyValue(XbapPROGID, null, "InfoTip", false);
                        ModifyValue(XbapPROGID, "DefaultIcon", "", false);
                        ModifyValue(XbapPROGID, @"shell\open\command", "", false);
                    }
                    if (XamlCLSID_HKLM != null)
                    {
                        ModifyValue(XamlCLSID_HKLM, "LocalServer32", "", false);
                        ModifyValue(XamlCLSID_HKLM, "ServerExecutable", "", false);
                    }
                    if (XamlPROGID != null)
                    {
                        ModifyValue(XamlPROGID, null, "FriendlyTypeName", false);
                        ModifyValue(XamlPROGID, null, "InfoTip", false);
                        ModifyValue(XamlPROGID, "DefaultIcon", "", false);
                        ModifyValue(XamlPROGID, @"shell\open\command", "", false);
                    }
                    if (CompositeFontPROGID != null)
                    {
                        ModifyValue(CompositeFontPROGID, null, "FriendlyTypeName", false);
                        ModifyValue(CompositeFontPROGID, "DefaultIcon", "", false);
                    }
                    if (cpu == HostCPU.x64)
                    {
                        if (XbapCLSID_HKLM_64 != null)
                        {
                            ModifyValue(XbapCLSID_HKLM_64, "LocalServer32", "", false);
                            ModifyValue(XbapCLSID_HKLM_64, "ServerExecutable", "", false);
                        }
                        if (XbapPROGID_64 != null)
                        {
                            ModifyValue(XbapPROGID_64, null, "FriendlyTypeName", true);
                            ModifyValue(XbapPROGID_64, null, "InfoTip", true);
                            ModifyValue(XbapPROGID_64, "DefaultIcon", "", true);
                            ModifyValue(XbapPROGID_64, @"shell\open\command", "", true);
                        }
                        if (XamlCLSID_HKLM_64 != null)
                        {
                            ModifyValue(XamlCLSID_HKLM_64, "LocalServer32", "", false);
                            ModifyValue(XamlCLSID_HKLM_64, "ServerExecutable", "", false);
                        }
                        if (XamlPROGID_64 != null)
                        {
                            ModifyValue(XamlPROGID_64, null, "FriendlyTypeName", true);
                            ModifyValue(XamlPROGID_64, null, "InfoTip", true);
                            ModifyValue(XamlPROGID_64, "DefaultIcon", "", true);
                            ModifyValue(XamlPROGID_64, @"shell\open\command", "", true);
                        }
                        if (CompositeFontPROGID_64 != null)
                        {
                            ModifyValue(CompositeFontPROGID_64, null, "FriendlyTypeName", false);
                            ModifyValue(CompositeFontPROGID_64, "DefaultIcon", "", false);
                        }
                    }
                }

                // Remove PrivateOnVista directory
                if (!WpfSetup.NoExecute)
                {
                    DeleteDirectory(_targetDirectory);
                }

                // On x64, remove the WOW PrivateOnVista directory
                if (cpu == HostCPU.x64)
                {
                    CreateDirectory(_targetWOWDirectory);
                }

                // Remove any .local directories created for installed EXEs
                if (!WpfSetup.NoExecute)
                {
                    DeleteDirectory(Path.Combine(cpu == HostCPU.x86 ? Environment.SystemDirectory : _wowDirectory, @"XPSViewer\XPSViewer.exe.local"));
                }

                // If this is a roll-back, remove the temp directory used to extract DLLs
                if (!WpfSetup.NoExecute && _tempDirectory != null)
                {
                    DeleteDirectory(_tempDirectory);
                }

                return 0;  // All steps are completed

            }
            catch(Exception e)
            {
                Console.WriteLine("Unexpected error: " + e.Message);
            }

            return 1;
        }

        //---------------------------------------------------------------------
        // only setup redirection on Vista
        public override bool InstallOnThisOS(HostOS hostOS)
        {
            return (hostOS == HostOS.Vista);
        }

        //---------------------------------------------------------------------
        // Because the state of this installer is difficult to determine accurately
        // we ask the framework to always carry out any operation on this installer
        // regardless of the state of IsInstalled
        public override bool ForceOperation
        {
            get { return true; }
        }

        //---------------------------------------------------------------------
        // Installed if PrivateOnVista directory exists and all redirected DLLs exists
        public override bool IsInstalled()
        {
            return (Directory.Exists(_targetDirectory) && (_targetWOWDirectory == null || Directory.Exists(_targetWOWDirectory)));
        }

        //---------------------------------------------------------------------
        public override string GetVersion()
        {
            return "<None>";
        }

        //---------------------------------------------------------------------
        public override string[] InstallerFiles
        {
            get { return new string[] { _wpfInstaller, _wicInstaller }; }
        }

        //---------------------------------------------------------------------
        public override bool IsSystemPrerequisite { get { return false; } }

        #region Extraction

        private void ExtractDLLs()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _toolDirectory = @"\\vblwcpbuilds\release\1-offs\wpfsetup"; 
            
            // Checking tool directory, will use fallback if necessary
            bool found = false;
            try
            {
                found = new DirectoryInfo(_toolDirectory).Exists;
            }
            finally
            {
                if (!found)
                {
                    Console.WriteLine("Warning: Cannot access " + _toolDirectory + "; as fallback, looking in "+BasePath+" for \"extract.exe\".");
                    _toolDirectory = BasePath;
                    if (!new FileInfo(Path.Combine(_toolDirectory, "extract.exe")).Exists)
                    {
                        throw new Exception("Cannot find \"extract.exe\".");
                    }
                }
            }

            CreateDirectory(_tempDirectory);

            ExtractCabFromMsi(_wpfInstaller);

            ExtractFilesFromCab();
            
            ExtractFilesFromUpdateInstaller(_wicInstaller);
        }

        private void ExtractCabFromMsi(string msiName)
        {        
            IntPtr hDatabase;

            uint uRet = MsiOpenDatabase(Path.Combine(BasePath, msiName), (IntPtr)0, out hDatabase);

            if (uRet != 0)
            {
                // Error occurs. Report error and stop the setup.
                throw new Exception("Cannot open installer " + msiName + ", return value is " + uRet.ToString());
            }

            //
            // Query for the WPF Cab stream.
            //

            IntPtr hView;
            uRet = MsiDatabaseOpenView(hDatabase, "SELECT * FROM _Streams WHERE Name='"+WpfCabName+"'", out hView);

            if (uRet != 0)
            {
                // Error occurs. Report error and stop the setup.
                throw new Exception("Cannot open view for "+WpfCabName+" in " + msiName + ", return value is " + uRet.ToString());
            }

            IntPtr hRecord;
            MsiViewExecute(hView, IntPtr.Zero);
            uRet = MsiViewFetch(hView, out hRecord);

            if (uRet != 0)
            {
                // Error occurs. Report error and stop the setup.
                throw new Exception("Cannot fetch "+WpfCabName+" from " + msiName + ", return value is " + uRet.ToString());
            }

            uint size = MsiRecordDataSize(hRecord, 2);
            byte[] buffer = new byte[size];

            uRet = MsiRecordReadStream(hRecord, 2, buffer, ref size);
            MsiCloseHandle(hRecord);

            if (uRet != 0)
            {
                throw new Exception("Cannot read stream for "+WpfCabName+",  return value is " + uRet.ToString());
            }

            // This file won't already exists because the directory was just created
            using (FileStream f = new FileStream(Path.Combine(_tempDirectory, WpfCabName), FileMode.CreateNew))
            {
                f.Write(buffer, 0, buffer.Length);
            }

            MsiCloseHandle(hView);
            MsiCloseHandle(hDatabase);
        }

        private void ExtractFilesFromCab()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = Path.Combine(_toolDirectory, "extract.exe");
            info.Arguments = "/Y " + WpfCabName;

            // Add the file names to extract
            foreach (string dll in _dlls)
            {
                info.Arguments += " " + dll;
            } 

            info.WorkingDirectory = _tempDirectory;

            if (LaunchProcess(info) != 0)
            {
                throw new Exception("Cannot extract files from "+WpfCabName+".");
            }
        }

        private void ExtractFilesFromUpdateInstaller(string installerName)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = installerName;
            info.Arguments = "/passive /x:"+_tempDirectory;
            info.WorkingDirectory = BasePath;

            if (LaunchProcess(info) != 0)
            {
                throw new Exception("Cannot extract CAB from " + installerName);
            }
        }

        #endregion Extraction


        #region Registry Editing

        // Get the registry keys we'll need to modify.  These can't be opened in the constructor
        // because they don't exist until after the WPF installer has completed.
        private void InitRegistryKeys(bool throwIfNotPresent)
        {
            DevOverride = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options");
            XbapCLSID_HKLM = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\CLSID\{ADBE6DEC-9B04-4A3D-A09C-4BB38EF1351C}", RegistryKeyPermissionCheck.ReadWriteSubTree);
            XbapPROGID = Registry.ClassesRoot.OpenSubKey(@"Windows.Xbap", RegistryKeyPermissionCheck.ReadWriteSubTree);
            XamlCLSID_HKLM = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\CLSID\{CF1BF3B6-7AD0-4410-996B-C78EAFCD3269}", RegistryKeyPermissionCheck.ReadWriteSubTree);
            XamlPROGID = Registry.ClassesRoot.OpenSubKey(@"Windows.XamlDocument", RegistryKeyPermissionCheck.ReadWriteSubTree);
            CompositeFontPROGID = Registry.ClassesRoot.OpenSubKey(@"Windows.CompositeFont", RegistryKeyPermissionCheck.ReadWriteSubTree);

            HostCPU cpu = HostEnvironment.hostCPU;
            if (cpu == HostCPU.x64)
            {
                XbapCLSID_HKLM_64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Classes\CLSID\{ADBE6DEC-9B04-4A3D-A09C-4BB38EF1351C}", RegistryKeyPermissionCheck.ReadWriteSubTree);
                XbapPROGID_64 = Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\Windows.Xbap", RegistryKeyPermissionCheck.ReadWriteSubTree);
                XamlCLSID_HKLM_64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Classes\CLSID\{CF1BF3B6-7AD0-4410-996B-C78EAFCD3269}", RegistryKeyPermissionCheck.ReadWriteSubTree);
                XamlPROGID_64 = Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\Windows.XamlDocument", RegistryKeyPermissionCheck.ReadWriteSubTree);
                CompositeFontPROGID_64 = Registry.ClassesRoot.OpenSubKey(@"Wow6432Node\Windows.CompositeFont", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            if (throwIfNotPresent && 
                (XbapCLSID_HKLM == null || XbapPROGID == null || XamlCLSID_HKLM == null || XamlPROGID == null || CompositeFontPROGID == null ||
                (cpu == HostCPU.x64 && (XbapCLSID_HKLM_64 == null || XbapPROGID_64 == null || XamlCLSID_HKLM_64 == null || XamlPROGID_64 == null || CompositeFontPROGID_64 == null))))
            {
                throw new Exception("Expected registry entries (created by WPF.msi) are missing.");
            }
        }

        // Given a registry value, replaces instances of the EXE's name with
        // the private directory followed by the EXE's name.  Will have no affect
        // if the EXE's not in the value or the private directory is already part
        // of the value.
        private void ModifyValue(RegistryKey master, string subkey, string valueName, bool addDirectory)
        {
            RegistryKey key = master;
            string modifiedPath = Path.Combine(RedirectDirectory, ExeToRedirect);

            // Subkey is optional
            if (subkey != null)
            {
                key = master.OpenSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            if (key == null)
            {
                // The key may have been removed already by the uninstaller
                if (!addDirectory)
                    return;
                else
                    throw new Exception("Expected registry entries (created by WPF.msi) are missing.");
            }

            // If valueName is null or "" it refers to the (Default) value name
            string value = key.GetValue(valueName) as String;

            // Make sure the key contains a PresentationHost.exe and doesn't already contain a PrivateOnVista
            if (value != null)
            {
                string newValue = null;

                if (addDirectory)
                {
                    if (value.IndexOf(ExeToRedirect) != -1 && value.IndexOf(RedirectDirectory) == -1)
                    {
                        newValue = value.Replace(ExeToRedirect, modifiedPath);
                    }
                }
                else
                {
                    if (value.IndexOf(modifiedPath) != -1)
                    {
                        newValue = value.Replace(modifiedPath, ExeToRedirect);
                    }
                }

                if (newValue != null)
                {
                    key.SetValue(valueName, newValue);
                }
            }
        }

        #endregion Registry Editing


        #region Files And Directories 


        private void MakeLink(string targetDirectory, string sourceDirectory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(targetDirectory);
            if (!dirInfo.Exists)
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "cmd.exe";
                info.Arguments = "/c \"mklink /D " + targetDirectory + " " + sourceDirectory + "\"";
                info.WorkingDirectory = BasePath;

                if (LaunchProcess(info) != 0)
                {
                    throw new Exception("Cannot create symbolic link: " + dirInfo.FullName);
                }
            }
        }


        // Copies or moves a file - handles read-only attributes on both the source and target
        private void CopyFile(string sourceFile, string targetFile, bool moveInsteadOfCopy)
        {
            try{
                FileInfo sourceFileInfo = new FileInfo(sourceFile);

                // The source file must exist for a copy or move operation
                if (!sourceFileInfo.Exists)
                {
                    throw new Exception("Cannot move " + sourceFile + ", file does not exist.");
                }

                if (moveInsteadOfCopy)
                {
                    // Source file might be read-only - clear the Attribute before Move                    
                    sourceFileInfo.Attributes = sourceFileInfo.Attributes & ~(FileAttributes.ReadOnly);
                }

                FileInfo fileInfo = new FileInfo(sourceFile);

                // If target file exists already, make sure its not read-only and delete it
                FileInfo targetFileInfo = new FileInfo(targetFile);
                if (targetFileInfo.Exists)
                {
                    targetFileInfo.Attributes = targetFileInfo.Attributes & ~(FileAttributes.ReadOnly);
                    targetFileInfo.Delete();
                }

                if (moveInsteadOfCopy)
                {
                    fileInfo.MoveTo(targetFile);
                }
                else
                {
                    fileInfo = fileInfo.CopyTo(targetFile, true);
                }

                // Mark new file read-only to prevent inadvertent deleting via symbolic link
                fileInfo.Attributes = fileInfo.Attributes | FileAttributes.ReadOnly;
            }
            catch (IOException)
            {
                throw new Exception("Cannot copy/move file to "+targetFile+", location is read-only");
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Cannot copy/move file to "+targetFile+", access denied");
            }
        }

        // Deletes a directory - takes care of the read-only attribute on the directory.
        // Note: Does not handle a non-empty directory.
        private void DeleteDirectory(string targetDirectory)
        {
            try{
                DirectoryInfo dirInfo = new DirectoryInfo(targetDirectory);

                Console.WriteLine("> Deleting: " + targetDirectory);

                // The directory might be read-only - remove the Attribute before deleting
                if (dirInfo.Exists)
                {
                    // Only do this for directories that aren't symbolic links
                    if ((dirInfo.Attributes & FileAttributes.ReparsePoint) == 0)
                    {
                        FileInfo[] files = dirInfo.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            file.Attributes = file.Attributes & ~(FileAttributes.ReadOnly);
                        }
                    }

                    dirInfo.Attributes = dirInfo.Attributes & ~(FileAttributes.ReadOnly);
                    dirInfo.Delete(true);
                }
            }
            catch (IOException)
            {
                throw new Exception("Cannot delete directory "+targetDirectory+", path is read-only");
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Cannot delete directory "+targetDirectory+", access denied");
            }
        }

        private DirectoryInfo CreateDirectory(string targetDirectory)
        {
            try
            {
                DirectoryInfo newDirectory = null;

                Console.WriteLine("> Creating directory: " + targetDirectory);
                if (!WpfSetup.NoExecute)
                {
                    newDirectory = new DirectoryInfo(targetDirectory);
                    // Following call is a no-op if the directory already exists
                    newDirectory.Create();                    
                }

                return newDirectory;
            }
            catch (IOException)
            {
                throw new Exception("Cannot create directory " + targetDirectory + ", path is read-only");
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Cannot create directory " + targetDirectory + ", access denied");
            }
        }

        #endregion Files And Directories

        
        // Name of the WPF installer
        private string _wpfInstaller;
        // Name of the WIC installer
        private string _wicInstaller;
        // Name of the DLLs we need to extract from the WPF installer
        private List<string> _dlls = new List<string>(2);
        // Name of the files to copy out of the temp directory
        private List<string> _sourceFiles = new List<string>(8);
        // Full path (including new name if any) of new location of files from temp directory
        private List<string> _targetFiles = new List<string>(8);
        // Path of the PrivateOnVista directory
        private string _targetDirectory;
        // Path of the second PrivateOnVista directory for 64-bit OSs (containing WOW dlls)
        private string _targetWOWDirectory;
        // Path of the WOW system directory
        private string _wowDirectory;
        // Path of the temp directory used to extract DLLs
        private string _tempDirectory;
        // Directory to get extract.exe from
        private string _toolDirectory;

        private RegistryKey DevOverride;        
        private RegistryKey XbapCLSID_HKLM;
        private RegistryKey XbapPROGID;
        private RegistryKey XamlCLSID_HKLM;
        private RegistryKey XamlPROGID;
        private RegistryKey CompositeFontPROGID;
        private RegistryKey XbapCLSID_HKLM_64;
        private RegistryKey XbapPROGID_64;
        private RegistryKey XamlCLSID_HKLM_64;
        private RegistryKey XamlPROGID_64;
        private RegistryKey CompositeFontPROGID_64;

        /// <summary>
        /// Name of directory to put DLLs and EXE in to cause DLL redirection
        /// </summary>
        private const string RedirectDirectory = "PrivateOnVista";

        /// <summary>
        /// Name of exe we are moving out of System32 to cause DLL redirection.
        /// </summary>
        private const string ExeToRedirect = "PresentationHost.exe";

        /// <summary>
        /// Name of the WPF cab located inside of the WPF MSI file.
        /// </summary>
        private const string WpfCabName = "wpf.cab";
    }


    //=========================================================================
    /// <summary>
    /// installable component
    /// </summary>
    internal abstract class Component
    {
        //---------------------------------------------------------------------
        public Component(string name)
        {
            _name = name;
        }

        /// <summary>
        /// install this component
        /// </summary>
        /// <returns>exit code of any external program; 0 indicates success</returns>
        public abstract int Install();

        /// <summary>
        /// Uninstall this component
        /// </summary>
        /// <returns>exit code of any external program; 0 indicates success</returns>
        public abstract int Uninstall();

        /// <summary>
        /// test if this component is already installed
        /// </summary>
        /// <returns>true if already installed</returns>
        public abstract bool IsInstalled();

        /// <summary>
        /// force any operation to be carried out, regardless of
        /// the value of IsInstalled
        /// </summary>
        /// <returns>true if already installed</returns>
        public virtual bool ForceOperation
        {
            get { return false; }
        }

        /// <summary>
        /// return version of installed component
        /// (from registry or actual files)
        /// </summary>
        /// <returns>version string</returns>
        public abstract string GetVersion();

        /// <summary>
        /// return name of installer files expected by this component
        /// </summary>
        /// <returns>file name</returns>
        public abstract string[] InstallerFiles { get; }

        /// <summary>
        /// true if this component is a system prerequisite (e.g. MSXML6)
        /// </summary>
        public abstract bool IsSystemPrerequisite { get; }

        /// <summary>
        /// true if this component is a required install on this OS
        /// </summary>
        public virtual bool InstallOnThisOS(HostOS hostOS)
        {
            return true;
        }

        /// <summary>
        /// full path to log file created by last action
        /// </summary>
        public virtual string LastLogFile
        {
            get { return _lastLogFile; }
        }

        //---------------------------------------------------------------------
        public string Name
        {
            get { return _name; }
        }

        //
        // Some Native methods which are used to get property value from MSI package.
        //
        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiOpenDatabase(string szDatabasePath, IntPtr uiOpenMode, out IntPtr hDatabase);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiDatabaseOpenView(IntPtr hDatabase, string szQuery, out IntPtr hView);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiViewExecute(IntPtr hView, IntPtr hRecord);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiViewFetch(IntPtr hView, out IntPtr hRecord);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiRecordGetString(IntPtr hRecord, uint iField, StringBuilder szValueBuf, ref uint cchValueBuf);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiRecordDataSize(IntPtr hRecord, uint iField);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiRecordReadStream(IntPtr hRecord, uint iField, byte[] buffer, ref uint cchValueBuf);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint MsiCloseHandle(IntPtr hAny);

        //---------------------------------------------------------------------
        protected int InstallMSI(string msiFile, string logFile, string additionalCmdLineArgs)
        {           
            VerifyFileExists("Cannot find MSI file to install ", msiFile);
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "msiexec";
            info.Arguments = String.Format("/passive /norestart /I {0} /lv* {1} {2}", WrapInQuotes(msiFile), WrapInQuotes(logFile), additionalCmdLineArgs);
            info.WorkingDirectory = BasePath;
            return LaunchProcess(info);
        }

        //---------------------------------------------------------------------
        protected int UninstallMSI(string msiFileOrProdCode, string logFile, string additionalCmdLineArgs)
        {
            string product;
            // this can be a product code (a GUID)
            msiFileOrProdCode = msiFileOrProdCode.Trim();
            if (msiFileOrProdCode.StartsWith("{") && msiFileOrProdCode.EndsWith("}"))
                product = msiFileOrProdCode;
            else
            {
                VerifyFileExists("Cannot find MSI file to uninstall ", msiFileOrProdCode);
                product = WrapInQuotes(msiFileOrProdCode);
            }

            Console.WriteLine("product is " + product);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "msiexec";
            info.Arguments = String.Format("/passive /x {0} /lv* {1} {2}", product, WrapInQuotes(logFile), additionalCmdLineArgs);
            info.WorkingDirectory = BasePath;
            return LaunchProcess(info);
        }

        //---------------------------------------------------------------------
        protected int InstallRegSvr(string dllName)
        {
            VerifyFileExists("Cannot find DLL to register COM with", dllName);
            // 
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "regsvr32";
            info.Arguments = "/s " + WrapInQuotes(dllName);
            info.WorkingDirectory = BasePath;
            return LaunchProcess(info);
        }

        //---------------------------------------------------------------------
        protected int UninstallRegSvr(string dllName)
        {
            VerifyFileExists("Cannot find DLL to unregister COM with", dllName);
            // 
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "regsvr32";
            info.Arguments = "/s /u " + WrapInQuotes(dllName);
            info.WorkingDirectory = BasePath;
            return LaunchProcess(info);
        }

        //---------------------------------------------------------------------
        // install or uninstall using the update.exe mechanism (e.g. WIC)
        protected int RunUpdateExe(string exeName, string logFile)
        {
            VerifyFileExists("Cannot find EXE to install/uninstall", exeName);
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = exeName;
            info.Arguments = "/passive /norestart /log:" + WrapInQuotes(logFile);
            return LaunchProcess(info);
        }


        //---------------------------------------------------------------------
        protected int LaunchProcess(ProcessStartInfo info)
        {
            Process process = new Process();
            process.StartInfo = info;
            Console.WriteLine("> {0} {1}", info.FileName, info.Arguments);

            if (WpfSetup.NoExecute)
                return 0;

            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot launch process {0} {1}", info.FileName, info.Arguments);
                throw;
            }
            int exitCode = process.ExitCode;
            Console.WriteLine("< exitCode={0}", exitCode);
            return exitCode;
        }

        //---------------------------------------------------------------------
        protected string BasePath
        {
            get
            {
                if (string.IsNullOrEmpty(_basePath))
                {
                    if (!String.IsNullOrEmpty(WpfSetup.RemoteBasePath))
                        _basePath = Path.GetFullPath(WpfSetup.RemoteBasePath);
                    else
                        _basePath = AppDomain.CurrentDomain.BaseDirectory;
                }
                return _basePath;
            }
        }

        //---------------------------------------------------------------------
        protected string LogFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    _logFilePath = AppDomain.CurrentDomain.BaseDirectory;
                }
                return _logFilePath;
            }
        }

        //---------------------------------------------------------------------
        private void VerifyFileExists(string msg, string fileName)
        {
            if (!File.Exists(fileName))
            {
                if (WpfSetup.NoExecute)
                    Console.WriteLine(msg, fileName);   // warn only
                else
                    throw new FileNotFoundException(msg, fileName);
            }
        }

        //---------------------------------------------------------------------
        internal static HostEnvironment HostEnvironment
        {
            get
            {
                if (!_hostEnvironment.HasValue)
                {
                    HostOS os = HostOS.unknown;
                    OperatingSystem system = System.Environment.OSVersion;
                    if ((system.Platform == PlatformID.Win32NT) && (system.Version.Major >= 6))
                        os = HostOS.Vista;
                    else
                    {
                        if (system.Version.Minor == 2)
                            os = HostOS.WS2k3;
                        else if (system.Version.Minor == 1)
                            os = HostOS.WinXP;
                    }

                    HostCPU cpu = HostCPU.unknown;
                    string arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                    arch = arch.ToUpper();
                    if (arch == "X86")
                        cpu = HostCPU.x86;
                    else if (arch == "AMD64")
                        cpu = HostCPU.x64;
                    if (arch == "IA64")
                        cpu = HostCPU.ia64;
                    _hostEnvironment = new HostEnvironment(cpu, os);
                }
                return (HostEnvironment)_hostEnvironment;
            }
        }

        //---------------------------------------------------------------------
        protected string WrapInQuotes(string path)
        {
            path = path.Trim();
            if (path.IndexOf(" ") < 0)
                return path;    // no blanks -> no quotes needed

            // avoid more than one pair of quotes
            path = path.Trim('\"');
            return "\"" + path + "\"";
        }

        protected string _lastLogFile = String.Empty;

        private string _name;
        private static Nullable<HostEnvironment> _hostEnvironment;
        private string _basePath;
        private string _logFilePath;
    }

    /// <summary>
    /// describes type of host operating system
    /// </summary>
    internal enum HostOS
    {
        unknown,
        WinXP,
        WS2k3,
        Vista
    }

    /// <summary>
    /// describes host CPU
    /// </summary>
    internal enum HostCPU
    {
        unknown,
        x86,
        x64,
        ia64        // not supported by WPF
    }

    /// <summary>
    /// describes target host environment
    /// </summary>
    internal struct HostEnvironment
    {
        public HostEnvironment(HostCPU hostCPU, HostOS hostOS)
        {
            this.hostCPU = hostCPU;
            this.hostOS = hostOS;
        }

        public HostCPU hostCPU;
        public HostOS hostOS;
    }
}
