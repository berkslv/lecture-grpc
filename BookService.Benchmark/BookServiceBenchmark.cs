using BenchmarkDotNet.Attributes;
using Grpc.Net.Client;
using Grpc.Core;
using BookService.Grpc;
using System.Net.Http.Json;

namespace BookService.Benchmark;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class BookServiceBenchmark
{
    private HttpClient? _httpClient;
    private GrpcChannel? _grpcChannel;
    private Grpc.BookService.BookServiceClient? _grpcClient;
    
    private const string RestUrl = "http://localhost:5000";
    private const string GrpcUrl = "http://localhost:5001";

    [GlobalSetup]
    public void Setup()
    {
        // REST client setup
        var httpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _httpClient = new HttpClient(httpHandler)
        {
            BaseAddress = new Uri(RestUrl)
        };

        // gRPC client setup
        _grpcChannel = GrpcChannel.ForAddress(GrpcUrl, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = 
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        });
        _grpcClient = new Grpc.BookService.BookServiceClient(_grpcChannel);
    }

    [Benchmark(Baseline = true)]
    public async Task<int> REST_GetAllBooks()
    {
        var books = await _httpClient!.GetFromJsonAsync<List<BookDto>>("/api/books");
        return books?.Count ?? 0;
    }

    [Benchmark]
    public async Task<int> gRPC_GetAllBooks()
    {
        var response = await _grpcClient!.GetBooksAsync(new GetBooksRequest());
        return response.Books.Count;
    }

    [Benchmark]
    public async Task<int> REST_GetAllBooks_10Times()
    {
        int totalCount = 0;
        for (int i = 0; i < 10; i++)
        {
            var books = await _httpClient!.GetFromJsonAsync<List<BookDto>>("/api/books");
            totalCount += books?.Count ?? 0;
        }
        return totalCount;
    }

    [Benchmark]
    public async Task<int> gRPC_GetAllBooks_10Times()
    {
        int totalCount = 0;
        for (int i = 0; i < 10; i++)
        {
            var response = await _grpcClient!.GetBooksAsync(new GetBooksRequest());
            totalCount += response.Books.Count;
        }
        return totalCount;
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        _httpClient?.Dispose();
        if (_grpcChannel != null)
        {
            await _grpcChannel.ShutdownAsync();
            _grpcChannel.Dispose();
        }
    }
}
