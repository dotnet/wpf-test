// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Imlements the common action sequence engine and its related
 *          interfaces.
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Xml;

using Microsoft.Test.Integration;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Common engine for running action sequences and verifying the events and properties.
    /// </summary>  
    public class ActionSequenceTestEngine : IHostedTest
    {
        static ActionSequenceTestEngine()
        {
            // Add TypeTypeConverter to TypeDescriptor so we can parse type strings from xtc.
            Attribute attrib = new TypeConverterAttribute(typeof(TypeTypeConverter));
            Utility.AddAttributeToTypeDescriptor(typeof(Type), attrib);
        }

        /// <summary>
        /// Creates a ActionSequenceTestEngine instance
        /// </summary>
        public ActionSequenceTestEngine()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void RunTestAction(ContentItem item)
        {
            TestLog.Current.LogStatus("In ActionSequenceTestEngine.RunTestAction...");

            //
            // Get TestContainer.
            //
            if (!CommonStorage.Current.Contains("TestContainer"))
            {
                throw new Microsoft.Test.TestSetupException("No 'TestContainer' in storage.");
            }

            TestLog.Current.LogStatus("'TestContainer' is in storage.");
            this.TestContainer = CommonStorage.Current.Get("TestContainer") as ITestContainer;

            //
            // Get XML TEST node.
            //
            XmlDocument doc = new XmlDocument();
            doc.LoadXml((string)item.Content);
            _testNode = doc.DocumentElement;

            NameTable ntable = new NameTable();
            _nsmgr = new XmlNamespaceManager(ntable);
            _nsmgr.AddNamespace("x", "ActionSequence");

            string execDir = (string)Harness.Current.RemoteSite["STI_ExecutionDirectory"];
            Environment.CurrentDirectory = execDir;

            //
            // Begin variation.
            //
            _RunVariationAsync(null);
        }

        /// <summary>
        /// Common entry point for all action sequence tests.
        /// This simply posts a delegate to run on the Dispatcher, and
        /// it starts the dispatcher if necessary.
        /// </summary>
        public void RunVariation(XmlNode testNode, string xmlns)
        {
            _testNode = testNode;

            NameTable ntable = new NameTable();
            _nsmgr = new XmlNamespaceManager(ntable);
            _nsmgr.AddNamespace("x", xmlns);

            DispatcherOperation op = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(_RunVariationAsync), null);
            if (!TestContainer.RequestStartDispatcher())
            {
                op.Wait();
            }
        }

        // Async sequence runner that is invoked by the Dispatcher.
        // It parses the given xml into a tree, EventRecorder, and action sequence.
        // Then, it runs the action sequence while recording events and properties.
        // Finally, it compares the expected sequence with the recorded sequence.
        private object _RunVariationAsync(object obj)
        {
            // Reset keyboard state.
            KeyboardHelper.ResetKeyboardState();

            // Parse tree config. Construct tree.
            DependencyObject root = _ParseTreeConfig(_testNode);

            // Turn Ime off by default - individual tests may override in actions.
            InputMethod.SetPreferredImeState(root, InputMethodState.Off);

            // Parse desired event names.
            List<EventFilter> eventFilters = _ParseEventFilters(_testNode);

            // Parse desired property names.
            List<PropertyFilter> propertyFilters = _ParsePropertyFilters(_testNode);

            // Parse action and event sequence.
            List<SequenceAction> sequence = _ParseSequence(_testNode);

            // Initialize event recorder.
            EventRecorder eventRecorder = new EventRecorder(root, eventFilters, propertyFilters);

            // Display tree.
            TestContainer.DisplayObject(root, 200, 200, 400, 400);
            TestContainer.CurrentSurface[0].ForceActivation();
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);

            // Pass tree root, sequence, and event names to _RunSequence.
            CoreLogger.LogStatus("Running sequence...");
            List<SequenceAction> actualSequence = _RunSequence(root, sequence, eventRecorder);

            // Compare recorded events against expected events.
            // If they match, detach the EventRecorder, rerun the sequence, and verify again.
            if (_VerifySequences(sequence, actualSequence) && _shouldRepeatSequence)
            {
                CoreLogger.LogStatus("Removing handlers and running sequence again...");

                MouseHelper.MoveOnVirtualScreenMonitor();

                eventRecorder.DetachHandlers();

                foreach (SequenceAction action in sequence)
                {
                    action.EventRecords.Clear();
                }

                // Pass tree root, sequence, and event names to _RunSequence.
                actualSequence = _RunSequence(root, sequence, eventRecorder);

                // Compare recorded events against expected events.
                _VerifySequences(sequence, actualSequence);
            }


            // Quit.
            TestContainer.EndTest();

            return null;
        }

        private void _ClearEventRecordsFromSequence(List<SequenceAction> sequence)
        {
            foreach (SequenceAction action in sequence)
            {
                action.EventRecords.Clear();
            }
        }

        // Runs a sequence of actions.
        // Returns a sequence of actions and recorded items that occurred 
        // as a result of running the actions.
        private List<SequenceAction> _RunSequence(DependencyObject root, List<SequenceAction> sequence, EventRecorder eventRecorder)
        {
            // Initialize actual sequence list. 
            List<SequenceAction> actualSequence = new List<SequenceAction>();

            // Loop thru every action.
            foreach (SequenceAction item in sequence)
            {
                // Clear the last recorded events.
                eventRecorder.RecordedEvents.Clear();

                // Enable or disabled recorder according to
                // the action's specification.
                if (item.IgnoreEvents)
                {
                    eventRecorder.IsEnabled = false;
                }
                else
                {
                    eventRecorder.IsEnabled = true;

                    eventRecorder.IgnoreInputState = item.IgnoreInputState;
                    eventRecorder.IgnoreEventArgs = item.IgnoreEventArgs;
                }

                // Do the action. (Add it to actual total sequence.)
                item.DoAction(root);

                // Add the recorded events to the actual total sequence.
                SequenceAction actualAction = (SequenceAction)item.Clone();

                actualAction.EventRecords.Clear();
                foreach (EventRecord eventRecord in eventRecorder.RecordedEvents)
                {
                    actualAction.EventRecords.Add(eventRecord);
                }

                actualSequence.Add(actualAction);
            }

            return actualSequence;
        }

        /// <summary>
        /// Extracts the tree definition from the xml, and creates a tree.
        /// </summary>
        /// <param name="testNode">Root node of an XML document.</param>
        /// <returns>Root element of a logical tree.</returns>
        private DependencyObject _ParseTreeConfig(XmlNode testNode)
        {
            XmlElement treeConfigNode = (XmlElement)testNode.SelectSingleNode(".//x:TreeConfig", _nsmgr);
            Stream stream = null;

            if (treeConfigNode.HasAttribute("Source"))
            {
                string filePath = treeConfigNode.GetAttribute("Source");
                stream = File.OpenRead(filePath);
            }
            else
            {
                stream = IOHelper.ConvertTextToStream(treeConfigNode.InnerXml);
            }

            return (DependencyObject)System.Windows.Markup.XamlReader.Load(stream);
        }

        /// <summary>
        /// Extracts the action sequence from the xaml, and creates a list
        /// of SequenceAction's.
        /// </summary>
        /// <param name="testNode">XML node containing some XAML.</param>
        /// <returns>Sequence of actions.</returns>
        private List<SequenceAction> _ParseSequence(XmlNode testNode)
        {
            List<SequenceAction> sequence = new List<SequenceAction>();
            Dictionary<string, Type> typeDictionary = _GetTypes(testNode);

            // Get actions.
            XmlElement seqNode = (XmlElement)testNode.SelectSingleNode(".//x:ActionSequence", _nsmgr);

            if(seqNode.HasAttribute("ShouldRepeat"))
            {
                _shouldRepeatSequence = Boolean.Parse(seqNode.GetAttribute("ShouldRepeat"));
            }

            XmlNodeList childNodes = seqNode.ChildNodes;
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i] is XmlComment)
                    continue;

                // Create action.
                XmlElement actionNode = (XmlElement)childNodes[i];

                SequenceAction action = (SequenceAction)_ConvertFromXmlElement(null, actionNode, typeDictionary);

                if (!action.IgnoreEvents)
                {
                    // Get expected event records and add to action.
                    List<EventRecordBase> eventRecords = new List<EventRecordBase>();

                    XmlNodeList eventNodes = actionNode.SelectNodes("./*", _nsmgr);
                    for (int j = 0; j < eventNodes.Count; j++)
                    {
                        EventRecordBase eventRecord = (EventRecordBase)_ConvertFromXmlElement(action, (XmlElement)eventNodes[j], typeDictionary);

                        eventRecords.Add(eventRecord);
                    }
                    action.EventRecords.AddRange(eventRecords);
                }

                sequence.Add(action);
            }

            return sequence;
        }

        /// <summary>
        /// Instantiates a type from an xml node.
        /// </summary>
        /// <param name="action">Current action.</param>
        /// <param name="xmlNode">XML node referring to a type name.</param>
        /// <param name="typeDictionary">Mapper object, mapping strings to type.</param>
        /// <returns>An object of the type specified by the XML node.</returns>
        private object _ConvertFromXmlElement(SequenceAction action, XmlElement xmlNode, Dictionary<string, Type> typeDictionary)
        {
            string typeName = xmlNode.Name;

            Type type = null;
            try
            {
                type = typeDictionary[typeName];
            }
            catch (KeyNotFoundException ex)
            {
                throw new Microsoft.Test.TestSetupException("Type '" + typeName + "' not found.", ex);
            }

            if (action == null && !typeof(SequenceAction).IsAssignableFrom(type))
                throw new ArgumentNullException("action", "Parameter must be a valid SequenceAction.");

            object obj = Activator.CreateInstance(
                type,
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new object[0],
                null
            );

            _SetProperties(obj, xmlNode.Attributes);

            // Special case: EventRecordGroup.
            // Add children as EventRecord items.
            if (obj is EventRecordGroup)
            {
                List<EventRecord> records = ((EventRecordGroup)obj).EventRecords;

                foreach (XmlNode child in xmlNode.ChildNodes)
                {
                    XmlElement childElement = child as XmlElement;

                    if (childElement == null)
                        continue;

                    EventRecord record = (EventRecord)_ConvertFromXmlElement(action, childElement, typeDictionary);
                    records.Add(record);
                }
            }
            // Special case: EventRecord.
            // Add children as PropertyRecord or EventArgsRecord items.
            else if (obj is EventRecord)
            {
                EventRecord eventRecord = (EventRecord)obj;

                if (action.IgnoreInputState)
                {
                    eventRecord.DownKeys.Clear();
                }

                List<PropertyRecord> propertyRecords = eventRecord.PropertyRecords;

                foreach (XmlNode child in xmlNode.ChildNodes)
                {
                    XmlElement childElement = child as XmlElement;

                    if (childElement == null)
                        continue;

                    if (action.IgnoreEventArgs && String.Equals(childElement.Name, typeof(EventArgsRecord).Name, StringComparison.InvariantCulture))
                        continue;

                    object record = _ConvertFromXmlElement(action, childElement, typeDictionary);

                    if (record is PropertyRecord)
                    {
                        propertyRecords.Add((PropertyRecord)record);
                    }
                    else if (record is EventArgsRecord)
                    {
                        eventRecord.EventArgs = (EventArgsRecord)record;
                    }
                    else
                    {
                        throw new NotSupportedException("Child xml element type is '" + childElement.Name + "'.  Only PropertyRecord or EventArgsRecord elements are supported under an EventRecord.");
                    }
                }
            }
            else if (obj is EventArgsRecord)
            {
                EventArgsRecord argsRecord = (EventArgsRecord)obj;

                foreach (XmlNode child in xmlNode.ChildNodes)
                {
                    XmlElement childElement = child as XmlElement;

                    if (childElement == null)
                        continue;

                    argsRecord.SetArg(childElement.GetAttribute("Property"), childElement.GetAttribute("Value"));
                }
            }

            return obj;
        }

        // Extracts EventFilter declarations from the xml, and creates
        // a list of EventFilter's.
        private List<EventFilter> _ParseEventFilters(XmlNode testNode)
        {
            List<EventFilter> eventFilters = new List<EventFilter>();
            Type type = typeof(EventFilter);

            XmlNodeList childNodes = testNode.SelectNodes(".//x:MemberFilters/x:EventFilter", _nsmgr);

            for (int i = 0; i < childNodes.Count; i++)
            {
                XmlElement xmlNode = childNodes[i] as XmlElement;

                if (xmlNode == null)
                    continue;

                EventFilter filter = new EventFilter();

                _SetProperties(filter, xmlNode.Attributes);

                eventFilters.Add(filter);
            }

            return eventFilters;
        }

        // Extracts PropertyFilter declarations from the xml, and creates
        // a list of PropertyFilter's.
        private List<PropertyFilter> _ParsePropertyFilters(XmlNode testNode)
        {
            List<PropertyFilter> propertyFilters = new List<PropertyFilter>();
            Type type = typeof(PropertyFilter);

            XmlNodeList childNodes = testNode.SelectNodes(".//x:MemberFilters/x:PropertyFilter", _nsmgr);

            for (int i = 0; i < childNodes.Count; i++)
            {
                XmlElement xmlNode = childNodes[i] as XmlElement;

                if (xmlNode == null)
                    continue;

                PropertyFilter filter = new PropertyFilter();

                _SetProperties(filter, xmlNode.Attributes);

                propertyFilters.Add(filter);
            }

            return propertyFilters;
        }

        // For every attribute on an xml node, the corresonding property
        // is set on the given object. EventRecord and PropertyRecord cannot
        // be set as attributes.
        private void _SetProperties(object obj, XmlAttributeCollection attributes)
        {
            Type type = obj.GetType();
            Type stringType = typeof(String);

            // Loop through attributes on xml node.
            foreach (XmlAttribute attrib in attributes)
            {
                PropertyInfo property = type.GetProperty(attrib.Name);
                if (property == null)
                {
                    throw new Microsoft.Test.TestSetupException("Could not find property '" + attrib.Name + "' on '" + type.Name + "' instance.");
                }

                if (property.PropertyType == typeof(List<EventRecord>) || property.PropertyType == typeof(List<PropertyRecord>))
                {
                    throw new Microsoft.Test.TestSetupException("Cannot set property '" + attrib.Name + "' of type '" + property.PropertyType.Name + "'.");
                }

                // Convert attribute value if the matching property isn't a string.
                object value = attrib.Value;
                Type propertyType = property.PropertyType;
                
                if (propertyType != stringType)
                {
                    TypeConverter converter = null;
                    if (propertyType == typeof(KeyList))
                        converter = new KeyListConverter();
                    else if (propertyType == typeof(Type))
                        converter = new TypeTypeConverter();
                    else
                        converter = TypeDescriptor.GetConverter(propertyType);
                    
                    if (converter.CanConvertFrom(stringType))
                    {
                        value = converter.ConvertFrom(value);
                    }
                    else
                    {
                        throw new Microsoft.Test.TestSetupException("Cannot convert attribute value '" + value + "' to type '" + propertyType.Name + "'.");
                    }
                }
                property.SetValue(obj, value, null);
            }
        }

        // Gets Type instances for all actions, EventRecord's,
        // and PropertyRecord's declared in the xml.
        private Dictionary<string, Type> _GetTypes(XmlNode testNode)
        {
            List<string> typeNames = new List<string>();

            XmlNodeList nodes = testNode.SelectNodes(".//x:ActionSequence/*", _nsmgr);
            _PopulateTypeNames(nodes, ref typeNames);

            typeNames.Add("EventRecord");
            typeNames.Add("EventRecordGroup");
            typeNames.Add("PropertyRecord");
            typeNames.Add("EventArgsRecord");
            typeNames.Add("DependencyPropertyChangedEventArgsWrapper");

            return Utility.FindTypes(typeNames, false);
        }

        /// <summary>
        /// Populate collection of type names from XML.
        /// </summary>
        /// <param name="nodeList">List of XML nodes.</param>
        /// <param name="typeNames">Reference to string list object.</param>
        private void _PopulateTypeNames(XmlNodeList nodeList, ref List<string> typeNames)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode node = nodeList[i];
                if (!typeNames.Contains(node.Name))
                    typeNames.Add(node.Name);
            }
        }

        /// <summary>
        /// Saves a sequence as a new XTC file.
        /// </summary>
        /// <param name="xmlNode">XML node as the root of a tree.</param>
        /// <param name="sequence">Sequence to save.</param>
        /// <remarks>XTC filename is defined in "_saveVariationFileName" variable.</remarks>
        private void _SaveVariation(XmlNode xmlNode, List<SequenceAction> sequence)
        {
            //
            // Create new xml doc. 
            // Remove all test nodes except the current one.
            //
            XmlDocument doc = new XmlDocument();
            XmlNode newNode = doc.ImportNode(xmlNode, true);
            XmlNode prevNode = null;
            for (XmlNode ParentNode = xmlNode.ParentNode; ParentNode != null; ParentNode = ParentNode.ParentNode)
            {
                if (ParentNode is XmlDocument)
                    break;

                prevNode = newNode;
                newNode = doc.ImportNode(ParentNode, false);
                newNode.AppendChild(prevNode);
            }
            doc.AppendChild(newNode);

            //
            // Get the ActionSequence node, and clear all actions from it.
            //
            XmlDocument document = doc;
            XmlElement seqNode = (XmlElement)doc.SelectSingleNode(".//x:ActionSequence", _nsmgr);

            if (!_shouldRepeatSequence)
                seqNode.SetAttribute("ShouldRepeat", "False");

            seqNode.RemoveAll();

            //
            // Convert all the actions to xml nodes.
            // Use reflection to read all properties.
            // 
            foreach (SequenceAction action in sequence)
            {
                XmlNode actionNode = _ConvertToXmlNode(action, document);

                foreach (EventRecordBase record in action.EventRecords)
                {
                    XmlNode eventNode = _ConvertToXmlNode(record, document);
                    actionNode.AppendChild(eventNode);

                    EventRecord eventRecord = record as EventRecord;
                    if (eventRecord != null)  // EventRecord
                    {
                        foreach (PropertyRecord propertyRecord in eventRecord.PropertyRecords)
                        {
                            XmlNode propNode = _ConvertToXmlNode(propertyRecord, document);
                            eventNode.AppendChild(propNode);
                        }
                        if (!action.IgnoreEventArgs)
                        {
                            eventNode.AppendChild(_ConvertToXmlNode(eventRecord.EventArgs, document));
                        }
                    }
                    else  
                    {
                        throw new InvalidOperationException("Cannot handle type '" + record.GetType().Name + "'.");
                    }
                }

                seqNode.AppendChild(actionNode);
            }

            //
            // Save xml doc.
            //
            string fileName = _saveVariationFileName;
            if (File.Exists(fileName))
                File.Delete(fileName);

            document.Save(fileName);
            GlobalLog.LogFile(fileName);
        }

        // Converts the given object to an xml node. All of the object's
        // properties are set as attributes on the new xml node, except
        // EventRecords and PropertyRecords.
        private XmlNode _ConvertToXmlNode(object obj, XmlDocument document)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            XmlElement newNode = document.CreateElement(type.Name, _nsmgr.LookupNamespace("x"));

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                if (property.PropertyType == typeof(List<EventRecordBase>)
                    || property.PropertyType == typeof(List<EventRecord>)
                    || property.PropertyType == typeof(List<EventRecordGroup>)
                    || property.PropertyType == typeof(List<PropertyRecord>)
                    || property.PropertyType == typeof(Dictionary<string, object>)
                    || property.PropertyType == typeof(EventArgsRecord))
                    continue;

                string value = property.GetValue(obj, null).ToString();

                if (!String.IsNullOrEmpty(value))
                    newNode.SetAttribute(property.Name, value);
            }

            if (obj is EventArgsRecord)
            {
                EventArgsRecord argsRecord = (EventArgsRecord)obj;
                foreach (KeyValuePair<string, object> pair in argsRecord.Args)
                {
                    XmlElement argNode = document.CreateElement("EventArg", _nsmgr.LookupNamespace("x"));
                    argNode.SetAttribute("Property", pair.Key);

                    object val = pair.Value == null ? "null" : pair.Value;
                    
                    argNode.SetAttribute("Value", val.ToString());

                    newNode.AppendChild(argNode);
                }
            }

            return newNode;
        }

        // Helper routine that creates a nicely-formatted string for presenting differences
        // between two sequences.
        private static string _CreateComparisonTable(List<SequenceAction> expectedSequence, List<SequenceAction> actualSequence)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Action Sequence");
            builder.AppendLine("--------------");

            int maxCount = expectedSequence.Count > actualSequence.Count ?
                expectedSequence.Count : actualSequence.Count;

            for (int i = 0; i < maxCount; i++)
            {
                SequenceAction expectedAction = i < expectedSequence.Count ? expectedSequence[i] : null;
                SequenceAction actualAction = i < actualSequence.Count ? actualSequence[i] : null;

                _BuildActionTable(expectedAction, actualAction, builder);
            }

            return builder.ToString();
        }

        // Helper routine that creates a nicely-formatted string for presenting differences
        // between two SequenceAction's.
        private static void _BuildActionTable(SequenceAction expectedAction, SequenceAction actualAction, StringBuilder builder)
        {
            bool ignoreEvents = false;
            List<EventRecordBase> expectedEvents = null;
            List<EventRecordBase> actualEvents = null;

            if (actualAction != null)
            {
                builder.Append("\r\nActual Action: " + actualAction.ToString());
                ignoreEvents = actualAction.IgnoreEvents;
                actualEvents = actualAction.EventRecords;
            }
            else
            {
                builder.Append("\r\nActual Action: none");
            }

            if (expectedAction != null)
            {
                builder.Append("\r\nExpected Action: " + expectedAction.ToString());
                ignoreEvents = expectedAction.IgnoreEvents;
                expectedEvents = expectedAction.EventRecords;
            }
            else
            {
                builder.Append("\r\nExpected Action: none");
            }

            builder.AppendLine();

            if (ignoreEvents)
            {
                builder.Append("\r\n\tEvent records ignored\r\n");
            }
            else
            {
                _BuildEventRecordTable(expectedEvents, actualEvents, expectedAction, builder);
            }
        }

        // Inflates EventRecordGroup instances in a sequence.
        private static void _InflateSequence(List<SequenceAction> sequence)
        {
            foreach (SequenceAction action in sequence)
            {
                List<EventRecordBase> expectedEvents = action.EventRecords;

                if (expectedEvents == null)
                {
                    return;
                }

                List<EventRecordGroup> groups = new List<EventRecordGroup>();
                foreach (EventRecordBase eventRecord in expectedEvents)
                {
                    EventRecordGroup group = eventRecord as EventRecordGroup;

                    if (group != null)
                    {
                        groups.Add(group);
                    }
                }

                foreach (EventRecordGroup group in groups)
                {
                    int index = expectedEvents.IndexOf(group);
                    expectedEvents.RemoveAt(index);

                    int repeatCount = group.RepeatCount >= 0 ? group.RepeatCount : 1;

                    for (int i = 0; i < repeatCount; i++)
                    {
                        foreach (EventRecord eventRecord in group.EventRecords)
                        {
                            expectedEvents.Insert(index++, eventRecord);
                        }
                    }
                }
            }
        }

        // Helper routine that creates a nicely-formatted string for presenting differences
        // between two EventRecord lists.
        private static void _BuildEventRecordTable(List<EventRecordBase> expectedEvents, List<EventRecordBase> actualEvents, SequenceAction expectedAction, StringBuilder builder)
        {
            if (expectedEvents == null)
                expectedEvents = new List<EventRecordBase>();

            if (actualEvents == null)
                actualEvents = new List<EventRecordBase>();

            int maxCount = expectedEvents.Count > actualEvents.Count ?
                expectedEvents.Count : actualEvents.Count;

            if (maxCount == 0)
            {
                builder.Append("\r\n\tNo event records\r\n");
            }

            for (int i = 0; i < maxCount; i++)
            {
                builder.AppendLine();

                object actualItem = "none";
                object expectedItem = "none";

                if (i < actualEvents.Count)
                    actualItem = actualEvents[i];

                if (i < expectedEvents.Count)
                    expectedItem = expectedEvents[i];

                if (!actualItem.Equals(expectedItem))
                    builder.Append("==>");

                //
                // Actual
                //
                builder.Append("\tActual:  \t");
                builder.Append(actualItem.ToString());

                _BuildPropertyRecordTable(i, actualEvents, builder);

                builder.AppendLine();

                //
                // Expected
                //
                builder.Append("\tExpected:\t");
                builder.Append(expectedItem.ToString());

                _BuildPropertyRecordTable(i, expectedEvents, builder);

                builder.AppendLine();
            }
        }

        // Helper routine that creates a nicely-formatted string for presenting differences
        // between two PropertyRecord lists.
        private static void _BuildPropertyRecordTable(int index, List<EventRecordBase> eventRecords, StringBuilder builder)
        {
            bool hasPropertyRecords = false;

            if (index < eventRecords.Count && eventRecords[index] is EventRecord)
            {
                List<PropertyRecord> propertyRecords = ((EventRecord)eventRecords[index]).PropertyRecords;
                foreach (PropertyRecord record in propertyRecords)
                {
                    hasPropertyRecords = true;
                    builder.AppendLine();
                    builder.Append("\t         \t    " + record.ToString());
                }
            }

            if (!hasPropertyRecords)
            {
                builder.AppendLine();
                builder.Append("\t         \t    No property records");
            }
        }

        // Simple verification that compares two sequences.

        /// <summary>
        /// Simple verifier that compares two sequences.
        /// </summary>
        /// <param name="expectedSequence">First sequence</param>
        /// <param name="actualSequence">Second sequence.</param>
        /// <returns>true if sequences are identical, false otherwise.</returns>
        private bool _VerifySequences(List<SequenceAction> expectedSequence, List<SequenceAction> actualSequence)
        {
            bool same = true;
            string failureMsg = String.Empty;

            CoreLogger.LogStatus("Comparing actual sequence to expected sequence...");

            if (expectedSequence.Count != actualSequence.Count)
            {
                failureMsg += "Sequence action count does not match expected count. Expected '" + expectedSequence.Count + "' Actual '" + actualSequence.Count + "'.";
                same = false;
            }
            else
            {
                int i = 0;
                foreach (SequenceAction action in expectedSequence)
                {
                    CoreLogger.LogStatus("Verifying action '" + action.ToString() + "'.");

                    // Compare records.
                    if (!action.Equals(actualSequence[i]))
                    {
                        failureMsg += "Sequence record '" + i.ToString() + "' does not match.";
                        same = false;
                        break;
                    }
                    i++;
                }
            }

            // Compare recorded events against expected events.
            if (!same)
            {
                _SaveVariation(_testNode, actualSequence);
                _InflateSequence(expectedSequence);
                failureMsg += "\r\nThe actual sequence does not match the expected sequence.\r\n\r\n"
                    + _CreateComparisonTable(expectedSequence, actualSequence)
                    + "\r\n\r\nActual sequence is saved as " + _saveVariationFileName + "\r\n";
                CoreLogger.LogTestResult(false, failureMsg);
            }

            CoreLogger.LogStatus("Done comparing sequences");

            return same;
        }

        /// <summary>
        /// IHostedTest.TestContainer implementation.
        /// </summary>
        public ITestContainer TestContainer
        {
            get
            {
                return _testContainer;
            }
            set
            {
                _testContainer = value;
            }
        }

        private ITestContainer _testContainer = null;
        private string _saveVariationFileName = "__ActualVariationSequence.xtc";
        private XmlNamespaceManager _nsmgr = null;
        private delegate void _RunAsyncCallback(XmlNode xmlNode, string xmlns);
        private XmlNode _testNode = null;
        private bool _shouldRepeatSequence = true;
    }

    /// <summary>
    /// Represents a sequence action that the ActionSequenceTestEngine reads
    /// from a sequence script and calls in order relative to other actions
    /// in the script.
    /// </summary>
    public abstract class SequenceAction : ICloneable
    {
        /// <summary>
        /// </summary>
        public SequenceAction()
        {
            _eventRecords = new List<EventRecordBase>();
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.IsSynchronous.GetHashCode() ^ this.IgnoreEvents.GetHashCode();

            for (int i = 0; i < this.EventRecords.Count; i++)
            {
                hashCode ^= this.EventRecords[i].GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SequenceAction))
                return false;

            SequenceAction action = (SequenceAction)obj;

            if (!this.IsSynchronous.Equals(action.IsSynchronous))
                return false;

            if (!this.IgnoreEvents.Equals(action.IgnoreEvents))
                return false;

            if (!this.IgnoreInputState.Equals(action.IgnoreInputState))
                return false;

            if (!this.IgnoreEventArgs.Equals(action.IgnoreEventArgs))
                return false;

            int i = 0;
            foreach(EventRecordBase record in this.EventRecords)
            {
                // If the expected record is a group, verify the number of repetitions
                // is correct. Otherwise, the expected record is a single EventRecord
                // so compare it directly.
                if (record is EventRecordGroup)
                {
                    i = _CheckEventRecordGroup((EventRecordGroup)record, action.EventRecords, i);
                    if (i < 0)
                        return false;
                }
                else
                {
                    if (i >= action.EventRecords.Count)
                        return false;

                    EventRecord eventRecord = (EventRecord)record;
                    if (!eventRecord.Equals(action.EventRecords[i++]))
                        return false;
                }
            }

            if (i < action.EventRecords.Count)
                return false;

            return true;
        }

        // Checks for repetions of an event record group.
        // Returns the index that comes after the last repetion.
        // If the number of repetions doesn't match the expected number,
        // returns -1.
        private int _CheckEventRecordGroup(EventRecordGroup group, List<EventRecordBase> eventRecords, int baseIndex)
        {
            int currentIndex = baseIndex;
            int iterationIndex = currentIndex;
            int j = 0, cnt = 0;

            // Continue counting the number of repetitions as long as
            // they exist.
            while(iterationIndex < eventRecords.Count)
            {
                // If we find an unmatched record, break immediately.
                // At this point, cnt should be the number of repetitions,
                // and currentIndex should be:
                //  currentIndex = baseIndex + (group.EventRecords.Count * cnt).
                if (!group.EventRecords[j].Equals(eventRecords[iterationIndex]))
                {
                    break;
                }

                // If we reach the end of the group, reset group index
                // and increment our iteration count.
                if (++j == group.EventRecords.Count)
                {
                    j = 0;
                    cnt++;
                    currentIndex += group.EventRecords.Count;
                }

                // Update temp index as we check if there is another
                // repetition of the group.
                iterationIndex = currentIndex + j;
            }

            // Return -1 if the number of iterations was less than RepeatCount
            // or not >= 1 if RepeatCount was infinite.
            if ((group.RepeatCount < 0 && cnt > 0) ||
                (group.RepeatCount >= 0 && cnt == group.RepeatCount))
            {
                return currentIndex;
            }
            else
            {
                CoreLogger.LogStatus("EventRecordGroup.RepeatCount is '" + group.RepeatCount + "', actual repeat count is '" + cnt + "'."); 
                return -1;
            }
        }

        /// <summary>
        /// </summary>
        public abstract void DoAction(DependencyObject root);

        /// <summary>
        /// </summary>
        public virtual object Clone()
        {
            Type type = this.GetType();

            // Create clone instance.
            object clone = Activator.CreateInstance(
                type,
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new object[0],
                null
            );

            // Loop through properties, copy values to clone.
            PropertyInfo[] properties = type.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                // Skip generic-type properties - not supported yet.
                if (property.PropertyType.IsGenericType)
                    continue;

                // Copy value of property to the clone.
                object val = property.GetValue(this, null);
                property.SetValue(clone, val, null);
            }

            return clone;
        }

        /// <summary>
        /// </summary>
        public bool IgnoreEvents
        {
            get { return _ignoreEvents; }
            set { _ignoreEvents = value; }
        }

        /// <summary>
        /// </summary>
        public bool IgnoreInputState
        {
            get { return _ignoreInputState; }
            set { _ignoreInputState = value; }
        }

        /// <summary>
        /// </summary>
        public bool IgnoreEventArgs
        {
            get { return _ignoreEventArgs; }
            set { _ignoreEventArgs = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<EventRecordBase> EventRecords
        {
            get { return _eventRecords; }
            set { _eventRecords = value; }
        }

        /// <summary>
        /// Directs whether or not the action should pump messages before returning.
        /// </summary>
        public bool IsSynchronous
        {
            get
            {
                return _isSynchronous;
            }
            set
            {
                _isSynchronous = value;
            }
        }

        private List<EventRecordBase> _eventRecords;
        private bool _ignoreEvents = false;
        private bool _ignoreInputState = true;
        private bool _ignoreEventArgs = true;
        private bool _isSynchronous = true;
    }    
    /// <summary>
    /// TypeConverter for System.Type
    /// </summary>
    internal class TypeTypeConverter : TypeConverter
    {
        static TypeTypeConverter()
        {
            s_types = new List<Type>();

            s_types.AddRange(Utility.GetAssemblyTypes(typeof(Dispatcher)).Assembly);
            s_types.AddRange(Utility.GetAssemblyTypes(typeof(UIElement)).Assembly);
            s_types.AddRange(Utility.GetAssemblyTypes(typeof(FrameworkElement)).Assembly);
#if TARGET_NET3_5
            _types.AddRange(Utility.GetAssemblyTypes(typeof(UIElement3D)).Assembly);
#endif

            s_types.AddRange(Utility.CoreTestTypes.Values);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string typeName = value as string;

            if (typeName != null)
            {
                return Utility.FindType(s_types, typeName, true);
            }

            return base.ConvertFrom(context, culture, value);
        }

        private static List<Type> s_types = null;
    }
}

