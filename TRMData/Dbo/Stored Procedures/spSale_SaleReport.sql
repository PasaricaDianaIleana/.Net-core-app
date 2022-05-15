CREATE PROCEDURE [dbo].[spSale_SaleReport]

AS BEGIN
 SET NOCOUNT ON;
	SELECT  [s].[SaleDate], [s].[SubTotal], [s].[Tax], 
	[s].[Total], u.FirstName, U.LastName, u.EmailAddress from dbo.Sale  s
	inner join dbo.[User] u on s.CashierId = u.Id
END
