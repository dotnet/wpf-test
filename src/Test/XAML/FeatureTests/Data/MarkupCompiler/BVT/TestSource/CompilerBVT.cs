// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.IO;
using MS.Internal;
using System.Windows;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Avalon.Test.ComponentModel
{
    class CLRExeStub
    {        
        [STAThread]
        static int Main(string[] args)
        {
            bool err = false;
            GlobalLog.LogStatus("Markup Compiler Tests");

            s_dispatcher = new HwndDispatcher();        
            s_context = new Dispatcher();
            s_dispatcher.RegisterContext(s_context);

            using (s_context.Access())
            {
                CLRExeStub test = new CLRExeStub();
                
                err = test.Compile();

                if (!err)
                    GlobalLog.LogEvidence (true, "MARKUP COMPILER BVT SUCCEEDED!");
                else
                    GlobalLog.LogEvidence (false, "MARKUP COMPILER BVT FAILED!");
            }

            return err ? 1 : 0;
        }

        bool Compile()
        {
            bool err = false;
            int startT;
            int endT;

            try
            {
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"MarkupCompiler\Caml"))
                    Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + @"MarkupCompiler\Caml", true);
            }
            catch
            {
            }

            string [] files = { @"page1.xaml"};
            CompilationUnit cu = new CompilationUnit("DrtApp", "C#", "DRT.CLRExeStubs", files);
            cu.ApplicationFile = "App.xaml";
            cu.SourcePath = @"";

            MarkupCompiler mc = new MarkupCompiler();
            mc.Error += new MarkupErrorEventHandler(OnError);
            
            
            mc.TargetPath = @"MarkupCompiler\Baml\";

            try
            {
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"MarkupCompiler\Baml"))
                    Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + @"MarkupCompiler\Baml", true);
            }
            catch
            {
            }

            GlobalLog.LogStatus("Starting Compiling To Baml Tests ...");
            startT = Environment.TickCount;
            mc.Compile(cu);
            endT = Environment.TickCount;

            if (_nErrors == 0)
            {
                GlobalLog.LogStatus("Generating Baml sources passed - Time Elapsed = " + (endT - startT) + "ms");
                err = GenerateAssembly(mc, cu);
            }

            err = err || _nErrors > 0;
            return err;
        }

        bool GenerateAssembly(MarkupCompiler mc, CompilationUnit cu)
        {
            int startT;
            int endT;
            string [] referencedAssemblies = { "System.dll", 
                                               "System.Xml.dll",
                                               "WindowsBase.dll",
                                               "PresentationCore.dll",
                                               "PresentationFramework.dll" };

            CompilerParameters parameters = new CompilerParameters(referencedAssemblies, mc.TargetPath + cu.TargetName + ".exe");
            parameters.IncludeDebugInformation = true;
            parameters.GenerateExecutable = true;
            parameters.CompilerOptions = "/t:winexe";

            string [] codeFiles = Directory.GetFiles(mc.TargetPath, "*.g.cs");

            CodeDomProvider codeProvider = new CSharpCodeProvider();
            startT = Environment.TickCount;
            CompilerResults cr = codeProvider.CreateCompiler ().CompileAssemblyFromFileBatch (parameters, codeFiles);
            endT = Environment.TickCount;

            if (cr.Errors.Count > 0)
            {
                foreach (CompilerError er in cr.Errors)
                {
                    GlobalLog.LogStatus(er.ToString ());
                }
                return true;
            }
            else
            {
                GlobalLog.LogStatus("Generating Baml assemblies passed - Time Elapsed = " + (endT - startT) + "ms");
                return false;
            }
        }

        void OnError(Object sender, MarkupErrorEventArgs e)
        {
            _nErrors++;
            GlobalLog.LogEvidence(false, e.FileName + "(" + e.LineNumber + "," + e.LinePosition + ") : error: " + e.Exception.Message);
        }

        static HwndDispatcher s_dispatcher;
        static Dispatcher      s_context;

        int _nErrors = 0;
    }
}
