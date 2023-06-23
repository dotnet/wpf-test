using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// RangeBase test interface. We can use it to run concrete RangeBase controls tests.
    /// </summary>
    public interface IRangeBaseTest
    {
        void Run(RangeBase rangeBase);
    }
}
