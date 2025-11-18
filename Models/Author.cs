using System.ComponentModel.DataAnnotations;

namespace Databas2.Models;

public class Author
{
    public int AuthorId { get; set; }
    [Required, MaxLength(30)]
    public string Name  { get; set; }
    [Required, MaxLength(30)]
    public string Country  { get; set; }
}