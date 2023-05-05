// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Controls;
    using System.IO;
    using System.Windows.Markup;
    using Microsoft.Test.Serialization;

    public partial class Application__ : Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {
        }
    }


    public partial class MyAppName : Application
    {
        void AppStartup(object sender, StartupEventArgs args)
        {
            
            Microsoft.Test.Logging.LogManager.BeginTest(Microsoft.Test.DriverState.TestName);
            Microsoft.Test.Logging.TestLog log = new Microsoft.Test.Logging.TestLog("CompilerTest");
            log.Stage = Microsoft.Test.Logging.TestStage.Initialize;
            log.Stage = Microsoft.Test.Logging.TestStage.Run;

            FileStream fs = new FileStream(".\\obj\\release\\ClassMarkup.baml", FileMode.Open);
            BamlReaderWrapper reader = new BamlReaderWrapper(fs);
            reader.Read();
            reader.Read();
            string name = reader.Name;

            
            if (name == "MyNamespace.MyClassNameLogic")
            
            {
                log.LogStatus("BAML has correct name as defined by x:Subclass");
                log.Result = Microsoft.Test.Logging.TestResult.Pass;
            }
            else
            {
                log.LogStatus("BAML record has incorrect value: " + name);
                log.Result = Microsoft.Test.Logging.TestResult.Fail;
            }
            log.Stage = Microsoft.Test.Logging.TestStage.Cleanup;
            log.Close();
            System.Windows.Application.Current.Shutdown();

            MyClassName instance = new MyClassName();
            instance.Title="Foo";
        }
    }

