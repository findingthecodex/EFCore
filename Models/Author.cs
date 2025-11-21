using System.ComponentModel.DataAnnotations;

namespace Databas2.Models;

public class Author
{
    public int AuthorId { get; set; }
    [Required, MaxLength(30)]
    public string AuthorName  { get; set; }
    [Required, MaxLength(30)]
    public string AuthorCountry  { get; set; }

    public List<Book> Books { get; set; } = new();
}