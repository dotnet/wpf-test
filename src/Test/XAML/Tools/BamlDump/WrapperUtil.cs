// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
*  Wrapper utilities
*
*
\***************************************************************************/
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Reflection;

using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace DRT
{
        
    public static class WrapperUtil
    {
        static public Assembly AssemblyPF =  LoadAssemblyPF();
        static public BindingFlags MethodBindFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod;
        static public BindingFlags PropertyBindFlags = BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.NonPublic |  BindingFlags.Public |  BindingFlags.GetProperty;
        private const string DOTNETCULTURE = "en-us";


        private static Assembly LoadAssemblyPF()
        {
            Assembly assm = FindAssemblyInDomain("PresentationFramework");
            if (assm == null)
            {
                // A quick way to load the compatible version of "PresentationFramework" 
                Type parserType = typeof(System.Windows.Markup.XamlReader);
                assm = parserType.Assembly;               
            }

            return assm;
        }

        // Loads the Assembly with the specified name.
        [SecurityTreatAsSafe, SecurityCritical]
        internal static Assembly FindAssemblyInDomain(string assemblyName)
        {
            // <SecurityNote>
            //  Critical - as this code does an elevation
            //  TreatAsSafe - the critical data is only used within this function to perform a safe operation. 
            // </SecurityNote>         
            {
                Assembly returnAssembly = null;
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                FileIOPermission permobj = new FileIOPermission(PermissionState.None);
                permobj.AllFiles = FileIOPermissionAccess.PathDiscovery;
                //[CodeAnalysis("SecureAsserts")] //29647
                permobj.Assert(); //BlessedAssert: 

                try
                {
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        if (String.Compare(assemblies[i].FullName, assemblyName, false, CultureInfo.GetCultureInfo(DOTNETCULTURE) ) == 0 ||
                            String.Compare( assemblies[i].GetName().Name, assemblyName, false, CultureInfo.GetCultureInfo(DOTNETCULTURE) ) == 0 )
                        {
                            returnAssembly = assemblies[i];
                            break;
                        }

                    }                
                }
                finally
                {
                    FileIOPermission.RevertAssert();
                }

                return returnAssembly;
            }
        }


        // Test public baml reader interfaces by loading a xaml file, writing out
        // a baml file, then reading the baml file using the BamlReader.
        internal static void ConvertToBaml(string filename)
        {
            // Create the BAML file in parserbaml\drtparser1.baml
            int nErrors = 0;
            bool err = false;
            int startT;
            int endT;
            string[] files = new string[1];
            files[0] = filename;

            Assembly assemblyPBT = FindAssemblyInDomain("PresentationBuildTasks");

            Type typeCompilationUnit;
            Type typeTaskFileService;
            object cu;
            object taskFileService;

            PropertyInfo propInfo_ApplicationFile;
            PropertyInfo propInfo_SourcePath;
            PropertyInfo propInfo_AssemblyName;
            PropertyInfo propInfo_ReferenceAssemblyList;
            PropertyInfo propInfo_TaskFileService;
            BindingFlags propBF = BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty;


            // Create a compilation unit
            typeCompilationUnit = assemblyPBT.GetType("MS.Internal.CompilationUnit");

            cu = Activator.CreateInstance(typeCompilationUnit, new object[4] { "ParserBamlApp", "C#", "DRT", files });

            propInfo_ApplicationFile = typeCompilationUnit.GetProperty("ApplicationFile", propBF);
            propInfo_ApplicationFile.SetValue(cu, @".\" + filename + ".App.xaml", null);

            propInfo_SourcePath = typeCompilationUnit.GetProperty("SourcePath", propBF);
            propInfo_SourcePath.SetValue(cu, @".\", null);

            propInfo_AssemblyName = typeCompilationUnit.GetProperty("AssemblyName", propBF);

            string appFile = propInfo_ApplicationFile.GetValue(cu, null) as string;

            // Create an assembly reference list for PresentationCore and PresentationFramework. This
            // is required so the parse knows where to look for default namespace information
            ArrayList refAssemblies = new ArrayList(4);
            Type typeReferenceAssembly = assemblyPBT.GetType("MS.Internal.ReferenceAssembly");

            string refWinFXAssemblyPath = GetWinFXReferenceAssemblyPath();

            object referenceAssembly = Activator.CreateInstance(typeReferenceAssembly,
                                         BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                         null, new object[2] { refWinFXAssemblyPath + "PresentationFramework.dll", "PresentationFramework" }, CultureInfo.GetCultureInfo("en-us"));
            refAssemblies.Add(referenceAssembly);
            referenceAssembly = Activator.CreateInstance(typeReferenceAssembly,
                                         BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                         null, new object[2] { refWinFXAssemblyPath + "PresentationCore.dll", "PresentationCore" }, CultureInfo.GetCultureInfo("en-us"));
            refAssemblies.Add(referenceAssembly);

            referenceAssembly = Activator.CreateInstance(typeReferenceAssembly,
                                         BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                         null, new object[2] { refWinFXAssemblyPath + "WindowsBase.dll", "WindowsBase" }, CultureInfo.GetCultureInfo("en-us"));
            refAssemblies.Add(referenceAssembly);

            referenceAssembly = Activator.CreateInstance(typeReferenceAssembly,
                                         BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                         null, new object[2] { refWinFXAssemblyPath + "UIAutomationProvider.dll", "UIAutomationProvider" }, CultureInfo.GetCultureInfo("en-us"));
            refAssemblies.Add(referenceAssembly);

            // Erase stub app and target baml file, if there is one.
            try
            {
                if (File.Exists(@".\" + appFile))
                    File.Delete(@".\" + appFile);
            }
            catch
            {
                // We can't always delete an existing application file, but don't worry about
                // it if that happens.
            }

            // Write a stub app file
            FileStream s = new FileStream(appFile, FileMode.Create);
            StreamWriter w = new StreamWriter(s);
            w.WriteLine("<!-- Stub app file -->");
            w.WriteLine("<Application xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            w.WriteLine("    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            w.WriteLine("    StartupUri=\"page1.xaml\">");
            w.WriteLine("</Application>");
            w.Close();

            Type typeMarkupCompiler;
            object mc;
            PropertyInfo propInfo_TargetPath;

            typeMarkupCompiler = assemblyPBT.GetType("MS.Internal.MarkupCompiler");
            mc = Activator.CreateInstance(typeMarkupCompiler);

            propInfo_TargetPath = typeMarkupCompiler.GetProperty("TargetPath", propBF);
            propInfo_TargetPath.SetValue(mc, @".\", null);

            propInfo_ReferenceAssemblyList = typeMarkupCompiler.GetProperty("ReferenceAssemblyList", propBF);
            propInfo_ReferenceAssemblyList.SetValue(mc, refAssemblies, null);

            typeTaskFileService = assemblyPBT.GetType("MS.Internal.TaskFileService");
            taskFileService = Activator.CreateInstance(typeTaskFileService, new object[1] { null });

            propInfo_TaskFileService = typeMarkupCompiler.GetProperty("TaskFileService", propBF);
            propInfo_TaskFileService.SetValue(mc, taskFileService, null);

            // Erase existing baml file, if there is one.
            string basefile = filename.Substring(0, filename.Length - 5);
            string bamlfilename = basefile + ".baml";
            try
            {
                if (File.Exists(@".\" + bamlfilename))
                    File.Delete(@".\" + bamlfilename);
            }
            catch
            {
                // We can't always delete an existing baml file, but don't worry about
                // it if that happens.
            }

            // Compile baml file.
            Console.WriteLine("Converting To Baml");
            startT = Environment.TickCount;

            BindingFlags bfMember = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod;

            typeMarkupCompiler.InvokeMember("Compile",
                                             bfMember,
                                             null,
                                             mc,
                                             new object[] { cu });

            string targetPath = propInfo_TargetPath.GetValue(mc, null) as string;
            string targetName = propInfo_AssemblyName.GetValue(cu, null) as string;

            GenerateAssembly(targetPath, targetName, basefile);

            endT = Environment.TickCount;
            Console.WriteLine("Converted to Baml in " + (endT - startT) + "ms");

            err = err || nErrors > 0;

        }

        // Generate assemblies, just because the compiler still generates a page class at
        // the start of the baml file that subclasses the first element.  
        static void GenerateAssembly(string targetPath, string targetName, string basefile)
        {
            int startT;
            int endT;
            string refWinFXAssemblyPath = GetWinFXReferenceAssemblyPath();

            string[] referencedAssemblies = { "System.dll", 
                                               "System.Xml.dll",
                                               refWinFXAssemblyPath + "WindowsBase.dll",
                                               refWinFXAssemblyPath + "PresentationCore.dll",
                                               refWinFXAssemblyPath + "PresentationFramework.dll" };

            CompilerParameters parameters = new CompilerParameters(referencedAssemblies, targetPath + targetName + ".exe");
            parameters.IncludeDebugInformation = true;
            parameters.GenerateExecutable = true;
            parameters.CompilerOptions = "/t:winexe";

            string[] codeFiles = new string[1];
            codeFiles[0] = basefile + ".xaml.App.g.cs";
            Array arCodeFiles = Array.CreateInstance(typeof(String), codeFiles.Length + 1);
            codeFiles.CopyTo(arCodeFiles, 0);
            ((string[])arCodeFiles)[arCodeFiles.Length - 1] = @"DrtFiles\Baml\drtparserpage.xaml.cs";

            CodeDomProvider codeProvider = new CSharpCodeProvider();
            startT = Environment.TickCount;
            CompilerResults cr = codeProvider.CompileAssemblyFromSource(parameters, (string[])arCodeFiles); 
            endT = Environment.TickCount;

        }

        //
        // All Drts should run with WPF which is appropriately installed through msi or WpfSetup.exe.
        // Drts should NOT assume all the WPF assemblies are copied to the Drt bin directory.
        //
        // After the WPF is installed appropriatelly, MarkupCompiler should reference WPF assemblies from
        // %ProgramFiles%\Reference Assemblies\Microsoft\Framework\v3.0 directory.
        //
        // This private method is to return above path.
        private static string GetWinFXReferenceAssemblyPath()
        {
            string refPath;

            refPath = Environment.GetEnvironmentVariable("ProgramFiles");

            refPath = refPath + @"\Reference Assemblies\Microsoft\Framework\v3.0\";

            return refPath;
        }

    }
        
}











