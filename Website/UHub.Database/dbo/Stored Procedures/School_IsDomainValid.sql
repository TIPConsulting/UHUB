create proc [dbo].[School_IsDomainValid]

	@Domain nvarchar(250)

as
begin
	--not a great solution, but will work well enough for this project

	declare @_entID bigint

	select 
		@_entID = xr.EntID
	from dbo.EntPropertyXRef xr
	where
		xr.PropID = 17		--DomainValidator
		and @Domain like xr.PropValue


	if(@_entID is null)
	begin
		select cast(0 as bit)
	end
	else begin
		select cast(1 as bit)
	end

		
end