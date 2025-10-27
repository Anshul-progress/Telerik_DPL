using System;
using System.IO;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Aspose.Pdf;
using Aspose.Words;

/// <summary>
/// Demonstrates using both Telerik and Aspose APIs assuming licenses are applied.
/// Not called automatically; invoke from Program if you want to verify.
/// </summary>
public static class LicensedOperationsDemo
{
    public static void Run()
    {
        Console.WriteLine("[Demo] Running dual-library operations...");

        // Telerik: create a small PDF in-memory
        var telerikDoc = new RadFixedDocument();
        var page = telerikDoc.Pages.AddPage();
        var editor = new FixedContentEditor(page);
        editor.Position.Translate(50, page.Size.Height - 80);
        editor.DrawText("Telerik PDF - dual license demo " + DateTime.UtcNow.ToString("u"));
        var telerikProvider = new PdfFormatProvider();
        using (var fs = new FileStream("Dual_Telerik.pdf", FileMode.Create))
        {
            telerikProvider.Export(telerikDoc, fs, null);
        }

        // Aspose.PDF: create a simple document
        var asposePdf = new Aspose.Pdf.Document();
        asposePdf.Pages.Add().Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("Aspose.PDF - dual license demo " + DateTime.UtcNow.ToString("u")));
        asposePdf.Save("Dual_AsposePdf.pdf");

        // Aspose.Words: create a DOCX and convert to PDF
        var wordsDoc = new Aspose.Words.Document();
        var builder = new Aspose.Words.DocumentBuilder(wordsDoc);
        builder.Writeln("Aspose.Words - dual license demo " + DateTime.UtcNow.ToString("u"));
        wordsDoc.Save("Dual_Words.docx");
        wordsDoc.Save("Dual_Words.pdf");

        Console.WriteLine("[Demo] Created Dual_Telerik.pdf, Dual_AsposePdf.pdf, Dual_Words.docx, Dual_Words.pdf");
    }
}
