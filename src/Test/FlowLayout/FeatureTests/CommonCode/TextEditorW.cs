// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Test.Layout
{
	public class TextEditorW 
	{
		static Type s_textEditorType;
		static BindingFlags s_FULL_ACCESS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		static TextEditorW()
		{
			s_textEditorType = typeof(FrameworkElement).Assembly.GetType("System.Windows.Documents.TextEditor");
		}

		static public TextSelection GetTextSelection(FrameworkElement fe) 
		{
			MethodInfo mi = s_textEditorType.GetMethod("GetTextSelection", s_FULL_ACCESS);
			return mi.Invoke(null, new object[] { fe }) as TextSelection;
		}
	}

	public class InputW
	{
		#region Input emulation support.

		#region Keyboard support.
		[DllImport("user32.dll", EntryPoint = "SendInput")]
		private static extern uint SendKeyboardInput(uint nInputs,
			KeyboardInput[] pInputs, int cbSize);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern ushort VkKeyScan(char ch);
		private const uint INPUT_KEYBOARD = 1;

		[StructLayout(LayoutKind.Sequential)]
		internal struct KeyboardInput
		{
			public IntPtr type;

			/// <summary>
			/// Specifies a virtual-key code.
			/// </summary>
			public ushort wVk;          // 16
			/// <summary>
			/// Specifies a hardware scan code for the key.
			/// </summary>
			public ushort wScan;        // 16 - 32
			/// <summary>
			/// Specifies various aspects of a keystroke.
			/// </summary>
			public uint dwFlags;        // 32 - 64
			/// <summary>
			/// Time stamp for the event, in milliseconds.
			/// </summary>
			public IntPtr time;           // 32 - 96
			/// <summary>
			/// Specifies an additional value associated with the keystroke.
			/// </summary>
			public IntPtr dwExtraInfo;  // 32 - 128

			public uint pad1;           // 32 - 160
			public uint pad2;           // 32 - 192

			/// <summary>Copy constructor.</summary>
			/// <param name='keyboardInput'>Keyboard input to copy.</param>
			public KeyboardInput(KeyboardInput keyboardInput)
			{
				type = new IntPtr(INPUT_KEYBOARD);
				this.wVk = keyboardInput.wVk;
				this.wScan = keyboardInput.wScan;
				this.dwFlags = keyboardInput.dwFlags;
				this.time = keyboardInput.time;
				this.dwExtraInfo = keyboardInput.dwExtraInfo;
				this.pad1 = keyboardInput.pad1;
				this.pad2 = keyboardInput.pad2;
			}
		}

		/// <summary>
		/// If specified, the scan code was preceded by a prefix
		/// byte that has the value 0xE0 (224).
		/// </summary>
		internal const uint KEYEVENTF_EXTENDEDKEY = 1;
		/// <summary>
		/// If specified, the key is being released. If not specified, the
		/// key is being pressed.
		/// </summary>
		internal const uint KEYEVENTF_KEYUP = 2;
		/// <summary>
		/// Windows 2000/XP: If specified, the system synthesizes a
		/// VK_PACKET keystroke. The wVk parameter must be zero.
		/// </summary>
		internal const uint KEYEVENTF_UNICODE = 4;
		/// <summary>
		/// If specified, wScan identifies the key and wVk is ignored.
		/// </summary>
		internal const uint KEYEVENTF_SCANCODE = 8;

		internal const byte VK_SHIFT = 0x10;

		internal static void PressShift()
		{
			KeyboardInput[] input = new KeyboardInput[1];
			input[0].type = new IntPtr(INPUT_KEYBOARD);
			input[0].wVk = VK_SHIFT;
			unsafe
			{
				SendKeyboardInput((uint)input.Length, input, sizeof(KeyboardInput));
			}
		}

		internal static void ReleaseShift()
		{
			KeyboardInput[] input = new KeyboardInput[1];
			input[0].type = new IntPtr(INPUT_KEYBOARD);
			input[0].wVk = VK_SHIFT;
			input[0].dwFlags = KEYEVENTF_KEYUP;
			unsafe
			{
				SendKeyboardInput((uint)input.Length, input, sizeof(KeyboardInput));
			}
		}

		/// <summary>Emulates typing on the keyboard.</summary>
		/// <param name='text'>Text to type.</param>
		/// <remarks>
		/// Case is not respected - everything goes in lowercase.
		/// To get uppercase characters, add a "+" in front of the
		/// character. The original design had the "+" toggle the
		/// shift state, but by resetting it we make text string
		/// compatible with CLR's SendKeys.Send.
		/// <para />
		/// Eg, to type "Hello, WORLD!", pass "+hello, +W+O+R+L+D+1"
		/// <para />
		/// This method has not been globalized to keep it simple.
		/// Non-US keyboard may break this functionality.
		/// </remarks>
		public static void KeyboardType(string text)
		{
			ArrayList list = new ArrayList();
			bool shiftIsPressed = false;
			bool controlIsPressed = false;
			const byte VK_CONTROL = 0x11;
			const byte VK_RETURN = 0x0D;
			const byte VK_END = 0x23;
			const byte VK_HOME = 0x24;
			const byte VK_LEFT = 0x25;
			const byte VK_UP = 0x26;
			const byte VK_RIGHT = 0x27;
			const byte VK_DOWN = 0x28;

			int i = 0;
			while (i < text.Length)
			{
				char c = text[i];
				if (c == '+')
				{
					KeyboardInput input = new KeyboardInput();
					input.type = new IntPtr(INPUT_KEYBOARD);
					input.wVk = VK_SHIFT;
					if (shiftIsPressed)
						input.dwFlags = KEYEVENTF_KEYUP;
					list.Add(input);
					shiftIsPressed = !shiftIsPressed;
					i++;
				}
				else if (c == '^')
				{
					KeyboardInput input = new KeyboardInput();
					input.type = new IntPtr(INPUT_KEYBOARD);
					input.wVk = VK_CONTROL;
					if (controlIsPressed)
						input.dwFlags = KEYEVENTF_KEYUP;
					list.Add(input);
					controlIsPressed = !controlIsPressed;
					i++;
				}
				else if (c == '{')
				{
					i++;
					int closeIndex = text.IndexOf('}', i);
					if (closeIndex == -1)
					{
						throw new ArgumentException(
							"Malformed typing text: no closing '}' to match " +
							"opening '{' at position " + i + ": " + text);
					}
					int length = closeIndex - i;
					string escapeCode = text.Substring(i, length);
					KeyboardInput input;
					switch (escapeCode)
					{
						case "ENTER":
							input = new KeyboardInput();
							input.type = new IntPtr(INPUT_KEYBOARD);
							input.wVk = VK_RETURN;
							list.Add(input);

							input = new KeyboardInput(input);
							input.dwFlags |= KEYEVENTF_KEYUP;
							list.Add(input);
							break;
						case "END":
							input = new KeyboardInput();
							input.type = new IntPtr(INPUT_KEYBOARD);
							input.wVk = VK_END;
							list.Add(input);

							input = new KeyboardInput(input);
							input.dwFlags |= KEYEVENTF_KEYUP;
							list.Add(input);

							KeyboardInput reset = new KeyboardInput();
							reset.type = new IntPtr(INPUT_KEYBOARD);
							reset.wVk = VK_SHIFT;
							reset.dwFlags = KEYEVENTF_KEYUP;
							list.Add(reset);
							shiftIsPressed = false;
							break;
						case "HOME":
							input = new KeyboardInput();
							input.type = new IntPtr(INPUT_KEYBOARD);
							input.wVk = VK_HOME;
							list.Add(input);

							input = new KeyboardInput(input);
							input.dwFlags |= KEYEVENTF_KEYUP;
							list.Add(input);

							reset = new KeyboardInput();
							reset.type = new IntPtr(INPUT_KEYBOARD);
							reset.wVk = VK_SHIFT;
							reset.dwFlags = KEYEVENTF_KEYUP;
							list.Add(reset);
							shiftIsPressed = false;
							break;
						case "LEFT":
							input = new KeyboardInput();
							input.type = new IntPtr(INPUT_KEYBOARD);
							input.wVk = VK_LEFT;
							list.Add(input);

							input = new KeyboardInput(input);
							input.dwFlags |= KEYEVENTF_KEYUP;
							list.Add(input);

							reset = new KeyboardInput();
							reset.type = new IntPtr(INPUT_KEYBOARD);
							reset.wVk = VK_SHIFT;
							reset.dwFlags = KEYEVENTF_KEYUP;
							list.Add(reset);
							shiftIsPressed = false;
							break;
						case "UP":
							input = new KeyboardInput();
							input.type = new IntPtr(INPUT_KEYBOARD);
							input.wVk = VK_UP;
							list.Add(input);

							input = new KeyboardInput(input);
							input.dwFlags |= KEYEVENTF_KEYUP;
							list.Add(input);

							reset = new KeyboardInput();
							reset.type = new IntPtr(INPUT_KEYBOARD);
							reset.wVk = VK_SHIFT;
							reset.dwFlags = KEYEVENTF_KEYUP;
							list.Add(reset);
							shiftIsPressed = false;
							break;
						case "RIGHT":
							input = new KeyboardInput();
							input.type = new IntPtr(INPUT_KEYBOARD);
							input.wVk = VK_RIGHT;
							list.Add(input);

							input = new KeyboardInput(input);
							input.dwFlags |= KEYEVENTF_KEYUP;
							list.Add(input);

							reset = new KeyboardInput();
							reset.type = new IntPtr(INPUT_KEYBOARD);
							reset.wVk = VK_SHIFT;
							reset.dwFlags = KEYEVENTF_KEYUP;
							list.Add(reset);
							shiftIsPressed = false;
							break;
						case "DOWN":
							input = new KeyboardInput();
							input.type = new IntPtr(INPUT_KEYBOARD);
							input.wVk = VK_DOWN;
							list.Add(input);

							input = new KeyboardInput(input);
							input.dwFlags |= KEYEVENTF_KEYUP;
							list.Add(input);

							reset = new KeyboardInput();
							reset.type = new IntPtr(INPUT_KEYBOARD);
							reset.wVk = VK_SHIFT;
							reset.dwFlags = KEYEVENTF_KEYUP;
							list.Add(reset);
							shiftIsPressed = false;
							break;
						default:
							throw new ArgumentException(
							"Malformed typing text: unknown escape code [" +
							escapeCode + "]" + i + ": " + text);
					}
					i = closeIndex + 1;
				}
				else
				{
					KeyboardInput input = new KeyboardInput();
					input.type = new IntPtr(INPUT_KEYBOARD);
					input.wVk = VkKeyScan(c);
					list.Add(input);

					input = new KeyboardInput(input);
					input.dwFlags |= KEYEVENTF_KEYUP;
					list.Add(input);

					// Reset shift.
					if (shiftIsPressed)
					{
						KeyboardInput reset = new KeyboardInput();
						reset.type = new IntPtr(INPUT_KEYBOARD);
						reset.wVk = VK_SHIFT;
						reset.dwFlags = KEYEVENTF_KEYUP;
						list.Add(reset);
						shiftIsPressed = false;
					}
					// Reset shift.
					if (controlIsPressed)
					{
						KeyboardInput reset = new KeyboardInput();
						reset.type = new IntPtr(INPUT_KEYBOARD);
						reset.wVk = VK_CONTROL;
						reset.dwFlags = KEYEVENTF_KEYUP;
						list.Add(reset);
						controlIsPressed = false;
					}
					i++;
				}
			}

			KeyboardInput[] inputList = (KeyboardInput[])
				list.ToArray(typeof(KeyboardInput));
			unsafe
			{
				SendKeyboardInput((uint)inputList.Length, inputList, sizeof(KeyboardInput));
			}
		}

		#endregion Keyboard support.

		#endregion Input emulation support.
	}
}
