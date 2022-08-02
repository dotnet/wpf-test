// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DrtAnnotations
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;    
    using System.Windows.Documents;
    using System.Windows.Annotations;
    using System.Windows.Media;
    using System.Windows.Resources;
    using System.Windows.Markup;
    using System.Windows.Annotations.Storage;


    public class InkEditingModeConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            InkCanvasEditingMode expectedMode = (InkCanvasEditingMode)parameter;
            InkCanvasEditingMode currentMode = (InkCanvasEditingMode)o;

            // If the current EditingMode is the mode which menuitem is expecting, return true for IsChecked.
            if (currentMode == expectedMode)
            {
                return true;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class WidthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)value;
            if (width > 300)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public partial class PageViewerApp
    {
        #region Public Elements

        public DependencyObject TopLevel
        {
            get { return topLevel; }
        }

        public DocumentViewerBase Viewer
        {
            get 
            {
                return (DocumentViewerBase)_viewer;
            }
        }

        #endregion Public Elements

        internal void Init(object sender, EventArgs args)
        {
            FlowDocument1Class doc1 = new FlowDocument1Class();
            doc1.InitializeComponent();
            _flowDocument = (FlowDocument)doc1;
            
            FixedDocument1Class doc2 = new FixedDocument1Class();
            doc2.InitializeComponent();
            _fixedDocument = (FixedDocument)doc2;

            FixedDocumentSequenceClass doc3 = new FixedDocumentSequenceClass();
            doc3.InitializeComponent();
            _fixedDocumentSequence = (FixedDocumentSequence)doc3;
        }


        internal void OnColorChanged(object sender, SelectionChangedEventArgs e)
        {
            Brush brush = null;

            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    if (item.Content.Equals("Pink"))
                    {
                        comboBox.Parent.SetValue(FrameworkElement.DataContextProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FFC0CB")));
                    }
                    else if (item.Content.Equals("Blue"))
                    {
                        brush = Brushes.Blue.CloneCurrentValue();
                        brush.Opacity = 0.2;  
                        comboBox.Parent.SetValue(FrameworkElement.DataContextProperty, brush);
                    }
                    else if (item.Content.Equals("Yellow"))
                    {
                        brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#66FFFF00"));
                        brush.Opacity = 0.5;
                        comboBox.Parent.SetValue(FrameworkElement.DataContextProperty, brush);
                    }
                    else if (item.Content.Equals("Default"))
                    {
                        comboBox.Parent.SetValue(FrameworkElement.DataContextProperty, null);
                    }
                }
            }
        }

        internal void LoadFixedDocument(object sender, System.Windows.RoutedEventArgs args)
        {
            LoadDocument(typeof(DocumentViewer), _fixedDocument);
        }

        internal void LoadFixedDocumentSequence(object sender, System.Windows.RoutedEventArgs args)
        {
            LoadDocument(typeof(DocumentViewer), _fixedDocumentSequence);
        }

        internal void LoadFlowDocumentPageViewer(object sender, System.Windows.RoutedEventArgs args)
        {
            LoadDocument(typeof(FlowDocumentPageViewer), _flowDocument);
        }

        internal void LoadFlowDocumentReader(object sender, System.Windows.RoutedEventArgs args)
        {
            LoadDocument(typeof(FlowDocumentReader), _flowDocument);
        }

        internal void LoadFlowDocumentScrollViewer(object sender, System.Windows.RoutedEventArgs args)
        {
            LoadDocument(typeof(FlowDocumentScrollViewer), _flowDocument);
        }

        internal void ClearContent(object sender, System.Windows.RoutedEventArgs args)
        {
            if (_content != null)
            {
                Cleanup();

                // Clear the content by simply setting it to null.
                SetDocument(null);
                _content = null;
            }
        }

        internal void EnableAnnotations(object sender, System.Windows.RoutedEventArgs args)
        {
            if (!_service.IsEnabled)
            {
                // If previously disabled, the store was closed so we recreate it
                if (_store == null)
                {
                    CreateStoreForContent();
                }
                
                _service.Enable(_store);
            }
        }

        internal void DisableAnnotations(object sender, System.Windows.RoutedEventArgs args)
        {
            if (_service.IsEnabled)
            {
                Cleanup();
            }
        }

        internal bool Cleanup()
        {
            // Save all annotations and close disable the service
            bool wasEnabled = false;
            if (_viewer == null)
            {
                // Enable it by default when its the first viewer
                wasEnabled = true;  
            }
            else
            {
                if (_service.IsEnabled)
                {
                    _service.Disable();
                    wasEnabled = true;
                }
                if (_store != null)
                {
                    _store.Flush();
                    _store.Dispose();
                    _stream.Close();
                    _store = null;
                    _stream = null;
                }
            }
            return wasEnabled;
        }

        internal void CreateHighlight(object sender, System.Windows.RoutedEventArgs args)
        {
            AnnotationHelper.CreateHighlightForSelection(_service, null, null);
        }

        internal void DeleteHighlight(object sender, System.Windows.RoutedEventArgs args)
        {
            AnnotationHelper.ClearHighlightsForSelection(_service);
        }

        internal void CreateTextStickyNote(object sender, System.Windows.RoutedEventArgs args)
        {
            AnnotationHelper.CreateTextStickyNoteForSelection(_service, null);
        }

        internal void DeleteStickyNotes(object sender, System.Windows.RoutedEventArgs args)
        {
            AnnotationHelper.DeleteTextStickyNotesForSelection(_service);
            AnnotationHelper.DeleteInkStickyNotesForSelection(_service);
        }

        internal void CreateInkStickyNote(object sender, System.Windows.RoutedEventArgs args)
        {
            AnnotationHelper.CreateInkStickyNoteForSelection(_service, null);
        }

        internal void DeleteAllAnnotations(object sender, System.Windows.RoutedEventArgs args)
        {
            AnnotationHelper.ClearHighlightsForSelection(_service);
            AnnotationHelper.DeleteTextStickyNotesForSelection(_service);
            AnnotationHelper.DeleteInkStickyNotesForSelection(_service);
        }

        internal void CreateService()
        {
            if (_viewer is DocumentViewerBase)
            {
                _service = new AnnotationService((DocumentViewerBase)_viewer);
            }
            else if (_viewer is FlowDocumentScrollViewer)
            {
                _service = new AnnotationService((FlowDocumentScrollViewer)_viewer);
            }
            else if (_viewer is FlowDocumentReader)
            {
                _service = new AnnotationService((FlowDocumentReader)_viewer);
            }
        }

        internal void SetDocument(IDocumentPaginatorSource source)
        {
            if (_viewer is DocumentViewerBase)
            {
                ((DocumentViewerBase)_viewer).Document = source;
            }
            else if (_viewer is FlowDocumentScrollViewer)
            {
                ((FlowDocumentScrollViewer)_viewer).Document = (FlowDocument)source;
            }
            else if (_viewer is FlowDocumentReader)
            {
                ((FlowDocumentReader)_viewer).Document = (FlowDocument)source;
            }
        }

        internal void LoadDocument(Type viewerType, IDocumentPaginatorSource document)
        {
            if (_content != document || (_viewer == null || !(_viewer.GetType().Equals(viewerType))))
            {
                bool reEnable = Cleanup();

                _content = document;

                // Enable annotations for the new content
                CreateStoreForContent();

                if (_viewer == null || !(_viewer.GetType().Equals(viewerType)))
                {
                    SetDocument(null);
                    _viewer = (FrameworkElement)Activator.CreateInstance(viewerType);
                    Holder.Child = _viewer;
                    CreateService();    
                    reEnable = true;
                }

                // Set content on viewer
                SetDocument(_content);

                DocumentViewer docViewer = _viewer as DocumentViewer;
                if (docViewer != null)
                    docViewer.FitToHeight();

                // Reenable service if necessary
                if (reEnable)
                {
                    _service.Enable(_store);
                }
            }
        }

        internal XmlStreamStore CreateStoreForContent()
        {
            if (!Directory.Exists(DrtAnnotationsDirectory))
            { 
                Directory.CreateDirectory(DrtAnnotationsDirectory);
            }

            if (_content is FlowDocument)
            {
                _stream = new FileStream(FlowContent1Store, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            else if (_content is FixedDocument)
            {
                _stream = new FileStream(FixedContent1Store, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            else if (_content is FixedDocumentSequence)
            {
                _stream = new FileStream(FixedDocSequenceStore, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            _store = new XmlStreamStore(_stream);
            return _store;
        }

        internal void ResetAll(object sender, System.Windows.RoutedEventArgs args)
        {
            bool wasEnabled = false;

            if (_stream != null)
            {
                string name = _stream.Name;
                wasEnabled = Cleanup();
                File.Delete(name);
            }

            // Recreate the store for the same content
            CreateStoreForContent();

            if (wasEnabled)
                _service.Enable(_store);
        }

        internal void DeleteStore()
        {
            File.Delete(FixedContent1Store);
        }

        internal bool Interactive
        {
            set
            {
                _interactive = value;
            }
        }

        protected override void OnClosing(CancelEventArgs args)
        {
            Cleanup();
            if (_interactive)
                Environment.Exit(0);
        }

        private FlowDocument          _flowDocument;
        private FixedDocument         _fixedDocument;
        private FixedDocumentSequence _fixedDocumentSequence;
        private FrameworkElement _viewer;
        private bool _interactive = false;
        private FileStream _stream = null;
        private XmlStreamStore _store = null;
        private AnnotationService _service = null;
        private IDocumentPaginatorSource _content = null;

        public static string DrtAnnotationsDirectory = @"DrtFiles\\DrtAnnotations";
        public static string FixedDocSequenceStore = @"DrtFiles\\DrtAnnotations\drtannotations_fixeddocsequence_store.xml"; 
        public static string FixedContent1Store = @"DrtFiles\\DrtAnnotations\drtannotations_fixedcontent1_store.xml";
        public static string FlowContent1Store = @"DrtFiles\\DrtAnnotations\drtannotations_flowcontent1_store.xml";
    }
}

