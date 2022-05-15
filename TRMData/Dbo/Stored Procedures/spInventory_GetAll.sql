CREATE PROCEDURE [dbo].[spInventory_GetAll]
AS Begin
	set nocount on;
	Select [Id], [ProductId], [Quantity], [PurchasePrice], [PurchaseDate]
	from dbo.Inventory
End
