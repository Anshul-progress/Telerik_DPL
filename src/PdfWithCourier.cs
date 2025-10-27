using System;
using System.IO;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Fonts;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Media;

public class PdfWithCourier
{
    public RadFixedDocument GeneratePdf()
    {
        RadFixedDocument document = new RadFixedDocument();
        RadFixedPage page = document.Pages.AddPage();
        FixedContentEditor editor = new FixedContentEditor(page);

        // Use built-in PDF base font (Courier) - no external TTF required
        editor.Position.Translate(50, 750);
        editor.TextProperties.Font = FontsRepository.Courier;
        editor.TextProperties.FontSize = 14;
        editor.DrawText("Courier Sample (Regular 14pt)");

        // Move down for next lines
        editor.Position.Translate(0, -25);
        editor.DrawText("All characters monospaced: 1234567890 ABCDEFG xyz");

        // Larger size
        editor.Position.Translate(0, -30);
        editor.TextProperties.FontSize = 18;
        editor.DrawText("Larger Courier 18pt");

        // Show simple aligned columns (monospacing demo)
        editor.Position.Translate(0, -40);
        editor.TextProperties.FontSize = 12;
        editor.DrawText("Col1    Col2    Col3    Col4");
        editor.Position.Translate(0, -18);
        editor.DrawText("1001    Alpha   Ready   TRUE");
        editor.Position.Translate(0, -18);
        editor.DrawText("1002    Beta    Waiting FALSE");
        editor.Position.Translate(0, -18);
        editor.DrawText("1003    Gamma   Active  TRUE");

        // Reset font size and add footer note
        editor.Position.Translate(0, -40);
        editor.TextProperties.FontSize = 10;
        editor.DrawText("(Courier base font embedded by PDF viewer)\n");

        return document;
    }
}
