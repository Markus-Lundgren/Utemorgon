-- Reduce stock after creating a OrderRow.
-- Note that an OrderRow may implicitly be a RentalRow, where the product does not contain a stock.
-- We do not want to call -1 on NULL.

CREATE TRIGGER AfterOrderRowInsert
AFTER INSERT ON OrderRow
FOR EACH ROW
	UPDATE Product p SET Stock = Stock - NEW.Quantity
	WHERE p.Id = (SELECT ProductId FROM BarcodeToProduct btp WHERE btp.Barcode = NEW.Barcode)
	AND p.Stock IS NOT NULL;