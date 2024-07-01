// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  The set of Developer Regression Tests for the MMCF PackageDigitalSignature classes.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.IO.Packaging;
using System.Xml;               // XmlTextWriter

using MS.Utility;

namespace DRT
{
    /// <summary>
    /// PackageDigitalSignature DRT class. All tests live as methods within this class.
    /// </summary>
    public sealed class DigitalSignatureTestHarness : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DigitalSignatureTestHarness();
            return drt.Run(args);
        }

        private DigitalSignatureTestHarness()
        {
            WindowTitle = DigSigTestSuite.Title;
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = DigSigTestSuite.Title;
            Suites = new DrtTestSuite[]
            {
                new DigSigTestSuite(),
                null            // list terminator - optional
            };
        }
    }

    public sealed class DigSigTestSuite : DrtTestSuite
    {
        #region Statics
        public static string Title = "DrtDigitalSignature";
        static String s_file = Title + ".container";
        static int s_testNum = 0;        // test number

        // must match was is created by CertificateCreate.bat
        static string s_australiaCertFile = @"drtfiles\DigitalSignature\Australia.cer";
        static string s_africaCertFile = @"drtfiles\DigitalSignature\Africa.cer";
        static string s_asiaCertFile = @"drtfiles\DigitalSignature\Asia.cer";
        static string s_booBooFile = @"drtfiles\DigitalSignature\booboo.xml";
        static string s_part1File = @"drtfiles\DigitalSignature\part1.xml";
        static string s_image1File = @"drtfiles\DigitalSignature\image1.jpg";
        static string s_certFile = null;
        static Uri s_packageRelationshipPart = CreatePartUri("_rels/.rels");
        static Uri s_originPartUri = CreatePartUri("/package/services/digital-signature/origin.psdsor");
        static Uri s_originPartRelationshipUri = CreatePartUri("/package/services/digital-signature/_rels/origin.psdsor.rels");

        // command-line switches
        bool _dontDeleteContainer;      // keep container around after test?

        // temp variables (passing data between test suites)
        private Uri _signatureName;
        private int _sigCount;
        private Uri _signatureName2;

        private Uri[] _partUri = new Uri[]
        {
            new Uri("/baabaa.nie", UriKind.Relative),
            new Uri("/booboo.xml", UriKind.Relative),
            new Uri("/part1.xaml", UriKind.Relative),
            new Uri("/shared/image1.jpg", UriKind.Relative),    // deleted in Exercise() test
            new Uri("/shared/image2.jpg", UriKind.Relative),
            new Uri("/shared/image3.jpg", UriKind.Relative),
        };

        private string[] _partContentType = new string[]
        {
            "fake/contenttype", // no transforms
            "text/xml",     // C14N with comments
            "text/xaml",    // C14N
            "image/jpg",    // no transforms
            @"image/jpg",    // no transforms
            "image/jpg",    // no transforms
        };
        #endregion

        public DigSigTestSuite() : base(Title)
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // create a container
            CreatePackage(s_file);

            // return the lists of tests to run against the tree
            return new DrtTest[]
            {
                new DrtTest( XmlTests ),
                new DrtTest( RelationshipSigningTest ),
                new DrtTest( CertificateSharingTests ),
                new DrtTest( Exercise ),
                new DrtTest( EnumeratorTests ),             // uses output from Exercise test
                new DrtTest( ExercisePhase2 ),              // uses output from Exercise test
                new DrtTest( ExerciseSignatureObject ),
                new DrtTest( ExerciseSigningCustomSignatureObject ),
                new DrtTest( ExerciseSigningCustomSignatureObject2 ),
                new DrtTest( ExerciseXAdES ),
                new DrtTest( VerifyHardenedDelegate ),
                new DrtTest( ExerciseSignatureMethodMapping ),  // Recreates file for clean signature testing
                null        // list terminator - optional
            };
        }

        public override void ReleaseResources()
        {
            // clean up - delete file
            if (!_dontDeleteContainer)
            {
                Console.WriteLine("\nDeleting container file: " + s_file);
                Console.WriteLine();
                FileInfo fi = new FileInfo(s_file);
                if (fi.Exists)
                    fi.Delete();
            }
        }

        public override void PrintOptions()
        {
            // general options
            base.PrintOptions();

            Console.WriteLine("\n\nDrtDigitalSignature options:");
            Console.WriteLine("  -nodelete         doesn't delete the generated container - useful for debugging");
        }

        /// <summary>
        /// Override this in derived classes to handle command-line arguments one-by-one.
        /// </summary>
        /// <param name="arg">current argument</param>
        /// <param name="option">if there was a leading "-" or "/" to arg</param>
        /// <param name="args">the array of command line arguments</param>
        /// <param name="k">current index in the argument array.  passed by ref so you can increase it to "consume" arguments to options.</param>
        /// <returns>True if handled</returns>
        public override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            if (option)
            {
                switch (arg)
                {
                    case "nodelete":
                        Console.WriteLine("Container delete disabled for this run - see: " + s_file);
                        _dontDeleteContainer = true;        // no point if container is gone
                        break;

                    case "cert":
                        s_certFile = args[++k];
                        Console.WriteLine("Using custom certificate: " + s_certFile);
                        s_australiaCertFile = s_certFile;      // override to do custom tests
                        break;

                    // we don't recognize so pass to base class for default handling
                    default:
                        return base.HandleCommandLineArgument(arg, option, args, ref k);
                }

                return true;
            }

            return false;
        }

        static public void OnInvalidSignature(object source, SignatureVerificationEventArgs e)
        {
            Console.WriteLine("\nOnInvalidSignature Event:\nInvalid signature found: " + e.VerifyResult);
            DisplaySignature(e.Signature, true);
            Console.WriteLine();
        }

        static public void DisplaySignatures(PackageDigitalSignatureManager dsm, bool refs)
        {
            foreach (PackageDigitalSignature ds in dsm.Signatures)
                DisplaySignature(ds, refs);
        }

        static public void DisplaySignature(PackageDigitalSignature ds, bool refs)
        {
            X509Certificate cert = ds.Signer;
            StringBuilder sb = new StringBuilder();
            System.DateTime stamp = ds.SigningTime;

            sb.AppendFormat("Signature: {0}", ds.SignaturePart.Uri);
            if (cert != null)
            {
                sb.AppendFormat("\nCertificate:\n\tSerial: {0}\n\tPrincipal: {1}\n\tTimestamp: {2} UTC", cert.GetSerialNumberString(), cert.GetName(), stamp.ToString());
            }
            else
                sb.Append("\nCertificate: {no certificate}");

            Console.WriteLine(sb.ToString());
            Console.WriteLine();
            if (refs)
            {
                Console.WriteLine("References for this signature:");
                foreach (Uri uri in ds.SignedParts)
                    DumpUri(uri);
            }
        }

        /// <summary>
        /// Dump reference to log
        /// </summary>
        /// <param name="cr">reference</param>
        static void DumpUri(Uri uri)
        {
            Console.WriteLine("\t" + uri.ToString());
        }

        /// <summary>
        /// Test collection properties to ensure that they behave as expected
        /// </summary>
        public void EnumeratorTests()
        {
            X509Certificate2 cert = new X509Certificate2(s_africaCertFile);

            Console.WriteLine("\n---------------------------------------->");
            Console.WriteLine("  Enumerator Tests:");
            Console.WriteLine("---------------------------------------->\n");

            // ensure that Signatures list is invalidating enumerators when list changes
            // open
            LogTest("Open existing package to exercise enumerators", s_testNum++);
            Package p = Package.Open(s_file, FileMode.Open);
            PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(p);

            LogTest("Get signature count and then ensure that it is invalidated when we change the count", s_testNum++);
            IList<PackageDigitalSignature> signatures = dsm.Signatures;
            int n = signatures.Count;

            Console.WriteLine("Enumerate signatures:");
            IEnumerator<PackageDigitalSignature> e = signatures.GetEnumerator();
            while (e.MoveNext())
            {
                Console.WriteLine("    Signature: {0}", e.Current.SignaturePart.Uri.ToString());
            }

            // re-enumerate to a partial result (some left)
            e = signatures.GetEnumerator();
            e.MoveNext();       // just start it

            Console.WriteLine("Now add a signature");
            PackageDigitalSignature sig = dsm.Countersign(cert);
            _sigCount++;    // update global so next test is not fooled

            DRT.Assert(dsm.Signatures.Count == n + 1, "Count is incorrect - should have one more signature.  Old count: {0} New Count: {1}",
                n, dsm.Signatures.Count);

            Console.WriteLine("Original Enumerator should be invalid now");
            bool exceptionCaught = false;
            try
            {
                e.MoveNext();
            }
            catch (Exception ex)
            {
                exceptionCaught = true;
                Console.WriteLine("-> Expected exception: {0}", ex.Message);
            }
            DRT.Assert(exceptionCaught, "-> Enumerator should have thrown an exception - FAIL");

            Console.WriteLine("Original returned collection should automatically reflect new changes");
            DRT.Assert(signatures.Count == dsm.Signatures.Count, "FAIL: old collection count: {0} should equal new count {1}",
                signatures.Count, dsm.Signatures.Count);

            Console.WriteLine("New enumerator off old collection should work fine too");
            e = signatures.GetEnumerator();
            while (e.MoveNext())
            {
                Console.WriteLine("    Signature: {0}", e.Current.SignaturePart.Uri.ToString());
            }

            Console.WriteLine("  --> success");

            p.Close();
        }

        public void Exercise()
        {
            Console.WriteLine("\n---------------------------------------->");
            Console.WriteLine("  Exercise Client API: Phase 1");
            Console.WriteLine("---------------------------------------->\n");

            DRT.Assert((new FileInfo(s_australiaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_australiaCertFile + ".  Run CMD file instead of EXE.");
            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 cert = new X509Certificate2(s_australiaCertFile);
            X509Certificate2 cert2 = new X509Certificate2(s_africaCertFile);
            DRT.Assert(cert != null, "No usable certificate found on this machine or none selected.");

            // save statistics for round-trip tests
            _sigCount = -1;                      // verify same number on open
            {
                Package c = Package.Open(s_file, FileMode.Open);
                try
                {
                    // create a digital signature manager
                    PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(c);
                    dsm.CertificateOption = CertificateEmbeddingOption.InSignaturePart;

                    // register for invalid signature verification event
                    dsm.InvalidSignatureEvent += new InvalidSignatureEventHandler(OnInvalidSignature);

                    LogTest("Sign zero parts should throw", s_testNum++);
                    System.Collections.Generic.List<Uri> toSign = new System.Collections.Generic.List<Uri>();
                    bool thrown = false;
                    PackageDigitalSignature sig = null;
                    try
                    {
                        sig = dsm.Sign(toSign, cert);
                    }
                    catch (ArgumentException argEx0)
                    {
                        Console.WriteLine("Caught expected ArgumentException for param {0}: {1}", argEx0.ParamName, argEx0.Message);
                        thrown = true;
                    }
                    DRT.Assert(thrown, "Sign with zero parts should throw");

                    LogTest("Sign non-existent part should throw", s_testNum++);
                    toSign.Add(new Uri("/PartThatDoesNotExist", UriKind.Relative));
                    thrown = false;
                    try
                    {
                        sig = dsm.Sign(toSign, cert);
                    }
                    catch (ArgumentException argEx1)
                    {
                        Console.WriteLine("Caught expected ArgumentException for param {0}: {1}", argEx1.ParamName, argEx1.Message);
                        thrown = true;
                    }
                    DRT.Assert(thrown, "Sign with unknown parts should throw");
                    toSign.Clear();

                    LogTest("Sign all non-Relationship parts", s_testNum++);
                    int skippedRels = 0;
                    foreach (PackagePart p in c.GetParts())
                    {
                        if (PackUriHelper.IsRelationshipPartUri(p.Uri))
                        {
                            skippedRels++;
                        }
                        else
                        {
                            Console.WriteLine("    -> " + p.Uri);
                            toSign.Add(p.Uri);
                        }
                    }
                    if (skippedRels > 0)
                        Console.WriteLine("    [skipped {0} Relationship Parts]", skippedRels);

                    LogTest("Sign with bad hashAlgorithm should throw and clean-up", s_testNum++);
                    thrown = false;
                    try
                    {
                        dsm.HashAlgorithm = "NoAlgorithmYouEverHeardOf";
                        sig = dsm.Sign(toSign, cert);
                    }
                    catch (InvalidOperationException ioEx)
                    {
                        Console.WriteLine("Caught expected InvalidOperationException bad hash algorithm {0}: {1}", dsm.HashAlgorithm, ioEx.Message);
                        thrown = true;
                    }
                    DRT.Assert(thrown, "Sign with bad hashAlgorithm should throw");
                    DRT.Assert(dsm.Signatures.Count == 0, "Sign with bad hashAlgorithm should clean-up - signature count should be zero");
                    DRT.Assert(!c.PartExists(dsm.SignatureOrigin), "Sign with bad hashAlgorithm should clean-up - origin should have been removed");
                    dsm.HashAlgorithm = PackageDigitalSignatureManager.DefaultHashAlgorithm;


                    sig = dsm.Sign(toSign, cert);

                    LogTest("Verify", s_testNum++);
                    VerifyResult verifyResult = dsm.VerifySignatures(false);

                    DRT.Assert(verifyResult == VerifyResult.Success, "Test 1b: Digital signature verify failed");
                    // sign again
                    System.Collections.Generic.List<Uri> partList = new System.Collections.Generic.List<Uri>();
                    for (int i = 0; i < _partUri.Length; i++)
                    {
                        partList.Add(PackUriHelper.CreatePartUri(_partUri[i]));
                    }
                    DRT.Assert(sig.CertificateEmbeddingOption == CertificateEmbeddingOption.InSignaturePart,
                        "CertificateEmbedding option should be InSignaturePart: " + sig.CertificateEmbeddingOption);

                    LogTest("Sign part list without embedding certificate at all", s_testNum++);
                    dsm.CertificateOption = CertificateEmbeddingOption.NotEmbedded;

                    // sign without embedding the certificate
                    PackageDigitalSignature noCertSig = dsm.Sign(partList, cert);
                    LogTest("Verify - passing certificate", s_testNum++);
                    verifyResult = noCertSig.Verify(cert);        // must pass the cert to verify
                    DRT.Assert(verifyResult == VerifyResult.Success, "Test 2b: digital signature verify failed");
                    DRT.Assert(noCertSig.CertificateEmbeddingOption == CertificateEmbeddingOption.NotEmbedded,
                        "CertificateEmbedding option should be None: " + noCertSig.CertificateEmbeddingOption);

                    dsm.HashAlgorithm = "http://www.w3.org/2001/04/xmlenc#sha512";
                    dsm.CertificateOption = CertificateEmbeddingOption.InCertificatePart;
                    LogTest("Sign modified part list", s_testNum++);

                    // sign again embedding the certificate
                    partList.RemoveAt(0);   // change the list
                    sig = dsm.Sign(partList, cert);
                    LogTest("\n3b. Dump Manifest", s_testNum++);
                    DisplaySignature(sig, true);

                    LogTest("Verify signature", s_testNum++);
                    verifyResult = sig.Verify();
                    DRT.Assert(verifyResult == VerifyResult.Success, "Test 3c: digital signature verify failed");

                    LogTest("Global verify - should be invalid because of sig with no embedded cert", s_testNum++);

                    // global verify - should fail because no-cert embedded for one sig
                    verifyResult = dsm.VerifySignatures(false);
                    DRT.Assert(verifyResult != VerifyResult.Success, "Failure: Digital signature should not verify\n");

                    LogTest("Remove signature with no embedded cert", s_testNum++);

                    // remove sig with no cert and verify again
                    dsm.RemoveSignature(noCertSig.SignaturePart.Uri);
                    LogTest("Global Verify - should succeed", s_testNum++);
                    verifyResult = dsm.VerifySignatures(false);
                    DRT.Assert(verifyResult == VerifyResult.Success, "Test 6: digital signature verify failed");

                    // counter sign
                    dsm.HashAlgorithm = "http://www.w3.org/2001/04/xmlenc#sha256";
                    LogTest("Countersign", s_testNum++);
                    //                    dsm.HashAlgorithm = SignedXml.XmlDsigHMACSHA1Url;
                    sig = dsm.Countersign(cert);
                    verifyResult = sig.Verify();
                    //                    DRT.Assert(String.Compare(sig.HashAlgorithm, SignedXml.XmlDsigHMACSHA1Url, true) == 0,
                    //                        "Fail: Hash algorithm doesn't match http://www.w3.org/2000/09/xmldsig#hmac-sha1: " + sig.HashAlgorithm);
                    DRT.Assert(verifyResult == VerifyResult.Success, "Test 7: digital signature verify failed");

                    // verify "publisher" signature scenario
                    LogTest("Sign origin to prevent addition of more signatures", s_testNum++);
                    toSign.Clear();
                    toSign.Add(s_packageRelationshipPart);       // sign _rels/.rels too - just to test the scenario
                    toSign.Add(s_originPartRelationshipUri);
                    sig = dsm.Sign(toSign, cert2);
                    verifyResult = sig.Verify();
                    DRT.Assert(verifyResult == VerifyResult.Success, "Digital signature verify failed after signing Signature Origin Relationship part");

                    // now verify counter-sign fails
                    PackageDigitalSignature sigX = dsm.Countersign(cert2);
                    verifyResult = dsm.VerifySignatures(false);
                    DRT.Assert(verifyResult == VerifyResult.InvalidSignature, "Digital signature verify should have failed after signing Signature Origin Relationship part");

                    // remove these signatures to allow further testing
                    dsm.RemoveSignature(sigX.SignaturePart.Uri);
                    dsm.RemoveSignature(sig.SignaturePart.Uri);

                    // restore hash algorithm
                    dsm.HashAlgorithm = PackageDigitalSignatureManager.DefaultHashAlgorithm;

                    LogTest("Sign Relationships with Africa cert", s_testNum++);
                    PackageRelationshipCollection packageRelationships = c.GetRelationships();
                    List<PackageRelationship> relationships = new List<PackageRelationship>(packageRelationships);
                    //                    Uri _originPartUri = CreatePartUri("/package/services/digital-signature/origin.psdsor");
                    //                    PackageRelationshipCollection otherRelationships = c.GetPart(_originPartUri).GetRelationships();
                    //                    relationships.InsertRange(1, otherRelationships);       // mix it up
                    Console.WriteLine("No Parts");
                    DumpRelationships(relationships);
                    PackageDigitalSignature africaSig = dsm.Sign(null, cert2, GetRelationshipSelectors(relationships));     // just relationships
                    LogTest("Verify Africa signature", s_testNum++);
                    verifyResult = africaSig.Verify();
                    DRT.Assert(verifyResult == VerifyResult.Success,
                        "Test: " + s_testNum + " digital signature verify failed for Africa signature");

                    LogTest("Sign Relationships and Parts with Australia cert", s_testNum++);
                    PackageRelationshipCollection otherRelationships = c.GetPart(s_originPartUri).GetRelationships();
                    relationships.Clear();
                    relationships.AddRange(otherRelationships);
                    relationships.RemoveAt(0);      // don't sign all relationships
                    DumpParts(partList, c);
                    DumpRelationships(relationships);
                    dsm.TimeFormat = "YYYY-MM";
                    PackageDigitalSignature aussieSig = dsm.Sign(partList, cert, GetRelationshipSelectors(relationships));  // parts + relationships
                    _signatureName2 = aussieSig.SignaturePart.Uri;           // save this for later
                    dsm.TimeFormat = "YYYY-MM-DDThh:mm:ss.sTZD";        // restore
                    LogTest("Verify Australia signature", s_testNum++);
                    verifyResult = aussieSig.Verify();
                    DRT.Assert(verifyResult == VerifyResult.Success, "Test 8d: digital signature verify failed for Australia signature");

                    // ensure ID and GetSignature work
                    LogTest("GetSignature by ID: " + africaSig.SignaturePart.Uri, s_testNum++);
                    PackageDigitalSignature sig2 = dsm.GetSignature(africaSig.SignaturePart.Uri);
                    DisplaySignature(sig2, false);
                    DRT.Assert(africaSig == sig2, "Fail: PackageDigitalSignatureManager.GetSignature failed.  {0} != {1}", sig, sig2);

                    // save statistics for round-trip tests
                    _signatureName = africaSig.SignaturePart.Uri;                  // use this to open with new PackageDigitalSignatureManager
                    _sigCount = dsm.Signatures.Count;                        // verify same number on open
                    Console.WriteLine("Close container");
                }
                finally
                {
                    c.Flush();
                    c.Close();
                }
            }
        }

        public void ExercisePhase2()
        {
            // PHASE 2
            {
                Console.WriteLine("\n---------------------------------------->");
                Console.WriteLine("  Exercise Client API: Phase 2");
                Console.WriteLine("---------------------------------------->\n");

                // open
                LogTest("Open again to verify serialization", s_testNum++);
                Package c2 = Package.Open(s_file, FileMode.Open);
                PackageDigitalSignatureManager dsm2 = new PackageDigitalSignatureManager(c2);

                LogTest("GetSignature with ID returned when signature was originally created", s_testNum++);
                PackageDigitalSignature sig3 = dsm2.GetSignature(_signatureName);
                DRT.Assert(sig3 != null && sig3.SignaturePart.Uri == _signatureName, "Fail: GetSignature after close and re-open failed with _signatureName: " + _signatureName);
                Console.WriteLine("  --> success");

                LogTest("DeleteSignature with embedded certificate", s_testNum++);
                dsm2.RemoveSignature(dsm2.Signatures[0].SignaturePart.Uri);
                _sigCount--;     // since we are deleting a signature we need to adjust our numbers for test 15

                LogTest("Verify Certificate status", s_testNum++);
                if (sig3 != null)
                {
                    X509ChainStatusFlags flags = PackageDigitalSignatureManager.VerifyCertificate(sig3.Signer);
                    if (flags != X509ChainStatusFlags.NoError)
                    {
                        Console.WriteLine("[non-fatal] Certificate status invalid: " + flags);
                    }
                }

                LogTest("Verify Signature count: " + dsm2.Signatures.Count, s_testNum++);
                DRT.Assert(dsm2.Signatures.Count == _sigCount, "Fail: Signature count before close " + _sigCount + " != signature count on open: " + dsm2.Signatures.Count);
                Console.WriteLine("  --> success");

                LogTest("Remove a signed Part and verify correct invalid result", s_testNum++);
                if (VerifyPartRemovalBreaksSignature(c2, dsm2, dsm2.Signatures[0], _partUri[3]))
                    Console.WriteLine("  --> success");

                LogTest("Transform Query: look for transforms for a single Part and a Relationship Part", s_testNum++);
                PackageDigitalSignature xFormSig = dsm2.GetSignature(_signatureName2);
                List<String> transformList = xFormSig.GetPartTransformList(_partUri[1]);
                DumpXransformList(transformList, _partUri[1]);
                //                DRT.Assert(transformList.Count == 1, "Fail: Xml type part should undergo one transformation");
                Console.WriteLine("  --> success");

                // now query for a relationship part
                LogTest("Confirm that Relationship has two Transforms", s_testNum++);
                Uri originRelationshipPart = PackUriHelper.GetRelationshipPartUri(s_originPartUri);
                transformList = xFormSig.GetPartTransformList(originRelationshipPart);
                DumpXransformList(transformList, originRelationshipPart);
                DRT.Assert(transformList.Count == 2, "Fail: Relationship part should undergo two transformations");
                Console.WriteLine("  --> success");

                LogTest("TimeFormat tests", s_testNum++);
                DumpTimeFormat(xFormSig, "YYYY-MM");
                DumpTimeFormat(dsm2.Signatures[0], "YYYY-MM-DDThh:mm:ss.sTZD");

                LogTest("Negative CounterSign tests", s_testNum++);
                X509Certificate2 cert = new X509Certificate2(s_africaCertFile);
                List<Uri> sigsToSign = new List<Uri>();
                foreach (PackageDigitalSignature s in dsm2.Signatures)
                    sigsToSign.Add(s.SignaturePart.Uri);

                DRT.Assert(NegativeCounterSign(dsm2, cert, null), "Fail: CounterSign with null collection should have failed");
                DRT.Assert(NegativeCounterSign(dsm2, null, sigsToSign), "Fail: CounterSign with null collection should have failed");
                DRT.Assert(NegativeCounterSign(dsm2, null, null), "Fail: CounterSign with all null arguments should have failed");
                DRT.Assert(!NegativeCounterSign(dsm2, cert, sigsToSign), "Fail: CounterSign with correct arguments should have passed");
                sigsToSign.Add(new Uri("http://www.yahoo.com", UriKind.Absolute));  // add a non-signature
                DRT.Assert(NegativeCounterSign(dsm2, cert, sigsToSign), "Fail: CounterSign with non-signature collection should have failed");

                // can be disabled for debugging purposes
                LogTest("Delete all signatures", s_testNum++);
                dsm2.RemoveAllSignatures();
                DRT.Assert(dsm2.Signatures.Count == 0, "Fail: Signature count after RemoveAllSignatures > 0: " + dsm2.Signatures.Count);
                Console.WriteLine("  --> success\n");

                c2.Close();
            }
        }

        /// <summary>
        /// Call this to exercise the Countersign method
        /// </summary>
        /// <param name="dsm"></param>
        /// <param name="cert"></param>
        /// <param name="signatures"></param>
        /// <returns>true if an exception was thrown</returns>
        private bool NegativeCounterSign(PackageDigitalSignatureManager dsm,
            X509Certificate2 cert, IEnumerable<Uri> signatures)
        {
            try
            {
                dsm.Countersign(cert, signatures);
            }
            catch (Exception)
            {
                return true;
            }
            return false;
        }

        public void ExerciseSignatureObject()
        {
            Console.WriteLine("\n---------------------------------------->");
            Console.WriteLine("  Exercise Signature Object");
            Console.WriteLine("---------------------------------------->\n");

            DRT.Assert((new FileInfo(s_australiaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_australiaCertFile + ".  Run CMD file instead of EXE.");
            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 certAus = new X509Certificate2(s_australiaCertFile);
            X509Certificate2 certAfrica = new X509Certificate2(s_africaCertFile);
            DRT.Assert(certAfrica != null, "No usable certificate found on this machine or none selected.");

            Package c = Package.Open(s_file, FileMode.Open);

            // create a digital signature manager
            PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(c);
            dsm.CertificateOption = CertificateEmbeddingOption.InSignaturePart;

            // register for invalid signature verification event
            dsm.InvalidSignatureEvent += new InvalidSignatureEventHandler(OnInvalidSignature);

            LogTest("Sign using custom id", s_testNum++);
            System.Collections.Generic.List<Uri> toSign = new System.Collections.Generic.List<Uri>();
            int skippedRels = 0;
            foreach (PackagePart p in c.GetParts())
            {
                if (PackUriHelper.IsRelationshipPartUri(p.Uri))
                {
                    skippedRels++;
                }
                else
                {
                    Console.WriteLine("    -> " + p.Uri);
                    toSign.Add(p.Uri);
                }
            }
            if (skippedRels > 0)
                Console.WriteLine("    [skipped {0} Relationship Parts]", skippedRels);

            string[] idTable = new string[]
            {
                "abc123",
                "",								// illegal
                "PollyWantACracker",
                "#fragment",
            };

            LogTest("Sign - specifying Id", s_testNum++);
            Console.WriteLine("Digital signature id round-trip tests:");
            for (int i = 0; i < idTable.Length; i++)
            {
                PackageDigitalSignature sig = null;
                try
                {
                    sig = dsm.Sign(toSign, certAfrica, null, idTable[i]);
                }
                catch (ArgumentException e)
                {
                    if (e.ParamName == "signatureId")
                    {
                        Console.WriteLine("\tExpected rejection of malformed signatureId string: '" + idTable[i] + "'");
                    }
                    else
                        throw e;
                }

                if (sig == null)
                    continue;

                VerifyResult verifyResult = dsm.VerifySignatures(false);

                DRT.Assert(verifyResult == VerifyResult.Success, "Digital signature verify failed");

                // get the signature object
                Signature signatureObject = sig.Signature;

                // verify that id round-trips
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("\t{0}\t->\t{1}", idTable[i] == String.Empty ? "<empty>" : idTable[i], signatureObject.Id == null ? "<null>" : signatureObject.Id);
                Console.WriteLine(sb.ToString());
            }

            LogTest("Signature Invalidation Tests", s_testNum++);
            Console.WriteLine("Delete a signature and ensure that the object is then unusable");
            PackageDigitalSignature[] sigs = new PackageDigitalSignature[dsm.Signatures.Count];
            dsm.Signatures.CopyTo(sigs, 0);
            dsm.RemoveSignature(dsm.Signatures[1].SignaturePart.Uri);

            object o = null;
            bool exceptionCaught = false;
            try
            {
                o = sigs[1].Signature;
            }
            catch (Exception)
            {
                exceptionCaught = true;
            }
            DRT.Assert(exceptionCaught, "Digital signature object should have been invalid after underlying signature was deleted. ");

            // ensure other two signatures CAN be accessed
            o = sigs[0].Signature;
            o = sigs[2].Signature;

            // update the signature - add an unsigned Object tag
            Signature sigObject = sigs[2].Signature;

            // create an <Object> tag:
            sigObject.ObjectList.Add(GenerateObjectTag("unsigned-after-signing"));
            sigs[2].Signature = sigObject;

            VerifyResult result = sigs[2].Verify();
            DRT.Assert(result == VerifyResult.Success,
                "Verify fails after Signature object round-trip");

            c.Close();
        }

        public void ExerciseSigningCustomSignatureObject()
        {
            Console.WriteLine("\n---------------------------------------->");
            Console.WriteLine("  Exercise Signing Signature Object");
            Console.WriteLine("---------------------------------------->\n");

            DRT.Assert((new FileInfo(s_australiaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_australiaCertFile + ".  Run CMD file instead of EXE.");
            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 certAus = new X509Certificate2(s_australiaCertFile);
            X509Certificate2 certAfrica = new X509Certificate2(s_africaCertFile);
            DRT.Assert(certAfrica != null, "No usable certificate found on this machine or none selected.");

            Package c = Package.Open(s_file, FileMode.Open);

            // create a digital signature manager
            PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(c);
            dsm.CertificateOption = CertificateEmbeddingOption.InSignaturePart;

            // register for invalid signature verification event
            dsm.InvalidSignatureEvent += new InvalidSignatureEventHandler(OnInvalidSignature);

            LogTest("Sign using custom id", s_testNum++);
            System.Collections.Generic.List<Uri> toSign = new System.Collections.Generic.List<Uri>();
            int skippedRels = 0;
            foreach (PackagePart p in c.GetParts())
            {
                if (PackUriHelper.IsRelationshipPartUri(p.Uri))
                {
                    skippedRels++;
                }
                else
                {
                    Console.WriteLine("    -> " + p.Uri);
                    toSign.Add(p.Uri);
                }
            }
            if (skippedRels > 0)
                Console.WriteLine("    [skipped {0} Relationship Parts]", skippedRels);

            LogTest("Sign - custom object", s_testNum++);
            Console.WriteLine("Sign custom object:");

            Reference objectReference = new Reference("#signedObject");
            List<Reference> objectReferences = new List<Reference>(4);
            objectReferences.Add(objectReference);

            List<DataObject> signatureObjects = new List<DataObject>(4);
            signatureObjects.Add(GenerateObjectTag("signedObject"));
            signatureObjects.Add(GenerateObjectTag("unsignedObject"));

            PackageDigitalSignature sig = null;
            sig = dsm.Sign(toSign, certAfrica, null, "sig1", signatureObjects, objectReferences);

            VerifyResult verifyResult = dsm.VerifySignatures(false);
            DRT.Assert(verifyResult == VerifyResult.Success, "Digital signature verify failed");

            // get the signature object
            Signature signatureObject = sig.Signature;

            // verify that the objects round-tripped
            Console.WriteLine("<Object> tags in resulting signature:");
            foreach (DataObject obj in signatureObject.ObjectList)
            {
                // check if the object is signed or not
                string type = "unsigned";
                foreach (Reference r in signatureObject.SignedInfo.References)
                {
                    if (String.CompareOrdinal(r.Uri.Substring(1), obj.Id) == 0)
                    {
                        type = "signed";
                        break;
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("\t{0} status: {1}", obj.Id, type);
                Console.WriteLine(sb.ToString());
            }

            c.Close();
        }

        public void ExerciseSigningCustomSignatureObject2()
        {
            Console.WriteLine("\n---------------------------------------->");
            Console.WriteLine("  Exercise Signing Signature Object - no parts");
            Console.WriteLine("---------------------------------------->\n");

            DRT.Assert((new FileInfo(s_australiaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_australiaCertFile + ".  Run CMD file instead of EXE.");
            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 certAus = new X509Certificate2(s_australiaCertFile);
            X509Certificate2 certAfrica = new X509Certificate2(s_africaCertFile);
            DRT.Assert(certAfrica != null, "No usable certificate found on this machine or none selected.");

            Package c = Package.Open(s_file, FileMode.Open);

            // create a digital signature manager
            PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(c);
            dsm.CertificateOption = CertificateEmbeddingOption.InSignaturePart;

            // register for invalid signature verification event
            dsm.InvalidSignatureEvent += new InvalidSignatureEventHandler(OnInvalidSignature);

            LogTest("Sign - custom object", s_testNum++);
            Console.WriteLine("Sign custom object:");

            Reference objectReference = new Reference("#signedObject");
            List<Reference> objectReferences = new List<Reference>(4);
            objectReferences.Add(objectReference);

            List<DataObject> signatureObjects = new List<DataObject>(4);
            signatureObjects.Add(GenerateObjectTag("signedObject"));
            signatureObjects.Add(GenerateObjectTag("unsignedObject"));

            // Sign the SignatureOrigin part contents because <Manifest> must contain
            // at least a single <Reference> tag by w3c spec.  The Signature Origin is always empty
            // so this is a relatively benign choice.
            System.Collections.Generic.List<Uri> partsToSign = new System.Collections.Generic.List<Uri>(1);
            partsToSign.Add(dsm.SignatureOrigin);

            PackageDigitalSignature sig = null;
            sig = dsm.Sign(partsToSign, certAfrica, null, "sig2", signatureObjects, objectReferences);

            VerifyResult verifyResult = sig.Verify();
            DRT.Assert(verifyResult == VerifyResult.Success, "Digital signature verify failed");

            // get the signature object
            Signature signatureObject = sig.Signature;

            // verify that the objects round-tripped
            Console.WriteLine("<Object> tags in resulting signature:");
            foreach (DataObject obj in signatureObject.ObjectList)
            {
                // check if the object is signed or not
                string type = "unsigned";
                foreach (Reference r in signatureObject.SignedInfo.References)
                {
                    if (String.CompareOrdinal(r.Uri.Substring(1), obj.Id) == 0)
                    {
                        type = "signed";
                        break;
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("\t{0} status: {1}", obj.Id, type);
                Console.WriteLine(sb.ToString());
            }

            // ensure that object id uniqueness check is functional
            bool exceptionThrown = false;
            try
            {
                signatureObjects.Add(GenerateObjectTag("signedObject"));    // add a duplicate
                sig = dsm.Sign(partsToSign, certAfrica, null, "sig2", signatureObjects, objectReferences);
            }
            catch (ArgumentException argEx)
            {
                // ignore
                Console.WriteLine("Caught expected ArgumentException for param {0}: {1}", argEx.ParamName, argEx.Message);
                exceptionThrown = true;
            }

            DRT.Assert(exceptionThrown, "Duplicate Object id check should have thrown an exception");

            c.Close();
        }

        public void ExerciseXAdES()
        {
            Console.WriteLine("\n---------------------------------------->");
            Console.WriteLine("  Exercise XAdES");
            Console.WriteLine("---------------------------------------->\n");

            String XAdESObjectId = "AdvancedElectronicSignatureSignedProperties";

            Package c = Package.Open(s_file, FileMode.Open);

            // create a digital signature manager
            PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(c);
            dsm.CertificateOption = CertificateEmbeddingOption.InSignaturePart;

            // register for invalid signature verification event
            dsm.InvalidSignatureEvent += new InvalidSignatureEventHandler(OnInvalidSignature);

            LogTest("Sign using custom id", s_testNum++);
            System.Collections.Generic.List<Uri> toSign = new System.Collections.Generic.List<Uri>();
            int skippedRels = 0;
            foreach (PackagePart p in c.GetParts())
            {
                if (PackUriHelper.IsRelationshipPartUri(p.Uri))
                {
                    skippedRels++;
                }
                else
                {
                    //                    Console.WriteLine("    -> " + p.Uri);
                    toSign.Add(p.Uri);
                }
            }
            if (skippedRels > 0)
                Console.WriteLine("    [skipped {0} Relationship Parts]", skippedRels);


            PackageDigitalSignature sig = CreateXAdESSignature(dsm, toSign, XAdESObjectId);
            VerifyResult verifyResult = sig.Verify();
            DRT.Assert(verifyResult == VerifyResult.Success, "Digital signature verify failed");

            // get the signature object
            Signature signatureObject = sig.Signature;

            // verify that the objects round-tripped
            Console.WriteLine("<Object> tags in XAdES signature:");
            foreach (DataObject obj in signatureObject.ObjectList)
            {
                // check if the object is signed or not
                string type = "unsigned";
                foreach (Reference r in signatureObject.SignedInfo.References)
                {
                    // special casing for XAdES
                    if ((String.CompareOrdinal(r.Uri.Substring(1), obj.Id) == 0) ||
                        (String.CompareOrdinal(obj.Id, "XAdES") == 0))
                    {
                        type = "signed";
                        break;
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("\t{0} status: {1}", obj.Id, type);
                Console.WriteLine(sb.ToString());
            }

            // insert a TimeStamp into the XAdES unsigned area and ensure signature is still valid
            sig.Signature = InsertTimeStamp(signatureObject);
            verifyResult = sig.Verify();
            DRT.Assert(verifyResult == VerifyResult.Success, "Digital signature verify failed after adding TimeStamp");


            // create a second signature and break it
            PackageDigitalSignature sig2 = CreateXAdESSignature(dsm, toSign, XAdESObjectId);
            verifyResult = sig2.Verify();
            DRT.Assert(verifyResult == VerifyResult.Success, "Digital signature verify failed");

            // modify the signed portion
            sig2.Signature = BreakXAdESSignature(sig2.Signature);
            verifyResult = sig2.Verify();
            DRT.Assert(verifyResult != VerifyResult.Success, "Digital signature verify SHOULD HAVE failed");


            // create a signature with a duplicate id in a signed object
            PackageDigitalSignature sig3 = CreateXAdESSignatureWithDuplicateId(dsm, toSign, XAdESObjectId);
            sig3.Signature = AddDuplicateIdToXadESSignature(sig3.Signature, "XAdES", XAdESObjectId);
            try
            {
                verifyResult = sig3.Verify();
            }
            catch (XmlException)
            {
                // we expect XmlException due to duplicate id.  By now, we know the
                // certificate is valid, so use CertificateRequired to signal 'success'
                verifyResult = VerifyResult.CertificateRequired;
            }
            DRT.Assert(verifyResult == VerifyResult.CertificateRequired, "Digital signature verify SHOULD HAVE failed due to duplicate Id");

            // create a signature with a duplicate id in different signed objects
            PackageDigitalSignature sig4 = CreateXAdESSignatureWithIdInDifferentObjects(dsm, toSign, XAdESObjectId);
            sig4.Signature = AddDuplicateIdToXadESSignature(sig4.Signature, "XAdES2", XAdESObjectId);
            try
            {
                verifyResult = sig4.Verify();
            }
            catch (XmlException)
            {
                verifyResult = VerifyResult.CertificateRequired;
            }
            DRT.Assert(verifyResult == VerifyResult.CertificateRequired, "Digital signature verify SHOULD HAVE failed due to Id in different objects");
            c.Close();
        }

        private PackageDigitalSignature CreateXAdESSignature(PackageDigitalSignatureManager dsm, IEnumerable<Uri> toSign, string refId)
        {
            DRT.Assert((new FileInfo(s_australiaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_australiaCertFile + ".  Run CMD file instead of EXE.");
            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 certAus = new X509Certificate2(s_australiaCertFile);
            X509Certificate2 certAfrica = new X509Certificate2(s_africaCertFile);
            DRT.Assert(certAfrica != null, "No usable certificate found on this machine or none selected.");

            LogTest("Create XAdES signature", s_testNum++);
            Console.WriteLine("Create XAdES signature");

            Reference objectReference = new Reference("#" + refId);
            objectReference.Type = "http://uri.etsi.org/01903/v1.2.2#SignedProperties";
            List<Reference> objectReferences = new List<Reference>(4);
            objectReferences.Add(objectReference);

            List<DataObject> signatureObjects = new List<DataObject>(4);
            signatureObjects.Add(GenerateComplexObjectTag("XAdES", refId));
            signatureObjects.Add(GenerateObjectTag("someOtherObject"));

            return dsm.Sign(toSign, certAfrica, null, null, signatureObjects, objectReferences);
        }

        private PackageDigitalSignature CreateXAdESSignatureWithDuplicateId(PackageDigitalSignatureManager dsm, IEnumerable<Uri> toSign, string refId)
        {
            DRT.Assert((new FileInfo(s_australiaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_australiaCertFile + ".  Run CMD file instead of EXE.");
            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 certAus = new X509Certificate2(s_australiaCertFile);
            X509Certificate2 certAfrica = new X509Certificate2(s_africaCertFile);
            DRT.Assert(certAfrica != null, "No usable certificate found on this machine or none selected.");

            LogTest("Create XAdES signature with duplicate id", s_testNum++);
            Console.WriteLine("Create XAdES signature");

            Reference objectReference = new Reference("#" + refId);
            objectReference.Type = "http://uri.etsi.org/01903/v1.2.2#SignedProperties";
            List<Reference> objectReferences = new List<Reference>(4);
            objectReferences.Add(objectReference);

            List<DataObject> signatureObjects = new List<DataObject>(4);
            signatureObjects.Add(GenerateComplexObjectTagWithDuplicateId("XAdES", refId));
            signatureObjects.Add(GenerateObjectTag("someOtherObject"));

            return dsm.Sign(toSign, certAfrica, null, null, signatureObjects, objectReferences);
        }

        private PackageDigitalSignature CreateXAdESSignatureWithIdInDifferentObjects(PackageDigitalSignatureManager dsm, IEnumerable<Uri> toSign, string refId)
        {
            DRT.Assert((new FileInfo(s_australiaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_australiaCertFile + ".  Run CMD file instead of EXE.");
            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 certAus = new X509Certificate2(s_australiaCertFile);
            X509Certificate2 certAfrica = new X509Certificate2(s_africaCertFile);
            DRT.Assert(certAfrica != null, "No usable certificate found on this machine or none selected.");

            LogTest("Create XAdES signature with id in different objects", s_testNum++);
            Console.WriteLine("Create XAdES signature");

            Reference objectReference = new Reference("#" + refId);
            objectReference.Type = "http://uri.etsi.org/01903/v1.2.2#SignedProperties";
            List<Reference> objectReferences = new List<Reference>(4);
            objectReferences.Add(objectReference);

            List<DataObject> signatureObjects = new List<DataObject>(4);
            signatureObjects.Add(GenerateComplexObjectTag("XAdES", refId));
            signatureObjects.Add(GenerateComplexObjectTag("XAdES2", "x" + refId));
            signatureObjects.Add(GenerateObjectTag("someOtherObject"));

            return dsm.Sign(toSign, certAfrica, null, null, signatureObjects, objectReferences);
        }

        private DataObject LocateDataObject(Signature sig, String name)
        {
            DataObject dataObject = null;
            foreach (DataObject obj in sig.ObjectList)
            {
                foreach (Reference r in sig.SignedInfo.References)
                {
                    // locate XAdES
                    if (String.CompareOrdinal(obj.Id, name) == 0)
                    {
                        dataObject = obj;
                        break;
                    }
                }

                if (dataObject != null)
                    break;
            }
            return dataObject;
        }

        private Signature InsertTimeStamp(Signature sig)
        {
            // locate the xades object tag
            DataObject xaedesObject = LocateDataObject(sig, "XAdES");

            // locate the unsigned data
            foreach (XmlNode node in xaedesObject.Data)
            {
                if (node.Name == "QualifyingProperties")
                {
                    foreach (XmlNode innerNode in node)
                    {
                        if (innerNode.Name == "UnsignedProperties")
                        {
                            // add the time stamp
                            innerNode.InnerXml = "<SignatureTimeStamp bogusData=\"10101968\" />";
                            //                    XmlDocument xDoc = new XmlDocumentFragment();
                            //                    XmlNode timeStampNode = xDoc.CreateNode(XmlNodeType.Element, "SignatureTimeStamp", "");
                            //                    node.AppendChild(timeStampNode);
                            break;
                        }
                    }
                    break;
                }
            }

            return sig;
        }

        private Signature BreakXAdESSignature(Signature sig)
        {
            // locate the xades object tag
            DataObject xaedesObject = LocateDataObject(sig, "XAdES");

            // locate the unsigned data
            foreach (XmlNode node in xaedesObject.Data)
            {
                if (node.Name == "QualifyingProperties")
                {
                    foreach (XmlNode innerNode in node)
                    {
                        if (innerNode.Name == "SignedProperties")
                        {
                            // add the time stamp
                            innerNode.InnerText = "signature breaking text";
                            break;
                        }
                    }
                    break;
                }
            }

            return sig;
        }

        private Signature AddDuplicateIdToXadESSignature(Signature sig, string objectName, string idValue)
        {
            string xid = "x" + idValue;
            // locate the given object tag
            DataObject xaedesObject = LocateDataObject(sig, objectName);

            // locate the given id
            foreach (XmlNode node in xaedesObject.Data)
            {
                if (node.Name == "QualifyingProperties")
                {
                    foreach (XmlNode innerNode in node)
                    {
                        XmlElement element = innerNode as XmlElement;
                        if (element != null)
                        {
                            XmlAttribute attr = element.GetAttributeNode("Id");
                            if (attr != null && attr.Value == xid)
                            {
                                // change the ID
                                attr.Value = attr.Value.Replace("x", null);
                                break;
                            }
                        }
                    }
                }
            }

            return sig;
        }

        /// <summary>
        /// XAdES sample
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataObject GenerateComplexObjectTag(string objectId, string nodeId)
        {
            XmlDocument xDoc = new XmlDocument();

            string XAdESNamespace = "http://uri.etsi.org/01903/v1.2.2#";
            XmlNode qualifyingProperties = xDoc.CreateNode(XmlNodeType.Element, "QualifyingProperties",
                XAdESNamespace);

            XmlNode signedProperties = xDoc.CreateNode(XmlNodeType.Element, "SignedProperties", "");
            XmlNode unsignedProperties = xDoc.CreateNode(XmlNodeType.Element, "UnsignedProperties", "");

            XmlAttribute attr = (XmlAttribute)xDoc.CreateNode(XmlNodeType.Attribute, "Id", "");
            attr.Value = nodeId;
            signedProperties.Attributes.Append(attr);

            qualifyingProperties.AppendChild(signedProperties);
            qualifyingProperties.AppendChild(unsignedProperties);

            return GenerateObjectTag(objectId, xDoc, qualifyingProperties);
        }

        private DataObject GenerateComplexObjectTagWithDuplicateId(string objectId, string nodeId)
        {
            XmlDocument xDoc = new XmlDocument();

            string XAdESNamespace = "http://uri.etsi.org/01903/v1.2.2#";
            XmlNode qualifyingProperties = xDoc.CreateNode(XmlNodeType.Element, "QualifyingProperties",
                XAdESNamespace);

            XmlNode signedProperties = xDoc.CreateNode(XmlNodeType.Element, "SignedProperties", "");
            XmlNode unsignedProperties = xDoc.CreateNode(XmlNodeType.Element, "UnsignedProperties", "");

            XmlAttribute attr = (XmlAttribute)xDoc.CreateNode(XmlNodeType.Attribute, "Id", "");
            attr.Value = nodeId;
            signedProperties.Attributes.Append(attr);

            qualifyingProperties.AppendChild(signedProperties);
            qualifyingProperties.AppendChild(unsignedProperties);

            // add a node to receive the duplicate id
            signedProperties = xDoc.CreateNode(XmlNodeType.Element, "DuplicateNode", "");
            attr = (XmlAttribute)xDoc.CreateNode(XmlNodeType.Attribute, "Id", "");
            attr.Value = "x" + nodeId;  // we'll strip the 'x' after signing
            signedProperties.Attributes.Append(attr);
            qualifyingProperties.AppendChild(signedProperties);

            return GenerateObjectTag(objectId, xDoc, qualifyingProperties);
        }

        // generate a simple object tag
        private DataObject GenerateObjectTag(string id)
        {
            XmlDocument xDoc = new XmlDocument();
            return GenerateObjectTag(id, xDoc, xDoc.CreateNode(XmlNodeType.Element, "root", "namespace")); // dummy root
        }

        // for more complex scenarios
        private DataObject GenerateObjectTag(string id, XmlDocument xDoc, XmlNode contents)
        {
            xDoc.AppendChild(xDoc.CreateNode(XmlNodeType.Element, "root", "namespace")); // dummy root
            xDoc.DocumentElement.AppendChild(contents);

            DataObject dataObject = new DataObject();
            dataObject.Data = xDoc.DocumentElement.ChildNodes;
            dataObject.Id = id;

            return dataObject;
        }

        /// <summary>
        /// Here's where we ensure that things break like expected
        /// </summary>
        public void RelationshipSigningTest()
        {
            Console.WriteLine("\n --- RELATIONSHIP SIGNING TESTS ----------------------------------");

            X509Certificate2 australiaCert = new X509Certificate2(s_australiaCertFile);
            VerifyResult result;
            using (Package c = Package.Open(s_file, FileMode.Open))
            {
                // create a digital signature manager
                PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(c);
                dsm.InvalidSignatureEvent += new InvalidSignatureEventHandler(OnInvalidSignature);

                // add relationships between parts
                PackagePart niePart = c.GetPart(_partUri[0]);
                PackagePart booBooPart = c.GetPart(_partUri[1]);

                // add relationship
                PackageRelationship rel1 = niePart.CreateRelationship(_partUri[4], TargetMode.Internal, "a picture i need to render");          // image 2
                PackageRelationship rel2 = niePart.CreateRelationship(booBooPart.Uri, TargetMode.Internal, "just friends");
                PackageRelationship rel3 = niePart.CreateRelationship(_partUri[5], TargetMode.Internal, "a picture i need to render");          // image 3

                DumpRelationships(niePart.GetRelationships());

                #region SignByID

                // sign the single relationship
                List<PackageRelationship> relationshipList = new List<PackageRelationship>();
                relationshipList.Add(rel2);
                PackageDigitalSignature relSig = dsm.Sign(null, australiaCert, GetRelationshipSelectors(relationshipList));

                LogTest("Verify single Relationship signature", s_testNum++);
                result = relSig.Verify();
                DRT.Assert(result == VerifyResult.Success, "Sign relationship failed");

                // remove a different relationship
                LogTest("Remove different Relationship and re-verify", s_testNum++);
                niePart.DeleteRelationship(rel1.Id);
                result = relSig.Verify();
                DRT.Assert(result == VerifyResult.Success, "Verify relationship signature failed");

                DumpRelationships(niePart.GetRelationships());

                // remove the signed relationship to ensure signature fails
                LogTest("Remove signed Relationship and re-verify to ensure negative result", s_testNum++);
                niePart.DeleteRelationship(rel2.Id);
                result = relSig.Verify();
                DRT.Assert(result == VerifyResult.InvalidSignature,
                    "Verify relationship signature failed - result should have been Invalid");

                DumpRelationships(niePart.GetRelationships());
                dsm.RemoveSignature(relSig.SignaturePart.Uri);
                relSig = null;

                #endregion SingByID

                #region SignByType

                // re-create relationship
                rel1 = niePart.CreateRelationship(_partUri[4], TargetMode.External, "a picture i need to render");          // image 2
                rel2 = niePart.CreateRelationship(booBooPart.Uri, TargetMode.Internal, "just friends");


                DumpRelationships(niePart.GetRelationships());

                // sign the relationship based on type
                List<PackageRelationshipSelector> relationshipSelectorList = new List<PackageRelationshipSelector>();
                relationshipSelectorList.Add(new PackageRelationshipSelector(niePart.Uri, PackageRelationshipSelectorType.Type, "a picture i need to render"));
                relationshipSelectorList.Add(new PackageRelationshipSelector(niePart.Uri, PackageRelationshipSelectorType.Type, "a family picture"));
                relSig = dsm.Sign(null, australiaCert, relationshipSelectorList);

                LogTest("Verify Relationship Signed(type) - signature", s_testNum++);
                result = relSig.Verify();
                DRT.Assert(result == VerifyResult.Success, "Sign relationship(type) - failed");

                // remove a different relationship
                LogTest("Remove different Relationship(type) and re-verify", s_testNum++);
                niePart.DeleteRelationship(rel2.Id);
                result = relSig.Verify();
                DRT.Assert(result == VerifyResult.Success, "Verify relationship(type) signature failed");

                DumpRelationships(niePart.GetRelationships());

                // Add a different type of relationship
                LogTest("Add different Relationship(type) and re-verify", s_testNum++);
                niePart.CreateRelationship(_partUri[3], TargetMode.Internal, "just friends");          // new rel of different type
                result = relSig.Verify();
                DRT.Assert(result == VerifyResult.Success, "Verify relationship(type) signature failed");

                DumpRelationships(niePart.GetRelationships());

                // remove the signed relationship to ensure signature fails
                LogTest("Remove signed Relationship(type) and re-verify to ensure negative result", s_testNum++);
                String idRel1 = rel1.Id;
                niePart.DeleteRelationship(rel1.Id);
                result = relSig.Verify();
                DRT.Assert(result == VerifyResult.InvalidSignature,
                    "Verify relationship(type) signature failed - result should have been Invalid");

                DumpRelationships(niePart.GetRelationships());

                // Add a relationship of type signed earlier to ensure signature fails
                LogTest("Add a new Relationship of type signed earlier and re-verify to ensure negative result", s_testNum++);
                rel1 = niePart.CreateRelationship(_partUri[4], TargetMode.External, "a picture i need to render", idRel1);          // restore to initial state - image 2
                PackageRelationship rel4 = niePart.CreateRelationship(_partUri[4], TargetMode.Internal, "a family picture");          // new rel of signed type
                result = relSig.Verify();
                DRT.Assert(result == VerifyResult.InvalidSignature,
                    "Verify relationship(type) signature failed - result should have been Invalid");

                DumpRelationships(niePart.GetRelationships());

                dsm.RemoveSignature(relSig.SignaturePart.Uri);
                relSig = null;


                #endregion SignByType
            }
        }

        /// <summary>
        /// Here's where we verify certificate sharing works
        /// </summary>
        public void CertificateSharingTests()
        {
            Console.WriteLine("\n --- CERTIFICATE SHARING TESTS -----------------------------------");

            X509Certificate2 asiaCert = new X509Certificate2(s_asiaCertFile);
            VerifyResult result;
            using (Package p = Package.Open(s_file, FileMode.Open))
            {
                // create a digital signature manager
                PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(p);
                dsm.InvalidSignatureEvent += new InvalidSignatureEventHandler(OnInvalidSignature);

                // sign the image parts
                List<Uri> partList = new List<Uri>();
                partList.Add(_partUri[4]);
                partList.Add(_partUri[5]);
                PackageDigitalSignature s = dsm.Sign(partList, asiaCert, null);

                // sign nie
                partList.Clear();
                partList.Add(_partUri[0]);
                PackageDigitalSignature s2 = dsm.Sign(partList, asiaCert, null);

                LogTest("Verify Signatures that share the Asia cert", s_testNum++);
                result = dsm.VerifySignatures(false);
                DRT.Assert(result == VerifyResult.Success, "Verify failed");

                // list certificates
                List<Uri> certList = GetCertificates(p);
                DumpCertificates(p, certList);
                int certCount = certList.Count;

                // remove a signature
                Console.WriteLine("Remove first signature - count should be identical");
                dsm.RemoveSignature(s.SignaturePart.Uri);
                certList = GetCertificates(p);
                DumpCertificates(p, certList);
                DRT.Assert(certList.Count == certCount, "Removing a signature should not have dropped the cert count because it is shared");

                // remove second sig
                Console.WriteLine("Remove 2nd signature - count should be dropped because Asia cert is no longer being used");
                dsm.RemoveSignature(s2.SignaturePart.Uri);
                certList = GetCertificates(p);
                DumpCertificates(p, certList);
                DRT.Assert(certList.Count == certCount - 1,
                    "Removing a signature should have dropped the cert count because no signature now uses Asia cert");
            }
        }

        void LogTest(String s, int _testNum)
        {
            Console.WriteLine("\n{0}. {1}", _testNum, s);
        }

        bool VerifyPartRemovalBreaksSignature(Package p, PackageDigitalSignatureManager dsm, PackageDigitalSignature ds, Uri uri)
        {
            bool success = false;
            p.DeletePart(uri);
            VerifyResult verifyResult = ds.Verify();
            DRT.Assert(success = (verifyResult == VerifyResult.ReferenceNotFound),
                "Test Failed: Expecting ReferenceNotFound but got this: " + verifyResult);

            return success;
        }

        static public void BadHandler(object source, SignatureVerificationEventArgs e)
        {
            Console.WriteLine("\nBad delegate handler - throw unique exception");
            throw new InvalidOperationException("BogusException");
        }

        public void VerifyHardenedDelegate()
        {
            Console.WriteLine("\n---------------------------------------->");
            Console.WriteLine("  Verify Delegate Robustness with malicious handler");
            Console.WriteLine("---------------------------------------->\n");

            DRT.Assert((new FileInfo(s_australiaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_australiaCertFile + ".  Run CMD file instead of EXE.");
            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 certAus = new X509Certificate2(s_australiaCertFile);
            X509Certificate2 certAfrica = new X509Certificate2(s_africaCertFile);
            DRT.Assert(certAfrica != null, "No usable certificate found on this machine or none selected.");

            Package c = Package.Open(s_file, FileMode.Open);

            // create a digital signature manager
            PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(c);
            dsm.CertificateOption = CertificateEmbeddingOption.InSignaturePart;

            // register for invalid signature verification event
            dsm.InvalidSignatureEvent += new InvalidSignatureEventHandler(BadHandler);

            try
            {
                // create a digital signature manager
                dsm.CertificateOption = CertificateEmbeddingOption.InSignaturePart;

                LogTest("Sign once", s_testNum++);
                System.Collections.Generic.List<Uri> toSign = new System.Collections.Generic.List<Uri>();
                PackagePart partToDelete = null;
                foreach (PackagePart p in c.GetParts())
                {
                    if (!PackUriHelper.IsRelationshipPartUri(p.Uri))
                    {
                        toSign.Add(p.Uri);

                        // don't delete any signatures
                        if (partToDelete == null && (!p.ContentType.StartsWith("application/vnd.ms-package.digital-signature")))
                            partToDelete = p;
                    }
                }

                // break the signature to trigger the delegate
                if (partToDelete != null)
                    c.DeletePart(partToDelete.Uri);

                LogTest("Verify - catching and eating the InvalidOperationException", s_testNum++);
                for (int j = 0; j < 2; j++)
                {
                    try
                    {
                        VerifyResult verifyResult = dsm.VerifySignatures(false);
                    }
                    catch (InvalidOperationException ioe)
                    {
                        if (ioe.Message == "BogusException")
                            Console.WriteLine("Verify attempt: " + j.ToString() + ". Eating expected exception: InvalidOperationException: " + ioe.Message);
                        else
                        {
                            Console.WriteLine("Unexpected exception: " + ioe.ToString());
                            throw ioe;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught: " + e.Message);
                throw e;
            }

            LogTest("Verify - DSM is still usable and not damaged by the exceptions", s_testNum++);
            DisplaySignatures(dsm, false);


            c.Close();
        }

        /// <summary>
        /// Exercises mapping from PackageDigitalSignatureManager.HashAlgorithm to SignedXML.SignedInfo.SignatureMethod.
        /// These should be appropriately matched strength levels given an RSA key.
        /// </summary>
        public void ExerciseSignatureMethodMapping()
        {
            Dictionary<string, string> sigMethodLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "http://www.w3.org/2001/04/xmlenc#sha256", "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" },
                { "http://www.w3.org/2001/04/xmldsig-more#sha384", "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384" },
                { "http://www.w3.org/2001/04/xmlenc#sha512", "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512" },
            };

            // Re-create package so we don't have modified data from prior runs
            CreatePackage(s_file);

            Console.WriteLine("\n---------------------------------------->");
            Console.WriteLine("  Verify SignatureMethod mapping from HashAlgorithm");
            Console.WriteLine("---------------------------------------->\n");

            DRT.Assert((new FileInfo(s_africaCertFile)).Exists, "Configuration error - could not find certificate file: " + s_africaCertFile + ".  Run CMD file instead of EXE.");
            X509Certificate2 certAfrica = new X509Certificate2(s_africaCertFile);
            DRT.Assert(certAfrica != null, "No usable certificate found on this machine or none selected.");

            using (Package p = Package.Open(s_file, FileMode.Open))
            {
                // create a digital signature manager
                PackageDigitalSignatureManager dsm = new PackageDigitalSignatureManager(p);

                // Create a list of all the part URIs in the package to sign
                // (GetParts() also includes PackageRelationship parts).
                List<Uri> toSign = new List<Uri>();

                foreach (PackagePart packagePart in p.GetParts())
                {
                    // Add all package parts to the list for signing.
                    toSign.Add(packagePart.Uri);
                }

                // Add the URI for SignatureOrigin PackageRelationship part.
                // The SignatureOrigin relationship is created when Sign() is called.
                // Signing the SignatureOrigin relationship disables counter-signatures.
                toSign.Add(PackUriHelper.GetRelationshipPartUri(dsm.SignatureOrigin));

                // Also sign the SignatureOrigin part.
                toSign.Add(dsm.SignatureOrigin);

                // Add the package relationship to the signature origin to be signed.
                toSign.Add(PackUriHelper.GetRelationshipPartUri(new Uri("/", UriKind.RelativeOrAbsolute)));

                try
                {
                    foreach (var useMatching in new bool[] { false, true })
                    {
                        // Now attempt it with mapping turned off via compat flag
                        SetMatchPackageSignatureMethodToPackagePartDigestMethod(useMatching);

                        foreach (string hashAlgorithm in sigMethodLookup.Keys)
                        {
                            dsm.HashAlgorithm = hashAlgorithm;

                            dsm.Sign(toSign, certAfrica);

                            var result = dsm.VerifySignatures(false);
                            DRT.Assert(result == VerifyResult.Success, "Verify failed");

                            // If matching is off we're always SHA256, otherwise use the table
                            string selectedSignatureMethod = (useMatching) ? sigMethodLookup[hashAlgorithm] : GetSignedXmlDsigRSADefault();

                            foreach (var sig in dsm.Signatures)
                            {
                                DRT.Assert(sig.Signature.SignedInfo.SignatureMethod == selectedSignatureMethod,
                                    string.Format("HashAlgorithm: {0} Expected SignatureMethod {1} Actual SignatureMethod {2}",
                                    hashAlgorithm, selectedSignatureMethod, sig.Signature.SignedInfo.SignatureMethod));
                            }

                            dsm.RemoveAllSignatures();
                        }
                    }
                }
                finally
                {
                    // Just in case set this back to match before we exit
                    SetMatchPackageSignatureMethodToPackagePartDigestMethod(true);

                    // Clear any signatures to reset doc
                    dsm.RemoveAllSignatures();
                }
            }
        }

        private void SetMatchPackageSignatureMethodToPackagePartDigestMethod(bool value)
        {
            var compatProp = typeof(System.Windows.BaseCompatibilityPreferences)
                .GetProperty("MatchPackageSignatureMethodToPackagePartDigestMethod",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            compatProp?.SetValue(null, value);
        }

        private string GetSignedXmlDsigRSADefault()
        {
            var defaultField = typeof(System.Security.Cryptography.Xml.SignedXml)
                 .GetField("XmlDsigRSADefault",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            return defaultField?.GetValue(null) as string ?? "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        }

#if false
        public DateTime ParseSignatureProperties(Stream s)
        {
           return XmlSignatureProperties.ParseSigningTime(new XmlTextReader(s), 0);
        }

        public void ExerciseSignatureProperties(DateTime persistedTime, String dateTimeFormat)
        {
            //----------------------------------------
            // generate legal SignatureProperties tags
            //----------------------------------------
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, null);
            XmlSignatureProperties.GenerateSignatureProperties(writer, persistedTime, dateTimeFormat);
            writer.Flush();

            ms.Seek(0, SeekOrigin.Begin);

            //----------------------------------------
            // exercise parser
            //----------------------------------------
            XmlTextReader r = new XmlTextReader(ms);
            r.Read();   // prime the pump
            DateTime parsedTime = ParseSignatureProperties(ms);

            // comparing times is inexact because full fidelity is not maintained (ms is not encoded)
            TimeSpan diff = parsedTime - persistedTime;
            DRT.Assert(diff.Ticks < TimeSpan.TicksPerSecond, "Persisted time != parsed time");
        }
#endif
        private void DumpTimeFormat(PackageDigitalSignature sig, String expectedFormat)
        {
            Console.WriteLine("\n    Part: {0}\n    --> SigningTime: {1}\n    --> TimeFormat: {2}",
                sig.SignaturePart.Uri.ToString(),
                sig.SigningTime, sig.TimeFormat);
            DRT.Assert(sig.TimeFormat == expectedFormat,
                "TimeFormat should have been: " + expectedFormat);
        }

        private void DumpCertificates(Package p, List<Uri> certParts)
        {
            // list certificates
            foreach (Uri part in certParts)
            {
                using (Stream s = p.GetPart(part).GetStream())
                {
                    DRT.Assert(s.Length > 0, "Empty Cert Part found: " + part);
                    Byte[] byteArray = new Byte[s.Length];
#pragma warning disable CA2022 // Avoid inexact read
                    s.Read(byteArray, 0, (int)s.Length);
#pragma warning restore CA2022
                    DumpCertificate(new X509Certificate2(byteArray));
                }
            }
        }

        private void DumpCertificate(X509Certificate2 cert)
        {
            if (cert == null)
            {
                Console.WriteLine("<no certificate>");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("\nCertificate:\n\tSerial: {0}\n\tPrincipal: {1}\n",
                    cert.GetSerialNumberString(), cert.GetName());
                Console.WriteLine(sb.ToString());
            }
        }

        private List<Uri> GetCertificates(Package p)
        {
            // list certificates
            List<Uri> certPartList = new List<Uri>();
            foreach (PackagePart part in p.GetParts())
            {
                // look for certificates
                if (String.Compare(part.ContentType, "application/vnd.openxmlformats-package.digital-signature-certificate", StringComparison.InvariantCulture) == 0)
                    certPartList.Add(part.Uri);
            }

            return certPartList;
        }

        static private void DumpXransformList(List<String> xFormList, Uri partName)
        {
            Console.WriteLine("\n  {0} Xforms found for Part {1}", xFormList.Count, partName.ToString());
            foreach (String xFormName in xFormList)
                Console.WriteLine("     --> [{0}]", xFormName);
        }

        private void DumpRelationships(IEnumerable<PackageRelationship> relationships)
        {
            Console.WriteLine("\nRelationships:");
            foreach (PackageRelationship r in relationships)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("--> id: {0}\n  source: {1}\n  target: {2}",
                    r.Id,
                    r.SourceUri.ToString(),
                    r.TargetUri.ToString());
                Console.WriteLine(sb.ToString());
            }
        }

        private void DumpParts(IEnumerable<Uri> parts, Package p)
        {
            Console.WriteLine("\nParts:");
            foreach (Uri uri in parts)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("--> name: {0} contentType: {1}", uri.ToString(), p.GetPart(uri).ContentType);
                Console.WriteLine(sb.ToString());
            }
        }

        //        const string _packageFileName = "DrtDigSigXmlTests.zip  ";

        private void CreatePackage(String packageFileName)
        {
            Package package = Package.Open(packageFileName, FileMode.Create);

            int i = 0;
            byte[] buf = new byte[1024];
            for (i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)i;
            }

            for (i = 0; i < _partUri.Length; i++)
            {
                PackagePart p = package.CreatePart(_partUri[i], _partContentType[i]);
            }

            // baabaa
            Stream s = package.GetPart(_partUri[0]).GetStream();
            s.Write(buf, 0, buf.Length);
            s.Close();

            // booboo
            int bytesRead = 0;
            s = package.GetPart(_partUri[1]).GetStream();
            FileStream fs = new FileStream(s_booBooFile, FileMode.Open);
            while ((bytesRead = fs.Read(buf, 0, buf.Length)) > 0)
                s.Write(buf, 0, bytesRead);
            s.Close(); fs.Close();

            // part1
            s = package.GetPart(_partUri[2]).GetStream();
            fs = new FileStream(s_part1File, FileMode.Open);
            while ((bytesRead = fs.Read(buf, 0, buf.Length)) > 0)
                s.Write(buf, 0, bytesRead);
            s.Close(); fs.Close();

            // image1
            for (i = 3; i < _partUri.Length; i++)
            {
                s = package.GetPart(_partUri[i]).GetStream();
                fs = new FileStream(s_image1File, FileMode.Open);
                while ((bytesRead = fs.Read(buf, 0, buf.Length)) > 0)
                    s.Write(buf, 0, bytesRead);
                s.Close(); fs.Close();
            }

            package.Close();
        }

        private static Uri CreatePartUri(string partUri)
        {
            return PackUriHelper.CreatePartUri(new Uri(partUri, UriKind.Relative));
        }

        private List<PackageRelationshipSelector> GetRelationshipSelectors(List<PackageRelationship> relationships)
        {
            List<PackageRelationshipSelector> relationshipSelectors = new List<PackageRelationshipSelector>();
            Uri sourceUri;

            foreach (PackageRelationship relationship in relationships)
            {
                sourceUri = relationship.SourceUri;
                relationshipSelectors.Add(new PackageRelationshipSelector(sourceUri, PackageRelationshipSelectorType.Id, relationship.Id));
            }
            return relationshipSelectors;
        }

        /// <summary>
        /// Exercise the Xml machinery - unit tests for XmlDSig
        /// </summary>
        public void XmlTests()
        {
            //            const String xmlFileName = @".\DrtDigSig_xmlTests.xml";
            /*
                        //----------------------------------------
                        // exercise SignatureProperties with legal input
                        //----------------------------------------
                        DateTime persistedTime = DateTime.Now;
                        ExerciseSignatureProperties(persistedTime, null);              // default

                        // try all legal forms
                        for (int i = 0; i < _dateTimePatternMap.GetLength(0); i++)
                        {
                            persistedTime = DateTime.Now;
                            ExerciseSignatureProperties(persistedTime, _dateTimePatternMap[i, 0]);  // pass in Xml format string
                        }

                        //----------------------------------------
                        // exercise XmlSignatureManifest generator
                        //----------------------------------------
                        Package package = CreatePackage();

                        PackageDigitalSignatureManager manager = new PackageDigitalSignatureManager(package);
                        FileStream s = new FileStream("DigSigManifestXml.xml", FileMode.Create);
            //            MemoryStream s = new MemoryStream();
                        XmlTextWriter writer = new XmlTextWriter(s, null);
                        XmlSignatureManifest manifest = new XmlSignatureManifest(manager, writer);
                        SHA1Managed hashAlgorithm = new SHA1Managed();
                        List<Uri> parts = new List<Uri>(partUri);

                        // generate away
                        manifest.GenerateManifest(hashAlgorithm, parts, null);
            */
        }

    }
}
