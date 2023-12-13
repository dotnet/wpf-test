// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using System.Windows.Forms;
using System.Reflection;

public class EventBlocker : IDisposable
{
	DynamicDelegate.EventHandlerRef _refForRemove;
	bool _throwOnTimeout;
	private int _fireCount = 0;
	private int _requiredFireCount;
	private string _eventName;
	TimeSpan _timeout;
	~EventBlocker()
	{ throw new ApplicationException("Incorrect usage of EventBlocker, EventBlocker must be disposed"); }

	public EventBlocker(object sourceObject, string eventName, TimeSpan timeout)
		: this(sourceObject, eventName, timeout, 1, true)
	{ }
	public EventBlocker(object sourceObject, string eventName, TimeSpan timeout, int requiredFireCount, bool throwOnTimeout)
	{
		this._throwOnTimeout = throwOnTimeout;
		this._eventName = eventName;
		this._timeout = timeout;
		this._requiredFireCount = requiredFireCount;
		Type type = sourceObject.GetType();
		EventInfo info = type.GetEvent(eventName);
		_refForRemove = DynamicDelegate.AddHandler(sourceObject, info, delegate { ++_fireCount; return null; });
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		DateTime stopTime = DateTime.Now + _timeout;
		while (_fireCount < _requiredFireCount && DateTime.Now <= stopTime)
		{
			System.Threading.Thread.Sleep(10);
			Application.DoEvents();
		}
		_refForRemove.DetachHandler();
		if (_fireCount < _requiredFireCount)
		{
			if (_throwOnTimeout)
			{ throw new TimeoutException("Timeout expired waiting for event: " + _eventName + " to fire"); }
		}
	}
}
