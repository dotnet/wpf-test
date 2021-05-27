// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//using System.Collections.Generic;
//using System.Text;
//using System.Runtime.InteropServices;
//using System.Text.RegularExpressions;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;

//namespace Microsoft.Test.Diagnostics
//{
//    /// <summary>
//    /// Helper class that allows to retrieve information for Wtt
//    /// </summary>
//    internal static class WttHelper
//    {
//        //


//        /// <summary>
//        /// Retrieves the complete login information given a username and domain
//        /// </summary>
//        /// <param name="username">Username to get</param>
//        /// <param name="domain">Domain of the user</param>
//        /// <param name="userLoginInfo">The structure to be filled</param>
//        /// <returns>True if the structure is filled properly, false otherwise.</returns>
//        public static bool TryGetUserInfo(string username, string domain, out UserInfo userLoginInfo)
//        {
//            userLoginInfo = new UserInfo();
//            List<UserInfo> users = GetLLUUsers();

//            if (users == null || users.Count == 0)
//                return false;

//            foreach (UserInfo user in users)
//            {
//                if (String.Equals(username, user.Username, StringComparison.InvariantCultureIgnoreCase) &&
//                    String.Equals(domain, user.Domain, StringComparison.InvariantCultureIgnoreCase) &&
//                    !String.IsNullOrEmpty(UserSessionHelper.TryGetUserSid(username, domain)))
//                {
//                    userLoginInfo = user;
//                    return true;
//                }
//            }

//            return false;
//        }

//        #endregion

//        #region Private Members

//        // Retrieves a list of LLU users on the system
//        private static List<UserInfo> GetLLUUsers()
//        {
//            List<UserInfo> users = new List<UserInfo>();
//            string output = ExecuteWttCmd("/QueryLogicalUser");

//            if (String.IsNullOrEmpty(output))
//                return null;

//            //Output from WTT Cmd:
//            //
//            //The Following LLUs (Local Logical User) are configured on the system.
//            //     "Admin" == REDMOND\IEAL

//            Regex regex = new Regex(@"^\s+""(?<llu>.+)""\s+==\s(?<lluusername>.+)\r\n", RegexOptions.Multiline);
//            char[] chars = new char[] { '\\' };
//            foreach (Match match in regex.Matches(output))
//            {
//                string[] usernameAndDomain = match.Groups["lluusername"].Value.Trim().Split(chars, StringSplitOptions.RemoveEmptyEntries);
//                string llu = match.Groups["llu"].Value.Trim();

//                UserInfo userData = new UserInfo();
//                userData.LLUName = llu;
//                if (usernameAndDomain.Length == 1)
//                    userData.Username = usernameAndDomain[0];
//                else if (usernameAndDomain.Length == 2)
//                {
//                    userData.Domain = usernameAndDomain[0];
//                    userData.Username = usernameAndDomain[1];
//                }
//                userData.Password = GetLLUPassword(userData.LLUName);
//                users.Add(userData);
//            }

//            return users;
//        }

//        //Retrives the account password given an LLU name
//        private static string GetLLUPassword(string lluName)
//        {
//            //We don't know which LLU to use
//            if (String.IsNullOrEmpty(lluName))
//                return null;

//            string output = ExecuteWttCmd("/GetLogicalUser /LocalName:\"" + lluName + "\"");

//            //Parse the output of wtt cmd
//            //
//            //        LLU Name        : Admin.DomUser
//            //        UserName        : REDMOND\ieal
//            //        Password        : ID.#0601
//            //
//            Regex regex = new Regex(@"^\s+(?<name>.+)\s+:.(?<value>.+) \r\n", RegexOptions.Multiline);
//            foreach (Match match in regex.Matches(output))
//            {
//                string name = match.Groups["name"].Value.Trim();
//                string value = match.Groups["value"].Value;
//                if (name.ToLowerInvariant() == "password")
//                    return value;
//            }

//            return null;
//        }

//        private static string ExecuteWttCmd(string argument)
//        {
//            //HACK: WTT has no API for this so I have to use wttcmd.exe
//            ProcessStartInfo info = new ProcessStartInfo("wttcmd.exe", argument);
//            info.UseShellExecute = false;
//            info.RedirectStandardOutput = true;
//            info.CreateNoWindow = true;

//            //HACK: In order to prevent wttcmd from logging results to wtt we make sure its log files go
//            //      somwhere wtt wont find them and we trick wttcmd to think we are not running as a task
//            info.WorkingDirectory = Path.GetTempPath();
//            info.EnvironmentVariables.Remove("WttTaskGuid");

//            Process process;
//            try
//            {
//                process = Process.Start(info);
//            }
//            catch (Win32Exception)
//            {
//                //wtt client is not installed (cant find wttcmd.exe)
//                return null;
//            }

//            string output = process.StandardOutput.ReadToEnd();
//            if (process.ExitCode != 0)
//            {
//                //May be a bad LLU name or something bad happened
//                return null;
//            }

//            return output;
//        }

//        #endregion
//    }
//}
