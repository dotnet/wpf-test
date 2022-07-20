// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Visual tree parser for retrieving annotation control instances.

using System;
using System.Windows;
using Annotations.Test;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Proxies.MS.Internal.Annotations.Component;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;

using StickyNoteControl_Proxy = Proxies.System.Windows.Controls.StickyNoteControl;
using Proxies.MS.Internal.Annotations;
using System.Windows.Documents;
using System.Collections;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Module that parsers the visual tree of a DocumentViewer and returns annotation controls.
	/// </summary>
    public class AnnotationComponentFinder
	{
		#region Constructors

		public AnnotationComponentFinder(DependencyObject target)
        {
			_target = target;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Walk the visual tree and get all the StickyNote controls that are in it.
		/// </summary>
		/// <returns>IList of StickyNotes that exist in the visual tree.</returns>
		public static IList<StickyNoteControl> GetVisibleStickyNotes(Visual node)
		{
			return new VisualTreeWalker<StickyNoteControl>().FindChildren(node);
		}

		/// <summary>
		/// Walk the visual tree and get all the HighlightComponents that are in it.
		/// </summary>
		/// <returns>IList of HighlightComponents that exist in visual tree.</returns>
		public static IList GetVisibleHighlightComponents(Visual node)
		{
			return new HighlightComponentFinder().FindHighlights(node);
		}

		#endregion

		#region Static Properties

		public static Type HighlightComponentType
		{
			get
			{
				return typeof(FrameworkElement).Assembly.GetType("MS.Internal.Annotations.Component.HighlightComponent");
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Talk to the AnnotationComponentManager and get a list of all the current HighlightComponents,
		/// there should correspond to only visible highlights.
		/// </summary>
		/// <returns>IList of visible HighlightComponents.</returns>
		public IList<HighlightComponent> GetHighlightComponents()
		{
			return new ComponentParser<HighlightComponent>().GetComponents(_target);
		}

		/// <summary>
		/// Talk to the AnnotationComponentManager and get a list of all the current HighlightComponents,
		/// there should correspond to only visible highlights.
		/// </summary>
		/// <returns>IList of visible HighlightComponents.</returns>
		public IList<StickyNoteControl_Proxy> GetStickyNoteComponents()
		{
			return new ComponentParser<StickyNoteControl_Proxy>().GetComponents(_target);
		}

		#endregion

		#region Private Fields

		DependencyObject _target;

		#endregion

		#region Inner Classes

		class ComponentParser<T>
		{
			/// <summary>
			/// Look at the list of AnnotationComponents as held by the AnnotationComponentManager and return those
			/// that are of the given type.
			/// </summary>
			public IList<T> GetComponents(DependencyObject target)
			{
				IList<T> components = new List<T>();

                AnnotationService service = AnnotationService.GetService(target);
				AnnotationComponentManager componentManager = service.AnnotationComponentManager;
				IList<IAttachedAnnotation> attachedAnnotations = service.GetAttachedAnnotations();
				foreach (IAttachedAnnotation attached in attachedAnnotations)
				{
					IAnnotationComponent component = componentManager.attachedAnnotations[attached][0];
					try
					{
						T asType = (T)component;
						components.Add(asType);
					}
					catch (InvalidCastException)
					{
						// ignore.
					}
				}

				return components;
			}
		}

		/// <summary>
		/// Parses the visual tree looking for HighlightComponents that are NOT associated with 
		/// a StickyNote.
		/// </summary>
		class HighlightComponentFinder
		{
			/// <summary>
			/// Recursively walk tree starting at given node.
			/// </summary>
			/// <returns>All HighlightComponents that are NOT associated with a StickyNote</returns>
			public IList FindHighlights(Visual startNode)
			{
				_results = new ArrayList();
				DoWalkTree(startNode);
				return _results;
			}

			private void DoWalkTree(DependencyObject startNode)
			{
				int count = VisualTreeHelper.GetChildrenCount(startNode);
				for(int i = 0; i < count; i++)
				{
					object current = VisualTreeHelper.GetChild(startNode,i);
					// If it is of type HighlightComponent.
					if (current.GetType().Equals(AnnotationComponentFinder.HighlightComponentType))
					{
						// Check its _type field to determine if it is a standalone highlight or
						// a StickyNote anchor.
						if (ReflectionHelper.GetField(current, "_type").Equals(ReflectionHelper.GetField(current, "_name")))
							_results.Add(current);
					}
					DoWalkTree(VisualTreeHelper.GetChild(startNode,i));
				}
			}

			IList _results;
		}

		#endregion
	}
}	

