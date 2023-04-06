// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  NavigateToObject tests the ability to navigate to 
//  display-able objects (i.e. Canvas with contents, styled 
//  FrameworkContentElement, valid XAML), non-displayable objects
//  (i.e. String and unstyled FrameworkContentElement) that require
//  the use of ToString, and ambiguous objects (i.e. Image not contained
//  in a Panel, Canvas, etc.)
//
//  Step1 - Navigate to new XAML page (displayable content)
//  Step2 - Navigate to String (ToString should get called in this case)
//  Step3 - Navigate to Image (not contained in a Panel or Canvas)
//  Step4 - Navigate to a displayable object (MyCustomSeussPage created programmatically)
//  Step5 - Navigate to an unstyled FrameworkContentElement
//  Step6 - Navigate to a styled FrameworkContentElement
//

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Markup;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class NavigateToObject
    {
        private enum NavigatedTo // identifies the destination of the navigation
        {
            Start,
            String,
            Displayable_XAML,
            Displayable_Object,
            Image, 
            UnstyledFrameworkContentElement,
            StyledFrameworkContentElement,
            End
        }

        private const String              TEST_STRING = "Peter Piper picked a peck of pickled peppers";
        private const String              XAML_TEXTBOX_CONTENTS = "Did you ever fly a kite in bed? Did you ever walk with ten cats on your head?";
        private const String              OBJECT_TEXTBOX_CONTENTS = "One fish, two fish, red fish, blue fish.\nBlack fish, blue fish, old fish, new fish.";
        private Image                     _customImg = null;
        private FrameworkContentElement   _myTestFCE= null;
        private PageContent		          _myTestFCEPageContent = null;
        private bool                      _homePage = true;
        private NavigatedTo               _destination = NavigatedTo.Start;
        private TreeUtilities             _treeUtilities = null; // TreeUtilities object in NavigationTestLibrary

        /// <summary>
        /// Constructor
        /// </summary>
        public NavigateToObject()
        {
        }

        /// <summary>
        /// Sets up the AutomationFramework in NavigationHelper, and 
        /// registers eventhandlers for LoadCompleted and Navigated events.
        /// </summary>
        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigateToObject");
            Application.Current.StartupUri = new Uri("NavigateToObject_homePage.xaml", UriKind.RelativeOrAbsolute);

            // event counts
            NavigationHelper.NumExpectedNavigatedEvents = 6;
            NavigationHelper.NumActualNavigatedEvents = 0;
            NavigationHelper.NumExpectedLoadCompletedEvents = 7;
            NavigationHelper.NumActualLoadCompletedEvents = 0;

            // Start off by navigating to a valid page with displayable content.
            NavigationHelper.Output("Starting NavigateToObject BVTs.");
            NavigationHelper.SetStage(TestStage.Run);
            _destination = NavigatedTo.Displayable_XAML;

            _treeUtilities = new TreeUtilities();
        }


        /// <summary>
        /// Try to Navigate(...) to different objects (String, displayable XAML, displayable
        /// object tree, styled FrameworkContentElement, unstyled FrameworkContentElement, 
        /// Image not contained in a container.  Navigation starts from valid XAML homepage.
        /// </summary>
        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            // Grab the NavigationWindow that we'll be navigating in
            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            bool navigated;

            if (nw == null)
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }

            NavigationHelper.NumActualLoadCompletedEvents++;
            switch (_destination)
            {
                // [1] Navigate to new XAML page (displayable content)
                case NavigatedTo.Displayable_XAML:
                    NavigationHelper.Output("Navigate to a new XAML page");
                    navigated = nw.Navigate(new Uri("NavigateToObject_displayable.xaml", UriKind.RelativeOrAbsolute));
                    if (!navigated)
                    {
                        NavigationHelper.Fail("Navigation failed");
                    }
                    _homePage = false;
                    break;

                // [2] Navigate to String (ToString should get called in this case)
                case NavigatedTo.String:
                    NavigationHelper.Output("Navigate to a String object");
                    navigated = nw.Navigate(TEST_STRING);
                    if (!navigated)
                    {
                        NavigationHelper.Fail("Navigation failed");
                    }
                    break;

                // [3] Navigate to Image (not contained in a Panel or Canvas)
                case NavigatedTo.Image:
                    NavigationHelper.Output("Navigate to an Image object");
                    _customImg = new Image();
                    _customImg.Name = "CustomImage";
                    _customImg.Source = new BitmapImage(new Uri("NavigateToObject_joe.gif", UriKind.RelativeOrAbsolute));
                    navigated = nw.Navigate(_customImg);
                    if (!navigated)
                    {
                        NavigationHelper.Fail("Navigation failed");
                    }
                    break;

                // [4] Navigate to a displayable object (MyCustomSeussPage created programmatically)
                case NavigatedTo.Displayable_Object:
                    NavigationHelper.Output("Navigate to a displayable object");
                    navigated = nw.Navigate(new MyCustomSeussPage());
                    if (!navigated)
                    {
                        NavigationHelper.Fail("Navigation failed");
                    }
                    break;

                // [5] Navigate to an unstyled FrameworkContentElement
                case NavigatedTo.UnstyledFrameworkContentElement:
                    NavigationHelper.Output("Navigate to an unstyled FrameworkContentElement");
                    _myTestFCE = new FrameworkContentElement();
                    navigated = nw.Navigate(_myTestFCE);
                    if (!navigated)
                    {
                        NavigationHelper.Fail("Navigation failed");
                    }
                    break;

                // [6] Navigate to a styled FrameworkContentElement
                case NavigatedTo.StyledFrameworkContentElement:
                    NavigationHelper.Output("Navigate to a styled FrameworkContentElement");
                    BuildStyledFrameworkContentElement();
                    navigated = nw.Navigate(_myTestFCE);
                    if (!navigated)
                    {
                        NavigationHelper.Fail("Navigation failed");
                    }
                    break;

                // If we've gone through all the sub-tests, log results, wrap up the BVT
                // and shut down the app.
                case NavigatedTo.End:
                    NavigationHelper.FinishTest(true);
                    break;

                default:
                    NavigationHelper.Fail("Not a valid BVT test.  Ending BVT.");
                    break;
            }
        }


        /// <summary>
        /// After the navigation has completed, check if the contents were displayed
        /// by walking the logical/visual tree or if the non-styled object was displayed
        /// as a String (by walking the logical/visual tree in search of the object.ToString()
        /// string).
        /// </summary>
        public void Navigated(object source, NavigationEventArgs e)
        {
            if (!_homePage)
            {
                NavigationHelper.NumActualNavigatedEvents++;
                switch (_destination)
                {
                    // [1] Navigate to displayable XAML
                    // Walk logical tree and verify it matches our expected tree structure/content
                    case NavigatedTo.Displayable_XAML:
                        CheckForDisplayableTarget(e.Navigator);
                        _destination = NavigatedTo.String;
                        break;

                    // [2] Navigate to String
                    // Check that ToString was called and string can be found in the logical tree
                    case NavigatedTo.String:
                        CheckForTarget(e.Navigator, TEST_STRING);
                        _destination = NavigatedTo.Image;
                        break;

                    // [3] Navigate to uncontained Image
                    // Check that Image can be found in the logical tree
                    case NavigatedTo.Image:
                        CheckForTarget(e.Navigator, _customImg);
                        _destination = NavigatedTo.Displayable_Object;
                        break;

                    // [4] Navigate to displayable object tree
                    // Walk logical tree and verify it matches our expected tree structure/content
                    case NavigatedTo.Displayable_Object:
                        CheckObjectTree(e.Navigator);
                        _destination = NavigatedTo.UnstyledFrameworkContentElement;
                        break;

                    // [5] Navigate to unstyled FrameworkContentElement
                    // Check that same unstyled FrameworkContentElement can be found in logical tree
                    case NavigatedTo.UnstyledFrameworkContentElement:
                        CheckForTarget(e.Navigator, _myTestFCE);
                        _destination = NavigatedTo.StyledFrameworkContentElement;
                        break;

                    // [6] Navigate to a styled FrameworkContentElement
                    // Walk the logical tree and verify it matches our expected tree structure/content
                    case NavigatedTo.StyledFrameworkContentElement:
                        CheckForDisplayableTarget(e.Navigator);
                        _destination = NavigatedTo.End;
                        break;
                }
            }
        }


        /// <summary>
        /// Checks if the Content property of the NavigationWindow is the object that
        /// we want to navigate to.
        /// </summary>
        private void CheckForTarget(object source, object target)
        {
            NavigationWindow nw = source as NavigationWindow;

            if (nw == null)
            {
                NavigationHelper.Fail("Navigation did not occur from NavigationWindow");
            }

            if (target == null)
            {
                NavigationHelper.Fail("Cannot navigate to a null target object");
            }

            if (!nw.Content.Equals(target))
            {
                NavigationHelper.Fail("NavigationWindow.Content is not the " + target.ToString() + " we navigated to");
            }
            else
            {
                NavigationHelper.Output("Found the expected content in destination " + _destination.ToString());
            }
        }


        /// <summary>
        /// Searches the logical tree to see if we can find:
        /// [1] If test is Displayable_XAML,
        ///     TextBox containing the expected text, starting from NavigationWindow and FlowDocument
        /// [2] If test is StyledFrameworkContentElement, 
        ///     FixedDocument containing the expected text
        /// </summary>
        private void CheckForDisplayableTarget(object source)
        {
            NavigationWindow nw = source as NavigationWindow;
            if (nw == null)
            {
                NavigationHelper.Fail("Navigation did not occur from NavigationWindow");
            }

            if (_destination.Equals(NavigatedTo.Displayable_XAML))
            {
                FlowDocument myFD = nw.Content as FlowDocument;
                if (myFD == null)
                {
                    NavigationHelper.Fail("Could not find FlowDocument in XAML-defined logical tree");
                }

                TextBox myTxt = _treeUtilities.TraverseLogicalTree("DisplayableFlowText", myFD) as TextBox;
                if (myTxt == null || !myTxt.Text.Equals(XAML_TEXTBOX_CONTENTS))
                {
                    NavigationHelper.Fail("Did not find and display the correct controls from XAML");
                }
                else
                {
                    NavigationHelper.Output("Found text in traversing the logical tree - " + myTxt);
                }
            }
            else if (_destination.Equals(NavigatedTo.StyledFrameworkContentElement))
            {
                FixedDocument myFixedDoc = nw.Content as FixedDocument;
                if (myFixedDoc == null)
                {
                    NavigationHelper.Fail("Could not find FixedDocument in logical tree");
                }

                if (!myFixedDoc.Equals((FixedDocument)_myTestFCE))
                {
                    NavigationHelper.Fail("Did not find and display the correct styled FrameworkContentElement");
                }
                else
                {
                    NavigationHelper.Output("Found the FixedDocument content in destination " + _destination.ToString());
                }
            }
        }


        /// <summary>
        /// Searches the logical tree to see if we can find:
        /// [1] TextBox containing the expected text, starting from NavigationWindow and Canvas
        /// </summary>
        private void CheckObjectTree(object source)
        {
            NavigationWindow nw = source as NavigationWindow;
            Canvas myCanvas = nw.Content as Canvas;

            if (nw == null)
            {
                NavigationHelper.Fail("Navigation did not occur from NavigationWindow");
            }

            if (myCanvas == null)
            {
                NavigationHelper.Fail("Navigate to object:  NavWindow's content should be MyCustomSeussPage");
            }

            // Walk through the visual tree to find TextBox
            TextBox myTB = _treeUtilities.FindVisualTreeElementByID("SeussTextBox", myCanvas) as TextBox;
            if (!myTB.Text.Equals(OBJECT_TEXTBOX_CONTENTS))
            {
                NavigationHelper.Fail("Did not find and display the correct object tree");
            }
            else
            {
                NavigationHelper.Output("Found the text box content " + myTB.Text);
            }
        }

        /// <summary>
        /// Build a FixedDocument containing a single PageContent with a single FixedPage with text.
        /// A Fixed- or FlowDocument is an example of a styled FrameworkContentElement.
        /// </summary>
        private void BuildStyledFrameworkContentElement()
        {
            _myTestFCE = new FixedDocument();
            _myTestFCEPageContent = new PageContent();

            if (_myTestFCE == null || _myTestFCEPageContent == null)
            {
                NavigationHelper.Fail("Could not create new FixedDocument");
                _destination = NavigatedTo.End;
                return;
            }

            // Direct PageContent to XAML file containing FixedPage
            _myTestFCEPageContent.Source = new Uri("NavigateToObject_fixedDocContent.xaml", UriKind.RelativeOrAbsolute);

            // Add PageContent to FixedDocument
            ((IAddChild)_myTestFCE).AddChild(_myTestFCEPageContent);
        }
    }


    /// <summary>
    /// This class constructs an object tree that will be used in a sub-part
    /// of the BVT (try to navigate to a displayable object tree) 
    /// </summary>
    public class MyCustomSeussPage : Canvas
    {
        public MyCustomSeussPage() : base()
        {
            Log.Current.CurrentVariation.LogMessage("Inside MyCustomSeussPage constructor");
            this.Background = Brushes.SlateBlue;
            TextBox tb = new TextBox();
            tb.Name = "SeussTextBox";
            tb.Text = "One fish, two fish, red fish, blue fish.\n";
            tb.Text += "Black fish, blue fish, old fish, new fish.";
            tb.IsReadOnly = true;
            tb.TextWrapping = TextWrapping.Wrap;
            this.Children.Add(tb);
            Log.Current.CurrentVariation.LogMessage("Exiting MyCustomSeussPage constructor");
        }
    }
}

