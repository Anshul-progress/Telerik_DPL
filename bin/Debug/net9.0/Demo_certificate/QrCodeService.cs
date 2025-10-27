using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Printing; 
using Telerik.Reporting;
using Telerik.Reporting.Drawing;
using Telerik.Reporting.Processing;
using Telerik.Reporting.Barcodes;

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
        public static string GenerateQrFilePath(string url, string outputDir, int sizePx )
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

        /// <summary>
        /// Generates a pure square QR code image - calculate page size to get exact pixel dimensions
        /// </summary>
        private static byte[] GeneratePureQrCodePng(string url, int sizePx)
        {
            Console.WriteLine($"[QR] Creating pure {sizePx}x{sizePx} QR code");

            // Calculate page size: at 96 DPI, pixels / 96 = inches
            // So for 250px: 250 / 96 = 2.604 inches
            const int dpi = 96;
            double sizeInches = sizePx / (double)dpi;

            Console.WriteLine($"[QR] Page size: {sizeInches:F3} inches at {dpi} DPI = {sizePx}px");

            // Create minimal report with QR code
            var report = new Telerik.Reporting.Report();
            report.PageSettings.PaperKind = PaperKind.Custom;  // ADD THIS
            report.PageSettings.Landscape = false;
            report.PageSettings.Margins = new MarginsU(Unit.Inch(0), Unit.Inch(0), Unit.Inch(0), Unit.Inch(0));
            report.PageSettings.PaperSize = new SizeU(Unit.Inch(sizeInches), Unit.Inch(sizeInches));
            report.Width = Unit.Inch(sizeInches);

            var detailSection = new Telerik.Reporting.DetailSection
            {
                Height = Unit.Inch(sizeInches),

            };
           
            report.Items.Add(detailSection);

            var qrCode = new Telerik.Reporting.Barcode
            {
                Value = url,
                //Encoder = new QRCodeEncoder(),
              Encoder = new Telerik.Reporting.Barcodes.QRCodeEncoder 
                { 
                    ErrorCorrectionLevel = Telerik.Reporting.Barcodes.QRCode.ErrorCorrectionLevel.M
                },
                Location = new PointU(Unit.Inch(0), Unit.Inch(0)),
                Size = new SizeU(Unit.Inch(sizeInches), Unit.Inch(sizeInches)),
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
                ["StartPage"] = 0,  // ADD THIS
                ["EndPage"] = 0 
            };

            var result = reportProcessor.RenderReport("IMAGE", instanceReportSource, imageSettings);
            Console.WriteLine($"[QR] QR code rendered: {result.DocumentBytes.Length} bytes");

            return result.DocumentBytes;
        }
    }
}
