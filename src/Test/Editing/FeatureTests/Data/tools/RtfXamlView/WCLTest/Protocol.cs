// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace Protocol
{
	using Common;

	// Class Protocol - This object is responsible for writing protocol
	class CProtocol
	{
		// Object variables
		System.Windows.Forms.RichTextBox _richTextBox;

		public CProtocol (System.Windows.Forms.RichTextBox richTextBox)
		{
			this._richTextBox = richTextBox;
		}

		// Clear protocol
		public void Clear ()
		{
			_richTextBox.Clear ();
		}

		// Write new line
		public void Write (string strMsg)
		{
			WriteCore (true, Color.Black, strMsg);
		}

		// Write new error line
		public void WriteWarning (string strMsg)
		{
			WriteCore (true, Color.FromArgb (250, 130, 130), strMsg);
		}

		// New line
		public void NewLine ()
		{
			WriteCore (false, Color.Black, "");
		}

		// Private WriteCore
		private void WriteCore (bool fTime, Color color, string strMsg)
		{
			_richTextBox.Select (int.MaxValue, 0);
			_richTextBox.SelectionColor = color;
			if (fTime)
			{
				_richTextBox.SelectedText = (TimeToHHMMSS (DateTime.Now) + ". " + 
					strMsg + "\n");
			}
			else
			{
				_richTextBox.SelectedText = (strMsg + "\n");
			}

			_richTextBox.Select (int.MaxValue, 0);
			_richTextBox.ScrollToCaret ();
		}


		// Convert time to HH:MM:SS
		private string TimeToHHMMSS (DateTime dt)
		{
			int ich = 0;
			char [] rgch = new char [8];

			if (dt.Hour >= 10)
			{
				rgch [ich++] = (char) (byte) (dt.Hour / 10 + '0');
			};

			rgch [ich++] = (char) (byte) (dt.Hour % 10 + '0');
			rgch [ich++] = ':';
			rgch [ich++] = (char) (byte) (dt.Minute / 10 + '0');
			rgch [ich++] = (char) (byte) (dt.Minute % 10 + '0');
			rgch [ich++] = ':';
			rgch [ich++] = (char) (byte) (dt.Second / 10 + '0');
			rgch [ich++] = (char) (byte) (dt.Second % 10 + '0');

			return new string (rgch, 0, ich);
		}
	}
}