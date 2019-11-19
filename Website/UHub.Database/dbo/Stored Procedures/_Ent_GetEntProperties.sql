create proc _Ent_GetEntProperties
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


end