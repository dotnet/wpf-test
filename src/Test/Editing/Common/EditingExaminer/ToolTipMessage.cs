// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: ToolTipMessage.cs implement the immediate window with the following
 * features:
 *      1. define tooltip message for font dialog.
 *
 *******************************************************************************/

#region Using directives

using System;
using System.Collections;
using System.Text;

#endregion

namespace EditingExaminer
{
    /// <summary>
    /// define tool tip message for font dialog
    /// </summary>
    class ToolTipMessage
    {
        /// <summary>
        /// hashtable to hold all the message.
        /// </summary>
        static Hashtable s_table = new Hashtable();
        
        /// <summary>
        /// method to fill up the hastable.
        /// </summary>
        static ToolTipMessage()
        {
            s_table.Add("OKButton", "Close the dialog and save any changes you have made");
            s_table.Add("CancelButton", "Close the dialog without saving any changes you have made");
            
            s_table.Add("StrikeCheckBox", "Check/uncheck to draw/undraw StrikeLine");
            s_table.Add("UnderLineCheckBox", "Check/uncheck to draw/undraw UnderLine");
            s_table.Add("BaseLineCheckBox", "Check/uncheck to draw/undraw BaseLine");
            s_table.Add("OverLineCheckBox", "Check/uncheck to draw/undraw OverLine");
            s_table.Add("TestRichTextBox", "Sample font, size, weight, color, etc");
            s_table.Add("TextColorListBox", "List popular colors for text");
            s_table.Add("TextColorTextBox", "Input to find an available color");
            s_table.Add("FontSizeListBox", "List available sizes in point for specified font");
            s_table.Add("FontStretchListBox", "List available stretchs for specified font");
            s_table.Add("FontWeightListBox", "List available weights for specified font");
            s_table.Add("FontStyleListBox", "List available styles for specified font");
            s_table.Add("FontFamilyListBox", "List available System fonts");
            s_table.Add("FontSizeTextBox", "Input to find an available font Size");
            s_table.Add("FontStretchTextBox", "Input to find an available font Stretch");
            s_table.Add("FontWeightTextBox", "Input to find an available font Weight");
            s_table.Add("FontStyleTextBox", "Input to find an available font Style");
            s_table.Add("FontFamilyTextBox", "Input to find an available System font");
        }

        /// <summary>
        /// return the hastable containing all the tooltip messages. using the Control Name as key to retrieve the message.
        /// </summary>
        /// <param name="ControlID">the key to return a specific message.</param>
        /// <returns>return a string that describle the usage of each control in the Font dialog</returns>
        public static string GetToolTipMessage(string ControlID)
        {
            return (string)s_table[ControlID];
        }
    }
}