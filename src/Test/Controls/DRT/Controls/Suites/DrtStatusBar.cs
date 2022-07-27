// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DRT
{
    public class DrtStatusBarSuite : DrtTestSuite
    {
        public DrtStatusBarSuite() : base("StatusBar")
        {
            Contact = "Microsoft";
        }

        private StatusBar _statusbar;

        public override DrtTest[] PrepareTests()
        {
            Border b = new Border();
            b.Background = System.Windows.Media.Brushes.White;

            DockPanel panel = new DockPanel();
            panel.Background = System.Windows.Media.Brushes.White;
            panel.LastChildFill = false;

            panel.Width = 600;
            panel.Height = 150;
            b.Child = panel;

            _statusbar = new StatusBar();

            Button btn = new Button();
            _statusbar.Items.Add(btn);

            _statusbar.Items.Add("MSRA-ATC");

            Menu menu = new Menu();
            MenuItem miNew = new MenuItem();
            miNew.Header = "New";
            menu.Items.Add(miNew);
            MenuItem miFile = new MenuItem();
            miFile.Header = "File";
            menu.Items.Add(miFile);
            MenuItem miOpen = new MenuItem();
            miOpen.Header = "Open";
            miFile.Items.Add(miOpen);
            _statusbar.Items.Add(menu);

            _statusbar.Items.Add(9411049);


            ComboBox combobox = new ComboBox();
            combobox.Width = 100;
            for (int i = 0; i < 5; ++i)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i;
                combobox.Items.Add(item);
            }
            _statusbar.Items.Add(combobox);

            TextBlock tb = new TextBlock();
            Hyperlink link = new Hyperlink(new Run("http://www.foo.com"));
            tb.Inlines.Add(link);
            _statusbar.Items.Add(tb);

            DockPanel.SetDock(_statusbar, Dock.Bottom);
            panel.Children.Add(_statusbar);

            DRT.Show(b);

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();

                tests.Add(new DrtTest(Start));
                tests.Add(new DrtTest(BasicTest));
                tests.Add(new DrtTest(Cleanup));

                return tests.ToArray();
            }
            else
            {
                return new DrtTest[]{};
            }
        }

        private void Start()
        {
        }

        private void Cleanup()
        {
        }

        enum BasicTestStep
        {
            Start,

            Test1_ChangeText,
            Test1_Verify,

            End,
        }

        BasicTestStep _basicStep;

        public void BasicTest()
        {
            if (DRT.Verbose) Console.WriteLine("BasicTest: " + _basicStep);

            switch (_basicStep)
            {
                case BasicTestStep.Start:
                    break;

                case BasicTestStep.Test1_ChangeText:
                    DRT.Assert("MSRA-ATC" == _statusbar.Items[1].ToString(), "StatusBar.Items[1] don't equal to 'MSRA-ATC'");
                    _statusbar.Items[1] = "Sparta";
                    break;

                case BasicTestStep.Test1_Verify:
                    DRT.Assert("Sparta" == _statusbar.Items[1].ToString(), "StatusBar changetest error");
                    break;

                case BasicTestStep.End:
                default:
                    break;
            }

            if (_basicStep++ <= BasicTestStep.End)
            {
                DRT.RepeatTest();
            }
        }

    }
}
