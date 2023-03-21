// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Annotations.Storage;
using System.Collections.Generic;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.IO;
using System.Text;

namespace Avalon.Test.Annotations.Suites
{
    public class XmlStreamStoreSuite : AXmlStreamStoreSuite
    {
        #region BVT TESTS

        #region Ctor

        /// <summary>
        /// Compatibility	    Xml	    Contains	        Result
        /// 		                    V1 elements only	IgnoredNamespaces=[0]
        /// </summary>
        [Priority(0)]
        protected void ctor1()
        {
            XmlStreamStore store = new XmlStreamStore(CreateStreamFromTemplate("", ""));
            AssertEquals("Verify num annotations.", 2, store.GetAnnotations().Count);
            VerifyIgnoredNamespaces(store, new Uri[0]);
            passTest("V1 elements only.");
        }

        /// <summary>
        /// Compatibility	    Xml	    Contains	        Result
        /// <NS1, [V1]>			                            Exception.
        /// </summary>
        [Priority(0)]
        protected void ctor2()
        {
            IDictionary<Uri, IList<Uri>> namespaces = CreateNamespaceDictionary(UnknownNamespace1, new Uri[] { DefaultKnownNamespaces[2] });
            VerifyRegisteringNamespacesFails(namespaces);
            passTest("Exception adding default NS compatibility.");
        }

        /// <summary>
        /// Compatibility	    Xml	    Contains	        Result
        /// <V1, [NS1]>     	                            Exception.
        /// </summary>
        [Priority(0)]
        protected void ctor3()
        {
            IDictionary<Uri, IList<Uri>> namespaces = CreateNamespaceDictionary(DefaultKnownNamespaces[2], new Uri[] { UnknownNamespace1 });
            VerifyRegisteringNamespacesFails(namespaces);
            passTest("Exception adding default NS compatibility.");
        }

        /// <summary>
        /// Compatibility	    Xml	                Contains    Result
        ///                     Ignorable=”NS1”	    NS1	        IgnoredNamespaces=NS1
        /// </summary>
        [Priority(0)]
        protected void ctor7()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS1=\"" + UnknownNamespace1 + "\" x:Ignorable=\"NS1\"",
                "NS1:Imaginary=\"blah\"");
            XmlStreamStore store = new XmlStreamStore(stream);
            VerifyIgnoredNamespaces(store, new Uri[] { UnknownNamespace1 });
            passTest("Ignorable namespace is ignored.");
        }

        /// <summary>
        /// Compatibility	    Xml	                    Contains    Result
        ///                     Ignorable=”NS2, NS3”	NS2,NS3	    IgnoredNamespaces=NS2,NS3
        /// </summary>
        [Priority(0)]
        protected void ctor8()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS2=\"" + UnknownNamespace2 + "\" " +
                "xmlns:NS3=\"" + UnknownNamespace3 + "\" " +
                "x:Ignorable=\"NS2 NS3\"",
                "NS2:Imaginary=\"blah\" " +
                "NS3:BadAttribute=\"Hello World\"",
                "",
                "<NS2:Author>Future</NS2:Author>");
            XmlStreamStore store = new XmlStreamStore(stream);
            VerifyIgnoredNamespaces(store, new Uri[] { UnknownNamespace2, UnknownNamespace3 });
            passTest("Multiple ignorable namespaces ignored.");
        }

        /// <summary>
        /// Compatibility	    Xml	                Contains        Result
        ///                     	                NS2 attribute	Exception.
        /// </summary>
        
        [DisabledTestCase()]
        [Priority(0)]
        protected void ctor9_1()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS1=\"" + UnknownNamespace2 + "\"",
                "NS1:Imaginary=\"blah\"");
            VerifyExceptionParsingStream(stream);
            passTest("Exception thrown for unknown MustHave namespace attribute.");
        }

        /// <summary>
        /// Compatibility	    Xml	                Contains        Result
        ///                     	                NS2 cargo	    Exception.
        /// </summary>
        
        [DisabledTestCase()]
        [Priority(0)]
        protected void ctor9_2()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS1=\"" + UnknownNamespace2 + "\"",
                "",
                "<NS1:Imaginary>blah</NS1:Imaginary>",
                "");
            VerifyExceptionParsingStream(stream);
            passTest("Exception thrown for unknown MustHave namespace in cargo.");
        }

        /// <summary>
        /// Compatibility	    Xml	                Contains        Result
        ///                     	                NS2 element    	Exception.
        /// </summary>
        
        [DisabledTestCase()]
        [Priority(0)]
        protected void ctor9_3()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS1=\"" + UnknownNamespace2 + "\"",
                "",
                "",
                "<NS1:Imaginary>blah</NS1:Imaginary>");
            VerifyExceptionParsingStream(stream);
            passTest("Exception thrown for unknown MustHave namespace element.");
        }

        /// <summary>
        /// Compatibility	    Xml	                Contains    Result
        /// <NS1, null>	        MustHave=”NS1”	    NS1 	    IgnoredNamespaces=[0]
        /// </summary>
        [Priority(0)]
        protected void ctor11()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS1=\"" + UnknownNamespace1 + "\"",
                "",
                "<NS1:Custom>somestuff</NS1:Custom>",
                "");
            XmlStreamStore store = new XmlStreamStore(stream, CreateNamespaceDictionary(UnknownNamespace1));
            VerifyIgnoredNamespaces(store, new Uri[0]);
            passTest("Known namespace is not ignored.");
        }

        /// <summary>
        /// Compatibility	    Xml     Contains    Result
        /// <NS1, [NS2]>		        NS1, NS2    IgnoredNamespaces=[0]
        /// </summary>
        [Priority(0)]
        protected void ctor13()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS1=\"" + UnknownNamespace1 + "\" " +
                "xmlns:NS2=\"" + UnknownNamespace2 + "\"",
                "",
                "<NS1:Custom>somestuff</NS1:Custom><NS2:OtherCustom>foo</NS2:OtherCustom>",
                "");
            XmlStreamStore store = new XmlStreamStore(stream, CreateNamespaceDictionary(UnknownNamespace1, new Uri[] { UnknownNamespace2 }));
            VerifyIgnoredNamespaces(store, new Uri[0]);
            AnnotationResource cargo = store.GetAnnotations()[0].Cargos[1];
            AssertEquals("Verify number of contents.", 3, cargo.Contents.Count);
            AssertEquals("Verify NS2 namespace has been converted to NS1.", UnknownNamespace1.ToString(), cargo.Contents[2].NamespaceURI);
            passTest("Verified Subsuming.");
        }

        /// <summary>
        /// Compatibility	    Xml     Contains                       Result
        ///                             NS1 is defined but NOT used    IgnoredNamespaces=[0]
        /// </summary>
        [Priority(0)]
        protected void ctor19()
        {
            Stream stream = CreateStreamFromTemplate("xmlns:NS1=\"" + UnknownNamespace1 + "\" ", "");
            XmlStreamStore store = new XmlStreamStore(stream);
            VerifyIgnoredNamespaces(store, new Uri[0]);
            passTest("Defining a namespace does not cause it to be listed as Ignored.");
        }

                #endregion

        #region AlternateContent

        /// <summary>
        /// Compatibility	    Xml     Contains                            Result
        ///                             <AlternateContent>
        ///                                <Choice Requires=”NS1”/>
        ///                                <Fallback/>
        ///                             </AlternateContent>	                IgnoredNamespaces=NS1
        /// </summary>
        [Priority(0)]
        protected void alternatecontent1()
        {
            Stream stream = CreateStreamFromTemplate(
               "xmlns:NS1=\"" + UnknownNamespace1 + "\" ",
               "",
               "",
               AlternateAuthorXml(new string[] { "NS1" }, new string[] { "NS1" }, "Fallback"));
            XmlStreamStore store = new XmlStreamStore(stream);
            VerifyIgnoredNamespaces(store, new Uri[] { UnknownNamespace1 });
            AssertEquals("Verify author.", "Fallback", store.GetAnnotations()[0].Authors[0]);
            passTest("Namespace ignored in AlternateContent block.");
        }


        /// <summary>
        /// Compatibility	    Xml     Contains                            Result
        ///                             <AlternateContent>
        ///                               <Choice Requires=”NS2”/>
        ///                               <Choice Requires=”NS1”/>
        ///                               <Choice Requires=”V1”/>
        ///                               <Fallback/>
        ///                             </AlternateContent>	                IgnoredNamespaces=NS2, NS1
        /// </summary>
        [Priority(0)]
        protected void alternatecontent2()
        {
            Stream stream = CreateStreamFromTemplate(
               "xmlns:NS1=\"" + UnknownNamespace1 + "\" " +
               "xmlns:NS2=\"" + UnknownNamespace2 + "\"",
               "",
               "",
               AlternateAuthorXml(new string[] { "NS2", "NS1", "anc" }, new string[] { "ns2", "ns1", "pass" }, "fallback"));
            XmlStreamStore store = new XmlStreamStore(stream);
            VerifyIgnoredNamespaces(store, new Uri[] { UnknownNamespace2, UnknownNamespace1 });
            AssertEquals("Verify author.", "pass", store.GetAnnotations()[0].Authors[0]);
            passTest("Namespace ignored multiple namespaces in AlternateContent block.");
        }

        /// <summary>
        /// Compatibility	    Xml     Contains                            Result
        ///                             <AlternateContent>
        ///                                 <Choice Requires=”NS1”/>
        ///                             </AlternateContent>	                Exception, no Fallback.
        /// </summary>
        [Priority(0)]
        protected void alternatecontent4()
        {
            Stream stream = CreateStreamFromTemplate(
               "xmlns:NS1=\"" + UnknownNamespace1 + "\" ",
               "",
               "",
               AlternateAuthorXml(new string[] { "NS1" }, new string[] { "author" }, null));
            XmlStreamStore store = new XmlStreamStore(stream);
            VerifyIgnoredNamespaces(store, new Uri[] { UnknownNamespace1 });
            AssertEquals("Verify no author.", 0, store.GetAnnotations()[0].Authors.Count);
            passTest("No content loaded if no matching choice");
        }

        #endregion

        #region KnownNamespaces

        [Priority(0)]
        protected void knownnamespaces()
        {
            IList<Uri> knownNamespaces = XmlStreamStore.WellKnownNamespaces;
            AssertEquals("Verify number of known namespaces.", DefaultKnownNamespaces.Length, knownNamespaces.Count);
            foreach (Uri expectedUri in DefaultKnownNamespaces)
            {
                if (!knownNamespaces.Contains(expectedUri))
                {
                    failTest("Expected namespace '" + expectedUri + "' is not in the list of Known Namespaces.");
                }
            }
            passTest("Verified WellKnownNamespaces.");
        }

        #endregion

        #region CompatibleNamespaces

        /// <summary>
        /// Call with each namespace from KnownNamespaces.	No compatible namespaces in V1.
        /// </summary>
        [Priority(0)]
        protected void compatiblenamespaces1()
        {
            foreach (Uri ns in DefaultKnownNamespaces)
            {
                AssertNull("Verify no compatible namespaces for '" + ns + "'.", XmlStreamStore.GetWellKnownCompatibleNamespaces(ns));
            }
            passTest("Verified compatible Namespaces.");
        }

        #endregion
  
        #endregion BVT TESTS

        #region PRIORITY TESTS

        #region Ctor

        /// <summary>
        /// Compatibility	    Xml	                Contains    Result
        /// null                                                IgnoredNamespaces=[0]
        /// </summary>
        [Priority(1)]
        protected void ctor4()
        {
            XmlStreamStore store = new XmlStreamStore(CreateStreamFromTemplate("", "", "", ""), null);
            VerifyIgnoredNamespaces(store, new Uri[0]);
            passTest("Verify handling null namespace dictionary.");
        }

        /// <summary>
        /// Compatibility	    Xml	                Contains    Result
        /// <null, [NS1]>                                       Exception
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        protected void ctor5()
        {
            VerifyRegisteringNamespacesFails(CreateNamespaceDictionary(null, new Uri[] { UnknownNamespace1 }));
            passTest("Exception for null dictionary key.");
        }

        /// <summary>
        /// Compatibility	    Xml	                Contains    Result
        /// Empty Dictionary                                    IgnoredNamespaces=[0]
        /// </summary>
        [Priority(1)]
        protected void ctor6()
        {
            XmlStreamStore store = new XmlStreamStore(CreateStreamFromTemplate("", ""), new Dictionary<Uri, IList<Uri>>());
            VerifyIgnoredNamespaces(store, new Uri[0]);
            passTest("No exception for empty dictionary.");
        }

        /// <summary>
        /// Compatibility	    Xml	                Contains          Result
        /// <NS1, null>	        Ignorable=”NS1”	    NS1	              IgnoredNamespaces=[0]
        /// </summary>
        [Priority(1)]
        protected void ctor12()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS1=\"" + UnknownNamespace1 + "\" " +
                "x:Ignorable=\"NS1\"",
                "",
                "<NS1:Cargo>Foo</NS1:Cargo>",
                "");
            XmlStreamStore store = new XmlStreamStore(stream, CreateNamespaceDictionary(UnknownNamespace1, null));
            VerifyIgnoredNamespaces(store, new Uri[0]);
            passTest("Known Namespace not ignored.");
        }

        /// <summary>
        /// Compatibility	    Xml         Contains          Result
        /// <NS1, [NS2]>
        /// <NS1, [NS3]>			                          Exception.
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        protected void ctor14()
        {
            VerifyRegisteringNamespacesFails(CreateNamespaceDictionary(
                new Uri[] { UnknownNamespace1, UnknownNamespace1 },
                new Uri[][] { new Uri[] { UnknownNamespace2 }, new Uri[] { UnknownNamespace3 } }
                ));
            passTest("Exception registering same key multiple times.");
        }

        /// <summary>
        /// Compatibility	    Xml         Contains          Result
        /// <NS1, [NS2]>
        /// <NS2, [NS3]>			                          Exception.
        /// </summary>
        [Priority(1)]
        protected void ctor15()
        {
            VerifyRegisteringNamespacesFails(CreateNamespaceDictionary(
                new Uri[] { UnknownNamespace1, UnknownNamespace2 },
                new Uri[][] { new Uri[] { UnknownNamespace2 }, new Uri[] { UnknownNamespace3 } }
                ));
            passTest("Exception registering value as key.");
        }
       
        /// <summary>
        /// Compatibility	    Xml         Contains          Result
        /// <NS2, [NS1]>
        /// <NS3, [NS1]>			                          Exception.
        /// </summary>
        [Priority(1)]
        protected void ctor16()
        {
            VerifyRegisteringNamespacesFails(CreateNamespaceDictionary(
                new Uri[] { UnknownNamespace2, UnknownNamespace3 },
                new Uri[][] { new Uri[] { UnknownNamespace1 }, new Uri[] { UnknownNamespace1 } }
                ));
            passTest("Exception registering same value for multiple keys.");
        }

        /// <summary>
        /// Compatibility	    Xml         Contains            Result
        /// <NS1, [NS2,NS3]>		        NS2, NS3	        IgnoredNamespaces=[0]
        /// </summary>
        [Priority(1)]
        protected void ctor17()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS2=\"" + UnknownNamespace2 + "\" " +
                "xmlns:NS3=\"" + UnknownNamespace3 + "\"",
                "",
                "<NS2:Cargo>Foo</NS2:Cargo><NS3:Element>foo</NS3:Element>",
                "");
            XmlStreamStore store = new XmlStreamStore(stream, CreateNamespaceDictionary(UnknownNamespace1, new Uri[] { UnknownNamespace2, UnknownNamespace3 }));
            VerifyIgnoredNamespaces(store, new Uri[0]);
            passTest("Compatible namespaces not ignored.");
        }

        /// <summary>
        /// Compatibility	    Xml         Contains            Result
        /// <NS1, []>		                NS1                 IgnoredNamespaces=[0]
        /// </summary>
        [Priority(1)]
        protected void ctor18()
        {
            Stream stream = CreateStreamFromTemplate(
                "xmlns:NS1=\"" + UnknownNamespace1 + "\" ",
                "",
                "<NS1:Cargo>Foo</NS1:Cargo>",
                "");
            XmlStreamStore store = new XmlStreamStore(stream, CreateNamespaceDictionary(UnknownNamespace1, new Uri[0]));
            VerifyIgnoredNamespaces(store, new Uri[0]);
            passTest("Empty list is valid compatible namespace key");
        }

        /// <summary>
        /// Compatibility	    Xml         Contains                Result
        ///                                 Badly formated Uri      Meaningful exception.
        /// </summary>
        
        [DisabledTestCase()]
        [Priority(1)]
        protected void ctor20()
        {
            Stream stream = CreateStreamFromTemplate("xmlns:NS=\"bad Namespace\"", "");
            try
            {
                new XmlStreamStore(stream);
                failTest("Expected exception for badly formated NameSpace.");
            }
            catch (ArgumentException e)
            {
                Assert("Verify exception message.", e.Message.Contains("The namespace bad Namespace is not a valid Uri"));
            }
            passTest("Exception for invalid namespace.");
        }

        #endregion

        #region AlternateContent

        /// <summary>
        /// Compatibility	    Xml     Contains                            Result
        /// <NS1, [NS2]>                Authors Contains:                   IgnoredNamespaces=[0].
        ///                             <AlternateContent>
        ///                                 <Choice Requires=”NS1”/>
        ///                                 <Choice Requires=”NS2”/>
        ///                             </AlternateContent>	
        /// </summary>
        [Priority(1)]
        protected void alternatecontent3_1()
        {
            Stream stream = CreateStreamFromTemplate(
               "xmlns:NS1=\"" + UnknownNamespace1 + "\" " +
               "xmlns:NS2=\"" + UnknownNamespace2 + "\"",
               "",
               "",
               AlternateAuthorXml(new string[] { "NS1", "NS2" }, new string[] { "choice1", "choice2"}, null));
            XmlStreamStore store = new XmlStreamStore(stream, CreateNamespaceDictionary(UnknownNamespace1, new Uri[] { UnknownNamespace2 }));
            VerifyIgnoredNamespaces(store, new Uri[0]);
            AssertEquals("Verify loaded annotation count.", 2, store.GetAnnotations().Count);
            AssertEquals("Verify correct choice was loaded.", "choice1", store.GetAnnotations()[0].Authors[0]);
            passTest("Expected Choice selected.");
        }

        /// <summary>
        /// Compatibility	    Xml     Contains                            Result
        /// <NS1, [NS2]>                Cargo Contains:                   IgnoredNamespaces=[0].
        ///                             <AlternateContent>
        ///                                 <Choice Requires=”NS1”/>
        ///                                 <Choice Requires=”NS2”/>
        ///                             </AlternateContent>	
        /// </summary>
        [Priority(1)]
        protected void alternatecontent3_2()
        {
            Stream stream = CreateStreamFromTemplate(
               "xmlns:NS1=\"" + UnknownNamespace1 + "\" " +
               "xmlns:NS2=\"" + UnknownNamespace2 + "\"",
               "",
               "<x:AlternateContent><x:Choice Requires=\"NS1\"><NS1:namespace1/></x:Choice><x:Choice Requires=\"NS2\"><NS2:namespace2/></x:Choice></x:AlternateContent>",
               "");
            XmlStreamStore store = new XmlStreamStore(stream, CreateNamespaceDictionary(UnknownNamespace1, new Uri[] { UnknownNamespace2 }));
            VerifyIgnoredNamespaces(store, new Uri[0]);
            string cargoXml = store.GetAnnotations()[0].Cargos[1].Contents[1].OuterXml;
            Assert("Verify expected content exists.", cargoXml.Contains("<NS1:namespace1"));
            Assert("Verify unexpected content exists.", !cargoXml.Contains("<NS2:namespace2"));
            passTest("Expected Choice selected.");
        }

        /// <summary>
        /// Compatibility	    Xml     Contains                            Result
        ///                             <AlternateContent>                  Exception.
        ///                                 <Choice Requires=”NS1”/>
        ///                                 <FallBack>contains NS2</FallBack>
        ///                             </AlternateContent>	
        /// </summary>
        
        [DisabledTestCase()]
        [Priority(1)]
        protected void alternatecontent5()
        {
            Stream stream = CreateStreamFromTemplate(
               "xmlns:NS1=\"" + UnknownNamespace1 + "\" " +
               "xmlns:NS2=\"" + UnknownNamespace2 + "\"",
               "",
               "<x:AlternateContent><x:Choice Requires=\"NS1\"/><x:Fallback><NS2:Blah/></x:Fallback></x:AlternateContent>",
               "");
            VerifyExceptionParsingStream(stream);
            passTest("Exception for unknown NS in AlternateContent Choice.");
        }

        #endregion

        #region CompatibleNamespaces

        /// <summary>
        /// Call with application defined ns.	Null.
        /// </summary>
        [Priority(1)]
        protected void compatiblenamespaces2()
        {
            AssertNull("Verify no compatible ns for external ns.", XmlStreamStore.GetWellKnownCompatibleNamespaces(UnknownNamespace1));
            passTest("No compatible ns for external ns.");
        }

        /// <summary>
        /// Null.	Exception.
        /// </summary>
        [Priority(1)]
        protected void compatiblenamespaces3()
        {
            bool exception = false;
            try
            {
                XmlStreamStore.GetWellKnownCompatibleNamespaces(null);
            }
            catch (ArgumentNullException)
            {
                exception = true;
            }
            Assert("Verify ArgumentNullException.", exception);
            passTest("Expected exception occured.");
        }

        #endregion

        #endregion PRIORITY TESTS
    }
}

