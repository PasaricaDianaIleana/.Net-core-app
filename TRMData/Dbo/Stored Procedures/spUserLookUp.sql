	CREATE PROCEDURE [dbo].[spUserLookUp]
		@Id nvarchar(128) 
	AS
	Begin
	 set nocount on;
		SELECT Id,FirstName,LastName,EmailAddress , CreatedData
		from [dbo].[User]
	    where Id= @Id;
	End