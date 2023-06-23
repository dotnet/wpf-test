using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public class DataTemplateTicTacToeStyleValidator : TicTacToeStyleValidator
    {
        public DataTemplateTicTacToeStyleValidator(FrameworkElement frameworkElement, IEnumerator enumerator, double expectedFontSize, FontWeight expectedFontWeight)
            : base(frameworkElement, enumerator, expectedFontSize, expectedFontWeight)
        {
        }

        /// <summary>
        /// Validate visual element TextBlock FontSize and FontWeight properties that are set in DataTemplate to bind
        /// nonvisual object PlayerMove PlayerName and MoveNumber properties.
        /// </summary>
        public override void Validate()
        {
            enumerator.Reset();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            while (enumerator.MoveNext())
            {
                Button button = (Button)enumerator.Current;

                // We need to go into visualtree to get DataTemplate UIElement TextBlock
                // and validate it.
                Collection<TextBlock> textBlocks = VisualTreeHelper.GetVisualChildren<TextBlock>(button);

                foreach (TextBlock textBlock in textBlocks)
                {
                    if (textBlock.FontSize != expectedFontSize)
                    {
                        resultButton.Content = "Fail: " + button.Name + " first TextBlock FontSize != " + expectedFontSize;
                        return;
                    }

                    if (textBlock.FontWeight != expectedFontWeight)
                    {
                        resultButton.Content = "Fail: " + button.Name + " first TextBlock FontWeight != " + expectedFontWeight.ToString();
                        return;
                    }

                    break;
                }
            }
        }

        protected override void AssignContentToButton(Button button)
        {
            button.Content = new PlayerMove(CurrentPlayer, ++moveNumber);
        }
    }
}
