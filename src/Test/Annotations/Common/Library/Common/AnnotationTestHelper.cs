// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Set of static helper methods that perform recurring
//				 Annotation test operations.

using System;
using System.Collections;			// IList....
using System.Collections.Generic;   // IList<..>...
using System.IO;					// Stream.
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;				// PermissionSet.
using System.Security.Permissions;	// PermissionState.
using System.Windows;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;		// TextRange.
using System.Windows.Interop;
using System.Windows.Markup;	// ParserContext.
using System.Windows.Media;
using System.Xml;
using Annotations.Test.Reflection;
using Microsoft.Test;
using Microsoft.Test.Annotations;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Proxies.MS.Internal.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.System.Windows.Annotations;
using Annotation = System.Windows.Annotations.Annotation;
using AnnotationResource = System.Windows.Annotations.AnnotationResource;
using ContentLocator = System.Windows.Annotations.ContentLocator;
using ContentLocatorBase = System.Windows.Annotations.ContentLocatorBase;
using ContentLocatorGroup = System.Windows.Annotations.ContentLocatorGroup;
using ContentLocatorPart = System.Windows.Annotations.ContentLocatorPart;

namespace Avalon.Test.Annotations
{
	public class AnnotationTestHelper
	{
		/// <summary>
		/// Create an XmlStreamStore on a file with the given name.  If given file is null, create
		/// using a MemoryStream.
		/// If file already exists, load contents.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static AnnotationStore CreateStore(string filename)
		{
            Stream stream;
            if (filename == null)
                stream = new MemoryStream();
            else 
                stream = new FileStream(filename, FileMode.OpenOrCreate);

            return new XmlStreamStore(stream);
        }

        #region Window Helpers

        /// <summary>
        /// Make the given window the front most window.
        /// </summary>
        /// <returns>True if successful.</returns>
        public static bool BringToFront(Window window)
        {
            HandleRef handle = GetHandle(window);
            IntSetForegroundWindow(handle);
            return IntIsWindowVisible(handle);
        }

        /// <summary>
        /// Retrieves the Win32 handle of an arbitrary Avalon Visual.
        /// </summary>
        /// <returns>Handle to a visual.</returns>
        private static HandleRef GetHandle(Visual win)
        {
            //Visual visual = null;
            HandleRef handle = new HandleRef(null, IntPtr.Zero);
            HwndSource source = (HwndSource)PresentationSource.FromVisual(win);
            if (source != null)
            {
                handle = new HandleRef(source, source.Handle);
            }
            return handle;
        }

        /// <summary>
        /// Extern for SetForegroundWindow
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, EntryPoint = "SetForegroundWindow", CharSet = CharSet.Auto)]
        private static extern bool IntSetForegroundWindow(HandleRef hWnd);

        /// <summary>
        /// The IsWindowVisible function retrieves the visibility state of the specified window
        /// </summary>
        [DllImport("user32.dll", ExactSpelling = true, EntryPoint = "IsWindowVisible", CharSet = CharSet.Auto)]
        private static extern bool IntIsWindowVisible(HandleRef hwnd);

        #endregion

        #region File Helpers

        /// <summary>
		/// First try and load content from Application resources, then go to disk.
		/// </summary>
		/// <param name="filename">Name of file or resource to load.</param>
		/// <exception cref="FileNotFoundException">If content with given name is not found.</exception>
		public static IDocumentPaginatorSource LoadContent(string filename)
		{
			IDocumentPaginatorSource idp;
			
			 idp = (IDocumentPaginatorSource)LoadAsResource(filename);
			if (idp == null)
                idp = (IDocumentPaginatorSource)LoadAsLooseFile(filename);

			if (idp == null)
				throw new FileNotFoundException("Could not load '" + filename + "' as resource or loose file.");
			return idp;
		}

        /// <summary>
        /// First try and load content from Application resources, then go to disk.
        /// </summary>
        /// <param name="filename">Name of file or resource to load.</param>
        /// <exception cref="FileNotFoundException">If content with given name is not found.</exception>
        public static object LoadXaml(string filename)
        {
            object xaml;

            xaml = LoadAsResource(filename);
            if (xaml == null)
                xaml = LoadAsLooseFile(filename);

            if (xaml == null)
                throw new FileNotFoundException("Could not load '" + filename + "' as resource or loose file.");
            return xaml;
        }

		/// <summary>
		/// Try and load the given filename from the Application's Resources.
		/// </summary>
		/// <returns>IDP or null if resource does not exist or is of wrong type.</returns>
		private static object LoadAsResource(string filename) 
		{
			object xaml = null;
            try
            {
                Stream resourceStream = null;

                if (DriverState.TestName.EndsWith("XBAP"))
                {
                    resourceStream = Application.GetResourceStream(new Uri(filename, UriKind.Relative)).Stream;
                }
                else
                {
                    AnnotationsTestSettings settings = new AnnotationsTestSettings();
                    GlobalLog.LogEvidence(settings.TargetAssemblyName);
                    Assembly targetAssembly = Assembly.Load(settings.TargetAssemblyName);
                    ResourceManager resourceManager = new ResourceManager(settings.TargetAssemblyName + ".g", targetAssembly);
                    resourceManager.IgnoreCase = true;
                    resourceStream = (Stream)resourceManager.GetObject(filename);
                }

                if (resourceStream != null)
                {
                    xaml = XamlReader.Load(resourceStream);
                }
            }
            catch (Exception)
            {
                // ignore.
            }
            return xaml;
		}

		/// <summary>
		/// Try and load the given filename from disk.
		/// </summary>
		/// <returns>IDP or null if file does not exist or is of wrong type.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		private static object LoadAsLooseFile(string filename)
		{
			object xaml = null;
			try
			{
				Stream stream = new FileStream(filename, FileMode.Open);
				xaml = XamlReader.Load(stream);
				stream.Close();
			}
			catch (Exception)
			{
				// ignore.
			}
            return xaml;
		}

		#endregion

		#region Tree Helpers

		/// <summary>
		/// Build the following tree and return the Canvas root:
		/// 
		///              Canvas
		///           "mainCanvas"
		///                 |
		///             DockPanel
		///          "mainDockPanel"
		///                 |
		///             DockPanel
		///            "_dockpanel1"
		///                 |
		///               Button
		///           "btnAddAnnot1"
		/// 
		/// </summary>
		public static Canvas BuildSingleBranchTree()
		{
			Canvas mainCanvas = new Canvas();
			mainCanvas.Name = "mainCanvas";

			DockPanel mainDockPanel = new DockPanel();
			mainDockPanel.Name = "mainDockPanel";
			DataIdProcessor.SetDataId(mainDockPanel, mainDockPanel.Name);

			DockPanel dp1 = new DockPanel();
			dp1.Name = "_dockpanel1";
			DataIdProcessor.SetDataId(dp1, dp1.Name);
			DockPanel.SetDock(dp1, Dock.Top | Dock.Left);

			Button btnAddAnnot = new Button();
			btnAddAnnot.Name = "btnAddAnnot";
			DataIdProcessor.SetDataId(btnAddAnnot, btnAddAnnot.Name);
			btnAddAnnot.Content = "Add Annotation";

			mainCanvas.Children.Add(mainDockPanel);
			mainDockPanel.Children.Add(dp1);
			mainDockPanel.Children.Add(btnAddAnnot);

			return mainCanvas;
		}

		/// <summary>
		/// Build the following tree and return the Canvas root:
		/// 
		///              Canvas
		///           "mainCanvas"
		///                 |
		///             DockPanel
		///          "mainDockPanel"
		///           /            \
		///          /              \
		///         /                \
		///     DockPanel          DockPanel
		///   "_dockpanel1"       "_dockpanel2"
		///     /         \               \
		///   Canvas      Button          TextBox
		///  "dp1Canvas"  "btnAddAnnot1"  "textbox1"
		///             
		/// 
		/// NOTE: sets FetchAsBatch to true
		/// </summary>
		public static Canvas BuildMultiBranchTree()
		{
			Canvas mainCanvas = new Canvas();
			mainCanvas.Name = "mainCanvas";

			DockPanel mainDockPanel = new DockPanel();
			mainDockPanel.Name = "mainDockPanel";
			DataIdProcessor.SetDataId(mainDockPanel, mainDockPanel.Name);

			// - - - - Build 1st subtree - - - - //

			DockPanel dp1 = new DockPanel();
			dp1.Width = 150;
			dp1.Name = "_dockpanel1";
			DataIdProcessor.SetDataId(dp1, dp1.Name);
			DockPanel.SetDock(dp1, Dock.Top | Dock.Left);

			Button btnAddAnnot1 = new Button();
			btnAddAnnot1.Name = "btnAddAnnot1";
			DataIdProcessor.SetDataId(btnAddAnnot1, btnAddAnnot1.Name);
			btnAddAnnot1.Content = "Add Annotation";

			Canvas dp1Canvas = new Canvas();
			dp1Canvas.Name = "dp1Canvas";
			DataIdProcessor.SetDataId(dp1Canvas, dp1Canvas.Name);

			dp1.Children.Add(btnAddAnnot1);
			dp1.Children.Add(dp1Canvas);

			// - - - - Build 2nd Subtree - - - - //

			DockPanel dp2 = new DockPanel();
			dp2.Name = "_dockpanel2";
			DataIdProcessor.SetDataId(dp2, dp2.Name);
			DockPanel.SetDock(dp2, Dock.Bottom | Dock.Left);

			TextBox textbox = new TextBox();
			textbox.Name = "textbox1";
			DataIdProcessor.SetDataId(textbox, textbox.Name);
			textbox.Text = "I am a text box that contains text that is devoid of content, but servers a purpose none the less.";
			textbox.Select(10, 10);

			dp2.Children.Add(textbox);

			// - - - - Finish tree - - - - //

			mainDockPanel.Children.Add(dp1);
			mainDockPanel.Children.Add(dp2);
			mainCanvas.Children.Add(mainDockPanel);

			DataIdProcessor.SetFetchAnnotationsAsBatch(mainCanvas, true);

			return mainCanvas;
		}

		/// <summary>
		/// Add annotations to AnnotationTestHelper.BuildMultiBranchTree.
		/// </summary>
		/// <returns>A map from Node id (e.g. "mainDockPanel" etc) to all annotations 
		/// attached to the subtree rooted at this node.</returns>
		public static Hashtable AnnotateMultiBranchTree(DependencyObject root)
		{
			Window window = new Window();
			window.Content = (UIElement)root;
			window.Show();

			AnnotationService service = AnnotationService.GetService(root);
			AnnotationStore store = service.Store;

			Annotation mainDPAnno = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel"));
			store.AddAnnotation(mainDPAnno);

			// DockPanel1 subtree.
			Annotation dp1Anno = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "_dockpanel1"));
			Annotation dp1CanvasAnno = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "dp1Canvas"));
			Annotation dp1Btn1Anno = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "btnAddAnnot1"));
			store.AddAnnotation(dp1Anno);
			store.AddAnnotation(dp1CanvasAnno);
			store.AddAnnotation(dp1Btn1Anno);

			// DockPanel2 subtree.
			Annotation dp2Anno1 = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2"));
			Annotation dp2Anno2 = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2"));
			TextBox tb = (TextBox)LogicalTreeHelper.FindLogicalNode(root, "textbox1");
			Annotation dp2TextBoxAnno = AnnotationTestHelper.makeSimpleAnnotation(service, tb);

            PropertyInfo TextSelectionInfo = typeof(TextBoxBase).GetProperty("TextSelectionInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextSelectionInfo == null)
            {
                throw new Exception("TextBoxBase.TextSelectionInternal property is not accessible");
            }
            TextSelection textSelection = TextSelectionInfo.GetValue(tb, null) as TextSelection;

            Annotation dp2TextRangeAnno = AnnotationTestHelper.makeSimpleAnnotation(service, new TextRange(textSelection.Start, textSelection.End));
			store.AddAnnotation(dp2Anno1);
			store.AddAnnotation(dp2Anno2);
			store.AddAnnotation(dp2TextBoxAnno);
			store.AddAnnotation(dp2TextRangeAnno);

			// Map from Logical Node id -> array of Annotations at and below this node.
			// (e.g. All annotations attached to the subtree represented by node id.)
			Hashtable annotationAttachmentMap = new Hashtable();

			annotationAttachmentMap.Add("mainDockPanel", new Annotation[] { mainDPAnno, dp1Anno, dp1CanvasAnno, dp1Btn1Anno, dp2Anno1, dp2Anno2, dp2TextBoxAnno, dp2TextRangeAnno });
			annotationAttachmentMap.Add("_dockpanel1", new Annotation[] { dp1Anno, dp1CanvasAnno, dp1Btn1Anno });
			annotationAttachmentMap.Add("dp1Canvas", new Annotation[] { dp1CanvasAnno });
			annotationAttachmentMap.Add("btnAddAnnot1", new Annotation[] { dp1Btn1Anno });
			annotationAttachmentMap.Add("_dockpanel2", new Annotation[] { dp2Anno1, dp2Anno2, dp2TextBoxAnno, dp2TextRangeAnno });
			annotationAttachmentMap.Add("textbox1", new Annotation[] { dp2TextBoxAnno, dp2TextRangeAnno });

			return annotationAttachmentMap;
		}

		/// <summary>
		/// Load given XAML file.  Return the root of the tree.
		/// </summary>
		/// <param name="xamlFileName">Relative path of xaml file to load.</param></param>
		/// <returns>Root of the tree.</returns>
		public static object LoadTree(string xamlFileName)
		{
			Stream s = new FileStream(xamlFileName, FileMode.Open);
			ParserContext pc = new ParserContext();
			pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
            pc.BaseUri = new Uri("pack://siteoforigin:,,,/");
            object rootNode = XamlReader.Load(s, pc);
			s.Close();
			return rootNode;
		}

		#endregion Tree helpers

		#region Annotation helpers

		/// <summary>
		/// Make a simple annotation that has an anchor attached to the given object.
		/// </summary>
		public static Annotation makeSimpleAnnotation(AnnotationService service, object location)
		{
			AnnotationResource anchor = AnnotationTestHelper.makeAnchor(service, location);
			Annotation anno = new Annotation(new XmlQualifiedName("SimpleAnnotation", "AnnotationTests"));
			anno.Anchors.Add(anchor);

			return anno;
		}

		/// <summary>
		/// Create and populate and annotation for use in testing that has an author and
		/// multiple cargos.
		/// </summary>
		public static Annotation MakeAnnotation1()
		{
			Annotation an1 = new Annotation(new XmlQualifiedName("Stodgy", "foo"));

			XmlDocument xdoc = new XmlDocument();
			XmlNode AuthsNode = xdoc.CreateNode(XmlNodeType.Element, "Author", "testAuthorNamespace");

			AuthsNode.InnerXml = "<stringauthor>Mr Stodgy</stringauthor>";
			an1.Authors.Add("Mr Stodgy");

			// Make a context and add it
			AnnotationResource cont = new AnnotationResource();

			ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("MyLocatorPart", "testLocatorPartsNamespace"));
			part.NameValuePairs.Add("Value", "LocatorInner Xml part");

			ContentLocator seq = new ContentLocator();
			seq.Parts.Add(part);
			cont.ContentLocators.Add(seq);

			// Make a cargo and add it
			AnnotationResource res = new AnnotationResource();
			XmlElement CargoNode = (XmlElement)xdoc.CreateNode(XmlNodeType.Element, "MyCargo", "testCargoNamsepace");

			CargoNode.InnerXml = "<partone>contents of the cargo first piece</partone><partdeux> and here is another bit of it</partdeux>";
			res.Contents.Add(CargoNode);
			an1.Cargos.Add(res);
			an1.Cargos.Add(cont);
			return an1;
		}

		/// <summary>
		/// Create and populate and annotation for use in testing that has an author,
		/// a non-standard anchor (e.g. you would need a custom processor to resolve it), 
		/// and a single cargo.
		/// </summary>
		public static Annotation MakeAnnotation2()
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
		/// Uses MakePartiallyResolvedAnchor.
		/// see 'MakePartiallyResolvedAnchor' for descriptions of parameters.
		/// </summary>
		/// <returns>Annotaiton that will partially resolve.</returns>
		/// <exception cref="ArgumentException">If nodeId is not a UIElement, or parent of nodeId is not 
		/// a Panel</exception>
		public static Annotation MakePartiallyResolvedAnnotation(AnnotationService service, DependencyObject root, string nodeId)
		{
			AnnotationResource anchor = AnnotationTestHelper.MakePartiallyResolvedAnchor(service, root, nodeId);
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "AnnotationtestHelper"));
			anno.Anchors.Add(anchor);
			return anno;
		}

		/// <summary>
		/// Create an annotation that has some content and anchors, but no authors.
		/// </summary>
		/// <param name="tempannotationStore"></param>
		/// <returns></returns>
		public static Annotation UnauthoredAnnotation()
		{
			Annotation ann = new Annotation(new XmlQualifiedName("Secondo", "foo"));

			XmlDocument xdoc = new XmlDocument();

			// Make a context and add it
			AnnotationResource cont = new AnnotationResource();
			ContentLocator loc = new ContentLocator();
			ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("TheirLocatorPart", "testLocatorPartsNamespace"));

			part.NameValuePairs.Add("Value", "<Another> Inner Xml part</Another>");
            loc.Parts.Add(part);
			cont.ContentLocators.Add(loc);
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
		/// Create an annotation that has some content and an author, but no anchors.
		/// </summary>
		/// <param></param>
		/// <returns></returns>
		public static Annotation UnanchoredAnnotation()
		{
			Annotation ann = new Annotation(new XmlQualifiedName("Ternary", "blah blah blah"));

			XmlDocument xdoc = new XmlDocument();

			// Make an author and add it
			XmlNode myAuthor = AnnotationTestHelper.CreateAuthor(xdoc, "Some one else");
			ann.Authors.Add("Some one else");

			// Make a cargo and add it
			AnnotationResource res = new AnnotationResource();
			XmlElement CargoNode = (XmlElement)xdoc.CreateNode(XmlNodeType.Element, "MyCargo", "testCargoNamsepace");
			CargoNode.InnerXml = "Contents of the cargo <partone/><partdeux> and here is another bit of it</partdeux>";
			res.Contents.Add(CargoNode);
			ann.Cargos.Add(res);
			return ann;
		}

		/// <summary>
		/// Create an annotation that has some content and an author, and a AnnotationResource with no Locators.
		/// </summary>
		/// <param></param>
		/// <returns></returns>
		public static Annotation ResourceBlankAnnotation()
		{
			Annotation ann = new Annotation(new XmlQualifiedName("This is a much longer string to put in here at this point", "blah blah blah"));

			XmlDocument xdoc = new XmlDocument();

			// Make an author and add it
			XmlNode myAuthor = AnnotationTestHelper.CreateAuthor(xdoc, "Some one else");
			ann.Authors.Add("Some one else");

			// Make a cargo and add it
			AnnotationResource res = new AnnotationResource();
			XmlElement CargoNode = (XmlElement)xdoc.CreateNode(XmlNodeType.Element, "MyCargo", "testCargoNamsepace");
			CargoNode.InnerXml = "Contents of the cargo <partone/><partdeux> and here is another bit of it</partdeux>";
			res.Contents.Add(CargoNode);
			ann.Cargos.Add(res);

			// Make a blank Resouce and add to the annotation
			AnnotationResource res1 = new AnnotationResource();
			ann.Anchors.Add(res1);

			return ann;
		}

		/// <summary>
		/// If GetAttachedAnnotations() returns a collection with only 1 AttachedAnnotation, then return it.
		/// </summary>
		/// <returns>AttachedAnnotation or null if no attached annotations.</returns>
		/// <exception cref="ArgumentException">If GetAttachedAnnotations.Count > 1.</exception>
		public static IAttachedAnnotation GetOnlyAttachedAnnotation(AnnotationService service)
		{
			ICollection<IAttachedAnnotation> attachedAnnots = service.GetAttachedAnnotations();

			if (attachedAnnots.Count > 1)
				throw new ArgumentException("GetOnlyAttachedAnnotation should not be calledif there are more than 1 attached annotations.");
			if (attachedAnnots.Count == 0)
				return null;

			IEnumerator<IAttachedAnnotation> enumer = attachedAnnots.GetEnumerator();
			enumer.MoveNext();
			return enumer.Current;
		}		

		#endregion Annotation helpers

		public static XmlElement CreateContent(string name, string content)
		{
			XmlDocument xdoc = new XmlDocument();
			XmlElement element = xdoc.CreateElement(name);
			element.InnerText = content;
			return element;
        }

        #region TextAnchor helpers

        public static bool IsTextAnchor(object obj)
        {
            return obj != null && obj.GetType().Name == "TextAnchor";
        }

        public static string GetText(object obj)
        {
            return ReflectionHelper.GetProperty(obj, "Text") as string;
        }

        #endregion TextAnchor helpers

        #region Cargo helpers

        /// <summary>
		/// Create an XmlNode of type Element, with the given contents.
		/// </summary>
		public static AnnotationResource MakeCargo(string type, string nameSpace, string content)
		{
			AnnotationResource res = new AnnotationResource();
			XmlDocument xdoc = new XmlDocument();
			XmlElement CargoNode = (XmlElement)xdoc.CreateNode(XmlNodeType.Element, type, nameSpace);
			CargoNode.InnerXml = "<content>" + content + "</content>";
			res.Contents.Add(CargoNode);
			return res;
		}

		/// <summary>
		/// Create an XmlNode of type Element, with the given contents with a default
		/// namespace and type.
		/// </summary>
		public static AnnotationResource MakeCargo(string content)
		{
			return AnnotationTestHelper.MakeCargo("type", "namespace", content);
		}

		#endregion Cargo helpers

		#region Author helpers

		/// <summary>
		/// Create an XmlElement from the given document whose name is "AuthorElement" and 
		/// InnerText is the given Author.
		/// </summary>
		/// <param name="author">Value of InnerText.</param>
		/// <returns>Initialized XmlNode.</returns>
		public static XmlNode CreateAuthor(XmlDocument xdoc, string author)
		{
			XmlNode node = xdoc.CreateNode(XmlNodeType.Element, "testType", "testNamespace");
			node.InnerText = author;
			return node;
		}

		#endregion Author helpers

		#region Anchor helpers

		/// <summary>
		/// Create an anchor attached to the given object using this AnnotationService's
		/// LocatorManager.
		/// </summary>
		public static AnnotationResource makeAnchor(AnnotationService service, object location)
		{
			AnnotationResource anchor = new AnnotationResource();
			IList<ContentLocatorBase> locs = service.LocatorManager.GenerateLocators(location);
			foreach(ContentLocatorBase loc in locs)
                        anchor.ContentLocators.Add(loc);
			return anchor;
		}

		/// <summary>
		/// Create an anchor AnnotationResource with specified name and namespace
		/// and a set content.
		/// </summary>
		/// <param name="anchorName">Value of name.</param>
		/// <param name="anchorNamespace">Value of namespace.</param>
		/// <returns>Initialized anchor AnnotationResource.</returns>
		public static AnnotationResource CreateAnchor(string anchorName, string anchorNamespace)
		{
			AnnotationResource cont = new AnnotationResource();
			ContentLocator loc = new ContentLocator();
			ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName(anchorName, anchorNamespace));

			part.NameValuePairs.Add("Value", "<Another> Inner Xml part</Another>");
            loc.Parts.Add(part);
			cont.ContentLocators.Add(loc);
			return cont;
		}

		/// <summary>
		/// Create an anchor AnnotationResource with specified name, namespace
		/// and content for value.
		/// </summary>
		/// <param name="anchorName">Value of name.</param>
		/// <param name="anchorNamespace">Value of namespace.</param>
		/// <param name="partContent">Value of the locator part.</param>
		/// <returns>Initialized anchor AnnotationResource.</returns>
		public static AnnotationResource CreateAnchor(string anchorName, string anchorNamespace, string partContent)
		{
			AnnotationResource cont = new AnnotationResource();
			ContentLocator loc = CreateLPS(anchorName, anchorNamespace, partContent);
			cont.ContentLocators.Add(loc);
			return cont;
		}

		/// <summary>
		/// Modify an anchor AnnotationResource by adding a locator.
		/// </summary>
		/// <param name="anchorName">Value of name.</param>
		/// <param name="anchorNamespace">Value of namespace.</param>
		/// <param name="partContent">Value of the locator part.</param>
		/// <returns>Initialized anchor AnnotationResource.</returns>
		public static void ModifyAnchor(AnnotationResource anchor)
		{
			ContentLocator loc = new ContentLocator();
			ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("NAwhatsME", "Sp ace"));

			part.NameValuePairs.Add("Value", "string is");
            loc.Parts.Add(part);
			anchor.ContentLocators.Add(loc);
		}

		/// <summary>
		/// Helper for creating a partially resolved anchor. 
		/// Creates a partially resolved anchor by getting the given node, and its parent.
		/// Creating locators for nodeId, and then removing this node from its parent.  
		/// 
		/// PreConditions: 
		///  -nodeId must be a valid node and it must be a UIElement.
		///  -parent of nodeId must be of type Panel (otherwise we don't know how to remove children).
		/// 
		/// </summary>
		/// <param name="service"></param>
		/// <param name="root">Root of tree to locate nodeID within.</param>
		/// <param name="nodeId">Name of UIElement to create an anchor on.</param>
		/// <returns>Anchor with locators for node with given id.</returns>
		/// <exception cref="ArgumentException">If nodeId is not a UIElement, or parent of nodeId is not 
		/// a Panel</exception>
		public static AnnotationResource MakePartiallyResolvedAnchor(AnnotationService service, DependencyObject root, string nodeId)
		{
			DependencyObject node = LogicalTreeHelper.FindLogicalNode(root, nodeId);
			if (node == null)
				throw new ArgumentException("No node with id '" + nodeId + "' exists.");

			DependencyObject parent = LogicalTreeHelper.GetParent(node);

			if (!(node is UIElement))
				throw new ArgumentException("Node with id '" + nodeId + "' is not of type UIElement.");
			if (!(parent is Panel))
				throw new ArgumentException("Parent of node '" + nodeId + "' must be of type Panel.");

			UIElement element = node as UIElement;
			Panel parentPanel = parent as Panel;

			AnnotationResource anchor = AnnotationTestHelper.makeAnchor(service, node);
			parentPanel.Children.Remove(element);

			return anchor;
		}

		#endregion Anchor helpers

		#region ContentLocator helpers

		/// <summary>
		/// Create a ContentLocatorBase with specified name, namespace
		/// and content for value.
		/// </summary>
		/// <param name="anchorName">Value of name.</param>
		/// <param name="anchorNamespace">Value of namespace.</param>
		/// <param name="partContent">Value of the locator part.</param>
		/// <returns>Initialized anchor AnnotationResource.</returns>
		public static ContentLocator CreateLPS(string anchorName, string anchorNamespace, string partContent)
		{
			ContentLocator loc = new ContentLocator();
            loc.Parts.Add(CreateLocatorPart(anchorName, anchorNamespace, partContent));
			return loc;
		}

		/// <summary>
		/// Create a ContentLocator that contains a ContentLocatorParts with given contents.
		/// Default type and namespace are used.
		/// </summary>
		public static ContentLocator CreateLocator(string [] partContents)
		{
			ContentLocator loc = new ContentLocator();
			foreach (string part in partContents)
				loc.Parts.Add(CreateLocatorPart(part));	
			return loc;
		}

		/// <summary>
		/// Create trivial ContentLocatorPart that has one key value pair where Key='Value' and Value='content'.
		/// </summary>
		/// <remarks>
		/// Uses default type and namespace.
		/// </remarks>
		public static ContentLocatorPart CreateLocatorPart(string content)
		{
			ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("type", "nsp"));
			part.NameValuePairs.Add("Value", content);
			return part;
		}

		/// <summary>
		/// Create trivial ContentLocatorPart that has one key value pair where Key='Value' and Value='content'.
		/// </summary>
		public static ContentLocatorPart CreateLocatorPart(string name, string nsp, string content)
		{
			ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName(name, nsp));
			part.NameValuePairs.Add("Value", content);
			return part;
		}

		/// <summary>
		/// Create a ContentLocatorGroup.
		/// </summary>
		/// <param name="locatorCount">Number of locators.</param>
		/// <param name="locatorPartCount">Number of ContentLocatorParts per ContentLocator.</param>
		/// <param name="lpContent">Single NameValuePair entry for each ContentLocatorPart.</param>
		public static ContentLocatorGroup CreateLocatorGroup(int locatorCount, int locatorPartCount, string lpContent)
		{
			ContentLocatorGroup group = new ContentLocatorGroup();
			for (int i = 0; i < locatorCount; i++)
			{
				ContentLocator loc = new ContentLocator();
				for (int k=0; k < locatorPartCount; k++)
					loc.Parts.Add(CreateLocatorPart(lpContent));
				group.Locators.Add(loc);
			}
			return group;
		}

		public static bool LocatorSetsEqual(ContentLocatorGroup setA, ContentLocatorGroup setB)
		{
			if (setA == setB)
				return true;
			if (setA.Locators.Count != setB.Locators.Count)
				return false;

			IEnumerator<ContentLocator> enumA = setA.Locators.GetEnumerator();
			while (enumA.MoveNext())
			{
				bool match = false;
				IEnumerator<ContentLocator> enumB = setB.Locators.GetEnumerator();
				while (enumB.MoveNext())
				{
					if (LocatorPartListsEqual(enumA.Current, enumB.Current))
					{
						match = true;
						break;
					}
				}
				if (!match)
					return false;
			}

			return true;
		}

		public static bool LocatorPartListsEqual(ContentLocator listA, ContentLocator listB)
		{
			bool same = true;
			if (listA == listB)
				return true;
			if (listA.Parts.Count == listB.Parts.Count)
			{
				for (int i = 0; i < listA.Parts.Count; i++)
				{
					ContentLocatorPart partA = listA.Parts[i];
					ContentLocatorPart partB = listB.Parts[i];
					IEnumerator<KeyValuePair<string, string>> enumer = partA.NameValuePairs.GetEnumerator();
					while (enumer.MoveNext())
					{
						if (!partB.NameValuePairs.Keys.Contains(enumer.Current.Key) || !partB.NameValuePairs.Values.Contains(enumer.Current.Value))
						{
							same = false;
							break;
						}
					}
				}
			}
			else
				same = false;
			return same;
		}

		public static ContentLocator GetOnlyLocator(ICollection<ContentLocator> locatorCollection)
		{
			if (locatorCollection.Count != 1)
				throw new ArgumentException("GetOnlyLocator should only be called when collection contains only 1 locator.");
			IEnumerator<ContentLocator> locatorEnum = locatorCollection.GetEnumerator();
			locatorEnum.MoveNext();
			return locatorEnum.Current;
		}

		public static ContentLocatorPart GetOnlyLocatorPart(ICollection<ContentLocatorPart> locatorPartCollection)
		{
			if (locatorPartCollection.Count != 1)
				throw new ArgumentException("GetOnlyLocatorPart should only be called when collection contains only 1 ContentLocatorPart.");
			IEnumerator<ContentLocatorPart> enumer = locatorPartCollection.GetEnumerator();
			enumer.MoveNext();
			return enumer.Current;
		}

		public static void AddLocators(AnnotationResource resource, ICollection<ContentLocatorBase> locCollect)
		{
			foreach (ContentLocatorBase loc in locCollect)
				resource.ContentLocators.Add(loc);
		}

		public static void ReplaceAllLocators(AnnotationResource resource, ICollection<ContentLocatorBase> locCollect)
		{
			resource.ContentLocators.Clear();
			AddLocators(resource, locCollect);
		}

		#endregion
	}
}
