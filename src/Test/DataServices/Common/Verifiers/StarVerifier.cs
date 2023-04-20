// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Test;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Data;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	public class StarVerifier : IVerifier
	{
		#region Private members

		private ItemsControl _ic;

		#endregion

		#region Constructor

		public StarVerifier(ItemsControl ic)
		{
			if(ic == null)
			{
				throw new Exception("ItemsControl is null");
			}
			this._ic = ic;
		}

		#endregion

		#region Private methods

		private bool VerifyCount(int lengthExpected, int lengthActual, string step)
		{
			if (lengthExpected != lengthActual)
			{
				GlobalLog.LogStatus("Fail - Expected length:" + lengthExpected + " Actual:" + lengthActual + " - " + step);
				return false;
			}
			return true;
		}

		private bool VerifyProperty(string expectedValue, FrameworkElement actualElement, string step)
		{
			TextBox actualElementText = actualElement as TextBox;
			if (actualElementText == null)
			{
				GlobalLog.LogStatus("Fail - Could not convert FrameworkElement to Text - " + step);
				return false;
			}
			string actualValue = actualElementText.Text;
			if (expectedValue != actualValue)
			{
                GlobalLog.LogStatus("Fail - Expected value:" + expectedValue + " Actual:" + actualValue + " - " + step);
				return false;
			}
			return true;
		}

		#endregion

		#region IVerifier implementation

		public IVerifyResult Verify(params object[] ExpectedState)
		{
            ObservableCollection<MyStar> aldc = ExpectedState[0] as ObservableCollection<MyStar>;

            int lengthExpected = 8;

			FrameworkElement[] firstNames = Util.FindElements(_ic, "firstName");
			FrameworkElement[] lastNames = Util.FindElements(_ic, "lastName");
			FrameworkElement[] ages = Util.FindElements(_ic, "age");

            if (!VerifyCount(lengthExpected, firstNames.Length, "First Names")) { return new VerifyResult(TestResult.Fail, "Error in VerifyCount - First Names"); }
            if (!VerifyCount(lengthExpected, lastNames.Length, "Last Names")) { return new VerifyResult(TestResult.Fail, "Error in VerifyCount - Last Names"); }
            if (!VerifyCount(lengthExpected, ages.Length, "Ages")) { return new VerifyResult(TestResult.Fail, "Error in VerifyCount - Ages"); }

			for(int i=0; i<lengthExpected; i++)
			{
                if (!VerifyProperty(((MyStar)aldc[i]).FirstName, firstNames[i], "First Names")) { return new VerifyResult(TestResult.Fail, "Error in VerifyProperty - First Names"); }
                if (!VerifyProperty(((MyStar)aldc[i]).LastName, lastNames[i], "Last Names")) { return new VerifyResult(TestResult.Fail, "Error in VerifyProperty - Last Names"); }
                if (!VerifyProperty(((MyStar)aldc[i]).Age.ToString(), ages[i], "Ages")) { return new VerifyResult(TestResult.Fail, "Error in VerifyProperty - Ages"); }
			}
            return new VerifyResult(TestResult.Pass, "Verify was successful");
		}

		#endregion

	}
}

