// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Packaging;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using System.Windows.Markup;
using System.Globalization;
using System.Windows;

namespace DRT
{
    /// <summary>
    /// This class will create a XpsDocument.
    /// </summary>
    public static class ReachPackageHelper
    {
        #region Public Methods
        /// <summary>
        /// Creates a sample XpsDocument with one page for each canvas.
        /// Page size will match canvas size.
        /// Page name is 'PageX' where 'X' is the index + 1 of the canvas.
        /// </summary>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="canvases">A list of canvases that will be used to create pages.</param>
        public static void CreatePackage(string fileName, Canvas[] canvases)
        {
            CreatePackage(fileName, canvases, null);
        }
        /// <summary>
        /// Creates a sample XpsDocument with one page for each canvas.
        /// Page size will match canvas size.
        /// Page name is 'PageX' where 'X' is the index + 1 of the canvas.
        /// </summary>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="canvases">A list of canvases that will be used to create pages.</param>
        /// <param name="coreProperties">A PackageProperties element to be included in the package.</param>
        public static void CreatePackage(string fileName, Canvas[] canvases, PackageProperties coreProperties)
        {
            if (File.Exists(fileName)) { File.Delete(fileName); }

            XpsDocument rp = new XpsDocument(fileName, FileAccess.ReadWrite);

            FixedPage[] pages = new FixedPage[canvases.Length];

            for (int i = 0; i < canvases.Length; i++)
            {
                pages[i] = CreateFixedPage(
                        canvases[i],
                        string.Format(CultureInfo.InvariantCulture, "Page{0}", i+1));
            }

            FixedDocumentSequence fds = CreateFixedDocumentSequence(
                CreateFixedDocument(pages));

            AttachSequenceToPackage(fds, rp);

            if (coreProperties != null)
            {
                AttachCorePropertiesToPackage(coreProperties, rp);
            }

            rp.Close();
        }
        
        /// <summary>
        /// This will create a Canvas 8.5" x 11" with a single TextBlock containing the specified string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Canvas CreateCanvas(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;

            return CreateCanvas(textBlock, 0.0, 0.0);
        }

        /// <summary>
        /// This will create a Canvas 8.5" x 11" with the TextBlock passed in.
        /// </summary>
        /// <param name="textBlock">The text to place on the Canvas.</param>
        /// <param name="horizontalOffset">The number of pixels to offset the TextBlock from the left.</param>
        /// <param name="verticalOffset">The number of pixels to offset the TextBlock from the top.</param>
        /// <returns>A Canvas with the text.</returns>
        public static Canvas CreateCanvas(UIElement element, double horizontalOffset, double verticalOffset)
        {
            Canvas canvas = new Canvas();
            canvas.Width = 96 * 8.5;
            canvas.Height = 96 * 11;

            Canvas.SetTop(element, verticalOffset);
            Canvas.SetLeft(element, horizontalOffset);

            canvas.Children.Add(element);

            return canvas;
        }

        /// <summary>
        /// This will create a Canvas 8.5" x 11" with a single TextBlock containing the specified string followed by an image.
        /// </summary>
        /// <param name="text">The text to place on the Canvas.</param>
        /// <param name="imagePath">A relative location of an image to add to the Canvas.</param>
        /// <returns>A Canvas with the text and image.</returns>
        public static Canvas CreateCanvas(string text, string imagePath)
        {
            Canvas canvas = CreateCanvas(text);

            Image image = new Image();
            image.Source = BitmapFrame.Create(
                new Uri(imagePath, UriKind.Relative),
                BitmapCreateOptions.None,
                BitmapCacheOption.None);

            Canvas.SetTop(image, 96 * .5);
            Canvas.SetLeft(image, 96 * .5);

            canvas.Children.Add(image);

            return canvas;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Attaches a FixedDocument to a FixedDocumentSequence.
        /// </summary>
        /// <param name="document">The document to attach.</param>
        /// <param name="fds">The document sequence to attach to.</param>
        private static void AttachDocumentToSequence(FixedDocument document, FixedDocumentSequence fds)
        {
            DocumentReference dr = new DocumentReference();

            dr.BeginInit();
            dr.SetDocument(document);
            dr.EndInit();

            ((IAddChild)fds).AddChild(dr);
        }

        /// <summary>
        /// Attaches a FixedPage to a FixedDocument.
        /// </summary>
        /// <param name="page">The page to attach.</param>
        /// <param name="document">The document to attach to.</param>
        private static void AttachPageToDocument(FixedPage page, FixedDocument document)
        {
            PageContent content = new PageContent();

            content.BeginInit();
            ((IAddChild)content).AddChild(page);
            content.EndInit();

            ((IAddChild)document).AddChild(content);
        }

        /// <summary>
        /// Attaches a FixedDocumentSequence to an XpsDocument.
        /// </summary>
        /// <param name="fds">The document sequence to attach.</param>
        /// <param name="rp">The Xps package that will contain the document sequence.</param>
        private static void AttachSequenceToPackage(FixedDocumentSequence fds, XpsDocument rp)
        {
            XpsSerializationManager rsm = new XpsSerializationManager(
                new XpsPackagingPolicy(rp), false);

            rsm.SaveAsXaml(fds);
        }

        /// <summary>
        /// Attaches PackageProperties to an XpsDocument.
        /// </summary>
        /// <param name="coreProperties">The PackageProperties to attach.</param>
        /// <param name="rp">The Xps package that will contain the properties.</param>
        private static void AttachCorePropertiesToPackage(PackageProperties coreProperties, XpsDocument rp)
        {
            TestServices.Assert(rp.CoreDocumentProperties != null,
                "XpsDocument.CoreDocumentProperties is null, cannot set new property values.");

            rp.CoreDocumentProperties.Category = coreProperties.Category;
            rp.CoreDocumentProperties.ContentStatus = coreProperties.ContentStatus;
            rp.CoreDocumentProperties.ContentType = coreProperties.ContentType;
            rp.CoreDocumentProperties.Creator = coreProperties.Creator;
            if (coreProperties.Created.HasValue)
            {
                rp.CoreDocumentProperties.Created = coreProperties.Created.Value;
            }
            if (coreProperties.Modified.HasValue)
            {
                rp.CoreDocumentProperties.Modified = coreProperties.Modified.Value;
            }
            rp.CoreDocumentProperties.Description = coreProperties.Description;
            rp.CoreDocumentProperties.Identifier = coreProperties.Identifier;
            rp.CoreDocumentProperties.Keywords = coreProperties.Keywords;
            rp.CoreDocumentProperties.Language = coreProperties.Language;
            rp.CoreDocumentProperties.LastModifiedBy = coreProperties.LastModifiedBy;
            if (coreProperties.LastPrinted.HasValue)
            {
                rp.CoreDocumentProperties.LastPrinted = coreProperties.LastPrinted.Value;
            }
            rp.CoreDocumentProperties.Revision = coreProperties.Revision;
            rp.CoreDocumentProperties.Subject = coreProperties.Subject;
            rp.CoreDocumentProperties.Title = coreProperties.Title;
            rp.CoreDocumentProperties.Version = coreProperties.Version;
        }

        /// <summary>
        /// This will create a valid Xps FixedDocument.
        /// </summary>
        /// <param name="pages">The FixedPages to include.</param>
        /// <returns>A valid Xps FixedDocument.</returns>
        private static FixedDocument CreateFixedDocument(FixedPage[] pages)
        {
            FixedDocument document = new FixedDocument();

            foreach (FixedPage page in pages)
            {
                AttachPageToDocument(page, document);
            }

            return document;
        }

        /// <summary>
        /// This will create a valid Xps FixedPage.
        /// </summary>
        /// <param name="canvas">The Canvas to include in the page.</param>
        /// <param name="name">The name of the page.</param>
        /// <returns>A valid Xps FixedPage.</returns>
        private static FixedPage CreateFixedPage(Canvas canvas, string name)
        {
            FixedPage page = new FixedPage();

            FixedPage.SetLeft(canvas, 0);
            FixedPage.SetTop(canvas, 0);

            page.Width = canvas.Width;
            page.Height = canvas.Height;

            page.Name = name;

            page.Children.Add(canvas);

            return page;
        }

        /// <summary>
        /// This will create a valid Xps FixedDocumentSequence.
        /// </summary>
        /// <param name="document">The FixedDocument to include.</param>
        /// <returns>A valid Xps FixedDocumentSequence.</returns>
        private static FixedDocumentSequence CreateFixedDocumentSequence(FixedDocument document)
        {
            FixedDocumentSequence fds = new FixedDocumentSequence();

            AttachDocumentToSequence(document, fds);

            return fds;
        }
        #endregion
    }
}
