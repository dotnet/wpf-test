// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Layout {
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Test.TestTypes;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Markup;
    using System.Windows.Threading;

    using Microsoft.Test.Logging;
    
    ///<summary>A Testcase that provides a framework for testing whether API's trigger ui relayout or not</summary>
    ///<remarks>
    ///Use the BeginRelayoutTest() method as follows
    ///  RelayoutListener result = BeginRelayoutTest(elementToTest, ExpectedFlags.Expected)
    ///  call some api that affects layout
    ///  call result.GetResult(); to get the result
    ///  call or result.Cancel(); to cancel the test
    ///</remarks>
    public abstract class UIRelayoutTestCase: AvalonTest {
        ///<summary>Constructor</summary>
        ///<param Name="target">UIElement on which relayout is expected to occur</param>
        ///<param Name="flags">Enum indicating whethere relayout is expected or not</param>
        ///<returns>Object that listens for relayout an Logs testcase</returns>
        protected internal RelayoutListener BeginRelayoutTest(UIElement target, ExpectedFlags expected) {
            return new UIRelayoutListener(target, expected);
        }
    }    
}