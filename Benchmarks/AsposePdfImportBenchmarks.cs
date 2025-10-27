using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Aspose.Pdf;

namespace Benchmarks
{
    /// <summary>
    /// Mirrors PdfImportBenchmarks but uses Aspose.Pdf to measure comparable import and round-trip export performance
    /// for the same test file (default 30mb.pdf) copied to output TestCases folder.
    /// </summary>
    [MemoryDiagnoser]
    public class AsposePdfImportBenchmarks
    {
        private byte[]? _pdfBytes;

        [Params("30mb.pdf")] public string FileName { get; set; } = string.Empty;

        [GlobalSetup]
        public void Setup()
        {
            string baseDir = AppContext.BaseDirectory;
            string path = Path.Combine(baseDir, "TestCases", FileName);
            if (File.Exists(path))
            {
                _pdfBytes = File.ReadAllBytes(path);
            }
            else
            {
                // tiny fallback: create an empty Aspose document and save to memory
                var fallback = new Document();
                using var ms = new MemoryStream();
                fallback.Save(ms);
                _pdfBytes = ms.ToArray();
            }
        }

        [Benchmark(Description = "Aspose Import - page count only")]
        public int Import_PageCount()
        {
            using var ms = new MemoryStream(_pdfBytes!);
            var doc = new Document(ms);
            return doc.Pages.Count;
        }

        [Benchmark(Description = "Aspose Import+Export round-trip length")]
        public long Import_RoundTripLength()
        {
            using var msIn = new MemoryStream(_pdfBytes!);
            var doc = new Document(msIn);
            using var msOut = new MemoryStream();
            doc.Save(msOut); // export
            return msOut.Length;
        }
    }
}
