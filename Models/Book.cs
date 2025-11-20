using System.ComponentModel.DataAnnotations;

namespace Databas2.Models;

public class Book
{
    public int BookId { get; set; }
    [Required]
    public string? Title { get; set; }
    [Required]
    public int Year { get; set; }
    [Required]
    
    // Foreignkey
    public int AuthorId { get; set; }
    
    //Navigering
    public Author? Author { get; set; }
}