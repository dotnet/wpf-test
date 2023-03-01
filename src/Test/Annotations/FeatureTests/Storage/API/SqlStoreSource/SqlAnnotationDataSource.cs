// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.IO;
using System.Windows.Annotations;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.ObjectModel;

namespace Avalon.Test.Annotations.Storage
{
    /// <summary>
    /// Module that uses a SQL database to store and retrieve Annotations.
    /// </summary>
    class SqlAnnotationDataSource : IAnnotationDataSource
    {       
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Create and initialize an AnnotationDataSource which uses a SQL database to
        /// persist annotations.
        /// </summary>
        /// <remarks>
        /// -The database connection used by this object is opened at the time of creation
        /// and is not closed until it is closed.
        /// -For each unique tablePrefix a new set of tables are created.
        /// </remarks>
        /// <param name="databaseIdentifier">Connect string for database to use.</param>
        /// <param name="tablePrefix">Unique identifier to differentiate which documents an
        /// annotation belongs to.</param>
        public SqlAnnotationDataSource(string databaseIdentifier, string tablePrefix)
        {
            if (string.IsNullOrEmpty(tablePrefix))
                throw new ArgumentException("TablePrefix must be non-empty.");

            // Save table names.
            DefinitionsTablename = tablePrefix + DefinitionsTableSuffix;
            ContentLocatorPartsTablename = tablePrefix + ContentLocatorPartsTableSuffix;

            _databaseIdentifier = databaseIdentifier;
            
            // Verify that database exists and we can connect to it.
            if (string.IsNullOrEmpty(databaseIdentifier))
                throw new ArgumentException("'databaseIdentifier' must reference a valid Database.");
            
            try 
            {
                // Open the connection once.
                OpenConnection();
                CreateTables();
            }
            catch (Exception e){ 
                throw new ArgumentException("Could not connect to database '" + databaseIdentifier + "': " + e.Message);
            }            
        }

        #endregion

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Serialize the given annotation into a database.
        /// </summary>
        public void AddAnnotation(Annotation annotation)
        {
            EnsureDatabaseConnection();
            ExecuteUpdate("INSERT INTO " + DefinitionsTablename + " (" + ID + ", " + DEFINITION + ") VALUES('" + annotation.Id + "','" + SerializeAnnotation(annotation) + "')");
            InsertContentLocatorParts(annotation);            
        }

        /// <summary>
        /// Remove annotation from database.
        /// </summary>
        /// <param name="annotationId">Id of the annotation to remove.</param>
        public void DeleteAnnotation(Guid id)
        {
            EnsureDatabaseConnection();
            ExecuteUpdate("DELETE FROM " + DefinitionsTablename + " WHERE " + ID + " = '" + id + "'");
            RemoveAllPartsForAnnotation(id);
        }      
   
        /// <summary>
        /// Update the database entry for the given annotation.
        /// </summary>
        /// <param name="annotation">Annotation to update database entries for.</param>
        public void UpdateAnnotation(Annotation annotation)
        {
            EnsureDatabaseConnection();
            ExecuteUpdate("UPDATE " + DefinitionsTablename + " SET " + DEFINITION + " = '" + SerializeAnnotation(annotation) + "' WHERE " + ID + " = '" + annotation.Id + "'");
            RemoveAllPartsForAnnotation(annotation.Id);
            InsertContentLocatorParts(annotation);
        }

        /// <summary>
        /// Returns whether or not data source contains an annotation.
        /// </summary>
        /// <param name="id">Id of annotation to look for.</param>
        /// <returns>True if data source contains annotation with given id.</returns>
        public bool ContainsAnnotation(Guid id)
        {
            EnsureDatabaseConnection();
            return (ExecuteQuery("SELECT " + ID + " FROM " + DefinitionsTablename + " WHERE " + ID + " = '" + id + "'").Count > 0);
        }

        /// <summary>
        /// Deserialize annotation with the given id from the database.
        /// </summary>
        /// <param name="id">Id of annotation to read from database.</param>
        /// <returns>Annotation with given id or null if none exists.</returns>
        public Annotation GetAnnotation(Guid id)
        {
            EnsureDatabaseConnection();
            IList<string> result = ExecuteQuery("SELECT Definition FROM " + DefinitionsTablename + " WHERE " + ID + " = '" + id + "'");
            if (result.Count == 1)
                return DeserializeAnnotation(result[0]);
            return null;
        }

        /// <summary>
        /// Returns all annotations from the database.
        /// </summary>
        /// <returns>string list of all "text blobs"</returns>
        public IList<Guid> GetAnnotations()
        {
            EnsureDatabaseConnection();
            return ExecuteGuidQuery("SELECT " + ID + " FROM " + DefinitionsTablename + ";");
        }

        /// <summary>
        /// Get all annotations whose ContentLocators are the same or a superset of the
        /// LocatorParts of the given locator.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>All annotations whose Anchor starts with the given locator.</returns>
        public IList<Guid> GetAnnotations(ContentLocator locator)
        {
            // Create query fragments that select rows that match each individual
            // ContentLocatorPart in the given ContentLocator.
            IList<string> fragments = new List<string>();            
            for (int i=0; i < locator.Parts.Count; i++)
                fragments.Add(CreateQueryFragment(locator.Parts[i], i));

            string query = CombineQueryFragments(fragments, "PartFrag");
            return ExecuteGuidQuery(query);
        }

        /// <summary>
        /// Apply the current transaction to the database.
        /// </summary>
        public void Flush()
        {
            CurrentTransaction.Commit();
            _transaction = null;
        }

        /// <summary>
        /// Close the database connection and reset the transaction.
        /// </summary>
        public void Close()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }
            CloseConnection();            
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        // Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Serializes an Annotation into an xml blob.
        /// </summary>
        /// <param name="annotation">Annotation to serialize.</param>
        /// <returns>Xml blob that represents the given annotation.</returns>
        private string SerializeAnnotation(Annotation annotation)
        {
            StringWriter stringWriter = new StringWriter();
            XmlWriterSettings xmlWriterSetts = new XmlWriterSettings();
            xmlWriterSetts.Indent = false;
            xmlWriterSetts.NewLineHandling = NewLineHandling.None;
            xmlWriterSetts.NewLineOnAttributes = false;

            XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSetts);

            //Create serializer for Annotation Type
            XmlSerializer serializer = new XmlSerializer(typeof(Annotation));
            //Serialize the annotation
            serializer.Serialize(xmlWriter, annotation);

            return stringWriter.ToString();
        }

        /// <summary>
        /// Deserializes an annotation from xml blob to an Annotation instance.
        /// </summary>
        /// <param name="textBlob">Xml representation on an annotation to be deserialized.</param>
        /// <returns>Annotation instance respresented by the given xml blob.</returns>
        private Annotation DeserializeAnnotation(string xmlBlob)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Annotation));
            StringReader reader = new StringReader(xmlBlob);

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            readerSettings.IgnoreComments = true;
            readerSettings.IgnoreProcessingInstructions = true;

            XmlReader xmlReader = XmlReader.Create(reader, readerSettings);

            return (Annotation)serializer.Deserialize(xmlReader);
        }

        /// <summary>
        /// Open connection to the Database.
        /// </summary>
        private void OpenConnection()
        {
            _sqlConnection = new SqlConnection(_databaseIdentifier);
            _sqlConnection.Open();
        }

        /// <summary>
        /// Close connection to the DB.
        /// </summary>
        private void CloseConnection()
        {
            if (_sqlConnection != null)
                _sqlConnection.Close();
        }

        /// <summary>
        /// Create tables in the database.
        /// </summary>
        private void CreateTables()
        {
            if (!DoesTableExists(DefinitionsTablename))
                ExecuteUpdate("CREATE TABLE " + DefinitionsTablename + " (" + ID + " varchar(50), " + DEFINITION + " text)");
            if (!DoesTableExists(ContentLocatorPartsTablename))
                ExecuteUpdate("CREATE TABLE " + ContentLocatorPartsTablename + "(" + ID + " varchar(50), " + LOCATOR_INDEX + " int, " + PART_INDEX + " int, " + TYPE + " varchar(50), " + NAME + " varchar(50), " + VALUE + " varchar(50))");
        }

        /// <summary>
        /// Determine is a table with the given name already exists.
        /// </summary>
        /// <returns>True if Database contains table with given name.</returns>
        private bool DoesTableExists(string tablename)
        {
            IList<string> results = ExecuteQuery("IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE ='BASE TABLE' AND TABLE_NAME='" + tablename + "') SELECT 'TRUE' ELSE SELECT 'FALSE'");
            return (results.Count > 0 && results[0].Equals("TRUE"));
        }

        /// <summary>
        /// Ensure that the database connection is still open.  If not throw a meaningful exception.
        /// </summary>
        private void EnsureDatabaseConnection()
        {
            if (_sqlConnection == null)
                throw new InvalidOperationException("Database Connection was never established.");
            if (_sqlConnection.State == ConnectionState.Broken)
                throw new InvalidProgramException("Database Connection was broken.");
            if (_sqlConnection.State == ConnectionState.Closed)
                throw new InvalidProgramException("Database Connection is closed.");
        }

        /// <summary>
        /// Execute an UPDATE query on the database.
        /// </summary>
        private void ExecuteUpdate(string query)
        {
            SqlCommand cmd = new SqlCommand(query, _sqlConnection, CurrentTransaction);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a SELECT query that will return a list of annotation Guids.
        /// </summary>
        /// <returns>List of annotation Guids.</returns>
        private IList<Guid> ExecuteGuidQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query, _sqlConnection, CurrentTransaction);

            IList<Guid> results = new List<Guid>();
            SqlDataReader dataReader = cmd.ExecuteReader();
            try
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        string idAsString = dataReader.GetValue(0).ToString();
                        results.Add(new Guid(idAsString));
                    }
                }
            }
            finally
            {
                dataReader.Close();
            }

            return results;
        }

        /// <summary>
        /// Execute a SELECT query.
        /// </summary>
        /// <returns>List of results as strings.</returns>
        private IList<string> ExecuteQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query, _sqlConnection, CurrentTransaction);

            IList<string> results = new List<string>();
            SqlDataReader dataReader = cmd.ExecuteReader();
            try
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        results.Add(dataReader.GetValue(0).ToString());
                    }
                }
            }
            finally
            {
                dataReader.Close();
            }

            return results;
        }
 
        /// <summary>
        /// Remove all rows for the given Id from ContentLocatorPartsTablename.
        /// </summary>
        /// <param name="id"></param>
        private void RemoveAllPartsForAnnotation(Guid id)
        {
            ExecuteUpdate("DELETE FROM " + ContentLocatorPartsTablename + " WHERE Id = '" + id + "'");
        }

        /// <summary>
        /// Insert one row per ContentLocatorPart's NameValuePair into ContentLocatorPartsTablename.
        /// </summary>
        /// <param name="annotation"></param>
        private void InsertContentLocatorParts(Annotation annotation)
        {
            if (annotation.Anchors.Count > 1)
                throw new NotSupportedException("Store does not support annotations with multiple anchors.");
            foreach (AnnotationResource resource in annotation.Anchors)
            {
                int locatorIdx = 0;
                foreach (ContentLocatorBase locator in resource.ContentLocators)
                {
                    if (locator is ContentLocator)
                    {
                        InsertContentLocator(annotation.Id, (ContentLocator)locator, locatorIdx++);
                    }
                    else if (locator is ContentLocatorGroup)
                    {
                        ContentLocatorGroup locatorGroup = locator as ContentLocatorGroup;
                        foreach (ContentLocator groupLocator in locatorGroup.Locators)
                            InsertContentLocator(annotation.Id, groupLocator, locatorIdx++);
                    }
                    else
                        throw new NotSupportedException("Store does not support locators of type '" + locator.GetType().FullName + "'.");
                }
            }
        }

        private void InsertContentLocator(Guid id, ContentLocator locator, int locatorIdx)
        {
            int partIdx = 0;
            foreach (ContentLocatorPart part in ((ContentLocator)locator).Parts)
            {
                // Insert one row per ContentLocatorPart's NameValuePair.
                // NameValuePairs are not ordered.
                //                
                foreach (KeyValuePair<string, string> nameValuePairs in part.NameValuePairs)
                {
                    ExecuteUpdate("INSERT INTO " + ContentLocatorPartsTablename + " (" + ID + ", " + LOCATOR_INDEX + ", " + PART_INDEX + ", " + TYPE + ", " + NAME + ", " + VALUE + ") VALUES('" + id + "','" + locatorIdx + "','" + partIdx + "','" + part.PartType.Name + "','" + nameValuePairs.Key + "','" + nameValuePairs.Value + "')");                    
                }
                partIdx++;
            }
        }

        /// <summary>
        /// Create a query that will select only annotations that contain the given ContentLocatorPart at the 
        /// given index.
        /// </summary>
        /// <param name="locatorPart">ContentLocatorPart to create query to match.</param>
        /// <param name="partIdx">ContentLocatorParts are ordered, this is the index of the part to select on.</param>
        /// <returns>Query to select all annotations that have the given ContentLocatorPart at the correct index.</returns>
        private string CreateQueryFragment(ContentLocatorPart locatorPart, int partIdx)
        {
            string fragment = string.Empty;
            switch (locatorPart.PartType.Name)
            {
                case "CharacterRange":
                    fragment = CharacterRangeFragment(locatorPart, partIdx);
                    break;
                default:
                    fragment = FragmentForExactMatch(locatorPart, partIdx);
                    break;
            }
            return fragment;
        }

        /// <summary>
        /// Queries for LocatorParts of type 'CharacterRange' are unique because if contains a
        /// NameValuePair <'IncludeOverlaps', 'true'> then we need to select all annotations that
        /// overlap the given characeter range in any way.
        /// </summary>
        /// <param name="locatorPart">LocatorPart of type 'CharacterRange' to generate query for.</param>
        /// <param name="partIdx">Index of LocatorPart.</param>
        /// <returns>Query to select all annotations that match this LocatorPart and index.</returns>
        private string CharacterRangeFragment(ContentLocatorPart locatorPart, int partIdx)
        {            
            bool includeOverlaps = false;
            if (locatorPart.NameValuePairs.ContainsKey("IncludeOverlaps"))
                includeOverlaps = Convert.ToBoolean(locatorPart.NameValuePairs["IncludeOverlaps"]);
            
            // If 'IncludOverlaps' == true, then we need to perform a special query which will select
            // all annotations that have anchors that overlap the specified CharacterRange.
            //
            // Query: Join 'Length' and 'Offset' rows into a temporary table containing
            // the start and end of the range.  Then select all distinct Id's whose start or end
            // points fall within this range.
            //
            if (includeOverlaps)
            {
                int start = int.Parse(locatorPart.NameValuePairs["Offset"]);
                int end = int.Parse(locatorPart.NameValuePairs["Length"]) + start;

                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT " + ID + ", " + LOCATOR_INDEX);
                query.AppendLine("FROM (");
                query.AppendLine("\t" + "SELECT a." + ID + ", a." + LOCATOR_INDEX + ", a." + PART_INDEX + ", a." + VALUE + " AS RangeStart, CONVERT(int, a." + VALUE + ") + CONVERT(int, b." + VALUE + ") AS RangeEnd");
                query.AppendLine("\t" + "FROM (");
                query.AppendLine("\t\t" + "SELECT *");
                query.AppendLine("\t\t" + "FROM " + ContentLocatorPartsTablename + " WHERE (Type = 'CharacterRange') AND (Name = 'Offset')");
                query.AppendLine("\t" + ") AS a INNER JOIN (");
                query.AppendLine("\t\t" + "SELECT * ");
                query.AppendLine("\t\t" + "FROM " + ContentLocatorPartsTablename + " WHERE (Type = 'CharacterRange') AND (Name = 'Length')");
                query.AppendLine("\t" + ") AS b ON (a." + ID + " = b." + ID + ") AND (a. " + LOCATOR_INDEX + " = b." + LOCATOR_INDEX + ") AND (a." + PART_INDEX + " = b." + PART_INDEX + ") ) AS derivedtbl_1");
                query.AppendLine("WHERE (RangeStart >= " + start + ") AND (RangeStart <= " + end + ") OR (RangeEnd >= " + start + ") AND (RangeEnd <= " + end + ")");
                return query.ToString();
            }
            // Otherwise, if 'IncludeOverlaps' == false, perform a normal query.
            //
            else
            {
                return FragmentForExactMatch(locatorPart, partIdx);
            }
        }

        /// <summary>
        /// Create a query that will select annotations that match the given ContentLocatorPart and index exactly.
        /// </summary>
        /// <param name="locatorPart">ContentLocatorPart to query on.</param>
        /// <param name="partIdx">Indexs of ContentLocatorPart to queyr on.</param>
        /// <returns>Query to select all annotations that match the parameters exactly.</returns>
        private string FragmentForExactMatch(ContentLocatorPart locatorPart, int partIdx)
        {
            string fragmentPrefix = "KeyValueFrag";
            IList<string> fragments = new List<string>();

            // Create one query per NameValuePair.
            foreach (KeyValuePair<string, string> pair in locatorPart.NameValuePairs)
            {
                StringBuilder selectStatment = new StringBuilder();
                selectStatment.AppendLine("SELECT " + ID + ", " + LOCATOR_INDEX);
                selectStatment.AppendLine("FROM " + ContentLocatorPartsTablename);
                selectStatment.AppendLine("WHERE");
                selectStatment.AppendLine("\t(" + TYPE + "= '" + locatorPart.PartType.Name + "')");
                selectStatment.AppendLine("\tAND");
                selectStatment.AppendLine("\t(" + PART_INDEX + "='" + partIdx + "')");
                selectStatment.AppendLine("\tAND");
                selectStatment.AppendLine("\t(" + NAME + "='" + pair.Key + "')");
                selectStatment.AppendLine("\tAND");
                selectStatment.AppendLine("\t(" + VALUE + "='" + pair.Value + "')");

                fragments.Add(selectStatment.ToString());
            }

            // Combine all of the NameValuePair query fragments into a single query.
            return CombineQueryFragments(fragments, fragmentPrefix);
        }

        /// <summary>
        /// Given a list of query fragments, chains them together using INNER JOINs to 
        /// make a single query which will return the intersection of all the smaller queries.
        /// </summary>
        /// <param name="fragments">List of queries to combine into a single query.</param>
        /// <param name="fragmentPrefix">Prefix for the names of the temporary tables that will be
        /// used to perform the INNER JOINs.</param>
        /// <returns>Single query that is the intersection of all the query fragments.</returns>
        private string CombineQueryFragments(IList<string> fragments, string fragmentPrefix)
        {
            // If there is only 1 fragment then there is no merging to do.
            if (fragments.Count == 1)
                return fragments[0];

            // Combine all the query fragments into a single query.
            String query = fragments[0];
            for (int i = 1; i < fragments.Count; i++)
            {
                string previousFragmentName = fragmentPrefix + (i - 1);
                string currentFragmentName = fragmentPrefix + i;

                string previousQuery = "(" + query + ") AS " + previousFragmentName;
                string currentQuery = "(" + fragments[i] + ") AS " + currentFragmentName;

                // Build up a full query by performing INNER JOINS on the current
                // query and the next fragment.  Make sure that we only query against
                // parts that are in the same locator as one another.
                query = "SELECT DISTINCT " + currentFragmentName + "." + ID + ", " + currentFragmentName + "." + LOCATOR_INDEX + " FROM " + "\n"
                    + previousQuery + "\n"
                    + " INNER JOIN " + "\n"
                    + currentQuery + "\n"
                    + " ON (" + currentFragmentName + "." + ID + " = " + previousFragmentName + "." + ID + ") AND (" + currentFragmentName + "." + LOCATOR_INDEX + " = " + previousFragmentName + "." + LOCATOR_INDEX + ")";
            }
            return query;
        }

        /// <summary>
        /// Return the current transaction that is being processed.
        /// </summary>
        private SqlTransaction CurrentTransaction
        {
            get
            {
                if (_transaction == null)
                    _transaction = _sqlConnection.BeginTransaction();
                return _transaction;
            }
        }

        #endregion Private Methods

        //------------------------------------------------------
        //
        // Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // Connection string used to establish the database connection.
        private string _databaseIdentifier;
        
        // Connection to the database.
        private SqlConnection _sqlConnection;

        private SqlTransaction _transaction;

        // Names of tables used to store the annotations.
        private string DefinitionsTablename;
        private string ContentLocatorPartsTablename;

        // Table Column names.
        //
        private string ID = "Id";
        private string LOCATOR_INDEX = "LocatorIndex";
        private string PART_INDEX = "PartIndex";
        private string TYPE = "Type";
        private string NAME = "Name";
        private string VALUE = "Value";
        private string DEFINITION = "Definition";

        // Table name Suffix's.
        private const string DefinitionsTableSuffix = "_Definitions";
        private const string ContentLocatorPartsTableSuffix = "_ContentLocatorParts";

        #endregion
    }
}

