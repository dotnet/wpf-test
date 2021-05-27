[[Version]]
0.9 - July 14, 2003
1.0 - August 18, 2003
1.1 Beta - January 30, 2003
1.1 Release - April 21, 2004
1.2 Beta - May 10, 2004
1.2 Release - April 6, 2005

[[Overview]]
This is a managed wrapper (API) for PICT 3.1 (Latest).

The sln and csproj are for the Everett (2003) release of Visual Studio .NET.

To run the samples/use the DLL, you must get PICT 3.1 from 

https://github.com/Microsoft/pict ->

pict.exe 
pict.amd64.exe 
pict.ia64.exe

and put it in the same directory as Microsoft.Test.Pict.dll .

[[Fixes]]
ObjectArray* now disposes enumerators
Temporary pict file is File.Delete'd more often
PairwiseSettings will now be populated with the random seed after generation
Now works with Doubles on German, etc.
v1.1 Release fixed an issue with Warnings

[[Changes]]
PairwiseParameterConstraint is now deprecated since constraints are much more general now.

Cache file is now cache31.bin .

The directory for pict.exe now defaults to the same directory as the managed wrapper DLL.
You can change it by changing PairwiseSettings.PictDirectory. Note that you can include 
environment variables in there, too: %TESTROOT%\pictbinaries, etc.

The pict executable file is _always_ determined by your PROCESSOR_ARCHITECTURE environment variable:
	x86   --> pict.exe
	ia64  --> pict.ia64.exe
	amd64 --> pict.amd64.exe

[[Additions]]
new WarningBehavior off of PairwiseSettings lets you control things better (especially because 
there are more warnings.)

There is a Weight property off of PairwiseValue; see the description at https://github.com/Microsoft/pict/doc
