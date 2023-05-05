// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using System.IO;

namespace ElementLayout.FeatureTests.Property
{
    //////////////////////////////////////////////////////////////////
    /// This contains code for UIElementCollection Tests.
    //////////////////////////////////////////////////////////////////

    [Test(2, "Property.UiElementCollection", "UiElementCollectionTest", Variables="Area=ElementLayout")]
    public class UiElementCollectionTest : CodeTest
    {
        public UiElementCollectionTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 700;
            this.window.Top = 25;
            this.window.Left = 25;

            this.window.Content = this.TestContent();
        }

        StackPanel _root;
        public override FrameworkElement TestContent()
        {
            _root = new StackPanel();
            _root.Background = Brushes.RoyalBlue;
            return _root;
        }

        int _childCount = 156;
        UIElementCollection _uic;

        public override void TestActions()
        {
            //null visual parent in uielementcollection ctor.
            try
            {
                _uic = new UIElementCollection(null, null);
                _fail_comments += (" NULL PARENT IN ctor : UIElementCollection ctor should have thrown an Exception with null visual parent.");
                _tempresult = false;
            }
            catch (Exception ctor_EX)
            {
                Helpers.Log("NULL PARENT IN ctor : Exception Caught, Test Passed.");
                Helpers.Log(ctor_EX.Message);
            }

            _uic = _root.Children;

            //no children test
            Helpers.Log("UIC Count " + _uic.Count + ".");
            Helpers.Log("UIC Capacity " + _uic.Capacity + ".");

            if (_uic.Count != 0)
            {
                _fail_comments += " NO CHILD : Count is not zero. ";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("NO CHILD : Count Test Passed.");
            }

            if (_uic.Capacity != 0)
            {
                _fail_comments += (" NO CHILD : Capacity is not zero");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("NO CHILD : Capacity Test Passed");
            }

            AddChildren(_childCount, _root);
            CommonFunctionality.FlushDispatcher();

            //children have been added. 
            //verify children count is expected and capacity is greater than child count
            Helpers.Log("UIC Count " + _uic.Count + ".");
            Helpers.Log("UIC Capacity " + _uic.Capacity + ".");

            if (_uic.Count != _childCount)
            {
                _fail_comments += " COLLECTION HAS CHILDREN : Count is not expected value. ";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("COLLECTION HAS CHILDREN : Count Test Passed.");
            }

            if (_uic.Capacity <= _childCount)
            {
                _fail_comments += (" COLLECTION HAS CHILDREN : Capacity is not greater than Child Count");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("COLLECTION HAS CHILDREN : Capacity Test Passed");
            }

            _uic.Capacity = _uic.Count;
            CommonFunctionality.FlushDispatcher();

            //collection has been trimmed.  
            //count should equal capacity
            Helpers.Log("UIC Count " + _uic.Count + ".");
            Helpers.Log("UIC Capacity " + _uic.Capacity + ".");

            if (_uic.Count != _childCount)
            {
                _fail_comments += " TRIM COLLECTION : Count is not expected value. ";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("TRIM COLLECTION : Count Test Passed.");
            }

            if (_uic.Capacity != _childCount)
            {
                _fail_comments += (" TRIM COLLECTION : Capacity is not equal to Child Count");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("TRIM COLLECTION : Capacity Test Passed");
            }

            AddChildren(1, _root);
            CommonFunctionality.FlushDispatcher();

            //child added to trimmed collection
            //capacity should be pushed past bounds
            Helpers.Log("UIC Count " + _uic.Count + ".");
            Helpers.Log("UIC Capacity " + _uic.Capacity + ".");

            if (_uic.Count != (_childCount + 1))
            {
                _fail_comments += " CHILD AFTER TRIM : Count is not expected value. ";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("CHILD AFTER TRIM : Count Test Passed.");
            }

            if (_uic.Capacity <= (_childCount + 1))
            {
                _fail_comments += (" CHILD AFTER TRIM : Capacity is not equal to Child Count");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("CHILD AFTER TRIM : Capacity Test Passed");
            }

            CommonFunctionality.FlushDispatcher();
            //capacity test.  should not be able to set capacity to less than child count.

            try
            {
                _uic.Capacity = 5;
                _fail_comments += (" CAPACITY LESS THAN : Should not be able to set Capacity to less than child count.");
                _tempresult = false;
            }
            catch (Exception capacity_EX)
            {
                Helpers.Log("CAPACITY LESS THAN : Exception Caught, Test Passed.");
                Helpers.Log(capacity_EX.Message);
            }

            _root.Children.Clear();
            _childCount = 57;
            CommonFunctionality.FlushDispatcher();
            AddChildren(_childCount, _root);
            CommonFunctionality.FlushDispatcher();


            //copy to test.  copy child collect from root to a object array, then back to root.
            object[] uic_copy;

            _uic = _root.Children;

            uic_copy = new object[_uic.Count];

            try
            {
                _uic.CopyTo(uic_copy, 0);
                Helpers.Log("COPY TO : CopyTo Test passed.");
            }
            catch (Exception copyto_EX)
            {
                _fail_comments += " COPY TO : CopyTo Test failed. ";
                _tempresult = false;
                Helpers.Log(copyto_EX.Message);
            }

            if (_uic.Count != uic_copy.Length)
            {
                _fail_comments += " COPY TO : Original Count is not equal to Copy Count. ";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("COPY TO : Original Count is equal to Copy Count.");
            }

            //child test after copy to.
            if (_uic.Count != _childCount)
            {
                _fail_comments += " CHILD AFTER COPY TO : Count is not expected value. ";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("CHILD AFTER COPY TO : Count Test Passed.");
            }

            if (_uic.Capacity <= _childCount)
            {
                _fail_comments += (" CHILD AFTER COPY TO : Capacity is not greater than Child Count");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("CHILD AFTER COPY TO : Capacity Test Passed");
            }

            _root.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            for (int i = 1; i <= uic_copy.Length; i++)
            {
                object pre_moved_child = uic_copy[uic_copy.Length - i];
                UIElement moved_child = pre_moved_child as UIElement;
                _root.Children.Add(moved_child);
            }

            _uic = _root.Children;

            Helpers.Log("UIC Count " + _uic.Count + ".");
            Helpers.Log("UIC Capacity " + _uic.Capacity + ".");

            if (_uic.Count != _childCount)
            {
                _fail_comments += " CHILD AFTER COPY TO / ADD AGAIN : Count is not expected value. ";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("CHILD AFTER COPY TO / ADD AGAIN : Count Test Passed.");
            }

            if (_uic.Capacity <= (_childCount + 1))
            {
                _fail_comments += (" CHILD AFTER COPY TO / ADD AGAIN : Capacity is not greater than Child Count");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("CHILD AFTER COPY TO / ADD AGAIN : Capacity Test Passed");
            }

            //set item to null
            Border null_border = null;
            try
            {
                _uic[3] = null_border;
                _fail_comments += " SET ITEM : Set Item should have thrown exception with null element. ";
                _tempresult = false;
            }
            catch (Exception setitem_EX)
            {
                Helpers.Log("SET ITEM : Set Item threw exception with null element, Test Passed.");
                Helpers.Log(setitem_EX.Message);
            }

            CommonFunctionality.FlushDispatcher();
            LoadFinalTest();
            CommonFunctionality.FlushDispatcher();

            Border nonBoundChild = new Border();
            StackPanel stack = null;
            ItemsControl ic = LogicalTreeHelper.FindLogicalNode(window, "myItemsControl") as ItemsControl;
            if (ic != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(ic);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject v = VisualTreeHelper.GetChild(ic, i);
                    if (v.GetType().Name == "StackPanel")
                    {
                        stack = v as StackPanel;
                    }
                }
            }

            if (stack != null)
            {
                try
                {
                    stack.Children.Remove(stack.Children[3]);
                    _fail_comments += (" REMOVE CHILD FROM BOUND PARENT : Should not be able to remove a child from a bound parent.");
                    _tempresult = false;
                }
                catch (Exception boundRemove_EX)
                {
                    Helpers.Log("REMOVE CHILD FROM BOUND PARENT : Exception Caught, Test Passed.");
                    Helpers.Log(boundRemove_EX.Message);
                }

                try
                {
                    stack.Children.Add(nonBoundChild);
                    _fail_comments += (" ADD CHILD TO BOUND PARENT : Should not be able to add a child to a bound parent.");
                    _tempresult = false;
                }
                catch (Exception boundAdd_EX)
                {
                    Helpers.Log("ADD CHILD TO BOUND PARENT : Exception Caught, Test Passed.");
                    Helpers.Log(boundAdd_EX.Message);
                }
            }
            else
            {
                Helpers.Log("Bound Stack was null, so this test fails.");
                _tempresult = false;
            }


            //set item to real item??  bring this back if code coverage is not hit.

            //Border border = new Border();
            //try
            //{
            //    uic.RemoveAt(10);
            //    uic.Insert(10, border);
            //    border.Height = 10;
            //    border.Width = 10;
            //    border.Background = Brushes.Crimson;
            //    Helpers.Log("SET ITEM : Set Item did not throw exception with real element, Test Passed.");
            //}
            //catch (Exception setitem_EX)
            //{
            //    fail_comments += " SET ITEM : Set Item should not have thrown exception with real element. ";
            //    tempresult = false;
            //    Helpers.Log(setitem_EX.Message);
            //}

        }

        void LoadFinalTest()
        {
            window.Content = null;
            CommonFunctionality.FlushDispatcher();

            System.IO.FileStream f = new System.IO.FileStream("BoundParent.xaml", FileMode.Open, FileAccess.Read);
            window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);
            f.Close();

            while (!_testLoaded)
            {
                isLoaded();
                CommonFunctionality.FlushDispatcher();
            }
        }

        bool _testLoaded = false;
        void isLoaded()
        {
            ItemsControl ic = LogicalTreeHelper.FindLogicalNode(window, "myItemsControl") as ItemsControl;
            if (ic.HasItems)
            {
                _testLoaded = true;
            }
            else
            {
                _testLoaded = false;
            }
        }

        string _fail_comments = "";
        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        public static void AddChildren(int count, Panel root)
        {
            for (int i = 0; i < count; i++)
            {
                TextBlock txt = new TextBlock();
                txt.Text = i.ToString();
                root.Children.Add(txt);
            }
        }
    }
}
