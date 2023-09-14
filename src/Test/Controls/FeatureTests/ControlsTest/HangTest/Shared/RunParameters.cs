using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HangTest
{
    public class RunParameters
    {
        public TimeSpan TimeLimit { get; set; }
        public ScrollUnit ScrollUnit { get; set; }
        public VirtualizationMode VirtualizationMode { get; set; }
        public VirtualizationCacheLengthUnit CacheUnit { get; set; }
        public VirtualizationCacheLength CacheLength { get; set; }
        public bool UseLayoutRounding { get; set; }
        public FlowDirection FlowDirection { get; set; }
        public double TreeViewHeight { get; set; }
        public HeaderPosition HeaderPosition { get; set; }
        public bool BindItemHeight { get; set; }
        public bool BindItemMargin { get; set; }
        public Thickness ViewportMargin { get; set; }
        public int DataSeed { get; set; }
        public DataGenerationMethod DataGenerationMethod { get; set; }
        public string Filename { get; set; }
        public string FilterAttribute { get; set; }
        public string ChildrenTag { get; set; }
        public List<Distribution> DistributionsByLevel { get { return _distributionByLevel; } }
        public Distribution SizeDistribution { get; set; }
        public Distribution PartitionDistribution { get; set; }
        public Distribution HeightDistribution { get; set; }
        public Distribution MarginDistribution { get; set; }
        public bool VerifyLayout { get; set; }
        public bool VerifyScrolling { get; set; }
        public bool VerifyProgress { get; set; }
        public int ActionSeed { get; set; }
        public ScrollAction ScrollAction { get; set; }
        public DispatcherPriority ScrollPriority { get; set; }

        public List<string> Errors { get { return _errors; } }
        public bool HasErrors { get { return _errors != null && _errors.Count > 0; } }

        public Random DataRNG { get { return _dataRNG; } }
        public Random ActionRNG { get { return _actionRNG; } }

        public void AddError(string name, object value, string message=null)
        {
            AddRawError(String.Format("'{0}' is not a legal value for {1}. {2}", value, name, message));
        }

        public void AddRawError(string error)
        {
            if (_errors == null)
                _errors = new List<string>();
            _errors.Add(error);
        }

        public void Prepare()
        {
            _dataRNG = new Random(DataSeed);
            _actionRNG = new Random(ActionSeed);

            foreach (Distribution distribution in DistributionsByLevel)
            {
                distribution.BaseRandom = _dataRNG;
            }

            if (SizeDistribution == null) SizeDistribution = new ConstantDistribution(25);
            if (PartitionDistribution == null) PartitionDistribution = new UniformDistribution();
            if (HeightDistribution == null) HeightDistribution = new ExponentialDistribution(45);
            if (MarginDistribution == null) MarginDistribution = new ConstantDistribution(0);

            SizeDistribution.BaseRandom = _dataRNG;
            PartitionDistribution.BaseRandom = _dataRNG;
            HeightDistribution.BaseRandom = _dataRNG;
            MarginDistribution.BaseRandom = _dataRNG;
        }

        List<Distribution> _distributionByLevel = new List<Distribution>();
        List<string> _errors;
        Random _dataRNG, _actionRNG;
    }
}
