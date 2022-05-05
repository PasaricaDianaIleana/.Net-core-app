CREATE PROCEDURE [dbo].[spProduce_GetAll]
AS
begin
SET nocount on;
	SELECT Id, ProductName,[Description],QuantityInStock,RetailPrice
	from dbo.Product order by Product.ProductName
RETURN 0
end