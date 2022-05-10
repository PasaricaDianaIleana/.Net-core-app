CREATE PROCEDURE [dbo].[spProduce_GetById]
	@id int
AS Begin

SET nocount on;
	SELECT Id, ProductName,[Description],
	QuantityInStock,RetailPrice, IsTaxable
	from dbo.Product Where Id = @id
end

