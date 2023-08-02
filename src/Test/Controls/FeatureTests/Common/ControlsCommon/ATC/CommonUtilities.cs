//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

#region Using Statements

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

#endregion

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    /// interface for test step
    /// every test case is made of sevarl test steps
    /// if any of them fails to execute, the whole test case will fail.
    /// </summary>
    public interface ITestStep
    {
        /// <summary>
        /// do test agaist testObject
        /// </summary>
        /// <param name="testObject">the object to be tested</param>
        void Do(object testObject);
    }

    /// <summary>
    /// interface for finder utility class
    /// it is used to find other object from the source object.
    /// </summary>
    public interface IFinder
    {
        /// <summary>
        /// do finding agaist the source object
        /// </summary>
        /// <param name="source">the object to do finding against</param>
        /// <returns>the object that is found</returns>
        object Find(object source);
    }

    /// <summary>
    /// 1. abstract class for inner finder
    ///     every finder deriving from this class can have an InnerFinder property
    ///     the InnerFinder will be used to do finding against the found item.
    /// 2. subclass from MarkupExtension
    ///     every finder deriving from this class can be used the sample as other markup extension,
    ///     such as Binding, StaticExtension
    /// </summary>
    [XmlContentParser("InnerFinder")]
    public abstract class InnerFinderBase : MarkupExtension, IFinder
    {
        #region InnerFinder

        public IFinder InnerFinder
        {
            get { return _innerFinder; }
            set { _innerFinder = value; }
        }

        private IFinder _innerFinder;

        #endregion

        #region IFinder Members

        object IFinder.Find(object source)
        {
            object target = Find(source);
            if (InnerFinder != null)
                target = InnerFinder.Find(target);
            return target;
        }

        #endregion

        #region Protected Members

        protected abstract object Find(object source);

        #endregion

        #region MarkupExtension Members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Find resource with ResourceKey in a FrameworkElement
    /// Sample:
    /// <code>
    ///     <ResourceFinder ResourceKey='src'/>
    /// </code>
    /// or
    /// <code>
    ///     <DispatcherAction Finder='{ResourceFinder src}'/>
    /// </code>
    /// </summary>
    public sealed class ResourceFinder : InnerFinderBase
    {
        #region Constructor

        public ResourceFinder()
        {
        }

        public ResourceFinder(object resourceKey)
        {
            ResourceKey = resourceKey;
        }

        #endregion

        #region InnerFinderBase Members

        protected override object Find(object obj)
        {
            FrameworkElement fe = obj as FrameworkElement;
            Assert.AssertTrue("obj is not a FrameworkElement", fe != null);
            Assert.AssertTrue("ResourceKey not specified", ResourceKey != null);
            return fe.Resources[ResourceKey];
        }

        #endregion

        #region ResourceKey

        public object ResourceKey
        {
            get { return _resourceKey; }
            set { _resourceKey = value; }
        }

        private object _resourceKey;

        #endregion
    }

    /// <summary>
    /// ObservableCollection&lt;object&gt; not supported in xaml, so create this class
    /// </summary>
    public sealed class ObservableObjectCollection : ObservableCollection<object>
    {
    }
}
