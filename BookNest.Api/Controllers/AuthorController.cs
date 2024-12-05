using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepositoryWithUOW.Core.DTOs;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;

namespace RepositoryWithUOW.Api.Controllers
{
    [ApiController]
    [Route("api/author")]
    public class AuthorController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var authors = await _unitOfWork.Authors.IndexAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _unitOfWork.Authors.Find(x => x.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpGet("GetAuthorByName")]
        public async Task<IActionResult> GetAuthorByName(string name)
        {
            var author = await _unitOfWork.Authors.Find(x => x.Name == name);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpPost("AddAuthor")]
        public async Task<IActionResult> AddAuthor(AuthorDTO authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            await _unitOfWork.Authors.Add(author);
            await _unitOfWork.CompleteAsync();
            return Ok(author.Id);
        }

        [HttpPut("UpdateAuthor")]
        public async Task<IActionResult> UpdateAuthor(AuthorDTO authorDto)
        {
            var updatedAuthor = _mapper.Map<Author>(authorDto);
            await _unitOfWork.Authors.Update(updatedAuthor);
            await _unitOfWork.CompleteAsync();
            return Ok(updatedAuthor.Id);
        }

        [HttpDelete("DeleteAuthor")]
        public async Task<IActionResult> DeleteAuthor(int authorId)
        {
            await _unitOfWork.Authors.Delete(authorId);
            await _unitOfWork.CompleteAsync();
            return Ok(authorId);
        }
    }
}
