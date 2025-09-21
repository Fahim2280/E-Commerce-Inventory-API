# E-Commerce Inventory API

A comprehensive RESTful API for e-commerce inventory management built with ASP.NET Core 8, featuring JWT authentication, image upload capabilities, and a Domain-Driven Design (DDD) layered, Unity of work, Repository pattern.

## 🚀 Features

- **User Authentication**: JWT-based authentication system
- **Product Management**: Full CRUD operations for products with image support
- **Category Management**: Complete category management system
- **Image Upload**: Support for both file system and Base64 image storage
- **Search Functionality**: Product search capabilities
- **Clean Architecture**: Domain-driven design with separation of concerns
- **Swagger Documentation**: Interactive API documentation
- **Entity Framework Core**: Code-first approach with migrations

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Documentation**: Swagger/OpenAPI
- **Image Processing**: Custom image service
- **Password Hashing**: BCrypt.Net
- **Mapping**: AutoMapper

## 📋 Prerequisites

Before running this project, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, or full version)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## 🔧 Local Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/Fahim2280/E-Commerce-Inventory-API.git
cd "E-Commerce Inventory API"
```

### 2. Database Configuration

#### Connection String Setup

1. **Open `appsettings.json`** in the `ECommerceAPI.API` project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ECommerceInventoryDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "your-super-secret-key-that-is-at-least-32-characters-long",
    "Issuer": "ECommerceAPI",
    "Audience": "ECommerceAPI",
    "ExpireMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

2. **Update Connection String** for your environment:

   **For LocalDB (Recommended for development):**
   ```json
   "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ECommerceInventoryDB;Trusted_Connection=true;TrustServerCertificate=true;"
   ```

   **For SQL Server Express:**
   ```json
   "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ECommerceInventoryDB;Trusted_Connection=true;TrustServerCertificate=true;"
   ```

   **For Full SQL Server:**
   ```json
   "DefaultConnection": "Server=localhost;Database=ECommerceInventoryDB;Trusted_Connection=true;TrustServerCertificate=true;"
   ```

   **For SQL Server with username/password:**
   ```json
   "DefaultConnection": "Server=localhost;Database=ECommerceInventoryDB;User Id=your_username;Password=your_password;TrustServerCertificate=true;"
   ```

#### JWT Configuration

Update the JWT settings in `appsettings.json`:

```json
"Jwt": {
  "Key": "your-super-secret-key-that-is-at-least-32-characters-long",
  "Issuer": "ECommerceAPI",
  "Audience": "ECommerceAPI",
  "ExpireMinutes": 60
}
```

**Important**: Replace the JWT Key with a secure, randomly generated key in production.

### 3. Database Migration and Setup

#### Option A: Using Entity Framework Migrations (Recommended)

1. **Install EF Core Tools** (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. **Navigate to solution directory**:
   ```bash
   cd "E-Commerce Inventory API"
   ```

3. **Apply existing migrations**:
   ```bash
   dotnet ef database update --project ECommerceAPI.Infrastructure --startup-project ECommerceAPI.API
   ```

#### Option B: Create Database Automatically

The application will automatically create the database on first run using `EnsureCreatedAsync()`.

**Note**: If you use this option and later want to add migrations, you may need to manually synchronize the migration history.

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the Application

**From the solution directory**, use the following command:

```bash
dotnet run --project ECommerceAPI.API
```

The API will start and be available at:
- **HTTP**: `http://localhost:5253`
- **HTTPS**: `https://localhost:7253`
- **Swagger UI**: `http://localhost:5253/swagger`

## 📖 API Usage

### Authentication Flow

1. **Register a new user**:
   ```
   POST /api/auth/register
   ```

2. **Login to get JWT token**:
   ```
   POST /api/auth/login
   ```

3. **Use the token** in subsequent requests:
   ```
   Authorization: Bearer <your-jwt-token>
   ```

### Available Endpoints

#### Authentication (Public)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login

#### Categories (Protected)
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

#### Products (Protected)
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/category/{categoryId}` - Get products by category
- `GET /api/products/search?q={keyword}` - Search products
- `POST /api/products` - Create product (JSON)
- `POST /api/products/with-file` - Create product with image upload
- `PUT /api/products/{id}` - Update product (JSON)
- `PUT /api/products/{id}/with-file` - Update product with image upload
- `POST /api/products/{id}/upload-image` - Upload product image (Base64)
- `POST /api/products/{id}/upload-image-advanced` - Upload with storage choice
- `DELETE /api/products/{id}` - Delete product

### Image Upload Options

The API supports two image storage methods:

1. **Base64 Storage** (in database):
   - Suitable for small images
   - Included in database backups
   - Use regular endpoints with `ImageBase64` field

2. **File System Storage**:
   - Better performance for larger images
   - Images stored in `wwwroot/images/products/`
   - Use `/upload-image-advanced` endpoint with `useFileSystem=true`

## 🧪 Testing with Swagger

1. Navigate to `http://localhost:5253/swagger`
2. Click **"Authorize"** button
3. Register/Login to get a JWT token
4. Enter: `Bearer <your-jwt-token>`
5. Test protected endpoints

## 🗂️ Project Structure

```
E-Commerce Inventory API/
├── ECommerceAPI.API/              # Presentation Layer
│   ├── Controllers/               # API Controllers
│   ├── wwwroot/images/           # Static image files
│   └── Program.cs                # Application entry point
├── ECommerceAPI.Application/      # Application Layer
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Services/                 # Business Logic Services
│   └── Mappings/                 # AutoMapper Profiles
├── ECommerceAPI.Domain/           # Domain Layer
│   ├── Entities/                 # Domain Models
│   └── Interfaces/               # Repository Interfaces
└── ECommerceAPI.Infrastructure/   # Infrastructure Layer
    ├── Data/                     # DbContext
    ├── Repositories/             # Repository Implementations
    ├── Migrations/               # EF Core Migrations
    └── UnitOfWork/              # Unit of Work Pattern
```

## 🔧 Development Setup

### Adding New Migrations

When you modify entity models:

```bash
dotnet ef migrations add YourMigrationName --project ECommerceAPI.Infrastructure --startup-project ECommerceAPI.API
dotnet ef database update --project ECommerceAPI.Infrastructure --startup-project ECommerceAPI.API
```

### Environment Variables

For production, consider using environment variables:

```bash
export ConnectionStrings__DefaultConnection="Your-Production-Connection-String"
export Jwt__Key="Your-Production-JWT-Key"
```

## 🚨 Important Notes

- **JWT Secret**: Always use a secure, random key for JWT signing in production
- **Connection String**: Update connection string for your database server
- **HTTPS**: Enable HTTPS in production environments
- **CORS**: Configure CORS settings for your client applications
- **File Permissions**: Ensure the application has write permissions to `wwwroot/images/`

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Errors**:
   - Verify SQL Server is running
   - Check connection string format
   - Ensure database permissions

2. **Migration Conflicts**:
   - If using `EnsureCreated()`, manually sync migration history
   - Delete database and recreate if in development

3. **JWT Token Issues**:
   - Verify JWT key length (minimum 32 characters)
   - Check token expiration time
   - Ensure proper Authorization header format

4. **File Upload Issues**:
   - Check `wwwroot/images/` directory exists
   - Verify file permissions
   - Confirm maximum file size settings

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📞 Support

For support and questions:
- Create an issue in the GitHub repository
- Check the [API documentation](http://localhost:5253/swagger) when running locally