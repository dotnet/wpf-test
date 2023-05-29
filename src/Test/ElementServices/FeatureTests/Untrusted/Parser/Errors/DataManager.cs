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

using System;
using System.Collections;

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// A class that manages error data (master).
    /// Please see the comments on class Engine to know how Engine drives this class.
    /// 
    /// Implementors of this abstract class should provide the following functionality:
    /// 1. In Load(), read the master error data from error file into in-memory records
    /// 2. Upon a request from Engine through ReadNextRecord(), return a data record.
    /// 3. Upon a request from Engine through ChangeCurrentRecord(), update the master error 
    ///    record with new data supplied by Engine.
    /// 4. Upon a request from Engine through PersistChanges(), save the in-memory records
    ///    back to the error file.
    /// </summary>
    public abstract class DataManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFilename"></param>
        protected DataManager(string dataFilename)
        {
            _dataFilename = dataFilename;
            Load();
        }

        /// <summary>
        /// Load the error data from file into records.
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Hashtable ReadNextRecord(); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newErrData"></param>
        public abstract void ChangeCurrentRecord(Hashtable newErrData);

        /// <summary>
        /// 
        /// </summary>
        public abstract void PersistChanges();

        /// <summary>
        /// Name of the data file from where to load the error data. 
        /// </summary>
        protected string _dataFilename = null;
    }
}
