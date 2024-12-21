using RepositoryWithUOW.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace BookNest.Core.Models;

public class BookQueryParameters
{
    public int AuthorId { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    [AllowedValues([OrderByStrings.Ascending, OrderByStrings.Desending])]
    public string OrderByDirection { get; set; }
    public string PropertyName { get; set; }

}
