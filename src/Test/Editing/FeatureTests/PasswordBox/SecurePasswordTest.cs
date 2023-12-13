// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Test.Uis.Management;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using System.Windows.Controls;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Loggers;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// Test for SecurePassword property on PasswordBox
    /// </summary>
    [Test(0, "PasswordBox", "SecurePasswordTest", MethodParameters = "/TestCaseType:SecurePasswordTest")]
    [TestOwner("Microsoft"), TestTitle("Secure Password Property Test")]
    public class SecurePasswordTest : CustomTestCase
    {
        private PasswordBox _passwordBox = new PasswordBox();
        private readonly string _password = "hello";

        public override void RunTestCase()
        {
            MainWindow.Content = _passwordBox;
            VerifyPasswordEncryption();
            VerifyPasswordDecryption();
            Logger.Current.ReportSuccess();
        }

        private void VerifyPasswordEncryption()
        {
            _passwordBox.Password = _password;

            //Check if the securepassword.tostring() spits out the password
            Verifier.Verify(_passwordBox.SecurePassword.ToString() != _passwordBox.Password, "Encrypted SecurePassword should never be equal to the password", true);
        }

        private void VerifyPasswordDecryption()
        {
            //check if its possible to convert secure string back to a string
            string decryptedSecureString = StringUtils.GetStringFromSecureString(_passwordBox.SecurePassword);
            Verifier.Verify(decryptedSecureString == _passwordBox.Password, "Decrypted Secure Password and Password should be equal", true);
        }
    }
}