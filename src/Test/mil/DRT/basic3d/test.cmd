build /cZ
if %ERRORLEVEL% EQU 0 (
	pushd ..\obj%BUILD_ALT_DIR%\i386
	DrtBasic3D -i
	popd
)
