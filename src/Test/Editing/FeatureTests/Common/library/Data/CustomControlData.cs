// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data about custom controls for test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/CustomControlData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Threading;
    using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Provides an implementation of a TextBox subclass.
    /// </summary>
    public class TextBoxSubClass : TextBox
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new Test.Uis.Data.TextBoxSubClass instance.
        /// </summary>
        public TextBoxSubClass(): base()
        {
        }

        /// <summary>
        /// Creates a style appropriate for a multiline text box.
        /// </summary>
        static TextBoxSubClass()
        {
            Style style;

            style = new Style(typeof(TextBoxSubClass));
            style.Setters.Add (new Setter(BackgroundProperty, BrushData.GradientBrush.Brush));
            style.Setters.Add (new Setter(AcceptsReturnProperty, true));
            style.Setters.Add (new Setter(MinLinesProperty, 3));
            style.Setters.Add (new Setter(MaxLinesProperty, 6));
            style.Setters.Add (new Setter(TextWrappingProperty, TextWrapping.Wrap));

            StyleProperty.OverrideMetadata(typeof(TextBoxSubClass),
                new FrameworkPropertyMetadata(style));
        }

        #endregion Constructors.
    }

    /// <summary>
    /// Provides an implementation of a RichTextBoxSubClass subclass.
    /// </summary>
    public class RichTextBoxSubClass : RichTextBox
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new Test.Uis.Data.RichTextBoxSubClass instance.
        /// </summary>
        public RichTextBoxSubClass()
            : base()
        {
        }

        #endregion Constructors.
    }

    // NOTE: PasswordBox cannot be subclassed.
}
