// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Layout
{       
    //property dump tests
    [Test(0, "Table", "TableColumnSpanFlow", TestParameters = "content=C_ColSpantest1.xaml, container=fd, cw=400, verification=pdump")]
    [Test(0, "Table", "TableCellSpacingBottomless", TestParameters = "content=C_CellSpacing.xaml, verification=pdump")]
    [Test(0, "Table", "TableCellSpacingFlow", TestParameters = "content=C_CellSpacing.xaml, container=fd, cw=400, verification=pdump")]
    [Test(0, "Table", "StylingTable", TestParameters = "content=TableStyling.xaml, verification=pdump")]
    [Test(0, "Table", "CPaddingFlow", TestParameters = "content=C_Padding.xaml,  container=fd,cw=400, verification=pdump")]
    [Test(2, "Table", "EmptyTable", TestParameters = "content=EmptyTable.xaml, verification=pdump")]    
    [Test(2, "Table", "RowSpanPagination", TestParameters = "content=RowSpanPagination.xaml, container=fd, verification=pdump")]
    [Test(2, "Table", "TableColumnSpanBottomless", TestParameters = "content=C_ColSpantest1.xaml, verification=pdump")]
    [Test(2, "Table", "TablePaginateCellBorderPaginated", TestParameters = "content=C_PaginatebCellBorder.xaml, container=fd, verification=pdump")]
    [Test(2, "Table", "TablePaginateColSpanPaginate", TestParameters = "content=C_PaginateColSpan.xaml, container=fd, verification=pdump")]
    [Test(2, "Table", "TablePaginateColSpanBottomless", TestParameters = "content=C_PaginateColSpan.xaml, verification=pdump")]
    [Test(2, "Table", "TableRowSpanPaginationBottomless", TestParameters = "content=C_RowSpanPagination.xaml, verification=pdump")]
    [Test(2, "Table", "TableRowSpanPaginationPaginate", TestParameters = "content=C_RowSpanPagination.xaml, container=fd, verification=pdump")]
    [Test(2, "Table", "TableRowSpanBottomless", TestParameters = "content=C_RowSpantest1.xaml, verification=pdump")]
    [Test(2, "Table", "TableRowSpanPaginate", TestParameters = "content=C_RowSpantest1.xaml, container=fd, cw=400, verification=pdump")]
    [Test(2, "Table", "TableCellContentRTLBottomless", TestParameters = "content=C_TableCellContentRTL.xaml, verification=pdump")]
    [Test(2, "Table", "TableCellContentRTLPaginate", TestParameters = "content=C_TableCellContentRTL.xaml, container=fd, cw=400, verification=pdump")]
    [Test(2, "Table", "TableFlowDirectionBottomless", TestParameters = "content=C_TableFlowDirection.xaml, verification=pdump")]
    [Test(2, "Table", "TableFlowDirectionPaginate", TestParameters = "content=C_TableFlowDirection.xaml, container=fd, cw=400, verification=pdump")]
    [Test(2, "Table", "TableInRTLSection", TestParameters = "content=C_TableInRTLSection.xaml, verification=pdump")]
    [Test(2, "Table", "TableInRTLSectionFlow", TestParameters = "content=C_TableInRTLSection.xaml, container=fd, verification=pdump")]
    [Test(2, "Table", "CPadding", TestParameters = "content=C_Padding.xaml, verification=pdump")]
    [Test(3, "Table", "TablePaginatedThickBorderedCells", TestParameters = "content=Table_PaginatedThickBorderedCells.xaml, container=fd,cw=150,cb=false, verification=pdump")]
    [Test(3, "Table", "TableFloatersInTableCells", TestParameters = "content=Table_FloatersInTableCells.xaml, container=fd, cb=false, verification=pdump")]
    [Test(3, "Table", "TableAsButton", TestParameters = "content=TableAsButtonStyle.xaml, verification=pdump")]    
    [Test(0, "Hyperlink", "HyperlinkBottomless", TestParameters = "content=BVT_Hyperlink.xaml, container=sv, verification=pdump")]
    [Test(0, "Hyperlink", "HyperlinkFlowDoc", TestParameters = "content=BVT_Hyperlink.xaml,  container=fd, width=300, verification=pdump")]
    [Test(0, "ContentElement", "ContentElementFloaterHorizontalAlignment", TestParameters = "content=ContentElement_Floater_HorizontalAlignment.xaml, container=sv, verification=pdump")]
    [Test(0, "ContentElement", "ContentElementListNestedListBottomLess", TestParameters = "content=ContentElement_List_NestedList.xaml, container=sv, verification=pdump")]
    [Test(0, "ContentElement", "ContentElementListNestedListFlow", TestParameters = "content=ContentElement_List_NestedList.xaml,  container=fd, cw=600, cb=false, verification=pdump")]
    [Test(0, "ContentElement", "ContentElementFloaterHorizontalAlignmentFlow", TestParameters = "content=ContentElement_Floater_HorizontalAlignment.xaml, container=fd, cw=800, verification=pdump")]
    [Test(0, "ContentElement", "ContentElementClearFloatersboth", TestParameters = "content=ContentElement_ClearFloaters_both.xaml, container=sv, verification=pdump")]
    [Test(0, "ContentElement", "ContentElementBlockUIContainerFlow", TestParameters = "content=ContentElement_BlockUIContainer.xaml, container=fd, verification=pdump")]
    [Test(0, "ContentElement", "ContentElementFigureBottomless", TestParameters = "content=ContentElement_Figure_Bottomless.xaml, container=sv, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementParagraphBreakColumnBefore", TestParameters = "content=ContentElement_Paragraph_BreakColumnBefore.xaml,container=fd, pgNum=0, height=620, width=600, cw=150, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementFloaterWidthGreaterThanContainer", TestParameters = "content=ContentElement_Floater_WidthGreaterThanContainer.xaml, container=sv, width=400, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementFloaterWidthSetAndStretchHAlignFlow", TestParameters = "content=ContentElement_Floater_WidthSetAndStretchHAlign.xaml, container=fd, cw=500, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementBlockUIContainer", TestParameters = "content=ContentElement_BlockUIContainer.xaml,  container=sv, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementAnchoredBlocksUIElements", TestParameters = "content=ContentElement_AnchoredBlocks_UIElements.xaml, container=fd, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementFloaterTopOfParagraph", TestParameters = "content=ContentElement_Floater_TopOfParagraph.xaml, container=fd, verification=pdump")]
    [Test(2, "ContentElement", "RTLSection", TestParameters = "content=RTL_Section.xaml, container=sv, verification=pdump")]
    [Test(2, "ContentElement", "RTLParagraphAndInline", TestParameters = "content=RTL_ParagraphAndInline.xaml, container=sv, verification=pdump")]
    [Test(2, "ContentElement", "RTLUIContainer", TestParameters = "content=RTL_UIContainer.xaml, container=sv, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementFloaterWidthSetAndStretchHAlign", TestParameters = "content=ContentElement_Floater_WidthSetAndStretchHAlign.xaml, container=sv, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementParagraphMinOrphanLines", TestParameters = "content=ContentElement_Paragraph_MinOrphanLines.xaml, container=fd ,pgNum=1, height=530, width=600, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementParagraphMinWidowLines", TestParameters = "content=ContentElement_Paragraph_MinWidowLines.xaml, container=fd, pgNum=1, height=490, width=600, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementParagraphKeepTogether", TestParameters = "content=ContentElement_Paragraph_KeepTogether.xaml, container=fd, pgNum=1, height=600, width=600, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementParagraphKeepWithNext", TestParameters = "content=ContentElement_Paragraph_KeepWithNext.xaml, container=fd, pgNum=1, height=400, width=550, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementParagraphKeepWithNextRegression_Bug38", TestParameters = "content=ContentElement_Paragraph_KeepWithNext_Regression_Bug38.xaml, container=fd, cw=150, cb=false, pgNum=0, verification=pdump")]
    [Test(2, "ContentElement", "ContentElementParagraphKeepWithNextColumn", TestParameters = "content=ContentElement_Paragraph_KeepWithNext_Column.xaml, container=fd, cw=250, pgNum=0, cwf=false, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementLineBreak", TestParameters = "content=ContentElement_LineBreak.xaml, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementLineBreakFlow", TestParameters = "content=ContentElement_LineBreak.xaml, container=fd, cb=false, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementParagraphFlow", TestParameters = "content=ContentElement_Paragraph.xaml, container=fd, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementSectionFlow", TestParameters = "content=ContentElement_Section.xaml, container=fd, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersbothFlow", TestParameters = "content=ContentElement_ClearFloaters_both.xaml, container=fd, cw=800, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersLeft", TestParameters = "content=ContentElement_ClearFloaters_Left.xaml, container=sv, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersLeftFlow", TestParameters = "content=ContentElement_ClearFloaters_Left.xaml, container=fd, cw=450, cwf=false, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersRight", TestParameters = "content=ContentElement_ClearFloaters_Right.xaml, container=sv, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersRightFlow", TestParameters = "content=ContentElement_ClearFloaters_Right.xaml, container=fd, cw=400, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersNone", TestParameters = "content=ContentElement_ClearFloaters_None.xaml, container=sv, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersNoneFlow", TestParameters = "content=ContentElement_ClearFloaters_None.xaml, container=fd, cw=500, cb=false, cwf=false, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersCenterHAonFloater", TestParameters = "content=ContentElement_ClearFloaters_CenterHA_onFloater.xaml, container=sv, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementClearFloatersCenterHAonFloaterFlow", TestParameters = "content=ContentElement_ClearFloaters_CenterHA_onFloater.xaml, container=fd, cw=400, verification=pdump")]
    [Test(3, "ContentElement", "ContentElementFloaterWidthGreaterThanContainerFlow", TestParameters = "content=ContentElement_Floater_WidthGreaterThanContainer.xaml, container=fd, width=400, verification=pdump")]
    [Test(3, "ContentElement", "Regression_Bug4", TestParameters = "content=Regression_Bug4.xaml, container=sv, verification=pdump")]   
    [Test(2, "TextBlock", "RTLTextBlock", TestParameters = "content=RTL_TextBlock.xaml, container=sv, verification=pdump")]    
    [Test(0, "FlowDocument", "FlowDocumentFiguresOffset", TestParameters = "content=FlowDocument_FiguresOffset.xaml, container=fd, height=500, width=600, bg=true, verification=pdump")]
    [Test(0, "FlowDocument", "FlowDocumentFiguresWrapping", TestParameters = "content=FlowDocument_FiguresWrapping.xaml, container=fd, height=500, width=750, verification=pdump")]
    [Test(0, "FlowDocument", "LineStackingStrategy", TestParameters = "content=LineStackingStrategy.xaml, container=sv, verification=pdump")]
    [Test(0, "FlowDocument", "LineStackingStrategyOptimal", TestParameters = "content=LineStackingStrategy.xaml, container=fd, optimal=true, verification=pdump")]    
    [Test(0, "FlowDocument", "FontSize", TestParameters = "content=FontSize.xaml, container=sv, verification=pdump")]
    [Test(2, "FlowDocument", "FlowDocumentPageStackedFigures", TestParameters = "content=FlowDocument_PageStackedFigures.xaml, container=fd, cw=400, verification=pdump")]
    [Test(2, "FlowDocument", "FlowDocumentListTableWithAnchoredBlocks", TestParameters = "content=FlowDocument_ListTableWithAnchoredBlocks.xaml, container=fd, height=600, cw=600, verification=pdump")]    
    [Test(2, "FlowDocument", "FlowDocumentHorizontallyStackedFiguresRegression_Bug39", TestParameters = "content=FlowDocument_HorizontallyStackedFigures_Regression_Bug39.xaml, container=fd, verification=pdump")]
    [Test(2, "FlowDocument", "FlowDocumentHorizontallyStackedFiguresFullColumnWidth", TestParameters = "content=FlowDocument_HorizontallyStackedFigures_FullColumnWidth.xaml, container=fd, verification=pdump")]
    [Test(2, "FlowDocument", "RTLTextAlignment", TestParameters = "content=RTL_TextAlignment.xaml, container=fd, verification=pdump")]
    [Test(2, "FlowDocument", "FlowDocumentHorizontallyStackedFiguresRegression_Bug40", TestParameters = "content=FlowDocument_HorizontallyStackedFigures_Regression_Bug40.xaml, container=fd, cw=200, verification=pdump")]
    [Test(2, "FlowDocument", "FlowDocumentHorizontallyStackedFiguresMiddleAlignment", TestParameters = "content=FlowDocument_HorizontallyStackedFigures_MiddleAlignment.xaml, container=fd, verification=pdump")]
    [Test(3, "FlowDocument", "Regression_Bug5", TestParameters = "content=Regression_Bug5.xaml, container=sv, width=400, verification=pdump")]
    [Test(3, "FlowDocument", "Regression_Bug6", TestParameters = "content=Regression_Bug6.xaml, container=sv, verification=pdump")]
    [Test(3, "FlowDocument", "Regression_Bug7", TestParameters = "content=Regression_Bug7.xaml, container=sv, verification=pdump")]
    [Test(3, "FlowDocument", "FlowDocumentFigureWidthGreaterThanContainer", TestParameters = "content=FlowDocument_Figure_WidthGreaterThanContainer.xaml, container=fd, width=400, pp=0:0:0:0, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureWidthColumnAndPixel", TestParameters = "content=Figure_Width_ColumnAndPixel.xaml, container=fd, cw=200, cb=false, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureWidthContentAndPage", TestParameters = "content=Figure_Width_ContentAndPage.xaml, container=fd, cw=200, cb=false, pp=80:0:80:0, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureHeightColumnAndPixel", TestParameters = "content=Figure_Height_ColumnAndPixel.xaml, container=fd, cw=200, cb=false, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureHeightContentAndPage", TestParameters = "content=Figure_Height_ContentAndPage.xaml, container=fd, cw=200, cb=false, pp=0:80:0:80, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureAnchoringContent", TestParameters = "content=Figure_Anchoring_Content.xaml, container=fd, height=500, width=800, pp=60:50:20:20, cw=1000, bg=true, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureAnchoringColumnAndParagraph", TestParameters = "content=Figure_Anchoring_ColumnAndParagraph.xaml, container=fd, height=400, width=700, pp=10:10:10:10, bg=true, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureAnchoringPage", TestParameters = "content=Figure_Anchoring_Page.xaml, container=fd, height=500, width=750, pp=30:30:30:30, cw=1000, bg=true, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureBottomlessHAnchoring", TestParameters = "content=Figure_Bottomless_HAnchoring.xaml, container=sv, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureBottomlessVAnchoring", TestParameters = "content=Figure_Bottomless_VAnchoring.xaml, container=sv, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureBottomlessWidthSizing", TestParameters = "content=Figure_Bottomless_WidthSizing.xaml, container=sv, verification=pdump")]
    [Test(0, "AnchoredBlock", "FigureBottomlessHeightSizing", TestParameters = "content=Figure_Bottomless_HeightSizing.xaml, container=sv, verification=pdump")]
    [Test(0, "AnchoredBlock", "FloaterPaginatingColumns", TestParameters = "content=Floater_PaginatingColumns.xaml, container=fd, cw=250, verification=pdump")]
    [Test(2, "AnchoredBlock", "ContentElementAnchoredBlocksRTL", TestParameters = "content=ContentElement_AnchoredBlocks_RTL.xaml, container=sv, verification=pdump")]
    [Test(2, "AnchoredBlock", "ContentElementAnchoredBlocksRTLFlow", TestParameters = "content=ContentElement_AnchoredBlocks_RTL.xaml, container=fd, cw=500, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFigureWidthAutoSizing", TestParameters = "content=FlowDocument_Figure_WidthAutoSizing.xaml, container=fd, cw=250, cb=false, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresStackingColumnAligned1", TestParameters = "content=FlowDocument_Figures_Stacking_ColumnAligned1.xaml, container=fd, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresStackingContentAligned1", TestParameters = "content=FlowDocument_Figures_Stacking_ContentAligned1.xaml, container=fd, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresStackingContentAligned2", TestParameters = "content=FlowDocument_Figures_Stacking_ContentAligned2.xaml, container=fd, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresStackingMixed1", TestParameters = "content=FlowDocument_Figures_Stacking_Mixed1.xaml, container=fd, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresStackingMixed2", TestParameters = "content=FlowDocument_Figures_Stacking_Mixed2.xaml, container=fd, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresStackingMixed3", TestParameters = "content=FlowDocument_Figures_Stacking_Mixed3.xaml, container=fd, height=400, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresStackingMixed4", TestParameters = "content=FlowDocument_Figures_Stacking_Mixed4.xaml, container=fd, height=450, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresStackingPageAligned1", TestParameters = "content=FlowDocument_Figures_Stacking_PageAligned1.xaml, container=fd, verification=pdump")]
    [Test(2, "AnchoredBlock", "FlowDocumentFiguresAnchoringColumnWidthNOTFlexible", TestParameters = "content=FlowDocument_Figures_Anchoring_ColumnWidthNOTFlexible.xaml, container=fd, cwf=false, height=380, verification=pdump")]
    [Test(2, "AnchoredBlock", "FigureBottomlessClearFloaters", TestParameters = "content=Figure_Bottomless_ClearFloaters.xaml, container=sv, verification=pdump")]
    [Test(2, "AnchoredBlock", "FigureBottomlessWrapDirection", TestParameters = "content=Figure_Bottomless_WrapDirection.xaml, container=sv, verification=pdump")]
    [Test(2, "AnchoredBlock", "FloaterPaginatingPages", TestParameters = "content=Floater_PaginatingPages.xaml, container=fd, verification=pdump")]
    [Test(2, "AnchoredBlock", "FloaterUIElement", TestParameters = "content=Floater_UIElement.xaml, container=fd, verification=pdump")]
    [Test(3, "AnchoredBlock", "FlowDocumentVerticalOffsetOnStackedFigures", TestParameters = "content=FlowDocument_VerticalOffsetOnStackedFigures.xaml, container=fd, height=400, width=500, bg=true, verification=pdump")]
    [Test(3, "AnchoredBlock", "FlowDocumentHorizontalOffsetOnStackedFigures", TestParameters = "content=FlowDocument_HorizontalOffsetOnStackedFigures.xaml, container=fd, height=400, width=500, bg=true, verification=pdump")]
    [Test(3, "AnchoredBlock", "FlowDocumentFigureColumnAnchoringandWidth", TestParameters = "content=FlowDocument_Figure_ColumnAnchoringandWidth.xaml, container=fd, cw=200, width=700, height=500, verification=pdump")]
    [Test(3, "AnchoredBlock", "FlowDocumentAnchoredBlockChildTopMarginCollapsing", TestParameters = "content=FlowDocument_AnchoredBlock_ChildTopMarginCollapsing.xaml, container=fd, verification=pdump")]
    [Test(3, "AnchoredBlock", "FlowDocumentABRemovingUnwantedSpaceOfAnchorPos", TestParameters = "content=FlowDocument_AB_RemovingUnwantedSpaceOfAnchorPos.xaml, container=fd, verification=pdump")]
    [Test(3, "AnchoredBlock", "FlowDocumentFiguresCollidingFigureMargins", TestParameters = "content=FlowDocument_Figures_CollidingFigureMargins.xaml, container=fd, verification=pdump")]
    [Test(3, "AnchoredBlock", "FlowDocumentFloatersCollidingFloaterMargins", TestParameters = "content=FlowDocument_Floaters_CollidingFloaterMargins.xaml, container=fd, verification=pdump")]
    [Test(3, "AnchoredBlock", "FlowDocumentFiguresMarginsAndAnchoring", TestParameters = "content=FlowDocument_Figures_MarginsAndAnchoring.xaml, container=fd, cb=false, height=400, verification=pdump")]
    [Test(3, "AnchoredBlock", "ContentElementFiguresOverflowingContent", TestParameters = "content=ContentElement_Figures_OverflowingContent.xaml, container=fd, cw=250, cb=false, verification=pdump")]
  
    //vscan tests            
    [Test(3, "Table", "TableBackgroundHierarchy", TestParameters = "content=Table_BackgroundHierarchy.xaml,  container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]          
    [Test(3, "Hyperlink", "HyperlinkNoTextDecoration", TestParameters = "content=Hyperlink_noTextDecorations.xaml", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]            
    [Test(2, "ContentElement", "ContentElementListMarkerOffsetBottomLess", TestParameters = "content=ContentElement_List_MarkerOffset.xaml", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementListMarkerOffsetFlow", TestParameters = "content=ContentElement_List_MarkerOffset.xaml,  container=fd, cb=false", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementListMarkerStyleBottomless", TestParameters = "content=ContentElement_List_MarkerStyle.xaml,  container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementListMarkerStyleFlow", TestParameters = "content=ContentElement_List_MarkerStyle.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementListStartIndexBottomLess", TestParameters = "content=ContentElement_List_StartIndex.xaml,  container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementListStartIndexFlow", TestParameters = "content=ContentElement_List_StartIndex.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]   
    [Test(2, "ContentElement", "ContentElementFloaterBackgroundOverFloater", TestParameters = "content=ContentElement_Floater_BackgroundOverFloater.xaml, container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementFigureBackground", TestParameters = "content=ContentElement_Figure_Background.xaml, container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementStyledContentElementsFlow", TestParameters = "content=ContentElement_StyledContentElements.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementStyledContentElements", TestParameters = "content=ContentElement_StyledContentElements.xaml,  container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]   
    [Test(2, "ContentElement", "ContentElementFloaterBackground", TestParameters = "content=ContentElement_Floater_Background.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementRunBackground", TestParameters = "content=ContentElement_Run_Background.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementSpanBackground", TestParameters = "content=ContentElement_Span_Background.xaml", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementBoldBackground", TestParameters = "content=ContentElement_Bold_Background.xaml", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementItalicBackground", TestParameters = "content=ContentElement_Italic_Background.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementUnderlineBackground", TestParameters = "content=ContentElement_Underline_Background.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "ContentElement", "ContentElementHyperlinkBackground", TestParameters = "content=ContentElement_Hyperlink_Background.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]        
    [Test(2, "ContentElement", "RTLList", TestParameters = "content=RTL_List.xaml,  container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]    
    [Test(3, "ContentElement", "ContentElementBoldFlow", TestParameters = "content=ContentElement_Bold.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(3, "ContentElement", "ContentElementItalicFlow", TestParameters = "content=ContentElement_Italic.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]    
    [Test(3, "ContentElement", "ContentElementSubScriptFlow", TestParameters = "content=ContentElement_SubScript.xaml,  container=fd, cw=600", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(3, "ContentElement", "ContentElementSuperScriptFlow", TestParameters = "content=ContentElement_SuperScript.xaml,  container=fd, cw=600", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(3, "ContentElement", "ContentElementUnderlineFlow", TestParameters = "content=ContentElement_Underline.xaml,  container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]           
    [Test(3, "ContentElement", "ContentElementFloaterBackgroundOverFloaterFlow", TestParameters = "content=ContentElement_Floater_BackgroundOverFloater.xaml,  container=fd,cw=1000", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(3, "ContentElement", "ContentElementListBackgroundBug", TestParameters = "content=ContentElement_List_BackgroundBug.xaml,  container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(3, "ContentElement", "Regression_Bug8", TestParameters = "content=Regression_Bug8.xaml,container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(3, "ContentElement", "Regression_Bug9", TestParameters = "content=Regression_Bug9.xaml,container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]        
    [Test(2, "TextBlock", "TextBlockVisibility", TestParameters = "content=TextBlock_Visibility.xaml,container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]    
    [Test(2, "TextBlock", "CTBTextBlock", TestParameters = "content=CTB_TextBlock.xaml,  container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]            
    [Test(0, "FlowDocument", "Foreground", TestParameters = "content=Foreground.xaml,container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "FlowDocument", "ColumnRuleBrush", TestParameters = "content=FlowDocument_ColumnRuleBrush.xaml,container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "FlowDocument", "ColumnRuleWidth", TestParameters = "content=FlowDocument_ColumnRuleWidth.xaml,container=fd", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "FlowDocument", "ColumnGap", TestParameters = "content=FlowDocument_ColumnGap.xaml, container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "FlowDocument", "ColumnWidth", TestParameters = "content=FlowDocument_ColumnWidth.xaml, container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "FlowDocument", "FlexibleColumnWidth", TestParameters = "content=FlowDocument_FlexibleColumnWidth.xaml, container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "FlowDocument", "TextDecorations", TestParameters = "content=FlowDocument_TextDecorations.xaml,  container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]        
    [Test(3, "FlowDocument", "Regression_Bug10", TestParameters = "content=Regression_Bug10.xaml,container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]        
    [Test(2, "Viewer", "ViewerClip", TestParameters = "content=Viewer_Clip.xaml,container=sv", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
        
    //Tests that specify dimensions on the master
    // [Test(2, "FlowDocument", "RTLFD", TestParameters="content=RTL_FD.xaml, container=sv, masterdimensions=wpfculture;os", Variables="VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN", Keywords="Localization_Suite")]
    // [Test(2, "FlowDocument", "RTLFDAndViewer", TestParameters = "content=RTL_FDAndViewer.xaml,container=sv, masterdimensions=wpfculture;os", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN", Keywords = "Localization_Suite")]
    // [Test(2, "Viewer", "RTLViewer", TestParameters = "content=RTL_Viewer.xaml, container=sv, masterdimensions=wpfculture;os", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN", Keywords = "Localization_Suite")]
    //[Test(3, "ContentElement", "RTLTextBox", TestParameters = "content=RTL_TextBox.xaml, container=sv, masterdimensions=os", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    //[Test(3, "ContentElement", "TextBoxTextAlignmentRight", TestParameters = "content=TextBox_TextAlignment_Right.xaml, container=sv, masterdimensions=os", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    [Test(2, "FlowDocument", "Fractions", TestParameters = "content=FlowDocument_Fractions.xaml,container=sv, masterdimensions=os", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]

    // added for missing code coverage
    [Test(0, "ContentElement", "RunWithMongolianLang", TestParameters = "content=ContentElement_Run_Mongolian.xaml, container=fd, masterdimensions=os", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    
    public class FlowLayoutLoader : AvalonTest
    {
        private FlowLayoutLoaderHelper _fh;
                                      
        public FlowLayoutLoader()
            : base()
        {
            _fh = new FlowLayoutLoaderHelper(this);            
            RunSteps += new TestStep(FlowLayoutLoader_RunTest);
            CleanUpSteps += new TestStep(CleanUp);
        }
                     
        private TestResult FlowLayoutLoader_RunTest()
        {
            Status("Running FlowLayoutLoader Test...");
            try
            {
                _fh.RunLayoutVerificationTest();
            }
            catch (System.Xml.XmlException)
            {
                return TestResult.Ignore;
            }

            if (_fh.finalResult)
            {
                Log.LogStatus("Test Passed.");
                return TestResult.Pass;
            }
            else
            {
                Log.LogStatus("Test Failed.");
                return TestResult.Fail;
            }
        }

        private TestResult CleanUp()
        {
            _fh.CloseWindow();
            return TestResult.Pass;
        }
    }
}
