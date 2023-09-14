using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public class DataTriggerTicTacToeStyleValidator : TicTacToeStyleValidator
    {
        public DataTriggerTicTacToeStyleValidator(FrameworkElement frameworkElement, IEnumerator enumerator)
            : base(frameworkElement, enumerator)
        {
        }

        /// <summary>
        /// Validate visual element TextBlock Foreground property that is binding to nonvisual object PlayerMove
        /// through DataTrigger to watch PlayerName property change.
        /// </summary>
        public override void Validate()
        {
            enumerator.Reset();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            while (enumerator.MoveNext())
            {
                Button button = (Button)enumerator.Current;

                // We need to go into visualtree to get DataTemplate UIElement TextBlock
                // and validate it
                Collection<TextBlock> textBlocks = VisualTreeHelper.GetVisualChildren<TextBlock>(button);

                foreach (TextBlock textBlock in textBlocks)
                {
                    switch (textBlock.Text)
                    {
                        case "X":
                            if (textBlock.Foreground != Brushes.Red)
                            {
                                resultButton.Content = "Fail: X button first TextBlock Foreground != Red";
                                return;
                            }
                            break;
                        case "O":
                            if (textBlock.Foreground != Brushes.Green)
                            {
                                resultButton.Content = "Fail: O button first TextBlock Foreground != Green";
                                return;
                            }
                            break;
                    }
                }
            }
        }
    }
}
