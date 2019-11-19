CREATE proc [dbo].[_Ent_GetEntTypeByID]

	@EntTypeID smallint

as
begin


	select
		ID,
		[UID],
		[Name],
		[Description],
		AutoViewSchema,
		AutoViewName

	from EntityTypes
	where
		ID = @EntTypeID


end