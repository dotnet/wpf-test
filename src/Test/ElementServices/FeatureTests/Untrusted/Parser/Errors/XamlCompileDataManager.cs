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

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// DataManager for XamlCompile type of testcases.
    /// </summary>
    public class XamlCompileDataManager : XmlDataManager
    {
        /// <summary>
        /// Inherits all the funtionality from XmlDataManager. This class just
        /// sets the data node name to "Compile" i.e. it instructs XmlDataManager to look 
        /// for expected error data in Xml nodes labeled "Compile".
        /// </summary>
        /// <param name="dataFilename"></param>
        public XamlCompileDataManager(string dataFilename) :  base(dataFilename)
        {
            DataNodeName = "Compile";
        }
    }
}
