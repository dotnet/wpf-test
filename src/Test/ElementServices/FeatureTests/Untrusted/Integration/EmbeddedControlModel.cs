// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model EmbeddedControl.
 *          Construct trees, serialize them and verify.
 *
 
  
 * Revision:         $Revision: 2 $
 
 *********************************************************************/
using System;
using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using System.Windows.Controls;
using Microsoft.Test.Markup;
using System.Xml;
using Avalon.Test.CoreUI.Serialization;
using System.Diagnostics;
using Avalon.Test.CoreUI.IdTest;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Parser.EmbededControl
{
    /// <summary>
    /// EmbeddedControl code-based test suit with a Model. 
    /// </summary>
    [TestCaseModel("EmbeddedControlCases.xtc", "1", @"Parser\EmbeddedControl\test", TestCaseSecurityLevel.FullTrust, "EmbeddedControl model pairwise")]
    [TestCaseSupportFile("EmbeddedControlBase.xaml")]
    public class EmbeddedControlModel : Model
    {

        /// <summary>
        /// Construct new instance of the model.
        /// </summary>
        public EmbeddedControlModel()
            : base()
        {
            Name = "untitled";
            Description = "EmbeddedControl Model";

            //Add Action Handlers
            AddAction("RunTest", new ActionHandler(RunTest));
        }

        /// <summary>
        /// Single action for this model.  Creates xaml and run the 
        /// Compilation and Serialization test case. 
        /// </summary>
        /// <remarks>Handler for RunTest</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RunTest(State endState, State inParams, State outParams)
        {
            ModelState = new EmbeddedControlModelState(inParams);
            EmbeddedControlModelState.Persist(ModelState);
            ModelState.LogState();
            //create xaml file
            string tempXamlFileName="___EmbeddedControlComponentXaml.xaml";
            CreateXaml(tempXamlFileName);
            

            //Run Compile Case
            RunCompilationCase(tempXamlFileName);
            
            //Run serialization round trip case
            RunSerializationRoundTrip(tempXamlFileName);
            return true;
        }
        void RunCompilationCase(string tempXamlFileName)
        {
            //CompilerHelper compiler = new CompilerHelper();
            //compiler.CleanUpCompilation();
            //compiler.AddDefaults();
            //compiler.CompileApp(tempXamlFileName, "Application"/*, null,extraFiles*/);
            //File.Copy("avalon.png", "bin\\release\\avalon.png");
            //compiler.RunCompiledApp();
            return;
        }

        void RunSerializationRoundTrip(string tempXamlFileName)
        {
            SerializationHelper helper = new SerializationHelper();
            helper.RoundTripTestFile(tempXamlFileName, XamlWriterMode.Expression, true);
            return;
        }


        /// <summary>
        /// Create the xaml according to the parameter read from xtc.
        /// </summary>
        private void CreateXaml(string tempXamlFileName)
        {
            XmlDocument newXaml = new XmlDocument();

            newXaml.Load("EmbeddedControlBase.xaml");

            XmlElement rootElement = newXaml.DocumentElement;
            XmlElement blockHolder = newXaml.GetElementsByTagName(ModelState.BlockDerivedClassHolder)[0] as XmlElement;

            if(String.Equals(ModelState.BlockDerivedClassBlock, "BlockContainerWithControl", StringComparison.InvariantCulture))
                ConstructBlockContainer(newXaml, blockHolder);
            else if(String.Equals(ModelState.BlockDerivedClassBlock, "ParagraphBlock", StringComparison.InvariantCulture))
                ConstructParagraphBlock(newXaml, blockHolder);
            //Select verification routine.
            string verifier = "CoreTestsUntrusted.dll#Avalon.Test.CoreUI.Parser.EmbededControl.EmbeddedControlModel.EmbeddedControlVerifier" + _numberOfControls;
            rootElement.SetAttribute("Verifier", verifier);
            CoreLogger.LogStatus("Saving xaml to : " + tempXamlFileName);
            newXaml.Save(tempXamlFileName);
            return;
        }
        void ConstructParagraphBlock(XmlDocument doc, XmlElement parent)
        {
            XmlElement paragraph = doc.CreateElement("Paragraph", XamlGenerator.AvalonXmlns);
            parent.InsertAfter(paragraph, null);
            if (String.Equals(ModelState.UnderParagraph, "InlineElement", StringComparison.InvariantCulture))
                ConstructInlineBlock(doc, paragraph);
            else
                ContructMixedBlock(doc, paragraph);
            return;
        }

        void ConstructBlockContainer(XmlDocument doc, XmlElement parent)
        {
            XmlElement container = doc.CreateElement("BlockUIContainer", XamlGenerator.AvalonXmlns);
            parent.InsertAfter(container, null);
            container.InsertAfter(ContructControlBlock(doc), null);
        }

        void ConstructInlineBlock(XmlDocument doc, XmlElement parent)
        {
            if (String.Equals(ModelState.InLineElementBlock, "InlineUIContainerBlock", StringComparison.InvariantCulture))
            {
                XmlElement container = doc.CreateElement("InlineUIContainer", XamlGenerator.AvalonXmlns);
                parent.InsertAfter(container, null);

                container.InsertAfter(ContructControlBlock(doc), null);
            }
            else
                ContructMixedBlock(doc, parent);
            return;
        }

        XmlElement ContructControlBlock(XmlDocument doc)
        {
            XmlElement control = doc.CreateElement(ModelState.Control, XamlGenerator.AvalonXmlns);
            _numberOfControls++;
            if (String.Equals(ModelState.Control, "Image", StringComparison.InvariantCulture))
            {
                control.SetAttribute("Source", "pack://siteoforigin:,,,/avalon.png");
            }
            else if (String.Equals(ModelState.Control, "TextBlock", StringComparison.InvariantCulture))
            {
                ContructMixedBlock(doc, control);
            }

            SetControlProperties(doc, control);

            return control;
        }

        void SetControlProperties(XmlDocument doc, XmlElement control)
        {   
            // set attributes
            control.SetAttribute(ModelState.ResourceLookUp, "{DynamicResource Test" + ModelState.ResourceLookUp + "}");
            control.SetAttribute("Name", "control" + _numberOfControls.ToString());
            control.SetAttribute("FontSize", "20");

            //add resources
            XmlElement controlResources = doc.CreateElement(control.Name + ".Resources", XamlGenerator.AvalonXmlns);
            control.InsertAfter(controlResources, null);
            XmlElement DoubleNode = doc.CreateElement("Double", "clr-namespace:System;assembly=Mscorlib");
            DoubleNode.SetAttribute("Key",  XamlGenerator.AvalonXmlnsX , "TestDouble");
            DoubleNode.InnerText = "36";
            controlResources.InsertAfter(DoubleNode, null);
        }

        void ContructMixedBlock(XmlDocument doc, XmlElement parent)
        {
            char[] sequence = ModelState.Sequence.ToCharArray();
            Debug.Assert(sequence.Length==3);
            for (int i = 0; i < 3; i++)
            {
                AddElement(doc, parent, sequence[i]);
            }
            return;
        }
        void AddElement(XmlDocument doc, XmlElement parent, char type)
        {
            string contentString = "content";
            XmlElement element;
            switch (type)
            {
                case 'B': 
                    if(string.Equals(ModelState.BoldSpace, "Yes"))
                        contentString = " " + contentString + " ";

                    element = doc.CreateElement("Bold", XamlGenerator.AvalonXmlns);
                    element.InnerText = contentString;
                    parent.InsertAfter(element, null);
                    break;
                case 'H': 
                    if(string.Equals(ModelState.HyperlinkSpace, "Yes", StringComparison.InvariantCulture))
                        contentString = " " + contentString + " ";
                    element = doc.CreateElement("Hyperlink", XamlGenerator.AvalonXmlns);
                    element.InnerText = contentString;
                    parent.InsertAfter(element, null);
                    break;
                case 'C': 
                    element = doc.CreateElement("Control", XamlGenerator.AvalonXmlns);
                    XmlElement wrapper;
                    if (string.Equals(ModelState.ControlWrapper, "Yes", StringComparison.InvariantCulture))
                    {
                        wrapper = doc.CreateElement("InlineUIContainer", XamlGenerator.AvalonXmlns);
                        wrapper.InsertAfter(element, null);
                        parent.InsertAfter(wrapper, null);
                    }
                    else
                    {
                        parent.InsertAfter(element, null);
                    }
                    _numberOfControls++;
                    SetControlProperties(doc, element);
                    break;
                default:
                    throw new Microsoft.Test.TestSetupException("Not support: " + type);
            }
        }

        /// <summary>
        /// Verification for EmbeddedControl Model: verify only one control.
        /// </summary>
        /// <param name="root"></param>
        public static void EmbeddedControlVerifier1(UIElement root)
        {
            CoreLogger.LogStatus("EmbeddedControlModel.EmbeddedControlVerifier1()...");
            CoreLogger.LogStatus("Verify that Control1 can be found.");
            Control rootControl = root as Control;
            //Verify resource lookup to child failed.
            Double size = rootControl.FontSize;
            CoreLogger.LogStatus("FontSize: " + size);
            VerifyElement.VerifyBool(size != 36, true);

            Control control = (Control)IdTestBaseCase.FindElementWithId(root, "control1");
            VerifyElement.VerifyBool(null != control, true);
            //Verify The Control
            VerifyTheControl(control);

        }
        /// <summary>
        /// Verification for EmbeddedControl Model: verify two controls.
        /// </summary>
        /// <param name="root"></param>
        public static void EmbeddedControlVerifier2(UIElement root)
        {
            CoreLogger.LogStatus("Inside EmbeddedControlModel.EmbeddedControlVerifier2()...");
            CoreLogger.LogStatus("Verify that Control2 can be found.");
            Control control = (Control)IdTestBaseCase.FindElementWithId(root, "control2");
            
            VerifyElement.VerifyBool(null != control, true);

            //Verify The Control
            VerifyTheControl(control);
            EmbeddedControlVerifier1(root);
        }
        static void VerifyTheControl(Control control)
        {
            //verify inheritable property has property valuE
            VerifyElement.VerifyString(control.Language.IetfLanguageTag, "de-de");
            //Verify Style or Template
            EmbeddedControlModelState status = (EmbeddedControlModelState)EmbeddedControlModelState.Load();
            if(String.Equals(status.ResourceLookUp, "Style", StringComparison.InvariantCulture))
            {
                SolidColorBrush brush = control.Background as SolidColorBrush;
                VerifyElement.VerifyBool(brush == null, false);
                VerifyElement.VerifyColor(brush.Color, Colors.Red);
            }
            else
            {
                //Verify Template
                ControlTemplate template = control.Template as ControlTemplate;
                VerifyElement.VerifyBool(template == null, false);
                //Verify Property Trigger

                CoreLogger.LogStatus("Verifying property trigger.");
                SolidColorBrush brush = control.Background as SolidColorBrush;
                VerifyElement.VerifyBool(brush == null, false);
                VerifyElement.VerifyColor(brush.Color, Colors.LightGreen);

                //Verify FindName
                Control controlInTemplate = template.FindName("ControlInTemplate", control) as Control;
                VerifyElement.VerifyBool(controlInTemplate == null, false);
            }
        }


        /// <summary>
        /// ModelState.
        /// </summary>
        EmbeddedControlModelState ModelState
        {
            get
            {
                return _modelState;
            }
            set
            {
                _modelState = value;
            }
        }

        EmbeddedControlModelState _modelState = null;
        int _numberOfControls = 0;
    }

    /// <summary>
    /// EmbeddedControlModelState inherits CoreModelState and 
    /// holds the parameters from the Model, as well as a LogState function 
    /// which print out the information about the correct state. 
    /// </summary>
    [Serializable()]
    class EmbeddedControlModelState : CoreModelState
    {
        public EmbeddedControlModelState(State state)
        {
            BlockDerivedClassHolder = state["BlockDerivedClassHolder"];
            BlockDerivedClassBlock = state["BlockDerivedClassBlock"];
            InLineElementBlock = state["InLineElementBlock"];
            Control = state["Control"];
            BoldSpace = state["BoldSpace"];
            HyperlinkSpace = state["HyperlinkSpace"];
            ControlWrapper = state["ControlWrapper"];
            Sequence = state["Sequence"];
            MixedContent = state["MixedContent"];
            UnderParagraph = state["UnderParagraph"];
            ResourceLookUp = state["ResourceLookUp"];
        }

        public override void LogState()
        {

            CoreLogger.LogStatus(
                "BlockDerivedClassHolder: " + BlockDerivedClassHolder +
                "\r\n  BlockDerivedClassBlock: " + BlockDerivedClassBlock +
                "\r\n  InLineElementBlock: " + InLineElementBlock +
                "\r\n  Control: " + Control +
                "\r\n  BoldSpace: " + BoldSpace +
                "\r\n  HyperlinkSpace: " + HyperlinkSpace +
                "\r\n  ControlWrapper: " + ControlWrapper +
                "\r\n  Sequence: " + Sequence +
                "\r\n  MixedContent: " + MixedContent +
                "\r\n  ResourceLookUp: " + ResourceLookUp +
                "\r\n  UnderParagraph: " + UnderParagraph               
                );
        }
        public string BlockDerivedClassHolder;
        public string BlockDerivedClassBlock;
        public string InLineElementBlock;
        public string Control;
        public string BoldSpace;
        public string HyperlinkSpace;
        public string ControlWrapper;
        public string Sequence;
        public string MixedContent;
        public string UnderParagraph;
        public string ResourceLookUp;
    }
}
