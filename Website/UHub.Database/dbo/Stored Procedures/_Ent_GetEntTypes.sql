CREATE proc [dbo].[_Ent_GetEntTypes]
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


end