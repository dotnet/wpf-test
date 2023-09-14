using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls.Actions
{
    /// <summary>
    /// Encapsulates RangeBase controls actions.
    /// </summary>
    public static class RangeBaseControlsActions
    {
        /// <summary>
        /// Run RangeBase Controls Event Behavior Tests
        /// </summary>
        /// <param name="menubase"></param>
        public static void RunRangeBaseControlsEventBehaviorTests(RangeBase rangeBase)
        {
            EventInfo[] eventInfos = rangeBase.GetType().GetEvents();
            foreach (EventInfo eventInfo in eventInfos)
            {
                if (eventInfo.DeclaringType.IsSubclassOf(typeof(RangeBase)) || eventInfo.DeclaringType.Equals(typeof(RangeBase)))
                {
                    IRangeBaseTest rangeBaseEventTest = ObjectFactory.CreateObjectFromTypeName(eventInfo.Name + "EventTest") as IRangeBaseTest;
                    if (rangeBaseEventTest != null)
                    {
                        rangeBaseEventTest.Run(rangeBase);
                        DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                    }
                    else
                    {
                        throw new TestValidationException("IRangeBaseTest instance is null.");
                    }
                }
            }
        }
    }
}


