// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Integration
{
    // Added to unbreak build.  This may need to be re-written, but simply disabling warning for now.
    #pragma warning disable 1691
    #pragma warning disable 3028
    /// <summary>
    /// Workaround for WPF 

    public class VariationItemCollection : CustomList<VariationItem>
    {
    }

    /// <summary>
    /// Workaround for WPF 

    public class StringCollection : CustomList<String>
    {
    }

    /// <summary>
    /// Workaround for WPF 

    public class IVariationGeneratorCollection : CustomList<IVariationGenerator>
    {
        
    }

    /// <summary>
    /// Workaround for WPF 

    public class StorageItemCollection : CustomList<StorageItem>
    {
    }

    /// <summary>
    /// Workaround for WPF 

    public class ContentItemCollection : CustomList<ContentItem>
    {
    }

    /// <summary>
    /// Workaround for WPF 

    public class AssemblyDescCollection : CustomList<AssemblyDesc>
    {
    }

    #pragma warning restore 3028
    #pragma warning restore 1691
}
