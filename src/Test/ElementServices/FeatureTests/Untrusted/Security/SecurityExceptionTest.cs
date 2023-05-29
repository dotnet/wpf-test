// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verify SecurityExceptions are thrown when protected
 * members are used in partial trust.
 * 
 * Tests from the data file SecurityExceptionVerification_Cases.xml add a test case
 * attribute to the SecurityExceptionTest.Run method with parameter to select
 * the XML assembly element to be tested.
 * 
 * This harness only tests classes with default constructors (no arguments). To test 
 * classes without default constructors an instance of the class and call TestMembers(...)
 * with a list of the methods to be tested.
 * 
 * Contributors: 
 *
 
  
 * Revision:         $Revision:  $
 
********************************************************************/

using System;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Xml;

using System.Collections;

using Microsoft.Test.Security;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI.Trusted.Utilities; // internalobject

namespace Avalon.Test.CoreUI.Hosting.Security
{
    /// <summary>
    /// Parse an xml file describing assembly class members to test for SecurityExceptions in partial trust.
    /// </summary>
    /// <description>
    /// Test case helper for verifying SecurityExceptions
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class SecurityExceptionTest
    {
        private string _fileName;

        private string _targetAssemblyName;

        /// <summary>
        /// Test case entry point.
        /// The TestCaseAttribute param is used to select which assembly in the support file will be tested. The case
        /// parses this element in the file to determine which classes and members to test.
        /// </summary>
        [TestCase("0", @"Security", "WindowsBase", TestCaseSecurityLevel.PartialTrust, "WindowsBase security exception tests")]
        [TestCase("0", @"Security", "PresentationCore", TestCaseSecurityLevel.PartialTrust, "PresentationCore security exception tests")]
        [TestCaseSupportFile("SecurityExceptionVerification_Cases.xml")]
        public void Run()
        {
            _fileName = "SecurityExceptionVerification_Cases.xml";

            //
            // Setup test case.
            //

            // Parse params.
            TestCaseInfo testCaseInfo = TestCaseInfo.GetCurrentInfo();
            _targetAssemblyName = testCaseInfo.Params; 

            // Load the security exception tests file.
            XmlDocument securityExceptionDoc = new XmlDocument();
            securityExceptionDoc.Load(_fileName);           

            // Get class nodes from test case doc for current assembly.
            XmlNodeList classNodeList = securityExceptionDoc.SelectNodes("./SecurityExceptions/Assembly[@Name='" + _targetAssemblyName + "']/Class");
            if ((classNodeList == null) || (classNodeList.Count == 0))
            {
                throw new Exception("XML class elements could not be found for assembly " + _targetAssemblyName + " in test case file " + _fileName);
            }

            //
            // Test each class.
            //
            bool allPassed = true;
            foreach (XmlNode classNode in classNodeList)
            {
                CoreLogger.LogStatus("Testing class " + classNode.Attributes["Name"].Value);

                // Use the trusted internal object class to get target class type reference from assembly.
                InternalObject internalObj = new InternalObject(_targetAssemblyName, classNode.Attributes["Name"].Value);
                Type targetType = internalObj.GetRealType();

                // Reflect MemberInfo for the class depending on member
                MemberInfo[] targetMembers = GetMemberInfo(targetType, classNode);
                foreach(MemberInfo mi in targetMembers)
                {
                    CoreLogger.LogStatus("    " + mi);
                }

                if ((targetMembers == null) || (targetMembers.Length == 0))
                {
                    throw new Exception("No members matching selection found. :(");
                }

                // Test the target class.
                if (TestMembers(targetType, targetMembers, null) == false) allPassed = false;

                CoreLogger.LogStatus("Completed testing class " + classNode.Attributes["Name"].Value);
            }
            CoreLogger.LogTestResult(allPassed, "Test complete");
        }

        /// <summary>
        /// Test target type for security exceptions. Combination mode is Include.
        /// </summary>
        public static bool TestSecurityExceptions(string targetTypeName, string targetAssemblyName, string[] targetMemberNames, object targetInstance)
        {
            // What we're doing.
            CoreLogger.LogStatus("Testing security exceptions for type " + targetTypeName);
            CoreLogger.LogStatus("    in assembly " + targetAssemblyName);
            CoreLogger.LogStatus("    members:");
            foreach (string targetMemberName in targetMemberNames)
            {
                CoreLogger.LogStatus("        " + targetMemberName);
            }

            //
            // Load type and member info that will be used for test.
            //

            // Use the trusted internal object class to get target class type reference from assembly.
            InternalObject internalObj = new InternalObject(targetAssemblyName, targetTypeName);
            Type targetType = internalObj.GetRealType();

            if (targetType != null)
            {
                CoreLogger.LogStatus("Got target type");
            }
            else
            {
                throw new Exception("Could not get target type " + targetTypeName);
            }

            // Get MemberInfo data for the listed members, this throws if a member name is not found.
            MemberInfo[] targetMembers;
            targetMembers = SecurityExceptionTest.GetMemberInfo(targetType, targetMemberNames, MemberSelectMode.Include);

            if ((targetMembers == null) || (targetMembers.Length <= 0))
            {
                throw new Exception("Could not find any target members");
            }

            // 
            // Test the target class.
            //

            return TestMembers(targetType, targetMembers, targetInstance);            
        }
        

        /// <summary>
        /// Lookup MemberInfo for the target type based on listed names and selection mode.
        /// </summary>
        /// <param name="target">Target type</param>
        /// <param name="classNode">XML node containing list of Member nodes to be tested.</param>
        /// <returns>Array of MemberInfos</returns>
        /// <remarks>Builds string array of member names to test from classNode then uses
        /// the other ParseMemberNames</remarks>
        private static MemberInfo[] GetMemberInfo(Type target, XmlNode classNode)
        {
            //
            // Get selection mode. Class node must have attribute MemberSelect with value All, Include or Exclude.
            //
            if (classNode.Attributes["MemberSelect"] == null)
            {
                throw new Exception("MemberSelect attribute is required. Possible values are All, Exclude, or Include");
            }
            MemberSelectMode mode = ParseSelectionMode(classNode.Attributes["MemberSelect"].Value);


            ArrayList targetMemberNames = new ArrayList();

            // Select each Member element in this Class XML node.
            XmlNodeList memberNodes = classNode.SelectNodes("./Member");

            // Copy member element names into an array of strings.
            foreach (XmlNode memberNode in memberNodes)
            {
                targetMemberNames.Add(memberNode.Attributes["Name"].Value);
            }

            return GetMemberInfo(target, (string[])targetMemberNames.ToArray(typeof(string)), mode);
        }

        /// <summary>
        /// Lookup MemberInfo for the target type based on listed names and selection mode.
        /// </summary>
        /// <param name="target">Target type</param>
        /// <param name="memberNames">List of member names to be tested</param>
        /// <param name="mode">Member selection mode: All, Exclude or Include</param>
        /// <returns>Array of MemberInfo</returns>
        public static MemberInfo[] GetMemberInfo(Type target, string[] memberNames, MemberSelectMode mode)
        {
            MemberInfo[] allMembers = null;

            // Get an array of all members for MemberSelectMode All and Exclude.
            if (mode != MemberSelectMode.Include)
            {
                allMembers = target.GetMembers(BindingFlags.Static |  BindingFlags.Instance | BindingFlags.Public);
            }

            // Just return all public static members for MemberSelectMode All
            if (mode == MemberSelectMode.All)
            {
                return allMembers;
            }

            //
            // Get MemberInfo for all names in the list even if they will be excluded.
            //

            ArrayList memberList = new ArrayList();
            foreach (string name in memberNames)
            {
                MemberInfo[] member = target.GetMember(name);
                if (member == null)
                {
                    throw new Exception("Member for name " + name + " not found on type " + target);
                }

                // Add each member info retrieved for name (methods may be overloaded).
                foreach (MemberInfo m in member)
                {
                    memberList.Add(m);
                }
            }

            // Return all the requested members for MemberSelectMode Include.
            if (mode == MemberSelectMode.Include)
            {
                return (MemberInfo[])memberList.ToArray(typeof(MemberInfo));
            }
            
            //
            // Build list of all members not in user's list.
            //

            // Loop through each member in allMembers, if it is not contained in memberList add it to notExcludedMembers.
            ArrayList notExcludedMembers = new ArrayList();
            foreach (MemberInfo m in allMembers)
            {
                // Add to members if member info is not in memberList.
                if (!memberList.Contains(m))
                {
                    notExcludedMembers.Add(m);
                }
            }

            // Return remainder.
            return (MemberInfo[])notExcludedMembers.ToArray(typeof(MemberInfo));
        }

        /// <summary>
        /// Verify the listed members throw a security exception when used in partial trust.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="targetMembers"></param>
        /// <param name="instance">Nonstatic members require an instance to be executed</param>
        /// <returns></returns>
        public static bool TestMembers(Type t, MemberInfo[] targetMembers, object instance)
        {
            bool passed = true;

            foreach (MemberInfo targetMember in targetMembers)
            {
                // Get no argument constructor for type.
                Type[] w = {};
                ConstructorInfo ctor = t.GetConstructor(
                    BindingFlags.Instance |BindingFlags.Public | BindingFlags.NonPublic,
                    null, w, null);

                // If an instance hasn't already been given and we were able to get constructor info create the instance.
                if (instance == null && ctor != null && ctor != null)
                {
                    CoreLogger.LogStatus("Creating instance of type " + ctor);
                    instance = ctor.Invoke(w);
                }

                // Call appropriate method for testing member type.
                switch (targetMember.MemberType)
                {
                    case MemberTypes.Method:
                        if (!VerifyMethod((MethodInfo)targetMember, instance)) passed = false;
                        break;
                    case MemberTypes.Property:
                        if (!VerifyProperty((PropertyInfo)targetMember, instance)) passed = false;
                        break;
                    case MemberTypes.Event:
                        if (!VerifyEvent((EventInfo)targetMember, instance)) passed = false;
                        break;
                    default:
                        throw new Exception("Target member type of " + targetMember + " not supported.");
                }
            }

            return passed;
        }

        private static bool VerifyProperty(PropertyInfo targetProperty, object instance)
        {
            MethodInfo getMethod = targetProperty.GetGetMethod();
            MethodInfo setMethod = targetProperty.GetSetMethod();

            if ((getMethod == null) && (setMethod == null))
            {
                return false;
            }

            bool getResult = true; 
            bool setResult = true;

            if (getMethod != null)
            {
                getResult = VerifyMethod(getMethod, instance);
            }
            if (setMethod != null)
            {
                setResult = VerifyMethod(setMethod, instance);
            }

            return (getResult && setResult);
        }

        private static bool VerifyEvent(EventInfo targetEvent, object instance)
        {
            MethodInfo addMethod = targetEvent.GetAddMethod();
            MethodInfo removeMethod = targetEvent.GetRemoveMethod();

            if ((addMethod == null) && (removeMethod == null))
            {
                return false;
            }

            bool addResult = true;
            bool removeResult = true;

            if (addMethod != null)
            {
                addResult = VerifyMethod(addMethod, instance);
            }
            if (removeMethod != null)
            {
                removeResult = VerifyMethod(removeMethod, instance);
            }

            return (addResult && removeResult);
        }

        private static bool VerifyMethod(MethodInfo targetMethod, object instance)
        {
            bool caughtException = false;

            // Allocate any parameters the method may need. The types and values do not
            // matter as the method is assumed to throw a security exception before touching them.
            ParameterInfo[] methodParams = targetMethod.GetParameters();
            object[] passedParams = new object[methodParams.Length];
            
            try
            {
                CoreLogger.LogStatus("Invoking " + targetMethod + ", this is expected to throw a security exception.");

                targetMethod.Invoke(instance, passedParams);
            }

            //    
            // Validate result of Invoking method.
            // 
            catch (Exception e)
            {
                caughtException = true;

                // If the attribute is on the method (before the method body) invoking causes a security exception, but
                // if the attribute is in the body we get a SecurityException as the InnerException of a TargetInvocationException.
                if (e is SecurityException || (e is TargetInvocationException && e.InnerException is SecurityException))
                {
                    if (e is TargetInvocationException)
                    {
                        CoreLogger.LogStatus("Caught security exception as expected: " + e.InnerException, ConsoleColor.Green);
                    }
                    else
                    {
                        CoreLogger.LogStatus("Caught security exception as expected: " + e, ConsoleColor.Green);
                    }
                    return true;
                }
                else
                {
                    CoreLogger.LogStatus("Caught the wrong kind of exception. Invoking method " + targetMethod + " is expected to throw a security exception. " + e, ConsoleColor.Red);
                }
            }

            if (!caughtException)
            {
                CoreLogger.LogStatus("Didn't catch an exception. Invoking method " + targetMethod + " is expected to throw a security exception.", ConsoleColor.Red);
            }

            return false;
        }


        /// <summary>
        /// Parse the member selection mode specified in the data file.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static MemberSelectMode ParseSelectionMode(string mode)
        {
            switch (mode)
            {
                case "All":     return MemberSelectMode.All;
                case "Include": return MemberSelectMode.Include;
                case "Exclude": return MemberSelectMode.Exclude;
            }
            throw new Exception("Member selection mode '" + mode + "' not supported.");
        }
    }

    /// <summary>
    /// Selection mode:
    /// All - Reflect the class and get all members.
    /// Exclude - Reflect the class and get all members then remove those listed.
    /// Include - Only use the members listed in the data file.
    /// </summary>
    public enum MemberSelectMode
    {
        /// <summary>
        /// Select all public static members.
        /// </summary>
        All,
        /// <summary>
        /// Include only the listed members.
        /// </summary>
        Include,
        /// <summary>
        /// Select all public static members but exclude those listed.
        /// </summary>
        Exclude
    }
}

