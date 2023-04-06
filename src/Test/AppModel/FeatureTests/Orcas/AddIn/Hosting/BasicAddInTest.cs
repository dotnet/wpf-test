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
using Microsoft.Test.RenderingVerification;
using System.Drawing;
using System.Xml;
using System.Windows.Media;
using Microsoft.Test.Security.Wrappers;

namespace Microsoft.Test.AddIn
{
    public class BasicAddInTest : WindowTest
    {


        #region Private Members

        private StackPanel _panel;
        private AddInHostContainer _hostContainer;
        private Stream _stream;

        #endregion

        #region Constructor

        // Someone, somewhere, changed something that causes TestDriver.RunServices.ExecuteTestCase to complain that BasicAddInTest
        // has an ambiguous ctor reference.  It used to work fine, it doesn't now.  Workaround is to make one ctor have a different
        // number of arguments than the other, so they're no longer ambiguous.  (I couldn't find any way to specify in the .xtc file
        // which type the CtorParams I specify are.  Though it kind of seems obvious to me they should be treated as strings, since
        // that's what they are inside the .xtc file.  It worked before.)
        public BasicAddInTest(Stream stream, int unused)
        {
            this._stream = stream;
            ConstructorHelper();
        }


        public BasicAddInTest(string fileName)
        {
            StreamSW stream = FileSW.OpenRead(fileName);
            this._stream = stream.InnerObject;
            ConstructorHelper();
        }

        #endregion

        #region Private Helper Methods

        private void ConstructorHelper()
        {
            this.InitializeSteps += new TestStep(SetUp);
            this.RunSteps += new TestStep(ActivateAddIns);
            this.RunSteps += new TestStep(AddToTree);
            this.RunSteps += new TestStep(InitializeAddIns);
            this.RunSteps += new TestStep(VerifyAddIns);
            this.RunSteps += new TestStep(ShutdownAddIns);
        }

        #endregion

        #region Initialize Steps

        private TestResult SetUp()
        {
            Log.LogStatus("Setting up the test");
            _panel = new StackPanel();
            _panel.HorizontalAlignment = HorizontalAlignment.Left;
            _panel.Background = new SolidColorBrush(Colors.AliceBlue);
            Window.Content = _panel;

            object hostContainerObject = XamlReader.Load(_stream);
            _hostContainer = (AddInHostContainer)hostContainerObject;
            if (_hostContainer == null)
            {
                Log.LogEvidence("HostContainer was null");
                return TestResult.Fail;
            }

            _hostContainer.RootPanel = _panel;

            Log.LogStatus("Initializing the Hosts");
            _hostContainer.InitializeHosts();

            Log.LogStatus("Initializing the Verifiers");
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

        private TestResult ShutdownAddIns()
        {
            try
            {
                _hostContainer.ShutdownAddIns();
            }
            catch (CannotUnloadAppDomainException exception)
            {
                if(System.Environment.OSVersion.Version >= new Version(6, 1))
                {
                    Log.LogStatus("Due to a known issue on Win7 (Dev10 bug 741318) the addin's appdomain fails to unload, only on lab runs.  Ignoring.");
                    Log.LogStatus(exception.ToString());
                }
                else throw(exception);
            }
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        #endregion

    }

}
