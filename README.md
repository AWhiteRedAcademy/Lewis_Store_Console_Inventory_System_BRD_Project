[README.md](https://github.com/user-attachments/files/26063240/README.md)
# Lewis Store Console Inventory System

## Week 4 User Manual / README

### 1\. Project Overview

The **Lewis Store Console Inventory System** is a C# console application used to manage basic stock in a small store scenario. The system allows the user to:

* add stock items
* view current inventory
* sell items
* calculate VAT at 15% on sales
* update stock quantities after each sale

The system is menu-driven and runs in a console window.

### 2\. Features

The system includes the following main features:

* **Add Item** – capture a product name, description, quantity, and price excluding VAT
* **View Inventory** – display all items currently stored in the system
* **Sell Items** – sell available stock, reduce the quantity, and calculate the total including VAT
* **Exit** – close the application safely

### 3\. System Requirements

To run the project from source code, the following is needed:

* **Operating System:** Windows recommended
* **Framework:** .NET SDK that supports `net10.0`
* **Language:** C#
* **Package used:** `Spectre.Console`
* **IDE (optional):** Visual Studio 2022 or later / Visual Studio Code

### 4\. How to Open and Run the Program

#### Option 1: Run in Visual Studio

1. Open the solution/project folder.
2. Open the file named:

   * `Lewis_Store_Console_Inventory_System_BRD.slnx`
3. Restore NuGet packages if prompted.
4. Build the solution.
5. Run the program.

#### Option 2: Run from terminal

Open a terminal in the project folder and use:

```bash
dotnet restore
dotnet run
```

#### Option 3: Run the compiled executable

If the project has already been built, the executable can be found in:

```text
bin/Debug/net10.0/
```

Then run the `.exe` file from that folder.

### 5\. Main Menu

When the program starts, the following menu is shown:

1. Add Item
2. View Inventory
3. Sell Items
4. Exit

The menu keeps repeating until the user chooses **Exit**.

### 6\. How to Use the System

#### 6.1 Add Item

Choose **1. Add Item** from the main menu.

The system will ask for:

* item name
* description
* quantity
* price excluding VAT

**Rules and validation:**

* item name cannot be empty
* description cannot be empty
* quantity must be a valid number
* quantity cannot be less than 0
* price must be a valid number
* price cannot be less than 0
* a maximum of **100 items** can be stored
* typing `CANCEL` at the item name prompt cancels the add process

**Result:**
If the input is valid, the item is saved into the inventory.

\---

#### 6.2 View Inventory

Choose **2. View Inventory** from the main menu.

The system will display all items currently in stock in a formatted table showing:

* item name
* description
* quantity
* price excluding VAT

This option allows the user to quickly check what products are available before making a sale.

\---

#### 6.3 Sell Items

Choose **3. Sell Items** from the main menu.

The system first displays the available stock. The user then:

1. enters the **item name**
2. enters the **quantity to buy**
3. repeats the process until checkout is complete

**Validation during sales:**

* the item must exist in inventory
* the quantity entered must be a valid number
* the quantity requested must not be more than the available stock
* typing `Cancel` cancels the current item selection

**What the system calculates:**

* total excluding VAT
* total including VAT
* VAT rate used = **15%**

**What happens after a sale:**

* sold stock is deducted from the item quantity
* checkout items are displayed
* final totals are shown at the end

### 7\. VAT Calculation

The system uses a fixed VAT rate of **15%**.

Formula used:

```text
Total including VAT = Price excluding VAT + (Price excluding VAT × 15%)
```

Example:

```text
Item price excl. VAT = R100.00
VAT = R15.00
Total incl. VAT = R115.00
```

### 8\. Example Usage Scenario

#### Example: Adding an item

* Name: Kettle
* Description: Stainless steel kettle
* Quantity: 10
* Price excl. VAT: 250

The item will be added successfully.

#### Example: Selling an item

If 2 kettles are sold:

```text
Subtotal excl. VAT = R500.00
VAT (15%) = R75.00
Total incl. VAT = R575.00
```

The stock quantity will update from **10** to **8**.

### 9\. Important Notes / Limitations

This system is intentionally simple and follows the project scope.

* Inventory is stored in memory while the program is running.
* There is **no database**.
* There is **no file saving**.
* All stock data is lost when the program closes.
* The inventory capacity is limited to **100 items**.
* The system is console-based only.

### 10\. Error Handling

The program includes validation and error handling for common mistakes, such as:

* invalid menu choices
* empty item name or description
* negative quantity or price
* trying to sell an item that does not exist
* trying to sell more than the available stock
* adding more than 100 items

When invalid input is entered, the program shows an error message and returns the user to the relevant step.

### 11\. Test Checklist

The following checks can be used during final testing:

* \[ ] Add a valid stock item
* \[ ] Reject empty item name
* \[ ] Reject empty description
* \[ ] Reject invalid quantity
* \[ ] Reject negative quantity
* \[ ] Reject invalid price
* \[ ] Reject negative price
* \[ ] View inventory after adding items
* \[ ] Sell an existing item successfully
* \[ ] Reject sale when stock is insufficient
* \[ ] Confirm VAT is calculated correctly
* \[ ] Confirm stock quantity updates after sale
* \[ ] Confirm the menu loops until Exit is selected

### 12\. File Structure

Important files in the project:

```text
Program.cs                                  Main program 
Lewis\\\_Store\\\_Console\\\_Inventory\\\_System\\\_BRD.csproj   Project file
Lewis\\\_Store\\\_Console\\\_Inventory\\\_System\\\_BRD.slnx     Solution file
README.md                                   User manual / readme
```

### 13\. Conclusion

The Lewis Store Console Inventory System meets the main requirements of the BRD by providing a simple stock management system in a console application. It allows users to add items, view stock, process sales, calculate VAT, and update quantities using a menu-driven GUI.

### 14\. Author

**Prepared by:** Aidan White, Abduraghmaan Cassiem
**Project:** Lewis Store Console Inventory System

