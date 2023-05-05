// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Coverage for changing implicit data template resources
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Styles", "ChangeImplicitDataTemplate")]
    public class ChangeImplicitDataTemplate : XamlTest
    {
        #region Private Data

        private StackPanel _outerPanel, _innerPanel;
        private ResourceDictionary _outerDictionary, _innerDictionary;
        private DataTemplate _outerTemplate, _innerTemplate, _replacementTemplate;
        private DataTemplateKey _implicitKey;
        private ListBox _listBox;
        ItemContainerGenerator _generator;

        #endregion

        #region Constructors

        public ChangeImplicitDataTemplate()
            : base(@"ChangeImplicitDataTemplate.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RemoveRestoreInner);
            RunSteps += new TestStep(RemoveRestoreOuter);
            RunSteps += new TestStep(RemoveInnerRemoveOuterRestoreOuterRestoreInner);
            RunSteps += new TestStep(RemoveInnerRemoveOuterRestoreInnerRestoreOuter);
            RunSteps += new TestStep(RemoveOuterRemoveInnerRestoreOuterRestoreInner);
            RunSteps += new TestStep(RemoveOuterRemoveInnerRestoreInnerRestoreOuter);
            RunSteps += new TestStep(ReplaceInner);
            RunSteps += new TestStep(ReplaceOuter);
            RunSteps += new TestStep(AddMergedDictionary);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _outerPanel = (StackPanel) RootElement.FindName("OuterPanel");
            _innerPanel = (StackPanel) RootElement.FindName("InnerPanel");
            _listBox = (ListBox)RootElement.FindName("listbox");
            _implicitKey = new DataTemplateKey(typeof(MediaItem));

            if (_outerPanel == null || _innerPanel == null || _listBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            _outerDictionary = _outerPanel.Resources;
            _innerDictionary = _innerPanel.Resources;

            _outerDictionary.InvalidatesImplicitDataTemplateResources = true;
            _innerDictionary.InvalidatesImplicitDataTemplateResources = true;

            _outerTemplate = (DataTemplate)_outerDictionary[_implicitKey];
            _innerTemplate = (DataTemplate)_innerDictionary[_implicitKey];
            _replacementTemplate = (DataTemplate)_outerDictionary["Replacement"];

            _generator = _listBox.ItemContainerGenerator;
            _listBox.ItemsSource = new MediaCollection("5");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        // removing inner template exposes outer,  restoring inner exposes inner
        private TestResult RemoveRestoreInner()
        {
            _innerDictionary.Remove(_implicitKey);
            Verify("Outer");

            _innerDictionary[_implicitKey] = _innerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        // removing outer template has no effect,  inner still wins
        private TestResult RemoveRestoreOuter()
        {
            _outerDictionary.Remove(_implicitKey);
            Verify("Inner");

            _outerDictionary[_implicitKey] = _outerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        // remove inner then outer, restore in the opposite order
        private TestResult RemoveInnerRemoveOuterRestoreOuterRestoreInner()
        {
            _innerDictionary.Remove(_implicitKey);
            Verify("Outer");

            _outerDictionary.Remove(_implicitKey);
            Verify(null);

            _outerDictionary[_implicitKey] = _outerTemplate;
            Verify("Outer");

            _innerDictionary[_implicitKey] = _innerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        // remove inner then outer, restore in the same order
        private TestResult RemoveInnerRemoveOuterRestoreInnerRestoreOuter()
        {
            _innerDictionary.Remove(_implicitKey);
            Verify("Outer");

            _outerDictionary.Remove(_implicitKey);
            Verify(null);

            _innerDictionary[_implicitKey] = _innerTemplate;
            Verify("Inner");

            _outerDictionary[_implicitKey] = _outerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        // remove outer then inner, restore in the same order
        private TestResult RemoveOuterRemoveInnerRestoreOuterRestoreInner()
        {
            _outerDictionary.Remove(_implicitKey);
            Verify("Inner");

            _innerDictionary.Remove(_implicitKey);
            Verify(null);

            _outerDictionary[_implicitKey] = _outerTemplate;
            Verify("Outer");

            _innerDictionary[_implicitKey] = _innerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        // remove outer then inner, restore in the opposite order
        private TestResult RemoveOuterRemoveInnerRestoreInnerRestoreOuter()
        {
            _outerDictionary.Remove(_implicitKey);
            Verify("Inner");

            _innerDictionary.Remove(_implicitKey);
            Verify(null);

            _innerDictionary[_implicitKey] = _innerTemplate;
            Verify("Inner");

            _outerDictionary[_implicitKey] = _outerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        // replace inner template
        private TestResult ReplaceInner()
        {
            _innerDictionary[_implicitKey] = _replacementTemplate;
            Verify("Replacement");

            _innerDictionary[_implicitKey] = _innerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        // replace outer tempate - no effect (hidden by inner)
        private TestResult ReplaceOuter()
        {
            _outerDictionary[_implicitKey] = _replacementTemplate;
            Verify("Inner");

            _outerDictionary[_implicitKey] = _outerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        // add a merged dictionary with an implicit template
        private TestResult AddMergedDictionary()
        {
            ResourceDictionary merged = new ResourceDictionary();
            merged[_implicitKey] = _replacementTemplate;

            _innerDictionary.Remove(_implicitKey);
            Verify("Outer");

            _innerDictionary.MergedDictionaries.Add(merged);
            Verify("Replacement");

            _innerDictionary.MergedDictionaries.Remove(merged);
            Verify("Outer");

            _innerDictionary[_implicitKey] = _innerTemplate;
            Verify("Inner");

            return TestResult.Pass;
        }

        private void Verify(string expectedTemplate)
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            foreach (object item in _listBox.Items)
            {
                DependencyObject container = _generator.ContainerFromItem(item);
                if (container != null)
                {
                    Label label = (Label)Util.FindVisualByType(typeof(Label), container);
                    if (expectedTemplate == null)
                    {
                        Util.AssertEquals(label, null);
                    }
                    else
                    {
                        Util.AssertEquals(label.Content, expectedTemplate);
                    }
                }
            }
        }

        #endregion

    }
}

