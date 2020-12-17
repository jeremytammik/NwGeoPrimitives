echo off
set p=C:\ProgramData\Autodesk\ApplicationPlugins
set q=NwGeoPrimitives
echo Installing %p%\%q%.bundle...
rem IF EXIST %p%\NwGeoPrimitives.bundle" rmdir /S /Q "C:\Program Files\Autodesk\Navisworks Manage 2021\Plugins\$(TargetName)\"
rem xcopy /Y "$(TargetDir)*.*" "C:\Program Files\Autodesk\Navisworks Manage 2021\Plugins\$(TargetName)\"
