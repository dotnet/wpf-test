// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Integration;
using System.ComponentModel;

namespace Microsoft.Test.Integration
{
    /// <summary>
    ///     
    ///</summary>
    public class ActionForXamlInfo
    {

        /// <summary>
        ///     
        ///</summary>        
        public string XamlFile
        {
            get
            {
                return _xamlFile;
            }
            set 
            {
                _xamlFile = value;
            }
        }


        /// <summary>
        ///     
        ///</summary>  
        [DefaultValue("")]
        public string SupportingAssemblies
        {
            get
            {
                return _supportingAssemblies;
            }
            set 
            {
                _supportingAssemblies = value;
            }
        }

        /// <summary>
        ///     
        ///</summary>        
        public string ActionForXaml
        {
            get
            {
                return _actionForXaml;
            }
            set 
            {
                _actionForXaml = value;
            }
        }



        /// <summary>
        ///     
        ///</summary>        
        [DefaultValue("")]
        public string SecurityLevel
        {
            get
            {
                return _securityLevel;
            }
            set 
            {
                _securityLevel = value;
            }
        }




        /// <summary>
        ///     
        ///</summary>        
        [DefaultValue("")]
        public string Disabled
        {
            get
            {
                return _disabled;
            }
            set 
            {
                _disabled = value;
            }
        }


        /// <summary>
        ///     
        ///</summary>        
        [DefaultValue("")]
        public string Priority
        {
            get
            {
                return _priority;
            }
            set 
            {
                _priority = value;
            }
        }



        /// <summary>
        ///     
        ///</summary>        
        [DefaultValue("")]
        public string Area
        {
            get
            {
                return _area;
            }
            set 
            {
                _area = value;
            }
        }

        /// <summary>
        ///     
        ///</summary>        
        public StringCollection SupportFiles
        {
            get
            {
                return _supportFiles;
            }
        }


        string _supportingAssemblies = "";
        string _area = "";
        string _priority = "";
        string _securityLevel = "";
        string _disabled = "";
        string _actionForXaml = "";
        string _xamlFile = "";
        StringCollection _supportFiles = new StringCollection();
        

    }
}
