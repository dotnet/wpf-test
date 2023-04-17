// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using System.Threading;
using System.Windows.Automation;
using Microsoft.Win32;
using Microsoft.Test.Deployment;
using Microsoft.Test.Diagnostics;

namespace Microsoft.Test.Windows.Client.AppSec.Deployment
{
    public class CheckPresentationHostBitnesses
    {
        public static void EntryPoint()
        {
            TestLog myLog;
            if (TestLog.Current == null)
            {
                myLog = new TestLog("PresentationHost bitness test : Verify PH.exe files on disk are correct bit-ness");
            }
            else
            {
                myLog = TestLog.Current;
            }

            if (Environment.GetEnvironmentVariable("ProgramFiles(x86)") == null)
            {
                GlobalLog.LogEvidence("PASS: This test only matters on AMD64 Operating systems, setting result to pass");
                myLog.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Inspecting PresentationHost.exe binaries to ensure these are as expected:");
                bool pass = true;
                string PresHost32Path = Environment.GetEnvironmentVariable("Windir") + "\\SysWow64\\Presentationhost.exe";
                string PresHost64Path = Environment.GetEnvironmentVariable("Windir") + "\\System32\\Presentationhost.exe";

                GlobalLog.LogEvidence("PresentationHost 32-bit path:" + PresHost32Path);
                pass &= CheckBinaryBitness(false, PresHost32Path);

                GlobalLog.LogEvidence("PresentationHost 64-bit path:" + PresHost64Path);
                pass &= CheckBinaryBitness(true, PresHost64Path);

                if (pass)
                {
                    GlobalLog.LogEvidence("PASS: PresentationHost binary bit-nesses as expected");
                    myLog.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("FAIL: PresentationHost binary bit-nesses were not as expected");
                    myLog.Result = TestResult.Fail;
                }
            }
            TestLog.Current.Close();
            ApplicationMonitor.NotifyStopMonitoring();
        }

        private static bool CheckBinaryBitness(bool shouldBe64Bit, string executable)
        {
            bool is64Bit = IEAutomationHelper.Is64BitExecutable(executable);
            GlobalLog.LogEvidence("Binary " + executable + " is " + (is64Bit ? "64-bit" : "32-bit") + " and should " + (shouldBe64Bit ? "be" : "not be") + " 64-bit");

            if (is64Bit && shouldBe64Bit)
            {
                return true;
            }
            if (!is64Bit && !shouldBe64Bit)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Test coverage  - Perf: Is PresentationHost restarting needed under LUA?
    /// </summary>
    public class PresentationHostUACRestartTest
    {
        public static void EntryPoint(bool amRunningElevated)
        {
            TestLog myLog;

            // On XP this test should verify that PH.exe always restarts
            bool verifyRestartOccurred = false;

            if (TestLog.Current == null)
            {
                myLog = new TestLog("PresentationHost restart test: ensure that PresentationHost.exe does NOT restart when launched with elevated UAC. ");
            }
            else
            {
                myLog = TestLog.Current;
            }

            if (Diagnostics.SystemInformation.Current.MajorVersion < 6)
            {
                GlobalLog.LogEvidence("Checking for PH.exe restart (pre-Vista OS, not restarting is a UAC-specific behavior");
                verifyRestartOccurred = true;
            }

            string presHostPath = Environment.GetEnvironmentVariable("WINDIR");
            // If we're running 32-bit on a 64-bit OS, use the 32-bit System folder
            if (Diagnostics.SystemInformation.Current.IsWow64Process)
            {
                presHostPath += @"\SysWOW64\PresentationHost.exe";
            }
            else
            {
                presHostPath += @"\System32\PresentationHost.exe";
            }

            if (File.Exists(presHostPath))
            {
                Process initialPresHostInstance = Process.Start(presHostPath, "-embedding");
                Thread.Sleep(1000);

                // This log spew doesnt mean much on downlevel
                if (!verifyRestartOccurred)
                {
                    GlobalLog.LogEvidence("Running with " + (amRunningElevated ? "elevated" : "low") + " UAC. (Expect same result for both elevated and non-elevated process)");
                    GlobalLog.LogEvidence("(Should exit on non-elevated setups to restart w/o Administrators token, and not do so on elevated)");
                }
                GlobalLog.LogEvidence("Initial PresentationHost.exe Process, PID: " + initialPresHostInstance.Id + " has " + (initialPresHostInstance.HasExited ? "" : "not ") + "exited.");

                // Find other PresHosts:
                Process[] allRunningPhosts = Process.GetProcessesByName("presentationhost");

                GlobalLog.LogEvidence("Running PresentationHost Instances:");
                foreach (Process runningPresHostInstance in allRunningPhosts)
                {
                    GlobalLog.LogEvidence("  - PID: " + runningPresHostInstance.Id + ", Exited: " + runningPresHostInstance.HasExited.ToString());
                }
                if (allRunningPhosts.Length > 1)
                {
                    GlobalLog.LogEvidence("ERROR: Should have only seen one running PresentationHost.exe instance");
                }
                // XP scenario: Elevation means nothing, in both cases the process must restart.
                if (verifyRestartOccurred)
                {
                    if (initialPresHostInstance.HasExited && allRunningPhosts.Length == 1 && initialPresHostInstance.Id != allRunningPhosts[0].Id)
                    {
                        myLog.Result = TestResult.Pass;
                        GlobalLog.LogEvidence("Success: PresentationHost.exe restarted itself on pre-Vista OS (by design behavior- stripping Administrator's group process ACL)");
                    }
                    else
                    {
                        myLog.Result = TestResult.Fail;
                        GlobalLog.LogEvidence(@"Failure: PresentationHost.exe did not restart itself on pre-Vista OS (Check for HKLM\SOFTWARE\Microsoft\.NETFramework\Windows Presentation Foundation\Hosting\RunUnrestricted = 1 in registry, file a bug if it's not there.)");
                    }
                }
                // Vista scenario: Elevated or not, the process should never restart.
                else
                {
                    if (!initialPresHostInstance.HasExited && allRunningPhosts.Length == 1 && initialPresHostInstance.Id == allRunningPhosts[0].Id)
                    {
                        myLog.Result = TestResult.Pass;
                        GlobalLog.LogEvidence("Success: PresentationHost.exe did not restart itself under UAC-elevated conditions");
                    }
                    else
                    {
                        myLog.Result = TestResult.Fail;
                        GlobalLog.LogEvidence("Failure: PresentationHost.exe restarted itself under UAC-elevated conditions");
                    }
                }
            }
            else
            {
                GlobalLog.LogEvidence("Error: Could not find " + presHostPath);
                myLog.Result = TestResult.Fail;
            }
            TestLog.Current.Close();
            ApplicationMonitor.NotifyStopMonitoring();
        }
    }

    public class PresentationHostEventTest
    {
        public static void EntryPoint()
        {
            if (TestLog.Current == null)
            {
                new TestLog("PresentationHost -event Test");
            }

            TestLog.Current.LogEvidence("Registering for event \"foo\" ...");
            int handle = CreateEvent(IntPtr.Zero, true, false, "foo");

            TestLog.Current.LogEvidence("Starting PresentationHost.exe with args \"-debug -event foo\".");

            ProcessStartInfo presHostInfo = new ProcessStartInfo(IEAutomationHelper.PresentationHostExePath, "-debug -event foo");
            Process toKill = Process.Start(presHostInfo);

            int result = WaitForSingleObject(handle, 15000);

            switch (result)
            {
                case 0:
                    {
                        TestLog.Current.Result = TestResult.Pass;
                        TestLog.Current.LogEvidence("Passed... got event \"foo\".");
                        break;
                    }
                default:
                    {
                        TestLog.Current.Result = TestResult.Fail;
                        TestLog.Current.LogEvidence("Failed... did not see event \"foo\" before timeout.");
                        break;
                    }
            }
            TestLog.Current.Close();
            ApplicationMonitor.NotifyStopMonitoring();
        }

        #region External Methods
        [DllImport("kernel32.dll")]
        static private extern int CreateEvent(IntPtr lpSecurityAttributes, bool bManualReset, bool bInitialState, string lpName);
        // HANDLE CreateEvent(LPSECURITY_ATTRIBUTES lpEventAttributes, BOOL bManualReset, BOOL bInitialState, LPCTSTR lpName);

        [DllImport("kernel32.dll")]
        static private extern int WaitForSingleObject(int hHandle, int dwMilliseconds);
        #endregion
    }

    public class PresentationHostOrphanMitigation
    {
        public static void EntryPoint()
        {
            new TestLog("Presentation Host Orphan Mitigation (Process Suicide Trigger)");

            if (SystemInformation.Current.IsServer)
            {
                GlobalLog.LogEvidence("This test cannot pass on Server. Setting result to \"ignore\" and returning...");
                TestLog.Current.Result = TestResult.Ignore;
                TestLog.Current.Close();
                return;
            }

            GlobalLog.LogDebug("Cleaning ClickOnce cache...");
            ApplicationDeploymentHelper.CleanClickOnceCache();
            Process IEProcess = Process.Start("iexplore", Directory.GetCurrentDirectory() + @"\" + @"SimpleBrowserHostedApplication.xbap");
            GlobalLog.LogEvidence("Started IE Process...");
            IEProcess.WaitForInputIdle();
            Thread.Sleep(9000);
            AutomationElement IEWindow = AutomationElement.FromHandle(IEProcess.MainWindowHandle);
            GlobalLog.LogEvidence("Clicking button to activate infinite loop...");
            IEAutomationHelper.ClickCenterOfElementById("btnInfiniteLoop");

            Thread.Sleep(1000);
            if (ApplicationDeploymentHelper.GetIEVersion() < 8)
            {
                GlobalLog.LogEvidence("Killed IE process... ");
                IEProcess.Kill();
            }
            else
            {
                GlobalLog.LogEvidence("Killing all IE processes (To work around IE8+ LCIE behavior)... ");
                // We can probably assume that future IE versions will continue the "Loosely coupled" model.
                // PresHost's kill thread only works if both processes die, so kill both.
                foreach (Process ieProcess in Process.GetProcessesByName("iexplore"))
                {
                    ieProcess.Kill();
                }
            }


            Thread.Sleep(15000);

            if (Process.GetProcessesByName("PresentationHost").Length > 0)
            {
                TestLog.Current.Result = TestResult.Fail;
                GlobalLog.LogEvidence("Failed... instances of PresentationHost found 15+ seconds after killing IE Process");
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
                GlobalLog.LogEvidence("Passed... no running instances of PresentationHost found 4 seconds after killing IE Process");
            }
            TestLog.Current.Close();
            ApplicationMonitor.NotifyStopMonitoring();
        }
    }

    public class PresentationHostNoRespawnTest
    {
        public static void EntryPoint()
        {
            new TestLog("Ensure PresentationHost does NOT restart in special scenarios");

            bool xamlPasses = doPresentationHostNoRespawnTest(Directory.GetCurrentDirectory() + "\\deploy_markup2.xaml", "xamlTestButton");
            bool xbapPasses = doPresentationHostNoRespawnTest(Directory.GetCurrentDirectory() + "\\SimpleBrowserHostedApplication.xbap", "btnSecurityTester");

            if (xamlPasses && xbapPasses)
            {
                TestLog.Current.Result = TestResult.Pass;
                GlobalLog.LogEvidence("Passed... no extra instances of PresentationHost started on exit for xaml + xbap");
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
                GlobalLog.LogEvidence("Failed... did not see event \"foo\" before timeout.");
            }
            TestLog.Current.Close();
            ApplicationMonitor.NotifyStopMonitoring();
        }

        private static bool doPresentationHostNoRespawnTest(string toActivate, string autoElementId)
        {

            // Kill all currently running presentationHost instances... 
            foreach (Process p in Process.GetProcessesByName("presentationhost"))
            {
                p.Kill();
            }
            GlobalLog.LogEvidence("Starting two instances of PresentationHost.exe with args \"-embedding\".");

            // Workaround ... for some reason on XP64 there's no guarantee that it will actually find PresHost
            // Even though it verily is in the SysWOW64 directory.  Solution... find the right one before we try
            string presHostPath = "presentationhost.exe";

            if ((Environment.OSVersion.Version.Major == 5))
            {
                presHostPath = (Directory.GetFiles(Environment.GetEnvironmentVariable("SystemRoot"), "PresentationHost.exe", SearchOption.AllDirectories))[0];
            }
            Process.Start(presHostPath, "-embedding");
            Process.Start(presHostPath, "-embedding");
            Thread.Sleep(3000);

            // PHost processes will restart themselves, so start them then get a reference to them, as the initial 
            // Processes returned from Process.Start are long gone...
            Process[] existingProcesses = Process.GetProcessesByName("presentationhost");

            if (existingProcesses.Length != 2)
            {
                GlobalLog.LogEvidence("ERROR: Couldn't find two presentationhost processes...");
                return false;
            }


            GlobalLog.LogEvidence("Starting " + toActivate);
            Process ieProc = Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Internet Explorer\IEXPLORE.exe", toActivate);

            // Sleep til the window starts up, then get an element representing it
            while (ieProc.MainWindowHandle == IntPtr.Zero)
            {
                ieProc.Refresh();
                Thread.Sleep(1000);
            }

            AutomationElement ieWindow = AutomationElement.FromHandle(ieProc.MainWindowHandle);

            // Wait until we're sure the xaml has rendered before checking.
            // In this case, block until finding an automationId of an element in the xaml.
            int countdown = 60;
            AutomationElement xamlTestButton = null;
            while ((countdown > 0) && (xamlTestButton == null))
            {
                try
                {
                    xamlTestButton = ieWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, autoElementId));
                }
                catch (System.InvalidOperationException)
                {
                    // Ignore.  Seen when the test is a bit too fast and searches through the (currently being destroyed) Accesibility tree of XappLauncher (in browser progress UI)
                }
                Thread.Sleep(750);
                countdown--;
            }

            if ((countdown == 0) && (xamlTestButton == null))
            {
                GlobalLog.LogEvidence("Error... couldnt find element with AutomationId \"" + autoElementId + "\" in IE WIndow... failing test.");
                return false;
            }
            else
            {
                // Xaml is rendered... now close it and verify that the other process is still running.
                ieProc.CloseMainWindow();

                while (!ieProc.HasExited)
                {
                    Thread.Sleep(1000);
                }
                GlobalLog.LogEvidence("Closed main window of IE process hosting the content... now to count remaining processes");
            }

            GlobalLog.LogEvidence("Sleeping 10 seconds to allow PresentationHost processes to exit if they're going to... ");
            Thread.Sleep(10000);

            int exitedCount = 0;
            foreach (Process p in existingProcesses)
            {
                if (p.HasExited)
                {
                    exitedCount++;
                }
            }
            // If one and only one of the setup processes exited, it's all good.  If both or none did, something bad happened.
            if (exitedCount == 1)
            {
                GlobalLog.LogEvidence("Passed... PresentationHost exit behavior was correct for " + Path.GetExtension(toActivate));
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Failed... PresentationHost exit behavior was correct for " + Path.GetExtension(toActivate));
                return false;
            }
        }
    }

    public class PresentationHostTimeOutTest
    {
        public static void EntryPoint()
        {
            TestLog myLog;
            if (TestLog.Current == null)
            {
                myLog = new TestLog("PresentationHost No Host Time-out Test");
            }
            else
            {
                myLog = TestLog.Current;
            }

            // Problem : Can't trust SystemInformation.Current.IsServer, since XP64 is Server but evaluates to false.
            // Solution: Check for existence of 64-bit Program Files + Pre-Vista version #.
            if ((SystemInformation.Current.IsServer) ||
               ((Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null) && (Environment.OSVersion.Version.Major < 6)))
            {
                GlobalLog.LogEvidence("This test cannot pass on Server.  Setting result to \"ignore\" and returning . . .");
                TestLog.Current.Result = TestResult.Ignore;
                TestLog.Current.Close(); 
                return;
            }

            RegistryKey HostingKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\Windows Presentation Foundation\Hosting", true);
            HostingKey.SetValue("NoHostTimeoutSeconds", 25, RegistryValueKind.DWord);
            GlobalLog.LogEvidence("Set the NoHostTimeoutSeconds reg key to 25 seconds ... ");
            try
            {
                // On x64 Xp / Server, we need to load PresHost from the SysWow64 folder.  On vista we run a true x64 version.
                if ((Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null) && (Environment.OSVersion.Version.Major < 6))
                {
                    Process.Start(Environment.GetEnvironmentVariable("windir") + "\\SysWow64\\" + "PresentationHost.exe", "-embedding");
                }
                else
                {
                    Process.Start("presentationhost.exe", "-embedding");
                }
                Thread.Sleep(1000);

                Process[] procs = Process.GetProcessesByName("presentationhost");

                if (procs.Length > 1)
                {
                    throw new Exception("Error! One or more PresentationHost processes are already running on this machine!");
                }
                GlobalLog.LogEvidence("Started PresentationHost... now waiting through timeout period ... ");

                // Sleep for the timeout + 1 second then check...
                Thread.Sleep(26000);

                if (procs[0].HasExited)
                {
                    TestLog.Current.Result = TestResult.Pass;
                    GlobalLog.LogEvidence("Success! Process exited after the NoHostTimeoutSeconds period!");
                }
                else
                {
                    TestLog.Current.Result = TestResult.Fail;
                    GlobalLog.LogEvidence("Error! Process has not exited after more than the NoHostTimeoutSeconds period!");
                    procs[0].Kill();
                }
            }
            catch (Exception exc)
            {
                TestLog.Current.Result = TestResult.Fail;
                GlobalLog.LogEvidence(exc.Message);
            }
            finally
            {
                // Clean up this key, although it's unlikely to matter... 
                HostingKey.DeleteValue("NoHostTimeoutSeconds", false);
                TestLog.Current.Close();
            }
        }
    }
}
