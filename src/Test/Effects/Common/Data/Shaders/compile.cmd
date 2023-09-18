@echo off
set FXCTOOL=fxc

%FXCTOOL% /T ps_2_0 ShaderSource\Simple.fx /E PS /FoShaderBytecode\Simple.ps
%FXCTOOL% /T ps_2_0 ShaderSource\Reddish.fx /E PS /FoShaderBytecode\Reddish.ps
%FXCTOOL% /T ps_2_0 ShaderSource\Reddish.fx /E PS /FoShaderBytecode\ContentReddish.ps
%FXCTOOL% /T ps_2_0 ShaderSource\MultiParameterShader.fx /E PS /FoShaderBytecode\MultiParameterShader.ps
%FXCTOOL% /T ps_3_0 ShaderSource\MultiParameterShader3.fx /E PS /FoShaderBytecode\MultiParameterShader3.ps
%FXCTOOL% /T ps_3_0 ShaderSource\MultiIntParameterShader3.fx /E PS /FoShaderBytecode\MultiIntParameterShader3.ps
%FXCTOOL% /T ps_3_0 ShaderSource\MultiTextureShader3.fx /E PS /FoShaderBytecode\MultiTextureShader3.ps
%FXCTOOL% /T ps_2_0 ShaderSource\MultiTextureShader.fx /E PS /FoShaderBytecode\MultiTextureShader.ps
%FXCTOOL% /T ps_2_0 ShaderSource\DoubleTextureShader.fx /E PS /FoShaderBytecode\DoubleTextureShader.ps
%FXCTOOL% /T ps_2_0 ShaderSource\ScaleShader.fx /E PS /FoShaderBytecode\ScaleShader.ps
%FXCTOOL% /T ps_2_0 ShaderSource\swirl.fx /E PS /FoShaderBytecode\Swirl.ps
%FXCTOOL% /T ps_2_0 ShaderSource\AllVisible.fx /E PS /FoShaderBytecode\AllVisible.ps
%FXCTOOL% /T ps_2_0 ShaderSource\average.fx /E PS /FoShaderBytecode\average.ps
%FXCTOOL% /T ps_2_0 ShaderSource\NoChange.fx /E PS /FoShaderBytecode\NoChange.ps
%FXCTOOL% /T ps_2_0 ShaderSource\VerifyDdxUvDdyUv.fx /E PS /FoShaderBytecode\VerifyDdxUvDdyUv.ps
%FXCTOOL% /T ps_2_0 ShaderSource\ReadOutsideTexture.fx /E PS /FoShaderBytecode\ReadOutsideTexture.ps