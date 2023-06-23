using System.Windows.Controls;
using Microsoft.Test.Controls.Actions;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid test info.
    /// </summary>
    public struct DataGridTestInfo
    {
        public Panel Panel { set; get; }
        public DataGrid DataGrid { set; get; }
        public DataGridSelectionMode DataGridSelectionMode { set; get; }
        public DataGridBuilder DataGridBuilder { set; get; }
        public DataGridAction DataGridAction { set; get; }
    }
}
