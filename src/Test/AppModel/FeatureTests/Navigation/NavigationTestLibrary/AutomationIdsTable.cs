// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Some common AutomationIds (prepended by @ symbol)
    /// </summary>
    public class AutomationIdsTable
    {
        /// <summary>
        /// </summary>
        public const string
            File = "@Item 32768",
                FileNew = "@Item 1",
                    FileNewWindow = "@Item 275",
                FileOpen = "@Item 256",
                FileEdit = "@Item 41030",
                FileSave = "@Item 257",
                FileSaveAs = "@Item 258",
                FilePageSetup = "@Item 259",
                FilePrint = "@Item 260",
                FilePrintPreview = "@Item 277",
                FileProperties = "@Item 262",
                FileWorkOffline = "@Item 40998",
                FileClose = "@Item 40993",

            Edit = "@Item 32832",
                EditCut = "@Item 41025",
                EditCopy = "@Item 41026",
                EditPaste = "@Item 41027",
                EditSelectAll = "@Item 41028",
                EditFind = "@Item 1091",

            View = "@Item 32896",
                ViewStatusBar = "@Item 41474",
                ViewGoTo = "@Item 5",
                    ViewGoToBack = "@Item 41249",
                    ViewGoToForward = "@Item 41250",
                    ViewGoToHome = "@Item 41253",
                ViewStop = "@Item 41498",
                ViewRefresh = "@Item 41504",
                ViewSource = "@Item 3163",
                ViewFullScreen = "@Item 41499",

            Favorites = "@Item 33136",
            Tools = "@Item 32960",
            Help = "@Item 33024"
            ;

        internal static string GetUndecoratedId(string decoratedID)
        {
            if (string.IsNullOrEmpty(decoratedID) || !IsDecoratedAutomationId(decoratedID))
            {
                throw new ArgumentException("The decorated Automation Name is null, empty, or does not start with @ symbol");
            }

            return decoratedID.Substring(1);

        }

        internal static bool IsDecoratedAutomationId(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentException("Name is null or empty");
            }

            return Name.StartsWith("@");
        }

    }
}
