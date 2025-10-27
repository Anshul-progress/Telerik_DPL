```

BenchmarkDotNet v0.13.12, macOS 15.5 (24F74) [Darwin 24.5.0]
Apple M3 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 9.0.108
  [Host]     : .NET 9.0.7 (9.0.725.31616), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.7 (9.0.725.31616), Arm64 RyuJIT AdvSIMD


```
| Method                 | Mean     | Error     | StdDev    | Gen0    | Gen1   | Allocated |
|----------------------- |---------:|----------:|----------:|--------:|-------:|----------:|
| CreateDocument_ToBytes | 1.332 ms | 0.0257 ms | 0.0215 ms | 70.3125 | 5.8594 | 575.07 KB |
| CreateDocument_ToFile  | 5.850 ms | 0.1744 ms | 0.5141 ms | 62.5000 |      - | 569.73 KB |
