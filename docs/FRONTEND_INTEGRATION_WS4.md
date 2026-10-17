# Front-End Integration Spec — WS4: API Standards, REST Semantics & Error Handling

Tracking: EVE-41 (Workstream 4 of EVE-34). Branch: `feat/ws4-api-standards`.

This document lists every change in this workstream that the front end can observe. Changes are
grouped by impact. **This PR must not be merged until the front end has consolidated these changes.**

Legend: 🔴 breaking (front-end code likely needs a change) · 🟡 behaviour change (check error handling) · 🟢 additive (opt-in)

---

## 1. Summary table

| Endpoint | Change | Impact |
|---|---|---|
| `GET /personal/items/{searchTerm}` | Purely numeric terms (e.g. `123`) now hit the new get-by-ID route and return a **single object** (or 404) instead of an array | 🔴 |
| `POST /recipe/` | 201 now returns the created recipe as JSON plus a `Location: /recipe/{id}` header (was an empty body) | 🟡 |
| `PUT /recipe/{id}` | 200 now returns the updated recipe as JSON (was empty). Mismatched array lengths are 400 (was 404). Renaming to an existing recipe's name is 409 (new) | 🟡 |
| `POST /recipe/`, `PUT /recipe/{id}` | Body validation: 400 ValidationProblem for empty `name`, empty `uexItemIds`, mismatched lengths, or any `itemAmounts` ≤ 0 | 🟡 |
| `POST /personal/items`, `PUT /personal/items/{id}` | Body validation: 400 ValidationProblem for empty/whitespace `name` or `quantity` ≤ 0 | 🟡 |
| `PUT /personal/items/{id}` | Item not found is 404 (was 500). Item owned by another user is 403 (was 500) | 🟡 |
| `DELETE /personal/items/{id}` | Item owned by another user is 403 (was 500) | 🟡 |
| `POST /organization/participatingusers` | 400 ValidationProblem for empty `username` | 🟡 |
| All `{id}` routes (`/personal/items/{id}`, `/recipe/{id}`) | Non-numeric IDs are 404 (route constraint `{id:int}`); before, binding failed with 400 | 🟡 |
| All endpoints | Error responses without a body (401, 403, 404, 405, …) and unhandled exceptions (500) now return RFC 7807 ProblemDetails in **production** too | 🟡 |
| `GET /personal/items/{id}` | **New**: get one personal item by ID | 🟢 |
| `GET /personal/items?search={term}` | **New** optional query parameter, the recommended way to search | 🟢 |
| `GET /recipe/{id}` | **New**: get one recipe by ID | 🟢 |
| `GET /cache/items`, `GET /cache/locations` | **New** optional `page` / `pageSize` query params and an `X-Total-Count` response header. Results are now ordered by `id` | 🟢 |
| CORS | `X-Total-Count` and `Location` are now exposed to browser JavaScript | 🟢 |
| Auth (JWT) | Username claim lookup: `nickname` → `preferred_username` → `sub`. No change for tokens that carry `nickname` | 🟢 |

No request or response **model shapes** changed: `StarItem`, `Recipe`, `User`, `StarLocation` and
`UexItem` keep the same JSON fields.

---

## 2. Personal items (`/personal`)

### 2.1 🔴 `GET /personal/items/{searchTerm}`: numeric terms
Routing is now:

| Route | Matches | Returns |
|---|---|---|
| `GET /personal/items/{id:int}` | `/personal/items/42` | `200 StarItem` or `404` |
| `GET /personal/items/{searchTerm}` | `/personal/items/laser` | `200 StarItem[]` (max 10) |

A search for a purely numeric term (e.g. `/personal/items/300`) now returns a single item by ID, or
404. **Action:** switch searches to `GET /personal/items?search={term}`, which accepts any term. The
path-based search route still works for non-numeric terms, for compatibility.

### 2.2 🟢 `GET /personal/items/{id}` (new)
- `200` → `StarItem`
- `404` → item does not exist **or belongs to another user** (deliberately the same response)
- `401` → token has no usable username claim

### 2.3 🟢 `GET /personal/items?search={term}` (new query param)
- No `search` (or blank) → all of your items (unchanged behaviour)
- `search=foo` → items whose name contains `foo`, case-insensitive, max 10 (same as the path-based search)

### 2.4 🟡 `POST /personal/items`
Request body unchanged (`StarItem`; `username` is still required in JSON but is overwritten server-side).

| Status | When | Body |
|---|---|---|
| `201` | Created or merged into an existing stack | `StarItem`, plus a `Location` header |
| `400` | **New:** `name` empty/whitespace or `quantity` ≤ 0 | ValidationProblem (see §5) |
| `400` | Unknown `locationId` (unchanged) | JSON string `"Invalid Location"` |
| `401` | No username claim | ProblemDetails |

### 2.5 🟡 `PUT /personal/items/{id}`: update (handler renamed `UpdatePersonalItem`)
Same URL and payload. The handler was misnamed `GetOnePersonalItemFromId`; it has always updated the item.

| Status | When | Body |
|---|---|---|
| `200` | Updated (or merged into a stack at the new location) | `StarItem` (the resulting stack) |
| `400` | **New:** `name` empty or `quantity` ≤ 0 | ValidationProblem |
| `403` | **Was 500:** item belongs to another user | ProblemDetails |
| `404` | **Was 500:** item does not exist. Also returned for non-numeric `{id}` | ProblemDetails |

> Note: `quantity: 0` is now rejected. To remove an item, use `DELETE /personal/items/{id}`.

### 2.6 🟡 `DELETE /personal/items/{id}`
| Status | When |
|---|---|
| `200` | Deleted (empty body, unchanged) |
| `403` | **Was 500:** item belongs to another user (ProblemDetails) |
| `404` | Not found (JSON string `"Item not found"`, unchanged) |

---

## 3. Recipes (`/recipe`)

### 3.1 🟢 `GET /recipe/{id}` (new)
`200 Recipe` or `404`.

### 3.2 🟡 `POST /recipe/` *(organization role)*
| Status | Before | After |
|---|---|---|
| `201` | empty body | `Recipe` JSON (with the server-assigned `id`) and `Location: /recipe/{id}` |
| `400` | empty body | ValidationProblem with per-field `errors` |
| `409` | empty body | JSON string `"A recipe with this name already exists."` |

Validation rules (`Recipe`):
- `name`: required, not empty or whitespace
- `uexItemIds`: at least one entry
- `itemAmounts`: same length as `uexItemIds`, and every value > 0

> Field-level rules (`name`) are reported first. The cross-field rules (`uexItemIds` / `itemAmounts`)
> are only evaluated once the field-level rules pass, so a client may see two 400s in a row while
> fixing a form. Validate on the client as well, to avoid that.

### 3.3 🟡 `PUT /recipe/{id}` *(organization role)*
| Status | Before | After |
|---|---|---|
| `200` | empty body | updated `Recipe` JSON |
| `400` | (mismatched arrays returned **404**) | ValidationProblem (same rules as POST) |
| `404` | not found | not found (unchanged). Also returned for a non-numeric `{id}` |
| `409` | — | **New:** another recipe already has this `name` |

### 3.4 `DELETE /recipe/{id}`
Unchanged, except that a non-numeric `{id}` now returns 404 instead of 400.

---

## 4. Cache (`/cache`): optional pagination 🟢

Applies to `GET /cache/items` and `GET /cache/locations`.

| Query param | Type | Default | Rules |
|---|---|---|---|
| `page` | int | — | ≥ 1 |
| `pageSize` | int | 50 (when only `page` is given) | 1–500 |

- **Neither param given:** returns the full list, as before. Front-end code that does not page keeps working.
- **Either param given:** returns that page (`Skip((page-1)*pageSize).Take(pageSize)`).
- The response body is always a **plain JSON array** (no envelope).
- `X-Total-Count` response header: the total number of rows before paging. It is set on every
  response, paged or not, and exposed through CORS.
- Results are ordered by `id` ascending, so pages are stable.
- Invalid values (e.g. `page=0`, `pageSize=1000`) → `400` ValidationProblem with `errors.page` / `errors.pageSize`.

Example:
```http
GET /cache/items?page=2&pageSize=100
→ 200 OK
X-Total-Count: 4312
[ { "id": 101, ... }, ... ]
```

The search routes (`/cache/items/{searchTerm}`, `/cache/locations/{searchTerm}`) and `/cache/categories` are unchanged.

---

## 5. Error format (all endpoints) 🟡

All error responses now use **RFC 7807 ProblemDetails** (`Content-Type: application/problem+json`)
in every environment. Before this change that only happened in Development, and production returned
empty bodies or bare 500s.

### 5.1 Generic ProblemDetails
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Resource not found",
  "status": 404,
  "detail": "Item not found",
  "traceId": "00-…"
}
```
- **500s** carry `"title": "An unexpected error occurred"` and **no `detail`** in production; exception
  messages are not exposed. Quote the `traceId` when reporting a bug.
- **401/403** from the authorization middleware now also have a ProblemDetails body (they were empty).

### 5.2 ValidationProblem (400)
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Name must not be empty."],
    "Quantity": ["Quantity must be greater than 0."]
  },
  "traceId": "00-…"
}
```
Keys in `errors` are the **C# property names** (PascalCase: `Name`, `Quantity`, `UexItemIds`,
`ItemAmounts`, `Username`), except the pagination keys `page` / `pageSize`. Match them case-insensitively
against form field names.

### 5.3 Unchanged string error bodies
These responses still return a **JSON string** body, not ProblemDetails:
- `POST /personal/items` → `400 "Invalid Location"`
- `DELETE /personal/items/{id}` → `404 "Item not found"`
- `POST /organization/participatingusers` → `409 "User already exists in organization inventory."`
- `DELETE /organization/participatingusers/{username}` → `404 "User not found in organization inventory."`
- `POST/PUT /recipe` → `409 "A recipe with this name already exists."`

**Recommended client handling:** branch on `Content-Type`. For `application/problem+json`, show
`detail ?? title`, and map `errors` onto form fields. For `application/json` with a string body,
show the string.

---

## 6. Authentication: username claim 🟢

Handlers now share one helper (`ClaimsPrincipal.GetUsername()`). It takes the first non-empty value of:
1. `nickname` (unchanged primary; existing inventories keep resolving to the same user)
2. `preferred_username`
3. `sub` (or its mapped form `ClaimTypes.NameIdentifier`)

No front-end change is needed. Tokens that lack `nickname` used to return 401; they now resolve to
`preferred_username` or `sub`.

---

## 7. Front-end checklist

- [ ] Replace path-based personal item search with `GET /personal/items?search=` (required for numeric search terms).
- [ ] Recipe create/edit: use the returned `Recipe` body (e.g. to get the new `id`) instead of re-fetching, if that's convenient.
- [ ] Recipe edit: handle the new `409` (duplicate name). Mismatched arrays are now `400`, not `404`.
- [ ] Item forms: check `quantity > 0` and a non-empty `name` on the client; handle `400` ValidationProblem `errors`.
- [ ] Item update/delete: handle `403` (not your item) and `404` (gone) instead of a generic 500.
- [ ] Error toast/handler: parse `application/problem+json` (`title`, `detail`, `errors`, `traceId`).
- [ ] (Optional) Page the item/location pickers with `page`/`pageSize` and read `X-Total-Count`.
