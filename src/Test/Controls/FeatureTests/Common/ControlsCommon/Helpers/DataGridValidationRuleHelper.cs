using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using System.Windows.Data;
using Microsoft.Test.Controls.DataSources;

namespace Microsoft.Test.Controls.Helpers
{
#if TESTBUILD_CLR40
    /// <summary>
    /// BindingGroup validation rule specific to a Person object.  The 
    /// actual validation is hard coded to a specific condition that the
    /// tests will use to either trigger a failure or not.
    /// </summary>
    public class ItemValidationRule1 : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            BindingGroup bindingGroup = (BindingGroup)value;

            IList items = bindingGroup.Items;
            Person person = (Person)items[0];

            object firstNameObj;
            if (!bindingGroup.TryGetValue(person, "FirstName", out firstNameObj))
            {
                return new ValidationResult(false, string.Format("Unable to get FirstName from the BindingGroup"));
            }

            if (firstNameObj.ToString().ToUpper() == "XX")
            {
                return new ValidationResult(false, string.Format("FirstName must exist"));
            }
            
            return result;
        }
    }    
#endif
}
