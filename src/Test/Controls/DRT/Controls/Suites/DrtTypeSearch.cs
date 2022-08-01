// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Automation;
using System.Windows.Threading;
using System.IO;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Markup;
using System.Collections.Generic;

namespace DRT
{
    public class TypeSearchSuite : DrtTestSuite
    {
        public TypeSearchSuite() : base("TypeSearch")
        {
            Contact = "Microsoft";
        }

        ListBox _listBox;
        ComboBox _comboBox;
        ListBox _myDataListBox;
        ListBox _noTextListBox;

        ArrayList _dictionary = new ArrayList();

        ComboBox _sortCB;

        private ColumnDefinition MakeColumn(double width)
        {
            ColumnDefinition cd = new ColumnDefinition();
            cd.MinWidth = width;
            cd.MaxWidth = width;
            return cd;
        }

        private ColumnDefinition MakeColumn(GridLength width)
        {
            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = width;
            return cd;
        }

        private RowDefinition MakeRow(double height)
        {
            RowDefinition cd = new RowDefinition();

            cd.MinHeight = height;
            cd.MaxHeight = height;
            return cd;
        }

        private RowDefinition MakeRow(GridLength height)
        {
            RowDefinition cd = new RowDefinition();

            cd.Height = height;
            return cd;
        }

        public override DrtTest[] PrepareTests()
        {
            Grid g = new Grid();
            g.Width = 680;

            g.ColumnDefinitions.Add(MakeColumn(new GridLength(200)));
            g.ColumnDefinitions.Add(MakeColumn(new GridLength(200)));
            g.ColumnDefinitions.Add(MakeColumn(new GridLength(200)));
            g.ColumnDefinitions.Add(MakeColumn(new GridLength(100, GridUnitType.Star)));

            g.RowDefinitions.Add(MakeRow(new GridLength(100, GridUnitType.Auto)));
            g.RowDefinitions.Add(MakeRow(new GridLength(100, GridUnitType.Auto)));
            g.RowDefinitions.Add(MakeRow(new GridLength(100, GridUnitType.Auto)));
            g.RowDefinitions.Add(MakeRow(new GridLength(400, GridUnitType.Pixel)));

            _listBox = new ListBox();
            _comboBox = new ComboBox();
            _myDataListBox = new ListBox();
            // 

            _noTextListBox = new ListBox();

            Border root = new Border();
            root.Child = g;

            root.Background = SystemColors.WindowBrush;

            // 1st ListBox
            g.Children.Add(_listBox);
            Grid.SetColumn(_listBox, 0);
            Grid.SetRow(_listBox, 3);


            _sortCB = new ComboBox();
            _sortCB.SelectedIndex = 0;
            _sortCB.Items.Add("Name");
            _sortCB.Items.Add("Subject");
            _sortCB.SelectionChanged += new SelectionChangedEventHandler(_sortCB_SelectionChanged);

            g.Children.Add(_sortCB);
            Grid.SetColumn(_sortCB, 1);
            Grid.SetRow(_sortCB, 1);

            _myDataListBox.ItemTemplate = GetMyDataTemplate();
            Binding binding = new Binding("SelectedValue");
            binding.Mode = BindingMode.OneWay;
            binding.Source = _sortCB;
            _myDataListBox.SetBinding(TextSearch.TextPathProperty, binding);

            g.Children.Add(_myDataListBox);
            Grid.SetColumn(_myDataListBox, 1);
            Grid.SetRow(_myDataListBox, 3);

            TextBlock textSelectedIndex = new TextBlock();
            binding = new Binding("SelectedIndex");
            binding.Mode = BindingMode.OneWay;
            binding.Source = _myDataListBox;
            textSelectedIndex.SetBinding(TextBlock.TextProperty, binding);
            g.Children.Add(textSelectedIndex);
            Grid.SetColumn(textSelectedIndex, 1);
            Grid.SetRow(textSelectedIndex, 2);

            g.Children.Add(_comboBox);
            Grid.SetColumn(_comboBox, 2);
            Grid.SetRow(_comboBox, 1);

            TextBlock countLabel = new TextBlock();
            countLabel.Text = "Dictionary Size: ";

            g.Children.Add(countLabel);
            Grid.SetColumn(countLabel, 0);
            Grid.SetRow(countLabel, 0);

            TextBlock count = new TextBlock();

            g.Children.Add(count);
            Grid.SetColumn(count, 0);
            Grid.SetRow(count, 1);


            g.Children.Add(_noTextListBox);
            Grid.SetColumn(_noTextListBox, 2);
            Grid.SetRow(_noTextListBox, 3);

            TextBlock textSelectedIndex2 = new TextBlock();
            binding = new Binding("SelectedIndex");
            binding.Mode = BindingMode.OneWay;
            binding.Source = _noTextListBox;
            textSelectedIndex2.SetBinding(TextBlock.TextProperty, binding);
            g.Children.Add(textSelectedIndex2);
            Grid.SetColumn(textSelectedIndex2, 2);
            Grid.SetRow(textSelectedIndex2, 2);

            /*
            _myDataListBox.SetValue(PrimaryTextLookup.ShowFeedbackProperty, true);
            _listBox.SetValue(PrimaryTextLookup.ShowFeedbackProperty, true);
            _comboBox.SetValue(PrimaryTextLookup.ShowFeedbackProperty, true);
            */
            EnableITSFeedback(_myDataListBox, false);
            EnableITSFeedback(_listBox, true);
            EnableITSFeedback(_comboBox, false);
            EnableITSFeedback(_noTextListBox, true);

            StreamReader reader = new StreamReader(DRT.BaseDirectory + "smallenglish.txt");
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (!_dictionary.Contains(line))
                {
                    _dictionary.Add(line);
                }
            }

            foreach (string word in _dictionary)
            {
                _comboBox.Items.Add(word);
                _listBox.Items.Add(word);
            }

            binding = new Binding("Items.Count");
            binding.Mode = BindingMode.OneWay;
            binding.Source = _comboBox;
            count.SetBinding(TextBlock.TextProperty, binding);

            for (int i = 0; i < 10; i++)
            {
                MyData m = new MyData();

                m.Name = (string)_dictionary[(i * 50) % _dictionary.Count];
                m.Subject = (string)_dictionary[(i * 101) % _dictionary.Count];
                m.Date = DateTime.Now;

                _myDataListBox.Items.Add(m);
            }

            int brushCount = 0;
            foreach (PropertyInfo brushInfo in typeof(Brushes).GetProperties())
            {
                if (brushCount++ > 20) break;
                SolidColorBrush brush = brushInfo.GetValue(null, null) as SolidColorBrush;

                if (brush != null)
                {
                    string colorString = brushInfo.Name;

                    Button b = new Button();
                    b.Background = brush;
                    b.SetValue(TextSearch.TextProperty, colorString);
                    b.ToolTip = colorString;
                    _noTextListBox.Items.Add(b);
                }
            }

            DRT.Show(root);

            _listBoxTests = new TypeSearchTest[]
                        {
                            new TypeSearchTest("b", "b", "babble"),
                            new TypeSearchTest("a", "ba", "babble"),
                            new TypeSearchTest("c", "bac", "baccalaureate"),
                            new TypeSearchTest("c", "bacc", "baccalaureate"),
                            new TypeSearchTest("d", "bacc", "baccalaureate"),
                            new TypeSearchTest(true),
                            new TypeSearchTest("s", "s", "sabbat"),
                            new TypeSearchTest("t", "st", "staple"),
                            new TypeSearchTest("a", "sta", "staple"),
                            new TypeSearchTest("r", "star", "star"),
                            new TypeSearchTest("r", "star", "stars"),
                            new TypeSearchTest("r", "star", "start"),
                            new TypeSearchTest("r", "star", "star"),
                            new TypeSearchTest(true),
                            new TypeSearchTest("w", "w", "wacky"),
                            new TypeSearchTest("w", "w", "wag"),
                            new TypeSearchTest("w", "w", "walnut"),
                            new TypeSearchTest("w", "w", "warehouse"),
                            new TypeSearchTest(true),
                        };

            _myDataListBoxTests = new TypeSearchTest[]
                        {
                            new TypeSearchTest("b", "b", 3 /* baboon */),
                            new TypeSearchTest("a", "ba", 3 /* baboon */),
                            new TypeSearchTest("a", "ba", 8 /* babbler */),
                            new TypeSearchTest(true),
                            new TypeSearchTest("s", "s", 2 /* star */),
                            new TypeSearchTest("s", "s", 7 /* sevenfold */),
                            new TypeSearchTest("s", "s", 2 /* star */),
                            new TypeSearchTest("e", "se", 7 /* sevenfold */),
                            new TypeSearchTest(true),
                        };

            _noTextListBoxTests = new TypeSearchTest[]
                        {
                            new TypeSearchTest("b", "b", 5 /* beige */),
                            new TypeSearchTest("l", "bl", 7 /* black */),
                            new TypeSearchTest("u", "blu", 9 /* blue */),
                            new TypeSearchTest(true),
                            new TypeSearchTest("c", "c", 13 /* cadetblue */),
                            new TypeSearchTest("h", "ch", 14 /* chartreuse */),
                            new TypeSearchTest("a", "cha", 14 /* chartreuse */),
                            new TypeSearchTest(true),
                            new TypeSearchTest("z", "", 14 /* chartreuse -- no change */),
                            new TypeSearchTest(true),
                            new TypeSearchTest("a", "a", 0 /* aliceblue */),
                            new TypeSearchTest("a", "a", 1 /* antiquewhite */),
                            new TypeSearchTest("a", "a", 2 /* aqua */),
                            new TypeSearchTest("a", "a", 3 /* aquamarine */),
                            new TypeSearchTest("a", "a", 4 /* azure */),
                            new TypeSearchTest(true),
                        };

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                    new DrtTest(Start),
                    new DrtTest(StartListBoxTestSearch),
                    new DrtTest(Pause),
                    new DrtTest(StartComboBoxTestSearch),
                    new DrtTest(Pause),
                    new DrtTest(StartMyDataListBoxSearch),
                    new DrtTest(Pause),
                    new DrtTest(StartNoTextListBoxSearch),
                    new DrtTest(Cleanup),
                };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void Start()
        {
        }

        private void Pause()
        {
            //DRT.Pause(2000);
        }

        class TypeSearchTest
        {
            public TypeSearchTest(string c, string expectedPrefix, object expectedCurrent)
            {
                Char = c; ExpectedPrefix = expectedPrefix; ExpectedCurrent = expectedCurrent;
            }

            public TypeSearchTest(string c, string expectedPrefix, int expectedIndex)
            {
                Char = c; ExpectedPrefix = expectedPrefix; ExpectedIndex = expectedIndex;
            }

            public TypeSearchTest(bool isTimeOut)
            {
                IsTimeOut = isTimeOut;
            }

            public bool IsTimeOut;

            public string Char;
            public string ExpectedPrefix;
            public object ExpectedCurrent;
            public int ExpectedIndex;
        }

        TypeSearchTest[] _listBoxTests;
        TypeSearchTest[] _myDataListBoxTests;
        TypeSearchTest[] _noTextListBoxTests;

        class ITSTestInfo
        {
            public TypeSearchTest[] tests;

            public int testIndex = -1;

            public Selector attachedTo;

            public string name;
        }

        private void StartListBoxTestSearch()
        {
            ITSTestInfo info = new ITSTestInfo();
            info.tests = _listBoxTests;
            info.attachedTo = _listBox;
            info.name = "ListBox";

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new DispatcherOperationCallback(ITSTest),
                info);
        }

        private void StartComboBoxTestSearch()
        {
            ITSTestInfo info = new ITSTestInfo();
            info.tests = _listBoxTests;
            info.attachedTo = _comboBox;
            info.name = "ComboBox";

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new DispatcherOperationCallback(ITSTest),
                info);
        }

        private void StartMyDataListBoxSearch()
        {
            ITSTestInfo info = new ITSTestInfo();
            info.tests = _myDataListBoxTests;
            info.attachedTo = _myDataListBox;
            info.name = "MyData ListBox";

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new DispatcherOperationCallback(ITSTest),
                info);
        }

        private void StartNoTextListBoxSearch()
        {
            ITSTestInfo info = new ITSTestInfo();
            info.tests = _noTextListBoxTests;
            info.attachedTo = _noTextListBox;
            info.name = "No text ListBox";

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new DispatcherOperationCallback(ITSTest),
                info);
        }

        private object ITSTest(object arg)
        {
            ITSTestInfo info = (ITSTestInfo)arg;

            if (info.testIndex == -1)
            {
                // First move focus onto the element, then start the tests.
                info.attachedTo.Focus();

                Console.WriteLine("Begin " + info.name);
            }
            else if (info.testIndex < info.tests.Length)
            {
                object instance = CallInternal("GetInstance", null, info.attachedTo);

                DRT.Assert(instance != null, "No TextSearch instance was attached to the element (" + info.attachedTo + ")");

                TypeSearchTest currTest = info.tests[info.testIndex];
                Selector selector = info.attachedTo;

                Console.WriteLine("  Type '{0}', expected: current = {1}, index = {2}, prefix = {3}", currTest.Char, currTest.ExpectedCurrent, currTest.ExpectedIndex, currTest.ExpectedPrefix);

                if (!currTest.IsTimeOut)
                {
                    CallInternal("TypeAKey", instance, currTest.Char);
                    if (currTest.ExpectedCurrent != null)
                    {
                        DRT.Assert(String.Equals((string)selector.SelectedItem, (string)currTest.ExpectedCurrent, StringComparison.InvariantCulture),
                            "Typed key: " + currTest.Char + ", should have selected " + (string)currTest.ExpectedCurrent + ", but was " + (string)selector.SelectedItem);
                    }
                    else
                    {
                        DRT.Assert(selector.SelectedIndex == currTest.ExpectedIndex,
                            "Typed key: " + currTest.Char + ", should have selected '" + currTest.ExpectedIndex + "', but was '" + selector.SelectedIndex + "'");
                    }

                    string prefix = (string)CallInternal("GetCurrentPrefix", instance, null);

                    DRT.Assert(String.Equals(prefix, currTest.ExpectedPrefix, StringComparison.InvariantCulture), "Expected prefix was '" + currTest.ExpectedPrefix + "', is: '" + prefix + "'");
                    if (selector is ListBox)
                    {
                        Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Input,
                            (DispatcherOperationCallback)delegate(object unused)
                            {
                                return null;
                            },
                            null);
                            DRT.Assert(((UIElement)selector.ItemContainerGenerator.ContainerFromItem(selector.SelectedItem)).IsKeyboardFocused, "Should have moved focus to " + selector.SelectedItem + ", focus is on " + Keyboard.FocusedElement);
                            DRT.Assert(Selector.GetIsSelectionActive(((UIElement)selector.ItemContainerGenerator.ContainerFromItem(selector.SelectedItem))), "Selector.IsSelectionActive should be true for " + selector.SelectedItem);
                            DRT.Assert(Selector.GetIsSelectionActive(selector), "Selector.IsSelectionActive should be true for " + selector);
                        }
                    }
                else
                {
                    CallInternal("CauseTimeOut", instance, null);
                }
            }
            else
            {
                // Reset the timeout for this items control.
                object instance = CallInternal("GetInstance", null, info.attachedTo);
                CallInternal("CauseTimeOut", instance, null);

                // Don't queue a callback if we're done with this set of tests
                return null;
            }

            info.testIndex++;
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new DispatcherOperationCallback(ITSTest),
                info);

            return null;
        }

        private object CallInternal(string name, object instance, object arg)
        {
            MethodInfo info = typeof(TextSearch).GetMethod(name,
                       BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            if (info == null)
            {
                info = typeof(TextSearch).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            }

            DRT.Assert(info != null, "Couldn't find internal method TextSearch." + name);
            if (arg != null)
            {
                return info.Invoke(instance, new object[] { arg });
            }
            else
            {
                return info.Invoke(instance, new object[] {});
            }
        }

        private object GetInternalDP(string name)
        {
            FieldInfo info = typeof(TextSearch).GetField(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField);

            DRT.Assert(info != null, "Couldn't find internal field TextSearch." + name);

            return info.GetValue(null);
        }

        private void Cleanup()
        {
        }

        private DataTemplate GetMyDataTemplate()
        {
            DataTemplate t = new DataTemplate();

            /* Setters are not supported in DataTemplate.  Review with NamitaG.
            s.Setters.Add (new Setter(TextBlock.FontSizeProperty, 10.0));
            */

            FrameworkElementFactory gridPanel = new FrameworkElementFactory(typeof(StackPanel));
            FrameworkElementFactory dockPanel1 = new FrameworkElementFactory(typeof(DockPanel));
            FrameworkElementFactory flowPanel1 = new FrameworkElementFactory(typeof(StackPanel));
            FrameworkElementFactory textSubject = new FrameworkElementFactory(typeof(TextBlock));
            FrameworkElementFactory textDate = new FrameworkElementFactory(typeof(TextBlock));
            FrameworkElementFactory textName = new FrameworkElementFactory(typeof(TextBlock));

            gridPanel.AppendChild(dockPanel1);
            gridPanel.AppendChild(flowPanel1);
            dockPanel1.AppendChild(textDate);
            dockPanel1.AppendChild(textSubject);
            textDate.SetValue(DockPanel.DockProperty, Dock.Left);
            textSubject.SetValue(DockPanel.DockProperty, Dock.Left);
            textSubject.SetBinding(TextBlock.TextProperty, new Binding("Subject"));
            textDate.SetValue(TextBlock.TextProperty, "Subject: ");
            textDate.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);

            flowPanel1.AppendChild(textName);
            textName.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            textName.SetValue(TextBlock.ForegroundProperty, Brushes.Gray);

            t.VisualTree = gridPanel;
            return t;
            /*
            string style =
            "<Style>" +
            "   <GridPanel>" +
            "      <DockPanel>" +
            "          <TextBlock Text=\"*Bind(Path=Subject)\" DockPanel.Dock=\"Left\"/>" +
            "          <TextBlock Text=\"*Bind(Path=Date)\" DockPanel.Dock=\"Right\"/>" +
            "      </DockPanel>" +
            "      <StackPanel>" +
            "          <TextBlock Text=\"*Bind(Path=Name)\" />" +
            "      </StackPanel>" +
            "   </GridPanel>" +
            "</Style>";

            StringReader sr = new StringReader(style);
            return (Style)XamlReader.Load();
            */
        }

        private void _sortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            string sortProp = (string)e.SelectedItems[0];
            _myDataListBox.Items.Sort.Add(new SortDescription(sortProp, ListSortDirection.Ascending));
            */
        }

        private void EnableITSFeedback(Selector attachedTo, bool followItem)
        {
            Popup _feedbackPopup = new Popup();
            _feedbackPopup.DataContext = attachedTo;

            ContentControl dropShadow = new ContentControl();

            /*Style dropShadowStyle = (Style)dropShadow.FindResource("DropShadowEffect");
            //dropshadow.SetResourceReference(ContentControl.StyleProperty, "DropShadowEffect");
            if (dropShadowStyle != null)
            {
                dropShadow.Style = dropShadowStyle;
            }*/

            Border b = new Border();

            b.BorderBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));
            b.BorderThickness = new Thickness(4);
            b.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

            TextBlock t = new TextBlock();

            //t.SetBinding(TextBlock.TextProperty, new Binding("(TextSearch.CurrentPrefix)"));
            DependencyProperty CurrentPrefixProperty = (DependencyProperty)GetInternalDP("CurrentPrefixProperty");
            Binding binding = new Binding("Text");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = t;
            attachedTo.SetBinding(CurrentPrefixProperty, binding);

            b.Child = t;
            t.Margin = new Thickness(2);

            dropShadow.Content = b;
            _feedbackPopup.Child = dropShadow;
            if (followItem)
            {
                _feedbackPopup.Placement = PlacementMode.Right;
                _feedbackPopup.SetBinding(Popup.PlacementTargetProperty, new Binding("(ToolTip.PlacementTarget)"));
                // Dynamically changing the placement target doesn't work, so need to tweak the HorizontalOffset property to cause it to be repositioned
                _feedbackPopup.SetBinding(Popup.HorizontalOffsetProperty, new Binding("(ToolTip.HorizontalOffset)"));
            }
            else
            {
                // 
                _feedbackPopup.Placement = PlacementMode.Relative;
                _feedbackPopup.SetBinding(Popup.PlacementTargetProperty, new Binding());
                _feedbackPopup.VerticalOffset = -30;
            }

            //_feedbackPopup.SetBinding(Popup.IsOpenProperty, new Binding("(TextSearch.IsActive)"));
            DependencyProperty IsActiveProperty = (DependencyProperty)GetInternalDP("IsActiveProperty");
            binding = new Binding("IsOpen");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = _feedbackPopup;
            attachedTo.SetBinding(IsActiveProperty, binding);

            attachedTo.SelectionChanged += new SelectionChangedEventHandler(OnITSSelectionChanged);
        }

        private static void OnITSSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector s = sender as Selector;

            if (s.SelectedIndex != -1)
            {
                s.SetValue(Popup.PlacementTargetProperty, s.ItemContainerGenerator.ContainerFromIndex(s.SelectedIndex));
                // Need to randomly change HorizontalOffset so that the popup gets repositioned -- changes to PlacementTarget don't cause a reposition.
                //s.SetValue(Popup.HorizontalOffsetProperty, new Length((new Random()).NextDouble()));
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(AfterLayout),
                    s);
            }
            else
            {
                s.ClearValue(Label.TargetProperty);
            }
        }

        private static object AfterLayout(object arg)
        {
            Selector s = arg as Selector;

            s.SetValue(Popup.HorizontalOffsetProperty, (new Random()).NextDouble());
            return null;
        }

        public static readonly DependencyProperty SelectedItemUIProperty = DependencyProperty.RegisterAttached("SelectedItemUI", typeof(UIElement), typeof(TypeSearchSuite));
    }

    public class MyData
    {
        public string Name { get { return _name; } set { _name = value; } }

        public string Subject { get { return _subject; } set { _subject = value; }  }

        private string _name, _subject;

        public DateTime Date { get { return _date; } set { _date = value; } }

        private DateTime _date;
    }

}
