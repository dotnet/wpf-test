using System.Collections;
using System.Xml;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Double ParameterBuilder
    /// </summary>
    public class DoubleParameterBuilder : ParameterBuilder
    {
        public override void Construct(ArrayList parameterList, XmlAttribute attribute)
        {
            parameterList.Add(ConvertXmlAttributeValueToTypeValue(attribute.Value, typeof(double)));
        }
    }
}
