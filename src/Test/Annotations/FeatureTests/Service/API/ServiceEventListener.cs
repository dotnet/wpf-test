// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Class that will catch all exceptions from an AnnotationService
//				 and keep statistics about these events.  Also provides an
//				 api for checking event information against expected values.

using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations
{
	public class ServiceEventListener
	{
		AnnotationService service;
		TestSuite suite;

		public int nLoadEvents = 0;
		public int nUnloadEvents = 0;
		public int nAnchorModifiedEvents = 0;

		public IAttachedAnnotation lastAttachedAnnotation = null;
		public object lastPreviousAttachedAnchor = null;
		public AttachmentLevel lastPreviousAttachmentLevel = AttachmentLevel.Unresolved;

		public ServiceEventListener(TestSuite aSuite, AnnotationService aService)
		{
			suite = aSuite;
			service = aService;
			service.AttachedAnnotationChanged += new AttachedAnnotationChangedEventHandler(HandleAttachedAnnotationChanged);
		}

		public void HandleAttachedAnnotationChanged(object sender, AttachedAnnotationChangedEventArgs args)
		{
			lastAttachedAnnotation = args.AttachedAnnotation;
			lastPreviousAttachedAnchor = args.PreviousAttachedAnchor;
			lastPreviousAttachmentLevel = args.PreviousAttachmentLevel;

			switch(args.Action) {
				case (AttachedAnnotationAction.Loaded):
				case (AttachedAnnotationAction.Added):
					suite.printStatus("Event Received: Loaded");
					nLoadEvents++;
					break;
				case (AttachedAnnotationAction.Unloaded):
				case (AttachedAnnotationAction.Deleted):
					suite.printStatus("Event Received: Unloaded");
					nUnloadEvents++;
					break;

				case (AttachedAnnotationAction.AnchorModified):
					suite.printStatus("Event Received: AnchorModified");
					nAnchorModifiedEvents++;
					break;

				default:
					suite.failTest("Unknown AttachedAnnotationAction '" + args.Action + "'.");
					break;
			}
		}

		public void VerifyLoadEventCount(int count)
		{
			suite.assertEquals("Verify number of 'Load' events.", count, nLoadEvents);
			suite.printStatus("Verified number of 'Load' events: " + nLoadEvents);
		}

		public void VerifyUnloadEventCount(int count)
		{
			suite.assertEquals("Verify number of 'Unload' events.", count, nUnloadEvents);
			suite.printStatus("Verified number of 'Unload' events: " + nUnloadEvents);
		}

		public void VerifyAnchorModifiedEventCount(int count)
		{
			suite.assertEquals("Verify number of 'AnchorModified' events.", count, nAnchorModifiedEvents);
			suite.printStatus("Verified number of 'AnchorModified' events: " + nAnchorModifiedEvents);
		}

		public void VerifyEventCounts(int nExpectedLoads, int nExpectedUnloads, int nExpectedAnchorModifieds)
		{
			if (nExpectedLoads == nLoadEvents &&
				nExpectedUnloads == nUnloadEvents &&
				nExpectedAnchorModifieds == nAnchorModifiedEvents)
			{
				suite.printStatus("Verified Load, Unload, and AnchorModified event counts (" + nLoadEvents + ", " + nUnloadEvents + ", " + nExpectedAnchorModifieds + ").");
			}
			else
			{
				suite.failTest("Unexpected number of Load, Unload, or AnchorModified events. Expected (" 
								+ nExpectedLoads 
								+ ", " 
								+ nExpectedUnloads 
								+ ", " 
								+ nExpectedAnchorModifieds 
								+ ") but was (" 
								+ nLoadEvents 
								+ ", " 
								+ nUnloadEvents 
								+ ", " 
								+ nAnchorModifiedEvents 
								+ ").");
			}
		}

		public void VerifyLastAttachedAnnotationIdentity(IAttachedAnnotation expectedAA)
		{
			suite.assert("Verify identity of last AttachedAnnotation.", expectedAA == lastAttachedAnnotation);
			suite.printStatus("Verified last AttachedAnnotation identity.");
		}

		public void VerifyLastPreviousAttachedAnchorIdentity(object expectedAttachedAnchor)
		{
			suite.assert("Verify identity of last PreviousAttachedAnchor.", expectedAttachedAnchor == lastPreviousAttachedAnchor);
			suite.printStatus("Verified last PreviousAttachedAnchor identity.");
		}

		public void VerifyLastPreviousAttachmentLevel(AttachmentLevel expectedAttachmentLevel)
		{
			suite.assertEquals("Verify last PreviousAttachmentLevel.", expectedAttachmentLevel, lastPreviousAttachmentLevel);
			suite.printStatus("Verified last PreviousAttachmentLevel.");
		}

		/// <summary>
		/// Reset all eventing vars.
		/// </summary>
		public void Reset()
		{
			nLoadEvents = 0;
			nUnloadEvents = 0;
			nAnchorModifiedEvents = 0;

			lastAttachedAnnotation = null;
			lastPreviousAttachedAnchor = null;
			lastPreviousAttachmentLevel = AttachmentLevel.Unresolved;

			suite.printStatus("EventListener Reset.");
		}
	}
}

