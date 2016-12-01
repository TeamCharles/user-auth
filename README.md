# user-auth
Refactor of TeamCharles/initial-site with user authentication using Identity Framework

# Installation

Touch an `appsettings.json` file in your root directory and add the following code. Update the DefaultConnection property to a reference to your database.
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:bangazon-db.database.windows.net,1433;Initial Catalog=team-charles-bangazon-db;Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```
