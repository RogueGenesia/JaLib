dotnet build --configuration Release

# update manifest
$xml = [Xml] (Get-Content ".\JaLib.csproj")
$manifest = Get-Content ".\manifest.json" | ConvertFrom-Json

$manifest.description = $xml.Project.PropertyGroup[0].Description
$manifest.version_number = $xml.Project.PropertyGroup[0].Version

$manifest | ConvertTo-Json | Out-File ".\manifest.json"

Compress-Archive -Path ".\bin\Release\net6.0\JaLib.dll", ".\icon.png", ".\manifest.json", ".\README.md" -CompressionLevel "Optimal" -DestinationPath ".\JaLib.zip" -Force