

namespace RepositoryWithUOW.Api.Controllers
{
    [ApiController]
    [Route("api/v1/books")]
    [ApiVersion("1.0")]

    public class BookController(IUnitOfWork _unitOfWork, IMapper _mapper) : ControllerBase
    {

        /// <summary>
        /// الحصول على جميع الكتب.
        /// </summary>
        /// <returns>قائمة الكتب.</returns>
        [HttpGet]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetBooksAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }

        /// <summary>
        /// الحصول على كتاب بواسطة المعرف.
        /// </summary>
        /// <param name="id">معرف الكتاب.</param>
        /// <returns>الكتاب المطلوب.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _unitOfWork.Books.FindAsync(x => x.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BookDTO>(book));
        }

        /// <summary>
        /// الحصول على كتاب بواسطة العنوان.
        /// </summary>
        /// <param name="title">عنوان الكتاب.</param>
        /// <returns>الكتاب المطلوب.</returns>
        [HttpGet("ByTitle")]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetBookByTitle(string title)
        {
            var book = await _unitOfWork.Books.FindAsync(x => x.Title == title, ["Author"]);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BookDTO>(book));
        }

        /// <summary>
        /// الحصول على جميع الكتب بواسطة معرف المؤلف.
        /// </summary>
        /// <param name="authorId">معرف المؤلف.</param>
        /// <returns>قائمة الكتب.</returns>
        [HttpGet("ByAuthor")]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetBooksByAuthorId(int authorId)
        {
            var books = await _unitOfWork.Books.FindAll(x => x.AuthorId == authorId, ["Author"]);
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }

        /// <summary>
        /// الحصول على جميع الكتب مرتبة بواسطة معرف المؤلف.
        /// </summary>
        /// <param name="parameters">معلمات الاستعلام عن الكتاب.</param>
        /// <returns>قائمة الكتب مرتبة.</returns>
        [HttpGet("Pagenated")]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetAllBooksOrderedBy([FromQuery] BookQueryParameters parameters)
        {
            var orderByDirection = parameters.OrderByDirection == "ASC" ? OrderByStrings.Ascending : OrderByStrings.Desending;
            var books = await _unitOfWork.Books.FindAll(
                x => x.AuthorId == parameters.AuthorId,
                parameters.Skip,
                parameters.Take,
                orderByDirection,
                x => EF.Property<object>(x, parameters.PropertyName), ["Author"]
            );
            return Ok(_mapper.Map<IEnumerable<BookDTO>>(books));
        }

        /// <summary>
        /// إضافة كتاب جديد.
        /// </summary>
        /// <param name="bookDto">بيانات الكتاب.</param>
        /// <returns>معرف الكتاب المضاف.</returns>
        [HttpPost]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> AddBook(BookDTO bookDto)
        {
            if (!await _unitOfWork.Authors.AnyAsync(x => x.Id == bookDto.AuthorId))
                return BadRequest("معرف المؤلف غير صالح");

            var newBook = _mapper.Map<Book>(bookDto);
            await _unitOfWork.Books.AddAsync(newBook);
            await _unitOfWork.SaveAsync();
            return Ok(newBook.Id);
        }

        /// <summary>
        /// تحديث بيانات الكتاب.
        /// </summary>
        /// <param name="bookDto">بيانات الكتاب المحدثة.</param>
        /// <returns>معرف الكتاب المحدث.</returns>
        [HttpPut]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> UpdateBook(BookDTO bookDto)
        {
            if (!await _unitOfWork.Authors.AnyAsync(x => x.Id == bookDto.AuthorId))
                return BadRequest("معرف المؤلف غير صالح");

            var updatedBook = _mapper.Map<Book>(bookDto);
            await _unitOfWork.Books.UpdateAsync(updatedBook);
            await _unitOfWork.SaveAsync();
            return Ok(updatedBook.Id);
        }

        /// <summary>
        /// حذف كتاب بواسطة المعرف.
        /// </summary>
        /// <param name="bookId">معرف الكتاب.</param>
        /// <returns>معرف الكتاب المحذوف.</returns>
        [HttpDelete]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> DeleteBook(int bookId)
        {
            await _unitOfWork.Books.DeleteAsync(bookId);
            await _unitOfWork.SaveAsync();
            return Ok(bookId);
        }

    }
}

