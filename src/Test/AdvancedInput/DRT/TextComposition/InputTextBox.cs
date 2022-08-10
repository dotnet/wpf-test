// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DRT
{

public class InputTextBox : TextBox, ITestElement
{
    public InputTextBox()
    {
        _textComposition = null;

        AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(OnTextChanged), true);

        AddHandler(TextCompositionManager.PreviewTextInputStartEvent, new TextCompositionEventHandler(OnPreviewTextInputStart), true);
        AddHandler(TextCompositionManager.TextInputStartEvent, new TextCompositionEventHandler(OnTextInputStart), true);

        AddHandler(TextCompositionManager.PreviewTextInputUpdateEvent, new TextCompositionEventHandler(OnPreviewTextInputUpdate), true);
        AddHandler(TextCompositionManager.TextInputUpdateEvent, new TextCompositionEventHandler(OnTextInputUpdate), true);

        AddHandler(TextCompositionManager.PreviewTextInputEvent, new TextCompositionEventHandler(OnPreviewTextInput), true);
        AddHandler(TextCompositionManager.TextInputEvent, new TextCompositionEventHandler(OnTextInput), true);


    }


    public TextComposition TextComposition
    {
        get {return _textComposition;}
    }


    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        MyLogOut("    PreviewKeyDown         (" + e.Key + "(" + e.SystemKey + ")=" + e.KeyStates + "," + "repeat=" + e.IsRepeat + ")");
        base.OnPreviewKeyDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        MyLogOut("    KeyDown                (" + e.Key + "(" + e.SystemKey + ")=" + e.KeyStates + "," + "repeat=" + e.IsRepeat + ")");
        base.OnKeyDown(e);
    }

    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
        MyLogOut("    PreviewKeyUp           (" + e.Key + "(" + e.SystemKey + ")=" + e.KeyStates + "," + "repeat=" + e.IsRepeat + ")");
        base.OnPreviewKeyUp(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        MyLogOut("    KeyUp                  (" + e.Key + "(" + e.SystemKey + ")=" + e.KeyStates + "," + "repeat=" + e.IsRepeat + ")");
        base.OnKeyUp(e);
    }

    protected void OnPreviewTextInputStart(object sender, TextCompositionEventArgs e)
    {
        MyLogOut(CreateLogMessage("    PreviewTextInputStart  ", e));
    }

    protected void OnPreviewTextInputUpdate(object sender, TextCompositionEventArgs e)
    {
        MyLogOut(CreateLogMessage("    PreviewTextInputUpdate ", e));
    }

    protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        MyLogOut(CreateLogMessage("    PreviewTextInput       ", e));
        base.OnPreviewTextInput(e);
    }

    protected void OnTextInputStart(object sender, TextCompositionEventArgs e)
    {
        MyLogOut(CreateLogMessage("    TextInputStart         ", e));
        _textComposition = e.TextComposition;
    }

    protected void OnTextInputUpdate(object sender, TextCompositionEventArgs e)
    {
        MyLogOut(CreateLogMessage("    TextInputUpdate        ", e));
        _textComposition = e.TextComposition;
    }

    protected void OnTextInput(object sender, TextCompositionEventArgs e)
    {
        MyLogOut(CreateLogMessage("    TextInput              ", e));
        MyLogOut("        : " + CreateTextDump(e.TextComposition.Text));
        _textComposition = null;
    }

    private string CreateLogMessage(string strPre, TextCompositionEventArgs e)
    {
        string strLog = strPre;
        strLog += "(";

        FrameworkTextComposition ftc = e.TextComposition as FrameworkTextComposition;
        if (ftc != null)
        {
            strLog += ftc.Text;
            // TextPosition origin = StartPosition;
            // if (ftc.TextRange != null)
            // {
            //     strLog += ",";
            //     strLog += origin.GetDistanceTo(ftc.TextRange.Start);
            //     strLog += ",";
            //     strLog += origin.GetDistanceTo(ftc.TextRange.End);
            // }

            strLog += " - ";
            strLog += ftc.CompositionText;

            // if (ftc.CompositionTextRange != null)
            // {
            //     strLog += ",";
            //     strLog += origin.GetDistanceTo(ftc.CompositionTextRange.Start);
            //     strLog += ",";
            //     strLog += origin.GetDistanceTo(ftc.CompositionTextRange.End);
            // }

            strLog += ")";
        }
        else
        {
            strLog += e.TextComposition.Text;
            strLog += " - ";
            strLog += e.TextComposition.CompositionText;
            strLog += ")";
        }
        return strLog;
    }

    private string CreateTextDump(string srcText)
    {
        string strLog = "";

        foreach(char ch in srcText)
        {
            int n = (int)ch;
            strLog += n.ToString();
            strLog += ",";
        }
        return strLog;
    }


    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        MyLogOut("    TextChanged " + CreateTextDump(Text));
    }

    private void MyLogOut(string logstr)
    {
        Console.WriteLine(logstr);
    }

    private TextComposition _textComposition;
}

}
