









CREATE proc [dbo].[SchoolClubs_GetByDomain]

	@Domain nvarchar(250)

as
begin


	declare @_schoolID bigint

	select 
		@_schoolID = xr.EntID
	from dbo.EntPropertyXRef xr
	where
		xr.PropID = 17		--DomainValidator
		and @Domain = xr.PropValue



	select
		vu.ID,
		vu.IsEnabled,
		vu.IsReadonly,
		vu.[Name],
		vu.[Description],
		vu.[ParentID],
		vu.[IsDeleted],
		vu.[CreatedBy],
		vu.[CreatedDate],
		vu.[ModifiedBy],
		vu.[ModifiedDate],
		vu.[DeletedBy],
		vu.[DeletedDate]

	from dbo.vSchoolClubs vu

	INNER JOIN dbo.EntChildXRef
	ON 
		ParentEntID = @_schoolID
		AND ChildEntID = vu.ID



end