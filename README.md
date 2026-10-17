
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

- **GET** `/cache/locations?page={n}&pageSize={n}`  
	Get a list of all locations. `page`/`pageSize` are optional (pageSize 1–500, default 50 when only `page` is set); the unpaged total is returned in the `X-Total-Count` header.
- **GET** `/cache/locations/{searchTerm}`  
	Search locations by term.
- **GET** `/cache/categories`  
	Get a list of all categories.
- **GET** `/cache/items?page={n}&pageSize={n}`  
	Get a list of all items. Same optional pagination and `X-Total-Count` header as `/cache/locations`.
- **GET** `/cache/items/{searchTerm}`  
	Search items by term.

---

### **Development Endpoints** (`/dev`)
*Require Authorization: `dev`*

- **GET** `/dev/updateCache`  
	Synchronizes the cache from UEX API into the local database and compiles locations. Returns a Server-Sent Events (SSE) stream (`text/event-stream`).

	**Query Parameters:**
	| Parameter | Type | Default | Description |
	|---|---|---|---|
	| `updateItems` | `boolean` | `false` | If `true`, synchronizes UEX categories and items (~8,000 items). |
	| `updateLocations` | `boolean` | `false` | If `true`, synchronizes POIs, space stations, cities, and compiles `StarLocations`. |

	**Queue Architecture & Concurrency:**
	- Cache sync jobs are queued via an in-process FIFO `CacheSyncQueue` and processed sequentially by a background worker (`CacheSyncWorker`).
	- If multiple users or tabs trigger syncs simultaneously, subsequent requests wait in line rather than colliding in MariaDB.
	- The database operations are fully decoupled from HTTP request lifetimes; if a client disconnects or times out, the background worker safely finishes updating the database.

	**SSE Event Contract:**
	Each event is emitted as a JSON object on the `data:` line:

	| Status (`status`) | Accompanying Fields | Description |
	|---|---|---|
	| `queued` | `message: string` | Emitted when a job is waiting in line behind an active sync. |
	| `started` | _(none)_ | Sync execution has begun. |
	| `updating_categories` | _(none)_ | Fetching and updating UEX category definitions. |
	| `updating_items` | `category: { id: number, name: string }`, `progress: { current: number, total: number }` | Emitted for each category during item sync. **Note for frontend:** use `progress.current` and `progress.total` to render granular progress (e.g. `(current / total) * 100%`). |
	| `processing` | `category?: string`, `note?: string` | Periodic heartbeat emitted every 5s during processing to keep connections and proxies alive. |
	| `items_updated` | _(none)_ | All categories and items have finished saving to the database. |
	| `updating_pois` | _(none)_ | Updating points of interest from UEX. |
	| `updating_space_stations` | _(none)_ | Updating space stations from UEX. |
	| `updating_cities` | _(none)_ | Updating cities from UEX. |
	| `compiling_locations` | _(none)_ | Merging POIs, stations, and cities into `StarLocations`. |
	| `completed` | `response: { totalItems: number, totalPois: number, totalStations: number, totalCities: number, total: number }` | All requested sync steps finished successfully with final counts. |
	| `error` | `step: string`, `message?: string` | Emitted if an error occurred in a specific step. |

	**Reverse Proxy / Gateway Note:**
	A full sync takes ~25 seconds. Ensure intermediate reverse proxies (e.g., Envoy Gateway, Nginx, Cloudflare) have their route timeouts configured to at least 300s for this endpoint to prevent premature termination.

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

- **GET** `/personal/items?search={term}`  
	Get a list of all personal items, or search them when `search` is given.
- **GET** `/personal/items/{id}`  
	Get one personal item by numeric ID (404 if it does not exist or is not yours).
- **GET** `/personal/items/{searchTerm}`  
	Search personal items by a non-numeric term (prefer `?search=`; numeric terms resolve to the item-by-ID route).
- **POST** `/personal/items`  
	Add a personal item to inventory.
- **DELETE** `/personal/items/{id}`  
	Remove a personal item from inventory by ID.
- **PUT** `/personal/items/{id}`  
	Update one personal item by ID (quantity, location, organization sharing).
- **DELETE** `/personal/wipePersonalItems`  
	Wipe all personal items for the current user.

---

### **Recipe Endpoints** (`/recipe`)
*Require Authorization: `user` (some require `organization`)*

- **GET** `/recipe/`  
	Get a list of all recipes.
- **GET** `/recipe/{id}`  
	Get one recipe by ID.
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
- Error responses use RFC 7807 ProblemDetails (`application/problem+json`); validation failures return 400 with an `errors` map per field.
- The caller's username is read from the JWT `nickname` claim, falling back to `preferred_username`, then `sub`.
- See [docs/FRONTEND_INTEGRATION_WS4.md](docs/FRONTEND_INTEGRATION_WS4.md) for the full list of front-end-facing changes.

---
