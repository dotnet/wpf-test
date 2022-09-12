// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace D2Payloads
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows.Markup;     // IUriContext
    using System.Windows;
    using System.Windows.Documents;
    using System.Net;
    using System.IO.Packaging;


    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public sealed class FixedDocData : FixedDocSource
    {
        //--------------------------------------------------------------------
        //
        // Ctors
        //
        //---------------------------------------------------------------------
        #region Ctors
        private FixedDocData()
        {
        }

        public FixedDocData(Uri source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (!source.IsAbsoluteUri)
            {
                throw new ArgumentException("only absolute Uri is allowed");
            }

            _source = source;
        }


        public FixedDocData(IDocumentPaginatorSource doc)
        {
            _doc = doc;
        }
        #endregion Ctors

        //--------------------------------------------------------------------
        //
        // Public Methods
        //
        //---------------------------------------------------------------------

        #region Public methods
        public override InstanceDescriptor ToInstanceDescriptor()
        {
            if (_source != null)
            {
                ConstructorInfo ci = typeof(FixedDocData).GetConstructor(new Type[] { typeof(Uri) });
                return new InstanceDescriptor(ci, new object[] { this._source });
            }
            else 
            {
                ConstructorInfo ci = typeof(FixedDocData).GetConstructor(new Type[] { typeof(IDocumentPaginatorSource) });
                return new InstanceDescriptor(ci, new object[] { this._doc });
            }
        }
        #endregion Public Methods

        //--------------------------------------------------------------------
        //
        // Public Properties
        //
        //---------------------------------------------------------------------

        #region Public Properties
        public static FixedDocData Empty
        {
            get
            {
                return s_empty;
            }
        }

        public override IDocumentPaginatorSource FixedDoc
        {
            get
            {
                if (_doc == null)
                {
                    _LoadDocument();
                }
                return _doc;
            }
        }

        public override Uri Source
        {
            get
            {
                return _source;
            }
        }
        #endregion Public Properties

        //--------------------------------------------------------------------
        //
        // Public Events
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Protected Methods
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Internal Methods
        //
        //---------------------------------------------------------------------
        #region Internal Methods
        internal static FixedDocData FromString(
                            ITypeDescriptorContext  typeDescriptorContext, 
                            CultureInfo cultureInfo, 
                            string  uriString
                            )
        {
            if (String.IsNullOrEmpty(uriString))
            {
                return FixedDocData.Empty;
            }

            Uri  resolvedUri = GetUriFromUriContext(typeDescriptorContext, uriString);
            return new FixedDocData(resolvedUri);
        }


        internal static Uri GetUriFromUriContext(ITypeDescriptorContext context, string inputString)
        {
            Uri fullUri = new Uri(inputString, UriKind.RelativeOrAbsolute);
            if (fullUri.IsAbsoluteUri == false)
            {
                //Debug.Assert (context != null, "Context should not be null");
                if (context != null)
                {
                    IUriContext iuc = (IUriContext)context.GetService(typeof(IUriContext));

                    //Debug.Assert (iuc != null, "IUriContext should not be null here");
                    if (iuc != null)
                    {
                        // the base uri is NOT ""
                        if (iuc.BaseUri != null)
                        {

                            Uri relativeBaseUri = iuc.BaseUri;

                            if (relativeBaseUri.IsAbsoluteUri == true)
                            {
                                fullUri = new Uri(relativeBaseUri, fullUri);
                            }
                            else
                            {
                                relativeBaseUri = new Uri(new Uri("pack://application:,,,/"), relativeBaseUri);
                                fullUri = new Uri(relativeBaseUri, fullUri);
                            }
                        } // relativeBaseUriString != ""
                        else
                        {
                            // if we reach here, the base uri we got from IUriContext is ""
                            // and the inputString is a relative uri.  Here we resolve it to 
                            // application's base 
                            fullUri = new Uri(new Uri("pack://application:,,,/"), fullUri);
                        }
                    } // iuc != null 
                } // context!= null
            } // fullUri.IsAbsoluteUri == false

            return fullUri;
        }
        #endregion Internal Methods

        //--------------------------------------------------------------------
        //
        // private Properties
        //
        //---------------------------------------------------------------------

        //--------------------------------------------------------------------
        //
        // Private Methods
        //
        //---------------------------------------------------------------------

        #region Private Methods
        private void _LoadDocument()
        {
            Stream docStream = null;
            IWebRequestCreate factory = (IWebRequestCreate)new PackWebRequestFactory();

            if (_source.IsAbsoluteUri)
            {
                WebRequest request;
                if (String.Compare(_source.Scheme, "pack", StringComparison.Ordinal) == 0)
                {
                    request = factory.Create(_source);
                }
                else
                {
                    request = WebRequest.Create(_source);
                }
                docStream = request.GetResponse().GetResponseStream();
            }
            else
            {
                if (_source.IsFile)
                {
                    docStream = new System.IO.FileStream(_source.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    Debug.Assert(docStream != null);
                }
            }

            ParserContext pc = new ParserContext();

            pc.BaseUri = _source;
            _doc = (new XamlReader()).LoadAsync(docStream, pc) as IDocumentPaginatorSource;

        }
        #endregion Private Methods

        //--------------------------------------------------------------------
        //
        // Private Fields
        //
        //---------------------------------------------------------------------
        #region Private Fields
        private Uri _source;
        private IDocumentPaginatorSource _doc;
        private static FixedDocData s_empty = new FixedDocData();
        #endregion Private Fields
    }
}
