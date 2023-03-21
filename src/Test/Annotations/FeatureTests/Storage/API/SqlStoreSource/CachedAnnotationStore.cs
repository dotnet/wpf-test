// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;

namespace Avalon.Test.Annotations.Storage
{
    /// <summary>
    /// AnnotationService requires that an AnnotationStore maintain the identity of Annotation
    /// instances that it returns.  This store provides instance caching to ensure that each
    /// store query will always return the same object for a given annotation Id.
    /// </summary>
    public class CachedAnnotationStore : AnnotationStore
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Create an instance of CachedAnnotationStore.
        /// </summary>
        /// <param name="datasource">Class responsible for persisting annotations in some form.</param>
        public CachedAnnotationStore(IAnnotationDataSource datasource) : base()
        {
            _annotationCache = new AnnotationCache(HandleAuthorChanged, HandleAnchorChanged, HandleCargoChanged);
            _dataSource = datasource;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Add a new annotation to the store.
        /// </summary>
        /// <param name="newAnnotation">Annotation to be added to the store.</param>
        /// <exception cref="ArgumentNullException">The Passed in paramter is null.</exception>
        /// <exception cref="ArgumentException">The annotation already exists in the store.</exception>
        /// <exception cref="ObjectDisposedException">if object has been Disposed</exception>
        public override void AddAnnotation(Annotation newAnnotation)
        {
            if (newAnnotation == null)
                throw new ArgumentNullException("newAnnotation");

            //Modifying internal data.  Lock the object to avoid
            //conflicting modifications from other threads.
            lock (SyncRoot)
            {
                CheckStatus();

                //Check to see if the Annotation already exists in the store.  If so throw argument.
                if (_dataSource.ContainsAnnotation(newAnnotation.Id))
                    throw new ArgumentException("Annotation Already Exists in DataSource");
                
                //Check to see if the Annotation already exists in the AnnotationsMap
                if (_annotationCache.FindAnnotation(newAnnotation.Id) != null)
                    throw new ArgumentException("Annotation Already Exists in Map");

                // simply add the annotation to the map to save on performance
                // notice that we need to tell the map that this instance of the annotation is dirty
                _annotationCache.AddAnnotation(newAnnotation, true);
            }

            OnStoreContentChanged(new StoreContentChangedEventArgs(StoreContentAction.Added, newAnnotation));
        }

        /// <summary>
        /// Delete the Annotation specified by the parameter passed.
        /// </summary>
        /// <param name="annotationId">Id of the Annotation to delete</param>
        /// <returns>The annotation that was deleted or null if no annotation 
        /// with the id was found.</returns>
        /// <exception cref="InvalidOperationException">if no stream has been set on the store</exception>
        /// <exception cref="ObjectDisposedException">if object has been disposed</exception>
        public override Annotation DeleteAnnotation(Guid annotationId)
        {
            Annotation returnAnnotation = null;

            //We are going to modify internal data.  Lock the object
            //to avoid modifications from other threads.
            lock (SyncRoot)
            {
                CheckStatus();

                //attempt to obtain the annotation from the map
                returnAnnotation = _annotationCache.FindAnnotation(annotationId);

                //Only attempt to get the annotation from the db
                //if it is not already in the map.
                if (_dataSource.ContainsAnnotation(annotationId))
                {
                    // Only deserialize the annotation if its not already in our map
                    if (returnAnnotation == null)
                    {
                        returnAnnotation = _dataSource.GetAnnotation(annotationId);
                    }
                    _dataSource.DeleteAnnotation(annotationId);
                }

                // notice that in Add we add the annotation to the map only
                // but in delete we delete it from both to the map and the disk.                
                _annotationCache.RemoveAnnotation(annotationId);
            }

            //Only Fire this notification if we actually removed an annotation.
            if(returnAnnotation != null)
                OnStoreContentChanged(new StoreContentChangedEventArgs(StoreContentAction.Deleted, returnAnnotation));

            return returnAnnotation;
        }

        public override IList<Annotation> GetAnnotations(ContentLocator anchorLocator)
        {
            if (anchorLocator == null)
                throw new ArgumentNullException("anchorLocator");

            IList<Guid> dataSourceAnnotations = _dataSource.GetAnnotations(anchorLocator);           
            Dictionary<Guid, Annotation> mapAnnotations = _annotationCache.FindAnnotations(anchorLocator);
            
            //Merge And Cache both results and return list.
            return MergeAndCacheAnnotations(mapAnnotations, dataSourceAnnotations);
        }

        /// <summary>
        /// Returns a list of all annotations in the store.
        /// </summary>
        /// <returns>Annotations list.  Can return an empty list but never a null.</returns>
        /// <exception cref="ObjectDisposedException">if object has been disposed</exception>
        public override IList<Annotation> GetAnnotations()
        {
            lock (SyncRoot)
            {
                CheckStatus();

                IList<Guid> dataSourceAnnotations = _dataSource.GetAnnotations();
                Dictionary<Guid, Annotation> mapAnnotations = _annotationCache.FindAnnotations();

                //Merge And Cache both results and return list.
                return MergeAndCacheAnnotations(mapAnnotations, dataSourceAnnotations);
            }
        }

        /// <summary>
        /// Finds annotation by id.
        /// </summary>
        /// <param name="annotationId">Id of annotation to find.</param>
        /// <returns>The annotation.  Null if no annotation was with matching Id was found.</returns>
        /// <exception cref="ObjectDisposedException">if object has been disposed</exception>
        public override Annotation GetAnnotation(Guid annotationId)
        {
            lock (SyncRoot)
            {
                CheckStatus();

                Annotation annot = _annotationCache.FindAnnotation(annotationId);
                // If annotation is not in map, read it from the datasource.
                if (annot == null)
                {
                    // Since annotation wasn't in the map, attempt to retrieve the annotation from the datasource.
                    annot = _dataSource.GetAnnotation(annotationId);
                    // If the datasource contains the annotation then add it to the map.
                    if (annot != null)
                        _annotationCache.AddAnnotation(annot, false);
                }
                return annot;
            }
        }

        /// <summary>
        ///     Causes any buffered data to be written to the database.
        ///     Gets called after each operation if AutoFlush is set to true.
        /// </summary>
        /// <exception cref="ObjectDisposedException">if object has been disposed</exception>
        /// <seealso cref="AutoFlush"/>
        public override void Flush()
        {
            //Manipulating data so lock to avoid conflicts
            //with other threads.
            lock (SyncRoot)
            {
                CheckStatus();

                // If there are any changes that have been applied to the cache then
                // apply these to the AnnotationDataSource.
                if (_annotationCache.IsDirty)
                {
                    IList<Annotation> dirtyAnnotations = _annotationCache.FindDirtyAnnotations();
                    foreach (Annotation dirtyAnnotation in dirtyAnnotations)
                    {
                        // If annotation does not exist in the datasource then add it.
                        if (!_dataSource.ContainsAnnotation(dirtyAnnotation.Id))
                            _dataSource.AddAnnotation(dirtyAnnotation);
                        // Otherwise, update the existing defintion.
                        else
                            _dataSource.UpdateAnnotation(dirtyAnnotation);                 
                    }

                    // Only validate all the dirty annotations when we have updated all of them.
                    _annotationCache.ValidateDirtyAnnotations();
                }

                // Finally flush the AnnotationDataSource to save these changes.
                _dataSource.Flush();
            }            
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Event Handlers
        //
        //------------------------------------------------------

        #region Event Handlers

        protected void HandleCargoChanged(object sender, AnnotationResourceChangedEventArgs args)
        {
            base.OnCargoChanged(args);
        }
        protected void HandleAnchorChanged(object sender, AnnotationResourceChangedEventArgs args)
        {
            base.OnAnchorChanged(args);
        }
        protected void HandleAuthorChanged(object sender, AnnotationAuthorChangedEventArgs args)
        {            
            base.OnAuthorChanged(args);
        }

        #endregion Event Handlers

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        /// <summary>
        ///    Verifies the store is in a valid state.  Throws exceptions otherwise.
        /// </summary>
        private void CheckStatus()
        {
            lock (SyncRoot)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("CachedAnnotationStore", "Object Disposed");
            }
        }

        private IList<Annotation> MergeAndCacheAnnotations(Dictionary<Guid, Annotation> mapAnnotations, IList<Guid> dataSourceAnnotations)
        {
            //First put all of the annotations in the map into the return list.
            IList<Annotation> annotations = new List<Annotation>((IEnumerable<Annotation>)mapAnnotations.Values);

            //Three possible conditions we need to handle.
            //1 - An annotation exists in both the database and the map.
            //2 - An annotation exists in the database but not in the map results.
            //      2a - The annotation is found in the map
            //      2b - The annotation is not found in the map.

            //Cycle through each annotation in the database results and find annotations that
            //are not in the map results and verify that they should be returned.
            foreach (Guid annotationId in dataSourceAnnotations)
            {
                Annotation annot;
                bool foundInMapResults = mapAnnotations.TryGetValue(annotationId, out annot);

                //Check to see if annotation was in the map results.
                if (!foundInMapResults)
                {
                    annot = GetAnnotation(annotationId);
                    annotations.Add(annot);
                }
            }
            return annotations;
        }        

        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        public override bool AutoFlush
        {
            get
            {
                lock (SyncRoot)
                {
                    return _autoFlush;
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    _autoFlush = value;

                    //Commit Anything that needs to be commited.
                    Flush();
                }
            }
        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields
        
        // Boolean flag represents whether the store should perform a flush
        // after every operation or not
        private bool _autoFlush;
        
        // Map that holds AnnotationId->Annotation
        private AnnotationCache _annotationCache;

        // Module that handles persisting annotations to some media.
        private IAnnotationDataSource _dataSource;

        #endregion Private Fields

    }
}

