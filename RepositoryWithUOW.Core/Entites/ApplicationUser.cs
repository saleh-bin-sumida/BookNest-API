using Microsoft.AspNetCore.Identity;
using RepositoryWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Entites;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public List<RefreshToken>? RefreshTokens { get; set; }
}
