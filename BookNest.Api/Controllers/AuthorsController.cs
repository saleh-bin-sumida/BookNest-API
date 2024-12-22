

namespace RepositoryWithUOW.Api.Controllers
{
    [ApiController]
    [Route("api/v1/authors")]
    [Authorize(Roles = "User")]

    public class AuthorsController(IUnitOfWork _unitOfWork, IMapper _mapper) : ControllerBase
    {

        /// <summary>
        /// الحصول على جميع المؤلفين.
        /// </summary>
        /// <returns>قائمة المؤلفين.</returns>
        [HttpGet]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetAsync()
        {
            var authors = await _unitOfWork.Authors.GetAllAsync();
            return Ok(authors);
        }

        /// <summary>
        /// الحصول على مؤلف بواسطة المعرف.
        /// </summary>
        /// <param name="id">معرف المؤلف.</param>
        /// <returns>المؤلف المطلوب.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _unitOfWork.Authors.FindAsync(x => x.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        /// <summary>
        /// الحصول على مؤلف بواسطة الاسم.
        /// </summary>
        /// <param name="name">اسم المؤلف.</param>
        /// <returns>المؤلف المطلوب.</returns>
        [HttpGet("ByName")]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetAuthorByName(string name)
        {
            var author = await _unitOfWork.Authors.FindAsync(x => x.Name == name);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        /// <summary>
        /// إضافة مؤلف جديد.
        /// </summary>
        /// <param name="authorDto">بيانات المؤلف.</param>
        /// <returns>معرف المؤلف المضاف.</returns>
        [HttpPost]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> AddAuthor(AuthorDTO authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            await _unitOfWork.Authors.AddAsync(author);
            await _unitOfWork.SaveAsync();
            return Ok(author.Id);
        }

        /// <summary>
        /// تحديث بيانات المؤلف.
        /// </summary>
        /// <param name="authorDto">بيانات المؤلف المحدثة.</param>
        /// <returns>معرف المؤلف المحدث.</returns>
        [HttpPut()]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> UpdateAuthor(AuthorDTO authorDto)
        {
            var updatedAuthor = _mapper.Map<Author>(authorDto);
            await _unitOfWork.Authors.UpdateAsync(updatedAuthor);
            await _unitOfWork.SaveAsync();
            return Ok(updatedAuthor.Id);
        }

        /// <summary>
        /// حذف مؤلف بواسطة المعرف.
        /// </summary>
        /// <param name="authorId">معرف المؤلف.</param>
        /// <returns>معرف المؤلف المحذوف.</returns>
        [HttpDelete]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> DeleteAuthor(int authorId)
        {
            await _unitOfWork.Authors.DeleteAsync(authorId);
            await _unitOfWork.SaveAsync();
            return Ok(authorId);
        }
    }
}
