// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.IO;
using System.Windows.Annotations;
using Avalon.Test.Annotations;

namespace Avalon.Test.Annotations.Suites
{
    [OverrideClassTestDimensions]
    [TestDimension("flow,fixed")]
    public class DataPersistanceSuite_BVT : AStickyNoteControlFunctionalSuite
    {
        #region BVT TESTS

        #region Overrides

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);
            foreach (string arg in args)
            {
                if (arg.Equals("image"))
                    _contentkind = ContentKind.Image;
                if (arg.Equals("mixed"))
                    _contentkind = ContentKind.Mixed;
            }
        }

        [TestCase_Cleanup]
        protected override void CleanupVariation()
        {
            base.CleanupVariation();
            _contentkind = ContentKind.Standard_Small;
        }

        #endregion

        #region Tests

        /// <summary>
        /// Note with content.
        /// Page down.
        /// Page up.	
        /// Verify: Content is preserved.
        /// </summary>
        [TestDimension("stickynote,inkstickynote,stickynote image")]
        [Priority(0)]
        protected void data1()
        {
            CreateDefaultNote();
            SetContent(_contentkind);
            PageDown(2);
            PageUp(2);
            VerifyContent(_contentkind);
            passTest("Content persisted through page navigation.");
        }

        /// <summary>
        /// Note with content.
        /// Disable service.
        /// Enable service.
        /// Verify: Content is preserved.
        /// </summary>
        [TestDimension("stickynote,inkstickynote,stickynote image")]
        [Priority(0)]
        protected void data2()
        {
            CreateDefaultNote();
            SetContent(_contentkind);
            DisableAnnotationService();
            SetupAnnotationService();
            VerifyContent(_contentkind);
            passTest("Content persisted through disable/enable.");
        }

        #endregion

        #region Private Fields

        ContentKind _contentkind = ContentKind.Standard_Small;

        #endregion

        #endregion BVT TESTS
    }
}	

