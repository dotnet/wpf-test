// // Licensed to the .NET Foundation under one or more agreements.
// // The .NET Foundation licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.

// using System;
// using System.Windows;
// using System.Windows.Resources;
// using System.IO;
// using System.Runtime.InteropServices;
// using System.Diagnostics;
// using Microsoft.Test.Logging;
// using Microsoft.Test.Loaders;
// using System.Threading;
// using System.Windows.Automation;
// using Microsoft.Win32;
// using Microsoft.Test.Deployment;
// using System.Runtime.Remoting;
// using System.Reflection;
// using Microsoft.Test.Discovery;

// namespace Microsoft.Test.Windows.Client.AppSec.Deployment
// {
//     [TestDefaults(DefaultPriority=0, DefaultSubArea="Application_Orcas")]
//     public class ResourceBaseAssemblyTests
//     {
//         [Test("ResourceBaseAssembly_1")]
//         public void LoadContentFromResourceBaseAssembly()
//         {
//             new TestLog("ResourceBaseAssembly Test 1");

//             bool pass = true;

//             // Setup the domains
//             AppDomain currentDomain = AppDomain.CurrentDomain;
//             AppDomain otherDomain = AppDomain.CreateDomain("otherDomain");

//             // Create an object for the other domain to use...
//             ObjectHandle oh = otherDomain.CreateInstance(Assembly.GetExecutingAssembly().GetName().ToString(), "Microsoft.Test.Windows.Client.AppSec.Deployment.ResBaseAsmTestObject");
//             ResBaseAsmTestObject otherDomainObject = (ResBaseAsmTestObject)oh.Unwrap();
//             GlobalLog.LogEvidence("Created second appDomain and instance of test object...");

//             // Validate that ResourceBaseAssembly is null on the other domain
//             pass &= otherDomainObject.TestResourceBaseAssembly(true, false);

//             // Now give it a ResourceBaseAssembly but a nonexistent resource (or exists in the principal assembly?)
//             otherDomainObject.SetResourceBaseAssembly(@"DeploymentCommonLibrary.dll");
//             otherDomainObject.MyResourcePath = "/nonexistentresource.jpg";

//             // Validate that ResourceBaseAssembly is not null but the content fails to load
//             pass &= otherDomainObject.TestResourceBaseAssembly(false, false);

//             // Now give it an accurate resource and make sure this works
//             otherDomainObject.MyResourcePath = "/deploy_picture1.jpg";

//             // Validate content loads without fanfare.
//             pass &= otherDomainObject.TestResourceBaseAssembly(false, true);

//             if (pass)
//             {
//                 GlobalLog.LogEvidence("Pass: Content loaded from set ResourceBaseAssembly as expected");
//                 TestLog.Current.Result = TestResult.Pass;
//             }
//             else
//             {
//                 GlobalLog.LogEvidence("Fail: Content did not load from set ResourceBaseAssembly as expected");
//                 TestLog.Current.Result = TestResult.Fail;
//             }
//             TestLog.Current.Close();
//         }
//         [Test("ResourceBaseAssembly_2")]
//         public void VerifyExceptionOnSecondSet()
//         {
//             bool pass = true;

//             new TestLog("ResourceBaseAssembly Test 2");

//             // Setup the domains
//             AppDomain currentDomain = AppDomain.CurrentDomain;
//             AppDomain otherDomain = AppDomain.CreateDomain("otherDomain");

//             // Create an object for the other domain to use...
//             ObjectHandle oh = otherDomain.CreateInstance(Assembly.GetExecutingAssembly().GetName().ToString(), "Microsoft.Test.Windows.Client.AppSec.Deployment.ResBaseAsmTestObject");
//             ResBaseAsmTestObject otherDomainObject = (ResBaseAsmTestObject)oh.Unwrap();
//             GlobalLog.LogEvidence("Created second appDomain and instance of test object...");

//             // Validate that ResourceBaseAssembly is null on the other domain
//             pass &= otherDomainObject.TestResourceBaseAssembly(true, false);

//             // Now give it a ResourceBaseAssembly but a nonexistent resource (or exists in the principal assembly?)
//             otherDomainObject.SetResourceBaseAssembly(@"DeploymentCommonLibrary.dll");
//             otherDomainObject.MyResourcePath = "/nonexistentresource.jpg";

//             // Validate that ResourceBaseAssembly is not null but the content fails to load
//             pass &= otherDomainObject.TestResourceBaseAssembly(false, false);

//             bool exceptionThrown = false;

//             // Now set the ResourceBaseAssembly again, making sure the expected exception is thrown
//             try
//             {
//                 otherDomainObject.SetResourceBaseAssembly(@"OrcasDeploymentTestCodeLibrary.dll");
//             }
//             catch (System.InvalidOperationException)
//             {
//                 GlobalLog.LogEvidence("Got the expected exception for setting the ResourceBaseAssembly a second time.");
//                 exceptionThrown = true;
//             }

//             // If setting throw an IOE, we're golden.
//             pass &= exceptionThrown;

//             if (pass)
//             {
//                 GlobalLog.LogEvidence("Pass : Behavior as expected on second setting of ResourceBaseAssembly");
//                 TestLog.Current.Result = TestResult.Pass;
//             }
//             else
//             {
//                 GlobalLog.LogEvidence("Fail : Behavior not as expected on second setting of ResourceBaseAssembly");
//                 TestLog.Current.Result = TestResult.Fail;
//             }
//             TestLog.Current.Close();
//         }
//     }

//     public class ResBaseAsmTestObject : MarshalByRefObject
//     {
//         public string MyResourcePath = null;

//         public void SetResourceBaseAssembly(string BaseAssemblyPath)
//         {
//             BaseAssemblyPath = Path.GetFullPath(BaseAssemblyPath);
//             GlobalLog.LogEvidence("Setting ResourceBaseAssembly to " + BaseAssemblyPath);
//             Application.ResourceAssembly = Assembly.LoadFile(BaseAssemblyPath);
//         }

//         public bool TestResourceBaseAssembly(bool RBAShouldBeNull, bool resourceShouldBeFound)
//         {
//             GlobalLog.LogEvidence("Application.ResourceBaseAssembly validation: ");
//             if (Application.ResourceAssembly == null)
//             {
//                 if (RBAShouldBeNull)
//                 {
//                     GlobalLog.LogEvidence("Application.ResourceBaseAssembly was null (Expected)");
//                     return true;
//                 }
//                 else
//                 {
//                     GlobalLog.LogEvidence("(Error!) Application.ResourceBaseAssembly was null (Error!)");
//                     return false;
//                 }
//             }
//             else
//             {
//                 if (RBAShouldBeNull)
//                 {
//                     GlobalLog.LogEvidence("(Error!) Application.ResourceBaseAssembly was expected null but was not (Error!)");
//                     return false;
//                 }

//                 GlobalLog.LogEvidence("***  Current ResourceBaseAssembly is set to " + Application.ResourceAssembly.GetName().ToString() + " ***");
//                 try
//                 {
//                     StreamResourceInfo streamResourceInfo = Application.GetResourceStream(new Uri(MyResourcePath, UriKind.RelativeOrAbsolute));
//                     GlobalLog.LogEvidence("Successfully got resource " + MyResourcePath);
//                     if (resourceShouldBeFound)
//                     {
//                         return true;
//                     }
//                     else
//                     {
//                         GlobalLog.LogEvidence("... but the resource was expected to not load.  (Error!)");
//                         return false;
//                     }
//                 }
//                 catch (System.IO.IOException)
//                 {
//                     if (resourceShouldBeFound)
//                     {
//                         GlobalLog.LogEvidence("(Error!) Expected resource " + MyResourcePath + " to be present in " + Application.ResourceAssembly.GetName().ToString() + " but it was not (Error!)");
//                         return false;
//                     }
//                     else
//                     {
//                         GlobalLog.LogEvidence("Resource " + MyResourcePath + " was not present in " + Application.ResourceAssembly.GetName().ToString() + " (Expected) ");
//                         return true;
//                     }
//                 }
//             }
//         }
//     }
// }
