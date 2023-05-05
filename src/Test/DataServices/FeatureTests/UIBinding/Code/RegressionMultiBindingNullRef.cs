// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests null-ref a multibinding's target element is gc'd while the expression is responding to a property change.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Binding", SecurityLevel=TestCaseSecurityLevel.FullTrust)]
    public class RegressionMultiBindingNullRef : XamlTest
    {
        #region Private Data

        private static MethodInfo s_cleanupMethodInfo;
        private static TextBlock s_textBlock;

        private DependencyPropertyObject<Places> _placesDependencyObject;
        private MultiBindingExpression _multiBindingExpression;
        private ListBox _listBox;

        #endregion

        #region Constructors

        public RegressionMultiBindingNullRef()
            : base(@"RegressionMultiBindingNullRef.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Public and Protected Members

        // the bug arises when a GC collects a MultiBindingExpression's target element
        // while the expression is responding to a property change.  This is hard to repro
        // in the wild, but we can force it to happen here.  This method gets called during
        // a property change (from the multi-converter).
        public static void ForceGC()
        {
            if (s_textBlock == null)
                return;

            // see if the element is still in the tree
            if (s_textBlock.IsLoaded)
                return;

            // not in the tree - release the last reference and force a GC
            s_textBlock = null;
            long x = GetMemory();
        }

        public class DependencyPropertyObject<T> : DependencyObject
        {
            public DependencyPropertyObject()
            {
            }

            public DependencyPropertyObject(T defaultValue)
            {
                DependencyProperty = defaultValue;
            }

            public T DependencyProperty
            {
                get { return (T)GetValue(DependencyPropertyProperty); }
                set { SetValue(DependencyPropertyProperty, value); }
            }

            public static readonly DependencyProperty DependencyPropertyProperty =
                System.Windows.DependencyProperty.Register("DependencyProperty", typeof(T), typeof(DependencyPropertyObject<T>), new UIPropertyMetadata(default(T)));
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            s_cleanupMethodInfo = typeof(BindingOperations).GetMethod("Cleanup", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            _placesDependencyObject = new DependencyPropertyObject<Places>(new Places());
            _listBox = (ListBox)RootElement.FindName("listBox");
            _listBox.DataContext = _placesDependencyObject;

            return TestResult.Pass;
        }

        private TestResult Verify()
        {
            // Because of timing specialness with GC and such, it's important to wait for the queue to clear after each operation
            WaitForPriority(DispatcherPriority.SystemIdle);
            ResetCollection();
            WaitForPriority(DispatcherPriority.SystemIdle);
            ResetCollection();
            WaitForPriority(DispatcherPriority.SystemIdle);
            // This reset collection was causing the null ref
            ResetCollection();
            WaitForPriority(DispatcherPriority.SystemIdle);

            // If we made it here the null ref didn't repro.
            return TestResult.Pass;
        }

        private void ResetCollection()
        {
            SaveBinding();
            _placesDependencyObject.DependencyProperty = new Places();
        }

        private void SaveBinding()
        {
            if (_multiBindingExpression != null)
                return;

            TextBlock foundTextBlock = (TextBlock)Util.FindVisualByType(typeof(TextBlock), _listBox);
            if (foundTextBlock != null)
            {
                _multiBindingExpression = BindingOperations.GetMultiBindingExpression(foundTextBlock, TextBlock.TextProperty);
                s_textBlock = foundTextBlock;
            }
        }

        private static long GetMemory()
        {
            long result = GC.GetTotalMemory(true);
            GC.WaitForPendingFinalizers();
            while ((bool)s_cleanupMethodInfo.Invoke(null, null))
            {
                result = GC.GetTotalMemory(true);
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        #endregion
    }

    public class GarbageCollectingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            RegressionMultiBindingNullRef.ForceGC();  // cause a GC to collect the target element during a property change
            return "GarbageCollected";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
