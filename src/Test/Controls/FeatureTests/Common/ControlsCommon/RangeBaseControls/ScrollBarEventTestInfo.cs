using System.Windows.Controls.Primitives;
using Microsoft.Test.Controls.Helpers;

namespace Microsoft.Test.Controls
{
    class ScrollBarEventTestInfo : RangeBaseEventTestInfoBase
    {
        private ScrollingMode scrollBarRepeatButtonCommandName;
        public ScrollingMode ScrollBarRepeatButtonCommandName
        {
            set { scrollBarRepeatButtonCommandName = value; }
            get { return scrollBarRepeatButtonCommandName; }
        }
        private ScrollEventType scrollEventType;
        public ScrollEventType ScrollEventType
        {
            set { scrollEventType = value; }
            get { return scrollEventType; }
        }
    }
}
