// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Copyright (C) Microsoft Corporation.  All rights reserved.
// Description: DRTBase for fixed payload testing.
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.IO.Packaging;
using System.IO;
using System.Text;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Layout DRT for content presentation layouts.
    // ----------------------------------------------------------------------
    internal class DrtFixedBase : DrtBase
    {
        // ------------------------------------------------------------------
        // Application entry point.
        // ------------------------------------------------------------------
        [STAThread]
        internal static int Main(string[] args)
        {
            DrtBase drt = new DrtFixedBase();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return drt.Run(args);
        }

        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        private DrtFixedBase()
        {
            //this.AssertsAsExceptions = false;
            this.WindowSize = new Size(850, 650);
            this.WindowTitle = "Fixed Hyperlink DRT";
            this.TeamContact = "WPF";
            this.Contact = "Microsoft";
            this.DrtName = "DrtFixedHyperlink";

            this.Suites = new DrtTestSuite[] {
                new FixedHyperlinkSuite(),
                new FixedStructureSuite()
            };
        }

        // ------------------------------------------------------------------
        // Handle command-line arguments one-by-one.
        // ------------------------------------------------------------------
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
            {
                return true;
            }

            // Process arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:
            if (option)
            {
                switch (arg)
                {
                    case "arg":
                        break;

                    case "in":             // option with parameter:  -use something
                        _inFileName = args[++k];
                        // this also implies keeping alive
                        KeepAlive = true;
                        BlockInput = false;
                        break;

                    default: // Unknown option. Don't handle it.
                        return false;
                }
                return true;
            }
            return false;
        }

        public String InFileName
        {
            get { return _inFileName; }
        }

        // ------------------------------------------------------------------
        // Print a description of command line arguments.
        // ------------------------------------------------------------------
        protected override void PrintOptions()
        {
            Console.WriteLine("     /IN InputFile   The input file. Use DRTFiles\\Payloads\\PageContent\\MSNMain.xaml by default.");
            Console.WriteLine("                     Can input loose xaml file as *.xaml or XPS document as *.xps.");

            base.PrintOptions();
        }

        public object LoadXaml(string filename)
        {
            // Set the baseUri of ParserContext
            FileInfo info = new FileInfo(filename);
            Uri baseUri = new Uri(info.FullName);
            ParserContext pc = new ParserContext();
            pc.BaseUri = baseUri;

            string fullname = DRT.BaseDirectory + filename;
            System.IO.Stream stream = File.OpenRead(fullname);

            return XamlReader.Load(stream, pc);
        }

        private static PackagePart GetPackageStartingPart(Package package)
        {
            PackagePart packagePart = null;

            PackageRelationship startingPartRelationship = null;

            foreach (PackageRelationship rel in package.GetRelationshipsByType("http://schemas.microsoft.com/xps/2005/06/fixedrepresentation"))
            {
                startingPartRelationship = rel;
            }

            if (startingPartRelationship != null)
            {
                Uri startPartUri = PackUriHelper.ResolvePartUri(startingPartRelationship.SourceUri, startingPartRelationship.TargetUri);

                if (package.PartExists(startPartUri))
                {
                    packagePart = (package.GetPart(startPartUri));
                }
            }
            return packagePart;
        }

        public object LoadPackage(string filename)
        {
            // 1. First to get the stream.
            Package package = Package.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            PackagePart packs = GetPackageStartingPart(package);
            Stream f = packs.GetStream();

            // 2. Add Package to Package Store
            FileInfo file = new FileInfo(filename);
            _fileUri = new Uri(file.FullName);
            PackageStore.AddPackage(_fileUri, package);

            // 3. Set the property ParserContext.
            Uri bpu = PackUriHelper.Create(_fileUri);
            ParserContext parserContext = new ParserContext();
            parserContext.BaseUri = bpu;

            // 4. Load the content.
            parserContext.XamlTypeMapper = XamlTypeMapper.DefaultMapper;

            object element = XamlReader.Load(f, parserContext);

            return element;
        }

        public void Cleanup()
        {
            PackageStore.RemovePackage(_fileUri);
        }

        // ------------------------------------------------------------------
        // Drt mode.
        // ------------------------------------------------------------------
        private String _inFileName;
        private Uri _fileUri;
    }
}
