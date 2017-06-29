echo %APPVEYOR_BUILD_NUMBER%

dotnet pack src/Cimpress.Nancy --configuration Release
dotnet pack src/Cimpress.Nancy.Logging --configuration Release
dotnet pack src/Cimpress.Nancy.Security --configuration Release
dotnet pack src/Cimpress.Nancy.Swagger --configuration Release