// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s testing the enablement API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.IO;
using System.Collections;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Xml;

namespace Avalon.Test.Annotations.Pri1s
{
	public class EnablementSuite_Pri1 : TestSuite
	{
		protected override void RunTestCase()
		{
			switch (CaseNumber)
			{
				case "enablement3": enablement3(); break;
				case "enablement4": enablement4(); break;
				case "enablement5": enablement5(); break;
				case "enablement6": enablement6(); break;
				case "enablement7": enablement7(); break;
				case "enablement8": enablement8(); break;
				case "enablement9": enablement9(); break;
				case "enablement10": enablement10(); break;
				case "enablement11": enablement11(); break;
				case "enablement12": enablement12(); break;
				case "enablement14": enablement14(); break;
				case "enablement15": enablement15(); break;
				case "enablement16": enablement16(); break;
				case "enablement17": enablement17(); break;
				case "enablement18": enablement18(); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		/// <summary>
		/// Enable once on non-root.  GetService on parent node returns null.  GetService on 
		/// children node returns same as GetParent on root.
		/// </summary>
		public void enablement3()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel");
			AddService(nonRoot);
			//AnnotationService anserv = new AnnotationService(nonRoot);
			//Stream annotationStream = new FileStream(_storeFilename, FileMode.Create);
			//anserv.Enable(annotationStream);

			queueTask(new DispatcherOperationCallback(enablement3_stage1), nonRoot);
		}

		public object enablement3_stage1(object node)
		{
			DependencyObject enablementNode = (DependencyObject)node;

			assert("Service above Enabled node should be null.", AnnotationService.GetService(LogicalTreeHelper.GetParent(enablementNode)) == null);

			AnnotationService rootService = AnnotationService.GetService(enablementNode);
			IEnumerator children = LogicalTreeHelper.GetChildren(enablementNode).GetEnumerator();
			while (children.MoveNext()) 
			{
				assert("Verify child service.", rootService == AnnotationService.GetService((DependencyObject)children.Current));
			}

			passTest("Verified Service above and below node of enablement.");
			return null;
		}

		/// <summary>
		/// Multiple enables on same node fail.
		/// </summary>
		public void enablement4()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel");
			AddService(nonRoot);
			assert("Verify failure exception.", AssertEnableFails(nonRoot) is InvalidOperationException);
			passTest("Expected exception occurred.");
		}

		/// <summary>
		/// Enable above existing fails.
		/// </summary>
		public void enablement5()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel");
			AddService(nonRoot);
			assert("Verify failure exception.", AssertEnableFails(root) is InvalidOperationException);
			passTest("Expected exception occurred.");
		}

		/// <summary>
		/// Enable below existing fails.
		/// </summary>
		public void enablement6()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel");
			AddService(root);
			assert("Verify failure exception.", AssertEnableFails(nonRoot) is InvalidOperationException);
			passTest("Expected exception occurred.");
		}

		/// <summary>
		/// Enable as sibling of existing succeeds.
		/// </summary>
		public void enablement7()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject leftChild = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel1");
			DependencyObject rightChild = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2");
			AddService(leftChild);
			AddSecondService(rightChild);

			passTest("Successfully added services to sibling nodes.");
		}

		/// <summary>
		/// Enable(null) throws ArgumentNullException.
		/// </summary>
		public void enablement8()
		{
			assert("Verify failure exception.", AssertEnableFails(null) is ArgumentNullException);
			passTest("Expected exception occurred.");
		}

		/// <summary>
		/// Passing DependencyObject with no DataID to Enable is ok.
		/// </summary>
		public void enablement9()
		{
			Canvas canvas = new Canvas();
			AddService(canvas);
			passTest("Can enable service on node with no DataID DP.");
		}

		/// <summary>
		/// Disable once. GetService returns null. Cached AnnotationService.IsReady == false.
		/// </summary>
		public void enablement10()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			AddService(root);
			queueTask(new DispatcherOperationCallback(enablement10_verify), root);
		}

		public object enablement10_verify(object inObj) 
		{
			DependencyObject root = (DependencyObject) inObj;

			AnnotationService service = AnnotationService.GetService(root);
			service.Disable();
			assertEquals("Service is disabled.", false, service.IsEnabled);
			assert("GetService should be null.", AnnotationService.GetService(root) == null);

			passTest("Disable works.");
			return null;
		}

		/// <summary>
		/// Disable child: no exception.
		/// </summary>
		public void enablement11()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel");
			AddService(root);
			AnnotationService.GetService(nonRoot).Disable();
			passTest("No exception.");
		}

		/// <summary>
		/// Disable parent: no exception.
		/// </summary>
		public void enablement12()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel");
			AddService(nonRoot);
			AnnotationService.GetService(nonRoot).Disable();
			passTest("No exception.");
		}

		/// <summary>
		/// Multiple disables throws.
		/// </summary>
		public void enablement14()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			AnnotationService anserv = new AnnotationService(root);
			Stream annotationStream = new FileStream(_storeFilename, FileMode.Create);
			anserv.Enable(new XmlStreamStore(annotationStream));
			anserv.Disable();
			AssertDisableFails(anserv, typeof(InvalidOperationException));
			passTest("Expected exception occurred.");
		}

		/// <summary>
		/// Disable(null) throws ArgumentNullException.
		/// </summary>
		public void enablement15()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			AssertDisableFails(new AnnotationService(root), null, typeof(InvalidOperationException));
			passTest("Expected exception occurred.");
		}

		/// <summary>
		/// Enable, disable, re-enable.
		/// </summary>
		public void enablement16()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			queueTask(new DispatcherOperationCallback(EnableTask), root);
			queueTask(new DispatcherOperationCallback(DisableTask), root);
			queueTask(new DispatcherOperationCallback(EnableTask2), root);
			queueTask(new DispatcherOperationCallback(QueueVerifyServiceEnabledTask), root);
		}

		/// <summary>
		/// Enable, disable, enable above.
		/// </summary>
		public void enablement17()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel");
			queueTask(new DispatcherOperationCallback(EnableTask), nonRoot);
			queueTask(new DispatcherOperationCallback(DisableTask), nonRoot);
			queueTask(new DispatcherOperationCallback(EnableTask2), root);
			queueTask(new DispatcherOperationCallback(QueueVerifyServiceEnabledTask), root);
		}

		/// <summary>
		/// Enable, disable, enable below.
		/// </summary>
		public void enablement18()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject nonRoot = LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel");
			queueTask(new DispatcherOperationCallback(EnableTask), root);
			queueTask(new DispatcherOperationCallback(DisableTask), root);
			queueTask(new DispatcherOperationCallback(EnableTask2), nonRoot);
			queueTask(new DispatcherOperationCallback(QueueVerifyServiceEnabledTask), nonRoot);
		}

		// ----------------------------------------------------------------------------------
		//									  TASKS
		// ----------------------------------------------------------------------------------

		public object EnableTask(object node)
		{
			AddService((DependencyObject)node);
			return null;
		}

		public object EnableTask2(object node)
		{
			AddSecondService((DependencyObject)node);
			return null;
		}

		public object DisableTask(object node)
		{
			AnnotationService anserv = AnnotationService.GetService((DependencyObject)node);
			anserv.Disable();
			return null;
		}

		/// <summary>
		/// Post VerifyServiceEnabledTask. Need this method because AnnotationService.Enable
		/// needs more time to complete.  If we just call VerifyServiceEnabledTask, it will
		/// always fail because Service is not yet Ready.
		/// </summary>
		public object QueueVerifyServiceEnabledTask(object node)
		{
			queueTask(new DispatcherOperationCallback(VerifyServiceEnabledTask), node);
			return null;
		}

		/// <summary>
		/// PASS if Service on given node is enabled.
		/// </summary>
		public object VerifyServiceEnabledTask(object node)
		{
			AnnotationService service = AnnotationService.GetService((DependencyObject)node);
			assert("Service not null.", service != null);
			assertEquals("Verify service is enabled.", true, service.IsEnabled);
			passTest("Service is enabled.");
			return null;
		}

		// ----------------------------------------------------------------------------------
		//									  PRIVATE METHODS
		// ----------------------------------------------------------------------------------

		private void AddService(DependencyObject node)
		{
			AnnotationService anserv = new AnnotationService(node);
			annotationStream = new FileStream(_storeFilename, FileMode.Create);
			anserv.Enable(new XmlStreamStore(annotationStream));
		}

		// add using a different store for test that require two possibly valid services
		private void AddSecondService(DependencyObject node)
		{
			AnnotationService anserv = new AnnotationService(node);
			Stream annotationStream = new FileStream(_storeFilename2, FileMode.Create);
			anserv.Enable(new XmlStreamStore(annotationStream));
		}

		private void AssertDisableFails(AnnotationService service, DependencyObject node, Type expectedExceptionType)
		{
			bool exceptionOccured = false;
			try
			{				
				service.Disable();
			}
			catch (Exception e)
			{
				AssertEquals("Verify exception type", expectedExceptionType, e.GetType());
				exceptionOccured = true;
			}

			Assert("Verify Exception occurred.", exceptionOccured);
		}
		private void AssertDisableFails(AnnotationService service, Type expectedExceptionType)
		{
			bool exceptionOccured = false;
			try
			{
				service.Disable();
			}
			catch (Exception e)
			{
				AssertEquals("Verify exception type", expectedExceptionType, e.GetType());
				exceptionOccured = true;
			}

			Assert("Verify Exception occurred.", exceptionOccured);
		}

		private Exception AssertEnableFails(DependencyObject node) 
		{
			try
			{
				AddSecondService(node);
			}
			catch (Exception e)
			{
				return e;
			}

			failTest("Call to AnnotationService.Enable should have thrown an exception.");
			return null;
		}

		private string _storeFilename = "annotations.xml";
		private string _storeFilename2 = "annotations2.xml";
		Stream annotationStream;
	}
}

