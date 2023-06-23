using System;
using System.Resources;
using System.IO;
using System.Globalization;
using System.Text;
using System.Collections ;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;

namespace GenStrings
{
    [FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
    public class IntlStrings
    {        
        //Default locale ID, used to verify the current LCID.
        private const int TEMP_LCID = 0 ;

        //Current LCID, this LCID is used to retrieve the information from the resource files.
        private int iCurrentLCID = TEMP_LCID; 

        //Random generator used to generate the random numbers.
        private static Random rand ;

        // All the information about the current LocaleID stored in this variable. This helps to improve the 
        // perfomance because we don't wnat to retrieve the resource information every time when user calls
        // a method in IntlStrings.
        private static String strCodePageInfo ;

        //Keeps an instance of NumberFormatInfo specific to the current LocaleID.
        private static NumberFormatInfo nfiCurrent = null ;

        //Postfix string used to load the Resoure file.
        private static String RESOURCE_STRING_POSTFIX = "_Text" ;

        //Invlaid characters that you can't use in file names.
        private static Char[] cInvalidFileChars = new Char[] { '|', '\\', '/', ':', '*', '?', '\"', '<', '>' };

        // This stores information about the valid character range for different languages. Information in this struct 
        // helps to generate the ANSI characters for any given code page. 
        private LeadByteType[] LeadByteRange = new LeadByteType[4];

        // To represent single byte character type.
        private const int SINGLES = 1;

        // To represent double byte character type.
        private const int DOUBLES = 2 ;

        //to represent current code page location to retrieve the bytes. 
        private int intCPPoint = 0 , intCPPoint2  = 0;

        // Off set number that we have to use while generating the ANSI characters.
        private int intCharOffset = 0;

        private int iDefaultStrLength = 30 ;

        private const int LOCALE_IDEFAULTANSICODEPAGE = 0x1004 ;

        private const int VALID_PATH_LENGTH = 258 ;
   
        //*-------------------------------------------------------------------------------------------------
        //    Name           : IntlStrings ( constructor )
        //    Purpose        : Creates a random genrator and loads the default resources.
        //*-------------------------------------------------------------------------------------------------
        public IntlStrings() :this( (long)DateTime.Now.Ticks , TEMP_LCID ){
        }

        public IntlStrings( long lSeed ) : this ( lSeed , TEMP_LCID ){  
        }

        public IntlStrings( int iLCID ) : this ( (long)DateTime.Now.Ticks , iLCID ){  
        }

        public IntlStrings(long lSeed, int iLCID){                                
            //Set the trace listener to write the output to console and to a file.
            Trace.Listeners.Add( new TextWriterTraceListener( Console.Out ) );
            StreamWriter sw= new StreamWriter(File.Open("GenStringsDebug.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite));
            sw.AutoFlush = true ;
            Trace.Listeners.Add( new TextWriterTraceListener( sw ));
            
            //Create a random object.
            rand = new Random( (int)lSeed );
            
            //Output the random seed, user may need this to repro a testcase with the same seed.
            Debug.WriteLine("Random seed :: " + lSeed );                
    
            SetLeadByteRange(); // This function sets the byte range for ANSI characters in different locales.

            VerifyLCIDAndLoadResource(iLCID);   // It will load the default locale resources.
        }

        //*-------------------------------------------------------------------------------------------------
        //    Name           : VerifyLCIDAndLoadResource
        //    Purpose        : Verfies the Locale ID and loads the resources if it's valid testing Locale ID.
        //    Inputs         : iLCID - locale ID
        //    Outputs        : loads the resource file for the current locale ID, if it's a valid locale.
        //*-------------------------------------------------------------------------------------------------
        private void VerifyLCIDAndLoadResource(int iLCID){
    		try{
                if ( iCurrentLCID > TEMP_LCID &&  iCurrentLCID == iLCID)  //Check to see if we have already loaded the requested resources.
                    return;
    			
                if ( iLCID == TEMP_LCID ) //Load default system locale specific resources.
    		        iCurrentLCID = CultureInfo.CurrentCulture.LCID ;
    			else
    				iCurrentLCID = iLCID ;

            	Debug.WriteLine( "Locale ID... " + iCurrentLCID ); // We may need this to debug test cases.                
                
                //verify whether genstrings supports this culture.
                bool bResult = VerifyLCID( iCurrentLCID );
                if (! bResult )
                    throw new NotSupportedException("Requested LCID is not suppported...  LCID number:" + iCurrentLCID );

                //Set the NumberFormatInfo to the current culture number format info
                nfiCurrent = new CultureInfo( iCurrentLCID ).NumberFormat ;
                    
                //Load resource file.            
                if(! LoadResourceFile(iCurrentLCID) )
                    throw new NotSupportedException("Not able to load the resource file...  LCID number:" + iCurrentLCID );
            } catch ( Exception e){
    			Debug.WriteLine("Resource loading failed... \r\nFollowing exception occured.... \r\n" + e.ToString()  );
                // We have written the debug statements to the file. Now throw the exception, testcase should know, that
                // some exception occured here. 
                throw new Exception("Current LCID number... " + iCurrentLCID.ToString() , e );
    		}		
    	}

        //*-------------------------------------------------------------------------------------------------
        //    Name           : VerifyLCID
        //    Purpose        : Verify whether it's a valid Locale ID.
        //    Inputs         : iLCID - locale ID
        //    Outputs        : True, if the LCID is a valid testing LocaleID. 
        //                     False, if the LCID is an invalid testing Locale ID.
        //*-------------------------------------------------------------------------------------------------
        private bool VerifyLCID(int iLCID){
            Array arrEnumValues = Enum.GetValues( typeof( enuLCIDList ) );
             
            for (int iLoop = 0 ; iLoop < arrEnumValues.Length ; iLoop++){
                if ( iLCID == (int)arrEnumValues.GetValue(iLoop) )
                    return true ;
            }
            return false;
        }

        //*-------------------------------------------------------------------------------------------------
        //    Name           : LoadResourceFile
        //    Purpose        : Load the resource file depends on the input Locale ID. 
        //    Inputs         : iLCID - locale ID
        //    Outputs        : True, if the resource file is successfully loaded.  
        //                     False, if the resource file is not successfully loaded.
        //*-------------------------------------------------------------------------------------------------
        private bool LoadResourceFile( int iLCID){
            try{
                
                String strLangName = Enum.GetName( typeof( enuLCIDList ) , (object)iLCID);  // Get the current locale name.
                ResourceManager manager = new ResourceManager("IntlStrings", this.GetType().Module.Assembly);  //Load the resource file.
                strCodePageInfo = manager.GetString( strLangName + RESOURCE_STRING_POSTFIX); //Get the Resource information for the current locale.
                
                if ( strCodePageInfo  == null )
                    Debug.WriteLine("Resource file is empty");

                Debug.WriteLine("Resource loaded for .. " +  strLangName + RESOURCE_STRING_POSTFIX ); //Debug statement, we can remove later.
            }catch (FileNotFoundException e ){
                Debug.WriteLine("Specified Resource file doesn't exist... " + e.ToString());
                return false ;
            }catch ( Exception e ){
                Debug.WriteLine("Current LCID number.... " + iLCID );
                Debug.WriteLine("Exception occured while loading the resources... Current Locale ID:{0} \r\n Following exception occured... \r\n " + e.ToString() );
                return false;
            }    
            return true ;     
        }

        //*--------------------------------------------------------------------------------------------------
        //    Name           : GetRandString
        //    Purpose        : Generates a string composed of valid characters for the current locale ID. 
        //                     String is retrieved from TEXT block in the resouce file.
        //    Inputs         : iMaxChar -- int maximum number of unicode character to be generated.
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                        if false, does not verify generated characters
        //    Outputs        : Random generated string
        //*--------------------------------------------------------------------------------------------------        
        public String GetString(int iMaxChar , bool bAbsolute , bool bValidate){
            return GetString(iMaxChar, bAbsolute, bValidate, false );
        }
        
        public String GetString(int iMaxChar , bool bAbsolute , bool bValidate, bool bNoLeadNum){
            if ( iMaxChar <= 0 )  // If the string length is zero, return an empty string
                return String.Empty;
            
            if (! bAbsolute )
                iMaxChar = rand.Next( 1  , iMaxChar );  

            String strTemp = MakeTheString(iMaxChar);
            
            //Include all the intestring characters.
            strTemp = InsertInterestingChars( strTemp );
            return strTemp ;
        }

        private String InsertInterestingChars(String strText){
            //We should return the same length string back after inserting the problematic characters, 
            String strInteresting = RetrieveSectionInfo( enuDataSectionTypes.CHARS_TO_INCLUDE_IN_STRING, false );
            
            String strLine = GetRandomLine( strInteresting );

            if ( strLine == String.Empty ) 
                return strText ;

            // Remove strLine.Length number of characters from the string to include the strLine.
            if ( strText.Length > strLine.Length) {
                strText = strText.Remove( rand.Next(0, strText.Length - strLine.Length) , strLine.Length );
                strText = strText.Insert( rand.Next(0, strText.Length), strLine );
            }

            return strText; 
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetTop20String
        //    Purpose        : Generates a string that is composed of Interesting characters in various positions.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                       if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                       if false, does not verify generated characters
        //    Outputs        : Returns a string with top 20 interesting characters inserted in various positions.        
        //--------------------------------------------------------------------------- ----
        public String GetTop20String(Int16 iMaxChar, bool bAbsolute, bool bValidate){                                     
            String strText = null;

            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;
            
            if (! bAbsolute )
                iMaxChar = (Int16)rand.Next( 1  , iMaxChar );  

            String strInteresting = RetrieveSectionInfo( enuDataSectionTypes.INTERESTING_CHARS, false );
	    //Console.WriteLine(strInteresting);
            strText =  MakeProbOrInterestingString( strInteresting, iMaxChar);
            return strText;    
        }

        private string MakeProbOrInterestingString(string strText, int iMaxChar){
            String strTemp = String.Empty ;
            int iRandNum = 0 ;

            ArrayList alLines = GetAllLines( strText);
            for(int iLoop = 0; iLoop < alLines.Count ; iLoop++)
                strTemp = strTemp + (String) alLines[iLoop];

            int iStrLength = iMaxChar - strTemp.Length ;   
            if( iStrLength > 0 ) {
                strText = GetString( iStrLength , true , true ); //Get the string from the current testing locale resources.
            
                //Insert Interesting characters in various positions.
                int[] iArrPos = GetRandomPositions( strText.Length, alLines);                       
                for(int iLoop =0; iLoop < alLines.Count ; iLoop++)
		{
			//Console.WriteLine("Length " + strText.Length.ToString() + " insert " + Convert.ToString( alLines[iLoop]) + " at: " + iArrPos[iLoop].ToString());
			if(iArrPos[iLoop]>strText.Length)
				iArrPos[iLoop]=strText.Length;
                    strText =strText.Insert( iArrPos[iLoop] , Convert.ToString( alLines[iLoop]) ) ;
		}
            }
            else{  //In case the requested string length less than the intesting chars.
                iRandNum = rand.Next( 0 , strTemp.Length );
                strText = (strTemp+strTemp).Substring( iRandNum , iMaxChar );
            }               

            return strText ;
        }

        private int[] GetRandomPositions(int iLength, ArrayList al){
            int[] iArrLocations = new int[al.Count] ;
            int iPosOffSet = 0 ;
            for(int iLoop = 0 ; iLoop < al.Count ; iLoop++)
                iArrLocations[iLoop] = rand.Next( (iLength * iLoop)/al.Count, (iLength * (iLoop+1))/al.Count);
            
            for(int iLoop = 1 ; iLoop < al.Count ; iLoop++){
                iPosOffSet += ((String) al[iLoop]).Length  ;
                iArrLocations[iLoop] += iPosOffSet ;
            }

            return iArrLocations ;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetProblematicStrings
        //    Purpose        : Retrieves all the problematic characters for the current Locale resource file and makes a string 
        //                     out of each problematic character.
        //    Outputs        : returns string array with atleast one problematic character in each string.
        //-------------------------------------------------------------------------------
        public String[] GetProblematicStrings(){
            int iProbCharNum = 0 ; 
            String strProbChars = String.Empty , strTemp = String.Empty;
            String[] strProbStrs = null;
            
            strProbChars = RetrieveSectionInfo( enuDataSectionTypes.PROBLEMATIC_CHARS, false  );
            ArrayList al = GetAllLines( strProbChars );
            strProbStrs = new String[al.Count];

            //Get a random string and insert a problematic charcter at a random location.
            for(int iLoop = 0 ; iLoop < al.Count ; iLoop++){
                strTemp = GetString( iDefaultStrLength, true, true, false );
                strTemp =strTemp.Insert( rand.Next( 0, strTemp.Length ) , Convert.ToString( al[iLoop] ) ) ;
                strProbStrs[iProbCharNum++] = strTemp ;
            } 

            return strProbStrs;  
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetProbCharString
        //    Purpose        : Generates a string that is composed of problem characters in various positions.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                       if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                       if false, does not verify generated characters
        //    Outputs        : Returns a string with problematic characters inserted in various positions.        
        //-------------------------------------------------------------------------------
        public String GetProbCharString(Int16 iMaxChar , bool bAbsolute , bool bValidate){
            String strText = null;
                
            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;
            
            if (! bAbsolute )
                iMaxChar = (Int16)rand.Next ( 0 , iMaxChar );

            String strProblematic = RetrieveSectionInfo( enuDataSectionTypes.PROBLEMATIC_CHARS, false  );
            
            strText =  MakeProbOrInterestingString(strProblematic, iMaxChar);
            return strText; 
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetProbURTCString
        //    Purpose        : This function returns a string of problem Unicode Round Trip Conversion
        //                        characters if there are found problem characters for the tested code page.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                        if false, does not verify generated characters
        //    Outputs        : Returns a string with problematic unicode round trip conversion characters.
        //-------------------------------------------------------------------------------
        public String GetProbURTCString(Int16 iMaxChar, bool bAbsolute, bool bValidate ){ 
            String strProbURT = null;

            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;

            if (! bAbsolute )
                iMaxChar = (Int16)rand.Next ( 0 , iMaxChar );

            strProbURT = RetrieveSectionInfo( enuDataSectionTypes.NOT_ROUNDTRIPPABLE, true  );
            
            if ( strProbURT.Length < iMaxChar ) {
                 String strTemp = GetString( iMaxChar - strProbURT.Length , true , true );
                 strProbURT = strTemp.Insert( rand.Next(0, strTemp.Length - strProbURT.Length ) , strProbURT ) ;
            }
            else
                strProbURT = strProbURT.Substring( 0 , iMaxChar ) ;
            return strProbURT;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetRandStrLCID
        //    Purpose        : This function returns random ansi characters.
        //                     Random character generation is based on the input lcid.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                       if false, does not verify generated characters
        //                   : enulcidType -- Locale ID.
        //    Outputs        : A random generated string
        //-------------------------------------------------------------------------------
        public String GetRandStrLCID(Int16 iMaxChar, bool bAbsolute, bool bValidate){
            return GetRandStrLCID(iMaxChar, bAbsolute, bValidate, enuLCIDList.English, false );                                
        }
        
        public String GetRandStrLCID(Int16 iMaxChar, bool bAbsolute, bool bValidate,enuLCIDList enulcidType){
            return GetRandStrLCID(iMaxChar, bAbsolute, bValidate, enulcidType, false );                                                                
        }

        public String GetRandStrLCID(Int16 iMaxChar, bool bAbsolute, bool bValidate,enuLCIDList enulcidType, bool bNoLeadNum){
            String strText = null ;
            int iOldLCID = iCurrentLCID ;
            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;

            VerifyLCIDAndLoadResource( (int)enulcidType );   // Change the current Locale ID to the requested one.                                          
            
            //MakeRandString generates a random ansi string for the specified Locale type.
            strText = MakeRandString(iMaxChar, bAbsolute, bValidate, bNoLeadNum);

            VerifyLCIDAndLoadResource( iOldLCID  );  //Change back to the default one.
            return strText ;                                  
        }
                
        //-------------------------------------------------------------------------------
        //    Name           : GetStrLCID
        //    Purpose        : To generate a string composed of consecutive ascending ansi characters values
        //                        thus assurring every character is hit.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                        if false, does not verify generated characters
        //                   : enulcidType -- LCID, valid locale identifier
        //    Outputs        : Returns a consecutive ascending ansi character string.
        //-------------------------------------------------------------------------------
        public String GetStrLcid(Int16 iMaxChar, bool bAbsolute, bool bValidate){                                     
            return GetStrLcid( iMaxChar, bAbsolute, bValidate, enuLCIDList.English, false );
        }
        
        public String GetStrLcid(Int16 iMaxChar, bool bAbsolute, bool bValidate, enuLCIDList enulcidType){                                     
            return GetStrLcid( iMaxChar, bAbsolute, bValidate, enulcidType, false ); 
        }

        public String GetStrLcid(Int16 iMaxChar, bool bAbsolute, bool bValidate, enuLCIDList enulcidType , bool bNoLeadNum){                                     
            String strText = null ;
            int iOldLCID = iCurrentLCID ;

            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;

            VerifyLCIDAndLoadResource( (int)enulcidType );                                          
            
            strText = MakeString(iMaxChar, bAbsolute, bValidate, bNoLeadNum);
            VerifyLCIDAndLoadResource( iOldLCID  ); 
            return strText ;
        }
        
        //-------------------------------------------------------------------------------
        //    Name           : GetProbCharStrLCID
        //    Purpose        : To generate a string that is composed of problem characters in various positions.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                        if false, does not verify generated characters
        //                   : enulcidType -- LCID, valid locale identifier
        //    Outputs        : Returns a string that is composed of problem characters.
        //-------------------------------------------------------------------------------        
        public String GetProbCharStrLCID(Int16 iMaxChar, bool bAbsolute, bool bValidate){
        	return GetProbCharStrLCID(iMaxChar, bAbsolute, bValidate, enuLCIDList.English);	
            
        }
        
        public String GetProbCharStrLCID(Int16 iMaxChar, bool bAbsolute, bool bValidate, enuLCIDList enulcidType ){
            String strText = null;
            int iOldLCID = iCurrentLCID ;
            VerifyLCIDAndLoadResource( (int)enulcidType );  //Load the specified resource.                                        
            
            strText = GetProbCharString(iMaxChar, bAbsolute, bValidate);
            VerifyLCIDAndLoadResource( iOldLCID  );   //Switch back to the default resource.
            return strText ;                                                                              
        }
        
        //-------------------------------------------------------------------------------
        //    Name           : GetProbURTCStrLCID   
        //    Purpose        : To generate a string that is composed of Unicode Round Trip Conversion
        //                     problem characters in various positions.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                        if false, does not verify generated characters
        //                   : enulcidType -- LCID, valid locale identifier
        //    Outputs        : A problem URT string
        //-------------------------------------------------------------------------------
        public String GetProbURTCStrLCID(Int16 iMaxChar, bool bAbsolute, bool bValidate){
        	return GetProbURTCStrLCID(iMaxChar, bAbsolute, bValidate, enuLCIDList.English);	
        }

        public String GetProbURTCStrLCID(Int16 iMaxChar, bool bAbsolute, bool bValidate, enuLCIDList enulcidType ){
            String strText = null ;
            int iOldLCID = iCurrentLCID ;
            VerifyLCIDAndLoadResource( (int)enulcidType );                                          
            strText = GetProbURTCString(iMaxChar, bAbsolute, bValidate);
            VerifyLCIDAndLoadResource( iOldLCID  );
            return strText ;                                                                              
        }
        
        //-------------------------------------------------------------------------------
        //    Name           : GetRandString
        //    Purpose        : We need this function for backward compitable with the old gestrings.dll. We may delete this 
        //                     in V2 time frame.
        //    Inputs         : iMaxChar -- int maximum number of unicode character to be generated.
        //                     : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                     :     if set false, number of generated chars will be random.
        //                     : bValidate -- boolean, if true, verify generated characters are valid
        //                     :     if false, does not verify generated characters
        //    Outputs        : Random generated string
        //-------------------------------------------------------------------------------
        [Obsolete("Please use GetString method")]
        public String GetRandString(Int16 iMaxChar, bool bAbsolute, bool bValidate){                                     
            return GetRandString(iMaxChar, bAbsolute, bValidate, false) ;
        }
        
        public String GetRandString(Int16 iMaxChar, bool bAbsolute, bool bValidate, bool bNoLeadNum ){                                     
            return GetString(iMaxChar, bAbsolute, bValidate) ;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetUniStrRandAnsi
        //    Purpose        : This function always returns a string of Unicode String.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                       if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                       if false, does not verify generated characters
        //                   : intType -- Type of conversion to make. CODE_UNI, CODE_UTF7, CODE_UTF8.
        //                       String is always unicode we can't return UTF8 or UTF7 encoding string. If you
        //                       need UTF8 or UTF7 encoding bytes use GetUniStrRandAnsiBytes method.
        //    Note           : enulcidType -- Locale ID.
        //-------------------------------------------------------------------------------
        [Obsolete("String is always unicode, if you need different encoding bytes please use GetUniStrRandAnsiBytes method")]
        public String GetUniStrRandAnsi(int iMaxChar, bool bAbsolute, bool bValidate, enuCodeType intCodeType, enuLCIDList enulcidType, bool bNoLeadNum ){
            String strText; 
            
            int iOldLCID = iCurrentLCID ;
            VerifyLCIDAndLoadResource( (int)enulcidType );                                                  
            
            if ((int)intCodeType < 0 || (int)intCodeType > 3)  // invalid conversion type
                intCodeType = enuCodeType.CODE_UNI;

            // Even if you read a string from UTF8 encoding file the string is always unicode. We cann't really 
            // do much here. Just return the string not to break the existing testcases,
            strText = GetString( iMaxChar, bAbsolute, bValidate, false );

            VerifyLCIDAndLoadResource( iOldLCID  );
                        
            return strText ;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetUniStrRandAnsiBytes
        //    Purpose        : This function returns a Byte array of Unicode,UTF7 or UTF8 characters.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                       if set falise, number of generated chars will be random.
        //                   : bValidate -- boolean, if true, verify generated characters are valid
        //                       if false, does not verify generated characters
        //                   : intType -- Type of conversion to make. CODE_UNI, CODE_UTF7, CODE_UTF8.
        //    Note           : enulcidType -- Locale ID.
        //-------------------------------------------------------------------------------
        public Byte[] GetUniStrRandAnsiBytes(int iMaxChar, bool bAbsolute, bool bValidate, enuCodeType intCodeType, enuLCIDList enulcidType, bool bNoLeadNum ){
            String strText; 
            Byte[] bArrEncoded = null;
            
            int iOldLCID = iCurrentLCID ;
            VerifyLCIDAndLoadResource( (int)enulcidType );                                                  
            
            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return bArrEncoded;

            if ((int)intCodeType < 0 || (int)intCodeType > 3)  // invalid conversion type
                intCodeType = enuCodeType.CURRENT; //use default system code page. 

            strText = GetString( iMaxChar, bAbsolute, bValidate );

            switch( intCodeType ){
                case enuCodeType.CODE_UTF8:
                    UTF8Encoding encUTF8 = new UTF8Encoding();
                    bArrEncoded = encUTF8.GetBytes( strText );
                    break ;
                case enuCodeType.CODE_UTF7:
                    throw new NotSupportedException("The UTF-7 encoding is insecure and should not be used");
                    // UTF7Encoding encUTF7 = new UTF7Encoding();
                    // bArrEncoded = encUTF7.GetBytes( strText );
                    // break ;
                case enuCodeType.CODE_UNI:
                    UnicodeEncoding encUnicode = new UnicodeEncoding();
                    bArrEncoded = encUnicode.GetBytes( strText );
                    break ;
                case enuCodeType.CURRENT: //In case if you need the encoding bytes for default system code page.
                    Encoding encode = Encoding.Default;
                    bArrEncoded = encode.GetBytes( strText );
                    break;
                default:
                    break ; 
            }

            VerifyLCIDAndLoadResource( iOldLCID  );
                        
            return bArrEncoded ;
        }
        
        //-------------------------------------------------------------------------------
        //    Name           : GetUniStrInvalidAnsi
        //    Purpose        : To return a string of Unicode characters. These unicode characters
        //                        are random invalid ansi characters when converted to ansi.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //                   : intCodeType -- Type of conversion to make. 
        //                   : enulcidType -- Current Locale ID.
        //-------------------------------------------------------------------------------
        public String GetUniStrInvalidAnsi(int iMaxChar, bool bAbsolute, enuCodeType intCodeType, enuLCIDList enulcidType ){
            String strText; 

            int iOldLCID = iCurrentLCID ;
            VerifyLCIDAndLoadResource( (int)enulcidType );                                                  
            
            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;

            if ((int)intCodeType < 0 || (int)intCodeType > 3)  // invalid conversion type
                intCodeType = enuCodeType.CURRENT;

            strText = RetrieveSectionInfo( enuDataSectionTypes.RANDOM_INVALID_ANSI_CHAR, true  );
            strText = RemoveAdditionalnfoChars(strText);  // Remove all the line feeds, carriage return and the additional information that we have each char.

            strText = strText.Replace( "\r\n" , "" );  // Remove all the line feeds and carriage return characters from the data.

            VerifyLCIDAndLoadResource( iOldLCID  );
            
            return strText ;
        }              
        
       //-------------------------------------------------------------------------------
        //    Name           : RemoveLineFeedChars
        //    Purpose        : Helper function to retrieve only characters and to remove additional info(ED56 -> FA72: 侔 -----> 侔 )
        //    Inputs         : strText -- String with information about each character.
        //    Outputs        : Returns a string with only required testing characters.
        //--------------------------------------------------------------------------- ----
        private String RemoveAdditionalnfoChars(String strText){
            StringBuilder strTemp = new StringBuilder();
            String strLine ; 
            
            StringReader sr = new StringReader( strText );
            while( true ) {
                strLine = sr.ReadLine() ;
                if ( strLine == null) 
                    break ;
                if( strLine.Trim() != String.Empty )
                    strTemp.Append( strLine.Substring( strLine.IndexOf(":") + 2));
            } 
            return strTemp.ToString() ;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetUniStrMappedAnsi
        //    Purpose        : To return a string of Unicode characters. These unicode characters
        //                        are random valid ansi characters when converted to ansi.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //                   : intType -- Type of conversion to make. 
        //                   : enulcidType -- Locale id.
        //-------------------------------------------------------------------------------
        public String GetUniStrMappedAnsi(Int16 iMaxChar, bool bAbsolute, enuCodeType intCodeType, enuLCIDList enuLCIDType ){
            String strText; 

            // always ansi characters are mapped to unicode. Runtime supports unicode characters, We don't need this method
            // any more.
            int iOldLCID = iCurrentLCID ;
            VerifyLCIDAndLoadResource( (int)enuLCIDType );                                                  
            
            if ((int)intCodeType < 0 || (int)intCodeType > 3)  // invalid conversion type
                intCodeType = enuCodeType.CODE_UNI;

            strText = GetRandStrLCID( iMaxChar, bAbsolute, false, enuLCIDType );
        
            VerifyLCIDAndLoadResource( iOldLCID  );
            
            return strText ;
        }
        
        //-------------------------------------------------------------------------------
        //    Name           : MakeTheString
        //    Purpose        : Generates a string to a specified length.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //    Output         : returns a string to the specified length from the TEXT section.
        //-------------------------------------------------------------------------------
        private String MakeTheString(int iMaxChar ){            
            StringBuilder strGenerate = new StringBuilder();
            String strText = String.Empty;
            int iRandLength = 0 , iStart = 0, iReqStrLength = iMaxChar;

            strGenerate.Length = 0;
            try {
                strText = RetrieveSectionInfo( enuDataSectionTypes.TEXT, true );   

                //Build the string to the requested length in a loop.
                while(strGenerate.ToString().Length < iMaxChar ){
                    iReqStrLength = iReqStrLength - iRandLength ;    //New Length.
                    iRandLength = rand.Next( 0 , strText.Length) ;
                    
                    if ( iRandLength > iReqStrLength ) 
                        iRandLength = iReqStrLength ;   //Random length 
                    
                    iStart = rand.Next( 0 , strText.Length );    // Random starting point
                    if ( iRandLength + iStart >= strText.Length) // We need this check to make sure Start + Length is always less than the string length.
                        iRandLength = strText.Length - iStart ;
                    strGenerate.Append( strText.Substring( iStart , iRandLength )); 
                }      

            }catch (Exception e){
                Debug.WriteLine("Unexpected exception while reading the token file... " + e.ToString() );
            } 

            strText =strGenerate.ToString(); 
            return strText;   
        }
        
        //Returns true if it's a valid file name char, otherwise false.
        private bool IsValidFileNameChar(Char cFileChar){
            for(int iLoop = 0 ; iLoop < cInvalidFileChars.Length ; iLoop++ ){
                if ( cFileChar == cInvalidFileChars[iLoop] ) 
                    return false ;
            }

            UnicodeCategory unicat = Char.GetUnicodeCategory( cFileChar ) ;
            if (! (unicat == UnicodeCategory.LowercaseLetter || unicat == UnicodeCategory.UppercaseLetter || unicat == UnicodeCategory.OtherLetter || unicat == UnicodeCategory.OtherLetter || unicat == UnicodeCategory.LetterNumber || unicat == UnicodeCategory.DecimalDigitNumber) ) 
                return false ;
            return true ;
        }
        
        //-------------------------------------------------------------------------------
        //    Name           : GetValidFileName
        //    Purpose        : Generates a valid file name string.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //    Output         : returns a valid file name.
        //-------------------------------------------------------------------------------
        public String GetValidFileName(int iMaxChar, bool bAbsolute){
            StringBuilder strFileName = new StringBuilder( 0 );
            
            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;

            if (! bAbsolute) 
                iMaxChar = rand.Next(1 , iMaxChar ) ;

            String strText = GetString( iMaxChar * 2, bAbsolute, true ); //Get a large string.

            for( int iLoop = 0 ; iLoop < strText.Length ; iLoop++ ){
                if ( IsValidFileNameChar( strText[iLoop]) && strFileName.ToString().Length < iMaxChar)  {
                    strFileName.Append( Convert.ToString( strText[iLoop] ) );
                }
            }
            return strFileName.ToString() ;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetInvalidFileName
        //    Purpose        : Generates a invalid file name string.
        //    Inputs         : iMaxChar -- maximum number of character to be generated
        //                   : bAbsolute -- if set true, exact number (iMaxChar) will be generated,
        //                        if set falise, number of generated chars will be random.
        //    Output         : returns a invalid file name.
        //-------------------------------------------------------------------------------
        public String GetInvalidFileName(int iMaxChar, bool bAbsolute){
            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;
            
            if (! bAbsolute) 
                iMaxChar = rand.Next(0 , iMaxChar ) ;
            
            if ( iMaxChar <= cInvalidFileChars.Length ) 
                return new String( cInvalidFileChars ).Substring( 0 , iMaxChar );

            String strFileName = GetString( iMaxChar - cInvalidFileChars.Length, bAbsolute, true );

            for( int iLoop = 0 ; iLoop < cInvalidFileChars.Length ; iLoop++ ){
                int iNumRand = rand.Next( 1, strFileName.Length );
                strFileName = strFileName.Insert( iNumRand, Char.ToString(cInvalidFileChars[iLoop]) ); 
            }
            return strFileName ;
        }
        
        // This method gives a alphanumeric string, first character is always a character.
        // We need this method for back ward compatibility.        
        [Obsolete("Use GetRandomValidIdentifer(Int16, bool) method to get random valid identifier.")]
        public String GetAlphaNumericString(Int16 iMaxChar, bool bAbsolute){
            return  GetRandomValidIdentifier( iMaxChar , bAbsolute );
        }

        //This method helps to get the random valid identifier.
        public String GetRandomValidIdentifier(Int16 iMaxChar, bool bAbsolute){
            StringBuilder strBuild = new StringBuilder();
            bool bFirst = true ;
            UnicodeCategory unicat ;
            String strTemp = String.Empty ;
            int iStrPos = 0;

            if ( iMaxChar <= 0 )  // If the string length is <= zero, return an empty string
                return String.Empty;

            if (! bAbsolute) 
                iMaxChar = (Int16)rand.Next(1 , iMaxChar ) ;

            //Verify whether the first character is valid character, if not remove it.
            for (int iLoop = 0; iLoop < iMaxChar ; iLoop++ ){
                if ( strTemp.Length == 0 || strTemp.Length == iStrPos) {
                    strTemp = GetString(2 * iMaxChar, true, false); // We are getting bigger string than requested.
                    iStrPos = 0;
                }

                for( int i = iStrPos ; i < strTemp.Length ; i++ ){
                    char c = strTemp.ToCharArray()[iStrPos++] ;
                    unicat = Char.GetUnicodeCategory( c ) ;

                    if( bFirst ){ 
                        if ( unicat == UnicodeCategory.LowercaseLetter || unicat == UnicodeCategory.UppercaseLetter || unicat == UnicodeCategory.OtherLetter) {
                            strBuild.Append( c );
                            bFirst = false ;
                        }
                        else
                            continue ;
                    }
                    else{
                        if ( unicat == UnicodeCategory.LowercaseLetter || unicat == UnicodeCategory.UppercaseLetter ||  unicat == UnicodeCategory.DecimalDigitNumber || unicat == UnicodeCategory.OtherLetter) 
                            strBuild.Append( c );
                        else 
                            continue ;
                    } 
                    break;
                }
                
                if ( strTemp.Length == iStrPos) {
                    iLoop-- ;
                }
           }
           return strBuild.ToString(); //Return the string to requested length. 
        }

        public String GetRandomDirectoryName(int iNumberOfSubDirs){
            return GetRandomDirectoryName(Environment.CurrentDirectory , iNumberOfSubDirs);
        }

        public String GetRandomDirectoryName(String strPath, int iNumberOfSubDirs){
            for(int iLoop = 0 ; iLoop < iNumberOfSubDirs ; iLoop++ ){
                 //Make sure that the directory name won't exceed more than 258 characters.
                 String strDir = GetValidFileName( (VALID_PATH_LENGTH - iNumberOfSubDirs)/iNumberOfSubDirs , false );
                 strPath = Path.Combine( strPath, strDir ); 
            }
            return strPath ;
        }

        // All methods below are helper functions to get inforamtion from the NumbberformatInfo for the 
        // current culture. 
        public int CurrenyDecimalDigits(){
            return nfiCurrent.CurrencyDecimalDigits ;
        }
        
        public String CurrenyDecimalSeparator(){
            return nfiCurrent.CurrencyDecimalSeparator ;
        }

        public string CurrenyGroupSeparator(){
            return nfiCurrent.CurrencyGroupSeparator ;
        }

        public int[] CurrenyGroupSizes(){
            return nfiCurrent.CurrencyGroupSizes ;
        }

        public string CurrenySymbol(){
            return nfiCurrent.CurrencySymbol ;
        }

        public int NumberDecimalDigits(){
            return nfiCurrent.NumberDecimalDigits ;
        }

        public string NumberDecimalSeparator(){
            return nfiCurrent.NumberDecimalSeparator ;
        }

        public string NumberGroupSeparator(){
            return nfiCurrent.NumberGroupSeparator ;
        }

        public int[] NumberGroupSizes(){
            return nfiCurrent.NumberGroupSizes ;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetInterestingDateTime
        //    Purpose        : Generates a random valid interesting DateTime object.
        //    Output         : returns a datetime object
        //-------------------------------------------------------------------------------
        public DateTime GetInterestingDateTime(){
             DateTime dt = DateTime.Now; 
             String strErrMsg = "" ;
             String strDates = RetrieveSectionInfo( enuDataSectionTypes.VALIDDATETIMES , false);
             //Console.WriteLine( "Dates string... " + strDates );
             String strLine = GetRandomLine( strDates ) ;
             try{
                 strLine = strLine.Trim(); 
                 if ( String.Empty != strLine )
                     dt = DateTime.Parse( strLine );
             }
             catch(FormatException e ){
                 strErrMsg = "FormatException occured... Input string..." + strLine ;
                 throw new FormatException( strErrMsg, e );                
             }
             return dt ;
        }

       //The following function helps to get the random line from the given section name. 
       private String GetRandomLine(String strText){
           ArrayList strLines = GetAllLines( strText ); 
           
           if ( strLines.Count == 0 ) 
                return String.Empty ;
            
            String strLine = (String)strLines[rand.Next(0 , strLines.Count )] ;
            return strLine ;
        }

        private ArrayList GetAllLines(String strText){
            ArrayList strLines = new ArrayList(); 
            String strLine = "" ; 
            StringReader sr = new StringReader( strText);
            while( true ) {
                strLine = sr.ReadLine() ;
                if ( strLine == null) 
                    break ;
                if( strLine != String.Empty ){
                    strLine = strLine.Replace( "\r\n" , "" );
                    if(strLine.Trim()!=String.Empty)
                        strLines.Add( strLine );
                }
            } 
            return strLines ;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetFormattedDateTimeStrings
        //    Purpose        : Generates a random valid formatted DateTime string.
        //    Output         : returns random formatted datetime string
        //-------------------------------------------------------------------------------
        public String GetFormattedDateTimeStrings(){
            String strDateStrings = RetrieveSectionInfo( enuDataSectionTypes.VALIDDATESTRINGS, false );

            String strLine = GetRandomLine( strDateStrings  ) ;
            return strLine ;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GetInterestingDateTimeFormat
        //    Purpose        : Generates a random Interesting datetime format string.
        //    Output         : returns random interesting datetime format
        //-------------------------------------------------------------------------------
        public String GetInterestingDateTimeFormat(){
            String strDateTimeFormats = RetrieveSectionInfo( enuDataSectionTypes.VALIDFORMATS, false );

            String strLine = GetRandomLine( strDateTimeFormats  ) ;
            return strLine ;       
        }

        //-------------------------------------------------------------------------------
        //    Name           : RetrieveSectionInfo
        //    Purpose        : Retrieves informaton from the specified data section.
        //    Input          : DataSectionType - Section type name
        //    Output         : returns all the information in the specified location.
        //-------------------------------------------------------------------------------
        private String RetrieveSectionInfo(enuDataSectionTypes DataSectionType, bool bRemoveLineFeed){            
            String strText = null, strNextSectionName = null ;

            try{
                if ( (int)DataSectionType < 0 || (int) DataSectionType > 10) {
                    throw new NotSupportedException("Requested data section doesn't exist in the resource file...  Data section value:" + (int) DataSectionType );
                }
    
                String strSectionName = Enum.GetName( typeof(enuDataSectionTypes),  DataSectionType) ; 
                int iSectionValue = (int) DataSectionType ;
                
                strText = strCodePageInfo.Substring( strCodePageInfo.IndexOf(strSectionName) + strSectionName.Length + 2 );
                if ( iSectionValue != 10 ) {
                    strNextSectionName = Enum.GetName( typeof(enuDataSectionTypes), iSectionValue + 1  ) ;
                    strText = strText.Substring( 0, strText.IndexOf( strNextSectionName)   - 2 ) ; 
                }

                if( bRemoveLineFeed ){
                    // Remove all the line feeds and carriage return characters from the data.
                    strText = strText.Replace(new String( new Char[]{(Char)13}) , "" );   // Remove all the carriage characters from the data.
                    strText = strText.Replace(new String( new Char[]{(Char)10}) , "" );   // Remove all the line feed characters from the data.
                }
            } catch ( Exception e ){
                Debug.WriteLine("Unexpected exception occured in RetrieveSectionInfo method... " + e.ToString() );
            }
            return strText;
        }

        //-------------------------------------------------------------------------------
        //    Name           : GenerateRandChar
        //    Purpose        : Return ONE random ANSI character. Can be either SB or DB.
        //    Inputs         : intNumberOfByte -- Single byte char (SINGLES) or Double byte char (DOUBLE)
        //                   : blnValidate -- True if valid char, False if invalid char
        //    Outputs        : Returns one random generated valid char
        //-------------------------------------------------------------------------------
        private String GenerateRandChar(Int16 intNumberOfByte, bool blnValidate){
            String strRet = String.Empty; 
            Int16 intchar;                                     
            char strBuff= Char.MinValue;
            int lngLeadRange =0, lngLead , lngTrail  ;
            bool blnBreaker = true;                     
        
            // current platform does not support DBCS
            // generate a single byte char instead.
            int iLCIDType = GetLeadByteRangeIndex(iCurrentLCID);
            if( iLCIDType < 0)
                intNumberOfByte = SINGLES ;

        	try{
                switch ( intNumberOfByte){
                case DOUBLES:
                    do{  // validate the char
                        lngLeadRange = rand.Next(1, 1000);
                        if( (lngLeadRange % 2 != 0) && LeadByteRange[iLCIDType].intStart2 != 0 && LeadByteRange[iLCIDType].intEnd2 !=  0)                                     
                            lngLead = rand.Next(LeadByteRange[iLCIDType].intStart2, LeadByteRange[iLCIDType].intEnd2);
                        else
                            lngLead = rand.Next(LeadByteRange[iLCIDType].intStart, LeadByteRange[iLCIDType].intEnd);
                        
                        // trail byte
                        lngTrail = rand.Next(48, 254) ;
                        try{
                            strRet = Encoding.Default.GetString(new Byte[]{Convert.ToByte(lngLead) , Convert.ToByte(lngTrail)} );
                        }catch(Exception e){
                            Debug.WriteLine("Lead  ::" + lngLead.ToString("x"));
                            Debug.WriteLine("Trail ::" + lngTrail.ToString("x")) ;
                            Debug.WriteLine( Convert.ToChar(Int32.Parse(lngTrail.ToString("x") + lngTrail.ToString("x"))) );
                            Debug.WriteLine("Unexpected exception occured.. " + e.ToString() );
                        }
    
                        if( blnValidate && System.Char.IsLetterOrDigit(strRet[0]) && System.Char.IsLetterOrDigit(strRet[1]))      //TODO: && System.Char.IsPrintable(strBuff)
                            blnBreaker = false;                    
                        else
                             blnBreaker = true;
                    }while( blnBreaker) ;
                    break ;
                case SINGLES:
                    do{  // Single byte char
                        intchar = (Int16)rand.Next(1, 255) ;
                        strBuff = Convert.ToChar(intchar);
                        if( blnValidate ){
                            if (System.Char.IsLetterOrDigit(strBuff) ){  
                                break ;
                            }
                        }
                        else
                            break ;
                    }while( true );
                    strRet = Convert.ToString(strBuff) ;
                    break ;
                default:
                    Debug.WriteLine("Invlid type... ");
                    break ;
                }
            }catch(Exception e){
                throw new Exception("Excetpion occured in GenerateRandChar.." + intNumberOfByte.ToString() +  "is an invalid character set", e  );
            }
            return strRet ;
        }

        //Helper function to retrieve the Index for the LeadByteRange for the current locale.
        private int GetLeadByteRangeIndex(int iLCID){
              int iLeadByteIndex = -1 ;
              for(int iLoop = 0 ; iLoop < LeadByteRange.Length ; iLoop++){
                  if( LeadByteRange[0].lcid == iLCID )
                      return iLoop ;
              }
              return iLeadByteIndex ;                                                    
        }

        //-------------------------------------------------------------------------------
        //    Name           : GenerateChar
        //    Purpose        : Return ONE ANSI character.
        //    Inputs         : intNumberOfByte -- Single byte char (SINGLE) or Double byte char (DOUBLE)
        //                     : blnValidate -- True if valid char, False if invalid char
        //    Outputs        : Returns one generated valid char
        //-------------------------------------------------------------------------------
        private String GenerateChar(Int16 intNumberOfByte,bool  blnValidate){                                     
            String strRet = String.Empty; 
            Int16 intchar;                                     
            char strBuff= Char.MinValue;
            int lngLead , lngTrail, intAlt = 1  ;
            bool blnBreaker = true;
            
            // If the current platform does not support DBCS, generate a single byte char instead.
            int iIndex = GetLeadByteRangeIndex(iCurrentLCID);
            if( iIndex > 0)
                intNumberOfByte = (Int16)LeadByteRange[iIndex].iLangCharType ;
            else
                intNumberOfByte = SINGLES ;
            
            intAlt = intAlt + 1;
            try{
                switch ( intNumberOfByte ){
                    case DOUBLES:
                        // Double byte char
                        do{     
                                if( (intAlt % 2 != 0) && LeadByteRange[iIndex].intStart2 != 0 && LeadByteRange[iIndex].intEnd2 !=  0){                                     
                                intCPPoint = intCPPoint + 1 ;
                                if( intCPPoint + LeadByteRange[iIndex].intStart2 > LeadByteRange[iIndex].intEnd2) 
                                    intCPPoint = 0 ;
                                lngLead = LeadByteRange[iIndex].intStart2 + intCPPoint  ;
                            }
                            else{
                                intCPPoint2 = intCPPoint2 + 1 ;
                                if( LeadByteRange[iIndex].intEnd - intCPPoint2 < LeadByteRange[iIndex].intStart)
                                    intCPPoint2 = 0 ;
                                lngLead = LeadByteRange[iIndex].intEnd - intCPPoint2 ;
                            }
                                         
                            // trail byte can be any value between 48 and 254
                            lngTrail =(Int16) rand.Next(48, 254) ;
                            strRet = Encoding.Default.GetString(new Byte[]{Convert.ToByte(lngLead) , Convert.ToByte(lngTrail)} );
                            if( blnValidate){
                                for(int iLoop = 0 ; iLoop < strRet.Length ; iLoop++ ){
                                    if(! IsValidChar(new String(new Char[]{strRet[iLoop],'\0'}))){
                                        blnBreaker = true;
                                        break;
                                    }
                                    else
                                        blnBreaker = false ;
                                }
                            }            
                        } while (blnBreaker);
                        break ;
                    case SINGLES:
                        // Single byte char
                        do{
                            if( (intCPPoint + 47) > 255) // incrementing string
                                intCPPoint = 1 ;
    
                            if( (intCPPoint + intCharOffset + 47) > 255)
                                intCharOffset = 0;
    
                            intchar =(Int16) (intCPPoint + intCharOffset + 47) ;
                            strBuff = Convert.ToChar(intchar) ;
                            
                            if( blnValidate ){
                                bool bValid = IsValidFileNameChar(strBuff) ;                                          
                                if( IsValidChar(new String(new Char[]{strBuff,'0'})) && bValid){                                     
                                    strRet = Convert.ToString(strBuff) ;
                                    return strRet ;
                                }
                                else{
                                    if( (intCPPoint + 47 + 1) < 254 )
                                        intCPPoint = intCPPoint + 1 ;
                                }
                            }
                            else
                                break ;
                        } while ( true );
                        break ;
                    default:  
                        Debug.WriteLine("Invlid type... ");
                        break ;
                }
                strRet = Convert.ToString(strBuff) ;
            } catch (Exception e ){
                throw new Exception("Excetpion occured in GenerateChar.." + intNumberOfByte.ToString() + " is an invalid character set", e  );
            }
            return strRet;
        }

        [DllImport("Kernel32.dll")]
        public static extern int GetLocaleInfo(int iLCID, int LCType, StringBuilder lpData, int cchData);
        
        [DllImport("Kernel32.dll")]
        public static extern int IsValidCodePage(int CodePage);
               
        [DllImport("Kernel32.dll")]
        public static extern int GetStringTypeA(int iLCID, int dwInfoType, String lpSrcStr, int cchSrc, ref Int16 lpCharType);
        
        //-------------------------------------------------------------------------------
        //    Name           : IsValidChar
        //    Purpose        : Make sure the given ANSI char is a valid character
        //               NOTE: We always assume there is only one ANSI char in the param,
        //                             and its null terminated
        //    Inputs         : strTestStr -- random generated char
        //    Outputs        : True if char meets the criterion below, False otherwise
        //-------------------------------------------------------------------------------
        public bool IsValidChar(String strTestStr){
            int C1_PUNCT = 0x10, C1_CNTRL = 0x20, C1_BLANK = 0x40, CT_CTYPE1 = 0x1  ;
            int iCodePage;                                    
            bool bCheck = false;
            Int16[] wResult ;                                      
            StringBuilder strLCData =new StringBuilder(10) ; 
            try{
                int iValue = GetLocaleInfo(iCurrentLCID, LOCALE_IDEFAULTANSICODEPAGE, strLCData, 20);
                if( iValue != 0){                                  
                    iCodePage = Convert.ToInt32(strLCData.ToString());
                    if( IsValidCodePage(iCodePage) == 1)                                     
                        bCheck = true ;
                }
                if( bCheck ){
                    wResult = new Int16[strTestStr.Length] ;  
                    Int16 iCharType = Convert.ToInt16(wResult[0]) ;

                    int iReturn = GetStringTypeA(iCurrentLCID, CT_CTYPE1, strTestStr, -1,ref iCharType);
                    if( iReturn != 1)
                        return false ;
                    if( iReturn != 0 &&  iCharType == 0)                                     
                        return false ;
                    if( (iCharType  == 1) && ( ((int)iCharType == C1_PUNCT) || ((int)iCharType == C1_CNTRL) || ((int)iCharType == C1_BLANK)) )
                        return false ;
                }
            }catch( Exception e ){
                Debug.WriteLine("Exception occured... " + e.ToString() );
            }
            return true ;
        }
        
        //-------------------------------------------------------------------------------
        //    Name           : MakeString
        //    Purpose        : Call this private function to generate a string. If codepage is sb, characters
        //                       will be sequential. If codepage is db, it acts like MakeRandString.
        //    Inputs         : intMaxChar -- Maximum number of characters (not byte) in the return string
        //                   : blnAbsolute -- True, if you want the exact number of char specify in intMaxChar
        //                          False, if you want a random number of char, up to a maximum of intMaxChar.
        //                   : blnValidate -- True, if you want every char to be a legal char
        //                          False, if you do not want any validation. Will gen any char with in the 255 and DBCS range.
        //    Outputs        : Returns one generated valid char
        //-------------------------------------------------------------------------------
        private String MakeString(Int16 intMaxChar,bool blnAbsolute,bool blnValidate,bool blnNoLeadNum){                                     
            Int16 intRandNum;
            int intOddEven;                                     
            String strRetStr= String.Empty; 

            if( blnAbsolute)
                intRandNum = intMaxChar ;
            else
                intRandNum = (Int16)rand.Next(1, intMaxChar); 

            intCPPoint = intCPPoint + 1;
            intCharOffset = 0;

            int iIndex = GetLeadByteRangeIndex(iCurrentLCID);

            if( iIndex !=  -1){
                for(int iLoop = 0 ; iLoop < intRandNum ; iLoop++ ){
                    // Create random string
                    intOddEven = rand.Next(0, 1000);
                    if( intOddEven % 2 == 1)
                        strRetStr = strRetStr + GenerateRandChar(SINGLES, blnValidate);  // odd number, gen RANDOM single one byte char
                    else
                        strRetStr = strRetStr + GenerateChar(DOUBLES, blnValidate); // even number, gen one FIXED OFFSET double byte char
                                     
                    if( blnNoLeadNum && Char.IsNumber(Convert.ToChar(strRetStr.Substring(0, 1)))){
                        strRetStr = "" ;
                        iLoop = 0 ;
                    }
                }
            }
            else{
                for(int iLoop = 0 ; iLoop < intRandNum ; iLoop++ ){
                    intCharOffset = intCharOffset + 1 ;
                    if ( (intCharOffset + intCPPoint + 47 + 1) > 254 )
                        intCharOffset = 0 ;
                    
                    strRetStr = strRetStr + GenerateChar(SINGLES, blnValidate) ;
                                     
                    if( blnNoLeadNum && Char.IsNumber(Convert.ToChar(strRetStr.Substring(0, 1)))){                                     
                        strRetStr = "" ;
                        iLoop = 0 ;
                    }
                }
            }
            return strRetStr ;      
        }

        //-------------------------------------------------------------------------------
        //    Name           : MakeRandString
        //    Purpose        : Call this private function to generate and retrieve a random string.
        //    Inputs         : intMaxChar -- Maximum number of characters (not byte) in the return string
        //                   : blnAbsolute -- True, if you want the exact number of char specify in intMaxChar
        //                         False, if you want a random number of char, up to a maximum of intMaxChar.
        //                   : blnValidate -- True, if you want every char to be a legal char
        //                         False, if you do not want any validation. Will gen any char with in the 255 and DBCS range.
        //    Outputs        : Returns one generated random valid char
        //-------------------------------------------------------------------------------
        private String MakeRandString(Int16 intMaxChar,bool blnAbsolute,bool blnValidate,bool blnNoLeadNum){ 
            Int16 intRandNum, intOddEven;                                     
            String strRetStr= String.Empty; 
            
            if( blnAbsolute)
                intRandNum = intMaxChar ;
            else
                intRandNum = (Int16)rand.Next(1, intMaxChar);   

            for(int iLoop = 0 ; iLoop < intRandNum ; iLoop++ ){
                // Create random string
                intOddEven = (Int16)rand.Next(0, 1000);
                if ((intOddEven % 2) == 1) {
                    // odd number, generate single one byte char
                    strRetStr = strRetStr + GenerateRandChar(SINGLES, blnValidate);
            
                    if (blnNoLeadNum && Char.IsNumber(Convert.ToChar(strRetStr.Substring(0, 1)))){                                     
                        strRetStr = "";
                        iLoop = 0;
                    }
                }
                else{
                    // even number, generate one double byte char
                    strRetStr = strRetStr + GenerateRandChar(DOUBLES, blnValidate);                
                    if( blnNoLeadNum && Char.IsNumber(Convert.ToChar(strRetStr.Substring(0, 1)))){                                     
                        strRetStr = "";
                        iLoop = 0 ;
                    }
                }
            }

            return strRetStr ;            
        }

        // leading byte range of DBCS
        struct LeadByteType {
            public int lcid  ;                                   
            public short intStart ;                                       
            public short intEnd ;                                     
            public short intStart2 ;
            public short intEnd2 ;
            public short iLangCharType;
        }

        //-------------------------------------------------------------------------------
        //    Name           : SetLeadByteRange
        //    Purpose        : To set lead byte range for double byte character set
        //-------------------------------------------------------------------------------
        private void SetLeadByteRange() {
                // Japan , next time: and 0x0E0-0x0FC
              LeadByteRange[0].lcid = 0x0411 ;
              LeadByteRange[0].intStart = 0x081 ;
              LeadByteRange[0].intEnd = 0x09F  ;
              LeadByteRange[0].intStart2 = 0x0E0 ;
              LeadByteRange[0].intEnd2 = 0x0FC  ;
              LeadByteRange[0].iLangCharType = DOUBLES ;

                  // Taiwain
              LeadByteRange[1].lcid = 0x0404  ;
              LeadByteRange[1].intStart = 0x0A1  ;
              LeadByteRange[1].intEnd = 0x0F9   ;
              LeadByteRange[1].intStart2 = 0x00  ;
              LeadByteRange[1].intEnd2 = 0x00   ;
              LeadByteRange[0].iLangCharType = DOUBLES ;

                  // PRC
        	  LeadByteRange[2].lcid = 0x0804  ;
              LeadByteRange[2].intStart = 0x0A1;
              LeadByteRange[2].intEnd = 0x0F7 ;
              LeadByteRange[2].intStart2 = 0x00   ;
              LeadByteRange[2].intEnd2 = 0x00  ;
              LeadByteRange[0].iLangCharType = DOUBLES ;

                  // Korean
              LeadByteRange[3].lcid = 0x0412 ;
              LeadByteRange[3].intStart = 0x081 ;
              LeadByteRange[3].intEnd = 0x0FD  ;
              LeadByteRange[3].intStart2 = 0x00  ;
              LeadByteRange[3].intEnd2 = 0x00  ;
              LeadByteRange[0].iLangCharType = DOUBLES ;
        }

    } 
                      
    //GenStrings supports only the following LCID//s. We will throw an exception if you pass that//s doesn//t 
    //in the list below. If you really need support some other LCID please talk to the appropriate person.
    public enum enuLCIDList{
        English     = 0x409,    // English
        Japanese    = 0x411,    // Japanese
        German      = 0x407,    // German                                             
        Russian     = 0x419,    // Russian
        Hindi       = 0x439,    // Hindi
        Arabic      = 0x401,    // Arabic
        French      = 0x40C,    // French
        Spanish     = 0xC0A,    // Spain
        Italian     = 0x410,    // Italian
        ChineseTra  = 0x404,    // Chinese simplified
        ChineseSim  = 0x804,    // Chinese traditional
        ChineseHK=0xc04,    // Chinese Hong Kong SAR
        Korean      = 0x412,    // Korean 
        Turkish     = 0x41F,    // Turkish
        Brazilian = 0x416,    // Brazilian
        Dutch = 0x413,    //Dutch
        Danish = 0x406,    //Danish
        Swedish = 0x41D,    //Swedish
        Czech = 0x405,    // Czech
        Finish = 0x40B, // Finish
        Hebrew = 0x40D, // Hebrew
        Hungarian = 0x40E, // Hungarian 
        Polish = 0x415, // Polish
        Portuguese = 0x816, // Portuguese
        Greek = 0x408, // Greek
        NorwegianNyn= 0x814,    // Norwegian (Nynorsk)
        NorwegianBok= 0x414,    // Norwegian (Bokmal)
        Thai= 0x41E    // Norwegian (Bokmal)
        
    }                

    // The following 4 encoding types are supported. By default you will get UTF8 encoding strings
    // unless you specify the one you want.
    public enum enuCodeType{
    	CODE_UNI   = 0,
    	CODE_UTF8  = 1,
    	CODE_UTF7  = 2,
	    CURRENT    = 3
    }

    //We have the following section of blocks of data in the resource files. 
    public enum enuDataSectionTypes{
    	INTERESTING_CHARS           = 0,
    	PROBLEMATIC_CHARS           = 1,
        INVALID_CHARS               = 2,
    	TEXT                        = 3,
        VALIDDATESTRINGS            = 4,
        VALIDFORMATS                = 5,
        VALIDDATETIMES              = 6,
        NOT_ROUNDTRIPPABLE          = 7,                                   
        UNICODE_CATEGORY            = 8,
        RANDOM_INVALID_ANSI_CHAR    = 9,
        CHARS_TO_INCLUDE_IN_STRING  = 10
    }
}    

