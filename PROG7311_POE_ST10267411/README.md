# Agri-Energy Connect Platform

A lightweight web application that connects small-scale farmers with energy companies looking for sustainable biomass inputs.

## Features

- **Role-based authentication** with separate Farmer and Employee roles
- **Farmer functionality**: Add and manage products
- **Employee functionality**: Browse farmers, view their products, filter products by date/category
- **Database**: SQLite database with auto-migration and seeding
- **Responsive UI**: Mobile-friendly Bootstrap interface

## Requirements

- .NET SDK 8.0

## Setup Instructions

1. Install .NET SDK 8.0 if not already installed (https://dotnet.microsoft.com/download/dotnet/8.0)
2. Navigate to the project directory in a terminal 
3. Run the command: `dotnet run`
4. Access the application at https://localhost:5001 or http://localhost:5000

## Demo Accounts

- **Employee**: email=employee@demo.local, password=Password123!
- **Farmer 1**: email=farmer1@demo.local, password=Password123!  
- **Farmer 2**: email=farmer2@demo.local, password=Password123!

## Technical Details

- ASP.NET Core 8.0 MVC
- Entity Framework Core 8.0 with SQLite
- ASP.NET Core Identity for authentication
- Bootstrap 5 for responsive UI 