using BookService.Services;

var builder = WebApplication.CreateBuilder(args);

// REST API servisleri
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// gRPC servisleri
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

// REST endpoints
app.MapControllers();

// gRPC endpoints
app.MapGrpcService<GrpcBookService>();

app.MapGet("/", () => "BookService is running! REST API: /swagger | gRPC: port 5001");

// Swagger (REST için)
app.UseSwagger();
app.UseSwaggerUI();

// gRPC reflection (development için)
app.MapGrpcReflectionService();

app.Run();
