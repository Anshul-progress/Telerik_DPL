using System.IO;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Media;

public class Fill_Color_Text
{
    public static void GeneratePdf()
    {
        // Create a new RadFixedDocument
        RadFixedDocument document = new RadFixedDocument();

        // Add a page
        RadFixedPage page = document.Pages.AddPage();

        // Create an editor for the page
        FixedContentEditor editor = new FixedContentEditor(page);

        // Position where the block will be drawn
        editor.Position.Translate(100, 100);

        // Create a block
        Block block = new Block();

        // Set font and size
       // block.TextProperties.TrySetFont(new FontFamily("Arial"));
        block.TextProperties.FontSize = 16;

        // ✅ Set the text color (FillColor works as ForegroundColor)
        //block.GraphicProperties.FillColor =  RgbColors.Black;
        editor.GraphicProperties.StrokeColor = RgbColors.Black;
        editor.GraphicProperties.StrokeThickness = 30;

        // Insert some text
        block.InsertText("This is the first line.");
        block.InsertLineBreak();
        block.InsertText("This is the second line in red.");

        // Draw the block on the page
        editor.DrawBlock(block);

        // Export to PDF
        PdfFormatProvider provider = new PdfFormatProvider();
        using (FileStream fs = new FileStream("BlockFillColorTest.pdf", FileMode.Create))
        {
            provider.Export(document, fs, null);
        }
    }
}
