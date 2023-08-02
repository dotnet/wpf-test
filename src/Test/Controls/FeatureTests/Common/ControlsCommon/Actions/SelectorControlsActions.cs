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
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls.Actions
{
    public static class SelectorControlsActions
    {
        public static void RunSelectorControlsEventBehaviorTests(Selector selector)
        {
            EventInfo[] eventInfos = selector.GetType().GetEvents();
            foreach (EventInfo eventInfo in eventInfos)
            {
                if (eventInfo.DeclaringType.IsSubclassOf(typeof(Selector)) || eventInfo.DeclaringType.Equals(typeof(Selector)))
                {
                    ISelectorTest selectorEventTest = ObjectFactory.CreateObjectFromTypeName(eventInfo.Name + "EventTest") as ISelectorTest;
                    if (selectorEventTest != null)
                    {
                        DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                        selectorEventTest.Run(selector);
                    }
                    else
                    {
                        throw new TestValidationException("ISelectorTest instance is null.");
                    }
                }
            }
        }
    }
}


