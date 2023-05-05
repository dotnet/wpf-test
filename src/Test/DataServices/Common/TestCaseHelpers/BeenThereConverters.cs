// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using Microsoft.Test;
using Microsoft.Test.Data;
using System.Globalization;
using System.Security;
using System.Reflection;
using Microsoft.Test.DataServices;
using System.Collections;

namespace Microsoft.Test.DataServices
{
    public class BeenThereConverterClr : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CountryWithExtraInfo country = (CountryWithExtraInfo)value;
            string countryName = country.CountryName;
            return BeenThereGroups.GetGroups(countryName, country);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class BeenThereConverterXml : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            XmlElement country = (XmlElement)value;
            XmlNode countryNameNode = country.FirstChild;
            string countryName = countryNameNode.InnerText;
            return BeenThereGroups.GetGroups(countryName, country);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public static class BeenThereGroups
    {
        public static object GetGroups(string countryName, object country)
        {
            ArrayList collection = new ArrayList();
            switch (countryName)
            {
                case "Yemen":
                    collection.Add(BeenThere.BeenThereIn1980s);
                    return collection;
                case "Qatar":
                    collection.Add(BeenThere.BeenThereIn1990s);
                    collection.Add(BeenThere.BeenThereIn2000s);
                    return collection;
                case "Saint Lucia":
                    return null;
                case "San Marino":
                    return countryName;
                case "Turkmenistan":
                    return BeenThere.NeverBeenThere;
                case "Moldova":
                    collection.Add(BeenThere.BeenThereIn1980s);
                    collection.Add(BeenThere.BeenThereIn2000s);
                    return collection;
                case "Nauru":
                    return country;
                case "Sierra Leone":
                    return BeenThere.NeverBeenThere;
                case "Kyrgyzstan":
                    return BeenThere.NeverBeenThere;
                case "Malawi":
                    return BeenThere.NeverBeenThere;
                case "Djibouti":
                    collection.Add(BeenThere.BeenThereIn1980s);
                    collection.Add(BeenThere.BeenThereIn1990s);
                    collection.Add(BeenThere.BeenThereIn2000s);
                    return collection;
                case "East Timor":
                    collection.Add(null);
                    return collection;
                case "Armenia":
                    collection.Add(country);
                    return collection;
                case "El Salvador":
                    collection.Add(countryName);
                    return collection;
                case "Guyana":
                    return BeenThere.NeverBeenThere;
                case "Mexico":
                    return BeenThere.NeverBeenThere;
                case "New country":
                    collection.Add(BeenThere.BeenThereIn1980s);
                    collection.Add(BeenThere.BeenThereIn1990s);
                    collection.Add(BeenThere.BeenThereIn2000s);
                    return collection;
            }
            return collection;
        }
    }

    public class PopulationConverterClr : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CountryWithExtraInfo country = (CountryWithExtraInfo)value;
            int population = country.Population;
            if (population < 1000000)
            {
                return "Small";
            }
            else if (population < 10000000)
            {
                return "Medium";
            }
            else
            {
                return "Large";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class PopulationConverterXml : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            XmlElement country = (XmlElement)value;
            int population = Int32.Parse(country["Population"].InnerText);
            if (population < 1000000)
            {
                return "Small";
            }
            else if (population < 10000000)
            {
                return "Medium";
            }
            else
            {
                return "Large";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class FirstLetterConverterClr : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CountryWithExtraInfo country = (CountryWithExtraInfo)value;
            return ((country.CountryName)[0]).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class FirstLetterConverterXml : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            XmlElement country = (XmlElement)value;
            return ((country["CountryName"].InnerText)[0]).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}