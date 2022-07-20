// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//
using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Windows.Annotations;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Definition that encapsulates the procedure of creating an annotation for a certain selection.
	/// </summary>
	public abstract class AnnotationDefinition
	{
		#region Constructors

		public AnnotationDefinition(ISelectionData anchor)
		{
			_anchor = anchor;
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Create annotation for the given selection.
		/// </summary>
		public abstract void Create(ATextControlWrapper target);

		/// <summary>
		/// Before creating this annotation, navigate so that the selection it will be created for is
		/// visible.  Then create annotation.
		/// </summary>
		public abstract void Create(ATextControlWrapper target, bool goToSelection);

		/// <summary>
		/// Delete annotation for the given selection.
		/// </summary>
		/// <param name="target"></param>
		public abstract void Delete(ATextControlWrapper target);

		#endregion

		#region Protected Properties

		protected ISelectionData Anchor
		{
			get { return _anchor; }
		}

		#endregion

		#region Fields

		private ISelectionData _anchor;

		#endregion
	}	
}	
