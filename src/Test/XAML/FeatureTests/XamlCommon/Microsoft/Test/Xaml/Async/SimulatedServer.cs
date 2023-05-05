// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Microsoft.Test.Logging;
using System.IO;

 // Purpose: Please see comments on class declaration below.
namespace Microsoft.Test.Xaml.Async
{
    /// <summary>
    /// This class provides a file server that streams a file's content
    /// through a network stream. It takes a user-provided filename, then creates server
    /// and client sockets (both on the local machine), and returns a network stream 
    /// that has access to the client socket.
    /// It tries to simulate a real-life busy file server, so it doesn't stream the
    /// entire source content in a continuous stream. Instead it provides an
    /// intermittent stream. It streams some bytes then pauses for some time, then
    /// streams few more bytes, then pauses again, and so on.
    /// It starts with a pause (to simulate a request from client plus initial
    /// transmission delay etc.)
    /// The server uses two user-settable parameters to control the above behavior:
    /// 1. Chuck size: Number of bytes streamed at a time.
    /// 2. Sleep time: Amount of time to pause between 2 chunks
    /// User can set both of these parameters to be either random (between 0 and a 
    /// user-provided max value), or fixed at a user-provided value.
    /// </summary>
    public class SimulatedServer
    {
        /// <summary> File Name String  </summary>
        private readonly string _filename;

        /// <summary> Chunk size </summary>
        private int _chunkSize;

        /// <summary> Sleep time </summary>
        private int _sleepTime;

        /// <summary> Random chunk size </summary>
        private bool? _chunkSizeRandom = null; // Nullable bool

        /// <summary> Random sleep time </summary>
        private bool? _sleepTimeRandom = null; // Nullable bool

        /// <summary> Chunk Size RandomGenerator </summary>
        private Random _chunkSizeRandomGenerator;

        /// <summary> Sleep Time Random Generator </summary>
        private Random _sleepTimeRandomGenerator;

        /// <summary> Throttled Read Stream </summary>
        private ThrottledReadStream _throttledReadStream = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedServer"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public SimulatedServer(string filename)
        {
            this._filename = filename;
        }

        #region Properties

        /// <summary>
        /// Gets the chunk size to be used (considering the random or not decision). 
        /// </summary>
        /// <value>The size of the chunk.</value>
        private int ChunkSize
        {
            get
            {
                if ((bool) _chunkSizeRandom)
                {
                    return _chunkSizeRandomGenerator.Next(_chunkSize + 1);
                }
                else // fixed
                {
                    return _chunkSize;
                }
            }
        }

        /// <summary>
        /// Gets the sleep time to be used (considering the random or not decision). 
        /// </summary>
        /// <value>The sleep time.</value>
        private int SleepTime
        {
            get
            {
                if ((bool) _sleepTimeRandom)
                {
                    return _sleepTimeRandomGenerator.Next(_sleepTime + 1);
                }
                else // fixed
                {
                    return _sleepTime;
                }
            }
        }

        #endregion

        /// <summary>
        /// Instructs the simulated server to use a random chunk size,
        /// based on the given seed, and with the given maxChunkSize as the
        /// upper bound (lower bound is 0).
        /// </summary>
        /// <param name="seed"> The seed value. </param>
        /// <param name="maxChunkSize">Size of the max chunk.</param>
        public void UseRandomChunkSize(int seed, int maxChunkSize)
        {
            GlobalLog.LogStatus("SimulatedServer using random chunk size between 0 and " +
                                maxChunkSize);
            _chunkSizeRandom = true;
            _chunkSizeRandomGenerator = new Random(seed);
            _chunkSize = maxChunkSize;
        }

        /// <summary>
        /// Instructs the simulated server to use a fixed given chunk size.
        /// </summary>
        /// <param name="chunkSize">Size of the chunk.</param>
        public void UseFixedChunkSize(int chunkSize)
        {
            GlobalLog.LogStatus("SimulatedServer using fixed chunk size of " + chunkSize);
            _chunkSizeRandom = false;
            this._chunkSize = chunkSize;
        }

        /// <summary>
        /// Instructs the simulated server to use a random sleep time,
        /// based on the given seed, and with the given maxSleepTime as the
        /// upper bound (in milliseconds) (lower bound is 0).
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <param name="maxSleepTime">The max sleep time.</param>
        public void UseRandomSleepTime(int seed, int maxSleepTime)
        {
            GlobalLog.LogStatus("SimulatedServer using random sleep time between 0 and " +
                                maxSleepTime + " milliseconds");
            _sleepTimeRandom = true;
            _sleepTimeRandomGenerator = new Random(seed);
            _sleepTime = maxSleepTime;
        }

        /// <summary>
        /// Instructs the simulated server to use a fixed given sleep time
        /// (in milliseconds).
        /// </summary>
        /// <param name="sleepTime">The sleep time.</param>
        public void UseFixedSleepTime(int sleepTime)
        {
            GlobalLog.LogStatus("SimulatedServer using fixed sleep time of " +
                                sleepTime + " milliseconds");
            _sleepTimeRandom = false;
            this._sleepTime = sleepTime;
        }

        /// <summary>
        /// Stream the given file.
        /// </summary>
        /// <returns> File Stream </returns>
        public Stream ServeFile()
        {
            if ((_chunkSizeRandom == null) || (_sleepTimeRandom == null))
            {
                throw new Exception("Need to choose random/non-random chunk size and sleep time.");
            }

            // Create a new thread that would act as a server 
            ThreadStart threadStart = new ThreadStart(ServerRoutine);
            Thread thread = new Thread(threadStart);
            thread.Start();

            while (_throttledReadStream == null)
            {
                Thread.Sleep(1000); // sleep for a second to give the thread time to start
            }

            return _throttledReadStream;
        }

        /// <summary>
        /// Server routine.
        /// </summary>
        private void ServerRoutine()
        {
            // limit how fast the given file can be read
            _throttledReadStream = new ThrottledReadStream(File.OpenRead(_filename));

            while (_throttledReadStream.ReadLimit < _throttledReadStream.Length)
            {
                Thread.Sleep(SleepTime);
                _throttledReadStream.ReadLimit += ChunkSize;
            }
        }
    }
}
