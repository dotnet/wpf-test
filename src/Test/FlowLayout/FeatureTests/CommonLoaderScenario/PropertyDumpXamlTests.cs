// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Layout;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace FlowLayout.FeatureTests.CommonLoaderScenario
{
    [Test(0, "TextBlock", "TextBlockVariousProperties", TestParameters = "content=Text_Properties.xaml")]
    [Test(0, "TextBlock", "TextBlockTextTrimmingProperties", TestParameters = "content=Text_TextTrimming.xaml")]
    [Test(0, "TextBlock", "TextBlockHorizontalAlignment", TestParameters = "content=Text_HorizontalAlignment.xaml")]
    [Test(0, "TextBlock", "TextBlockVerticalAlignment", TestParameters = "content=Text_VerticalAlignment.xaml")]
    [Test(0, "TextBlock", "TextBlockFontSize", TestParameters = "content=Text_FontSize.xaml")]
    [Test(1, "TextBlock", "TextBlockWidth", TestParameters = "content=Text_Width.xaml")]
    [Test(2, "TextBlock", "TextBlockMinWidthMaxWidth", TestParameters = "content=Text_MinWidth_MaxWidth.xaml")]
    [Test(3, "TextBlock", "TextBlockTextTrimmingNestedNonNested", TestParameters = "content=Text_TextTriming_NestedNonNested.xaml")]
    [Test(3, "TextBlock", "TextBlockTextWrapNestedNonNested", TestParameters = "content=Text_TextWrap_NestedNonNested.xaml")]
    [Test(2, "TextBlock", "TextBlockWidthGreaterThanPanel", TestParameters = "content=Text_WidthGreaterThanPanel.xaml")]    
    [Test(0, "Bottomless", "BottomlessFloatImages", TestParameters = "content=BottomlessFloatImages.xaml")]
    [Test(0, "Bottomless", "BottomlessHorizontalAlignment", TestParameters = "content=BottomlessHorizontalAlignment.xaml")]
    [Test(0, "Bottomless", "BottomlessInlineImages", TestParameters = "content=BottomlessInlineImages.xaml")]
    [Test(0, "Bottomless", "BottomlessMarginCollapse", TestParameters = "content=BottomlessMarginCollapse.xaml")]
    [Test(0, "Bottomless", "BottomlessParagraph", TestParameters = "content=BottomlessParagraph.xaml")]
    [Test(2, "Bottomless", "BottomlessFlowDirection", TestParameters = "content=Bottomless_FlowDirection.xaml")]
    [Test(2, "Bottomless", "BottomlessTextAlignment", TestParameters = "content=Bottomless_TextAlignment.xaml")]
    [Test(2, "Bottomless", "BottomlessTextIndent", TestParameters = "content=Bottomless_TextIndent.xaml")]
    public class FlowLayoutPropertyDumpTests : PropertyDumpTest
    {        
        public FlowLayoutPropertyDumpTests()
        {
            this.DumpTest(DriverState.DriverParameters["content"], GenericStyles.LoadAllStyles());            
        }
    }        
}
