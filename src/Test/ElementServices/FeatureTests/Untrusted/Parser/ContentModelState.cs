// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Provides ContentModelState for ParserVerifier.cs
 *          
 * Owner: Microsoft 
********************************************************************/

using System;
using Microsoft.Test.Modeling;
namespace Avalon.Test.CoreUI.Parser
{
   
    [Serializable()]
    class ContentModelState : CoreModelState
    {
        public ContentModelState() : base()
        {            
        }

        public ContentModelState(State state) : base(state)
        {
        }

        public string Parent_object
        {
            get { return _parentObject; }
            set { _parentObject = value; }
        }
        public string[] Content_objects
        {
            get { return _contentObjects; }
            set { _contentObjects = value; }
        }
            
        private string _parentObject;
        private string[] _contentObjects;
    }
}
