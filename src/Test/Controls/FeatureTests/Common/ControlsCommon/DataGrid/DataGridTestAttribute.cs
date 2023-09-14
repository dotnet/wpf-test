using System;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid test Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class DataGridTestAttribute : Attribute
    {
        public Type DataGridType { set; get; }
        public DataGridFeature DataGridFeature { set; get; }
        public DataGridTestType DataGridTestType { set; get; }
    }
}
