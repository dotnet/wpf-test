// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Printing;
using System.Threading;
using System.Windows.Automation;
using System.Windows;
using System.Windows.Input;
using System.Windows.Xps;
using Microsoft.Internal.OSVersionHelper;
using WPF.Printing.Common.Helpers;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace DRT
{
    public sealed class DrtPrinting : DrtBase
    {
        #region Static bootstrap method
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DrtPrinting();
            return drt.Run(args);
        }
        #endregion Static bootstrap method
        
        #region Constructor
        private DrtPrinting()
        {
            WindowTitle = PrintQueueTestSuite.Title;
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = PrintQueueTestSuite.Title;
            Suites = new DrtTestSuite[]
            {
                new PrintQueueTestSuite(),
                null            // list terminator - optional
            };
        }

        #endregion Constructor
    }
    
    public sealed class PrintQueueTestSuite: DrtTestSuite
    {
        public PrintQueueTestSuite(): base(Title)
        {
            Contact = "Microsoft";

            // Prefrentially use Microsoft PDF printer
            if(PrinterManager.IsPrinterInstalled(_msftPdfPrinterName))
            {
                _printerName = _msftPdfPrinterName;
                _printerPort = "PORTPROMPT:";
                _textBoxControlType = ControlType.ComboBox;
                _expectedOutputFile = @"DrtFiles\Printing\PrintingDrt.pdf";
                _editControlIndex = 3;
                _verifyFunction = VerifyPDFPrint;
            }
            else if(VersionHelper.IsWindows8OrGreater())
            {
                _printerName = "HP Color LaserJet 5550 PS Class Driver";
                _printerPort = "PORTPROMPT:";
                _textBoxControlType = ControlType.ComboBox;
                _expectedOutputFile = @"DrtFiles\Printing\ExpectedOutputWin8+.txt";
                _editControlIndex = 2;
                _verifyFunction = VerifyPrint;
            }
            else
            {
                _printerName = "MS Publisher Color Printer";
                _printerPort = "FILE:";
                _textBoxControlType = ControlType.Edit;
                _expectedOutputFile = @"DrtFiles\Printing\ExpectedOutputWin7.txt";
                _editControlIndex = 2;
                _verifyFunction = VerifyPrint;
            }
        }
        
        public override DrtTest[] PrepareTests()
        {
            // initialize the suite here.  This includes loading the tree.

            // return the lists of tests to run against the tree
            return new DrtTest[]{

                        new DrtTest( Initialize ),
                        new DrtTest( StartPrinting ),
                        new DrtTest( FindSaveAsWindow ),
                        new DrtTest( InputPrintOutputText ),
                        new DrtTest( ClickSaveButton ),
                        new DrtTest( _verifyFunction ),
                        null        // list terminator - optional
                        };
        }
        
        private void Initialize()
        {
            DRT.LogOutput("Initialize");
            DRT.LogOutput("Using printer: " + _printerName);
            DRT.Assert(File.Exists(_printFile),"Print input file was not found in execution directory");
            DRT.Assert(File.Exists(_expectedOutputFile),String.Format("reference file for expected output was not found in execution directory"));
            
            _printOutput = Path.Combine(Directory.GetCurrentDirectory(), _printOutput);
            if (File.Exists(_printOutput))
            {
                File.Delete(_printOutput);
            }

            bool printerInstalled = PrinterManager.InstallPrinter(_printerName, _printerPort);
            DRT.Assert(printerInstalled, String.Format("Couldn't install printer: {0}", _printerName));

            if (PrinterManager.GetDefaultPrinter() != _printerName)
            {
                bool printerIsDefault = PrinterManager.SetDefaultPrinter(_printerName);
                DRT.Assert(printerIsDefault, String.Format("Couldn't set printer \"{0}\" as default", _printerName));
            }
        }
        
        private void StartPrinting() 
        {
            DRT.LogOutput("StartPrinting");
            Thread thread = new Thread(AddPrintJob);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
        
        private void AddPrintJob()
        {
            DRT.LogOutput("AddPrintJob");
            using (PrintQueue queue = LocalPrintServer.GetDefaultPrintQueue())
            {
                queue.AddJob("test", _printFile, false);
            }
            _printDialogEvent.Set();
        }
        
        private void FindSaveAsWindow()
        {
            DRT.LogOutput("FindSaveAsWindow");
            Thread.Sleep(1000);

            // Find the `Save As' dialog from the 'DrtPrinting' process
            Process[] processes = Process.GetProcessesByName(_drtPrintingProcessName);
            DRT.Assert(processes.Length == 1, "DrtPrinting process not found");
            Process currentProcess = processes[0];

            _saveAsWindow = AutomationHelper.FindElementWithPropertyValue(AutomationElement.RootElement,
                                                                      AutomationElementIdentifiers.ProcessIdProperty,
                                                                      currentProcess.Id,
                                                                      2,
                                                                      false);
            
            DRT.Assert(_saveAsWindow != null, "Couldn't find \"Save As\" dialog");
        }
        
        private void InputPrintOutputText()
        {
            DRT.LogOutput("InputPrintOutputText");
            AutomationElement EditBox = AutomationHelper.FindElementWithPropertyValue(_saveAsWindow, AutomationElementIdentifiers.ControlTypeProperty,
                                                                                      _textBoxControlType,
                                                                                      _editControlIndex,
                                                                                      false);
            DRT.Assert(EditBox != null, String.Format("Couldn't find \"File name:\" editable text box in {0} window", _saveAsWindow));
            AutomationHelper.SetText(EditBox, _printOutput);
        }
        
        private void ClickSaveButton()
        {           
            DRT.LogOutput("ClickSaveButton");
            // Hitting Enter while focused on the save window has the same effect as clicking the save button
            _saveAsWindow.SetFocus();
            AutomationHelper.InputKey(Key.Enter,0);
            
        }


        private IEnumerable<(int index, string content)> GetPDFObjects(string pdfFilePath)
        {
            var indexedContent = new List<(int index, string content)>();
            using (StreamReader reader = new StreamReader(pdfFilePath))
            {
                while (!reader.EndOfStream)
                {
                    var words = reader.ReadLine().Split(' ');
                    if (words.Length == 3)
                    {
                        if (words[2] == "obj")
                        {
                            // PDF section starts here
                            var index = Convert.ToInt32(words[0]);
                            var content = "";
                            while (!reader.EndOfStream)
                            {
                                var contentLine = reader.ReadLine();
                                if (contentLine == "endobj")
                                    break;

                                content += contentLine;
                            }

                            indexedContent.Add((index, content));
                        }
                    }

                }
            }
            return indexedContent;
        }

        private string GetPDFObjectHash(string pdfFilePath)
        {
            var indexedContent = GetPDFObjects(pdfFilePath);
            string result = string.Join("\r\n", 
                indexedContent
                    .OrderBy(c => c.index)
                    .Select(c => c.content)
                    .SkipLast(1) // Skipping the last block with metadata
                );

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(result));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void VerifyPDFPrint()
        {
            DRT.LogOutput("VerifyPDFPrint");
            _printDialogEvent.WaitOne(15000);

            var expectedHash = GetPDFObjectHash(_expectedOutputFile);
            var resultHash = GetPDFObjectHash(_printOutput);
            DRT.Assert(expectedHash == resultHash, $"PostScript output does not match expected output. expectedHash={expectedHash} and resultHash={resultHash}");
        }

        private void VerifyPrint()
        {
            DRT.LogOutput("VerifyPrint");
            _printDialogEvent.WaitOne(15000);
            DRT.Assert(File.Exists(_printOutput),"Print output file was not generated");
            
            string[] referenceLines = File.ReadAllLines(_expectedOutputFile);
            string[] outputLines = File.ReadAllLines(_printOutput);
            int lineIndex = 0;
            
            foreach(string line in outputLines)
            {
                if (line.Equals(referenceLines[lineIndex]))
                {
                   if (++lineIndex >= referenceLines.Length)
                   {
                        return;
                   }
                }
            }
            DRT.Fail("PostScript output does not match expected output");
        }                                            
        
        public static string Title = "Print Queue"; 
        
        private AutoResetEvent _printDialogEvent = new AutoResetEvent(false);

        // _expectedOutputFile is set to the path of a file containing a snippet of PostScript that is
        // expected to be included in the PostScript file generated by the Printer driver
        private string _expectedOutputFile;
        private string _printOutput = @"DrtFiles\Printing\_printOutput.ps";
        private const string _printFile = @"DrtFiles\Printing\PrintingDrt.xps";
        private const string _drtPrintingProcessName = @"DrtPrinting";
        private const string _msftPdfPrinterName = "Microsoft Print to PDF";
        private int _editControlIndex = 2;
        private Action _verifyFunction;
                
        private AutomationElement _saveAsWindow;
        
        private string _printerName;
        private string _printerPort;
        private ControlType _textBoxControlType;
    }
}
