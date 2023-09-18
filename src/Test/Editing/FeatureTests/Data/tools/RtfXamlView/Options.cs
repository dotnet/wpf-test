// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;

namespace RtfXamlView
{
   /// <summary>
   /// 
   /// </summary>
   public partial class Options : Form
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="ini"></param>
      public Options(ref CInifile ini)
      {
         _ini = ini;
         InitializeComponent();
         _szBVT = _ini._szBVTPath;
         _szLogFile = _ini._szLogFilePath;
         _fSubDir = _ini._fBVTSubDir;
         _fXCVT = _ini._fUseXCVT;
         _fLogPri = _ini._fLogPri;
      }

      private void btnBVTBrowse_Click(object sender, EventArgs e)
      {
         tbBVTFolder.Text = Common.Common.GetFolder(_szBVT);
         this.Update();
      }

      private void btnLogBrowse_Click(object sender, EventArgs e)
      {
         tbLogFilePath.Text = Common.Common.GetFolder(_szLogFile);
         this.Update();
      }


      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         _ini._szBVTPath = _szBVT;
         _ini._szLogFilePath = _szLogFile;
         _ini._fUseXCVT = _fXCVT;
         _ini._fBVTSubDir = _fSubDir;
         _ini._fLogPri = _fLogPri;
         this.Close();
      }


      private void Options_Load(object sender, EventArgs e)
      {
         tbBVTFolder.Text = _szBVT;
         tbLogFilePath.Text = _szLogFile;
         chkFXCVT.Checked = _fXCVT;
         chkSubFolders.Checked = _fSubDir;
         chkPri0.Checked = _fLogPri;
      }

      private void chkSubFolders_CheckedChanged(object sender, EventArgs e)
      {
         _fSubDir = chkSubFolders.Checked;
      }

      private void chkFXCVT_CheckedChanged(object sender, EventArgs e)
      {
         _fXCVT = chkFXCVT.Checked;
      }

      private void tbBVTFolder_TextChanged(object sender, EventArgs e)
      {
         _szBVT = tbBVTFolder.Text;
      }

      private void tbLogFilePath_TextChanged(object sender, EventArgs e)
      {
         _szLogFile = tbLogFilePath.Text;
      }
      private void chkPri0_CheckedChanged(object sender, EventArgs e)
      {
         _fLogPri = chkPri0.Checked;
      }

      #region class fields
      /// <summary>
      /// 
      /// </summary>
      public CInifile _ini;

      //temp variables in case user cancels
      string _szBVT;
      bool _fSubDir;
      string _szLogFile;
      bool _fXCVT;
      bool _fLogPri;
      #endregion

   }

   //add an option to write our basic ini file settings
   /// <summary>
   /// 
   /// </summary>
   public class CInifile
   {
      StreamReader _stream;
      string _sfnInifile;
      int _nLine;

      //our apps stored settings
      /// <summary></summary>
      public bool _fBVTSubDir;//BVT should process sub folders as well
      /// <summary></summary>
      public string _szBVTPath;
      /// <summary></summary>
      public string _szLogFilePath;
      /// <summary></summary>
      public bool _fUseXCVT;
      /// <summary></summary>
      public bool _fLogPri;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="_sfnInifile"></param>
      public CInifile(string _sfnInifile)
      {
         _sfnInifile = _sfnInifile;
         _nLine = 0;

         if (File.Exists(_sfnInifile))
         {
            _stream = File.OpenText(_sfnInifile);
            _szBVTPath = GetNext("BVTPath");
            _fBVTSubDir = GetNext("IncludeSubDir") == "1" ? true : false;
            _szLogFilePath = GetNext("LogFilePath");
            _fUseXCVT = GetNext("UseXCVT") == "1" ? true : false;
            _fLogPri = GetNext("LogPri") == "1" ? true : false;
            _stream.Close();
         }
         else//create a basic ini file
         {
            StreamWriter sw = new StreamWriter(_sfnInifile);
            sw.WriteLine(@"BVTPath=c:\");
            _szBVTPath = "c:\\";
            sw.WriteLine("IncludeSubDir=1");
            _fBVTSubDir = true;
            sw.WriteLine(@"LogFilePath=c:\");
            _szLogFilePath = "c:\\";
            sw.WriteLine(@"UseXCVT=1");
            sw.WriteLine(@"LogPri=1");
            _fUseXCVT = true;            
            sw.Close();
         }
      }

      // Finalizer: releases system resources
      /// <summary>
      /// 
      /// </summary>
      public void Close()
      {
         _stream.Close();
      }

      /// <summary>
      ///   Read next statement, return false if end of stream
      /// </summary>
      /// <param name="strKey"></param>
      /// <param name="strValue"></param>
      /// <returns></returns>
      public bool FGetNext(string strKey, out string strValue)
      {
         string s1, s2;
         bool fFoundNextStatement;

         s1 = null;
         s2 = null;
         fFoundNextStatement = false;

         while (_stream.Peek() != -1 && !fFoundNextStatement)
         {
            string help = _stream.ReadLine().Trim();

            _nLine++; /* Increase line counter for error messages */

            if (help != "" && help[0] != ';')
            {
               /* Not empty line and not a comment line */
               if (!Common.Common.FSplitString(help, '=', out s1, out s2))
               {
                  RaiseError("Statement must include '='");
               }

               s1 = s1.Trim();
               s2 = s2.Trim();
               fFoundNextStatement = true;
            };
         };

         if (fFoundNextStatement)
         {
            if (Common.Common.CompareStr(s1, strKey) == 0)
            {
               /* Found correct key */
               strValue = s2;
               return true;
            }
            else
            {
               RaiseError("Incorrect key '" + s1 + "', expected: '" + strKey + "'");
               strValue = ""; // will not be executed
               return false;
            }
         }
         else
         {
            /* End of stream */
            strValue = "";
            return false;
         };
      }

      void RaiseError(string strMessage)
      {
         /*W11Messages.RaiseError*/
         System.Windows.MessageBox.Show("Error in configuation file " + _sfnInifile + ", line " + _nLine.ToString() + "\n\n" +
             strMessage);
      }

     /// <summary>
      ///   Read next statement, fire exception is not equal to Key
      /// </summary>
      /// <param name="strKey"></param>
      /// <returns></returns>
      public string GetNext(string strKey)
      {
         string strValue;

         if (!FGetNext(strKey, out strValue))
         {
            RaiseError("Can not find key '" + strKey + "'");
            return "";
         }
         else return strValue;
      }

      /// <summary>
      /// 
      /// </summary>
      public void SaveSettings()
      {
         StreamWriter sw = new StreamWriter(_sfnInifile);
         sw.WriteLine("BVTPath=" + _szBVTPath);
         sw.WriteLine("IncludeSubDir=" + (_fBVTSubDir ? "1" : "0"));
         sw.WriteLine("LogFilePath=" + _szLogFilePath);
         sw.WriteLine("UseXCVT=" + (_fUseXCVT ? "1" : "0"));
         sw.WriteLine("LogPri=" + (_fLogPri ? "1" : "0"));
         sw.Close();
      }
   }
}