using System;
using WFCTestLib.Util;
using System.Windows.Forms;
using System.Reflection;

public class EventBlocker : IDisposable
{
	DynamicDelegate.EventHandlerRef refForRemove;
	bool throwOnTimeout;
	private int fireCount = 0;
	private int requiredFireCount;
	private string eventName;
	TimeSpan timeout;
	~EventBlocker()
	{ throw new ApplicationException("Incorrect usage of EventBlocker, EventBlocker must be disposed"); }

	public EventBlocker(object sourceObject, string eventName, TimeSpan timeout)
		: this(sourceObject, eventName, timeout, 1, true)
	{ }
	public EventBlocker(object sourceObject, string eventName, TimeSpan timeout, int requiredFireCount, bool throwOnTimeout)
	{
		this.throwOnTimeout = throwOnTimeout;
		this.eventName = eventName;
		this.timeout = timeout;
		this.requiredFireCount = requiredFireCount;
		Type type = sourceObject.GetType();
		EventInfo info = type.GetEvent(eventName);
		refForRemove = DynamicDelegate.AddHandler(sourceObject, info, delegate { ++fireCount; return null; });
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		DateTime stopTime = DateTime.Now + timeout;
		while (fireCount < requiredFireCount && DateTime.Now <= stopTime)
		{
			System.Threading.Thread.Sleep(10);
			Application.DoEvents();
		}
		refForRemove.DetachHandler();
		if (fireCount < requiredFireCount)
		{
			if (throwOnTimeout)
			{ throw new TimeoutException("Timeout expired waiting for event: " + eventName + " to fire"); }
		}
	}
}
