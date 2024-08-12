using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.Models;

public class UserManagerResponse
{
    public string Message { get; set; }
    public bool IsSucced { get; set; }
    public IEnumerable<string> Errors { get; set; }
  
    public UserManagerResponse(string message, bool isSucced, IEnumerable<string> errors = null)
    {
        Message = message;
        IsSucced = isSucced;
        this.Errors = errors;
    }
}
