// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DrtAnnotations
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Annotations;
    using System.Windows.Annotations.Component;
    using System.Windows.Annotations.Storage;
    

    public partial class SampleApp
    {
        #region Public Elements

        public DependencyObject A
        {
            get { return a; }
        }

        public DependencyObject B
        {
            get { return b; }
        }

        public DependencyObject C
        {
            get { return c; }
        }

        public DependencyObject D
        {
            get { return d; }
        }

        public DependencyObject E
        {
            get { return e; }
        }
        
        public DependencyObject F
        {
            get { return f; }
        }

        public DependencyObject G
        {
            get { return g; }
        }

        public DependencyObject Unresolvable
        {
            get { return unresolvable; }
        }

        public DependencyObject TopLevel
        {
            get { return topLevel; }
        }

        public TextBox TextBox
        {
            get { return textBox; }
        }

        public RichTextBox RichTextBox
        {
            get { return richTextBox; }
        }

        public AnnotationService Service
        {
            get { return AnnotationService.GetService(topLevel); }
        }

        #endregion Public Elements

        private void Init(object sender, EventArgs args)
        {
            AnnotationService.Enable(topLevel);
            AnnotationService.SetChooser(topLevel, new SampleComponentChooser());
        }

        /// <summary>
        /// Creates an annotation on a given anchor.  This method (or one like it) will eventually 
        /// be made a helper API on the AnnotationService.
        /// </summary>
        /// <param name="anchor">the object the annotation will be anchored to</param>
        private void CreateAnnotationsForAnchor(object anchor)
        {
            Resource context = new Resource();
            IList<Locator> locators = Service.LocatorManager.GenerateLocators(anchor);
            context.AddLocators(locators);

            Annotation annotation = new Annotation("marker", "testNS");
            annotation.AddAnchor(context);
            Service.Store.AddAnnotation(annotation);
        }

        private void CreateMarker(object sender, System.Windows.RoutedEventArgs args)
        {
            CreateAnnotationsForAnchor(sender);
        }

        private void CreateMarkerOnSelection(object sender, System.Windows.RoutedEventArgs args)
        {
            CreateAnnotationsForAnchor(new TextRange(textBox.Selection.Start, textBox.Selection.End));
        }

        private void CreateMarkerOnSelectionForTextPanel(object sender, System.Windows.RoutedEventArgs args)
        {
            TextSelection selection = richTextBox.TextSelection;
            CreateAnnotationsForAnchor(new TextRange(selection.Start, selection.End));
        }

        private void ClearAllAnnotations(object sender, System.Windows.RoutedEventArgs args)
        {            
            AnnotationStore store = Service.Store;

            IList<Annotation> annotations = store.GetAnnotations();

            foreach (Annotation a in annotations)
            {                
                store.DeleteAnnotation(a.Id); 
            }
        }

        private void UnloadAllAnnotations(object sender, System.Windows.RoutedEventArgs args)
        {
            Service.UnloadAnnotations(topLevel);
        }

        private void ReloadAllAnnotations(object sender, System.Windows.RoutedEventArgs args)
        {
            Service.LoadAnnotations(topLevel);
        }
    }
}

