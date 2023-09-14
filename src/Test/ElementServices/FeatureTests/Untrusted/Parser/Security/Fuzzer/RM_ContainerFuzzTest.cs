// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: 
 *      Represents a Rights Management container fuzz test. Contains overrides for generating 
 *      RM container files.
 * 
 * 
 * ToDo: New errors are exposed when fuzzing encryptedPackageEnvelopes.  Must analyze and catalog.
 * 
 
  
 * Revision:         $Revision: 1 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon/windowstest/client/wcptests/Core/Framework/BVT/parser/Security/Fuzzer/ContainerFuzzTest.cs $
********************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Net;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Test.Container;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser;

namespace Avalon.Test.CoreUI.Parser.Security
{
    /// <summary>
    /// </summary>
    public class RM_ContainerFuzzTest : ContainerFuzzTest
    {
        /// <summary> 
        /// </summary>
        public RM_ContainerFuzzTest()
            : base()
        {
            InitRM();
        }

        /// <summary>
        /// </summary>
        public RM_ContainerFuzzTest(XmlElement xmlElement)
            : base(xmlElement)
        {
            XmlAttributeCollection attribs = xmlElement.Attributes;
            if (attribs["EnableEncryptedPackageOpen"] != null)
                _enableEncryptedPackageOpen = Convert.ToBoolean(attribs["EnableEncryptedPackageOpen"].Value);

            if (attribs["InjectEncryptedStreams"] != null)
                _injectEncryptedStreams = Convert.ToBoolean(attribs["InjectEncryptedStreams"].Value);

            if (attribs["EmbedUseLicense"] != null)
                _embedUseLicense = Convert.ToBoolean(attribs["EmbedUseLicense"].Value);

            if (attribs["EnableStorageInfoOpen"] != null)
                _enableStorageInfoOpen = Convert.ToBoolean(attribs["EnableStorageInfoOpen"].Value);

            if (attribs["InspectExceptionsFromEncryptedPackageOpen"] != null)
                _inspectExceptionsFromEncryptedPackageOpen = Convert.ToBoolean(attribs["InspectExceptionsFromEncryptedPackageOpen"].Value);

            if ((!_enableStorageInfoOpen) && (!_enableEncryptedPackageOpen))
            {
                throw new ApplicationException("RM Fuzz testing must have either EnableStorageInfoOpen or EnableEncryptedPackageOpen enabled!");
            }
            InitRM();
        }

        private void InitRM()
        {
            ReadOnlyCollection<ContentUser> activeUsers = SecureEnvironment.GetActivatedUsers();
            _author = activeUsers[0];

            SetUpPublishingInformation(_author, out _authorUseLicense, out _publishLicense, out _cryptoProvider);
        }

        /// <summary>
        /// Create an encryptedPackageEnvelope
        /// </summary>
        /// <returns></returns>
        protected override string CreateFile()
        {
            string tempContainerFile = GetRandomFileNameWithExtension("container");
            // Clean existing file.
            if (File.Exists(tempContainerFile))
            {
                File.Delete(tempContainerFile);
            }

            
            using (EncryptedPackageEnvelope encryptedPackage = EncryptedPackageEnvelope.Create(tempContainerFile, _publishLicense, _cryptoProvider))
            {
                // Embed the licenses into the CF
                if (_embedUseLicense)
                {
                    RightsManagementInformation rm = encryptedPackage.RightsManagementInformation;
                    rm.SaveUseLicense(_author, _authorUseLicense);
                }

                using (Package package = encryptedPackage.GetPackage())
                {

                    PackagePart contentPart = package.CreatePart(
                                                    PackUriHelper.CreatePartUri(
                                                        new Uri("foo.xaml", UriKind.Relative)
                                                        ),
                                                        "text/xml"
                                                    );
                    using (Stream contentStream = contentPart.GetStream())
                    {
                        string content = "Hello EncryptedPackage";
                        byte[] contentBytes = new UnicodeEncoding(false, false).GetBytes(content.ToCharArray());
                        contentStream.Write(contentBytes, 0, contentBytes.Length);
                    }
                }
            }
            return tempContainerFile;
        }

        /// <summary>
        /// Attempts to open the fuzzedFile into an EncryptedPackageEnvelope
        /// </summary>
        /// <param name="fuzzedFile"></param>
        /// <returns>EncryptedPackageEnvelope</returns>
        private EncryptedPackageEnvelope OpenEncryptedPackageEnvelope (string fuzzedFile) 
        {
            EncryptedPackageEnvelope encryptedPackage = null;
            try
            {
                encryptedPackage = EncryptedPackageEnvelope.Open(fuzzedFile);
            }
            catch (Exception Ex) 
            {
                CoreLogger.LogStatus("Failed opening fuzzed EncryptedPackageEnvelope.\r\n");
                CoreLogger.LogStatus("Fuzzing caused opening error: " + Ex.ToString() +"\r\n");
                if ((_inspectExceptionsFromEncryptedPackageOpen) && (! IsExceptionOkay( Ex)))
                {   
                    throw Ex;
                }
            }
            return encryptedPackage;
        }

        /// <summary>
        /// Attempts to get the package out of an encrypted envelope.
        /// </summary>
        /// <param name="encryptedPackage"></param>
        /// <returns></returns>
        private Package GetPackageFromEncryptedPackageEnvelope(EncryptedPackageEnvelope encryptedPackage)
        {
                RightsManagementInformation rm = encryptedPackage.RightsManagementInformation;
                if (null == rm)
                {
                    throw new FileFormatException("RightsManagementInformation instance could not be found in EncryptedPackageEnvelope instance.");
                }

              
                PublishLicense publishLicense = rm.LoadPublishLicense();
                if (null == publishLicense)
                {
                    throw new FileFormatException("Cannot read PublishLicense instance in RightsManagementInformation instance.");
                }

                // Get the use license out of the CFRM, creating one if necessary
                UseLicense useLicense = rm.LoadUseLicense(_author);
                if (useLicense == null)
                {
                    useLicense = publishLicense.AcquireUseLicense(_secureEnvironment);

                    if (useLicense != null)
                    {
                        rm.SaveUseLicense(_author, useLicense);
                    }
                    else
                    {
                        throw new ApplicationException("User has no right for use license."); // This user has no right
                    }
                }

                // Attempt to bind the use license to the secure environment.
                _cryptoProvider = useLicense.Bind(_secureEnvironment);
 
                if (null == _cryptoProvider)
                {
                    throw new Microsoft.Test.TestSetupException("Failed to bind author's use license to secure environment (no crypto provider)");
                }
                rm.CryptoProvider = _cryptoProvider;
                return encryptedPackage.GetPackage();
        }



        /// <summary>
        /// Testing a fuzzed EncryptedPackageEnvelope, sometimes through opening the encrypted package, 
        /// and sometimes by using storageInfo directly.
        /// </summary>
        /// <param name="fuzzedFile"></param>
        protected override void TestFuzzedFile(string fuzzedFile)
        {
            CoreLogger.LogStatus("Testing RM container..." + "\r\n");

            testPlanLogger.Log("<Title />");

            if (_enableEncryptedPackageOpen && _enableStorageInfoOpen)
            {
                if (random.Next(2) < 1)
                {
                    TestFuzzedFileByEncryptedPackageOpen(fuzzedFile);
                }
                else
                {
                    TestFuzzedFileByStorageInfoOpen(fuzzedFile);
                }
            }
            else if (_enableStorageInfoOpen) 
            {
                TestFuzzedFileByStorageInfoOpen(fuzzedFile);
            }
            else if (_enableEncryptedPackageOpen)
            {
                TestFuzzedFileByEncryptedPackageOpen(fuzzedFile);
            }
            else
            {
                throw new ApplicationException("RM Fuzz testing must have either EnableStorageInfoOpen or EnableEncryptedPackageOpen enabled!");
            }
            
        }

        /// <summary>
        /// Test a fuzzed file by using EncryptedPackageEnvelope.Open
        /// </summary>
        /// <param name="fuzzedFile"></param>
        private void TestFuzzedFileByEncryptedPackageOpen(string fuzzedFile)
        {

            StorageInfo stRoot = null;
            EncryptedPackageEnvelope encryptedPackage = null;
            Package stPackage = null;
            testPlanLogger.Log("<OpenEncryptedPackageEnvelopeAttempt />");
            encryptedPackage = OpenEncryptedPackageEnvelope(fuzzedFile);
            // Note: if we couldn't open the encryptedPackage, then we have no further testing of it...
            if (encryptedPackage != null)
            {
                testPlanLogger.Log("<OpenEncryptedPackageEnvelopeSuccess />");
                testPlanLogger.Log("<EncryptedPackageStorageInfoAttempt />");
                stRoot = encryptedPackage.StorageInfo;
                testPlanLogger.Log("<EncryptedPackageStorageInfoSuccess />");
                testPlanLogger.Log("<EncryptedPackageStorageInfoGetPackageAttempt />");
                stPackage = GetPackageFromEncryptedPackageEnvelope(encryptedPackage);
                testPlanLogger.Log("<EncryptedPackageStorageInfoGetPackageSuccess />");
                testPlanLogger.Log("<DumpStorageInfoAttempt />");
                DumpStorageInfo(stRoot, dumpDepth);
                testPlanLogger.Log("<DumpStorageInfoSuccess />");

            }
            else
            {
                testPlanLogger.Log("<OpenEncryptedPackageEnvelopeFailure />");
            }
        }


        /// <summary>
        /// Test a fuzzed file by using StorageInfo to Open.
        /// </summary>
        /// <param name="fuzzedFile"></param>
        private void TestFuzzedFileByStorageInfoOpen(string fuzzedFile)
        {
            StorageRootWrapper stRootWrapper = null;
            testPlanLogger.Log("<StorageRootWrapperOpenAttempt />");
            stRootWrapper = StorageRootWrapper.Open(fuzzedFile);
            testPlanLogger.Log("<StorageRootWrapperOpenSuccess />");
            testPlanLogger.Log("<DumpStorageInfoAttempt />");
            DumpStorageInfo(stRootWrapper, dumpDepth);
            testPlanLogger.Log("<DumpStorageInfoSuccess />");
        }



        /// <summary>
        /// Walks through the storage tree of the given StorageInfo,
        /// and randomly inserts new streams and storages, deletes storages and 
        /// streams, and changes existing streams.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="storageInfo"></param>
        protected override void DumpStorageInfo(StorageInfo storageInfo, int depth)
        {
            // Sometimes create an encrypted stream
            if ((random.Next(2) < 1) && _injectEncryptedStreams)
            {
                String nameForChildStream = "RandomSubStreamEncrypted" + (_randomInjectionCount++).ToString();
                testPlanLogger.Log("<CreateStreamEncrypted Name=\"" + nameForChildStream + "\" SeqNum=\"" + seqNum++ + "\"/>");
                StreamInfo childStreamInfo = storageInfo.CreateStream(nameForChildStream, 
                                                            CompressionOption.NotCompressed , 
                                                            EncryptionOption.RightsManagement );
                _WriteFuzzToStream(childStreamInfo, true);
            }
            // everything else is assumed to be the same
            base.DumpStorageInfo(storageInfo, depth);

        }


        private void SetUpPublishingInformation(
            ContentUser author,
            out UseLicense authorUseLicense,
            out PublishLicense publishLicense,
            out CryptoProvider cryptoProvider
            )
        {
            // Try to build a manifest and use it if it's successfully built.
            string manifestFile = "ContainerFuzzTestManifest.xml";
            BuildManifestForExe("CoreTests.exe", manifestFile);

            string applicationManifest = "<manifest></manifest>";
            if (File.Exists(manifestFile))
            {
                CoreLogger.LogStatus("USING A MANIFEST: " + manifestFile);
                StreamReader manifestReader = File.OpenText(manifestFile);
                applicationManifest = manifestReader.ReadToEnd();
            }

            _secureEnvironment = SecureEnvironment.Create(applicationManifest, author);

            UnsignedPublishLicense unsignedPublishLicense = new UnsignedPublishLicense();

            unsignedPublishLicense.Grants.Add(new ContentGrant(author, ContentRight.Owner));
            unsignedPublishLicense.Grants.Add(new ContentGrant(new ContentUser("Anyone", AuthenticationType.Internal), ContentRight.Owner));
            unsignedPublishLicense.Owner = author;

            publishLicense = unsignedPublishLicense.Sign(
                                                        _secureEnvironment,
                                                        out authorUseLicense
                                                        );



            // Attempt to bind the use license to the secure environment.
            //
            cryptoProvider = authorUseLicense.Bind(_secureEnvironment);

            if (cryptoProvider == null)
            {
                throw new Microsoft.Test.TestSetupException("Failed to bind author's use license to secure environment (no crypto provider)");
            }

          }

          // Function: BuildManifestForExe
          // Details : This will build a ISV manifest for a RM exe. Exe is expected to be
          //           in the current directory. Manifest will be pleced in the current 
          //           directory
          //           \\wpf\TestScratch\ghart\rm\genmanifest.exe is also required to be 
          //           in the current directory.
          // Thanx to GHart for this function.
          private void BuildManifestForExe(string exeName, string manifestFileName)
          {
              if (File.Exists(manifestFileName))
              {
                  File.Delete(manifestFileName);
              }

              StreamWriter sw = new StreamWriter("temp_rm.mcf");
              sw.WriteLine("AUTO-GUID");
              sw.WriteLine("MyPrivateKeyFile.dat");
              sw.WriteLine();
              sw.WriteLine("MODULELIST");
              sw.WriteLine("            REQ HASH " + exeName);
              sw.WriteLine("            REQ HASH PresentationHostDLL.dll");
              sw.WriteLine();
              sw.WriteLine("POLICYLIST");
              sw.WriteLine("            INCLUSION");
              sw.WriteLine("            PUBLICKEY MyPubKeyFile.dat");
              sw.Close();

              _manifestGenerationDone = false;
              Process myProcess = new Process();
              myProcess.StartInfo.FileName = "genmanifest.exe";
              myProcess.StartInfo.Arguments = "-chain edocs_Client.xml temp_rm.mcf " + manifestFileName;
              myProcess.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
              myProcess.EnableRaisingEvents = true;
              myProcess.Exited += new EventHandler(OnManifestGenerationExit);
              myProcess.Start();

              while (!_manifestGenerationDone)
              {
                  Thread.Sleep(1000);
              }

              if (File.Exists("temp_rm.mcf"))
              {
                  File.Delete("temp_rm.mcf");
              }          
          }

        private void OnManifestGenerationExit(object sender, EventArgs e)
        {
            _manifestGenerationDone = true;
        }

        bool _manifestGenerationDone;
        UseLicense _authorUseLicense;
        PublishLicense _publishLicense;
        CryptoProvider _cryptoProvider;
        ContentUser _author;
        SecureEnvironment _secureEnvironment;
        Boolean _injectEncryptedStreams = false;
        Boolean _enableEncryptedPackageOpen = true;
        Boolean _enableStorageInfoOpen = true;
        Boolean _embedUseLicense = true;
        Boolean _inspectExceptionsFromEncryptedPackageOpen = true;

    }
}
