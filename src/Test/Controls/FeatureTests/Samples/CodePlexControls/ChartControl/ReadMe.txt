CustomControl Template Version 1.0

#####
Goals
#####

This Template is meant to be used in conjunction with the CustomControlDemoApplication template.
They should both be part of the same solution and they should be the only two project in that
solution.

The goal of this template is to define a projet that should be easy to merge with other projects of
the same format with a minimum of cracking open and parsing files. At the same time, this serves as 
the format for final release and the format for individual pieces of that release.


#####
Key pieces
#####

Controls: (.cs files)
One or more controls and pieces of their heirarchy publicly exposed as necessary. Controls should
have comments on all public API. Exposed controls should all be part of the same namespace which is
"WpfControlToolkit".

Themes: (/themes/*.xaml files)
For controls that support templates, generic templates should be the minimum items provided and
they should reside in a controlname.generic.xaml named resource dictionary. All control specific
dictionaries should then be added to a MergedResourceDictionary in generic.xaml. If more themes are
provided, they should follow this same format for example. controlname.luna.metallic.xaml would be
merged into luna.metallic.xaml.

Properties: (AssemblyIfno.cs)
Currently this file is told to look for a generic resource dictionary and not theme specific ones.
If theme specific resource dictionaries are provided, the themeinfo attribute will need updating.
