# Benchmark Comparison Report

Date: 2025-09-30
Environment: macOS 15.5, Apple M3 Pro (12 cores), .NET 9.0.7, BenchmarkDotNet 0.13.12
Test File: `TestCases/30mb.pdf`

## Scope

Side-by-side performance of Telerik vs Aspose PDF processing for:

- Creation (Telerik only small synthetic doc)
- Import (page count only)
- Import + Export round-trip (memory stream)

Both import benchmark classes now directly import from bytes (no helper indirection) for symmetry.

## Raw Results (Latest Run)

### Creation (Telerik)

| Method                 | Mean     | Allocated |
| ---------------------- | -------- | --------- |
| CreateDocument_ToBytes | ~1.33 ms | ~575 KB   |
| CreateDocument_ToFile  | ~5.85 ms | ~570 KB   |

### Import Only (Page Count)

| Library | Mean                                                                    | Allocated | Notes                                              |
| ------- | ----------------------------------------------------------------------- | --------- | -------------------------------------------------- |
| Telerik | 5.36–5.14 s (two recent runs; latest 5.36 s after direct import change) | ~3.77 GB  | Full model materialization (all pages/resources)   |
| Aspose  | 3.52 ms                                                                 | ~5.83 MB  | Likely lightweight structural parse for page count |

### Import + Export Round-Trip

| Library | Mean                           | Allocated | Notes                                |
| ------- | ------------------------------ | --------- | ------------------------------------ |
| Telerik | 10.90–10.73 s (latest 10.90 s) | ~7.01 GB  | Full in-memory model + serialization |
| Aspose  | 553.8 ms                       | ~746 MB   | More memory efficient pipeline       |

## Derived Metrics (Approximate)

| Scenario         | Telerik | Aspose  | Ratio (Telerik / Aspose) |
| ---------------- | ------- | ------- | ------------------------ |
| Import only time | 5.36 s  | 3.52 ms | ~1520x slower            |
| Round-trip time  | 10.90 s | 0.554 s | ~19.7x slower            |
| Import alloc     | 3.77 GB | 5.83 MB | ~647x larger             |
| Round-trip alloc | 7.01 GB | 746 MB  | ~9.4x larger             |

## Interpretation

- The massive gap in import-only suggests different architectural strategies: Telerik eagerly constructs a rich object graph; Aspose defers heavy parsing until deeper inspection, enabling near-constant-time page counting.
- Round-trip brings Aspose cost up (it must fully materialize enough to write), but it still remains far below Telerik in both time and memory for this file size.
- Memory pressure from repeated Telerik large-file operations could trigger GC churn and potential throughput degradation under load.

## Method Parity Confirmation

- Both benchmarks use `[Params("30mb.pdf")]` and read from `AppContext.BaseDirectory/TestCases/30mb.pdf`.
- Both page-count tests: byte[] -> MemoryStream -> Import -> `Pages.Count`.
- Both round-trips: Import -> Export to MemoryStream -> return length.
- No extra stamping or mutation code remains in benchmark paths.

## Caveats

1. The 30mb.pdf internal structure (object count, image compression, fonts) influences parse complexity—results may shift with different PDFs.
2. Aspose import-only figure does not imply _fully ready_ page content; forcing full traversal might narrow the gap.
3. BenchmarkDotNet measures managed allocations; native or unmanaged resources (e.g., font raster caches) are not included.
4. Results are single-threaded scenarios; concurrency patterns could change relative behavior.

## Suggested Next Steps

| Priority | Action                                                                                     | Rationale                                        |
| -------- | ------------------------------------------------------------------------------------------ | ------------------------------------------------ |
| High     | Add forced full traversal benchmark for Aspose (e.g., iterate each page & access contents) | Achieve apples-to-apples parsing cost comparison |
| Medium   | Investigate Telerik lazy/lite import options (if available)                                | Reduce up-front cost for metadata queries        |
| Medium   | Track pages/sec & MB/sec metrics automatically                                             | Easier trend comparisons across files            |
| Low      | Add multiple file sizes (small, medium, large)                                             | Observe scaling curve                            |
| Low      | Persist historical runs (CSV append)                                                       | Trend regression detection                       |

## Reproduction

Run benchmarks (all suites):

```bash
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet run -c Release -- bench
```

## Raw Artifacts

BenchmarkDotNet generated per-suite CSV/HTML under `BenchmarkDotNet.Artifacts/results/`. Keep these for historical baselining.

## Summary

Aspose currently dominates in both speed and memory for the tested large PDF, especially for metadata-style operations (page count). Telerik’s strength may lie in rich in-memory manipulation scenarios, but for pure import/round-trip throughput on large monolithic documents, optimization headroom is significant.

-- End of Report --
