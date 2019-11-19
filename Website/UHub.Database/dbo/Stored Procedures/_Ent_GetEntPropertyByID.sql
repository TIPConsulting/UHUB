
create proc [dbo].[_Ent_GetEntPropertyByID]

	@PropID int

as
begin


	select
		ID,
		PropName,
		PropFriendlyName,
		[Description],
		DataTypeID,
		DataType,
		DefaultLength,
		DefaultPrecision
		
	from EntProperties

	where
		ID = @PropID


end