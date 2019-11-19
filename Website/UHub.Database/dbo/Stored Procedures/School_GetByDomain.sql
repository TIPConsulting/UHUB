create proc [dbo].[School_GetByDomain]

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
		and @Domain = xr.PropValue


	select top(1) 
		vu.ID,
		vu.IsEnabled,
		vu.IsReadonly,
		vu.[Name],
		vu.[State],
		vu.[City],
		vu.[DomainValidator],
		vu.[Description],
		vu.[IsDeleted],
		vu.[CreatedBy],
		vu.[CreatedDate],
		vu.[ModifiedBy],
		vu.[ModifiedDate],
		vu.[DeletedBy],
		vu.[DeletedDate]
	from dbo.vSchools vu
	where
		vu.ID = @_entID

		
end