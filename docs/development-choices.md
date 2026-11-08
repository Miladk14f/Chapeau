# NoDesk: TGG — Development Choices

**Course:** 1918IN233A · **Group:** TGG  
**Tech stack:** ASP.NET Core 8 MVC · MongoDB Atlas · C#

---

## 1. Architecture

### Repository pattern
Controllers never touch MongoDB directly. Each collection has a dedicated
repository interface (`IEmployeeRepository`, `ITicketRepository`) with a
concrete implementation. Services (`IEmployeeService`, `ITicketService`,
`ITicketSearchService`) sit above repositories and contain business logic
(password hashing, deadline calculation, audit log building). Controllers call
services only.

**Alternative considered:** Active-Record style with the MongoDB driver
injected directly into controllers. Rejected because it would scatter query
logic across the codebase and make unit testing impossible without a live
database.

### Dependency injection lifetimes
| Component | Lifetime | Reason |
|---|---|---|
| `IMongoClient` | Singleton | MongoDB recommends one client per process for connection pooling |
| `IMongoDatabase` | Scoped | Re-resolved per request so collection handles are fresh |
| Repositories / Services | Scoped | Match `IMongoDatabase` scope |

---

## 2. Database design

### ERD fidelity
The model follows the ERD exactly:

- `EMPLOYEES` → `Employee` class with `_id`, `name`, `email`, `passwordHash`, `role`, `department`, `createdAt`.
- `TICKETS` → `Ticket` class. `reportedBy` and `assignedTo` are stored as plain `ObjectId` string references, **not embedded snapshots**.
- `LOG (embedded in Ticket)` → `TicketLog` class embedded in `Ticket.Logs[]`. Each log entry stores `action`, `previousValue`, `newValue`, `note`, `changedBy` (ObjectId ref), and `timestamp`.

### Why FK references, not embedded user snapshots in Ticket
Embedding a full user object (name, email) inside each ticket document would
denormalise the data and require bulk updates of all tickets if an employee's
name changes. Using ObjectId references keeps the document small and accurate.
The trade-off is an extra round trip at display time — mitigated by batched
multi-id lookups (`$in` filter) in `GetByIdsAsync` to load all needed names in
a single query.

### Indexes created at startup (`DatabaseInitializer`)
| Collection | Index | Type | Purpose |
|---|---|---|---|
| employees | `email` | Unique | Enforce unique email, fast login lookup |
| tickets | `reportedBy` | Ascending | Filter tickets by employee |
| tickets | `status` | Ascending | Dashboard status counts |
| tickets | `dateReported` | Descending | Default sort (newest first) |
| tickets | `title + description` | Text | Full-text keyword search (individual feature) |

---

## 3. Authentication & authorisation

### Cookie authentication
ASP.NET Core cookie authentication with Claims-based identity. After a
successful login the following claims are stored:

| Claim | Value |
|---|---|
| `NameIdentifier` | Employee's `ObjectId` |
| `Name` | Display name |
| `Email` | Email |
| `Role` | `Employee` or `ServiceDesk` |

**Alternative considered:** JWT tokens. Rejected because the app is a
traditional server-rendered MVC app with no separate SPA. Cookie auth is
simpler, works out of the box with `[Authorize]` and tag-helpers, and handles
CSRF through anti-forgery tokens which Razor already generates.

### Password hashing
`PasswordHelper` uses `Rfc2898DeriveBytes.Pbkdf2` (PBKDF2-SHA256,
100 000 iterations, 16-byte random salt, 32-byte derived key). The stored hash
format is `Base64( 0x01 version_byte | salt[16] | hash[32] )`.

No extra NuGet package required — `System.Security.Cryptography` is part of
the BCL. This keeps the Seeder and web project dependency-free from hashing
libraries.

**Alternative considered:** `BCrypt.Net-Next`. Comparable security but adds a
NuGet dependency. PBKDF2 from the BCL was preferred.

---

## 4. Data access patterns

### Status counts — single aggregation
`GetStatusCountsAsync` groups by `status` in a single `$group` pipeline:

```
tickets.Aggregate()
       .Match(filter)
       .Group(t => t.Status, g => new { Status = g.Key, Count = g.Count() })
```

This costs one round trip instead of three `CountDocumentsAsync` calls, which
matters when the dashboard loads.

### Employee ticket counts — `$lookup + $size`
`GetAllWithTicketCountAsync` uses a BsonDocument pipeline with `$lookup` to
join the `tickets` collection, then `$addFields` with `$size` to count
the matched array, then `$project` to discard the array before returning. One
query returns every employee with their count — no N+1 problem.

### Cross-collection display join (C# batching)
Ticket list pages show employee names, but `Ticket` only stores ObjectId refs.
The service layer:
1. Loads all relevant tickets (one query).
2. Collects unique `reportedBy` / `assignedTo` IDs.
3. Calls `GetByIdsAsync(ids)` which uses a `$in` filter — one query.
4. Builds a dictionary and projects into `TicketDisplayViewModel`.

Total: **2 round trips** regardless of the ticket count. MongoDB's `$lookup` in
an aggregation pipeline would also work but produces harder-to-read C# code and
is harder to test without a live database.

---

## 5. Individual feature — Full-text keyword search

### Implementation
A MongoDB text index covers `title` and `description` with equal weight.
`TicketSearchService.SearchAsync` calls
`Builders<Ticket>.Filter.Text(query, options)` and sorts by meta text score
(most relevant first).

### AND vs OR mode
MongoDB `$text` treats space-separated words as **OR** by default.  
AND mode is achieved by wrapping each individual word in double quotes before
passing to the driver:

```csharp
// AND: "word1" "word2"  — both required
string andQuery = string.Join(" ", words.Select(w => $"\"{w}\""));

// OR: word1 word2  — either suffices
string orQuery = rawQuery;
```

This uses only standard MongoDB text search — no Lucene or Atlas Search
required, keeping infrastructure simple.

---

## 6. Frontend

### Bootstrap 5 + Bootstrap Icons (CDN)
Loaded from `cdn.jsdelivr.net`. No local build step needed; the app works with
only `dotnet run`.

### Chart.js (cdnjs)
Dashboard doughnut charts use Chart.js 4 from `cdnjs.cloudflare.com`. Data is
serialised from the server into the page as a JSON object in a `<script>` block
so no AJAX call is needed on page load.

### Client-side table filtering
Ticket and Employee list pages implement real-time filter-as-you-type using
plain JavaScript (no jQuery, no additional framework). Each `<tr>` stores
filter keys as `data-*` attributes set server-side so the JS never touches the
DOM for text extraction.

---

## 7. Testing

### NUnit connectivity test
`NoSQL_Project.Tests/ConnectivityTest.cs` reads the connection string from the
`.env` file and verifies that:
1. A `MongoClient` can be created.
2. A `ListDatabases` command succeeds within 10 seconds.

This confirms Atlas credentials and network access without requiring the full
web app to run.

### Bogus seeder (`NoSQL_Project.Seeder`)
`Program.cs` in the Seeder project uses the **Bogus** library to generate
realistic fake employees (names, emails, departments, roles) and tickets
(titles, descriptions, statuses, priorities, deadlines). It calls
`DatabaseInitializer.CreateIndexesAsync` before seeding, so indexes are always
present.

---

## 8. Environment configuration

The `.env` file at the solution root stores secrets:

```
Mongo__ConnectionString=mongodb+srv://…
```

`DotNetEnv` (`Env.TraversePath().Load()`) is called before the web host is
built, so the value is available via `IConfiguration` under the key
`Mongo:ConnectionString`. The `.env` file is listed in `.gitignore`.

**Why `Mongo__ConnectionString` (double underscore)?** ASP.NET Core uses `__`
as the hierarchy separator in environment variables, so this maps automatically
to the `Mongo.ConnectionString` configuration section without any extra code.
