@echo off
set PTHDIR=D:\Temp\_DelWorking\publish\boondoggle
del %PTHDIR%\*.*

copy ..\assets\newdefaultmap.json %PTHDIR%
copy ..\assets\newdefaultmap.tmx %PTHDIR%

copy dockerfile %PTHDIR%
cd ..\..\bdconsolerunner
dotnet publish -o %PTHDIR%

pushd %PTHDIR%
::..\..\bdconsolerunner\bin\debug\netcoreapp2.1
docker build -t boondoggle .

popd
