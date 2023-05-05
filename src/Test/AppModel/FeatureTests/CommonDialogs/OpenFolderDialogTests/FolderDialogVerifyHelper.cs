using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    internal static class FolderDialogVerifyHelper
    {
        internal static bool CheckSelectedFolders(string[] fileNames, string[] expectedFileNames)
        {
            int length = fileNames.Length;
            if(length != expectedFileNames.Length)
            {
                return false;
            }

            for(int i=0;i<length;i++)
            {
                if(!string.Equals(fileNames[i], expectedFileNames[i]))
                {
                    GlobalLog.LogEvidence(string.Format("FileName : {0} does not match expectedFileName : {1}", fileNames[i], expectedFileNames[i]));
                    return false;
                }
            }
            return true;
        }

        internal static void CreateTestFoldersStructure(string rootDir)
        {
            string testDataDir = Path.Combine(rootDir, "testdata");
            string[] testDataChildDirs = new string[] {"dir1", ".dir2", "dir3"}; 
            Directory.CreateDirectory(testDataDir);
            foreach(string childDir in testDataChildDirs)
            {
                string pathName = Path.Combine(testDataDir, childDir);
                Directory.CreateDirectory(pathName);
            }
        }

        internal static string[] GetFileNamesFromSafeFileNames(string parentDir, string[] safeFileNames)
        {
            List<string> fileNames = new List<string>();

            if(safeFileNames is null || safeFileNames.Length == 0)
            {
                return fileNames.ToArray();
            }

            foreach(string safeFileName in safeFileNames)
            {
                fileNames.Add(Path.Combine(parentDir, safeFileName));
            }

            return fileNames.ToArray();
        }
    }
}