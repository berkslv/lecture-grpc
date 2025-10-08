using Grpc.Core;
using BookService.Grpc;
using BookService.Models;

namespace BookService.Services;

public class GrpcBookService : Grpc.BookService.BookServiceBase
{
    private static readonly List<BookService.Grpc.Book> Books = BookService.Models.Book.GenerateBooks()
        .Select(b => new BookService.Grpc.Book { Id = b.Id, Title = b.Title, Author = b.Author, Year = b.Year, Price = (double)b.Price })
        .ToList();

    // Unary RPC - Klasik request-response
    public override Task<BookList> GetBooks(GetBooksRequest request, ServerCallContext context)
    {
        var response = new BookList();
        response.Books.AddRange(Books);
        
        return Task.FromResult(response);
    }

    // Server Streaming - Sunucu kitapları tek tek stream eder
    public override async Task StreamBooks(StreamBooksRequest request, 
        IServerStreamWriter<BookService.Grpc.Book> responseStream, 
        ServerCallContext context)
    {
        foreach (var book in Books)
        {
            // İptal kontrolü
            if (context.CancellationToken.IsCancellationRequested)
            {
                break;
            }

            await responseStream.WriteAsync(book);

            // Belirtilen süre kadar bekle (streaming'i göstermek için)
            if (request.DelayMs > 0)
            {
                await Task.Delay(request.DelayMs);
            }
        }
    }

    // Client Streaming - İstemci birden fazla kitap gönderir
    public override async Task<AddBooksResponse> AddBooksStream(
        IAsyncStreamReader<BookService.Grpc.Book> requestStream, 
        ServerCallContext context)
    {
        int count = 0;
        int maxId = Books.Any() ? Books.Max(b => b.Id) : 0;

        await foreach (var book in requestStream.ReadAllAsync())
        {
            book.Id = ++maxId;
            Books.Add(book);
            count++;
        }

        return new AddBooksResponse
        {
            Count = count,
            Message = $"Successfully added {count} books"
        };
    }

    // Bidirectional Streaming - İki yönlü gerçek zamanlı arama
    public override async Task SearchBooksStream(
        IAsyncStreamReader<SearchRequest> requestStream,
        IServerStreamWriter<BookService.Grpc.Book> responseStream,
        ServerCallContext context)
    {
        await foreach (var searchRequest in requestStream.ReadAllAsync())
        {
            var results = Books.Where(b => 
                b.Title.Contains(searchRequest.Query, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(searchRequest.Query, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            foreach (var book in results)
            {
                await responseStream.WriteAsync(book);
            }
        }
    }
}
