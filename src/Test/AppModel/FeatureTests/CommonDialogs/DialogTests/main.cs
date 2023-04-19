// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
    public class EbEntry
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1 || args == null)
                {
                    Logging.LogFail("Wrong Usage.");
                    Usage();
                    return;
                }

                int i;

                for (i = 0; i < args.Length; ++i)
                {
                    if (args[i].IndexOf("/test:") == 0)
                    {
                        break;
                    }
                }

                string[] extraargs;

                if (i >= args.Length)
                {
                    Logging.LogFail("Wrong Usage.");
                    Usage();
                    return;
                }
                else
                {
                    if (args.Length == (i + 1)) // no more args
                        extraargs = null;

                    extraargs = new string[args.Length - i - 1];
                    for (int i_extra = 0, i2 = i + 1; i2 < args.Length; ++i2, i_extra++)
                    {
                        extraargs[i_extra] = args[i2];
                    }
                }

                Application app = null;

                switch (args[i].Substring (6).ToLower(CultureInfo.InvariantCulture))
                {
                    case "checkdialogblockage":
                        app = new CheckDialogBlockage();
                        break;

                    case "dialogerrorcases":
                        app = new DialogErrorCases();
                        break;

                    case "dialogmodality2":
                        app = new DialogModality2();
                        break;

                    case "dialogonstarting":
                        app = new DialogOnStarting();
                        break;

                    case "dialogreset":
                        app = new DialogReset();
                        break;

                    case "dialogtestapp":
                        app = new DialogTestApp();
                        break;

                    case "dialogwindowcountvisibility":
                        app = new DialogWindowCountVisibility();
                        break;

                    case "dialogwindowreuse":
                        app = new DialogWindowReuse();
                        break;

                    case "dlgcloseprogrammatic":
                        app = new DlgCloseProgrammatic();
                        break;

                    case "dlgretvaltest":
                        app = new DlgRetValTest();
                        break;

                    case "manualcloseexplicit":
                        app = new ManualCloseExplicit();
                        break;

                    default:
                        Logging.LogFail("Unknown test: " + args[i].Substring(6));
                        return;
                }
                app.Startup += new StartupEventHandler(disableIme);
                app.Run();
            }
            catch
            {
                throw;
            }
        }

        public static void Usage()
        {
            Logging.LogStatus("Usage:");
            Logging.LogStatus("DialogTests /test:<testname>");
        }

        static void disableIme(object sender, StartupEventArgs e)
        {
            InputMethod.Current.ImeState = InputMethodState.Off;
        }
    }

    #region Logging
    public class Logging
    {
        static TestLog s_fw;

        public Logging()
        {
            s_fw = TestLog.Current;
            if (null == s_fw) s_fw = new TestLog("test.log");
        }
        public static void LogStatus(string message)
        {
            s_fw.LogEvidence(message);
        }

        public static void LogPass(string message)
        {
            s_fw.LogEvidence(message);
            s_fw.Result = TestResult.Pass;
        }

        public static void LogFail(string message)
        {
            s_fw = TestLog.Current;
            if (null == s_fw) s_fw = new TestLog("test.log");

            s_fw.LogEvidence(message);
            s_fw.Result = TestResult.Fail;
        }
    }
    #endregion    
}
