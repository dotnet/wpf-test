// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/

using Avalon.Test.CoreUI.Trusted;
using System;
using System.IO;
using System.Threading;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// This class provides a file server that streams a file's content
    /// through a network stream. It takes a user-provided filename, then creates server
    /// and client sockets (both on the local machine), and returns a network stream 
    /// that has access to the client socket.
    /// 
    /// It tries to simulate a real-life busy file server, so it doesn't stream the
    /// entire source content in a continuous stream. Instead it provides an
    /// intermittent stream. It streams some bytes then pauses for some time, then
    /// streams few more bytes, then pauses again, and so on.
    /// 
    /// It starts with a pause (to simulate a request from client plus initial
    /// transmission delay etc.)
    /// 
    /// The server uses two user-settable parameters to control the above behavior:
    /// 1. Chuck size: Number of bytes streamed at a time.
    /// 2. Sleep time: Amount of time to pause between 2 chunks
    /// 
    /// User can set both of these parameters to be either random (between 0 and a 
    /// user-provided max value), or fixed at a user-provided value.
    /// </summary>
    public class SimulatedServer
    {
        /// <summary>
        /// 
        /// </summary>
        public SimulatedServer(string filename)
        {
            _filename = filename;
        }

        /// <summary>
        /// Instructs the simulated server to use a random chunk size,
        /// based on the given seed, and with the given maxChunkSize as the 
        /// upper bound (lower bound is 0).
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="maxChunkSize"></param>
        public void UseRandomChunkSize(int seed, int maxChunkSize)
        {
            CoreLogger.LogStatus("SimulatedServer using random chunk size between 0 and " +
                maxChunkSize);
            _chunkSizeRandom = true;
            _chunkSizeRandomGenerator = new Random(seed);
            _chunkSize = maxChunkSize;
        }

        /// <summary>
        /// Instructs the simulated server to use a fixed given chunk size.
        /// </summary>
        /// <param name="chunkSize"></param>
        public void UseFixedChunkSize(int chunkSize)
        {
            CoreLogger.LogStatus("SimulatedServer using fixed chunk size of " + chunkSize);
            _chunkSizeRandom = false;
            _chunkSize = chunkSize;
        }

        /// <summary>
        /// Instructs the simulated server to use a random sleep time,
        /// based on the given seed, and with the given maxSleepTime as the 
        /// upper bound (in milliseconds) (lower bound is 0).
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="maxSleepTime"></param>
        public void UseRandomSleepTime(int seed, int maxSleepTime)
        {
            CoreLogger.LogStatus("SimulatedServer using random sleep time between 0 and " +
                maxSleepTime + " milliseconds");
            _sleepTimeRandom = true;
            _sleepTimeRandomGenerator = new Random(seed);
            _sleepTime = maxSleepTime;
        }

        /// <summary>
        /// Instructs the simulated server to use a fixed given sleep time
        /// (in milliseconds).
        /// </summary>
        /// <param name="sleepTime"></param>
        public void UseFixedSleepTime(int sleepTime)
        {
            CoreLogger.LogStatus("SimulatedServer using fixed sleep time of " +
                sleepTime + " milliseconds");
            _sleepTimeRandom = false;
            _sleepTime = sleepTime;
        }

        /// <summary>
        /// Stream the given file.
        /// </summary>
        /// <returns></returns>
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
                Thread.Sleep(1000); //sleep for a second to give the thread time to start
            }

            return _throttledReadStream;
        }

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

        // Returns the chunk size to be used (considering the random or not decision). 
        private int ChunkSize
        {
            get
            {
                if ((bool)_chunkSizeRandom)
                {
                    return _chunkSizeRandomGenerator.Next(_chunkSize + 1);
                }
                else //fixed
                {
                    return _chunkSize;
                }
            }
        }

        // Returns the sleep time to be used (considering the random or not decision). 
        private int SleepTime
        {
            get
            {
                if ((bool)_sleepTimeRandom)
                {
                    return _sleepTimeRandomGenerator.Next(_sleepTime + 1);
                }
                else //fixed
                {
                    return _sleepTime;
                }
            }
        }

        private string _filename;
        private int _chunkSize;
        private int _sleepTime;
        private bool? _chunkSizeRandom = null; //Nullable bool
        private bool? _sleepTimeRandom = null; //Nullable bool
        private Random _chunkSizeRandomGenerator;
        private Random _sleepTimeRandomGenerator;
        private ThrottledReadStream _throttledReadStream = null;
    }
}
