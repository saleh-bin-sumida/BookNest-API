﻿using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace RepositoryWithUOW.Core.Models;

public class LoginModel
{
    [Required]
    public string Username { get; set; }
    [Required]

    public string Password { get; set; }

}