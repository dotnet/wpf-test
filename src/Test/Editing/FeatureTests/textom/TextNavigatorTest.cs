// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  TextPointerTest.cs
//  Unit and functional testing for the TextPointer class.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 24 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Common/Library/Wrappers/ElementWrappers.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Threading; using System.Windows.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Verifies the functionality of the TextPointer class.
    ///
    /// Regression_Bug596
    /// Verifies that the TextPointer is not allowed to move beyond
    /// the container boundaries.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("513"),TestBugs("596"),
     WindowlessTest(true)]
    public class TextPointerTests: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            TestBugRegression_Bug596();
            TestGravity();
            TestMove();
            TestMoveToNextCharacter();
            Logger.Current.ReportSuccess();
        }

        private void TestBugRegression_Bug596()
        {
            Log("Setting up TextBlock element with content...");
            TextBlock text = new TextBlock();
            text.TextRange.Text = "content";

            Log("Creating navigator and moving past end...");
            TextPointer n = text.TextRange.Start.CreateNavigator();
            try
            {
                n.MoveByDistance(8);
            }
            catch(ArgumentException)
            {
                // An ArgumentException is acceptable at this point.
                Log("ArgumentException thrown.");
                Logger.Current.ReportSuccess();
                return;
            }

            Logger.Current.ReportSuccess();
        }

        private void TestGravity()
        {
            TextContainer container;    // Container for content.
            TextPointer navigator;    // Navigator tested.

            container = CreateContainerFromXaml("12");
            navigator = container.Start.CreateNavigator();

            Log("Verifying Backward gravity...");
            navigator.SetGravity(LogicalDirection.Backward);
            container.InsertTextInRun(navigator, "-");
            Verifier.Verify(navigator.GetTextInRun(LogicalDirection.Backward) == "");
            Verifier.Verify(navigator.GetTextInRun(LogicalDirection.Forward) == "-12");

            Log("Verifying Forward gravity...");
            navigator.SetGravity(LogicalDirection.Forward);
            container.InsertTextInRun(navigator, "+");
            Verifier.Verify(navigator.GetTextInRun(LogicalDirection.Backward) == "+");
            Verifier.Verify(navigator.GetTextInRun(LogicalDirection.Forward) == "-12");
        }

        private void TestMove()
        {
            TextContainer container;    // Container for content.
            TextPointer navigator;    // Navigator tested.

            container = CreateContainerFromXaml("12");
            navigator = container.Start.CreateNavigator();

            Log("Verifying Move for boundaries...");
            Verifier.Verify(!navigator.Move(LogicalDirection.Backward));
            Verifier.Verify(navigator.Move(LogicalDirection.Forward));
            Verifier.Verify(!navigator.Move(LogicalDirection.Forward));

            Log("Verifying Move for elements...");
            container = CreateContainerFromXaml("1<Bold>foo</Bold><Italic>boo</Italic>");
            navigator = container.Start.CreateNavigator();
            Verifier.Verify(!navigator.Move(LogicalDirection.Backward));
            Verifier.Verify(navigator.Move(LogicalDirection.Forward));
            VerifyBeforeAfterText(navigator, "1", "");

            Verifier.Verify(navigator.Move(LogicalDirection.Forward));
            VerifyBeforeAfterText(navigator, "", "foo");

            navigator.MoveByDistance(3 + 1);
            VerifyBeforeAfterText(navigator, "", "");

            Verifier.Verify(navigator.Move(LogicalDirection.Forward));
            VerifyBeforeAfterText(navigator, "", "boo");
            Verifier.Verify(navigator.Move(LogicalDirection.Forward));
            VerifyBeforeAfterText(navigator, "boo", "");

            Verifier.Verify(navigator.Move(LogicalDirection.Backward));
            VerifyBeforeAfterText(navigator, "", "boo");
            Verifier.Verify(navigator.Move(LogicalDirection.Backward));
            VerifyBeforeAfterText(navigator, "", "");

            Log("Verifying Move for nested elements...");
            container = CreateContainerFromXaml("<Bold>f<Italic>o</Italic></Bold>");
            navigator = container.Start.CreateNavigator();
            VerifyBeforeAfterText(navigator, "", "");
            Verifier.Verify(navigator.Move(LogicalDirection.Forward));
            VerifyBeforeAfterText(navigator, "", "f");
            Verifier.Verify(navigator.Move(LogicalDirection.Forward));
            VerifyBeforeAfterText(navigator, "f", "");
            Verifier.Verify(navigator.Move(LogicalDirection.Forward));
            VerifyBeforeAfterText(navigator, "", "o");

            Log("Verifying Move for long text...");
            navigator = container.End;
            navigator.SetGravity(LogicalDirection.Backward);
            container.InsertTextInRun(navigator, new string('-', 1024 * 4));
            container.InsertTextInRun(navigator, "+");
            navigator = container.End;
            VerifyBeforeAfterText(navigator, "+" + new string('-', 1024 * 4), "");

            navigator.Move(LogicalDirection.Forward);
        }

        private void TestMoveToNextCharacter()
        {
            const char CombiningGraveAccent = '\x0300';
            const char CombiningDiaeresisBelow = '\x0324';

            TextContainer container;
            TextPointer navigator;

            container = CreateContainerFromXaml("1a" + CombiningGraveAccent +
                "e" + CombiningGraveAccent + CombiningDiaeresisBelow +
                "  <Bold>foo</Bold>");
            navigator = container.Start.CreateNavigator();

            VerifyPositionsToStart(navigator, 0);

            Log("Moving across simple character...");
            navigator.MoveToNextCharacter(LogicalDirection.Forward);
            VerifyPositionsToStart(navigator, 1);

            Log("Moving across combined pair...");
            navigator.MoveToNextCharacter(LogicalDirection.Forward);
            VerifyPositionsToStart(navigator, 3);

            Log("Moving into combination...");
            navigator.MoveByDistance(-1);
            VerifyPositionsToStart(navigator, 2);

            Log("Moving out of combination...");
            navigator.MoveToNextCharacter(LogicalDirection.Forward);
            VerifyPositionsToStart(navigator, 3);

            Log("Moving into triple combination...");
            navigator.MoveByDistance(1);
            VerifyPositionsToStart(navigator, 4);

            Log("Moving out of combination...");
            navigator.MoveToNextCharacter(LogicalDirection.Forward);
            VerifyPositionsToStart(navigator, 6);

            Log("Moving across spaces..");
            navigator.MoveToNextCharacter(LogicalDirection.Forward);
            VerifyPositionsToStart(navigator, 7);
            navigator.MoveToNextCharacter(LogicalDirection.Forward);
            VerifyPositionsToStart(navigator, 8);

            Log("Moving into element..");
            navigator.MoveToNextCharacter(LogicalDirection.Forward);
            VerifyPositionsToStart(navigator, 10);

            //container = CreateContainerFromXaml("<Bold>\r\n</Bold>");
            container = CreateContainerFromXaml("\r\n");
            navigator = container.Start.CreateNavigator();
            VerifyPositionsToStart(navigator, 0);

            navigator.MoveToNextCharacter(LogicalDirection.Forward);
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>
        /// Counts the number of positions from the specified position
        /// to the start of its container.
        /// </summary>
        /// <param name="position">Position to count from.</param>
        /// <returns>Number of positions from <paramref name="position"/> to start.</returns>
        private int CountPositionsToStart(TextPointer position)
        {
            TextPointer navigator;
            TextPointer start;
            int result;

            System.Diagnostics.Debug.Assert(position != null);

            navigator = position.CreateNavigator();
            start = position.TextContainer.Start;
            result = 0;
            while (navigator > start)
            {
                navigator.MoveByDistance(-1);
                result++;
                if (result > 12000)
                {
                    throw new Exception(
                        "More than 12000 positions counted - possible bug here.");
                }
            }
            return result;
        }

//        /// <summary>
//        /// Creates a new TextContainer object with the specified
//        /// <paramref name="xamlContent"/> in its content.
//        /// </summary>
//        /// <param name="xamlContent">XAML content to populate container with.</param>
//        /// <returns>A new TextContainer instance.</returns>
//        private TextContainer CreateContainerFromXaml(string xamlContent)
//        {
//            TextContainer result;        // Resulting tree with XAML information.
//            TextRange textRange;    // TextRange encompassing content.
//
//            Log("Creating a new container with content [" + xamlContent + "]...");
//            result = new TextContainer();
//            textRange = new TextRange(result.Start, result.End);
//            XamlUtils.SetXamlContent(textRange, xamlContent);
//
//            return result;
//        }

        /// <summary>
        /// Creates a new Container object with the specified
        /// <paramref name="xamlContent"/> in its content.
        /// </summary>
        /// <param name="xamlContent">XAML content to populate container with.</param>
        /// <returns>A new TextPointer which points to Start position of the created container object</returns>
        private TextPointer CreateContainerFromXaml(string xamlContent)
        {
            TextPointer result;        // Resulting tree with XAML information.
            TextRange textRange;    // TextRange encompassing content.
            TextPointer endPtr;

            Log("Creating a new container with content [" + xamlContent + "]...");
            FlowDocumentScrollViewer testPanel = new FlowDocumentScrollViewer();
            textPanel.Document = new FlowDocument();
            result = testPanel.Document.Start;
            endPtr = testPanel.Document.End;
            textRange = new TextRange(result, endPtr);
            XamlUtils.SetXamlContent(textRange, xamlContent);

            return result;
        }

        private void VerifyBeforeAfterText(TextPointer position, string before, string after)
        {
            string actualAfter;     // String after position (according to container)
            string actualBefore;    // String before position (according to container)

            actualBefore = position.GetTextInRun(LogicalDirection.Backward);
            actualAfter = position.GetTextInRun(LogicalDirection.Forward);
            Log("Expecting text before / after position [" + before + "] / [" + after + "]");
            Log("Actual text before / after position [" + actualBefore + "] / [" + actualAfter + "]");
            Verifier.Verify(actualBefore == before);
            Verifier.Verify(actualAfter == after);
        }

        private void VerifyPositionsToStart(TextPointer position, int expectedCount)
        {
            int actualCount;
            bool match;
            string text;

            actualCount = CountPositionsToStart(position);
            match = expectedCount == actualCount;
            Log("Count of positions to start is " + actualCount + ", expected "
                + expectedCount);
            if (!match)
            {
                text = position.GetTextInRun(LogicalDirection.Backward);
                Logger.Current.Log("Text before position ({0} chars): [{1}]", text.Length, text);
                text = position.GetTextInRun(LogicalDirection.Forward);
                Logger.Current.Log("Text after position ({0} chars): [{1}]", text.Length, text);
                throw new Exception("Expected and actual position values do not match.");
            }
        }

        #endregion Helper methods.
    }
}
