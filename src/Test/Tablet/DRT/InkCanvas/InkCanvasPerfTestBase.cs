// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Ink;
using System.Runtime.InteropServices;

namespace DRT
{
    public static class PerfLogger
    {
        static long start;
        static bool s_memoryProfile = true;

        public static void StartLogging(DrtBase drt, string name)
        {
            CLRProfilerControl.AllocationLoggingActive = s_memoryProfile;

            drt.LogOutput("Enter " + name);

            CLRProfilerControl.LogWriteLine("Enter");

            PerformanceUtilities.QueryPerformanceCounter(out start);
        }

        public static void EndLogging(DrtBase drt, string name)
        {
            long end;

            PerformanceUtilities.QueryPerformanceCounter(out end);

            double cost = PerformanceUtilities.CostInMilliseconds(end - start);

            CLRProfilerControl.LogWriteLine("Exit");
            drt.LogOutput("Exit " + name);

            GC.Collect(2);
            GC.WaitForPendingFinalizers();

            CLRProfilerControl.AllocationLoggingActive = false;

            CLRProfilerControl.DumpHeap();

            drt.LogOutput(string.Format("Test: {0} time: {1} ms", name, cost));
        }

    }

    #region PerformanceUtilities

    /// <summary>
    /// </summary>
    /// <ExternalAPI/>     
    public static class PerformanceUtilities
    {
        static private long _frequency = 0;
        static private long _frequency1000th = 0;

        /// <summary>
        ///     Initializes static data used by the Utilities class.
        /// </summary>

        static PerformanceUtilities()
        {
            QueryPerformanceFrequency(out _frequency);
            _frequency1000th = _frequency / 1000;
            //Debug.Assert((_frequency != 0),"PerformanceCounter is not supported on this machine");
        }

        /// <summary>
        ///     An optimized wrapper for QueryPerformanceCounter().
        /// </summary>
        /// <param name="lpPerformanceCount">
        ///     The value of <paramref name="lpPerformanceCount" /> is set to the current high-resolution counter value.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.  If the function fails, the return value is zero.
        /// </returns>
        /// <ExternalAPI/> 
        [System.Security.SuppressUnmanagedCodeSecurity, System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        /// <summary>
        ///     An optimized wrapper for QueryPerformanceFrequency().
        /// </summary>
        /// <param name="lpFrequency">
        ///     The value of <paramref name="lpFrequency" /> is set to the high-resolution counter frequency.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.  If the function fails, the return value is zero.
        /// </returns>
        /// <ExternalAPI/>      
        [System.Security.SuppressUnmanagedCodeSecurity, System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        /// <summary>
        ///     Calculates the elapsed time in QueryPerformanceCounter ticks between two given QueryPerformanceCounter values.
        /// </summary>
        /// <param name="startCount">
        ///     The starting counter value.
        /// </param>
        /// <param name="endCount">
        ///     The ending counter value.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time between the two counter values in QueryPerformanceCounter ticks.  
        ///     Wrapping is taken into account.
        /// </returns>
        /// <ExternalAPI/> 
        public static long Cost(long startCount, long endCount)
        {
            if ( endCount >= startCount )
            {
                return ( endCount - startCount );
            }
            else
            {
                return endCount - startCount + long.MaxValue;
            }
        }

        /// <summary>
        ///     Calculates the elapsed time in seconds between two given QueryPerformanceCounter values.
        /// </summary>
        /// <param name="startCount">
        ///     The starting counter value.
        /// </param>
        /// <param name="endCount">
        ///     The ending counter value.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time between the two counter values in seconds.  
        ///     Wrapping is taken into account.
        /// </returns>
        /// <ExternalAPI/>      
        public static double CostInSeconds(long startCount, long endCount)
        {
            return ( (double)Cost(startCount, endCount) ) / ( (double)_frequency );
        }

        /// <summary>
        ///     Calculates the elapsed time in seconds given an elapsed time expressed in QueryPerformanceCounter ticks.
        /// </summary>
        /// <param name="cost">
        ///     The elapsed time in QueryPerformanceCounter ticks.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time in seconds.  
        /// </returns>
        /// <ExternalAPI/> 
        public static double CostInSeconds(long cost)
        {
            return ( (double)cost ) / ( (double)_frequency );
        }

        /// <summary>
        ///     Calculates the elapsed time in milliseconds between two given QueryPerformanceCounter values.
        /// </summary>
        /// <param name="startCount">
        ///     The starting counter value.
        /// </param>
        /// <param name="endCount">
        ///     The ending counter value.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time between the two counter values in milliseconds.  
        ///     Wrapping is taken into account.
        /// </returns>
        /// <ExternalAPI/> 
        public static double CostInMilliseconds(long startCount, long endCount)
        {
            return ( (double)Cost(startCount, endCount) ) / ( (double)_frequency1000th );
        }

        /// <summary>
        ///     Calculates the elapsed time in milliseconds given an elapsed time expressed in QueryPerformanceCounter ticks.
        /// </summary>
        /// <param name="cost">
        ///     The elapsed time in QueryPerformanceCounter ticks.
        /// </param>
        /// <returns>
        ///     The function returns the elapsed time in milliseconds.  
        /// </returns>
        /// <ExternalAPI/> 
        public static double CostInMilliseconds(long cost)
        {
            return ( (double)cost ) / ( (double)_frequency1000th );
        }

        /// <summary>
        ///     Returns the total size of the managed (GC) heap.
        /// </summary>
        /// <remarks>
        ///     GetManagedHeapSize() utilizes System.GC.GetTotalMemory() to get the managed heap size.
        ///     Before the size is calculated, it performs a full garbage collection and waits for all 
        ///     pending finalizers.
        /// </remarks>
        /// <returns>
        ///     The function returns the total size of the managed (GC) heap expressed in bytes.
        /// </returns>
        /// <ExternalAPI/> 
        public static double GetManagedHeapSize()
        {
            return (double)GC.GetTotalMemory(true);
        }
    }

    #endregion PerformanceUtilities


    public abstract class InkCanvasPerfTestBase : InkCanvasEditingTests
    {
        private static bool s_hasUsagePrinted;

        protected InkCanvasPerfTestBase(string name)
            : base(name)
        {
        }

        protected void StartLog()
        {
            if ( DrtBase.WaitForPriority(DispatcherPriority.ApplicationIdle) )
            {
                PerfLogger.StartLogging(DRT, Name);
            }
            else
            {
                PerfLogger.StartLogging(DRT, Name + " - Timeout");
            }
        }

        protected void EndLog()
        {
            if ( DrtBase.WaitForPriority(DispatcherPriority.ApplicationIdle) )
            {
                PerfLogger.EndLogging(DRT, Name);
            }
            else
            {
                PerfLogger.EndLogging(DRT, Name + " - Timeout");
            }
        }

        public override void PrintOptions()
        {
            if ( !s_hasUsagePrinted )
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("All the perf tests are disabled by default:");

                int longestName = 0;
                foreach ( InkCanvasPerfTestBase suite in DrtInkCanvas.PerfSuites )
                {
                    longestName = Math.Max(longestName, suite.Name.Length);
                }

                string indent = "    ";
                int namesPerRow = ( ( 80 - indent.Length ) / ( longestName + 1 ) );
                string columnPaddingString = "{0,-" + longestName + "} ";

                int currentColumn = 0;
                foreach ( InkCanvasPerfTestBase suite in DrtInkCanvas.PerfSuites )
                {
                    if ( currentColumn == 0 )
                    {
                        Console.Write(indent);
                    }

                    Console.Write(columnPaddingString, suite.Name);
                    if ( ( ++currentColumn % namesPerRow ) == 0 )
                    {
                        Console.WriteLine();
                        currentColumn = 0;
                    }
                }

                if ( currentColumn != 0 )
                {
                    Console.WriteLine();
                }

                Console.WriteLine();
                string[] textArray1 = Environment.GetCommandLineArgs();
                Console.WriteLine(string.Format("Speed Test Usage:"));
                Console.WriteLine(string.Format(indent + "{0} /suite <PerfSuiteName> /verbose", textArray1[0]));
                Console.WriteLine(string.Format("Allocation Test Usage:"));
                Console.WriteLine(string.Format(indent + "clrprofiler -o <logfile> -p {0} /suite <PerfSuiteName>", textArray1[0]));
                Console.WriteLine(indent+"clrprofiler -l <logfile>");

                s_hasUsagePrinted = true;
            }
        }
    }
}
