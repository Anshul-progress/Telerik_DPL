using System;
using System.IO;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing; // retained only for potential future use (not needed now)

public static class PdfImport
{
    private static readonly PdfFormatProvider Provider = new PdfFormatProvider();

    /// <summary>
    /// Reads a PDF file's bytes from the TestCases folder (case-sensitive as requested) and returns a RadFixedDocument.
    /// </summary>
    public static RadFixedDocument ImportFromTestCases(string fileName = "30mb.pdf")
    {
        string baseDir = AppContext.BaseDirectory;
        string testDir = Path.Combine(baseDir, "TestCases");
        string path = Path.Combine(testDir, fileName);
        if (!File.Exists(path))
        {
            return CreateMessageDocument($"Missing TestCases/{fileName}");
        }
        byte[] bytes = File.ReadAllBytes(path);
        return ImportFromBytes(bytes);
    }

    /// <summary>
    /// Imports a PDF from raw bytes and returns the Telerik RadFixedDocument.
    /// Optionally adds a small stamp (text block) to the first page and/or appends a new trailing page.
    /// This shows how to mutate the imported document with Telerik's FixedContentEditor API.
    /// </summary>
    /// <param name="pdfBytes">Raw PDF bytes.</param>
    /// <param name="addStamp">If true, draws a stamp text on the first page (creates one if no pages).</param>
    /// <param name="stampText">Custom stamp text (defaults to "Imported").</param>
    /// <param name="appendInfoPage">If true, appends a new page summarizing basic properties (page count before append).</param>
    public static RadFixedDocument ImportFromBytes(byte[] pdfBytes, bool addStamp = false, string? stampText = null, bool appendInfoPage = false)
    {
        using var ms = new MemoryStream(pdfBytes);
        var doc = Provider.Import(ms, null);

        if (addStamp)
        {
            // Ensure at least one page exists
            var page = doc.Pages.Count > 0 ? doc.Pages[0] : doc.Pages.AddPage();
            var editor = new FixedContentEditor(page);
            // Y= top of page is 0 in PDF coordinate system used by Telerik (origin bottom-left); translate from top by using page.Size.Height if needed.
            // We'll position near upper-left margin (20, pageHeight - 40) to avoid overlapping existing content too much.
            double y = page.Size.Height - 40;
            editor.Position.Translate(20, y);
            editor.DrawText((stampText ?? "Imported") + " - " + DateTime.UtcNow.ToString("u"));
        }

        if (appendInfoPage)
        {
            int originalPages = doc.Pages.Count;
            var infoPage = doc.Pages.AddPage();
            var infoEditor = new FixedContentEditor(infoPage);
            infoEditor.Position.Translate(50, infoPage.Size.Height - 80);
            infoEditor.DrawText($"Info Page (added)\nOriginal pages: {originalPages}\nGenerated UTC: {DateTime.UtcNow:u}");
        }

        return doc;
    }

    private static RadFixedDocument CreateMessageDocument(string message)
    {
        var doc = new RadFixedDocument();
        var page = doc.Pages.AddPage();
        var editor = new FixedContentEditor(page);
        editor.Position.Translate(50, 750);
        editor.DrawText(message);
        return doc;
    }
}
