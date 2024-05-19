@echo off

cd ../Api

dotnet restore

dotnet build

dotnet run

pause