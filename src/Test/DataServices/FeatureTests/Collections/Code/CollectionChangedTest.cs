// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Test different types of CollectionChanged events
    /// </description>
    /// </summary>
    [Test(0, "Collections", "CollectionChangedTest")]
    public class CollectionChangedTest : WindowTest
    {

        private LibraryGenericListMR _library;
        private ListBox _listbox;
        private NotifyCollectionChangedAction _lastAction;

        public CollectionChangedTest()
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(VerifySetup);
            RunSteps += new TestStep(Add);
            RunSteps += new TestStep(VerifyAdd);
            RunSteps += new TestStep(Remove);
            RunSteps += new TestStep(VerifyRemove);
            RunSteps += new TestStep(Move);
            RunSteps += new TestStep(VerifyMove);
            RunSteps += new TestStep(ReBind);
            RunSteps += new TestStep(VerifyRebind);
            RunSteps += new TestStep(Replace);
            RunSteps += new TestStep(VerifyReplace);
            RunSteps += new TestStep(RaiseCollectionChangedDuringGetEnumerator);
        }

        TestResult SetUp()
        {
            _library = new LibraryGenericListMR(10, 0.2);
            _library.CollectionChanged += new NotifyCollectionChangedEventHandler(library_CollectionChanged);
            _listbox = new ListBox();
            _listbox.ItemTemplate = TitleTemplate();
            //listbox.DisplayMemberPath = "Title";

            Page p = new Page();
            StackPanel s = new StackPanel();
            p.Content = s;
            s.Children.Add(_listbox);
            Window.Content = p;

            Binding b = new Binding();
            b.Source = _library;
            _listbox.SetBinding(ListBox.ItemsSourceProperty, b);

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            return TestResult.Pass;
        }

        void library_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _lastAction = e.Action;
            Status(e.Action.ToString());
            Signal(TestResult.Pass);
        }

        TestResult VerifySetup()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { String.Format("Title of book {0}", 0.2),
                                            String.Format("Title of book {0}", 1.2),
                                            String.Format("Title of book {0}", 2.2),
                                            String.Format("Title of book {0}", 3.2),
                                            String.Format("Title of book {0}", 4.2),
                                            String.Format("Title of book {0}", 5.2),
                                            String.Format("Title of book {0}", 6.2),
                                            String.Format("Title of book {0}", 7.2),
                                            String.Format("Title of book {0}", 8.2),
                                            String.Format("Title of book {0}", 9.2)
            };
            return VerifyContent(expected, "Verify Setup");
        }

        TestResult Add()
        {
            _library.Add(new Book(10.3));

            TestResult result = WaitForSignal(10000);
            if (result != TestResult.Pass)
            {
                LogComment("Timeout occured while waiting for collection changed");
                return TestResult.Fail;
            }
            else
            {
                if (_lastAction != NotifyCollectionChangedAction.Add)
                {
                    LogComment("Last Action was expected Add, actual " + _lastAction.ToString());
                    return TestResult.Fail;
                }
                else
                {
                    if (_library.Count != 11)
                    {
                        LogComment("Expected Library to be size 11, actual " + _library.Count.ToString());
                        return TestResult.Fail;
                    }
                    else
                    {
                        LogComment("Add was correct size");
                        return TestResult.Pass;
                    }
                }
            }
        }

        TestResult VerifyAdd()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { String.Format("Title of book {0}", 0.2),
                                            String.Format("Title of book {0}", 1.2),
                                            String.Format("Title of book {0}", 2.2),
                                            String.Format("Title of book {0}", 3.2),
                                            String.Format("Title of book {0}", 4.2),
                                            String.Format("Title of book {0}", 5.2),
                                            String.Format("Title of book {0}", 6.2),
                                            String.Format("Title of book {0}", 7.2),
                                            String.Format("Title of book {0}", 8.2),
                                            String.Format("Title of book {0}", 9.2),
                                            String.Format("Title of book {0}", 10.3)
            };
            return VerifyContent(expected, "Verify Add");
        }

        TestResult Remove()
        {
            _library.RemoveAt(2);

            TestResult result = WaitForSignal(10000);
            if (result != TestResult.Pass)
            {
                LogComment("Timeout occured while waiting for collection changed");
                return TestResult.Fail;
            }
            else
            {
                if (_lastAction != NotifyCollectionChangedAction.Remove)
                {
                    LogComment("Last Action was expected Remove, actual " + _lastAction.ToString());
                    return TestResult.Fail;
                }
                else
                {
                    if (_library.Count != 10)
                    {
                        LogComment("Expected Library to be size 10, actual " + _library.Count.ToString());
                        return TestResult.Fail;
                    }
                    else
                    {
                        LogComment("Remove was correct size");
                        return TestResult.Pass;
                    }
                }
            }
        }

        TestResult VerifyRemove()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { String.Format("Title of book {0}", 0.2),
                                            String.Format("Title of book {0}", 1.2),
                                            String.Format("Title of book {0}", 3.2),
                                            String.Format("Title of book {0}", 4.2),
                                            String.Format("Title of book {0}", 5.2),
                                            String.Format("Title of book {0}", 6.2),
                                            String.Format("Title of book {0}", 7.2),
                                            String.Format("Title of book {0}", 8.2),
                                            String.Format("Title of book {0}", 9.2),
                                            String.Format("Title of book {0}", 10.3)
            };
            return VerifyContent(expected, "Verify Remove");
        }

        TestResult Move()
        {
            _library.Move(2, 4);

            TestResult result = WaitForSignal(10000);
            if (result != TestResult.Pass)
            {
                LogComment("Timeout occured while waiting for collection changed");
                return TestResult.Fail;
            }
            else
            {
                if (_lastAction != NotifyCollectionChangedAction.Move)
                {
                    LogComment("Last Action was expected Move, actual " + _lastAction.ToString());
                    return TestResult.Fail;
                }
                else
                {
                    if (_library.Count != 10)
                    {
                        LogComment("Expected Library to be size 10, actual " + _library.Count.ToString());
                        return TestResult.Fail;
                    }
                    else
                    {
                        LogComment("Move was correct size");
                        return TestResult.Pass;
                    }
                }
            }
        }

        TestResult VerifyMove()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { String.Format("Title of book {0}", 0.2),
                                            String.Format("Title of book {0}", 1.2),
                                            String.Format("Title of book {0}", 4.2),
                                            String.Format("Title of book {0}", 5.2),
                                            String.Format("Title of book {0}", 3.2),
                                            String.Format("Title of book {0}", 6.2),
                                            String.Format("Title of book {0}", 7.2),
                                            String.Format("Title of book {0}", 8.2),
                                            String.Format("Title of book {0}", 9.2),
                                            String.Format("Title of book {0}", 10.3)
            };

            return VerifyContent(expected, "Verify Move");
        }

        TestResult ReBind()
        {
            BindingOperations.ClearAllBindings(_listbox);

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            Binding b = new Binding();
            b.Source = _library;
            _listbox.SetBinding(ListBox.ItemsSourceProperty, b);

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            return TestResult.Pass;
        }

        TestResult VerifyRebind()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { String.Format("Title of book {0}", 0.2),
                                            String.Format("Title of book {0}", 1.2),
                                            String.Format("Title of book {0}", 4.2),
                                            String.Format("Title of book {0}", 5.2),
                                            String.Format("Title of book {0}", 3.2),
                                            String.Format("Title of book {0}", 6.2),
                                            String.Format("Title of book {0}", 7.2),
                                            String.Format("Title of book {0}", 8.2),
                                            String.Format("Title of book {0}", 9.2),
                                            String.Format("Title of book {0}", 10.3) 
            };
            return VerifyContent(expected, "Verify Rebind");
        }

        TestResult Replace()
        {
            Book b = new Book("REPLACE BOOK", "00001", "REPLACEMENT AUTHOR", "REPLACEMENT PUBLISHER", 22.07, Book.BookGenre.Mystery);

            _library[1] = b;

            TestResult result = WaitForSignal(10000);
            if (result != TestResult.Pass)
            {
                LogComment("Timeout occured while waiting for collection changed");
                return TestResult.Fail;
            }
            else
            {
                if (_lastAction != NotifyCollectionChangedAction.Replace)
                {
                    LogComment("Last Action was expected Replace, actual " + _lastAction.ToString());
                    return TestResult.Fail;
                }
                else
                {
                    if (_library.Count != 10)
                    {
                        LogComment("Expected Library to be size 10, actual " + _library.Count.ToString());
                        return TestResult.Fail;
                    }
                    else
                    {
                        LogComment("Move was correct size");
                        return TestResult.Pass;
                    }
                }
            }
        }

        TestResult VerifyReplace()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { String.Format("Title of book {0}", 0.2),
                                            "REPLACE BOOK",
                                            String.Format("Title of book {0}", 4.2),
                                            String.Format("Title of book {0}", 5.2),
                                            String.Format("Title of book {0}", 3.2),
                                            String.Format("Title of book {0}", 6.2),
                                            String.Format("Title of book {0}", 7.2),
                                            String.Format("Title of book {0}", 8.2),
                                            String.Format("Title of book {0}", 9.2),
                                            String.Format("Title of book {0}", 10.3) 
            };
            return VerifyContent(expected, "Verify Replace");
        }

        // 
        TestResult RaiseCollectionChangedDuringGetEnumerator()
        {
            ListBox _listBox1 = new ListBox();
            // This line caused a nullref exception before the fix
            _listBox1.ItemsSource = new EventRaisingEnumerable();

            return TestResult.Pass;
        }

        TestResult VerifyContent(string[] expected, string stepName)
        {
            //WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            FrameworkElement[] titles = Util.FindElements(_listbox, "Title");

            if (expected.Length != titles.Length)
            {
                LogComment("Expected " + expected.Length + " Titles, but actual " + titles.Length);
                return TestResult.Fail;
            }

            TextBlock title;
            bool allCorrect = true;

            for (int i = 0; i < expected.Length; i++)
            {
                title = (TextBlock)titles[i];
                if (title != null)
                {
                    if (title.Text == expected[i])
                    {
                        Status("Correct value for Title" + i);
                    }
                    else
                    {
                        if (allCorrect)
                        {
                            LogComment("Incorrect value found in step " + stepName);
                        }
                        LogComment("Expected '" + expected[i] + "' for Item " + i + " actual '" + title.Text + "' ");
                        allCorrect = false;
                    }
                }
                else
                {
                    if (allCorrect)
                    {
                        LogComment("Incorrect value found in step " + stepName);
                    }
                    LogComment("Title " + i + " could not cast to TextBlock");
                    allCorrect = false;
                }
            }

            if (allCorrect)
            {
                LogComment("All values were correct for " + stepName);
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        private DataTemplate TitleTemplate()
        {
            DataTemplate template = new DataTemplate(typeof(FrameworkElement));
            FrameworkElementFactory sp = new FrameworkElementFactory(typeof(StackPanel));
            sp.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            sp.SetValue(FrameworkElement.HeightProperty, 30.0);

            template.VisualTree = sp;

            FrameworkElementFactory text1 = new FrameworkElementFactory(typeof(TextBlock));
            text1.SetBinding(TextBlock.TextProperty, new Binding("Title"));
            text1.SetValue(FrameworkElement.WidthProperty, 150.0);
            text1.SetValue(FrameworkElement.NameProperty, "Title");
            sp.AppendChild(text1);

            return template;
        }

        // IEnumerable that intentionally raises a CollectionChanged event during GetEnumerator,
        private class EventRaisingEnumerable : IEnumerable<string>, INotifyCollectionChanged
        {
            private List<string> _strings = new List<string>();
            private readonly object _stringsLock = new object();

            public EventRaisingEnumerable()
            {

            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public IEnumerator<string> GetEnumerator()
            {
                lock (_stringsLock)
                {
                    if (_strings.Count == 0)
                    {
                        _strings.Add("String 1");
                        _strings.Add("String 1");
                        _strings.Add("String 1");
                        _strings.Add("String 1");
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    }
                    return _strings.GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                lock (_stringsLock)
                {
                    if (this.CollectionChanged != null)
                    {
                        this.CollectionChanged(this, e);
                    }
                }
            }
        }
    }
}
