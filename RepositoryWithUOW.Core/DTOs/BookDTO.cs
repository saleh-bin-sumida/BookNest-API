using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWithUOW.Core.DTOs;

public record BookDTO(int Id, string Title, int AuthorId);
