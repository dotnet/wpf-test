// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides logging services hooked into the InputManager.

namespace Test.Uis.Loggers
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Test.Input;

    /// <summary>
    /// Logs input events.
    /// </summary>
    public class InputLogger
    {
        #region Constructors.

        /// <summary>Private constructor for the logger.</summary>
        private InputLogger()
        { }

        #endregion Constructors.

        /// <summary>
        /// Events that can be logged.
        /// </summary>
        [Flags]
        public enum LogEvents
        {
            /// <summary>Log before notification step.</summary>
            PreNotifyInput = 1,
            /// <summary>Log after notification step.</summary>
            PostNotifyInput = 2,
            /// <summary>Log before process step.</summary>
            PreProcessInput = 4,
            /// <summary>Log after process step.</summary>
            PostProcessInput = 8,
        }

        #region Private methods.

        private bool DescribeInput(InputEventArgs args, out string description)
        {
            string deviceName = GetFromLastPeriod(args.Device);
            string sourceName = GetFromLastPeriod(args.Source);

            if (ShouldLogDevice(deviceName))
            {
                description = args.GetType().Name + ", device " + deviceName;
                if (sourceName != null && sourceName != String.Empty)
                    description +=", source " + sourceName;

                RoutedEventArgs routed = args as RoutedEventArgs;
                if (routed != null && routed.Source != null)
                {
                    description += ", source " + IdentifySource(routed.Source);
                }

                if (InputReportEventArgsWrapper.IsCorrectType(args))
                {
                    InputReportEventArgsWrapper report = new InputReportEventArgsWrapper(args);
                    if (report.Report != null)
                    {
                        description += ", mode " + report.Report.Mode.ToString() +
                            ", type " + report.Report.Type.ToString();
                    }
                }

                KeyEventArgs keyArgs = args as KeyEventArgs;
                if (keyArgs != null)
                {
                    description += ", key " + keyArgs.Key.ToString() +
                        ", IsDown " + keyArgs.IsDown;
                }

                TextCompositionEventArgs textArgs = args as TextCompositionEventArgs;
                if (textArgs != null)
                {
                    description += ", text [" + textArgs.Text + "]";
                }

                return true;
            }
            else
            {
                description = String.Empty;
                return false;
            }
        }

        /// <summary>Returns a string identifying the source.</summary>
        private string IdentifySource(object source)
        {
            if (source == null) return "null";
            FrameworkElement fe = source as FrameworkElement;
            if (fe != null)
            {
                return fe.ToString() + "[" + fe.Name + "]";
            }
            else
            {
                return source.ToString();
            }
        }

        private bool ShouldLogDevice(string deviceName)
        {
            if (_filterDevice == null) return true;
            return (deviceName.IndexOf(_filterDevice) != -1);
        }

        private string GetFromLastPeriod(object o)
        {
            if (o == null) return "";
            string s = o.ToString();
            if (s == "") return "";

            int lastPeriod = s.LastIndexOf('.');
            if (lastPeriod != -1)
                s = s.Substring(lastPeriod + 1);
            return s;
        }

        #region Event handlers.

        private void PostNotifyInput(object sender, NotifyInputEventArgs args)
        {
            string s;
            if (DescribeInput(args.StagingItem.Input, out s))
                Logger.Current.Log("PostNotifyInput [{0}]", s);
        }

        private void PreNotifyInput(object sender, NotifyInputEventArgs args)
        {
            string s;
            if (DescribeInput(args.StagingItem.Input, out s))
                Logger.Current.Log("PreNotifyInput [{0}]", s);
        }

        private void PostProcessInput(object sender, ProcessInputEventArgs args)
        {
        }

        private void PreProcessInput(object sender, PreProcessInputEventArgs args)
        {
        }

        #endregion Event handlers.

        #endregion Private methods.

        /// <summary>
        /// Starts input logging for the current InputManager.
        /// </summary>
        public static void EnableInputLogging()
        {
            EnableInputLogging(null, LogEvents.PreNotifyInput);
        }

        /// <summary>
        /// Starts input logging for the current InputManager.
        /// </summary>
        /// <param name="filterDevice">Device to filter logging.</param>
        public static void EnableInputLogging(string filterDevice)
        {
            EnableInputLogging(filterDevice, LogEvents.PreNotifyInput);
        }

        /// <summary>
        /// Starts input logging for the current InputManager.
        /// </summary>
        /// <param name="filterDevice">Device to filter logging.</param>
        /// <param name="events">Events to be logged.</param>
        public static void EnableInputLogging(string filterDevice, LogEvents events)
        {
            InputLogger logger = new InputLogger();
            logger._filterDevice = filterDevice;
            InputManager manager = InputManager.Current;
            if ((LogEvents.PreProcessInput & events) != 0)
                manager.PreProcessInput += new PreProcessInputEventHandler(logger.PreProcessInput);
            if ((LogEvents.PostNotifyInput & events) != 0)
                manager.PostNotifyInput += new NotifyInputEventHandler(logger.PostNotifyInput);
            if ((LogEvents.PreNotifyInput & events) != 0)
                manager.PreNotifyInput += new NotifyInputEventHandler(logger.PreNotifyInput);
            if ((LogEvents.PostProcessInput & events) != 0)
                manager.PostProcessInput += new ProcessInputEventHandler(logger.PostProcessInput);
        }

        #region Private fields.

        private string _filterDevice;

        #endregion Private fields.
    }
}