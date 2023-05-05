// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Windows.Controls;
using Microsoft.Test;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This is a get set test for System.Collections.Generic.IList&lt;Binding&gt;
    /// </description>
    /// </summary>
    [Test(3, "Binding", "BindListTest")]
    public class BindListTest : WindowTest
    {
        MultiBinding _mb;
        Object[] _values;
        ObservableCollection<CLRBook> _verifyArrayList;
      //  ArrayList verifyArrayList;
        Binding _bTitle,_bYear,_bAuthor,_bPrice;
        TextBox _tb;
        string _step;
        ConverterVerifier _convertVerifier;
        public BindListTest()
        {
        //  RunSteps += new TestStep(Test);

            InitializeSteps += new TestStep(CreateTest);
            RunSteps += new TestStep(VerifyConverter);
            RunSteps += new TestStep(AddToList);
            RunSteps += new TestStep(VerifyConverter);
            RunSteps += new TestStep(RemoveFromList);
            RunSteps += new TestStep(VerifyConverter);
            RunSteps += new TestStep(IListRemoveFromList);
            RunSteps += new TestStep(VerifyConverter);
            RunSteps += new TestStep(IndexOfObjInList);
            RunSteps += new TestStep(ContainsInList);
            RunSteps += new TestStep(InsertInToList);
            RunSteps += new TestStep(VerifyConverter);
            RunSteps += new TestStep(CheckProperties);
            RunSteps += new TestStep(CopyToList);
            RunSteps += new TestStep(AssignTo);
            RunSteps += new TestStep(VerifyConverter);
            RunSteps += new TestStep(ClearList);
            RunSteps += new TestStep(VerifyConverter);

        }
        private ObservableCollection<CLRBook> CreateData()
        {
                ObservableCollection<CLRBook> books = new ObservableCollection<CLRBook>();
                CLRBook item = new CLRBook("Homo Faber", "Max Frisch", 1957, 14.92, BookType.Novel);
            books.Add(item);
            item = new CLRBook("The Fourth Hand", "John Irving", 2001, 14.91, BookType.Novel);
            books.Add(item);
            item = new CLRBook("Inside C#", "Tom Archer e.a.", 2002, 49.99, BookType.Reference);
            books.Add(item);
            item = new CLRBook("A Man in Full", "Tom Wolfe", 1998, 8.95, BookType.Novel);
            books.Add(item);


            return books;
        }
        private TestResult CreateTest()
        {
            _verifyArrayList = CreateData();
            _convertVerifier = new ConverterVerifier();
            _tb = new TextBox();
            _mb = new MultiBinding();
            _mb.Converter = new MBConverter();
            _bTitle = new Binding("Title");
            _mb.Bindings.Add(_bTitle);
            _bYear = new Binding("Year");
            _mb.Bindings.Add(_bYear);
            _bPrice = new Binding("Price");
            _mb.Bindings.Add(_bPrice);
            _tb.SetBinding(TextBox.TextProperty, _mb);

            DockPanel dp = new DockPanel();
            dp.DataContext = CreateData();
            dp.Children.Add(_tb);
            Window.Content = dp;
            _values = new object[] { ((CLRBook)_verifyArrayList[0]).Title, ((CLRBook)_verifyArrayList[0]).Year, ((CLRBook)_verifyArrayList[0]).Price};
            _step = "Setting up BindList";
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }
        private TestResult VerifyConverter()
        {
            IVerifyResult res = _convertVerifier.Verify(_values, _step);
            LogComment(res.Message);
            return res.Result;
        }
        private TestResult AddToList()
        {
            Status("Adding bind to the BindList");
            MultiBinding newMB = new MultiBinding();
            newMB.Converter = _mb.Converter;
            foreach (BindingBase bb in _mb.Bindings)
            {
                newMB.Bindings.Add(bb);
            }
            _mb = newMB;

            try
            {
                _mb.Bindings.Add(null);
                LogComment("Add(null) didn't throw");
                return TestResult.Fail;
            }
            catch (Exception e)
            {
                Status("Expected Error: " + e.Message);
            }

            _values = new object[] { ((CLRBook)_verifyArrayList[0]).Title, ((CLRBook)_verifyArrayList[0]).Year, ((CLRBook)_verifyArrayList[0]).Price, ((CLRBook)_verifyArrayList[0]).Author };
            _step = "Add to BindList";

            _bAuthor = new Binding("Author");
            _mb.Bindings.Add(_bAuthor);
            _tb.SetBinding(TextBox.TextProperty, _mb);

            return TestResult.Pass;
        }
        private TestResult RemoveFromList()
        {
            Status("Removing Binding from Bindlist");
            MultiBinding newMB = new MultiBinding();
            newMB.Converter = _mb.Converter;
            foreach (BindingBase bb in _mb.Bindings)
            {
                newMB.Bindings.Add(bb);
            }
            _mb = newMB;

            _values = new object[] { ((CLRBook)_verifyArrayList[0]).Title, ((CLRBook)_verifyArrayList[0]).Price, ((CLRBook)_verifyArrayList[0]).Author };
            _step = "Remove obj from BindList";
            _mb.Bindings.Remove(_bYear);
            _tb.SetBinding(TextBox.TextProperty, _mb);
            return TestResult.Pass;

        }

        private TestResult IListRemoveFromList()
        {
            Status("Removing from the BindList");
            MultiBinding newMB = new MultiBinding();
            newMB.Converter = _mb.Converter;
            foreach (BindingBase bb in _mb.Bindings)
            {
                newMB.Bindings.Add(bb);
            }
            _mb = newMB;

            _values = new object[] { ((CLRBook)_verifyArrayList[0]).Title, ((CLRBook)_verifyArrayList[0]).Author };
            _step = "(Ilist)Remove from BindList";
            ((IList)_mb.Bindings).Remove(((IList)_mb.Bindings)[1]);
            _tb.SetBinding(TextBox.TextProperty, _mb);

            return TestResult.Pass;
        }

        private TestResult IndexOfObjInList()
        {
            if (((IList)_mb.Bindings).IndexOf(null) >= 0)
            {
                LogComment("IndexOf(null) didn't return -1");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        private TestResult ContainsInList()
        {
            if (((IList)_mb.Bindings).Contains(null))
            {
                LogComment("Binds.Contains() couldn't handle a null value!");
                return TestResult.Fail;
            }

            if (((IList)_mb.Bindings).Contains(_bPrice))
            {
                LogComment("Binds.Contains() contains a value, it shouldn't!" );
                return TestResult.Fail;
            }
            if (!((IList)_mb.Bindings).Contains(_bAuthor))
            {
                LogComment("Binds.Contains() doesn't contains a value, it should!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult InsertInToList()
        {
            MultiBinding newMB = new MultiBinding();
            newMB.Converter = _mb.Converter;
            foreach (BindingBase bb in _mb.Bindings)
            {
                newMB.Bindings.Add(bb);
            }
            _mb = newMB;

            try
            {
                ((IList)_mb.Bindings).Insert(0, null);
                LogComment("Insert ignored a null value");
                return TestResult.Fail;
            }
            catch (Exception e)
            {
                Status("Expected exception: " + e.Message);
            }

            try
            {
                ((IList)_mb.Bindings).Insert(0, new object());
                LogComment("Insert ignored a bad value");
                return TestResult.Fail;
            }
            catch (Exception e)
            {
                Status("Expected exception: " + e.Message);
            }

            _values = new object[] { ((CLRBook)_verifyArrayList[0]).Price, ((CLRBook)_verifyArrayList[0]).Title, ((CLRBook)_verifyArrayList[0]).Author };
            _step = "Insert to BindList";
            ((IList)_mb.Bindings).Insert(0, _bPrice);
            _tb.SetBinding(TextBox.TextProperty, _mb);
            return TestResult.Pass;
        }
        private TestResult CheckProperties()
        {
            if (((IList)_mb.Bindings).IsFixedSize)
            {
                return TestResult.Fail;
            }
            if (((IList)_mb.Bindings).IsReadOnly)
            {
                return TestResult.Fail;
            }
            if (((ICollection)_mb.Bindings).IsSynchronized)
            {
                return TestResult.Fail;
            }
            IEnumerator _enumerator = ((ICollection)_mb.Bindings).GetEnumerator() as IEnumerator;
            if (_enumerator == null)
            {
                LogComment("Couldn't GetEnumerator()");
                return TestResult.Fail;
            }
            _enumerator.MoveNext();

            if (((Binding)_enumerator.Current).Path.Path != "Price")
            {
                LogComment("Current Item in enumerator was incorrect");
                return TestResult.Fail;
            }
            return TestResult.Pass;

        }
        private TestResult CopyToList()
        {
            Status("CopyToList");
            Binding[] b = new Binding[4];
            _mb.Bindings.CopyTo(b,1);
            if (b[0] != null)
            {
                LogComment("CopyTo didn't copy correctly");
                return TestResult.Fail;
            }
            if (b[1].Path.Path != "Price" || b[2].Path.Path != "Title" || b[3].Path.Path != "Author")
            {
                LogComment("CopyTo didn't copy correctly");
                return TestResult.Fail;
            }
            Binding[] bb = new Binding[4];
            ((IList)_mb.Bindings).CopyTo(bb, 0);
            if (bb[0].Path.Path != "Price" || bb[1].Path.Path != "Title" || bb[2].Path.Path != "Author")
            {
                LogComment("(IList)CopyTo didn't copy correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        private TestResult AssignTo()
        {
            MultiBinding newMB = new MultiBinding();
            newMB.Converter = _mb.Converter;
            foreach (BindingBase bb in _mb.Bindings)
            {
                newMB.Bindings.Add(bb);
            }
            _mb = newMB;

            try
            {
                _mb.Bindings[0] = null;
                LogComment("Assigning null failed to throw");
                return TestResult.Fail;
            }
            catch (Exception e)
            {
                Status("Expected Error message: " + e.Message);
            }

            try
            {
                ((IList)_mb.Bindings)[0] = "BadValue";
                LogComment("Assigning Bad Value failed to throw");
                return TestResult.Fail;
            }
            catch (Exception e)
            {
                Status("Expected Error message: " + e.Message);
            }

            try
            {
                ((IList)_mb.Bindings)[0] = null;
                LogComment("Assigning null failed to throw");
                return TestResult.Fail;
            }
            catch (Exception e)
            {
                Status("Expected Error message: " + e.Message);
            }

            _values = new object[] { ((CLRBook)_verifyArrayList[0]).Year, ((CLRBook)_verifyArrayList[0]).Title, ((CLRBook)_verifyArrayList[0]).Author };
            _step = "Insert to BindList";

            ((IList)_mb.Bindings)[0] = _bYear;
            _tb.SetBinding(TextBox.TextProperty, _mb);

            return TestResult.Pass;
        }
        private TestResult ClearList()
        {
            MultiBinding newMB = new MultiBinding();
            newMB.Converter = _mb.Converter;
            foreach (BindingBase bb in _mb.Bindings)
            {
                newMB.Bindings.Add(bb);
            }
            _mb = newMB;

            _values = new object[] { };
            _step = "Clear BindList";

            _mb.Bindings.Clear();
            _tb.SetBinding(TextBox.TextProperty, _mb);
            return TestResult.Pass;
        }
    }


    public class MBConverter : IMultiValueConverter
    {
        #region static properties

        static object s_paramvalue;
        static public object ParameterValue
        {
            get { return s_paramvalue; }
        }

        static int s_count;
        static public int Count
        {
            get { return s_count; }
        }

        static CultureInfo s_culture;
        static public CultureInfo Culture
        {
            get { return s_culture; }
        }
        static ArrayList s_values;

        static public ArrayList Values
        {
            get { return s_values; }
            set { s_values = value; }
        }
        #endregion
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            s_paramvalue = parameter;
            s_culture = culture;
            ArrayList al = new ArrayList();


            string s = "";
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != DependencyProperty.UnsetValue)
                    if (values[i] is string)
                        s += values[i] + " ";
                    else
                        s += values[i].ToString() + " ";
                al.Add(values[i]);
            }
            s_values = al;
            GlobalLog.LogStatus("ConverterDone");
            return s;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            s_count++;

            object[] o = new object[2];
            string[] _splitValues = ((string)value).Split(' ');

            o[0] = _splitValues[0];
            for (int i = 0; i < 2; i++)
            {
                string _targetType = targetTypes[i].ToString();

                switch (_targetType)
                {
                    case "System.String":
                        o[i] = _splitValues[i];
                        break;

                    case "System.Double":
                        if (_splitValues[i].ToString() == "13")
                            o[i] = DependencyProperty.UnsetValue;
                        else
                            o[i] = double.Parse(_splitValues[i].ToString());
                        break;
                }
            }

            return o;
        }

    }

    public class ConverterVerifier : IVerifier
    {
        #region Constructor

        public ConverterVerifier()
        {
        }

        #endregion


        #region IVerifier implementation

        public IVerifyResult Verify(params object[] expectedState)
        {
            int i;
            object[] o = expectedState[0] as object[];
            for (i = 0; i < o.Length - 1; i++)
            {
                if (MBConverter.Values[i].ToString() == null || o[i].ToString() != MBConverter.Values[i].ToString())
                {
                    return new VerifyResult(TestResult.Fail, "Collection not correct after step: " + expectedState[expectedState.Length - 1]);
                }
            }
            return new VerifyResult(TestResult.Pass, expectedState[expectedState.Length - 1] + " was successful");
        }
        #endregion

    }
}



