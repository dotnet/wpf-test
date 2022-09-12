// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: High-level end-to-end test for annotations.  It creates an 
//              app that contains different kinds of content and performs
//              various operations on the content (create/delete/load/unload
//              annotations). 
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Xml;

using DRT;

namespace DrtAnnotations
{
    public sealed class EndToEndTestSuite : DrtTestSuite
    {
        public EndToEndTestSuite() : base("EndToEnd")
        {
            TeamContact = "WPF";
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            _window = new SampleApp();
            _window.Visibility = Visibility.Visible;

            // Cleanup from previous run of this test
            FileInfo fi = new FileInfo(FileName);
            fi.Delete();

            // Replace the default store with one specifically for this suite
            XmlFileStore store = new XmlFileStore(FileName);
            store.AutoFlush = true;
            AnnotationService.SetStore(_window.TopLevel, store);

            _window.Service.AttachedAnnotationChanged += OnAttachedAnnotationChanged;

            // Return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest( StartCreateAnnotations ),
                        (DrtTest) delegate() { CreateAnnotationsForAnchor(_window.C); },
                        (DrtTest) delegate() { CreateAnnotationsForAnchor(_window.E); },
                        (DrtTest) delegate() { CreateAnnotationsForAnchor(_window.F); },
                        (DrtTest) delegate() { CreateAnnotationsForAnchor(_window.G); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.RichTextBox.Start, _window.RichTextBox.End, _window.RichTextBox.TextSelection,  0,   0,  0); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.RichTextBox.Start, _window.RichTextBox.End, _window.RichTextBox.TextSelection, 30, -30,  0); },
                        //(DrtTest) delegate() { CreateAnnotationTextSelection(_window.RichTextBox.Start, _window.RichTextBox.End, _window.RichTextBox.TextSelection,  0,   1, -1); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.RichTextBox.Start, _window.RichTextBox.End, _window.RichTextBox.TextSelection, -1,   0,  1); },
                        //(DrtTest) delegate() { CreateAnnotationTextSelection(_window.RichTextBox.Start, _window.RichTextBox.End, _window.RichTextBox.TextSelection,  0,   0, -1); },
                        //(DrtTest) delegate() { CreateAnnotationTextSelection(_window.RichTextBox.Start, _window.RichTextBox.End, _window.RichTextBox.TextSelection,  0,   0,  1); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.TextBox.StartPosition, _window.TextBox.EndPosition, _window.TextBox.Selection,  0,   0,  0); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.TextBox.StartPosition, _window.TextBox.EndPosition, _window.TextBox.Selection, 30, -30,  0); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.TextBox.StartPosition, _window.TextBox.EndPosition, _window.TextBox.Selection,  0,   1, -1); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.TextBox.StartPosition, _window.TextBox.EndPosition, _window.TextBox.Selection, -1,   0,  1); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.TextBox.StartPosition, _window.TextBox.EndPosition, _window.TextBox.Selection,  0,   0, -1); },
                        (DrtTest) delegate() { CreateAnnotationTextSelection(_window.TextBox.StartPosition, _window.TextBox.EndPosition, _window.TextBox.Selection,  0,   0,  1); },
                        new DrtTest( EndCreateAnnotations ),
                        new DrtTest( UnloadAnnotations ),
                        new DrtTest( ReloadAnnotations ),                        
                        new DrtTest( ModifyAnchors ),
                        new DrtTest( DeleteAnnotations ),
                        };
        }           

        private void StartCreateAnnotations()
        {
            Console.Write("Creating Annotations....");

            ClearEventCounters();

            _createdAnnotationCount = 0;
        }

        private void EndCreateAnnotations()
        {
            AnnotationService svc = _window.Service;

            DRT.Assert(svc.AttachedAnnotations.Count == _createdAnnotationCount,
                "FAILED: AnnotationService has " + svc.AttachedAnnotations.Count + " attached annotations, not " + _createdAnnotationCount + ".");

            CheckEventCounters(_createdAnnotationCount, 0, 0);

            Console.WriteLine("done.");
        }

        private void UnloadAnnotations()
        {
            Console.Write("Unloading Annotations....");
          
            ClearEventCounters();

            AnnotationService svc = _window.Service; 
            
            svc.UnloadAnnotations(_window.TopLevel);
            DRT.Assert(svc.AttachedAnnotations.Count == 0,
                "AnnotationService failed to unload all annotations. Count is " + svc.AttachedAnnotations.Count + ".");

            CheckEventCounters(0, _createdAnnotationCount, 0);
            
            Console.WriteLine("done.");
        }

        private void ReloadAnnotations()
        {
            Console.Write("Reloading Annotations....");

            ClearEventCounters();

            AnnotationService svc = _window.Service;
   
            svc.LoadAnnotations(_window.TopLevel);

            if (svc.AttachedAnnotations.Count != _createdAnnotationCount)
            {
                Console.WriteLine("Reloading Annotations Failed.  AnnotationService has " + svc.AttachedAnnotations.Count + " annotations, not " + _createdAnnotationCount + ".");

                // Find which annotation wasn't reloaded
                foreach (Annotation annotation in _annotations)
                {
                    bool found = false;
                    foreach (IAttachedAnnotation attachedAnn in svc.AttachedAnnotations)
                    {
                        if (attachedAnn.Annotation == annotation)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        DRT.Assert(annotation != null,
                            "  Annotation {"+ annotation.Id +"} not reloaded.");
                    }
                }
            }

            CheckEventCounters(_createdAnnotationCount, 0, 0);

            Console.WriteLine("done.");
        }

        private void DeleteAnnotations()
        {
            Console.Write("Deleting annotations...");

            ClearEventCounters();

            AnnotationService svc = _window.Service;
            int initialCount = svc.AttachedAnnotations.Count;
            int deletedCount = _annotations.Count;

            foreach (Annotation annotation in _annotations)
            {
                svc.Store.DeleteAnnotation(annotation.Id);
            }            

            DRT.Assert(svc.AttachedAnnotations.Count == initialCount - deletedCount,
                "Deleting Annotations Failed.  AnnotationService has " + svc.AttachedAnnotations.Count + " annotations, not " + (initialCount - deletedCount) + ".");

            CheckEventCounters(0, deletedCount, 0);

            Console.WriteLine("done.");
        }

        private void ModifyAnchors()
        {
            Console.Write("Modifying anchors...");
         
            ClearEventCounters();

            AnnotationService svc = _window.Service;
            
            // Create an anchor for a tree node
            Resource newAnchor = new Resource("secondAnchor");
            IList<Locator> locators = svc.LocatorManager.GenerateLocators(_window.F);
            newAnchor.AddLocators(locators);            

            // Get an annotation and one of its existing anchors
            DRT.Assert(svc.AttachedAnnotations.Count > 1, "Problem with test.  Should not be modifying anchors with zero attached anchors loaded.");
            Annotation annotation = svc.AttachedAnnotations[0].Annotation;
            IEnumerator<Resource> resources = (IEnumerator<Resource>)annotation.Anchors.GetEnumerator();
            resources.MoveNext();
            Resource existingAnchor = resources.Current;

            // Add an anchor to an existing annotation - should get Loaded
            annotation.AddAnchor(newAnchor);
            CheckEventCounters(1, 0, 0);

            // Remove the same anchor from the annotation - should get an Unloaded
            annotation.RemoveAnchor(newAnchor);
            CheckEventCounters(1,1,0);

            // Modify an existing anchor from the annotation - should get a Modified
            existingAnchor.CopyFrom(newAnchor);
            CheckEventCounters(1,1,1);

            // Create two XmlElement contents
            XmlDocument doc = new XmlDocument();
            XmlElement contentPartOfAnchor = doc.CreateElement("TestContentType", "TestContentNS");
            XmlElement unassociatedContent = doc.CreateElement("TestContentType", "TestContentNS");

            // Add a content to the existing anchor - should get a Modified
            existingAnchor.AddContent(contentPartOfAnchor);
            CheckEventCounters(1,1,2);

            // Modify a content that's not part of the annotation - should get no events
            unassociatedContent.SetAttribute("TestAttr", "TestValue");
            CheckEventCounters(1,1,2);

            // Modify a content that is part of the existing anchor - should get a Modified
            contentPartOfAnchor.SetAttribute("TestAttr", "TestValue");
            CheckEventCounters(1,1,3);
            
            // Modify the anchor to have only unresolvable locators - should get an Unloaded
            locators = svc.LocatorManager.GenerateLocators(_window.Unresolvable);
            existingAnchor.ReplaceAllLocators(locators);
            CheckEventCounters(1,2,3);

            _annotations.Remove(annotation);

            Console.WriteLine("done.");
        }

        #region Event Counting

        private void OnAttachedAnnotationChanged(object sender, AttachedAnnotationChangedEventArgs args)
        {
            if (args.Action == AttachedAnnotationAction.Loaded)
            {
                _loadedCount++;
            }
            else if (args.Action == AttachedAnnotationAction.Unloaded)
            {
                _unloadedCount++;
            }
            else if (args.Action == AttachedAnnotationAction.AnchorModified)
            {
                _modifiedCount++;
            }
        }

        private void ClearEventCounters()
        {
            _loadedCount = 0;
            _unloadedCount = 0;
            _modifiedCount = 0;
        }

        private void CheckEventCounters(int loaded, int unloaded, int modified)
        {
            DRT.Assert(_loadedCount == loaded, "Loaded events not fired correctly.  Expected " + loaded + " but received " + _loadedCount + ".");
            DRT.Assert(_unloadedCount == unloaded, "Unloaded events not fired correctly.  Expected " + unloaded + " but received " + _unloadedCount + ".");
            DRT.Assert(_modifiedCount == modified, "Modified events not fired correctly.  Expected " + modified + " but received " + _modifiedCount + ".");
        }

        #endregion Event Counting

        private Annotation CreateAnnotationsForAnchor(object anchor)
        {
			AnnotationStore store = _window.Service.Store;
            DRT.Assert(store != null);

            Resource context = new Resource();
            IList<Locator> locators = _window.Service.LocatorManager.GenerateLocators(anchor);
            context.AddLocators(locators);

            Annotation annotation = new Annotation("marker", "testNS");
            annotation.AddAnchor(context);
            store.AddAnnotation(annotation);
            _annotations.Add(annotation);
            _createdAnnotationCount++;

            CheckEventCounters(_createdAnnotationCount, 0, 0);

            return annotation;
        }

        private Annotation CreateAnnotationTextSelection(TextPointer containerStart, TextPointer containerEnd, TextSelection selection, int start, int end, int initialPosition)
        {
            // This method could be more efficient by simply creating the
            // anchor TextRange instead of moving the selection for each
            // annotations but in real life - the user would be moving the
            // selection.  Trying to keep it as close to real as possible.
            TextPointer startNav = null;
            TextPointer endNav = null;

            switch (initialPosition)
            {
                case 0:
                    startNav = containerStart;
                    endNav = containerEnd;
                    break;
                case 1:
                    // start both at the end (positive means forward)
                    startNav = containerEnd;
                    endNav = containerEnd;
                    break;
                case -1:
                    // start both at the beginning (negative meands backwards)
                    startNav = containerStart;
                    endNav = containerStart;
                    break;
            }

            startNav = startNav.GetPositionAtOffset(start);
            endNav = endNav.GetPositionAtOffset(end);
            selection.Select(startNav, endNav);
            return CreateAnnotationsForAnchor(new TextRange(selection.Start, selection.End));
        }

        #region Private Fields

        private SampleApp _window;
        private int _createdAnnotationCount = 0;
        private int _loadedCount = 0;
        private int _unloadedCount = 0;
        private int _modifiedCount = 0;
        private List<Annotation> _annotations = new List<Annotation>(3);

        private const string FileName = "DrtFiles\\DrtAnnotationsData.xml";

        #endregion Private Fields
    }
}
