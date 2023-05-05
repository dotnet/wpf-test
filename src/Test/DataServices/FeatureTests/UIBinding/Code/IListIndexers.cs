// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices.IListIndexers
{
    /// <summary>
    /// <description>
    /// This tests bindings using indexers that do or do not agree with the IList indexer
    /// </description>
    /// </summary>
    [Test(1, "Binding", "IListIndexers", Versions="4.8+")]
    public class IListIndexers : XamlTest
    {
        Model _model = new Model();
        TextBlock _tbNormal, _tbOutOfRange;
        MethodInfo _miEnableExceptionLogging;
        PropertyInfo _piLog;
        ItemToStringConverter _itemToStringConverter;

        public IListIndexers()
                : base(@"IListIndexers.xaml")
        {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(VerifyIndexers);
        }

        TestResult Init()
        {
            RootElement.DataContext = _model;
            _tbNormal = (TextBlock)RootElement.FindName("tbNormal");
            _tbOutOfRange = (TextBlock)RootElement.FindName("tbOutOfRange");
            _itemToStringConverter = RootElement.Resources["ItemToStringConverter"] as ItemToStringConverter;

            if (_tbNormal == null || _tbOutOfRange == null || _itemToStringConverter == null)
            {
                LogComment("Unable to load XAML elements.");
                return TestResult.Fail;
            }

            _miEnableExceptionLogging = typeof(BindingOperations).GetMethod("EnableExceptionLogging",
                BindingFlags.Static | BindingFlags.NonPublic);

            return TestResult.Pass;
        }

        TestResult VerifyIndexers()
        {
            bool pass = true;

            // Array
            string[] stringArray = new string[] { "String 0" };
            pass = VerifyIndexer(stringArray, stringArray[0], exceptionAllowed:false) && pass;

            // ArrayList
            ArrayList al = new ArrayList();
            al.Add("String 0");
            pass = VerifyIndexer(al, al[0], exceptionAllowed:false) && pass;

            // ArrayList with override for this[]
            al = new DerivedArrayList();
            al.Add("String 0");
            al.Add("String 1");
            pass = VerifyIndexer(al, al[0], exceptionAllowed:true) && pass;

            // type-safe subclass of CollectionBase
            DerivedCollection dc = new DerivedCollection();
            dc.Add("String 0");
            pass = VerifyIndexer(dc, dc[0], exceptionAllowed:true) && pass;

            // LinkTargetCollection - as above, but built-in so exception optimization should apply
            LinkTargetCollection ltc = new LinkTargetCollection();
            ltc.Add(new LinkTarget{ Name="Target 0" });
            pass = VerifyIndexer(ltc, ltc[0], exceptionAllowed:false) && pass;

            // List<T>
            ListOfString list = new ListOfString();
            list.Add("String 0");
            pass = VerifyIndexer(list, list[0], exceptionAllowed:false) && pass;

            // Collection<T>
            CollectionOfString collection = new CollectionOfString();
            collection.Add("String 0");
            pass = VerifyIndexer(collection, collection[0], exceptionAllowed:false) && pass;

            // ReadOnlyCollection<T>
            ReadOnlyCollectionOfString roCollection = new ReadOnlyCollectionOfString(collection);
            pass = VerifyIndexer(roCollection, roCollection[0], exceptionAllowed:false) && pass;

            // StringCollection
            StringCollection sc = new StringCollection();
            sc.Add("String 0");
            pass = VerifyIndexer(sc, sc[0], exceptionAllowed:false) && pass;


            SimpleOrder so = new SimpleOrder();
            so.Add(new OrderItem {PartNumber=23, Description="Part 23"});
            so.Add(new OrderItem {PartNumber=0, Description="Part 0"});
            pass = VerifyIndexer(so, so[0], exceptionAllowed:true) && pass;


            UnsignedSimpleOrder uso = new UnsignedSimpleOrder();
            uso.Add(new UnsignedOrderItem {PartNumber=23, Description="Part 23"});
            uso.Add(new UnsignedOrderItem {PartNumber=0, Description="Part 0"});
            pass = VerifyIndexer(uso, uso[0], exceptionAllowed:true) && pass;

            return pass ? TestResult.Pass : TestResult.Fail;
        }

        bool VerifyIndexer(IList ilist, object expectedObject, bool exceptionAllowed=true)
        {
            string expected = _itemToStringConverter.Convert(expectedObject, typeof(string), null, null) as string;
            Exception ex = null;

            using (IDisposable logger = EnableExceptionLogging())
            {
                _model.Strings = ilist;

                ex = GetException(logger);
            }
            WaitForPriority(DispatcherPriority.Background);

            string observed = _tbNormal.Text;
            if (!Object.Equals(observed, expected))
            {
                LogComment(String.Format("Mismatch using {0}.  Expected: '{1}'  Observed: '{2}'",
                    ilist.GetType().Name, expected, observed));
                return false;
            }

            if (!exceptionAllowed && ex != null)
            {
                LogComment(String.Format("Unexpected exception using {0}: {1}",
                    ilist.GetType().Name, ex));
                return false;
            }

            return true;
        }

        IDisposable EnableExceptionLogging()
        {
            return (_miEnableExceptionLogging == null) ? null :
                _miEnableExceptionLogging.Invoke(null, null) as IDisposable;
        }

        Exception GetException(IDisposable logger)
        {
            if (logger == null) return null;

            if (_piLog == null)
            {
                _piLog = logger.GetType().GetProperty("Log",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (_piLog == null) return null;
            }

            List<Exception> log = _piLog.GetValue(logger, null) as List<Exception>;
            if (log == null || log.Count == 0) return null;

            return log[0];
        }
    }

    public class Model : INotifyPropertyChanged
    {
        IList _strings;
        public IList Strings
        {
            get { return _strings; }
            set { _strings = value;  OnPropertyChanged("Strings"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class ItemToStringConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            if (type != typeof(String) || o is string)
                return o;

            LinkTarget lt = o as LinkTarget;
            if (lt != null)
                return lt.Name;

            OrderItem oi = o as OrderItem;
            if (oi != null)
                return oi.Description;

            UnsignedOrderItem uoi = o as UnsignedOrderItem;
            if (uoi != null)
                return uoi.Description;

            return o;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #region IList implementations to test

    public class DerivedArrayList : ArrayList
    {
        public override object this[int index]
        {
            get { return base[index + 1]; }
            set { base[index + 1] = value; }
        }
    }

    public class DerivedCollection : CollectionBase
    {
        public String this[int index]
        {
            get { return (String)((IList)this)[index]; }
            set { ((IList)this)[index] = value; }
        }

        public int Add(String value)
        {
            return ((IList)this).Add((object)value);
        }

        public void Remove(String value)
        {
            ((IList)this).Remove((object) value);
        }

        public bool Contains(String value)
        {
            return ((IList)this).Contains((object)value);
        }

        public void CopyTo(String[] array, int index)
        {
            ((ICollection)this).CopyTo(array, index);
        }

        public int IndexOf(String value)
        {
            return ((IList)this).IndexOf((object)value);
        }

        public void Insert(int index, String value)
        {
            ((IList)this).Insert(index, (object)value);
        }
    }

    public class ListOfString : List<String>
    {
    }

    public class CollectionOfString : Collection<String>
    {
    }

    public class ReadOnlyCollectionOfString : ReadOnlyCollection<String>
    {
        public ReadOnlyCollectionOfString(CollectionOfString coll) : base(coll as IList<String>)
        {
        }
    }

    public class SimpleOrder : KeyedCollection<int, OrderItem>
    {
        protected override int GetKeyForItem(OrderItem item)
        {
            return item.PartNumber;
        }
    }

    public class OrderItem
    {
        public int PartNumber { get; set; }
        public string Description { get; set; }
    }

    public class UnsignedSimpleOrder : KeyedCollection<uint, UnsignedOrderItem>
    {
        protected override uint GetKeyForItem(UnsignedOrderItem item)
        {
            return item.PartNumber;
        }
    }

    public class UnsignedOrderItem
    {
        public uint PartNumber { get; set; }
        public string Description { get; set; }
    }

    #endregion
}
