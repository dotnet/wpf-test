// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Collections.Generic;
using System.Windows.Annotations;

namespace Avalon.Test.Annotations.Storage
{
    /// <summary>
    /// Interface for a class that provides persistent storage of Annotations.
    /// </summary>
    public interface IAnnotationDataSource
    {
        /// <summary>
        /// Add annotation to the DataSource.
        /// </summary>
        void AddAnnotation(Annotation annotation);

        /// <summary>
        /// Delete annotation with the given id from the DataSource.
        /// </summary>
        /// <param name="annotationId">Id of the annotation to be removed.</param>
        void DeleteAnnotation(Guid id);

        /// <summary>
        /// Update data source entry for given annotation.
        /// </summary>
        /// <param name="annotation">Annotation to update.</param>
        void UpdateAnnotation(Annotation annotation);

        /// <summary>
        /// Returns whether or not data source contains an annotation.
        /// </summary>
        /// <param name="id">Id of annotation to look for.</param>
        /// <returns>True if data source contains annotation with given id.</returns>
        bool ContainsAnnotation(Guid id);

        /// <summary>
        /// Get Annotation for given id.
        /// </summary>
        /// <param name="id">Id of annotation to return.</param>
        /// <returns>Annotation with given id or null if none exists.</returns>
        Annotation GetAnnotation(Guid id);

        /// <summary>
        /// Returns all annotations in data source.
        /// </summary>
        IList<Guid> GetAnnotations();

        /// <summary>
        /// Get all annotations whose ContentLocators are the same or a superset of the
        /// LocatorParts of the given locator.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns>All annotations whose Anchor starts with the given locator.</returns>
        IList<Guid> GetAnnotations(ContentLocator locator);

        /// <summary>
        /// Apply all changes to the data source.
        /// </summary>
        void Flush();

        /// <summary>
        /// Closes data source.
        /// </summary>
        void Close();
    }
}

