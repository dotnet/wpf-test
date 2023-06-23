using System.ComponentModel;

namespace Microsoft.Test.Controls.UIADataSources
{
    /// <summary>
    /// UIAPerson
    /// </summary>
    public class UIAPerson : INotifyPropertyChanged
    {
        public UIAPerson() { }
        
        public UIAPerson(int id, string name, int age, bool likesNoodle, string noodle, string website, string food, string car)
        {
            this.id = id;
            this.name = name;
            this.age = age;
            this.likesNoodle = likesNoodle;
            this.noodle = noodle;
            this.website = website;
            this.food = food;
            this.car = car;
        }

        private int id;
        private string name;
        private int age;
        private bool likesNoodle;
        private string noodle;
        private string website;
        private string food;
        private string car;

        protected void Notify(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int ID
        {
            get { return this.id; }
            set
            {
                if (this.id == value)
                {
                    return;
                }

                this.id = value;
                Notify("ID");
            }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name == value)
                {
                    return;
                }

                this.name = value;
                Notify("Name");
            }
        }

        public int Age
        {
            get { return this.age; }
            set
            {
                if (this.age == value)
                {
                    return;
                }

                this.age = value;
                Notify("Age");
            }
        }

        public bool LikesNoodle
        {
            get { return this.likesNoodle; }
            set
            {
                if (this.likesNoodle == value)
                {
                    return;
                }

                this.likesNoodle = value;
                Notify("LikesNoodle");
            }
        }

        public string Noodle
        {
            get { return this.noodle; }
            set
            {
                if (this.noodle == value)
                {
                    return;
                }

                this.noodle = value;
                Notify("Noodle");
            }
        }

        public string WebSite
        {
            get { return this.website; }
            set
            {
                if (this.website == value)
                {
                    return;
                }

                this.website = value;
                Notify("WebSite");
            }
        }

        public string Food
        {
            get { return this.food; }
            set
            {
                if (this.food == value)
                {
                    return;
                }

                this.food = value;
                Notify("Food");
            }
        }

        public string Car
        {
            get { return this.car; }
            set
            {
                if (this.car == value)
                {
                    return;
                }

                this.car = value;
                Notify("Car");
            }
        }
    }
}
