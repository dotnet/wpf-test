using System.ComponentModel;
using System.Xml;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataBindXmlDataProviderFilter
    /// </summary>
    public class DataBindXmlDataProviderFilter : IDataBindFilter
    {
        public void Filter(ICollectionView view)
        {
            view.Filter = delegate(object item)
            {
                return int.Parse(((XmlElement)item).Attributes["Age"].Value) > 25;
            };
        }
    }
}
