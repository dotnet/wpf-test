// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Documents;
using System.Windows.Threading;

using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Part1.RegressionTest</area>
    /// <owner>Microsoft</owner>
    /// <priority>1</priority>
    /// <description>
    /// Regression coverage for Part1 Regression_Bug49 where applying a negative TextIndent on a TextRange through the property system created an exception.
    /// </description>
    /// </summary>
    [Test(1, "Part1.RegressionTests", "NegativeTextIndentBug", MethodName = "Run")]
    public class NegativeTextIndentBug : AvalonTest
    {
        private TextRange _textRange;
       
        public NegativeTextIndentBug()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(RunTest);           
        }

        /// <summary>
        /// Creates content for the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            Paragraph paragraph = new Paragraph(new Run("This is a Paragraph."));
            _textRange = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);

            return TestResult.Pass;
        }

        /// <summary>
        /// Apply a negative TextIndent to the TextRange.
        /// The test passes if this operation does not create an exception.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {
            _textRange.ApplyPropertyValue(Paragraph.TextIndentProperty, -1.0);

            WaitForPriority(DispatcherPriority.ApplicationIdle);     

            return TestResult.Pass;
        }
    }
}
