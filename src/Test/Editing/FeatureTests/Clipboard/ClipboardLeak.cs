// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.Utils;
using Test.Uis.Wrappers;
using Test.Uis.TestTypes;
using Microsoft.Test.Imaging;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Diagnostics;

namespace DataTransfer
{
    [Test(1, "Clipboard", "ClipboardLeak", MethodParameters = "/TestCaseType=ClipboardLeakTestCase /TestName=ClipboardLeak", Versions="4.8+")]
    public class ClipboardLeakTestCase : ManagedCombinatorialTestCase
    {
        SetBehavior _setDataBehavior=0;
        SetFormat _setDataFormat=0;
        GetFormat _getDataFormat=0;

        const long LeakThreshold = 10000000;
        const int RepeatCount = 5;
        int _count;
        long _baselineManagedMemory, _baselinePrivateBytes;
        System.Diagnostics.Process _currentProcess = System.Diagnostics.Process.GetCurrentProcess();

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            SetData();

            if (_getDataFormat == GetFormat.None)
            {
                QueueDelegate(StartIterateSet);
            }
            else
            {
                QueueDelegate(StartIterateGet);
            }
        }

        void StartIterateSet()
        {
            GetMemory(out _baselineManagedMemory, out _baselinePrivateBytes);
            _count = 0;
            IterateSet();
        }

        void IterateSet()
        {
            SetData();
            if (++_count < RepeatCount)
            {
                QueueDelegate(IterateSet);
            }
            else
            {
                QueueDelegate(EndIterateSet);
            }
        }

        void EndIterateSet()
        {
            VerifyLeak("Set");
            QueueDelegate(NextCombination);
        }

        void SetData()
        {
            string format=null;
            object data=null;

            switch (_setDataFormat)
            {
                case SetFormat.Bitmap:
                    format = "Bitmap";
                    data = CreateBitmap();
                    break;
                case SetFormat.SDBitmap:
                    format = "System.Drawing.Bitmap";
                    data = CreateBitmap();
                    break;
                case SetFormat.BitmapSource:
                    format = "System.Windows.Media.Imaging.BitmapSource";
                    data = CreateBitmapSource();
                    break;
            }

            switch (_setDataBehavior)
            {
                case SetBehavior.SetData:
                    Clipboard.SetData(format, data);
                    break;
                case SetBehavior.SetNoFlush:
                    Clipboard.SetDataObject(new DataObject(format, data), false);
                    break;
                case SetBehavior.SetAndFlush:
                    Clipboard.SetDataObject(new DataObject(format, data), true);
                    break;
            }
        }

        Bitmap CreateBitmap()
        {
            Bitmap bitmap = new Bitmap(2000, 1000, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(new System.Drawing.Point(10, 10),
                                        System.Drawing.Point.Empty,
                                        new System.Drawing.Size(2000, 1000));
            }
            return bitmap;
        }

        BitmapSource CreateBitmapSource()
        {
            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgr32;
            int width = 2000;
            int height = 1000;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];

            // Initialize the image with data.
            Random value = new Random();
            value.NextBytes(rawImage);

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);

            return bitmap;
        }

        void StartIterateGet()
        {
            GetData();
            GetMemory(out _baselineManagedMemory, out _baselinePrivateBytes);
            _count = 0;
            IterateGet();
        }

        void IterateGet()
        {
            GetData();
            if (++_count < RepeatCount)
            {
                QueueDelegate(IterateGet);
            }
            else
            {
                QueueDelegate(EndIterateGet);
            }
        }

        void EndIterateGet()
        {
            VerifyLeak("Get");
            QueueDelegate(NextCombination);
        }

        void GetData()
        {
            string format = null;
            switch (_getDataFormat)
            {
                case GetFormat.Bitmap:
                    format = "Bitmap";
                    break;
                case GetFormat.SDBitmap:
                    format = "System.Drawing.Bitmap";
                    break;
                case GetFormat.BitmapSource:
                    format = "System.Windows.Media.Imaging.BitmapSource";
                    break;
            }

            if (format != null)
            {
                object data = Clipboard.GetData(format);
            }
        }

        void GetMemory(out long managedMemory, out long privateBytes)
        {
            managedMemory = GC.GetTotalMemory(true);
            GC.WaitForPendingFinalizers();

            _currentProcess.Refresh();
            privateBytes = _currentProcess.PrivateMemorySize64;
        }

        void VerifyLeak(string s)
        {
            long managedMemory, privateBytes;
            GetMemory(out managedMemory, out privateBytes);
            managedMemory -= _baselineManagedMemory;
            privateBytes -= _baselinePrivateBytes;

            if (managedMemory > LeakThreshold || privateBytes > LeakThreshold)
            {
                string msg = null;
                if (managedMemory > LeakThreshold)
                {
                    msg = String.Format("{0}Data leaks {1} managed bytes", s, managedMemory/RepeatCount);
                }
                if (privateBytes > LeakThreshold)
                {
                    if (msg == null)
                    {
                        msg = String.Format("{0}Data leaks {1} private bytes", s, privateBytes/RepeatCount);
                    }
                    else
                    {
                        msg = String.Format("{0} and {1} private bytes", msg, privateBytes/RepeatCount);
                    }
                }
                msg = String.Format("{0} (on average)", msg);

                Verifier.Verify(false, msg);
            }
        }
    }

    enum SetBehavior { SetData, SetNoFlush, SetAndFlush }
    enum SetFormat { Bitmap, SDBitmap, BitmapSource }
    enum GetFormat { None, Bitmap, SDBitmap, BitmapSource }
}
