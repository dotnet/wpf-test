using System;
using System.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Tracking tic-tac-toe moves
    /// </summary>
    public class PlayerMove : INotifyPropertyChanged
    {
        public PlayerMove(string playerName, int moveNumber)
        {
            this.playerName = playerName;
            this.moveNumber = moveNumber;
        }

        string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set
            {
                if (String.Compare(playerName, value) == 0) { return; }
                playerName = value;
                Notify("PlayerName");
            }
        }
        int moveNumber;
        public int MoveNumber
        {
            get { return moveNumber; }
            set
            {
                if (moveNumber == value) { return; }
                moveNumber = value;
                Notify("MoveNumber");
            }
        }
        bool isPartOfWin = false;
        public bool IsPartOfWin
        {
            get { return isPartOfWin; }
            set
            {
                if (isPartOfWin == value) { return; }
                isPartOfWin = value;
                Notify("IsPartOfWin");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
