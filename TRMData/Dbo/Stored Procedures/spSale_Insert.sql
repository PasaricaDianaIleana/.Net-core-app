CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id int output,
	@CashierId nvarchar(128),
   @SaleDate datetime2,
   @SubTotal money,
   @Tax  money,
   @Total money
AS BEGIN
set nocount on;
Insert into dbo.Sale(CashierId,SaleDate,SubTotal,Tax,Total)
 VALUES  (@CashierId,@SaleDate,@SubTotal,@Tax,@Total)

 Select @Id =  SCOPE_IDENTITY();
END
