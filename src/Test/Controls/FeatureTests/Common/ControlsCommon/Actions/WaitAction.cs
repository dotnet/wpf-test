using System;
using System.Windows;
using System.Windows.Media;
using System.Reflection;

using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// wait for a period of time
    /// </summary>
    [Parser("TimeToWait")]
    public class WaitAction : ActionSeparator
    {
        #region ActionSeparator Members

        public override void Do(object testObject)
        {
            if (TimeToWait == TimeSpan.Zero)
                QueueHelper.WaitTillQueueItemsProcessed();
            else
                QueueHelper.WaitTillTimeout(TimeToWait);
        }

        #endregion

        #region TimeToWait

        /// <summary>
        /// time to wait.
        /// if set to Zero, it will wait till queue items get processed.
        /// </summary>
        public TimeSpan TimeToWait
        {
            get { return _timeToWait; }
            set { _timeToWait = value; }
        }

        private TimeSpan _timeToWait = TimeSpan.Zero;

        #endregion
    }
}
