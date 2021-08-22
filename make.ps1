$targetPath = "Output\APManagerC3.exe"
$hasPath = Test-Path $targetPath
if ($hasPath) {
    Remove-Item $targetPath
}
dotnet publish -c release -r win10-x64  --self-contained=false /p:PublishSingleFile=True -o "Output"