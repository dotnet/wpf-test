// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Deployment;
using Microsoft.Test.Discovery;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Wpf.AppModel.Deployment
{
    /// <summary>
    /// UI handler
    /// </summary>
    [Test(0, "Xbap", "ETWEventVerification", Disabled=true)]
    public class DeploymentETWTests : EtwEventExistenceTest
    {
        public DeploymentETWTests()
            : base("XbapExpectedEvents.xml")
        {
            this.RunSteps += new TestStep(DoXbapActions);
        }

        TestResult DoXbapActions()
        {
            // Note this part is only for launching Xbaps... not needed for in-process WPF actions 
            ApplicationMonitor monitor = new ApplicationMonitor();
            ApplicationDeploymentHelper.CleanClickOnceCache();
            monitor.RegisterUIHandler(new PassIfSeenHandler(), "IEXPLORE", "RegExp:(Simple Browser Hosted SEE Application)" , UIHandlerNotification.All);
            monitor.StartProcess("SimpleBrowserHostedApplication.Xbap");
            monitor.WaitForUIHandlerAbort();
            monitor.Close();

            monitor = new ApplicationMonitor();
            monitor.RegisterUIHandler(new PassIfSeenHandler(), "IEXPLORE", "RegExp:(Loose XAML 1 V4 Edition)", UIHandlerNotification.All);
            monitor.StartProcess("Deploy_Markup1_v4.xaml");
            monitor.WaitForUIHandlerAbort();
            monitor.Close();            
            
            return VerifyExpectedEvents();
        }
    }
}
