// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Collections.Generic;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations
{
    [TestDimension("fixed,fixed /fds=false")]
    [TestDimension("stickynote,highlight")]
	public class AFixedContentSuite : AVisualSuite
	{
        public const string FixedWithEmptyPage = "Fixed_EmptyPage.xaml";		

		protected override TestMode DetermineTestMode(string[] args)
		{
			return TestMode.Fixed;
		}

		/// <summary>
		/// Override default behavior because we want to load non-default content.
		/// </summary>
        [TestCase_Setup()]
		protected override void DoSetup()
		{
			SetupTestWindow();
            WholePageLayout();
            SetContent(FixedWithEmptyPage);
		}

        protected void SetContent(string filename)
        {
            ViewerBase.Document = LoadContent(filename);
            DispatcherHelper.DoEvents();
        }
	}
}	

