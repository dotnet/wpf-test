using System;
using System.IO;
using System.Windows;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    internal static class FolderDialogVerifyHelper
    {
        internal static bool CheckSelectedFolders(string[] fileNames, string[] expectedFileNames)
        {
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
    }
}