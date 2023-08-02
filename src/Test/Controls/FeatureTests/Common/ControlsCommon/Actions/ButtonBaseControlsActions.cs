using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using System.Reflection;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Input;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Test;
using Microsoft.Test.Threading;
using System.Windows.Threading;

namespace Avalon.Test.ComponentModel.Actions
{
    public static class ButtonBaseControlsActions
    {
        public static void RunButtonBaseControlsBehaviorEventsTests(ButtonBase buttonBase)
        {
            EventInfo[] eventInfos = buttonBase.GetType().GetEvents();
            foreach (EventInfo eventInfo in eventInfos)
            {
                if (eventInfo.DeclaringType.IsSubclassOf(typeof(ButtonBase)) || eventInfo.DeclaringType.Equals(typeof(ButtonBase)))
                {
                    IButtonBaseTest buttonEventTest = ObjectFactory.CreateObjectFromTypeName(eventInfo.Name + "EventTest") as IButtonBaseTest;
                    if (buttonEventTest != null)
                    {
                        DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                        buttonEventTest.Run(buttonBase);
                    }
                    else
                    {
                        throw new TestValidationException("IButtonBaseEventTest instance is null.");
                    }
                }
            }
        }
    }
}


