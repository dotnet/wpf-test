// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Controls;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

#endregion

namespace Microsoft.Test.Layout
{
    public class SimpleDecoratorForTextBlock : Decorator
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            InlineUIContainer iuc = (InlineUIContainer)Parent;
            DerivedTextBlock tb = (DerivedTextBlock)iuc.Parent;
            DerivedTextBlock tb2 = new DerivedTextBlock();
            Size someRet = base.ArrangeOverride(arrangeSize);
            ReflectionHelper rh = ReflectionHelper.WrapObject(tb);          
            // HasComplexContent
            object o = rh.GetProperty("HasComplexContent");
            // Logical Children
            rh.SetProperty("IsContentPresenterContainer", true);
            tb.getLogicalChildren();
            rh.SetProperty("IsContentPresenterContainer", false);
            tb.getLogicalChildren();
            rh.GetProperty("TextView");
            rh.CallMethod("GetPlainText");
            // for Complex Content = NULL
            ReflectionHelper rh1 = ReflectionHelper.WrapObject(tb2);
            rh1.CallMethod("GetPlainText");

            TextBlock newTextBlock = new TextBlock();
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo mInfo = newTextBlock.GetType().GetMethod("ShouldSerializeBaselineOffset", flags);
            try
            {
                rh.CallMethod(mInfo);
            }
            catch (ArgumentException){}

            // hosted elements 
            tb.getHostedElementsCore();                
            tb.GetPositionFromPoint(new Point(), false);
          
            return someRet;
        }
    }

}