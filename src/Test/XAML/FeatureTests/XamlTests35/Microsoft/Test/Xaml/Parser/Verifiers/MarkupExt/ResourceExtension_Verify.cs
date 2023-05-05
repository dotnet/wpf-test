// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Test.Serialization.CustomElements;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using System.Windows.Documents;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.MarkupExt
{
    /// <summary/>
    public static class ResourceExtension_Verify
    {
        /// <summary>
        /// ResourceExtensionVerify
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            CustomDockPanel customdockpanel0 = (CustomDockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "CustomDockPanel0");
            customdockpanel0.AllowDrop = true;

            Button button0 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");
            Button button1 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button1");
            Button button2 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button2");
            Button button3 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button3");
            Button button4 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button4");
            Button button5 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button5");
            Button button6 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button6");
            Button button7 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button7");
            Button button8 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button8");

            DockPanel          dockpanel0 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel0");
            ResourceDictionary resources0 = dockpanel0.Resources;
            // change a dynamic reference
            resources0["string1"] = "string1mod";

            TextBlock textblock1 = (TextBlock) LogicalTreeHelper.FindLogicalNode(rootElement, "TextBlock1");
            ListBox   listbox1   = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox1");

            DockPanel          dockpanel1 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel1");
            ResourceDictionary resources1 = dockpanel1.Resources;

            DockPanel          dockpanel2 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel2");
            ResourceDictionary resources2 = dockpanel2.Resources;
            ComboBox           combobox3  = (ComboBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ComboBox3");
            ListBox            listbox3   = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox3");

            DockPanel          dockpanel3 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel3");
            ResourceDictionary resources3 = dockpanel3.Resources;
            Button             button20   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button20");
            Button             button21   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button21");
            Button             button22   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button22");
            Button             button23   = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button23");
            ListBox            listbox13  = (ListBox) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBox13");


            Button    buttonresource12 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "ButtonResource1");
            TextBlock textblock2       = (TextBlock) LogicalTreeHelper.FindLogicalNode(rootElement, "TextBlock2");

            TextBox textbox1 = (TextBox) LogicalTreeHelper.FindLogicalNode(rootElement, "TextBox1");
            TextBox textbox2 = (TextBox) LogicalTreeHelper.FindLogicalNode(rootElement, "TextBox2");


            // checks listboxes with dynamic and static buttons and strings
            ListBoxItem listboxitemimabutton6 = (ListBoxItem) LogicalTreeHelper.FindLogicalNode(rootElement, "ListBoxItem_imabutton6");
            if ((listbox1.Items.Count != 10))
            {
                result = false;
                GlobalLog.LogEvidence("ListBox1.Items.Count != 10");
            }
            if (!(listboxitemimabutton6.Content.GetType().ToString().Equals("System.Windows.Controls.Button")))
            {
                result = false;
                GlobalLog.LogEvidence("(ListBoxItem_imabutton6.Content.GetType().ToString().Equals(System.Windows.Controls.Button))");
            }

            Button listbox1button6 = (Button) listboxitemimabutton6.Content;
            if (!ContentControlHasButton(listbox1.Items[0], "button4"))
            {
                result = false;
                GlobalLog.LogEvidence("!ContentControlHasButton(ListBox1.Items[0], button4)");
            }
            if (!ContentControlHasButton(listbox1.Items[1], "button5"))
            {
                result = false;
                GlobalLog.LogEvidence("!ContentControlHasButton(ListBox1.Items[1], button5)");
            }
            if (!ContentControlHasButton(listbox1.Items[2], "button6"))
            {
                result = false;
                GlobalLog.LogEvidence("!ContentControlHasButton(ListBox1.Items[2], button6)");
            }
            // ensure that dynamic reference changed
            if (!ContentControlHasString(listbox1.Items[3], "string1mod"))
            {
                result = false;
                GlobalLog.LogEvidence("!ContentControlHasString(ListBox1.Items[3], string1mod)");
            }
            if (!ContentControlHasString(listbox1.Items[4], "string2"))
            {
                result = false;
                GlobalLog.LogEvidence("!ContentControlHasString(ListBox1.Items[4], string2");
            }
            if (!ContentControlHasString(listbox1.Items[5], "string3"))
            {
                result = false;
                GlobalLog.LogEvidence("!ContentControlHasString(ListBox1.Items[5], string3");
            }
            if (!ContentControlHasString(listbox1.Items[6], "string4"))
            {
                result = false;
                GlobalLog.LogEvidence("!ContentControlHasString(ListBox1.Items[6], string4");
            }

            if (!(((string) listbox1.Items[7]).Equals(" notintags ")))
            {
                result = false;
                GlobalLog.LogEvidence("!(((string)ListBox1.Items[7]).Equals( notintags ))");
            }

            if (!(((string) listbox1.Items[8]).Equals("string5")))
            {
                result = false;
                GlobalLog.LogEvidence("!(((string)ListBox1.Items[8]).Equals(string5))");
            }
            // ensure static reference didn't change
            if (!(((string) listbox1.Items[9]).Equals("string1")))
            {
                result = false;
                GlobalLog.LogEvidence("!(((string)ListBox1.Items[9]).Equals(string1))");
            }

            // test Textblock with static button and string resources
            // (Textblocks don't accept dynamic resources)
            if (!(listbox1button6.Content.ToString().Equals("button6")))
            {
                result = false;
                GlobalLog.LogEvidence("!(ListBox1Button6.Content.ToString().Equals(button6))");
            }
            if (textblock1.Inlines.Count != 7) // add check for 3 buttons and 2 whitespaces
            {
                result = false;
                GlobalLog.LogEvidence("TextBlock1.Inlines.Count != 7");
            }


            IEnumerator inlineEnum = textblock1.Inlines.GetEnumerator();

            int counter       = 0;
            int buttonCounter = 0;
            while (inlineEnum.MoveNext()) // isn't there a way to index into an inlines collection?
            {
                switch (counter)
                {
                    case 0:
                    case 2:
                    case 4:
                        buttonCounter++;
                        if (!InlineButtonHasString(inlineEnum.Current, "button" + buttonCounter))
                        {
                            result = false;
                            GlobalLog.LogEvidence("!InlineButtonHasString(inlineEnum.Current, button + buttonCounter)");
                        }
                        break;
                    case 1:
                    case 3:
                    case 5:
                        if (!InlineRunHasString(inlineEnum.Current, " "))
                        {
                            result = false;
                            GlobalLog.LogEvidence("!InlineRunHasString(inlineEnum.Current, __)");
                        }
                        break;
                    case 6:
                        if (!InlineRunHasString(inlineEnum.Current, "string1"))
                        {
                            result = false;
                            GlobalLog.LogEvidence("!InlineRunHasString(inlineEnum.Current, string1)");
                        }
                        break;
                    default:
                        GlobalLog.LogEvidence("Too many inlines in ResourceExtension_Verify");
                        result = false;
                        break;
                }
                counter++;
            }


            if (dockpanel1.Children.Count != 3)
            {
                result = false;
                GlobalLog.LogEvidence("DockPanel1.Children.Count != 3");
            }
            if (dockpanel2.Children.Count != 2)
            {
                result = false;
                GlobalLog.LogEvidence("DockPanel2.Children.Count != 2");
            }
            if (dockpanel3.Children.Count != 9)
            {
                result = false;
                GlobalLog.LogEvidence("DockPanel3.Children.Count != 9");
            }


            // checks dynamic and static refs in textboxes
            if (textbox1.Text != "wowww2")
            {
                result = false;
                GlobalLog.LogEvidence("TextBox1.Text != wowww2");
            }
            if (textbox2.Text != "wowww2")
            {
                result = false;
                GlobalLog.LogEvidence("TextBox2.Text != wowww2");
            }


            SolidColorBrush button22Foreground = (SolidColorBrush) button22.GetValue(Control.ForegroundProperty);
            if (!Color.Equals(button22Foreground.Color, Colors.Brown))
            {
                result = false;
                GlobalLog.LogEvidence("!Color.Equals(button22Foreground.Color, Colors.Brown)");
            }

            SolidColorBrush button23Foreground = (SolidColorBrush) button23.GetValue(Control.ForegroundProperty);
            if (!Color.Equals(button23Foreground.Color, Colors.Brown))
            {
                result = false;
                GlobalLog.LogEvidence("!Color.Equals(button23Foreground.Color, Colors.Brown)");
            }

            // check button contents with dynamic and static resources (buttons and strings);
            if (!(button21.Content.GetType().ToString().Equals("System.Windows.Controls.Button")))
            {
                result = false;
                GlobalLog.LogEvidence("!(Button21.Content.GetType().ToString().Equals(System.Windows.Controls.Button))");
            }
            if (!(button22.Content.GetType().ToString().Equals("System.Windows.Controls.Button")))
            {
                result = false;
                GlobalLog.LogEvidence("!(Button22.Content.GetType().ToString().Equals(System.Windows.Controls.Button))");
            }
            if (!(button23.Content.ToString().Equals("wowww1")))
            {
                result = false;
                GlobalLog.LogEvidence("!(Button23.Content.ToString().Equals(wowww1))");
            }
            if (!(button20.Content.ToString().Equals("wowww1")))
            {
                result = false;
                GlobalLog.LogEvidence("!(Button23.Content.ToString().Equals(wowww1))");
            }

            // Verify Button0 and Button1's background property
            SolidColorBrush background = (SolidColorBrush) button0.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, Colors.Green)))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, Colors.Green))");
            }

            background = (SolidColorBrush) button1.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, Colors.Red)))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, Colors.Red))");
            }

            // Verify Button2's content
            String content = (String) button2.GetValue(ContentControl.ContentProperty);
            if ("Hello World" != content)
            {
                result = false;
                GlobalLog.LogEvidence("Hello World != content");
            }


            // Verify Button5 thru 8's background property
            background = (SolidColorBrush) button5.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, Colors.Pink)))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, Colors.Pink))");
            }

            background = (SolidColorBrush) button6.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, Colors.Pink)))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, Colors.Pink))(1)");
            }

            background = (SolidColorBrush) button7.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, Colors.Pink)))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, Colors.Pink))(2)");
            }

            background = (SolidColorBrush) button8.GetValue(Control.BackgroundProperty);
            if (!(Color.Equals(background.Color, Colors.Pink)))
            {
                result = false;
                GlobalLog.LogEvidence("!(Color.Equals(background.Color, Colors.Pink))(3)");
            }
            return result;
        }

        private static bool ContentControlHasButton(object contentControl, string content)
        {
            try
            {
                ContentControl control = (ContentControl) contentControl;
                Button         button  = (Button) control.Content;
                return ContentControlHasString(button, content);
            }
            catch
            {
                return false;
            }
        }

        private static bool ContentControlHasString(object contentControl, string content)
        {
            try
            {
                ContentControl control = (ContentControl) contentControl;
                string         str     = (string) control.Content;
                return content.Equals(str);
            }
            catch
            {
                return false;
            }
        }

        private static bool InlineButtonHasString(object inline, string strContent)
        {
            try
            {
                InlineUIContainer container = (InlineUIContainer) inline;
                Button            child     = (Button) container.Child;
                string            content   = (string) child.Content;
                return strContent.Equals(content);
            }
            catch
            {
                return false;
            }
        }

        private static bool InlineRunHasString(object inline, string strContent)
        {
            try
            {
                Run    run  = (Run) inline;
                string text = run.Text;
                return strContent.Equals(text);
            }
            catch
            {
                return false;
            }
        }
    }
}
