# Gym Management System

A comprehensive web-based gym management solution built with ASP.NET Core MVC and Entity Framework Core. This system streamlines gym operations including member management, class scheduling, trainer assignments, memberships, and health tracking.

## Features

### Core Functionality
- **Member Management** - Register, update, and manage gym members with health records
- **Trainer Management** - Add trainers, assign them to sessions, and track their availability
- **Session/Class Management** - Create and manage fitness classes with capacity limits and scheduling
- **Membership Plans** - Define flexible membership tiers and track member subscriptions
- **Booking System** - Members can book fitness sessions with availability tracking
- **Health Records** - Track member health metrics and progress
- **Analytics Dashboard** - Gain insights into gym operations and member engagement
- **Attachment Management** - Upload and manage member photos and documents

### User Management
- **ASP.NET Identity Integration** - Secure user authentication and authorization
- **Role-Based Access Control** - SuperAdmin, Admin, Trainer, and Member roles
- **Account Management** - Login, registration, and access denied handling

## Architecture

### Clean Architecture with 3-Tier Design
The project follows SOLID principles and clean architecture patterns:

```
├── GymSystemPL (Presentation Layer)
│   ├── Controllers - Request handling and response generation
│   ├── Views - Razor templates for user interface
│   └── Models - View models for data transfer
│
├── GymSystemBLL (Business Logic Layer)
│   ├── Services - Business logic implementation
│   ├── ViewModels - DTOs for data transfer
│   └── MappingProfiles - AutoMapper configurations
│
└── GymSystemDAL (Data Access Layer)
    ├── Entities - Domain models
    ├── Repositories - Data access patterns
    ├── Contexts - DbContext configuration
    └── Migrations - Database schema versioning
```

### Design Patterns Implemented
- **Repository Pattern** - Abstracts data access logic
- **Unit of Work Pattern** - Manages multiple repositories in transactions
- **Service Layer Pattern** - Encapsulates business logic
- **Dependency Injection** - Loose coupling and testability
- **AutoMapper Pattern** - Object-to-object mapping
- **Entity Framework Code-First** - Database-first migrations

## Technologies & Dependencies

- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core 9.0
- **Identity**: ASP.NET Core Identity
- **Mapping**: AutoMapper
- **Frontend**: Bootstrap, jQuery
- **Language**: C# with nullable reference types enabled

## Project Structure

### Presentation Layer (GymSystemPL)
Controllers for:
- `AccountController` - User authentication and account management
- `MemberController` - Member CRUD operations
- `TrainerController` - Trainer management
- `SessionController` - Class/session management
- `BookingController` - Session bookings
- `MembershipController` - Membership plan management
- `PlanController` - Plan configuration
- `HomeController` - Home page and navigation

### Business Logic Layer (GymSystemBLL)
Services for:
- `IMemberService` / `MemberService`
- `ITrainerService` / `TrainerService`
- `ISessionService` / `SessionService`
- `IMembershipService` / `MembershipService`
- `IBookingService` / `BookingService`
- `IAccountService` / `AccountService`
- `IAnalyticsService` / `AnalyticsService`
- `IAttachmentService` / `AttachmentService`

### Data Access Layer (GymSystemDAL)
**Core Entities:**
- `ApplicationUser` - Identity user
- `GymUser` - Base gym user class
- `Member` - Gym member with health records
- `Trainer` - Fitness trainer
- `Session` - Fitness class/session
- `Membership` - Member subscription
- `MemberSession` - Member session enrollment (booking)
- `Plan` - Membership plan
- `Category` - Session categories
- `HealthRecord` - Member health metrics

**Data Access:**
- `IGenericRepository<T>` / `GenericRepository<T>` - Generic CRUD operations
- Specialized repositories: `IPlanRepository`, `ISessionRepository`, `IMembershipRepository`, `IBookingRepository`
- `IUnitOfWork` / `UnitOfWork` - Transaction management

## Installation & Setup

### Prerequisites
- .NET 9.0 SDK
- SQL Server (local or remote)
- Visual Studio 2022 or VS Code

### Steps

1. **Clone or open the project**
   ```bash
   cd GymSystemSolution
   ```

2. **Update Database Connection**
   - Open `GymSystemPL/appsettings.json`
   - Update the `ConnectionStrings:DefaultConnection` with your SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=GymSystemDB;Trusted_Connection=true;"
     }
   }
   ```

3. **Apply Migrations**
   ```bash
   dotnet ef database update --project GymSystemDAL --startup-project GymSystemPL
   ```
   Or use Package Manager Console in Visual Studio:
   ```
   Update-Database
   ```

4. **Run the Application**
   ```bash
   dotnet run --project GymSystemPL
   ```

5. **Access the Application**
   - Navigate to `https://localhost:5001` (or the configured port)
   - Default seeded admin credentials will be available (check data seeding section)

## Database Seeding

The application automatically seeds initial data on startup:
- Creates default roles (SuperAdmin, Admin, Trainer, Member)
- Seeds initial admin users
- Initializes sample gym categories and plans

Seeding occurs in:
- `GymDbContextSeeding.SeedData()` - Entity data
- `IdentityDbContextSeeding.SeedData()` - Identity roles and users

## Authentication & Authorization

The system uses ASP.NET Core Identity with the following roles:

| Role | Permissions |
|------|-------------|
| SuperAdmin | Full system access |
| Admin | Manage members, trainers, and sessions |
| Trainer | View assigned sessions and members |
| Member | Book sessions, view health records |

Protected routes redirect unauthorized users to:
- Login: `/Account/Login`
- Access Denied: `/Account/AccessDenied`

## Password Policy

- **Minimum Length**: 6 characters
- **Uppercase**: Required
- **Lowercase**: Required
- **Unique Email**: Required

## File Upload

The `IAttachmentService` handles file uploads for member photos and documents, storing them in the `/wwwroot/Files/` directory.

## Development

### Adding a New Feature

1. **Create Entity** in `GymSystemDAL/Entities`
2. **Add DbSet** to `GymSystemDbContext`
3. **Create Migration**: 
   ```bash
   dotnet ef migrations add [MigrationName] --project GymSystemDAL
   ```
4. **Create Repository** (if needed) in `GymSystemDAL/Repositories`
5. **Create Service** in `GymSystemBLL/Services`
6. **Register in DI** in `Program.cs`
7. **Create Controller** in `GymSystemPL/Controllers`
8. **Add Views** in `GymSystemPL/Views`

### Configuration

Key configurations in `Program.cs`:
- **DbContext** setup with SQL Server
- **AutoMapper** profiles registration
- **Dependency Injection** service registration
- **Identity** password policies and cookie settings

## Code Quality

The project follows SOLID principles:
- **S**ingle Responsibility - Each class has one reason to change
- **O**pen/Closed - Open for extension, closed for modification
- **L**iskov Substitution - Base entity pattern for inheritance
- **I**nterface Segregation - Multiple focused interfaces
- **D**ependency Inversion - Depends on abstractions, not concrete implementations

## Project Files

- `GymSystemSolution.sln` - Solution file
- `GymSystemPL/Program.cs` - Application startup and DI configuration
- `appsettings.json` - Application settings and connection strings
- `launchSettings.json` - Launch profiles for development

## Future Enhancements

- Payment integration for memberships
- Email notifications for bookings
- SMS reminders for sessions
- Advanced reporting and analytics
- Mobile app integration
- Real-time session availability updates

## Support & Troubleshooting

### Common Issues

**Migration Error:**
```bash
# Ensure EF tools are installed
dotnet tool install --global dotnet-ef

# Update database from PL project directory
dotnet ef database update --project GymSystemDAL --startup-project GymSystemPL
```

**Connection String Error:**
- Verify SQL Server is running
- Check `appsettings.json` connection string
- Ensure database user has appropriate permissions

**Login Issues:**
- Check that migrations have been applied
- Verify seeding has completed
- Review identity configuration in `Program.cs`

## License

This project is created for educational and commercial use.

## Author

Created as a comprehensive MVC project demonstrating clean architecture and SOLID principles in ASP.NET Core.

---

**Version**: 1.0  
**Framework**: .NET 9.0  
**Last Updated**: January 2026
