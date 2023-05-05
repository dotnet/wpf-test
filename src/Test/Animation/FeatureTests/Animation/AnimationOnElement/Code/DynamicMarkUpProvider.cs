// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  summary: this file provides the supplementary markup data based on
//  element type for parser-based tests.
//
// $Id:$ $Change:$
using System;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Navigation;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// Markup Dynamic content consumer interface
    /// </summary>
    public interface IDynamicContentConsumer
    {
        /// <summary>
        /// String reprensenting xml before the given element
        /// </summary>
        string PreContent     { set; }
        /// <summary>
        /// String reprensenting xml within the given element's opening tag
        /// </summary>
        string InlineContent  { set; }
        /// <summary>
        /// String reprensenting xml between the element's tags
        /// </summary>
        string InnerContent   { set; }
        /// <summary>
        /// String reprensenting xml after the given element
        /// </summary>
        string PosContent     { set; }
    }

    /// <summary>
    /// Data container for Markup for elements which require
    /// special considerations on parsing.
    /// </summary>
    public class DynamicContent
    {
        /// <summary>
        /// Customizes a consumer of Dynamic content
        /// </summary>
        public static void CustomizeConsumer( IDynamicContentConsumer contentConsumer, string elementName )
        {
            contentConsumer.PreContent = GetPreMarkup( elementName );
            contentConsumer.InlineContent = GetInlineMarkup( elementName );
            contentConsumer.InnerContent = GetInnerMarkup( elementName );
            contentConsumer.PosContent = GetPosMarkup( elementName );
        }

        /// <summary>
        /// Gets the markup to be set before a given element
        /// </summary>
        public static string GetPreMarkup( string elementName )
        {
            // default is empty string
            string markup = "";
            // certain elements requiere special treatment
            switch( elementName.ToUpper() )
            {
                case "TOOLTIP":
                    markup = @"<Button xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Button.ToolTip>" ;
                    break;

                case "CONTEXTMENU":
                    markup = @"<Button xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Button.ContextMenu>" ;
                    break;

                case "LISTITEM":
                    markup = @"<ListBox xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>";
                    break;

                case "TABITEM":
                    markup = "<TabControl>" ;
                    break;

                case "PARAGRAPH" :
                case "BLOCK" :
                case "BOLD" :
                case "HEADING" :
                case "INLINE" :
                case "ITALIC" :
                case "LINEBREAK" :
                case "LIST" :
                case "PAGEBREAK" :
                case "SECTION" :
                case "SMALLCAPS" :
                case "SUBSCRIPT" :
                case "SUPERSCRIPT" :
                case "UNDERLINE" :
                    markup = @"<FlowDocumentScrollViewer xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><FlowDocument>" ;
                    break;

                case "BODY":
                    markup = @"<Table xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Column Background='red'/><Column Background='green'/><Column Background='blue'/><Header><Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Header</Cell></Row></Header>";
                    break;

                case "HEADER":
                    markup = @"<Table xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Column Background='red'/><Column Background='green'/><Column Background='blue'/>";
                    break;

                case "FOOTER":
                    markup = @"<Table xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Column Background='red'/><Column Background='green'/><Column Background='blue'/><Header><Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Header</Cell></Row></Header><Body><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,2)</Cell></Row></Body>";
                    break;

                case "ROW":
                    markup = @"<Table xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Column Background='red'/><Column Background='green'/><Column Background='blue'/><Header>";
                    break;

                case "COLUMN":
                    markup = @"<Table xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>";
                    break;

                default:
                    break;
            }
            return markup;
        }

        /// <summary>
        /// Gets the markup to be set inside a given element
        /// </summary>
        public static string GetInnerMarkup( string elementName )
        {
            // default is empty string
            string markup = "";
            // certain elements requiere special treatment
            switch( elementName.ToUpper() )
            {
                // some elements cannot have content
                case "AUDIO":
                case "IMAGE":
                case "PASSWORD":
                case "SCROLLBAR":
                case "PAGEBAR":
                case "PAGEVIEWER":
                case "FRAME":
                case "FRAME_WITH_HTML_SOURCE":
                case "ELLIPSE":
                case "LINE":
                case "PATH":
                case "POLYGON":
                case "POLYLINE":
                case "RECTANGLE":
                case "HORIZONTALSLIDER":
                case "VERTICALSLIDER":
                case "THUMB":
                case "CONTROL":
                case "VIDEO":
                case "CONTENTPRESENTER":
                case "GLYPHS":
                    markup = "" ;
                    break;

                case "CANVAS":
                case "DOCKPANEL":
                case "FLOWPANEL":
                    markup = "<TextBlock>Some content text</TextBlock><TextBlock>Some content text</TextBlock>" ;
                    break;

                case "FIXEDPANEL":
                    markup = "<PageContent><FixedPage><TextBlock>Some content text</TextBlock></FixedPage></PageContent>" ;
                    break;

                case "CONTEXTMENU":
                    markup = "<ContextMenu><MenuItem>Item A</MenuItem><MenuItem>Item B</MenuItem></ContextMenu>" ;
                    break;

                case "TOOLTIP":
                    markup = "<ToolTip>The Button.ToolTip</ToolTip>" ;
                    break;

                case "BODY":
                    markup = "<Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,2)</Cell></Row>";
                    break;

                case "HEADER":
                    markup = "<Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Header</Cell></Row>";
                    break;

                case "FOOTER":
                    markup = "<Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Footer</Cell></Row>";
                    break;

                case "COLUMN":
                    markup = "";
                    break;

                case "ROW":
                    markup = "<Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Header</Cell>";
                    break;

                case "MENU":
                    markup = "<MenuItem Header='MenuItem 1'><MenuItem Header='MenuItem 1-1'/><MenuItem Header='MenuItem 1-2'/><MenuItem Header='MenuItem 1-3'/></MenuItem><MenuItem Header='MenuItem 2'/><MenuItem Header='MenuItem 3'/>" ;
                    break;

                case "COMBOBOX":
                case "LISTBOX":
                case "LISTVIEW":
                    markup = "<ListBoxItem>Item A</ListBoxItem><ListBoxItem>Item B</ListBoxItem><ListBoxItem>Item X</ListBoxItem>" ;
                    break;

                case "RADIOBUTTONLIST":
                    markup = "<RadioButton>Button A</RadioButton><RadioButton>Button B</RadioButton><RadioButton>Button C</RadioButton>" ;
                    break;

                case "REPEATBUTTON":
                    markup = "<TextBlock> the Repeat Button </TextBlock>" ;
                    break;

                case "SCROLLVIEWER":
                    markup = "<DockPanel><Button >Button 1</Button><Button >Button 2</Button></DockPanel>" ;
                    break;

                case "TABCONTROL":
                    markup = "<TabItem><TabItem.Header><TextBlock xmlns='using:System.Windows.Documents'>Header A</TextBlock></TabItem.Header><TextBlock xmlns='using:System.Windows.Documents'> Tab Content hello hello hello</TextBlock></TabItem><TabItem><TabItem.Header><TextBlock xmlns='using:System.Windows.Documents'>Header B</TextBlock></TabItem.Header><TextBlock xmlns='using:System.Windows.Documents'> Tab Content</TextBlock></TabItem><TabItem><TabItem.Header><TextBlock xmlns='using:System.Windows.Documents'>Header C</TextBlock></TabItem.Header><TextBlock xmlns='using:System.Windows.Documents'> Tab Content xxca afasf asaf </TextBlock></TabItem>" ;
                    break;

                case "TABITEM":
                    markup = "<TabItem.Header><TextBlock xmlns='using:System.Windows.Documents'>Header A</TextBlock></TabItem.Header><TextBlock xmlns='using:System.Windows.Documents'> Tab Content hello hello hello</TextBlock>" ;
                    break;

                case "TEXTPANEL":
                    markup = "<TextBlock>FlowDocumentScrollViewer TEST_ITEM</TextBlock>" ;
                    break;

                case "TOOLBAR":
                    markup = "<Button>Content X</Button><Button>Content Y</Button><Button>Content W</Button>" ;
                    break;

                case "TREEITEM":
                    markup = "<TreeItem.Title><TextBlock>Desktop</TextBlock></TreeItem.Title>" ;
                    break;

                case "TREEVIEW":
                    markup = "<TreeItem><TreeItem.Title><TextBlock xmlns='using:System.Windows.Documents'>Desktop</TextBlock></TreeItem.Title></TreeItem><TreeItem><TreeItem.Title><TextBlock xmlns='using:System.Windows.Documents'>Desktop number 2</TextBlock></TreeItem.Title></TreeItem>" ;
                    break;

                default:
                    // set all controls to some random text here
                    markup = "Some random text content is placed here...!";
                    break;
            }
            return markup;
        }

        /// <summary>
        /// Gets the markup to be set inside a given element
        /// </summary>
        public static string GetInlineMarkup( string elementName )
        {
            // default is empty string
            string markup = "";
            // certain elements requiere special treatment
            switch( elementName.ToUpper() )
            {
                // Misc
                case "HYPERLINK":
                    markup = "NavigateUri='http://www.msn.com'";
                    break;
                case "PROGRESSBAR":
                    markup = "Width='200' Height='20' Background='LightGray' Value='33' Minimum='0' Maximum='100' BorderColor='blue'";
                    break;

                // Geometry
                case "LINE":
                    markup = "Stroke='Magenta' Fill='Pink' StrokeThickness='4' X1='20' Y1='10' X2='60' Y2='100'";
                    break;
                case "PATH":
                    markup = "Stroke='Magenta' Fill='Pink' StrokeThickness='4' Data='M130,130 L 150,0 L -150,60 L 150,30'";
                    break;
                case "POLYGON":
                    markup = "Stroke='Magenta' Fill='Pink' StrokeThickness='4' Points='176.5,50 189.2,155.003 286.485,113.5 201.9,177 286.485,240.5 189.2,198.997 176.5,304 163.8,198.997 66.5148,240.5 151.1,177 66.5148,113.5 163.8,155.003'";
                    break;
                case "POLYLINE":
                    markup = "Stroke='Magenta' Fill='Pink' StrokeThickness='4' Points='250,50 365.2,210 290,230.8 360,55 420,135.1 380,129 440,85'";
                    break;
                case "ELLIPSE":
                    markup = "Stroke='Magenta' Fill='Pink' StrokeThickness='4' CenterX='101' CenterY='130' RadiusX='75' RadiusY='52'";
                    break;
                case "RECTANGLE":
                    markup = "Stroke='Magenta' Fill='Pink' StrokeThickness='4' Width='50' Height='50'";
                    break;

                // Content Containers
                case "AUDIO":
                    markup = "Source='sound.wma' Begin='Immediately'";
                    break;
                case "VIDEO":
                    markup = "Source='shuttle.wmv' Begin='Immediately'";
                    break;
                case "PAGEVIEWER":
                    markup = @"Source='PageViewerContent1.xaml'";
                    break;
                case "FRAME":
                    markup = @"Source='FrameContent.xaml'";
                    break;
                case "FRAME_WITH_HTML_SOURCE":
                    markup = @"SourceUri='FrameContent.html'";
                    break;
                case "IMAGE":
                    markup = "Source=\"Sample.png\"";
                    break;

                default:
                    break;
            }
            return markup;
        }

        /// <summary>
        /// Gets the markup to be set after a given element
        /// </summary>
        public static string GetPosMarkup( string elementName )
        {
            // default is empty string
            string markup = "";
            // certain elements requiere special treatment
            switch( elementName.ToUpper() )
            {
                case "CONTEXTMENU":
                    markup = "</Button.ContextMenu></Button>" ;
                    break;

                case "TOOLTIP":
                    markup = "</Button.ToolTip></Button>" ;
                    break;

                case "LISTITEM":
                    markup = "</ListBox>";
                    break;

                case "TABITEM":
                    markup = "</TabControl>" ;
                    break;

                case "PARAGRAPH" :
                case "BLOCK" :
                case "BOLD" :
                case "HEADING" :
                case "INLINE" :
                case "ITALIC" :
                case "LINEBREAK" :
                case "LIST" :
                case "PAGEBREAK" :
                case "SECTION" :
                case "SMALLCAPS" :
                case "SUBSCRIPT" :
                case "SUPERSCRIPT" :
                case "UNDERLINE" :
                    markup = @"</FlowDocument></FlowDocumentScrollViewer>" ;
                    break;

                case "BODY":
                    markup = @"<Footer><Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Footer</Cell></Row></Footer></Table>" ;
                    break;

                case "HEADER":
                    markup = @"<Body><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,2)</Cell></Row></Body><Footer><Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Footer</Cell></Row></Footer></Table>" ;
                    break;

                case "FOOTER":
                    markup = @"</Table>" ;
                    break;

                case "ROW":
                    markup = @"</Header><Body><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,2)</Cell></Row></Body><Footer><Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Footer</Cell></Row></Footer></Table>" ;
                    break;

                case "COLUMN":
                    markup = @"<Column Background='green'/><Column Background='blue'/><Header><Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Header</Cell></Row></Header><Body><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (0,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (1,2)</Cell></Row><Row><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,0)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,1)</Cell><Cell BorderThickness='1,1,1,1' BorderBrush='White'>Cell (2,2)</Cell></Row></Body><Footer><Row><Cell BorderThickness='1,1,1,1' ColumnSpan='3'>Footer</Cell></Row></Footer></Table>" ;
                    break;

                default:
                    break;
            }
            return markup;
        }

        /// <summary>
        /// Gets the url for another test, randomly
        /// </summary>
        public static string GetRandomTestURL()
        {
            // bias this towards drivers
            if ( System.Environment.TickCount % 3 != 0 )
            {
                // we currently have 5 drivers
                return s_fullTestList[ System.Environment.TickCount % 5 ];
            }
            return s_fullTestList[ System.Environment.TickCount % s_fullTestList.Length ];
        }

        /// <summary>
        /// Gets the url for another test, sequentially
        /// </summary>
        public static string GetSequentialTestURL( int sequenceNumber )
        {
            return s_fullTestList[ sequenceNumber % s_fullTestList.Length ];
        }

         /// <summary>
        /// Gets the list of supported test URLs
        /// </summary>
        public static string[] TestURLList
        {
            get { return s_fullTestList; }
        }
        private static string[] s_fullTestList = new string[] {
            "ANM_Common.xaml",
            "PPT_Common.xaml",
            "RPT_Common.xaml",
            "UIB_Common.xaml",
            "UIS_Common.xaml",
            "COR_AnimatePaints.xaml",
            "COR_MIL_DeepHier.xaml",
            "COR_MIL_DeepHier2.xaml",
            "COR_MIL_Transf_Edocs.xaml",
            "COR_MIL_Transf_UIBinding.xaml",
            "COR_MIL_Paints_EDocs.xaml",
            "COR_MIL_Paints_UIBinding.xaml",
            "COR_MIL_Shapes_EDocs.xaml",
            "COR_MIL_ANM_Button.xaml",
            "COR_MIL_ANM_Button_Filt.xaml",
            "COR_MIL_ANM_Button_Pars.xaml",
            "COR_MIL_ANM_ComboBox.xaml",
            "COR_MIL_ByAnimBinding.xaml",
            "COR_MIL_RepeaterAnim.xaml",
            "COR_MIL_LinkedAnim.xaml",
            "COR_MIL_TransformPrec.xaml",
            "COR_Heading_UIB_hl.xaml",
            "COR_PageViewer_anc.xaml",
            "COR_PageViewer_anm.xaml",
            "COR_PageViewer_CustBtn.xaml",
            "COR_PageViewer_CustPV.xaml",
            "COR_PageViewer_UIB_ic.xaml",
            "COR_PageViewer_UIB_pn.xaml",
            "COR_PageViewer_UIB_src.xaml",
            "COR_UIS_Btn_Cnv_Anim.xaml",
            "COR_UIS_Btn_Cnv_DataBind.xaml",
            "COR_UIS_Btn_Cnv_DB_OneW.xaml",
            "COR_UIS_Btn_Cnv_PSheet.xaml",
            "COR_UIS_Btn_Cnv_PSheet_Ih.xaml",
            "COR_UIS_TextBox_Canvas.xaml",
            "COR_UIS_BoxUnitAnim_Freeze.xaml",
            "COR_UIS_ColorAnim.xaml",
            "COR_UIS_FloatAnim.xaml",
            "COR_CORE_HostingCC.xaml",
            "COR_CORE_HostingANM.xaml",
            "COR_CORE_HostingEvents.xaml",
            "COR_CORE_HostingPPT.xaml",
            "COR_APP_ANM.xaml",
            "COR_APP_UIB_PF.xaml",
            "COR_APP_PPT.xaml",
            "COR_APP_UIB.xaml",
            "COR_Frame_ANM.xaml",
            "COR_Frame1_ANM.xaml",
            "COR_Frame2_ANM.xaml",
            "COR_Frame_IN_OUT.xaml",
            "COR_Frame_IN_OUT2.xaml",
            "COR_Frame_UIB_Src.xaml",
            "COR_Frame_Src.xaml",
            "UIS_Paints_FlowPanel_Cnv.xaml",
            "UIS_Transf_Btn.xaml",
            "COR_BASIC_SimpleLog.xaml"
            };


        /// <summary>
        /// Gets the element, randomly
        /// </summary>
        public static string GetRandomElement()
        {
            return s_fullElementList[ System.Environment.TickCount % s_fullElementList.Length ];
        }

        /// <summary>
        /// Gets the element, sequentially
        /// </summary>
        public static string GetSequentialElement( int sequenceNumber )
        {
            return s_fullElementList[ sequenceNumber % s_fullElementList.Length ];
        }

        /// <summary>
        /// Gets the list of supported elements in
        /// </summary>
        public static string[] ElementList
        {
            get { return s_fullElementList; }
        }
        private static string[] s_fullElementList = new string[] {
            "ADAPTIVEMETRICSCONTEXT",
            "AUDIO",
            "BODY",
            "BORDER",
            "BUTTON",
            "CANVAS",
            "CHECKBOX",
            "COLUMN",
            "COMBOBOX",
            "COMBOBOXITEM",
            "CONTENTCONTROL",
            "CONTENTPRESENTER",
            "CONTEXTMENU",
            "CONTROL",
            "CUSTOMBUTTON",
            "DESIGNSURFACE",
            "DOCKPANEL",
            "ELLIPSE",
            "EMBEDDEDDIALOG",
            "FIXEDPANEL",
            "FLOWPANEL",
            "FOOTER",
            "FRAME",
            "GLYPHS",
            "GRID",
            "HEADER",
            "HORIZONTALSLIDER",
            "HYPERLINK",
            "IMAGE",
            "ITEMSCONTROL",
            "LABEL",
            "LINE",
            "LISTBOX",
            "LISTITEM",
            "MENU",
            "MENUITEM",
            "NAVIGATIONWINDOW",
            "PAGE",
            "PANEL",
            "PAGEVIEWER",
            "PATH",
            "POLYGON",
            "POLYLINE",
            "RADIOBUTTONLIST",
            "RADIOBUTTON",
            "RECTANGLE",
            "REPEATBUTTON",
            "ROW",
            "SCROLLVIEWER",
            "TABLE",
            "TEXT",
            "TEXTBOX",
            "TEXTPANEL",
            "THUMB",
            "TOOLTIP",
            "TRANSFORMDECORATOR",
            "VERTICALSLIDER",
            "VIDEO",
            "WINDOW"
            };

    }
}
