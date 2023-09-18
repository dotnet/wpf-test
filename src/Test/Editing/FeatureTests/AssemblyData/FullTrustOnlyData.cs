// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


//  Assembly-level test case metadata.

[assembly: Test.Uis.Management.VersionInformation("$Author$ $Change$ $Date$ $Revision$ $Source$")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using System.Xml;

    using DragDropAPI;
    using DataTransfer;

    using Test.Uis.Data;
    using Test.Uis.TextEditing;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion

    /// <summary>Test case data for test cases that can only be compiled into full trust assemblies.</summary>
    [TestCaseDataTableClass]
    public static class FullTrustOnlyData
    {
        /// <summary>Test case data for test cases in this assembly.</summary>
        [TestCaseDataTable]
        public static TestCaseData[] Data = new TestCaseData[] {

            new TestCaseData(typeof(IsReadOnlyValuePattern), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(TextBoxValuePattern), "",
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(XamlToRtfTest), "Opr=CutCopyPaste",
                new Dimension("XamlRtfOperation", new object[] {XamlRtfOperations.Copy, XamlRtfOperations.Cut}),
                new Dimension("XamlContent", new string[] { "One <Bold>Two </Bold>Three",
                    "One <Run FontWeight='Bold'>Two </Run>Three",
                    "One <Italic>Two </Italic>Three",
                    "One <Run FontStyle='Italic'>Two </Run>Three",
                    "One <Underline>Two </Underline>Three",
                    "One <Run TextDecorations='Underline'>Two </Run>Three",
                    "<Paragraph>One</Paragraph><Paragraph><Bold>Two</Bold></Paragraph><Paragraph>Three</Paragraph>",
                    "abc <Bold><Italic>def </Italic></Bold>", })),
            
            new TestCaseData(typeof(XamlToRtfTest), "Opr=DragDrop", 
                new Dimension("XamlRtfOperation", new object[] {XamlRtfOperations.Drag}),
                new Dimension("XamlContent", new string[] { "One <Bold>Two </Bold>Three",
                    "One <Run FontWeight='Bold'>Two </Run>Three",
                    "One <Italic>Two </Italic>Three",
                    "One <Run FontStyle='Italic'>Two </Run>Three",
                    "One <Underline>Two </Underline>Three",
                    "One <Run TextDecorations='Underline'>Two </Run>Three",
                    "<Paragraph>One</Paragraph><Paragraph><Bold>Two</Bold></Paragraph><Paragraph>Three</Paragraph>",
                    "abc <Bold><Italic>def </Italic></Bold>", })),
        };

        
        //
        //  Public Properties
        //
        

        #region Public properties.

        /// <summary>Array with static boolean values.</summary>
        public static object[] BooleanValues
        {
            get
            {
                if (s_booleanValues == null)
                {
                    s_booleanValues = new object[] { true, false };
                }
                return s_booleanValues;
            }
        }

        #endregion Public properties.


        #region Private fields.

        /// <summary>Array with static boolean values.</summary>
        private static object[] s_booleanValues;

        #endregion Private fields.
    }
}