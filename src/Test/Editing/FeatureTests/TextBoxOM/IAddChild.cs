// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Tests the IAddChild implementation of the TextBox class.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/IAddChild.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Drawing;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;    
    using Test.Uis.IO;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>Verifies the IAddChild implementation of TextBox.</summary>
    [Test(0, "TextBox", "TextBoxAddChild1", MethodParameters = "/TestCaseType=TextBoxAddChild")]
    [Test(2, "TextBox", "TextBoxAddChild2", MethodParameters = "/TestCaseType=TextBoxAddChild /RunExhaustive:true")]
    [TestOwner("Microsoft"), TestTactics("589,588"),
     TestBugs("627,628,629,630,631, 571"),
     TestWorkItem("91"),
     TestArgument("RunExhaustive", "Whether to run exhaustive testing (optional)")]
    public class TextBoxAddChild : TextBoxTestCase
    {
        #region Settings.

        /// <summary>Whether exhaustive testing should be performed.</summary>
        public bool RunExhaustive
        {
            get { return ConfigurationSettings.Current.GetArgumentAsBool("RunExhaustive"); }
        }

        #endregion Settings.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            ArrayList content;
            Hashtable combination;
            object[] usageValues;

            // Set up content values, with special "flag" values.
            content = new ArrayList();
            content.AddRange(StringData.Values);
            content.Add(EmbeddedObjectContent);
            content.Add(InvalidTypedContent);
            content.Add(TextElementContent);

            usageValues = new object[] {
                IAddChildUsage.DynamicXamlParsing,
                IAddChildUsage.MSBuild,
                IAddChildUsage.ProgrammaticInvocation
            };

            _validPageXml = "";

            // Set up a combinatorial engine with the relevant dimensions.
            _combinatorialEngine = CombinatorialEngine.FromDimensions(new Dimension[] {                
                new Dimension("TextEditableType", TextEditableType.Values),
                new Dimension("Content", content.ToArray()),
                new Dimension("IAddChildUsage", usageValues),
                });

            // Loop through all interesting combinations.
            combination = new Hashtable();
            while (_combinatorialEngine.Next(combination))
            {
                StringData stringData;

                stringData = combination["Content"] as StringData;
                if (stringData == null)
                {
                    _content = (string)combination["Content"];
                }
                else
                {
                    if (stringData.IsLong)
                    {
                        continue;
                    }
                    _content = stringData.Value;
                }
                _textEditableType = (TextEditableType)combination["TextEditableType"];
                _addChildUsage = (IAddChildUsage)combination["IAddChildUsage"];

                // _content == null is tested in the invalid calls case.
                if (_content == null)
                {
                    continue;
                }

                Log("Content: [" + TextUtils.ConvertToSingleLineAnsi(_content) + "]");
                Log("Text editable type: " + _textEditableType.Type);
                Log("IAddChild usage: " + _addChildUsage);

                VerifyInvalidCalls();
                VerifyValidCalls();
            }

            // Add xaml which has all properties set for each TextEditableType to _validPageXml
            CreateXamlForProperties();
            VerifyBatchedMSBuild();
            Logger.Current.ReportSuccess();
        }

        /// <summary>Add all dependency property</summary>
        private string CreateXamlForProperties()
        {
            _combinatorialEngine = CombinatorialEngine.FromDimensions(new Dimension[] {
                new Dimension("TextEditableType", TextEditableType.Values),
                });
            Hashtable combination = new Hashtable();
            while (_combinatorialEngine.Next(combination))
            {
                _textEditableType = (TextEditableType)combination["TextEditableType"];

                _dpDataArray = DependencyPropertyData.GetDPDataForControl((_textEditableType.Type));
                foreach (DependencyPropertyData dpData in _dpDataArray)
                {       
                    if (dpData.Property != TextBlock.TextAlignmentProperty)
                    {
                        string attributes;
                        attributes = dpData.Property.Name + "='";
                        attributes += dpData.TestValue.ToString() + "'";
                        if (_textEditableType.IsPassword)
                        {
                            _validPageXml += _textEditableType.GetEditableXaml(attributes, "");
                        }
                        else
                        {
                            if (dpData.Property == TextBox.TextProperty)
                            {
                                _validPageXml += _textEditableType.GetEditableXaml(attributes, "");
                            }
                            else
                            {
                                _validPageXml += _textEditableType.GetEditableXaml(attributes, "Hello World!");
                            }
                        }
                    }
                }
            }
            return _validPageXml;
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalidCalls()
        {            
            if (_addChildUsage == IAddChildUsage.ProgrammaticInvocation)
            {
                Log("Verifying invalid calls to IAddChild implementation...");
                IAddChild add = _textEditableType.CreateInstance() as IAddChild;
                if (add == null)
                {
                    return;
                }
                try
                {
                    add.AddChild(null);
                    AcceptedException("null content");
                }
                catch (ArgumentException)
                {
                    LogRejected("null content");
                }
                try
                {
                    add.AddText(null);
                    AcceptedException("null text");
                }
                catch (ArgumentException)
                {
                    LogRejected("null text");
                }

                try
                {
                    add.AddChild(MainWindow);
                    AcceptedException("main window");
                }
                catch (InvalidOperationException)
                {
                    LogRejected("main window");
                }
                catch (ArgumentException)
                {
                    LogRejected("main window");
                }

                try
                {
                    add.AddChild(add);
                    AcceptedException("self");
                }
                catch (InvalidOperationException)
                {
                    LogRejected("self");
                }
                catch (ArgumentException)
                {
                    LogRejected("self");
                }

            }
            else if (_addChildUsage == IAddChildUsage.DynamicXamlParsing)
            {
                // There are no invalid dynamic XAML parsing scenarios
                // that are not type-specific.
            }
            else if (_addChildUsage == IAddChildUsage.MSBuild)
            {
                // There are no invalid dynamic XAML parsing scenarios
                // that are not type-specific.
            }
            else
            {
                throw new Exception("Test case cannot handle add child usage " +
                    _addChildUsage);
            }
        }

        /// <summary>Creates an object to be added, based on _content.</summary>
        private object CreateObjectToAdd()
        {
            switch (_content)
            {
                case EmbeddedObjectContent:
                    return new Button();
                case TextElementContent:
                    return new Paragraph();
                case InvalidTypedContent:
                    return 8;
                default:
                    return null;
            }
        }

        /// <summary>Whether the current combination is for object addition.</summary>
        private bool IsObjectAddition
        {
            get
            {
                return _content == EmbeddedObjectContent ||
                    _content == TextElementContent ||
                    _content == InvalidTypedContent ||
                    _content == FlowDocumentContent;
            }
        }

        /// <summary>
        /// Runs MSBuild to create an application with the given page content,
        /// and verifies that it builds or fails as specified.
        /// </summary>
        /// <param name="pageContent">Content in XAML page to build.</param>
        /// <param name="shouldSucceed">Whether the build should succeed.</param>
        private void VerifyMSBuildForPageContent(string pageContent, bool shouldSucceed)
        {
            //const string BaseFileName = "WTC.Uis.AddChild";
            //bool buildSucceeded = false ;

            //buildSucceeded = Test.Uis.Utils.BuildUtils.RunMSBuildForPageContent(pageContent, BaseFileName);
            //Verifier.Verify(buildSucceeded == shouldSucceed,
            //    "Built success [" + buildSucceeded + "] as expected [" +
            //    shouldSucceed + "]", true);
        }

        private string RemoveSpace(string inputString)
        {
            string outputString;

            outputString = inputString;
            outputString = outputString.Replace(" ", string.Empty);
            outputString = outputString.Replace("\t", string.Empty);
            outputString = outputString.Replace("\r", string.Empty);
            outputString = outputString.Replace("\n", string.Empty);

            return outputString;
        }

        /// <summary>Whether the current addition operation should succeed.</summary>
        private bool ShouldAdditionSucceed()
        {
            //Password box dont support adding any children. Remove the cases which just have
            //space or tabs as content.
            if(_textEditableType.IsPassword)
            {                
                string tempString = RemoveSpace(_content);                

                if ( (_content == string.Empty) || (tempString==string.Empty) )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //Content Validation during compile time is scheduled for V2.
            //Look at Regression_Bug571
            if (_addChildUsage == IAddChildUsage.MSBuild)
            {                
                if (_content == InvalidTypedContent)
                {
                    return false;
                }
                else
                {
                    return true;
                }             
            }

            switch (_content)
            {
                case EmbeddedObjectContent:                    
                case TextElementContent:
                    //return _textEditableType.SupportsParagraphs;
                case InvalidTypedContent:
                    return false;
                case FlowDocumentContent:
                    return true;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Verifies that the batch MSBuild cases build correctly.
        /// </summary>
        private void VerifyBatchedMSBuild()
        {
            string pageXml;   // The final XAML on the page to be built.

            if (_validPageXml.Length == 0)
            {
                Log("There are no batched MSBuild cases to run.");
                return;
            }

            pageXml =
                "<DockPanel xml:space='preserve' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                TextEditableType.SubClassNamespaceAttributeName + "='" +
                TextEditableType.SubClassNamespaceAttributeValue + "'>" +
                _validPageXml + "</DockPanel>";
            VerifyMSBuildForPageContent(pageXml, true);
        }

        /// <summary>Whether dynamic XAML scenario.</summary>
        private void VerifyDynamicXamlParsing()
        {
            const string attributes = "xml:space='preserve' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ";
            string xaml;                // XAML to be dynamically parsed.
            object parsedObject;        // Parsed object.
            UIElementWrapper wrapper;   // Wrapper for parsed object.

            xaml = _textEditableType.GetEditableXaml(attributes, _content);

            // 
            if (_textEditableType.IsSubClass)
            {
                return;
            }

            if (IsObjectAddition)
            {
                if (ShouldAdditionSucceed())
                {
                    Log("Expecting addition to succeed for this type.");
                    parsedObject = XamlUtils.ParseToObject(xaml);

                    Log("Parsed object created...");
                    wrapper = new UIElementWrapper((UIElement)parsedObject);
                    VerifyObjectAdded(wrapper, _objectToAdd);
                }
                else
                {
                    Log("Expecting addition to fail for this type.");
                    try
                    {
                        parsedObject = XamlUtils.ParseToObject(xaml);
                    }
                    catch (SystemException)
                    {
                        Log("Exception thrown as expected.");
                    }
                }
            }
            else if (_textEditableType.IsPassword)
            {
                // This should throw an exception during parsing, unless
                // it's ignorable whitespace.
                if (_content != null && _content.Trim().Length > 0)
                {
                    try
                    {
                        XamlUtils.ParseToObject(xaml);
                        throw new Exception("Parsing " + xaml + " should fail, but succeeded.");
                    }
                    catch(XamlParseException)
                    {
                        Log("Parsing " + xaml + " failed as expected for PasswordBox.");
                    }
                }
                else
                {
                    PasswordBox passwordBox = (PasswordBox)XamlUtils.ParseToObject(xaml);
                    Verifier.Verify(passwordBox.Password == "",
                        "PasswordBox is empty.", true);
                }
            }
            else
            {
                // This should "just work" with regular content.
                parsedObject = XamlUtils.ParseToObject(xaml);
                wrapper = new UIElementWrapper((UIElement)parsedObject);

                _content = TextUtils.NormalizeEndOfLines(_content);
                Verifier.Verify((wrapper.Text == _content)||(wrapper.Text == _content + "\r\n"),
                    "Actual text [" +
                    TextUtils.ConvertToSingleLineAnsi(wrapper.Text) +
                    "] matches added text [" + _content + "]", true);
            }
        }

        /// <summary>Verifies that IAddChild works in MSBuild scenario.</summary>
        /// <remarks>
        /// MSBuild testing is very resource-intensive, so the test minimizes
        /// the number of times it is done.
        ///
        /// If the operation should succeed, it is batched into _validPageXml.
        /// If the operation should fail, it is run only if RunExhaustive is true.
        /// </remarks>
        private void VerifyMSBuild()
        {
            if (ShouldAdditionSucceed())
            {
                _validPageXml += _textEditableType.GetEditableXaml("", _content);
            }
            else
            {
                string pageXml; // The final XAML on the page to be built.

                if (RunExhaustive)
                {
                    pageXml =
                        "<DockPanel xml:space='preserve' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                        _textEditableType.GetEditableXaml("", _content) +
                        "</DockPanel>";
                    VerifyMSBuildForPageContent(pageXml, false);
                }
                else
                {
                    Log("Failing MSBuild cases are not run if RunExhaustive is not set to True.");
                    return;
                }
            }
        }

        /// <summary>
        /// Verifies that the specified object was added to the given
        /// wrapped control.
        /// </summary>
        private void VerifyObjectAdded(UIElementWrapper wrapper,
            object objectAdded)
        {
            // Very simple verification - if a tag with the right
            // type is there, pass. Object consistency not checked (but
            // implementable).
            Verifier.Verify(
                wrapper.XamlText.IndexOf(objectAdded.GetType().Name) != -1,
                "Content added: [" + wrapper.XamlText + "]", true);
        }

        private void VerifyProgrammaticInvocation()
        {
            IAddChild add;              // IAddChild for control.
            UIElementWrapper wrapper;   // Control wrapper.

            add = _textEditableType.CreateInstance() as IAddChild;
            if (add == null)
            {
                return;
            }
            wrapper = new UIElementWrapper((UIElement)add);

            // Verify content, unless it's password (it gets mangled).
            //Also skip for RichTextBox due to content validation.
            if( (!_textEditableType.IsPassword)&&(!_textEditableType.SupportsParagraphs) )
            {
                Log("Adding text...");
                add.AddText(_content);

                Verifier.Verify(wrapper.Text == _content,
                    "Actual text [" + wrapper.Text + "] matches added text ["
                    + _content + "]", true);
            }

            ////Check if exception is thrown when trying to add TextElement/InvalidType besides text run.
            //if ((_content != null) && (_content != string.Empty) &&
            //    (IsObjectAddition) && (_content != EmbeddedObjectContent) )
            //{
            //    add.AddText(_content);
            //    try
            //    {
            //        add.AddChild(_objectToAdd);
            //        throw new ApplicationException("Exception expected - for trying to add TextElement/InvalidType besides text run. None thrown");
            //    }
            //    catch (SystemException)
            //    {
            //        Log("Exception thrown as expected when trying to add TextElement/InvalidType besides text run");
            //    }

            //    wrapper.Clear(); //clear the contents before doing TextElement addition
            //}

            if (IsObjectAddition)
            {
                Log("Adding object " + _objectToAdd + "...");
                if (ShouldAdditionSucceed())
                {
                    Log("Expecting addition to succeed for this type.");
                    add.AddChild(_objectToAdd);
                    VerifyObjectAdded(wrapper, _objectToAdd);
                }
                else
                {
                    Log("Expecting addition to fail for this type.");
                    try
                    {
                        add.AddChild(_objectToAdd);
                    }
                    catch (SystemException)
                    {
                        Log("Exception thrown as expected.");
                    }
                }
            }
        }

        private void VerifyValidCalls()
        {
            this._objectToAdd = CreateObjectToAdd();

            Log("Verifying valid calls to IAddChild implementation...");
            switch (_addChildUsage)
            {
                case IAddChildUsage.ProgrammaticInvocation:
                    VerifyProgrammaticInvocation();
                    break;
                case IAddChildUsage.DynamicXamlParsing:
                    VerifyDynamicXamlParsing();
                    break;
                case IAddChildUsage.MSBuild:
                    VerifyMSBuild();
                    break;
                default:
                    throw new Exception("Test case cannot handle add " +
                        "child usage " + _addChildUsage);
            }
        }

        #endregion Verifications.

        #region Private fields.

        private CombinatorialEngine _combinatorialEngine;

        private string _content;
        private TextEditableType _textEditableType;
        private IAddChildUsage _addChildUsage;
        private object _objectToAdd;    // Object to be added, if necessary.
        private string _validPageXml;   // Batched XAML page to be built.
        private DependencyPropertyData[] _dpDataArray;

        private const string EmbeddedObjectContent = "<Button>Embedded object.</Button>";
        private const string InvalidTypedContent = "<System.Int32>8</System.Int32>";
        private const string TextElementContent = "<Paragraph>paragraph</Paragraph>";
        private const string FlowDocumentContent = "<FlowDocument><Paragraph>FlowDocument</Paragraph></FlowDocument>";

        #endregion Private fields.

        #region Inner types.

        /// <summary>
        /// Ways in which the IAddChild interface may be called.
        /// </summary>
        enum IAddChildUsage
        {
            DynamicXamlParsing,
            MSBuild,
            ProgrammaticInvocation
        }

        #endregion Inner types.
    }
}
