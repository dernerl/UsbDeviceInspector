# PowerShell Script to enumerate USB devices using the same API as the app
Add-Type -AssemblyName "Windows.Devices, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime"

$aqsFilter = 'System.Devices.InterfaceClassGuid:="{6AC27878-A6FA-4155-BA85-F98F491D4F33}"'
$additionalProperties = @(
    "System.ItemNameDisplay",
    "System.Devices.Manufacturer",
    "System.Devices.HardwareIds",
    "System.Devices.DeviceInstanceId"
)

Write-Host "Searching for USB storage devices..." -ForegroundColor Cyan
Write-Host "AQS Filter: $aqsFilter" -ForegroundColor Gray
Write-Host ""

try {
    $asyncOp = [Windows.Devices.Enumeration.DeviceInformation]::FindAllAsync($aqsFilter, $additionalProperties)
    $devices = $asyncOp.GetResults()

    Write-Host "Found $($devices.Count) device(s) matching filter" -ForegroundColor Green
    Write-Host ""

    $index = 0
    foreach ($device in $devices) {
        $index++
        Write-Host "Device #$index" -ForegroundColor Yellow
        Write-Host "  Id: $($device.Id)" -ForegroundColor White
        Write-Host "  Name: $($device.Name)" -ForegroundColor White
        Write-Host "  IsEnabled: $($device.IsEnabled)" -ForegroundColor White
        Write-Host "  Properties Count: $($device.Properties.Count)" -ForegroundColor Gray

        # Check for Device Instance Path
        if ($device.Properties.ContainsKey("System.Devices.DeviceInstanceId")) {
            $instancePath = $device.Properties["System.Devices.DeviceInstanceId"]
            Write-Host "  DeviceInstanceId: $instancePath" -ForegroundColor Cyan

            # Check if it's a USB device
            if ($instancePath -like "USB\*") {
                Write-Host "    -> This IS a USB device!" -ForegroundColor Green
            } elseif ($instancePath -like "SD\*" -or $instancePath -like "SDBUS\*" -or $instancePath -like "MMC\*") {
                Write-Host "    -> This is an SD card reader (would be filtered out)" -ForegroundColor Yellow
            } elseif ($instancePath -like "SCSI\*" -or $instancePath -like "SATA\*" -or $instancePath -like "NVME\*") {
                Write-Host "    -> This is an internal drive (would be filtered out)" -ForegroundColor Yellow
            } else {
                Write-Host "    -> Unknown device type" -ForegroundColor Red
            }
        } else {
            Write-Host "  DeviceInstanceId: NOT FOUND" -ForegroundColor Red
        }

        Write-Host ""
    }

    # Count USB devices
    $usbCount = 0
    foreach ($device in $devices) {
        if ($device.Properties.ContainsKey("System.Devices.DeviceInstanceId")) {
            $instancePath = $device.Properties["System.Devices.DeviceInstanceId"]
            if ($instancePath -like "USB\*") {
                $usbCount++
            }
        }
    }

    Write-Host "Summary:" -ForegroundColor Cyan
    Write-Host "  Total devices found: $($devices.Count)" -ForegroundColor White
    Write-Host "  USB devices: $usbCount" -ForegroundColor Green
    Write-Host "  Would be filtered: $($devices.Count - $usbCount)" -ForegroundColor Yellow

} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}
