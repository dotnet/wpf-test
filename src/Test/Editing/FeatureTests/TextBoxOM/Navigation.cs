// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for TextBox in navigation scenarios.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Navigation;

    using Test.Uis.Data;
    using Microsoft.Test.Imaging;
    using Test.Uis.IO;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that a TextBox serializes its Text property as expected
    /// when navigating back and forth between pages, and that it can
    /// restore its state.
    /// </summary>
    /// <remarks>
    /// Test matrix:
    /// - Property to be tested: Text.
    /// - Control to test: all editable controls.
    /// - Initial state of object: populated, empty.
    /// - Modification made to object: true, false.
    /// - Modification made programmatically (as opposed to by typing): true, false.
    /// - Event after which we verify: loading, navigate back, navigate forward (done in all cases).
    /// </remarks>
    [TestOwner("Microsoft"), TestTactics("105"), TestBugs("407,704,705"), TestWorkItem("100, 101")]
    [Test(3, "TextBox", "TextBoxNavigationText", MethodParameters = "/TestCaseType=TextBoxNavigationText /TestName=TextBoxNavigationText-Simple")]
    public class TextBoxNavigationText: ManagedCombinatorialTestCase
    {
        #region Private data.

        /// <summary>Container for navigation contents.</summary>
        private NavigationWindow _container;

        /// <summary>Element being tested.</summary>
        private UIElement _element;

        /// <summary>Count of LoadCompleted events fired.</summary>
        private int _loadCount;

        /// <summary>Sample text to populate tested elements.</summary>
        private const string SampleText = "sample text.";

        /// <summary>Name for element, to refer to it across navigations.</summary>
        private const string ElementName = "ElementName";

        /// <summary>String typed through user actions.</summary>
        private const string UserTypeString = "   ";

        /// <summary>Type of element to test.</summary>
        private TextEditableType _editableType=null;

        /// <summary>Whether the control should be populated before navigating to it.</summary>
        private bool _isInitiallyPopulated=false;

        /// <summary>Whether the content should be modified before navigating from it.</summary>
        private bool _isContentModified=false;

        /// <summary>Whether content should be modified by the user.</summary>
        private bool _isUserModified=false;

        #endregion Private data.

        #region Main flow.

        /// <summary>Reads a combination and determines whether it should run.</summary>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            // If IsContentModified is false, it doesn't matter what value
            // we have for IsUserModified - arbitrarily skip true.
            result = result && !(_isContentModified == false && _isUserModified == true);

            return result;
        }

        /// <summary>Runs the test case combination.</summary>
        protected override void DoRunCombination()
        {
            // Reset field values from previous combinations.
            _loadCount = 0;

            // Create the container for navigation.
            _container = new NavigationWindow();
            _container.Width = 200;
            _container.Height = 200;
            _container.Show();

            // Create the control to be used for navigation.
            _element = _editableType.CreateInstance();

            if (_isInitiallyPopulated)
            {
                new UIElementWrapper(_element).Text = SampleText;
            }
            ((FrameworkElement)_element).Name = ElementName;

            QueueDelegate(AfterLayoutPass);
        }

        private void AfterLayoutPass()
        {
            _container.LoadCompleted += ContainerLoadCompleted;

            Log("Navigating to the object...");
            _container.Navigate(_element);
            _element = null;
            Log("Container content: " + _container.Content);
        }

        private void ContainerLoadCompleted(object sender, NavigationEventArgs args)
        {
            _loadCount++;
            Log("Completed load " + _loadCount);
            Log("Navigated to content " + args.Content);
            switch (_loadCount)
            {
                case 1:
                    AfterFirstLoad();
                    break;
                case 2:
                    AfterSecondLoad();
                    break;
                case 3:
                    AfterGoneBack();
                    break;
                default:
                    throw new Exception("Unexpected load count: " + _loadCount);
            }
        }

        private void AfterFirstLoad()
        {
            UIElement navigatedElement;

            navigatedElement = GetNavigatedElement();

            CheckContent(navigatedElement, _isInitiallyPopulated, false);

            if (_isContentModified)
            {
                if (_isUserModified)
                {
                    // On LoadCompleted, the visual tree has not been hooked up yet.
                    // We can work around that by listening to the Loaded event
                    // on the root, or by directing all commands explicitly.
                    ApplicationCommands.SelectAll.Execute(null, navigatedElement);
                    EditingCommands.Delete.Execute(null, navigatedElement);
                    if (!_isInitiallyPopulated)
                    {
                        KeyboardDevice keyboard;
                        TextCompositionEventArgs textArgs;
                        
                        keyboard = InputManager.Current.PrimaryKeyboardDevice;
                        textArgs = new TextCompositionEventArgs(keyboard,
                            new TextComposition(InputManager.Current, navigatedElement, "   "));
                        textArgs.RoutedEvent = TextCompositionManager.TextInputEvent;
                        navigatedElement.RaiseEvent(textArgs);
                    }
                }
                else
                {
                    new UIElementWrapper(navigatedElement).Text =
                        (_isInitiallyPopulated)? "" : SampleText;
                }
            }
            QueueDelegate(AfterModification);
        }

        private void AfterModification()
        {
            Log("Navigating to the second object...");

            // 
            QueueDelegate(delegate() {
                TextBlock target;
                target = new TextBlock();
                target.Background = System.Windows.Media.Brushes.SkyBlue;
                target.Text = "TextBlock in second navigation";
                _container.Navigate(target);
            });
        }

        private void AfterSecondLoad()
        {
            Log("Second page loaded.");
            Log("CanGoBack: " + _container.CanGoBack);

            Log("Navigating back...");
            
            QueueDelegate(delegate() {
                _container.GoBack();
            });
        }

        private void AfterGoneBack()
        {
            UIElement naviagatedElement;

            Log("Navigated back.");

            naviagatedElement = GetNavigatedElement();

           CheckContent(naviagatedElement, (_isContentModified) ?
                 !_isInitiallyPopulated : _isInitiallyPopulated, _isUserModified);

            _container.Close();

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Helper methods.

        private void CheckContent(UIElement element, bool isPopulatedExpected,
            bool wasUserModified)
        {
            string text;

            text = GetElementText(element);
            if ((element is PasswordBox)&&(_loadCount >1))
            {
                Verifier.Verify(text.Length ==0, "Text is not retained for passwordBox Expected:[] Actual: [" + text + "]", true);
            }
            else
            if (isPopulatedExpected)
            {
                Verifier.Verify(text.Length > 2, "Text is populated: [" + text + "]", true);
                if (wasUserModified)
                {
                    Verifier.Verify(text.Trim().Length == 0, "Text is all whitespace after user modification: [" + text + "]",true);
                }
            }
            else
            {
                Verifier.Verify(text.Length <= 2, "Text is not populated: [" + text + "]",true);
            }
        }

        private static string GetElementText(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return new UIElementWrapper(element).Text;
        }

        /// <summary>
        /// Gets an element in the navigation container given its Name.
        /// </summary>
        /// <returns>The element with the given Name.</returns>
        private UIElement GetNavigatedElement()
        {
            Log("Looking for element with Name=" + ElementName + "...");
            UIElement result = (UIElement) LogicalTreeHelper.FindLogicalNode(
                (DependencyObject)_container.Content, ElementName);
            if (result == null)
            {
                throw new Exception("Element with Name=" + ElementName + " not found.");
            }
            return result;
        }

        #endregion Helper methods.
    }
}
