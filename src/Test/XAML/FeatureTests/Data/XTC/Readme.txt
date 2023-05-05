XTC files are organized as per the .NET releases. Hence the 3.0/4.0 folders. This structure will be followed irrespective of how its presented in the View (ReportViewer/DiscoveryInfo)


Subfolders under these folders

Client
-- Includes xtc files targetting the FullProduct as well as the Client Profile. 
   Note that features EXCLUSIVE to the Full Product should NOT be placed here.

Full
-- Targets ONLY Full Product exclusive features - MarkupCompiler;Speller;Hyphenation ...


OTHER NOTES:
Files having SDX in the name use the XamlServices API exclusively
    e.g:SerializationSDX - uses S.X Parse\Save API
        Serialization - uses WPF XamlReader\XamlWriter API     