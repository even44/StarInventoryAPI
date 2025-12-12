
# StarInventoryAPI

## API Endpoints Documentation

Below is a list of available API endpoints, grouped by their route and purpose. All endpoints require appropriate authorization as noted.

---

### **Administration Endpoints** (`/admin`)
*Require Authorization: `admin`*

- **DELETE** `/admin/wipePersonalItems`  
	Wipes all users' personal items.
- **GET** `/admin/users`  
	Retrieves a list of all users.

---

### **Cache Endpoints** (`/cache`)
*Require Authorization: `user`*

- **GET** `/cache/locations`  
	Get a list of all locations.
- **GET** `/cache/locations/{searchTerm}`  
	Search locations by term.
- **GET** `/cache/categories`  
	Get a list of all categories.
- **GET** `/cache/items`  
	Get a list of all items.
- **GET** `/cache/items/{searchTerm}`  
	Search items by term.

---

### **Development Endpoints** (`/dev`)
*Require Authorization: `dev`*

- **GET** `/dev/updateCache`  
	Update the cache from UEX and compile a list of locations.

---

### **Organization Endpoints** (`/organization`)
*Require Authorization: `user` (some require `organization`)*

- **GET** `/organization/inventory`  
	Get all shared items from organization inventory users.
- **POST** `/organization/participatingusers`  
	Add a user to the organization inventory. (*Requires `organization` authorization*)
- **DELETE** `/organization/participatingusers/{username}`  
	Remove a user from the organization inventory. (*Requires `organization` authorization*)
- **GET** `/organization/participatingusers`  
	Get a list of organization inventory users.
- **GET** `/organization/users`  
	Get a list of all users in the organization.

---

### **Personal Endpoints** (`/personal`)
*Require Authorization: `user`*

- **GET** `/personal/items`  
	Get a list of all personal items.
- **GET** `/personal/items/{searchTerm}`  
	Search personal items by term.
- **POST** `/personal/items`  
	Add a personal item to inventory.
- **DELETE** `/personal/items/{id}`  
	Remove a personal item from inventory by ID.
- **PUT** `/personal/items/{id}`  
	Get one personal item by ID.
- **DELETE** `/personal/wipePersonalItems`  
	Wipe all personal items for the current user.

---

### **Recipe Endpoints** (`/recipe`)
*Require Authorization: `user` (some require `organization`)*

- **GET** `/recipe/`  
	Get a list of all recipes.
- **POST** `/recipe/`  
	Add a new recipe. (*Requires `organization` authorization*)
- **DELETE** `/recipe/{id}`  
	Remove a recipe by ID. (*Requires `organization` authorization*)
- **PUT** `/recipe/{id}`  
	Edit a recipe by ID. (*Requires `organization` authorization*)

---

## Notes
- All endpoints require authentication and may require specific roles as indicated.
- Replace `{id}` or `{searchTerm}` or `{username}` with the appropriate value.

---
