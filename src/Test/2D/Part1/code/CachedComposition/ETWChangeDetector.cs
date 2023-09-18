// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.EventTracing;
using Microsoft.Test.Logging;
using Microsoft.Win32;
using System;
using System.Collections;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Detect changes by listening for ETW event specific to this test.
    /// </summary>
    class ETWDetector : ChangeDetector
    {

        #region Public methods

        //ctor
        public ETWDetector()
        {
            _updatedAreas = new ArrayList(256);
        }

        /// <summary>
        /// Starts event tracing.
        /// </summary>
        /// <returns>Whether we were able to start up the event tracing engine.</returns>
        public override TestResult DetectBefore(System.Windows.Window w)
        {
            _cacheRegenerations = 0;

            //we need old-style events, until the EVentTracing section of testruntime is updated.            
            _topKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\avalon.graphics");
            //switch to classic ETW event style
            //this doesn't actually do anything other than set the key
            _topKey.SetValue("ClassicETW", 1);

            //make sure we were able to set it
            if (0 == (int)_topKey.GetValue("ClassicETW", 0))
            {
                return TestResult.Fail;
            }
            //create and start the tracing session. This file will not be saved unless we manually grab it with /setupshell
            _session = new TraceEventSession(s_traceSessionFriendlyName, s_traceSessionFileName);

            //add WPF provider
            _session.EnableProvider(new Guid("E13B77A8-14B6-11DE-8069-001B212B5009"), TraceEventLevel.Verbose);

            return TestResult.Pass;
        }

        /// <summary>
        /// Stops the event tracing engine.
        /// </summary>
        /// <returns>Simple status of whether we were able to stop.</returns>
        public override TestResult DetectAfter()
        {
            //stop listening for the event              
            TraceEventSession.StopUserAndKernelSession(s_traceSessionFriendlyName);
            return TestResult.Pass;
        }

        /// <summary>
        /// Sift through the events and see if ours came up.
        /// </summary>
        /// <returns>Pass if the event was detected at least once.</returns>
        public override bool VerifyChanges(Requirements r, TestLog log)
        {
            ETWTraceEventSource events = new ETWTraceEventSource(s_traceSessionFileName, TraceEventSourceType.FileOnly);
            _cacheRegenerations = 0;
            _cacheUpdates = 0;
            events.EveryEvent += new Action<TraceEvent>(Events_EveryEvent);
            _log = log;
            events.Process(); /// Process the events.

            bool result = false;

            //a negative expected regens means that we expect that many or LESS
            //or to put it another way, no more regens than that amount.
            //0 counts as 0 or less in this case, so if we say 0, we mean NO REGENS
            //-1 counts as AT MAX ONE regen, if we get 2 then we would fail.
            if ((r.cacheRegensExpected < 1) && _cacheRegenerations <= Math.Abs(r.cacheRegensExpected))
            {
                result = true;
            }

            //greater than 0 expected regens means that many or more are REQUIRED
            //and we'll only pass if we see that many (or more!!!) regens   
            //1 counts as AT LEAST one regen, if we get two then we would pass.
            if ((r.cacheRegensExpected >= 1) && _cacheRegenerations >= r.cacheRegensExpected)
            {
                result = true;
            }

            //this could also be a screen area update test
            //screen area update tests trump event counting
            //so, if the total updated screen area was LESS than the expected max screen area update, this is a pass
            if (r.screenUpdateAreaExpected > 0) // -ve screen area means ignore this requirement
            {
                log.LogStatus("We expect " + r.screenUpdateAreaExpected + " or less of the content surface area to be allocated and updated...");

                log.LogStatus("There were " + _cacheUpdates + " update events.");
                log.LogStatus("There were " + _cacheRegenerations + " allocation/regeneration events.");

                float area = GetUpdatedSurfaceArea(r, log);
                if (area > r.screenUpdateAreaExpected || area < 1) // it has to be allocated, and it shouldn't be refreshed more than expected.
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
                log.LogStatus(area + " times the area of the content area was allocated and updated.");
            }

            return (result && r.successExpected);
        }

        /// <summary>
        /// Examines each event and checks if it's the one we want.
        /// Adjusts the count of cache regenerations by one for each time the event was fired.
        /// </summary>
        /// <param name="obj">a TraceEvent abject which holds the event info.</param>
        void Events_EveryEvent(TraceEvent obj)
        {
            //the event GUID and Version number are how we know that this was one of our cache regen events
            if (obj.TaskGuid == s_cacheRegenGUID && obj.Version == 0)
            {
                _cacheRegenerations++;
            }
            else if (obj.TaskGuid == s_cacheUpdateGUID && obj.Version == 0)
            {
                //the event is a texture memory update.
                _cacheUpdates++;
            }
            else { return; }//it's some other event that we don't care about.

            //right now, we track both updated and allocated area in the same list.
            //grab the rect that we've updated and add it to our record of updated area
            //first, get the data from the event
            byte[] eventData = obj.EventData(); // grab the data from the event object            
            _log.LogStatus(obj.Dump());
            AddUpdatedArea(eventData); // add it to the list of rects
            return;
        }

        /// <summary>
        /// Grab the updated area of texture memory from the event and keep track of it.
        /// </summary>
        /// <param name="data">The data block from the event.</param>
        void AddUpdatedArea(byte[] data)
        {
            // not a long enough data packet, this event may not have one. This is a evironment error.
            if (data.Length < 16)
            {
                _log.LogStatus("event found but data packet only " + data.Length + " long. Check that classicETW events are turned on with the avalon.graphics regkey.");
                return;
            }

            System.Int32 x = MakeInt(data[0], data[1], data[2], data[3]);
            System.Int32 y = MakeInt(data[4], data[5], data[6], data[7]);
            System.Int32 width = MakeInt(data[8], data[9], data[10], data[11]);
            System.Int32 height = MakeInt(data[12], data[13], data[14], data[15]);

            //now make a rect out of the retrieved values 
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle((Int32)x, (Int32)y, (Int32)width, (Int32)height);

            //and add that rect to our list of updated areas
            _updatedAreas.Add(rect);
        }

        /// <summary>
        /// Helper function to construct an int from 4 bytes.
        /// </summary>
        /// <returns></returns>
        Int32 MakeInt(byte a, byte b, byte c, byte d)
        {
            return (Int32)a | ((Int32)b) << 8 | ((Int32)c) << 16 | ((Int32)d) << 24;
        }
        /// <summary>
        /// return the area of the allocated and updated area retreived from the events as a 
        /// proportion of the content area
        /// </summary>
        /// <returns>
        /// The total surface area allocated and updated, normalised to the content area
        /// </returns>
        private float GetUpdatedSurfaceArea(Requirements req, TestLog log)
        {
            //sum up all of the rects - we count overdraw, since we're interested in it
            float area = 0.0f;

            float contentHeight = ChangeableContent.NormalHeight;
            float contentWidth = ChangeableContent.NormalWidth;

            if (req.uIElementSize == UIElementSize.FullScreen)
            {
                contentWidth = Microsoft.Test.Display.Monitor.GetPrimary().Area.Width;
                contentHeight = Microsoft.Test.Display.Monitor.GetPrimary().Area.Height;
            }

            if (req.uIElementSize != UIElementSize.FullScreen && req.uIElementSize != UIElementSize.Normal)
            {
                log.LogStatus("Surface area tracking testing is only raelly valid for Normal or FullScreen cases.");
            }

            foreach (System.Drawing.Rectangle r in _updatedAreas)
            {
                log.LogStatus("updated area rect: x:" + r.X + " y:" + r.Y + " width:" + r.Width + " height:" + r.Height);
                area += (r.Height / contentHeight) * (r.Width / contentWidth);
            }

            return area;
        }

        #endregion

        #region Variables

        private int _cacheRegenerations; // texture memory allocations
        private int _cacheUpdates; // texture data updates
        private ArrayList _updatedAreas;

        private static readonly Guid s_cacheRegenGUID = new Guid("A4FDB257-F156-48f6-B0F5-C4A944B553FB");
        private static readonly Guid s_cacheUpdateGUID = new Guid("85EB64F6-DC84-43c6-B14C-3BD607F42C0D");

        private static string s_traceSessionFileName = "trace.etl";
        private static string s_traceSessionFriendlyName = "CachedCompositionTrace";
        private TraceEventSession _session;
        private TestLog _log;
        private RegistryKey _topKey;
        #endregion Variables

    }
}
