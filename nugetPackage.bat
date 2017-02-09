echo %APPVEYOR_BUILD_NUMBER%
powershell -Command "(gc src/project.json) -replace '.0-\*', '.%APPVEYOR_BUILD_NUMBER%-*' | Out-File src/project.json"

dotnet pack src --configuration Release --output src/NuGet --version-suffix "alpha"