# Bestella Technical Overview

## Technical Overview

Bestella is a .NET 8 Blazor Web App using:

- Blazor Server interactivity
- Entity Framework Core 8
- PostgreSQL via Npgsql
- Bootstrap and Bootstrap Icons
- Razor components for UI and routing

Startup is configured in [Program.cs](Program.cs). It registers `OrderDbContext` with PostgreSQL and enables interactive server-rendered Razor components.

The project targets `net8.0` and uses `Microsoft.EntityFrameworkCore.Design` plus `Npgsql.EntityFrameworkCore.PostgreSQL`, as defined in [Bestella.csproj](Bestella.csproj).

## Data Model

The main entity is `ParsedOrder`:

- `Id`
- `Name`
- `Address`
- `Date`
- `Phone`
- Collection of `OrderItem`

Each `OrderItem` contains:

- `ProductName`
- `Quantity`
- `ParsedOrderId`

The relationship is one-to-many: one order has many items. Deleting an order cascades to its items. This is defined in [OrderDbContext.cs](Components/Data/OrderDbContext.cs) and the EF Core migrations.

A notable design choice is that `Date` is stored as a `string`, not as a PostgreSQL date or timestamp.

## Order Entry Flow

The `/pasteorder` page is implemented in [PasteOrder.razor](Components/Pages/PasteOrder.razor).

The process is:

1. The baker pastes WhatsApp text into a textarea.
2. Clicking **Parse** splits the text into non-empty lines.
3. Lines beginning with these labels are recognized:
   - `Nama`
   - `Alamat`
   - `Tarikh`
   - `Phone`
   - `Type`
4. Values are extracted by splitting each line at the first colon.
5. Every `Type` line becomes one `OrderItem` with quantity `1`.
6. A custom numeric order ID is generated.
7. EF Core saves the order and its items to PostgreSQL.
8. The parsed order is displayed and a success toast appears.

The parser is currently a simple line-based parser. It is not an AI parser and does not understand arbitrary WhatsApp formats.

## Order History

The `/allorders` page is implemented in [AllOrders.razor](Components/Pages/AllOrders.razor).

It:

- Loads orders with their related items using `Include(o => o.Items)`
- Sorts by the string value of `Date`
- Displays all orders in a table
- Opens an edit modal when a row is clicked
- Allows editing name, phone, date, address, products, and quantities
- Deletes orders
- Replaces all existing items when saving edits

The page uses a copied order object for editing, so changes are not written until **Save** is selected.

## Calendar

The `/calendarorders` page is implemented in [CalendarOrders.razor](Components/Pages/CalendarOrders.razor).

It:

1. Loads all orders from PostgreSQL.
2. Includes their items.
3. Parses each string date using a fixed list of accepted formats.
4. Filters orders for the selected month.
5. Builds a Sunday-first calendar grid in memory.
6. Displays the number of orders on each date.
7. Opens a popup showing customer and item details when a date is clicked.

The calendar accepts formats such as:

```text
dd MMMM yyyy
dd/MM/yyyy
dd/MM/yy
yyyy-MM-dd
MM/dd/yyyy
```

Parsing uses `CultureInfo.InvariantCulture`, so localized month names may not work reliably.

## Navigation and UI

Navigation is defined in [NavMenu.razor](Components/Layout/NavMenu.razor):

- `Order` -> `/pasteorder`
- `History` -> `/allorders`
- `Calendar` -> `/calendarorders`

The shared layout is in [MainLayout.razor](Components/Layout/MainLayout.razor). The root landing page is [Home.razor](Components/Pages/Home.razor).

## Database and Configuration

The PostgreSQL connection string is configured in [appsettings.json](appsettings.json). The schema is represented by:

- [InitialCreate migration](Migrations/20250615073945_InitialCreate.cs)
- [InitPostgres migration](Migrations/20250705161339_InitPostgres.cs)
- [Model snapshot](Migrations/OrderDbContextModelSnapshot.cs)

`Program.cs` does not automatically call `Database.Migrate()`, so migrations must be applied separately with EF tooling.

The connection string currently contains a database password directly in configuration. That credential should be rotated and moved to user secrets or environment variables.

## Important Current Limitations

- Dates are stored as strings, making sorting and filtering fragile.
- The parser only recognizes exact prefixes and colon-separated values.
- Quantities from WhatsApp are ignored; every parsed item starts at `1`.
- The generated order ID uses a fixed user ID plus a timestamp to the second, which can collide if two orders are created within the same second.
- There is no user/account model or authentication.
- There is no validation for missing name, date, phone, or items.
- Calendar month changes launch an asynchronous reload without awaiting it.
- All orders are loaded into memory before calendar filtering.
- The current machine cannot build the application until the required .NET 8 SDK is installed, as specified in [global.json](global.json).
