// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;
using System;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests BindingOperations.GetDefaultView(...), BindingOperations.GetNamedView(...) and BindingOperations.GetCustomView(...).
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Views", "GetViews")]
    public class GetViews : WindowTest
    {
        private ObservableCollection<BookSource> _bookList;
        private ListBox _lb;
        private MyListSource _mls;
        private IEnumerable _ie;

        public GetViews()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestDefaultView);
            RunSteps += new TestStep(TestNamedView);
            RunSteps += new TestStep(TestCustomView);
            RunSteps += new TestStep(MakeNamedViewDefault);
            RunSteps += new TestStep(MakeCustomViewDefault);
        }

        #region Setup
        private TestResult Setup()
        {
            Status("Setup");
            _bookList = new ObservableCollection<BookSource>();
            _bookList.Add(new BookSource("0735616485", "Inside C#"));
            _bookList.Add(new BookSource("0321154916", "The C# Programming Language"));
            _bookList.Add(new BookSource("0764543989", "Professional C#"));
            _bookList.Add(new BookSource("0321126971", "Design Patterns in C#"));
            _bookList.Add(new BookSource("0735615543", "Microsoft Visual C# .NET Language Reference"));

            _lb = new ListBox();
            _lb.Width = 300;
            // this provides coverage for the getter and setter of ItemsSource (we always bind to it)
            _lb.ItemsSource = _bookList;
            if (_lb.ItemsSource != _bookList)
            {
                LogComment("Fail - Data source not properly set to ItemsSource");
                return TestResult.Fail;
            }

            DataTemplate myItemTemplate = new DataTemplate();
            FrameworkElementFactory simpletext = new FrameworkElementFactory(typeof(TextBlock));
            simpletext.SetBinding(TextBlock.TextProperty, new Binding("Title"));
            simpletext.SetValue(TextBlock.NameProperty, "lb1");
            myItemTemplate.VisualTree = simpletext;

            _lb.ItemTemplate = myItemTemplate;

            DockPanel dockPanel = new DockPanel();
            dockPanel.Background = Brushes.AliceBlue;
            dockPanel.Children.Add(_lb);

            Window.Content = dockPanel;

            _ie = _lb.ItemsSource as IEnumerable;
            _mls = new MyListSource();

            LogComment("Setup was successful");
            return TestResult.Pass;
        }
        #endregion

        #region DefaultView
        private TestResult TestDefaultView()
        {
            Status("TestDefaultView");
            if (!TestDefaultViewHelper(_ie)) { return TestResult.Fail; }
            if (!TestDefaultViewHelper(_mls)) { return TestResult.Fail; }

            ICollectionView cv1 = CollectionViewSource.GetDefaultView((IEnumerable)null);
            if (cv1 != null)
            {
                LogComment("Fail - CollectionView should be null (cv1)");
                return TestResult.Fail;
            }

            ICollectionView cv2 = CollectionViewSource.GetDefaultView((IListSource)null);
            if (cv2 != null)
            {
                LogComment("Fail - CollectionView should be null (cv2)");
                return TestResult.Fail;
            }

            LogComment("TestDefaultView was successful");
            return TestResult.Pass;
        }

        private bool TestDefaultViewHelper(object obj)
        {
            ICollectionView cv1;
            ICollectionView cv2;
            if (obj is IEnumerable)
            {
                cv1 = CollectionViewSource.GetDefaultView((IEnumerable)obj);
                cv2 = CollectionViewSource.GetDefaultView((IEnumerable)obj);
            }
            else if (obj is IListSource)
            {
                cv1 = CollectionViewSource.GetDefaultView((IListSource)obj);
                cv2 = CollectionViewSource.GetDefaultView((IListSource)obj);
            }
            else
            {
                LogComment("Fail - Object was of type " + obj.GetType().ToString() + ". Expected types are IEnumerable and IListSource.");
                return false;
            }
            cv1.MoveCurrentToLast();

            if (cv1 != cv2)
            {
                LogComment("Fail - GetDefaultView should always return the same view");
                return false;
            }

            if (cv1.CurrentItem != cv2.CurrentItem)
            {
                LogComment("Fail - Currency on cv1 and cv2 should be the same (they're the same view)");
                return false;
            }
            return true;
        }

        #endregion

        #region NamedView
        private TestResult TestNamedView()
        {
            Status("TestNamedView");

            if (!TestNamedViewHelper(_mls)) { return TestResult.Fail; }
            if (!TestNamedViewHelper(_ie)) { return TestResult.Fail; }

            LogComment("TestNamedView was successful");
            return TestResult.Pass;
        }

        private bool TestNamedViewHelper(object obj)
        {
            CollectionViewSource cvs1 = new CollectionViewSource();
            cvs1.Source = obj;
            CollectionViewSource cvs2 = new CollectionViewSource();
            cvs2.Source = obj;

            ICollectionView cv1 = CollectionViewSource.GetDefaultView(obj);
            ICollectionView cv2 = cvs1.View;
            ICollectionView cv3 = cvs2.View;
            ICollectionView cv4 = cvs1.View;

            cv1.MoveCurrentToLast();

            cv2.MoveCurrentToFirst();
            cv2.MoveCurrentToNext();

            cv2.MoveCurrentToFirst();
            cv2.MoveCurrentToNext();
            cv2.MoveCurrentToNext();

            // cv1 and cv2 should be different
            if (cv1 == cv2)
            {
                LogComment("Fail - Default view and view1 should be different");
                return false;
            }
            if (cv1.CurrentItem == cv2.CurrentItem)
            {
                LogComment("Fail - Currency on default view and view1 should be different");
                return false;
            }
            // cv2 and cv3 should be different
            if (cv2 == cv3)
            {
                LogComment("Fail - view1 and view2 should be different");
                return false;
            }
            if (cv2.CurrentItem == cv3.CurrentItem)
            {
                LogComment("Fail - Currency on view1 and view2 should be different");
                return false;
            }
            // cv2 and cv4 should be the same
            if (cv2 != cv4)
            {
                LogComment("Fail - cv2 and cv4 should be the same view - view1");
                return false;
            }
            if (cv2.CurrentItem != cv4.CurrentItem)
            {
                LogComment("Fail - Currency on cv2 and cv4 should be the same");
                return false;
            }
            return true;
        }
        #endregion

        #region CustomView
        private TestResult TestCustomView()
        {
            Status("TestCustomView");

            if(!TestCustomViewHelper(_mls)) { return TestResult.Fail; }
            if (!TestCustomViewHelper(_ie)) { return TestResult.Fail; }
            if (!TestCustomViewExceptionCheck(_mls)) { return TestResult.Fail; }
            if (!TestCustomViewExceptionCheck(_ie)) { return TestResult.Fail; }

            LogComment("TestCustomView was successful");
            return TestResult.Pass;
        }

        private bool TestCustomViewHelper(object obj)
        {
            CollectionViewSource cvs1 = new CollectionViewSource();
            ((ISupportInitialize)cvs1).BeginInit();
            cvs1.CollectionViewType = typeof(MyOwnCollectionView);
            cvs1.Source = obj;
            ((ISupportInitialize)cvs1).EndInit();
            CollectionViewSource cvs2 = new CollectionViewSource();
            ((ISupportInitialize)cvs2).BeginInit();
            cvs2.CollectionViewType = typeof(MyOwnCollectionView);
            cvs2.Source = obj;
            ((ISupportInitialize)cvs2).EndInit();
            ICollectionView cv1 = cvs1.View;
            ICollectionView cv2 = cvs1.View;
            ICollectionView cv3 = cvs1.View;
            ICollectionView cv4 = cvs2.View;
            ICollectionView cv5 = CollectionViewSource.GetDefaultView(obj);

            cv1.MoveCurrentToLast();
            if (cv1.GetType() != typeof(MyOwnCollectionView))
            {
                LogComment("Fail - Custom view should be of type MyOwnCollectionView, instead it is of type " + cv1.GetType());
                return false;
            }
            cv4.MoveCurrentToFirst();
            cv4.MoveCurrentToNext();

            cv5.MoveCurrentToFirst();
            cv5.MoveCurrentToNext();
            cv5.MoveCurrentToNext();

            // cv1 and cv2 should be the same
            if (cv1 != cv2)
            {
                LogComment("Fail - cv1 and cv2 should be the same view - customView1");
                return false;
            }
            if (cv1.CurrentItem != cv2.CurrentItem)
            {
                LogComment("Fail - Currency in cv1 and cv2 should be the same");
                return false;
            }
            // cv1 and cv3 should be the same
            if (cv1 != cv3)
            {
                LogComment("Fail - cv1 and cv3 should be the same view - customView1");
                return false;
            }
            if (cv1.CurrentItem != cv3.CurrentItem)
            {
                LogComment("Fail - Currency in cv1 and cv3 should be the same");
                return false;
            }
            // cv1 and cv4 should be different
            if (cv1 == cv4)
            {
                LogComment("Fail - cv1 and cv4 should be different");
                return false;
            }
            if (cv1.CurrentItem == cv4.CurrentItem)
            {
                LogComment("Fail - Currency in cv1 and cv4 should be different");
                return false;
            }
            // cv1 and cv5 should be different
            if (cv1 == cv5)
            {
                LogComment("Fail - cv1 and cv5 should be different");
                return false;
            }
            if (cv1.CurrentItem == cv5.CurrentItem)
            {
                LogComment("Fail - Currency in cv1 and cv5 should be different");
                return false;
            }
            return true;
        }

        private bool TestCustomViewExceptionCheck(object obj)
        {
            // view type is not correct for the collection+name passed
            CollectionViewSource cvs1 = new CollectionViewSource();
            ((ISupportInitialize)cvs1).BeginInit();
            cvs1.CollectionViewType = typeof(MyOwnCollectionView);
            cvs1.Source = obj;
            ((ISupportInitialize)cvs1).EndInit();
            bool throwsException3 = false;
            try
            {
                cvs1.CollectionViewType = typeof(MyOtherCollectionView);
            }
            catch (InvalidOperationException ae)
            {
                Status("Expected exception:" + ae.Message + " - " + ae.GetType());
                throwsException3 = true;
            }
            catch (Exception e)
            {
                LogComment("Fail - Exception does not have expected type. Actual:" + e.Message + " - " + e.GetType());
                return false;
            }
            if (!throwsException3)
            {
                LogComment("Fail - No exception was thrown. Expected a InvalidOperationException.");
                return false;
            }

            // view type does not have a constructor that accepts that type of collection
            bool throwsException4 = false;
            try
            {
                CollectionViewSource cvs3 = new CollectionViewSource();
                ((ISupportInitialize)cvs3).BeginInit();
                cvs3.CollectionViewType = typeof(BindingListCollectionView);
                cvs3.Source = obj;
                ((ISupportInitialize)cvs3).EndInit();
            }
            catch (ArgumentException ae)
            {
                Status("Expected exception:" + ae.Message + " - " + ae.GetType());
                throwsException4 = true;
            }
            catch (Exception e)
            {
                LogComment("Fail - Exception does not have expected type. Actual:" + e.Message + " - " + e.GetType());
                return false;
            }
            if (!throwsException4)
            {
                LogComment("Fail - No exception was thrown. Expected a ArgumentException.");
                return false;
            }
            return true;
        }
        #endregion

        #region MakeNamedViewDefault
        private TestResult MakeNamedViewDefault()
        {
            Status("MakeNamedViewDefault");

            _lb.ItemsSource = _bookList;

            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = _bookList;
            ICollectionView cv1 = cvs.View;
            Binding b = new Binding("");
            b.Source = cvs;
            _lb.SetBinding(ListBox.ItemsSourceProperty, b);

            ICollectionView cv2 = CollectionViewSource.GetDefaultView(_lb.ItemsSource);

            // If the items source is bound to a view, doing:
            // GetDefaultView - returns the actual view
            // GetNamedView - creates a new view that has SourceCollection = the view from the items source

//          ICollectionView cv3 = BindingOperations.GetNamedView(lb.ItemsSource, "namedView10");
//          IEnumerable source = cv3.SourceCollection;
//
//          bool theSame1 = (cv2 == cv3); // false
//          bool theSame2 = (cv2 == source); // true

            if (cv1 != cv2)
            {
                LogComment("Fail - cv1 and cv2 should be the same");
                return TestResult.Fail;
            }

            LogComment("MakeNamedViewDefault was successful");
            return TestResult.Pass;
        }
        #endregion

        #region MakeCustomViewDefault
        private TestResult MakeCustomViewDefault()
        {
            Status("MakeCustomViewDefault");

            _lb.ItemsSource = _bookList;

            CollectionViewSource cvs = new CollectionViewSource();
            ((ISupportInitialize)cvs).BeginInit();
            cvs.CollectionViewType = typeof(MyOwnCollectionView);
            cvs.Source = _bookList;
            ((ISupportInitialize)cvs).EndInit();
            ICollectionView cv1 = cvs.View;
            Binding b = new Binding("");
            b.Source = cvs;
            _lb.SetBinding(ListBox.ItemsSourceProperty, b);

            ICollectionView cv2 = CollectionViewSource.GetDefaultView(_lb.ItemsSource);

            if (cv1 != cv2)
            {
                LogComment("Fail - cv1 and cv2 should be the same");
                return TestResult.Fail;
            }

            LogComment("MakeCustomViewDefault was successful");
            return TestResult.Pass;
        }
        #endregion
    }

    public class MyOwnCollectionView : ListCollectionView
    {
        public MyOwnCollectionView(ObservableCollection<BookSource> col)
            : base((IList)col)
        {
        }
        public MyOwnCollectionView(IList col) : base(col)
        {
        }
    }

    public class MyOtherCollectionView : ListCollectionView
    {
        public MyOtherCollectionView(ObservableCollection<BookSource> col)
            : base((IList)col)
        {
        }
        public MyOtherCollectionView(IList col) : base(col)
        {
        }
    }
}

