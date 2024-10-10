using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryWithUOW.Core.Constants;
using RepositoryWithUOW.Core.DTOs;
using RepositoryWithUOW.Core.Entites;
using RepositoryWithUOW.Core.Interfaces;
using static System.Reflection.Metadata.BlobBuilder;


namespace RepositoryWithUWO.Api.Controllers;

[ApiController]
[Route("api/book")]

//[Authorize]
public class BookController(IUnitOfWork BooksRepository, IMapper mapper) : ControllerBase
{
    private IUnitOfWork _booksRepository = BooksRepository;
    private readonly IMapper mapper = mapper;



    [HttpGet]
    public async Task<IActionResult> GetBooksAsync()
    {
        var books = await _booksRepository.Books.IndexAsync();
        return  Ok(mapper.Map<IEnumerable<BookDTO>>(books));
    }


    [HttpGet]
    [Route("{Id}")]
    public async Task< IActionResult> GetBookById(int Id)
    {
        var book = await _booksRepository.Books.Find(x => x.Id == Id);
       return Ok(  mapper.Map<BookDTO>(book));

    }


    [HttpGet("GetBookByTitle")]
    public async Task<IActionResult> GetBookByTitle(string title)
    { 
       var book = (await _booksRepository.Books.Find(x => x.Title == title, new string[] { "Author" }));
       return Ok(  mapper.Map<BookDTO>(book));

    }



    [HttpGet("GetAllBooksWithAuthorId")]
    public async Task<IActionResult> GetAllBooksWithAuthorId(int AuthorId)
    {
        var books = (await _booksRepository.Books.FindAll(x => x.AuthorId == AuthorId, new string[] { "Author" }));
       return Ok(  mapper.Map<IEnumerable<BookDTO>>(books));

    }




    [HttpGet("GetAllBooksOrderd")]
    public async Task<IActionResult> GetAllBooksOrderd(int AuthorId)
    {
        var books = (await _booksRepository.Books.FindAll(x => x.AuthorId == AuthorId,1,2,OrderByStrings.Ascending,x => x.Id, new string[] { "Author" }));
        return Ok(mapper.Map<IEnumerable< BookDTO>>(books));

    }





    [HttpPost("AddBook")]
    public async Task< IActionResult >AddBook(BookDTO book)
    {
        
        Book Newbook = mapper.Map<Book>(book);
        Newbook.Id = 0;

         await  _booksRepository.Books.Add(Newbook);
        _booksRepository.Complete();    
        return Ok(Newbook.Id);
    }




    [HttpPut("UpdateBook")]
    public async Task<IActionResult> UpdateBook(BookDTO book)
    {
        Book UpdatedBook = mapper.Map<Book>(book);
        await _booksRepository.Books.Update(UpdatedBook);
        _booksRepository.Complete();

        return Ok(UpdatedBook.Id);
    }




    [HttpDelete("DeleteBook")]
    public async Task<IActionResult> DeleteAuthor(int authorId)
    {
        await _booksRepository.Books.Delete(authorId);
        _booksRepository.Complete();

        return Ok(authorId);
    }


    [AllowAnonymous]
    [Authorize]
    [HttpGet("SpeicalForBooks")]
    public IActionResult SpeicalForBooks()
    {
        //  author.Id = 0;
         _booksRepository.Books.SpecialMethodForBooks();
        if (User.IsInRole("manager"))
        {
            return Ok();
        }
        return Forbid();
    }
}
