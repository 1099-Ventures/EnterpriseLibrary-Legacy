@ECHO OFF
REM ** Assumes we're starting in the Azuro.Common directory
del *.nupkg
nuget pack Azuro.Common.csproj -Prop Configuration=Release -Symbols
rem for /F %%a in ('dir /b *.nupkg') do (
rem  set FileName=%%~na
rem  nuget push %FileName%.nupkg
rem )

cd ..\Azuro.Common.Configuration.Design
del *.nupkg
nuget pack Azuro.Common.Configuration.Design.csproj -Prop Configuration=Release -Symbols
rem for /F %%a in ('dir /b *.nupkg') do (
rem  set FileName=%%~na
rem  nuget push %FileName%.nupkg
rem )

cd ..\Azuro.Common.MSMQ
del *.nupkg
nuget pack Azuro.Common.MSMQ.csproj -Prop Configuration=Release -Symbols
rem for /F %%a in ('dir /b *.nupkg') do (
rem  set FileName=%%~na
rem  nuget push %FileName%.nupkg
rem )

cd ..\Azuro.Common.WindowsService
del *.nupkg
nuget pack Azuro.Common.WindowsService.csproj -Prop Configuration=Release -Symbols
rem for /F %%a in ('dir /b *.nupkg') do (
rem  set FileName=%%~na
rem  nuget push %FileName%.nupkg
rem )

cd ..\Azuro.MSMQ
del *.nupkg
nuget pack Azuro.MSMQ.csproj -Prop Configuration=Release -Symbols
rem for /F %%a in ('dir /b *.nupkg') do (
rem  set FileName=%%~na
rem  nuget push %FileName%.nupkg
rem )

cd ..\Azuro.Common