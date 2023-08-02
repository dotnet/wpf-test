using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Documents;
using SWI = System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Display;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.Actions;
using System.Reflection.Emit;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// <description>
    /// Test coverage for DTS Issue : ??
    /// </summary>
    [Test(1, "ToolTip", "ToolTipRegressionTest72")]
    public class ToolTipRegressionTest72 : XamlTest
    {
        #region Public Members

        public ToolTipRegressionTest72()
            : base(@"ToolTipRegressionTest72.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Toggle_IsOpenAlwaysFalseTooltip);
        }

        public TestResult Setup()
        {
            Status("Setup");

            _btn = RootElement.FindName("_btn") as Button;
            _toolTip = new IsOpenAlwaysFalseToolTip
            {
                Content = "Tooltip"
            };
            ToolTipService.SetToolTip(_btn, _toolTip);

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            return TestResult.Pass;
        }

        #endregion

        #region Tests

        Button _btn;
        ToolTip _toolTip;

        public TestResult Toggle_IsOpenAlwaysFalseTooltip()
        {
            // move mouse into the label
            UserInput.MouseMove(_btn, 5, 5);

            // since IsOpen property is overridden, it should still stay false            
            WaitFor(1000);

            // calls the closing mechanism for the tooltip
            UserInput.MouseLeftDown(_btn, 5, 45);
            WaitForPriority(DispatcherPriority.Background);
            UserInput.MouseLeftUp(_btn, 5, 45);
            WaitForPriority(DispatcherPriority.Background);

            return TestResult.Pass;     // no crash
        }

        #endregion


        public class IsOpenAlwaysFalseToolTip : ToolTip
        {
            static IsOpenAlwaysFalseToolTip()
            {
                System.Windows.Controls.ToolTip
                    .IsOpenProperty
                    .OverrideMetadata(
                        typeof(IsOpenAlwaysFalseToolTip),
                        (PropertyMetadata)new FrameworkPropertyMetadata(
                                                            (object)false,
                                                            (PropertyChangedCallback)null,
                                                            (CoerceValueCallback)((o, v) => (object)false)));
            }
        }
    }
}
