// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Integration
{
    ///<summary>
    ///</summary>
    public interface ITestContract
    {
        ///<summary>
        ///</summary>
        string Area
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        TestCaseSecurityLevel SecurityLevel
        {
            get;
            set;
        }

        
        ///<summary>
        ///</summary>
        StorageItemCollection Input
        {
            get;
        }

        ///<summary>
        ///</summary>
        StorageItemCollection Output
        {
            get;
        }

        ///<summary>
        ///</summary>
        string Description
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        string Title
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        int Priority
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        StringCollection SupportFiles
        {
            get;
        }


        /// <summary>
        /// 
        /// </summary>
        bool Disabled
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        int Timeout
        {
            get;
            set;
        }

    }    
}
