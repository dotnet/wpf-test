// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.XamlTextReader
{
    /// <summary>
    /// Verify the internal class XamlSubreader's properties and methods.
    /// The contents of the subtree are compared to the corresponding contents of the original tree.
    /// This test uses ReadSubtree() to get a subtree, the first time from the original tree, the 
    /// second time from the first subtree.
    /// </summary>
    public class XamlSubReaderTest
    {
        #region Public Methods

        /// <summary>
        /// Test case Entry point.  The Xaml test framework uses an .xtc file to call this method.
        /// </summary>
        public void RunTest()
        {
            bool firstSubtreePassed = false;
            bool secondSubtreePassed = false;

            // This causes the WPF assemblies to load, this is a hack that will be removed later.
            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;

            const string XamlString = @"
            <ContentControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
                <ContentControl.Content>
                    <Button Name='Button1'>
                        <Button.Content>
                            <StackPanel Background='DodgerBlue'>
                                <TextBox Text='textbox1' />
                                <Ellipse>
                                    <Ellipse.Fill>LemonChiffon</Ellipse.Fill>
                                </Ellipse>
                            </StackPanel>
                        </Button.Content>
                    </Button>
                </ContentControl.Content>
            </ContentControl>";

            // ********************************************************************************************
            // I - Create a XamlXmlReader subtree and verify that it matches the correct portion of
            // the original "main" tree.  It is advanced to the StackPanel node.
            // ********************************************************************************************
            XamlSchemaContext xamlSchemaContext = null;
            XamlXmlReader mainReader = CreateReader(XamlString, "StackPanel", ref xamlSchemaContext);

            XamlXmlReader tempReader1 = CreateReader(XamlString, "StackPanel", ref xamlSchemaContext);

            // Create a list of nodes and their properties, which will be used as expected values
            // for comparision with the actual values in the SubTree.
            Queue<ReaderProperties> nodeListMain = CreateList(tempReader1);
            tempReader1.Close();

            // Obtain the 1st Subtree.
            XamlReader subTreeReader1 = mainReader.ReadSubtree();
            subTreeReader1.Skip(); // Access the root of the Subtree (the StackPanel).

            // Verify the 1st Subtree by comparing it to the list of nodes from the main tree.
            firstSubtreePassed = VerifySubtree(subTreeReader1, nodeListMain);

            subTreeReader1.Close();
            mainReader.Close();

            // ********************************************************************************************
            // II - Create a 2nd subtree from the 1st subtree, and verify that it matches the correct
            // portion of the 1st subtree.  It is advanced to the Ellipse node.
            // ********************************************************************************************
            mainReader = CreateReader(XamlString, "Ellipse", ref xamlSchemaContext);

            XamlXmlReader tempReader2 = CreateReader(XamlString, "Ellipse", ref xamlSchemaContext);

            // Create a list of nodes and their properties, which will be used as expected values
            // for comparision with the actual values in the SubTree.
            Queue<ReaderProperties> ellipseList = CreateList(tempReader2);
            tempReader2.Close();

            // Obtain the 1st Subtree.
            subTreeReader1 = mainReader.ReadSubtree();
            subTreeReader1.Skip();

            // Obtain the 2nd Subtree.
            XamlReader subTreeReader2 = subTreeReader1.ReadSubtree();
            subTreeReader2.Skip();

            // Verify the 2nd Subtree.
            secondSubtreePassed = VerifySubtree(subTreeReader2, ellipseList);

            subTreeReader2.Close();
            subTreeReader1.Close();
            mainReader.Close();

            if (firstSubtreePassed && secondSubtreePassed)
            {
                GlobalLog.LogEvidence("---PASS: All subtests passed.");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("---FAIL: one or more subtests failed.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a XamlReader, then reads until the specified XamlType is reached.
        /// </summary>
        /// <param name="xamlString">The Xaml string being tested.</param>
        /// <param name="requestedXamlType">The XamlType specified.</param>
        /// <param name="xamlSchemaContext">The XamlSchemaContext that is created.</param>
        /// <returns>An XamlXmlReader, to which the xamlType is read</returns>
        private XamlXmlReader CreateReader(string xamlString, string requestedXamlType, ref XamlSchemaContext xamlSchemaContext)
        {
            xamlSchemaContext = new XamlSchemaContext();
            string elementXns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XamlType xamlType = xamlSchemaContext.GetXamlType(elementXns, requestedXamlType);

            StringReader stringReader = new StringReader(xamlString);
            XmlReader xmlReader = XmlReader.Create(stringReader);

            XamlXmlReader reader = new XamlXmlReader(xmlReader, xamlSchemaContext);
            reader.Skip();

            ReadToXamlType(reader, xamlType);

            GlobalLog.LogStatus("---Created an XamlXmlReader, advanced to the " + requestedXamlType + ".");

            return reader;
        }

        /// <summary>
        /// Given a XamlReader, reads until the specified XamlType is reached.
        /// </summary>
        /// <param name="reader">The XamlReader to be read.</param>
        /// <param name="requestedXamlType">The XamlType that is looked for.</param>
        private void ReadToXamlType(XamlReader reader, XamlType requestedXamlType)
        {
            bool xamlTypeFound = false;

            if (!reader.IsEof)
            {
                do
                {
                    if (reader.NodeType == XamlNodeType.StartObject)
                    {
                        if (reader.Type.Name == requestedXamlType.Name)
                        {
                            xamlTypeFound = true;
                            break;
                        }
                    }
                } 
                while (reader.Read());
            }

            if (xamlTypeFound == false)
            {
                throw new Microsoft.Test.TestSetupException("\nERROR: the requested XamlType (" + requestedXamlType + ") was not found.\n");
            }
        }

        /// <summary>
        /// Copy the nodes read by the provided XamlReader and return them as a Queue of structs. 
        /// </summary>
        /// <param name="reader">The XamlReader subtree to be verified.</param>
        /// <returns>A Queue containing XamlReader nodes</returns>
        private Queue<ReaderProperties> CreateList(XamlReader reader)
        {
            var list = new Queue<ReaderProperties>();
            int depth = 0;
            if (!reader.IsEof)
            {
                bool isRootMember = reader.NodeType == XamlNodeType.StartMember;
                do
                {
                    list.Enqueue(new ReaderProperties(
                                     reader.IsEof,
                                     reader.Member,
                                     reader.Namespace,
                                     reader.NodeType,
                                     reader.SchemaContext,
                                     reader.Type,
                                     reader.Value));

                    if (isRootMember)
                    {
                        if (reader.NodeType == XamlNodeType.StartMember)
                        {
                            depth += 1;
                        }
                        else if (reader.NodeType == XamlNodeType.EndMember)
                        {
                            depth -= 1;
                        }
                    }
                    else
                    {
                        if (reader.NodeType == XamlNodeType.StartObject ||
                            reader.NodeType == XamlNodeType.GetObject)
                        {
                            depth += 1;
                        }
                        else if (reader.NodeType == XamlNodeType.EndObject)
                        {
                            depth -= 1;
                        }
                    }

                    if (depth <= 0)
                    {
                        break;
                    }
                } 
                while (reader.Read());
            }
            else
            {
                throw new Microsoft.Test.TestSetupException("\nERROR: attempted to create a node list when the XamlReader is EOF.\n");
            }

            return list;
        }

        /// <summary>
        /// Carry out a set of verification tests for properties of the XamlSubreader. 
        /// </summary>
        /// <param name="subTreeReader">The XamlReader subtree to be verified.</param>
        /// <param name="expectedNodeList">An ArrayList of structs, each row containing all the expected property values of a node.</param>
        /// <returns>A boolean indicating whether or not the tests passed</returns>
        private bool VerifySubtree(XamlReader subTreeReader, Queue<ReaderProperties> expectedNodeList)
        {
            bool subTestsPassed = true;

            for (int i = 0; expectedNodeList.Count > 0; i++)
            {
                ReaderProperties expectedNode = expectedNodeList.Dequeue();
                if (subTreeReader.IsEof)
                {
                    break;
                }
                else
                {
                    GlobalLog.LogStatus("--------TESTING NODE #" + i.ToString(CultureInfo.InvariantCulture) + " / NodeType: " + expectedNode.NodeType + "--------");

                    if (subTreeReader.IsEof != expectedNode.IsEof)
                    {
                        GlobalLog.LogEvidence("--FAILED. Expected IsEof: " + expectedNode.IsEof + " / Actual IsEof: " + subTreeReader.IsEof);
                        subTestsPassed = false;
                    }

                    if (subTreeReader.Member == null)
                    {
                        if (expectedNode.Member != null)
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Member: " + expectedNode.Member + " / Actual Member: null");
                            subTestsPassed = false;
                        }
                    }
                    else
                    {
                        if (expectedNode.Member == null)
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Member: null / Actual Member: " + subTreeReader.Member);
                            subTestsPassed = false;
                        }
                        else if (subTreeReader.Member.Name != expectedNode.Member.Name)
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Member.Name: " + expectedNode.Member.Name + " / Actual Member.Name: " + subTreeReader.Member.Name);
                            subTestsPassed = false;
                        }
                    }

                    if (subTreeReader.Namespace != expectedNode.NameSpace)
                    {
                        GlobalLog.LogEvidence("--FAILED. Expected Namespace: " + expectedNode.NameSpace + " / Actual Namespace: " + subTreeReader.Namespace);
                        subTestsPassed = false;
                    }

                    if (subTreeReader.NodeType != expectedNode.NodeType)
                    {
                        GlobalLog.LogEvidence("--FAILED. Expected NodeType: " + expectedNode.NodeType + " / Actual NodeType: " + subTreeReader.NodeType);
                        subTestsPassed = false;
                    }

                    if (subTreeReader.SchemaContext == expectedNode.SchemaContext)
                    {
                        GlobalLog.LogEvidence("--FAILED. The SchemaContexts should be different, but they are the same.");
                        subTestsPassed = false;
                    }

                    if (subTreeReader.Type == null)
                    {
                        if (expectedNode.Type != null)
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Type: " + expectedNode.Type + " / Actual Type: null");
                            subTestsPassed = false;
                        }
                    }
                    else
                    {
                        if (expectedNode.Type == null)
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Type null / ActualType: " + subTreeReader.Type);
                            subTestsPassed = false;
                        }
                        else if (subTreeReader.Type.Name != expectedNode.Type.Name)
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Type.Name: " + expectedNode.Type.Name + " / Actual Type.Name: " + subTreeReader.Type.Name);
                            subTestsPassed = false;
                        }
                    }

                    if (subTreeReader.Value == null)
                    {
                        if (expectedNode.Value != null)
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Value: " + expectedNode.Value.ToString() + " / Actual Value: null");
                            subTestsPassed = false;
                        }
                    }
                    else
                    {
                        if (expectedNode.Value == null)
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Value: null" + " / Actual Value = " + subTreeReader.Value.ToString());
                            subTestsPassed = false;
                        }
                        else if (subTreeReader.Value.ToString() != expectedNode.Value.ToString())
                        {
                            GlobalLog.LogEvidence("--FAILED. Expected Value: " + expectedNode.Value.ToString() + " / Actual Value = " + subTreeReader.Value.ToString());
                            subTestsPassed = false;
                        }
                    }

                    GlobalLog.LogStatus("--------------------------------------");

                    if (!subTestsPassed)
                    {
                        break;
                    }

                    subTreeReader.Read();
                }
            }

            if (subTestsPassed && expectedNodeList.Count != 0)
            {
                GlobalLog.LogEvidence("--FAILED. Subtree reader read fewer nodes than expected.");
                subTestsPassed = false;
            }

            if (subTestsPassed && !subTreeReader.IsEof)
            {
                GlobalLog.LogEvidence("--FAILED. Subtree reader read more nodes than expected.");
                subTestsPassed = false;
            }

            return subTestsPassed;
        }

        /// <summary>
        /// A struct used to contain expected property values.
        /// </summary>
        private struct ReaderProperties
        {
            /// <summary>
            /// The expected value of the IsEof property.
            /// </summary>
            public readonly bool IsEof;

            /// <summary>
            /// The expected value of the Member property.
            /// </summary>
            public readonly XamlMember Member;

            /// <summary>
            /// The expected value of the Namespace property.
            /// </summary>
            public readonly NamespaceDeclaration NameSpace;

            /// <summary>
            /// The expected value of the NodeType property.
            /// </summary>
            public readonly XamlNodeType NodeType;

            /// <summary>
            /// The expected value of the SchemaContext property.
            /// </summary>
            public readonly XamlSchemaContext SchemaContext;

            /// <summary>
            /// The expected value of the Type property.
            /// </summary>
            public readonly XamlType Type;

            /// <summary>
            /// The expected value of the Value property.
            /// </summary>
            public readonly object Value;

            /// <summary>
            /// Initializes a new instance of the ReaderProperties struct.
            /// </summary>
            /// <param name="p1">The Expected IsEof property.</param>
            /// <param name="p2">The Expected Member property.</param>
            /// <param name="p3">The Expected Namespace property.</param>
            /// <param name="p4">The Expected NodeType property.</param>
            /// <param name="p5">The Expected SchemaContext property.</param>
            /// <param name="p6">The Expected Type property.</param>
            /// <param name="p7">The Expected Value property.</param>
            public ReaderProperties(bool p1, XamlMember p2, NamespaceDeclaration p3, XamlNodeType p4, XamlSchemaContext p5, XamlType p6, object p7)
            {
                IsEof = p1;
                Member = p2;
                NameSpace = p3;
                NodeType = p4;
                SchemaContext = p5;
                Type = p6;
                Value = p7;
            }
        }

        #endregion
    }
}
