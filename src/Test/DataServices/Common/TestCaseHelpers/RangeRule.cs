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
	public class RangeRule : ValidationRule
	{
		public RangeRule()
		{
		}

		public int Min
		{
			get
			{
				return _min;
			}
			set
			{
				_min = value;
			}
		}

		public int Max
		{
			get
			{
				return _max;
			}
			set
			{
				_max = value;
			}
		}

		public string ErrorContent
		{
			get
			{
				return _errorContent;
			}
			set
			{
				_errorContent = value;
			}
		}

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			int intvalue;
			string strValue = value.ToString();
			if (Int32.TryParse(strValue, out intvalue))
			{
				if (intvalue < _min || intvalue > _max)
				{
					return new ValidationResult(false, ErrorContent);
				}
			}
			else if (strValue != String.Empty)
				return new ValidationResult(false, "Value is not an integer");

			return new ValidationResult(true, null);
		}

		private int _min;

		private int _max;

		private string _errorContent = string.Empty;
	}
}
