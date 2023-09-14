using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    public class TicTacToeValidator
    {
        public TicTacToeValidator(FrameworkElement frameworkElement, IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                Button cell = (Button)enumerator.Current;
                cells.Add(cell);
            }

            foreach (Button cell in cells)
            {
                cell.Click += new RoutedEventHandler(Cell_Click);
            }

            this.frameworkElement = frameworkElement;

            NewGame();
        }

        private FrameworkElement frameworkElement;
        protected int moveNumber;
        // Track the current player (X or O)
        private string currentPlayer;
        // Track the list of cells for finding a winner, etc.
        private List<Button> cells = new List<Button>();

        // Wrapper around the current player for future expansion,
        // e.g., updating status text with the current player
        protected string CurrentPlayer
        {
            get { return currentPlayer; }
            set
            {
                currentPlayer = value;
                TextBlock textblock = (TextBlock)frameworkElement.FindName("statusTextBlock");
                if (textblock != null)
                {
                    textblock.Text = "It's your turn, " + currentPlayer;
                }
            }
        }

        private bool TieGame()
        {
            return false;
        }

        private bool HasWon(string currentPlayer)
        {
            return false;
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            // Don't let multiple clicks change the player for a cell
            if (button.Content != null) { return; }
            // Set button content
            AssignContentToButton(button);

            // Check for winner or a tie
            if (HasWon(currentPlayer))
            {
                MessageBox.Show("Winner!", "Game Over");
                NewGame();
                return;
            }
            else if (TieGame())
            {
                MessageBox.Show("No Winner!", "Game Over");
                NewGame();
                return;
            }

            // Switch player
            if (CurrentPlayer == "X")
            {
                Style xStyle = (Style)frameworkElement.TryFindResource("XStyle");

                if (xStyle != null)
                {
                    button.Style = xStyle;
                }

                CurrentPlayer = "O";
            }
            else
            {
                Style oStyle = (Style)frameworkElement.TryFindResource("OStyle");

                if (oStyle != null)
                {
                    button.Style = oStyle;
                }

                CurrentPlayer = "X";
            }
        }

        protected virtual void AssignContentToButton(Button button)
        {
            button.Content = CurrentPlayer;
        }

        // Use the buttons to track game state
        protected void NewGame()
        {
            foreach (Button cell in cells)
            {
                cell.ClearValue(Button.ContentProperty);
            }
            CurrentPlayer = "X";
            moveNumber = 0;
        }
    }
}
