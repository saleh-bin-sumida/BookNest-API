using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;
using System.ComponentModel.DataAnnotations;
namespace RepositoryWithUOW.Core.Models;

public class RegisterModel
{
    [Required]

    public string UserName { get; set; }

    [Required]
    public string FullName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }


    [Required]
    public string ConfirmPassword { get; set; }


}