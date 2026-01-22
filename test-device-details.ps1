# Simple test to show what devices are being detected
Write-Host "Testing USB Device Detection..." -ForegroundColor Cyan
Write-Host ""

# Run the app and capture debug output
$appPath = "src\UsbDeviceInspector\bin\x64\Debug\net8.0-windows10.0.19041.0\UsbDeviceInspector.exe"

if (Test-Path $appPath) {
    Write-Host "App found at: $appPath" -ForegroundColor Green
    Write-Host ""
    Write-Host "Launch the app and check the Debug Output window in VS Code" -ForegroundColor Yellow
    Write-Host "Look for lines like:" -ForegroundColor Yellow
    Write-Host "  - 'Found X device(s) before filtering'" -ForegroundColor Gray
    Write-Host "  - 'Y USB storage device(s) after filtering'" -ForegroundColor Gray
    Write-Host "  - Device Instance Paths starting with 'USB\' or 'SWD\WPDBUSENUM\'" -ForegroundColor Gray
} else {
    Write-Host "App not built yet. Run: dotnet build -p:Platform=x64" -ForegroundColor Red
}
