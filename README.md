# PROG7311 - POE - Part 2

#### ST10267411 - Abdul Baari Davids
#### May 2025

> NOTE: You can try out the Published application here:
>
 https://st10267411.azurewebsites.net/
 

## Agri-Energy Connect Platform

A platform that connects small-scale farmers with energy companies looking for sustainable biomass. The platform provides a streamlined way for farmers to showcase their agricultural products and for energy sector employees to find them.

<p align="center">
<img src="https://github.com/user-attachments/assets/d91e8b06-da8f-4454-9ab2-5899e070d1c4" width="700" />
</p>

## Project Background

Small-scale farmers across South Africa struggle to showcase their produce to large energy companies looking for sustainable biomass inputs. The Agri-Energy Connect Platform bridges this gap with a lightweight web portal where:

- **Farmers** list and manage their products
- **Energy-sector employees** discover and filter farmer offerings

This prototype demonstrates:

1. **Viability** - A single .NET 8 MVC app handles user management, data capture, and filtering without bloat
2. **Usability** - The UI works for field-based farmers on mobile data and for office employees on desktop
3. **Data integrity** - Product records are validated, persisted, and role-isolated yet searchable to authorized staff
4. **Extensibility** - A clear, test-covered codebase future teams can extend with payments, logistics, analytics, etc.

## Setup Instructions

### Prerequisites

- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/vs/)

### Option 1: Setup with Visual Studio 2022

1. Clone or download the repository
2. Open the solution file `PROG7311_POE_ST10267411.sln` in Visual Studio 
3. Ensure the `PROG7311_POE_ST10267411` project is set as the startup project
4. Press F5 or click the "Start" button to build and run the application
5. The application will launch in your default browser and the database will be automatically created and seeded

### Option 2: Setup with Command Line

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/PROG7311_POE_ST10267411.git
   ```

2. Navigate to the project directory:
   ```
   cd PROG7311_POE_ST10267411
   ```

3. Restore dependencies:
   ```
   dotnet restore
   ```

4. Run the application:
   ```
   dotnet run --project PROG7311_POE_ST10267411
   ```

5. Access the application in your browser (check terminal output for exact URL)

### Database Information

The application uses SQLite as the database, as such the database itself exists in the `App_Data` folder:

- Database file is automatically created in the `App_Data` folder when the application first runs
- Sample data is seeded at startup if no data exists
- No additional database setup is required
- The connection string can be modified in `appsettings.json` to use SQL Server if needed

## Key Features

### Authentication & Authorization

- **Two distinct roles**: `Farmer` and `Employee` (energy sector)
- **Secure login system**: ASP.NET Core Identity with role-based guards
- **Protected routes**: All data views require authentication

### Farmer Features

- **Dashboard** with product statistics and management tools
- **Add products** in a few simple steps through an intuitive form
- **Edit existing products** with full validation
- **View personal product listings** with filtering options

### Employee Features

- **Browse all farmers** registered in the system
- **Create new farmer profiles** with optional account creation
- **View farmer-specific products** by selecting a farmer
- **Advanced filtering** by date range and product category
- **Comprehensive product search** across all farmers

### Data Management & Validation

- **Data validation** on all inputs with meaningful error messages
- **Global error handling** for consistent user experience
- **Role isolation** ensuring farmers only see and manage their own products
- **Entity Framework Core** with proper relationships and constraints

## Demo Accounts

Use these credentials to explore the platform:

**Farmer Accounts:**
- Email: sundar@gmail.com
- Password: Password123!

**Energy Company Account:**
- Email: elon@gmail.com
- Password: Password123!

## Testing 

The tests are located in the `PROG7311_POE_ST10267411.Tests` project. To run the tests, navigate to the project directory and run the following command:

```
dotnet test
```

Or open the solution in Visual Studio and run the tests from there.

## Technology Stack

- **Backend**: ASP.NET Core 8 MVC
- **Database**: Entity Framework Core 8 with SQLite (swappable to SQL Server)
- **Authentication**: ASP.NET Core Identity with role-based authorization
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript
- **Validation**: Data Annotations and server-side validation
- **Testing**: xUnit for controller unit tests
- **Github Actions**: Because its cool!

## License

This project is created for educational purposes as part of the PROG7311 module.

