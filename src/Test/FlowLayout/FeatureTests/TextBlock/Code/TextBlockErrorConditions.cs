// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Automation;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>TextBlock</area>
    /// <owner>Microsoft</owner>
    /// <priority>2</priority>
    /// <description>
    /// Testing Textblock error conditions.
    /// </description>
    /// </summary>
    [Test(2, "TextBlock", "TextBlockErrorConditions")]
    public class TextBlockErrorConditions : AvalonTest
    {
        #region Test case members

        private Window _w;
        private TextBlock _tb;
        private DerivedTextBlock _dtb;
        #endregion

        #region Constructor

        public TextBlockErrorConditions()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps
        
        /// <summary>
        /// Initialize: Setup test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {           
            Status("Initialize ....");
            _w = new Window();
            _tb = new TextBlock();
            _dtb = new DerivedTextBlock();
            _w.Content = _tb;
            _w.Show();

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }
     
        /// <summary>
        /// RunTests: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            ReflectionHelper rh = ReflectionHelper.WrapObject(_dtb);
            ReflectionHelper rh2 = ReflectionHelper.WrapObject(_tb);
            IContentHostTests(_dtb);
            IAddChildTests(_dtb);
            IServiceProviderTests(_dtb);
            PropertyTests(_dtb);
            ProtectedMethodsTests(_tb, _dtb, rh);
            InternalMethodsTests(_tb, _dtb, rh, rh2);
            PrivateMethodsTests(_tb, _dtb, rh, rh2);
            DependencyPropertyHelpersTests(_tb, _dtb, rh, rh2);
            try
            {
                // Will Throw because, no complex content inside the DerivedTextBlock                
                _dtb.GetPositionFromPoint(new Point(0, 0), false);
            }
            catch(NullReferenceException) { }
            
            LogComment("TestCase Passed");
            
            return TestResult.Pass;
        }
        #endregion
        
        private void IContentHostTests(DerivedTextBlock dtb)
        {
            IContentHost ichost = (IContentHost)dtb;
            //-------------------------------------------------------------------
            //
            //  IContentHost Exception Tests
            //
            //-------------------------------------------------------------------

            #region IContentHost Null Parameter Tests

            try
            {
                ichost.GetRectangles(null);
            }
            catch (ArgumentNullException) { }
            
            try
            {
                ichost.OnChildDesiredSizeChanged(null);
            }
            catch (ArgumentNullException) { }

            #endregion IContentHost Null Parameter Tests
        }

        private void IAddChildTests(DerivedTextBlock dtb)
        {
            IAddChild iaddc = (IAddChild)dtb;
            //-------------------------------------------------------------------
            //
            //  IAddChild Exception Tests
            //
            //-------------------------------------------------------------------

            #region IContentHost Null Parameter Tests

            try
            {
                iaddc.AddChild(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                iaddc.AddText(null);
            }
            catch (ArgumentNullException) { }

            #endregion IContentHost Null Parameter Tests

            #region IContentHost Invalid Parameter Tests

            #endregion IContentHost Invalid Parameter Tests
        }

        private void IServiceProviderTests(DerivedTextBlock dtb)
        {
            IServiceProvider isp = (IServiceProvider)dtb;
            //-------------------------------------------------------------------
            //
            //  IServiceProvider Exception Tests
            //
            //-------------------------------------------------------------------

            #region IServiceProvider Null Parameter Tests

            try
            {
                isp.GetService(null);
            }
            catch (ArgumentNullException) { }

            #endregion IServiceProvider Null Parameter Tests
        }

        private void PropertyTests(DerivedTextBlock dtb)
        {
            //-------------------------------------------------------------------
            //
            //  Invalid Values for Public and Private Properties
            //
            //-------------------------------------------------------------------

            #region Dynamic Public Properties Null Tests

            try
            {
                DerivedTextBlock.SetFontFamily(null, null);
            }
            catch (ArgumentNullException) { }

            try
            {
                DerivedTextBlock.SetFontFamily(dtb, null);
            }
            catch (ArgumentException) { }

            try
            {
                DerivedTextBlock.SetFontStyle(null, FontStyles.Normal);
            }
            catch (ArgumentNullException) { }

            try
            {
                DerivedTextBlock.SetFontStretch(null, FontStretches.Normal);
            }
            catch (ArgumentNullException) { }

            try
            {
                DerivedTextBlock.SetFontSize(null, 10);
            }
            catch (ArgumentNullException) { }

            try
            {
                DerivedTextBlock.SetForeground(null, null);
            }
            catch (ArgumentNullException) { }

            try
            {
                DerivedTextBlock.SetForeground(dtb, null);
            }
            catch (ArgumentException) { }

            try
            {
                DerivedTextBlock.SetLineHeight(null, 10);
            }
            catch (ArgumentNullException) { }

            try
            {
                DerivedTextBlock.SetBaselineOffset(null, 2);
            }
            catch (ArgumentNullException) { }

            #endregion Dynamic Public Properties Null Tests
        }

        private void ProtectedMethodsTests(TextBlock tb, DerivedTextBlock dtb, ReflectionHelper rh)
        {
            //-------------------------------------------------------------------
            //
            //  Null Parameters for Protected Methods Tests
            //
            //-------------------------------------------------------------------
            #region Protected Methods Tests

            try
            {
                dtb.OnRender(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                dtb.HitTestCore(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                dtb.CallGetRectanglesCore(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                dtb.CallOnChildDesiredSizeChangedCore(null);
            }
            catch (ArgumentNullException) { }

            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo mInfo = tb.GetType().GetMethod("GetPatternProviderCore", flags, null, new Type[] { typeof(UIElement), typeof(AutomationPattern) } , null);

            try
            {
                rh.CallMethod(mInfo, null, null);
            }
            catch (ArgumentNullException) { }

            try
            {
                rh.CallMethod(mInfo, dtb, null);
            }
            catch (ArgumentNullException) { }

            #endregion Protected Methods Tests
        }

        private void InternalMethodsTests(TextBlock tb, DerivedTextBlock dtb, ReflectionHelper rh, ReflectionHelper rh2)
        {
            //-------------------------------------------------------------------
            //
            //  Null Parameters for Internal Methods Tests
            //
            //-------------------------------------------------------------------
            #region Internal Methods Tests

            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo mInfo;

            mInfo = tb.GetType().GetMethod("SetTextContainer", flags, null, new Type[] { ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextContainer") }, null);
            try
            {
                rh.CallMethod(mInfo, tb);
            }
            catch (ArgumentException) { }

            CommonFunctionality.FlushDispatcher();
            mInfo = tb.GetType().GetMethod("GetRectangleFromTextPosition", flags, null, new Type[] { ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextPointer") }, null);
            try
            {
                rh2.CallMethod(mInfo, tb.ContentStart);
            }
            catch (ArgumentException) { }

            mInfo = tb.GetType().GetMethod("IsAtCaretUnitBoundary", flags, null, new Type[] { ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextPointer"), typeof(int), typeof(int) }, null);
            try
            {
                rh2.CallMethod(mInfo, null, null, null);
            }
            catch (TargetInvocationException) { }

            mInfo = tb.GetType().GetMethod("GetNextCaretUnitPosition", flags, null, new Type[] { ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextPointer"), typeof(LogicalDirection), typeof(int), typeof(int) }, null);
            try
            {
                rh2.CallMethod(mInfo, null, null, null, null);
            }
            catch (TargetInvocationException) { }


            mInfo = tb.GetType().GetMethod("GetBackspaceCaretUnitPosition", flags, null, new Type[] { ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextPointer"), typeof(int), typeof(int) }, null);
            try
            {
                rh2.CallMethod(mInfo, null, null, null);
            }
            catch (TargetInvocationException) { }

            #endregion Internal Methods Tests
        }

        private void PrivateMethodsTests(TextBlock tb, DerivedTextBlock dtb, ReflectionHelper rh, ReflectionHelper rh2)
        {
            //-------------------------------------------------------------------
            //
            //  Null Parameters for Private Methods Tests
            //
            //-------------------------------------------------------------------
            #region Private Methods Tests

            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo mInfo;

            mInfo = tb.GetType().GetMethod("OnHighlightChanged", flags, null, new Type[] { typeof(object), typeof(EventArgs) }, null);
            try
            {
                rh.CallMethod(mInfo, null, null);
            }
            catch (ArgumentException) { }

            mInfo = tb.GetType().GetMethod("OnHighlightChanged", flags, null, new Type[] { typeof(object), typeof(EventArgs) }, null);
            try
            {
                rh.CallMethod(mInfo, tb, null);
            }
            catch (ArgumentException) { }

            mInfo = tb.GetType().GetMethod("OnTextContainerChanging", flags, null, new Type[] { typeof(object), typeof(EventArgs) }, null);
            try
            {
                rh.CallMethod(mInfo, null, null);
            }
            catch (TargetInvocationException) { }

            flags = BindingFlags.NonPublic | BindingFlags.Static;
            mInfo = tb.GetType().GetMethod("OnBaselineOffsetChanged", flags, null, new Type[] { typeof(DependencyObject), typeof(DependencyPropertyChangedEventArgs) }, null);
            try
            {
                DependencyObject dobj = null;
                rh.CallMethod(mInfo, new object[] { dobj, null });
            }
            catch (TargetInvocationException) { }

            #endregion Private Methods Tests
        }

        private void DependencyPropertyHelpersTests(TextBlock tb, DerivedTextBlock dtb, ReflectionHelper rh, ReflectionHelper rh2)
        {
            //-------------------------------------------------------------------
            //
            //  Null Parameters for Dependency Property Helpers Tests
            //
            //-------------------------------------------------------------------
            #region Dependency Property Helpers Tests

            BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
            MethodInfo mInfo;
            DependencyObject dobj = null;

            mInfo = tb.GetType().GetMethod("GetFontFamily", flags, null, new Type[] { typeof(DependencyObject) }, null);
            try
            {
                rh.CallMethod(mInfo, dobj);
            }
            catch (TargetInvocationException) { }

            mInfo = tb.GetType().GetMethod("GetFontStyle", flags, null, new Type[] { typeof(DependencyObject) }, null);
            try
            {
                rh.CallMethod(mInfo, dobj);
            }
            catch (TargetInvocationException) { }

            mInfo = tb.GetType().GetMethod("GetFontWeight", flags, null, new Type[] { typeof(DependencyObject) }, null);
            try
            {
                rh.CallMethod(mInfo, dobj);
            }
            catch (TargetInvocationException) { }

            mInfo = tb.GetType().GetMethod("GetFontStretch", flags, null, new Type[] { typeof(DependencyObject) }, null);
            try
            {
                rh.CallMethod(mInfo, dobj);
            }
            catch (TargetInvocationException) { }

            mInfo = tb.GetType().GetMethod("GetFontSize", flags, null, new Type[] { typeof(DependencyObject) }, null);
            try
            {
                rh.CallMethod(mInfo, dobj);
            }
            catch (TargetInvocationException) { }

            mInfo = tb.GetType().GetMethod("GetForeground", flags, null, new Type[] { typeof(DependencyObject) }, null);
            try
            {
                rh.CallMethod(mInfo, dobj);
            }
            catch (TargetInvocationException) { }

            mInfo = tb.GetType().GetMethod("GetBaselineOffset", flags, null, new Type[] { typeof(DependencyObject) }, null);
            try
            {
                rh.CallMethod(mInfo, dobj);
            }
            catch (TargetInvocationException) { }

            flags = BindingFlags.Static | BindingFlags.NonPublic;
            mInfo = tb.GetType().GetMethod("OnTextChanged", flags, null, new Type[] { typeof(DependencyObject), typeof(DependencyPropertyChangedEventArgs) }, null);
            try
            {
                rh.CallMethod(mInfo, new object[] { null, null });
            }
            catch (TargetInvocationException) { }

            #endregion
        }       
    }
}
