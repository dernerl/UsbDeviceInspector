using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

Console.WriteLine("USB Device Debug Tool");
Console.WriteLine("=====================\n");

// Same AQS filter as the app
string aqsFilter = "System.Devices.InterfaceClassGuid:=\"{6AC27878-A6FA-4155-BA85-F98F491D4F33}\"";
string[] additionalProperties = new[]
{
    "System.ItemNameDisplay",
    "System.Devices.Manufacturer",
    "System.Devices.HardwareIds",
    "System.Devices.DeviceInstanceId"
};

Console.WriteLine($"AQS Filter: {aqsFilter}");
Console.WriteLine($"Additional Properties: {string.Join(", ", additionalProperties)}");
Console.WriteLine("\nEnumerating devices...\n");

try
{
    var devices = await DeviceInformation.FindAllAsync(aqsFilter, additionalProperties);

    Console.WriteLine($"Found {devices.Count} device(s)\n");

    if (devices.Count == 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("No devices found matching the filter!");
        Console.ResetColor();
        Console.WriteLine("\nPossible reasons:");
        Console.WriteLine("- No USB storage devices connected");
        Console.WriteLine("- Windows storage interface GUID not matching");
        Console.WriteLine("- Permissions issue");
        return;
    }

    int index = 0;
    int usbCount = 0;

    foreach (var device in devices)
    {
        index++;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Device #{index}:");
        Console.ResetColor();

        Console.WriteLine($"  Id: {device.Id}");
        Console.WriteLine($"  Name: {device.Name ?? "(null)"}");
        Console.WriteLine($"  IsEnabled: {device.IsEnabled}");
        Console.WriteLine($"  Properties: {device.Properties.Count}");

        // Check for Device Instance Path
        if (device.Properties.TryGetValue("System.Devices.DeviceInstanceId", out var instancePathObj))
        {
            string? instancePath = instancePathObj?.ToString();
            Console.WriteLine($"  DeviceInstanceId: {instancePath}");

            if (!string.IsNullOrEmpty(instancePath))
            {
                // Check for direct USB enumeration
                bool isUsb = instancePath.StartsWith("USB\\", StringComparison.OrdinalIgnoreCase);

                // Check for WPD-enumerated USB storage (SWD\WPDBUSENUM\..._USBSTOR_...)
                bool isWpdUsb = instancePath.StartsWith("SWD\\WPDBUSENUM\\", StringComparison.OrdinalIgnoreCase) &&
                                instancePath.Contains("USBSTOR", StringComparison.OrdinalIgnoreCase);

                if (isUsb || isWpdUsb)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(isWpdUsb
                        ? "    -> USB device via WPD layer (will be included)"
                        : "    -> USB device (will be included)");
                    Console.ResetColor();
                    usbCount++;
                }
                else if (instancePath.StartsWith("SD\\", StringComparison.OrdinalIgnoreCase) ||
                         instancePath.StartsWith("SDBUS\\", StringComparison.OrdinalIgnoreCase) ||
                         instancePath.StartsWith("MMC\\", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("    -> SD card reader (filtered out)");
                    Console.ResetColor();
                }
                else if (instancePath.StartsWith("SCSI\\", StringComparison.OrdinalIgnoreCase) ||
                         instancePath.StartsWith("SATA\\", StringComparison.OrdinalIgnoreCase) ||
                         instancePath.StartsWith("NVME\\", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("    -> Internal drive (filtered out)");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"    -> Unknown type (filtered out)");
                    Console.ResetColor();
                }
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  DeviceInstanceId: NOT FOUND");
            Console.ResetColor();
        }

        // Check for other properties
        if (device.Properties.TryGetValue("System.Devices.Manufacturer", out var mfg))
        {
            Console.WriteLine($"  Manufacturer: {mfg}");
        }

        Console.WriteLine();
    }

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("Summary:");
    Console.ResetColor();
    Console.WriteLine($"  Total devices: {devices.Count}");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  USB storage devices: {usbCount}");
    Console.ResetColor();
    Console.WriteLine($"  Filtered out: {devices.Count - usbCount}");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Type: {ex.GetType().Name}");
    Console.ResetColor();
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
