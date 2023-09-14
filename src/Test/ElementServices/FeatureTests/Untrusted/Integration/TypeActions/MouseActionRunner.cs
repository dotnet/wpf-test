// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Windows;


namespace Avalon.Test.CoreUI.Integration.TypeActions
{
    /******************************************************************************
    * CLASS:          MouseActionRunner
    ******************************************************************************/
    /// <summary>
    /// Implements routines for testing Mouse actions for all types
    ///</summary>
    [Test(1, "Integration.MouseAction", "MouseActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\MouseActionIntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
    public class MouseActionRunner : WindowTest
    {
        #region Private Data
        /// <summary>
        /// List of events with order for each mouse action - used to verify mouse events fired by FrameworkElement
        /// </summary>
        private string[] _mouseMoveIn = new string[] { "MouseEnter", "MouseMove"};
        private string[] _mouseDown   = new string[] { "PreviewMouseDown", "PreviewMouseDown", "MouseDown", "MouseDown" };
        private string[] _mouseUp     = new string[] { "PreviewMouseUp", "PreviewMouseUp", "MouseUp", "MouseUp" };
        private string[] _mouseLeave  = new string[] { "MouseMove", "MouseLeave" };
        private string[] _mouseWheel  = new string[] { "PreviewMouseWheel", "MouseWheel" };
        
        /// <summary>
        /// List of events expected - used for verifying testing mouse events fired by FrameworkContentElement
        /// Not checking order of events
        /// </summary>
        private string[] _allExpectedEvents = new string[] {"MouseEnter", "MouseMove", "PreviewMouseDown", "MouseDown", "PreviewMouseUp", "MouseUp", "MouseLeave","PreviewMouseWheel", "MouseWheel"};

        private ArrayList        _eventArgs = new ArrayList();    //Store mouse events
        private bool             _isExpectedAnyEvents = true;     //Are any events expected; if the element is disabled or hidden no events are expected
        private bool             _isFCETest = false;  
        private object           _root;                           //Root of the XAML tree
        private Button           _button;                         //To move mouse of test element to this button
        private string           _testElementTypeName;
        private DependencyObject _testElement; 
        private Window           _mainWindow;
        private string           _testElementName = "elementWithInitialValues";
        private string           _rootForElementWithInitialValues = "RootForElementWithInitialValues";
        private bool             _finalResult = false;
        private DispatcherSignalHelper _dispatcherSignalHelper = null;
        private XamlTestDocument                 _xamlTestDocument        = null;
        private string                           _typeName                = "";
        #endregion

        #region Constructors

        [Variation("AccessText")]
        [Variation("AdornerDecorator")]
        [Variation("Border")]
        [Variation("BulletDecorator")]
        [Variation("Button")]
        [Variation("Canvas")]
        [Variation("CheckBox")]
        [Variation("ComboBox")]
        [Variation("ComboBoxItem")]
        [Variation("ContentControl")]
        [Variation("ContentPresenter")]
        [Variation("Control")]
        [Variation("Decorator")]
        [Variation("DockPanel")]
//        [Variation("DocumentViewer")]
        [Variation("Ellipse")]
        [Variation("Expander")]
        [Variation("Figure")]
        [Variation("FixedPage")]
        [Variation("Floater")]
        [Variation("FlowDocument")]
        [Variation("FlowDocumentReader")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Frame")]
        [Variation("Grid")]
        [Variation("GridSplitter")]
        [Variation("GridViewColumnHeader")]
        [Variation("GroupBox")]
        [Variation("HeaderedItemsControl")]
        [Variation("InkPresenter")]
        [Variation("ItemsControl")]
        [Variation("Label")]
        [Variation("List")]
        [Variation("ListBox")]
        [Variation("ListBoxItem")]
        [Variation("ListItem")]
        [Variation("ListView")]
        [Variation("ListViewItem")]
        [Variation("Menu")]
        [Variation("MenuItem")]
        [Variation("Paragraph")]
        [Variation("PasswordBox")]
        [Variation("ProgressBar")]
        [Variation("RadioButton")]
        [Variation("RepeatButton")]
        [Variation("Rectangle")]
        [Variation("ResizeGrip")]
        [Variation("RichTextBox")]
        [Variation("Section")]
        [Variation("ScrollBar")]
        [Variation("ScrollContentPresenter")]
        [Variation("ScrollViewer")]
        [Variation("Section")]
        [Variation("Slider")]
        [Variation("StackPanel")]
        [Variation("StatusBar")]
        [Variation("StatusBarItem")]
        [Variation("TabItem")]
//        [Variation("Table")]
        [Variation("TableRow")]
        [Variation("TableRowGroup")]
        [Variation("TabPanel")]
        [Variation("TextBlock")]
        [Variation("TextBox")]
        [Variation("Thumb")]
        [Variation("ToggleButton")]
        [Variation("ToolBarOverflowPanel")]
        [Variation("ToolBar")]
        [Variation("ToolBarPanel")]
        [Variation("TreeView")]
        [Variation("UniformGrid")]
        [Variation("UserControl")]
        [Variation("VirtualizingStackPanel")]
        [Variation("WrapPanel")]

        /******************************************************************************
        * Function:          MouseActionRunner Constructor
        ******************************************************************************/
        public MouseActionRunner(string arg)
        {
            _typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoMouseActionTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns>A TestResult</returns>
        TestResult Initialize()
        {
            _typeName = ActionHelper.GetFullName(_typeName);
            ActionHelper.XamlFileName = "__MouseActionForType.xaml";
            ActionHelper.EmptyFileName= "MouseActionForType_empty.xaml";
            ActionHelper.snippetsFileName= "MouseActionIntegrationXamlSnippets.xaml";
            _xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          DoMouseActionTest
        ******************************************************************************/
        /// <summary>
        /// Get XAML for the type, run different mouse actions and verify 
        /// events are fired correctly
        /// </summary>
        TestResult DoMouseActionTest()
        {
            Type type = ActionHelper.GetType(_typeName, true);

            _dispatcherSignalHelper = new DispatcherSignalHelper();

            _testElementTypeName = _typeName;

            BuildTemplateMouseActionXaml(_typeName, _xamlTestDocument);
            GlobalLog.LogFile(ActionHelper.XamlFileName);

            // Parse xaml
            _root = SerializationHelper.ParseXamlFile(ActionHelper.XamlFileName);

            GlobalLog.LogStatus("xaml loaded. ");

            _mainWindow = null;
            _mainWindow = new Window();
            _mainWindow.ContentRendered -= new EventHandler(MainWindow_ContentRendered);            
            _mainWindow.Content = null;
            _mainWindow.Content = _root;
            _mainWindow.ContentRendered += new EventHandler(MainWindow_ContentRendered);            
            _mainWindow.Closed += new EventHandler(MainWindow_Closed);            
            _mainWindow.Show();

            _dispatcherSignalHelper.WaitForSignal("TestFinished", 600000);

            if (!_finalResult)
            {
                throw new Microsoft.Test.TestValidationException("FAIL: MouseAction test of " + _typeName);
            }

            //"Restore" mouse position to 0,0, to prevent a current mouse move affecting later cases.
            MouseHelper.MoveOnPrimaryMonitor();

            if (_finalResult)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          GetStyleForTypeFromCode
        ******************************************************************************/
        /// <summary>
        /// Create and return style for the specified type
        /// Style definition is hard coded and same for all types derived from Control
        /// </summary>
        /// <returns></returns>
        private Style GetStyleForTypeFromCode(Type controlType, double rectSize, Color color)
        {
            Style style = new Style(controlType, null);

            FrameworkElementFactory rectangle = new FrameworkElementFactory(typeof(Rectangle), "styleRootVisual");

            rectangle.SetValue(Control.WidthProperty, rectSize);
            rectangle.SetValue(Control.HeightProperty, rectSize);
            rectangle.SetValue(Shape.FillProperty, new SolidColorBrush(color));

            ControlTemplate template = new ControlTemplate(controlType);
            template.VisualTree = rectangle;
            style.Setters.Add(new Setter(Control.TemplateProperty, template));

            return style;
        }

        /******************************************************************************
        * Function:          MainWindow_ContentRendered
        ******************************************************************************/
        /// <summary>
        /// Launches the test.
        /// </summary>
        /// <returns></returns>
        private void MainWindow_ContentRendered(object sender, EventArgs args )
        {
            GlobalLog.LogStatus("---mainWindow ContentRendered---");

            bool testContinue = true;

            _button = new Button();

            if (!(_root is Panel))
            {
                GlobalLog.LogStatus("Expected: Root element Panel");
                GlobalLog.LogStatus("Actual: Root element: " + _root.GetType().ToString());
                testContinue = false;
            }

            //Element under test
            _testElement = (_root as FrameworkElement).FindName(_testElementName) as DependencyObject;

            //If not found, try to find it in VisualTree
            if (_testElement == null)
            {
                _testElement = FindElement((_root as DependencyObject), _testElementName);
            }

            if (_testElement == null)
            {
                GlobalLog.LogStatus("Unable to find test element with name elementWithInitialValues under root element");
                testContinue = false;
            }

            if (!_testElement.GetType().ToString().Equals(_testElementTypeName))
            {
                GlobalLog.LogStatus("Expected test element type: " + _testElementTypeName);
                GlobalLog.LogStatus("Actual test element type: " + _testElement.GetType().ToString());
                testContinue = false;
            }

            if (!testContinue)
            {
                RegisterResult(false, null);
                return;
            }

            (_root as Panel).Children.Add(_button);
            SetTestElement(_testElement);

            DispatcherHelper.DoEvents(1000);

            //To make sure styles are applied
            ApplyTemplate(_testElement);
            DispatcherHelper.DoEvents(1000);

            SetPopupNull();

            AttachMouseEvents(_testElement);  //The testElement can be either a FrameworkElement or a FrameworkContentElement.

            _mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(TestMouseActions), _testElement);
        }

        /******************************************************************************
        * Function:          MainWindow_Closed
        ******************************************************************************/
        /// <summary>
        /// Fires when the MainWindow is closed, finishing the test case.
        /// </summary>
        /// <returns></returns>
        private void MainWindow_Closed(object sender, EventArgs args )
        {
            GlobalLog.LogStatus("---mainWindow Closed---");
            _dispatcherSignalHelper.Signal("TestFinished", TestResult.Pass);
        }

        /******************************************************************************
        * Function:          ApplyTemplate
        ******************************************************************************/
        //Call ApplyTemplate to ensure Style applied
        private void ApplyTemplate(DependencyObject element)
        {
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
        }

        /******************************************************************************
        * Function:          SetPopupNull
        ******************************************************************************/
        //Set ToolTip/ContextMenu of all elements to null since that may interfere with the test
        private void SetPopupNull()
        {
            if ((_testElement is ToolTip) || (_testElement is ContextMenu))
                return;

            ArrayList arrList = new ArrayList();

            GetElements(_root as DependencyObject, arrList);

            foreach (DependencyObject dep in arrList)
            {
                if (dep is FrameworkElement)
                {
                    (dep as FrameworkElement).ContextMenu = null;
                    (dep as FrameworkElement).ToolTip = null;
                }
                else
                {
                    (dep as FrameworkContentElement).ContextMenu = null;
                    (dep as FrameworkContentElement).ToolTip = null;
                }
            }
        }

        /******************************************************************************
        * Function:          AttachMouseEvents
        ******************************************************************************/
        /// <summary>
        /// Attach event handlers for various mouse events on UIElement
        /// </summary>
        /// <param name="testElement"></param>
        private void AttachMouseEvents(DependencyObject testElement)
        {
            GlobalLog.LogStatus("Attaching mouse events " + testElement.ToString());

            if (testElement is FrameworkContentElement)
            {
                FrameworkContentElement targetFCE = (FrameworkContentElement)testElement;
                targetFCE.MouseLeftButtonDown          += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseLeftButtonUp            += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseDown                    += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseEnter                   += new MouseEventHandler(GenericEventHandler);
                targetFCE.MouseLeave                   += new MouseEventHandler(GenericEventHandler);
                targetFCE.MouseMove                    += new MouseEventHandler(GenericEventHandler);
                targetFCE.MouseRightButtonDown         += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseRightButtonUp           += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseUp                      += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseWheel                   += new MouseWheelEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseLeftButtonDown   += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseLeftButtonUp     += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseDown             += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseRightButtonDown  += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseRightButtonUp    += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseUp               += new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseWheel            += new MouseWheelEventHandler(GenericEventHandler);
            }
            else
            {
                UIElement targetUI = (UIElement)testElement;
                targetUI.MouseLeftButtonDown          += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseLeftButtonUp            += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseDown                    += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseEnter                   += new MouseEventHandler(GenericEventHandler);
                targetUI.MouseLeave                   += new MouseEventHandler(GenericEventHandler);
                targetUI.MouseMove                    += new MouseEventHandler(GenericEventHandler);
                targetUI.MouseRightButtonDown         += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseRightButtonUp           += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseUp                      += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseWheel                   += new MouseWheelEventHandler(GenericEventHandler);
                targetUI.PreviewMouseLeftButtonDown   += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseLeftButtonUp     += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseDown             += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseRightButtonDown  += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseRightButtonUp    += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseUp               += new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseWheel            += new MouseWheelEventHandler(GenericEventHandler);
            }
        }

        /******************************************************************************
        * Function:          DetachMouseEvents
        ******************************************************************************/
        /// <summary>
        /// Detach event handlers for various mouse events on UIElement
        /// </summary>
        /// <param name="testElement"></param>
        private void DetachMouseEvents(DependencyObject testElement)
        {
            GlobalLog.LogStatus("Detaching mouse events " + testElement.ToString());

            if (testElement is FrameworkContentElement)
            {
                FrameworkContentElement targetFCE = (FrameworkContentElement)testElement;
                targetFCE.MouseLeftButtonDown          -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseLeftButtonUp            -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseDown                    -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseEnter                   -= new MouseEventHandler(GenericEventHandler);
                targetFCE.MouseLeave                   -= new MouseEventHandler(GenericEventHandler);
                targetFCE.MouseMove                    -= new MouseEventHandler(GenericEventHandler);
                targetFCE.MouseRightButtonDown         -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseRightButtonUp           -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseUp                      -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.MouseWheel                   -= new MouseWheelEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseLeftButtonDown   -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseLeftButtonUp     -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseDown             -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseRightButtonDown  -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseRightButtonUp    -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseUp               -= new MouseButtonEventHandler(GenericEventHandler);
                targetFCE.PreviewMouseWheel            -= new MouseWheelEventHandler(GenericEventHandler);
            }
            else
            {
                UIElement targetUI = (UIElement)testElement;
                targetUI.MouseLeftButtonDown          -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseLeftButtonUp            -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseDown                    -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseEnter                   -= new MouseEventHandler(GenericEventHandler);
                targetUI.MouseLeave                   -= new MouseEventHandler(GenericEventHandler);
                targetUI.MouseMove                    -= new MouseEventHandler(GenericEventHandler);
                targetUI.MouseRightButtonDown         -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseRightButtonUp           -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseUp                      -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.MouseWheel                   -= new MouseWheelEventHandler(GenericEventHandler);
                targetUI.PreviewMouseLeftButtonDown   -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseLeftButtonUp     -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseDown             -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseRightButtonDown  -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseRightButtonUp    -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseUp               -= new MouseButtonEventHandler(GenericEventHandler);
                targetUI.PreviewMouseWheel            -= new MouseWheelEventHandler(GenericEventHandler);
            }
        }

        /******************************************************************************
        * Function:          GenericEventHandler
        ******************************************************************************/
        /// <summary>
        /// Generic event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenericEventHandler(object sender, RoutedEventArgs e)
        {
            if ( sender != _testElement )
                return;
            //GlobalLog.LogStatus("Sender: " + sender.ToString() + " /Event: " + e.RoutedEvent);
            _eventArgs.Add(e);
        }

        /******************************************************************************
        * Function:          TestMouseActions
        ******************************************************************************/
        /// <summary>
        /// Test Mouse action
        /// For FrameworkElement, mouse actions are repeated after setting
        /// IsEnabled=false, Visibility=Hidden
        /// </summary>
        private object TestMouseActions(object arg)
        {
            bool testResult = true;

            _eventArgs.Clear();

            GlobalLog.LogStatus("TestMouseActions");

            FrameworkElement target = null;

            Panel rootForTestElementPanel = (_root as FrameworkElement).FindName(_rootForElementWithInitialValues) as Panel;

            if (rootForTestElementPanel == null )
            {
              GlobalLog.LogStatus("Unable to find Panel with Name: " + rootForTestElementPanel);
              RegisterResult(false, null);
              return null;
            }

            if (arg is FrameworkContentElement)
            {
                (arg as FrameworkContentElement).IsEnabled = true;
                //For FCE, target topmost FE Container
                if (rootForTestElementPanel.Children.Count > 0)
                  target = (rootForTestElementPanel.Children[0]) as FrameworkElement;
                _mainWindow.FontSize = 100.0;
                _isFCETest = true;
            }
            else
            {
                (arg as FrameworkElement).IsEnabled = true;
                target = arg as FrameworkElement;
            }

            if (target == null )
            {
              GlobalLog.LogStatus("Target TestElement is null");
              RegisterResult(false, null);
              return null;
            }

            GlobalLog.LogStatus(target.ToString() + " is the Target");

            if (((target.ActualHeight < 10) || (target.ActualWidth < 10)) && (target is Control))
            {
                GlobalLog.LogStatus("Apply style");
                target.Style = GetStyleForTypeFromCode(target.GetType(), 100.0, Colors.Red);
                target.ApplyTemplate();
                DispatcherHelper.DoEvents(1000);
            }

            //DocumentViewer, FlowDocumentReader if put inside a Panel of
            //size 100X100, no events fired, so this work around
            if (!((arg is DocumentViewer) || (arg is FlowDocumentReader) || (arg is PageContent)))
            {
              target.Height = 100.0;
              target.Width = 100.0;
            }
            else
            {
              rootForTestElementPanel.Height = 600.0;
              rootForTestElementPanel.Width = 800.0;
            }

            if (!target.IsHitTestVisible)
            {
                GlobalLog.LogStatus("Target IsHitTestVisible= false, setting to true" );
                target.IsHitTestVisible = true;
            }

            target.UpdateLayout();
            DispatcherHelper.DoEvents(1000);

            _isExpectedAnyEvents = true;

            testResult = PerformMouseActions(target);

            //FrameworkContentElement, we are not doing test after setting the element to disable/hidden 
            if (arg is FrameworkContentElement)
            {
                RegisterResult(testResult, target);
                return null;
            }
            else
            {
                GlobalLog.LogStatus("Set " + arg.ToString() + " IsEnabled=false");
                (arg as FrameworkElement).IsEnabled = false;
            }

            DispatcherHelper.DoEvents(1000);

            //Perform MouseActions with disabled element
            _isExpectedAnyEvents = false;
            testResult &= PerformMouseActions(target);

            GlobalLog.LogStatus("Reset test element IsEnabled=true");
            (arg as FrameworkElement).IsEnabled = true;

            target.Visibility = Visibility.Hidden;
            DispatcherHelper.DoEvents(1000);
            GlobalLog.LogStatus(target.ToString() + " set Visibility=Hidden");

            //Perform MouseActions with Hidden element
            _isExpectedAnyEvents = false;
            testResult &= PerformMouseActions(target);

            RegisterResult(testResult, (DependencyObject)arg);

            return null;
        }

        /******************************************************************************
        * Function:          RegisterResult
        ******************************************************************************/
        /// <summary>
        /// Register test result with GlobalLog, detach events, and close the main window
        /// </summary>
        private void RegisterResult(bool result, DependencyObject elementTested)
        {
            _finalResult = result;

            if (elementTested == null )
            {
                DetachMouseEvents(elementTested);
            }

            _mainWindow.Close();
        }

        /******************************************************************************
        * Function:          PerformMouseActions
        ******************************************************************************/
        /// <summary>
        /// Perform MouseActions
        /// </summary>
        private bool PerformMouseActions(FrameworkElement target)
        {
            bool testResult = true;

            _eventArgs.Clear();
           
            GlobalLog.LogStatus("Perform MouseActions");

            GlobalLog.LogStatus("Target :" + target.ToString());

            MouseHelper.Move(target, MouseLocation.Center);
            DispatcherHelper.DoEvents(1000);
            testResult = VerifyMouseEvents(_mouseMoveIn, "MouseMoveIn");
            DispatcherHelper.DoEvents(100);

            MouseHelper.PressButton(MouseButton.Left);
            DispatcherHelper.DoEvents(200);
            testResult &= VerifyMouseEvents(_mouseDown, "LeftMouseButtonDown");

            MouseHelper.ReleaseButton(MouseButton.Left);
            DispatcherHelper.DoEvents(200);
            testResult &= VerifyMouseEvents(_mouseUp, "LeftMouseButtonUp");

            MouseHelper.PressButton(MouseButton.Right);
            DispatcherHelper.DoEvents(200);
            testResult &= VerifyMouseEvents(_mouseDown, "RightMouseButtonDown");

            MouseHelper.ReleaseButton(MouseButton.Right);
            DispatcherHelper.DoEvents(200);
            testResult &= VerifyMouseEvents(_mouseUp, "RightMouseButtonUp");

            MouseHelper.MoveWheel(10);
            DispatcherHelper.DoEvents(200);
            testResult &= VerifyMouseEvents(_mouseWheel, "MouseWheel");

            MouseHelper.Move(_button, MouseLocation.Center);
            DispatcherHelper.DoEvents(200);
            testResult &= VerifyMouseEvents(_mouseLeave, "MouseMoveOut");

            //If element under test is of type FrameworkContentElement, then verification done
            //at the end
            if (_isFCETest)
                testResult = VerifyMouseEvents();

            return testResult;
        }

        /******************************************************************************
        * Function:          VerifyMouseEvents
        ******************************************************************************/
        /// <summary>
        /// FrameworkContentElement event verification
        /// Not able to verify order or number of events
        /// So this verification only check for expected events are fired
        /// </summary>
        private bool VerifyMouseEvents()
        {
            bool bResult = true;

            if (!_isExpectedAnyEvents)
            {
                return VerifyNoEventsFired();
            }

            ArrayList eventList = new ArrayList();

            foreach (RoutedEventArgs  args in _eventArgs)
            {
                eventList.Add(args.RoutedEvent.Name);
            }

            for (int i = 0; i < _allExpectedEvents.Length; i++)
            {
                if (!eventList.Contains(_allExpectedEvents[i]))
                {
                    GlobalLog.LogStatus("Expected  " + _allExpectedEvents[i]);
                    bResult = false;
                }
            }

            if (eventList.Count == 0)
                GlobalLog.LogStatus("No events fired");

            return bResult;
        }

        /******************************************************************************
        * Function:          VerifyNoEventsFired
        ******************************************************************************/
        /// <summary>
        /// Verify no events fired - when element disabled/hidden
        /// </summary>
        private bool VerifyNoEventsFired()
        {
            bool bResult = true;

            GlobalLog.LogStatus("Expected no events");

            if (_eventArgs.Count > 0)
            {
                bResult = false;
                GlobalLog.LogStatus("FAIL: events fired when no events expected");

                foreach (RoutedEventArgs obj in _eventArgs)
                {
                    GlobalLog.LogStatus(obj.RoutedEvent.Name);
                }
            }
            else
            {
                GlobalLog.LogStatus("No events fired as expected");
            }

            return bResult;
        }

        /******************************************************************************
        * Function:          VerifyMouseEvents(string[],string)
        ******************************************************************************/
        /// <summary>
        /// Verify Mouse events fired as expected on FrameworkElement
        /// </summary>
        private bool VerifyMouseEvents(string[] expectedEvents, string mouseAction)
        {
            bool bResult  = true;

            //If element under test is FrameworkContent, verification done later
            if (_isFCETest)
                return bResult;

            GlobalLog.LogStatus("Verify " + mouseAction);

            if ( !_isExpectedAnyEvents )
            {
                return VerifyNoEventsFired();
            }

            int expectedEventsIndex = 0;

            string prevEvent = null;
            string curEvent = null;

            for (int i = 0; i < _eventArgs.Count; i++)
            {

                curEvent = (_eventArgs[i] as RoutedEventArgs).RoutedEvent.Name;

                if (prevEvent != null)
                {
                    if ( curEvent.Equals(prevEvent) && curEvent.Equals("MouseMove") )
                    {
                        //GlobalLog.LogStatus("Current/Previous event: MouseMove - ignore current event for verification");
                        continue;
                    }
                }

                if (expectedEventsIndex >= expectedEvents.Length)
                {
                    if (!curEvent.Equals("MouseMove"))
                    {
                        bResult = false;
                        GlobalLog.LogStatus("There are more mouse events actually fired than expected");
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!(expectedEvents[expectedEventsIndex].Equals(curEvent)))
                {
                    if (curEvent.Equals("MouseMove"))
                    {
                        //GlobalLog.LogStatus("Ignore MouseMove - happens sometimes though not expected");
                        continue;
                    }
                    GlobalLog.LogStatus("Events are not in expected order");
                    bResult = false;
                    break;
                }

                expectedEventsIndex++;

                prevEvent = curEvent;

            }

            if ( (_eventArgs.Count == 0) && _isExpectedAnyEvents )
            {
                GlobalLog.LogStatus("No events fired");
                bResult = false;
            }

            if (!bResult)
            {
                GlobalLog.LogStatus("FAIL: " + mouseAction);

                GlobalLog.LogStatus("Expected order of events");

                foreach (string ev in expectedEvents)
                {
                    GlobalLog.LogStatus(ev);
                }

                GlobalLog.LogStatus("Actual order of events");

                foreach (RoutedEventArgs obj in _eventArgs)
                {
                    GlobalLog.LogStatus(obj.RoutedEvent.Name);
                    //GlobalLog.LogStatus("Source : " + obj.Source.ToString());
                    //GlobalLog.LogStatus("OriginalSource : " + obj.OriginalSource.ToString());
                }

            }
            else
            {
                GlobalLog.LogStatus("PASS: " + mouseAction);
            }

            _eventArgs.Clear();

            return bResult;
        }

        /******************************************************************************
        * Function:          BuildTemplateMouseActionXaml
        ******************************************************************************/
        /// <summary>
        /// Builds a new xaml file for the type to use for loading and verification.        
        /// </summary>
        private void BuildTemplateMouseActionXaml(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            XmlElement containerNode = doc.GetContainerNode(type);

            // Insert element with just initial value for comparision.
            InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", _testElementName, type);
            GlobalLog.LogStatus("elementWithInitialValues generated.");

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), ActionHelper.XamlFileName);
            GlobalLog.LogStatus("Xaml Generated : " + ActionHelper.XamlFileName + ".");
        }

        /******************************************************************************
        * Function:          InsertMainElement
        ******************************************************************************/
        /// <summary>
        /// Create XAML for test element and insert that inside XAML for Container element
        /// </summary>
        internal static void InsertMainElement(XamlTestDocument doc, XmlElement containerSnippet, string rootName, string nodeName, Type type)
        {
            // Get the root node
            XmlElement testRootNode = doc.SelectSingleNode("//*[@Name='" + rootName + "']", ActionHelper.NamespaceManager) as XmlElement;

            //Get the Container node
            XmlElement containerNode = (XmlElement)doc.ImportNode(containerSnippet, true);
            testRootNode.AppendChild(containerNode);
            containerNode = (XmlElement)containerNode.SelectSingleNode("//*[@Name='ContainerElement']", ActionHelper.NamespaceManager);
            containerNode.RemoveAttribute("Name");
            
            //Retrieve XAML for the given type
            XmlElement elementSnippet = ActionHelper.GetElementNode(type, doc);

            //
            if (elementSnippet == null)
            {
                //Get Content for this type
                XmlElement contentSnippet = doc.GetContentNode(type);

                XmlElement element = doc.CreateElement("", type.Name,
                    testRootNode.NamespaceURI);

                element.SetAttribute("Name", nodeName);

                //Add Content
                if (contentSnippet != null)
                    element.InnerXml = contentSnippet.InnerXml;

                containerNode.AppendChild(element);
            }
            else
            {
                //Add XAML for the element to Container node
                containerNode.InnerXml = elementSnippet.InnerXml;
            }
        }

        /******************************************************************************
        * Function:          SetTestElement
        ******************************************************************************/
        /// <summary>
        /// Set the test element such that it will render, so mouse actions
        /// will work. Set the Background for the test element, where applicable
        /// </summary>
        /// <param name="dep"></param>
        private void SetTestElement(DependencyObject dep)
        {
            Button button = new Button();
            button.Content = "Test";
            button.Height = 100.0;
            button.Width = 100.0;

            if (dep is Panel)
            {
                (dep as Panel).Children.Add(button);
                (dep as Panel).Background = Brushes.Blue;
            }

            if (dep is ItemsControl)
            {
                (dep as ItemsControl).Items.Add(button);
            }

            if (dep is Decorator)
            {
                (dep as Decorator).Child = button;
            }
          
            if (dep is ContentControl)
            {
                (dep as ContentControl).Content = "Test Content";
            }

            if (dep is ContentPresenter)
            {
                (dep as ContentPresenter).Content = "Test Content";
            }

            if (dep is Shape)
            {
                (dep as Shape).Height = 100.0;
                (dep as Shape).Width = 20.0;
                (dep as Shape).Fill = Brushes.Red;
            }

            if (dep is Control)
            {
                (dep as Control).Background = Brushes.Green;
            }

            if (dep is TextElement)
            {
                (dep as TextElement).Background = Brushes.Maroon;
            }
        }

        /******************************************************************************
        * Function:          FindElement
        ******************************************************************************/
        /// <summary>
        /// Find the element with given name
        /// 




        private DependencyObject FindElement(DependencyObject depObj, string name)
        {
          DependencyObject result = null;

          if (depObj != null)
          {

            if ((name.Equals(depObj.GetValue(FrameworkElement.NameProperty))) ||
                (name.Equals(depObj.GetValue(FrameworkContentElement.NameProperty))))
            {
              return depObj;
            }

            int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj);

            for (int i = 0; i < count; i++)
            {
              DependencyObject curDepObj = VisualTreeHelper.GetChild(depObj, i);
              if ((result = FindElement(curDepObj, name)) != null)
                return result;
            }
          }

          return null;
        }

        /******************************************************************************
        * Function:          GetElements
        ******************************************************************************/
        /// <summary>
        /// Get all the elements under the given dependencyobject
        /// 






        private System.Collections.ArrayList GetElements(DependencyObject depObj, System.Collections.ArrayList elements)
        {
            if (depObj != null)
            {
                if ((depObj is FrameworkElement) | (depObj is FrameworkContentElement))
                {
                    elements.Add(depObj);
                }

                int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj);

                for (int i = 0; i < count; i++)
                {
                    DependencyObject curDepObj = VisualTreeHelper.GetChild(depObj, i);
                    elements = GetElements(curDepObj, elements);
                }
            }

            return elements;
        }
        #endregion
    }
}
