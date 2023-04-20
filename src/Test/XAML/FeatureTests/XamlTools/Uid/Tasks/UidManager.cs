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
    public sealed class UidManager2 : ReferenceAwareTask
    {
        internal static CultureInfo InvariantEnglishUS = new CultureInfo("en-us", true);
        internal static string DefinitionUid = "Uid";
        internal static string DefinitionNamespaceURI = "http://schemas.microsoft.com/winfx/2006/xaml";

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Create a UidManager object.
        /// </summary>
        public UidManager2()
            : base(SR.ResourceManager)
        {
            _backupPath = Directory.GetCurrentDirectory();
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

            try
            {
                _task = (UidTask)Enum.Parse(typeof(UidTask), _taskAsString);
            }
            catch (ArgumentException)
            {
                Log.LogErrorWithCodeFromResources(SRID.BadUidTask, _taskAsString);
                return false;
            }


            bool allFilesOk;
            try
            {
                allFilesOk = ManageUids();
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
        /// Enum to determine which Uid management task to undertake
        ///</summary>
        private enum UidTask
        {

            ///<summary>
            /// Uid managment task to check validity of Uids
            ///</summary>
            Check = 0,

            ///<summary>
            /// Uid managment task to Update Uids to a valid state
            ///</summary>
            Update = 1,

            ///<summary>
            /// Uid managment task to remove all Uids
            ///</summary>
            Remove = 2,
        }

        ///<summary>
        /// Uid management task required
        ///</summary>
        [Required]
        public string Task
        {
            get { return _taskAsString; }
            set { _taskAsString = value; }
        }

        #endregion

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        private bool ManageUids()
        {
            int countGoodFiles = 0;
            // enumerate through each file
            foreach (ITaskItem inputFile in MarkupFiles)
            {
                Log.LogMessageFromResources(SRID.CheckingUids, inputFile.ItemSpec);
                switch (_task)
                {
                    case UidTask.Check:
                        {
                            UidCollector collector = ParseFile(inputFile.ItemSpec);

                            bool success = VerifyUid(
                                collector,          // uid collector
                                true                // log error
                                );

                            if (success) countGoodFiles++;
                            break;
                        }
                    case UidTask.Update:
                        {
                            UidCollector collector = ParseFile(inputFile.ItemSpec);

                            bool success = VerifyUid(
                                collector,          // uid collector
                                false               // log error
                                );

                            if (!success)
                            {
                                if (SetupBackupDirectory())
                                {
                                    // resolve errors
                                    collector.ResolveUidErrors();

                                    // temp file to write to
                                    string tempFile = GetTempFileName(inputFile.ItemSpec);

                                    // backup file of the source file before it is overwritten.
                                    string backupFile = GetBackupFileName(inputFile.ItemSpec);

                                    using (Stream uidStream = new FileStream(tempFile, FileMode.Create))
                                    {
                                        using (Stream source = File.OpenRead(inputFile.ItemSpec))
                                        {
                                            UidWriter writer = new UidWriter(collector, source, uidStream);
                                            writer.UpdateUidWrite();
                                        }
                                    }

                                    // backup source file by renaming it. Expect to be (close to) atomic op.
                                    RenameFile(inputFile.ItemSpec, backupFile);

                                    // rename the uid output onto the source file. Expect to be (close to) atomic op.
                                    RenameFile(tempFile, inputFile.ItemSpec);

                                    // remove the temp files
                                    RemoveFile(tempFile);
                                    RemoveFile(backupFile);

                                    countGoodFiles++;
                                }
                            }
                            else
                            {
                                // all uids are good. No-op
                                countGoodFiles++;
                            }

                            break;
                        }
                    case UidTask.Remove:
                        {
                            UidCollector collector = ParseFile(inputFile.ItemSpec);

                            bool hasUid = false;
                            for (int i = 0; i < collector.Count; i++)
                            {
                                if (collector[i].Status != UidStatus.Absent)
                                {
                                    hasUid = true;
                                    break;
                                }
                            }

                            if (hasUid)
                            {
                                if (SetupBackupDirectory())
                                {
                                    // temp file to write to
                                    string tempFile = GetTempFileName(inputFile.ItemSpec);

                                    // backup file of the source file before it is overwritten.
                                    string backupFile = GetBackupFileName(inputFile.ItemSpec);

                                    using (Stream uidStream = new FileStream(tempFile, FileMode.Create))
                                    {
                                        using (Stream source = File.OpenRead(inputFile.ItemSpec))
                                        {
                                            UidWriter writer = new UidWriter(collector, source, uidStream);
                                            writer.RemoveUidWrite();
                                        }
                                    }

                                    // rename the source file to the backup file name. Expect to be (close to) atomic op.
                                    RenameFile(inputFile.ItemSpec, backupFile);

                                    // rename the output file over to the source file. Expect to be (close to) atomic op.
                                    RenameFile(tempFile, inputFile.ItemSpec);

                                    // remove the temp files
                                    RemoveFile(tempFile);
                                    RemoveFile(backupFile);

                                    countGoodFiles++;
                                }
                            }
                            else
                            {
                                // There is no Uid in the file. No need to do remove.
                                countGoodFiles++;
                            }

                            break;
                        }
                }
            }

            // spew out the overral log info for the task
            switch (_task)
            {
                case UidTask.Remove:
                    Log.LogMessageFromResources(SRID.FilesRemovedUid, countGoodFiles);
                    break;

                case UidTask.Update:
                    Log.LogMessageFromResources(SRID.FilesUpdatedUid, countGoodFiles);
                    break;

                case UidTask.Check:
                    Log.LogMessageFromResources(SRID.FilesPassedUidCheck, countGoodFiles);

                    if (MarkupFiles.Length > countGoodFiles)
                    {
                        Log.LogErrorWithCodeFromResources(SRID.FilesFailedUidCheck, MarkupFiles.Length - countGoodFiles);
                    }
                    break;
            }

            return MarkupFiles.Length == countGoodFiles;
        }


        private string GetTempFileName(string fileName)
        {
            return Path.Combine(_backupPath, Path.ChangeExtension(Path.GetFileName(fileName), "uidtemp"));
        }

        private string GetBackupFileName(string fileName)
        {
            return Path.Combine(_backupPath, Path.ChangeExtension(Path.GetFileName(fileName), "uidbackup"));
        }

        private void RenameFile(string src, string dest)
        {
            RemoveFile(dest);
            File.Move(src, dest);
        }

        private void RemoveFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
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




        /// <summary>
        /// Verify the Uids in the file
        /// </summary>
        /// <param name="collector">UidCollector containing all Uid instances</param>
        /// <param name="logError">true to log errors while verifying</param>
        /// <returns>true indicates no errors</returns>
        private bool VerifyUid(
            UidCollector collector,
            bool logError
            )
        {
            bool errorFound = false;

            for (int i = 0; i < collector.Count; i++)
            {
                Uid currentUid = collector[i];
                if (currentUid.Status == UidStatus.Absent && currentUid.IsNeeded())
                {
                    // Uid missing
                    if (logError)
                    {
                        Log.LogErrorWithCodeFromResources(
                             null,
                             collector.FileName,
                             currentUid.LineNumber,
                             currentUid.LinePosition,
                             0, 0,
                             SRID.UidMissing, currentUid.ElementName
                         );
                    }

                    errorFound = true;
                }
                else if (currentUid.Status == UidStatus.Duplicate)
                {
                    // Uid duplicates
                    if (logError)
                    {
                        Log.LogErrorWithCodeFromResources(
                              null,
                              collector.FileName,
                              currentUid.LineNumber,
                              currentUid.LinePosition,
                              0, 0,
                              SRID.MultipleUidUse, currentUid.Value, currentUid.ElementName
                         );

                    }

                    errorFound = true;
                }

            }

            return !errorFound;
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
                    _mainSystemAssembly = Assembly.LoadFrom(MainAssembly[0].ItemSpec);
                }
                else
                {
                    _mainSystemAssembly = Assembly.ReflectionOnlyLoadFrom(MainAssembly[0].ItemSpec);
                }
                //
                Console.WriteLine("Loaded: " + _mainSystemAssembly.FullName + " for " + MainAssembly[0].ItemSpec);
                _referenceAssemblies.Add(_mainSystemAssembly);
            }
            return _referenceAssemblies;
        }
        /// <summary>
        /// Parse the input file and get all the information of Uids
        /// </summary>
        /// <param name="fileName">input file</param>
        /// <returns>UidCollector containing all the information for the Uids in the file</returns>
        private UidCollector ParseFile(string fileName)
        {
            UidCollector collector = new UidCollector(fileName);
            List<Assembly> refAssemblies = GetReferenceAssemblies();
            XamlSchemaContext schemaContext;
            if (s_presentationFramework != null)
            {
                schemaContext = new XamlSchemaContext();
            }
            else
            {
                Assembly slXamlTools = Assembly.LoadFrom("Microsoft.Xaml.Tools.Silverlight.dll");
                Type slXscType = slXamlTools.GetType("Microsoft.Xaml.Tools.Silverlight.SilverlightSchemaContext");

                schemaContext = (XamlSchemaContext)Activator.CreateInstance(slXscType);
            }

            XamlDomObject rootDomObject = null;
            try
            {
                using (XmlReader xr = XmlReader.Create(fileName))
                {
                    XamlXmlReaderSettings xxrs = new XamlXmlReaderSettings();
                    xxrs.ProvideLineInfo = true;
                    xxrs.LocalAssembly = _mainSystemAssembly;
                    Console.WriteLine("Loaded: " + _mainSystemAssembly.FullName);
                    XamlXmlReader xxr = new XamlXmlReader(xr, schemaContext, xxrs);
                    rootDomObject = (XamlDomObject)XamlDomServices.Load(xxr);
                }
            }
            catch (Exception e)
            {
                //TDOO: make this message appropriate...should say XML parse error?
                Log.LogErrorWithCodeFromResources(
                             null,
                             fileName,
                             0, //line#
                             0, //lineposition.
                             0, 0,
                             e.Message //should use SRID.Foo?, "" //elementName
                         );
                return collector; //
            }
            IEnumerable<XamlDomObject> domObjects = rootDomObject.DescendantsAndSelf();

            collector.RootElementLineNumber = rootDomObject.StartLineNumber;
            collector.RootElementLinePosition = rootDomObject.StartLinePosition;

            foreach (XamlDomObject domObject in domObjects)
            {
                //
                if (domObject.IsGetObject) //only interested in objects that were specified in markup
                {
                    continue;
                }

                string uidValue = null;
                XamlDomMember uidMember = domObject.GetMemberNode(XamlLanguage.Uid);
                if (uidMember != null)
                {
                    uidValue = ((uidMember.Item) as XamlDomValue).Value as string;
                }
                IEnumerable<NamespaceDeclaration> nsDecls = domObject.GetNamespacePrefixes();
                int prefixLength = 0;
                foreach (NamespaceDeclaration nsDecl in nsDecls)
                {
                    if (nsDecl.Namespace == domObject.Type.PreferredXamlNamespace)
                    {
                        prefixLength = String.IsNullOrEmpty(nsDecl.Prefix) ? 0 : nsDecl.Prefix.Length + 1;
                    }
                }
                Uid currentUid = new Uid(domObject.StartLineNumber,
                                  prefixLength + 
                                  domObject.StartLinePosition + domObject.Type.Name.Length,  //
                                  domObject.Type.Name,
                                  SpaceInsertion.BeforeUid  // insert space before the Uid
                                  );

                currentUid.DomObject = domObject;

                //UID was found
                if (uidValue != null)
                {
                    currentUid.Value = uidValue;
                    currentUid.LineNumber = uidMember.StartLineNumber;
                    currentUid.LinePosition = uidMember.StartLinePosition;
                }

                else
                {
                    //try to find Name or x:Name (will be the right value, or null, if there is no name)
                    //
                    string runtimeName = null;
                    XamlDomMember nameDomMember = domObject.GetMemberNode(XamlLanguage.Name);

                    Console.WriteLine("Name for XT "+ domObject.Type.Name + " is " + (nameDomMember == null ? "NOTFOUND" : (nameDomMember.Member != null ? nameDomMember.Member.Name : "NULL MEMBER")));
                    Console.WriteLine(domObject.Type.Name + "'s RuntimeName is: " + domObject.Type.GetAliasedProperty(XamlLanguage.Name));

                    if (nameDomMember != null)
                    {
                        XamlDomValue nameValue = nameDomMember.Item as XamlDomValue;
                        if (nameValue != null)
                        {
                            runtimeName = nameValue.Value as string;
                        }
                    }

                    currentUid.FrameworkElementName = runtimeName;

                    //a collector needs to know all prefixes defined on each element, so in case it needs to add a Uid, it knows what prefixes have been used.
                    foreach (XamlDomNamespace localDomNamespace in domObject.Namespaces)
                    {
                        collector.AddNamespacePrefix(localDomNamespace.NamespaceDeclaration.Prefix);
                    }

                    //a collector needs to know the xaml xmlns prefix for each element, so in case it needs to add a Uid, it knows what prefix to use.
                    foreach (NamespaceDeclaration namespaceDeclaration in domObject.GetNamespacePrefixes())
                    {
                        if (namespaceDeclaration.Namespace == XamlLanguage.Xaml2006Namespace)
                        {
                            currentUid.NamespacePrefix = namespaceDeclaration.Prefix;
                            break;
                        }
                    }
                }
                collector.AddUid(currentUid);
            }

            return collector;

        }

        static XamlMember s_localizationAttributesXamlMember;
        internal static bool InstanceShouldHaveUid(XamlDomObject objectNode)
        {
            if (s_presentationFramework == null) //doing a SL build
            {
                //
                return true;
            }

            string locAttributes = null;

            if (s_localizationAttributesXamlMember == null)
            {
                XamlType localizationXamlType = objectNode.SchemaContext.GetXamlType(s_presentationFramework.GetType("System.Windows.Localization"));
                s_localizationAttributesXamlMember = localizationXamlType.GetAttachableMember("Attributes");
            }

            XamlDomMember locAttrDomMember = objectNode.GetMemberNode(s_localizationAttributesXamlMember);
            if (locAttrDomMember != null)
            {
                locAttributes = (locAttrDomMember.Items[0] as XamlDomValue).Value as string;
            }

            bool? addUidOverride = null;

            //$Content(Button Readable Modifiable) FontFamily(Font Readable Unmodifiable)
            if (locAttributes != null)
            {
                List<LocalizationAttribute> locAttrs = LocalizationAttribute.Parse(locAttributes);

                foreach (LocalizationAttribute locAttr in locAttrs)
                {
                    if (
                        (locAttr.Modifiability.HasValue && locAttr.Modifiability.Value) ||
                        (locAttr.Readability.HasValue && locAttr.Readability.Value)
                        )
                    {
                        addUidOverride = true;
                    }
                    if (locAttr.Category == LocalizationCategory.NeverLocalize
                        || locAttr.Category == LocalizationCategory.Inherit
                        || locAttr.Category == LocalizationCategory.Ignore
                        )
                    {
                        addUidOverride = false;
                    }
                    else
                    {
                        addUidOverride = true;
                    }
                }
            }
            bool hasOverride = addUidOverride.HasValue;
            bool doOverride = (hasOverride ? addUidOverride.Value : false); //leave as false, if !hasOverride
            bool typeNeeds = TypeNeedsUid(objectNode.Type);

            bool result =
                (typeNeeds && (!hasOverride || doOverride))
                || (hasOverride && doOverride);
            Console.WriteLine(objectNode.Type.Name + ": " + result);
            return result;
        }

        //First attempt at full logic for which types need Uid...likely by reflecting on the type for all members to see if any properties are localizable
        //       or not...perhaps including the .xml config file not just attributes on the assembly.
        public static bool TypeNeedsUid(XamlType xamlType)
        {
            Type type = xamlType.UnderlyingType;
            bool needsUid = false;

            while (type != null)
            {
                IList<CustomAttributeData> attrs = type.GetCustomAttributesData();

                //
                foreach (CustomAttributeData attrData in attrs)
                {
                    if (attrData.Constructor.DeclaringType == s_presentationCore.GetType("System.Windows.LocalizabilityAttribute"))
                    {
                        IList<CustomAttributeTypedArgument> ctorArgs = attrData.ConstructorArguments;
                        if (((LocalizationCategory)ctorArgs[0].Value) != LocalizationCategory.None
                            && ((LocalizationCategory)ctorArgs[0].Value) != LocalizationCategory.NeverLocalize
                            )
                        {
                            needsUid = true;
                            return needsUid;
                        }
                    }
                }
                type = type.BaseType;
            }
            return needsUid;
        }
        //-----------------------------------
        // Private members
        //-----------------------------------
        private UidTask _task;            // task
        private string _taskAsString;    // task string
        private string _backupPath;      // path to store to backup source Xaml files
        private const string UnknownErrorID = "UM1000";
    }

    // represent all the information about a Uid
    // The uid may be valid, absent, or duplicate
    internal sealed class Uid
    {
        internal Uid(
            int lineNumber,
            int linePosition,
            string elementName,
            SpaceInsertion spaceInsertion
            )
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
            ElementName = elementName;
            Value = null;
            NamespacePrefix = null;
            FrameworkElementName = null;
            Status = UidStatus.Valid;
            Space = spaceInsertion;

        }

        //if type is localizable, and has been overridden locally, return true
        public bool IsNeeded()
        {
            if (!_IsNeeded.HasValue)
            {
                _IsNeeded = UidManager2.InstanceShouldHaveUid(DomObject);
            }
            return _IsNeeded.Value;
        }

        internal int LineNumber;         // Referenced line number of the original document
        internal int LinePosition;       // Reference line position of the original document
        internal string ElementName;        // name of the element that needs this uid
        internal SpaceInsertion Space;       // Insert a space before/after the Uid

        internal string Value;              // value of the uid
        internal string NamespacePrefix;    // namespace prefix for the uid
        internal string FrameworkElementName; // the FrameworkElement.Name of element
        internal UidStatus Status;             // the status of the this uid

        internal XamlDomObject DomObject;   // DomObject of the element

        private bool? _IsNeeded;
    }


    internal enum UidStatus : byte
    {
        Valid = 0,    // uid is valid
        Absent = 1,    // uid is absent
        Duplicate = 2,    // uid is duplicated
    }

    internal enum SpaceInsertion : byte
    {
        BeforeUid,          // Insert a space before the Uid
        AfterUid            // Insert a space after the Uid
    }

    // writing to a file, removing or updating uid
    internal sealed class UidWriter
    {
        internal UidWriter(UidCollector collector, Stream source, Stream target)
        {
            _collector = collector;
            _sourceReader = new StreamReader(source);

            UTF8Encoding encoding = new UTF8Encoding(true);
            _targetWriter = new StreamWriter(target, encoding);
            _lineBuffer = new LineBuffer(_sourceReader.ReadLine());
        }

        // write to target stream and update uids
        internal bool UpdateUidWrite()
        {
            try
            {
                // we need to add a new namespace
                if (_collector.NamespaceAddedForMissingUid != null)
                {
                    // write to the beginning of the root element
                    WriteTillSourcePosition(
                        _collector.RootElementLineNumber,
                        _collector.RootElementLinePosition
                        );

                    WriteElementTag();
                    WriteSpace();
                    WriteNewNamespace();
                }

                for (int i = 0; i < _collector.Count; i++)
                {
                    Uid currentUid = _collector[i];
                    WriteTillSourcePosition(currentUid.LineNumber, currentUid.LinePosition);

                    if (currentUid.Status == UidStatus.Absent && currentUid.IsNeeded())
                    {

                        if (currentUid.Space == SpaceInsertion.BeforeUid)
                        {
                            WriteSpace();
                        }

                        WriteNewUid(currentUid);

                        if (currentUid.Space == SpaceInsertion.AfterUid)
                        {
                            WriteSpace();
                        }
                    }
                    else if (currentUid.Status == UidStatus.Duplicate)
                    {
                        ProcessAttributeStart(WriterAction.Write);
                        SkipSourceAttributeValue();
                        WriteNewAttributeValue(currentUid.Value);
                    }
                }
                WriteTillEof();
                return true;
            }
            catch (Exception e)
            {
                // PreSharp Complaint 6500 - do not handle null-ref or SEH exceptions.
                if (e is NullReferenceException || e is SEHException)
                {
                    throw;
                }

                return false;
            }
#pragma warning disable 6500
            catch
            {
                return false;
            }
#pragma warning restore 6500
        }

        // writing to the target stream removing uids
        internal bool RemoveUidWrite()
        {
            try
            {
                for (int i = 0; i < _collector.Count; i++)
                {
                    Uid currentUid = _collector[i];

                    // skipping valid and duplicate uids.
                    if (currentUid.Status == UidStatus.Duplicate
                      || currentUid.Status == UidStatus.Valid)
                    {
                        // write till the space in front of the Uid
                        WriteTillSourcePosition(currentUid.LineNumber, currentUid.LinePosition - 1);

                        // skip the uid
                        ProcessAttributeStart(WriterAction.Skip);
                        SkipSourceAttributeValue();
                    }
                }

                WriteTillEof();
                return true;
            }
            catch (Exception e)
            {
                // PreSharp Complaint 6500 - do not handle null-ref or SEH exceptions.
                if (e is NullReferenceException || e is SEHException)
                {
                    throw;
                }

                return false;
            }
#pragma warning disable 6500
            catch
            {
                return false;
            }
#pragma warning restore 6500
        }

        private void WriteTillSourcePosition(int lineNumber, int linePosition)
        {
            // write to the correct line
            while (_currentLineNumber < lineNumber)
            {
                // write out the line buffer
                _targetWriter.WriteLine(_lineBuffer.ReadToEnd());
                _currentLineNumber++;
                _currentLinePosition = 1;

                // read one more line
                _lineBuffer.SetLine(_sourceReader.ReadLine());
            }

            // write to the correct line position
            while (_currentLinePosition < linePosition)
            {
                _targetWriter.Write(_lineBuffer.Read());
                _currentLinePosition++;
            }

        }

        private void WriteElementTag()
        {
            if (_lineBuffer.EOL)
            {
                // advance to the non-empty line
                AdvanceTillNextNonEmptyLine(WriterAction.Write);
            }

            char ch = _lineBuffer.Peek();

            // stop when we see space, "/" or ">". That is the end of the
            // element name
            while (!Char.IsWhiteSpace(ch)
                   && ch != '/'
                   && ch != '>'
                  )
            {
                _targetWriter.Write(ch);

                _currentLinePosition++;
                _lineBuffer.Read();
                if (_lineBuffer.EOL)
                {
                    AdvanceTillNextNonEmptyLine(WriterAction.Write);
                }

                ch = _lineBuffer.Peek();
            }
        }

        private void WriteNewUid(Uid uid)
        {
            // construct the attribute name, e.g. x:Uid
            // "x" will be the resolved namespace prefix for the definition namespace
            string attributeName =
                (uid.NamespacePrefix == null) ?
                 _collector.NamespaceAddedForMissingUid + ":" + UidManager2.DefinitionUid
               : uid.NamespacePrefix + ":" + UidManager2.DefinitionUid;

            // escape all the Xml entities in the value
            string attributeValue = s_escapedXmlEntities.Replace(
                uid.Value,
                s_escapeMatchEvaluator
                );

            string clause = string.Format(
                UidManager2.InvariantEnglishUS,
                "{0}=\"{1}\"",
                attributeName,
                attributeValue
                );

            _targetWriter.Write(clause);
        }

        private void WriteNewNamespace()
        {
            string clause = string.Format(
                UidManager2.InvariantEnglishUS,
                "xmlns:{0}=\"{1}\"",
                _collector.NamespaceAddedForMissingUid,
                XamlLanguage.Xaml2006Namespace
                );

            _targetWriter.Write(clause);
        }

        private void WriteNewAttributeValue(string value)
        {
            string attributeValue = s_escapedXmlEntities.Replace(
                value,
                s_escapeMatchEvaluator
                );

            _targetWriter.Write(
                string.Format(
                    UidManager2.InvariantEnglishUS,
                    "\"{0}\"",
                    value
                    )
                );
        }

        private void WriteSpace()
        {
            // insert a space
            _targetWriter.Write(" ");
        }

        private void WriteTillEof()
        {
            _targetWriter.WriteLine(_lineBuffer.ReadToEnd());
            _targetWriter.Write(_sourceReader.ReadToEnd());
            _targetWriter.Flush();
        }

        private void SkipSourceAttributeValue()
        {
            char ch = (char)0;

            // read to the start quote of the attribute value
            while (ch != '\"' && ch != '\'')
            {
                if (_lineBuffer.EOL)
                {
                    AdvanceTillNextNonEmptyLine(WriterAction.Skip);
                }

                ch = _lineBuffer.Read();
                _currentLinePosition++;
            }

            char attributeValueStart = ch;
            // read to the end quote of the attribute value
            ch = (char)0;
            while (ch != attributeValueStart)
            {
                if (_lineBuffer.EOL)
                {
                    AdvanceTillNextNonEmptyLine(WriterAction.Skip);
                }

                ch = _lineBuffer.Read();
                _currentLinePosition++;
            }
        }

        private void AdvanceTillNextNonEmptyLine(WriterAction action)
        {
            do
            {
                if (action == WriterAction.Write)
                {
                    _targetWriter.WriteLine();
                }

                _lineBuffer.SetLine(_sourceReader.ReadLine());
                _currentLineNumber++;
                _currentLinePosition = 1;

            } while (_lineBuffer.EOL);
        }


        private void ProcessAttributeStart(WriterAction action)
        {
            if (_lineBuffer.EOL)
            {
                AdvanceTillNextNonEmptyLine(action);
            }

            char ch;
            do
            {
                ch = _lineBuffer.Read();

                if (action == WriterAction.Write)
                {
                    _targetWriter.Write(ch);
                }

                _currentLinePosition++;

                if (_lineBuffer.EOL)
                {
                    AdvanceTillNextNonEmptyLine(action);
                }

            } while (ch != '=');
        }

        //
        // source position in a file starts from (1,1)
        //

        private int _currentLineNumber = 1;   // current line number in the source stream
        private int _currentLinePosition = 1;   // current line position in the source stream
        private LineBuffer _lineBuffer;                // buffer for one line's content

        private UidCollector _collector;
        private StreamReader _sourceReader;
        private StreamWriter _targetWriter;

        //
        // buffer for the content of a line
        // The UidWriter always reads one line at a time from the source
        // and store the line in this buffer.
        //
        private sealed class LineBuffer
        {
            private int _index;
            private string _content;

            public LineBuffer(string line)
            {
                SetLine(line);
            }

            public void SetLine(string line)
            {
                _content = (line == null) ? string.Empty : line;
                _index = 0;
            }

            public bool EOL
            {
                get { return (_index == _content.Length); }
            }

            public char Read()
            {
                if (!EOL)
                {
                    return _content[_index++];
                }

                throw new InvalidOperationException();
            }

            public char Peek()
            {
                if (!EOL)
                {
                    return _content[_index];
                }

                throw new InvalidOperationException();
            }

            public string ReadToEnd()
            {
                if (!EOL)
                {
                    int temp = _index;
                    _index = _content.Length;

                    return _content.Substring(temp);
                }

                return string.Empty;
            }
        }

        private enum WriterAction
        {
            Write = 0,  // write the content
            Skip = 1,  // skip the content
        }

        private static Regex s_escapedXmlEntities = new Regex("(<|>|\"|'|&)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static MatchEvaluator s_escapeMatchEvaluator = new MatchEvaluator(EscapeMatch);

        /// <summary>
        /// the delegate to escape the matched pattern
        /// </summary>
        private static string EscapeMatch(Match match)
        {
            switch (match.Value)
            {
                case "<":
                    return "&lt;";
                case ">":
                    return "&gt;";
                case "&":
                    return "&amp;";
                case "\"":
                    return "&quot;";
                case "'":
                    return "&apos;";
                default:
                    return match.Value;
            }
        }
    }

    public class LocalizationAttribute
    {
        public string PropertyName { get; set; }
        public LocalizationCategory? Category { get; set; }
        public bool? Readability { get; set; }
        public bool? Modifiability { get; set; }

        internal static List<LocalizationAttribute> Parse(string locAttributesString)
        {
            List<LocalizationAttribute> locAttributes = null;

            if (locAttributesString == null)
                return null;

            int startChar = 0;
            int leftParen = -1;
            do
            {
                leftParen = locAttributesString.IndexOf('(', startChar);
                if (leftParen < 0)
                    break;
                LocalizationAttribute locAttr = new LocalizationAttribute();
                locAttr.PropertyName = locAttributesString.Substring(startChar, leftParen - startChar).Trim();
                int rightParen = locAttributesString.IndexOf(')', leftParen);
                string[] words = locAttributesString.Substring(leftParen + 1, rightParen - leftParen - 1).Split(new char[] { ' ' });

                foreach (string word in words)
                {
                    switch (word.ToUpper())
                    {
                        case "MODIFIABLE":
                            locAttr.Modifiability = true;
                            break;
                        case "UNMODIFIABLE":
                            locAttr.Modifiability = false;
                            break;
                        case "READABLE":
                            locAttr.Readability = true;
                            break;
                        case "UNREADABLE":
                            locAttr.Readability = false;
                            break;
                        default:
                            if (word.Trim() != String.Empty)
                            {
                                EnumConverter enumConverter = new EnumConverter(typeof(LocalizationCategory));
                                LocalizationCategory locCategory = (LocalizationCategory)enumConverter.ConvertFromString(word);
                                locAttr.Category = locCategory;
                            }
                            break;
                    }
                }

                if (locAttributes == null)
                {
                    locAttributes = new List<LocalizationAttribute>();
                }
                locAttributes.Add(locAttr);

                startChar = rightParen + 1;
            } while (leftParen > -1);
            return locAttributes;
        }
    }

}
