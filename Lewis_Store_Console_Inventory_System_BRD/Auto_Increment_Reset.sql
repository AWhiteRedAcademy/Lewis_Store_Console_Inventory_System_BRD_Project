ALTER TABLE Products
ALTER COLUMN ProductID int IDENTITY(1,1) PRIMARY KEY;
DBCC CHECKIDENT ('Products', RESEED, 0);

ALTER TABLE Sales
ALTER COLUMN SaleID int identity(1,1) PRIMARY KEY;
DBCC CHECKIDENT ('Sales', RESEED, 0);

SELECT * FROM SALES

ALTER TABLE Sales
ADD SaleID_New INT IDENTITY(1,1);

-- Drop the old primary key constraint
ALTER TABLE Sales
DROP CONSTRAINT PK_Sales;

-- Drop the old SaleID column
ALTER TABLE Sales
DROP COLUMN SaleID;

-- Rename the new column to SaleID
EXEC sp_rename 'Sales.SaleID_New', 'SaleID', 'COLUMN';

-- Add primary key constraint
ALTER TABLE Sales
ADD CONSTRAINT PK_Sales PRIMARY KEY (SaleID);

-- Reset identity seed
DBCC CHECKIDENT ('Sales', RESEED, 0);