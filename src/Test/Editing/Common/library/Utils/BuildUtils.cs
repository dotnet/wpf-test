// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides MSBuild utility methods for test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/BuildUtils.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Reflection;

    using Microsoft.Test.Security.Wrappers;

    using Test.Uis.IO;
    using Test.Uis.Loggers;

    #endregion Namespaces.

    /// <summary>
    /// Provides MSBuild utility methods for test cases.
    /// </summary>
    public static class BuildUtils
    {
        #region Public methods.

        /// <summary>
        /// Runs MSBuild to create an application with the given page content.
        /// </summary>
        /// <param name="pageContent">Content in XAML page to build.</param>
        /// <param name="baseFileName">Base name used for page and project.</param>
        /// <returns>true if the build succeeded; false otherwise.</returns>
        /// <example>The following code shows how to use this method:<code>...
        /// bool buildSucceeded;
        ///
        /// buildSucceeded = Test.Uis.Utils.BuildUtils.RunMSBuildForPageContent(
        ///   pageContent, "WTC.Uis.MyTestCase");
        ///
        /// if (!buildSucceeded) throw new Exception("Unable to build");
        /// ...</code></example>
        public static bool RunMSBuildForPageContent(string pageContent, string baseFileName)
        {
            string applicationXml;          // Content of application XML file.
            string applicationXmlFileName;  // Name of application XML file.
            string pageXmlFileName;         // Name of page XML file.
            string projectXml;              // Content of project XML file.
            string projectXmlFileName;      // Name of project XML file.
            // Build executor helper.
            Microsoft.Test.MSBuildEngine.MSBuildProjExecutor executor;

            if (pageContent == null)
            {
                throw new ArgumentNullException("pageContent");
            }
            if (baseFileName == null)
            {
                throw new ArgumentNullException("baseFileName");
            }

            applicationXmlFileName = baseFileName + ".App.xaml";
            pageXmlFileName = baseFileName + ".Page.xaml";
            projectXmlFileName = baseFileName + ".proj";

            projectXml = @"<Project DefaultTargets='build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>";
            projectXml += "<PropertyGroup>";
            projectXml += "<AssemblyName>" + baseFileName + "</AssemblyName>";
            projectXml += "<OutputType>winexe</OutputType>";
            projectXml += "<Configuration>Release</Configuration>";
            projectXml += "<OutputPath>.\\</OutputPath>";
            projectXml += "</PropertyGroup>";
            projectXml += "<Import Project=\"$(MSBuildBinPath)\\\\Microsoft.CSharp.targets\" />";
            projectXml += "<Import Project=\"$(MSBuildBinPath)\\\\Microsoft.WinFX.targets\" />";
            projectXml += "<ItemGroup>";
            projectXml += "<ApplicationDefinition Include=\"" + applicationXmlFileName + "\" />";
            projectXml += @"<Page Include='" + pageXmlFileName + "' />";
            projectXml += "</ItemGroup>";
            projectXml += "<ItemGroup>";
            projectXml += "<Reference Include=\"System\" />";
            projectXml += "<Reference Include=\"System.Xml\" />";
            projectXml += "<Reference Include=\"System.Data\" />";
            projectXml += "<Reference Include=\"WindowsBase\" />";
            projectXml += "<Reference Include=\"PresentationCore\" />";
            projectXml += "<Reference Include=\"PresentationFramework\" />";
            projectXml += "<Reference Include=\"UIAutomationClient\" />";
            projectXml += "<Reference Include=\"UIAutomationProvider\" />";
            projectXml += "<Reference Include=\"UIAutomationTypes\" />";
            projectXml += "<Reference Include=\"EditingTestLib\"><HintPath>.\\EditingTestLib.dll</HintPath></Reference>";
            projectXml += "</ItemGroup>";
            projectXml += "</Project>";


            applicationXml = @"<Application " +
                @"xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                @"StartupUri='" + pageXmlFileName + "' />";

            TextFileUtils.SaveToFile(projectXml, projectXmlFileName);
            TextFileUtils.SaveToFile(applicationXml, applicationXmlFileName);
            TextFileUtils.SaveToFile(pageContent, pageXmlFileName);

            executor = new Microsoft.Test.MSBuildEngine.MSBuildProjExecutor();
            return executor.Build(projectXmlFileName);
        }

        #endregion Public methods.
    }
}