// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model DisjointTree.
 *  
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 14 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/PropertyEngine/DisjointTree/DisjointTreeModel.cs $
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.IO;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Markup;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Avalon.Test.CoreUI.PropertyEngine.DisjointTree
{
    /// <summary>
    /// DisjointTreeTreeHelper class
    /// </summary>  
    internal class DisjointTreeTreeHelper
    {
        /// <summary>
        /// Create an DisjointTreeTreeHelper instance.
        /// </summary>
        public DisjointTreeTreeHelper(DisjointTreeModelState state)
        {
            _state = state;
        }

        /// <summary>
        /// Build the window and tree for testing.
        /// </summary>
        /// <returns>test window.</returns>
        public Window BuildTree()
        {
            // Create test window and namescope.
            _win = new Window();
            _win.Title = "DisjointTree model: " + _state.Service + " via " + _state.Link;
            NameScope.SetNameScope(_win, new NameScope());
            
            //
            // Create test tree.
            //

            _testRoot = _CreateTestRoot(_win);

            _firstMentorParent = _CreateMentorParent(_testRoot, _state.FirstParentName);

            // Second mentor parent may be sibling of or nested in forst mentor parent.
            if (_state.ContextOrientation == "Nested")
            {
                _secondMentorParent = _CreateMentorParent(_testRoot, _state.SecondParentName);
            }
            else // _state.ContextOrientation == "Sibling"
            {
                _secondMentorParent = _CreateMentorParent(_firstMentorParent, _state.SecondParentName);
            }

            _firstMentor = _CreateMentor(_firstMentorParent, _state.FirstMentorName);

            _secondMentor = _CreateMentor(_secondMentorParent, _state.SecondMentorName);

            _CreateMentee(_firstMentor, _state.FirstMenteeName, _state.FirstMentorName);
            _CreateMentee(_secondMentor, _state.SecondMenteeName, _state.SecondMentorName);

            _win.Show();

            return _win;
        }

        /// <summary>
        /// Create and add test root to test window. The test root may contain 
        /// resources used by IC mentees but will not be a mentor.
        /// </summary>
        private Panel _CreateTestRoot(Window win)
        {
            StackPanel testRoot = new StackPanel();

            win.Content = testRoot;
            testRoot.RegisterName(_state.TestRootName, testRoot);

            return testRoot;
        }

        /// <summary>
        /// Create a parent for a mentor element. The parent will contain resources
        /// found by the mentee via the IC and mentor.
        /// </summary>        
        /// <remarks>
        /// parentPanel may be another mentor parent (in the case of nested ICs)
        /// or the test root.
        /// </remarks>
        private Panel _CreateMentorParent(Panel parentPanel, string mentorParentName)
        {
            StackPanel mentorParent = new StackPanel();

            //
            // Set properties on mentor parent depending on IC service used by mentee.
            //

            if (_state.Service == "DynamicResource")
            {
                // Add brush resource to parent.
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Lime;
                mentorParent.Resources.Add(_state.DynamicResourceKey, brush);
            }
            
            if (_state.Service == "BindingElementName")
            {
                // Add DataSource and set data context on root to that context.

                // todo: for now I just set the background on the test root and bind to that.
                // need to add a datasource.
                mentorParent.Background = new SolidColorBrush(Colors.Orange);
            }
            
            //
            // Add mentor parent to test root and register name.
            //
            parentPanel.Children.Add(mentorParent);
            mentorParent.RegisterName(mentorParentName, mentorParent);

            return mentorParent;
        }

        /// <summary>
        /// Creates an mentor element that will serve as an IC.
        /// </summary>
        /// <remarks>Mentor element is always a Button.</remarks>
        private FrameworkElement _CreateMentor(Panel mentorParent, string mentorName)
        {
            Button mentorButton = new Button();
            mentorButton.Content = "Mentor Element";
            
            // If testing property inheritance set an inheritable property on the mentor.
            if (_state.Service == "Inheritance")
            {
                mentorButton.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Red));
            }

            // Add mentor to parent and register name.
            mentorParent.Children.Add(mentorButton);
            mentorButton.RegisterName(mentorName, mentorParent);

            return mentorButton;
        }

        /// <summary>
        /// Create and add mentee item to a mentor. The mentee will require some service via IC.
        /// </summary>
        /// <remarks>Mentee is always an FE for now.</remarks>
        private FrameworkElement _CreateMentee(FrameworkElement mentor, string menteeName, string mentorName)
        {
            FrameworkElement mentee = null;

            //
            // Create mentee based on IC service being tested.
            //
            switch (_state.Service)
            {
                case "DynamicResource":
                    Rectangle dr_r = new Rectangle();

                    // Reference DR that will be found through IC.
                    dr_r.Width = 200d;
                    dr_r.Height = 200d;

                    dr_r.SetResourceReference(Rectangle.FillProperty, _state.DynamicResourceKey);

                    mentee = dr_r;
                    break;

                case "BindingElementName":
                    Rectangle db_r = new Rectangle();

                    db_r.Width = 200d; 
                    db_r.Height = 200d;

                    // Create binding to background property of mentor.
                    Binding b = new Binding();
                    b.ElementName = mentorName;
                    b.Path = new PropertyPath(StackPanel.BackgroundProperty);

                    db_r.SetBinding(Rectangle.FillProperty, b);

                    mentee = db_r;
                    break;

                case "Inheritance":
                    // Create element that will use inherited property that must be resolved via IC.
                    TextBlock tb = new TextBlock();
                    tb.Text = "I need an IC to inherit foreground color!";

                    mentee = tb;
                    break;

                case "LoadedEvent":
                    // Create element with Loaded event.
                    Button l_b = new Button();
                    l_b.Content = "I need an IC to start fire Loaded!";
                    l_b.Background = new SolidColorBrush(Colors.Lime);

                    l_b.Loaded += DoChangeBackground;

                    mentee = l_b;
                    break;
                    
                case "InitializedEvent":
                    // Create element with initialized event.
                    Button i_b = new Button();
                    i_b.Content = "I need an IC to start fire Loaded!";
                    i_b.Background = new SolidColorBrush(Colors.Lime);
                    i_b.Initialized += DoChangeBackground;

                    mentee = i_b;
                    break;

                case "NameLookup":
                    // Create element that will be used to lookup a name that must be resolved via IC.                    
                    Button n_b = new Button();
                    n_b.Content = "I need an IC to lookup a name";
                    // n_b.Name = "ButtonInIC";                    

                    mentee = n_b;
                    break;

                //default:
                //    throw new NotSupportedException("Mentee with service: " + _state.Service);
                //    break;
            }            

            //
            // Add the mentee to the mentor.
            //
            
            switch (_state.Link)
            {
                case "VisualBrush":
                    // Create VisualBrush that will contain the IC dependent item.
                    VisualBrush vb = new VisualBrush();
                    vb.Visual = (Visual)mentee;

                    // Assign the VisualBrush to FE that will provide IC.
                    // 
                    ((Button)mentor).Background = vb;

                    mentor.RegisterName(menteeName, mentee);
                    
                    break;

                case "ContextMenu":
                    // Create menuitem that will contain the IC dependent item.
                    MenuItem mi = new MenuItem();
                    mi.Header = mentee;
                    
                    // Add menuitem to context menu.
                    ContextMenu cm = new ContextMenu();
                    cm.Items.Add(mi);
                    
                    // Assign context menu to FE.
                    mentor.ContextMenu = cm;
                    break;

                case "ToolTip":
                    // Create tooltip 
                    ToolTip tt = new ToolTip();
                    tt.Content = mentee;

                    // Assign tooltip to FE.
                    mentor.ToolTip = tt;
                    break;

                //default:
                //    throw new NotSupportedException(_state.Link);
                //    break;
            }
            return mentee;
        }

        private void DoChangeBackground(object s, EventArgs e)
        {
            Button b = s as Button;
            b.Background = new SolidColorBrush(Colors.Crimson);
        }

        private Window _win = null;
        private Panel _testRoot = null;

        private Panel _firstMentorParent;
        private Panel _secondMentorParent;

        private FrameworkElement _firstMentor;
        private FrameworkElement _secondMentor;
        
        //private FrameworkElement _firstMentee;
        //private FrameworkElement _secondMentee;

        private DisjointTreeModelState _state = null;

    }
}

