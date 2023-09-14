using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Template for generic XamlTests
    /// </description>
    /// </summary>
    public class RegressionXamlTest : XamlTest
    {
        protected Button debugButton;

        #region Constructor

        public RegressionXamlTest(string fileName)
            : base(fileName)
        {
            CleanUpSteps += new TestStep(CleanUp);
        }

        #endregion

        #region Setup

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public virtual TestResult Setup()
        {
            Status("RegressionXamlTest Setup");

            debugButton = (Button)RootElement.FindName("btn_Debug");            
            if (debugButton != null)
            {
                Debug();
            }
            else
            {
                LogComment("Unable to freeze test for ad-hoc debugging.  Xaml file must include a button named \"btn_Debug\".");
            }

            LogComment("RegressionXamlTest Setup was successful");
            return TestResult.Pass;
        }

        public virtual TestResult CleanUp()
        {
            debugButton = null;
            return TestResult.Pass;
        }

        #endregion Setup
       
        #region Helpers

        private void Debug()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            debugButton.MouseLeftButtonDown += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }

       

        #endregion Helpers
    }

    

    
}
