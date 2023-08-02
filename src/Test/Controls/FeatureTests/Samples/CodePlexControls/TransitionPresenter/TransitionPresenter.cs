// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Limited Permissive License.
// See 
// http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Documents;

namespace WpfControlToolkit
{


    /// <summary>
    ///     
    /// TransitionPresenter is a FrameworkElement that hosts content and has a Transition Dependency Property.  Whenever the Content property is changed
    /// it applies the current Transition (which typically animates between the old and new content).
    /// 
    /// It does this by using two AdornerDecorators stacked on top of the other, each with a single ContentPresenter as a child.
    /// One adorner has the old Content and one has the new Content.  The Transition object is given the old and new ContentPresenters;
    /// it is up to the Transition to create an Animation and apply it to one or both of the ContentPresenters (e.g. fade the Opacity
    /// of the previous ContentPresenter to 0).  When the transition is complete the Transition object calls back into the TransitionPresenter,
    /// which removes the AdornerDecorator for the old Content from its visual tree.
    /// 
    /// Other than the Transition property this control is similar to a ContentControl.  It isn't a subclass of ContentControl because
    /// it makes no use of the Control.Template property.
    /// </summary>
    [System.Windows.Markup.ContentProperty("Content")]
    public class TransitionPresenter : FrameworkElement
    {

        //
        // Constructors
        //

        #region Constructors

        static TransitionPresenter()
        {
            ClipToBoundsProperty.OverrideMetadata(typeof(TransitionPresenter), new FrameworkPropertyMetadata(null, CoerceClipToBounds));
        }

        public TransitionPresenter()
        {
            _children = new UIElementCollection(this, null);
            ContentPresenter currentContent = new ContentPresenter();
            _currentHost = new AdornerDecorator();
            _currentHost.Child = currentContent;
            _children.Add(_currentHost);

            ContentPresenter previousContent = new ContentPresenter();
            _previousHost = new AdornerDecorator();
            _previousHost.Child = previousContent;
        }

        #endregion

        // 
        // Dependency Properties
        //

        #region Dependency Properties

        //
        //  Content
        //
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content",
                typeof(object),
                typeof(TransitionPresenter),
                new UIPropertyMetadata(null, OnContentChanged, CoerceContent));

        // Don't update content until done transitioning
        private static object CoerceContent(object element, object value)
        {
            TransitionPresenter te = (TransitionPresenter)element;
            if (te.IsTransitioning)
                return te.CurrentContentPresenter.Content;
            return value;
        }

        private static void OnContentChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            TransitionPresenter te = (TransitionPresenter)element;
            te.BeginTransition();
        }


        //
        //  Content Template
        //

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate",
                typeof(DataTemplate),
                typeof(TransitionPresenter),
                new UIPropertyMetadata(null, OnContentTemplateChanged));

        private static void OnContentTemplateChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            TransitionPresenter te = (TransitionPresenter)element;
            te.CurrentContentPresenter.ContentTemplate = (DataTemplate)e.NewValue;
        }


        //
        // ContentTemplateSelector
        //

        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(TransitionPresenter),
                new UIPropertyMetadata(null, OnContentTemplateSelectorChanged));


        private static void OnContentTemplateSelectorChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            TransitionPresenter te = (TransitionPresenter)element;
            te.CurrentContentPresenter.ContentTemplateSelector = (DataTemplateSelector)e.NewValue;
        }


        //
        // IsTransitioning
        //

        public bool IsTransitioning
        {
            get { return (bool)GetValue(IsTransitioningProperty); }
            private set { SetValue(IsTransitioningPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsTransitioningPropertyKey =
            DependencyProperty.RegisterReadOnly("IsTransitioning",
                typeof(bool),
                typeof(TransitionPresenter),
                new UIPropertyMetadata(false));

        public static readonly DependencyProperty IsTransitioningProperty =
            IsTransitioningPropertyKey.DependencyProperty;


        //
        // Transition
        //

        public Transition Transition
        {
            get { return (Transition)GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }

        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register("Transition", typeof(Transition), typeof(TransitionPresenter), new UIPropertyMetadata(null, null, CoerceTransition));

        private static object CoerceTransition(object element, object value)
        {
            TransitionPresenter te = (TransitionPresenter)element;
            if (te.IsTransitioning) return te._activeTransition;
            return value;
        }


        //
        // TransitionSelector
        //

        public TransitionSelector TransitionSelector
        {
            get { return (TransitionSelector)GetValue(TransitionSelectorProperty); }
            set { SetValue(TransitionSelectorProperty, value); }
        }

        public static readonly DependencyProperty TransitionSelectorProperty =
            DependencyProperty.Register("TransitionSelector", typeof(TransitionSelector), typeof(TransitionPresenter), new UIPropertyMetadata(null));


        #endregion // Dependency Properties


        //
        // Protected Overrides
        //

        #region Protected Overrides

        protected override Size MeasureOverride(Size availableSize)
        {
            _currentHost.Measure(availableSize);
            return _currentHost.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement uie in _children)
                uie.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
                throw new ArgumentException("index");
            return _children[index];
        }

        #endregion

        // 
        // Internal Methods and Properties
        //

        #region Internal Methods and Properties

        internal UIElementCollection Children
        {
            get { return _children; }
        }

        // Clean up after the transition is complete
        internal void OnTransitionCompleted(Transition transition)
        {
            _children.Clear();
            _children.Add(_currentHost);
            _currentHost.Visibility = Visibility.Visible;
            _previousHost.Visibility = Visibility.Visible;
            ((ContentPresenter)_previousHost.Child).Content = null;

            IsTransitioning = false;
            _activeTransition = null;
            CoerceValue(TransitionProperty);
            CoerceValue(ClipToBoundsProperty);
            CoerceValue(ContentProperty);
        }

        #endregion // Internal Methods and Properties

        // 
        // Private Methods and Properties
        //

        #region Private Methods and Properties

        private ContentPresenter PreviousContentPresenter
        {
            get { return ((ContentPresenter)_previousHost.Child); }
        }

        private ContentPresenter CurrentContentPresenter
        {
            get { return ((ContentPresenter)_currentHost.Child); }
        }

        private void BeginTransition()
        {
            TransitionSelector selector = TransitionSelector;

            Transition transition = selector != null ?
                selector.SelectTransition(CurrentContentPresenter.Content, Content) :
                Transition;

            if (transition != null)
            {
                // Swap content presenters
                AdornerDecorator temp = _previousHost;
                _previousHost = _currentHost;
                _currentHost = temp;
            }

            ContentPresenter currentContent = CurrentContentPresenter;
            // Set the current content
            currentContent.Content = Content;
            currentContent.ContentTemplate = ContentTemplate;
            currentContent.ContentTemplateSelector = ContentTemplateSelector;

            if (transition != null)
            {
                ContentPresenter previousContent = PreviousContentPresenter;

                if (transition.IsNewContentTopmost)
                    Children.Add(_currentHost);
                else
                    Children.Insert(0, _currentHost);

                IsTransitioning = true;
                _activeTransition = transition;
                CoerceValue(TransitionProperty);
                CoerceValue(ClipToBoundsProperty);
                transition.BeginTransition(this, previousContent, currentContent);
            }
        }


        // Force clip to be true if the active Transition requires it
        private static object CoerceClipToBounds(object element, object value)
        {
            TransitionPresenter transitionElement = (TransitionPresenter)element;
            bool clip = (bool)value;
            if (!clip && transitionElement.IsTransitioning)
            {
                Transition transition = transitionElement.Transition;
                if (transition.ClipToBounds)
                    return true;
            }
            return value;
        }



        #endregion // Private Methods and Properties


        //
        // Fields
        //

        #region Fields

        private UIElementCollection _children;

        private AdornerDecorator _currentHost;
        private AdornerDecorator _previousHost;

        private Transition _activeTransition;

        #endregion // Fields
    }
}
