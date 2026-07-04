# GraphQL Query Examples (No Variables Needed!)

## ✅ The Fix Applied

Added `[ID]` attributes to all ID parameters so they properly match the GraphQL `ID!` type instead of `String!`. This fixes the "variable type not compatible" error.

## 📋 Query All Data Without Variables

### Get All Users

```graphql
query GetAllUsers {
  users {
	id
	username
	email
	firstName
	lastName
	isActive
	createdAt
  }
}
```

**No variables needed!** Returns all 5 users.

---

### Get All Products

```graphql
query GetAllProducts {
  products {
	id
	name
	description
	price
	currency
	category
	inStock
	stockQuantity
	createdAt
	updatedAt
  }
}
```

**No variables needed!** Returns up to 10 products (default limit).

---

### Get All Products with Custom Limit

```graphql
query GetAllProductsCustom {
  products(limit: 20, offset: 0) {
	id
	name
	price
	category
  }
}
```

**No variables needed!** Just inline the limit/offset values.

---

### Get Products by Category

```graphql
query GetElectronics {
  productsByCategory(category: ELECTRONICS) {
	id
	name
	price
	stockQuantity
  }
}
```

**No variables needed!** Available categories:
- `ELECTRONICS`
- `CLOTHING`
- `BOOKS`
- `HOME`
- `SPORTS`
- `TOYS`
- `FOOD`
- `OTHER`

---

## 🔧 Queries With Variables (Now Fixed!)

### Get Single User

```graphql
query GetUser($userId: ID!) {
  user(id: $userId) {
	id
	username
	email
  }
}
```

**Variables:**
```json
{
  "userId": "1"
}
```

---

### Get Single Product

```graphql
query GetProduct($productId: ID!) {
  product(id: $productId) {
	id
	name
	price
	category
  }
}
```

**Variables:**
```json
{
  "productId": "1"
}
```

✅ **This now works!** The `ID!` type is properly supported.

---

## 🎯 Combined Queries (Get Everything at Once)

```graphql
query GetAllData {
  users {
	id
	username
	email
  }
  products {
	id
	name
	price
	category
  }
}
```

**No variables needed!** Returns both users and products in a single request.

---

## 📊 Sample Product IDs

- `"1"` - Wireless Mouse (ELECTRONICS)
- `"2"` - Cotton T-Shirt (CLOTHING)
- `"3"` - Programming in C# (BOOKS)
- `"4"` - Coffee Maker (HOME)
- `"5"` - Yoga Mat (SPORTS)
- `"6"` - Building Blocks Set (TOYS) - Out of stock!
- `"7"` - Organic Granola (FOOD)
- `"8"` - Laptop Stand (ELECTRONICS)

## 👥 Sample User IDs

- `"1"` - johndoe
- `"2"` - janesmithdev
- `"3"` - bobbuilder
- `"4"` - alicewonder
- `"5"` - charliebrown

---

## 🚀 Testing

### Direct Service Access (Banana Cake Pop IDE)
- Users: http://localhost:4001/graphql
- Products: http://localhost:4002/graphql

### Through Federation Gateway
If you're using Apollo Router/Gateway, these queries should now work through the federated graph as well!

---

## 💡 Quick Tip

**To query everything without any variables or complexity:**

```graphql
{
  users {
	id
	username
  }
  products {
	id
	name
  }
}
```

Just open the GraphQL IDE and paste this in! No variables tab needed.
