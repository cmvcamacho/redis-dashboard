@echo off

:: Check command syntax
IF '%1'=='' (
	echo Usage: Install.bat [environment]
	echo
	goto end
)


:: Gets the current path
set rootDir=%~dp0

::Determine what architecture EXEs should run on.
if %PROCESSOR_ARCHITECTURE%==x86 (
	::use the Build Tools 2015
	::set msbuild=C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
	set msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"
) else (
	::set msbuild=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe
	set msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\amd64\msbuild.exe"
)

%msbuild% default.build /t:PublishHost /p:Environment=%1


:end