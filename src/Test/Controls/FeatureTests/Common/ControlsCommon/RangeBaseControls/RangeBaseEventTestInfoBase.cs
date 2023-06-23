using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// RangeBase event test info base
    /// </summary>
    class RangeBaseEventTestInfoBase
    {
        private RangeBase rangeBase;
        public RangeBase RangeBase
        {
            set { rangeBase = value; }
            get { return rangeBase; }
        }
        private string eventName;
        public string EventName
        {
            set { eventName = value; }
            get { return eventName; }
        }
        private double oldValue;
        public double OldValue
        {
            set { oldValue = value; }
            get { return oldValue; }
        }
        private double newValue;
        public double NewValue
        {
            set { newValue = value; }
            get { return newValue; }
        }
    }
}
