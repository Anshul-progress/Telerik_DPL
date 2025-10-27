using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using System;
using System.IO;

// Apply all third-party licenses (Telerik & Aspose)
AllLicenses.Apply();

if (args.Contains("bench", StringComparer.OrdinalIgnoreCase))
{
    Console.WriteLine("Running benchmarks (this may take a while)...");
    Benchmarks.BenchmarkRunnerHost.Run();
    return;
}

if (args.Contains("demo-cert", StringComparer.OrdinalIgnoreCase))
{
    Console.WriteLine("Generating demo certificate (QR + CSS)...");
    try
    {
        var model = new DPL_project.DemoCertificate.CertificateViewModel();
        var gen = new DPL_project.DemoCertificate.CertificateGenerator();
        string pdfPath = gen.GenerateCertificateAsync(model).GetAwaiter().GetResult();
        Console.WriteLine("Demo certificate generated at: " + Path.GetFullPath(pdfPath));
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("Demo certificate generation failed: " + ex.Message);
        Console.Error.WriteLine(ex);
        Environment.ExitCode = 1;
    }
    return;
}

// (QR generation path removed; QR now handled inside certificate-specific flows if needed)

// Default generation workflow
PdfCreation.CreatePdf();
Console.WriteLine("PDF created successfully at output.pdf");

Fill_Color_Text.GeneratePdf();
Console.WriteLine("PDF generated: BlockFillColorTest.pdf");

var courierGen = new PdfWithCourier();
var courierDoc = courierGen.GeneratePdf();
var courierProvider = new Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.PdfFormatProvider();
using (var fs = new FileStream("CourierFont.pdf", FileMode.Create))
{
    courierProvider.Export(courierDoc, fs, null);
}
Console.WriteLine("PDF generated: CourierFont.pdf");

// Import 30mb.pdf from TestCases folder if present (falls back to placeholder if missing)
var imported = PdfImport.ImportFromTestCases();
var importProvider = new Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.PdfFormatProvider();
using (var ifs = new FileStream("Imported.pdf", FileMode.Create))
{
    importProvider.Export(imported, ifs, null);
}
Console.WriteLine("PDF generated: Imported.pdf (or placeholder if sample missing)");


