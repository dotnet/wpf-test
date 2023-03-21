// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using Microsoft.Test.Discovery;

namespace Microsoft.Test.Annotations
{
    using Microsoft.Test;

    [Test(0, "VersionTolerance", "AnnotationStore_30")]
    public class AnnotationStore_30 : AVersionToleranceTest
    {
        public AnnotationStore_30()
            : base("Master_Annotations_30.xml")
        { }
    }

    [Test(0, "VersionTolerance", "AnnotationStore_35")]
    public class AnnotationStore_35 : AVersionToleranceTest
    {
        public AnnotationStore_35()
            : base("Master_Annotations_35.xml")
        { }
    }

    [Test(0, "VersionTolerance", "AnnotationStore_40")]
    public class AnnotationStore_40 : AVersionToleranceTest
    {
        public AnnotationStore_40()
            : base("Master_Annotations_40.xml")
        { }
    }
}

