create proc _Ent_GetEntPropertyRevisionMaps

as
begin


	select
		
		EntTypeID,
		PropID,
		[Description]
		
	from EntPropertyRevisionMap



end