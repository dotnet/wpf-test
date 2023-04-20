// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;

namespace Microsoft.Test.DataServices
{
	public class MinCharsRule : ValidationRule
	{
		private int _min;
		private string _errorContent = string.Empty;

		public MinCharsRule()
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
			string stringValue = value as string;
			if (stringValue == null)
			{
				return new ValidationResult(false, "Value is null or not a string");
			}
			if (stringValue.Length < Min)
			{
				return new ValidationResult(false, ErrorContent);
			}
			return new ValidationResult(true, null);
		}
	}
}
