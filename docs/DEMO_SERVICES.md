# Demo GraphQL Services

`RoverDotNet.Demo.Api.Users` and `RoverDotNet.Demo.Api.Products` are two small ASP.NET Core + [HotChocolate](https://chillicream.com/docs/hotchocolate) GraphQL services with hardcoded sample data. They act as example Apollo Federation subgraphs for exercising RoverDotNet (e.g. `rover dev`, schema introspection, federation composition) and are not intended to be feature-complete or production-relevant.

## Services

| Service | Port | Endpoint | Queries |
|---------|------|----------|---------|
| `RoverDotNet.Demo.Api.Users` | 4001 | `http://localhost:4001/graphql` | `user(id)`, `users`, `me` |
| `RoverDotNet.Demo.Api.Products` | 4002 | `http://localhost:4002/graphql` | `product(id)`, `products(limit, offset)`, `productsByCategory(category)` |

Both expose the built-in Banana Cake Pop GraphQL IDE at their `/graphql` endpoint.

## Running

**Launcher script (starts both):**
```powershell
.\start-demo-services.cmd
```

**Individually:**
```powershell
cd src\RoverDotNet.Demo.Api.Users
dotnet run
```
```powershell
cd src\RoverDotNet.Demo.Api.Products
dotnet run
```

**Visual Studio:** set both projects as startup projects (Solution → Configure Startup Projects → Multiple startup projects) and press F5.

## Sample data

- **Users**: johndoe, janesmithdev, bobbuilder, alicewonder, charliebrown (ids `1`-`5`).
- **Products**: 8 products across Electronics, Clothing, Books, Home, Sports, Toys and Food categories (ids `1`-`8`); `Building Blocks Set` (id `6`) is out of stock.

## Example queries

```graphql
# All users
{
  users { id username email firstName lastName isActive }
}

# Single user
{
  user(id: "1") { username email }
}

# All products
{
  products(limit: 10, offset: 0) { id name price currency category inStock stockQuantity }
}

# Products by category
{
  productsByCategory(category: ELECTRONICS) { name price inStock }
}
```

Note: `id` arguments use the GraphQL `ID!` scalar (via `[ID]` attributes on resolver parameters), so plain string values like `"1"` work directly without needing separate variable declarations.

## Notes / limitations

- Data is in-memory and hardcoded; nothing persists.
- Mutations are implemented but return mock responses without saving.
- Cross-service entity references (e.g. `Product.createdBy` → `User`) are stubbed and federation entity resolution isn't fully implemented.
- No authentication/authorisation.

Original GraphQL SDL schemas: `src/RoverDotNet.Demo/Schemas/users.graphql` and `src/RoverDotNet.Demo/Schemas/products.graphql`.
