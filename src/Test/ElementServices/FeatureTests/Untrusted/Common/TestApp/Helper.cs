// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Avalon.Test.CoreUI.Common
{
    static class Helper
    {

        public static Assembly LoadAssembly(string assemblyString)
        {
            return LoadAssembly(assemblyString, String.Empty);
        }

#pragma warning disable 618
        public static Assembly LoadAssembly(string assemblyString, string hintPath)
        {
            Assembly asm = null;

            if (!String.IsNullOrEmpty(hintPath))
            {
                try
                {
                    asm = Assembly.LoadFrom(Path.Combine(hintPath,assemblyString));
                }
                catch (Exception) { }
            }

            try
            {
                if (asm == null)
                {
                    asm = Assembly.LoadFrom(assemblyString);                    
                }
            }
            catch (Exception) { }

            try
            {
                if (asm == null)
                {
                    asm = Assembly.Load(assemblyString);                    
                }
            }
            catch (Exception) { }

            try
            {
                if (asm == null)
                {
                    asm = Assembly.LoadFile(assemblyString);                    
                }
            }
            catch (Exception) { }

            Assembly assembly = null;
            try
            {
                if (asm == null)
                    assembly = Assembly.LoadWithPartialName(Path.GetFileNameWithoutExtension(assemblyString));
            }
            catch (Exception) { assembly = null; }
            finally
            {
                if (assembly != null)
                    //asm = new Assembly(assembly);
                    asm = assembly;
            }

            if (asm == null)
            {
                throw new InvalidOperationException(assemblyString + " cannot be loaded using any form of Assembly.Load*");
            }

            return asm;
        }
#pragma warning restore 618
    }
}
