// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Collections.Generic;
using System.Text;
using Avalon.Test.Annotations;
using System.Windows.Annotations;
using System.Windows.Controls;


using Proxy_AnnotationPublicNamespace = Proxies.System.Windows.Annotations;
using Proxy_AnnotationInternalNamespace = Proxies.MS.Internal.Annotations;
using System.Xml;
using System.Collections.ObjectModel;
using Annotations.Test.Framework;
using System.Collections;
using Microsoft.Test.Logging;
using System.Windows.Annotations.Storage;
using System.IO;
using System.Windows.Media;
using System.Windows.Documents;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Xml.Schema;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Test;
using Microsoft.Test.Annotations;
using Microsoft.Test.Discovery;

namespace Avalon.Test.Annotations
{
    public static class IAnchorInfoHelper
    {
        #region Test Helpers

        [TestCase_Helper()]
        public static XmlReader LoadSingleAnnotation(string filename)
        {
            Stream resourceStream = null;
            XmlReader reader = null;
            AnnotationsTestSettings settings = new AnnotationsTestSettings();

            Assembly targetAssembly = Assembly.Load(settings.TargetAssemblyName);
            ResourceManager resourceManager = new ResourceManager(settings.TargetAssemblyName + ".g", targetAssembly);
            resourceManager.IgnoreCase = true;
            resourceStream = (Stream)resourceManager.GetObject(filename);

            if (resourceStream != null)
            {
                reader = XmlReader.Create(resourceStream);
            }

            return reader;
        }

        [TestCase_Helper()]
        public static void ImportAnnotationsStore(string filename, AnnotationService service)
        {
            Stream resourceStream = null;

            AnnotationsTestSettings settings = new AnnotationsTestSettings();
            Assembly targetAssembly = Assembly.Load(settings.TargetAssemblyName);
            ResourceManager resourceManager = new ResourceManager(settings.TargetAssemblyName + ".g", targetAssembly);
            resourceManager.IgnoreCase = true;
            resourceStream = (Stream)resourceManager.GetObject(filename);

            if (resourceStream != null)
            {
                //   xaml = XamlReader.Load(resourceStream);
                AnnotationStore tmpStore = new XmlStreamStore(resourceStream);
                IList<Annotation> toImport = tmpStore.GetAnnotations();
                foreach (Annotation annot in toImport)
                {
                    service.Store.AddAnnotation(annot);
                }
            }

            DispatcherHelper.DoEvents();
        }

        //[TestCase_Helper()]
        //public static Annotation MakeNewAnnotation()
        //{
        //    //Create Annotation.
        //    XmlQualifiedName type = new XmlQualifiedName("someType", "someNsp");
        //    Guid id = Guid.NewGuid();
        //    DateTime createTime = DateTime.Now;
        //    DateTime modifiedTime = DateTime.MaxValue;

        //    Annotation annotation = new Annotation(type, id, createTime, modifiedTime);
        //    annotation.Authors.Add("otis");

        //    AnnotationResource anchor = new AnnotationResource();

        //    ContentLocator loc = new ContentLocator();
        //    ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("OOOO", "XXXX"));
        //    part.NameValuePairs.Add("Value", "<Another> Inner Xml part</Another>");
        //    loc.Parts.Add(part);

        //    anchor.ContentLocators.Add(loc);
        //    annotation.Anchors.Add(anchor);

        //    AnnotationResource cargo = new AnnotationResource();
        //    cargo.Contents.Add(AnnotationTestHelper.CreateContent("matt", "awesome"));
        //    annotation.Cargos.Add(cargo);

        //    return annotation;
        //}

        [TestCase_Helper]
        public static Annotation CreateNewAnnotation()
        {
            Annotation ann = new Annotation(new XmlQualifiedName("Secondo", "bar"));

            XmlDocument xdoc = new XmlDocument();
            XmlNode AuthsNode = xdoc.CreateNode(XmlNodeType.Element, "WhoWroteThis", "testAuthorNamespace");

            AuthsNode.InnerXml = "I wrote this";
            ann.Authors.Add("I wrote this");

            // Make a context and add it
            AnnotationResource cont = new AnnotationResource();

            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("TheirLocatorPart", "testLocatorPartsNamespace"));
            part.NameValuePairs.Add("Value", "<Another> Inner Xml part</Another>");

            ContentLocator seq = new ContentLocator();
            seq.Parts.Add(part);

            cont.ContentLocators.Add(seq);
            ann.Anchors.Add(cont);

            // Make a cargo and add it
            AnnotationResource res = new AnnotationResource();
            XmlElement CargoNode = (XmlElement)xdoc.CreateNode(XmlNodeType.Element, "MyCargo", "testCargoNamsepace");

            CargoNode.InnerXml = "Contents of the cargo <partone/><partdeux> and here is another bit of it</partdeux>";
            res.Contents.Add(CargoNode);
            ann.Cargos.Add(res);
            return ann;
        }


        /// <summary>
        /// Gets all IAnchorInfo's for all attached StickyNotes.
        /// </summary>
        /// <param name="wrappers"></param>
        /// <returns></returns>
        [TestCase_Helper()]
        public static List<IAnchorInfo> GetIAnchorInfos(StickyNoteWrapper[] wrappers)
        {
            List<IAnchorInfo> anchorinfos = new List<IAnchorInfo>();

            foreach (StickyNoteWrapper wrapper in wrappers)
            {
                IAnchorInfo anchorinfo = wrapper.Target.AnchorInfo;

                TestSuite.Current.printStatus(
                    anchorinfo != null ?
                    string.Format("IAnchorInfo for {0} is found.", wrapper.Type.ToString()) :
                    string.Format("Could not get IAnchorInfo for {0}.", wrapper.Type.ToString()));

                TestSuite.Current.AssertNotNull("IAnchorInfo is null.", anchorinfo);

                anchorinfos.Add(anchorinfo);
            }

            return anchorinfos;
        }

        /// <summary>
        /// Gets all IAnchorInfo's for each Annotation in the store.
        /// </summary>
        /// <param name="annotations"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [TestCase_Helper()]
        public static List<IAnchorInfo> GetIAnchorInfos(IList<Annotation> annotations, AnnotationService service)
        {
            List<IAnchorInfo> anchorinfos = new List<IAnchorInfo>();

            foreach (Annotation annotation in annotations)
            {
                IAnchorInfo anchorinfo = AnnotationHelper.GetAnchorInfo(service, annotation);

                TestSuite.Current.printStatus(
                     anchorinfo != null ?
                     string.Format("IAnchorInfo for {0} is found.", annotation.AnnotationType.Name) :
                     string.Format("Could not get IAnchorInfo for {0}.", annotation.AnnotationType.Name));

                TestSuite.Current.AssertNotNull("IAnchorInfo is null.", anchorinfo);

                anchorinfos.Add(anchorinfo);
            }

            return anchorinfos;
        }

        /// <summary>
        /// Verify that there is one AttachedAnnotation with an AttachedAnchor that meets the given constraints.
        /// </summary>
        [TestCase_Helper()]
        public static void VerifyAnnotationProxies(string expectedAnchor, IAnchorInfo anchorInfo, Proxy_AnnotationPublicNamespace.AnnotationService service)
        {
            List<IAnchorInfo> anchorinfos = new List<IAnchorInfo>();
            anchorinfos.Add(anchorInfo);
            VerifyAnnotationProxies(new string[] { expectedAnchor }, anchorinfos, 1, service);
        }

        /// <summary>
        /// Verify that there is one AttachedAnnotation with an AttachedAnchor that meets the given constraints.
        /// </summary>
        [TestCase_Helper()]
        public static void VerifyAnnotationProxies(string expectedAnchor, List<IAnchorInfo> anchorInfos, Proxy_AnnotationPublicNamespace.AnnotationService service)
        {
            VerifyAnnotationProxies(new string[] { expectedAnchor }, anchorInfos, 1, service);
        }

        /// <summary>
        /// Verify that there is one AttachedAnnotation with an AttachedAnchor that meets the given constraints.
        /// </summary>
        [TestCase_Helper()]
        public static void VerifyAnnotationProxies(string[] expectedAnchors, List<IAnchorInfo> anchorInfos, int expectedNumAttachedAnnotations, Proxy_AnnotationPublicNamespace.AnnotationService service)
        {
            TestSuite.Current.printStatus("Get IAttachedAnnotation through proxies.");

            //Proxy_AnnotationPublicNamespace.AnnotationService service = Service;
            IList<Proxy_AnnotationInternalNamespace.IAttachedAnnotation> attachedAnnotations = service.GetAttachedAnnotations();
            TestSuite.Current.AssertEquals("Verify number of attached annotations.", expectedNumAttachedAnnotations, attachedAnnotations.Count);

            TestSuite.Current.AssertEquals("There should be the same amount of Expected Anchors and IAnchorInfos.", expectedAnchors.Length, anchorInfos.Count);

            // AttachedAnnotations is un-ordered, so search accordingly.
            for (int i = 0; i < expectedNumAttachedAnnotations; i++)
            {
                Proxy_AnnotationInternalNamespace.IAttachedAnnotation current = attachedAnnotations[i];
                CompareIAnchorInfo(anchorInfos[i], current);
            }
        }

        /// <summary>
        /// Compare IAnchorInfo to IAttachedAnnotation
        /// </summary>
        /// <param name="actual">Retrieved from IAnchorInfo public api</param>
        /// <param name="expected">from proxies</param>
        /// <returns></returns>
        [TestCase_Helper()]
        public static void CompareIAnchorInfo(IAnchorInfo actual, Proxy_AnnotationInternalNamespace.IAttachedAnnotation expected)
        {
            TestSuite.Current.printStatus("Compare IAnchorInfo to IAttachedAnnotation.");

            TestSuite.Current.printStatus("Compare Anchor ...");
            CompareAnchors(actual.Anchor, expected.Anchor);

            TestSuite.Current.printStatus("Compare Annotation ...");
            CompareAnnotations(actual.Annotation, expected.Annotation);

            TestSuite.Current.printStatus("Compare ResolvedAnchor ...");
            CompareResolvedAnchor(actual.ResolvedAnchor, expected.AttachedAnchor);
        }

        /// <summary>
        /// Compare Anchors
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        [TestCase_Helper()]
        public static void CompareAnchors(AnnotationResource actual, AnnotationResource expected)
        {
            TestSuite.Current.AssertEquals("ContentLocators count are not equal.", actual.ContentLocators.Count, expected.ContentLocators.Count);
            if (actual.ContentLocators.Count > 0)
                ContentLocatorCompare(actual.ContentLocators, expected.ContentLocators);

            TestSuite.Current.AssertEquals("Anchor content count are not equal.", actual.Contents.Count, expected.Contents.Count);
            if (actual.Contents.Count > 0)
                XmlCompare(actual.Contents, expected.Contents);

            TestSuite.Current.AssertEquals("Id's of anchors are not equal.", actual.Id, expected.Id);

            TestSuite.Current.AssertEquals("Name's of anchors are not equal.", actual.Name, expected.Name);

            TestSuite.Current.AssertEquals("Actual and Expected not equal", actual, expected);
        }

        public static void CompareAnchors(Collection<AnnotationResource> actual, Collection<AnnotationResource> expected)
        {
            if (actual.Count != 0)
            {
                for (int i = 0; i <= actual.Count - 1; i++)
                {
                    CompareAnchors(actual[i], expected[i]);
                }
            }
        }

        /// <summary>
        /// Compare Annotations
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        [TestCase_Helper()]
        public static void CompareAnnotations(Annotation actual, Annotation expected)
        {
            TestSuite.Current.AssertEquals("Anchor count is not equal", actual.Anchors.Count, expected.Anchors.Count);
            if (actual.Anchors.Count > 0)
                CompareAnchors(actual.Anchors, expected.Anchors);

            TestSuite.Current.Assert("AnnotationType is not equal", XmlQualifiedName.Equals(actual.AnnotationType, expected.AnnotationType));
            TestSuite.Current.AssertEquals("AnnotationType.IsEmpty is not equal", actual.AnnotationType.IsEmpty, expected.AnnotationType.IsEmpty);
            TestSuite.Current.AssertEquals("AnnotationType.Name is not equal", actual.AnnotationType.Name, expected.AnnotationType.Name);
            TestSuite.Current.AssertEquals("AnnotationType.Namespace is not equal", actual.AnnotationType.Namespace, expected.AnnotationType.Namespace);

            TestSuite.Current.AssertEquals("Author count is not equal", actual.Authors.Count, expected.Authors.Count);
            TestSuite.Current.AssertEquals("Author collection is not equal", actual.Authors, expected.Authors);

            TestSuite.Current.AssertEquals("Cargo count is not equal", actual.Cargos.Count, expected.Cargos.Count);
            if (actual.Cargos.Count > 0)
                CompareAnchors(actual.Cargos, expected.Cargos);

            TestSuite.Current.AssertEquals("Annotation.Id's are not equal.", actual.Id, expected.Id);

            TestSuite.Current.Assert("Create time is not equal", DateTime.Equals(actual.CreationTime, expected.CreationTime));
            TestSuite.Current.Assert("Last modified time is not equal", DateTime.Equals(actual.LastModificationTime, expected.LastModificationTime));

            TestSuite.Current.AssertEquals("Actual and Expected not equal", actual, expected);
        }

        /// <summary>
        /// Compare ResolvedAnchor
        /// </summary>
        /// <param name="actual"></param>
        [TestCase_Helper()]
        public static void CompareResolvedAnchor(object actual, object expected)
        {
            TestSuite.Current.AssertNotNull("IAnchorInfo.ResolvedAnchor should not be null.", actual);
            TestSuite.Current.Assert("ResolvedAnchor is not of type TextAnchor.", AnnotationTestHelper.IsTextAnchor(actual));

            TestSuite.Current.AssertEquals("ResolvedAnchor types are not equal.", actual, expected);
            TestSuite.Current.AssertEquals("ResolvedAnchor text is not equal.", AnnotationTestHelper.GetText(actual), AnnotationTestHelper.GetText(actual));
            
            TestSuite.Current.AssertEquals("Actual and Expected not equal", actual, expected);
        }

        /// <summary>
        /// Compare XML objects
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        [TestCase_Helper()]
        public static void XmlCompare(Collection<XmlElement> actual, Collection<XmlElement> expected)
        {
            TestSuite.Current.Assert("XmlElement collections do not have equal item count.", actual.Count == expected.Count);

            if (actual.Count != 0)
            {
                for (int i = 0; i <= actual.Count - 1; i++)
                {
                    TestSuite.Current.Assert("XmlElement Compare failed.", XmlCompare(actual[i], expected[i]));
                }
            }
        }

        [TestCase_Helper()]
        public static bool XmlCompare(XmlElement actual, XmlElement expected)
        {
            // Simple comparison of XML should be enough
            if (actual.OuterXml == expected.OuterXml)
            {
                return true;
            }
            else
            {
                TestSuite.Current.printStatus(string.Format("Acutal   : {0}", actual.OuterXml));
                TestSuite.Current.printStatus(string.Format("Expected : {0}", expected.OuterXml));
                return false;
            }
        }

        /// <summary>
        /// Compare ContentLocators objects
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        [TestCase_Helper()]
        public static void ContentLocatorCompare(Collection<ContentLocatorBase> actual, Collection<ContentLocatorBase> expected)
        {
            TestSuite.Current.Assert("ContentLocatorBase collections do not have equal item count.", actual.Count == expected.Count);

            if (actual.Count != 0)
            {
                for (int i = 0; i <= actual.Count - 1; i++)
                {
                    TestSuite.Current.Assert("ContentLocator collection comparison failed.", ContentLocatorCompare(actual[i], expected[i]));
                }
            }
        }

        [TestCase_Helper()]
        public static bool ContentLocatorCompare(ContentLocatorBase actual, ContentLocatorBase expected)
        {
            if (!XmlSchema.Equals(((ContentLocator)actual).GetSchema(), ((ContentLocator)expected).GetSchema()))
            {
                TestSuite.Current.printStatus("ContentLocator GetSchema compare failed.");
                TestSuite.Current.printStatus(string.Format("Acutal   : {0}", ((ContentLocator)actual).GetSchema().ToString()));
                TestSuite.Current.printStatus(string.Format("Expected : {0}", ((ContentLocator)expected).GetSchema().ToString()));
                return false;
            }

            if (((ContentLocator)actual).Parts.Count != ((ContentLocator)expected).Parts.Count)
            {
                TestSuite.Current.printStatus("ContentLocator Parts.Count compare failed.");
                TestSuite.Current.printStatus(string.Format("Acutal   : {0}", ((ContentLocator)actual).Parts.Count.ToString()));
                TestSuite.Current.printStatus(string.Format("Expected : {0}", ((ContentLocator)expected).Parts.Count.ToString()));
                return false;
            }

            if (!ContentLocatorPartsCompare(((ContentLocator)actual).Parts, ((ContentLocator)expected).Parts))
                return false;

            TestSuite.Current.AssertEquals("Actual and Expected not equal", actual, expected);

            return true;
        }

        /// <summary>
        /// Compare ContentLocators objects
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        [TestCase_Helper()]
        public static bool ContentLocatorPartsCompare(Collection<ContentLocatorPart> actual, Collection<ContentLocatorPart> expected)
        {
            TestSuite.Current.Assert("ContentLocatorPart collections do not have equal item count.", actual.Count == expected.Count);

            if (actual.Count != 0)
            {
                for (int i = 0; i <= actual.Count - 1; i++)
                {
                    if (!ContentLocatorPartsCompare(actual[i], expected[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [TestCase_Helper()]
        public static bool ContentLocatorPartsCompare(ContentLocatorPart actual, ContentLocatorPart expected)
        {
            if (!IDictionary<string, string>.Equals(actual.NameValuePairs, expected.NameValuePairs))
            {
                TestSuite.Current.printStatus("ContentLocatorPart NameValuePairs are different.");
                return false;
            }

            if (!XmlQualifiedName.Equals(actual.PartType, expected.PartType))
            {
                TestSuite.Current.printStatus("ContentLocatorPart PartTypes are different.");
                return false;
            }

            TestSuite.Current.AssertEquals("Actual and Expected not equal", actual, expected);

            return true;
        }

        #endregion
    }
}

