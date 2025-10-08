using Bogus;

namespace BookService.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }

    public static List<Book> GenerateBooks()
    {
        var books = new List<Book>();
        
        // Generate 195 more books using Bogus (total 200)
        var bookFaker = new Faker<Book>()
            .RuleFor(b => b.Id, f => f.IndexFaker + 6) // Start from ID 6
            .RuleFor(b => b.Title, f => f.Commerce.ProductName() + ": " + f.Commerce.ProductAdjective() + " Guide")
            .RuleFor(b => b.Author, f => f.Name.FullName())
            .RuleFor(b => b.Year, f => f.Random.Int(1990, 2024))
            .RuleFor(b => b.Price, f => Math.Round(f.Random.Decimal(15.99m, 99.99m), 2));

        var generatedBooks = bookFaker.Generate(1000);
        books.AddRange(generatedBooks);

        return books;
    }
}
