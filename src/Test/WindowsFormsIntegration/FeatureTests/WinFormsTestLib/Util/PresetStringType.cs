namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  Defines constants that specify what string to return from the preset
    //  string file used for international boundary testing.
    //
    //  The ID numbers here map to a string table in IntlString.cool and in
    //  the INI file *.UNI.
    // </desc>
    // <seealso class="IntlString" member="Sections"/>
    // </doc>
    public enum PresetStringType
    {
        // <doc>
        // <desc>
        //  A regular string for use as button text, edit control text, etc..
        // </desc>
        // </doc>
        Boundary  = 0,
    
        // <doc>
        // <desc>
        //  A string used to create files and directories.
        // </desc>
        // </doc>
        Directory = 1
    }
}
