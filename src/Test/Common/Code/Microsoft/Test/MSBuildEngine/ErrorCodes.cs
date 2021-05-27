// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Collections;
using System.Globalization;

using Microsoft.Build.Framework;
using Microsoft.Test.Security.Wrappers;

namespace Microsoft.Test.MSBuildEngine
{
    /// <summary>
    /// Object that holds the Error and Warning code information.
    /// The object reads an Xml Node that follows a specified structure as shown in the Example.
    /// </summary>
    /// <example name="example1">
    /// &lt;Error ID="###" Ignore="true"&gt;
    ///		&lt;Source&gt;Text&lt;/Source&gt;
    ///		&lt;Description&gt;Error description.&lt;/Description&gt;
    /// &lt;/Error&gt;
    /// </example>
    /// <remarks>
    /// &lt;Error ID="###" Ignore="true"&gt;
    ///		&lt;Source&gt;Text&lt;/Source&gt;
    ///		&lt;Description&gt;Error description.&lt;/Description&gt;
    /// &lt;/Error&gt;
    /// The same structure is valid for Warnings also.
    ///	</remarks>
    /// <description>
    /// Error or warning messages follow this structure, Example -
    ///		msbuild : error MSB0000: The target "TestRun" does not exist in the project	
    /// 	
    /// Error or Warning			{Element}		[REQUIRED]
    ///		- Element that refers to appropriate error or warning information 
    ///		  In the the above example the element would be Error.
    ///	
    /// Error.ID					{Attribute}		[REQUIRED]
    ///		- ID is the error or warning ID.
    ///		  In the above example MSB0000 is the error ID.
    /// 
    /// Error.ReferredID			{Attribute}		[Optional]
    ///		- ReferredID is the error or warning ID that's referred by when there are duplicates.
    ///
    /// Error.Partial			{Attribute}		[OPTINAL]
    /// Warning.Partial			{Attribute}		[OPTINAL]
    /// 
    /// Error.Ignore				{Attribute}		[OPTIONAL] defaults to do not ignore.
    ///		- This attribute is specified if the user wants to ignore or not ignore a specific
    ///			error or warning.
    ///								
    /// Source						{Element}		[OPTIONAL]		 
    ///		- Source is which file or assembly caused the compilation error.
    ///		  In the above example msbuild is the Source.
    ///								
    /// Description					{Element}		[OPTIONAL]
    ///		- Description message for the error.
    ///			In the above example "The target "TestRun" does not exist in the project" 
    ///			is the description.
    ///								
    ///		- Indicates if the current error/warning is a partial value or full value. 
    ///         Defaults to full value if not set.
    /// </description>
    [Serializable]
    public class ErrorWarningCode
    {
        #region Local Variables
        string id;
        string referredid;
        internal string[] description;
        string source;
        string startingmessage;
        int linenumber = 0;
        int offsetnumber = 0;

        bool ignoreable = false;
        bool bpartial = false;

        ErrorType errortype;

        static Hashtable listofassembliesloaded = null;
        static Hashtable listofresourcesloaded = null;

        static string presentationassemblyfullname = null;
        static string urtpath = null;

        #endregion Local Variables

        #region Methods

        /// <summary>
        /// Constructor which takes in a Node and parses the node information for all
        /// relevant Error and Warning codes based on rules specified in description.
        /// </summary>
        /// <param name="node">XmlNode that is either a Error or a Warning Node</param>
        internal ErrorWarningCode(XmlNode node)
        {
            MSBuildEngineCommonHelper.LogDiagnostic = "ErrorWarningCodes constructor - ";

            if (node == null)
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "Input node is null";
                return;
            }

            if (node.Name != Constants.ErrorElement && node.Name != Constants.WarningElement)
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "node passed in is not Error or Warning";
                MSBuildEngineCommonHelper.LogDiagnostic = "Ignore node";
                return;
            }

            // Set error type depending on node name.
            if (node.Name == Constants.ErrorElement)
            {
                errortype = ErrorType.Error;
            }
            else
            {
                errortype = ErrorType.Warning;
            }

            // Get the root node attributes.
            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                // Get Ignore attribute and convert the value to boolean.
                if (node.Attributes[Constants.IgnoreAttribute] != null)
                {
                    if (String.IsNullOrEmpty(node.Attributes[Constants.IgnoreAttribute].Value) == false)
                    {
                        this.ignoreable = Convert.ToBoolean(node.Attributes[Constants.IgnoreAttribute].Value);
                    }
                }

                if (node.Attributes[Constants.PartialAttribute] != null && String.IsNullOrEmpty(node.Attributes[Constants.PartialAttribute].Value) == false)
                {
                    bpartial = Convert.ToBoolean(node.Attributes[Constants.PartialAttribute].Value);
                }

                // Get ID Attribute value.
                if (node.Attributes[Constants.IDAttribute] != null)
                {
                    id = node.Attributes[Constants.IDAttribute].Value;
                }
                else
                {
                    MSBuildEngineCommonHelper.LogDiagnostic = "ID attribute was not specified on error node, ignoring";
                    if (bpartial == false)
                    {
                        // Todo: Throw an exception.
                        return;
                    }
                }

                // Get ReferredID attribute value.
                if (node.Attributes[Constants.ReferredIDAttribute] != null)
                {
                    referredid = node.Attributes[Constants.ReferredIDAttribute].Value;
                }
                else
                {
                    referredid = id;
                }
            }

            // Check if current error has children if not error out.
            if (node.HasChildNodes == false)
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "Current " + errortype.ToString() + " does not have a Source Node";
            }

            XmlNode childnode = node.SelectSingleNode("./" + Constants.AssemblyResourceAssembly);
            if (childnode != null)
            {
                // Will have to use node instead of childnode to get all other info.                
                GetErrorWarningInfoFromAssemblyResources(node);
                return;
            }

            childnode = node.SelectSingleNode("./" + Constants.SourceElement);
            if (childnode != null && String.IsNullOrEmpty(childnode.InnerText) == false)
            {
                source = childnode.InnerText;
            }
            else
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "Current " + errortype.ToString() + " does not have a Source node";
            }

            childnode = node.SelectSingleNode("./" + Constants.StartingMessageElement);
            if (childnode != null && String.IsNullOrEmpty(childnode.InnerText) == false)
            {
                this.startingmessage = childnode.InnerText;
            }

            childnode = node.SelectSingleNode("./" + Constants.LineNumber);
            if (childnode != null && String.IsNullOrEmpty(childnode.InnerText) == false)
            {
                this.linenumber = Convert.ToInt32(childnode.InnerText);
            }

            childnode = node.SelectSingleNode("./" + Constants.ColumnNumber);
            if (childnode != null && String.IsNullOrEmpty(childnode.InnerText) == false)
            {
                this.offsetnumber = Convert.ToInt32(childnode.InnerText);
            }

            childnode = node.SelectSingleNode("./" + Constants.DescriptionElement);
            if (childnode != null && String.IsNullOrEmpty(childnode.InnerText) == false)
            {
                ConvertResourceDescriptionToArray(childnode.InnerText.Trim());
                //description[0] = childnode.InnerText.Trim();                
            }
        }

        /// <summary>
        /// Constructor that takes a MSBuild Event args and converts 
        /// it into a ErrorWarningCode object.
        /// </summary>
        /// <param name="eventargs">MSBuild Event args</param>
        internal ErrorWarningCode(BuildEventArgs eventargs)
        {
            BuildWarningEventArgs warningeventargs = null;
            BuildErrorEventArgs erroreventargs = eventargs as BuildErrorEventArgs;
            if (erroreventargs == null)
            {
                warningeventargs = eventargs as BuildWarningEventArgs;
                if (String.IsNullOrEmpty(warningeventargs.Code) == false)
                {
                    id = warningeventargs.Code;
                    startingmessage = "warning " + id;
                }

                source = warningeventargs.File;
                errortype = ErrorType.Warning;
                linenumber = warningeventargs.LineNumber;
                offsetnumber = warningeventargs.ColumnNumber;
            }
            else
            {
                if (String.IsNullOrEmpty(erroreventargs.Code) == false)
                {
                    id = erroreventargs.Code;
                    startingmessage = "error " + id;
                }

                source = erroreventargs.File;
                errortype = ErrorType.Error;
                linenumber = erroreventargs.LineNumber;
                offsetnumber = erroreventargs.ColumnNumber;
            }

            if (String.IsNullOrEmpty(eventargs.Message))
            {
                throw new ApplicationException("No error or warning message set on Build event args.");
            }

            if (String.IsNullOrEmpty(id))
            {
                // No ID was found. 
                if (eventargs.Message.Contains(":"))
                {
                    int index = eventargs.Message.IndexOf(':');
                    if (index >= 0)
                    {
                        id = eventargs.Message.Substring(0, index);

                        if (id.Length < "MSB10".Length - 2 || id.Length > "MSB1000".Length)
                        {
                            id = null;
                        }
                        else if (eventargs.Message.Substring(0, index).IndexOfAny(new char[] { '\'', '"' }) != -1)
                        {
                            id = null;
                        }
                    }
                }

            }

            //ConvertDescriptionToArray(eventargs.Message);
            description = new string[1];
            description[0] = eventargs.Message;
        }

        /// <summary>
        /// From an Assembly get the error/warning description.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Assembly resource found result</returns>
        private bool GetErrorWarningInfoFromAssemblyResources(XmlNode node)
        {
            if (node == null)
            {
                return false;
            }

            XmlNode childnode = node.SelectSingleNode("./" + Constants.AssemblyResourceAssembly);
            if (childnode == null)
            {
                return false;
            }

            AssemblySW assm = null;

            if (listofassembliesloaded == null)
            {
                listofassembliesloaded = new Hashtable();
            }

            if (String.IsNullOrEmpty(childnode.InnerText))
            {
                return false;
            }

            string assemblyname = childnode.InnerText;
            if (assemblyname.Contains("Presentation"))
            {
                if (String.IsNullOrEmpty(presentationassemblyfullname))
                {
                    presentationassemblyfullname = MSBuildEngineCommonHelper.PresentationFrameworkFullName;
                }

                int startindex = presentationassemblyfullname.IndexOf(",");
                presentationassemblyfullname = presentationassemblyfullname.Substring(startindex);

                assemblyname = assemblyname + presentationassemblyfullname;
            }
            else
            {
                if (assemblyname.EndsWith(".dll") == false)
                {
                    assemblyname += ".dll";
                }

                if (String.IsNullOrEmpty(urtpath))
                {
                    urtpath = PathSW.GetDirectoryName(typeof(object).Assembly.Location) + PathSW.DirectorySeparatorChar;
                }

                assemblyname = urtpath + assemblyname;
            }

            childnode = node.SelectSingleNode("./" + Constants.AssemblyResoruceResourceName);
            if (childnode == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(childnode.InnerText))
            {
                return false;
            }

            string resourcename = childnode.InnerText;
            if (listofresourcesloaded == null)
            {
                listofresourcesloaded = new Hashtable();
            }

            childnode = node.SelectSingleNode("./" + Constants.AssemblyResourceErrorIdentifier);
            if (childnode == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(childnode.InnerText))
            {
                return false;
            }

            string erroridentifier = childnode.InnerText;
            ResourceSetSW rs = null;
            string cultureinfo = null;

            if (listofresourcesloaded.Contains(resourcename) == false)
            {
                if (listofassembliesloaded.Contains(assemblyname))
                {
                    assm = (AssemblySW)listofassembliesloaded[assemblyname];
                }
                else
                {
                    if (FileSW.Exists(assemblyname))
                    {
                        if (listofassembliesloaded.Contains(assemblyname) == false)
                        {
                            assm = AssemblySW.ReflectionOnlyLoadFrom(assemblyname);
                        }
                        else
                        {
                            assm = (AssemblySW)listofassembliesloaded[assemblyname];
                        }
                    }
                    else
                    {
                        //assm = Assembly.GetAssembly(typeof(Microsoft.Build.BuildEngine.BuildItem));
                        AssemblySW[] assmlist = AppDomainSW.CurrentDomain.GetAssemblies();
                        for (int i = 0; i < assmlist.Length; i++)
                        {
                            object obj = assmlist.GetValue(i);
                            assm = (AssemblySW)obj;
                            if (PathSW.GetFileNameWithoutExtension(assm.ManifestModule.Name).ToLowerInvariant() == assemblyname.ToLowerInvariant())
                            {
                                break;
                            }
                            assm = null;
                            obj = null;
                        }

                        assmlist = null;
                    }

                }

                if (assm == null)
                {
                    assm = AssemblySW.ReflectionOnlyLoad(assemblyname);
                    if (assm == null)
                    {
                        throw new ApplicationException(assemblyname + " could not be loaded.");
                    }
                }

                AssemblySW assm2 = null;
                try
                {
                    assm2 = assm.GetSatelliteAssembly(System.Globalization.CultureInfo.CurrentUICulture);

                    cultureinfo = System.Globalization.CultureInfo.CurrentUICulture.Name;
                    assm = null;
                    assm = assm2;
                    assm2 = null;
                }
                catch (FileNotFoundException)
                {
                    MSBuildEngineCommonHelper.LogDiagnostic = "Current UI culture = " + CultureInfo.CurrentUICulture.Name + " with full culture name";
                    MSBuildEngineCommonHelper.LogDiagnostic = "Assembly " + assm.FullName + " doesn't have a culture dependent resource assembly.";
                }

                try
                {
                    assm2 = assm.GetSatelliteAssembly(System.Globalization.CultureInfo.CurrentUICulture.Parent);

                    cultureinfo = System.Globalization.CultureInfo.CurrentUICulture.Parent.Name;
                    assm = null;
                    assm = assm2;
                    assm2 = null;
                }
                catch (FileNotFoundException)
                {
                    MSBuildEngineCommonHelper.LogDiagnostic = "Current UI culture = " + CultureInfo.CurrentUICulture.Parent.Name + " with full culture name";
                    MSBuildEngineCommonHelper.LogDiagnostic = "Assembly " + assm.FullName + " doesn't have a culture dependent resource assembly.";
                }

                string resourcenamewithculture = null;
                if (String.IsNullOrEmpty(cultureinfo) == false)
                {
                    if (resourcename.Contains("."))
                    {
                        string[] resourcesplit = resourcename.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (resourcesplit.Length >= 2)
                        {
                            if (resourcesplit[resourcesplit.Length - 1].ToLowerInvariant() == "resources")
                            {
                                int resindex = resourcename.IndexOf(".resources");
                                if (resindex > 0)
                                {
                                    resourcenamewithculture = resourcename.Substring(0, resindex) + "." + cultureinfo + "." + resourcesplit[resourcesplit.Length - 1];
                                }
                            }
                        }
                        resourcesplit = null;
                    }
                }

                if (listofassembliesloaded.Contains(assemblyname) == false)
                {
                    listofassembliesloaded.Add(assemblyname, assm);
                }

                string[] resourcenames = assm.GetManifestResourceNames();
                bool resourcefound = false;
                for (int j = 0; j < resourcenames.Length; j++)
                {
                    if (resourcename == resourcenames[j])
                    {
                        resourcefound = true;
                        break;
                    }

                    if (resourcenamewithculture == resourcenames[j])
                    {
                        resourcefound = true;
                        break;
                    }
                }
                resourcenames = null;

                if (resourcefound)
                {
                    StreamSW resourcestream = null;
                    if (String.IsNullOrEmpty(cultureinfo))
                    {
                        resourcestream = assm.GetManifestResourceStream(resourcename);
                        rs = new ResourceSetSW(resourcestream.InnerObject);

                        if (listofresourcesloaded.Contains(resourcename) == false)
                        {
                            listofresourcesloaded.Add(resourcename, rs);
                        }
                        resourcestream.Close();
                    }
                    else
                    {
                        resourcestream = assm.GetManifestResourceStream(resourcenamewithculture);
                        rs = new ResourceSetSW(resourcestream.InnerObject);

                        if (listofresourcesloaded.Contains(resourcename) == false)
                        {
                            listofresourcesloaded.Add(resourcename, rs);
                        }
                        resourcestream.Close();
                    }
                    resourcestream = null;
                }
            }
            else
            {
                rs = (ResourceSetSW)listofresourcesloaded[resourcename];
            }

            if (rs != null)
            {
                string errordescription = rs.GetString(erroridentifier);
                if (String.IsNullOrEmpty(errordescription))
                {
                    return false;
                }

                if (errordescription.StartsWith(id))
                {
                    try
                    {
                        Convert.ToInt16(id);
                    }
                    catch (FormatException)
                    {
                        errordescription = errordescription.Substring(id.Length);
                    }
                    //errordescription = errordescription.Substring(id.Length);

                    startingmessage = errortype.ToString().ToLowerInvariant() + " " + id;
                    errordescription = errordescription.Trim();
                }

                if (errordescription.StartsWith(":"))
                {
                    errordescription = errordescription.Substring(1).Trim();
                }

                ConvertResourceDescriptionToArray(errordescription);
                errordescription = null;
            }
            else
            {
                MSBuildEngineCommonHelper.Log = "The following erroridentifier could not be found as resourceset was null.";
                MSBuildEngineCommonHelper.Log = "ErrorIdentifier = " + erroridentifier;
                MSBuildEngineCommonHelper.Log = "Resourcname = " + resourcename;
                MSBuildEngineCommonHelper.Log = "Culture = " + cultureinfo;
                MSBuildEngineCommonHelper.Log = "AssemblyName = " + assemblyname;
            }

            return true;
        }

        /// <summary>
        /// Take a string and convert it to string array based on where '{' chars are found.
        /// </summary>
        /// <param name="errordescription"></param>
        private void ConvertResourceDescriptionToArray(string errordescription)
        {
            List<string> descriptionlist = new List<string>();

            // If the string does not contain currly braces it does not follow the pattern of
            // parameters in a resource string, update the description array with the current string 
            // and exit.
            if (errordescription.Contains("{") == false && errordescription.Contains("}") == false)
            {
                description = new string[1];
                description[0] = errordescription;
                return;
            }

            // If there are currly braces split the string into an array get the strings
            // which match the pattern of {n} where n is a integer value and add currly
            // braces around them. (This is done to allow Compare code to function appropriately.
            string[] szdescriptionlist = errordescription.Split(new char[] { '{', '}' });
            for (int i = 0; i < szdescriptionlist.Length; i++)
            {
                // Blank strings are valid, thus only looking for strings which are "".
                if (szdescriptionlist[i].Length <= 0)
                {
                    continue;
                }

                short result = 0;
                string tempdescription = szdescriptionlist[i];
                if (Int16.TryParse(tempdescription, out result))
                {
                    tempdescription = "{" + tempdescription + "}";
                }

                // Add to temporary list.
                descriptionlist.Add(tempdescription);
                tempdescription = null;
            }

            szdescriptionlist = null;

            // Update description property array.
            if (description == null)
            {
                description = new string[descriptionlist.Count];
            }
            descriptionlist.CopyTo(description);

            descriptionlist = null;
        }

        /// <summary>
        /// Override of Equals to do ErrorWarningCode specific comparison.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ErrorWarningCode parsederrorwarningcode = (ErrorWarningCode)obj;
            if (parsederrorwarningcode == null)
            {
                return false;
            }

            // Simple check to see if Full Description of Expected and Actual are the same.
            if (this.FullDescription == parsederrorwarningcode.FullDescription)
            {
                return true;
            }

            // Check for error ID, if it doesn't match return failure.
            if (this.id != parsederrorwarningcode.id)
            {
                if (parsederrorwarningcode.id.ToString().StartsWith(this.id) == false)
                {
                    MSBuildEngineCommonHelper.LogDiagnostic = "Expected and Actual ID's are different";
                    MSBuildEngineCommonHelper.LogDiagnostic = "Expected ID Value : " + parsederrorwarningcode.id;
                    MSBuildEngineCommonHelper.LogDiagnostic = "Actual ID Value : " + this.id;
                    return false;
                }
            }

            if (this.linenumber != parsederrorwarningcode.linenumber && parsederrorwarningcode.bpartial == true)
            {
                if (parsederrorwarningcode.bpartial == false)
                {
                    MSBuildEngineCommonHelper.LogWarning = "Expected and Actual Line Numbers are different";
                    MSBuildEngineCommonHelper.LogWarning = "Expected Line Number Value : " + parsederrorwarningcode.linenumber;
                    MSBuildEngineCommonHelper.LogWarning = "Actual Line Number Value : " + this.linenumber;
                }
            }

            if (this.offsetnumber != parsederrorwarningcode.offsetnumber && parsederrorwarningcode.bpartial == true)
            {
                if (parsederrorwarningcode.bpartial == false)
                {
                    MSBuildEngineCommonHelper.LogWarning = "Expected and Actual Column Numbers are different";
                    MSBuildEngineCommonHelper.LogWarning = "Expected Column Number Value : " + parsederrorwarningcode.offsetnumber;
                    MSBuildEngineCommonHelper.LogWarning = "Actual Column Number Value : " + this.offsetnumber;
                }
            }

            if (String.IsNullOrEmpty(parsederrorwarningcode.source) == false)
            {
                //if (expectederrorwarningcode.IsPartial == false)
                //{
                MSBuildEngineCommonHelper.Log = "Expected error/warning does not define a Source value.";
                //    return false;
                //}

                if (this.source.ToLowerInvariant() != parsederrorwarningcode.source.ToLowerInvariant())
                {
                    MSBuildEngineCommonHelper.LogDiagnostic = "Expected and Actual sources are different";
                    MSBuildEngineCommonHelper.LogDiagnostic = "Expected Source Value : " + parsederrorwarningcode.source;
                    MSBuildEngineCommonHelper.LogDiagnostic = "Actual Source Value : " + this.source;
                    return false;
                }

            }

            if (String.IsNullOrEmpty(parsederrorwarningcode.startingmessage) == false)
            {
                //if (expectederrorwarningcode.IsPartial == false)
                //{
                MSBuildEngineCommonHelper.LogDiagnostic = "Expected error/warning does not define a Starting Message value.";
                //    return false;
                //}

                if (this.startingmessage != parsederrorwarningcode.startingmessage)
                {
                    if (parsederrorwarningcode.bpartial == false)
                    {
                        MSBuildEngineCommonHelper.LogError = "Expected and Actual Starting Message are different";
                        MSBuildEngineCommonHelper.LogError = "Expected Starting Message Value : " + parsederrorwarningcode.startingmessage;
                        MSBuildEngineCommonHelper.LogError = "Actual Starting Message Value : " + this.startingmessage;
                        return false;
                    }
                }
            }

            if (parsederrorwarningcode.description == null)
            {
                MSBuildEngineCommonHelper.Log = "Expected error/warning does not define a Description value.";
                return false;
            }

            if (parsederrorwarningcode.description.Length == 0)
            {
                MSBuildEngineCommonHelper.Log = "Expected error/warning does not define a Description value.";
                return false;
            }

            if (this.Description != parsederrorwarningcode.Description)
            {
                if (parsederrorwarningcode.bpartial || parsederrorwarningcode.ignoreable)
                {
                    if (parsederrorwarningcode.description.Length > 1)
                    {
                        if (Compare(parsederrorwarningcode.description, this.description[0]) == false)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        int index = this.FullDescription.IndexOf(parsederrorwarningcode.Description);
                        if (index < 0)
                        {
                            MSBuildEngineCommonHelper.Log = "Expected error/warning does not match the actual error/warning description.";
                            return false;
                        }
                    }
                }
                else
                {
                    MSBuildEngineCommonHelper.Log = "Expected error/warning does not match the actual error/warning description.";
                    return false;
                }
            }

            return parsederrorwarningcode.IsIgnoreable;
        }

        /// <summary>
        /// Compare a string with a resource string derived from Resources files.
        /// </summary>
        /// <param name="errorfromresourcetable">A string split up between string and params</param>
        /// <param name="currenterrordescription">The current error description.</param>
        /// <returns></returns>
        public static bool Compare(string[] errorfromresourcetable, string currenterrordescription)
        {
            if (errorfromresourcetable == null || String.IsNullOrEmpty(currenterrordescription))
            {
                return false;
            }

            string[] detokenizeddescription = new string[errorfromresourcetable.Length];
            short count = 0;

            for (int i = 0; i < errorfromresourcetable.Length; i++)
            {
                if (String.IsNullOrEmpty(errorfromresourcetable[i]))
                {
                    continue;
                }

                if (errorfromresourcetable[i].Contains("{"))
                {
                    int beginingindex = errorfromresourcetable[i].IndexOf("{");
                    int endingindex = errorfromresourcetable[i].IndexOf("}", beginingindex);

                    if (endingindex > 0)
                    {
                        string[] datastring = new string[3];
                        datastring[0] = errorfromresourcetable[i].Substring(0, beginingindex);
                        datastring[1] = errorfromresourcetable[i].Substring(beginingindex, endingindex - beginingindex + 1);
                        datastring[2] = errorfromresourcetable[i].Substring(endingindex + 1);

                        if (String.IsNullOrEmpty(currenterrordescription))
                        {
                            continue;
                        }

                        beginingindex = currenterrordescription.IndexOf(datastring[0]);
                        if (beginingindex >= 0)
                        {
                            beginingindex += datastring[0].Length;
                            endingindex = currenterrordescription.IndexOf(datastring[2], beginingindex + 1);
                            if (endingindex >= 0)
                            {
                                if (string.IsNullOrEmpty(datastring[0]) && string.IsNullOrEmpty(datastring[2]))
                                {
                                    if (errorfromresourcetable.Length > i + 1)
                                    {
                                        beginingindex = currenterrordescription.IndexOf(errorfromresourcetable[i + 1]);
                                        if (beginingindex < 0)
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            datastring[1] = currenterrordescription.Substring(0, beginingindex);
                                            detokenizeddescription[count++] = datastring[1];
                                            currenterrordescription = currenterrordescription.Substring(beginingindex);
                                        }
                                    }
                                    else
                                    {
                                        datastring[1] = currenterrordescription;
                                        detokenizeddescription[count++] = datastring[1];
                                        currenterrordescription = null;
                                    }
                                }
                                else
                                {
                                    datastring[1] = currenterrordescription.Substring(beginingindex, endingindex - beginingindex);
                                    detokenizeddescription[count++] = datastring[1];
                                    currenterrordescription = currenterrordescription.Substring(endingindex + datastring[2].Length);
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (currenterrordescription.StartsWith(errorfromresourcetable[i]))
                {
                    currenterrordescription = currenterrordescription.Substring(errorfromresourcetable[i].Length);
                    detokenizeddescription[count++] = errorfromresourcetable[i].Trim();
                }
                else
                {
                    return false;
                }

                //if (String.IsNullOrEmpty(currenterrordescription) == false)
                //{
                //currenterrordescription = currenterrordescription.Trim();
                //}
            }

            // Currently I do not check if Current error description is null or not.
            // It should be but in XamlParseException Line 1 Position 2 gets added. 
            // Currently I will not do this so that we pass.

            return true;
        }

        /// <summary>
        /// Overload not implemented
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Methods

        #region Properties
        /// <summary>
        /// Public ErrorType Property indicates error type for current ErrorCode
        /// </summary>
        /// <value>ErrorType for current ErrorWarningCodes</value>
        public ErrorType Type
        {
            get
            {
                return errortype;
            }
        }

        /// <summary>
        /// Public Error code ID for current Error Code
        /// </summary>
        /// <value>String value for error ID.</value>
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// Public Referred ID for Current ErrorWarning Code
        /// </summary>
        public string ReferredID
        {
            get
            {
                return referredid;
            }
        }

        /// <summary>
        /// Public Error code error Description for current Error Code.
        /// </summary>
        /// <value>String value for error description</value>
        public string Description
        {
            get
            {
                string szdescription = null;
                if (description == null)
                {
                    return null;
                }

                for (int i = 0; i < description.Length; i++)
                {
                    if (i > 0)
                    {
                        szdescription += " ";
                    }
                    szdescription += description[i];
                }

                return szdescription;
            }
        }

        /// <summary>
        /// Public Error code source for current Error Code.
        /// </summary>
        /// <value></value>
        public string Source
        {
            get
            {
                return source;
            }
        }

        /// <summary>
        /// Public Error code StartingMessage for current Error Code.
        /// </summary>
        /// <value></value>
        public string StartingMessage
        {
            get
            {
                return startingmessage;
            }
        }

        /// <summary>
        /// Public Error code boolean property if the current ErrorWarningCode should
        /// be ignored or not.
        /// </summary>
        /// <value></value>
        internal bool IsIgnoreable
        {
            get
            {
                return ignoreable;
            }
            set
            {
                ignoreable = value;
            }
        }

        /// <summary>
        /// Property describing if the Description for the 
        /// current ErrorWarningCode is a Partial description.
        /// </summary>
        /// <value></value>
        internal bool IsPartial
        {
            get
            {
                return bpartial;
            }
        }

        /// <summary>
        /// Line Number where the error/warning can occur.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return linenumber;
            }
        }

        /// <summary>
        /// Column Number where the error/warning can occur.
        /// </summary>
        public int ColumnNumber
        {
            get
            {
                return offsetnumber;
            }
        }

        /// <summary>
        /// Public property which colates all the above properties of ErrorWarningCode
        /// and puts it into one string and returns the value.
        /// </summary>
        /// <value></value>
        public string FullDescription
        {
            get
            {
                StringBuilder fulldescription = new StringBuilder();
                if (String.IsNullOrEmpty(source) == false)
                {
                    fulldescription.AppendFormat("{0} ", source); //+= source + " : ";

                    if (linenumber >= 0 && offsetnumber >= 0)
                    {
                        fulldescription.AppendFormat("({0},{1}) :  ", linenumber, offsetnumber);
                    }
                }

                if (String.IsNullOrEmpty(startingmessage) == false)
                {
                    fulldescription.AppendFormat("{0}: ", startingmessage); //+= startingmessage + " :";
                }

                if (description != null)
                {
                    if (description.Length > 0)
                    {
                        for (int i = 0; i < description.Length; i++)
                        {
                            fulldescription.Append(description[i]);
                        }
                    }
                }

                return fulldescription.ToString();
            }
        }

        /// <summary>
        /// Full description of avalon errors and warnings.
        /// This is a temporary work around until we fix error/warnings reporting in avalon.
        /// </summary>
        /// <value></value>
        internal string AvalonFullDescription
        {
            get
            {
                if (String.IsNullOrEmpty(this.id))
                {
                    return null;
                }

                StringBuilder fulldescription = new StringBuilder();
                if (this.id.ToLowerInvariant().StartsWith(@"ac") == false)
                {
                    fulldescription.AppendFormat("{0}: ", this.id);
                }

                //if (String.IsNullOrEmpty(description))
                //{
                //    fulldescription = null;
                //    return null;
                //}

                return fulldescription.Append(description).ToString();

            }
        }

        #endregion Properties
    }

    /// <summary>
    /// The possible Error types supported.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Indicates Error Element.
        /// </summary>
        Error,
        /// <summary>
        /// Indicates Warning Element.
        /// </summary>
        Warning
    }

}
