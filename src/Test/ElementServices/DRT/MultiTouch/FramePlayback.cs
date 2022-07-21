// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace DRT
{
    /// <summary>
    ///     Reports a FrameSequence to manipulation.
    /// </summary>
    public class FramePlayback
    {
        public FramePlayback(DrtBase drtBase)
        {
            _drtBase = drtBase;
        }

        /// <summary>
        ///     The sequence to report.
        /// </summary>
        public FrameSequence Sequence
        {
            get;
            set;
        }

        /// <summary>
        ///     The X,Y offset to apply to each frame input.
        /// </summary>
        public Point InitialOffset
        {
            get;
            set;
        }

        /// <summary>
        ///     Whether the reporting is currently sending input.
        /// </summary>
        public bool IsReporting
        {
            get;
            private set;
        }

        /// <summary>
        ///     The index of the current frame being played.
        /// </summary>
        public int CurrentFrameIndex
        {
            get { return _currentFrame; }
        }

        public Frame CurrentFrame
        {
            get
            {
                if ((_currentFrame >= 0) && (_currentFrame < Sequence.Frames.Count))
                {
                    return Sequence.Frames[_currentFrame];
                }

                return null;
            }
        }

        /// <summary>
        ///     Suspend the DRT and report the sequence of frames.
        /// </summary>
        public void Report()
        {
            if (IsReporting || (Sequence == null) || (Sequence.Frames.Count == 0))
                return;

            IsReporting = true;
            _currentFrame = -1;

            _timer = new DispatcherTimer(DispatcherPriority.Input);
            _timer.Interval = TimeSpan.FromMilliseconds(Sequence.Frames[0].TimeOffset);
            _timer.Tick += OnTick;

            _drtBase.Suspend();

            _timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            Frame lastFrame = null;
            if (_currentFrame >= 0)
            {
                lastFrame = Sequence.Frames[_currentFrame];
            }
            _currentFrame++;
            Frame frame = Sequence.Frames[_currentFrame];

            IList<FrameInput> added;
            IList<FrameInput> removed;
            IList<FrameInput> same;
            frame.GetFrameDelta(lastFrame, out added, out removed, out same);

            if (removed != null)
            {
                foreach (FrameInput frameInput in removed)
                {
                    TestTouchDevice device = _devices[frameInput.Id];
                    frameInput.Update(device, InitialOffset);
                    device.OnUp();
                    device.OnDeactivate();

                    _devices.Remove(frameInput.Id);
                }
            }
            if (same != null)
            {
                foreach (FrameInput frameInput in same)
                {
                    TestTouchDevice device = _devices[frameInput.Id];
                    frameInput.Update(device, InitialOffset);
                    device.OnMove();
                }
            }
            if (added != null)
            {
                foreach (FrameInput frameInput in added)
                {
                    TestTouchDevice device = frameInput.CreateTouchDevice();
                    frameInput.Update(device, InitialOffset);
                    device.UpdateActiveSource(_drtBase.MainWindow);
                    _devices[frameInput.Id] = device;

                    device.OnActivate();
                    device.OnDown();
                }
            }

            if ((_currentFrame + 1) < Sequence.Frames.Count)
            {
                // Queue for the next frame
                Frame nextFrame = Sequence.Frames[_currentFrame + 1];
                _timer.Interval = TimeSpan.FromMilliseconds(nextFrame.TimeOffset);
            }
            else
            {
                // The frame sequence is complete, resume the DRT

                _timer.Stop();
                _timer = null;

                _drtBase.Assert(_devices.Count == 0, "The test sequence should end with no devices.");

                IsReporting = false;
                _drtBase.Resume();
            }
        }

        private DispatcherTimer _timer;
        private int _currentFrame;
        private DrtBase _drtBase;
        private Dictionary<int, TestTouchDevice> _devices = new Dictionary<int, TestTouchDevice>();
    }

    /// <summary>
    ///     Represents a sequence of input frames.
    /// </summary>
    [ContentProperty("Frames")]
    public class FrameSequence
    {
        public static FrameSequence Load(string resourceName)
        {
            Assembly thisExe = Assembly.GetExecutingAssembly();
            Stream stream = thisExe.GetManifestResourceStream(String.Format("{0}.{1}", thisExe.GetName().Name, resourceName));
            return (FrameSequence)XamlReader.Load(stream);
        }

        public FrameSequence()
        {
        }

        /// <summary>
        ///     The sequence of frames.
        /// </summary>
        public IList<Frame> Frames
        {
            get
            {
                if (_frames == null)
                {
                    _frames = new List<Frame>();
                }

                return _frames;
            }
        }

        private List<Frame> _frames;
    }

    /// <summary>
    ///     Represents an input frame, containing the current state of zero or more FrameInputs.
    /// </summary>
    [ContentProperty("FrameInputs")]
    public class Frame
    {
        public Frame()
        {
        }

        /// <summary>
        ///     The time offset from the beginning of the sequence.
        /// </summary>
        public int TimeOffset
        {
            get;
            set;
        }

        /// <summary>
        ///     The current state of FrameInputs.
        /// </summary>
        public IList<FrameInput> FrameInputs
        {
            get
            {
                if (_frameInputs == null)
                {
                    _frameInputs = new List<FrameInput>();
                }

                return _frameInputs;
            }
        }

        public FrameInput GetFrameInput(int id)
        {
            if (_frameInputs != null)
            {
                for (int i = 0; i < _frameInputs.Count; i++)
                {
                    FrameInput input = _frameInputs[i];
                    if (input.Id == id)
                    {
                        return input;
                    }
                }
            }

            return null;
        }

        public void GetFrameDelta(Frame baseline, out IList<FrameInput> added, out IList<FrameInput> removed, out IList<FrameInput> same)
        {
            if (_frameInputs != null)
            {
                if ((baseline != null) && (baseline._frameInputs != null))
                {
                    _frameInputs.Sort(CompareById);

                    added = new List<FrameInput>();
                    removed = new List<FrameInput>();
                    same = new List<FrameInput>();

                    int frameIndex = 0;
                    int frameCount = _frameInputs.Count;
                    int baselineIndex = 0;
                    int baselineCount = baseline._frameInputs.Count;

                    while ((frameIndex < frameCount) || (baselineIndex < baselineCount))
                    {
                        FrameInput frameInput = null;
                        if (frameIndex < frameCount)
                        {
                            frameInput = _frameInputs[frameIndex];
                        }

                        FrameInput baselineInput = null;
                        if (baselineIndex < baselineCount)
                        {
                            baselineInput = baseline._frameInputs[baselineIndex];
                        }

                        if ((frameInput != null) && (baselineInput != null))
                        {
                            if (frameInput.Id == baselineInput.Id)
                            {
                                same.Add(frameInput);
                                frameIndex++;
                                baselineIndex++;
                            }
                            else if (frameInput.Id < baselineInput.Id)
                            {
                                added.Add(frameInput);
                                frameIndex++;
                            }
                            else // frameInput.Id > baselineInput.Id)
                            {
                                removed.Add(baselineInput);
                                baselineIndex++;
                            }
                        }
                        else if (frameInput != null)
                        {
                            added.Add(frameInput);
                            frameIndex++;
                        }
                        else if (baselineInput != null)
                        {
                            removed.Add(baselineInput);
                            baselineIndex++;
                        }
                    }
                }
                else
                {
                    added = _frameInputs;
                    removed = null;
                    same = null;
                }
            }
            else
            {
                added = null;
                removed = (baseline != null) ? baseline._frameInputs : null;
                same = null;
            }
        }

        private static int CompareById(FrameInput a, FrameInput b)
        {
            return (a.Id - b.Id);
        }

        private List<FrameInput> _frameInputs;
    }

    /// <summary>
    ///     Represents an individual input, such as a finger.
    /// </summary>
    public class FrameInput
    {
        /// <summary>
        ///     The ID of the input.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        ///     The X location of the input.
        /// </summary>
        public double X
        {
            get;
            set;
        }

        /// <summary>
        ///     The Y location of the input.
        /// </summary>
        public double Y
        {
            get;
            set;
        }

        /// <summary>
        ///     The size of the input.
        /// </summary>
        public double Size
        {
            get;
            set;
        }

        public void Update(TestTouchDevice touchDevice, Point initialOffset)
        {
            touchDevice.CurrentTouchPoint = new TouchPoint(
                touchDevice, 
                new Point(X + initialOffset.X, Y + initialOffset.Y), 
                new Rect(X + initialOffset.X - Size * 0.5, Y + initialOffset.Y - Size * 0.5, Size, Size), TouchAction.Move);
        }

        public TestTouchDevice CreateTouchDevice()
        {
            TestTouchDevice device = new TestTouchDevice(Id);
            return device;
        }

        public override string ToString()
        {
            return String.Format("FrameInput Id={0} X={1:F2} Y={2:F2}", Id, X, Y);
        }
    }
}
