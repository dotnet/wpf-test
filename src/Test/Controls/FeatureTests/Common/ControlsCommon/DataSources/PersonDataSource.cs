using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls.DataSources
{
    public class People : ObservableCollection<Person>
    {
        private Random random = new Random();        

        public People()
        {
            CreateGenericPersonData(1);
        }

        public People(int multiplier)
        {
            CreateGenericPersonData(multiplier);
        }        

        private void CreateGenericPersonData(int multiplier)
        {
            if (multiplier > 0)
            {
                for (int i = 0; i < multiplier; i++)
                {
                    Add(new Person("George", "Washington", GenerateRandomDate()));
                    Add(new Person("John", "Adams", GenerateRandomDate()));
                    Add(new Person("Thomas", "Jefferson", GenerateRandomDate()));
                    Add(new Person("James", "Madison", GenerateRandomDate()));
                    Add(new Person("James", "Monroe", GenerateRandomDate()));
                    Add(new Person("John", "Quincy", "Adams", GenerateRandomDate()));
                    Add(new Person("Andrew", "Jackson", GenerateRandomDate()));
                    Add(new Person("Martin", "Van Buren", GenerateRandomDate()));
                    Add(new Person("William", "H.", "Harrison", GenerateRandomDate()));
                    Add(new Person("John", "Tyler", GenerateRandomDate()));
                    Add(new Person("James", "K.", "Polk", GenerateRandomDate()));
                    Add(new Person("Zachary", "Taylor", GenerateRandomDate()));
                    Add(new Person("Millard", "Fillmore", GenerateRandomDate()));
                    Add(new Person("Franklin", "Pierce", GenerateRandomDate()));
                    Add(new Person("James", "Buchanan", GenerateRandomDate()));
                    Add(new Person("Abraham", "Lincoln", GenerateRandomDate()));
                    Add(new Person("Andrew", "Johnson", GenerateRandomDate()));
                    Add(new Person("Ulysses", "S.", "Grant", GenerateRandomDate()));
                    Add(new Person("Rutherford", "B.", "Hayes", GenerateRandomDate()));
                    Add(new Person("James", "Garfield", GenerateRandomDate()));
                    Add(new Person("Chester", "A.", "Arthur", GenerateRandomDate()));
                    Add(new Person("Grover", "Cleveland", GenerateRandomDate()));
                    Add(new Person("Benjamin", "Harrison", GenerateRandomDate()));
                    Add(new Person("William", "McKinley", GenerateRandomDate()));
                    Add(new Person("Theodore", "Roosevelt", GenerateRandomDate()));
                    Add(new Person("William", "H.", "Taft", GenerateRandomDate()));
                    Add(new Person("Woodrow", "Wilson", GenerateRandomDate()));
                    Add(new Person("Warren", "G.", "Harding", GenerateRandomDate()));
                    Add(new Person("Calvin", "Coolidge", GenerateRandomDate()));
                    Add(new Person("Herbert", "Hoover", GenerateRandomDate()));
                    Add(new Person("Franklin", "D.", "Roosevelt", GenerateRandomDate()));
                    Add(new Person("Harry", "S.", "Truman", GenerateRandomDate()));
                    Add(new Person("Dwight", "D.", "Eisenhower", GenerateRandomDate()));
                    Add(new Person("John", "F.", "Kennedy", GenerateRandomDate()));
                    Add(new Person("Lyndon", "B.", "Johnson", GenerateRandomDate()));
                    Add(new Person("Richard", "Nixon", GenerateRandomDate()));
                    Add(new Person("Gerald", "Ford", GenerateRandomDate()));
                    Add(new Person("Jimmy", "Carter", GenerateRandomDate()));
                    Add(new Person("Ronald", "Reagan", GenerateRandomDate()));
                    Add(new Person("George", "Bush", GenerateRandomDate()));
                    Add(new Person("Bill", "Clinton", GenerateRandomDate()));
                    Add(new Person("George", "W.", "Bush", GenerateRandomDate()));
                }
            }
        }

        private DateTime GenerateRandomDate()
        {
            int randInt = random.Next();
            return new DateTime(Convert.ToInt64(randInt));
        }
    }

    /// <summary>
    /// Add this new collection for a group of datagrid tests
    /// </summary>
    public class NewPeople : ObservableCollection<Person>
    {
        public NewPeople()
        {
            Add(new Person("George", string.Empty, "Washington", new DateTime(1999, 1, 26)));
            Add(new Person("John", string.Empty, "Adams", new DateTime(2000, 2, 26)));
            Add(new Person("Thomas", string.Empty, "Jefferson", new DateTime(2001, 3, 26)));
            Add(new Person("James", string.Empty, "Madison", new DateTime(2002, 4, 26)));
            Add(new Person("James", string.Empty, "Monroe", new DateTime(2003, 5, 26)));
            Add(new Person("John", "Quincy", "Adams", new DateTime(2004, 6, 26)));
            Add(new Person("Andrew", string.Empty, "Jackson", new DateTime(2005, 7, 26)));
            Add(new Person("Martin", string.Empty, "Van Buren", new DateTime(2006, 8, 26)));
            Add(new Person("William", "H.", "Harrison", new DateTime(2007, 9, 26)));
            Add(new Person("John", string.Empty, "Tyler", new DateTime(2008, 10, 26)));
        }
    }

    public class EditablePeople : ObservableCollection<EditablePerson>
    {
        private Random random = new Random();

        public EditablePeople()
        {
            CreateGenericPersonData(1);
        }

        public EditablePeople(int multiplier)
        {
            CreateGenericPersonData(multiplier);
        }

        private void CreateGenericPersonData(int multiplier)
        {
            if (multiplier > 0)
            {
                for (int i = 0; i < multiplier; i++)
                {
                    Add(new EditablePerson("George", "Washington", GenerateRandomDate()));
                    Add(new EditablePerson("John", "Adams", GenerateRandomDate()));
                    Add(new EditablePerson("Thomas", "Jefferson", GenerateRandomDate()));
                    Add(new EditablePerson("James", "Madison", GenerateRandomDate()));
                    Add(new EditablePerson("James", "Monroe", GenerateRandomDate()));
                    Add(new EditablePerson("John", "Quincy", "Adams", GenerateRandomDate()));
                    Add(new EditablePerson("Andrew", "Jackson", GenerateRandomDate()));
                    Add(new EditablePerson("Martin", "Van Buren", GenerateRandomDate()));
                    Add(new EditablePerson("William", "H.", "Harrison", GenerateRandomDate()));
                    Add(new EditablePerson("John", "Tyler", GenerateRandomDate()));
                    Add(new EditablePerson("James", "K.", "Polk", GenerateRandomDate()));
                    Add(new EditablePerson("Zachary", "Taylor", GenerateRandomDate()));
                    Add(new EditablePerson("Millard", "Fillmore", GenerateRandomDate()));
                    Add(new EditablePerson("Franklin", "Pierce", GenerateRandomDate()));
                    Add(new EditablePerson("James", "Buchanan", GenerateRandomDate()));
                    Add(new EditablePerson("Abraham", "Lincoln", GenerateRandomDate()));
                    Add(new EditablePerson("Andrew", "Johnson", GenerateRandomDate()));
                    Add(new EditablePerson("Ulysses", "S.", "Grant", GenerateRandomDate()));
                    Add(new EditablePerson("Rutherford", "B.", "Hayes", GenerateRandomDate()));
                    Add(new EditablePerson("James", "Garfield", GenerateRandomDate()));
                    Add(new EditablePerson("Chester", "A.", "Arthur", GenerateRandomDate()));
                    Add(new EditablePerson("Grover", "Cleveland", GenerateRandomDate()));
                    Add(new EditablePerson("Benjamin", "Harrison", GenerateRandomDate()));
                    Add(new EditablePerson("William", "McKinley", GenerateRandomDate()));
                    Add(new EditablePerson("Theodore", "Roosevelt", GenerateRandomDate()));
                    Add(new EditablePerson("William", "H.", "Taft", GenerateRandomDate()));
                    Add(new EditablePerson("Woodrow", "Wilson", GenerateRandomDate()));
                    Add(new EditablePerson("Warren", "G.", "Harding", GenerateRandomDate()));
                    Add(new EditablePerson("Calvin", "Coolidge", GenerateRandomDate()));
                    Add(new EditablePerson("Herbert", "Hoover", GenerateRandomDate()));
                    Add(new EditablePerson("Franklin", "D.", "Roosevelt", GenerateRandomDate()));
                    Add(new EditablePerson("Harry", "S.", "Truman", GenerateRandomDate()));
                    Add(new EditablePerson("Dwight", "D.", "Eisenhower", GenerateRandomDate()));
                    Add(new EditablePerson("John", "F.", "Kennedy", GenerateRandomDate()));
                    Add(new EditablePerson("Lyndon", "B.", "Johnson", GenerateRandomDate()));
                    Add(new EditablePerson("Richard", "Nixon", GenerateRandomDate()));
                    Add(new EditablePerson("Gerald", "Ford", GenerateRandomDate()));
                    Add(new EditablePerson("Jimmy", "Carter", GenerateRandomDate()));
                    Add(new EditablePerson("Ronald", "Reagan", GenerateRandomDate()));
                    Add(new EditablePerson("George", "Bush", GenerateRandomDate()));
                    Add(new EditablePerson("Bill", "Clinton", GenerateRandomDate()));
                    Add(new EditablePerson("George", "W.", "Bush", GenerateRandomDate()));
                }
            }
        }

        private DateTime GenerateRandomDate()
        {
            int randInt = random.Next();
            return new DateTime(Convert.ToInt64(randInt));
        }
    }

    public class Customers : ObservableCollection<Customer>
    {
        public Customers()
        {
            Add(new Customer("George", string.Empty, "Washington", new DateTime(1999, 1, 26)));
            Add(new Customer("John", string.Empty, "Adams", new DateTime(2000, 2, 26)));
            Add(new Customer("Thomas", string.Empty, "Jefferson", new DateTime(2001, 3, 26)));
            Add(new Customer("James", string.Empty, "Madison", new DateTime(2002, 4, 26)));
            Add(new Customer("James", string.Empty, "Monroe", new DateTime(2003, 5, 26)));
            Add(new Customer("John", "Quincy", "Adams", new DateTime(2004, 6, 26)));
            Add(new Customer("Andrew", string.Empty, "Jackson", new DateTime(2005, 7, 26)));
            Add(new Customer("Martin", string.Empty, "Van Buren", new DateTime(2006, 8, 26)));
            Add(new Customer("William", "H.", "Harrison", new DateTime(2007, 9, 26)));
            Add(new Customer("John", string.Empty, "Tyler", new DateTime(2008, 10, 26)));
        }
    }

    /// <summary>
    /// Contract that must be implemented for DataGrid tests when 
    /// using generics for the DataSource
    /// </summary>
    public interface IDataGridDataType : ICloneable
    {
        bool CustomEquals(object obj);
    }

    public class Person : INotifyPropertyChanged, ICloneable, IDataGridDataType
    {
        #region Private Fields

        private static int globalId = 0;   // not thread-safe
        private int id;
        private string firstName;
        private string lastName;
        private string middleName;
        private bool likesCake;
        private string cake = String.Empty;
        private Uri homepage = null;
        private DateTime dob;
        private EyeColor eyeColor = EyeColor.Unknown;

        #endregion Private Fields

        #region Constructors

        public Person()
        {
        }

        public Person(string firstName, string lastName)
            : this(firstName, String.Empty, lastName)
        {
        }

        public Person(string firstName, string lastName, DateTime dob)
            : this(firstName, String.Empty, lastName, dob)
        {
        }

        public Person(string firstName, string middleName, string lastName)
            : this(firstName, middleName, lastName, new DateTime(2008, 10, 26))
        {
        }

        public Person(string firstName, string middleName, string lastName, DateTime dob)
            : this(firstName, middleName, lastName, true, "Chocolate", dob)
        {
        }

        public Person(string firstName, string middleName, string lastName, bool likeCake, string cake, DateTime dob)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            LikesCake = likeCake;
            Cake = cake; 
            DOB = dob;
            Id = globalId++;

            string prefix = firstName.ToLower() + "." + lastName.ToLower();
            Homepage = new Uri("http://" + prefix.Replace(' ', '_') + ".whitehouse.gov/");           
        }

        #endregion Constructors

        #region Public Properties

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string MiddleName
        {
            get { return middleName; }
            set
            {
                middleName = value;
                OnPropertyChanged("MiddleName");
            }
        }

        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        public bool LikesCake
        {
            get { return likesCake; }
            set
            {
                likesCake = value;
                OnPropertyChanged("LikesCake");
            }
        }

        public string Cake
        {
            get { return cake; }
            set
            {
                cake = value;
                OnPropertyChanged("Cake");
            }
        }

        public Uri Homepage
        {
            get
            {
                return homepage;
            }
            set
            {
                homepage = value;
                OnPropertyChanged("Homepage");
            }
        } 
        
        public DateTime DOB 
        {
            get
            {
                return dob;
            }
            set
            {
                dob = value;
                OnPropertyChanged("DOB");
            }
        }

        public EyeColor MyEyeColor
        {
            get
            {
                return eyeColor;
            }
            set
            {
                eyeColor = value;
                OnPropertyChanged("MyEyeColor");
            }
        }

        [PropertyTestExpectedResults(TestId = "VerifyAutoGeneratedColumns", IsReadOnly = true)]
        public Color PreferredColor
        {
            get
            {
                return Colors.RoyalBlue;
            }
        }

        #endregion Public Properties
        
        #region Public Members

        public enum EyeColor
        {
            Unknown,
            Blue,
            Green,
            Brown,
            Other
        };

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public bool CustomEquals(object obj)
        {
            bool retValue = true;

            Person person = obj as Person;
            Assert.AssertTrue("person cannot be null.", person != null);

            if (this.Cake.ToLower() != person.Cake.ToLower())
            {
                TestLog.Current.LogEvidence(string.Format("Person property: Cake, values do not match.  Expected: {0}, Actual: {1}", this.Cake, person.Cake));
                retValue = false;
            }
            else if (this.DOB != person.DOB)
            {
                TestLog.Current.LogEvidence(string.Format("Person property: DOB, values do not match.  Expected: {0}, Actual: {1}", this.DOB, person.DOB));
                retValue = false;
            }
            else if (this.FirstName != null && person.FirstName != null && this.FirstName.ToLower() != person.FirstName.ToLower())
            {
                TestLog.Current.LogEvidence(string.Format("Person property: FirstName, values do not match.  Expected: {0}, Actual: {1}", this.FirstName, person.FirstName));
                retValue = false;
            }
            else if (this.Homepage != person.Homepage)
            {
                TestLog.Current.LogEvidence(string.Format("Person property: Homepage, values do not match.  Expected: {0}, Actual: {1}", this.Homepage, person.Homepage));
                retValue = false;
            }
            else if (this.Id != person.Id)
            {
                TestLog.Current.LogEvidence(string.Format("Person property: Id, values do not match.  Expected: {0}, Actual: {1}", this.Id, person.Id));
                retValue = false;
            }
            else if (this.LastName != null && person.LastName != null && this.LastName.ToLower() != person.LastName.ToLower())
            {
                TestLog.Current.LogEvidence(string.Format("Person property: LastName, values do not match.  Expected: {0}, Actual: {1}", this.LastName, person.LastName));
                retValue = false;
            }
            else if (this.LikesCake != person.LikesCake)
            {
                TestLog.Current.LogEvidence(string.Format("Person property: LikesCake, values do not match.  Expected: {0}, Actual: {1}", this.LikesCake, person.LikesCake));
                retValue = false;
            }
            else if (this.MiddleName != null && person.MiddleName != null && this.MiddleName.ToLower() != person.MiddleName.ToLower())
            {
                TestLog.Current.LogEvidence(string.Format("Person property: MiddleName, values do not match.  Expected: {0}, Actual: {1}", this.MiddleName, person.MiddleName));
                retValue = false;
            }
            else if (this.MyEyeColor != person.MyEyeColor)
            {
                TestLog.Current.LogEvidence(string.Format("Person property: MyEyeColor, values do not match.  Expected: {0}, Actual: {1}", this.MyEyeColor, person.MyEyeColor));
                retValue = false;
            }

            return retValue;
        }

        public void CopyFrom(Person person)
        {
            this.FirstName = person.FirstName;
            this.MiddleName = person.MiddleName;
            this.LastName = person.LastName;
            this.Id = person.Id;
            this.Cake = person.Cake;
            this.Homepage = new Uri(person.Homepage.OriginalString);
            this.LikesCake = person.LikesCake;
            this.DOB = person.DOB;
            this.MyEyeColor = person.MyEyeColor;
        }

        #endregion Public Members

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged        

        #region ICloneable

        public object Clone()
        {
            Person person = (Person)this.MemberwiseClone();

            // need to manually copy Uri
            person.Homepage = new Uri(this.Homepage.OriginalString);

            return person;
        }       

        #endregion ICloneable        
    }

    public class EditablePerson : Person, IEditableObject
    {
        private EditablePerson copy;
        private bool inTransaction = false;

        #region Constructors

        public EditablePerson()
            : base("Enter FirstName", "Enter MiddleName", "Enter LastName")
        {
        }

        public EditablePerson(string firstName, string lastName, DateTime dob) 
            : base(firstName, lastName, dob)
        {
        }

        public EditablePerson(string firstName, string middleName, string lastName, DateTime dob)
            : base(firstName, middleName, lastName, dob)
        {
        }

        #endregion Constructors        

        #region IEditableObject Members

        public void BeginEdit()
        {
            TestLog.Current.LogDebug(string.Format("Item: {0} is up for editing", this.ToString()));

            if (!inTransaction)
            {
                inTransaction = true;
                if (this.copy == null)
                {
                    this.copy = new EditablePerson();
                }

                this.copy = (EditablePerson)Clone();
            }
        }

        public void CancelEdit()
        {
            TestLog.Current.LogDebug(string.Format("Item: {0} has been cancelled for editing", this.ToString()));

            if (inTransaction)
            {
                inTransaction = false;
                this.CopyFrom(this.copy);
            }
        }

        public void EndEdit()
        {
            TestLog.Current.LogDebug(string.Format("Item: {0} has been committed", this.ToString()));

            if (inTransaction)
            {
                inTransaction = false;
                this.copy = null;
            }
        }

        #endregion
    }

    public class Customer : EditablePerson
    {
        public Customer(string firstName, string middleName, string lastName, DateTime dob)
            : base(firstName, middleName, lastName, dob)
        {
            this.customerID = "tempID";
        }

        private string customerID;

        // read-only property
        public string CustomerID
        {
            get
            {
                return customerID;
            }
        }
    }    

    public class CakeChoices : ObservableCollection<CakeType>
    {
        public CakeChoices()
        {
            Add(new CakeType("Apple", 1, "Sweet"));
            Add(new CakeType("Chocolate", 2, "Fatty"));
            Add(new CakeType("Vanilla", 3, "Plain"));            
        }
    }

    public class CakeType
    {
        private string name;
        private int id;
        private string description;

        public CakeType(string name, int id, string desc)
        {
            this.name = name;
            this.id = id;
            this.description = desc;
        }

        public string Cake
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }      
        }

        public override bool Equals(object obj)
        {
            if (obj is CakeType)
            {
                return this.name.Equals(((CakeType)obj).name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class CakeData
    {
        public string Kind { get; set; }
        public string DisplayName { get; set; }
    }
}