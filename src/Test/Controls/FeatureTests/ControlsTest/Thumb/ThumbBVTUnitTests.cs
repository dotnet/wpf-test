using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.UnitTests
{
    [TargetType(typeof(Thumb))]
    public class ThumbDragDeltaBVT : IUnitTest
    {
        public ThumbDragDeltaBVT()
        {
        }

        /// <summary>
        /// Test Mouse drag the thumb.
        /// </summary>
        /// <param name="thumb"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Thumb thumb = (Thumb)obj;
            isDragDelta = false;
            TestLog.Current.LogEvidence("Attach DragDeltaevent handler to Thumb.");
            thumb.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));

            TestLog.Current.LogDebug("Drag the mouse");

            UserInput.MouseLeftDown(thumb, 5, 5);
            UserInput.MouseMove(thumb, 10, 10);
            UserInput.MouseLeftUp(thumb, 10, 10);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!isDragDelta)
            {
                TestLog.Current.LogDebug("Fail: Thumb DragDelta event does not fire.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private void OnDragDelta(object sender, RoutedEventArgs e)
        {
            isDragDelta = true;
        }

        //necessary because of testing DragDelta event
        bool isDragDelta;
    }
}


