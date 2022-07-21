// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Runs an AsyncTestScript and calls TestSuite.PassTest
//			     if no exceptions occur.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading; using System.Windows.Threading;				// DispatcherOperationCallback.					
using System.Reflection;
using Annotations.Test.Framework;
using Annotations.Test.Reflection;

namespace Annotations.Test
{
	/// <summary>
	/// Runs an AsyncTestScript using the given TestSuite as the context for
	/// the script.
	/// </summary>
	public class AsyncTestScriptRunner 
	{
		/// <summary>
		/// Create a AsyncTestScriptRunner for this TestSuite.
		/// </summary>
		/// <param name="context">TestSuite which all calls to RunScript will be run
		/// against.</param>
		public AsyncTestScriptRunner(TestSuite context)
		{
			runContext = context;
		}

		/// <summary>
		/// Asynchrounously run given AsyncTestScript, pass if we reach the end of the script
		/// without error.
		/// </summary>
		public void Run(AsyncTestScript scriptToRun)
		{
			Run(scriptToRun, true);
		}

		/// <summary>
		/// Asynchrounously run given AsyncTestScript.
		/// </summary>
		/// <param name="passOnFinish">If True assert TestSuite.PassTest if script runs
		/// without error.</param>
		public void Run(AsyncTestScript scriptToRun, bool passOnFinish)
		{
			this.passOnFinish = passOnFinish;
			_script = scriptToRun;
            runContext.QueueTimerTask(LaunchNextAction, new TimeSpan(0));
        }

		/// <summary>
		/// Read the next action from the script.  Then post a task for this action with its specified priority.
		/// </summary>
		/// <param name="arg">unused.</param>
		/// <returns>null.</returns>
        private void LaunchNextAction(object sender, EventArgs args)
        {
			DispatcherTimer timer = sender as DispatcherTimer;
			if (timer != null)
				timer.Stop();

			if (_script.Next())
			{
				runContext.queueTask(new DispatcherOperationCallback(RunAction), _script.Current, _script.Current.Priority);
			}
			else if (passOnFinish)
			{
				runContext.passTest("Test script completed without error.");
			}
		}

		/// <summary>
		/// Run the given AsyncAction, then post LaunchNextAction to the queue so that the next action will be selected and run.
		/// </summary>
		/// <param name="arg">AsyncAction to execute.</param>
		/// <returns>Null.</returns>
		private object RunAction(object arg)
		{
			AsyncAction action = (AsyncAction) arg;
			object caller = action.Caller;
			if (caller == null)
				caller = runContext;

			MethodInfo methodToRun = ReflectionHelper.FindMethod(caller.GetType(), action.Method, action.ArgTypes);
			if (methodToRun == null)
				runContext.failTest("Couldn't find method named '" + action.MethodSignature + "' in class '" + caller.GetType().ToString() + "'.");

			runContext.printStatus("Running method: " + caller.GetType().Name + "." + action.CallSignature);
			methodToRun.Invoke(caller, action.Args);

            runContext.QueueTimerTask(LaunchNextAction, _actionDelay);
            return null;
		}

		/// <summary>
		/// Context inwhich to execute the script.
		/// </summary>
		TestSuite runContext;

		/// <summary>
		/// If True, call TestSuite.PassTest if script runs without exception.
		/// </summary>
		bool passOnFinish = false;

		/// <summary>
		/// Current script that is being run.
		/// </summary>
		AsyncTestScript _script;

        /// <summary>
        /// How long to wait before queueing the next test action.
        /// 0 by default.
        /// </summary>
        public TimeSpan ActionDelay
        {
            set
            {
                _actionDelay = value;
            }
        }
        private TimeSpan _actionDelay = new TimeSpan(0);
    }
}
