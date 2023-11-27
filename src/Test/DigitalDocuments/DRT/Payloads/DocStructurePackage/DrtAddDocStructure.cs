// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Copyright (C) Microsoft Corporation.  All rights reserved. 
//

using System;
using System.IO;
using System.Threading;
using System.IO.Packaging;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Shapes;

using System.Printing;
using DRT;

namespace DrtPrinting
{
    internal class DrtAddDocStructure : DrtBase
    {
       
        [STAThread]
        static int Main(String[] args)
        {
            DrtBase drt = new DrtAddDocStructure();
            return drt.Run(args);
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        private DrtAddDocStructure()
        {
            WindowTitle = "Reach package creation";
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = "DrtAddDocStructure";
            Suites = new DrtTestSuite[]{
                        new DrtAddDocStructureSuite(),
                        null            // list terminator - optional
                        };

            _docNumber = 0;
            _pageNumber = -1;
            _isDrtRun = true;
        }

        // Override this in derived classes to handle command-line arguments one-by-one.
        // Return true if handled.
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
                return true;

            // process your own arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:

            if (option)
            {
                switch (arg)    // arg is lower-case, no leading - or /
                {
                    case "in":             // option with parameter:  -use something
                        _inFileName = args[++k];
                        _isDrtRun = false;
                        break;

                    case "add":             // option with parameter:  -use something
                        _addFileName = args[++k];
                        _isDrtRun = false;
                        break;

                    case "doc":             // option with parameter:  -use something
                        _docNumber = Convert.ToInt32(args[++k]);
                        _isDrtRun = false;
                        break;

                    case "page":             // option with parameter:  -use something
                        _pageNumber = Convert.ToInt32(args[++k]);
                        _isDrtRun = false;
                        break;

                    default:                // unknown option.  don't handle it
                        return false;
                }
                return true;
            }
            return false;
        }

        protected override void PrintOptions()
        {
            Console.WriteLine("Add Document Structure in the package.");
            Console.WriteLine("     /IN InputContainer   The input container file.");
            Console.WriteLine("     /Add StrutureFile    The document structure files.");
            Console.WriteLine("     /Doc DocNumber.      The default is zero");
            Console.WriteLine("     /Page PageNumber.    The default means it is document structure.");
            Console.WriteLine("If no argue, perform the DRT run.");
            //  Examples:
            //  drtAddDocStructure /IN  DrtFiles\Payloads\Sequence\sample.xps /Add DrtFiles\Payloads\Sequence\MSFTQ3-04balancesheets_page_1.dsxaml /Page 0
        }

        /// <summary>
        /// Called when the DRT is starting up -- after the Dispatcher has been created,
        /// before any suites are started.
        /// </summary>
        protected override void OnStartingUp()
        {
            if (_isDrtRun)
            {
                _inFileName = "DrtFiles\\Payloads\\Sequence\\Test_Document.xps"; 
                _addFileName = "DrtFiles\\Payloads\\Sequence\\MSFTQ3-04balancesheets_page_1.dsxaml";
                _pageNumber = 0;
            }
        }

        public String InFileName
        {
            get { return _inFileName; }
        }
        public String AddFileName
        {
            get { return _addFileName; }
        }
        public int DocNumber
        {
            get { return _docNumber; }
        }

        public int PageNumber
        {
            get { return _pageNumber; }
        }

        public bool IsDrtRun
        {
            get { return _isDrtRun; }
        }

        private String _inFileName;
        private String _addFileName;
        private int _docNumber;
        private int _pageNumber;

        private bool _isDrtRun;

    }


    public sealed class DrtAddDocStructureSuite : DrtTestSuite 
    {

        public DrtAddDocStructureSuite()
            : base("Create package with document structure")
        {
            TeamContact = "WPF";     // if different from DRT
            Contact = "Microsoft";         // if different from DRT
        }


        public override DrtTest[] PrepareTests()
        {
            DrtAddDocStructure drtReach = DRT as DrtAddDocStructure;

            //
            // Prepare the test matrix
            //
            int testIndex = 0;
            DrtTest[] drtTests = new DrtTest[2];

            drtTests[testIndex++] = new DrtTest(StartCreatePackage);

            drtTests[testIndex] = null;
            if (drtReach.IsDrtRun)
            {
                //drtTests[testIndex++] = new DrtTest(AddToExistingPackage);
            }

            return drtTests;
        }

        private
        void
        StartCreatePackage(
            )
        {
            //
            // Prepare the print queue. 
            //
            DrtAddDocStructure drtReach = DRT as DrtAddDocStructure;

            //
            // After this test the file gets corrupted and cannot be used for next iterations.
            // Hence creating a copy of required file and deleting it after test.  
            //
            string sourceFilePath = drtReach.InFileName;
            string targetFileName = System.IO.Path.GetFileNameWithoutExtension(sourceFilePath) + "_Copy.xps";
            string targetFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sourceFilePath), targetFileName);

            // Deleting the copy, if it already exists
            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }

            File.Copy(sourceFilePath, targetFilePath);

            XpsDocument reachPackage = new XpsDocument(targetFilePath, FileAccess.ReadWrite);
            IXpsFixedDocumentSequenceReader docSequence = reachPackage.FixedDocumentSequenceReader;
            ICollection<IXpsFixedDocumentReader> fixedDocuments = docSequence.FixedDocuments;

            int Doccount = 0, PageCount = 0;
            XpsResource docStructure, pageStructure;

            foreach (IXpsFixedDocumentReader ifdr in fixedDocuments)
            {
                if (Doccount == drtReach.DocNumber)
                {
                    if (drtReach.PageNumber < 0)
                    {
                        docStructure = ifdr.AddDocumentStructure();
                        WriteResource(docStructure, drtReach.AddFileName);
                    }
                    else
                    {
                        ICollection<IXpsFixedPageReader> fixedPages = ifdr.FixedPages;
                        PageCount = 0;
                        foreach (IXpsFixedPageReader ifrr in fixedPages)
                        {
                            if (PageCount == drtReach.PageNumber)
                            {
                                pageStructure = ifrr.AddStoryFragment();
                                WriteResource(pageStructure, drtReach.AddFileName);
                            }
                            PageCount++;
                            // how to commit?
                        }

                    }
                }

                Doccount++;
                // how to commit?
            }

            reachPackage.Close();

            // Deleting the Copied XPS File
            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }

            return;
        }

        private
        static
        string[]
        DocStructureTable = {
            ".\\DrtFiles\\Payloads\\Sequence\\MSFTQ3-04Doc.dsxaml",
            null
        };

        private
        static
        string[]
        PageStructureTable = {
            ".\\DrtFiles\\Payloads\\Sequence\\MSFTQ3-04doc_page_1.dsxaml",
            ".\\DrtFiles\\Payloads\\Sequence\\MSFTQ3-04doc_page_2.dsxaml",
            null,
            null
        };

        private
        object
        AddToExistingPackage(
            object arg
            )
        {
            string ExistingPackage = ".\\DrtFiles\\Payloads\\Sequence\\MSFTQ3-04.xps";

            XpsDocument reachPackage = new XpsDocument(ExistingPackage, FileAccess.ReadWrite);
            IXpsFixedDocumentSequenceReader docSequence = reachPackage.FixedDocumentSequenceReader;
            ICollection<IXpsFixedDocumentReader> fixedDocuments = docSequence.FixedDocuments;

            int Doccount = 0, PageCount = 0;
            XpsResource docStructure, pageStructure;

            foreach (IXpsFixedDocumentReader ifdr in fixedDocuments)
            {
                ICollection<IXpsFixedPageReader> fixedPages = ifdr.FixedPages;
                PageCount = 0;
                foreach (IXpsFixedPageReader ifrr in fixedPages)
                {
                    if (PageStructureTable[PageCount] != null)
                    {
                        pageStructure = ifrr.AddStoryFragment();
                        WriteResource(pageStructure, PageStructureTable[PageCount]);
                    }
                    PageCount++;
                    // how to commit?
                }
                if (DocStructureTable[Doccount] != null)
                {
                    docStructure = ifdr.AddDocumentStructure();
                    WriteResource(docStructure, DocStructureTable[Doccount]);
                }
                Doccount++;
                // how to commit?
            }

            reachPackage.Close();

            return null;
        }

        void
        WriteResource(
            XpsResource reachImage,
            string filename
            )
        {
            WriteStream(reachImage.GetStream(), filename);
        }

        private
        void
        WriteStream(
            Stream stream,
            string filename
            )
        {
            FileStream srcStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            byte[] buf = new byte[65536];
            int bytesRead;

            do
            {
                bytesRead = srcStream.Read(buf, 0, 65536);
                stream.Write(buf, 0, bytesRead);
            }
            while (bytesRead > 0);

            srcStream.Close();
        }

    }
};


