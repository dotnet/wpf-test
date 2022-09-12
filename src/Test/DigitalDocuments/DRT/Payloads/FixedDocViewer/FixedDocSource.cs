// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace D2Payloads
{
    using System;
    using System.ComponentModel; 
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Documents;



    /// <summary>
    /// 
    /// </summary>
    [TypeConverter(typeof(FixedDocSourceConverter))]
    public abstract class FixedDocSource 
    {
        //--------------------------------------------------------------------
        //
        // Ctors
        //
        //---------------------------------------------------------------------

        #region Ctors
        protected FixedDocSource()
        {
        }
        #endregion Ctors
        
        //--------------------------------------------------------------------
        //
        // Public Methods
        //
        //---------------------------------------------------------------------

        #region Public methods
        public abstract InstanceDescriptor ToInstanceDescriptor();
        #endregion Public Methods

        //--------------------------------------------------------------------
        //
        // Public Properties
        //
        //---------------------------------------------------------------------

        #region Public Properties
        public abstract IDocumentPaginatorSource FixedDoc
        {
            get;
        }

        public abstract Uri Source
        {
            get;
        }
        #endregion Public Properties

        //--------------------------------------------------------------------
        //
        // Public Events
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Protected Methods
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Internal Methods
        //
        //---------------------------------------------------------------------


        //--------------------------------------------------------------------
        //
        // private Properties
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Private Methods
        //
        //---------------------------------------------------------------------

        #region Private Methods
        #endregion Private Methods

        //--------------------------------------------------------------------
        //
        // Private Fields
        //
        //---------------------------------------------------------------------
        #region Private Fields
        #endregion Private Fields
    }
}
