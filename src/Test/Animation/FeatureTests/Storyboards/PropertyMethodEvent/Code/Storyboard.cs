// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Animation Storyboard Testing ********************************************
*     Description:
*          Tests Storyboard methods and properties.
*     Pass Conditions:
*          The test case will Pass if Clock properties are correct, when checked (a) directly,
*          and (b) via the CurrentStateInvalidated event handler.
*
*     NOTE:  the FrameworkContentElement animation (FontSize on the Table) works, but does not
*          render.  [This is also true when the animation is applied directly
*          to the Run object.]
*
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll
*     Support Files:
* *******************************************************************************************/
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.PropertyMethodEvent.Storyboards</area>
    /// <priority>0</priority>
    /// <description>
    /// Verify Storyboard properties, methods, and events.
    /// </description>
    /// </summary>
    [Test(0, "Storyboards.PropertyMethodEvent.Storyboards", "StoryboardTest")]

    class StoryboardTest : WindowTest
    {
        #region Test case members
        
        public Canvas                       body;
        public Expander                     expander1;
        public Table                        table1;
        public Button                       button1;
        public FrameworkElementFactory      textbox1;
        public ControlTemplate              template;
        public Storyboard                   storyboard;
        public PropertyPath                 path;
        public DependencyProperty           dp              = null;
        private string                      _parm1           = null;
        private string                      _parm2           = null;
        private string                      _parm3           = null;
        private int                         _REPETITIONS     = 2;
        private double                      _BEGINTIME       = 500d;
        private double                      _DURATION        = 1000d;
        private bool                        _testPassed      = false;

        #endregion

        #region Constructor

        [Variation("FrameworkElement", "None", "None")]
        [Variation("FrameworkElement", "None", "True")]
        [Variation("FrameworkElement", "None", "False")]
        [Variation("FrameworkElement", "SnapshotAndReplace", "None")]
        [Variation("FrameworkElement", "SnapshotAndReplace", "True")]
        [Variation("FrameworkElement", "SnapshotAndReplace", "False")]
        [Variation("FrameworkElement", "Compose", "None")]
        [Variation("FrameworkElement", "Compose", "True")]
        [Variation("FrameworkElement", "Compose", "False")]
        
        [Variation("FrameworkContentElement", "None", "None")]
        [Variation("FrameworkContentElement", "None", "True")]
        [Variation("FrameworkContentElement", "None", "False")]
        [Variation("FrameworkContentElement", "SnapshotAndReplace", "None")]
        [Variation("FrameworkContentElement", "SnapshotAndReplace", "True")]
        [Variation("FrameworkContentElement", "SnapshotAndReplace", "False")]
        [Variation("FrameworkContentElement", "Compose", "None")]
        [Variation("FrameworkContentElement", "Compose", "True")]
        [Variation("FrameworkContentElement", "Compose", "False")]
        
        [Variation("ControlTemplate", "None", "None")]
        [Variation("ControlTemplate", "None", "True")]
        [Variation("ControlTemplate", "None", "False")]
        [Variation("ControlTemplate", "SnapshotAndReplace", "None")]
        [Variation("ControlTemplate", "SnapshotAndReplace", "True")]
        [Variation("ControlTemplate", "SnapshotAndReplace", "False")]
        [Variation("ControlTemplate", "Compose", "None")]
        [Variation("ControlTemplate", "Compose", "True")]
        [Variation("ControlTemplate", "Compose", "False")]

        [Variation("FrameworkElementNoParm", "None", "None")]
        [Variation("FrameworkContentElementNoParm", "None", "None")]


        /******************************************************************************
        * Function:          StoryboardTest Constructor
        ******************************************************************************/
        public StoryboardTest(string testValue1, string testValue2, string testValue3)
        {
            _parm1 = testValue1;
            _parm2 = testValue2;
            _parm3 = testValue3;
            InitializeSteps += new TestStep(CheckInputParameters);
            RunSteps += new TestStep(CreateWindow);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          CheckInputParameters
        ******************************************************************************/
        /// <summary>
        /// CheckInputParameters: checks for a valid input string.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult CheckInputParameters()
        {
            bool        arg1Found   = false;
            bool        arg2Found   = false;
            bool        arg3Found   = false;
            string      errMessage  = "";
            string[]    expList1    = { "FrameworkElementNoParm", "FrameworkContentElementNoParm", "FrameworkElement", "FrameworkContentElement", "ControlTemplate" };
            string[]    expList2    = { "None", "SnapshotAndReplace", "Compose" };
            string[]    expList3    = { "None", "True", "False" };

            arg1Found = AnimationUtilities.CheckInputString(_parm1, expList1, ref errMessage);
            if (errMessage != "")
            {
                GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 1st Parameter: " + errMessage);
            }
            else
            {
                arg2Found = AnimationUtilities.CheckInputString(_parm2, expList2, ref errMessage);
                if (errMessage != "")
                {
                    GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 2nd Parameter: " + errMessage);
                }
                else
                {
                    arg3Found = AnimationUtilities.CheckInputString(_parm3, expList3, ref errMessage);
                    if (errMessage != "")
                    {
                        GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 3rd Parameter: " + errMessage);
                    }
                }
            }

            if (arg1Found && arg2Found && arg3Found)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          CreateWindow
        ******************************************************************************/
        /// <summary>
        /// CreateWindow: creates a new Window and add content to it.
        /// </summary>
        /// <returns></returns>
        TestResult CreateWindow()
        {
            Window.Width                = 500;
            Window.Height               = 500;
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            RadialGradientBrush rgBrush = new RadialGradientBrush();
            rgBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.0));
            rgBrush.GradientStops.Add(new GradientStop(Colors.Navy, 0.5));
            rgBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));
            Window.Background = rgBrush;

            body = new Canvas();

            NameScope.SetNameScope(Window, new NameScope());

            CreateExpander();
            CreateTable();
            CreateTemplate();

            Window.Content = body;
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page has loaded and rendered.  Start the test case here.
        /// </summary>
        /// <returns> </returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("OnContentRendered");
            
            DoubleAnimation animDouble = CreateAnimation(_parm1);
            
            storyboard = CreateStoryboard(_parm1, animDouble);
            
            switch (_parm1)
            {
                case "FrameworkContentElement":
                    AnimateFCE(_parm2, _parm3);
                    break;
                case "FrameworkElement":
                    AnimateFE(_parm2, _parm3);
                    break;
                case "ControlTemplate":
                    AnimateCT(_parm2, _parm3);
                    break;
                case "FrameworkContentElementNoParm":
                case "FrameworkElementNoParm":
                    AnimateNoParm();
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          CreateExpander
        ******************************************************************************/
        /// <summary>
        /// Creates an Expander DO to be used for FrameworkElement testing.
        /// </summary>
        /// <returns> An animation </returns>
        protected void CreateExpander()
        {
            GlobalLog.LogStatus("CreateExpander");

            expander1 = new Expander();
            expander1.Name             = "EXPANDER";
            expander1.Background       = Brushes.DarkSlateBlue;
            expander1.BorderThickness  = new Thickness(2.0);
            expander1.Width            = 150.0;
            expander1.Height           = 150.0;
            expander1.IsExpanded       = true;
            expander1.ExpandDirection  = ExpandDirection.Right;
            Canvas.SetTop(expander1, 10d);
            Canvas.SetLeft(expander1, 10d);

            body.Children.Add(expander1);
            
            Window.RegisterName(expander1.Name, expander1);
        }
        
        /******************************************************************************
        * Function:          CreateTable
        ******************************************************************************/
        /// <summary>
        /// Creates a Table DO to be used for FrameworkContentElement testing.
        /// </summary>
        /// <returns> </returns>
        protected void CreateTable()
        {
            GlobalLog.LogStatus("CreateTable");

            TableRow row;
            TableCell cell1, cell2, cell3;

            Border border = new Border();
            border.Background = Brushes.White;
            border.Width       = 300;
            Canvas.SetTop(border, 300d);
            Canvas.SetLeft(border, 50d);

            FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
            tp.Document = new FlowDocument();
            
            table1 = new Table();
            table1.Name          = "TABLE";
            table1.CellSpacing   = 3;

            CreateColumn(table1, Brushes.Pink);
            CreateColumn(table1, Brushes.LightGreen);
            CreateColumn(table1, Brushes.LightBlue);

            TableRowGroup rowGroup = new TableRowGroup();
            table1.RowGroups.Add(rowGroup);

            row = CreateRow(rowGroup);
            table1.RowGroups.Clear();
            table1.RowGroups.Add(rowGroup);

            cell1 = CreateCell(row, "Body 1");
            cell2 = CreateCell(row, "Body 2");
            cell3 = CreateCell(row, "Body 3");

            table1.RowGroups[0].Rows.Clear();
            rowGroup.Rows.Add(row);

            table1.RowGroups[0].Rows[0].Cells.Clear();
            row.Cells.Add(cell1); 
            row.Cells.Add(cell2); 
            row.Cells.Add(cell3); 

            tp.Document.Blocks.Add(table1);
            border.Child = tp;
            
            body.Children.Add(border);

            Window.RegisterName(table1.Name, table1);
        }

        /******************************************************************************
        * Function:          CreateRow
        ******************************************************************************/
        /// <summary>
        /// Creates a row and adds it to the Table's TableRowGroup.
        /// </summary>
        /// <returns> Returns a TableRow </returns>
        private TableRow CreateRow(TableRowGroup rowGroup)
        {
            TableRow row = new TableRow();
            rowGroup.Rows.Add(row);
            return (row);
        }

        /******************************************************************************
        * Function:          CreateCell
        ******************************************************************************/
        /// <summary>
        /// Creates a cell and adds it to the Table's TableRow.
        /// </summary>
        /// <returns> Returns a TableCell </returns>
        private TableCell CreateCell(TableRow row, string text)
        {
            TableCell cell = new TableCell(new Paragraph(new Run(text)));
            cell.BorderThickness = new Thickness(1);
            cell.Padding = new Thickness(4);
            cell.BorderBrush = Brushes.Black;
            row.Cells.Add(cell);
            return (cell);
        }

        /******************************************************************************
        * Function:          CreateColumn
        ******************************************************************************/
        /// <summary>
        /// Creates a column and adds it to the Table's Columns.
        /// </summary>
        /// <returns> Returns a TableCell </returns>
        private TableColumn CreateColumn(Table table, Brush background)
        {
            TableColumn column = new TableColumn();
            column.Background = background;
            table.Columns.Add(column);
            return (column);
        }

        /******************************************************************************
        * Function:          CreateTemplate
        ******************************************************************************/
        /// <summary>
        /// Creates an Expander DO to be used for FrameworkElement testing.
        /// </summary>
        /// <returns> An animation </returns>
        protected void CreateTemplate()
        {
            GlobalLog.LogStatus("CreateTemplate --- 1");

            button1 = new Button();
            button1.Name = "BUTTON";
            Canvas.SetTop(button1, 200d);
            Canvas.SetLeft(button1, 10d);
            body.Children.Add(button1);
            
            GlobalLog.LogStatus("CreateTemplate --- 2");
            
            textbox1 = new FrameworkElementFactory(typeof(TextBox), "TextBox");
            
            GlobalLog.LogStatus("CreateTemplate --- 3");
            textbox1.SetValue(TextBox.BackgroundProperty, Brushes.Purple);
            textbox1.SetValue(TextBox.HeightProperty, 20d);
            textbox1.SetValue(TextBox.WidthProperty,  20d);
            textbox1.SetValue(TextBox.NameProperty,   "TEXTBOX");
            
            GlobalLog.LogStatus("CreateTemplate --- 4");
            
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX   = 1d;
            scaleTransform.ScaleY   = 1d;
            textbox1.SetValue(TextBox.RenderTransformProperty, scaleTransform);
            
            GlobalLog.LogStatus("CreateTemplate --- 5");
           
            template = new ControlTemplate(typeof(Button));
            
            GlobalLog.LogStatus("CreateTemplate --- 6");

            template.VisualTree = textbox1;
            
            GlobalLog.LogStatus("CreateTemplate --- 7");
            
            button1.Template = template;
            
            GlobalLog.LogStatus("CreateTemplate --- 8");
        }

        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// Creates a DoubleAnimation.
        /// </summary>
        /// <returns> A DoubleAnimation </returns>
        private DoubleAnimation CreateAnimation(string testType)
        {
            GlobalLog.LogStatus("CreateAnimation");

            DoubleAnimation anim = new DoubleAnimation();
            anim.BeginTime          = TimeSpan.FromMilliseconds(_BEGINTIME);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(_DURATION));

            if (testType == "FrameworkElement")
            {
                anim.From               = 0;
                anim.To                 = 1;
            }
            else if (testType == "FrameworkContentElement")
            {
                anim.From               = 1;
                anim.To                 = 32;
            }
            else if (testType == "ControlTemplate")
            {
                anim.By                 = 5;
            }
            
            return anim;
        }

        /******************************************************************************
        * Function:          CreateStoryboard
        ******************************************************************************/
        /// <summary>
        /// Creates Storyboard, ready to be animated.
        /// </summary>
        /// <returns> A Storyboard </returns>
        private Storyboard CreateStoryboard(string animatedObject, DoubleAnimation animDouble)
        {
            GlobalLog.LogStatus("CreateStoryboard for: " + animatedObject);
            
            Storyboard story = new Storyboard();
            story.Name                     = "storyboard";
            story.RepeatBehavior           = new RepeatBehavior(_REPETITIONS);
            story.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
            story.Children.Add(animDouble);

            if (animatedObject == "FrameworkContentElement")
            {
                dp = Table.FontSizeProperty;
                path  = new PropertyPath("(0)", dp);
                Storyboard.SetTargetProperty(story, path);
                Storyboard.SetTargetName(story, table1.Name);
            }
            else if (animatedObject == "FrameworkElement")
            {
                dp = Expander.OpacityProperty;
                path  = new PropertyPath("(0)", dp);
                Storyboard.SetTargetProperty(story, path);
                Storyboard.SetTargetName(story, expander1.Name);
            }
            else if (animatedObject == "ControlTemplate")
            {
                dp = ScaleTransform.ScaleXProperty;
                path  = new PropertyPath("(0).(1)", new DependencyProperty[] { TextBox.RenderTransformProperty, dp });
                Storyboard.SetTargetProperty(story, path);
                Window.RegisterName(textbox1.Name, textbox1);
                Storyboard.SetTargetName(story, textbox1.Name);
            }
            else if (animatedObject == "FrameworkContentElementNoParm")
            {
                dp = Table.FontSizeProperty;
                path  = new PropertyPath("(0)", dp);
                Storyboard.SetTargetProperty(story, path);
                Storyboard.SetTarget(story, table1);    //The 2nd argument is the target object, not its name.
            }
            else if (animatedObject == "FrameworkElementNoParm")
            {
                dp = Expander.OpacityProperty;
                path  = new PropertyPath("(0)", dp);
                Storyboard.SetTargetProperty(story, path);
                Storyboard.SetTarget(story, expander1); //The 2nd argument is the target object, not its name.
            }
            
            return story;
        }
        
        /******************************************************************************
        * Function:          AnimateFCE
        ******************************************************************************/
        /// <summary>
        /// Invoke the appropriate Begin method to start the Animation of a FrameworkContentElement.
        /// </summary>
        /// <returns>  </returns>
        private void AnimateFCE(string handoff, string controllable)
        {
            GlobalLog.LogStatus("AnimateFCE");

            switch (_parm2)
            {
                case "None":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(table1);
                    }
                    else
                    {
                        storyboard.Begin(table1, Convert.ToBoolean(_parm3));
                    }
                    break;
                case "SnapshotAndReplace":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(table1, HandoffBehavior.SnapshotAndReplace);
                    }
                    else
                    {
                        storyboard.Begin(table1, HandoffBehavior.SnapshotAndReplace, Convert.ToBoolean(_parm3));
                    }
                    break;
                case "Compose":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(table1, HandoffBehavior.Compose);
                    }
                    else
                    {
                        storyboard.Begin(table1, HandoffBehavior.Compose, Convert.ToBoolean(_parm3));
                    }
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! AnimateFCE: Argument #1 was not found (1)\n\n");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          AnimateFE
        ******************************************************************************/
        /// <summary>
        /// Invoke the appropriate Begin method to start the Animation of a FrameworkElement.
        /// </summary>
        /// <returns>  </returns>
        private void AnimateFE(string handoff, string controllable)
        {
            GlobalLog.LogStatus("AnimateFE");

            switch (_parm2)
            {
                case "None":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(expander1);
                    }
                    else
                    {
                        storyboard.Begin(expander1, Convert.ToBoolean(_parm3));
                    }
                    break;
                case "SnapshotAndReplace":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(expander1, HandoffBehavior.SnapshotAndReplace);
                    }
                    else
                    {
                        storyboard.Begin(expander1, HandoffBehavior.SnapshotAndReplace, Convert.ToBoolean(_parm3));
                    }
                    break;
                case "Compose":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(expander1, HandoffBehavior.Compose);
                    }
                    else
                    {
                        storyboard.Begin(expander1, HandoffBehavior.Compose, Convert.ToBoolean(_parm3));
                    }
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! AnimateFE: Argument #1 was not found (2)\n\n");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          AnimateCT
        ******************************************************************************/
        /// <summary>
        /// Invoke the appropriate Begin method to start the Animation for a ControlTemplate.
        /// </summary>
        /// <returns>  </returns>
        private void AnimateCT(string handoff, string controllable)
        {
            GlobalLog.LogStatus("AnimateCT");

            switch (_parm2)
            {
                case "None":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(button1, template);
                    }
                    else
                    {
                        storyboard.Begin(button1, template, Convert.ToBoolean(_parm3));
                    }
                    break;
                case "SnapshotAndReplace":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(button1, template, HandoffBehavior.SnapshotAndReplace);
                    }
                    else
                    {
                        storyboard.Begin(button1, template, HandoffBehavior.SnapshotAndReplace, Convert.ToBoolean(_parm3));
                    }
                    break;
                case "Compose":
                    if (_parm3 == "None")
                    {
                        storyboard.Begin(button1, template, HandoffBehavior.Compose);
                    }
                    else
                    {
                        storyboard.Begin(button1, template, HandoffBehavior.Compose, Convert.ToBoolean(_parm3));
                    }
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! AnimateCT: Argument #2 was not found (3)\n\n");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }
        }
        
        /******************************************************************************
        * Function:          AnimateNoParameter
        ******************************************************************************/
        /// <summary>
        /// Invoke the appropriate Begin method to start the Animation, with no parameter specified for Begin.
        /// </summary>
        /// <returns>  </returns>
        private void AnimateNoParm()
        {
            GlobalLog.LogStatus("AnimateNoParm");

            storyboard.Begin();
        }
        
        /******************************************************************************
        * Function:          OnCurrentStateInvalidated
        ******************************************************************************/
        /// <summary>
        /// Invoked when the CurrentStateInvalidated event fires on the Animation.
        /// Verification is carried out here.
        /// </summary>
        /// <returns>  </returns>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---CurrentStateInvalidated fired---" + ((Clock)sender).CurrentState);

            if ( ((Clock)sender).CurrentState == ClockState.Filling)
            {
                _testPassed = true;
                
                //TEST 1: Verify Clock properties via the storyboard GetCurrent methods.
                if (_parm3 == "True")
                {
                    //Verify GetCurrent methods only when the storyboard is controllable.
                    _testPassed = VerifyStoryboardAPIs() && _testPassed;
                }
                
                //TEST 2: Verify Clock properties via the CurrentStateInvalidated sender parameter.
                _testPassed = VerifySender(sender) && _testPassed;
                
                //Call more Storyboard methods for Code Coverage purposes; no verification here.
                storyboard.CurrentStateInvalidated -= new EventHandler(OnCurrentStateInvalidated);
                CoverAdditionalAPIs();
                
                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }

        }

        /******************************************************************************
        * Function:          VerifyStoryboardAPIs
        ******************************************************************************/
        /// <summary>
        /// Verifies Clock properties by checking them via methods on the Storyboard.
        /// </summary>
        /// <returns> Boolean: pass/fail</returns>
        private bool VerifyStoryboardAPIs()
        {
            //Verification of properties of the Storyboard Clock, parent of the Animation.
            bool passed = true;

            switch (_parm1)
            {
                case "FrameworkContentElement":
                    passed = VerifyFCE(table1);
                    break;
                case "FrameworkContentElementNoParm":
                    passed = VerifyFCE(table1);
                    break;
                case "FrameworkElement":
                    passed = VerifyFE(expander1);
                    break;
                case "FrameworkElementNoParm":
                    passed = VerifyFE(expander1);
                    break;
                case "ControlTemplate":
                    passed = VerifyCT(button1);
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! VerifyStoryboardAPIs: Argument #1 was not found (1)\n\n");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }
            return passed;
        }
        
        /******************************************************************************
        * Function:          VerifyFCE
        ******************************************************************************/
        /// <summary>
        /// Verifies Clock properties for a FrameworkContentElement.
        /// </summary>
        /// <returns> Boolean: pass/fail</returns>
        private bool VerifyFCE(Table fce)
        {
            GlobalLog.LogStatus("VerifyFCE");

            bool passedFCE = true;

            if (storyboard.GetCurrentState(fce) != ClockState.Filling)
                passedFCE = false;

            if (storyboard.GetCurrentGlobalSpeed(fce) != 0)
                passedFCE = false;

            if (storyboard.GetCurrentIteration(fce) != 2)
                passedFCE = false;

            if (storyboard.GetCurrentProgress(fce)!= 1)
                passedFCE = false;

            if (storyboard.GetIsPaused(fce)!= false)
                passedFCE = false;

            double expectedTime = _BEGINTIME + _DURATION; 
            if ( ((TimeSpan)storyboard.GetCurrentTime(fce)).TotalMilliseconds != expectedTime)
                passedFCE = false;

            string targetName = Storyboard.GetTargetName(storyboard);
            if ( targetName != fce.Name)
                passedFCE = false;
            
            PropertyPath actualPath = Storyboard.GetTargetProperty(storyboard);
            DependencyProperty actualDP = (DependencyProperty)actualPath.PathParameters[0];
            string actualName = (string)actualDP.Name;
            
            DependencyProperty expectedDP = (DependencyProperty)path.PathParameters[0];
            string expectedName = (string)expectedDP.Name;
            if (actualName != expectedName)
                passedFCE = false;

            GlobalLog.LogEvidence(" CurrentState:   " + storyboard.GetCurrentState(fce)        + "  Expected: Filling");
            GlobalLog.LogEvidence(" GlobalSpeed:    " + storyboard.GetCurrentGlobalSpeed(fce)  + "  Expected: 0");
            GlobalLog.LogEvidence(" Iteration:      " + storyboard.GetCurrentIteration(fce)    + "  Expected: " + _REPETITIONS);
            GlobalLog.LogEvidence(" Progress:       " + storyboard.GetCurrentProgress(fce)     + "  Expected: 1");
            GlobalLog.LogEvidence(" Time:           " + ((TimeSpan)storyboard.GetCurrentTime(fce)).TotalMilliseconds + "  Expected: " + expectedTime);
            GlobalLog.LogEvidence(" TargetName:     " + Storyboard.GetTargetName(storyboard)   + "  Expected: " + fce.Name);
            GlobalLog.LogEvidence(" TargetProperty: " + actualName                             + "  Expected: " + expectedName);

            return passedFCE;
        }

        /******************************************************************************
        * Function:          VerifyFE
        ******************************************************************************/
        /// <summary>
        /// Verifies Clock properties for a FrameworkElement.
        /// </summary>
        /// <returns> Boolean: pass/fail</returns>
        private bool VerifyFE(Expander fe)
        {
            GlobalLog.LogStatus("VerifyFE");

            bool passedFE = true;

            if (storyboard.GetCurrentState(fe) != ClockState.Filling)
                passedFE = false;

            if (storyboard.GetCurrentGlobalSpeed(fe) != 0)
                passedFE = false;

            if (storyboard.GetCurrentIteration(fe) != 2)
                passedFE = false;

            if (storyboard.GetCurrentProgress(fe)!= 1)
                passedFE = false;

            if (storyboard.GetIsPaused(fe)!= false)
                passedFE = false;

            double expectedTime = _BEGINTIME + _DURATION; 
            if ( ((TimeSpan)storyboard.GetCurrentTime(fe)).TotalMilliseconds != expectedTime)
                passedFE = false;

            string targetName = Storyboard.GetTargetName(storyboard);
            if ( targetName != fe.Name)
                passedFE = false;
            
            PropertyPath actualPath = Storyboard.GetTargetProperty(storyboard);
            DependencyProperty actualDP = (DependencyProperty)actualPath.PathParameters[0];
            string actualName = (string)actualDP.Name;
            
            DependencyProperty expectedDP = (DependencyProperty)path.PathParameters[0];
            string expectedName = (string)expectedDP.Name;
            if (actualName != expectedName)
                passedFE = false;

            GlobalLog.LogEvidence(" CurrentState:   " + storyboard.GetCurrentState(fe)        + "  Expected: Filling");
            GlobalLog.LogEvidence(" GlobalSpeed:    " + storyboard.GetCurrentGlobalSpeed(fe)  + "  Expected: 0");
            GlobalLog.LogEvidence(" Iteration:      " + storyboard.GetCurrentIteration(fe)    + "  Expected: " + _REPETITIONS);
            GlobalLog.LogEvidence(" Progress:       " + storyboard.GetCurrentProgress(fe)     + "  Expected: 1");
            GlobalLog.LogEvidence(" Time:           " + ((TimeSpan)storyboard.GetCurrentTime(fe)).TotalMilliseconds + "  Expected: " + expectedTime);
            GlobalLog.LogEvidence(" TargetName:     " + Storyboard.GetTargetName(storyboard)  + "  Expected: " + fe.Name);
            GlobalLog.LogEvidence(" TargetProperty: " + actualName                            + "  Expected: " + expectedName);

            return passedFE;
        }

        /******************************************************************************
        * Function:          VerifyCT
        ******************************************************************************/
        /// <summary>
        /// Verifies Clock properties for an element that's part of a ControlTemplate.
        /// </summary>
        /// <returns> Boolean: pass/fail</returns>
        private bool VerifyCT(Button bt)
        {
            GlobalLog.LogStatus("VerifyCT");

            bool passedCT = true;

            if (storyboard.GetCurrentState(bt) != ClockState.Filling)
                passedCT = false;

            if (storyboard.GetCurrentGlobalSpeed(bt) != 0)
                passedCT = false;

            if (storyboard.GetCurrentIteration(bt) != 2)
                passedCT = false;

            if (storyboard.GetCurrentProgress(bt)!= 1)
                passedCT = false;

            if (storyboard.GetIsPaused(bt)!= false)
                passedCT = false;

            double expectedTime = _BEGINTIME + _DURATION; 
            if ( ((TimeSpan)storyboard.GetCurrentTime(bt)).TotalMilliseconds != expectedTime)
                passedCT = false;

            string targetName = Storyboard.GetTargetName(storyboard);
            if ( targetName != textbox1.Name )
                passedCT = false;
            
            PropertyPath actualPath = Storyboard.GetTargetProperty(storyboard);
            DependencyProperty actualDP = (DependencyProperty)actualPath.PathParameters[0];
            string actualName = (string)actualDP.Name;
            
            DependencyProperty expectedDP = (DependencyProperty)path.PathParameters[0];
            string expectedName = (string)expectedDP.Name;
            if (actualName != expectedName)
                passedCT = false;

            GlobalLog.LogEvidence(" CurrentState:   " + storyboard.GetCurrentState(bt)        + "  Expected: Filling");
            GlobalLog.LogEvidence(" GlobalSpeed:    " + storyboard.GetCurrentGlobalSpeed(bt)  + "  Expected: 0");
            GlobalLog.LogEvidence(" Iteration:      " + storyboard.GetCurrentIteration(bt)    + "  Expected: " + _REPETITIONS);
            GlobalLog.LogEvidence(" Progress:       " + storyboard.GetCurrentProgress(bt)     + "  Expected: 1");
            GlobalLog.LogEvidence(" Time:           " + ((TimeSpan)storyboard.GetCurrentTime(bt)).TotalMilliseconds + "  Expected: " + expectedTime);
            GlobalLog.LogEvidence(" TargetName:     " + Storyboard.GetTargetName(storyboard)  + "  Expected: " + bt.Name);
            GlobalLog.LogEvidence(" TargetProperty: " + actualName                            + "  Expected: " + expectedName);

            return passedCT;
        }

        /******************************************************************************
        * Function:          VerifySender
        ******************************************************************************/
        /// <summary>
        /// Verifies Clock properties by checking them via the handler's sender parameter.
        /// </summary>
        /// <param name="sender"> </param>
        /// <returns> Boolean: pass/fail</returns>
        private bool VerifySender(object sender)
        {
            GlobalLog.LogStatus("VerifySender");

            bool passedSender = true;

            if (((Clock)sender).CurrentGlobalSpeed != 0)
            {
                passedSender = false;
            }

            if (((Clock)sender).CurrentIteration != 2)
            {
                passedSender = false;
            }

            if (((Clock)sender).CurrentProgress != 1)
            {
                passedSender = false;
            }

            if (((Clock)sender).IsPaused != false)
            {
                passedSender = false;
            }

            double expectedTime = _BEGINTIME + _DURATION; 
            if ( ((TimeSpan)((Clock)sender).CurrentTime).TotalMilliseconds != expectedTime)
            {
                passedSender = false;
            }
            
            GlobalLog.LogEvidence(" GlobalSpeed: " + ((Clock)sender).CurrentGlobalSpeed + "  Expected: 0");
            GlobalLog.LogEvidence(" Iteration:   " + ((Clock)sender).CurrentIteration   + "  Expected: " + _REPETITIONS);
            GlobalLog.LogEvidence(" Progress:    " + ((Clock)sender).CurrentProgress    + "  Expected: 1");
            GlobalLog.LogEvidence(" Time:        " + ((TimeSpan)((Clock)sender).CurrentTime).TotalMilliseconds + "  Expected: " + expectedTime);
            GlobalLog.LogEvidence(" IsPaused:    " + ((Clock)sender).IsPaused    + "  Expected: false");

            return passedSender;
        }
        
        /******************************************************************************
        * Function:          CoverAdditionalAPIs
        ******************************************************************************/
        /// <summary>
        /// Call additional methods for API coverage purposes. No verification is carried out
        /// in this test. Some of these methods are covered in lower-priority Modeing tests.
        /// TO-DO: Ensure verification tests exist for all of these scenarios.
        /// </summary>
        /// <returns>  </returns>
        private void CoverAdditionalAPIs()
        {
            GlobalLog.LogStatus("CoverAdditionalAPIs");

            switch (_parm1)
            {
                case "FrameworkContentElement":
                    storyboard.SetSpeedRatio(table1, 2d);
                    storyboard.Pause(table1);
                    storyboard.Resume(table1);
                    storyboard.Seek(table1, TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SeekAlignedToLastTick(table1, TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SkipToFill(table1);
                    storyboard.Stop(table1);
                    storyboard = storyboard.Clone();
                    storyboard.Remove(table1);
                    break;
                case "FrameworkContentElementNoParm":
                    storyboard.SetSpeedRatio(2d);
                    storyboard.Pause();
                    storyboard.Resume();
                    storyboard.Seek(TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SkipToFill();
                    storyboard.Stop();
                    storyboard = storyboard.Clone();
                    storyboard.Remove();
                    break;
                case "FrameworkElement":
                    storyboard.Begin(expander1);
                    storyboard.SetSpeedRatio(expander1, 2d);
                    storyboard.Pause(expander1);
                    storyboard.Resume(expander1);
                    storyboard.Seek(expander1, TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SeekAlignedToLastTick(expander1, TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SkipToFill(expander1);
                    storyboard.Stop(expander1);
                    storyboard = storyboard.Clone();
                    storyboard.Remove(table1);
                    break;
                case "FrameworkElementNoParm":
                    storyboard.SetSpeedRatio(2d);
                    storyboard.Pause();
                    storyboard.Resume();
                    storyboard.Seek(TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SeekAlignedToLastTick(TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SkipToFill();
                    storyboard.Stop();
                    storyboard = storyboard.Clone();
                    storyboard.Remove();
                    break;
                case "ControlTemplate":
                    storyboard.Pause(button1);
                    storyboard.SetSpeedRatio(button1, 2d);
                    storyboard.Pause(button1);
                    storyboard.Resume(button1);
                    storyboard.Seek(button1, TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SeekAlignedToLastTick(button1, TimeSpan.FromMilliseconds(1),TimeSeekOrigin.BeginTime);
                    storyboard.SkipToFill(button1);
                    storyboard.Stop(button1);
                    storyboard = storyboard.Clone();
                    storyboard.Remove(table1);
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! CoverAdditionalAPIs: Argument #1 was not found (2)\n\n");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }
        #endregion
    }
}
