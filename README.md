# Gym Member Management System

A complete ASP.NET Core 8 MVC web application for managing gym memberships with role-based access control.

## Features

### Member Features
- Self-registration and login
- Choose membership plans
- View membership status and days remaining
- Renew membership with different plans
- Cancel membership (with fee calculation)
- View payment history

### Admin Features
- Dashboard with statistics and analytics
- Manage all members (view, freeze, unfreeze)
- Manage membership plans (create, edit, activate/deactivate)
- View member details and payment history
- Waive cancellation fees
- Search members by name, email, or phone

## Technology Stack

- **Framework**: ASP.NET Core 8 MVC
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL
- **Authentication**: Session-based with role-based authorization
- **Password Hashing**: BCrypt
- **Styling**: Bootstrap 5

## Prerequisites

- .NET 8 SDK
- PostgreSQL 12 or higher
- Visual Studio 2022 or VS Code

## Installation Steps

### 1. Install NuGet Packages

```bash
dotnet restore
```

Or install individually:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package BCrypt.Net-Next
```

### 2. Configure Database Connection

Update `appsettings.json` with your PostgreSQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=gymdb;Username=postgres;Password=yourpassword"
  }
}
```

### 3. Run Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`



## Default Seeded Plans

The database is seeded with 4 default plans:
1. Monthly - 1 month - ₹800
2. Quarterly - 3 months - ₹2,100
3. Half Yearly - 6 months - ₹3,800
4. Yearly - 12 months - ₹6,500

## Project Structure

```
GymManagement/
├── Controllers/          # MVC Controllers
├── Models/              # Domain models and ViewModels
├── Data/                # DbContext and database configuration
├── Filters/             # Authorization filters
├── Views/               # Razor views
├── wwwroot/             # Static files (CSS, JS)
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration
```

## Business Rules

### Membership Cancellation Fees
- **Within 7 days**: 50% of plan price
- **8-30 days**: 25% of plan price
- **After 30 days**: No fee

### Account Freezing
- Admin can freeze member accounts
- Frozen members cannot login
- Frozen members see a warning message

### Plan Management
- Plans are soft-deleted (IsActive = false)
- Inactive plans are hidden from member selection
- Admin can reactivate deactivated plans

## Testing the Application

### As a Member:
1. Register a new account
2. Choose a membership plan
3. Complete payment (simulated - any card details work)
4. View membership dashboard
5. Try renewing or cancelling membership

### As an Admin:
1. Login with admin credentials
2. View dashboard statistics
3. Manage members (freeze/unfreeze)
4. Create/edit membership plans
5. View member details and payment history
6. Waive cancellation fees

## API Endpoints

### Account
- GET/POST `/Account/Login`
- GET/POST `/Account/Register`
- GET `/Account/Logout`

### Dashboard (Admin Only)
- GET `/Dashboard`

### Plans (Admin Only)
- GET `/Plans`
- GET/POST `/Plans/Create`
- GET/POST `/Plans/Edit/{id}`
- POST `/Plans/Delete/{id}`
- POST `/Plans/Toggle/{id}`

### Members (Admin Only)
- GET `/Members?search={query}`
- GET `/Members/Details/{id}`
- POST `/Members/Freeze/{id}`
- POST `/Members/Unfreeze/{id}`
- POST `/Members/WaiveCancellationFee/{membershipId}`

### Member Portal (Member Only)
- GET `/MemberPortal`
- GET `/MemberPortal/ChoosePlan`
- GET `/MemberPortal/Renew`
- GET `/MemberPortal/Cancel`
- POST `/MemberPortal/ConfirmCancel`

### Payment (Member Only)
- GET `/Payment/Checkout?planId={id}&paymentType={type}&membershipId={id}`
- POST `/Payment/Checkout`
- GET `/Payment/Success`

## Security Features

- BCrypt password hashing
- Session-based authentication
- Role-based authorization filters
- Unique email constraint
- Frozen account checks

## Notes

- Payment processing is simulated - all payments succeed regardless of card details
- All dates use UTC timezone
- Session timeout is set to 30 minutes
- Plans are soft-deleted to preserve foreign key relationships

## Troubleshooting

### Migration Issues
If you encounter migration errors, try:
```bash
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Connection Issues
Ensure PostgreSQL is running and credentials in `appsettings.json` are correct.

### Port Conflicts
If ports 5000/5001 are in use, modify `launchSettings.json` or use:
```bash
dotnet run --urls "http://localhost:5002"
```

## License

This project is for educational purposes.
