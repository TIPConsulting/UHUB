create proc _Ent_GetEntChildMaps

as
begin


	select
		
		ParentEntType,
		ChildEntType,
		[Description]
		
	from EntChildMap



end