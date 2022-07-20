// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Class for defining a series of asynchronous methods calls
//               that should be executed in order.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading; 
using System.Windows.Threading;		    // DispatcherOperationCallback.					
using System.Reflection;
using Annotations.Test.Reflection;

namespace Annotations.Test
{
	/// <summary>
	/// Mechanism for programmatically defining a series of ordered asynchronous method calls.  
	/// Use AsyncTestScriptRunner to execute an AsyncTestScript.
	/// 
	/// Here is an example of creating and running a script:
	/// 
	///   AsyncTestScript script = new AsyncTestScript();
	///   script.Add("methodA", new object [] { 1, "hello", foo });
	///   script.Add("methodB", null);
	///   script.Add("methodA", new object [] { 2, "world", bar });
	///   AsyncTestScriptRunner runner = new AsyncTestScriptRunner(this);
	///   runner.RunScript(script);
	/// 
	/// Parsing API is similar to an Enumerator.  You must call Next before accessing 
	/// the Script, then you can get the current method and arguments using the 
	/// CurrentXXX api.
	/// </summary>
	public class AsyncTestScript
	{		
		/// <summary>
		/// Default execution priority for all actions in this script.
		/// </summary>
		public DispatcherPriority DefaultPriority
		{
			get { return _defaultPriority; }
			set { _defaultPriority = value; }
		}

        /// <summary>
        /// Add a task to print status.
        /// </summary>
        public void AddStatus(string msg)
        {
            Add(null, "printStatus", new object[] { msg }, DefaultPriority);
        }

        /// <summary>
        /// Add an action to the script.  
        /// </summary>
        /// <param name="methodName">Name of a public method to invoke.</param>
        public void Add(string methodName)
        {
            Add(null, methodName, null, DefaultPriority);
        }        

        /// <summary>
		/// Add an action to the script.  
		/// </summary>
		/// <param name="methodName">Name of a public method to invoke.</param>
		/// <param name="args">list of arguments to pass to method or null if none.</param>
		public void Add(string methodName, object [] args) 
		{
			Add(null, methodName, args, DefaultPriority);
		}        

		/// <summary>
		/// Add an action to the script that should be invoked on an object other than the TestSuite.
		/// </summary>
		/// <param name="caller">Object to invoke method on.</param>
		/// <param name="methodName">Method to invoke.</param>
		/// <param name="args">Arguments to pass to method.</param>
		public void Add(object caller, string methodName, object[] args)
		{
			Add(caller, methodName, args, DefaultPriority);
		}

		/// <summary>
		/// Add an action to script.
		/// </summary>
		/// <param name="methodName">Method to invoke.</param>
		/// <param name="args">Arguments to pass to method.</param>
		/// <param name="priority">Priority with which the method should be invoked.</param>
		public void Add(string methodName, object[] args, DispatcherPriority priority)
		{
			Add(null, methodName, args, priority);
		}

		/// <summary>
		/// Add an action to script.
		/// </summary>
		/// <param name="caller">Object to invoke method on.</param>
		/// <param name="methodName">Method to invoke.</param>
		/// <param name="args">Arguments to pass to method.</param>
		/// <param name="priority">Priority with which the method should be invoked.</param>
		public void Add(object caller, string methodName, object[] args, DispatcherPriority priority)
		{
			_actions.Add(new AsyncAction(caller, methodName, args, priority));
		}

		/// <summary>
		/// Advance script iterator.  Must be called before first call to CurrentXXX api.
		/// </summary>
		public bool Next()
		{
			position++;
			return position <= _actions.Count - 1;
		}

		/// <summary>
		/// Get the current AsyncAction to perform.
		/// </summary>
		public AsyncAction Current
		{
			get
			{
				if (position < 0)
					throw new InvalidOperationException("Must call AsyncTestScript.Next before accessing Current.");
				if (position >= _actions.Count)
					throw new InvalidOperationException("There are no more Actions in the script.");
				return _actions[position];
			}
		}

		/// <summary>
		/// Map of objects to invoke methods indexed by the action order.
		/// </summary>
		private IList<AsyncAction> _actions = new List<AsyncAction>();
		private int position = -1;
		private DispatcherPriority _defaultPriority = DispatcherPriority.SystemIdle;
	}

	/// <summary>
	/// Definition of a single Asyncronous method call.
	/// </summary>
	public class AsyncAction
	{
		public AsyncAction(object caller, string methodName, object[] args, DispatcherPriority priority)
		{
			_caller = caller;
			_methodName = methodName;
			_args = args;
			_priority = priority;
		}

		public DispatcherPriority Priority
		{
			get
			{
				return _priority;
			}
		}

		public string Method
		{
			get
			{
				return _methodName;
			}
		}

		public object Caller
		{
			get
			{
				return _caller;
			}
		}

		public object[] Args
		{
			get
			{
				return _args;
			}
		}

		public Type[] ArgTypes
		{
			get
			{
                if (_argTypes == null)
					_argTypes = ReflectionHelper.ArgsToTypes(Args);
				return _argTypes;
			}
		}

		/// <returns>
		/// Text representation of what the current method call would look like. Good for logging.
		/// </returns>
		public string CallSignature
		{
			get
			{
				string callSignature = _methodName + "(";
				if (_args != null)
				{
					for (int i = 0; i < _args.Length; i++)
					{
						callSignature += (_args[i] != null) ? _args[i] : "null";
						if (i + 1 < _args.Length)
							callSignature += ", ";
					}
				}
				callSignature += ")";
				return callSignature;
			}
		}

        /// <returns>
        /// Text representation of what the method signature would look like.
        /// </returns>
        public string MethodSignature
        {
			get
			{
				string callSignature = _methodName + "(";
				if (_args != null)
				{
					for (int i = 0; i < ArgTypes.Length; i++)
					{
                        callSignature += (ArgTypes[i] != null) ? ArgTypes[i].Name : "object";
                        if (i + 1 < ArgTypes.Length)
							callSignature += ", ";
					}
				}
				callSignature += ")";
				return callSignature;
			}
        }

		object _caller;
		string _methodName;
		object[] _args;
        Type[] _argTypes;
		DispatcherPriority _priority;
	}
}
