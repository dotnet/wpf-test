using System;
using System.Collections.Generic;
using System.Text;
using ReflectTools;
using WFCTestLib.Log;
using System.Reflection;
using System.Windows.Forms;

namespace WFCTestLib.Util
{
	/// <summary>
	/// Contains data about a specific time that an event fired.
	/// </summary>
	public struct EventData
	{
        /// <summary>
        /// Format string which displays the name of the sender before the event name 
        /// </summary>
        public const string SenderPrefixedFormatString = @"({SenderName}){EventName}";

        /// <summary>
        /// Format string which displays the event arg type after the EventName
        /// </summary>
        public const string EventArgTypeFormatString = @"{EventName}[{E.GetType}]";
        
        /// <summary>
		/// Constructs a new EventData struct
		/// </summary>
		/// <param name="eventName">The name of the event which was fired.</param>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		public EventData(string eventName, object sender, EventArgs e)
		{
			_EventTime = DateTime.Now;
			_Sender = sender;
			_E = e;
			_EventName = eventName;
			_ManagedThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
		}
		private int _ManagedThreadId;
		/// <summary>
		/// Gets the managed thread id on the thread which fired the event.
		/// </summary>
		public int ManagedThreadId
		{ get { return _ManagedThreadId; } }
		private DateTime _EventTime;
		/// <summary>
		/// Gets the time at which the event was fired.
		/// </summary>
		public DateTime EventTime
		{ get { return _EventTime; } }

		private object _Sender;
		/// <summary>
		/// Gets the Sender of the event.
		/// Assumes that the first parameter of event signature was the sender.
		/// </summary>
		public object Sender
		{ get { return _Sender; } }
		private EventArgs _E;
		/// <summary>
		/// Gets the EventArgs of the event.
		/// Assumes the second parameter of the event signature was of type EventArgs.
		/// </summary>
		public EventArgs E
		{ get { return _E; } }
		private string _EventName;
		/// <summary>
		/// Gets the name of the event that fired.
		/// This is extracted from the MetaData.
		/// </summary>
		public string EventName
		{ get { return _EventName; } }

		/// <summary>
		/// Creates a human-readable representation of the event.  Ideal for outputting to log files.
		/// </summary>
		/// <returns>A human-readable string.</returns>
		public override string ToString()
		{
			string senderString = Sender.ToString() + "[" + (object.ReferenceEquals(null, Sender) ? "UNKNOWN" : Sender.GetType().Name) + "]";
			return string.Format("Event: {0} By: {1} Args: {2} At: {3}",
				PadToSize(EventName, 20),
				PadToSize(senderString, 25),
				E.ToString(),
				EventTime.ToString("hh:mm:ss.fffffff"));
		}
		private string PadToSize(string s, int size)
		{
			if (string.IsNullOrEmpty(s)) { return new String(' ', size); }
			if (s.Length < size)
			{ return s + new String(' ', size - s.Length); }
			else
			{ return s; }
		}

        public string SenderName
        {
            get
            {
                if (null != Sender)
                {
                    Type senderType = Sender.GetType();
                    PropertyInfo pi = senderType.GetProperty("Name", typeof(string));
                    if (null != pi)
                    {
                        string name = pi.GetValue(Sender, null) as string;
                        if (null != name)
                        { return name; }
                    }
                }
                return "UnknownSender";
            }
        }
	}
	/// <summary>
	/// Event args used when an event is fired by EventListener.
	/// </summary>
	public class EventFiredEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the EventData for the event which was fired.
		/// </summary>
		public readonly EventData EventInfo;

		public EventFiredEventArgs(EventData eventInfo)
		{ EventInfo = eventInfo; }
	}

	/// <summary>
	/// A collection of <see cref="EventData"/> items.
	/// Contains methods for generating subsets and verifying event ordering.
	/// </summary>
	public class EventDataList : List<EventData>
	{
		/// <summary>
		/// Constructs an empty Event DataList.
		/// </summary>
		public EventDataList() : base() { }
		/// <summary>
		/// Constructs copy of the specified EventDataList.
		/// </summary>
		/// <param name="source">The EventDataList to copy.</param>
		public EventDataList(EventDataList source) : base(source) { }

		/// <summary>
		/// Returns an EventDataList which is a proper subset of this list.
		/// The predicate parameter is used to identify which items will be present in the subset.
		/// To return all events with a specified sender, you could do this.
		/// <example>
		/// public static EventDataList GetSubsetOfSender(EventDataList list, object o)
		/// {
		/// 	return list.GetSubset(delegate(EventData ed) { return ed.Sender == o; });
		/// }
		/// </example>
		/// </summary>
		/// <param name="predicate">A predicate which returns true for each item which should be in the subset.</param>
		/// <returns>A subset of the current list.</returns>
		public EventDataList GetSubset(Predicate<EventData> predicate)
		{
			if (null == predicate)
			{ throw new ArgumentNullException("predicate"); }
			EventDataList ret = new EventDataList();
			foreach (EventData ed in this)
			{
				if (predicate(ed))
				{ ret.Add(ed); }
			}
			return ret;
		}

		/// <summary>
		/// Gets a list of all EventArgs of the specified type.
		/// For example you could return all of the MouseEventArgs in event order.
		/// <example>
		/// <![CDATA[
		/// GetArgs<MouseEventArgs>();
		/// ]]>
		/// </example>
		/// </summary>
		/// <typeparam name="T">The type of EventArgs to return a list of.</typeparam>
		/// <returns>A list of EventArgs-derived items.</returns>
		public List<T> GetArgs<T>()
			where T : EventArgs
		{
			List<T> ret = new List<T>();
			foreach (EventData ed in this)
			{
				if (ed.E is T)
				{ ret.Add((T)ed.E); }
			}
			return ret;
		}
		/// <summary>
		/// Gets the most recent EventArgs of the specified type.
		/// For example you could return the most recent MouseEventArgs.
		/// <example>
		/// <![CDATA[
		/// GetLastArg<MouseEventArgs>();
		/// ]]>
		/// </example>
		/// </summary>
		/// <typeparam name="T">The type of EventArgs.</typeparam>
		/// <returns>The most recent eventArgs of the specified type.</returns>
		public T GetLastArg<T>()
			where T : EventArgs
		{
			return (T)this.FindLast(delegate(EventData ed) { return (ed.E is T); }).E;
		}

		/// <summary>
		/// Gets a subset of the events in the list matching the specified event names.
		/// </summary>
		/// <param name="eventName">The list of eventNames for which all events should be returned</param>
		/// <returns>The subset of EventData objects caused by the specified events.</returns>
		public EventDataList GetSubset(params string[] eventName)
		{
			if (null == eventName || eventName.Length == 0)
			{ throw new ArgumentException("Specify at least one event name"); }
			return GetSubset(
				delegate(EventData ed)
				{
					return Array.Exists(eventName,
						delegate(string s)
						{
							return s == ed.EventName;
						});
				});
		}
		/// <summary>
		/// Verifies that all of the events in this list correspond to a certain order.
		/// Specifically, it validates the following assumptions.
		/// 1. That both firstEvent and secondEvent are present in the EventList.
		/// 2. That, for each instance of firstEvent, there is an instance of secondEvent before another instance of firstEvent.
		/// 3. That no secondEvent occurs that is not preceded by a firstEvent.
		/// 
		/// For example where 
		/// MouseDown:MouseUp:MouseClick:MouseDown:MouseUp:MouseDoubleClick
		/// firstEvent=MouseDown and secondEvent=MouseUp is valid
		/// firstEvent=MouseClick and secondEvent=MouseDoubleClick is valid
		/// but if
		/// MouseDown:MouseDown:MouseUp:MouseClick:MouseUp:MouseDoubleClick
		/// firstEvent=MouseDown and secondEvent=MouseUp is INVALID
		/// firstEvent=MouseClick and secondEvent=MouseDoubleClick is valid
		/// </summary>
		/// <param name="p">Used for logging.</param>
		/// <param name="firstEvent">Specifies an event which should come before the secondEvent.</param>
		/// <param name="secondEvent">An event which should come after the firstEvent.</param>
		/// <returns>A ScenarioResult indicating the success or failure of the verification.</returns>
		public ScenarioResult VerifyOrder(TParams p, string firstEvent, string secondEvent)
		{
			if (string.IsNullOrEmpty(firstEvent)) { throw new ArgumentNullException("firstEvent"); }
			if (string.IsNullOrEmpty(secondEvent)) { throw new ArgumentNullException("secondEvent"); }
			Predicate<EventData> beforeComparer = CreateComparer(firstEvent);
			Predicate<EventData> afterComparer = CreateComparer(secondEvent);

			ScenarioResult ret = new ScenarioResult();
			//Ensure that the firstEvent is present
			ret.IncCounters(this.Exists(beforeComparer), "FAIL: expected event not fired", p.log);
			//Ensure that the secondEvent is present
			ret.IncCounters(this.Exists(afterComparer), "FAIL: expected event not fired", p.log);
			//verify the order 
			//build a state machine with two states (expectBeforeEvent=[true|false])
			bool expectBeforeEvent = true;
			bool pass = true;
			foreach (EventData ed in this)
			{
				string expectedEvent = (expectBeforeEvent ? firstEvent : secondEvent);
				string disallowedEvent = (!expectBeforeEvent ? firstEvent : secondEvent);

				if (ed.EventName == expectedEvent)
				{ expectBeforeEvent = !expectBeforeEvent; }
				else if (ed.EventName == disallowedEvent)
				{ pass = false; }
			}
			//Unpaired event found a before but not an after
			if (!expectBeforeEvent)
			{ pass = false; }
			ret.IncCounters(pass, "FAIL: Event order incorrect", p.log);
			if (!ret.IsPassing)
			{
				p.log.WriteLine("Expecting event sequence: {0}, {1}", firstEvent, secondEvent);
				LogEvents(p);
			}
			return ret;
		}
		private static Predicate<EventData> CreateComparer(string eventName)
		{ return (Predicate<EventData>)delegate(EventData compare) { return eventName == compare.EventName; }; }

		/// <summary>
		/// Verifies that the EventList has the specified number of events.
		/// </summary>
		/// <param name="p">Used for logging</param>
		/// <param name="expectedCount">The expected number of events firing.</param>
		/// <returns>A ScenarioResult.</returns>
		public ScenarioResult Verify(TParams p, int expectedCount)
		{
			return new ScenarioResult(expectedCount, this.Count, "FAIL: Event fired wrong number of times", p.log);
		}

		/// <summary>
		/// Outputs all of the events to the log specified by p.
		/// </summary>
		/// <param name="p">Used for logging</param>
		public void LogEvents(TParams p)
		{
			p.log.WriteLine("Actual Events (count:{0})", Count);
			foreach (EventData ed in this)
			{ p.log.WriteLine(ed.ToString()); }
		}
		/// <summary>
		/// Verifies that the sender is correct for all EventData in the list.
		/// </summary>
		/// <param name="p">Used for logging.</param>
		/// <param name="sender">The expected sender</param>
		/// <returns>A ScenarioResult.</returns>
		public ScenarioResult VerifySender(TParams p, object sender)
		{
			foreach (EventData ed in this)
			{
				if (ed.Sender != sender)
				{ return new ScenarioResult(sender, ed.Sender, "FAIL: Sender was incorrect", p.log); }
			}
			return ScenarioResult.Pass;
		}
		/// <summary>
		/// Verifies the sender, event count and event ordering of the EventList.
		/// For more complex event ordering testing see <see cref="VerifyOrder"/>
		/// </summary>
		/// <param name="p">Used for logging.</param>
		/// <param name="sender">The expected sender.</param>
		/// <param name="expectedCount">The expected event count.</param>
		/// <param name="eventOrder">The expected event order string.  Of the form "MouseDown:MouseUp:MouseClick:" (note the trailing colon).</param>
		/// <returns>A ScenarioResult.</returns>
		public ScenarioResult Verify(TParams p, object sender, int expectedCount, string eventOrder)
		{
			ScenarioResult ret = new ScenarioResult();
			ret.IncCounters(VerifySender(p, sender));
			ret.IncCounters(Verify(p, expectedCount));
			ret.IncCounters(eventOrder, this.ToString(), "FAIL: Event order was incorrect", p.log);
			if (!ret.IsPassing)
			{ LogEvents(p); }
			return ret;
		}

		/// <summary>
		/// The expected event order string.  Of the form "MouseDown:MouseUp:MouseClick:" (note the trailing colon).
		/// </summary>
		/// <returns>A string.</returns>
		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			foreach (EventData ed in this)
			{
				ret.Append(ed.EventName);
				ret.Append(":");
			}
			return ret.ToString();

		}

        public string ToString(string eventDataFormatString)
        {
            StringBuilder ret = new StringBuilder();
            foreach (EventData ed in this)
            {
                ret.Append(Utilities.FormatObject(ed, eventDataFormatString));
                ret.Append(":");
            }
            return ret.ToString();
        }
	}

	/// <summary>
	/// Provides a container which can be used to specify which events to attach.
	/// </summary>
	public struct EventAttachInfo
	{
		/// <summary>
		/// Creates a new EventAttachInfo based on the specified source and event name.
		/// </summary>
		/// <param name="source">The source object which is expected to fire the specified event.</param>
		/// <param name="eventName">The name of the event to attach.</param>
		public EventAttachInfo(object source, string eventName)
		{
			_Source = source;
			_EventInfo = source.GetType().GetEvent(eventName); ;
			Validate();
		}

		/// <summary>
		/// Creates a new EventAttachInfo based on the specified event info.
		/// To be used to listen to static events.
		/// </summary>
		/// <param name="eventInfo">The EventInfo for the event to listen to.</param>
		public EventAttachInfo(EventInfo eventInfo)
		{
			_Source = null;
			_EventInfo = eventInfo;
		}
		/// <summary>
		/// Validates that this instance of EventAttachInfo is a valid (that it refers to a real event).
		/// </summary>
		/// <exception cref="ArgumentException">thrown when the EventInfo is invalid</exception>
		public void Validate()
		{
			if (null == this._EventInfo)
			{ throw new ArgumentException("The specified event was not found on the source object"); }
			if (null == this.Source && !this._EventInfo.GetAddMethod().IsStatic)
			{ throw new ArgumentNullException("source"); }

		}
		/// <summary>
		/// Gets the name of the event to attach.
		/// </summary>
		public string EventName
		{
			get { return _EventInfo.Name; }
		}
		private object _Source;
		/// <summary>
		/// Gets the source object which should fire the event.
		/// </summary>
		public object Source
		{
			get { return _Source; }
		}

		private EventInfo _EventInfo;
		/// <summary>
		/// Gets the event info object for the event to attach.
		/// </summary>
		public EventInfo EventInfo
		{
			get { return _EventInfo; }
		}
	}
	/// <summary>
	/// Encapsulates the ability to listen to events fired by any object.
	/// </summary>
	public class EventListener : IDisposable
	{
		/// <summary>
		/// Constructs an EventListener object to listen to any number of events on any given
		/// number of source objects.  You might use this to attach events on a container and its items.
		/// <example>
		/// <![CDATA[
		/// ToolStrip ts = new ToolStrip();
		/// ToolStripItem tsi = new ToolStripButton();
		/// ts.Items.Add(tsi);
		/// EventListener e = new EventListener(
		///   new EventAttachInfo(ts, "ItemClicked"),
		///   new EventAttachInfo(tsi, "Click")
		/// );
		/// ]]></example>
		/// </summary>
		/// <param name="eventsToAttach"></param>
		public EventListener(params EventAttachInfo[] eventsToAttach)
		{
			foreach (EventAttachInfo eai in eventsToAttach)
			{
				//Ensure that the event is valid
				eai.Validate();
				DynamicDelegate.EventHandlerRef currentRef = null;
				currentRef = DynamicDelegate.AddHandler(eai.Source, eai.EventInfo, new DelegateHandler(UnTypedHandler));
				_HandlerReferences.Add(currentRef);
			}
		}
		/// <summary>
		/// Constructs a new EventListener and connects any number of events.
		/// </summary>
		/// <param name="source">The source object.</param>
		/// <param name="eventsToAttach">All of the events to attach.  Choose any number of events separated by commas.</param>
		public EventListener(object source, params string[] eventsToAttach)
		{
			if (null == source) { throw new ArgumentNullException("source"); }
			Type t = source.GetType();
			Initialize(source, Array.ConvertAll<string, EventInfo>(eventsToAttach,
				delegate(string before)
				{
					EventInfo inf = t.GetEvent(before);
					if (null == inf)
					{ throw new ArgumentException(string.Format("Event {0} could not be found on type {1}", before, t.Name)); }
					return inf;
				}));
		}
		/// <summary>
		/// Constructs a new EventListener and connects any number of events.
		/// </summary>
		/// <param name="source">The source object.</param>
		/// <param name="eventsToAttach">All of the events to attach.  Choose any number of events separated by commas.</param>
		public EventListener(object source, params EventInfo[] eventsToAttach)
		{
			if (null == source)
			{
				foreach (EventInfo ei in eventsToAttach)
				{
					if (!ei.GetAddMethod().IsStatic)
					{ throw new ArgumentNullException("source can not be null when there are non-static events"); }
				}
			}
			Initialize(source, eventsToAttach);
		}
		/// <summary>
		/// Gets the total count of Events fired.
		/// </summary>
		public int EventCount
		{ get { return _EventsFired.Count; } }
		/// <summary>
		/// Gets a string identifying the order in which the events were fired.
		/// Of the form "MouseDown:MouseUp:MouseClick:" (note the trailing colon).
		/// </summary>
		public string EventOrderString
		{
			get
			{ return _EventsFired.ToString(); }
		}

        /// <summary>
        /// Gets a string identifying the events which were fired.
        /// A specific format string is applied to each EventData object.
        /// </summary>
        /// <param name="formatString">The format string</param>
        /// <returns>A formatted event order string</returns>
        public string GetEventOrderString(string formatString)
        { return _EventsFired.ToString(formatString); }

		private EventDataList _EventsFired = new EventDataList();
		/// <summary>
		/// Gets the list of events fired.
		/// </summary>
		public EventDataList EventsFired
		{ get { return _EventsFired; } }

		private List<DynamicDelegate.EventHandlerRef> _HandlerReferences = new List<DynamicDelegate.EventHandlerRef>();

		private void Initialize(object source, EventInfo[] eventsToAttach)
		{
			foreach (EventInfo ei in eventsToAttach)
			{
				DynamicDelegate.EventHandlerRef currentRef = null;
				currentRef = DynamicDelegate.AddHandler(source, ei, new DelegateHandler(UnTypedHandler));
				_HandlerReferences.Add(currentRef);
			}
		}

		public void Clear()
		{ _EventsFired.Clear(); }

		private object UnTypedHandler(object[] parameters)
		{
			object sender = null;
			EventArgs e = EventArgs.Empty;
			string eventName = "";
			if (null != parameters && parameters.Length >= 1)
			{ eventName = (string)parameters[0]; }
			if (null != parameters && parameters.Length >= 2)
			{ sender = parameters[1]; }
			if (null != parameters && parameters.Length >= 3 && parameters[2] is EventArgs)
			{ e = (EventArgs)parameters[2]; }
			EventData eventData = new EventData(eventName, sender, e);
			_EventsFired.Add(eventData);
			OnEventFired(eventData);
			return null;
		}
		/// <summary>
		/// This event is fired whenever the EventListener handles an event which it is attached to.
		/// </summary>
		public event EventHandler<EventFiredEventArgs> EventFired;

		protected void OnEventFired(EventData eventInfo)
		{
			Log.Log tempLog = OutputLog;
			if (null != tempLog)
			{ tempLog.WriteLine(eventInfo.ToString()); }
			EventHandler<EventFiredEventArgs> handler = EventFired;
			if (null != handler)
			{ handler(this, new EventFiredEventArgs(eventInfo)); }
		}


		/// <summary>
		/// Gets or sets a log target to output all events to.  When this value is set, all events will immediately
		/// cause an output to the logfile.
		/// </summary>
		public Log.Log OutputLog
		{
			get { return _OutputLog; }
			set { _OutputLog = value; }
		}
		private Log.Log _OutputLog;

		#region IDisposable Members
		private int _DisposeCount = 0;
		/// <summary>
		/// Disposes the EventListener and detaches all events.
		/// </summary>
		public void Dispose()
		{
			int test = System.Threading.Interlocked.CompareExchange(ref _DisposeCount, 1, 0);
			if (0 == test)
			{
				foreach (DynamicDelegate.EventHandlerRef handlerRef in _HandlerReferences)
				{ handlerRef.DetachHandler(); }
				_HandlerReferences.Clear();
			}
		}

		~EventListener()
		{
			if (0 == _DisposeCount && null != OutputLog)
			{ throw new InvalidOperationException("You must dispose the event listener before leaving scope"); }
		}
		#endregion
		/// <summary>
		/// Verifies that all of the events in this list correspond to a certain order.
		/// Specifically, it validates the following assumptions.
		/// 1. That both firstEvent and secondEvent are present in the EventList.
		/// 2. That, for each instance of firstEvent, there is an instance of secondEvent before another instance of firstEvent.
		/// 3. That no secondEvent occurs that is not preceded by a firstEvent.
		/// 
		/// For example where 
		/// MouseDown:MouseUp:MouseClick:MouseDown:MouseUp:MouseDoubleClick
		/// firstEvent=MouseDown and secondEvent=MouseUp is valid
		/// firstEvent=MouseClick and secondEvent=MouseDoubleClick is valid
		/// but if
		/// MouseDown:MouseDown:MouseUp:MouseClick:MouseUp:MouseDoubleClick
		/// firstEvent=MouseDown and secondEvent=MouseUp is INVALID
		/// firstEvent=MouseClick and secondEvent=MouseDoubleClick is valid
		/// </summary>
		/// <param name="p">Used for logging.</param>
		/// <param name="firstEvent">Specifies an event which should come before the secondEvent.</param>
		/// <param name="secondEvent">An event which should come after the firstEvent.</param>
		/// <returns>A ScenarioResult indicating the success or failure of the verification.</returns>
		public ScenarioResult VerifyOrder(TParams p, string firstEvent, string secondEvent)
		{ return this.EventsFired.VerifyOrder(p, firstEvent, secondEvent); }

		/// <summary>
		/// Verifies that the EventList has the specified number of events.
		/// </summary>
		/// <param name="p">Used for logging</param>
		/// <param name="expectedCount">The expected number of events firing.</param>
		/// <returns>A ScenarioResult.</returns>
		public ScenarioResult Verify(TParams p, int expectedCount)
		{ return this.EventsFired.Verify(p, expectedCount); }

		/// <summary>
		/// Outputs all of the events to the log specified by p.
		/// </summary>
		/// <param name="p">Used for logging</param>
		public void LogEvents(TParams p)
		{ this.EventsFired.LogEvents(p); }
		/// <summary>
		/// Verifies that the sender is correct for all EventData in the list.
		/// </summary>
		/// <param name="p">Used for logging.</param>
		/// <param name="sender">The expected sender</param>
		/// <returns>A ScenarioResult.</returns>
		public ScenarioResult VerifySender(TParams p, object sender)
		{ return this.EventsFired.VerifySender(p, sender); }
		/// <summary>
		/// Verifies the sender, event count and event ordering of the EventList.
		/// For more complex event ordering testing see <see cref="VerifyOrder"/>
		/// </summary>
		/// <param name="p">Used for logging.</param>
		/// <param name="sender">The expected sender.</param>
		/// <param name="expectedCount">The expected event count.</param>
		/// <param name="eventOrder">The expected event order string.  Of the form "MouseDown:MouseUp:MouseClick:" (note the trailing colon).</param>
		/// <returns>A ScenarioResult.</returns>
		public ScenarioResult Verify(TParams p, object sender, int expectedCount, string eventOrder)
		{ return this.EventsFired.Verify(p, sender, expectedCount, eventOrder); }

        /// <summary>
        /// Blocks execution on the message loop thread (while pumping the loop with Application.DoEvents)
        /// until the specified event fires.
        /// </summary>
        /// <param name="expectedEvent">The event which is expected to fire.</param>
        /// <returns>True if the event was fired before the timeout.</returns>
        public bool Block(string expectedEvent)
        { return Block(expectedEvent, TimeSpan.FromSeconds(25)); }

        /// <summary>
        /// Blocks execution on the message loop thread (while pumping the loop with Application.DoEvents)
        /// until the specified event fires.
        /// </summary>
        /// <param name="expectedEvent">The event which is expected to fire.</param>
        /// <param name="timeout">A maximum timeout to wait.</param>
        /// <returns>True if the event was fired before the timeout.</returns>
        public bool Block(string expectedEvent, TimeSpan timeout)
        { return Block(expectedEvent, timeout, 1, true); }
        /// <summary>
        /// Blocks execution on the message loop thread (while pumping the loop with Application.DoEvents)
        /// until the specified event fires.
        /// </summary>
        /// <param name="expectedEvent">The event which is expected to fire.</param>
        /// <param name="timeout">A maximum timeout to wait.</param>
        /// <param name="expectedCount">The expected number of times the event must fire.</param>
        /// <param name="throwOnTimeout">True if the method should throw on timeout (otherwise it will return false).</param>
        /// <returns>True if the event was fired before the timeout.</returns>
        public bool Block(string expectedEvent, TimeSpan timeout, int expectedCount, bool throwOnTimeout)
        {
            if (string.IsNullOrEmpty(expectedEvent)) 
            { throw new ArgumentNullException("expectedEvent"); }
            if (timeout.Ticks <= 0)
            { throw new ArgumentOutOfRangeException("timeout"); }
            if (expectedCount <= 0)
            { throw new ArgumentOutOfRangeException("expectedCount"); }

            DateTime stopTime = DateTime.Now + timeout;

            while ( GetFireCount(expectedEvent) <expectedCount && DateTime.Now <= stopTime)
            {
                System.Threading.Thread.Sleep(10);
                Application.DoEvents();
            }
            if (GetFireCount(expectedEvent) < expectedCount)
            {
                if (throwOnTimeout)
                { throw new TimeoutException(string.Format("Timeout expired waiting for event: {0} (Fired {1} Expected {2})",
                    expectedEvent,
                    GetFireCount(expectedEvent),
                    expectedCount)); }
                return false;
            }
            return true;
        }
        private int GetFireCount(string eventName)
        { return EventsFired.GetSubset(eventName).Count; }
	}
}
