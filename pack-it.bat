REM Version
set /p version="Version? [e.g. 0.0.5]"
REM 32bit
mkdir TEMP
copy SymbSearch\bin\x86\Release\ TEMP
copy LICENCE TEMP
cd TEMP
"C:\Program Files\7-Zip\7z.exe" a -r ..\RELEASE\SymbSearch-%version%-win32.zip *
del /Q *
REM 64bit
copy SymbSearch\bin\x64\Release\ TEMP
copy LICENCE TEMP
cd TEMP
"C:\Program Files\7-Zip\7z.exe" a -r ..\RELEASE\SymbSearch-%version%-win64.zip *
cd ..
rd /Q /S TEMP
