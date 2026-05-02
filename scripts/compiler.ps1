$VerbosePreference = "SilentlyContinue"

# Setting variables
$apps = @("calculator", "clocks", "explorer", "music", "photos", "server", "terminal", "ui")
$runtimes = @("win-x64", "win-x86", "win-arm64")

foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        # Compiling (build)
        dotnet.exe publish "..\source-codes\$app" -c release -r $runtime --self-contained false -p:publishsinglefile=false --output ..\downloads\$app\$runtime\sources
        # Compressing portable version
        Compress-Archive -Path "..\downloads\$app\$runtime\sources" -DestinationPath ..\downloads\nether-os-$app-$runtime-portable.zip -Force
        # Compressing installer version
        Compress-Archive -Path @("..\downloads\$app\$runtime\sources", "..\scripts\install-user.ps1") -DestinationPath ..\downloads\nether-os-$app-$runtime-installer.zip -Force
        # Removing unnecessary folders in downloads folder
        Remove-Item ..\downloads\$app -Recurse -Force
    }
}