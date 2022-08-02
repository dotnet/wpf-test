// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Interop;
using System.Xml;
using System.Text;
using System.Runtime.InteropServices;
using MS.Win32;
using MS.Internal;
using DRT;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;

using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Forms; // For Screen class

public class DrtCommanding : DrtBase
{
    [STAThread]
    public static void Main(string[] args)
    {
        (new DrtCommanding()).Run(args);
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

    public DrtCommanding()
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
        this.DrtName = "DrtCommanding";

        this.BlockInput = true;
        this.WindowPosition = new Point(50, 50);
        this.WindowSize = new Size(500, 500);
        this.WindowTitle = "DrtCommanding";

        Suites = new DrtTestSuite[]
        {
            new CommandMarkupLoadTest(),
            new CommandConvertersTest(),
            new CommandSimulationTest(),
            new TestCommandOverride(),
         };
    }

    public static HwndSource Window
    {
        get
        {
            return ((DrtCommanding)DRT).MainWindow;
        }
    }

    public static void VerifyInput(string input)
    {
        if (_currentSuite is CommandSimulationTest)
            ((CommandSimulationTest)_currentSuite).VerifyInput(input);
    }

    public static DrtTestSuite _currentSuite;
    public static bool RefreshMode = false;
}

// Base for all CommandMarkup tests.
public class CommandMarkupSuites : DrtTestSuite
{
    public CommandMarkupSuites(string name) : base (name)
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
    }

    // Loads up a test file.
    // File is relative to the TestFileDir
    protected Stream LoadTestFile(string fileName)
    {
        string testFile = TestFileDir + fileName;
        System.IO.Stream stream = File.OpenRead(testFile);

        return stream;
    }

    string TestFileDir = ".\\"; // directory path for test files, include \ at end.

}

// Simple load of a file that contains commanding elements.
public class CommandMarkupLoadTest : CommandMarkupSuites
{
    public CommandMarkupLoadTest(): base("CommandMarkup Load Test")
    {
    }

    public override DrtTest[] PrepareTests()
    {
        // return the lists of tests to run against the tree
        return new DrtTest[]{
                        new DrtTest( RunLoadTest ),
                        };
    }

    private void RunLoadTest()
    {
        Console.WriteLine("CommandMarkup Load Started.");
        Stream xamlFileStream = LoadTestFile(@"DrtFiles\Commanding\DrtCommandingMarkup.xaml");
        UIElement root = null;
        int startT;
        int endT;

        try
        {
            // see if it loads
            startT = Environment.TickCount;
            root = (UIElement)XamlReader.Load(xamlFileStream);
            endT = Environment.TickCount;
        }
        finally
        {

            // done with the stream
            xamlFileStream.Close();
        }

        Console.WriteLine("CommandMarkupLoadTest finished - Time = " + (endT - startT) + "ms");
        Console.WriteLine();
    }
}

// Base for all CommandMarkup tests.
public class CommandConvertersTest : DrtTestSuite
{
    public CommandConvertersTest() : base ("Command Converters Test")
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
    }

    public override DrtTest[] PrepareTests()
    {
        // return the lists of tests to run against the tree
        return new DrtTest[]{
                        new DrtTest( RunConvertersTest ),
                        };
    }

    private void RunConvertersTest()
    {
        Console.WriteLine("Converters Test Started.");
        Console.WriteLine();
        Console.WriteLine("Checking KeyGesture Converter :");
        CheckKeyGestureConverter();
        Console.WriteLine("Checking KeyGesture Converter finished.");
        Console.WriteLine("Checking KeyConverer :");
        CheckKeyConverter();
        Console.WriteLine("Checking KeyConverter finished.");
        Console.WriteLine("Checking CommandConverter :");
        CheckCommandConverter();
        Console.WriteLine("Checking CommandConverter finished.");
        Console.WriteLine();
        Console.WriteLine("Converters Test Finished.");

    }

    private void CheckKeyGestureConverter()
    {
        TypeConverter keyGestureConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(KeyGesture));
        if (!(keyGestureConverter is KeyGestureConverter))
        {
            DRT.Fail("Couldn't get KeyGesture TypeConverter");
        }
        // Trying multiple modifiers and a single Char key
        KeyGesture keyGesture = new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt);
        string strKeyGesture = keyGestureConverter.ConvertToInvariantString(keyGesture);
        Console.WriteLine("keyGestureConverter CovertTo returned : " + strKeyGesture);
        KeyGesture keyGestureFrom = (KeyGesture)keyGestureConverter.ConvertFromInvariantString(strKeyGesture);
        if (keyGestureFrom != null && keyGestureFrom.Equals(keyGesture))
            Console.WriteLine("keyGestureConverter CovertFrom Success");
        else
            Console.WriteLine("keyGestureConverter CovertFrom Failed");

        // Trying Control Modifier and F3 key
        keyGesture = new KeyGesture(Key.F3, ModifierKeys.Control);
        strKeyGesture = keyGestureConverter.ConvertToInvariantString(keyGesture);
        Console.WriteLine("keyGestureConverter CovertTo returned : " + strKeyGesture);
        keyGestureFrom = (KeyGesture)keyGestureConverter.ConvertFromInvariantString(strKeyGesture);
        if (keyGestureFrom != null && IsGesturesEqual(keyGestureFrom, keyGesture))
        {
            Console.WriteLine("keyGestureConverter CovertFrom Success");
        }
        else
        {
            Console.WriteLine("keyGestureConverter CovertFrom Failed");
        }

        // Trying Windows Modifier and a single Char key
        keyGesture = new KeyGesture(Key.D, ModifierKeys.Windows);
        strKeyGesture = keyGestureConverter.ConvertToInvariantString(keyGesture);
        Console.WriteLine("keyGestureConverter CovertTo returned : " + strKeyGesture);
        keyGestureFrom = (KeyGesture)keyGestureConverter.ConvertFromInvariantString(strKeyGesture);
        if (keyGestureFrom != null && IsGesturesEqual(keyGestureFrom, keyGesture))
            Console.WriteLine("keyGestureConverter CovertFrom Success");
        else
            Console.WriteLine("keyGestureConverter CovertFrom Failed");

        keyGestureFrom = (KeyGesture)keyGestureConverter.ConvertFromInvariantString(String.Empty);
        if (keyGestureFrom.Key != Key.None || keyGestureFrom.Modifiers != ModifierKeys.None)
        {
            DRT.Fail("keyGestureConverter CovertFrom Failed for String.Empty");
        }
        Console.WriteLine("keyGestureConverter CovertFrom passed for String.Empty");
    }

    internal bool IsGesturesEqual(KeyGesture keyGesture1, KeyGesture keyGesture2)
    {
        if (keyGesture1 != null && keyGesture2 != null)
            return (((int)keyGesture1.Key == (int)keyGesture2.Key) && (keyGesture1.Modifiers == keyGesture2.Modifiers));

        return false;
    }

    private void CheckCommandConverter()
    {
        TypeConverter commandConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(RoutedCommand));
        if (!(commandConverter is CommandConverter))
        {
            DRT.Fail("Couldn't get TypeConverter for RoutedCommand ");
        }

        Console.Write("CommandConverter ConvertTo for ApplicationCommands.Cut : ");
        string strCommand = commandConverter.ConvertToInvariantString(ApplicationCommands.Cut);
        if (strCommand != "Cut")
        {
            DRT.Fail("CommandConverter ConvertTo for ApplicationCommands.Cut : " + strCommand + " : failed");
        }
        Console.WriteLine(strCommand + " : working");
    }

    private void CheckKeyConverter()
    {
        TypeConverter keyConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Windows.Input.Key));
        if (!(keyConverter is System.Windows.Input.KeyConverter))
        {
            DRT.Fail("Couldn't get Key TypeConverter for Key");
        }

        int[] values = (int[])Enum.GetValues(typeof(Key));
        foreach (int i in values)
        {
            Key key = (Key)i;
            string strKey = keyConverter.ConvertToInvariantString(key);
            Key keyFrom = (Key)keyConverter.ConvertFromInvariantString(strKey);
            if (!Enum.IsDefined(typeof(Key), keyFrom))
            {
                DRT.Fail("KeyConverter CovertFrom FAILED : Input : " + Enum.GetName(typeof(Key), key) + "Output : " + strKey );
            }
        }
        Console.WriteLine("KeyConverter ConvertFrom/To for all known keys result : SUCCESS ");
    }
}

public class CommandSimulationTest : DrtTestSuite
{
    public CommandSimulationTest() : base("CommandSimulation")
    {
        this.TeamContact = "WPF";
        this.Contact     = "Microsoft";
    }

    public HwndSource MainWindow
    {
        get
        {
            return DrtCommanding.Window;
        }
    }

    public override DrtTest[] PrepareTests()
    {
        DrtCommanding._currentSuite = this;

        _drtCommandingParent = new DrtCommandingBox();
        DRT.RootElement = _drtCommandingParent;
        DRT.Show(_drtCommandingParent);


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
        if(DrtCommanding.RefreshMode)
        {
            Console.WriteLine("<InputList>");
        }
    }

    public void Finish()
    {
        _drtStarted = false;
        if(DrtCommanding.RefreshMode)
        {
            Console.Write("</InputList>");
        }

    }

    private void OnTick(object sender, EventArgs e)
    {
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

        _mouseSwapped = SystemParameters.SwapButtons;
    }

    private void ParseInputToSimulate()
    {
        int _lastX = 0;
        int _lastY = 0;

        // First, build up a list of input events to simulate.
        XmlTextReader reader = new XmlTextReader(@"DrtFiles\Commanding\DrtCommandingVerify.xml");
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

                    attribute = reader["Modifiers"];
                    scriptKeyboardInput.ModifiersData = attribute;

                    // Add the script keyboard input first.
                    _inputList.Add(scriptKeyboardInput);

                    bool bControl = false, bShift = false, bAlt = false;

                    if(attribute != null)
                    {
                        string modifiers = attribute ;
                        if (modifiers.IndexOf("Shift") != -1)   bShift = true;
                        if (modifiers.IndexOf("Control") != -1) bControl = true;
                        if (modifiers.IndexOf("Alt") != -1)     bAlt = true;
                    }

                    // Generate all of the intermediate key strokes.
                    if(scriptKeyboardInput.KeySequence)
                    {
                        if (bShift)
                        {
                            // Down
                            KeyboardInput keyboardInput = new KeyboardInput();
                            keyboardInput.VirtualKey = (byte)(int)0x10;
                            _inputList.Add(keyboardInput);
                        }

                        if (bControl)
                        {
                            // Down
                            KeyboardInput keyboardInput = new KeyboardInput();
                            keyboardInput.VirtualKey = (byte)(int)0x11;
                            _inputList.Add(keyboardInput);
                        }

                        if (bAlt)
                        {
                            // Down
                            KeyboardInput keyboardInput = new KeyboardInput();
                            keyboardInput.VirtualKey = (byte)(int)0x12;
                            _inputList.Add(keyboardInput);
                        }

                        if (scriptKeyboardInput.KeySequenceData.Length > 0)
                        {
                            byte virtualKey = (byte)GetVKeyFromString(scriptKeyboardInput.KeySequenceData);

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

                        // Up
                        if (bShift)
                        {
                            // Down
                            KeyboardInput keyboardInput = new KeyboardInput();
                            keyboardInput.VirtualKey = (byte)(int)0x10;
                            keyboardInput.Flags = NativeMethods.KEYEVENTF_KEYUP;
                            _inputList.Add(keyboardInput);
                        }

                        if (bControl)
                        {
                            // Down
                            KeyboardInput keyboardInput = new KeyboardInput();
                            keyboardInput.VirtualKey = (byte)(int)0x11;
                            keyboardInput.Flags = NativeMethods.KEYEVENTF_KEYUP;
                            _inputList.Add(keyboardInput);
                        }

                        if (bAlt)
                        {
                            // Down
                            KeyboardInput keyboardInput = new KeyboardInput();
                            keyboardInput.VirtualKey = (byte)(int)0x12;
                            keyboardInput.Flags = NativeMethods.KEYEVENTF_KEYUP;
                            _inputList.Add(keyboardInput);
                        }
                    }
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

                if(DrtCommanding.RefreshMode || DRT.Verbose)
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

                if(DrtCommanding.RefreshMode || DRT.Verbose)
                {
                    Console.Write("    <KeyboardInput");

                    if(scriptKeyboardInput.KeySequence)
                    {
                        Console.Write(" KeySequence=\"" + scriptKeyboardInput.KeySequenceData + "\"");
                        if(scriptKeyboardInput.ModifiersData != null && scriptKeyboardInput.ModifiersData.Length > 0)
                        {
                            Console.Write(" Modifiers=\"" + scriptKeyboardInput.ModifiersData + "\"");
                        }
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
                        string error = "WARNING: DrtCommanding is trying to send input to its HWND at (" + screenX + ", " + screenY + ") , but another HWND is in the way!\n";
                        error += "DRT Window: " + MainWindow.Handle.ToString("X") + " \"" + GetWindowTitle(MainWindow.Handle) + "\"\n";
                        error += "Bad Window: " + hwndOver.ToString("X") + " \"" + GetWindowTitle(hwndOver) + "\"\n";

                        DRT.Fail(error);
                    }
                }

                IntPtr activeHwnd = GetForegroundWindow();
                string activeWindowTitle = GetWindowTitle(activeHwnd);
                DRT.Assert(MainWindow.Handle == activeHwnd,
                        "Main DRT window " + MainWindow.Handle.ToString("X") + " should be foreground but instead hwnd "
                        + activeHwnd.ToString("X") + " (" + activeWindowTitle + ") was foreground.");

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
            else if(input is string)
            {
                // In refresh mode, we ignore all existing InputResponses.
                if(!DrtCommanding.RefreshMode)
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

    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    private static extern IntPtr GetForegroundWindow();

    public void VerifyInput(string inputToVerify)
    {
        if(DRT.Verbose || (!DRT.KeepAlive && _drtStarted && DrtCommanding.RefreshMode))
        {
            Console.WriteLine("    <InputResponse Response=\"" + inputToVerify + "\"/>");

            if(DrtCommanding.RefreshMode)
            {
                return;
            }
        }

        if (!DRT.KeepAlive && _drtStarted)
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

                        if (!inputToVerify.Equals(inputString))
                        {
                            DRT.Fail("Unexpected input: expected '" + inputString + "', received '" + inputToVerify + "'.");
                        }
                        break;
                    }
                    else if(input is ScriptKeyboardInput)
                    {
                        DRT.Fail("Unexpected input: expected '<nothing>', received '" + inputToVerify + "'.");
                    }
                }
            }
            else
            {
                DRT.Fail("Unexpected input: expected '<nothing>', received '" + inputToVerify + "'.");
            }
        }
    }

    [DllImport("user32.dll", CharSet=CharSet.Auto)]
    private static extern short VkKeyScan(char key);

    private byte GetVKeyFromString( string strKey )
    {
        if (strKey.Length > 1)
        {
            TypeConverter keyConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Windows.Input.Key));
            Key key = (Key)keyConverter.ConvertFromInvariantString(strKey);
            return (byte)KeyInterop.VirtualKeyFromKey(key);
        }
        else
        {
            char ch = strKey[0];
            return ((byte)VkKeyScan(ch));
	    }
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
    private DrtCommandingBox _drtCommandingParent;
    private DispatcherTimer _timer;

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
    public string ModifiersData;
}

public struct KeyboardInput
{
    public byte VirtualKey;
    public byte ScanCode;
    public int Flags;
    public IntPtr ExtraInfo;
}


