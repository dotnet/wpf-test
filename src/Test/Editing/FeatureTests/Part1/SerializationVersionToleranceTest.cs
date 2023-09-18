// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup;

using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{
    /// <summary>
    /// This test case tests TextRange serialization version tolerance between 35sp1 and 40.
    /// Its uses serialized xaml/xamlpackage files generated from GenerateSerializedOuput test (ran on 35 framework)
    /// </summary>
    [Test(1, "TextOM", "SerializationVersionToleranceTest", MethodParameters = "/TestCaseType:SerializationVersionToleranceTest", SupportFiles = @"FeatureTests\Editing\SerializedOutput.Xaml, FeatureTests\Editing\SerializedOutput.XamlPackage")]
    public class SerializationVersionToleranceTest : CustomTestCase
    {
        public override void RunTestCase()
        {
            DoVersionToleranceTest("SerializedOutput.Xaml");
            DoVersionToleranceTest("SerializedOutput.XamlPackage");

            if (_testPassed)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                Logger.Current.ReportResult(false, "One or more serialized outputs didnt match");
            }
        }        

        private void DoVersionToleranceTest(string fileName)
        {
            LoadRichTextBox(fileName);
            DispatcherHelper.DoEvents(5000); // Extra wait for the images to complete render         
            
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            if (fileName.EndsWith(DataFormats.Xaml))
            {
                CompareSerializedOutput(tr, DataFormats.Xaml);
            }
            else
            {
                CompareSerializedOutput(tr, DataFormats.XamlPackage);
            }            
        }

        private void CompareSerializedOutput(TextRange tr, string dataFormat)
        {
            MemoryStream mStream = new MemoryStream();
            tr.Save(mStream, dataFormat);
            mStream.Seek(0, SeekOrigin.Begin);            
            Logger.Current.TestLog.LogFile("SerializedOutput40." + dataFormat, mStream);
            
            if (dataFormat == DataFormats.Xaml)
            {
                // Get 40 xaml output from the memory stream
                mStream.Seek(0, SeekOrigin.Begin);
                StreamReader sReader = new StreamReader(mStream);
                string serializedOutput40 = sReader.ReadToEnd();
                sReader.Close();

                if (serializedOutput40 != _serializedXamlOutput35)
                {
                    Log("Serialized outputs did NOT match - Format: " + DataFormats.Xaml);
                    _testPassed = false;
                }
                else
                {
                    Log("Serialization outputs match - Format: " + DataFormats.Xaml);
                }
            }
            else
            {
                // Get 40 package from the memory stream
                mStream.Seek(0, SeekOrigin.Begin);
                ZipPackage package40 = (ZipPackage)ZipPackage.Open(mStream);

                // Get 35 package from the input file
                ZipPackage package35 = (ZipPackage)ZipPackage.Open("SerializedOutput." + dataFormat);

                ComparePackages(package35, package40);                
            }

            mStream.Close();                       
        }

        // Simplification: We only compare the Xaml part of the package. Image is not compared.
        private void ComparePackages(ZipPackage package35, ZipPackage package40)
        {
            PackagePart packagePart35 = GetXamlPackagePart(package35);
            PackagePart packagePart40 = GetXamlPackagePart(package40);

            StreamReader sReader = null;
            sReader = new StreamReader(packagePart35.GetStream());
            string packagePartContent35 = sReader.ReadToEnd();
            sReader.Close();
            sReader = new StreamReader(packagePart40.GetStream());
            string packagePartContent40 = sReader.ReadToEnd();
            sReader.Close();

            if (packagePartContent35 != packagePartContent40)
            {
                Log("Serialized outputs did NOT match - Format: " + DataFormats.XamlPackage);
                _testPassed = false;
            }
            else
            {
                Log("Serialized outputs match - Format: " + DataFormats.XamlPackage);
            }
        }
        
        private PackagePart GetXamlPackagePart(ZipPackage zipPackage)
        {
            PackagePart wpfEntryPart = null;

            PackageRelationshipCollection relationshipCollection = zipPackage.GetRelationshipsByType(XamlRelationshipFromPackageToEntryPart);
            PackageRelationship entryPartRelationship = null;
            foreach (PackageRelationship packageRelationship in relationshipCollection)
            {
                entryPartRelationship = packageRelationship;
                break;
            }

            if (entryPartRelationship != null)
            {
                // Get entry part uri
                Uri entryPartUri = entryPartRelationship.TargetUri;
                // Get the enrty part
                wpfEntryPart = zipPackage.GetPart(entryPartUri);
            }

            return wpfEntryPart;
        }

        // Load serialized output from 35sp1 TextRange serialization
        private void LoadRichTextBox(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new ApplicationException("Required file not found: " + fileName);
            }

            _rtb = new RichTextBox();
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
                if (fileName.EndsWith(DataFormats.Xaml))
                {    
                    tr.Load(fileStream, DataFormats.Xaml);

                    // Get the 35 xaml input into a string - used for verification later
                    fileStream.Seek(0, SeekOrigin.Begin);
                    StreamReader sReader = new StreamReader(fileStream);
                    _serializedXamlOutput35 = sReader.ReadToEnd();
                    sReader.Close();
                }
                else
                {
                    tr.Load(fileStream, DataFormats.XamlPackage);                                     
                }                
            }

            // Important: Need to hard code few properties on RTB to avoid false test failures dues to localized runs.
            // These exact properties are also set in the GenerateSerializedOutput test which is used to generate serialized output for 3.x version
            // Set FontFamily, FontSize and Language
            _rtb.FontSize = 11;
            _rtb.FontFamily = new System.Windows.Media.FontFamily("Palatino Linotype");
            _rtb.Language = XmlLanguage.GetLanguage(new System.Globalization.CultureInfo("en-US").Name);

            MainWindow.Content = _rtb;
        }

        private RichTextBox _rtb = null;
        private bool _testPassed = true;
        private const string XamlRelationshipFromPackageToEntryPart = "http://schemas.microsoft.com/wpf/2005/10/xaml/entry";
        private string _serializedXamlOutput35 = string.Empty;        
    }
}