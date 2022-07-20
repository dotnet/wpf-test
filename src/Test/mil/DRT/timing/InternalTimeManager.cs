// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Media.Animation;

namespace DRT
{

    /// <summary>
    /// The TimeManager is internal so the DRT can't access it directly.
    /// This class exposes a subset of the TimeManager's functionality using Reflection
    /// </summary>
    public class InternalTimeManager
    {
        public InternalTimeManager()
        {
            //
            // Lookup PresentationCore and get the 
            // MediaContext for the current UI context
            // Use that to get the TimeManager.
            //
            Assembly assembly = Assembly.GetAssembly(typeof(Timeline));
            Type type = assembly.GetType("System.Windows.Media.MediaContext");
            PropertyInfo property = type.GetProperty("CurrentMediaContext", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);
            _mediaContext = property.GetValue(null, null);
            property = type.GetProperty("TimeManager", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            _timeManager = property.GetValue(_mediaContext, null);
            _timeManagerType = assembly.GetType("System.Windows.Media.Animation.TimeManager");

            SetClock();
        }


        #region TimeManager Internal Methods
        public void Restart()
        {
            _timeManagerType.InvokeMember("Restart",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        public void Start()
        {
            _timeManagerType.InvokeMember("Start",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        public void Stop()
        {
            _timeManagerType.InvokeMember("Stop",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }



        public void Tick()
        {
            _timeManagerType.InvokeMember("Tick",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }
#endregion

        /// <summary>
        /// This property is a pass-through to TimeManager.TestTimingClock.CurrentTime 
        /// on the instance we created in SetClock().  Setting this property thus
        /// also sets the TimeManager's current time.
        public long CurrentTime
        {
            get
            {
                // The TimeManager's CurrentTime is a TimeSpan but we assume milliSeconds
                // so convert appropriately
                return (long) ((TimeSpan) _clockType.InvokeMember("CurrentTime",
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.GetProperty,
                            null, _clock, null)).TotalMilliseconds;

            }
            set
            {
                // The TimeManager's CurrentTime is a TimeSpan but we assume milliSeconds
                // so convert appropriately
                _clockType.InvokeMember("CurrentTime",
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.SetProperty,
                        null, _clock, new object[] {TimeSpan.FromMilliseconds((double) value)});
            }
        }

        /// <summary>
        /// Nulls out the TimeManager
        /// </summary>
        public void SetToNull()
        {
            FieldInfo timeManagerField = _mediaContext.GetType().GetField("_timeManager", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            timeManagerField.SetValue(_mediaContext, null);
        }

        /// <summary>
        /// Restores a nulled-out TimeManager to the MediaContext
        /// </summary>
        public void Restore()
        {

            FieldInfo timeManagerField = _mediaContext.GetType().GetField("_timeManager", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            timeManagerField.SetValue(_mediaContext, _timeManager);
        }

        /// <summary>
        /// Instantiates TimeManager.TestTimingClock, which implements System.Windows.Media.IClock.
        /// An instance of this class is then passed into the TimeManager, which uses it as its clock.
        /// This allows us to muck with the TimeManager's time by setting CurrentTime
        /// </summary>
        private void SetClock()
        {
            //
            // look up System.Windows.Media.TimeManager.TestTimingClock
            //
            _clockType = _timeManagerType.GetNestedType("TestTimingClock", BindingFlags.NonPublic);


            //
            // create an instance of TestTimingClock and save it
            //
            _clock = Activator.CreateInstance(_clockType);

            //
            // Pass that instance into the TimeManager
            //

            _timeManagerType.InvokeMember("Clock",
                        BindingFlags.DeclaredOnly |
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.SetProperty,
                        null, _timeManager, new Object[] { _clock });
        }

        private object _mediaContext;
        private object _timeManager;
        private Type _timeManagerType;

        // this will actually point to a dynamically-generated derived type
        // that derives from System.Windows.Media.TimingClock
        private object _clock;
        private Type _clockType;
    }
}
