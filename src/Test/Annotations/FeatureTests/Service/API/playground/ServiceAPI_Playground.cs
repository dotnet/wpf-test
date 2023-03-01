// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Markup;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Threading; using System.Windows.Threading;
using System.Windows.Annotations;
using System.Windows.Annotations.Anchoring;
using System.Windows.Annotations.Storage;


namespace Annotations.TestCases.Service
{
	/// <summary>
	/// Set of experimental test cases.
	/// </summary>
	public class ServiceAPI_Playground : AnnotationTestDriver
	{
		// name of xml file for store
		public const string XMLFILESTORE = ".\\serviceTestStore.xml";
		public const string XMLFILESTORE2 = ".\\fullTestStore.xml";

		// Holds the case to be run this time
		private static string g_runCase;

		[STAThread]
		static int Main(string[] args)
		{
			if ((args == null) || (args.Length == 0) || (args[0] == null))
				throw new Exception("ERROR: test name not provided to test driver.");

			g_runCase = args[0].ToLower();

			ServiceAPI_Playground test = new ServiceAPI_Playground();
			return test.Run();
		}

		public ServiceAPI_Playground() : base()
		{
			try
			{
				// Cleanup from previous run of this test
				FileInfo fi = new FileInfo(XMLFILESTORE);
				fi.Delete();

				// Set up Exception handler for events on the rest of the application. May be service inti exceptions
				Application.Current.Context.DispatcherException += new DispatcherExceptionEventHandler(OutsideExceptionHandler);
			}
			catch (Exception E)
			{
				g_autoFrmk.LogTest(false, "Exception in TEST constructor: " + E.ToString());
			}
		}

		/// <summary>
		/// Automatically called when application runs.  
		/// Selects a testcase to run based on the command line args.
		/// </summary>
		/// <param name="args">Case number to run.</param>
		protected override void OnStartup(StartupEventArgs args)
		{
			base.OnStartup(args);
			try
			{
				switch (g_runCase)
				{
					// TC dummy
					case "dummy":
						DummyTests dummy = new DummyTests(this);
						dummy.TestDummy();
						break;

					default:
						g_autoFrmk.LogTest(false, "No test exists for the label " + g_runCase);
						this.Shutdown();
						break;
				}
			}
			catch (Exception E)
			{
				g_autoFrmk.LogTest(false, "Unhandled exception thrown:\n" + E.ToString() + "\n");
				this.Shutdown();
			}
		}

		/// <summary>
		/// Automatically called when application shuts down.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnExit(object sender, ExitEventArgs e)
		{

		}

		#region TestResolve
/*
*/
		#endregion TestResolve

		/// <summary>
		/// Dummy is a testing playground.  It is isolated in its own class
		/// to protect other tests.
		/// </summary>
		class DummyTests : AServiceAPITests
		{
			public DummyTests(AnnotationTestDriver parent) : base(parent)
			{
				g_annotationStore = new XmlFileStore(XMLFILESTORE);
			}

			#region Dummy

			const string EVENT_LOG_FILE = "C:\\annotationsEventsLog.txt";

			private AnnotationStore g_annotationStore;

			int g_addCount = 0;
			int g_deleteCount = 0;
			int g_modifyCount = 0;

			// Handler for the events from the Service. Tallies the number of each event that
			// is fired.
			private void AAUpdatedEventHandler(object sender, AttachedAnnotationUpdatedEventArgs e)
			{
				if (e.Action == AttachedAnnotationAction.Added)
					g_addCount++;

				if (e.Action == AttachedAnnotationAction.Deleted)
					g_deleteCount++;

				if (e.Action == AttachedAnnotationAction.Modified)
					g_modifyCount++;

				StreamWriter logFile = new StreamWriter(EVENT_LOG_FILE);
				logFile.WriteLine(" Adds :" + g_addCount.ToString());
				logFile.WriteLine(" Deletes :" + g_deleteCount.ToString());
				logFile.WriteLine(" Modifys :" + g_modifyCount.ToString());
				logFile.Close();

				Console.WriteLine("event" + e.Action);
			}

			private void PrintList(IList list)
			{
				foreach (IAttachedAnnotation aa in list)
				{
					Console.WriteLine(aa.Annotation.Id);
				}
			}

			// construct two services and attach to a non-root node and make sure we can get the store from a descendent
			public void TestDummy()
			{
				DockPanel g_dockP1 = null;
				DockPanel g_dockP2 = null;
				Canvas g_mainCanvas = null;
				AnnotationService g_svc1 = null;
				// AnnotationService g_svc2 = null;
				bool g_failed = false;

				// build the tree to test this on
				try
				{
					g_mainCanvas = ServiceAPIHelpers.buildBigCanvas();
					// g_mainCanvas.SetValue(AnnotationService.DataIdProperty, "mc");
					g_dockP1 = (DockPanel)LogicalTreeHelper.FindLogicalNode(g_mainCanvas, "_dockpanel1");
					g_dockP1.SetValue(AnnotationService.DataIdProperty, "dock1");
					g_dockP2 = (DockPanel)LogicalTreeHelper.FindLogicalNode(g_mainCanvas, "_dockpanel2");
				}
				catch (Exception E)
				{
					g_failed = true;
					g_autoFrmk.LogTest(false, "3 - Exception thrown building canvas in constructor: " + E.ToString() + "\n");
				}

				// construct the first annotation service
				if (!g_failed)
				{
					try
					{
						AnnotationService.SetStore(g_mainCanvas, g_annotationStore);
//					g_svc1 = new AnnotationService();
//					ServiceManager.SetServiceEnabled(typeof(AnnotationService), g_mainCanvas, true);
//					ServiceManager.SetService(typeof(AnnotationService), g_mainCanvas, g_svc1);
//					((IScopedService)g_svc1).Attach(g_mainCanvas);

						AnnotationService.Enable(g_mainCanvas);
						g_svc1 = AnnotationService.GetService(g_mainCanvas);
					}
					catch (Exception E)
					{
						g_failed = true;
						g_autoFrmk.LogTest(false, "3 - Exception thrown attaching first service to the tree: " + E.ToString() + "\n");
					}
				}

				// set an event handler for the AttachedAnnotationUpdated event
				g_svc1.AttachedAnnotationUpdated += new AttachedAnnotationUpdatedEventHandler(AAUpdatedEventHandler);

				// register the tree processors for the test
//			// g_svc1.RegisterSelectionProcessor(new TreeNodeSelectionProcessor(g_svc1.LocatorManager), typeof(Window), new XmlQualifiedName[0]);
//			// g_svc1.RegisterSelectionProcessor(new TreeNodeSelectionProcessor(g_svc1.LocatorManager), typeof(DockPanel), new XmlQualifiedName[0]);

				AnnotationResource g_anchor = g_annotationStore.CreateResource();
				IList g_locs = g_svc1.LocatorManager.GenerateLocators(g_dockP1);
				foreach (object locator in g_locs)
					g_anchor.Locators.Add(locator);
				Annotation g_anno1 = g_annotationStore.CreateAnnotation();
				g_anno1.TypeName = "ServiceTestAnn";
				g_anno1.TypeNamespaceUri = "caf";
				g_anno1.Anchors.Add(g_anchor);
				g_annotationStore.AddAnnotation(g_anno1);
				g_anno1.TypeName = "TestAnn";
				g_annotationStore.ModifyAnnotation(g_anno1);
				// g_annotationStore.DeleteAnnotation(g_anno1.Id);

				IList g_attachList = g_svc1.GetAttachedAnnotations();
				PrintList(g_attachList);

//			// construct the second annotation service
//			if (!g_failed)
//			{
//				try
//				{
//					g_svc2 = new AnnotationService();
//				}
//				catch (Exception E)
//				{
//					g_failed = true;
//					g_autoFrmk.LogTest(false, "3 - Exception thrown in second service constructor: " + E.ToString() + "\n");
//				}
//			}
//
//			// attach the first annotation service to the tree
//			if (!g_failed)
//			{
//				try
//				{
//					switch (caseNumber)
//					{
//						case "2":
//							AnnotationService.SetStore(g_dockP2, g_annotationStore);
//							break;
//
//						default:
//							g_autoFrmk.LogTest(false, "3 - Unknown case number\n");
//							break;
//					}
//					ServiceManager.SetServiceEnabled(typeof(AnnotationService), g_dockP2, true);
//					ServiceManager.SetService(typeof(AnnotationService), g_dockP2, g_svc2);
//					((IScopedService)g_svc2).Attach(g_dockP2);
//				}
//				catch (Exception E)
//				{
//					g_failed = true;
//					g_autoFrmk.LogTest(false, "3 - Exception thrown attaching second service to the tree: " + E.ToString() + "\n");
//				}
//			}
//
//			if (!g_failed)
//			{
//				try
//				{
//					// find a descendent in the tree to get the first service
//					Button but = (Button)LogicalTreeHelper.FindLogicalNode(g_mainCanvas, "btnAddAnnot1");
//					AnnotationService lookupSvc = (AnnotationService)ServiceManager.GetService(typeof(AnnotationService), (DependencyObject)but);
//
//					if (g_svc1 != lookupSvc)
//					{
//						g_failed = true;
//						g_autoFrmk.LogTest(false, "3.2 - Service returned from lookup is not the service created.\n");
//					}
//				}
//				catch (Exception E)
//				{
//					g_failed = true;
//					g_autoFrmk.LogTest(false, "2.2 - Exception thrown looking up the first service on the tree: " + E.ToString() + "\n");
//				}
//				if (!g_failed)
//				{
//					try
//					{
//						// find a descendent in the tree to get the second service
//						Button but = (Button)LogicalTreeHelper.FindLogicalNode(g_mainCanvas, "btnAddAnnot2");
//						AnnotationService lookupSvc = (AnnotationService)ServiceManager.GetService(typeof(AnnotationService), (DependencyObject)but);
//
//						if (g_svc2 != lookupSvc)
//						{
//							g_failed = true;
//							g_autoFrmk.LogTest(false, "3.2 - Service returned from lookup is not the service created.\n");
//						}
//					}
//					catch (Exception E)
//					{
//						g_failed = true;
//						g_autoFrmk.LogTest(false, "3.2 - Exception thrown looking up the second service on the tree: " + E.ToString() + "\n");
//					}
//					finally
//					{
//						if (!g_failed)
//							g_autoFrmk.LogTest(true, "3.2 - Constructor test two services\n");
//					}
//				}
//			}

			}

			public void TestDummyXAML()
			{
				DockPanel g_dockP1 = null;
				DockPanel g_dockP2 = null;
				Canvas g_mainCanvas = null;
				AnnotationService g_svc1 = null;

				// AnnotationService g_svc2 = null;
				bool g_failed = false;

				// build the tree to test this on
				try
				{
					Stream s = new FileStream("ComplexTree.xaml", FileMode.Open);
					ParserContext pc = new ParserContext();
					pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
                    pc.BaseUri = new Uri("pack://siteoforigin:,,,/");
					PermissionSet ps = new PermissionSet(PermissionState.Unrestricted);

					DependencyObject rootNode = (DependencyObject)XamlReader.Load(s, pc, null, ps);

					g_mainCanvas = (Canvas)LogicalTreeHelper.FindLogicalNode(rootNode, "mainCanvas");
					g_dockP1 = (DockPanel)LogicalTreeHelper.FindLogicalNode(rootNode, "_dockpanel1");
					g_dockP1.SetValue(AnnotationService.DataIdProperty, "dock1");
					g_dockP2 = (DockPanel)LogicalTreeHelper.FindLogicalNode(rootNode, "_dockpanel2");
				}
				catch (Exception exp)
				{
					g_failed = true;
					g_autoFrmk.LogTest(false, "3 - Markup:  Exception thrown building canvas in constructor: " + exp.ToString() + "\n");
				}

				// get the annotation service
				if (!g_failed)
				{
					try
					{
						g_svc1 = AnnotationService.GetService(g_mainCanvas);
					}
					catch (Exception exp)
					{
						g_failed = true;
						g_autoFrmk.LogTest(false, "3 - Markup:  Exception thrown getting first service: " + exp.ToString() + "\n");
					}
				}

				// set an event handler for the AttachedAnnotationUpdated event
				g_svc1.AttachedAnnotationUpdated += new AttachedAnnotationUpdatedEventHandler(AAUpdatedEventHandler);

				AnnotationResource g_anchor = g_annotationStore.CreateResource();
				IList g_locs = g_svc1.LocatorManager.GenerateLocators(g_dockP1);

				foreach (object locator in g_locs)
					g_anchor.Locators.Add(locator);

				Annotation g_anno1 = g_annotationStore.CreateAnnotation();

				g_anno1.TypeName = "ServiceTestAnn";
				g_anno1.TypeNamespaceUri = "caf";
				g_anno1.Anchors.Add(g_anchor);
				g_annotationStore.AddAnnotation(g_anno1);
				g_anno1.TypeName = "TestAnn";
				g_annotationStore.ModifyAnnotation(g_anno1);

				IList g_attachList = g_svc1.GetAttachedAnnotations();

				PrintList(g_attachList);
			}

			#endregion Dummy
		}

		class RalphsTests : AServiceAPITests
		{
			public RalphsTests(AnnotationTestDriver parent) : base (parent) { }

			#region RalphsTest
			/// <summary>
			/// Ralph's test method... useful as an example.
			/// </summary>
			/// <param name="root">root of logical tree created in test</param>
			/// <returns>list of valid locators generated during the test</returns>
			/* KIMS
		public IList RALPHTestLocatorGeneration(out DependencyObject root)
		{
			//Log.Write("   Testing ContentLocatorBase Generation: ");

			IList validResults = new ArrayList();
			Annotation ann = _manager.Environment.Store.CreateAnnotation();

			ann.Contexts.Add(_manager.Environment.Store.CreateResource());

			Canvas a = new Canvas();

			root = a;
			a.Name = "a";

			DockPanel b = new DockPanel();

			b.Name = "b";

			DockPanel c = new DockPanel();

			c.Name = "c";

			DockPanel d = new DockPanel();

			d.Name = "d";

			Canvas e = new Canvas();

			e.Name = "e";

			Text f = new Text();

			f.Name = "f";

			Text g = new Text();

			g.Name = "g";

			Canvas h = new Canvas();

			h.Name = "h";

			Text i = new Text();

			i.Name = "i";
			i.Text = "This is useful for testing selections on non-Paragraphs.";
			((IAddChild)a).AddChild(b);
			((IAddChild)b).AddChild(c);
			((IAddChild)b).AddChild(d);
			((IAddChild)b).AddChild(e);
			((IAddChild)c).AddChild(f);
			((IAddChild)c).AddChild(g);
			((IAddChild)d).AddChild(h);
			((IAddChild)e).AddChild(i);
			a.SetValue(IdProcessor.DataIdProperty, "_a_");
			b.SetValue(IdProcessor.DataIdProperty, "_b_");
			d.SetValue(IdProcessor.DataIdProperty, "_d_");
			f.SetValue(IdProcessor.DataIdProperty, "_f_");
			g.SetValue(IdProcessor.DataIdProperty, "_g_");
			h.SetValue(IdProcessor.DataIdProperty, "_h_");
			e.SetValue(LocatorManager.SubtreeProcessorIdProperty, "TextFingerprint");

			IList results = _manager.GenerateLocators(a);

			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 1)
				throw new Exception();

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(a);
			results = _manager.GenerateLocators(h);
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 4)
				throw new Exception();

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(h);
			results = _manager.GenerateLocators(e);
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 2)
				throw new Exception();

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(b);
			results = _manager.GenerateLocators(new DependencyObject[] { h, g });
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 1)
				throw new Exception();

			Type rangePartType = _manager.GetType().Assembly.GetType("System.Windows.Annotations.RangePart");
			PropertyInfo locators = (PropertyInfo)rangePartType.GetMember("Locators")[0];
			object range = ((ContentLocatorBase)results[0])[0];
			IList rangeLocators = (IList)locators.GetValue(range, null);

			if (range == null)
				throw new Exception();

			if (rangeLocators.Count != 2)
				throw new Exception();

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(new DependencyObject[] { h, g });

			FlowDocumentScrollViewer panel1 = new FlowDocumentScrollViewer();
			panel1.Document = new FlowDocument();

			panel1.SetValue(LocatorManager.SubtreeProcessorIdProperty, "TextFingerprint");

			Paragraph para11 = new Paragraph();
			Paragraph para12 = new Paragraph();

			((IAddChild)panel1.Document).AddChild(para11);
			((IAddChild)panel1.Document).AddChild(para12);
			((IAddChild)e).AddChild(panel1);
			para11.Append("This is the text I'm going to use.  It should be enough to get a unique hash.");
			para12.Append("You were all yellow.  I drew a line.  I drew a line for you.  Oh what a thing to do.");

			FlowDocumentScrollViewer panel2 = new FlowDocumentScrollViewer();
			panel2.Document = new FlowDocument();
			
			panel2.SetValue(LocatorManager.SubtreeProcessorIdProperty, "TextFingerprint");

			Paragraph para21 = new Paragraph();
			Paragraph para22 = new Paragraph();

			((IAddChild)panel2.Document).AddChild(para21);
			((IAddChild)panel2.Document).AddChild(para22);
			((IAddChild)h).AddChild(panel2);
			para21.Append("I know that prose will help save the day.  People will be reading instead of fighting.");
			para22.Append("You know, for you I bleed myself dry.  These are the things you do.  The thing you do.");
			results = _manager.GenerateLocators(para11);
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 3)
				throw new Exception();

			XmlElement element = ((ContentLocatorBase)results[0]).Parts[2] as XmlElement;

			if (element.ChildNodes.Count == 1)
			{
				element = (XmlElement)element.ChildNodes[0];
				if (!element.LocalName.Equals("TextFingerprint"))
					throw new Exception();
			}

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);

			// Temp work around because resolving to paragraphs isn't working
			// b is the closest we can get to the paragraph at this time
			validResults.Add(para11);
			results = _manager.GenerateLocators(para22);
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 5)
				throw new Exception();

			element = ((ContentLocatorBase)results[0]).Parts[4] as XmlElement;
			if (element.ChildNodes.Count == 1)
			{
				element = (XmlElement)element.ChildNodes[0];
				if (!element.LocalName.Equals("TextFingerprint"))
					throw new Exception();
			}

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(para22);
			results = _manager.GenerateLocators(new DependencyObject[] { para11, para22 });
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 1)
				throw new Exception();

			range = ((ContentLocatorBase)results[0]).Parts[0];
			rangeLocators = (IList)locators.GetValue(range, null);
			if (range == null)
				throw new Exception();

			if (rangeLocators.Count != 2)
				throw new Exception();

			ContentLocatorBase tempLocator = (ContentLocatorBase)rangeLocators[0];

			element = (XmlElement)tempLocator.Parts[tempLocator.Parts.Count - 1];
			if (element.ChildNodes.Count == 1)
			{
				element = (XmlElement)element.ChildNodes[0];
				if (!element.LocalName.Equals("TextFingerprint"))
					throw new Exception();
			}

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(new DependencyObject[] { para11, para22 });

			// Create a TextRange that spans some text within a Text element            
			TextPointer startTN = i.TextRange.TextContext.GetTextPointer();

			startTN.MoveTo(i.TextRange.Start);
			startTN.Move(TextUnits.Char, 15);

			TextPointer endTN = i.TextRange.TextContext.GetTextPointer();

			endTN.MoveTo(i.TextRange.Start);
			endTN.Move(TextUnits.Char, 25);

			TextRangeMovable tr = new TextTreeRange(startTN, endTN);

			results = _manager.GenerateLocators(tr);
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 4)
				throw new Exception();

			element = (XmlElement)((ContentLocatorBase)results[0]).Parts[((ContentLocatorBase)results[0]).Parts.Count - 2];
			if (element.ChildNodes.Count == 1)
			{
				element = (XmlElement)element.ChildNodes[0];
				if (!element.LocalName.Equals("TextFingerprint"))
					throw new Exception();
			}

			element = (XmlElement)((ContentLocatorBase)results[0]).Parts[((ContentLocatorBase)results[0]).Parts.Count - 1];
			if (element.ChildNodes.Count == 1)
			{
				element = (XmlElement)element.ChildNodes[0];
				if (!element.LocalName.Equals("TextRange"))
					throw new Exception();
			}

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(tr.Text);

			// Create a TextRange that spans some text within one paragraph
			startTN = para21.TextContext.GetTextPointer();
			startTN.Move(TextUnits.Char, 4);
			endTN = para21.TextContext.GetTextPointer();
			endTN.Move(TextUnits.Char, 8);
			tr = new TextTreeRange(startTN, endTN);
			results = _manager.GenerateLocators(tr);
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 6)
				throw new Exception();

			element = (XmlElement)((ContentLocatorBase)results[0]).Parts[((ContentLocatorBase)results[0]).Parts.Count - 1];
			if (element.ChildNodes.Count == 1)
			{
				element = (XmlElement)element.ChildNodes[0];
				if (!element.LocalName.Equals("TextRange"))
					throw new Exception();
			}

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(tr.Text);

			// Create a TextRange that spans from one paragraph to another
			startTN = para21.TextContext.GetTextPointer();
			startTN.Move(TextUnits.Char, 4);
			endTN = para22.TextContext.GetTextPointer();
			endTN.MoveTo(para22.Start);
			endTN.Move(TextUnits.Char, 8);
			tr = new TextTreeRange(startTN, endTN);
			results = _manager.GenerateLocators(tr);
			if (results.Count != 1)
				throw new Exception();

			if (((ContentLocatorBase)results[0]).Parts.Count != 1)
				throw new Exception();

			range = ((ContentLocatorBase)results[0]).Parts[0];
			rangeLocators = (IList)locators.GetValue(range, null);
			if (range == null)
				throw new Exception();

			if (rangeLocators.Count != 2)
				throw new Exception();

			tempLocator = (ContentLocatorBase)rangeLocators[0];
			element = (XmlElement)tempLocator.Parts[tempLocator.Parts.Count - 2];
			if (element.ChildNodes.Count == 1)
			{
				element = (XmlElement)element.ChildNodes[0];
				if (!element.LocalName.Equals("TextFingerprint"))
					throw new Exception();
			}

			element = (XmlElement)tempLocator.Parts[tempLocator.Parts.Count - 1];
			if (element.ChildNodes.Count == 1)
			{
				element = (XmlElement)element.ChildNodes[0];
				if (!element.LocalName.Equals("TextRange"))
					throw new Exception();
			}

			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(tr.Text);
			((AnnotationResource)ann.Contexts[0]).Locators.Add(results[0]);
			validResults.Add(tr.Text);
			_manager.Environment.Store.AddAnnotation(ann);
			//Log.WriteLine(" Success!");
			return validResults;
		}
			*/
			#endregion RalphsTest
		}
	}
}

