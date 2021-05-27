// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *
 *   Program:   TimeManagerCommon
 *
 *
 ************************************************************/
using System;
using System.Reflection;
using System.Windows.Media.Animation;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// The TimeManager is internal so the DRT can't access it directly.
    /// This class exposes a subset of the TimeManager's functionality using Reflection
    /// </summary>
    public class InternalTimeManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InternalTimeManager()
        {
            //
            // Lookup PresentationCore and get the 
            // MediaContext for the current UI context
            // Use that to get the TimeManager.
            //
            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.Media.Animation.Timeline));
            Type type = assembly.GetType("System.Windows.Media.MediaContext");
            PropertyInfo property = type.GetProperty("CurrentMediaContext", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty);
            object mediaContext = property.GetValue(null, null);
            property = type.GetProperty("TimeManager", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            _timeManager = property.GetValue(mediaContext, null);
            _timeManagerType = assembly.GetType("System.Windows.Media.Animation.TimeManager");
        }


        #region TimeManager Internal Methods
        /// <summary>
        /// restart method
        /// </summary>
        public void Restart()
        {
            _timeManagerType.InvokeMember("Restart",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        /// <summary>
        /// start method
        /// </summary>
        public void Start()
        {
            _timeManagerType.InvokeMember("Start",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        /// <summary>
        /// stop method
        /// </summary>
        public void Stop()
        {
            _timeManagerType.InvokeMember("Stop",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        /// <summary>
        /// pause method
        /// </summary>
        public void Pause()
        {
            _timeManagerType.InvokeMember("Pause",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }

        /// <summary>
        /// Resume Method
        /// </summary>
        public void Resume()
        {
            _timeManagerType.InvokeMember("Resume",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, null);
        }



        /// <summary>
        /// Seek Method, seeking from TimeSeekOrigin.BeginTime
        /// </summary>
        public void Seek(int offset)
        {
            _timeManagerType.InvokeMember("Seek",
                            BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.InvokeMethod,
                            null, _timeManager, new Object[]{offset, TimeSeekOrigin.BeginTime} );
        }

        #endregion


        /// <summary>
        /// CurrentTime
        /// </summary>
        public Nullable<TimeSpan> CurrentTime
        {
            get
            {

               return (Nullable<TimeSpan>)_timeManagerType.InvokeMember("CurrentTime",
                            BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.GetProperty,
                            null, _timeManager, null);
            }
        }
     
        private object _timeManager;
        private Type _timeManagerType;
      
    }
}
