// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/****************************************************
* This file tests DataFormats.GetFormat(int Name)
* Where IDs are from 1 to 16
* Command Line: DataFormatsGetFormatWithInt.exe /TestCaseType=DataFormatsGetFormatWithInt /Name=1 to 16
*/

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/forms/TestCases/DataTransfer/DataObject/DataFormatsGetFormatWithInt.cs $")]

namespace DataTransfer
{
    #region Namespaces.

    using System;
    using System.Windows;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.TestTypes;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>Provides information about data transfer formats.</summary>
    class DataFormatData
    {
        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private DataFormatData()
        {
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Finds the DataFormatData for the specified format ID.</summary>
        /// <param name='formatId'>ID to look for.</param>
        /// <returns>The DataFormatData for the specified format ID, null if not found.</returns>
        public static DataFormatData FindById(int formatId)
        {
            foreach(DataFormatData result in Values)
            {
                if (result.DataFormatId == formatId)
                {
                    return result;
                }
            }
            return null;
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Unique, system-assigned or well-known data format identifier.</summary>
        public int DataFormatId
        {
            get { return this._dataFormatId; }
        }

        /// <summary>Name for data format.</summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>Whether this is a well-known system format.</summary>
        public bool IsSystemFormat
        {
            get { return this._isSystemFormat; }
        }

        /// <summary>Provides values for known data formats.</summary>
        public static DataFormatData[] Values = new DataFormatData[] {
            ForSystem(1, "Text"),
            ForSystem(2, "Bitmap"),
            ForSystem(3, "MetaFilePict"),
            ForSystem(4, "SymbolicLink"),
            ForSystem(5, "DataInterchangeFormat"),
            ForSystem(6, "TaggedImageFileFormat"),
            ForSystem(7, "OEMText"),
            ForSystem(8, "DeviceIndependentBitmap"),
            ForSystem(9, "Palette"),
            ForSystem(10, "PenData"),
            ForSystem(11, "RiffAudio"),
            ForSystem(12, "WaveAudio"),
            ForSystem(13, "UnicodeText"),
            ForSystem(14, "EnhancedMetafile"),
            ForSystem(15, "FileDrop"),
            ForSystem(16, "Locale"),
        };

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Creates a new DataFormatData instance for the given values.
        /// </summary>
        private static DataFormatData ForSystem(int id, string name)
        {
            DataFormatData result;

            result = new DataFormatData();
            result._dataFormatId = id;
            result._name = name;
            result._isSystemFormat = true;

            return result;
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Unique, system-assigned or well-known data format identifier.</summary>
        private int _dataFormatId;

        /// <summary>Name for data format.</summary>
        private string _name;

        /// <summary>Whether this is a well-known system format.</summary>
        private bool _isSystemFormat;

        #endregion Private fields.
    }

    /// <summary>
    /// DataFormatsGetFormatWithInt
    /// </summary>
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt1", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=1")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt2", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=2")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt3", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=3")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt4", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=4")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt5", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=5")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt6", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=6")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt7", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=7")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt8", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=8")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt9", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=9")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt10", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=10")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt11", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=11")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt12", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=12")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt12", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=12")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt14", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=14")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt15", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=15")]
    [Test(2, "DataObject", "DataFormatsGetFormatWithInt16", MethodParameters = "/TestCaseType=DataFormatsGetFormatWithInt /ID=16")]
    public class DataFormatsGetFormatWithInt : CustomTestCase
    {
        #region Main flow.

        /// <summary>
        /// Entry point
        /// </summary>
        public override void RunTestCase ()
        {
            // This version checks a single format.
            /*
            int formatId;
            formatId = ConfigurationSettings.Current.GetArgumentAsInt("ID", true);
            pass = CheckFormat(formatId);
            Logger.Current.ReportResult (pass, "The test is : " + pass + " in ID = " + formatId);
            */

            // This version checks all formats.
            _pass = true;
            foreach(DataFormatData formatData in DataFormatData.Values)
            {
                _pass = CheckFormat(formatData.DataFormatId) && _pass;
            }
            Logger.Current.ReportResult(_pass, "The test passed: " + _pass);

        }

        private bool CheckFormat(int formatId)
        {
            DataFormatData formatData;
            DataFormat format;

            format = DataFormats.GetDataFormat(formatId);

            Logger.Current.Log("System Format is: " + format.Id);
            Logger.Current.Log("System Format is: " + format.Name);

            formatData = DataFormatData.FindById(formatId);
            if (formatData == null)
            {
                Logger.Current.Log("Cannot find format in well-known data formats.");
                return false;
            }
            else
            {
                Logger.Current.Log("Expected format id and name: " +
                    formatData.DataFormatId + " - " + formatData.Name);
                return (format.Id == formatData.DataFormatId) &&
                    (format.Name == formatData.Name);
            }
        }

        #endregion Main flow.

        #region Private fields.

        private bool _pass = false;

        #endregion Private fields.
    }
}