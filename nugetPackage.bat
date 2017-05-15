echo %APPVEYOR_BUILD_NUMBER%

dotnet pack src/Cimpress.Nancy --configuration Release --version-suffix "alpha"
dotnet pack src/Cimpress.Nancy.Logging --configuration Release --version-suffix "alpha"
dotnet pack src/Cimpress.Nancy.Security --configuration Release --version-suffix "alpha"
dotnet pack src/Cimpress.Nancy.Swagger --configuration Release --version-suffix "alpha"