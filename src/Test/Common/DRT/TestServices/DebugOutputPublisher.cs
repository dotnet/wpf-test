// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <description>

// This class can be used to listen to all system debug output.

// </description>



using System;
using System.Collections.Generic;      // List<>
using System.ComponentModel;           // Win32Exception
using System.Diagnostics;              // TraceListener
using System.Runtime.InteropServices;  // DLLImport
using System.Text;                     // StringBuilder
using System.Threading;                // threading, events

namespace DRT
{
    /// <summary>
    /// This class can be used to listen to all system debug output.
    /// </summary>
    /// <remarks>
    /// This is based on the DBMon Visual Studio sample and reads all output
    /// from the unmanaged OutputDebugString call that underlies almost all
    /// trace output written by managed applications. This uses the system
    /// shared memory buffer DBWIN_BUFFER where all of this debug output is
    /// written. Two system events control access to this buffer,
    /// DBWIN_DATA_READY and DBWIN_BUFFER_READY. The first event indicates that
    /// debug output is ready to be consumed from the buffer. The second event
    /// is fired by the listener and indicates that the listener is done
    /// reading from the buffer. An alternative managed implementation can be
    /// found at
    /// http://blogs.msdn.com/philipbeber/archive/2005/07/06/debugmonitor.aspx.
    /// Many parts of this class borrow from that sample as well.
    /// </remarks>
    public sealed class DebugOutputPublisher
    {
        #region Public Event
        //----------------------------------------------------------------------
        // Public Event
        //----------------------------------------------------------------------

        /// <summary>
        /// Occurs when the listening thread receives debug output.
        /// </summary>
        public static event EventHandler<DebugOutputEventArgs> DebugOutput
        {
            add
            {
                lock (typeof(DebugOutputPublisher))
                {
                    // If this is the first handler being added, start the
                    // listening thread
                    if (_debugOutput == null)
                    {
                        StartListening();
                    }

                    _debugOutput = (EventHandler<DebugOutputEventArgs>)Delegate.Combine(
                        _debugOutput, value);
                }
            }

            remove
            {
                lock (typeof(DebugOutputPublisher))
                {
                    _debugOutput = (EventHandler<DebugOutputEventArgs>)Delegate.Remove(
                        _debugOutput, value);

                    // If the last handler has been removed, stop the listening
                    // thread
                    if (_debugOutput == null)
                    {
                        StopListening();
                    }
                }
            }
        }

        #endregion Public Event

        #region Private Methods
        //----------------------------------------------------------------------
        // Private Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Starts a thread that listens for debug output.
        /// </summary>
        /// <remarks>
        /// Only one DebugListener listening thread is allowed be active at a
        /// time. Also if any other application (e.g. DebugView) is already
        /// listening for debug output, the listening thread will exit
        /// immediately.
        /// </remarks>
        private static void StartListening()
        {
            // Ensure we start only one listening thread at a time
            lock (typeof(DebugOutputPublisher))
            {
                if (_debugOutputPublisher == null)
                {
                    // Create the synchronization events if necessary
                    if (_stop == null)
                    {
                        _stop = new ManualResetEvent(false);
                    }

                    if (_started == null)
                    {
                        _started = new ManualResetEvent(false);
                    }

                    _stop.Reset();
                    _started.Reset();

                    // Start the listener thread
                    Thread listenerThread = new Thread(new ThreadStart(ListenThreadProc));
                    listenerThread.Start();

                    TestServices.Trace("Started DebugListener thread.");

                    // Check to make sure the thread is listening (i.e. it was able
                    // to set up all the handles etc. properly). This also ensures
                    // that the listener is set up before any tests are run.
                    if (_started.WaitOne(DefaultTimeout, false))
                    {
                        TestServices.Trace("DebugListener thread is listening.");
                        _debugOutputPublisher = listenerThread;
                    }
                    else
                    {
                        TestServices.Trace("DebugListener thread is not listening.");

                        // Signal the event to stop the thread
                        _stop.Set();

                        if (!listenerThread.Join(DefaultTimeout))
                        {
                            throw new ApplicationException("Could not join listener thread.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stops any thread that is listening for debug output.
        /// </summary>
        private static void StopListening()
        {
            lock (typeof(DebugOutputPublisher))
            {
                if (_debugOutputPublisher != null)
                {
                    _stop.Set();

                    if (_debugOutputPublisher.Join(DefaultTimeout))
                    {
                        TestServices.Trace("Stopped DebugListener thread.");
                        _debugOutputPublisher = null;
                    }
                    else
                    {
                        throw new ApplicationException("Could not join listener thread.");
                    }
                }
            }
        }

        /// <summary>
        /// Publishes all system debug output to all DebugOutput handlers.
        /// </summary>
        private static void ListenThreadProc()
        {
            EventWaitHandle dataAcknowledged = null;
            EventWaitHandle dataReady = null;
            IntPtr debugOutputPages = IntPtr.Zero;
            IntPtr mappedDebugOutput = IntPtr.Zero;

            try
            {
                // Create event handles for the two named system events
                dataAcknowledged = CreateEventWaitHandle("DBWIN_BUFFER_READY");

                if (dataAcknowledged == null)
                {
                    TestServices.Trace(
                        "Could not listen for debug output. Check if another application is already listening.");
                    return;
                }

                dataReady = CreateEventWaitHandle("DBWIN_DATA_READY");

                if (dataReady == null)
                {
                    TestServices.Trace(
                        "Could not listen for debug output. Check if another application is already listening.");
                    return;
                }

                // Open the file mapping for the buffer
                debugOutputPages = CreateFileMapping("DBWIN_BUFFER");

                // Map the shared file to memory
                mappedDebugOutput = CreateMemoryMapping(debugOutputPages);

                // Set the events to indicate that we are listening
                _started.Set();
                dataAcknowledged.Set();

                // Create the set of events that we will break on:
                //   - when the buffer is ready
                //   - when the thread has been told to stop listening
                WaitHandle[] waitEvents = new WaitHandle[] {
                    dataReady,
                    _stop };

                // WaitAny returns the array index of the event that triggered
                // the wait to return. We will loop until the stop listening
                // event is signaled.
                while (WaitHandle.WaitAny(waitEvents) == 0)
                {
                    // Read the output and send it to the DebugOutput event
                    // handlers
                    int pid;
                    string output;

                    ReadNextOutput(mappedDebugOutput, out pid, out output);
                    WriteOutput(pid, output);

                    // Acknowledge that the data has been read out of the buffer
                    dataAcknowledged.Set();
                }
            }
            finally
            {
                // Reset the started event to indicate we have stopped listening
                if (_started != null)
                {
                    _started.Reset();
                }

                if (dataAcknowledged != null)
                {
                    dataAcknowledged.Close();
                    dataAcknowledged = null;
                }

                if (dataReady != null)
                {
                    dataReady.Close();
                    dataReady = null;
                }

                if (mappedDebugOutput != IntPtr.Zero)
                {
                    NativeMethods.UnmapViewOfFile(mappedDebugOutput);
                    mappedDebugOutput = IntPtr.Zero;
                }

                if (debugOutputPages != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(debugOutputPages);
                    debugOutputPages = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Creates a handle for the given system event.
        /// </summary>
        /// <param name="handleName">The name of the system event</param>
        /// <returns>An EventWaitHandle corresponding to the event. This can be
        /// null if there was already another listener for the event.</returns>
        private static EventWaitHandle CreateEventWaitHandle(
            string handleName)
        {
            bool created;

            EventWaitHandle handle = new EventWaitHandle(
                false,
                EventResetMode.AutoReset,
                handleName,
                out created);

            // We want to be the only listener for these events. If we aren't,
            // close the handle and exit.
            if (!created)
            {
                if (handle != null)
                {
                    handle.Close();
                    handle = null;
                }
            }

            return handle;
        }

        /// <summary>
        /// Creates a handle to a named system file mapping.
        /// </summary>
        /// <remarks>
        /// This is a wrapper around the Windows API CreateFileMapping call.
        /// </remarks>
        /// <param name="name">The name of the mapping</param>
        /// <returns>A handle to the file map</returns>
        private static IntPtr CreateFileMapping(string name)
        {
            IntPtr sharedFile = IntPtr.Zero;
            IntPtr invalidFileHandle = new IntPtr(-1);

            // Unless the file mapping is created with the PAGE_READWRITE flag,
            // the DBWIN_DATA_READY events are never fired.
            sharedFile = NativeMethods.CreateFileMapping(
                invalidFileHandle,             // hFile
                IntPtr.Zero,                   // lpFileMappingAttributes
                NativeMethods.PAGE_READWRITE,  // flProtect
                0,                             // dwMaximumSizeHigh
                DebugOutputBufferSize,         // dwMaximumSizeLow
                name);                         // lpName

            if (sharedFile == IntPtr.Zero)
            {
                Exception innerException = new Win32Exception();
                throw new ApplicationException("CreateFileMapping failed", innerException);
            }

            return sharedFile;
        }

        /// <summary>
        /// Maps a file mapping to memory.
        /// </summary>
        /// <remarks>
        /// This is a wrapper around the Windows API MapViewOfFile call.
        /// </remarks>
        /// <param name="sharedFile">The file mapping to map to memory</param>
        /// <returns>A pointer to the memory-mapped file</returns>
        private static IntPtr CreateMemoryMapping(IntPtr sharedFile)
        {
            IntPtr sharedMemory = IntPtr.Zero;

            sharedMemory = NativeMethods.MapViewOfFile(
                sharedFile,                    // hFileMappingObject
                NativeMethods.FILE_MAP_READ,   // dwDesiredAccess
                0,                             // dwFileOffsetHigh
                0,                             // dwFileOffsetLow
                new IntPtr(DebugOutputBufferSize));        // dwNumberOfBytesToMap

            if (sharedMemory == IntPtr.Zero)
            {
                Exception innerException = new Win32Exception();
                throw new ApplicationException("MapViewOfFile failed", innerException);
            }

            return sharedMemory;
        }

        /// <summary>
        /// Reads the next output line from a debug output memory buffer.
        /// </summary>
        /// <remarks>
        /// The buffer is assumed to be formatted like DBWIN_BUFFER. The first
        /// four bytes of the buffer are a process identifier. The rest of the
        /// buffer is a null-terminated string that contains the actual debug
        /// output.
        /// </remarks>
        /// <param name="sharedMemory">A pointer to the buffer</param>
        /// <param name="pid">The process ID from the buffer</param>
        /// <param name="output">The output string from the buffer</param>
        private static void ReadNextOutput(IntPtr buffer, out int pid, out string output)
        {
            // Read the PID (the first DWORD) from the buffer 
            pid = Marshal.ReadInt32(buffer);
            IntPtr pMessage;

            // Calculate where the message starts by adding sizeof(int) to the
            // starting address of the buffer
            if (IntPtr.Size == 4)
            {
                pMessage = new IntPtr(buffer.ToInt32() + Marshal.SizeOf(typeof(Int32)));
            }
            else if (IntPtr.Size == 8)
            {
                pMessage = new IntPtr(buffer.ToInt64() + Marshal.SizeOf(typeof(Int32)));
            }
            else
            {
                output = null;
                return;
            }

            // Read the ASCII message string out from the buffer
            // This uses the unsafe native method lstrlen to get the number of
            // bytes in the buffered string. lstrlen can cause access violations
            // because it keeps reading until it sees a NULL byte, but in this
            // case the buffer is always NULL-terminated by convention.
            output = Marshal.PtrToStringAnsi(pMessage);

            // Trim the output string to remove trailing newlines
            output = output.Trim();
        }

        /// <summary>
        /// Send the (optional) process identifier and output string to all
        /// the DebugOutput event handlers.
        /// </summary>
        /// <param name="pid">The process identifier for the trace output (-1
        /// indicates that it is unspecified)</param>
        /// <param name="output">The actual trace output to write</param>
        private static void WriteOutput(int pid, string output)
        {
            lock (typeof(DebugOutputPublisher))
            {
                if (_debugOutput != null)
                {
                    _debugOutput(
                        typeof(DebugOutputPublisher),
                        new DebugOutputEventArgs(pid, output));
                }
            }
        }

#endregion Private Methods

#region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        /// <summary>
        /// The size in bytes of the debug output buffer
        /// </summary>
        private const uint DebugOutputBufferSize = 4096;

        /// <summary>
        /// The default time in milliseconds to wait for other threads.
        /// </summary>
        private const int DefaultTimeout = 1000;
        
        /// <summary>
        /// The thread that is publishing debug output
        /// </summary>
        private static Thread _debugOutputPublisher;

        /// <summary>
        /// An event that indicates any active listening thread should stop
        /// </summary>
        private static ManualResetEvent _stop;

        /// <summary>
        /// An event that indicates the listening thread has started listening
        /// </summary>
        private static ManualResetEvent _started;

        /// <summary>
        /// The handler for the DebugOutput event
        /// </summary>
        private static EventHandler<DebugOutputEventArgs> _debugOutput;

#endregion Private Fields

#region NativeMethods (private class)
        //----------------------------------------------------------------------
        // NativeMethods (private class)
        //----------------------------------------------------------------------

        private static class NativeMethods
        {
            /// <summary>
            /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/createfilemapping.asp"/>
            /// </summary>
            internal const uint PAGE_READWRITE = 0x04;

            /// <summary>
            /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/mapviewoffile.asp"/>
            /// </summary>
            internal const uint FILE_MAP_READ = 0x0004;

            /// <summary>
            /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/createfilemapping.asp"/>
            /// </summary>
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr CreateFileMapping(
                IntPtr hFile,
                IntPtr lpFileMappingAttributes,
                UInt32 flProtect,
                UInt32 dwMaximumSizeHigh,
                UInt32 dwMaximumSizeLow,
                string lpName);

            /// <summary>
            /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/mapviewoffile.asp"/>
            /// </summary>
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr MapViewOfFile(
                IntPtr hFileMappingObject,
                UInt32 dwDesiredAccess,
                UInt32 dwFileOffsetHigh,
                UInt32 dwFileOffsetLow,
                IntPtr dwNumberOfBytesToMap);

            /// <summary>
            /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/unmapviewoffile.asp"/>
            /// </summary>
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

            /// <summary>
            /// <see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/sysinfo/base/closehandle.asp"/>
            /// </summary>
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool CloseHandle(IntPtr hHandle);
        }

#endregion NativeMethods (private class)
    }

    /// <summary>
    /// The event arguments associated with the DebugOutput event.
    /// </summary>
    public class DebugOutputEventArgs : EventArgs
    {
#region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Constructs a DebugOutputEventArgs with the given information.
        /// </summary>
        /// <param name="pid">The process ID of the debug output</param>
        /// <param name="output">The output</param>
        public DebugOutputEventArgs(int pid, string output)
        {
            _pid = pid;
            _output = output;
        }

#endregion Constructors

#region Public Properties
        //----------------------------------------------------------------------
        // Public Properties
        //----------------------------------------------------------------------

        /// <summary>
        /// Gets the process ID associated with the debug output.
        /// </summary>
        public int ProcessId
        {
            get
            {
                return _pid;
            }
        }

        /// <summary>
        /// Gets the actual debug output, represented as a string.
        /// </summary>
        public string Output
        {
            get
            {
                return _output;
            }
        }

#endregion Public Properties

#region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        /// <summary>
        /// The process ID
        /// </summary>
        private int _pid;

        /// <summary>
        /// The actual output
        /// </summary>
        private string _output;

#endregion Private Fields
    }
}
