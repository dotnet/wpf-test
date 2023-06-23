using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls.Helpers
{
    public enum SliderRepeatButtonCommandName
    {
        DecreaseLarge,
        IncreaseLarge
    }

    /// <summary>
    /// Slider Helper
    /// </summary>
    public static class SliderHelper
    {
        public static RepeatButton FindRepeatButton(Slider slider, SliderRepeatButtonCommandName sliderRepeatButtonCommandName)
        {
            RepeatButton[] repeatButtons = (RepeatButton[])VisualTreeUtils.FindPartByType(slider, typeof(RepeatButton)).ToArray(typeof(RepeatButton));

            for (int j = 0; j < repeatButtons.Length; j++)
            {
                RoutedCommand routedCommand = repeatButtons[j].Command as RoutedCommand;

                if (routedCommand == null)
                {
                    throw new ArgumentException("Could not find RoutedCommand of the RepeatButton.");
                }

                if (String.Compare(routedCommand.Name, sliderRepeatButtonCommandName.ToString(), true) == 0)
                {
                    return repeatButtons[j];
                }
            }

            return null;
        }
    }
}


