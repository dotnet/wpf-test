// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  RichTextContentData.
//  Provides data to be used when rich content is used in testing.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/RichTextContentData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.IO;
    using System.Threading; using System.Windows.Threading;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Documents;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Provides information about interesting rich text content values.
    /// </summary>
    public sealed class RichTextContentData
    {
        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private RichTextContentData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>Overload to ToString</summary>
        /// <returns>String representation of this object</returns>
        public override string ToString()
        {
            return Xaml;
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>
        /// Returns a XAML string that is a valid XAML fragment by
        /// itself.
        /// </summary>
        public string StandaloneXaml
        {
            get
            {
                return
                    "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xml:space='preserve'>"
                    + Xaml +
                    "</Section>";
            }
        }

        /// <summary>
        /// Returns the plain XAML snippet for this content.
        /// </summary>
        public string Xaml
        {
            get
            {
                return _xaml;
            }
        }

        /// <summary>
        /// Returns true if the contents contain UIElements
        /// </summary>
        public bool ContainsUIElements
        {
            get
            {
                return _containsUIElements;
            }
        }

        /// <summary>Returns complex content fully populated with text.</summary>
        /// <remarks>This will always be the last item in the array.</remarks>
        public static RichTextContentData FullyPopulatedContent
        {
            get
            {
                return Values[Values.Length - 1];
            }
        }

        /// <summary>Interesting values for testing rich content.</summary>
        public static RichTextContentData[] Values
        {
            get
            {
                const string RunStart = "<Run>";
                const string RunEnd = "</Run>";
                const string InlineStart = "<Italic>";
                const string InlineEnd = "</Italic>";                
                const string ParaStart = "<Paragraph>";
                const string ParaEnd = "</Paragraph>";
                const string ListStart = "<List>";
                const string ListEnd = "</List>";
                const string ListItemStart = "<ListItem>";
                const string ListItemEnd = "</ListItem>";                
                const string Text = "text";
                const string HyperlinkTag = "<Hyperlink NavigateUri='http://www.msn.com'>" +
                    RunStart + Text + RunEnd + "</Hyperlink>";
                const string TableTag = "<Table><TableRowGroup>" +
                    "<TableRow>" +
                    "<TableCell><Paragraph>" + Text + "</Paragraph></TableCell>" +
                    "<TableCell><Paragraph>" + Text + "</Paragraph></TableCell>" +
                    "</TableRow>" +
                    "<TableRow>" +
                    "<TableCell><Paragraph>" + Text + "</Paragraph></TableCell>" +
                    "<TableCell><Paragraph>" + Text + "</Paragraph></TableCell>" +
                    "</TableRow>" +
                    "</TableRowGroup></Table>";
                const string BUICStart = "<BlockUIContainer>";
                const string BUICEnd = "</BlockUIContainer>";
                
                string ImageTag = "<Image Height='50' Width='50' Source='" +
                    GetCurrentDirectory() + "\\colors.PNG' />";

                if (s_values == null)
                {
                    s_values = new RichTextContentData[] {
                        // Simple structures with no text.
                        FromXaml("", false),
                        FromXaml(ParaStart + ParaEnd, false),
                        FromXaml(ParaStart + RunStart + RunEnd + ParaEnd, false),
                        FromXaml(ParaStart + InlineStart + InlineEnd + ParaEnd, false),                        

                        // Simple structures with text.
                        FromXaml(ParaStart + RunStart + Text + RunEnd + ParaEnd, false),
                        FromXaml(ParaStart + InlineStart + Text + InlineEnd + ParaEnd, false),                        
                        FromXaml(ParaStart + InlineStart + Text + InlineEnd + 
                            HyperlinkTag + ParaEnd, false),
                        FromXaml(ListStart + ListItemStart + ParaStart + Text + ParaEnd + ListItemEnd + ListEnd, false),

                        // Nested structures with no text.
                        FromXaml(ParaStart + InlineStart + InlineStart + InlineEnd + InlineEnd + ParaEnd, false),                        
                        FromXaml(ListStart + ListItemStart + 
                            ParaStart + InlineStart + RunStart + RunEnd + InlineEnd + ParaEnd + 
                            ListStart + ListItemStart + ParaStart + RunStart + RunEnd + ParaEnd + ListItemEnd + ListEnd +
                            ListItemEnd + ListEnd, false),

                        // Nested structures with inside text.
                        FromXaml(ParaStart + InlineStart + InlineStart + Text + InlineEnd + InlineEnd + ParaEnd, false),                        
                        FromXaml(ListStart + ListItemStart + 
                            ParaStart + InlineStart + RunStart + Text + RunEnd + InlineEnd + ParaEnd + 
                            ListStart + ListItemStart + ParaStart + RunStart + Text + RunEnd + ParaEnd + ListItemEnd + ListEnd +
                            ListItemEnd + ListEnd, false),                        

                        // Nested structures with full text.
                        FromXaml(ParaStart + InlineStart + Text + InlineEnd + ParaEnd +
                            ListStart + ListItemStart + ParaStart + Text + ParaEnd + ListItemEnd + ListEnd +                            
                            ParaStart + RunStart + Text + RunEnd + ParaEnd +
                            TableTag, false),

                        // Nested structures with full text and image. (must always be last item in array)
                        FromXaml(ParaStart + InlineStart + Text + InlineEnd + ParaEnd +
                            ParaStart + HyperlinkTag + ParaEnd +
                            ListStart + ListItemStart + ParaStart + Text + ParaEnd + ListItemEnd + ListEnd +
                            TableTag + 
                            ParaStart + ImageTag + ParaEnd +
                            BUICStart + ImageTag + BUICEnd +
                            ParaStart + RunStart + Text + RunEnd + ParaEnd, true),
                    };
                }
                return s_values;
            }
        }

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Creates a new RichTextContentData instance for the given value.
        /// </summary>
        private static RichTextContentData FromXaml(string xaml, bool containsUIElements)
        {
            RichTextContentData result;

            result = new RichTextContentData();
            result._xaml = xaml;
            result._containsUIElements = containsUIElements;

            return result;
        }

        private static string GetCurrentDirectory()
        {
            new System.Security.Permissions.FileIOPermission(                
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            string workDirectory = System.Environment.CurrentDirectory;
            //string workDirectory = Path.Combine(Path.GetPathRoot(System.Environment.SystemDirectory), "work");
            //return "E:\\work";
            return workDirectory;            
        }

        #endregion Private methods.


        #region Private fields.

        /// <summary>Plain XAML content.</summary>
        private string _xaml;

        /// <summary>Interesting values for testing string values.</summary>
        private static RichTextContentData[] s_values;

        /// <summary>Whether the contents contain UIElements.</summary>
        private bool _containsUIElements;

        #endregion Private fields.
    }
}
