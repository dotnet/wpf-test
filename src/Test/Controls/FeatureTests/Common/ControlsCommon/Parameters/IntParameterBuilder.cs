using System.Collections;
using System.Xml;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Int ParameterBuilder
    /// </summary>
    public class IntParameterBuilder : ParameterBuilder
    {
        public override void Construct(ArrayList parameterList, XmlAttribute attribute)
        {
            parameterList.Add(ConvertXmlAttributeValueToTypeValue(attribute.Value, typeof(int)));
        }
    }
}
