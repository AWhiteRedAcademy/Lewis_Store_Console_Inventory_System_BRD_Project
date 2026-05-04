
CREATE DATABASE LEWIS_STORE_STOCK;

use LEWIS_STORE_STOCK

CREATE Table Sales(
	SaleID	int IDENTITY(1,1) PRIMARY KEY,
	Subtotal	decimal(10,2) NOT NULL,
	VATAmount	decimal(10,2) NOT NULL,
	TotalAmount	decimal(10,2) NOT NULL,
	SalesDate	DATETIME NOT NULL,	
)

CREATE Table Products(
	ProductID int IDENTITY(1,1) PRIMARY KEY,
	ProductName	varchar(100) NOT NULL,
	Description varchar(255),
	QuantityInStock	int NOT NULL,
	PriceExcludingVAT decimal(10, 2) NOT NULL
)

CREATE TABLE SaleItems(
	SaleItemID int IDENTITY(1,1) PRIMARY KEY,
	SaleID int,
	ProductID int,
	Quantity int,
	SalePrice DECIMAL(10,2),
	FOREIGN KEY (SaleID) REFERENCES Sales(SaleID),
	FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
)
