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
    /// This is the base class for TextContainer test cases. It has some helper functions like DumpTextTree, verifier
    /// for comparing the dumps with the expected dumps and a log function to log the TextContainer dumps.
    /// </summary>
    public abstract class TextTreeTestBase : CustomTestCase
    {
        /// <summary>Initializes a new TextPointerTestBase instance.</summary>
        public TextTreeTestBase() : base()
        {
        }

        /// <summary>
        /// Dumps the TextContainer. Calls the DumpTextTreeNode function with the root node.
        /// </summary>
        /// <param name="tree">TextContainer to be dumped</param>
        /// <returns>Output string has the list of all the nodes in the tree. The left most child
        /// is listed first and the right most child is listed at the end</returns>
        public string DumpTextTree(TextContainer tree)
        {
            String treeDump;
            object rootNode = ReflectionUtils.GetField(tree, "_rootNode");

            if (rootNode == null)
            {
                treeDump="[]";
            }
            else
            {
                treeDump = DumpTextTreeNode(rootNode, 0);
            }

            return treeDump.ToString();
        }

        /// <summary>
        /// Logs the TextContainer in the log file.
        /// </summary>
        /// <param name="tree">TextContainer to be logged</param>
        public void LogTextTree(TextContainer tree)
        {
            string dumpTextTree = DumpTextTree(tree);
            Log(DrawTree(dumpTextTree));
        }

        /// <summary>
        /// Dumps the TextContainerNode. It makes a recursive call to its LEFT, CONTAINEDNODE and RIGHT in the following way
        /// If (LeftChild Exists)
        ///     Call recursively on LeftChild
        /// If (ContainedChild Exists)
        ///     Call recursively on ContainedChild
        /// Else
        ///     Print the Node
        /// If (Node is not RootNode)
        ///     If (Parent.ContainedNode == this)
        ///         Print the ParentNode
        /// If (RightChild Exists)
        ///     Call recursively on RightChild
        /// </summary>
        /// <param name="node">Node to be dumped (TextContainerNode)</param>
        /// <param name="indent">Indent for the node (root node will have 0 indent)</param>
        /// <returns>Output string has the list of all the nodes under the node given as input. The left most child
        /// is listed first and the right most child is listed at the end</returns>
        public string DumpTextTreeNode(object node, int indent)
        {
            System.Text.StringBuilder nodeDump = new System.Text.StringBuilder();
            object tempNode;

            tempNode = ReflectionUtils.GetProperty(node, "LeftChildNode");
            if (tempNode != null)
            {
                //nodeDump.Append(DumpTextTreeNode(tempNode, indent + 1));
                nodeDump.Append(DumpTextTreeNode(tempNode, indent));
            }

            tempNode = ReflectionUtils.GetProperty(node, "ContainedNode");
            if (tempNode != null)
            {
                nodeDump.Append(NodeToString(node, indent));
                nodeDump.Append(DumpTextTreeNode(tempNode, indent + 1));
                nodeDump.Append(NodeToString(node, indent));
            }
            else
            {
                nodeDump.Append(NodeToString(node, indent));
            }

            /*
            if(ReflectionUtils.GetNameFromFullTypeName(node.GetType().ToString()) != "TextTreeRootNode")
            {
                object parent = ReflectionUtils.GetProperty(node, "ParentNode");

                if (ReflectionUtils.GetProperty(parent, "ContainedNode") == node)
                    nodeDump.Append(NodeToString(parent, indent - 1));
            }
            */
            
            tempNode = ReflectionUtils.GetProperty(node, "RightChildNode");
            if (tempNode != null)
            {
                //nodeDump.Append(DumpTextTreeNode(tempNode, indent + 1));
                nodeDump.Append(DumpTextTreeNode(tempNode, indent));
            }

            return nodeDump.ToString();
        }

        /// <summary>
        /// This is like a ToString function for the TextContainer nodes.
        /// </summary>
        /// <param name="node">Input Node</param>
        /// <param name="indent">Indent will be helpfull when drawing the tree and will also be used when verying the dump.</param>
        /// <returns>Outputs a string with the node details.</returns>
        public string NodeToString(object node, int indent)
        {
            System.Text.StringBuilder nodeString = new System.Text.StringBuilder();

            nodeString.Append("[ ");
            string nodeType = node.GetType().ToString();
            nodeType = ReflectionUtils.GetNameFromFullTypeName(nodeType);
            nodeString.Append(nodeType);
            
            //nodeString.Append(" Role=");
            //nodeString.Append(ReflectionUtils.GetProperty(node, "Role"));
            nodeString.Append(" SC=");
            nodeString.Append(ReflectionUtils.GetProperty(node, "SymbolCount"));
            nodeString.Append(" Indent=");
            nodeString.Append(indent);
            nodeString.Append(" ];");
            return nodeString.ToString();
        }

        /// <summary>
        /// Draws the TextContainer with indents.
        /// </summary>
        /// <param name="treeString">string which represents the tree. Output of DumpTextTree</param>
        /// <returns>Outputs a string with all indents</returns>
        public string DrawTree(string treeString)
        {
            //int indentIndex =4;
            int indentIndex = 3;
            System.Text.StringBuilder tree = new System.Text.StringBuilder();

            tree.AppendLine();
            tree.Append('-', 8);
            tree.Append("TREE");
            tree.Append('-', 8);
            tree.AppendLine();

            string[] treeNodes = treeString.Split(';');

            foreach (string node in treeNodes)
            {
                if (node == String.Empty)
                    continue;

                string[] elements = node.Split(' ');
                int found = elements[indentIndex].IndexOf('=');

                int indent = Int32.Parse(elements[indentIndex].Substring(found+1));

                tree.Append(' ', indent*3);
                for (int i = 0; i < elements.Length; i++)
                {
                    if (i != indentIndex)
                    {
                        tree.Append(elements[i]);
                        tree.Append(' ');
                    }
                }
                tree.AppendLine();
            }

            tree.Append('-', 20);
            tree.AppendLine();
            //System.Console.WriteLine(tree.ToString());
            return tree.ToString();
        }

        /// <summary>
        /// Verifier for treeDump with the expected Dump. This function calls GC.Collect() and sleeps for a 
        /// short time so that all optimization on TextContainerNodes will be done before we compare.
        /// </summary>
        /// <param name="textTree">TextContainer to be verified</param>
        /// <param name="expDump">Expected Dump</param>
        public void VerifyDump(TextContainer textTree, string expDump)
        {
            GC.Collect();
            Thread.Sleep(100);
            //call a public API so that TextContainerNodes merging/placement optimization gets done.
            textTree.CanInsertEmbeddedObject(typeof(UIElement));

            Log("Verifying TextContainer dump...");
            string actualDump = DumpTextTree(textTree);
            Log("Actual Dump: " + DrawTree(actualDump));
            Log("Expected Dump: " + DrawTree(expDump));
            Verifier.Verify(actualDump == expDump);
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {  
        }
        
        /// <summary>TextContainer to be tested.</summary>
        protected TextContainer TextTree
        {
            get { return this._textTree; }
            set { this._textTree = value;}
        }

        private TextContainer _textTree = null;
    }

    /// <summary>
    /// This class is for testing the OnParentChanged events
    /// </summary>
    public class MyTextElement : TextElement
    {
        DependencyObject _oldParent;

        /// <summary>
        /// Will return true if the parent has changed since the last call
        /// </summary>
        public bool HasParentChanged
        {
            get
            {
                if (_oldParent != Parent)
                {
                    _oldParent = Parent;
                    return true;
                }
                return false;
            }
        }
    }

    /// <summary>
    /// This class is for testing the OnParentChanged events
    /// </summary>
    public class MyFrameworkElement : FrameworkElement
    {
        DependencyObject _oldParent;

        /// <summary>
        /// Will return true if the parent has changed since the last call
        /// </summary>
        public bool HasParentChanged
        {
            get
            {
                if (_oldParent != Parent)
                {
                    _oldParent = Parent;
                    return true;
                }
                return false;
            }
        }
    }

    // Command Line: /TestCaseType:TextTreeTest_TextEditing /TestName:TextTreeTest_TextEditing-Simple /xml:testxml.xml
    /// <summary>
    /// Performs various Insertion/Deletion operations on plain text in TextContainer and then compares its dump with 
    /// the expected dump. It also creates some TextPointers and insert text at those positions.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("TextStep1", "Text to be inserted into an empty TextContainer for step1"),
    TestArgument("TextStep2", "Text to be inserted at End of a non-empty TextContainer for step2"),
    TestArgument("TextStep3", "Text to be inserted at Start of a non-empty TextContainer for step3"),
    TestArgument("TextStep4", "Text to be inserted at TextPointer1 for step4"), 
    TestArgument("TextStep5", "Text to be inserted at TextPointer2 for step5"), 
    TestArgument("TextStep7", "Text to be inserted at TextPointer3 for step7"), 
    TestArgument("TP1", "Distance from Start at which TextPointer1 is to be placed"), 
    TestArgument("TP2", "Distance from Start at which TextPointer2 is to be placed"),
    TestArgument("TP3FromTP1", "Distance from TextPointer1 at which TextPointer3 is to be placed"),
    TestArgument("ExpectedTreeDumpStep3", "Expected TextContainer dump after step3"),
    TestArgument("ExpectedTreeDumpStep5", "Expected TextContainer dump after step5"),
    TestArgument("ExpectedTreeDumpStep6", "Expected TextContainer dump after step6"),
    TestArgument("ExpectedTreeDumpStep7", "Expected TextContainer dump after step7"),
    TestArgument("ExpectedTreeDumpStep8", "Expected TextContainer dump after step8")]
    public class TextTreeTest_TextEditing : TextTreeTestBase
    {
        #region Settings
        /// <summary>Text to be inserted into an empty TextContainer for step1</summary>
        private string TextStep1
        {
            get { return Settings.GetArgument("TextStep1"); }
        }

        /// <summary>Text to be inserted at End of a non-empty TextContainer for step2</summary>
        private string TextStep2
        {
            get { return Settings.GetArgument("TextStep2"); }
        }

        /// <summary>Text to be inserted at Start of a non-empty TextContainer for step3</summary>
        private string TextStep3
        {
            get { return Settings.GetArgument("TextStep3"); }
        }

        /// <summary>Text to be inserted at TextPointer1 for step4</summary>
        private string TextStep4
        {
            get { return Settings.GetArgument("TextStep4"); }
        }

        /// <summary>Text to be inserted at TextPointer2 for step5</summary>
        private string TextStep5
        {
            get { return Settings.GetArgument("TextStep5"); }
        }

        /// <summary>Text to be inserted at TextPointer3 for step7</summary>
        private string TextStep7
        {
            get { return Settings.GetArgument("TextStep7"); }
        }

        /// <summary>Distance from Start at which TextPointer1 is to be placed</summary>
        private int TP1
        {
            get { return Settings.GetArgumentAsInt("TP1"); }
        }

        /// <summary>Distance from Start at which TextPointer2 is to be placed</summary>
        private int TP2
        {
            get { return Settings.GetArgumentAsInt("TP2"); }
        }

        /// <summary>Distance from TextPointer1 at which TextPointer3 is to be placed</summary>
        private int TP3FromTP1
        {
            get { return Settings.GetArgumentAsInt("TP3FromTP1"); }
        }

        /// <summary>Expected TextContainer dump after step3</summary>
        private string ExpectedTreeDumpStep3
        {
            get { return Settings.GetArgument("ExpectedTreeDumpStep3"); }
        }

        /// <summary>Expected TextContainer dump after step5</summary>
        private string ExpectedTreeDumpStep5
        {
            get { return Settings.GetArgument("ExpectedTreeDumpStep5"); }
        }

        /// <summary>Expected TextContainer dump after step6</summary>
        private string ExpectedTreeDumpStep6
        {
            get { return Settings.GetArgument("ExpectedTreeDumpStep6"); }
        }

        /// <summary>Expected TextContainer dump after step7</summary>
        private string ExpectedTreeDumpStep7
        {
            get { return Settings.GetArgument("ExpectedTreeDumpStep7"); }
        }

        /// <summary>Expected TextContainer dump after step8</summary>
        private string ExpectedTreeDumpStep8
        {
            get { return Settings.GetArgument("ExpectedTreeDumpStep8"); }
        }

        #endregion Settings

        #region Members
        TextPointer _tp1 = null;
        TextPointer _tp2 = null;
        TextPointer _tp3 = null;
        #endregion Members

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            this.TextTree = new TextContainer();

            Log("Step1: Insert text in to an empty TextContainer...");
            TextTree.InsertText(TextTree.Start, TextStep1);

            Log("Step2: Insert text at the END of a non-empty TextContainer...");
            TextTree.InsertText(TextTree.End, TextStep2);

            Log("Step3: Insert text at the START of a non-empty TextContainer...");
            TextTree.InsertText(TextTree.Start, TextStep3);

            VerifyDump(TextTree, ExpectedTreeDumpStep3);

            Log("Creating TextPointer's TP1 and TP2...");
            _tp1 = TextTree.Start.CreatePosition(TP1, LogicalDirection.Backward);
            _tp2 = TextTree.Start.CreatePosition(TP2, LogicalDirection.Forward);
            LogTextTree(TextTree);

            Log("Step4: Insert text at a TextPointer(TP1) with gravity = backward...");
            TextTree.InsertText(_tp1, TextStep4);

            Log("Step5: Insert text at a TextPointer(TP2) with gravity = forward...");
            TextTree.InsertText(_tp2, TextStep5);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep5);
            
            Log("Creating a TextPointer(TP3) between TP1 and TP2 and then will delete the text between TP1 and TP2");
            _tp3 = _tp1.CreatePosition(TP3FromTP1, LogicalDirection.Forward);

            Log("Step6: Delete contents between two TextPointer's...");
            TextTree.DeleteContent(_tp1, _tp2);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep6);
            
            Log("Step7: Insert text at a TextPointer(TP3) with gravity = forward...");
            TextTree.InsertText(_tp3, TextStep7);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep7);
            
            Log("Step8: Delete all contents of TextContainer...");
            TextTree.DeleteContent(this.TextTree.Start, this.TextTree.End);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep8);
            
            Logger.Current.ReportSuccess();
        }
    }

    // Command Line: /TestCaseType:TextTreeTest_TextElementEditing /TestName:TextTreeTest_TextElementEditing-Simple /xml:testxml.xml
    /// <summary>
    /// Performs various Insertion/Deletion operations on Text/TextElements in TextContainer and then compares its dump with 
    /// the expected dump. TextPointers are used to insert text/textelements in most of the steps.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("TextElementTypeName1", "Type name of TextElement to be inserted at Step1"),
    TestArgument("TextElementTypeName2", "Type name of TextElement to be inserted at Step8"),
    TestArgument("TextStep2", "Text to be inserted at Step2"),
    TestArgument("TextStep3", "Text to be inserted at Step3"),
    TestArgument("TextStep4", "Text to be inserted at Step4"),
    TestArgument("TextStep7", "Text to be inserted at Step7"),
    TestArgument("TN1", "Distance from Start at which TextPointer1 is to be placed. Should be equal to (strln(TextStep2) + 1)"),
    TestArgument("TN2FromTN1", "Distance from TextPointer1 to the left at which TextPointer2 is to be placed. Should be less than strln(TextStep2)"),
    TestArgument("TN2Step7", "Distance from Start at which TextPointer2 is to be placed for Step7. Should be >= 4 (we want to nest textelements) and less than strln(TextStep7)"),
    TestArgument("ExpectedTreeDumpStep2", "Expected TextContainer dump after step2"),
    TestArgument("ExpectedTreeDumpStep4", "Expected TextContainer dump after step4"),
    TestArgument("ExpectedTreeDumpStep5", "Expected TextContainer dump after step5"),
    TestArgument("ExpectedTreeDumpStep6", "Expected TextContainer dump after step6"),
    TestArgument("ExpectedTreeDumpStep8", "Expected TextContainer dump after step8"),
    TestArgument("ExpectedTreeDumpStep10", "Expected TextContainer dump after step10")]
    public class TextTreeTest_TextElementEditing : TextTreeTestBase
    {
        #region TestCaseData
        /// <summary>Data driven test cases.</summary>
        internal class TestData
        {
            #region PrivateData
            string _textElementTypeName1;
            string _textElementTypeName2;
            string _textStep2;
            int _TN1;           //-- Length of TextStep2 + 1 --
            int _TN2FromTN1;    //-- Should be less then the length of TextStep2 --
            string _textStep3;
            string _textStep4;
            string _textStep7;
            int _TN2Step7;      //-- Should be greater or equal to 4 (we want to nest 
                                //   textelements) and less than length of TextStep7 --
            string _expectedTreeDumpStep2;
            string _expectedTreeDumpStep4;
            string _expectedTreeDumpStep5;
            string _expectedTreeDumpStep6;
            string _expectedTreeDumpStep8;
            string _expectedTreeDumpStep10;
            #endregion PrivateData

            #region InternalProperties
            internal string TextElementTypeName1 { get { return this._textElementTypeName1; } }
            internal string TextElementTypeName2 { get { return this._textElementTypeName2; } }
            internal string TextStep2 { get { return this._textStep2; } }
            internal int TN1 { get { return this._TN1; } }
            internal int TN2FromTN1 { get { return this._TN2FromTN1; } }
            internal string TextStep3 { get { return this._textStep3; } }
            internal string TextStep4 { get { return this._textStep4; } }
            internal string TextStep7 { get { return this._textStep7; } }
            internal int TN2Step7 { get { return this._TN2Step7; } }
            internal string ExpectedTreeDumpStep2 { get { return this._expectedTreeDumpStep2; } }
            internal string ExpectedTreeDumpStep4 { get { return this._expectedTreeDumpStep4; } }
            internal string ExpectedTreeDumpStep5 { get { return this._expectedTreeDumpStep5; } }
            internal string ExpectedTreeDumpStep6 { get { return this._expectedTreeDumpStep6; } }
            internal string ExpectedTreeDumpStep8 { get { return this._expectedTreeDumpStep8; } }
            internal string ExpectedTreeDumpStep10 { get { return this._expectedTreeDumpStep10; } }
            #endregion InternalProperties

            internal TestData(string textElementTypeName1, string textElementTypeName2, string textStep2, int TN1,
                            int TN2FromTN1, string textStep3, string textStep4, string textStep7, int TN2Step7,
                            string expectedTreeDumpStep2, string expectedTreeDumpStep4, string expectedTreeDumpStep5, 
                            string expectedTreeDumpStep6, string expectedTreeDumpStep8, string expectedTreeDumpStep10)
            {
                this._textElementTypeName1 = textElementTypeName1;
                this._textElementTypeName2 = textElementTypeName2;
                this._textStep2 = textStep2;
                this._TN1 = TN1;
                this._TN2FromTN1 = TN2FromTN1;
                this._textStep3 = textStep3;
                this._textStep4 = textStep4;
                this._textStep7 = textStep7;
                this._TN2Step7 = TN2Step7;
                this._expectedTreeDumpStep2 = expectedTreeDumpStep2;
                this._expectedTreeDumpStep4 = expectedTreeDumpStep4;
                this._expectedTreeDumpStep5 = expectedTreeDumpStep5;
                this._expectedTreeDumpStep6 = expectedTreeDumpStep6;
                this._expectedTreeDumpStep8 = expectedTreeDumpStep8;
                this._expectedTreeDumpStep10 = expectedTreeDumpStep10;
            }

            internal static TestData[] TestCases = new TestData[] {
                new TestData("System.Windows.Documents.Inline", "System.Windows.Documents.Bold", 
                            "abcd", 5, -2, "efgh", "ijkl", "abcdefghijkl", 6,
                            "[ TextTreeRootNode SC=8 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=2 Indent=1 ];[ TextTreeRootNode SC=8 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=12 Indent=0 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=12 Indent=0 ];",
                            "[ TextTreeRootNode SC=18 Indent=0 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=4 Indent=3 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=18 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextNode SC=3 Indent=2 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];"),
                new TestData("System.Windows.Documents.Paragraph", "System.Windows.Documents.Paragraph", 
                            "abcd", 5, -2, "efgh", "ijkl", "abcdefghijkl", 6,
                            "[ TextTreeRootNode SC=8 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=2 Indent=1 ];[ TextTreeRootNode SC=8 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=12 Indent=0 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=12 Indent=0 ];",
                            "[ TextTreeRootNode SC=18 Indent=0 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=4 Indent=3 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=18 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextNode SC=3 Indent=2 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];"),
                new TestData("System.Windows.Documents.Heading", "System.Windows.Documents.Heading", 
                            "abcd", 5, -2, "efgh", "ijkl", "abcdefghijkl", 6,
                            "[ TextTreeRootNode SC=8 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=2 Indent=1 ];[ TextTreeRootNode SC=8 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=12 Indent=0 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=12 Indent=0 ];",
                            "[ TextTreeRootNode SC=18 Indent=0 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=4 Indent=3 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=18 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextNode SC=3 Indent=2 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];"),
                new TestData("System.Windows.Documents.Section", "System.Windows.Documents.Section", 
                            "abcd", 5, -2, "efgh", "ijkl", "abcdefghijkl", 6,
                            "[ TextTreeRootNode SC=8 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=2 Indent=1 ];[ TextTreeRootNode SC=8 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=12 Indent=0 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=12 Indent=0 ];",
                            "[ TextTreeRootNode SC=18 Indent=0 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=4 Indent=3 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=18 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextNode SC=3 Indent=2 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];"),
                new TestData("System.Windows.Documents.LineBreak", "System.Windows.Documents.LineBreak", 
                            "abcd", 5, -2, "efgh", "ijkl", "abcdefghijkl", 6,
                            "[ TextTreeRootNode SC=8 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=2 Indent=1 ];[ TextTreeRootNode SC=8 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=12 Indent=0 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=12 Indent=0 ];",
                            "[ TextTreeRootNode SC=18 Indent=0 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=4 Indent=3 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=18 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextNode SC=3 Indent=2 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];"),
                new TestData("System.Windows.Documents.Italic", "System.Windows.Documents.Italic", 
                            "abcd", 5, -2, "efgh", "ijkl", "abcdefghijkl", 6,
                            "[ TextTreeRootNode SC=8 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=2 Indent=1 ];[ TextTreeRootNode SC=8 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=12 Indent=0 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=12 Indent=0 ];",
                            "[ TextTreeRootNode SC=18 Indent=0 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=4 Indent=3 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=18 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextNode SC=3 Indent=2 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];"),
                new TestData("System.Windows.Documents.Underline", "System.Windows.Documents.Underline", 
                            "abcd", 5, -2, "efgh", "ijkl", "abcdefghijkl", 6,
                            "[ TextTreeRootNode SC=8 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=2 Indent=1 ];[ TextTreeRootNode SC=8 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=4 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=12 Indent=0 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=12 Indent=0 ];",
                            "[ TextTreeRootNode SC=18 Indent=0 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=4 Indent=3 ];[ TextTreeTextElementNode SC=6 Indent=2 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextElementNode SC=10 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=18 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=1 Indent=2 ];[ TextTreeTextNode SC=3 Indent=2 ];[ TextTreeTextNode SC=2 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeTextNode SC=6 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];"),
            };
        }
        /// <summary>Current test data being used</summary>
        private TestData _testData;

        /// <summary> TextPointers which are used in the test</summary>
        TextPointer _tn1 = null;
        TextPointer _tn2 = null;

        #endregion TestCaseData

        #region Settings
        /// <summary>Type of TextElement to be inserted at Step1</summary>
        private string TextElementTypeName1
        {
            //get { return Settings.GetArgument("TextElementTypeName1"); }
            get { return this._testData.TextElementTypeName1; }
        }

        /// <summary>Type of TextElement to be inserted at Step8</summary>
        private string TextElementTypeName2
        {
            //get { return Settings.GetArgument("TextElementTypeName2"); }
            get { return this._testData.TextElementTypeName2; }
        }

        /// <summary>Text to be inserted at Step2</summary>
        private string TextStep2
        {
            //get { return Settings.GetArgument("TextStep2"); }
            get { return this._testData.TextStep2; }
        }

        /// <summary>Text to be inserted at Step3</summary>
        private string TextStep3
        {
            //get { return Settings.GetArgument("TextStep3"); }
            get { return this._testData.TextStep3; }
        }

        /// <summary>Text to be inserted at Step4</summary>
        private string TextStep4
        {
            //get { return Settings.GetArgument("TextStep4"); }
            get { return this._testData.TextStep4; }
        }

        /// <summary>Text to be inserted at Step7</summary>
        private string TextStep7
        {
            //get { return Settings.GetArgument("TextStep7"); }
            get { return this._testData.TextStep7; }
        }

        /// <summary>Distance from Start at which TextPointer1 is to be placed. Should be equal to (strln(TextStep2) + 1)</summary>
        private int TN1
        {
            //get { return Settings.GetArgumentAsInt("TN1"); }
            get { return this._testData.TN1; }
        }

        /// <summary>Distance from TextPointer1 to the left at which TextPointer2 is to be placed. Should be less than strln(TextStep2)</summary>
        private int TN2FromTN1
        {
            //get { return Settings.GetArgumentAsInt("TN2FromTN1"); }
            get { return this._testData.TN2FromTN1; }
        }

        /// <summary>Distance from Start at which TextPointer2 is to be placed for Step7. Should be >= 4 (we want to nest textelements) and less than strln(TextStep7)</summary>
        private int TN2Step7
        {
            //get { return Settings.GetArgumentAsInt("TN2Step7"); }
            get { return this._testData.TN2Step7; }
        }

        /// <summary>Expected TextContainer dump after step2</summary>
        private string ExpectedTreeDumpStep2
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep2"); }
            get { return this._testData.ExpectedTreeDumpStep2; }
        }

        /// <summary>Expected TextContainer dump after step4</summary>
        private string ExpectedTreeDumpStep4
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep4"); }
            get { return this._testData.ExpectedTreeDumpStep4; }
        }

        /// <summary>Expected TextContainer dump after step5</summary>
        private string ExpectedTreeDumpStep5
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep5"); }
            get { return this._testData.ExpectedTreeDumpStep5; }
        }

        /// <summary>Expected TextContainer dump after step6</summary>
        private string ExpectedTreeDumpStep6
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep6"); }
            get { return this._testData.ExpectedTreeDumpStep6; }
        }

        /// <summary>Expected TextContainer dump after step8</summary>
        private string ExpectedTreeDumpStep8
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep8"); }
            get { return this._testData.ExpectedTreeDumpStep8; }
        }

        /// <summary>Expected TextContainer dump after step10</summary>
        private string ExpectedTreeDumpStep10
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep10"); }
            get { return this._testData.ExpectedTreeDumpStep10; }
        }

        #endregion Settings

        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            for (int i = 0; i < TestData.TestCases.Length; i++)
            {
                _testData = TestData.TestCases[i];
                Log("Running test case #: " + i);
                RunCase();
            }
            Logger.Current.ReportSuccess();
        }

        private void RunCase()
        {
            _tn1 = null;
            _tn2 = null;
            Type elementType = null;

            this.TextTree = new TextContainer();

            elementType = ReflectionUtils.FindType(TextElementTypeName1);
            object elementObject1 = ReflectionUtils.CreateInstance(elementType);

            Log("Step1: Insert element (TE1) in an empty TextContainer...");
            Verifier.Verify(this.TextTree.CanInsertElement(elementType), "Verifying CanInsertElement", false);
            TextTree.InsertElement(TextTree.Start, TextTree.End, (TextElement)elementObject1);            

            Log("Step2: Insert text at Start of the TextContainer which has a TextElement...");
            TextTree.InsertText(TextTree.Start, TextStep2);

            VerifyDump(TextTree, ExpectedTreeDumpStep2);

            Log("Creating a TextPointer (TN1) inside the element (TE1)...");
            _tn1= TextTree.Start.CreateNavigator(TN1);
            _tn1.SetGravity(LogicalDirection.Forward);
            LogTextTree(this.TextTree);

            Log("Step3: Insert Text inside the element (TE1)...");
            TextTree.InsertText(_tn1, TextStep3);

            Log("Step4: Insert text at End of the tree...");
            TextTree.InsertText(TextTree.End, TextStep4);

            VerifyDump(TextTree, ExpectedTreeDumpStep4);

            Log("Creating a TextPointer (TN2) inside the element (TE1) to the left of TN1...");
            _tn2 = _tn1.CreateNavigator();
            _tn2.SetGravity(LogicalDirection.Backward);
            _tn2.MoveByDistance(TN2FromTN1);
            LogTextTree(this.TextTree);

            Log("Step5: Delete some contents inside the TextElement (TE1) between TN1 and TN2...");
            TextTree.DeleteContent(_tn2, _tn1);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep5);

            Log("Step6: Extract the TextElement (TE1)...");
            TextTree.ExtractElement(_tn1);

            VerifyDump(TextTree, ExpectedTreeDumpStep6);

            Log("Deleting all the contents of the TextContainer...");
            this.TextTree.DeleteContent(this.TextTree.Start, this.TextTree.End);

            Log("Inserting the following text into the TextContainer: " + TextStep7);
            TextTree.InsertText(this.TextTree.Start, TextStep7);

            Log("Placing TextPointer (TN1) at Start and TextPointer (TN2) at a distance of " + TN2Step7 + " from Start...");
            _tn1 = this.TextTree.Start.CreateNavigator();
            _tn2 = this.TextTree.Start.CreateNavigator(TN2Step7);            

            Log("Step7: Re-insert element TE1 between TN1 and TN2...");
            TextTree.InsertElement(_tn1, _tn2, (TextElement)elementObject1);
            LogTextTree(this.TextTree);

            Log("Moving TextPointers TN1 and TN2 inside the TextElement TE1 so that we can create a nested TextElement...");
            _tn1.MoveByDistance(2);
            _tn2.MoveByDistance(-1);
            LogTextTree(this.TextTree);

            elementType = ReflectionUtils.FindType(TextElementTypeName2);
            object elementObject2 = ReflectionUtils.CreateInstance(elementType);

            Log("Step8: Insert Textelement TE2 inside TextElement TE1...");
            TextTree.InsertElement(_tn1, _tn2, (TextElement)elementObject2);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep8);

            Log("Moving TextPointer TN2 by 1 position to left so that we can try inserting an element which doesn't obey scoping of TE1 and TE2...");
            _tn2.MoveByDistance(-1);
            LogTextTree(this.TextTree);

            Log("Step9: Try inserting a Textelement which doesn't obey the scoping of elements (TE1, TE2)...");
            try
            {
                TextTree.InsertElement(_tn2, this.TextTree.End, elementType);
                throw(new ApplicationException());
            }
            catch(InvalidOperationException)
            {
                Log("Expected exception got fired...");
            }
            catch(ApplicationException)
            {
                Log("Expected exception didnt get fired...");
                Verifier.Verify(false, "Expected InvalidOperationException didnt get fired", true);
            }

            Log("Step10: Extract the TextElement (TE2)...");
            TextTree.ExtractElement(_tn2);

            VerifyDump(TextTree, ExpectedTreeDumpStep10);
        }
    }

    // Command Line: /TestCaseType:TextTreeTest_UIElementEditing /TestName:TextTreeTest_UIElementEditing-Simple /xml:testxml.xml
    /// <summary>
    /// Performs various Insertion/Deletion operations on Text/TextElements/UIElements in TextContainer and then compares its dump with 
    /// the expected dump.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("EmbeddedObjectTypeName", "Embedded Object type to be inserted in the TextContainer"),
    TestArgument("TextElementTypeName", "TextElement type to be inserted in the TextContainer"),
    TestArgument("TextToInsert", "Text to be inserted in the TextContainer"),
    TestArgument("ExpectedTreeDumpStep3", "Expected TextContainer dump after step3"),
    TestArgument("ExpectedTreeDumpStep4", "Expected TextContainer dump after step4"),
    TestArgument("ExpectedTreeDumpStep5", "Expected TextContainer dump after step5"),
    TestArgument("ExpectedTreeDumpStep6", "Expected TextContainer dump after step6"),
    TestArgument("ExpectedTreeDumpStep7", "Expected TextContainer dump after step7"),
    TestArgument("ExpectedTreeDumpStep8", "Expected TextContainer dump after step8"),
    TestArgument("ExpectedTreeDumpStep9", "Expected TextContainer dump after step9")]
    public class TextTreeTest_UIElementEditing : TextTreeTestBase
    {
        #region TestCaseData
        /// <summary>Data driven test cases.</summary>
        internal class TestData
        {
            #region PrivateData
            string _embeddedObjectTypeName;
            string _textElementTypeName;
            string _textToInsert;
            string _expectedTreeDumpStep3;
            string _expectedTreeDumpStep5;
            string _expectedTreeDumpStep6;
            string _expectedTreeDumpStep7;
            string _expectedTreeDumpStep8;
            string _expectedTreeDumpStep9;
            #endregion PrivateData

            #region InternalProperties
            internal string EmbeddedObjectTypeName { get { return this._embeddedObjectTypeName; } }
            internal string TextElementTypeName { get { return this._textElementTypeName; } }
            internal string TextToInsert { get { return this._textToInsert; } }
            internal string ExpectedTreeDumpStep3 { get { return this._expectedTreeDumpStep3; } }
            internal string ExpectedTreeDumpStep5 { get { return this._expectedTreeDumpStep5; } }
            internal string ExpectedTreeDumpStep6 { get { return this._expectedTreeDumpStep6; } }
            internal string ExpectedTreeDumpStep7 { get { return this._expectedTreeDumpStep7; } }
            internal string ExpectedTreeDumpStep8 { get { return this._expectedTreeDumpStep8; } }
            internal string ExpectedTreeDumpStep9 { get { return this._expectedTreeDumpStep9; } }
            #endregion InternalProperties

            internal TestData(string embeddedObjectTypeName, string textElementTypeName, string textToInsert, string expectedTreeDumpStep3,
                            string expectedTreeDumpStep5, string expectedTreeDumpStep6, string expectedTreeDumpStep7,
                            string expectedTreeDumpStep8, string expectedTreeDumpStep9)
            {
                this._embeddedObjectTypeName = embeddedObjectTypeName;
                this._textElementTypeName = textElementTypeName;
                this._textToInsert = textToInsert;
                this._expectedTreeDumpStep3 = expectedTreeDumpStep3;
                this._expectedTreeDumpStep5 = expectedTreeDumpStep5;
                this._expectedTreeDumpStep6 = expectedTreeDumpStep6;
                this._expectedTreeDumpStep7 = expectedTreeDumpStep7;
                this._expectedTreeDumpStep8 = expectedTreeDumpStep8;
                this._expectedTreeDumpStep9 = expectedTreeDumpStep9;
            }

            internal static TestData[] TestCases = new TestData[] {
                new TestData("Button", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
                new TestData("TextBox", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
                new TestData("TextPanel", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
                new TestData("Video", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
                new TestData("System.Windows.Shapes.Rectangle", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
                new TestData("System.Windows.Controls.Image", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
                new TestData("Viewport3D", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
                new TestData("ScrollViewer", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
                new TestData("Audio", "System.Windows.Documents.Inline", "abcdefgh", 
                            "[ TextTreeRootNode SC=11 Indent=0 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=11 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];",
                            "[ TextTreeRootNode SC=13 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=1 ];[ TextTreeTextNode SC=4 Indent=1 ];[ TextTreeRootNode SC=13 Indent=0 ];",
                            "[ TextTreeRootNode SC=14 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeRootNode SC=14 Indent=0 ];",
                            "[ TextTreeRootNode SC=16 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=8 Indent=1 ];[ TextTreeRootNode SC=16 Indent=0 ];",
                            "[ TextTreeRootNode SC=15 Indent=0 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=6 Indent=1 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeObjectNode SC=1 Indent=2 ];[ TextTreeTextNode SC=4 Indent=2 ];[ TextTreeTextElementNode SC=7 Indent=1 ];[ TextTreeRootNode SC=15 Indent=0 ];"),
            };
        }

        /// <summary>Current test data being used</summary>
        private TestData _testData;

        #endregion TestCaseData

        #region Settings
        /// <summary>Embedded Object type to be inserted in the TextContainer</summary>
        private string EmbeddedObjectTypeName
        {
            //get { return Settings.GetArgument("EmbeddedObjectTypeName"); }
            get { return this._testData.EmbeddedObjectTypeName; }
        }

        /// <summary>TextElement type to be inserted in the TextContainer</summary>
        private string TextElementTypeName
        {
            //get { return Settings.GetArgument("TextElementTypeName"); }
            get { return this._testData.TextElementTypeName; }
        }

        /// <summary>Text to be inserted in the TextContainer</summary>
        private string TextToInsert
        {
            //get { return Settings.GetArgument("TextToInsert"); }
            get { return this._testData.TextToInsert; }
        }
        
        /// <summary>Expected TextContainer dump after step3</summary>
        private string ExpectedTreeDumpStep3
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep3"); }
            get { return this._testData.ExpectedTreeDumpStep3; }
        }

        /// <summary>Expected TextContainer dump after step5</summary>
        private string ExpectedTreeDumpStep5
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep5"); }
            get { return this._testData.ExpectedTreeDumpStep5; }
        }
        
        /// <summary>Expected TextContainer dump after step6</summary>
        private string ExpectedTreeDumpStep6
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep6"); }
            get { return this._testData.ExpectedTreeDumpStep6; }
        }

        /// <summary>Expected TextContainer dump after step7</summary>
        private string ExpectedTreeDumpStep7
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep7"); }
            get { return this._testData.ExpectedTreeDumpStep7; }
        }

        /// <summary>Expected TextContainer dump after step8</summary>
        private string ExpectedTreeDumpStep8
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep8"); }
            get { return this._testData.ExpectedTreeDumpStep8; }
        }

        /// <summary>Expected TextContainer dump after step9</summary>
        private string ExpectedTreeDumpStep9
        {
            //get { return Settings.GetArgument("ExpectedTreeDumpStep9"); }
            get { return this._testData.ExpectedTreeDumpStep9; }
        }
        #endregion Settings

        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            for (int i = 0; i < TestData.TestCases.Length; i++)
            {
                _testData = TestData.TestCases[i];
                Log("Running test case #: " + i);
                RunCase();
            }
            Logger.Current.ReportSuccess();
        }

        private void RunCase()
        {
            Type objectType = null;
            Type elementType = null;
            TextPointer tn1 = null;
            
            bool testRes;
            this.TextTree = new TextContainer();

            objectType = ReflectionUtils.FindType(EmbeddedObjectTypeName);
            object embeddedObject1 = ReflectionUtils.CreateInstance(objectType);

            Log("Step1: Insert (UIElement UE1) in an empty TextContainer...");
            testRes = this.TextTree.CanInsertEmbeddedObject(objectType);
            Verifier.Verify(testRes, "Verifying TextContainer.CanInsertEmbeddedObject for Step1...", true);
            TextTree.InsertEmbeddedObject(TextTree.Start, embeddedObject1);
            LogTextTree(this.TextTree);

            Log("Step2: Remove (UIElement UE1) from the TextContainer...");
            TextTree.DeleteEmbeddedObject(TextTree.Start, LogicalDirection.Forward);
            LogTextTree(this.TextTree);

            Log("Inserting plain text into an empty TextContainer...");
            TextTree.InsertText(TextTree.Start, TextToInsert);

            Log("Placing a TextPointer in between the inserted text...");
            tn1 = this.TextTree.Start.CreateNavigator(TextToInsert.Length/2);
            LogTextTree(this.TextTree);

            Log("Step3: Insert the (UIElement UE1) in between the text...");
            testRes = this.TextTree.CanInsertEmbeddedObject(objectType);
            Verifier.Verify(testRes, "Verifying TextContainer.CanInsertEmbeddedObject for Step3...", true);
            TextTree.InsertEmbeddedObject(tn1, embeddedObject1);

            VerifyDump(TextTree, ExpectedTreeDumpStep3);

            elementType = ReflectionUtils.FindType(TextElementTypeName);
            object elementObj1 = ReflectionUtils.CreateInstance(elementType);
            object elementObj2 = ReflectionUtils.CreateInstance(elementType);

            Log("Step4: Insert TextElement (TE1) scoping the UIElement (UE1)...");
            TextTree.InsertElement(tn1, this.TextTree.End, (TextElement)elementObj1);

            Log("Step5: Insert TextElement (TE2) without scoping the UIElement (UE1)...");
            TextTree.InsertElement(this.TextTree.Start, tn1, (TextElement)elementObj2);

            VerifyDump(TextTree, ExpectedTreeDumpStep5);

            Log("Step6: Extract the TextElement (TE1) which scope's the UIElement (UE1)...");
            TextTree.ExtractElement((TextElement)elementObj1);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep6);

            Log("Moving the TextPointer by one position to the right and reinserting the TextElement TE1 scoping the UIElement (UE1)...");
            tn1.MoveByDistance(1);
            tn1.SetGravity(LogicalDirection.Forward);
            TextTree.InsertElement(tn1, this.TextTree.End, (TextElement)elementObj1);
            LogTextTree(this.TextTree);

            Log("Step7: Delete the UIElement (UE1) which is inside a TextElement (TE1)...");
            TextTree.DeleteEmbeddedObject(tn1, LogicalDirection.Forward);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep7);

            Log("Step8: Insert two UIElement (UE1 & UE2) side by side...");
            testRes = this.TextTree.CanInsertEmbeddedObject(objectType);
            Verifier.Verify(testRes, "Verifying TextContainer.CanDeleteEmbeddedObject for Step8...", true);
            TextTree.InsertEmbeddedObject(tn1, embeddedObject1);
            object embeddedObject2 = ReflectionUtils.CreateInstance(objectType);
            testRes = this.TextTree.CanInsertEmbeddedObject(objectType);
            Verifier.Verify(testRes, "Verifying TextContainer.CanDeleteEmbeddedObject for Step8...", true);
            TextTree.InsertEmbeddedObject(tn1, embeddedObject2);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep8);

            Log("Inserting a TextElement from Start to End of TextContainer...");

            Log("Step9: Delete the UIElement (UE2) which is inside a nested (TextElement)...");
            TextTree.DeleteEmbeddedObject(tn1, LogicalDirection.Backward);
            
            VerifyDump(TextTree, ExpectedTreeDumpStep9);
        }
    }
    
    // Command Line: /TestCaseType:TextTreeTest_TextChangedEvent /TestName:TextTreeTest_TextChangedEvent-Simple /xml:testxml.xml
    /// <summary>
    /// Performs various Insertion/Deletion operations on Text/TextElements/UIElements in TextContainer and then checks if TextChangedEvent is fired.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("EmbeddedObjectTypeName", "Embedded Object type to be inserted in the TextContainer"),
    TestArgument("TextElementTypeName", "TextElement type to be inserted in the TextContainer"),
    TestArgument("TextToInsert", "Text to be inserted in the TextContainer")]
    public class TextTreeTest_TextChangedEvent : TextTreeTestBase
    {
        #region Settings
        /// <summary>Embedded Object type to be inserted in the TextContainer</summary>
        private string EmbeddedObjectTypeName
        {
            get { return Settings.GetArgument("EmbeddedObjectTypeName"); }
        }

        /// <summary>TextElement type to be inserted in the TextContainer</summary>
        private string TextElementTypeName
        {
            get { return Settings.GetArgument("TextElementTypeName"); }
        }

        /// <summary>Text to be inserted in the TextContainer</summary>
        private string TextToInsertStep1
        {
            get { return Settings.GetArgument("TextToInsertStep1"); }
        }

        /// <summary>Text to be inserted in the TextContainer</summary>
        private string TextToInsertStep2
        {
            get { return Settings.GetArgument("TextToInsertStep2"); }
        }
        #endregion Settings

        #region Members
        Type _objectType = null;
        Type _elementType = null;
        TextPointer _tn1 = null;
        TextPointer _tn2 = null;
        int _eventCount=0;   
        #endregion Members

        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            bool testRes;
            this.TextTree = new TextContainer();
            this.TextTree.Changed += new TextContainerChangedEventHandler(OnTextChanged);

            Log("Step1: Add Text into empty TextContainer");
            TextTree.InsertText(this.TextTree.Start, TextToInsertStep1);
            VerifyEventCount(1);
            
            Log("Step2: Add some more text (on an non-empty TextContainer)");
            TextTree.InsertText(this.TextTree.End, TextToInsertStep2);
            VerifyEventCount(2);

            _elementType = ReflectionUtils.FindType(TextElementTypeName);
            object elementObject1 = ReflectionUtils.CreateInstance(_elementType);
            
            int tn1Location = (TextToInsertStep1.Length + TextToInsertStep2.Length)/2;
            _tn1 = this.TextTree.Start.CreateNavigator(tn1Location);
            _tn2 = this.TextTree.End.CreateNavigator(); 

            //Adding a textelement fires 3 textchanged events. This is done for a workaround for now.
            //A task item is added to clean this up. Refer to TaskItem#624 for more info.
            Log("Step3: Add a textelement");
            TextTree.InsertElement(_tn1, _tn2, (TextElement)elementObject1);
            VerifyEventCount(3);

            _objectType = ReflectionUtils.FindType(EmbeddedObjectTypeName);
            object embeddedObject1 = ReflectionUtils.CreateInstance(_objectType);

            Log("Step4: Add an embedded object");
            testRes = this.TextTree.CanInsertEmbeddedObject(_objectType);
            Verifier.Verify(testRes, "Verifying TextContainer.CanInsertEmbeddedObject for Step4...", true);
            TextTree.InsertEmbeddedObject(_tn1, embeddedObject1);
            VerifyEventCount(4);

            object embeddedObject2 = ReflectionUtils.CreateInstance(_objectType);

            Log("Step5: Add one more textObject");
            testRes = this.TextTree.CanInsertEmbeddedObject(_objectType);
            Verifier.Verify(testRes, "Verifying TextContainer.CanInsertEmbeddedObject for Step5...", true);
            TextTree.InsertEmbeddedObject(_tn1, embeddedObject2);
            VerifyEventCount(5);

            _tn2=_tn1.CreateNavigator(-2);
            
            Log("Step6: Delete some text");
            TextTree.DeleteContent(_tn2, _tn1);
            VerifyEventCount(6);

            Log("Step7: Delete textelement");
            TextTree.ExtractElement((TextElement)elementObject1);
            VerifyEventCount(7);

            Log("Step8: Delete the embedded object");
            TextTree.DeleteEmbeddedObject(_tn1, LogicalDirection.Forward);
            VerifyEventCount(8);

            Log("Step9: Delete an embedded object and text together");
            TextTree.DeleteContent(_tn1, this.TextTree.End);
            VerifyEventCount(9);

            Log("Step10: Re-insert textelement and then change a property of the element");
            TextTree.InsertElement(this.TextTree.Start, _tn1, (TextElement)elementObject1);
            VerifyEventCount(10);
            
            Log("Step11: Change FontSize property of the element directly through the element");
            ((TextElement)elementObject1).FontSize = 15.0;
            VerifyEventCount(11);
            
            Log("Step12: Change FontSize property of the element through TextContainer");
            this.TextTree.SetValue(_tn1, TextElement.FontSizeProperty, 20.0);
            VerifyEventCount(12);
            
            Log("Step13: Call deletecontent using the same textposition for both start and end textpositions");
            TextTree.DeleteContent(_tn1, _tn1);
            VerifyEventCount(12);

            Log("Step14: Call deletecontent with both start and end textpositions placed at same position.");
            TextTree.DeleteContent(_tn2, _tn1);
            VerifyEventCount(12);

            Log("Step15: Remove the TextChanged event and do some insertion/deletion operations");
            this.TextTree.Changed -= (OnTextChanged);
            _tn1.MoveByDistance(1);
            TextTree.DeleteContent(this.TextTree.Start, _tn1);

            TextTree.DeleteContent(this.TextTree.Start, this.TextTree.End);
            VerifyEventCount(12);

            Logger.Current.ReportSuccess();
        }

        /// <summary>
        /// OnTextChanged event handler
        /// </summary>
        /// <param name="sender">object which raised the event</param>
        /// <param name="args">arguments</param>
        public void OnTextChanged(object sender, TextContainerChangedEventArgs args)
        {
            _eventCount++;
            Log("OnTextChanged event is called. EventCount = " + _eventCount);
        }
        
        /// <summary>
        /// Helper function to check the event count raised till now with the expected count
        /// </summary>
        /// <param name="expectedCount">expected event count</param>
        public void VerifyEventCount(int expectedCount)
        {
            Log("Expected EventCount: [" + expectedCount + "] Actual EventCount: [" + _eventCount + "]");
            Verifier.Verify((expectedCount==_eventCount), "Verifying the TextChanged EventCount...", true);
        }
    }

    // Command Line: /TestCaseType:TextTreeTest_4kBlockTest 
    /// <summary>
    /// TextContainer stores the data in blocks of 4K size. This test is to check the boundary condition of 4k.
    /// </summary>
    [TestOwner("Microsoft"), 
    WindowlessTest(true), 
    TestTactics("383")]
    public class TextTreeTest_4kBlockTest : TextTreeTestBase
    {
        #region Settings
        #endregion Settings
    
        #region Members
        TextPointer _tn1 = null;
        #endregion Members
    
        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            bool testRes;

            System.Text.StringBuilder checkString;
            string testString;
            this.TextTree = new TextContainer();

            Log("Step1: Insert 4K bytes of data into TextContainer");
            for (int i = 0; i < 4096; i++)
            {
                this.TextTree.InsertText(this.TextTree.Start, "c");
            }
            testRes = (4096==this.TextTree.Start.GetTextRunLength(LogicalDirection.Forward))&&(4096==this.TextTree.End.GetTextLength(LogicalDirection.Backward));
            Verifier.Verify(testRes, "Verifying the text length inside TextContainer for Step1...", true);

            checkString = new System.Text.StringBuilder();
            checkString.Append('c', 4096);
            testString = this.TextTree.Start.GetTextInRun(LogicalDirection.Forward);
            Verifier.Verify((checkString.ToString()==testString), "Verifying the textcontents of the TextContainer for Step1...", true);

            Log("Step2: Insert one more character at the end of TextContainer");
            this.TextTree.InsertText(this.TextTree.End, "e");
            testRes = (4097==this.TextTree.Start.GetTextRunLength(LogicalDirection.Forward))&&(4097==this.TextTree.End.GetTextLength(LogicalDirection.Backward));
            Verifier.Verify(testRes, "Verifying the text length after inserting an extra character for Step2...", true);

            char[] testArray = new Char[2];
            int returnVal = this.TextTree.End.GetTextInRun(LogicalDirection.Backward, 2, null, testArray, 0);
            testRes= (testArray[0]=='c')&&(testArray[1]=='e');
            Verifier.Verify(testRes, "Verifying the last two characters in TextContainer for Step2...", true);

            Log("Step3: Delete the last two characters in the TextContainer");
            _tn1 = this.TextTree.End.CreateNavigator(-2);
            this.TextTree.DeleteContent(_tn1, this.TextTree.End);
            testRes = (4095 == this.TextTree.Start.GetTextRunLength(LogicalDirection.Forward)) && (4095 == this.TextTree.End.GetTextLength(LogicalDirection.Backward));
            Verifier.Verify(testRes, "Verifying the text length after deleting last two characters for Step3...", true);

            Log("Step4: Insert an element into TextContainer");
            this.TextTree.InsertElement(this.TextTree.Start, _tn1, typeof(Inline));

            Log("Step5: Insert an embedded object into TextContainer");
            Button button1 = new Button();
            testRes = this.TextTree.CanInsertEmbeddedObject(typeof(Button));
            Verifier.Verify(testRes, "Verifying CanInsertEmbeddedObject for Step5...", true);
            this.TextTree.InsertEmbeddedObject(_tn1, button1);

            Log("Step6: Delete all contents and then insert three 4K bytes of data into TextContainer");
            this.TextTree.DeleteContent(this.TextTree.Start, this.TextTree.End);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 1024; j++)
                {
                    this.TextTree.InsertText(this.TextTree.Start, "abc");
                }
            }
            testRes = (12288 == this.TextTree.Start.GetTextRunLength(LogicalDirection.Forward)) && (12288 == this.TextTree.End.GetTextLength(LogicalDirection.Backward));
            Verifier.Verify(testRes, "Verifying the text length inside TextContainer for Step6...", true);

            checkString.Remove(0, checkString.Length);
            for (int i = 0; i < 4096; i++)
            {
                checkString.Append("abc");
            }
            testString = this.TextTree.Start.GetTextInRun(LogicalDirection.Forward);
            Verifier.Verify((checkString.ToString() == testString), "Verifying the textcontents of the TextContainer for Step6...", true);

            Logger.Current.ReportSuccess();
        }
    }

    // Command Line: /TestCaseType:TextTreeTest_PropertyInheritanceWithRoot /TestName:TextTreeTest_PropertyInheritanceWithRoot-Simple /xml:testxml.xml
    /// <summary>
    /// Verify property inheritance is inplace when you create a TextContainer with a root.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("RootTypeName", "Name of the type of object to be used as root for the TextContainer"), 
    TestArgument("TextElement1TypeName", "Name of the type of textElement1 to be tested"), 
    TestArgument("TextElement2TypeName", "Name of the type of textElement2 to be tested"),
    TestArgument("TextElement3TypeName", "Name of the type of textElement3 to be tested"),
    TestArgument("PropertyName", "Name of the property that has to be checked for inheritance"), 
    TestArgument("PropertyValue", "Value of the property to be assigned in the test"),
    TestArgument("SecondPropertyValue", "Another Value for the property to be assigned explicitly to TextElement in the test")]
    public class TextTreeTest_PropertyInheritanceWithRoot : TextTreeTestBase
    {
        #region Settings
        /// <summary>Name of the type of object to be used as root for the TextContainer</summary>
        private string RootTypeName
        {
            get { return Settings.GetArgument("RootTypeName"); }
        }

        /// <summary>Name of the type of UIElement object to be tested</summary>
        private string UIElementTypeName
        {
            get { return Settings.GetArgument("UIElementTypeName"); }
        }

        /// <summary>Name of the type of textElement1 to be tested</summary>
        private string TextElement1TypeName
        {
            get { return Settings.GetArgument("TextElement1TypeName"); }
        }

        /// <summary>Name of the type of textElement2 to be tested</summary>
        private string TextElement2TypeName
        {
            get { return Settings.GetArgument("TextElement2TypeName"); }
        }

        /// <summary>Name of the type of textElement3 to be tested</summary>
        private string TextElement3TypeName
        {
            get { return Settings.GetArgument("TextElement3TypeName"); }
        }

        /// <summary>Name of the property that has to be checked for inheritance</summary>
        private string PropertyName
        {
            get { return Settings.GetArgument("PropertyName"); }
        }

        /// <summary>Value of the property to be assigned in the test</summary>
        private string PropertyValue
        {
            get { return Settings.GetArgument("PropertyValue"); }
        }

        /// <summary>Another Value for the property to be assigned explicitly to TextElement in the test</summary>
        private string SecondPropertyValue
        {
            get { return Settings.GetArgument("SecondPropertyValue"); }
        }

        #endregion Settings
    
        #region Members
        TextPointer _tn1 = null;
        #endregion Members
    
        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            object testPropertyValue;

            Type rootType = ReflectionUtils.FindType(RootTypeName);
            Object root = ReflectionUtils.CreateInstance(rootType);

            ReflectionUtils.SetProperty(root, PropertyName, PropertyValue);

            this.TextTree = new TextContainer((DependencyObject)root);

            Type element1Type = ReflectionUtils.FindType(TextElement1TypeName);
            Object element1 = ReflectionUtils.CreateInstance(element1Type);

            Log("Step1: One level inheritance test from root of TextContainer...");
            this.TextTree.InsertElement(TextTree.Start, TextTree.End, (TextElement)element1);
            testPropertyValue = ReflectionUtils.GetProperty(element1, PropertyName);
            Verifier.Verify( ((string)testPropertyValue == PropertyValue), "Verifying that " + PropertyName + " is inherited in Step1...", true);

            Type element2Type = ReflectionUtils.FindType(TextElement2TypeName);
            Object element2 = ReflectionUtils.CreateInstance(element2Type);
            
            _tn1 = this.TextTree.End.CreateNavigator(-1);

            Log("Step2: Two level inheritance test from root of TextContainer...");
            this.TextTree.InsertElement(_tn1, _tn1, (TextElement)element2);
            testPropertyValue = ReflectionUtils.GetProperty(element2, PropertyName);
            Verifier.Verify(((string)testPropertyValue == PropertyValue), "Verifying that " + PropertyName + " is inherited in Step2...", true);

            Type uiElementType = ReflectionUtils.FindType(UIElementTypeName);
            Object uiElement1 = ReflectionUtils.CreateInstance(uiElementType);

            Log("Step3: Property inheritance on an embedded object in TextContainer...");
            this.TextTree.InsertEmbeddedObject(this.TextTree.Start, uiElement1);
            testPropertyValue = ReflectionUtils.GetProperty(uiElement1, PropertyName);
            Verifier.Verify(((string)testPropertyValue == PropertyValue), "Verifying that " + PropertyName + " is inherited in Step3...", true);

            Type element3Type = ReflectionUtils.FindType(TextElement3TypeName);
            Object element3 = ReflectionUtils.CreateInstance(element3Type);
            this.TextTree.InsertElement(this.TextTree.End, this.TextTree.End, (TextElement)element3);

            Log("Step4: Changing the property value of Element1 explicitly should reflect in Element2 but not in Element3...");
            ReflectionUtils.SetProperty((TextElement)element1, PropertyName, SecondPropertyValue);
            testPropertyValue = ReflectionUtils.GetProperty(element2, PropertyName);
            Verifier.Verify(((string)testPropertyValue == SecondPropertyValue), "Verifying that " + PropertyName + " is inherited to Element2 in Step4...", true);
            testPropertyValue = ReflectionUtils.GetProperty(element3, PropertyName);
            Verifier.Verify(((string)testPropertyValue == PropertyValue), "Verifying that " + PropertyName + " didnt change for Element3 in Step4...", true);

            Logger.Current.ReportSuccess();
        }
    }

    // Command Line: /TestCaseType:TextTreeTest_PropertyInheritanceWithoutRoot /TestName:TextTreeTest_PropertyInheritanceWithoutRoot-Simple /xml:testxml.xml
    /// <summary>
    /// Verify property inheritance is inplace when you create a TextContainer without a root.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestArgument("TextElement1TypeName", "Name of the type of textElement1 to be tested"), 
    TestArgument("TextElement2TypeName", "Name of the type of textElement2 to be tested"), 
    TestArgument("TextElement3TypeName", "Name of the type of textElement3 to be tested"), 
    TestArgument("PropertyName", "Name of the property that has to be checked for inheritance"), 
    TestArgument("PropertyValue", "Value of the property to be assigned in the test"),
    TestArgument("SecondPropertyValue", "Another Value for the property to be assigned explicitly to TextElement in the test")]
    public class TextTreeTest_PropertyInheritanceWithoutRoot : TextTreeTestBase
    {
        #region Settings
        /// <summary>Name of the type of UIElement object to be tested</summary>
        private string UIElementTypeName
        {
            get { return Settings.GetArgument("UIElementTypeName"); }
        }

        /// <summary>Name of the type of textElement1 to be tested</summary>
        private string TextElement1TypeName
        {
            get { return Settings.GetArgument("TextElement1TypeName"); }
        }

        /// <summary>Name of the type of textElement2 to be tested</summary>
        private string TextElement2TypeName
        {
            get { return Settings.GetArgument("TextElement2TypeName"); }
        }

        /// <summary>Name of the type of textElement3 to be tested</summary>
        private string TextElement3TypeName
        {
            get { return Settings.GetArgument("TextElement3TypeName"); }
        }

        /// <summary>Name of the property that has to be checked for inheritance</summary>
        private string PropertyName
        {
            get { return Settings.GetArgument("PropertyName"); }
        }

        /// <summary>Value of the property to be assigned in the test</summary>
        private string PropertyValue
        {
            get { return Settings.GetArgument("PropertyValue"); }
        }

        /// <summary>Another Value for the property to be assigned explicitly to TextElement in the test</summary>
        private string SecondPropertyValue
        {
            get { return Settings.GetArgument("SecondPropertyValue"); }
        }

        #endregion Settings
    
        #region Members
        #endregion Members
    
        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            object testPropertyValue;

            this.TextTree = new TextContainer();

            Type element1Type = ReflectionUtils.FindType(TextElement1TypeName);
            Object element1 = ReflectionUtils.CreateInstance(element1Type);
            ReflectionUtils.SetProperty(element1, PropertyName, PropertyValue);

            this.TextTree.InsertElement(TextTree.Start, TextTree.End, (TextElement)element1);

            Type element2Type = ReflectionUtils.FindType(TextElement2TypeName);
            Object element2 = ReflectionUtils.CreateInstance(element2Type);

            Log("Step1: One level inheritance test from an element in TextContainer...");
            ((TextElement)element1).Append((TextElement)element2);
            testPropertyValue = ReflectionUtils.GetProperty(element2, PropertyName);
            Verifier.Verify(((string)testPropertyValue == PropertyValue), "Verifying that " + PropertyName + " is inherited in Step1...", true);

            Type element3Type = ReflectionUtils.FindType(TextElement3TypeName);
            Object element3 = ReflectionUtils.CreateInstance(element3Type);

            Log("Step2: Two level inheritance test from an element in TextContainer...");
            ((TextElement)element2).Append((TextElement)element3);
            testPropertyValue = ReflectionUtils.GetProperty(element3, PropertyName);
            Verifier.Verify(((string)testPropertyValue == PropertyValue), "Verifying that " + PropertyName + " is inherited in Step2...", true);

            Type uiElementType = ReflectionUtils.FindType(UIElementTypeName);
            Object uiElement1 = ReflectionUtils.CreateInstance(uiElementType);

            Log("Step3: Property inheritance on an embedded object in TextContainer...");
            ((TextElement)element2).Append((UIElement)uiElement1);
            testPropertyValue = ReflectionUtils.GetProperty(uiElement1, PropertyName);
            Verifier.Verify(((string)testPropertyValue == PropertyValue), "Verifying that " + PropertyName + " is inherited in Step3...", true);

            Log("Step4: Changing the property value of Element2 explicitly should reflect in Element3 but not in Element1...");
            ReflectionUtils.SetProperty(element2, PropertyName, SecondPropertyValue);
            testPropertyValue = ReflectionUtils.GetProperty(element3, PropertyName);
            Verifier.Verify(((string)testPropertyValue == SecondPropertyValue), "Verifying that " + PropertyName + " is inherited to Element3 in Step4...", true);
            testPropertyValue = ReflectionUtils.GetProperty(element1, PropertyName);
            Verifier.Verify(((string)testPropertyValue == PropertyValue), "Verifying that " + PropertyName + " didnt change for Element1 in Step4...", true);

            Logger.Current.ReportSuccess();
        }
    }

    // Command Line: /TestCaseType:TextTreeTest_InsertingElementInstances /TestName:TextTreeTest_InsertingElementInstances-Simple /xml:testxml.xml
    /// <summary>
    /// Inserts already created instances of TextElements with some text into TextContainer. Also tests inserting
    /// TextElements/EmbeddedObjects which were previously inserted and removed in a different TextContainer.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestBugs("610"),
    TestArgument("TextElement1TypeName", "Name of the type of textElement1 to be tested"), 
    TestArgument("TextElement2TypeName", "Name of the type of textElement2 to be tested"), 
    TestArgument("EmbeddedObjectTypeName", "Name of the type of EmbeddedObject to be tested"), 
    TestArgument("TextElement1Text", "Text to be inserted inside textElement1"), 
    TestArgument("TextElement2Text", "Text to be inserted inside textElement2")]
    public class TextTreeTest_InsertingElementInstances : TextTreeTestBase
    {
        #region Settings
        /// <summary>Name of the type of UIElement object to be tested</summary>
        private string EmbeddedObjectTypeName
        {
            get { return Settings.GetArgument("EmbeddedObjectTypeName"); }
        }

        /// <summary>Name of the type of textElement1 to be tested</summary>
        private string TextElement1TypeName
        {
            get { return Settings.GetArgument("TextElement1TypeName"); }
        }

        /// <summary>Name of the type of textElement2 to be tested</summary>
        private string TextElement2TypeName
        {
            get { return Settings.GetArgument("TextElement2TypeName"); }
        }

        /// <summary>Text to be inserted inside textElement1</summary>
        private string TextElement1Text
        {
            get { return Settings.GetArgument("TextElement1Text"); }
        }

        /// <summary>Text to be inserted inside textElement2</summary>
        private string TextElement2Text
        {
            get { return Settings.GetArgument("TextElement2Text"); }
        }

        #endregion Settings
    
        #region Members
        #endregion Members
    
        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            this.TextTree = new TextContainer();
            TextBox textBox = new TextBox();
            TextContainer textBoxTextTree = (TextContainer)(textBox.StartPosition.TextContainer);
            textBoxTextTree.InsertText(textBoxTextTree.Start, "This is a TextBox");

            Type element1Type = ReflectionUtils.FindType(TextElement1TypeName);
            Object element1 = ReflectionUtils.CreateInstance(element1Type);
            ((TextElement)element1).Text = TextElement1Text;

            Type element2Type = ReflectionUtils.FindType(TextElement2TypeName);
            Object element2 = ReflectionUtils.CreateInstance(element2Type);
            ((TextElement)element2).Text = TextElement2Text;

            Type embeddedObjectType = ReflectionUtils.FindType(EmbeddedObjectTypeName);
            Object embeddedObject = ReflectionUtils.CreateInstance(embeddedObjectType);

            // Regression_Bug610.
            ((TextElement)element2).Append((UIElement)embeddedObject);
            ((TextElement)element1).Append((TextElement)element2);

            Log("Step1: Inserting an instance of TextElement with text and elements inside it...");
            this.TextTree.InsertElement(this.TextTree.Start, this.TextTree.End, (TextElement)element1);

            Log("Step2: Extracting an element from one TextContainer and then inserting it into another TextContainer...");
            this.TextTree.ExtractElement((TextElement)element1);
            textBoxTextTree.InsertElement(textBoxTextTree.Start, textBoxTextTree.End, (TextElement)element1);

            Verifier.Verify((this.TextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.Text), "Verifying that the element is removed from the 1st tree...", true);
            Verifier.Verify((textBoxTextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.ElementStart), "Verifying that the element is inserted into the 2nd tree...", true);
            Verifier.Verify((((TextElement)element1).TextContainer == textBoxTextTree), "Verifying that element belongs to the new tree...", true);

            Logger.Current.ReportSuccess();
        }
    }

    // Command Line: /TestCaseType:TextTreeTest_OnParentChanged
    /// <summary>
    /// This test case tests whether the OnParentChanged event gets fired for TextElements.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestTactics("382"),
    TestBugs("611")]
    public class TextTreeTest_OnParentChanged : TextTreeTestBase
    {
        #region Settings
        #endregion Settings
    
        #region Members
        #endregion Members
    
        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            FrameworkContentElement root1 = new FrameworkContentElement();

            this.TextTree = new TextContainer(root1);

            MyTextElement element1 = new MyTextElement();

            Log("Step1: Insert a TextElement into TextContainer.");
            this.TextTree.InsertElement(this.TextTree.Start, this.TextTree.End, element1);
            VerifyParentChanged(element1.HasParentChanged, true);

            Log("Step2: Create a TextElement and assign some text to it.");
            MyTextElement element2 = new MyTextElement();
            element2.Text = "Element2";
            // Adding a child to a TextElement will not change its parent
            VerifyParentChanged(element2.HasParentChanged, false);

            TextPointer tn1 = this.TextTree.Start.CreateNavigator(1);

            Log("Step3: Insert another TextElement into TextContainer using a TextPointer.");
            this.TextTree.InsertElement(tn1, tn1, element2);
            VerifyParentChanged(element2.HasParentChanged, true);

            MyTextElement element3 = new MyTextElement();

            Log("Step4: Insert a TextElement into TextContainer using the append function of TextElement.");
            element2.Append(element3);
            VerifyParentChanged(element3.HasParentChanged, true);

            Log("Step5: Extract a TextElement from TextContainer.");
            this.TextTree.ExtractElement(element3);
            VerifyParentChanged(element3.HasParentChanged, true);
            //The expected CallCount should be 5. This is Regression_Bug611. Presently OnParentChanged is not
            //getting called in the ExtractElement call.

            TextContainer newTextTree = new TextContainer();

            Log("Step6: Insert an extracted TextElement into another TextContainer.");
            newTextTree.InsertElement(newTextTree.Start, newTextTree.End, element3);
            // Adding to a tree with no parent does not change the parent
            VerifyParentChanged(element3.HasParentChanged, false);

            MyFrameworkElement frameworkElement1 = new MyFrameworkElement();

            Log("Step7: Insert an embedded object into TextContainer.");
            newTextTree.InsertEmbeddedObject(newTextTree.Start, frameworkElement1);
            // newTextTree still has no parent, so frameworkElement1's parent will not change
            VerifyParentChanged(frameworkElement1.HasParentChanged, false);

            Log("Step8: Delete an embedded object from TextContainer.");
            newTextTree.DeleteEmbeddedObject(newTextTree.Start, LogicalDirection.Forward);
            // newTextTree still has no parent, so frameworkElement1's parent will not change
            VerifyParentChanged(frameworkElement1.HasParentChanged, false);

            Log("Step9: Insert an embedded object which was previously inserted and then removed from a TextContainer into another TextContainer.");
            this.TextTree.InsertEmbeddedObject(this.TextTree.Start, frameworkElement1);
            VerifyParentChanged(frameworkElement1.HasParentChanged, true);

            MyFrameworkElement frameworkElement2 = new MyFrameworkElement();
            Log("Step10: Append an embedded object inside a TextElement");
            element1.Append(frameworkElement2);
            VerifyParentChanged(frameworkElement2.HasParentChanged, true);

            Logger.Current.ReportSuccess();
        }

        /// <summary>
        /// Helper function to check the OnParentChanged call count with the expected count
        /// </summary>
        /// <param name="actualCount">actual call count for OnParentChanged</param>
        /// <param name="expectedCount">expected call count for OnParentChanged</param>
        public void VerifyCallCount(int actualCount, int expectedCount)
        {
            Log("Expected CallCount for OnParentChanged:[" + expectedCount + "] Actual CallCount:[" + actualCount + "]");
            Verifier.Verify((expectedCount == actualCount), "Verifying the OnParentChanged CallCount...", true);
        }

        /// <summary>
        /// Helper function to check the OnParentChanged call count with the expected count
        /// </summary>
        /// <param name="expectedParentChanged">actual call count for OnParentChanged</param>
        /// <param name="actualParentChanged">expected call count for OnParentChanged</param>
        public void VerifyParentChanged(bool expectedParentChanged, bool actualParentChanged)
        {
            Log("Expected ParentChanged:[" + expectedParentChanged + "] Actual ParentChanged:[" + actualParentChanged + "]");
            Verifier.Verify((expectedParentChanged == actualParentChanged), "Verifying parent changed...", true);
        }

    }

//    // Command Line: /TestCaseType:TextTreeTest_UndoRedo /TestName:TextTreeTest_UndoRedo-Simple /xml:testxml.xml
//    /// <summary>
//    /// This test case tests the functionality of UndoRedo in TextContainer.
//    /// </summary>
//    [TestOwner("Microsoft"),
//    WindowlessTest(true),
//    TestBugs("7566, 757"),
//    TestArgument("TestText", "Text to be used in Step1"),
//    TestArgument("TextElement1TypeName", "Name of the type of textElement1 to be tested"), 
//    TestArgument("EmbeddedObjectTypeName", "Name of the type of EmbeddedObject to be tested"),
//    TestArgument("TextElement2TypeName", "Name of the type of textElement2 to be tested"), 
//    TestArgument("TextElement1Text", "Text to be inserted inside textElement1"), 
//    TestArgument("TextElement2Text", "Text to be inserted inside textElement2")]
//    public class TextTreeTest_UndoRedo : TextTreeTestBase
//    {
//        #region Settings
//        /// <summary>Text to be used in Step1</summary>
//        private string TestText
//        {
//            get { return Settings.GetArgument("TestText"); }
//        }
//
//        /// <summary>Name of the type of UIElement object to be tested</summary>
//        private string EmbeddedObjectTypeName
//        {
//            get { return Settings.GetArgument("EmbeddedObjectTypeName"); }
//        }
//
//        /// <summary>Name of the type of textElement1 to be tested</summary>
//        private string TextElement1TypeName
//        {
//            get { return Settings.GetArgument("TextElement1TypeName"); }
//        }
//        #endregion Settings
//
//        #region Members
//        TextPointer tn1 = null;
//
//        TextPointer tn2 = null;
//        #endregion Members
//
//        /// <summary>
//        /// Runs the test case
//        /// </summary>
//        public override void RunTestCase()
//        {
//            UndoManager undoService;
//            FrameworkContentElement root;
//            
//            root = new FrameworkContentElement();
//            this.TextTree = new TextContainer(root);
//            undoService = new UndoManager();
//            
//            // Set up the UndoManager.
//            UndoManager.AttachUndoManager(root, undoService);
//
//            //Verify Undo/Redo operations on InsertText method
//            #region Step1
//            Log("Step1: Insert some text into TextContainer and verify undo/redo");
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertText(this.TextTree.Start, TestText);
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 1, 0);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText), "Verifying the contents of the TextContainer for Step1...", true);
//
//            undoService.Undo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 0, 1);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == ""), "Verifying the contents of the TextContainer for Step1...", true);
//
//            undoService.Redo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 1, 0);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText), "Verifying the contents of the TextContainer for Step1...", true);
//            #endregion Step1
//
//            int tn1OffSet = TestText.Length / 2;
//            tn1 = this.TextTree.Start.CreateNavigator(tn1OffSet);
//
//            //Verify Undo/Redo operations on DeleteContent (TextBlock) method
//            #region Step2
//            Log("Step2: Delete some text from TextContainer and verify undo/redo");
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.DeleteContent(tn1, this.TextTree.End);
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 2, 0);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of the TextContainer for Step2...", true);
//
//            undoService.Undo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 1, 1);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText), "Verifying the contents of the TextContainer for Step2...", true);
//
//            undoService.Redo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 2, 0);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of the TextContainer for Step2...", true);
//            #endregion Step2
//
//            Type element1Type = ReflectionUtils.FindType(TextElement1TypeName);
//            Object element1 = ReflectionUtils.CreateInstance(element1Type);
//
//            //Verify Undo/Redo operations on InsertElement method.
//            #region Step3
//            Log("Step3: Insert a TextElement and verify undo/redo");
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertElement(this.TextTree.Start, this.TextTree.End, (TextElement)element1);
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 3, 0);
//            Verifier.Verify((this.TextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.ElementStart), "Verifying the contents at the Start of TextContainer for Step3...", true);
//
//            undoService.Undo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 2, 1);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of TextContainer for Step3...", true);
//
//            undoService.Redo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 3, 0);
//            Verifier.Verify((this.TextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.ElementStart), "Verifying the contents at the Start of TextContainer for Step3...", true);
//            #endregion Step3
//
//            //Verify Undo/Redo operations on ExtractElement method.
//            #region Step4
//            Log("Step4: Extract the TextElement and verify undo/redo");
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.ExtractElement(tn1);
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 4, 0);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of the TextContainer for Step4...", true);
//
//            undoService.Undo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 3, 1);
//            Verifier.Verify((this.TextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.ElementStart), "Verifying the contents at the Start of TextContainer for Step4...", true);
//
//            undoService.Redo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 4, 0);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of the TextContainer for Step4...", true);
//            #endregion Step4
//
//            Type embeddedObjectType = ReflectionUtils.FindType(EmbeddedObjectTypeName);
//            Object embeddedObject1 = ReflectionUtils.CreateInstance(embeddedObjectType);
//
//            //Verify Undo/Redo operations on InsertEmbeddedObject method.
//            #region Step5
//            Log("Step5: Insert a EmbeddedObject and verify undo/redo");
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertEmbeddedObject(this.TextTree.Start, embeddedObject1);
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 5, 0);
//            Verifier.Verify((this.TextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement), "Verifying the contents at the Start of TextContainer for Step5...", true);
//
//            undoService.Undo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 4, 1);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of TextContainer for Step5...", true);
//
//            undoService.Redo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 5, 0);
//            Verifier.Verify((this.TextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement), "Verifying the contents at the Start of TextContainer for Step5...", true);
//            #endregion Step5
//
//            //Verify Undo/Redo operations on DeleteEmbeddedObject method.
//            #region Step6
//            Log("Step6: Delete the EmbeddedObject and verify undo/redo");
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.DeleteEmbeddedObject(this.TextTree.Start, LogicalDirection.Forward);
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 6, 0);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of the TextContainer for Step6...", true);
//
//            undoService.Undo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 5, 1);
//            Verifier.Verify((this.TextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement), "Verifying the contents at the Start of TextContainer for Step6...", true);
//
//            undoService.Redo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 6, 0);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of the TextContainer for Step6...", true);
//
//            undoService.Undo(6);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 0, 6);
//            Verifier.Verify((this.TextTree.Start.GetTextInRun(LogicalDirection.Forward) == ""), "Verifying the contents of the TextContainer for Step6...", true);
//            #endregion Step6
//
//            //Verify Undo/Redo operations after various (more than one) insertion/deletion operations are done on the TextContainer
//            #region Step7
//            Log("Step7: Perform multiple operations and verify undo/redo");
//
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertText(this.TextTree.Start, TestText);
//            undoService.Close(true);
//
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertEmbeddedObject(this.TextTree.End, embeddedObject1);
//            undoService.Close(true);
//
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertElement(this.TextTree.Start, this.TextTree.End, (TextElement)element1);
//            undoService.Close(true);
//
//            tn1 = this.TextTree.Start.CreateNavigator(tn1OffSet + 1);
//            tn2 = this.TextTree.End.CreateNavigator(-1);
//
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.DeleteContent(tn1, tn2); //" a test<embeddedObject1>" is deleted
//            undoService.Close(true);
//
//            Object embeddedObject2 = ReflectionUtils.CreateInstance(embeddedObjectType);
//
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertEmbeddedObject(this.TextTree.Start, embeddedObject2);
//            undoService.Close(true);
//
//            // Verify Undo/Redo over the multiple operations done above.
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 5, 0);
//            Verifier.Verify((this.TextTree.Start.GetSymbolType(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement), "Verifying the contents at the Start of TextContainer for Step7a...", true);
//
//            undoService.Undo(2);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 3, 2);
//            Verifier.Verify((tn2.GetSymbolType(LogicalDirection.Backward) == TextPointerContext.EmbeddedElement), "Verifying the undo operation for Step7...", true);
//            tn1 = this.TextTree.Start.CreateNavigator(1);
//            Log("Expected test: [" + TestText + "]");
//            Log("Actual test: [" + tn1.GetTextInRun(LogicalDirection.Forward) + "]");
//            Verifier.Verify((tn1.GetTextInRun(LogicalDirection.Forward) == TestText), "Verifying the contents of the TextContainer for Step7b...", true);
//
//            undoService.Redo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 4, 1);
//            Verifier.Verify((tn2.GetSymbolType(LogicalDirection.Backward) == TextPointerContext.Text), "Verifying the redo operation for Step7...", true);
//            Verifier.Verify((tn1.GetTextInRun(LogicalDirection.Forward) == TestText.Substring(0, tn1OffSet)), "Verifying the contents of the TextContainer for Step7c...", true);
//
//            undoService.Undo(3);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 1, 4);
//            Verifier.Verify((tn1.GetTextInRun(LogicalDirection.Forward) == TestText), "Verifying the contents of the TextContainer for Step7d...", true);
//
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertText(this.TextTree.Start, TestText);
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 2, 0);
//            Verifier.Verify((tn1.GetTextInRun(LogicalDirection.Forward) == (TestText + TestText)), "Verifying the contents of the TextContainer for Step7e...", true);
//            #endregion Step7
//
//            //Verify Undo/Redo operations on some bogus operation like insert nothing, delete nothing.
//            #region Step8
//            Log("Step8: Verify Undo/Redo operations on some bogus operation like insert nothing, delete nothing.");
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertText(this.TextTree.End, "");
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 3, 0);
//            Verifier.Verify((this.TextTree.End.GetTextInRun(LogicalDirection.Backward) == (TestText + TestText)), "Verifying the contents of the TextContainer for Step8...", true);
//
//            undoService.Undo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 2, 1);
//            Verifier.Verify((this.TextTree.End.GetTextInRun(LogicalDirection.Backward) == (TestText + TestText)), "Verifying the contents of the TextContainer for Step8...", true);
//
//            undoService.Redo(1);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 3, 0);
//            Verifier.Verify((this.TextTree.End.GetTextInRun(LogicalDirection.Backward) == (TestText + TestText)), "Verifying the contents of the TextContainer for Step8...", true);
//             
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.DeleteContent(this.TextTree.End, this.TextTree.End);
//            undoService.Close(true);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 4, 0);
//            
//            #endregion Step8
//
//            //Check to see if undo and redo stack are cleared when no ParentUndoUnit is opened.
//            #region Step9
//            Log("Step9: Check to see if undo and redo stack are cleared when no ParentUndoUnit is opened.");
//            undoService.Undo(1);
//            this.TextTree.InsertText(this.TextTree.Start, TestText);
//            VerifyUndoRedoCount(undoService.UndoCount, undoService.RedoCount, 0, 0);
//            #endregion Step9
//
//            Logger.Current.ReportSuccess();
//        }
//
//        /// <summary>
//        /// Helper function to Verify the undo and redo count with their respective expected counts
//        /// </summary>
//        /// <param name="undoCount">actual undo count</param>
//        /// <param name="redoCount">actual redo count</param>
//        /// <param name="expUndoCount">expected undo count</param>
//        /// <param name="expRedoCount">expected redo count</param>
//        public void VerifyUndoRedoCount(int undoCount, int redoCount, int expUndoCount, int expRedoCount)
//        {
//            Log("Expected Undo Count:[" + expUndoCount + "] Actual UndoCount:[" + undoCount + "]");
//            Log("Expected Redo Count:[" + expRedoCount + "] Actual RedoCount:[" + redoCount + "]");
//            Verifier.Verify(((expUndoCount == undoCount) && (expRedoCount == redoCount)), "Verifying the undo and redo counts...", true);
//        }
//    }

    // Command Line: /TestCaseType:TextTreeTest_ReproRegression_Bug321
    /// <summary>
    /// Repro for Regression_Bug321, Regression_Bug314 and Regression_Bug313. Used to get an exception while performing a redo operation (after undo is done)
    /// on insertion of an textelement in a empty TextContainer.
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestBugs("321, 314, 313"),
    TestTactics("381")]
    public class TextTreeTest_ReproRegression_Bug321 : TextTreeTestBase
    {
        #region Settings
        #endregion Settings
    
        #region Members
        #endregion Members
    
        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            Log("Verifying that Regression_Bug321 doesnt repro");
//            UndoManager undoService;
            FrameworkContentElement root;
            
            root = new FrameworkContentElement();
            this.TextTree = new TextContainer(root);
//            undoService = new UndoManager();
//            
//            // Set up the UndoManager.
//            UndoManager.AttachUndoManager(root, undoService);
//
//            Inline element1 = new Inline();
//
//            Log("Inserting a textelement in an empty TextContainer");
//            undoService.Open(new ParentUndoUnit());
//            this.TextTree.InsertElement(this.TextTree.Start, this.TextTree.End, element1);
//            undoService.Close(true);
//
//            Log("Calling undo operation");
//            undoService.Undo(1);
//            Verifier.Verify(undoService.UndoCount == 0, "Verifying that after undo is done, its count is zero", true);
//
//            Log("Calling redo operation");
//            undoService.Redo(1);
//            Verifier.Verify(undoService.UndoCount == 1, "Verifying that after redo is done, undo's count is again 1", true);

            //Logger.Current.ReportSuccess();
            Log("Regression_Bug321 didnt repro");
            TestReproRegression_Bug314();
        }

        /// <summary>Verifies that Regression_Bug314 doesnt repro</summary>
        private void TestReproRegression_Bug314()
        {
            Paragraph para1, para2, para3;

            Log("Verifying that Regression_Bug314 doesnt repro");
            this.TextTree.DeleteContent(this.TextTree.Start, this.TextTree.End);

            para1 = new Paragraph();
            para1.Text = "This is para1";
            para2 = new Paragraph();
            para2.Text = "This is para2";
            para3 = new Paragraph();
            para3.Text = "This is para3";

            this.TextTree.InsertElement(this.TextTree.End, this.TextTree.End, para1);
            this.TextTree.InsertElement(this.TextTree.End, this.TextTree.End, para2);
            this.TextTree.InsertElement(this.TextTree.End, this.TextTree.End, para3);

            this.TextTree.DeleteContent(this.TextTree.Start, this.TextTree.End);
            Log("Regression_Bug314 didnt repro");
            TestReproRegression_Bug313();
        }

        /// <summary>Verifies that Regression_Bug313 doesnt repro</summary>
        private void TestReproRegression_Bug313()
        {
            Log("Verifying that Regression_Bug313 doesnt repro");
            this.TextTree.DeleteContent(this.TextTree.Start, this.TextTree.End);

            TextPointer tp1, tp2;
            TextPointer tn1;
            TextElement textElement;
            int i;

            //Append some text
            for (i = 0; i < 200; i++)
            {
                this.TextTree.InsertText(this.TextTree.Start, s_text[i % s_text.Length]);
            }

            //Append TextElement's 
            for (i = 1; i < 200; i++)
            {
                tp1 = this.TextTree.Start.CreatePosition(i, LogicalDirection.Backward);
                tp2 = this.TextTree.End.CreatePosition(-i, LogicalDirection.Forward);
                this.TextTree.InsertElement(tp1, tp2, typeof(Bold));
            }

            //Delete all TextElements.
            tn1 = this.TextTree.Start.CreateNavigator((this.TextTree.Start.GetOffsetToPosition(this.TextTree.End)) / 2);
            textElement = this.TextTree.GetElement(tn1);
            while (textElement != null)
            {
                this.TextTree.ExtractElement(textElement);
                textElement = this.TextTree.GetElement(tn1);
            }

            Log("Regression_Bug313 didnt repro");
            Logger.Current.ReportSuccess();
        }

        private static string[] s_text = {
            "This is a stress test", 
            "Avalon editing features rock!", 
            "UIS feature is great",
            "Avalon is one of the pillars in WinFX"            
        };
    }
    
    /// <summary>
    /// This test case tests some of the API's of TextContainer
    /// 1.SetElementValue()
    /// 2.ClearValue()
    /// 3.SetValue()
    /// </summary>
    [TestOwner("Microsoft"),
    WindowlessTest(true),
    TestBugs(""),
    TestTactics("380")]
    public class TextTreeAPITest : TextTreeTestBase
    {
        #region Settings
        #endregion Settings

        #region Members
        #endregion Members

        /// <summary>
        /// Runs the test case
        /// </summary>
        public override void RunTestCase()
        {
            Test_SetClearValue();
            Logger.Current.ReportSuccess();
        }
        
        private void Test_SetClearValue()
        {
            const string textTreeToString = "TextContainer Id=0 SymbolCount=8";
            const string systemFontFamily = "Arial";
            const string testFontFamily = "Courier";
            const Double systemFontSize = 10;
            const Double testFontSize = 24;

            FrameworkContentElement root;
            TextPointer tpBeforeElement, tpInsideElement, tpAfterElement;

            Log("Testing SetElementValue(), ClearValue() & SetValue() methods");
            root = new FrameworkContentElement();
            this.TextTree = new TextContainer(root,Application.Current.Context);

            Inline inlineElement = new Inline();
            inlineElement.Text = "Test";
            this.TextTree.InsertElement(this.TextTree.Start, this.TextTree.End, inlineElement);

            tpBeforeElement = this.TextTree.Start;
            tpInsideElement = this.TextTree.Start.CreatePosition(3, LogicalDirection.Forward);
            tpAfterElement = this.TextTree.End;

            //Test SetElementValue()
            //Set some values which are not the default ones.            
            /*
            try
            {
                this.TextTree.SetElementValue(tpInsideElement, LogicalDirection.Forward, TextElement.FontWeightProperty, FontWeights.Bold);
                throw new ApplicationException("TextContainer.SetElementValue() didnt throw exception when expected");
            }
            catch (System.InvalidOperationException e)
            {
                Log("TextContainer.SetElementValue() has thrown exception as expected with invalid TextPointer.\n"
                    + "Source of Exception: " + e.Source);
            }
            */
            this.TextTree.SetElementValue(tpBeforeElement, LogicalDirection.Forward, 
                TextElement.FontSizeProperty, (double)testFontSize);
            this.TextTree.SetElementValue(tpAfterElement, LogicalDirection.Backward, 
                TextElement.FontFamilyProperty, new FontFamily(testFontFamily));

            Verifier.Verify(((tpInsideElement.GetValue(TextElement.FontSizeProperty))) == testFontSize, 
                "Verifying SetElementValue() on FontSize property", true);
            Verifier.Verify(tpInsideElement.GetValue(TextElement.FontFamilyProperty).ToString() == testFontFamily, 
                "Verifying SetElementValue() on FontFamily property", true);

            //Test ClearValue()            
            try
            {
                this.TextTree.ClearValue(tpBeforeElement, TextElement.FontSizeProperty);
                throw new ApplicationException("TextContainer.ClearValue() didnt throw exception when expected");
            }
            catch (System.InvalidOperationException e)
            {
                Log("TextContainer.ClearValue() has thrown exception as expected with invalid TextPointer.\n"
                    + "Source of Exception: " + e.Source);              
            }
            this.TextTree.ClearValue(tpInsideElement, TextElement.FontSizeProperty);            
            this.TextTree.ClearValue(tpInsideElement, TextElement.FontFamilyProperty);

            Verifier.Verify(((tpInsideElement.GetValue(TextElement.FontSizeProperty))) == systemFontSize, 
                "Verifying ClearValue() on FontSize property when SetElementValue was done", true);
            Verifier.Verify(tpInsideElement.GetValue(TextElement.FontFamilyProperty).ToString() == systemFontFamily, 
                "Verifying ClearValue() on FontFamily property when SetElementValue was done", true);

            //Test SetValue()
            try
            {
                this.TextTree.SetValue(tpBeforeElement, TextElement.FontSizeProperty, 24.0);
                throw new ApplicationException("TextContainer.SetValue() didnt throw exception when expected");
            }
            catch (System.InvalidOperationException e)
            {
                Log("TextContainer.SetValue() has thrown exception as expected with invalid TextPointer.\n"
                    + "Source of Exception: " + e.Source);
            }
            this.TextTree.SetValue(tpInsideElement, TextElement.FontSizeProperty, 24.0);

            Verifier.Verify(((tpInsideElement.GetValue(TextElement.FontSizeProperty))) == testFontSize, 
                "Verifying SetValue() on FontSize property", true);

            this.TextTree.ClearValue(tpInsideElement, TextElement.FontSizeProperty);
            Verifier.Verify(((tpInsideElement.GetValue(TextElement.FontSizeProperty))) == systemFontSize,
                "Verifying ClearValue() on FontSize property when SetValue was done", true);

            //Test IsReadOnly - Adding it for coverage
            Verifier.Verify(!this.TextTree.IsReadOnly, "Verifying the IsReadOnly is false", true);

            //ToString - Adding it for coverage
            Log("Calling ToString()");
            Log("TextContainer.ToString: [" + this.TextTree.ToString() + "]");
            Verifier.Verify((this.TextTree.ToString() == textTreeToString)||(this.TextTree.ToString() == "System.Windows.Documents.TextContainer"), 
                "Verifying TextContainer's ToString method", true);

            Logger.Current.ReportSuccess();
        }        
    }   
}
