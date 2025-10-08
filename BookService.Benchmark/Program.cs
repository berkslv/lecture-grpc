using BenchmarkDotNet.Running;
using BookService.Benchmark;

Console.WriteLine("BookService Performance Benchmark");
Console.WriteLine("==================================\n");
Console.WriteLine("Make sure BookService is running on http://localhost:5000 before starting!");
Console.WriteLine("\nPress any key to start benchmarks...");
Console.ReadKey();

var summary = BenchmarkRunner.Run<BookServiceBenchmark>();
