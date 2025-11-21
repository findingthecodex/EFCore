using System.ComponentModel.DataAnnotations;

namespace Databas2.Models;

public class Book
{
    public int BookId { get; set; }
    [Required, MaxLength(100)]
    public string BookTitle { get; set; }
    [Required, MaxLength(10)]
    public int? ReleaseYear { get; set; }
    
    // Foreignkey
    public int? AuthorId { get; set; }
    
    //Navigering
    public Author? Author { get; set; }
}