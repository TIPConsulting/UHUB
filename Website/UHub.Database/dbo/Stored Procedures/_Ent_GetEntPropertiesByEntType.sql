create proc [dbo].[_Ent_GetEntPropertiesByEntType]

	@EntTypeID smallint

as
begin


	select
		ep.ID,
		ep.PropName,
		ep.PropFriendlyName,
		ep.[Description],
		ep.DataTypeID,
		ep.DataType,
		ep.DefaultLength,
		ep.DefaultPrecision
	from EntProperties ep

	inner join dbo.EntPropertyMap epm
	on
		epm.EntTypeID = @EntTypeID
		and epm.PropID = ep.ID


end