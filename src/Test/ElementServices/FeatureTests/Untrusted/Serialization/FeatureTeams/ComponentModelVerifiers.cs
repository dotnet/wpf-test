// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections;
using Avalon.Test.CoreUI.Parser;
using System.Windows.Shapes;
using System.Windows.Media.Animation;


namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Verify xaml files for Animation
    /// Verification method for xaml files from ComponentModel team
    
    /// </summary>
    public class ComponentModelVerifiers
    {
        /// <summary>
        /// 
        /// </summary>
        public static void ButtonComponentModelVerify(UIElement uie)
        {
            StackPanel myPanel = uie as StackPanel;
            VerifyElement.VerifyBool(null == myPanel, false);
            
            CoreLogger.LogStatus("Verifying Button ...");

            Button button = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button");

            VerifyElement.VerifyBool(null == button, false);

            VerifyElement.VerifyString((string)button.Content, "Button");
        }
        /// <summary>
        /// 
        /// </summary>
        public static void CheckBoxComponentModelVerify(UIElement uie)
        {
            StackPanel myPanel = uie as StackPanel;
            VerifyElement.VerifyBool(null == myPanel, false);
            
            CoreLogger.LogStatus("Verifying CheckBox ...");

            CheckBox checkBox = (CheckBox)LogicalTreeHelper.FindLogicalNode(uie, "CheckBox");

            VerifyElement.VerifyBool(null == checkBox, false);
            VerifyElement.VerifyString((string)checkBox.Content, "CheckBox");

        }
        /// <summary>
        /// 
        /// </summary>
        public static void PopupComponentModelVerify(UIElement uie)
        {
            Canvas myPanel = uie as Canvas;
            VerifyElement.VerifyBool(null == myPanel, false);

            CoreLogger.LogStatus("Verifying Popup ...");

            Popup popup = (Popup)LogicalTreeHelper.FindLogicalNode(uie, "Popup");

            VerifyElement.VerifyBool(null == popup, false);
            VerifyElement.VerifyBool(popup.StaysOpen, false);
            VerifyElement.VerifyDouble(popup.HorizontalOffset, 10);
            VerifyElement.VerifyDouble(popup.VerticalOffset, 10);
            VerifyElement.VerifyBool(popup.Placement == PlacementMode.Center, true);
            VerifyElement.VerifyBool(popup.Child is TextBlock, true);
            VerifyElement.VerifyString(((TextBlock)popup.Child).Text, "Hello");

        }
        /// <summary>
        /// 
        /// </summary>
        public static void ComboBoxComponentModelVerify(UIElement uie)
        {
            StackPanel myPanel = uie as StackPanel;
            VerifyElement.VerifyBool(null == myPanel, false);
            
            CoreLogger.LogStatus("Verifying Combobox ...");

            ComboBox myComboBox = (ComboBox)LogicalTreeHelper.FindLogicalNode(uie, "CostCenterCombo");

            VerifyElement.VerifyBool(null == myComboBox, false);
            VerifyElement.VerifyDouble(myComboBox.Width, 200);
            ItemCollection myItems = myComboBox.Items as ItemCollection;
            VerifyElement.VerifyBool(null == myItems, false);
            VerifyElement.VerifyInt(myComboBox.Items.Count, 3);

            ComboBoxItem item = myComboBox.Items[1] as ComboBoxItem;

            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "Item2");

            item = myItems[0] as ComboBoxItem;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "Item1");

            item = myItems[2] as ComboBoxItem;

            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "Item3");

        }
        /// <summary>
        /// 
        /// </summary>
        public static void HyperlinkComponentModelVerify(UIElement uie)
        {
            StackPanel myPanel = uie as StackPanel;
            VerifyElement.VerifyBool(null == myPanel, false);
            
            CoreLogger.LogStatus("Verifying Hyperlink ...");

            Hyperlink myHyperlink = (Hyperlink)LogicalTreeHelper.FindLogicalNode(uie, "Hyperlink");

            VerifyElement.VerifyBool(null == myHyperlink, false);
            VerifyElement.VerifyString((string)((Run)(myHyperlink.Inlines.FirstInline)).Text, "Hyperlink");

        }
        /// <summary>
        /// 
        /// </summary>
        public static void ListBoxComponentModelVerify(UIElement uie)
        {
            StackPanel myPanel = uie as StackPanel;

            VerifyElement.VerifyBool(null == myPanel, false);
            
            CoreLogger.LogStatus("Verifying Button ...");

            ListBox myListBox = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox");

            VerifyElement.VerifyBool(null == myListBox, false);

            ItemCollection myItems = myListBox.Items as ItemCollection;

            VerifyElement.VerifyBool(null == myItems, false);
            VerifyElement.VerifyInt(myItems.Count, 6);

            ListBoxItem item = myItems[0] as ListBoxItem;

            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "ListItem0");

            item = myItems[1] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "ListItem1");

            item = myItems[2] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "ListItem2");

            item = myItems[3] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "ListItem3");

            item = myItems[4] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "ListItem4");

            item = myItems[5] as ListBoxItem;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "ListItem5");
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static void RadioButtonListComponentModelVerify(UIElement uie)
        {
            FrameworkElement fe = uie as FrameworkElement;
            RadioButton item = fe.FindName("RADIOBUTTON_1") as RadioButton;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "RadioButton1");


            item = fe.FindName("RADIOBUTTON_2") as RadioButton;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "RadioButton2");

            item = fe.FindName("RADIOBUTTON_3") as RadioButton;
            VerifyElement.VerifyBool(null == item, false);
            VerifyElement.VerifyString((string)item.Content, "RadioButton3");
        }
    }
}
