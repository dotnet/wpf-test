// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System; // for STAThread
using Microsoft.Win32; // for Registry stuff
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Windows.Markup;

namespace DRT
{
    public sealed class DrtMedia : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DrtMedia();

            int result = drt.Run(args);
            drt = null;

            // This is done so if leak detection is enabled we should have released everything on exit.
            GC.Collect(GC.MaxGeneration);

            return result;
        }

        private DrtMedia()
        {
            WindowTitle = "DrtMedia";
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = "DrtMedia";

            Suites = new DrtTestSuite[] {
#if PRERELEASE
#if DEBUG // This DRT is for debug bits only
                new MediaTests(),
#endif // DEBUG
#endif // PRERELEASE
                null
            };
        }
    }

    public class MediaTests : DrtTestSuite
    {
        private const string AvalonGraphicsRegKey = @"Software\Microsoft\Avalon.Graphics";
        private const string EnableFakePlayerPresenter = "EnableFakePlayerPresenter";
        private const string FrameDuration = "FrameDuration";
        private const string Frames = "Frames";
        private const string XamlFilename = @"DrtFiles\DrtMedia\DrtMedia.xaml";
        private const string LogFilename = "Microsoft.log";

        // Minimum number of frames that must be shown for the test to pass.
        // This is fairly arbitrary, but since we show 5 frames per second
        // and we stay open for 5 seconds, we expect 25 frames to be shown.
        // I've observed as little as 19 frames shown when this DRT is run for
        // the first time. We want this number to be low enough that we don't
        // fail unnecessarily, but high enough that we do fail when something is
        // actually wrong.
        private const int MinFrames = 10;

        public MediaTests() : base("MediaTests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] {
                new DrtTest( Test_ShowMedia ),
                new DrtTest( Test_Dispose ),
                new DrtTest( Test_CheckLog )
            };
        }

        private void SaveRegistryInformation()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(AvalonGraphicsRegKey);

            if (key != null)
            {
                object temp = null;
                temp = key.GetValue(EnableFakePlayerPresenter);
                _oldEnableFakePlayerPresenter = (temp == null)?0:(Int32)temp;
                temp = key.GetValue(FrameDuration);
                _oldFrameDuration = (temp == null)?0:(Int32)temp;
                temp = key.GetValue(Frames);
                _oldFrames = (temp == null)?0:(Int32)temp;
            }
        }

        private void RestoreRegistryInformation()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(AvalonGraphicsRegKey, true);

            DRT.Assert(key != null, "Couldn't open RegKey - do you have sufficient privileges?");

            key.SetValue(EnableFakePlayerPresenter, _oldEnableFakePlayerPresenter);
            key.SetValue(FrameDuration, _oldFrameDuration);
            key.SetValue(Frames, _oldFrames);
        }

        private void SetRegistryInformationForTest()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(AvalonGraphicsRegKey, true);

            DRT.Assert(key != null, "Couldn't open RegKey - do you have sufficient privileges?");

            key.SetValue(EnableFakePlayerPresenter, 1);
            key.SetValue(FrameDuration, 200);
            key.SetValue(Frames, 50);
        }

        private void OnContentRendered(object o, EventArgs ea)
        {
            DRT.Pause(5000); // allow video to play for 5 seconds
            DRT.Resume();
        }

        // We want to show the window for 5 seconds, but we don't want these 5
        // seconds to start until after the window is rendered
        private void ShowWindow(Window window)
        {
            window.ContentRendered += new EventHandler(this.OnContentRendered);
            window.Show();
        }

        private void DoTest()
        {
            Stream stream = null;

            try
            {
                stream = File.OpenRead(XamlFilename);
                _window = (Window)XamlReader.Load(stream);
                ShowWindow(_window);
                DRT.Suspend();
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        // If there is a log file left from before, delete it
        private void ClearLog()
        {
            if (File.Exists(LogFilename))
            {
                File.Delete(LogFilename);
            }
        }

        // Play the video
        private void Test_ShowMedia()
        {
            ClearLog();
            SaveRegistryInformation();

            try
            {
                SetRegistryInformationForTest();
                DoTest();
            }
            finally
            {
                RestoreRegistryInformation();
            }

        }

        // Remove references to media and force garbage collection
        // to clean up video framework
        private void Test_Dispose()
        {
            if (_window != null)
            {
                _window.Close();
                _window = null;
                GC.Collect(GC.MaxGeneration);

                // We need to sleep for a bit because the video framework
                // is destroyed asynchronously
                DRT.Pause(500);
            }
        }

        // Look at the log file and check to make sure that frames were
        // rendered
        private void Test_CheckLog()
        {
            try
            {
                using (StreamReader sr = new StreamReader(LogFilename))
                {
                    int lines = 0;
                    while (sr.ReadLine() != null)
                    {
                        lines++;
                    }
                    Console.WriteLine(String.Format("{0} frames rendered.", lines));
                    Console.WriteLine(String.Format("{0} frames required to pass.", MinFrames));
                    DRT.Assert(lines >= MinFrames);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("No log file is present.");
                Console.WriteLine("This means you probably saw a blank window with no flashing colors.");
                Console.WriteLine("Please make sure you are not running over TS.");

                throw;
            }
        }

        // We keep track of the old values in the registry so that we can
        // restore them at the end of this DRT
        private Int32 _oldEnableFakePlayerPresenter;
        private Int32 _oldFrameDuration;
        private Int32 _oldFrames;

        // _window is accessed from 2 different tests, so we need to store it in
        // between
        private Window _window = null;
    }
}
