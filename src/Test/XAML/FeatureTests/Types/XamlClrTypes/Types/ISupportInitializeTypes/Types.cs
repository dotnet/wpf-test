// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.Test.Xaml.Types.ISupportInitializeTypes
{
    public class IsupportInitializeParent : ISupportInitialize
    {
        public SimpleIsupportInitialize ISIProperty { get; set; }

        public bool BeginInitCalled = false;
        public bool EndInitCalled = false;

        #region ISupportInitialize Members

        public void BeginInit()
        {
            BeginInitCalled = true;
        }

        public void EndInit()
        {
            EndInitCalled = true;
        }

        #endregion
    }

    public class SimpleIsupportInitialize : ISupportInitialize
    {
        public int IntProperty { get; set; }

        public bool BeginInitCalled = false;
        public bool EndInitCalled = false;

        #region ISupportInitialize Members

        public void BeginInit()
        {
            BeginInitCalled = true;
        }

        public void EndInit()
        {
            EndInitCalled = true;
        }

        #endregion
    }
}
