using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace RepositoryWithUOW.Core.Models;

public class Book
{
    public int Id { get; set; }
    [MaxLength(100)]
    public required string Title { get; set; }
    public int AuthorId { get; set; }
    public required Author Author { get; set; }

    public DateTime CreatedDate { get; set; }  = DateTime.Now;
}
