// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test used for the scenario that load a xaml, change something (animation or other changes)
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.IO;
using System.Windows;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Test that some thing after loading a xaml. Load a xaml, do a visual validation, 
    /// call a virtual function change, wait 5 second for the change to complete, and then 
    /// do another visual validation
    /// </summary>`
    public abstract class XamlBasedChangingTest : EffectsWindowTest
    {
        #region Methods

        /// <summary>
        /// Constructor, pass in xaml file name, master file name, and shader file name. 
        /// and set up test step. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        public XamlBasedChangingTest(string xamlFileName, string startMasterImageName, string endMasterImageName) : base(300, 300)
        {
            this._xamlFileName = xamlFileName;
            this._startMasterImageName = startMasterImageName;
            this._endMasterImageName = endMasterImageName;
            ToleranceFilePath = "testprofile.xml";

            Interval = 5000;

            RunSteps += new TestStep(RunTest);
        }

        /// <summary>
        /// Runtest 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            try
            {
                //load content
                FileStream stream = File.OpenRead(_xamlFileName);
                _content = XamlReader.Load(stream) as FrameworkElement;
                ContentRoot.Content = _content;

                //Wait for message pump.
                WaitFor(200);

                //Do verification before animation. 
                if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, _startMasterImageName, ToleranceFilePath))
                {
                    throw new TestValidationException("Test failed in the first visual validation.");
                }

                Change(_content);

                //Wait until change complated. 
                WaitFor(Interval);

                //Do second verification after change.
                if(!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, _endMasterImageName, ToleranceFilePath))
                {
                    throw new TestValidationException("Test failed in the second visual validation.");
                }
            }
            catch (Exception e)
            {
                Log.LogStatus(e.Message);
                return TestResult.Fail;
            }

            // Fail if no exception
            Log.LogStatus("No Excception throw loading a shader from internet.");
            return TestResult.Pass;
        }

        abstract protected void Change(FrameworkElement content);
        protected string ToleranceFilePath {get; set;}
        protected int Interval {get; set;}

        #endregion

        #region private fields

        private string _xamlFileName;
        private string _startMasterImageName;
        private string _endMasterImageName;
        private FrameworkElement _content;

        #endregion
    }
}