using System;
using System.Collections;
using System.Text ; 
using System.Runtime.InteropServices;

namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  This class encapsulates a Windows .INI file of the format:
    //  [section1]
    //  key1=value
    //  key2=value
    //  [section2]
    //  key1=value
    //  key2=value
    // </desc>
    // </doc>
    public class IniFile
    {
        // <doc>
        // <desc>
        //  The name of the .INI formatted file we are wrapping
        // </desc>
        // </doc>
        private String fileName;

        // <doc>
        // <desc>
        //  The name of the current section we are mucking about in.
        // </desc>
        // <seealso member="GetInt"/>
        // <seealso member="GetSectionStrings"/>
        // <seealso member="GetString"/>
        // <seealso member="SetCurrentSection"/>
        // <seealso member="SetInt"/>
        // <seealso member="SetString"/>
        // </doc>
        private String currentSection;

        // <doc>
        // <desc>
        //  A private buffer used for working space.
        // </desc>
        // </doc>
        private static char[] ca = new char[32767];

        // <doc>
        // <desc>
        //  A private StringBuilder object used for working space.
        // </desc>
        // </doc>
        private static StringBuilder sb = new StringBuilder(32767);

        // <doc>
        // <desc>
        //  Constructs an IniFile object from the specified file name.
        // </desc>
        // <param term="fileName">
        //  The name of the file to open.
        // </param>
        // </doc>
        public IniFile(String fileName)
        {
            this.fileName = fileName;
        }

        // <doc>
        // <desc>
        //  Retrieves the file name that this class is wrapping.
        // </desc>
        // </doc>
        public virtual String FileName
        {
            get
            {
                return fileName;
            }
        }

        // <doc>
        // <desc>
        //  Retrives an integer from the specified key in the current section.
        // </desc>
        // <param term="keyName">
        //  The name of the key to look for
        // </param>
        // <retvalue>
        //  If found, the integer associated with the specified key/section pair; zero otherwise
        // </retvalue>
        // <seealso member="setCurrentSection"/>
        // </doc>
        public int GetInt(String keyName)
        {
            if (currentSection == null)
                throw new Exception("currentSection has not been set");
            return GetInt(currentSection, keyName);
        }
        
        // <doc>
        // <desc>
        //  Retrives an integer from the specified key in the specified section.
        // </desc>
        // <param term="sectionName">
        //  The name of the section to look for the key/value pair
        // </param>
        // <param term="keyName">
        //  The name of the key to look for
        // </param>
        // <retvalue>
        //  If found, the integer associated with the specified key/section pair; zero otherwise
        // </retvalue>
        // </doc>
        public int GetInt(String sectionName, String keyName)
        {
            return GetInt(sectionName, keyName, 0);
        }
        
        // <doc>
        // <desc>
        //  Retrives an integer from the specified key in the specified section.
        // </desc>
        // <param term="sectionName">
        //  The name of the section to look for the key/value pair
        // </param>
        // <param term="keyName">
        //  The name of the key to look for
        // </param>
        // <param term="defaultValue">
        //  The value to return if the key couldn't be found in the specified section
        // </param>
        // <retvalue>
        //  If found, the integer associated with the specified key/section pair; zero otherwise
        // </retvalue>
        // </doc>
        public int GetInt(String sectionName, String keyName, int defaultValue)
        {
            return GetPrivateProfileInt(sectionName, keyName, defaultValue, FileName);
        }

        // <doc>
        // <desc>
        //  Retrives a String array representing the lines of the current section.
        // </desc>
        // <retvalue>
        //  A String array representing the lines of the specified section.
        // </retvalue>
        // <seealso member="setCurrentSection"/>
        // <seealso member="getSectionNames"/>
        // </doc>
        public String[] GetSectionStrings()
        {
            if (currentSection == null)
                throw new Exception("currentSection has not been set");
            return (GetSectionStrings(currentSection));
        }
        
        // <doc>
        // <desc>
        //  Retrives a String array representing the lines of the specified section.
        // </desc>
        // <param term="secionName">
        //  The name of the section to get the strings from
        // </param>
        // <retvalue>
        //  A String array representing the lines of the specified section.
        // </retvalue>
        // <seealso member="setCurrentSection"/>
        // <seealso member="getSectionNames"/>
        // </doc>
        public String[] GetSectionStrings(String sectionName)
        {
            GetPrivateProfileSection(sectionName, ca, ca.Length, FileName);
            return ParseStringList(ca);
        }

        // <doc>
        // <desc>
        //  Retrives a list of sections that are present in the .ini file.
        // </desc>
        // <retvalue>
        //  A string array containing the sections in this file.
        // </retvalue>
        // </doc>
        public String[] GetSectionNames()
        {
            GetPrivateProfileSectionNames(ca, ca.Length, FileName);
            return ParseStringList(ca);
        }

        // <doc>
        // <desc>
        //  Retrieves a String from a key in the current section
        // </desc>
        // <param term="keyName">
        //  The name of the key to look for.
        // </param>
        // <retvalue>
        //  If found, the String corresponding to the section and key supplied; null otherwise.
        // </retvalue>
        // <seealso member="setCurrentSection"/>
        // </doc>
        public String GetString(String keyName)
        {
            if (currentSection == null)
                throw new Exception("currentSection has not been set");
            return GetString(currentSection, keyName);
        }
        
        // <doc>
        // <desc>
        //  Retrieves a String from a key in the specified section
        // </desc>
        // <param term="sectionName">
        //  The name of the section to look for the name/value pair
        // </param>
        // <param term="keyName">
        //  The name of the key to look for.
        // </param>
        // <retvalue>
        //  If found, the String corresponding to the section and key supplied; null otherwise.
        // </retvalue>
        // <seealso member="setCurrentSection"/>
        // </doc>
        public String GetString(String sectionName, String keyName)
        {
            return GetString(sectionName, keyName, null);
        }
        
        // <doc>
        // <desc>
        //  Retrieves a String from a key in the specified section
        // </desc>
        // <param term="sectionName">
        //  The name of the section to look for the name/value pair
        // </param>
        // <param term="keyName">
        //  The name of the key to look for.
        // </param>
        // <param term="defaultValue">
        //  The value to return if the key couldn't be found in the section
        // </param>
        // <retvalue>
        //  If found, the String corresponding to the section and key supplied; null otherwise.
        // </retvalue>
        // <seealso member="setCurrentSection"/>
        // </doc>
        public String GetString(String sectionName, String keyName, String defaultValue)
        {
            GetPrivateProfileString(sectionName, keyName, defaultValue, sb, sb.Capacity, FileName);
            return sb.ToString();
        }

        // <doc>
        // <desc>
        //  Parses through a buffer of characters of the format:
        //  string<null>string<null>string<null><null>
        //  and returns an array of String objects.
        // </desc>
        // <param term="ca">
        //  The character array to parse through.
        // </param>
        // <retvalue>
        //  An array of strings grovelled out of the buffer
        // </retvalue>
        // </doc>
        private static String[] ParseStringList(char[] ca)
        {
            ArrayList l = new ArrayList();
            int index=0;
            while (ca[index] != '\0')
            {
                int i;
                for (i = index; ca[i]!='\0'; i++);
                char[] nca = new char[i - index];
                Array.Copy(ca, index, nca, 0, nca.Length);
                l.Add(new String(nca));
                index = i+1;
            }
            return (String[])l.ToArray(typeof(String));
        }

        // <doc>
        // <desc>
        //  Sets the current section we are dealing with in the .ini file.
        // </desc>
        // <param term="currentSection">
        //  The new current section to deal with.
        // </param>
        // <seealso member="GetInt"/>
        // <seealso member="GetSection"/>
        // <seealso member="GetString"/>
        // <seealso member="SetInt"/>
        // <seealso member="SetSection"/>
        // <seealso member="SetString"/>
        // </doc>
        public void SetCurrentSection(String currentSection)
        {
            this.currentSection = currentSection;
        }

        // <doc>
        // <desc>
        //  Writes an integer to the .ini file under the specified key in the current section.
        // </desc>
        // <param term="keyName">
        //  The name of the key to write the value under
        // </param>
        // <param term="value">
        //  The integer to write
        // </param>
        // </doc>
        public void SetInt(String keyName, int value)
        {
            SetString(keyName, "" + value.ToString());
        }
        
        // <doc>
        // <desc>
        //  Writes an integer to the .ini file under the specified key in the specified section.
        // </desc>
        // <param term="sectionName">
        //  The name of the section to write the name/value pair
        // </param>
        // <param term="keyName">
        //  The name of the key to write the value under
        // </param>
        // <param term="value">
        //  The integer to write
        // </param>
        // </doc>
        public void SetInt(String sectionName, String keyName, int value)
        {
            SetString(sectionName, keyName, "" + value.ToString());
        }

        // <doc>
        // <desc>
        //  Writes an entire section in the current Section in the .ini file.
        // </desc>
        // <param term="strings">
        //  The array of strings to stick in the current section
        // </param>
        // </doc>
        public void SetSectionStrings(String[] strings)
        {
            if (currentSection == null)
                throw new Exception("currentSection has not been set");
            SetSectionStrings(currentSection, strings);
        }
        
        // <doc>
        // <desc>
        //  Writes an entire section in the specified Section in the .ini file.
        // </desc>
        // <param term="sectionName>
        //  The name of the section to write the list of strings
        // </param>
        // <param term="strings">
        //  The array of strings to stick in the current section
        // </param>
        // </doc>
        public void SetSectionStrings(String sectionName, String[] strings)
        {
            // remove the current section (at least on NT it isn't done for you)
            WritePrivateProfileString(sectionName, null, null, FileName);
            
            int bufferLength=1;
            for (int i=0; i<strings.Length; i++)
                bufferLength += strings[i].Length + 1;
            char[] buffer = new char[bufferLength];
            int index=0;
            for (int i=0; i<strings.Length; i++)
            {
                Array.Copy(strings[i].ToCharArray(), 0, buffer, index, strings[i].Length);
                index += strings[i].Length;
                buffer[index++] = '\0';
            }
            buffer[index] = '\0';
            WritePrivateProfileSection(sectionName, buffer, FileName);
        }

        // <doc>
        // <desc>
        //  Writes a String to the .ini file under the specified key in the current section.
        // </desc>
        // <param term="keyName">
        //  The name of the key to write the value under
        // </param>
        // <param term="value">
        //  The String to write
        // </param>
        // <seealso member="SetCurrentSection"/>
        // </doc>
        public void SetString(String keyName, String value)
        {
            if (currentSection == null)
                throw new Exception("currentSection has not been set");
            SetString(currentSection, keyName, value);
        }
        
        // <doc>
        // <desc>
        //  Writes a String to the .ini file under the specified key in the current section.
        // </desc>
        // <param term="sectionName">
        //  The name of the section to write the name/value pair
        // </param>
        // <param term="keyName">
        //  The name of the key to write the value under
        // </param>
        // <param term="value">
        //  The String to write
        // </param>
        // <seealso member="SetCurrentSection"/>
        // </doc>
        public void SetString(String sectionName, String keyName, String value)
        {
            WritePrivateProfileString(sectionName, keyName, value, FileName);
        }
        
        [DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
        private static extern bool WritePrivateProfileSection(String sectionName, char[] data, String fileName);

        [DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
        private static extern bool WritePrivateProfileString(String sectionName, String key, String data, String fileName);
        
        [DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
        private static extern int GetPrivateProfileInt(String sectionName, String keyName, int defaultValue, String fileName);
        
        [DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
        private static extern int GetPrivateProfileString(String sectionName, String keyName, String defaultValue, StringBuilder bufer, int bufferSize, String fileName);
        
        [DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
        private static extern int GetPrivateProfileSection(String sectionName, char[] returnBuffer, int bufferSize, String fileName);

        [DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
        private static extern int GetPrivateProfileSectionNames(char[] returnBuffer, int bufferSize, String fileName);
     }
}
