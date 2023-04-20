// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Controls;
    using System.IO;
    using System.Windows.Markup;
    using System.Reflection;
    using Microsoft.Test.Serialization;

    public partial class MyAppName : Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {
            
            Microsoft.Test.Logging.LogManager.BeginTest(Microsoft.Test.DriverState.TestName);
            Microsoft.Test.Logging.TestLog log = new Microsoft.Test.Logging.TestLog("CompilerTest");
            log.Stage = Microsoft.Test.Logging.TestStage.Initialize;
            log.Stage = Microsoft.Test.Logging.TestStage.Run;

            FieldInfo fi = typeof(MyClassName).GetField("NameField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            
            if (fi.IsPublic)
            
            {
                log.LogStatus("Field has correct access level defined by x:FieldModifier");
                log.Result = Microsoft.Test.Logging.TestResult.Pass;
            }
            else
            {
                log.LogStatus("Field has incorrect access level");
                log.Result = Microsoft.Test.Logging.TestResult.Fail;
            }
            log.Stage = Microsoft.Test.Logging.TestStage.Cleanup;
            log.Close();
            System.Windows.Application.Current.Shutdown();
        }
    }

    internal partial class MyClassName : Page
    {
        void StubFunction()
        {
        }
    }

