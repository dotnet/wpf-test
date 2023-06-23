using System.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// IDataBindFilter
    /// </summary>
    public interface IDataBindFilter
    {
        void Filter(ICollectionView view);
    }
}
