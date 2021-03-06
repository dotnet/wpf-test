// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Packaging;
using MS.Utility;
using System.Collections;                   // for IEnumerator
using System.Collections.Generic;
using System.Text;                          // for StringBuilder
using System.Xml;
using System.Reflection;                    // to access internal classes


namespace MS.Internal.WppDrt.ContainerServices
{
    /// <summary>
    /// MMCF API DRT class, all tests live as methods within this class.
    /// </summary>
    class MMCFTests
    {
        #region Member Variables 

        private static SpewWriter Log = new SpewWriter();

        const int numberOfParts = 8;
        const int numberOfInvalidPartNames = 16;

        static string[] partNames = new string[numberOfParts] { 
            "frodo.xaml", 
            "SAM.xaml", 
            @"%06merry.xaMl", 
            "/my%20images/hobbits.jpg", 
            "/my%20images/elves.jpg", 
            "/imaGES/лЛpic.jpg", 
            "/my%20images/gandalf.jpg", 
            "/my%20images/lordoftheringsposterforthereturnofthekingmovie.jpg"
            };

        static string[] compressedPartNames = new string[numberOfParts] {
            "compressed/frodo.xaml",
            "compressed/sam.xaml",
            "compressed/merry.xaml", 
            "compressed/my%20images/hobbits.jpg", 
            "compressed/my%20images/elves.jpg",
            "compressed/files/dwarves.xaml",
            "compressed/my%20images/gandalf.jpg",
            "compressed/my%20images/lordoftheringsposterforthereturnofthekingmovie.jpg"
            };

        static string[] contentTypes = new string[numberOfParts] { 
            "text/xml", 
            "tExt/xml", 
            "text/xml", 
            "image/jpeg", 
            "image/jpeg", 
            "text/xml", 
            "image/jpeg", 
            "image/jpeg" 
        };

        static string[] invalidPartNames = new string[numberOfInvalidPartNames] {
            "/asdasda/fin%31al.xaml",
            "/asdasda/[fin%30al].xaml",
            "/asdasda//fin%30al.xaml",
            "../../my%20xam l.asda%2F./fin%30al.xaml",
            "../../my%20xaml.asda%2F../",
            "/my%30xaml.asda",
            "/my%30xa#ml.asda",
            "/my%30xa?ml.asda",
            "/my%20xaml.asda%2F../", 
            "/asdasd/../..", 
            "/../",
            "//",
            "//www.abc.com/text.xaml",
            " /",
            "/",
            "",
        };

      
        static PackagePart[] parts = new PackagePart[numberOfParts];
        static Uri[] partUris = new Uri[numberOfParts];
   
        static PackagePart[] compressedParts = new PackagePart[numberOfParts];
        static Uri[] compressedPartUris = new Uri[numberOfParts];
        
        const String fileZip = "DrtPackagingApi.container";
        const string logFile = "DrtPackagingApi.log";

        //PackageRelationship Variables
// never used        internal static readonly string _segmentName                = "_rels";
// never used       internal static readonly string _extensionName              = ".rels";

        #endregion Member Variables  

        #region Main Method
        /// <summary>
        /// The main DRT test method that will run series of tests on Packaging APIs
        /// </summary>
        /// <returns>0 if all are successful, 1 if there is at least one failure</returns>
        [STAThread]
        public static int Main(string[] args)
        {
            int result = 0;

            if (args.Length > 0 && args[0] == "/verbose")
                Log.Verbose = true;

            //Check if log file present, and delete it
            FileInfo logFileInfo = new FileInfo(logFile);
            if(logFileInfo.Exists)
                logFileInfo.Delete();

            Log.WriteLine();
            Log.WriteLine();

            Log.Banner("PackagingAPIs", "Microsoft");

            Log.WriteLine();  // Do two linefeeds between tests
            Log.WriteLine();

            try
            {
                // test by Adaptor type
                Log.AlwaysWriteLine("Starting Zip Archive tests");
                result += APITests(fileZip);
                
                // Adaptor independent tests
                Log.AlwaysWriteLine("Starting Adaptor Independent tests");
                result += UriTests();
                result += UriCompareTests();

                result += ContentTypeTests();

                // SparseMemoryStream and other internal classes
                result += UtilityTests();
            }
            catch (Exception e)
            {
            if(Log.Verbose)
            {
                Log.WriteLine(">>> Packaging API test failed with exception: \n");
                Log.WriteLine(e.Message); Log.WriteLine(">>> Packaing API test failed with exception: \n {0} \n", e.Message);
                Log.WriteLine("stack:\n{0}\n", e.StackTrace);
            }
            else
            {
                Console.SetError(new StreamWriter(logFile));
                Console.Error.WriteLine(" DRTPackagingApis.exe [owner - Microsoft ]\n");
                Console.Error.WriteLine(">>> Packaging API test failed with exception: \n");
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine("stack:\n{0}\n", e.StackTrace);
                Console.Error.Close();
            }	
                result++;
            }

            Log.WriteLine();

            if( result != 0 )
            {
                Log.AlwaysWriteLine("FAILED.");
                result = 1;
            }
            else
            {
                Log.AlwaysWriteLine("Passed.");
            }

            Log.WriteLine();
            Log.WriteLine();

            return result;
        }

        #endregion Main Method     

        #region Adaptor Specific Tests

        static private int APITests(string fileName)
        {
            int result = 0;
           
            Log.WriteLine("Testing Packaging APIs ");
            Log.WriteLine();  // Do a linefeed between tests

            //Delete Previous container file if it exits
            FileInfo fileInfo = new FileInfo(fileName);
            if(fileInfo.Exists)
               fileInfo.Delete();

            result += ContainerFileTests(fileName);
            result += ContainerStreamTests(fileName);              

            return result;
        }
        
        static private int ContainerFileTests(string fileName)
        {
            int result = 0;
         
            #region Initialize

            //creating URIs for all the part Names.
            for (int i = 0; i < numberOfParts; i++)
            {
                partUris[i] = CreatePartUri(partNames[i]);
                compressedPartUris[i] = CreatePartUri(compressedPartNames[i]);
            }

            // create a new container
            Package c = Package.Open(fileName, FileMode.Create);

            #endregion Initialize

            #region Properties

            Log.WriteLine();
            Log.WriteLine("Package Open Access - " + c.FileOpenAccess);
            Log.WriteLine();
            #endregion Properties

            #region Add Method

            for (int i = 0; i < invalidPartNames.Length; i++)
            {
                try
                {
                    result++;
                    c.CreatePart(new Uri(invalidPartNames[i], UriKind.Relative), contentTypes[0]);                   
                }
                catch (ArgumentException)
                {
                    result--;
                    //expected exception - Invalid part name 
                }                
            }
            
            for (int i = 0; i < partNames.Length; i++)
            {
                parts[i] = c.CreatePart(partUris[i], contentTypes[i]);
                Log.WriteLine("Adding PackagePart - \t" + partNames[i] + " with content type - " + contentTypes[i]);

                compressedParts[i] = c.CreatePart(compressedPartUris[i], contentTypes[i], CompressionOption.Normal);
                Log.WriteLine("Adding Part - \t" + compressedPartNames[i] + " with content type - " + contentTypes[i]);
            }
             
            c.Flush();

            Log.WriteLine();  // Do a linefeed between tests
            try
            {
                // Adding a part that already exists, this is expected failure
                result++;
                c.CreatePart(PackUriHelper.CreatePartUri(new Uri("/imaGES/лЛpic.jpg", UriKind.RelativeOrAbsolute)), contentTypes[0]);
            }
            catch (InvalidOperationException)
            {
                // Expected failure
                Log.WriteLine("Expected Exception - Adding a part that already exists");
                result--;
            }
            Log.WriteLine();  // Do a linefeed between tests

            try
            {
                // Adding a part that is a prefix of part that already exists, this is expected failure
                result++;
                c.CreatePart(PackUriHelper.CreatePartUri(new Uri("/FRODO.XAML/fail.jpg", UriKind.RelativeOrAbsolute)), contentTypes[0]);
            }
            catch (InvalidOperationException)
            {
                // Expected failure
                Log.WriteLine("Expected Exception - Adding a part that has a prefix matching");
                result--;
            }

            try
            {
                // Adding a part that is a prefix of part that already exists, this is expected failure
                result++;
                c.CreatePart(PackUriHelper.CreatePartUri(new Uri("/my%20images", UriKind.RelativeOrAbsolute)), contentTypes[0]);
            }
            catch (InvalidOperationException)
            {
                // Expected failure
                Log.WriteLine("Expected Exception - Adding a part that has a prefix matching");
                result--;
            }

            Log.WriteLine();  // Do a linefeed between tests
                                  
            try
            {
                // Adding a null part, this is expected failure
                result++;
                c.CreatePart(null, contentTypes[0]);
            }
            catch (ArgumentException)
            {
                // Expected failure
                Log.WriteLine("Expected Exception - Adding a null part");
                result--;
            }
            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine();  // Do a linefeed between tests

            #endregion Add Method

            #region Get Method


            for (int i = 0; i < partNames.Length; i++)
            {
                parts[i] = c.GetPart(partUris[i]);
                Log.WriteLine("Getting PackagePart - " + parts[i].Uri + " with content type - " + parts[i].ContentType);

                compressedParts[i] = c.GetPart(compressedPartUris[i]);
                Log.WriteLine("Getting Compressed Part - " + compressedParts[i].Uri + " with content type - " + compressedParts[i].ContentType);
            }

            Log.WriteLine();  // Do a linefeed between tests
            try
            {
                // Get a part that does not exist, this is expected failure
                result++;
                c.GetPart(CreatePartUri("/part/does/not/exist.xml")); 
            }
            catch (InvalidOperationException)
            {
                // Expected failure
                Log.WriteLine("Expected Exception - Getting a part that does not exists");
                result--;
            }
            
            Log.WriteLine();  // Do a linefeed between tests

            //Getting the same part again to make sure we get the same object
            PackagePart p = c.GetPart(partUris[0]);

            Log.WriteLine("Getting PackagePart Again- " + p.Uri + " with content type - " + p.ContentType);
            
            if (p == parts[0])
            {
                Log.WriteLine("Same Reference got returned");
            }
            else
            {
                Log.AlwaysWriteLine("Failed: Different Reference got returned");
                result++;
            }

            Log.WriteLine();  // Do a linefeed between tests
            
            try
            {
                // Getting a null part, this is expected failure
                result++;
                c.GetPart(null);
            }
            catch (ArgumentException)
            {
                // Expected failure
                Log.WriteLine("Expected Exception - Getting a null part");
                result--;
            }

            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine();  // Do a linefeed between tests

            #endregion Get Method

            #region Stream Operations

            foreach (PackagePart currentPart in parts)
            {
                Stream s = currentPart.GetStream(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(s); // Frameworks class

                writer.WriteLine("Writing to the stream for the first time.");
                writer.Close(); //closes the stream also.
                s = currentPart.GetStream(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                writer = new StreamWriter(s); // Frameworks class
                writer.WriteLine("Writing to the stream the second time.");
                writer.Close(); //closes the stream also.
            }

            #endregion StreamOperations

            #region Compressed Stream Operations
            
            int iterations = 1; // 0x10000;
            foreach (PackagePart currentPart in compressedParts)
            {
                Stream s = currentPart.GetStream(FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(s); // Frameworks class

                for (int k = 0; k < iterations; k++)
                {
                    writer.WriteLine("Writing to the stream for the first time.");
                }
                iterations = 1;     // only do this for the first part (for expediency)
                writer.Close(); //closes the stream also.
               
                // read from it
                s = currentPart.GetStream(FileMode.Open, FileAccess.ReadWrite);

                StreamReader reader = new StreamReader(s);
                string sentence = reader.ReadLine();
                if (sentence.CompareTo("Writing to the stream for the first time.") != 0)
                {
                    Log.WriteLine("FAIL: Data corruption adding and retrieving compressed data");
                    result++;
                }

                // ReadByte just for completeness
                s.ReadByte();
                s.Close();

            }

            c.Flush();
                     
            #endregion Compressed Stream Operations

            #region PackageRelationship Operations

            result += RelationshipTests(c);

            #endregion 
            
            #region Delete Method

            c.DeletePart(partUris[6]);
            Log.WriteLine("Deleted part - " + partNames[6]);
            c.DeletePart(CreatePartUri("/part/does/not/exist.xml"));
            Log.WriteLine("Deleted part - /part/does/not/exist.xml ");

            //Rels and deleting a rels part
            //PackageRelationshipCollection rels = c.GetRelationships();
            c.CreateRelationship(partUris[6], TargetMode.Internal, "http://generalrels1/");
            c.DeletePart(CreatePartUri("/_rels/.rels"));
            c.CreateRelationship(partUris[6], TargetMode.Internal, "http://generalrels2/");
            c.Flush();

            parts[1].CreateRelationship(partUris[6], TargetMode.Internal, "http://generalrels1/");
            c.DeletePart(CreatePartUri("/_rels/sam.xaml.rels"));
            parts[1].CreateRelationship(partUris[6], TargetMode.Internal, "http://generalrels2/");
            c.Flush();

            #endregion Delete Method

            #region PartExists Method

            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine("Check if a part exists");
            Log.WriteLine();  // Do a linefeed between tests

            for (int i = 0; i < partUris.Length; i++)
            {
                if (c.PartExists(partUris[i]))
                    Log.WriteLine("PackagePart - " + partUris[i] + " exists");
            }

            Log.WriteLine();  // Do a linefeed between tests
            if (c.PartExists(partUris[6]))
            {
                Log.AlwaysWriteLine("Failed: Package.PartExists returns true for non-existent part - " + partNames[6]);
                result++;
            }
            else
                Log.WriteLine("Package.PartExists returns false for non-existent part- " + partNames[6]);

            #endregion PartExists Method

            c.Flush();
            c.Close();

            #region Check Dispose

            try
            {
                result++;
                Log.WriteLine("Package last write time - " + c.GetPart(partUris[0]));                
            }
            catch (ObjectDisposedException)
            {
                // Expected Exception
                Log.WriteLine("Expected Exception - trying to access disposed object");
                result--;
            }

            Log.WriteLine();
            Log.WriteLine("Trying to access a PackagePart object after the parent container is closed");
            

            try
            {
                result++;
                Package testPackage = parts[3].Package;
            }
            catch (InvalidOperationException)
            {
                //Expected Exception
                result--;
                Log.WriteLine("Expected Exception - Parent container for this part is closed");
            }

            try
            {
                result++;
                parts[3].GetStream(FileMode.Open, FileAccess.Read);
            }
            catch (InvalidOperationException)
            {
                //Expected Exception
                result--;
                Log.WriteLine("Expected Exception - Parent container for this part is closed");
            }

            try
            {
                result++;
                parts[3].GetStream(FileMode.Open);
            }
            catch (InvalidOperationException)
            {
                //Expected Exception
                result--;
                Log.WriteLine("Expected Exception - Parent container for this part is closed");
            }
            
            #endregion CheckDispose
         
            #region GetEnumerator Method
 
            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine("Enumerating all the parts");
            Log.WriteLine();  // Do a linefeed between tests

            c = Package.Open(fileName, FileMode.Open);

            PackagePartCollection pc = c.GetParts();
            
            try
            {
                //result++;
                foreach (PackagePart part in pc)
                {
                    Log.WriteLine("PackagePart name - " + part.Uri);
                    if (part.Uri.ToString() == "/frodo.xaml")
                        c.DeletePart(part.Uri);
                }
            }
            catch (Exception)
            {
                // Expected Exception
                Log.WriteLine("Expected Exception - Invalidating an enumerator");
                //result--;
            }
            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine("Enumerating all the parts after deleting one");
            Log.WriteLine();  // Do a linefeed between tests
            foreach (PackagePart part in pc)
            {
                Log.WriteLine("PackagePart name - " + part.Uri);
                //deleting non-existing part gives no error.
                if (part.Uri.ToString() == "/frodo.xaml")
                    c.DeletePart(part.Uri);
            }

            // verify relationship part was removed when part was deleted
            if (c.PartExists(PackUriHelper.GetRelationshipPartUri(CreatePartUri(partNames[0]))))
            {
                Log.AlwaysWriteLine("PackagePart delete failed to delete associated PackageRelationship part");
            }
            else
                Log.WriteLine("PackageRelationship part was correctly removed when part was deleted");

            c.Close();
        
            #endregion GetEnumerator Method


            #region Read Relationships

            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine("Parsing persisted Relationships");
            Log.WriteLine();  // Do a linefeed between tests

            c = Package.Open(fileName, FileMode.Open);
            
            //Reading the package relationships
            PackageRelationshipCollection relscollection = c.GetRelationships();

            foreach (PackageRelationship r in relscollection)
                Log.WriteLine("Relationship ID = " + r.Id + "RelationshipType = " + r.RelationshipType);

            c.Close();

            #endregion Read Relationships

            return result;
        }
        
        static private int RelationshipTests(Package c)
        {
            int result = 0;
            Log.WriteLine();
            Log.WriteLine("PackageRelationship tests");
            Log.WriteLine();

            // get relationship when none exist
            PackageRelationshipCollection relationships = c.GetRelationships();
            foreach (PackageRelationship r in relationships)
            {
                Log.AlwaysWriteLine("FAIL: GetRelationships returns results when it should be empty");
            }

            // add relationships
            int i, j;
            bool flushed = false;
            for (i = 0; i < parts.Length; i++)
            {
                for (j = parts.Length - 1; j >= 0; j--)
                {
                    Log.WriteLine("Creating PackageRelationship: " + parts[i].Uri);
                    c.CreateRelationship(parts[i].Uri, TargetMode.Internal, "http://" + i.ToString(),
                    j % 2 == 1 ? "C2P_" + i.ToString("X2") + j.ToString("X2") : null);

                    string targetName = "http://" + i.ToString() + "to" + j.ToString();

                    Log.WriteLine("Creating PackageRelationship: " + parts[i].Uri + " to " + parts[j].Uri + " with name " + targetName);
                    parts[i].CreateRelationship(parts[j].Uri, TargetMode.External, targetName,
                    j % 2 == 1 ? "P2P_" + j.ToString("X2") : null);
                }

                // flush to exercise the persistence
                if (!flushed && i > parts.Length / 2)
                {
                    c.Flush();
                    flushed = true;
                }
            }

			// verify relationships
            foreach (PackageRelationship r2 in c.GetRelationships())
            {
                Dump("container", r2);
            }

            // Attempt to add a container relationship that already exists.
            try
            {
                Log.WriteLine("Creating container relationship with existing ID.");
                
                // create relationship
                c.CreateRelationship(new Uri("/dummyURI", UriKind.Relative), TargetMode.Internal, "http://dummy", "C2P_0101");
                
                // if we get here, we failed
                Log.AlwaysWriteLine("Creating container relationship with existing ID failed to throw exception.");
                ++result;
            }
            catch (XmlException)
            {
                Log.WriteLine("Expected Exception - Creating container relationship with existing ID.");
                Log.WriteLine();
            }

            // Attempt to add a container relationship that has an empty ID.
            try
            {
                Log.WriteLine("Creating container relationship with empty ID.");

                // create relationship
                c.CreateRelationship(new Uri("/dummyURI", UriKind.Relative), TargetMode.Internal, "http://dummy", "");

                // if we get here, we failed
                Log.AlwaysWriteLine("Creating container relationship with empty ID failed to throw exception.");
                ++result;
            }
            catch (ArgumentNullException)
            {
                Log.WriteLine("Expected Exception - Creating container relationship with empty ID.");
                Log.WriteLine();
            }

            // verify relationships
            foreach (PackagePart p in parts)
            {
                foreach (PackageRelationship r3 in p.GetRelationships())
                {
                    Dump(p.Uri.ToString(), r3);
                }
            }

            // Attempt to add a part relationship that already exists.
            try
            {
                Log.WriteLine("Creating part relationship with existing ID.");
                
                // create relationship
                parts[0].CreateRelationship(new Uri("/dummyURI", UriKind.Relative), TargetMode.Internal, "http://dummy", "P2P_01");
                
                // if we get here, we failed
                Log.AlwaysWriteLine("Creating part relationship with existing ID failed to throw exception.");
                ++result;
            }
            catch (XmlException)
            {
                Log.WriteLine("Expected Exception - Creating part relationship with existing ID.");
                Log.WriteLine();
            }

            Log.WriteLine();
            Log.WriteLine();
            
            // verify fitered relationships
            Log.WriteLine("Filtered Relationships at the container level");
            foreach (PackageRelationship r2 in c.GetRelationshipsByType("http://5"))
            {                
                Dump("container", r2);
            }

            
            // verify filtered relationships
            Log.WriteLine(); 
            Log.WriteLine();
            Log.WriteLine("Filtered Relationships at the part level");
            foreach (PackagePart p in parts)
            {              
                foreach (PackageRelationship r3 in p.GetRelationshipsByType("http://7to3"))
                {                    
                    Dump(p.Uri.ToString(), r3);
                }
            }


            //Testing the generic Enumerator
            Log.WriteLine();
            Log.WriteLine();
            Log.WriteLine("Generic Relationships at the part level");
            IEnumerable<PackageRelationship> ienum = parts[0].GetRelationshipsByType("http://0to4");
            IEnumerator<PackageRelationship> enumerator = ienum.GetEnumerator();
            while(enumerator.MoveNext())
            {
                Dump(parts[0].Uri.ToString(), enumerator.Current);
            }

            Log.WriteLine();

            // delete a few
            PackagePart p2 = parts[2];
            Log.WriteLine();
            Log.WriteLine("Inspecting relationships on part 2: " + p2.Uri.ToString());
            Log.WriteLine();
            IEnumerator e = ((IEnumerable)p2.GetRelationships()).GetEnumerator();
            i = 0;
            Uri relPartName = PackUriHelper.GetRelationshipPartUri(p2.Uri);

            long length = 0;
            using (var stream = c.GetPart(relPartName).GetStream())
            {
                length = stream.Length; // make sure stream shrinks when rel removed
            }

            PackageRelationship r4 = null;
            PackageRelationship r5 = null;
            while (e.MoveNext())
            {
                if (i == 0)
                {
                    r4 = (PackageRelationship)e.Current;
                }
                if (i == 1)
                {
                    r5 = (PackageRelationship)e.Current;
                }
                ++i;
            }

            Log.WriteLine(i.ToString() + " relationships found");
            p2.DeleteRelationship(r4.Id);
			c.Flush();

            using (var stream = c.GetPart(relPartName).GetStream())
            {
                // make sure stream shrinks when rel removed
                if (stream.Length >= length)
                {
                    Log.AlwaysWriteLine("relationship stream length should be shorter after relationship removed");
                    ++result;
                }
            }

            // now verify that there is one less relationship
            e = ((IEnumerable)p2.GetRelationships()).GetEnumerator();
            while (e.MoveNext())
            {
                --i;
            }
            if (i != 1)
            {
                Log.AlwaysWriteLine("Removing PackageRelationship failed - count is off");
                ++result;
            }
            else
                Log.WriteLine("Removing one relationship succeeded");
            Log.WriteLine();

            // verify creating relationship to relationship fails
            try
            {
                Log.WriteLine("Creating PackageRelationship to PackageRelationship");
                
                // create relationship
                Uri relationship = PackUriHelper.GetRelationshipPartUri(parts[1].Uri);
                parts[3].CreateRelationship(relationship, TargetMode.Internal, "http://dummy");
                
                // if we get here, we failed
                Log.AlwaysWriteLine("Creating PackageRelationship to PackageRelationship failed to throw exception.");
                ++result;
            }
            catch
            {
                Log.WriteLine("Expected Exception - Creating PackageRelationship to PackageRelationship part");
                Log.WriteLine();
            }

            // verify creating relationship from relationship part fails
            try
            {
                Log.WriteLine("Creating PackageRelationship from PackageRelationship PackagePart");
                
                // create relationship
                Uri relationship = PackUriHelper.GetRelationshipPartUri(parts[1].Uri);
                PackagePart relPart = c.GetPart(relationship);
                relPart.CreateRelationship(parts[4].Uri, TargetMode.Internal, "http://dummy");

                // if we get here, we failed
                Log.AlwaysWriteLine("Creating PackageRelationship from PackageRelationship PackagePart failed to throw exception.");
                ++result;
            }
            catch
            {
                Log.WriteLine("Expected Exception - Creating PackageRelationship from PackageRelationship part");
                Log.WriteLine();
            }
            
            // verify removing last relationship removes the PackageRelationship PackagePart
			List<PackageRelationship> relsToRemove = new List<PackageRelationship>();
			foreach (PackageRelationship r in p2.GetRelationships())
			{
				relsToRemove.Add(r);
			}

			for (i = 0; i < relsToRemove.Count; i++)
			{
				p2.DeleteRelationship(relsToRemove[i].Id);
			}

			c.Flush();



            // verify removing last package-level - relationship removes the PackageRelationship PackagePart
            relsToRemove = new List<PackageRelationship>();
            foreach (PackageRelationship r in c.GetRelationships())
            {
                relsToRemove.Add(r);
            }

            for (i = 0; i < relsToRemove.Count; i++)
            {
                c.DeleteRelationship(relsToRemove[i].Id);
            }

            c.Flush();


            Uri relationshipPartName = PackUriHelper.GetRelationshipPartUri(p2.Uri);
            if (c.PartExists(relationshipPartName))
            {
                Log.AlwaysWriteLine("Removing last PackageRelationship failed to cause PackageRelationship part to be removed");
                ++result;
            }
            else
                Log.WriteLine("PackageRelationship part was correctly removed when last PackageRelationship was deleted");

            return result;
        }
               
        static private int ContainerStreamTests(string fileName)
        {
            int result = 0;

            FileInfo fInfo = new FileInfo(fileName);
            Stream containerStream = fInfo.Open(FileMode.Open, FileAccess.ReadWrite);
            Package c = Package.Open(containerStream, FileMode.Open, FileAccess.ReadWrite);

            Log.WriteLine();  // Do a linefeed between tests
            Log.WriteLine("Enumerating all the parts after fetching from Stream");
            Log.WriteLine();  // Do a linefeed between tests
            c.CreatePart(CreatePartUri("/images/aragon.jpg"), "xml/jpg+img");
            c.CreatePart(CreatePartUri("/images/arathon.jpg"), "xml/jpg+img");

            bool b = c.PartExists(CreatePartUri("/images/hobbits.jpg"));
            c.Close();
            containerStream.Close();

            containerStream = fInfo.Open(FileMode.Open, FileAccess.ReadWrite);
            c = Package.Open(containerStream, FileMode.Open, FileAccess.ReadWrite);
            foreach (PackagePart part in c.GetParts())
            {
                Log.WriteLine("PackagePart name - " + part.Uri);               
            }

            c.DeletePart(CreatePartUri("/images/aragon.jpg"));
            c.Close();
            containerStream.Close();

            //ReadOnly Package opened on a readwrite stream
            containerStream = fInfo.Open(FileMode.Open, FileAccess.ReadWrite);
            c = Package.Open(containerStream, FileMode.Open, FileAccess.Read);

            PackagePart readOnlyPart = c.GetPart(CreatePartUri("/images/arathon.jpg"));
            Stream s = readOnlyPart.GetStream();
            
            Log.WriteLine();  // Do a linefeed between tests

            //stream should not be writable
            try
            {
                result++;
                StreamWriter writer = new StreamWriter(s); 	
            }
            catch (ArgumentException)
            {
                // Expected Exception
                result--;
                Log.WriteLine(
                    "Expected Exception - ReadOnly container created on a ReadWrite stream. None of the parts streams are writable");
                s.Close();
            }

            s = readOnlyPart.GetStream();            

            //Package cannot be modified
            try
            {
                result++;
                c.DeletePart(CreatePartUri("/images/arathon.jpg"));
            }
            catch (IOException)
            {
                // Expected Exception
                Log.WriteLine("Expected Exception - ReadOnly container created on a ReadWrite stream. Cannot be modified.");
                result--;
            }

            c.Close();
            containerStream.Close();

            containerStream = fInfo.Open(FileMode.Open, FileAccess.ReadWrite);
            c = Package.Open(containerStream, FileMode.Open, FileAccess.ReadWrite);
            c.DeletePart(CreatePartUri("/images/arathon.jpg"));
            c.Close();
            containerStream.Close();

            return result;
        }

        static private int UriTests()
        {
            int result = 0;
            const int numberOfUris = 19;
            Uri[] uriArray = new Uri[numberOfUris];
            string[] expectedUriArray = new string[numberOfUris];

            uriArray[0] = CreatePartUri("/images/Pic.jpg");
            uriArray[1] = CreatePartUri("./images/pic.jpg");
            uriArray[2] = CreatePartUri("../images/pic.jpg");
            uriArray[3] = CreatePartUri("images/pic.jpg");
            uriArray[4] = CreatePartUri("/../images/pic.jpg");
            uriArray[5] = PackUriHelper.ResolvePartUri(uriArray[3], new Uri("frodo.jpg", UriKind.RelativeOrAbsolute));
            uriArray[6] = PackUriHelper.ResolvePartUri(uriArray[3], new Uri("../hobbits.jpg", UriKind.RelativeOrAbsolute));
            uriArray[7] = PackUriHelper.ResolvePartUri(uriArray[3], new Uri("./hobbits.jpg", UriKind.RelativeOrAbsolute));
            uriArray[8] = PackUriHelper.ResolvePartUri(uriArray[3], new Uri("../../../hobbits.jpg", UriKind.RelativeOrAbsolute));

            uriArray[9] = PackUriHelper.Create(new Uri("http://user+:Info@www.abc.com:2030/files/photos.package"), new Uri("/mypics/fun.jpg", UriKind.RelativeOrAbsolute));
            uriArray[10] = CreatePackUri("file:///www.abc%2541.com/files/photos.package", "/my%20pics/fun.jpg");
            uriArray[11] = CreatePackUri("https://www.def.com/my%2bfiles/%photos.package#everything", "/my pics/fun.jpg"); // escaped chars not allowed in host names
            uriArray[12] = CreatePackUri("http://www.hij.com/my files/photos.package#tag1", "/my pics/fun.jpg", "#fragment%20here");
            uriArray[13] = CreatePackUri("http://www.klm.com/fi,les/photos.package?asdadasd", "/my +&=pics/fun.jpg");
            uriArray[14] = CreatePackUri("HTTP://www.nop.com/files*/photos.package", "/my pics/fun.jpg", "#");
            uriArray[15] = CreatePackUri("http://www.qrs.com/fi%20l&es/photos.package#", "/my%20pics/fun.jpg", "#    ");
            uriArray[16] = PackUriHelper.Create(new Uri("http://www.tuv.com/files/photos.packageццц"), null);
            uriArray[17] = CreatePackUri("PACK://file:,,,www.xyz.com,files,photos.package:8888/my pics/fun.jpg", "/your pics/foo.jpg");

            expectedUriArray[0] = "/images/Pic.jpg";
            expectedUriArray[1] = "/images/pic.jpg";
            expectedUriArray[2] = "/images/pic.jpg";
            expectedUriArray[3] = "/images/pic.jpg";
            expectedUriArray[4] = "/images/pic.jpg";
            expectedUriArray[5] = "/images/frodo.jpg";
            expectedUriArray[6] = "/hobbits.jpg";
            expectedUriArray[7] = "/images/hobbits.jpg";
            expectedUriArray[8] = "/hobbits.jpg";

            expectedUriArray[9]  = "pack://http:,,user+:Info%40www.abc.com:2030,files,photos.package/mypics/fun.jpg";
            expectedUriArray[10] = "pack://file:,,,www.abc%252541.com,files,photos.package/my%20pics/fun.jpg";
            expectedUriArray[11] = "pack://https:,,www.def.com,my%252bfiles,%2525photos.package/my%20pics/fun.jpg";
            expectedUriArray[12] = "pack://http:,,www.hij.com,my%2520files,photos.package/my%20pics/fun.jpg#fragment%20here";
            expectedUriArray[13] = "pack://http:,,www.klm.com,fi%2Cles,photos.package%3Fasdadasd/my%20+&=pics/fun.jpg"; 
            expectedUriArray[14] = "pack://http:,,www.nop.com,files*,photos.package/my%20pics/fun.jpg#";
            expectedUriArray[15] = "pack://http:,,www.qrs.com,fi%2520l&es,photos.package/my%20pics/fun.jpg#";
            expectedUriArray[16] = "pack://http:,,www.tuv.com,files,photos.package%25D1%2586%25D1%2586%25D1%2586/";
            expectedUriArray[17] = "pack://pack:,,file:%2C%2C%2Cwww.xyz.com%2Cfiles%2Cphotos.package:8888,my%2520pics,fun.jpg/your%20pics/foo.jpg";

            for (int i = 0; i < numberOfUris-1; i++)
            {
                if (uriArray[i].IsAbsoluteUri)
                {
                    if (uriArray[i].GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped) != expectedUriArray[i])
                       throw new InvalidDataException("The URI created [" + uriArray[i].ToString() + "] was not as expected [ " + expectedUriArray[i] + "]");
                   Log.WriteLine(" PackUri " + uriArray[i].GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped));
               }
                else
                {
                    if (uriArray[i].ToString() != expectedUriArray[i])
                        throw new InvalidDataException("The URI created [" + uriArray[i].ToString() + "] was not as expected [ " + expectedUriArray[i] + "]");
                    Log.WriteLine(" PackUri " + uriArray[i].ToString());
                }
            }

            try
            {
                result++;
                uriArray[9] = PackUriHelper.CreatePartUri(new Uri("http://www.abc.com/myfiles/frodo.jpg#werwe"));
            }
            catch (ArgumentException)
            {
                //Expected Exception
                result--;
            }

            Uri relativeUri = PackUriHelper.GetRelativeUri(uriArray[0], uriArray[5]);
            Log.WriteLine(" relative path " + relativeUri.ToString());


            for (int i = 9; i < 18; i++)
            {
                Uri packageUri = PackUriHelper.GetPackageUri(new Uri(expectedUriArray[i]));
                Log.WriteLine(" package Uri [" + i + "]  - " + packageUri.ToString());

                Uri partUri = PackUriHelper.GetPartUri(new Uri(expectedUriArray[i]));
                if (partUri != null)
                    Log.WriteLine(" part Uri    [" + i + "]  - " + partUri.ToString());
            }

            try
            {
                result++;
                Uri packageUri = PackUriHelper.GetPackageUri(new Uri("pack://http:,,www.abc.com,files,a.html%23fragment/"));
            }
            catch (ArgumentException)
            {
                result--;
                Log.WriteLine("Expected Exception - - package Uri may not have a fragment");
            }

            try
            {
                result++;
                Uri packUri = CreatePackUri("http://www.hij.com/my files/photos.package#tag1", "/my pics/fun.jpg#fragment%20here");
            }
            catch (ArgumentException)
            {
                result--;
                Log.WriteLine("Expected Exception - - partUri may not have a fragment");
            }

            try
            {
                result++;
                Uri packUri = PackUriHelper.GetPackageUri(new Uri("pack://http:,,www.hij.com,my%20files,photos.package/mypics/"));
            }
            catch (ArgumentException)
            {
                result--;
                Log.WriteLine("Expected Exception - packUri with a path component ending in '/'");
            }

            return result;
        }

        static private int UriCompareTests()
        {
            int result = 0;

            const int numberOfUris = 16;
            Uri[] uriArray = new Uri[numberOfUris];
            Uri[] equivalentUriArray = new Uri[numberOfUris];
            int[] expectedInequalities = { 1, 2, 8, 10, 11 };

            uriArray[0] = CreatePartUri("/images/%Pic.jpg");
            uriArray[1] = CreatePartUri("./iMAges/Ллpic%25.jpg");
            uriArray[2] = CreatePartUri("../фываimages/uıköpic.jpg");
            uriArray[3] = CreatePartUri("images/my pic.jpg");
            uriArray[4] = CreatePartUri("/../images/pic.jpg");
            uriArray[5] = PackUriHelper.Create(new Uri("http://user+:Info@www.abc.com:2030/files/photos.package"), new Uri("/mypics/fun.jpg", UriKind.RelativeOrAbsolute));
            uriArray[6] = CreatePackUri("file:///www.abc%2541.com/files/photos.package", "/my%20pics/fun.jpg");
            uriArray[7] = CreatePackUri("https://www.DEF.com/my%2bfiles/%photos.package#everything", "/my pics/fun.jpg"); // escaped chars not allowed in host names
            uriArray[8] = CreatePackUri("http://www.hij.com/my files/PHOTOS.package#tag1", "/my pics/fun.jpg", "#fragment%20here");
            uriArray[9] = CreatePackUri("http://www.klm.com/fi,les/photos.package?asdadasd", "/my +&=pics/fun.jpg");
            uriArray[10] = CreatePackUri("http://www.nop.com/files*/photos.package", "/my pics/funЩ.jpg");
            uriArray[11] = CreatePackUri("http://www.qrs.com/fi%20l&es/photos.package", "/my%20pics/funв.jpg");
            uriArray[12] = PackUriHelper.Create(new Uri("http://www.tuv.com/files/photos.packageццц"), null);
            uriArray[13] = CreatePackUri("pack://file:,,,www.xyz.com,files,photos.package:8888/my pics/fun.jpg", "/your pics/foo.jpg");
            uriArray[14] = PackUriHelper.Create(new Uri("application://"));
            uriArray[15] = PackUriHelper.Create(new Uri("location:///"));


            equivalentUriArray[0] = new Uri("/images/%25pic.jpg", UriKind.RelativeOrAbsolute);
            equivalentUriArray[1] = PackUriHelper.CreatePartUri(new Uri("/imaGES/лЛpic%25.jpg", UriKind.RelativeOrAbsolute));
            equivalentUriArray[2] = PackUriHelper.CreatePartUri(new Uri("/ФЫВАimages/uıKÖpic.jpg", UriKind.RelativeOrAbsolute));
            equivalentUriArray[3] = PackUriHelper.CreatePartUri(new Uri("/images/my pic.jpg", UriKind.RelativeOrAbsolute));
            equivalentUriArray[4] = PackUriHelper.CreatePartUri(new Uri("/images/pic.jpg", UriKind.RelativeOrAbsolute));
            equivalentUriArray[5] = new Uri("pack://http:,,user+:Info%40www.abc.com:2030,files,photos.package/mypics/fun.jpg");
            equivalentUriArray[6] = new Uri("pack://file:,,,www.abc%252541.com,files,photos.package/my%20pics/fun.jpg");
            equivalentUriArray[7] = new Uri("pack://https:,,www.def.com,my%252bfiles,%2525photos.package/my%20pics/fun.jpg");
            equivalentUriArray[8] = new Uri("pack://http:,,www.hij.com,my%2520files,photos.package/my%20pics/fun.jpg#fragment%20here");
            equivalentUriArray[9] = new Uri("pack://http:,,www.klm.com,fi%2Cles,photos.package%3Fasdadasd/my%20+&=pics/fun.jpg");
            equivalentUriArray[10] = new Uri("pack://http:,,www.NOP.com,files*,photos.package/my%20pics/funщ.jpg");
            equivalentUriArray[11] = new Uri("pack://http:,,www.qrs.com,fi%2520l&es,PHOTOS.package/my%20pics/fun%d0%92.jpg");
            equivalentUriArray[12] = new Uri("pack://http:,,www.tuv.com,files,photos.package%25D1%2586%25D1%2586%25D1%2586/");
            equivalentUriArray[13] = new Uri("pack://pack:,,file:%2C%2C%2Cwww.xyz.com%2Cfiles%2Cphotos.package:8888,my%2520pics,fun.jpg/your%20pics/foo.jpg");
            equivalentUriArray[14] = new Uri("pack://application:,,,/");
            equivalentUriArray[15] = new Uri("pack://location:,,/");

            for (int i = 0; i < numberOfUris; i++)
            {
                if (Contains(expectedInequalities, i))
                    result++;                

                if (uriArray[i].IsAbsoluteUri)
                {
                    if (PackUriHelper.ComparePackUri(uriArray[i], equivalentUriArray[i]) != 0)
                    {
                        result--;                        
                    }
                }
                else
                {
                    if (PackUriHelper.ComparePartUri(uriArray[i], equivalentUriArray[i]) != 0)
                    {
                        result--;
                    }
                }

                if (result != 0)
                {
                    Log.AlwaysWriteLine("The URI created [" + uriArray[i].ToString() + "] was equivalent to [ " + equivalentUriArray[i].ToString() + "]");
                    break;
                }
            }

            return result;
        }

        static Type GetTypeFromWindowsBase(String typeName)
        {
            Assembly assembly = typeof(System.Windows.DependencyProperty).Assembly;  // WindowsBase.dll
            return assembly.GetType(typeName, false, true);
        }

        static private Stream ConstructSparseMemoryStream(long lowWaterMark, long highWaterMark)
        {
            Type type = GetTypeFromWindowsBase("MS.Internal.IO.Packaging.SparseMemoryStream");
            ConstructorInfo[] ci = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            object[] args = new object[2];
            args[0] = lowWaterMark;
            args[1] = highWaterMark;
            return (Stream)(ci[0].Invoke(args));
        }

        static private void WriteToStream(Stream source, Stream sink)
        {
            Type type = GetTypeFromWindowsBase("MS.Internal.IO.Packaging.SparseMemoryStream");
            MethodInfo _methodInfo = type.GetMethod("WriteToStream", BindingFlags.Instance | BindingFlags.NonPublic);
            _methodInfo.Invoke(source, new object [] {sink});
        }

        static private int UtilityTests()
        {
            int result = 0;

            Stream sparseMemoryStream = ConstructSparseMemoryStream(0x10000, 0x1000000);
            Stream ms = new MemoryStream();
            sparseMemoryStream.SetLength(5);
            WriteToStream(sparseMemoryStream, ms);
            if (ms.Length != 5)
            {
                Log.AlwaysWriteLine("TEST FAIL: SparseMemoryStream writeToStream failed");
                result++;
            }

            return result;
        }

        static private int ContentTypeTests()
        {
            int result = 0;
            string[] strings = new string[33];
            strings[0] = "text/xml;\tparameter1=value1 \n; \n parameter2=\"value2\"";
            strings[1] = "text/xml  \n \r \t  ";       // trailing LWS characters
            strings[2] = "text /xml";                  // space between type and subtype
            strings[3] = "text/xml;parameter\t=value"; //tab between param and value
            strings[4] = "image/jpeg; param=color";
            strings[5] = "image/jpeg;param=color;param2=color2"; 
            strings[6] = "image/jpeg[]; param=color";   //invalid subtype
            strings[7] = "image/jpeg; param=color<>";   //invalid parameter value
            strings[8] = "text/\"xml\"";                //invalid subtype - quoted string
            strings[9] = "text/xml;param1==value";      //extra equal sign
            strings[10] = "text/xml\\jpg";              //invalid character \
            strings[11] = "text\\xml";                  //invalid separator "\"
            strings[12] = "text/uniüode";               // unicode character
            strings[13] = "image/jpg;param=color ; param=color2"; //same parameter names
            strings[14] = "application/xaml+text";
            strings[15] = "application/xaml+text ; param=abc ; \n param2=xyz";
            strings[16] = "application/xaml ; param=abc param2=\"xyz\""; //missing semicolon
            strings[17] = "application/xaml ;param=abc;param2=\"xyz\"param3=\"abc\""; //no semicolon after "
            strings[18] = "application/xaml ; param=abc; param2=\"xyz\\\"abc\\\"pqr\"";
            strings[19] = "application/xaml; param=abc  ; param2=\"xyz\\\"abc\\\"pqr\" (comments)"; //comments
            strings[20] = "application/xaml ; param=abc     ;param2=\"xyz\\\"abc=;\\\"pqr\"";
            strings[21] = "";
            strings[22] = "     ";  //multiple spaces
            strings[23] = "application/xaml+text ; param=abc ; ; param2=xyz"; //two semicolons
            strings[24] = "application/#$%^&*-_+~`!.|' ; param=abc ;\n param2=\"xyz;stu\"";
            strings[25] = "application/xaml+text ; param=abc ;\r\n param2=\"xyz;\r\n stu\"";
            strings[26] = "application/xaml+text ; param=abc ;\r\n param2=\"xyz;\r\n stu\r\"";
            strings[27] = "application/xaml+text ; param=\"abc";
            strings[28] = "application/{xaml+text}";
            strings[29] = "application/<xaml+text>";
            strings[30] = "application/xaml ; param="; //missing param value - error
            strings[31] = "application/xaml ; param=;"; //missing param value - error
            strings[32] = "application/xaml ; param=\"\""; //empty parameter value is ok

            int[] expectedExceptions = { 1, 2, 3, 6, 7, 8, 9, 10, 11, 12, 13, 
                                         16, 17, 19, 22, 23, 26, 27, 28, 29, 
                                         30, 31 };

            Package package = Package.Open(fileZip, FileMode.Open, FileAccess.ReadWrite);
            string  tempPart = "/temporary/part";
            Uri tempPartUri;
            PackagePart packagePart;

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    if (Contains(expectedExceptions, i))
                        result++;

                    Log.WriteLine("\n ContentType [" + i + "]\n");
                    Log.WriteLine(" original    =   '" + strings[i] + "'");

                    tempPartUri = new Uri(tempPart + i + ".xaml", UriKind.Relative);
                    packagePart = package.CreatePart(tempPartUri, strings[i]);

                    Log.WriteLine("");
                    Log.WriteLine(" validate type        =   " + packagePart.ContentType);
                }
                catch (ArgumentException)
                {
                    if (Contains(expectedExceptions, i))
                    {
                        result--;
                        Log.WriteLine("Expected Exception");
                        continue;
                    }
                    else
                    {
                        result++;
                        throw;
                    }
                }
            }

            return result;
        }
               
        #endregion API Tests   

        #region Utility Methods

        private static bool Contains(int[] intArray, int value)
        {
            foreach (int x in intArray)
            {
                if (x == value)
                {
                    return true;
                }                   
            }
            return false;
        }
              
        private static void Dump(string title, PackageRelationship r)
        {
			Log.WriteLine("PackageRelationship: " + title + " RelationshipType={" + r.RelationshipType + "} target={" + r.TargetUri + "} + TargetMode={" + r.TargetMode + "} Id={" + r.Id + "}");
		}

        //Helper method to call PackUriHelper.CreatePartUri with a string parameter.
        private static Uri CreatePartUri(string uri)
        {
            return PackUriHelper.CreatePartUri(new Uri(uri, UriKind.Relative));
        }

        //Helper method to call PackUriHelper.Create with a string parameter.
        private static Uri CreatePackUri(string uri, string partUri)
        {
            return PackUriHelper.Create(new Uri(uri), PackUriHelper.CreatePartUri(new Uri(partUri, UriKind.Relative)), null);
        }

        //Helper method to call PackUriHelper.Create with a string parameter.
        private static Uri CreatePackUri(string uri, string partUri, string fragment)
        {
            return PackUriHelper.Create(new Uri(uri), PackUriHelper.CreatePartUri(new Uri(partUri, UriKind.Relative)), fragment);
        }

        private static string GetStringForPartUri(Uri partUri)
        {
            Uri tempUri = new Uri(new Uri("http://default/"), partUri);
            return tempUri.GetComponents(UriComponents.Path | UriComponents.KeepDelimiter, UriFormat.UriEscaped);
        }
        
        #endregion Utility Methods

    }    
}
