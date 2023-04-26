// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
*/
using System;
using System.Reflection;
using System.Windows.Media.Animation;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// This class exposes a subset of the TimeManager's functionality using Reflection
    /// </summary>
    public class TimeManagerInternal
    {
        public TimeManagerInternal()
        {
            //
            // Lookup PresentationCore and get the 
            // MediaContext for the current UI context
            // Use that to get the TimeManager.
            //
            Assembly assembly = Assembly.GetAssembly(typeof(Timeline));
            Type type = assembly.GetType("System.Windows.Media.MediaContext");
            PropertyInfo property = type.GetProperty("CurrentMediaContext", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);
            object mediaContext = property.GetValue(null, null);
            property = type.GetProperty("TimeManager", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            _timeManager = property.GetValue(mediaContext, null);
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

        public void Pause()
        {
            _timeManagerType.InvokeMember("Pause",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        public void Resume()
        {
            _timeManagerType.InvokeMember("Resume",
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

        public void Seek(int offset, TimeSeekOrigin origin)
        {
            _timeManagerType.InvokeMember("Seek",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, new object[] {offset, origin});
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

        public TimeSpan CurrentTime
        {
            get
            {

               return (TimeSpan)_clockType.InvokeMember("CurrentTime",
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.GetProperty,
                            null, _clock, null);

            }
            set
            {
                _clockType.InvokeMember("CurrentTime",
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.SetProperty,
                        null, _clock, new object[] { value });
            }
        }

        /// <summary>
        /// Creates a class that implements System.Windows.Media.IClock.  That interface
        /// is internal to PresentationCore so this class has to be dynamically generated 
        /// at runtime.  An instance of this class (CustomClock) is then passed into the
        /// TimeManager, which uses it as its clock.  This allows us to muck with the TimeManager's
        /// time by setting CurrentTime
        /// </summary>
        private void SetClock()
        {
            //
            // look up System.Windows.Media.TimeManager.TestTimingClock
            //
            _clockType = (Type)_timeManagerType.GetNestedType("TestTimingClock", BindingFlags.NonPublic);


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

        private object _timeManager;
        private Type _timeManagerType;

        // this will actually point to a dynamically-generated derived type
        // that derives from System.Windows.Media.TimingClock
        private object _clock;
        private Type _clockType;
    }
}
