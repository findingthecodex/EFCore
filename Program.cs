using Databas2;
using Databas2.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory, "shop.db"));
//säkerställ DB + migrations + Seed
using (var db = new ShopContext())
{
    // Migrate Async: Skapar databasen om den inte finns
    // Kör bara om det inte finns några kategorier sen innan
    await db.Database.MigrateAsync();

    // Enkel seeding för databasen
    // Kör bara om det inte finns några kategorier sen innan
    if (!await db.Categories.AnyAsync())
    {
        db.Categories.AddRange(
            new Category { CategoryName = "Book", CategoryDescription = "All books we have" },
            new Category { CategoryName = "Movies", CategoryDescription = "All movies we have" },
            new Category { CategoryName =  "PC-parts", CategoryDescription = "All PC-parts we have"});
        
        db.Products.AddRange(
            new Product { ProductName = "GPU", Description = "RTX 5080", Price = 8999.99m, CategoryId = 3 },
            new Product { ProductName = "RAM", Description = "32GB Kingston", Price = 1299.99m, CategoryId = 3 }
            );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }
    
    // Product
    if (!await db.Products.AnyAsync())
    {
        db.Products.AddRange(
            new Product { ProductName = "GPU", Description = "RTX 5080", Price = 8999.99m, CategoryId = 1 },
            new Product { ProductName = "RAM", Description = "32GB Kingston", Price = 1299.99m, CategoryId = 1 }
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }
    
    //Author
    if (!await db.Authors.AnyAsync())
    {
        db.Authors.AddRange(
            new Author { AuthorName = "G.Lucas", AuthorCountry = "USA", AuthorId = 987 },
            new Author { AuthorName = "J.R.R Tolkien", AuthorCountry = "England", AuthorId = 976 }
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }
    
    //Book
    if (!await db.Books.AnyAsync())
    {
        db.Books.AddRange(
            new Book {  BookTitle = "Star Wars", ReleaseYear = 1944, AuthorId = 987 },
            new Book { BookTitle  = "The Hobbit", ReleaseYear = 1892, AuthorId = 976 }
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }
}

// CLI för CRUD; CREATE; READ; UPDATE; DE:ETE
while (true)
{
    Console.WriteLine("Meny");
    Console.WriteLine("1 - Categories");
    Console.WriteLine("2 - Products");
    Console.WriteLine("3 - Library");
    Console.WriteLine();
    Console.WriteLine("exit - Shut down");
    Console.WriteLine(">");

    var choice = Console.ReadLine();
    if (choice == "1")
        await CategoryMenu();
    else if (choice == "2")
        await ProductMenu();
    else if (choice == "3")
        await LibraryMenu();
    else
    {
        Console.WriteLine("Invalid choice");
    }
}

// SweKing
    static async Task CategoryMenu()
    {
        while (true)
        {
            Console.WriteLine("\nCategories: List (1) | Category+ (2) | edit <id> | delete <id> | Back (..)");
            Console.WriteLine(">");
            var line = Console.ReadLine()?.Trim() ?? string.Empty;
            // Hoppa över tomma rader
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.Equals("..", StringComparison.OrdinalIgnoreCase))
            {
                break; // Avsluta programmet, hoppa ur loopen
            }

            // Delar upp raden på mellanslag: tex "edit 2" --> ["edit", "2"]
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            // Enkel switch för kommandotolkning
            switch (cmd)
            {
                case "1":
                    // Lista våra categories
                    await ListAsync();
                    break;
                case "2":
                    // Lägg till en category
                    await AddAsync();
                    break;
                case "edit":
                    // Redigera en category
                    // Kräver Id efter kommandot "edit"
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                    {
                        Console.WriteLine("Usage: Edit <id>");
                        break;
                    }

                    await EditCategoryAsync(id);
                    break;
                case "delete":
                    // Radera en category
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var idD))
                    {
                        Console.WriteLine("Usage: Delete <id>");
                        break;
                    }

                    await DeleteCategoryAsync(idD);
                    break;
                default:
                    Console.WriteLine($"Unknown command.");
                    break;
            }
        }
    }

    static async Task ProductMenu()
    {
        while (true)
        {
            Console.WriteLine("\nProducts: List (1) | Add (2) | edit <id> | delete <id> | Back (..)");
            Console.WriteLine(">");
            var line = Console.ReadLine()?.Trim() ?? string.Empty;
            // Hoppa över tomma rader
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.Equals("..", StringComparison.OrdinalIgnoreCase))
            {
                break; // Avsluta programmet, hoppa ur loopen
            }

            // Delar upp raden på mellanslag: tex "edit 2" --> ["edit", "2"]
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            // Enkel switch för kommandotolkning
            switch (cmd)
            {
                case "cl":
                    // Visa kategorier först så användaren vet vilka Id som finns
                    await ListAsync();
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var idCategory))
                    {
                        Console.WriteLine("Usage: cl <categoryId> (Products By Category)");
                        break;
                    }

                    await ProductsByCategoryAsync(idCategory);
                    break;
                case "1":
                    // Lista våra produkter
                    await ListProductsAsync();
                    break;
                case "2":
                    // Lägg till en product
                    await AddProductAsync();
                    break;
                case "edit":
                    // Redigera en product
                    // Kräver Id efter kommandot "edit"
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var editId))
                    {
                        Console.WriteLine("Usage: Edit <id>");
                        break;
                    }

                    await EditProductAsync(editId);
                    break;
                case "delete":
                    // Radera en product
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var delitId))
                    {
                        Console.WriteLine("Usage: Delete <id>");
                        break;
                    }

                    await DeleteProductAsync(delitId);
                    break;
                default:
                    Console.WriteLine($"Unknown command.");
                    break;
            }
        }
    }

    static async Task LibraryMenu()
    {
        while (true)
        {
            Console.WriteLine("\nLibrary: Authors (1) | Books (2) | Auth+ <id> (add) | Book+ <id> (add) | booksbyauthor | Back (..)");
            Console.WriteLine(">");
            var line = Console.ReadLine()?.Trim() ?? string.Empty;
            // Hoppa över tomma rader
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.Equals("..", StringComparison.OrdinalIgnoreCase))
            {
                break; // Avsluta programmet, hoppa ur loopen
            }

            // Delar upp raden på mellanslag: tex "edit 2" --> ["edit", "2"]
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            // Enkel switch för kommando
            switch (cmd)
            {
                case "1":
                    await Auths();
                    break;
                case "2":
                    await Books();
                    break;
                case "auth+":
                    await AddAuths();
                    break;
                case "book+":
                    await AddBooks();
                    break;
                case "booksbyauthor":
                    await Auths();
                    if (parts.Length < 2 || !int.TryParse(parts[1], out var authId))
                    {
                        Console.WriteLine("Usage: Books By Author <id>");
                        break;
                    }

                    await BooksByAuthor(authId);
                    break;



            }
        }
    }

    // ----- Product helpers (missing in file) -----
    static async Task ListProductsAsync()
    {
        using var db = new ShopContext();
        var products = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.ProductId)
            .ToListAsync();

        Console.WriteLine("Id | Name | Category | Price | Description");
        foreach (var p in products)
        {
            Console.WriteLine($"{p.ProductId} | {p.ProductName} | {p.Category?.CategoryName} | {p.Price} | {p.Description}");
        }
    }

    static async Task DeleteProductAsync(int id)
    {
        using var db = new ShopContext();
        var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        db.Products.Remove(product);
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    // Overload: Products by category when categoryId is provided programmatically
    static async Task ProductsByCategoryAsync(int categoryId)
    {
        using var db = new ShopContext();
        var products = await db.Products
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync();

        if (!products.Any())
        {
            Console.WriteLine("No products found for that category.");
            return;
        }

        Console.WriteLine($"Products for the category: {products[0].Category?.CategoryName}");
        Console.WriteLine("Id | Name | Price | Description");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.ProductId} | {product.ProductName} | {product.Description} | {product.Price} SEK");
        }
    }

    // ----- Category helpers moved to top-level scope -----
    static async Task DeleteCategoryAsync(int id)
    {
        using var db = new ShopContext();
        var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        if (category == null)
        {
            Console.WriteLine("Category not found.");
            return;
        }

        db.Categories.Remove(category);
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Category deleted.");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    static async Task EditCategoryAsync(int id)
    {
        using var db = new ShopContext();
        var category = await db.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        if (category == null)
        {
            Console.WriteLine("Category not found.");
            return;
        }

        Console.WriteLine($"{category.CategoryName}");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(name))
            category.CategoryName = name;

        Console.Write($"{category.CategoryDescription}");
        var description = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(description))
            category.CategoryDescription = description;

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Category edited.");
        }
        catch (DbUpdateException exeption)
        {
            Console.WriteLine(exeption.Message);
        }
    }

    static async Task EditProductAsync(int id)
    {
        using var db = new ShopContext();
        var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        Console.WriteLine($"{product.ProductName}");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(name))
            product.ProductName = name;

        Console.WriteLine($"Description {product.Description}:");
        var description = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(description))
            product.Description = description;

        Console.WriteLine($"Price ({product.Price}):");
        var priceInput = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out var newPrice))
            product.Price = newPrice;

        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product edited.");
        }
        catch (DbUpdateException exeption)
        {
            Console.WriteLine(exeption.Message);
        }
    }

    static async Task ListAsync()
    {
        using var db = new ShopContext();
        var rows = await db.Categories.AsNoTracking().OrderBy(category => category.CategoryId).ToListAsync();
        Console.WriteLine("Id | Name | Description");
        foreach (var row in rows)
            Console.WriteLine($"{row.CategoryId} | {row.CategoryName} | {row.CategoryDescription}");
    }

    static async Task AddAsync()
    {
        Console.WriteLine("Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name) || name.Length > 100)
        {
            Console.WriteLine("Name is required (max 100).");
            return;
        }

        Console.WriteLine("Description: (Optional): ");
        var description = Console.ReadLine()?.Trim() ?? string.Empty;

        using var db = new ShopContext();
        db.Categories.Add(new Category { CategoryName = name, CategoryDescription = description });
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Category added.");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine("DB Error (Maybe duplicate?)! " + exception.GetBaseException().Message);
        }
    }

    static async Task AddProductAsync()
    {
        using var db = new ShopContext();
        var category = await db.Categories.AsNoTracking().OrderBy(c => c.CategoryId).ToListAsync();
        Console.WriteLine("Available categories: ");
        foreach (var c in category)
            Console.WriteLine($"{c.CategoryId} | {c.CategoryName} | {c.CategoryDescription}");

        Console.WriteLine("Enter category ID for the new product: ");
        if (!int.TryParse(Console.ReadLine(), out int categoryId) || !category.Any(c => c.CategoryId == categoryId))
        {
            Console.WriteLine("Invalid category.");
            return;
        }

        Console.WriteLine("Enter product name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name) || name.Length > 100)
        {
            Console.WriteLine("Name is required (max 100).");
            return;
        }

        Console.WriteLine("Enter product description (Optional): ");
        var description = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.WriteLine("Enter Price");
        if (!decimal.TryParse(Console.ReadLine(), out var price))
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        db.Products.Add(new Product { ProductName = name, Description = description, Price = price, CategoryId = categoryId });
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Product added.");
        }
        catch (DbUpdateException exeption)
        {
            Console.WriteLine(exeption.Message);
        }
    }

    // ----- Library helpers -----
    static async Task Auths()
    {
        using var db = new ShopContext();
        var authors = await db.Authors.AsNoTracking().OrderBy(a => a.AuthorId).ToListAsync();
        Console.WriteLine("Id | Name | Country");
        foreach (var a in authors)
            Console.WriteLine($"{a.AuthorId} | {a.AuthorName} | {a.AuthorCountry}");
    }

    static async Task Books()
    {
        using var db = new ShopContext();
        var books = await db.Books.AsNoTracking().Include(b => b.Author).OrderBy(b => b.BookId).ToListAsync();
        Console.WriteLine("Id | Title | Year | Author");
        foreach (var b in books)
            Console.WriteLine($"{b.BookId} | {b.BookTitle} | {b.ReleaseYear} | {b.Author?.AuthorName}");
    }

    static async Task AddAuths()
    {
        Console.WriteLine("Author name:");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Name required.");
            return;
        }

        Console.WriteLine("Author country:");
        var country = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.WriteLine("Optional AuthorId (press enter to skip):");
        var idInput = Console.ReadLine()?.Trim() ?? string.Empty;
        int? id = null;
        if (!string.IsNullOrEmpty(idInput) && int.TryParse(idInput, out var parsed)) id = parsed;

        using var db = new ShopContext();
        var author = new Author { AuthorName = name, AuthorCountry = country };
        if (id.HasValue) author.AuthorId = id.Value;
        db.Authors.Add(author);
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Author added.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.GetBaseException().Message);
        }
    }

    static async Task AddBooks()
    {
        using var db = new ShopContext();
        var authors = await db.Authors.AsNoTracking().OrderBy(a => a.AuthorId).ToListAsync();
        Console.WriteLine("Available authors:");
        foreach (var a in authors) Console.WriteLine($"{a.AuthorId} | {a.AuthorName}");

        Console.WriteLine("AuthorId for the book:");
        if (!int.TryParse(Console.ReadLine(), out var authorId) || !authors.Any(a => a.AuthorId == authorId))
        {
            Console.WriteLine("Invalid author.");
            return;
        }

        Console.WriteLine("Book title:");
        var title = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Title required.");
            return;
        }

        Console.WriteLine("Release year:");
        if (!int.TryParse(Console.ReadLine(), out var year)) year = 0;

        db.Books.Add(new Book { BookTitle = title, ReleaseYear = year, AuthorId = authorId });
        try
        {
            await db.SaveChangesAsync();
            Console.WriteLine("Book added.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.GetBaseException().Message);
        }
    }

    static async Task BooksByAuthor(int authorId)
    {
        using var db = new ShopContext();
        var books = await db.Books.AsNoTracking().Where(b => b.AuthorId == authorId).Include(b => b.Author).ToListAsync();
        if (!books.Any())
        {
            Console.WriteLine("No books found for that author.");
            return;
        }
        Console.WriteLine($"Books by {books[0].Author?.AuthorName}:");
        foreach (var b in books)
            Console.WriteLine($"{b.BookId} | {b.BookTitle} | {b.ReleaseYear}");
    }

    /*static async Task SearchBookAsync()
    {
        Console.WriteLine("Search book:");
        var title = Console.ReadLine()?. Trim() ?? string.Empty;
        
        if (string.IsNullOrEmpty(title)) || title.Length > 100)
        {
            Console.WriteLine("Title is required.");
            return;
        }
        using var db 
    }*/

