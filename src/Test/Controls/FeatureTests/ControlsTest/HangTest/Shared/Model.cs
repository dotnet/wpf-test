using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace HangTest
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Model : PropertyChangedBase
    {
        #region Properties visible to UI

        bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            { 
                if (value != _isRunning)
                {
                    if (!value)
                    {
                        _lastParameters = Parameters;
                    }

                    _isRunning = value;
                    OnPropertyChanged(nameof(IsRunning));
                    RaiseIsRunningChanged();
                }
            }
        }

        string _timeLimit;
        public String TimeLimit
        {
            get { return _timeLimit; }
            set { _timeLimit = value; OnPropertyChanged(nameof(TimeLimit)); }
        }

        bool _runRandomTest;
        public bool RunRandomTest
        {
            get { return _runRandomTest; }
            set { _runRandomTest = value; OnPropertyChanged(nameof(RunRandomTest)); }
        }

        bool _isStress;
        public bool IsStress
        {
            get { return _isStress; }
            set { _isStress = value; OnPropertyChanged(nameof(IsStress)); }
        }

        public string[] ScrollUnitValues
        {
            get { return Enum.GetNames(typeof(ScrollUnit)); }
        }

        ScrollUnit _scrollUnit;
        public ScrollUnit ScrollUnit
        {
            get { return _scrollUnit; }
            set { _scrollUnit = value; OnPropertyChanged(nameof(ScrollUnit)); }
        }

        public string[] VirtualizationModeValues
        {
            get { return Enum.GetNames(typeof(VirtualizationMode)); }
        }

        VirtualizationMode _virtualizationMode;
        public VirtualizationMode VirtualizationMode
        {
            get { return _virtualizationMode; }
            set { _virtualizationMode = value; OnPropertyChanged(nameof(VirtualizationMode)); }
        }

        public string[] CacheUnitValues
        {
            get { return Enum.GetNames(typeof(VirtualizationCacheLengthUnit)); }
        }

        VirtualizationCacheLengthUnit _cacheUnit;
        public VirtualizationCacheLengthUnit CacheUnit
        {
            get { return _cacheUnit; }
            set { _cacheUnit = value; OnPropertyChanged(nameof(CacheUnit)); }
        }

        string _cacheLength;
        public string CacheLength
        {
            get { return _cacheLength; }
            set { _cacheLength = value; OnPropertyChanged(nameof(CacheLength)); }
        }

        bool _useLayoutRounding;
        public bool UseLayoutRounding
        {
            get { return _useLayoutRounding; }
            set { _useLayoutRounding = value; OnPropertyChanged(nameof(UseLayoutRounding)); }
        }

        public string[] FlowDirectionValues
        {
            get { return Enum.GetNames(typeof(FlowDirection)); }
        }

        FlowDirection _flowDirection;
        [Random("W(9,1)")]
        public FlowDirection FlowDirection
        {
            get { return _flowDirection; }
            set { _flowDirection = value; OnPropertyChanged(nameof(FlowDirection)); }
        }

        double _treeViewHeight;
        [Random("BN(0.5, 0,0, 700,100)")]
        public double TreeViewHeight
        {
            get { return _treeViewHeight; }
            set { _treeViewHeight = value; OnPropertyChanged(nameof(TreeViewHeight)); }
        }

        Thickness _viewportMargin;
        [Random("BN(0.5, 0,0, 10,5)")]
        public Thickness ViewportMargin
        {
            get { return _viewportMargin; }
            set { _viewportMargin = value; OnPropertyChanged(nameof(ViewportMargin)); }
        }

        public string[] HeaderPositionValues
        {
            get { return Enum.GetNames(typeof(HeaderPosition)); }
        }

        HeaderPosition _headerPosition;
        [Random("C(0)")]    // replace when HeaderPosition layout is supported
        public HeaderPosition HeaderPosition
        {
            get { return _headerPosition; }
            set { _headerPosition = value; OnPropertyChanged(nameof(HeaderPosition)); }
        }

        bool _bindItemHeight;
        public bool BindItemHeight
        {
            get { return _bindItemHeight; }
            set { _bindItemHeight = value; OnPropertyChanged(nameof(BindItemHeight)); }
        }

        bool _bindItemMargin;
        public bool BindItemMargin
        {
            get { return _bindItemMargin; }
            set { _bindItemMargin = value; OnPropertyChanged(nameof(BindItemMargin)); }
        }

        int _dataSeed;
        public int DataSeed
        {
            get { return _dataSeed; }
            set { _dataSeed = value; OnPropertyChanged(nameof(DataSeed)); }
        }

        DataGenerationMethod _dataGenerationMethod;
        [Random("W(1,1,0)")]    // don't use File in random tests
        public DataGenerationMethod DataGenerationMethod
        {
            get { return _dataGenerationMethod; }
            set { _dataGenerationMethod = value; OnPropertyChanged(nameof(DataGenerationMethod)); }
        }

        string _filename;
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; OnPropertyChanged(nameof(Filename)); }
        }

        string _filterAttribute;
        public string FilterAttribute
        {
            get { return _filterAttribute; }
            set { _filterAttribute = value; OnPropertyChanged(nameof(FilterAttribute)); }
        }

        string _childrenTag;
        public string ChildrenTag
        {
            get { return _childrenTag; }
            set { _childrenTag = value; OnPropertyChanged(nameof(ChildrenTag)); }
        }

        DistributionsByLevel _distributionsByLevel = new DistributionsByLevel();
        public DistributionsByLevel DistributionsByLevel
        {
            get { return _distributionsByLevel; }
        }

        string _sizeDistribution;
        public string SizeDistribution
        {
            get { return _sizeDistribution; }
            set { _sizeDistribution = value; OnPropertyChanged(nameof(SizeDistribution)); }
        }

        string _partitionDistribution;
        public string PartitionDistribution
        {
            get { return _partitionDistribution; }
            set { _partitionDistribution = value; OnPropertyChanged(nameof(PartitionDistribution)); }
        }

        string _heightDistribution;
        public string HeightDistribution
        {
            get { return _heightDistribution; }
            set { _heightDistribution = value; OnPropertyChanged(nameof(HeightDistribution)); }
        }

        string _marginDistribution;
        public string MarginDistribution
        {
            get { return _marginDistribution; }
            set { _marginDistribution = value; OnPropertyChanged(nameof(MarginDistribution)); }
        }

        bool _verifyLayout;
        [Random("C(1)")]    // always true, unless explicitly overridden
        public bool VerifyLayout
        {
            get { return _verifyLayout; }
            set { _verifyLayout = value; OnPropertyChanged(nameof(VerifyLayout)); }
        }

        bool _verifyScrolling;
        [Random("C(1)")]    // always true, unless explicitly overridden
        public bool VerifyScrolling
        {
            get { return _verifyScrolling; }
            set { _verifyScrolling = value; OnPropertyChanged(nameof(VerifyScrolling)); }
        }

        bool _verifyProgress = true;
        [Random("C(1)")]    // always true, unless explicitly overridden
        public bool VerifyProgress
        {
            get { return _verifyProgress; }
            set { _verifyProgress = value; OnPropertyChanged(nameof(VerifyProgress)); }
        }

        int _actionSeed;
        public int ActionSeed
        {
            get { return _actionSeed; }
            set { _actionSeed = value; OnPropertyChanged(nameof(ActionSeed)); }
        }

        public string[] ScrollActionValues
        {
            get { return Enum.GetNames(typeof(ScrollAction)); }
        }

        ScrollAction _scrollAction;
        public ScrollAction ScrollAction
        {
            get { return _scrollAction; }
            set { _scrollAction = value; OnPropertyChanged(nameof(ScrollAction)); }
        }

        string[] _scrollPriorityValues = { "ApplicationIdle", "ContextIdle", "Background", "Input" };
        public string[] ScrollPriorityValues { get { return _scrollPriorityValues; } }

        DispatcherPriority _scrollPriority = DispatcherPriority.ContextIdle;
        [Random("U(2,6)")]      // uniform among the ScrollPriorityValues
        public DispatcherPriority ScrollPriority
        {
            get { return _scrollPriority; }
            set { _scrollPriority = value; OnPropertyChanged(nameof(ScrollPriority)); }
        }

        string _parameters;
        public string Parameters
        { 
            get { return _parameters; }
            set { _parameters = value; OnPropertyChanged(nameof(Parameters)); }
        }


        DataRoot _dataRoot;
        public DataRoot DataRoot
        {
            get { return _dataRoot; }
            private set { _dataRoot = value; OnPropertyChanged(nameof(DataRoot)); OnPropertyChanged(nameof(Data)); }
        }

        public DataCollection Data
        {
            get { return DataRoot.Children; }
        }

        RunParameters _runParameters;
        public RunParameters RunParameters
        {
            get { return _runParameters; }
            private set { _runParameters = value; OnPropertyChanged(nameof(RunParameters)); }
        }

        RealizedItem _bestItem;
        public RealizedItem BestItem
        {
            get { return _bestItem; }
            set { _bestItem = value; OnPropertyChanged(nameof(LastPosition)); }
        }

        public string LastPosition
        {
            get { return (_bestItem != null) ? _bestItem.Path : String.Empty; }
        }

        int _lastStepCount;
        public int LastStepCount
        {
            get { return _lastStepCount; }
            set { _lastStepCount = value; OnPropertyChanged(nameof(LastStepCount)); }
        }

        #endregion Properties visible to UI

        #region Data

        public void PrepareData()
        {
            HangTest.Data.Reset(RunParameters.HeightDistribution, RunParameters.MarginDistribution);

            switch (RunParameters.DataGenerationMethod)
            {
                case DataGenerationMethod.File:
                    DataRoot = DataRoot.ReadDataFromFile(RunParameters.Filename, RunParameters.FilterAttribute, RunParameters.ChildrenTag);
                    break;
                case DataGenerationMethod.FixedHeight:
                    DataRoot = DataRoot.CreateFixedHeightData(RunParameters.DistributionsByLevel);
                    break;
                case DataGenerationMethod.NonUniform:
                    DataRoot = DataRoot.CreateNonUniformData(RunParameters.SizeDistribution, RunParameters.PartitionDistribution);
                    break;
            }
        }

        #endregion Data

        Random _modelRandom = new Random();
        public Random ModelRandom { get { return _modelRandom; } }

        bool _restoreDefaultTimeLimit;
        string _lastParameters;

        public void RestoreDefaultTimeLimit()
        {
            if (_restoreDefaultTimeLimit)
            {
                TimeLimit = String.Empty;
                _restoreDefaultTimeLimit = false;
            }
        }

        public void SetRunParameters()
        {
            if (RunRandomTest)
            {
                RandomizeParameters();
            }
            else if (!String.Equals(_lastParameters, Parameters))
            {
                ClearParameters();
                SetParameters(Parameters);
            }

            RunParameters = BuildRunParameters();
            ReportParameters();
        }

        private RunParameters BuildRunParameters()
        {
            RunParameters rp = new RunParameters();
            TypeConverter tc;

            foreach (var tuple in GetCommonProperties())
            {
                PropertyInfo pi = tuple.Item1, piModel = tuple.Item2;
                switch (pi.Name)
                {
                    default:
                        object modelValue = piModel.GetValue(this);
                        if (piModel.PropertyType != typeof(String) ||
                            !String.IsNullOrWhiteSpace(modelValue as string))
                        {
                            try
                            {
                                object value = modelValue;
                                if (!pi.PropertyType.IsAssignableFrom(piModel.PropertyType))
                                {
                                    tc = TypeDescriptor.GetConverter(pi.PropertyType);
                                    value = tc.ConvertFrom(null, CultureInfo.InvariantCulture, modelValue);
                                }
                                pi.SetValue(rp, value);
                            }
                            catch (Exception ex)
                            {
                                rp.AddError(pi.Name, modelValue, ex.Message);
                            }
                        }
                        break;

                    case "TimeLimit":
                        try
                        {
                            if (String.IsNullOrWhiteSpace(TimeLimit))
                            {
                                pi.SetValue(rp, TimeSpan.MaxValue);
                            }
                            else
                            {
                                int seconds = Int32.Parse(TimeLimit);
                                pi.SetValue(rp, TimeSpan.FromSeconds(seconds));
                            }
                        }
                        catch (Exception ex)
                        {
                            rp.AddError(pi.Name, TimeLimit, ex.Message);
                        }
                        break;

                    case "DistributionsByLevel":
                        tc = TypeDescriptor.GetConverter(typeof(Distribution));
                        foreach (DistributionByLevel dbl in DistributionsByLevel)
                        {
                            try
                            {
                                object value = tc.ConvertFrom(null, CultureInfo.InvariantCulture, dbl.Distribution);
                                rp.DistributionsByLevel.Add(value as Distribution);
                            }
                            catch (Exception ex)
                            {
                                rp.AddError(String.Format("DistributionsByLevel[{0}]", dbl.Level), dbl.Distribution, ex.Message);
                            }
                        }
                        break;
                }
            }

            // seed the randomizers, if not already done
            if (rp.DataSeed == 0) rp.DataSeed = ModelRandom.Next();
            if (rp.ActionSeed == 0) rp.ActionSeed = ModelRandom.Next();

            return rp;
        }

        public void RandomizeParameters()
        {
            RandomAttribute randomAttribute;

            // we may overwrite a default TimeLimit - remember to restore it
            _restoreDefaultTimeLimit = String.IsNullOrEmpty(TimeLimit);

            ClearParameters();

            foreach (var tuple in GetCommonProperties())
            {
                PropertyInfo pi = tuple.Item1, piModel = tuple.Item2;
                randomAttribute = piModel.GetCustomAttribute<RandomAttribute>();
                Type range = (randomAttribute != null && randomAttribute.Range != null)
                                ? randomAttribute.Range : piModel.PropertyType;
                Distribution distribution = (randomAttribute != null)
                                ? randomAttribute.Distribution : GetDefaultDistribution(range);
                if (distribution != null)
                    distribution.BaseRandom = ModelRandom;

                object value = null;

                switch (pi.Name)
                {
                    default:
                        if (distribution != null)
                        {
                            if (range.IsEnum)
                            {
                                var values = Enum.GetValues(range);
                                value = values.GetValue(distribution.NextInt(0, values.Length));
                            }
                            else if (range == typeof(bool))
                            {
                                value = (distribution.NextInt(0, 2) == 1);
                            }
                            else if (range == typeof(int))
                            {
                                value = distribution.NextInt();
                            }
                            else if (range == typeof(double))
                            {
                                value = Math.Round(distribution.NextDouble(), 2);
                            }
                        }
                        break;

                    case "ViewportMargin":
                        if (distribution != null)
                        {
                            value = new Thickness(Math.Round(distribution.NextDouble(0), 2),
                                                  Math.Round(distribution.NextDouble(0), 2),
                                                  Math.Round(distribution.NextDouble(0), 2),
                                                  Math.Round(distribution.NextDouble(0), 2));
                        }
                        break;

                    case "SizeDistribution":
                        value = "E(10000,300)";
                        break;

                    case "PartitionDistribution":
                        value = String.Format(CultureInfo.InvariantCulture, "M({0})", ModelRandom.Next(1, 5));
                        break;

                    case "HeightDistribution":
                        value = "BN(0.8, 30,4, 400,100)";
                        break;

                    case "MarginDistribution":
                        value = "BN(0.8, 0,0, 5,1)";
                        break;

                    case "DistributionsByLevel":
                        // 1. choose a target size
                        double size = new ExponentialDistribution(10000, 300) { BaseRandom = ModelRandom }.NextDouble();

                        // 2. choose random numbers that add up to ln(size)
                        List<double> sizes = new List<double>();
                        double logSize = Math.Log(size);

                        while (logSize > 1)
                        {
                            // allocate a child with a random fraction of the remaining size
                            double childSize = ModelRandom.NextDouble() * logSize;
                            sizes.Add(childSize);
                            logSize -= childSize;
                        }

                        // include the remaining piece
                        sizes.Add(logSize);

                        // randomly permute the child sizes
                        for (int i = 1; i < sizes.Count; ++i)
                        {
                            int j = ModelRandom.Next(0, i + 1);
                            double t = sizes[i];
                            sizes[i] = sizes[j];
                            sizes[j] = t;
                        }

                        // 3. create levels with expected fanout of e^(sizes[i]).
                        // A tree with fanouts exactly equal to e^(sizes[i]) would
                        // have the desired size.  It's not literally true that the
                        // trees we actually create have the desired size on average,
                        // due to integer roundoff and the non-linearity of e^x.
                        // But it's true enough for our purposes.  The trees will
                        // exercise scrolling and virtualization with some randomness.
                        for (int i=0; i<sizes.Count; ++i)
                        {
                            double mu = Math.Exp(sizes[i]);
                            double sigma = mu / 3.0;
                            DistributionByLevel dbl = new DistributionByLevel();
                            dbl.Distribution = String.Format("N({0:F2},{1:F2})", mu, sigma);
                            dbl.SetLevel(i);
                            DistributionsByLevel.Add(dbl);
                        }
                        break;
                }

                if (value != null)
                {
                    piModel.SetValue(this, value);
                }

                // some properties affect others
                switch (pi.Name)
                {
                    // limit Jump test to 2 minutes
                    case "ScrollAction":
                        if ((ScrollAction)value == HangTest.ScrollAction.Jump &&
                            _restoreDefaultTimeLimit)
                        {
                            TimeLimit = "120";
                        }
                        break;

                    // choose a random cache length, depending on the cache unit
                    case "CacheUnit":
                        if (ModelRandom.NextDouble() < 0.8)
                        {
                            // when not using an empty cache... 
                            Distribution d;
                            switch ((VirtualizationCacheLengthUnit)value)
                            {
                                case VirtualizationCacheLengthUnit.Pixel:
                                    d = new NormalDistribution(500, 20) { BaseRandom = ModelRandom };
                                    CacheLength = String.Format(CultureInfo.InvariantCulture, "{0},{1}",
                                        d.NextInt(0), d.NextInt(0));
                                    break;
                                case VirtualizationCacheLengthUnit.Item:
                                    d = new NormalDistribution(10, 3) { BaseRandom = ModelRandom };
                                    CacheLength = String.Format(CultureInfo.InvariantCulture, "{0},{1}",
                                        d.NextInt(0), d.NextInt(0));
                                    break;
                                case VirtualizationCacheLengthUnit.Page:
                                    d = new ExponentialDistribution(1) { BaseRandom = ModelRandom };
                                    CacheLength = String.Format(CultureInfo.InvariantCulture, "{0},{1}",
                                        Math.Round(d.NextDouble(),2), Math.Round(d.NextDouble(), 2));
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        Distribution GetDefaultDistribution(Type range)
        {
            Distribution result = null;

            if (range.IsEnum)
            {
                result = new UniformDistribution(0, Enum.GetNames(range).Length);
            }
            else if (range == typeof(bool))
            {
                result = new ProbabilityDistribution(0.5);
            }

            return result;
        }

        public void EnableAllVerification()
        {
            VerifyLayout = true;
            VerifyScrolling = true;
            VerifyProgress = true;
        }

        void ReportParameters()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var tuple in GetCommonProperties())
            {
                PropertyInfo pi = tuple.Item1, piModel = tuple.Item2;
                switch (pi.Name)
                {
                    default:
                        object sourceValue = piModel.GetValue(this);

                        bool isDefault = false;

                        if (piModel.PropertyType == typeof(String))
                        {
                            isDefault = String.IsNullOrWhiteSpace(sourceValue as string);
                        }
                        else if (piModel.PropertyType.IsValueType)
                        {
                            isDefault = Object.Equals(sourceValue, Activator.CreateInstance(piModel.PropertyType));
                        }

                        if (!isDefault)
                        {
                            sb.AppendFormat("{0}={1}; ", pi.Name, sourceValue);
                        }
                        break;

                    case "DataSeed":
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}; ", pi.Name, RunParameters.DataSeed);
                        break;

                    case "ActionSeed":
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}; ", pi.Name, RunParameters.ActionSeed);
                        break;

                    case "DistributionsByLevel":
                        if (DistributionsByLevel.Count > 0)
                        {
                            sb.AppendFormat("{0}=[", pi.Name);
                            for (int i=0; i<DistributionsByLevel.Count; ++i)
                            {
                                if (i > 0) sb.Append(",");
                                sb.Append(DistributionsByLevel[i].Distribution.Trim());
                            }
                            sb.Append("]; ");
                        }
                        break;
                }
            }

            Parameters = sb.ToString();
        }

        public void ClearParameters()
        {
            foreach (var tuple in GetCommonProperties())
            {
                PropertyInfo pi = tuple.Item1, piModel = tuple.Item2;
                switch (pi.Name)
                {
                    default:
                        object sourceValue = null;

                        if (piModel.PropertyType.IsValueType)
                        {
                            sourceValue = Activator.CreateInstance(piModel.PropertyType);
                        }

                        piModel.SetValue(this, sourceValue);
                        break;

                    case "DistributionsByLevel":
                        DistributionsByLevel.Clear();
                        break;

                    case "ScrollPriority":
                        ScrollPriority = DispatcherPriority.ContextIdle;
                        break;
                }
            }
        }

        public void SetParameters(string parameters)
        {
            string[] p = parameters.Split(';');
            foreach (string s in p)
            {
                string[] a = s.Split('=');
                if (a.Length == 2)
                {
                    try
                    {
                        SetParameter(a[0], a[1]);
                    }
                    catch
                    {
                        // ignore errors for now
                    }
                }
            }
        }

        void SetParameter(string name, string value)
        {
            name = name.Trim();
            value = value.Trim();

            PropertyInfo pi = typeof(Model).GetProperty(name);
            if (pi == null)
            {
                throw new MissingMemberException(nameof(Model), name);
            }

            switch (name)
            {
                default:
                    TypeConverter converter = TypeDescriptor.GetConverter(pi.PropertyType);
                    pi.SetValue(this, converter.ConvertFrom(null, CultureInfo.InvariantCulture, value));
                    break;

                case "DistributionsByLevel":
                    // typical value is "[N(50,7),U,C(10)]", i.e. a comma-separated
                    // list of distributions, each of which may have comma-separated
                    // args.  To parse the list, we have to ignore commas inside parens.
                    if (value.StartsWith("[") && value.EndsWith("]"))
                    {
                        int level = 0;
                        int start = 1;
                        for (int i = 1, n = value.Length; i < n; ++i)
                        {
                            switch (value[i])
                            {
                                case '(': ++level; break;
                                case ')': --level; break;
                                case ',': if (level == 0) goto case ']'; break;
                                default: break;

                                case ']':
                                    DistributionByLevel dbl = new DistributionByLevel();
                                    dbl.Distribution = value.Substring(start, i - start).Trim();
                                    dbl.SetLevel(DistributionsByLevel.Count);
                                    DistributionsByLevel.Add(dbl);
                                    start = i + 1;
                                    break;
                            }
                        }
                    }
                    else
                        throw new FormatException("Missing enclosing [] in " + value);
                    break;
            }
        }

        IEnumerable<Tuple<PropertyInfo, PropertyInfo>> GetCommonProperties()
        {
            foreach (PropertyInfo piRunParameters in typeof(RunParameters).GetProperties())
            {
                PropertyInfo piModel = typeof(Model).GetProperty(piRunParameters.Name);
                if (piModel != null)
                    yield return new Tuple<PropertyInfo, PropertyInfo>(piRunParameters, piModel);
            }
        }

        public event EventHandler IsRunningChanged;

        void RaiseIsRunningChanged()
        {
            var handler = IsRunningChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }

    public enum DataGenerationMethod { NonUniform, FixedHeight, File }
    public enum ScrollAction { LineUp, LineDown, MouseWheelUp, MouseWheelDown, PageUp, PageDown, Jump }
    public enum HeaderPosition { BeforeSubtree, AfterSubtree, SideBySide }

    public class DistributionByLevel : PropertyChangedBase
    {
        int _level;
        public int Level
        {
            get { return _level; }
            private set { _level = value; OnPropertyChanged(nameof(Level)); }
        }

        string _distribution;
        public string Distribution
        {
            get { return _distribution; }
            set { _distribution = value; OnPropertyChanged(nameof(Distribution)); }
        }

        internal void SetLevel(int level)
        {
            Level = level;
        }
    }

    public class DistributionsByLevel : ObservableCollection<DistributionByLevel>
    {
        protected override void InsertItem(int index, DistributionByLevel item)
        {
            item.SetLevel(index);
            base.InsertItem(index, item);
        }
    }

    public class RandomAttribute : Attribute
    {
        public RandomAttribute()
        { }

        public RandomAttribute(string distribution)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(Distribution));
            Distribution = (Distribution)tc.ConvertFrom(null, CultureInfo.InvariantCulture, distribution);
        }

        public Distribution Distribution { get; set; }
        public Type Range { get; set; }
    }
}
