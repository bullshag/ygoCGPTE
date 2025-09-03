-- Removes quantity from an item and deletes if none remain
UPDATE user_items SET quantity=quantity-@qty WHERE account_id=@id AND item_name=@name;
DELETE FROM user_items WHERE account_id=@id AND item_name=@name AND quantity<=0;
