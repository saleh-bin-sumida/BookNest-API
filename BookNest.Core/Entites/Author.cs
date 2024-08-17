using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Entites;

public class Author
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [JsonIgnore]
    public List<Book> Books { get; set; } = new();


}
