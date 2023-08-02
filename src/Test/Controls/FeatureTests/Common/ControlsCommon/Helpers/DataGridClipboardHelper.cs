using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using System.Text;
using Microsoft.Test.Logging;
using System.Xml;


namespace Microsoft.Test.Controls.Helpers
{
#if TESTBUILD_CLR40
    /// <summary>
    /// General DataGrid helper
    /// </summary>
    public static class DataGridClipboardHelper
    {
        // info needed for verification
        public struct ClipboardCopyInfo
        {
            public int minColumnDisplayIndex;
            public int maxColumnDisplayIndex;
            public int minRowIndex;
            public int maxRowIndex;
            public int[] rowIndices;
            public DataGridClipboardCopyMode clipboardCopyMode;
            public DataGridHelper.GetDataFromTemplateColumn GetDataFromTemplateColumn;
        }        

        public static void VerifyClipboardCopy(DataGrid dataGrid, ClipboardCopyInfo clipboardCopyInfo)
        {
            // the default formats
            Collection<string> formats = new Collection<string>(new string[] { DataFormats.Text, DataFormats.UnicodeText, DataFormats.CommaSeparatedValue, DataFormats.Html });

            // create the expected data
            List<List<string>> expectedRowValues = CreateExpectedData(dataGrid, ref clipboardCopyInfo);

            // get the actual
            IDataObject ido = Clipboard.GetDataObject();
            foreach (string format in formats)
            {
                object actualData = ((DataObject)ido).GetData(format);
                if (actualData == null)
                {
                    // 


                    continue;
                }

                // dump the actual data
                DumpActualData(actualData, format);

                // verify here
                VerifyClipboardCopyHelper(expectedRowValues, (string)actualData, format, clipboardCopyInfo.clipboardCopyMode);
            }
        }

        public static List<List<string>> CreateExpectedData(DataGrid dataGrid, ref ClipboardCopyInfo clipboardCopyInfo)
        {
            List<List<string>> expectedRowValues = new List<List<string>>();
            if (clipboardCopyInfo.clipboardCopyMode == DataGridClipboardCopyMode.IncludeHeader)
            {
                List<string> cellValues = new List<string>();
                for (int i = clipboardCopyInfo.minColumnDisplayIndex; i <= clipboardCopyInfo.maxColumnDisplayIndex; i++)
                {
                    DataGridColumn column = dataGrid.ColumnFromDisplayIndex(i);
                    cellValues.Add((string)column.Header);
                }

                expectedRowValues.Add(cellValues);
            }

            if (clipboardCopyInfo.rowIndices != null)
            {
                foreach (int rowIndex in clipboardCopyInfo.rowIndices)
                {
                    List<string> cellValues = new List<string>();
                    for (int j = clipboardCopyInfo.minColumnDisplayIndex; j <= clipboardCopyInfo.maxColumnDisplayIndex; j++)
                    {
                        string cellValue = DataGridHelper.GetDataFromCell(dataGrid, rowIndex, j, false, clipboardCopyInfo.GetDataFromTemplateColumn);
                        cellValues.Add(cellValue);
                    }

                    expectedRowValues.Add(cellValues);
                }
            }
            else
            {
                for (int i = clipboardCopyInfo.minRowIndex; i <= clipboardCopyInfo.maxRowIndex; i++)
                {
                    List<string> cellValues = new List<string>();
                    for (int j = clipboardCopyInfo.minColumnDisplayIndex; j <= clipboardCopyInfo.maxColumnDisplayIndex; j++)
                    {
                        string cellValue = DataGridHelper.GetDataFromCell(dataGrid, i, j, false, clipboardCopyInfo.GetDataFromTemplateColumn);
                        cellValues.Add(cellValue);
                    }

                    expectedRowValues.Add(cellValues);
                }
            }

            // dump the expected data
            DumpExpectedData(expectedRowValues);

            return expectedRowValues;
        }

        private static void VerifyClipboardCopyHelper(List<List<string>> expectedRowValues, string actualData, string format, DataGridClipboardCopyMode copyMode)
        {
            string[] actualRows;
            if (format == DataFormats.Html)
            {
                actualRows = ParseHtmlRows(actualData);
            }
            else
            {
                actualRows = actualData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (expectedRowValues.Count != actualRows.Length)
            {
                throw new TestValidationException(string.Format("Expected row count: {0}, does not match actual row count: {1}", expectedRowValues.Count, actualRows.Length));
            }

            for(int i=0; i<expectedRowValues.Count; i++)
            {
                string[] actualCells = GetActualCells(format, actualRows, i);

                if (expectedRowValues[i].Count != actualCells.Length)
                {
                    throw new TestValidationException(string.Format("Expected cell count: {0}, does not match actual cell count: {1}", expectedRowValues[i].Count, actualCells.Length));
                }

                for (int j = 0; j < expectedRowValues[i].Count; j++)
                {
                    if ((expectedRowValues[i])[j] == null && actualCells[j] != null)
                    {
                        throw new TestValidationException(string.Format("expected cell copy value is null, actual cell copy value: {0}", actualCells[j]));
                    }
                    else if ((expectedRowValues[i])[j] != null && actualCells[j] == null)
                    {
                        throw new TestValidationException(string.Format("expected cell copy value: {0}, actual cell copy value is null", (expectedRowValues[i])[j].ToString()));
                    }
                    else if ((expectedRowValues[i])[j].ToString() != actualCells[j])
                    {
                        throw new TestValidationException(string.Format("expected cell copy value: {0}, actual cell copy value: {1}", (expectedRowValues[i])[j].ToString(), actualCells[j]));
                    }
                }
            }
        }

        private static string[] GetActualCells(string format, string[] actualRows, int i)
        {
            string[] actualCells;
            if (format == DataFormats.CommaSeparatedValue)
            {
                actualCells = Parse(actualRows[i], ',');
            }
            else if (format == DataFormats.Text || format == DataFormats.UnicodeText)
            {
                actualCells = Parse(actualRows[i], '\t');
            }
            else if (format == DataFormats.Html)
            {
                actualCells = ParseHtmlCells(actualRows[i]);
            }
            else
            {
                throw new ArgumentException(string.Format("The format parameter: {0} is not a valid or implemented format.", format));
            }
            return actualCells;
        }

        private static string[] Parse(string value, char separator)
        {
            List<string> outputList = new List<string>();

            int startIndex = 0;
            int endIndex = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if(ch == separator)
                {
                    outputList.Add(value.Substring(startIndex, endIndex - startIndex));

                    startIndex = endIndex + 1;
                    endIndex = startIndex;
                }
                else if(ch == '\"')
                {
                    // skip until the ending quotes
                    i++;
                    if (i >= value.Length)
                    {
                        throw new FormatException(string.Format("value: {0} had a format exception", value));
                    }
                    char tempCh = value[i];
                    while (tempCh != '\"' && i < value.Length)                                            
                        i++;                    

                    endIndex = i;
                }
                else if (i + 1 == value.Length)
                {
                    // add the last value
                    outputList.Add(value.Substring(startIndex));
                    break;
                }
                else
                {
                    endIndex++;
                }                
            }

            return outputList.ToArray();
        }

        private static string[] ParseHtmlRows(string value)
        {
            List<string> outputList = new List<string>();

            int startIndex = value.IndexOf("<TABLE>", StringComparison.OrdinalIgnoreCase);
            int endIndex = value.IndexOf("</TABLE>", StringComparison.OrdinalIgnoreCase);
            string updatedString = value.Substring(startIndex, (endIndex + "</TABLE>".Length) - startIndex);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(updatedString);
            foreach (XmlNode node in doc.DocumentElement.GetElementsByTagName("TR"))
            {
                outputList.Add("<TR>" + node.InnerXml + "</TR>");
            }

            return outputList.ToArray();
        }

        private static string[] ParseHtmlCells(string value)
        {
            List<string> outputList = new List<string>();                   

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(value);
            foreach (XmlNode node in doc.DocumentElement.GetElementsByTagName("TD"))
            {
                outputList.Add(node.InnerXml);
            }            

            return outputList.ToArray();
        }

        private static void DumpExpectedData(List<List<string>> expectedRowValues)
        {
            int i = 0;
            foreach (List<string> expectedRow in expectedRowValues)
            {
                TestLog.Current.LogStatus(string.Format("Expected Row {0}: ", i++));
                StringBuilder sb = new StringBuilder();
                foreach (string cellValue in expectedRow)
                {
                    sb.Append(cellValue + " ");
                }
                TestLog.Current.LogStatus(sb.ToString());
            }
        }

        private static void DumpActualData(object actualData, string format)
        {
            TestLog.Current.LogStatus(string.Format("Actual data format: {0}", format));
            TestLog.Current.LogStatus(string.Format("Actual data: {0}{1}", Environment.NewLine, actualData.ToString()));            
        }
    }
#endif
}
