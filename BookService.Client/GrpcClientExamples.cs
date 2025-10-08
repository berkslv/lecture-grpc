using Grpc.Net.Client;
using Grpc.Core;
using BookService.Grpc;

namespace BookService.Client;

public class GrpcClientExamples
{
    private readonly GrpcChannel _channel;
    private readonly Grpc.BookService.BookServiceClient _client;

    public GrpcClientExamples(string serverAddress = "http://localhost:5001")
    {
        _channel = GrpcChannel.ForAddress(serverAddress, new GrpcChannelOptions
        {
            // HTTPS tanımı disable ediliyor. Production ortamda dikkatlı kullanınız
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        });
        _client = new Grpc.BookService.BookServiceClient(_channel);
    }

    // Unary RPC Örneği
    public async Task UnaryCallExample()
    {
        Console.WriteLine("=== Unary Call Example ===");
        var request = new GetBooksRequest();
        var response = await _client.GetBooksAsync(request);

        Console.WriteLine($"Received {response.Books.Count} books:");
        foreach (var book in response.Books)
        {
            Console.WriteLine($"- {book.Title} by {book.Author} ({book.Year}) - ${book.Price}");
        }
    }

    // Server Streaming Örneği
    public async Task ServerStreamingExample()
    {
        Console.WriteLine("\n=== Server Streaming Example ===");
        var request = new StreamBooksRequest { DelayMs = 500 };

        using var call = _client.StreamBooks(request);
        
        await foreach (var book in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"Received: {book.Title} - ${book.Price}");
        }
        
        Console.WriteLine("Streaming completed");
    }

    // Client Streaming Örneği
    public async Task ClientStreamingExample()
    {
        Console.WriteLine("\n=== Client Streaming Example ===");

        using var call = _client.AddBooksStream();

        var booksToAdd = new[]
        {
            new Book { Title = "Design Patterns", Author = "Gang of Four", Year = 1994, Price = 59.99 },
            new Book { Title = "Head First Design Patterns", Author = "Eric Freeman", Year = 2004, Price = 44.99 },
            new Book { Title = "Microservices Patterns", Author = "Chris Richardson", Year = 2018, Price = 49.99 }
        };

        foreach (var book in booksToAdd)
        {
            await call.RequestStream.WriteAsync(book);
            Console.WriteLine($"Sent: {book.Title}");
            await Task.Delay(300); // Simüle edilmiş gecikme
        }

        await call.RequestStream.CompleteAsync();
        var response = await call;
        
        Console.WriteLine($"Result: {response.Message}");
    }

    // Bidirectional Streaming Örneği
    public async Task BidirectionalStreamingExample()
    {
        Console.WriteLine("\n=== Bidirectional Streaming Example ===");

        using var call = _client.SearchBooksStream();

        // Background'da sonuçları oku
        var readTask = Task.Run(async () =>
        {
            await foreach (var book in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Found: {book.Title} by {book.Author}");
            }
        });

        // Arama sorgularını gönder
        var queries = new[] { "Clean", "Design", "Refactoring", "Domain" };
        
        foreach (var query in queries)
        {
            Console.WriteLine($"Searching for: {query}");
            await call.RequestStream.WriteAsync(new SearchRequest { Query = query });
            await Task.Delay(1000); // Her aramadan sonra bekle
        }

        await call.RequestStream.CompleteAsync();
        await readTask;
        
        Console.WriteLine("Bidirectional streaming completed");
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.ShutdownAsync();
        _channel.Dispose();
    }
}
