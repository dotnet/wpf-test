using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public class TicTacToeStyleValidator : TicTacToeValidator
    {
        public TicTacToeStyleValidator(FrameworkElement frameworkElement, IEnumerator enumerator)
            : this(frameworkElement, enumerator, 0, FontWeights.Normal)
        {
        }

        public TicTacToeStyleValidator(FrameworkElement frameworkElement, IEnumerator enumerator, double expectedFontSize, FontWeight expectedFontWeight)
            : base(frameworkElement, enumerator)
        {
            this.enumerator = enumerator;
            this.expectedFontSize = expectedFontSize;
            this.expectedFontWeight = expectedFontWeight;
            resultButton = (Button)frameworkElement.FindName("result");
        }

        protected IEnumerator enumerator;
        protected Button resultButton;
        protected double expectedFontSize;
        protected FontWeight expectedFontWeight;


        /// <summary>
        /// Validate the Button FontSize and FontWeight properties that are set through Style.
        /// </summary>
        public virtual void Validate()
        {
            enumerator.Reset();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            while (enumerator.MoveNext())
            {
                Button button = (Button)enumerator.Current;

                if (button.FontSize != expectedFontSize)
                {
                    resultButton.Content = "Fail: " + button.Name + " FontSize != " + expectedFontSize;
                    return;
                }

                if (button.FontWeight != expectedFontWeight)
                {
                    resultButton.Content = "Fail: " + button.Name + " FontWeight != " + expectedFontWeight.ToString();
                    return;
                }
            }
        }

        /// <summary>
        /// Mouse click Tic Tac Toe cells.
        /// </summary>
        public virtual void ClickCells()
        {
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                Button button = (Button)enumerator.Current;

                InputHelper.MouseClickCenter(button, MouseButton.Left);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            }
        }
    }
}
