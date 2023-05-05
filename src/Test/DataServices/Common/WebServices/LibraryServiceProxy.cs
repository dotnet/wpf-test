// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Test.DataServices;

namespace Microsoft.Test.DataServices
{

    #region Xml Books as a list proxy

    public class XmlBookListProxy
	{
        protected XmlDocument doc;

        public XmlBookListProxy()
        {
            Library l = new Library(10, 0.0);
            doc = l.ToXmlDocument();
        }

        public XmlNodeList GetBooksA()
        {
            return doc.SelectNodes("/Library/Book");
        }

        public XmlNodeList GetBooksA(int startAt)
        {
            return doc.SelectNodes("/Library/Book[position()>=" + (startAt + 1) + "]");
        }

        public XmlNodeList GetBooksA(int startAt, int endBefore)
        {
            return doc.SelectNodes("/Library/Book[(position()>=" + (startAt + 1) + ") and (position()<=" + (endBefore + 1) + ")]");
        }

        public XmlNodeList GetBooksB()
        {
            return doc.SelectNodes("/Library/Book");
        }

        public XmlNodeList GetBooksB(int startAt)
        {
            return doc.SelectNodes("/Library/Book[position()>=" + (startAt + 1) + "]");
        }

        public XmlNodeList GetBooksB(int startAt, int endBefore)
        {
            return doc.SelectNodes("/Library/Book[(position()>=" + (startAt + 1) + ") and (position()<=" + (endBefore + 1) + ")]");
        }
	}

    public class XmlBookListAProxy : XmlBookListProxy 
    {
        public XmlBookListAProxy()
        {
            Library l = new Library(10, 0.1);
            doc = l.ToXmlDocument();
        }
    }
    public class XmlBookListBProxy : XmlBookListProxy
    {
        public XmlBookListBProxy()
        {
            Library l = new Library(10, 0.2);
            doc = l.ToXmlDocument();
        }
    }

    #endregion

    #region Clr Books as a list proxy

    public class ClrBookListProxy
    {
        protected Library library;

        public ClrBookListProxy()
        {
            library = new Library(10, 0.0);
        }

        public Library GetBooksA()
        {
            return library;
        }

        public Library GetBooksA(int startAt)
        {
            return library.Subset(startAt);
        }

        public Library GetBooksA(int startAt, int endBefore)
        {
            return library.Subset(startAt,endBefore);
        }

        public Library GetBooksB()
        {
            return library;
        }

        public Library GetBooksB(int startAt)
        {
            return library.Subset(startAt);
        }

        public Library GetBooksB(int startAt, int endBefore)
        {
            return library.Subset(startAt, endBefore);
        }
    }

    public class ClrBookListAProxy : ClrBookListProxy
    {
        public ClrBookListAProxy()
        {
            library = new Library(10, 0.1);
        }
    }

    public class ClrBookListBProxy : ClrBookListProxy
    {
        public ClrBookListBProxy()
        {
            library = new Library(10, 0.2);
        }
    }

    #endregion

    #region Xml Book proxy

    public class XmlBookProxy
    {
        protected double revision;

        public XmlBookProxy()
        {
            revision = 0.0;
        }

        public XmlNode GetBookA()
        {
            Book book = new Book(revision);
            XmlDocument doc = new XmlDocument();
            return book.ToXml(doc);
        }

        public XmlNode GetBookA(int a)
        {
            Book book = new Book(a + revision);
            XmlDocument doc = new XmlDocument();
            return book.ToXml(doc);
        }

        public XmlNode GetBookA(int a, int b)
        {
            Book book = new Book(a + b + revision);
            XmlDocument doc = new XmlDocument();
            return book.ToXml(doc);
        }

        public XmlNode GetBookB()
        {
            Book book = new Book(revision);
            XmlDocument doc = new XmlDocument();
            return book.ToXml(doc);
        }

        public XmlNode GetBookB(int a)
        {
            Book book = new Book(a + revision);
            XmlDocument doc = new XmlDocument();
            return book.ToXml(doc);
        }

        public XmlNode GetBookB(int a, int b)
        {
            Book book = new Book(a + b + revision);
            XmlDocument doc = new XmlDocument();
            return book.ToXml(doc);
        }
    }

    public class XmlBookAProxy : XmlBookProxy
    {
        public XmlBookAProxy()
        {
            revision = 0.1;
        }
    }

    public class XmlBookBProxy : XmlBookProxy
    {
        public XmlBookBProxy()
        {
            revision = 0.2;
        }
    }

    #endregion

    #region Clr Book proxy

    public class ClrBookProxy
    {
        protected double revision;

        public ClrBookProxy()
        {
            revision = 0.0;
        }

        public Book GetBookA()
        {
            return new Book(revision);
        }

        public Book GetBookA(int a)
        {
            return new Book(a + revision);
        }

        public Book GetBookA(int a, int b)
        {
            return new Book(a + b + revision);
        }

        public Book GetBookB()
        {
            return new Book(revision);
        }

        public Book GetBookB(int a)
        {
            return new Book(a + revision);
        }

        public Book GetBookB(int a, int b)
        {
            return new Book(a + b + revision);
        }
    }

    public class ClrBookAProxy : ClrBookProxy
    {
        public ClrBookAProxy()
        {
            revision = 0.1;
        }
    }

    public class ClrBookBProxy : ClrBookProxy
    {
        public ClrBookBProxy()
        {
            revision = 0.1;
        }
    }

    #endregion

}
