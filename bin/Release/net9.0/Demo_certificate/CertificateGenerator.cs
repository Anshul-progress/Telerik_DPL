using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RazorLight;
using DPL_project.DemoCertificate;
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;
using Telerik.Reporting.Processing;

namespace DPL_project.DemoCertificate
{
    public class CertificateGenerator
    {
        public async Task<string> GenerateCertificateAsync(CertificateViewModel model)
        {
            // Paths
            string templateDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Demo_certificate");
            string templateFile = Path.Combine(templateDir, "Demo_certificate.cshtml");
            if (!File.Exists(templateFile))
                throw new FileNotFoundException("Demo certificate Razor template not found", templateFile);

            // RazorLight engine
            var engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(templateDir)
                .UseMemoryCachingProvider()
                .Build();

            // Load external CSS file if SignatureCertificateCssTag is empty
            string cssPath = Path.Combine(templateDir, "certificate.css");
            if (string.IsNullOrWhiteSpace(model.SignatureCertificateCssTag) && File.Exists(cssPath))
            {
                var css = File.ReadAllText(cssPath);
                model.SignatureCertificateCssTag = "<style>" + css + "</style>";
            }

            // Inline company logo if it references relative SVG path
            if (!string.IsNullOrEmpty(model.RightSignatureLogo) && model.RightSignatureLogo.Contains("ProgressShareFileLogoDark.svg", StringComparison.OrdinalIgnoreCase))
            {
                string logoPath = Path.Combine(templateDir, "Logo", "ProgressShareFileLogoDark.svg");
                if (File.Exists(logoPath))
                {
                    var svg = File.ReadAllText(logoPath).Replace("\r", " ").Replace("\n", " ");
                    string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(svg));
                    model.RightSignatureLogo = $"<img alt='Logo' width='200' src='data:image/svg+xml;base64,{encoded}' />";
                }
            }

            // We no longer inject QR into the model pre-render; template has a <!--QR_PLACEHOLDER--> marker at bottom.

            // Render HTML from template + model (QR placeholder still present)
            string html = await engine.CompileRenderAsync(Path.GetFileName(templateFile), model);
            if (html.Contains("&lt;"))
            {
                html = WebUtility.HtmlDecode(html);
            }

            // After rendering, replace QR placeholder with generated QR fragment
            const string qrUrl = "https://www.google.com"; // Fixed per requirement
            string qrHtml = QrCodeService.GenerateQrHtml(qrUrl, 150);
            html = html.Replace("<!--QR_PLACEHOLDER-->", qrHtml);

            // Import HTML -> Flow document
            var htmlProvider = new HtmlFormatProvider();
            RadFlowDocument flowDoc;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(html)))
                flowDoc = htmlProvider.Import(ms, null);

            // Export Flow -> PDF file directly
            var pdfProvider = new PdfFormatProvider();
            string outDir = Path.Combine(templateDir, "Output");
            Directory.CreateDirectory(outDir);
            string idPart = model.Document?.Id ?? Guid.NewGuid().ToString("N").Substring(0, 8);
            string outputPath = Path.Combine(outDir, $"demo_certificate_{idPart}.pdf");
            using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                pdfProvider.Export(flowDoc, fs, null);

            // Also copy to project root's Demo_certificate/Output (locate root by finding the csproj up the directory tree)
            try
            {
                string? current = AppDomain.CurrentDomain.BaseDirectory;
                string? projectRoot = null;
                for (int i = 0; i < 6 && current != null; i++)
                {
                    if (File.Exists(Path.Combine(current, "DPL_project.csproj")))
                    {
                        projectRoot = current;
                        break;
                    }
                    current = Directory.GetParent(current)?.FullName;
                }
                if (projectRoot != null)
                {
                    string sourceOutputDir = Path.Combine(projectRoot, "Demo_certificate", "Output");
                    Directory.CreateDirectory(sourceOutputDir);
                    string destPath = Path.Combine(sourceOutputDir, Path.GetFileName(outputPath));
                    File.Copy(outputPath, destPath, overwrite: true);
                }
            }
            catch { /* Swallow copy issues without failing generation */ }

            return outputPath;
        }

        // (Removed inline QR generation methods; now handled by QrCodeService)
    }
}
