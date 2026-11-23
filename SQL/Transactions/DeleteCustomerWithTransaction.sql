-- Example of a transaction for deleting a customer
-- Move customer's orders to DELETED CUSTOMER (Id 1)
-- and delete the customer.
-- Both operations are committed together or rolled back if an error occurs.

DELIMITER //
CREATE PROCEDURE DeleteCustomerWithTransaction(IN p_Id INT)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    IF p_Id = 1 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Cannot delete the DELETED CUSTOMER.';
    END IF;

    START TRANSACTION;

    UPDATE `Order`
    SET CustomerId = 1
    WHERE CustomerId = p_Id;

    DELETE FROM Customer
    WHERE Id = p_Id;

    COMMIT;
END //
DELIMITER ;
