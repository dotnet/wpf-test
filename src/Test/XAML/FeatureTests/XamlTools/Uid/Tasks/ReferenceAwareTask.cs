// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows; //for LocalizationCategory
using System.ComponentModel; //for EnumConverter
using System.Resources;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using MS.Utility;                   // For SR
using MS.Internal.Tasks;

using System.Xaml;
using Microsoft.Xaml.Tools.XamlDom;
using Microsoft.Xaml.Tools; //for UidTools
// Since we disable PreSharp warnings in this file, we first need to disable warnings about unknown message numbers and unknown pragmas.
#pragma warning disable 1634, 1691

namespace Microsoft.Build.Tasks.Windows.Demos
{
    /// <summary>
    /// An MSBuild task that checks or corrects unique identifiers in
    /// XAML markup.
    /// </summary>
    public abstract class ReferenceAwareTask : Task
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors
        protected ReferenceAwareTask(ResourceManager taskResources)
            : base(taskResources)
        {
        }

        #endregion

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// The method invoked by MSBuild to check or correct Uids.
        /// </summary>
        public override bool Execute()
        {
            TaskHelper.DisplayLogo(Log, SR.Get(SRID.UidManagerTask));

            if (MarkupFiles == null || MarkupFiles.Length == 0)
            {
                Log.LogErrorWithCodeFromResources(SRID.SourceFileNameNeeded);
                return false;
            }



            bool allFilesOk;
            try
            {
                allFilesOk = true;
            }
            catch (Exception e)
            {
                // PreSharp Complaint 6500 - do not handle null-ref or SEH exceptions.
                if (e is NullReferenceException || e is SEHException)
                {
                    throw;
                }
                else
                {
                    string message;
                    string errorId;

                    errorId = Log.ExtractMessageCode(e.Message, out message);

                    if (String.IsNullOrEmpty(errorId))
                    {
                        errorId = UnknownErrorID;
                        message = SR.Get(SRID.UnknownBuildError, message);
                    }

                    Log.LogError(null, errorId, null, null, 0, 0, 0, 0, message, null);

                    allFilesOk = false;
                }
            }
#pragma warning disable 6500
            catch // Non-CLS compliant errors
            {
                Log.LogErrorWithCodeFromResources(SRID.NonClsError);
                allFilesOk = false;
            }
#pragma warning restore 6500

            return allFilesOk;
        }

        #endregion

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties
        ///<summary>
        /// The markup file(s) to be checked or updated.
        ///</summary>
        [Required]
        public ITaskItem[] MarkupFiles
        {
            get { return _markupFiles; }
            set { _markupFiles = value; }
        }

        /// <summary>
        /// The directory for intermedia files
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string IntermediateDirectory
        {
            get { return _backupPath; }
            set
            {
                string sourceDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
                _backupPath = TaskHelper.CreateFullFilePath(value, sourceDir);
            }
        }

        /// <summary>
        /// Assembly References.
        /// </summary>
        /// <value></value>
        public ITaskItem[] References
        {
            get { return _references; }
            set
            {
                _references = value;
            }
        }

        /// <summary>
        /// Main Assembly.
        /// </summary>
        /// <value></value>
        public ITaskItem[] MainAssembly
        {
            get { return _mainAssembly; }
            set
            {
                _mainAssembly = value;
            }
        }

        #endregion



        private string GetTempFileName(string fileName)
        {
            return Path.Combine(_backupPath, Path.ChangeExtension(Path.GetFileName(fileName), "uidtemp"));
        }

        private string GetBackupFileName(string fileName)
        {
            return Path.Combine(_backupPath, Path.ChangeExtension(Path.GetFileName(fileName), "uidbackup"));
        }

        private bool SetupBackupDirectory()
        {
            try
            {
                if (!Directory.Exists(_backupPath))
                {
                    Directory.CreateDirectory(_backupPath);
                }

                return true;
            }
            catch (Exception e)
            {
                // PreSharp Complaint 6500 - do not handle null-ref or SEH exceptions.
                if (e is NullReferenceException || e is SEHException)
                {
                    throw;
                }
                else
                {
                    Log.LogErrorWithCodeFromResources(SRID.IntermediateDirectoryError, _backupPath);
                    return false;
                }
            }
#pragma warning disable 6500
            catch   // Non-cls compliant errors
            {
                Log.LogErrorWithCodeFromResources(SRID.IntermediateDirectoryError, _backupPath);
                return false;
            }
#pragma warning restore 6500
        }





        private List<Assembly> _referenceAssemblies;
        private static Assembly s_presentationFramework;
        private static Assembly s_presentationCore;
        private Assembly _mainSystemAssembly; //strange name to avoid duplication with _mainAssembly
        private List<Assembly> GetReferenceAssemblies()
        {
            if (_referenceAssemblies != null)
            {
                return _referenceAssemblies;
            }
            else
            {
                _referenceAssemblies = new List<Assembly>();
                bool isSilverlight = false;
                foreach (ITaskItem referencePath in References)
                {
                    if (referencePath.ItemSpec.EndsWith("\\System.Windows.dll"))
                    {
                        isSilverlight = true;
                    }
                }
                foreach (ITaskItem referencePath in References)
                {
                    Assembly referenceAssembly;
                    if (referencePath.ItemSpec.Contains("mscorlib"))
                    {
                        referenceAssembly = typeof(object).Assembly; //just use normal mscorlib, not reflectiononlyload.
                    }
                    else
                    {
                        if (isSilverlight)
                        {
                            //SilverlightSchemaContext doesn't work in ROL right now, so workaround
                            //via an Assembly.Load
                            referenceAssembly = Assembly.LoadFrom(referencePath.ItemSpec);
                        }
                        else
                        {
                            referenceAssembly = Assembly.ReflectionOnlyLoadFrom(referencePath.ItemSpec);
                        }
                    }
                    if (referencePath.ItemSpec.Contains("\\PresentationFramework.dll"))
                    {
                        s_presentationFramework = referenceAssembly;
                    }
                    if (referencePath.ItemSpec.Contains("\\PresentationCore.dll"))
                    {
                        s_presentationCore = referenceAssembly;
                    }

                    //
                    Console.WriteLine("Loaded: " + referenceAssembly.FullName + " for " + referencePath.ItemSpec);
                    _referenceAssemblies.Add(referenceAssembly);
                }
                //Load the assembly that is being built by this project
                if (isSilverlight)
                {
                    _mainSystemAssembly = Assembly.LoadFrom(_mainAssembly[0].ItemSpec);
                }
                else
                {
                    _mainSystemAssembly = Assembly.ReflectionOnlyLoadFrom(_mainAssembly[0].ItemSpec);
                }
                //
                Console.WriteLine("Loaded: " + _mainSystemAssembly.FullName + " for " + _mainAssembly[0].ItemSpec);
                _referenceAssemblies.Add(_mainSystemAssembly);
            }
            return _referenceAssemblies;
        }


        //-----------------------------------
        // Private members
        //-----------------------------------
        private ITaskItem[] _markupFiles;     // input Xaml files
        private string _backupPath;      // path to store to backup source Xaml files
        private const string UnknownErrorID = "UM1000";
        private ITaskItem[] _references;
        private ITaskItem[] _mainAssembly;
    }
}
