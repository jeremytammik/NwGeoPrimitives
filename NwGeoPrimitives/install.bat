@echo off
set p=C:\ProgramData\Autodesk\ApplicationPlugins
set q=NwGeoPrimitives
set d=%p%\%q%.bundle
echo Installing %d%...
if exist %d% rmdir /s /q %d%
mkdir %d%
mkdir %d%\Contents
copy PackageContents.xml %d% 
copy bin\Debug\%q%.dll %d%\Contents
