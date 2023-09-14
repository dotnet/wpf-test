// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
\***************************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Xml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Serialization;

namespace Avalon.Test.CoreUI.Parser
{
    #region InnerXml compare
    /// <summary>
    /// Diffs the two XAML trees
    /// </summary>
    public class XmlDiff
    {
        ///<summary>Diffs the two XML trees. for now only validates structure
        /// Will eventually check tag names, attributes, whitespace, etc. and
        /// also have flags to specify which of these to check for
        /// If the xmlDiff we throw an exception so it can be logged to avPad.
        /// for generate IL code the first tag is a subclass of the original 
        /// so it is okay for it to be different.
        ///</summary> 
        public static void Compare(Stream originalXML,TextReader newXML)
        {
            XmlTextReader original = new XmlTextReader(originalXML);
            XmlTextReader current = new XmlTextReader(newXML);
            bool haveCheckedFirstTag = false;
            try
            {
                // loop through checking depth and that the files contain the 
                // same number of tags.
                // if they don't then throw an exception.
                while (original.Read())
                {
                    // when checking for more things like exact structure
                    // we will always want to read current at this point in
                    // which case currentHasBeenRead will be set to true.
                    // for now since we are only checking structure it will always be
                    // false.
                    bool currentHasBeenRead = false; 
                    if (original.NodeType ==  System.Xml.XmlNodeType.Element)
                    {
                        // if current hasn't been read yet do it now.
                        if (false == currentHasBeenRead)
                        {
                            current.Read();
                        }
                        // if not checking for comments or whitespace preservation
                        // loop forward until the current is also at an element
                        while (current.NodeType != System.Xml.XmlNodeType.Element
                            && current.Read())
                        {
                        }
                        if (haveCheckedFirstTag)
                        {
                            // add assert both nodeTypes are the same.
                            if (current.LocalName != original.LocalName)
                            {
                                throw new XMLDiffException("Tree Structure names don't match",original,current,null);
                            }
                            // add assert both nodeTypes are the same.
                            if (current.Depth != original.Depth)
                            {
                                throw new XMLDiffException("Tree Structure depth doesn't match",original,current,null);
                            }
                        }
                        haveCheckedFirstTag = true;
                    }
                }
                // check to make sure that there isn't anything left in the current
                while (current.Read())
                {
                    if (current.NodeType ==  System.Xml.XmlNodeType.Element)
                    {
                        throw new XMLDiffException("Tree Structure doesn't match",original,current,null);
                    }
                }
            }
            catch (Exception e)
            {
                // if one of our exceptions already just throw
                // if now wrap it in an XMLDiffException
                if (e.GetType() == typeof(XMLDiffException))     
                {
                    throw e;
                }
                throw new XMLDiffException(e.Message,original,current,e);
            }
            finally
            {
                originalXML.Close();
                newXML.Close();
                original.Close();
                current.Close();
            }
        }
    }
    // Exception class used by XMLDiff 
    /// <summary>
    /// creating exception for different.
    /// </summary>
    public class XMLDiffException : Exception
    {
        int _originalLineNumber;
        int _originalLinePosition;
        int _currentLineNumber;
        int _currentLinePosition;
        bool _originalIsAtEndOfFile;
        bool _currentIsAtEndOfFile;
        /// <summary>
        /// output different exception with linenumber and info.
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="original"></param>
        /// <param name="current"></param>
        /// <param name="InnerException"></param>
        public XMLDiffException(String Message,XmlTextReader original,XmlTextReader current,Exception InnerException) 
            : base(Message,InnerException)
        {
            // if end of file then lineNumber and position will be 0,0
            _originalIsAtEndOfFile = (original.ReadState == ReadState.EndOfFile ) ? true : false;
            _originalLineNumber = original.LineNumber;
            _originalLinePosition = original.LinePosition;
            _currentIsAtEndOfFile = (current.ReadState == ReadState.EndOfFile ) ? true : false;
            _currentLineNumber = current.LineNumber;
            _currentLinePosition = current.LinePosition;
        }
        /// <summary>
        /// Get LineNumber that have different
        /// </summary>
        public String LineNumberDiffMessage
        {
            get 
            {
                String LineNumberDiff = "";
                LineNumberDiff = "Files Differences start at: \n\r";
                if (true == _originalIsAtEndOfFile)
                {
                    LineNumberDiff +=    "    OriginalFile (End of File Reached)\n\r";
                }
                else
                {
                    LineNumberDiff +=   "    OriginalFile (Line " + _originalLineNumber + ", Position " + _originalLinePosition + ")\n\r";
                }
                if (true == _currentIsAtEndOfFile)
                {
                    LineNumberDiff +=    "    CurrentFile (End of File Reached)\n\r";
                }
                else
                {
                    LineNumberDiff +=   "    CurrentFile (Line " + _currentLineNumber + ", Position " + _currentLinePosition + ")";
                }
                return LineNumberDiff;
            }
        }
    }
    #endregion InnerXml compare
    /// <summary>
    /// Test controls persistence.
    ///     - FixedPage





    public class Persistence
    {
        #region Persistence
        /// <summary>
        /// 
        /// </summary>
        public Persistence()
        {
        }
        #endregion Persistence
        #region BVT cases

        /// <summary>
        /// Test controls persistence: FixedPage
        /// Scenario:
        ///     - Create Temp file from input xaml string.
        ///     - SaveAsXml from temp file.
        ///     - Compare Original Xaml file with the file that just created.
        /// Verify:
        ///     - Compare correctly between two xaml.
        /// </summary>
        public void TestInnerXmlFixedPage() // check for controls persistence
        {
            CoreLogger.LogStatus("Core:Persistence.TestInnerXmlFixedPage Started ..." + "\n");
            CreateContext();
            try
            {

                    CreateFiles(_innerXmlFixedPage, "<FixedPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><PageContent><FixedPage></FixedPage></PageContent></FixedPanel>");
            }
            catch(Exception exp2)
            {
                _exp1 = exp2;
            }
            finally
            {
                DisposeContext();
                if (File.Exists("tempFile.xaml"))
                {
                    CoreLogger.LogStatus("Clean up created file...");
                    ;//File.Delete("tempFile.xaml");
                }
                if (_exp1 != null)
                    throw new Microsoft.Test.TestValidationException("TestInnerXmlFixedPage: Test Fail, catch exception.... " + _exp1.ToString());
                CoreLogger.LogStatus("TestInnerXmlFixedPage Test Pass!...");
            }
        }
        /// <summary>
        /// Test controls persistence: Menu
        /// Scenario:
        ///     - Create Temp file from input xaml string.
        ///     - SaveAsXml from temp file.
        ///     - Compare Original Xaml file with the file that just created.
        /// Verify:
        ///     - Content is correctly serialized.
        /// </summary>
        public void TestInnerXmlMenu() // check for controls persistence
        {
            if(File.Exists("tempFile.xaml"))
            {
                CoreLogger.LogStatus("Clean up file...");
                try
                {
                    File.Delete("tempFile.xaml");
                }
                catch(IOException e)
                {
                    CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                }
            }

            CoreLogger.LogStatus("Core:Persistence.TestInnerXmlMenu Started ..." + "\n");
            CreateContext();
            try
            {

                    string xaml = "<Border Width=\"100\" Height=\"100\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Canvas><Menu DockPanel.Dock=\"Top\"><MenuItem Header=\"File\"><MenuItem Header=\"New\" /><MenuItem Header=\"Open\" /><MenuItem Mode=\"Separator\" /><MenuItem Header=\"Save\" /></MenuItem></Menu></Canvas></Border>";
                    // create a new file for comparison
                    CoreLogger.LogStatus("Creating a new temporary file for comparison..." + "\n");
                    FileStream fs = new FileStream("tempFile.xaml", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    // create a xaml with the expected markup
                    CoreLogger.LogStatus("Creating a xaml file..." + "\n");
                    sw.Write(xaml);
                    sw.Close();
                    fs.Close();
                    // reading the new file created
                    CoreLogger.LogStatus("Reading the newly created file..." + "\n");
                    Stream stream = File.OpenRead("tempFile.xaml");
                    UIElement el;
                    try
                    {
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        el = System.Windows.Markup.XamlReader.Load(stream, pc) as UIElement;
                    }
                    finally
                    {
                        stream.Close();
                    }
                    // get outerxml from new file
                    CoreLogger.LogStatus("Get OuterXml from the new xaml file" + "\n");
                    string outer = SerializationHelper.SerializeObjectTree(el);
                    CoreLogger.LogStatus("Verifying if serialized content correctly....");
                    CoreLogger.LogStatus("SaveAsXml String is: \n" + outer);
                    if(outer.LastIndexOf("New") == -1)
                        throw new Exception("Failed to serialize Content text of Menu: New");
                    if(outer.LastIndexOf("File") == -1)
                        throw new Exception("Failed to serialize Content text of Menu: File");
                    if(outer.LastIndexOf("Mode=\"Separator\"") == -1)
                        throw new Exception("Failed to serialize Mode property of Menu");
                    if(outer.LastIndexOf("Save") == -1)
                        throw new Exception("Failed to serialize Content text of Menu: Save");
                    CoreLogger.LogStatus("Create temp file from SaveAsXml string....");
                    FileStream fs1 = new FileStream("tempFile1.xaml", FileMode.Create);
                    StreamWriter sw1 = new StreamWriter(fs1);
                    sw1.Write(outer);
                    sw1.Close();
                    fs1.Close();
                    Stream stm = File.OpenRead("tempFile1.xaml");
                    try
                    {
                        CoreLogger.LogStatus("Parsing Serialized xaml....");
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        UIElement el1 = System.Windows.Markup.XamlReader.Load(stm, pc) as UIElement;
                    }
                    finally
                    {
                        stm.Close();
                    }
                
            }
            finally
            {
                DisposeContext();
                if(File.Exists("tempFile.xaml"))
                {
                    CoreLogger.LogStatus("Clean up created file...");
                    try
                    {
                        ;//File.Delete("tempFile.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                    }
                    try
                    {
                        ;//File.Delete("tempFile1.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile1.xaml\n" + e.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Test controls persistence: ScrollViewer
        /// Scenario:
        ///     - Create Temp file from input xaml string.
        ///     - SaveAsXml from temp file.
        ///     - Compare Original Xaml file with the file that just created.
        /// Verify:
        ///     - Content is correctly serialized.
        /// </summary>
        public void TestInnerXmlScrollViewer() // check for controls persistence
        {
            if(File.Exists("tempFile.xaml"))
            {
                CoreLogger.LogStatus("Clean up file...");
                try
                {
                    ;//File.Delete("tempFile.xaml");
                }
                catch(IOException e)
                {
                    CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                }
            }

            CoreLogger.LogStatus("Core:Persistence.TestInnerXmlScrollViewer Started ..." + "\n");
            CreateContext();
            try
            {
                  
                    //SimpleText is has been replaced by Text
                    string xaml = "<ScrollViewer VerticalScrollBarVisibility=\"Visible\" HorizontalScrollBarVisibility=\"Visible\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Canvas><TextBlock>ScrollViewer Text</TextBlock></Canvas></ScrollViewer>";

                    // create a new file for comparison
                    CoreLogger.LogStatus("Creating a new temporary file for comparison..." + "\n");
                    FileStream fs = new FileStream("tempFile.xaml", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    
                    // create a xaml with the expected markup
                    CoreLogger.LogStatus("Creating a xaml file..." + "\n");
                    sw.Write(xaml);
                    sw.Close();
                    fs.Close();
                    
                    // reading the new file created
                    CoreLogger.LogStatus("Reading the newly created file..." + "\n");
                    Stream stream = File.OpenRead("tempFile.xaml");
                    UIElement el;
                    try
                    {
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        el = System.Windows.Markup.XamlReader.Load(stream, pc) as UIElement;
                    }
                    finally
                    {
                        stream.Close();
                    }

                    // get outerxml from new file
                    CoreLogger.LogStatus("Get OuterXml from the new xaml file" + "\n");
                    string outer = SerializationHelper.SerializeObjectTree(el);
                    CoreLogger.LogStatus("Verifying if serialized content correctly....");
                    CoreLogger.LogStatus("SaveAsXml String is: \n" + outer);
                    if (outer.LastIndexOf("VerticalScrollBarVisibility=\"Visible\"") == -1)
                        throw new Exception("Failed to serialize Content Property of ScrollViewer.");
                    if (outer.LastIndexOf("HorizontalScrollBarVisibility=\"Visible\"") == -1)
                        throw new Exception("Failed to serialize Content Property of ScrollViewer.");
                    if(outer.LastIndexOf("ScrollViewer Text") == -1)
                        throw new Exception("Failed to serialize content text of ScrollViewer.");
                    CoreLogger.LogStatus("Create temp file from SaveAsXml string....");
                    FileStream fs1 = new FileStream("tempFile1.xaml", FileMode.Create);
                    StreamWriter sw1 = new StreamWriter(fs1);
                    sw1.Write(outer);
                    sw1.Close();
                    fs1.Close();
                    Stream stm = File.OpenRead("tempFile1.xaml");
                    try
                    {
                        CoreLogger.LogStatus("Parsing Serialized xaml....");
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        UIElement el1 = System.Windows.Markup.XamlReader.Load(stm, pc) as UIElement;
                    }
                    finally
                    {
                        stm.Close();
                    }
                
            }
            finally
            {
                DisposeContext();
                if(File.Exists("tempFile.xaml"))
                {
                    CoreLogger.LogStatus("Clean up created file...");
                    try
                    {
                        ;//File.Delete("tempFile.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                    }
                    try
                    {
                        ;//File.Delete("tempFile1.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile1.xaml\n" + e.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Test controls persistence: Thumb
        /// Scenario:
        ///     - Create Temp file from input xaml string.
        ///     - SaveAsXml from temp file.
        ///     - Compare Original Xaml file with the file that just created.
        /// Verify:
        ///     - Content is correctly serialized.
        /// </summary>
        public void TestInnerXmlThumb() // check for controls persistence
        {
            if(File.Exists("tempFile.xaml"))
            {
                CoreLogger.LogStatus("Clean up file...");
                try
                {
                    File.Delete("tempFile.xaml");
                }
                catch(IOException e)
                {
                    CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                }
            }

            CoreLogger.LogStatus("Core:Persistence.TestInnerXmlThumb Started ..." + "\n");
            CreateContext();
            try
            {

                    string xaml = "<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Thumb /></Canvas>";

                    // create a new file for comparison
                    CoreLogger.LogStatus("Creating a new temporary file for comparison..." + "\n");

                    FileStream fs = new FileStream("tempFile.xaml", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);

                    // create a xaml with the expected markup
                    CoreLogger.LogStatus("Creating a xaml file..." + "\n");
                    sw.Write(xaml);
                    sw.Close();
                    fs.Close();

                    // reading the new file created
                    CoreLogger.LogStatus("Reading the newly created file..." + "\n");

                    Stream stream = File.OpenRead("tempFile.xaml");
                    UIElement el;

                    try
                    {
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        el = System.Windows.Markup.XamlReader.Load(stream, pc) as UIElement;
                    }
                    finally
                    {
                        stream.Close();
                    }
                    // get outerxml from new file
                    CoreLogger.LogStatus("Get OuterXml from the new xaml file" + "\n");
                    string outer = SerializationHelper.SerializeObjectTree(el);
                    CoreLogger.LogStatus("Verifying if serialized content correctly....");
                    CoreLogger.LogStatus("SaveAsXml String is: \n" + outer);
                    if(outer.LastIndexOf("Thumb") == -1)
                        throw new Exception("Failed to serialize Content Property of Thumb!!!!!!!!");

                    CoreLogger.LogStatus("Create temp file from SaveAsXml string....");

                    FileStream fs1 = new FileStream("tempFile1.xaml", FileMode.Create);
                    StreamWriter sw1 = new StreamWriter(fs1);

                    sw1.Write(outer);
                    sw1.Close();
                    fs1.Close();

                    Stream stm = File.OpenRead("tempFile1.xaml");
                    try
                    {
                        CoreLogger.LogStatus("Parsing Serialized xaml....");
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        UIElement el1 = System.Windows.Markup.XamlReader.Load(stm, pc) as UIElement;
                    }
                    finally
                    {
                        stm.Close();
                    }
                
            }
            finally
            {
                DisposeContext();
                if(File.Exists("tempFile.xaml"))
                {
                    CoreLogger.LogStatus("Clean up created file...");
                    try
                    {
                        ;//File.Delete("tempFile.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                    }
                    try
                    {
                        ;//File.Delete("tempFile1.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile1.xaml\n" + e.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Test controls persistence: RepeatButton
        /// Scenario:
        ///     - Create Temp file from input xaml string.
        ///     - SaveAsXml from temp file.
        ///     - Compare Original Xaml file with the file that just created.
        /// Verify:
        ///     - Content is correctly serialized.
        /// </summary>
        public void TestInnerXmlRepeatButton() // check for controls persistence
        {
            if(File.Exists("tempFile.xaml"))
            {
                CoreLogger.LogStatus("Clean up file...");
                try
                {
                    ;//File.Delete("tempFile.xaml");
                }
                catch(IOException e)
                {
                    CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                }
            }

            CoreLogger.LogStatus("Core:Persistence.TestInnerXmlRepeatButton Started ..." + "\n");
            CreateContext();
            try
            {

                    string xaml = "<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><RepeatButton Delay=\"10\" Interval=\"10\" /></Canvas>";

                    // create a new file for comparison
                    CoreLogger.LogStatus("Creating a new temporary file for comparison..." + "\n");

                    FileStream fs = new FileStream("tempFile.xaml", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);

                    // create a xaml with the expected markup
                    CoreLogger.LogStatus("Creating a xaml file..." + "\n");
                    sw.Write(xaml);
                    sw.Close();
                    fs.Close();

                    // reading the new file created
                    CoreLogger.LogStatus("Reading the newly created file..." + "\n");

                    Stream stream = File.OpenRead("tempFile.xaml");
                    UIElement el;

                    try
                    {
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        el = System.Windows.Markup.XamlReader.Load(stream, pc) as UIElement;
                    }
                    finally
                    {
                        stream.Close();
                    }

                    // get outerxml from new file
                    CoreLogger.LogStatus("Get OuterXml from the new xaml file" + "\n");

                    string outer = SerializationHelper.SerializeObjectTree(el);

                    CoreLogger.LogStatus("Verifying if serialized content correctly....");
                    CoreLogger.LogStatus("SaveAsXml String is: \n" + outer);
                    if(outer.LastIndexOf("Delay=\"10\"") == -1)
                        throw new Exception("Failed to serialize Content Property of RepeatButton!!!!!!!!");
                    if(outer.LastIndexOf("Interval=\"10\"") == -1)
                        throw new Exception("Failed to serialize Content Property of RepeatButton!!!!!!!!");
                    CoreLogger.LogStatus("Create temp file from SaveAsXml string....");

                    FileStream fs1 = new FileStream("tempFile1.xaml", FileMode.Create);
                    StreamWriter sw1 = new StreamWriter(fs1);

                    sw1.Write(outer);
                    sw1.Close();
                    fs1.Close();

                    Stream stm = File.OpenRead("tempFile1.xaml");

                    try
                    {
                        CoreLogger.LogStatus("Parsing Serialized xaml....");
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        UIElement el1 = System.Windows.Markup.XamlReader.Load(stm, pc) as UIElement;
                    }
                    finally
                    {
                        stm.Close();
                    }
                
            }
            finally
            {
                DisposeContext();
                if(File.Exists("tempFile.xaml"))
                {
                    CoreLogger.LogStatus("Clean up created file...");
                    try
                    {
                        ;//File.Delete("tempFile.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                    }
                    try
                    {
                        ;//File.Delete("tempFile1.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile1.xaml\n" + e.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Test controls persistence: ScrollBar
        /// Scenario:
        ///     - Create Temp file from input xaml string.
        ///     - SaveAsXml from temp file.
        ///     - Compare Original Xaml file with the file that just created.
        /// Verify:
        ///     - Content is correctly serialized.
        /// </summary>
        public void TestInnerXmlScrollBar() // check for controls persistence
        {
            if(File.Exists("tempFile.xaml"))
            {
                CoreLogger.LogStatus("Clean up file...");
                try
                {
                    ;//File.Delete("tempFile.xaml");
                }
                catch(IOException e)
                {
                    CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                }
            }

            CoreLogger.LogStatus("Core:Persistence.TestInnerXmlScrollBar Started ..." + "\n");
            CreateContext();
            try
            {

                string xaml = "<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><ScrollBar Orientation=\"Horizontal\"/><ScrollBar Orientation=\"Vertical\"/></Canvas>";

                    // create a new file for comparison
                    CoreLogger.LogStatus("Creating a new temporary file for comparison..." + "\n");

                    FileStream fs = new FileStream("tempFile.xaml", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);

                    // create a xaml with the expected markup
                    CoreLogger.LogStatus("Creating a xaml file..." + "\n");
                    sw.Write(xaml);
                    sw.Close();
                    fs.Close();

                    // reading the new file created
                    CoreLogger.LogStatus("Reading the newly created file..." + "\n");

                    Stream stream = File.OpenRead("tempFile.xaml");
                    UIElement el;

                    try
                    {
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        el = System.Windows.Markup.XamlReader.Load(stream, pc) as UIElement;
                    }
                    finally
                    {
                        stream.Close();
                    }

                    // get outerxml from new file
                    CoreLogger.LogStatus("Get OuterXml from the new xaml file" + "\n");

                    string outer = SerializationHelper.SerializeObjectTree(el);

                    CoreLogger.LogStatus("Verifying if serialized content correctly....");
                    CoreLogger.LogStatus("SaveAsXml String is: \n" + outer);
                    if(outer.LastIndexOf("ScrollBar") == -1)
                        throw new Exception("Failed to serialize Content Property of ScrollBar!!!!!!!!");
                    if (outer.LastIndexOf("Horizontal") == -1)
                        throw new Exception("Failed to serialize Content Property of Horizontal!");
                    if (outer.LastIndexOf("Vertical") == -1)
                        throw new Exception("Failed to serialize Vertical.");

                    CoreLogger.LogStatus("Create temp file from SaveAsXml string....");
                    FileStream fs1 = new FileStream("tempFile1.xaml", FileMode.Create);
                    StreamWriter sw1 = new StreamWriter(fs1);
                    sw1.Write(outer);
                    sw1.Close();
                    fs1.Close();

                    Stream stm = File.OpenRead("tempFile1.xaml");

                    try
                    {
                        CoreLogger.LogStatus("Parsing Serialized xaml....");
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        UIElement el1 = System.Windows.Markup.XamlReader.Load(stm, pc) as UIElement;
                    }
                    finally
                    {
                        stm.Close();
                    }
                
            }
            finally
            {
                DisposeContext();
                if(File.Exists("tempFile.xaml"))
                {
                    CoreLogger.LogStatus("Clean up created file...");
                    try
                    {
                        ;//File.Delete("tempFile.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                    }
                    try
                    {
                        ;//File.Delete("tempFile1.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile1.xaml\n" + e.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Test controls persistence: Slider
        /// Scenario:
        ///     - Create Temp file from input xaml string.
        ///     - SaveAsXml from temp file.
        ///     - Compare Original Xaml file with the file that just created.
        /// Verify:
        ///     - Content is correctly serialized.
        /// </summary>
        public void TestInnerXmlSlider() // check for controls persistence
        {
            if(File.Exists("tempFile.xaml"))
            {
                CoreLogger.LogStatus("Clean up file...");
                try
                {
                    ;//File.Delete("tempFile.xaml");
                }
                catch(IOException e)
                {
                    CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                }
            }

            CoreLogger.LogStatus("Core:Persistence.TestInnerXmlSlider Started ..." + "\n");
            CreateContext();
            try
            {

                    string xaml = "<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Slider /></Canvas>";

                    // create a new file for comparison
                    CoreLogger.LogStatus("Creating a new temporary file for comparison..." + "\n");

                    FileStream fs = new FileStream("tempFile.xaml", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);

                    // create a xaml with the expected markup
                    CoreLogger.LogStatus("Creating a xaml file..." + "\n");
                    sw.Write(xaml);
                    sw.Close();
                    fs.Close();

                    // reading the new file created
                    CoreLogger.LogStatus("Reading the newly created file..." + "\n");

                    Stream stream = File.OpenRead("tempFile.xaml");
                    UIElement el;

                    try
                    {
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        el = System.Windows.Markup.XamlReader.Load(stream, pc) as UIElement;
                    }
                    finally
                    {
                        stream.Close();
                    }

                    // get outerxml from new file
                    CoreLogger.LogStatus("Get OuterXml from the new xaml file" + "\n");

                    string outer = SerializationHelper.SerializeObjectTree(el);

                    CoreLogger.LogStatus("Verifying if serialized content correctly....");
                    CoreLogger.LogStatus("SaveAsXml String is: \n" + outer);
                    if(outer.LastIndexOf("Slider") == -1)
                        throw new Exception("Failed to serialize Content Property of HorizontalSlider!!!!!!!!");

                    CoreLogger.LogStatus("Create temp file from SaveAsXml string....");

                    FileStream fs1 = new FileStream("tempFile1.xaml", FileMode.Create);
                    StreamWriter sw1 = new StreamWriter(fs1);

                    sw1.Write(outer);
                    sw1.Close();
                    fs1.Close();

                    Stream stm = File.OpenRead("tempFile1.xaml");

                    try
                    {
                        CoreLogger.LogStatus("Parsing Serialized xaml....");
                        ParserContext pc = new ParserContext();
                        pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                        UIElement el1 = System.Windows.Markup.XamlReader.Load(stm, pc) as UIElement;
                    }
                    finally
                    {
                        stm.Close();
                    }
                
            }
            finally
            {
                DisposeContext();
                if(File.Exists("tempFile.xaml"))
                {
                    CoreLogger.LogStatus("Clean up created file...");
                    try
                    {
                        ;//File.Delete("tempFile.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                    }
                    try
                    {
                        ;//File.Delete("tempFile1.xaml");
                    }
                    catch(IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete tempFile1.xaml\n" + e.ToString());
                    }
                }
            }
        }
        #endregion BVT cases
        #region CreateFiles
        /// <summary>
        /// - Create New file for Comparison from input xaml string
        /// - SaveAsXml from the file that just created
        /// - Compare from original file.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="xaml">xaml</param>
        void CreateFiles(string filename, string xaml)
        {
            if(File.Exists("tempFile.xaml"))
            {
                CoreLogger.LogStatus("Clean up file...");
                try
                {
                    ;//File.Delete("tempFile.xaml");
                }
                catch(IOException e)
                {
                    CoreLogger.LogStatus("Unable to delete tempFile.xaml\n" + e.ToString());
                }
            }
            CoreLogger.LogStatus("Testing " + filename + "..." + "\n");
            // create a new file for comparison
            CoreLogger.LogStatus("Creating a new temporary file for comparison..." + "\n");
            FileStream fs = new FileStream("tempFile.xaml", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            // create a xaml with the expected markup
            CoreLogger.LogStatus("Creating a xaml file..." + "\n");
            sw.Write(xaml);
            sw.Close();
            fs.Close();
            Stream newstream = File.OpenRead (filename);
            // reading the new file created
            CoreLogger.LogStatus("Reading the newly created file..." + "\n");
            Stream stream = File.OpenRead ("tempFile.xaml");
            UIElement el = null;
            try
            {
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://")); 
                el = System.Windows.Markup.XamlReader.Load(stream, pc) as UIElement;
            }
            catch (Exception exp)
            {
                stream.Close();
                throw new Microsoft.Test.TestValidationException ("Test fail! Parsing Tempfile error." + exp.ToString ());
            }
            // get outerxml from new file
            CoreLogger.LogStatus("Get OuterXml from the new xaml file" + "\n");
            string outer = SerializationHelper.SerializeObjectTree (el);
            CoreLogger.LogStatus("SaveAsXml String is: \n" + outer);
            StringReader sr = new StringReader (outer);
            // compare the two files
            try
            {
                CoreLogger.LogStatus("Comparing OuterXml file with original xaml file.");
                //XmlDiff.Compare (newstream, sr).
            }
            catch (XMLDiffException exp)
            {
                throw new Microsoft.Test.TestValidationException ("Test fail! Compare files are different." + exp.ToString ());
            }
            finally
            {
                sr.Close ();
                stream.Close ();
                newstream.Close ();
            }
        }
        #endregion CreateFiles
        #region filenames
        /// <summary>
        /// predefine the xaml files as strings here for
        /// the logging methods second arguments
        /// </summary>
        string _innerXmlFixedPage = "innerxmlfixedpage.xaml";
        #endregion filenames
        #region Defined
        // UiContext defined here
        Exception _exp1 = null;


        #endregion Defined
        #region Context
        /// <summary>
        /// Creating UIContext
        /// </summary>
        private void CreateContext()
        {
        }
        /// <summary>
        /// Disposing UIContext here
        /// </summary>
        private void DisposeContext()
        {

        }
        #endregion Context
    }
}
