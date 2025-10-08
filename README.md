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

## 🎯 Performans Karşılaştırması

Benchmark sonuçları tipik olarak şunu gösterir:

| Metrik | REST | gRPC | Kazanç |
|--------|------|------|--------|
| **Latency** | ~1.2ms | ~0.45ms | 2.7x daha hızlı |
| **Memory** | ~48KB | ~24KB | %50 daha az |
| **Payload** | ~1.3KB | ~500 bytes | %62 daha küçük |

## 🛠️ Teknolojiler

- **.NET 9** - Modern web framework
- **ASP.NET Core** - REST API
- **gRPC** - Yüksek performanslı RPC framework
- **Protocol Buffers** - Veri serializasyon
- **BenchmarkDotNet** - Performans ölçümü
- **Swagger/OpenAPI** - REST API dökümantasyonu

## 📖 Öğrenme Kaynakları

- [Microsoft gRPC Dokümantasyonu](https://learn.microsoft.com/en-us/aspnet/core/grpc/)
- [Protocol Buffers](https://protobuf.dev/)
- [grpcurl Kullanımı](https://github.com/fullstorydev/grpcurl)

## 🧪 Test Senaryoları

1. **Temel Operasyonlar**: REST ve gRPC ile CRUD işlemleri
2. **Streaming**: Farklı streaming modlarının kullanımı
3. **Performans**: REST vs gRPC hız ve kaynak kullanımı karşılaştırması
4. **Gerçek Zamanlı İletişim**: Bidirectional streaming ile canlı arama

## 🤝 Ne Zaman REST, Ne Zaman gRPC?

### REST Kullanın:
- ✅ Public API'ler
- ✅ Tarayıcı entegrasyonu
- ✅ Basit CRUD işlemleri
- ✅ Cache/CDN desteği

### gRPC Kullanın:
- ✅ Mikroservisler arası iletişim
- ✅ Yüksek performans gereksinimleri
- ✅ Gerçek zamanlı streaming
- ✅ Tip güvenliği ve kod generation
- ✅ Düşük latency kritik

## 📝 Notlar

- Servis hem HTTP/1.1 hem HTTP/2 destekler
- Development modunda gRPC reflection aktiftir
- Swagger UI otomatik olarak REST endpoint'leri dokümante eder
- Benchmark'lar Release modunda çalıştırılmalıdır

## 🎓 Demo Amaçları

Bu proje eğitim amaçlı hazırlanmıştır ve şunları gösterir:

1. REST ve gRPC'nin aynı projede birlikte kullanımı
2. 4 farklı gRPC streaming modunun implementasyonu
3. Client-server iletişim örnekleri
4. Performans karşılaştırma metodolojisi
5. Production-ready best practices

## 📄 Lisans

Bu proje MIT lisansı altında sunulmaktadır.

---

**Yazar:** [@berkslv](https://x.com/berkslv)

Sorularınız için issue açabilir veya pull request gönderebilirsiniz! 🚀
