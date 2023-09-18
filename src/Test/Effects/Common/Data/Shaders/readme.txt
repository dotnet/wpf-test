This fold contains source file for shaders. 


To compile shaders you need to install the DX SDK and add the utility directory with the 
fxc.exe compiler to your path environment variable. 

Since there is no fxc in SD, the shaders would be built separately, with compile.cmd, and result is under folder Binaries.
Once a new shader added, need to modify compile.cmd to compile it. 

The Shaders.nativeproj binplace compiled shaders. 
