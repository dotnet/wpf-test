// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data about text-editable control types.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/TextEditableTypeData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections;
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
    /// Provides information about types that can be edited with an Avalon
    /// TextEditor.
    /// </summary>
    public sealed class TextEditableType: Test.Uis.Management.IStringIdentifierSupport
    {
        #region Constructors.

        /// <summary>Hidden constructor.</summary>
        private TextEditableType(string xamlName, Type type,
            bool supportsParagraphs, bool isAggregate, bool isPassword)
        {
            System.Diagnostics.Debug.Assert(type != null);
            System.Diagnostics.Debug.Assert(xamlName != null);
            System.Diagnostics.Debug.Assert(xamlName.Length > 0);

            this._isAggregate = isAggregate;
            this._supportsParagraphs = supportsParagraphs;
            this._type = type;
            this._xamlName = xamlName;
            this._isPassword = isPassword;
        }

        private static TextEditableType FromSubClass(string xamlName,
            Type type, bool supportsParagraphs)
        {
            TextEditableType result;

            result = new TextEditableType(xamlName, type, supportsParagraphs, true, false);
            result._isSubClass = true;

            return result;
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Creates an instance of the editable type.</summary>
        /// <returns>A new instance of the editable type.</returns>
        public FrameworkElement CreateInstance()
        {
            FrameworkElement result;

            result = Activator.CreateInstance(this._type) as FrameworkElement;
            if (result == null)
            {
                throw new InvalidOperationException(
                    "Common library error: " + this._type + " is not " +
                    "a FrameworkElement.");
            }

            return result;
        }

        /// <summary>
        /// Gets the XAML code required to instantiate the type
        /// in editable mode.
        /// </summary>
        /// <param name='attributes'>Any additional attributes to be added.</param>
        /// <param name='content'>Any content to be added.</param>
        /// <returns>The required XAML string.</returns>
        public string GetEditableXaml(string attributes, string content)
        {
            string result;

            result = "<" + XamlName;
            if (!IsAggregate)
            {
                result += " TextEditor.IsEnabled='True'";
            }
            result += " " + attributes + ">";
            if (SupportsParagraphs)
            {
                if (!content.Contains("<Paragraph>"))
                {
                    result += "<FlowDocument><Paragraph><Run>" + content + "</Run></Paragraph></FlowDocument>";
                }
                else
                {
                    result += "<FlowDocument>" + content + "</FlowDocument>";
                }
            }
            else
            {
                result += content;
            }
            result += "</" + XamlName + ">";
            return result;
        }

        /// <summary>
        /// Gets the value that matches a supertype in the given
        /// type/value pairs.
        /// </summary>
        /// <param name='type'>Type to match.</param>
        /// <param name='types'>Types with values.</param>
        /// <param name='values'>Values associated with types.</param>
        /// <param name='value'>On return, the matched value.</param>
        /// <remarks>An exception is thrown if no match is found.</remarks>
        /// <example>The following code shows how to use this method:<code>
        /// bool IsRich(Control myControl) {
        ///   bool result;
        ///   TextEditableType.GetValueForType(myControl.GetType(),
        ///     new Type[] { typeof(RichTextBox), typeof(TextBox), typeof(PasswordBox) },
        ///     new bool[] { true,                false,           false               },
        ///     out result);
        ///   return result;
        /// }</code></example>
        public static void GetValueForType(Type type,
            Type[] types, bool[] values, out bool value)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (types.Length != values.Length)
            {
                throw new ArgumentException("Length of types and values must be equal.");
            }

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsAssignableFrom(type))
                {
                    value = values[i];
                    return;
                }
            }
            throw new Exception("Type has no value associated: " + type);
        }

        /// <summary>
        /// Gets the value that matches a supertype in the given
        /// type/value pairs.
        /// </summary>
        /// <param name='type'>Type to match.</param>
        /// <param name='types'>Types with values.</param>
        /// <param name='values'>Values associated with types.</param>
        /// <param name='value'>On return, the matched value.</param>
        /// <remarks>An exception is thrown if no match is found.</remarks>
        /// <example>The following code shows how to use this method:<code>
        /// string GetFriendlyName(Control myControl) {
        ///   string result;
        ///   TextEditableType.GetValueForType(myControl.GetType(),
        ///     new Type[] { typeof(RichTextBox), typeof(TextBox), typeof(PasswordBox) },
        ///     new bool[] { "RichTextBox",       "TextBox",       "PasswordBox"       },
        ///     out result);
        ///   return result;
        /// }</code></example>
        public static void GetValueForType(Type type,
            Type[] types, string[] values, out string value)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (types.Length != values.Length)
            {
                throw new ArgumentException("Length of types and values must be equal.");
            }

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsAssignableFrom(type))
                {
                    value = values[i];
                    return;
                }
            }
            throw new Exception("Type has no value associated: " + type);
        }

        /// <summary>Provides a string representation of this object.</summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return "EditableType [" + _type.Name + "]";
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>
        /// Whether the element is an aggregate that does not require
        /// initialization.
        /// </summary>
        public bool IsAggregate
        {
            get { return this._isAggregate; }
        }

        /// <summary>
        /// Whether the element holds a password.
        /// </summary>
        public bool IsPassword
        {
            get { return this._isPassword; }
        }

        /// <summary>
        /// Whether the element is a subclass of an Avalon control.
        /// </summary>
        public bool IsSubClass
        {
            get { return this._isSubClass; }
        }


        /// <summary>
        /// Attribute name to map an XML namespace to reference
        /// subclassed controls.
        /// </summary>
        public static string SubClassNamespaceAttributeName
        {
            get
            {
                return "xmlns:subclass";
            }
        }

        /// <summary>
        /// Attribute value to map an XML namespace to reference
        /// subclassed controls.
        /// </summary>
        public static string SubClassNamespaceAttributeValue
        {
            get
            {
                return "clr-namespace:" + typeof(TextBoxSubClass).Namespace + ";assembly=EditingTestLib";
            }
        }

        /// <summary>
        /// Whether paragraphs are supported by the layout engine.
        /// </summary>
        public bool SupportsParagraphs
        {
            get { return this._supportsParagraphs; }
        }

        /// <summary>The type of the editable element.</summary>
        public Type Type
        {
            get { return this._type; }
        }

        /// <summary>Name used in XAML with default namespaces.</summary>
        public string XamlName
        {
            get { return this._xamlName; }
        }

        /// <summary>Gets a named TextEditableType.</summary>
        /// <param name='name'>Name of editable type, eg TextBox.</param>
        /// <returns>
        /// The TextEditableType with the name. If the name is not found,
        /// an exception is thrown.
        /// </returns>
        /// <example>The following code shows how this might be used.<code>
        /// FrameworkElement CreateControlByName(string name) {
        ///   TextEditableType t = TextEditableType.GetByName(name);
        ///   return t.CreateInstance();
        /// }</code></example>
        public static TextEditableType GetByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            foreach(TextEditableType result in Values)
            {
                if (result.XamlName == name)
                {
                    return result;
                }
            }
            throw new InvalidOperationException("Unknown editable type: " + name);
        }

        /// <summary>A list of all known editable types supplied by the platform.</summary>
        public static TextEditableType[] PlatformTypes
        {
            get
            {
                List<TextEditableType> typeList;

                typeList = new List<TextEditableType>();
                foreach (TextEditableType t in TextEditableType.Values)
                {
                    if (!t.IsSubClass)
                    {
                        typeList.Add(t);
                    }
                }
                return typeList.ToArray();
            }
        }

        /// <summary>A list of all known TextEditableTypes.</summary>
        public static TextEditableType[] Values
        {
            get
            {
                if (s_textEditableTypes == null)
                {
                    s_textEditableTypes = new TextEditableType[] {
                        new TextEditableType("PasswordBox", typeof(System.Windows.Controls.PasswordBox), false, true, true),
                        new TextEditableType("RichTextBox", typeof(System.Windows.Controls.RichTextBox), true, true, false),
                        new TextEditableType("TextBox", typeof(System.Windows.Controls.TextBox), false, true, false),
                        FromSubClass("subclass:TextBoxSubClass", typeof(TextBoxSubClass), false),
                        FromSubClass("subclass:RichTextBoxSubClass", typeof(RichTextBoxSubClass), true),

                        // Non-'aggregates', or controls that require initialization,
                        // are no longer supported from the public API.
                        // new TextEditableType("Text", typeof(System.Windows.Controls.TextBlock), false, false),
                        // new TextEditableType("TextPanel", typeof(FlowDocumentScrollViewer), true, false),
                    };
                }
                return s_textEditableTypes;
            }
        }

        #endregion Public properties.


        #region Private properties.

        /// <summary>Non-null string identifier for this instance.</summary>
        string Test.Uis.Management.IStringIdentifierSupport.StringIdentifier
        {
            get { return this.Type.Name; }
        }

        #endregion Private properties.

        #region Private fields.

        /// <summary>
        /// Whether the element is an aggregate that does not require
        /// initialization.
        /// </summary>
        private readonly bool _isAggregate;

        /// <summary>
        /// Whether the element holds text in a password container.
        /// </summary>
        private readonly bool _isPassword;

        /// <summary>
        /// Whether the element is a subclass of an Avalon control.
        /// </summary>
        private bool _isSubClass;

        /// <summary>
        /// Whether paragraphs are supported by the layout engine.
        /// </summary>
        private readonly bool _supportsParagraphs;

        /// <summary>The type of the editable element.</summary>
        private readonly Type _type;

        /// <summary>Name used in XAML with default namespaces.</summary>
        private readonly string _xamlName;

        /// <summary>A list of all known TextEditableTypes.</summary>
        private static TextEditableType[] s_textEditableTypes;

        #endregion Private fields.
    }
}