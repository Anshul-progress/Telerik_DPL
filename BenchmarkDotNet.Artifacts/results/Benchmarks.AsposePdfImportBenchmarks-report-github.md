```

BenchmarkDotNet v0.13.12, macOS 15.5 (24F74) [Darwin 24.5.0]
Apple M3 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 9.0.108
  [Host]     : .NET 9.0.7 (9.0.725.31616), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.7 (9.0.725.31616), Arm64 RyuJIT AdvSIMD


```
| Method                                   | FileName | Mean       | Error     | StdDev    | Gen0       | Gen1       | Gen2      | Allocated |
|----------------------------------------- |--------- |-----------:|----------:|----------:|-----------:|-----------:|----------:|----------:|
| &#39;Aspose Import - page count only&#39;        | 30mb.pdf |   3.643 ms | 0.0191 ms | 0.0169 ms |   503.9063 |   207.0313 |   85.9375 |   5.83 MB |
| &#39;Aspose Import+Export round-trip length&#39; | 30mb.pdf | 555.710 ms | 2.4682 ms | 2.1880 ms | 79000.0000 | 20000.0000 | 3000.0000 | 745.98 MB |
