// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*************************************************************
//
//
//   Program:   TestChangeble base class
 
 
//*************************************************************
using System;
using System.Xml;
using System.Windows;

using Microsoft.Test.ElementServices.Freezables.Utils;


namespace Microsoft.Test.ElementServices.Freezables
{

    /**********************************************************************************
    * CLASS:          FreezablesPatternsBase
    **********************************************************************************/
    internal abstract class FreezablesPatternsBase
    {

        #region Private Data
        internal Microsoft.Test.ElementServices.Freezables.Utils.Result result;
        internal string testName;
        #endregion

        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal FreezablesPatternsBase(Microsoft.Test.ElementServices.Freezables.Utils.Result logResult)
        {
            result = logResult;
        }
        #endregion


        #region Internal Members

        internal abstract void Perform(Type t);

        /******************************************************************************
        * Function:          LoadChangeableFile
        ******************************************************************************/
        /// <summary>
        /// Create and load a new XmlDocument.
        /// </summary>
        /// <returns>An XmlDocument loaded from FreezablesPatterns.xtc.</returns>
        internal XmlDocument LoadChangeableFile()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load("FreezablesPatterns.xtc");
            }
            catch (Exception)
            {
                throw new ApplicationException("Unable to load FreezablesPatterns.xtc file");
            }
            return doc;
        }

        /******************************************************************************
        * Function:          IsKnownIssue
        ******************************************************************************/
        /// <summary>
        /// Check for presence of Name and IssueType.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <returns></returns>
        internal virtual bool IsKnownIssue(Type t)
        {
            XmlDocument doc = LoadChangeableFile();
            XmlElement changeableTest = (XmlElement)doc["FreezablesTest"];
            for (XmlNode testNameNode = changeableTest["TestName"]; testNameNode != null; testNameNode = testNameNode.NextSibling)
            {
                XmlAttribute name = testNameNode.Attributes["Name"];
                // Throw exception on IssueType because I want to enforce tester to enter whether it is by design
                // or a KnownBug, it has nothing to do with more with the code.
                XmlAttribute issue = testNameNode.Attributes["IssueType"];
                if (name == null || issue == null)
                {
                    throw new ApplicationException("TestName Element must have attributes Name=, IssueType=");
                }
                if (name.Value == testName)
                {   
                    for (XmlNode classNode = testNameNode["Class"]; classNode != null; classNode = classNode.NextSibling)
                    {
                        XmlAttribute className = classNode.Attributes["Name"];
                        if (className == null)
                        {
                            throw new ApplicationException("Class element must have attribute Name=");
                        }
                        if (className.Value == t.ToString())
                        {
                            return true;
                        }
                    }
                }

            }
            return false;
        }
        #endregion
    }


    /**********************************************************************************
    * CLASS:          TestGetAndSetChangeable
    **********************************************************************************/
    internal abstract class TestGetAndSetChangeable : FreezablesPatternsBase
    {
        #region Private Data
        protected System.Windows.Freezable      obj     = null;
        protected string                        pName   = null;
        #endregion

        #region Constructor
        /******************************************************************************
        * Function:          TestGetAndSetChangeable
        ******************************************************************************/
        internal TestGetAndSetChangeable(Microsoft.Test.ElementServices.Freezables.Utils.Result result)
            : base(result)
        {
        }
        #endregion

     
        #region Internal Members
        /******************************************************************************
        * Function:          IsKnownIssue
        ******************************************************************************/
        /// <summary>
        /// Overrides check for presence of Name and IssueType.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <returns></returns>
        internal override bool IsKnownIssue(Type t)
        {
            if (pName == "Item" && ((System.Collections.ICollection)obj).Count == 0)
            {
                // Some Animation collection classes does not have public constuctor
                // that enable us to construct properly. This is the work around for 
                // the OutOfRange exception.
                return true;
            }
            if (pName == "Context" || pName == "DependencyObjectType")
            {
                return true;
            }

            XmlDocument doc = LoadChangeableFile();
            // 
            XmlElement changeableTest = (XmlElement)doc["FreezablesTest"];
            for (XmlNode testNameNode = changeableTest["TestName"]; testNameNode != null; testNameNode = testNameNode.NextSibling)
            {
                XmlAttribute name = testNameNode.Attributes["Name"];
                XmlAttribute issue = testNameNode.Attributes["IssueType"];
                if (name == null || issue == null)
                {
                    throw new ApplicationException("TestName Element must have attributes Name=, IssueType=");
                }
                if (name.Value == testName)
                {
                    for (XmlNode classNode = testNameNode["Class"]; classNode != null; classNode = classNode.NextSibling)
                    {
                        XmlAttribute className = classNode.Attributes["Name"];
                        if (className == null)
                        {
                            throw new ApplicationException("Class element must have attribute Name=");
                        }
                        if (t.ToString().StartsWith(className.Value))
                        {
                            for (XmlNode propertyNode = classNode["Property"]; propertyNode != null; propertyNode = propertyNode.NextSibling)
                            {
                                XmlAttribute propertyName = propertyNode.Attributes["Name"];
                                if (propertyName == null)
                                {
                                    throw new ApplicationException("Property element must have attribute Name=");
                                }
                                if (propertyName.Value == pName)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

            }
            return false;
        }
        #endregion
    }
}                            
