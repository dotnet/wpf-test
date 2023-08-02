// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Windows;
using Microsoft.Test.UIAutomaion;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing ControlType for Controls Below
    /// Button
    /// CheckBox
    /// ComboBox
    /// DocumentViewer    
    /// Expander
    /// FixedPage
    /// FlowDocument 
    /// FlowDocumentPageViewer
    /// FlowDocumentReader
    /// Frame    
    /// GridView
    /// GridViewColumnHeader
    /// GridViewHeaderRowPresenter
    /// GroupBox
    /// GroupItem
    /// Hyperlink
    /// Image
    /// InkCanvas
    /// Label
    /// Listbox
    /// ListboxItem
    /// ListView
    /// MediaElement
    /// Menu
    /// MenuItem
    /// PasswordBox
    /// PopupRoot       
    /// ProgressBar
    /// RadioButton
    /// RepeatButton
    /// RichTextBox
    /// ScrollBar
    /// ScrollViewer    -- bug
    /// Selector        
    /// Separator
    /// Slider
    /// StatusBar
    /// StatusBarItem   -- bug 
    /// TabControl
    /// TabItem
    /// Table 
    /// TableCell 
    /// TextBlock
    /// TextBox
    /// Thumb
    /// ToggleButton
    /// ToolBar
    /// ToolTip
    /// TreeView
    /// TreeViewItem
    /// UserControl
    /// Viewport3D
    /// Window    
    /// </summary>
    [Serializable]
    public class UiaControlTypeTest : UiaSimpleTestcase
    {
        public override void Init(object target)
        {
        }

        public override void DoTest(AutomationElement target)
        {
            bool controlTypeFlag = true;

            if (string.Compare(target.Current.AutomationId, "button", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Button)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "checkbox", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.CheckBox)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "combobox", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                TestLog.Current.LogEvidence("ComboBox Automation");
                if (target.Current.ControlType != ControlType.ComboBox)
                {
                    TestLog.Current.LogEvidence("ComboBox Automation ControlType is not ControlType.ComboBox");
                    TestLog.Current.LogEvidence("ComboBox Automation ControlType is " + target.Current.ControlType.LocalizedControlType.ToString());
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "expander", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                TestLog.Current.LogEvidence("Expander Automation");
                if (target.Current.ControlType != ControlType.Group)
                {
                    TestLog.Current.LogEvidence("Expander Automation ControlType is not ControlType.Group");
                    TestLog.Current.LogEvidence("Expander Automation ControlType is " + target.Current.ControlType.LocalizedControlType.ToString());
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "label", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Text)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "listbox", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.List)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "listboxitem", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.ListItem)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "listview", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                TestLog.Current.LogEvidence("ListView Automation");
                if (target.Current.ControlType != ControlType.List)
                {
                    TestLog.Current.LogEvidence("ListView Automation ControlType is not ControlType.List");
                    TestLog.Current.LogEvidence("ListView Automation ControlType is " + target.Current.ControlType.LocalizedControlType.ToString());
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "menu", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Menu)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "menuitem", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.MenuItem)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "popuproot", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Window)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "progressbar", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.ProgressBar)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "radiobutton", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.RadioButton)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "repeatbutton", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Button)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "scrollbar", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.ScrollBar)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "scrollviewer", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                TestLog.Current.LogEvidence("ScrollViewer Automation");
                if (target.Current.ControlType != ControlType.Pane)
                {
                    TestLog.Current.LogEvidence("ScrollViewer Automation ControlType is not ControlType.Pane");
                    TestLog.Current.LogEvidence("ScrollViewer Automation ControlType is " + target.Current.ControlType.LocalizedControlType.ToString());
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "selector", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.List)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "separator", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Separator)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "slider", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Slider)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "statusbar", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.StatusBar)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "statusbaritem", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Text)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "tabcontrol", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Tab)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "tabitem", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.TabItem)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "thumb", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Thumb)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "togglebutton", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Button)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "toolbar", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.ToolBar)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "treeview", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Tree)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "treeviewitem", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.TreeItem)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "window", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Window)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "image", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Image)
                {
                    controlTypeFlag = false;
                }
            }


            // Group 
            else if (string.Compare(target.Current.AutomationId, "groupbox", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Group)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "groupitem", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Group)
                {
                    controlTypeFlag = false;
                }
            }

            // Document 
            else if (string.Compare(target.Current.AutomationId, "documentviewer", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Document)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "flowdocumentpageviewer", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Document)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "richtextbox", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Document)
                {
                    controlTypeFlag = false;
                }
            }

            // Pane
            else if (string.Compare(target.Current.AutomationId, "fixedpage", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Pane)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "frame", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Pane)
                {
                    controlTypeFlag = false;
                }
            }

            // GridView        
            else if (string.Compare(target.Current.AutomationId, "gridview", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.DataGrid)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "gridviewcolumnheader", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.HeaderItem)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "gridviewheaderrowpresenter", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Header)
                {
                    controlTypeFlag = false;
                }
            }

            else if (string.Compare(target.Current.AutomationId, "hyperlink", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Hyperlink)
                {
                    controlTypeFlag = false;
                }
            }

            // Custom
            else if (string.Compare(target.Current.AutomationId, "flowdocumentreader", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Custom)  // 
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "inkcanvas", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Custom)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "mediaelement", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Custom)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "usercontrol", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Custom)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "viewport3d", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Custom)
                {
                    controlTypeFlag = false;
                }
            }

            // Edit
            else if (string.Compare(target.Current.AutomationId, "passwordbox", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Edit)
                {
                    controlTypeFlag = false;
                }
            }
            else if (string.Compare(target.Current.AutomationId, "textbox", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Edit)
                {
                    controlTypeFlag = false;
                }
            }

            else if (string.Compare(target.Current.AutomationId, "textblock", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.Text)
                {
                    controlTypeFlag = false;
                }
            }

            else if (string.Compare(target.Current.AutomationId, "tooltip", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                if (target.Current.ControlType != ControlType.ToolTip)
                {
                    controlTypeFlag = false;
                }
            }

            SharedState["controlTypeFlag"] = controlTypeFlag;
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;

            if (!(bool)SharedState["controlTypeFlag"])
            {
                TestLog.Current.LogEvidence(((FrameworkElement)target).Name + " Automation ControlType is not " + ((FrameworkElement)target).Name);
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }
}
