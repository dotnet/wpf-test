// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Baml.Utilities
{
    public class BamlString
    {
        public BamlString(bool isLocalizable, string attributeId, string parentUid, string strValue)
        {
            IsLocalizable = isLocalizable;
            AttributeID = attributeId;
            ParentUid = parentUid;
            Value = strValue;
        }

        public bool IsLocalizable { get; private set; }
        public string AttributeID { get; private set; }
        public string ParentUid { get; private set; }
        public string Value { get; private set; }
    }
}
