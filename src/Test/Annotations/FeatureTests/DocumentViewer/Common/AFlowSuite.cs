// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Base class for a Suite that tests FlowContent only.

using System;
using System.Windows;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;

namespace Avalon.Test.Annotations
{
	public abstract class AFlowSuite : ADefaultContentSuite
	{
        #region Overrides

        protected override TestMode DetermineTestMode(string[] args)
		{
			return TestMode.Flow;
		}      

        #endregion
    }
}	

