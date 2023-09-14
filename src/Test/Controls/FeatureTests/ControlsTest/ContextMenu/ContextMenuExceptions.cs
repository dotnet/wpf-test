using System;
using System.Windows.Controls;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// <description>
    /// ContextMenuExceptions
    /// </description>
    /// </summary>
    [Test(0, "ContextMenu", "Exceptions")]
    public class ContextMenuExceptions : StepsTest
    {
        #region Public Members

        public ContextMenuExceptions()
        {
            RunSteps += new TestStep(TestExceptions);
        }

        public TestResult TestExceptions()
        {
            ContextMenu contextMenu = new ContextMenu();
            ContextMenuActions.TestExceptions(contextMenu);

            return TestResult.Pass;
        }

        #endregion
    } 
}
