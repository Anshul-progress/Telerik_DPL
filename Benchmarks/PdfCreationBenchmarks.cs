using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class PdfCreationBenchmarks
    {
        private PdfFormatProvider _provider = null!; // initialized in GlobalSetup

        [GlobalSetup]
        public void Setup()
        {
            _provider = new PdfFormatProvider();
        }

        [Benchmark]
        public byte[] CreateDocument_ToBytes()
        {
            var doc = PdfCreation.CreateDocument();
            using var ms = new MemoryStream();
            _provider.Export(doc, ms, null);
            return ms.ToArray();
        }

        [Benchmark]
        public void CreateDocument_ToFile()
        {
            var doc = PdfCreation.CreateDocument();
            using var fs = new FileStream("bench-output.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            _provider.Export(doc, fs, null);
        }
    }

    public static class BenchmarkRunnerHost
    {
        public static void Run()
        {
            // Run both benchmark classes (creation + import)
            BenchmarkRunner.Run(new[]
            {
                typeof(PdfCreationBenchmarks),
                typeof(PdfImportBenchmarks),
                typeof(AsposePdfImportBenchmarks) // added Aspose import benchmarks
            });
        }
    }
}
