using System;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// The modifier keys that we support to test DataGrid selection.
    /// </summary>
    [Flags]
    public enum ModifierKeys
    {
        None = 0x0,
        Shift = 0x1,
        Ctrl = 0x2
    }
}
