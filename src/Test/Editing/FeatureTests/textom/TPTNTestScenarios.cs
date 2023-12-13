// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional testing for the TextContainer class.

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
    using System.Windows.Shapes;
    
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    #endregion Namespaces.

    /// <summary>
    /// This is the base class for TextPointer (TP) and TextPointer (TN) test cases. It has some helper functions 
    /// to verify the validity of like TP and TN.
    /// </summary>
    public abstract class TPTNTestBase : CustomTestCase
    {
        /// <summary>Initializes a new TPTNTestBase instance.</summary>
        public TPTNTestBase() : base()
        {
        }

        #region Protected Properties
        /// <summary>TextTree</summary>
        protected TextContainer TextTree
        {
            get { return this._textTree; }
            set { this._textTree = value; }
        }

        /// <summary>LogicalDirection.Forward</summary>
        protected LogicalDirection Forward
        {
            get { return LogicalDirection.Forward; }
        }

        /// <summary>LogicalDirection.Backward</summary>
        protected LogicalDirection Backward
        {
            get { return LogicalDirection.Backward; }
        }
        #endregion Protected Properties

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
        }

        /// <summary>
        /// Verifies the GetTextInRun method of the TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="direction">Input paramenter LogicalDirection</param>
        /// <param name="expText">Expected Test</param>
        public void VerifyGetTextInRun(TextPointer testTP, LogicalDirection direction, String expText)
        {
            String testString = testTP.GetTextInRun(direction);
            string message = "Verifying the GetTextInRun() method in " + direction.ToString() + ".\n";
            message += "Expected: [" + expText + "] " + " Actual: [" + testString + "]\n";
            Verifier.Verify(testString == expText, message, false);
        }

        /// <summary>
        /// Verifies the GetTextRunLength method of TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="direction">Input paramenter LogicalDirection</param>
        /// <param name="expLength">Expected Length</param>
        public void VerifyGetTextRunLength(TextPointer testTP, LogicalDirection direction, int expLength)
        {
            int testLength = testTP.GetTextRunLength(direction);
            String message = "Verifying GetTextRunLength() method in " + direction.ToString() + ".\n";
            message += "Expected: [" + expLength + "] " + "Actual: [" + testLength + "]\n";
            Verifier.Verify(testLength==expLength, message, false);
        }

        /// <summary>
        /// Verifies the GetOffsetToPosition method of TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="toTP">Input TextPointer argument for GetTextRunLength method</param>
        /// <param name="expDistance">Expected distance</param>
        public void VerifyGetOffsetToPosition(TextPointer testTP, TextPointer toTP, int expDistance)
        {
            int testDistance = testTP.GetOffsetToPosition(toTP);
            String message = "Verifying GetOffsetToPosition() method.\n";
            message += "Expected: [" + expDistance + "] " + "Actual: [" + testDistance + "]\n";
            Verifier.Verify(testDistance==expDistance, message, false);
        }

        /// <summary>
        /// Verifies the GetElementType method of TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="expType">Expected Element type</param>
        public void VerifyGetElementType(TextPointer testTP, Type expType)
        {
            Type testType;
            testType = testTP.GetElementType();
            
            String message = "Verifying GetElementType() method.\n";
            if ((expType != null)&&(testType != null))
            {
                message += "Expected: [" + expType.ToString() + "] " + "Actual: [" + testType.ToString() + "]\n";
            }
            if(expType==null)
            {
                message += "Expected: [null] \n";
            }
            if (testType == null)
            {
                message += "Actual: [null] \n";
            }
            Verifier.Verify(testType==expType, message, false);
        }

        /// <summary>
        /// Verifies the GetElementType method of TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="direction">Input paramenter LogicalDirection</param>
        /// <param name="expType">Expected Element type</param>
        public void VerifyGetElementType(TextPointer testTP, LogicalDirection direction, Type expType)
        {
            Type testType;
            testType = testTP.GetElementType(direction);
            String message = "Verifying GetElementType() method.\n";
            if ((expType != null) && (testType != null))
            {
                message += "Expected: [" + expType.ToString() + "] " + "Actual: [" + testType.ToString() + "]\n";
            }
            if (expType == null)
            {
                message += "Expected: [null] \n";
            }
            if (testType == null)
            {
                message += "Actual: [null] \n";
            }
            Verifier.Verify(testType == expType, message, false);
        }

        /// <summary>
        /// Verifies the GetEmbeddedObject method of TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="direction">Input paramenter LogicalDirection</param>
        /// <param name="expObject">Expected embedded object</param>
        public void VerifyGetEmbeddedObject(TextPointer testTP, LogicalDirection direction, object expObject)
        {
            object testObject = testTP.GetEmbeddedObject(direction);
            String message = "Verifying GetEmbeddedObject() in  " + direction.ToString() + ".\n";
            message += "Expected: [" + expObject.GetType().ToString()+ "] " + "Actual: [" + testObject.GetType().ToString() + "]\n";
            Verifier.Verify(testObject.GetType().ToString()==expObject.GetType().ToString(), message, false);
        }

        /// <summary>
        /// Verifies GetElementValue method of TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="direction">Input paramenter LogicalDirection</param>
        /// <param name="dp">Dependency property to be checked</param>
        /// <param name="expValue">Expected value for the dependency property</param>
        public void VerifyGetElementValue(TextPointer testTP, LogicalDirection direction, DependencyProperty dp, object expValue)
        {
            object testValue = testTP.GetElementValue(direction, dp);
            String message = "Verifying GetElementValue() in  " + direction.ToString() + ".\n";
            message += "Expected: [" + expValue.ToString() + "] " + "Actual: [" + testValue.ToString() + "]\n";
            Verifier.Verify(testValue==expValue, message, false);
        }

        /// <summary>
        /// Verifies the GetSymbolType method of TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="direction">Input paramenter LogicalDirection</param>
        /// <param name="expSymbolType">Expected TextPointerContext</param>
        public void VerifyGetSymbolType(TextPointer testTP, LogicalDirection direction, TextPointerContext expSymbolType)
        {
            TextPointerContext testSymbolType = testTP.GetSymbolType(direction);
            String message = "Verifying GetSymbolType() in " + direction.ToString() + ".\n";
            message += "Expected: [" + expSymbolType.ToString() + "] " + "Actual: [" + testSymbolType.ToString() + "]\n";
            Verifier.Verify(testSymbolType==expSymbolType, message, false);

            if (expSymbolType == TextPointerContext.Text)
            {
                Verifier.Verify(testTP.IsAtCharacterBoundary, "Verifying IsAtCharacterBoundary when it should return true", false);
            }            
        }

        /// <summary>
        /// Verifies that testTP is located between start and end
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="start">start TextPointer</param>
        /// <param name="end">end TextPointer</param>
        public void VerifyPosition(TextPointer testTP, TextPointer start, TextPointer end)
        {
            String message = "Verifying that the TextPointer is between the start and end provided.";
            Verifier.Verify((TextPointer.LessThanOrEqual(start,testTP))&&(TextPointer.LessThanOrEqual(testTP,end)), message, false);
            Verifier.Verify(TextPointer.Max(testTP, start) == testTP, "VerifyPosition(): Verifying using Max method", false);
            Verifier.Verify(TextPointer.Min(testTP, end) == testTP, "VerifyPosition(): Verifying using Min method", false);            
        }

        /// <summary>
        /// Verifies TextPointer.Equal method
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="tp">TextPointer to which testTP should be equal</param>
        public void VerifyEqual(TextPointer testTP, TextPointer tp)
        {
            String message = "Verifying TextPointer.Equal method";
            Verifier.Verify(TextPointer.Equal(testTP, tp), message, false);
            Verifier.Verify(testTP.Equals(tp), "Verifying Equals method", false);
            Verifier.Verify(TextPointer.Max(testTP, tp) == testTP, "Verifying TextPointer.Max method", false);
            Verifier.Verify(TextPointer.Min(testTP, tp) == testTP, "Verifying TextPointer.Min method", false);

        }

        /// <summary>
        /// Verifies the gravity of a TextPointer
        /// </summary>
        /// <param name="testTP">Textposition to be tested</param>
        /// <param name="direction">Expected LogicalDirection</param>
        public void VerifyGravity(TextPointer testTP, LogicalDirection direction)
        {
            string message = "Verify the Gravity of the TextPointer.";
            Verifier.Verify(testTP.Gravity == direction, message, false);
        }
        private TextContainer _textTree = null;
    }

    //CommandLine: /TestCaseType:TPTest_TextEditing /TestName:TPTest_TextEditing-Simple
    /// <summary>
    /// This test case does various types of verification  on the functionality of TextPointer 
    /// in a simple Text editing scenario
    /// </summary>
    [Test(0, "TextOM", "TPTest_TextEditing", MethodParams = "/TestCaseType=TPTest_TextEditing /TestName=TPTest_TextEditing-Simple")]
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("TestText1", "Text to be inserted in the TextContainer"),
    TestArgument("TestText2", "Text to be inserted in the TextContainer"),
    TestTactics("140"), TestLastUpdatedOn("Jan 25, 2007")]
    public class TPTest_TextEditing : TPTNTestBase
    {
        #region Settings
        /// <summary>Text to be inserted in the TextContainer</summary>
        private string TestText1
        {
            get { return Settings.GetArgument("TestText1"); }
        }

        /// <summary>Text to be inserted in the TextContainer</summary>
        private string TestText2
        {
            get { return Settings.GetArgument("TestText2"); }
        }
        #endregion Settings

        #region Members
        TextPointer _tpForward = null;
        TextPointer _tpBackward = null;
        TextPointer _tp1 = null;
        TextPointer _tp2 = null;
        TextPointer _tpCenter = null;
        #endregion Members

        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            TextTree = new TextContainer();
            System.Text.StringBuilder checkString = new System.Text.StringBuilder(TestText1);

            #region Step1: Testing TextPointers on empty TextContainer
            //Present TextContainer state: <TT></TT>
            Log("Step1: Testing TextPointers on empty TextContainer");
            
            _tp1 = TextTree.Start;            
            _tp2 = TextTree.End;

            VerifyPosition(_tp1, TextTree.Start, TextTree.End);
            VerifyPosition(_tp2, TextTree.Start, TextTree.End);

            VerifyGetOffsetToPosition(_tp1, TextTree.Start, 0);
            VerifyGetOffsetToPosition(_tp1, TextTree.End, 0);
            VerifyGetOffsetToPosition(_tp1, _tp2, 0);

            //Create TextPointers in reverse order and verify that everything is ok
            _tp1 = TextTree.Start.CreatePosition(Forward);
            _tp2 = TextTree.End.CreatePosition(Backward);

            VerifyPosition(_tp1, TextTree.Start, TextTree.End);
            VerifyPosition(_tp2, TextTree.Start, TextTree.End);

            VerifyGetOffsetToPosition(_tp1, TextTree.Start, 0);
            VerifyGetOffsetToPosition(_tp1, TextTree.End, 0);
            VerifyGetOffsetToPosition(_tp1, _tp2, 0);

            //Set the TextPointers to normal way to continue testing.
            _tp1 = TextTree.Start;
            _tp2 = TextTree.End;
            #endregion Step1: Testing TextPointers on empty TextContainer

            #region Step2: Insert text and test TP's
            //Present TextContainer state: <TT><Start=tp1><End=tp2></TT>
            Log("Step2: Insert text and test TP's");
            TextTree.InsertText(TextTree.Start, TestText1);

            VerifyPosition(_tp1, TextTree.Start, TextTree.End);
            VerifyGetTextInRun(_tp1, Forward, TestText1);
            VerifyGetTextInRun(_tp1, Backward, String.Empty);
            VerifyGetTextRunLength(_tp1, Forward, TestText1.Length);
            VerifyGetOffsetToPosition(_tp1, TextTree.End, TestText1.Length);
            VerifyGetSymbolType(_tp1, Forward, TextPointerContext.Text);
            VerifyGetSymbolType(_tp1, Backward, TextPointerContext.None);

            VerifyPosition(_tp2, TextTree.Start, TextTree.End);
            VerifyGetTextInRun(_tp2, Backward, TestText1);
            VerifyGetTextInRun(_tp2, Forward, String.Empty);
            VerifyGetTextRunLength(_tp2, Backward, TestText1.Length);
            VerifyGetOffsetToPosition(_tp2, TextTree.Start, -TestText1.Length);
            VerifyGetSymbolType(_tp2, Backward, TextPointerContext.Text);
            VerifyGetSymbolType(_tp2, Forward, TextPointerContext.None);
            #endregion Step2: Insert text and test TP's

            #region Step3: Insert text in the middle of existing contents
            //Present TextContainer state: <TT><Start=tp1>[TestText1]<End=tp2></TT>
            Log("Step3: Insert text in the middle of TextContainer with two TextPointer with different gravity");

            //Create two textpositions at same location but with different gravity's
            int offSet = TestText1.Length / 2;
            _tpBackward = TextTree.Start.CreatePosition(offSet, Backward);
            _tpForward = TextTree.End.CreatePosition(-offSet, Forward);

            VerifyGetOffsetToPosition(_tpBackward, _tpForward, 0);

            TextTree.InsertText(_tpBackward, TestText2);
            checkString.Insert(offSet, TestText2);

            VerifyGetTextInRun(_tpBackward, Backward, checkString.ToString().Substring(0, offSet));
            VerifyGetTextInRun(_tpBackward, Forward, checkString.ToString().Substring(offSet));
            VerifyGetTextInRun(_tpForward, Backward, checkString.ToString().Substring(0, offSet + TestText2.Length));
            VerifyGetTextInRun(_tpForward, Forward, checkString.ToString().Substring(offSet + TestText2.Length));

            VerifyGetTextInRun(TextTree.Start, Forward, checkString.ToString());
            #endregion Step3: Insert text in the middle of existing contents

            #region Step4: Delete contents with a TextPointer placed inside.
            //Present TextContainer state: <TT><Start=tp1>[TestText1.1sthalf]<tpBackward>[TestText2]<tpForward>[TestText1.2ndhalf]<End=tp2></TT>
            Log("Step4: Delete content with a TextPointer placed between the content. Start and End TP's with opp. gravity.");
            _tpCenter = TextTree.Start.CreatePosition(offSet + (TestText2.Length / 2), Forward);

            VerifyGetTextRunLength(_tpCenter, Backward, (offSet + (TestText2.Length / 2)));
            VerifyPosition(_tpCenter, _tpBackward, _tpForward);

            TextTree.DeleteContent(_tpBackward, _tpForward);
            checkString.Remove(offSet, TestText2.Length);

            VerifyGravity(_tpCenter, Forward);
            VerifyEqual(_tpCenter, _tpBackward);
            VerifyGetOffsetToPosition(_tpCenter, _tpForward, 0);
            VerifyGetOffsetToPosition(_tpBackward, TextTree.Start, -offSet);
            VerifyGetTextInRun(_tpCenter, Backward, checkString.ToString().Substring(0, offSet));
            VerifyGetTextInRun(_tpCenter, Forward, checkString.ToString().Substring(offSet));
            VerifyGetSymbolType(_tpCenter, Forward, TextPointerContext.Text);
            VerifyGetSymbolType(_tpCenter, Backward, TextPointerContext.Text);
            #endregion Step4: Delete contents with a TextPointer placed inside.

            #region Step5: Delete content with start and end TP's having same gravity
            //Present TextContainer state: <TT><Start=tp1>[TestText1.1sthalf]<tpBackward><tpCenter=tpForward>[TestText1.2ndhalf]<End=tp2></TT>
            Log("Step5: Delete content with start and end TP's having same gravity");

            TextTree.DeleteContent(_tpForward, _tp2);
            checkString.Remove(offSet, offSet);

            VerifyEqual(_tpBackward, _tpForward);
            VerifyGetOffsetToPosition(_tpCenter, _tpForward, 0);
            VerifyGetOffsetToPosition(_tpForward, _tp2, 0);
            VerifyGetTextInRun(_tp1, Forward, checkString.ToString());
            VerifyGetTextInRun(_tpCenter, Backward, checkString.ToString());
            VerifyGravity(_tpForward, Forward);
            VerifyGetSymbolType(_tpCenter, Forward, TextPointerContext.None);
            #endregion Step5: Delete content with start and end TP's having same gravity

            #region Step6: Bogus delete with same TP's as start and end
            //Present TextContainer state: <TT><Start=tp1>[TestText1.1sthalf]<tpBackward><tpCenter=tpForward=tp2=End></TT>
            Log("Step6: Bogus delete with same TP's as start and end");

            TextTree.DeleteContent(_tpCenter, _tpCenter);

            VerifyGetTextInRun(_tp1, Forward, checkString.ToString());
            VerifyGravity(_tpCenter, Forward);
            VerifyEqual(_tpCenter, _tpCenter);
            VerifyEqual(_tpCenter, _tpBackward);

            TextTree.DeleteContent(_tpCenter, _tpForward);

            VerifyGetTextInRun(_tp1, Forward, checkString.ToString());
            VerifyGravity(_tp2, Forward);
            VerifyEqual(_tpCenter, _tp2);

            #endregion Step6: Bogus delete with same TP's as start and end

            #region Step7: Delete just one character
            //Present TextContainer state: <TT><Start=tp1>[TestText1.1sthalf]<tpBackward><tpCenter=tpForward=tp2=End></TT>
            Log("Step7: Delete just one character");
            _tp2 = TextTree.End.CreatePosition(-1, Forward);
            VerifyPosition(_tp2, _tp1, _tpForward);
            VerifyGetOffsetToPosition(_tp2, TextTree.End, 1);

            TextTree.DeleteContent(_tp2, _tpForward);
            checkString.Remove(offSet - 1, 1);

            VerifyEqual(_tp2, _tpForward);
            VerifyGetTextInRun(_tp2, Backward, checkString.ToString());
            #endregion Step7: Delete just one character

            #region Step8: Delete all the contents
            //Present TextContainer state: <TT><Start=tp1>[TestText1.1sthalf - 1char]<tpBackward><tpCenter=tpForward=tp2=End></TT>
            Log("Step8: Delete all the contents");
            TextTree.DeleteContent(_tp1, _tp2);

            VerifyGetOffsetToPosition(_tp1, TextTree.End, 0);
            VerifyEqual(TextTree.Start, _tp2);
            VerifyEqual(_tpBackward, _tpForward);
            #endregion Step8: Delete all the contents

            Logger.Current.ReportSuccess();
        }
    }

    //CommandLine: /TestCaseType:TPTest_TextElementEditing /TestName:TPTest_TextElementEditing-Simple
    /// <summary>
    /// This test case verifies the functionality of TextPointer in a TextElement editing scenario
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("TestText1", "Text to be inserted in the TextContainer"),
    TestArgument("TextElementTypeName", "Type of TextElement to be used in the test"),
    TestArgument("TestFontFamily", "Name of FontFamily to be used in the test"),
    TestArgument("TestProperty", "Name of property to be used in the test"),
    TestTactics("141"), 
    TestLastUpdatedOn("Jan 25, 2007")]
    public class TPTest_TextElementEditing : TPTNTestBase
    {
        #region Settings
        /// <summary>Type of TextElement to be used in the test</summary>
        private string TextElementTypeName
        {
            get { return Settings.GetArgument("TextElementTypeName"); }
        }

        /// <summary>Name of FontFamily to be used in the test</summary>
        private string TestFontFamily
        {
            get { return Settings.GetArgument("TestFontFamily"); }
        }

        /// <summary>Name of property to be used in the test</summary>
        private string TestProperty
        {
            get { return Settings.GetArgument("TestProperty"); }
        }

        /// <summary>Text to be used in the test</summary>
        private string TestText1
        {
            get { return Settings.GetArgument("TestText1"); }
        }
        #endregion Settings

        #region Members
        Type _elementType = null;

        TextPointer _tpForward = null;
        TextPointer _tpBackward = null;
        TextPointer _tp1 = null;
        TextPointer _tp2 = null;
        TextPointer _tpCenter = null;
        #endregion Members

        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            TextTree = new TextContainer();
            System.Text.StringBuilder checkString = new System.Text.StringBuilder(TestText1);

            _elementType = ReflectionUtils.FindType(TextElementTypeName);
            object elementObj1 = ReflectionUtils.CreateInstance(_elementType);

            #region Step1: Testing TP with a textelement in an empty tree
            //Present TextContainer state: <TT></TT>
            Log("Step1: Testing TP with a textelement in an empty tree");
            _tp1 = TextTree.Start.CreatePosition();
            TextTree.InsertElement(_tp1, TextTree.End, ((TextElement)elementObj1));
            _tp2 = TextTree.Start.CreatePosition(1, Forward);

            VerifyGetSymbolType(_tp1, Forward, TextPointerContext.ElementStart);
            VerifyGetSymbolType(_tp2, Backward, TextPointerContext.ElementStart);
            VerifyGetSymbolType(_tp2, Forward, TextPointerContext.ElementEnd);
            VerifyGetSymbolType(TextTree.End, Backward, TextPointerContext.ElementEnd);

            VerifyGetElementType(_tp2, _elementType);
            VerifyGetElementType(_tp1, Forward, _elementType);
            VerifyGetElementType(_tp1, Backward, null);

            VerifyGetOffsetToPosition(_tp2, _tp1, -1);
            VerifyGetTextRunLength(_tp2, Backward, 0);

            ReflectionUtils.SetProperty(elementObj1, TestProperty, TestFontFamily);
            VerifyGetElementValue(_tp2, Backward, TextElement.FontFamilyProperty, new FontFamily(TestFontFamily));
            VerifyGetElementValue(_tp2, Forward, TextElement.FontFamilyProperty, new FontFamily(TestFontFamily));
            VerifyGetElementValue(_tp1, Forward, TextElement.FontFamilyProperty, new FontFamily(TestFontFamily));
            #endregion Step1: Testing TP with a textelement in an empty tree

            #region Step2: Insert text into the TextElement.
            //Present TextContainer state: <TT><tp1><elementObj1><tp2></elementObj1></TT>
            Log("Step2: Insert text into the TextElement.");
            ((TextElement)elementObj1).Append(TestText1);

            VerifyGetOffsetToPosition(_tp1, TextTree.Start, 0);
            VerifyGetOffsetToPosition(_tp1, _tp2, TestText1.Length + 1);
            VerifyGetTextInRun(_tp2, Backward, TestText1);
            VerifyGetSymbolType(_tp2, Backward, TextPointerContext.Text);
            VerifyGetSymbolType(_tp2, Forward, TextPointerContext.ElementEnd);

            _tpCenter = TextTree.Start.CreatePosition((TestText1.Length / 2) + 1, Backward);

            VerifyGetTextInRun(_tpCenter, Backward, TestText1.Substring(0, TestText1.Length / 2));
            VerifyGetTextInRun(_tpCenter, Forward, TestText1.Substring(TestText1.Length / 2));
            VerifyGetElementType(_tpCenter, _elementType);
            #endregion Step2: Insert text into the TextElement.

            #region Step3: Insert text outsite the TextElement
            //Present TextContainer state: <TT><tp1><elementObj1>[TestText1.1sthalf]<tpCenter>[TestText1.2ndhalf]<tp2></elementObj1></TT>
            Log("Step3: Insert text outsite the TextElement");
            TextTree.InsertText(_tp1, TestText1);

            VerifyGetTextInRun(_tp1, Forward, TestText1);
            VerifyGetOffsetToPosition(_tp1, _tp2, (2 * TestText1.Length) + 1);
            VerifyGetSymbolType(_tp1, Forward, TextPointerContext.Text);
            VerifyGetElementType(_tp1, null);
            #endregion Step3: Insert text outsite the TextElement

            #region Step4: Insert a nested text element
            //Present TextContainer state: <TT><tp1>[TestText1]<elementObj1>[TestText1.1sthalf]<tpCenter>
            //[TestText1.2ndhalf]<tp2></elementObj1><TT>
            Log("Step4: Insert a nested text element");
            object elementObj2 = ReflectionUtils.CreateInstance(_elementType);
            TextTree.InsertElement(_tpCenter, _tp2, (TextElement)elementObj2);

            VerifyGetSymbolType(_tpCenter, Forward, TextPointerContext.ElementStart);
            VerifyGetSymbolType(_tpCenter, Backward, TextPointerContext.Text);
            VerifyGetSymbolType(_tp2, Backward, TextPointerContext.ElementEnd);
            VerifyGetTextRunLength(_tpCenter, Forward, 0);
            VerifyGetOffsetToPosition(_tp1, _tp2, (2 * TestText1.Length) + 3);

            object elementObj3 = ReflectionUtils.CreateInstance(_elementType);
            _tpForward = _tp1.CreatePosition(TestText1.Length, Forward);

            VerifyGetTextInRun(_tpForward, Backward, TestText1);

            TextTree.InsertElement(_tp1, _tpForward, (TextElement)elementObj3);

            VerifyGetSymbolType(_tpForward, Backward, TextPointerContext.ElementEnd);
            VerifyGetSymbolType(_tpForward, Forward, TextPointerContext.ElementStart);
            VerifyGetOffsetToPosition(_tpForward, _tp1, -(TestText1.Length + 2));

            _tpBackward = _tp1.CreatePosition((TestText1.Length / 2) + 1, Backward);

            VerifyGetElementType(_tpBackward, _elementType);
            #endregion Step4: Insert a nested text element

            #region Step5: Extract an element with TextPointers inside it.
            //Present TextContainer state: <TT><tp1><elementObj3>[TestText1.1sthalf]<tpBackward>[TestText1.2ndhalf]</elementObj3>
            //<tpForward><elementObj1>[TestText1.1sthalf]<tpCenter><elementObj2>[TestText1.2ndhalf]</elementObj2><tp2></elementObj1><TT>
            Log("Step5: Extract an element with TextPointers inside it.");
            TextTree.ExtractElement((TextElement)elementObj3);

            VerifyGetElementType(_tpBackward, Backward, null);
            VerifyGetOffsetToPosition(_tp1, _tpForward, TestText1.Length);
            VerifyGetSymbolType(_tpForward, Backward, TextPointerContext.Text);

            #endregion Step5: Extract an element with TextPointers inside it.

            #region Step6: Delete contents between two textPositions and with a textelement inside it.
            //Present TextContainer state: <TT><tp1>[TestText1.1sthalf]<tpBackward>[TestText1.2ndhalf]
            //<tpForward><elementObj1>[TestText1.1sthalf]<tpCenter><elementObj2>[TestText1.2ndhalf]</elementObj2><tp2></elementObj1><TT>
            Log("Step6: Delete contents between two textPositions and with a textelement inside it.");
            TextTree.DeleteContent(_tpCenter, _tp2);

            VerifyEqual(_tpCenter, _tp2);
            VerifyGetSymbolType(_tpCenter, Forward, TextPointerContext.ElementEnd);
            #endregion Step6: Delete contents between two textPositions and with a textelement inside it.

            #region Step7: Delete everything and check the positions of all TP's
            //Present TextContainer state: <TT><tp1>[TestText1.1sthalf]<tpBackward>[TestText1.2ndhalf]
            //<tpForward><elementObj1>[TestText1.1sthalf]<tpCenter><tp2></elementObj1><TT>
            Log("Step7: Delete everything and check the positions of all TP's");
            TextTree.DeleteContent(TextTree.Start, TextTree.End);
            VerifyEqual(TextTree.Start, _tp1);
            VerifyEqual(_tp1, _tpForward);
            VerifyEqual(_tpForward, _tpCenter);
            VerifyEqual(_tpCenter, TextTree.End);
            VerifyGetOffsetToPosition(_tpForward, TextTree.End, 0);
            #endregion Step7: Delete everything and check the positions of all TP's

            Logger.Current.ReportSuccess();
        }
    }

    //CommandLine: /TestCaseType:TPTest_UIElementEditing /TestName:TPTest_UIElementEditing-Simple
    /// <summary>
    /// This test case verifies the functionality of TextPointer in a UIElement editing scenario.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("TestText1", "Text to be inserted in the TextContainer"),
    TestArgument("TextElementTypeName", "Type of TextElement to be used in the test"),
    TestArgument("UIElementTypeName", "Type of UIElement to be used in the test")]
    public class TPTest_UIElementEditing : TPTNTestBase
    {
        #region Settings
        /// <summary>Type of UIElement to be used in the test</summary>
        private string UIElementTypeName
        {
            get { return Settings.GetArgument("UIElementTypeName"); }
        }

        /// <summary>Type of TextElement to be used in the test</summary>
        private string TextElementTypeName
        {
            get { return Settings.GetArgument("TextElementTypeName"); }
        }

        /// <summary>Text to be used in the test</summary>
        private string TestText1
        {
            get { return Settings.GetArgument("TestText1"); }
        }
        #endregion Settings

        #region Members
        Type _elementType = null;
        Type _objectType = null;
        TextPointer _tpForward = null;
        TextPointer _tpBackward = null;
        TextPointer _tp1 = null;
        #endregion Members

        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            TextTree = new TextContainer();
            System.Text.StringBuilder checkString = new System.Text.StringBuilder(TestText1);

            _objectType = ReflectionUtils.FindType(UIElementTypeName);
            object embObj1 = ReflectionUtils.CreateInstance(_objectType);

            #region Step1: Testing TP with a single UIElement in an empty tree
            //Present Tree State: <TT></TT>
            Log("Step1: Testing TP with a single uielement in an empty tree");
            TextTree.InsertEmbeddedObject(TextTree.Start, embObj1);

            VerifyGetOffsetToPosition(TextTree.Start, TextTree.End, 1);
            VerifyGetSymbolType(TextTree.Start, Forward, TextPointerContext.EmbeddedElement);
            VerifyGetSymbolType(TextTree.End, Backward, TextPointerContext.EmbeddedElement);
            VerifyGetEmbeddedObject(TextTree.Start, Forward, embObj1);
            VerifyGetEmbeddedObject(TextTree.End, Backward, embObj1);
            #endregion Step1: Testing TP with a single UIElement in an empty tree

            #region Step2: Insert one more instance of UIElement
            //Present Tree State: <TT><embObj1></TT>
            Log("Step2: Insert one more instance of UIElement");
            object embObj2 = ReflectionUtils.CreateInstance(_objectType);
            TextTree.InsertEmbeddedObject(TextTree.End, embObj2);

            VerifyGetOffsetToPosition(TextTree.Start, TextTree.End, 2);

            _tpForward = TextTree.Start.CreatePosition(1, Forward);
            _tpBackward = TextTree.Start.CreatePosition(1, Backward);

            VerifyGetOffsetToPosition(_tpForward, TextTree.Start, -1);
            VerifyGetSymbolType(_tpForward, Forward, TextPointerContext.EmbeddedElement);
            VerifyGetSymbolType(_tpForward, Backward, TextPointerContext.EmbeddedElement);
            VerifyGetSymbolType(_tpBackward, Forward, TextPointerContext.EmbeddedElement);
            VerifyEqual(_tpForward, _tpBackward);

            VerifyGetEmbeddedObject(_tpForward, Backward, embObj1);
            VerifyGetEmbeddedObject(_tpForward, Forward, embObj2);
            VerifyGetEmbeddedObject(_tpBackward, Forward, embObj2);
            #endregion Step2: Insert one more instance of UIElement

            #region Step3: Insert text into TextContainer
            //Present Tree State: <TT><embObj1><tpBackward><tpForward><embObj2></TT>
            Log("Step3: Insert text into TextContainer");
            TextTree.InsertText(TextTree.Start, TestText1);

            _tp1 = TextTree.Start.CreatePosition(TestText1.Length, Backward);
            VerifyGetSymbolType(_tp1, Forward, TextPointerContext.EmbeddedElement);
            VerifyGetTextInRun(_tp1, Backward, TestText1);
            VerifyGetOffsetToPosition(_tp1, _tpBackward, 1);

            TextTree.InsertText(_tpBackward, TestText1);
            VerifyGetOffsetToPosition(_tpBackward, _tpForward, TestText1.Length);
            VerifyGetSymbolType(_tpForward, Backward, TextPointerContext.Text);
            VerifyGetSymbolType(_tpForward, Forward, TextPointerContext.EmbeddedElement);
            #endregion Step3: Insert text into TextContainer

            #region Step4: Insert a textelement which scopes a UIElement
            //Present Tree State: <TT>[TestText1]<tp1><embObj1><tpBackward>[TestText1]<tpForward><embObj2></TT>
            Log("Step4: Insert a textelement which scopes a UIElement");
            _elementType = ReflectionUtils.FindType(TextElementTypeName);
            object elementObj1 = ReflectionUtils.CreateInstance(_elementType);
            TextTree.InsertElement(TextTree.Start, _tpBackward, (TextElement)elementObj1);

            VerifyGetSymbolType(_tpBackward, Forward, TextPointerContext.ElementEnd);
            VerifyGetOffsetToPosition(_tp1, _tpBackward, 1);
            VerifyGetElementType(_tp1, _elementType);
            VerifyGetOffsetToPosition(_tpBackward, _tpForward, (TestText1.Length + 1));
            VerifyGetEmbeddedObject(_tpBackward, Backward, embObj1);
            #endregion Step4: Insert a textelement which scopes a UIElement

            #region Step5: Delete a UIElement
            //Present Tree State: <TT><elementObj1>[TestText1]<tp1><embObj1><tpBackward></elementObj1>
            //[TestText1]<tpForward><embObj2></TT>
            Log("Step5: Delete a UIElement");
            TextTree.DeleteEmbeddedObject(TextTree.End, Backward);

            VerifyGetSymbolType(_tpForward, Forward, TextPointerContext.None);
            VerifyEqual(_tpForward, TextTree.End);
            VerifyGravity(_tpForward, Forward);
            #endregion Step5: Delete a UIElement

            #region Step6: Delete contents which includes a UIElement
            //Present Tree State: <TT><elementObj1>[TestText1]<tp1><embObj1><tpBackward></elementObj1>
            //[TestText1]<tpForward></TT>
            Log("Step6: Delete contents which includes a UIElement");
            TextTree.DeleteContent(_tp1, _tpBackward);

            VerifyEqual(_tp1, _tpBackward);
            VerifyGetSymbolType(_tp1, Forward, TextPointerContext.ElementEnd);
            #endregion Step6: Delete contents which includes a UIElement

            #region Step7: Re-insert an embedded object delete all contents
            //Present Tree State: <TT><elementObj1>[TestText1]<tp1=tpBackward></elementObj1>[TestText1]<tpForward></TT>
            Log("Step7: Re-insert an embedded object delete all contents");
            TextTree.InsertEmbeddedObject(_tp1, embObj1);

            VerifyGetEmbeddedObject(_tp1, Forward, embObj1);
            VerifyGetOffsetToPosition(_tp1, _tpBackward, 0);

            TextTree.DeleteContent(TextTree.Start, _tpForward);
            VerifyEqual(TextTree.Start, _tp1);
            VerifyEqual(_tp1, _tpBackward);
            VerifyEqual(_tpBackward, _tpForward);
            VerifyGravity(_tpBackward, Backward);
            #endregion Step7: Re-insert an embedded object delete all contents

            Logger.Current.ReportSuccess();
        }
    }
}
