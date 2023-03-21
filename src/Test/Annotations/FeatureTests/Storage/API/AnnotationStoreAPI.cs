// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Abstract base class for AnnotationStore tests.  Contains
//				 context specific helper methods.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Avalon.Test.Annotations.Storage;
using Annotations.Test.Framework;
using System.Reflection;
using Annotations.Test.Reflection;
using System.Text;
using System.Data.SqlClient;

namespace Avalon.Test.Annotations
{
	public abstract class AnnotationStoreAPI : TestSuite
    {

        #region Overrides

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);
            _storeWrapper = CreateStoreWrapper(args);
        }

        #endregion

        #region Protected Methods

        protected AnnotationStore CreateEmptyStore()
        {
            _storeWrapper.Reset();
            return CreateStore();
        }

        protected AnnotationStore CreateStore()
        {            
            _storeWrapper.Create();
            return _storeWrapper.Store;
        }

        protected void CloseStore()
        {
            _storeWrapper.Close();
        }

        /// <summary>
		/// Verify that the given annotations are to ONLY annotations in this store.
		/// </summary>
		protected void verifyStoreContents(AnnotationStore store, Annotation[] expectedAnnos)
		{
			IList<Annotation> annotations = store.GetAnnotations();

			AssertEquals("Check number of annotations.", expectedAnnos.Length, annotations.Count);
			for (int i = 0; i < expectedAnnos.Length; i++)
				Assert("Verifying existance of expected annotation " + i + ".", store.GetAnnotation(expectedAnnos[i].Id) != null);
		}

        /// <summary>
        /// Verify the contents of a CLOSED store.
        /// </summary>
        protected void verifyStoreContents(Annotation[] expectedAnnos)
        {
            AnnotationStore store = CreateStore();
            verifyStoreContents(store, expectedAnnos);
            CloseStore();
        }

		/// <summary>
		/// Very simple comparison.  Check that their ids are equal and that they have the
		/// same number of anchors, authors, and cargos.
		/// </summary>
		protected void weakCompareAnnotations(Annotation a1, Annotation a2)
		{
			AssertEquals("Comparing annotations: Verify annotation ids.", a1.Id, a2.Id);
			AssertEquals("Comparing annotations: Verify number of anchors.", a1.Anchors.Count, a2.Anchors.Count);
			AssertEquals("Comparing annotations: Verify number of cargos.", a1.Cargos.Count, a2.Cargos.Count);
			AssertEquals("Comparing annotations: Verify number of authors.", a1.Authors.Count, a2.Authors.Count);
        }

        /// <summary>
        /// Make an annotation file that has some annotations in it
        /// </summary>
        /// <param name="storeURI"></param>
        protected void SetupStoreWithContent()
        {
            _storeWrapper.Reset();

            AnnotationStore tempannotationStore = CreateStore();

            Annotation an1 = AnnotationStoreTestHelpers.MakeAnnotation1();
            tempannotationStore.AddAnnotation(an1);
            Annotation an2 = AnnotationStoreTestHelpers.MakeAnnotation2();
            tempannotationStore.AddAnnotation(an2);
            Annotation an3 = AnnotationStoreTestHelpers.MakeAnnotation1();
            tempannotationStore.AddAnnotation(an3);

            tempannotationStore.Flush();

            CloseStore();
        }

        protected bool StoreHasContent()
        {
            return _storeWrapper.HasContent();
        }

        #endregion

        #region Private Methods

        private StoreWrapper CreateStoreWrapper(string[] args)
        {
            StoreWrapper storeWrapper = null;
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case XmlStreamStoreOption:
                        storeWrapper = new XmlStreamStoreWrapper();
                        break;
                    case SqlStoreOption:
                        storeWrapper = new SqlStoreWrapper();
                        break;
                    default:
                        failTest("Undefined STORAGE type defined.");
                        break;
                }
            }
            return storeWrapper;
        }

        #endregion

        #region Public Fields

        // Contents of an empty Annotations file that has never had annotations in it
        public const string EMPTYFILE = "<?xml version=\"1.0\" encoding=\"utf-8\"?><annotations xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/caf2\" />";
        // Contents of an empty Annotations file that HAS had annotations in it, then deleted
        // public const string EMPTYFILEafter = "<?xml version=\"1.0\" encoding=\"utf-8\"?><annotations xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/caf2\"></annotations>";
        public const string EMPTYFILEafter = "<?xml version=\"1.0\" encoding=\"utf-8\"?><anc:Annotations xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/windows/annotations/2003/11/core\" xmlns:anc=\"http://schemas.microsoft.com/windows/annotations/2003/11/core\"  xmlns:anb=\"http://schemas.microsoft.com/windows/annotations/2003/11/base\"></anc:Annotations>";

        // Size of an empty annotations file including a couple of line feeds and the BOM
        public int EMPTYFILESIZE = EMPTYFILE.Length + 10;
        // Size of an empty annotations file that has had annotations deleted including a couple of line feeds and the BOM
        public int EMPTYFILESIZEafter = EMPTYFILEafter.Length + 10;

        #endregion

        #region Private Fields

        const string XmlStreamStoreOption = "-xmlstream";
        const string SqlStoreOption = "-sql";
        StoreWrapper _storeWrapper;

        #endregion 

        /// <summary>
		/// Class that registers for events on an AnnotationStore, counts
		/// the events that it receives and provides an interface for comparing
		/// these counts against expected values.
		/// </summary>
		public class StoreEventCounter
		{
			TestSuite m_parent;
			AnnotationStore m_store;

			int m_nAddEvents = 0;
			int m_nDeleteEvents = 0;

			Annotation m_lastAdded = null;
			Annotation m_lastDeleted = null;

			public StoreEventCounter(TestSuite parent, AnnotationStore store)
			{
				m_parent = parent;
				m_store = store;

				m_store.StoreContentChanged += new StoreContentChangedEventHandler(contentEventHandler);
			}

			protected void contentEventHandler(object sender, StoreContentChangedEventArgs e)
			{
				if (e.Action == StoreContentAction.Added)
				{
					m_nAddEvents++;
					m_lastAdded = e.Annotation;
				}

				if (e.Action == StoreContentAction.Deleted)
				{
					m_nDeleteEvents++;
					m_lastDeleted = e.Annotation;
				}
			}

			/// <summary>
			/// Verify that the expected number of content change events occurred.
			/// </summary>
			public void verifyContentEventCounts(int nAddEvents, int nDeleteEvents)
			{
				m_parent.AssertEquals("Check number of 'add' events.", nAddEvents, m_nAddEvents);
				m_parent.AssertEquals("Check number of 'delete' events.", nDeleteEvents, m_nDeleteEvents);
			}

			/// <summary>
			/// Check that the Annotation contained in the last Add event is
			/// the same Annotation object that was added.
			/// </summary>
			public void verifyLastAddedAnnotation(Annotation anno)
			{
				m_parent.Assert("Verify last add event's StoreContentChangedEventArgs.Annotation reference.", anno == m_lastAdded);
			}

			/// <summary>
			/// Check that the Annotation contained in the last Delete event is
			/// the same Annotation object that was deleted.
			/// </summary>
			public void verifyLastDeleteAnnotation(Annotation anno)
			{
				m_parent.Assert("Verify last delete event's StoreContentChangedEventArgs.Annotation reference.", anno == m_lastDeleted);
			}
		}
	}

    public abstract class StoreWrapper
    {
        public StoreWrapper()
        {
            Reset();
        }

        public abstract void Reset();
        public abstract void Create();
        public abstract void Close();
        public abstract bool HasContent();

        public AnnotationStore Store;
    }

    public class XmlStreamStoreWrapper : StoreWrapper
    {
        public override void Reset()
        {
            if (File.Exists(StoreUri))
                File.Delete(StoreUri);
        }

        public override void Create()
        {
            Stream = new FileStream(StoreUri, FileMode.OpenOrCreate);
            Store = XmlStreamStoreWrapper.Create(Stream);
        }

        public override void Close()
        {
            Stream.Close();
        }

        public override bool HasContent()
        {
            return new FileInfo(StoreUri).Length > 0;
        }

        public static AnnotationStore Create(Stream stream)
        {
            
            // We need to register all the test namespaces, otherwise the XCR parser will throw.
            IDictionary<Uri, IList<Uri>> testNamespaces = new Dictionary<Uri, IList<Uri>>();
            foreach (string ns in AnnotationStoreTestHelpers.ValidNamespaces) 
                testNamespaces.Add(new Uri(ns, UriKind.RelativeOrAbsolute), null);
            return new XmlStreamStore(stream, testNamespaces);
        }

        public const string StoreUri = "storetest.xml";
        Stream Stream;        
    }

    public class SqlStoreWrapper : StoreWrapper
    {
        public override void Reset()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            ClearTable(TablePrefix + "_Definitions", connection);
            ClearTable(TablePrefix + "_ContentLocatorParts", connection);
            connection.Close();
        }

        public override void Create()
        {           
            _datasource = new SqlAnnotationDataSource(ConnectionString, TablePrefix);
            Store = new CachedAnnotationStore(_datasource);
        }

        public override void Close()
        {
            _datasource.Close();
        }

        private void ClearTable(string tablename, SqlConnection connection)
        {
            try
            {
                new SqlCommand("DELETE FROM " + tablename, connection).ExecuteNonQuery();
            }
            //catch (Exception e)
            catch (Exception)
            {
                // Ignore.
            }
        }

        public override bool HasContent()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            string tablename = TablePrefix + "_Definitions";
            bool result = false;
            if (TableExists(tablename, connection))
                result = new SqlCommand("SELECT Id FROM " + tablename + ";", connection).ExecuteReader().HasRows;
            connection.Close();
            return result;
        }
        
        private bool TableExists(string tablename, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand("IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE ='BASE TABLE' AND TABLE_NAME='" + tablename + "') SELECT 'TRUE' ELSE SELECT 'FALSE'", connection);
            SqlDataReader results = command.ExecuteReader();
            try
            {
                if (results.HasRows)
                {
                    if (results.Read())
                    {
                        return results.GetString(0).Equals("TRUE");
                    }
                }
            }
            finally
            {
                results.Close();
            }
            return false;
        }

        protected string ConnectionString
        {
            get
            {
                return new StringBuilder().AppendFormat(DatabaseIdentifierTemplate, new object[] { ServerName, DatabaseName }).ToString();
            }
        }

        protected string TablePrefix
        {
            get
            {
                return Environment.UserName + "_StorageTests";
            }
        }

        private const string ServerName = "AV-YUKON";
        private const string DatabaseName = "AvalonAppTest";
        public const string DatabaseIdentifierTemplate = "Data Source={0};Initial Catalog={1};Integrated Security=True";
       
        private IAnnotationDataSource _datasource;
    }
}

