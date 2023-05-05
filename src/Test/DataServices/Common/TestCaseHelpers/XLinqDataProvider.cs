// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description: Implementation of XLinqDataProvider object.
//              This is a code drop from Sam Bent 'as-is' and therefore
//              does not match the Wpf Test Style Guidelines. Changing style
//              would be additionally expensive for future drops. This
//              decision was reached in consultation with Patrick Danino.
//

using System;
using System.IO;                    // Stream
using System.Collections;
using System.Collections.Generic;   // IList<T>
using System.Collections.ObjectModel;   // ObservableCollection<T>
using System.ComponentModel;        // ISupportInitialize, AsyncCompletedEventHandler, [DesignerSerialization*], [DefaultValue]
using System.Diagnostics;
using System.IO.Packaging;          // PackUriHelper
using System.Globalization;         // CultureInfo
using System.Net;                   // WebRequest, IWebRequestCreate
using System.Threading;             // ThreadPool, WaitCallback
using System.Xml.Serialization;     // IXmlSerializable
using System.Xml.Schema;
using System.Xml;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;     // Dispatcher*
using System.Windows.Markup;        // IUriContext, [XamlDesignerSerializer]
using System.Windows.Navigation;    // BaseUriHelper
using System.Xml.Linq;

namespace Codeplex
{
    /// <summary>
    /// XLinqDataProvider class, gets XmlNodes to use as source in data binding
    /// </summary>
    /// <ExternalAPI/>
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    [ContentProperty("XmlSerializer")]
    public class XLinqDataProvider : DataSourceProvider, IUriContext
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        /// <summary>
        /// Instantiates a new instance of a XLinqDataProvider
        /// </summary>
        public XLinqDataProvider()
        {
        }

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// Source property, indicated the Uri of the source Xml data file, if this
        /// property is set, then any inline xml data is discarded.
        /// </summary>
        /// <remarks>
        /// Setting this property will implicitly cause this DataProvider to refresh.
        /// When changing multiple refresh-causing properties, the use of
        /// <seealso cref="DeferRefresh"/> is recommended.
        /// </remarks>
        public Uri Source
        {
            get { return _source; }
            set
            {
                if ((_activeDocument != null) || _source != value)
                {
                    _activeDocument = null;
                    _source = value;

                    if (!IsRefreshDeferred)
                        Refresh();
                }
            }
        }

        /// <summary>
        /// This method is used by TypeDescriptor to determine if this property should
        /// be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeSource()
        {
            return (_document == null) && (_source != null);
        }

        /// <summary>
        /// Document Property, returns the current XDocument that this data source
        /// is using, if set the Source property is cleared and any inline xml data is
        /// discarded
        /// </summary>
        /// <remarks>
        /// Setting this property will implicitly cause this DataProvider to refresh.
        /// When changing multiple refresh-causing properties, the use of
        /// <seealso cref="DeferRefresh"/> is recommended.
        /// </remarks>
        // this property cannot be serialized since the produced XAML/XML wouldn't be parseable anymore;
        // instead, a user-set DOM is serialized as InlineData
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XDocument Document
        {
            get { return _activeDocument; }
            set
            {
                _document = value;

                if (_source != null || _activeDocument != value)
                {
                    _source = null;
                    ChangeActiveDocument(value);

                    if (!IsRefreshDeferred)
                        Refresh();
                }
            }
        }

        /// <summary>
        /// XPath property, the XPath query used for generating the DataCollection
        /// </summary>
        /// <remarks>
        /// Setting this property will implicitly cause this DataProvider to refresh.
        /// When changing multiple refresh-causing properties, the use of
        /// <seealso cref="DeferRefresh"/> is recommended.
        /// </remarks>
        [DesignerSerializationOptions(DesignerSerializationOptions.SerializeAsAttribute)]
        public string XPath
        {
            get { return _xPath; }
            set
            {
                if (_xPath != value)
                {
                    _xPath = value;

                    if (!IsRefreshDeferred)
                        Refresh();
                }
            }
        }

        /// <summary>
        /// This method is used by TypeDescriptor to determine if this property should
        /// be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeXPath()
        {
            return (_xPath != null) && (_xPath.Length != 0);
        }

        /// <summary>
        /// If true object creation will be performed in a worker
        /// thread, otherwise will be done in active context.
        /// </summary>
        [DefaultValue(true)]
        public bool IsAsynchronous
        {
            get { return _isAsynchronous; }
            set { _isAsynchronous = value; }
        }

        /// <summary>
        /// The content property for inline Xml data.
        /// Used by the parser to compile the literal content of the embedded XML island.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IXmlSerializable XmlSerializer
        {
            get
            {
                if (_xmlSerializer == null)
                {
                    _xmlSerializer = new XmlIslandSerializer(this);
                }
                return _xmlSerializer;
            }
        }

        /// <summary>
        /// This method is used by TypeDescriptor to determine if this property should
        /// be serialized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeXmlSerializer()
        {
            // serialize InlineData only if the Xml DOM used was originally a inline Xml data island
            return (DocumentForSerialization != null);
        }

        #endregion


        #region IUriContext

        /// <summary>
        ///     Provides the base uri of the current context.
        /// </summary>
        Uri IUriContext.BaseUri
        {
            get { return BaseUri; }
            set { BaseUri = value; }
        }

        /// <summary>
        ///     Implementation for BaseUri.
        /// </summary>
        protected virtual Uri BaseUri
        {
            get { return _baseUri; }
            set { _baseUri = value; }
        }

        #endregion IUriContext

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        /// <summary>
        /// Prepare loading either the external XML or inline XML and
        /// produce Xml node collection.
        /// Execution is either immediately or on a background thread, see IsAsynchronous.
        /// Called by base class from InitialLoad or Refresh
        /// </summary>
        protected override void BeginQuery()
        {
            if (_source != null)
            {
                // load DOM from external source
                Debug.Assert(_document == null, "Should not be possible to be using Source and user-set Doc at the same time.");
                DiscardInline();
                LoadFromSource(); // will execute synch or asycnh, depends on IsAsynchronous
            }
            else
            {
                XDocument doc = null;
                if (_document != null)
                {
                    DiscardInline();
                    doc = _document;
                }
                else // inline doc
                {
                    // Did we already parse the inline DOM?
                    // Don't do this during EndInit - it duplicates effort of Parse
                    if (_inEndInit)
                        return;

                    doc = _savedDocument;
                }

                // Doesn't matter if the doc is set programmatically or from inline,
                // here we create a new collection for it and make it active.
                if (IsAsynchronous && doc != null)
                {
                    // process node collection on a worker thread ?
                    ThreadPool.QueueUserWorkItem(new WaitCallback(BuildNodeCollectionAsynch),
                                                 doc);
                }
                else if (doc != null || Data != null)
                {
                    // process the doc synchronously if we're in synchronous mode,
                    // or if the doc is empty.  But don't process an empty doc
                    // if the data is already null, to avoid unnecessary work
                    BuildNodeCollection(doc);
                }
            }
        }


        /// <summary>
        ///     Initialization of this element has completed;
        ///     this causes a Refresh if no other deferred refresh is outstanding
        /// </summary>
        protected override void EndInit()
        {
            // inhibit re-parsing of inline doc (from BeginQuery)
            try
            {
                _inEndInit = true;
                base.EndInit();
            }
            finally
            {
                _inEndInit = false;
            }
        }


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Preparing Document

        // Load XML from a URI; this runs on caller thread of BeginQuery (Refresh/InitialLoad)
        private void LoadFromSource()
        {
            // convert the Source into an absolute URI
            Uri sourceUri = this.Source;
            if (!sourceUri.IsAbsoluteUri)
            {
                Uri baseUri = (_baseUri != null) ? _baseUri : BaseUriHelper.GetBaseUri(_dummyDO);
                sourceUri = new Uri(baseUri, sourceUri);
            }

            // create a request to load the content
            WebRequest request;
            if (sourceUri.Scheme == "pack")
            {
                IWebRequestCreate factory = (IWebRequestCreate)new PackWebRequestFactory();
                request = factory.Create(sourceUri);
            }
            else
            {
                request = WebRequest.Create(sourceUri);
            }
            if (request == null)
            {
                throw new Exception("could not create WebRequest");
            }

            // load it on a worker thread ?
            if (IsAsynchronous)
                ThreadPool.QueueUserWorkItem(new WaitCallback(CreateDocFromExternalSourceAsynch),
                                             request);
            else
                CreateDocFromExternalSource(request);

        }

        #region Content Parsing implementation

        private class XmlIslandSerializer : IXmlSerializable
        {
            internal XmlIslandSerializer(XLinqDataProvider host)
            {
                _host = host;
            }
            public XmlSchema GetSchema()
            {
                // 
                return null;
            }

            public void WriteXml(XmlWriter writer)
            {
                XDocument doc = _host.DocumentForSerialization;
                if (doc != null)
                    doc.Save(writer);
            }

            public void ReadXml(XmlReader reader)
            {
                _host.ParseInline(reader);
            }

            private XLinqDataProvider _host;
        }

        /// <summary>
        /// Parse method,
        /// </summary>
        /// <param name="xmlReader"></param>
        private void ParseInline(XmlReader xmlReader)
        {
            if ((_source == null) && (_document == null) && _tryInlineDoc)
            {
                // load it on a worker thread ?
                if (IsAsynchronous)
                {
                    _waitForInlineDoc = new ManualResetEvent(false); // tells serializer to wait until _activeDocument is ready
                    ThreadPool.QueueUserWorkItem(new WaitCallback(CreateDocFromInlineXmlAsync),
                                                     xmlReader);
                }
                else
                    CreateDocFromInlineXml(xmlReader);
            }
        }

        private XDocument DocumentForSerialization
        {
            get
            {
                // allow inline serialization only if the original XML data was inline
                // or the user has assigned to our Document property
                if (_tryInlineDoc || (_savedDocument != null) || (_document != null))
                {
                    // if inline or assigned doc hasn't been parsed yet, wait for it
                    if (_waitForInlineDoc != null)
                        _waitForInlineDoc.WaitOne();
                    return _activeDocument;
                }
                return null;
            }
        }

        #endregion //Content Parsing implementation

        // this method can run on a worker thread!
        private void CreateDocFromInlineXmlAsync(object arg)
        {
            XmlReader xmlReader = (XmlReader)arg;
            CreateDocFromInlineXml(xmlReader);
        }

        // this method can run on a worker thread!
        private void CreateDocFromInlineXml(XmlReader xmlReader)
        {
            // Maybe things have changed and we don't want to use inline doc anymore
            if (!_tryInlineDoc)
            {
                _savedDocument = null;
                if (_waitForInlineDoc != null)
                    _waitForInlineDoc.Set();
                return;
            }

            XDocument doc = null;
            Exception ex = null;

            try
            {
                try
                {
                    // Load the inline doc from the reader
                    doc = XDocument.Load(xmlReader);
                }
                catch (Exception xmle)
                {
                    ex = xmle;
                }

                if (ex == null)
                {
                    // Save a copy of the inline document to be used in future
                    // queries, and by serialization.
                    _savedDocument = new XDocument(doc);
                }
            }
            finally
            {
                xmlReader.Close();
                // Whether or not parsing was successful, unblock the serializer thread.

                // If serializer had to wait for the inline doc, it's available now.
                // If there was an error, null will be returned for DocumentForSerialization.
                if (_waitForInlineDoc != null)
                    _waitForInlineDoc.Set();
            }

            if (ex == null)
            {
                // Load succeeded.  Create the node collection.  (This calls
                // OnQueryFinished to reset the Document and Data properties).
                BuildNodeCollection(doc);
            }
            else
            {
                // Load failed.  Report the error, and reset
                // Data and Document properties to null.
                OnQueryFinished(null, ex, CompletedCallback, null);
            }
        }

        // this method can run on a worker thread!
        private void CreateDocFromExternalSourceAsynch(object arg)
        {
            WebRequest request = (WebRequest)arg;
            CreateDocFromExternalSource(request);
        }

        // this method can run on a worker thread!
        private void CreateDocFromExternalSource(WebRequest request)
        {
            XDocument doc = new XDocument();
            Exception ex = null;
            // request the content from the URI
            try
            {
                WebResponse response = request.GetResponse();
                if (response == null)
                {
                    throw new InvalidOperationException("WebRequest returned null response");
                }

                // Get Stream and content type from WebResponse.
                Stream stream = response.GetResponseStream();

                // load the XML from the stream
                doc = XDocument.Load(new XmlTextReader(stream));
                stream.Close();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e))
                {
                    throw;
                }
                ex = e;
            }

            if (ex != null)
            {
                // we're done if we got an error up to this point
                // both .Data and .Document properties will be reset to null
                OnQueryFinished(null, ex, CompletedCallback, null);
                return;  // have an error, no processing of DOM
            }

            BuildNodeCollection(doc);
            // above method also calls OnQueryFinished to push new property values
        }


        // this method can run on a worker thread!
        private void BuildNodeCollectionAsynch(object arg)
        {
            XDocument doc = (XDocument)arg;
            BuildNodeCollection(doc);
        }


        // this method can run on a worker thread!
        private void BuildNodeCollection(XDocument doc)
        {
            IEnumerable collection = null;
            if (doc != null)
            {
                collection = GetResultNodeList(doc);
            }

            OnQueryFinished(collection, null, CompletedCallback, doc);
        }

        // this callback will execute on the UI thread;
        // OnQueryFinished marshals back to UI thread if necessary
        private object OnCompletedCallback(object arg)
        {
            ChangeActiveDocument((XDocument)arg);
            return null;
        }

        // change Document property, and update event listeners accordingly
        private void ChangeActiveDocument(XDocument doc)
        {
            if (_activeDocument != doc)
            {
                _activeDocument = doc;

                OnPropertyChanged(new PropertyChangedEventArgs("Document"));
            }
        }

        #endregion


        // Point of no return: do not ever try to use the inline XML again.
        private void DiscardInline()
        {
            _tryInlineDoc = false;
            _savedDocument = null;
            if (_waitForInlineDoc != null)
                _waitForInlineDoc.Set();
        }

        private IEnumerable GetResultNodeList(XDocument doc)
        {
            Debug.Assert(doc != null);

            return (IEnumerable)
                Dispatcher.Invoke(DispatcherPriority.Send, new DispatcherOperationCallback(CreateCollection), doc);
        }

        private object CreateCollection(object arg)
        {
            XDocument doc = (XDocument)arg;
            IEnumerable nodes = null;

            if (doc.Root != null)
            {
                if (String.IsNullOrEmpty(XPath))
                {
                    // if no XPath is specified, use the root node
                    ObservableCollection<XElement> oc = new ObservableCollection<XElement>();
                    oc.Add(doc.Root);
                    nodes = oc;
                }
                else
                {
                    // apply the XPath to the root node
                    _dummy = new FrameworkElement();
                    //_dummy.TargetUpdated += new EventHandler<DataTransferEventArgs>(OnTargetUpdated);
                    Binding binding = new Binding(XPath);
                    binding.Source = doc.Root;
                    //binding.NotifyOnTargetUpdated = true;
                    //PresentationTraceSources.SetTraceLevel(binding, PresentationTraceLevel.High);
                    BindingExpressionBase beb = BindingOperations.SetBinding(_dummy, FrameworkElement.TagProperty, binding);
                    nodes = _dummy.GetValue(FrameworkElement.TagProperty) as IEnumerable;
                }
            }

            return nodes;
        }

        FrameworkElement _dummy;

        void OnTargetUpdated(object sender, DataTransferEventArgs e)
        {
            OnQueryFinished(_dummy.GetValue(FrameworkElement.TagProperty) as IEnumerable);
        }

        //------------------------------------------------------
        //
        //  Private Properties
        //
        //------------------------------------------------------

        private DispatcherOperationCallback CompletedCallback
        {
            get
            {
                if (_onCompletedCallback == null)
                    _onCompletedCallback = new DispatcherOperationCallback(OnCompletedCallback);
                return _onCompletedCallback;
            }
        }

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        private XDocument _activeDocument;          // the active XLinq document
        private XDocument _document;    // a document set by user
        private XDocument _savedDocument;     // stored copy of Inline Xml for rollback
        private ManualResetEvent _waitForInlineDoc;  // serializer waits on this for inline doc
        private Uri _source;
        private Uri _baseUri;
        private string _xPath = String.Empty;
        private bool _tryInlineDoc = true;
        private XmlIslandSerializer _xmlSerializer;
        bool _isAsynchronous = true;
        bool _inEndInit;
        private DispatcherOperationCallback _onCompletedCallback;
        private DependencyObject _dummyDO = new DependencyObject();
    }

    public static class CriticalExceptions
    {
        public static bool IsCriticalException(Exception ex)
        {
            return ex is NullReferenceException ||
                    ex is StackOverflowException ||
                    ex is OutOfMemoryException ||
                    ex is System.Threading.ThreadAbortException ||
                    ex is System.Runtime.InteropServices.SEHException ||
                    ex is System.Security.SecurityException;
        }
    }
}
