using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Actions;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// SelectionForCustomINCC
    /// </description>
    /// </summary>
    [Test(1, "Selection", "SelectionForCustomINCC", Versions = "4.7.1+")]
    public class SelectionForCustomINCC : XamlTest
    {
        #region Private Members
        ListBox listbox;
        HubList hublist = new HubList();
        #endregion

        #region Public Members

        public SelectionForCustomINCC()
            : base(@"ListBoxSelectionTest.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestListBoxSelection);
        }

        public TestResult Setup()
        {
            Status("Setup");

            for (int i = 0; i < 5; ++i)
            {
                hublist.Add(new Hub { Name = "hub " + i });
            }

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            listbox = (ListBox)RootElement.FindName("listbox");
            if (listbox == null)
            {
                throw new TestValidationException("ListBox is null");
            }

            listbox.ItemsSource = hublist;

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            listbox = null;
            return TestResult.Pass;
        }

        public TestResult TestListBoxSelection()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // remove selected item
            listbox.SelectedIndex = 0;
            hublist.RemoveAt(0);

            return TestResult.Pass;
        }

        #endregion
    }


    public class Hub
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override string ToString()
        {
            return _name;
        }
    }

    // custom implementation of INCC, doesn't supply position information
    // This causes problems for Selector
    public class HubList : List<Hub>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var eh = CollectionChanged;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        public new void Add(Hub hub)
        {
            base.Add(hub);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, hub));
        }

        public new void Remove(Hub hub)
        {
            base.Remove(hub);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, hub));
        }

        public new void RemoveAt(int index)
        {
            Hub hub = this[index];
            base.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, hub));
        }
    }
}

