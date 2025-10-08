# REST vs gRPC Comparison Project

A comprehensive demo application comparing REST and gRPC technologies using .NET 9.

## 📚 Project Structure

```
lecture-grpc/
├── BookService/              # Main service (REST + gRPC)
│   ├── Controllers/          # REST API controllers
│   ├── Services/            # gRPC service implementations
│   ├── Models/              # Data models
│   └── Protos/              # Protocol Buffer definitions
├── BookService.Client/      # gRPC client examples
└── BookService.Benchmark/   # Performance tests
```

## 🚀 Quick Start

### Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [grpcurl](https://github.com/fullstorydev/grpcurl) (optional, for testing)

### 1. Run the Service

```bash
cd BookService
dotnet run
```

The service will be available at:
- REST API: http://localhost:5000/api/books
- Swagger UI: http://localhost:5000/swagger
- gRPC: http://localhost:5001

### 2. Run the Client

Open a new terminal:

```bash
cd BookService.Client
dotnet run
```

This demonstrates all gRPC streaming modes (Unary, Server Streaming, Client Streaming, Bidirectional).

### 3. Run Benchmarks

```bash
cd BookService.Benchmark
dotnet run -c Release
```

## 🔧 API Usage

### REST API Examples

```bash
# Get all books
curl http://localhost:5000/api/books

# Get a specific book
curl http://localhost:5000/api/books/1

# Add a new book
curl -X POST http://localhost:5000/api/books \
  -H "Content-Type: application/json" \
  -d '{"title":"New Book","author":"Author Name","year":2024,"price":29.99}'
```

### gRPC API Examples (using grpcurl)

```bash
# List services
grpcurl -plaintext localhost:5001 list

# Get all books (Unary RPC)
grpcurl -plaintext localhost:5001 books.BookService/GetBooks

# Stream books (Server Streaming)
grpcurl -plaintext -d '{"delay_ms": 500}' localhost:5001 books.BookService/StreamBooks
```

## 📊 gRPC Streaming Modes

The project demonstrates 4 different RPC types:

### 1. Unary RPC
Traditional request-response model.

```csharp
var response = await client.GetBooksAsync(new GetBooksRequest());
```

### 2. Server Streaming RPC
Client sends one request, server streams multiple responses.

```csharp
using var call = client.StreamBooks(new StreamBooksRequest { DelayMs = 500 });
await foreach (var book in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine(book.Title);
}
```

### 3. Client Streaming RPC
Client sends multiple messages, server returns one response.

```csharp
using var call = client.AddBooksStream();
await call.RequestStream.WriteAsync(book1);
await call.RequestStream.WriteAsync(book2);
await call.RequestStream.CompleteAsync();
var response = await call;
```

### 4. Bidirectional Streaming RPC
Both client and server stream messages independently.

```csharp
using var call = client.SearchBooksStream();

// Read in background
var readTask = Task.Run(async () => {
    await foreach (var book in call.ResponseStream.ReadAllAsync())
        Console.WriteLine(book.Title);
});

// Write requests
await call.RequestStream.WriteAsync(new SearchRequest { Query = "Clean" });
await call.RequestStream.CompleteAsync();
await readTask;
```

## 🎯 Performance Comparison

Benchmark results typically show:

| Metric | REST | gRPC | Improvement |
|--------|------|------|------------|
| **Latency** | ~332μs | ~297μs | 10% faster |
| **Memory** | ~23KB | ~26KB | 12% more |
| **Multiple Calls** | 3.2ms | 2.9ms | 11% faster |

## 🛠️ Technologies

- **.NET 9** - Modern web framework
- **ASP.NET Core** - REST API
- **gRPC** - High-performance RPC framework
- **Protocol Buffers** - Data serialization
- **BenchmarkDotNet** - Performance measurement
- **Bogus** - Fake data generation

## 🤝 When to Use REST vs gRPC

### Use REST for:
- ✅ Public APIs
- ✅ Browser integration
- ✅ Simple CRUD operations
- ✅ Caching/CDN support

### Use gRPC for:
- ✅ Microservice communication
- ✅ High-performance requirements
- ✅ Real-time streaming
- ✅ Type safety and code generation
- ✅ Low latency critical applications

## 📝 Notes

- Service supports both HTTP/1.1 and HTTP/2
- gRPC reflection is enabled in development
- Swagger UI automatically documents REST endpoints
- Run benchmarks in Release mode for accurate results

---

**Author:** [@berkslv](https://x.com/berkslv)
