-- Parkodad: Markus, Timmy
-- Power-move procedure to create special, non colliding barcodes.


-- Creates a barcode of exactly 13 in length, with any special prefix.
-- IE: S000000000033 | R000000000042
DELIMITER //
CREATE PROCEDURE AddSpecialBarcodeToProduct(IN prefix VARCHAR(13), IN p_Id INT)
BEGIN
	INSERT INTO BarcodeToProduct (ProductId, Barcode, IsArchived)
	WITH Seq (Code) AS (
	SELECT CONCAT(prefix, LPAD(AUTO_INCREMENT, 13 - LENGTH(prefix), '0'))
	FROM information_schema.tables 
	WHERE table_name = 'BarcodeToProduct' 
	AND table_schema = DATABASE()
	)
	SELECT p_Id, Seq.Code, False
	FROM Seq
	RETURNING Barcode;
END //
DELIMITER ;