using System.ComponentModel.DataAnnotations;

namespace Databas2.Models;

public class Product
{
    public int ProductId { get; set; }
    [Required, MaxLength(100)] 
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    // Navigation property
    public Category? Category { get; set; }
}