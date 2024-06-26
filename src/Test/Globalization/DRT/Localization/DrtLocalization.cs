// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Baml localization API Drt
//
//

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Markup.Localizer;


namespace DRT
{
    public class LocalizationTest : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new LocalizationTest();
            return drt.Run(args);
        }

        public LocalizationTest()
        {
            WindowTitle = "Localization API DRT";
            TeamContact = "WPF";
            Contact     = "Microsoft";
            DrtName     = "DrtLocalization";
            Suites      = new DrtTestSuite[]{
                    new BamlLocalizationTestSuite()
                    };
        }        
    }

    public class BamlLocalizationTestSuite : DrtTestSuite
    {
        public BamlLocalizationTestSuite() : base("BamlLocalizer test suite")
        {
            Contact = "Microsoft";
        }
        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] {
                new DrtTest(EnumerateAndGenerateResourcesTest),
                new DrtTest(ErrorNotifyEventTest)                
                };            
        }    
        private void EnumerateAndGenerateResourcesTest()
        {             
            Console.WriteLine("Starting Baml localizable resource enumeration and generation test");

            string bamlFile = Path.Combine(DrtFilesDir, "Basic.baml");
            string commentsFile = Path.Combine(DrtFilesDir, "Basic.loc");            
            
            using (Stream bamlStream = File.OpenRead(bamlFile))            
            {
                using (TextReader comments = new StreamReader(commentsFile))
                {
                    BamlLocalizer manager = new BamlLocalizer(
                        bamlStream,                         
                        new TestLocalizabilityResolver(), 
                        comments
                        );
                        
                    BamlLocalizationDictionary resources = manager.ExtractResources();

                    string resourcesDump = DumpLocalizableResourceToString(resources);
                    CompareExtractedValues(resourcesDump, "basic.txt");

                    // Check against a hard-coded Root element name
                    BamlLocalizableResourceKey ExpectedRoot = new BamlLocalizableResourceKey(
                        "Root",
                        "System.Windows.Controls.DockPanel",
                        "$Content"
                        );

                    DRT.Assert(
                        resources.RootElementKey.Equals(ExpectedRoot), 
                        string.Format(
                            "Root Element is expected to be {0}, but got {1}",
                            ExpectedRoot,
                            resources.RootElementKey
                            )
                        );                                    
                        
                    // provide dummy localization to the resources    
                    foreach (DictionaryEntry entry in resources)
                    {
                        BamlLocalizableResourceKey resourceKey = (BamlLocalizableResourceKey) entry.Key;
                        if (ToLocalize(resourceKey))
                        {
                            BamlLocalizableResource resource = (BamlLocalizableResource) entry.Value;
                            resource.Content = Localize(resource.Content);
                        }
                    }

                    using (Stream target = new MemoryStream())
                    {
                        manager.UpdateBaml(
                            target,
                            resources
                            ); 

                        // Validate localized stream 
                        target.Seek(0, SeekOrigin.Begin);
                        BamlLocalizer targetLocalizer = new BamlLocalizer(
                            target,                         
                            new TestLocalizabilityResolver(), 
                            comments
                            );                        
                        ValidateLocalizedStream(manager, targetLocalizer, target, "Basic");                   
                    }      

                    Console.WriteLine("Succeed.");
                }
            } 
        }        

        private void ErrorNotifyEventTest()
        {
            Console.WriteLine("Starting Baml localizable ErrorNotify event test");
            string bamlFile = Path.Combine(DrtFilesDir, "ErrorNotify.baml");
            string commentsFile = Path.Combine(DrtFilesDir, "ErrorNotify.loc");            

            using (Stream bamlStream = File.OpenRead(bamlFile))            
            {
                using (TextReader comments = new StreamReader(commentsFile))
                {            
                    BamlLocalizer manager = new BamlLocalizer(
                        bamlStream, 
                        null, 
                        comments
                        ); 

                    _errorNotifyEventLogger = new StringWriter();
                    manager.ErrorNotify += new BamlLocalizerErrorNotifyEventHandler(OnBamlLocalizerErrorNotify);

                    BamlLocalizationDictionary resources = manager.ExtractResources();
                    string resourcesDump = DumpLocalizableResourceToString(resources);
                    CompareExtractedValues(resourcesDump, "ErrorNotify.txt");


                    //
                    // Purposefully add incorrect translations
                    //
                    BamlLocalizableResourceKey key = new BamlLocalizableResourceKey(
                        "InCompleteElementPlaceHolder",
                        "System.Windows.Controls.DockPanel",
                        "$Content"
                        );
                    resources[key].Content = resources[key].Content + "#a"; // append an incomplete placeholder.

                    key = new BamlLocalizableResourceKey(
                        "SubstitutionAsPlainText", 
                        "System.Windows.Controls.TextBlock",
                        "$Content"
                        );

                    resources[key].Content = resources[key].Content + "<>"; // Make the content not valid XML

                    key = new BamlLocalizableResourceKey(
                        "DuplicateElement", 
                        "System.Windows.Controls.DockPanel",
                        "$Content"
                        );

                    resources[key].Content = resources[key].Content + resources[key].Content; // Make the content has duplicate elements

                    key = new BamlLocalizableResourceKey(
                        "InvalidUid", 
                        "System.Windows.Controls.DockPanel",
                        "$Content"
                        );

                    resources[key].Content = resources[key].Content + "#Dummy123;"; // Make the content has invalid uid

                    key = new BamlLocalizableResourceKey(
                        "MismatchedElements", 
                        "System.Windows.Controls.DockPanel",
                        "$Content"
                        );

                    resources[key].Content = string.Empty; // Delete child placeholders from the content

                    key = new BamlLocalizableResourceKey(
                        "UnknownFormattingTag", 
                        "System.Windows.Controls.TextBlock",
                        "$Content"
                        );

                    resources[key].Content = resources[key].Content + "<unknown> unknown </unknown>"; // Add unknown formatting tag

                    using (Stream target = new MemoryStream())
                    {
                        manager.UpdateBaml(
                            target,
                            resources
                            ); 

                        // Validate localized stream 
                        target.Seek(0, SeekOrigin.Begin);

                        BamlLocalizer targetLocalizer = new BamlLocalizer(
                            target, 
                            null, 
                            comments
                            );                                                    
                    }  

                    string testWarnings = _errorNotifyEventLogger.ToString();
                    _errorNotifyEventLogger.Close();
                    
                    string masterWarningFiles = Path.Combine(DrtFilesDir, "ExpectedWarnings.txt");
                    string failedWarningFiles = Path.Combine(DrtFilesDir, "FailedWarnings.txt");
                    
                    using (StreamReader master = File.OpenText(masterWarningFiles))
                    {
                        string masterString = master.ReadToEnd();
                        if (masterString != testWarnings)
                        {
                            // dump the test output
                            using (StreamWriter failTestDumpWriter = new StreamWriter(failedWarningFiles))
                            {
                                failTestDumpWriter.Write(testWarnings);                                
                            }       

                            DRT.Fail(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Localization test got unexpected errors, expected errors are in {0}, actually got {1}",
                                    masterWarningFiles,
                                    failedWarningFiles
                                )
                            );                            
                        }
                    }                             
                    
                    Console.WriteLine("Succeed.");                    
                }
            }            
        
        }

        private void OnBamlLocalizerErrorNotify(object sender, BamlLocalizerErrorNotifyEventArgs e)
        {
            DRT.Assert(_errorNotifyEventLogger != null);

            _errorNotifyEventLogger.WriteLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Uid = {0}, ErrorCode = {1}",
                    e.Key.Uid,
                    e.Error
                    )
                );
        }        

        private void CompareExtractedValues(string testDumpString, string masterFileName)
        {
            // Compare the extracted resources with the expected values.
            string masterFile   = Path.Combine(DrtFilesDir, masterFileName);
            string failDumpFile = Path.Combine(DrtFilesDir, "failed." + masterFileName);
           
            using (StreamReader master = File.OpenText(masterFile))
            {             
                string masterString   = master.ReadToEnd();

                if (masterString != testDumpString)
                {
                    // dump the test output
                    using (StreamWriter failTestDumpWriter = new StreamWriter(failDumpFile))
                    {
                        failTestDumpWriter.Write(testDumpString);
                    }

                    // The test failed
                    DRT.Fail(
                        string.Format(
                            "\n{0}\n{1}\n",
                            "Localization Update test failed. Expects different set of localizable resources in Baml",
                            string.Format(
                                "Compare the master file and fail dump by: Windiff.exe {0} {1}", 
                                masterFile, 
                                failDumpFile
                                )
                        )
                    );
                }                
            }                          
        }       


        private void DumpTestBamlStream(Stream stream, string filename)
        {          
            // also dump the baml stream we are testing against
            using (Stream bamlDumpStream = File.OpenWrite(filename))
            {
                stream.Seek(0, SeekOrigin.Begin);
                byte[] bamlContent = new byte[stream.Length];
#pragma warning disable CA2022 // Avoid inexact read
                stream.Read(bamlContent, 0, bamlContent.Length);
#pragma warning restore CA2022

                using (BinaryWriter binaryWriter = new BinaryWriter(bamlDumpStream))
                {
                     binaryWriter.Write(bamlContent);
                }                        
            }
        }

        private string DumpLocalizableResourceToString(BamlLocalizationDictionary resources)
        {
            string content = string.Empty;
            using (StringWriter testDump = new StringWriter())
            {
                foreach(DictionaryEntry entry in resources)
                {
                    BamlLocalizableResourceKey key = (BamlLocalizableResourceKey) entry.Key;
                    BamlLocalizableResource resource = (BamlLocalizableResource) entry.Value;

                    testDump.WriteLine(
                        "Key = {0, -60} Category = {1, -7} Readable = {2, -5} Modifiable = {3, -5} Content = '{4}' Comments = '{5}'",
                        string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", key.Uid, key.ClassName, key.PropertyName),
                        resource.Category,
                        resource.Readable,
                        resource.Modifiable,
                        resource.Content,
                        resource.Comments
                        );                        
                }
                content = testDump.ToString();
            }        
            return content;
        }

        // provide dummy localization
        private static string Localize(string value)
        {
            return value + "Localized";
        }

        // find the resource to localize
        private static bool ToLocalize(BamlLocalizableResourceKey key)
        {
            return key.PropertyName == "$Content";
        }

        private void ValidateLocalizedStream(BamlLocalizer source, BamlLocalizer target, Stream targetStream, string testName)
        {            

            BamlLocalizationDictionary newResources = target.ExtractResources();
            BamlLocalizationDictionary oldResources = source.ExtractResources();                                    

            foreach (DictionaryEntry oldEntry in oldResources)
            {
                BamlLocalizableResourceKey resourceKey = (BamlLocalizableResourceKey) oldEntry.Key;
                if (ToLocalize(resourceKey))
                {
                    BamlLocalizableResource oldResource = (BamlLocalizableResource)oldEntry.Value;
                    BamlLocalizableResource newResource = newResources[resourceKey];

                    // check whether the value is localized
                    if (Localize(oldResource.Content) != newResource.Content)
                    {
                        string outputName = "Failed." + Path.ChangeExtension(testName, "baml");
                        outputName = Path.Combine(DrtFilesDir, outputName);

                        DumpTestBamlStream(targetStream, outputName);
                        DRT.Fail(string.Format(
                            CultureInfo.InvariantCulture,
                            "Localized stream doesn't contain the correct localized value for resource '{0}'.\nCheck the localized stream at {1} by BamlDump.exe",                             
                             string.Format(CultureInfo.InvariantCulture, "{0}:{1}.{2}", resourceKey.Uid, resourceKey.ClassName, resourceKey.PropertyName),
                            outputName                            
                            )
                        );
                    }
                }
            }                      
        }

        
        private const string DrtFilesDir = "DrtFiles\\Localization";

        private StringWriter _errorNotifyEventLogger = null; // used to log all the expected events from the test.
        
    }


    /// <summary>
    /// LocalizabilityResolver for the BamlLocalizer
    /// </summary>
    internal class TestLocalizabilityResolver : BamlLocalizabilityResolver
    {
        public override ElementLocalizability GetElementLocalizability(
            string                      assembly,
            string                      className
            )
        {
            if (className == "System.Windows.Controls.Canvas")
            {
                return new ElementLocalizability(null, new LocalizabilityAttribute(LocalizationCategory.Ignore));
            }
            else if (className == "System.Windows.Documents.Underline")
            {
                return new ElementLocalizability("u", new LocalizabilityAttribute(LocalizationCategory.Text));
            }

            return null;
        }

        public override LocalizabilityAttribute GetPropertyLocalizability(
            string assembly, 
            string className, 
            string property
            )
        {
            if (property == "Background")
            {
                return new LocalizabilityAttribute(LocalizationCategory.NeverLocalize);
            }

            return null;
        }        
        

        public override string ResolveFormattingTagToClass(
            string formattingTag
            )
        {
            if (formattingTag == "u") return "System.Windows.Documents.Underline";           
            return null;
        }

        public override string ResolveAssemblyFromClass(
            string className
            )
        {
            return "presentationframework";
        }                   
    }
}
