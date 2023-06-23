using Microsoft.Test.Controls.Helpers;

namespace Microsoft.Test.Controls
{
    class SliderEventTestInfo : RangeBaseEventTestInfoBase
    {
        private SliderRepeatButtonCommandName sliderRepeatButtonCommandName;
        public SliderRepeatButtonCommandName SliderRepeatButtonCommandName
        {
            set { sliderRepeatButtonCommandName = value; }
            get { return sliderRepeatButtonCommandName; }
        }
    }
}
