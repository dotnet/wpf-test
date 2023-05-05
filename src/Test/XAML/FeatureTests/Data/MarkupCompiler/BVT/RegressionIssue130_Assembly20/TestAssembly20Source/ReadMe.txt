The files in this folder, TestAssembly20Source, are used to create TestAssembly20.dll.  It must be compiled as a .Net 2.0 project to be used for the regression test.  Compilation of a 2.0 project via the test enlistment in Razzle is not supported, so TestAssembly20.dl is directly checked into the enlistment.

Therefore, these source files can be used to recompile if necessary outside of Razzle, but are omitted from the dirs file.
