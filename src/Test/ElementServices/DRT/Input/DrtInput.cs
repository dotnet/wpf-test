// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Threading;

using System.Windows.Media;
using System.Xml;
using System.Text;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using System.Windows.Forms; // For Screen class

using MS.Win32;
using MS.Internal;
using DRT;

public class DrtInput : DrtBase
{
    [STAThread]
    public static void Main(string[] args)
    {
        (new DrtInput()).Run(args);
    }

    protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
    {
        if ("refresh".Equals(arg))
        {
            RefreshMode = true;
            return true;
        }

        return base.HandleCommandLineArgument(arg, option, args, ref k);
    }

    public DrtInput()
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
        this.DrtName = "DrtInput";

        this.BlockInput = true;
        this.WindowPosition = new Point(50, 50);
        this.WindowSize = new Size(500, 500);
        this.WindowTitle = "DrtInput";

        Suites = new DrtTestSuite[]
        {
            new InputSimulationTest()
        };
    }

    public static HwndSource Window
    {
        get
        {
            return ((DrtInput)DRT).MainWindow;
        }
    }

    public static void VerifyInput(string input)
    {
        _currentSuite.VerifyInput(input);
    }

    public static InputSimulationTest _currentSuite;
    public static bool RefreshMode = false;
}

public class InputSimulationTest : DrtTestSuite
{
    public InputSimulationTest() : base("InputSimulation")
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
    }

    public HwndSource MainWindow
    {
        get
        {
            return DrtInput.Window;
        }
    }

    public override DrtTest[] PrepareTests()
    {
        DrtInput._currentSuite = this;

        _inputBoxGrid = new InputBoxGrid();
        _inputBoxGrid.VerticalDividerPercent = 50;
        _inputBoxGrid.HorizontalDividerPercent = 50;
        _xAdjustment = 1;
        _yAdjustment = 2;

        _inputBoxGrid.NW.Cursor = System.Windows.Input.Cursors.None;
        _inputBoxGrid.NE.Cursor = System.Windows.Input.Cursors.SizeNESW;
        _inputBoxGrid.SE.Cursor = System.Windows.Input.Cursors.SizeNWSE;
        _inputBoxGrid.SW.Cursor = System.Windows.Input.Cursors.SizeNESW;

        DRT.Show(_inputBoxGrid);


        if (!DRT.KeepAlive)
        {
            PrepWindowForSimulation();
            ParseInputToSimulate();

            return new DrtTest[]
            {
                new DrtTest(Start),
                new DrtTest(SimulateNextInput),
                new DrtTest(Finish),
            };
        }
        else
        {
            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Tick += new EventHandler(OnTick);
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Start();

            return new DrtTest[] {};
        }
    }

    public void Start()
    {
        _drtStarted = true;
        if(DrtInput.RefreshMode)
        {
            Console.WriteLine("<InputList>");
        }
    }


    public void Finish()
    {
        _drtStarted = false;
        if(DrtInput.RefreshMode)
        {
            Console.Write("</InputList>");
        }
    }
    
    private void PrepWindowForSimulation()
    {
        DRT.MoveMouse(new Point(0, 0));

        // Find the point on the screen of the upper-left corner of the
        // client area of our window.
        _rcScreen = Screen.PrimaryScreen.Bounds;
        _origin.X = 0;
        _origin.Y = 0;
        _origin = PointUtil.ClientToScreen(_origin, MainWindow);

        if(SystemParameters.SwapButtons)
        {
            _mouseSwapped = true;
        }
        else
        {
            _mouseSwapped = false;
        }
    }


    [DllImport("user32.dll", CharSet=CharSet.Auto)]
    private static extern short VkKeyScan(char key);

    private void ParseInputToSimulate()
    {
        int _lastX = 0;
        int _lastY = 0;

        // First, build up a list of input events to simulate.
        XmlTextReader reader = new XmlTextReader(@"DrtFiles\Input\DrtInput.xml");
        string attribute = null;
        while(reader.Read())
        {
            if(reader.NodeType == XmlNodeType.Element)
            {
                if(reader.Name == "InputList")
                {
                }
                else if(reader.Name == "InputResponse")
                {
                    attribute = reader["Response"];
                    if(attribute != null)
                    {
                        _inputList.Add(attribute);
                    }
                }
                else if(reader.Name == "MouseInput")
                {
                    ScriptMouseInput scriptMouseInput = new ScriptMouseInput();
                    MouseInput mouseLeftButtonInput = new MouseInput();
                    MouseInput mouseRightButtonInput = new MouseInput();
                    MouseInput mouseMiddleButtonInput = new MouseInput();
                    MouseInput mouseMoveInput = new MouseInput();
                    MouseInput mouseWheelInput = new MouseInput();

                    attribute = reader["MoveTo"];
                    if(attribute != null)
                    {
                        string[] paramList = attribute.Split(new char[] {','});
                        if(paramList.Length == 3)
                        {
                            scriptMouseInput.MoveTo = true;
                            scriptMouseInput.X = Int32.Parse(paramList[0]);
                            scriptMouseInput.Y = Int32.Parse(paramList[1]);
                            scriptMouseInput.Steps = Int32.Parse(paramList[2]);

                            mouseMoveInput.X = scriptMouseInput.X;
                            mouseMoveInput.Y = scriptMouseInput.Y;
                            mouseMoveInput.Flags |= NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE;
                        }
                        else
                        {
                            DRT.Fail("MoveTo attribute of MouseInput element is malformed.");
                        }
                    }

                    attribute = reader["Left"];
                    if(attribute != null)
                    {
                        scriptMouseInput.LeftButton = true;

                        if(attribute == "Down")
                        {
                            scriptMouseInput.Left = true;
                            if(_mouseSwapped)
                            {
                                mouseLeftButtonInput.Flags |= NativeMethods.MOUSEEVENTF_RIGHTDOWN;
                            }
                            else
                            {
                                mouseLeftButtonInput.Flags |= NativeMethods.MOUSEEVENTF_LEFTDOWN;
                            }
                        }
                        else if(attribute == "Up")
                        {
                            scriptMouseInput.Left = false;
                            if(_mouseSwapped)
                            {
                                mouseLeftButtonInput.Flags |= NativeMethods.MOUSEEVENTF_RIGHTUP;
                            }
                            else
                            {
                                mouseLeftButtonInput.Flags |= NativeMethods.MOUSEEVENTF_LEFTUP;
                            }
                        }
                        else
                        {
                            DRT.Fail("Left attribute of MouseInput element is malformed.");
                        }
                    }

                    attribute = reader["Right"];
                    if(attribute != null)
                    {
                        scriptMouseInput.RightButton = true;

                        if(attribute == "Down")
                        {
                            scriptMouseInput.Right = true;
                            if(_mouseSwapped)
                            {
                                mouseRightButtonInput.Flags |= NativeMethods.MOUSEEVENTF_LEFTDOWN;
                            }
                            else
                            {
                                mouseRightButtonInput.Flags |= NativeMethods.MOUSEEVENTF_RIGHTDOWN;
                            }
                        }
                        else if(attribute == "Up")
                        {
                            scriptMouseInput.Right = false;
                            if(_mouseSwapped)
                            {
                                mouseRightButtonInput.Flags |= NativeMethods.MOUSEEVENTF_LEFTUP;
                            }
                            else
                            {
                                mouseRightButtonInput.Flags |= NativeMethods.MOUSEEVENTF_RIGHTUP;
                            }
                        }
                        else
                        {
                            DRT.Fail("Left attribute of MouseInput element is malformed.");
                        }
                    }

                    attribute = reader["Middle"];
                    if(attribute != null)
                    {
                        scriptMouseInput.MiddleButton = true;

                        if(attribute == "Down")
                        {
                            scriptMouseInput.Middle = true;
                            mouseMiddleButtonInput.Flags |= NativeMethods.MOUSEEVENTF_MIDDLEDOWN;
                        }
                        else if(attribute == "Up")
                        {
                            scriptMouseInput.Middle = false;
                            mouseMiddleButtonInput.Flags |= NativeMethods.MOUSEEVENTF_MIDDLEUP;
                        }
                        else
                        {
                            DRT.Fail("Left attribute of MouseInput element is malformed.");
                        }
                    }

                    attribute = reader["Wheel"];
                    if(attribute != null)
                    {
                        scriptMouseInput.Wheel = true;
                        scriptMouseInput.WheelData = Int32.Parse(attribute);

                        mouseWheelInput.Flags |= NativeMethods.MOUSEEVENTF_WHEEL;
                        mouseWheelInput.Data = Int32.Parse(attribute);
                    }

                    // Add the script mouse input first.
                    _inputList.Add(scriptMouseInput);

                    // Generate all of the intermediate moves.
                    if(scriptMouseInput.MoveTo)
                    {
                        for(int step = 0; step < scriptMouseInput.Steps; step++)
                        {
                            MouseInput mouseInputStep = new MouseInput();
                            mouseInputStep.Flags = NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE;
                            mouseInputStep.X = (((mouseMoveInput.X - _lastX) * step)/scriptMouseInput.Steps) + _lastX;
                            mouseInputStep.Y = (((mouseMoveInput.Y - _lastY) * step)/scriptMouseInput.Steps) + _lastY;
                            _inputList.Add(mouseInputStep);
                        }

                        _lastX = mouseMoveInput.X;
                        _lastY = mouseMoveInput.Y;
                    }

                    // Add the mouse events in a very specific order:
                    // 1) Move event
                    // 2) Left button event
                    // 3) Right button event
                    // 4) Middle button event
                    // 5) Wheel event
                    if(scriptMouseInput.MoveTo)
                    {
                        _inputList.Add(mouseMoveInput);
                    }
                    if(scriptMouseInput.LeftButton)
                    {
                        _inputList.Add(mouseLeftButtonInput);
                    }
                    if(scriptMouseInput.RightButton)
                    {
                        _inputList.Add(mouseRightButtonInput);
                    }
                    if(scriptMouseInput.MiddleButton)
                    {
                        _inputList.Add(mouseMiddleButtonInput);
                    }
                    if(scriptMouseInput.Wheel)
                    {
                        _inputList.Add(mouseWheelInput);
                    }
                }
                else if(reader.Name == "KeyboardInput")
                {
                    ScriptKeyboardInput scriptKeyboardInput = new ScriptKeyboardInput();

                    attribute = reader["KeySequence"];
                    if(attribute != null)
                    {
                        scriptKeyboardInput.KeySequence = true;
                        scriptKeyboardInput.KeySequenceData = attribute;
                    }

                    // Add the script keyboard input first.
                    _inputList.Add(scriptKeyboardInput);

                    // Generate all of the intermediate key strokes.
                    if(scriptKeyboardInput.KeySequence)
                    {
                        for(int i = 0; i < scriptKeyboardInput.KeySequenceData.Length; i++)
                        {
                            char ch = scriptKeyboardInput.KeySequenceData[i];
                            byte virtualKey = (byte) VkKeyScan(ch);

                            // Down
                            KeyboardInput keyboardInput = new KeyboardInput();
                            keyboardInput.VirtualKey = virtualKey;
                            _inputList.Add(keyboardInput);

                            // Up
                            keyboardInput = new KeyboardInput();
                            keyboardInput.VirtualKey = virtualKey;
                            keyboardInput.Flags = NativeMethods.KEYEVENTF_KEYUP;
                            _inputList.Add(keyboardInput);
                        }
                    }
                }
                else if(reader.Name == "ChangeTree")
                {
                    _inputList.Add(new ChangeTree());
                }
                else
                {
                    DRT.Fail("Unknown opening element: " + reader.Name);
                }
            }
        }
    }

    private void SimulateNextInput()
    {
        object input = null;

        while(_inputIndex < _inputList.Count && input == null)
        {
            input = _inputList[_inputIndex];
            _inputIndex++;
        }

        if(input != null)
        {
            if(input is ScriptMouseInput)
            {
                ScriptMouseInput scriptMouseInput = (ScriptMouseInput) input;

                if(DrtInput.RefreshMode || DRT.Verbose)
                {
                    Console.Write("    <MouseInput");

                    if(scriptMouseInput.MoveTo)
                    {
                        Console.Write(" MoveTo=\"" + scriptMouseInput.X + "," + scriptMouseInput.Y + "," + scriptMouseInput.Steps + "\"");
                    }

                    if(scriptMouseInput.RightButton)
                    {
                        Console.Write(" Right=\"" + (scriptMouseInput.Right ? "Down" : "Up") + "\"");
                    }

                    if(scriptMouseInput.LeftButton)
                    {
                        Console.Write(" Left=\"" + (scriptMouseInput.Left ? "Down" : "Up") + "\"");
                    }

                    if(scriptMouseInput.MiddleButton)
                    {
                        Console.Write(" Middle=\"" + (scriptMouseInput.Middle ? "Down" : "Up") + "\"");
                    }

                    if(scriptMouseInput.Wheel)
                    {
                        Console.Write(" Wheel=\"" + scriptMouseInput.WheelData + "\"");
                    }

                    Console.WriteLine("/>");
                }
            }
            else if(input is ScriptKeyboardInput)
            {
                ScriptKeyboardInput scriptKeyboardInput = (ScriptKeyboardInput) input;

                if(DrtInput.RefreshMode || DRT.Verbose)
                {
                    Console.Write("    <KeyboardInput");

                    if(scriptKeyboardInput.KeySequence)
                    {
                        Console.Write(" KeySequence=\"" + scriptKeyboardInput.KeySequenceData + "\"");
                    }

                    Console.WriteLine("/>");
                }
            }
            else if(input is MouseInput)
            {
                MouseInput mouseInput = (MouseInput) input;

                int screenX = mouseInput.X + (int) _origin.X;
                int screenY = mouseInput.Y + (int) _origin.Y;

                // Double-Check that the mouse coordinates are over the
                // DRT window.
                NativeMethods.RECT rcWindow = new NativeMethods.RECT();
                SafeNativeMethods.GetWindowRect(new HandleRef(null, MainWindow.Handle), ref rcWindow);
                if(screenX >= rcWindow.left && screenX <= rcWindow.right && screenY >= rcWindow.top && screenY <= rcWindow.bottom)
                {
                    IntPtr hwndOver = UnsafeNativeMethods.WindowFromPoint(screenX, screenY);
                    if(hwndOver != MainWindow.Handle)
                    {
                        string error = "WARNING: DrtInput is trying to send input to its HWND at (" + screenX + ", " + screenY + ") , but another HWND is in the way!\n";
                        error += "DRT Window: " + MainWindow.Handle.ToString("X") + " \"" + GetWindowTitle(MainWindow.Handle) + "\"\n";
                        error += "Bad Window: " + hwndOver.ToString("X") + " \"" + GetWindowTitle(hwndOver) + "\"\n";

                        DRT.Fail(error);
                    }
                }

                // Convert the device-units into normalized screen space.
                // We find the center-point of the pixel in normalized screen
                // space by finding the left/right/top/bottom edges and
                // calculating the middle.
                double normalizedX1 = (65535.0 * (screenX)) / ((double) _rcScreen.Width);
                double normalizedY1 = (65535.0 * (screenY)) / ((double) _rcScreen.Height);
                double normalizedX2 = (65535.0 * (screenX + 1)) / ((double) _rcScreen.Width);
                double normalizedY2 = (65535.0 * (screenY + 1)) / ((double) _rcScreen.Height);

                int normalizedX = (int) (normalizedX1 + ((normalizedX2 - normalizedX1)/2.0));
                int normalizedY = (int) (normalizedY1 + ((normalizedY2 - normalizedY1)/2.0));

                MS.Win32.UnsafeNativeMethods.Mouse_event(mouseInput.Flags, normalizedX, normalizedY, mouseInput.Data, mouseInput.ExtraInfo);
            }
            else if(input is KeyboardInput)
            {
                KeyboardInput keyboardInput = (KeyboardInput) input;

                MS.Win32.UnsafeNativeMethods.Keybd_event(keyboardInput.VirtualKey, keyboardInput.ScanCode, keyboardInput.Flags, keyboardInput.ExtraInfo);
            }
            else if(input is ChangeTree)
            {
                if(DrtInput.RefreshMode || DRT.Verbose)
                {
                    Console.WriteLine("    <ChangeTree/>");
                }

                _inputBoxGrid.ChangeTree();
            }
            else if(input is string)
            {
                // In refresh mode, we ignore all existing InputResponses.
                if(!DrtInput.RefreshMode)
                {
                    DRT.Fail("Failed to receive event: " + input.ToString());
                }
            }
            else
            {
                DRT.Fail("Unknown input to simulate: " + input.ToString());
            }

            DRT.ResumeAt(new DrtTest(SimulateNextInput));
        }

    }

    public void VerifyInput(string inputToVerify)
    {
        if( _drtStarted && ( DRT.Verbose || (!DRT.KeepAlive && DrtInput.RefreshMode) ) )
        {
            Console.WriteLine("    <InputResponse Response=\"" + inputToVerify + "\"/>");

            if(DrtInput.RefreshMode)
            {
                return;
            }
        }

        if(!DRT.KeepAlive && _drtStarted)
        {
            if(_inputIndex < _inputList.Count)
            {
                // Skip over any MouseInput or KeyboardInput objects.  These
                // are the implementation details of the ScriptMouseInput
                // and ScriptKeyboardInput objects.
                object input = null;
                for(int i = _inputIndex; i < _inputList.Count; i++)
                {
                    input = _inputList[i];
                    if(input is string)
                    {
                        string inputString = (string) input;
                        _inputList[i] = null;

                        if(!inputToVerify.Equals(inputString))
                        {
                            DRT.Fail("Unexpected input: expected '" + inputString + "', received '" + inputToVerify + "'.");
                        }
                        break;
                    }
                    else if(input is ScriptMouseInput)
                    {
                        DRT.Fail("Unexpected input: expected '<nothing>', received '" + inputToVerify + "'.");
                    }
                    else if(input is ScriptKeyboardInput)
                    {
                        DRT.Fail("Unexpected input: expected '<nothing>', received '" + inputToVerify + "'.");
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                DRT.Fail("Unexpected input: expected '<nothing>', received '" + inputToVerify + "'.");
            }
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        int x = _inputBoxGrid.HorizontalDividerPercent;
        x += _xAdjustment;
        if(x > 90)
        {
            x = 90;
            _xAdjustment = 0 - _xAdjustment;
        }
        else if(x < 10)
        {
            x = 10;
            _xAdjustment = 0 - _xAdjustment;
        }
        _inputBoxGrid.HorizontalDividerPercent = x;

        int y = _inputBoxGrid.VerticalDividerPercent;
        y += _yAdjustment;
        if(y > 90)
        {
            y = 90;
            _yAdjustment = 0 - _yAdjustment;
        }
        else if(y < 10)
        {
            y = 10;
            _yAdjustment = 0 - _yAdjustment;
        }
        _inputBoxGrid.VerticalDividerPercent = y;
    }

    private static string GetWindowTitle(IntPtr hwnd)
    {
        StringBuilder sb = new StringBuilder(500);
        UnsafeNativeMethods.GetWindowText(new HandleRef(null, hwnd), sb, 500);
        return sb.ToString();
    }

    private System.Drawing.Rectangle _rcScreen = new System.Drawing.Rectangle();
    private Point _origin = new Point();
    private bool _mouseSwapped = false;

    private bool _drtStarted = false;
    private ArrayList _inputList = new ArrayList();
    private int _inputIndex = 0;
    private ArrayList _responseList = new ArrayList();

    private InputBoxGrid _inputBoxGrid;
    private DispatcherTimer _timer;
    private int _xAdjustment;
    private int _yAdjustment;
}

public struct ScriptMouseInput
{
    public bool MoveTo;
    public int X;
    public int Y;
    public int Steps;
    public bool LeftButton;
    public bool Left;
    public bool RightButton;
    public bool Right;
    public bool MiddleButton;
    public bool Middle;
    public bool Wheel;
    public int WheelData;
}

public struct MouseInput
{
    public int Flags;
    public int X;
    public int Y;
    public int Data;
    public IntPtr ExtraInfo;
}

public struct ScriptKeyboardInput
{
    public bool KeySequence;
    public string KeySequenceData;
}

public struct KeyboardInput
{
    public byte VirtualKey;
    public byte ScanCode;
    public int Flags;
    public IntPtr ExtraInfo;
}

public struct ChangeTree
{
}

