// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.Test.DataServices
{

    #region MediaItem

    public class MediaItem : INotifyPropertyChanged, IComparable
    {

        #region Constructors

        public MediaItem()
        {
            _title = "";
            _publisher = "";
            _isbn = "";
            _price = 0.0;
            _releaseDate = new DateTime();
            _isSpecialEdition = true;
        }

        public MediaItem(string title, string publisher, string isbn, double price, bool isSpecialEdition, DateTime releaseDate)
        {
            _title = title;
            _publisher = publisher;
            _isbn = isbn;
            _price = price;
            _isSpecialEdition = isSpecialEdition;
            _releaseDate = releaseDate;
        }

        public MediaItem(double index)
        {
            _title = "Title of " + this.GetType().Name.ToLower() + " " + index;
            _isbn = index.ToString();
            _publisher = "Publisher of " + this.GetType().Name.ToLower() + " " + index;

            int i = (int)index;

            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            
            if (i % 2 == 0)
            {
                _price = 200.0 - index;
                _isSpecialEdition = true;
                _releaseDate = DateTime.Now - ts;
            }
            else
            {
                _price = 200.0 + index;
                _isSpecialEdition = false;
                _releaseDate = DateTime.Now + ts;
            }
        }


        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            MediaItem right = obj as MediaItem;
            if (right == null)
                throw new InvalidCastException("Could not cast " + obj.GetType().FullName + " to type of MediaItem");

            if (Price < right.Price)
                return -1;
            else if (Price > right.Price)
                return 1;
            else
            {
                return Title.CompareTo(right.Title);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Public Properties

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChangedEvent("Title");
            }
        }

        public string ISBN
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                RaisePropertyChangedEvent("ISBN");
            }
        }

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
                RaisePropertyChangedEvent("Publisher");
            }
        }

        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                RaisePropertyChangedEvent("Price");
            }
        }

        public DateTime ReleaseDate
        {
            get { return _releaseDate; }
            set
            {
                _releaseDate = value;
                RaisePropertyChangedEvent("ReleaseDate");
            }
        }

        public bool IsSpecialEdition
        {
            get { return _isSpecialEdition; }
            set
            {
                _isSpecialEdition = value;
                RaisePropertyChangedEvent("IsSpecialEdition");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an XmlNode of the MediaItem. It is not added to the document.
        /// </summary>
        /// <param name="document">Document to create the node from</param>
        /// <returns>XmlNode of the MediaItem</returns>
        public virtual XmlNode ToXml(XmlDocument document)
        {
            XmlElement node = document.CreateElement(this.GetType().Name);

            XmlElement property;

            property = document.CreateElement("Title");
            property.InnerText = this.Title;
            node.AppendChild(property);

            property = document.CreateElement("ISBN");
            property.InnerText = this.ISBN;
            node.AppendChild(property);

            property = document.CreateElement("Publisher");
            property.InnerText = this.Publisher;
            node.AppendChild(property);

            property = document.CreateElement("Price");
            property.InnerText = this.Price.ToString();
            node.AppendChild(property);

            return node;
        }

        public override string ToString()
        {
            return (this.GetType().Name + " " + this.ISBN);
        }

        #endregion

        #region Data Members

        private string _title;

        private string _isbn;

        private string _publisher;

        private double _price;

        private DateTime _releaseDate;

        private bool _isSpecialEdition;

        #endregion

    }

    #endregion

    #region Book

    public class Book : MediaItem
    {

        public Book() { }

        public Book(string index) : this(Double.Parse(index)) { }

        public Book(double index) : base(index)
        {
            _author = "Author of book " + index;

            int i = (int)index;
            string[] names = Enum.GetNames(typeof(BookGenre));
            _genre = (BookGenre)Enum.Parse(typeof(BookGenre), names[(i % names.Length)]);
        }

        public Book(string title, string isbn, string author, string publisher, double price, BookGenre genre) : base(title, isbn, publisher, price, true, DateTime.Now)
        {
            _author = author;
            _genre = genre;
        }

        public Book(string title, string isbn, string author, string publisher, double price, BookGenre genre, bool isSpecialEdition, DateTime releaseDate)
            : base(title, isbn, publisher, price, isSpecialEdition, releaseDate)
        {
            _author = author;
            _genre = genre;
        }

        public enum BookGenre { Mystery, Reference, SciFi, Romance, Comic, SelfHelp };

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                RaisePropertyChangedEvent("Author");
            }
        }

        public BookGenre Genre
        {
            get { return _genre; }
            set
            {
                _genre = value;
                RaisePropertyChangedEvent("Genre");
            }
        }

        private BookGenre _genre;

        private string _author;

        public override XmlNode ToXml(XmlDocument document)
        {
            XmlNode node = base.ToXml(document);

            XmlElement property;

            property = document.CreateElement("Author");
            property.InnerText = this.Author;
            node.AppendChild(property);

            property = document.CreateElement("Genre");
            property.InnerText = this.Genre.ToString();
            node.AppendChild(property);

            return node;
        }

    }

    #endregion

    #region AudioCD

    public class AudioCD : MediaItem
    {


        public AudioCD() { }

        public AudioCD(string index) : this(Double.Parse(index)) { }

        public AudioCD(double index) : base(index)
        {
            _artist = "Artist of book " + index;
        }

        public AudioCD(string title, string isbn, string artist, string publisher, double price)
            : base(title, isbn, publisher, price, true, DateTime.Now)
        {
            _artist = artist;
        }

        public AudioCD(string title, string isbn, string artist, string publisher, double price, bool isSpecialEdition, DateTime releaseDate)
            : base(title, isbn, publisher, price, isSpecialEdition, releaseDate)
        {
            _artist = artist;
        }


        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                RaisePropertyChangedEvent("Artist");
            }
        }

        private string _artist;

        public override XmlNode ToXml(XmlDocument document)
        {
            XmlNode node = base.ToXml(document);

            XmlElement property;

            property = document.CreateElement("Artist");
            property.InnerText = this.Artist;
            node.AppendChild(property);

            return node;
        }
    }

    #endregion

}
