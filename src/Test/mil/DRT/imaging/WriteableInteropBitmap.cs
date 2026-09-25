// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows.Interop;
using System.Reflection;
using System.Windows;
using System.Runtime.InteropServices;

namespace DRT
{
    public sealed class WriteableInteropBitmap : IDisposable
    {
        public WriteableInteropBitmap(int width, int height, PixelFormat format)
        {
            Width = width;
            Height = height;
            Format = format;
            CreateBitmap();
        }

        ~WriteableInteropBitmap()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public int Width {get; private set;}
        public int Height {get; private set;}
        public PixelFormat Format { get; private set; }
        public InteropBitmap Bitmap { get; private set; }

        public void FillQuadrant(int quadrant, Color fill, bool useDirtyRect)
        {
            int x = 0;
            int y = 0;

            unsafe
            {
                switch (quadrant)
                {
                    case 0:
                        x = 0;
                        y = 0;
                        break;

                    case 1:
                        x = Width/2;
                        y = 0;
                        break;

                    case 2:
                        x = Width / 2;
                        y = Height / 2;
                        break;

                    case 3:
                        x = 0;
                        y = Height / 2;
                        break;
                }

                int bytesPerPixel = Format.BitsPerPixel / 8;
                int stride = Width * bytesPerPixel;

                byte* buffer = (byte*)_view.ToPointer();
                buffer += y * stride;        // full strides
                buffer += x * bytesPerPixel; // partial stride

                for (int iRow = 0; iRow < Height / 2; iRow++)
                {
                    byte* rowPtr = buffer + iRow * stride;

                    for (int iPixel = 0; iPixel < Width / 2; iPixel++)
                    {
                        byte* pixelPtr = rowPtr + iPixel * bytesPerPixel;

                        pixelPtr[0] = fill.B;
                        pixelPtr[1] = fill.G;
                        pixelPtr[2] = fill.R;
                        pixelPtr[3] = fill.A;
                    }
                }
            }

            if (useDirtyRect)
            {
                Int32Rect dirtyRect = new Int32Rect(x, y, Width / 2, Height / 2);
                Bitmap.Invalidate(dirtyRect);
            }
            else
            {
                Bitmap.Invalidate();
            }
        }

        private void Dispose(bool disposing)
        {
            DestroyBitmap();
        }

        private void CreateBitmap()
        {
            int bytesPerPixel = Format.BitsPerPixel / 8;
            int stride = Width * bytesPerPixel;
            int size = Height * stride;

            _section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 4, 0, size, null);
            _view = MapViewOfFile(_section, 0xF001F, 0, 0, size);
            Bitmap = (InteropBitmap)System.Windows.Interop.Imaging.CreateBitmapSourceFromMemorySection(_section, Width, Height, Format, stride, 0);
        }

        private void DestroyBitmap()
        {
            if (_view != IntPtr.Zero)
            {
                UnmapViewOfFile(_view);
                _view = IntPtr.Zero;
            }

            if (_section != IntPtr.Zero)
            {
                CloseHandle(_section);
                _section = IntPtr.Zero;
            }
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, int flProtect, int dwMaximumSizeHigh, int dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, int dwNumberOfBytesToMap);
        
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        private IntPtr _section;
        private IntPtr _view;
    }
}

