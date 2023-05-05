// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class CustomValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || ((BindingGroup)value).Items == null || ((BindingGroup)value).Items.Count <= 0)
            {
                return new ValidationResult(false, ErrorStrings.VALIDATION_RULE);
            }

            object item = ((BindingGroup)value).Items[0];

            string sValue = string.Empty; 

            // Only three data types will be used by this test. Check here 
            // to see which and get StringProperty.  
            if (item is DataStringErrors)
            {
                sValue = ((DataStringErrors)item).StringProperty;
            }
            else if (item is DataThrowGetErrors)
            {
                sValue = ((DataThrowGetErrors)item).StringProperty;
            }
            else if (item is DataNullErrors)
            {
                sValue = ((DataNullErrors)item).StringProperty;
            }
            else
            {
                // do nothing.
            }

            if (sValue.ToLower().Equals("gotham"))
            {
                return new ValidationResult(false, ErrorStrings.VALIDATION_RULE);
            }

            return ValidationResult.ValidResult;
        }
    }
}
