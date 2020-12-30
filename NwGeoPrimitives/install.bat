@echo off
set s=%1%
echo Project directory: %s%
set p=C:\ProgramData\Autodesk\ApplicationPlugins
set q=NwGeoPrimitives
set d=%p%\%q%.bundle
echo Installing: %d%
if exist %d% rmdir /s /q %d%
mkdir %d%
mkdir %d%\Contents
copy %s%PackageContents.xml %d% 
copy %s%bin\Debug\%q%.dll %d%\Contents
