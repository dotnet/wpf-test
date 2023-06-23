using System;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid features.
    /// </summary>
    [Flags]
    public enum DataGridFeature
    {
        All = 0x0,
        Editing = 0x1,
        Keyboarding = 0x2,
        ObjectModel = 0x3,
        Selection = 0x4,
        Sorting = 0x5
    }
}
