using System.Collections;
using System.Xml;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Bool ParameterBuilder
    /// </summary>
    public class BoolParameterBuilder : ParameterBuilder
    {
        public override void Construct(ArrayList parameterList, XmlAttribute attribute)
        {
            parameterList.Add(ConvertXmlAttributeValueToTypeValue(attribute.Value, typeof(bool)));
        }
    }
}
