using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Test.Controls.ItemLeak
{
    public class Model : INotifyPropertyChanged
    {
        static int _seq;
        static MethodInfo _miCleanup;

        public static int NextSeq()
        {
            return _seq++;
        }

        public Model(int n1, int n2, int n3, int n4)
        {
            _miCleanup =
                typeof(System.Windows.Data.BindingOperations).GetMethod("Cleanup",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            TrackingList = new TrackingList();

            CreateWideItems(n1);
            CreateSimpleItems(n2);
            CreateNarrowItems(n3);
            CreateHierarchicalItems(n4);
        }

        #region TrackingList

        public TrackingList TrackingList { get; private set; }

        void Track(object item)
        {
            TrackingList.Add(new TrackingItem(item));
            RecursiveTrack(item as HierarchicalItem);
        }

        void RecursiveTrack(HierarchicalItem item)
        {
            if (item != null)
            {
                foreach (HierarchicalItem subitem in item.SubItems)
                {
                    Track(subitem);
                }
            }
        }

        public void DoGC()
        {
            GetMemory();
            TrackingList.Purge();
        }

        public static long GetMemory()
        {
            long result = GC.GetTotalMemory(true);
            GC.WaitForPendingFinalizers();
            // while (BindingOperations.Cleanup())
            while ((bool)_miCleanup.Invoke(null, null))
            {
                result = GC.GetTotalMemory(true);
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        #endregion TrackingList

        #region Event Handlers

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_ignoreChanges)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    Track(e.OldItems[0]);
                    break;
            }
        }

        void OnPreviewClear(object sender, EventArgs e)
        {
            IList list = sender as IList;
            if (list != null)
            {
                foreach (object item in list)
                {
                    Track(item);
                }
            }
        }

        #endregion Event Handlers

        #region WideItems

        public WideItemList WideItems { get; private set; }

        void CreateWideItems(int n)
        {
            WideItems = new WideItemList();
            for (int i=0; i<n; ++i)
            {
                WideItems.Add(new WideItem());
            }

            WideItems.CollectionChanged += OnCollectionChanged;
            WideItems.PreviewClear += OnPreviewClear;
        }

        public void AddWideItem()
        {
            WideItems.Add(new WideItem());
        }

        public void RemoveWideItem(int index)
        {
            if (0 <= index && index < WideItems.Count)
            {
                WideItems.RemoveAt(index);
            }
        }

        public void ClearWideItems()
        {
            WideItems.Clear();
        }

        #endregion WideItems

        #region SimpleItems

        public SimpleItemList SimpleItems { get; private set; }

        void CreateSimpleItems(int n)
        {
            SimpleItems = new SimpleItemList();
            for (int i = 0; i < n; ++i)
            {
                SimpleItems.Add(new SimpleItem());
            }

            SimpleItems.CollectionChanged += OnCollectionChanged;
            SimpleItems.PreviewClear += OnPreviewClear;
        }

        public void AddSimpleItem()
        {
            SimpleItems.Add(new SimpleItem());
        }

        public void RemoveSimpleItem(int index)
        {
            if (0 <= index && index < SimpleItems.Count)
            {
                SimpleItems.RemoveAt(index);
            }
        }

        public void ClearSimpleItems()
        {
            SimpleItems.Clear();
        }

        #endregion SimpleItems

        #region NarrowItems

        public NarrowItemList NarrowItems { get; private set; }

        void CreateNarrowItems(int n)
        {
            NarrowItems = new NarrowItemList();
            for (int i = 0; i < n; ++i)
            {
                NarrowItems.Add(new NarrowItem());
            }

            NarrowItems.CollectionChanged += OnCollectionChanged;
            NarrowItems.PreviewClear += OnPreviewClear;
        }

        public void AddNarrowItem()
        {
            NarrowItems.Add(new NarrowItem());
        }

        public void RemoveNarrowItem(int index)
        {
            if (0 <= index && index < NarrowItems.Count)
            {
                NarrowItems.RemoveAt(index);
            }
        }

        public void ClearNarrowItems()
        {
            NarrowItems.Clear();
        }

        #endregion NarrowItems

        #region HierarchicalItems

        public HierarchicalItemList HierarchicalItems { get; private set; }

        void CreateHierarchicalItems(int n)
        {
            HierarchicalItems = new HierarchicalItemList();
            for (int i = 0; i < n; ++i)
            {
                HierarchicalItem item = AddHierarchicalItem(null);
                for (int j=0; j<3; ++j)
                {
                    AddHierarchicalItem(item);
                }
            }
            HierarchicalItems.CollectionChanged += OnCollectionChanged;
            HierarchicalItems.PreviewClear += OnPreviewClear;
        }

        public HierarchicalItem AddHierarchicalItem(HierarchicalItem parent)
        {
            HierarchicalItem subitem;
            if (parent == null)
            {
                subitem = HierarchicalItems.AddNewItem(null);
            }
            else
            {
                subitem = parent.SubItems.AddNewItem(parent);
            }
            subitem.SubItems.CollectionChanged += OnCollectionChanged;
            subitem.SubItems.PreviewClear += OnPreviewClear;
            return subitem;
        }

        public void RemoveHierarchicalItem(HierarchicalItem item)
        {
            HierarchicalItems.RecursiveRemove(item);
        }

        public void ClearHierarchicalItems(HierarchicalItem parent)
        {
            if (parent == null)
            {
                HierarchicalItems.Clear();
            }
            else
            {
                parent.SubItems.Clear();
            }
        }

        public void UnselectHierarchicalItem(HierarchicalItem item)
        {
            using (new Ignorer(this))
            {
                HierarchicalItems.RecursiveUnselect(item);
            }
        }

        #endregion HierarchicalItems

        #region Miscellaneous

        bool _workaroundDataGridLeak = true;
        public bool WorkaroundDataGridLeak
        {
            get { return _workaroundDataGridLeak; }
            set { _workaroundDataGridLeak = value; OnPropertyChanged(nameof(WorkaroundDataGridLeak)); }
        }

        bool _ignoreChanges;

        class Ignorer : IDisposable
        {
            Model _model;

            public Ignorer(Model model)
            {
                _model = model;
                _model._ignoreChanges = true;
            }

            public void Dispose()
            {
                _model._ignoreChanges = false;
            }
        }

        #endregion Miscellaneous

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class TrackingItem
    {
        public TrackingItem(object item)
        {
            DisplayName = item.ToString();
            TrackedItem = new WeakReference(item);
        }

        public string DisplayName { get; private set; }
        WeakReference TrackedItem { get; set; }

        public bool IsGCd { get { return !TrackedItem.IsAlive; } }

        public bool Tracks(object item) { return Object.Equals(item, TrackedItem.Target); }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class TrackingList : ObservableCollection<TrackingItem>
    {
        public void Purge()
        {
            if (Count <= 0)
                return;

            List<TrackingItem> toRemove = new List<TrackingItem>();
            foreach (TrackingItem ti in this)
            {
                if (ti.IsGCd)
                {
                    toRemove.Add(ti);
                }
            }

            foreach (TrackingItem ti in toRemove)
            {
                this.Remove(ti);
            }
        }

        public bool IsTracked(object item)
        {
            foreach (TrackingItem ti in this)
            {
                if (ti.Tracks(item))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class WideItem
    {
        static int s_seq;

        public WideItem()
        {
            Id = Model.NextSeq();
            Name = "WI " + s_seq++;
            Link = new Uri("http://www.foo.com");
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public bool IsValid { get; set; }
        public Uri Link { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, Id);
        }
    }

    public class WideItemList : ObservableCollection<WideItem>
    {
        public event EventHandler PreviewClear;

        void OnPreviewClear()
        {
            EventHandler handler = PreviewClear;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void ClearItems()
        {
            OnPreviewClear();
            base.ClearItems();
        }
    }

    public class SimpleItem
    {
        static int s_seq;

        public SimpleItem()
        {
            Id = Model.NextSeq();
            Name = "SI " + s_seq++;
        }

        public int Id { get; private set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, Id);
        }
    }

    public class SimpleItemList : ObservableCollection<SimpleItem>
    {
        public event EventHandler PreviewClear;

        void OnPreviewClear()
        {
            EventHandler handler = PreviewClear;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void ClearItems()
        {
            OnPreviewClear();
            base.ClearItems();
        }
    }

    public class NarrowItem
    {
        static int s_seq;

        public NarrowItem()
        {
            Id = Model.NextSeq();
            Name = "NI " + s_seq++;
        }

        public int Id { get; private set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, Id);
        }
    }

    public class NarrowItemList : ObservableCollection<NarrowItem>
    {
        public event EventHandler PreviewClear;

        void OnPreviewClear()
        {
            EventHandler handler = PreviewClear;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void ClearItems()
        {
            OnPreviewClear();
            base.ClearItems();
        }
    }

    public class HierarchicalItem
    {
        HierarchicalItemList _subitems = new HierarchicalItemList();
        public static readonly HierarchicalItem SentinelItem = new HierarchicalItem();

        private HierarchicalItem() { }

        public HierarchicalItem(HierarchicalItem parent, ref int seq)
        {
            Id = Model.NextSeq();
            if (parent == null)
            {
                Name = "HI " + seq++;
            }
            else
            {
                Name = parent.Name + "." + seq++;
            }
        }

        public int Id { get; private set; }
        public string Name { get; set; }
        public HierarchicalItemList SubItems { get { return _subitems; } }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, Id);
        }
    }

    public class HierarchicalItemList : ObservableCollection<HierarchicalItem>
    {
        int _seq;
        public event EventHandler PreviewClear;

        public HierarchicalItem AddNewItem(HierarchicalItem parent)
        {
            HierarchicalItem item = new HierarchicalItem(parent, ref _seq);
            Add(item);
            return item;
        }

        public bool RecursiveRemove(HierarchicalItem target)
        {
            foreach (HierarchicalItem item in this)
            {
                if (item == target)
                {
                    Remove(item);
                    return true;
                }
                if (item.SubItems.RecursiveRemove(target))
                {
                    return true;
                }
            }
            return false;
        }

        public bool RecursiveUnselect(HierarchicalItem target)
        {
            for (int i=0, n=this.Count; i<n; ++i)
            {
                HierarchicalItem item = this[i];
                if (item == target)
                {
                    // TreeView unselects when the selected item is replaced
                    this[i] = HierarchicalItem.SentinelItem;
                    // put the original item back right away
                    this[i] = target;
                    return true;
                }
                if (item.SubItems.RecursiveUnselect(target))
                {
                    return true;
                }
            }
            return false;
        }

        void OnPreviewClear()
        {
            EventHandler handler = PreviewClear;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void ClearItems()
        {
            OnPreviewClear();
            base.ClearItems();
        }
    }
}

