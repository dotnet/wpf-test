// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// This class is Wrapper for AutoData class. 
    /// </summary>
    /// <remarks>
    /// This AutoData is a class that is located on AutoData.dll (developed by Globalization team)
    /// </remarks>
    public class AutoData
    {               
        # region  Constructor

        

        /// <summary>
        /// Contructor Method
        /// </summary>        
        /// <remarks>
        /// This contructor reads from the currentDomain a stored variable called "IsLabToolsSupported" that is a boolean.
        /// If IsLabToolsSupported is true, AutoData.dll is used <para />
        /// If IsLabToolsSupported is false, we create a mock up to use<para />
        /// Default value is true <para />
        /// </remarks>
        public AutoData()
        {
            bool autoDataSupport = true;

            object var = AppDomain.CurrentDomain.GetData("IsLabToolsSupported");
            if ( var != null) 
            {
                autoDataSupport = (bool)var;
            }
                

            this.UseAutoData = autoDataSupport;                        
        }

        #endregion Constructor

        # region  PublicRegion

        /// <summary>
        /// Return the value if AutoData.Dll is used
        /// </summary>
        public bool IsAutoDataLoaded 
        {
            get 
            {
                return this._adType != null;
            }
        }

        /// <summary>
        /// Set and Retrieve if you want to use autodata class.
        /// </summary>
        /// <remarks>
        /// If IsAutoDataLoaded is false always the value for this will be False. If you load autodata but you don't want to use
        /// it, just set thid property to false.
        /// </remarks>
        public bool UseAutoData
        {
            get 
            {
                return this._useAutoData;
            }

            set 
            {
                // _useAutoData can be set to false after it was previously set to
                // true, because in debugging scenarios you might want to first
                // use AutoData for real, and then not use it.  AutoData.dll can 
                // remain loaded either way.

                this._useAutoData = value;

                if (this._useAutoData && !this.IsAutoDataLoaded)
                {
                    _InitAutoData();
                }
                
            }
        }

        /// <summary>
        /// Wrapper for GetTestString method on AutoData
        /// </summary>
        /// <param name="index">Parameter needed for AutoData.  Index=0 it will return a mixed string.</param>
        /// <returns></returns>
        /// <remarks>If IsAutoDataLoaded=true and UseAutoData=true, this method will act as proxy for the real
        /// AutoData. If not will just return a simple string
        /// </remarks>
        public string GetTestString(int index)
        {
            string str = String.Empty;

            if ( (this.IsAutoDataLoaded) && (this.UseAutoData))
            {

                try 
                {
                    
                    int mod_val = -1;
                
                    string strMerged = String.Empty;
                    while ((index = Math.DivRem(index,10,out mod_val)) > 0)
                    {
                        str = this._GetTestString(mod_val) + str;
                    }
                    str = this._GetTestString(mod_val) + str;                    
                    
                    
                }
                catch
                {
                    CoreLogger.LogStatus("Exception throw by AutoData");
                    str = this._GetLocalString(index);
                }
            }
            else
            {
                str = this._GetLocalString(index);
                            
            }

            CoreLogger.LogStatus("String from AutoData:");
            CoreLogger.LogStatus("<AutoDataString>" + str + "</AutoDataString>");
            return str;
        }


        # endregion  PublicRegion

           /// <summary>
           /// Makes the call to autodata GetTestString
           /// </summary>
           /// <param name="index"></param>
           /// <returns>string from AutoData</returns>
        private string _GetTestString(int index)
        {
            object o = this._adType.InvokeMember( "GetTestString", 
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.InvokeMethod | BindingFlags.Public, 
                null, this._adObject, new object[] {index} );
                    
            return (string)o;
        }

        /// <summary>
        /// Gets the string from the local string Table
        /// </summary>
        /// <param name="index"></param>
        /// <returns>an local string</returns>
        private string _GetLocalString(int index)
        {
            if ( (index>= this._localStrings.Length))
            {
                //If there is no index we create a string using for exaple 15 index. string 1 and 5 
                int mod_val = -1;
                
                string strMerged = String.Empty;
                while ((index = Math.DivRem(index,10,out mod_val)) > 0)
                {
                    strMerged = this._localStrings[mod_val] + strMerged;
                }
                strMerged = this._localStrings[mod_val] + strMerged;
                return strMerged;
            }
            return this._localStrings[index];
        }
                
                
        /// <summary>
        /// This method load AutoData.dll and creates an instance of AutoData.Data object.
        /// </summary>
        /// <remarks>
        /// This class only is called from the constructor 
        /// </remarks>
        private void _InitAutoData()
        {
            try
            {
                _adAsm = Assembly.Load("AutoData, Version=1.1.0.0, Culture=neutral, PublicKeyToken="); // +  Microsoft.Internal.BuildInfo.WCP_PUBLIC_KEY_TOKEN);
                this._adType = _adAsm.GetType("AutoData.Extract",false,true);
                        
            }
            catch
            {
                _adAsm = null;
                _adType = null;
                _adObject = null;
                this.UseAutoData = false;
            }
        }


        /// <summary>
        /// Reference to the assembly that contains AutoData.dll 
        /// </summary>
        private Assembly _adAsm = null;
                
        /// <summary>
        /// Reference of AutoData.Data class
        /// </summary>
        private object _adObject = null;


        /// <summary>
        /// Reference of AutoData.Data type
        /// </summary>
        private Type _adType = null;


        /// <summary>
        /// Reference that knows if the user want to use AutoData
        /// </summary>
        private bool _useAutoData = false;

        /// <summary>
        /// Local strings table for look up.
        /// </summary>
        private string[] _localStrings = {"Heillo","1234567890","abcdefghijklmnopqrstuvwxyz",@"!@#$%^&*()_+-=//.,as{}[]`~","a1a1a","12","aaaaaa","asdasdasdasd","!@#@!#$%","~{}","qwertttreew","     "};
    }
}
