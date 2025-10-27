using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class PdfImportBenchmarks
    {
        private byte[]? _pdfBytes;
        private PdfFormatProvider _provider = null!;

        [Params("30mb.pdf")] public string FileName { get; set; } = string.Empty;

        [GlobalSetup]
        public void Setup()
        {
            _provider = new PdfFormatProvider();
            string baseDir = AppContext.BaseDirectory;
            string path = Path.Combine(baseDir, "TestCases", FileName);
            if (File.Exists(path))
            {
                _pdfBytes = File.ReadAllBytes(path);
            }
            else
            {
                // create a tiny one-page PDF as fallback so benchmark still runs
                var fallback = PdfCreation.CreateDocument();
                using var ms = new MemoryStream();
                _provider.Export(fallback, ms, null);
                _pdfBytes = ms.ToArray();
            }
        }

        [Benchmark(Description = "ImportFromTestCases - page count only")]
        public int ImportFromTestCases_PageCount()
        {
            // Direct import mirroring Aspose benchmark style: read bytes -> import -> count pages.
            using var ms = new MemoryStream(_pdfBytes!);
            var doc = _provider.Import(ms, null);
            return doc.Pages.Count;
        }

        [Benchmark(Description = "ImportFromBytes then export again (round-trip)")]
        public long ImportFromBytes_RoundTripLength()
        {
            using var msIn = new MemoryStream(_pdfBytes!);
            var doc = _provider.Import(msIn, null); // direct import (no stamp/info mutations)
            using var msOut = new MemoryStream();
            _provider.Export(doc, msOut, null); // export back out
            return msOut.Length; // size ensures work not optimized away
        }
    }
}
