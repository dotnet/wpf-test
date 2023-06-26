// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;		    //Reflection
using System.Collections.Generic;	//List<T>
using System.Threading;			    //Timeout

namespace Microsoft.Test.KoKoMo
{
	/// <summary>
	/// Any exception thrown from KoKoMo code.
	/// </summary>
	public class ModelException : Exception
	{
        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="item"></param>
        /// <param name="message"></param>
		public ModelException(ModelItem item, string message)
			: this(item, message, null, null)
		{
			//Delegate
		}

        /// <summary>
        /// ModelException method
        /// </summary>
        /// <param name="item"></param>
        /// <param name="message"></param>
        /// <param name="inner"></param>
		public ModelException(ModelItem item, string message, Exception inner)
			: this(item, message, inner, null)
		{
			//Delegate
		}

        /// <summary>
        /// ModelException method
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="message"></param>
		public ModelException(ModelEngine engine, string message)
			: this(null, "Engine: " + message, null, engine.ActionsTrace)
		{
			//Delegate
		}

        /// <summary>
        /// ModelException method
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="message"></param>
        /// <param name="inner"></param>
		public ModelException(ModelEngine engine, string message, Exception inner)
			: this(null, "Engine: " + message, inner, engine.ActionsTrace)
		{
			//Delegate
		}
        /// <summary>
        /// method class 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="actions"></param>
        public ModelException(ModelItem source, string message, Exception inner, List<ModelActionInfo>   actions)
			: base(source != null ? (source.FullName + ": " + message) : message, inner)
		{
			//Action dump
			if(actions != null && actions.Count > 0)
			{
				ModelTrace.WriteLine( "Actions : ");
				foreach(ModelActionInfo action in actions )
					ModelTrace.WriteLine(action);
			}
		}
	}
}
