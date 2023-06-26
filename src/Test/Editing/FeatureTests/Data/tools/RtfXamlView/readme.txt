the following three chars indicate a cmd line command: /, \, -

Command line Arguements are as follows:
f - a file to open app with, use a space then type full path to the file. ie. rtfxamlview -f c:\myxaml.xaml
    note app will remain open after opening the file.
    
BVTDir - used incoordination with BVT commands to force app to use assigned BVTDir instead of one assoc. with ini.
         ie, -BVTDir c:\localBVT
NumFiles - Number of files to run in this BVT. Allows for for limited runs in Directories where more than NumFiles exists.
           ie, -NumFiles 20
           
Index - Index into BVTDir or FileList
        ie, -Index 15
        
LogFile - cmd line param for LogFilename, else default name is used.
         ie, -LogFile c:\deleteme.run

FileList - txt file that contains a comma seperated list of files (full path) to run, instead of a directory.
           ie. -FileList \\Microsoft\sharename\myCustomFiles.txt
           
RTRtf - RoundTrip RTF BVT test. 

RTRtfXaml - RTF to XAML BVT test

RTXaml - RoundTrip XAML BVT test

REAssert - Simple load test, does the converter or target apps asert on paste?
         