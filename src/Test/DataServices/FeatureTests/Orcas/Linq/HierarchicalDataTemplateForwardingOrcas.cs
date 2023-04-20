// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    [Test(2, "Styles.HierarchicalDataTemplate", "HierarchicalDataTemplateForwardingOrcas", SecurityLevel=TestCaseSecurityLevel.FullTrust)]
    public class HierarchicalDataTemplateForwardingOrcas : HierarchicalDataTemplateForwarding
    {
        [Variation("XLinq", "ExplicitItemTemplate", "NoForwarding")]
        [Variation("XLinq", "ExplicitItemTemplate", "ItemTemplateForwarded")]
        [Variation("XLinq", "ExplicitItemTemplate", "ItemTemplateSelectorForwarded")]
        [Variation("XLinq", "ExplicitItemTemplate", "ItemTemplateForwardingCancelled")]
        [Variation("XLinq", "ExplicitItemTemplate", "ItemTemplateSelectorForwardingCancelled")]
        [Variation("XLinq", "ImplicitItemTemplate", "NoForwarding")]
        [Variation("XLinq", "ImplicitItemTemplate", "ItemTemplateForwarded")]
        [Variation("XLinq", "ImplicitItemTemplate", "ItemTemplateSelectorForwarded")]
        [Variation("XLinq", "ImplicitItemTemplate", "ItemTemplateForwardingCancelled")]
        [Variation("XLinq", "ImplicitItemTemplate", "ItemTemplateSelectorForwardingCancelled")]
        [Variation("XLinq", "ItemTemplateSelector", "NoForwarding")]
        [Variation("XLinq", "ItemTemplateSelector", "ItemTemplateForwarded")]
        [Variation("XLinq", "ItemTemplateSelector", "ItemTemplateSelectorForwarded")]
        [Variation("XLinq", "ItemTemplateSelector", "ItemTemplateForwardingCancelled")]
        [Variation("XLinq", "ItemTemplateSelector", "ItemTemplateSelectorForwardingCancelled")]
        public HierarchicalDataTemplateForwardingOrcas(string ds, string tT, string fwd)
            : base(ds, tT, fwd, @"Markup\HierarchicalDataTemplateForwardingOrcas.xaml")
        {
        }
    }
}
