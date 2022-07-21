// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT for StickyNoteControl
//

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace DRT
{
    public sealed class DrtStickyNoteControl : DrtTabletBase
    {

        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DrtStickyNoteControl();
            int result = drt.Run(args);
            drt.Dispose();
            return result;
        }

        private DrtStickyNoteControl() : base()
        {
            WindowTitle = "StickyNoteControl DRT";
            Contact     = "Microsoft";
            TeamContact = "WPF";
            DrtName     = "DrtStickyNoteControl";

            Suites = new DrtTestSuite[]{
                                           new StickyNoteControlBasicTests(),
                                           new StickyNoteControlUITests(),
                                           //new StickyNoteControlStyleTests(),
                                           new InkStickyNoteTests(),
                                       };
        }


        // The style ID of StickyNote's children
        internal const string c_CloseButtonId   = "PART_CloseButton";
        internal const string c_TitleBarId      = "PART_TitleThumb";
        internal const string c_ResizeCornerId  = "PART_ResizeBottomRightThumb";
        internal const string c_ContentId  	= "PART_ContentControl";
        internal const string c_IconId          = "PART_IconButton";
    }
}
