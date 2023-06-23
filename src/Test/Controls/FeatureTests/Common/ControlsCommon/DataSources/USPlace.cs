using System.ComponentModel;

namespace Microsoft.Test.Controls
{
    public class USPlace : INotifyPropertyChanged
    {
        private string name;
        private string state;

        public USPlace()
        {
            name = "";
            state = "";
        }

        public USPlace(string name, string state)
        {
            this.name = name;
            this.state = state;
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (this.name == value)
                {
                    return;
                }

                name = value;
                Notify("Name");
            }
        }

        public string State
        {
            get { return state; }
            set
            {
                if (this.state == value)
                {
                    return;
                }

                state = value;
                Notify("State");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void Notify(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
