// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// Library command functionality.
    /// </summary>
    public static class TestCommandLibrary
    {
        /// <summary>
        /// List of all public Avalon commands.
        /// </summary>
        /// <remarks>
        /// This list should be updated whenever a public command is added to or removed from Avalon.
        /// </remarks>
        public static RoutedCommand[] AllCommands
        {
            get
            {
                return s_allCommands;
            }
        }

        private static RoutedCommand[] s_allCommands = new RoutedCommand[] {
            ApplicationCommands.Close,
            ApplicationCommands.ContextMenu,
            ApplicationCommands.Copy,
            ApplicationCommands.CorrectionList,
            ApplicationCommands.Cut,
            ApplicationCommands.Delete,
            ApplicationCommands.Find,
            ApplicationCommands.Help,
            ApplicationCommands.New,
            ApplicationCommands.Open,
            ApplicationCommands.Paste,
            ApplicationCommands.Print,
            ApplicationCommands.PrintPreview,
            ApplicationCommands.Properties,
            ApplicationCommands.Redo,
            ApplicationCommands.Replace,
            ApplicationCommands.Save,
            ApplicationCommands.SaveAs,
            ApplicationCommands.SelectAll,
            ApplicationCommands.Stop,
            ApplicationCommands.Undo,
            ComponentCommands.ExtendSelectionDown,
            ComponentCommands.ExtendSelectionLeft,
            ComponentCommands.ExtendSelectionRight,
            ComponentCommands.ExtendSelectionUp, 
            ComponentCommands.MoveDown, 
            ComponentCommands.MoveFocusBack,
            ComponentCommands.MoveFocusDown,
            ComponentCommands.MoveFocusForward,
            ComponentCommands.MoveFocusPageDown,
            ComponentCommands.MoveFocusPageUp,
            ComponentCommands.MoveFocusUp,
            ComponentCommands.MoveLeft,
            ComponentCommands.MoveRight,
            ComponentCommands.MoveToEnd,
            ComponentCommands.MoveToHome,
            ComponentCommands.MoveToPageDown,
            ComponentCommands.MoveToPageUp,
            ComponentCommands.MoveUp,
            ComponentCommands.ScrollByLine,
            ComponentCommands.ScrollPageDown,
            ComponentCommands.ScrollPageLeft,
            ComponentCommands.ScrollPageRight,
            ComponentCommands.ScrollPageUp,
            ComponentCommands.SelectToEnd,
            ComponentCommands.SelectToHome,
            ComponentCommands.SelectToPageDown,
            ComponentCommands.SelectToPageUp, 
            EditingCommands.AlignCenter,
            EditingCommands.AlignJustify,
            EditingCommands.AlignLeft,
            EditingCommands.AlignRight, 
            //EditingCommands.ApplyBackground,
            //EditingCommands.ApplyFontFamily,
            //EditingCommands.ApplyFontSize,
            //EditingCommands.ApplyForeground,
            EditingCommands.Backspace,
            EditingCommands.DecreaseFontSize,
            EditingCommands.DecreaseIndentation,
            EditingCommands.Delete,
            EditingCommands.DeleteNextWord,
            EditingCommands.DeletePreviousWord,            
            EditingCommands.EnterLineBreak,            
            EditingCommands.EnterParagraphBreak,
            EditingCommands.IncreaseFontSize,
            EditingCommands.IncreaseIndentation,
            EditingCommands.MoveDownByLine,
            EditingCommands.MoveDownByPage,
            EditingCommands.MoveDownByParagraph,
            EditingCommands.MoveLeftByCharacter,
            EditingCommands.MoveLeftByWord,
            EditingCommands.MoveRightByCharacter,
            EditingCommands.MoveRightByWord,
            EditingCommands.MoveToDocumentEnd,
            EditingCommands.MoveToDocumentStart,
            EditingCommands.MoveToLineEnd,
            EditingCommands.MoveToLineStart,
            EditingCommands.MoveUpByLine,
            EditingCommands.MoveUpByPage,
            EditingCommands.MoveUpByParagraph,
            EditingCommands.SelectDownByLine,
            EditingCommands.SelectDownByPage,
            EditingCommands.SelectDownByParagraph,
            EditingCommands.SelectLeftByCharacter,
            EditingCommands.SelectLeftByWord,
            EditingCommands.SelectRightByCharacter,
            EditingCommands.SelectRightByWord,
            EditingCommands.SelectToDocumentEnd,
            EditingCommands.SelectToDocumentStart,
            EditingCommands.SelectToLineEnd,
            EditingCommands.SelectToLineStart,
            EditingCommands.SelectUpByLine,
            EditingCommands.SelectUpByPage,
            EditingCommands.SelectUpByParagraph,
            EditingCommands.TabBackward,
            EditingCommands.TabForward,
            EditingCommands.ToggleBold,
            EditingCommands.ToggleBullets,
            EditingCommands.ToggleInsert,
            EditingCommands.ToggleItalic,
            EditingCommands.ToggleNumbering,
            EditingCommands.ToggleSubscript,
            EditingCommands.ToggleSuperscript,
            EditingCommands.ToggleUnderline,
            MediaCommands.BoostBass, 
            MediaCommands.ChannelDown, 
            MediaCommands.ChannelUp, 
            MediaCommands.DecreaseBass, 
            MediaCommands.DecreaseMicrophoneVolume, 
            MediaCommands.DecreaseTreble, 
            MediaCommands.DecreaseVolume, 
            MediaCommands.FastForward, 
            MediaCommands.IncreaseBass, 
            MediaCommands.IncreaseMicrophoneVolume, 
            MediaCommands.IncreaseTreble, 
            MediaCommands.IncreaseVolume, 
            MediaCommands.MuteMicrophoneVolume, 
            MediaCommands.MuteVolume, 
            MediaCommands.NextTrack, 
            MediaCommands.Pause, 
            MediaCommands.Play, 
            MediaCommands.PreviousTrack, 
            MediaCommands.Record, 
            MediaCommands.Rewind, 
            MediaCommands.Select, 
            MediaCommands.Stop, 
            MediaCommands.ToggleMicrophoneOnOff, 
            MediaCommands.TogglePlayPause, 
            NavigationCommands.BrowseBack, 
            NavigationCommands.BrowseForward,
            NavigationCommands.BrowseHome, 
            NavigationCommands.BrowseStop, 
            NavigationCommands.DecreaseZoom, 
            NavigationCommands.Favorites, 
            NavigationCommands.FirstPage, 
            NavigationCommands.GoToPage, 
            NavigationCommands.IncreaseZoom, 
            NavigationCommands.LastPage, 
            NavigationCommands.NextPage, 
            NavigationCommands.PreviousPage, 
            NavigationCommands.Refresh, 
            NavigationCommands.Search, 
            NavigationCommands.Zoom, 
        };

        /// <summary>
        /// A new dictionary of commands, associating them with expected default input gestures.
        /// </summary>
        private static Dictionary<RoutedCommand, InputGesture[]> s_globalGestureDictionary =
            new Dictionary<RoutedCommand, InputGesture[]>();

        /// <summary>
        /// A new dictionary of commands, associating them with expected string values.
        /// </summary>
        private static Dictionary<RoutedCommand, string> s_globalCommandStringDictionary =
            new Dictionary<RoutedCommand, string>();

        /// <summary>
        /// Fill in default input gesture dictionary with our gestures.
        /// </summary>
        static TestCommandLibrary()
        {
            // ApplicationCommands
            s_globalGestureDictionary.Add(ApplicationCommands.Close, new InputGesture[] { });
            s_globalGestureDictionary.Add(ApplicationCommands.ContextMenu, new InputGesture[] { 
                new KeyGesture(Key.F10, ModifierKeys.Shift), 
                new KeyGesture(Key.Apps, ModifierKeys.None) 
            });
            s_globalGestureDictionary.Add(ApplicationCommands.Copy, new InputGesture[] { 
                new KeyGesture(Key.C, ModifierKeys.Control), 
                new KeyGesture(Key.Insert, ModifierKeys.Control) 
            });
            s_globalGestureDictionary.Add(ApplicationCommands.CorrectionList, new InputGesture[] { });
            s_globalGestureDictionary.Add(ApplicationCommands.Cut, new InputGesture[] { 
                new KeyGesture(Key.X, ModifierKeys.Control), 
                new KeyGesture(Key.Delete, ModifierKeys.Shift) 
            });
            s_globalGestureDictionary.Add(ApplicationCommands.Delete, new InputGesture[] { new KeyGesture(Key.Delete, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ApplicationCommands.Find, new InputGesture[] { new KeyGesture(Key.F, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.Help, new InputGesture[] { new KeyGesture(Key.F1, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ApplicationCommands.New, new InputGesture[] { new KeyGesture(Key.N, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.Open, new InputGesture[] { new KeyGesture(Key.O, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.Paste, new InputGesture[] { 
                new KeyGesture(Key.V, ModifierKeys.Control), 
                new KeyGesture(Key.Insert, ModifierKeys.Shift) 
            });
            s_globalGestureDictionary.Add(ApplicationCommands.Print, new InputGesture[] { new KeyGesture(Key.P, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.PrintPreview, new InputGesture[] { new KeyGesture(Key.F2, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.Properties, new InputGesture[] { new KeyGesture(Key.F4, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ApplicationCommands.Redo, new InputGesture[] { new KeyGesture(Key.Y, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.Replace, new InputGesture[] { new KeyGesture(Key.H, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.Save, new InputGesture[] { new KeyGesture(Key.S, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.SelectAll, new InputGesture[] { new KeyGesture(Key.A, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ApplicationCommands.Stop, new InputGesture[] { new KeyGesture(Key.Escape, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ApplicationCommands.Undo, new InputGesture[] { new KeyGesture(Key.Z, ModifierKeys.Control) });

            // ComponentCommands
            s_globalGestureDictionary.Add(ComponentCommands.ExtendSelectionDown, new InputGesture[] { new KeyGesture(Key.Down, ModifierKeys.Shift) });
            s_globalGestureDictionary.Add(ComponentCommands.ExtendSelectionLeft, new InputGesture[] { new KeyGesture(Key.Left, ModifierKeys.Shift) });
            s_globalGestureDictionary.Add(ComponentCommands.ExtendSelectionRight, new InputGesture[] { new KeyGesture(Key.Right, ModifierKeys.Shift) });
            s_globalGestureDictionary.Add(ComponentCommands.ExtendSelectionUp, new InputGesture[] { new KeyGesture(Key.Up, ModifierKeys.Shift) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveDown, new InputGesture[] { new KeyGesture(Key.Down, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveFocusBack, new InputGesture[] { new KeyGesture(Key.Left, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveFocusDown, new InputGesture[] { new KeyGesture(Key.Down, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveFocusForward, new InputGesture[] { new KeyGesture(Key.Right, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveFocusPageDown, new InputGesture[] { new KeyGesture(Key.Next, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveFocusPageUp, new InputGesture[] { new KeyGesture(Key.PageUp, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveFocusUp, new InputGesture[] { new KeyGesture(Key.Up, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveLeft, new InputGesture[] { new KeyGesture(Key.Left, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveRight, new InputGesture[] { new KeyGesture(Key.Right, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveToEnd, new InputGesture[] { new KeyGesture(Key.End, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveToHome, new InputGesture[] { new KeyGesture(Key.Home, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveToPageDown, new InputGesture[] { new KeyGesture(Key.Next, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveToPageUp, new InputGesture[] { new KeyGesture(Key.PageUp, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.MoveUp, new InputGesture[] { new KeyGesture(Key.Up, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.ScrollByLine, new InputGesture[] { });
            s_globalGestureDictionary.Add(ComponentCommands.ScrollPageDown, new InputGesture[] { new KeyGesture(Key.Next, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.ScrollPageLeft, new InputGesture[] { });
            s_globalGestureDictionary.Add(ComponentCommands.ScrollPageRight, new InputGesture[] { });
            s_globalGestureDictionary.Add(ComponentCommands.ScrollPageUp, new InputGesture[] { new KeyGesture(Key.PageUp, ModifierKeys.None) });
            s_globalGestureDictionary.Add(ComponentCommands.SelectToEnd, new InputGesture[] { new KeyGesture(Key.End, ModifierKeys.Shift) });
            s_globalGestureDictionary.Add(ComponentCommands.SelectToHome, new InputGesture[] { new KeyGesture(Key.Home, ModifierKeys.Shift) });
            s_globalGestureDictionary.Add(ComponentCommands.SelectToPageDown, new InputGesture[] { new KeyGesture(Key.Next, ModifierKeys.Shift) });
            s_globalGestureDictionary.Add(ComponentCommands.SelectToPageUp, new InputGesture[] { new KeyGesture(Key.PageUp, ModifierKeys.Shift) });

            // EditingCommands
            // (none have default input gestures)

            // MediaCommands
            // (none have default input gestures)

            // NavigationCommands
            s_globalGestureDictionary.Add(NavigationCommands.BrowseBack, new InputGesture[] { 
                new KeyGesture(Key.Left , ModifierKeys.Alt ),
                new KeyGesture(Key.Back, ModifierKeys.None ) });
            s_globalGestureDictionary.Add(NavigationCommands.BrowseForward, new InputGesture[] { 
                new KeyGesture(Key.Right , ModifierKeys.Alt ),
                new KeyGesture(Key.Back, ModifierKeys.Shift ) });
            s_globalGestureDictionary.Add(NavigationCommands.BrowseHome, new InputGesture[] { 
                new KeyGesture(Key.Home , ModifierKeys.Alt ),
                new KeyGesture(Key.BrowserHome, ModifierKeys.None )
            });
            s_globalGestureDictionary.Add(NavigationCommands.BrowseStop, new InputGesture[] { 
                new KeyGesture(Key.Escape , ModifierKeys.Alt ),
                new KeyGesture(Key.BrowserStop, ModifierKeys.None )
            });
            s_globalGestureDictionary.Add(NavigationCommands.Favorites, new InputGesture[] { new KeyGesture(Key.I, ModifierKeys.Control) });
            s_globalGestureDictionary.Add(NavigationCommands.Refresh, new InputGesture[] { new KeyGesture(Key.F5, ModifierKeys.None) });
            s_globalGestureDictionary.Add(NavigationCommands.Search, new InputGesture[] { new KeyGesture(Key.F3, ModifierKeys.None) });

            // ApplicationCommands
            s_globalCommandStringDictionary.Add(ApplicationCommands.Close, "Close");
            s_globalCommandStringDictionary.Add(ApplicationCommands.ContextMenu, "ContextMenu");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Copy, "Copy");
            s_globalCommandStringDictionary.Add(ApplicationCommands.CorrectionList, "CorrectionList");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Cut, "Cut");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Delete, "Delete");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Find, "Find");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Help, "Help");
            s_globalCommandStringDictionary.Add(ApplicationCommands.New, "New");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Open, "Open");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Paste, "Paste");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Print, "Print");
            s_globalCommandStringDictionary.Add(ApplicationCommands.PrintPreview, "PrintPreview");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Properties, "Properties");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Redo, "Redo");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Replace, "Replace");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Save, "Save");
            s_globalCommandStringDictionary.Add(ApplicationCommands.SaveAs, "SaveAs");
            s_globalCommandStringDictionary.Add(ApplicationCommands.SelectAll, "SelectAll");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Stop, "Stop");
            s_globalCommandStringDictionary.Add(ApplicationCommands.Undo, "Undo");

            // ComponentCommands
            s_globalCommandStringDictionary.Add(ComponentCommands.ExtendSelectionDown, "ExtendSelectionDown");
            s_globalCommandStringDictionary.Add(ComponentCommands.ExtendSelectionLeft, "ExtendSelectionLeft");
            s_globalCommandStringDictionary.Add(ComponentCommands.ExtendSelectionRight, "ExtendSelectionRight");
            s_globalCommandStringDictionary.Add(ComponentCommands.ExtendSelectionUp, "ExtendSelectionUp");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveDown, "MoveDown");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveFocusBack, "MoveFocusBack");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveFocusDown, "MoveFocusDown");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveFocusForward, "MoveFocusForward");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveFocusPageDown, "MoveFocusPageDown");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveFocusPageUp, "MoveFocusPageUp");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveFocusUp, "MoveFocusUp");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveLeft, "MoveLeft");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveRight, "MoveRight");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveToEnd, "MoveToEnd");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveToHome, "MoveToHome");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveToPageDown, "MoveToPageDown");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveToPageUp, "MoveToPageUp");
            s_globalCommandStringDictionary.Add(ComponentCommands.MoveUp, "MoveUp");
            s_globalCommandStringDictionary.Add(ComponentCommands.ScrollByLine, "ScrollByLine");
            s_globalCommandStringDictionary.Add(ComponentCommands.ScrollPageDown, "ScrollPageDown");
            s_globalCommandStringDictionary.Add(ComponentCommands.ScrollPageLeft, "ScrollPageLeft");
            s_globalCommandStringDictionary.Add(ComponentCommands.ScrollPageRight, "ScrollPageRight");
            s_globalCommandStringDictionary.Add(ComponentCommands.ScrollPageUp, "ScrollPageUp");
            s_globalCommandStringDictionary.Add(ComponentCommands.SelectToEnd, "SelectToEnd");
            s_globalCommandStringDictionary.Add(ComponentCommands.SelectToHome, "SelectToHome");
            s_globalCommandStringDictionary.Add(ComponentCommands.SelectToPageDown, "SelectToPageDown");
            s_globalCommandStringDictionary.Add(ComponentCommands.SelectToPageUp, "SelectToPageUp");

            // EditingCommands
            s_globalCommandStringDictionary.Add(EditingCommands.AlignCenter, "AlignCenter");
            s_globalCommandStringDictionary.Add(EditingCommands.AlignJustify, "AlignJustify");
            s_globalCommandStringDictionary.Add(EditingCommands.AlignLeft, "AlignLeft");
            s_globalCommandStringDictionary.Add(EditingCommands.AlignRight, "AlignRight");
            //_globalCommandStringDictionary.Add(EditingCommands.ApplyBackground, "ApplyBackground");
            //_globalCommandStringDictionary.Add(EditingCommands.ApplyFontFamily, "ApplyFontFamily");
            //_globalCommandStringDictionary.Add(EditingCommands.ApplyFontSize, "ApplyFontSize");
            //_globalCommandStringDictionary.Add(EditingCommands.ApplyForeground, "ApplyForeground");
            s_globalCommandStringDictionary.Add(EditingCommands.Backspace, "Backspace");
            s_globalCommandStringDictionary.Add(EditingCommands.DecreaseFontSize, "DecreaseFontSize");
            s_globalCommandStringDictionary.Add(EditingCommands.DecreaseIndentation, "DecreaseIndentation");
            s_globalCommandStringDictionary.Add(EditingCommands.Delete, "Delete");
            s_globalCommandStringDictionary.Add(EditingCommands.DeleteNextWord, "DeleteNextWord");
            s_globalCommandStringDictionary.Add(EditingCommands.DeletePreviousWord, "DeletePreviousWord");
            s_globalCommandStringDictionary.Add(EditingCommands.EnterLineBreak, "EnterLineBreak");            
            s_globalCommandStringDictionary.Add(EditingCommands.EnterParagraphBreak, "EnterParagraphBreak");
            s_globalCommandStringDictionary.Add(EditingCommands.IncreaseFontSize, "IncreaseFontSize");
            s_globalCommandStringDictionary.Add(EditingCommands.IncreaseIndentation, "IncreaseIndentation");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveDownByLine, "MoveDownByLine");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveDownByPage, "MoveDownByPage");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveDownByParagraph, "MoveDownByParagraph");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveLeftByCharacter, "MoveLeftByCharacter");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveLeftByWord, "MoveLeftByWord");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveRightByCharacter, "MoveRightByCharacter");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveRightByWord, "MoveRightByWord");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveToDocumentEnd, "MoveToDocumentEnd");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveToDocumentStart, "MoveToDocumentStart");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveToLineEnd, "MoveToLineEnd");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveToLineStart, "MoveToLineStart");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveUpByLine, "MoveUpByLine");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveUpByPage, "MoveUpByPage");
            s_globalCommandStringDictionary.Add(EditingCommands.MoveUpByParagraph, "MoveUpByParagraph");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectDownByLine, "SelectDownByLine");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectDownByPage, "SelectDownByPage");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectDownByParagraph, "SelectDownByParagraph");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectLeftByCharacter, "SelectLeftByCharacter");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectLeftByWord, "SelectLeftByWord");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectRightByCharacter, "SelectRightByCharacter");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectRightByWord, "SelectRightByWord");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectToDocumentEnd, "SelectToDocumentEnd");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectToDocumentStart, "SelectToDocumentStart");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectToLineEnd, "SelectToLineEnd");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectToLineStart, "SelectToLineStart");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectUpByLine, "SelectUpByLine");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectUpByPage, "SelectUpByPage");
            s_globalCommandStringDictionary.Add(EditingCommands.SelectUpByParagraph, "SelectUpByParagraph");
            s_globalCommandStringDictionary.Add(EditingCommands.TabBackward, "TabBackward");
            s_globalCommandStringDictionary.Add(EditingCommands.TabForward, "TabForward");
            s_globalCommandStringDictionary.Add(EditingCommands.ToggleBold, "ToggleBold");
            s_globalCommandStringDictionary.Add(EditingCommands.ToggleBullets, "ToggleBullets");
            s_globalCommandStringDictionary.Add(EditingCommands.ToggleInsert, "ToggleInsert");
            s_globalCommandStringDictionary.Add(EditingCommands.ToggleItalic, "ToggleItalic");
            s_globalCommandStringDictionary.Add(EditingCommands.ToggleNumbering, "ToggleNumbering");
            s_globalCommandStringDictionary.Add(EditingCommands.ToggleSubscript, "ToggleSubscript");
            s_globalCommandStringDictionary.Add(EditingCommands.ToggleSuperscript, "ToggleSuperscript");
            s_globalCommandStringDictionary.Add(EditingCommands.ToggleUnderline, "ToggleUnderline");

            // MediaCommands
            s_globalCommandStringDictionary.Add(MediaCommands.BoostBass, "BoostBass");
            s_globalCommandStringDictionary.Add(MediaCommands.ChannelDown, "ChannelDown");
            s_globalCommandStringDictionary.Add(MediaCommands.ChannelUp, "ChannelUp");
            s_globalCommandStringDictionary.Add(MediaCommands.DecreaseBass, "DecreaseBass");
            s_globalCommandStringDictionary.Add(MediaCommands.DecreaseMicrophoneVolume, "DecreaseMicrophoneVolume");
            s_globalCommandStringDictionary.Add(MediaCommands.DecreaseTreble, "DecreaseTreble");
            s_globalCommandStringDictionary.Add(MediaCommands.DecreaseVolume, "DecreaseVolume");
            s_globalCommandStringDictionary.Add(MediaCommands.FastForward, "FastForward");
            s_globalCommandStringDictionary.Add(MediaCommands.IncreaseBass, "IncreaseBass");
            s_globalCommandStringDictionary.Add(MediaCommands.IncreaseMicrophoneVolume, "IncreaseMicrophoneVolume");
            s_globalCommandStringDictionary.Add(MediaCommands.IncreaseTreble, "IncreaseTreble");
            s_globalCommandStringDictionary.Add(MediaCommands.IncreaseVolume, "IncreaseVolume");
            s_globalCommandStringDictionary.Add(MediaCommands.MuteMicrophoneVolume, "MuteMicrophoneVolume");
            s_globalCommandStringDictionary.Add(MediaCommands.MuteVolume, "MuteVolume");
            s_globalCommandStringDictionary.Add(MediaCommands.NextTrack, "NextTrack");
            s_globalCommandStringDictionary.Add(MediaCommands.Pause, "Pause");
            s_globalCommandStringDictionary.Add(MediaCommands.Play, "Play");
            s_globalCommandStringDictionary.Add(MediaCommands.PreviousTrack, "PreviousTrack");
            s_globalCommandStringDictionary.Add(MediaCommands.Record, "Record");
            s_globalCommandStringDictionary.Add(MediaCommands.Rewind, "Rewind");
            s_globalCommandStringDictionary.Add(MediaCommands.Select, "Select");
            s_globalCommandStringDictionary.Add(MediaCommands.Stop, "Stop");
            s_globalCommandStringDictionary.Add(MediaCommands.ToggleMicrophoneOnOff, "ToggleMicrophoneOnOff");
            s_globalCommandStringDictionary.Add(MediaCommands.TogglePlayPause, "TogglePlayPause");

            // NavigationCommands
            s_globalCommandStringDictionary.Add(NavigationCommands.BrowseBack, "BrowseBack");
            s_globalCommandStringDictionary.Add(NavigationCommands.BrowseForward, "BrowseForward");
            s_globalCommandStringDictionary.Add(NavigationCommands.BrowseHome, "BrowseHome");
            s_globalCommandStringDictionary.Add(NavigationCommands.BrowseStop, "BrowseStop");
            s_globalCommandStringDictionary.Add(NavigationCommands.DecreaseZoom, "DecreaseZoom");
            s_globalCommandStringDictionary.Add(NavigationCommands.Favorites, "Favorites");
            s_globalCommandStringDictionary.Add(NavigationCommands.FirstPage, "FirstPage");
            s_globalCommandStringDictionary.Add(NavigationCommands.GoToPage, "GoToPage");
            s_globalCommandStringDictionary.Add(NavigationCommands.IncreaseZoom, "IncreaseZoom");
            s_globalCommandStringDictionary.Add(NavigationCommands.LastPage, "LastPage");
            s_globalCommandStringDictionary.Add(NavigationCommands.NextPage, "NextPage");
            s_globalCommandStringDictionary.Add(NavigationCommands.PreviousPage, "PreviousPage");
            s_globalCommandStringDictionary.Add(NavigationCommands.Refresh, "Refresh");
            s_globalCommandStringDictionary.Add(NavigationCommands.Search, "Search");
            s_globalCommandStringDictionary.Add(NavigationCommands.Zoom, "Zoom");

        }

        /// <summary>
        /// Returns whether the supplied command has all the gestures we expect.
        /// </summary>
        /// <param name="cmd">Command object</param>
        /// <returns>True if all the expected input gestures are attached to the command, false otherwise.</returns>
        /// <remarks>
        /// We only look for Key gestures among the default input gestures. Mouse/Stylus/Other gestures are ignored.
        /// </remarks>
        public static bool HasExpectedGestures(RoutedCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            InputGesture[] expectedGestures = null;
            if (!s_globalGestureDictionary.TryGetValue(cmd, out expectedGestures) || expectedGestures == null)
            {
                // This command has no entry in our expected gesture dictionary.
                // If this command is not expected to have gestures, it better not have any actual gestures.
                return (cmd.InputGestures.Count == 0);
            }

            // We have some expected gestures 

            if (expectedGestures.Length != cmd.InputGestures.Count)
            {
                // Gesture counts are incorrect for this command -- fail
                return false;
            }

            // Gesture counts are correct 

            int i;
            for (i = 0; i < expectedGestures.Length; i++)
            {
                // Look for key gestures
                KeyGesture expectedKeyGesture = expectedGestures[i] as KeyGesture;
                if (expectedKeyGesture == null)
                {
                    // Not a key gesture, try the next input gesture.
                    continue;
                }

                // We have a key gesture

                // Check whether this gesture exists in the command
                KeyGestureFinder finder = new KeyGestureFinder(expectedKeyGesture);
                bool bGestureFound = false;
                foreach (InputGesture actualGesture in cmd.InputGestures)
                {
                    KeyGesture actualKeyGesture = actualGesture as KeyGesture;
                    if (actualKeyGesture == null)
                    {
                        // Not a key gesture, try the next gesture in the command.
                        continue;
                    }

                    if (finder.Matches(actualKeyGesture))
                    {
                        // We have a match, no longer necessary to look in this command for the gesture.
                        bGestureFound = true;
                        break;
                    }

                    // We don't have a match, try the next gesture in the command.
                }


                if (!bGestureFound)
                {
                    return false;
                }

                // We found this expected gesture, loop back and try the next expected gesture in our list.
            }

            // Match occurs if we've visited every expected gesture and found a match.
            bool bAllGesturesFound = (i == expectedGestures.Length);
            return bAllGesturesFound;
        }

        /// <summary>
        /// Get the expected gestures from a library command.
        /// </summary>
        /// <param name="cmd">Command object</param>
        /// <returns>Input gesture collection, containing gestures we expect to be associated with this command.</returns>
        public static InputGestureCollection GetExpectedGestures(RoutedCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            InputGesture[] expectedGestures = null;
            if (!s_globalGestureDictionary.TryGetValue(cmd, out expectedGestures) || expectedGestures == null)
            {
                // This command has no entry in our expected gesture dictionary.
                return (null);
            }

            return new InputGestureCollection(expectedGestures);
        }

        /// <summary>
        /// Returns whether the supplied command has the string we expect.
        /// </summary>
        /// <param name="cmd">Command object</param>
        /// <returns>True if all the expected strings are attached to the command, false otherwise.</returns>
        public static bool HasExpectedString(RoutedCommand cmd)
        {
            string expectedString = GetExpectedStringValue(cmd);
            if (expectedString == "")
            {
                throw new ArgumentException("Command not valid.");
            }

            string actualString = s_commandTypeConverter.ConvertTo(cmd, typeof(string)) as string;
            
            return expectedString == actualString;
        }
            
        /// <summary>
        /// Get the expected converted string value from a library command.
        /// </summary>
        /// <param name="cmd">Command object</param>
        /// <returns>String representation of this command.</returns>
        public static string GetExpectedStringValue(RoutedCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            string expectedString = "";
            if (!s_globalCommandStringDictionary.TryGetValue(cmd, out expectedString) || expectedString == null)
            {
                // This command has no entry in our expected gesture dictionary.
                return ("");
            }

            return expectedString;
        }

        /// <summary>
        /// Get the expected converted command value from a library command.
        /// </summary>
        /// <param name="s">String representing command name.</param>
        /// <returns>Object representation of this command.</returns>
        public static RoutedCommand GetExpectedCommandValue(string s)
        {
            if (s == "")
            {
                throw new ArgumentException("Command string not valid.");
            }

            RoutedCommand actualCommand;
            try
            {
                actualCommand = s_commandTypeConverter.ConvertFrom(s) as RoutedCommand;
            }
            catch (NotSupportedException)
            {
                return (null);
            }
            if (!s_globalCommandStringDictionary.ContainsKey(actualCommand))
            {
                // This command has no entry in our expected gesture dictionary.
                return (null);
            }

            return actualCommand;
        }

        private static TypeConverter s_commandTypeConverter = TypeDescriptor.GetConverter(typeof(RoutedCommand));


        /// <summary>
        /// Helper class that provides functionality for finding a match between KeyGesture objects.
        /// </summary>
        private class KeyGestureFinder
        {
            public KeyGestureFinder(KeyGesture gesture)
            {
                _gesture = gesture;
            }
            private KeyGesture _gesture;

            public bool Matches(KeyGesture g)
            {
                return (_gesture.Key == g.Key) && (_gesture.Modifiers == g.Modifiers);
            }
        }
    }
}
