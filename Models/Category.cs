using System.ComponentModel.DataAnnotations;

namespace Databas2.Models;

// Enkel modelklass (Entity) som EF Core kommer mappa till eb tabell "Category"
public class Category
{
    // Primäarnyckel. EF Core ser "CategoryId" och gör det till PK
    public int CategoryId { get; set; }
    
    // Required = får inte vara Null (varken i C# eller i databasen)
    // MaxLenght = genererar en kolumn med maxlängd 100 + används vid validering
    [Required, MaxLength(100)]
    public string CategoryName { get; set; } = null!;
    
    //Optional (Nullable '?') text med max 250
    [MaxLength (250)]
    public string? CategoryDescription { get; set; }
    
    // Many to till (varje kategori har flera produkter)
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

