// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Utilities
{
    /******************************************************************************
    * CLASS:          BamlInfosetVerifier
    ******************************************************************************/

    /// <summary>
    /// Class for comparing Xaml and Baml Infosets to validate the Baml Infoset.
    /// </summary>
    public class BamlInfosetVerifier
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          CompareInfosetToMaster
        ******************************************************************************/

        /// <summary>
        /// Compare the Infoset from the dynamically-compiled baml to the specified master.
        /// </summary>
        /// <param name="diagBamlOutput">A string containig the an Infoset.</param>
        /// <param name="xamlFileName">The name of the .xaml file being tested.</param>
        /// <param name="masterInfosetFile">Name of the master Infoset file.</param>
        /// <returns>bool value</returns>
        public static bool CompareInfosetToMaster(string diagBamlOutput, string xamlFileName, string masterInfosetFile)
        {
            try
            {
                // Verify the Infoset exists.
                if (String.IsNullOrEmpty(diagBamlOutput))
                {
                    GlobalLog.LogEvidence("No DiagBaml returned, test failed.");
                    return false;
                }

                // Load the master file and retrieve its contents.
                string masterString;
                try
                {
                    StreamReader streamReader = new StreamReader(masterInfosetFile);
                    masterString = streamReader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    GlobalLog.LogEvidence("ERROR: Reading Master Infoset file failed.");
                    GlobalLog.LogEvidence(ex);
                    return false;
                }

                // Trim the strings 
                diagBamlOutput = diagBamlOutput.Trim();
                masterString = masterString.Trim();

                // Compare the master to the created DiagBaml.
                if (masterString.Equals(diagBamlOutput))
                {
                    GlobalLog.LogStatus("Strings matched.");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("The actual Infoset failed to match the expected master.\nGo to the 'Logs location' shown below to compare the Infoset (.diagbaml) files. ");
                    GlobalLog.LogStatus("Writing diagOutput to disk...");
                    StreamWriter writer = new StreamWriter("DiagOutput.DiagBaml");
                    writer.Write(diagBamlOutput);
                    writer.Flush();
                    writer.Close();
                    GlobalLog.LogStatus("Logging files...");
                    GlobalLog.LogFile(masterInfosetFile);
                    GlobalLog.LogFile(xamlFileName);
                    GlobalLog.LogFile("DiagOutput.DiagBaml");
                    return false;
                }
            }
            catch (Exception e)
            {
                GlobalLog.LogEvidence("UNEXPECTED EXCEPTION:\n" + e.Message);
                return false;
            }
        }

        /******************************************************************************
        * Function:          CompareInfosets
        ******************************************************************************/

        /// <summary>
        /// Creates lists of Infoset nodes, reorders the SM's alphabetically, because the
        /// order of SM's may not be the same (in particular for xaml- and baml-based Infosets), and then
        /// conducts a simple string comparison between the two.  Also, the counts are 
        /// compared.  This check is conducted last, so that the complete Infosets can be
        /// written to disk if the test fails.  NOTE: the addConnectionId parameter is required specifically by
        /// Xaml-Baml(BRAT) Infoset verification.
        /// </summary>
        /// <param name="infoset1">The first Infoset to be compared.</param>
        /// <param name="infoset2">The second Infoset to be compared.</param>
        /// <param name="valuesMustBeString">Indicates the value of XamlReaderSettings.ValuesMustBeString.</param>
        /// <param name="addConnectionId">Indicates whether or not the test should add a ConnectionId group.</param>
        /// <returns>A boolean indicating whether or not the test passed.</returns>
        public static bool CompareInfosets(ArrayList infoset1, ArrayList infoset2, bool valuesMustBeString, bool addConnectionId)
        {
            bool passed = true;
            string infoLine1 = string.Empty;
            string infoLine2 = string.Empty;
            ArrayList processedInfoset1 = null;
            ArrayList processedInfoset2 = null;

            try
            {
                // Proccess the xaml and baml infosets, reordering SM's and accounting for Baml optimizations.
                processedInfoset1 = ProcessList(infoset1, "xaml", valuesMustBeString, addConnectionId);
                processedInfoset2 = ProcessList(infoset2, "baml", valuesMustBeString, addConnectionId);

                if (processedInfoset1.Count == 0 || processedInfoset2.Count == 0)
                {
                    throw new TestSetupException("ERROR: ProcessList returned 0 baml and/or xaml counts.");
                }

                // Compare the reordered/altered xaml and baml node lists.
                for (int j = 0; j < processedInfoset1.Count; j++)
                {
                    infoLine1 = ((string)processedInfoset1[j]);
                    infoLine2 = ((string)processedInfoset2[j]);

                    if (String.Compare(infoLine1, infoLine2, false, CultureInfo.InvariantCulture) != 0)
                    {
                        passed = false;
                        break;
                    }
                }

                // Check counts last, so a line-by-line comparison will still take place.
                if (processedInfoset1.Count != processedInfoset2.Count)
                {
                    GlobalLog.LogEvidence("FAIL: Infoset counts differ.\nInfoset1: " + processedInfoset1.Count + "\nInfoset2: " + processedInfoset2.Count);
                    passed = false;
                }
            }
            catch (Exception e)
            {
                passed = false;
                GlobalLog.LogEvidence("UNEXPECTED EXCEPTION:\n" + e.Message);
            }

            // Write the xaml and baml reordered strings to disk, as well as the original .xaml
            // file, but only if the test failed.
            if (!passed)
            {
                if (processedInfoset1 != null)
                {
                    WriteToFile("DiagOutput.Infoset1", processedInfoset1);
                }

                if (processedInfoset2 != null)
                {
                    WriteToFile("DiagOutput.Infoset2", processedInfoset2);
                }
            }

            return passed;
        }

        /******************************************************************************
        * Function:          CompareInfosets II
        ******************************************************************************/

        /// <summary>
        /// This version of CompareInfosets can be called with no reference to connectionId parameter required
        /// specically for Xaml-Baml comparisions.
        /// </summary>
        /// <param name="infoset1">The first Infoset to be compared.</param>
        /// <param name="infoset2">The second Infoset to be compared.</param>
        /// <param name="valuesMustBeString">Indicates the value of XamlReaderSettings.ValuesMustBeString.</param>
        /// <returns>A boolean indicating whether or not the test passed.</returns>
        public static bool CompareInfosets(ArrayList infoset1, ArrayList infoset2, bool valuesMustBeString)
        {
            return CompareInfosets(infoset1, infoset2, valuesMustBeString, false);
        }

        /******************************************************************************
        * Function:          ProcessList
        ******************************************************************************/

        /// <summary>
        /// An Infoset produced from Baml may contain optimizations, particulary if valuesMustBeString is false.
        /// Therefore, because the resulting Infoset can be altered by design, some additional processing
        /// is necessary: either a .baml or .xaml Infoset line will be changed to match the expected content.
        /// and adds it to a new ArrayList.
        /// </summary>
        /// <param name="infoSetList">An ArrayList containing the Infoset.</param>
        /// <param name="infosetType">Indicates whether or not the incoming ArrayList is a Baml or Xaml list.</param>
        /// <param name="valuesMustBeString">Indicates the value of XamlReaderSettings.ValuesMustBeString.</param>
        /// <param name="addConnectionId">Indicates whether or not the test should add a ConnectionId group.</param>
        /// <returns>A new ArrayList containing the reordered Infoset</returns>
        private static ArrayList ProcessList(ArrayList infoSetList, string infosetType, bool valuesMustBeString, bool addConnectionId)
        {
            // Replace infoset content, in order to account for baml optimization.
            ArrayList replacedContent = ReplaceInfosetContent(infoSetList, infosetType, valuesMustBeString, addConnectionId);

            // Reorder SMs with each SO, because property order in Xaml may not be maintained in Baml.
            infoSetList = ReorderInfoset(replacedContent);

            // Copy the 'info' part of the struct into a simple ArrayList to be returned.
            ArrayList infoOnly = new ArrayList();
            foreach (InfoSetRow row in infoSetList)
            {
                infoOnly.Add(((InfoSetRow)row).Info);
            }

            return infoOnly;
        }

        #endregion

        #region Private Members

        /******************************************************************************
        * Function:          ReplaceInfosetContent
        ******************************************************************************/

        /// <summary>
        /// Processes each line of the Infoset, makes any necessary changes, and
        /// adds it to a new ArrayList.
        /// </summary>
        /// <param name="oldList">An ArrayList containing the Infoset.</param>
        /// <param name="infosetType">Indicates whether or not the incoming ArrayList is a Baml or Xaml list.</param>
        /// <param name="valuesMustBeString">Indicates the value of XamlReaderSettings.ValuesMustBeString.</param>
        /// <param name="addConnectionId">Indicates whether or not the test should add a ConnectionId group.</param>
        /// <returns>A new ArrayList containing the reordered Infoset</returns>
        private static ArrayList ReplaceInfosetContent(ArrayList oldList, string infosetType, bool valuesMustBeString, bool addConnectionId)
        {
            string xamlNodeType = string.Empty;
            string itemContent = string.Empty;
            ArrayList newList = new ArrayList(); // An array of InfoSetRow structs.
            int newListIndex = 0;
            string item = string.Empty;
            int skipAddCount = 0;
            int connectionIdCounter = 0;
            bool pointsFoundInSM = false;
            bool setterOrTriggerFoundInSO = false;
            string xamlPrefix = string.Empty;

            // In some cases, an SM group must be removed to account for expected Xaml-Baml Infoset differences.
            RemoveSMGroup(ref oldList, infosetType);

            // Iterate through the infosetList, altering the content to account for expected Xaml/Baml differences, and insert
            // each new row to newList, using newListIndex.
            foreach (string nodeString in oldList)
            {
                if (skipAddCount <= 0)
                {
                    if (nodeString != null)
                    {
                        // Remove any trailing spaces.
                        item = nodeString.TrimEnd(' ');

                        // Remove any quotes that may appear as part of the Value XamlNodeTypes.
                        item = item.Replace("\"", String.Empty);

                        ParseInfosetRow(item, ref xamlNodeType, ref itemContent);

                        switch (xamlNodeType.Trim())
                        {
                            case "NS":
                                HandleNS(xamlNodeType, itemContent, ref xamlPrefix);
                                break;
                            case "SO":
                                HandleSO(itemContent, ref setterOrTriggerFoundInSO);
                                break;
                            case "GO":
                            case "EM":
                            case "EO":
                            case "Closed.":
                                // No alterations in the Infoset line needed for these Node types.
                                break;
                            case "SM":
                                HandleSM(ref newList, ref newListIndex, oldList, ref skipAddCount, ref connectionIdCounter, item, xamlNodeType, ref itemContent, infosetType, valuesMustBeString, ref pointsFoundInSM, xamlPrefix, addConnectionId);
                                item = String.Concat(xamlNodeType, itemContent);
                                break;
                            case "V":
                                HandleV(ref newList, xamlNodeType, ref itemContent, infosetType, valuesMustBeString, ref pointsFoundInSM, setterOrTriggerFoundInSO);
                                item = String.Concat(xamlNodeType, itemContent);
                                break;
                            default:
                                // Values may contain data in multiple rows.  Create a bogus XamlNodeType ("Z")
                                // to handle these.
                                item = "Z " + item;
                                break;
                        }

                        // Adding Infoset line to new ArrayList; in some cases, the item has been changed.
                        AddNodeStringToNewList(ref newList, false, item);

                        newListIndex++;
                    }

                    if (xamlNodeType.Trim() == "Closed.")
                    {
                        break;
                    }
                }

                skipAddCount--;
            }

            return newList;
        }

        /******************************************************************************
        * Function:          HandleNS
        ******************************************************************************/

        /// <summary>
        /// Carry out processing of the NS node in the Infoset.
        /// </summary>
        /// <param name="xamlNodeType">Type of node in the infoset node stream.</param>
        /// <param name="itemContent">The content of a Node.</param>
        /// <param name="xamlPrefix">The prefix to NS content</param>
        private static void HandleNS(string xamlNodeType, string itemContent, ref string xamlPrefix)
        {
            // Retrieve the prefix for the "http://schemas.microsoft.com/winfx/2006/xaml" namespace.
            if (xamlPrefix == string.Empty && xamlNodeType.Trim().Contains("NS"))
            {
                int index1 = itemContent.IndexOf(":", 0, StringComparison.InvariantCulture);
                int index2 = itemContent.IndexOf("=", 0, StringComparison.InvariantCulture);

                if (index1 >= 0 && index2 >= 0)
                {
                    if (String.Equals(itemContent.Substring(index2 + 1), "http://schemas.microsoft.com/winfx/2006/xaml"))
                    {
                        xamlPrefix = itemContent.Substring(index1 + 1, index2 - index1 - 1).Trim();
                    }
                }
            }
        }

        /******************************************************************************
        * Function:          HandleSO
        ******************************************************************************/

        /// <summary>
        /// Carry out processing of the SO node in the Infoset.
        /// </summary>
        /// <param name="itemContent">The content of a Node.</param>
        /// <param name="setterOrTriggerFoundInSO">Indicates whether or not an SO contains a Setter</param>
        private static void HandleSO(string itemContent, ref bool setterOrTriggerFoundInSO)
        {
            // Flag the occurance of an 'SO Setter' or 'SO Trigger'.  It will be used later to change 'V' content.
            if ((itemContent.Trim() == "Setter" || itemContent.Trim() == "Trigger") && itemContent.Trim() != "EventTrigger")
            {
                setterOrTriggerFoundInSO = true;
            }
        }

        /******************************************************************************
        * Function:          HandleSM
        ******************************************************************************/

        /// <summary>
        /// Some of the differences between the Xaml- and Baml-based Infosets are expected, including SM's.
        /// Therefore, changes are made to the Infoset rows.  An altered itemContent is returned by ref.
        /// </summary>
        /// <param name="newList">The to-be-reordered Infoset list.</param>
        /// <param name="newListIndex">Index to newList.</param>
        /// <param name="oldList">The original Infoset list.</param>
        /// <param name="skipAddCount">Used to track skipping of items .</param>
        /// <param name="connectionIdCounter">Counter used for ConnectionId cases.</param>
        /// <param name="item">A single line in the Infoset.</param>
        /// <param name="xamlNodeType">Type of node in the infoset node stream.</param>
        /// <param name="itemContent">The content of a Node.</param>
        /// <param name="infosetType">Either xaml or baml</param>
        /// <param name="valuesMustBeString">Indicates whether or not the Infoset is created as text.</param>
        /// <param name="pointsFoundInSM">Indicates whether or not the SM contains 'Points'</param>
        /// <param name="xamlPrefix">The prefix specified for the Xaml namespace</param>
        /// <param name="addConnectionId">Indicates whether or not the test should add a ConnectionId group.</param>
        private static void HandleSM(ref ArrayList newList, ref int newListIndex, ArrayList oldList, ref int skipAddCount, ref int connectionIdCounter, string item, string xamlNodeType, ref string itemContent, string infosetType, bool valuesMustBeString, ref bool pointsFoundInSM, string xamlPrefix, bool addConnectionId)
        {
            string alteredSMContent = itemContent;

            if (valuesMustBeString)
            {
                if (infosetType == "xaml")
                {
                    // Handle xml:lang. Anything that has an aliased property should be treated as identical,
                    // in this case xml:lang and the XmlLanguage property.
                    alteredSMContent = alteredSMContent.Replace("xml:lang", "Language");
                    alteredSMContent = alteredSMContent.Replace("xml:Lang", "Language");

                    // xaml reports VisualTree but baml reports template
                    alteredSMContent = alteredSMContent.Replace("VisualTree", "Template");

                    // Handle ConnectionId.  Insert a ConnectionId SM before the 'SM Name'; increment its value each time.
                    // The 'addConnectionId' read from the .xtc file indicates that a test must add a ConnectionId group.
                    if (addConnectionId)
                    {
                        if (itemContent.Trim() == "Name" || itemContent.Trim() == "x:Name")
                        {
                            GlobalLog.LogStatus("Adding a ConnectionId SM group to the .xaml...");
                            AddConnectionId(ref newList, ref newListIndex, ref connectionIdCounter, xamlNodeType, xamlPrefix);
                        }
                    }
                }

                // Handle ResourceKey. A SM content can contain 'ResourceKey'. Replacing it with 'x:_PositionalParameters',
                // for both Xaml- and Baml-based Infosets.  They are interchangable.
                alteredSMContent = alteredSMContent.Replace("ResourceKey", "x:_PositionalParameters");

                // Handle xml:space.  A Baml SM content can contain 'x:XmlAttributeProperties.XmlSpace'.  Replacing it
                // with 'xml:space'.
                alteredSMContent = alteredSMContent.Replace("x:XmlAttributeProperties.XmlSpace", "xml:space");

                // Handle Points values.  Set flag for later processing of the associated 'V' node.
                if (alteredSMContent.Contains("Points"))
                {
                    pointsFoundInSM = true;
                }

                // Handle x:Name. In some cases, x:Name in the Xaml Infoset because Name in the Baml Infoset
                // Changing both to Name.
                if (alteredSMContent.Contains(":Name"))
                {
                    alteredSMContent = "Name";
                }

                itemContent = alteredSMContent;
            }
        }

        /******************************************************************************
        * Function:          HandleV
        ******************************************************************************/

        /// <summary>
        /// Some of the differences between the Xaml- and Baml-based Infosets are expected, including V's.
        /// Therefore, changes are made to the content ("itemContent") of 'V' Infoset rows.
        /// </summary>
        /// <param name="newList">The to-be-reordered Infoset list.</param>
        /// <param name="xamlNodeType">Type of node in the infoset node stream.</param>
        /// <param name="itemContent">The content of a Node.</param>
        /// <param name="infosetType">Either xaml or baml</param>
        /// <param name="valuesMustBeString">Indicates whether or not the Infoset is created as text.</param>
        /// <param name="pointsFoundInSM">Indicates whether or not the SM contains 'Points'</param>
        /// <param name="setterOrTriggerFoundInSO">Indicates whether or not an SO contains a Setter</param>
        private static void HandleV(ref ArrayList newList, string xamlNodeType, ref string itemContent, string infosetType, bool valuesMustBeString, ref bool pointsFoundInSM, bool setterOrTriggerFoundInSO)
        {
            // There are several expected differences between Xaml-based and Baml-based
            // Infosets. They will be handled by converting the content of either Baml or Xaml values,
            // so that the comparisons will pass.
            itemContent = AlterValueContent(itemContent, infosetType);

            AlterColorContent(ref itemContent, infosetType, valuesMustBeString);

            // An additional conversion is made because the Xaml and Baml Infosets may produce different point formats
            // which are actually equivalent.
            // Therefore, the point value string will be converted to a point, then back again.
            if (pointsFoundInSM)
            {
                pointsFoundInSM = false;
                try
                {
                    System.Windows.PointConverter pointConverter = new System.Windows.PointConverter();
                    System.Windows.Point point = (System.Windows.Point)pointConverter.ConvertFrom(null, CultureInfo.InvariantCulture, itemContent);
                    itemContent = point.ToString();
                }
                catch
                {
                    // Ignore: the string following the '#' is not a point.
                    GlobalLog.LogStatus("Point conversion failed. Skipping: " + itemContent);
                }
            }

            if (setterOrTriggerFoundInSO)
            {
                // The Baml Infoset adds a object to the Property value of a Setter, e.g., 'V FrameworkElement.Width'.
                // Removing it so that Baml row will match the Xaml row, e.g., 'V Width'.
                // In some cases, Xaml also has a an object added, such as 'V DockPanel.Dock'. So, carrying out this
                // removal for both Xaml and Baml Infosets.
                int index = itemContent.IndexOf(".", 0, StringComparison.InvariantCulture);
                if (index > 0)
                {
                    string value = itemContent.Substring(0, index + 1);
                    itemContent = itemContent.Replace(value, string.Empty);
                }

                setterOrTriggerFoundInSO = false;
            }
        }

        /******************************************************************************
        * Function:          ReorderInfoset
        ******************************************************************************/

        /// <summary>
        /// Processes each line of the Infoset, reorders any SM's within an SO,
        /// and adds it to a new ArrayList.
        /// </summary>
        /// <param name="processedList">An ArrayList containing altered Infoset content.</param>
        /// <returns>A new ArrayList containing the reordered Infoset</returns>
        private static ArrayList ReorderInfoset(ArrayList processedList)
        {
            string item = string.Empty;
            string xamlNodeType = string.Empty;
            string itemContent = string.Empty;
            int matchingSO = 0;
            int listIndex = 0;
            Stack<int> startObjStack = new Stack<int>();
            ArrayList tempList = new ArrayList(); // An array of InfoSetRow structs.

            foreach (object row in processedList)
            {
                item = ((InfoSetRow)row).Info;
                ParseInfosetRow(item, ref xamlNodeType, ref itemContent);

                if (xamlNodeType != null)
                {
                    switch (xamlNodeType.Trim())
                    {
                        case "SO":
                        case "GO":
                            startObjStack.Push(listIndex);
                            AddNodeStringToNewList(ref tempList, false, item);
                            break;
                        case "EO":
                            // Go back and create list of SM-based items to reorder, add to the List, then continue reading.
                            AddNodeStringToNewList(ref tempList, false, item);
                            matchingSO = startObjStack.Pop();

                            ReplaceNodes(ref tempList, matchingSO, listIndex);

                            break;
                        default:
                            AddNodeStringToNewList(ref tempList, false, item);
                            break;
                    }

                    listIndex++;
                }

                if (xamlNodeType.Trim() == "Closed.")
                {
                    break;
                }
            }

            return tempList;
        }

        /******************************************************************************
        * Function:          ParseInfosetRow
        ******************************************************************************/

        /// <summary>
        /// Splits an Infoset row into a XamlNodeType and content (if any).
        /// Assumption:  each Infoset row contains a XamlNodeType and content, separated by one space,
        /// or contains no content.
        /// </summary>
        /// <param name="infosetItem">The array containing XamlNodes</param>
        /// <param name="xamlNodeType">The position of the SO in the ArrayList</param>
        /// <param name="itemContent">The content of a Node</param>
        private static void ParseInfosetRow(string infosetItem, ref string xamlNodeType, ref string itemContent)
        {
            string tempInfosetItem = infosetItem.Trim();

            int index = tempInfosetItem.IndexOf(" ", 0, StringComparison.InvariantCulture);
            if (index < 0)
            {
                // In this case, there is no content following the NodeType.
                xamlNodeType = infosetItem;
                itemContent = string.Empty;
            }
            else
            {
                itemContent = tempInfosetItem.Substring(index + 1).Trim();
                xamlNodeType = infosetItem.Replace(itemContent, string.Empty); // Retain any leading spaces.
            }
        }

        /******************************************************************************
        * Function:          AddConnectionId
        ******************************************************************************/

        /// <summary>
        /// Inserts an SM-V-EM group in the xaml Infoset to match the Baml infoset.
        /// </summary>
        /// <param name="newList">The Infoset ArrayList to be updated.</param>
        /// <param name="newListIndex">An index to the newList ArrayList.</param>
        /// <param name="connectionIdCounter">Increments each time a new ConnectionId is added.</param>
        /// <param name="xamlNodeType">A string containing the Infoset.</param>
        /// <param name="xamlPrefix">A string containing the xaml namespace prefix, if any.</param>
        private static void AddConnectionId(ref ArrayList newList, ref int newListIndex, ref int connectionIdCounter, string xamlNodeType, string xamlPrefix)
        {
            string newInfo = string.Empty;

            // Determine the number of leading spaces to add to the new rows.
            int index = xamlNodeType.TrimEnd(' ').LastIndexOf(' ');
            int leadingCount = xamlNodeType.TrimEnd(' ').Substring(0, index).Length + 1;

            connectionIdCounter++;

            if (xamlPrefix == string.Empty)
            {
                // The default prefix is 'xamlNamespace'.
                newInfo = "SM " + "xamlNamespace:ConnectionId";
            }
            else
            {
                newInfo = "SM " + xamlPrefix + ":ConnectionId";
            }

            newInfo = new string(' ', leadingCount) + newInfo;
            AddNodeStringToNewList(ref newList, false, newInfo);
            newListIndex++;

            newInfo = "V " + connectionIdCounter.ToString();
            newInfo = new string(' ', leadingCount + 1) + newInfo;
            AddNodeStringToNewList(ref newList, false, newInfo);
            newListIndex++;

            newInfo = "EM";
            newInfo = new string(' ', leadingCount) + newInfo;
            AddNodeStringToNewList(ref newList, false, newInfo);
            newListIndex++;
        }

        /******************************************************************************
        * Function:          AddNodeStringToNewList
        ******************************************************************************/

        /// <summary>
        /// Removes content from the Infoset that is not expected to be in both Xaml and Baml.
        /// </summary>
        /// <param name="arrayList">The Infoset ArrayList to be updated.</param>
        /// <param name="ignoreFlag">A boolean indicating whether or not to ignore during replacement.</param>
        /// <param name="nodeString">A string containing the Infoset.</param>
        private static void AddNodeStringToNewList(ref ArrayList arrayList, bool ignoreFlag, string nodeString)
        {
            // Remove from the Xaml string content that is not expected to be in the Baml string.
            CleanUpDiagString(ref nodeString);

            arrayList.Add(new InfoSetRow(ignoreFlag, nodeString));
        }

        /******************************************************************************
        * Function:          ReplaceNodes
        ******************************************************************************/

        /// <summary>
        /// Reorders alphabetically the SMs within an SO group and replaces the original content
        /// in the SO group ArrayList.
        /// NOTE: Each SO group will contain no additional SO nodes that have not already been sorted,
        /// as marked by the ignore flag.
        /// </summary>
        /// <param name="nodeList">The array containing XamlNodes</param>
        /// <param name="startObjPosition">The start obj position.</param>
        /// <param name="endObjPosition">The end obj position.</param>
        private static void ReplaceNodes(ref ArrayList nodeList, int startObjPosition, int endObjPosition)
        {
            // Array of structs containing an SM value plus a pointer to its original position in list.
            ArrayList startList = new ArrayList();
            InfoSetRow row;
            string xamlNodeType = string.Empty;
            string rowContent = string.Empty;
            int endPosition = 0;

            // (1) Copy SM lines into a separate array so they can be sorted.
            // The array consists of structs, each containing the value of the SM XamlNodeType and the
            // start and end positions of the corresponding SM group.
            for (int i = startObjPosition; i < endObjPosition - 1; i++)
            {
                row = (InfoSetRow)nodeList[i];
                ParseInfosetRow(row.Info, ref xamlNodeType, ref rowContent);
                if (xamlNodeType.Trim() == "SM" && row.Ignore == false)
                {
                    nodeList[i] = new InfoSetRow(true, row.Info); // Update the ignore field: ignore the EM from now on.
                    endPosition = FindMatchingEM(nodeList, i);
                    startList.Add(new SMRow(rowContent, i, endPosition)); // Add an new SMRow struct to startList.
                }
            }

            // (2) Replace the contents of the SO group with the sorted SM groups.
            ReplaceSOGroup(ref nodeList, startList, startObjPosition);
        }

        /******************************************************************************
        * Function:          ReplaceSOGroup
        ******************************************************************************/

        /// <summary>
        /// The incoming spList contains an array of structs, each row containing content of an SM.
        /// This routine sorts the SMs, and returns the entire content of all the SMs.
        /// </summary>
        /// <param name="nodeList">The list that will be reordered</param>
        /// <param name="startList">A temporary list</param>
        /// <param name="startingPoint">The starting point of the replacement</param>
        private static void ReplaceSOGroup(ref ArrayList nodeList, ArrayList startList, int startingPoint)
        {
            int startPosition = 0;
            int endPosition = 0;
            ArrayList tempList = new ArrayList();

            SortSMList(ref startList);

            // At this point, the spList is sorted. Now the original contents of the SO group are copied
            // accordingly to a temporary list.
            for (int i = 0; i < startList.Count; i++)
            {
                startPosition = ((SMRow)startList[i]).FirstItem;
                endPosition = ((SMRow)startList[i]).LastItem;
                for (int j = startPosition; j <= endPosition; j++)
                {
                    // Set the first field ("ignore") to true, indicating that the item has been handled.
                    tempList.Add(new InfoSetRow(true, ((InfoSetRow)nodeList[j]).Info));
                }
            }

            // Replace the original SO group with a new, sorted version.  Mark the ignore field in InfoSetRow,
            // so these items won't be involved in any further sorting of these SMs in higher-level SO groups.
            int startObjIndex = startingPoint + 1;
            for (int k = 0; k < tempList.Count; k++)
            {
                nodeList[startObjIndex] = tempList[k];
                startObjIndex++;
            }
        }

        /******************************************************************************
        * Function:          SortSMList
        ******************************************************************************/

        /// <summary>
        /// Apply an Insertion Sort to the SM values in the array of structs.  The sorting is 
        /// based on the struct's spValues.
        /// </summary>
        /// <param name="startList">A temporary list</param>
        private static void SortSMList(ref ArrayList startList)
        {
            int j;
            SMRow index;

            for (int i = 1; i < startList.Count; i++)
            {
                index = (SMRow)startList[i];
                j = i;
                while ((j > 0) && String.Compare(((SMRow)startList[j - 1]).ActualInfoSetValue, index.ActualInfoSetValue, StringComparison.InvariantCulture) > 0)
                {
                    startList[j] = startList[j - 1];
                    j--;
                }

                startList[j] = index;
            }
        }

        /******************************************************************************
        * Function:          FindMatchingEM
        ******************************************************************************/

        /// <summary>
        /// Find the position of the EM that matches the given SM position.  The position of the first EM
        /// not marked as ignored is returned. (Marked as ignored means that it has already been dealt with.)
        /// </summary>
        /// <param name="newList">The to-be-reordered list of nodes</param>
        /// <param name="startObjPosition">The position of the SO in the list</param>
        /// <returns>The position in the list of the matching EM</returns>
        private static int FindMatchingEM(ArrayList newList, int startObjPosition)
        {
            InfoSetRow row;
            bool ignoreSM = false;
            string xamlNodeType = string.Empty;
            string rowContent = string.Empty;
            int j = startObjPosition + 1;

            do
            {
                j++;
                row = (InfoSetRow)newList[j];
                ParseInfosetRow(row.Info, ref xamlNodeType, ref rowContent);
                ignoreSM = Convert.ToBoolean(row.Ignore);
            }
            while (ignoreSM || xamlNodeType.Trim() != "EM");

            return j;
        }

        /******************************************************************************
        * Function:          CreateKnownColorList
        ******************************************************************************/

        /// <summary>
        /// Use reflection to obtain the internal enum KnownColor, which contains all color names.
        /// </summary>
        /// <returns>A array of colors names</returns>
        private static ArrayList CreateKnownColorList()
        {
            ArrayList listOfNames = new ArrayList();

            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.Media.Color));
            Type t = assembly.GetType("System.Windows.Media.KnownColor");
            if (t == null)
            {
                throw new TestSetupException("ERROR: assembly.GetType returned null.");
            }

            Enum en = (Enum)t.InvokeMember(
                                            "KnownColor",
                                            BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                                            null,
                                            null,
                                            new object[] { });
            if (en == null)
            {
                throw new TestSetupException("ERROR: InvokeMember returned null.");
            }

            string[] colorNames = (string[])Enum.GetNames(en.GetType()); // Array of color names.

            for (int j = 0; j < colorNames.Length; j++)
            {
                listOfNames.Add(colorNames[j].ToLower(CultureInfo.InvariantCulture));
            }

            return listOfNames;
        }

        /******************************************************************************
        * Function:          FindColorName
        ******************************************************************************/

        /// <summary>
        /// Look for a color name in the KnownColor dictionary.
        /// </summary>
        /// <param name="possibleColorName">A color name to be found</param>
        /// <returns>Whether or not the color name was found</returns>
        private static bool FindColorName(string possibleColorName)
        {
            bool returnValue = false;

            ArrayList knownColorEnums = CreateKnownColorList(); // Used later for finding color names.

            for (int j = 0; j < knownColorEnums.Count; j++)
            {
                if (String.Compare((string)knownColorEnums[j], possibleColorName, true, CultureInfo.InvariantCulture) == 0)
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }

        /******************************************************************************
        * Function:          ConvertToHexString
        ******************************************************************************/

        /// <summary>
        /// Return a hex color string, given its corresponding Color name (as string).
        /// </summary>
        /// <param name="colorName">The color  string to be converted</param>
        /// <returns>The hex color value converted from a Hex color name</returns>
        private static string ConvertToHexString(string colorName)
        {
            System.Drawing.Color color = ColorTranslator.FromHtml(colorName);
            string hexColor = color.ToArgb().ToString("X", CultureInfo.InvariantCulture);

            return hexColor;
        }

        /******************************************************************************
        * Function:          RemoveSMGroup
        ******************************************************************************/

        /// <summary>
        /// Remove an SM group from the infoset, as required by certain content to make the Xaml
        /// and Baml versions match.
        /// </summary>
        /// <param name="infoSet">The Infoset being changed.</param>
        /// <param name="infosetType">Indicates whether to Infoset originated from Xaml or Baml.</param>
        private static void RemoveSMGroup(ref ArrayList infoSet, string infosetType)
        {
            // Handle xml:base.  An SM node that contains "xml:base" is followed by a Value (V) 
            // node with a local url.  This SM-V-EM sequence is removed.
            foreach (object item in infoSet)
            {
                if (item != null)
                {
                    if (((string)item).Contains("SM xml:base"))
                    {
                        int i = infoSet.IndexOf(item);
                        infoSet.RemoveRange(i, 3);
                        break;
                    }
                }
            }

            // Handle StartupUri.  An SM node that contains "xml:base" is followed by a Value (V) 
            // node with a local url.  This SM-V-EM sequence is removed.
            if (infosetType == "xaml")
            {
                foreach (object item in infoSet)
                {
                    if (item != null)
                    {
                        if (((string)item).Contains("StartupUri"))
                        {
                            int j = infoSet.IndexOf(item);
                            infoSet.RemoveRange(j, 3);
                            break;
                        }
                    }
                }
            }

            // If the Xaml Style lacks a x:Key, a special one is inserted in the Baml Infoset only. Removing it
            // in order to match the Xaml Infoset.
            bool finished = false;

            while (!finished)
            {
                RemoveKey(ref infoSet, ref finished);
            }
        }

        /******************************************************************************
        * Function:          RemoveKey
        ******************************************************************************/

        /// <summary>
        /// Looks for a specific x:Key/x:TypeExtension combination in the Baml infoset and removes it.
        /// </summary>
        /// <param name="infoSet">The Infoset being changed.</param>
        /// <param name="finished">Indicates whether or not the entire Infoset has been examined.</param>
        private static void RemoveKey(ref ArrayList infoSet, ref bool finished)
        {
            bool keyFound = false;

            for (int j = 0; j < infoSet.Count; j++)
            {
                if (infoSet[j] != null)
                {
                    if (((string)infoSet[j]).Contains(":Key"))
                    {
                        keyFound = true;
                        if (j + 1 < infoSet.Count)
                        {
                            if (((string)infoSet[j + 1]).Contains(":TypeExtension") || ((string)infoSet[j + 1]).Contains("DataTemplateKey")) 
                            {
                                infoSet.RemoveRange(j, 7);
                                break;
                            }
                            else
                            {
                                keyFound = false;
                            }
                        }
                    }
                }
            }

            if (keyFound == false)
            {
                finished = true;
            }
        }

        /******************************************************************************
        * Function:          CleanUpDiagString
        ******************************************************************************/

        /// <summary>
        /// Removes content from the Infoset that is not expected to be in both Xaml and Baml.
        /// </summary>
        /// <param name="nodeString">The node string.</param>
        private static void CleanUpDiagString(ref string nodeString)
        {
            nodeString = nodeString.Replace("[Retrieved]", String.Empty);
            nodeString = nodeString.Replace("[Implicit]", String.Empty);
            nodeString = nodeString.TrimEnd(' ');  // Remove spaces that preceded the replaced string.
        }

        /******************************************************************************
        * Function:          AlterValueContent
        ******************************************************************************/

        /// <summary>
        /// Altering value content, in cases where the Baml and Xaml versions differ by design.
        /// </summary>
        /// <param name="valueContent">The baml infoset V node's content</param>
        /// <param name="infosetType">Either xaml or baml</param>
        /// <returns>The altered V content</returns>
        private static string AlterValueContent(string valueContent, string infosetType)
        {
            string alteredValueContent = valueContent;

            if (infosetType == "baml")
            {
                // A Baml V content can contain 'x:ConnectionId'. Replacing it with 'x:Name'.
                alteredValueContent = alteredValueContent.Replace("x:ConnectionId", "x:Name");

                // A Baml V content can contain '(True)  [Boolean]'. Replacing it with 'True'.
                alteredValueContent = alteredValueContent.Replace("(True)  [Boolean]", "True");
                alteredValueContent = alteredValueContent.Replace("(False)  [Boolean]", "False");

                // A Baml V content can contain '(System.String)  [RuntimeType]'.  Replacing it with
                // 'sys:String'.
                alteredValueContent = alteredValueContent.Replace("(System.String)  [RuntimeType]", "sys:String");

                // A Baml V content can contain '(propertyvalue)  [DependencyProperty]'.  Replacing it with
                // just the property value.
                if (alteredValueContent.Contains("[DependencyProperty]"))
                {
                    alteredValueContent = alteredValueContent.Replace("[DependencyProperty]", String.Empty);
                    alteredValueContent = alteredValueContent.Replace("(", String.Empty);
                    alteredValueContent = alteredValueContent.Replace(")", String.Empty);
                    alteredValueContent = alteredValueContent.Trim();
                }

                // A Baml V content can contain '(propertyvalue)  [PointCollection]'.  Replacing it with
                // just the property value.
                if (alteredValueContent.Contains("[PointCollection]"))
                {
                    alteredValueContent = alteredValueContent.Replace("[PointCollection]", String.Empty);
                    alteredValueContent = alteredValueContent.Replace("(", String.Empty);
                    alteredValueContent = alteredValueContent.Replace(")", String.Empty);
                    alteredValueContent = alteredValueContent.Trim();
                }
            }
            else if (infosetType == "xaml")
            {
                if (alteredValueContent.Trim().Equals("true"))
                {
                    alteredValueContent = alteredValueContent.Replace("true", "True");
                }
                else if (alteredValueContent.Trim().Equals("false"))
                {
                    alteredValueContent = alteredValueContent.Replace("false", "False");
                }
            }

            return alteredValueContent;
        }

        /******************************************************************************
        * Function:          AlterColorContent
        ******************************************************************************/

        /// <summary>
        /// Baml replaces color names with their corresponding hex strings.
        /// </summary>
        /// <param name="content">The baml infoset V node's content</param>
        /// <param name="infosetType">Indicates whether or not the incoming ArrayList is a Baml or Xaml list.</param>
        /// <param name="valuesMustBeString">Indicates the value of XamlReaderSettings.ValuesMustBeString.</param>
        private static void AlterColorContent(ref string content, string infosetType, bool valuesMustBeString)
        {
            // Convert both Xaml and Baml based infosets to a hex color, in order to handle 
            // cases where color names are present in text strings.
            if (FindColorName(content))
            {
                content = ConvertToHexString(content);

                if (infosetType == "xaml")
                {
                    // Also, must add additional content for Xaml to match the Baml hex color string.
                    if (valuesMustBeString)
                    {
                        content = "#" + content;
                    }
                    else
                    {
                        content = "(#" + content + ")  [SolidColorBrush]";
                    }
                }
                else if (!content.Contains("[SolidColorBrush]"))
                {
                    // The Baml may have a color name embedded in a text string.
                    if (valuesMustBeString)
                    {
                        content = "#" + content.Trim();
                    }
                    else
                    {
                        content = "(#" + content.Trim() + ")  [SolidColorBrush]";
                    }
                }
            }

            // An additional conversion is made because the Xaml and Baml Infosets may produce different hex strings
            // which are actually equivalent.
            // Therefore, the hex color will be converted to a Color object, then back again.
            if (valuesMustBeString && content.Contains("#"))
            {
                try
                {
                    System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(content);
                    content = color.ToString();
                }
                catch
                {
                    // Ignore: the string following the '#' is not a hex color.
                }
            }
        }

        /******************************************************************************
        * Function:          WriteToFile
        ******************************************************************************/

        /// <summary>
        /// Creates and writes a file to disk, for test case failure analysis.
        /// </summary>
        /// <param name="fileName">The name of the .xaml file to be written.</param>
        /// <param name="resultArray">The array of nodes.</param>
        private static void WriteToFile(string fileName, ArrayList resultArray)
        {
            string outputString = string.Empty;

            // First, format an output string.
            foreach (string line in resultArray)
            {
                outputString += line + "\n";
            }

            StreamWriter writer = new StreamWriter(fileName);
            writer.Write(outputString);
            writer.Flush();
            writer.Close();
            GlobalLog.LogFile(fileName);
        }

        /// <summary>
        /// InfoSetRow representation
        /// </summary>
        private struct InfoSetRow
        {
            /// <summary>
            /// bool ignore
            /// </summary>
            public readonly bool Ignore; // Indicates whether or not SO content has been reordered, and so can be ignored for further reordering.

            /// <summary>
            /// string info
            /// </summary>
            public readonly string Info; // The actual InfoSet line.

            /// <summary>
            /// Initializes a new instance of the <see cref="InfoSetRow"/> struct.
            /// </summary>
            /// <param name="flag">if set to <c>true</c> [flag].</param>
            /// <param name="infoLine">The info line.</param>
            public InfoSetRow(bool flag, string infoLine)
            {
                Ignore = flag;
                Info = infoLine;
            }
        }

        /// <summary>
        /// struct SMRow
        /// </summary>
        private struct SMRow
        {
            /// <summary>
            /// The actualInfoSet line
            /// </summary>
            public readonly string ActualInfoSetValue; // The actualInfoSet line.

            /// <summary>
            /// The position of the SM
            /// </summary>
            public readonly int FirstItem; // The position of the SM.

            /// <summary>
            /// The position of the EM corresponding to the SM.
            /// </summary>
            public readonly int LastItem; // The position of the EM corresponding to the SM.

            /// <summary>
            /// Initializes a new instance of the <see cref="SMRow"/> struct.
            /// </summary>
            /// <param name="val">The value.</param>
            /// <param name="first">The first pos.</param>
            /// <param name="last">The last pos.</param>
            public SMRow(string val, int first, int last)
            {
                ActualInfoSetValue = val;
                FirstItem = first;
                LastItem = last;
            }
        }

        #endregion
    }
}
