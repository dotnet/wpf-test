// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Office = Microsoft.Office.InteropLateBound.Office;

namespace CoreInteropLateBound
{	
	public class ApplicationCommon : CoreLateBound
	{	
		protected object m_OfficeApp;
		
		protected enum EOfficeAppName
		{
			Word,
			PowerPoint,
			Excel
		}

		private string GetOfficeProgId(EOfficeAppName appName)
		{
			return appName.ToString() + ".Application";
		}		

		protected void StartOfficeApp(EOfficeAppName appName)
		{
			m_OfficeApp = CreateObject(GetOfficeProgId(appName));			
		}	
	
		public Office.MsoAutomationSecurity AutomationSecurity
		{
			get
			{					
				return (Office.MsoAutomationSecurity)Enum.Parse(typeof(Office.MsoAutomationSecurity), GetProperty(m_OfficeApp, "AutomationSecurity").ToString());
			}

			set
			{
				SetProperty(m_OfficeApp, "AutomationSecurity", new object[] {value});
			}
		}	
	
		public object GetUnderlyingObject()
		{
			return m_OfficeApp;
		}

		public bool Visible
		{
			set
			{					
				SetProperty(m_OfficeApp, "Visible", value);
			}
			get
			{
				return Convert.ToBoolean(GetProperty(m_OfficeApp, "Visible"));
			}	
		}		
		
		public Office.MsoFeatureInstall FeatureInstall
		{
			get
			{
				return (Office.MsoFeatureInstall)Enum.Parse(typeof(Office.MsoFeatureInstall), GetProperty(m_OfficeApp, "FeatureInstall").ToString());
			}

			set
			{
				SetProperty(m_OfficeApp, "FeatureInstall", new object[] {value});
			}
		}
	}
}