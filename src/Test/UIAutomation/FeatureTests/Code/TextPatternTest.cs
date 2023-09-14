// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Windows.Automation.Text;

namespace UIAutomation
{
    interface IUIAutomationTest
    {
        bool Perform(AutomationElement el, XmlElement xe);
    }

    /// <summary>
    /// Calling TextPattern.DocumentRange.GetText()
    /// Xml Tag needed:
    /// <TEXT>Expected text</TEXT>
    /// </summary>
    public class GetTextTest : IUIAutomationTest
    {
        public bool Perform(AutomationElement rootDocumentElement, XmlElement xe)
        {
            GlobalLog.LogEvidence("GetTextTest - Perform");

            try
            {
                TextPattern textPattern = (TextPattern)rootDocumentElement.GetCurrentPattern(TextPattern.Pattern);
                string actualText = textPattern.DocumentRange.GetText(-1).Trim();
                string expectingText = xe["TEXT"].InnerText.Trim();

                if (actualText == expectingText)
                {
                    GlobalLog.LogEvidence("Text matched as expected");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Text mismatched");
                    GlobalLog.LogEvidence("Expecting: [" + expectingText + "]");
                    GlobalLog.LogEvidence("Actual   : [" + actualText + "]");
                    return false;
                }
            }
            catch (Exception exp)
            {
                GlobalLog.LogEvidence("Exception occurs");
                GlobalLog.LogEvidence(exp.ToString());
                return false;
            }
        }
    }

    /// <summary>
    /// Calling TextPattern.DocumentRange.GetChildren()
    /// Xml Tag needed:
    /// <CHILD>
    /// <NAME>The name of the child AutomationElement, if there's any</NAME>
    /// <CONTROLTYPE>ControlType of the current child</CONTROLTYPE>
    /// <HELPTEXT>Help text specified on the tag, if there's any</HELPTEXT>
    /// </CHILD>
    /// </summary>
    class GetChildrenTest : IUIAutomationTest
    {
        /// <summary>
        /// Testing get document's children
        /// </summary>
        /// <param name="rootDocumentElement"></param>
        /// <param name="xe"></param>
        /// <returns></returns>
        public bool Perform(AutomationElement rootDocumentElement, XmlElement xe)
        {
            GlobalLog.LogEvidence("GetChildrenTest - Perform");

            List<MyAutomationElement> actualChildren = new List<MyAutomationElement>();
            List<MyExpectedElement> expectedChildren = new List<MyExpectedElement>();

            bool bPassed = true;
            string parent = string.IsNullOrEmpty(rootDocumentElement.Current.Name)
                ? rootDocumentElement.Current.AutomationId : rootDocumentElement.Current.Name;

            try
            {
                // get children of the document
                TextPattern textPattern = (TextPattern)rootDocumentElement.GetCurrentPattern(TextPattern.Pattern);
                AutomationElement[] children = textPattern.DocumentRange.GetChildren();
                
                // Collecting AutomationElement
                foreach (AutomationElement child in children)
                {
                    MyAutomationElement childEl = new MyAutomationElement(child);
                    CollectChildren(childEl);
                    actualChildren.Add(childEl);
                }

                // For each expected names, we make sure it was found on Document's children
                // and the order also correct
                
                foreach (XmlNode el in xe.ChildNodes)
                {
                    if (el.Name == "CHILD")
                    {
                        MyExpectedElement child = new MyExpectedElement();
                        child.Name = el["NAME"].InnerText;
                        child.ControlType = el["CONTROLTYPE"].InnerText;
                        child.HelpText = el["HELPTEXT"].InnerText;

                        if (el["CHILDREN"] != null)
                        {
                            CollectExpectedChild(el["CHILDREN"], child);
                        }
                        expectedChildren.Add(child);
                    }
                }

                if (expectedChildren.Count != actualChildren.Count)
                {
                    GlobalLog.LogEvidence("FAIL: Child count mismatched");
                    GlobalLog.LogEvidence("Expecting: " + expectedChildren.Count);
                    GlobalLog.LogEvidence("Actual   : " + actualChildren.Count);
                    return false;
                }
                else
                {
                    for (int i = 0; i < actualChildren.Count; i++)
                    {
                        bPassed &= VerifyChild(actualChildren[i], expectedChildren[i]);
                    }
                }
            }
            catch (Exception exp)
            {
                GlobalLog.LogEvidence("Exception occurs");
                GlobalLog.LogEvidence(exp.ToString());
                bPassed = false;
            }

            return bPassed;
        }

        private void CollectChildren(MyAutomationElement currentEl)
        {
            foreach (AutomationElement elChild in currentEl.ThisElement.FindAll(TreeScope.Children, Condition.TrueCondition))
            {
                MyAutomationElement child = new MyAutomationElement(elChild);
                CollectChildren(child);
                currentEl.Children.Add(child);
            }
        }

        private void CollectExpectedChild(XmlElement xe, MyExpectedElement currentEl)
        {
            foreach (XmlNode el in xe.ChildNodes)
            {
                if (el.Name == "CHILD")
                {
                    MyExpectedElement child = new MyExpectedElement();
                    child.Name = el["NAME"].InnerText;
                    child.ControlType = el["CONTROLTYPE"].InnerText;
                    child.HelpText = el["HELPTEXT"].InnerText;

                    if (el["CHILDREN"] != null)
                    {
                        CollectExpectedChild(el["CHILDREN"], child);
                    }

                    currentEl.Children.Add(child);
                }
            }
        }

        private bool VerifyChild(MyAutomationElement currentChild, MyExpectedElement expectedChild)
        {
            bool bPassed = true;
            string actualName = currentChild.ThisElement.Current.Name;
            string actualCtrlType = currentChild.ThisElement.Current.LocalizedControlType;
            string actualHlpTxt = currentChild.ThisElement.Current.HelpText;

            string expectedName = expectedChild.Name;
            string expectedCtrlType = expectedChild.ControlType;
            string expectedHlpText = expectedChild.HelpText;
            
            GlobalLog.LogEvidence("Child found, type: " + actualCtrlType + 
                ", name:\"" + actualName + "\"" + ", HelpText: \"" + actualHlpTxt + "\"");
             
            if (string.Compare(actualCtrlType, expectedCtrlType, StringComparison.OrdinalIgnoreCase) != 0)
            {
                GlobalLog.LogEvidence("FAIL: Child ControlType mismatched");
                GlobalLog.LogEvidence("Expecting: [" + expectedCtrlType + "]");
                GlobalLog.LogEvidence("Actual   : [" + actualCtrlType + "]");
                bPassed = false;
            }

            if (actualName != expectedName)
            {
                GlobalLog.LogEvidence("FAIL: Child name mismatched");
                GlobalLog.LogEvidence("Expecting: [" + expectedName + "]");
                GlobalLog.LogEvidence("Actual   : [" + actualName + "]");
                bPassed = false;
            }

            if (string.Compare(actualHlpTxt, expectedHlpText, StringComparison.OrdinalIgnoreCase) != 0)
            {
                GlobalLog.LogEvidence("FAIL: Child HelpText mismatched");
                GlobalLog.LogEvidence("Expecting: [" + expectedHlpText + "]");
                GlobalLog.LogEvidence("Actual   : [" + actualHlpTxt + "]");
                bPassed = false;
            }

            if (currentChild.Children != null && expectedChild.Children != null)
            {
                if (currentChild.Children.Count != expectedChild.Children.Count)
                {
                    GlobalLog.LogEvidence("Children count mismatched");
                    GlobalLog.LogEvidence("Expecting: " + expectedChild.Children.Count);
                    GlobalLog.LogEvidence("Actual   : " + currentChild.Children.Count);
                    return false;
                }
                else
                {
                    for (int i = 0; i < currentChild.Children.Count; i++)
                    {
                        bPassed &= VerifyChild(currentChild.Children[0], expectedChild.Children[0]);
                    }
                }
            }

            if (bPassed)
            {
                GlobalLog.LogEvidence("Child matched as expected");
            }

            return bPassed;
        }

//        public static int Timeout = 60000;  // Timeout when performing search on AutomationElement
//        public static int Interval = 15;    // Amount of time to sleep between search iteration

        private class MyAutomationElement
        {
            public MyAutomationElement()
            {
                ThisElement = null;
                _children = new List<MyAutomationElement>();
            }

            public MyAutomationElement(AutomationElement ae)
            {
                ThisElement = ae;
                _children = new List<MyAutomationElement>();
            }

            public AutomationElement ThisElement
            {
                get
                {
                    return _ae;
                }
                set
                {
                    _ae = value;
                }
            }

            public List<MyAutomationElement> Children
            {
                get
                {
                    return _children;
                }
            }
            private AutomationElement _ae;
            private List<MyAutomationElement> _children;
        }

        private class MyExpectedElement
        {
            public string Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }

            public string ControlType
            {
                get
                {
                    return _controlType;
                }
                set
                {
                    _controlType = value;
                }
            }

            public string HelpText
            {
                get
                {
                    return _helpText;
                }
                set
                {
                    _helpText = value;
                }
            }

            public List<MyExpectedElement> Children
            {
                get
                {
                    return _children;
                }
            }

            private string _name;
            private string _controlType;
            private string _helpText;
            List<MyExpectedElement> _children = new List<MyExpectedElement>();
        }
    }

    /// <summary>
    /// Calling TextPattern.DocumentRange.Move()
    /// Xml Tag needed:
    /// <TEXTRANGESTARTPOINT>5</TEXTRANGESTARTPOINT>
    /// <TEXTRANGEENDPOINT>10</TEXTRANGEENDPOINT>
    /// <MOVE>
    ///     <TEXTUNIT>TextUnit such as Character, Document, Format, Line, Page, Paragraph, Word</TEXTUNIT>
    ///     <COUNT>How many unit to move</COUNT>
    ///     <TEXT>Text expected after the move</TEXT>
    /// </MOVE>
    /// </summary>
    class MoveByUnitTest : IUIAutomationTest
    {
        /// <summary>
        /// Testing Move on element containing text pattern
        /// </summary>
        /// <param name="rootDocumentElement"></param>
        /// <param name="xe"></param>
        /// <returns></returns>
        public bool Perform(AutomationElement rootDocumentElement, XmlElement xe)
        {
            GlobalLog.LogEvidence("MoveByUnitTest - Perform");

            bool bPassed = true;

            try
            {
                TextPattern textPattern = (TextPattern)rootDocumentElement.GetCurrentPattern(TextPattern.Pattern);
                TextPatternRange documentRange = textPattern.DocumentRange.Clone();
                documentRange.MoveEndpointByRange(TextPatternRangeEndpoint.End, documentRange, TextPatternRangeEndpoint.Start);

                UIAutomationHelper.PositionEndpoints(documentRange, xe);

                foreach (XmlNode node in xe.ChildNodes)
                {
                    if (node.Name == "MOVE")
                    {
                        TextUnit textUnit =
                            (TextUnit)Enum.Parse(typeof(TextUnit),
                            node["TEXTUNIT"].InnerText.Trim(),
                            true);
                        int count = int.Parse(node["COUNT"].InnerText);
                        string expectedText = node["TEXT"].InnerText;
                        int successCount = int.Parse(node["SUCCESSCOUNT"].InnerText);
                        GlobalLog.LogEvidence("Move " + count + " " + textUnit.ToString() + ", Expecting: " + expectedText);

                        int moved = documentRange.Move(textUnit, count);
                        bPassed &= UIAutomationHelper.VerifyText(documentRange, expectedText);
                        if (moved != successCount)
                        {
                            bPassed &= false;
                            GlobalLog.LogEvidence("Move count mismatched");
                            GlobalLog.LogEvidence("Expecting: " + successCount);
                            GlobalLog.LogEvidence("Actual   : " + moved);
                        }
                        else
                        {
                            bPassed &= true;
                            GlobalLog.LogEvidence("Move count matched");
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                GlobalLog.LogEvidence("Exception occurs");
                GlobalLog.LogEvidence(exp.ToString());
                bPassed = false;
            }

            return bPassed;
        }
    }

    /// <summary>
    /// Calling TextPattern.DocumentRange.MoveEndpointByUnit()
    /// Xml Tag needed:
    /// <TEXTRANGESTARTPOINT>5</TEXTRANGESTARTPOINT>
    /// <TEXTRANGEENDPOINT>10</TEXTRANGEENDPOINT>
    /// <MOVEENDPOINTBYUNIT>
    ///     <TEXTPATTERNRANGEENDPOINT>TextPatternRangeEndpoint, End or Start</TEXTPATTERNRANGEENDPOINT>
    ///     <TEXTUNIT>TextUnit such as Character, Document, Format, Line, Page, Paragraph, Word</TEXTUNIT>
    ///     <COUNT>How many unit to move</COUNT>
    ///     <TEXT>Text expected after the move</TEXT>
    /// </MOVEENDPOINTBYUNIT>
    /// </summary>
    class MoveEndpointByUnitTest : IUIAutomationTest
    {
        /// <summary>
        /// Testing MoveEndpointByUnit on element containing text pattern
        /// </summary>
        /// <param name="rootDocumentElement"></param>
        /// <param name="xe"></param>
        /// <returns></returns>
        public bool Perform(AutomationElement rootDocumentElement, XmlElement xe)
        {
            GlobalLog.LogEvidence("MoveEndpointByUnitTest - Perform");

            bool bPassed = true;

            try
            {
                TextPattern textPattern = (TextPattern)rootDocumentElement.GetCurrentPattern(TextPattern.Pattern);
                TextPatternRange documentRange = textPattern.DocumentRange.Clone();
                documentRange.MoveEndpointByRange(TextPatternRangeEndpoint.End, documentRange, TextPatternRangeEndpoint.Start);

                UIAutomationHelper.PositionEndpoints(documentRange, xe);

                foreach (XmlNode node in xe.ChildNodes)
                {
                    if (node.Name == "MOVEENDPOINTBYUNIT")
                    {
                        TextPatternRangeEndpoint endPoint =
                            (TextPatternRangeEndpoint)Enum.Parse(typeof(TextPatternRangeEndpoint),
                            node["TEXTPATTERNRANGEENDPOINT"].InnerText.Trim(),
                            true);
                        TextUnit textUnit =
                            (TextUnit)Enum.Parse(typeof(TextUnit),
                            node["TEXTUNIT"].InnerText.Trim(),
                            true);
                        int count = int.Parse(node["COUNT"].InnerText);
                        string expectedText = node["TEXT"].InnerText;
                        int successCount = int.Parse(node["SUCCESSCOUNT"].InnerText);

                        GlobalLog.LogEvidence("MoveEndpointByUnit " + count + " " + textUnit.ToString() +
                            ",Starting from: " + endPoint + ", Expecting: " + expectedText);

                        int moved = documentRange.MoveEndpointByUnit(endPoint, textUnit, count);
                        bPassed &= UIAutomationHelper.VerifyText(documentRange, expectedText);

                        if (moved != successCount)
                        {
                            bPassed &= false;
                            GlobalLog.LogEvidence("Success Move count mismatched");
                            GlobalLog.LogEvidence("Expecting: " + successCount);
                            GlobalLog.LogEvidence("Actual   : " + moved);
                        }
                        else
                        {
                            bPassed &= true;
                            GlobalLog.LogEvidence("Move count matched");
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                GlobalLog.LogEvidence("Exception occurs");
                GlobalLog.LogEvidence(exp.ToString());
                bPassed = false;
            }

            return bPassed;
        }
    }

    /// <summary>
    /// Calling TextPatternRange.ExpandToEnclosingUnit()
    /// Xml Tag sample:
    /// <VARIATION ID="1">
    ///   <XAMLFILE>PlainText.xaml</XAMLFILE>
    ///   <AUTOMATIONID>PUIDocumentApplicationDocumentViewer</AUTOMATIONID>
    ///   <UIELEMENTID>FlowDoc</UIELEMENTID>
    ///   <TEXTRANGESTARTPOINT>5</TEXTRANGESTARTPOINT>
    ///   <TEXTRANGEENDPOINT>10</TEXTRANGEENDPOINT>
    ///   <EXPANDTOENCLOSINGUNIT>
    ///     <TEXTUNIT>Word</TEXTUNIT>
    ///     <TEXT>term </TEXT>
    ///   </EXPANDTOENCLOSINGUNIT>
    ///   </VARIATION>
    /// </summary>
    public class ExpandToEnclosingUnitTest : IUIAutomationTest
    {
        /// <summary>
        /// Testing ExpandToEnclosingUnit on an AutomationElement that supports TextPattern.
        /// </summary>
        /// <param name="rootDocumentElement">The AutomationElement that implements TextPattern.</param>
        /// <param name="xe">XmlElement containing variation information.</param>
        /// <returns>Testcase pass or failure.</returns>
        public bool Perform(AutomationElement rootDocumentElement, XmlElement xe)
        {
            GlobalLog.LogEvidence("ExpandToEnclosingUnitTest - Perform");

            TextPattern textPattern = (TextPattern)rootDocumentElement.GetCurrentPattern(TextPattern.Pattern);
            TextPatternRange tpr = textPattern.DocumentRange.Clone();
            tpr.MoveEndpointByRange(TextPatternRangeEndpoint.End, tpr, TextPatternRangeEndpoint.Start);

            UIAutomationHelper.PositionEndpoints(tpr, xe);

            TextUnit textUnit = (TextUnit)Enum.Parse(typeof(TextUnit), xe["EXPANDTOENCLOSINGUNIT"]["TEXTUNIT"].InnerText.Trim(), true);
            string expectedText = xe["EXPANDTOENCLOSINGUNIT"]["TEXT"].InnerText;

            tpr.ExpandToEnclosingUnit(textUnit);

            return UIAutomationHelper.VerifyText(tpr, expectedText);
        }
    }

    public static class UIAutomationHelper
    {
        /// <summary>
        /// Position the starting and ending endpoints of the text range.
        /// 'TEXTRANGESTARTPOINT' and 'TEXTRANGEENDPOINT' describe where
        /// the start and end endpoints should be, measured in characters, from
        /// the current start endpoint. If an element is missing, it is assumed
        /// to be 0.
        /// </summary>
        /// <param name="tpr">Range whose endpoints are to be repositioned.</param>
        /// <param name="xe">XmlElement describing how much they are to be moved.</param>
        public static void PositionEndpoints(TextPatternRange tpr, XmlElement xe)
        {
            int start = 0;
            if (xe["TEXTRANGESTARTPOINT"] != null)
            {
                start = int.Parse(xe["TEXTRANGESTARTPOINT"].InnerText);
            }
            int end = 0;
            if (xe["TEXTRANGEENDPOINT"] != null)
            {
                end = int.Parse(xe["TEXTRANGEENDPOINT"].InnerText);
            }
            tpr.MoveEndpointByRange(TextPatternRangeEndpoint.End, tpr, TextPatternRangeEndpoint.Start);
            tpr.MoveEndpointByUnit(TextPatternRangeEndpoint.End, TextUnit.Character, end);
            tpr.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, TextUnit.Character, start);
        }

        public static bool VerifyText(TextPatternRange textPatternRange, string expectedText)
        {
            string actualText = textPatternRange.GetText(-1);

            if (string.Compare(expectedText, actualText) == 0)
            {
                GlobalLog.LogEvidence("Text matched as expected.");

                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Text mismatched.");
                GlobalLog.LogEvidence("Expecting: [" + expectedText + "]");
                GlobalLog.LogEvidence("Actual   : [" + actualText + "]");

                return false;
            }
        }
    }
}
