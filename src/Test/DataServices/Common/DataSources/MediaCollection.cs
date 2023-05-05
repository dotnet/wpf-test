// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{

    #region MediaCollection ObservableCollection of AudioCDs, Books, and MediaItems

    public class MediaCollection : ObservableCollection<MediaItem>
    {

        #region Constructors

        public MediaCollection(int totalItems, double revision)
        {
            if (totalItems < 0)
                totalItems = 0;

            int books = 0, audiocds = 0, mediaItems = 0;
            books = totalItems / 3;
            audiocds = totalItems / 3;
            mediaItems = (totalItems - books) - audiocds;

            while (mediaItems > 0 && books > 0 && audiocds > 0)
            {
                Add(new Book(books + revision));
                Add(new AudioCD(audiocds + revision));
                Add(new MediaItem(mediaItems + revision));
                books--;
                audiocds--;
                mediaItems--;
            }

            while (books > 0)
            {
                Add(new Book(books + revision));
                books--;
            }

            while (audiocds > 0)
            {
                Add(new AudioCD(audiocds + revision));
                audiocds--;
            }

            while (mediaItems > 0)
            {
                Add(new MediaItem(mediaItems + revision));
                mediaItems--;
            }
        }

        // Default Constructor
        public MediaCollection() { }

        // Constructor creates specified number of books
        public MediaCollection(string totalItems) : this(Int32.Parse(totalItems), 0.0) { }

        // Constructor creates specified number of books with specified revision
        public MediaCollection(string totalItems, string revision) : this(Int32.Parse(totalItems), Double.Parse(revision)) { }

        #endregion

        #region XmlDocument from items

        /// <summary>
        /// Creates an XmlDocument for all items in the specified range
        /// startAt >= x > endBefore (does not include endBefore)
        /// </summary>
        /// <param name="startAt">First item to create xml</param>
        /// <param name="endBefore">Stop before this index</param>
        /// <returns>XmlDocument of items</returns>
        public XmlDocument ToXmlDocument(int startAt, int endBefore)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<MediaCollection xmlns=\"\"></MediaCollection>");
            XmlNode node;

            for (int i = startAt; i < endBefore; i++)
            {
                node = this[i].ToXml(doc);
                doc.DocumentElement.AppendChild(node);
            }

            return doc;
        }

        /// <summary>
        /// Creates an XmlDocument for all items starting at startAt
        /// </summary>
        /// <param name="startAt">First item to create xml</param>
        /// <returns>XmlDocument of items</returns>
        public XmlDocument ToXmlDocument(int startAt)
        {
            return this.ToXmlDocument(startAt, this.Count);
        }

        /// <summary>
        /// Create an XmlDocument for all items in collection
        /// </summary>
        /// <returns>XmlDocument of items</returns>
        public XmlDocument ToXmlDocument()
        {
            return this.ToXmlDocument(0, this.Count);
        }

        #endregion

        #region Subset of the MediaCollection

        public MediaCollection Subset(int startAt, int endBefore)
        {
            MediaCollection l = new MediaCollection();
            for (int i = startAt; i < endBefore; i++)
            {
                l.Add(this[i]);
            }
            return l;
        }

        public MediaCollection Subset(int startAt)
        {
            return this.Subset(startAt, this.Count);
        }

        public MediaCollection Subset()
        {
            return this;
        }

        #endregion

    }

    #endregion

}
