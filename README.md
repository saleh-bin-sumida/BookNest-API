# BookNest API

BookNest API is a robust and scalable API designed to manage books and authors. It follows a 3-tier architecture and implements various design patterns and technologies to ensure maintainability, efficiency, and security.

## Features

- **3-Tier Architecture:** Clean separation of concerns for better maintainability.
- **Repository and Unit of Work Patterns:** Encapsulates data access logic and manages transactions.
- **EF Core:** Efficient data operations with a powerful ORM.
- **AutoMapper:** Streamlines object mapping between different layers.
- **Microsoft Identity:** Handles authentication and authorization with refresh token support.
- **CRUD Operations:** Supports creation, reading, updating, and deleting authors and books.
- **Scalability and Extensibility:** Designed for future growth and feature additions.

## Technologies Used

- **.NET 8**
- **C# 12.0**
- **Entity Framework Core 8.0.7**
- **AutoMapper**
- **Microsoft Identity**
- **Swashbuckle (Swagger) 6.4.0**

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation

1. Clone the repository:
    git clone https://github.com/your-repo/BookNest.git
cd BookNest


2. Set up the database:
    - Update the connection string in `appsettings.json` to point to your SQL Server instance.
    - Run the following commands to apply migrations and create the database:
    - `dotnet ef database update --project BookNest.EF`
    
    
3. Build and run the solution:

 `dotnet build ` 

 `dotnet run --project BookNest.Api` 



    
### Usage

- The API will be available at `https://localhost:5001` or `http://localhost:5000`.
- Use Swagger UI to explore and test the API endpoints: `https://localhost:5001/swagger`.

## API Endpoints

### Authors

- `GET /api/author`: Get all authors.
- `GET /api/author/{id}`: Get an author by ID.
- `GET /api/author/GetAuthorByName?name={name}`: Get an author by name.
- `POST /api/author/AddAuthor`: Add a new author.
- `PUT /api/author/UpdateAuthor`: Update an existing author.
- `DELETE /api/author/DeleteAuthor?authorId={authorId}`: Delete an author by ID.


### Books

- `GET /api/book`: Get all books.
- `GET /api/book/{id}`: Get a book by ID.
- `GET /api/book/GetBookByTitle?title={title}`: Get a book by title.
- `GET /api/book/GetAllBooksWithAuthorId?authorId={authorId}`: Get all books by author ID.
- `GET /api/book/GetAllBooksOrdered?authorId={authorId}`: Get all books by author ID, ordered by ID.
- `POST /api/book/AddBook`: Add a new book.
- `PUT /api/book/UpdateBook`: Update an existing book.
- `DELETE /api/book/DeleteBook?bookId={bookId}`: Delete a book by ID.

### Authentication

- `POST /api/auth/Register`: Register a new user.
- `POST /api/auth/login`: Login a user.
- `POST /api/auth/AddUserToRole`: Add a user to a role.
- `POST /api/auth/CreateRole`: Create a new role.
- `GET /api/auth/RefreshToken`: Refresh the authentication token.
- `POST /api/auth/RevokeToken`: Revoke a refresh token.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any changes.

## License

This project is licensed under the MIT License.
