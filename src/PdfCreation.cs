using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Primitives;

public class PdfCreation
{
    public static RadFixedDocument CreateDocument()
    {
        var document = new RadFixedDocument();
        var page = document.Pages.AddPage();
        var editor = new FixedContentEditor(page);

        // Simple visible content for benchmarking
        editor.Position.Translate(50, 750);
        editor.DrawText("Benchmark Sample PDF - PdfCreation");
        editor.Position.Translate(0, -25);
        editor.DrawText("Timestamp: " + System.DateTime.UtcNow.ToString("o"));

        return document;
    }

    public static void CreatePdf(string fileName = "output.pdf")
    {
        var document = CreateDocument();
        var provider = new PdfFormatProvider();
        using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
        provider.Export(document, fs, null);
    }
}
