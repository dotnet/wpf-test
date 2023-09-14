using System.Collections;
using System.Xml;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// String ParameterBuilder
    /// </summary>
    public class StringParameterBuilder : ParameterBuilder
    {
        public override void Construct(ArrayList parameterList, XmlAttribute attribute)
        {
            parameterList.Add(ConvertXmlAttributeValueToTypeValue(attribute.Value, typeof(string)));
        }
    }
}
