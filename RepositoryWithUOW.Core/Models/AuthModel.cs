using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Models;

public class AuthModel
{
    public string? Message { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Username{ get; set; }
    public string? Email{ get; set; }
    public IEnumerable<string>? Roles { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpirsOn { get; set; }

    //public IEnumerable<string> Errors { get; set; }
    //[JsonIgnore]
    //public string? RefreshToken { get; set; }
    //public DateTime RefreshTokenExpiration { get; set; }

}
