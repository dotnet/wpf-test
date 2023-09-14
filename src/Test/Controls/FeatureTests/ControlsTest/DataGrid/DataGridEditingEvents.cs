using System.Windows.Controls;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Controls.DataSources;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests for the possible events that occur when editing commands are issued.  This
    /// currently tests cell editing.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridEditingEvents", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite,MicroSuite")]
    public class DataGridEditingEvents : DataGridEditingCommands
    {
        #region Constructor

        public DataGridEditingEvents()
            : base()
        {
        }

        #endregion

        #region Helpers

        protected override void DoBeginEditing(int row, int col, DataGridCommandHelper.BeginEditAction action)
        {
            DataGridBeginningEditEventArgs expectedArgs = null;
            DataGridBeginningEditEventArgs actualEventArgs = null;

            this.DoBeginAndRecordEvent(row, col, action, out expectedArgs, out actualEventArgs);
            this.VerifyBeginEventArgs(expectedArgs, actualEventArgs);
        }

        protected override void DoCancelEditing(int row, int col, DataGridCommandHelper.CancelEditAction action)
        {
            DataGridCellEditEndingEventArgs expectedCellEventArgs = null;
            DataGridCellEditEndingEventArgs actualCellEventArgs = null;
            DataGridRowEditEndingEventArgs expectedRowEventArgs = null;
            DataGridRowEditEndingEventArgs actualRowEventArgs = null;

            this.DoCancelAndRecordEvent(
                row,
                col,
                action,
                out expectedCellEventArgs,
                out actualCellEventArgs,
                out expectedRowEventArgs,
                out actualRowEventArgs);

            this.VerifyEndingEventArgs(
                expectedCellEventArgs,
                actualCellEventArgs,
                expectedRowEventArgs,
                actualRowEventArgs);
        }

        protected override void DoCommitEditing(int row, int col, DataGridCommandHelper.CommitEditAction action)
        {
            DataGridCellEditEndingEventArgs expectedCellEventArgs = null;
            DataGridCellEditEndingEventArgs actualCellEventArgs = null;
            DataGridRowEditEndingEventArgs expectedRowEventArgs = null;
            DataGridRowEditEndingEventArgs actualRowEventArgs = null;

            this.DoCommitAndRecordEvent(
                row,
                col,
                action,
                out expectedCellEventArgs,
                out actualCellEventArgs,
                out expectedRowEventArgs,
                out actualRowEventArgs);

            this.VerifyEndingEventArgs(
                expectedCellEventArgs,
                actualCellEventArgs,
                expectedRowEventArgs,
                actualRowEventArgs);
        }

        #endregion Helpers
    }
}
