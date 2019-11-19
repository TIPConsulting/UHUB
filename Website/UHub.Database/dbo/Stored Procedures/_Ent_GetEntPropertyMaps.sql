create proc _Ent_GetEntPropertyMaps
as
begin


	select
		EntTypeID,
		PropID,
		DataTypeID,
		OverrideLength,
		OverridePrecision,
		IsNullable,
		DefaultValue,
		[Description]
		
	from EntPropertyMap


end