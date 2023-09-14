using System;
using System.Windows.Controls.Primitives;
using Microsoft.Test;

namespace Avalon.Test.ComponentModel
{
    public abstract class ToggleButtonValidation
    {
        protected bool? initialIsCheckedState = false;
        protected void ValidateEndState(ToggleButton toggleButton)
        {
            bool? endIsCheckedState = toggleButton.IsChecked;

            // Nothing happens when it is a RadioButton with IsChecked=true, so skip validating this scenario.
            if (!(toggleButton.GetType().Name.Equals("RadioButton") && toggleButton.IsChecked == true))
            {
                if (toggleButton.IsThreeState)
                {
                    if ((initialIsCheckedState == false && endIsCheckedState != true)
                           || (initialIsCheckedState == true && endIsCheckedState != null)
                           || (initialIsCheckedState == null && endIsCheckedState != false))
                    {
                        throw new TestValidationException("Fail: IsThreeState is true and initial IsChecked is " +
                            initialIsCheckedState.ToString() + ".  Result end state of IsChecked is " + (endIsCheckedState == null ? "null" : endIsCheckedState.ToString()) + ".");
                    }
                }
                else
                {
                    if (initialIsCheckedState == endIsCheckedState)
                    {
                        throw new TestValidationException("Fail: IsThreeState is false and initial IsChecked is " +
                            initialIsCheckedState.ToString() + ".  Result end state of IsChecked is " + (endIsCheckedState == null ? "null" : endIsCheckedState.ToString()) + ".");
                    }
                }
            }
            // Reset initial IsChecked state.
            toggleButton.IsChecked = initialIsCheckedState;
        }
    }
}
