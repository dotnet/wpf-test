// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Test.Security.Wrappers;

namespace Microsoft.Test.MSBuildEngine
{
    /// <summary>
    /// Parses a Xml file containing error and warning information and 
    /// converts it into a ErrorWarningCode object.
    /// A list of ErrorWarningCode objects is stored. 
    /// </summary>
    class ErrorWarningParser
    {
        Hashtable errorcodes = null;

        #region Internal Methods
        /// <summary>
        /// Constructor that initializes the ErrorWarningCodes list.
        /// </summary>
        internal ErrorWarningParser()
        {
            //_errorcodes = new List<ErrorWarningCode>();
            errorcodes = new Hashtable();
        }

        /// <summary>
        /// Parses a given error file and translates the data to a list of ErrorWarningCodes objects.
        /// </summary>
        /// <param name="errorfilename"></param>
        /// <returns></returns>
        internal bool Parse(string errorfilename)
        {
            if (errorfilename == null)
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "Error file was not specified";
                return false;
            }

            try
            {
                XmlDocumentSW doc = new XmlDocumentSW();

                doc.Load(errorfilename);
                if (doc.DocumentElement.Name != Constants.ErrorRootElement)
                {
                    MSBuildEngineCommonHelper.LogError = "Invalid Error codes file.";
                    return false;
                }

                Parse(doc.DocumentElement);

            }
            catch (XmlException xex)
            {
                MSBuildEngineCommonHelper.DisplayExceptionInformation(xex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorwarningelem"></param>
        internal void Parse(XmlElement errorwarningelem)
        {
            // Get a Xml list of errors and convert them into ErrorWarningCodes objects
            // and them into the _errorcodes list.
            XmlNodeList nodelist = errorwarningelem.GetElementsByTagName(Constants.ErrorElement);

            if (nodelist.Count == 0)
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "No Error descriptions found in ";
            }
            else
            {
                // Set the capacity of the _errorcodes list to the nodelist count.
                // Removed :_errorcodes.Capacity = nodelist.Count;
                for (int i = 0; i < nodelist.Count; i++)
                {
                    ErrorWarningCode ec = new ErrorWarningCode(nodelist[i]);
                    if (errorcodes.Contains(ec.ID))
                    {
                        //string newid = ec.ID + "_Dup" + new Random().Next(1000).ToString();
                        //ec.ID = newid;
                        if (ec.ReferredID != ec.ID)
                        {
                            errorcodes.Add(ec.ReferredID.ToUpper(), ec);
                            continue;
                        }
                    }
                    errorcodes.Add(ec.ID.ToUpper(), ec);
                }
            }

            // Get a Xml list of warnings and convert them into ErrorWarningCodes objects
            // and them into the _errorcodes list.
            nodelist = errorwarningelem.GetElementsByTagName(Constants.WarningElement);
            if (nodelist.Count > 0)
            {
                // Set the capacity of the _errorcodes list to the nodelist count.
                // Removed : _errorcodes.Capacity += nodelist.Count;
                for (int i = 0; i < nodelist.Count; i++)
                {
                    ErrorWarningCode ec = new ErrorWarningCode(nodelist[i]);
                    if (errorcodes.Contains(ec.ID))
                    {
                        //string newid = ec.ID + "_Dup" + new Random().Next(1000).ToString();
                        //ec.ID = newid;
                        if (ec.ReferredID != ec.ID)
                        {
                            errorcodes.Add(ec.ReferredID.ToUpper(), ec);
                            continue;
                        }
                    }
                    errorcodes.Add(ec.ID.ToUpper(), ec);
                }
            }
            else
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "No Warning descriptions found.";
            }

            MSBuildEngineCommonHelper.LogDiagnostic = "Done Parsing Error/Warning file";
        }

        #endregion Internal Methods

        #region Internal Properties

        /// <summary>
        /// Find a particular error code and return the object.
        /// If nothing with the id was found, return null.
        /// </summary>
        /// <param name="errorid"></param>
        /// <returns></returns>
        internal ErrorWarningCode this[string errorid]
        {
            get
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "GetErrorDescription -";
                if (String.IsNullOrEmpty(errorid.ToUpper()))
                {
                    MSBuildEngineCommonHelper.LogDiagnostic = "Input value was null";
                    return null;
                }

                ErrorWarningCode ewc = (ErrorWarningCode)errorcodes[errorid.ToUpper()];

                if (ewc == null)
                {
                    return null;
                }

                return ewc;
            }
        }

        /// <summary>
        /// Additional error/warnings file that is for the specific build problem.
        /// </summary>
        internal string AdditionalErrorWarningsFile
        {
            set
            {
                Parse(value);
            }
        }

        /// <summary>
        /// List of Errors and Warnings
        /// </summary>
        /// <value></value>
        internal Hashtable ListofErrors
        {
            get
            {
                return errorcodes;
            }
        }

        #endregion Internal Properties
    }
}
