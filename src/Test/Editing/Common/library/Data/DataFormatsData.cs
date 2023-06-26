// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data for DataFormats supported by Avalon

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Threading;
    using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Animation;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Provides information about interesting DataFormats.
    /// </summary>
    public sealed class DataFormatsData
    {

        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private DataFormatsData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>Overload to ToString</summary>
        /// <returns>String representation of this object</returns>
        public override string ToString()
        {
            return DataFormatString;
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>
        /// Value of DataFormat being encapsulated
        /// </summary>
        public string DataFormatString
        {
            get
            {
                return _dataFormatString;
            }
        }

        /// <summary>Interesting DataFormatsData values for testing.</summary>
        public static DataFormatsData[] Values = new DataFormatsData[] {            
            FromString(DataFormats.Bitmap),
            FromString(DataFormats.CommaSeparatedValue),
            FromString(DataFormats.Dib),
            FromString(DataFormats.Dif),
            FromString(DataFormats.EnhancedMetafile),
            FromString(DataFormats.FileDrop),
            FromString(DataFormats.Html),
            FromString(DataFormats.Locale),
            FromString(DataFormats.MetafilePicture),
            FromString(DataFormats.OemText),
            FromString(DataFormats.Palette),
            FromString(DataFormats.PenData),
            FromString(DataFormats.Riff),
            FromString(DataFormats.Rtf),
            FromString(DataFormats.Serializable),
            FromString(DataFormats.StringFormat),
            FromString(DataFormats.SymbolicLink),
            FromString(DataFormats.Text),
            FromString(DataFormats.Tiff),
            FromString(DataFormats.UnicodeText),
            FromString(DataFormats.WaveAudio),
            FromString(DataFormats.Xaml),
            FromString(DataFormats.XamlPackage),   
        };

        /// <summary>DataFormatsData values which are supported by TextRange.CopyTo/CopyFrom.</summary>
        public static DataFormatsData[] TRSupportedValues = new DataFormatsData[] {                        
            FromString(DataFormats.Text),                                    
            FromString(DataFormats.Xaml),
            FromString(DataFormats.Rtf),
            FromString(DataFormats.XamlPackage),               
        };

        /// <summary>DataFormatsData values which are NOT supported by TextRange.CopyTo/CopyFrom.</summary>
        public static DataFormatsData[] TRUnSupportedValues = new DataFormatsData[] {                        
            FromString(DataFormats.Bitmap),
            FromString(DataFormats.CommaSeparatedValue),
            FromString(DataFormats.Dib),
            FromString(DataFormats.Dif),
            FromString(DataFormats.EnhancedMetafile),
            FromString(DataFormats.FileDrop),
            FromString(DataFormats.Html),
            FromString(DataFormats.Locale),
            FromString(DataFormats.MetafilePicture),
            FromString(DataFormats.OemText),
            FromString(DataFormats.Palette),
            FromString(DataFormats.PenData),
            FromString(DataFormats.Riff),            
            FromString(DataFormats.Serializable),
            FromString(DataFormats.StringFormat),
            FromString(DataFormats.SymbolicLink),            
            FromString(DataFormats.Tiff),
            FromString(DataFormats.UnicodeText),
            FromString(DataFormats.WaveAudio),                           
        };

        #endregion Public properties.


        #region Private methods.

        private static DataFormatsData FromString(string dataFormatString)
        {
            DataFormatsData result = new DataFormatsData();
            result._dataFormatString = dataFormatString;
            return result;
        }

        #endregion Private methods.


        #region Private fields.

        private string _dataFormatString;

        #endregion Private fields.
    }
}