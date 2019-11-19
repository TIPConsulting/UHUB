









CREATE proc [dbo].[SchoolMajors_GetByEmail]

	@Email nvarchar(250)

as
begin


	declare @_schoolID bigint

	declare @domain nvarchar(250)
	set @domain = SUBSTRING(@email, CHARINDEX('@', @email), 250)



	select 
		@_schoolID = xr.EntID
	from dbo.EntPropertyXRef xr
	where
		xr.PropID = 17		--DomainValidator
		and @domain = xr.PropValue



	select
		vu.ID,
		vu.IsEnabled,
		vu.IsReadonly,
		vu.[Name],
		vu.[Description],
		vu.[IsDeleted],
		vu.[CreatedBy],
		vu.[CreatedDate],
		vu.[ModifiedBy],
		vu.[ModifiedDate],
		vu.[DeletedBy],
		vu.[DeletedDate]

	from dbo.vSchoolMajors vu

	INNER JOIN dbo.EntChildXRef
	ON 
		ParentEntID = @_schoolID
		AND ChildEntID = vu.ID



end