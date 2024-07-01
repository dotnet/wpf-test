// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using System.Windows.Ink;
using System.Windows;
using System.IO;

namespace DRT
{
	/// <summary>
	/// Summary description for DrtHelpers.
	/// </summary>
	public static class DrtHelpers
	{
        static SafeFileLoader FileLoader = new SafeFileLoader();

        public static String TestDirectory = @"DrtFiles\Ink\";

        public static bool ComparePoint(Point a, Point b)
        {
            const double DBL_EPSILON  =   2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
            // Mean squared magnitude of point coefficients.
            double mag = 0.25 * (a.X*a.X+
                                 a.Y*a.Y+
                                 b.X*b.X+
                                 b.Y*b.Y);
            if (mag == 0)
                return true;
            
            // Allowing magnitude * DBL_EPSILON * 10 error, but squared
            double maxError = mag * DBL_EPSILON * DBL_EPSILON * 100;
            Vector delta = a - b;
            return delta.LengthSquared < maxError;
        }
            
        public static bool CompareStrokes(Stroke sSrc, Stroke sDst, bool bComparePoints)
        {
            int nPoints = sSrc.StylusPoints.Count;

            if (nPoints != sDst.StylusPoints.Count)
                return false;

            if (sSrc.DrawingAttributes != sDst.DrawingAttributes)
                return false;

            bool bRetCode = true;

            if (bComparePoints)
            {
                StylusPointCollection ptSrc = sSrc.StylusPoints;
                StylusPointCollection ptDst = sDst.StylusPoints;
                int i = 0;

                nPoints = ptSrc.Count;
                for (; i < nPoints; ++i)
                    if (!StylusPoint.Equals(ptSrc[i],ptDst[i]))
                        break;

                bRetCode = (i == nPoints);
            }

            return bRetCode;
        }
        public static bool CompareStrokeCollections(StrokeCollection strokesSource, StrokeCollection strokesDestination, bool bComparePoints)
        {
            int nStrokes = strokesSource.Count;

            if (nStrokes != strokesDestination.Count)
                return false;

            int i = 0;

            for (; i < nStrokes; ++i)
            {
                Stroke sSrc = strokesSource[i];
                Stroke sDst = strokesDestination[i];

                if (!DrtHelpers.CompareStrokes(sSrc, sDst, bComparePoints))
                    break;
            }

            return (i == nStrokes);
        }

        public static StrokeCollection LoadInk(String filename)
        {
            byte[] data = FileLoader.RetrieveBytes(TestDirectory + filename);

            StrokeCollection result = null;
            
            try 
            {
                result = new StrokeCollection(new MemoryStream(data));
            }
            catch(ArgumentException e)
            {
                if(!IsPreWindows7OS() && IsGIFData(data))
                {
                    // Report the failure but ignore the result in order to continue running other tests
                    Console.WriteLine(e);     
                    
                    // pretend that the file loaded consisted of an empty stroke collection
                    result = new StrokeCollection();
                }
                else
                {
                    throw;
                }
            }
            
            return result;
        }

        static public void CompareArray<ItemType>(ItemType[] first, ItemType[] second)
        {
            if (first.Length != second.Length)
            {
                throw new InvalidOperationException("Arrays do not have equal lengths");
            }
            else
            {
                for (int i = 0; i < first.Length; i++)
                {
                    if (!first[i].Equals(second[i]))
                    {
                        throw new InvalidOperationException("Array item does not match");
                    }
                }
            }
        }

        static public void ComparePoints(Point[] first, Point[] second)
        {
            if (first.Length != second.Length)
            {
                throw new InvalidOperationException("Arrays do not have equal lengths");
            }
            else
            {
                for (int i = 0; i < first.Length; i++)
                {
                    if (!ComparePoint(first[i],second[i]))
                    {
                        throw new InvalidOperationException("Array item does not match");
                    }
                }
            }
        }
        
        static private bool IsPreWindows7OS()
        {
            OperatingSystem os = Environment.OSVersion;
            if(os.Platform != PlatformID.Win32NT)
            {
                throw new NotSupportedException(string.Format("{0} is not a know supported WPF platform", os.Platform));
            }
            
            Version firstWin7Verision = new Version(6, 1, 7600, 0);
            return os.Version < firstWin7Verision;
        }
        
        static private bool IsGIFData(byte [] inkData)
        {
            if(HasBase64Header(inkData))
            {
                inkData = Base64Decode(inkData);
            }
            
            if(inkData.Length >= 3)
            {
                return inkData[0] == 'G' && inkData[1] == 'I' && inkData[2] == 'F';
            }
            else 
            {
                return false;
            }            
        }
        

        private static readonly byte [] Base64Header = new byte[] { 0x62, 0x61, 0x73, 0x65, 0x36, 0x34, 0x3a };
        
        private static bool HasBase64Header(byte [] inkData)
        {

            if(inkData.Length < Base64Header.Length)
            {
                return false;
            }
            
            for(int i = 0; i < Base64Header.Length; i++)
            {
                if(inkData[i] != Base64Header[i])
                {
                    return false;
                }
            }
            
            return true;
        }
        
        static private byte[] Base64Decode(byte [] inkData)
        {
            int inkDataLength;
            
            // Ignore trailing 0 byte in ink data
            if(inkData.Length > 0 && (inkData[inkData.Length - 1] == 0))
            {
                inkDataLength = inkData.Length - 1;
            }
            else 
            {
                inkDataLength = inkData.Length;
            }
            
            // The actual base64 payload occurs immediately after the base 64 header
            char [] result = new char[inkDataLength - Base64Header.Length];
            
            for(int i = Base64Header.Length; i < inkDataLength; i++)
            {
                result[i - Base64Header.Length] = (char)inkData[i];
            }
                
            return Convert.FromBase64CharArray(result, 0, result.Length);
        }
    }
    
    public class SafeFileLoader : MarshalByRefObject
    {
        public byte[] RetrieveBytes (string filename)
        {
            byte[] data;
            using (FileStream fs = File.OpenRead (filename))
            {
                data = new byte[fs.Length];
#pragma warning disable CA2022 // Avoid inexact read
                fs.Read (data, 0, (int)fs.Length);
#pragma warning restore CA2022
            }
            return data;
        }
    }
}
