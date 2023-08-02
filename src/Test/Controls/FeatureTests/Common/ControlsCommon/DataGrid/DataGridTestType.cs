using System;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid test types.
    /// </summary>
    [Flags]
    public enum DataGridTestType
    {
        All = 0x0,
        Property = 0x1,
        Method = 0x2,
        Event = 0x3,
        Behavior = 0x4,
        Exception = 0x5
    }
}
