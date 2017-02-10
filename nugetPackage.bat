echo %APPVEYOR_BUILD_NUMBER%
powershell -Command "(gc src/Cimpress.Nancy/project.json) -replace '.0-\*', '.%APPVEYOR_BUILD_NUMBER%-*' | Out-File src/Cimpress.Nancy/project.json"
powershell -Command "(gc src/Cimpress.Nancy.Logging/project.json) -replace '.0-\*', '.%APPVEYOR_BUILD_NUMBER%-*' | Out-File src/Cimpress.Nancy.Logging/project.json"
powershell -Command "(gc src/Cimpress.Nancy.Security/project.json) -replace '.0-\*', '.%APPVEYOR_BUILD_NUMBER%-*' | Out-File src/Cimpress.Nancy.Security/project.json"
powershell -Command "(gc src/Cimpress.Nancy.Swagger/project.json) -replace '.0-\*', '.%APPVEYOR_BUILD_NUMBER%-*' | Out-File src/Cimpress.Nancy.Swagger/project.json"

dotnet pack src/Cimpress.Nancy --configuration Release --output src/Cimpress.Nancy/NuGet --version-suffix "alpha"
dotnet pack src/Cimpress.Nancy.Logging --configuration Release --output src/Cimpress.Nancy.Logging/NuGet --version-suffix "alpha"
dotnet pack src/Cimpress.Nancy.Security --configuration Release --output src/Cimpress.Nancy.Security/NuGet --version-suffix "alpha"
dotnet pack src/Cimpress.Nancy.Swagger --configuration Release --output src/Cimpress.Nancy.Swagger/NuGet --version-suffix "alpha"