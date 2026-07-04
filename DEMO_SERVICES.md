# RoverDotNet Demo Services

Two functional GraphQL API services demonstrating Apollo Federation v2.3 subgraphs with sample data.

## 🚀 Quick Start

### Build the projects first:
```powershell
dotnet build
```

### Option 1: Use the launcher script (easiest)
```powershell
.\start-demo-services.ps1
```

### Option 2: Start services individually

**Terminal 1 - Users Service:**
```powershell
cd src\RoverDotNet.Demo.Api.Users
dotnet run
```

**Terminal 2 - Products Service:**
```powershell
cd src\RoverDotNet.Demo.Api.Products
dotnet run
```

### Option 3: Visual Studio
1. Set multiple startup projects:
   - Right-click Solution → "Configure Startup Projects"
   - Select "Multiple startup projects"
   - Set both `RoverDotNet.Demo.Api.Users` and `RoverDotNet.Demo.Api.Products` to "Start"
2. Press F5

## 📡 Service Endpoints

| Service | Port | GraphQL Endpoint | Sandbox |
|---------|------|------------------|---------|
| **Users** | 4001 | `http://localhost:4001/graphql` | Open in browser |
| **Products** | 4002 | `http://localhost:4002/graphql` | Open in browser |

Both services include the **Banana Cake Pop** GraphQL IDE for easy testing.

## 📊 Sample Data

### Users (5 users)
- johndoe (ID: 1)
- janesmithdev (ID: 2)
- bobbuilder (ID: 3)
- alicewonder (ID: 4)
- charliebrown (ID: 5)

### Products (8 products)
- Electronics: Wireless Mouse, Laptop Stand
- Clothing: Cotton T-Shirt
- Books: Programming in C#
- Home: Coffee Maker
- Sports: Yoga Mat
- Toys: Building Blocks Set (out of stock)
- Food: Organic Granola

## 🔍 Example Queries

### Users Service

```graphql
# Get all users
{
  users {
	id
	username
	email
	firstName
	lastName
	isActive
  }
}

# Get specific user
{
  user(id: "1") {
	username
	email
  }
}

# Get current user
{
  me {
	username
	email
  }
}
```

### Products Service

```graphql
# Get all products (with pagination)
{
  products(limit: 10, offset: 0) {
	id
	name
	price
	currency
	category
	inStock
	stockQuantity
  }
}

# Get specific product
{
  product(id: "1") {
	name
	description
	price
	createdBy {
	  id
	}
  }
}

# Get products by category
{
  productsByCategory(category: ELECTRONICS) {
	name
	price
	inStock
  }
}
```

## 🏗️ Architecture

Both services are built with:
- **Framework**: ASP.NET Core 10.0
- **GraphQL**: HotChocolate 16.3.0
- **Data**: In-memory repositories with hardcoded data
- **Federation**: Basic @key directives for entity references

## 📝 Notes

- **No persistence**: Data is hardcoded for demo purposes
- **Mutations work but don't save**: They return mock responses
- **Federation refs**: Product.createdBy → User reference is stubbed
- **No auth**: Services are open for testing
- **Cross-references**: The schemas support federation but entity resolution isn't fully implemented

## 🛠️ Schema Files

Original GraphQL SDL schemas are in:
- `src/RoverDotNet.Demo/Schemas/users.graphql`
- `src/RoverDotNet.Demo/Schemas/products.graphql`

## 🎯 Use Cases

These demo services are perfect for:
- Testing RoverDotNet CLI commands
- Learning Apollo Federation concepts
- Exploring GraphQL introspection
- Demonstrating subgraph composition
- Training and development scenarios

## 📚 Additional Documentation

See `src/RoverDotNet.Demo.Api.Users/README.md` for more detailed examples and technical information.
