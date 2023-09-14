using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Test.Controls
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyTestExpectedResultsAttribute : Attribute
    {
        public string TestId { get; set; }
        public bool IsReadOnly { get; set; }

        public static PropertyTestExpectedResultsAttribute GetExpectedResults(PropertyInfo propertyInfo, string testId)
        {
            var result = propertyInfo.GetCustomAttributes(true).OfType<PropertyTestExpectedResultsAttribute>().SingleOrDefault(attr => attr.TestId == testId);

            if (result == null)
            {
                result = new PropertyTestExpectedResultsAttribute();
            }

            return result;
        }
    } 
}
