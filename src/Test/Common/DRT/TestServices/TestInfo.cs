// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------------
//
//
// Description: Test Framework for Document Reading Platform 
// 
//
//---------------------------------------------------------------------------

using System;

namespace DRT
{
    /// <summary>
    /// A convience class to bridge attribute information from a class marked with
    /// the Test attribute to DrtTestSuite.
    /// </summary>
    internal class TestInfo
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// 
        /// If the class is not marked with Test attribute the class name will be used.
        /// </summary>
        /// <param name="test">A type.</param>
        public TestInfo(Type test)
        {
            TestAttribute attrib = TestServices.GetFirstAttribute(typeof(TestAttribute), test) as TestAttribute;
            if (attrib != null)
            {
                _attrib = attrib;
            }
            else
            {
                _attrib = new TestAttribute(test.Name, string.Empty);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Name of the test.
        /// </summary>
        public string Name
        {
            get { return _attrib.Name; }
        }

        /// <summary>
        /// Contact's alias for the test.
        /// </summary>
        public string Contact
        {
            get { return _attrib.Contact; }
        }
        #endregion

        #region Private Fields
        private TestAttribute _attrib;
        #endregion
    }
}
