// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using Microsoft.Test;
using Microsoft.Test.Data;
using System.Globalization;
using System.Security;
using System.Reflection;

namespace Microsoft.Test.DataServices
{
	public class LengthRangeRule : ValidationRule
	{
		public LengthRangeRule()
		{
		}

		private int _min;

		public int Min
		{
			get { return _min; }
			set { _min = value; }
		}

		private int _max;

		public int Max
		{
			get { return _max; }
			set { _max = value; }
		}

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			string strInserted = value.ToString();
			int actualLength = strInserted.Length;

			if ((actualLength < Min) || (actualLength > Max))
			{
				return new ValidationResult(false, "You entered " + actualLength + " characters. Please enter " + Min + " - " + Max + " characters.");
			}
			return new ValidationResult(true, null);
		}
	}
}
