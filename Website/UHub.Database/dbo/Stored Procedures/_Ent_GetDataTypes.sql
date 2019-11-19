create proc _Ent_GetDataTypes
as
begin


	select
		ID,
		[Name],
		DefaultLength,
		DefaultPrecision
	from DataTypes


end