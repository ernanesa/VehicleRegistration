# Vehicle Registration API

The Vehicle Registration API is a RESTful API that allows users to manage vehicle registrations. It provides functionality for administrators to register new vehicles, view vehicle details, update existing vehicles, and delete vehicles.

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
  - [Authentication](#authentication)
  - [Endpoints](#endpoints)
    - [Administrators](#administrators)
    - [Vehicles](#vehicles)
- [Contributing](#contributing)
- [License](#license)

## Installation

To run the Vehicle Registration API, you'll need to have the following prerequisites installed:

- .NET 6.0 SDK
- A SQL Server or SQLite database

1. Clone the repository:

   ```
   git clone https://github.com/your-username/vehicle-registration-api.git
   ```

2. Navigate to the project directory:

   ```
   cd vehicle-registration-api/src/VehicleRegistration.API
   ```

3. Update the database connection string in the `appsettings.json` file.

4. Build and run the application:

   ```
   dotnet build
   dotnet run
   ```

The API will be available at `https://localhost:5001/swagger`.

## Usage

### Authentication

The Vehicle Registration API uses JWT (JSON Web Token) authentication. To access the protected endpoints, you'll need to obtain a valid JWT token by logging in as an administrator.

### Endpoints

#### Administrators

- **POST /api/v1/administrators/login**: Authenticate an administrator and obtain a JWT token.
- **POST /api/v1/administrators/register**: Register a new administrator.

#### Vehicles

- **GET /api/v1/vehicles**: Retrieve a paginated list of vehicles.
- **GET /api/v1/vehicles/{id}**: Get details of a specific vehicle.
- **POST /api/v1/vehicles**: Create a new vehicle.
- **PUT /api/v1/vehicles/{id}**: Update an existing vehicle.
- **DELETE /api/v1/vehicles/{id}**: Delete a vehicle.

## Contributing

Contributions to the Vehicle Registration API are welcome! To contribute, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and ensure that all tests pass.
4. Submit a pull request with a detailed description of your changes.

Please also review the [CONTRIBUTING.md](CONTRIBUTING.md) file for more information.

## License

The Vehicle Registration API is licensed under the [MIT License](LICENSE).
