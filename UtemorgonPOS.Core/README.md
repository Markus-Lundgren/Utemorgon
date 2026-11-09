# Core

This is the core utilities classlib. It contains everything we need to speak with the database and manage the object returns.

## About mixing of concerns

Since we are a 3 man team of mixed experience, some code purity has been sacraficed in favor of easy collaboration.  
Concenrs regarding data fetching has been mixed to some degree, and we do not utilize things like finance or order managers,
instead we opt for centralized queries to be used in pure C#.

## TODO:

### Generic

- Add all necessary models from DB schema
- Add db connection

### Queries

#### Customer

- Add customer
- Fetch customer with ID
- Fetch customers with IDs (Returns a collection of Customer)
-   - Append all with SQL "OR id = ..." (Procedural queries)
-   -   - Use "string.Join()"
- Fetch customer with SSN
- Fetch customers with late rentals
- Update customer (any field except ID)
- Delete customer FLOW
-   - Point all current orders to DELETED CUSTOMER (Id: 1)
-   - Delete row

#### Order

- Create new order
- Get all orders for Customer.
-   - FromDate?
-   - ToDate?
-   - Get all unpaid orders
-   -   - CustomerId?
- Update order (only Customer ID)

#### Product

- Create product
-   - Must specify product type etc
- Create barcode (R-Code using sequence ID. Intermediate / Hard!)
- Get product from barcode
- Check stock
- Update ProductCategory price
- Add barcode to product
- UPDATE PRODUCT
-   - Name
-   - PriceOverride
-   - Category (type)
-   - Stock
- Remove (soft-delete)
- Remove barcode from product

##### Rental

- Fetch all
-   - FILTERS
-   -   - Rented?
-   -   - Late?
-   -   - ProductName?
-   - In general
-   - For customer (ID)
-   - (Shows with barcode)

## Example usages

```csharp
Data.Orders.GetForCustomer(Data.Customers.GetFromSSN("1997-12-09").Id);


Customer[] customers = Data.Customers.GetFromID([2, 3, 4, 5]);


List<int> csid = [];
var lates = Data.Rentals.GetAllLate();
foreach(var late in lates)
	csid.Add(late.CustomerId);

return Data.Customers.GetFromId(cs);

```
