// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data about text elements (Bold, Italic, etc.)

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/TextElementTypeData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Threading;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Provides information about available text element types.
    /// </summary>
    public static class TextElementType
    {
        #region Public properties.

        /// <summary>A list of all BlockItem types.</summary>
        public static Type[] BlockItemValues
        {
            get
            {
                s_textElementTypes = new Type[] {
                    typeof(List),
                    typeof(Paragraph),
                    typeof(Section),
                    typeof(Table),
                    };

                return s_textElementTypes;
            }
        }

        /// <summary>Determines whether the specifed child type can be inserted into the parent type.</summary>
        /// <param name="childType">Type of element to consider inserting.</param>
        /// <param name="parentType">Type of element to consider inserting into.</param>
        /// <returns>true if childType can be inserted in parentType; false otherwise.</returns>
        public static bool IsValidChildType(Type childType, Type parentType)
        {
            if (childType == null)
            {
                throw new ArgumentNullException("childType");
            }
            if (parentType == null)
            {
                throw new ArgumentNullException("parentType");
            }

            foreach(KeyValuePair<Type, Type> pair in ValidParentChildTypes)
            {
                if (pair.Key.IsAssignableFrom(parentType) &&
                    pair.Value.IsAssignableFrom(childType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>A list of all TextElement types that can be instantiated.</summary>
        public static Type[] NonAbstractValues
        {
            get
            {
                if (s_nonAbstractValues == null)
                {
                    List<Type> list;

                    list = new List<Type>();
                    foreach(Type type in Values)
                    {
                        if (!type.IsAbstract)
                        {
                            list.Add(type);
                        }
                    }
                    s_nonAbstractValues = list.ToArray();
                }
                return s_nonAbstractValues;
            }
        }

        /// <summary>A list of all TextElement types.</summary>
        public static Type[] Values
        {
            get
            {
                if (s_textElementTypes == null)
                {
                    s_textElementTypes = new Type[] {
                        typeof(Block),
                        typeof(Bold),
                        typeof(Figure),
                        typeof(Floater),
                        typeof(Hyperlink),
                        typeof(Inline),
                        typeof(InlineUIContainer),
                        typeof(Italic),
                        typeof(LineBreak),
                        typeof(List),
                        typeof(ListItem),
                        typeof(Paragraph),
                        typeof(Run),
                        typeof(Section),
                        typeof(Span),
                        typeof(Table),
                        typeof(TableCell),
                        typeof(TableRow),
                        typeof(TableRowGroup),
                        typeof(TextElement),
                        typeof(Underline),
                    };
                }
                return s_textElementTypes;
            }
        }

        /// <summary>A pair of Parent Type / Child Type valid combinations.</summary>
        /// <remarks>This list is minimal, not exhaustive. Use IsValidChildType for specific usage.</remarks>
        public static Dictionary<Type, Type> ValidParentChildTypes
        {
            get
            {
                if (s_validParentChildTypes == null)
                {
                    s_validParentChildTypes = new Dictionary<Type,Type>(12);

                    s_validParentChildTypes.Add(typeof(FlowDocument), typeof(Block));
                    // Block is abstract and has no specific children.
                    // Bold is a Span and inherits its children.
                    s_validParentChildTypes.Add(typeof(Figure), typeof(Block));
                    s_validParentChildTypes.Add(typeof(Floater), typeof(Block));
                    // Hyperlink is a Span and inherits its children.
                    // Inline is abstract and has no specific children.
                    // InlineUIContainer can have no children.
                    // Italic is a Span and inherits its children.
                    // LineBreak is an Inline that has no children.
                    s_validParentChildTypes.Add(typeof(List), typeof(ListItem));
                    s_validParentChildTypes.Add(typeof(ListItem), typeof(Block));
                    s_validParentChildTypes.Add(typeof(Paragraph), typeof(Inline));
                    // A Run can have no children.
                    s_validParentChildTypes.Add(typeof(Section), typeof(Block));
                    s_validParentChildTypes.Add(typeof(Span), typeof(Inline));
                    // Subscript is a Span and inherits its children.
                    // Superscript is a Span and inherits its children.
                    s_validParentChildTypes.Add(typeof(Table), typeof(TableRowGroup));
                    s_validParentChildTypes.Add(typeof(TableCell), typeof(Block));
                    s_validParentChildTypes.Add(typeof(TableRow), typeof(TableCell));
                    s_validParentChildTypes.Add(typeof(TableRowGroup), typeof(TableRow));
                    // TextElement is abstract and has no specific children.
                    // Underline is a Span and inherits its children.
                }
                return s_validParentChildTypes;
            }
        }

        #endregion Public properties.

        #region Private fields.

        /// <summary>A list of all TextElement types that can be instantiated.</summary>
        private static Type[] s_nonAbstractValues;

        /// <summary>A list of all TextElement types.</summary>
        private static Type[] s_textElementTypes;

        private static Dictionary<Type, Type> s_validParentChildTypes;

        #endregion Private fields.
    }
}
