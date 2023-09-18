// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;

namespace Microsoft.Office.InteropLateBound.Office
{
	/// <summary>
	/// Wrapper for Office
	/// </summary>
	public class Office
	{
		public Office()
		{
		}
	}

	public enum MsoTriState
	{
		msoCTrue = 1,
		msoFalse = 0,
		msoTriStateMixed = -2,
		msoTriStateToggle = -3,
		msoTrue = -1
	}

	public enum MsoAutomationSecurity
	{
		msoAutomationSecurityByUI = 2,
		msoAutomationSecurityForceDisable = 3,
		msoAutomationSecurityLow = 1
	}

	public enum MsoFeatureInstall
	{
		msoFeatureInstallNone = 0,
		msoFeatureInstallOnDemand = 1,
		msoFeatureInstallOnDemandWithUI = 2
	}
	
}
