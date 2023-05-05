// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Microsoft Windows
//
//
//  Description : Helper functions to obtain Markup for an Element for testing
//  purpose. File name containing XAML markup should be provided. Expect more
//  details specified here later.
//
//  Usage:
//      ElementFactory e = new ElementFactory("MarkupElements.xaml");
//      e.GetElement("Button");
//      foo.AddChild(e.TargetElementRoot); // add the tree containing TargetElement
//      foo.AddChild(e.TargetElement);     // add just the target element. may not work for all elements
//  
//  File with XAML markup should be in a pre-defined format (add details later)
// $Id:$ $Change:$
using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;


namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Class the provides ready-made XAML elements
    /// </summary>
    public class ElementFactory
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementsFile">Name of the file with Markup for Elements</param>
        public ElementFactory(string elementsFile)
        {
            SourceFile = elementsFile;
            _sourceFileXAML = TrustedHelper.GetTextFile( elementsFile ); // exception thrown if invalid
        }

        /// <summary>
        /// Get XAML corresponding for the given Element
        /// </summary>
        /// <param name="elementTag">Name of the Control/Element/Shape</param>
        /// <returns>Returns XAML for Control/Element/Shape</returns>
        public string GetElementXAML(string elementTag)
        {
            String          line    = String.Empty;
            String          XAML    = String.Empty;
            bool            inElementXaml  = false;
            StringReader    SourceFileXAMLReader    = new StringReader(_sourceFileXAML);
            string          searchElementID         = "<!--" + elementTag.ToUpper() + "-->";

            //Read until see searchElementID
            while ( ( line = SourceFileXAMLReader.ReadLine() ) != null )
            {
                if (inElementXaml)
                {
                    XAML += line + "\n";
                }

                if  ( String.Compare( line.Trim(), searchElementID.Trim() ) == 0 )
                {
                    if (!inElementXaml)
                    {
                        // When searchElementID found first time - Begin
                        inElementXaml = true;
                    }
                    else
                    {
                        // When searchElementID found second time - End
                        break;
                    }
                }
            }

            return XAML;
        }

        /// <summary>
        /// Gets a list of the element TAGS available to the from the given file.
        /// </summary>
        /// <returns>A list of string tags, zero count list if empty.</returns>
        public string[] GetAvailableElements()
        {
            string line = String.Empty;
            string lastMatch = String.Empty;
            ArrayList tags = new ArrayList();

            // Note: Add "_" to the expression to get compound tags            
            Regex tagExpression = new Regex(@"\<\!--(?<elementTag>[A-Z]*)--\>",RegexOptions.Compiled);

            // Read file 
            StringReader SourceFileXAMLReader = new StringReader(_sourceFileXAML);
            while ( ( line = SourceFileXAMLReader.ReadLine() ) != null )
            {
                // See if we find a TAG
                Match m = tagExpression.Match(line);
                if ( m.Success )
                {
                    // get the actual tag
                    string currentMatch = m.Result( "$1" );
                    // if this is the second time we match, we found a block
                    if ( currentMatch != null && currentMatch != String.Empty && lastMatch == currentMatch )
                    {
                        tags.Add( currentMatch );
                    }
                    // remember the last one
                    lastMatch = currentMatch;
                }
            }
            // build output
            string[] result = new string[ tags.Count ];
            for ( int i=0; i<tags.Count; i++ )
            {
                result[i] = tags[i] as string;
            }
            return result;
        }


        /// <summary>
        /// Returns full XAML in the ElementsFile minus those in ElementNames
        /// 
        /// Iterates through XAML and skip the blocks that are between two matching comment
        /// blocks. This expects properly formatted MarkupElements file:
        ///      <!--CONTROL1-->
        ///      ... don't include this
        ///      <!--CONTROL1-->
        /// </summary>
        /// <param name="ElementNames">Array of strings containing the element to exclude</param>
        /// <returns>Markup XAML minus ElementNames</returns>
        public string GetFullFilteredXamlSource(string[] ElementNames)
        {
            StringReader    SourceFileXAMLReader    = new StringReader(_sourceFileXAML);
            ArrayList       FilteredControlsExp     = new ArrayList();
            string          FilteredXaml            = String.Empty;
            string          XamlLine                = String.Empty;
            bool            FoundOpeningLine, FoundClosingLine;
            
            foreach (string FilteredControl in ElementNames)
            {
                // regex for "<!-- -->"
                FilteredControlsExp.Add(new Regex(@"\s*<!--\s*" + FilteredControl + @"\s*-->\s*", RegexOptions.Compiled));
            }

            FoundOpeningLine    = false;
            FoundClosingLine    = true;

            while ( ( XamlLine = SourceFileXAMLReader.ReadLine() ) != null )
            {
                // find all Control tags
                foreach (Regex exp in FilteredControlsExp)
                {
                    if ((exp.Match(XamlLine)).Success)
                    {
                        if (!FoundOpeningLine)
                        {
                            // Opening line found
                            FoundOpeningLine    = true;
                            FoundClosingLine    = false;
                        }
                        else
                        {
                            // Closing line found
                            FoundOpeningLine    = false;
                            FoundClosingLine    = true;
                        }

                        break;
                    }
                }

                // only save content outside of the Opening/Closing comment blocks
                // this doesn't skip the closing comment line 
                if (!FoundOpeningLine && FoundClosingLine)
                    FilteredXaml += XamlLine;
            }

            return FilteredXaml;
        }


        /// <summary>
        /// This function will return an element given target element name. Case insensitive.
        /// 
        /// ex. GetElement("Button") returns an FrameworkElement containing a Button from 
        /// 
        /// For example, "Row" element can only be used in "Table" or else an InvalidContext exception
        /// is thrown. Calling GetElementParent("Row") will return a Table element containing a "Row"
        /// that can be found with Name="ELEMENTFACTORY_ROW". The element returned will be in canonical
        /// form ready to be attached to the model tree.
        /// 
        /// </summary>
        /// <param name="ElementName">Name of the target element. ex "Row"</param>
        /// <returns>
        ///     1. NULL if element in elementTag can be attached to any parent in any context,
        ///     2. FrameworkElement containing the parent element for elementTag. ex: "Table" if elementTag="Row"</returns>
        public DependencyObject GetLogicalElement(string ElementName)
        {
            TargetElementName = ElementName;
            TargetElementXAML = GetElementXAML(TargetElementName);

            DependencyObject block = System.Windows.Markup.XamlReader.Load( IntegrationUtilities.GetStreamFromString(TargetElementXAML) ) as FrameworkElement;

            // find the element by it's hard-coded Name in MarkupElements.xaml
            TargetElement = LogicalTreeHelper.FindLogicalNode(block, TARGET_ELEMENT_XAML_ID_PREFIX + TargetElementName.ToUpper());

            // find TargetElement's tree if there is one. NULL if there isn't.
            TargetElementRoot = LogicalTreeHelper.FindLogicalNode(block, TARGET_ELEMENT_PARENT_XAML_ID_PREFIX + TargetElementName.ToUpper());

            return TargetElement;
        }

        /// <summary>
        /// Wrapper for GetLogicalElement with return value cast as FrameworkElement
        /// </summary>
        public FrameworkElement GetElement(string ElementName)
        {
            return GetLogicalElement(ElementName) as FrameworkElement;
        }
        
        /// <summary>
        /// Wrapper for GetLogicalElement with return value cast as FrameworkContentElement
        /// </summary>
        public FrameworkContentElement GetContentElement(string ElementName)
        {
            return GetLogicalElement(ElementName) as FrameworkContentElement;
        }
        
        /// <summary>
        /// Returns Element for the given Element Name as DependencyObject
        /// </summary>
        /// <param name="ElementName"></param>
        /// <returns></returns>
        public DependencyObject GetDependencyObject(string ElementName)
        {
            return GetLogicalElement(ElementName) as DependencyObject;
        }


        public readonly     string SourceFile;         // Filename of the ElementFactory XAML file
        public              string TargetElementName;  // the name of the element to find and create
        public              string TargetElementXAML;  // target element markup 
        public              DependencyObject TargetElement      = null; // Target element defined by TARGET_ELEMENT_XAML_ID_PREFIX
        public              DependencyObject TargetElementRoot  = null; // this may be NULL if the element tree for Target doesn't define TARGET_ELEMENT_PARENT_XAML_ID_PREFIX
        private readonly    string _sourceFileXAML;
        private const       string TARGET_ELEMENT_XAML_ID_PREFIX           = "ELEMENTFACTORY_"; // Name of the TargetElement
        private const       string TARGET_ELEMENT_PARENT_XAML_ID_PREFIX    = "ELEMENTFACTORY_PARENT_"; // Name prefix of TargetElement's parent
    }
}

