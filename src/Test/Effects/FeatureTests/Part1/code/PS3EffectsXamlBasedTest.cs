// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test hardware accelerated ps3 effects with xaml files.
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Xml;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.VisualVerification;
using System.Drawing;
using Microsoft.Test.Display;
using Microsoft.Test.Discovery;
using System.IO;
using System.Windows.Interop;
using Microsoft.Test.Serialization;
using System.Collections.Generic;
using Microsoft.Test.Threading;
using System.Windows.Markup;
using Microsoft.Test.Win32;
using System.Windows.Media;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// xaml base test for Effects. 
    /// verify with visual validation, move window to the second window and 
    /// verify, and test hittesting and serialization.
    /// </summary>
    public class PS3EffectsXamlBasedTest : EffectsXamlBasedTest
    {
        #region Methods

        /// <summary>
        /// create TestLog and run RunTest. 
        /// This is the entry point for xtc adaptor. 
        /// </summary>
        public new static void Run()
        {
            PS3EffectsXamlBasedTest test = new PS3EffectsXamlBasedTest();
            using (test.log = new TestLog("Effects Xamlbase Test"))
            {
                try
                {
                    test.RunTest(DriverState.DriverParameters);
                    test.log.Result = TestResult.Pass;
                }
                catch (Exception e)
                {
                    test.log.LogEvidence(string.Format("Got an exception: \n{0}", e.ToString()));
                    test.log.Result = TestResult.Fail;
                }
            }
        }

        /// <summary>
        /// Initialization, log initialzation information, create an ImageComparator
        /// and ImageAdapter for master image. 
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();         
            log.LogStatus("No Effect Master: " + _noEffectMasterImageFile);

            if (!createMasterImagesOnly)
            {
                if (File.Exists(_noEffectMasterImageFile))
                {
                    _noEffectMasterSnapshot = Snapshot.FromFile(_noEffectMasterImageFile);
                }
                else
                {
                    throw new TestSetupException(string.Format("master image file: {0} not found.", _noEffectMasterImageFile));
                }
            }
        }


        /// <summary>
        /// Compare capured window with master image.
        /// </summary>
        /// <returns></returns>
        protected override void ValidateWindow()
        {
            if(shouldRenderNoopEffect())
            {
                ValidateWindow(_noEffectMasterSnapshot, _noEffectMasterImageFile);
            }
            else
            {
                ValidateWindow(masterImageSnapshot, masterImageFile);
            }
        }

        private bool shouldRenderNoopEffect()
        {
            //should noop effect for software rendering, or hardware cannot render ps3. 
            if (currentRenderingMode == RenderingMode.Software
                || !RenderCapability.IsPixelShaderVersionSupported(3,0)
                ) return true;
            else return false; 

        }
        

        /// <summary>
        /// Clean Window Comparator, master image adaptors, and clean the window.
        /// </summary>
        protected override void Cleanup()
        {
            base.Cleanup();
            _noEffectMasterSnapshot = null;
        }

        /// <summary>
        /// method to set parameters to the test.
        /// </summary>
        /// <param name="parameters"></param>
        protected override void SetParameters(PropertyBag parameters)
        {
            base.SetParameters(parameters);
            // parse master image for no effect rendering.
            if (string.IsNullOrEmpty(parameters["NoEffectMaster"]))
            {
                throw new TestValidationException("Must Specify noop effect Master for ps3 xaml based tests.");
            }
            
            _noEffectMasterImageFile = parameters["NoEffectMaster"];
        }

        #endregion

        #region Private Data

        private string _noEffectMasterImageFile = string.Empty;
        private Snapshot _noEffectMasterSnapshot;
        
        #endregion
    }
}