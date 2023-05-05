// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Provides coverage for bugs on Views.
    /// Make sure that after adding sort descriptions to an empty collection, new items added
    /// to that collection are in fact added in their sorted location. This is tested for CLR and XML objects.
	/// </description>
	/// <relatedBugs>




    /// </relatedBugs>
	/// </summary>
    [Test(2, "Views", "ViewsBugsCoverage")]
	public class ViewsBugsCoverage : XamlTest
    {
        #region Private Data

        ListBox _lb2;

        #endregion

        #region Constructors

        public ViewsBugsCoverage()
            : base(@"ViewsBugsCoverage.xaml")
		{
            RunSteps += new TestStep(RemoveItemOutOfViewPassingFilter);
            RunSteps += new TestStep(BindsDirectlyToSourceCVSMagic);
            RunSteps += new TestStep(DefaultViewAlwaysGenerated);
            RunSteps += new TestStep(SortDescriptionEmptyCollection);
        }

        #endregion

        #region Private Members

        // Exception thrown by CollectionView when removing an item from the source collection
        private TestResult RemoveItemOutOfViewPassingFilter()
        {
            Places places = new Places();
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(places);

            lcv.Filter = new Predicate<object>(delegate(object objectToFilter)
                {
                    Place place = (Place)objectToFilter;
                    return place.Name.StartsWith("B");
                });

            places[0].Name = "Burien";

            // This line causes the exception
            places.Remove(places[0]);

            return TestResult.Pass;
        }

        // BindsDirectlyToSource should also turn off CVS magic
        TestResult BindsDirectlyToSourceCVSMagic()
        {
            // Special CVS magic automatically drills into the CVS.View property, which
            // prevents you from binding to a property on the CVS itself. The
            // fix is to make BindsDirectlyToSource turn off CVS magic.

            MyCollectionViewSource mcvs = new MyCollectionViewSource();
            mcvs.Foo = "TestValue";

            TextBlock tb = new TextBlock();
            ((Panel)((Page)RootElement).Content).Children.Add(tb);

            Binding b = new Binding("Foo");
            b.Source = mcvs;
            b.BindsDirectlyToSource = true;

            tb.SetBinding(TextBlock.TextProperty, b);

            // Without turning off CVS magic we can't pick up the property value
            if (tb.Text != "TestValue") return TestResult.Fail;

            return TestResult.Pass;
        }

        // Default view is still created when we ask for a custom view
        TestResult DefaultViewAlwaysGenerated()
        {
            // The strategy is to create a collection that keeps track of how many
            // views have signed up to listen to it's CollectionChanged event. The
            // only view should be the one I made through CollectionViewSource, but
            // the bug was about a Default View being made anyways. Before the fix
            // mc.numTimes was greater than one.

            CountingCollection mc = new CountingCollection();
            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = mc;
            Binding b = new Binding();
            b.Source = cvs;
            ListBox lb = new ListBox();
            lb.SetBinding(ListBox.ItemsSourceProperty, b);

            if (mc.numTimes != 1) return TestResult.Fail;

            return TestResult.Pass;
        }

        private TestResult SortDescriptionEmptyCollection()
        {
            Status("SortDescriptionEmptyCollection");
            WaitForPriority(DispatcherPriority.SystemIdle);

            // setup
            ListBox lb1 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb1"));
            _lb2 = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb2"));

            // add sort descriptions to a view over an empty collection
            ObservableCollection<string> oc = new ObservableCollection<string>();
            lb1.ItemsSource = oc;
            ListCollectionView lcv = (ListCollectionView)(CollectionViewSource.GetDefaultView(oc));
            lcv.SortDescriptions.Add(new SortDescription());

            XmlDocument doc = new XmlDocument();
            XmlElement elements = doc.CreateElement("Elements");
            doc.AppendChild(elements);
            XmlDataProvider xdp = new XmlDataProvider();
            
            xdp.DataChanged += new EventHandler(xdp_DataChanged);
            xdp.XPath = "Elements/Element";
            xdp.Document = doc;
            WaitForSignal("dataChanged");

            Binding b = new Binding();
            b.Source = xdp;
            _lb2.SetBinding(ListBox.ItemsSourceProperty, b);
            _lb2.Items.SortDescriptions.Add(new SortDescription(".", ListSortDirection.Ascending));

            // add items to that collection
            oc.Add("Z");
            oc.Add("A");
            oc.Add("T");

            XmlElement elementZ = doc.CreateElement("Element");
            XmlElement elementA = doc.CreateElement("Element");
            XmlElement elementT = doc.CreateElement("Element");
            elementZ.InnerText = "Z";
            elementA.InnerText = "A";
            elementT.InnerText = "T";
            elements.AppendChild(elementZ);
            elements.AppendChild(elementA);
            elements.AppendChild(elementT);

            // verify those items are sorted
            WaitForPriority(DispatcherPriority.SystemIdle);
            Util.AssertEquals(lb1.Items[0].ToString(), "A");
            Util.AssertEquals(lb1.Items[1].ToString(), "T");
            Util.AssertEquals(lb1.Items[2].ToString(), "Z");

            WaitForPriority(DispatcherPriority.SystemIdle);
            Util.AssertEquals(((XmlElement)(_lb2.Items[0])).InnerText, "A");
            Util.AssertEquals(((XmlElement)(_lb2.Items[1])).InnerText, "T");
            Util.AssertEquals(((XmlElement)(_lb2.Items[2])).InnerText, "Z");

            return TestResult.Pass;
        }

        private void xdp_DataChanged(object sender, EventArgs e)
        {
            Signal("dataChanged", TestResult.Pass);
        }

        private class CountingCollection : IEnumerable, INotifyCollectionChanged
        {
            private ArrayList _myList;

            public int numTimes = 0;

            public CountingCollection()
            {
                _myList = new ArrayList();
                _myList.Add("a");
                _myList.Add("b");
                _myList.Add("c");
                _myList.Add("d");
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    collectionChanged += value;
                    numTimes++;
                }
                remove
                {
                    collectionChanged -= value;
                }
            }
            private event NotifyCollectionChangedEventHandler collectionChanged;

            public IEnumerator GetEnumerator()
            {
                return _myList.GetEnumerator();
            }
        }

        public class MyCollectionViewSource : CollectionViewSource
        {
            public string Foo { get; set; }
        }

        #endregion
    }
}
