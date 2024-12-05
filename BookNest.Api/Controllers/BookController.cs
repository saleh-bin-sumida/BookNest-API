using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryWithUOW.Core.Constants;
using RepositoryWithUOW.Core.DTOs;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;

namespace RepositoryWithUOW.Api.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetBooksAsync()
        {
            var books = await _unitOfWork.Books.IndexAsync();
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _unitOfWork.Books.Find(x => x.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BookDTO>(book));
        }

        [HttpGet("GetBookByTitle")]
        public async Task<IActionResult> GetBookByTitle(string title)
        {
            var book = await _unitOfWork.Books.Find(x => x.Title == title, new string[] { "Author" });
            if (book == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BookDTO>(book));
        }

        [HttpGet("GetAllBooksWithAuthorId")]
        public async Task<IActionResult> GetAllBooksWithAuthorId(int authorId)
        {
            var books = await _unitOfWork.Books.FindAll(x => x.AuthorId == authorId, new string[] { "Author" });
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }

        [HttpGet("GetAllBooksOrdered")]
        public async Task<IActionResult> GetAllBooksOrdered(int authorId)
        {
            var books = await _unitOfWork.Books.FindAll(x => x.AuthorId == authorId, 1, 2, OrderByStrings.Ascending, x => x.Id, new string[] { "Author" });
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }

        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBook(BookDTO bookDto)
        {
            var newBook = _mapper.Map<Book>(bookDto);
            newBook.Id = 0;
            await _unitOfWork.Books.Add(newBook);
            await _unitOfWork.CompleteAsync();
            return Ok(newBook.Id);
        }

        [HttpPut("UpdateBook")]
        public async Task<IActionResult> UpdateBook(BookDTO bookDto)
        {
            var updatedBook = _mapper.Map<Book>(bookDto);
            await _unitOfWork.Books.Update(updatedBook);
            await _unitOfWork.CompleteAsync();
            return Ok(updatedBook.Id);
        }

        [HttpDelete("DeleteBook")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            await _unitOfWork.Books.Delete(bookId);
            await _unitOfWork.CompleteAsync();
            return Ok(bookId);
        }

        [AllowAnonymous]
        [Authorize]
        [HttpGet("SpecialForBooks")]
        public IActionResult SpecialForBooks()
        {
            _unitOfWork.Books.SpecialMethodForBooks();
            if (User.IsInRole("manager"))
            {
                return Ok();
            }
            return Forbid();
        }
    }
}
