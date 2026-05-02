$apps = @("calculator", "clocks", "explorer", "music", "photos", "server", "terminal", "ui")
$runtimes = @("win-x64", "win-x86", "win-arm64")

foreach ($app in $apps) {
    if (Test-Path ..\source-codes\$app\bin) {
        Remove-Item "..\source-codes\$app\bin" -Recurse -Force
    }
    if (Test-Path ..\source-codes\$app\obj) {
        Remove-Item "..\source-codes\$app\obj" -Recurse -Force
    }
}

foreach ($app in $apps) {
    foreach ($runtime in $runtimes) {
        if (Test-Path ..\downloads\nether-os-$app-$runtime-portable.zip) {
            Remove-Item "..\downloads\nether-os-$app-$runtime-portable.zip" -Recurse -Force
            
        }
        if (Test-Path ..\downloads\nether-os-$app-$runtime-installer.zip) {
            Remove-Item "..\downloads\nether-os-$app-$runtime-installer.zip" -Recurse -Force
        }
    }
}