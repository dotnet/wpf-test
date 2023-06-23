using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Drawing.Printing;

using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Log;
using WFCTestLib.Util;

using StringTable = WFCTestLib.Util.StringTable;

using System.ComponentModel;

namespace ReflectTools.AutoPME
{
	/// <summary>
	/// AutoTest is an extension of AutoPME which uses Reflection to automatically test certain
	/// properties with pre-defined scenarios.  Currently AutoTest tests read/write properties of
	/// types bool, enum, and string.
	///
	/// Added 4/24/01: Tests event add/remove methods using the OnXxxx() methods to fire events.
	///                AutoTest puts the "E" in AutoPME!
	///
	/// To use AutoTest, simply have your test subclass AutoTest.  XObject subclasses AutoTest
	/// so all AutoPME tests will get this functionality for free.
	///
	/// Occasionally, a property will behave differently than AutoTest expects and will fail
	/// the auto test.  If this is the case, you should add this property's name to the
	/// ExcludedProperties collection.  AutoTest skips any properties in this list.
	/// </summary>
	public abstract class AutoTest : AutoPME
	{
		private StringTable excludedProperties = new StringTable();
		private StringTable excludedEvents = new StringTable();

		// PropertyChanged event testing in progress (NYI)
		//		private StringTable excludedPropertyChangedEvents = new StringTable();

		private PropertyInfo[] properties;
		private EventInfo[] events;

		// For counting events fired
		private static AutoTestEventCounter eventCounter = new AutoTestEventCounter();

		// Command-line flags
		private bool noAutoProp = false;
		private bool noAutoEvent = false;
		private bool noEnumBoundsTest = false;
		private bool ignoreMissingDelegates = false;

		const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
		const BindingFlags LookupAll = DefaultLookup | BindingFlags.NonPublic;

		/// <summary>
		/// Construct an AutoTest object.
		/// </summary>
		/// <param name="args">Command-line parameters.</param>
		protected AutoTest(string[] args) : base(args) { }

		protected override void InitTest(TParams p)
		{
			base.InitTest(p);
			properties = Class.GetProperties();
			events = Class.GetEvents();
		}


		//
		// (KevinTao 9/13/02)
		// TODO: Remove this when work items to add unimplemented scenarios are complete.
		//       We're temporarily disabling missing scenario failures for early M1 Whidbey
		//       nightlies (so they don't skew failure rates).
		//
		// (KevinTao 5/29/03): Removed.
		//
		//        protected override void SetOptions() {
		//            base.SetOptions();
		//            IgnoreMissing = true;
		//            IgnoreMissingDelegates = true;
		//        }

		//
		// The list of properties to exclude from auto-testing.  Occasionally, a property doesn't
		// behave like other properties of its type or shouldn't be auto-tested for whatever
		// reason.  AutoTest will ignore any properties in this list.
		//
		// In InitTest(), add the names of any properties you want excluded from auto-testing.
		//
		protected StringTable ExcludedProperties
		{
			get { return excludedProperties; }
		}

		//
		// Same as ExcludedProperties, except for events.
		//
		protected StringTable ExcludedEvents
		{
			get { return excludedEvents; }
		}

		//
		// Same as ExcludedPropertyChangedEvents, except for property changed events.  This only
		// affects the AutoTestPropertyChangedEvents autotest scenario.
		//
		//        protected StringTable ExcludedPropertyChangedEvents {
		//            get { return excludedEvents; }
		//        }

		//
		// If true, AutoTest's property tests will not be executed.
		//
		protected bool NoAutoProp
		{
			get { return noAutoProp; }
			set { noAutoProp = value; }
		}

		//
		// If true, AutoTest's event test will not be executed.
		//
		protected bool NoAutoEvent
		{
			get { return noAutoEvent; }
			set { noAutoEvent = value; }
		}

		//
		// If true, AutoTestEvents() ignores events which are missing delegate methods.
		//
		protected bool IgnoreMissingDelegates
		{
			get { return ignoreMissingDelegates; }
			set { ignoreMissingDelegates = value; }
		}

		//
		// If true, AutoTest will not check that out-of-bound enums throw an exception.
		// We need this because 3rd party controls tend to not check for out-of-bound
		// enums like our controls do and we still want to autotest them.
		//
		protected bool NoEnumBoundsTest
		{
			get { return noEnumBoundsTest; }
			set { noEnumBoundsTest = value; }
		}

		protected override void ProcessCommandLineParameters()
		{
			ArrayList paramsToRemove = new ArrayList();

			foreach (string arg in CommandLineParameters)
			{
				string argUpper = arg.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

				if (argUpper.StartsWith("/NOAUTOPROP"))
				{
					NoAutoProp = true;
					paramsToRemove.Add(arg);
				}
				else if (argUpper.StartsWith("/NOAUTOEVENT"))
				{
					NoAutoEvent = true;
					paramsToRemove.Add(arg);
				}
				else if (argUpper.StartsWith("/NOAUTO"))
				{
					NoAutoProp = true;
					NoAutoEvent = true;
					paramsToRemove.Add(arg);
				}
				else if (argUpper.StartsWith("/NOENUMBOUNDS"))
				{
					NoEnumBoundsTest = true;
					paramsToRemove.Add(arg);
				}
				else if (argUpper.StartsWith("/IGNOREMISSINGDELEGATES"))
				{
					IgnoreMissingDelegates = true;
					paramsToRemove.Add(arg);
				}
			}

			foreach (string arg in paramsToRemove)
				CommandLineParameters.Remove(arg);

			base.ProcessCommandLineParameters();
		}

		protected override void PrintHelp()
		{
			base.PrintHelp();
			Console.WriteLine();
			Console.WriteLine("AutoTest command-line parameters:");
			Console.WriteLine("  /noautoprop                Do not run AutoTest property tests");
			Console.WriteLine("  /noautoevent               Do not run AutoTest event tests");
			Console.WriteLine("  /noauto                    Do not run any AutoTest tests");
			Console.WriteLine("  /noenumbounds              Do not check for out-of-bound enum values");
			Console.WriteLine("  /ignoremissingdelegates    Ignore events which don't have delegates defined");
		}

		protected override void LogCommandLineParameters()
		{
			base.LogCommandLineParameters();

			if (NoAutoProp != false)
				LogCommandLineParameter("noautoprop", NoAutoProp.ToString());

			if (NoAutoEvent != false)
				LogCommandLineParameter("noautoevent", NoAutoEvent.ToString());

			if (NoEnumBoundsTest != false)
				LogCommandLineParameter("noenumbounds", NoEnumBoundsTest.ToString());

			if (IgnoreMissingDelegates != false)
				LogCommandLineParameter("ignoremissingdelegates", IgnoreMissingDelegates.ToString());
		}

		private void HandleException(TParams p, ScenarioResult result, Exception e)
		{
			p.log.WriteLine("Exception caught: " + e.ToString());

			// We'll ignore security exceptions since they should be tested in the X-test itself.
			if (e is SecurityException)
				p.log.WriteLine("Caught SecurityException.  Ignoring this property.");
			else
				result.IncCounters(false, "FAIL: caught unexpected exception " + e.GetType().ToString(), p.log);
		}

		//
		// Automatically tests all boolean properties on the target class. Sets each enum property to
		// true, then false, verifies that after each set the value "stuck".
		//
		[PreHandleScenario(true)]
		protected virtual ScenarioResult AutoTestBoolProperties(TParams p)
		{
			if (NoAutoProp)
			{
				p.log.WriteLine("NoAutoProp flag specified.  Skipping property AutoTest.");
				return ScenarioResult.Pass;
			}

			// Start passing by default in case there aren't any bool props
			ScenarioResult result = new ScenarioResult(true);

			foreach (PropertyInfo pi in properties)
			{
				string name = pi.Name;

				// Skip any properties that aren't bools or are read-only
				if (pi.PropertyType != typeof(bool) || !pi.CanWrite)
					continue;

				// Skip excluded properties
				if (excludedProperties.Contains(name))
				{
					p.log.WriteLine("*****Excluding {0}.{1}*****\r\n", pi.DeclaringType, name);
					continue;
				}

				MethodInfo getMethod = pi.GetGetMethod();
				MethodInfo setMethod = pi.GetSetMethod();

				p.log.WriteLine("*****Testing {0}.{1}", pi.DeclaringType, name);

				//
				// Perform automatic tests
				//
				try
				{
					bool returned = (bool)getMethod.Invoke(p.target, null);
					p.log.WriteLine("get returned " + returned);

					TestBoolPropGetSet(p, getMethod, setMethod, result, true);
					TestBoolPropGetSet(p, getMethod, setMethod, result, false);

					// Restore the original value so subsequent AutoTests aren't messed up.
					TestBoolPropGetSet(p, getMethod, setMethod, result, returned);
					ScenarioEpilog();
				}
				catch (TargetInvocationException e)
				{
					HandleException(p, result, e.InnerException);
				}

				p.log.WriteLine("*****\r\n");
			}

			return result;
		}

		//
		// Perform the work of setting the property to a value, getting the value, and verifying
		// it matches what was set.
		//
		private void TestBoolPropGetSet(TParams p,
										MethodInfo getMethod,
										MethodInfo setMethod,
										ScenarioResult result,
										bool value)
		{
			bool returned;
			string propertyName = getMethod.Name;
			if (propertyName.StartsWith("get_")) { propertyName = propertyName.Substring("get_".Length); }

			setMethod.Invoke(p.target, new object[] { value });
			returned = (bool)getMethod.Invoke(p.target, null);
			p.log.WriteLine("set to {0,-15} get returned {1}", value, returned);
			result.IncCounters(returned == value, "FAIL: (" + propertyName + ") set to " + value + "; get returned " + returned, p.log);
		}

		//
		// Performs 2 automatic tests on each enum property:
		//      1) Set property to every valid value
		//      2) Set property to lower bound - 1 and upper bound + 1 and verify exception
		//
		// TODO: Doesn't do any special handling of bit-flag enums.
		//
		[PreHandleScenario(true)]
		protected virtual ScenarioResult AutoTestEnumProperties(TParams p)
		{
			if (NoAutoProp)
			{
				p.log.WriteLine("NoAutoProp flag specified.  Skipping property AutoTest.");
				return ScenarioResult.Pass;
			}

			// Start passing by default in case there aren't any enum props
			ScenarioResult result = new ScenarioResult(true);

			foreach (PropertyInfo pi in properties)
			{
				string name = pi.Name;

				// Skip any properties that aren't enums or are read-only
				if (!typeof(Enum).IsAssignableFrom(pi.PropertyType) || !pi.CanWrite)
					continue;

				// Skip excluded properties
				if (excludedProperties.Contains(name))
				{
					p.log.WriteLine("*****Excluding {0}.{1}*****\r\n", pi.DeclaringType, name);
					continue;
				}

				// Skip [Flags] enums
				object[] flagsAttributeArray = pi.PropertyType.GetCustomAttributes(typeof(FlagsAttribute), true);
				if (flagsAttributeArray.Length > 0)
				{
					p.log.WriteLine("*****Excluding {0}.{1} (Flags enum)*****\r\n", pi.DeclaringType, name);
					continue;
				}

				MethodInfo getMethod = pi.GetGetMethod();
				MethodInfo setMethod = pi.GetSetMethod();

				p.log.WriteLine("*****Testing {0}.{1}", pi.DeclaringType, name);

				//
				// Perform automatic tests
				//
				Array values = Enum.GetValues(pi.PropertyType);
				Enum returned;

				// 1) Set to each valid enum value, verify get returns correct value
				try
				{
					p.log.WriteLine("1. Test each valid value");

					returned = (Enum)getMethod.Invoke(p.target, null);
					p.log.WriteLine("get returned " + returned.ToString());

					foreach (Enum e in values)
						TestEnumPropGetSet(p, getMethod, setMethod, result, e);

					// Restore the original value so subsequent AutoTests aren't messed up.
					TestEnumPropGetSet(p, getMethod, setMethod, result, returned);
					ScenarioEpilog();
				}
				catch (TargetInvocationException e)
				{
					HandleException(p, result, e.InnerException);
				}

				// 2) Set to 1 below the min value and 1 above the max, verify exception thrown
				if (!NoEnumBoundsTest)
				{
					p.log.WriteLine();
					p.log.WriteLine("2. Test lb - 1, ub + 1");
					int value;
					Array.Sort(values);

					value = ((int)values.GetValue(0)) - 1;                    // lower bound - 1
					TestEnumPropInvalidInt(p, getMethod, setMethod, result, value);

					value = ((int)values.GetValue(values.Length - 1)) + 1;    // upper bound + 1
					TestEnumPropInvalidInt(p, getMethod, setMethod, result, value);
				}

				p.log.WriteLine("*****\r\n");
			}

			return result;
		}

		//
		// Perform the work of setting the property to a valid value, getting the value, and verifying
		// it matches what was set.
		//
		private void TestEnumPropGetSet(TParams p,
										MethodInfo getMethod,
										MethodInfo setMethod,
										ScenarioResult result,
										Enum value)
		{
			Enum returned;
			string propertyName = getMethod.Name;
			if (propertyName.StartsWith("get_")) { propertyName = propertyName.Substring("get_".Length); }

			setMethod.Invoke(p.target, new object[] { value });
			returned = (Enum)getMethod.Invoke(p.target, null);
			p.log.WriteLine("set to {0,-15} get returned {1}", value.ToString(), returned.ToString());
			result.IncCounters(returned.Equals(value), "FAIL: (" + propertyName + ") set to " + value.ToString() +
				"; get returned " + returned.ToString(), p.log);
		}

		//
		// Set the property to an invalid value and verify an ArgumentException (or subclass) is
		// thrown.  Value used must be an invalid enum or this method will produce faulty results.
		//
		private void TestEnumPropInvalidInt(TParams p,
											MethodInfo getMethod,
											MethodInfo setMethod,
											ScenarioResult result,
											int value)
		{
			Enum orig = (Enum)getMethod.Invoke(p.target, null);
			Enum returned;

			try
			{
				setMethod.Invoke(p.target, new object[] { Enum.ToObject(getMethod.ReturnType, value) });
				result.IncCounters(false, "FAIL: set to " + value + "; No exception thrown", p.log);
			}
			catch (TargetInvocationException te)
			{
				bool passed = true;
				Exception e = te.InnerException;

				p.log.WriteLine("Caught {0}: {1}", e.GetType().ToString(), e.Message);
				returned = (Enum)getMethod.Invoke(p.target, null);

				// Fail if we threw the wrong type of exception
				if (!(e is ArgumentException))
				{
					passed = false;
					p.log.WriteLine("FAIL: Didn't throw an ArgumentException.  e.ToString = " + e.ToString());
				}

				// Fail if we threw an exception but the value still got set
				if (!returned.Equals(orig))
				{
					passed = false;
					p.log.WriteLine("FAIL: Value changed to " + returned.ToString() + " when invalid value set");
				}

				result.IncCounters(passed, "FAIL: Failed set invalid enum value", p.log);
			}
		}

		//
		// Performs 4 automatic tests on string properties:
		//      1) Set the string to null
		//      2) Set the string to an empty string
		//      3) Set the string to a short random string
		//      4) Set the string to a long random string
		//
		[PreHandleScenario(true)]
		protected virtual ScenarioResult AutoTestStringProperties(TParams p)
		{
			if (NoAutoProp)
			{
				p.log.WriteLine("NoAutoProp flag specified.  Skipping property AutoTest.");
				return ScenarioResult.Pass;
			}

			const int ShortStringLen = 15;
			const int LongStringLen = 500;

			// Start passing by default in case there aren't any string props
			ScenarioResult result = new ScenarioResult(true);

			foreach (PropertyInfo pi in properties)
			{
				string name = pi.Name;

				// Skip any properties that aren't string or are read-only
				if (pi.PropertyType != typeof(string) || !pi.CanWrite)
					continue;

				// Skip excluded properties
				if (excludedProperties.Contains(name))
				{
					p.log.WriteLine("*****Excluding {0}.{1}*****\r\n", pi.DeclaringType, name);
					continue;
				}

				MethodInfo getMethod = pi.GetGetMethod();
				MethodInfo setMethod = pi.GetSetMethod();

				p.log.WriteLine("*****Testing {0}.{1}", pi.DeclaringType, name);

				//
				// Perform automatic tests
				//
				try
				{
					string returned = (string)getMethod.Invoke(p.target, null);
					p.log.WriteLine("get returned \"{0}\"", returned);

					TestStringPropGetSet(p, getMethod, setMethod, result, null);
					TestStringPropGetSet(p, getMethod, setMethod, result, "");
					TestStringPropGetSet(p, getMethod, setMethod, result, p.ru.GetString(ShortStringLen));
					TestStringPropGetSet(p, getMethod, setMethod, result, p.ru.GetString(LongStringLen));

					// Restore the original value so subsequent AutoTests aren't messed up.
					TestStringPropGetSet(p, getMethod, setMethod, result, returned);
					ScenarioEpilog();
				}
				catch (TargetInvocationException e)
				{
					HandleException(p, result, e.InnerException);
				}

				p.log.WriteLine("*****\r\n");
			}

			return result;
		}

		//
		// Perform the work of setting the property to a value, getting the value, and verifying
		// it matches what was set.
		//
		private void TestStringPropGetSet(TParams p,
										  MethodInfo getMethod,
										  MethodInfo setMethod,
										  ScenarioResult result,
										  string value)
		{
			string returned;
			string comment;

			string propertyName = getMethod.Name;
			if(propertyName.StartsWith("get_")){ propertyName = propertyName.Substring("get_".Length); }

			setMethod.Invoke(p.target, new object[] { value });
			returned = (string)getMethod.Invoke(p.target, null);

			// special case setting null string
			if (value == null)
			{
				comment = "set to null; get returned " +
					(returned == null ? "null" : ("\"" + returned + "\""));
				p.log.WriteLine(comment);
				result.IncCounters(returned == value || returned == "", "FAIL: (" + propertyName + ") " + comment, p.log);
			}
			else
			{
				comment = String.Format("set to \"{0}\"; get returned \"{1}\"", value, returned);
				p.log.WriteLine(comment);

				if (returned != value)
				{
					p.log.WriteLine("    Strings didn't match.  Chars different between value set and returned value:");

					foreach (string s in Utilities.DiffStrings(value, returned))
						p.log.WriteLine(s);
				}
				result.IncCounters(returned == value, "FAIL: (" + propertyName + ") " + comment, p.log);
			}
		}

		private MethodInfo GetCustomRaiseMethod(string eventName)
		{
			MethodInfo mi = this.GetType().GetMethod("Raise" + eventName, LookupAll);

			if (mi == null)
				return null;

			ParameterInfo[] pi = mi.GetParameters();

			if (pi.Length != 1 || pi[0].ParameterType != typeof(TParams))
				return null;

			return mi;
		}

		//
		// Automatically tests all event add/remove methods.  This is how it works for an
		// event named Foo of type BarEventHandler:
		//  1) Adds an event handler to Foo using the required delegate method BarEventHandler()
		//     in the AutoTestEventDelegates class (this can also go in the test class itself).
		//  2) If there is a method in the test class with the signature RaiseFoo(TParams p),
		//     call that method.  Else, if there is a method called OnFoo() on the target class,
		//     call that.  Else fail.
		//  3) Pass if the event was raised and Sender == the target object, else fail.
		//  4) Remove the event handler.
		//  5) Call RaiseFoo() if it exists, else call OnFoo().
		//  6) Pass if the event was not raised, else fail.
		//
		// Delegate Methods
		// ================
		// Each type of event delegate to be tested (e.g. EventHandler, CancelEventHandler, etc.)
		// is required to have a delegate method defined in the AutoTestEventDelegates.class.
		// The method must be static and must have the same name as the delegate type (e.g. EventHandler).
		//
		// Custom Raise Methods
		// ====================
		// If the event needs to be raised in a special way (e.g. there is no OnFoo() method, or
		// special arguments must be passed to OnFoo()), you may declare a method with the name
		// RaiseXxxx() (in this case, RaiseFoo()).  From there, you may do whatever is necessary to
		// raise the event.
		//
		[PreHandleScenario(true)]
		[SecurityPermission(SecurityAction.Assert, Unrestricted =true)]
		protected virtual ScenarioResult AutoTestEvents(TParams p)
		{
			if (NoAutoEvent)
			{
				p.log.WriteLine("NoAutoEvent flag specified.  Skipping event AutoTest.");
				return ScenarioResult.Pass;
			}
			ScenarioResult result = AutoTestEventsActual(p);

			return result;
		}
[Scenario(false)]
		private ScenarioResult AutoTestEventsActual(TParams p)
		{

			// Start passing by default in case there aren't any events
			ScenarioResult result = new ScenarioResult(true);

			foreach (EventInfo info in events)
			{
				string name = info.Name;
				Type handlerType = info.EventHandlerType;
				MethodInfo customRaiseMethod = null;   // We'll use customRaiseMethod or onMethod, but not both.
				MethodInfo onMethod = null;

				// Skip excluded properties
				if (excludedEvents.Contains(name))
				{
					p.log.WriteLine("*****Excluding {0}.{1}*****\r\n", info.DeclaringType, name);
					continue;
				}
				else
					p.log.WriteLine("*****Testing {0}.{1}", info.DeclaringType, name);

				// Make sure we can test this event. (2 conditions)
				// 1. There needs to be either a RaiseFoo() event on this test class, or an
				//    OnFoo() event on the target class.
				customRaiseMethod = GetCustomRaiseMethod(info.Name);

				if (customRaiseMethod != null)
					p.log.WriteLine("Using custom method Raise" + info.Name + "() to raise event.");
				else
				{
					onMethod = Class.GetMethod("On" + info.Name, LookupAll);

					if (onMethod == null)
					{
						result.IncCounters(false, "FAIL: No On" + info.Name + "() method found.  Exclude this event from AutoTest or define a Raise" + info.Name + "() method.", p.log);
						p.log.WriteLine();
						continue;
					}
				}

				Debug.Assert((onMethod != null && customRaiseMethod == null) ||
							 (onMethod == null && customRaiseMethod != null),
							  "onMethod or customRaiseMethod must be non-null and the other must be null");

				// 2. Has to have a delegate method defined
				//
				// KevinTao 10/24/01:
				//     Now you can define this in the test class itself.  Makes it easier to test
				//     events which take arguments that are defined in another assembly (e.g.
				//     an ActiveX event test).
				//
				//     Now we search the test class first, then if the delegate method is not found, we
				//     search AutoTestEventDelegates.  This allows a test to define event delegates, or
				//     "override" the one from AutoTestEventDelegates if necessaary for some reason.
				//  

				DynamicDelegate.EventHandlerRef token = null;
				MethodInfo mi;
				object declaringType;
				object[] parameters;
				try
				{
					token = DynamicDelegate.AddHandler(p.target, info, new DelegateHandler(AutoTestEventHandler));

					// If we're using the custom raise method, invoke it, else create a
					// fake EventArgs using CreateInstance() and call the OnFoo() method.
					try
					{

						if (customRaiseMethod != null)
						{  // set up for invoking the custom raise method
							mi = customRaiseMethod;
							declaringType = this;
							parameters = new object[] { p };
						}
						else
						{                              // set up for invoking the On method
							ParameterInfo[] pis;
							mi = onMethod;
							pis = onMethod.GetParameters();
							declaringType = p.target;
							parameters = new object[pis.Length];

							// Create fake parameters for the On method's parameters
							for (int i = 0; i < pis.Length; i++)
							{
								if (pis[i].ParameterType == typeof(HtmlElementEventArgs))
									parameters[i] = null;
								else
									parameters[i] = CreateInstance(pis[i].ParameterType, false);
							}
						}

						// 1. Verify event raised when event handler is attached
						p.log.WriteLine("1. Verify event raised when event handler is attached");
						eventCounter.Reset();
						mi.Invoke(declaringType, parameters);

						//RickyT 8/10/07: OnGotFocus is expected to be raised twice
						if(info.Name == "GotFocus" && eventCounter.TimesFired == 2)
						{

							eventCounter.TimesFired = 1;
						}

						result.IncCounters(eventCounter.TimesFired == 1, "FAIL: Event raised " + eventCounter.TimesFired + " times.", p.log);
						result.IncCounters(eventCounter.Sender == p.target, "FAIL: Expected sender = " + p.target + ", but got " + eventCounter.Sender, p.log);
					}
					finally
					{
						if (null != token)
						{ token.DetachHandler(); }
						//// Remove event handler
						//info.RemoveEventHandler(p.target, handler);

					}

					// 2. Verify event not raised after event handler is removed
					p.log.WriteLine("2. Verify event not raised after event handler is removed");
					eventCounter.Reset();
					mi.Invoke(declaringType, parameters);
					result.IncCounters(eventCounter.TimesFired == 0, "FAIL: Event raised " + eventCounter.TimesFired + " times.", p.log);
					//Add this code to try to get any exceptions caused by hooking this event to occur within this sub-scenario
					ScenarioEpilog();
				}
				catch (TargetInvocationException e)
				{
					HandleException(p, result, e.InnerException);
				}

				p.log.WriteLine("*****\r\n");
			}
			return result;
		}

		private static void ScenarioEpilog()
		{
			Application.DoEvents();
			System.Threading.Thread.Sleep(1);
			Application.DoEvents();
		}

		//
		// The OnFoo() methods are protected.  You can use this method to invoke one for your
		// event.  E.g. InvokeOnEventMethod(myButton, "Click", new object[] { new EventArgs() });
		// will execute the OnClick() event on myButton.
		//
		protected void InvokeOnEventMethod(object target, string eventName, object[] args)
		{
			InvokeMethod(target, "On" + eventName, args);
		}

		//
		// Invokes any method on a given class.
		//
		[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
		protected void InvokeMethod(object target, string methodName, object[] args)
		{
			MethodInfo mi = target.GetType().GetMethod(methodName, LookupAll);

			if (mi == null)
				throw new NotImplementedException("Method " + methodName + "() was not found on class " + Class);

			try
			{
				mi.Invoke(target, args);
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		public static object AutoTestEventHandler(object[] parameters)
		{
			eventCounter.RaiseEvent(parameters.Length > 1 ? parameters[1] : null, parameters.Length > 2 ? parameters[2] as EventArgs : null);
			return null;
		}
		//
		// Call this from the event delegate methods to record the event firing.
		//
		internal protected static void EventRaised(object sender, EventArgs e)
		{
			eventCounter.RaiseEvent(sender, e);
		}

		//
		// Subclasses WFCTestLib.Util.EventCounterBase.  Inherits members TimesFired
		// and Reset().
		//
		class AutoTestEventCounter : EventCounterBase
		{
			private EventArgs eventArgs;
			private object sender;

			public EventArgs EventArgs
			{
				get { return eventArgs; }
			}

			public object Sender
			{
				get { return sender; }
			}

			public override void Reset()
			{
				base.Reset();
				eventArgs = null;
				sender = null;
			}

			public void RaiseEvent(object sender, EventArgs e)
			{
				++TimesFired;
				this.sender = sender;
				eventArgs = e;
			}
		}

		/// <summary>
		/// Verifies that property changed events have the same browser and editor
		/// visibility as the corresponding property.
		/// </summary>
		//		protected virtual ScenarioResult AutoTestPropertyChangedEvents() {
		//			return null;
		//		}
	}
}
