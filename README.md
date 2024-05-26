# Employee Census

This is an ASP.NET Core project for managing employee records. Follow the instructions below to set up the project locally.

## Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Git](https://git-scm.com/downloads)

## Setup Instructions

1. **Clone the repository**:
    ```sh
    git clone https://github.com/MasterSanya/EmployeeCensus.git
    cd EmployeeCensus
    ```

2. **Configure the database**:
    - Open `appsettings.json` and update the connection string to point to your local SQL Server instance.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeCensusDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3. **Create the database**:
    - Open a terminal and navigate to the project directory.
    - Run the following commands to apply migrations and create the database:
    ```sh
    dotnet ef database update
    ```

4. **Run the project**:
    - In the terminal, run:
    ```sh
    dotnet run
    ```

5. **Open the application**:
    - Open a web browser and navigate to `https://localhost:5001` to see the application in action.

## Authentication

The application uses basic authentication for secured routes. You can register a new user and then log in to access secured features.

## Troubleshooting

If you encounter any issues, please check the following:
- Ensure that SQL Server is running and accessible.
- Verify that the connection string in `appsettings.json` is correct.
- Ensure that you have applied the latest migrations to the database.

## Contributing

Feel free to fork the repository and submit pull requests. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License.
