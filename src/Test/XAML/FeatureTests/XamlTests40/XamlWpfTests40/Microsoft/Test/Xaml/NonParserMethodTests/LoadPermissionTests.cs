// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using System.Xaml;
using System.Xaml.Permissions;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Schema.MethodTests;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.NonParserMethodTests
{
    /// <summary>
    /// Load Permission Tests
    /// </summary>
    public static class LoadPermissionTests
    {
        /// <summary>
        /// Empty Permission
        /// </summary>
        private static XamlLoadPermission s_emptyPermission = new XamlLoadPermission(PermissionState.None);

        /// <summary>
        /// Unrestricted Permission
        /// </summary>
        private static XamlLoadPermission s_unrestrictedPermission = new XamlLoadPermission(PermissionState.Unrestricted);

        /// <summary>
        /// Unrestricted Permissions.
        /// </summary>
        public static void Unrestricted()
        {
            VerifyInvariants(s_unrestrictedPermission, true);
        }

        /// <summary>
        /// Empty Permissions.
        /// </summary>
        public static void Empty()
        {
            VerifyInvariants(s_emptyPermission, false);
            XamlLoadPermission emptyPerm = new XamlLoadPermission(new XamlAccessLevel[0]);
            VerifyInvariants(emptyPerm, false);
            Assert.AreEqual(s_emptyPermission, emptyPerm);
        }

        /// <summary>
        /// Single  assembly Permissions.
        /// </summary>
        public static void SingleAssembly()
        {
            XamlAccessLevel assemblyAccess = XamlAccessLevel.AssemblyAccessTo(typeof(LoadPermissionTests).Assembly);
            XamlLoadPermission assemblyPerm = new XamlLoadPermission(assemblyAccess);
            VerifyInvariants(assemblyPerm, false);

            XamlLoadPermission testPerm, intersect, union;

            GlobalLog.LogStatus("Identical permission");
            XamlAccessLevel sameAssemblyAccess = XamlAccessLevel.AssemblyAccessTo(typeof(LoadPermissionTests).Assembly);
            testPerm = new XamlLoadPermission(sameAssemblyAccess);
            VerifyInvariants(testPerm, false);
            Assert.AreEqual(assemblyPerm, testPerm, "assemblyPerm == TestPerm");
            Assert.IsTrue(testPerm.IsSubsetOf(assemblyPerm), "TestPerm is a subset of assemblyPerm");
            Assert.IsTrue(assemblyPerm.IsSubsetOf(testPerm), "assemblyPerm is a subset of TestPerm");
            intersect = (XamlLoadPermission)testPerm.Intersect(assemblyPerm);
            Assert.AreEqual(assemblyPerm, intersect, "assemblyPerm == (TestPerm intersect assemblyPerm)");
            union = (XamlLoadPermission)testPerm.Union(assemblyPerm);
            Assert.AreEqual(assemblyPerm, union, "assemblyPerm == (TestPerm U assemblyPerm)");
            Assert.AreEqual(null, assemblyPerm.Intersect(null));
            Assert.AreEqual(assemblyPerm, assemblyPerm.Union(null));

            GlobalLog.LogStatus("Identical permission");
            XamlAccessLevel typeAccess = XamlAccessLevel.PrivateAccessTo(typeof(LoadPermissionTests));
            testPerm = new XamlLoadPermission(typeAccess);
            VerifyInvariants(testPerm, false);
            Assert.AreNotEqual(assemblyPerm, testPerm, "assemblyPerm != TestPerm");
            Assert.IsFalse(testPerm.IsSubsetOf(assemblyPerm), "TestPerm is a NOT subset of assemblyPerm");
            Assert.IsTrue(assemblyPerm.IsSubsetOf(testPerm), "assemblyPerm is a subset of TestPerm");
            intersect = (XamlLoadPermission)testPerm.Intersect(assemblyPerm);
            Assert.AreEqual(assemblyPerm, intersect, "assemblyPerm == (TestPerm intersect assemblyPerm)");
            union = (XamlLoadPermission)testPerm.Union(assemblyPerm);
            Assert.AreEqual(testPerm, union, "testPerm == (TestPerm U assemblyPerm)");

            GlobalLog.LogStatus("Identical permission");
            XamlAccessLevel diffAssemblyAccess = XamlAccessLevel.AssemblyAccessTo(typeof(string).Assembly);
            testPerm = new XamlLoadPermission(diffAssemblyAccess);
            VerifyInvariants(testPerm, false);
            Assert.AreNotEqual(assemblyPerm, testPerm, "assemblyPerm != TestPerm");
            Assert.IsFalse(testPerm.IsSubsetOf(assemblyPerm), "TestPerm is a NOT subset of assemblyPerm");
            Assert.IsFalse(assemblyPerm.IsSubsetOf(testPerm), "assemblyPerm is NOT a subset of TypePerm");
            intersect = (XamlLoadPermission)testPerm.Intersect(assemblyPerm);
            Assert.AreEqual(s_emptyPermission, intersect, "emptyPermission == (TestPerm intersect assemblyPerm)");
            union = (XamlLoadPermission)testPerm.Union(assemblyPerm);
            Assert.IsTrue(testPerm.IsSubsetOf(union), "testPerm is a subset of (TestPerm U assemblyPerm)");
            Assert.IsTrue(assemblyPerm.IsSubsetOf(union), "assemblyPerm is a subset of (TestPerm U assemblyPerm)");
        }

        /// <summary>
        /// SingleType Permission test.
        /// </summary>
        public static void SingleType()
        {
            XamlAccessLevel typeAccess = XamlAccessLevel.PrivateAccessTo(typeof(LoadPermissionTests));
            XamlLoadPermission typePerm = new XamlLoadPermission(typeAccess);
            VerifyInvariants(typePerm, false);

            XamlLoadPermission testPerm, intersect, union;

            GlobalLog.LogStatus("Identical permission");
            XamlAccessLevel sameTypeAccess = XamlAccessLevel.PrivateAccessTo(typeof(LoadPermissionTests));
            testPerm = new XamlLoadPermission(sameTypeAccess);
            VerifyInvariants(testPerm, false);
            Assert.AreEqual(typePerm, testPerm, "TypePerm == TestPerm");
            Assert.IsTrue(testPerm.IsSubsetOf(typePerm), "TestPerm is a subset of TypePerm");
            Assert.IsTrue(typePerm.IsSubsetOf(testPerm), "TypePerm is a subset of TestPerm");
            intersect = (XamlLoadPermission)testPerm.Intersect(typePerm);
            Assert.AreEqual(typePerm, intersect, "TypePerm == (TestPerm intersect TypePerm)");
            union = (XamlLoadPermission)testPerm.Union(typePerm);
            Assert.AreEqual(typePerm, union, "TypePerm == (TestPerm U TypePerm)");

            GlobalLog.LogStatus("Assembly permission to the same assembly");
            XamlAccessLevel assemblyAccess = XamlAccessLevel.AssemblyAccessTo(typeof(LoadPermissionTests).Assembly);
            testPerm = new XamlLoadPermission(assemblyAccess);
            VerifyInvariants(testPerm, false);
            Assert.AreNotEqual(typePerm, testPerm, "TypePerm != TestPerm");
            Assert.IsTrue(testPerm.IsSubsetOf(typePerm), "TestPerm is NOT subset of TypePerm");
            Assert.IsFalse(typePerm.IsSubsetOf(testPerm), "TypePerm is NOT subset of TestPerm");
            intersect = (XamlLoadPermission)testPerm.Intersect(typePerm);
            Assert.AreEqual(testPerm, intersect, "TypePerm == (TestPerm intersect TypePerm)");
            union = (XamlLoadPermission)testPerm.Union(typePerm);
            Assert.AreEqual(typePerm, union, "TypePerm == (TestPerm U TypePerm)");

            GlobalLog.LogStatus("Type permission on different type in same assembly");
            XamlAccessLevel sameAsmTypeAccess = XamlAccessLevel.PrivateAccessTo(typeof(XamlLanguageTests));
            testPerm = new XamlLoadPermission(sameAsmTypeAccess);
            VerifyInvariants(testPerm, false);
            Assert.AreNotEqual(typePerm, testPerm, "TypePerm != TestPerm");
            Assert.IsFalse(testPerm.IsSubsetOf(typePerm), "TestPerm is NOT subset of TypePerm");
            Assert.IsFalse(typePerm.IsSubsetOf(testPerm), "TypePerm is NOT subset of TestPerm");
            intersect = (XamlLoadPermission)testPerm.Intersect(typePerm);
            XamlLoadPermission assemblyPerm = new XamlLoadPermission(assemblyAccess);
            Assert.AreEqual(assemblyPerm, intersect, "assemblyPerm == (TestPerm intersect TypePerm)");
            union = (XamlLoadPermission)testPerm.Union(typePerm);
            Assert.IsTrue(testPerm.IsSubsetOf(union), "TestPerm == (TestPerm U TypePerm)");
            Assert.IsTrue(typePerm.IsSubsetOf(union), "TypePerm == (TestPerm U TypePerm)");
            Assert.IsTrue(assemblyPerm.IsSubsetOf(union), "assemblyPerm == (TestPerm U TypePerm)");

            GlobalLog.LogStatus("Type permission in different assembly");
            XamlAccessLevel diffTypeAccess = XamlAccessLevel.PrivateAccessTo(typeof(string));
            testPerm = new XamlLoadPermission(diffTypeAccess);
            VerifyInvariants(testPerm, false);
            Assert.AreNotEqual(typePerm, testPerm, "TypePerm != TestPerm");
            Assert.IsFalse(testPerm.IsSubsetOf(typePerm), "TestPerm is NOT subset of TypePerm");
            Assert.IsFalse(typePerm.IsSubsetOf(testPerm), "TypePerm is NOT subset of TestPerm");
            intersect = (XamlLoadPermission)testPerm.Intersect(typePerm);
            Assert.AreEqual(s_emptyPermission, intersect, "emptyPermission == (TestPerm intersect TypePerm)");
            union = (XamlLoadPermission)testPerm.Union(typePerm);
            Assert.IsTrue(testPerm.IsSubsetOf(union), "TestPerm == (TestPerm U TypePerm)");
            Assert.IsTrue(typePerm.IsSubsetOf(union), "TypePerm == (TestPerm U TypePerm)");
        }

        /// <summary>
        /// Accesslevel ctor is positive.
        /// </summary>
        public static void AccessLevelCtorsPositive()
        {
            XamlAccessLevel accessByRef = XamlAccessLevel.AssemblyAccessTo(typeof(LoadPermissionTests).Assembly);
            XamlAccessLevel accessByName = XamlAccessLevel.AssemblyAccessTo(typeof(LoadPermissionTests).Assembly.GetName());
            Assert.AreEqual(new XamlLoadPermission(accessByRef), new XamlLoadPermission(accessByName));

            accessByRef = XamlAccessLevel.PrivateAccessTo(typeof(LoadPermissionTests));
            accessByName = XamlAccessLevel.PrivateAccessTo(typeof(LoadPermissionTests).AssemblyQualifiedName);
            Assert.AreEqual(new XamlLoadPermission(accessByRef), new XamlLoadPermission(accessByName));
        }

        /// <summary>
        /// Accesslevel ctor has unqualified assembly name.
        /// </summary>
        public static void AccessLevelCtorUnqualifiedAssemblyName()
        {
            ExceptionHelper.ExpectException(
                delegate { XamlAccessLevel.AssemblyAccessTo(new AssemblyName("XamlWpfTests40")); },
                new ArgumentException(String.Empty, "assemblyName", null));
        }

        /// <summary>
        /// Accesslevel ctor has unqualified Type name.
        /// </summary>
        public static void AccessLevelCtorUnqualifiedTypeName()
        {
            ExceptionHelper.ExpectException(
                delegate { XamlAccessLevel.PrivateAccessTo(typeof(LoadPermissionTests).FullName); },
                new ArgumentException(String.Empty, "assemblyQualifiedTypeName", null));
        }

        /// <summary>
        /// Verifies the invariants.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="isUnrestricted">if set to <c>true</c> [is unrestricted].</param>
        private static void VerifyInvariants(XamlLoadPermission original, bool isUnrestricted)
        {
            Assert.AreEqual(original, original, "Compare XamlLoadPermission to itself");
            Assert.AreEqual(isUnrestricted, original.IsUnrestricted(), "Compare XamlLoadPermission.IsUnrestricted. Expected[" + isUnrestricted + "] Actual[" + original.IsUnrestricted() + "]");

            XamlLoadPermission copy = (XamlLoadPermission)original.Copy();
            Assert.AreEqual(original, copy, "Compare Original To a Copy");
            Assert.AreEqual(original.IsUnrestricted(), copy.IsUnrestricted(), "Compare IsUnrestricted to that of a copy");
            Assert.IsTrue(original.IsSubsetOf(copy), "Verify Original is a subset of the copy");
            Assert.IsTrue(copy.IsSubsetOf(original), "Verify copy is a subset of Original");

            XamlLoadPermission xmlCopy = new XamlLoadPermission(PermissionState.None);
            xmlCopy.FromXml(original.ToXml());
            Assert.AreEqual(original, xmlCopy, "Compare Original to XmlCopy");

            var intersect = (XamlLoadPermission)original.Intersect(copy);
            Assert.AreEqual(original, intersect, "Compare Original to the intersect of Original and Copy");
            var union = (XamlLoadPermission)original.Union(copy);
            Assert.AreEqual(original, union, "Compare Original to the Union of Original and Copy");

            intersect = (XamlLoadPermission)original.Intersect(s_emptyPermission);
            Assert.AreEqual(s_emptyPermission, intersect, "Compare Original to the Intersect of Original and EmptyPermission");
            union = (XamlLoadPermission)original.Union(s_emptyPermission);
            Assert.AreEqual(original, union, "Compare Original to the Union of Original and EmptyPermission");

            intersect = (XamlLoadPermission)original.Intersect(s_unrestrictedPermission);
            Assert.AreEqual(original, intersect, "Compare Original to the Intersect of Original and UnrestrictedPermission");
            union = (XamlLoadPermission)original.Union(s_unrestrictedPermission);
            Assert.AreEqual(s_unrestrictedPermission, union, "Compare Original to the Union of Original and UnrestrictedPermission");
        }
    }
}
