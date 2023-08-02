using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using Avalon.Test.ComponentModel;
using System.Windows.Controls;
//using Microsoft.Windows.Controls;

namespace Microsoft.Test.Controls
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGridData Collection Builder
    /// </summary>
    public class DataGridDataCollectionBuilder
    {
        /// <summary>
        /// Construct Observable DataGridData Collection
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>Observable DataGridData Collection</returns>
        public static ObservableCollection<DataGridData> Construct(XmlDocument xmlDocument)
        {
            ObservableCollection<DataGridData> dataGridDataCollection = new ObservableCollection<DataGridData>();
            ParserXmlData(xmlDocument, dataGridDataCollection);
            return dataGridDataCollection;
        }

        public static void AddItemsToDataGrid(XmlDocument xmlDocument, DataGrid dataGrid)
        {
            ParserXmlData(xmlDocument, dataGrid);
        }

        private static void ParserXmlData(XmlDocument xmlDocument, object collectionObject)
        {
            foreach (XmlNode node in xmlDocument["DataGridDataInfo"].ChildNodes)
            {
                if (node.Name.Equals("DataGridData"))
                {
                    // Pre-Build default parameter collection with the type we support.
                    Dictionary<string, XmlAttribute> parameterCollection = new Dictionary<string, XmlAttribute>();
                    parameterCollection.Add("StringType", null);
                    parameterCollection.Add("IntType", null);
                    parameterCollection.Add("DoubleType", null);
                    parameterCollection.Add("BoolType", null);
                    parameterCollection.Add("StructType", null);

                    // Loop through the xml attributes and add attribute that match the type in the pre-builded parameter collection above.
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if (parameterCollection.Keys.Contains(attribute.Name))
                        {
                            parameterCollection[attribute.Name] = attribute;
                        }
                    }

                    ArrayList parameterList = new ArrayList();
                    foreach (XmlAttribute attribute in parameterCollection.Values)
                    {
                        AddValueToParameterList(parameterList, attribute);
                    }

                    // There are two ways to add data to DataGrid.
                    // First, bind data to DataGrid.ItemsSource
                    // Second, add data to DataGrid.Items.Add() manually.
                    if (collectionObject is ObservableCollection<DataGridData>)
                    {
                        ((ObservableCollection<DataGridData>)collectionObject).Add((DataGridData)Activator.CreateInstance(typeof(DataGridData), parameterList.ToArray(), null));
                    }
                    else if (collectionObject is DataGrid)
                    {
                        ((DataGrid)collectionObject).Items.Add((DataGridData)Activator.CreateInstance(typeof(DataGridData), parameterList.ToArray(), null));
                    }
                }
            }
        }

        /// <summary>
        /// Add value to parameter list
        /// </summary>
        /// <param name="parameterList">Use ArrayList because it takes any type</param>
        /// <param name="xmlAttribute">XmlAttribute</param>
        private static void AddValueToParameterList(ArrayList parameterList, XmlAttribute xmlAttribute)
        {
            if (xmlAttribute == null)
            {
                parameterList.Add(null);
                return;
            }

            // Construct parameterbuiler name.
            // For example: XmlAttribute.Name="StringType", remove "Type" from the string, then append "ParameterBuilder" 
            // to the string to construct "StringParameterBuilder" type name.
            string parameterBuilderName = xmlAttribute.Name.Substring(0, xmlAttribute.Name.Length - "Type".Length) + "ParameterBuilder";
            ParameterBuilder parameterBuilder = (ParameterBuilder)ObjectFactory.CreateObjectFromTypeName(parameterBuilderName);
            if (parameterBuilder == null)
            {
                throw new ArgumentException("ParameterBuilder is null.");
            }
            parameterBuilder.Construct(parameterList, xmlAttribute);
        }
    }
#endif
}
