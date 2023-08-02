using System;
using System.Collections;
using System.Xml;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Struct ParameterBuilder
    /// </summary>
    public class StructParameterBuilder : ParameterBuilder
    {
        public override void Construct(ArrayList parameterList, XmlAttribute attribute)
        {
            parameterList.Add(ConvertXmlAttributeValueToTypeValue(attribute.Value, typeof(Nullable<DateTime>)));
        }
    }
}
