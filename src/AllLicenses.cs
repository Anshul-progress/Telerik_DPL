using System;
using System.IO;

/// <summary>
/// Orchestrates activation of all 3rd party licenses (Telerik + Aspose).
/// Keeps one place to call early in Program startup.
/// </summary>
public static class AllLicenses
{
    private static bool _done;
    public static void Apply()
    {
        if (_done) return;
        // Telerik: relies on telerik-license.txt being present (Telerik.Licensing package loads it automatically)
        // We just verify presence for logging clarity.
        string baseDir = AppContext.BaseDirectory;
        string telerikFile = Path.Combine(baseDir, "telerik-license.txt");
        Console.WriteLine(File.Exists(telerikFile)
            ? "[Licenses] Telerik license file detected."
            : "[Licenses] Telerik license file NOT found (will run in trial/eval).");

        // Aspose: handled by AsposeLicense helper
        AsposeLicense.Ensure();
        Console.WriteLine("[Licenses] Aspose license initialization attempted.");

        _done = true;
    }
}
