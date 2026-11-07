## Order flow

From the admin terminal, the cashier can choose "Start transaction".

## Transaction view

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
