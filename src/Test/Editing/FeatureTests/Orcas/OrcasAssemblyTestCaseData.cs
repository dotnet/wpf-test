// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Assembly-level test case metadata for EditingOrcasTest

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.TextEditing;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Test.Uis.TextEditing
{    
    /// <summary>Test case data for test cases in this assembly.</summary>
    [TestCaseDataTableClass]
    public static class OrcasAssemblyTestCaseData
    {
        /// <summary>Test case data for test cases in this assembly.</summary>
        [TestCaseDataTable]
        public static TestCaseData[] Data = new TestCaseData[] {
            new TestCaseData(typeof(HyperlinkInRichTextBoxTest), "",
               new Dimension("EditableType", new object[] {TextEditableType.GetByName("RichTextBox"),
                                             TextEditableType.GetByName("subclass:RichTextBoxSubClass")})),            

            new TestCaseData(typeof(RichTextBoxIsDocumentEnabled), "",
               new Dimension("EditableType", new object[] {TextEditableType.GetByName("RichTextBox"),
                                             TextEditableType.GetByName("subclass:RichTextBoxSubClass")})),

            new TestCaseData(typeof(UndoLimitTest), "",
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("IsUndoEnabledTestValue", BooleanValues)),

            new TestCaseData(typeof(IndicCutCopyPaste), "",
                new Dimension("EditableType",               TextEditableType.Values),
                new Dimension("SelectTextOptionSwitch",    GetEnumValues(typeof(SelectTextOption))),
                new Dimension("CutCopyPasteSwitch",         GetEnumValues(typeof(CutCopyPasteOperation)))),

             
        };

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

        #endregion


        #region Private methods
        
        //Returns an object array with all values defined in thespecified enumeration.        
        private static object[] GetEnumValues(Type enumType)
        {
            object[] result;
            Array values;

            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            // Simple typecasting from System.Array to System.Object[] fails.
            values = System.Enum.GetValues(enumType);
            result = new object[values.Length];
            values.CopyTo(result, 0);
            return result;
        }

        #endregion


        #region Private fields.

        /// <summary>Array with static boolean values.</summary>
        private static object[] s_booleanValues;

        #endregion
    }
}