For PomBaml testing:  The msvc*80.dll files in the X86 and AMD subfolders are necessary for testing with configurations which lack them, e.g., 4.5-only runs.
They are required by LSUtils.exe, which is a 2.0 CLR assembly used by PomBaml testing that PInvokes and needs the VC80 runtime.
These files  will be copied to the Test folder when WPF_TESTBUILD_TARGETVERSION = 4.X, as specified in LocTools.nativeproj.
