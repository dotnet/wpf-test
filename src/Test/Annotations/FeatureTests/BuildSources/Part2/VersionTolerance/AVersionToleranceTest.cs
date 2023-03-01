// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

namespace Microsoft.Test.Annotations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Annotations;
    using System.Windows.Annotations.Storage;
    using System.Windows.Controls;
    using System.Windows.Xps.Packaging;
    using Microsoft.Test.Logging;
    using Microsoft.Test.TestTypes;
    using Microsoft.Test.Threading;

    public abstract class AVersionToleranceTest : StepsTest
    {
        public AVersionToleranceTest(string masterxml)
        {
            this.InitializeSteps += delegate
            {
                MasterXmlFile = masterxml;
                return TestResult.Pass;
            };
            this.InitializeSteps += InitializeTest;
            this.RunSteps += LoadAnnotationStore;
            this.RunSteps += CloseAnnotationStore;
            this.RunSteps += CompareStores;
        }
        
        public DocumentViewer Viewer { get; set; }
        public AnnotationStore Store { get; set; }
        public AnnotationService Service { get; set; }
        public FileStream Stream { get; set; }
        public Window Window { get; set; }

        /// <summary>
        /// Initialize Test.
        /// - Create Window.
        /// - Create DocumentViewer as content for Window
        /// - Load XPS document into DocumentViewer
        /// </summary>
        public TestResult InitializeTest()
        {
            CopyMasterXml();

            XpsDocument document = new XpsDocument(XpsFile, FileAccess.Read);
            IsNotNull(document, string.Format("Could not create XpsDocument('{0}')", XpsFile));

            Viewer = new DocumentViewer();
            Viewer.Document = document.GetFixedDocumentSequence();
            IsNotNull(Viewer,"Could not create DocumentViewer");

            Window = new Window { Content = Viewer, Top = 0, Left = 0, Height = 550, Width = 750 };
            Window.Show();

            DispatcherHelper.DoEvents();

            return TestResult.Pass;
        }

        /// <summary>
        /// Load AnnotationStore from XML file into DocumentViewer's AnnotationService
        /// </summary>
        public TestResult LoadAnnotationStore()
        {
            Stream = new FileStream(XmlFile, FileMode.OpenOrCreate);
            Store = new XmlStreamStore(Stream);

            IsNotNull(Store, "AnnotationStore is NULL");

            Service = new AnnotationService(Viewer);
            Service.Enable(Store);
            
            IsNotNull(Service,"AnnotationService is NULL");

            IList<Annotation> serviceAnnotations = Service.Store.GetAnnotations();
            IList<Annotation> storeAnnotations = Store.GetAnnotations();

            IsTrue(serviceAnnotations.Count == storeAnnotations.Count, "AnnotationStore and ANnotationService have different count on loaded Annotations");

            DispatcherHelper.DoEvents();

            return TestResult.Pass;
        }

        /// <summary>
        /// Close AnnotationStore and AnnotationService
        /// This serializes the current store back to xml file on disk
        /// </summary>
        /// <returns></returns>
        public TestResult CloseAnnotationStore()
        {
            IsNotNull(Store, "AnnotationStore is NULL");
            
            Store.Flush();
            Stream.Flush();
            Stream.Close();
            Store = null;

            IsNotNull(Service,"AnnotationService is NULL");
            
            if (Service.IsEnabled)
            {
                Service.Disable();
                DispatcherHelper.DoEvents();

                IsTrue(!Service.IsEnabled, "AnnotationService should be disabled");
                
                Service = null;
            }
            
            IsTrue(Store == null, "AnnotationStore should be null");
            IsTrue(Service == null, "AnnotationService should be null");

            DispatcherHelper.DoEvents();

            return TestResult.Pass;
        }

        /// <summary>
        /// Compare the loaded/saved XML to the master XML.
        /// Currently this only compares the xml as strings
        /// </summary>
        /// <returns></returns>
        public TestResult CompareStores()
        {
            IsTrue(File.Exists(XmlFile), string.Format("File '{0}' does not exist", XmlFile));
            IsTrue(File.Exists(MasterXmlFile), string.Format("File '{0}' does not exist", MasterXmlFile));

            string a = File.ReadAllText(XmlFile);
            string b = File.ReadAllText(MasterXmlFile);

            IsTrue(String.Equals(a, b, StringComparison.OrdinalIgnoreCase), "Xml files are not identical");

            DispatcherHelper.DoEvents();

            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure object is not null before continuing
        /// </summary>
        public void IsNotNull(object o, string message)
        {
            if (o is string)
            {
                if (string.IsNullOrEmpty(o as string))
                {
                    FailTest(message);
                }
            }
            else if (o == null)
            {
                FailTest(message);
            }
        }

        /// <summary>
        /// Ensure statement is true before continuing
        /// </summary>
        public void IsTrue(bool statement, string message) 
        {
            if (!statement)
            {
                FailTest(message);
            }
        }

        private void FailTest(string message)
        {
            throw new Exception(string.Format("TEST FAILED : {0}", message));
        }

        private void CopyMasterXml()
        {
            IsNotNull(MasterXmlFile, "MasterXmlFile property was not set in test.");

            XmlFile = MasterXmlFile.Replace("Master_", string.Empty);

            if (File.Exists(MasterXmlFile))
            {
                File.Copy(MasterXmlFile, XmlFile, true);
            }

            IsTrue(File.Exists(XmlFile), string.Format("File '{0}' does not exist", XmlFile));
        }

        private string XpsFile { get { return "word.xps"; } }
        private string MasterXmlFile { get; set; }
        private string XmlFile { get; set; }
    }
}

