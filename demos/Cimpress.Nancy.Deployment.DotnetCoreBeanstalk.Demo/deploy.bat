dotnet publish --configuration Release --output ./output ./project.json

7z a site.zip ./output/*
7z a Demo.zip site.zip aws-windows-deployment-manifest.json

eb deploy demo --message "Demo application" --timeout 20