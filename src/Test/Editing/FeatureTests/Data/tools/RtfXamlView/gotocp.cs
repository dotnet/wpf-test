// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RtfXamlView
{
    /// <summary>
    /// 
    /// </summary>
	public partial class gotocp : Form
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iCP"></param>
		public gotocp(int iCP)
		{
            _iCP = iCP;
			InitializeComponent();
		}
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int m_GetCP()
        {
            return _iCP;
        }

		private void button1_Click(object sender, EventArgs e)
		{
			//here I need to set the current cp to whatever text is in the box.
            //first lets just force the richedit WindowsFormsSection to goto the given cp
            //_richTextBox.
            //determine which view window has the focus.
            _iCP = Int32.Parse(textBox1.Text);
            this.Close();
		}

        private void gotocp_Load(object sender, EventArgs e)
        {
            //get the current cp in the _richEditWindow
            textBox1.Text = _iCP.ToString();
        }

        #region Private Fields
        private int _iCP;
        #endregion
    }
}