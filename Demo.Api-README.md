# Demo GraphQL Services

The RoverDotNet.Demo.Api.* directories contain two functional GraphQL API services that demonstrate federated subgraphs based on the Apollo Federation v2.3 specification.

## Services

### Users Service (Port 4001)
- **Project**: `RoverDotNet.Demo.Api.Users`
- **Default URL**: `http://localhost:4001/graphql`
- **Sandbox**: `http://localhost:4001/graphql` (opens Banana Cake Pop IDE)

Provides user management queries with hardcoded test data:
- 5 sample users (johndoe, janesmithdev, bobbuilder, alicewonder, charliebrown)
- Queries: `user(id)`, `users`, `me`

### Products Service (Port 4002)
- **Project**: `RoverDotNet.Demo.Api.Products`
- **Default URL**: `http://localhost:4002/graphql`
- **Sandbox**: `http://localhost:4002/graphql` (opens Banana Cake Pop IDE)

Provides product catalog queries with hardcoded test data:
- 8 sample products across different categories
- Queries: `product(id)`, `products(limit, offset)`, `productsByCategory(category)`

## Running the Services

### Option 1: Visual Studio
1. Right-click each project and select "Debug > Start Without Debugging"
2. Both services will start on their respective ports

### Option 2: Command Line

**Users Service:**
```powershell
cd src/RoverDotNet.Demo.Api.Users
dotnet run
```

**Products Service (in a new terminal):**
```powershell
cd src/RoverDotNet.Demo.Api.Products
dotnet run
```

## Example Queries

### Users Service Examples

```graphql
# Get a specific user
query {
  user(id: "1") {
	id
	username
	email
	firstName
	lastName
	isActive
  }
}

# Get all users
query {
  users {
	id
	username
	email
  }
}

# Get current user
query {
  me {
	username
	email
  }
}
```

### Products Service Examples

```graphql
# Get a specific product
query {
  product(id: "1") {
	id
	name
	description
	price
	currency
	category
	inStock
	stockQuantity
	createdBy {
	  id
	}
  }
}

# Get products with pagination
query {
  products(limit: 5, offset: 0) {
	id
	name
	price
	category
  }
}

# Get products by category
query {
  productsByCategory(category: ELECTRONICS) {
	id
	name
	price
	stockQuantity
  }
}
```

## Technical Details

- **Framework**: ASP.NET Core (.NET 10)
- **GraphQL Library**: HotChocolate 16.3.0
- **Data Storage**: In-memory (hardcoded lists)
- **Federation Support**: Basic @key directives for entity references

## Notes

- Mutations are implemented but don't persist data (for demo purposes only)
- The services use HotChocolate's built-in Banana Cake Pop GraphQL IDE
- Cross-service entity references (e.g., Product.createdBy -> User) are stubbed
- No authentication or authorisation is implemented
