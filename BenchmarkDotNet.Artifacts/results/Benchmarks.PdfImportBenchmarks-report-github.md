```

BenchmarkDotNet v0.13.12, macOS 15.5 (24F74) [Darwin 24.5.0]
Apple M3 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 9.0.108
  [Host]     : .NET 9.0.7 (9.0.725.31616), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.7 (9.0.725.31616), Arm64 RyuJIT AdvSIMD


```
| Method                                           | FileName | Mean     | Error    | StdDev   | Gen0        | Gen1        | Gen2      | Allocated |
|------------------------------------------------- |--------- |---------:|---------:|---------:|------------:|------------:|----------:|----------:|
| &#39;ImportFromTestCases - page count only&#39;          | 30mb.pdf |  5.354 s | 0.1065 s | 0.1093 s | 486000.0000 | 177000.0000 | 7000.0000 |   3.74 GB |
| &#39;ImportFromBytes then export again (round-trip)&#39; | 30mb.pdf | 11.004 s | 0.2131 s | 0.2188 s | 904000.0000 | 340000.0000 | 9000.0000 |   7.01 GB |
