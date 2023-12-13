// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data on Editing commands

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

using Test.Uis.Loggers;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Test.Uis.Data
{    
    /// <summary>Enumeration of Command type taken from EditingCommands.cs</summary>
    public enum CommandType
    {
        /// <summary>Typing command</summary>
        TypingCommand = 1,

        /// <summary>Caret navigation command</summary>
        CaretNavigationCommand = 2,

        /// <summary>Selection extension command</summary>
        SelectionExtensionCommand = 3,

        /// <summary>Character editing command</summary>
        CharacterEditingCommand = 4,

        /// <summary>Paragraph editing command</summary>
        ParagraphEditingCommand = 5,

        /// <summary>List editing command</summary>
        ListEditingCommand = 6,

        /// <summary>Speller command</summary>
        SpellingCommand = 7,

        /// <summary>Copy/Paste command</summary>
        CopyPasteCommand = 8,

        /// <summary>Table editing command</summary>
        TableEditingCommand = 9,
    }

    /// <summary>
    /// Provides information about interesting editing commands.
    /// </summary>
    public sealed class EditingCommandData
    {
        #region Constructors.
        
        private EditingCommandData() { }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Returns input gesture string for a given command
        /// </summary>
        /// <param name="routedUICommand">Command for which the input gesture string is returned</param>
        /// <returns>Input gesture string or NotImplemented exception</returns>
        public static string GetInputGestureStringForCommand(RoutedUICommand routedUICommand)
        {
            string inputGestureString = string.Empty;

            if ((routedUICommand == ApplicationCommands.Copy)||
                (routedUICommand == ApplicationCommands.Cut)||
                (routedUICommand == ApplicationCommands.Paste)||
                (routedUICommand == ApplicationCommands.Undo)||
                (routedUICommand == ApplicationCommands.Redo))
            {
                KeyGesture keyGesture = (KeyGesture)routedUICommand.InputGestures[0];
                inputGestureString = "^" + keyGesture.Key.ToString().ToLowerInvariant();                
            }            
            else
            {                
                throw new ApplicationException("Not implemented - " + routedUICommand.Name);
            }

            return inputGestureString;
        }

        /// <summary>
        /// Returns keyboard shortcut for specified Command in specified CultureInfo
        /// </summary>
        /// <param name="command">Command for which keyboard shortcut is required</param>
        /// <param name="cultureInfo">CultureInfo of the culture on which keyboard shortcut is required</param>
        /// <returns>Keyboard shortcut for specified command. Returns null if not able to find one.</returns>
        public static string GetKeyboardShortCutForCommand(RoutedUICommand command, CultureInfo cultureInfo)
        {
            bool langPackNotInstalled = false;
            if (Microsoft.Test.Globalization.LanguagePackHelper.CurrentWPFUICulture() != cultureInfo)
            {
                langPackNotInstalled = true;
            }

            switch (command.Name)
            {
                case "ToggleBold":
                    if (langPackNotInstalled)
                    {
                        return s_englishBoldShortCut;
                    }

                    if (cultureInfo.Name == CultureInfos.GermanGermany.Name)
                    {
                        return s_germanBoldShortCut;
                    }
                    else
                    if ((cultureInfo.Name == CultureInfos.SpanishSpain.Name) ||
                        (cultureInfo.Name == CultureInfos.PortugueseBrazil.Name))
                    {
                        return s_spanishBoldShortcut;
                    }
                    else
                    {
                        return s_englishBoldShortCut;
                    }
                case "ToggleItalic":
                    if (langPackNotInstalled)
                    {
                        return s_englishItalicShortCut;
                    }

                    if (cultureInfo.Name == CultureInfos.GermanGermany.Name)
                    {
                        return s_germanItalicShortCut;
                    }
                    else
                    if (cultureInfo.Name == CultureInfos.SpanishSpain.Name)
                    {
                        return s_spanishItalicShortCut;
                    }
                    else
                    {
                        return s_englishItalicShortCut;
                    }
                case "ToggleUnderline":
                    if (langPackNotInstalled)
                    {
                        return s_englishUnderlineShortCut;
                    }

                    if (cultureInfo.Name == CultureInfos.GermanGermany.Name)
                    {
                        return s_germanUnderlineShortCut;
                    }
                    else
                    if ((cultureInfo.Name == CultureInfos.SpanishSpain.Name) ||
                        (cultureInfo.Name == CultureInfos.PortugueseBrazil.Name))
                    {
                        return s_spanishUnderlineShortCut;
                    }
                    else
                    {
                        return s_englishUnderlineShortCut;
                    }

                //The rest of the shortcut keys mostly dont vary across different languages
                case "ToggleSubscript":
                    return s_englishSubscriptShortCut;                    
                case "ToggleSuperscript":
                    return s_englishSuperscriptShortCut;                    
                case "IncreaseFontSize":
                    return s_englishIncreaseFontSize;                    
                case "DecreaseFontSize":
                    return s_englishDecreaseFontSize;                    
                case "ResetFormat":
                    return s_englishResetFormat;                    
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns access key for the specified Command in specified CultureInfo
        /// </summary>
        /// <param name="command">Command for which access key is required</param>
        /// <param name="cultureInfo">CultureInfo of the culture on which access key is required</param>
        /// <returns>Accesss key for specified command. Returns null if not able to find one.</returns>
        public static string GetAccessKeyForCommand(RoutedUICommand command, CultureInfo cultureInfo)
        {
            bool langPackNotInstalled = false;
            if (Microsoft.Test.Globalization.LanguagePackHelper.CurrentWPFUICulture() != cultureInfo)
            {
                langPackNotInstalled = true;
            }

            switch (command.Name)
            {
                case "Cut":
                    if (langPackNotInstalled)
                    {
                        return s_englishCutAccessKey;
                    }

                    if (cultureInfo.Name == CultureInfos.GermanGermany.Name)
                    {
                        return s_germanCutAccessKey;
                    }
                    else
                    {
                        return s_englishCutAccessKey;
                    }
                case "Copy":
                    if (langPackNotInstalled)
                    {
                        return s_englishCopyAccessKey;
                    }

                    if (cultureInfo.Name == CultureInfos.GermanGermany.Name)
                    {
                        return s_germanCopyAccessKey;
                    }
                    else
                    {
                        return s_englishCopyAccessKey;
                    }
                case "Paste":
                    if (langPackNotInstalled)
                    {
                        return s_englishPasteAccessKey;
                    }

                    if (cultureInfo.Name == CultureInfos.GermanGermany.Name)
                    {
                        return s_germanPasteAccessKey;
                    }
                    else
                    {
                        return s_englishPasteAccessKey;
                    }
                default:
                    return null;
            }
        }

        /// <summary>Overload to ToString</summary>
        /// <returns>String representation of this object</returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Interesting values of Character Editing commands.</summary>
        public static EditingCommandData[] CharacterEditingValues
        {
            get
            {
                System.Collections.Generic.List<EditingCommandData> values;

                values = new System.Collections.Generic.List<EditingCommandData>(Values.Length);
                foreach (EditingCommandData data in Values)
                {
                    if (data.CommandType == CommandType.CharacterEditingCommand)
                    {
                        values.Add(data);
                    }
                }
                return values.ToArray();
            }
        }

        /// <summary>EditingCommandData for Bold</summary>
        public static EditingCommandData ToggleBold
        {
            get
            {
                foreach (EditingCommandData data in Values)
                {
                    if (data.RoutedUICommand == EditingCommands.ToggleBold)
                    {
                        return data;
                    }
                }
                return null;
            }
        }

        /// <summary>EditingCommandData for Italic</summary>
        public static EditingCommandData ToggleItalic
        {
            get
            {
                foreach (EditingCommandData data in Values)
                {
                    if (data.RoutedUICommand == EditingCommands.ToggleItalic)
                    {
                        return data;
                    }
                }
                return null;
            }
        }

        /// <summary>EditingCommandData for Underline</summary>
        public static EditingCommandData ToggleUnderline
        {
            get
            {
                foreach (EditingCommandData data in Values)
                {
                    if (data.RoutedUICommand == EditingCommands.ToggleUnderline)
                    {
                        return data;
                    }
                }
                return null;
            }
        }


        /// <summary>Returns type of the command</summary>
        public CommandType CommandType
        {
            get
            {
                return _commandType;
            }
        }

        /// <summary>Returns true if the command is a format command</summary>
        public bool IsFormatCommand
        {
            get
            {
                return _isToggleCommand;
            }
        }

        /// <summary>Returns true if the command is a toggle command type</summary>
        public bool IsToggleCommand
        {
            get
            {
                return _isToggleCommand;
            }
        }

        /// <summary>Returns the keyboard shortcut for the command.</summary>
        public string KeyboardShortcut
        {
            get
            {                    
                return GetKeyboardShortCutForCommand(RoutedUICommand, CultureInfo.CurrentUICulture);                
            }
        }
        
        /// <summary>Returns the name of the command</summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
        
        /// <summary>RoutedUICommand associated with this command</summary>
        public RoutedUICommand RoutedUICommand
        {
            get
            {
                return _routedUIcommand;
            }
        }

        /// <summary>Interesting values of Editing commands.</summary>
        public static EditingCommandData[] Values
        {
            get
            {
                if (s_values == null)
                {
                    s_values = new EditingCommandData[] {
                        //Character editing commands
                        CreateFromValue("ToggleBold", CommandType.CharacterEditingCommand, true, true, EditingCommands.ToggleBold),
                        CreateFromValue("ToggleItalic", CommandType.CharacterEditingCommand, true, true, EditingCommands.ToggleItalic),
                        CreateFromValue("ToggleUnderline", CommandType.CharacterEditingCommand, true, true, EditingCommands.ToggleUnderline),
                        CreateFromValue("ToggleSubscript", CommandType.CharacterEditingCommand, true, true, EditingCommands.ToggleSubscript),
                        CreateFromValue("ToggleSuperscript", CommandType.CharacterEditingCommand, true, true, EditingCommands.ToggleSuperscript),
                        CreateFromValue("IncreaseFontSize", CommandType.CharacterEditingCommand, true, false, EditingCommands.IncreaseFontSize),
                        CreateFromValue("DecreaseFontSize", CommandType.CharacterEditingCommand, true, false, EditingCommands.DecreaseFontSize),
                        CreateFromValue("ResetFormat", CommandType.CharacterEditingCommand, true, false, null),
                    };
                }
                return s_values;
            }
        }

        #endregion Public properties.

        #region Private methods.        

        //Creates a new EditingCommandData instance for given value.
        private static EditingCommandData CreateFromValue(string name, CommandType commandType, 
            bool isFormatCommand, bool isToggleCommand, RoutedUICommand routedUICommand)
        {
            EditingCommandData result;

            result = new EditingCommandData();
            result._name = name;
            result._commandType = commandType;
            result._isFormatCommand = isFormatCommand;
            result._isToggleCommand = isToggleCommand;
            result._routedUIcommand = routedUICommand;
            result._keyboardShortcut = string.Empty;            

            return result;
        }        

        #endregion Private methods.

        #region Private fields.

        //Type of the command
        private CommandType _commandType;

        //Whether the command is format command
        private bool _isFormatCommand;

        //Whether the command is toggle type command
        private bool _isToggleCommand;

        //Keyboard shortcut for the command
        private string _keyboardShortcut;

        //Name of the command
        private string _name;

        //RoutedUICommand associated with this command
        private RoutedUICommand _routedUIcommand;        

        //Interesting values for Editing commands
        private static EditingCommandData[] s_values;

        //Keyboard shortcuts
        private static readonly string s_englishBoldShortCut = "^b";
        private static readonly string s_germanBoldShortCut = "^+f";
        private static readonly string s_spanishBoldShortcut = "^n";

        private static readonly string s_englishItalicShortCut = "^i";
        private static readonly string s_germanItalicShortCut = "^+k";
        private static readonly string s_spanishItalicShortCut = "^k";

        private static readonly string s_englishUnderlineShortCut = "^u";
        private static readonly string s_germanUnderlineShortCut = "^+u";
        private static readonly string s_spanishUnderlineShortCut = "^s";        

        private static readonly string s_englishSubscriptShortCut = "^=";
        private static readonly string s_englishSuperscriptShortCut = "^+=";

        private static readonly string s_englishIncreaseFontSize = "^]";
        private static readonly string s_englishDecreaseFontSize = "^[";

        private static readonly string s_englishResetFormat = "^ ";

        //Access keys
        private static readonly string s_englishCutAccessKey = "t";
        private static readonly string s_germanCutAccessKey = "a";

        private static readonly string s_englishCopyAccessKey = "c";
        private static readonly string s_germanCopyAccessKey = "k";

        private static readonly string s_englishPasteAccessKey = "p";
        private static readonly string s_germanPasteAccessKey = "e";      

        #endregion Private fields.
    }
}