echo off
set p=C:\ProgramData\Autodesk\ApplicationPlugins
set q=NwGeoPrimitives
set d=%p%\%q%.bundle
echo Installing %d%...
IF EXIST %d% rmdir /S /Q %d%
mkdir %d%
mkdir %d%\Contents
copy PackageContents.xml %d% 
copy bin\Debug\%q%.dll %d%\Contents
