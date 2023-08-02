using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid Builder
    /// </summary>
    public class DataGridBuilder
    {
        public static ObservableCollection<DataGridData> Construct()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<DataGridDataInfo>" +
            "<DataGridData StringType='one' IntType='1' DoubleType='1.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='two' IntType='2' DoubleType='2.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='three' IntType='3' DoubleType='3.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='four' IntType='4' DoubleType='4.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='five' IntType='5' DoubleType='5.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='six' IntType='6' DoubleType='6.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='seven' IntType='7' DoubleType='7.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='eight' IntType='8' DoubleType='8.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='nine' IntType='9' DoubleType='9.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='ten' IntType='10' DoubleType='10.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='eleven' IntType='11' DoubleType='11.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='twelve' IntType='12' DoubleType='12.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='thirteen' IntType='13' DoubleType='13.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='fourteen' IntType='14' DoubleType='14.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='fifteen' IntType='15' DoubleType='15.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='sixteen' IntType='16' DoubleType='16.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='seventeen' IntType='17' DoubleType='17.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='eighteen' IntType='18' DoubleType='18.0' BoolType='false' StructType='Now' />" +
            "<DataGridData StringType='nineteen' IntType='19' DoubleType='19.0' BoolType='true' StructType='{null}' />" +
            "<DataGridData StringType='twenty' IntType='20' DoubleType='20.0' BoolType='false' StructType='Now' />" +
            "</DataGridDataInfo>");

            return Construct(xmlDocument);
        }
        /// <summary>
        /// Construct Observable DataGridData Collection
        /// </summary>
        /// <param name="xmlDocument">XmlDocument</param>
        /// <returns>Observable DataGridData Collection</returns>
        public static ObservableCollection<DataGridData> Construct(XmlDocument xmlDocument)
        {
            ObservableCollection<DataGridData> dataGridData = new ObservableCollection<DataGridData>();
            ParserXmlData(xmlDocument, dataGridData);
            return dataGridData;
        }

        private static void ParserXmlData(XmlDocument xmlDocument, ObservableCollection<DataGridData> dataGridData)
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

                    ((ObservableCollection<DataGridData>)dataGridData).Add((DataGridData)Activator.CreateInstance(typeof(DataGridData), parameterList.ToArray(), null));
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
}
