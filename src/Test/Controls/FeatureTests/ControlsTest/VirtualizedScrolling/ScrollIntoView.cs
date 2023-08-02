using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Test.Controls.ScrollIntoView
{
    /// <summary>
    /// <description>
    /// Test of ScrollIntoView, especially when nested within an outer ScrollViewer.
    /// </description>
    /// </summary>
    [Test(1, "VirtualizedScrolling", "ScrollIntoView", Versions = "4.8+")]
    public class ScrollIntoView : XamlTest
    {
        Model _model;
        CollectionViewSource _cvs;
        ScrollViewer _outerSV, _innerSV;
        ListBox _listbox;
        double _beforeHeight;
        ObservableCollection<Config> _configs;
        ObservableCollection<Tuple<int,int>> _scrollList;

        public ScrollIntoView()
            : base(@"ScrollIntoView.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        private TestResult Setup()
        {
            Status("Setup");

            _outerSV = (ScrollViewer)RootElement.FindName("_outerSV");
            Assert.AssertTrue("Failed to find the outer ScrollViewer. ", _outerSV != null);

            _listbox = (ListBox)RootElement.FindName("_listbox");
            Assert.AssertTrue("Failed to find the main ListBox. ", _listbox != null);

            _cvs = RootElement.Resources["CVS"] as CollectionViewSource;
            _innerSV = FindElement<ScrollViewer>(_listbox);
            _innerSV.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

            GeneralTransform innerToOuter = _innerSV.TransformToAncestor(_outerSV);
            Rect rect = innerToOuter.TransformBounds(new Rect(_innerSV.RenderSize));
            _beforeHeight = rect.Top;

            DataList.RandomSeed = 314159;
            _model = new Model();
            _model.Data = DataList.Create();

            RootElement.DataContext = _model;

            _configs = InitializeConfigs();
            _scrollList = InitializeScrollList();

            return TestResult.Pass;
        }

        ObservableCollection<Config> InitializeConfigs()
        {
            ObservableCollection<Config> list = new ObservableCollection<Config>();

            list.Add(new Config{ SVHeight=400, ScrollUnit=ScrollUnit.Item, CacheLength=100, GroupingLevels=0 });
            list.Add(new Config{ SVHeight=400, ScrollUnit=ScrollUnit.Pixel, CacheLength=100, GroupingLevels=2 });
            list.Add(new Config{ SVHeight=150, ScrollUnit=ScrollUnit.Pixel, CacheLength=100, GroupingLevels=0 });
            list.Add(new Config{ SVHeight=150, ScrollUnit=ScrollUnit.Item, CacheLength=100, GroupingLevels=2 });

            return list;
        }

        ObservableCollection<Tuple<int,int>> InitializeScrollList()
        {
            ObservableCollection<Tuple<int,int>> list = new ObservableCollection<Tuple<int,int>>();

            list.Add(new Tuple<int,int>(0,15));
            list.Add(new Tuple<int,int>(0,20));
            list.Add(new Tuple<int,int>(0,50));
            list.Add(new Tuple<int,int>(0,80));
            list.Add(new Tuple<int,int>(0,100));
            list.Add(new Tuple<int,int>(30,15));
            list.Add(new Tuple<int,int>(30,20));
            list.Add(new Tuple<int,int>(30,50));
            list.Add(new Tuple<int,int>(30,80));
            list.Add(new Tuple<int,int>(30,100));
            list.Add(new Tuple<int,int>(48,15));
            list.Add(new Tuple<int,int>(48,20));
            list.Add(new Tuple<int,int>(48,50));
            list.Add(new Tuple<int,int>(48,80));
            list.Add(new Tuple<int,int>(48,100));
            list.Add(new Tuple<int,int>(67,15));
            list.Add(new Tuple<int,int>(67,20));
            list.Add(new Tuple<int,int>(67,50));
            list.Add(new Tuple<int,int>(67,80));
            list.Add(new Tuple<int,int>(67,100));
            list.Add(new Tuple<int,int>(90,15));
            list.Add(new Tuple<int,int>(90,20));
            list.Add(new Tuple<int,int>(90,50));
            list.Add(new Tuple<int,int>(90,80));
            list.Add(new Tuple<int,int>(90,100));

            return list;
        }

        private TestResult RunTest()
        {
            Status("RunTest");

            _model.AlignViewport = true;
            _model.ScrollViewport = true;
            _model.ScrollType = ScrollType.Ratio;
            TestResult result = TestResult.Pass;

            foreach (Config config in _configs)
            {
                LogComment(String.Format("Using config: {0}", config));
                _model.ProposedConfig = config;
                _model.ApplyProposedConfig(_listbox, _cvs);

                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
                var items = _listbox.Items;

                foreach (VerticalAlignment svAlignment in Enum.GetValues(typeof(VerticalAlignment)))
                {
                    if (svAlignment == VerticalAlignment.Stretch)
                        continue;
                    _model.SVAlignment = svAlignment;

                    foreach (VerticalAlignment vpAlignment in Enum.GetValues(typeof(VerticalAlignment)))
                    {
                        if (vpAlignment == VerticalAlignment.Stretch)
                            continue;
                        _model.ViewportAlignment = vpAlignment;

                        foreach (Tuple<int,int> tuple in _scrollList)
                        {
                            _model.ScrollValue = tuple.Item1;
                            int index = Math.Min((int)(tuple.Item2 / 100.0 * items.Count), items.Count-1);

                            string error = TestScrollIntoView(items[index]);

                            if (error != null)
                            {
                                result = TestResult.Fail;
                                LogComment(String.Format("  SIV failed from {0} to {1}\n  {2}\n   Before: {3}\n   After:  {4}",
                                    tuple.Item1, index, error, _model.BeforeSnapshot, _model.AfterSnapshot));
                            }
                        }
                    }

                }
            }

            return result;
        }

        static T FindElement<T>(DependencyObject d) where T : DependencyObject
        {
            T result = d as T;
            for (int i=0, n=System.Windows.Media.VisualTreeHelper.GetChildrenCount(d); i<n && result==null; ++i)
            {
                result = FindElement<T>(System.Windows.Media.VisualTreeHelper.GetChild(d, i));
            }
            return result;
        }

        string TestScrollIntoView(object item)
        {
            // set up the test
            if (_model.AlignViewport)
            {
                double offset = _beforeHeight;

                switch (_model.ViewportAlignment)
                {
                    case VerticalAlignment.Center:  offset += _innerSV.ActualHeight / 2; break;
                    case VerticalAlignment.Bottom:  offset += _innerSV.ActualHeight; break;
                }
                switch (_model.SVAlignment)
                {
                    case VerticalAlignment.Center:  offset -= _outerSV.ActualHeight / 2; break;
                    case VerticalAlignment.Bottom:  offset -= _outerSV.ActualHeight; break;
                }

                _outerSV.ScrollToVerticalOffset(offset);
            }

            if (_model.ScrollViewport)
            {
                double offset = _model.ScrollValue;
                switch (_model.ScrollType)
                {
                    case ScrollType.Ratio:  offset = offset / _innerSV.ExtentHeight / 100.0; break;
                }
                _innerSV.ScrollToVerticalOffset(offset);
            }

            // take the "before" snapshot
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            _model.BeforeSnapshot = TakeSnapshot(item);

            // scroll the requested item into view
            _listbox.ScrollIntoView(item);

            // take the "after" snapshot
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            _model.AfterSnapshot = TakeSnapshot(item);

            // check whether the right things happened
            string error = VerifyScrollIntoView(_model.BeforeSnapshot, _model.AfterSnapshot);
            return error;
        }

        Snapshot TakeSnapshot(object item)
        {
            Snapshot snapshot = new Snapshot();
            ItemContainerGenerator icg = _listbox.ItemContainerGenerator;
            UIElement container = icg.ContainerFromItem(item) as UIElement;
            GeneralTransform containerToInner = (container != null) ? container.TransformToAncestor(_innerSV) : null;
            GeneralTransform innerToOuter = _innerSV.TransformToAncestor(_outerSV);
            Rect innerSVRect = new Rect(_innerSV.RenderSize);
            Rect outerSVRect = new Rect(_outerSV.RenderSize);
            Rect temp;

            if (containerToInner != null)
            {
                Rect itemRect = new Rect(container.RenderSize);
                temp = snapshot.ItemInVp = containerToInner.TransformBounds(itemRect);
                temp.Intersect(innerSVRect);
                snapshot.ItemVisInVp = temp;

                temp = snapshot.ItemInSV = innerToOuter.TransformBounds(snapshot.ItemVisInVp);
                temp.Intersect(outerSVRect);
                snapshot.ItemVisInSV = temp;
            }
            else
            {
                snapshot.ItemVisInVp = snapshot.ItemInVp = Rect.Empty;
                snapshot.ItemVisInSV = snapshot.ItemInSV = Rect.Empty;
            }

            temp = snapshot.VpInSV = innerToOuter.TransformBounds(innerSVRect);
            temp.Intersect(outerSVRect);
            snapshot.VpVisInSV = temp;

            snapshot.VpOffset = _innerSV.VerticalOffset;
            snapshot.VpExtent = _innerSV.ExtentHeight;
            snapshot.SvOffset = _outerSV.VerticalOffset;
            snapshot.SvExtent = _outerSV.ExtentHeight;

            snapshot.InnerToOuter = innerToOuter;

            return snapshot;
        }

        enum ScrollDirection { Forward, Backward, None}

        string VerifyScrollIntoView(Snapshot before, Snapshot after)
        {
            double delta;
            Size viewportSize = _innerSV.RenderSize;
            Size svSize = _outerSV.RenderSize;

            // verify position within inner viewport
            if (IsLessThan(after.ItemVisInVp.Height, after.ItemInVp.Height) &&
                IsLessThan(after.ItemVisInVp.Height, viewportSize.Height))
            {
                return String.Format("Item is not maximally visible within inner viewport.\n" +
                    "  Item rect: {0}  Visible rect: {1}  VP size: {2}",
                    after.ItemInVp, after.ItemVisInVp, viewportSize);
            }

            // verify alignment within viewport
            delta = after.VpOffset - before.VpOffset;
            ScrollDirection innerScrollDirection = (delta < 0) ? ScrollDirection.Backward :
                                                   (delta > 0) ? ScrollDirection.Forward :
                                                                 ScrollDirection.None;
            switch (innerScrollDirection)
            {
                case ScrollDirection.Forward:
                    // item-scrolling always aligns at the top, so the target item
                    // isn't necessarily aligned at the bottom.
                    if (_model.CurrentConfig.ScrollUnit == ScrollUnit.Pixel &&
                        !IsClose(after.ItemVisInVp.Bottom, viewportSize.Height))
                    {
                        return String.Format("After forward scroll, item not aligned at bottom of viewport.\n" +
                            "  Item rect: {0}  Visible rect: {1}  VP size: {2}",
                            after.ItemInVp, after.ItemVisInVp, viewportSize);
                    }
                    break;

                case ScrollDirection.Backward:
                    if (!IsClose(after.ItemVisInVp.Top, 0))
                    {
                        return String.Format("After backward scroll, item not aligned at top of viewport.\n" +
                            "  Item rect: {0}  Visible rect: {1}  VP size: {2}",
                            after.ItemInVp, after.ItemVisInVp, viewportSize);
                    }
                    break;

                case ScrollDirection.None:
                    break;
            }

            // verify position within outer ScrollViewer
            if (IsLessThan(after.ItemVisInSV.Height, after.ItemInSV.Height) &&
                IsLessThan(after.ItemVisInSV.Height, svSize.Height))
            {
                return String.Format("Item is not maximally visible within outer ScrollViewer.\n" +
                    "  Item rect: {0}  Visible rect: {1}  SV size: {2}",
                    after.ItemInSV, after.ItemVisInSV, svSize);
            }

            // verify alignment within outer ScrollViewer
            bool verifyScrollNecessary = true;
            delta = after.SvOffset - before.SvOffset;
            ScrollDirection outerScrollDirection = (delta < 0) ? ScrollDirection.Backward :
                                                   (delta > 0) ? ScrollDirection.Forward :
                                                                 ScrollDirection.None;
            switch (outerScrollDirection)
            {
                case ScrollDirection.Forward:
                    switch (_model.CurrentConfig.ScrollUnit)
                    {
                        case ScrollUnit.Pixel:
                            if (!IsClose(after.ItemVisInSV.Bottom, svSize.Height))
                            {
                                return String.Format("After forward scroll, item not aligned at bottom of outer ScrollViewer.\n" +
                                    "  Item rect: {0}  Visible rect: {1}  SV size: {2}",
                                    after.ItemInSV, after.ItemVisInSV, svSize);
                            }
                            break;
                        case ScrollUnit.Item:
                            if (innerScrollDirection == ScrollDirection.Forward)
                            {
                                // Item-scrolling always aligns at the top, so the target item's
                                // final position isn't necessarily at the predicted position
                                // (aligned to the bottom of the innerSV).  The outerSV
                                // may scroll to show the predicted position before scrolling
                                // again to show the final position.  So we have to skip both
                                // the alignment check and the "is scroll necessary" check.
                                verifyScrollNecessary = false;
                            }
                            else goto case ScrollUnit.Pixel;
                            break;
                    }
                    break;

                case ScrollDirection.Backward:
                    if (!IsClose(after.ItemVisInSV.Top, 0))
                    {
                        return String.Format("After backward scroll, item not aligned at top of outer ScrollViewer.\n" +
                            "  Item rect: {0}  Visible rect: {1}  SV size: {2}",
                            after.ItemInSV, after.ItemVisInSV, svSize);
                    }
                    break;

                case ScrollDirection.None:
                    verifyScrollNecessary = false;
                    break;
            }

            // verify that outer ScrollViewer didn't scroll
            // past the inner one
            switch (outerScrollDirection)
            {
                case ScrollDirection.Forward:
                    if (IsLessThan(after.VpInSV.Bottom, svSize.Height))
                    {
                        return String.Format("Outer ScrollViewer scrolled forward past the bottom of viewport.\n" +
                            "  Viewport rect: {0}  SV size: {1}",
                            after.VpInSV, svSize);
                    }
                    break;
                case ScrollDirection.Backward:
                    if (IsLessThan(0, after.VpInSV.Top))
                    {
                        return String.Format("Outer ScrollViewer scrolled backward past the top of viewport.\n" +
                            "  Viewport rect: {0}  SV size: {1}",
                            after.VpInSV, svSize);
                    }
                    break;
           }

            // verify that outer ScrollViewer only scrolled if it had to
            if (verifyScrollNecessary)
            {
                Rect rect = before.InnerToOuter.TransformBounds(after.ItemVisInVp);
                if (IsLessOrClose(0, rect.Top) && IsLessOrClose(rect.Bottom, svSize.Height))
                {
                    return String.Format("Outer ScrollViewer scrolled unnecessarily.\n" +
                        "  Item rect: {0}  SV size: {1}",
                        rect, svSize);
                }
            }

            return null;
        }

        const double Tolerance = 2.0;   // ignore FP drift, contributions from borders and padding, etc.

        static bool IsClose(double x, double y)
        {
            return Math.Abs(x-y) < Tolerance;
        }

        static bool IsLessThan(double x, double y)
        {
            return (x<y) && !IsClose(x,y);
        }

        static bool IsLessOrClose(double x, double y)
        {
            return (x<y) || IsClose(x,y);
        }
    }

    public class INPCBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }

    public enum ScrollType { Offset, Ratio }

    public class Model : INPCBase
    {
        DataList _data;
        public DataList Data
        {
            get { return _data; }
            set { _data = value; OnPropertyChanged(nameof(Data)); }
        }


        Config _currentConfig = new Config();
        public Config CurrentConfig
        {
            get { return _currentConfig; }
            set { _currentConfig = value; OnPropertyChanged(nameof(CurrentConfig)); }
        }

        Config _proposedConfig = new Config();
        public Config ProposedConfig
        {
            get { return _proposedConfig; }
            set { _proposedConfig = value; OnPropertyChanged(nameof(ProposedConfig)); }
        }

        public void ApplyProposedConfig(ItemsControl ic, CollectionViewSource cvs)
        {
            using (cvs.DeferRefresh())
            {
                // apply grouping directly (can't be done via databinding)
                if (ProposedConfig.GroupingLevels != CurrentConfig.GroupingLevels)
                {
                    PropertyGroupDescription pgd;

                    cvs.GroupDescriptions.Clear();
                    ic.GroupStyle.Clear();

                    if (ProposedConfig.GroupingLevels >= 1)
                    {
                        pgd = new PropertyGroupDescription(nameof(DataItem.Group1));
                        pgd.SortDescriptions.Add(new SortDescription(nameof(CollectionViewGroup.Name), ListSortDirection.Ascending));
                        cvs.GroupDescriptions.Add(pgd);
                        ic.GroupStyle.Add(GroupStyle.Default);
                    }

                    if (ProposedConfig.GroupingLevels >= 2)
                    {
                        pgd = new PropertyGroupDescription(nameof(DataItem.Group2));
                        pgd.SortDescriptions.Add(new SortDescription(nameof(CollectionViewGroup.Name), ListSortDirection.Ascending));
                        cvs.GroupDescriptions.Add(pgd);
                        ic.GroupStyle.Add(GroupStyle.Default);
                    }
                }

                // swap in the new config - databinding does the rest
                CurrentConfig = ProposedConfig;
                ProposedConfig = new Config(ProposedConfig);
            }
        }


        bool _alignViewport;
        public bool AlignViewport
        {
            get { return _alignViewport; }
            set { _alignViewport = value; OnPropertyChanged(nameof(AlignViewport)); }
        }

        VerticalAlignment _viewportAlignment;
        public VerticalAlignment ViewportAlignment
        {
            get { return _viewportAlignment; }
            set { _viewportAlignment = value; OnPropertyChanged(nameof(ViewportAlignment)); }
        }

        VerticalAlignment _svAlignment;
        public VerticalAlignment SVAlignment
        {
            get { return _svAlignment; }
            set { _svAlignment = value; OnPropertyChanged(nameof(SVAlignment)); }
        }

        bool _scrollViewport;
        public bool ScrollViewport
        {
            get { return _scrollViewport; }
            set { _scrollViewport = value; OnPropertyChanged(nameof(ScrollViewport)); }
        }

        ScrollType _scrollType;
        public ScrollType ScrollType
        {
            get { return _scrollType; }
            set { _scrollType = value; OnPropertyChanged(nameof(ScrollType)); }
        }

        double _scrollValue;
        public double ScrollValue
        {
            get { return _scrollValue; }
            set { _scrollValue = value; OnPropertyChanged(nameof(ScrollValue)); }
        }


        Snapshot _beforeSnapshot;
        public Snapshot BeforeSnapshot
        {
            get { return _beforeSnapshot; }
            set { _beforeSnapshot = value; OnPropertyChanged(nameof(BeforeSnapshot)); }
        }

        Snapshot _afterSnapshot;
        public Snapshot AfterSnapshot
        {
            get { return _afterSnapshot; }
            set { _afterSnapshot = value; OnPropertyChanged(nameof(AfterSnapshot)); }
        }

        public string[] ScrollUnitValues
        {
            get { return Enum.GetNames(typeof(ScrollUnit)); }
        }

        public string[] VerticalAlignmentValues
        {
            get { return Enum.GetNames(typeof(VerticalAlignment)); }
        }

        public string[] ScrollTypeValues
        {
            get { return Enum.GetNames(typeof(ScrollType)); }
        }
    }

    public class DataItem : INPCBase
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value;  OnPropertyChanged(nameof(Name)); }
        }

        int _displayHeight;
        public int DisplayHeight
        {
            get { return _displayHeight; }
            set { _displayHeight = value; OnPropertyChanged(nameof(DisplayHeight)); }
        }

        char _group1;
        public char Group1
        {
            get { return _group1; }
            set { _group1 = value; OnPropertyChanged(nameof(Group1)); }
        }

        int _group2;
        public int Group2
        {
            get { return _group2; }
            set { _group2 = value; OnPropertyChanged(nameof(Group2)); }
        }
    }

    public class DataList : ObservableCollection<DataItem>
    {
        public static int RandomSeed { get; set; }

        public static DataList Create(int n=100)
        {
            DataList list = new DataList();
            Random rng = new Random(RandomSeed);

            // create n items with random heights and groups
            for (int i=0; i<n; ++i)
            {
                DataItem item = new DataItem
                {
                    Name = "Item " + i,
                    DisplayHeight = 20 + rng.Next(10),
                    Group1 = (char)((int)'A' + rng.Next(4)),
                    Group2 = rng.Next(3)
                };

                list.Add(item);
            }

            // make some of the heights exceptionally large, even larger than the
            // listbox height (200)
            list[(int)(0.2 * n)].DisplayHeight = 300;
            list[(int)(0.8 * n)].DisplayHeight = 300;
            list[(int)(0.4 * n)].DisplayHeight = 200;
            list[(int)(0.6 * n)].DisplayHeight = 200;
            list[(int)(0.3 * n)].DisplayHeight = 100;
            list[(int)(0.7 * n)].DisplayHeight = 100;

            return list;
        }
    }

    public class Config : INPCBase
    {
        public Config()
        {
            SVHeight = 400;
            ScrollUnit = ScrollUnit.Item;
            CacheLength = 100;
            GroupingLevels = 0;
        }

        public Config(Config config)
        {
            SVHeight = config.SVHeight;
            ScrollUnit = config.ScrollUnit;
            CacheLength = config.CacheLength;
            GroupingLevels = config.GroupingLevels;
        }

        double _svHeight;
        public double SVHeight
        {
            get { return _svHeight; }
            set { _svHeight = value; OnPropertyChanged(nameof(SVHeight)); }
        }

        ScrollUnit _scrollUnit;
        public ScrollUnit ScrollUnit
        {
            get { return _scrollUnit; }
            set { _scrollUnit = value; OnPropertyChanged(nameof(ScrollUnit)); }
        }

        double _cacheLength;
        public double CacheLength
        {
            get { return _cacheLength; }
            set { _cacheLength = value; OnPropertyChanged(nameof(CacheLength)); }
        }

        int _groupingLevels;
        public int GroupingLevels
        {
            get { return _groupingLevels; }
            set { _groupingLevels = value; OnPropertyChanged(nameof(GroupingLevels)); }
        }

        public override string ToString()
        {
            return String.Format("SVh:{0} SU:{1} CL:{2} GL:{3}",
                SVHeight, ScrollUnit, CacheLength, GroupingLevels);
        }
    }

    public class Snapshot : INPCBase
    {
        Rect _itemInVp;
        public Rect ItemInVp
        {
            get { return _itemInVp; }
            set { _itemInVp = value;  OnPropertyChanged(nameof(ItemInVp)); }
        }

        Rect _itemVisInVp;
        public Rect ItemVisInVp
        {
            get { return _itemVisInVp; }
            set { _itemVisInVp = value; OnPropertyChanged(nameof(ItemVisInVp)); }
        }

        Rect _itemInSV;
        public Rect ItemInSV
        {
            get { return _itemInSV; }
            set { _itemInSV = value; OnPropertyChanged(nameof(ItemInSV)); }
        }

        Rect _itemVisInSV;
        public Rect ItemVisInSV
        {
            get { return _itemVisInSV; }
            set { _itemVisInSV = value; OnPropertyChanged(nameof(ItemVisInSV)); }
        }

        Rect _vpInSV;
        public Rect VpInSV
        {
            get { return _vpInSV; }
            set { _vpInSV = value; OnPropertyChanged(nameof(VpInSV)); }
        }

        Rect _vpVisInSV;
        public Rect VpVisInSV
        {
            get { return _vpVisInSV; }
            set { _vpVisInSV = value; OnPropertyChanged(nameof(VpVisInSV)); }
        }

        double _vpOffset;
        public double VpOffset
        {
            get { return _vpOffset; }
            set { _vpOffset = value; OnPropertyChanged(nameof(VpOffset)); }
        }

        double _vpExtent;
        public double VpExtent
        {
            get { return _vpExtent; }
            set { _vpExtent = value; OnPropertyChanged(nameof(VpExtent)); }
        }

        double _svOffset;
        public double SvOffset
        {
            get { return _svOffset; }
            set { _svOffset = value; OnPropertyChanged(nameof(SvOffset)); }
        }

        double _svExtent;
        public double SvExtent
        {
            get { return _svExtent; }
            set { _svExtent = value; OnPropertyChanged(nameof(SvExtent)); }
        }

        GeneralTransform _innerToOuter;
        public GeneralTransform InnerToOuter
        {
            get { return _innerToOuter; }
            set { _innerToOuter = value; OnPropertyChanged(nameof(InnerToOuter)); }
        }

        public override string ToString()
        {
            return String.Format("ItemInVP:{0} vis:{1} ItemInSV:{2} vis:{3} VpInSV:{4} vis:{5} VpO/E:{6:f2}/{7:f2} SvO/E:{8:f2}/{9:f2}",
                ItemInVp, ItemVisInVp, ItemInSV, ItemVisInSV, VpInSV, VpVisInSV,
                VpOffset, VpExtent, SvOffset, SvExtent);
        }
    }
}


