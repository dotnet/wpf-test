// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Helper classes for editing testing. 

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Editing/TestHelpers.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// A custom ContentElement for testing.
    /// </summary>
    class TestContentElement: ContentElement
    {
    }

    /// <summary>
    /// A UIElement class with a custom routed event.
    /// </summary>
    class TestRoutingUIElement: UIElement
    {
        #region Public methods.

        public void FireTestEvent()
        {
            RaiseEvent(new TestEventArgs());
        }

        public class TestEventArgs: RoutedEventArgs
        {
            public TestEventArgs(): base()
            {
                RoutedEvent= TestRoutingUIElement.TestEvent;
            }
        }

        #endregion Public methods.

        public delegate void TestEventHandler(object sender,
            TestEventArgs args);

        public static readonly RoutedEvent TestEvent = EventManager
            .RegisterRoutedEvent("Test",
            System.Windows.RoutingStrategy.Tunnel,
            typeof(TestEventHandler), typeof(TestRoutingUIElement));
    }

    /// <summary>
    /// Provides a class that does not have an EditorType attribute
    /// declared.
    /// </summary>
    class TestNoEditorObject { }
}
