// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Helper methods for AnnotationStore API tests.


using System;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Reflection;
//using MS.Internal;
using System.Threading; 
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;


namespace Avalon.Test.Annotations
{
	public class AnnotationStoreTestHelpers
	{
		/// <summary>
		/// Create and populate and annotation for use in testing
		/// </summary>
		/// <param name="tempannotationStore"></param>
		/// <returns></returns>
		public static Annotation MakeAnnotation1()
		{
			Annotation an1 = new Annotation(new XmlQualifiedName("Stodgy", "foo"));

			XmlDocument xdoc = new XmlDocument();
			an1.Authors.Add("<stringauthor>Mr Stodgy</stringauthor>");

			// Make a context and add it
			AnnotationResource cont = new AnnotationResource();
			ContentLocator loc = new ContentLocator();
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("MyLocatorPart", ValidNamespaces[0]));

			part.NameValuePairs.Add("Value", "LocatorInner Xml part");
			loc.Parts.Add(part);
			cont.ContentLocators.Add(loc);
			an1.Anchors.Add(cont);

			// Make a cargo and add it
			AnnotationResource res = new AnnotationResource();
            XmlElement CargoNode = xdoc.CreateElement("MyCargo", ValidNamespaces[1]);
			CargoNode.InnerXml = "<partone>contents of the cargo first piece</partone><partdeux> and here is another bit of it</partdeux>";
			res.Contents.Add(CargoNode);
			an1.Cargos.Add(res);

			return an1;
		}

		/// <summary>
		/// Create and populate and annotation for use in testing
		/// </summary>
		/// <param name="tempannotationStore"></param>
		/// <returns></returns>
		public static Annotation MakeAnnotation2()
		{
			Annotation ann = new Annotation(new XmlQualifiedName("Secondo", "foo"));

			XmlDocument xdoc = new XmlDocument();
			ann.Authors.Add("I wrote this");

			// Make a context and add it
			AnnotationResource cont = new AnnotationResource();
			ContentLocator loc = new ContentLocator();
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("TheirLocatorPart", ValidNamespaces[0]));

			part.NameValuePairs.Add("Value", "<Another> Inner Xml part</Another>");
			loc.Parts.Add(part);
			cont.ContentLocators.Add(loc);
			ann.Anchors.Add(cont);

			// Make a cargo and add it
			AnnotationResource res = new AnnotationResource();
            XmlElement CargoNode = xdoc.CreateElement("MyCargo", ValidNamespaces[1]);
			CargoNode.InnerXml = "Contents of the cargo <partone/><partdeux> and here is another bit of it</partdeux>";
			res.Contents.Add(CargoNode);
			ann.Cargos.Add(res);
			return ann;
		}

		public static AnnotationResource createAnchor(string type, string nameSpace, int nKeyValuePairs)
		{
			AnnotationResource cont = new AnnotationResource();
			ContentLocator loc = new ContentLocator();
			ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName(type, nameSpace));

			for (int i = 0; i < nKeyValuePairs; i++)
			{
				part.NameValuePairs.Add("Value" + i, "part" + i);
			}
			loc.Parts.Add(part);
			cont.ContentLocators.Add(loc);
			return cont;
		}

		public static AnnotationResource createCargo(string type, string nameSpace, string content)
		{
			AnnotationResource res = new AnnotationResource();
			XmlDocument xdoc = new XmlDocument();
			XmlElement CargoNode = xdoc.CreateElement(type, nameSpace);
			CargoNode.InnerXml = "<content>" + content + "</content>";
			res.Contents.Add(CargoNode);
			return res;
		}

        public static string[] ValidNamespaces = new string[] { "http://annotations/Tests/Storage/2005/fake", "NotARealNamespace" };
	}
}

