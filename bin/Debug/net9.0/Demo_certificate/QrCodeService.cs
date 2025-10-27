// using System;
// using System.Drawing;
// using System.Drawing.Imaging;
// using System.IO;
// using System.Drawing.Printing;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;
using Telerik.Reporting.Processing;
//using Telerik.Reporting.Barcodes;

namespace DPL_project.DemoCertificate
{
    /// <summary>
    /// QR Code Generator - renders QR code report to PNG using Telerik.Drawing.Skia
    /// </summary>
    public static class QrCodeService
    {
        /// <summary>
        /// Generates a pure square QR code PNG and saves to file
        /// </summary>
        public static string GenerateQrFilePath(string url, string outputDir, int sizePx)
        {
            Console.WriteLine($"[QR] Generating pure QR code image for URL: {url} at size {sizePx}px");

            try
            {
                byte[] pngBytes = GeneratePureQrCodePng(url, sizePx);
                Console.WriteLine($"[QR] Pure QR PNG generated successfully: {pngBytes.Length} bytes");

                // Save to file
                Directory.CreateDirectory(outputDir);
                string qrFilePath = Path.Combine(outputDir, "qr_code.png");
                File.WriteAllBytes(qrFilePath, pngBytes);
                Console.WriteLine($"[QR] QR code saved to: {qrFilePath}");

                return qrFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QR] Error generating QR: {ex.Message}");
                Console.WriteLine($"[QR] Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"[QR] Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        private static byte[] GeneratePureQrCodePng(string url, int sizePx)
        {
            Console.WriteLine($"[QR] Creating {sizePx}x{sizePx} QR code centered on A4 page");

            const int dpi = 96;
            double qrSizeInches = sizePx / (double)dpi;

            // A4 dimensions
            const double a4HeightInches = 11.69;
            const double a4WidthInches = 8.27;

            // Calculate center position for QR code on A4 page
            double xOffset = (a4WidthInches - qrSizeInches) / 2.0;
            double yOffset = (a4HeightInches - qrSizeInches) / 2.0;

            Console.WriteLine($"[QR] QR size: {qrSizeInches:F3}\" × {qrSizeInches:F3}\"");
            Console.WriteLine($"[QR] A4 page: {a4WidthInches:F3}\" × {a4HeightInches:F3}\"");
            Console.WriteLine($"[QR] QR centered at position: X={xOffset:F3}\", Y={yOffset:F3}\"");

            // Create minimal report with QR code
            var report = new Telerik.Reporting.Report();
            // Don't set PaperKind - it's Windows-only and not needed for IMAGE rendering
            // PaperSize alone is sufficient for cross-platform image generation
            report.PageSettings.Landscape = false;
            report.PageSettings.Margins = new MarginsU(Unit.Inch(0), Unit.Inch(0), Unit.Inch(0), Unit.Inch(0));
            report.PageSettings.PaperSize = new SizeU(Unit.Inch(a4WidthInches), Unit.Inch(a4HeightInches));
            report.Width = Unit.Inch(a4WidthInches);

            var detailSection = new Telerik.Reporting.DetailSection
            {
                Height = Unit.Inch(a4HeightInches),
            };

            report.Items.Add(detailSection);

            var qrCode = new Telerik.Reporting.Barcode
            {
                Value = url,
                Encoder = new Telerik.Reporting.Barcodes.QRCodeEncoder
                {
                    ErrorCorrectionLevel = Telerik.Reporting.Barcodes.QRCode.ErrorCorrectionLevel.M
                },
                Location = new PointU(Unit.Inch(xOffset), Unit.Inch(yOffset)), // Centered position
                Size = new SizeU(Unit.Inch(qrSizeInches), Unit.Inch(qrSizeInches)),
                Stretch = true,  // Make QR fill entire area
                Style =
                {
                    BackgroundColor = System.Drawing.Color.White,
                    Color = System.Drawing.Color.Black,
                    LineColor = System.Drawing.Color.Black,
                    BorderStyle =
                    {
                        Default = Telerik.Reporting.Drawing.BorderType.None // No border
                    }
                }
            };
            detailSection.Items.Add(qrCode);

            // Render using IMAGE format with 96 DPI
            var reportProcessor = new ReportProcessor();
            var instanceReportSource = new InstanceReportSource { ReportDocument = report };

            var imageSettings = new System.Collections.Hashtable
            {
                ["OutputFormat"] = "PNG",
                ["DpiX"] = dpi,
                ["DpiY"] = dpi,
                ["StartPage"] = 0,
                ["EndPage"] = 0
            };

            var result = reportProcessor.RenderReport("IMAGE", instanceReportSource, imageSettings);
            Console.WriteLine($"[QR] QR code rendered: {result.DocumentBytes.Length} bytes");

            return result.DocumentBytes;
        }
    }
}
