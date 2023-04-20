// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public abstract class XamlDomNode
    {
        public virtual int StartLineNumber
        {
            get { return _startLineNumber; }
            set { CheckSealed(); _startLineNumber = value; }
        }

        public virtual int StartLinePosition
        {
            get { return _startLinePosition; }
            set { CheckSealed(); _startLinePosition = value; }
        }
        public virtual int EndLineNumber
        {
            get { return _endLineNumber; }
            set { CheckSealed(); _endLineNumber = value; }
        }
        public virtual int EndLinePosition
        {
            get { return _endLinePosition; }
            set { CheckSealed(); _endLinePosition = value; }
        }
        public virtual bool IsSealed { get { return _isSealed; } }

        public virtual void Seal()
        {
            _isSealed = true;
        }

        protected void CheckSealed()
        {
            if (IsSealed)
            {
                throw new NotSupportedException(String.Format("The {0} is sealed.", this.GetType().Name));
            }
        }

        private bool _isSealed;
        private int _startLineNumber;
        private int _startLinePosition;
        private int _endLineNumber;
        private int _endLinePosition;
    }
}
