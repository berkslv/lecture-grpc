using BookService.Client;

Console.WriteLine("BookService gRPC Client");
Console.WriteLine("======================\n");

await using var client = new GrpcClientExamples();

try
{
    await client.UnaryCallExample();
    await client.ServerStreamingExample();
    await client.ClientStreamingExample();
    await client.BidirectionalStreamingExample();
}
catch (Exception ex)
{
    Console.WriteLine($"\nError: {ex.Message}");
    Console.WriteLine("Make sure the BookService server is running!");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
