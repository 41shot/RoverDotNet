# Quick Reference: Demo GraphQL Services

## What Was Created

Two fully functional ASP.NET Core GraphQL API projects:

### 1. **RoverDotNet.Demo.Api.Users** (Port 4001)
- **Location**: `src/RoverDotNet.Demo.Api.Users/`
- **Queries**: `user(id)`, `users`, `me`
- **Sample Data**: 5 hardcoded users
- **Schema**: Implements `src/RoverDotNet.Demo/Schemas/users.graphql`

### 2. **RoverDotNet.Demo.Api.Products** (Port 4002)
- **Location**: `src/RoverDotNet.Demo.Api.Products/`
- **Queries**: `product(id)`, `products(limit, offset)`, `productsByCategory(category)`
- **Sample Data**: 8 hardcoded products across various categories
- **Schema**: Implements `src/RoverDotNet.Demo/Schemas/products.graphql`

## Key Features

✅ Both projects added to solution  
✅ HotChocolate 16.3.0 (latest non-vulnerable version)  
✅ Configured to run on correct ports (4001, 4002)  
✅ Launch settings configured with browser auto-open to GraphQL IDE  
✅ In-memory data repositories with realistic sample data  
✅ All queries functional (mutations return mock responses)  
✅ Built-in Banana Cake Pop GraphQL IDE for testing  
✅ Build verified successful  
✅ Both services tested and confirmed working

## Files Created/Modified

### New Projects
- `src/RoverDotNet.Demo.Api.Users/` (complete project)
- `src/RoverDotNet.Demo.Api.Products/` (complete project)

### Configuration
- `Directory.Packages.props` - Added HotChocolate packages
- `start-demo-services.ps1` - PowerShell launcher script
- `DEMO_SERVICES.md` - Comprehensive documentation
- `src/RoverDotNet.Demo.Api.Users/README.md` - Detailed guide

## How to Use

### 1. Build (first time)
```powershell
dotnet build
```

### 2. Run Both Services
```powershell
.\start-demo-services.ps1
```

### 3. Access GraphQL IDEs
- Users: http://localhost:4001/graphql
- Products: http://localhost:4002/graphql

### 4. Try Sample Queries
Open either endpoint in a browser and use the built-in IDE to run queries. Examples provided in DEMO_SERVICES.md.

## Project Structure

Each service includes:
- `Models/` - Data models (User/Product, inputs)
- `Data/` - In-memory repositories with sample data
- `GraphQL/` - Query resolvers and type configurations
- `Program.cs` - Service configuration
- `appsettings.json` - Configuration with port settings
- `Properties/launchSettings.json` - Debug launch settings

## Technology Stack

- **.NET 10.0** target framework
- **HotChocolate 16.3.0** for GraphQL
- **ASP.NET Core** for web hosting
- **In-memory data** (no database needed)
- **Banana Cake Pop** GraphQL IDE (included)

## Next Steps

1. Both services are ready to use with RoverDotNet CLI
2. Run queries in the GraphQL IDE to explore the data
3. Use for testing schema introspection commands
4. Demonstrate federation concepts with real working services
5. No setup needed - just build and run!

## Notes

- Services use hardcoded sample data for demonstration
- Mutations work but don't persist changes
- No authentication/authorization (demo purposes)
- Federation entity references are partially implemented
- Perfect for RoverDotNet CLI testing and demos
