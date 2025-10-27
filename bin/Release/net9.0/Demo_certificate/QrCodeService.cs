using System;
using System.Text;
using System.Net;
using Telerik.Reporting;
using Telerik.Reporting.Drawing;
using Telerik.Reporting.Processing;

namespace DPL_project.DemoCertificate
{
    /// <summary>
    /// Generates a QR code using Telerik Reporting (PNG rendered to base64 <img>). No fallback is provided.
    /// </summary>
    public static class QrCodeService
    {
        /// <summary>
        /// Returns an <img> (base64 PNG) representing the QR of the provided URL.
        /// </summary>
        public static string GenerateQrHtml(string url, int sizePx = 150)
        {
            var png = GenerateQrPngViaReporting(url, sizePx);
            var b64 = Convert.ToBase64String(png);
            return $"<img alt='QR' width='{sizePx}' height='{sizePx}' src='data:image/png;base64,{b64}' />";
        }
        private static byte[] GenerateQrPngViaReporting(string url, int sizePx)
        {
            // Convert pixel size to centimeters (approx 96 dpi)
            double sizeCm = sizePx / 96.0 * 2.54;
            var report = new Telerik.Reporting.Report();
            report.PageSettings.Margins = new MarginsU(Unit.Mm(1), Unit.Mm(1), Unit.Mm(1), Unit.Mm(1));
            report.Width = Unit.Cm(sizeCm + 0.2);
            var detail = new Telerik.Reporting.DetailSection { Height = Unit.Cm(sizeCm + 0.2) };
            report.Items.Add(detail);

            var barcode = new Telerik.Reporting.Barcode
            {
                Value = url,
                Location = new PointU(Unit.Cm(0), Unit.Cm(0)),
                Size = new SizeU(Unit.Cm(sizeCm), Unit.Cm(sizeCm))
            };
            var qrType = typeof(Telerik.Reporting.Barcode).Assembly.GetType("Telerik.Reporting.Barcode.QRCode");
            var symProp = barcode.GetType().GetProperty("Symbology");
            if (qrType != null && symProp != null)
                symProp.SetValue(barcode, Activator.CreateInstance(qrType));
            detail.Items.Add(barcode);

            var processor = new ReportProcessor();
            var result = processor.RenderReport("PNG", new InstanceReportSource { ReportDocument = report }, null);
            if (result?.DocumentBytes == null || result.DocumentBytes.Length == 0)
                throw new InvalidOperationException("QR render failed");
            return result.DocumentBytes;
        }
    }
}
