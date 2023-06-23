using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;
namespace ReflectTools
{
	public class AccessibleEventListener : IDisposable
	{
		public enum EVENT : uint
		{
			MIN = 0x00000001,
			MAX = 0x7FFFFFFF,

			SYSTEM_SOUND = 0x0001,

			SYSTEM_ALERT = 0x0002,

			SYSTEM_FOREGROUND = 0x0003,


			SYSTEM_MENUSTART = 0x0004,
			SYSTEM_MENUEND = 0x0005,

			SYSTEM_MENUPOPUPSTART = 0x0006,
			SYSTEM_MENUPOPUPEND = 0x0007,


			SYSTEM_CAPTURESTART = 0x0008,
			SYSTEM_CAPTUREEND = 0x0009,

			SYSTEM_MOVESIZESTART = 0x000A,
			SYSTEM_MOVESIZEEND = 0x000B,

			SYSTEM_CONTEXTHELPSTART = 0x000C,
			SYSTEM_CONTEXTHELPEND = 0x000D,

			SYSTEM_DRAGDROPSTART = 0x000E,
			SYSTEM_DRAGDROPEND = 0x000F,


			SYSTEM_DIALOGSTART = 0x0010,
			SYSTEM_DIALOGEND = 0x0011,

			SYSTEM_SCROLLINGSTART = 0x0012,
			SYSTEM_SCROLLINGEND = 0x0013,

			SYSTEM_SWITCHSTART = 0x0014,
			SYSTEM_SWITCHEND = 0x0015,

			SYSTEM_MINIMIZESTART = 0x0016,
			SYSTEM_MINIMIZEEND = 0x0017,

			OBJECT_CREATE = 0x8000,  // hwnd + ID + idChild is created item
			OBJECT_DESTROY = 0x8001,  // hwnd + ID + idChild is destroyed item
			OBJECT_SHOW = 0x8002,  // hwnd + ID + idChild is shown item
			OBJECT_HIDE = 0x8003,  // hwnd + ID + idChild is hidden item
			OBJECT_REORDER = 0x8004,  // hwnd + ID + idChild is parent of zordering children

			OBJECT_FOCUS = 0x8005,  // hwnd + ID + idChild is focused item
			OBJECT_SELECTION = 0x8006,  // hwnd + ID + idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
			OBJECT_SELECTIONADD = 0x8007,  // hwnd + ID + idChild is item added
			OBJECT_SELECTIONREMOVE = 0x8008,  // hwnd + ID + idChild is item removed
			OBJECT_SELECTIONWITHIN = 0x8009,  // hwnd + ID + idChild is parent of changed selected items


			OBJECT_STATECHANGE = 0x800A,  // hwnd + ID + idChild is item w/ state change

			OBJECT_LOCATIONCHANGE = 0x800B,  // hwnd + ID + idChild is moved/sized item



			OBJECT_NAMECHANGE = 0x800C,  // hwnd + ID + idChild is item w/ name change
			OBJECT_DESCRIPTIONCHANGE = 0x800D,  // hwnd + ID + idChild is item w/ desc change
			OBJECT_VALUECHANGE = 0x800E,  // hwnd + ID + idChild is item w/ value change
			OBJECT_PARENTCHANGE = 0x800F,  // hwnd + ID + idChild is item w/ new parent
			OBJECT_HELPCHANGE = 0x8010,  // hwnd + ID + idChild is item w/ help change
			OBJECT_DEFACTIONCHANGE = 0x8011,  // hwnd + ID + idChild is item w/ def action change
			OBJECT_ACCELERATORCHANGE = 0x8012  // hwnd + ID + idChild is item w/ keybd accel change
		}

		private enum WINEVENT : uint
		{
			OUTOFCONTEXT = 0x0000,  // Events are ASYNC
			SKIPOWNTHREAD = 0x0001,  // Don't call back for events on installer's thread
			SKIPOWNPROCESS = 0x0002,  // Don't call back for events on installer's process
			INCONTEXT = 0x0004  // Events are SYNC, this causes your dll to be injected into every process
		}

		[System.Security.SuppressUnmanagedCodeSecurity()]
		private class NativeMethods
		{
			public delegate void WinEventDelegate(IntPtr hWinEventHook, [MarshalAs(UnmanagedType.U4)]EVENT accEvent, IntPtr hwnd, long idObject, long idChild, uint idEventThread, uint dwmsEventTime);

			[DllImport("user32.dll", SetLastError = true)]
			public static extern IntPtr SetWinEventHook([MarshalAs(UnmanagedType.U4)]EVENT eventMin, [MarshalAs(UnmanagedType.U4)]EVENT eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, [MarshalAs(UnmanagedType.U4)]WINEVENT dwFlags);
			[DllImport("user32.dll")]
			public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
		}
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		private void WinEventHandler(IntPtr hWinEventHook, EVENT eventID, IntPtr hwnd, long idObject, long idChild, uint idEventThread, uint dwmsEventTime)
		{ OnAccessibleEventFired(eventID); }


		public class AccessibleEventArgs : EventArgs
		{
			public readonly EVENT Event;
			public AccessibleEventArgs(EVENT eventID)
			{ this.Event = eventID; }
		}

		public event EventHandler<AccessibleEventArgs> AccessibleEventFired;
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		protected void OnAccessibleEventFired(EVENT eventID)
		{
			if (_AllEvents || _SubscribedEvents.Contains(eventID))
			{
				AccessibleEventArgs args = new AccessibleEventArgs(eventID);
				_FiredEvents.Add(args);
				EventHandler<AccessibleEventArgs> handler = AccessibleEventFired;
				if (null != handler)
				{ handler(this, args); }
			}
		}


		#region IDisposable Members
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		public void Dispose()
		{ UnHookEvent(); GC.SuppressFinalize(this); }

		~AccessibleEventListener()
		{
			Debug.Assert(false, "AccessibleEvents was not properly disposed");
			Dispose();
		}
		#endregion
		private IntPtr _WinEventHookToken;
		private NativeMethods.WinEventDelegate _Handler;
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		private void HookEvent()
		{
			if (IntPtr.Zero != _WinEventHookToken || null != _Handler)
			{ throw new InvalidOperationException("WinEvent was already hooked, cannot be hooked again"); }
			_Handler = new NativeMethods.WinEventDelegate(WinEventHandler);
			_WinEventHookToken = NativeMethods.SetWinEventHook(
				EVENT.MIN,
				EVENT.MAX,
				IntPtr.Zero,
				_Handler,
				(uint)System.Diagnostics.Process.GetCurrentProcess().Id,
				0,
				WINEVENT.OUTOFCONTEXT);
			WFCTestLib.Log.Log l = ReflectBase.Log;
			if (null != l)
			{ l.WriteLine("Accessible Hool Event returned: " + _WinEventHookToken.ToString()); }
			if (IntPtr.Zero == _WinEventHookToken)
			{
				int n = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
				throw new ApplicationException("can't hook WinEvent: " + n.ToString());
			}
		}

		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		private void UnHookEvent()
		{
			if (IntPtr.Zero == _WinEventHookToken || null == _Handler)
			{ throw new InvalidOperationException("Event was not hooked, cannot be unhooked"); }
			NativeMethods.UnhookWinEvent(_WinEventHookToken);
			_Handler = null;
			_WinEventHookToken = IntPtr.Zero;
		}
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		public AccessibleEventListener()
		{
			_AllEvents = true;
			HookEvent();
		}

		/// <summary>
		/// Constructs a new AccessibleEventListener which is filtered to respond to only the specified events.
		/// All other AccessibleEvents will be discarded.
		/// </summary>
		/// <param name="events">An array of the events to listen for.</param>
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		public AccessibleEventListener(params EVENT[] events)
		{
			_SubscribedEvents.AddRange(events);
			HookEvent();
		}
		private bool _AllEvents = false;
		private List<AccessibleEventArgs> _FiredEvents = new List<AccessibleEventArgs>();
		public IList<AccessibleEventArgs> FiredEvents
		{
			get
			{ return _FiredEvents.AsReadOnly(); }
		}
		private List<EVENT> _SubscribedEvents = new List<EVENT>();
	}
}
