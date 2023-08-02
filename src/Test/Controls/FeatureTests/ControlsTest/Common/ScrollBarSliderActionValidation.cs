using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Validations
{

    public class ScrollBarSliderActionValidation 
    {
        public enum ActionToValidate
        {
            LargeChangeDecreaseButton,
            LargeChangeIncreaseButton,
            SmallChangeDecreaseButton,
            SmallChangeIncreaseButton,
            IncreaseMaximum,
            DecreaseMinimum,
        }
    }
}
