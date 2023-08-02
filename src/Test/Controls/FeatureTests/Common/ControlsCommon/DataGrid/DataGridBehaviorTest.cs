using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid behavior test base.
    /// </summary>
    public abstract class DataGridBehaviorTest : DataGridTestBase
    {
        /// <summary>
        /// Template methods defines a contract how we are going to run the tests.
        /// </summary>
        public override void Run()
        {
            Setup();
            PerformAction();
            Validate();
        }

        /// <summary>
        /// Provide default setup service to add a datagrid to the panel.
        /// Concrete tests can override it if they want.
        /// </summary>
        protected virtual void Setup()
        {
            if (DataGridTestInfo.Panel == null)
            {
                throw new TestValidationException("Panel is null.");
            }
            if (DataGridTestInfo.DataGrid == null)
            {
                throw new TestValidationException("DataGrid is null.");
            }
            if (DataGridTestInfo.DataGridAction == null)
            {
                throw new TestValidationException("DataGridAction is null.");
            }

            DataGridTestInfo.Panel.Children.Add(DataGridTestInfo.DataGrid);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        /// <summary>
        /// Make it abstract to defer implementation to concrete tests.
        /// </summary>
        protected abstract void PerformAction();

        /// <summary>
        /// Make it empty virtual because for Exception tests, we don't need to call Validate method.
        /// </summary>
        protected virtual void Validate() { }

        // Concrete tests can override it if they want to implement cleanup no their own.
        public override void Dispose()
        {
            DataGridTestInfo.Panel.Children.Clear();
        }
    }
}
