# PizzaShop — Intentional Bugs for Copilot Demo

All bugs compile without errors or warnings. Each one causes a runtime or logical
failure that is non-obvious from a quick read of the code.

---

## BUG-01 — Incomplete object update (Data Layer)
**File:** `PizzaShop.Data/Repositories/JsonOrderRepository.cs` — `UpdateOrderAsync`  
**Category:** Incorrect data mapping / shallow copy  
**Symptom:** After an admin saves a status change on an order, the order's pizza list
disappears from the JSON file. Subsequent reads show an empty `Pizzas` array and
`TotalPrice` of `€0.00` in the admin detail view.  
**Root cause:** Instead of replacing `orders[index]` with the full incoming `order`
object, only `Status` and `AdminNotes` are written back. The `Pizzas`, `TotalPrice`,
and all other fields on the persisted record are silently lost.

**Fix:**
```csharp
// Replace the partial property copy with a full replacement:
if (index >= 0)
    orders[index] = order;
```

---

## BUG-02 — Case-sensitive username lookup (Data Layer)
**File:** `PizzaShop.Data/Repositories/JsonAdminRepository.cs` — `GetAdminUserAsync`  
**Category:** String comparison bug  
**Symptom:** Logging in as `Admin` or `ADMIN` fails with "Invalid username or password"
even when the password is correct. Only the exact lowercase `admin` works.  
**Root cause:** The `==` operator performs an ordinal case-sensitive comparison.
The original code used `StringComparison.OrdinalIgnoreCase`.

**Fix:**
```csharp
return users.FirstOrDefault(u =>
    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
```

---

## BUG-03 — Operator precedence error in price calculation (API Layer)
**File:** `PizzaShop.Api/Services/PricingService.cs` — `CalculateCustomPrice`  
**Category:** Arithmetic / operator precedence  
**Symptom:** Custom pizza prices are wrong for any size other than Medium. For example,
a Large custom pizza with 3 toppings should be `(8.99 + 3.00) * 1.30 = €15.58` but
instead costs `8.99 + 3.00 * 1.30 = €12.89`. The base price is never scaled by size.  
**Root cause:** Missing parentheses mean `*` binds before `+`, so only the
per-ingredient cost is multiplied by the size factor.

**Fix:**
```csharp
var total = (CustomBasePrice + ingredientCount * CustomBasePricePerIngredient)
            * SizeMultipliers[size];
```

---

## BUG-04 — Calculated total never persisted on the order (API Layer)
**File:** `PizzaShop.Api/Endpoints/OrderEndpoints.cs` — `POST /api/orders`  
**Category:** Variable not assigned / data loss  
**Symptom:** The order confirmation screen shows the correct total (e.g. `€23.97`)
because it comes from the API response. But when the admin opens the order, it shows
`€0.00`. The same `€0.00` appears in the orders list total column.  
**Root cause:** `TotalPrice = 0` is hardcoded on the `Order` object. The correctly
computed `total` variable is used only in the response DTO and never saved.

**Fix:**
```csharp
TotalPrice = total
```

---

## BUG-05 — Route parameter name mismatch causes 404 (API Layer)
**File:** `PizzaShop.Api/Endpoints/AdminEndpoints.cs` — `GET /api/admin/orders/{id}`  
**Category:** Parameter binding / typo  
**Symptom:** Clicking "View" on any order in the admin panel always shows the error
message "Could not load order." Every request returns a 404 Not Found.  
**Root cause:** The route template is `{id}` but the handler parameter is named
`orderId`. ASP.NET Core minimal APIs require the parameter name to match the route
token exactly. The unmatched parameter receives its default value (`null`), so
`GetOrderByIdAsync(null)` finds no order and returns `NotFound()`.

**Fix:**
```csharp
app.MapGet("/api/admin/orders/{id}", async (
    string id,   // must match {id}
    HttpContext ctx, ...
```

---

## BUG-06 — Off-by-one error in cart count (Web Layer)
**File:** `PizzaShop.Web/Services/CartService.cs` — `Count` property  
**Category:** Off-by-one error  
**Symptom:** The cart badge in the navbar always shows one fewer item than is actually
in the cart. With one item it shows `0` (badge hidden). With zero items it returns `-1`.  
**Root cause:** `GetCart().Count - 1` subtracts 1 from the real count.

**Fix:**
```csharp
public int Count => GetCart().Count;
```

---

## BUG-07 — Null dereference after placing an order (Web Layer)
**File:** `PizzaShop.Web/Pages/Order/Checkout.cshtml.cs` — `OnPostPlaceOrderAsync`  
**Category:** Null reference exception  
**Symptom:** Under normal operation the bug is invisible because `PlaceOrderAsync`
returns a valid object. However if the API returns an empty or malformed response body,
`result` is `null` and accessing `result.OrderId` throws a `NullReferenceException`.
The exception is not caught, bubbling up as a 500 error — and the cart has already
been cleared.  
**Root cause:** The return value of `PlaceOrderAsync` is used without a null check.

**Fix:**
```csharp
if (result is null)
{
    ErrorMessage = "The order was submitted but we received no confirmation. Please contact us.";
    return Page();
}
return RedirectToPage("/Order/Confirmation", new { id = result.OrderId });
```

---

## BUG-08 — Admin orders list sorted oldest-first (Web Layer)
**File:** `PizzaShop.Web/Pages/Admin/Orders.cshtml.cs` — `OnGetAsync`  
**Category:** Incorrect sort direction / logic error  
**Symptom:** The admin orders overview always shows the oldest orders at the top. New
incoming orders appear at the very bottom of the table, making them easy to miss.  
**Root cause:** `Orders.OrderBy(o => o.PlacedAt)` sorts ascending. The API already
returns orders newest-first, so this web-side sort silently inverts the correct ordering.

**Fix:**
```csharp
Orders = Orders.OrderByDescending(o => o.PlacedAt).ToList();
// Or simply remove the line entirely — the API ordering is already correct.
```

---

## BUG-09 — Race condition on shared token store (API Layer)
**File:** `PizzaShop.Api/Services/AdminTokenService.cs` — `GenerateToken`, `ValidateToken`, `RevokeToken`  
**Category:** Thread safety / race condition  
**Symptom:** Under concurrent admin requests (e.g. two logins or a login and a
validation at the same time), `HashSet<string>` can throw an
`InvalidOperationException` ("collection was modified during enumeration"), silently
drop a token, or return incorrect validation results. The bug is invisible under
single-user load but surfaces under stress or parallel requests.  
**Root cause:** `HashSet<T>` is not thread-safe. All three methods read and write
`_validTokens` without any synchronisation, so concurrent access causes data races.

**Fix:**
```csharp
// Option A — lock on every access:
private readonly object _lockObj = new();

public string GenerateToken(string username)
{
    var token = ComputeHmac(...);
    lock (_lockObj) { _validTokens.Add(token); }
    return token;
}

// Option B — replace the field entirely with a thread-safe collection:
private readonly System.Collections.Concurrent.ConcurrentDictionary<string, bool> _validTokens = new();
```

---

## BUG-10 — Sync-over-async deadlock inside semaphore (Data Layer)
**File:** `PizzaShop.Data/Repositories/JsonOrderRepository.cs` — `SaveOrderAsync`  
**Category:** Async / deadlock  
**Symptom:** Placing a new order hangs indefinitely under any concurrent load.
A single sequential request may work fine in development, but as soon as two order
submissions arrive close together the application stops responding entirely and
must be restarted.  
**Root cause:** `GetAllOrdersAsync().Result` blocks the current thread synchronously
while the `SemaphoreSlim` is held. Under ASP.NET Core's thread pool, all available
threads can become blocked waiting for async I/O continuations that can never be
scheduled because no threads are free — a classic sync-over-async deadlock.

**Fix:**
```csharp
// Replace the blocking call with a proper await:
var orders = await GetAllOrdersAsync();
```

---

## BUG-11 — Missing `await` on async call (API Layer)
**File:** `PizzaShop.Api/Endpoints/MenuEndpoints.cs` — `GET /api/menu`  
**Category:** Missing await / unawaited task  
**Symptom:** The menu page loads but all ingredient data is missing — the toppings
list is empty and the "Build Your Own" section shows no checkboxes. No exception is
thrown; the menu simply appears incomplete.  
**Root cause:** `menuRepo.GetIngredientsAsync()` is called without `await`, so
`ingredients` holds a `Task<List<Ingredient>>` rather than the resolved list.
The result is only awaited later with `(await ingredients)`, meaning the task does
run — but without the initial `await` the intent is obscured and the task is
started later than the other two calls, losing any benefit of concurrent execution.
More critically, in a refactor where `(await ingredients)` is removed or missed,
the entire ingredient list silently disappears.

**Fix:**
```csharp
var ingredients = await menuRepo.GetIngredientsAsync();
// Then use `ingredients` directly — no second await needed.
var ingredientDtos = ingredients.Select(...).ToList();
```

---

## BUG-12 — File stream not disposed on exception (Data Layer)
**File:** `PizzaShop.Data/Repositories/JsonFileHelper.cs` — `ReadAsync`  
**Category:** Resource leak / missing disposal  
**Symptom:** If a JSON data file is malformed (e.g. manually edited incorrectly),
`JsonSerializer.DeserializeAsync` throws an exception. The `FileStream` opened by
`File.OpenRead` is never disposed, leaving the file handle open. On Windows this
prevents any other write to the same file until the GC finaliser eventually runs,
which can cause subsequent save operations to fail with an
`IOException: The process cannot access the file because it is being used by another process`.

**Root cause:** `await using` was replaced with a plain `var` declaration, so the
stream's `DisposeAsync` is never called when an exception is thrown.

**Fix:**
```csharp
await using var stream = File.OpenRead(filePath);
return await JsonSerializer.DeserializeAsync<T>(stream, Options) ?? new T();
```

---

## Summary Table

| #  | Layer | File                        | Category                  | Visible symptom                               |
|----|-------|-----------------------------|---------------------------|-----------------------------------------------|
| 01 | Data  | `JsonOrderRepository.cs`    | Incomplete mapping        | Pizzas vanish after admin saves               |
| 02 | Data  | `JsonAdminRepository.cs`    | String comparison         | Login fails with mixed-case username          |
| 03 | API   | `PricingService.cs`         | Operator precedence       | Wrong price for custom pizzas                 |
| 04 | API   | `OrderEndpoints.cs`         | Variable not assigned     | Order total always €0.00 in admin             |
| 05 | API   | `AdminEndpoints.cs`         | Parameter name typo       | Admin order detail always 404                 |
| 06 | Web   | `CartService.cs`            | Off-by-one                | Cart badge shows wrong count                  |
| 07 | Web   | `Checkout.cshtml.cs`        | Null dereference          | Crash after order if API returns null         |
| 08 | Web   | `Orders.cshtml.cs`          | Wrong sort direction      | Newest orders appear last in admin            |
| 09 | API   | `AdminTokenService.cs`      | Race condition            | Token corruption under concurrent logins      |
| 10 | Data  | `JsonOrderRepository.cs`    | Sync-over-async deadlock  | App hangs on concurrent order submissions     |
| 11 | API   | `MenuEndpoints.cs`          | Missing await             | Ingredient list empty on menu page            |
| 12 | Data  | `JsonFileHelper.cs`         | Resource leak             | File locked after malformed JSON exception    |
