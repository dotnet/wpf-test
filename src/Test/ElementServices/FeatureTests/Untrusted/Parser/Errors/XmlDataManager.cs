// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using Avalon.Test.CoreUI.Trusted;
using System;
using System.Collections;
using System.Xml;

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// XmlDataManager manages the expected error data.
    /// At initialization, Load() gets called, in which this class reads the expected 
    /// error data (master) from the error file into an in-memory XmlNodeList.
    /// This class has knowledge about the format (Xml schema) of the error file.
    /// 
    /// When the engine calls ReadNextRecord(), this class searches in the XmlNodeList for a 
    /// data node labeled "All". This label is represented by the property DataNodeName.
    /// The default value for this property is "All", and this class will always look for a data node
    /// labeled "All" first. If it finds one, it will return it. Subclasses can override the
    /// DataNodeName property, in order to specify an additional data node to look for, 
    /// in case "All" is not found.
    /// When a suitable data node is found, it extracts the data under it, puts it into a Hashtable, 
    /// and returns the Hashtable.
    /// 
    /// If the engine calls ChangeCurrentRecord(), this class changes the error data
    /// (master) in the current data node with the actual error data that is sent by the engine.
    /// 
    /// When the engine calls PersistChanges(), this class writes out the (possibly changed)
    /// XmlNodeList to the error file.
    /// </summary>
    public class XmlDataManager : DataManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFilename"></param>
        public XmlDataManager(string dataFilename) : base(dataFilename)
        {
            // A subclass may override this.    
            DataNodeName = "All";
        }

        /// <summary>
        /// Load the error file, and populate the XmlNodeList.
        /// Also, set the position to before the first node.
        /// </summary>
        override public void Load()
        {
            _errDoc = new XmlDocument();
            _errDoc.Load(_dataFilename);
            _dataNodes = _errDoc.SelectNodes("./Errors/File");

            // Currently we are positioned even before the first node.
            _currentPosition = -1;
        }

        /// <summary>
        /// 1. Advance the current position. 
        /// 2. If the current position is now more than the number of nodes, return null 
        /// 3. If not, check if the current node has a child node with either the 
        ///    label "All" (searched first), or the label returned by DataNodeName property
        ///   (if "All" not found)
        /// 4. If it has such a child node, extract the data under it and return it.   
        /// 5. If not, go to step 1
        /// </summary>
        /// <returns></returns>
        override public Hashtable ReadNextRecord()
        {
            bool nodeFound = false;
            XmlNode currentNode = null;

            while (!nodeFound)
            {
                _currentPosition++;
                if (_currentPosition == _dataNodes.Count)
                {
                    return null;
                }

                // First look for a child node with the label "All"
                // If found, go ahead with it.
                // If not found, look for a child node with the label returned by DataNodeName
                // property
                currentNode = _dataNodes[_currentPosition];
                _currentSubNode = currentNode.SelectSingleNode("./All");
                if (null != _currentSubNode)
                {
                    nodeFound = true;
                }
                else
                {
                    _currentSubNode = currentNode.SelectSingleNode("./" + DataNodeName);
                    if (null != _currentSubNode)
                    {
                        nodeFound = true;
                    }
                }
            }

            Hashtable expectedErrData = new Hashtable();
            // Put in the name of the Xaml file, since that's the only way for 
            // TestExecutor to know it.
            expectedErrData.Add("XamlFileName", (currentNode as XmlElement).GetAttribute("Name"));
            // Get all the children nodes, and populate the hashtable,
            // with key being the LocalName of a child node, and value 
            // being the text inside the node.
            XmlNodeList childNodes = _currentSubNode.ChildNodes;
            foreach (XmlNode childNode in childNodes)
            {
                expectedErrData.Add(childNode.LocalName, childNode.InnerText);
            }
            return expectedErrData;
        }

        /// <summary>
        /// Retrives a File node with the given filename as the value of its
        /// "Name" attribute
        /// 
        /// This function does not change the "current position" pointer
        /// that is used by functions like ChangeCurrentRecord() and ReadNextRecord() 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Hashtable GetRecord(string filename)
        {
            XmlNode node, subNode;
            for (int i = 0; i < _dataNodes.Count; i++)
            {
                node = _dataNodes[i];
                if (filename == (node as XmlElement).GetAttribute("Name"))
                {
                    // First look for a child node with the label "All"
                    // If found, go ahead with it.
                    // If not found, look for a child node with the label returned by DataNodeName
                    // property
                    subNode = node.SelectSingleNode("./All");
                    if (null == subNode)
                    {
                        subNode = node.SelectSingleNode("./" + DataNodeName);
                        if (null == subNode)
                        {
                            return null;
                        }
                    }
                    
                    Hashtable expectedErrData = new Hashtable();
                    // Put in the name of the Xaml file, since that's the only way for 
                    // TestExecutor to know it.
                    expectedErrData.Add("XamlFileName", filename);
                    // Get all the children nodes, and populate the hashtable,
                    // with key being the LocalName of a child node, and value 
                    // being the text inside the node.
                    XmlNodeList childNodes = subNode.ChildNodes;
                    foreach (XmlNode childNode in childNodes)
                    {
                        expectedErrData.Add(childNode.LocalName, childNode.InnerText);
                    }
                    return expectedErrData;            
                }
            }
            return null; // Node not found, so return null
        }

        /// <summary>
        /// Update the master error data with the new data.
        /// 
        /// Get keys and values from the new error data Hashtable,
        /// and change the current Xml node.
        /// We have special processing for key mismatch, i.e. for 
        /// entries that are in the new error data, but not in existing data,
        /// or vice-versa.
        /// </summary>
        /// <param name="newErrData"></param>
        override public void ChangeCurrentRecord(Hashtable newErrData)
        {
            // For each child node, do the following:
            // 1. If the name of the child node appears as a key in newErrData, 
            //    modify the contents of the child node with the corresponding value in newErrData.
            //    Then, delete the corresponding entry in newErrData.
            // 2. If the name of the child node does NOT appear in newErrData, remove the child node
            XmlNodeList childNodes = _currentSubNode.ChildNodes;
            ArrayList toBeRemoved = new ArrayList();
            foreach (XmlNode childNode in childNodes)
            {
                if (newErrData.Contains(childNode.LocalName))
                {
                    childNode.InnerText = newErrData[childNode.LocalName] as string;
                    newErrData.Remove(childNode.LocalName);
                }
                else
                {
                    toBeRemoved.Add(childNode);
                }
            }

            // Now remove the child nodes that were designated as to be removed.
            foreach (XmlNode nodeToBeRemoved in toBeRemoved)
            {
                _currentSubNode.RemoveChild(nodeToBeRemoved);
            }

            // Now that we have deleted all entries in newErrData corresponding to respective 
            // children nodes, any entries still in newErrData are new. We need to create
            // children nodes for them, and add them to the data node.
            ICollection remainingKeys = newErrData.Keys;
            foreach (string remainingKey in remainingKeys)
            {
                XmlElement newElement = _errDoc.CreateElement(remainingKey);
                newElement.InnerText = newErrData[remainingKey] as string;
                _currentSubNode.AppendChild(newElement);
            }
        }

        /// <summary>
        /// Save the XmlNodeList to the error file.
        /// </summary>
        override public void PersistChanges()
        {
            _errDoc.Save(_dataFilename);
        }
       
        private string _dataNodeName = null;
        /// <summary>
        /// This is the name of the Xml node that contains the error data.
        /// See the comments on the class for how this is used.
        /// </summary>
        protected string DataNodeName
        {
            get { return _dataNodeName; }
            set { _dataNodeName = value; }
        }

        private XmlDocument _errDoc = null;
        private XmlNodeList _dataNodes = null;
        private XmlNode _currentSubNode = null;
        private int _currentPosition;
    }
}
