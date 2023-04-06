// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.WPF.AppModel.CommonDialogs
{
    public class AppEntry
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length < 1 || args == null)
            {
                Logging.LogFail("Wrong Usage.");
                Usage();
                return;
            }

            int index;

            for (index = 0; index < args.Length; ++index)
            {
                if (args[index].IndexOf("/test:") == 0)
                {
                    break;
                }
            }

            string[] splitArgs = args[index].Split(':');
            
            if (splitArgs.Length < 2)
            {
                Logging.LogFail("Wrong Usage.");
                Usage();
                return;
            }

            Application app = null;

            switch (splitArgs[1].ToLower(CultureInfo.InvariantCulture))
            {
                case "customplacecases":
                    app = new CustomPlaceCases();
                    break;

                case "filedialogcustomplacecases":
                    app = new FileDialogCustomPlaceCases();
                    break;

                case "openfilebeforeshow":
                    app = new OpenFileBeforeShow();
                    break;

                case "openfileisthreadmodal":
                    app = new OpenFileIsThreadModal();
                    break;

                case "savefileisthreadmodal":
                    app = new SaveFileIsThreadModal();
                    break;

                case "openfileonsecondthread":
                    app = new OpenFileOnSecondThread();
                    break;

                case "savefileonsecondthread":
                    app = new SaveFileOnSecondThread();
                    break;

                default:
                    Logging.LogFail("Unknown test: '" + splitArgs[1] + "'");
                    return;
            }

            app.Exit += new ExitEventHandler(CloseTheLog);
            app.Startup += new StartupEventHandler(disableIme);
            app.Run();

        }

        public static void Usage()
        {
            Logging.LogStatus("Usage:");
            Logging.LogStatus("DialogTestsDev10 /test:<testname>");
        }

        static void disableIme(object sender, StartupEventArgs e)
        {
            InputMethod.Current.ImeState = InputMethodState.Off;
        }

        static void CloseTheLog(object sender, ExitEventArgs e)
        {
            if (Variation.Current != null)
            {
                Variation.Current.LogMessage("Closing Log...");
                Variation.Current.LogResult(Logging.CurrentLog.NewResult);                
                Variation.Current.Close();
            }
            else
            {
                LogManager.LogMessageDangerously("No left-over variation to close on app shutdown");
            }
        }
    }

    #region Logging
    public class Logging
    {
        public static Microsoft.Windows.Test.Client.AppSec.FrameworkLoggerWrapper CurrentLog;

        static Logging()
        {
            CurrentLog = new Microsoft.Windows.Test.Client.AppSec.FrameworkLoggerWrapper();
        }

        public static void LogStatus(string message)
        {
            CurrentLog.LogEvidence(message);
        }

        public static void SetPass(string message)
        {
            CurrentLog.LogEvidence(message);
            CurrentLog.Result = TestResult.Pass;
        }

        public static void LogPass(string message)
        {
            CurrentLog.LogEvidence(message);
            CurrentLog.Result = TestResult.Pass;
            CurrentLog.Close();
        }

        public static void SetFail(string message)
        {
            CurrentLog.LogEvidence(message);
            CurrentLog.Result = TestResult.Fail;
        }

        public static void LogFail(string message)
        {
            CurrentLog.LogEvidence(message);
            CurrentLog.Result = TestResult.Fail;
            CurrentLog.Close();
        }
    }
    #endregion    
}
