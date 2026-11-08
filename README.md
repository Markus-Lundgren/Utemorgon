# Utemorgon 🎬🍿

A 3 student school project utilizing MariaDB, Dapper and C# to create a Terminal based Point of Sale system.

## The environment variables

In order to connect to the database, we need an Ip, port, username and password.  
These must be placed in a `.env` file at the root of the project.  
The `.env` file must contain exactly 1 key-value-pair per row, and split them using an equals sign.  
There is no need for citations `"` to define strings.

To fetch env variables in code, use the `EasyEnv` wrapper:

```csharp
int port = EasyEnv.Get<int>("port"); // Case insensitive.
```

## Program flow

TODO: Expand this to provide some actual usable information
From the admin terminal, the cashier can choose "Start transaction".

## Diagrams and designs 📊

### Database

A note on **denomarlization**:  
We are purposefully not introducing denormalization, even though it would make sense in a large scale application.
This is to keep the normalization somewhat pure and academic for learning purposes, but also to decrease scope and possible point of failures.

_Where denormalization **could** be applied_:

- TotalPrice & TotalPayed in **Order**
- MerchandiseType in Product
  This would remove the implicit flow of `stock = null AND ProductType = 'Rental'` / `stock = null AND ProductType = 'Service'`

#### Entity relations diagram

```mermaid
erDiagram
    Customer ||--o{ Order : places
    ProductType ||--o{ ProductType : contains
    ProductType ||--o{ Product : categorizes
	MerchandiseType ||--o{ Product : defines
    Product ||--o{ BarcodeToProduct : has
    Order ||--o{ PaymentToOrder : has
    Order ||--o{ OrderPriceModifier : has
    Order ||--o{ OrderItem : includes
    BarcodeToProduct ||--o{ OrderItem : identifies
    OrderItem ||--o{ OrderItemModifier : has

    Customer {
        INT Id PK
        CHAR(12) SocialNumber
        VARCHAR(128) FName
        VARCHAR(128) LName
        VARCHAR(128) Address
        VARCHAR(20) PhoneNumber
    }

    ProductType {
        INT Id PK
        INT ProductTypeId FK
        VARCHAR(128) Name
        DECIMAL Price
    }

	MerchandiseType {
		INT Id PK
		VARCHAR(64) Name
	}

    Product {
        INT Id PK
        INT ProductTypeId FK
		INT MerchandiseTypeId FK
        VARCHAR(64) Name
        DECIMAL PriceOverride
        INT Stock
        BOOL IsRemoved
    }

    Order {
        INT Id PK
        INT CustomerId FK
        DATETIME CreationDate
    }

    PaymentToOrder {
        INT Id PK
        INT OrderId FK
        DECIMAL Amount
        DATETIME ReceivedDate
    }

    OrderPriceModifier {
        INT Id PK
        INT OrderId FK
        DECIMAL Amount
        VARCHAR(64) Reason
    }

    BarcodeToProduct {
        INT Id PK
        INT ProductId FK
        CHAR(13) Barcode UK
    }

    OrderItem {
        INT Id PK
        INT OrderId FK
        CHAR(13) Barcode FK
        VARCHAR(128) Name
        DECIMAL Price
        INT Quantity
        DATETIME ExpectedReturnDate
        DATETIME ReturnedDate
    }

    OrderItemModifier {
        INT Id PK
        INT OrderItemId FK
        DECIMAL Amount
        VARCHAR(64) Reason
    }
```

#### Arrow diagram

_Note: Mermaid doesn't allow for neat diagrams. Instead see the attached image._

[![SQL Visualizer](/repo-media/utemorgon_arrowdiagram.svg)]

If you want to see the raw definitions for the arrow diagram, please see [SQL Visualizer (Hashlink)](https://trangius.se/sql_visualizer/#d=lVbNbts4EH6VD-qhDqAYSbpYLAJsF2nj7hpoftZJ0YsuY3JsEaFIlaScGMXe9pn6An2xxVCSkzotir0YFEcczvczI38uXHFafEjc-LD2riiLpjgtEj-koixicVq8wC6IjdHsA99NK1e5FzizFm-7mHzDAZoSwUScHN7TFjOnwrZNrHFvUg3CHW-ncmZ8P6LpYsKSUVPQh4Ebv6Gl5Sk-1uxAUGNiE5GjrEsoyxRA1iLVbAJWhq2OIKcROQ2bjhoGRZzP3s9uZ-d4--Hm9upitpiiLzpxcJTMhu0WXaspcc74eKGOWPmAVJu42y1hXH-xVBlLJI_WG5dACQTNlgXtLsmSImOyCrQ2lg8qN-KuHDDX8nvjlSF72TVLDlA1hcnxyQE6Zz51LPF3l9TkxftxcaZ14BhleV17x8PZjZqcHB30ksw1ushaqlOBBVpgl8hiSUF5zRHLLUgpjtG4NSJ_6tgpxuSPg8yicZFDMm59iqpYHB0dHU2n8_OqqNybPsGtvw5edyo9Ahk25hqHr8eHaR8aDg3wXj3Ck1rfGZHCuHXlhlO325af5ZXNuYbrrH1ygewOl4z0XAejGJqVachOjo_Kk4GUjxS4xCITUeKGw8YolsBftBEWstD80FqjTBrMFsVMcMw6e0H5prX8gE8dByNBj-iDvCxJRwfmxBGr4Bu0XVA1RXEL2r7mWOZu8F2C9Zl_fmjJaVoaa1LfH7M_T2FWiMmrO_yOlwL7Jc4uz58Cl8CV1T2il7ha7Acv-X4MTqfTyl2wFOO0ifwtxz13cvGNXw3YyaLL1S29t4geqaYEy2tSW1zMbs9wb6yF4434nJHIuMQ6l5-bN6ZO3Unfus6WmceBANkzTc-z3YJGcw7hnGGQJ5aInaqlj6vivRj5HXNVwAcxZmccawHdckjbqgDZ6FHThgfmKF9uS9T-XurMZWxBgeFXu3KS0FUV45VVUaJhcoJ9qKzxG5E7z6pV9itEvsxIPp28UEBKdYEST3dW_pGNv-vgPXn6176jmagl7f7rLwc7w19tOASj94yf0ecpk-mQOTXuzOOiH6dZ3178q6BlJAs9ZO9pG9GSkWmAlbSdgK1llKxNTBymlcsHHjGO062vfHwa0L2VQWS8OxcVZdwm05vumrYNu3Tr97Llxz5VXg55zhrfubTf4MCCFZsN6738yON-suDWknSlKMp6p7107Bvfres06HEwoJonbn5ayzjYDl9jfzBOhw157QUOD_v62gTvrFkLEzo-FfP45LdHNZ-j-7sjl0zaioZPUvbu5ESY5MloVlnYvG0iVmSjfHaA2UPLKrFecOrCtxrsLNHH9hjcRfuZPRmYKEfAOXv-5jhtNkZ3ZKFNVFmkVr6jiZsSvD7F138ToqOQSrzC6uuXgJMS8euX5HwMtKSU4ONGBtVOgQuvzco8d0Xi5oka8vhTd1CUvy67rhkuyXT_6Jb_4b0n2Yt__gM)

### Transaction view

This view is presented when the cashier chooses "Enter transaction" from the main menu.  
If there are no transactions, a new one is automatically created.  
If there exists unfinished transactions, the cashier will be presented ongoing transactions in a date-ordered list.  
The cashier can select a transaction using its ID, or create a new one by pressing n.

When the transaction state is active, the program activates the scanner thread and listens asynchronously for
updates via the Virtual COM port. The cashier now has 2 interfaces to interact with items:

- The scanner, which can add or remove items.
- The TUI, which has full control over the transaction.

When the transaction is finalized (Cashier presses F and a payment is made),
the transaction is added to the order table.

Note: Any transaction containing a rental item or service (such as "Åverkansvite" or "Förseningsavgift")
must have a customer bound to the order. The cashier will be prompted about this when entering the finalize stage, if no customer is already added.

```mermaid
flowchart TD
    %% Define styling classes to match the vibe
    classDef blueBox fill:#cce5ff,stroke:#0066cc,stroke-width:2px,color:#000;
    classDef yellowDiamond fill:#fff2cc,stroke:#d6b656,stroke-width:2px,color:#000;
    classDef greenBox fill:#d5e8d4,stroke:#82b366,stroke-width:2px,color:#000;
    classDef redOval fill:#f8cecc,stroke:#b85450,stroke-width:2px,color:#000;
    classDef mockupBox fill:#e5e5e5,stroke:#999,stroke-width:1px,color:#000;

    %% Main Application Flow
    subgraph Flow [Admin View Transaction Flow]
        direction LR
        Start[Start transaction]:::blueBox --> Menu[Admin Menu]:::blueBox

        Menu -- "New / Load Order<br>(Start 2nd thread listener)" --> ScanMenu[Transaction Loop]:::blueBox
        ScanMenu -- "key: s (Save)<br>key: a (Abort)<br>(Kill listener)" --> Menu
        ScanMenu -- "key: f (Finalize)<br>(Kill listener)" --> Checkout[Payment / Finalize]:::blueBox
        Checkout --> Menu

        ScanMenu -- "Scan / Enter<br>Item" --> Action{Action}:::yellowDiamond

        Action -- "Scan same" --> AmountPlus[Amount++]:::greenBox
        AmountPlus --> ScanMenu

        Action -- "Scan new" --> ScanMenu

        %% Sending state is a broadcast triggered by the Scan Menu updates
        ScanMenu -. "State Broadcast" .-> SendState([SEND NEW STATE]):::redOval
    end

    %% Contextual UI Mockups
    subgraph UI [Contextual TUI States]
        direction LR
        RootView["<div style='text-align:left;'><b>TRANSACTION (NO INSERT)</b><br><br>---- Order ----<br><i>No items scanned yet (or all cleared)</i><br><br>--------------<br>key: c = Enter barcode<br><br>key: u = Add customer to order<br>key: o = Add order modifier<br>key: f = Finalize transaction<br>key: s = Save & Go back<br>key: a = Abort transaction</div>"]:::mockupBox

        Rental["<div style='text-align:left;'><b>RENTAL</b><br><br>---- Order ----<br>Pepsi max x12 &nbsp;&nbsp;&nbsp;&nbsp;23 SEK<br>---- Insert ----<br>Die hard (rental)<br>-- Return By: 1998-08-08<br>SEK: 35<br>--------------<br>key: c = Enter barcode<br>key: p = Change root price<br>key: m = Add item modifier<br>key: d = Change return date<br>key: e = Deselect (Back to root)</div>"]:::mockupBox

        Product["<div style='text-align:left;'><b>PRODUCT</b><br><br>---- Order ----<br>Ipren x4 &nbsp;&nbsp;&nbsp;&nbsp;2032 SEK<br>---- Insert ----<br>Monster energy x3<br>--------------<br>key: c = Enter barcode<br>key: m = Add item modifier<br>key: a = Change amount<br>key: e = Deselect (Back to root)</div>"]:::mockupBox

    end

    %% Connect the state broadcast to the UI updates
    SendState -. "Renders Contextual View" .-> UI
```
