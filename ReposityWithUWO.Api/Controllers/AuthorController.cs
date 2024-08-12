using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryWithUOW.Core.DTOs;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Models;
using RepositoryWithUWO.EF.Repositories;


namespace RepositoryWithUOW.Api.Controllers;

[ApiController]
[Route("api/author")]
public class AuthorController(IUnitOfWork authorsRepository, IMapper mapper) : ControllerBase
{
    private IUnitOfWork _authorsRepository = authorsRepository;
    private readonly IMapper mapper = mapper;


    [HttpGet()]
    public async Task<IActionResult> GetAuthorsAsync()
    {
        var authors = await _authorsRepository.Authors.IndexAsync();
        return Ok(mapper.Map<IEnumerable<AuthorDTO>>(authors));
    }



    [HttpGet]
    [Route("{Id}")]
    [Authorize]

    public async Task<IActionResult> GetAuthorById(int Id)
    {
        var autor =  (await _authorsRepository.Authors.Find(x => x.Id == Id));
        return Ok(mapper.Map<AuthorDTO>(autor));
    }




     [HttpGet("GetAuthorByName")]
    public async Task<IActionResult> GetAuthorByName(string name)
    {
        var autor = (await _authorsRepository.Authors.Find(x => x.Name == name));
        return Ok(mapper.Map<AuthorDTO>(autor));
    }





    [HttpPost("AddAuthor")]
    public async Task< IActionResult> AddAuthor(AuthorDTO author)
    {
        var NewAuthor = mapper.Map<Author>(author);
        NewAuthor.Id = 0;

        await _authorsRepository.Authors.Add(NewAuthor);
        _authorsRepository.Complete();

        return Ok(NewAuthor.Id);
    }




    [HttpPut("UpdateAuthor")]
    public async Task<IActionResult> UpdateAuthor(AuthorDTO author)
    {
        //  author.Id = 0;
        var UpdatedAuthor = mapper.Map<Author>(author);

        await _authorsRepository.Authors.Update( UpdatedAuthor);
        _authorsRepository.Complete();

        return Ok(UpdatedAuthor.Id);
    }





    [HttpDelete("DeleteAuthor")]
    public async Task<IActionResult> DeleteAuthor(int authorId)
    {
        //  author.Id = 0;
        await _authorsRepository.Authors.Delete(authorId );
        _authorsRepository.Complete();

        return Ok(authorId);
    }

}



