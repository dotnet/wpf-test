// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.Test.Security.Wrappers;

namespace Microsoft.Test.MSBuildEngine
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MSBuildPerf
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndTime;
    }

    /// <summary>
    /// 
    /// </summary>
    public class MSBuildProjectPerf : MSBuildPerf
    {
        string projectFileName;
        //string currenttargetName = null;
        //Hashtable temporarytableoftargets = null;
        MSBuildTargetPerf currenttargetperf = null;
        List<MSBuildTargetPerf> listoftargetsinproject = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public MSBuildProjectPerf()
        {
            //temporarytableoftargets = new Hashtable();
            listoftargetsinproject = new List<MSBuildTargetPerf>();
        }

        /// <summary>
        /// Project name.
        /// </summary>
        public string ProjectName
        {
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    projectFileName = PathSW.GetFileName(value);
                }
            }
            get
            {
                return projectFileName;
            }
        }

        /// <summary>
        /// List of Targets that executed in the project.
        /// </summary>
        public List<MSBuildTargetPerf> TargetsPerfDataList
        {
            get
            {
                return listoftargetsinproject;
            }
        }

        /// <summary>
        /// Set current project Target Data
        /// </summary>
        /// <param name="targetname">MSBuild target name</param>
        /// <param name="endtime">Boolean value indicating if time is end time.
        ///     <value>True = endtime</value>
        ///     <value>False = starttime</value>
        /// </param>
        /// <param name="timespan">DateTime object for build time.</param>
        internal void SetProjectTargetData(string targetname, bool endtime, DateTime timespan)
        {
            //if (temporarytableoftargets.Count > 0 && temporarytableoftargets[targetname] != null)
            //{
            //    currenttargetperf = (MSBuildTargetPerf)temporarytableoftargets[targetname];
            //}
            //else
            if (currenttargetperf == null)
            {
                currenttargetperf = new MSBuildTargetPerf();
                currenttargetperf.TargetName = targetname;
            }

            //currenttargetName = targetname;
            if (endtime && currenttargetperf != null)
            {
                //temporarytableoftargets.Remove(targetname);
                currenttargetperf.EndTime = timespan;
                listoftargetsinproject.Add(currenttargetperf);
                currenttargetperf = null;
            }

            if (endtime == false && currenttargetperf != null)
            {
                currenttargetperf.StartTime = timespan;
                //temporarytableoftargets.Add(targetname, currenttarget);
            }
        }

        /// <summary>
        /// Set current Targets tasks data.
        /// </summary>
        /// <param name="taskname">Task name</param>
        /// <param name="endtime">Boolean value indicating if time is end time.
        ///     <value>True = endtime</value>
        ///     <value>False = starttime</value>
        /// </param>
        /// <param name="timespan">DateTime object for build time.</param>
        internal void SetProjectTargetTaskData(string taskname, bool endtime, DateTime timespan)
        {
            //MSBuildTargetPerf currenttarget = null;
            //if (temporarytableoftargets.Count > 0 && temporarytableoftargets[currenttargetName] != null)
            //{
            //    currenttarget = (MSBuildTargetPerf)temporarytableoftargets[currenttargetName];

            currenttargetperf.SetTaskData(taskname, endtime, timespan);
            //}
        }
    }

    /// <summary>
    /// MSBuild perf data for Targets executed in a build.
    /// </summary>
    public class MSBuildTargetPerf : MSBuildPerf
    {
        string targetName;
        MSBuildTaskPerf currenttaskperf = null;
        //Hashtable temporarytableoftasks = null;
        List<MSBuildTaskPerf> listoftasksintarget = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public MSBuildTargetPerf()
        {
            //temporarytableoftasks = new Hashtable();
            listoftasksintarget = new List<MSBuildTaskPerf>();
        }

        /// <summary>
        /// Project file Target name.
        /// </summary>
        public string TargetName
        {
            set
            {
                targetName = value;
            }
            get
            {
                return targetName;
            }
        }

        /// <summary>
        /// List of Tasks with their Perf data.
        /// </summary>
        public List<MSBuildTaskPerf> TasksPerfDataList
        {
            get
            {
                return listoftasksintarget;
            }
        }

        /// <summary>
        /// Set the current Targets task perf data.
        /// </summary>
        /// <param name="taskname">MSBuild project Task name</param>
        /// <param name="endtime">Boolean value indicating if time is end time.
        ///     <value>True = endtime</value>
        ///     <value>False = starttime</value>
        /// </param>
        /// <param name="timespan">DateTime object for build time.</param>
        internal void SetTaskData(string taskname, bool endtime, DateTime timespan)
        {
            //MSBuildTaskPerf currenttaskperf = null;
            //if (temporarytableoftasks.Count > 0 && temporarytableoftasks[taskname] != null)
            //{
            //    currenttaskperf = (MSBuildTaskPerf)temporarytableoftasks[taskname];
            //}
            //else
            if (currenttaskperf == null)
            {
                currenttaskperf = new MSBuildTaskPerf();
                currenttaskperf.TaskName = taskname;
            }

            if (endtime && currenttaskperf != null)
            {
                //temporarytableoftasks.Remove(taskname);
                currenttaskperf.EndTime = timespan;
                listoftasksintarget.Add(currenttaskperf);
                currenttaskperf = null;
            }

            if (endtime == false && currenttaskperf != null)
            {
                currenttaskperf.StartTime = timespan;
                //temporarytableoftasks.Add(taskname, currenttaskperf);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MSBuildTaskPerf : MSBuildPerf
    {
        string taskName;

        /// <summary>
        /// MSBuild Task name.
        /// </summary>
        public string TaskName
        {
            set
            {
                taskName = value;
            }
            get
            {
                return taskName;
            }
        }
    }
}
