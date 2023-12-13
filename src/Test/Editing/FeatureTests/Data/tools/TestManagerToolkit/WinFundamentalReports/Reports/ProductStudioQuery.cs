// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinFundamentalReports.Reports
{
    #region Namespaces.

    using System;
    using System.Xml;
    using ProductStudio;

    #endregion Namespaces.

    /// <summary>
    /// Provides an object to run a Product Stuido query file
    /// and write the results to an XML stream.
    /// </summary>
    internal class ProductStudioQuery
    {
        internal ProductStudioQuery()
        {
        }

        internal static DatastoreItemList ExecuteQuery(string productName,
            string queryXml, string[] fieldNames)
        {
            Directory directory;

            directory = new DirectoryClass();
            directory.Connect("", "", "");
            try
            {
                Product product;
                Datastore store;
                Query query;
                DatastoreItemList items;

                product = directory.GetProductByName(productName);
                store = product.Connect("", "", "");

                query = new QueryClass();
                query.CountOnly = false;
                query.SelectionCriteria = queryXml;
                query.DatastoreItemType = PsDatastoreItemTypeEnum.psDatastoreItemTypeBugs;

                items = new DatastoreItemListClass();
                items.Datastore = store;
                items.Query = query;

                query.QueryFields.Clear();
                foreach(string s in fieldNames)
                {
                    try
                    {
                        query.QueryFields.Add(store.FieldDefinitions[s]);
                    }
                    catch(Exception exception)
                    {
                        string message;
                        message = "Error: " + exception.Message +
                            "\r\n\r\nAccessing field: " + s + "\r\n\r\n" +
                            "Available fields:\r\n";
                        foreach(FieldDefinition d in store.FieldDefinitions)
                        {
                            message += "[" + d.Name + "] ";
                        }
                        System.Windows.Forms.MessageBox.Show(message);
                        throw;
                    }
                }
                items.Execute();

                return items;
            }
            finally
            {
                directory.Disconnect();
            }
        }

        /// <summary>Gets the field value for the given item.</summary>
        /// <param name="item">PS item to query.</param>
        /// <param name="fieldName">Field to return value for.</param>
        /// <returns>The string value of the value, an empty string if it's null.</returns>
        internal static string FieldValue(DatastoreItem item, string fieldName)
        {
            Field field;

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }

            field = item.Fields[fieldName];
            if (field == null)
            {
                throw new ArgumentException("Cannot find field " + fieldName, "fieldName");
            }
            if (field.Value == null)
            {
                return "";
            }
            else
            {
                return field.Value.ToString();
            }
        }

        /// <summary>Runs the specified query file.</summary>
        internal string RunQuery(string psqFileName)
        {
            if (psqFileName == null)
            {
                throw new ArgumentNullException("psqFileName");
            }

            using (System.IO.StringWriter stringWriter = new System.IO.StringWriter())
            using (System.Xml.XmlWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter))
            {
                WritePsqFile(psqFileName, xmlWriter);
                xmlWriter.Flush();
                return stringWriter.ToString();
            }
        }

        /// <summary>Runs the specified query file.</summary>
        internal System.Xml.XmlReader RunQueryAsXml(string psqFileName)
        {
            return new System.Xml.XmlTextReader(new System.IO.StringReader(RunQuery(psqFileName)));
        }

        /// <summary>
        /// Transforms an PS item field name into an XML attribute name.
        /// </summary>
        private static string FieldNameToAttributeName(string fieldName)
        {
            return fieldName.Replace(" ", "");
        }

        /// <summary>
        /// Writes the results of executing the specified PSQ file
        /// into an XmlWriter object.
        /// </summary>
        private void WritePsqFile(string psqFileName, XmlWriter writer)
        {
            PSQFile psqFile;
            PSQProductQueries productQueries;
            PSQMode mode;

            System.Diagnostics.Debug.Assert(psqFileName != null);
            System.Diagnostics.Debug.Assert(writer != null);

            writer.WriteStartElement("QueryFile");
            writer.WriteAttributeString("FileName", psqFileName);

            // Open the query file.
            psqFile = new PSQFileClass();
            psqFile.Load(psqFileName);
            if (!psqFile.IsValid)
            {
                throw new InvalidOperationException("Invalid PSQ file: " + psqFileName);
            }

            // Retrieve the queries from the file.
            mode = psqFile.Modes[psqFile.CurrentMode];
            if (mode.HasHandler == true && mode.Handler is IPSQModeHandlerCoreModes)
            {
                IPSQModeHandlerCoreModes handler = (IPSQModeHandlerCoreModes) mode.Handler;
                productQueries = handler.GenerateProductQueries();
            }
            else
            {
                throw new InvalidOperationException("Unknown PS mode.");
            }

            // Output the query results into an XML file.
            foreach (PSQProductQuery productQuery in productQueries)
            {
                WriteQuery(productQuery, writer);
            }

            writer.WriteEndElement();
        }

        private void WriteQuery(PSQProductQuery query, XmlWriter writer)
        {
            System.Diagnostics.Debug.Assert(writer != null);
            System.Diagnostics.Debug.Assert(query != null);

            writer.WriteStartElement("Query");
            writer.WriteAttributeString("ProductName", query.ProductName);

            DatastoreItems items;
            items = query.Run();
            writer.WriteAttributeString("ItemCount", items.Count.ToString());
            for (int i = 0; i < items.Count; i++)
            {
                DatastoreItem item = items[i];
                writer.WriteStartElement("Item");
                for (int j = 0; j < item.Fields.Count; j++)
                {
                    Field field = item.Fields[j];
                    writer.WriteStartElement(FieldNameToAttributeName(field.Name));
                    if (field.Value != null)
                    {
                        writer.WriteString(field.Value.ToString());
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
