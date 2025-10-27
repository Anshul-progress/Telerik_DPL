using System;
using System.IO;

/// <summary>
/// Centralized Aspose licensing helper.
/// Priority order (first successful wins):
/// 1. Explicit base64 string passed to Ensure (argument)
/// 2. Environment variable ASPose_LICENSE_BASE64 (exact match, case-insensitive key variations handled)
/// 3. File named "aspose-license.txt" (raw XML or base64) in base directory
/// 4. Embedded placeholder (NO real license applied if none found)
/// 
/// This avoids hard-coding the license inside source.
/// </summary>
public static class AsposeLicense
{
    private static bool _applied;

    public static void Ensure(string? licenseBase64 = null)
    {
        if (_applied) return;
        try
        {
            // 1. explicit argument
            if (!string.IsNullOrWhiteSpace(licenseBase64))
            {
                TryApplyFromBase64(licenseBase64, source: "argument");
                return;
            }

            // 2. environment variable
            string? env = Environment.GetEnvironmentVariable("ASPOSE_LICENSE_BASE64")
                           ?? Environment.GetEnvironmentVariable("Aspose_License_Base64")
                           ?? Environment.GetEnvironmentVariable("ASPOSE_LICENSE");
            if (!string.IsNullOrWhiteSpace(env))
            {
                TryApplyFromBase64(env, source: "env var");
                return;
            }

            // 3. file in base directory
            string baseDir = AppContext.BaseDirectory;
            string filePath = Path.Combine(baseDir, "aspose-license.txt");
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath).Trim();
                if (LooksLikeBase64(text))
                {
                    TryApplyFromBase64(text, source: "file (base64)");
                }
                else
                {
                    TryApplyXml(text, source: "file (xml)");
                }
                return;
            }

            Console.WriteLine("[AsposeLicense] No license source found; running in evaluation mode.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AsposeLicense] Failed to apply license: {ex.Message}");
        }
    }

    private static void TryApplyFromBase64(string b64, string source)
    {
        try
        {
            var raw = Convert.FromBase64String(b64);
            using var ms = new MemoryStream(raw);
            TrySet(ms, source + " (decoded)");
        }
        catch (FormatException)
        {
            // maybe it was plain XML despite the path
            TryApplyXml(b64, source + " (not actually base64, treated as xml)");
        }
    }

    private static void TryApplyXml(string xml, string source)
    {
        using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
        TrySet(ms, source);
    }

    private static void TrySet(Stream licenseStream, string source)
    {
        try
        {
            licenseStream.Position = 0;
            var pdfLicense = new Aspose.Pdf.License();
            pdfLicense.SetLicense(licenseStream);

            licenseStream.Position = 0;
            var wordsLicense = new Aspose.Words.License();
            wordsLicense.SetLicense(licenseStream);

            _applied = true;
            Console.WriteLine($"[AsposeLicense] License applied from {source}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AsposeLicense] Source {source} failed: {ex.Message}");
        }
    }

    private static bool LooksLikeBase64(string text)
    {
        // crude heuristic: base64 length multiple of 4 and only allowed chars
        if (text.Length % 4 != 0) return false;
        foreach (char c in text)
        {
            if (!(char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=' || c == '\n' || c == '\r'))
                return false;
        }
        return true;
    }
}
