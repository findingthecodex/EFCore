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
            new Category { CategoryName = "Books", CategoryDescription = "All books we have" },
            new Category { CategoryName = "Movies", CategoryDescription = "All movies we have" }
        );
        db.Products.AddRange(
            new Product { ProductName = "GPU", Description = "RTX 5080", Price = 8999.99m, CategoryId = 1 },
            new Product { ProductName = "RAM", Description = "32GB Kingston", Price = 1299.99m, CategoryId = 1 }
        );
        await db.SaveChangesAsync();
        Console.WriteLine("Seeded db!");
    }

    if (!await db.Authors.AnyAsync())
    {
        db.Authors.AddRange(
            new Author() { Name = "J.R.R Tolkien", Country = "England", AuthorId = 1 },
            new Author() { Name = "G.Lucas", Country = "USA", AuthorId = 2 }
        );
    }
    
    if (!await db.Books.AnyAsync())
    {
        db.Books.AddRange(
            new Book() { Title = "Hobbit", Year = 1947, BookId = 123 },
            new Book() { Title = "Star Wars", Year =1997, BookId = 102 },
            new Book() { Title = "The Fellowship of the Ring", Year = 1986, BookId = 103 }
        );
    }

// CLI för CRUD; CREATE; READ; UPDATE; DE:ETE
    while (true)
    {
        Console.WriteLine(
            "\nCommands : list | (pbc) See products in categories | (p) ALl Products | (a) Author | (ap) Add Product | (add) add a Category | (deletec + <ID>) Delete category | (deletep + <ID>) Delete product | (editc + <id>) Edit category | (editp + <ID>) Edit product | exit");
        Console.Write("> ");
        var line = Console.ReadLine()?.Trim() ?? string.Empty;
        // hoppa över tomma rader
        if (string.IsNullOrEmpty(line))
        {
            continue;
        }

        if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            break; // Avsluta programmet, hoppa ut ur loopen
        }

        // Delar upp raden på mellanslag: t.ex. "edit 2" --> ["edit", "2"]
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLowerInvariant();

        // Enkel switch för kommandotolkning
        switch (cmd)
        {
            case "list":
                await ListAsync();
                break; // Lista vår categories
            case "add":
                await AddAsync();
                break; // Lägg till en category
            case "ap":
                await AddProductAsync();
                break; // Lägg till product

            // Edit category
            case "editc":
            {
                // Kräver id efter kommandot "edit"
                if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                {
                    Console.WriteLine("Usage: editc <id>");
                    break;
                }

                await EditCategoryAsync(id);
                break; // redigera en category
            }
            // Author
            case "a":
                await AuthorAsync();
                break;
        

        // Edit product
            case "editp":
            {
                if (parts.Length < 2 || !int.TryParse(parts[1], out var id))
                {
                    Console.WriteLine("Usage: editp <id>");
                    break;
                }

                await EditProductAsync(id);
                break;
            }

            case "deletep":
                // Radera en category
                if (parts.Length < 2 || !int.TryParse(parts[1], out var idD))
                {
                    Console.WriteLine("Usage: Delete <id>");
                    break;
                }

                await DeleteCategoryAsync(idD);
                break;
            // Kategorilista
            case "pbc":
                //if(parts.Length < 2 || !int.TryParse(parts[1], out var idCategory))
            {
                await ProductsByCategoryAsync();
                break;
            }
                await ProductsByCategoryAsync();
                break;
            default:
                Console.WriteLine("Unknown command: ");
                break;
            // Produker i kategorin
            case "p":
                await ListProductsAsync();
                break;
        }
    }

    return;

// Delete category
    static async Task DeleteCategoryAsync(int id)
    {
        using var db = new ShopContext();
        var category = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id); // hittar Id
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

        // Hämta raden vi vill uppdatera
        var category = await db.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        if (category == null)
        {
            Console.WriteLine("Category not found.");
            return;
        }

        // Visar nuvarande värden
        Console.WriteLine($"{category.CategoryName}");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(name))
        {
            category.CategoryName = name;
        }

        //Uppdaterar description för specifik category; TODO: FIX ME
        Console.Write($"{category.CategoryDescription}");
        var description = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(description))
        {
            category.CategoryDescription = description;
        }

        // Uppdaterad DB:n med våra ändringar
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

        // Namn
        Console.WriteLine($"{product.ProductName}");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(name))
            product.ProductName = name;

        // Beskrivning
        Console.WriteLine($"Description {product.Description}:");
        var description = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(description))
            product.Description = description;

        // Pris
        Console.WriteLine($"Price ({product.Price}):");
        var priceInput = Console.ReadLine()?.Trim() ?? string.Empty;

        if (!string.IsNullOrEmpty(priceInput) && decimal.TryParse(priceInput, out var newPrice))
        {
            product.Price = newPrice;
        }

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


// READ: Lista alla kategorier
    static async Task ListAsync()
    {
        // Ny context per operation (bra praxis)
        using var db = new ShopContext();

        // AsNoTracking = snabbare för read-only scenarion. (Ingen Change tracking)
        var rows = await db.Categories.AsNoTracking().OrderBy(category => category.CategoryId).ToListAsync();
        Console.WriteLine("Id | Name | Description");
        foreach (var row in rows)
        {
            Console.WriteLine($"{row.CategoryId} | {row.CategoryName} | {row.CategoryDescription}");
        }
    }

// CREATE: Lägg till en ny category
    static async Task AddAsync()
    {
        Console.WriteLine("Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;

        // Enkel validering
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
            // Spara våra ändringar; Trigga en INSERT + all validering/contraints i databasen
            await db.SaveChangesAsync();
            Console.WriteLine("Category added.");
        }
        catch (DbUpdateException exception)
        {
            Console.WriteLine("DB Error (Maybe duplicate?)! " + exception.GetBaseException().Message);
        }
    }

// Add product
    static async Task AddProductAsync()
    {
        // Lista ut våra kategorier
        Console.WriteLine("Available categories: ");
        await ListAsync();

        // In `Program.cs` inside AddProductAsync()
        Console.WriteLine("Choose categoryID: ");
        var categoryInput = Console.ReadLine()?.Trim() ?? string.Empty;
        if (!int.TryParse(categoryInput, out var id))
        {
            Console.WriteLine("CategoryID is required.");
            return;
        }

        Console.WriteLine("Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(name) || name.Length > 100)
        {
            Console.WriteLine("Name is required (max 100).");
            return;
        }

        Console.Write("Price: ");
        var StringPrice = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(StringPrice) || !decimal.TryParse(StringPrice, out var Price) || Price < 0)
        {
            Console.WriteLine("Price is required. Has to be a number");
            return;
        }

        Console.Write("Description: (Optional):");
        var description = Console.ReadLine();

        using var db = new ShopContext();
        db.Products.Add(new Product
        {
            ProductName = name,
            Price = Price,
            Description = description,
            CategoryId = id
        });
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

    static async Task ProductsByCategoryAsync1(int categoryId)
    {
        using var db = new ShopContext();
        var products = await db.Products.Where(x => x.CategoryId == categoryId)
            .Include(x => x.Category)
            .OrderBy(x => x.Price).ToListAsync();

        Console.WriteLine("Id | Name | Price | Description | Category");
        foreach (var product in products)
        {
            Console.WriteLine(
                $"{product.ProductId} | {product.ProductName} | {product.Price} | {product.Description} | ");
        }


    }

// Lista alla produkter som tillhör en kategori
    static async Task ProductsByCategoryAsync()
    {
        using var db = new ShopContext();
        var category = await db.Categories.AsNoTracking()
            .OrderBy(c => c.CategoryId)
            .ToListAsync();
        Console.WriteLine("Available categories: ");
        foreach (var c in category)
        {
            Console.WriteLine($"{c.CategoryId} | {c.CategoryName} | {c.CategoryDescription}");
        }

        //be användaren välja CategoryId
        Console.WriteLine("Enter category ID: ");
        if (!int.TryParse(Console.ReadLine(), out int categoryId) || !category.Any(c => c.CategoryId == categoryId))
        {
            Console.WriteLine("Invalid category.");
            return; // Avsluta om ogiltigt val
        }

        var products = await db.Products
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId)
            .Include(p => p.Category) // för att få kategorinamn
            .ToListAsync();

        if (!products.Any())
        {
            Console.WriteLine("No products found.");
            return;
        }

        Console.WriteLine($"Products for the category: {products[0].Category?.CategoryName}");
        Console.WriteLine("Id | Name | Price | Description");
        foreach (var product in products)
        {
            Console.WriteLine(
                $"{product.ProductId} | {product.ProductName} | {product.Description} | {product.Price} SEK");
        }
    }


    static async Task ListProductsAsync()
    {
        using var db = new ShopContext();
        var productRow = await db.Products.AsNoTracking().OrderBy(product => product.ProductId)
            .Include(p => p.Category).ToListAsync();
        Console.WriteLine("Id | Name | CategoryId | Description | Pris");
        foreach (var row in productRow)
        {
            Console.WriteLine(
                $"{row.ProductId} | {row.Category?.CategoryName} | {row.ProductName} | {row.Description} | {row.Price} SEK");
        }
    }
}

static async Task AuthorAsync()
{
    using var db = new ShopContext();
    var author = await db.Authors
        .AsNoTracking()
        .OrderBy(a => a.AuthorId)
        .ToListAsync();
    
    Console.WriteLine("Name | Country | AuthorId");
    foreach (var a in author)
    {
        Console.WriteLine($"{a.Name} | {a.Country} | {a.AuthorId}");
    }
}
    
        
