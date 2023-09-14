using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// A Custom control for 

    public class RegressionTest10Control : Control
    {
        static RegressionTest10Control()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RegressionTest10Control), new FrameworkPropertyMetadata(typeof(RegressionTest10Control)));
        }
    }
}
