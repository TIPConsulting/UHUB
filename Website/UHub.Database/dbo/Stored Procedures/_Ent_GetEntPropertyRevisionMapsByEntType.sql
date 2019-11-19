create proc _Ent_GetEntPropertyRevisionMapsByEntType

	@EntTypeID smallint

as
begin


	select
		
		EntTypeID,
		PropID,
		[Description]
		
	from EntPropertyRevisionMap

	where
		EntTypeID = @EntTypeID


end