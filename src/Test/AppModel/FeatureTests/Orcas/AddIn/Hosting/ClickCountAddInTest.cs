// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading; //WindowTest
using System.AddIn.Hosting;
using System.Windows.Markup;
using System.IO;

namespace Microsoft.Test.AddIn
{
    public class ClickCountAddInTest : WindowTest
    {

        #region Private Members

        private StackPanel _panel;
        private AddInHostContainer _hostContainer;

        #endregion

        #region Constructor

        public ClickCountAddInTest()
        {
            Harness.Current.Publish();
            this.InitializeSteps += new TestStep(SetUp);
            this.RunSteps += new TestStep(ActivateAddIns);
            this.RunSteps += new TestStep(AddToTree);
            this.RunSteps += new TestStep(InitializeAddIns);
            this.RunSteps += new TestStep(VerifyAddIns);
        }

        #endregion

        #region Initialize Steps

        private TestResult SetUp()
        {
            _panel = new StackPanel();
            _panel.HorizontalAlignment = HorizontalAlignment.Left;
            Window.Content = _panel;

            _hostContainer = new AddInHostContainer();

            TextBox tb = new TextBox();
            tb.Height = 100;
            tb.Width = 300;
            _panel.Children.Add(tb);

            AddInHost host = new AddInHost();
            host.AddInSecurityLevel = AddInSecurityLevel.Internet;
            host.RelativeAddInPath = ".";
            host.RelativePipelinePath = ".";
            host.VerifierType = typeof(CountClickAddInVerifier);
            host.AddInType = typeof(HostCountClicksAddInView);
            _hostContainer.Hosts.Add(host);

            _hostContainer.RootPanel = _panel;

            _hostContainer.InitializeHosts();
            _hostContainer.InitializeVerifiers();
            WaitForPriority(DispatcherPriority.Background);
            return TestResult.Pass;

        }

        #endregion

        #region Run Steps

        private TestResult ActivateAddIns()
        {
            _hostContainer.ActivateAddIns();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult AddToTree()
        {
            for (int i = 0; i < _hostContainer.Hosts.Count; i++)
            {
                _hostContainer.Hosts[i].ParentTheAddInUI();
            }
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult InitializeAddIns()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            _hostContainer.InitializeAddIns();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult VerifyAddIns()
        {
            return _hostContainer.VerifyTestAddIns();
        }


        #endregion

    }

}
