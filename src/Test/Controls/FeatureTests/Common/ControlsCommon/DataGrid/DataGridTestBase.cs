using System;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid test base.
    /// </summary>
    public abstract class DataGridTestBase : IDisposable
    {
        public DataGridTestInfo DataGridTestInfo { set; get; }
        public abstract void Run();
        public virtual void Dispose() { }
    }
}
