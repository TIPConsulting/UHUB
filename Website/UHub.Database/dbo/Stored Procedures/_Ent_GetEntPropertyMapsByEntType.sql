create proc _Ent_GetEntPropertyMapsByEntType

	@EntTypeID smallint

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

	where
		EntTypeID = @EntTypeID


end