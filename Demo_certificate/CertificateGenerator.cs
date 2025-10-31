// using System;
// using System.IO;
// using System.Text;
// using System.Threading.Tasks;
using System.Net;
using RazorLight;
//using DPL_project.DemoCertificate;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Extensibility;
using Telerik.Documents.ImageUtils;

namespace DPL_project.DemoCertificate
{
    public class CertificateGenerator
    {
        static CertificateGenerator()
        {
            // Initialize Telerik Document Processing image support
            FixedExtensibilityManager.ImagePropertiesResolver = new ImagePropertiesResolver();
            FixedExtensibilityManager.JpegImageConverter = new JpegImageConverter();
            Console.WriteLine("[CertGen] Telerik image processing initialized");
        }

        public async Task<string> GenerateCertificateAsync(CertificateViewModel model)
        {
            // Paths - find project root by searching for .csproj file
            string? current = AppDomain.CurrentDomain.BaseDirectory;
            string? sourceProjectRoot = null;

            for (int i = 0; i < 6 && current != null; i++)
            {
                if (File.Exists(Path.Combine(current, "DPL_project.csproj")))
                {
                    sourceProjectRoot = current;
                    break;
                }
                current = Directory.GetParent(current)?.FullName;
            }

            if (sourceProjectRoot == null)
                sourceProjectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));

            string templateDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Demo_certificate");
            string qrOutputDir = Path.Combine(sourceProjectRoot, "Demo_certificate", "Output");
            string templateFile = Path.Combine(templateDir, "Demo_certificate.cshtml");
            if (!File.Exists(templateFile))
                throw new FileNotFoundException("Demo certificate Razor template not found", templateFile);

            // RazorLight engine
            var engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(templateDir)
                .UseMemoryCachingProvider()
                .Build();

            // Generate QR code and embed as base64
            string outDir = Path.Combine(templateDir, "Output"); // For PDF output in bin
            if (string.IsNullOrWhiteSpace(model.DocumentQr))
            {
                Console.WriteLine("[CertGen] Generating QR code...");
                string qrFilePath = QrCodeService.GenerateQrFilePath("https://www.google.com", qrOutputDir, 800);

                byte[] qrImageBytes = File.ReadAllBytes(qrFilePath);
                string base64 = Convert.ToBase64String(qrImageBytes);
                model.DocumentQr = $"<img class='qr-code' alt='QR Code' src='data:image/png;base64,{base64}' style='width: 100px; height: 100px;display: block;' />";
                Console.WriteLine($"[CertGen] QR embedded as base64 ({qrImageBytes.Length} bytes)");
            }

            // Render Razor template to HTML

            // Render Razor template to HTML
            Console.WriteLine("[CertGen] Rendering Razor template...");
            string html = await engine.CompileRenderAsync(Path.GetFileName(templateFile), model);

            if (html.Contains("&lt;"))
                html = WebUtility.HtmlDecode(html);

            // Save HTML for debugging
            string htmlDebugPath = Path.Combine(outDir, "debug_certificate.html");
            File.WriteAllText(htmlDebugPath, html);
            Console.WriteLine($"[CertGen] HTML saved for debugging: {htmlDebugPath}");

            // Import HTML to RadFlowDocument
            var htmlProvider = new HtmlFormatProvider();
            RadFlowDocument flowDoc = htmlProvider.Import(html, null);

            // Set page margins (20pt = ~0.28 inches)
            foreach (var section in flowDoc.Sections)
            {
                section.PageMargins = new Telerik.Windows.Documents.Primitives.Padding(25, 25, 25, 25);
            }

            Console.WriteLine("[CertGen] HTML imported to RadFlowDocument");

            // Export to PDF
            var pdfProvider = new PdfFormatProvider();
            Directory.CreateDirectory(outDir);
            string idPart = model.Document?.Id ?? Guid.NewGuid().ToString("N").Substring(0, 8);
            string outputPath = Path.Combine(outDir, $"demo_certificate_{idPart}.pdf");
            using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                pdfProvider.Export(flowDoc, fs, null);

            Console.WriteLine($"[CertGen] PDF generated: {outputPath}");

            // Copy to project source Demo_certificate/Output folder
            try
            {
                if (sourceProjectRoot != null)
                {
                    string sourceOutputDir = Path.Combine(sourceProjectRoot, "Demo_certificate", "Output");
                    Directory.CreateDirectory(sourceOutputDir);

                    string destPath = Path.Combine(sourceOutputDir, Path.GetFileName(outputPath));
                    File.Copy(outputPath, destPath, overwrite: true);
                    Console.WriteLine($"[CertGen] PDF also copied to: {destPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CertGen] Warning: Could not copy to source folder: {ex.Message}");
            }

            return outputPath;
        }
    }
}
